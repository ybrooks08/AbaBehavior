namespace AbaBackend.Model.Client
{
  public class AddRemoveClientProblem
  {
    public int ClientId { get; set; }
    public int ProblemId { get; set; }
    public char Action { get; set; }
  }
}