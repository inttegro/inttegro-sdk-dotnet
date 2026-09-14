using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Inttegro;

internal abstract class SemanticDictionaryJsonConverter<TCollection, TValue> : JsonConverter<TCollection>
    where TCollection : class, new()
{
    protected abstract IDictionary<string, TValue> Values(TCollection collection);
    protected virtual void Add(TCollection collection, string key, TValue value) => Values(collection)[key] = value;

    public override TCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var values = JsonSerializer.Deserialize<Dictionary<string, TValue>>(ref reader, options)
            ?? new Dictionary<string, TValue>();
        var collection = new TCollection();
        foreach (var pair in values) Add(collection, pair.Key, pair.Value);
        return collection;
    }

    public override void Write(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, Values(value), options);
}

[JsonConverter(typeof(CustomDataJsonConverter))]
public sealed class CustomData : IReadOnlyDictionary<string, string>
{
    public const int MaxKeyBytes = 256;
    public const int MaxEncodedBytes = 25 * 1024;
    private readonly Dictionary<string, string> _values = new();
    internal IDictionary<string, string> MutableValues => _values;

    public CustomData() { }
    public CustomData(IEnumerable<KeyValuePair<string, string>> values)
    {
        foreach (var pair in values) Set(pair.Key, pair.Value);
    }

    public CustomData Set(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        ValidateKey(key);
        var existed = _values.TryGetValue(key, out var previous);
        _values[key] = value;
        try { ValidateSize(_values); }
        catch { if (existed) _values[key] = previous!; else _values.Remove(key); throw; }
        return this;
    }

    public bool Remove(string key) => _values.Remove(key);
    public string this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<string> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal static void ValidateKey(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (Encoding.UTF8.GetByteCount(key) > MaxKeyBytes) throw new ArgumentException("Custom data key exceeds 256 bytes.", nameof(key));
    }

    internal static void ValidateSize<T>(IDictionary<string, T> values)
    {
        if (JsonSerializer.SerializeToUtf8Bytes(values).Length > MaxEncodedBytes) throw new ArgumentException("Custom data exceeds 25 KiB.");
    }
}

internal sealed class CustomDataJsonConverter : SemanticDictionaryJsonConverter<CustomData, string>
{
    protected override IDictionary<string, string> Values(CustomData collection) => collection.MutableValues;
    protected override void Add(CustomData collection, string key, string value) => collection.Set(key, value);
}

[JsonConverter(typeof(CustomDataInputJsonConverter))]
public sealed class CustomDataInput : IReadOnlyDictionary<string, JsonElement>
{
    private readonly Dictionary<string, JsonElement> _values = new();
    internal IDictionary<string, JsonElement> MutableValues => _values;
    public CustomDataInput Set<T>(string key, T value)
    {
        CustomData.ValidateKey(key);
        var existed = _values.TryGetValue(key, out var previous);
        _values[key] = JsonSerializer.SerializeToElement(value);
        try { CustomData.ValidateSize(_values); }
        catch { if (existed) _values[key] = previous; else _values.Remove(key); throw; }
        return this;
    }
    internal CustomDataInput SetElement(string key, JsonElement value)
    {
        CustomData.ValidateKey(key);
        var existed = _values.TryGetValue(key, out var previous);
        _values[key] = value.Clone();
        try { CustomData.ValidateSize(_values); }
        catch { if (existed) _values[key] = previous; else _values.Remove(key); throw; }
        return this;
    }
    public bool Remove(string key) => _values.Remove(key);
    public T? Get<T>(string key) => _values.TryGetValue(key, out var value) ? value.Deserialize<T>() : default;
    public JsonElement this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<JsonElement> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out JsonElement value) => _values.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<string, JsonElement>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class CustomDataInputJsonConverter : SemanticDictionaryJsonConverter<CustomDataInput, JsonElement>
{
    protected override IDictionary<string, JsonElement> Values(CustomDataInput collection) => collection.MutableValues;
    protected override void Add(CustomDataInput collection, string key, JsonElement value) => collection.SetElement(key, value);
}

