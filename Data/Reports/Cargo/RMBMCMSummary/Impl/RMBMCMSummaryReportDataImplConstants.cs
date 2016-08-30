using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Reports.Cargo.RMBMCMSummary.Impl
{
    public class RMBMCMSummaryReportDataImplConstants
    {
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
        public const string GetRmBmCmSummaryReport = "GetRmBmCmSummaryReport";
        //public const string GetRmBmCmSummaryPayblesReport = "GetRmBmCmSummaryPayblesReport";
    }
}
