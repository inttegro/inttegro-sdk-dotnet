using System.Diagnostics;
using Inttegro.Http;
using Inttegro.Diagnostics;
using Inttegro.Resources;

namespace Inttegro;

public class InttegroClient : IDisposable
{
    public const string Version = "10.0.0";
    public const string ActivitySourceName = "Inttegro";
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName, Version);

    private readonly ApiClient _apiClient;
    private bool _disposed;

    public OrdersResource Orders { get; }
    public RefundsResource Refunds { get; }
    public PaymentMethodsResource PaymentMethods { get; }
    public PayoutsResource Payouts { get; }
    public BalanceTransactionsResource BalanceTransactions { get; }
    public FinancialAccountsResource FinancialAccounts { get; }
    public FilesResource Files { get; }
    public FileReferencesResource FileReferences { get; }
    public FileLinksResource FileLinks { get; }
    public CustomersResource Customers { get; }
    public ProductsResource Products { get; }
    public PricesResource Prices { get; }
    public ChimesResource Chimes { get; }
    public SchedulesResource Schedules { get; }
    public BroadcastsResource Broadcasts { get; }
    public MessageTemplatesResource MessageTemplates { get; }
    public OtpResource Otp { get; }
    public AppsResource Apps { get; }
    public KeysResource Keys { get; }
    public SpecResource Spec { get; }
    public BalancesResource Balances { get; }
    public UploadRequestsResource UploadRequests { get; }
    public PurchaseIntentsResource PurchaseIntents { get; }

    public InttegroClient(
        string apiKey,
        string? baseUrl = null,
        TimeSpan? timeout = null,
        HttpClient? httpClient = null,
        bool telemetryEnabled = true,
        ErrorReporter? errorReporter = null,
        ErrorReportingPolicy errorReportingPolicy = ErrorReportingPolicy.Unexpected
    )
    {
        var resolvedBaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "https://api.inttegro.com" : baseUrl!;
        var resolvedTimeout = timeout ?? TimeSpan.FromSeconds(30);

        _apiClient = new ApiClient(
            apiKey,
            resolvedBaseUrl,
            resolvedTimeout,
            httpClient,
            telemetryEnabled,
            errorReporter,
            errorReportingPolicy
        );

        Orders = new OrdersResource(_apiClient);
        Refunds = new RefundsResource(_apiClient);
        PaymentMethods = new PaymentMethodsResource(_apiClient);
        Payouts = new PayoutsResource(_apiClient);
        BalanceTransactions = new BalanceTransactionsResource(_apiClient);
        FinancialAccounts = new FinancialAccountsResource(_apiClient);
        Files = new FilesResource(_apiClient);
        FileReferences = new FileReferencesResource(_apiClient);
        FileLinks = new FileLinksResource(_apiClient);
        Customers = new CustomersResource(_apiClient);
        Products = new ProductsResource(_apiClient);
        Prices = new PricesResource(_apiClient);
        Chimes = new ChimesResource(_apiClient);
        Schedules = new SchedulesResource(_apiClient);
        Broadcasts = new BroadcastsResource(_apiClient);
        MessageTemplates = new MessageTemplatesResource(_apiClient);
        Otp = new OtpResource(_apiClient);
        Apps = new AppsResource(_apiClient);
        Keys = new KeysResource(_apiClient);
        Spec = new SpecResource(_apiClient);
        Balances = new BalancesResource(_apiClient);
        UploadRequests = new UploadRequestsResource(_apiClient);
        PurchaseIntents = new PurchaseIntentsResource(_apiClient);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _apiClient.Dispose();
        _disposed = true;
    }
}
