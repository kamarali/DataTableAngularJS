using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
  public interface IFutureUpdatesRepository : IRepository<FutureUpdates>
  {
    List<FutureUpdateDetails> GetAuditTrialList(DateTime? fromDateOrPeriod, DateTime? toDateOrPeriod, int user, string groupList, int userType, int isDateOrPeriodSearch, int memberId, string reportType);

    /// <summary>
    /// Get user list for given user category and member id.
    /// </summary>
    /// <param name="userCategoryId">user category id</param>
    /// <param name="memberId"> member id</param>
    /// <returns>list of audit trail users corresponding to given user category id and member id.</returns>
    List<AuditTrailUserDetails> GetAuditTrailUserList(int userCategoryId, int memberId);
  }
}
