using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public enum CompetencyCheckType
  {
    Rbt = 1,
    Caregiver = 2
  }
  public class CompetencyCheckParam
  {
    public int CompetencyCheckParamId { get; set; }
    public CompetencyCheckType CompetencyCheckType { get; set; }
    [MaxLength(250)]
    public string Description { get; set; }
    [MaxLength(100)]
    public string Comment { get; set; }
  }
}