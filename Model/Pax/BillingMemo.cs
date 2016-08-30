using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Model.Pax
{
  public class BillingMemo : MemoBase
  {
    public BillingMemo()
    {
      CouponBreakdownRecord = new List<BMCoupon>();
      Attachments = new List<BillingMemoAttachment>();
      VatBreakdown = new List<BillingMemoVat>();
    }

    public long CorrespondenceRefNumber { get; set; }

    public long? FimNumber { get; set; }

    public decimal TaxAmountBilled { get; set; }

    public string BillingMemoNumber { get; set; }

    public string ReasonCode { get; set; }

    public decimal NetAmountBilled { get; set; }

    public int SourceCodeId { get; set; }

    public decimal TotalGrossAmountBilled { get; set; }

    public decimal TotalIscAmountBilled { get; set; }

    public decimal TotalOtherCommissionAmount { get; set; }

    public decimal TotalUatpAmountBilled { get; set; }

    public double TotalHandlingFeeBilled { get; set; }

    public decimal TotalVatAmountBilled { get; set; }

    public List<BillingMemoVat> VatBreakdown { get; set; }

    public List<BillingMemoAttachment> Attachments { get; set; }

    public List<BMCoupon> CouponBreakdownRecord { get; set; }

    public bool CouponAwbBreakdownMandatory { get; set; }

    private string _resonCodeDesription = string.Empty;
    public string ReasonCodeDescription
    {
      get
      {
        return _resonCodeDesription;
      }
      set
      {
        _resonCodeDesription = value;
      }
    }

    /// <summary>
    /// ErrorCorrectable = 1, ErrorNonCorrectable = 2,Validated = 3
    /// </summary>
    public int TransactionStatusId { set; get; }

    public TransactionStatus TransactionStatus
    {
      get
      {
        return (TransactionStatus)TransactionStatusId;
      }
      set
      {
        TransactionStatusId = Convert.ToInt32(value);
      }
    }
    public decimal NetBilledAmount { get; set; }
  }
}
