namespace Iata.IS.Model.Common
{
  // Review: Do we need this class?
  public class FileTotal
  {

    public int BilledAirline { get; set; } //Added this field to assign Billed Airline code

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public int ProvisionalAdjustmentRate { get; set; }

    public decimal TotalGrossValue { get; set; }

    public decimal TotalInterlineServiceChargeAmount { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal NetTotal { get; set; }

    public decimal NetBillingAmount { get; set; }

    public int NoOfBillingRecords { get; set; }

    public double HandlingFeeAmount { get; set; }

    public decimal TotalOtherCommissionAmount { get; set; }

    public decimal TotalUatpAmount { get; set; }

    public decimal TotalVatAmount { get; set; }

    public int TotalNumberOfRecords { get; set; }

    //--------------------CGO-------------------------------
      public decimal TotalWeightCharges { get; set; }

      public decimal TotalOtherCharges { get; set; }

      public decimal TotalValuationCharges { get; set; }

  }
}
