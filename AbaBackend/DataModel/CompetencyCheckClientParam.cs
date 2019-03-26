using System.ComponentModel.DataAnnotations.Schema;

namespace AbaBackend.DataModel
{
  public class CompetencyCheckClientParam
  {
    public int CompetencyCheckClientParamId { get; set; }
    public int CompetencyCheckId { get; set; }
    public int CompetencyCheckParamId { get; set; }
    public CompetencyCheckParam CompetencyCheckParam { get; set; }
    public string Comment { get; set; }
    public byte Score { get; set; }
  }
}