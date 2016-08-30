using System.ComponentModel;

namespace Iata.IS.Model.Reports.Pax
{
  public class PaxNonSamplingRmBmCmListingReport
  {
    [DisplayName("Serial No")]
    public string SerialNo { get; set; }

    [DisplayName("Billing Airline Code")]
    public string BillingAirlineCode { get; set; }

    [DisplayName("Billed Airline Code")]
    public string BilledAirlineCode { get; set; }

    [DisplayName("Invoice Number")]
    public string InvoiceNumber { get; set; }

    [DisplayName("Billing Month/Year")]
    public string BillingMonthYear { get; set; }

    [DisplayName("Period No")]
    public string PeriodNo { get; set; }

    [DisplayName("Sample/Non Sample")]
    public string SampleOrNonSample { get; set; }

    [DisplayName("Source Code")]
    public string SourceCode { get; set; }

    [DisplayName("Batch No")]
    public string BatchNo { get; set; }

    [DisplayName("Seq No")]
    public string SequenceNo { get; set; }

    [DisplayName("Rejection Memo/Billing Memo/Credit Memo")]
    public string MemoNumber { get; set; }

    [DisplayName("Currency of Listing")]
    public string CurrencyOfListing { get; set; }

    [DisplayName("Gross Fare Value")]
    public string GrossFareValue { get; set; }

    [DisplayName("ISC Amount")]
    public string IscAmount { get; set; }

    [DisplayName("Other Comm Amount")]
    public string OtherCommissionAmount { get; set; }

    [DisplayName("UATP Amount")]
    public string UatpAmount { get; set; }

    [DisplayName("Handling Fee Amount")]
    public string HandlingFeeAmount { get; set; }

    [DisplayName("Tax Amount")]
    public string TaxAmount { get; set; }

    [DisplayName("VAT Amount")]
    public string VatAmount { get; set; }

    [DisplayName("Net Reject/ Net Credit Amount")]
    public string NetRejectCreditAmount { get; set; }

  }
}