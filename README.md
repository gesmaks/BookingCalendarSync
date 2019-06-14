# Calendar Synchronization Example

This repo contains a .NET Core console application that is a simple example of reading an [iCalendar](https://en.wikipedia.org/wiki/ICalendar) file and adding calendar entries to a local database. The example uses [iCal.NET library](https://github.com/rianjs/ical.net) that aims at providing RFC 5545 compliance.

I used similar code for a 1-way synchronization of a reservations calendar on a lodging reservation web page [https://www.jastrzebiagora4.pl](https://www.jastrzebiagora4.pl). The synchronization takes place between that calendar and a calendar on booking[.]com.

Before running the example, edit `appsettings.json` and change the path to the `BookingDB.mdf` database file. Then go to the directory where `BookingCalendarSync.csproj` is located and run the following commands:
```sh
dotnet build
dotnet run ..\TestData\Test_iCal_data.ics
```
You can also provide a URL (`http` or `https`) to an iCalendar file/content.