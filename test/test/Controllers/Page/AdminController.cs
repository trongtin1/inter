using Microsoft.AspNetCore.Mvc;

namespace test.Controllers.Page
{
    public class AdminController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }
        
    }
} 