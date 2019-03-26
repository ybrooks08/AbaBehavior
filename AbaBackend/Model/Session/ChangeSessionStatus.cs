using AbaBackend.DataModel;

namespace AbaBackend.Model.Session
{
  public class ChangeSessionStatus
  {
    // public int ServiceLogId { get; set; }
    public int SessionId { get; set; }
    public SessionStatus SessionStatus { get; set; }
  }
}