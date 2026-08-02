// Handles staff login. Verifies credentials, returns a JWT.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SmartDesk.Api.Data;


namespace SmartDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly SmartDeskContext _context;
    private readonly IConfiguration _config;
    public AuthController (SmartDeskContext context, IConfiguration config)
    {
        _config = config;
        _context = context;
    }
    //POST/api/auth/login
    [HttpPost("login")]
    public async Task <IActionResult> Login ([FromBody] LoginRequest request)
    {
        var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);   
    
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
                return Unauthorized(new { message = "Invalid email or password" });
        }
        
        // HTTP is stateless — the server forgets user after each request.
        // The token proves who particular user are on every future request.
        var token = GenerateToken(user.Email, user.Role);
        return Ok(new
            {
                token = token,
                email = user.Email,
                role = user.Role
            });

    }
    

    private string GenerateToken(string email, string role)
    {
        // Uses the same secret key to sign AND verify the token.
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        // HMAC-SHA256 is the standard JWT signing algorithm.
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims =new[]
        {
            new Claim(ClaimTypes.Email,email),
            new Claim(ClaimTypes.Role,role),
        };
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                double.Parse(_config["Jwt:ExpiryHours"]!)
            ),
            signingCredentials: credentials
        );
         return new JwtSecurityTokenHandler().WriteToken(token);
    }
    // TEMPORARY — delete after getting the hash
[HttpGet("hash")]
public IActionResult GetHash()
{
    var hash = BCrypt.Net.BCrypt.HashPassword("password123");
    return Ok(new { hash });
}
}

// Defines exactly what the login endpoint accepts.
public class LoginRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}