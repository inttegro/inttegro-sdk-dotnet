using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class PricesResource
{
    private readonly ApiClient _client;

    internal PricesResource(ApiClient client) => _client = client;

    public Task<CatalogPrice> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CatalogPrice>("/prices/create", "price", payload, cancellationToken);

    public Task<CatalogPrice> CreateAsync(CatalogPriceParams payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.ValidatePriceDefinition(
            payload.Type,
            payload.FixedAmount,
            payload.CustomerSelectedAmount,
            payload.ProductId
        );
        return _client.PostResourceAsync<CatalogPrice>("/prices/create", "price", payload, cancellationToken);
    }

    public Task<CatalogPrice> LookupAsync(string priceId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CatalogPrice>("/prices/lookup", "price", new { price_id = priceId }, cancellationToken);

    public Task<CatalogPrice> LookupAsync(LookupPriceRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PriceId, "price_id");
        return _client.PostResourceAsync<CatalogPrice>("/prices/lookup", "price", payload, cancellationToken);
    }

    public Task<CatalogPrice> UpdateAsync(UpdatePriceRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PriceId, "price_id");
        return _client.PostResourceAsync<CatalogPrice>("/prices/update", "price", payload, cancellationToken);
    }

    public Task<CatalogPrice> ActivateAsync(string priceId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CatalogPrice>("/prices/activate", "price", new { price_id = priceId }, cancellationToken);

    public Task<CatalogPrice> DeactivateAsync(string priceId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CatalogPrice>("/prices/deactivate", "price", new { price_id = priceId }, cancellationToken);

    public Task<CatalogPrice> ArchiveAsync(string priceId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<CatalogPrice>("/prices/archive", "price", new { price_id = priceId }, cancellationToken);

    public Task<PricePage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PricePage>("/prices/page", "page", payload ?? new { }, cancellationToken);

    public Task<PricePage> PageAsync(PricePageRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PricePage>("/prices/page", "page", payload, cancellationToken);
}
