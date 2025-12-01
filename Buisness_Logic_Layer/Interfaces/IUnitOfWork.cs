using System;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRoomRepository RoomRepository { get; }
        IRoomTypeRepository RoomTypeRepository { get; }
        IHotelRepository HotelRepository { get; }
        IHotelImageRepository HotelImageRepository { get; }
        IBookingRepository BookingRepository { get; }
        IReviewRepository ReviewRepository { get; }
        IGuestRepository GuestRepository { get; }
        IRoomImageRepository RoomImageRepository { get; }


        Task<int> SaveChangesAsync();
    }
}