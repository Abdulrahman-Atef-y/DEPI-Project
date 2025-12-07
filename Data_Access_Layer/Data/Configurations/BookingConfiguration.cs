using Data_Access_Layer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasMany(b => b.Reviews)
                .WithOne(r => r.Booking)
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(b => b.Guest)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GuestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Room)
       .WithMany(r => r.Bookings)
       .HasForeignKey(b => b.RoomId)
       .IsRequired()
       .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.BookingGuests)
    .WithOne(bg => bg.Booking)
    .HasForeignKey(bg => bg.BookingId)
    .OnDelete(DeleteBehavior.Cascade);



        }
    }
}