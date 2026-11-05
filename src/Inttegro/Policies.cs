using System.Text.Json.Serialization;

namespace Inttegro;

public sealed class PolicyAmountRange
{
    [JsonPropertyName("min")] public long Min { get; set; }
    [JsonPropertyName("max")] public long Max { get; set; }
}

public sealed class PolicyLimits
{
    [JsonPropertyName("amount")] public PolicyAmountRange Amount { get; set; } = null!;
}

public sealed class PolicyBTAgingSpec
{
    [JsonPropertyName("public_id")] public string PublicId { get; set; } = null!;
    [JsonPropertyName("t_plus")] public string TPlus { get; set; } = null!;
}

public sealed class PolicyBTAgingSpecReference
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
}

public sealed class ChildrenPolicyPaymentsCapabilities
{
    [JsonPropertyName("currencies")] public List<string>? Currencies { get; set; }
    [JsonPropertyName("limits")] public PolicyLimits? Limits { get; set; }
}

public sealed class ChildrenPolicyPayoutsCapabilities
{
    [JsonPropertyName("enabled")] public List<bool>? Enabled { get; set; }
    [JsonPropertyName("limits")] public PolicyLimits? Limits { get; set; }
    [JsonPropertyName("bt_aging_spec")] public List<PolicyBTAgingSpec>? BTAgingSpec { get; set; }
}

public sealed class ChildrenPolicyPaymentMethodCapabilities
{
    [JsonPropertyName("enabled")] public List<bool> Enabled { get; set; } = [];
    [JsonPropertyName("confirms_use")] public List<bool> ConfirmsUse { get; set; } = [];
}

public sealed class ChildrenPolicyPaymentMethodsCapabilities
{
    [JsonPropertyName("mobile_money")] public ChildrenPolicyPaymentMethodCapabilities? MobileMoney { get; set; }
    [JsonPropertyName("bank_account")] public ChildrenPolicyPaymentMethodCapabilities? BankAccount { get; set; }
    [JsonPropertyName("motito")] public ChildrenPolicyPaymentMethodCapabilities? Motito { get; set; }
    [JsonPropertyName("card")] public ChildrenPolicyPaymentMethodCapabilities? Card { get; set; }
}

public sealed class ChildrenPolicyCapabilities
{
    [JsonPropertyName("payments")] public ChildrenPolicyPaymentsCapabilities? Payments { get; set; }
    [JsonPropertyName("payouts")] public ChildrenPolicyPayoutsCapabilities? Payouts { get; set; }
    [JsonPropertyName("payment_methods")] public ChildrenPolicyPaymentMethodsCapabilities? PaymentMethods { get; set; }
}

public sealed class ChildrenPolicy
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("owner_application_id")] public string OwnerApplicationId { get; set; } = null!;
    [JsonPropertyName("version")] public int Version { get; set; }
    [JsonPropertyName("active_version")] public int ActiveVersion { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("capabilities")] public ChildrenPolicyCapabilities Capabilities { get; set; } = null!;
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset UpdatedAt { get; set; }
    [JsonPropertyName("version_created_at")] public DateTimeOffset VersionCreatedAt { get; set; }
}

public sealed class OperatingPolicyPaymentsCapabilitiesInput
{
    [JsonPropertyName("currencies")] public List<string>? Currencies { get; set; }
    [JsonPropertyName("limits")] public PolicyLimits? Limits { get; set; }
}

public sealed class OperatingPolicyPayoutsCapabilitiesInput
{
    [JsonPropertyName("enabled")] public bool? Enabled { get; set; }
    [JsonPropertyName("limits")] public PolicyLimits? Limits { get; set; }
    [JsonPropertyName("bt_aging_spec")] public PolicyBTAgingSpecReference? BTAgingSpec { get; set; }
}

public sealed class OperatingPolicyPaymentMethodCapabilities
{
    [JsonPropertyName("enabled")] public bool Enabled { get; set; }
    [JsonPropertyName("confirms_use")] public bool ConfirmsUse { get; set; }
}

public sealed class OperatingPolicyPaymentMethodsCapabilities
{
    [JsonPropertyName("mobile_money")] public OperatingPolicyPaymentMethodCapabilities? MobileMoney { get; set; }
    [JsonPropertyName("bank_account")] public OperatingPolicyPaymentMethodCapabilities? BankAccount { get; set; }
    [JsonPropertyName("motito")] public OperatingPolicyPaymentMethodCapabilities? Motito { get; set; }
    [JsonPropertyName("card")] public OperatingPolicyPaymentMethodCapabilities? Card { get; set; }
}

