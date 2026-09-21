## [Unreleased]

## [9.1.0] - 2026-09-21

- Added typed search across customers, financial accounts, orders, payouts, and
  products, including filters, facets, sorting, cursor pagination, totals, and
  freshness metadata.
- Added typed verification purposes for OTP initiation requests.

## [9.0.0] - 2026-09-16

- Breaking: customer create requests now expose native `Address` properties
  for billing and shipping addresses instead of untyped JSON objects.
- Standardized customer custom data on the validated `CustomDataInput` value
  while retaining open-ended JSON serialization.

## [8.0.0] - 2026-09-14

- Breaking: replaced payout dictionaries and anonymous payloads with named
  request, response, settings, page, error, and destination models.
- Made `Ghs` the explicit supported payout-destination property and exposed
  payout timestamps as `DateTimeOffset` values.
- Removed server-internal purchase-intent activity response models and exposed
  refund order and cancellation context.

## [7.1.0] - 2026-09-12

- Added opt-in response envelopes that expose status, headers, request IDs,
  retry hints, and response metadata without changing existing resource return
  values.
- Added fluent domain helpers for common resource state and actionability checks.

## [7.0.0] - 2026-09-10

- Breaking: replaced generic dictionaries with named models for balances, purchase intents, products, payment methods, payments, and orders.
- Breaking: exposed API timestamps as `DateTimeOffset` values and accepted them in timestamp request objects.

## [6.2.0] - 2026-09-06

- Added opt-in, typed error reporting to application-owned collectors with privacy-safe payloads, stable fingerprints, isolated reporter failures, and no reporting work when unconfigured.

## [6.1.0] - 2026-09-04

- Added vendor-neutral `ActivitySource` spans for logical SDK operations, HTTP attempts, response receipt, decoding, and safe failure categories.
- Added W3C trace-context propagation and per-client telemetry disabling without a runtime package dependency.
- Kept request bodies, credentials, resource identifiers, dynamic URLs, and exception details out of telemetry.

## [6.0.0] - 2026-09-03

- Breaking: moved wallet types into the `Inttegro.Wallets` namespace.
- Breaking: moved financial-account bank types into the `Inttegro.BankAccounts` namespace.
- Kept financial-account lifecycle types in the root `Inttegro` namespace.

## [5.0.0] - 2026-09-03

- Breaking: replaced the catch-all `ApiEnums` constant container with native top-level domain enum types.
- Added exact wire-value JSON serialization for the new domain enums.

## [4.0.0] - 2026-09-03

- Breaking: replaced order-prefixed payment models with semantic `Payment`, `PaymentAttempt`, `PaymentNextAction`, and payout-configuration types.
- Added native payment enums with exact wire-value serialization and removed their duplicate `ApiEnums` constants.
- Separated request and response amount, price, and order line-item types.

## [3.0.1] - 2026-09-03

- Corrected the transport user agent and README to match the direct domain return API.

## [3.0.0] - 2026-09-03

- Breaking: resource methods now return concrete domain objects and pages instead of the generic transport response.
- Breaking: removed the public response wrapper and renamed payment result status constants to `PaymentResultStatus`.

## [2.0.0] - 2026-09-02

- Breaking: moved public request, response, enum, and domain types from `Inttegro.Models` to `Inttegro`.

## [1.0.0] - 2026-09-01

- Breaking: renamed the package, namespaces, client, response, and exception types to `Inttegro`.
- Aligned package metadata, examples, and the transport user agent with the public Inttegro service name.
