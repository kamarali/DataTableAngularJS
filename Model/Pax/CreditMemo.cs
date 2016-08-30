using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Model.Pax
{
  public class CreditMemo : MemoBase
  {
    public int SourceCodeId { get; set; }

    public SourceCode SourceCode { get; set; }

    public long CorrespondenceRefNumber { get; set; }

    public long? FimNumber { get; set; }

    public decimal TaxAmount { get; set; }

    public string CreditMemoNumber { get; set; }

    public string ReasonCode { get; set; }

    public string ReasonCodeDescription { get; set; }

    public decimal NetAmountCredited { get; set; }

    public decimal TotalGrossAmountCredited { get; set; }

    public decimal TotalIscAmountCredited { get; set; }

    public decimal TotalOtherCommissionAmountCredited { get; set; }

    public decimal TotalUatpAmountCredited { get; set; }

    public double TotalHandlingFeeCredited { get; set; }

    public decimal VatAmount { get; set; }

    public List<CreditMemoVat> VatBreakdown { get;  set; }

    public List<CreditMemoAttachment> Attachments { get; set; }

    public List<CMCoupon> CouponBreakdownRecord { get;  set; }

    public bool CouponAwbBreakdownMandatory { get; set; }

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

    public CreditMemo()
    {
      CouponBreakdownRecord = new List<CMCoupon>();
      VatBreakdown = new List<CreditMemoVat>();
      Attachments = new List<CreditMemoAttachment>();
    }
  }
}
