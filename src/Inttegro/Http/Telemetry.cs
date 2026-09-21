using System.Diagnostics;
using Inttegro.Diagnostics;
using Inttegro.Errors;

namespace Inttegro.Http;

internal sealed class Telemetry
{
    private static readonly HashSet<string> SafeResources = new(StringComparer.Ordinal)
    {
        "apps", "balance_transactions", "balances", "broadcasts", "checkout", "chimes", "customers",
        "file_links", "file_references", "files", "financial_accounts", "keys", "message_templates",
        "orders", "otp", "payment_methods", "payouts", "ping", "prices", "products", "purchase_intents",
        "refunds", "schedules", "sessions", "spec", "upload_requests"
    };
    private static readonly HashSet<string> SafeActions = new(StringComparer.Ordinal)
    {
        "activate", "add_price", "archive", "broadcast", "cancel", "complete", "confirm_payment",
        "confirm_verification", "connect", "contents", "countries", "create", "deactivate", "delete",
        "destroy", "disable", "disable_fx", "disable_pull", "disable_push", "disactivate", "disconnect",
        "enable", "enable_fx", "enable_pull", "enable_push", "finalize", "generate", "initiate", "lookup",
        "new", "open", "page", "pay", "publish", "reconcile", "reconnect", "refund", "render_preview",
        "request_confirmation", "review", "revoke", "schedule", "search", "send", "send_invoice", "send_receipt",
        "set_default_unit_price", "set_destinations", "settings", "tokenize", "unarchive", "unpublish",
        "update", "upload", "usage", "verify"
    };
    private readonly Uri _baseUri;
    private readonly bool _enabled;
    private readonly ErrorReporter? _errorReporter;
    private readonly ErrorReportingPolicy _errorReportingPolicy;

    internal Telemetry(Uri baseUri, bool enabled)
        : this(baseUri, enabled, null, ErrorReportingPolicy.Unexpected)
    {
    }

    internal Telemetry(
        Uri baseUri,
        bool enabled,
        ErrorReporter? errorReporter,
        ErrorReportingPolicy errorReportingPolicy
    )
    {
        _baseUri = baseUri;
        _enabled = enabled;
        _errorReporter = errorReporter;
        _errorReportingPolicy = errorReportingPolicy;
    }

    internal Request Start(HttpMethod method, Uri? requestUri, string? explicitOperation = null)
    {
        if (requestUri == null || (!_enabled && _errorReporter == null))
        {
            return new Request(null);
        }

        var isRelativePath = !requestUri.IsAbsoluteUri;
        var route = isRelativePath && IsKnownRoute(requestUri.OriginalString) ? requestUri.OriginalString : null;
        var operation = string.IsNullOrWhiteSpace(explicitOperation)
            ? (route == null ? "http.request" : OperationFromRoute(route))
            : explicitOperation;
        var serverAddress = isRelativePath ? _baseUri.Host : requestUri.Host;
        Activity? activity = null;
        if (_enabled)
        {
            activity = InttegroClient.ActivitySource.StartActivity(
                $"inttegro.{operation}",
                ActivityKind.Client
            );
        }
        if (activity != null)
        {
            activity.SetTag("inttegro.operation.name", operation);
            activity.SetTag("inttegro.sdk.language", "dotnet");
            activity.SetTag("inttegro.sdk.version", InttegroClient.Version);
            activity.SetTag("http.request.method", method.Method.ToUpperInvariant());
            activity.SetTag("server.address", serverAddress);
            if (route != null)
            {
                activity.SetTag("url.template", route);
            }
            activity.AddEvent(new ActivityEvent("inttegro.request.prepared"));
        }
        return _errorReporter == null
            ? new Request(activity)
            : new Request(
                activity,
                _errorReporter,
                _errorReportingPolicy,
                operation,
                route,
                serverAddress,
                method.Method
            );
    }

