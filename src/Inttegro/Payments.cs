using System.Runtime.Serialization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

internal sealed class WireEnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    private static readonly IReadOnlyDictionary<string, T> FromWire = Enum.GetValues<T>()
        .ToDictionary(value => WireValue(value), value => value, StringComparer.Ordinal);

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (value is not null && FromWire.TryGetValue(value, out var parsed))
        {
            return parsed;
        }

        throw new JsonException($"{value ?? "null"} is not valid for {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
        writer.WriteStringValue(WireValue(value));

    private static string WireValue(T value)
    {
        var name = value.ToString();
        return typeof(T).GetField(name)?.GetCustomAttribute<EnumMemberAttribute>()?.Value ?? name;
    }
}

[JsonConverter(typeof(WireEnumJsonConverter<PaymentStatus>))]
public enum PaymentStatus
{
    [EnumMember(Value = "initiated")] Initiated,
    [EnumMember(Value = "requires_action")] RequiresAction,
    [EnumMember(Value = "overdue")] Overdue,
    [EnumMember(Value = "executed")] Executed,
    [EnumMember(Value = "paid")] Paid,
    [EnumMember(Value = "canceled")] Canceled,
    [EnumMember(Value = "expired")] Expired,
    [EnumMember(Value = "failed")] Failed,
    [EnumMember(Value = "unknown")] Unknown
}

[JsonConverter(typeof(WireEnumJsonConverter<PaymentAttemptStatus>))]
public enum PaymentAttemptStatus
{
    [EnumMember(Value = "initiated")] Initiated,
    [EnumMember(Value = "executed")] Executed,
    [EnumMember(Value = "succeeded")] Succeeded,
    [EnumMember(Value = "canceled")] Canceled,
    [EnumMember(Value = "expired")] Expired,
    [EnumMember(Value = "failed")] Failed,
    [EnumMember(Value = "unknown")] Unknown
}

[JsonConverter(typeof(WireEnumJsonConverter<PaymentNextActionType>))]
public enum PaymentNextActionType
{
    [EnumMember(Value = "confirm_payment")] ConfirmPayment,
    [EnumMember(Value = "execute")] Execute,
    [EnumMember(Value = "redirect")] Redirect,
    [EnumMember(Value = "authorize_payment")] AuthorizePayment,
    [EnumMember(Value = "request_confirmation")] RequestConfirmation
}

[JsonConverter(typeof(WireEnumJsonConverter<PaymentConfirmationChannel>))]
public enum PaymentConfirmationChannel
{
    [EnumMember(Value = "sms")] Sms,
    [EnumMember(Value = "email")] Email,
    [EnumMember(Value = "push")] Push
}

[JsonConverter(typeof(WireEnumJsonConverter<CheckoutPaymentStatus>))]
public enum CheckoutPaymentStatus
{
    [EnumMember(Value = "requires_action")] RequiresAction,
    [EnumMember(Value = "processing")] Processing,
    [EnumMember(Value = "succeeded")] Succeeded,
    [EnumMember(Value = "failed")] Failed,
    [EnumMember(Value = "cancelled")] Cancelled
}

[JsonConverter(typeof(WireEnumJsonConverter<PaymentResultStatus>))]
public enum PaymentResultStatus
{
    [EnumMember(Value = "pending")] Pending,
    [EnumMember(Value = "requires_confirmation")] RequiresConfirmation,
    [EnumMember(Value = "processing")] Processing,
    [EnumMember(Value = "succeeded")] Succeeded,
    [EnumMember(Value = "failed")] Failed
}

public sealed class PayoutConfiguration
{
    [JsonPropertyName("destination")]
    public PayoutConfigurationDestination Destination { get; set; } = new();

    [JsonPropertyName("enable_fx")]
    public bool EnableFx { get; set; }
}

public sealed class PayoutConfigurationDestination
{
    [JsonPropertyName("financial_account_id")]
    public string FinancialAccountId { get; set; } = string.Empty;
}

public sealed class PaymentAttemptError
{
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
}

public sealed class PaymentAttempt
{
    [JsonPropertyName("payment_method_type")]
    public PaymentMethodType? PaymentMethodType { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }

