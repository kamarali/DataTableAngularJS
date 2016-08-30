using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.InterlinePayablesAnalysis;

namespace Iata.IS.Data.Reports.InterlinePayablesAnalysis
{
    public interface IInterlinePayablesAnalysis : IRepository<InvoiceBase>
    {
        /// <summary>
        /// Author: Upendra Yadav
        /// Date of Creation: 25-10-2011
        /// Purpose: For Interline Payables Analysis Report
        /// </summary>
        /// <param name="clearanceMonth"></param>
        /// <param name="clearanceYear"></param>
        /// <param name="clearancePeriodNo"></param>
        /// <param name="settlementMethod"></param>
        /// <param name="loginEntityId"></param>
        /// <param name="billingEntityId"></param>
        /// <param name="currencyCode"></param>
        /// <param name="comparisonPeriod"></param>
        /// <returns></returns>
        List<InterlinePayablesAnalysisModel> GetInterlinePayablesAnalysis(int clearanceMonth, 
                                                                                int clearanceYear,
                                                                                int clearancePeriodNo,
                                                                                int settlementMethod,
                                                                                int loginEntityId,
                                                                                int billingEntityId, 
                                                                                int currencyCode,
                                                                                int comparisonPeriod);

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 25-10-2011
        /// Purpose: For Interline Billing Summary Report
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="period"></param>
        /// <param name="settlementMethodId"></param>
        /// <param name="currencyId"></param>
        /// <param name="billedEntityCode"></param>
        /// <param name="billingEntityCode"></param>
        /// <returns></returns>
        List<InterlineBillingSummaryReportResultModel> GetInterlineBillingSummaryReportDetails(int year,
                                                                                               int month,
                                                                                               int period,
                                                                                               int? settlementMethodId,
                                                                                               int currencyId,
                                                                                               int? billedEntityCode,
                                                                                               int billingEntityCode, int isCurrentPeriod);

    }
}