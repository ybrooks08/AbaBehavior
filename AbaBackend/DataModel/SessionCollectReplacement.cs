using System;

namespace AbaBackend.DataModel
{
  public class SessionCollectReplacement
  {
    public int SessionCollectReplacementId { get; set; }
    public int SessionId { get; set; }
    public int ReplacementId { get; set; }
    public int ClientId { get; set; }
    public DateTime Entry { get; set; }
    public string Notes { get; set; }
    public bool Completed { get; set; }
    public ReplacementProgram Replacement { get; set; }
  }
}