using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class Caregiver
  {
    public int CaregiverId { get; set; }

    public int ClientId { get; set; }

    [Required]
    [MaxLength(60)]
    public string CaregiverFullname { get; set; }

    [MaxLength(15)]
    public string Phone { get; set; }

    [MaxLength(60)]
    public string Email { get; set; }

    public int CaregiverTypeId { get; set; }
    public CaregiverType CaregiverType { get; set; }
  }
}