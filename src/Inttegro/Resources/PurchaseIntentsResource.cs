using Inttegro.Http;

namespace Inttegro.Resources;

public class PurchaseIntentsResource
{
    private readonly ApiClient _client;

    internal PurchaseIntentsResource(ApiClient client) => _client = client;

    public Task<PurchaseIntent> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/create", "purchase_intent", payload, cancellationToken);

    public Task<PurchaseIntent> CreateAsync(CreatePurchaseIntentParams payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/create", "purchase_intent", payload, cancellationToken);

    public Task<PurchaseIntent> LookupAsync(string id, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/lookup", "purchase_intent", new { id }, cancellationToken);

    public Task<PurchaseIntent> LookupAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/lookup", "purchase_intent", payload, cancellationToken);

    public Task<PurchaseIntentPage> PageAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntentPage>("/purchase_intents/page", "page", payload, cancellationToken);

    public Task<PurchaseIntent> UpdateAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/update", "purchase_intent", payload, cancellationToken);

    public Task<PurchaseIntent> UpdateAsync(UpdatePurchaseIntentParams payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/update", "purchase_intent", payload, cancellationToken);

    public Task<PurchaseIntent> CancelAsync(string id, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/cancel", "purchase_intent", new { id }, cancellationToken);

    public Task<PurchaseIntent> CancelAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PurchaseIntent>("/purchase_intents/cancel", "purchase_intent", payload, cancellationToken);
}
