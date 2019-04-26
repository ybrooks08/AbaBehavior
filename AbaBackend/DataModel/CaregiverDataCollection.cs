using System;
using System.Collections.Generic;

namespace AbaBackend.DataModel
{
  public class CaregiverDataCollection
  {
    public int CaregiverDataCollectionId { get; set; }
    public int ClientId { get; set; }
    public DateTimeOffset CollectDate { get; set; }
    public int CaregiverId { get; set; }

    public List<CaregiverDataCollectionProblem> CaregiverDataCollectionProblems { get; set; }
    public List<CaregiverDataCollectionReplacement> CaregiverDataCollectionReplacements { get; set; }
  }
}