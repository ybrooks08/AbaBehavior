using System;

public static class DateTimeExtensions
{
  public static Decimal ConvertTimeToNumber(this DateTime dt)
  {
    var h = dt.Hour;
    var m = dt.Minute;
    var ticks = m * (decimal)60 / (decimal)3600;
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
}