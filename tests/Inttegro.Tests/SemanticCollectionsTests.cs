using System.Text.Json;
using Xunit;

namespace Inttegro.Tests;

public sealed class SemanticCollectionsTests
{
    [Fact]
    public void PayoutModelsDecodeCanonicalTypedFields()
    {
        var payout = JsonSerializer.Deserialize<Payout>(
            """
            {
              "id":"po_123",
              "destination_id":"fa_ghs",
              "execute_after":"2026-09-14T09:00:00Z",
              "initiated_at":"2026-09-14T08:55:00Z",
              "max_amount":{"currency":"ghs","value":12500},
              "status":"invalid",
              "balance_transactions":["bt_123"],
              "custom_data":{"batch":"weekly"},
              "error":{"cause":"provider unavailable","message":"Payout failed","occurred_at":"2026-09-14T09:05:00Z","type":"network_error"},
              "failed_at":"2026-09-14T09:05:00Z"
            }
            """);

        Assert.Equal(PayoutStatus.Invalid, payout!.Status);
        Assert.Equal("bt_123", Assert.Single(payout.BalanceTransactions!));
        Assert.Equal("weekly", payout.CustomData!["batch"]);
        Assert.Equal("network_error", payout.Error!.Type);
        Assert.Equal(DateTimeOffset.Parse("2026-09-14T09:05:00Z"), payout.FailedAt);

        var settings = JsonSerializer.Deserialize<PayoutSettingsLookup>(
            """
            {"destinations":{"ghs":"fa_ghs"},"schedule":{"aging_spec":{"abide":"strict","label":"Seven days","t_plus":"168h"},"description":"Weekly payouts","interval":"weekly","name":"Weekly","schedule_on":"monday","type":"automatic"}}
            """);
        Assert.Equal("fa_ghs", settings!.Destinations!.Ghs);
        Assert.Equal("168h", settings.Schedule!.AgingSpec!.TPlus);
    }

    [Fact]
    public void CustomDataControlsMutationAndRollsBackInvalidChanges()
    {
        var data = new CustomData().Set("order", "first");

        Assert.Throws<ArgumentException>(() => data.Set(new string('x', 257), "invalid"));
        Assert.Equal("first", data["order"]);
        Assert.Single(data);
        Assert.Equal("{\"order\":\"first\"}", JsonSerializer.Serialize(data));
    }

    [Fact]
    public void CustomDataPatchDistinguishesSetFromUnset()
    {
        var patch = new CustomDataPatch()
            .Set("campaign", "winter")
            .Unset("legacy");

        Assert.Equal(
            "{\"campaign\":\"winter\",\"legacy\":null}",
            JsonSerializer.Serialize(patch));
    }

    [Fact]
    public void SemanticCollectionsRoundTripWithoutExposingMutableDictionaries()
    {
        var metadata = new FileMetadata().Set("source", "invoice");
        var destinations = new PayoutDestinations { Ghs = "fa_example" };

        var decodedMetadata = JsonSerializer.Deserialize<FileMetadata>(JsonSerializer.Serialize(metadata));
        var decodedDestinations = JsonSerializer.Deserialize<PayoutDestinations>(JsonSerializer.Serialize(destinations));

        Assert.Equal("invoice", decodedMetadata!["source"]);
        Assert.Equal("fa_example", decodedDestinations!.Ghs);
        Assert.IsAssignableFrom<IReadOnlyDictionary<string, string>>(decodedMetadata);
    }

    [Fact]
    public void StructuredInputAndCustomerBalanceKeepTheirDomainTypes()
    {
        var input = new CustomDataInput()
            .Set("campaign", "launch")
            .Set("attribution", new { channel = "partner" });
        var customer = JsonSerializer.Deserialize<Customer>(
            """
            {
              "id":"cu_example",
              "name":"Ada",
              "created_at":"2026-01-01T00:00:00Z",
              "guest":false,
              "balance":{"ghs":{"as_of":"2026-01-01T00:00:00Z","available":{"currency":"ghs","value":2500}}}
            }
            """);

        Assert.Equal("partner", input.Get<Dictionary<string, string>>("attribution")!["channel"]);
        Assert.Equal(2500, customer!.Balance!["ghs"].Available!.Value);
    }
}
