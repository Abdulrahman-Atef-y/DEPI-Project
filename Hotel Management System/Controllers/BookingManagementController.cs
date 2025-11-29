using Buisness_Logic_Layer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_System.Controllers
{
        [Authorize(Roles = "Admin")]
        public class BookingManagementController : Controller
        {
            private readonly IUnitOfWork _unitOfWork;

            public BookingManagementController(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IActionResult> Index()
            {

                var allBookings = await _unitOfWork.BookingRepository.FindAllAsync(
                    criteria: null,
                    includes: new[] { "Guest", "Room" }
                );

                return View(allBookings.OrderByDescending(b => b.Date));
            }


            public async Task<IActionResult> ChangeStatus(int id, string status)
            {
                var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (booking == null) return NotFound();

                booking.Status = status;
                await _unitOfWork.BookingRepository.UpdateAsync(booking);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Booking #{id} status updated to {status}.";
                return RedirectToAction(nameof(Index));
            }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            await _unitOfWork.BookingRepository.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Booking #{id} cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
    }