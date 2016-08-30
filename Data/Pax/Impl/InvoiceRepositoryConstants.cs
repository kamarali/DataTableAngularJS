
namespace Iata.IS.Data.Pax.Impl
{
  internal static class InvoiceRepositoryConstants
  {
    #region GetDerivedVatDetails Constants

    public const string InvoiceIdParameterName = "INVOICE_ID_I";
    public const string ParameterNameInvoiceNo = "INVOICE_NO_I";
    public const string ParameterNameBillingMonth = "BILLING_MONTH_I";
    public const string ParameterNameBillingYear = "BILLING_YEAR_I";
    public const string ParameterNameBillingPeriod = "BILLING_PERIOD_I";
    public const string ParameterNameBillingMemberId = "BILLING_MEMBER_ID_I";
    public const string ParameterNameBilledMemberId = "BILLED_MEMBER_ID_I";
    public const string ParameterNameBillingCode = "BILLING_CODE_ID_I";
    public const string ParameterNameInvoiceStatusId = "INVOICE_STATUS_ID_I";
    public const string ParameterNameCheckValueConfurmationStatus = "CHECK_VALUE_CONFIRM_I";
    public const string ParameterNameCouponSearchCriteriaString = "TICKET_COUPON_DOC_I";
    public const string ParameterNameSubmissionMethod = "SUBMISSION_METHOD_ID_I";
    public const string ParameterNameRejectionMemoNumber = "RM_NUMBER_I";
    public const string GetDerivedVatDetailsFunctionName = "GetDerivedVatDetails";
    //CMP#622: MISC Outputs Split as per Location ID
    public const string ParameterNameCreateNilFileMiscLocation = "IS_REQ_MISC_LOC_NILFILE_I";

    #endregion 
    public const string ParameterNameBillingYearForLinking = "L_BILLING_YEAR";
    public const string ParameterNameBillingMonthForLinking = "L_BILLING_MONTH";
    public const string ParameterNameBillingPeriodForLinking = "L_BILLING_PERIOD";
    #region

    #endregion
    #region GetNonAppliedVatDetails Constants

    public const string GetNonAppliedVatDetailsFunctionName = "GetNonAppliedVatDetails";

    #endregion 
    
    #region UpdatePrimeInvoiceTotal Constants

    public const string SourceIdParameterName = "SOURCE_CODE_I";
    public const string IsCouponDeleteParameterName = "IS_COUPON_DELETE_I";
    public const string UpdatePrimeInvoiceTotalFunctionName = "UpdatePrimeInvoiceTotal";

    #endregion 
        
    #region UpdateRMInvoiceTotal Constants

    public const string RejectionMemoIdParameterName = "REJECTION_MEMO_ID_I";
    public const string UpdateRMInvoiceTotalFunctionName = "UpdateRMInvoiceTotal";

    #endregion 


    #region UpdateBMInvoiceTotal Constants

    public const string BillingMemoIdParameterName = "BILLING_MEMO_ID_I";
    public const string UpdateBMInvoiceTotalFunctionName = "UpdateBMInvoiceTotal";

    #endregion 

    #region SubmitMiscInvoice Constants

    public const string IsLateSubmissionParameterName = "IS_LATE_SUBMISSION_I";
    public const string DsRequiredByParameterName = "DS_REQUIRED_BY_I";
    public const string ClearingHouseParameterName = "CLEARING_HOUSE_I";
    public const string SponsoredByMemberIdParameterName = "SPONSORED_BY_MEMBER_ID_I";
    public const string IsValidBillingPeriodParameterName = "IS_VALID_BILLING_PERIOD_I";
    public const string SubmitMiscInvoiceFunctionName = "SubmitMiscInvoice";

    #endregion 

    #region UpdateCMInvoiceTotal Constants

    public const string CreditMemoIdParameterName = "CREDIT_MEMO_ID_I";
    public const string UpdateCMInvoiceTotalFunctionName = "UpdateCMInvoiceTotal";

    #endregion 
            
