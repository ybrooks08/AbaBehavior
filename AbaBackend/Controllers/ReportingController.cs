using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Extensions;
using AbaBackend.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AbaBackend.Infrastructure.Utils.Static;

namespace AbaBackend.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/reporting")]
  public class ReportingController : Controller
  {
    private AbaDbContext _dbContext;
    private IUtils _utils;
    private IConfiguration _configuration;
    private IHostingEnvironment _env;

    public ReportingController(AbaDbContext context, IUtils utils, IConfiguration configuration, IHostingEnvironment env)
    {
      _dbContext = context;
      _utils = utils;
      _configuration = configuration;
      _env = env;
    }

    [HttpGet("[action]/{from}/{to}/{clientId}/{behaviorAnalysisCodeId}")]
    public async Task<IActionResult> GetBillingGuide(string from, string to, int clientId, int behaviorAnalysisCodeId)
    {
      try
      {
        var fromDate = Convert.ToDateTime(from).Date;
        var toDate = Convert.ToDateTime(to).Date;

        var client = await _dbContext.Clients
          .AsNoTracking()
          .Where(w => w.ClientId.Equals(clientId))
          .Select(s => new
          {
            MemberId = s.MemberNo,
            Firstname = s.Firstname,
            Lastname = s.Lastname,
            s.Dob,
            s.Code,
            Diagnosis = s.ClientDiagnostics.Select(sd => new
            {
              sd.Diagnosis.Code,
              sd.Diagnosis.Description
            }),
            Assessments = s.Assessments.Where(w => (w.BehaviorAnalysisCodeId.Equals(behaviorAnalysisCodeId) || w.BehaviorAnalysisCodeId == 1 || w.BehaviorAnalysisCodeId == 2) && (w.StartDate <= toDate && fromDate <= w.EndDate))
              .Select(a => new
              {
                a.AssessmentId,
                a.BehaviorAnalysisCode,
                a.PaNumber,
                a.TotalUnits,
                a.StartDate,
                a.EndDate
              }),
            Referrals = s.Referrals.Where(w => (w.DateReferral <= toDate && fromDate <= w.DateExpires) && w.Active),
            Assignments = s.Assignments.Where(w => w.User.Rol.BehaviorAnalysisCodeId.Equals(behaviorAnalysisCodeId) && w.Active).Select(su => su.User),
          })
          .FirstOrDefaultAsync();

        var sessions = await _dbContext.Sessions
          .AsNoTracking()
          .Where(w => w.ClientId.Equals(clientId))
          .Where(w => w.SessionStart.Date >= fromDate && w.SessionStart.Date <= toDate)
          .Where(w => w.BehaviorAnalysisCodeId.Equals(behaviorAnalysisCodeId))
          .Where(w => w.SessionStatus == SessionStatus.Reviewed || w.SessionStatus == SessionStatus.Billed)
          .Select(s => new
          {
            s.SessionId,
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            s.TotalUnits,
            s.Pos,
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            SessionStatus = s.SessionStatus.ToString(),
            SessionType = s.SessionType.ToString().Replace("_", " "),
          })
          .OrderBy(w => w.SessionStart)
          .ToListAsync();
        return Ok(new
        {
          client,
          sessions
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{from}/{to}/{clientId}/{userId?}")]
    public async Task<IActionResult> GetServiceLog(string from, string to, int clientId, int userId = -1)
    {
      try
      {
        var fromDate = Convert.ToDateTime(from).Date;
        var toDate = Convert.ToDateTime(to).Date;

        var user = userId == -1 ? await _utils.GetCurrentUser() : await _utils.GetUserById(userId);

        var client = await _dbContext.Clients
          .AsNoTracking()
          .Where(w => w.ClientId.Equals(clientId))
          .Select(s => new
          {
            MemberId = s.MemberNo,
            Firstname = s.Firstname,
            Lastname = s.Lastname,
            s.Dob,
            s.Code,
            Assessments = s.Assessments.Where(w => w.BehaviorAnalysisCodeId == user.Rol.BehaviorAnalysisCodeId && (w.StartDate <= toDate && fromDate <= w.EndDate)),
            Referrals = s.Referrals.Where(w => (w.DateReferral <= toDate && fromDate <= w.DateExpires) && w.Active)
          }).FirstOrDefaultAsync();

        var sessions = await _dbContext.Sessions
          .AsNoTracking()
          .Where(w => w.SessionStatus == SessionStatus.Checked || w.SessionStatus == SessionStatus.Reviewed || w.SessionStatus == SessionStatus.Billed)
          .Where(w => w.ClientId == clientId && w.UserId == user.UserId)
          .Where(w => w.SessionStart.Date >= fromDate && w.SessionStart.Date <= toDate)
          .OrderBy(w => w.SessionStart)
          .Select(s => new
          {
            s.SessionId,
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            s.TotalUnits,
            SessionType = s.SessionType.ToString().Replace("_", " "),
            SessionTypeCode = s.SessionType,
            SessionStatus = s.SessionStatus.ToString(),
            SessionStatusCode = s.SessionStatus,
            SessionStatusColor = ((SessionStatusColors) s.SessionStatus).ToString(),
            Pos = s.Pos.ToString(),
            PosCode = s.Pos,
            s.SessionNote.Caregiver.CaregiverFullname,
            CaregiverFullnameSupervision = s.SessionSupervisionNote.Caregiver.CaregiverFullname,
            s.Sign
          })
          .ToListAsync();
        return Ok(new
        {
          user,
          client,
          sessions
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{from}/{to}/{clientId}")]
    public async Task<IActionResult> GetSessionsHistory(string from, string to, int clientId)
    {
      try
      {
        var fromDate = Convert.ToDateTime(from).Date;
        var toDate = Convert.ToDateTime(to).Date;

        var user = await _utils.GetCurrentUser();

        var sessions = await _dbContext.Sessions
          .AsNoTracking()
          .Where(w => w.ClientId == clientId)
          .Where(w => w.SessionStart.Date >= fromDate && w.SessionStart.Date <= toDate)
          .OrderBy(w => w.SessionStart)
          .Select(s => new
          {
            s.SessionId,
            s.UserId,
            s.ClientId,
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            s.TotalUnits,
            SessionType = s.SessionType.ToString().Replace("_", " "),
            SessionStatus = s.SessionStatus.ToString(),
            SessionStatusCode = s.SessionStatus,
            SessionStatusColor = ((SessionStatusColors) s.SessionStatus).ToString(),
            Pos = s.Pos.ToString().Replace("_", " "),
            PosCode = s.Pos,
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            Rol = s.User.Rol.RolShortName
          })
          .ToListAsync();

        if (user.Rol.RolShortName == "tech") sessions = sessions.Where(s => s.UserId == user.UserId).ToList();
        if (user.Rol.RolShortName == "assistant")
        {
          var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId).Select(s => s.ClientId).ToListAsync();
          sessions = sessions.Where(s => myClients.Contains(s.ClientId) && s.Rol != "analyst").ToList();
        }

        if (user.Rol.RolShortName == "analyst")
        {
          var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId && w.Active).Select(s => s.ClientId).ToListAsync();
          sessions = sessions.Where(s => myClients.Contains(s.ClientId)).ToList();
        }

        return Ok(sessions);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{from}/{to}/{userId}")]
    public async Task<IActionResult> GetSessionsByUser(string from, string to, int userId)
    {
      try
      {
        var fromDate = Convert.ToDateTime(from).Date;
        var toDate = Convert.ToDateTime(to).Date;

        var sessions = await _dbContext.Sessions
          .AsNoTracking()
          .Where(w => w.UserId == userId)
          .Where(w => w.SessionStart.Date >= fromDate && w.SessionStart.Date <= toDate)
          .Where(w => w.SessionStatus == SessionStatus.Billed)
          .OrderBy(w => w.SessionStart)
          .Select(s => new
          {
            s.SessionId,
            s.UserId,
            s.ClientId,
            ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
            s.Client.Code,
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            s.TotalUnits,
            SessionType = s.SessionType.ToString().Replace("_", " "),
            SessionStatus = s.SessionStatus.ToString(),
            SessionStatusCode = s.SessionStatus,
            SessionStatusColor = ((SessionStatusColors) s.SessionStatus).ToString(),
            Pos = s.Pos.ToString().Replace("_", " "),
            PosCode = s.Pos,
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            Rol = s.User.Rol.RolShortName
          })
          .OrderBy(o => o.SessionStart)
          .ToListAsync();

        return Ok(sessions);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{start}/{end}/{userId}")]
    public async Task<IActionResult> GetTimeSheet(DateTime start, DateTime end, int userId)
    {
      try
      {
        var user = await _utils.GetUserById(userId);
        if (!user.Rol.CanCreateSession) throw new Exception("You must select a valid user");

        var sessions = await _dbContext.Sessions
          .Include(i => i.Client)
          .Where(w => w.SessionStart.Date >= start && w.SessionStart.Date <= end)
          .Where(w => w.SessionStatus == SessionStatus.Reviewed || w.SessionStatus == SessionStatus.Billed)
          .Where(w => w.UserId.Equals(userId))
          .OrderBy(o => o.SessionStart)
          .ToListAsync();

        var rows = new List<Object>();
        var totalHours = 0m;
        foreach (var session in sessions)
        {
          var sessionHours = session.TotalUnits / (decimal) 4;
          var sessionDriveTime = session.DriveTime;

          var regularHours = 0m;
          var regularDrive = 0m;
          var extraHours = 0m;
          var extraDrive = 0m;

          var serviceHours = StaticUtils.CalculateWageHour(totalHours, sessionHours);
          totalHours += sessionHours;
          regularHours = serviceHours.regular;
          extraHours = serviceHours.extra;

          var driveHours = StaticUtils.CalculateWageHour(totalHours, sessionDriveTime);
          totalHours += sessionDriveTime;
          regularDrive = driveHours.regular;
          extraDrive = driveHours.extra;

          rows.Add(new OkObjectResult(
            new
            {
              sessionId = session.SessionId,
              date = session.SessionStart.ToString("u"),
              sessionIn = session.SessionStart.ToString("u"),
              sessionOut = session.SessionEnd.ToString("u"),
              client = $"{session.Client.Firstname} {session.Client.Lastname[0]}.",
              sessionHours,
              sessionDriveTime,
              regularHours,
              regularDrive,
              extraHours,
              extraDrive
            }).Value);
        }

        return Ok(new
        {
          rows,
          user
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{yearMonth}/{clientId}")]
    public async Task<IActionResult> GetMonthWeekData(DateTime yearMonth, int clientId)
    {
      try
      {
        var endMonth = yearMonth.AddMonths(1).AddDays(-1);
        var start = yearMonth.GetPrevDay(DayOfWeek.Sunday);
        var end = endMonth.GetPrevDay(DayOfWeek.Saturday);

        var totalWeeks = ((end - start).Days + 1) / 7;

        var problems = await _utils.GetClientBehaviors(clientId);
        var rowsProblems = new List<List<Object>>();

        var mainDataBehaviorClient = await _dbContext.SessionCollectBehaviors
          .Where(w => w.ClientId == clientId)
          .ToListAsync();

        var mainDataReplacementClient = await _dbContext.SessionCollectReplacements
          .Where(w => w.ClientId == clientId)
          .ToListAsync();

        foreach (var problem in problems)
        {
          var newRow = new List<Object>
          {
            problem.ProblemBehavior.ProblemBehaviorDescription
          };

          var monthStart = start;
          var monthEnd = end;
          decimal sum = 0;
          while (monthStart < end)
          {
            var weekStart = monthStart;
            var weekEnd = monthStart.AddDays(6);

            var mainData = mainDataBehaviorClient
              .Where(w => w.ProblemId == problem.ProblemId)
              .Where(w => w.Entry.Date >= weekStart && w.Entry.Date <= weekEnd)
              .Count();
            var mainDataCompleted = mainDataBehaviorClient
              .Where(w => w.ProblemId == problem.ProblemId)
              .Where(w => w.Entry.Date >= weekStart && w.Entry.Date <= weekEnd)
              .Where(w => w.Completed)
              .Count();
            var mainDataCaregiverCollect = await _dbContext.CaregiverDataCollections
              .Where(w => w.ClientId == clientId && w.CollectDate.Date >= weekStart && w.CollectDate.Date <= weekEnd)
              .Select(s => s.CaregiverDataCollectionProblems.Where(w => w.ProblemId == problem.ProblemId).Sum(q => q.Count))
              .FirstOrDefaultAsync();

            if (!problem.ProblemBehavior.IsPercent)
            {
              var totalWeek = mainData + (mainDataCaregiverCollect ?? 0);
              sum += totalWeek;
              newRow.Add(totalWeek);
            }
            else
            {
              var percent = mainData == 0 ? 0 : mainDataCompleted / (decimal) mainData;
              sum += percent * 100;
              newRow.Add($"{percent:p0}");
            }

            monthStart = monthStart.AddDays(7);
          }

          newRow.Add(!problem.ProblemBehavior.IsPercent ? sum.ToString() : "-");
          newRow.Add((sum / (decimal) totalWeeks).ToString("n0") + (problem.ProblemBehavior.IsPercent ? "%" : ""));
          rowsProblems.Add(newRow);
        }

        var hStart = start;
        var headers = new List<string> {"Problem"};
        while (hStart < end)
        {
          var weekStart = hStart;
          var weekEnd = hStart.AddDays(6);
          headers.Add($"{weekStart.ToShortDateString()}<br>{weekEnd.ToShortDateString()}");
          hStart = hStart.AddDays(7);
        }

        headers.Add("Total");
        headers.Add("Average");

        var replacements = await _utils.GetClientReplacements(clientId);
        var rowsReplacements = new List<List<Object>>();

        foreach (var replacement in replacements)
        {
          var newRow = new List<Object>
          {
            replacement.Replacement.ReplacementProgramDescription
          };

          var monthStart = start;
          var monthEnd = end;
          decimal sumTotal = 0;
          while (monthStart < end)
          {
            var weekStart = monthStart;
            var weekEnd = monthStart.AddDays(6);

            var mainDataCount = mainDataReplacementClient
              .Where(w => w.ReplacementId == replacement.ReplacementId)
              .Where(w => w.Entry.Date >= weekStart && w.Entry.Date <= weekEnd)
              .Count();

            var mainDataCompleted = mainDataReplacementClient
              .Where(w => w.ReplacementId == replacement.ReplacementId)
              .Where(w => w.Entry.Date >= weekStart && w.Entry.Date <= weekEnd)
              .Where(w => w.Completed)
              .Count();

            var caregiverReplacements = await _dbContext.CaregiverDataCollections
              .Where(w => w.ClientId == clientId && w.CollectDate.Date >= weekStart && w.CollectDate.Date <= weekEnd)
              .GroupBy(g => 0)
              .Select(s => new
              {
                TotalTrial = s.Sum(d => d.CaregiverDataCollectionReplacements.Where(w => w.ReplacementId == replacement.ReplacementId).Sum(s1 => s1.TotalTrial)),
                TotalCompleted = s.Sum(d => d.CaregiverDataCollectionReplacements.Where(w => w.ReplacementId == replacement.ReplacementId).Sum(s1 => s1.TotalCompleted))
              }).FirstOrDefaultAsync();

            mainDataCount += caregiverReplacements?.TotalTrial ?? 0;
            mainDataCompleted += caregiverReplacements?.TotalCompleted ?? 0;
            var percent = mainDataCount == 0 ? 0 : mainDataCompleted / (decimal) mainDataCount * 100;
            sumTotal += percent;
            newRow.Add($"{percent:n0}%");
            monthStart = monthStart.AddDays(7);
          }

          var replacementAve = sumTotal / totalWeeks;
          newRow.Add($"{replacementAve:n0}%");
          rowsReplacements.Add(newRow);
        }

        var headersReplacement = new List<string> {"Replacement"};
        while (start < end)
        {
          var weekStart = start;
          var weekEnd = start.AddDays(6);
          headersReplacement.Add($"{weekStart.ToShortDateString()}<br>{weekEnd.ToShortDateString()}");
          start = start.AddDays(7);
        }

        headersReplacement.Add("Average");

        return Ok(new
        {
          rowsProblems,
          rowsReplacements,
          headers,
          headersReplacement
        });
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetSessionsReady2Bill()
    {
      try
      {
        var sessions = await _dbContext.Sessions
          .AsNoTracking()
          .Where(w => w.SessionStatus == SessionStatus.Reviewed)
          .OrderByDescending(w => w.SessionStart)
          .Select(s => new
          {
            s.SessionId,
            s.UserId,
            s.ClientId,
            s.Client,
            ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            s.TotalUnits,
            SessionType = s.SessionType.ToString().Replace("_", " "),
            SessionStatus = s.SessionStatus.ToString(),
            SessionStatusCode = s.SessionStatus,
            SessionStatusColor = ((SessionStatusColors) s.SessionStatus).ToString(),
            Pos = s.Pos.ToString().Replace("_", " "),
            PosCode = s.Pos,
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            Rol = s.User.Rol.RolShortName,
            BehaviorAnalisisCode = s.User.Rol.BehaviorAnalysisCode
          })
          .ToListAsync();
        return Ok(sessions);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}