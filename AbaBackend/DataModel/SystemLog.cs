using System;

namespace AbaBackend.DataModel
{
  public enum SystemLogType
  {
    Info,
    Warning,
    Error,
    Critical,
    Unknow
  }

  public enum Module
  {
    Client,
    User
  }

  public class SystemLog
  {
    public int SystemLogId { get; set; }
    public DateTimeOffset Entry { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public SystemLogType SystemLogType { get; set; }
    public Module Module { get; set; }
    public string ModuleValue { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
  }
}