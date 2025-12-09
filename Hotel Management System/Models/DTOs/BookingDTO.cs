using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class BookingDTO
    {
        public int RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public double PricePerNight { get; set; }

        public string phonenumber { get; set; }
        public int BookingId { get; set; }

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
        public List<string>? ImageUrls { get; set; } = new List<string>();
        public List<string> Amenities { get; set; } = new List<string>();

        public string? Email { get; set; }
       
        public int Occupancy { get; set; } // ← Add this so you can pass it from controller

        public int NumberOfGuests { get; set; }

        public List<BookingGuestDTO> BookingGuests { get; set; } = new List<BookingGuestDTO>();


    }
}
