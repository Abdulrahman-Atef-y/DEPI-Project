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

            var user = await _userManager.GetUserAsync(User);
            
            var email = user?.Email;

            var dto = new BookingDTO
            {
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Name,
                PricePerNight = roomType.Price,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1),
                Occupancy = roomType.Occupancy,
                Email = email,
                Amenities = string.IsNullOrEmpty(roomType.RoomAmenities)
                    ? new List<string>()
                    : roomType.RoomAmenities.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList()
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
                Date = DateTime.UtcNow,
                BookingGuests = new List<BookingGuest>()
            };

            if (bookingDto.BookingGuests != null)
            {
                foreach (var guestDto in bookingDto.BookingGuests)
                {
                    var guest = new BookingGuest
                    {
                        FirstName = guestDto.FirstName,
                        LastName = guestDto.LastName,
                        SSN = guestDto.SSN,   // or SSN property
                        Booking = booking
                    };

                    booking.BookingGuests.Add(guest);
                }
            }

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToAction(nameof(MyBookings));
        }

        public async Task<IActionResult> MyBookings()
        {
            var user = await _userManager.GetUserAsync(User);
            var allBookings = await _unitOfWork.BookingRepository.FindAllAsync(
                criteria: b => b.GuestId == user.Id,
                includes: new[] { "Room", "Room.RoomType", "Guest" } // include nested RoomType
            );

            return View(allBookings.OrderByDescending(b => b.Date));
        }


        [HttpGet]
        public async Task<IActionResult> BookingDetails(int id)
        {
            // 1. Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            // 2. Retrieve the booking from the database
            // We search by Booking ID AND GuestId to ensure the user owns this booking.
            // We use "Includes" to get the Room, RoomType, and Guest list data.
            var bookingsList = await _unitOfWork.BookingRepository.FindAllAsync(
                criteria: b => b.Id == id && b.GuestId == user.Id,
                includes: new[] { "Room", "Room.RoomType", "Guest", "BookingGuests" }
            );

            var booking = bookingsList.FirstOrDefault();

            // 3. If booking not found or doesn't belong to user, return 404
            if (booking == null)
            {
                return NotFound();
            }

            // 4. Map the Entity (Booking) to the DTO (BookingDTO)
            // This is necessary because your View expects 'BookingDTO'
            var dto = new BookingDTO
            {
                // Basic Info
                BookingId = booking.Id, // Ensure your DTO has an ID property if you need to print the booking #
                Email = booking.Guest.Email,
                phonenumber = booking.Guest.PhoneNumber,

                // Room Info
                RoomTypeId = booking.Room.RoomType.Id,
                RoomTypeName = booking.Room.RoomType.Name,
                PricePerNight = booking.Room.RoomType.Price,
                Occupancy = booking.Room.RoomType.Occupancy,

                // Dates
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,

                // Calculate Total Price (or map it if it's stored in DB)
                // If your DTO has a TotalPrice property:
                TotalPrice = booking.TotalPrice,

                // Map the list of Guests
                NumberOfGuests = booking.BookingGuests.Count,
                BookingGuests = booking.BookingGuests.Select(bg => new BookingGuestDTO
                {
                    FirstName = bg.FirstName,
                    LastName = bg.LastName,
                    SSN = bg.SSN
                }).ToList()
            };

            return View(dto);
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