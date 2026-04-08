using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp
{
    public class DiscountCalculator : IDiscountCalculator
    {
        public DiscountResult Calculate(Customer customer, SubscriptionPlan plan, decimal baseAmount, int seatCount, bool useLoyaltyPoints)
        {
            decimal discountAmount = 0m;
            var notes = new NotesBuilder();

            discountAmount += CalculateSegmentDiscount(customer, plan, baseAmount, notes);
            discountAmount += CalculateLoyaltyDiscount(customer, baseAmount, notes);
            discountAmount += CalculateSeatDiscount(seatCount, baseAmount, notes);
            discountAmount += CalculatePointsDiscount(customer, useLoyaltyPoints, notes);

            return new DiscountResult(discountAmount, notes.ToString());
        }

        private decimal CalculateSegmentDiscount(Customer customer, SubscriptionPlan plan, decimal baseAmount, NotesBuilder notes)
        {
            if (customer.Segment == "Silver")
            {
                notes.Add("silver discount");
                return baseAmount * 0.05m;
            }

            if (customer.Segment == "Gold")
            {
                notes.Add("gold discount");
                return baseAmount * 0.10m;
            }

            if (customer.Segment == "Platinum")
            {
                notes.Add("platinum discount");
                return baseAmount * 0.15m;
            }

            if (customer.Segment == "Education" && plan.IsEducationEligible)
            {
                notes.Add("education discount");
                return baseAmount * 0.20m;
            }

            return 0m;
        }

        private decimal CalculateLoyaltyDiscount(Customer customer, decimal baseAmount, NotesBuilder notes)
        {
            if (customer.YearsWithCompany >= 5)
            {
                notes.Add("long-term loyalty discount");
                return baseAmount * 0.07m;
            }

            if (customer.YearsWithCompany >= 2)
            {
                notes.Add("basic loyalty discount");
                return baseAmount * 0.03m;
            }

            return 0m;
        }

        private decimal CalculateSeatDiscount(int seatCount, decimal baseAmount, NotesBuilder notes)
        {
            if (seatCount >= 50)
            {
                notes.Add("large team discount");
                return baseAmount * 0.12m;
            }

            if (seatCount >= 20)
            {
                notes.Add("medium team discount");
                return baseAmount * 0.08m;
            }

            if (seatCount >= 10)
            {
                notes.Add("small team discount");
                return baseAmount * 0.04m;
            }

            return 0m;
        }

        private decimal CalculatePointsDiscount(Customer customer, bool useLoyaltyPoints, NotesBuilder notes)
        {
            if (!useLoyaltyPoints || customer.LoyaltyPoints <= 0)
            {
                return 0m;
            }

            int pointsToUse = Math.Min(customer.LoyaltyPoints, 200);
            notes.Add($"loyalty points used: {pointsToUse}");
            return pointsToUse;
        }
    }
}