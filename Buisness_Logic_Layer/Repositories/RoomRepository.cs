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
    internal class RoomRepository : GenericRepository<Room> , IRoomRepository
    {
        public RoomRepository(DbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime from, DateTime to, int? roomTypeId = null)
        {
            var bookings = _context.Set<Booking>();
            var rooms = _dbSet.AsQueryable();

            if (roomTypeId.HasValue)
                rooms = rooms.Where(r => r.RoomTypeId == roomTypeId.Value);

            var unavailableRoomIds = await bookings
                .Where(b => b.Status != "Cancelled" && !(b.CheckOutDate <= from || b.CheckInDate >= to))
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            return await rooms
                .Where(r => !unavailableRoomIds.Contains(r.Id))
                .ToListAsync();
        }
    }
}
