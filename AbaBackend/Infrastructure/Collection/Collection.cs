using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Extensions;
using AbaBackend.Infrastructure.Utils;
using AbaBackend.Model.Client;
using AbaBackend.Model.Session;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;

namespace AbaBackend.Infrastructure.Collection
{
  public class Collection : ICollection
  {
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
      if (problemId.Count == 0) return new List<CollectionBeh>();
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
      if (problemId.Count == 0) return new List<CollectionBehCaregiver>();
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
      if (replacementId.Count == 0) return new List<CollectionRep>();
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
      if (replacementId.Count == 0) return new List<CollectionRepCaregiver>();
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
      _dbContext = dbContext;
      _utils = utils;
      _connectionString = configuration.GetConnectionString("AbaDbConnectionString");
    }

    public int? GetClientProblems(List<CollectionBeh> collection, List<CollectionBehCaregiver> caregiverCollection, bool isPercent)
    {
      var collectionNoData = collection.Count(w => w.NoData);
      var collectionWithData = collection.Count - collectionNoData;
      var collectionSum = !isPercent ? collection.Sum(s => s.Total) : collection.Sum(s => s.Total == 0 ? 0 : s.Completed / (decimal)s.Total * 100);
      var collectionCompleted = collection.Sum(s => s.Completed);
      var percent = collectionSum == 0 ? 0 : collectionSum / collectionWithData;//collectionCompleted / (decimal)collectionSum * 100;
      var totalCollection = collectionNoData == collection.Count ? null : !isPercent ? (int?)collectionSum : (int?)Math.Round(percent);

      var caregiverCollectionNoData = caregiverCollection.Count(w => w == null);
      var caregiverCollectionSum = caregiverCollection.Sum(s => s.Count);
      var totalCaregiverCollection = caregiverCollectionNoData == caregiverCollection.Count ? null : caregiverCollectionSum;

      if (totalCaregiverCollection == null & totalCollection == null) return null;
      return (totalCaregiverCollection ?? 0) + (totalCollection ?? 0);
    }

    public List<ValueWeek> GetClientProblemsByWeek(int problemId, DateTime firstWeekStart, DateTime lastWeekEnd, List<CollectionBeh> allCollection, List<CollectionBehCaregiver> allCaregiverCollection, bool isPercent)
    {
      var res = new List<ValueWeek>();
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

        var sum = GetClientProblems(collections, collectionsCaregiver, isPercent);
        res.Add(new ValueWeek
        {
          Start = firstWeekStart,
          End = calWeekEnd,
          Total = sum
        });
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
        var collection = GetClientProblemsByWeek(p.ProblemId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection, p.ProblemBehavior.IsPercent);
        data.AddRange(collection.Select(s => s.Total));

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

    public async Task<object> GetClientBehaviorMonthlyChart(int clientId, int problemId, DateTime? end = null)
    {
      var dataSet = new List<MultiSerieChart>();
      var plotLines = new List<PlotLine>();
      var plotBands = new List<PlotBand>();
      var clientProblem = (await _utils.GetClientBehaviors(clientId)).FirstOrDefault(w => w.ProblemId == problemId);
      if (clientProblem == null) return new OkObjectResult(new { chartOptions = new { series = new List<int>() } }).Value;

      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = clientProblem?.BaselineFrom ?? currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end ?? DateTime.Today;
      while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday) lastWeekEnd = lastWeekEnd.AddDays(1);
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var allCollection = await GetCollectionBehaviors(firstWeekStart, lastWeekEnd, clientId, new List<int> { problemId });
      var allCaregiverCollection = await GetCollectionBehaviorsCaregiver(firstWeekStart, lastWeekEnd, clientId, new List<int> { problemId });

      var notes = await _dbContext.ClientChartNotes
        .Where(w => w.ClientId == clientId)
        .Where(w => w.ChartNoteType == ChartNoteType.Both || w.ChartNoteType == ChartNoteType.Problem)
        .Where(w => w.ChartNoteDate >= firstWeekStart && w.ChartNoteDate <= lastWeekEnd)
        .OrderBy(o => o.ChartNoteDate)
        .ToListAsync();

      var data = new List<int?>();
      var collection = GetClientProblemsByWeek(problemId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection, clientProblem.ProblemBehavior.IsPercent);
      data.AddRange(collection.Select(s => s.Total));

      dataSet.Add(new MultiSerieChart
      {
        Data = data,
        Name = $"{clientProblem.ProblemBehavior.ProblemBehaviorDescription}{(clientProblem.ProblemBehavior.IsPercent ? "(%)" : "")}"
      });

      var baseLineStart = 0;
      var baseLineEnd = 0;
      var legend = new List<string>();
      var calWeekStartLegend = firstWeekStart;
      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = calWeekStartLegend.AddDays(6);
        legend.Add(calWeekEnd.ToString("M/d/yy"));
        var notesWeek = notes.Where(w => w.ChartNoteDate >= calWeekStartLegend && w.ChartNoteDate <= calWeekEnd).ToList();
        foreach (var n in notesWeek) plotLines.Add(new PlotLine { Label = new Label { Text = n.Title }, Value = i });
        if (clientProblem.BaselineFrom >= calWeekStartLegend && clientProblem.BaselineFrom <= calWeekEnd) baseLineStart = i;
        if (clientProblem.BaselineTo >= calWeekStartLegend && clientProblem.BaselineTo <= calWeekEnd) baseLineEnd = i;
        calWeekStartLegend = calWeekStartLegend.AddDays(7);
      }

