using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbaBackend.DataModel
{
  public class ClientReplacementSto
  {
    public int ClientReplacementStoId { get; set; }
    public int ClientReplacementId { get; set; }
    public int Percent { get; set; }
    public int Weeks { get; set; }
    public StoStatus Status { get; set; } = StoStatus.Unknow;
    public DateTime? WeekStart { get; set; }
    public DateTime? WeekEnd { get; set; }
    [NotMapped]
    public int Index { get; set; }
    public bool MasteredForced { get; set; }
    public string LevelAssistance { get; set; }
    public int? TimeMinutes { get; set; }
  }
}