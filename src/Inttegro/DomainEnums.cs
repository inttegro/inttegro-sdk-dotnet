using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Inttegro;

[JsonConverter(typeof(WireEnumJsonConverter<AppManagementRole>))]
public enum AppManagementRole { [EnumMember(Value = "parent")] Parent, [EnumMember(Value = "child")] Child }

[JsonConverter(typeof(WireEnumJsonConverter<AppCredentialOwner>))]
public enum AppCredentialOwner { [EnumMember(Value = "child")] Child, [EnumMember(Value = "parent")] Parent }

[JsonConverter(typeof(WireEnumJsonConverter<AppRelationshipKind>))]
public enum AppRelationshipKind { [EnumMember(Value = "placement")] Placement }

[JsonConverter(typeof(WireEnumJsonConverter<AppRelationshipStatus>))]
public enum AppRelationshipStatus { [EnumMember(Value = "active")] Active, [EnumMember(Value = "inactive")] Inactive, [EnumMember(Value = "suspended")] Suspended, [EnumMember(Value = "revoked")] Revoked }

[JsonConverter(typeof(WireEnumJsonConverter<SecretKeyTokenType>))]
public enum SecretKeyTokenType { [EnumMember(Value = "bearer")] Bearer }

[JsonConverter(typeof(WireEnumJsonConverter<SecretKeyStatus>))]
public enum SecretKeyStatus { [EnumMember(Value = "active")] Active, [EnumMember(Value = "revoked")] Revoked, [EnumMember(Value = "expired")] Expired }

[JsonConverter(typeof(WireEnumJsonConverter<SecretKeyAuthResult>))]
public enum SecretKeyAuthResult { [EnumMember(Value = "succeeded")] Succeeded, [EnumMember(Value = "failed")] Failed }

[JsonConverter(typeof(WireEnumJsonConverter<FileStatus>))]
public enum FileStatus { [EnumMember(Value = "uploading")] Uploading, [EnumMember(Value = "processing")] Processing, [EnumMember(Value = "available")] Available, [EnumMember(Value = "failed")] Failed, [EnumMember(Value = "deleted")] Deleted }

[JsonConverter(typeof(WireEnumJsonConverter<FileDisposition>))]
public enum FileDisposition { [EnumMember(Value = "attachment")] Attachment, [EnumMember(Value = "inline")] Inline }

[JsonConverter(typeof(WireEnumJsonConverter<FileDelivery>))]
public enum FileDelivery { [EnumMember(Value = "stream")] Stream, [EnumMember(Value = "redirect")] Redirect }

[JsonConverter(typeof(WireEnumJsonConverter<FileScanStatus>))]
public enum FileScanStatus { [EnumMember(Value = "pending")] Pending, [EnumMember(Value = "passed")] Passed, [EnumMember(Value = "failed")] Failed, [EnumMember(Value = "skipped")] Skipped }

[JsonConverter(typeof(WireEnumJsonConverter<FileSourceType>))]
public enum FileSourceType { [EnumMember(Value = "direct")] Direct, [EnumMember(Value = "upload_request")] UploadRequest, [EnumMember(Value = "service")] Service }

[JsonConverter(typeof(WireEnumJsonConverter<FileStorageEncoding>))]
public enum FileStorageEncoding { [EnumMember(Value = "identity")] Identity, [EnumMember(Value = "br")] Brotli }

[JsonConverter(typeof(WireEnumJsonConverter<FileLinkStatus>))]
public enum FileLinkStatus { [EnumMember(Value = "active")] Active, [EnumMember(Value = "revoked")] Revoked, [EnumMember(Value = "expired")] Expired, [EnumMember(Value = "disabled")] Disabled }

[JsonConverter(typeof(WireEnumJsonConverter<FileLinkKind>))]
public enum FileLinkKind { [EnumMember(Value = "public")] Public }

[JsonConverter(typeof(WireEnumJsonConverter<FileLinkDeliveryMode>))]
public enum FileLinkDeliveryMode { [EnumMember(Value = "redirect")] Redirect, [EnumMember(Value = "download")] Download, [EnumMember(Value = "inline")] Inline }

[JsonConverter(typeof(WireEnumJsonConverter<UploadRequestStatus>))]
public enum UploadRequestStatus { [EnumMember(Value = "pending")] Pending, [EnumMember(Value = "uploading")] Uploading, [EnumMember(Value = "fulfilled")] Fulfilled, [EnumMember(Value = "expired")] Expired, [EnumMember(Value = "canceled")] Canceled, [EnumMember(Value = "failed")] Failed }

