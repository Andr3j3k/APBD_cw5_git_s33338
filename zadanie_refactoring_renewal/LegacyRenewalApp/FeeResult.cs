namespace LegacyRenewalApp
{
    public class FeeResult
    {
        public decimal Amount { get; set; }
        public string Notes { get; set; }

        public FeeResult(decimal amount, string notes)
        {
            Amount = amount;
            Notes = notes;
        }
    }
}