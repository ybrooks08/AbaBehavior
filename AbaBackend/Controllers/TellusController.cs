using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
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
        DateTime tempDateTo = DateTime.ParseExact( to, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture );
        tempDateTo = tempDateTo.Add( new TimeSpan( 23, 59, 59 ) );          
        to = tempDateTo.ToString( "yyyy-MM-ddTHH:mm:ss" );  

        
        var customFrom = ConvertDateToUTCSumFiveHours( from );
        var customTo = TimeZoneInfo.ConvertTimeToUtc( tempDateTo ).ToString( "yyyy-MM-ddTHH:mm:ss.000Z" );
        var credentials = await _dbContext.TellusCredentials.AsNoTracking().FirstOrDefaultAsync();
        var auth = await _tellusManager.AuthTellus( credentials );

        IDictionary<string, JObject> visitsDetails = new Dictionary<string, JObject>();
        // AGF: Aqui voy a meter los chamas pal tema de comparar los nombres
        IDictionary<string, JObject> recipients = new Dictionary<string, JObject>();
        // AGF: contendra los recipients q tienen diferencia en el nombre u otra cosa que se defina en el futuro
        List<Tuple<JObject, Client>> recipients_with_differences = new List<Tuple<JObject, Client>>();
        // AGF: contendra los recipients q vienen en la visita y no estan en la bd
        List<JObject> recipients_not_in_bd = new List<JObject>();
        //YBCH: Aca la lista de los datos de tellus a incluir en el resultado final 
        List<VisitToMatch> tellusSessions = new List<VisitToMatch>();
        //YBCH: Aca la lista de los datos del sistema local a incluir en el resultado final 
        List<VisitToMatch> localSessions = new List<VisitToMatch>();
        //YBCH: Almaceno las listas de datos del sistema local y de tellus para después llenar cada una de las listas del front y hacer el macheo
        IDictionary<string, List<VisitToMatch>> gotten_visits = new Dictionary<string, List<VisitToMatch>>();
        //Lista de visitas de tellus a devolver
        

        if ( auth.access_token != null )
        {
          string access_token = auth.access_token;
          var userDetails = _tellusManager.GetAuthenticatedUserDetails( access_token );
          var current_provider_id = userDetails["providers"][0]["providerId"].ToString();
          var tempVisits = _tellusManager.GetVisits( access_token, current_provider_id, customFrom, customTo, null, null, 100 );


          foreach ( var i in tempVisits )
          {
            var visits = i.Value["_embedded"]["visits"];
            foreach ( JObject v in visits.OfType<JObject>() )
            {
              //Validando que la visita esta completada
              if ( v["status"].ToString() == "COMPLETED" )
              {
                //Visita de tellus a agregar en la lista a devolver
                var visit = new VisitToMatch();
                //La mejor manera que hay de resolver esto es mediante la zona horaria, mejora pendiente
                DateTime sessionStart;

                if ( DateTime.TryParseExact( v["actualStartTime"].ToString(), "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionStart ) )
                {
                  sessionStart = sessionStart.Add( new TimeSpan( 5, 0, 0 ) );
                  visit.SessionStart = sessionStart.ToString( "u" );
                }
                else
                {
                  sessionStart = DateTime.ParseExact( v["actualStartTime"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture );
                  visit.SessionStart = sessionStart.ToString( "u" );
                }

                DateTime sessionEnd;
                if ( DateTime.TryParseExact( v["actualEndTime"].ToString(), "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out sessionEnd ) )
                {
                  sessionEnd = sessionEnd.Add( new TimeSpan( 5, 0, 0 ) );
                  visit.SessionEnd = sessionEnd.ToString( "u" );
                }
                else
                {
                  sessionEnd = DateTime.ParseExact( v["actualEndTime"].ToString(), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture );
                  visit.SessionEnd = sessionEnd.ToString( "u" );
                }

                visit.SessionId = v["id"].ToString();
                visit.UserId = v["user"]["id"].ToString();
                visit.ClientId = v["visitRecipientServicesInfo"][0]["recipient"]["id"].ToString();
                visit.ClientFullname = v["visitRecipientServicesInfo"][0]["recipient"]["fullName"].ToString();
                // Find out
                visit.Code = "Code";

                //Find service hours
                visit.TotalUnits = 0;
                // Find out SessionType
                visit.SessionType = "BA Service";
                visit.SessionStatus = v["status"].ToString();
                // Find out
                visit.SessionStatusCode = "SessionStatusCode";
                // Find out
                visit.SessionStatusColor = "SessionStatusColor";
                visit.Pos = "Pos";
                // Find out
                visit.PosCode = "Home";
                visit.UserFullname = v["user"]["fullName"].ToString();
                visit.Rol = "Rol";
                visit.Edit = false;
                visit.Difference = false;

                /*** Working with visit details ***/
                /*string visitId = v["id"].ToString();
                var visit_details = _tellusManager.GetVisitDetails( access_token, visitId );
                visitsDetails.Add( visitId, visit_details );
                // AGF: adiciono los recipients que vienen en la visita al listado de visitas
                foreach ( var recipient in visit_details["recipients"].OfType<JObject>() )
                {
                  String medicaidId = recipient["medicaidId"].ToString();
                  if ( !recipients.ContainsKey( medicaidId ) )
                  {
                    recipients.Add( medicaidId, recipient );
                  }
                }*/                
                tellusSessions.Add( visit ); 
              }
            }

          }

          /*** Working with visit details ***/
          /*if (recipients.Count() > 0)
          {
            // AGF: busco en bd los recipients q esten en el listado de arriba. lo hago buscando por la columna MemberNo lo que en el recipient de la visita viene en el campo medicaidId
            var recipients_in_bd = from client in _dbContext.Clients.AsNoTracking() where recipients.Keys.Contains( client.MemberNo ) select client;
            foreach ( var item in recipients_in_bd )
            {
              //Ver si es correcta esta condicional
              if ( recipients.ContainsKey( item.MemberNo ) )
              {
                var to_compare = recipients[item.MemberNo];
                if ( to_compare["firstName"].ToString().Trim().ToUpper() != item.Firstname.Trim().ToUpper() || to_compare["lastName"].ToString().Trim().ToUpper() != item.Lastname.Trim().ToUpper() )
                {
                  recipients_with_differences.Add( new Tuple<JObject, Client>( recipients[item.MemberNo], item ) );
                }

                recipients.Remove( item.MemberNo ); 
              }
            }
          }

          // AGF: si queda algun recipient en este listado es que no esta en la bd...
          if (recipients.Count() > 0)
          {
            recipients_not_in_bd = recipients.Values.ToList();
          }*/
        }
        else
        {
          return BadRequest( auth.error_description );
        }

        localSessions = await GetSessionsByDate( from, to );
        //YBCH: Aca la lista de los datos de tellus a incluir en el resultado final 
        List<VisitToMatch> tellusSessionsOne = new List<VisitToMatch>();
        //YBCH: Aca la lista de los datos del sistema local a incluir en el resultado final 
        List<VisitToMatch> localSessionsOne = new List<VisitToMatch>();
        foreach ( var i in localSessions )
        {
          foreach ( var j in tellusSessions )
          {
            var tellusClientName = Regex.Replace( j.ClientFullname.ToLowerInvariant(), @"(^\w)|(\s\w)", m => m.Value.ToUpper() );
            var tellusUserName = Regex.Replace( j.UserFullname.ToLowerInvariant(), @"(^\w)|(\s\w)", m => m.Value.ToUpper() );
            
            if ( i.ClientFullname == tellusClientName && i.UserFullname == tellusUserName )
            {
              string tellusDateStart = i.SessionStart.Remove( i.SessionStart.Length - 10, 10 );              
              string localDateStart = j.SessionStart.Remove( j.SessionStart.Length - 10, 10 );              
              string tellusDateEnd = i.SessionEnd.Remove( i.SessionEnd.Length - 10, 10 );              
              string localDateEnd = j.SessionEnd.Remove( j.SessionEnd.Length - 10, 10 );
              
              string tellusHourStart = i.SessionStart.Substring( 11, i.SessionStart.Length - 15 );
              string localHourStart = j.SessionStart.Substring( 11, j.SessionStart.Length - 15 );
              string tellusHourEnd = i.SessionEnd.Substring( 11, i.SessionEnd.Length - 15 );
              string localHourEnd = j.SessionEnd.Substring( 11, j.SessionEnd.Length - 15 );
              if ( tellusDateStart == localDateStart )
              {
                if ( tellusHourStart != localHourStart || tellusHourEnd != localHourEnd )
                {
                  if ( !localSessionsOne.Any( n => n.SessionId == i.SessionId ) )
                  {
                    localSessionsOne.Add( i ); 
                  }
                  tellusSessionsOne.Add( j );
                  /**Visita duplicada a mano para probar***/
                  /*DateTime lolo;
                  DateTime lolol1;
                  if ( DateTime.TryParseExact( j.SessionStart, "yyyy-MM-dd HH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out lolo ) )
                  {
                    lolo = lolo.Add( new TimeSpan( 1, 0, 0 ) );
                    ///tempTellus.SessionStart = lolo.ToString( "u" );
                  }
                  if ( DateTime.TryParseExact( j.SessionEnd, "yyyy-MM-dd HH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out lolol1 ) )
                  {
                    lolol1 = lolol1.Add( new TimeSpan( 1, 0, 0 ) );
                    ///tempTellus.SessionEnd = lolol1.ToString( "u" );
                  }
                  var tempTellus = new VisitToMatch
                  {
                    SessionId = j.SessionId,
                    UserId = j.UserId,
                    ClientId = j.ClientId,
                    ClientFullname = j.ClientFullname,
                    Code = j.Code,
                    SessionStart = lolo.ToString( "u" ),
                    SessionEnd = lolol1.ToString( "u" ),
                    TotalUnits = j.TotalUnits,
                    SessionType = j.SessionType,
                    SessionStatus = j.SessionStatus,
                    SessionStatusCode = j.SessionStatusCode,
                    SessionStatusColor = j.SessionStatusColor,
                    Pos = j.Pos,
                    PosCode = j.PosCode,
                    UserFullname = j.UserFullname,
                    Rol = j.Rol,
                    Edit = j.Edit,
                    Difference = j.Difference
                  };
                  tellusSessionsOne.Add( tempTellus );*/
                  /***/

                }
              } 
            }
          }
        }
        /*** ***/
        gotten_visits.Add( "system_visits", ( localSessionsOne ) );
        gotten_visits.Add( "tellus_visits", ( tellusSessionsOne ) );
        return Ok( gotten_visits );
      }
      catch ( System.Exception e )
      {
        return BadRequest( e.Message );
      }
    }

    public async Task<List<VisitToMatch>> GetSessionsByDate( string from, string to = null )
    {
      try
      {
        var fromDate = Convert.ToDateTime( from ).Date;
        var toDate = ( to != null ) ? Convert.ToDateTime( to ).Date : fromDate;

        var sessions = await _dbContext.Sessions
            .AsNoTracking()
            .Where( w => w.SessionStart.Date >= fromDate && w.SessionStart.Date <= toDate )
            .Where( w => w.SessionStatus == SessionStatus.Billed )
            .OrderBy( w => w.SessionStart )
            .Select( s => new VisitToMatch
            {
              SessionId = s.SessionId.ToString(),
              UserId = s.UserId.ToString(),
              ClientId = s.ClientId.ToString(),
              ClientFullname = $"{s.Client.Firstname} {s.Client.Lastname}",
              Code = s.Client.Code,
              SessionStart = s.SessionStart.ToString( "u" ),
              SessionEnd = s.SessionEnd.ToString( "u" ),
              TotalUnits = s.TotalUnits,
              SessionType = s.SessionType.ToString().Replace( "_", " " ),
              SessionStatus = s.SessionStatus.ToString(),
              //Find out
              //SessionStatusCode = s.SessionStatus,
              SessionStatusCode = "SessionStatusCode",
              SessionStatusColor = ( (SessionStatusColors) s.SessionStatus ).ToString(),
              Pos = s.Pos.ToString().Replace( "_", " " ),
              //Find out
              //PosCode = s.Pos,
              PosCode = "PosCode",
              UserFullname = $"{s.User.Firstname} {s.User.Lastname}",
              Rol = s.User.Rol.RolShortName,
              Edit = false,
              Difference = false
            } )
            .OrderBy( o => o.SessionStart )
            .ToListAsync();

        return sessions;
      }
      catch ( System.Exception e )
      {
        throw new InvalidOperationException( e.Message );
      }
    }

    private string ConvertDateToUTCSumFiveHours( string date )
    {
      var myDate = Convert.ToDateTime( date ).Date;
      var result = TimeZoneInfo.ConvertTimeToUtc( myDate ).ToString( "yyyy-MM-ddThh:mm:ss.000Z" );
      return result;
    }
  }

}