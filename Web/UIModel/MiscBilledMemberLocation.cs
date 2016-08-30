using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Web.UIModel
{
  public class MiscBilledMemberLocation
  {
    public List<LocationCodeDetails> BilledLocations { get; set; }
    public string DefaultLocation { get; set; }
  }
}