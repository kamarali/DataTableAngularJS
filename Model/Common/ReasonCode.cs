using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Enums;

namespace Iata.IS.Model.Common
{
  [Serializable]
  public class ReasonCode : MasterBase<int>
  {
    public string Code { get; set; }

    public string Description { get; set; }

    public int BillingCodeId { get; set; }

    public BillingCode BillingCode
    {
      get
      {
        return (BillingCode)BillingCodeId;
      }
      set
      {
        BillingCodeId = Convert.ToInt32(value);
      }
    }

    public int TransactionTypeId { get; set; }

    public bool CouponAwbBreakdownMandatory { get; set; }

    public bool BilateralCode { get; set; }

    public TransactionType TransactionType { get; set; }

    public string TransactionTypeName
    {
        get { return TransactionType!=null?this.TransactionType.Name:string.Empty; }
    }
  }

  //SCP 121308 : Reason Codes in PAX billing history screen appear multiple times.
  public class ReasonCodeComparer : IEqualityComparer<ReasonCode>
  {
    public bool Equals(ReasonCode x, ReasonCode y)
    {
      if (x.Code == y.Code && x.Description == y.Description)
        return true;
      return false;
    }

    public int GetHashCode(ReasonCode a)
    {
      string z = a.Code + a.Description;
      return (z.GetHashCode());
    }
  }

}
