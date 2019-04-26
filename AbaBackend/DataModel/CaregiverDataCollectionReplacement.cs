namespace AbaBackend.DataModel
{
  public class CaregiverDataCollectionReplacement
  {
    public int CaregiverDataCollectionReplacementId { get; set; }
    public int CaregiverDataCollectionId { get; set; }
    public int ReplacementId { get; set; }
    public int? TotalTrial { get; set; }
    public int? TotalCompleted { get; set; }
  }
}