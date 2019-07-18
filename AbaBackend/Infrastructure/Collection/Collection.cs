using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Extensions;
using AbaBackend.Infrastructure.Utils;
using AbaBackend.Model.Session;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AbaBackend.Infrastructure.Collection
{
  public class Collection : ICollection
  {
    readonly IConfiguration _configuration;
    readonly AbaDbContext _dbContext;
    readonly IUtils _utils;
    readonly string _connectionString;

    public class CollectionBeh : SessionCollectBehaviorV2
    {
      public DateTime SessionStart { get; set; }
    }

    public class CollectionBehCaregiver : CaregiverDataCollectionProblem
    {
      public DateTime CollectDate { get; set; }
    }

    public class CollectionRep : SessionCollectReplacementV2
    {
      public DateTime SessionStart { get; set; }
    }

    public class CollectionRepCaregiver : CaregiverDataCollectionReplacement
    {
      public DateTime CollectDate { get; set; }
    }

    public async Task<List<CollectionBeh>> GetCollectionBehaviors(DateTime start, DateTime end, int clientId, List<int> problemId)
    {
      var query = $@"SELECT
                      CONVERT(date, S.SessionStart) as SessionStart,
                      C.*
                    FROM Sessions S
                    JOIN SessionCollectBehaviorsV2 C ON S.SessionId = C.SessionId
                    WHERE
                      CONVERT(date, S.SessionStart) BETWEEN '{start.ToShortDateString()}' and '{end.ToShortDateString()}'
                      AND S.ClientId = {clientId}
                      AND C.ProblemId IN ({problemId.ConvertIdToListDelimitedByComma()})";
      using (SqlConnection db = new SqlConnection(_connectionString))
      {
        var c = (await db.QueryAsync<CollectionBeh>(query, commandTimeout: 180)).ToList();
        return c;
      }
    }

    public async Task<List<CollectionBehCaregiver>> GetCollectionBehaviorsCaregiver(DateTime start, DateTime end, int clientId, List<int> problemId)
    {
      var query = $@"select 
                      CONVERT(date, C.CollectDate) as CollectDate,
                      P.*
                    FROM CaregiverDataCollections C
                    JOIN CaregiverDataCollectionProblems P ON C.CaregiverDataCollectionId = P.CaregiverDataCollectionId
                    WHERE 
                      C.CollectDate BETWEEN '{start.ToShortDateString()}' and '{end.ToShortDateString()}'
                      AND C.ClientId = {clientId}
                      AND P.ProblemId IN ({problemId.ConvertIdToListDelimitedByComma()})";
      using (SqlConnection db = new SqlConnection(_connectionString))
      {
        var c = (await db.QueryAsync<CollectionBehCaregiver>(query, commandTimeout: 180)).ToList();
        return c;
      }
    }

    public async Task<List<CollectionRep>> GetCollectionReplacements(DateTime start, DateTime end, int clientId, List<int> replacementId)
    {
      var query = $@"SELECT
                      CONVERT(date, S.SessionStart) as SessionStart,
                      C.*
                    FROM Sessions S
                    JOIN SessionCollectReplacementsV2 C ON S.SessionId = C.SessionId
                    WHERE
                      CONVERT(date, S.SessionStart) BETWEEN '{start.ToShortDateString()}' and '{end.ToShortDateString()}'
                      AND S.ClientId = {clientId}
                      AND C.ReplacementId IN ({replacementId.ConvertIdToListDelimitedByComma()})";
      using (SqlConnection db = new SqlConnection(_connectionString))
      {
        var c = (await db.QueryAsync<CollectionRep>(query, commandTimeout: 180)).ToList();
        return c;
      }
    }

    public async Task<List<CollectionRepCaregiver>> GetCollectionReplacementsCaregiver(DateTime start, DateTime end, int clientId, List<int> replacementId)
    {
      var query = $@"select 
                      CONVERT(date, C.CollectDate) as CollectDate,
                      P.*
                    FROM CaregiverDataCollections C
                    JOIN CaregiverDataCollectionReplacements P ON C.CaregiverDataCollectionId = P.CaregiverDataCollectionId
                    WHERE 
                      C.CollectDate BETWEEN '{start.ToShortDateString()}' and '{end.ToShortDateString()}'
                      AND C.ClientId = {clientId}
                      AND P.ReplacementId IN ({replacementId.ConvertIdToListDelimitedByComma()})";
      using (SqlConnection db = new SqlConnection(_connectionString))
      {
        var c = (await db.QueryAsync<CollectionRepCaregiver>(query, commandTimeout: 180)).ToList();
        return c;
      }
    }

    public Collection(IConfiguration configuration, AbaDbContext dbContext, IUtils utils)
    {
      _configuration = configuration;
      _dbContext = dbContext;
      _utils = utils;
      _connectionString = configuration.GetConnectionString("AbaDbConnectionString");
    }

    public int? GetClientProblems(List<CollectionBeh> collection, List<CollectionBehCaregiver> caregiverCollection, ClientProblem clientProblem)
    {
      var collectionNoData = collection.Count(w => w.NoData);
      var collectionSum = collection.Sum(s => s.Total);
      var collectionCompleted = collection.Sum(s => s.Completed);
      var percent = collectionSum == 0 ? 0 : collectionCompleted / (decimal)collectionSum * 100;
      var totalCollection = collectionNoData == collection.Count ? null : !clientProblem.ProblemBehavior.IsPercent ? (int?)collectionSum : (int?)Math.Round(percent);

      var caregiverCollectionNoData = caregiverCollection.Count(w => w == null);
      var caregiverCollectionSum = caregiverCollection.Sum(s => s.Count);
      var totalCaregiverCollection = caregiverCollectionNoData == caregiverCollection.Count ? null : caregiverCollectionSum;

      if (totalCaregiverCollection == null & totalCollection == null) return null;
      return (totalCaregiverCollection ?? 0) + (totalCollection ?? 0);
    }

    List<int?> GetClientProblemsByWeek(int problemId, DateTime firstWeekStart, DateTime lastWeekEnd, List<CollectionBeh> allCollection, List<CollectionBehCaregiver> allCaregiverCollection, ClientProblem clientProblem)
    {
      var res = new List<int?>();
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var collection = allCollection
        .Where(w => w.SessionStart >= firstWeekStart && w.SessionStart <= lastWeekEnd)
        .Where(w => w.ProblemId == problemId)
        .ToList();

      var collectionCaregiver = allCaregiverCollection
        .Where(w => w.CollectDate >= firstWeekStart && w.CollectDate <= lastWeekEnd)
        .Where(w => w.ProblemId == problemId)
        .ToList();

      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = firstWeekStart.AddDays(6);

        var collections = collection
          .Where(w => w.SessionStart.Date >= firstWeekStart && w.SessionStart.Date <= calWeekEnd)
          .ToList();
        var collectionsCaregiver = collectionCaregiver
          .Where(w => w.CollectDate >= firstWeekStart && w.CollectDate <= calWeekEnd)
          .ToList();

        var sum = GetClientProblems(collections, collectionsCaregiver, clientProblem);
        res.Add(sum);
        firstWeekStart = firstWeekStart.AddDays(7);
      }

      return res;
    }

    public async Task<object> GetClientBehaviorChart(int clientId, List<int> problems = null, DateTime? start = null, DateTime? end = null)
    {
      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start ?? currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end ?? DateTime.Today;
      if (end == null)
        while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday)
          lastWeekEnd = lastWeekEnd.AddDays(1);
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var notes = await _dbContext.ClientChartNotes
        .Where(w => w.ClientId == clientId)
        .Where(w => w.ChartNoteType == ChartNoteType.Both || w.ChartNoteType == ChartNoteType.Problem)
        .Where(w => w.ChartNoteDate >= firstWeekStart && w.ChartNoteDate <= lastWeekEnd)
        .OrderBy(o => o.ChartNoteDate)
        .ToListAsync();

      var clientProblems = await _utils.GetClientBehaviors(clientId);
      var problemsProcess = (problems == null || problems.Count == 0 ? clientProblems : clientProblems.Where(w => problems.Contains(w.ProblemId)).ToList()).OrderBy(o => o.ProblemBehavior.ProblemBehaviorDescription).ToList();

      var allCollection = await GetCollectionBehaviors(firstWeekStart, lastWeekEnd, clientId, problemsProcess.Select(s => s.ProblemId).ToList());
      var allCaregiverCollection = await GetCollectionBehaviorsCaregiver(firstWeekStart, lastWeekEnd, clientId, problemsProcess.Select(s => s.ProblemId).ToList());

      var dataSet = new List<MultiSerieChart>();
      var plotLines = new List<PlotLine>
      {
        new PlotLine {Label = new Label {Text = "Baseline"}, Value = 0, Color = "Blue", DashStyle = "ShortDot"},
        new PlotLine {Label = new Label {Text = "Start"}, Value = 2, Color = "Green", DashStyle = "ShortDot"}
      };

      foreach (var p in problemsProcess)
      {
        var data = new List<int?> { p.BaselineCount, null };
        var collection = GetClientProblemsByWeek(p.ProblemId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection, p);
        data.AddRange(collection);

        dataSet.Add(new MultiSerieChart
        {
          Data = data,
          Name = $"{p.ProblemBehavior.ProblemBehaviorDescription}{(p.ProblemBehavior.IsPercent ? "(%)" : "")}"
        });
      }

      var legend = new List<string> { "Base", "" };
      var calWeekStartLegend = firstWeekStart;
      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = calWeekStartLegend.AddDays(6);
        legend.Add(calWeekEnd.ToString("M/d/yy"));

        var notesWeek = notes.Where(w => w.ChartNoteDate >= calWeekStartLegend && w.ChartNoteDate <= calWeekEnd).ToList();
        foreach (var n in notesWeek) plotLines.Add(new PlotLine { Label = new Label { Text = n.Title }, Value = i + 2 });

        calWeekStartLegend = calWeekStartLegend.AddDays(7);
      }

      var res = new OkObjectResult(new
      {
        chartOptions = new
        {
          series = dataSet,
          xAxis = new { categories = legend, plotLines, title = new { text = "Weeks (label is last day of week)" }, crosshair = true },
          title = new { text = problemsProcess.Count != 1 ? "" : problemsProcess.First().ProblemBehavior.ProblemBehaviorDescription },
          chart = new { type = "spline", height = problemsProcess.Count != 1 ? null : "350" },
          tooltip = new { shared = true },
          yAxis = new { title = new { text = "Count" } },
          legend = new { enabled = problemsProcess.Count != 1 },
          exporting = new { chartOptions = new { title = new { text = problemsProcess.Count != 1 ? "" : problemsProcess.First().ProblemBehavior.ProblemBehaviorDescription } } }
        },
        notes
      }).Value;

      return res;
    }

    public int? GetClientReplacements(List<CollectionRep> collections, List<CollectionRepCaregiver> collectionsCaregiver)
    {
      var collectionNoData = collections.Count(w => w.NoData);
      var totalTrial = collections.Sum(s => s.Total);
      var totalCompleted = collections.Sum(s => s.Completed);

      var hasNoData = collectionNoData == collections.Count;

      var caregiverCollection = collectionsCaregiver
        .GroupBy(w => w)
        .Select(s => new
        {
          TotalTrial = s.Sum(w => w.TotalTrial),
          TotalCompleted = s.Sum(w => w.TotalCompleted),
          TotalTrialNull = s.Count(w => w.TotalTrial == null),
          TotalCompletedNull = s.Count(w => w.TotalCompleted == null)
        })
        .FirstOrDefault();

      var caregiverCollectionNoData = caregiverCollection == null || caregiverCollection.TotalCompleted == null || caregiverCollection.TotalTrial == null ||
                                      (caregiverCollection.TotalTrialNull == collectionsCaregiver.Count || caregiverCollection.TotalCompleted == collectionsCaregiver.Count);
      if (hasNoData & caregiverCollectionNoData) return null;

      var caregiverTotalTrial = caregiverCollection?.TotalTrial ?? 0;
      var caregiverTotalCompleted = caregiverCollection?.TotalCompleted ?? 0;

      var trials = totalTrial + caregiverTotalTrial;
      var completed = totalCompleted + caregiverTotalCompleted;
      var percent = trials == 0 ? 0 : completed / (decimal)trials * 100;

      return (int?)percent;
      ;
    }

    List<int?> GetClientReplacementsByWeek(int replacementId, DateTime firstWeekStart, DateTime lastWeekEnd, List<CollectionRep> allCollection, List<CollectionRepCaregiver> allCaregiverCollection)
    {
      var res = new List<int?>();
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var collection = allCollection
        .Where(w => w.SessionStart >= firstWeekStart && w.SessionStart <= lastWeekEnd)
        .Where(w => w.ReplacementId == replacementId)
        .ToList();

      var collectionCaregiver = allCaregiverCollection
        .Where(w => w.CollectDate >= firstWeekStart && w.CollectDate <= lastWeekEnd)
        .Where(w => w.ReplacementId == replacementId)
        .ToList();

      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = firstWeekStart.AddDays(6);

        var collections = collection
          .Where(w => w.SessionStart.Date >= firstWeekStart && w.SessionStart.Date <= calWeekEnd)
          .ToList();
        var collectionsCaregiver = collectionCaregiver
          .Where(w => w.CollectDate >= firstWeekStart && w.CollectDate <= calWeekEnd)
          .ToList();

        var sum = GetClientReplacements(collections, collectionsCaregiver);
        res.Add(sum);
        firstWeekStart = firstWeekStart.AddDays(7);
      }

      return res;
    }

    public async Task<object> GetClientReplacementChart(int clientId, List<int> replacements = null, DateTime? start = null, DateTime? end = null)
    {
      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start ?? currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end ?? DateTime.Today;
      if (end == null)
        while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday)
          lastWeekEnd = lastWeekEnd.AddDays(1);
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var notes = await _dbContext.ClientChartNotes
        .Where(w => w.ClientId == clientId)
        .Where(w => w.ChartNoteType == ChartNoteType.Both || w.ChartNoteType == ChartNoteType.Replacement)
        .Where(w => w.ChartNoteDate >= firstWeekStart && w.ChartNoteDate <= lastWeekEnd)
        .OrderBy(o => o.ChartNoteDate)
        .ToListAsync();

      var clientReplacements = await _utils.GetClientReplacements(clientId);
      var replacementsProcess = (replacements == null || replacements.Count == 0 ? clientReplacements : clientReplacements.Where(w => replacements.Contains(w.ReplacementId)).ToList()).OrderBy(o => o.Replacement.ReplacementProgramDescription).ToList();

      var allCollection = await GetCollectionReplacements(firstWeekStart, lastWeekEnd, clientId, replacementsProcess.Select(s => s.ReplacementId).ToList());
      var allCaregiverCollection = await GetCollectionReplacementsCaregiver(firstWeekStart, lastWeekEnd, clientId, replacementsProcess.Select(s => s.ReplacementId).ToList());

      var dataSet = new List<MultiSerieChart>();
      var plotLines = new List<PlotLine>
      {
        new PlotLine {Label = new Label {Text = "Baseline"}, Value = 0, Color = "Blue", DashStyle = "ShortDot"},
        new PlotLine {Label = new Label {Text = "Start"}, Value = 2, Color = "Green", DashStyle = "ShortDot"}
      };

      foreach (var p in replacementsProcess)
      {
        var data = new List<int?> { p.BaselinePercent, null };
        var collection = GetClientReplacementsByWeek(p.ReplacementId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection);
        data.AddRange(collection);

        dataSet.Add(new MultiSerieChart
        {
          Data = data,
          Name = p.Replacement.ReplacementProgramDescription,
        });
      }

      var legend = new List<string> { "Base", "" };
      var calWeekStartLegend = firstWeekStart;
      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = calWeekStartLegend.AddDays(6);
        legend.Add(calWeekEnd.ToString("M/d/yy"));

        var notesWeek = notes.Where(w => w.ChartNoteDate >= calWeekStartLegend && w.ChartNoteDate <= calWeekEnd).ToList();
        foreach (var n in notesWeek) plotLines.Add(new PlotLine { Label = new Label { Text = n.Title }, Value = i + 2 });

        calWeekStartLegend = calWeekStartLegend.AddDays(7);
      }

      var res = new OkObjectResult(new
      {
        chartOptions = new
        {
          series = dataSet,
          xAxis = new { categories = legend, plotLines, title = new { text = "Weeks (label is last day of week)" }, crosshair = true },
          title = new { text = replacementsProcess.Count != 1 ? "" : replacementsProcess.First().Replacement.ReplacementProgramDescription },
          chart = new { type = "spline", height = replacementsProcess.Count != 1 ? null : "350" },
          tooltip = new { shared = true },
          yAxis = new { title = new { text = "Trials percent" } },
          legend = new { enabled = replacementsProcess.Count != 1 },
          exporting = new { chartOptions = new { title = new { text = replacementsProcess.Count != 1 ? "" : replacementsProcess.First().Replacement.ReplacementProgramDescription } } }
        },
        notes
      }).Value;

      return res;
    }
  }
}