using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("HotelImages")]
    public class HotelImage
    {
        public int Id { get; set; }

        [Required]
        public string? ImageUrl { get; set; }

        [Required]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
