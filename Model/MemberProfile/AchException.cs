using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class AchException : EntityBase<int>
  {
    public int BillingCategoryId { get; set; }

    public BillingCategoryType BillingCategory
    {
      get
      {
        return (BillingCategoryType)BillingCategoryId;
      }
      set
      {
        BillingCategoryId = Convert.ToInt32(value);
      }
    }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public Member ExceptionMember { get; set; }

    public string ExceptionMemberCommercialName { get; set; }

    public int ExceptionMemberId { get; set; }
  }
}
