
using System.Collections.Generic;

namespace AbaBackend.Model.Tellus
{
public class AuthenticatedUserDetails
  {
    public UserAdditionalInfo additionalInfo { get; set; }
    public List<UserProviders> providers { get; set; }
    public string recipients { get; set; }
    public List<UserUiTypes> uiTypes { get; set; }
  }
}
