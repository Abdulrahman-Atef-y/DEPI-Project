using System;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        public string? RoomTypeName { get; set; }
        public double PricePerNight { get; set; }

        // --- UPDATED: Removed [Required] and [CreditCard] attributes ---
        // We will validate these manually in the controller
        public string? CardNumber { get; set; }
        public string? CVV { get; set; }
        public string? Expiry { get; set; }
    }
}