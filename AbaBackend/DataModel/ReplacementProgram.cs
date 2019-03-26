using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class ReplacementProgram
  {
    [Key]
    public int ReplacementId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ReplacementProgramDescription { get; set; }

    public bool Active { get; set; } = true;
  }
}