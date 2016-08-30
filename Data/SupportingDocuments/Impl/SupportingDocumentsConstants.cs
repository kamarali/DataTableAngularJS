
namespace Iata.IS.Data.SupportingDocuments.Impl
{
  internal static class SupportingDocumentsConstants
  {

    #region SearchRecords Constants

    public const string BillingMemberId = "Billing_Member_ID_i";
    public const string BillingMonth = "Billing_Month_i";
    public const string BillingPeriod = "Period_No_i";
    public const string BilledMemberId = "Billed_Member_ID_i";
    public const string InvoiceNumber = "Invoice_No_i";
    public const string BillingCategory = "Biling_Category_Id_i";
    public const string BatchSequenceNumber = "Batch_Seq_No_i";
    public const string BatchRecordSequenceNumber = "Batch_Rec_Seq_No_i";
    public const string BreakdownSerialNumber = "BM_BD_Sr_No_i";
    public const string ChargeCategory = "CHARGE_CATEGORY_ID_I";
    public const string MismatchTransaction = "ATTACHMENT_MISMATCH";

    public const string BillingCategoryUnlinked = "BILLING_CATEGORY_ID_I";
    public const string BillingYearUnlinked = "BILLING_YEAR_I";
    public const string BillingMonthUnlinked = "BILLING_MONTH_I";
    public const string BillingPeriodUnlinked = "BILLING_PERIOD_I";
    public const string BillingMemberUnlinked = "BILLING_MEMBER_ID_I";
    public const string BilledMemberUnlinked = "BILLED_MEMBER_ID_I";
    public const string InvoiceNumberUnlinked = "INVOICE_NO_I";
    public const string SubmissionDate = "LAST_UPDATED_DATE_I";
    public const string BatchSequenceNumberUnlinked = "BATCH_NO_I";
    public const string RecordSequenceNumberUnlinked = "SEQUENCE_NO_I";
    public const string BreakdownSerialNumberUnlinked = "BD_SR_NO_I";
    public const string OriginalFileName = "ORG_FILE_NAME_I";
    //public const string BillingCode = "AWB_Billing_Code_I";
    //public const string AWBSerialNumber = "AWB_Sr_No_I";

    //Output parameters
    public const string RecordDetails = "r_Cur_Memo_ID_o";
    public const string AttachmentDetails = "r_Cur_Attachment_o";

    //Function name
    public const string GetRecordsFunctionName = "GetSupportingDocumentRecord";
    public const string GetSupportingDocumentsFunctionName = "GetSupportingDocuments";
    public const string GetUnlinkedSupportingDocumentsFunctionName = "GetUnlinkedSupportingDocument";

    public const string InvoiceId = "InvoiceId";

    public const string UpdateNumberOfAttachmentsFunctionName = "UpdateNoOfAttachments";
    public const string RecordTypeId = "RECORD_TYPE_I";
    public const string RecordId = "RECORD_ID_I";

    #endregion

    #region GetSupportingDocumentSearchResult Constants

    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string InvoiceTypeParameterName = "INVOICE_TYPE_I";
    public const string InvoiceNoParameterName = "INVOICE_NO_I";
    public const string SourceCodeParameterName = "SOURCE_CODE_I";
    public const string BillingCodeParameterName = "BILLING_CODE_I";
    public const string CGOBillingCodeParameterName = "BILLING_CODE_I";
    public const string CGOAWBSerialNumberParameterName = "AWB_NO_I";
    public const string BatchNoParameterName = "BATCH_NO_I";
    public const string SequenceNoParameterName = "SEQ_NO_I";
    public const string RMBMCMNumberParameterName = "RMBMCM_NO_I";
    public const string AWBSerialNumberParameterName = "AWB_SR_NO_I";
    public const string DocNoParameterName = "DOC_NO_I";
    public const string CouponNoParameterName = "COUPON_NO_I";
    public const string AttachmentIndOrigParameterName = "ATTACHMENT_IND_ORIG_I";
    public const string IsMismatchCaseParameterName = "IS_MISMATCH_CASE_I";
    public const string CutOffEventNameParameterName = "CUT_OFF_EVENT_NAME";

    public const string GetSupportingDocumentSearchResultFunctionName = "GetSupportingDocumentSearchResult";
    public const string GetCargoSupportingDocumentSearchResultFunctionName = "GetCargoSupportingDocumentSearchResult";
 
    #endregion

    #region GetPayableSupportingDocumentSearchResult Constants
    
    public const string PayableBillingYearParameterName = "BILLING_YEAR_I";
    public const string PayableBillingMonthParameterName = "BILLING_MONTH_I";
    public const string PayableBillingPeriodParameterName = "BILLING_PERIOD_I";
    public const string PayableBillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string PayableBilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string PayableInvoiceTypeParameterName = "INVOICE_TYPE_I";
    public const string PayableInvoiceNoParameterName = "INVOICE_NO_I";
    public const string PayableSourceCodeParameterName = "SOURCE_CODE_I";
    public const string PayableBatchNoParameterName = "BATCH_NO_I";
    public const string PayableSequenceNoParameterName = "SEQ_NO_I";
    public const string PayableRMBMCMNumberParameterName = "RMBMCM_NO_I";
    public const string PayableDocNoParameterName = "DOC_NO_I";
    public const string PayableCouponNoParameterName = "COUPON_NO_I";
    public const string PayableAttachmentIndValidParameterName = "ATTACHMENT_IND_VALID_I";
    public const string PayableCutOffEventNameParameterName = "CUT_OFF_EVENT_NAME";
    public const string PayableChargeCodeParameterName = "CHARGE_CATEGORY_ID_I";


    public const string GetPayableSupportingDocumentSearchResultFunctionName = "GetPayableSupportingDocumentSearchResult";

   

    #endregion

    public const string CgoPayableBillingYearParameterName = "BILLING_YEAR_I";
    public const string CgoPayableBillingMonthParameterName = "BILLING_MONTH_I";
    public const string CgoPayableBillingPeriodParameterName = "BILLING_PERIOD_I";
    public const string CgoPayableBillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string CgoPayableBilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string CgoPayableInvoiceTypeParameterName = "INVOICE_TYPE_I";
    public const string CgoPayableInvoiceNoParameterName = "INVOICE_NO_I";
    public const string CgoPayableBatchNoParameterName = "BATCH_NO_I";
    public const string CgoPayableSequenceNoParameterName = "SEQ_NO_I";
    public const string CgoPayableRMBMCMNumberParameterName = "RMBMCM_NO_I";
    public const string CgoPayableBillingCodeParameterName = "AWB_BILLING_CODE_I";
    public const string CgoPayableAWBSerialNumberParameterName = "AWB_SR_NO_I";
    public const string CgoPayableAttachmentIndOriginParameterName = "ATTACHMENT_IND_VALID_I";
    
    public const string GetCargoPayableSupportingDocumentSearchResultFunctionName = "GetCargoPayableSupportingDocumentSearchResult";

    // SCP162502  Form C - AC OAR Jul P3 failure - No alert received
    public const string FormCDetailIdI = "FORMC_DETAIL_ID_I";
    public const string SkipSuppDocLinkingDeadlineCheck = "SKIP_SUP_DOC_LINK_DEAD_CHECK";
    public const string FolderNameO = "FOLDER_NAME_O";
    public const string GetFormCFolderName = "GetFormCFolderName";
  }
}
