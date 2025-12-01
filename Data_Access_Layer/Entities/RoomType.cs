using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Data_Access_Layer.Entities
{
    [Table("RoomTypes")]
    public class RoomType
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Room Type Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
        [Range(1, 1000, ErrorMessage = "Area must be a positive value.")]
        public double Area { get; set; }
        public string? RoomAmenities { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Occupancy must be between 1 and 20 guests.")]
        public int Occupancy { get; set; }

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0.")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public List<RoomImage> Images { get; set; }
        public List<Room> Rooms { get; set; }
        public RoomType()
        {
            Images = new List<RoomImage>();
            Rooms = new List<Room>();
        }
    }
}



