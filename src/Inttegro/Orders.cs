using System.Text.Json;
using System.Text.Json.Serialization;
using Inttegro.BankAccounts;
using Inttegro.Money;
using Inttegro.Wallets;

namespace Inttegro;

public sealed class OrderPayoutSettings
{
    [JsonPropertyName("destination")]
    public OrderPayoutDestination? Destination { get; set; }

    [JsonPropertyName("enable_fx")]
    public bool? EnableFx { get; set; }
}

public sealed class OrderPayoutDestination
{
    [JsonPropertyName("financial_account_id")]
    public string? FinancialAccountId { get; set; }

    [JsonPropertyName("financial_account_data")]
    public OrderPayoutFinancialAccount? FinancialAccountData { get; set; }
}

public sealed class OrderPayoutFinancialAccount
{
    [JsonPropertyName("type")]
    public FinancialAccountType Type { get; set; }

    [JsonPropertyName("wallet")]
    public WalletConfig? Wallet { get; set; }

    [JsonPropertyName("bank_account")]
    public BankAccountConfig? BankAccount { get; set; }

    [JsonPropertyName("dosh_account")]
    public DoshAccount? DoshAccount { get; set; }
}

public sealed class OrderCreateRequest
{
    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("customer_data")]
    public CustomerData? CustomerData { get; set; }

    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }

    [JsonPropertyName("payment_method_data")]
    public PaymentMethodData? PaymentMethodData { get; set; }

    [JsonPropertyName("statement_descriptor")]
    public string? StatementDescriptor { get; set; }

    [JsonPropertyName("statement_descriptor_prefix")]
    public string? StatementDescriptorPrefix { get; set; }

    [JsonPropertyName("execute_payment")]
    public bool? ExecutePayment { get; set; }

    [JsonPropertyName("finalize")]
    public bool? Finalize { get; set; }

    [JsonPropertyName("checkout_settings")]
    public CheckoutSettings? CheckoutSettings { get; set; }

    [JsonPropertyName("payout_settings")]
    public OrderPayoutSettings? PayoutSettings { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("line_items")]
    public List<LineItemParams>? LineItems { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("billing_details")]
    public BillingDetails? BillingDetails { get; set; }

    [JsonPropertyName("shipping")]
    public Shipping? Shipping { get; set; }
}

public sealed class OrderLookupRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }
}

public sealed class OrderUpdateRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("clear_payment_method")]
    public bool? ClearPaymentMethod { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("invoice_settings")]
    public InvoiceSettings? InvoiceSettings { get; set; }

    [JsonPropertyName("finalize")]
    public bool? Finalize { get; set; }

    [JsonPropertyName("line_items")]
    public List<LineItemParams>? LineItems { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("receipt_number")]
    public string? ReceiptNumber { get; set; }

    [JsonPropertyName("payment_method_data")]
    public PaymentMethodData? PaymentMethodData { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }

    [JsonPropertyName("statement_descriptor")]
    public string? StatementDescriptor { get; set; }

    [JsonPropertyName("statement_descriptor_prefix")]
    public string? StatementDescriptorPrefix { get; set; }
}

public sealed class InvoiceSettings
{
    [JsonPropertyName("number")] public string? Number { get; set; }
    [JsonPropertyName("memo")] public string? Memo { get; set; }
    [JsonPropertyName("footer")] public string? Footer { get; set; }
    [JsonPropertyName("custom_data")] public CustomData? CustomData { get; set; }
}

public sealed class OrderPayRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }

    [JsonPropertyName("payment_method_data")]
    public PaymentMethodData? PaymentMethodData { get; set; }

    [JsonPropertyName("paid_out_of_band")]
    public bool? PaidOutOfBand { get; set; }
}

public sealed class OrderConfirmPaymentRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }
}

public sealed class OrderRequestConfirmationRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class OrderFinalizeRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class OrderSendInvoiceParams
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class OrderSendReceiptParams
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class OrderCompleteRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("paid_out_of_band")]
    public bool? PaidOutOfBand { get; set; }
}

public sealed class OrderCancelRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class OrderPageRequest
{
    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

public sealed class OrderCustomer
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("guest")]
    public bool Guest { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("billing_address")]
    public OrderAddress? BillingAddress { get; set; }

    [JsonPropertyName("shipping_address")]
    public OrderAddress? ShippingAddress { get; set; }
}

