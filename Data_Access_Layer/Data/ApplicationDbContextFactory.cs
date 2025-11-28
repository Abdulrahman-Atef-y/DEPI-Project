using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data_Access_Layer.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1️⃣ Build configuration manually (because Program.cs is NOT used during migrations)
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // where EF runs migration
                .AddJsonFile("appsettings.json")              // load your json
                .Build();

            // 2️⃣ Read the connection string:
            string connectionString = config.GetConnectionString("DefaultConnection");

            // 3️⃣ Build DbContextOptions:
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
