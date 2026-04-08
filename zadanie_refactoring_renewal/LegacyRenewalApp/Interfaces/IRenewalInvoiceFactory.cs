namespace LegacyRenewalApp.Interfaces
{
    public interface IRenewalInvoiceFactory
    {
        RenewalInvoice Create(
            int customerId,
            Customer customer,
            string normalizedPlanCode,
            string normalizedPaymentMethod,
            int seatCount,
            decimal baseAmount,
            decimal discountAmount,
            decimal supportFee,
            decimal paymentFee,
            decimal taxAmount,
            decimal finalAmount,
            string notes);
    }
}