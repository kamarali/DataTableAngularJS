using System;
using System.Collections.Generic;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Base;
using Iata.IS.Model.Pax.Enums;
using TransactionStatus = Iata.IS.Model.Common.TransactionStatus;

namespace Iata.IS.Model.Pax
{
  public class RejectionMemo : MemoBase
  {
    public int SourceCodeId { get; set; }

    public int RejectionStage { get; set; }

    public string YourRejectionNumber { get; set; }

    public string RejectionMemoNumber { get; set; }

    public string FimBMCMNumber { get; set; }

    public double SamplingConstant { get; set; }

    public decimal TotalNetRejectAmountAfterSamplingConstant { get; set; }

    public string IsRejectionFlag { get; set; }

    public decimal TotalNetRejectAmount { get; set; }

    public double TotalGrossAmountBilled { get; set; }

    public double TotalGrossAcceptedAmount { get; set; }

    public double TotalGrossDifference { get; set; }

    public double TotalTaxAmountBilled { get; set; }

    public double TotalTaxAmountAccepted { get; set; }

    public double TotalTaxAmountDifference { get; set; }

    public double TotalVatAmountBilled { get; set; }

    public double TotalVatAmountAccepted { get; set; }

    public double TotalVatAmountDifference { get; set; }

    public double AllowedIscAmount { get; set; }

    public double AcceptedIscAmount { get; set; }

    public double IscDifference { get; set; }

    public double AllowedOtherCommission { get; set; }

    public double AcceptedOtherCommission { get; set; }

    public double OtherCommissionDifference { get; set; }

    public double AllowedUatpAmount { get; set; }

    public double AcceptedUatpAmount { get; set; }

    public double UatpAmountDifference { get; set; }

    public double AllowedHandlingFee { get; set; }

    public double AcceptedHandlingFee { get; set; }

    public double HandlingFeeAmountDifference { get; set; }

    public string ReasonCode { get; set; }

    public string ReasonCodeDescription { get; set; }

    public Correspondence Correspondence { get; set; }

    //Added for the display of multiple correspondences in Audit trail.
    public List<Correspondence> Correspondences { get; set; }

    public Guid? CorrespondenceId { get; set; }

    public List<RejectionMemoAttachment> Attachments { get; set; }

    public List<RMCoupon> CouponBreakdownRecord { get; set; }

    public List<RejectionMemoVat> RejectionMemoVat { get;  set; }

    public bool CouponAwbBreakdownMandatory { get; set; }

    public bool? IsLinkingSuccessful { get; set; }

    public bool? IsBreakdownAllowed { get; set; }

    public bool? IsFromBillingHistory { get; set; }

    public decimal? CurrencyConversionFactor { get; set; }

    public FIMBMCMIndicator FIMBMCMIndicator
    {
      get
      {
        return (FIMBMCMIndicatorId == 0 ? FIMBMCMIndicator.None : (FIMBMCMIndicator)FIMBMCMIndicatorId);
      }
      set
      {
        FIMBMCMIndicatorId = Convert.ToInt32(value);
      }
    }

    public int FIMBMCMIndicatorId { get; set; }

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

    public RejectionMemo()
    {
      CouponBreakdownRecord = new List<RMCoupon>();
      Attachments = new List<RejectionMemoAttachment>();
      RejectionMemoVat = new List<RejectionMemoVat>();
      FIMBMCMIndicator = FIMBMCMIndicator.None;
    }
  }
}