[JsonConverter(typeof(WireEnumJsonConverter<UploadReviewDecision>))]
public enum UploadReviewDecision { [EnumMember(Value = "approved")] Approved, [EnumMember(Value = "rejected")] Rejected }

[JsonConverter(typeof(WireEnumJsonConverter<UploadReviewType>))]
public enum UploadReviewType { [EnumMember(Value = "automatic")] Automatic, [EnumMember(Value = "manual")] Manual }

[JsonConverter(typeof(WireEnumJsonConverter<ProductShipmentType>))]
public enum ProductShipmentType { [EnumMember(Value = "delivery")] Delivery, [EnumMember(Value = "download")] Download, [EnumMember(Value = "render")] Render, [EnumMember(Value = "service")] Service, [EnumMember(Value = "stream")] Stream }

[JsonConverter(typeof(WireEnumJsonConverter<ProductShipmentInputType>))]
public enum ProductShipmentInputType { [EnumMember(Value = "delivery")] Delivery, [EnumMember(Value = "download")] Download, [EnumMember(Value = "render")] Render, [EnumMember(Value = "stream")] Stream }

[JsonConverter(typeof(WireEnumJsonConverter<PurchaseIntentStatus>))]
public enum PurchaseIntentStatus { [EnumMember(Value = "active")] Active, [EnumMember(Value = "expired")] Expired, [EnumMember(Value = "inactive")] Inactive, [EnumMember(Value = "used")] Used }

[JsonConverter(typeof(WireEnumJsonConverter<MessageTemplateChannel>))]
public enum MessageTemplateChannel { [EnumMember(Value = "sms")] Sms, [EnumMember(Value = "email")] Email }

[JsonConverter(typeof(WireEnumJsonConverter<MessageTemplateStatus>))]
public enum MessageTemplateStatus { [EnumMember(Value = "draft")] Draft, [EnumMember(Value = "published")] Published, [EnumMember(Value = "archived")] Archived }

[JsonConverter(typeof(WireEnumJsonConverter<MessageTemplateVariableType>))]
public enum MessageTemplateVariableType { [EnumMember(Value = "string")] String, [EnumMember(Value = "number")] Number, [EnumMember(Value = "integer")] Integer, [EnumMember(Value = "boolean")] Boolean, [EnumMember(Value = "url")] Url, [EnumMember(Value = "email")] Email, [EnumMember(Value = "phone")] Phone, [EnumMember(Value = "date")] Date, [EnumMember(Value = "datetime")] Datetime, [EnumMember(Value = "array")] Array }

[JsonConverter(typeof(WireEnumJsonConverter<MessageTemplateVariableItemType>))]
public enum MessageTemplateVariableItemType { [EnumMember(Value = "string")] String, [EnumMember(Value = "number")] Number, [EnumMember(Value = "integer")] Integer, [EnumMember(Value = "boolean")] Boolean, [EnumMember(Value = "url")] Url, [EnumMember(Value = "email")] Email, [EnumMember(Value = "phone")] Phone, [EnumMember(Value = "date")] Date, [EnumMember(Value = "datetime")] Datetime }

[JsonConverter(typeof(WireEnumJsonConverter<ContentSafetyStatus>))]
public enum ContentSafetyStatus { [EnumMember(Value = "allowed")] Allowed, [EnumMember(Value = "rejected")] Rejected, [EnumMember(Value = "quarantined")] Quarantined }

[JsonConverter(typeof(WireEnumJsonConverter<OrderDocumentKind>))]
public enum OrderDocumentKind { [EnumMember(Value = "invoice")] Invoice, [EnumMember(Value = "receipt")] Receipt }

[JsonConverter(typeof(WireEnumJsonConverter<DeliveryChannel>))]
public enum DeliveryChannel { [EnumMember(Value = "email")] Email, [EnumMember(Value = "sms")] Sms }

[JsonConverter(typeof(WireEnumJsonConverter<CheckoutOrderStatus>))]
public enum CheckoutOrderStatus { [EnumMember(Value = "preparing")] Preparing, [EnumMember(Value = "requires_payment")] RequiresPayment, [EnumMember(Value = "completed")] Completed, [EnumMember(Value = "canceled")] Canceled, [EnumMember(Value = "expired")] Expired }

