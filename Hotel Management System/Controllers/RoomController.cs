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
            var rooms = await _unitOfWork.RoomRepository.FindAllAsync(criteria: null, includes: new[] { "RoomType" });

            var roomDtos = rooms.Select(r => new RoomDTO
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Floor = r.Floor,
                Status = r.Status,
                RoomTypeId = r.RoomTypeId
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
                TempData["SuccessMessage"] = "Room created successfully!";
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

        public async Task<IActionResult> Delete(int id)
        {
            var room = await _unitOfWork.RoomRepository.GetByIdAsync(id);
            if (room == null) return NotFound();

            await _unitOfWork.RoomRepository.DeleteAsync(room);
            await _unitOfWork.SaveChangesAsync();
            TempData["SuccessMessage"] = "Room deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
