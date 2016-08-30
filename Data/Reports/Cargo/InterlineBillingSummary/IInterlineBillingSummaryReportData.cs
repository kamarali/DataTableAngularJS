using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Reports.Cargo.InterlineBillingSummary
{
    public interface IInterlineBillingSummaryReportData : IRepository<InvoiceBase>
    {
        List<Iata.IS.Model.Reports.Cargo.InterlineBillingSummary> GetInterlineBillingSummaryReport(int billingType, 
                                        int billingMonthFrom, int billingMonthTo, int billingYearFrom, 
                                        int billingYearTo, int? billingPeriodFrom, int? billingPeriodTo, 
                                        int? billingMemberId, int? billedMemberId, int? settlementMethod, int currencyCode);
    }
}
