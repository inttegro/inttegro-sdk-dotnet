using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Inttegro.Money;

namespace Inttegro;

[JsonConverter(typeof(WireEnumJsonConverter<ResourceSearchOperator>))]
public enum ResourceSearchOperator
{
    [EnumMember(Value = "eq")] Equal,
    [EnumMember(Value = "in")] In
}

[JsonConverter(typeof(WireEnumJsonConverter<ResourceSearchSortField>))]
public enum ResourceSearchSortField
{
    [EnumMember(Value = "relevance")] Relevance,
    [EnumMember(Value = "updated_at")] UpdatedAt,
    [EnumMember(Value = "published_at")] PublishedAt
}

[JsonConverter(typeof(WireEnumJsonConverter<ResourceSearchSortDirection>))]
public enum ResourceSearchSortDirection
{
    [EnumMember(Value = "asc")] Ascending,
    [EnumMember(Value = "desc")] Descending
}

[JsonConverter(typeof(WireEnumJsonConverter<ResourceSearchResourceType>))]
public enum ResourceSearchResourceType
{
    [EnumMember(Value = "customer")] Customer,
    [EnumMember(Value = "financial_account")] FinancialAccount,
    [EnumMember(Value = "order")] Order,
    [EnumMember(Value = "payout")] Payout,
    [EnumMember(Value = "product")] Product
}

[JsonConverter(typeof(WireEnumJsonConverter<ResourceSearchTotalRelation>))]
public enum ResourceSearchTotalRelation
{
    [EnumMember(Value = "exact")] Exact,
    [EnumMember(Value = "lower_bound")] LowerBound
}

[JsonConverter(typeof(WireEnumJsonConverter<ResourceSearchFreshnessState>))]
public enum ResourceSearchFreshnessState
{
    [EnumMember(Value = "current")] Current,
    [EnumMember(Value = "delayed")] Delayed,
    [EnumMember(Value = "partial")] Partial,
    [EnumMember(Value = "unknown")] Unknown,
    [EnumMember(Value = "unavailable")] Unavailable
}

public sealed class ResourceSearchFilter
{
    [JsonPropertyName("field")] public required string Field { get; init; }
    [JsonPropertyName("operator")] public required ResourceSearchOperator Operator { get; init; }
    [JsonPropertyName("values")] public required IReadOnlyList<string> Values { get; init; }
}

public sealed class ResourceSearchFacet
{
    [JsonPropertyName("field")] public required string Field { get; init; }
    [JsonPropertyName("limit")] public int? Limit { get; init; }
}

public sealed class ResourceSearchSort
{
    [JsonPropertyName("field")] public required ResourceSearchSortField Field { get; init; }
    [JsonPropertyName("direction")] public required ResourceSearchSortDirection Direction { get; init; }
}

public sealed class ResourceSearchRequest
{
    [JsonPropertyName("text")] public string? Text { get; init; }
    [JsonPropertyName("filters")] public IReadOnlyList<ResourceSearchFilter>? Filters { get; init; }
    [JsonPropertyName("facets")] public IReadOnlyList<ResourceSearchFacet>? Facets { get; init; }
    [JsonPropertyName("sort")] public ResourceSearchSort? Sort { get; init; }
    [JsonPropertyName("page_size")] public int? PageSize { get; init; }
    [JsonPropertyName("cursor")] public string? Cursor { get; init; }
}

public sealed class ResourceSearchTotal
{
    [JsonPropertyName("value")] public long Value { get; init; }
    [JsonPropertyName("relation")] public ResourceSearchTotalRelation Relation { get; init; }
}

public sealed class ResourceSearchResourceTotal
{
    [JsonPropertyName("resource_type")] public ResourceSearchResourceType ResourceType { get; init; }
    [JsonPropertyName("value")] public long Value { get; init; }
    [JsonPropertyName("relation")] public ResourceSearchTotalRelation Relation { get; init; }
}

public sealed class ResourceSearchResourceReference
{
    [JsonPropertyName("type")] public ResourceSearchResourceType Type { get; init; }
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;
}

public sealed class ResourceSearchResult
{
    [JsonPropertyName("resource")] public ResourceSearchResourceReference Resource { get; init; } = new();
    [JsonPropertyName("title")] public string Title { get; init; } = string.Empty;
    [JsonPropertyName("summary")] public string? Summary { get; init; }
    [JsonPropertyName("status")] public string? Status { get; init; }
    [JsonPropertyName("customer_name")] public string? CustomerName { get; init; }
    [JsonPropertyName("amount")] public Amount? Amount { get; init; }
    [JsonPropertyName("url")] public Uri? Url { get; init; }
    [JsonPropertyName("updated_at")] public DateTimeOffset UpdatedAt { get; init; }
}

public sealed class ResourceSearchFacetBucket
{
    [JsonPropertyName("value")] public string Value { get; init; } = string.Empty;
    [JsonPropertyName("count")] public long Count { get; init; }
}

public sealed class ResourceSearchFacetResult
{
    [JsonPropertyName("field")] public string Field { get; init; } = string.Empty;
    [JsonPropertyName("buckets")] public IReadOnlyList<ResourceSearchFacetBucket> Buckets { get; init; } = [];
}

public sealed class ResourceSearchResourceFreshness
{
    [JsonPropertyName("resource_type")] public ResourceSearchResourceType ResourceType { get; init; }
    [JsonPropertyName("state")] public ResourceSearchFreshnessState State { get; init; }
    [JsonPropertyName("observed_at")] public DateTimeOffset? ObservedAt { get; init; }
    [JsonPropertyName("last_indexed_at")] public DateTimeOffset? LastIndexedAt { get; init; }
}

public sealed class ResourceSearchFreshness
{
    [JsonPropertyName("state")] public ResourceSearchFreshnessState State { get; init; }
    [JsonPropertyName("observed_at")] public DateTimeOffset? ObservedAt { get; init; }
    [JsonPropertyName("resources")] public IReadOnlyList<ResourceSearchResourceFreshness>? Resources { get; init; }
}

public sealed class ResourceSearchPage
{
    [JsonPropertyName("resource_types")] public IReadOnlyList<ResourceSearchResourceType> ResourceTypes { get; init; } = [];
    [JsonPropertyName("sort")] public ResourceSearchSort Sort { get; init; } = new() { Field = ResourceSearchSortField.Relevance, Direction = ResourceSearchSortDirection.Descending };
    [JsonPropertyName("page_size")] public int PageSize { get; init; }
    [JsonPropertyName("result_count")] public int ResultCount { get; init; }
    [JsonPropertyName("has_more")] public bool HasMore { get; init; }
    [JsonPropertyName("total")] public ResourceSearchTotal Total { get; init; } = new();
    [JsonPropertyName("resource_totals")] public IReadOnlyList<ResourceSearchResourceTotal> ResourceTotals { get; init; } = [];
    [JsonPropertyName("results")] public IReadOnlyList<ResourceSearchResult> Results { get; init; } = [];
    [JsonPropertyName("facets")] public IReadOnlyList<ResourceSearchFacetResult> Facets { get; init; } = [];
    [JsonPropertyName("next_cursor")] public string? NextCursor { get; init; }
    [JsonPropertyName("freshness")] public ResourceSearchFreshness Freshness { get; init; } = new();
}
