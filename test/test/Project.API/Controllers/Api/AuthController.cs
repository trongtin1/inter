using Microsoft.AspNetCore.Mvc;
using test.Project.Application.DTOs.Request;
using test.Project.Application.DTOs.Request.ForgetPassword;
using test.Project.Application.ServiceInterfaces;

namespace test.Project.API.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDTO userLogin)
        {
            return await _authService.LoginAsync(userLogin);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return await _authService.LogoutAsync();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            return await _authService.ForgotPasswordAsync(model);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDTO model)
        {
            return await _authService.VerifyOTPAsync(model);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            return await _authService.ResetPasswordAsync(model);
        }
    }
}   