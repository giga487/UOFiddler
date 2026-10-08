namespace UoFiddler.Plugin.ResourceAccess;

internal sealed class ResourceAccessControl : UserControl
{
    private readonly string _pipeName = $"UOFiddler.ResourceAccess.{Environment.ProcessId}";
    private readonly Label _status;
    private readonly Button _start;
    private readonly Button _stop;
    private ResourcePipeServer? _server;
    private ResourceService _service = new();
    private string? _questSource;

    public ResourceAccessControl()
    {
        Dock = DockStyle.Fill;
        var layout = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(16) };
        layout.Controls.Add(new Label { AutoSize = true, Text = "Read and replace images in the active profile.", Margin = new Padding(0, 0, 0, 12) });
        layout.Controls.Add(new Label { AutoSize = true, Text = "Start the connection, then run Invoke-FiddlerResource.ps1 from PowerShell." });
        layout.Controls.Add(new TextBox { ReadOnly = true, Dock = DockStyle.Top, Text = _pipeName });
        var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true };
        _start = new Button { AutoSize = true, Text = "Start connection" };
        _stop = new Button { AutoSize = true, Text = "Stop connection", Enabled = false };
        var copy = new Button { AutoSize = true, Text = "Copy status command" };
        var chooseQuest = new Button { AutoSize = true, Text = "Choose quest JSON" };
        _start.Click += (_, _) => StartConnection();
        _stop.Click += (_, _) => StopConnection();
        copy.Click += (_, _) => Clipboard.SetText($"& '{Path.Combine(AppContext.BaseDirectory, "plugins", "Invoke-FiddlerResource.ps1")}' -PipeName '{_pipeName}' -Command status");
        chooseQuest.Click += (_, _) =>
        {
            using var dialog = new OpenFileDialog { Filter = "Quest JSON files (*.json)|*.json", CheckFileExists = true, RestoreDirectory = true };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _questSource = dialog.FileName;
            }
        };
        buttons.Controls.AddRange([_start, _stop, copy, chooseQuest]);
        layout.Controls.Add(buttons);
        _status = new Label { AutoSize = true, Text = "Connection stopped." };
        layout.Controls.Add(_status);
        layout.Controls.Add(new Label { AutoSize = true, MaximumSize = new Size(720, 0), Margin = new Padding(0, 16, 0, 0),
            Text = "Commands: status, files, read, item, land, gump, font, cliloc, quest, import, save.\r\n" +
                "Images and tile properties reflect the current Fiddler session. Font, text, quest and byte-range reads reflect files on disk.\r\n" +
                "Import replaces an image in this session. Save writes a complete art or gump collection to a separate output folder, with backups of existing output files.\r\n" +
                "The connection is available only to your Windows user." });
        Controls.Add(layout);
    }

    internal void StartConnection()
    {
        if (_server != null)
        {
            return;
        }
        try
        {
            _ = Handle;
            _service = new ResourceService(() => _questSource);
            _server = new ResourcePipeServer(_pipeName, ExecuteOnUiAsync);
            _server.Start();
            _start.Enabled = false;
            _stop.Enabled = true;
            _status.Text = "Connection running. Waiting for requests.";
            _ = ObserveServerAsync(_server);
        }
        catch (Exception ex)
        {
            StopConnection();
            _status.Text = $"Could not start connection: {ex.Message}";
        }
    }

    private async Task<object> ExecuteOnUiAsync(ResourceRequest request, CancellationToken cancellation)
    {
        var completion = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        using CancellationTokenRegistration registration = cancellation.Register(() => completion.TrySetCanceled(cancellation));
        if (IsDisposed || !IsHandleCreated)
        {
            throw new OperationCanceledException(cancellation);
        }
        BeginInvoke(new Action(() =>
        {
            if (completion.Task.IsCompleted || IsDisposed)
            {
                return;
            }
            try
            {
                completion.TrySetResult(_service.Execute(request));
            }
            catch (Exception ex)
            {
                completion.TrySetException(ex);
            }
        }));
        return await completion.Task.ConfigureAwait(false);
    }

    private async Task ObserveServerAsync(ResourcePipeServer server)
    {
        try
        {
            await server.Completion;
        }
        catch (Exception ex)
        {
            if (!IsDisposed && ReferenceEquals(_server, server))
            {
                StopConnection();
                _status.Text = $"Connection stopped: {ex.Message}";
            }
        }
    }

    internal void StopConnection()
    {
        _server?.Dispose();
        _server = null;
        _start.Enabled = true;
        _stop.Enabled = false;
        _status.Text = "Connection stopped.";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _server?.Dispose();
            _server = null;
        }
        base.Dispose(disposing);
    }
}
