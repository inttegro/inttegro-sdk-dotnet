using System.Text.Json.Serialization;

namespace Inttegro;

public sealed class App
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("alias")] public string? Alias { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("legal_entity_type")] public string? LegalEntityType { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonPropertyName("archived_at")] public DateTimeOffset? ArchivedAt { get; set; }
    [JsonPropertyName("capabilities")] public Dictionary<string, object?> Capabilities { get; set; } = new();
    [JsonPropertyName("secret_key")] public GeneratedSecretKey? SecretKey { get; set; }
    [JsonPropertyName("relationship")] public AppRelationship? Relationship { get; set; }
}

public sealed class AppRelationship
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("kind")] public string? Kind { get; set; }
    [JsonPropertyName("parent")] public AppRelationshipParent? Parent { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
}

public sealed class AppRelationshipParent
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("alias")] public string? Alias { get; set; }
}

public sealed class AppRelationshipPolicy
{
    [JsonPropertyName("child_standing")] public string? ChildStanding { get; set; }
    [JsonPropertyName("management")] public string? Management { get; set; }
    [JsonPropertyName("credentials")] public string? Credentials { get; set; }
}

public sealed class BalanceAmount
{
    [JsonPropertyName("amount")] public long Amount { get; set; }
}

public sealed class BalanceBreakdown
{
    [JsonPropertyName("available")] public BalanceAmount Available { get; set; } = null!;
    [JsonPropertyName("pending")] public BalanceAmount Pending { get; set; } = null!;
    [JsonPropertyName("reserved")] public BalanceAmount Reserved { get; set; } = null!;
    [JsonPropertyName("refund")] public BalanceAmount Refund { get; set; } = null!;
    [JsonPropertyName("includes_transactions_before")] public DateTimeOffset IncludesTransactionsBefore { get; set; }
}

public sealed class BalanceSnapshot
{
    [JsonPropertyName("ghs")] public BalanceBreakdown GHS { get; set; } = null!;
}

public sealed class BalanceTransactionPage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("transactions")] public List<BalanceTransaction>? Transactions { get; set; }
}

public sealed class Broadcast
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("recipients")] public List<string>? Recipients { get; set; }
    [JsonPropertyName("customer_ids")] public List<string>? CustomerIds { get; set; }
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("sender_id")] public string? SenderId { get; set; }
    [JsonPropertyName("purpose")] public string? Purpose { get; set; }
    [JsonPropertyName("send_after")] public DateTimeOffset? SendAfter { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("executed_at")] public DateTimeOffset? ExecutedAt { get; set; }
    [JsonPropertyName("canceled_at")] public DateTimeOffset? CanceledAt { get; set; }
    [JsonPropertyName("chime_ids")] public List<string>? ChimeIds { get; set; }
    [JsonPropertyName("errors")] public List<ApiError>? Errors { get; set; }
}

public sealed class Chime
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("full_message")] public string? FullMessage { get; set; }
    [JsonPropertyName("sender_id")] public string? SenderId { get; set; }
    [JsonPropertyName("purpose")] public string? Purpose { get; set; }
    [JsonPropertyName("customer_id")] public string? CustomerId { get; set; }
    [JsonPropertyName("recipient")] public ChimeRecipient? Recipient { get; set; }
    [JsonPropertyName("email")] public ChimeEmailMessage? Email { get; set; }
    [JsonPropertyName("transmission")] public ChimeTransmission? Transmission { get; set; }
    [JsonPropertyName("custom_data")] public CustomData? CustomData { get; set; }
}

public sealed class ChimeRecipient
{
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("phone")] public ChimeRecipientPhone? Phone { get; set; }
    [JsonPropertyName("email")] public ChimeRecipientEmail? Email { get; set; }
}

public sealed class ChimeRecipientPhone
{
    [JsonPropertyName("number")] public string? Number { get; set; }
}

public sealed class ChimeRecipientEmail
{
    [JsonPropertyName("address")] public string? Address { get; set; }
}

public sealed class MessageMailbox
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("address")] public string? Address { get; set; }
}