public sealed class OperatingPolicyCapabilitiesInput
{
    [JsonPropertyName("payments")] public OperatingPolicyPaymentsCapabilitiesInput? Payments { get; set; }
    [JsonPropertyName("payouts")] public OperatingPolicyPayoutsCapabilitiesInput? Payouts { get; set; }
    [JsonPropertyName("payment_methods")] public OperatingPolicyPaymentMethodsCapabilities? PaymentMethods { get; set; }
}

public sealed class OperatingPolicyPaymentsCapabilities
{
    [JsonPropertyName("currencies")] public List<string>? Currencies { get; set; }
    [JsonPropertyName("limits")] public PolicyLimits? Limits { get; set; }
}

public sealed class OperatingPolicyPayoutsCapabilities
{
    [JsonPropertyName("enabled")] public bool? Enabled { get; set; }
    [JsonPropertyName("limits")] public PolicyLimits? Limits { get; set; }
    [JsonPropertyName("bt_aging_spec")] public PolicyBTAgingSpec? BTAgingSpec { get; set; }
}

public sealed class OperatingPolicyCapabilities
{
    [JsonPropertyName("payments")] public OperatingPolicyPaymentsCapabilities? Payments { get; set; }
    [JsonPropertyName("payouts")] public OperatingPolicyPayoutsCapabilities? Payouts { get; set; }
    [JsonPropertyName("payment_methods")] public OperatingPolicyPaymentMethodsCapabilities? PaymentMethods { get; set; }
}

public sealed class OperatingPolicySelection
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("capabilities")] public OperatingPolicyCapabilitiesInput? Capabilities { get; set; }
}

public sealed class OperatingPolicySource
{
    [JsonPropertyName("type")] public string Type { get; set; } = null!;
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("version")] public int? Version { get; set; }
}

public sealed class ApplicationOperatingPolicy
{
    [JsonPropertyName("capabilities")] public OperatingPolicyCapabilities Capabilities { get; set; } = null!;
    [JsonPropertyName("source")] public OperatingPolicySource Source { get; set; } = null!;
}

public sealed class OperatingPolicy
{
    [JsonPropertyName("id")] public string Id { get; set; } = null!;
    [JsonPropertyName("version")] public int Version { get; set; }
    [JsonPropertyName("active_version")] public int ActiveVersion { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("capabilities")] public OperatingPolicyCapabilities Capabilities { get; set; } = null!;
    [JsonPropertyName("children_policy_id")] public string ChildrenPolicyId { get; set; } = null!;
    [JsonPropertyName("children_policy_version")] public int ChildrenPolicyVersion { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset UpdatedAt { get; set; }
    [JsonPropertyName("version_created_at")] public DateTimeOffset VersionCreatedAt { get; set; }
}

public sealed class CreateAppRequest
{
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("alias")] public string? Alias { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("legal_entity_type")] public string? LegalEntityType { get; set; }
    [JsonPropertyName("placement_parent_application_id")] public string? PlacementParentApplicationId { get; set; }
    [JsonPropertyName("operating_policy")] public OperatingPolicySelection? OperatingPolicy { get; set; }
    [JsonPropertyName("relationship_policy")] public AppRelationshipPolicy? RelationshipPolicy { get; set; }
}

public sealed class UpdateAppRequest
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("alias")] public string? Alias { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("legal_entity_type")] public string? LegalEntityType { get; set; }
    [JsonPropertyName("operating_policy")] public OperatingPolicySelection? OperatingPolicy { get; set; }
}

public sealed class CreateOperatingPolicyRequest
{
    [JsonPropertyName("name")] public string Name { get; set; } = null!;
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("capabilities")] public OperatingPolicyCapabilitiesInput Capabilities { get; set; } = null!;
}

public sealed class UpdateOperatingPolicyRequest
{
    [JsonPropertyName("operating_policy_id")] public string OperatingPolicyId { get; set; } = null!;
    [JsonPropertyName("expected_version")] public int ExpectedVersion { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("capabilities")] public OperatingPolicyCapabilitiesInput? Capabilities { get; set; }
}
