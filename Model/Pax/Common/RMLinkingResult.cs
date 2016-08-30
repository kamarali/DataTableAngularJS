using System;
using System.Collections.Generic;

namespace Iata.IS.Model.Pax.Common
{
  public class RMLinkingResultDetails
  {
    public bool IsLinkingSuccessful { get; set; }
    public List<RMLinkingRecords> Records { get; set; }
    public RMLinkingAmount MemoAmount { get; set; }
    public string ErrorMessage { get; set; }
    public decimal CurrencyConversionFactor { get; set; }

    public bool HasBreakdown { get; set; }

    public RMLinkingResultDetails()
    {
      Records = new List<RMLinkingRecords>();
    }
  }

  public class RMLinkingRecords
  {
    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public Guid MemoId { get; set; }

    public RMLinkingRecords()
    {
    }
  }

  public class RMLinkingAmount
  { 
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

    public double TotalNetRejectAmount { get; set; }
  }
}
