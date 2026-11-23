using Inttegro.Http;

namespace Inttegro.Resources;

public class ChimesResource
{
    private readonly ApiClient _client;

    internal ChimesResource(ApiClient client) => _client = client;

    public Task<Chime> SendAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Chime>("/chimes/send", "chime", payload, cancellationToken);

    public Task<Chime> LookupAsync(string chimeId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Chime>("/chimes/lookup", "chime", new { chime_id = chimeId }, cancellationToken);

    public Task<ScheduledChime> ScheduleAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ScheduledChime>("/chimes/schedule", "scheduled_chime", payload, cancellationToken);

    public Task<Broadcast> BroadcastAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<Broadcast>("/chimes/broadcast", "broadcast", payload, cancellationToken);

    public Task<ChimePage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ChimePage>("/chimes/page", "page", payload ?? new { }, cancellationToken);

    public Task<ChimeSettings> SettingsAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<ChimeSettings>("/chimes/settings", "settings", new { }, cancellationToken);
}
