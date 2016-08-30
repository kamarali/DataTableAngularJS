using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Cargo.Base;

namespace Iata.IS.Model.Cargo
{
  public class CargoCreditMemo : MemoBase
  {
      public CargoCreditMemo()
      {
          AWBBreakdownRecord = new List<CMAirWayBill>();
          VatBreakdown = new List<CargoCreditMemoVat>();
          Attachments = new List<CargoCreditMemoAttachment>();
      }
    public long CorrespondenceRefNumber { get; set; }
       
    //YOUR_INVOICE_BILLING_DATE 

    public string CreditMemoNumber { get; set; }

    public string ReasonCode { get; set; }

    public string ReasonCodeDescription { get; set; }

    public decimal? TotalWeightCharges { get; set; }

    public decimal? TotalValuationAmt { get; set; }

    public decimal TotalOtherChargeAmt { get; set; }

    public decimal? NetAmountCredited { get; set; }    

    public decimal TotalIscAmountCredited { get; set; }   

    public decimal? TotalVatAmountCredited { get; set; }

    public List<CargoCreditMemoVat> VatBreakdown { get;  set; }

    public List<CargoCreditMemoAttachment> Attachments { get; set; }

    public List<CMAirWayBill> AWBBreakdownRecord { get;  set; }

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

   
  }
}
