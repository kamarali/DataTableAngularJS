using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Pax.Sampling
{
  public class SamplingFormEDetail : EntityBase<Guid>
  {
    public decimal GrossTotalOfUniverse { get; set; }

    public decimal GrossTotalOfUaf { get; set; }

    public decimal UniverseAdjustedGrossAmount { get; set; }

    public decimal GrossTotalOfSample { get; set; }

    public decimal GrossTotalOfUafSampleCoupon { get; set; }

    public decimal SampleAdjustedGrossAmount { get; set; }

    public double SamplingConstant { get; set; }

    public decimal TotalOfGrossAmtXSamplingConstant { get; set; }

    public decimal TotalOfIscAmtXSamplingConstant { get; set; }

    public decimal TotalOfOtherCommissionAmtXSamplingConstant { get; set; }

    public decimal UatpCouponTotalXSamplingConstant { get; set; }

    public double HandlingFeeTotalAmtXSamplingConstant { get; set; }

    public decimal TaxCouponTotalsXSamplingConstant { get; set; }

    public decimal VatCouponTotalsXSamplingConstant { get; set; }

    public decimal NetAmountDue { get; set; }

    public decimal NetAmountDueInCurrencyOfBilling { get; set; }

    public decimal ProvisionalFormBGrossBilled { get; set; }

    public decimal ProvisionalFormBTaxAmount { get; set; }

    public decimal ProvisionalFormBIscAmount { get; set; }

    public decimal ProvisionalFormBOtherCommissionAmount { get; set; }

    public decimal ProvisionalFormBUatpAmount { get; set; }

    public double ProvisionalFormBHandlingFeeAmountBilled { get; set; }

    public decimal ProvisionalFormBVatAmountBilled { get; set; }

    public decimal TotalAmountFormB { get; set; }

    public decimal NetBilledCreditedAmount { get; set; }

    // Commented by Tushar
    public int NumberOfBillingRecords { get; set; }

    public long TotalNumberOfRecords { get; set; }

    public long NumberOfChildRecords { get; set; }

    public double VatAmount { get; set; }

    public PaxInvoice Invoice { get; set; }

    public Guid InvoiceId { get; set; }

    public decimal TotalGrossValue { get; set; }

    public decimal TotalTaxAmount { get; set; }

    public decimal TotalIscAmount { get; set; }

    public decimal TotalOtherCommission { get; set; }

    public decimal TotalUatpAmount { get; set; }

    public double TotalHandlingFee { get; set; }

    public decimal TotalVatAmount { get; set; }

  }
}
