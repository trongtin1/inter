using Microsoft.AspNetCore.Mvc;

namespace test.Controllers.Page
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 