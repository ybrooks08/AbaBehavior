using System;

namespace AbaBackend.Model.Client
{
  public class AddClientDiagnosisModel
  {
    public int ClientId { get; set; }

    public string Code { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsMain { get; set; }
    public int ClientDiagnosisId { get; set; }
  }
}