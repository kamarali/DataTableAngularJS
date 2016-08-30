using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.Cargo
{
  public class CargoRmBmCmSummaryPayablesReport
  {
    /// <summary>
    /// Billing Year - Month
    /// </summary>
    [DisplayName("Billing Month")]
    public string BillingMonth { get; set; }

    /// <summary>
    /// Period No
    /// </summary>
    [DisplayName("Period No")]
    public int PeriodNo { get; set; }

    /// <summary>
    /// Settlement Method
    /// </summary>
    [DisplayName("Settlement Method")]
    public string SettlementMethod { get; set; }

    /// <summary>
    /// Billing Member Code
    /// </summary>
    [DisplayName("Billing Member Code")]
    public string AirlineCode { get; set; }

    /// <summary>
    /// Invoice Number
    /// </summary>
    [DisplayName("Invoice Number")]
    public string InvoiceNumber { get; set; }

    /// <summary>
    /// Memo Type
    /// </summary>
    [DisplayName("Memo Type")]
    public string MemoType { get; set; }

    /// <summary>
    /// Memo Number
    /// </summary>
    [DisplayName("Memo Number")]
    public string MemoNumber { get; set; }

    /// <summary>
    /// Stage
    /// </summary>
    [DisplayName("Stage")]
    public int Stage { get; set; }

    /// <summary>
    /// Reason Code
    /// </summary>
    [DisplayName("Reason Code")]
    public string ReasonCode { get; set; }

    /// <summary>
    /// Currency Code
    /// </summary>
    [DisplayName("Currency Code")]
    public string CurrencyCode { get; set; }

    /// <summary>
    /// Total Weight Charges (Rejected /Billed / Credited)
    /// </summary>
    [DisplayName("Total Weight Charges (Rejected /Billed / Credited)")]
    public decimal WeightCharges { get; set; }

    /// <summary>
    /// Total Valuation Charges (Rejected/ Billed / Credited)
    /// </summary>
    [DisplayName("Total Valuation Charges (Rejected/ Billed / Credited)")]
    public decimal ValuationCharges { get; set; }

    /// <summary>
    /// Total Other Charge Amount (Rejected  /Billed / Credited)
    /// </summary>
    [DisplayName("Total Other Charge Amount (Rejected  /Billed / Credited)")]
    public decimal OtherChargeAmount { get; set; }

    /// <summary>
    /// Total ISC Amount (Rejected /Billed / Credited)
    /// </summary>
    [DisplayName("Total ISC Amount (Rejected /Billed / Credited)")]
    public decimal IscAmount { get; set; }

    /// <summary>
    /// Total VAT Amount (Rejected / Billed / Credited)
    /// </summary>
    [DisplayName("Total VAT Amount (Rejected / Billed / Credited)")]
    public decimal VatAmount { get; set; }

    /// <summary>
    /// Total Net (Reject /Billed / Credited) Amount
    /// </summary>
    [DisplayName("Total Net (Reject /Billed / Credited) Amount")]
    public decimal NetAmount { get; set; }

    /// <summary>
    /// Number of Linked AWBs
    /// </summary>
    [DisplayName("Number of Linked AWBs")]
    public int NumberofLinkedAwb { get; set; }

    /// <summary>
    /// Attachment Indicator
    /// </summary>
    [DisplayName("Attachment Indicator")]
    public string AttachmentIndicatorOrig { get; set; }
  }
}