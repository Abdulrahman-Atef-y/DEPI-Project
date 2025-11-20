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

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? Policies { get; set; }
        public string? Description { get; set; }
        public List<RoomType> RoomTypes { get; set; }
    }
}


// suggested entity for scalability

/*
 namespace Domain.Entities
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Policies { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public float Rating { get; set; }
        public int Stars { get; set; }

        // Navigation
        public ICollection<RoomType> RoomTypes { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<HotelImage> HotelImages { get; set; }
    }
}

 */