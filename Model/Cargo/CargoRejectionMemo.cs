using System;
using System.Collections.Generic;
using Iata.IS.Model.Cargo.Base;
using Iata.IS.Model.Cargo.Enums;
using TransactionStatus = Iata.IS.Model.Common.TransactionStatus;


namespace Iata.IS.Model.Cargo
{
  public class CargoRejectionMemo : MemoBase
  {
    public int RejectionStage { get; set; }

    public string YourRejectionNumber { get; set; }

    public string RejectionMemoNumber { get; set; }

    public string YourBillingMemoNumber { get; set; }

    public decimal? BilledTotalWeightCharge { get; set; }

    public decimal? AcceptedTotalWeightCharge { get; set; }

    public decimal? TotalWeightChargeDifference { get; set; }

    public decimal? BilledTotalValuationCharge { get; set; }

    public decimal? AcceptedTotalValuationCharge { get; set; }

    public decimal? TotalValuationChargeDifference { get; set; }

    public decimal? BilledTotalOtherChargeAmount { get; set; }

    public decimal? AcceptedTotalOtherChargeAmount { get; set; }

    public decimal? TotalOtherChargeDifference { get; set; }

    public decimal? AllowedTotalIscAmount { get; set; }

    public decimal? AcceptedTotalIscAmount { get; set; }

    public decimal? TotalIscAmountDifference { get; set; }

    public decimal? BilledTotalVatAmount { get; set; }

    public decimal? AcceptedTotalVatAmount { get; set; }

    public double? TotalVatAmountDifference { get; set; }
    
    public decimal? TotalNetRejectAmount { get; set; }

    public string ReasonCode { get; set; }

    public string ReasonCodeDescription { get; set; }

    public bool? IsLinkingSuccessful { get; set; }

    public bool? IsBreakdownAllowed { get; set; }

    public bool? IsFromBillingHistory { get; set; }

    public decimal? CurrencyConversionFactor { get; set; }

    public List<CgoRejectionMemoAttachment> Attachments { get; set; }

    public List<RMAwb> CouponBreakdownRecord { get; set; }

    public List<CgoRejectionMemoVat> RejectionMemoVat { get; set; }

    public string IsRejectionFlag { get; set; }

    public Guid? CorrespondenceId { get; set; }

    public bool CouponAwbBreakdownMandatory { get; set; }

    public CargoCorrespondence Correspondence { get; set; }

    public List<CargoCorrespondence> Correspondences { get; set; }
   
    public BMCMIndicator BMCMIndicator
    {
      get
      {
          return (BMCMIndicatorId == 0 ? BMCMIndicator.None : (BMCMIndicator)BMCMIndicatorId);
      }
      set
      {
        BMCMIndicatorId = Convert.ToInt32(value);
      }
    }

    public int BMCMIndicatorId { get; set; }

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

      // CMP#650
    public string YourReasonCode { get; set; }

    

    public CargoRejectionMemo()
    {
      CouponBreakdownRecord = new List<RMAwb>();
      Attachments = new List<CgoRejectionMemoAttachment>();
      RejectionMemoVat = new List<CgoRejectionMemoVat>();
    }
  }
}
