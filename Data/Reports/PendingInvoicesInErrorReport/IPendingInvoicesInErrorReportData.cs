using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Reports.PendingInvoicesInErrorReport
{
    public interface IPendingInvoicesInErrorReportData : IRepository<InvoiceBase>
    {
        List<Iata.IS.Model.Reports.PendingInvoicesInErrorReport> GetPendingInvoicesInErrorReport(int memberId, int billingMonth, int billingYear, int billingPeriod, int billingCategory ,
                                                                                                 int settlementMethod, int errorType, int isTotalsRequired);
    }
}
