using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class BehaviorAnalysisCode
  {
    public int BehaviorAnalysisCodeId { get; set; }

    [MaxLength(10)]
    public string Hcpcs { get; set; }

    [MaxLength(60)]
    public string Description { get; set; }

    public bool Checkable { get; set; }

    [MaxLength(60)]
    public string Color { get; set; }
  }
}