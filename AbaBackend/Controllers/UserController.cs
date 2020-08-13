using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Collection;
using AbaBackend.Infrastructure.Extensions;
using AbaBackend.Infrastructure.Security;
using AbaBackend.Infrastructure.Utils;
using AbaBackend.Model.MasterTables;
using AbaBackend.Model.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace AbaBackend.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/users")]
  public class UserController : Controller
  {
    private readonly AbaDbContext _dbContext;
    private readonly IUtils _utils;
    private readonly IHostingEnvironment _env;
    readonly ICollection _collection;

    public UserController(AbaDbContext context, IUtils utils, IHostingEnvironment env, ICollection collection)
    {
      _dbContext = context;
      _utils = utils;
      _env = env;
      _collection = collection;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
      try
      {
        var users = await _dbContext.Users
          .Select(u => new
          {
            u.UserId,
            u.Username,
            u.RolId,
            rolname = u.Rol.RolName.ToString(),
            u.Firstname,
            u.Lastname,
            u.Active,
            u.Created,
            u.Email,
            PassesAvailable = u.Passes.Where(w => !w.Used).Count()
          })
          .ToListAsync();
        return Ok(users);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
      try
      {
        var user = await _dbContext.Users
          .Where(w => w.UserId.Equals(id))
          .Select(u => new
          {
            u.UserId,
            u.Username,
            u.RolId,
            rolname = u.Rol.RolName.ToString(),
            u.Firstname,
            u.Lastname,
            u.Active,
            u.Created,
            u.Email,
            u.Npi,
            u.Mpi,
            u.LicenseNo,
            u.SocialSecurity,
            u.Phone,
            u.Address,
            u.Apt,
            u.City,
            u.State,
            u.Zipcode,
            u.BankName,
            u.BankAddress,
            u.BankRoutingNumber,
            u.BankAccountNumber,
            u.PayRate,
            u.DriveTimePayRate,
            u.SessionsDateAllowed
          })
          .FirstOrDefaultAsync();
        if (user == null) return BadRequest("User not found");
        return Ok(user);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-user-full/{id}")]
    public async Task<IActionResult> GetUserFull(int id)
    {
      try
      {
        var user = await _dbContext.Users
          .Where(w => w.UserId.Equals(id))
          .Include(i => i.Rol)
          .Include(i => i.Documents).ThenInclude(i => i.Document)
          .Include(i => i.UserSign)
          .FirstOrDefaultAsync();
        if (user == null) return BadRequest("User not found");
        return Ok(user);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-document-groups")]
    public async Task<IActionResult> GetDocumentGroups()
    {
      try
      {
        var groups = await _dbContext.DocumentGroups
          .Include(i => i.Documents)
          .OrderBy(o => o.DocumentGroupId)
          .ToListAsync();
        return Ok(groups);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("change-status")]
    public async Task<IActionResult> ChangeUserStatus([FromBody] ModChangeUserStatus newStatus)
    {
      try
      {
        var user = await _dbContext.Users.FirstOrDefaultAsync(w => w.UserId.Equals(newStatus.UserId));
        if (user == null) return BadRequest("User not found");
        user.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangeUserPassword([FromBody] ModChangeUserPassword password)
    {
      try
      {
        if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        var user = await _dbContext.Users.FirstOrDefaultAsync(w => w.UserId.Equals(password.userId));
        if (user == null) return BadRequest("User not found");
        var salt = Guid.NewGuid().ToByteArray();
        var hash = new PasswordHasher();
        user.Salt = salt;
        user.Hash = hash.Hash(password.Password, salt);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-edit")]
    public async Task<IActionResult> AddEditUser([FromBody] AddEditUser user)
    {
      using (var transaction = await _dbContext.Database.BeginTransactionAsync())
      {
        try
        {
          if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
          var userProcess = user.UserId == 0 ? new User() : await _dbContext.Users.FirstOrDefaultAsync(w => w.UserId.Equals(user.UserId));
          if (userProcess == null) return BadRequest("User not found");
          if (user.UserId == 0)
          {
            var salt = Guid.NewGuid().ToByteArray();
            var hash = new PasswordHasher();
            userProcess.Salt = salt;
            userProcess.Hash = hash.Hash(user.Password, salt);
            userProcess.Username = user.Username;
            //userProcess.SessionsDateAllowed = DayOfWeekBit.Monday || DayOfWeekBit.Tuesday || DayOfWeekBit;
          }

          userProcess.Email = user.Email;
          userProcess.Firstname = user.Firstname;
          userProcess.Lastname = user.Lastname;
          userProcess.RolId = user.RolId;

          userProcess.Npi = user.Npi;
          userProcess.Mpi = user.Mpi;
          userProcess.LicenseNo = user.LicenseNo;
          userProcess.SocialSecurity = user.SocialSecurity;
          userProcess.Phone = user.Phone;
          userProcess.Address = user.Address;
          userProcess.Apt = user.Apt;
          userProcess.City = user.City;
          userProcess.State = user.State;
          userProcess.Zipcode = user.Zipcode;
          userProcess.BankName = user.BankName;
          userProcess.BankAddress = user.BankAddress;
          userProcess.BankRoutingNumber = user.BankRoutingNumber;
          userProcess.BankAccountNumber = user.BankAccountNumber;
          userProcess.PayRate = user.PayRate;
          userProcess.DriveTimePayRate = user.DriveTimePayRate;

          userProcess.SessionsDateAllowed = user.SessionsDateAllowed;

          if (user.UserId == 0) await _dbContext.Users.AddAsync(userProcess);
          await _dbContext.SaveChangesAsync();

          //if user is new check for documents
          if (user.UserId == 0 && (await _dbContext.Roles.FirstOrDefaultAsync(w => w.RolId.Equals(userProcess.RolId))).HasDocuments)
          {
            var documents = await _dbContext.Documents.OrderBy(o => o.DocumentId).ToListAsync();
            foreach (var document in documents)
              _dbContext.DocumentsUsers.Add(new DocumentUser
              {
                DocumentId = document.DocumentId,
                UserId = userProcess.UserId,
                Active = false,
                Expires = null,
              });
          }

          await _dbContext.SaveChangesAsync();
          transaction.Commit();
          return Ok();
        }
        catch (Exception e)
        {
          return BadRequest(e.InnerException?.Message ?? e.Message);
        }
      }
    }

    [HttpGet("get-roles")]
    public async Task<IActionResult> GetRoles()
    {
      try
      {
        var roles = await _dbContext.Roles.OrderBy(r => r.RolId).ToListAsync();
        return Ok(roles);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-users-can-create-session")]
    public async Task<IActionResult> GetUsersCanCreateSession()
    {
      try
      {
        var users = await _dbContext.Users
          .Where(w => w.Rol.CanCreateSession)
          .Select(u => new
          {
            u.UserId,
            u.Username,
            u.RolId,
            rolname = u.Rol.RolName.ToString(),
            u.Firstname,
            u.Lastname,
            fullname = $"{u.Firstname} {u.Lastname} ({u.Rol.RolName})",
            u.Active,
            u.Created,
            u.Email
          })
          .OrderByDescending(o => o.Active).ThenBy(t => t.fullname)
          .ToListAsync();
        return Ok(users);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetAnalistFromClient(int clientId)
    {
      try
      {
        var analists = await _dbContext.Assignments.Where(w => w.ClientId == clientId && w.User.RolId == 2)
        .Select(u => new
        {
          u.User.UserId,
          u.User.Username,
          u.User.RolId,
          rolname = u.User.Rol.RolName.ToString(),
          u.User.Firstname,
          u.User.Lastname,
          fullname = $"{u.User.Firstname} {u.User.Lastname} ({u.User.Rol.RolName})",
          u.User.Active,
          u.User.Created,
          u.User.Email
        }).ToListAsync();
        return Ok(analists);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("change-user-document-status")]
    public async Task<IActionResult> ChangeUserDocumentStatus([FromBody] ChangeStatus newStatus)
    {
      try
      {
        var document = await _dbContext.DocumentsUsers.FirstOrDefaultAsync(w => w.Id.Equals(newStatus.Id));
        if (document == null) return BadRequest("Document not found");
        document.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("change-user-document-date")]
    public async Task<IActionResult> ChangeUserDocumentDate([FromBody] ChangeDateNull model)
    {
      try
      {
        if (model == null) throw new Exception("Bad date format.");
        var document = await _dbContext.DocumentsUsers.FirstOrDefaultAsync(w => w.Id.Equals(model.Id));
        if (document == null) return BadRequest("Document not found");
        document.Expires = model.Date;
        document.Active = true;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-expiring-documents/{current?}")]
    public async Task<IActionResult> GetExpiringDocuments(bool current = false)
    {
      var currentUser = current ? await _utils.GetCurrentUser() : null;
      try
      {
        var expiring = await _dbContext
          .DocumentsUsers
          .Where(w => w.Document.DocumentExpires)
          .Where(w => EF.Functions.DateDiffDay(DateTime.Today, w.Expires != null ? Convert.ToDateTime(w.Expires).Date : new DateTime(2999, 1, 1).Date) <= 60 && w.Active)
          .Where(w => w.User.Active)
          .Where(w => !current || w.UserId.Equals(currentUser.UserId))
          .Select(s => new
          {
            s.Id,
            s.DocumentId,
            s.UserId,
            UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
            s.User.Rol.RolName,
            s.Document.DocumentName,
            s.Document.DocumentGroup.GroupName,
            s.Expires,
            Days = EF.Functions.DateDiffDay(DateTime.Today, Convert.ToDateTime(s.Expires).Date)
          })
          .OrderBy(o => o.Days).ThenBy(o => o.UserFullname)
          .ToListAsync();
        return Ok(expiring);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("add-missing-documents/{id}")]
    public async Task<IActionResult> AddMissingDocuments(int id)
    {
      try
      {
        var allDocuments = await _dbContext.Documents.Select(s => s.DocumentId).ToListAsync();
        var allUserDocuments = await _dbContext.DocumentsUsers.Where(u => u.UserId.Equals(id)).Select(s => s.DocumentId).ToListAsync();
        var missing = allDocuments.Except(allUserDocuments).ToList();
        if (missing.Count == 0) return Ok(0);
        foreach (var documentId in missing)
        {
          await _dbContext.DocumentsUsers.AddAsync(new DocumentUser
          {
            DocumentId = documentId,
            UserId = id,
            Active = false
          });
        }

        await _dbContext.SaveChangesAsync();
        return Ok(missing.Count);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-clients-for-user/{userId?}")]
    public async Task<IActionResult> GetClientsForUser(int userId = -1)
    {
      try
      {
        var user = userId == -1 ? await _utils.GetCurrentUser() : await _utils.GetUserById(userId);
        var clients = await _dbContext.Assignments
          .Where(w => w.UserId.Equals(user.UserId))
          .Select(s => new
          {
            s.AssignmentId,
            s.Active,
            s.ClientId,
            s.Client.Dob,
            ClientName = $"{s.Client.Firstname} {s.Client.Lastname}",
            ClientCode = s.Client.Code,
            ClientActive = s.Client.Active,
            s.Client.Gender
          })
          .OrderBy(o => o.ClientName)
          .ToListAsync();

        if (user.RolId == 1 || user.RolId == 7)
          clients = await _dbContext.Clients
            .Where(w => w.Active)
            .Select(s => new
            {
              AssignmentId = 0,
              s.Active,
              s.ClientId,
              s.Dob,
              ClientName = $"{s.Firstname} {s.Lastname}",
              ClientCode = s.Code,
              ClientActive = s.Active,
              s.Gender
            })
            .OrderBy(o => o.ClientName)
            .ToListAsync();
        return Ok(clients);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-current-autorizations-for-current-user")]
    public async Task<IActionResult> GetCurrentAuthorizationsForCurrentUser()
    {
      var user = await _utils.GetCurrentUser();
      var today = DateTime.Today;
      var autorizations = await _dbContext.Assessments
        .AsNoTracking()
        .Include(i => i.Client)
        .Where(w => w.Client.Assignments.Any(a => a.UserId == user.UserId && a.Active))
        .Where(w => w.BehaviorAnalysisCodeId.Equals(user.Rol.BehaviorAnalysisCodeId))
        .Where(w => today >= w.StartDate && today <= w.EndDate)
        .ToListAsync();
      var result = autorizations
        .Select(s => new
        {
          s.AssessmentId,
          clientFirstName = s.Client.Firstname,
          clientLastName = s.Client.Lastname,
          clientCode = s.Client.Code,
          s.PaNumber,
          s.StartDate,
          s.EndDate,
          s.TotalUnits,
          AvailableUnits = _utils.GetUnitsAvailable(today, s.ClientId, user).Result
        }).OrderByDescending(s => s.EndDate);
      return Ok(result);
    }

    [HttpGet("get-last7-session-for-current-user")]
    public async Task<IActionResult> GetLast7SessionsForCurrentUser()
    {
      var user = await _utils.GetCurrentUser();
      var today = DateTime.Today;
      var Last7 = today.AddDays(-6);
      var sessions = await _dbContext.Sessions
        .AsNoTracking()
        .Where(w => w.UserId.Equals(user.UserId))
        .Where(w => w.SessionStart.Date >= Last7 && w.SessionStart.Date <= today)
        .Select(s => new
        {
          s.SessionId,
          s.SessionStart,
          s.SessionEnd,
          ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
          CLientCode = s.Client.Code,
          s.TotalUnits,
          SessionType = Enum.GetName(typeof(SessionType), s.SessionType).Replace("_", " "),
          Pos = Enum.GetName(typeof(Pos), s.Pos).Replace("_", " ")
        })
        .OrderByDescending(o => o.SessionStart)
        .ThenBy(o => o.ClientFullname)
        .ToListAsync();
      return Ok(sessions);
    }

    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> GetUserSignature(int userId)
    {
      try
      {
        var sign = await _dbContext.UserSigns.FirstOrDefaultAsync(a => a.UserId == userId);
        return Ok(sign);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveUserSignature([FromBody] UserSign sign)
    {
      try
      {
        _dbContext.Update(sign);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{days}/{showClosed}")]
    public async Task<IActionResult> GetSessionList(int days, bool showClosed)
    {
      var today = DateTime.Today;
      var firstDay = today.AddDays(-days);
      var sessions = await _dbContext.Sessions
        .AsNoTracking()
        .Where(w => days == 0 || w.SessionStart.Date >= firstDay && w.SessionStart.Date <= today)
        .Where(w => showClosed || (w.SessionStatus != SessionStatus.Reviewed && w.SessionStatus != SessionStatus.Billed && w.SessionStatus != SessionStatus.Checked))
        .Select(s => new
        {
          s.SessionId,
          SessionStart = s.SessionStart.ToString("u"),
          SessionEnd = s.SessionEnd.ToString("u"),
          UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
          UserRol = s.User.Rol.RolShortName,
          s.ClientId,
          s.UserId,
          ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
          CLientCode = s.Client.Code,
          s.TotalUnits,
          SessionType = Enum.GetName(typeof(SessionType), s.SessionType).Replace("_", " "),
          Pos = Enum.GetName(typeof(Pos), s.Pos).Replace("_", " "),
          EnumStatus = s.SessionStatus,
          SessionStatus = s.SessionStatus.ToString(),
          SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString()
        })
        .OrderByDescending(o => o.SessionStart)
        .ThenBy(o => o.ClientFullname)
        .ToListAsync();

      var user = await _utils.GetCurrentUser();
      //if (user.Rol.RolShortName == "admin") return Ok(sessions);
      if (user.Rol.RolShortName == "tech") sessions = sessions.Where(s => s.UserId == user.UserId).ToList();
      if (user.Rol.RolShortName == "assistant")
      {
        var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId).Select(s => s.ClientId).ToListAsync();
        sessions = sessions.Where(s => myClients.Contains(s.ClientId) && s.UserRol != "analyst").ToList();
      }

      if (user.Rol.RolShortName == "analyst")
      {
        var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId && w.Active).Select(s => s.ClientId).ToListAsync();
        sessions = sessions.Where(s => myClients.Contains(s.ClientId)).ToList();
      }

      return Ok(sessions);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetSessionReadyToReview()
    {
      var sessions = await _dbContext.Sessions
        .AsNoTracking()
        .Where(w => w.SessionStatus == SessionStatus.Checked)
        .Select(s => new
        {
          s.SessionId,
          SessionStart = s.SessionStart.ToString("u"),
          SessionEnd = s.SessionEnd.ToString("u"),
          UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
          UserRol = s.User.Rol.RolShortName,
          s.ClientId,
          s.UserId,
          ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
          CLientCode = s.Client.Code,
          s.TotalUnits,
          SessionType = Enum.GetName(typeof(SessionType), s.SessionType).Replace("_", " "),
          Pos = Enum.GetName(typeof(Pos), s.Pos).Replace("_", " "),
          EnumStatus = s.SessionStatus,
          SessionStatus = s.SessionStatus.ToString(),
          SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString()
        })
        .OrderByDescending(o => o.SessionStart)
        .ThenBy(o => o.ClientFullname)
        .ToListAsync();

      var user = await _utils.GetCurrentUser();
      //if (user.Rol.RolShortName == "admin") return Ok(sessions);
      if (user.Rol.RolShortName == "tech") sessions = sessions.Where(s => s.UserId == user.UserId).ToList();
      if (user.Rol.RolShortName == "assistant")
      {
        var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId).Select(s => s.ClientId).ToListAsync();
        sessions = sessions.Where(s => myClients.Contains(s.ClientId) && s.UserRol != "analyst").ToList();
      }

      if (user.Rol.RolShortName == "analyst")
      {
        var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId && w.Active).Select(s => s.ClientId).ToListAsync();
        sessions = sessions.Where(s => myClients.Contains(s.ClientId)).ToList();
      }

      return Ok(sessions);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetSessionListClosedLw()
    {
      var today = DateTime.Today;
      var firstDay = today.StartOfWeek(DayOfWeek.Sunday).AddDays(-7);
      var lastDay = firstDay.AddDays(6);
      var sessions = await _dbContext.Sessions
        .AsNoTracking()
        //  .Where(w => w.SessionStart.Date >= firstDay && w.SessionStart.Date <= lastDay)
        .Where(w => w.SessionStatus == SessionStatus.Reviewed || w.SessionStatus == SessionStatus.Billed)
        .Select(s => new
        {
          s.SessionId,
          SessionStart = s.SessionStart.ToString("u"),
          SessionEnd = s.SessionEnd.ToString("u"),
          UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
          UserRol = s.User.Rol.RolShortName,
          s.ClientId,
          s.UserId,
          ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
          CLientCode = s.Client.Code,
          s.TotalUnits,
          SessionType = Enum.GetName(typeof(SessionType), s.SessionType).Replace("_", " "),
          Pos = Enum.GetName(typeof(Pos), s.Pos).Replace("_", " "),
          EnumStatus = s.SessionStatus,
          SessionStatus = s.SessionStatus.ToString(),
          SessionStatusColor = ((SessionStatusColors)s.SessionStatus).ToString()
        })
        .OrderByDescending(o => o.SessionStart)
        .ThenBy(o => o.ClientFullname)
        .ToListAsync();

      var user = await _utils.GetCurrentUser();
      //if (user.Rol.RolShortName == "admin") return Ok(sessions);
      if (user.Rol.RolShortName == "tech") sessions = sessions.Where(s => s.UserId == user.UserId).ToList();
      if (user.Rol.RolShortName == "assistant")
      {
        var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId).Select(s => s.ClientId).ToListAsync();
        sessions = sessions.Where(s => myClients.Contains(s.ClientId) && s.UserRol != "analyst").ToList();
      }

      if (user.Rol.RolShortName == "analyst")
      {
        var myClients = await _dbContext.Assignments.Where(w => w.UserId == user.UserId && w.Active).Select(s => s.ClientId).ToListAsync();
        sessions = sessions.Where(s => myClients.Contains(s.ClientId)).ToList();
      }

      return Ok(sessions);
    }

    [HttpDelete("[action]/{userId}")]
    public async Task<IActionResult> DeleteDocuments(int userId)
    {
      try
      {
        var entries = await _dbContext.DocumentsUsers.Where(w => w.UserId == userId).ToListAsync();
        _dbContext.RemoveRange(entries);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> addEditDocumentGroup([FromBody] DocumentGroup group)
    {
      try
      {
        _dbContext.Update(group);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> DeleteDocument([FromBody] Document document)
    {
      try
      {
        _dbContext.Remove(document);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AddEditDocument([FromBody] Document document)
    {
      try
      {
        _dbContext.Update(document);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> DeleteDocumentGroup([FromBody] DocumentGroup group)
    {
      try
      {
        _dbContext.Remove(group);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]/{documentUserId}")]
    public async Task<IActionResult> UploadDocumentPdf([FromRoute] int documentUserId, [FromForm] IFormFile body)
    {
      try
      {
        var path = Path.Combine(_env.WebRootPath, "UserDocuments", documentUserId.ToString());
        var fullFilename = Path.Combine(path, body.FileName);
        Directory.CreateDirectory(path);

        if (System.IO.File.Exists(fullFilename)) throw new Exception("File already exists");

        using (var fileStream = new FileStream(fullFilename, FileMode.Create))
          await body.CopyToAsync(fileStream);

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{documentUserId}")]
    public IActionResult GetUserPdfs(int documentUserId)
    {
      try
      {
        var path = Path.Combine(_env.WebRootPath, "UserDocuments", documentUserId.ToString());
        if (!Directory.Exists(path)) return Ok();
        var di = new DirectoryInfo(path);
        var files = di.GetFiles("*.pdf");
        var f = files.Select(s => new
        {
          s.Name,
          s.Length,
          s.CreationTime
        }).OrderByDescending(o => o.CreationTime).ToList();
        return Ok(f);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public IActionResult DeletePdf([FromBody] ClassIntString model)
    {
      try
      {
        var path = Path.Combine(_env.WebRootPath, "UserDocuments", model.Id.ToString(), model.Value);
        System.IO.File.Delete(path);
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetClientMonthlyNotes(int clientId)
    {
      try
      {
        var notes = await _dbContext.MonthlyNotes
          .Where(w => w.ClientId == clientId)
          .OrderByDescending(o => o.Year).ThenByDescending(o => o.Month)
          .Select(s => new
          {
            value = s.MonthlyNoteId,
            text = s.MonthlyNoteDate.ToString("Y"),
            date = s.MonthlyNoteDate
          })
          .ToListAsync();
        return Ok(notes);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{monthlyNoteId}")]
    public async Task<IActionResult> GetClientMonthlyNote(int monthlyNoteId)
    {
      try
      {
        var note = await _dbContext.MonthlyNotes
          .Include(i => i.MonthlyAnalyst)
          .Include(i => i.MonthlyAssistant)
          .Include(i => i.MonthlyRbt)
          .FirstOrDefaultAsync(w => w.MonthlyNoteId == monthlyNoteId);
        var client = await _dbContext.Clients
          .Where(f => f.ClientId == note.ClientId)
          .Select(s => new
          {
            s.Dob,
            Code = s.Code ?? "N/A",
            s.MemberNo,
            s.Firstname,
            s.Lastname,
            Assignments = s.Assignments.Where(w => w.Active).Select(q => q.User)
          }).FirstOrDefaultAsync();
        return Ok(new
        {
          note,
          client
        });
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{monthlyNoteId}")]
    public async Task<IActionResult> GetClientMonthlyData(int monthlyNoteId)
    {
      try
      {
        var note = await _dbContext.MonthlyNotes.FirstOrDefaultAsync(w => w.MonthlyNoteId == monthlyNoteId);
        var month = new DateTime(note.Year, note.Month, 1);
        var dataBeh = await _collection.GetMonthlyDataBehavior(note.ClientId, month);
        var dataRpl = await _collection.GetMonthlyDataReplacement(note.ClientId, month);
        return Ok(new
        {
          behaviors = dataBeh,
          replacements = dataRpl
        });
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> GrantPass(int userId)
    {
      try
      {
        var pass = new AuthPass
        {
          Created = DateTime.Now,
          UserId = userId,
        };
        await _dbContext.AddAsync(pass);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> RevokePass(int userId)
    {
      try
      {
        var pass = await _dbContext.AuthPasses.Where(w => w.UserId == userId && w.Used == false).FirstOrDefaultAsync();
        if (pass == null) return Ok();
        _dbContext.Remove(pass);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{userId}/{value}")]
    public async Task<IActionResult> ChangeDayOfWeekBit(int userId, int value)
    {
      try
      {
        var user = await _utils.GetUserById(userId);
        if (user == null) throw new Exception("User not found");
        user.SessionsDateAllowed = (DayOfWeekBit)value;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{userId}")]
    public async Task<IActionResult> GetSign(int userId)
    {
      try
      {
        var user = await _utils.GetUserById(userId);
        return Ok(user);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
  }
}