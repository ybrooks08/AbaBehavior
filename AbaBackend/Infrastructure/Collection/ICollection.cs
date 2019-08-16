using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Model.Client;

namespace AbaBackend.Infrastructure.Collection
{
  public class ValueWeek
  {
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
    public int? Total { get; set; }
  }

  public interface ICollection
  {
    Task<List<Collection.CollectionBeh>> GetCollectionBehaviors(DateTime start, DateTime end, int clientId, List<int> problemId);
    Task<List<Collection.CollectionBehCaregiver>> GetCollectionBehaviorsCaregiver(DateTime start, DateTime end, int clientId, List<int> problemId);
    int? GetClientProblems(List<Collection.CollectionBeh> collection, List<Collection.CollectionBehCaregiver> caregiverCollection, bool isPercent);
    Task<object> GetClientBehaviorChart(int clientId, List<int> problems, DateTime? start = null, DateTime? end = null);
    List<ValueWeek> GetClientProblemsByWeek(int problemId, DateTime firstWeekStart, DateTime lastWeekEnd, List<Collection.CollectionBeh> allCollection, List<Collection.CollectionBehCaregiver> allCaregiverCollection, bool isPercent);

    Task<List<Collection.CollectionRep>> GetCollectionReplacements(DateTime start, DateTime end, int clientId, List<int> replacementId);
    Task<List<Collection.CollectionRepCaregiver>> GetCollectionReplacementsCaregiver(DateTime start, DateTime end, int clientId, List<int> replacementId);
    int? GetClientReplacements(List<Collection.CollectionRep> collections, List<Collection.CollectionRepCaregiver> collectionsCaregiver);
    Task<object> GetClientReplacementChart(int clientId, List<int> replacements, DateTime? start = null, DateTime? end = null);
    List<ValueWeek> GetClientReplacementsByWeek(int replacementId, DateTime firstWeekStart, DateTime lastWeekEnd, List<Collection.CollectionRep> allCollection, List<Collection.CollectionRepCaregiver> allCaregiverCollection);
    Task<List<int?>> GetClientBehaviorChartValuesWeek(int clientId, int problemId, DateTime? start = null, DateTime? end = null);
    Task<CurrentStoStatusBehavior> GetStoStatusBehavior(int clientId, int problemId);
    Task<List<int?>> GetClientReplacementChartValuesWeek(int clientId, int replacementId, DateTime? start = null, DateTime? end = null);
    Task<CurrentStoStatusReplacement> GetStoStatusReplacement(int clientId, int replacementId);
  }
}