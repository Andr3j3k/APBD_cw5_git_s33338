namespace LegacyRenewalApp
{
    public class DiscountResult
    {
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public DiscountResult(decimal amount, string notes)
        {
            Amount = amount;
            Notes = notes;
        }
    }
}