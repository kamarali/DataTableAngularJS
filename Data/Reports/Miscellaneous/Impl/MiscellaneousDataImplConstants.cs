using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Reports.Miscellaneous.Impl
{
    public class MiscellaneousDataImplConstants
    {

        /// <summary>
        /// Author: Sanket Shrivastava
        /// Date of Creation: 17-10-2011
        /// Purpose: Misc: Invoice Summary Report
        /// Author: Kamarali Dukandar
        /// Date of Modification: 15-07-2016
        /// Purpose: Pax: CMP# 663 MISC Invoice Summary Reports - Add 'Transaction Type'
        /// added an extra input parameter InvoiceType
        /// </summary>

        public const string FMonth = "BILLING_MONTH_FROM";
        public const string FYear = "BILLING_YEAR_FROM";
        public const string FPeriod = "BILLING_PERIOD_FROM";
        public const string Tmonth = "BILLING_MONTH_TO";
        public const string Tyear = "BILLING_YEAR_TO";
        public const string Tperiod = "BILLING_PERIOD_TO";
        public const string BillingType = "BILLING_TYPE_I";
        public const string SubmissionMethod = "SUBMISSION_METHOD_I";
        public const string SettlementMethod = "SETTLEMENT_METHOD_ID_I";
        public const string BillingEntityId = "BILLING_MEMBER_ID_I";
        public const string BilledEntityId = "BILLED_MEMBER_ID_I";
        public const string ChargeCategory = "CHARGE_CATEGORY_ID_I";
        public const string CurrencyCode = "CURRENCY_CODE_NUM_I";
        public const string InvoiceType = "INVOICE_TYPE_I";
        public const string GetInvoiceSummaryFunction = "GetInvoiceSummary";

        public const string FromYear = "BILLING_YEAR_FROM_I";
        public const string FromMonth = "BILLING_MONTH_FROM_I";
        public const string FromPeriod = "PERIOD_NO_FROM_I";
        public const string ToYear = "BILLING_YEAR_TO_I";
        public const string ToMonth = "BILLING_MONTH_TO_I";
        public const string ToPeriod = "PERIOD_NO_TO_I";
        public const string BillingEntityCode = "BILLING_MEMBER_ID_I";
        public const string BilledEntityCode = "BILLED_MEMBER_ID_I";
        public const string ChargeCategoryMisc = "CHARGE_CATEGORY_I";
        public const string ChargeCode = "CHARGE_CODE_I";
        public const string InvoiceNumber = "INVOICE_NO_I";
        public const string GetMiscSubstitutionValuesReportFunction = "GetMiscSubstitutionValuesReport";
    }
}
