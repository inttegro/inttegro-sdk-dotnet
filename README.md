# Inttegro .NET SDK

[![OpenSSF Scorecard](https://api.scorecard.dev/projects/github.com/inttegro/inttegro-sdk-dotnet/badge)](https://scorecard.dev/viewer/?uri=github.com/inttegro/inttegro-sdk-dotnet)

The official .NET client for building server-side Inttegro integrations.

[API documentation](https://dotnet.inttegro.dev/) · [Integration guides](https://studio.inttegro.com/sdks/dotnet)

> **Fastest, most modern path:** connect an agent to [Inttegro MCP](https://studio.inttegro.com/inttegro-mcp) at `https://mcp.inttegro.com`, then ask it to run `design_integration`. It will produce an implementation and test plan for your application. Use this SDK when you are ready to connect that plan to your .NET service.

All official Inttegro SDKs expose the same API capabilities. This package adds .NET-specific domain types, async APIs, and transport integration.

## Install

Targets .NET 8.

```bash
dotnet add package Inttegro
```

Store your secret key in the server environment:

```bash
export INTTEGRO_API_KEY="your_secret_key"
```

Never put the key in browser code, a mobile app, or source control. The client uses `https://api.inttegro.com` by default.

## Create a hosted checkout

Create and finalize an order, then send the customer to its hosted invoice URL:

```csharp
using Inttegro;
using Inttegro.Errors;

var apiKey = Environment.GetEnvironmentVariable("INTTEGRO_API_KEY")
    ?? throw new InvalidOperationException("INTTEGRO_API_KEY is required");

using var inttegro = new InttegroClient(apiKey);

try
{
    var order = await inttegro.Orders.CreateAsync(new OrderCreateRequest
    {
        RequestMeta = new RequestMeta { IdempotencyKey = "checkout-cart-123" },
        CustomerData = new CustomerData
        {
            Name = "Akua Mensah",
            EmailAddress = "akua@example.com",
            PhoneNumber = "+233544998605"
        },
        Finalize = true,
        CheckoutSettings = new CheckoutSettings
        {
            RedirectUrl = "https://example.com/orders/complete",
            CancelUrl = "https://example.com/cart"
        },
        LineItems = new List<LineItemParams>
        {
            new()
            {
                Type = LineItemType.Product,
                Product = new ProductDetailsParams
                {
                    Type = ProductType.Digital,
                    Name = "Monthly subscription",
                    Quantity = 1,
                    Price = new PriceParams { Currency = Inttegro.Money.Currency.GHS, Value = 5000 }
                }
            }
        }
    });

    var checkoutUrl = order.Invoice?.Format?.Web?.Url
        ?? throw new InvalidOperationException("Order did not include a checkout URL");
    Console.WriteLine($"{order.Id} {checkoutUrl}");
}
catch (InttegroApiException error)
{
    Console.Error.WriteLine($"{error.Code}: {error.Detail ?? error.Message}");
    throw;
}
```

Amounts use integer minor units: `5000` GHS is GHS 50.00. Reuse the same idempotency key when retrying the same logical write. If you omit one, the SDK generates a UUIDv7 key for mutating calls.

## Create a partial refund

Refunds target paid order line items and return money to the original payment method:

```csharp
var refund = await inttegro.Refunds.CreateAsync(new CreateRefundRequest
{
    OrderId = "or_0123456789abcdefghijklmnopqrstuvwxyzABCD",
    Reason = RefundReason.ItemReturned,
    LineItems =
    [
        new CreateRefundLineItem
        {
            OrderLineItemId = "oli_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMN",
            RefundAmount = new Inttegro.Money.AmountParams
            {
                Currency = Inttegro.Money.Currency.GHS,
                Value = 2500
            }
        }
    ]
});

Console.WriteLine($"{refund.Id} {refund.Status} {refund.Total?.Value}");
```

Use `Refunds.CreateAsync`, `Refunds.CancelAsync`, `Refunds.LookupAsync`, and `Refunds.PageAsync` to manage the refund lifecycle.

## Observe SDK operations

The SDK emits vendor-neutral `ActivitySource` spans and never configures an exporter or sends telemetry by itself. Register its source in your application's OpenTelemetry setup:

```csharp
services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource(InttegroClient.ActivitySourceName)
        .AddOtlpExporter());
```

Spans are named after logical operations such as `inttegro.orders.create`. HTTP attempts, response receipt, and decoding are span events. API keys, bodies, resource IDs, dynamic URLs, and exception messages are never recorded. See [SDK observability](https://studio.inttegro.com/sdk-observability) for the complete contract and pass `telemetryEnabled: false` when needed.

### Report SDK failures

Provide an application-owned reporter to receive one immutable, typed, privacy-safe report after an SDK operation finally fails. The default `Unexpected` policy reports transport, timeout, decoding, SDK, `unknown_error`, and server-side failures while leaving normal 4xx API errors alone:

```csharp
using Inttegro;
using Inttegro.Diagnostics;

var inttegro = new InttegroClient(
    apiKey,
    errorReporter: report => errorCollector.Enqueue(report)
);
```

Pass `errorReportingPolicy: ErrorReportingPolicy.All` to include expected API failures; cancellation is never reported. Reports contain the logical operation, static route, server host, status and request IDs when available, duration, safe API error codes, SDK identity, stable fingerprint, exception type, and trace IDs when tracing is active. They exclude credentials, headers, bodies, resource IDs, dynamic URLs, exception messages, and stack traces. Reporter failures are isolated and the original SDK exception is still thrown.

Error reporting is completely opt-in. Without `errorReporter`, the SDK does not calculate report metadata, create an event ID or timestamp, allocate a report, or serialize a payload.

## Work with the API

The SDK covers orders and checkout, customers, products and prices, purchase intents, payment methods, balances, payouts and refunds, notifications, files, application settings, keys, and country specifications. Resources use .NET properties such as `PurchaseIntents` and `PaymentMethods`.

.NET-specific features:

- Nullable-aware typed request and domain types with public enum constants.
- Async resource methods with `CancellationToken` support.
- Concrete domain objects and pages returned directly from every resource method.
- Injectable `HttpClient`, base URL, and timeout for connection reuse and tests.
- Structured authentication, rate-limit, network, timeout, and API exceptions.

See the [API reference](https://studio.inttegro.com/api-reference) for request fields and lifecycle rules, [errors](https://studio.inttegro.com/errors) for recovery guidance, and [idempotency](https://studio.inttegro.com/idempotency) for safe retries.

## Verify a release

The GitHub release for each version is the canonical record. It contains the exact NuGet and symbol packages uploaded to NuGet.org, SHA-256 checksums, and a Sigstore attestation tied to the source commit and release workflow. Source Link connects packaged binaries to this repository.

```bash
sha256sum --check SHA256SUMS
gh attestation verify Inttegro.6.1.0.nupkg \
  --repo inttegro/inttegro-sdk-dotnet
```

## Develop

```bash
dotnet build src/Inttegro/Inttegro.csproj
dotnet test tests/Inttegro.Tests/Inttegro.Tests.csproj
```
