using System;
using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class SessionSign
  {
    public int SessionSignId { get; set; }
    public int SessionId { get; set; }
    public string Sign { get; set; }
    public DateTime SignedDate { get; set; }
  }
}