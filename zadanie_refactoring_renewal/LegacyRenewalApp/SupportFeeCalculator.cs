using System.Collections.Generic;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp
{
    public class SupportFeeCalculator : ISupportFeeCalculator
    {
        private static readonly Dictionary<string, decimal> PremiumSupportFees = new Dictionary<string, decimal>
        {
            { "START", 250m },
            { "PRO", 400m },
            { "ENTERPRISE", 700m }
        };

        public FeeResult Calculate(string normalizedPlanCode, bool includePremiumSupport)
        {
            if (!includePremiumSupport)
            {
                return new FeeResult(0m, string.Empty);
            }

            decimal fee = PremiumSupportFees.ContainsKey(normalizedPlanCode)
                ? PremiumSupportFees[normalizedPlanCode]
                : 0m;

            return new FeeResult(fee, "premium support included");
        }
    }
}