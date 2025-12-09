using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("Rooms")]
    public class Room
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Room Number")]
        [Range(1, 10000, ErrorMessage = "Room Number must be valid.")]
        public int RoomNumber { get; set; }
        [Range(0, 100, ErrorMessage = "Floor must be a valid number.")]

        public int Floor { get; set; }

        [Required]
        public roomStatus Status { get; set; } = roomStatus.Available;

        public DateTime? ReservedUntil { get; set; }

        [Required]
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
