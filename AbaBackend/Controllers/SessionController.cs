using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Reporting;
using AbaBackend.Infrastructure.Reporting.Sessions;
using AbaBackend.Infrastructure.Utils;
using AbaBackend.Model.Session;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using AbaBackend.Model.MasterTables;

namespace AbaBackend.Controllers
{
  public class UrlClass
  {
    public string Url { get; set; }
  }

  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/session")]
  public class SessionController : Controller
  {
    private readonly AbaDbContext _dbContext;
    private IUtils _utils;
    private readonly IConfiguration _configuration;
    private IHostingEnvironment _env;

    public SessionController(AbaDbContext context, IUtils utils, IConfiguration configuration, IHostingEnvironment env)
    {
      _dbContext = context;
      _utils = utils;
      _configuration = configuration;
      _env = env;
    }

    [HttpPost("add-session")]
    public async Task<IActionResult> AddSession([FromBody] Session session)
    {
      using (var transaction = await _dbContext.Database.BeginTransactionAsync())
      {
        try
        {
          if (session.SessionEnd <= session.SessionStart) throw new Exception("Wrong time, please check.");

          //if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
          var user = await _utils.GetCurrentUser();

          //check if client is actived or exists
          var client = await _utils.GetClientById(session.ClientId);
          if (client == null || !client.Active) throw new Exception("Client isn't active or doesn't exists.");

          //check if have expiring documents
          var documentsExpired = await _utils.GetUserExpiredDocumentsCount();
          if (documentsExpired > 0) throw new Exception($"You cannot create any session beacause you have {documentsExpired} expired document(s).");

          // check if user is assigned to the client
          if (!await _utils.CheckAssignmentClientActive(user.UserId, session.ClientId)) throw new Exception("Selected client is not assigned or not actived in your assigments.");

          //check if client has a valid assessment 
          if (!(await _utils.GetClientValidAssessmentsForUser(session.SessionStart, session.ClientId, user)).Any()) throw new Exception($"Client has no {user.Rol.BehaviorAnalysisCode.Hcpcs} assessment or date is invalid.");

          //check if user has units available
          var unitsAvailable = await _utils.GetUnitsAvailable(session.SessionStart, session.ClientId, user);
          if (unitsAvailable <= 0) throw new Exception("No units available");
          if (unitsAvailable < session.TotalUnits) throw new Exception("There are not enough units for this session.");

          //check if client has BCABA service
          if (session.SessionType == SessionType.Supervision_BCABA)
          {
            var bcaba = await _dbContext.Clients
                        .Where(w => w.ClientId.Equals(session.ClientId))
                        .Where(w => w.Assignments.Where(w1 => w1.User.Rol.BehaviorAnalysisCode.Hcpcs == "H2012").Count() > 0)
                        .ToListAsync();
            if (bcaba.Count == 0) throw new Exception("Client haven't any BCBAB service/user.");
          }

          //check if client doesn't exceds max units per day (initial 8 hours = 32 units)
          var check = await _utils.CheckMaxHoursClientsInDay(session.SessionStart, session.ClientId, session.TotalUnits);
          if (check != "ok") throw new Exception(check);

          //check if user doesn't exced max units por day and all clients (initial 10 hours = 40 units)
          check = await _utils.CheckMaxHoursUserInDay(session.SessionStart, user.UserId, session.TotalUnits);
          if (check != "ok") throw new Exception(check);

          //check if user rol session doesnt exced max per session (initial RBT = 6 hours and Analyst and Assistant = 5 hours)
          check = await _utils.CheckMaxHoursUserByClientInDay(session.SessionStart, user.UserId, session.ClientId, session.TotalUnits);
          if (check != "ok") throw new Exception(check);

          //check if session is in school and same provider doesn't exced max units (initial 4 hours = 16 units in school)
          if (session.Pos == Pos.School)
          {
            check = await _utils.CheckMaxHoursByClientInSchool(session.SessionStart, user.UserId, session.ClientId, session.TotalUnits);
            if (check != "ok") throw new Exception(check);
          }

          //check if same user has session overlaping between clients
          check = await _utils.CheckUserSessionOverlap(session.SessionStart, session.SessionEnd, user.UserId);
          if (check != "ok") throw new Exception(check);

          //check if same client has session overlaping between users
          check = await _utils.CheckSessionOverlapSameDayClient(session.SessionStart, session.SessionEnd, session.ClientId);
          if (check != "ok") throw new Exception(check);

          session.SessionStart = session.SessionStart.ToUniversalTime();
          session.SessionEnd = session.SessionEnd.ToUniversalTime();
          session.TotalUnits = Convert.ToInt32((session.SessionEnd - session.SessionStart).TotalMinutes / 15);
          session.UserId = user.UserId;
          session.BehaviorAnalysisCodeId = Convert.ToInt32(user.Rol.BehaviorAnalysisCodeId);
          await _dbContext.Sessions.AddAsync(session);
          await _dbContext.SaveChangesAsync();

          if (session.SessionType != SessionType.Supervision_BCABA)
          {
            var note = new SessionNote { SessionId = session.SessionId };
            await _dbContext.SessionNotes.AddAsync(note);
            await _dbContext.SaveChangesAsync();
          }
          else
          {
            var note = new SessionSupervisionNote { SessionId = session.SessionId };
            await _dbContext.AddAsync(note);
            await _dbContext.SaveChangesAsync();
          }

          if (session.SessionType == SessionType.BA_Service) await _utils.AddSessionProblemNotes(session.SessionId, client.ClientId);

          await _utils.NewEntryLog(session.SessionId, "Created", "Session was created", "fa-info", "purple");

          transaction.Commit();
          return Ok();
        }
        catch (Exception e)
        {
          return BadRequest(e.InnerException?.Message ?? e.Message);
        }
      }
    }

