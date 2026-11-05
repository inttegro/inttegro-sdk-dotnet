using System.Text.Json;
using Xunit;

namespace Inttegro.Tests;

public class PolicyTests
{
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
              "capabilities":{"payouts":{"enabled":false,"bt_aging_spec":{"public_id":"bt_aging_spec_immediate","t_plus":"0h"}}},
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

        Assert.Equal("0h", policy.Capabilities.Payouts!.BTAgingSpec!.TPlus);
        Assert.Equal("op_123", app.OperatingPolicy!.Source.Id);
        Assert.Equal("ghs", Assert.Single(app.OperatingPolicy.Capabilities.Payments!.Currencies!));
    }
}
