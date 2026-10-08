using System.Text.Json;
using System.Text.Json.Serialization;

namespace UoFiddler.Plugin.ResourceAccess;

internal sealed record ResourceRequest
{
    public string Command { get; init; } = "status";
    public int? Id { get; init; }
    public string? File { get; init; }
    public long Offset { get; init; }
    public int Length { get; init; } = 256;
    public int Font { get; init; }
    public int? CodePoint { get; init; }
    public bool Unicode { get; init; }
    public string Language { get; init; } = "enu";
    public bool IncludeImage { get; init; } = true;
    public string? Kind { get; init; }
    public string? ImagePath { get; init; }
    public string? OutputDirectory { get; init; }
    public string Format { get; init; } = "mul";
}

internal sealed record ResourceError(string Code, string Message);
internal sealed record ResourceResponse(bool Ok, object? Data = null, ResourceError? Error = null);
internal sealed class ResourceException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

internal static class Protocol
{
    public const int MaxRequestBytes = 16 * 1024;
    public const int MaxReadBytes = 1024 * 1024;
    public const int MaxDocumentBytes = 64 * 1024 * 1024;
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        MaxDepth = 128
    };
    public static readonly JsonSerializerOptions RequestOptions = new(JsonOptions) { MaxDepth = 16 };

    public static ResourceResponse Failure(Exception exception) => exception switch
    {
        ResourceException error => new(false, Error: new(error.Code, error.Message)),
        JsonException => new(false, Error: new("invalid_request", "Invalid JSON or unsupported request property.")),
        System.Text.DecoderFallbackException => new(false, Error: new("invalid_request", "Requests must use valid UTF-8.")),
        EndOfStreamException => new(false, Error: new("invalid_file", "The resource file is truncated.")),
        InvalidDataException error => new(false, Error: new("invalid_file", error.Message)),
        FileNotFoundException error => new(false, Error: new("not_found", error.Message)),
        OperationCanceledException => new(false, Error: new("cancelled", "The request was cancelled.")),
        _ => new(false, Error: new("read_failed", exception.Message))
    };
}
