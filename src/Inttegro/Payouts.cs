using System.Text.Json;
using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

public sealed class PayoutSetDestinationsRequest
{
    [JsonPropertyName("destinations")]
    public PayoutDestinations? Destinations { get; set; }
}

/// <summary>Complete payout settings returned by the settings endpoint.</summary>
public sealed class PayoutSettingsLookup
{
    [JsonPropertyName("fx_enabled")]
    public bool? FxEnabled { get; set; }

    [JsonPropertyName("destinations")]
    public PayoutDestinations? Destinations { get; set; }

    [JsonPropertyName("schedule")]
    public PayoutSettingsLookupSchedule? Schedule { get; set; }
}

/// <summary>Payout settings fields returned after a settings mutation.</summary>
public sealed class PayoutSettingsMutation
{
    [JsonPropertyName("destinations")]
    public PayoutDestinations? Destinations { get; set; }

    [JsonPropertyName("fx_enabled")]
    public bool? FxEnabled { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("schedule")]
    public PayoutSettingsMutationSchedule? Schedule { get; set; }
}

/// <summary>Active payout schedule returned by the settings endpoint.</summary>
public sealed class PayoutSettingsLookupSchedule
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("interval")]
    public string? Interval { get; set; }

    [JsonPropertyName("schedule_on")]
    public string? ScheduleOn { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("aging_spec")]
    public PayoutSettingsLookupScheduleAgingSpec? AgingSpec { get; set; }
}

/// <summary>Rules that determine when balance transactions become eligible for payout.</summary>
public sealed class PayoutSettingsLookupScheduleAgingSpec
{
    [JsonPropertyName("t_plus")]
    public string? TPlus { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("abide")]
    public string? Abide { get; set; }
}

/// <summary>Updated payout schedule returned after a settings mutation.</summary>
public sealed class PayoutSettingsMutationSchedule
{
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("interval")]
    public string? Interval { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("schedule_on")]
    public string? ScheduleOn { get; set; }

    [JsonPropertyName("spec")]
    public PayoutSettingsMutationScheduleSpec? Spec { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

/// <summary>Aging rule returned after a payout settings mutation.</summary>
public sealed class PayoutSettingsMutationScheduleSpec
{
    [JsonPropertyName("abide")]
    public string? Abide { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("t_plus")]
    public string? TPlus { get; set; }
}

public sealed class PayoutPageRequest
{
    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

public sealed class SchedulePayoutRequest
{
    [JsonPropertyName("destination_id")]
    public string? DestinationId { get; set; }

    [JsonPropertyName("execute_after")]
    public DateTimeOffset? ExecuteAfter { get; set; }

    [JsonPropertyName("max_amount")]
    public long? MaxAmount { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }
}

public sealed class CancelPayoutRequest
{
    [JsonPropertyName("payout_id")]
    public string? PayoutId { get; set; }
}

public sealed class PayoutPage
{
    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("payouts")]
    public List<Payout>? Payouts { get; set; }
}

public sealed class Payout
{
    [JsonPropertyName("amount")]
    public Amount? Amount { get; set; }

    [JsonPropertyName("balance_transactions")]
    public List<PayoutBalanceTransaction>? BalanceTransactions { get; set; }

    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("destination_id")]
    public string? DestinationId { get; set; }

    [JsonPropertyName("error")]
    public PayoutError? Error { get; set; }

    [JsonPropertyName("execute_after")]
    public DateTimeOffset? ExecuteAfter { get; set; }

    [JsonPropertyName("executed_by")]
    public string? ExecutedBy { get; set; }

    [JsonPropertyName("expected_at")]
    public DateTimeOffset? ExpectedAt { get; set; }

    [JsonPropertyName("failed_at")]
    public DateTimeOffset? FailedAt { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("initiated_at")]
    public DateTimeOffset? InitiatedAt { get; set; }

    [JsonPropertyName("initiated_by")]
    public string? InitiatedBy { get; set; }

    [JsonPropertyName("max_amount")]
    public Amount? MaxAmount { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("schedule_id")]
    public string? ScheduleId { get; set; }

    [JsonPropertyName("scheduled_at")]
    public DateTimeOffset? ScheduledAt { get; set; }

    [JsonPropertyName("scheduled_by")]
    public string? ScheduledBy { get; set; }

    [JsonPropertyName("sent_at")]
    public DateTimeOffset? SentAt { get; set; }

    [JsonPropertyName("source_id")]
    public string? SourceId { get; set; }

    [JsonPropertyName("status")]
    public PayoutStatus Status { get; set; }

    [JsonPropertyName("succeeded_at")]
    public DateTimeOffset? SucceededAt { get; set; }
}

/// <summary>A sparse view of one balance transaction's contribution to a payout.</summary>
public sealed class PayoutBalanceTransaction
{
    [JsonPropertyName("allocated_amount")]
    public Amount? AllocatedAmount { get; set; }

    [JsonPropertyName("amount")]
    public Amount? Amount { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public sealed class PayoutError
{
    [JsonPropertyName("cause")] public string? Cause { get; set; }
    [JsonPropertyName("message")] public string? Message { get; set; }
    [JsonPropertyName("occurred_at")] public DateTimeOffset? OccurredAt { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
}
