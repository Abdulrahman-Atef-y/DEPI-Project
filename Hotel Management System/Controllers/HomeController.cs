using System.Diagnostics;
using Buisness_Logic_Layer.Interfaces;
using Hotel_Management_System.Models.DTOs;
using Hotel_Management_System.Models;
using Microsoft.AspNetCore.Mvc;
using Data_Access_Layer.Entities;

namespace Hotel_Management_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            
        }

        public async Task<IActionResult> Index()
        {
          var roomTypes = await _unitOfWork.RoomTypeRepository.FindAllAsync(criteria: r => r.Price > 0,
          includes: new[] { "Images" });
            var dtos = roomTypes.Select(rt => new RoomTypeDTO
            {
                Id = rt.Id,
                Name = rt.Name ?? "unknown",
                Description = rt.Description,
                Price = rt.Price,
                Occupancy = rt.Occupancy,
                ImageUrl = rt.Images != null && rt.Images.Any()
                           ? rt.Images.First().ImageUrl
                           : "https://via.placeholder.com/800x600?text=No+Image",
                ImageUrls = rt.Images != null && rt.Images.Any()
                ? rt.Images.Select(i => i.ImageUrl).ToList()
                : new List<string> { "https://via.placeholder.com/800x600?text=No+Image" },
                AmenitiesList = !string.IsNullOrWhiteSpace(rt.RoomAmenities)
               ? rt.RoomAmenities.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList()
               : new List<string>()

            }).ToList();
            return View(dtos);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
