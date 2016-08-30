using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Cargo
{
  public class CgoCreditMemoDetailReport : CargoCreditMemo
  {
    public string CurrencyOfCm { get; set; }
    public string From { get; set; }
    public string To { get; set; }
    public string InvoiceNumber { get; set; }
    public int BillingMonth { get; set; }
    public int BillingPeriod { get; set; }
    public int BillingYear { get; set; }
    //CMP#648: Convert Exchange rate into nullable field.
    public decimal? ExchangeRate { get; set; }
    public double TotalVatAmountReport { get; set; }
    public string DisplayBillingMonthYear { get; set; }
    public string DisplayYourInvoiceBillingMonthYearReport { get; set; }
    public string ReportGenerationDate { get; set; }
    public double AwbOtherChargesBreakdownVatAmoutnSum { get; set; }
  }
}
