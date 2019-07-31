using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbaBackend.DataModel;


namespace AbaBackend.Infrastructure.Collection
{
  public interface ICollection
  {
    Task<List<Collection.CollectionBeh>> GetCollectionBehaviors(DateTime start, DateTime end, int clientId, List<int> problemId);
    Task<List<Collection.CollectionBehCaregiver>> GetCollectionBehaviorsCaregiver(DateTime start, DateTime end, int clientId, List<int> problemId);
    int? GetClientProblems(List<Collection.CollectionBeh> collection, List<Collection.CollectionBehCaregiver> caregiverCollection, ClientProblem clientProblem);
    Task<object> GetClientBehaviorChart(int clientId, List<int> problems, DateTime? start = null, DateTime? end = null);
    Task<List<Collection.CollectionRep>> GetCollectionReplacements(DateTime start, DateTime end, int clientId, List<int> replacementId);
    Task<List<Collection.CollectionRepCaregiver>> GetCollectionReplacementsCaregiver(DateTime start, DateTime end, int clientId, List<int> replacementId);
    int? GetClientReplacements(List<Collection.CollectionRep> collections, List<Collection.CollectionRepCaregiver> collectionsCaregiver);
    Task<object> GetClientReplacementChart(int clientId, List<int> replacements, DateTime? start = null, DateTime? end = null);
  }
}