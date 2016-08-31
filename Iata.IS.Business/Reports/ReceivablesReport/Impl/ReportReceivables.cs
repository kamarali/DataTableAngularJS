using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.ReceivablesReport;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.ReceivablesReport;

namespace Iata.IS.Business.Reports.ReceivablesReport.Impl
{
    public class ReportReceivables : IReceivablesReport
    {

        public IReceivablesReportData IreceivablesreportdataProp { get; set; }


        public ReportReceivables(IReceivablesReportData Ireceivablesreportdata)
        {
            IreceivablesreportdataProp = Ireceivablesreportdata;
        }

        // CMP523 - Source Code in RMBMCM Summary Report
        public List<ReceivablesReportModel> GetReceivablesReportDetails(int loginMemberId,  int billingMonth,int billingYear, int billingPeriod, int settlementMethod,int MemoType, int submissionMethod, string invoiceNo, int billedEntityCode, string RMBMCMNo, string sourceCode)
        {
            // Getcorrespondencedetails from data
            return IreceivablesreportdataProp.GetReceivablesReportDetails(loginMemberId, billingMonth,billingYear, billingPeriod,
                                                               settlementMethod,MemoType,  submissionMethod, invoiceNo,
                                                               billedEntityCode, RMBMCMNo, sourceCode);
        }// End GetCorrespondenceDetails


        /// <summary>
        /// Author: Sanket Shrivastava
        /// Date of Creation: 11-10-2011
        /// Purpose: Pax: Sampling Billing Analysis
        /// </summary>
        /// <param name="billingType"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="billedEntityId"></param>
        /// <param name="billingEntityId"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public List<SamplAnalysisRec> GetSamplAnalysisRecReportDetails(int billingType,
                                                                       int month,
                                                                       int year,
                                                                       int? billedEntityId,
                                                                       int? billingEntityId,
                                                                       int currencyCode)
        {
            return IreceivablesreportdataProp.GetSamplAnalysisRecReportDetails(billingType,
                                                                               month,
                                                                               year,
                                                                               billedEntityId,
                                                                               billingEntityId,
                                                                               currencyCode);
        } // End of GetSamplAnalysisRecReportDetails


        public List<RMBMCMDetailsUI> GetRMBMCMDetailsReport(int billingYear, int billingMonth, int billingPeriod, int billingType, int settlementMethod, int MemoType, int submissionMethod, int billedEntityCode, string invoiceNo, string RMBMCMNo, int billingEntityCode, int output)
        {
          return IreceivablesreportdataProp.GetRMBMCMDetailsReport(billingYear, billingMonth, billingPeriod, billingType, settlementMethod, MemoType, submissionMethod, billedEntityCode, invoiceNo, RMBMCMNo, billingEntityCode, output);
        }

        public List<RMBMCMSummaryCargoReceivableReport> RMBMCMSummaryReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod, int MemoType, int submissionMethod, int billingEntityId, int billedEntityCode, string invoiceNo, string RMBMCMNo)
        {
            return IreceivablesreportdataProp.RmBmCmSummaryReport(billingYear, billingMonth, billingPeriod, settlementMethod, MemoType, submissionMethod, billingEntityId, billedEntityCode, invoiceNo, RMBMCMNo);
        }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 05-10-2011
        /// Purpose: For Passanger Interline Billing Summary Report
        /// </summary>
        /// <param name="billingType"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <param name="periodNo"></param>
        /// <param name="billingEntityId"></param>
        /// <param name="billedEntityId"></param>
        /// <param name="settlementMethodIndicatorId"></param>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public List<PaxInterlineBillingSummaryReportResult> GetPaxInterlineBillingSummaryReportDetails(int billingType,
                                                                                                       int month,
                                                                                                       int year,
                                                                                                       int? periodNo,
                                                                                                       int? billingEntityId,
                                                                                                       int? billedEntityId,
                                                                                                       int? settlementMethodIndicatorId,
                                                                                                       int currencyCode)
        {
            return IreceivablesreportdataProp.GetPaxInterlineBillingSummaryReportDetails(billingType,
                                                                                         month,
                                                                                         year,
                                                                                         periodNo,
                                                                                         billingEntityId,
                                                                                         billedEntityId,
                                                                                         settlementMethodIndicatorId,
                                                                                         currencyCode);
        } // End of GetPaxInterlineBillingSummaryReportDetails

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 18-10-2011
        /// Purpose: For Passenger Rejection Analysis - Non Sampling
        /// CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        /// input parameter updated (From Year Month and To Year Month)
        /// </summary>
        /// <param name="fromMonth"></param>
        /// <param name="fromYear"></param>
        /// <param name="toMonth"></param>
        /// <param name="toYear"></param>
        /// <param name="billingEntityCode"></param>
        /// <param name="billedEntityCode"></param>
        /// <param name="currencyId"></param>
        /// <param name="includeFIMData"></param>
        /// <param name="billingType"></param>
        /// <returns></returns>
        public List<PaxRejectionAnalysisNonSamplingReportResult> GetPaxRejectionAnalysisNonSamplingReportDetails(int fromMonth,
                                                                                                                 int fromYear,
                                                                                                                 int toMonth,
                                                                                                                 int toYear,
                                                                                                                 int billingEntityCode,
                                                                                                                 int billedEntityCode,
                                                                                                                 int currencyId,
                                                                                                                 int includeFIMData,
                                                                                                                 int billingType
                                                                                                                 )
        {
            return IreceivablesreportdataProp.GetPaxRejectionAnalysisNonSamplingReportDetails(fromMonth,
                                                                                              fromYear,
                                                                                              toMonth,
                                                                                              toYear,
                                                                                              billingEntityCode,
                                                                                              billedEntityCode,
                                                                                              currencyId,
                                                                                              includeFIMData,
                                                                                              billingType);
        }
    }
}
