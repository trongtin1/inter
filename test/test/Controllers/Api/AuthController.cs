using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test.Models;
using test.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using test.Services;
using test.Models.DTOs.Request;
using test.Models.DTOs;
namespace test.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase

{
    private readonly ApplicationDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(ApplicationDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
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
            var token = _tokenService.GenerateJwtToken(user);
           
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
}
}   