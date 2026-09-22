using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Reflection;
using Inttegro;
using Inttegro.Errors;
using Inttegro.Diagnostics;
using Inttegro.Money;
using System.Linq;
using Xunit;

namespace Inttegro.Tests;

public class InttegroClientTests
{
    private static readonly Regex UuidV7Regex = new("^[0-9a-f]{8}-[0-9a-f]{4}-7[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$", RegexOptions.IgnoreCase);

    [Fact]
    public void AmountAndNetworkConstantsSerializeToLowercaseWireValues()
    {
        var amountJson = JsonSerializer.Serialize(new AmountParams { Currency = Currency.GHS, Value = 3005 });
        var priceJson = JsonSerializer.Serialize(new PriceParams { Currency = Currency.GHS, Value = 3005 });
        var catalogPriceJson = JsonSerializer.Serialize(new CatalogPriceParams
        {
            Amount = new AmountParams { Currency = Currency.GHS, Value = 3005 },
            Label = "Retail"
        });
        var networkJson = JsonSerializer.Serialize(MobileMoneyNetwork.MTN);
        var paymentMethodJson = JsonSerializer.Serialize(PaymentMethodType.MobileMoney);
        var paymentResultJson = JsonSerializer.Serialize(PaymentResultStatus.RequiresConfirmation);

        Assert.Equal("{\"currency\":\"ghs\",\"value\":3005}", amountJson);
        Assert.Equal("{\"currency\":\"ghs\",\"value\":3005}", priceJson);
        Assert.Equal("{\"product_id\":null,\"label\":\"Retail\",\"about\":null,\"amount\":{\"currency\":\"ghs\",\"value\":3005}}", catalogPriceJson);
        var catalogPrice = JsonSerializer.Deserialize<CatalogPrice>(
            "{\"id\":\"pr_123\",\"active\":true,\"nominal\":{\"currency\":\"ghs\",\"value\":3005},\"product_id\":\"prod_123\",\"created_at\":\"2026-09-02T12:00:00Z\"}"
        );
        Assert.Equal("prod_123", catalogPrice!.ProductId);
        Assert.Equal(DateTimeOffset.Parse("2026-09-02T12:00:00Z"), catalogPrice.CreatedAt);
        Assert.Equal("\"mtn\"", networkJson);
        Assert.Equal("\"mobile_money\"", paymentMethodJson);
        Assert.Equal("\"requires_confirmation\"", paymentResultJson);
    }

    [Fact]
    public void BalanceTransactionsDeserializeSemanticSourcesAndOrderEmbedding()
    {
        const string paymentJson = """
	    {"id":"bt_payment","type":"payment","payment_id":"py_123","order_id":"or_123","amount":{"currency":"GHS","value":2500},"available_amount":{"currency":"GHS","value":1500},"pending_amount":{"currency":"GHS","value":1000},"spent_amount":{"currency":"GHS","value":0},"allocations":[{"id":"bta_123","type":"payout","status":"pending","payout":{"id":"po_123","amount":{"currency":"GHS","value":1000}},"created_at":"2026-08-31T12:01:00Z","updated_at":"2026-08-31T12:01:00Z"}],"created_at":"2026-08-31T12:00:00Z"}
            """;
        var payment = JsonSerializer.Deserialize<BalanceTransaction>(paymentJson)!;
        Assert.Equal(BalanceTransactionType.Payment, payment.Type);
        Assert.Equal("py_123", payment.SourceId);
        Assert.Null(payment.RefundId);
        Assert.Equal(2500L, payment.Amount.Value);
	Assert.Equal(1500L, payment.AvailableAmount!.Value);
	var allocation = Assert.Single(payment.Allocations!);
	Assert.Equal(BalanceTransactionAllocationType.Payout, allocation.Type);
	Assert.Equal(BalanceTransactionAllocationStatus.Pending, allocation.Status);
	Assert.Equal("po_123", allocation.Payout!.Id);

        const string refundJson = """
            {"id":"bt_refund","type":"refund","refund_id":"rf_123","order_id":"or_123","amount":{"currency":"GHS","value":500},"created_at":"2026-08-31T12:01:00Z"}
            """;
        var refund = JsonSerializer.Deserialize<BalanceTransaction>(refundJson)!;
        Assert.Equal(BalanceTransactionType.Refund, refund.Type);
        Assert.Equal("rf_123", refund.SourceId);
        Assert.Null(refund.PaymentId);

        const string orderJson = """
            {"id":"or_123","payment":{"id":"py_123","balance_transaction":{"id":"bt_payment","type":"payment","payment_id":"py_123","order_id":"or_123","amount":{"currency":"GHS","value":2500},"created_at":"2026-08-31T12:00:00Z"}}}
            """;
        var order = JsonSerializer.Deserialize<Order>(orderJson)!;
        Assert.Equal(BalanceTransactionType.Payment, order.Payment!.BalanceTransaction!.Type);
    }

