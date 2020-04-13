using System;
using System.ComponentModel.DataAnnotations;

namespace AbaBackend.DataModel
{
  public class Assessment
  {
    public int AssessmentId { get; set; }
    public int ClientId { get; set; }
    public int BehaviorAnalysisCodeId { get; set; }
    public BehaviorAnalysisCode BehaviorAnalysisCode { get; set; }
    public int TotalUnits { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [MaxLength(12)]
    public string PaNumber { get; set; }
    public Client Client { get; set; }
    public int Status { get; set; }  //0 = Not billed, 1 = Billed
    public int? TotalUnitsWeek { get; set; }
  }
}