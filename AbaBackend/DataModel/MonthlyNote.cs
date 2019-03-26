using System;

namespace AbaBackend.DataModel
{
  public class MonthlyNote
  {
    public int MonthlyNoteId { get; set; }
    public string MonthlySummary { get; set; }
    public string CommentsAboutCaregiver { get; set; }
    public string Services2ProvideNextMonth { get; set; }
    public string RecipientHealthIssues { get; set; }
    public string Medication { get; set; }
    public string FamilyChanges { get; set; }
    public string HomeChanges { get; set; }
    public string ProviverChanges { get; set; }
    public string Barriers2Treatment { get; set; }
    public bool ContinueNextMonth { get; set; }
    public bool ReassessmentNextMonth { get; set; }
    public bool Refer2OtherServices { get; set; }
    public bool ChangesCurrentPlan { get; set; }
    public string ExtraNotes { get; set; }
    public int ClientId { get; set; }
    public DateTime MonthlyNoteDate { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
  }
}