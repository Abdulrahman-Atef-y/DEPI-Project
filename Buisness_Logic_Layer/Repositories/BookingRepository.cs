using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly DbContext _context;
        public BookingRepository(DbContext context) : base(context)
        {
            _context = context;
        }



       
    }
}
