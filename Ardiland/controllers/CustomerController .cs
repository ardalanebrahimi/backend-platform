// Controllers/CustomerAuthController.cs
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Ardiland.Entities;
using PayPalCheckoutSdk.Orders;
using System.Threading.Tasks;
using Ardiland.Data;
using Ardiland.Models;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;

    public CustomerController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterCustomerRequest request)
    {
        // Check if the username or email already exists
        if (_context.Customers.Any(c => c.Email == request.Email))
        {
            return BadRequest("Username or email already exists.");
        }

        // Hash the password
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Create and save the new customer
        var newCustomer = new Customer
        {
            Name = request.Name,
            Email = request.Email,
            Address = request.Address,
            Phone = request.Phone,
            PasswordHash = passwordHash
        };

        _context.Customers.Add(newCustomer);
        await _context.SaveChangesAsync();

        return Ok("Customer registration successful");
    }
}
