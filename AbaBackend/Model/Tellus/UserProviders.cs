
using System.Collections.Generic;

namespace AbaBackend.Model.Tellus
{
public class UserProviders
  {
    public string providerId { get; set;}
    public string providerName { get; set;}
    public string providerShortName { get; set;}
    public bool active { get; set;}
    public string invitationTimestamp { get; set;}
    public string acceptanceTimestamp { get; set;}
    public List<UserRoles> roles { get; set;}
    public string confirmationLink { get; set;}
    public List<UserPayerLinks> payerLinks { get; set;}
    public string npiZipCode { get; set;}
    public string npiTaxonomy { get; set;}
    public string npiNumber { get; set;}
    public string skin { get; set;}
    public string providerActive { get; set;}
    public List<UserUiTypes> uiTypes { get; set;}
  }
}
