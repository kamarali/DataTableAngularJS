using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class DSRequiredCountrymapping : EntityBase<string>
  {
   // public char DsReqCountryId { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public Country Country { get; set; }

    public string CountryId { get; set; }

    public int BillingTypeId { get; set; }

    public BillingType BillingType
    {
      get
      {
        return (BillingType)BillingTypeId;
      }
      set
      {
        BillingTypeId = Convert.ToInt32(value);
      }
    }


  }
}
