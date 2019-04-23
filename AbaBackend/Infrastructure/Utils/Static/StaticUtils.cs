using System;
using System.Globalization;
using System.Text.RegularExpressions;

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

    public static bool IsValidEmail(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        return false;

      try
      {
        // Normalize the domain
        email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                              RegexOptions.None, TimeSpan.FromMilliseconds(200));

        // Examines the domain part of the email and normalizes it.
        string DomainMapper(Match match)
        {
          // Use IdnMapping class to convert Unicode domain names.
          var idn = new IdnMapping();

          // Pull out and process domain name (throws ArgumentException on invalid)
          var domainName = idn.GetAscii(match.Groups[2].Value);

          return match.Groups[1].Value + domainName;
        }
      }
      catch (RegexMatchTimeoutException)
      {
        return false;
      }
      catch (ArgumentException)
      {
        return false;
      }

      try
      {
        return Regex.IsMatch(email,
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
      }
      catch (RegexMatchTimeoutException)
      {
        return false;
      }
    }
  }


}