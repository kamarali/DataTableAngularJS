using Iata.IS.Data.Reports.InterlinePayablesAnalysis;
using Iata.IS.Model.Reports.InterlinePayablesAnalysis;

namespace Iata.IS.Business.Reports.InterlinePayablesAnalysis.Impl
{

    public class InterlinePayablesAnalysisReport : IInterlinePayablesAnalysisReport
    {
        
        private IInterlinePayablesAnalysis InterlinePayablesAnalysis { get; set; }

        public InterlinePayablesAnalysisReport(IInterlinePayablesAnalysis interlinePayablesAnalysis)
        {
            InterlinePayablesAnalysis = interlinePayablesAnalysis;
        }

        public System.Collections.Generic.List<InterlinePayablesAnalysisModel> GetInterlinePayablesAnalysisReport(int clearanceMonth, int clearanceYear, int clearancePeriodNo, int settlementMethod, int loginEntityId, int billingEntityId, int currencyCode, int comparisonPeriod)
        {
            return InterlinePayablesAnalysis.GetInterlinePayablesAnalysis(clearanceMonth, clearanceYear,
                                                                          clearancePeriodNo, settlementMethod,
                                                                          loginEntityId,billingEntityId, currencyCode, comparisonPeriod);
        }

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
        /// CMP#596: converted int to long to support membernumeric code upto 12 digits
        public System.Collections.Generic.List<InterlineBillingSummaryReportResultModel> GetInterlineBillingSummaryReportDetails(int year,
                                                                                                                                 int month,
                                                                                                                                 int period,
                                                                                                                                 int? settlementMethodId,
                                                                                                                                 int currencyId,
                                                                                                                                 int? billedEntityCode,
                                                                                                                                 int billingEntityCode,
            int isCurrentPeriod)
        {
            return InterlinePayablesAnalysis.GetInterlineBillingSummaryReportDetails(year,
                                                                                    month,
                                                                                    period,
                                                                                    settlementMethodId,
                                                                                    currencyId,
                                                                                    billedEntityCode,
                                                                                    billingEntityCode,
                                                                                    isCurrentPeriod);
        }
    }
}