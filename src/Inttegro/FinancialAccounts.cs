using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Inttegro.BankAccounts;
using Inttegro.Wallets;

namespace Inttegro;

[JsonConverter(typeof(WireEnumJsonConverter<FinancialAccountType>))]
public enum FinancialAccountType
{
    [EnumMember(Value = "wallet")]
    Wallet,
    [EnumMember(Value = "bank_account")]
    BankAccount,
    [EnumMember(Value = "dosh_account")]
    DoshAccount
}

public sealed class PullPushConfig
{
    [JsonPropertyName("enabled")]
    public bool? Enabled { get; set; }

    [JsonPropertyName("enabled_at")]
    public DateTimeOffset? EnabledAt { get; set; }

    [JsonPropertyName("mandate")]
    public FinancialAccountMandate? Mandate { get; set; }
}

public sealed class FinancialAccountMandate
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("user_agent")] public string? UserAgent { get; set; }
    [JsonPropertyName("ip_address")] public string? IpAddress { get; set; }
}

public sealed class FinancialAccountVerificationRequest
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("mechanism")] public string? Mechanism { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
}

public sealed class FinancialAccountVerification
{
    [JsonPropertyName("initiated_at")] public DateTimeOffset? InitiatedAt { get; set; }
    [JsonPropertyName("completed_at")] public DateTimeOffset? CompletedAt { get; set; }
    [JsonPropertyName("request")] public FinancialAccountVerificationRequest? Request { get; set; }
}

public sealed class DoshAccount { }

public sealed class FinancialAccountCreateRequest
{
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("type")]
    public FinancialAccountType Type { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("pull_configuration")]
    public PullPushConfig? PullConfiguration { get; set; }

    [JsonPropertyName("push_configuration")]
    public PullPushConfig? PushConfiguration { get; set; }

    [JsonPropertyName("wallet")]
    public WalletConfig? Wallet { get; set; }

    [JsonPropertyName("bank_account")]
    public BankAccountConfig? BankAccount { get; set; }

    [JsonPropertyName("dosh_account")]
    public DoshAccount? DoshAccount { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }

    [JsonPropertyName("owner")]
    public BankAccountOwner? Owner { get; set; }
}

public sealed class FinancialAccount
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("verification")]
    public FinancialAccountVerification? Verification { get; set; }

    [JsonPropertyName("push_configuration")]
    public PullPushConfig? PushConfiguration { get; set; }

    [JsonPropertyName("pull_configuration")]
    public PullPushConfig? PullConfiguration { get; set; }

    [JsonPropertyName("archived_at")]
    public DateTimeOffset? ArchivedAt { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("disconnected_at")]
    public DateTimeOffset? DisconnectedAt { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("type")]
    public FinancialAccountType Type { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("fingerprint")]
    public string? Fingerprint { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("wallet")]
    public WalletConfig? Wallet { get; set; }

    [JsonPropertyName("bank_account")]
    public BankAccountConfig? BankAccount { get; set; }

    [JsonPropertyName("dosh_account")]
    public DoshAccount? DoshAccount { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("owner")]
    public BankAccountOwner? Owner { get; set; }
}

public sealed class FinancialAccountUpdateRequest
{
    [JsonPropertyName("account_id")]
    public string? AccountId { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataPatch? CustomData { get; set; }

    [JsonPropertyName("owner")]
    public BankAccountOwner? Owner { get; set; }
}

public sealed class FinancialAccountToggleRequest
{
    [JsonPropertyName("account_id")]
    public string? AccountId { get; set; }

    [JsonPropertyName("unset_as_payout_destination")]
    public bool? UnsetAsPayoutDestination { get; set; }
}

public sealed class FinancialAccountLookupRequest
{
    [JsonPropertyName("account_id")]
    public string? AccountId { get; set; }
}

public sealed class FinancialAccountArchiveRequest
{
    [JsonPropertyName("account_id")]
    public string? AccountId { get; set; }
}

public sealed class FinancialAccountPageRequest
{
    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

public sealed class FinancialAccountPage
{
    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("accounts")]
    public List<FinancialAccount>? Accounts { get; set; }
}

public sealed class FinancialAccountVerifyRequest
{
    [JsonPropertyName("account_id")]
    public string? AccountId { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }
}
