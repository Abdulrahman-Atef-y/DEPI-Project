using Buisness_Logic_Layer.Interfaces;
using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _appContext;

        public BookingRepository(DbContext context) : base(context)
        {
            // Cast DbContext to your actual context to access DbSets
            _appContext = context as ApplicationDbContext
                ?? throw new ArgumentException("Invalid DbContext type");
        }

        public async Task<Booking> GetBookingWithRoomAndTypeAsync(int bookingId)
        {
            return await _appContext.Bookings
                  .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }
    }
}
 