    #region IsInvoiceExists Constants

    public const string YourInvoiceNumberParameterName = "YOUR_INVOICE_NO_I";
    public const string BillingMonthParameterName = "YOUR_INVOICE_BILLING_MONTH_I";
    public const string BillingYearParameterName = "YOUR_INVOICE_BILLING_YEAR_I";
    public const string BillingPeriodParameterName = "YOUR_INVOICE_BILLING_PERIOD_I";
    public const string ResultParameterName = "IS_EXISTS_O";
    public const string IsInvoiceExistsFunctionName = "IsInvoiceExists";

    #endregion 

    #region GetAchInvoiceCount Constants
    public const string BillingMemberId = "BILLING_MEMBER_ID_I";
    public const string BillingCategoryId = "BILLING_CATEGORY_ID_I";
    public const string AchBillingYear = "BILLING_YEAR_I";
    public const string AchBillingMonth = "BILLING_MONTH_I";
    public const string AchBillingPeriod = "BILLING_PERIOD_I";
    public const string SettlementMethodId= "SETTLEMENT_METHOD_ID_I";
    public const string Clearinghouse = "CLEARANCE_HOUSE_I";
    public const string AchResultParameterName = "INVOICE_COUNT_O";
    public const string AchInvoiceCountFunctionName = "GetAchInvoiceCount";

    #endregion 

    #region IsRefCorrespondenceNumberExists Constants

    public const string CorrespondenceNumberParameterName = "CORRESPONDENCE_NO_I";
    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string IsRefCorrespondenceNumberExistsFunctionName = "IsRefCorrespondenceNoExists";

    #endregion 

    #region IsInvoiceNumberExists Constants
    
    public const string InvoiceNumberParameterName = "INVOICE_NO_I";
    public const string InvoiceBillingYearParameterName = "BILLING_YEAR_I";

    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string IsInvoiceNumberExistsFunctionName = "ProcIsInvoiceNoExists";

    #endregion 
    
    #region GetFormDDerivedVatDetails Constants


    public const string GetFormDDerivedVatFunctionName = "GetFormDDerivedVatDetails";

    #endregion

    #region GetAttachmentIndOrgForInvoice Constants

    public const string GetAttachmentIndOrgForInvoiceFunctionName = "GetAttachmentIndOrgForInvoice";

    public const string AttachmentIndOrg = "R_ATT_IND_ORG_O";

    #endregion

    #region GetFormDNonAppliedVatDetails Constants

    public const string GetFormDNonAppliedVatFunctionName = "GetFormDNonAppliedVatDetails";

    #endregion 

    #region GetInvoiceMemberLocationInformation Constants

    public const string IsBillingMemberParameterName = "IS_BILLING_MEMBER_I";
    public const string GetInvoiceMemberLocationInformationFunctionName = "GetInvoiceMemberLocationInformation";

    #endregion

    #region GetMemberLocationInformation Constants

    public const string LocationIdParameterName = "LOCATION_ID_I";
    public const string GetMemberLocationInformationFunctionName = "GetLocationInformation";

    #endregion

    #region Update file invoice status

    public const string FileNameParameterName = "FILE_NAME_I";
    public const string UpdateFileInvoiceStatusFunctionName = "UpdateFileInvoiceStatus";

    #endregion 

    #region Update invoice status

    public const string InvoiceIdsParameterName = "INVOICE_IDS_I";
    public const string InvoiceStatusIdParameterName = "INVOICE_STATUS_ID_I";
    public const string UpdateInvoiceStatusFunctionName = "UpdateInvoiceStatus";

    #endregion 

    #region Update daily delivery status

    public const string InvIdsParameterName = "INVOICE_IDS_I";
    public const string DailyDeliveryStatusIdParameterName = "DAILY_DELIVERY_STATUS_I";

    #endregion 

    #region LoadStrategy Constants

