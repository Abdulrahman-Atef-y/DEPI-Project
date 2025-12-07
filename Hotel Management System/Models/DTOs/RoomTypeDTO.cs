using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class RoomTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        public double Price { get; set; }

        public int Occupancy { get; set; }
        public string? ImageUrl { get; set; }
        public int HotelId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<string> AmenitiesList { get; set; } = new List<string>();
        [Display(Name = "وسائل الراحة")]
        public string Amenities
        {
            get => string.Join(", ", AmenitiesList);
            set => AmenitiesList = value?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
        }

    }
}
