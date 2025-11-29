using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Hotel_Management_System.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_System.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<Guest> _userManager;

        public BookingController(IUnitOfWork unitOfWork, UserManager<Guest> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(roomTypeId);
            if (roomType == null) return NotFound();

            var dto = new BookingDTO
            {
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Name,
                PricePerNight = roomType.Price,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingDTO bookingDto)
        {
            if (!ModelState.IsValid) return View(bookingDto);

            if (bookingDto.CheckInDate >= bookingDto.CheckOutDate)
            {
                ModelState.AddModelError("", "Check-out date must be after check-in date.");
                return View(bookingDto);
            }

            var availableRooms = await _unitOfWork.RoomRepository.GetAvailableRoomsAsync(
                bookingDto.CheckInDate,
                bookingDto.CheckOutDate,
                bookingDto.RoomTypeId
            );

            var selectedRoom = availableRooms.FirstOrDefault();

            if (selectedRoom == null)
            {
                ModelState.AddModelError("", "Sorry, no rooms of this type are available for your selected dates.");
                return View(bookingDto);
            }

            int nights = (bookingDto.CheckOutDate - bookingDto.CheckInDate).Days;
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(bookingDto.RoomTypeId);
            decimal totalPrice = (decimal)roomType.Price * nights;

            var user = await _userManager.GetUserAsync(User);

            var booking = new Booking
            {
                GuestId = user.Id,
                RoomId = selectedRoom.Id,
                CheckInDate = bookingDto.CheckInDate,
                CheckOutDate = bookingDto.CheckOutDate,
                TotalPrice = totalPrice,
                Status = "Pending",
                Date = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync(); // Save to DB

            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToAction(nameof(MyBookings));
        }

        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var allBookings = await _unitOfWork.BookingRepository.FindAllAsync(
                criteria: b => b.GuestId == user.Id,
                includes: new[] { "Room" }
            );

            return View(allBookings.OrderByDescending(b => b.Date));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            await _unitOfWork.BookingRepository.DeleteAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم حذف الحجز بنجاح!";
            return RedirectToAction(nameof(MyBookings));
        }
    }
}