using Microsoft.AspNetCore.Mvc;

namespace test.Project.API.Controllers.Page
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult VerifyOTP()
        {
            return View();
        }
        public IActionResult ResetPassword()
        {
            return View();
        }
    }
} 