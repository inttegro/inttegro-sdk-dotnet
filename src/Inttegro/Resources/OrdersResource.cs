using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class OrdersResource
{
    private readonly ApiClient _client;

    internal OrdersResource(ApiClient client) => _client = client;

    public Task<Order> CreateAsync(object payload, CancellationToken cancellationToken = default) =>
        PostOrderAsync("/orders/create", payload, cancellationToken);

    public Task<Order> CreateAsync(OrderCreateRequest payload, CancellationToken cancellationToken = default)
    {
        ValidateCreate(payload);
        return PostOrderAsync("/orders/create", payload, cancellationToken);
    }

    public Task<InttegroResponse<Order>> CreateWithResponseAsync(
        object payload,
        CancellationToken cancellationToken = default
    ) =>
        _client.PostResourceWithResponseAsync<Order>(
            "/orders/create",
            "order",
            payload,
            cancellationToken
        );

    public Task<InttegroResponse<Order>> CreateWithResponseAsync(
        OrderCreateRequest payload,
        CancellationToken cancellationToken = default
    )
    {
        ValidateCreate(payload);
        return _client.PostResourceWithResponseAsync<Order>(
            "/orders/create",
            "order",
            payload,
            cancellationToken
        );
    }

    public Task<Order> UpdateAsync(object payload, CancellationToken cancellationToken = default) =>
        PostOrderAsync("/orders/update", payload, cancellationToken);

    public Task<Order> UpdateAsync(OrderUpdateRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/update", payload, cancellationToken);
    }

    public Task<Order> LookupAsync(string orderId, object? options = null, CancellationToken cancellationToken = default)
    {
        var body = options as IDictionary<string, object?> ?? new Dictionary<string, object?>();
        body["order_id"] = orderId;
        return PostOrderAsync("/orders/lookup", body, cancellationToken);
    }

    public Task<Order> LookupAsync(OrderLookupRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/lookup", payload, cancellationToken);
    }

    public Task<Order> PayAsync(object payload, CancellationToken cancellationToken = default) =>
        PostOrderAsync("/orders/pay", payload, cancellationToken);

    public Task<Order> PayAsync(OrderPayRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/pay", payload, cancellationToken);
    }

    public Task<Order> ConfirmPaymentAsync(object payload, CancellationToken cancellationToken = default) =>
        PostOrderAsync("/orders/confirm_payment", payload, cancellationToken);

    public Task<Order> ConfirmPaymentAsync(OrderConfirmPaymentRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        RequestValidator.Require(payload.Token, "token");
        return PostOrderAsync("/orders/confirm_payment", payload, cancellationToken);
    }

    public Task<Order> RequestConfirmationAsync(string orderId, CancellationToken cancellationToken = default) =>
        RequestConfirmationAsync(new OrderRequestConfirmationRequest
        {
            OrderId = orderId,
            RequestMeta = StableOrderRequestMeta("request_confirmation", orderId)
        }, cancellationToken);

    public Task<Order> RequestConfirmationAsync(OrderRequestConfirmationRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/request_confirmation", payload, cancellationToken);
    }

    public Task<Order> FinalizeAsync(string orderId, CancellationToken cancellationToken = default) =>
        FinalizeAsync(new OrderFinalizeRequest
        {
            OrderId = orderId,
            RequestMeta = StableOrderRequestMeta("finalize", orderId)
        }, cancellationToken);

    public Task<Order> FinalizeAsync(OrderFinalizeRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/finalize", payload, cancellationToken);
    }

    public Task<OrderDocumentDeliveryResult> SendInvoiceAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostAsync<OrderDocumentDeliveryResult>("/orders/send_invoice", payload, cancellationToken);

    public Task<OrderDocumentDeliveryResult> SendInvoiceAsync(OrderSendInvoiceParams payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return _client.PostAsync<OrderDocumentDeliveryResult>("/orders/send_invoice", payload, cancellationToken);
    }

    public Task<OrderDocumentDeliveryResult> SendReceiptAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostAsync<OrderDocumentDeliveryResult>("/orders/send_receipt", payload, cancellationToken);

    public Task<OrderDocumentDeliveryResult> SendReceiptAsync(OrderSendReceiptParams payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return _client.PostAsync<OrderDocumentDeliveryResult>("/orders/send_receipt", payload, cancellationToken);
    }

    public Task<Order> CompleteAsync(object payload, CancellationToken cancellationToken = default) =>
        PostOrderAsync("/orders/complete", payload, cancellationToken);

    public Task<Order> CompleteAsync(OrderCompleteRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/complete", payload, cancellationToken);
    }

    public Task<Order> CancelAsync(string orderId, CancellationToken cancellationToken = default) =>
        CancelAsync(new OrderCancelRequest
        {
            OrderId = orderId,
            RequestMeta = StableOrderRequestMeta("cancel", orderId)
        }, cancellationToken);

    public Task<Order> CancelAsync(OrderCancelRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.OrderId, "order_id");
        return PostOrderAsync("/orders/cancel", payload, cancellationToken);
    }

    public Task<OrderPage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        PostOrderPageAsync(payload ?? new { }, cancellationToken);

    public Task<OrderPage> PageAsync(OrderPageRequest payload, CancellationToken cancellationToken = default) =>
        PostOrderPageAsync(payload, cancellationToken);

    public Task<ResourceSearchPage> SearchAsync(ResourceSearchRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ResourceSearchPage>("/orders/search", "search", payload, cancellationToken);

    private async Task<Order> PostOrderAsync(string path, object payload, CancellationToken cancellationToken)
    {
        return await _client.PostResourceAsync<Order>(path, "order", payload, cancellationToken);
    }

    private async Task<OrderPage> PostOrderPageAsync(object payload, CancellationToken cancellationToken)
    {
        return await _client.PostResourceAsync<OrderPage>(
            "/orders/page",
            "page",
            payload,
            cancellationToken
        );
    }

    private static RequestMeta StableOrderRequestMeta(string action, string orderId) =>
        new() { IdempotencyKey = $"orders_{action}_{orderId}" };

    private static void ValidateCreate(OrderCreateRequest payload)
    {
        RequestValidator.RequireAny(
            ("customer_data", payload.CustomerData),
            ("customer_id", payload.CustomerId),
            "Either 'customer_data' or 'customer_id' is required."
        );
        RequestValidator.RequireCollection(payload.LineItems, "line_items");
    }
}
