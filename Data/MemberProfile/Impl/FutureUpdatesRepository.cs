using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;
using System.Data.Objects;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class FutureUpdatesRepository : Repository<FutureUpdates>, IFutureUpdatesRepository
  {
    public override IQueryable<FutureUpdates> Get(System.Linq.Expressions.Expression<Func<FutureUpdates, bool>> where)
    {
      var blockMemberList = EntityObjectSet.Include("Member").Where(where);

      return blockMemberList;
    }

    public List<FutureUpdateDetails> GetAuditTrialList(DateTime? fromDateOrPeriod, DateTime? toDateOrPeriod, int user, string groupList, int userType, int isDateOrPeriodSearch, int memberId,string reportType)
    {
      var parameters = new ObjectParameter[8];

      parameters[0] = new ObjectParameter(FutureUpdatesRepositoryConstants.FromDateOrPeriod, typeof(DateTime)) { Value = fromDateOrPeriod };
      parameters[1] = new ObjectParameter(FutureUpdatesRepositoryConstants.ToDateOrPeriod, typeof(DateTime)) { Value = toDateOrPeriod };
      parameters[2] = new ObjectParameter(FutureUpdatesRepositoryConstants.User, typeof(int)) { Value = user };
      parameters[3] = new ObjectParameter(FutureUpdatesRepositoryConstants.UserType, typeof(int)) { Value = userType };
      parameters[4] = new ObjectParameter(FutureUpdatesRepositoryConstants.GroupList, typeof(string)) { Value = groupList };
      parameters[5] = new ObjectParameter(FutureUpdatesRepositoryConstants.IsDateOrPeriodSearch, typeof(int)) { Value = isDateOrPeriodSearch };
      parameters[6] = new ObjectParameter(FutureUpdatesRepositoryConstants.MemberId, typeof(int)) { Value = memberId };
      parameters[7] = new ObjectParameter(FutureUpdatesRepositoryConstants.ReportType, typeof(string)) { Value = reportType };

      var list = ExecuteStoredFunction<FutureUpdateDetails>(FutureUpdatesRepositoryConstants.GetFutureUpdateFunctionName, parameters);
      return list.ToList();

    }

    /// <summary>
    /// Get user list for given user category and member id.
    /// </summary>
    /// <param name="userCategoryId">user category id</param>
    /// <param name="memberId"> member id</param>
    /// <returns>list of audit trail users corresponding to given user category id and member id.</returns>
    public List<AuditTrailUserDetails> GetAuditTrailUserList(int userCategoryId, int memberId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(FutureUpdatesRepositoryConstants.UserCategoryId, typeof(string)) { Value = userCategoryId };
      parameters[1] = new ObjectParameter(FutureUpdatesRepositoryConstants.MemberId, typeof(int)) { Value = memberId };
      var list = ExecuteStoredFunction<AuditTrailUserDetails>(FutureUpdatesRepositoryConstants.GetAuditTrailUserListFunctionName, parameters);
      return list.ToList();
    }
  }
}
