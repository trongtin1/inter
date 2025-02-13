using Microsoft.AspNetCore.Mvc;

namespace test.Controllers.Page
{
    public class UsersController : Controller
    {   
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Edit()
        {
            return View();
        }
    }
} 