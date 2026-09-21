using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Inttegro;
using Inttegro.Errors;
using Inttegro.Diagnostics;

namespace Inttegro.Http;

internal class ApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly Telemetry _telemetry;

    public ApiClient(
        string apiKey,
        string baseUrl,
        TimeSpan timeout,
        HttpClient? httpClient = null,
        bool telemetryEnabled = true,
        ErrorReporter? errorReporter = null,
        ErrorReportingPolicy errorReportingPolicy = ErrorReportingPolicy.Unexpected
    )
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("apiKey is required", nameof(apiKey));
        }

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        if (httpClient != null)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            _httpClient = new HttpClient { Timeout = timeout };
            _ownsHttpClient = true;
        }

        _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/'));
        _telemetry = new Telemetry(_httpClient.BaseAddress, telemetryEnabled, errorReporter, errorReportingPolicy);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd($"inttegro-sdk-dotnet/{InttegroClient.Version}");
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    internal async Task<WireEnvelope> PostAsync(string path, object? payload = null, CancellationToken cancellationToken = default)
    {
        var requestUri = path.StartsWith('/') ? path : "/" + path;
        var json = SerializeRequestPayload(requestUri, payload, generateIdempotencyKey: true);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        return await SendForJsonAsync(request, cancellationToken);
    }

    public async Task<T> PostAsync<T>(string path, object? payload = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var response = await PostAsync(path, payload, cancellationToken);
        return response.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid response for {path}");
    }

    public async Task<T> PostResourceAsync<T>(
        string path,
        string field,
        object? payload = null,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var response = await PostAsync(path, payload, cancellationToken);
        return response[field]?.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid {field} response for {path}");
    }

    public async Task<InttegroResponse<T>> PostResourceWithResponseAsync<T>(
        string path,
        string field,
        object? payload = null,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var response = await PostAsync(path, payload, cancellationToken);
        var data = response[field]?.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid {field} response for {path}");
        return response.ToResponse(data);
    }

    internal async Task<WireEnvelope> PostWithHeadersAsync(
        string path,
        object? payload = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        var requestUri = path.StartsWith('/') ? path : "/" + path;
        var headerMap = headers ?? new Dictionary<string, string>();
        var json = SerializeRequestPayload(requestUri, payload, generateIdempotencyKey: !HasHeader(headerMap, "Idempotency-Key"));
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        foreach (var header in headerMap)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return await SendForJsonAsync(request, cancellationToken);
    }

    internal async Task<T> PostWithHeadersAsync<T>(
        string path,
        object? payload = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var envelope = await PostWithHeadersAsync(path, payload, headers, cancellationToken);
        return envelope.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid value for {path}");
    }

    public async Task<T> PostResourceWithHeadersAsync<T>(
        string path,
        string field,
        object? payload = null,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var response = await PostWithHeadersAsync(path, payload, headers, cancellationToken);
        return response[field]?.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid {field} response for {path}");
    }

    internal async Task<WireEnvelope> PostMultipartAsync(
        string pathOrUrl,
        IDictionary<string, object?> fields,
        IDictionary<string, string> files,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    )
    {
        using var content = new MultipartFormDataContent();
        foreach (var field in fields)
        {
            if (field.Value == null) continue;
            var value = field.Value is string s ? s : JsonSerializer.Serialize(field.Value, _serializerOptions);
            content.Add(new StringContent(value), field.Key);
        }
        foreach (var file in files)
        {
            var stream = File.OpenRead(file.Value);
            var fileContent = new StreamContent(stream);
            content.Add(fileContent, file.Key, Path.GetFileName(file.Value));
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, RequestUri(pathOrUrl))
        {
            Content = content
        };
        var headerMap = headers == null ? new Dictionary<string, string>() : new Dictionary<string, string>(headers);
        if (!IsAbsoluteUrl(pathOrUrl) && IsIdempotentMutationPath(pathOrUrl) && !HasHeader(headerMap, "Idempotency-Key"))
        {
            headerMap["Idempotency-Key"] = GenerateIdempotencyKey();
        }
        foreach (var header in headerMap)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return await SendForJsonAsync(
            request,
            cancellationToken,
            IsAbsoluteUrl(pathOrUrl) ? "upload_requests.upload" : null
        );
    }

    internal async Task<T> PostMultipartAsync<T>(
        string pathOrUrl,
        IDictionary<string, object?> fields,
        IDictionary<string, string> files,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var envelope = await PostMultipartAsync(pathOrUrl, fields, files, headers, cancellationToken);
        return envelope.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid value for {pathOrUrl}");
    }

    internal async Task<T> PostMultipartResourceAsync<T>(
        string pathOrUrl,
        string field,
        IDictionary<string, object?> fields,
        IDictionary<string, string> files,
        IDictionary<string, string>? headers = null,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        var envelope = await PostMultipartAsync(pathOrUrl, fields, files, headers, cancellationToken);
        return envelope[field]?.Deserialize<T>()
            ?? throw new InvalidOperationException($"Inttegro returned an invalid {field} value for {pathOrUrl}");
    }

    public async Task<FileDownload> PostBinaryJsonAsync(string path, object payload, CancellationToken cancellationToken = default)
    {
        var json = SerializeRequestPayload(path, payload, generateIdempotencyKey: false);
        using var request = new HttpRequestMessage(HttpMethod.Post, RequestUri(path))
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
        return await SendForDownloadAsync(request, cancellationToken);
    }

    public async Task<FileDownload> GetBinaryPublicAsync(string url, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        return await SendForDownloadAsync(request, cancellationToken, "file_links.download");
    }

    private Uri RequestUri(string pathOrUrl)
    {
        if (pathOrUrl.StartsWith("http://") || pathOrUrl.StartsWith("https://"))
        {
            return new Uri(pathOrUrl);
        }
        return new Uri(pathOrUrl.StartsWith('/') ? pathOrUrl : "/" + pathOrUrl, UriKind.Relative);
    }

    private string SerializeRequestPayload(string path, object? payload, bool generateIdempotencyKey)
    {
        var json = JsonSerializer.Serialize(payload ?? new { }, _serializerOptions);
        var node = ParseJson(json);
        if (node is not JsonObject obj)
        {
            return json;
        }

        obj.Remove("idempotency_key");
        if (!generateIdempotencyKey || !IsIdempotentMutationPath(path))
        {
            return obj.ToJsonString(_serializerOptions);
        }

        var requestMeta = obj["request_meta"] as JsonObject;
        if (requestMeta == null)
        {
            requestMeta = new JsonObject();
            obj["request_meta"] = requestMeta;
        }

        var existing = requestMeta["idempotency_key"]?.ToString();
        if (string.IsNullOrWhiteSpace(existing))
        {
            requestMeta["idempotency_key"] = GenerateIdempotencyKey();
        }
        return obj.ToJsonString(_serializerOptions);
    }

    private static bool IsIdempotentMutationPath(string pathOrUrl)
    {
        var path = pathOrUrl;
        if (IsAbsoluteUrl(pathOrUrl))
        {
            if (!Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var uri))
            {
                return false;
            }
            path = uri.AbsolutePath;
        }

        if (path.StartsWith("/keys/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var action = path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        return action is not null and not "lookup" and not "page" and not "search" and not "settings" and not "countries" and not "contents" and not "balances" and not "render_preview";
    }

    private static bool IsAbsoluteUrl(string pathOrUrl) =>
        pathOrUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || pathOrUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

    private static bool HasHeader(IDictionary<string, string> headers, string name) =>
        headers.Any(header => string.Equals(header.Key, name, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(header.Value));

    private static string GenerateIdempotencyKey()
    {
        Span<byte> bytes = stackalloc byte[16];
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0xffffffffffffL;
        bytes[0] = (byte)(timestamp >> 40);
        bytes[1] = (byte)(timestamp >> 32);
        bytes[2] = (byte)(timestamp >> 24);
        bytes[3] = (byte)(timestamp >> 16);
        bytes[4] = (byte)(timestamp >> 8);
        bytes[5] = (byte)timestamp;
        RandomNumberGenerator.Fill(bytes[6..]);
        bytes[6] = (byte)((bytes[6] & 0x0f) | 0x70);
        bytes[8] = (byte)((bytes[8] & 0x3f) | 0x80);
        var hex = Convert.ToHexString(bytes).ToLowerInvariant();
        return $"{hex[..8]}-{hex[8..12]}-{hex[12..16]}-{hex[16..20]}-{hex[20..]}";
    }

    private async Task<WireEnvelope> SendForJsonAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken,
        string? explicitOperation = null
    )
    {
        using var telemetryRequest = _telemetry.Start(request.Method, request.RequestUri, explicitOperation);
        telemetryRequest.Inject(request);
        telemetryRequest.Attempt();
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            var error = new InttegroTimeoutException("Request timed out", ex);
            telemetryRequest.Fail(error, "timeout");
            throw error;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            var error = new InttegroNetworkException("Network request failed", ex);
            telemetryRequest.Fail(error, cancellationToken.IsCancellationRequested ? "canceled" : "transport_error");
            throw error;
        }

        telemetryRequest.Response(response);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        var parsed = ParseJson(body);
        if (!response.IsSuccessStatusCode)
        {
            var error = CreateError(response, body, parsed);
            telemetryRequest.Fail(error, $"http_{(int)response.StatusCode}");
            throw error;
        }
        telemetryRequest.Decoded();
        return new WireEnvelope(
            parsed,
            (int)response.StatusCode,
            ResponseHeaders(response),
            ResponseMeta(parsed)
        );
    }

    private static IReadOnlyDictionary<string, string> ResponseHeaders(HttpResponseMessage response)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in response.Headers)
        {
            headers[header.Key] = string.Join(",", header.Value);
        }
        foreach (var header in response.Content.Headers)
        {
            headers[header.Key] = string.Join(",", header.Value);
        }
        return headers;
    }

    private static IReadOnlyDictionary<string, object?>? ResponseMeta(JsonNode? parsed)
    {
        if (parsed is not JsonObject obj || obj["response_meta"] is not JsonObject meta)
        {
            return null;
        }

        var result = new Dictionary<string, object?>();
        foreach (var item in meta)
        {
            result[item.Key] = NodeValue(item.Value);
        }
        return result;
    }

    private static object? NodeValue(JsonNode? node) =>
        node switch
        {
            null => null,
            System.Text.Json.Nodes.JsonValue value => ScalarValue(value),
            JsonArray array => array.Select(NodeValue).ToList(),
            JsonObject obj => obj.ToDictionary(item => item.Key, item => NodeValue(item.Value)),
            _ => node.ToJsonString()
        };

    private static object? ScalarValue(System.Text.Json.Nodes.JsonValue value)
    {
        if (value.TryGetValue<JsonElement>(out var element))
        {
            return ElementValue(element);
        }
        if (value.TryGetValue<string>(out var stringValue))
        {
            return stringValue;
        }
        if (value.TryGetValue<bool>(out var boolValue))
        {
            return boolValue;
        }
        if (value.TryGetValue<long>(out var longValue))
        {
            return longValue;
        }
        if (value.TryGetValue<double>(out var doubleValue))
        {
            return doubleValue;
        }
        return value.ToJsonString();
    }

    private static object? ElementValue(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var longValue) ? longValue : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Array => element.EnumerateArray().Select(ElementValue).ToList(),
            JsonValueKind.Object => element.EnumerateObject().ToDictionary(item => item.Name, item => ElementValue(item.Value)),
            _ => element.ToString()
        };

    private async Task<FileDownload> SendForDownloadAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken,
        string? explicitOperation = null
    )
    {
        using var telemetryRequest = _telemetry.Start(request.Method, request.RequestUri, explicitOperation);
        telemetryRequest.Inject(request);
        telemetryRequest.Attempt();
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request, cancellationToken);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            var error = new InttegroTimeoutException("Request timed out", ex);
            telemetryRequest.Fail(error, "timeout");
            throw error;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            var error = new InttegroNetworkException("Network request failed", ex);
            telemetryRequest.Fail(error, cancellationToken.IsCancellationRequested ? "canceled" : "transport_error");
            throw error;
        }
        telemetryRequest.Response(response);
        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = Encoding.UTF8.GetString(bytes);
            var error = CreateError(response, body, ParseJson(body));
            telemetryRequest.Fail(error, $"http_{(int)response.StatusCode}");
            throw error;
        }
        telemetryRequest.Decoded();
        return new FileDownload(bytes, response.Content.Headers.ContentType?.MediaType);
    }

    private JsonNode? ParseJson(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return null;
        try
        {
            return JsonNode.Parse(body);
        }
        catch
        {
            return null;
        }
    }

    private InttegroApiException CreateError(HttpResponseMessage response, string rawBody, JsonNode? parsed)
    {
        var status = (int)response.StatusCode;
        var message = $"HTTP {status}";
        JsonObject? payload = null;
        if (parsed is JsonObject obj)
        {
            payload = obj["error"] as JsonObject ?? obj;
        }

        if (payload != null)
        {
            message = payload["message"]?.GetValue<string>() ??
                      payload["detail"]?.GetValue<string>() ??
                      (parsed as JsonObject)?["message"]?.GetValue<string>() ??
                      message;
        }

        var code = payload?["code"]?.GetValue<string>();
        var type = payload?["type"]?.GetValue<string>();
        var url = payload?["url"]?.GetValue<string>();
        var detail = payload?["detail"]?.GetValue<string>();
        var fixCode = payload?["fix_code"]?.GetValue<string>();
        var cause = payload?["cause"]?.GetValue<string>();
        var requestId = response.Headers.TryGetValues("x-request-id", out var requestIdValues)
            ? requestIdValues.FirstOrDefault()
            : null;

        if (status == 401)
        {
            return new InttegroAuthenticationException(
                message,
                status,
                code,
                type,
                url,
                detail,
                fixCode,
                cause,
                rawBody,
                parsed,
                requestId
            );
        }

        if (status == 429)
        {
            int? retryAfter = null;
            if (response.Headers.TryGetValues("Retry-After", out var values))
            {
                var value = values.FirstOrDefault();
                if (int.TryParse(value, out var parsedRetry))
                {
                    retryAfter = parsedRetry;
                }
            }

            return new InttegroRateLimitException(
                message,
                status,
                code,
                type,
                url,
                detail,
                fixCode,
                cause,
                rawBody,
                parsed,
                retryAfter,
                requestId
            );
        }

        return new InttegroApiException(
            message,
            status,
            code,
            type,
            url,
            detail,
            fixCode,
            cause,
            rawBody,
            parsed,
            requestId
        );
    }

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }
}
