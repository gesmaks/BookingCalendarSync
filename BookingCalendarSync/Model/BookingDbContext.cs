using Microsoft.EntityFrameworkCore;

namespace BookingCalendarSync.Model
{
    public class BookingDbContext : DbContext
    {
        public DbSet<Booking> Bookings { get; set; }

        public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
        {
        }
    }
}
