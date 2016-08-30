using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Cargo.Impl
{
  internal static class CargoInvoiceRepositoryConstants
  {
    public const string InvoiceIdParameterName = "INVOICE_ID_I";
    public const string ParameterNameInvoiceNo = "INVOICE_NO_I";
    public const string ParameterNameBillingMonth = "BILLING_MONTH_I";
    public const string ParameterNameBillingYear = "BILLING_YEAR_I";
    public const string ParameterNameBillingPeriod = "BILLING_PERIOD_I";
    public const string ParameterNameBillingMemberId = "BILLING_MEMBER_ID_I";
    public const string ParameterNameBilledMemberId = "BILLED_MEMBER_ID_I";
    public const string ParameterNameBillingCode = "BILLING_CODE_ID_I";
    public const string ParameterNameInvoiceStatusId = "INVOICE_STATUS_ID_I";
    public const string ParameterNameCouponSearchCriteriaString = "TICKET_COUPON_DOC_I";

    public const string ErrorCodeParameterName = "ERROR_CODE_O";
    public const string TransactionTypeParameterName = "TRANSACTION_TYPE_I";
    public const string GetBillingMemosForCorrespondenceFunctionName = "IsCgoBillingMemoExistsForCorrespondence";
    public const string ValidateMemoFunctionName = "CGOValidateMemo";
    public const string BillingCodeParameterName = "BILLING_CODE_I";
    public const string AwbBillingCodeParameterName = "AWB_BILLING_CODE_I";
    public const string UserIdParameterName = "USER_ID_I";

    public const string AwbRecordIdParameterName = "AWB_ID_I";
    public const string GetRejectedTransactionDetailsFunctionName = "GetCargoRejectedTransactionDetails";
    public const string GetCargoBillingCodeVatTotalFunctionName = "GetCargoBillingCodeVatTotalDetails";
    public const string BmAwbRecordIdParameterName = "BM_AWB_ID_I";
    #region UpdateBillingCodeTotalVat constants

    public const string UpdateBillingCodeVatFunctionName = "UpdateBillingCodeTotalVat";

    #endregion

    #region GetNonAppliedVatDetails Constants

    public const string GetCgoNonAppliedVatDetails = "GetCgoNonAppliedVatDetails";

    #endregion 

    #region DerivedVatDetails Constants

    public const string GetCgoDerivedVatDetails = "GetCgoDerivedVatDetails";

    #endregion 

    #region UpdateAwbInvoiceTotal
    public const string BatchSeqNumber = "BATCH_SEQ_NO_I";
    public const string RecordSeqNumber = "REC_SEQ_NO_I";
    public const string UpdateAwbInvoiceTotal = "UpdateAwbInvoiceTotal";
    #endregion
    #region GetAwbRecordDuplicateCount Constants

    public const string TicketCouponNumberParameterName = "TICKET_COUPON_NO_I";
    public const string TicketDocNumberParameterName = "TICKET_DOC_NO_I";
    public const string IssuingAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string BillingMemberParameterName = "BILLING_MEMBER_ID_I";
    public const string BilledMemberParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";

    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetAwbRecordDuplicateCountFunctionName = "GetAwbRecordDuplicateCount";

    #endregion 

    #region IsValidBatchSequenceNo constants

    public const string BatchRecordSequenceNoParameterName = "BATCH_REC_SEQ_NO_I";
    public const string BatchSequenceNoParameterName = "BATCH_SEQ_NO_I";
    //SCP85837: PAX CGO Sequence No
    public const string BillingCode = "BILLING_CODE_I";
    public const string MemoId = "MEMO_ID_I";
    public const string IsUniqueNoParameterName = "IS_UNIQUE_NO_O";
   
    public const string IsValidBatchSequenceNoFunctionName = "IsCgoValidBatchSequenceNo";

    # endregion

    #region UpdateBMInvoiceTotal Constants

    public const string CargoBillingCodeIdParameterName = "BILLING_CODE_I";
    public const string IsAwbDeleteParameterName = "IS_AWB_DELETE_I";
    public const string CargoBillingMemoIdParameterName = "BILLING_MEMO_ID_I";
    public const string CargoBillingMemoUserIdParameterName = "USER_ID_I";
    public const string UpdateCargoBMInvoiceTotalFunctionName = "UpdateCargoBMInvoiceTotal";

    #endregion

    #region UpdateRMInvoiceTotal Constants

    public const string RejectionMemoIdParameterName = "REJECTION_MEMO_ID_I";
    public const string UpdateCargoRMInvoiceTotalFunctionName = "UpdateCargoRMInvoiceTotal";

    #endregion

    #region GetDuplicateAwbRecordCount Constants
    public const string AwbSerialNumberParameterName = "AWB_SER_NO_I";
    public const string AwbIssueingAirlineParameterName = "AWB_ISSUING_AIRLINE_I";
    public const string AwbBillingMemberParameterName = "BILLING_MEMBER_ID_I";
    public const string AwbCarriageFromParameterName = "CARRIAGE_FROM_I";
    public const string AwbCarriageToParameterName = "CARRIAGE_TO_I";
    public const string AwbIssueingDateParameterName = "AWB_ISSUE_DATE_I";
    public const string ProcGetAwbRecDupCount = "ProcGetAWBRecDupCount";
    #endregion

    #region ProcGetAwbRecSeqNumber Constants
    public const string AwbBatchSeqNumber = "BATCH_SEQ_NO_I";
    public const string InvoiceNumber = "INVOICE_NO_I";
    public const string AwbRecSeqNumber = "SEQUENCE_NO_O";
    public const string ProcGetAwbRecSeqNumber = "ProcGetAwbRecSeqNumber";
    #endregion

    #region UpdateCMInvoiceTotal Constants

    public const string CargoCreditMemoIdParameterName = "CREDIT_MEMO_ID_I";
    public const string CargoCreditMemoUserIdParameterName = "USER_ID_I";
    public const string UpdateCargoCMInvoiceTotalFunctionName = "UpdateCargoCMInvoiceTotal";

    #endregion

    #region GetCargoTransactionBatchAndSequenceNumber
    public const string GetBatchAndSequenceNumber = "GetCargoTransactionBatchAndSequenceNumber";
    public const string InvoiceIdParameter = "INVOICE_ID_I";
    public const string TransactionTypeIdParameter = "TRANSACTION_TYPE_ID_I";
    public const string BatchNumberParameter = "BATCH_NUMBER_O";
    public const string SequenceNumberParameter = "SEQUENCE_NUMBER_O";
    #endregion

    #region UpdateExpiryDatePeriod Constants
    public const string TransactionIdParameterName = "TRANSACTION_ID_I";
    public const string ExpiryPeriodParameterName = "EXPIRY_PERIOD_I";
    public const string UpdateExpiryDatePeriodFunctionName = "UpdateCGOExpiryDatePeriod";
    #endregion

    #region Delete AWB constants

    public const string RejectionMemoAwbId = "RM_AWB_ID_I";
    public const string DeleteRejectionMemoAwb = "DeleteRMAwb";

    public const string BillingMemoAwbId = "BM_AWB_ID_I";
    public const string DeleteBillingMemoAwb = "DeleteBMAwb";

    public const string CreditMemoAwbId = "CM_AWB_ID_I";
    public const string DeleteCreditMemoAwb = "DeleteCMAwb";

    #endregion

    //SCP85837 :- PAX CGO Sequence Number 
    #region Update Sequence No in serial order

    public const string InvoiceIdparam = "INVOICE_ID_I";
    public const string IsUpdate = "R_IS_UPDATED";
    public const string UpdateCgoSequenceNoFunctionName = "UpdateCgoTransSeqNoWithInBatch";
    #endregion
  }
}
