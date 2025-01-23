using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using test.Services;
using test.Models.DTOs.Request;
using test.Models.DTOs;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // [HttpPost("register")]
    // public async Task<IActionResult> Register([FromBody]UserRegisterDTO model)
    // {
    //     if (!ModelState.IsValid)
    //         return BadRequest(ModelState);

    //     if (await _context.User.AnyAsync(u => u.Username == model.Username))
    //     {
    //         return BadRequest(new { message = "Username already exists" });
    //     }

    //     var user = new User
    //     {
    //         Username = model.Username,
    //         Password = BCrypt.Net.BCrypt.HashPassword(model.Password)
    //     };

    //     _context.User.Add(user);
    //     await _context.SaveChangesAsync();

    //     return Ok(new { message = "User registered successfully" });
    // }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]UserLoginDTO userLogin)
    {
        var user = await _context.User
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == userLogin.Username);

        if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
        {
            var token = GenerateJwtToken(user);
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Login Success",
                Data = new { Token = token }
            });
        }

        return Unauthorized(new ApiResponse<object>
        {
            Success = false,
            Message = "Invalid username or password",
            Data = null
        });
    }

    
    // [HttpGet("test")]
    // public IActionResult Test()
    // {
    //     return Ok(new { message = "You are authorized!", user = User.Identity.Name });
    // }

    private string GenerateJwtToken(User user)
    {   
        var claims = new List<Claim>
        {   
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email ?? ""),
            new Claim("username", user.Username ?? ""),
            new Claim("roles", string.Join(",", user.UserRoles?.Select(ur => ur.Role?.Name) ?? Array.Empty<string>()))
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var securityTokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(1),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(securityTokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}