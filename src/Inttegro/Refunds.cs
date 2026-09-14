using System.Text.Json;
using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

[JsonConverter(typeof(RefundReasonJsonConverter))]
public enum RefundReason
{
    RequestedByCustomer,
    Duplicate,
    Fraudulent,
    OrderCanceled,
    ItemReturned,
    ItemDamaged,
    ItemNotReceived,
    ItemNotAsDescribed,
    Custom
}

[JsonConverter(typeof(RefundStatusJsonConverter))]
public enum RefundStatus
{
    Canceled,
    Failed,
    Pending,
    Processing,
    Succeeded
}

[JsonConverter(typeof(RefundFailureReasonJsonConverter))]
public enum RefundFailureReason
{
    InsufficientBalance,
    OriginalPaymentMethodUnavailable,
    OriginalPaymentNotRefundable,
    RefundNotSupported,
    AmountNotSupported,
    RefundDeclined,
    RefundNotPermitted,
    TemporarilyUnavailable,
    Unknown
}

public sealed class RefundFailure
{
    [JsonPropertyName("reason")]
    public RefundFailureReason Reason { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("retryable")]
    public bool Retryable { get; set; }
}

public sealed class CreateRefundLineItem
{
    [JsonPropertyName("order_line_item_id")]
    public string? OrderLineItemId { get; set; }

    [JsonPropertyName("refund_amount")]
    public AmountParams? RefundAmount { get; set; }

    [JsonPropertyName("reason")]
    public RefundReason? Reason { get; set; }

    [JsonPropertyName("reason_details")]
    public string? ReasonDetails { get; set; }
}

public sealed class CreateRefundRequest
{
    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("line_items")]
    public List<CreateRefundLineItem>? LineItems { get; set; }

    [JsonPropertyName("reason")]
    public RefundReason? Reason { get; set; }

    [JsonPropertyName("reason_details")]
    public string? ReasonDetails { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class CancelRefundRequest
{
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }
}

public sealed class LookupRefundRequest
{
    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }
}

public sealed class PageRefundsRequest
{
    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

public sealed class RefundLineItem
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("order_line_item_id")]
    public string? OrderLineItemId { get; set; }

    [JsonPropertyName("original_amount_paid")]
    public Amount? OriginalAmountPaid { get; set; }

    [JsonPropertyName("refund_amount")]
    public Amount? RefundAmount { get; set; }

    [JsonPropertyName("reason")]
    public RefundReason? Reason { get; set; }

    [JsonPropertyName("reason_details")]
    public string? ReasonDetails { get; set; }
}

public sealed class Refund
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("order_id")]
    public string? OrderId { get; set; }

    [JsonPropertyName("order_amount")]
    public Amount? OrderAmount { get; set; }

    [JsonPropertyName("status")]
    public RefundStatus Status { get; set; }

    [JsonPropertyName("total")]
    public Amount? Total { get; set; }

    [JsonPropertyName("line_items")]
    public List<RefundLineItem>? LineItems { get; set; }

    [JsonPropertyName("reason")]
    public RefundReason Reason { get; set; }

    [JsonPropertyName("reason_details")]
    public string? ReasonDetails { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("processing_at")]
    public DateTimeOffset? ProcessingAt { get; set; }

    [JsonPropertyName("succeeded_at")]
    public DateTimeOffset? SucceededAt { get; set; }

    [JsonPropertyName("failed_at")]
    public DateTimeOffset? FailedAt { get; set; }

    [JsonPropertyName("failure")]
    public RefundFailure? Failure { get; set; }

    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; set; }

    [JsonPropertyName("cancel_reason")]
    public string? CancelReason { get; set; }
}

public sealed class RefundPage
{
    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("refunds")]
    public List<Refund>? Refunds { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }
}

