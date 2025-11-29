using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "التقارير والتحليلات";
            return View();
        }
    }
}
