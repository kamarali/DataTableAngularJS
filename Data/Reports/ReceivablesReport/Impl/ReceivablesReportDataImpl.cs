using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.ReceivablesReport;

namespace Iata.IS.Data.Reports.ReceivablesReport.Impl
{
    public class ReceivablesReportDataImpl : Repository<InvoiceBase>, IReceivablesReportData
    {
        public List<ReceivablesReportModel> GetReceivablesReportDetails(int loginMemberId, int billingMonth, int billingYear, int billingPeriod, int settlementMethod, int MemoType, int submissionMethod, string invoiceNo, int billedEntityCode, string RMBMCMNo, string sourceCode)
        {
            var parameters = new ObjectParameter[11];
            parameters[0] = new ObjectParameter(ReceivablesReportDataImplConstants.MemberId, loginMemberId);
            parameters[1] = new ObjectParameter(ReceivablesReportDataImplConstants.BillingMonth, billingMonth);
            parameters[2] = new ObjectParameter(ReceivablesReportDataImplConstants.BillingYear, billingYear);
            parameters[3] = new ObjectParameter(ReceivablesReportDataImplConstants.BillingPeriod, billingPeriod);
            parameters[4] = new ObjectParameter(ReceivablesReportDataImplConstants.SettlementMethodId, settlementMethod);
            parameters[5] = new ObjectParameter(ReceivablesReportDataImplConstants.MemoType, MemoType);
            parameters[6] = new ObjectParameter(ReceivablesReportDataImplConstants.SubmissionId, submissionMethod);
            parameters[7] = new ObjectParameter(ReceivablesReportDataImplConstants.InvoiceNumber, invoiceNo);
            parameters[8] = new ObjectParameter(ReceivablesReportDataImplConstants.BilledEntityCode, billedEntityCode);
            parameters[9] = new ObjectParameter(ReceivablesReportDataImplConstants.RMBMCMNO, RMBMCMNo);
            // CMP523 - Source Code in RMBMCM Summary Report
            parameters[10] = new ObjectParameter(ReceivablesReportDataImplConstants.SourceCode, sourceCode);
            var list =
                ExecuteStoredFunction<ReceivablesReportModel>(ReceivablesReportDataImplConstants.PaxReceivablesReport, parameters);
            return list.ToList();
        }

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
        public List<SamplAnalysisRec> GetSamplAnalysisRecReportDetails(int billingType,int month, int year, int? billedEntityId, int? billingEntityId, int currencyCode)
        {
            var parameters = new ObjectParameter[6];

            parameters[0] = new ObjectParameter(ReceivablesReportDataImplConstants.SBillingType, typeof(int)) { Value = billingType};
            parameters[1] = new ObjectParameter(ReceivablesReportDataImplConstants.SMonth, typeof(int)) { Value = month };
            parameters[2] = new ObjectParameter(ReceivablesReportDataImplConstants.SYear, typeof(int)) { Value = year };
            parameters[3] = new ObjectParameter(ReceivablesReportDataImplConstants.SBilledEntityId, typeof(int?)) { Value = billedEntityId };
            parameters[4] = new ObjectParameter(ReceivablesReportDataImplConstants.SBillingEntityId, typeof(int?)) { Value = billingEntityId };
            parameters[5] = new ObjectParameter(ReceivablesReportDataImplConstants.SCurrencyCode, typeof(int)) { Value = currencyCode };

            var list = ExecuteStoredFunction<SamplAnalysisRec>(ReceivablesReportDataImplConstants.GetSamplAnalysisRecFunction, parameters);
            return list.ToList();
        }

       
        public List<RMBMCMDetailsUI> GetRMBMCMDetailsReport(int billingYear, int billingMonth, int billingPeriod, int billingType, int settlementMethod, int MemoType, int submissionMethod, int billedEntityCode, string invoiceNo, string RMBMCMNo, int billingEntityCode, int output)
        {
            var parameters = new ObjectParameter[12];
            parameters[0] = new ObjectParameter(ReceivablesReportDataImplConstants.billingyear_rm, billingYear);
            parameters[1] = new ObjectParameter(ReceivablesReportDataImplConstants.billingMonth_rm, billingMonth);
            parameters[2] = new ObjectParameter(ReceivablesReportDataImplConstants.billingPeriod_rm, billingPeriod);
            parameters[3] = new ObjectParameter(ReceivablesReportDataImplConstants.billingtype_rm, billingType);
            parameters[4] = new ObjectParameter(ReceivablesReportDataImplConstants.settelmentMethod, settlementMethod);
            parameters[5] = new ObjectParameter(ReceivablesReportDataImplConstants.memo_type, MemoType);
            parameters[6] = new ObjectParameter(ReceivablesReportDataImplConstants.dataSource, submissionMethod);
            parameters[7] = new ObjectParameter(ReceivablesReportDataImplConstants.billedEntityCode, billedEntityCode);
            parameters[8] = new ObjectParameter(ReceivablesReportDataImplConstants.invoiceNo, invoiceNo);
            parameters[9] = new ObjectParameter(ReceivablesReportDataImplConstants.rmbmcmNumber, RMBMCMNo);
            parameters[10] = new ObjectParameter(ReceivablesReportDataImplConstants.billingEntityCode, billingEntityCode);
            parameters[11] = new ObjectParameter(ReceivablesReportDataImplConstants.output, output);
            var list =
                ExecuteStoredFunction<RMBMCMDetailsUI>(ReceivablesReportDataImplConstants.RmBmCmDetailReport, parameters);
            return list.ToList();
        }

