using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
  public interface IMemberProfileImmediateUpdatesEmailHandler
  {
    List<FutureUpdates> UpdatesList { get; set; }
    bool SendMailsForImmediateMemberProfileUpdates();
   }
}
