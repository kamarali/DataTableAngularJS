using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Reports.ReceivablesReport.Impl
{
    public class ReceivablesReportDataImplConstants
    {
        public const string MemberId = "MEMBER_ID_I";
        public const string BillingMonth = "BILLING_MONTH_I";
        public const string BillingYear = "BILLING_YEAR_I";
        public const string BillingPeriod = "BILLING_PERIOD_I";
        public const string SettlementMethodId = "SETTLEMENT_METHOD_ID_I";
        public const string MemoType = "MEMO_TYPE_I";
        public const string SubmissionId = "SUBMISSION_ID_I";
        public const string InvoiceNumber = "INVOICE_NO_I";
        public const string BilledEntityCode = "BILLED_ENTITY_CODE_I";
        public const string RMBMCMNO = "RMBMCM_NO_I";
        // CMP523 - Source Code in RMBMCM Summary Report
        public const string SourceCode = "SOURCE_CODE_I";
        public const string PaxReceivablesReport = "GetPaxReceivablesReport";


        public const string billingyear_rm = "BILLING_YEAR_I";
        public const string billingMonth_rm = "BILLING_MONTH_I";
        public const string billingPeriod_rm = "BILLING_PERIOD_I";
        public const string billingtype_rm = "BILLING_TYPE_I";
        public const string settelmentMethod = "SETTELMENT_METHOD_I";
        public const string memo_type = "MEMO_TYPE_I";
        public const string dataSource = "DATA_SOURCE_I";
        public const string billedEntityCode = "BILLED_ENTITY_CODE_I";
        public const string invoiceNo = "INVOICE_NUMBER_I";
        public const string rmbmcmNumber = "RMBMCM_NUMBER_I";
        public const string billingEntityCode = "BIILING_ENTITY_CODE_I";
        public const string output = "OUTPUT_I";
        public const string RmBmCmDetailReport = "GetRmBmCmDetails";


        /// <summary>
        /// Author: Sanket Shrivastava
        /// Date of Creation: 11-10-2011
        /// Purpose: Pax: Sampling Billing Analysis
        /// </summary>
        public const string SBillingType = "BILLING_TYPE_I";
        public const string SMonth = "BILLING_MONTH_I";
        public const string SYear = "BILLING_YEAR_I";
        public const string SBilledEntityId = "BILLED_MEMBER_ID_I";
        public const string SBillingEntityId = "BILLING_MEMBER_ID_I";
        public const string SCurrencyCode = "BILLING_CURRENCY_CODE_NUM_I";
        public const string GetSamplAnalysisRecFunction = "GetSamplAnalysisRec";
        
        /// <summary>
        /// Constans for RMBMCM Summary Report
        /// </summary>
        public const string billngEntityCode_sum = "MEMBER_ID_I";
        public const string billingYear_sum = "BILLING_YEAR_I";
        public const string billingMonth_sum = "BILLING_MONTH_I";
        public const string periodNo_sum = "BILLING_PERIOD_I";
        public const string settlementMethod_sum = "SETTLEMENT_METHOD_ID_I";
        public const string memoType_sum = "MEMO_TYPE_I";
        public const string dataSource_sum = "SUBMISSION_ID_I";
        public const string airlineCode_sum = "BILLED_ENTITY_CODE_I";
        public const string invoiceNumber_sum = "INVOICE_NO_I";
        public const string output_sum = "OUTPUT_I";
        public const string rMBMCMNumber_sum = "RMBMCM_NO_I";
        public const string RmBmCmSummaryReport = "RMBMCMSummaryReport";

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 05-10-2011
        /// Purpose: For Passanger Interline Billing Summary Report
        /// </summary>
        public const string BillingType = "BILLING_TYPE_I";
        public const string Month = "BILLING_MONTH_I";
        public const string Year = "BILLING_YEAR_I";
        public const string PeriodNo = "PERIOD_NO_I";
        public const string BillingEntityId = "BILLING_MEMBER_ID_I";
        public const string BilledEntityId = "BILLED_MEMBER_ID_I";
        public const string SettlementMethodIndicatorId = "SETTLEMENT_METHOD_ID_I";
        public const string CurrencyCode = "BILLING_CURRENCY_CODE_NUM_I";
        public const string PaxInterlineBillingSummaryReportFunction = "PaxInterlineBillingSummaryReport";

        /// <summary>
        /// Author: Sachin Pharande
        /// Date of Creation: 18-10-2011
        /// Purpose: For Passenger Rejection Analysis - Non Sampling
        /// </summary>
        //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        //input parameter updated (From Year Month and To Year Month)
        public const string PRANonFromMonth = "FROM_BILLING_MONTH_I";
        public const string PRANonFromYear = "FROM_BILLING_YEAR_I";
        public const string PRANonToMonth = "TO_BILLING_MONTH_I";
        public const string PRANonToYear = "TO_BILLING_YEAR_I";

        public const string PRANonBillingEntityCode = "LOGIN_ENTITY_ID_I";
        public const string PRANonBilledEntityCode = "TO_ENTITY_ID_I";
        public const string PRANonCurrencyId = "CURRENCY_CODE_I";
        public const string PRANonIncludeFIMData = "INCLUDE_FIM_DATA_I";

        public const string PaxRejectionAnalysisNonSamplingReportRFunction = "PaxRejectionAnalysisNonSamplingReportR";
        public const string PaxRejectionAnalysisNonSamplingReportPFunction = "PaxRejectionAnalysisNonSamplingReportP";
    }
}
