using System.ComponentModel.DataAnnotations;
using AbaBackend.DataModel;

namespace AbaBackend.Model.User
{
  public class AddEditUser
  {
    public int UserId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; }
    [Required]
    [MaxLength(100)]
    public string Email { get; set; }
    [Required]
    [StringLength(50, MinimumLength = 6)]
    public string Password { get; set; }
    [Required]
    [Compare("Password")]
    [StringLength(50, MinimumLength = 6)]
    public string Repassword { get; set; }
    [Required]
    [MaxLength(60)]
    public string Firstname { get; set; }
    [Required]
    [MaxLength(100)]
    public string Lastname { get; set; }
    [Required]
    [Range(1, 10, ErrorMessage = "You must provide a valid rol")]
    public int RolId { get; set; }

    public string Npi { get; set; }
    public string Mpi { get; set; }
    public string LicenseNo { get; set; }
    public string SocialSecurity { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public string Apt { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zipcode { get; set; }
    public string BankName { get; set; }
    public string BankAddress { get; set; }
    public string BankRoutingNumber { get; set; }
    public string BankAccountNumber { get; set; }
    public decimal PayRate { get; set; }
    public decimal DriveTimePayRate { get; set; }
    public DayOfWeekBit SessionsDateAllowed { get; set; }
  }
}