      //if (baseLineStart != 0 || baseLineEnd != 0) 
      plotBands.Add(new PlotBand { Label = new Label { Text = "" }, From = baseLineStart, To = baseLineEnd });
      // if (baseLineEnd != 0)
      {
        plotLines.Add(new PlotLine { Label = new Label { Text = "Baseline" }, Value = baseLineEnd, Color = "Blue", DashStyle = "Solid" });
        baseLineEnd++;
        dataSet.First().Data.Insert(baseLineEnd, null);
        legend.Insert(baseLineEnd, "");
      }

      var firstValue = collection.Select((s, i) => new { s.Total, i }).ToList().FirstOrDefault(w => w.Total != 0 && w.Total != null)?.i ?? 0;
      var start = baseLineEnd == 0 ? firstValue - 1 : baseLineEnd + 1;
      plotLines.Add(new PlotLine { Label = new Label { Text = "Treatment" }, Value = start, Color = "Green", DashStyle = "Solid" });
      plotLines.ForEach(w =>
      {
        if (w.Value > baseLineEnd + 1) w.Value += 1;
      });


      var res = new OkObjectResult(new
      {
        chartOptions = new
        {
          series = dataSet,
          xAxis = new
          {
            categories = legend,
            plotLines,
            plotBands,
            title = new { enabled = false },
            crosshair = true
          },
          title = new { text = "" },
          chart = new { type = "spline", height = 200 },
          yAxis = new { title = new { text = "Count" }, min = 0, max = dataSet.First().Data.Max() < 100 ? 100 : dataSet.First().Data.Max(), tickInterval = 10 },
          legend = new { enabled = false },
          exporting = new { enabled = true }
        },
      }).Value;