    public const string BilledMemberIdStrategyLoadParameterName = "Billed_Member_ID_i";
    public const string BillingPeriodStrategyLoadParameterName = "Billing_Period_i";
    public const string BillingMonthStrategyLoadParameterName = "Billing_Month_i";
    public const string BillingYearStrategyLoadParameterName = "Billing_Year_i";
    public const string BillingCodeIdStrategyLoadParameterName = "Billing_Code_ID_i";
    public const string InvoiceStatusIdStrategyLoadParameterName = "Invoice_Status_ID_i";

    #endregion

    #region GetPaxInvoiceAudit
    public const string PaxInvoiceTransactionIdParameterName = "TRANSACTION_ID_I";
    public const string PaxInvoiceTransactionTypeParameterName = "TRANSACTION_TYPE_I";
    #endregion

    #region GetCargoInvoiceAudit
    public const string CargoInvoiceTransactionIdParameterName = "TRANSACTION_ID_I";
    public const string CargoInvoiceTransactionTypeParameterName = "TRANSACTION_TYPE_I";
    #endregion

    #region GetFormFSamplingConstant constants
    public const string ProvBillingMonthParameterName = "PROV_BILLING_MONTH_I";
    public const string ProvBillingYearParameterName = "PROV_BILLING_YEAR_I";
    public const string ErrorCodeParameterName = "ERROR_CODE_O";
    public const string SamplingConstantParameterName = "SAMPLING_CONST_O";
    public const string IsLinkingSucessfulParameterName = "IS_LINKING_SUCESSFUL_O";

    public const string GetFormFSamplingConstantFunctionName = "GetFormFSamplingConstant";
    

    #endregion

    #region GetRejectedTransactionDetails constants
    public const string GetRejectedTransactionDetailsFunctionName = "GetRejectedTransactionDetails";
    #endregion

    public const string GetBillingMemosForCorrespondenceFunctionName = "IsBillingMemoExistsForCorrespondence";

    public const string GetClaimFailedInvoices = "GetClaimFailedInvoices";

    public const string ValidateMemoFunctionName = "ValidateMemo";
    public const string BillingCodeParameterName = "BILLING_CODE_I";
    public const string UserIdParameterName = "USER_ID_I";

    #region UpdateSourceCodeTotalVat constants

    public const string UpdateSourceCodeVatFunctionName = "UpdateSourceCodeTotalVat";

    #endregion

    #region UpdateInvoiceOnReadyForBilling Constants

    public const string InvoiceIdOnReadyForBillingParameterName = "INVOICE_ID_i";
    public const string InvoiceBillingCatIdParameterName = "BILLING_CATEGORY_ID_i";
    public const string InvoiceBillingMemberIdParameterName = "BILLING_MEMBER_ID_i";
    public const string InvoiceBilledMemberIdParameterName = "BILLED_MEMBER_ID_i";
    public const string InvoiceBillingCodeIdParameterName = "BILLING_CODE_ID_i";
    public const string UpdateInvoiceOnReadyForBillingFunctionName = "UpdateInvoiceOnReadyForBilling";

    #endregion 

    #region -FinalizeSupportingDocument-

    public const string FinalizeSupportingDocumentFunctionName = "FinalizeSupportingDocument";
    //SCP 170146: Proposed improvement in Supporting Doc Linking Finalization Process
    public const string FinalizeSuppDocLinkFunctionName = "FinalizeSuppDocLinking";

    #endregion

    #region UpdateIsFileLogConstants Constants

    public const string ISFileLogId = "IS_FILE_LOG_ID_i";
    public const string FilePathParam = "FILE_PATH_I";
    public const string FileSenderReciver = "FILE_SENDER_RECEIVER_i";
    public const string FileProcessStartTime = "FILE_PROCESS_START_TIME_i";
    public const string FileStatus = "FILE_STATUS_ID_i";
    public const string LastUpdateBy = "LAST_UPDATED_BY_i";
    public const string UpdateISFileLogFunctionName = "UpdateISFileLog";
    public const string UpdateCorrReportFunctionName = "InsertToCorrReport";
    #endregion

