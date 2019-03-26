using System;
using System.Collections.Generic;

namespace AbaBackend.DataModel
{
  public class ClientProblem
  {
    public int ClientProblemId { get; set; }
    public int ClientId { get; set; }
    public int ProblemId { get; set; }
    public DateTime? BaselineFrom { get; set; }
    public DateTime? BaselineTo { get; set; }
    public int? BaselineCount { get; set; }
    public ProblemBehavior ProblemBehavior { get; set; }
    public List<ClientProblemSto> STOs { get; set; }
  }
}

