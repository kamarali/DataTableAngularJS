using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.Reports.PendingInvoicesInErrorReport
{
    public interface IPendingInvoicesInErrorReport
    {
        List<Iata.IS.Model.Reports.PendingInvoicesInErrorReport> GetPendingInvoicesInErrorReport(int memberId, int billingMonth, int billingYear, int billingPeriod, int billingCategory,
                                                                                                 int settlementMethod, int errorType, int isTotalsRequired);
    }
}
