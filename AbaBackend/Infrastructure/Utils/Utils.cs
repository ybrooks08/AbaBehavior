using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Model.Session;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AbaBackend.Infrastructure.Utils
{

  public class Utils : IUtils
  {
    private readonly AbaDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContext;
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;

    public Utils(AbaDbContext dbContext, IHttpContextAccessor httpContext, ILogger<Utils> logger, IConfiguration configuration)
    {
      _dbContext = dbContext;
      _httpContext = httpContext;
      _logger = logger;
      _configuration = configuration;
    }

    public async Task<User> GetUserByUsername(string username)
    {
      var user = await _dbContext.Users
                                 .Include(i => i.Rol)
                                 .ThenInclude(t => t.BehaviorAnalysisCode)
                                 .Include(i => i.UserSign)
                                 .FirstOrDefaultAsync(w => w.Username.Equals(username));
      return user;
    }

    public async Task<User> GetCurrentUser()
    {
      var user = await GetUserByUsername(_httpContext.HttpContext.User.Identity.Name);
      return user;
    }

    public decimal ConvertTimeToNumber(DateTime time)
    {
      var h = time.Hour;
      var m = time.Minute;
      return h + (m / 60m);
    }

    public async Task<bool> CheckAssignmentClientActive(int userId, int clientId)
    {
      var assignments = await _dbContext.Assignments
                                        .Where(w => w.UserId.Equals(userId) && w.ClientId.Equals(clientId) && w.Active)
                                        .FirstOrDefaultAsync();
      if (assignments == null) return false;
      return true;
    }

    public async Task<List<Assessment>> GetClientValidAssessmentsForUser(DateTime date, int clientId, User user)
    {
      var assessments = await _dbContext.Assessments
                                        .Where(w => w.ClientId.Equals(clientId))
                                        .Where(w => w.BehaviorAnalysisCodeId.Equals(user.Rol.BehaviorAnalysisCodeId))
                                        .Where(w => date.Date >= w.StartDate.Date && date.Date <= w.EndDate.Date)
                                        .ToListAsync();
      return assessments;
    }

    public async Task<int> GetUnitsAvailable(DateTime? date, int clientId, User user)
    {
      if (user == null) return 0;
      date = date ?? DateTime.Today;
      var assessments = await GetClientValidAssessmentsForUser((DateTime)date, clientId, user);
      var lastAssessment = assessments.OrderByDescending(o => o.EndDate).FirstOrDefault();
      if (lastAssessment == null || lastAssessment.TotalUnits <= 0) return 0;

      var unitsSum = await _dbContext.Sessions
                                     .Where(w => w.SessionStart.Date >= lastAssessment.StartDate.Date && w.SessionStart.Date <= lastAssessment.EndDate.Date)
                                     .Where(w => w.ClientId.Equals(clientId))
                                     .Where(w => w.BehaviorAnalysisCodeId.Equals(user.Rol.BehaviorAnalysisCodeId))
                                     .Select(s => s.TotalUnits)
                                     .SumAsync();

      var available = lastAssessment.TotalUnits - unitsSum;

      return available;
    }

    public async Task MidNightProcess()
    {
      var frecuency = _configuration.GetSection("NotificationFrecuency").Get<List<int>>();
      foreach (int days in frecuency)
      {
        await CreateDocumentsEmails(days);
        await CreateReferralsEmails(days);
        await CreateAssessmentEmails(days);
      }

      var checkStos = new Classes.CheckStos(_dbContext);
      await checkStos.ProcessStos();
    }

    internal async Task CreateDocumentsEmails(int days)
    {
      var documents = await _dbContext.DocumentsUsers
                                      .Where(w => EF.Functions.DateDiffDay(DateTime.Today, w.Expires != null ? Convert.ToDateTime(w.Expires).Date : new DateTime(2999, 1, 1).Date) == days && w.Active)
                                      .Where(w => w.User.Active)
                                      .Select(s => new
                                      {
                                        UserEmail = s.User.Email,
                                        UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
                                        s.User.Rol.RolName,
                                        s.Document.DocumentName,
                                        s.Document.DocumentGroup.GroupName,
                                        s.Expires,
                                        Days = EF.Functions.DateDiffDay(DateTime.Today, Convert.ToDateTime(s.Expires).Date)
                                      })
                                      .ToListAsync();

      foreach (var doc in documents)
      {
        var to = doc.UserEmail;
        var subject = $"ALERT expiring documents in {days} days";
        var body = $@"<h3>The following user has documents that are about to expire:</h3><br>
                      User: <b>{doc.UserFullname} ({doc.RolName})</b><br> 
                      Document Group: <b>{doc.GroupName}</b> <br> 
                      Document: <b>{doc.DocumentName}</b> <br> 
                      Expires: <b>{Convert.ToDateTime(doc.Expires).ToLongDateString()}</b> <br> 
                      Days left: <span style='color:red;'><b>{doc.Days}</b></span>";
        await CreateEmail(to, subject, body, MessageType.Hr);
      }
    }

    internal async Task CreateReferralsEmails(int days)
    {
      var referrals = await _dbContext.Referrals
                                      .Where(w => EF.Functions.DateDiffDay(DateTime.Today, w.DateExpires.Date) == days)
                                      .Where(w => w.Client.Active)
                                      .Where(w => w.Active)
                                      .Select(s => new
                                      {
                                        ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
                                        s.Client.Code,
                                        s.ReferralFullname,
                                        s.Specialty,
                                        s.DateReferral,
                                        s.DateExpires,
                                        Days = EF.Functions.DateDiffDay(DateTime.Today, s.DateExpires.Date)
                                      })
                                      .ToListAsync();
      foreach (var referr in referrals)
      {
        var to = ""; //referr.UserEmail;
        var subject = $"ALERT expiring referral in {days} days";
        var body = $@"<h3>The following client has a referral that is about to expire:</h3><br>
                      Client: <b>{referr.ClientFullname}</b><br> 
                      Referral: <b>{referr.ReferralFullname}</b> <br> 
                      Specialty: <b>{referr.Specialty}</b> <br> 
                      Expires: <b>{Convert.ToDateTime(referr.DateExpires).ToLongDateString()}</b> <br> 
                      Days left: <span style='color:red;'><b>{referr.Days}</b></span>";
        await CreateEmail(to, subject, body, MessageType.Hr);
      }
    }

    internal async Task CreateAssessmentEmails(int days)
    {
      var assessments = await _dbContext.Assessments
                                        .Where(w => (EF.Functions.DateDiffDay(DateTime.Today, w.EndDate.Date) == days) && w.BehaviorAnalysisCode.Checkable)
                                        .Where(w => w.Client.Active)
                                        .Select(s => new
                                        {
                                          ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
                                          s.TotalUnits,
                                          s.StartDate,
                                          s.EndDate,
                                          Days = EF.Functions.DateDiffDay(DateTime.Today, s.EndDate.Date),
                                          s.BehaviorAnalysisCode
                                        })
                                        .ToListAsync();
      foreach (var assess in assessments)
      {
        var to = ""; //referr.UserEmail;
        var subject = $"ALERT expiring assessment in {days} days";
        var body = $@"<h3>The following client has an assessment that is about to expire:</h3><br>
                      Client: <b>{assess.ClientFullname}</b><br> 
                      Units left: <b>{assess.TotalUnits}</b> <br> 
                      Code: <b>{assess.BehaviorAnalysisCode.Hcpcs} ({assess.BehaviorAnalysisCode.Description})</b> <br> 
                      Expires: <b>{Convert.ToDateTime(assess.EndDate).ToLongDateString()}</b> <br> 
                      Days left: <span style='color:red;'><b>{assess.Days}</b></span>";
        await CreateEmail(to, subject, body, MessageType.Hr);
      }
    }

    public async Task<bool> CreateEmail(string to, string subject, string message, MessageType messageType)
    {
      try
      {
        var email = new Email
        {
          To = to,
          Subject = subject,
          Body = message,
          MesssageType = messageType,
          Created = DateTime.Now
        };
        await _dbContext.Emails.AddAsync(email);
        await _dbContext.SaveChangesAsync();
        return true;
      }
      catch
      {
        return false;
      }
    }

    public async Task<bool> SendEmailsAsync(bool sendGlobal = true)
    {
      var now = DateTime.Now.TimeOfDay;
      var earliestEmail = new TimeSpan(8, 0, 0);
      var laterEmail = new TimeSpan(21, 0, 0);
      if (now < earliestEmail || now > laterEmail) return false;

      string from;
      string toGlobal;

      try
      {
        var pendingEmails = await _dbContext.Emails
                                            .Where(w => w.Sent == null)
                                            .OrderBy(w => w.EmailId)
                                            .ToListAsync();
        if (!pendingEmails.Any()) return false;
        from = _configuration["Email:From"];
        toGlobal = _configuration["Email:GlobalEmail"];
        var host = _configuration["Email:Host"];
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];
        var port = Convert.ToInt32(_configuration["Email:Port"]);
        using (var client = new SmtpClient())
        {
          client.ServerCertificateValidationCallback = (s, c, h, e) => true;
          await client.ConnectAsync(host, port, false);
          await client.AuthenticateAsync(username, password);
          foreach (var email in pendingEmails)
          {
            var msg = CreateEmails(email);
            await client.SendAsync(msg);
            email.Sent = DateTime.Now;
            await _dbContext.SaveChangesAsync();
          }

          await client.DisconnectAsync(true);
        }
      }
      catch (System.Exception e)
      {
        _logger.LogCritical("Error: " + e.Message);
        throw;
      }

      return true;

      MimeMessage CreateEmails(Email email)
      {
        var msg = new MimeMessage();
        var to = email.To.Split(';', StringSplitOptions.RemoveEmptyEntries);
        msg.From.Add(new MailboxAddress("DONOTREPLY", from));
        foreach (var e in to) msg.To.Add(new MailboxAddress(e));
        if (sendGlobal) msg.Cc.Add(new MailboxAddress(toGlobal));
        msg.Subject = email.Subject;
        msg.Body = new TextPart("html") { Text = email.Body };
        msg.Priority = MessagePriority.Urgent;
        return msg;
      }
    }

    public async Task<string> CheckMaxHoursClientsInDay(DateTime date, int clientId, int totalUnits)
    {
      var maxUnits = Convert.ToInt32(_configuration["Session:MaxUnitsClientSessionByDay"]);
      var allSessionsUnits = await _dbContext.Sessions
                                             .Where(w => w.ClientId.Equals(clientId) && w.SessionStart.Date.Equals(date.Date))
                                             .Select(s => s.TotalUnits)
                                             .SumAsync();
      var totalUnitsToCreate = allSessionsUnits + totalUnits;
      if (totalUnitsToCreate > maxUnits) return $"You can't create this session because client exceeds the {maxUnits} units per day.";
      return "ok";
    }

    public async Task<string> CheckMaxHoursUserInDay(DateTime date, int userId, int totalUnits)
    {
      var maxUnits = Convert.ToInt32(_configuration["Session:MaxUnitsUserSessionByDay"]);
      var allSessionsUnits = await _dbContext.Sessions
                                             .Where(w => w.UserId.Equals(userId) && w.SessionStart.Date.Equals(date.Date))
                                             .Select(s => s.TotalUnits)
                                             .SumAsync();
      var totalUnitsToCreate = allSessionsUnits + totalUnits;
      if (totalUnitsToCreate > maxUnits) return $"You can't create this session because exceeds the {maxUnits} units per day.";
      return "ok";
    }

    public async Task<string> CheckMaxHoursUserByClientInDay(DateTime date, int userId, int clientId, int totalUnits)
    {
      var user = await GetCurrentUser();
      var maxUnits = Convert.ToInt32(_configuration[$"Session:MaxUnitsByClient{user.Rol.BehaviorAnalysisCode.Hcpcs}"]);
      var allSessionsUnits = await _dbContext.Sessions
                                             .Where(w => w.UserId.Equals(userId) && w.ClientId.Equals(clientId) && w.SessionStart.Date.Equals(date.Date))
                                             .Select(s => s.TotalUnits)
                                             .SumAsync();
      var totalUnitsToCreate = allSessionsUnits + totalUnits;
      if (totalUnitsToCreate > maxUnits) return $"You can't create any session because exceeds the {maxUnits} units per client and day.";
      return "ok";
    }

    public async Task<string> CheckMaxHoursByClientInSchool(DateTime date, int userId, int clientId, int totalUnits)
    {
      var user = await GetCurrentUser();
      var maxUnits = Convert.ToInt32(_configuration[$"Session:MaxUnitsByClientInSchool"]);
      var allSessionsUnits = await _dbContext.Sessions
                                             .Where(w => w.UserId.Equals(userId) && w.ClientId.Equals(clientId) && w.SessionStart.Date.Equals(date.Date) && w.Pos.Equals(Pos.School))
                                             .Select(s => s.TotalUnits)
                                             .SumAsync();
      var totalUnitsToCreate = allSessionsUnits + totalUnits;
      if (totalUnitsToCreate > maxUnits) return $"You can't create this session because exceeds the {maxUnits} units per client in the school.";
      return "ok";
    }

    public async Task<string> CheckUserSessionOverlap(DateTime dateStart, DateTime dateEnd, int userId)
    {
      var allSessionsOverlapping = await _dbContext.Sessions
                                                   .Where(w => w.UserId.Equals(userId))
                                                   .Where(w => w.SessionStart < dateEnd.ToUniversalTime() && dateStart.ToUniversalTime() < w.SessionEnd)
                                                   .ToListAsync();
      if (allSessionsOverlapping.Any()) return "You cannot overlap sessions";
      return "ok";
    }

    public async Task<string> CheckSessionOverlapSameDayClient(DateTime dateStart, DateTime dateEnd, int clientId)
    {
      var allSessionsOverlapping = await _dbContext.Sessions
                                                   .Where(w => w.ClientId.Equals(clientId))
                                                   .Where(w => w.SessionStart < dateEnd.ToUniversalTime() && dateStart.ToUniversalTime() < w.SessionEnd)
                                                   .ToListAsync();
      if (allSessionsOverlapping.Any()) return "You cannot overlap sessions with same client";
      return "ok";
    }

    public async Task<Client> GetClientById(int clientId)
    {
      return await _dbContext.Clients.FirstOrDefaultAsync(w => w.ClientId.Equals(clientId));
    }

    public async Task<(string Color, string Text)> ChangeSessionStatus(ChangeSessionStatus changeSessionStatus)
    {
      var session = await _dbContext.Sessions.FirstAsync(w => w.SessionId == changeSessionStatus.SessionId);
      session.SessionStatus = changeSessionStatus.SessionStatus;
      await _dbContext.SaveChangesAsync();
      return (changeSessionStatus.SessionStatus.ToString(), ((SessionStatusColors)changeSessionStatus.SessionStatus).ToString());
    }

    public async Task<int> GetUserExpiredDocumentsCount(int userId = 0)
    {
      userId = userId == 0 ? (await GetCurrentUser()).UserId : userId;
      var count = await _dbContext
                        .DocumentsUsers
                        .Where(w => w.Document.DocumentExpires)
                        .Where(w => w.Expires < DateTime.Today)
                        .Where(w => w.UserId == userId)
                        .CountAsync();
      return count;
    }

    public async Task UpdateClientProblemStos(int clientProblemId)
    {
      var clientProblem = await _dbContext.ClientProblems
                                          .Include(i => i.STOs)
                                          .FirstOrDefaultAsync(w => w.ClientProblemId == clientProblemId);
      var period = await GetClientWholePeriod(clientProblem.ClientId);
      var stos = clientProblem.STOs.OrderBy(o => o.ClientProblemStoId).ToList();

      var lastSto = new ClientProblemSto();
      foreach (var s in stos)
      {
        if (s == stos.First()) s.WeekStart = period.start.StartOfWeek(DayOfWeek.Sunday);
        else s.WeekStart = lastSto.WeekEnd.AddDays(1);
        s.WeekEnd = s.WeekStart.AddDays(6).AddWeeks(s.Weeks - 1);
        s.Status = StoStatus.Unknow;
        _dbContext.Update(s);
        await _dbContext.SaveChangesAsync();
        lastSto = s;
      }
    }

    public async Task UpdateClientReplacementStos(int clientReplacementId)
    {
      var clientReplacement = await _dbContext.ClientReplacements
                                                    .Include(i => i.STOs)
                                                    .FirstOrDefaultAsync(w => w.ClientReplacementId == clientReplacementId);
      var period = await GetClientWholePeriod(clientReplacement.ClientId);
      var stos = clientReplacement.STOs.OrderBy(o => o.ClientReplacementStoId).ToList();

      var lastSto = new ClientReplacementSto();
      foreach (var s in stos)
      {
        if (s == stos.First()) s.WeekStart = period.start.StartOfWeek(DayOfWeek.Sunday);
        else s.WeekStart = lastSto.WeekEnd.AddDays(1);
        s.WeekEnd = s.WeekStart.AddDays(6).AddWeeks(s.Weeks - 1);
        s.Status = StoStatus.Unknow;
        _dbContext.Update(s);
        await _dbContext.SaveChangesAsync();
        lastSto = s;
      }
    }

    public async Task<(bool isValid, DateTime start, DateTime end)> GetClientWholePeriod(int clientId)
    {
      var all = await _dbContext.Assessments.Where(s => s.ClientId == clientId).ToListAsync();
      if (!all.Any()) return (false, DateTime.Today, DateTime.Today);
      var start = all.Select(s => s.StartDate).Min();
      var end = all.Select(s => s.EndDate).Max();
      return (true, start, end);
    }

    public async Task<List<ClientProblem>> GetClientBehaviors(int clientId)
    {
      var behaviors = await _dbContext.ClientProblems
                                      .Where(w => w.ClientId == clientId)
                                      .Include(i => i.ProblemBehavior)
                                      .OrderBy(o => o.ProblemBehavior.ProblemBehaviorDescription)
                                      .ToListAsync();
      return behaviors;
    }

    public async Task<List<ClientReplacement>> GetClientReplacements(int clientId)
    {
      var replacements = await _dbContext.ClientReplacements
                                         .Where(w => w.ClientId == clientId)
                                         .Include(i => i.Replacement)
                                         .OrderBy(o => o.Replacement.ReplacementProgramDescription)
                                         .ToListAsync();
      return replacements;
    }

    public async Task NewEntryLog(int sessionId, string title, string description, string icon = "fa-info-circle", string iconColor = "blue")
    {
      var user = await GetCurrentUser();
      var newEntry = new SessionLog
      {
        SessionId = sessionId,
        UserId = user.UserId,
        Title = title,
        Description = description,
        Entry = DateTimeOffset.UtcNow,
        Icon = icon,
        IconColor = iconColor
      };
      await _dbContext.AddAsync(newEntry);
      await _dbContext.SaveChangesAsync();
    }

    public async Task AddSessionProblemNotes(int sessionId, int clientId)
    {
      await Task.Delay(1);
      // var currentProblems = (await GetCurrentPeriodBehaviors(clientId)).Select(s => s.ProblemId).ToList();
      // var currentReplacements = (await GetCurrentPeriodReplacements(clientId)).Select(s => s.ReplacementId).ToList();


      // foreach (var problemId in currentProblems)
      // {
      //   var newSessionProblemNote = new SessionProblemNote
      //   {
      //     ProblemId = problemId,
      //     SessionId = sessionId
      //   };
      //   await _dbContext.SessionProblemNotes.AddAsync(newSessionProblemNote);
      //   await _dbContext.SaveChangesAsync();
      //   foreach (var replacementId in currentReplacements)
      //   {
      //     await _dbContext.SessionProblemNoteReplacements.AddAsync(new SessionProblemNoteReplacement
      //     {
      //       SessionProblemNoteId = newSessionProblemNote.SessionProblemNoteId,
      //       ReplacementId = replacementId
      //     });
      //     await _dbContext.SaveChangesAsync();
      //   }
      // }

    }

    public async Task<Object> GetCompetencyCheckChart(int clientId, CompetencyCheckType competencyCheckType, int userOrCaregiverId)
    {
      var comps = competencyCheckType == CompetencyCheckType.Rbt
        ? _dbContext.CompetencyChecks.Where(w => w.UserId == userOrCaregiverId && w.ClientId == clientId).AsQueryable()
        : _dbContext.CompetencyChecks.Where(w => w.CaregiverId == userOrCaregiverId && w.ClientId == clientId).AsQueryable();

      var competencies = await comps.Include(i => i.User).Include(i => i.Caregiver).ToListAsync();

      if (!competencies.Any()) return null;

      var provider = competencyCheckType == CompetencyCheckType.Caregiver ? competencies.Select(s => $"Caregiver {s.Caregiver.CaregiverFullname}").First() : competencies.Select(s => $"RBT {s.User.Firstname} {s.User.Lastname}").First();
      var firstDate = competencies.Select(s => s.Date).Min();
      var lastDate = competencies.Select(s => s.Date).Max();
      var beginMonth = new DateTime(firstDate.Year, firstDate.Month, 1);
      var lastMonth = new DateTime(lastDate.Year, lastDate.Month, 1).AddMonths(1).AddDays(-1);
      var legend = new List<string>();
      var data = new List<decimal?>();

      while (beginMonth <= lastMonth)
      {
        var lastComp = competencies.Where(s => s.Date.ToString("MM/yy") == beginMonth.ToString("MM/yy")).OrderByDescending(s => s.Date).FirstOrDefault();
        var percent = lastComp?.TotalScore * 100;
        data.Add(percent);
        legend.Add(beginMonth.ToString("MMM/yy"));

        beginMonth = beginMonth.AddMonths(1);
      }

      Object[] dataset = { new OkObjectResult(new { data, name = "progress" }).Value };
      //var dataset = new OkObjectResult(new {data, name = "progress"}).Value;

      var a = new OkObjectResult
      (
        new
        {
          chartOptions = new
          {
            xAxis = new { categories = legend, title = new { text = "Months" }, crosshair = true },
            series = dataset,
            title = new { text = provider },
            chart = new { type = "column" },
            yAxis = new { title = new { text = "Percent" }, max = 100, min = 0 },
            plotOptions = new { column = new { minPointLength = 2 } },
            legend = new { enabled = false }
          }
        }
      ).Value;
      return a;
    }

    public async Task NewSystemLog(SystemLogType logType, Module module, int moduleId, string title, string description)
    {
      var user = await GetCurrentUser();
      var who = await GetFullDataForSystemLog(module, moduleId);
      await _dbContext.AddAsync(new SystemLog
      {
        UserId = user.UserId,
        Title = title,
        Module = module,
        ModuleValue = who,
        Description = description,
        SystemLogType = logType,
        Entry = DateTimeOffset.UtcNow
      });
      await _dbContext.SaveChangesAsync();
    }

    public async Task<string> GetFullDataForSystemLog(Module who, int valueId)
    {
      var name = "";
      switch (who)
      {
        case Module.Client:
          var client = await _dbContext.Clients.FirstOrDefaultAsync(w => w.ClientId == valueId);
          name = $"{client.Firstname} {client.Lastname}";
          break;
        case Module.User:
          var user = await _dbContext.Users.FirstOrDefaultAsync(w => w.UserId == valueId);
          name = $"{user.Firstname} {user.Lastname}";
          break;
        default:
          name = "N/A";
          break;
      }
      return name;
    }

    public async Task<User> GetUserById(int userId)
    {
      var user = await _dbContext.Users
                .Include(i => i.Rol)
                .ThenInclude(t => t.BehaviorAnalysisCode)
                .Include(i => i.UserSign)
                .FirstOrDefaultAsync(w => w.UserId.Equals(userId));
      return user;
    }
  }
}