using Inttegro.Http;

namespace Inttegro.Resources;

public sealed class ChildrenPoliciesResource
{
    private readonly ApiClient _client;

    internal ChildrenPoliciesResource(ApiClient client) => _client = client;

    public Task<ChildrenPolicy> LookupAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ChildrenPolicy>("/children_policies/lookup", "children_policy", new { }, cancellationToken);
}
