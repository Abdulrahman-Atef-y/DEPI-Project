using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
           
        public class BookingGuest
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int BookingId { get; set; }

            [ForeignKey("BookingId")]
            public Booking Booking { get; set; }

            [Required]
            [MaxLength(50)]
            public string FirstName { get; set; }

            [Required]
            [MaxLength(50)]
            public string LastName { get; set; }

            // Optional fields:
            [MaxLength(100)]
            public string? Nationality { get; set; }

            [MaxLength(50)]
            public string SSN { get; set; }

            public DateTime? BirthDate { get; set; }

            

            
        }
    

}

