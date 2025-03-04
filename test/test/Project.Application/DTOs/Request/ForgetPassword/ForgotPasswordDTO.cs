using System.ComponentModel.DataAnnotations;

namespace test.Project.Application.DTOs.Request.ForgetPassword
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}