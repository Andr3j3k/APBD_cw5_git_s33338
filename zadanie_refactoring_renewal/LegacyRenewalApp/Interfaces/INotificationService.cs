namespace LegacyRenewalApp.Interfaces
{
    public interface INotificationService
    {
        void Send(Customer customer, string normalizedPlanCode, RenewalInvoice invoice);
    }
}