using System;
using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public enum ChartNoteType
  {
    Both = 0,
    Problem = 1,
    Replacement = 2
  }
  public class ClientChartNote
  {
    public int ClientChartNoteId { get; set; }
    public int ClientId { get; set; }
    public ChartNoteType ChartNoteType { get; set; }
    public DateTime ChartNoteDate { get; set; }
    [MaxLength(20)]
    public string Title { get; set; }
    [MaxLength(200)]
    public string Note { get; set; }
  }
}