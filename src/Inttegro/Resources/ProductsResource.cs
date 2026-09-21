using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class ProductsResource
{
    private readonly ApiClient _client;

    internal ProductsResource(ApiClient client) => _client = client;

    public Task<Product> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/create", "product", payload, cancellationToken);

    public Task<Product> CreateAsync(CreateProductRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.Type, "type");
        RequestValidator.Require(payload.Name, "name");
        return _client.PostResourceAsync<Product>("/products/create", "product", payload, cancellationToken);
    }

    public Task<ProductDefaultUnitPrice> AddPriceAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ProductDefaultUnitPrice>("/products/add_price", "price", payload, cancellationToken);

    public Task<ProductDefaultUnitPrice> AddPriceAsync(AddProductPriceRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        RequestValidator.Require(payload.Amount, "amount");
        return _client.PostResourceAsync<ProductDefaultUnitPrice>("/products/add_price", "price", payload, cancellationToken);
    }

    public Task<Product> SetDefaultUnitPriceAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/set_default_unit_price", "product", payload, cancellationToken);

    public Task<Product> SetDefaultUnitPriceAsync(SetDefaultUnitPriceRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        RequestValidator.Require(payload.PriceId, "price_id");
        return _client.PostResourceAsync<Product>("/products/set_default_unit_price", "product", payload, cancellationToken);
    }

    public Task<Product> LookupAsync(string productId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/lookup", "product", new { product_id = productId }, cancellationToken);

    public Task<Product> LookupAsync(LookupProductRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        return _client.PostResourceAsync<Product>("/products/lookup", "product", payload, cancellationToken);
    }

    public Task<Product> UpdateAsync(UpdateProductRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        return _client.PostResourceAsync<Product>("/products/update", "product", payload, cancellationToken);
    }

    public Task<Product> UpdateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/update", "product", payload, cancellationToken);

    public Task<Product> PublishAsync(ProductActionRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        return _client.PostResourceAsync<Product>("/products/publish", "product", payload, cancellationToken);
    }

    public Task<Product> PublishAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/publish", "product", payload, cancellationToken);

    public Task<Product> UnpublishAsync(ProductActionRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        return _client.PostResourceAsync<Product>("/products/unpublish", "product", payload, cancellationToken);
    }

    public Task<Product> UnpublishAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/unpublish", "product", payload, cancellationToken);

    public Task<Product> ArchiveAsync(ProductActionRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.ProductId, "product_id");
        return _client.PostResourceAsync<Product>("/products/archive", "product", payload, cancellationToken);
    }

    public Task<Product> ArchiveAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Product>("/products/archive", "product", payload, cancellationToken);

    public Task<ProductPage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ProductPage>("/products/page", "page", payload ?? new { }, cancellationToken);

    public Task<ProductPage> PageAsync(PageProductsRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ProductPage>("/products/page", "page", payload, cancellationToken);

    public Task<ResourceSearchPage> SearchAsync(ResourceSearchRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ResourceSearchPage>("/products/search", "search", payload, cancellationToken);
}
