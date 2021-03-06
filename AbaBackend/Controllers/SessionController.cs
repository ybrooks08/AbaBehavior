using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Collection;
using AbaBackend.Infrastructure.Extensions;
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
using AbaBackend.Infrastructure.Utils.Static;
using AbaBackend.Infrastructure.StoProcess;

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
    readonly AbaDbContext _dbContext;
    readonly IUtils _utils;
    readonly IConfiguration _configuration;
    readonly IHostingEnvironment _env;
    readonly ICollection _collection;
    private readonly IStoProcess _stoProcess;

    public SessionController(AbaDbContext context, IUtils utils, IConfiguration configuration, IHostingEnvironment env, ICollection collection, IStoProcess stoProcess)
    {
      _dbContext = context;
      _utils = utils;
      _configuration = configuration;
      _env = env;
      _collection = collection;
      _stoProcess = stoProcess;
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

          //check if user is allowed to create session on day of week
          if (!_utils.CheckIfuserAllowedDayOfWeek(user.SessionsDateAllowed, session.SessionStart.Date)) throw new Exception("You aren't allowed to create session on selected day");

          //check if have expiring documents
          var documentsExpired = await _utils.GetUserExpiredDocumentsCount();
          if (documentsExpired > 0) throw new Exception($"You cannot create any session because you have {documentsExpired} expired document(s).");

          // check if user is assigned to the client
          if (!await _utils.CheckAssignmentClientActive(user.UserId, session.ClientId)) throw new Exception("Selected client is not assigned or not actived in your assigments.");

          //check if client has a valid assessment 
          if (!(await _utils.GetClientValidAssessmentsForUser(session.SessionStart, session.ClientId, user)).Any()) throw new Exception($"Client has no {user.Rol.BehaviorAnalysisCode.Hcpcs} assessment or date is invalid.");

          //check if user has units available
          var unitsAvailable = await _utils.GetUnitsAvailable(session.SessionStart, session.ClientId, user);
          if (unitsAvailable <= 0) throw new Exception("No units available");
          if (unitsAvailable < session.TotalUnits) throw new Exception("There are not enough units for this session.");

          //check if client has BCABA service
          if (session.SessionType == SessionType.Training_BCABA)
          {
            var bcaba = await _dbContext.Clients
              .Where(w => w.ClientId.Equals(session.ClientId))
              .Where(w => w.Assignments.Count(w1 => w1.User.Rol.BehaviorAnalysisCode.Hcpcs == "H2012") > 0)
              .ToListAsync();
            if (bcaba.Count == 0) throw new Exception("Client haven't any BCBAB service/user.");
          }

          // session.SessionStart = session.SessionStart.ToUniversalTime();
          // session.SessionEnd = session.SessionEnd.ToUniversalTime();
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

          //check if user can create after x hours and have any pass
          if (!_utils.CanCreateAfterHours(user, session.SessionStart)) throw new Exception("You can not create session bacause exced the hours allowed and you dont have any pass.");

          //check if not pass week
          var (Allowed, Total) = await _utils.GetUnitsInWeek(session.SessionStart.Date, user.UserId, session.ClientId);
          if (Allowed != null && Total + session.TotalUnits > Allowed) throw new Exception("You can not create session bacause exced the units per week.");

          //check if there is a time gap between sessions
          if (!await _utils.CheckIfTimeGap(session.SessionStart.ToUniversalTime(), session.SessionEnd.ToUniversalTime(), user.UserId, session.ClientId)) throw new Exception("You can not create a session too close the end of other. Please leave a time between sessions.");

          //get current client analist
          int? analyst = client.Assignments.Where(w => w.Active && w.User.RolId == 2).FirstOrDefault()?.UserId ?? null;

          //check if session is not more than 2 days from today.
          var sessionDate = session.SessionStart.Date;
          var daysSpan = (sessionDate - DateTime.Today).Days;
          if (daysSpan > 2) throw new Exception("You can not create this session 2 days from today.");

          session.SessionStart = session.SessionStart.ToUniversalTime();
          session.SessionEnd = session.SessionEnd.ToUniversalTime();

          var diff = session.SessionEnd - session.SessionStart;
          var units = diff.TotalMinutes / 15;
          var sum1Unit = (units % 1) > 0.5 ? 1 : 0;
          var unitsTruncate = Decimal.Truncate((decimal)units) + sum1Unit;
          session.TotalUnits = (int)unitsTruncate;// Convert.ToInt32(diff.TotalMinutes / 15);

          // session.TotalUnits = Convert.ToInt32((session.SessionEnd - session.SessionStart).TotalMinutes / 15);
          session.UserId = user.UserId;
          session.BehaviorAnalysisCodeId = Convert.ToInt32(user.Rol.BehaviorAnalysisCodeId);
          session.SessionAnalystId = analyst;
          await _dbContext.Sessions.AddAsync(session);
          await _dbContext.SaveChangesAsync();

          if (session.SessionType != SessionType.Training_BCABA)
          {
            var note = new SessionNote { SessionId = session.SessionId };
            note.SessionResult = session.SessionStart.Date < new DateTime(2020, 7, 21) ? "NA" : null;
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

          var problems = await _utils.GetClientBehaviors(session.ClientId);
          foreach (var p in problems)
          {
            await _dbContext.SessionCollectBehaviorsV2.AddAsync(new SessionCollectBehaviorV2
            {
              SessionId = session.SessionId,
              ClientId = session.ClientId,
              NoData = true,
              ProblemId = p.ProblemId
            });
          }

          await _dbContext.SaveChangesAsync();

          var replacements = await _utils.GetClientReplacements(session.ClientId);
          foreach (var r in replacements)
          {
            await _dbContext.SessionCollectReplacementsV2.AddAsync(new SessionCollectReplacementV2
            {
              SessionId = session.SessionId,
              ClientId = session.ClientId,
              NoData = true,
              ReplacementId = r.ReplacementId
            });
          }

          await _dbContext.SaveChangesAsync();

          await _utils.RemovePassIfApply(user, session.SessionStart);

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
            s.BehaviorAnalysisCode.Color,
            Code = s.BehaviorAnalysisCode.Hcpcs,
            s.User,
            className = $"white--text v-card--hover  pa-0 ma-0 {s.BehaviorAnalysisCode.Color}",
            SessionType = Enum.GetName(typeof(SessionType), s.SessionType),
            s.TotalUnits,
            Pos = s.Pos.ToString().Replace("_", " "), //Enum.GetName(typeof(Pos), s.Pos),
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
          .Include(i => i.SessionNote).ThenInclude(i => i.Caregiver)
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
            s.SessionAnalystId,
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

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> GetSessionForPrint(int sessionId)
    {
      try
      {
        var x = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == sessionId);
        var lead = await _dbContext.Assignments
          .Where(w => w.ClientId == x.ClientId)
          .Where(w => w.User.RolId == 2 && w.Active)
          .Select(s => s.User).Include(i => i.UserSign)
          .FirstOrDefaultAsync();

        var clientDiagnosis = await _utils.GetClientDiagnosisBySession(sessionId);

        var session = await _dbContext
          .Sessions
          .AsNoTracking()
          .Where(w => w.SessionId.Equals(sessionId))
          .Include(i => i.SessionAnalyst).ThenInclude(i => i.UserSign)
          .Select(s => new
          {
            s.ClientId,
            ClientName = $"{s.Client.Firstname} {s.Client.Lastname}",
            ClientDob = s.Client.Dob,
            s.Client.MemberNo,
            ClientDiagnosis = clientDiagnosis,// s.Client.ClientDiagnostics.Where(w => w.Active).Select(s1 => new { s1.Diagnosis.Code, s1.Diagnosis.Description }),
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            UserLicense = s.User.LicenseNo,
            s.User.UserSign,
            s.User.UserId,
            Analyst = s.SessionAnalyst,
            AnalystSign = s.SessionAnalyst.UserSign,
            UserRol = s.User.Rol.RolName,
            UserRolShort = s.User.Rol.RolShortName,
            s.TotalUnits,
            ClientCode = s.Client.Code ?? "N/A",
            Pos = s.Pos.ToString().Replace('_', ' '),
            PosNum = s.Pos,
            SessionType = s.SessionType.ToString().Replace('_', ' '),
            SessionTypeNum = s.SessionType,
            Service = s.User.Rol.BehaviorAnalysisCode.Hcpcs,
            ServiceDescription = s.User.Rol.BehaviorAnalysisCode.Description,
            s.DriveTime,
            Caregiver = s.SessionType == SessionType.Training_BCABA ? $"{s.SessionSupervisionNote.Caregiver.CaregiverFullname}" : $"{s.SessionNote.Caregiver.CaregiverFullname}",
            CaregiverType = s.SessionType == SessionType.Training_BCABA ? $"{s.SessionSupervisionNote.Caregiver.CaregiverType.Description}" : $"{s.SessionNote.Caregiver.CaregiverType.Description}",
            CaregiverNote = s.SessionType == SessionType.Training_BCABA ? $"{s.SessionSupervisionNote.CaregiverNote}" : $"{s.SessionNote.CaregiverNote}",
            CaregiverTraining = s.SessionType == SessionType.Caregiver_Trainer
              ? new OkObjectResult(new
              {
                s.SessionNote.CaregiverTrainingObservationFeedback,
                s.SessionNote.CaregiverTrainingParentCaregiverTraining,
                s.SessionNote.CaregiverTrainingCompetencyCheck,
                s.SessionNote.CaregiverTrainingOther,
                s.SessionNote.CaregiverTrainingSummary
              }).Value
              : null,
            SupervisionNote = s.SessionType == SessionType.Training_BCABA
              ? new OkObjectResult(new
              {
                s.SessionSupervisionNote.isDirectSession,
                s.SessionSupervisionNote.WorkWith,
                s.SessionSupervisionNote.BriefObservation,
                s.SessionSupervisionNote.BriefReplacement,
                s.SessionSupervisionNote.BriefGeneralization,
                s.SessionSupervisionNote.BriefBCaBaTraining,
                s.SessionSupervisionNote.BriefInService,
                s.SessionSupervisionNote.BriefInServiceSubject,
                s.SessionSupervisionNote.BriefOther,
                s.SessionSupervisionNote.BriefOtherDescription,
                s.SessionSupervisionNote.OversightFollowUpBool,
                OversightFollowUp = s.SessionSupervisionNote.OversightFollowUp.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightDesigningBool,
                OversightDesigning = s.SessionSupervisionNote.OversightDesigning.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightContributingBool,
                OversightContributing = s.SessionSupervisionNote.OversightContributing.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightAnalyzingBool,
                OversightAnalyzing = s.SessionSupervisionNote.OversightAnalyzing.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightGoalsBool,
                OversightGoals = s.SessionSupervisionNote.OversightGoals.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightMakingDecisionsBool,
                OversightMakingDecisions = s.SessionSupervisionNote.OversightMakingDecisions.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightModelingBool,
                OversightModeling = s.SessionSupervisionNote.OversightModeling.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightResponseBool,
                OversightResponse = s.SessionSupervisionNote.OversightResponse.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.OversightOverallBool,
                OversightOverall = s.SessionSupervisionNote.OversightOverall.ToString().Replace('_', ' '),
                s.SessionSupervisionNote.CommentsRelated,
                s.SessionSupervisionNote.Recommendations,
                s.SessionSupervisionNote.Validation,
                s.SessionSupervisionNote.NextScheduledDate,
              }).Value
              : null,
            SessionNote = s.SessionType == SessionType.BA_Service
              ? new OkObjectResult(new
              {
                RiskBehavior = s.SessionNote.RiskBehavior != 0 ? s.SessionNote.RiskBehavior.ToString() : "Undefined",
                CrisisInvolved = s.SessionNote.RiskBehaviorCrisisInvolved,
                CrisisInvolvedExplain = s.SessionNote.RiskBehaviorExplain,
                s.SessionNote.ReinforcersEdibles,
                s.SessionNote.ReinforcersNonEdibles,
                s.SessionNote.ReinforcersOthers,
                s.SessionNote.ReinforcersResult,
                ParticipationLevel = s.SessionNote.ParticipationLevel != 0 ? s.SessionNote.ParticipationLevel.ToString() : "Undefined",
                s.SessionNote.ProgressNotes,
                s.SessionNote.FeedbackCaregiver,
                s.SessionNote.FeedbackCaregiverExplain,
                s.SessionNote.FeedbackOtherServices,
                s.SessionNote.FeedbackOtherServicesExplain,
                s.SessionNote.SummaryDirectObservation,
                s.SessionNote.SummaryObservationFeedback,
                s.SessionNote.SummaryImplementedReduction,
                s.SessionNote.SummaryImplementedReplacement,
                s.SessionNote.SummaryGeneralization,
                s.SessionNote.SummaryCommunication,
                s.SessionNote.SummaryOther,
                s.SessionNote.SessionResult,
                s.SessionNote.Supervision1,
                s.SessionNote.Supervision2,
                s.SessionNote.Supervision3,
                s.SessionNote.Supervision4,
                s.SessionNote.Supervision5,
                s.SessionNote.Supervision6,
                s.SessionNote.Supervision7,
                s.SessionNote.SupervisionOther,
                Problems = s.SessionProblemNotes.Select(w => new
                {
                  Problem = w.ProblemBehavior.ProblemBehaviorDescription,
                  w.DuringWichActivities,
                  w.ReplacementInterventionsUsed,
                  SessionProblemReplacements = w.SessionProblemNoteReplacements.Where(q => q.Active).Select(q => q.ReplacementProgram.ReplacementProgramDescription).OrderBy(o => o).ToList()
                }).OrderBy(o => o.Problem).ToList(),
                DataCollection = new OkObjectResult(new
                {
                  s.SessionCollectBehaviorsV2,
                  s.SessionCollectReplacementsV2
                }).Value
              }).Value
              : null
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
        var user = await _utils.GetUserByUsername(session.User.Username);
        var currentUser = await _utils.GetCurrentUser();
        var currentSession = await _dbContext.Sessions.AsNoTracking().FirstOrDefaultAsync(s => s.SessionId == session.SessionId);
        if ((currentSession.SessionStatus == SessionStatus.Billed || currentSession.SessionStatus == SessionStatus.Reviewed) && user.RolId != 1) throw new Exception("This session has been checked, billed or reviewed. You cannot edit it.");

        //check if user can create after x hours and have any pass
        if (!_utils.CanCreateAfterHours(currentUser, session.SessionStart)) throw new Exception("You can not edit this session bacause exced the hours allowed and you dont have any pass.");

        //if (!ModelState.IsValid) ret
        if (session.SessionStatus != SessionStatus.Checked) _dbContext.Update(session).Property(s => s.SessionStatus).CurrentValue = SessionStatus.Edited;

        if (session.SessionType == SessionType.Training_BCABA)
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
        await _utils.RemovePassIfApply(user, session.SessionStart);
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
        var client = await _utils.GetClientById(clientId);
        var date = Convert.ToDateTime(dateStr);
        var monthlyLookUp = await _dbContext.MonthlyNotes
          .Where(w => w.ClientId.Equals(clientId))
          .Where(w => w.Year == date.Year && w.Month == date.Month)
          .FirstOrDefaultAsync();
        if (monthlyLookUp == null)
        {
          var analyst = client.Assignments?.Where(w => w.Active && w.User.RolId == 2).FirstOrDefault();
          var assistant = client.Assignments?.Where(w => w.Active && w.User.RolId == 3).FirstOrDefault();
          var rbt = client.Assignments?.Where(w => w.Active && w.User.RolId == 4).FirstOrDefault();
          var newMonthlyNote = new MonthlyNote
          {
            ClientId = clientId,
            MonthlyNoteDate = date.Date,
            Month = date.Month,
            Year = date.Year,
            MonthlyAnalystId = analyst?.UserId,
            MonthlyAssistantId = assistant?.UserId,
            MonthlyRbtId = rbt?.UserId
          };
          await _dbContext.MonthlyNotes.AddAsync(newMonthlyNote);
          await _dbContext.SaveChangesAsync();
          monthlyLookUp = newMonthlyNote;
        }

        return Ok(new { note = monthlyLookUp, Assignments = client.Assignments?.ToList() });
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

    [HttpGet("[action]/{clientId}/{problemId?}/{dateStart?}/{dateEnd?}")]
    public async Task<IActionResult> GetProblemBehaviorsChart(int clientId, string problemId = "0", DateTime? dateStart = null, DateTime? dateEnd = null)
    {
      if (clientId == 0) return Ok();
      var problems = problemId == "0" ? new List<int>() : problemId.Split(',').Select(int.Parse).ToList();
      var chartData = await _collection.GetClientBehaviorChart(clientId, problems, dateStart, dateEnd);
      return Ok(chartData);
    }

    [HttpGet("[action]/{clientId}/{problemId}/{dateEnd?}")]
    public async Task<IActionResult> GetBehaviorMontlyChart(int clientId, int problemId, DateTime? dateEnd = null)
    {
      if (clientId == 0 || problemId == 0) return Ok();
      var endCal = dateEnd ?? DateTime.Today;
      var chartData = await _collection.GetClientBehaviorMonthlyChart(clientId, problemId, endCal);
      return Ok(chartData);
    }

    [HttpGet("[action]/{clientId}/{replacementId}/{dateEnd?}")]
    public async Task<IActionResult> GetReplacementMontlyChart(int clientId, int replacementId, DateTime? dateEnd = null)
    {
      if (clientId == 0 || replacementId == 0) return Ok();
      var endCal = dateEnd ?? DateTime.Today;
      var chartData = await _collection.GetClientReplacementMonthlyChart(clientId, replacementId, endCal);
      return Ok(chartData);
    }

    [HttpGet("[action]/{clientId}/{replacementId?}/{dateStart?}/{dateEnd?}")]
    public async Task<IActionResult> GetReplacementProgramChart(int clientId, string replacementId = "0", DateTime? dateStart = null, DateTime? dateEnd = null)
    {
      if (clientId == 0) return Ok();
      var replacements = replacementId == "0" ? new List<int>() : replacementId.Split(',').Select(int.Parse).ToList();
      var chartData = await _collection.GetClientReplacementChart(clientId, replacements, dateStart, dateEnd);
      return Ok(chartData);
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

          comp = new CompetencyCheck
          {
            Date = DateTime.Today,
            CompetencyCheckClientParams = compClientParams
          };
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
        if (changeSessionStatus.SessionStatus == SessionStatus.Billed) await _utils.NewEntryLog(changeSessionStatus.SessionId, "Billed", $"Session was billed.", "fa-dollar-sign", "green");
        else await _utils.NewEntryLog(changeSessionStatus.SessionId, "Status", $"Status changed to: {changeSessionStatus.SessionStatus}");
        return Ok(newStatus);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> SaveSessionSign([FromBody] SessionSign sign)
    {
      try
      {
        var signSaved = await _dbContext.SessionSigns.FirstOrDefaultAsync(w => w.Auth == sign.Auth);
        signSaved.Sign = sign.Sign;
        signSaved.SignedDate = DateTime.UtcNow;
        _dbContext.Update(signSaved);
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(signSaved.SessionId, "Signed", "Status electronically signed", "fa-signature");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]/{sessionId}")]
    public async Task<IActionResult> SendUrlSign([FromBody] UrlClass url, [FromRoute] int sessionId)
    {
      using (var transaction = await _dbContext.Database.BeginTransactionAsync())
      {
        try
        {
          var session = await _dbContext.Sessions.Include(i => i.Client).Include(i => i.User).FirstOrDefaultAsync(w => w.SessionId == sessionId);
          if (DateTime.UtcNow < session.SessionEnd) throw new Exception("Caregiver can not sign the session before it ends.");
          var sessionNotes = await _dbContext.SessionNotes.FirstOrDefaultAsync(w => w.SessionId.Equals(sessionId));
          var supervisionNotes = await _dbContext.SessionSupervisionNotes.FirstOrDefaultAsync(w => w.SessionId.Equals(sessionId));
          if (session.SessionType != SessionType.Training_BCABA && (sessionNotes == null || sessionNotes.CaregiverId == null)) throw new Exception("You must select a valid caregiver for this session. If you already select a caregiver, please save session before.");
          if (session.SessionType == SessionType.Training_BCABA && (supervisionNotes == null || supervisionNotes.CaregiverId == null)) throw new Exception("You must select a valid caregiver for this session. If you already select a caregiver, please save session before.");
          var caregiverId = session.SessionType != SessionType.Training_BCABA ? sessionNotes.CaregiverId : supervisionNotes.CaregiverId;
          var caregiver = await _dbContext.Caregivers.FirstOrDefaultAsync(w => w.CaregiverId == caregiverId);
          if (!StaticUtils.IsValidEmail(caregiver.Email)) throw new Exception("The caregiver hasn't a valid email address.");
          var sign = await _dbContext.SessionSigns.FirstOrDefaultAsync(w => w.SessionId == sessionId);
          var token = sign?.Auth ?? Guid.NewGuid();
          if (sign == null)
          {
            sign = new SessionSign
            {
              Auth = token,
              SessionId = sessionId
            };
            await _dbContext.AddAsync(sign);
            await _dbContext.SaveChangesAsync();
          }

          var to = caregiver.Email;
          var subject = $"Please sign the {session.SessionStart.ToShortDateString()} session.";
          var body = $@"Client: <b>{session.Client.Firstname} {session.Client.Lastname}</b><br>
                        Units: <b>{session.TotalUnits}</b> ({session.TotalUnits / 4:n1}) hour(s)<br>
                        Service by: <b>{session.User.Firstname} {session.User.Lastname}</b><br>
                        <br>
                        <a href=""{url.Url}/{token}"">Tap here to sign</a>
                        <br>
                        Thank you";
          var email = await _utils.CreateEmail(to, subject, body, MessageType.General);
          var response = await _utils.SendEmailsAsync(email);
          transaction.Commit();
          return Ok(new { response, sign });
        }
        catch (System.Exception e)
        {
          return BadRequest(e.Message);
        }
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveSessionCollectBehavior([FromBody] SessionCollectBehaviorV2 s)
    {
      try
      {
        _dbContext.Update(s);
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
        var collectBehaviors = await _dbContext.SessionCollectBehaviorsV2
          .Where(w => w.SessionId == sessionId)
          .Include(w => w.Behavior)
          .OrderBy(w => w.Behavior.ProblemBehaviorDescription)
          .ToListAsync();
        return Ok(collectBehaviors);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveSessionCollectReplacement([FromBody] SessionCollectReplacementV2 s)
    {
      try
      {
        _dbContext.Update(s);
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
        var collectReplacements = await _dbContext.SessionCollectReplacementsV2
          .Where(w => w.SessionId == sessionId)
          .Include(w => w.Replacement)
          .OrderBy(w => w.Replacement.ReplacementProgramDescription)
          .ToListAsync();

        return Ok(collectReplacements);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [AllowAnonymous]
    [HttpGet("testing")]
    public async Task<IActionResult> TestingHangFire()
    {
      // var a = await _collection.GetClientProblemsByWeek(21, 16);
      //var a = await _collection.GetClientBehaviorChart(21, new List<int>());
      // var a = await _collection.GetClientReplacements(new DateTime(2019, 7, 7), new DateTime(2019, 7, 13), 21, 4);
      //var a = await _collection.GetClientReplacementsByWeek(21, 4);
      // await _utils.MidNightProcess();
      //await _utils.SendEmailsAsync();

      var a = await _collection.GetMonthlyDataReplacement(68, new DateTime(2020, 1, 1));
      //await Task.Delay(1);

      return Ok(a);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RejectSession([FromBody] RejectSession reject)
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == reject.SessionId);
        var sessionNote = await _dbContext.SessionNotes.FirstOrDefaultAsync(w => w.SessionId == reject.SessionId);
        var supervisionNote = await _dbContext.SessionSupervisionNotes.FirstOrDefaultAsync(w => w.SessionId == reject.SessionId);
        if (session.SessionType != SessionType.Training_BCABA && (sessionNote == null || session == null)) throw new Exception("Session and/or Note not found");
        if (session.SessionType == SessionType.Training_BCABA && (supervisionNote == null || session == null)) throw new Exception("Session and/or Supervision Note not found");

        session.SessionStatus = SessionStatus.Rejected;
        if (session.SessionType != SessionType.Training_BCABA) sessionNote.RejectNotes = reject.RejectMessage;
        if (session.SessionType == SessionType.Training_BCABA) supervisionNote.RejectNotes = reject.RejectMessage;

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
            Pos = s.Pos.ToString().Replace("_", " "), //Enum.GetName(typeof(Pos), s.Pos),
            SessionStatus = s.SessionStatus.ToString(),
            SessionStatusCode = s.SessionStatus,
            SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString(),
            s.User.Rol.RolShortName,
            s.User,
            UserRole = s.User.Rol
          })
          .ToListAsync();
        return Ok(sessions);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}/{onlyActive?}")]
    public async Task<IActionResult> GetClientBehaviors(int clientId, bool onlyActive = true)
    {
      try
      {
        var behaviors = await _utils.GetClientBehaviors(clientId, onlyActive);
        return Ok(behaviors);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{clientId}/{onlyActive?}")]
    public async Task<IActionResult> GetClientReplacements(int clientId, bool onlyActive = true)
    {
      try
      {
        var replacements = await _utils.GetClientReplacements(clientId, onlyActive);
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
        var units = diff.TotalMinutes / 15;
        var sum1Unit = (units % 1) > 0.5 ? 1 : 0;
        var unitsTruncate = Decimal.Truncate((decimal)units) + sum1Unit;
        session.TotalUnits = (int)unitsTruncate;// Convert.ToInt32(diff.TotalMinutes / 15);
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(session.SessionId, "Time", "Session time edited", "fa-clock", "orange");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPut( "[action]" )]
    public async Task<IActionResult> MatchingSessionTellus( [FromBody] MatchingSessionTellus s )
    {
      try
      {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync( w => w.SessionId == s.SessionId );
        if ( session == null )
          throw new Exception( "Session not found" );
        bool timeChanged = false;
        DateTime dt = DateTime.ParseExact( "01/01/0001 0:00:00", "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture );
        if ( s.Start != dt && s.End != dt )
        {
          session.SessionStart = s.Start.ToUniversalTime();
          session.SessionEnd = s.End.ToUniversalTime();
          timeChanged = true;
        }
        session.Matched = true;
        var diff = session.SessionEnd - session.SessionStart;
        var units = diff.TotalMinutes / 15;
        var sum1Unit = ( units % 1 ) > 0.5 ? 1 : 0;
        var unitsTruncate = Decimal.Truncate( (decimal) units ) + sum1Unit;
        session.TotalUnits = (int) unitsTruncate;
        await _dbContext.SaveChangesAsync();
        if ( timeChanged )
        {
          await _utils.NewEntryLog( session.SessionId, "Time", "Session time edited and marked as matched", "fa-clock", "orange" );
        }
        else
        {
          await _utils.NewEntryLog( session.SessionId, "Time", "Session marked as matched", "fa-clock", "orange" );
        }
        return Ok();
      }
      catch ( Exception e )
      {
        return BadRequest( e.InnerException?.Message ?? e.Message );
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
        await _utils.NewEntryLog(session.SessionId, "Pos", $"Session POS edited to {(Pos)s.Value}", "fa-clock", "teal");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}/{chartMaxDate?}")]
    public async Task<IActionResult> GetCompetencyCheckCaregiversCharts(int clientId, DateTime? chartMaxDate = null)
    {
      var competencyChecks = await _dbContext.CompetencyChecks.Where(w => w.ClientId == clientId).Include(i => i.Caregiver).Include(i => i.User).ToListAsync();
      var caregivers = competencyChecks.Where(w => w.Caregiver != null).Select(s => s.Caregiver).Distinct().ToList();
      var rbts = competencyChecks.Where(w => w.User != null).Select(s => s.User).Distinct().ToList();

      var charts = new List<Object>();

      foreach (var caregiver in caregivers)
      {
        var c = await _utils.GetCompetencyCheckChart(clientId, CompetencyCheckType.Caregiver, caregiver.CaregiverId, chartMaxDate);
        charts.Add(c);
      }

      foreach (var rbt in rbts)
      {
        var c = await _utils.GetCompetencyCheckChart(clientId, CompetencyCheckType.Rbt, rbt.UserId, chartMaxDate);
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

    [AllowAnonymous]
    [HttpGet("[action]/{token}")]
    public async Task<IActionResult> GetSessionDetailedForSign(Guid token)
    {
      try
      {
        var sign = await _dbContext.SessionSigns.FirstOrDefaultAsync(w => w.Auth == token);
        if (sign == null) return Ok(new { error = 1, message = "Security token not found, this may happend due to another email sent before causing erasing this token. Please check your last email or contact a system administrator." });
        var session = await _dbContext
          .Sessions
          .AsNoTracking()
          .Where(w => w.SessionId.Equals(sign.SessionId))
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

    [HttpGet("[action]/{dateStr}/{clientId}")]
    public async Task<IActionResult> GetCaregiverCollectionData(string dateStr, int clientId)
    {
      try
      {
        var date = Convert.ToDateTime(dateStr);
        var data = await _dbContext.CaregiverDataCollections
          .Where(w => w.CollectDate.Date == date.Date && w.ClientId == clientId)
          .Include(i => i.CaregiverDataCollectionProblems)
          .Include(i => i.CaregiverDataCollectionReplacements)
          .FirstOrDefaultAsync();

        if (data == null)
        {
          var problems = await _utils.GetClientBehaviors(clientId);
          var replacements = await _utils.GetClientReplacements(clientId);

          var caregiverDataCollectionProblems = new List<CaregiverDataCollectionProblem>();
          var caregiverDataCollectionReplacements = new List<CaregiverDataCollectionReplacement>();

          problems.ForEach(s =>
          {
            caregiverDataCollectionProblems.Add(new CaregiverDataCollectionProblem
            {
              ProblemId = s.ProblemId,
              Count = null
            });
          });

          replacements.ForEach(s =>
          {
            caregiverDataCollectionReplacements.Add(new CaregiverDataCollectionReplacement
            {
              ReplacementId = s.ReplacementId,
              TotalTrial = null,
              TotalCompleted = null
            });
          });

          data = data ?? new CaregiverDataCollection
          {
            CollectDate = date,
            ClientId = clientId,
            CaregiverDataCollectionProblems = caregiverDataCollectionProblems,
            CaregiverDataCollectionReplacements = caregiverDataCollectionReplacements
          };
        }

        return Ok(data);
      }
      catch (System.Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveCaregiverCollectionData([FromBody] CaregiverDataCollection caregiverDataCollection)
    {
      try
      {
        _dbContext.Update(caregiverDataCollection);
        await _dbContext.SaveChangesAsync();

        //await _utils.NewEntryLog(session.SessionId, "Rejected", $"Session rejected: {reject.RejectMessage}", "fa-exclamation-circle", "red");

        return Ok(caregiverDataCollection);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetCaregiverCollectionDataForCalendar(int clientId)
    {
      try
      {
        var sessions = await _dbContext.CaregiverDataCollections
          .Where(w => w.ClientId.Equals(clientId))
          .Select(s => new
          {
            s.CaregiverDataCollectionId,
            title = "Collected",
            date = s.CollectDate.ToString("yyyy-MM-dd"),
          })
          .ToListAsync();
        return Ok(sessions);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpDelete("[action]/{id}")]
    public async Task<IActionResult> DeleteSign(int id)
    {
      try
      {
        var sign = await _dbContext.SessionSigns.FirstOrDefaultAsync(w => w.SessionId == id);
        if (sign == null) throw new Exception("Sign data not found");
        _dbContext.SessionSigns.Remove(sign);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (System.Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetSessionsCalendar3(int clientId)
    {
      try
      {
        var sessions = await _dbContext.Sessions
          .Where(w => w.ClientId.Equals(clientId))
          .Select(s => new
          {
            s.SessionId,
            SessionType = Enum.GetName(typeof(SessionType), s.SessionType).ToLower(),
            Title = s.SessionType.ToString().Replace('_', ' '),
            SessionStart = s.SessionStart.ToString("u"),
            SessionEnd = s.SessionEnd.ToString("u"),
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            Color = ((SessionStatusColors)s.SessionStatus).ToString(),
            s.TotalUnits,
            SessionStatus = s.SessionStatus.ToString(),
            s.User.Rol.RolShortName,
            s.User,
            UserRole = s.User.Rol
          })
          .ToListAsync();

        var caregivers = await _dbContext.CaregiverDataCollections
          .Where(w => w.ClientId.Equals(clientId))
          .Select(s => new
          {
            s.CaregiverDataCollectionId,
            title = "Caregiver Collected",
            date = s.CollectDate.ToString("yyyy-MM-dd")
          })
          .ToListAsync();
        return Ok(new { sessions, caregivers });
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}/{problemId}/{dateStart?}/{dateEnd?}")]
    public async Task<IActionResult> GetClientBehaviorChartValuesWeek(int clientId, int problemId, DateTime? dateStart = null, DateTime? dateEnd = null)
    {
      if (clientId == 0) return Ok();
      var chartData = await _collection.GetClientBehaviorChartValuesWeek(clientId, problemId, dateStart, dateEnd);
      var stoStatus = await _collection.GetStoStatusBehavior(clientId, problemId);
      return Ok(new { chartData = chartData.Select(s => s == null ? 0 : s), stoStatus });
    }

    [HttpGet("[action]/{clientId}/{replacementId}/{dateStart?}/{dateEnd?}")]
    public async Task<IActionResult> GetClientReplacementChartValuesWeek(int clientId, int replacementId, DateTime? dateStart = null, DateTime? dateEnd = null)
    {
      if (clientId == 0) return Ok();
      var chartData = await _collection.GetClientReplacementChartValuesWeek(clientId, replacementId, dateStart, dateEnd);
      var stoStatus = await _collection.GetStoStatusReplacement(clientId, replacementId);
      return Ok(new { chartData = chartData.Select(s => s == null ? 0 : s), stoStatus });
    }

    [HttpGet("[action]/{assessmentId}")]
    public async Task<IActionResult> MarkAssessmentAsBilled(int assessmentId)
    {
      var a = await _dbContext.Assessments.FirstOrDefaultAsync(w => w.AssessmentId == assessmentId);
      a.Status = 1;
      await _dbContext.SaveChangesAsync();
      return Ok();
    }

    [HttpGet("[action]/{clientId}/{dateStr}/{userName?}")]
    public async Task<IActionResult> GetUnitsByWeek(int clientId, string dateStr, string userName)
    {
      var date = dateStr == null ? DateTime.Today : Convert.ToDateTime(dateStr);
      userName = userName ?? User.Identity.Name;
      var user = await _utils.GetUserByUsername(userName);
      var (Allowed, Total) = await _utils.GetUnitsInWeek(date, user.UserId, clientId);
      return Ok(new
      {
        Allowed,
        Total
      });
    }

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> CheckMatchingPercentaje(int sessionId)
    {
      var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == sessionId);
      if (session.SessionType != SessionType.BA_Service) return Ok(0);
      var notes = await _dbContext.SessionNotes.FirstOrDefaultAsync(w => w.SessionId == sessionId);
      var note = notes.ProgressNotes;

      var allSessions = await _dbContext.Sessions
      .Where(w => w.UserId == session.UserId && w.ClientId == session.ClientId && w.SessionType == SessionType.BA_Service && w.SessionId != sessionId)
      .Select(s => new
      {
        s.SessionId,
        s.SessionNote.ProgressNotes,
        s.SessionStart.Date,
        Percentaje = _utils.CalculateSimilarity(note, s.SessionNote.ProgressNotes)
        // Percentaje = _utils.Compare(note, s.SessionNote.ProgressNotes)
      })
      .OrderByDescending(o => o.Percentaje)
      .ThenByDescending(o => o.Date)
      .ToListAsync();

      var matchest = allSessions.FirstOrDefault();

      return Ok(matchest == null || matchest.Percentaje == 0 ? (new { SessionId = 0, ProgressNotes = "", Date = DateTime.Now, Percentaje = 0.1d }) : matchest);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> CheckMatchingPercentajeString([FromBody] MatchSessionString matchSessionString)
    {
      var note = matchSessionString.ProgressNotes;
      var session = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == matchSessionString.SessionId);
      if (session.SessionType != SessionType.BA_Service) return Ok(0);
      var allSessions = await _dbContext.Sessions
      .Where(w => w.UserId == session.UserId && w.ClientId == session.ClientId && w.SessionType == SessionType.BA_Service && w.SessionId != matchSessionString.SessionId)
      .Select(s => new
      {
        s.SessionId,
        s.SessionNote.ProgressNotes,
        s.SessionStart.Date,
        Percentaje = _utils.CalculateSimilarity(note, s.SessionNote.ProgressNotes)
        // Percentaje = _utils.Compare(note, s.SessionNote.ProgressNotes)
      })
      .OrderByDescending(o => o.Percentaje)
      .ThenByDescending(o => o.Date)
      .ToListAsync();

      var matchest = allSessions.FirstOrDefault();

      return Ok(matchest);
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangeSessionAnalyst([FromBody] ChangeAnalystModel model)
    {
      try
      {
        var s = await _dbContext.Sessions.FirstOrDefaultAsync(w => w.SessionId == model.SessionId);
        s.SessionAnalystId = model.AnalystId;
        await _dbContext.SaveChangesAsync();
        await _utils.NewEntryLog(model.SessionId, "Analyst changed", "Session analyst was changed", "fa-user");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AdjustSessionAnalyst([FromBody] AdjustSessionAnalystModel model)
    {
      try
      {
        var sessions = await _dbContext.Sessions
        .Where(w => w.ClientId == model.ClientId)
        .Where(w => w.SessionStart.Date >= model.From && w.SessionStart.Date <= model.To)
        .ToListAsync();

        sessions.ForEach(f => f.SessionAnalystId = model.AnalystId);
        await _dbContext.SaveChangesAsync();
        return Ok(sessions.Count);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{sessionId}")]
    public async Task<IActionResult> GetSessionSign(int sessionId)
    {
      var sign = await _dbContext.SessionSigns.Where(w => w.SessionId == sessionId).FirstOrDefaultAsync();
      return Ok(sign ?? new SessionSign());
    }

    [HttpGet("[action]/{sessionId}/{clientId}")]
    public async Task<IActionResult> RecreateBehaviors(int sessionId, int clientId)
    {
      var problems = await _dbContext.SessionProblemNotes.Where(w => w.SessionId == sessionId).Include(i => i.SessionProblemNoteReplacements).ToListAsync();
      foreach (var p in problems) _dbContext.RemoveRange(p.SessionProblemNoteReplacements);
      _dbContext.RemoveRange(problems);
      await _dbContext.SaveChangesAsync();
      await _utils.AddSessionProblemNotes(sessionId, clientId);
      return Ok();
    }
  }
}