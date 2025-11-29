using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Buisness_Logic_Layer.Interfaces;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using Hotel_Management_System.Models.ViewModels;

namespace Hotel_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] //uncomment later when Login is working
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Guest> _userManager;
        public AdminController(IUnitOfWork unitOfWork, UserManager<Guest> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var bookings = await _unitOfWork.BookingRepository.GetAllAsync();
            var rooms = await _unitOfWork.RoomRepository.GetAllAsync();
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();
            var model = new AdminDashboardViewModel
            {
                TotalRooms = rooms.Count(),
                TotalBookings = bookings.Count(),
                TotalUsers = _userManager.Users.Count(),
                TotalRevenue = bookings.Where(b => b.Status != "Cancelled").Sum(b => b.TotalPrice),
                RecentBookings = await _unitOfWork.BookingRepository.FindAllAsync(
                    take: 5,
                    orderBy: b => b.Date,
                    isDesc: true,
                    includes: new[] { "Guest", "Room" }
                    )
                };
            var bookingGroups = bookings
                .GroupBy(b => b.Date.Month)
                .ToDictionary(g => g.Key, g => g.Count());
            for (int i = 1; i <= 12; i++)
            {
                model.MonthlyBookings[i - 1] = bookingGroups.ContainsKey(i) ? bookingGroups[i] : 0;
            }
            model.RoomTypeLabels = roomTypes.Select(rt => rt.Name).ToArray();
            model.RoomTypeCounts = roomTypes.Select(rt => rooms.Count(r => r.RoomTypeId == rt.Id)).ToArray();

            return View(model);
        }
    }
}