        public List<RMBMCMSummaryCargoReceivableReport> RmBmCmSummaryReport(int billingYear,
            int billingMonth, int billingPeriod, int settlementMethod, 
            int memoType,int dataSource, int billingEntityCode, int billedEntityCode,
            string invoiceNo, string RMBMCMNo)
        {
            var parameters = new ObjectParameter[10];
            parameters[0] = new ObjectParameter(ReceivablesReportDataImplConstants.billngEntityCode_sum, billingEntityCode);
            parameters[1] = new ObjectParameter(ReceivablesReportDataImplConstants.billingMonth_sum, billingMonth);
            parameters[2] = new ObjectParameter(ReceivablesReportDataImplConstants.billingYear_sum ,billingYear);
            parameters[3] = new ObjectParameter(ReceivablesReportDataImplConstants.periodNo_sum, billingPeriod);
            parameters[4] = new ObjectParameter(ReceivablesReportDataImplConstants.settlementMethod_sum, settlementMethod);
            parameters[5] = new ObjectParameter(ReceivablesReportDataImplConstants.memoType_sum, memoType);
            parameters[6] = new ObjectParameter(ReceivablesReportDataImplConstants.dataSource_sum, dataSource);
            parameters[7] = new ObjectParameter(ReceivablesReportDataImplConstants.invoiceNumber_sum,invoiceNo);
            parameters[8] = new ObjectParameter(ReceivablesReportDataImplConstants.airlineCode_sum, billedEntityCode);
            parameters[9] = new ObjectParameter(ReceivablesReportDataImplConstants.rMBMCMNumber_sum,RMBMCMNo);

            var list =
                ExecuteStoredFunction<RMBMCMSummaryCargoReceivableReport>(ReceivablesReportDataImplConstants.RmBmCmSummaryReport, parameters);
            return list.ToList(); 
            
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
        public List<PaxInterlineBillingSummaryReportResult> GetPaxInterlineBillingSummaryReportDetails(int billingType, int month, int year, int? periodNo, int? billingEntityId, int? billedEntityId, int? settlementMethodIndicatorId, int currencyCode)
        {
            var parameters = new ObjectParameter[8];

            parameters[0] = new ObjectParameter(ReceivablesReportDataImplConstants.BillingType, typeof(int)) { Value = billingType };
            parameters[1] = new ObjectParameter(ReceivablesReportDataImplConstants.Month, typeof(int)) { Value = month };
            parameters[2] = new ObjectParameter(ReceivablesReportDataImplConstants.Year, typeof(int)) { Value = year };
            parameters[3] = new ObjectParameter(ReceivablesReportDataImplConstants.PeriodNo, typeof(int?)) { Value = periodNo };
            parameters[4] = new ObjectParameter(ReceivablesReportDataImplConstants.BillingEntityId, typeof(int?)) { Value = billingEntityId };
            parameters[5] = new ObjectParameter(ReceivablesReportDataImplConstants.BilledEntityId, typeof(int?)) { Value = billedEntityId };
            parameters[6] = new ObjectParameter(ReceivablesReportDataImplConstants.SettlementMethodIndicatorId, typeof(int?)) { Value = settlementMethodIndicatorId };
            parameters[7] = new ObjectParameter(ReceivablesReportDataImplConstants.CurrencyCode, typeof(int?)) {Value =  currencyCode};

            var list = ExecuteStoredFunction<PaxInterlineBillingSummaryReportResult>(
                                             ReceivablesReportDataImplConstants.PaxInterlineBillingSummaryReportFunction, parameters);
            return list.ToList();
        }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 18-10-2011
        /// Purpose: For Passenger Rejection Analysis - Non Sampling
        /// CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        /// input parameter updated (From Year Month and To Year Month)
        /// </summary>
        /// <param name="fromMonth">fromMonth</param>
        /// <param name="fromYear">fromYear</param>
        /// <param name="toMonth">toMonth</param>
        /// <param name="toYear">toYear</param>
        /// <param name="billingEntityCode">billingEntityCode</param>
        /// <param name="billedEntityCode">billedEntityCode</param>
        /// <param name="currencyId">currencyId</param>
        /// <param name="includeFIMData">includeFIMData</param>
        /// <param name="billingType">billingType</param>
        /// <returns></returns>
        public List<PaxRejectionAnalysisNonSamplingReportResult> GetPaxRejectionAnalysisNonSamplingReportDetails(int fromMonth, int fromYear, int toMonth, int toYear, int billingEntityCode, int billedEntityCode, int currencyId, int includeFIMData, int billingType)
        {
            var parameters = new ObjectParameter[8];

            parameters[0] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonFromMonth, typeof(int)) { Value = fromMonth };
            parameters[1] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonFromYear, typeof(int)) { Value = fromYear };
            parameters[2] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonToMonth, typeof(int)) { Value = toMonth };
            parameters[3] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonToYear, typeof(int)) { Value = toYear };
            parameters[4] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonBillingEntityCode, typeof(int?)) { Value = billingEntityCode };
            parameters[5] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonBilledEntityCode, typeof(int?)) { Value = billedEntityCode };
            parameters[6] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonCurrencyId, typeof(int)) { Value = currencyId };
            parameters[7] = new ObjectParameter(ReceivablesReportDataImplConstants.PRANonIncludeFIMData, typeof(int)) { Value = includeFIMData };

            var stroredFunctionName = string.Empty;
            // check here for Payable or Receivables report
            if (billingType == 1)
            {
                stroredFunctionName = ReceivablesReportDataImplConstants.PaxRejectionAnalysisNonSamplingReportPFunction;
            }
            else if (billingType == 2)
            {
                stroredFunctionName = ReceivablesReportDataImplConstants.PaxRejectionAnalysisNonSamplingReportRFunction;
            }

            var list = ExecuteStoredFunction<PaxRejectionAnalysisNonSamplingReportResult>(stroredFunctionName, parameters);
            return list.ToList();
        }
    }
}

