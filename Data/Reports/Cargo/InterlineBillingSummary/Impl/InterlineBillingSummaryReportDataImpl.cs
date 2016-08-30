using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Reports.Cargo.InterlineBillingSummary.Impl
{
    public class InterlineBillingSummaryReportDataImpl : Repository<InvoiceBase>, IInterlineBillingSummaryReportData
    {

        /*List<Iata.IS.Model.Reports.Cargo.InterlineBillingSummary> GetInterlineBillingSummaryReport(int billingType, int billingMonthFrom, int billingMonthTo, int billingYearFrom, int billingYearTo, int billingPeriodFrom, int billingPeriodTo, int billingMemberId, int billedMemberId, int settlementMethod, int currencyCode)
        {
            var parameters = new ObjectParameter[11];

            parameters[0] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingType, billingType);
            parameters[1] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingMonthFrom, billingMonthFrom);
            parameters[2] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingMonthTo, billingMonthTo);
            parameters[3] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingYearFrom, billingYearFrom);
            parameters[4] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingYearTo, billingYearTo);
            parameters[5] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.periodNoFrom, billingPeriodFrom);
            parameters[6] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.periodNoTo, billingPeriodTo);
            parameters[7] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingMemberId, billingMemberId);
            parameters[8] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billedMemberId, billedMemberId);
            parameters[9] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.settlementMethodId, settlementMethod);
            parameters[10] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingCurrencyCode, currencyCode);

            var list =
                ExecuteStoredFunction<Iata.IS.Model.Reports.Cargo.InterlineBillingSummary>(InterlineBillingSummaryReportDataImplConstants.GetInterlineBillingSummaryReport, parameters);
            return list.ToList();

        } */

        List<Iata.IS.Model.Reports.Cargo.InterlineBillingSummary> IInterlineBillingSummaryReportData.GetInterlineBillingSummaryReport(int billingType, int billingMonthFrom, int billingMonthTo, int billingYearFrom, int billingYearTo, int? billingPeriodFrom, int? billingPeriodTo, int? billingMemberId, int? billedMemberId, int? settlementMethod, int currencyCode)
        {
            var parameters = new ObjectParameter[11];
            
            parameters[0] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingType, typeof (int)) { Value = billingType };
            parameters[1] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingMonthFrom, typeof (int)) { Value = billingMonthFrom };
            parameters[2] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingMonthTo, typeof(int)) { Value = billingMonthTo };
            parameters[3] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingYearFrom, typeof(int)) { Value = billingYearFrom };
            parameters[4] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingYearTo, typeof(int)) { Value = billingYearTo };
            parameters[5] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.periodNoFrom, typeof (int?)) { Value = billingPeriodFrom };
            parameters[6] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.periodNoTo, typeof (int?)) { Value = billingPeriodTo };
            parameters[7] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingMemberId, typeof (int?)) { Value = billingMemberId };
            parameters[8] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billedMemberId, typeof (int?)) { Value = billedMemberId };
            parameters[9] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.settlementMethodId, typeof (int?)) { Value = settlementMethod };
            parameters[10] = new ObjectParameter(InterlineBillingSummaryReportDataImplConstants.billingCurrencyCode, typeof(int)) { Value = currencyCode };

            var list =
                ExecuteStoredFunction<Iata.IS.Model.Reports.Cargo.InterlineBillingSummary>(InterlineBillingSummaryReportDataImplConstants.GetInterlineBillingSummaryReport, parameters);
            return list.ToList();
        }
    }
}
