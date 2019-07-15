using System.Collections.Generic;

namespace AbaBackend.Infrastructure.Extensions
{
  public static class ListExtensions
  {
    public static string ConvertIdToListDelimitedByComma(this List<int> listId)
    {
      var result = string.Join(',', listId);
      return result;
    }
  }
}