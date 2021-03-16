using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AbaBackend.DataModel;
using AbaBackend.Model.Tellus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbaBackend.Auxiliary
{
  public class TellusManager
  {
    public async Task<TellusResponse> AuthTellus( TellusCredential user )
    {
      using ( HttpClient client = new HttpClient() )
      {
        var content = new FormUrlEncodedContent( new[]
        {
            new KeyValuePair<string, string>("client_id", "frontend"),
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", user.Username),
            new KeyValuePair<string, string>("password", user.Password)
        } );
        string endpoint = "https://auth.4tellus.net/auth/realms/evv/protocol/openid-connect/token";

        using ( var Response = await client.PostAsync( endpoint, content ) )
        {
          //if ( Response.StatusCode == System.Net.HttpStatusCode.OK )
          //{
          var gottenContent = await Response.Content.ReadAsStringAsync();
          var json = JsonConvert.DeserializeObject<TellusResponse>( gottenContent );
          return json;
          ///}       
        }
      }
    }

    public JObject GetAuthenticatedUserDetails( string token )
    {
      using ( HttpClient client = new HttpClient() )
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", token );
        string endpoint = "https://services.4tellus.net/users/details";

        using ( var Response = client.GetAsync( endpoint ).Result )
        {
          var gottenContent = Response.Content.ReadAsStringAsync().Result;
          JObject json = JObject.Parse( gottenContent );
          return json;
        }
      }
    }

    public IDictionary<string, JObject> GetVisits( string access_token, string current_user_provider_id, string start, string end, string[] recipient_ids, string user_id, int pagesize )
    {
      using ( HttpClient client = new HttpClient() )
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", access_token );
        string[] arr = new string[] { };

        var contentObj = new
        {
          providerId = current_user_provider_id,
          uiType = "Provider",
          recipientIds = arr,
          states = arr,
          statuses = arr,
          start,
          end
          //start = "2020-12-01T05:00:00.000Z",
          //end = "2020-12-31T05:00:00.000Z"
        };
        var content = JsonConvert.SerializeObject( contentObj );
        var httpContent = new StringContent( content, Encoding.UTF8, "application/json" );
        int page = 0;
        bool broughtAny = true;
        //Objeto para almacenar los elementos de cada página
        JObject json = null;
        //Lista de objetos almacenados por página que viene de tellus
        IDictionary<string, JObject> resultList = new Dictionary<string, JObject>();

        //UNCOMMENT WHILE 
        while ( broughtAny )
        {
          string endpoint = $"https://services.4tellus.net/visits/search?size={pagesize}&page={page}&sort=scheduledStartTime,asc&projection=visitShortListItem&uiType=Provider";
          using ( var Response = client.PostAsync( endpoint, httpContent ).Result )
          {
            
            if ( Response.Content != null )
            {
              page++;
              var gottenContent = Response.Content.ReadAsStringAsync().Result;
              if ( gottenContent.Contains( "visits" ) )
              {
                json = JObject.Parse( gottenContent );
                resultList.Add( page.ToString(), json );
              }
              else
                broughtAny = false;
            }
            else
              broughtAny = false;
          } 
        }
        return resultList;

      }
    }
                                                  
    public IDictionary<string, JObject> GetClaimsFromWorkList( string access_token, string current_user_provider_id, string payer_id, int pagesize, string[] recipient_ids, string start = null, string end = null, bool include_archived = false )
    {
      using ( HttpClient client = new HttpClient() )
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", access_token );
        string[] arr = new string[] { };


        string authorizations = null;
        string noClaimSupport = null;
        string payerICNs = null;
        string serviceCodes = null;
        string statuses = null;
        string visitInternalId = null;
        string recipients = null;

        /*var contentObj = new
        {
          archived= include_archived,
          providerId= current_user_provider_id,
          uiType= "Provider",
          payerId= payer_id,  // ACBA
          authorizations,
          dateEnd = end,
          dateStart = start,
          noClaimSupport,
          payerICNs,
          //recipientIds,
          recipients= recipient_ids,
          serviceCodes,
          serviceIds,
          statuses,
          visitInternalId,
        };*/
        var contentObj = new
        {
          providerId = current_user_provider_id,
          uiType = "Provider",
          payerId = payer_id,
          noClaimSupport,
          recipients,
          serviceCodes,
          statuses,
          payerICNs,
          authorizations,
          visitInternalId,
          dateStart = start,
          dateEnd = end,
          archived = include_archived,
          recipientIds = recipient_ids
        };
          /*var contentObj = new
          {
            providerId = current_user_provider_id,
            uiType = "Provider",
            payerId = payer_id,
            recipients = arr,
            serviceCodes = arr,
            payerICNs = arr,
            memberIds = arr,
            authorizations = arr,
            visitInternalIds = arr,
            dateStart = start,
            dateEnd = end
          };*/
          var content = JsonConvert.SerializeObject( contentObj );
        var httpContent = new StringContent( content, Encoding.UTF8, "application/json" );
        int page = 0;
        bool broughtAny = true;
        //Objeto para almacenar los elementos de cada página
        JObject json = null;
        //Lista de objetos almacenados por página que viene de tellus
        IDictionary<string, JObject> resultList = new Dictionary<string, JObject>();
        //UNCOMMENT WHILE 
        while ( broughtAny )
        {
          //Este usa work list
          string endpoint = $"https://services.4tellus.net/payer/worklist/{payer_id}/{current_user_provider_id.ToLower()}/search?page={page}&size={pagesize}&uiType=Provider";
          ///Este usa el claimlist
          ///string endpoint = $"https://services.4tellus.net/payer/claimlist/{payer_id}/{current_user_provider_id.ToLower()}/search?page={page}&size={pagesize}&sort=statusCode,desc&uiType=Provider";
          using ( var Response = client.PostAsync( endpoint, httpContent ).Result )
          {
            
            if ( Response.Content != null )
            {
              page++;
              var gottenContent = Response.Content.ReadAsStringAsync().Result;
              if ( gottenContent.Contains( "claimInvoices" ) )
              {
                json = JObject.Parse( gottenContent );
                if ( json["page"]["totalElements"].ToString() != "0" )
                {
                  resultList.Add( page.ToString(), json );
                }
                else
                  broughtAny = false;
              }
              else
                broughtAny = false;
            }
            else
              broughtAny = false;
          } 
        }
        return resultList;

      }
    }

    public JObject GetVisitDetails( string access_token, string visit_id )
    {
      using ( HttpClient client = new HttpClient() )
      {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue( "Bearer", access_token );
        string endpoint = $"https://services.4tellus.net/visits/{visit_id}?uiType=Provider";

        using ( var Response = client.GetAsync( endpoint ).Result )
        {
          var gottenContent = Response.Content.ReadAsStringAsync().Result;
          JObject json = JObject.Parse( gottenContent );
          return json;
        }
      }
    }
  }
}
