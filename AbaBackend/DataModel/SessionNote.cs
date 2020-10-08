namespace AbaBackend.DataModel
{
  public enum RiskBehavior
  {
    Monitored = 1,
    Addressed = 2,
    NA = 3
  }

  public enum ParticipationLevel
  {
    Good = 1,
    Fair = 2,
    Poor = 3
  }

  public class SessionNote
  {
    public int SessionNoteId { get; set; }

    public int SessionId { get; set; }

    //public Session Session { get; set; }

    public int? CaregiverId { get; set; }

    public string CaregiverNote { get; set; }

    public Caregiver Caregiver { get; set; }

    public RiskBehavior RiskBehavior { get; set; }

    public bool RiskBehaviorCrisisInvolved { get; set; }

    public string RiskBehaviorExplain { get; set; }

    public string ReinforcersEdibles { get; set; }

    public string ReinforcersNonEdibles { get; set; }

    public string ReinforcersOthers { get; set; }

    public string ReinforcersResult { get; set; }

    public ParticipationLevel ParticipationLevel { get; set; }

    public string ProgressNotes { get; set; }

    public bool FeedbackCaregiver { get; set; }

    public string FeedbackCaregiverExplain { get; set; }

    public bool FeedbackOtherServices { get; set; }

    public string FeedbackOtherServicesExplain { get; set; }

    public bool CaregiverTrainingObservationFeedback { get; set; }

    public bool CaregiverTrainingParentCaregiverTraining { get; set; }

    public bool CaregiverTrainingCompetencyCheck { get; set; }

    public string CaregiverTrainingOther { get; set; }

    public string CaregiverTrainingSummary { get; set; }

    public bool SummaryDirectObservation { get; set; }

    public bool SummaryObservationFeedback { get; set; }

    public bool SummaryImplementedReduction { get; set; }

    public bool SummaryImplementedReplacement { get; set; }

    public bool SummaryGeneralization { get; set; }

    public bool SummaryCommunication { get; set; }
    public string SummaryOther { get; set; }

    public bool Supervision1 { get; set; }
    public bool Supervision2 { get; set; }
    public bool Supervision3 { get; set; }
    public bool Supervision4 { get; set; }
    public bool Supervision5 { get; set; }
    public bool Supervision6 { get; set; }
    public bool Supervision7 { get; set; }
    public string SupervisionOther { get; set; }

    public string RejectNotes { get; set; }
    public string SessionResult { get; set; }
  }
}