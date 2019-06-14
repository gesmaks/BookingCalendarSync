using BookingCalendarSync.Model;
using BookingCalendarSync.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarSync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Validate input.
            if (args.Length < 1)
            {
                Console.WriteLine(
@"An input argument is missing.

Usage:

BookingCalendarSync <calendar_url_or_path>");
                return;
            }
            if (!Uri.TryCreate(args[0], UriKind.RelativeOrAbsolute, out Uri calendarUri))
            {
                Console.WriteLine("The calendar URL/file path is invalid.");
                return;
            }

            // Register services and build service provider.
            var serviceProvider = RegisterServices();

            // Synchronize the local calendar with the calendar located at the given URI.
            await serviceProvider.GetService<BookingCalendarSynchronizer>().SynchronizeCalendarAsync(calendarUri);

            // Display the events/bookings.
            using (var context = serviceProvider.GetService<BookingDbContext>())
            {
                await (from b in context.Bookings
                       orderby b.EndDate
                       select b)
                    .ForEachAsync(b => Console.WriteLine($"Booked from {b.StartDate:d} to {b.EndDate:d}: {b.Description}"));
            }
        }

        private static IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<BookingCalendarSynchronizer>();
            services.AddDbContext<BookingDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MyBookingDB")));

            return services.BuildServiceProvider();
        }
    }
}
