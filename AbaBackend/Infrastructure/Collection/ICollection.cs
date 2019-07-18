using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using static AbaBackend.Infrastructure.Collection.Collection;

namespace AbaBackend.Infrastructure.Collection
{
  public interface ICollection
  {
    Task<List<CollectionBeh>> GetCollectionBehaviors(DateTime start, DateTime end, int clientId, List<int> problemId);
    Task<List<CollectionBehCaregiver>> GetCollectionBehaviorsCaregiver(DateTime start, DateTime end, int clientId, List<int> problemId);
    int? GetClientProblems(List<CollectionBeh> collection, List<CollectionBehCaregiver> caregiverCollection, ClientProblem clientProblem);
    Task<object> GetClientBehaviorChart(int clientId, List<int> problems, DateTime? start = null, DateTime? end = null);
    Task<List<CollectionRep>> GetCollectionReplacements(DateTime start, DateTime end, int clientId, List<int> replacementId);
    Task<List<CollectionRepCaregiver>> GetCollectionReplacementsCaregiver(DateTime start, DateTime end, int clientId, List<int> replacementId);
    int? GetClientReplacements(List<CollectionRep> collections, List<CollectionRepCaregiver> collectionsCaregiver);
    Task<object> GetClientReplacementChart(int clientId, List<int> replacements, DateTime? start = null, DateTime? end = null);
  }
}