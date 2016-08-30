using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile 
{
  public interface IFutureUpdatesServiceRepository : IRepository<FutureUpdateTemp>
  {
    List<FutureUpdateTemp> GetFutureUpdatesDoneByService(DateTime ichBillingPeriod,DateTime achBillingPeriod);

    String GetRelationIdDisplayName(int futureUpdateId);
  }
}
