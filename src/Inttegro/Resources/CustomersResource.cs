using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class CustomersResource
{
    private readonly ApiClient _client;

    internal CustomersResource(ApiClient client) => _client = client;

    public Task<Customer> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Customer>("/customers/create", "customer", payload, cancellationToken);

    public Task<Customer> CreateAsync(CreateCustomerRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.Name, "name");
        return _client.PostResourceAsync<Customer>("/customers/create", "customer", payload, cancellationToken);
    }

    public Task<Customer> LookupAsync(string customerId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Customer>("/customers/lookup", "customer", new { customer_id = customerId }, cancellationToken);

    public Task<Customer> LookupAsync(LookupCustomerRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.CustomerId, "customer_id");
        return _client.PostResourceAsync<Customer>("/customers/lookup", "customer", payload, cancellationToken);
    }

    public Task<Customer> UpdateAsync(UpdateCustomerRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.CustomerId, "customer_id");
        return _client.PostResourceAsync<Customer>("/customers/update", "customer", payload, cancellationToken);
    }

    public Task<CustomersPage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CustomersPage>("/customers/page", "page", payload ?? new { }, cancellationToken);

    public Task<CustomersPage> PageAsync(PageCustomersRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CustomersPage>("/customers/page", "page", payload, cancellationToken);

    public Task<ResourceSearchPage> SearchAsync(ResourceSearchRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ResourceSearchPage>("/customers/search", "search", payload, cancellationToken);
}
