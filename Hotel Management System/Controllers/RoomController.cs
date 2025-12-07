using Microsoft.AspNetCore.Authorization;
using Data_Access_Layer.Entities;
using Hotel_Management_System.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Buisness_Logic_Layer.Interfaces;
namespace Hotel_Management_System.Controllers
{
    [Authorize(Roles = "Admin")] // Uncomment later
    public class RoomController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var rooms = await _unitOfWork.RoomRepository.FindAllAsync(criteria: null, includes: new[] { "RoomType", "RoomType.Images" });

            var roomDtos = rooms.Select(r => new RoomDTO
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Floor = r.Floor,
                Status = r.Status,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType.Name,
                RoomTypeImageUrl = r.RoomType.Images.FirstOrDefault()?.ImageUrl ?? "/Default/placeholder-room.png"

            }).ToList();

            return View(roomDtos);
        }

        public async Task<IActionResult> Create()
        {
            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();

            var dto = new RoomDTO
            {
                RoomTypeList = roomTypes.Select(rt => new SelectListItem
                {
                    Text = rt.Name,
                    Value = rt.Id.ToString()
                })
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomDTO roomDto)
        {
            if (ModelState.IsValid)
            {
                var room = new Room
                {
                    RoomNumber = roomDto.RoomNumber,
                    Floor = roomDto.Floor,
                    Status = roomDto.Status,
                    RoomTypeId = roomDto.RoomTypeId
                };

                await _unitOfWork.RoomRepository.AddAsync(room);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم انشاء غرفة بنجاح!";
                return RedirectToAction(nameof(Index));
            }

            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();
            roomDto.RoomTypeList = roomTypes.Select(rt => new SelectListItem
            {
                Text = rt.Name,
                Value = rt.Id.ToString()
            });

            return View(roomDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();

            var bookings = await _unitOfWork.BookingRepository.FindAllAsync(b => b.RoomId == id);
            if (bookings.Any())
            {
                TempData["ErrorMessage"] = "لا يمكن حذف الغرفة لأنها مرتبطة بالحجوزات الحالية.";
                return RedirectToAction(nameof(Index));
            }
            await _unitOfWork.RoomRepository.DeleteAsync(room);
            TempData["SuccessMessage"] = "تم حذف الغرفة بنجاح!";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();

            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();

            var dto = new RoomDTO
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                Status = room.Status,
                RoomTypeId = room.RoomTypeId,
                RoomTypeList = roomTypes.Select(rt => new SelectListItem
                {
                    Text = rt.Name,
                    Value = rt.Id.ToString()
                })
            };

            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoomDTO roomDto)
        {
            if (ModelState.IsValid)
            {
                var room = await _unitOfWork.RoomRepository.GetByIdAsync(roomDto.Id);
                if (room == null) return NotFound();

                room.RoomNumber = roomDto.RoomNumber;
                room.Floor = roomDto.Floor;
                room.Status = roomDto.Status;
                room.RoomTypeId = roomDto.RoomTypeId;

                await _unitOfWork.RoomRepository.UpdateAsync(room);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "تم تحديث بيانات الغرفة بنجاح!";
                return RedirectToAction(nameof(Index));
            }

            var roomTypes = await _unitOfWork.RoomTypeRepository.GetAllAsync();
            roomDto.RoomTypeList = roomTypes.Select(rt => new SelectListItem
            {
                Text = rt.Name,
                Value = rt.Id.ToString()
            });

            return View(roomDto);
        }
    }
}
