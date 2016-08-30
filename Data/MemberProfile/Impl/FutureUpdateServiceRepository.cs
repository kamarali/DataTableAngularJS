using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Objects;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class FutureUpdateServiceRepository : Repository<FutureUpdateTemp>, IFutureUpdatesServiceRepository
  {
    public List<FutureUpdateTemp> GetFutureUpdatesDoneByService(DateTime ichBillingPeriod, DateTime achBillingPeriod)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(FutureUpdatesServiceConstants.IchBillingPeriodParameterName, typeof(DateTime)) { Value = ichBillingPeriod };
      parameters[1] = new ObjectParameter(FutureUpdatesServiceConstants.AchBillingPeriodParameterName, typeof(DateTime)) { Value = achBillingPeriod };
      parameters[2] = new ObjectParameter(FutureUpdatesServiceConstants.ChangeEffectiveDateParameterName, typeof(DateTime)) { Value = DateTime.UtcNow };
      
      var list = ExecuteStoredFunction<FutureUpdateTemp>(FutureUpdatesServiceConstants.ProcDoMemberFutureUpdatesFunctionName, parameters);
      return list.ToList();
    }

    public String GetRelationIdDisplayName(int futureUpdateId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(FutureUpdatesServiceConstants.FutureUpdateId, typeof(int)) { Value = futureUpdateId };
      parameters[1] = new ObjectParameter(FutureUpdatesServiceConstants.RelationIdDisplayName, typeof(string));
      ExecuteStoredProcedure(FutureUpdatesServiceConstants.ProcGetRelationIdDisplayName, parameters);

      return parameters[1].Value.ToString();

      
    }
  }
}
