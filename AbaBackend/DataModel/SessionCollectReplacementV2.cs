namespace AbaBackend.DataModel
{
  public class SessionCollectReplacementV2
  {
    public int SessionCollectReplacementV2Id { get; set; }
    public int SessionId { get; set; }
    public int ReplacementId { get; set; }
    public int ClientId { get; set; }
    public int Total { get; set; }
    public int Completed { get; set; }
    public bool NoData { get; set; }
    public ReplacementProgram Replacement { get; set; }
  }
}