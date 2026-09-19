using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inttegro;

internal sealed class RefundOrderLineItemJsonConverter : JsonConverter<RefundOrderLineItem>
{
    public override RefundOrderLineItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var lineItem = document.RootElement;
        var id = lineItem.GetProperty("id").GetString()
            ?? throw new JsonException("Refund order line item is missing its ID.");
        return lineItem.GetProperty("type").GetString() switch
        {
            "product" => new RefundProductOrderLineItem
            {
                Id = id,
                Quantity = lineItem.GetProperty("quantity").GetInt32(),
                Product = lineItem.GetProperty("product").Deserialize<RefundOrderLineItemProduct>(options)
                    ?? throw new JsonException("Refund order line item is missing product details.")
            },
            "fee" => new RefundFeeOrderLineItem
            {
                Id = id,
                Fee = lineItem.GetProperty("fee").Deserialize<RefundOrderLineItemAdjustment>(options)
                    ?? throw new JsonException("Refund order line item is missing fee details.")
            },
            "shipping" => new RefundShippingOrderLineItem
            {
                Id = id,
                Shipping = lineItem.GetProperty("shipping").Deserialize<RefundOrderLineItemAdjustment>(options)
                    ?? throw new JsonException("Refund order line item is missing shipping details.")
            },
            _ => throw new JsonException("Unknown refund order line item type.")
        };
    }

    public override void Write(Utf8JsonWriter writer, RefundOrderLineItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("id", value.Id);
        switch (value)
        {
            case RefundProductOrderLineItem product:
                writer.WriteString("type", "product");
                writer.WriteNumber("quantity", product.Quantity);
                writer.WritePropertyName("product");
                JsonSerializer.Serialize(writer, product.Product, options);
                break;
            case RefundFeeOrderLineItem fee:
                writer.WriteString("type", "fee");
                writer.WritePropertyName("fee");
                JsonSerializer.Serialize(writer, fee.Fee, options);
                break;
            case RefundShippingOrderLineItem shipping:
                writer.WriteString("type", "shipping");
                writer.WritePropertyName("shipping");
                JsonSerializer.Serialize(writer, shipping.Shipping, options);
                break;
            default:
                throw new JsonException("Unknown refund order line item type.");
        }
        writer.WriteEndObject();
    }
}
