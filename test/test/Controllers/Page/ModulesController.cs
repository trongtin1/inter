using Microsoft.AspNetCore.Mvc;

namespace test.Controllers.Page
{
    public class ModulesController : Controller
    {   
        public IActionResult Index()
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