    private static string OperationFromRoute(string route)
    {
        var parts = route.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            return "http.request";
        }
        if (parts.Length == 1)
        {
            return parts[0] == "balances" ? "balances.lookup" : $"{parts[0]}.request";
        }
        return $"{parts[0]}.{parts[^1]}";
    }

    private static bool IsKnownRoute(string route)
    {
        var parts = route.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0
            && parts.Length <= 2
            && SafeResources.Contains(parts[0])
            && (parts.Length == 1 || SafeActions.Contains(parts[1]));
    }

    internal sealed class Request : IDisposable
    {
        private readonly Activity? _activity;
        private readonly ErrorReporter? _errorReporter;
        private readonly ErrorReportingPolicy _errorReportingPolicy;
        private readonly string? _operation;
        private readonly string? _route;
        private readonly string? _serverAddress;
        private readonly string? _method;
        private readonly long _startedAt;

        internal Request(Activity? activity)
            : this(activity, null, ErrorReportingPolicy.Unexpected, null, null, null, null)
        {
        }

        internal Request(
            Activity? activity,
            ErrorReporter? errorReporter,
            ErrorReportingPolicy errorReportingPolicy,
            string? operation,
            string? route,
            string? serverAddress,
            string? method
        )
        {
            _activity = activity;
            _errorReporter = errorReporter;
            _errorReportingPolicy = errorReportingPolicy;
            _operation = operation;
            _route = route;
            _serverAddress = serverAddress;
            _method = method;
            _startedAt = errorReporter == null ? 0 : Stopwatch.GetTimestamp();
        }

        internal void Inject(HttpRequestMessage request)
        {
            if (_activity == null)
            {
                return;
            }
            DistributedContextPropagator.Current.Inject(
                _activity,
                request,
                static (carrier, name, value) =>
                {
                    if (carrier is HttpRequestMessage message && !message.Headers.Contains(name))
                    {
                        message.Headers.TryAddWithoutValidation(name, value);
                    }
                }
            );
        }

        internal void Attempt()
        {
            _activity?.AddEvent(new ActivityEvent(
                "inttegro.http.attempt.started",
                tags: new ActivityTagsCollection { ["http.request.resend_count"] = 0 }
            ));
        }

        internal void Response(HttpResponseMessage response)
        {
            if (_activity == null)
            {
                return;
            }
            _activity.SetTag("http.response.status_code", (int)response.StatusCode);
            if (response.Headers.TryGetValues("x-request-id", out var values))
            {
                var requestId = values.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(requestId))
                {
                    _activity.SetTag("inttegro.request.id", requestId);
                }
            }
            _activity.AddEvent(new ActivityEvent("inttegro.response.received"));
        }

        internal void Decoded() => _activity?.AddEvent(new ActivityEvent("inttegro.response.decoded"));

        internal void Fail(string errorType)
        {
            if (_activity == null || string.IsNullOrWhiteSpace(errorType) || errorType == "canceled")
            {
                return;
            }
            _activity.SetTag("error.type", errorType);
            _activity.SetStatus(ActivityStatusCode.Error, errorType);
            _activity.AddEvent(new ActivityEvent(
                "inttegro.request.failed",
                tags: new ActivityTagsCollection { ["error.type"] = errorType }
            ));
        }

        internal void Fail(Exception error, string errorType)
        {
            Fail(errorType);
            var reporter = _errorReporter;
            if (reporter == null || errorType == "canceled")
            {
                return;
            }
            try
            {
                var apiException = error as InttegroApiException;
                if (_errorReportingPolicy == ErrorReportingPolicy.Unexpected
                    && apiException != null
                    && apiException.StatusCode < 500
                    && apiException.Type != "unknown_error")
                {
                    return;
                }

                var statusCode = apiException?.StatusCode;
                var apiContext = apiException != null
                    && (apiException.Type != null || apiException.Code != null || apiException.FixCode != null)
                    ? new ApiErrorReportContext(apiException.Type, apiException.Code, apiException.FixCode)
                    : null;
                var traceContext = _activity != null && _activity.Context != default
                    ? new TraceReportContext(_activity.TraceId.ToHexString(), _activity.SpanId.ToHexString())
                    : null;
                var duration = Stopwatch.GetElapsedTime(_startedAt);
                var report = new ErrorReport(
                    1,
                    Guid.NewGuid().ToString(),
                    DateTimeOffset.UtcNow,
                    "error",
                    errorType,
                    _operation ?? "http.request",
                    new SdkReportContext("dotnet", InttegroClient.Version),
                    new HttpReportContext(
                        _method?.ToUpperInvariant() ?? "UNKNOWN",
                        _route,
                        _serverAddress ?? "unknown",
                        statusCode,
                        apiException?.RequestId,
                        Math.Max(0, (long)duration.TotalMilliseconds)
                    ),
                    apiContext,
                    traceContext,
                    error.GetType().Name,
                    string.Join(":", new[]
                    {
                        "inttegro",
                        "dotnet",
                        _operation ?? "http.request",
                        errorType,
                        statusCode?.ToString() ?? "none"
                    })
                );
                if (error is InttegroException inttegroException)
                {
                    inttegroException.Report = report;
                }
                reporter(report);
            }
            catch
            {
                // Report preparation and delivery must never replace the original failure.
            }
        }

        public void Dispose() => _activity?.Dispose();
    }
}
