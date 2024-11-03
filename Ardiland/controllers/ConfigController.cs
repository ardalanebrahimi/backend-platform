using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Ardiland.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("paypal-client-id")]
        public IActionResult GetPayPalClientId()
        {
            var clientId = _configuration["PaymentProviders:PayPalClientId"];
            return Ok(new { clientId });
        }

        [HttpGet("stripe-public-key")]
        public IActionResult GetStripePublicKey()
        {
            var publicKey = _configuration["PaymentProviders:StripePublicKey"];
            return Ok(new { publicKey });
        }
    }

}
