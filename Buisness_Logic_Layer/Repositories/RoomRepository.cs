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
        private readonly DbContext _context;
        public RoomRepository(DbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime from, DateTime to, int? roomTypeId = null)
        {
            var bookings = _context.Set<Booking>();
            var rooms = _dbSet.AsQueryable();

            if (roomTypeId.HasValue)
                rooms = rooms.Where(r => EF.Property<int>(r, "RoomTypeId") == roomTypeId.Value);

            var unavailableRoomIds = await bookings
                .Where(b => !(b.CheckOutDate <= from || b.CheckInDate >= to))
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            return await rooms
                .Where(r => !unavailableRoomIds.Contains(EF.Property<int>(r, "Id")))
                .ToListAsync();
        }
    }
}
