using System;
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
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? AddedDate { get; set; }
    public bool IsMain { get; set; }
  }
}