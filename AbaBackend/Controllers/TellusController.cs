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
        // AGF: Aqui voy a meter los chamas pal tema de comparar los nombres
        IDictionary<string, JObject> recipients = new Dictionary<string, JObject>();
        // AGF: contendra los recipients q tienen diferencia en el nombre u otra cosa que se defina en el futuro
        List<Tuple<JObject, Client>> recipients_with_differences = new List<Tuple<JObject, Client>>();
        // AGF: contendra los recipients q vienen en la visita y no estan en la bd
        List<JObject> recipients_not_in_bd = new List<JObject>();
        if ( auth.access_token != null )
        {
          string access_token = auth.access_token;
          var userDetails = _tellusManager.GetAuthenticatedUserDetails( access_token );
          var current_provider_id = userDetails["providers"][0]["providerId"].ToString();
          //*****OJOOOOOO****** CAMBIAR VALORES FIJOS DE TAMAÑO DE PÁGINA Y PÁGINA ACTUAL
          // AGF: le puse de paginado 10 pa poder probar mas rapido y no esperar a que descargue los 500
          var tempVisits = _tellusManager.GetVisits( access_token, current_provider_id, fromDate, toDate, null, null, 10 );
          var visits = tempVisits["_embedded"]["visits"];
          
          foreach ( JObject v in visits.OfType<JObject>() )
          {
            string visitId = v["id"].ToString();
            var visit_details = _tellusManager.GetVisitDetails( access_token, visitId );        
            visitsDetails.Add( visitId, visit_details );
            // AGF: adiciono los recipients que vienen en la visita al listado de visitas
            foreach (var recipient in visit_details["recipients"].OfType<JObject>())
            {
              String medicaidId = recipient["medicaidId"].ToString();
              if (!recipients.ContainsKey(medicaidId))
              {
                recipients.Add(medicaidId, recipient);
              }
            }
          }

          if (recipients.Count() > 0)
          {
            // AGF: busco en bd los recipients q esten en el listado de arriba. lo hago buscando por la columna MemberNo lo que en el recipient de la visita viene en el campo medicaidId
            var recipients_in_bd = from client in _dbContext.Clients.AsNoTracking() where recipients.Keys.Contains(client.MemberNo) select client;
            foreach (var item in recipients_in_bd)
            {
              var to_compare = recipients[item.MemberNo];
              if(to_compare["firstName"].ToString().Trim().ToUpper() != item.Firstname.Trim().ToUpper() || to_compare["lastName"].ToString().Trim().ToUpper() != item.Lastname.Trim().ToUpper())
              {
                recipients_with_differences.Add(new Tuple<JObject, Client>(recipients[item.MemberNo], item));
              }

              recipients.Remove(item.MemberNo);
            }
          }

          // AGF: si queda algun recipient en este listado es que no esta en la bd...
          if (recipients.Count() > 0)
          {
            recipients_not_in_bd = recipients.Values.ToList();
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