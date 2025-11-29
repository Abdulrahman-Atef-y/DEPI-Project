using Buisness_Logic_Layer.Interfaces;
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
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();

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
                           ? rt.Images.First().ImageUrl
                           : "https://via.placeholder.com/800x600?text=No+Image"
            }).ToList();

            return View(dtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);
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
                           : "https://via.placeholder.com/800x600"
            };

            return View(dto);
        }
    }
}
