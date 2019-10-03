using System;

namespace AbaBackend.DataModel
{
  [Flags]
  public enum DayOfWeekBit
  {
    Sunday = 1,
    Monday = 2,
    Tuesday = 4,
    Wednesday = 8,
    Thursday = 16,
    Friday = 32,
    Saturday = 64
  }
}