[JsonConverter(typeof(CustomDataPatchJsonConverter))]
public sealed class CustomDataPatch : IReadOnlyDictionary<string, JsonElement?>
{
    private readonly Dictionary<string, JsonElement?> _changes = new();
    internal IDictionary<string, JsonElement?> MutableValues => _changes;
    public CustomDataPatch Set<T>(string key, T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Change(key, JsonSerializer.SerializeToElement(value));
    }
    public CustomDataPatch Unset(string key) => Change(key, null);
    internal CustomDataPatch Change(string key, JsonElement? value)
    {
        CustomData.ValidateKey(key);
        var existed = _changes.TryGetValue(key, out var previous);
        _changes[key] = value;
        try { CustomData.ValidateSize(_changes); }
        catch { if (existed) _changes[key] = previous; else _changes.Remove(key); throw; }
        return this;
    }
    public bool RemoveChange(string key) => _changes.Remove(key);
    public JsonElement? this[string key] => _changes[key];
    public IEnumerable<string> Keys => _changes.Keys;
    public IEnumerable<JsonElement?> Values => _changes.Values;
    public int Count => _changes.Count;
    public bool ContainsKey(string key) => _changes.ContainsKey(key);
    public bool TryGetValue(string key, out JsonElement? value) => _changes.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<string, JsonElement?>> GetEnumerator() => _changes.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class CustomDataPatchJsonConverter : SemanticDictionaryJsonConverter<CustomDataPatch, JsonElement?>
{
    protected override IDictionary<string, JsonElement?> Values(CustomDataPatch collection) => collection.MutableValues;
    protected override void Add(CustomDataPatch collection, string key, JsonElement? value) => collection.Change(key, value?.Clone());
}

[JsonConverter(typeof(JsonDataJsonConverter))]
public sealed class JsonData : IReadOnlyDictionary<string, JsonElement>
{
    private readonly Dictionary<string, JsonElement> _values = new();
    internal IDictionary<string, JsonElement> MutableValues => _values;
    public JsonData Set<T>(string key, T value) { _values[key] = JsonSerializer.SerializeToElement(value); return this; }
    public bool Remove(string key) => _values.Remove(key);
    public T? Get<T>(string key) => _values.TryGetValue(key, out var value) ? value.Deserialize<T>() : default;
    public JsonElement this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<JsonElement> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out JsonElement value) => _values.TryGetValue(key, out value);
    public IEnumerator<KeyValuePair<string, JsonElement>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class JsonDataJsonConverter : SemanticDictionaryJsonConverter<JsonData, JsonElement>
{
    protected override IDictionary<string, JsonElement> Values(JsonData collection) => collection.MutableValues;
}

[JsonConverter(typeof(JsonValueJsonConverter))]
public readonly struct JsonValue
{
    private readonly JsonElement _value;
    internal JsonValue(JsonElement value) => _value = value.Clone();
    public static JsonValue From<T>(T value) => new(JsonSerializer.SerializeToElement(value));
    public T? Get<T>() => _value.Deserialize<T>();
    internal void WriteTo(Utf8JsonWriter writer) => _value.WriteTo(writer);
}

internal sealed class JsonValueJsonConverter : JsonConverter<JsonValue>
{
    public override JsonValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        return new JsonValue(document.RootElement);
    }

    public override void Write(Utf8JsonWriter writer, JsonValue value, JsonSerializerOptions options) => value.WriteTo(writer);
}

[JsonConverter(typeof(MessageHeadersJsonConverter))]
public sealed class MessageHeaders : IReadOnlyDictionary<string, string>
{
    private readonly Dictionary<string, string> _values = new(StringComparer.OrdinalIgnoreCase);
    internal IDictionary<string, string> MutableValues => _values;
    public MessageHeaders Set(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);
        _values[name] = value;
        return this;
    }
    public bool Remove(string name) => _values.Remove(name);
    public string this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<string> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class MessageHeadersJsonConverter : SemanticDictionaryJsonConverter<MessageHeaders, string>
{
    protected override IDictionary<string, string> Values(MessageHeaders collection) => collection.MutableValues;
    protected override void Add(MessageHeaders collection, string key, string value) => collection.Set(key, value);
}

/// <summary>Supported currency-to-financial-account payout assignments.</summary>
public sealed class PayoutDestinations
{
    /// <summary>Financial account that receives Ghana cedi payouts.</summary>
    [JsonPropertyName("ghs")]
    public string? Ghs { get; set; }
}

[JsonConverter(typeof(FileMetadataJsonConverter))]
public sealed class FileMetadata : IReadOnlyDictionary<string, string>
{
    private readonly Dictionary<string, string> _values = new();
    internal IDictionary<string, string> MutableValues => _values;
    public FileMetadata Set(string key, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);
        _values[key] = value;
        return this;
    }
    public bool Remove(string key) => _values.Remove(key);
    public string this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<string> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class FileMetadataJsonConverter : SemanticDictionaryJsonConverter<FileMetadata, string>
{
    protected override IDictionary<string, string> Values(FileMetadata collection) => collection.MutableValues;
    protected override void Add(FileMetadata collection, string key, string value) => collection.Set(key, value);
}

[JsonConverter(typeof(ProductDimensionDetailsJsonConverter))]
public sealed class ProductDimensionDetails : IReadOnlyDictionary<string, string>
{
    private readonly Dictionary<string, string> _values = new();
    internal IDictionary<string, string> MutableValues => _values;
    public ProductDimensionDetails Set(string name, string value) { ArgumentException.ThrowIfNullOrWhiteSpace(name); ArgumentNullException.ThrowIfNull(value); _values[name] = value; return this; }
    public bool Remove(string name) => _values.Remove(name);
    public string this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<string> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out string value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal sealed class ProductDimensionDetailsJsonConverter : SemanticDictionaryJsonConverter<ProductDimensionDetails, string>
{
    protected override IDictionary<string, string> Values(ProductDimensionDetails collection) => collection.MutableValues;
    protected override void Add(ProductDimensionDetails collection, string key, string value) => collection.Set(key, value);
}

internal sealed class CountrySpecificationsJsonConverter : SemanticDictionaryJsonConverter<CountrySpecifications, CountrySpecification>
{
    protected override IDictionary<string, CountrySpecification> Values(CountrySpecifications collection) => collection.MutableValues;
}

internal sealed class CustomerBalanceJsonConverter : SemanticDictionaryJsonConverter<CustomerBalance, CustomerBalanceValue>
{
    protected override IDictionary<string, CustomerBalanceValue> Values(CustomerBalance collection) => collection.MutableValues;
}
