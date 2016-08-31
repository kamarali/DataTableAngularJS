using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
  public interface IMemberLocationUpdateHandler
  {
    bool LocationUpdateSenderForFutureUpdates(int memberId, List<FutureUpdateTemp> futureUpdateTemps);
    bool LocationUpdateSenderForImmediateUpdates(int memberId, List<FutureUpdates> futureUpdates);
  }
}
