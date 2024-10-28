using System;

namespace With_DIP
{
    #region Abstractions (Interfaces)
    public interface IPaymentGateway
    {
        void ProcessPayment(decimal amount);
    }

    public interface IDiscountCalculator
    {
        decimal CalculateDiscount(decimal subtotal);
    }
    #endregion

    #region Concrete Implementations

    // Concrete Implementations for Payment Gateways
    public class PayPalGateway : IPaymentGateway
    {
        public void ProcessPayment(decimal amount)
        {
            // PayPal payment processing logic
        }
    }

    public class StripeGateway : IPaymentGateway
    {
        public void ProcessPayment(decimal amount)
        {
            // Stripe payment processing logic
        }
    }

    // Concrete Implementations for Discount Calculators
    public class DiscountCalculator : IDiscountCalculator
    {
        public decimal CalculateDiscount(decimal subtotal)
        {
            // Default discount calculation logic
            return subtotal * 0.1m; // 10% discount for this example
        }
    }

    public class VIPDiscountCalculator : IDiscountCalculator
    {
        public decimal CalculateDiscount(decimal subtotal)
        {
            // VIP discount calculation logic
            return subtotal * 0.2m; // 20% discount for VIP customers
        }
    }
    #endregion

    #region Factory
    // Factory to create instances of payment gateway and discount calculator
    public class PaymentFactory
    {
        public static IPaymentGateway CreatePaymentGateway(string gatewayType)
        {
            switch (gatewayType.ToLower())
            {
                case "paypal":
                    return new PayPalGateway();
                case "stripe":
                    return new StripeGateway();
                default:
                    throw new ArgumentException("Invalid gateway type");
            }
        }
    }

    public class DiscountFactory
    {
        public static IDiscountCalculator CreateDiscountCalculator(bool isVIP)
        {
            return isVIP ? new VIPDiscountCalculator() : new DiscountCalculator();
        }
    }

    #endregion

    #region High-level module
    // Separate classes for Order processing and payment processing
    public class OrderProcessor
    {
        private IDiscountCalculator discountCalculator;

        public OrderProcessor(IDiscountCalculator discountCalculator)
        {
            this.discountCalculator = discountCalculator;
        }

        public decimal CalculateTotal(decimal subtotal)
        {
            decimal discount = discountCalculator.CalculateDiscount(subtotal);
            return subtotal - discount;
        }
    }

    public class OrderPaymentProcessor
    {
        private IPaymentGateway paymentGateway;

        public OrderPaymentProcessor(IPaymentGateway paymentGateway)
        {
            this.paymentGateway = paymentGateway;
        }

        public void ProcessPayment(decimal total)
        {
            paymentGateway.ProcessPayment(total);
        }
    }

    #endregion

    #region Usage
    public class Program
    {
        public static void Main()
        {
            // Create instances using the factory
            string gatewayType = "paypal"; // Choose "paypal" or "stripe"
            IPaymentGateway paymentGateway 
                = PaymentFactory.CreatePaymentGateway(gatewayType);
            bool isVIP = true; // Set to true for VIP customer, false for regular
            IDiscountCalculator discountCalculator 
                = DiscountFactory.CreateDiscountCalculator(isVIP);

            // Process an order using the chosen payment gateway and discount calculator
            OrderProcessor orderProcessor = new OrderProcessor(discountCalculator);
            decimal subtotal = 100.0m;
            decimal total = orderProcessor.CalculateTotal(subtotal);

            OrderPaymentProcessor paymentProcessor = new OrderPaymentProcessor(paymentGateway);
            paymentProcessor.ProcessPayment(total);
        }
    }
    #endregion
}