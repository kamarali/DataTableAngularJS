using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.Reports.ReceivablesReport
{
    /// <summary>
    /// CMP#570: Enhancements to PAX Non-Sampling Rejection Analysis Report
    /// </summary>
    public class PaxRejectionAnalysisNonSamplingReportResult
    {
        public string ClearanceMonthYear { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string CurrencyCode { get; set; }
        public int TotalNoOfPrimeCoupons { get; set; }
        public decimal TotalValueOfPrimeCoupons { get; set; }
        public int NoOfRm1CpnsRejected { get; set; }
        public decimal TotalRm1RejectedAmount { get; set; }
        public int NoOfRm2CpnsRejected { get; set; }
        public decimal TotalRm2RejectedAmount { get; set; }
        public int NoOfRm3CpnsRejected { get; set; }
        public decimal TotalRm3RejectedAmount { get; set; }
        public int CorrNoOfCpnsDisputed { get; set; }
        public decimal CorrValueOfCpnsDisputed { get; set; }
        public int CorrNoOfCpnAcceptedClosed { get; set; }
        public decimal CorrValueOfCpnAcceptedClosed { get; set; }
        public int CorrNoOfCpnExpiredClosed { get; set; }
        public decimal CorrValueOfCpnExpiredClosed { get; set; }
        public int CorrNoOfCpnBilledClosed { get; set; }
        public decimal CorrValueOfCpnBilledClosed { get; set; }
    }
}
