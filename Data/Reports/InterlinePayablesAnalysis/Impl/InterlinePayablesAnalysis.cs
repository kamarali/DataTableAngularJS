using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.InterlinePayablesAnalysis;

namespace Iata.IS.Data.Reports.InterlinePayablesAnalysis.Impl
{
    public class InterlinePayablesAnalysis : Repository<InvoiceBase>, IInterlinePayablesAnalysis
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
        public List<InterlinePayablesAnalysisModel> GetInterlinePayablesAnalysis(int clearanceMonth, int clearanceYear, int clearancePeriodNo, int settlementMethod, int loginEntityId, int billingEntityId, int currencyCode, int comparisonPeriod)
        {
            var parameters = new ObjectParameter[8];
            parameters[0] = new ObjectParameter(InterlinePayablesAnalysisConstant.ClearanceMonth, clearanceMonth);
            parameters[1] = new ObjectParameter(InterlinePayablesAnalysisConstant.ClearanceYear, clearanceYear);
            parameters[2] = new ObjectParameter(InterlinePayablesAnalysisConstant.ClearancePeriod, clearancePeriodNo);
            parameters[3] = new ObjectParameter(InterlinePayablesAnalysisConstant.LoginEntityId, loginEntityId);
            parameters[4] = new ObjectParameter(InterlinePayablesAnalysisConstant.BillingEntityId, billingEntityId);
            parameters[5] = new ObjectParameter(InterlinePayablesAnalysisConstant.CurrencyCode, currencyCode);
            parameters[6] = new ObjectParameter(InterlinePayablesAnalysisConstant.SettlmentMethod, settlementMethod);
            parameters[7] = new ObjectParameter(InterlinePayablesAnalysisConstant.ComparisonPeriod, comparisonPeriod);

         
            var list =
                  ExecuteStoredFunction<InterlinePayablesAnalysisModel>(InterlinePayablesAnalysisConstant.InterlinePayablesAnalysis, parameters);

            return list.ToList();
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
        public List<InterlineBillingSummaryReportResultModel>  GetInterlineBillingSummaryReportDetails(int year, int month, int period, int? settlementMethodId, int currencyId, int? billedEntityCode, int billingEntityCode, int isCurrentPeriod)
        {
            var parameters = new ObjectParameter[8];
            
            parameters[0] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsClearanceYear, typeof(int)) { Value = year };
            parameters[1] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsClearanceMonth, typeof(int)) { Value = month };
            parameters[2] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsPeriod, typeof(int)) { Value = period };
            parameters[3] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsSettlmentMethod, typeof(int?)) { Value = settlementMethodId };
            parameters[4] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsCurrencyCode, typeof(int)) { Value = currencyId };
            parameters[5] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsBilledMemberId, typeof(int?)) { Value = billedEntityCode };
            parameters[6] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsBillingMemberId, typeof(int)) { Value = billingEntityCode};
            parameters[7] = new ObjectParameter(InterlinePayablesAnalysisConstant.IbsCurrentOpenPeriod, typeof(int)) { Value = isCurrentPeriod };

            var list = ExecuteStoredFunction<InterlineBillingSummaryReportResultModel>(InterlinePayablesAnalysisConstant.InterlineBillingSummaryReportFunction, parameters);

            return list.ToList();
        }
    }
}