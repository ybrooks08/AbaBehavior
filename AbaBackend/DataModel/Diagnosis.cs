using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class Diagnosis
  {
    public int DiagnosisId { get; set; }

    [MaxLength(20)]
    public string Code { get; set; }

    [MaxLength(100)]
    public string Description { get; set; }
  }
}