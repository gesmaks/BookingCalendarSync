using BookingCalendarSync.Model;
using Ical.Net;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookingCalendarSync.Services
{
    class BookingCalendarSynchronizer
    {
        private readonly BookingDbContext context;
        private readonly IConfiguration configuration;

        public BookingCalendarSynchronizer(BookingDbContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public Task SynchronizeCalendarAsync(Uri calendarUri)
        {
            // Prepare data reading task: either read a file or download via HTTP.
            Task<string> readDataTask;
            if (!calendarUri.IsAbsoluteUri || calendarUri.IsFile)
            {
                readDataTask = File.ReadAllTextAsync(calendarUri.OriginalString);
            }
            else if (calendarUri.Scheme == Uri.UriSchemeHttp || calendarUri.Scheme == Uri.UriSchemeHttps)
            {
                readDataTask = new HttpClient().GetStringAsync(calendarUri);
            }
            else
            {
                throw new ArgumentException("The entered URI is not supported. Only http(s) and file URIs are supported.", nameof(calendarUri));
            }

            return readDataTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    throw new Exception("Error downloading calendar data.", t.Exception);
                }

                // Parse iCal data using iCal.NET library.
                Calendar cal;
                try
                {
                    cal = Calendar.Load(t.Result);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error parsing calendar data. The data is: {t.Result}", ex);
                }

                // Add events (reservations) if they are not in the local calendar. 
                foreach (var ev in cal.Events)
                {
                    if (!(from b in context.Bookings
                          where b.StartDate == ev.Start.Date && b.EndDate == ev.End.Date && b.Description == ev.Summary
                          select b).Any())
                    {
                        context.Bookings.Add(new Booking
                        {
                            CreatedDate = DateTime.Now,
                            Description = ev.Summary,
                            Id = Guid.NewGuid(),
                            EndDate = ev.End.Date,
                            StartDate = ev.Start.Date,
                        });
                    }
                }

                // Save changes to the database.
                context.SaveChangesAsync();
            });
        }
    }
}
