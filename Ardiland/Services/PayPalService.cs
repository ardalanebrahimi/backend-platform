using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class PayPalService
{
    private readonly IConfiguration _configuration;
    private readonly PayPalHttpClient _client;

    public PayPalService(IConfiguration configuration)
    {
        var environment = new SandboxEnvironment(
            configuration["PaymentProviders:PayPalClientId"],
            configuration["PaymentProviders:PayPalClientSecret"]
        );
        _client = new PayPalHttpClient(environment);
    }

    public async Task<string> CreateOrder(decimal amount)
    {
        var order = new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "EUR",
                        Value = amount.ToString()
                    }
                }
            }
        };

        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(order);

        var response = await _client.Execute(request);
        var result = response.Result<Order>();

        return result.Id; // Return PayPal order ID
    }

    public async Task<bool> CaptureOrder(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest());

        var response = await _client.Execute(request);
        return response.StatusCode == System.Net.HttpStatusCode.Created;
    }
}
