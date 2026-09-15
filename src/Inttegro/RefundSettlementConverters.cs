using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inttegro;

internal sealed class RefundSettlementJsonConverter : JsonConverter<RefundSettlement>
{
    public override RefundSettlement Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var settlement = document.RootElement;
        return settlement.GetProperty("type").GetString() switch
        {
            "offline" => new RefundOfflineSettlement(),
            "payment_method" => new RefundPaymentMethodSettlement
            {
                PaymentMethod = settlement.GetProperty("payment_method")
                    .Deserialize<RefundSettlementPaymentMethod>(options)
                    ?? throw new JsonException("Refund settlement is missing its payment method.")
            },
            _ => throw new JsonException("Unknown refund settlement type.")
        };
    }

    public override void Write(Utf8JsonWriter writer, RefundSettlement value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        switch (value)
        {
            case RefundOfflineSettlement:
                writer.WriteString("type", "offline");
                break;
            case RefundPaymentMethodSettlement paymentMethod:
                writer.WriteString("type", "payment_method");
                writer.WritePropertyName("payment_method");
                JsonSerializer.Serialize<RefundSettlementPaymentMethod>(writer, paymentMethod.PaymentMethod, options);
                break;
            default:
                throw new JsonException("Unknown refund settlement type.");
        }
        writer.WriteEndObject();
    }
}

internal sealed class RefundSettlementPaymentMethodJsonConverter : JsonConverter<RefundSettlementPaymentMethod>
{
    public override RefundSettlementPaymentMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var method = document.RootElement;
        var id = method.GetProperty("id").GetString()
            ?? throw new JsonException("Refund settlement payment method is missing its ID.");
        return method.GetProperty("type").GetString() switch
        {
            "mobile_money" => new RefundSettlementMobileMoneyPaymentMethod
            {
                Id = id,
                MobileMoney = method.GetProperty("mobile_money").Deserialize<RefundSettlementMobileMoney>(options)
                    ?? throw new JsonException("Refund settlement is missing mobile money details.")
            },
            "bank_account" => new RefundSettlementBankAccountPaymentMethod
            {
                Id = id,
                BankAccount = method.GetProperty("bank_account").Deserialize<RefundSettlementBankAccount>(options)
                    ?? throw new JsonException("Refund settlement is missing bank account details.")
            },
            _ => throw new JsonException("Unknown refund settlement payment method type.")
        };
    }

    public override void Write(Utf8JsonWriter writer, RefundSettlementPaymentMethod value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("id", value.Id);
        switch (value)
        {
            case RefundSettlementMobileMoneyPaymentMethod mobileMoney:
                writer.WriteString("type", "mobile_money");
                writer.WritePropertyName("mobile_money");
                JsonSerializer.Serialize(writer, mobileMoney.MobileMoney, options);
                break;
            case RefundSettlementBankAccountPaymentMethod bankAccount:
                writer.WriteString("type", "bank_account");
                writer.WritePropertyName("bank_account");
                JsonSerializer.Serialize(writer, bankAccount.BankAccount, options);
                break;
            default:
                throw new JsonException("Unknown refund settlement payment method type.");
        }
        writer.WriteEndObject();
    }
}
