using Inttegro.Http;

namespace Inttegro.Resources;

public class AppsResource
{
    private readonly ApiClient _client;

    internal AppsResource(ApiClient client) => _client = client;

    public Task<App> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<App>("/apps/create", "app", payload, cancellationToken);

    public Task<App> LookupAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<App>("/apps/lookup", "app", new { }, cancellationToken);

    public Task<AppCapabilities> CapabilitiesAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<AppCapabilities>("/apps/capabilities", "capabilities", new { }, cancellationToken);

    public Task<App> UpdateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<App>("/apps/update", "app", payload, cancellationToken);
}
