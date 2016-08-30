using System.ComponentModel;

namespace Iata.IS.Model.Reports.Pax
{
  public class PaxCouponListingReport
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

    [DisplayName("Sequence No")]
    public string SequenceNo { get; set; }

    [DisplayName("Issuing Airline")]
    public string IssuingAirline { get; set; }

    [DisplayName("Cpn No")]
    public string CouponNo { get; set; }

    [DisplayName("Document/Form/Serial/FIM No")]
    public string DocumentNo { get; set; }

    [DisplayName("Check Digit")]
    public string CheckDigit { get; set; }

    [DisplayName("From Airport")]
    public string FromAirport { get; set; }

    [DisplayName("To Airport")]
    public string ToAirport { get; set; }

    [DisplayName("Currency of Listing")]
    public string CurrencyOfListing { get; set; }

    [DisplayName("Gross Fare Value")]
    public string GrossFareValue { get; set; }

    [DisplayName("E Ticket Indicator")]
    public string ETicketIndicator { get; set; }

    [DisplayName("Orig PMI")]
    public string OriginalPmi { get; set; }

    [DisplayName("Agmt Indc Supp")]
    public string AgreementIndicatorSupplied { get; set; }

    [DisplayName("ISC Rate")]
    public string IscRate { get; set; }

    [DisplayName("ISC Amount")]
    public string IscAmount { get; set; }

    [DisplayName("Other Comm Rate")]
    public string OtherCommissionRate { get; set; }

    [DisplayName("Other Comm Amount")]
    public string OtherCommissionAmount { get; set; }

    [DisplayName("UATP Rate")]
    public string UatpRate { get; set; }

    [DisplayName("UATP Amount")]
    public string UatpAmount { get; set; }

    [DisplayName("Handling Fee Amount")]
    public string HandlingFeeAmount { get; set; }

    [DisplayName("Curr Adj Ind")]
    public string CurrencyAdjustmentIndicator { get; set; }

    [DisplayName("Tax Amount")]
    public string TaxAmount { get; set; }

    [DisplayName("VAT Amount")]
    public string VatAmount { get; set; }

    [DisplayName("Coupon Total Amount")]
    public string CouponTotalAmount  { get; set; }

  }
}