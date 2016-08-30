using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports
{
    public class PendingInvoicesInErrorReport
    {
        public string BillingMonthYear { get; set; }
        public int PeriodNumber { get; set; }
        public string SettlementMethod { get; set; }
        public string BilledEntityCode { get; set; }
        public string BilledEntityName { get; set; }
        public string InvoiceNumber { get; set; }
        public string BillingCategory { get; set; }
        public string InvoiceCurrency { get; set; }
        public decimal InvoiceAmount { get; set; }
        public string ErrorType { get; set; }
        public int ErrorCount { get; set; }
    }
}
