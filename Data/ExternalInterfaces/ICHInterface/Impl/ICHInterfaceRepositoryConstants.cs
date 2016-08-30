using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.ExternalInterfaces.ICHInterface.Impl
{
    public class ICHInterfaceRepositoryConstants
    {
        // Following const are used for PROC_GET_ICH_SETTLEMENT_DATA sp.
        public const string GetICHSettlementData = "GetICHSettlementData";
        public const string CurrentBillingMonth = "CUR_BILLING_MONTH";
        public const string CurrentBillingYear = "CUR_BILLING_YEAR";
        public const string CurrentBillingPeriod = "CUR_BILLING_PERIOD_NO";
        public const string PreviousBillingMonth = "PREV_BILLING_MONTH";
        public const string PreviousBillingYear = "PREV_BILLING_YEAR";
        public const string PreviousBillingPeriod = "PREV_BILLING_PERIOD_NO";
        public const string MaxRecordCount = "ROWS_TO_RETURN";

        // Following const are used for PROC_UPDATE_INVOICE_STATUS sp.
        public const string UpdateInvoiceStatusAfterSettlement = "UpdateInvoiceStatusAfterSettlement";

        // Following const are used for PROC_UPDATE_F_INVOICE_STATUS sp.
        public const string UpdateInvoiceStatusNullAfterFailedSettlement = "UpdateInvStatusNullAfterFailedSettlement";

        // Following const are used for PROC_UPDATE_F_INVOICE_STATUS sp.
        public const string UpdateInvoiceStatusAfterFailedSettlement = "UpdateInvoiceStatusAfterFailedSettlement";
        public const string InvoiceIds = "INVOICE_IDs";
        public const string ICHSubmissionDate = "SUBMISSION_DATE";

        // Following const are used for PROC_GET_ICH_CROSS_CHECK_DATA sp.
        public const string GetICHCrossCheckData = "GetICHCrossCheckRequestData";
        public const string BillingMonth = "BILLING_MONTH";
        public const string BillingYear = "BILLING_YEAR";
        public const string BillingPeriod = "BILLING_PERIOD_NO";
        public const string StartDateTime = "START_DATETIME";
        public const string EndDateTime = "END_DATETIME";


        // Following const are used for PROC_SETTLEMENT_DATA_FORRESEND sp.
        public const string GetICHSettlementDataForResend = "GetICHSettlementDataforResend";
        public const string invoiceIdList = "INVOICE_ID_LIST";
    }
}
