using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

public sealed class CreateCustomerRequest
{
    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; set; }

    [JsonPropertyName("request_meta")]
    public RequestMeta? RequestMeta { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }

    [JsonPropertyName("shipping_address")]
    public Address? ShippingAddress { get; set; }
}

public sealed class LookupCustomerRequest
{
    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; set; }
}

public sealed class UpdateCustomerRequest
{
    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomDataInput? CustomData { get; set; }

    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("shipping_address")]
    public Address? ShippingAddress { get; set; }

    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

public sealed class PageCustomersRequest
{
    [JsonPropertyName("page_number")]
    public int? PageNumber { get; set; }

    [JsonPropertyName("page_size")]
    public int? PageSize { get; set; }
}

public sealed class Customer
{
    [JsonPropertyName("balance")]
    public CustomerBalance? Balance { get; set; }

    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("suffix")]
    public string? Suffix { get; set; }

    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    [JsonPropertyName("email_address")]
    public string? EmailAddress { get; set; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }

    [JsonPropertyName("custom_data")]
    public CustomData? CustomData { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("guest")]
    public bool Guest { get; set; }

    [JsonPropertyName("shipping_address")]
    public Address? ShippingAddress { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }
}

[JsonConverter(typeof(CustomerBalanceJsonConverter))]
public sealed class CustomerBalance : IReadOnlyDictionary<string, CustomerBalanceValue>
{
    private readonly Dictionary<string, CustomerBalanceValue> _values = new();
    internal IDictionary<string, CustomerBalanceValue> MutableValues => _values;
    public CustomerBalanceValue this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<CustomerBalanceValue> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out CustomerBalanceValue value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<string, CustomerBalanceValue>> GetEnumerator() => _values.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}

public sealed class CustomerBalanceValue
{
    [JsonPropertyName("as_of")]
    public DateTimeOffset? AsOf { get; set; }

    [JsonPropertyName("available")]
    public Amount? Available { get; set; }
}

public sealed class CustomersPage
{
    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("size")]
    public int? Size { get; set; }

    [JsonPropertyName("customers")]
    public List<Customer>? Customers { get; set; }
}