    [JsonPropertyName("error")]
    public PaymentAttemptError? Error { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("status")]
    public PaymentAttemptStatus Status { get; set; }

    [JsonPropertyName("initiated_at")]
    public DateTimeOffset InitiatedAt { get; set; }

    [JsonPropertyName("succeeded_at")]
    public DateTimeOffset? SucceededAt { get; set; }

}

public sealed class PaymentConfirmationRequest
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("recipient")] public string Recipient { get; set; } = string.Empty;
    [JsonPropertyName("sent_via")] public PaymentConfirmationChannel SentVia { get; set; }
    [JsonPropertyName("token_size")] public int TokenSize { get; set; }
    [JsonPropertyName("sender_id")] public string SenderId { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string? Status { get; set; }
}

public sealed class PaymentConfirmationAttempt
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("confirmed")] public bool Confirmed { get; set; }
    [JsonPropertyName("reason")] public string Reason { get; set; } = string.Empty;
    [JsonPropertyName("executed_at")] public DateTimeOffset? ExecutedAt { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
}

public sealed class PaymentConfirmAction
{
    [JsonPropertyName("expires_at")] public DateTimeOffset ExpiresAt { get; set; }
    [JsonPropertyName("scheme")] public string Scheme { get; set; } = string.Empty;
    [JsonPropertyName("request")] public PaymentConfirmationRequest? Request { get; set; }
    [JsonPropertyName("attempt")] public PaymentConfirmationAttempt? Attempt { get; set; }
    [JsonPropertyName("confirmed")] public bool Confirmed { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}

public sealed class PaymentRedirectVisit
{
    [JsonPropertyName("user_agent")] public string UserAgent { get; set; } = string.Empty;
    [JsonPropertyName("ip_address")] public string IpAddress { get; set; } = string.Empty;
    [JsonPropertyName("at")] public DateTimeOffset At { get; set; }
}

public sealed class PaymentRedirectAction
{
    [JsonPropertyName("redirect_url")] public string RedirectUrl { get; set; } = string.Empty;
    [JsonPropertyName("valid_until")] public DateTimeOffset ValidUntil { get; set; }
    [JsonPropertyName("latest_visit")] public PaymentRedirectVisit? LatestVisit { get; set; }
}

public sealed class PaymentAuthorizeAction
{
    [JsonPropertyName("beneficiary")] public string Beneficiary { get; set; } = string.Empty;
    [JsonPropertyName("scheme")] public string Scheme { get; set; } = string.Empty;
    [JsonPropertyName("expires_at")] public DateTimeOffset ExpiresAt { get; set; }
}

public sealed class PaymentRequestConfirmationAction
{
    [JsonPropertyName("last_request")] public PaymentConfirmationRequest? LastRequest { get; set; }
    [JsonPropertyName("after")] public DateTimeOffset? After { get; set; }
}

public sealed class PaymentNextAction
{
    [JsonPropertyName("type")] public PaymentNextActionType Type { get; set; }
    [JsonPropertyName("confirm_payment")] public PaymentConfirmAction? ConfirmPayment { get; set; }
    [JsonPropertyName("redirect")] public PaymentRedirectAction? Redirect { get; set; }
    [JsonPropertyName("authorize")] public PaymentAuthorizeAction? Authorize { get; set; }
    [JsonPropertyName("request_confirmation")] public PaymentRequestConfirmationAction? RequestConfirmation { get; set; }
}

public sealed class PaymentAddress
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

public sealed class PaymentCustomer
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("email_address")] public string? EmailAddress { get; set; }
    [JsonPropertyName("guest")] public bool Guest { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }
    [JsonPropertyName("billing_address")] public PaymentAddress? BillingAddress { get; set; }
    [JsonPropertyName("shipping_address")] public PaymentAddress? ShippingAddress { get; set; }
}

public sealed class PaymentMethodSnapshotOwner
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("address")] public PaymentAddress? Address { get; set; }
}

public sealed class PaymentMethodSnapshotMobileMoney
{
    [JsonPropertyName("network")] public MobileMoneyNetwork Network { get; set; }
    [JsonPropertyName("account_number")] public string AccountNumber { get; set; } = string.Empty;
    [JsonPropertyName("last4")] public string Last4 { get; set; } = string.Empty;
}

