using Xunit;
using System.Text.Json;

namespace Inttegro.Tests;

public sealed class ResourceSemanticsTests
{
    [Fact]
    public void CustomerFingerprintIsRequiredAndRoundTrips()
    {
        var customer = JsonSerializer.Deserialize<Customer>("{\"id\":\"cu_1\",\"fingerprint\":\"cfp_v1_app_buyer\"}");
        Assert.Equal("cfp_v1_app_buyer", customer!.Fingerprint);
        Assert.Contains("\"fingerprint\":\"cfp_v1_app_buyer\"", JsonSerializer.Serialize(customer));
    }

    [Fact]
    public void PaymentAndOrderAnswerLifecycleQuestions()
    {
        var action = new PaymentNextAction { Type = PaymentNextActionType.Redirect };
        var payment = new Payment { Status = PaymentStatus.RequiresAction, NextAction = action };
        var order = new Order { Status = OrderStatus.RequiresPayment, Payment = payment };

        Assert.True(payment.RequiresAction());
        Assert.False(payment.IsTerminal());
        Assert.Same(action, payment.RequiredAction());
        Assert.True(order.RequiresPayment());
        Assert.Same(action, order.RequiredPaymentAction());
    }

    [Fact]
    public void CatalogAndPaymentMethodsAnswerProtocolQuestions()
    {
        var intent = new PurchaseIntent
        {
            Status = PurchaseIntentStatus.Used,
            Usage = new PurchaseIntentUsage
            {
                SingleUse = true,
                Order = new PurchaseIntentUsageOrder { Id = "or_123" }
            }
        };
        var product = new Product { Active = true, PublishedAt = DateTimeOffset.UtcNow };
        var method = new PaymentMethod { Active = true, VerifiedAt = DateTimeOffset.UtcNow };

        Assert.True(intent.IsSingleUse());
        Assert.Equal("or_123", intent.UsedOrderId());
        Assert.True(product.IsPublished());
        Assert.True(product.WasEverPublished());
        Assert.True(method.IsVerified());
        Assert.True(method.IsReusable());
    }
}
