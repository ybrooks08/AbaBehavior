using System;

namespace AbaBackend.DataModel
{
  public class SessionCollectBehavior
  {
    public int SessionCollectBehaviorId { get; set; }
    public int SessionId { get; set; }
    public int ProblemId { get; set; }
    public int ClientId { get; set; }
    public DateTime Entry { get; set; }
    public string Notes { get; set; }
    public int Duration { get; set; }
    public ProblemBehavior Behavior { get; set; }
  }
}