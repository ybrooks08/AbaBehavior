namespace AbaBackend.DataModel
{
  public class CaregiverDataCollectionProblem
  {
    public int CaregiverDataCollectionProblemId { get; set; }
    public int CaregiverDataCollectionId { get; set; }
    public int ProblemId { get; set; }
    public int? Count { get; set; }
  }
}