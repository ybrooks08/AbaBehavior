using System;
using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public enum MessageType
  {
    General = 0,
    Referral = 1,
    Assessment = 2,
    Hr = 3
  }
  public class Email
  {
    public int EmailId { get; set; }
    [Required]
    [MaxLength(100)]
    public string To { get; set; }
    [Required]
    [MaxLength(100)]
    public string Subject { get; set; }
    public string Body { get; set; }
    public MessageType MesssageType { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Sent { get; set; }
  }
}