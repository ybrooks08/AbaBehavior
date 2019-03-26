using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class CaregiverType
  {
    public int CaregiverTypeId { get; set; }

    [Required]
    [MaxLength(60)]
    public string Description { get; set; }
  }
}