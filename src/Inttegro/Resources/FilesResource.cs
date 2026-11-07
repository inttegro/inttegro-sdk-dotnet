using Inttegro.Http;

namespace Inttegro.Resources;

public sealed class FilesResource
{
    private readonly ApiClient _client;

    internal FilesResource(ApiClient client) => _client = client;

    public Task<StoredFile> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        CreateAsync(payload, idempotencyKey: null, cancellationToken);

    public Task<StoredFile> CreateAsync(object payload, string? idempotencyKey, CancellationToken cancellationToken = default)
    {
        var values = ToDictionary(payload);
        if (values.Keys.Any(key =>
            string.Equals(key, "idempotency_key", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(key, "IdempotencyKey", StringComparison.Ordinal)))
        {
            throw new ArgumentException(
                "top-level idempotency_key is not supported; pass the idempotencyKey method argument to use the Idempotency-Key header"
            );
        }
        var headers = Headers(idempotencyKey);
        var file = values["file"]?.ToString() ?? throw new ArgumentException("file is required");
        values.Remove("file");
        return _client.PostMultipartResourceAsync<StoredFile>("/files/create", "file", values, new Dictionary<string, string> { ["file"] = file }, headers, cancellationToken);
    }

    public Task<StoredFile> LookupAsync(string fileId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<StoredFile>("/files/lookup", "file", new { file_id = fileId }, cancellationToken);

    public Task<StoredFilePage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<StoredFilePage>("/files/page", "page", payload ?? new { }, cancellationToken);

    public Task<FileDownload> ContentsAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostBinaryJsonAsync("/files/contents", payload, cancellationToken);

    public Task<StoredFile> DeleteAsync(string fileId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<StoredFile>("/files/delete", "file", new { file_id = fileId }, cancellationToken);

    private static Dictionary<string, object?> ToDictionary(object payload) =>
        payload.GetType().GetProperties().ToDictionary(p => p.Name, p => p.GetValue(payload));

    private static IDictionary<string, string> Headers(string? idempotencyKey) =>
        idempotencyKey == null ? new Dictionary<string, string>() : new Dictionary<string, string> { ["Idempotency-Key"] = idempotencyKey };
}
