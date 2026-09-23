using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

[JsonConverter(typeof(WireEnumJsonConverter<BalanceTransactionType>))]
public enum BalanceTransactionType
{
    [EnumMember(Value = "payment")]
    Payment,
    [EnumMember(Value = "refund")]
    Refund
}

[JsonConverter(typeof(WireEnumJsonConverter<BalanceTransactionAllocationType>))]
public enum BalanceTransactionAllocationType
{
    [EnumMember(Value = "payout")]
    Payout,
    [EnumMember(Value = "refund")]
    Refund
}

[JsonConverter(typeof(WireEnumJsonConverter<BalanceTransactionAllocationStatus>))]
public enum BalanceTransactionAllocationStatus
{
    [EnumMember(Value = "pending")]
    Pending,
    [EnumMember(Value = "completed")]
    Completed
}

public class BalanceTransactionAllocationUse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public Amount Amount { get; set; } = new();
}

/// <summary>
/// Caller-safe allocation of part of a payment balance transaction. Exactly
/// one of Refund and Payout is present, matching Type.
/// </summary>
public class BalanceTransactionAllocation
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public BalanceTransactionAllocationType Type { get; set; }

    [JsonPropertyName("status")]
    public BalanceTransactionAllocationStatus Status { get; set; }

    [JsonPropertyName("refund")]
    public BalanceTransactionAllocationUse? Refund { get; set; }

    [JsonPropertyName("payout")]
    public BalanceTransactionAllocationUse? Payout { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }
}

/// <summary>
/// A merchant balance entry caused by a payment or refund. Type identifies the
/// semantic source, not accounting direction, and exactly one matching source ID
/// is present.
/// </summary>
public class BalanceTransaction
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public BalanceTransactionType Type { get; set; }

    [JsonPropertyName("payment_id")]
    public string? PaymentId { get; set; }

    [JsonPropertyName("refund_id")]
    public string? RefundId { get; set; }

    [JsonPropertyName("payout_id")]
    [Obsolete("Inspect Allocations because one payment transaction can fund many payouts.")]
    public string? PayoutId { get; set; }

    [JsonPropertyName("order_id")]
    public string OrderId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public Amount Amount { get; set; } = new();

    [JsonPropertyName("allocations")]
    public IReadOnlyList<BalanceTransactionAllocation>? Allocations { get; set; }

    [JsonPropertyName("available_amount")]
    public Amount? AvailableAmount { get; set; }

    [JsonPropertyName("pending_amount")]
    public Amount? PendingAmount { get; set; }

    [JsonPropertyName("spent_amount")]
    public Amount? SpentAmount { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("available_at")]
    public DateTimeOffset? AvailableAt { get; set; }

    [JsonPropertyName("claimed_at")]
    [Obsolete("Inspect Allocations for current payout participation.")]
    public DateTimeOffset? ClaimedAt { get; set; }

    [JsonPropertyName("paid_at")]
    [Obsolete("Inspect completed Allocations for consumed amounts.")]
    public DateTimeOffset? PaidAt { get; set; }

    [JsonPropertyName("payout_configuration")]
    public PayoutConfiguration? PayoutConfiguration { get; set; }

    [JsonIgnore]
    public string? SourceId => Type switch
    {
        BalanceTransactionType.Payment when !string.IsNullOrWhiteSpace(PaymentId) && RefundId is null => PaymentId,
        BalanceTransactionType.Refund when !string.IsNullOrWhiteSpace(RefundId) && PaymentId is null => RefundId,
        _ => null
    };
}
