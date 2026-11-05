using Inttegro.Http;

namespace Inttegro.Resources;

public sealed class OperatingPoliciesResource
{
    private readonly ApiClient _client;

    internal OperatingPoliciesResource(ApiClient client) => _client = client;

    public Task<OperatingPolicy> CreateAsync(CreateOperatingPolicyRequest request, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<OperatingPolicy>("/operating_policies/create", "operating_policy", request, cancellationToken);

    public Task<OperatingPolicy> LookupAsync(string operatingPolicyId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<OperatingPolicy>(
            "/operating_policies/lookup",
            "operating_policy",
            new { operating_policy_id = operatingPolicyId },
            cancellationToken
        );

    public Task<OperatingPolicy> UpdateAsync(UpdateOperatingPolicyRequest request, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<OperatingPolicy>("/operating_policies/update", "operating_policy", request, cancellationToken);
}
