using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

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

            public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? roomId)
            {

                var allBookings = await _unitOfWork.BookingRepository.FindAllAsync(
                    (Expression<Func<Booking, bool>>?)null,
                     new[] { "Guest", "Room" }
                );
            var filtered = allBookings.AsQueryable();

            if (startDate.HasValue)
                filtered = filtered.Where(b => b.Date >= startDate.Value);

            if (endDate.HasValue)
                filtered = filtered.Where(b => b.Date <= endDate.Value);

            if (roomId.HasValue)
                filtered = filtered.Where(b => b.RoomId == roomId.Value);

            ViewBag.Rooms = await _unitOfWork.RoomRepository.FindAllAsync((Expression<Func<Room, bool>>?)null,null);
            return View(filtered.OrderByDescending(b => b.Date));
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
            booking.Status = "Cancelled";
            await _unitOfWork.BookingRepository.UpdateAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Booking #{id} cancelled successfully.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            await _unitOfWork.BookingRepository.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Booking #{id} deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

    }
}