using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using AbaBackend.Auxiliary;
using AbaBackend.DataModel;
using AbaBackend.Infrastructure.Collection;
using AbaBackend.Infrastructure.Extensions;
using AbaBackend.Infrastructure.Security;
using AbaBackend.Infrastructure.Utils;
using AbaBackend.Model.MasterTables;
using AbaBackend.Model.Tellus;
using AbaBackend.Model.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbaBackend.Controllers
{
  [Authorize( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
  [Route( "api/tellus" )]
  public class TellusController : Controller
  {
    private readonly AbaDbContext _dbContext;
    private readonly TellusManager _tellusManager;
    private readonly IUtils _utils;
    private readonly IHostingEnvironment _env;
    readonly ICollection _collection;

    public TellusController( AbaDbContext context, IUtils utils, IHostingEnvironment env, ICollection collection, TellusManager tellusManager )
    {
      _dbContext = context;
      _tellusManager = tellusManager;
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
          .Select( u => new
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
            PassesAvailable = u.Passes.Where( w => !w.Used ).Count()
          } )
          .ToListAsync();
        return Ok( users );
      }
      catch ( Exception e )
      {
        return BadRequest( e.Message );
      }
    }

    [HttpGet( "{id}" )]
    public async Task<IActionResult> GetUser( int id )
    {
      try
      {
        var user = await _dbContext.Users
          .Where( w => w.UserId.Equals( id ) )
          .Select( u => new
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
          } )
          .FirstOrDefaultAsync();
        if ( user == null )
          return BadRequest( "User not found" );
        return Ok( user );
      }
      catch ( Exception e )
      {
        return BadRequest( e.Message );
      }
    }

    [HttpPost( "add-tellus-config" )]
    public async Task<IActionResult> AddTellusConfig( [FromBody] AddTellusConfig user )
    {
      using ( var transaction = await _dbContext.Database.BeginTransactionAsync() )
      {
        try
        {
          if ( !ModelState.IsValid )
            return BadRequest( ModelState.Values.SelectMany( e => e.Errors.Select( s => s.ErrorMessage ) ).FirstOrDefault() );
          if ( string.IsNullOrEmpty( user.Username ) )
            return BadRequest( "Username empty" );

          var oldUserConfig = await _dbContext.TellusCredentials.FirstOrDefaultAsync();
          if ( oldUserConfig != null )
          {
            _dbContext.TellusCredentials.Remove( oldUserConfig );
            await _dbContext.SaveChangesAsync();
          }

          var userConfig = new TellusCredential();
          /*var salt = Guid.NewGuid().ToByteArray();
          var hash = new PasswordHasher();
          userProcess.Salt = salt;
          userProcess.Hash = hash.Hash(user.Password, salt);*/
          userConfig.Username = user.Username;
          userConfig.Password = user.Password;

          await _dbContext.TellusCredentials.AddAsync( userConfig );
          await _dbContext.SaveChangesAsync();
          transaction.Commit();
          return Ok();
        }
        catch ( Exception e )
        {
          return BadRequest( e.InnerException?.Message ?? e.Message );
        }
      }
    }

    [HttpGet( "[action]/{from}/{to}" )]
    public async Task<IActionResult> GetTellusStepOne( string from, string to )
    {
      try
      {
        var fromDate = Convert.ToDateTime( from ).Date.ToString();
        var toDate = Convert.ToDateTime( to ).Date.ToString();
        var credentials = await _dbContext.TellusCredentials.AsNoTracking().FirstOrDefaultAsync();
        var auth = await _tellusManager.AuthTellus( credentials );

        IDictionary<string, JObject> visitsDetails = new Dictionary<string, JObject>();
        if ( auth.access_token != null )
        {
          string access_token = auth.access_token;
          var userDetails = _tellusManager.GetAuthenticatedUserDetails( access_token );
          var current_provider_id = userDetails["providers"][0]["providerId"].ToString();
          //*****OJOOOOOO****** CAMBIAR VALORES FIJOS DE TAMAÑO DE PÁGINA Y PÁGINA ACTUAL
          var tempVisits = _tellusManager.GetVisits( access_token, current_provider_id, fromDate, toDate, null, null, 500 );
          var visits = tempVisits["_embedded"]["visits"];
          
          foreach ( JObject v in visits.OfType<JObject>() )
          {
            string visitId = v["id"].ToString();
            var visit_details = _tellusManager.GetVisitDetails( access_token, visitId );
            visitsDetails.Add( visitId, visit_details );
          }
        }
        else
        {
          return BadRequest( auth.error_description );
        }
        /*var sessions = await _dbContext.Sessions
            .AsNoTracking()
            .Where( w => w.UserId == userId )
            .Where( w => w.SessionStart.Date >= fromDate && w.SessionStart.Date <= toDate )
            .Where( w => w.SessionStatus == SessionStatus.Billed )
            .OrderBy( w => w.SessionStart )
            .Select( s => new
            {
              s.SessionId,
              s.UserId,
              s.ClientId,
              ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
              s.Client.Code,
              SessionStart = s.SessionStart.ToString( "u" ),
              SessionEnd = s.SessionEnd.ToString( "u" ),
              s.TotalUnits,
              SessionType = s.SessionType.ToString().Replace( "_", " " ),
              SessionStatus = s.SessionStatus.ToString(),
              SessionStatusCode = s.SessionStatus,
              SessionStatusColor = ( (SessionStatusColors) s.SessionStatus ).ToString(),
              Pos = s.Pos.ToString().Replace( "_", " " ),
              PosCode = s.Pos,
              UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
              Rol = s.User.Rol.RolShortName
            } )
            .OrderBy( o => o.SessionStart )
            .ToListAsync();*/

        ///return Ok( sessions );
        return Ok( visitsDetails.ToList() );
      }
      catch ( System.Exception e )
      {
        return BadRequest( e.Message );
      }
    }
  }

}