namespace LegacyRenewalApp.Interfaces
{
    public interface ISupportFeeCalculator
    {
        FeeResult Calculate(string normalizedPlanCode, bool includePremiumSupport);
    }
}