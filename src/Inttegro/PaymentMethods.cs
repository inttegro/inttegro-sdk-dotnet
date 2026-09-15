using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inttegro;

[JsonConverter(typeof(WireEnumJsonConverter<PaymentMethodType>))]
public enum PaymentMethodType
{
    [EnumMember(Value = "mobile_money")]
    MobileMoney,
    [EnumMember(Value = "bank_account")]
    BankAccount,
    [EnumMember(Value = "card")]
    Card,
    [EnumMember(Value = "motito")]
    Motito
}

[JsonConverter(typeof(WireEnumJsonConverter<MobileMoneyNetwork>))]
public enum MobileMoneyNetwork
{
    [EnumMember(Value = "mtn")]
    MTN,
    [EnumMember(Value = "vodafone")]
    Vodafone,
    [EnumMember(Value = "airteltigo")]
    AirtelTigo,
    [EnumMember(Value = "airtel")]
    Airtel,
    [EnumMember(Value = "telecel")]
    Telecel
}

public sealed class MobileMoney
{
    [JsonPropertyName("network")]
    public MobileMoneyNetwork Network { get; set; }

    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; set; }
}

public sealed class PaymentMethodMobileMoney
{
    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [JsonPropertyName("last4")]
    public string Last4 { get; set; } = string.Empty;

    [JsonPropertyName("network")]
    public MobileMoneyNetwork Network { get; set; }
}

public sealed class PaymentMethodBankAccountGhana
{
    [JsonPropertyName("branch")]
    public string? Branch { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("account_number")]
    public string AccountNumber { get; set; } = string.Empty;

    [JsonPropertyName("sort_code")]
    public string? SortCode { get; set; }

    [JsonPropertyName("swift_code")]
    public string? SwiftCode { get; set; }
}

public sealed class PaymentMethodBankAccount
{
    [JsonPropertyName("ghana_bank_account")]
    public PaymentMethodBankAccountGhana? GhanaBankAccount { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public sealed class PaymentMethodCard { }

public sealed class PaymentMethodVerification
{
    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }

    [JsonPropertyName("initiated_at")]
    public DateTimeOffset InitiatedAt { get; set; }

    [JsonPropertyName("mechanism")]
    public string? Mechanism { get; set; }

    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public sealed class PaymentMethodOwner
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("address")] public PaymentMethodOwnerAddressResponse? Address { get; set; }
}

public sealed class PaymentMethodOwnerAddressResponse
{
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("country")] public string Country { get; set; } = string.Empty;
    [JsonPropertyName("line_1")] public string? Line1 { get; set; }
    [JsonPropertyName("line_2")] public string? Line2 { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }
    [JsonPropertyName("post_code")] public string? PostCode { get; set; }
    [JsonPropertyName("region")] public string? Region { get; set; }
}

public sealed class PaymentMethodSupplied
{
    [JsonPropertyName("attempt_id")] public string? AttemptId { get; set; }
    [JsonPropertyName("by")] public string By { get; set; } = string.Empty;
    [JsonPropertyName("channel")] public string? Channel { get; set; }
    [JsonPropertyName("resource_id")] public string? ResourceId { get; set; }
    [JsonPropertyName("resource_type")] public string? ResourceType { get; set; }
    [JsonPropertyName("supplied_at")] public DateTimeOffset SuppliedAt { get; set; }
}

public sealed class PaymentMethodData
{
    [JsonPropertyName("type")]
    public PaymentMethodType Type { get; set; }

    [JsonPropertyName("mobile_money")]
    public MobileMoney? MobileMoney { get; set; }
}

public sealed class PaymentMethodTokenizeRequest
{
    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("payment_method_data")]
    public PaymentMethodData? PaymentMethodData { get; set; }

    [JsonPropertyName("verify_immediately")]
    public bool? VerifyImmediately { get; set; }
}

public sealed class PaymentMethodVerifyRequest
{
    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }
}

public sealed class PaymentMethodConfirmVerificationRequest
{
    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }
}

public sealed class PaymentMethodLookupRequest
{
    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }
}

public sealed class PaymentMethodPageRequest
{
    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

public sealed class PaymentMethodActionRequest
{
    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }
}

public sealed class PaymentMethodUpdateRequest
{
    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataPatch? CustomData { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonPropertyName("archived")]
    public bool? Archived { get; set; }

    [JsonPropertyName("owner")]
    public PaymentMethodOwnerInput? Owner { get; set; }
}

public sealed class PaymentMethodOwnerInput
{
    [JsonPropertyName("address")]
    public PaymentMethodOwnerAddress? Address { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public sealed class PaymentMethodOwnerAddress
{
    [JsonPropertyName("city")] public string? City { get; set; }
    [JsonPropertyName("country")] public string? Country { get; set; }
    [JsonPropertyName("line1")] public string? Line1 { get; set; }
    [JsonPropertyName("line2")] public string? Line2 { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("phone_number")] public string? PhoneNumber { get; set; }
    [JsonPropertyName("post_code")] public string? PostCode { get; set; }
    [JsonPropertyName("region")] public string? Region { get; set; }
}

public sealed class PaymentMethodDeleteRequest
{
    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string? PaymentMethodId { get; set; }
}

public sealed class PaymentMethod
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("archived_at")]
    public DateTimeOffset? ArchivedAt { get; set; }

    [JsonPropertyName("customer_id")]
    public string CustomerId { get; set; } = string.Empty;

    [JsonPropertyName("fingerprint")]
    public string Fingerprint { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public PaymentMethodType Type { get; set; }

    [JsonPropertyName("mobile_money")]
    public PaymentMethodMobileMoney? MobileMoney { get; set; }

    [JsonPropertyName("bank_account")]
    public PaymentMethodBankAccount? BankAccount { get; set; }

    [JsonPropertyName("card")]
    public PaymentMethodCard? Card { get; set; }

    [JsonPropertyName("owner")]
    public PaymentMethodOwner? Owner { get; set; }

    [JsonPropertyName("supplied")]
    public PaymentMethodSupplied? Supplied { get; set; }

    [JsonPropertyName("verification")]
    public PaymentMethodVerification? Verification { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("expires_on")]
    public DateTimeOffset? ExpiresOn { get; set; }

    [JsonPropertyName("ephemeral")]
    public bool? Ephemeral { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("verified_at")]
    public DateTimeOffset? VerifiedAt { get; set; }
}

public sealed class PaymentMethodTypeSetting
{
    [JsonPropertyName("type")]
    public PaymentMethodType Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("confirms_use")]
    public bool ConfirmsUse { get; set; }
}

public sealed class PaymentMethodSettings
{
    [JsonPropertyName("mobile_money")]
    public PaymentMethodTypeSetting? MobileMoney { get; set; }

    [JsonPropertyName("bank_account")]
    public PaymentMethodTypeSetting? BankAccount { get; set; }

    [JsonPropertyName("card")]
    public PaymentMethodTypeSetting? Card { get; set; }

    [JsonPropertyName("motito")]
    public PaymentMethodTypeSetting? Motito { get; set; }
}

public sealed class PaymentMethodPage
{
    [JsonPropertyName("number")]
    public int Number { get; set; }

    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("payment_methods")]
    public List<PaymentMethod> PaymentMethods { get; set; } = [];
}
