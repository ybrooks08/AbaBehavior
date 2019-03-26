using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class ProblemBehavior
  {
    [Key]
    public int ProblemId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ProblemBehaviorDescription { get; set; }

    public bool Active { get; set; } = true;
  }
}