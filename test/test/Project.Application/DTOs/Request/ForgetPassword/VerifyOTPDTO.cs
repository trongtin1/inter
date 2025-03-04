using System.ComponentModel.DataAnnotations;

namespace test.Project.Application.DTOs.Request.ForgetPassword
{
    public class VerifyOTPDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        public string OTP { get; set; }
    }
}