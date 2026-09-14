using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class PayoutsResource
{
    private readonly ApiClient _client;

    internal PayoutsResource(ApiClient client) => _client = client;

    public Task<PayoutSettingsMutation> SetDestinationsAsync(PayoutDestinations destinations, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutSettingsMutation>("/payouts/set_destinations", "settings", new { destinations }, cancellationToken);

    public Task<PayoutSettingsMutation> SetDestinationsAsync(PayoutSetDestinationsRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.Destinations, "destinations");
        return _client.PostResourceAsync<PayoutSettingsMutation>("/payouts/set_destinations", "settings", payload, cancellationToken);
    }

    public Task<PayoutSettingsLookup> SettingsAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutSettingsLookup>("/payouts/settings", "settings", new { }, cancellationToken);

    public Task<PayoutSettingsMutation> DisableAutomaticAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutSettingsMutation>("/payouts/disable", "settings", new { }, cancellationToken);

    public Task<PayoutSettingsMutation> EnableAutomaticAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutSettingsMutation>("/payouts/enable", "settings", new { }, cancellationToken);

    public Task<PayoutSettingsMutation> EnableFXAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutSettingsMutation>("/payouts/enable_fx", "settings", new { }, cancellationToken);

    public Task<PayoutSettingsMutation> DisableFXAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutSettingsMutation>("/payouts/disable_fx", "settings", new { }, cancellationToken);

    public Task<PayoutPage> PageAsync(PayoutPageRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PayoutPage>("/payouts/page", "page", payload, cancellationToken);

    public Task<Payout> LookupAsync(string payoutId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Payout>("/payouts/lookup", "payout", new { payout_id = payoutId }, cancellationToken);

    public Task<Payout> ScheduleAsync(SchedulePayoutRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.DestinationId, "destination_id");
        RequestValidator.Require(payload.Reference, "reference");
        return _client.PostResourceAsync<Payout>("/payouts/schedule", "payout", payload, cancellationToken);
    }

    public Task<Payout> CancelAsync(string payoutId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Payout>("/payouts/cancel", "payout", new { payout_id = payoutId }, cancellationToken);

    public Task<Payout> CancelAsync(CancelPayoutRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PayoutId, "payout_id");
        return _client.PostResourceAsync<Payout>("/payouts/cancel", "payout", payload, cancellationToken);
    }
}
