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
        public int RoomNumber { get; set; }
        public int Floor { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        [Required]
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}