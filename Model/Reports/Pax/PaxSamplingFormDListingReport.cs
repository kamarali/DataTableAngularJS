using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.Pax
{
  public class PaxSamplingFormDListingReport
  {
    [DisplayName("Serial No")]
    public string SerialNo { get; set; }

    [DisplayName("Billing Airline Code")]
    public string BillingAirlineCode { get; set; }

    [DisplayName("Billed Airline Code")]
    public string BilledAirlineCode { get; set; }

    [DisplayName("Invoice Number")]
    public string InvoiceNumber { get; set; }

    [DisplayName("Provisional Billing Month/Year")]
    public string ProvisionalBillingMonthYear { get; set; }

    [DisplayName("Billing Month/Year")]
    public string BillingMonthYear { get; set; }

    [DisplayName("Period No")]
    public string PeriodNo { get; set; }

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

    [DisplayName("Agmt Indc Supp")]
    public string AgreementIndcSupplied { get; set; }

    [DisplayName("Orig PMI")]
    public string OriginalPmi { get; set; }

    [DisplayName("Agmt Indc Val")]
    public string AgreementIndValidated { get; set; }

    [DisplayName("Val PMI")]
    public string ValidatedPmi { get; set; }

    [DisplayName("Provisional Gross Amount/ALF")]
    public string ProvisionalGrossAmt { get; set; }

    [DisplayName("Currency of Listing")]
    public string CurrencyOfListing { get; set; }

    [DisplayName("Gross Fare Value")]
    public string GrossFareValue { get; set; }

    [DisplayName("ISC Rate")]
    public string IscRate { get; set; }

    [DisplayName("ISC Amount")]
    public string IscAmount { get; set; }

    [DisplayName("Other Comm Rate")]
    public string OtherCommRate { get; set; }

    [DisplayName("Other Comm Amount")]
    public string OtherCommAmount { get; set; }

    [DisplayName("UATP Rate")]
    public string UatpRate { get; set; }

    [DisplayName("UATP Amount")]
    public string UatpAmount { get; set; }

    [DisplayName("Handling Fee Amount")]
    public string HandlingFeeAmount { get; set; }

    [DisplayName("Tax Amount")]
    public string TaxAmount { get; set; }

    [DisplayName("VAT Amount")]
    public string VatAmount { get; set; }

    [DisplayName("Coupon Total Amount")]
    public string CouponTotalAmount { get; set; }
  }
}
