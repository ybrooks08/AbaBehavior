using System;

namespace AbaBackend.DataModel
{
  public class ClientProblemSto
  {
    public int ClientProblemStoId { get; set; }
    public int ClientProblemId { get; set; }
    public int Quantity { get; set; }
    public int Weeks { get; set; }
    public StoStatus Status { get; set; } = StoStatus.Unknow;
    public DateTime WeekStart { get; set; }
    public DateTime WeekEnd { get; set; }
  }
}