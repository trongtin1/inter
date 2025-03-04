using Microsoft.AspNetCore.Mvc;

namespace test.Project.API.Controllers.Page
{
    public class StatisticsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 