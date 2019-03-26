using System;

namespace AbaBackend.DataModel
{
  [Flags]
  public enum WorkWith
  {
    //None = 0,
    Client = 1,
    Caregiver = 2,
    Rbt = 4,
    BCABA = 8,
    Other = 16
  }

  public enum OversightSessionSupervision
  {
    Satisfactory = 1,
    Needs_improvements = 2,
    Unsatisfactory = 3,
    Not_applicable = 4
  }

  public class SessionSupervisionNote
  {
    public int SessionSupervisionNoteId { get; set; }
    public int SessionId { get; set; }
    public WorkWith WorkWith { get; set; }
    public bool isDirectSession { get; set; } = true;
    public bool BriefObservation { get; set; }
    public bool BriefReplacement { get; set; }
    public bool BriefGeneralization { get; set; }
    public bool BriefBCaBaTraining { get; set; }
    public bool BriefInService { get; set; }
    public string BriefInServiceSubject { get; set; }
    public bool BriefOther { get; set; }
    public string BriefOtherDescription { get; set; }
    public OversightSessionSupervision OversightFollowUp { get; set; }
    public OversightSessionSupervision OversightDesigning { get; set; }
    public OversightSessionSupervision OversightContributing { get; set; }
    public OversightSessionSupervision OversightAnalyzing { get; set; }
    public OversightSessionSupervision OversightGoals { get; set; }
    public OversightSessionSupervision OversightMakingDecisions { get; set; }
    public OversightSessionSupervision OversightModeling { get; set; }
    public OversightSessionSupervision OversightResponse { get; set; }
    public OversightSessionSupervision OversightOverall { get; set; }
    public bool OversightFollowUpBool { get; set; }
    public bool OversightDesigningBool { get; set; }
    public bool OversightContributingBool { get; set; }
    public bool OversightAnalyzingBool { get; set; }
    public bool OversightGoalsBool { get; set; }
    public bool OversightMakingDecisionsBool { get; set; }
    public bool OversightModelingBool { get; set; }
    public bool OversightResponseBool { get; set; }
    public bool OversightOverallBool { get; set; }
    public string CommentsRelated { get; set; }
    public string Recommendations { get; set; }
    public bool Validation { get; set; }
    public DateTime? NextScheduledDate { get; set; }
  }
}