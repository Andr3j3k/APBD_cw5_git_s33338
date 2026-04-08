namespace LegacyRenewalApp.Interfaces
{
    public interface IPaymentFeeCalculator
    {
        FeeResult Calculate(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee);
    }
}