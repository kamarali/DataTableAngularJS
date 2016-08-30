using System;
using System.Collections.Generic;

namespace Iata.IS.Model.Cargo.Common
{
  public class CgoRMLinkingResultDetails
  {
    public bool IsLinkingSuccessful { get; set; }
    public List<CgoRMLinkingRecords> Records { get; set; }
    public CgoRMLinkingAmount MemoAmount { get; set; }
    public string ErrorMessage { get; set; }
    public decimal CurrencyConversionFactor { get; set; }
    public bool HasBreakdown { get; set; }
    // CMP#650 Added fields to avoid DB hit
    public string ReasonCode { get; set; }
    
    public CgoRMLinkingResultDetails()
    {
      Records = new List<CgoRMLinkingRecords>();
    }
  }

  public class CgoRMLinkingRecords
  {
    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public Guid MemoId { get; set; }
  }

  public class CgoRMLinkingAmount
  {
    public decimal? BilledTotalWeightCharge { get; set; }

    public decimal? AcceptedTotalWeightCharge { get; set; }

    public decimal? TotalWeightChargeDifference { get; set; }

    public decimal? BilledTotalValuationCharge { get; set; }

    public decimal? AcceptedTotalValuationCharge { get; set; }

    public decimal? TotalValuationChargeDifference { get; set; }

    public decimal BilledTotalOtherChargeAmount { get; set; }

    public decimal AcceptedTotalOtherChargeAmount { get; set; }

    public decimal TotalOtherChargeDifference { get; set; }

    public decimal AllowedTotalIscAmount { get; set; }

    public decimal AcceptedTotalIscAmount { get; set; }

    public decimal TotalIscAmountDifference { get; set; }

    public decimal? BilledTotalVatAmount { get; set; }

    public decimal? AcceptedTotalVatAmount { get; set; }

    public decimal? TotalVatAmountDifference { get; set; }

    public decimal? TotalNetRejectAmount { get; set; }
  }
}
