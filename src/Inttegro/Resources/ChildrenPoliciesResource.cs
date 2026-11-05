using Inttegro.Http;

namespace Inttegro.Resources;

public sealed class ChildrenPoliciesResource
{
    private readonly ApiClient _client;

    internal ChildrenPoliciesResource(ApiClient client) => _client = client;

    /// <summary>Retrieves the latest active children-policy version.</summary>
    public Task<ChildrenPolicy> LookupAsync(CancellationToken cancellationToken = default) =>
        LookupAsync(new LookupChildrenPolicyRequest(), cancellationToken);

    /// <summary>Retrieves an exact children-policy version.</summary>
    public Task<ChildrenPolicy> LookupAsync(int version, CancellationToken cancellationToken = default) =>
        LookupAsync(new LookupChildrenPolicyRequest { Version = version }, cancellationToken);

    public Task<ChildrenPolicy> LookupAsync(
        LookupChildrenPolicyRequest request,
        CancellationToken cancellationToken = default
    )
    {
        ValidateVersion(request.Version);
        return _client.PostResourceAsync<ChildrenPolicy>(
            "/children_policies/lookup",
            "children_policy",
            request,
            cancellationToken
        );
    }

    /// <summary>Lists immutable children-policy versions, newest first.</summary>
    public Task<ChildrenPolicyPage> PageAsync(
        PageChildrenPoliciesRequest? request = null,
        CancellationToken cancellationToken = default
    ) => _client.PostResourceAsync<ChildrenPolicyPage>(
        "/children_policies/page",
        "page",
        request ?? new PageChildrenPoliciesRequest(),
        cancellationToken
    );

    private static void ValidateVersion(int? version)
    {
        if (version is < 1) throw new ArgumentOutOfRangeException(nameof(version), "version must be at least 1");
    }
}
