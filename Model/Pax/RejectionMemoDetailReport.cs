﻿using System;

namespace Iata.IS.Model.Pax
{
  public class SamplingRejectionMemoDetailReport : RejectionMemo
  {
    public string From { get; set; }
    public string To { get; set; }
    public string InvoiceNumber { get; set; }
    public int BillingMonth { get; set; }
    public int BillingPeriod { get; set; }
    public int BillingYear { get; set; }
    public string ProvisionalBillingMonth { get; set; }
    //CMP#648: Convert Exchange rate into nullable field.
    public decimal? ExchangeRate { get; set; }
    public string CurrencyOfRm { get; set; }
    public double TotalVatAmountReport{ get; set; }
    public string DisplayBillingMonthYear { get; set; }
    public string DisplayYourInvoiceBillingMonthYearReport { get; set; }
    public string ReportGenerationDate { get; set; }
  }
}
