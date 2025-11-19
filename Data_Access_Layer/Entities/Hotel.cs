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
