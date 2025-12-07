using Data_Access_Layer.Data;
using Data_Access_Layer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Utility
{
    public static class DbInitializer
    {
        public static async Task Initialize(
            UserManager<Guest> userManager,
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            var hotel = new Hotel
            {
                Name = "Grand Azure Hotel",
                Description = "A comfortable modern hotel offering excellent service and central location.",
                Policies = "Check-in 3:00 PM, Check-out 11:00 AM. No smoking. Pets allowed on request.",
                Address = "123 Main Street",
                City = "YourCity",
                Country = "YourCountry",
                Phone = "+1-555-0100",
                Stars = 4,
                HotelImages = new System.Collections.Generic.List<HotelImage>(),
                RoomTypes = new System.Collections.Generic.List<RoomType>(),
                Reviews = new System.Collections.Generic.List<Review>()
            };

            await context.Hotels.AddAsync(hotel);
            await context.SaveChangesAsync();



            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Customer"));
            }
            var adminEmail = "admin@grandhotel.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new Guest
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    PhoneNumber = "01000000000",
                    Gender = Gender.Male,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await userManager.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
