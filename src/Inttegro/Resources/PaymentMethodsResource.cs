using Inttegro.Http;
using Inttegro;
using Inttegro.Validation;

namespace Inttegro.Resources;

public class PaymentMethodsResource
{
    private readonly ApiClient _client;

    internal PaymentMethodsResource(ApiClient client) => _client = client;

    public Task<PaymentMethod> TokenizeAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PaymentMethod>("/payment_methods/tokenize", "payment_method", payload, cancellationToken);

    public Task<PaymentMethod> TokenizeAsync(PaymentMethodTokenizeRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.CustomerId, "customer_id");
        RequestValidator.Require(payload.PaymentMethodData, "payment_method_data");
        return _client.PostResourceAsync<PaymentMethod>("/payment_methods/tokenize", "payment_method", payload, cancellationToken);
    }

    public Task<PaymentMethodVerificationSession> VerifyAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        VerifyAsync(new PaymentMethodVerifyRequest
        {
            PaymentMethodId = paymentMethodId,
            RequestMeta = StablePaymentMethodRequestMeta("verify", paymentMethodId)
        }, cancellationToken);

    public Task<PaymentMethodVerificationSession> VerifyAsync(PaymentMethodVerifyRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PaymentMethodId, "payment_method_id");
        return _client.PostResourceAsync<PaymentMethodVerificationSession>("/payment_methods/verify", "verification", payload, cancellationToken);
    }

    public Task<PaymentMethod> ConfirmVerificationAsync(object payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PaymentMethod>("/payment_methods/confirm_verification", "payment_method", payload, cancellationToken);

    public Task<PaymentMethod> ConfirmVerificationAsync(PaymentMethodConfirmVerificationRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PaymentMethodId, "payment_method_id");
        RequestValidator.Require(payload.Token, "token");
        return _client.PostResourceAsync<PaymentMethod>("/payment_methods/confirm_verification", "payment_method", payload, cancellationToken);
    }

    public Task<PaymentMethod> LookupAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PaymentMethod>("/payment_methods/lookup", "payment_method", new { payment_method_id = paymentMethodId }, cancellationToken);

    public Task<PaymentMethod> LookupAsync(PaymentMethodLookupRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PaymentMethodId, "payment_method_id");
        return _client.PostResourceAsync<PaymentMethod>("/payment_methods/lookup", "payment_method", payload, cancellationToken);
    }

    public Task<PaymentMethodPage> PageAsync(object? payload = null, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PaymentMethodPage>("/payment_methods/page", "page", payload ?? new { }, cancellationToken);

    public Task<PaymentMethodPage> PageAsync(PaymentMethodPageRequest payload, CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PaymentMethodPage>("/payment_methods/page", "page", payload, cancellationToken);

    public Task<PaymentMethod> UpdateAsync(PaymentMethodUpdateRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PaymentMethodId, "payment_method_id");
        return _client.PostResourceAsync<PaymentMethod>("/payment_methods/update", "payment_method", payload, cancellationToken);
    }

    public Task<PaymentMethod> ActivateAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        ActivateAsync(ActionRequest("activate", paymentMethodId), cancellationToken);

    public Task<PaymentMethod> ActivateAsync(PaymentMethodActionRequest payload, CancellationToken cancellationToken = default) =>
        PostActionAsync("/payment_methods/activate", payload, cancellationToken);

    public Task<PaymentMethod> DisactivateAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        DisactivateAsync(ActionRequest("disactivate", paymentMethodId), cancellationToken);

    public Task<PaymentMethod> DisactivateAsync(PaymentMethodActionRequest payload, CancellationToken cancellationToken = default) =>
        PostActionAsync("/payment_methods/disactivate", payload, cancellationToken);

    public Task<PaymentMethod> ArchiveAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        ArchiveAsync(ActionRequest("archive", paymentMethodId), cancellationToken);

    public Task<PaymentMethod> ArchiveAsync(PaymentMethodActionRequest payload, CancellationToken cancellationToken = default) =>
        PostActionAsync("/payment_methods/archive", payload, cancellationToken);

    public Task<PaymentMethod> UnarchiveAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        UnarchiveAsync(ActionRequest("unarchive", paymentMethodId), cancellationToken);

    public Task<PaymentMethod> UnarchiveAsync(PaymentMethodActionRequest payload, CancellationToken cancellationToken = default) =>
        PostActionAsync("/payment_methods/unarchive", payload, cancellationToken);

    public Task<PaymentMethodDeletion> DeleteAsync(string paymentMethodId, CancellationToken cancellationToken = default) =>
        DeleteAsync(new PaymentMethodDeleteRequest
        {
            PaymentMethodId = paymentMethodId,
            RequestMeta = StablePaymentMethodRequestMeta("delete", paymentMethodId)
        }, cancellationToken);

    public Task<PaymentMethodDeletion> DeleteAsync(PaymentMethodDeleteRequest payload, CancellationToken cancellationToken = default)
    {
        RequestValidator.Require(payload.PaymentMethodId, "payment_method_id");
        return _client.PostAsync<PaymentMethodDeletion>("/payment_methods/delete", payload, cancellationToken);
    }

    public Task<PaymentMethodSettings> SettingsAsync(CancellationToken cancellationToken = default) =>
        _client.PostResourceAsync<PaymentMethodSettings>("/payment_methods/settings", "settings", new { }, cancellationToken);

    private static RequestMeta StablePaymentMethodRequestMeta(string action, string paymentMethodId) =>
        new() { IdempotencyKey = $"payment_methods_{action}_{paymentMethodId}" };

    private Task<PaymentMethod> PostActionAsync(string path, PaymentMethodActionRequest payload, CancellationToken cancellationToken)
    {
        RequestValidator.Require(payload.PaymentMethodId, "payment_method_id");
        return _client.PostResourceAsync<PaymentMethod>(path, "payment_method", payload, cancellationToken);
    }

    private static PaymentMethodActionRequest ActionRequest(string action, string paymentMethodId) =>
        new()
        {
            PaymentMethodId = paymentMethodId,
            RequestMeta = StablePaymentMethodRequestMeta(action, paymentMethodId)
        };
}
