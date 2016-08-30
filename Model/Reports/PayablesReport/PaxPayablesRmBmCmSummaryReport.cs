using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.PayablesReport
{
  public class PaxPayablesRmBmCmSummaryReport
  {
    /// <summary>
    /// Billing Month
    /// </summary>
    [DisplayName("Billing Month")]
    public string BillingMonth { get; set; }

    /// <summary>
    /// Period Number
    /// </summary>
    [DisplayName("Period Number")]
    public int PeriodNo { get; set; }

    /// <summary>
    /// Settlement Method
    /// </summary>
    [DisplayName("Settlement Method")]
    public string SettlementMethod { get; set; }

    /// <summary>
    /// Billed Entity Code
    /// </summary>
    [DisplayName("Billing Entity Code")]
    public string BillingEntityCode { get; set; }

    /// <summary>
    /// Invoice Number
    /// </summary>
    [DisplayName("Invoice Number")]
    public string InvoiceNo { get; set; }

    /// <summary>
    /// Memo Type
    /// </summary>
    [DisplayName("Memo Type")]
    public string MemoType { get; set; }

    /// CMP523 - Source Code in RMBMCM Summary Report
    /// <summary>
    /// Source Code
    /// </summary>
    [DisplayName("Source Code")]
    public int SourceCode { get; set; }

    /// <summary>
    /// Memo Number
    /// </summary>
    [DisplayName("Memo Number")]
    public string MemoNo { get; set; }

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
    /// Total Gross Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total Gross Amount (Rejected/Billed/Credited)")]
    public decimal TotalGrossAmt { get; set; }

    /// <summary>
    /// Total UATP Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total UATP Amount (Rejected/Billed/Credited)")]
    public decimal TotalUatpAmt { get; set; }

    /// <summary>
    /// Total Handling Fee Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total Handling Fee Amount (Rejected/Billed/Credited)")]
    public decimal TotalHandlingFeeAmt { get; set; }

    /// <summary>
    /// Total Other Commision Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total Other Commision Amount (Rejected/Billed/Credited)")]
    public decimal TotalOtherCommAmt { get; set; }

    /// <summary>
    /// Total ISC Amount (Rejected/Billed/Credited) 
    /// </summary>
    [DisplayName("Total ISC Amount (Rejected/Billed/Credited)")]
    public decimal TotalIscAmount { get; set; }

    /// <summary>
    /// Total VAT Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total VAT Amount (Rejected/Billed/Credited)")]
    public decimal TotalVatAmount { get; set; }

    /// <summary>
    /// Total TAX Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total TAX Amount (Rejected/Billed/Credited)")]
    public decimal TotalTaxAmount { get; set; }

    /// <summary>
    /// Total Net Amount (Rejected/Billed/Credited)
    /// </summary>
    [DisplayName("Total Net Amount (Rejected/Billed/Credited)")]
    public decimal TotalNetAmount { get; set; }

    /// <summary>
    /// Number of Linked Coupons
    /// </summary>
    [DisplayName("Number of Linked Coupons")]
    public int NoOfLinkedCoupons { get; set; }

    /// <summary>
    /// Attachment Indicator
    /// </summary>
    [DisplayName("Attachment Indicator")]
    public string AttachmentIndicator { get; set; }
  }
}