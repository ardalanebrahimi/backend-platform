using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.Threading.Tasks;

namespace Ardiland.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public PaymentController(IConfiguration configuration)
    {
        StripeConfiguration.ApiKey = configuration["PaymentProviders:StripeApiKey"];
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
}

public class PaymentIntentRequest
{
    public long Amount { get; set; }
}
