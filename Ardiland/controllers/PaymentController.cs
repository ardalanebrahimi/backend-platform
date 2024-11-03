using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Core;
using Stripe;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ardiland.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly PayPalService _payPalService;
    public PaymentController(IConfiguration configuration, PayPalService payPalService)
    {
        StripeConfiguration.ApiKey = configuration["PaymentProviders:StripeApiKey"];
        _payPalService = payPalService;
    }

    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] PaymentIntentRequest request)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount, // Amount in cents
            Currency = "usd",
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
        };

        var service = new PaymentIntentService();
        var paymentIntent = await service.CreateAsync(options);

        return Ok(new { clientSecret = paymentIntent.ClientSecret });
    }

    [HttpPost("create-paypal-order")]
    public async Task<IActionResult> CreatePayPalOrder([FromBody] PayPalOrderRequest request)
    {
        var orderId = await _payPalService.CreateOrder(request.Amount);
        return Ok(new { orderId });
    }

    [HttpPost("capture-paypal-order")]
    public async Task<IActionResult> CapturePayPalOrder([FromBody] PayPalCaptureRequest request)
    {
        var orderId = request.OrderId;
        // Capture order logic here
        var success = await _payPalService.CaptureOrder(orderId);
        return success ? Ok() : StatusCode(500, "Payment capture failed");
    }
}

public class PaymentIntentRequest
{
    public long Amount { get; set; }
}
public class PayPalOrderRequest
{
    public decimal Amount { get; set; }
}
public class PayPalCaptureRequest
{
    public string OrderId { get; set; }
}