    [Fact]
    public void PurchaseIntentExposesNestedResponseTypes()
    {
        const string json = """
            {"allow_variants":false,"created_at":"2026-09-09T12:00:00Z","id":"sale_123","merchant":{"organization_name":"Tea House Ltd"},"product":{"active":true,"created_at":"2026-09-09T11:00:00Z","dimensions":{"digital":{"bytes":1024}},"id":"prod_123","name":"Tea guide","type":"digital"},"quantity":{"min":1},"status":"active","usage":{"order":{"created_at":"2026-09-09T12:02:00Z","id":"or_123"},"single_use":true}}
            """;

        var intent = JsonSerializer.Deserialize<PurchaseIntent>(json)!;

        Assert.Equal("Tea House Ltd", intent.Merchant!.OrganizationName);
        Assert.Equal(1024, intent.Product!.Dimensions!.Digital!.Bytes);
        Assert.Equal("or_123", intent.Usage.Order!.Id);
        Assert.Equal(PurchaseIntentStatus.Active, intent.Status);
        Assert.Equal(DateTimeOffset.Parse("2026-09-09T12:00:00Z"), intent.CreatedAt);
    }

    [Fact]
    public void PaymentsDeserializeAsSemanticTypedObjects()
    {
        const string orderJson = """
            {"id":"or_123","payment":{"id":"py_123","status":"requires_action","latest_attempt":{"status":"initiated"},"next_action":{"type":"confirm_payment","confirm_payment":{"request":{"sent_via":"sms"}}}}}
            """;

        var order = JsonSerializer.Deserialize<Order>(orderJson)!;

        Assert.IsType<Payment>(order.Payment);
        Assert.Equal(PaymentStatus.RequiresAction, order.Payment!.Status);
        Assert.Equal(PaymentAttemptStatus.Initiated, order.Payment.LatestAttempt!.Status);
        Assert.Equal(PaymentNextActionType.ConfirmPayment, order.Payment.NextAction!.Type);
        Assert.Equal(PaymentConfirmationChannel.Sms, order.Payment.NextAction.ConfirmPayment!.Request!.SentVia);
    }

