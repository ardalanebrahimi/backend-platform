// Controllers/CustomerAuthController.cs
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Ardiland.Entities;
using PayPalCheckoutSdk.Orders;
using System.Threading.Tasks;
using Ardiland.Data;
using Ardiland.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public CustomerController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        // Check if customer with provided email exists
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == loginRequest.Email);
        if (customer == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Verify the password
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, customer.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        // Generate JWT token
        var token = GenerateJwtToken(customer);

        return Ok(new { AccessToken = token });
    }

    private string GenerateJwtToken(Customer customer)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, customer.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtIssuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
