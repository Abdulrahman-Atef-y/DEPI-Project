using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class RoomTypeRepository : GenericRepository<RoomType>, IRoomTypeRepository
    {
        public RoomTypeRepository(DbContext context) : base(context)
        {
        }


        public async Task<List<Review>> GetRoomTypeReviewsAsync(int roomTypeId)
        {
            var reviews = await _context.Set<Review>()
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Room)
                .Include(r => r.Booking)
                    .ThenInclude(b => b.Guest)
                .Where(r => r.Booking.Room.RoomTypeId == roomTypeId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();

            return reviews;
        }




    }
}