public sealed class ChimeEmailMessage
{
    [JsonPropertyName("subject")] public string? Subject { get; set; }
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("html")] public string? Html { get; set; }
    [JsonPropertyName("from")] public MessageMailbox? From { get; set; }
    [JsonPropertyName("reply_to")] public MessageMailbox? ReplyTo { get; set; }
    [JsonPropertyName("headers")] public MessageHeaders? Headers { get; set; }
    [JsonPropertyName("safety")] public MessageTemplateSafetyResult? Safety { get; set; }
    [JsonPropertyName("schema")] public JsonData? Schema { get; set; }
}

public sealed class ChimeTransmission
{
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
    [JsonPropertyName("sender_id")] public string? SenderId { get; set; }
    [JsonPropertyName("sent_via")] public string? SentVia { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("sent_at")] public DateTimeOffset? SentAt { get; set; }
    [JsonPropertyName("delivered_at")] public DateTimeOffset? DeliveredAt { get; set; }
    [JsonPropertyName("failed_at")] public DateTimeOffset? FailedAt { get; set; }
}

public sealed class ChimePage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("chimes")] public List<Chime>? Chimes { get; set; }
}

public sealed class ScheduledChime
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("recipients")] public List<string>? Recipients { get; set; }
    [JsonPropertyName("content")] public string? Content { get; set; }
    [JsonPropertyName("full_message")] public string? FullMessage { get; set; }
    [JsonPropertyName("sender_id")] public string? SenderId { get; set; }
    [JsonPropertyName("purpose")] public string? Purpose { get; set; }
    [JsonPropertyName("send_after")] public DateTimeOffset? SendAfter { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("executed_at")] public DateTimeOffset? ExecutedAt { get; set; }
    [JsonPropertyName("canceled_at")] public DateTimeOffset? CanceledAt { get; set; }
    [JsonPropertyName("chime_ids")] public List<string>? ChimeIds { get; set; }
    [JsonPropertyName("errors")] public List<ApiError>? Errors { get; set; }
}

public sealed class MessageTemplate
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("about")] public string? About { get; set; }
    [JsonPropertyName("channel")] public string? Channel { get; set; }
    [JsonPropertyName("purpose")] public string? Purpose { get; set; }
    [JsonPropertyName("locale")] public string? Locale { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("sms")] public MessageTemplateSmsContent? Sms { get; set; }
    [JsonPropertyName("email")] public MessageTemplateEmailContent? Email { get; set; }
    [JsonPropertyName("variables")] public List<MessageTemplateVariable>? Variables { get; set; }
    [JsonPropertyName("attachments")] public List<string>? Attachments { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonPropertyName("published_at")] public DateTimeOffset? PublishedAt { get; set; }
    [JsonPropertyName("archived_at")] public DateTimeOffset? ArchivedAt { get; set; }
}

public sealed class MessageTemplateSmsContent
{
    [JsonPropertyName("message_template")] public string? MessageTemplate { get; set; }
}

public sealed class MessageTemplateEmailContent
{
    [JsonPropertyName("subject")] public string? Subject { get; set; }
    [JsonPropertyName("html")] public string? Html { get; set; }
    [JsonPropertyName("from")] public MessageMailbox? From { get; set; }
    [JsonPropertyName("reply_to")] public MessageMailbox? ReplyTo { get; set; }
    [JsonPropertyName("headers")] public MessageHeaders? Headers { get; set; }
}

public sealed class MessageTemplateVariable
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("required")] public bool? Required { get; set; }
    [JsonPropertyName("default")] public JsonValue? Default { get; set; }
    [JsonPropertyName("about")] public string? About { get; set; }
    [JsonPropertyName("items")] public List<MessageTemplateVariableItem>? Items { get; set; }
}

public sealed class MessageTemplateVariableItem
{
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("required")] public bool? Required { get; set; }
    [JsonPropertyName("default")] public JsonValue? Default { get; set; }
    [JsonPropertyName("about")] public string? About { get; set; }
}