public sealed class PaymentMethodSnapshotGhanaBankAccount
{
    [JsonPropertyName("account_number")] public string AccountNumber { get; set; } = string.Empty;
    [JsonPropertyName("branch")] public string? Branch { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("sort_code")] public string? SortCode { get; set; }
    [JsonPropertyName("swift_code")] public string? SwiftCode { get; set; }
}

public sealed class PaymentMethodSnapshotBankAccount
{
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("ghana_bank_account")] public PaymentMethodSnapshotGhanaBankAccount? GhanaBankAccount { get; set; }
}

public sealed class PaymentMethodSnapshotCard { }

public sealed class PaymentMethodSnapshot
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("bank_account")] public PaymentMethodSnapshotBankAccount? BankAccount { get; set; }
    [JsonPropertyName("card")] public PaymentMethodSnapshotCard? Card { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("customer_id")] public string CustomerId { get; set; } = string.Empty;
    [JsonPropertyName("mobile_money")] public PaymentMethodSnapshotMobileMoney? MobileMoney { get; set; }
    [JsonPropertyName("owner")] public PaymentMethodSnapshotOwner? Owner { get; set; }
    [JsonPropertyName("type")] public PaymentMethodType Type { get; set; }
    [JsonPropertyName("verified")] public bool Verified { get; set; }
    [JsonPropertyName("verified_at")] public DateTimeOffset? VerifiedAt { get; set; }
}

public sealed class PaymentBillingDetails
{
    [JsonPropertyName("owner")] public PaymentMethodSnapshotOwner? Owner { get; set; }
}

public sealed class PaymentError
{
    [JsonPropertyName("message")] public string Message { get; set; } = string.Empty;
    [JsonPropertyName("docs_url")] public string DocsUrl { get; set; } = string.Empty;
    [JsonPropertyName("source")] public string Source { get; set; } = string.Empty;
    [JsonPropertyName("type")] public string Type { get; set; } = string.Empty;
    [JsonPropertyName("code")] public string Code { get; set; } = string.Empty;
}

public sealed class Payment
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("statement_descriptor")] public string StatementDescriptor { get; set; } = string.Empty;
    [JsonPropertyName("payment_method_types")] public List<string>? PaymentMethodTypes { get; set; }
    [JsonPropertyName("payment_method")] public PaymentMethodSnapshot? PaymentMethod { get; set; }
    [JsonPropertyName("billing_details")] public PaymentBillingDetails? BillingDetails { get; set; }
    [JsonPropertyName("customer")] public PaymentCustomer? Customer { get; set; }
    [JsonPropertyName("receipt")] public OrderReceipt? Receipt { get; set; }
    [JsonPropertyName("latest_attempt")] public PaymentAttempt? LatestAttempt { get; set; }
    [JsonPropertyName("amount")] public Amount Amount { get; set; } = new();
    [JsonPropertyName("next_action")] public PaymentNextAction? NextAction { get; set; }
    [JsonPropertyName("latest_error")] public PaymentError? LatestError { get; set; }
    [JsonPropertyName("balance_transaction")] public BalanceTransaction? BalanceTransaction { get; set; }
    [JsonPropertyName("payout_configuration")] public PayoutConfiguration? PayoutConfiguration { get; set; }
    [JsonPropertyName("status")] public PaymentStatus Status { get; set; }
    [JsonPropertyName("initiated_at")] public DateTimeOffset InitiatedAt { get; set; }
    [JsonPropertyName("executed_at")] public DateTimeOffset? ExecutedAt { get; set; }
    [JsonPropertyName("due_at")] public DateTimeOffset? DueAt { get; set; }
    [JsonPropertyName("canceled_at")] public DateTimeOffset? CanceledAt { get; set; }
    [JsonPropertyName("expired_at")] public DateTimeOffset? ExpiredAt { get; set; }
    [JsonPropertyName("paid_at")] public DateTimeOffset? PaidAt { get; set; }
    [JsonPropertyName("paid_offline")] public bool? PaidOffline { get; set; }
    [JsonPropertyName("failed_at")] public DateTimeOffset? FailedAt { get; set; }
}
