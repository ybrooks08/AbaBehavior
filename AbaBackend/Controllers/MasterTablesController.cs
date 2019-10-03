using System;
using System.Linq;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Model.MasterTables;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbaBackend.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
  [Route("api/master-tables")]
  public class MasterTablesController : Controller
  {
    private readonly AbaDbContext _dbContext;

    public MasterTablesController(AbaDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    [HttpGet("problems-behaviors")]
    public async Task<IActionResult> GetProblemBehaviors(int id)
    {
      try
      {
        var list = await _dbContext.ProblemBehaviors.OrderBy(s => s.ProblemBehaviorDescription).ToListAsync();
        return Ok(list);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("problems-behaviors/change-status")]
    public async Task<IActionResult> ProblemBehaviorChangeStatus([FromBody] ChangeStatus newStatus)
    {
      try
      {
        var user = await _dbContext.ProblemBehaviors.FirstOrDefaultAsync(w => w.ProblemId.Equals(newStatus.Id));
        if (user == null) return BadRequest("Problem behavior not found");
        user.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }

    }

    [HttpPost("problems-behaviors/add-edit")]
    public async Task<IActionResult> ProblemBehaviorAddEdit([FromBody] ProblemsBehaviors item)
    {
      try
      {
        if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        var itemProcess = item.Id == 0 ? new ProblemBehavior() : await _dbContext.ProblemBehaviors.FirstOrDefaultAsync(w => w.ProblemId.Equals(item.Id));
        if (itemProcess == null) return BadRequest("Item not found");
        itemProcess.ProblemBehaviorDescription = item.Description;

        if (itemProcess.ProblemId == 0) await _dbContext.ProblemBehaviors.AddAsync(itemProcess);
        await _dbContext.SaveChangesAsync();
        return Ok(itemProcess);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("replacement-programs")]
    public async Task<IActionResult> GetReplacementPrograms(int id)
    {
      try
      {
        var list = await _dbContext.ReplacementPrograms.OrderBy(s => s.ReplacementProgramDescription).ToListAsync();
        return Ok(list);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("replacement-programs/change-status")]
    public async Task<IActionResult> ReplacementProgramsChangeStatus([FromBody] ChangeStatus newStatus)
    {
      try
      {
        var user = await _dbContext.ReplacementPrograms.FirstOrDefaultAsync(w => w.ReplacementId.Equals(newStatus.Id));
        if (user == null) return BadRequest("Replacement program not found");
        user.Active = newStatus.Status;
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }

    }

    [HttpPost("replacement-programs/add-edit")]
    public async Task<IActionResult> ReplacementProgramsAddEdit([FromBody] ReplacementPrograms item)
    {
      try
      {
        if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        var itemProcess = item.Id == 0 ? new ReplacementProgram() : await _dbContext.ReplacementPrograms.FirstOrDefaultAsync(w => w.ReplacementId.Equals(item.Id));
        if (itemProcess == null) return BadRequest("Item not found");
        itemProcess.ReplacementProgramDescription = item.Description;

        if (itemProcess.ReplacementId == 0) await _dbContext.ReplacementPrograms.AddAsync(itemProcess);
        await _dbContext.SaveChangesAsync();
        return Ok(itemProcess);
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-diagnosis-count")]
    public async Task<IActionResult> GetDiagnosisCount()
    {
      try
      {
        var count = await _dbContext.Diagnostics.CountAsync();
        return Ok(count);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-diagnosis/{code}")]
    public async Task<IActionResult> GetDiagnosis(string code)
    {
      try
      {
        var diag = await _dbContext.Diagnostics.FirstOrDefaultAsync(w => w.Code.ToLower().Equals(code.ToLower()));
        diag = diag ?? new Diagnosis { DiagnosisId = 0 };
        return Ok(diag);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpPost("add-edit-diagnosis")]
    public async Task<IActionResult> AddEditDiagnosis([FromBody] Diagnosis diagnosis)
    {
      try
      {
        if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(e => e.Errors.Select(s => s.ErrorMessage)).FirstOrDefault());
        var diagCheck = await _dbContext.Diagnostics.FirstOrDefaultAsync(w => w.Code.ToLower().Equals(diagnosis.Code.ToLower()));
        var diag = diagCheck ?? new Diagnosis { Code = diagnosis.Code.ToUpper() };
        diag.Description = diagnosis.Description;
        if (diagCheck == null) await _dbContext.Diagnostics.AddAsync(diag);
        await _dbContext.SaveChangesAsync();
        return Ok();
      }
      catch (Exception e)
      {
        return BadRequest(e.InnerException?.Message ?? e.Message);
      }
    }

    [HttpGet("get-behavior-analysis-codes/{onlyCheck?}")]
    public async Task<IActionResult> GetBehaviorAnalysisCode(bool onlyCheck = false)
    {
      try
      {
        var count = await _dbContext
                          .BehaviorAnalysisCodes
                          .Where(w => !onlyCheck || w.Checkable)
                          .OrderBy(o => o.BehaviorAnalysisCodeId)
                          .ToListAsync();
        return Ok(count);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("get-pos-codes")]
    public IActionResult GetPosCodes()
    {
      var pos = Enum.GetValues(typeof(Pos))
                    .Cast<Pos>()
                    .Select(x => new { value = (int)x, text = x.ToString().Replace("_", " ") })
                    .ToList();
      return Ok(pos);
    }

    [HttpGet("get-risk-behavior-codes")]
    public IActionResult GetRiskBehaviorCodes()
    {
      var pos = Enum.GetValues(typeof(RiskBehavior))
                    .Cast<RiskBehavior>()
                    .Select(x => new { value = (int)x, text = x.ToString().Replace("_", " ") })
                    .ToList();
      return Ok(pos);
    }

    [HttpGet("get-participation-level-codes")]
    public IActionResult GetParticipationLevelCodes()
    {
      var pos = Enum.GetValues(typeof(ParticipationLevel))
                    .Cast<ParticipationLevel>()
                    .Select(x => new { value = (int)x, text = x.ToString().Replace("_", " ") })
                    .ToList();
      return Ok(pos);
    }

    [HttpGet("get-competency-check-types")]
    public IActionResult GetCompetencyCheckTypes()
    {
      var pos = Enum.GetValues(typeof(CompetencyCheckType))
                    .Cast<CompetencyCheckType>()
                    .Select(x => new { value = (int)x, text = x.ToString().Replace("_", " ") })
                    .ToList();
      return Ok(pos);
    }

    [HttpGet("get-competency-check-params")]
    public async Task<IActionResult> GetCompetencyCheckParams()
    {
      try
      {
        var competencyChecksParams = await _dbContext.CompetencyCheckParams.OrderBy(o => o.CompetencyCheckType).ThenBy(o => o.CompetencyCheckParamId).ToListAsync();
        return Ok(competencyChecksParams);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }

    [HttpGet("[action]")]
    public IActionResult GetSessionSupervisionWorkWith()
    {
      var pos = Enum.GetValues(typeof(WorkWith))
                    .Cast<WorkWith>()
                    .Select(x => new { value = (int)x, text = x.ToString().Replace("_", " ") })
                    .OrderBy(o => o.value)
                    .ToList();
      return Ok(pos);
    }

    [HttpGet("[action]")]
    public IActionResult GetOversightSessionSupervision()
    {
      var pos = Enum.GetValues(typeof(OversightSessionSupervision))
                    .Cast<OversightSessionSupervision>()
                    .Select(x => new { value = (int)x, text = x.ToString().Replace("_", " ") })
                    .OrderBy(o => o.value)
                    .ToList();
      return Ok(pos);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetSystemLogs()
    {
      try
      {
        var start = DateTime.Today.AddDays(-30);
        var end = DateTime.Today.AddDays(1).AddTicks(-1);
        var logs = await _dbContext.SystemLogs.Where(w => w.Entry.Date >= start && w.Entry.Date <= end)
        .Select(s => new
        {
          s.SystemLogId,
          SystemLogType = s.SystemLogType.ToString(),
          Module = s.Module.ToString(),
          s.Entry,
          s.Title,
          s.Description,
          s.User,
          s.ModuleValue
        }).OrderByDescending(o => o.Entry).Take(1000).ToListAsync();
        return Ok(logs);
      }
      catch (Exception e)
      {
        return BadRequest(e.Message);
      }
    }
    
    [HttpGet("[action]")]
    public IActionResult GetDayOfWeekBitValues()
    {
      var pos = Enum.GetValues(typeof(DayOfWeekBit))
        .Cast<DayOfWeekBit>()
        .Select(x => new { value = (int)x, text = x.ToString()})
        .OrderBy(o => o.value)
        .ToList();
      return Ok(pos);
    }

  }
}