    [HttpGet("get-sessions/{clientId}/{dateStr}")]
    public async Task<IActionResult> GetSessions(int clientId, string dateStr)
    {
      try
      {
        var date = Convert.ToDateTime(dateStr);
        var sessions = await _dbContext
                          .Sessions
                          .OrderBy(o => o.SessionStart)
                          .Where(w => w.SessionStart.Date.Equals(date.Date) || w.SessionEnd.Date.Equals(date.Date))
                          .Where(w => w.ClientId.Equals(clientId))
                          .Select(s => new
                          {
                            id = s.SessionId,
                            start = s.SessionStart,
                            end = s.SessionEnd,
                            content = s.BehaviorAnalysisCode.Hcpcs,
                            Color = s.BehaviorAnalysisCode.Color,
                            Code = s.BehaviorAnalysisCode.Hcpcs,
                            s.User,
                            className = $"white--text v-card--hover  pa-0 ma-0 {s.BehaviorAnalysisCode.Color}",
                            SessionType = Enum.GetName(typeof(SessionType), s.SessionType),
                            s.TotalUnits,
                            Pos = s.Pos.ToString().Replace("_", " "),//Enum.GetName(typeof(Pos), s.Pos),
                            SessionStatus = s.SessionStatus.ToString(),
                            SessionStatusCode = s.SessionStatus,
                            SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString(),
                            // SessionStartNumber = s.SessionStart.ConvertTimeToNumber(),
                            // SessionEndNumber = s.SessionEnd.ConvertTimeToNumber(),
                            // s.User.Rol,
                            s.BehaviorAnalysisCode,
                            // 
                          })
                          .ToListAsync();
        return Ok(sessions);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-session/{sessionId}")]
    public async Task<IActionResult> GetSession(int sessionId)
    {
      try
      {
        var session = await _dbContext
                          .Sessions
                          .AsNoTracking()
                          .Where(w => w.SessionId.Equals(sessionId))
                          .Include(i => i.SessionNote)
                          .Include(i => i.User).ThenInclude(t => t.UserSign)
                          .Include(i => i.SessionSupervisionNote)
                          .Include(i => i.SessionProblemNotes).ThenInclude(i => i.ProblemBehavior)
                          .Include(i => i.SessionProblemNotes).ThenInclude(i => i.SessionProblemNoteReplacements).ThenInclude(d => d.ReplacementProgram)
                          .FirstOrDefaultAsync();
        return Ok(session);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> GetSessionDetailed(int sessionId)
    {
      try
      {
        var session = await _dbContext
                          .Sessions
                          .AsNoTracking()
                          .Where(w => w.SessionId.Equals(sessionId))
                          .Select(s => new
                          {
                            s.SessionId,
                            SessionStart = s.SessionStart.ToString("u"),
                            SessionEnd = s.SessionEnd.ToString("u"),
                            s.TotalUnits,
                            SessionType = s.SessionType.ToString().Replace("_", " "),
                            ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
                            ClientCode = s.Client.Code ?? "N/A",
                            SessionStatus = s.SessionStatus.ToString(),
                            SessionStatusCode = s.SessionStatus,
                            SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString(),
                            Pos = s.Pos.ToString().Replace("_", " "),
                            PosCode = s.Pos,
                            s.BehaviorAnalysisCode.Description,
                            s.BehaviorAnalysisCode.Hcpcs,
                            s.Sign,
                            s.DriveTime,
                            SessionLogs = s.SessionLogs.Select(l => new
                            {
                              l.Entry,
                              l.Icon,
                              l.Title,
                              l.Description,
                              l.IconColor,
                              l.SessionLogId,
                              l.User
                            }).OrderByDescending(o => o.Entry)
                          })
                          .FirstOrDefaultAsync();
        return Ok(session);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-units-available/{clientId}/{dateStr?}/{userName?}")]
    public async Task<IActionResult> GetUnitsAvailable(int clientId, string dateStr, string userName)
    {
      var date = dateStr == null ? DateTime.Today : Convert.ToDateTime(dateStr);
      userName = userName ?? User.Identity.Name;
      var units = await _utils.GetUnitsAvailable(date, clientId, await _utils.GetUserByUsername(userName));
      return Ok(units);
    }

    [HttpGet("get-sessions-calendar/{clientId}")]
    public async Task<IActionResult> GetSessionsCalendar(int clientId)
    {
      try
      {
        var sessions = await _dbContext.Sessions
                      .Where(w => w.ClientId.Equals(clientId))
                      .GroupBy(g => new { g.SessionStart.Date, g.BehaviorAnalysisCode.Color })
                      .Select(s => new
                      {
                        s.Key.Date,
                        s.Key.Color
                      })
                      .ToListAsync();

        dynamic obj = new List<JObject>();
        var colors = sessions.Select(s => s.Color).Distinct().ToList();
        foreach (var color in colors)
        {
          dynamic a = new JObject();
          dynamic backGroundColor = new JObject();
          backGroundColor.backgroundColor = color;
          a.dot = backGroundColor;
          a.dates = new JArray(sessions.Where(w => w.Color.Equals(color)).Select(s => s.Date).ToArray());
          obj.Add(a);
        }
        return Ok(obj);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("edit-session-notes")]
    public async Task<IActionResult> EditSessionNotes([FromBody] Session session)
    {
      try
      {
        //if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        _dbContext.Update(session).Property(s => s.SessionStatus).CurrentValue = SessionStatus.Edited;

        if (session.SessionType == SessionType.Supervision_BCABA)
        {
          _dbContext.SessionSupervisionNotes.Attach(session.SessionSupervisionNote);
          _dbContext.Entry(session.SessionSupervisionNote).State = EntityState.Modified;
        }
        else
        {
          _dbContext.SessionNotes.Attach(session.SessionNote);
          _dbContext.Entry(session.SessionNote).State = EntityState.Modified;
          foreach (var sessionProblem in session.SessionProblemNotes)
          {
            _dbContext.SessionProblemNotes.Attach(sessionProblem);
            _dbContext.Entry(sessionProblem).State = EntityState.Modified;

            foreach (var ReplacementByProblem in sessionProblem.SessionProblemNoteReplacements)
            {
              _dbContext.SessionProblemNoteReplacements.Attach(ReplacementByProblem);
              _dbContext.Entry(ReplacementByProblem).State = EntityState.Modified;
            }
          }
        }
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(session.SessionId, "Edited", "Session notes was edited");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-monthly-note/{clientId}/{dateStr}")]
    public async Task<IActionResult> GetMonthlyNote(int clientId, string dateStr)
    {
      try
      {
        var date = Convert.ToDateTime(dateStr);
        var monthlyLookUp = await _dbContext.MonthlyNotes
                            .Where(w => w.ClientId.Equals(clientId))
                            .Where(w => w.Year == date.Year && w.Month == date.Month)
                            .FirstOrDefaultAsync();
        if (monthlyLookUp == null)
        {
          var newMonthlyNote = new MonthlyNote
          {
            ClientId = clientId,
            MonthlyNoteDate = date.Date,
            Month = date.Month,
            Year = date.Year
          };
          await _dbContext.MonthlyNotes.AddAsync(newMonthlyNote);
          await _dbContext.SaveChangesAsync();
          monthlyLookUp = newMonthlyNote;
        }
        return Ok(monthlyLookUp);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("edit-monthly-note")]
    public async Task<IActionResult> EditMonthlyNote([FromBody] MonthlyNote monthlyNote)
    {
      try
      {
        if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        _dbContext.MonthlyNotes.Attach(monthlyNote);
        _dbContext.Entry(monthlyNote).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}/{problemId?}")]
    public async Task<IActionResult> GetProblemBehaviorsChart(int clientId, int problemId = 0)
    {
      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var mainData = await _dbContext.SessionCollectBehaviors
                                     //.Where(w => w.Entry.Date >= currentPeriod.Start.Date && w.Entry.Date <= currentPeriod.End.Date)
                                     .Where(w => w.ClientId == clientId)
                                     .Where(w => problemId == 0 || w.ProblemId == problemId)
                                     .Include(i => i.Behavior)
                                     .ToListAsync();
      var notes = await _dbContext.ClientChartNotes
                                  //.Where(w => w.ChartNoteDate >= currentPeriod.Start && w.ChartNoteDate <= currentPeriod.End)
                                  .Where(w => w.ClientId == clientId)
                                  .Where(w => w.ChartNoteType == ChartNoteType.Both || w.ChartNoteType == ChartNoteType.Problem)
                                  .OrderBy(o => o.ChartNoteDate)
                                  .ToListAsync();

      var problemsUnique = await _utils.GetClientBehaviors(clientId);
      if (problemId != 0) problemsUnique = problemsUnique.Where(w => w.ProblemId == problemId).ToList();
      // var problemsUnique = mainData.Select(s => new { s.Behavior.ProblemId, s.Behavior.ProblemBehaviorDescription }).Distinct()
      //                              .Union(currentPeriod.PeriodClientProblems.Select(s => new { s.ProblemBehavior.ProblemId, s.ProblemBehavior.ProblemBehaviorDescription }).Distinct())
      //                              .OrderBy(o => o.ProblemBehaviorDescription).ToList();
      var dataSet = new List<MultiSerieChart>();
      var plotLines = new List<PlotLine>();

      var firstWeekStart = currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      // while (firstWeekStart.DayOfWeek != DayOfWeek.Sunday) firstWeekStart = firstWeekStart.AddDays(-1);
      var lastWeekEnd = DateTime.Today;
      while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday) lastWeekEnd = lastWeekEnd.AddDays(1);
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var legend = new List<string>();
      legend.Add("Base");
      legend.Add("");
      plotLines.Add(new PlotLine { Label = new Label { Text = "Baseline" }, Value = 0, Color = "Blue", DashStyle = "ShortDot" });
      plotLines.Add(new PlotLine { Label = new Label { Text = "Start" }, Value = 2, Color = "Green", DashStyle = "ShortDot" });

      foreach (var problem in problemsUnique)
      {
        var baseLine = await _dbContext.ClientProblems
                                       .Where(w => w.ProblemId == problem.ProblemId && w.ClientId == clientId)
                                       .Select(s => s.BaselineCount)
                                       .FirstOrDefaultAsync();

        var data = new List<int?>();

        data.Add(baseLine);
        data.Add(null);

        var calWeekStart = firstWeekStart;
        for (int i = 0; i < totalWeeks; i++)
        {
          var calWeekEnd = calWeekStart.AddDays(6);
          var problemsCount = mainData.Where(w => w.Entry.Date >= calWeekStart && w.Entry.Date <= calWeekEnd)
                                      .Count(w => w.ProblemId == problem.ProblemId);

          data.Add(problemsCount == 0 ? null : (int?)problemsCount);

          var notesWeek = notes.Where(w => w.ChartNoteDate >= calWeekStart && w.ChartNoteDate <= calWeekEnd).ToList();
          foreach (var n in notesWeek) plotLines.Add(new PlotLine { Label = new Label { Text = n.Title }, Value = i + 2 });

          calWeekStart = calWeekStart.AddDays(7);
        }

        dataSet.Add(new MultiSerieChart
        {
          Data = data,
          Name = problem.ProblemBehavior.ProblemBehaviorDescription
        });
      }

      var calWeekStartLegend = firstWeekStart;
      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = calWeekStartLegend.AddDays(6);
        legend.Add(calWeekEnd.ToString("M/d/yy"));
        calWeekStartLegend = calWeekStartLegend.AddDays(7);
      }

      return Ok(new
      {
        chartOptions = new
        {
          xAxis = new { categories = legend, plotLines, title = new { text = "Weeks (label is last day of week)" }, crosshair = true },
          series = dataSet,
          title = new { text = "" },
          chart = new { type = "spline" },
          tooltip = new { shared = true },
          yAxis = new { title = new { text = "Count" } },
          legend = new { enabled = problemId == 0 }
        },
        notes
      });
    }

    [HttpGet("[action]/{clientId}/{replacementId?}")]
    public async Task<IActionResult> GetReplacementProgramChart(int clientId, int replacementId = 0)
    {
      var currentPeriod = await _utils.GetClientWholePeriod(clientId);
      var mainData = await _dbContext.SessionCollectReplacements
                                     //.Where(w => w.Entry.Date >= currentPeriod.Start.Date && w.Entry.Date <= currentPeriod.End.Date)
                                     .Where(w => w.ClientId == clientId)
                                     .Where(w => replacementId == 0 || w.ReplacementId == replacementId)
                                     .Include(i => i.Replacement)
                                     .ToListAsync();
      var notes = await _dbContext.ClientChartNotes
                                  //.Where(w => w.ChartNoteDate >= currentPeriod.Start && w.ChartNoteDate <= currentPeriod.End)
                                  .Where(w => w.ClientId == clientId)
                                  .Where(w => w.ChartNoteType == ChartNoteType.Both || w.ChartNoteType == ChartNoteType.Replacement)
                                  .OrderBy(o => o.ChartNoteDate)
                                  .ToListAsync();

      var replacementUnique = await _utils.GetClientReplacements(clientId);
      if (replacementId != 0) replacementUnique = replacementUnique.Where(w => w.ReplacementId == replacementId).ToList();
      // var replacementUnique = mainData.Select(s => new { s.Replacement.ReplacementId, s.Replacement.ReplacementProgramDescription }).Distinct()
      //                                 .Union(currentPeriod.PeriodClientReplacements.Select(s => new { s.Replacement.ReplacementId, s.Replacement.ReplacementProgramDescription }).Distinct())
      //                                 .OrderBy(o => o.ReplacementProgramDescription).ToList();
      var dataSet = new List<MultiSerieChart>();
      var plotLines = new List<PlotLine>();

      var firstWeekStart = currentPeriod.start;
      firstWeekStart = firstWeekStart.StartOfWeek(DayOfWeek.Sunday);
      //while (firstWeekStart.DayOfWeek != DayOfWeek.Sunday) firstWeekStart = firstWeekStart.AddDays(-1);
      var lastWeekEnd = DateTime.Today;
      while (lastWeekEnd.DayOfWeek != DayOfWeek.Saturday) lastWeekEnd = lastWeekEnd.AddDays(1);
      var totalWeeks = ((lastWeekEnd - firstWeekStart).Days + 1) / 7;

      var legend = new List<string>();
      legend.Add("Base");
      legend.Add("");
      plotLines.Add(new PlotLine { Label = new Label { Text = "Baseline" }, Value = 0, Color = "Blue", DashStyle = "ShortDot" });
      plotLines.Add(new PlotLine { Label = new Label { Text = "Start" }, Value = 2, Color = "Green", DashStyle = "ShortDot" });


      foreach (var replacement in replacementUnique)
      {
        var baseLine = await _dbContext.ClientReplacements
                                       .Where(w => w.ReplacementId == replacement.ReplacementId && w.ClientId == clientId)
                                       .Select(s => s.BaselinePercent)
                                       .FirstOrDefaultAsync();

        var data = new List<int?>();

        data.Add(baseLine);
        data.Add(null);

        var calWeekStart = firstWeekStart;
        for (int i = 0; i < totalWeeks; i++)
        {
          var calWeekEnd = calWeekStart.AddDays(6);
          var replacementCount = mainData.Where(w => w.Entry.Date >= calWeekStart && w.Entry.Date <= calWeekEnd)
                                         .Where(w => w.ReplacementId == replacement.ReplacementId)
                                         .Count();
          var replacementComplete = mainData.Where(w => w.Entry.Date >= calWeekStart && w.Entry.Date <= calWeekEnd)
                                            .Where(w => w.ReplacementId == replacement.ReplacementId)
                                            .Count(w => w.Completed);
          var percent = replacementCount == 0 ? 0 : replacementComplete / (decimal)replacementCount * 100;

          data.Add(replacementCount == 0 ? null : (int?)percent);

          var notesWeek = notes.Where(w => w.ChartNoteDate >= calWeekStart && w.ChartNoteDate <= calWeekEnd).ToList();
          foreach (var n in notesWeek) plotLines.Add(new PlotLine { Label = new Label { Text = n.Title }, Value = i + 2 });

          calWeekStart = calWeekStart.AddDays(7);
        }

        dataSet.Add(new MultiSerieChart
        {
          Data = data,
          Name = replacement.Replacement.ReplacementProgramDescription
        });
      }

      var calWeekStartLegend = firstWeekStart;
      for (int i = 0; i < totalWeeks; i++)
      {
        var calWeekEnd = calWeekStartLegend.AddDays(6);
        legend.Add(calWeekEnd.ToString("M/d/yy"));
        calWeekStartLegend = calWeekStartLegend.AddDays(7);
      }

      return Ok(new
      {
        chartOptions = new
        {
          xAxis = new { categories = legend, plotLines = plotLines, title = new { text = "Weeks (label is last day of week)" }, crosshair = true },
          series = dataSet,
          title = new { text = "" },
          chart = new { type = "spline" },
          tooltip = new { shared = true },
          yAxis = new { title = new { text = "Trials percent" } },
          legend = new { enabled = replacementId == 0 }
        },
        notes
      });
    }

    [HttpPost("add-edit-chart-note")]
    public async Task<IActionResult> AddEditChartNote([FromBody] ClientChartNote note)
    {
      try
      {
        if (note.ClientChartNoteId == 0) await _dbContext.ClientChartNotes.AddAsync(note);
        else
        {
          _dbContext.ClientChartNotes.Attach(note);
          _dbContext.Entry(note).State = EntityState.Modified;
        }
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-chart-note/{id}")]
    public async Task<IActionResult> GetChartNote(int id)
    {
      try
      {
        var note = await _dbContext.ClientChartNotes
                  .Select(s => new
                  {
                    s.ClientChartNoteId,
                    s.ClientId,
                    s.ChartNoteType,
                    s.Title,
                    s.Note,
                    ChartNoteDate = s.ChartNoteDate.ToString("MM/dd/yyyy")
                  })
                  .FirstOrDefaultAsync(s => s.ClientChartNoteId.Equals(id));
        if (note == null) throw new Exception("Note not found");
        return Ok(note);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("delete-chart-note/{id}")]
    public async Task<IActionResult> DeleteChartNote(int id)
    {
      try
      {
        var note = await _dbContext.ClientChartNotes
                  .FirstOrDefaultAsync(s => s.ClientChartNoteId.Equals(id));
        if (note == null) throw new Exception("Note not found");
        _dbContext.ClientChartNotes.Remove(note);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("delete-session/{id}/{check?}")]
    public async Task<IActionResult> DeleteSession(int id)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.SessionId.Equals(id));
        if (session == null) throw new Exception("Session not found");
        _dbContext.SessionLogs.RemoveRange(_dbContext.SessionLogs.Where(s => s.SessionId == id).ToList());
        _dbContext.Sessions.Remove(session);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-competency-check/{id}")]
    public async Task<IActionResult> GetCompetencyCheck(int id)
    {
      try
      {
        var comp = await _dbContext.CompetencyChecks
                  .Include(i => i.EvaluationBy)
                  .Include(i => i.CompetencyCheckClientParams).ThenInclude(i => i.CompetencyCheckParam)
                  .FirstOrDefaultAsync(s => s.CompetencyCheckId.Equals(id));
        if (comp == null)
        {
          var compParams = await _dbContext.CompetencyCheckParams.OrderBy(o => o.CompetencyCheckType).ThenBy(o => o.CompetencyCheckParamId).ToListAsync();
          var compClientParams = new List<CompetencyCheckClientParam>();
          foreach (var compParam in compParams)
          {
            var entry = new CompetencyCheckClientParam
            {
              CompetencyCheckParamId = compParam.CompetencyCheckParamId,
              CompetencyCheckParam = await _dbContext.CompetencyCheckParams.FirstAsync(w => w.CompetencyCheckParamId.Equals(compParam.CompetencyCheckParamId))
            };
            compClientParams.Add(entry);
          }
          comp = new CompetencyCheck();
          comp.Date = DateTime.Today;
          comp.CompetencyCheckClientParams = compClientParams;
        }
        return Ok(comp);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-edit-competency-check")]
    public async Task<IActionResult> AddEditCompetencyCheck([FromBody] CompetencyCheck comp)
    {
      try
      {
        if (comp.CompetencyCheckType == CompetencyCheckType.Rbt && comp.UserId == null) throw new Exception("You must select a valid RBT");
        if (comp.CompetencyCheckType == CompetencyCheckType.Caregiver && comp.CaregiverId == null) throw new Exception("You must select a valid Caregiver");
        var scoreList = comp.CompetencyCheckClientParams.Where(w => w.CompetencyCheckParam.CompetencyCheckType == comp.CompetencyCheckType).ToList();
        var scoreSum = scoreList.Sum(s => s.Score);
        var score = scoreList.Count() == 0 ? 0 : scoreSum / (decimal)scoreList.Count();
        comp.TotalScore = score;
        if (comp.CompetencyCheckId == 0)
        {
          using (var transaction = await _dbContext.Database.BeginTransactionAsync())
          {
            comp.EvaluationById = (await _utils.GetCurrentUser()).UserId;
            var competencyParams = comp.CompetencyCheckClientParams;
            comp.CompetencyCheckClientParams = null;
            await _dbContext.CompetencyChecks.AddAsync(comp);
            await _dbContext.SaveChangesAsync();
            foreach (var item in competencyParams)
            {
              item.CompetencyCheckId = comp.CompetencyCheckId;
              item.CompetencyCheckParam = null;
              await _dbContext.CompetencyCheckClientParams.AddAsync(item);
              await _dbContext.SaveChangesAsync();
            }
            transaction.Commit();
          }
        }
        else
        {
          _dbContext.CompetencyChecks.Attach(comp);
          _dbContext.Entry(comp).State = EntityState.Modified;

          foreach (var item in comp.CompetencyCheckClientParams)
          {
            _dbContext.CompetencyCheckClientParams.Attach(item);
            _dbContext.Entry(item).State = EntityState.Modified;
          }
        }
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-competency-checks/{clientId}")]
    public async Task<IActionResult> GetCompetencyChecks(int clientId)
    {
      try
      {
        var comp = await _dbContext.CompetencyChecks
                  .Where(w => w.ClientId.Equals(clientId))
                  .Select(s => new
                  {
                    s.CompetencyCheckId,
                    CompetencyCheckType = s.CompetencyCheckType.ToString(),
                    Subject = s.CompetencyCheckType == CompetencyCheckType.Rbt ? $"{s.User.Firstname} {s.User.Lastname}" : s.Caregiver.CaregiverFullname,
                    s.TotalScore,
                    s.TotalDuration,
                    s.Caregiver,
                    s.User,
                    s.Date
                  }).ToListAsync();
        return Ok(comp);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("delete-competency-check/{id}")]
    public async Task<IActionResult> DeleteCompetencyCheck(int id)
    {
      try
      {
        var comp = await _dbContext.CompetencyChecks
                  .FirstOrDefaultAsync(s => s.CompetencyCheckId.Equals(id));
        if (comp == null) throw new Exception("Competency check not found");
        _dbContext.CompetencyChecks.Remove(comp);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("export-competency-check/{id}")]
    public async Task<IActionResult> ExportCompetencyCheck([FromServices] ICompetencyCheckReport competencyReport, int id)
    {
      try
      {
        byte[] fileContents;
        var report = await competencyReport.CreateReport(id);
        fileContents = report.GetAsByteArray();
        if (fileContents == null || fileContents.Length == 0) return NotFound();
        return File(fileContents: fileContents, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName: "test.xlsx");
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> ChangeSessionStatus([FromBody] ChangeSessionStatus changeSessionStatus)
    {
      try
      {
        var newStatus = await _utils.ChangeSessionStatus(changeSessionStatus);
        await _utils.NewEntryLog(changeSessionStatus.SessionId, "Status", $"Status changed to: {changeSessionStatus.SessionStatus.ToString()}");
        return Ok(newStatus);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveSessionSign([FromBody] SessionSign sign)
    {
      try
      {
        await _dbContext.AddAsync(sign);
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(sign.SessionId, "Signed", "Status electronically signed", "fa-signature");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SendUrlSign([FromBody] UrlClass url)
    {
      try
      {
        var user = await _utils.GetCurrentUser();
        var to = user.Email;
        var subject = $"Link to caregiver sign session";
        var body = $@"<h3>Caregiver sign session:</h3><br>
                      Please tap on following link to sign the session<br> 
                      {url.Url}";
        await _utils.CreateEmail(to, subject, body, MessageType.General);
        await _utils.SendEmailsAsync(false);
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveSessionCollectBehavior([FromBody] SessionCollectBehavior s)
    {
      try
      {
        await _dbContext.AddAsync(s);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> GetCollectBehaviors(int sessionId)
    {
      try
      {
        var dataDetails = await _dbContext.SessionCollectBehaviors
                          .Where(w => w.SessionId == sessionId)
                          .Include(i => i.Behavior)
                          .OrderBy(o => o.Entry)
                          .ToListAsync();

        var dataSummary = dataDetails.GroupBy(g => new { g.Behavior, g.ProblemId })
                          .Select(s => new
                          {
                            s.Key.ProblemId,
                            s.Key.Behavior.ProblemBehaviorDescription,
                            Count = s.Count()
                          })
                          .OrderBy(o => o.ProblemBehaviorDescription)
                          .ToList();
        return Ok(new
        {
          dataDetails,
          dataSummary
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteSessionCollectBehavior(int id)
    {
      try
      {
        var data = await _dbContext.SessionCollectBehaviors
                  .FirstOrDefaultAsync(s => s.SessionCollectBehaviorId.Equals(id));
        if (data == null) throw new Exception("Collect data not found");
        _dbContext.SessionCollectBehaviors.Remove(data);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveSessionCollectReplacement([FromBody] SessionCollectReplacement s)
    {
      try
      {
        await _dbContext.AddAsync(s);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> GetCollectReplacements(int sessionId)
    {
      try
      {
        var dataDetails = await _dbContext.SessionCollectReplacements
                          .Where(w => w.SessionId == sessionId)
                          .Include(i => i.Replacement)
                          .OrderBy(o => o.Entry)
                          .ToListAsync();

        var dataSummary = dataDetails.GroupBy(g => new { g.Replacement, g.ReplacementId })
                          .Select(s => new
                          {
                            s.Key.ReplacementId,
                            s.Key.Replacement.ReplacementProgramDescription,
                            Count = s.Count(),
                            Completed = s.Count(c => c.Completed)
                          })
                          .OrderBy(o => o.ReplacementProgramDescription)
                          .ToList();
        return Ok(new
        {
          dataDetails,
          dataSummary
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteSessionCollectReplacement(int id)
    {
      try
      {
        var data = await _dbContext.SessionCollectReplacements
                  .FirstOrDefaultAsync(s => s.SessionCollectReplacementId.Equals(id));
        if (data == null) throw new Exception("Collect data not found");
        _dbContext.SessionCollectReplacements.Remove(data);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [AllowAnonymous]
    [HttpGet("force-send-emails")]
    public async Task<IActionResult> TestingHangFire()
    {
      await _utils.MidNightProcess();
      //await _utils.SendEmailsAsync();
      return Ok();
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RejectSession([FromBody] RejectSession reject)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == reject.SessionId);
        var sessionNote = await _dbContext.SessionNotes.FirstOrDefaultAsync(w => w.SessionId == reject.SessionId);
        if (sessionNote == null || session == null) throw new Exception("Session and/or Note not found");

        session.SessionStatus = SessionStatus.Rejected;
        sessionNote.RejectNotes = reject.RejectMessage;
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(session.SessionId, "Rejected", $"Session rejected: {reject.RejectMessage}", "fa-exclamation-circle", "red");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetSessionsCalendar2(int clientId)
    {
      try
      {
        var sessions = await _dbContext.Sessions
                                       .Where(w => w.ClientId.Equals(clientId))
                                       .Select(s => new
                                       {
                                         s.SessionId,
                                         title = $"{s.User.Firstname} {s.User.Lastname}",
                                         date = s.SessionStart.ToString("yyyy-MM-dd"),
                                         SessionStart = s.SessionStart.ToString("u"),
                                         SessionEnd = s.SessionEnd.ToString("u"),
                                         time = s.SessionStart.ToString("HH:mm"),
                                         timeStartFormat = s.SessionStart.ToString("hh:mm tt"),
                                         timeEndFormat = s.SessionEnd.ToString("hh:mm tt"),
                                         timeMinutes = s.SessionStart.ToString("mm"),
                                         duration = (s.SessionEnd - s.SessionStart).TotalMinutes,
                                         className = $"white--text v-card--hover  pa-0 ma-0",
                                         SessionType = Enum.GetName(typeof(SessionType), s.SessionType).ToLower(),
                                         SessionTypeFormated = s.SessionType.ToString().Replace('_', ' '),
                                         s.TotalUnits,
                                         Pos = s.Pos.ToString().Replace("_", " "),//Enum.GetName(typeof(Pos), s.Pos),
                                         SessionStatus = s.SessionStatus.ToString(),
                                         SessionStatusCode = s.SessionStatus,
                                         SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString(),
                                         s.User.Rol.RolShortName,
                                         s.User
                                       })
                                       .ToListAsync();
        return Ok(sessions);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetClientBehaviors(int clientId)
    {
      try
      {
        var behaviors = await _utils.GetClientBehaviors(clientId);
        return Ok(behaviors);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetClientReplacements(int clientId)
    {
      try
      {
        var replacements = await _utils.GetClientReplacements(clientId);
        return Ok(replacements);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> EditSessionTime([FromBody] EditSessionTime s)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == s.SessionId);
        if (session == null) throw new Exception("Session not found");

        // var newDateStar = DateTime.Parse($"{session.SessionStart.Date.ToShortDateString()} {s.Start}").ToUniversalTime();
        // var newDateEnd = DateTime.Parse($"{session.SessionEnd.Date.ToShortDateString()} {s.End}").ToUniversalTime();

        session.SessionStart = s.Start.ToUniversalTime();
        session.SessionEnd = s.End.ToUniversalTime();
        var diff = session.SessionEnd - session.SessionStart;
        session.TotalUnits = Convert.ToInt32(diff.TotalMinutes / 15);
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(session.SessionId, "Time", "Session time edited", "fa-clock", "orange");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> EditSessionPos([FromBody] ClassIdInt s)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == s.Id);
        if (session == null) throw new Exception("Session not found");
        session.Pos = (Pos)s.Value;
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(session.SessionId, "Pos", $"Session POS edited to {((Pos)s.Value).ToString()}", "fa-clock", "teal");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetCompetencyCheckCaregiversCharts(int clientId)
    {
      var competencyChecks = await _dbContext.CompetencyChecks.Where(w => w.ClientId == clientId).Include(i => i.Caregiver).Include(i => i.User).ToListAsync();
      var caregivers = competencyChecks.Where(w => w.Caregiver != null).Select(s => s.Caregiver).Distinct().ToList();
      var rbts = competencyChecks.Where(w => w.User != null).Select(s => s.User).Distinct().ToList();

      var charts = new List<Object>();

      foreach (var caregiver in caregivers)
      {
        var c = await _utils.GetCompetencyCheckChart(clientId, CompetencyCheckType.Caregiver, caregiver.CaregiverId);
        charts.Add(c);
      }

      foreach (var rbt in rbts)
      {
        var c = await _utils.GetCompetencyCheckChart(clientId, CompetencyCheckType.Rbt, rbt.UserId);
        charts.Add(c);
      }

      return Ok(charts);
    }

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> GetSessionPrintExtraInfo(int sessionId)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == sessionId);
        var lead = await _dbContext.Assignments
                                    .Where(w => w.ClientId == session.ClientId)
                                    .Where(w => w.User.RolId == 2 && w.Active)
                                    .Select(s => s.User).Include(i => i.UserSign)
                                    .FirstOrDefaultAsync();
        return Ok(new
        {
          lead
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> EditSessionDriveTime([FromBody] ClassIdDecimal s)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == s.Id);
        if (session == null) throw new Exception("Session not found");
        session.DriveTime = s.Value;
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(session.SessionId, "Drive Time", $"Session Drive time edited to {s.Value} hours", "fa-car", "teal");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }
  }
}