    public const string ProcessId = "PID_I";
    public const string DeleteFileInvoiceStatsFunctionName = "DeleteFileInvoiceStats";

    #region IsValidBatchSequenceNo Constants

    //public const string InvoiceIdParameterName = "INVOICE_ID_I";
    public const string BatchRecordSequenceNoParameterName = "BATCH_REC_SEQ_NO_I";
    public const string BatchSequenceNoParameterName = "BATCH_SEQ_NO_I";
    public const string MemoId = "MEMO_ID_I";
    public const string IsUniqueNoParameterName = "IS_UNIQUE_NO_O";
    public const string IsValidBatchSequenceNoFunctionName = "IsValidBatchSequenceNo";

    #endregion 

    #region AddFileLogEntry Constants

    public const string IsFileLogId = "IS_FILE_LOG_ID_i";
    public const string FileName = "FILE_NAME_i";
    public const string InCommingOutGoing = "INCOMING_OUTGOING_i";
    public const string FileSenderReceiver = "FILE_SENDER_RECEIVER_i";
    public const string FileDateTime = "FILE_DATETIME_i";
    public const string BillingPeriod = "BILLING_PERIOD_i";
    public const string BillingMonth = "BILLING_MONTH_i";
    public const string FileReceivedSentOn = "FILE_RECEIVED_SENT_ON_i";
    public const string FileSenderReceiverIp = "FILE_SENDER_RECEIVER_IP_i";
    public const string FileLocation = "FILE_LOCATION_i";
    public const string FileStatusId = "FILE_STATUS_ID_i";
    public const string FileFormatId = "FILE_FORMAT_ID_i";
    public const string BillingCatId = "BILLING_CATEGORY_ID_i";
    public const string BillingYear = "BILLING_YEAR_i";
    public const string SenderReciverType = "SENDER_RECIEVER_TYPE_i";
    public const string FileVersion = "FILE_VERSION_i";
    public const string OutputDeleveryMethod = "OUTPUT_DELEVERY_METHOD_ID_i";
    public const string IsConsolidated = "IS_CONSOLIDATED_i";
    public const string FileStartTime = "FILE_START_TIME_I";
    public const string FileEndTime = "FILE_END_TIME_I";
    public const string ExpectedRespTime = "EXPECTED_RESPONCE_I";
    public const string UsageData = "USAGE_DATA_EXP_RESPO_HRS_I";
    public const string InsertoIsFileLogFunction = "InsertToIsFileLog";
    //CMP#622 : MISC Outputs Split as per Location ID
    public const string MiscLocationforFile = "MISC_MEM_LOCATION_I";
    public const string LastUpdatedBy = "LAST_UPDATED_BY_I";
    

    #endregion

    #region InsertUpdateFileProcessTime Constants

    public const string FileLogId = "IS_FILE_LOG_ID_I";
    public const string ProcessName = "PROCESS_NAME_I";
    public const string Filename = "FILE_NAME_I";
    public const string BillingCategory = "BILLING_CATEGORY_I";
    public const string FileSize = "FILE_SIZE_I";
    public const string InvoiceCount = "INVOICE_COUNT_I";
    public const string PrimeCouponCount = "PRIME_COUPON_CNT_I";
    public const string CpnTaxBreakdownCount = "CPN_TAXBD_CNT_I";
    public const string RmCount = "RM_COUNT_I";
    public const string RmCpnBreakdownCount = "RM_CPN_BD_CNT_I";
    public const string LineItemCnt = "LINE_ITEM_CNT_I";
    public const string LineItemDetCnt = "LINE_ITEM_DET_CNT_I";
    public const string FieldValueCnt = "FIELD_VALUE_CNT_I";
    public const string InsertUpdateFileProcessTime = "InsertUpdateFileProcessTime";

    #endregion

    #region UpdateExpiryDatePeriod Constants
    public const string ExpiryPeriodParameterName = "EXPIRY_PERIOD_I";
    public const string UpdateExpiryDatePeriodFunctionName = "UpdatePaxExpiryDatePeriod";
    #endregion

