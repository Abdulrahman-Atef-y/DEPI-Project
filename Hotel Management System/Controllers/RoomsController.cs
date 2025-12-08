using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Hotel_Management_System.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_System.Controllers
{
    public class RoomsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["CurrentSort"] = sortOrder ?? "all";
            var roomTypes = await _unitOfWork.RoomTypeRepository.FindAllAsync(criteria: null, includes: new[] { "Images" });
           
            switch (sortOrder)
            {
                case "price_desc":
                    roomTypes = roomTypes.OrderByDescending(r => r.Price);
                    break;
                case "price_asc":
                    roomTypes = roomTypes.OrderBy(r => r.Price);
                    break;
                default:
                    roomTypes = roomTypes.OrderBy(r => r.Name);
                    break;
            }

            var dtos = roomTypes.Select(rt => new RoomTypeDTO
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description,
                Price = rt.Price,
                Occupancy = rt.Occupancy,
                ImageUrl = rt.Images != null && rt.Images.Any()
                           ?  rt.Images.First().ImageUrl
                           : "https://via.placeholder.com/800x600?text=No+Image",
                ImageUrls = rt.Images != null && rt.Images.Any()
                ? rt.Images.Select(i => i.ImageUrl).ToList()
                : new List<string> { "https://via.placeholder.com/800x600?text=No+Image" },
                AmenitiesList = !string.IsNullOrWhiteSpace(rt.RoomAmenities)
               ? rt.RoomAmenities.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
               : new List<string>(),
                Area=rt.Area,
                Rating=4
                

            }).ToList();

            return View(dtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.FindAsync(
        r => r.Id == id,
        includes: new[] { "Images" });

            var reviews = await _unitOfWork.RoomTypeRepository
                .GetRoomTypeReviewsAsync(id);
    
            if (roomType == null) return NotFound();

            var dto = new RoomTypeDTO
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                Price = roomType.Price,
                Occupancy = roomType.Occupancy,
                ImageUrl = roomType.Images != null && roomType.Images.Any()
                           ? roomType.Images.First().ImageUrl
                           : "https://via.placeholder.com/800x600",
                            ImageUrls = roomType.Images != null && roomType.Images.Any()
                ? roomType.Images.Select(i => i.ImageUrl).ToList()
                : new List<string> { "https://via.placeholder.com/800x600?text=No+Image" },
                AmenitiesList = !string.IsNullOrWhiteSpace(roomType.RoomAmenities)
               ? roomType.RoomAmenities.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
               : new List<string>(),
                Area = roomType.Area,
                
                Rating = reviews.Any() ? (int)Math.Round(reviews.Average(r => r.Rating)) : 0,
                
                ReviewCount = reviews.Count,
                Reviews = reviews.Select(r => new RoomReviewDTO
                {
                    
                    GuestName = r.Booking.Guest.FirstName,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    Date = r.Date
                }).ToList()

            };

            return View(dto);
        }
    }
}
