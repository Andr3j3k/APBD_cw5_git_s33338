using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp
{
    public class NotificationService : INotificationService
    {
        private readonly IBillingGateway _billingGateway;

        public NotificationService(IBillingGateway billingGateway)
        {
            _billingGateway = billingGateway;
        }

        public void Send(Customer customer, string normalizedPlanCode, RenewalInvoice invoice)
        {
            if (string.IsNullOrWhiteSpace(customer.Email))
            {
                return;
            }

            string subject = "Subscription renewal invoice";
            string body =
                $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

            _billingGateway.SendEmail(customer.Email, subject, body);
        }
    }
}