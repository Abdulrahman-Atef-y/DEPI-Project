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

        [Required]
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}



// sugested entity for the scalibility

/*
 public int Id { get; set; }
        public int BookingId { get; set; }
        public int GuestId { get; set; }
        public int HotelId { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }

        public Booking Booking { get; set; }
        public Guest Guest { get; set; }
        public Hotel Hotel { get; set; }
 */