using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Hotel_Management_System.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] // Uncomment later
    public class RoomTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;


        public RoomTypeController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;

        }
        private List<SelectListItem> GetAllAmenities()
        {
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "WiFi", Text = "WiFi" },
        new SelectListItem { Value = "TV", Text = "TV" },
        new SelectListItem { Value = "AC", Text = "Air Conditioning" },
        new SelectListItem { Value = "Minibar", Text = "Minibar" },
        new SelectListItem { Value = "Balcony", Text = "Balcony" },
        new SelectListItem { Value = "Safe", Text = "Safe" }
    };
        }
        public async Task<IActionResult> Index()
        {
            var types = await _unitOfWork.RoomTypeRepository.GetAllAsync();

            // Map to DTO
            var dtos = types.Select(t => new RoomTypeDTO
            {
                Id = t.Id,
                Name = t.Name,
                Price = t.Price,
                Occupancy = t.Occupancy,
                Description = t.Description
            });

            return View(dtos);
        }

        public IActionResult Create()
        {
            var dto = new RoomTypeCreateDto
            {
                AvailableAmenities = GetAllAmenities()
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomTypeCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                var hotel = await _unitOfWork.HotelRepository.GetByIdAsync(2);
                if (hotel == null)
                {
                    ModelState.AddModelError("", "The hotel with Id = 1 does not exist.");
                    return View(dto);
                }
                var roomType = new RoomType
                {
                    Name = string.IsNullOrEmpty(dto.Name) ? "ديلوكس" : dto.Name,
                    Description = dto.Description ?? "غرفة فاخرة بتصميم عصري مع إطلالة رائعة على المدينة، مجهزة بأحدث وسائل الراحة.",
                    Price = dto.Price == 0 ? 450 : dto.Price,
                    Occupancy = dto.Occupancy == 0 ? 2 : dto.Occupancy,
                    HotelId = 2, // HARDCODED TEMPORARILY
                    Area = 30, // Default or add to DTO
                    RoomAmenities = dto.Amenities != null && dto.Amenities.Any()
                        ? string.Join(", ", dto.Amenities)
                        : "WiFi, TV", // Default or add to DTO
                    Images = new List<RoomImage>()
                };

                if (dto.RoomImages != null && dto.RoomImages.Length > 0)
                {
                    var relativeUploadFolder = Path.Combine("img", "roomtypes");
                    var absoluteUploadFolder = Path.Combine(_env.WebRootPath, relativeUploadFolder);
                    if (!Directory.Exists(absoluteUploadFolder))
                        Directory.CreateDirectory(absoluteUploadFolder);
                    foreach (var file in dto.RoomImages)
                    {
                        if (file.Length > 0)
                        {
                            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(absoluteUploadFolder, uniqueFileName);
                            try
                            {
                                using var fileStream = new FileStream(filePath, FileMode.Create);
                                await file.CopyToAsync(fileStream);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", "Error uploading image: " + ex.Message);
                                return View(dto);
                            }

                            var roomImage = new RoomImage
                            {
                                ImageUrl = $"/{relativeUploadFolder.Replace(Path.DirectorySeparatorChar, '/')}/{uniqueFileName}"
                            };
                            roomType.Images.Add(roomImage);
                            await _unitOfWork.RoomImageRepository.AddAsync(roomImage);
                        }
                    }
                }
                await _unitOfWork.RoomTypeRepository.AddAsync(roomType);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room Type created successfully!";
                return RedirectToAction(nameof(Index));

            }
            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
            {
                TempData["ErrorMessage"] = "نوع الغرفة غير موجود.";
                return RedirectToAction(nameof(Index));
            }
            var roomsWithType = await _unitOfWork.RoomRepository.FindAllAsync(r => r.RoomTypeId == id);
            if (roomsWithType.Any())
            {
                TempData["ErrorMessage"] = "لا يمكن حذف نوع الغرفة لأنه مرتبط بغرف موجودة.";
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWork.RoomTypeRepository.DeleteAsync(roomType);

            TempData["SuccessMessage"] = "تم حذف نوع الغرفة بنجاح.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
            {
                TempData["ErrorMessage"] = "نوع الغرفة غير موجود.";
                return RedirectToAction(nameof(Index));
            }

            var dto = new RoomTypeDTO
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Price = roomType.Price,
                Occupancy = roomType.Occupancy,
                Description = roomType.Description,
                ImageUrl = roomType.Images.FirstOrDefault()?.ImageUrl,
                AmenitiesList = string.IsNullOrEmpty(roomType.RoomAmenities)
                ? new List<string>()
                : roomType.RoomAmenities.Split(", ", StringSplitOptions.RemoveEmptyEntries).ToList()
            };
            ViewBag.AvailableAmenities = GetAllAmenities();
            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomTypeDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var roomType = await _unitOfWork.RoomTypeRepository.GetByIdAsync(dto.Id);
            if (roomType == null)
            {
                TempData["ErrorMessage"] = "نوع الغرفة غير موجود.";
                return RedirectToAction(nameof(Index));
            }

            roomType.Name = dto.Name;
            roomType.Price = dto.Price;
            roomType.Occupancy = dto.Occupancy;
            roomType.Description = dto.Description ?? roomType.Description;
            roomType.RoomAmenities = dto.Amenities;

            await _unitOfWork.RoomTypeRepository.UpdateAsync(roomType);
            await _unitOfWork.SaveChangesAsync();

            TempData["SuccessMessage"] = "تم تعديل نوع الغرفة بنجاح.";
            return RedirectToAction(nameof(Index));
        }


    }
}