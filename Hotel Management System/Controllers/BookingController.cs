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

            var paymentObject = await _unitOfWork.PaymentRepository.FindAsync(p => p.BookingId == booking.Id);


            // 4. Map the Entity (Booking) to the DTO (BookingDTO)
            // This is necessary because your View expects 'BookingDTO'
            var dto = new BookingPaymentDTO
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
                }).ToList(),
                paymentID=paymentObject.Id,
                PaymentStatus=paymentObject.PaymentStatus

                
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


        ////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////

        #region old create
        //public async Task<IActionResult> Create(int roomTypeId)
        //{
        //    var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(roomTypeId);
        //    if (roomType == null) return NotFound();

        //    var user = await _userManager.GetUserAsync(User);

        //    var email = user?.Email;

        //    var dto = new BookingDTO
        //    {
        //        RoomTypeId = roomType.Id,
        //        RoomTypeName = roomType.Name,
        //        PricePerNight = roomType.Price,
        //        CheckInDate = DateTime.Today,
        //        CheckOutDate = DateTime.Today.AddDays(1),
        //        Occupancy = roomType.Occupancy,
        //        Email = email,
        //        Amenities = string.IsNullOrEmpty(roomType.RoomAmenities)
        //            ? new List<string>()
        //            : roomType.RoomAmenities.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList(),

        //    };
        //    return View(dto);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create(BookingDTO bookingDto)
        //{
        //    if (!ModelState.IsValid) return View(bookingDto);

        //    if (bookingDto.CheckInDate >= bookingDto.CheckOutDate)
        //    {
        //        ModelState.AddModelError("", "Check-out date must be after check-in date.");
        //        return View(bookingDto);
        //    }

        //    var availableRooms = await _unitOfWork.RoomRepository.GetAvailableRoomsAsync(
        //        bookingDto.CheckInDate,
        //        bookingDto.CheckOutDate,
        //        bookingDto.RoomTypeId
        //    );

        //    var selectedRoom = availableRooms.FirstOrDefault();

        //    if (selectedRoom == null)
        //    {
        //        ModelState.AddModelError("", "Sorry, no rooms of this type are available for your selected dates.");
        //        return View(bookingDto);
        //    }




        //    int nights = (bookingDto.CheckOutDate - bookingDto.CheckInDate).Days;
        //    var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(bookingDto.RoomTypeId);
        //    decimal totalPrice = (decimal)roomType.Price * nights;

        //    var user = await _userManager.GetUserAsync(User);

        //    var booking = new Booking
        //    {
        //        GuestId = user.Id,
        //        RoomId = selectedRoom.Id,
        //        CheckInDate = bookingDto.CheckInDate,
        //        CheckOutDate = bookingDto.CheckOutDate,
        //        TotalPrice = totalPrice,
        //        Status = "Pending",
        //        Date = DateTime.UtcNow,
        //        BookingGuests = new List<BookingGuest>()
        //    };

        //    if (bookingDto.BookingGuests != null)
        //    {
        //        foreach (var guestDto in bookingDto.BookingGuests)
        //        {
        //            var guest = new BookingGuest
        //            {
        //                FirstName = guestDto.FirstName,
        //                LastName = guestDto.LastName,
        //                SSN = guestDto.SSN,   // or SSN property
        //                Booking = booking
        //            };

        //            booking.BookingGuests.Add(guest);
        //        }
        //    }
        //    ///////////////////////////////////////////////////////new ////////////////////////////

        //    await _unitOfWork.BookingRepository.AddAsync(booking);
        //    await _unitOfWork.SaveChangesAsync();


        //    var payment = new Payment
        //    {
        //        BookingId = booking.Id,
        //        Amount = booking.TotalPrice,
        //        PaymentStatus = "Pending",
        //        PaymentMethod = "CreditCard", // Or whatever the user selects
        //        PaymentDate = DateTime.UtcNow
        //    };

        //    await _unitOfWork.PaymentRepository.AddAsync(payment);
        //    await _unitOfWork.SaveChangesAsync();



        //    //TempData["SuccessMessage"] = "Booking confirmed successfully!";


        //    // Redirect to payment page
        //    return RedirectToAction("Payment", new { paymentId = payment.Id });
        //    /////////////////////////////////////////////////////////////////////////////////////////////



        //    //await _unitOfWork.BookingRepository.AddAsync(booking);
        //    //await _unitOfWork.SaveChangesAsync();

        //    //TempData["SuccessMessage"] = "Booking confirmed successfully!";
        //    //return RedirectToAction(nameof(MyBookings));
        //}


        #endregion



        [HttpGet]
        public async Task<IActionResult> Create(int roomTypeId)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(roomTypeId);
            if (roomType == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var dto = new BookingDTO
            {
                RoomTypeId = roomType.Id,
                RoomTypeName = roomType.Name,
                PricePerNight = roomType.Price,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1),
                Occupancy = roomType.Occupancy,
                Email = user?.Email,
                Amenities = string.IsNullOrEmpty(roomType.RoomAmenities)
                             ? new List<string>()
                            : roomType.RoomAmenities.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList(),
            };

            return View(dto);
        }


        // ---------------------------------------------------
        // STEP 2: User submits form → Check availability
        // ---------------------------------------------------
        [HttpPost]
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
            ///////////////////////////////////////////////////////new ////////////////////////////

            await _unitOfWork.BookingRepository.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();


            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = booking.TotalPrice,
                PaymentStatus = "Pending",
                PaymentMethod = "CreditCard", // Or whatever the user selects
                PaymentDate = DateTime.UtcNow
            };

            await _unitOfWork.PaymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();



            //TempData["SuccessMessage"] = "Booking confirmed successfully!";


            // Redirect to payment page
            return RedirectToAction("Payment", new { paymentId = payment.Id });
            /////////////////////////////////////////////////////////////////////////////////////////////



            //await _unitOfWork.BookingRepository.AddAsync(booking);
            //await _unitOfWork.SaveChangesAsync();

            //TempData["SuccessMessage"] = "Booking confirmed successfully!";
            //return RedirectToAction(nameof(MyBookings));
        }























        // ---------------------------------------------------
        // STEP 3: Payment page with countdown timer
        // ---------------------------------------------------





        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> Payment(int paymentId)
        {
            // 1. Get the payment by ID
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
            {
                TempData["Error"] = "Payment not found.";
                return RedirectToAction("Create");
            }

            // 2. Load the booking with room and room type info
            var booking = await _unitOfWork.BookingRepository
                .GetBookingWithRoomAndTypeAsync(payment.BookingId);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("Create");
            }

            var roomTypeName = booking.Room?.RoomType?.Name ?? "Unknown Room Type";
            var pricePerNight = booking.Room?.RoomType?.Price ?? 0;

            // 3. Map to DTO for the view
            var paymentDto = new PaymentDTO
            {
                PaymentId = payment.Id,
                BookingId = booking.Id,
                Amount = payment.Amount,
                PaymentStatus = payment.PaymentStatus ?? "Pending",
                PaymentMethod = payment.PaymentMethod ?? "CreditCard",
                PaymentDate = payment.PaymentDate,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                RoomTypeName = roomTypeName,
                PricePerNight = pricePerNight
            };

            // 4. Return the payment view
            return View(paymentDto);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(PaymentDTO paymentDto)
        {
            // 1. Manual Validation Logic
            if (paymentDto.PaymentMethod == "CreditCard")
            {
                // Check if fields are empty
                if (string.IsNullOrWhiteSpace(paymentDto.CardNumber) ||
                    string.IsNullOrWhiteSpace(paymentDto.CVV) ||
                    string.IsNullOrWhiteSpace(paymentDto.Expiry))
                {
                    ModelState.AddModelError("", "Please fill in all credit card details.");
                }
            }

            // 2. Check ModelState (This now checks basic fields like Dates, but skips Card validation if empty)
            if (!ModelState.IsValid)
            {
                return View(paymentDto);
            }

            

            // 3. Payment Processing Logic
            if (paymentDto.PaymentMethod == "CreditCard")
            {
                // CLEAN THE INPUT: Remove spaces and dashes
                string cleanCardNumber = paymentDto.CardNumber?.Replace(" ", "").Replace("-", "") ?? "";
                string cleanExpiry = paymentDto.Expiry?.Replace("/", "") ?? ""; // Assuming you compare raw or standard format

                // Define Test Credentials (Clean format)
                // Note: I removed dashes from testCardNumber to match the cleaned input
                string testCardNumber = "1111222233334444";
                string testCardCVV = "123";
                string testCardExpiry = "12/30"; // Keep slash if your JS sends it with slash

                bool isCardValid = cleanCardNumber == testCardNumber &&
                                   paymentDto.CVV == testCardCVV &&
                                   paymentDto.Expiry == testCardExpiry;

                if (!isCardValid)
                {
                    // Unlock room logic...
                    ModelState.AddModelError("", "Invalid credit card details (Test: 1111 2222 3333 4444, 123, 12/30)");
                    return View(paymentDto);
                }
            }

            // 4. Success -> Create Booking
            // (Your existing code to save the booking goes here)
            // Ensure you handle the "Cash" status correctly (e.g., set Status = "Pending Payment" instead of "Confirmed" if cash)

            // A. Get the existing Payment object using the ID from the form
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(paymentDto.PaymentId);

            if (payment == null)
            {
                // Fallback: If ID is 0 or not found, we cannot update it. 
                TempData["Error"] = "Payment record not found.";
                return RedirectToAction("Create");
            }

            // B. Get the Room to ensure the price is correct (Security check)
            // We assume the Payment object or the DTO has the RoomId
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(paymentDto.BookingId);
            
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(booking.RoomId);

            // C. Calculate the Total Amount
            var nights = (paymentDto.CheckOutDate - paymentDto.CheckInDate).Days;
            var subTotal = (room.RoomType?.Price ?? 0) * nights;
            var tax = subTotal * 0.05;
            var finalTotal = subTotal + tax; // THIS IS THE CALCULATED TOTAL

            // -------------------------------------------------------------------------
            // 4. UPDATE STATUS & SAVE
            // -------------------------------------------------------------------------

            // Update the properties
            payment.PaymentStatus = "Completed";       // Change status
            payment.Amount = (decimal)finalTotal;      // Update total amount
            payment.PaymentDate = DateTime.UtcNow;     // Set timestamp
            payment.PaymentMethod = "CreditCard";

            // Update the database
            await _unitOfWork.PaymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            // -------------------------------------------------------------------------
            // 5. SUCCESS
            // -------------------------------------------------------------------------

            TempData["Success"] = $"Payment of {finalTotal:C} Completed Successfully!";







            return RedirectToAction("BookingDetails", new { id = paymentDto.BookingId });

        
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentCash(PaymentDTO paymentDto)
        {
            // 1. Ignore Credit Card validation errors (since this is cash)
            ModelState.Remove("CardNumber");
            ModelState.Remove("CVV");
            ModelState.Remove("Expiry");

            // 2. Check if the rest of the form (Dates, RoomId) is valid
            if (!ModelState.IsValid)
            {
                return View("Payment", paymentDto);
            }


            

            // 8. Redirect
            // No need to clear session here anymore since we didn't use it
            TempData["Success"] = "Booking confirmed! Please pay at the front desk.";

            return RedirectToAction("BookingDetails", new { id = paymentDto.BookingId });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(bool isSuccess)
        {
            // 1. Get selected room from session
            int? roomId = HttpContext.Session.GetInt32("SelectedRoomId");
            if (roomId == null)
                return RedirectToAction("Create");

            // 2. Payment failed → unlock room
            if (!isSuccess)
            {
                await _unitOfWork.RoomRepository.UnlockRoom(roomId.Value);

                HttpContext.Session.Clear();
                TempData["Error"] = "Payment failed. Room unlocked.";
                return RedirectToAction("MyBookings");
            }

            // 3. Payment succeeded → create booking
            var tempCheckIn = HttpContext.Session.GetString("TempCheckIn");
            var tempCheckOut = HttpContext.Session.GetString("TempCheckOut");
            if (string.IsNullOrEmpty(tempCheckIn) || string.IsNullOrEmpty(tempCheckOut))
            {
                HttpContext.Session.Clear();
                TempData["Error"] = "Booking info missing. Please start again.";
                return RedirectToAction("Create");
            }

            DateTime checkIn = DateTime.Parse(tempCheckIn);
            DateTime checkOut = DateTime.Parse(tempCheckOut);

            var user = await _userManager.GetUserAsync(User);

            var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomId.Value);
            if (room == null)
            {
                HttpContext.Session.Clear();
                TempData["Error"] = "Selected room not found.";
                return RedirectToAction("Create");
            }

            // Calculate total price
            int nights = (checkOut - checkIn).Days;
            var roomprice = room.RoomType.Price;
            var totalPrice = (roomprice * nights);

            var booking = new Booking
            {
                GuestId = user.Id,
                RoomId = room.Id,
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                Status = "Confirmed",
                TotalPrice = Convert.ToDecimal(totalPrice),
                Date = DateTime.UtcNow
            };

            await _unitOfWork.BookingRepository.AddAsync(booking);

            // Mark room as occupied
            room.Status = roomStatus.Occupied;
            room.ReservedUntil = null;

            await _unitOfWork.SaveChangesAsync();

            HttpContext.Session.Clear();
            TempData["Success"] = "Payment successful. Booking confirmed!";
            return RedirectToAction("BookingDetails", new { id = booking.Id });
        }

        // ---------------------------------------------------
        // STEP 5: Payment Timeout → auto unlock room
        // ---------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> PaymentTimeout()
        {
            int? roomId = HttpContext.Session.GetInt32("SelectedRoomId");
            if (roomId != null)
                await _unitOfWork.RoomRepository.UnlockRoom(roomId.Value);

            HttpContext.Session.Clear();
            TempData["Error"] = "Payment timed out. Room unlocked.";
            return RedirectToAction("MyBookings");
        }










    }
}