using System.ComponentModel.DataAnnotations;
using AbaBackend.DataModel;

namespace AbaBackend.Model.Tellus
{
  public class AddTellusConfig
  {
    public int TellusCredentialId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; }    
    [Required]
    [StringLength(50, MinimumLength = 6)]
    public string Password { get; set; }    
  }
}