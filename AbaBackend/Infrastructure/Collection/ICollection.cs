using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbaBackend.Infrastructure.Collection
{
  public interface ICollection
  {
    Task<object> GetClientBehaviorChart(int clientId, List<int> problems, DateTime? start = null, DateTime? end = null);
    Task<object> GetClientReplacementChart(int clientId, List<int> replacements, DateTime? start = null, DateTime? end = null);
  }
}