public sealed class MessageTemplateSafetyResult
{
    [JsonPropertyName("content_hash")] public string? ContentHash { get; set; }
    [JsonPropertyName("links")] public List<MessageTemplateScannedLink>? Links { get; set; }
    [JsonPropertyName("normalized_text")] public string? NormalizedText { get; set; }
    [JsonPropertyName("quarantine_notes")] public string? QuarantineNotes { get; set; }
    [JsonPropertyName("reason_codes")] public List<string>? ReasonCodes { get; set; }
    [JsonPropertyName("sanitized_html")] public string? SanitizedHtml { get; set; }
    [JsonPropertyName("scanner")] public string? Scanner { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
}

public sealed class MessageTemplateScannedLink
{
    [JsonPropertyName("host")] public string? Host { get; set; }
    [JsonPropertyName("raw")] public string? Raw { get; set; }
    [JsonPropertyName("reason")] public string? Reason { get; set; }
    [JsonPropertyName("scheme")] public string? Scheme { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
}

public sealed class MessageTemplatePage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("message_templates")] public List<MessageTemplate>? MessageTemplates { get; set; }
}

public sealed class MessageTemplatePreview
{
    [JsonPropertyName("message_template")] public MessageTemplate? MessageTemplate { get; set; }
    [JsonPropertyName("rendered")] public RenderedMessageTemplate? Rendered { get; set; }
}

public sealed class RenderedMessageTemplate
{
    [JsonPropertyName("channel")] public string? Channel { get; set; }
    [JsonPropertyName("attachments")] public List<string>? Attachments { get; set; }
    [JsonPropertyName("sms")] public RenderedSmsMessageTemplate? Sms { get; set; }
    [JsonPropertyName("email")] public RenderedEmailMessageTemplate? Email { get; set; }
}

public sealed class RenderedSmsMessageTemplate
{
    [JsonPropertyName("full_message")] public string? FullMessage { get; set; }
}

public sealed class RenderedEmailMessageTemplate
{
    [JsonPropertyName("subject")] public string? Subject { get; set; }
    [JsonPropertyName("text")] public string? Text { get; set; }
    [JsonPropertyName("html")] public string? Html { get; set; }
    [JsonPropertyName("from")] public MessageMailbox? From { get; set; }
    [JsonPropertyName("reply_to")] public MessageMailbox? ReplyTo { get; set; }
    [JsonPropertyName("headers")] public MessageHeaders? Headers { get; set; }
    [JsonPropertyName("safety")] public MessageTemplateSafetyResult? Safety { get; set; }
}

public sealed class OtpTransaction
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("full_message")] public string? FullMessage { get; set; }
    [JsonPropertyName("initiated_at")] public DateTimeOffset? InitiatedAt { get; set; }
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("canceled_at")] public DateTimeOffset? CanceledAt { get; set; }
    [JsonPropertyName("cancel_reason")] public string? CancelReason { get; set; }
    [JsonPropertyName("transmission")] public ChimeTransmission? Transmission { get; set; }
}

public sealed class OtpVerificationAttempt
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
    [JsonPropertyName("presented_token")] public string? PresentedToken { get; set; }
    [JsonPropertyName("attempted_at")] public DateTimeOffset? AttemptedAt { get; set; }
    [JsonPropertyName("result")] public OtpVerificationAttemptResult? Result { get; set; }
}

public sealed class OtpVerificationAttemptResult
{
    [JsonPropertyName("detail")] public string? Detail { get; set; }
    [JsonPropertyName("verdict")] public string? Verdict { get; set; }
}

public sealed class OtpVerification
{
    [JsonPropertyName("transaction")] public OtpTransaction? Transaction { get; set; }
    [JsonPropertyName("verification_attempt")] public OtpVerificationAttempt? VerificationAttempt { get; set; }
}

public sealed class GeneratedSecretKey
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("token_type")] public string? TokenType { get; set; }
    [JsonPropertyName("issued_at")] public DateTimeOffset? IssuedAt { get; set; }
    [JsonPropertyName("token")] public string? Token { get; set; }
}

public sealed class SecretKey
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("label")] public string? Label { get; set; }
    [JsonPropertyName("token_type")] public string? TokenType { get; set; }
    [JsonPropertyName("issued_at")] public DateTimeOffset? IssuedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("active")] public bool? Active { get; set; }
    [JsonPropertyName("revoked_at")] public DateTimeOffset? RevokedAt { get; set; }
    [JsonPropertyName("last_used_at")] public DateTimeOffset? LastUsedAt { get; set; }
    [JsonPropertyName("usage_count")] public int? UsageCount { get; set; }
}

