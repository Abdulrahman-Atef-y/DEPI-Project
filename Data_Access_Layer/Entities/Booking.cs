using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("Booking")]
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public int GuestId { get; set; }
        public Guest Guest { get; set; }

        [Required]
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? CancellationPolicy { get; set; }
        public List<Review> Reviews { get; set; }
    }
}