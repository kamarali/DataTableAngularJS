using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ReceivablesReport
{
    public class PaxInterlineBillingSummaryReportResult
    {
        public string ClearanceMonthYear { get; set; }
        public int PeriodNumber { get; set; }
        public string SettlementMethodIndicator { get; set; }
        public string BillingEntityCode { get; set; }
        public string BillingEntityName { get; set; }
        public string BilledEntityCode { get; set; }
        public string BilledEntityName { get; set; }
        public int TotalNoOfInvoices { get; set; }
        public string CurrencyCode { get; set; }
        public decimal TotalOfPrimeBilling { get; set; }
        public decimal TotalOfRmBilling { get; set; }
        public decimal TotalOfBillingMemo { get; set; }
        public decimal TotalOfCreditMemo { get; set; }
    }
}
