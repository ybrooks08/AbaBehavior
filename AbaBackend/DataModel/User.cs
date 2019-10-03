using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbaBackend.DataModel
{
  public class User
  {
    public int UserId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; }
    [Required]
    [MaxLength(50)]
    [NotMapped]
    public string Password { get; set; }
    [Required]
    [MaxLength(60)]
    public string Firstname { get; set; }
    [Required]
    [MaxLength(100)]
    public string Lastname { get; set; }
    [Required]
    [MaxLength(100)]
    public string Email { get; set; }
    public int RolId { get; set; }
    public Rol Rol { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public Boolean Active { get; set; } = true;
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
    [MaxLength(20)]
    public string Npi { get; set; }
    [MaxLength(20)]
    public string Mpi { get; set; }
    [MaxLength(20)]
    public string LicenseNo { get; set; }
    [MaxLength(11)]
    public string SocialSecurity { get; set; }
    [MaxLength(15)]
    public string Phone { get; set; }
    [MaxLength(100)]
    public string Address { get; set; }
    [MaxLength(10)]
    public string Apt { get; set; }
    [MaxLength(50)]
    public string City { get; set; }
    [MaxLength(50)]
    public string State { get; set; }
    [MaxLength(10)]
    public string Zipcode { get; set; }
    [MaxLength(70)]
    public string BankName { get; set; }
    [MaxLength(100)]
    public string BankAddress { get; set; }
    [MaxLength(20)]
    public string BankRoutingNumber { get; set; }
    [MaxLength(20)]
    public string BankAccountNumber { get; set; }
    [Column(TypeName = "decimal(6,2)")]
    public decimal PayRate { get; set; }
    [Column(TypeName = "decimal(6,2)")]
    public decimal DriveTimePayRate { get; set; }
    public List<DocumentUser> Documents { get; set; }
    public UserSign UserSign { get; set; }
    public List<AuthPass> Passes { get; set; }
    public DayOfWeekBit SessionsDateAllowed { get; set; }
  }
}