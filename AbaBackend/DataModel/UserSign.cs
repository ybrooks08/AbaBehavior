using System;

namespace AbaBackend.DataModel
{
  public class UserSign
  {
    public int UserSignId { get; set; }
    public int UserId { get; set; }
    public string Sign { get; set; }
    public DateTime SignedDate { get; set; } = DateTime.Now;
  }
}