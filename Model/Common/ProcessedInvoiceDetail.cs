using System.ComponentModel;

namespace Iata.IS.Model.Common
{
  /// This class will be used for details of processed invoice.
  public class ProcessedInvoiceDetail
  {
    public int MemberId { get; set; }

    [DisplayName("Serial No")]
    public int SerialNo { get; set; }

    [DisplayName("Billing Entity Code")]
    public string BillingEntityCode { get; set; }

    [DisplayName("Clearance Month")]
    public string ClearanceMonth { get; set; }

    [DisplayName("Period")]
    public string Period { get; set; }

    [DisplayName("Billed Entity Code")]
    public string BilledEntityCode { get; set; }

    [DisplayName("Billing Category")]
    public string BillingCategory { get; set; }

    [DisplayName("Settlement Method Indicator")]
    public string SettlementMethodIndicator { get; set; }

    [DisplayName("Invoice Number")]
    public string InvoiceNumber { get; set; }

    [DisplayName("Invoice Type")]
    public string InvoiceType { get; set; }

    [DisplayName("Currency of Billing")]
    public string BillingCurrency { get; set; }

    [DisplayName("Invoice Amount in Billing Currency")]
    public decimal InvoiceAmountinBillingCurrency { get; set; }

    [DisplayName("Currency of Clearance")]
    public string ClearanceCurrency { get; set; }

    [DisplayName("Invoice Amount in Clearance Currency")]
    public decimal InvoiceAmountinClearanceCurrency { get; set; }
  }
}