    [Fact]
    public async Task CallsAllEndpointsWithExpectedPaths()
    {
        var handler = new RecordingHandler();
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        var client = new InttegroClient("test", httpClient: httpClient);

        await client.Orders.CreateAsync(new { number = "1" });
        await client.Orders.LookupAsync("or_1");
        await client.Orders.UpdateAsync(new { order_id = "or_1", number = "ORDER-1" });
        await client.Orders.PayAsync(new { order_id = "or_1" });
        await client.Orders.ConfirmPaymentAsync(new { order_id = "or_1", token = "123456" });
        await client.Orders.RequestConfirmationAsync("or_1");
        await client.Orders.FinalizeAsync("or_1");
        await client.Orders.SendInvoiceAsync(new OrderSendInvoiceParams { OrderId = "or_1" });
        await client.Orders.SendReceiptAsync(new OrderSendReceiptParams { OrderId = "or_1" });
        await client.Orders.CompleteAsync(new { order_id = "or_1" });
        await client.Orders.CancelAsync("or_1");
        var refundRequest = new CreateRefundRequest
        {
            OrderId = "or_0123456789abcdefghijklmnopqrstuvwxyzABCD",
            Reason = RefundReason.ItemReturned,
            LineItems =
            [
                new CreateRefundLineItem
                {
                    OrderLineItemId = "oli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN",
                    RefundAmount = new AmountParams { Currency = Currency.GHS, Value = 2500 },
                    Reason = RefundReason.ItemNotAsDescribed
                }
            ],
            RequestMeta = new RequestMeta { IdempotencyKey = "refund_order_alias_001" }
        };
        await client.Orders.PageAsync(new { });
        await client.Orders.SearchAsync(new ResourceSearchRequest { Text = "tea" });

        await client.Refunds.CreateAsync(refundRequest);
        await client.Refunds.CancelAsync(new CancelRefundRequest
        {
            RefundId = "rf_1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcd",
            Reason = "Customer no longer wants the refund"
        });
        await client.Refunds.LookupAsync("rf_1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcd");
        await client.Refunds.PageAsync(new PageRefundsRequest { PageNumber = 1 });

        await client.PaymentMethods.TokenizeAsync(new
        {
            customer_id = "cu_1",
            payment_method_data = new
            {
                type = "mobile_money",
                mobile_money = new
                {
                    network = "mtn",
                    account_number = "0544998605"
                }
            }
        });
        await client.PaymentMethods.VerifyAsync("pm_1");
        await client.PaymentMethods.ConfirmVerificationAsync(new { payment_method_id = "pm_1", token = "123456" });
        await client.PaymentMethods.LookupAsync("pm_1");
        await client.PaymentMethods.PageAsync(new { });
        await client.PaymentMethods.UpdateAsync(new { payment_method_id = "pm_1", active = true });
        await client.PaymentMethods.ActivateAsync("pm_1");
        await client.PaymentMethods.ArchiveAsync("pm_1");
        await client.PaymentMethods.DisactivateAsync("pm_1");
        await client.PaymentMethods.UnarchiveAsync("pm_1");
        await client.PaymentMethods.DeleteAsync("pm_1");
        await client.PaymentMethods.SettingsAsync();

        await client.Payouts.SetDestinationsAsync(new PayoutDestinations { GHS = "dest" });
        await client.Payouts.SettingsAsync();
        await client.Payouts.DisableAutomaticAsync();
        await client.Payouts.EnableAutomaticAsync();
        await client.Payouts.EnableFXAsync();
        await client.Payouts.DisableFXAsync();
        await client.Payouts.PageAsync(new PayoutPageRequest { PageNumber = 1 });
        await client.Payouts.LookupAsync("po_1");
        await client.Payouts.ScheduleAsync(new SchedulePayoutRequest { DestinationId = "fa_1", MaxAmount = 1000, Reference = "PAYOUT-1" });
        await client.Payouts.CancelAsync("po_1");
        await client.Payouts.SearchAsync(new ResourceSearchRequest { Text = "tea" });

        await client.BalanceTransactions.LookupAsync("txn_1");
        await client.BalanceTransactions.PageAsync(new { });

        await client.FinancialAccounts.CreateAsync(new { });
        await client.FinancialAccounts.LookupAsync("fa_1");
        await client.FinancialAccounts.ReconnectAsync("fa_1");
        await client.FinancialAccounts.ConnectAsync(new { });
        await client.FinancialAccounts.ArchiveAsync(new { account_id = "fa_1" });
        await client.FinancialAccounts.PageAsync(new { });
        await client.FinancialAccounts.VerifyAsync(new { account_id = "fa_1" });
        await client.FinancialAccounts.EnablePushAsync(new FinancialAccountToggleRequest { AccountId = "fa_1" });
        await client.FinancialAccounts.DisablePushAsync(new FinancialAccountToggleRequest
        {
            AccountId = "fa_1",
            UnsetAsPayoutDestination = true
        });
        await client.FinancialAccounts.EnablePullAsync(new FinancialAccountToggleRequest { AccountId = "fa_1" });
        await client.FinancialAccounts.DisablePullAsync(new FinancialAccountToggleRequest { AccountId = "fa_1" });
        await client.FinancialAccounts.DisconnectAsync(new FinancialAccountToggleRequest
        {
            AccountId = "fa_1",
            UnsetAsPayoutDestination = true
        });
        await client.FinancialAccounts.SearchAsync(new ResourceSearchRequest { Text = "tea" });

        await client.Customers.CreateAsync(new { name = "Jane Doe" });
        await client.Customers.LookupAsync("cu_1");
        await client.Customers.PageAsync(new { page_number = 1 });
        await client.Customers.SearchAsync(new ResourceSearchRequest { Text = "tea" });

        await client.Products.CreateAsync(new
        {
            type = "physical",
            name = "Product"
        });
        await client.Products.AddPriceAsync(new
        {
            product_id = "prod_1",
            amount = new { currency = "ghs", value = 5000 },
            set_as_default = true
        });
        await client.Products.SetDefaultUnitPriceAsync(new { product_id = "prod_1", price_id = "pr_1" });
        await client.Products.LookupAsync("prod_1");
        await client.Products.UpdateAsync(new { product_id = "prod_1", name = "Updated" });
        await client.Products.PublishAsync(new { product_id = "prod_1" });
        await client.Products.UnpublishAsync(new { product_id = "prod_1" });
        await client.Products.ArchiveAsync(new { product_id = "prod_1" });
        await client.Products.PageAsync(new { page_number = 1 });
        await client.Products.SearchAsync(new ResourceSearchRequest { Text = "tea" });

        await client.Prices.CreateAsync(new { currency = "ghs", amount = 5000 });
        await client.Prices.LookupAsync("pr_1");
        await client.Prices.UpdateAsync(new UpdatePriceRequest { PriceId = "pr_1", Label = "Retail" });
        await client.Prices.ActivateAsync("pr_1");
        await client.Prices.DeactivateAsync("pr_1");
        await client.Prices.PageAsync(new { });

        await client.Chimes.SendAsync(new { message = "hi" });
        await client.Chimes.LookupAsync("ch_1");
        await client.Chimes.ScheduleAsync(new { recipients = new[] { "+233544998605" }, full_message = "later", send_after = "2026-01-18T10:00:00Z" });
        await client.Chimes.BroadcastAsync(new { recipients = new[] { "+233544998605" }, message_template = "hello", service_name = "marketing" });
        await client.Chimes.PageAsync(new { });

        await client.Schedules.LookupAsync("sch_1");
        await client.Schedules.CancelAsync("sch_1");
        await client.Broadcasts.LookupAsync("brc_1");
        await client.Broadcasts.CancelAsync("brc_1");

        await client.Otp.InitiateAsync(new
        {
            recipient = "+233",
            sender = "Acme",
            service_name = "Acme Bank",
            idempotency_key = "otp_login_1700000000"
        });
        await client.Otp.VerifyAsync(new { transaction_id = "txn_1", recipient = "+233", token = "123456" });
        await client.Otp.LookupAsync(new { transaction_id = "txn_1" });
        await client.Otp.CancelAsync(new { transaction_id = "txn_1", reason = "test" });

        await client.Apps.CreateAsync(new { name = "My App" });
        await client.Apps.LookupAsync();
        await client.Apps.UpdateAsync(new { alias = "my-app" });
        await client.Keys.GenerateAsync(new { label = "Production" });
        await client.Keys.LookupAsync("sk_1");
        await client.Keys.PageAsync(new { });
        await client.Keys.UpdateAsync(new { secret_key_id = "sk_1", label = "Production checkout" });
        await client.Keys.DestroyAsync("sk_1");
        await client.Keys.UsageAsync("sk_1");
        await client.FileReferences.ReconcileAsync(new { resource_type = "product", resource_id = "prod_1" });
        await client.PurchaseIntents.CreateAsync(new { product_id = "prod_1", price_id = "pr_1", quantity = new { min = 1, max = 5 } });
        await client.PurchaseIntents.LookupAsync("sale_1");
        await client.PurchaseIntents.PageAsync(new { page_number = 1, page_size = 20 });
        await client.PurchaseIntents.UpdateAsync(new { id = "sale_1", quantity = new { min = 1, max = 3 } });
        await client.PurchaseIntents.CancelAsync("sale_1");

        await client.Spec.CountriesAsync();
        var balance = await client.Balances.GetAsync();
        Assert.Equal(1000, balance.GHS.Available.Amount);

        var expectedPaths = new[]
        {
            "/orders/create",
            "/orders/lookup",
            "/orders/update",
            "/orders/pay",
            "/orders/confirm_payment",
            "/orders/request_confirmation",
            "/orders/finalize",
            "/orders/send_invoice",
            "/orders/send_receipt",
            "/orders/complete",
            "/orders/cancel",
            "/orders/page",
            "/orders/search",
            "/refunds/create",
            "/refunds/cancel",
            "/refunds/lookup",
            "/refunds/page",
            "/payment_methods/tokenize",
            "/payment_methods/verify",
            "/payment_methods/confirm_verification",
            "/payment_methods/lookup",
            "/payment_methods/page",
            "/payment_methods/update",
            "/payment_methods/activate",
            "/payment_methods/archive",
            "/payment_methods/disactivate",
            "/payment_methods/unarchive",
            "/payment_methods/delete",
            "/payment_methods/settings",
            "/payouts/set_destinations",
            "/payouts/settings",
            "/payouts/disable",
            "/payouts/enable",
            "/payouts/enable_fx",
            "/payouts/disable_fx",
            "/payouts/page",
            "/payouts/lookup",
            "/payouts/schedule",
            "/payouts/cancel",
            "/payouts/search",
            "/balance_transactions/lookup",
            "/balance_transactions/page",
            "/financial_accounts/create",
            "/financial_accounts/lookup",
            "/financial_accounts/reconnect",
            "/financial_accounts/connect",
            "/financial_accounts/archive",
            "/financial_accounts/page",
            "/financial_accounts/verify",
            "/financial_accounts/enable_push",
            "/financial_accounts/disable_push",
            "/financial_accounts/enable_pull",
            "/financial_accounts/disable_pull",
            "/financial_accounts/disconnect",
            "/financial_accounts/search",
            "/customers/create",
            "/customers/lookup",
            "/customers/page",
            "/customers/search",
            "/products/create",
            "/products/add_price",
            "/products/set_default_unit_price",
            "/products/lookup",
            "/products/update",
            "/products/publish",
            "/products/unpublish",
            "/products/archive",
            "/products/page",
            "/products/search",
            "/prices/create",
            "/prices/lookup",
            "/prices/update",
            "/prices/activate",
            "/prices/deactivate",
            "/prices/page",
            "/chimes/send",
            "/chimes/lookup",
            "/chimes/schedule",
            "/chimes/broadcast",
            "/chimes/page",
            "/schedules/lookup",
            "/schedules/cancel",
            "/broadcasts/lookup",
            "/broadcasts/cancel",
            "/otp/initiate",
            "/otp/verify",
            "/otp/lookup",
            "/otp/cancel",
            "/apps/create",
            "/apps/lookup",
            "/apps/update",
            "/keys/generate",
            "/keys/lookup",
            "/keys/page",
            "/keys/update",
            "/keys/destroy",
            "/keys/usage",
            "/file_references/reconcile",
            "/purchase_intents/create",
            "/purchase_intents/lookup",
            "/purchase_intents/page",
            "/purchase_intents/update",
            "/purchase_intents/cancel",
            "/spec/countries",
            "/balances"
        };

        Assert.Equal(expectedPaths, handler.Requests.Select(r => r.RequestUri!.AbsolutePath).ToArray());

        // Order envelopes are lowered to the domain model.
        handler.ResponseBody = JsonSerializer.Serialize(new { order = new { id = "or_123" } });
        var resp = await client.Orders.CreateAsync(new { number = "3" });
        Assert.Equal("or_123", resp.Id);
    }

