using System;

namespace AbaBackend.DataModel
{
  public class SessionLog
  {
    public int SessionLogId { get; set; }
    public int SessionId { get; set; }
    public DateTimeOffset Entry { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public string IconColor { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
  }
}