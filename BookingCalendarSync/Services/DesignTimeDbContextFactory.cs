using BookingCalendarSync.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BookingCalendarSync.Services
{
    /// <summary>
    /// This class tells the Entity Framework Tools how to create the DbContext at design time in order to create migrations.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BookingDbContext>
    {
        public BookingDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<BookingDbContext>();
            var connectionString = configuration.GetConnectionString("MyBookingDB");
            builder.UseSqlServer(connectionString);
            return new BookingDbContext(builder.Options);
        }
    }
}
