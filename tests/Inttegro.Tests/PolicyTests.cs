using System.Text.Json;
using Xunit;

namespace Inttegro.Tests;

public class PolicyTests
{
    private static readonly JsonSerializerOptions RequestJsonOptions = new()
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void AppPolicySelectionSerializesReferenceOrInlineCapabilities()
    {
        var referenced = new CreateAppRequest
        {
            Name = "Child",
            OperatingPolicy = new OperatingPolicySelection { Id = "op_123" }
        };
        var inline = new UpdateAppRequest
        {
            OperatingPolicy = new OperatingPolicySelection
            {
                Capabilities = new OperatingPolicyCapabilitiesInput
                {
                    Payouts = new OperatingPolicyPayoutsCapabilitiesInput
                    {
                        Enabled = false,
                        BTAgingSpec = new PolicyBTAgingSpecReference { Id = "bt_aging_spec_immediate" }
                    }
                }
            }
        };

        using var referencedJson = JsonDocument.Parse(JsonSerializer.Serialize(referenced));
        using var inlineJson = JsonDocument.Parse(JsonSerializer.Serialize(inline));

        Assert.Equal("op_123", referencedJson.RootElement.GetProperty("operating_policy").GetProperty("id").GetString());
        Assert.Equal(
            "bt_aging_spec_immediate",
            inlineJson.RootElement
                .GetProperty("operating_policy")
                .GetProperty("capabilities")
                .GetProperty("payouts")
                .GetProperty("bt_aging_spec")
                .GetProperty("id")
                .GetString()
        );
    }

    [Fact]
    public void PolicyResponsesDeserializeTypedCapabilitiesAndAppProjection()
    {
        const string policyJson = """
            {
              "id":"op_123",
              "version":2,
              "active_version":2,
              "name":"Retail",
              "capabilities":{"payouts":{"enabled":false,"bt_aging_spec":{"id":"bt_aging_spec_immediate","t_plus":"0h"}}},
              "children_policy_id":"cp_123",
              "children_policy_version":1,
              "created_at":"2026-11-05T10:00:00Z",
              "updated_at":"2026-11-05T11:00:00Z",
              "version_created_at":"2026-11-05T11:00:00Z"
            }
            """;
        const string appJson = """
            {
              "id":"app_123",
              "name":"Child",
              "operating_policy":{
                "capabilities":{"payments":{"currencies":["ghs"]}},
                "source":{"type":"template","id":"op_123","version":2}
              }
            }
            """;

        var policy = JsonSerializer.Deserialize<OperatingPolicy>(policyJson)!;
        var app = JsonSerializer.Deserialize<App>(appJson)!;

        Assert.Equal("bt_aging_spec_immediate", policy.Capabilities.Payouts!.BTAgingSpec!.Id);
        Assert.Equal("0h", policy.Capabilities.Payouts.BTAgingSpec.TPlus);
        Assert.Equal("op_123", app.OperatingPolicy!.Source.Id);
        Assert.Equal("ghs", Assert.Single(app.OperatingPolicy.Capabilities.Payments!.Currencies!));
    }

    [Fact]
    public void PolicyLookupRequestsOmitOrSerializeExactVersion()
    {
        Assert.Equal("{}", JsonSerializer.Serialize(new LookupChildrenPolicyRequest(), RequestJsonOptions));
        Assert.Equal(
            "{\"version\":2}",
            JsonSerializer.Serialize(new LookupChildrenPolicyRequest { Version = 2 }, RequestJsonOptions)
        );
        Assert.Equal(
            "{\"operating_policy_id\":\"op_123\"}",
            JsonSerializer.Serialize(
                new LookupOperatingPolicyRequest { OperatingPolicyId = "op_123" },
                RequestJsonOptions
            )
        );
        Assert.Equal(
            "{\"operating_policy_id\":\"op_123\",\"version\":3}",
            JsonSerializer.Serialize(
                new LookupOperatingPolicyRequest { OperatingPolicyId = "op_123", Version = 3 },
                RequestJsonOptions
            )
        );
    }

    [Fact]
    public void PolicyPageRequestsAndResponsesUseVersionHistoryShape()
    {
        Assert.Equal(
            "{\"number\":1,\"size\":10}",
            JsonSerializer.Serialize(new PageChildrenPoliciesRequest { Number = 1, Size = 10 }, RequestJsonOptions)
        );
        Assert.Equal(
            "{\"operating_policy_id\":\"op_123\",\"number\":0}",
            JsonSerializer.Serialize(
                new PageOperatingPoliciesRequest { OperatingPolicyId = "op_123", Number = 0 },
                RequestJsonOptions
            )
        );

        const string childrenPageJson = """
            {"number":0,"size":20,"count":1,"total":2,"has_more":true,"versions":[{
              "id":"cp_123","version":2,"active_version":2,"name":"Default","capabilities":{},
              "created_at":"2026-11-05T10:00:00Z","updated_at":"2026-11-05T11:00:00Z","version_created_at":"2026-11-05T11:00:00Z"
            }]}
            """;
        const string operatingPageJson = """
            {"number":0,"size":20,"count":1,"total":1,"has_more":false,"versions":[{
              "id":"op_123","version":1,"active_version":1,"name":"Retail","capabilities":{},
              "children_policy_id":"cp_123","children_policy_version":1,
              "created_at":"2026-11-05T10:00:00Z","updated_at":"2026-11-05T10:00:00Z","version_created_at":"2026-11-05T10:00:00Z"
            }]}
            """;

        var childrenPage = JsonSerializer.Deserialize<ChildrenPolicyPage>(childrenPageJson)!;
        var operatingPage = JsonSerializer.Deserialize<OperatingPolicyPage>(operatingPageJson)!;
        Assert.Equal(2, Assert.Single(childrenPage.Versions).Version);
        Assert.True(childrenPage.HasMore);
        Assert.Equal("op_123", Assert.Single(operatingPage.Versions).Id);
        Assert.False(operatingPage.HasMore);
    }
}
