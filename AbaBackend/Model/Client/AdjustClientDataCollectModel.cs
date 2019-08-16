using System;
using System.Collections.Generic;

namespace AbaBackend.Model.Client
{
  public class AdjustClientDataCollectModel
  {
    public int ClientId { get; set; }
    public List<int> Problems { get; set; }
    public List<int> Replacements { get; set; }
    public string Action { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}