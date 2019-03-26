using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbaBackend.DataModel
{
  public class CompetencyCheck
  {
    public int CompetencyCheckId { get; set; }
    public CompetencyCheckType CompetencyCheckType { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; }
    public int? CaregiverId { get; set; }
    public Caregiver Caregiver { get; set; }
    public int? UserId { get; set; }
    public User User { get; set; }
    public int TotalDuration { get; set; }
    public DateTime Date { get; set; }
    public int EvaluationById { get; set; }
    [ForeignKey("EvaluationById")]
    public User EvaluationBy { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalScore { get; set; }
    public List<CompetencyCheckClientParam> CompetencyCheckClientParams { get; set; }
  }
}