using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    [Table("Reviews")]
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars.")]
        public int Rating { get; set; }
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        [DataType(DataType.MultilineText)]
        public string? Comment { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
