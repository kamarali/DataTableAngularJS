using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Reports.Cargo.InterlineBillingSummary.Impl
{
    public class InterlineBillingSummaryReportDataImplConstants
    {
        /// <summary>
        /// Constans for Cargo Interline Billing Summary Receivables and Paybles Report 
        /// </summary>
        public const string billingType = "BILLING_TYPE_I";
        public const string billingMonthFrom = "BILLING_MONTH_FROM_I";
        public const string billingMonthTo = "BILLING_MONTH_TO_I";
        public const string billingYearFrom = "BILLING_YEAR_FROM_I";
        public const string billingYearTo = "BILLING_YEAR_TO_I";
        public const string periodNoFrom = "PERIOD_NO_FROM_I";
        public const string periodNoTo = "PERIOD_NO_TO_I";
        public const string billingMemberId = "BILLING_MEMBER_ID_I";
        public const string billedMemberId = "BILLED_MEMBER_ID_I";
        public const string settlementMethodId = "SETTLEMENT_METHOD_ID_I";
        public const string billingCurrencyCode = "BILLING_CURRENCY_CODE_NUM_I";
        public const string GetInterlineBillingSummaryReport = "GetInterlineBillingSummaryReport";
    }
}
