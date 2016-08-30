using System.ComponentModel;

namespace Iata.IS.Model.Reports.Pax
{
  public class PaxSamplingFormCListingReport
  {
    [DisplayName("Serial No")]
    public string SerialNo { get; set; }

    [DisplayName("Billing Airline Code")]
    public string BillingAirlineCode { get; set; }

    [DisplayName("Billed Airline Code")]
    public string BilledAirlineCode { get; set; }

    [DisplayName("NIL Form C Indicator")]
    public string NilFormCIndicator { get; set; }

    [DisplayName("Currency of Listing")]
    public string CurrencyOfListing { get; set; }

    [DisplayName("Provisional Billing Month/Year")]
    public string ProvisionalBillingMonthYear { get; set; }

    [DisplayName("Issuing Airline")]
    public string IssuingAirline { get; set; }

    [DisplayName("Document No")]
    public string DocumentNo { get; set; }

    [DisplayName("Cpn No")]
    public string CouponNo { get; set; }

    [DisplayName("Provisional Invoice No")]
    public string ProvisionalInvoiceNo { get; set; }

    [DisplayName("Batch No")]
    public string BatchNo { get; set; }

    [DisplayName("Seq No")]
    public string SequenceNo { get; set; }

    [DisplayName("Provisional Gross Amount/ALF")]
    public string ProvisionalGrossAmountAlf { get; set; }

    [DisplayName("Reason Code")]
    public string ReasonCode { get; set; }

    [DisplayName("Additional Remarks")]
    public string AdditionalRemarks { get; set; }
  }
}