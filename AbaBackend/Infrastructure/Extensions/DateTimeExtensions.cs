using System;

namespace AbaBackend.Infrastructure.Extensions
{
  public static class DateTimeExtensions
  {
    public static Decimal ConvertTimeToNumber(this DateTime dt)
    {
      var h = dt.Hour;
      var m = dt.Minute;
      var ticks = m * (decimal) 60 / (decimal) 3600;
      return h + ticks;
    }

    public static DateTime AddWeeks(this DateTime dateTime, int numberOfWeeks)
    {
      return dateTime.AddDays(numberOfWeeks * 7);
    }

    public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
    {
      int diff = dt.DayOfWeek - startOfWeek;
      if (diff < 0)
      {
        diff += 7;
      }

      return dt.AddDays(-1 * diff).Date;
    }

    public static DateTime GetNextDay(this DateTime start, DayOfWeek day)
    {
      var daysToAdd = ((int) day - (int) start.DayOfWeek + 7) % 7;
      return start.AddDays(daysToAdd);
    }

    public static DateTime GetPrevDay(this DateTime start, DayOfWeek day)
    {
      var daysToRemove = ((int) day - (int) start.DayOfWeek - 7) % 7;
      return start.AddDays(daysToRemove);
    }
  }
}