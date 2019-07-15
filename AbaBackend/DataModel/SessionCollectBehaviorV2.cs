namespace AbaBackend.DataModel
{
  public class SessionCollectBehaviorV2
  {
    public int SessionCollectBehaviorV2Id { get; set; }
    public int SessionId { get; set; }
    public int ProblemId { get; set; }
    public int ClientId { get; set; }
    public int Total { get; set; }
    public int Completed { get; set; }
    public bool NoData { get; set; }
    public ProblemBehavior Behavior { get; set; }
  }
}