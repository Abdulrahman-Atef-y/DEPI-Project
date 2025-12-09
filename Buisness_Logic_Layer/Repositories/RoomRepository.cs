using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    internal class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(DbContext context) : base(context)
        {
        }

        // ---------------------------------------------------------
        // Existing Function (kept unchanged)
        // ---------------------------------------------------------
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
                .Where(r => !unavailableRoomIds.Contains(r.Id) &&
                            r.Status != roomStatus.Occupied &&
                            (r.Status != roomStatus.Reserved || r.ReservedUntil < DateTime.Now))
                .ToListAsync();
        }


        // ---------------------------------------------------------
        // NEW: Get a single free (available & not locked) room
        // ---------------------------------------------------------
        public async Task<Room> GetFirstFreeRoom(int roomTypeId)
        {
            return await _dbSet
                .Where(r =>
                    r.RoomTypeId == roomTypeId &&
                    (r.Status == roomStatus.Available ||
                     (r.Status == roomStatus.Reserved && r.ReservedUntil < DateTime.Now))
                )
                .OrderBy(r => r.Id)
                .FirstOrDefaultAsync();
        }


        // ---------------------------------------------------------
        // NEW: Lock room for X minutes
        // ---------------------------------------------------------
        public async Task<bool> LockRoom(int roomId, int minutes = 15)
        {
            var room = await _dbSet.FindAsync(roomId);

            if (room == null)
                return false;

            // Only reserve if available or expired reserved
            if (room.Status == roomStatus.Available ||
                (room.Status == roomStatus.Reserved && room.ReservedUntil < DateTime.Now))
            {
                room.Status = roomStatus.Reserved;
                room.ReservedUntil = DateTime.Now.AddMinutes(minutes);
                await _context.SaveChangesAsync();
                return true;
            }

            return false; // cannot reserve
        }


        // ---------------------------------------------------------
        // NEW: Unlock (free) a room
        // ---------------------------------------------------------
        public async Task UnlockRoom(int roomId)
        {
            var room = await _dbSet.FindAsync(roomId);

            if (room != null)
            {
                room.Status = roomStatus.Available;
                room.ReservedUntil = null;

                await _context.SaveChangesAsync();
            }
        }


        // ---------------------------------------------------------
        // NEW: Clean expired locks (can be called periodically)
        // ---------------------------------------------------------
        public async Task CleanExpiredLocks()
        {
            var lockedRooms = await _dbSet
                .Where(r => r.Status == roomStatus.Reserved && r.ReservedUntil < DateTime.Now)
                .ToListAsync();

            foreach (var room in lockedRooms)
            {
                room.Status = roomStatus.Available;
                room.ReservedUntil = null;
            }

            if (lockedRooms.Count > 0)
                await _context.SaveChangesAsync();
        }




        public async Task<Room> GetRoomWithTypeAsync(int roomId)
        {
                return await _dbSet
                    .Include(r => r.RoomType)
                    .FirstOrDefaultAsync(r => r.Id == roomId);
        }
    }
}
