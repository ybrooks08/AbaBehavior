using System;

namespace AbaBackend.DataModel
{
  public class AuthPass
  {
    public int AuthPassId { get; set; }
    public int UserId { get; set; }
    public DateTime Created { get; set; }
    public DateTime? UsedDate { get; set; }
    public bool Used { get; set; } = false;
  }
}