using Microsoft.Extensions.Caching.Memory;
using test.Project.Application.DTOs;
using test.Project.Application.DTOs.Request;
using test.Project.Application.DTOs.Request.ForgetPassword;
using test.Project.Application.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;

namespace test.Project.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ISmtpService _smtpService;
        private readonly IMemoryCache _cache;

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            ISmtpService smtpService,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _smtpService = smtpService;
            _cache = cache;
        }

        public async Task<IActionResult> LoginAsync(UserLoginDTO userLogin)
        {
            var user = await _unitOfWork.AuthRepository.GetUserByUsernameAsync(userLogin.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                var token = _tokenService.GenerateJwtToken(user);
                return new OkObjectResult(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Login Success",
                    Data = new { Token = token }
                });
            }

            return new BadRequestObjectResult(new ApiResponse<object>
            {
                Success = false,
                Message = "Invalid username or password",
                Data = null
            });
        }

        public async Task<IActionResult> LogoutAsync()
        {
            return new OkObjectResult(new ApiResponse<object>
            {
                Success = true,
                Message = "Logout successful",
                Data = null
            });
        }

        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDTO model)
        {
            var user = await _unitOfWork.AuthRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return new NotFoundObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email not found",
                    Data = null
                });
            }

            string otp = GenerateOTP();
            _cache.Set($"OTP_{model.Email}", otp, TimeSpan.FromMinutes(15));

            await _smtpService.SendEmailAsync(
                model.Email,
                "Reset Password OTP",
                $"Your OTP for password reset is: {otp}. This code will expire in 15 minutes."
            );

            return new OkObjectResult(new ApiResponse<object>
            {
                Success = true,
                Message = "OTP has been sent to your email",
                Data = null
            });
        }

        public async Task<IActionResult> VerifyOTPAsync(VerifyOTPDTO model)
        {
            var user = await _unitOfWork.AuthRepository.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                return new NotFoundObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email not found",
                    Data = null
                });
            }

            var storedOTP = _cache.Get<string>($"OTP_{model.Email}");
            if (storedOTP != model.OTP)
            {
                return new BadRequestObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid or expired OTP",
                    Data = null
                });
            }

            _cache.Set($"ResetPassword_{model.Email}", user.Id, TimeSpan.FromHours(1));
            _cache.Remove($"OTP_{model.Email}");

            return new OkObjectResult(new ApiResponse<object>
            {
                Success = true,
                Message = "OTP verified successfully",
                Data = null
            });
        }

        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDTO model)
        {
            if (!_cache.TryGetValue($"ResetPassword_{model.Email}", out int userId))
            {
                return new BadRequestObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Reset password session expired",
                    Data = null
                });
            }

            var user = await _unitOfWork.AuthRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found",
                    Data = null
                });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            await _unitOfWork.AuthRepository.UpdateUserAsync(user);
            _cache.Remove($"ResetPassword_{model.Email}");

            await _unitOfWork.SaveChangesAsync();
            return new OkObjectResult(new ApiResponse<object>
            {
                Success = true,
                Message = "Password has been reset successfully",
                Data = null
            });
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
} 