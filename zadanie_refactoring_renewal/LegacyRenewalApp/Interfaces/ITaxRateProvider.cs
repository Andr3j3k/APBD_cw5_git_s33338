namespace LegacyRenewalApp.Interfaces
{
    public interface ITaxRateProvider
    {
        decimal GetTaxRate(string country);
    }
}