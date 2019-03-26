using System;

namespace AbaBackend.Model.Session
{
  public class EditSessionTime
  {
    public int SessionId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
  }
}