public sealed class SecretKeyPage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("count")] public int? Count { get; set; }
    [JsonPropertyName("total")] public int? Total { get; set; }
    [JsonPropertyName("has_more")] public bool? HasMore { get; set; }
    [JsonPropertyName("keys")] public List<SecretKey>? Keys { get; set; }
}

public sealed class SecretKeyUsageRow
{
    [JsonPropertyName("secret_key_id")] public string? SecretKeyId { get; set; }
    [JsonPropertyName("occurred_at")] public DateTimeOffset? OccurredAt { get; set; }
    [JsonPropertyName("auth_result")] public string? AuthResult { get; set; }
}

public sealed class SecretKeyUsagePage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("count")] public int? Count { get; set; }
    [JsonPropertyName("total")] public int? Total { get; set; }
    [JsonPropertyName("has_more")] public bool? HasMore { get; set; }
    [JsonPropertyName("rows")] public List<SecretKeyUsageRow>? Rows { get; set; }
}

public sealed class SecretKeyUsage
{
    [JsonPropertyName("key")] public SecretKey? Key { get; set; }
    [JsonPropertyName("usage")] public SecretKeyUsagePage? Usage { get; set; }
}

public sealed class StoredFile
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("purpose")] public string? Purpose { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("scan_status")] public string? ScanStatus { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("filename")] public string? Filename { get; set; }
    [JsonPropertyName("content_type")] public string? ContentType { get; set; }
    [JsonPropertyName("size")] public long? Size { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("updated_at")] public DateTimeOffset? UpdatedAt { get; set; }
    [JsonPropertyName("deleted_at")] public DateTimeOffset? DeletedAt { get; set; }
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("custom_data")] public CustomData? CustomData { get; set; }
}

public sealed class StoredFilePage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("files")] public List<StoredFile>? Files { get; set; }
}

public sealed class FileLink
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("file_id")] public string? FileId { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("revoked_at")] public DateTimeOffset? RevokedAt { get; set; }
    [JsonPropertyName("custom_data")] public CustomData? CustomData { get; set; }
}

public sealed class FileLinkPage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("file_links")] public List<FileLink>? FileLinks { get; set; }
}

public sealed class FileLinkCreation
{
    [JsonPropertyName("file_link")] public FileLink? FileLink { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
}

public sealed class FileReferenceReconciliation
{
    [JsonPropertyName("reconciled")] public bool? Reconciled { get; set; }
    [JsonPropertyName("error")] public ApiError? Error { get; set; }
}

public sealed class UploadRequest
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("purpose")] public string? Purpose { get; set; }
    [JsonPropertyName("status")] public string? Status { get; set; }
    [JsonPropertyName("upload_url")] public string? UploadUrl { get; set; }
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("created_at")] public DateTimeOffset? CreatedAt { get; set; }
    [JsonPropertyName("canceled_at")] public DateTimeOffset? CanceledAt { get; set; }
    [JsonPropertyName("custom_data")] public CustomData? CustomData { get; set; }
    [JsonPropertyName("metadata")] public FileMetadata? Metadata { get; set; }
    [JsonPropertyName("constraints")] public UploadRequestConstraints? Constraints { get; set; }
    [JsonPropertyName("display")] public UploadRequestDisplay? Display { get; set; }
    [JsonPropertyName("attempts")] public UploadRequestAttempts? Attempts { get; set; }
}

public sealed class UploadRequestConstraints
{
    [JsonPropertyName("min_size")] public long? MinSize { get; set; }
    [JsonPropertyName("max_size")] public long? MaxSize { get; set; }
    [JsonPropertyName("exact_size")] public long? ExactSize { get; set; }
    [JsonPropertyName("content_types")] public List<string>? ContentTypes { get; set; }
    [JsonPropertyName("extensions")] public List<string>? Extensions { get; set; }
    [JsonPropertyName("filename")] public string? Filename { get; set; }
}

public sealed class UploadRequestDisplay
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("help_text")] public string? HelpText { get; set; }
}

public sealed class UploadRequestAttempts
{
    [JsonPropertyName("max_attempts")] public int? MaxAttempts { get; set; }
    [JsonPropertyName("attempt_count")] public int? AttemptCount { get; set; }
    [JsonPropertyName("failed_attempt_count")] public int? FailedAttemptCount { get; set; }
    [JsonPropertyName("last_attempted_at")] public DateTimeOffset? LastAttemptedAt { get; set; }
}

