using System.IO.Pipes;
using System.Text;
using System.Text.Json;

namespace UoFiddler.Plugin.ResourceAccess;

/// <summary>One bounded JSON request per connection, accessible only to this Windows user.</summary>
internal sealed class ResourcePipeServer : IDisposable
{
    private readonly Func<ResourceRequest, CancellationToken, Task<object>> _execute;
    private readonly CancellationTokenSource _stop = new();
    private readonly NamedPipeServerStream _pipe;
    private Task? _loop;
    private int _disposed;

    public string PipeName { get; }
    public Task Completion => _loop ?? Task.CompletedTask;

    public ResourcePipeServer(string pipeName, Func<ResourceRequest, CancellationToken, Task<object>> execute)
    {
        PipeName = pipeName;
        _execute = execute;
        _pipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut, 1,
            PipeTransmissionMode.Byte, PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly);
    }

    public void Start()
    {
        if (_loop != null)
        {
            throw new InvalidOperationException("The pipe server has already started.");
        }
        _loop = Task.Run(RunAsync);
    }

    private async Task RunAsync()
    {
        CancellationToken stop = _stop.Token;
        try
        {
            while (!stop.IsCancellationRequested)
            {
                await _pipe.WaitForConnectionAsync(stop).ConfigureAwait(false);
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stop);
                timeout.CancelAfter(TimeSpan.FromSeconds(30));
                try
                {
                    ResourceResponse response;
                    try
                    {
                        string json = await ReadRequestAsync(_pipe, timeout.Token).ConfigureAwait(false);
                        ResourceRequest request = JsonSerializer.Deserialize<ResourceRequest>(json, Protocol.RequestOptions)
                            ?? throw new ResourceException("invalid_request", "A request object is required.");
                        object data = await _execute(request, timeout.Token).ConfigureAwait(false);
                        response = new(true, data);
                    }
                    catch (Exception ex) when (ex is not OutOfMemoryException)
                    {
                        response = Protocol.Failure(ex);
                    }
                    byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response, Protocol.JsonOptions) + "\n");
                    await _pipe.WriteAsync(bytes, timeout.Token).ConfigureAwait(false);
                    await _pipe.FlushAsync(timeout.Token).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is IOException or OperationCanceledException)
                {
                    // A disconnected or idle client must not prevent the next request.
                }
                finally
                {
                    if (!stop.IsCancellationRequested)
                    {
                        // IsConnected becomes false on a broken pipe, but the instance still
                        // needs Disconnect before it can accept another client.
                        _pipe.Disconnect();
                    }
                }
            }
        }
        catch (Exception ex) when (stop.IsCancellationRequested && ex is OperationCanceledException or ObjectDisposedException or IOException)
        {
            // Unload closes both pending accepts and pending client reads.
        }
    }

    private static async Task<string> ReadRequestAsync(Stream stream, CancellationToken cancellation)
    {
        using var buffer = new MemoryStream();
        byte[] chunk = new byte[1024];
        bool tooLarge = false;
        while (true)
        {
            int read = await stream.ReadAsync(chunk, cancellation).ConfigureAwait(false);
            if (read == 0)
            {
                throw new ResourceException("invalid_request", "End the JSON request with a newline.");
            }
            for (int i = 0; i < read; i++)
            {
                if (chunk[i] == '\n')
                {
                    if (tooLarge)
                    {
                        throw new ResourceException("request_too_large", "The request exceeds 16 KiB.");
                    }
                    return new UTF8Encoding(false, true).GetString(buffer.ToArray());
                }
                if (buffer.Length >= Protocol.MaxRequestBytes)
                {
                    tooLarge = true;
                }
                else
                {
                    buffer.WriteByte(chunk[i]);
                }
            }
        }
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }
        _stop.Cancel();
        _pipe.Dispose();
        // CancellationTokenSource remains alive until the loop exits.
        _ = Completion.ContinueWith(_ => _stop.Dispose(), TaskScheduler.Default);
    }
}
