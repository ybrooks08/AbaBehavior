using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class ClientDiagnosis
  {
    public int ClientDiagnosisId { get; set; }

    public int ClientId { get; set; }

    public int DiagnosisId { get; set; }

    public Diagnosis Diagnosis { get; set; }

    public bool Active { get; set; } = true;
  }
}