    #region Update Coupon status

    public const string CouponIdsParameterName = "PRIME_CPN_IDS_I";
    public const string AutoBillingCouponIdsParameterName = "AUTOBILL_CPN_IDS_I";
    public const string UpdateAutoBillCouponStatusFunctionName = "UpdateAutoBillCouponStatus";

    #endregion

    //#region UpdateInvoiceSetLAParameters

    //public const string UpdateInvoiceSetLaParametersFunctionName = "UpdateInvoiceSetLAParameters";

    //#endregion 

    #region Update Coupon status

    public const string FileLogIdsParameterName = "FILE_PURGED_IDS_I";
    public const string FileLogIdPurgedStatusParameterName = "IS_PURGED_I";
    public const string IsFileLogPurgedParameterName = "IS_FILE_LOG_PURGED_I";
    public const string UpdateFileLogPurgedStatusFunctionName = "UpdateFileLogPurgedStatus";

    #endregion

    #region Update duplicate Coupon DU mark

    public const string InvoiceIdparamName = "INVOICE_ID_I";
    public const string BillingMemberIdParamName = "BILLING_MEMBER_ID_I";
    public const string UpdateduplicateCouponDuMarkFunctionName = "UpdateduplicateCouponDuMark";

    #endregion


    //SCP85837: PAX CGO Sequence Number
    #region Update Sequence No

    public const string InvoiceIdparam = "INVOICE_ID_I";
    public const string IsUpdate = "R_IS_UPDATED";
    public const string UpdatePaxTransSeqNoWithInBatchFunctionName = "UpdatePaxTransSeqNoWithInBatch";
    #endregion

    #region Delete coupon

    public const string RejectionMemoCouponId = "RM_CPN_ID_I";
    public const string DeleteRMCoupon = "DeleteRMCoupon";

    public const string BillingMemoCouponId = "BM_CPN_ID_I";
    public const string DeleteBMCoupon = "DeleteBMCoupon";

    public const string CreditMemoCouponId = "CM_CPN_ID_I";
    public const string DeleteCMCoupon = "DeleteCMCoupon";

    #endregion

    #region Get AutoBilling Invoice Num Sequence
    public const string BillingMemberParam = "BILLING_MEMBER_ID_I";
    public const string BillingYearParam = "BILLING_YEAR_I";
    public const string InvoiceSequenceNumberParam = "INVOICE_SEQ_NO_O";
    public const string GetNextOfInvoiceNoSeqParam = "ProcGetNextInvoiceSeqNum";
    #endregion
    
    #region Constant Parameters to Call SP in order to Close Correspondence On Invoice Ready For Billing
    
    //SCP 152109: as discussed
    //Desc: False alert was generated for correspondence to raise BM even when BM was already raised. 
    //Problem identified to be because of future invoices not calling the SP to close the respective correspondences when marked RFB by the Job.
    //Date: 24-July-2013
    public const string CloseCorrespondenceOnInvoiceReadyForBilling = "CloseCorrespondenceOnInvoiceReadyForBilling";

    public const string InvoiceIdForCorrClosingParam = "INVOICE_ID_I";
    public const string BillingCategoryForCorrClosingParam = "BILLING_CATEGORY_ID";
    
    #endregion

    #region -FinalizeSupportingDocumentForDailyOutput-

    public const string FinalizeSupportingDocumentForDailyOutputFunctionName = "FinalizeSupportingDocumentForDailyOutput";
    public const string ParameterNameTargetDate = "TARGET_DATE_I";
    public const string ParameterNameNilFileRequired = "NIL_FILE_REQUIRED_I";
    public const string ParameterNameNilLocFileRequired = "NIL_LOC_FILE_REQUIRED_I";

    #endregion
  
    #region PopulateInvoiceReportStats Constants

    public const string InvoiceIdInputParameterName = "INVOICE_ID_I";
    public const string PopulateInvoiceReportStats = "PopulateInvoiceReportStats";

    #endregion
  
  }
}
