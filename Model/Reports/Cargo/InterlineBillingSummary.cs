using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.Cargo
{
    public class InterlineBillingSummary
    {
        public string BillingMonthYear { get; set; }
        public int PeriodNumber { get; set; }
        public string SettlementMethod { get; set; }
        public string BillingEntityCode { get; set; }
        public string BillingEntityName { get; set; }
        public string BilledEntityCode { get; set; }
        public string BilledEntityName { get; set; }
        public int TotalNoOfInvoices { get; set; }
        public string CurrencyCode { get; set; }
        public decimal PrepaidAmount { get; set; }
        public decimal CollectAmount { get; set; }
        public decimal RejectAmount { get; set; }
        public decimal BillingAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