public sealed class RefundReasonJsonConverter : JsonConverter<RefundReason>
{
    public override RefundReason Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "requested_by_customer" => RefundReason.RequestedByCustomer,
            "duplicate" => RefundReason.Duplicate,
            "fraudulent" => RefundReason.Fraudulent,
            "order_canceled" => RefundReason.OrderCanceled,
            "item_returned" => RefundReason.ItemReturned,
            "item_damaged" => RefundReason.ItemDamaged,
            "item_not_received" => RefundReason.ItemNotReceived,
            "item_not_as_described" => RefundReason.ItemNotAsDescribed,
            "custom" => RefundReason.Custom,
            var value => throw new JsonException($"Unknown refund reason '{value}'.")
        };

    public override void Write(Utf8JsonWriter writer, RefundReason value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            RefundReason.RequestedByCustomer => "requested_by_customer",
            RefundReason.Duplicate => "duplicate",
            RefundReason.Fraudulent => "fraudulent",
            RefundReason.OrderCanceled => "order_canceled",
            RefundReason.ItemReturned => "item_returned",
            RefundReason.ItemDamaged => "item_damaged",
            RefundReason.ItemNotReceived => "item_not_received",
            RefundReason.ItemNotAsDescribed => "item_not_as_described",
            RefundReason.Custom => "custom",
            _ => throw new JsonException($"Unknown refund reason '{value}'.")
        });
}

public sealed class RefundFailureReasonJsonConverter : JsonConverter<RefundFailureReason>
{
    public override RefundFailureReason Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "insufficient_balance" => RefundFailureReason.InsufficientBalance,
            "original_payment_method_unavailable" => RefundFailureReason.OriginalPaymentMethodUnavailable,
            "original_payment_not_refundable" => RefundFailureReason.OriginalPaymentNotRefundable,
            "refund_not_supported" => RefundFailureReason.RefundNotSupported,
            "amount_not_supported" => RefundFailureReason.AmountNotSupported,
            "refund_declined" => RefundFailureReason.RefundDeclined,
            "refund_not_permitted" => RefundFailureReason.RefundNotPermitted,
            "temporarily_unavailable" => RefundFailureReason.TemporarilyUnavailable,
            "unknown" => RefundFailureReason.Unknown,
            var value => throw new JsonException($"Unknown refund failure reason '{value}'.")
        };

    public override void Write(Utf8JsonWriter writer, RefundFailureReason value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            RefundFailureReason.InsufficientBalance => "insufficient_balance",
            RefundFailureReason.OriginalPaymentMethodUnavailable => "original_payment_method_unavailable",
            RefundFailureReason.OriginalPaymentNotRefundable => "original_payment_not_refundable",
            RefundFailureReason.RefundNotSupported => "refund_not_supported",
            RefundFailureReason.AmountNotSupported => "amount_not_supported",
            RefundFailureReason.RefundDeclined => "refund_declined",
            RefundFailureReason.RefundNotPermitted => "refund_not_permitted",
            RefundFailureReason.TemporarilyUnavailable => "temporarily_unavailable",
            RefundFailureReason.Unknown => "unknown",
            _ => throw new JsonException($"Unknown refund failure reason '{value}'.")
        });
}

public sealed class RefundStatusJsonConverter : JsonConverter<RefundStatus>
{
    public override RefundStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString() switch
        {
            "canceled" => RefundStatus.Canceled,
            "failed" => RefundStatus.Failed,
            "pending" => RefundStatus.Pending,
            "processing" => RefundStatus.Processing,
            "succeeded" => RefundStatus.Succeeded,
            var value => throw new JsonException($"Unknown refund status '{value}'.")
        };

    public override void Write(Utf8JsonWriter writer, RefundStatus value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value switch
        {
            RefundStatus.Canceled => "canceled",
            RefundStatus.Failed => "failed",
            RefundStatus.Pending => "pending",
            RefundStatus.Processing => "processing",
            RefundStatus.Succeeded => "succeeded",
            _ => throw new JsonException($"Unknown refund status '{value}'.")
        });
}
