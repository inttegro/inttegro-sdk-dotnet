using Inttegro.Http;

namespace Inttegro.Resources;

public sealed class OperatingPoliciesResource
{
    private readonly ApiClient _client;

    internal OperatingPoliciesResource(ApiClient client) => _client = client;

    public Task<OperatingPolicy> CreateAsync(CreateOperatingPolicyRequest request, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<OperatingPolicy>("/operating_policies/create", "operating_policy", request, cancellationToken);

    /// <summary>Retrieves the latest active version of an operating policy.</summary>
    public Task<OperatingPolicy> LookupAsync(string operatingPolicyId, CancellationToken cancellationToken = default) =>
        LookupAsync(new LookupOperatingPolicyRequest { OperatingPolicyId = operatingPolicyId }, cancellationToken);

    /// <summary>Retrieves an exact immutable version of an operating policy.</summary>
    public Task<OperatingPolicy> LookupAsync(
        string operatingPolicyId,
        int version,
        CancellationToken cancellationToken = default
    ) => LookupAsync(
        new LookupOperatingPolicyRequest { OperatingPolicyId = operatingPolicyId, Version = version },
        cancellationToken
    );

    public Task<OperatingPolicy> LookupAsync(
        LookupOperatingPolicyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ValidateOperatingPolicyId(request.OperatingPolicyId);
        ValidateVersion(request.Version);
        return _client.PostResourceAsync<OperatingPolicy>(
            "/operating_policies/lookup",
            "operating_policy",
            request,
            cancellationToken
        );
    }

    /// <summary>Lists immutable versions of one operating policy, newest first.</summary>
    public Task<OperatingPolicyPage> PageAsync(
        PageOperatingPoliciesRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ValidateOperatingPolicyId(request.OperatingPolicyId);
        return _client.PostResourceAsync<OperatingPolicyPage>(
            "/operating_policies/page",
            "page",
            request,
            cancellationToken
        );
    }

    public Task<OperatingPolicy> UpdateAsync(UpdateOperatingPolicyRequest request, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<OperatingPolicy>("/operating_policies/update", "operating_policy", request, cancellationToken);

    private static void ValidateOperatingPolicyId(string operatingPolicyId)
    {
        if (string.IsNullOrWhiteSpace(operatingPolicyId))
            throw new ArgumentException("operatingPolicyId is required", nameof(operatingPolicyId));
    }

    private static void ValidateVersion(int? version)
    {
        if (version is < 1) throw new ArgumentOutOfRangeException(nameof(version), "version must be at least 1");
    }
}