    [Fact]
    public async Task RefundCreateUsesTheTypedContract()
    {
        var handler = new RecordingHandler
        {
            ResponseBody = """
                {"refund":{"id":"rf_1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcd","order_id":"or_0123456789abcdefghijklmnopqrstuvwxyzABCD","status":"pending","settlement":{"type":"payment_method","payment_method":{"id":"pm_123","type":"mobile_money","mobile_money":{"network":"mtn","account_number":"****7831","last4":"7831"}}},"total":{"currency":"ghs","value":2500},"line_items":[{"id":"rli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN","order_line_item_id":"oli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN","order_line_item":{"id":"oli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN","type":"product","quantity":2,"product":{"id":"prod_123","name":"Premium subscription"}},"original_amount_paid":{"currency":"ghs","value":5000},"refund_amount":{"currency":"ghs","value":2500}}],"reason":"item_returned","created_at":"2026-09-02T10:00:00Z"}}
                """
        };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        var client = new InttegroClient("test", httpClient: httpClient);
        var payload = new CreateRefundRequest
        {
            OrderId = "or_0123456789abcdefghijklmnopqrstuvwxyzABCD",
            Reason = RefundReason.ItemReturned,
            ReasonDetails = "Returned unopened",
            Reference = "RETURN-2026-0001",
            LineItems =
            [
                new CreateRefundLineItem
                {
                    OrderLineItemId = "oli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN",
                    RefundAmount = new AmountParams { Currency = Currency.GHS, Value = 2500 },
                    Reason = RefundReason.ItemNotAsDescribed,
                    ReasonDetails = "Wrong size"
                }
            ],
            RequestMeta = new RequestMeta { IdempotencyKey = "refund_contract_001" }
        };

        var canonical = await client.Refunds.CreateAsync(payload);
        Assert.Equal("rf_1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcd", canonical.Id);
        Assert.Equal(RefundStatus.Pending, canonical.Status);
        Assert.Equal(RefundReason.ItemReturned, canonical.Reason);
        var settlement = Assert.IsType<RefundPaymentMethodSettlement>(canonical.Settlement);
        var method = Assert.IsType<RefundSettlementMobileMoneyPaymentMethod>(settlement.PaymentMethod);
        Assert.Equal("****7831", method.MobileMoney.AccountNumber);
        var lineItem = Assert.IsType<RefundProductOrderLineItem>(Assert.Single(canonical.LineItems!).OrderLineItem);
        Assert.Equal(2, lineItem.Quantity);
        Assert.Equal("prod_123", lineItem.Product.Id);
        Assert.Equal(new[] { "/refunds/create" }, handler.Requests.Select(r => r.RequestUri!.AbsolutePath));
        using var request = JsonDocument.Parse(handler.Bodies[0]);
        Assert.Equal("oli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN", request.RootElement.GetProperty("line_items")[0].GetProperty("order_line_item_id").GetString());
        Assert.Equal(2500L, request.RootElement.GetProperty("line_items")[0].GetProperty("refund_amount").GetProperty("value").GetInt64());
        Assert.Equal("item_returned", request.RootElement.GetProperty("reason").GetString());
    }

