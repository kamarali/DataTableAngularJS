using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.ReceivablesReport;

namespace Iata.IS.Business.Reports.ReceivablesReport
{
    public interface IReceivablesReport
    {
        // CMP523 - Source Code in RMBMCM Summary Report
        List<ReceivablesReportModel> GetReceivablesReportDetails(int loginMemberId, int billingMonth,int billingYear, int billingPeriod, int settlementMethod,int MemoType, int submissionMethod, string invoiceNo, int billedEntityCode, string RMBMCMNo, string sourceCode);

        /// <summary>
        /// Author: Sanket Shrivastava
        /// Date of Creation: 11-10-2011
        /// Purpose: Pax: Sampling Billing Analysis
        /// </summary>
        /// <param name="sBillingType"></param>
        /// <param name="sMonth"></param>
        /// <param name="sYear"></param>
        /// <param name="sBilledEntityCode"></param>
        /// <param name="sBillingEntityCode"></param>
        /// <param name="sCurrencyCode"></param>
        /// <returns></returns>
        List<SamplAnalysisRec> GetSamplAnalysisRecReportDetails(int sBillingType,
                                                                int sMonth, 
                                                                int sYear,
                                                                int? sBilledEntityCode,
                                                                int? sBillingEntityCode,
                                                                int sCurrencyCode);

        List<RMBMCMDetailsUI> GetRMBMCMDetailsReport(int billingYear, int billingMonth, int billingPeriod, int billingType, int settlementMethod, int MemoType, int submissionMethod, int billedEntityCode, string invoiceNo, string RMBMCMNo, int billingEntityCode , int output);

        List<RMBMCMSummaryCargoReceivableReport> RMBMCMSummaryReport(int billingYear, int billingMonth,
                                                                     int billingPeriod, int settlementMethod,
                                                                     int MemoType, int submissionMethod,
                                                                     int billingEntityId, int billedEntityCode,
                                                                     string invoiceNo, string RMBMCMNo);

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 07-10-2011
        /// Purpose: For Receivables - Passanger Interline Billing Summary Report
        /// </summary>
        /// <param name="Year"></param>
        /// <param name="Month"></param>
        /// <param name="Period"></param>
        /// <param name="BillingEntityCode"></param>
        /// <param name="BilledEntityCode"></param>
        /// <param name="SettlementMethodId"></param>
        /// <param name="CurrencyId"></param>
        /// <returns></returns>
        List<PaxInterlineBillingSummaryReportResult> GetPaxInterlineBillingSummaryReportDetails(int BillingType,
                                                                                                int Month, 
                                                                                                int Year,
                                                                                                int? Period,
                                                                                                int? BillingEntityCode,
                                                                                                int? BilledEntityCode,
                                                                                                int? SettlementMethodId,
                                                                                                int CurrencyId);
        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 18-10-2011
        /// Purpose: For Passenger Rejection Analysis - Non Sampling
        /// CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        /// input parameter updated (From Year Month and To Year Month)
        /// </summary>
        /// <param name="fromMonth">From month.</param>
        /// <param name="fromYear">From year.</param>
        /// <param name="toMonth">To month.</param>
        /// <param name="toYear">To year.</param>
        /// <param name="billingEntityCode">The billing entity code.</param>
        /// <param name="billedEntityCode">The billed entity code.</param>
        /// <param name="currencyId">The currency identifier.</param>
        /// <param name="includeFIMData">The include fim data.</param>
        /// <param name="billingType">Type of the billing.</param>
        /// <returns></returns>
        List<PaxRejectionAnalysisNonSamplingReportResult> GetPaxRejectionAnalysisNonSamplingReportDetails(int fromMonth,
                                                                                                          int fromYear, 
                                                                                                          int toMonth,
                                                                                                          int toYear,
                                                                                                          int billingEntityCode,
                                                                                                          int billedEntityCode,
                                                                                                          int currencyId,
                                                                                                          int includeFIMData,
                                                                                                          int billingType
                                                                                                          );
    }
}
