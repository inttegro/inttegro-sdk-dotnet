using System.Text.RegularExpressions;
using Xunit;

namespace Inttegro.Tests;

public class OpenApiCoverageTests
{
    private static readonly Regex PathLiteral = new("\"(?<path>/[a-z0-9_]+(?:/[a-z0-9_]+)*)\"", RegexOptions.Compiled);
    private static readonly string[] CapabilityUrlOperations =
    [
        "/file_links/open",
        "/upload_requests/upload"
    ];
    private static readonly string[] ClientCheckoutOperations =
    [
        "/checkout/lookup",
        "/checkout/select_amount",
        "/checkout/pay",
        "/checkout/request_confirmation",
        "/checkout/confirm_payment"
    ];
    [Fact]
    public void SdkImplementsEveryPublicOpenApiPath()
    {
        var openApiPaths = LoadOpenApiPaths(FindOpenApiSpec());
        var implementedPaths = LoadImplementedPaths(FindSdkRoot());
        var exceptions = CapabilityUrlOperations
            .Concat(ClientCheckoutOperations)
            .ToHashSet(StringComparer.Ordinal);

        var missing = openApiPaths
            .Where(path => !implementedPaths.Contains(path) && !exceptions.Contains(path))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        Assert.True(
            missing.Length == 0,
            "Missing .NET SDK coverage for OpenAPI paths:" + Environment.NewLine + string.Join(Environment.NewLine, missing)
        );
    }

    private static HashSet<string> LoadOpenApiPaths(string specPath)
    {
        var paths = new HashSet<string>(StringComparer.Ordinal);
        var inPaths = false;

        foreach (var line in File.ReadLines(specPath))
        {
            if (!inPaths)
            {
                inPaths = line.TrimEnd() == "paths:";
                continue;
            }

            if (line.Length > 0 && line[0] != ' ' && !line.StartsWith("#", StringComparison.Ordinal))
            {
                break;
            }

            var trimmed = line.TrimStart();
            if (!trimmed.StartsWith("/", StringComparison.Ordinal))
            {
                continue;
            }

            var colon = trimmed.IndexOf(':');
            if (colon > 0)
            {
                paths.Add(trimmed[..colon].Trim('"', '\''));
            }
        }

        Assert.NotEmpty(paths);
        return paths;
    }

    private static HashSet<string> LoadImplementedPaths(string sdkRoot)
    {
        var sourceRoot = Path.Combine(sdkRoot, "src", "Inttegro");
        var paths = new HashSet<string>(StringComparer.Ordinal);

        foreach (var file in Directory.EnumerateFiles(sourceRoot, "*.cs", SearchOption.AllDirectories))
        {
            var source = File.ReadAllText(file);
            foreach (Match match in PathLiteral.Matches(source))
            {
                paths.Add(match.Groups["path"].Value);
            }
        }

        return paths;
    }

    private static string FindOpenApiSpec()
    {
        var overridePath = Environment.GetEnvironmentVariable("INTTEGRO_OPENAPI_SPEC");
        if (!string.IsNullOrWhiteSpace(overridePath))
        {
            return Path.GetFullPath(overridePath);
        }

        var defaultPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../openapi/commerce.yml"));
        if (File.Exists(defaultPath))
        {
            return defaultPath;
        }

        foreach (var start in SearchStarts())
        {
            for (var current = new DirectoryInfo(start); current != null; current = current.Parent)
            {
                var candidate = Path.Combine(current.FullName, "openapi", "commerce.yml");
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        throw new FileNotFoundException("Could not find OpenAPI spec. Set INTTEGRO_OPENAPI_SPEC or run tests from sdks/dotnet.", defaultPath);
    }

    private static string FindSdkRoot()
    {
        foreach (var start in SearchStarts())
        {
            for (var current = new DirectoryInfo(start); current != null; current = current.Parent)
            {
                var candidate = Path.Combine(current.FullName, "src", "Inttegro", "Inttegro.csproj");
                if (File.Exists(candidate))
                {
                    return current.FullName;
                }
            }
        }

        throw new DirectoryNotFoundException("Could not find sdks/dotnet root.");
    }

    private static IEnumerable<string> SearchStarts()
    {
        yield return Directory.GetCurrentDirectory();
        yield return AppContext.BaseDirectory;
    }
}
