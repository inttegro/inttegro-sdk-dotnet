using System.Text.Json.Serialization;

namespace Inttegro;

/// <summary>Controls one API request without changing the operation's domain payload.</summary>
public sealed class RequestMeta
{
    /// <summary>Stable key used to retry an idempotent mutation safely.</summary>
    [JsonPropertyName("idempotency_key")]
    public string? IdempotencyKey { get; set; }

    /// <summary>Preferences for computing and representing the successful response.</summary>
    [JsonPropertyName("response")]
    public RequestResponse? Response { get; set; }
}

/// <summary>Caller-controlled preferences for a successful response.</summary>
public sealed class RequestResponse
{
    /// <summary>Public property selections keyed by endpoint-defined response resource name.</summary>
    [JsonPropertyName("projection")]
    public ResponseProjection? Projection { get; set; }
}

/// <summary>Public property selections keyed by response resource name.</summary>
public sealed class ResponseProjection : Dictionary<string, PropertySelection>
{
    /// <summary>Adds or replaces a selection and returns this projection for fluent construction.</summary>
    public ResponseProjection Select(string resource, PropertySelection selection)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Response resource name must not be blank.", nameof(resource));
        }
        ArgumentNullException.ThrowIfNull(selection);
        this[resource] = selection;
        return this;
    }
}

/// <summary>A non-empty public-property selection for one response resource.</summary>
public sealed class PropertySelection
{
    private PropertySelection(IReadOnlyList<string>? picked, IReadOnlyList<string>? omitted)
    {
        Picked = picked;
        Omitted = omitted;
    }

    /// <summary>Properties to include, together with the ancestors needed to contain them.</summary>
    [JsonPropertyName("pick")]
    public IReadOnlyList<string>? Picked { get; }

    /// <summary>Properties to exclude, including all of their descendants.</summary>
    [JsonPropertyName("omit")]
    public IReadOnlyList<string>? Omitted { get; }

    /// <summary>Includes only the supplied public property paths.</summary>
    public static PropertySelection Pick(params string[] propertyPaths) =>
        new(Validate("pick", propertyPaths), null);

    /// <summary>Excludes the supplied public property paths.</summary>
    public static PropertySelection Omit(params string[] propertyPaths) =>
        new(null, Validate("omit", propertyPaths));

    private static IReadOnlyList<string> Validate(string mode, string[] propertyPaths)
    {
        ArgumentNullException.ThrowIfNull(propertyPaths);
        if (propertyPaths.Length == 0 || propertyPaths.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException($"{mode} must contain non-blank property paths.", nameof(propertyPaths));
        }
        return Array.AsReadOnly((string[])propertyPaths.Clone());
    }
}