      return res;
    }

    public int? GetClientReplacements(List<CollectionRep> collections, List<CollectionRepCaregiver> collectionsCaregiver)
    {
      var collectionNoData = collections.Count(w => w.NoData);
      var collectionWithData = collections.Count - collectionNoData;
      var collectionSum = collections.Sum(s => s.Total == 0 ? 0 : s.Completed / (decimal)s.Total * 100);
      var percent = collectionSum == 0 ? 0 : collectionSum / collectionWithData;

      // var totalTrial = collections.Sum(s => s.Total);
      // var totalCompleted = collections.Sum(s => s.Completed);
      //
      // var hasNoData = collectionNoData == collections.Count;
      //
      // var caregiverCollection = collectionsCaregiver
      //   .GroupBy(w => w)
      //   .Select(s => new
      //   {
      //     TotalTrial = s.Sum(w => w.TotalTrial),
      //     TotalCompleted = s.Sum(w => w.TotalCompleted),
      //     TotalTrialNull = s.Count(w => w.TotalTrial == null),
      //     TotalCompletedNull = s.Count(w => w.TotalCompleted == null)
      //   }).FirstOrDefault();
      //
      // var caregiverCollectionNoData = caregiverCollection == null || caregiverCollection.TotalCompleted == null || caregiverCollection.TotalTrial == null ||
      //                                 (caregiverCollection.TotalTrialNull == collectionsCaregiver.Count);
      // if (hasNoData & caregiverCollectionNoData) return null;
      //
      // var caregiverTotalTrial = caregiverCollection?.TotalTrial ?? 0;
      // var caregiverTotalCompleted = caregiverCollection?.TotalCompleted ?? 0;
      //
      // var trials = totalTrial + caregiverTotalTrial;
      // var completed = totalCompleted + caregiverTotalCompleted;
      // var percent = trials == 0 ? 0 : completed / (decimal)trials * 100;

      return (int?)Math.Round(percent);
    }

    public List<ValueWeek> GetClientReplacementsByWeek(int replacementId, DateTime firstWeekStart, DateTime lastWeekEnd, List<CollectionRep> allCollection, List<CollectionRepCaregiver> allCaregiverCollection)
    {
      var res = new List<ValueWeek>();
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
        res.Add(new ValueWeek
        {
          Start = firstWeekStart,
          End = calWeekEnd,
          Total = sum
        });
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
        data.AddRange(collection.Select(s => s.Total));

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
          yAxis = new { title = new { text = "Trials percent" }, max = 100 },
          legend = new { enabled = replacementsProcess.Count != 1 },
          exporting = new { chartOptions = new { title = new { text = replacementsProcess.Count != 1 ? "" : replacementsProcess.First().Replacement.ReplacementProgramDescription } } }
        },
        notes
      }).Value;

      return res;
    }

    public async Task<object> GetClientReplacementMonthlyChart(int clientId, int replacementId, DateTime? end = null)
    {
      var dataSet = new List<MultiSerieChart>();
      var plotLines = new List<PlotLine>();
      var plotBands = new List<PlotBand>();
      var clientReplacement = (await _utils.GetClientReplacements(clientId)).FirstOrDefault(w => w.ReplacementId == replacementId);
      if (clientReplacement == null) return new OkObjectResult(new { chartOptions = new { series = new List<int>() } }).Value;

      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = clientReplacement?.BaselineFrom ?? currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end ?? DateTime.Today;
      while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday) lastWeekEnd = lastWeekEnd.AddDays(1);
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var allCollection = await GetCollectionReplacements(firstWeekStart, lastWeekEnd, clientId, new List<int> { replacementId });
      var allCaregiverCollection = await GetCollectionReplacementsCaregiver(firstWeekStart, lastWeekEnd, clientId, new List<int> { replacementId });

      var notes = await _dbContext.ClientChartNotes
        .Where(w => w.ClientId == clientId)
        .Where(w => w.ChartNoteType == ChartNoteType.Both || w.ChartNoteType == ChartNoteType.Replacement)
        .Where(w => w.ChartNoteDate >= firstWeekStart && w.ChartNoteDate <= lastWeekEnd)
        .OrderBy(o => o.ChartNoteDate)
        .ToListAsync();

      var data = new List<int?>();
      var collection = GetClientReplacementsByWeek(replacementId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection);

      var allData = collection.Select(s => s.Total);

      var found = false;
      foreach (var i in allData)
      {
        if (i == 0 && !found) data.Add(null);
        else { data.Add(i); found = true; }
      }

      //data.AddRange(collection.Select(s => s.Total));



      dataSet.Add(new MultiSerieChart
      {
        Data = data,
        Name = clientReplacement.Replacement.ReplacementProgramDescription,
      });

      var baseLineStart = 0;
      var baseLineEnd = 0;
      var legend = new List<string>();
      var calWeekStartLegend = firstWeekStart;
      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = calWeekStartLegend.AddDays(6);
        legend.Add(calWeekEnd.ToString("M/d/yy"));
        var notesWeek = notes.Where(w => w.ChartNoteDate >= calWeekStartLegend && w.ChartNoteDate <= calWeekEnd).ToList();
        foreach (var n in notesWeek) plotLines.Add(new PlotLine { Label = new Label { Text = n.Title }, Value = i });
        if (clientReplacement.BaselineFrom >= calWeekStartLegend && clientReplacement.BaselineFrom <= calWeekEnd) baseLineStart = i;
        if (clientReplacement.BaselineTo >= calWeekStartLegend && clientReplacement.BaselineTo <= calWeekEnd) baseLineEnd = i;
        calWeekStartLegend = calWeekStartLegend.AddDays(7);
      }

      // if (baseLineStart != 0 || baseLineEnd != 0) 
      plotBands.Add(new PlotBand { Label = new Label { Text = "" }, From = baseLineStart, To = baseLineEnd });
      // if (baseLineEnd != 0)
      {
        plotLines.Add(new PlotLine { Label = new Label { Text = "Baseline" }, Value = baseLineEnd, Color = "Blue", DashStyle = "Solid" });
        baseLineEnd++;
        dataSet.First().Data.Insert(baseLineEnd, null);
        legend.Insert(baseLineEnd, "");
      }
      var firstValue = collection.Select((s, i) => new { s.Total, i }).ToList().FirstOrDefault(w => w.Total != 0 && w.Total != null)?.i ?? 0;
      var start = baseLineEnd == 0 ? firstValue - 1 : baseLineEnd + 1;
      plotLines.Add(new PlotLine { Label = new Label { Text = "Treatment" }, Value = start, Color = "Green", DashStyle = "Solid" });
      plotLines.ForEach(w =>
      {
        if (w.Value > baseLineEnd + 1) w.Value += 1;
      });

      var res = new OkObjectResult(new
      {
        chartOptions = new
        {
          series = dataSet,
          xAxis = new
          {
            categories = legend,
            plotLines,
            plotBands,
            title = new { enabled = false },
            crosshair = true
          },
          title = new { text = "" },
          chart = new { type = "spline", height = 200 },
          yAxis = new { title = new { text = "Trials percent" }, min = 0, max = dataSet.First().Data.Max() < 100 ? 100 : dataSet.First().Data.Max(), tickInterval = 10 },
          legend = new { enabled = false },
          exporting = new { enabled = true }
        },
      }).Value;

      return res;
    }

    public async Task<List<int?>> GetClientBehaviorChartValuesWeek(int clientId, int problemId, DateTime? start = null, DateTime? end = null)
    {
      var problem = await _dbContext.ProblemBehaviors.FirstAsync(w => w.ProblemId == problemId);
      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start ?? currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end ?? DateTime.Today;
      if (end == null)
        while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday)
          lastWeekEnd = lastWeekEnd.AddDays(1);

      var allCollection = await GetCollectionBehaviors(firstWeekStart, lastWeekEnd, clientId, new List<int> { problemId });
      var allCaregiverCollection = await GetCollectionBehaviorsCaregiver(firstWeekStart, lastWeekEnd, clientId, new List<int> { problemId });
      var collection = GetClientProblemsByWeek(problemId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection, problem.IsPercent);
      return collection.Select(s => s.Total).ToList();
    }

    public async Task<CurrentStoStatusBehavior> GetStoStatusBehavior(int clientId, int problemId)
    {
      var stos = await _dbContext.ClientProblems
        .Where(w => w.ClientId == clientId && w.ProblemId == problemId)
        .Include(i => i.STOs)
        .Select(s => s.STOs.OrderBy(o => o.ClientProblemStoId).ToList())
        .FirstOrDefaultAsync();

      var stoWithIndex = stos.Select((s, i) => new ClientProblemSto
      {
        ClientProblemId = s.ClientProblemId,
        ClientProblemStoId = s.ClientProblemStoId,
        Quantity = s.Quantity,
        Status = s.Status,
        WeekEnd = s.WeekEnd,
        WeekStart = s.WeekStart,
        Weeks = s.Weeks,
        Index = i + 1
      }).ToList();

      var lastMastered = stoWithIndex.LastOrDefault(w => w.Status == StoStatus.Mastered);
      var current = stoWithIndex.FirstOrDefault(w => w.Status == StoStatus.InProgress);

      return new CurrentStoStatusBehavior { LastMasteredSto = lastMastered, CurrentSto = current };
    }

    public async Task<List<int?>> GetClientReplacementChartValuesWeek(int clientId, int replacementId, DateTime? start = null, DateTime? end = null)
    {
      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start ?? currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      DateTime lastWeekEnd = end ?? DateTime.Today;
      if (end == null)
        while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday)
          lastWeekEnd = lastWeekEnd.AddDays(1);

      var allCollection = await GetCollectionReplacements(firstWeekStart, lastWeekEnd, clientId, new List<int> { replacementId });
      var allCaregiverCollection = await GetCollectionReplacementsCaregiver(firstWeekStart, lastWeekEnd, clientId, new List<int> { replacementId });
      var collection = GetClientReplacementsByWeek(replacementId, firstWeekStart, lastWeekEnd, allCollection, allCaregiverCollection);
      return collection.Select(s => s.Total).ToList();
    }

    public async Task<CurrentStoStatusReplacement> GetStoStatusReplacement(int clientId, int replacementId)
    {
      var stos = await _dbContext.ClientReplacements
        .Where(w => w.ClientId == clientId && w.ReplacementId == replacementId)
        .Include(i => i.STOs)
        .Select(s => s.STOs.OrderBy(o => o.ClientReplacementStoId).ToList())
        .FirstOrDefaultAsync();

      var stoWithIndex = stos.Select((s, i) => new ClientReplacementSto
      {
        ClientReplacementId = s.ClientReplacementId,
        ClientReplacementStoId = s.ClientReplacementStoId,
        Percent = s.Percent,
        Status = s.Status,
        WeekEnd = s.WeekEnd,
        WeekStart = s.WeekStart,
        Weeks = s.Weeks,
        Index = i + 1
      }).ToList();

      var lastMastered = stoWithIndex.LastOrDefault(w => w.Status == StoStatus.Mastered);
      var current = stoWithIndex.FirstOrDefault(w => w.Status == StoStatus.InProgress);

      return new CurrentStoStatusReplacement { LastMasteredSto = lastMastered, CurrentSto = current };
    }

    public async Task<List<MonthlyBehaviorContract>> GetMonthlyDataBehavior(int clientId, DateTime month)
    {
      var data = new List<MonthlyBehaviorContract>();
      //return data;
      var endMonth = month.AddMonths(1).AddDays(-1);
      var start = month.GetPrevDay(DayOfWeek.Sunday);
      var end = endMonth.GetPrevDay(DayOfWeek.Saturday);
      //var totalWeeks = ((end - start).Days + 1) / 7;

      var stoCalculated = await GetProblemStoOnDate(clientId, end);

      var behaviors = await _utils.GetClientBehaviors(clientId);
      foreach (var behavior in behaviors)
      {
        var weekValues = await GetClientBehaviorChartValuesWeek(clientId, behavior.ProblemId, start, end);
        var weeksWithData = weekValues.Count(s => s != null);
        var stos = stoCalculated
            .Where(w => w.ClientProblemId == behavior.ClientProblemId)
            .SelectMany(s => s.STOs)
            .ToList()
            .Select((s, i) => new ClientStoBehaviorContract
            {
              Weeks = s.Weeks,
              Quantity = s.Quantity,
              Status = s.Status.ToString(),
              StatusNo = s.Status,
              Index = i + 1,
              Start = s.WeekStart,
              End = s.WeekEnd,
              MasteredForced = s.MasteredForced
            }).ToList();

        var newBeh = new MonthlyBehaviorContract
        {
          Behavior = behavior.ProblemBehavior.ProblemBehaviorDescription,
          IsPercent = behavior.ProblemBehavior.IsPercent,
          ProblemId = behavior.ProblemId,
          Baseline = behavior.BaselineCount,
          BaselineFrom = behavior.BaselineFrom,
          BaselineTo = behavior.BaselineTo,
          WeekAverage = weeksWithData == 0 ? 0 : weekValues.Sum() / (decimal)weeksWithData,//(decimal)weekValues.Count,
          Total = weekValues.Sum(),
          Stos = stos,
        };
        data.Add(newBeh);
      }

      return data;
    }

    public async Task<List<MonthlyReplacementContract>> GetMonthlyDataReplacement(int clientId, DateTime month)
    {
      var data = new List<MonthlyReplacementContract>();

      var endMonth = month.AddMonths(1).AddDays(-1);
      var start = month.GetPrevDay(DayOfWeek.Sunday);
      var end = endMonth.GetPrevDay(DayOfWeek.Saturday);
      //var totalWeeks = ((end - start).Days + 1) / 7;

      var stoCalculated = await GetReplacementStoOnDate(clientId, end);

      // var replacements = (await _utils.GetClientReplacements(clientId)).Where(w => w.ReplacementId == 12).ToList();
      var replacements = await _utils.GetClientReplacements(clientId);
      foreach (var replacement in replacements)
      {
        var weekValues = await GetClientReplacementChartValuesWeek(clientId, replacement.ReplacementId, start, end);
        var weeksWithData = weekValues.Count(s => s != null);
        var stos = stoCalculated
            .Where(w => w.ClientReplacementId == replacement.ClientReplacementId)
            .SelectMany(s => s.STOs)
            .ToList()
            .Select((s, i) => new ClientStoReplacementContract
            {
              Weeks = s.Weeks,
              Percent = s.Percent,
              Status = s.Status.ToString(),
              StatusNo = s.Status,
              Index = i + 1,
              Start = s.WeekStart,
              End = s.WeekEnd,
              LevelAssistance = s.LevelAssistance,
              TimeMinutes = s.TimeMinutes,
              MasteredForced = s.MasteredForced
            }).ToList();

        var newRpl = new MonthlyReplacementContract
        {
          Replacement = replacement.Replacement.ReplacementProgramDescription,
          ReplacementId = replacement.ReplacementId,
          Baseline = replacement.BaselinePercent,
          BaselineFrom = replacement.BaselineFrom,
          BaselineTo = replacement.BaselineTo,
          WeekAverage = weeksWithData == 0 ? 0 : weekValues.Sum() / (decimal)weeksWithData,//weekValues.Sum() / (decimal)weekValues.Count,
          Stos = stos,
        };
        data.Add(newRpl);
      }
      return data;
    }

    async Task<List<ClientProblem>> GetProblemStoOnDate(int clientId, DateTime endDate)
    {
      var data = new List<ClientProblem>();

      var (_, start, end) = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      while (endDate.DayOfWeek != DayOfWeek.Saturday) endDate = endDate.AddDays(1);

      var clientBehaviors = await _utils.GetClientBehaviors(clientId, false);
      if (clientBehaviors.Count == 0) return data;

      var clientBehaviorsListId = clientBehaviors.Select(s => s.ProblemId).ToList();
      var allStos = await _dbContext.ClientProblemSTOs.AsNoTracking().Where(w => clientBehaviors.Select(s => s.ClientProblemId).Contains(w.ClientProblemId)).ToListAsync();
      allStos.ForEach(f => f.Status = StoStatus.Unknow);

      var allCollection = await GetCollectionBehaviors(firstWeekStart, endDate, clientId, clientBehaviorsListId);
      var allCaregiverCollection = await GetCollectionBehaviorsCaregiver(firstWeekStart, endDate, clientId, clientBehaviorsListId);

      foreach (var clientBehavior in clientBehaviors)
      {
        var newBeh = new ClientProblem
        {
          Active = clientBehavior.Active,
          BaselineCount = clientBehavior.BaselineCount,
          BaselineFrom = clientBehavior.BaselineFrom,
          BaselineTo = clientBehavior.BaselineTo,
          ClientId = clientBehavior.ClientId,
          ClientProblemId = clientBehavior.ClientProblemId,
          ProblemBehavior = clientBehavior.ProblemBehavior,
          ProblemId = clientBehavior.ProblemId
        };

        var valuesByWeek = GetClientProblemsByWeek(clientBehavior.ProblemId, firstWeekStart, endDate, allCollection, allCaregiverCollection, clientBehavior.ProblemBehavior.IsPercent);
        var stos = allStos.Where(w => w.ClientProblemId == clientBehavior.ClientProblemId).OrderBy(o => o.ClientProblemStoId).ToList();
        if (stos.Count == 0) continue;

        foreach (var sto in stos)
        {
          if (sto.MasteredForced)
          {
            sto.Status = StoStatus.Mastered;
            continue;
          };
          var qty = sto.Quantity;
          var weeks = sto.Weeks;

          var checkWeeks = valuesByWeek.FindConsecutiveGroups(week => week.Total <= qty, weeks).FirstOrDefault();
          if (checkWeeks != null)
          {
            sto.WeekStart = checkWeeks.First().Start;
            sto.WeekEnd = checkWeeks.Last().End;
            sto.Status = StoStatus.Mastered;
          }

        }
        var currentSto = allStos.Where(w => w.ClientProblemId == clientBehavior.ClientProblemId && w.Status != StoStatus.Mastered).OrderBy(o => o.ClientProblemStoId).FirstOrDefault();
        if (currentSto != null) currentSto.Status = StoStatus.InProgress;
        newBeh.STOs = stos;
        data.Add(newBeh);
      }

      return data;
    }

    async Task<List<ClientReplacement>> GetReplacementStoOnDate(int clientId, DateTime endDate)
    {
      var data = new List<ClientReplacement>();

      var (_, start, end) = await _utils.GetClientWholePeriod(clientId);
      var firstWeekStart = start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      while (endDate.DayOfWeek != DayOfWeek.Saturday) endDate = endDate.AddDays(1);

      // var clientReplacements = (await _utils.GetClientReplacements(clientId, false)).Where(w => w.ReplacementId == 12).ToList(); ;
      var clientReplacements = await _utils.GetClientReplacements(clientId, false);
      if (clientReplacements.Count == 0) return data;

      var clientReplacementsListId = clientReplacements.Select(s => s.ReplacementId).ToList();
      var allStos = await _dbContext.ClientReplacementSTOs.AsNoTracking().Where(w => clientReplacements.Select(s => s.ClientReplacementId).Contains(w.ClientReplacementId)).ToListAsync();
      allStos.ForEach(f => f.Status = StoStatus.Unknow);

      var allCollection = await GetCollectionReplacements(firstWeekStart, endDate, clientId, clientReplacementsListId);
      var allCaregiverCollection = await GetCollectionReplacementsCaregiver(firstWeekStart, endDate, clientId, clientReplacementsListId);

      foreach (var clientReplacement in clientReplacements)
      {
        var newRep = new ClientReplacement
        {
          Active = clientReplacement.Active,
          BaselinePercent = clientReplacement.BaselinePercent,
          BaselineFrom = clientReplacement.BaselineFrom,
          BaselineTo = clientReplacement.BaselineTo,
          ClientId = clientReplacement.ClientId,
          ClientReplacementId = clientReplacement.ClientReplacementId,
          Replacement = clientReplacement.Replacement,
          ReplacementId = clientReplacement.ReplacementId
        };

        var valuesByWeek = GetClientReplacementsByWeek(clientReplacement.ReplacementId, firstWeekStart, endDate, allCollection, allCaregiverCollection);
        var stos = allStos.Where(w => w.ClientReplacementId == clientReplacement.ClientReplacementId).OrderBy(o => o.ClientReplacementStoId).ToList();
        if (stos.Count == 0) continue;
        var groupStoMinutes = stos.Where(w => w.TimeMinutes != null).Select(s => s.TimeMinutes).Distinct().ToList();
        var groupStoAssistance = stos.Where(w => w.LevelAssistance != null).Select(s => s.LevelAssistance).Distinct().ToList();
        var groups = groupStoMinutes.Count > 0 ? groupStoMinutes.Select(s => s.ToString()).ToList() : groupStoAssistance.ToList();

        if (groupStoMinutes.Count == 0 && groupStoAssistance.Count == 0) groups.Add("all");


        foreach (var group in groups)
        {
          var stoGrouped = groupStoMinutes.Count > 0
          ? stos.Where(w => w.TimeMinutes == Convert.ToInt32(group)).OrderBy(o => o.ClientReplacementStoId).ToList()
          : stos.Where(w => w.LevelAssistance == group).OrderBy(o => o.ClientReplacementStoId).ToList();

          if (group == "all") stoGrouped = stos;

          DateTime? maxWeek = null;

          var forceNoBreak = false;
          var totalMastered = 0;
          foreach (var sto in stoGrouped)
          {
            if (sto.MasteredForced)
            {
              sto.Status = StoStatus.Mastered;
              forceNoBreak = true;
              continue;
            };
            forceNoBreak = false;
            var qty = sto.Percent;
            var weeks = sto.Weeks;

            var checkWeeks = valuesByWeek.FindConsecutiveGroups(week => week.Total >= qty, weeks).FirstOrDefault();
            if (checkWeeks != null)
            {
              sto.WeekStart = checkWeeks.First().Start;
              sto.WeekEnd = checkWeeks.Last().End;
              sto.Status = StoStatus.Mastered;
              totalMastered++;
              maxWeek = checkWeeks.Last().End;
            }
          }
          if (maxWeek != null) valuesByWeek.RemoveAll(w => w.End <= maxWeek);
          else if (!forceNoBreak) break;
          // if (totalMastered != stoGrouped.Count())
          // {
          //   var currentSto = allStos.Where(w => w.ClientReplacementId == clientReplacement.ClientReplacementId && w.Status != StoStatus.Mastered).OrderBy(o => o.ClientReplacementStoId).FirstOrDefault();
          //   if (currentSto != null) currentSto.Status = StoStatus.InProgress;
          //   break;
          // }
        }

        var lastMastered = stos.OrderBy(o => o.ClientReplacementStoId).Where(w => w.Status == StoStatus.Unknow).FirstOrDefault();
        if (lastMastered != null)
        {
          lastMastered.Status = StoStatus.InProgress;
        }
        newRep.STOs = stos;
        data.Add(newRep);
      }
      return data;
    }
  }
}