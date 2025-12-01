using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class RoomDTO
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        [Required]
        [Range(0, 100)]
        public int Floor { get; set; }

        [Required]
        public roomStatus Status { get; set; }

        [Required]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }
        public IEnumerable<SelectListItem>? RoomTypeList { get; set; }
        public string? RoomTypeImageUrl { get; set; }

    }
}
