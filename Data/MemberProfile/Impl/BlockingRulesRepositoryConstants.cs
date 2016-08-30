using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.MemberProfile.Impl
{
  internal class BlockingRulesRepositoryConstants
  {
    internal const string ValidateBlockingRules = "ValidateBlockingRules";
    internal const string BillingMemberId = "BILLING_MEMBER_ID_I";
    internal const string BilledMemberId = "BILLED_MEMBER_ID_I";
    internal const string BillingCategoryType = "BILLING_CATEGORY_ID_I";
    internal const string Clearinghouse = "CLEARING_HOUSE_I";
    internal const string BillingMemberZoneId = "BILLING_MEMBER_ZONE_ID_I";
    internal const string BilledMemberZoneId = "BILLED_MEMBER_ZONE_ID_I";
    internal const string IsCreditorBlocked = "IS_CREDITOR_BLOCKED_O";
    internal const string IsDebitorBlocked = "IS_DEBITOR_BLOCKED_O";
    internal const string IsCreditorGroupBlocked = "IS_CREDITOR_GROUP_BLOCKED_O";
    internal const string IsDebitorGroupBlocked = "IS_DEBITOR_GROUP_BLOCKED_O";

    internal const string GetBlokingRulesForClearingHouse = "GetBlokingRulesForClearingHouse";
    internal const string ClearingHouseInput = "CLEARING_HOUSE_I";
  }
}
