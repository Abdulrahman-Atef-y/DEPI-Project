using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_System.Controllers
{
    public class StaticPagesController : Controller
    {
        public IActionResult AboutUs()
        {
            ViewData["Title"] = "About Us";
            return View();
        }

        public IActionResult CustomerSupport()
        {
            ViewData["Title"] = "Customer Support";
            return View();
        }

        public IActionResult Policies()
        {
            ViewData["Title"] = "Policies";
            return View();
        }
    }
}
