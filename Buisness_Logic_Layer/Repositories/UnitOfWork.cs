using Buisness_Logic_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Buisness_Logic_Layer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private bool _disposed;

        private IRoomRepository? _roomRepository;
        private IRoomTypeRepository? _roomTypeRepository;
        private IHotelRepository? _hotelRepository;
        private IHotelImageRepository? _hotelImageRepository;
        private IBookingRepository? _bookingRepository;
        private IReviewRepository? _reviewRepository;
        private IGuestRepository? _guestRepository;

        public UnitOfWork(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRoomRepository RoomRepository => _roomRepository ??= new RoomRepository(_context);
        public IRoomTypeRepository RoomTypeRepository => _roomTypeRepository ??= new RoomTypeRepository(_context);
        public IHotelRepository HotelRepository => _hotelRepository ??= new HotelRepository(_context);
        public IHotelImageRepository HotelImageRepository => _hotelImageRepository ??= new HotelImageRepository(_context);
        public IBookingRepository BookingRepository => _bookingRepository ??= new BookingRepository(_context);
        public IReviewRepository ReviewRepository => _reviewRepository ??= new ReviewRepository(_context);
        public IGuestRepository GuestRepository => _guestRepository ??= new GuestRepository(_context);

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }
    }
}