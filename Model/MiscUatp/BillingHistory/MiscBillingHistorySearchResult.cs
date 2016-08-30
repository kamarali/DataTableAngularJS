using System;
using System.Linq;
using System.Text.RegularExpressions;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.MiscUatp.BillingHistory
{
  public class MiscBillingHistorySearchResult
  {
    public Guid InvoiceId { get; set; }
    public Guid CorrespondenceId { get; set; }
    public int BillingPeriod { get; set; }
    public int BillingMonth { get; set; }
    public int BillingYear { get; set; }

    public string DisplayBillingPeriod
    {
      get
      {
        return TransactionDate ?? string.Format("{0}-{1}-{2}", BillingYear, BillingMonth, BillingPeriod);
      }
    }

    public int BillingMemberId { get; set; }
    public int CorrInitiatingMember { get; set; }
    public string ClearingHouse { get; set; }
    public string MemberCode { get; set; }
    public string TransactionNumber { get; set; }
    public string TransactionDate { get; set; }
    public int ChargeCategoryId { get; set; }
    public string ChargeCategory { get; set; }
    public string CurrencyCode { get; set; }
    public string TotalNetAmount { get; set; }
    public int RejectionStage { get; set; }
    public long CorrespondenceNumber { get; set; }
    public int CorrespondenceStatusId { get; set; }
    public CorrespondenceStatus CorrespondenceStatus { get; set; }
    public int CorrespondenceSubStatusId { get; set; }
    public CorrespondenceSubStatus CorrespondenceSubStatus { get; set; }
    public bool AuthorityToBill { get; set; }
    public int NoOfDaysToExpire { get; set; }

    //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
    public decimal TotalNetAmt
    {
      get
      {
        //extract decimal (amount) from total net amount
          /* SCP432664 - KAL - Miscellaneous Billing history 
           * Desc: Negative (-) sign is not displayed for credit notes transaction amounts in the Search Result grid.
           * As a fix regEx used below was changed from @"[^0-9\.]+" to @"[^-?0-9\.]+" */
          return Convert.ToDecimal(Regex.Replace(TotalNetAmount, @"[^-?0-9\.]+", string.Empty)); 
      }
    }

    //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
    public string NetAmtCurrency
    {
      //extract currency name from total net amount
      get { return TotalNetAmount.Substring(0,3); }
    }

    public string DisplayCorrespondenceStatus
    {
      get
      {
        return EnumList.GetCorrespondenceStatusDisplayValue((CorrespondenceStatus)CorrespondenceStatusId);
      }
    }

    public string DisplayCorrespondenceSubStatus
    {
      get
      {
        return EnumList.GetCorrespondenceSubStatusDisplayValue((CorrespondenceSubStatus)CorrespondenceSubStatusId);
      }
    }

    public int InvoiceTypeId { get; set; }

    //CMP #655: IS-WEB Display per Location ID
    public string BillingMemberLocation { get; set; }

    public string BilledMemberLocation { get; set; }

    public string DisplayMemberLocation { get; set; }
  }
}
