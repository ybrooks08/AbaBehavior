using System.ComponentModel.DataAnnotations;

namespace AbaBackend.Model.MasterTables
{
  public class ProblemsBehaviors
  {
    [Required]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Description { get; set; }
  }
}