public sealed class UploadRequestPage
{
    [JsonPropertyName("number")] public int? Number { get; set; }
    [JsonPropertyName("size")] public int? Size { get; set; }
    [JsonPropertyName("upload_requests")] public List<UploadRequest>? UploadRequests { get; set; }
}

public sealed class UploadFulfillment
{
    [JsonPropertyName("upload_request")] public UploadRequest? UploadRequest { get; set; }
    [JsonPropertyName("file")] public StoredFile? File { get; set; }
}

public sealed class PaymentMethodVerificationSession
{
    [JsonPropertyName("payment_method_id")] public string PaymentMethodId { get; set; } = string.Empty;
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
    [JsonPropertyName("token_sent_at")] public DateTimeOffset? TokenSentAt { get; set; }
    [JsonPropertyName("expires_at")] public DateTimeOffset? ExpiresAt { get; set; }
    [JsonPropertyName("delivery")] public PaymentMethodVerificationDelivery? Delivery { get; set; }
}

public sealed class PaymentMethodVerificationDelivery
{
    [JsonPropertyName("recipient")] public string? Recipient { get; set; }
    [JsonPropertyName("channel")] public string? Channel { get; set; }
    [JsonPropertyName("sender_id")] public string? SenderId { get; set; }
}

public sealed class PaymentMethodDeletion
{
    [JsonPropertyName("deleted")] public bool? Deleted { get; set; }
    [JsonPropertyName("payment_method_id")] public string? PaymentMethodId { get; set; }
}

public sealed class CountryBankBranch
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("sort_code")] public string? SortCode { get; set; }
}

public sealed class CountryBank
{
    [JsonPropertyName("id")] public string? Id { get; set; }
    [JsonPropertyName("name")] public string? Name { get; set; }
    [JsonPropertyName("swift_code")] public string? SwiftCode { get; set; }
    [JsonPropertyName("sort_code_prefix")] public string? SortCodePrefix { get; set; }
    [JsonPropertyName("branches")] public List<CountryBankBranch>? Branches { get; set; }
}

public sealed class CountryBankDirectory
{
    [JsonPropertyName("bank_account_type")] public string? BankAccountType { get; set; }
    [JsonPropertyName("code_scheme")] public string? CodeScheme { get; set; }
    [JsonPropertyName("items")] public List<CountryBank>? Items { get; set; }
}

public sealed class CountrySpecification
{
    [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
    [JsonPropertyName("country_name")] public string? CountryName { get; set; }
    [JsonPropertyName("currencies")] public List<string>? Currencies { get; set; }
    [JsonPropertyName("payment_methods")] public List<string>? PaymentMethods { get; set; }
    [JsonPropertyName("payout_schedules")] public List<string>? PayoutSchedules { get; set; }
    [JsonPropertyName("bt_aging_specs")] public List<string>? BalanceTransactionAgingSpecs { get; set; }
    [JsonPropertyName("legal_entity_types")] public List<string>? LegalEntityTypes { get; set; }
    [JsonPropertyName("financial_account_types")] public List<string>? FinancialAccountTypes { get; set; }
    [JsonPropertyName("id_document_types")] public List<string>? IdDocumentTypes { get; set; }
    [JsonPropertyName("banks")] public CountryBankDirectory? Banks { get; set; }
}

[JsonConverter(typeof(CountrySpecificationsJsonConverter))]
public sealed class CountrySpecifications : IReadOnlyDictionary<string, CountrySpecification>
{
    private readonly Dictionary<string, CountrySpecification> _values = new();
    internal IDictionary<string, CountrySpecification> MutableValues => _values;
    public CountrySpecification this[string key] => _values[key];
    public IEnumerable<string> Keys => _values.Keys;
    public IEnumerable<CountrySpecification> Values => _values.Values;
    public int Count => _values.Count;
    public bool ContainsKey(string key) => _values.ContainsKey(key);
    public bool TryGetValue(string key, out CountrySpecification value) => _values.TryGetValue(key, out value!);
    public IEnumerator<KeyValuePair<string, CountrySpecification>> GetEnumerator() => _values.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
