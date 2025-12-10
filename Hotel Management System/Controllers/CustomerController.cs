    using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
using Hotel_Management_System.Models.DTOs;

namespace Hotel_Management_System.Controllers
    {
        [Authorize(Roles = "Admin")]
        public class CustomerController : Controller
        {
            private readonly UserManager<Guest> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;


            public CustomerController(UserManager<Guest> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;

            }

            public async Task<IActionResult> Index()
            {
                var users = _userManager.Users.ToList();
            var admins = new List<CustomerDTO>();

            var customers = new List<CustomerDTO>();
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Customer";

                var dto = new CustomerDTO
                {
                    Id = user.Id,
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    Role = role
                };
                if (role == "Admin")
                    admins.Add(dto);
                else
                    customers.Add(dto);
            }
            ViewBag.Admins = admins;
            ViewBag.Customers = customers;
            return View();
            }
        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != "Manager").Select(r => r.Name).ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != "Manager").Select(r => r.Name).ToList();
                return View(dto);
            }

            var user = new Guest
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Email = dto.Email,
                UserName = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != "Manager").Select(r => r.Name).ToList();
                return View(dto);
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            TempData["SuccessMessage"] = "تم إضافة العميل بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "Customer";

            var dto = new CustomerEditDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Role = role
            };

            ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != "Manager").Select(r => r.Name).ToList();
            return View(dto);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerEditDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _roleManager.Roles.Where(r => r.Name != "Manager").Select(r => r.Name).ToList();
                return View(dto);
            }

            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) return NotFound();
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Address = dto.Address;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.DateOfBirth = dto.DateOfBirth;
            user.Gender = dto.Gender;

            var updateUser = await _userManager.UpdateAsync(user);
            if (!updateUser.Succeeded)
            {
                ModelState.AddModelError("", updateUser.Errors.First().Description);
                return View(dto);
            }
            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);

            TempData["SuccessMessage"] = "تم تعديل البيانات بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(string id)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                if (user.UserName == User.Identity.Name)
                {
                    TempData["ErrorMessage"] = "لا يمكنك حذف حسابك الشخصي.";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "تم حذف العميل بنجاح.";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Error deleting user: {result.Errors.First().Description}";
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }
