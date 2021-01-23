using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AbaBackend.Controllers;
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

    public JObject GetVisits( string access_token, string current_user_provider_id, string start, string end, string[] recipient_ids, string user_id, int pagesize )
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
          statuses = arr
        };
        var content = JsonConvert.SerializeObject( contentObj );
        var httpContent = new StringContent( content, Encoding.UTF8, "application/json" );
        int page = 1;
        ///string endpoint = "https://services.4tellus.net/visits/search?size=" + pagesize + "&page=" + 1 + "&sort=scheduledStartTime,asc&projection=visitShortListItem&uiType=Provider";
        string endpoint = $"https://services.4tellus.net/visits/search?size={pagesize}&page={page}&sort=scheduledStartTime,asc&projection=visitShortListItem&uiType=Provider";

        using ( var Response = client.PostAsync( endpoint, httpContent ).Result )
        {
          if ( Response.Content != null )
          {
            var gottenContent = Response.Content.ReadAsStringAsync().Result;
            JObject json = JObject.Parse( gottenContent );
            return json;
          }
          else
            return new JObject();
        }
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