    [Fact]
    public async Task RaisesAuthenticationErrors()
    {
        var handler = new RecordingHandler
        {
            StatusCode = HttpStatusCode.Unauthorized,
            ResponseBody =
                "{\"type\":\"authentication_error\",\"code\":\"invalid_api_key\",\"url\":\"https://studio.inttegro.com/e/invalid_api_key\",\"message\":\"invalid key\",\"detail\":\"API key is missing or invalid.\",\"fix_code\":\"check_api_key\",\"cause\":\"authentication_failure\"}"
        };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        var client = new InttegroClient("bad", httpClient: httpClient);

        await Assert.ThrowsAsync<InttegroAuthenticationException>(() => client.Orders.LookupAsync("or_1"));
    }

    [Fact]
    public async Task MutatingPostsGenerateRequestMetaIdempotencyKey()
    {
        var handler = new RecordingHandler();
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        var client = new InttegroClient("test", httpClient: httpClient);

        await client.Orders.CreateAsync(new { number = "ORDER-1", idempotency_key = "legacy" });

        using var document = JsonDocument.Parse(handler.Bodies.Single());
        var root = document.RootElement;
        Assert.False(root.TryGetProperty("idempotency_key", out _));
        var key = root.GetProperty("request_meta").GetProperty("idempotency_key").GetString();
        Assert.Matches(UuidV7Regex, key!);
    }