public sealed class OrderAddress
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }
    [JsonPropertyName("line1")] public string? Line1 { get; set; }
    [JsonPropertyName("line2")] public string? Line2 { get; set; }
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("region")] public string? Region { get; set; }
    [JsonPropertyName("post_code")] public string? PostCode { get; set; }
    [JsonPropertyName("country")] public string Country { get; set; } = string.Empty;
}

public sealed class OrderCreatedFrom
{
    [JsonPropertyName("source")] public string? Source { get; set; }
    [JsonPropertyName("resource_type")] public OrderCreatedFromResourceType? ResourceType { get; set; }
    [JsonPropertyName("resource_id")] public string? ResourceId { get; set; }
}

public sealed class OrderLineItemGroup
{
    [JsonPropertyName("line_items")]
    public List<LineItem> LineItems { get; set; } = [];

    [JsonPropertyName("total")]
    public Amount Total { get; set; } = new();
}

public sealed class OrderInvoiceFormat
{
    [JsonPropertyName("web")]
    public InvoiceUrl Web { get; set; } = new();

    [JsonPropertyName("pdf")]
    public InvoiceUrl Pdf { get; set; } = new();
}

public sealed class InvoiceUrl
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public sealed class OrderReceiptFormat
{
    [JsonPropertyName("web")]
    public InvoiceUrl Web { get; set; } = new();

    [JsonPropertyName("pdf")]
    public InvoiceUrl Pdf { get; set; } = new();
}

public sealed class OrderReceipt
{
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("format")]
    public OrderReceiptFormat Format { get; set; } = new();

    [JsonPropertyName("delivery_summary")]
    public JsonElement? DeliverySummary { get; set; }
}

public sealed class OrderInvoice
{
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("format")]
    public OrderInvoiceFormat Format { get; set; } = new();
}

public sealed class Order
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("number")]
    public string? Number { get; set; }

    [JsonPropertyName("receipt_number")]
    public string? ReceiptNumber { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("status")]
    public OrderStatus Status { get; set; }

    [JsonPropertyName("customer")]
    public OrderCustomer Customer { get; set; } = new();

    [JsonPropertyName("checkout_settings")]
    public CheckoutSettings? CheckoutSettings { get; set; }

    [JsonPropertyName("invoice_settings")]
    public InvoiceSettings? InvoiceSettings { get; set; }

    [JsonPropertyName("line_item_group")]
    public OrderLineItemGroup? LineItemGroup { get; set; }

    [JsonPropertyName("payment")]
    public Payment? Payment { get; set; }

    [JsonPropertyName("invoice")]
    public OrderInvoice? Invoice { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("created_from")]
    public OrderCreatedFrom? CreatedFrom { get; set; }

    [JsonPropertyName("initiated_at")]
    public DateTimeOffset InitiatedAt { get; set; }

    [JsonPropertyName("sealed_at")]
    public DateTimeOffset? SealedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }

    [JsonPropertyName("paid_at")]
    public DateTimeOffset? PaidAt { get; set; }

    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("payment_due_at")]
    public DateTimeOffset? PaymentDueAt { get; set; }

    [JsonPropertyName("refunds")]
    public List<Refund>? Refunds { get; set; }
}

public sealed class OrderPage
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("orders")]
    public List<Order> Orders { get; set; } = [];
}

public sealed class OrderDocumentDeliveryResult
{
    [JsonPropertyName("order")]
    public Order? Order { get; set; }

    [JsonPropertyName("delivery")]
    public OrderDocumentDelivery? Delivery { get; set; }
}

public sealed class OrderDocumentDelivery
{
    [JsonPropertyName("document_kind")]
    public string? DocumentKind { get; set; }

    [JsonPropertyName("document_url")]
    public string? DocumentUrl { get; set; }

    [JsonPropertyName("sent_channels")]
    public List<string>? SentChannels { get; set; }

    [JsonPropertyName("failed_channels")]
    public List<string>? FailedChannels { get; set; }

    [JsonPropertyName("deliveries")]
    public List<OrderDocumentDeliveryAttempt>? Deliveries { get; set; }

    [JsonPropertyName("failures")]
    public List<OrderDocumentDeliveryFailure>? Failures { get; set; }
}

public sealed class OrderDocumentDeliveryAttempt
{
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    [JsonPropertyName("chime_id")]
    public string? ChimeId { get; set; }

}

public sealed class OrderDocumentDeliveryFailure
{
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}
