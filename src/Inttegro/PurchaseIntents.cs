using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

public sealed class PurchaseIntentProductSelectorParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("variant_set_id")]
    public string? VariantSetId { get; set; }
}

public sealed class PurchaseIntentOriginalPriceParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("nominal")]
    public PriceParams? Nominal { get; set; }
}

public sealed class PurchaseIntentPriceSelectorParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("nominal")]
    public PriceParams? Nominal { get; set; }

    [JsonPropertyName("original")]
    public PurchaseIntentOriginalPriceParams? Original { get; set; }

    [JsonPropertyName("original_id")]
    public string? OriginalId { get; set; }
}

public sealed class PurchaseIntentQuantity
{
    [JsonPropertyName("min")]
    public int Min { get; set; }

    [JsonPropertyName("max")]
    public int? Max { get; set; }
}

public sealed class PurchaseIntentUsage
{
    [JsonPropertyName("single_use")]
    public bool? SingleUse { get; set; }

    [JsonPropertyName("multi_use")]
    public bool? MultiUse { get; set; }

    [JsonPropertyName("order")]
    public PurchaseIntentUsageOrder? Order { get; set; }
}

public sealed class PurchaseIntentBuyPageText
{
    [JsonPropertyName("checkout_section_title")]
    public string? CheckoutSectionTitle { get; set; }

    [JsonPropertyName("amount_field_label")]
    public string? AmountFieldLabel { get; set; }

    [JsonPropertyName("primary_action_label")]
    public string? PrimaryActionLabel { get; set; }
}

public sealed class PurchaseIntentBuyPagePresentation
{
    [JsonPropertyName("text")]
    public PurchaseIntentBuyPageText? Text { get; set; }
}

public sealed class PurchaseIntentPresentation
{
    [JsonPropertyName("buy_page")]
    public PurchaseIntentBuyPagePresentation? BuyPage { get; set; }
}

public sealed class CreatePurchaseIntentParams
{
    [JsonPropertyName("product")]
    public PurchaseIntentProductSelectorParams? Product { get; set; }

    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    [JsonPropertyName("price")]
    public PurchaseIntentPriceSelectorParams? Price { get; set; }

    [JsonPropertyName("price_id")]
    public string? PriceId { get; set; }

    [JsonPropertyName("quantity")]
    public PurchaseIntentQuantity? Quantity { get; set; }

    [JsonPropertyName("usage")]
    public PurchaseIntentUsage? Usage { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("presentation")]
    public PurchaseIntentPresentation? Presentation { get; set; }
}

public sealed class UpdatePurchaseIntentParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("purchase_intent_id")]
    public string? PurchaseIntentId { get; set; }

    [JsonPropertyName("quantity")]
    public PurchaseIntentQuantity? Quantity { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; set; }

    [JsonPropertyName("reactivate")]
    public bool? Reactivate { get; set; }

    [JsonPropertyName("presentation")]
    public PurchaseIntentPresentation? Presentation { get; set; }
}

public sealed class PurchaseIntentOriginalPrice
{
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("nominal")]
    public Amount Nominal { get; set; } = null!;
}

public sealed class PurchaseIntentPrice
{
    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("nominal")]
    public Amount Nominal { get; set; } = null!;

    [JsonPropertyName("original")]
    public PurchaseIntentOriginalPrice? Original { get; set; }
}
