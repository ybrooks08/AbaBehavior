using AbaBackend.DataModel;

namespace AbaBackend.Model.Client
{
  public class CurrentStoStatusBehavior
  {
    public ClientProblemSto LastMasteredSto { get; set; }
    public ClientProblemSto CurrentSto { get; set; }
  }

  public class CurrentStoStatusReplacement
  {
    public ClientReplacementSto LastMasteredSto { get; set; }
    public ClientReplacementSto CurrentSto { get; set; }
  }
}