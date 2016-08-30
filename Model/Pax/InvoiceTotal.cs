using Iata.IS.Model.Pax.Base;

namespace Iata.IS.Model.Pax
{
  public class InvoiceTotal : InvoiceTotalBase
  {
    public double ProvAdjustmentRate { get; set; }

    public decimal TotalGrossValue { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal NetAmountAfterSamplingConstant { get; set; }

    public decimal TotalVatAmountAfterSamplingConstant { get; set; }

    public decimal TotalProvisionalAdjustmentAmount { get; set; }

    public decimal TotalIscAmount { get; set; }

    public decimal TotalOtherCommission { get; set; }

    public decimal TotalUatpAmount { get; set; }

    public double TotalHandlingFee { get; set; }

    public double SamplingConstant { get; set; }

    public int BatchSequenceNumber { get; set; }

    public int RecordSequenceWithinBatch { get; set; }

    public double FareAbsorptionPercent { get; set; }

    public decimal FareAbsorptionAmount { get; set; }

    public double IscAbsorptionPercent { get; set; }

    public decimal IscAbsorptionAmount { get; set; }

    public double TaxAbsorptionPercent { get; set; }

    public decimal TaxAbsorptionAmount { get; set; }

    public double UatpAbsorptionPercent { get; set; }

    public decimal UatpAbsorptionAmount { get; set; }

    public double HandlingFeeAbsorptionPercent { get; set; }

    public double HandlingFeeAbsorptionAmount { get; set; }

    public double OtherCommissionAbsorptionPercent { get; set; }

    public decimal OtherCommissionAbsorptionAmount { get; set; }

    public double VatAbsorptionPercent { get; set; }

    public decimal VatAbsorptionAmount { get; set; }

    //The field is not stored in database but it will be used to validate the data coming from IDEC.
    public int TotalNoOfRecords { get; set; }

    //public Guid InvoiceId { get; set; }

    public PaxInvoice Invoice { get; set; }
  }
}
