using System;
using System.Collections.Generic;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using System.Linq;

namespace Iata.IS.Data.MemberProfile
{
  public interface IBlockingRulesRepository : IRepository<BlockingRule>
  {
    IQueryable<BlockingRule> GetBlockingRuleWithGroup(System.Linq.Expressions.Expression<Func<BlockingRule, bool>> where);

    void ValidateBlockingRules(int billingMemberId, int billedMemberId, BillingCategoryType billingCategoryType, string smiValue, int billingMemberZoneId,
                                               int billedMemberZoneId, out bool isCreditorBlocked, out bool isDebitorBlocked, out bool isCreditorGroupBlocked, out bool isDebitorGroupBlocked);

    IEnumerable<DownloadBlockingRules> GetBlokingRulesForClearingHouse(string clearingHouse);

  }
}
