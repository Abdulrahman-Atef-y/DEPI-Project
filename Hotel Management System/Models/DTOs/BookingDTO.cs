using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class BookingDTO
    {
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public double PricePerNight { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(1);
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        
    }
}
