namespace Hotel_Management_System.Controllers
{
    using Data_Access_Layer.Entities;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    namespace Hotel_Management_System.Controllers
    {
        [Authorize(Roles = "Admin")]
        public class CustomerController : Controller
        {
            private readonly UserManager<Guest> _userManager;

            public CustomerController(UserManager<Guest> userManager)
            {
                _userManager = userManager;
            }

            public IActionResult Index()
            {
                var users = _userManager.Users.ToList();
                return View(users);
            }

            public async Task<IActionResult> Delete(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                if (user.UserName == User.Identity.Name)
                {
                    TempData["ErrorMessage"] = "Cannot delete the currently logged-in user.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Customer deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error deleting user: {result.Errors.First().Description}";
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
