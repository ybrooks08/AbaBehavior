using System;
using System.Collections.Generic;

namespace AbaBackend.DataModel
{
  public class ClientReplacement
  {
    public int ClientReplacementId { get; set; }
    public int ClientId { get; set; }
    public int ReplacementId { get; set; }
    public DateTime? BaselineFrom { get; set; }
    public DateTime? BaselineTo { get; set; }
    public int? BaselinePercent { get; set; }
    public bool Active { get; set; }
    public ReplacementProgram Replacement { get; set; }
    public List<ClientReplacementSto> STOs { get; set; }
  }
}