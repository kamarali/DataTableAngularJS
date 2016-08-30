using System;

namespace Iata.IS.Model.Common
{
    public class ExceptionDetailsSearchResult
    {

        public string LineItemOrBatchNo { get; set; }

        public string LineItemDetailOrSequenceNo { get; set; }

        public string DocumentNo { get; set; }

        public string LinkedDocNo { get; set; }

        public string ErrorLevel { get; set; }

        public string ErrorLevelDisplay { get; set; }

        public string FieldName { get; set; }

        public string FieldValue { get; set; }

        public string BillingCode { get; set; }

        public string TransactionName { get; set; }

        public string BillingEntityCode { get; set; }

        public string TransactionType { get; set; }

        public Guid ExceptionDetailId { get; set; }

        public String YourInvoiceNo { get; set; }

        public string YourInvoiceMonth { get; set; }

        public string YourInvoiceYear { get; set; }

        public string YourInvoicePeriod { get; set; }

        public string YourRejectionMemoNo { get; set; }

        public string YourBmCmNo { get; set; }

        public string BmCmIndicator { get; set; }

        public string YourInvoiceBillingDate { get; set; }

        public bool IsBmCmIndicator { get; set; }

        public Guid PkReferenceId { get; set; }

        public string TranscationId { get; set; }

        public string CorrespondenceRefNo { get; set; }

        public string ReasonCode { get; set; }

        public string RejectionStage { get; set; }

        public string BatchSeqNo { get; set; }

        public string BatchRecordSeq { get; set; }

        public string FimCouponNo { get; set; }

        public string CouponNo { get; set; }

        public string FimBmCmNo { get; set; } //FimBmCmNo

        public string FimBmCmIndicator { get; set; }
        //SCP:37078 Add Source code property used for Linking purpose of FIM RM 2 and RM3
        public int SourceCodeId { get; set; }

        //SCP252342 - SRM: ICH invoice in ready for billing status
        public DateTime LastUpdatedOn { get; set; }

    }
}
