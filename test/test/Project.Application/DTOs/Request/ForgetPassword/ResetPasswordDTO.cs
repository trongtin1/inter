using System.ComponentModel.DataAnnotations;

namespace test.Project.Application.DTOs.Request.ForgetPassword
{
    public class ResetPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "'ConfirmPassword' and 'NewPassword' do not match.")]
        public string ConfirmPassword { get; set; }
    }
}