    [Fact]
    public async Task ResourceSearchDoesNotGenerateMutationMetadata()
    {
        var handler = new RecordingHandler();
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        var client = new InttegroClient("test", httpClient: httpClient);

        var page = await client.Orders.SearchAsync(new ResourceSearchRequest
        {
            Text = "tea",
            Sort = new ResourceSearchSort
            {
                Field = ResourceSearchSortField.Relevance,
                Direction = ResourceSearchSortDirection.Descending
            }
        });

        using var document = JsonDocument.Parse(handler.Bodies.Single());
        Assert.False(document.RootElement.TryGetProperty("request_meta", out _));
        Assert.Equal(ResourceSearchFreshnessState.Current, page.Freshness.State);
    }

    [Fact]
    public async Task MessageTemplatesCreateUsesRequestMetaIdempotencyByDefault()
    {
        var handler = new RecordingHandler();
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        var client = new InttegroClient("test", httpClient: httpClient);

        await client.MessageTemplates.CreateAsync(new
        {
            name = "welcome_sms",
            channel = "sms",
            purpose = "marketing",
            sms = new { message_template = "Welcome {{name}}" }
        });

        var request = Assert.Single(handler.Requests);
        Assert.False(request.Headers.Contains("Idempotency-Key"));
        using var document = JsonDocument.Parse(handler.Bodies.Single());
        var key = document.RootElement.GetProperty("request_meta").GetProperty("idempotency_key").GetString();
        Assert.Matches(UuidV7Regex, key!);
    }

    [Fact]
    public async Task TelemetryTracesLogicalOperationPropagatesAndRedactsRequestData()
    {
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == InttegroClient.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activities.Add
        };
        ActivitySource.AddActivityListener(listener);

        var handler = new RecordingHandler { RequestId = "req_telemetry" };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        using var client = new InttegroClient("sk_test_do_not_trace", httpClient: httpClient);

        await client.Orders.LookupAsync("or_private_123");

        var activity = Assert.Single(activities);
        Assert.Equal("inttegro.orders.lookup", activity.DisplayName);
        Assert.Equal("orders.lookup", activity.GetTagItem("inttegro.operation.name"));
        Assert.Equal(200, activity.GetTagItem("http.response.status_code"));
        Assert.Equal("req_telemetry", activity.GetTagItem("inttegro.request.id"));
        Assert.Contains(activity.Events, item => item.Name == "inttegro.request.prepared");
        Assert.Contains(activity.Events, item => item.Name == "inttegro.http.attempt.started");
        Assert.Contains(activity.Events, item => item.Name == "inttegro.response.received");
        Assert.Contains(activity.Events, item => item.Name == "inttegro.response.decoded");

