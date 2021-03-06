using System;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.StoProcess;
using AbaBackend.Infrastructure.Utils;
using AbaBackend.Model;
using AbaBackend.Model.Client;
using AbaBackend.Model.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbaBackend.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/clients")]
  public class ClientController : Controller
  {
    private readonly AbaDbContext _dbContext;
    private readonly IUtils _utils;
    private readonly IStoProcess _stoProcess;

    public ClientController(AbaDbContext context, IUtils utils, IStoProcess stoProcess)
    {
      _dbContext = context;
      _utils = utils;
      _stoProcess = stoProcess;
    }

    [HttpGet]
    public async Task<IActionResult> GetClients()
    {
      try
      {
        var clients = await _dbContext.Clients
        .AsNoTracking()
        .Select(s => new
        {
          s.ClientId,
          s.Code,
          s.Firstname,
          s.Lastname,
          s.Nickname,
          s.Dob,
          s.Phone,
          s.Email,
          s.Address,
          s.Apt,
          s.City,
          s.State,
          s.Zipcode,
          s.Gender,
          s.Race,
          s.PrimaryLanguage,
          s.EmergencyContact,
          s.EmergencyPhone,
          s.EmergencyEmail,
          s.Notes,
          s.SocialSecurity,
          s.Insurance,
          s.MemberNo,
          s.MmaPlan,
          s.MmaIdNo,
          s.Active,
          s.Created,
          s.Modified
        })
        .OrderBy(o => o.Firstname)
        .ThenBy(o => o.Lastname)
        .ToListAsync();
        return Ok(clients);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetClient(int id)
    {
      try
      {
        var client = await _dbContext.Clients
                                     .Include(i => i.Caregivers)
                                     .ThenInclude(i => i.CaregiverType)
                                     .Include(i => i.Referrals)
                                     .Include(i => i.ClientDiagnostics)
                                     .ThenInclude(i => i.Diagnosis)
                                     .Include(i => i.Assignments).ThenInclude(a => a.User)
                                     .FirstOrDefaultAsync(w => w.ClientId.Equals(id));
        if (client == null) return BadRequest("Client not found");
        client.Referrals = client.Referrals.OrderByDescending(o => o.DateReferral).ToList();
        return Ok(client);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("change-status")]
    public async Task<IActionResult> ChangeClientStatus([FromBody] ModChangeUserStatus newStatus)
    {
      try
      {
        var client = await _dbContext.Clients.FirstOrDefaultAsync(w => w.ClientId.Equals(newStatus.UserId));
        if (client == null) return BadRequest("Client not found");
        client.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, newStatus.UserId, "Status changed", $"Client changed status to {newStatus.Status}");
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-edit")]
    public async Task<IActionResult> AddEditClient([FromBody] Client client)
    {
      try
      {
        //if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        var client2Process = client.ClientId == 0 ? new Client() : await _dbContext.Clients.FirstOrDefaultAsync(w => w.ClientId.Equals(client.ClientId));
        if (client2Process == null) return BadRequest("Client not found");

        client2Process.Code = client.Code;
        client2Process.Firstname = client.Firstname;
        client2Process.Lastname = client.Lastname;
        client2Process.Nickname = client.Nickname;
        client2Process.Dob = client.Dob;
        client2Process.Phone = client.Phone;
        client2Process.Email = client.Email;
        client2Process.Address = client.Address;
        client2Process.Apt = client.Apt;
        client2Process.City = client.City;
        client2Process.State = client.State;
        client2Process.Zipcode = client.Zipcode;
        client2Process.Gender = client.Gender;
        client2Process.Race = client.Race;
        client2Process.PrimaryLanguage = client.PrimaryLanguage;
        client2Process.EmergencyContact = client.EmergencyContact;
        client2Process.EmergencyEmail = client.EmergencyEmail;
        client2Process.EmergencyPhone = client.EmergencyPhone;
        client2Process.EmergencyPhone = client.EmergencyPhone;
        client2Process.Notes = client.Notes;
        client2Process.SocialSecurity = client.SocialSecurity;
        client2Process.Insurance = client.Insurance;
        client2Process.MemberNo = client.MemberNo;
        client2Process.MmaPlan = client.MmaPlan;
        client2Process.MmaIdNo = client.MmaIdNo;
        client2Process.Modified = DateTime.Now;

        if (client.ClientId == 0) await _dbContext.Clients.AddAsync(client2Process);
        await _dbContext.SaveChangesAsync();
        if (client.ClientId == 0) await _utils.NewSystemLog(SystemLogType.Info, Module.Client, client2Process.ClientId, "Created", $"Client created");
        else await _utils.NewSystemLog(SystemLogType.Info, Module.Client, client2Process.ClientId, "Edited", $"Client edited");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    // [HttpGet("get-problems/{id}")]
    // public async Task<IActionResult> GetClientProblems(int id)
    // {
    //   try
    //   {
    //     var problemsIncluded = await _dbContext.ClientsProblems
    //                                            .Where(w => w.ClientId.Equals(id) && w.ProblemBehavior.Active)
    //                                            .Select(s => new
    //                                            {
    //                                              s.ProblemBehavior.ProblemId,
    //                                              s.ProblemBehavior.ProblemBehaviorDescription,
    //                                              BaselineFrom = s.BaselineFrom == null ? null : Convert.ToDateTime(s.BaselineFrom).ToString("MM/dd/yyyy"),
    //                                              BaselineTo = s.BaselineTo == null ? null : Convert.ToDateTime(s.BaselineTo).ToString("MM/dd/yyyy"),
    //                                              s.BaselineCount
    //                                            })
    //                                            .OrderBy(o => o.ProblemBehaviorDescription)
    //                                            .ToListAsync();

    //     var idList = problemsIncluded.Select(s => s.ProblemId).ToList();

    //     var problemsExcluded = await _dbContext.ProblemBehaviors
    //                                            .Where(w => !idList.Contains(w.ProblemId) & w.Active)
    //                                            .Select(s => new { s.ProblemId, s.ProblemBehaviorDescription })
    //                                            .OrderBy(o => o.ProblemBehaviorDescription)
    //                                            .ToListAsync();
    //     return Ok(new { problemsIncluded, problemsExcluded });
    //   }
    //   catch (Exception e)
    //   {
    //     return BadRequest(e.Message);
    //   }
    // }

    // [HttpPost("edit-client-problem")]
    // public async Task<IActionResult> EditClientProblem([FromBody] ClientProblem model)
    // {
    //   try
    //   {
    //     var lookup = await _dbContext.ClientsProblems.FirstOrDefaultAsync(s => s.ClientId == model.ClientId && s.ProblemId == model.ProblemId);
    //     if (lookup == null) throw new Exception("Item not found");
    //     lookup.BaselineCount = model.BaselineCount;
    //     lookup.BaselineFrom = model.BaselineFrom;
    //     lookup.BaselineTo = model.BaselineTo;
    //     await _dbContext.SaveChangesAsync();
    //     return Ok();
    //   }
    //   catch (Exception e)
    //   {
    //     return BadRequest(e.InnerException?.Message ?? e.Message);
    //   }
    // }

    // [HttpPost("add-remove-client-problem")]
    // public async Task<IActionResult> AddRemoveClientProblem([FromBody] AddRemoveClientProblem model)
    // {
    //   try
    //   {
    //     if (model.Action == 'A') await _dbContext.ClientsProblems.AddAsync(new ClientProblem { ClientId = model.ClientId, ProblemId = model.ProblemId });
    //     else if (model.Action == 'D')
    //     {
    //       var toRemove = await _dbContext.ClientsProblems.FirstOrDefaultAsync(w => w.ClientId.Equals(model.ClientId) && w.ProblemId.Equals(model.ProblemId));
    //       _dbContext.ClientsProblems.Remove(toRemove);
    //     }

    //     await _dbContext.SaveChangesAsync();
    //     return Ok();
    //   }
    //   catch (Exception e)
    //   {
    //     return BadRequest(e.InnerException?.Message ?? e.Message);
    //   }
    // }

    // [HttpGet("get-replacements/{id}")]
    // public async Task<IActionResult> GetClientReplacements(int id)
    // {
    //   try
    //   {
    //     var replacementsIncluded = await _dbContext.ClientsReplacements
    //                                                .Where(w => w.ClientId.Equals(id) && w.ReplacementProgram.Active)
    //                                                .Select(s => new
    //                                                {
    //                                                  s.ReplacementProgram.ReplacementId,
    //                                                  s.ReplacementProgram.ReplacementProgramDescription,
    //                                                  BaselineFrom = s.BaselineFrom == null ? null : Convert.ToDateTime(s.BaselineFrom).ToString("MM/dd/yyyy"),
    //                                                  BaselineTo = s.BaselineTo == null ? null : Convert.ToDateTime(s.BaselineTo).ToString("MM/dd/yyyy"),
    //                                                  s.BaselinePercent
    //                                                })
    //                                                .OrderBy(o => o.ReplacementProgramDescription)
    //                                                .ToListAsync();

    //     var idList = replacementsIncluded.Select(s => s.ReplacementId).ToList();

    //     var replacementsExcluded = await _dbContext.ReplacementPrograms
    //                                                .Where(w => !idList.Contains(w.ReplacementId) & w.Active)
    //                                                .Select(s => new { s.ReplacementId, s.ReplacementProgramDescription })
    //                                                .OrderBy(o => o.ReplacementProgramDescription)
    //                                                .ToListAsync();
    //     return Ok(new { replacementsIncluded, replacementsExcluded });
    //   }
    //   catch (Exception e)
    //   {
    //     return BadRequest(e.Message);
    //   }
    // }

    // [HttpPost("edit-client-replacement")]
    // public async Task<IActionResult> EditClientReplacement([FromBody] ClientReplacement model)
    // {
    //   try
    //   {
    //     var lookup = await _dbContext.ClientsReplacements.FirstOrDefaultAsync(s => s.ClientId == model.ClientId && s.ReplacementId == model.ReplacementId);
    //     if (lookup == null) throw new Exception("Item not found");
    //     lookup.BaselinePercent = model.BaselinePercent;
    //     lookup.BaselineFrom = model.BaselineFrom;
    //     lookup.BaselineTo = model.BaselineTo;
    //     await _dbContext.SaveChangesAsync();
    //     return Ok();
    //   }
    //   catch (Exception e)
    //   {
    //     return BadRequest(e.InnerException?.Message ?? e.Message);
    //   }
    // }

    // [HttpPost("add-remove-client-replacement")]
    // public async Task<IActionResult> AddRemoveClientReplacement([FromBody] AddRemoveClientReplacement model)
    // {
    //   try
    //   {
    //     if (model.Action == 'A') await _dbContext.ClientsReplacements.AddAsync(new ClientReplacement { ClientId = model.ClientId, ReplacementId = model.ReplacementId });
    //     else if (model.Action == 'D')
    //     {
    //       var toRemove = await _dbContext.ClientsReplacements.FirstOrDefaultAsync(w => w.ClientId.Equals(model.ClientId) && w.ReplacementId.Equals(model.ReplacementId));
    //       _dbContext.ClientsReplacements.Remove(toRemove);
    //     }

    //     await _dbContext.SaveChangesAsync();
    //     return Ok();
    //   }
    //   catch (Exception e)
    //   {
    //     return BadRequest(e.InnerException?.Message ?? e.Message);
    //   }
    // }

    [HttpGet("get-caregivers-types")]
    public async Task<IActionResult> GetCaregiversTypes()
    {
      try
      {
        var types = await _dbContext.CaregiversType.OrderBy(o => o.Description).ToListAsync();
        return Ok(types);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-edit-caregiver")]
    public async Task<IActionResult> AddEditCaregiver([FromBody] Caregiver caregiver)
    {
      try
      {
        if (!ModelState.IsValid) throw new Exception(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        if (caregiver.CaregiverTypeId == 0) throw new Exception("Please select caregiver type.");
        if (caregiver.ClientId == 0) throw new Exception("Wrong client.");
        var caregiver2Process = caregiver.CaregiverId == 0 ? new Caregiver() : await _dbContext.Caregivers.FirstOrDefaultAsync(w => w.CaregiverId.Equals(caregiver.CaregiverId));
        if (caregiver2Process == null) return BadRequest("Caregiver not found.");

        caregiver2Process.CaregiverFullname = caregiver.CaregiverFullname;
        caregiver2Process.Phone = caregiver.Phone;
        caregiver2Process.Email = caregiver.Email;
        caregiver2Process.ClientId = caregiver.ClientId;
        caregiver2Process.CaregiverTypeId = caregiver.CaregiverTypeId;

        if (caregiver.CaregiverId == 0) await _dbContext.Caregivers.AddAsync(caregiver2Process);
        if (caregiver.CaregiverId == 0) await _utils.NewSystemLog(SystemLogType.Info, Module.Client, caregiver2Process.ClientId, "Created caregiver", $"A caregiver was created");
        else await _utils.NewSystemLog(SystemLogType.Info, Module.Client, caregiver2Process.ClientId, "Edited caregiver", $"A caregiver was edited");

        await _dbContext.SaveChangesAsync();
        return Ok(caregiver2Process.CaregiverId);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("delete-caregiver")]
    public async Task<IActionResult> DeleteCaregiver([FromBody] Caregiver caregiver)
    {
      try
      {
        var caregiver2Process = await _dbContext.Caregivers.FirstOrDefaultAsync(w => w.CaregiverId.Equals(caregiver.CaregiverId));
        if (caregiver2Process == null) return BadRequest("Caregiver not found.");
        _dbContext.Caregivers.Remove(caregiver2Process);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, caregiver2Process.ClientId, "Caregiver deleted", $"A caregiver was deleted");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("add-edit-referral")]
    public async Task<IActionResult> AddEditReferral([FromBody] Referral referral)
    {
      try
      {
        if (!ModelState.IsValid) throw new Exception(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        if (referral.ClientId == 0) throw new Exception("Wrong client.");
        var referral2Process = referral.ReferralId == 0 ? new Referral() : await _dbContext.Referrals.FirstOrDefaultAsync(w => w.ReferralId.Equals(referral.ReferralId));
        if (referral2Process == null) return BadRequest("Referral not found.");

        referral2Process.ClientId = referral.ClientId;
        referral2Process.ReferralFullname = referral.ReferralFullname;
        referral2Process.Specialty = referral.Specialty;
        referral2Process.License = referral.License;
        referral2Process.Provider = referral.Provider;
        referral2Process.Npi = referral.Npi;
        referral2Process.FullAddress = referral.FullAddress;
        referral2Process.Phone = referral.Phone;
        referral2Process.Fax = referral.Fax;
        referral2Process.Email = referral.Email;
        referral2Process.DateReferral = referral.DateReferral;
        referral2Process.DateExpires = referral.DateExpires;

        if (referral.ReferralId == 0) await _dbContext.Referrals.AddAsync(referral2Process);
        if (referral.ReferralId == 0) await _utils.NewSystemLog(SystemLogType.Info, Module.Client, referral2Process.ClientId, "Created referral", $"A referral was created");
        else await _utils.NewSystemLog(SystemLogType.Info, Module.Client, referral2Process.ClientId, "Edited referral", $"A referral was edited");
        await _dbContext.SaveChangesAsync();

        return Ok(referral2Process);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("delete-referral")]
    public async Task<IActionResult> DeleteReferral([FromBody] Referral referral)
    {
      try
      {
        var referral2Process = await _dbContext.Referrals.FirstOrDefaultAsync(w => w.ReferralId.Equals(referral.ReferralId));
        if (referral2Process == null) return BadRequest("Referral not found.");
        _dbContext.Referrals.Remove(referral2Process);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, referral2Process.ClientId, "Deleted referral", $"A referral was deleted");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("change-referral-status")]
    public async Task<IActionResult> ChangeReferralStatus([FromBody] ChangeReferralStatusModel newStatus)
    {
      try
      {
        var referral = await _dbContext.Referrals.FirstOrDefaultAsync(w => w.ReferralId.Equals(newStatus.ReferralId)); //generic model UserId = Id
        if (referral == null) return BadRequest("Referral not found");
        referral.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, referral.ClientId, "Referral status", $"A referral status was changed to {newStatus.Status}");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-assignment")]
    public async Task<IActionResult> AddAssigment([FromBody] Assignment assignment)
    {
      try
      {
        if (!ModelState.IsValid) throw new Exception(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        if (_dbContext.Assignments.Any(w => w.ClientId.Equals(assignment.ClientId) && w.UserId.Equals(assignment.UserId))) throw new Exception("User already assigned.");
        var assignment2Process = new Assignment
        {
          Active = true,
          ClientId = assignment.ClientId,
          UserId = assignment.UserId,
        };
        await _dbContext.Assignments.AddAsync(assignment2Process);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, assignment.ClientId, "Staff assigned", $"A user was assigned to client");

        return Ok(assignment2Process);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-assignments/{id}")]
    public async Task<IActionResult> GetAssignmentsByClient(int id)
    {
      try
      {
        var assignments = await _dbContext.Assignments
                                          .Where(s => s.ClientId.Equals(id))
                                          .Include(i => i.User)
                                          .ThenInclude(i => i.Rol)
                                          .OrderBy(o => o.AssignmentId)
                                          .ToListAsync();
        return Ok(assignments);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("delete-assignment")]
    public async Task<IActionResult> DeleteAssignment([FromBody] Assignment assignment)
    {
      try
      {
        var assignment2Process = await _dbContext.Assignments.FirstOrDefaultAsync(w => w.AssignmentId.Equals(assignment.AssignmentId));
        if (assignment2Process == null) return BadRequest("Assignment not found.");
        _dbContext.Assignments.Remove(assignment2Process);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, assignment.ClientId, "Staff removed", $"A user was removed for the client");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("change-assignment-status")]
    public async Task<IActionResult> ChangeAssignmentStatus([FromBody] ModChangeUserStatus newStatus)
    {
      try
      {
        var assignment = await _dbContext.Assignments.FirstOrDefaultAsync(w => w.AssignmentId.Equals(newStatus.UserId)); //generic model UserId = Id
        if (assignment == null) return BadRequest("Assignment not found");
        assignment.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, assignment.ClientId, "Staff status", $"A user assigned status was changed to {newStatus.Status}");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("change-diagnosis-status")]
    public async Task<IActionResult> ChangeDiagnosisStatus([FromBody] ModChangeUserStatus newStatus)
    {
      try
      {
        var diagnosis = await _dbContext.ClientDiagnostics.FirstOrDefaultAsync(w => w.ClientDiagnosisId.Equals(newStatus.UserId)); //generic model UserId = Id
        if (diagnosis == null) return BadRequest("Diagnosis not found");
        diagnosis.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, diagnosis.ClientId, "Diagnosis status", $"A client diagnosis status was changed to {newStatus.Status}");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-client-diagnosis")]
    public async Task<IActionResult> AddClientDiagnosis([FromBody] AddClientDiagnosisModel model)
    {
      try
      {
        var refer = await _dbContext.Diagnostics.FirstOrDefaultAsync(w => w.Code.ToLower().Equals(model.Code.ToLower()));
        if (refer == null) throw new Exception("Diagnosis code not found");
        var diag = new ClientDiagnosis
        {
          ClientId = model.ClientId,
          DiagnosisId = refer.DiagnosisId,
          StartDate = model.StartDate,
          EndDate = model.EndDate,
          AddedDate = DateTime.Now,
          IsMain = false
        };
        await _dbContext.AddAsync(diag);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, model.ClientId, "Diagnostic added", $"A diagnostic was added ({refer.Code})");

        return Ok(diag);
      }
      catch (DbUpdateException)
      {
        return BadRequest("Error adding diagnosis.");
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangeMainDiagnosis([FromBody] AddClientDiagnosisModel model)
    {
      try
      {
        var allClientDiagnosis = await _dbContext.ClientDiagnostics.Where(w => w.ClientId == model.ClientId).ToListAsync();
        allClientDiagnosis.ForEach(f => f.IsMain = false);
        var active = allClientDiagnosis.FirstOrDefault(w => w.ClientDiagnosisId == model.ClientDiagnosisId);
        active.IsMain = true;
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, model.ClientId, "Diagnostic edited", $"A client diagnostic was was changed to main");

        return Ok();
      }
      catch (DbUpdateException)
      {
        return BadRequest("Error updating diagnosis");
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("client-delete-diagnosis")]
    public async Task<IActionResult> ClienteDeleteDiagnosis([FromBody] ClientDiagnosis clientDiagnosis)
    {
      try
      {
        var clientDiagnosis2Process = await _dbContext.ClientDiagnostics.FirstOrDefaultAsync(w => w.ClientDiagnosisId.Equals(clientDiagnosis.ClientDiagnosisId));
        if (clientDiagnosis2Process == null) return BadRequest("Diagnosis in the client not found.");
        _dbContext.ClientDiagnostics.Remove(clientDiagnosis2Process);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, clientDiagnosis2Process.ClientId, "Diagnostic deleted", $"A diagnostic was deleted");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-clients-expiring-referrals")]
    public async Task<IActionResult> GetClientsExpiringReferrals()
    {
      try
      {
        var referrals = await _dbContext.Referrals
                                        .Where(w => EF.Functions.DateDiffDay(DateTime.Today, w.DateExpires.Date) <= 90)
                                        .Where(w => w.Client.Active)
                                        .Where(w => w.Active)
                                        .Select(s => new
                                        {
                                          s.ClientId,
                                          s.ReferralId,
                                          ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
                                          s.Client.Code,
                                          s.ReferralFullname,
                                          s.Specialty,
                                          s.DateReferral,
                                          s.DateExpires,
                                          Days = EF.Functions.DateDiffDay(DateTime.Today, s.DateExpires.Date)
                                        })
                                        .OrderBy(o => o.Days)
                                        .ToListAsync();
        return Ok(referrals);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-assessment")]
    public async Task<IActionResult> AddAssessment([FromBody] Assessment assessment)
    {
      try
      {
        if (!ModelState.IsValid) throw new Exception(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        if (assessment.StartDate >= assessment.EndDate) throw new Exception("Wrong dates.");
        var newAssessment = new Assessment
        {
          AssessmentId = assessment.AssessmentId,
          ClientId = assessment.ClientId,
          BehaviorAnalysisCodeId = assessment.BehaviorAnalysisCodeId,
          PaNumber = assessment.PaNumber,
          TotalUnits = assessment.TotalUnits,
          StartDate = assessment.StartDate,
          EndDate = assessment.EndDate,
          TotalUnitsWeek = assessment.TotalUnitsWeek
        };
        _dbContext.Assessments.Update(newAssessment);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, newAssessment.ClientId, "Auth added", $"A authorization was added ({assessment.TotalUnits} units)");

        return Ok(newAssessment);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-assessments/{id}")]
    public async Task<IActionResult> GetAssessmentsByClient(int id)
    {
      try
      {
        var assessments = await _dbContext.Assessments
                                          .Where(s => s.ClientId.Equals(id))
                                          .Include(i => i.BehaviorAnalysisCode)
                                          .OrderByDescending(o => o.EndDate)
                                          .ToListAsync();
        return Ok(assessments);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("delete-assessment")]
    public async Task<IActionResult> DeleteAsssessment([FromBody] Assessment assessment)
    {
      try
      {
        var assessment2Delete = await _dbContext.Assessments.FirstOrDefaultAsync(w => w.AssessmentId.Equals(assessment.AssessmentId));
        if (assessment2Delete == null) return BadRequest("Authorization not found.");
        _dbContext.Assessments.Remove(assessment2Delete);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, assessment.ClientId, "Auth deleted", $"A authorization was deleted");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-clients-expiring-assessments")]
    public async Task<IActionResult> GetClientsExpiringAssessment()
    {
      try
      {
        var topAssessments = await _dbContext.Assessments
                                   .Where(w => w.Client.Active)
                                   .GroupBy(x => new { x.BehaviorAnalysisCodeId, x.ClientId })
                                   .Select(s => s.OrderByDescending(o => o.StartDate).First())
                                   .Select(s => s.AssessmentId)
                                  .ToListAsync();

        var assignments = await _dbContext.Assessments
                                          .Where(w => topAssessments.Contains(w.AssessmentId))
                                          .Where(w => (EF.Functions.DateDiffDay(DateTime.Today, w.EndDate.Date) <= 60 || w.TotalUnits <= 10) && w.BehaviorAnalysisCode.Checkable)
                                          .Where(w => w.Client.Active)
                                          .Select(s => new
                                          {
                                            s.ClientId,
                                            ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
                                            s.Client.Code,
                                            s.TotalUnits,
                                            s.StartDate,
                                            s.EndDate,
                                            Days = EF.Functions.DateDiffDay(DateTime.Today, s.EndDate.Date),
                                            s.BehaviorAnalysisCode
                                          })
                                          .OrderBy(o => o.Days)
                                          .ToListAsync();
        return Ok(assignments);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-client-caregivers/{clientId}")]
    public async Task<IActionResult> GetClientCaregivers(int clientId)
    {
      try
      {
        var caregivers = await _dbContext
                               .Caregivers
                               .Where(w => w.ClientId.Equals(clientId))
                               .Select(s => new
                               {
                                 text = s.CaregiverFullname,
                                 value = s.CaregiverId
                               }).ToListAsync();
        return Ok(caregivers);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-client-users/{clientId}/{rol?}")]
    public async Task<IActionResult> GetClientUsers(int clientId, string rol = "")
    {
      try
      {
        var rbt = await _dbContext
                  .Assignments
                  .Where(w => w.ClientId.Equals(clientId))
                  .Where(w => rol == "" || w.User.Rol.RolShortName.Equals(rol))
                  .Select(s => new
                  {
                    text = $"{s.User.Firstname} {s.User.Lastname}",
                    value = s.UserId
                  }).ToListAsync();
        return Ok(rbt);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveProblem([FromBody] ClientProblem problem)
    {
      try
      {
        _dbContext.Update(problem);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, problem.ClientId, "Behavior", $"Behavior added or updated");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetClientProblems(int clientId)
    {
      try
      {
        var periods = await _dbContext
                      .ClientProblems
                      .Include(i => i.ProblemBehavior)
                      .Include(i => i.STOs)
                      .Where(w => w.ClientId == clientId)
                      .OrderBy(o => o.ProblemBehavior.ProblemBehaviorDescription)
                      .ToListAsync();
        return Ok(periods);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveClientProblemSto([FromBody] ClientProblemSto sto)
    {
      try
      {
        sto.Status = sto.MasteredForced ? StoStatus.Mastered : StoStatus.Unknow;
        _dbContext.Update(sto);
        await _dbContext.SaveChangesAsync();

        //update all weekrange and status = unknow
        //await _utils.UpdateClientProblemStos(sto.ClientProblemId);

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientProblemId}")]
    public async Task<IActionResult> GetClientProblemStos(int clientProblemId)
    {
      try
      {
        var periods = await _dbContext
                      .ClientProblemSTOs
                      .Where(w => w.ClientProblemId == clientProblemId)
                      .Select(s => new
                      {
                        s.ClientProblemStoId,
                        s.ClientProblemId,
                        s.Quantity,
                        s.Weeks,
                        s.WeekStart,
                        s.WeekEnd,
                        Status = s.Status.ToString(),
                        s.MasteredForced
                      })
                      .OrderBy(o => o.ClientProblemStoId)
                      .ToListAsync();
        return Ok(periods);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("[action]/{clientProblemId}")]
    public async Task<IActionResult> DeleteClientProblem(int clientProblemId)
    {
      try
      {
        var entry = await _dbContext.ClientProblems.FirstOrDefaultAsync(w => w.ClientProblemId == clientProblemId);
        _dbContext.Remove(entry);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, entry.ClientId, "Behavior", $"Behavior deleted for client");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("[action]/{clientProblemStoId}")]
    public async Task<IActionResult> DeleteClientProblemSto(int clientProblemStoId)
    {
      try
      {
        var entry = await _dbContext.ClientProblemSTOs.FirstOrDefaultAsync(w => w.ClientProblemStoId == clientProblemStoId);
        var id = entry.ClientProblemId;
        _dbContext.Remove(entry);
        await _dbContext.SaveChangesAsync();
        //await _utils.UpdateClientProblemStos(id);
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveReplacement([FromBody] ClientReplacement replacement)
    {
      try
      {
        _dbContext.Update(replacement);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Info, Module.Client, replacement.ClientId, "Replacement", $"Replacement added or updated");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> GetClientReplacements(int clientId)
    {
      try
      {
        var periods = await _dbContext
                            .ClientReplacements
                            .Include(i => i.Replacement)
                            .Include(i => i.STOs)
                            .Where(w => w.ClientId == clientId)
                            .OrderBy(o => o.Replacement.ReplacementProgramDescription)
                            .ToListAsync();
        return Ok(periods);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveClientReplacementSto([FromBody] ClientReplacementSto sto)
    {
      try
      {
        sto.Status = sto.MasteredForced ? StoStatus.Mastered : StoStatus.Unknow;
        _dbContext.Update(sto);
        await _dbContext.SaveChangesAsync();

        //update all weekrange and status = unknow
        await _utils.UpdateClientReplacementStos(sto.ClientReplacementId);
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientReplacementId}")]
    public async Task<IActionResult> GetClientReplacementStos(int clientReplacementId)
    {
      try
      {
        var periods = await _dbContext
                      .ClientReplacementSTOs
                      .Where(w => w.ClientReplacementId == clientReplacementId)
                      .Select(s => new
                      {
                        s.ClientReplacementStoId,
                        s.ClientReplacementId,
                        s.Percent,
                        s.Weeks,
                        s.WeekStart,
                        s.WeekEnd,
                        Status = s.Status.ToString(),
                        s.MasteredForced,
                        s.LevelAssistance,
                        s.TimeMinutes
                      })
                      .OrderBy(o => o.ClientReplacementStoId)
                      .ToListAsync();
        return Ok(periods);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("[action]/{clientReplacementId}")]
    public async Task<IActionResult> DeleteClientReplacement(int clientReplacementId)
    {
      try
      {
        var entry = await _dbContext.ClientReplacements.FirstOrDefaultAsync(w => w.ClientReplacementId == clientReplacementId);
        _dbContext.Remove(entry);
        await _dbContext.SaveChangesAsync();
        await _utils.NewSystemLog(SystemLogType.Warning, Module.Client, entry.ClientId, "Replacement", $"Replacement removed for client");

        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpDelete("[action]/{clientReplacementStoId}")]
    public async Task<IActionResult> DeleteClientReplacementSto(int clientReplacementStoId)
    {
      try
      {
        var entry = await _dbContext.ClientReplacementSTOs.FirstOrDefaultAsync(w => w.ClientReplacementStoId == clientReplacementStoId);
        _dbContext.Remove(entry);
        await _dbContext.SaveChangesAsync();
        await _utils.UpdateClientReplacementStos(entry.ClientReplacementId);
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ToggleClientProblem([FromBody] ClientProblem problem)
    {
      try
      {
        _dbContext.Update(problem);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ToggleClientReplacement([FromBody] ClientReplacement replacement)
    {
      try
      {
        _dbContext.Update(replacement);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> AdjustClientDataCollect([FromBody] AdjustClientDataCollectModel model)
    {
      try
      {
        var count = await _utils.AdjustClientDataCollect(model);
        return Ok(count);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> AdjustStoClientBehavior(int clientId)
    {
      try
      {
        await _stoProcess.ProccessClientBehavior(clientId);
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]/{clientId}")]
    public async Task<IActionResult> AdjustStoClientReplacement(int clientId)
    {
      try
      {
        await _stoProcess.ProccessClientReplacement(clientId);
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

  }
}