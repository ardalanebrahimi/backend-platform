using System;

namespace Without_DIP
{
    #region Low-level modules
    // Concrete Implementations for Payment Gateways
    public class PayPalGateway
    {
        public void ProcessPayment(decimal amount)
        {
            // PayPal payment processing logic
        }
    }

    public class StripeGateway
    {
        public void ProcessPayment(decimal amount)
        {
            // Stripe payment processing logic
        }
    }

    // Concrete Implementations for Discount Calculators
    public class DiscountCalculator
    {
        public decimal CalculateDiscount(decimal subtotal)
        {
            // Default discount calculation logic
            return subtotal * 0.1m; // 10% discount for this example
        }
    }

    public class VIPDiscountCalculator
    {
        public decimal CalculateDiscount(decimal subtotal)
        {
            // VIP discount calculation logic
            return subtotal * 0.2m; // 20% discount for VIP customers
        }
    }
    #endregion

    #region High-level module
    // High-level module
    public class OrderProcessor
    {
        private object paymentGateway;
        private object discountCalculator;

        public OrderProcessor(string gatewayType, bool isVIP)
        {
            switch (gatewayType.ToLower())
            {
                case "paypal":
                    paymentGateway = new PayPalGateway();
                    break;
                case "stripe":
                    paymentGateway = new StripeGateway();
                    break;
                default:
                    throw new ArgumentException("Invalid gateway type");
            }

            discountCalculator = isVIP ? (object)new VIPDiscountCalculator() : (object)new DiscountCalculator();
        }

        public decimal CalculateTotal(decimal subtotal)
        {
            if (discountCalculator is DiscountCalculator)
            {
                ((DiscountCalculator)discountCalculator).CalculateDiscount(subtotal);
            }
            else if (discountCalculator is VIPDiscountCalculator)
            {
                ((VIPDiscountCalculator)discountCalculator).CalculateDiscount(subtotal);
            }
            decimal discount = ((DiscountCalculator)discountCalculator).CalculateDiscount(subtotal);
            return subtotal - discount;
        }

        public void ProcessPayment(decimal total)
        {
            if (paymentGateway is PayPalGateway)
            {
                ((PayPalGateway)paymentGateway).ProcessPayment(total);
            }
            else if (paymentGateway is StripeGateway)
            {
                ((StripeGateway)paymentGateway).ProcessPayment(total);
            }
        }
    }
    #endregion

    #region Usage
    public class Program
    {
        public static void Main()
        {
            // Create OrderProcessor with the desired payment gateway and discount calculator
            string gatewayType = "paypal"; // Choose "paypal" or "stripe"
            bool isVIP = true; // Set to true for VIP customer, false for regular
            OrderProcessor orderProcessor = new OrderProcessor(gatewayType, isVIP);

            // Process an order using the chosen payment gateway and discount calculator
            decimal subtotal = 100.0m;
            decimal total = orderProcessor.CalculateTotal(subtotal);

            orderProcessor.ProcessPayment(total);
        }
    }
    #endregion
}