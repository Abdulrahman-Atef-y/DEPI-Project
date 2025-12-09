using Data_Access_Layer.Entities;
using System.ComponentModel.DataAnnotations;

public class Payment
{
    [Key]
    public int Id { get; set; }

    public int BookingId { get; set; }  // FK to Booking
    public decimal Amount { get; set; }
    public string PaymentStatus { get; set; } // Pending, Completed, Failed
    public string PaymentMethod { get; set; } // CreditCard, PayPal, etc.
    public DateTime PaymentDate { get; set; }

    public virtual Booking Booking { get; set; } // Navigation property
}
