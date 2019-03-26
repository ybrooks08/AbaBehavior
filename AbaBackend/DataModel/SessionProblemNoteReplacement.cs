namespace AbaBackend.DataModel
{
  public class SessionProblemNoteReplacement
  {
    public int SessionProblemNoteReplacementId { get; set; }
    public int SessionProblemNoteId { get; set; }
    public int ReplacementId { get; set; }
    public ReplacementProgram ReplacementProgram { get; set; }
    public bool Active { get; set; }
  }
}