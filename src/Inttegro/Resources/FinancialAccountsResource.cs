using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class FinancialAccountsResource
{
    private readonly ApiClient _client;

    internal FinancialAccountsResource(ApiClient client) => _client = client;

    public Task<FinancialAccount> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/create", "account", payload, cancellationToken);

    public Task<FinancialAccount> CreateAsync(FinancialAccountCreateRequest payload, CancellationToken cancellationToken = default) =>
        CreateAsync(payload, validateOwner: true, cancellationToken);

    private Task<FinancialAccount> CreateAsync(FinancialAccountCreateRequest payload, bool validateOwner, CancellationToken cancellationToken)
    {
        if (validateOwner)
        {
            RequestValidator.Require(payload.Owner, "owner");
        }
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/create", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> LookupAsync(string accountId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/lookup", "account", new { account_id = accountId }, cancellationToken);

    public Task<FinancialAccount> LookupAsync(FinancialAccountLookupRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/lookup", "account", payload, cancellationToken);

    public Task<FinancialAccount> ReconnectAsync(string accountId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/reconnect", "account", new { account_id = accountId }, cancellationToken);

    public Task<FinancialAccount> ReconnectAsync(FinancialAccountLookupRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/reconnect", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> ConnectAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/connect", "account", payload, cancellationToken);

    public Task<FinancialAccount> ConnectAsync(FinancialAccountCreateRequest payload, CancellationToken cancellationToken = default) =>
        ConnectAsync(payload, validateOwner: true, cancellationToken);

    private Task<FinancialAccount> ConnectAsync(FinancialAccountCreateRequest payload, bool validateOwner, CancellationToken cancellationToken)
    {
        if (validateOwner)
        {
            RequestValidator.Require(payload.Owner, "owner");
        }
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/connect", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> UpdateAsync(FinancialAccountUpdateRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/update", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> ArchiveAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/archive", "account", payload, cancellationToken);

    public Task<FinancialAccount> ArchiveAsync(FinancialAccountArchiveRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/archive", "account", payload, cancellationToken);

    public Task<FinancialAccountPage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccountPage>("/financial_accounts/page", "page", payload ?? new { }, cancellationToken);

    public Task<FinancialAccountPage> PageAsync(FinancialAccountPageRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccountPage>("/financial_accounts/page", "page", payload, cancellationToken);

    public Task<ResourceSearchPage> SearchAsync(ResourceSearchRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ResourceSearchPage>("/financial_accounts/search", "search", payload, cancellationToken);

    public Task<FinancialAccount> VerifyAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/verify", "account", payload, cancellationToken);

    public Task<FinancialAccount> VerifyAsync(FinancialAccountVerifyRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<FinancialAccount>("/financial_accounts/verify", "account", payload, cancellationToken);

    public Task<FinancialAccount> EnablePushAsync(FinancialAccountToggleRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/enable_push", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> DisablePushAsync(FinancialAccountToggleRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/disable_push", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> EnablePullAsync(FinancialAccountToggleRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/enable_pull", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> DisablePullAsync(FinancialAccountToggleRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/disable_pull", "account", payload, cancellationToken);
    }

    public Task<FinancialAccount> DisconnectAsync(FinancialAccountToggleRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.AccountId, "account_id");
        return _client.PostResourceAsync<FinancialAccount>("/financial_accounts/disconnect", "account", payload, cancellationToken);
    }
}
