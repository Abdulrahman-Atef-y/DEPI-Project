using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("Hotel")]
    public class Hotel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hotel Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Hotel Name must be between 2 and 100 characters.")]
        public string? Name { get; set; }
        public string? Policies { get; set; }
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
        [MaxLength(200)]
        public string? Address { get; set; }
        [MaxLength(100)]
        public string? City { get; set; }
        [MaxLength(100)]
        public string? Country { get; set; }
        [Phone(ErrorMessage = "Invalid Phone Number.")]
        [StringLength(20)]
        public string? Phone { get; set; }
        [Range(1, 5, ErrorMessage = "Stars must be between 1 and 5.")]
        public int Stars { get; set; }
        public List<HotelImage> HotelImages { get; set; }
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public ICollection<Review> Reviews { get; set; }
    }
}