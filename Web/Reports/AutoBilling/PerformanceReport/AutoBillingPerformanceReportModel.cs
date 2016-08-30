using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web.Reports.AutoBilling.PerformanceReport
{
    public class AutoBillingPerformanceReportModel
    {
        public int BilledMemberId { get; set; }
        public string BilledMemberCodeAlpha { get; set; }
        public string BilledMemberCodeNumeric { get; set; }
        public string BilledMemberName { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Count1 { get; set; }
        public decimal Amount1 { get; set; }
        public decimal Count2 { get; set; }
        public decimal Amount2 { get; set; }
        public decimal Count3 { get; set; }
        public decimal Amount3 { get; set; }
        public decimal Count4 { get; set; }
        public decimal Amount4 { get; set; }
        public decimal Count5 { get; set; }
        public decimal Amount5 { get; set; }
        public decimal Count6 { get; set; }
        public decimal Amount6 { get; set; }
        public decimal Total { get; set; }
    }
}