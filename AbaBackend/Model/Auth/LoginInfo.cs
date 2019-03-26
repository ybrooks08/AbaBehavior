using System.ComponentModel.DataAnnotations;

namespace AbaBackend.Model.Auth
{
  public class LoginInfo
  {
    [Required]
    [StringLength(50)]
    [MinLength(2)]
    public string Username { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 6)]
    public string Password { get; set; }
  }

}