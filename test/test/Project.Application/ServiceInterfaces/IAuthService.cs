
using test.Project.Application.DTOs.Request;
using test.Project.Application.DTOs.Request.ForgetPassword;
using Microsoft.AspNetCore.Mvc;

namespace test.Project.Application.ServiceInterfaces
{
    public interface IAuthService
    {
        Task<IActionResult> LoginAsync(UserLoginDTO userLogin);
        Task<IActionResult> LogoutAsync();
        Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDTO model);
        Task<IActionResult> VerifyOTPAsync(VerifyOTPDTO model);
        Task<IActionResult> ResetPasswordAsync(ResetPasswordDTO model);
    }
} 