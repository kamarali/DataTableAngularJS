using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax.Sampling;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class BlockingRulesRepository : Repository<BlockingRule>, IBlockingRulesRepository
  {
    public override IQueryable<BlockingRule> Get(System.Linq.Expressions.Expression<Func<BlockingRule, bool>> where)
    {
      var blockingRulesList = EntityObjectSet.Include("Member").Where(where);

      return blockingRulesList;
    }

    public override BlockingRule Single(System.Linq.Expressions.Expression<Func<BlockingRule, bool>> where)
    {
      var blockingRulesList = EntityObjectSet.Include("Member").SingleOrDefault(where);

      return blockingRulesList;
    }

    public IQueryable<BlockingRule> GetBlockingRuleWithGroup(System.Linq.Expressions.Expression<Func<BlockingRule, bool>> where)
    {
      var blockingRulesList = EntityObjectSet.Include("BlockedGroups").Where(where);

      return blockingRulesList;
    }

    public void ValidateBlockingRules(int billingMemberId, int billedMemberId, BillingCategoryType billingCategoryType, string smiValue, int billingMemberZoneId,
      int billedMemberZoneId, out bool isCreditorBlocked, out bool isDebitorBlocked, out bool isCreditorGroupBlocked, out bool isDebitorGroupBlocked)
    {
        
      var parameters = new ObjectParameter[10];
      parameters[0] = new ObjectParameter(BlockingRulesRepositoryConstants.BillingMemberId, typeof(int)) { Value = billingMemberId };
      parameters[1] = new ObjectParameter(BlockingRulesRepositoryConstants.BilledMemberId, typeof(int)) { Value = billedMemberId };
      parameters[2] = new ObjectParameter(BlockingRulesRepositoryConstants.BillingCategoryType, typeof(int)) { Value = (int)billingCategoryType };
      parameters[3] = new ObjectParameter(BlockingRulesRepositoryConstants.Clearinghouse,typeof(string)) {Value = smiValue};
      parameters[4] = new ObjectParameter(BlockingRulesRepositoryConstants.BillingMemberZoneId, typeof(int)) { Value = billingMemberZoneId };
      parameters[5] = new ObjectParameter(BlockingRulesRepositoryConstants.BilledMemberZoneId, typeof(int)) { Value = billedMemberZoneId };

      parameters[6] = new ObjectParameter(BlockingRulesRepositoryConstants.IsCreditorBlocked, typeof(int));
      parameters[7] = new ObjectParameter(BlockingRulesRepositoryConstants.IsDebitorBlocked, typeof(int));
      parameters[8] = new ObjectParameter(BlockingRulesRepositoryConstants.IsCreditorGroupBlocked, typeof(int));
      parameters[9] = new ObjectParameter(BlockingRulesRepositoryConstants.IsDebitorGroupBlocked, typeof(int));
      ExecuteStoredProcedure(BlockingRulesRepositoryConstants.ValidateBlockingRules, parameters);

      isCreditorBlocked = int.Parse(parameters[6].Value.ToString()) == 0 ? false : true;
      isDebitorBlocked = int.Parse(parameters[7].Value.ToString()) == 0 ? false : true;
      isCreditorGroupBlocked = int.Parse(parameters[8].Value.ToString()) == 0 ? false : true;
      isDebitorGroupBlocked = int.Parse(parameters[9].Value.ToString()) == 0 ? false : true;
    }

    /// <summary>
    /// Get all active blocking rules data for given clearing house.
    /// </summary>
    /// <param name="clearingHouse">clearing house string. e.g. ICH/ACH.</param>
    /// <returns>All active blocking rules data for given clearing house.</returns>
    public IEnumerable<DownloadBlockingRules> GetBlokingRulesForClearingHouse(string clearingHouse)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(BlockingRulesRepositoryConstants.ClearingHouseInput, typeof(string)) { Value = clearingHouse };
      return ExecuteStoredFunction<DownloadBlockingRules>(BlockingRulesRepositoryConstants.GetBlokingRulesForClearingHouse,parameters);
    }// End GetBlokingRulesForClearingHouse()
  }
}
