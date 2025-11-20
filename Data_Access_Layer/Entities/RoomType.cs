using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("RoomTypes")]
    public class RoomType
    {
        //public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        //public string? Name { get; set; }

        //public string? Description { get; set; }
        //public double Area { get; set; }
        //public string? RoomAmenities { get; set; }

        [Required]
        //public int Occupancy { get; set; }

        [Required]
        //public double Price { get; set; }
        //public int HotelId { get; set; }
        //public Hotel Hotel { get; set; }
        //public List<RoomImage> Images { get; set; }
        //public List<Room> Rooms { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}



// removing public List<Booking> Bookings { get; set; }
// because the relation is between the booking and the room