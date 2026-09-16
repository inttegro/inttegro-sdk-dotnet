using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

[JsonConverter(typeof(WireEnumJsonConverter<ProductType>))]
public enum ProductType
{
    [EnumMember(Value = "physical")]
    Physical,
    [EnumMember(Value = "digital")]
    Digital,
    [EnumMember(Value = "service")]
    Service,
    [EnumMember(Value = "voucher")]
    Voucher,
    [EnumMember(Value = "custom")]
    Custom,
    [EnumMember(Value = "cause")]
    Cause
}

[JsonConverter(typeof(WireEnumJsonConverter<LineItemType>))]
public enum LineItemType
{
    [EnumMember(Value = "product")]
    Product,
    [EnumMember(Value = "fee")]
    Fee,
    [EnumMember(Value = "shipping")]
    Shipping,
    [EnumMember(Value = "discount")]
    Discount
}

public sealed class RequestMeta
{
    [JsonPropertyName("idempotency_key")]
    public string? IdempotencyKey { get; set; }
}

public sealed class ApiError
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("fix_code")]
    public string? FixCode { get; set; }

    [JsonPropertyName("cause")]
    public string? Cause { get; set; }
}

public sealed class Address
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("line1")]
    public string? Line1 { get; set; }

    [JsonPropertyName("line2")]
    public string? Line2 { get; set; }

    [JsonPropertyName("town")]
    public string? Town { get; set; }

    [JsonPropertyName("region")]
    public string? Region { get; set; }

    [JsonPropertyName("district")]
    public string? District { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("post_code")]
    public string? PostCode { get; set; }
}

public sealed class BillingDetails
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}

public sealed class Shipping
{
    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}

public sealed class CustomerData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }
}

public sealed class CheckoutSettings
{
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; set; }
}

public sealed class LineItemParams
{
    [JsonPropertyName("type")]
    public LineItemType Type { get; set; }

    [JsonPropertyName("product")]
    public ProductDetailsParams? Product { get; set; }

    [JsonPropertyName("fee")]
    public FeeDetailsParams? Fee { get; set; }

    [JsonPropertyName("shipping")]
    public ShippingDetailsParams? Shipping { get; set; }
}

public sealed class LineItem
{
    [JsonPropertyName("type")]
    public LineItemType Type { get; set; }

    [JsonPropertyName("product")]
    public ProductDetails? Product { get; set; }

    [JsonPropertyName("fee")]
    public FeeDetails? Fee { get; set; }

    [JsonPropertyName("shipping")]
    public ShippingDetails? Shipping { get; set; }

    [JsonPropertyName("discount")]
    public DiscountDetails? Discount { get; set; }
}

public sealed class DiscountDetails { }

public sealed class ProductDetailsParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    [JsonPropertyName("type")]
    public ProductType Type { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("about")]
    public string? About { get; set; }

    [JsonPropertyName("quantity")]
    public long? Quantity { get; set; }

    [JsonPropertyName("price")]
    public PriceParams? Price { get; set; }

    [JsonPropertyName("price_id")]
    public string? PriceId { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }
}

public sealed class ProductDetails
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    [JsonPropertyName("price_id")]
    public string? PriceId { get; set; }

    [JsonPropertyName("type")]
    public ProductType Type { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("about")]
    public string? About { get; set; }

    [JsonPropertyName("quantity")]
    public long Quantity { get; set; }

    [JsonPropertyName("price")]
    public Price Price { get; set; } = new();

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }
}

public sealed class FeeDetailsParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }

    [JsonPropertyName("amount")]
    public AmountParams? Amount { get; set; }
}

public sealed class FeeDetails
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("amount")]
    public Amount Amount { get; set; } = new();
}

public sealed class ShippingDetailsParams
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }

    [JsonPropertyName("fee")]
    public AmountParams? Fee { get; set; }
}

public sealed class ShippingDetails
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("tax_code")]
    public string? TaxCode { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("fee")]
    public Amount Fee { get; set; } = new();
}
