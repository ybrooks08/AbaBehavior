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

    // public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
    // {
    //   return items.GroupBy(property).Select(x => x.First());
    // }

    // public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source,
    //           Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
    // {
    //   if (source == null) throw new ArgumentNullException(nameof(source));
    //   if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));

    //   return _(); IEnumerable<TSource> _()
    //   {
    //     var knownKeys = new HashSet<TKey>(comparer);
    //     foreach (var element in source)
    //     {
    //       if (knownKeys.Add(keySelector(element)))
    //         yield return element;
    //     }
    //   }
    // }
  }
}