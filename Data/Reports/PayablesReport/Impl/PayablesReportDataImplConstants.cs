using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Reports.PayablesReport.Impl
{
    public class PayablesReportDataImplConstants
    {
        public const string MemberId = "MEMBER_ID_I";
        public const string BillingMonth = "BILLING_MONTH_I";
        public const string BillingYear = "BILLING_YEAR_I";
        public const string BillingPeriod = "BILLING_PERIOD_I";
        public const string SettlementMethodId = "SETTLEMENT_METHOD_ID_I";
        public const string MemoType = "MEMO_TYPE_I";
        public const string SubmissionId = "SUBMISSION_ID_I";
        public const string InvoiceNumber = "INVOICE_NO_I";
        public const string BillingEntityCode = "BILLING_ENTITY_CODE_I";
        public const string RMBMCMNO = "RMBMCM_NO_I";
        // CMP523 - Source Code in RMBMCM Summary Report
        public const string SourceCode = "SOURCE_CODE_I";
        public const string PaxPayablesReport = "GetPaxPayablesReport";


        /// <summary>
        /// Constans for RMBMCM Summary Paybles Report 
        /// </summary>
        public const string billingType = "BILLING_TYPE_I";
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
        public const string GetRmBmCmSummaryPayblesReport = "GetRmBmCmSummaryPayblesReport";
    }
}
