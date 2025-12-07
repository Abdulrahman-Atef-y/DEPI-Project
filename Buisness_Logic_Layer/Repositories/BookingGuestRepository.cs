using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buisness_Logic_Layer.Interfaces;


namespace Buisness_Logic_Layer.Repositories
{
    public class BookingGuestRepository : GenericRepository<BookingGuest>, IBookingGuestRepository
    {
        public BookingGuestRepository(ApplicationDbContext context) : base(context)
        {
        }

    }

    internal interface IBookingGuestRepository
    {
    }
}
