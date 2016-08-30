using System;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Enums;
using System.Text.RegularExpressions;

namespace Iata.IS.Model.Cargo.BillingHistory
{
  public class CargoBillingHistorySearchResult
  {
    private string _transactionNumber;

    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public Guid TransactionId { get; set; }
    public int BillingPeriod { get; set; }
    public int BillingMonth { get; set; }
    public int BillingYear { get; set; }
    public int? BillingCodeId { get; set; }
    public int SettlementMethodId { get; set; }

    public string DisplayBillingPeriod
    {
      get
      {
        return TransactionDate ?? string.Format("{0}-{1}-{2}", BillingYear, BillingMonth, BillingPeriod);
      }
    }

    public string ReasonCode { get; set; }
    public int BillingMemberId { get; set; }
    public int CorrInitiatingMember { get; set; }
    public string ClearingHouse { get; set; }
    public string MemberCode { get; set; }
    public string TransactionNumber
    {
      get
      {
        return string.IsNullOrEmpty(TransactionType) ? string.Format("{0:00000000000}", Convert.ToInt64(_transactionNumber)) : _transactionNumber;
      }
      set
      {
        _transactionNumber = value;
      }
    }
    public string TransactionType { get; set; }
    public string DisplayTransactionType { 
      get
      {
        switch (TransactionType)
        {
          case "RM" :
            return "Rejection Memo";
          case "BM" :
            return "Billing Memo";
          case "CM" :
            return "Credit Memo";
          case "PC":
            return "Prime Coupon";
          case "FD":
            return "Form D Coupon";
          default:
            return string.Empty;
        }
      }
    }
    public string TransactionDate { get; set; }
    public int ChargeCategoryId { get; set; }
    public string ChargeCategory { get; set; }
    public string CurrencyCode { get; set; }
    public string TotalNetAmount { get; set; }
    public int? RejectionStage { get; set; }
    public long? CorrespondenceNumber { get; set; }
    public int CorrespondenceStatusId { get; set; }
    public CorrespondenceStatus CorrespondenceStatus { get; set; }
    public int CorrespondenceSubStatusId { get; set; }
    public CorrespondenceSubStatus CorrespondenceSubStatus { get; set; }
    public bool? AuthorityToBill { get; set; }
    public int? NoOfDaysToExpire { get; set; }

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

    //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
    public decimal TotalNetAmountValue
    {
      get
      {
        //extract decimal (amount) from total net amount
        var tna = Regex.Replace(TotalNetAmount, @"[^0-9\.\+\-]+", string.Empty).Trim();
        return Convert.ToDecimal(string.IsNullOrEmpty(tna) ? "0" : tna);
      }
    }
    public string TotalNetAmountCurrency
    {
      //extract currency name from total net amount
      get { return string.IsNullOrEmpty(TotalNetAmount.Trim()) ? "" : TotalNetAmount.Trim().Substring(0, 3); }
    }
  }
}
