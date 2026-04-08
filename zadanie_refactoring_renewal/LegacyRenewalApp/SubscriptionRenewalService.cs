using System;
using LegacyRenewalApp.Interfaces;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IPaymentFeeCalculator _paymentFeeCalculator;
        private readonly ITaxRateProvider _taxRateProvider;
        private readonly IRenewalInvoiceFactory _invoiceFactory;
        private readonly IBillingGateway _billingGateway;
        private readonly INotificationService _notificationService;
        private readonly RenewalRequestValidator _validator;

        public SubscriptionRenewalService()
            : this(
                new CustomerRepository(),
                new SubscriptionPlanRepository(),
                new DiscountCalculator(),
                new SupportFeeCalculator(),
                new PaymentFeeCalculator(),
                new CountryTaxRateProvider(),
                new RenewalInvoiceFactory(),
                new LegacyBillingGatewayAdapter(),
                new RenewalRequestValidator())
        {
        }

        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository planRepository,
            IDiscountCalculator discountCalculator,
            ISupportFeeCalculator supportFeeCalculator,
            IPaymentFeeCalculator paymentFeeCalculator,
            ITaxRateProvider taxRateProvider,
            IRenewalInvoiceFactory invoiceFactory,
            IBillingGateway billingGateway,
            RenewalRequestValidator validator)
        {
            _customerRepository = customerRepository;
            _planRepository = planRepository;
            _discountCalculator = discountCalculator;
            _supportFeeCalculator = supportFeeCalculator;
            _paymentFeeCalculator = paymentFeeCalculator;
            _taxRateProvider = taxRateProvider;
            _invoiceFactory = invoiceFactory;
            _billingGateway = billingGateway;
            _notificationService = new NotificationService(_billingGateway);
            _validator = validator;
        }

        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            _validator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customer = _customerRepository.GetById(customerId);
            var plan = _planRepository.GetByCode(normalizedPlanCode);

            EnsureCustomerIsActive(customer);

            decimal baseAmount = CalculateBaseAmount(plan, seatCount);

            var notes = new NotesBuilder();

            var discountResult = _discountCalculator.Calculate(customer, plan, baseAmount, seatCount, useLoyaltyPoints);
            notes.AddRange(discountResult.Notes);

            decimal subtotalAfterDiscount = ApplyMinimumDiscountedSubtotal(baseAmount - discountResult.Amount, notes);

            var supportFeeResult = _supportFeeCalculator.Calculate(normalizedPlanCode, includePremiumSupport);
            notes.AddRange(supportFeeResult.Notes);

            var paymentFeeResult = _paymentFeeCalculator.Calculate(normalizedPaymentMethod, subtotalAfterDiscount, supportFeeResult.Amount);
            notes.AddRange(paymentFeeResult.Notes);

            decimal taxRate = _taxRateProvider.GetTaxRate(customer.Country);
            decimal taxBase = subtotalAfterDiscount + supportFeeResult.Amount + paymentFeeResult.Amount;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = ApplyMinimumInvoiceAmount(taxBase + taxAmount, notes);

            var invoice = _invoiceFactory.Create(
                customerId,
                customer,
                normalizedPlanCode,
                normalizedPaymentMethod,
                seatCount,
                baseAmount,
                discountResult.Amount,
                supportFeeResult.Amount,
                paymentFeeResult.Amount,
                taxAmount,
                finalAmount,
                notes.ToString());

            _billingGateway.SaveInvoice(invoice);
            _notificationService.Send(customer, normalizedPlanCode, invoice);

            return invoice;
        }

        private static void EnsureCustomerIsActive(Customer customer)
        {
            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }
        }

        private static decimal CalculateBaseAmount(SubscriptionPlan plan, int seatCount)
        {
            return (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
        }

        private static decimal ApplyMinimumDiscountedSubtotal(decimal subtotalAfterDiscount, NotesBuilder notes)
        {
            if (subtotalAfterDiscount < 300m)
            {
                notes.Add("minimum discounted subtotal applied");
                return 300m;
            }

            return subtotalAfterDiscount;
        }

        private static decimal ApplyMinimumInvoiceAmount(decimal finalAmount, NotesBuilder notes)
        {
            if (finalAmount < 500m)
            {
                notes.Add("minimum invoice amount applied");
                return 500m;
            }

            return finalAmount;
        }
    }
}