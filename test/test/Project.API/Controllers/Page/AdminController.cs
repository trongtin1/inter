using Microsoft.AspNetCore.Mvc;

namespace test.Project.API.Controllers.Page
{
    public class AdminController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }
        
    }
} 