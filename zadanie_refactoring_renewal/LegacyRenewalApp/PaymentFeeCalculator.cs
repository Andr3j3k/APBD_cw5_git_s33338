using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp
{
    public class PaymentFeeCalculator : IPaymentFeeCalculator
    {
        public FeeResult Calculate(string normalizedPaymentMethod, decimal subtotalAfterDiscount, decimal supportFee)
        {
            decimal paymentBase = subtotalAfterDiscount + supportFee;

            if (normalizedPaymentMethod == "CARD")
            {
                return new FeeResult(paymentBase * 0.02m, "card payment fee");
            }

            if (normalizedPaymentMethod == "BANK_TRANSFER")
            {
                return new FeeResult(paymentBase * 0.01m, "bank transfer fee");
            }

            if (normalizedPaymentMethod == "PAYPAL")
            {
                return new FeeResult(paymentBase * 0.035m, "paypal fee");
            }

            if (normalizedPaymentMethod == "INVOICE")
            {
                return new FeeResult(0m, "invoice payment");
            }

            throw new ArgumentException("Unsupported payment method");
        }
    }
}