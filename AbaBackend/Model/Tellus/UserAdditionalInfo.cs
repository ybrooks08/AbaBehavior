
namespace AbaBackend.Model.Tellus
{
public class UserAdditionalInfo
  {
    public string id { get; set;}
    public string createdTimestamp { get; set;}
    public string updatedTimestamp { get; set;}
    public string username { get; set;}
    public string firstName { get; set;}
    public string lastName { get; set;}
    public string email { get; set;}
    public string phoneNumber { get; set;}
    public string ivrPin { get; set;}
    public string lastMessageRead { get; set;}
    public string avatarUrl { get; set;}
    public string driversLicense { get; set;}
    public string driversLicenseState { get; set;}
    public string providerIds { get; set;}
    public string type { get; set;}
    public string userType { get; set;}
    public string medicaidId { get; set;}
    public string status { get; set;}
    public bool acceptedTerms { get; set;}
    public string dateOfBirth { get; set;}
    public string ssn { get; set;}
    public string address { get; set;}
  }
}
