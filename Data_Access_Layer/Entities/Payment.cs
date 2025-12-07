using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string StripeSessionId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Paid, Failed
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
