using System.Collections.Generic;

namespace AbaBackend.DataModel
{
  public class SessionProblemNote
  {
    public int SessionProblemNoteId { get; set; }
    public int SessionId { get; set; }
    public int ProblemId { get; set; }
    public ProblemBehavior ProblemBehavior { get; set; }
    public string DuringWichActivities { get; set; }
    public string ReplacementInterventionsUsed { get; set; }
    public List<SessionProblemNoteReplacement> SessionProblemNoteReplacements { get; set; }
  }
}