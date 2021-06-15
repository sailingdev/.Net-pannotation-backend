namespace Pannotation.Common.Constants
{
    public static class StripeWebhookType
    {
        public const string ChargeExpired = "charge.expired";
        public const string ChargeFailed = "charge.failed";
        public const string ChargePending = "charge.pending";
        public const string ChargeSucceeded = "charge.succeeded";
        public const string ChargeUpdated = "charge.updated";
        public const string ChargeRefunded = "charge.refunded";

        public const string SubscriptionCreated = "customer.subscription.created";
        public const string SubscriptionDeleted = "customer.subscription.deleted";
        public const string SubscriptionUpdated = "customer.subscription.updated";
        public const string SubscriptionTrialEnd = "customer.subscription.trial_will_end";

        public const string InvoiceCreated = "invoice.created";
        public const string InvoiceDeleted = "invoice.deleted";
        public const string InvoicePaymentSucceeded = "invoice.payment_succeeded";
        public const string InvoicePaymentFailed = "invoice.payment_failed";
    }
}
