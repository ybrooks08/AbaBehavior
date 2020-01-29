using System;
using System.Collections.Generic;
using System.Linq;
using AbaBackend.Infrastructure.Collection;

namespace AbaBackend.Infrastructure.Extensions
{
  public static class ListExtensions
  {
    public static string ConvertIdToListDelimitedByComma(this List<int> listId)
    {
      var result = string.Join(',', listId);
      return result;
    }

    public static IEnumerable<IEnumerable<T>> FindConsecutiveGroups<T>(this IEnumerable<T> sequence, Predicate<T> predicate, int sequenceSize)
    {
      IEnumerable<T> window = Enumerable.Empty<T>();

      int count = 0;

      foreach (var item in sequence)
      {
        if (predicate(item))
        {
          window = window.Concat(Enumerable.Repeat(item, 1));
          count++;

          if (count == sequenceSize)
          {
            yield return window;
            window = window.Skip(1);
            count--;
          }
        }
        else
        {
          count = 0;
          window = Enumerable.Empty<T>();
        }
      }
    }
  }
}