        var request = Assert.Single(handler.Requests);
        Assert.True(request.Headers.Contains("traceparent"));
        var telemetryText = string.Join(" ", activity.TagObjects.Select(tag => $"{tag.Key}={tag.Value}"))
            + string.Join(" ", activity.Events.Select(item => item.Name));
        Assert.DoesNotContain("sk_test_do_not_trace", telemetryText);
        Assert.DoesNotContain("or_private_123", telemetryText);
    }

    [Fact]
    public void TelemetryDoesNotNameUnknownRoutesFromResourceIds()
    {
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == InttegroClient.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activities.Add
        };
        ActivitySource.AddActivityListener(listener);

        var telemetryType = typeof(InttegroClient).Assembly.GetType("Inttegro.Http.Telemetry")!;
        var telemetry = Activator.CreateInstance(
            telemetryType,
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { new Uri("https://api.inttegro.com"), true },
            culture: null
        )!;
        var start = telemetryType.GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic)!;
        using ((IDisposable)start.Invoke(
            telemetry,
            new object?[] { HttpMethod.Get, new Uri("/orders/or_private_123", UriKind.Relative), null }
        )!) { }

        var activity = Assert.Single(activities);
        Assert.Equal("inttegro.http.request", activity.DisplayName);
        Assert.Null(activity.GetTagItem("url.template"));
        Assert.DoesNotContain("or_private_123", string.Join(" ", activity.TagObjects));
    }

    [Fact]
    public async Task ResponseEnvelopeExposesResponseOnlyMetadata()
    {
        var handler = new RecordingHandler
        {
            RequestId = "req_123",
            RetryAfter = "15",
            ResponseBody = """
                {"order":{"id":"or_123"},"response_meta":{"request_id":"req_123","debug":{"provider_attempts":1}}}
                """
        };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        using var client = new InttegroClient("test", httpClient: httpClient);

        var response = await client.Orders.CreateWithResponseAsync(new OrderCreateRequest
        {
            CustomerId = "cu_123",
            LineItems = [new LineItemParams { Type = LineItemType.Product }]
        });

        Assert.Equal("or_123", response.Data.Id);
        Assert.Equal(200, response.StatusCode);
        Assert.Equal("req_123", response.RequestId);
        Assert.Equal("15", response.RetryAfter);
        Assert.Equal("req_123", response.Headers["x-request-id"]);
        Assert.Equal("req_123", response.Meta!["request_id"]);
    }

    [Fact]
    public async Task TelemetryRecordsSafeHttpFailure()
    {
        var activities = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == InttegroClient.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activities.Add
        };
        ActivitySource.AddActivityListener(listener);

        var handler = new RecordingHandler
        {
            StatusCode = HttpStatusCode.Unauthorized,
            ResponseBody = "{\"message\":\"sk_live_private must never be traced\"}"
        };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        using var client = new InttegroClient("sk_live_private", httpClient: httpClient);

        await Assert.ThrowsAsync<InttegroAuthenticationException>(() => client.Orders.LookupAsync("or_private"));

        var activity = Assert.Single(activities);
        Assert.Equal(ActivityStatusCode.Error, activity.Status);
        Assert.Equal("http_401", activity.GetTagItem("error.type"));
        Assert.Contains(activity.Events, item => item.Name == "inttegro.request.failed");
        var telemetryText = string.Join(" ", activity.TagObjects.Select(tag => $"{tag.Key}={tag.Value}"))
            + string.Join(" ", activity.Events.Select(item => item.Name));
        Assert.DoesNotContain("sk_live_private", telemetryText);
        Assert.DoesNotContain("or_private", telemetryText);
    }

    [Fact]
    public async Task ErrorReportingReportsOnePrivacySafeFinalFailureWhenConfigured()
    {
        var reports = new List<ErrorReport>();
        var handler = new RecordingHandler
        {
            StatusCode = HttpStatusCode.ServiceUnavailable,
            RequestId = "req_456",
            ResponseBody = "{\"error\":{\"type\":\"transient_error\",\"code\":\"provider_failed\",\"fix_code\":\"repeat_same_request\",\"message\":\"private provider detail\"}}"
        };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        using var client = new InttegroClient(
            "sk_live_must_not_appear",
            httpClient: httpClient,
            telemetryEnabled: false,
            errorReporter: report =>
            {
                reports.Add(report);
                throw new InvalidOperationException("collector unavailable");
            }
        );

        var exception = await Assert.ThrowsAsync<InttegroApiException>(
            () => client.Orders.LookupAsync("or_private")
        );

        var report = Assert.Single(reports);
        Assert.Equal("http_503", report.Category);
        Assert.Equal("orders.lookup", report.Operation);
        Assert.Equal("POST", report.Http.Method);
        Assert.Equal("/orders/lookup", report.Http.Route);
        Assert.Equal(503, report.Http.StatusCode);
        Assert.Equal("req_456", report.Http.RequestId);
        Assert.Equal("transient_error", report.ApiError?.Type);
        Assert.Equal("inttegro:dotnet:orders.lookup:http_503:503", report.Fingerprint);
        Assert.Same(report, exception.Report);
        var encoded = JsonSerializer.Serialize(report);
        Assert.Contains("\"schemaVersion\":1", encoded);
        Assert.Contains("\"serverAddress\":\"api.inttegro.com\"", encoded);
        Assert.DoesNotContain("private provider detail", encoded);
        Assert.DoesNotContain("sk_live_must_not_appear", encoded);
        Assert.DoesNotContain("or_private", encoded);
    }

    [Fact]
    public async Task DefaultErrorReportingSkipsExpectedApiErrors()
    {
        var reports = new List<ErrorReport>();
        var handler = new RecordingHandler
        {
            StatusCode = HttpStatusCode.BadRequest,
            ResponseBody = "{\"error\":{\"type\":\"invalid_request_parameter\"}}"
        };
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.inttegro.com") };
        using var client = new InttegroClient(
            "test",
            httpClient: httpClient,
            telemetryEnabled: false,
            errorReporter: reports.Add
        );

        var exception = await Assert.ThrowsAsync<InttegroApiException>(
            () => client.Orders.LookupAsync("or_private")
        );

        Assert.Empty(reports);
        Assert.Null(exception.Report);
    }

    private class RecordingHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = new();
        public List<string> Bodies { get; } = new();
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public string? ResponseBody { get; set; }
        public string? RequestId { get; set; }
        public string? RetryAfter { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request);
            Bodies.Add(request.Content == null ? "" : await request.Content.ReadAsStringAsync(cancellationToken));
            var responseBody = ResponseBody ?? DefaultResponseBody(request.RequestUri!.AbsolutePath);
            var response = new HttpResponseMessage(StatusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
            response.Headers.Date = DateTimeOffset.UtcNow;
            if (!string.IsNullOrWhiteSpace(RequestId))
            {
                response.Headers.TryAddWithoutValidation("x-request-id", RequestId);
            }
            if (!string.IsNullOrWhiteSpace(RetryAfter))
            {
                response.Headers.TryAddWithoutValidation("Retry-After", RetryAfter);
            }
            return response;
        }

        private static string DefaultResponseBody(string path) => path switch
        {
            var value when value.EndsWith("/search", StringComparison.Ordinal) => """
                {"search":{"resource_types":["order"],"sort":{"field":"relevance","direction":"desc"},"page_size":20,"result_count":0,"has_more":false,"total":{"value":0,"relation":"exact"},"resource_totals":[],"results":[],"facets":[],"freshness":{"state":"current"}}}
                """,
            "/orders/page" => "{\"page\":{\"number\":0,\"size\":0,\"orders\":[]}}",
            "/orders/send_invoice" or "/orders/send_receipt" => "{}",
            var value when value.StartsWith("/orders/", StringComparison.Ordinal) => "{\"order\":{\"id\":\"or_123\"}}",

            "/refunds/page" => "{\"page\":{}}",
            var value when value.StartsWith("/refunds/", StringComparison.Ordinal) => "{\"refund\":{}}",

            "/payment_methods/verify" => "{\"verification\":{}}",
            "/payment_methods/page" => "{\"page\":{}}",
            "/payment_methods/settings" => "{\"settings\":{}}",
            "/payment_methods/delete" => "{}",
            var value when value.StartsWith("/payment_methods/", StringComparison.Ordinal) => "{\"payment_method\":{}}",

            "/payouts/page" => "{\"page\":{}}",
            "/payouts/lookup" or "/payouts/schedule" or "/payouts/cancel" => "{\"payout\":{}}",
            var value when value.StartsWith("/payouts/", StringComparison.Ordinal) => "{\"settings\":{}}",

            "/balance_transactions/page" => "{\"page\":{}}",
            "/balance_transactions/lookup" => "{\"transaction\":{}}",
            "/financial_accounts/page" => "{\"page\":{}}",
            var value when value.StartsWith("/financial_accounts/", StringComparison.Ordinal) => "{\"account\":{}}",
            "/customers/page" => "{\"page\":{}}",
            var value when value.StartsWith("/customers/", StringComparison.Ordinal) => "{\"customer\":{}}",

            "/products/add_price" => "{\"price\":{}}",
            "/products/page" => "{\"page\":{}}",
            var value when value.StartsWith("/products/", StringComparison.Ordinal) => "{\"product\":{}}",
            "/prices/page" => "{\"page\":{}}",
            var value when value.StartsWith("/prices/", StringComparison.Ordinal) => "{\"price\":{}}",

            "/chimes/schedule" => "{\"scheduled_chime\":{}}",
            "/chimes/broadcast" => "{\"broadcast\":{}}",
            "/chimes/page" => "{\"page\":{}}",
            var value when value.StartsWith("/chimes/", StringComparison.Ordinal) => "{\"chime\":{}}",
            var value when value.StartsWith("/schedules/", StringComparison.Ordinal) => "{\"scheduled_chime\":{}}",
            var value when value.StartsWith("/broadcasts/", StringComparison.Ordinal) => "{\"broadcast\":{}}",

            "/otp/verify" => "{}",
            var value when value.StartsWith("/otp/", StringComparison.Ordinal) => "{\"transaction\":{}}",
            var value when value.StartsWith("/apps/", StringComparison.Ordinal) => "{\"app\":{}}",
            "/keys/page" => "{\"page\":{}}",
            "/keys/usage" => "{}",
            var value when value.StartsWith("/keys/", StringComparison.Ordinal) => "{\"key\":{}}",

            "/file_references/reconcile" => "{}",
            "/purchase_intents/page" => "{\"page\":{}}",
            var value when value.StartsWith("/purchase_intents/", StringComparison.Ordinal) => "{\"purchase_intent\":{}}",
            "/spec/countries" => "{\"countries\":{}}",
            "/balances" => "{\"balances\":{\"ghs\":{\"available\":{\"amount\":1000},\"pending\":{\"amount\":200},\"reserved\":{\"amount\":100},\"refund\":{\"amount\":50},\"includes_transactions_before\":\"2026-09-09T12:00:00Z\"}}}",

            "/files/page" or "/file_links/page" or "/upload_requests/page" or "/message_templates/page" => "{\"page\":{}}",
            var value when value.StartsWith("/files/", StringComparison.Ordinal) => "{\"file\":{}}",
            var value when value.StartsWith("/file_links/", StringComparison.Ordinal) => "{\"file_link\":{}}",
            var value when value.StartsWith("/upload_requests/", StringComparison.Ordinal) => "{\"upload_request\":{}}",
            "/message_templates/render_preview" => "{}",
            var value when value.StartsWith("/message_templates/", StringComparison.Ordinal) => "{\"message_template\":{}}",
            _ => "{}"
        };
    }
}
