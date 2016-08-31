using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.Cargo.InterlineBillingSummary;

namespace Iata.IS.Business.Reports.Cargo.InterlineBillingSummary.Impl
{
    public class InterlineBillingSummaryReport : IInterlineBillingSummaryReport
    {
        public IInterlineBillingSummaryReportData  InterlineBillingSummary { get; set; }

        /* Parameterised constructor */

        public InterlineBillingSummaryReport(IInterlineBillingSummaryReportData reportData)
        {
            InterlineBillingSummary = reportData;
 
        }
        public List<Model.Reports.Cargo.InterlineBillingSummary> GetInterlineBillingSummaryReport(int billingType, int billingMonthFrom, int billingMonthTo, int billingYearFrom, int billingYearTo, int? billingPeriodFrom, int? billingPeriodTo, int? billingMemberId, int? billedMemberId, int? settlementMethod, int currencyCode)
        {
            return InterlineBillingSummary.GetInterlineBillingSummaryReport( billingType, billingMonthFrom,
                                                            billingMonthTo,billingYearFrom, billingYearTo,
                                                            billingPeriodFrom, billingPeriodTo, billingMemberId, 
                                                            billedMemberId, settlementMethod, currencyCode);
        }
    }
}
