using Inttegro;
using Inttegro.BankAccounts;
using Inttegro.Wallets;
using System.Text.Json;
using Xunit;

namespace Inttegro.Tests;

public sealed class DomainEnumsTests
{
    [Fact]
    public void SerializesExactWireValues()
    {
        Assert.Equal("\"digital\"", JsonSerializer.Serialize(ProductType.Digital));
        Assert.Equal("\"requested_by_customer\"", JsonSerializer.Serialize(RefundReason.RequestedByCustomer));
        Assert.Equal("\"refund_declined\"", JsonSerializer.Serialize(RefundFailureReason.RefundDeclined));
        Assert.Equal(RefundFailureReason.Unknown, JsonSerializer.Deserialize<RefundFailureReason>("\"unknown\""));
        Assert.Equal("\"pending\"", JsonSerializer.Serialize(UploadRequestStatus.Pending));
        Assert.Equal("\"mobile_money\"", JsonSerializer.Serialize(WalletType.MobileMoney));
        Assert.Equal("\"ghana_bank_account\"", JsonSerializer.Serialize(BankAccountType.GhanaBankAccount));
    }

    [Fact]
    public void FinancialAccountVariantsUseFocusedNamespaces()
    {
        var account = new FinancialAccount
        {
            Type = FinancialAccountType.Wallet,
            Wallet = new WalletConfig
            {
                Type = WalletType.MobileMoney,
                MobileMoney = new WalletMobileMoney { AccountNumber = "233200000000", Network = "mtn" }
            },
            BankAccount = new BankAccountConfig
            {
                Type = BankAccountType.GhanaBankAccount,
                GhanaBankAccount = new GhanaBankAccount { Number = "0123456789" }
            }
        };

        Assert.Equal("mtn", account.Wallet!.MobileMoney!.Network);
        Assert.Equal("0123456789", account.BankAccount!.GhanaBankAccount!.Number);
    }

    [Fact]
    public void RefundSettlementDiscriminatorsCanFollowOtherFields()
    {
        const string json = """
            {"payment_method":{"id":"pm_123","mobile_money":{"network":"mtn","account_number":"****7831","last4":"7831"},"type":"mobile_money"},"type":"payment_method"}
            """;
        var settlement = JsonSerializer.Deserialize<RefundSettlement>(json);
        var method = Assert.IsType<RefundSettlementMobileMoneyPaymentMethod>(
            Assert.IsType<RefundPaymentMethodSettlement>(settlement).PaymentMethod);
        Assert.Equal("pm_123", method.Id);
        Assert.Equal("****7831", method.MobileMoney.AccountNumber);

        using var serialized = JsonDocument.Parse(JsonSerializer.Serialize<RefundSettlement>(settlement!));
        Assert.Equal("payment_method", serialized.RootElement.GetProperty("type").GetString());
        Assert.Equal("mobile_money", serialized.RootElement.GetProperty("payment_method").GetProperty("type").GetString());
    }
}
