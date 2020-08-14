using System;

namespace AbaBackend.Model.Session
{
  public class AdjustSessionAnalystModel
  {
    public int ClientId { get; set; }
    public int AnalystId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}