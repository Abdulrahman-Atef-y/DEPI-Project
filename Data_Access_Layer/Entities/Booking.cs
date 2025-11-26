using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string GuestId { get; set; }
        public Guest Guest { get; set; }

        [Required]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total Price cannot be negative.")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? CancellationPolicy { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public List<Review> Reviews { get; set; }
    }
}