using System.ComponentModel.DataAnnotations;

namespace AbaBackend.Model.User
{
  public class ModChangeUserPassword
  {
    public int userId { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 6)]
    public string Password { get; set; }

    [Required]
    [Compare("Password")]
    [StringLength(50, MinimumLength = 6)]
    public string RePassword { get; set; }
  }
}