[JsonConverter(typeof(WireEnumJsonConverter<OrderStatus>))]
public enum OrderStatus { [EnumMember(Value = "preparing")] Preparing, [EnumMember(Value = "requires_payment")] RequiresPayment, [EnumMember(Value = "paid")] Paid, [EnumMember(Value = "completed")] Completed, [EnumMember(Value = "canceled")] Canceled, [EnumMember(Value = "expired")] Expired, [EnumMember(Value = "unknown")] Unknown }

[JsonConverter(typeof(WireEnumJsonConverter<OrderCreatedFromResourceType>))]
public enum OrderCreatedFromResourceType { [EnumMember(Value = "purchase_intent")] PurchaseIntent }

[JsonConverter(typeof(WireEnumJsonConverter<PayoutStatus>))]
public enum PayoutStatus { [EnumMember(Value = "initialized")] Initialized, [EnumMember(Value = "scheduled")] Scheduled, [EnumMember(Value = "processing")] Processing, [EnumMember(Value = "executing")] Executing, [EnumMember(Value = "succeeded")] Succeeded, [EnumMember(Value = "invalid")] Invalid, [EnumMember(Value = "canceled")] Canceled }

[JsonConverter(typeof(WireEnumJsonConverter<ChimeRecipientType>))]
public enum ChimeRecipientType { [EnumMember(Value = "phone")] Phone, [EnumMember(Value = "email")] Email }

[JsonConverter(typeof(WireEnumJsonConverter<ChimeTransport>))]
public enum ChimeTransport { [EnumMember(Value = "sms")] Sms, [EnumMember(Value = "email")] Email }

[JsonConverter(typeof(WireEnumJsonConverter<ChimeEmailSchemaKind>))]
public enum ChimeEmailSchemaKind { [EnumMember(Value = "gmail_view_action")] GmailViewAction, [EnumMember(Value = "schema_org_order")] SchemaOrgOrder, [EnumMember(Value = "schema_org_invoice")] SchemaOrgInvoice }

[JsonConverter(typeof(WireEnumJsonConverter<OTPAlphabetType>))]
public enum OTPAlphabetType { [EnumMember(Value = "numeric")] Numeric, [EnumMember(Value = "alpha")] Alpha, [EnumMember(Value = "alphanumeric")] Alphanumeric }

[JsonConverter(typeof(WireEnumJsonConverter<OTPPurpose>))]
public enum OTPPurpose { [EnumMember(Value = "account_creation")] AccountCreation, [EnumMember(Value = "account_recovery")] AccountRecovery, [EnumMember(Value = "email_verification")] EmailVerification, [EnumMember(Value = "financial_account_verification")] FinancialAccountVerification, [EnumMember(Value = "password_reset")] PasswordReset, [EnumMember(Value = "payment_confirmation")] PaymentConfirmation, [EnumMember(Value = "payment_method_verification")] PaymentMethodVerification, [EnumMember(Value = "payout_confirmation")] PayoutConfirmation, [EnumMember(Value = "phone_verification")] PhoneVerification, [EnumMember(Value = "sensitive_action")] SensitiveAction, [EnumMember(Value = "sign_in")] SignIn, [EnumMember(Value = "transaction_confirmation")] TransactionConfirmation, [EnumMember(Value = "unspecified")] Unspecified }

[JsonConverter(typeof(WireEnumJsonConverter<OTPStatus>))]
public enum OTPStatus { [EnumMember(Value = "canceled")] Canceled, [EnumMember(Value = "expired")] Expired, [EnumMember(Value = "pending")] Pending, [EnumMember(Value = "pending_delivery")] PendingDelivery, [EnumMember(Value = "pending_verification")] PendingVerification, [EnumMember(Value = "verified")] Verified }

[JsonConverter(typeof(WireEnumJsonConverter<OTPTransmissionStatus>))]
public enum OTPTransmissionStatus { [EnumMember(Value = "delivered")] Delivered, [EnumMember(Value = "failed")] Failed, [EnumMember(Value = "submitted")] Submitted }

[JsonConverter(typeof(WireEnumJsonConverter<OTPVerificationVerdict>))]
public enum OTPVerificationVerdict { [EnumMember(Value = "fail")] Fail, [EnumMember(Value = "pass")] Pass }
