using System;

namespace AbaBackend.Infrastructure.Utils.Static
{
  public static class StaticUtils
  {
    public static (decimal regular, decimal extra) CalculateWageHour(decimal total, decimal newValue)
    {
      var newTotalHours = total + newValue;

      if (newTotalHours <= 40) return (newValue, 0);
      if (total < 40)
      {
        var extra = newValue - (40 - total);
        var reg = newValue - extra;
        return (reg, extra);
      }
      return (0, newValue);
    }
  }
}