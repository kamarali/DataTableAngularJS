using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.PendingInvoicesInErrorReport;

namespace Iata.IS.Business.Reports.PendingInvoicesInErrorReport.Impl
{
    public class PendingInvoicesInErrorReport : IPendingInvoicesInErrorReport
    {
        public IPendingInvoicesInErrorReportData PendingInvoices { get; set; }

         /* Parameterised constructor */

        public PendingInvoicesInErrorReport (IPendingInvoicesInErrorReportData reportData) 
        {
            PendingInvoices = reportData ;
        }
        public List<Model.Reports.PendingInvoicesInErrorReport> GetPendingInvoicesInErrorReport(int memberId, int billingMonth, int billingYear, int billingPeriod, 
                                                                        int billingCategory, int settlementMethod, int errorType, int isTotalsRequired)
        {
            return PendingInvoices.GetPendingInvoicesInErrorReport(memberId, billingMonth, billingYear, billingPeriod, 
                                                                         billingCategory,  settlementMethod,errorType, isTotalsRequired);
        }
    }
}
