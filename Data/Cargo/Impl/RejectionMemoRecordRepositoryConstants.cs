
namespace Iata.IS.Data.Cargo.Impl
{
  internal class RejectionMemoRecordRepositoryConstants
  {
    #region GetRejectionMemoDuplicateCount Constants

    public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string RejectionMemoNumberParameterName = "REJECTION_MEMO_NO_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetRMDuplicateCountFunctionName = "GetCgoRMDuplicateCount";
    #endregion 

    #region GetDuplicateRejectionMemoNumbers Constants

    public const string BilledMemberIdRMParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingMemberIdRMParameterName = "BILLING_MEMBER_ID_I";
    public const string RejectionMemoNumberRMParameterName = "REJECTION_MEMO_NOS_I";
    public const string BillingYearRMParameterName = "BILLING_YEAR_I";
    public const string BillingMonthRMParameterName = "BILLING_MONTH_I";
    public const string BillingPeriodRMParameterName = "BILLING_PERIOD_I";
    public const string DuplicateRejectionMemoNosParameterName = "DUPLICATE_RM_NOS_O";
    public const string GetDuplicateRejectionMemoNumbersFunctionName = "GetCargoDuplicateRejectionMemoNumbers";
    #endregion
    #region IsRMCouponExists Constants

    public const string TicketIssueAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string TicketDocNumberParameterName = "TICKET_DOC_NO_I";
    public const string CouponNumberParameterName = "COUPON_NO_I";
    public const string InvoiceNumberParameterName = "INVOICE_NO_I";
    public const string RMNumberParameterName = "RM_NO_I";
    public const string BillingMemoNumberParameterName = "BM_NO_I";
    public const string CreditMemoNumberParameterName = "CM_NO_I";
    public const string CouponCountParameterName = "IS_EXISTS_O";
    public const string IsRMCouponExistsFunctionName = "IsExistsRMCoupon";


    #endregion 

    #region IsRMLinkingExists Constants

    
    public const string InvoiceIdParameterName = "INVOICE_ID_I";
    public const string CorrespondenceIdParameterName = "CORRESPONDANCE_NO_I";
    public const string ResultCountParameterName = "IS_EXISTS_O";
    public const string IsRMLinkingExistsFunctionName = "ProcIsRMLinkingExists";

    #endregion

    #region GetRMLinkingDetails Constants

    public const string ReasonCodeParameterName = "REASON_CODE_I";
    public const string YourInvoiceNumberParameterName = "YOUR_INVOICE_NUMBER_I";
    public const string InvBillingMonthParameterName = "BILLING_MONTH_I";
    public const string InvBillingYearParameterName = "BILLING_YEAR_I";
    public const string InvBillingPeriodParameterName = "BILLING_PERIOD_I";
    public const string BMCMNumberParameterName = "BM_CM_NUMBER_I";
    
    public const string YourRMNumberParameterName = "YOUR_RM_NUMBER_I";
    public const string BMCMIndicatorIdParameterName = "BM_CM_IND_ID_I";
    public const string RejectionStageParameterName = "REJECTION_STAGE_I";
    public const string InvBillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string InvBilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string CheckLinkingParameterName = "CHECK_LINKING_I";
    public const string RejectingInvoiceIdParameterName = "REJECTING_INVOICE_ID_I";
    public const string CurrencyConversionFactorParameterName = "CURR_CONV_FACTOR_O";
    public const string IsLinkingSuccessfulParameterName = "IS_LINKING_SUCCESSFUL_O";
    public const string HasCouponBreakdownParameterName = "HAS_COUPON_BD_O";
    public const string ErrorCodeParameterName = "ERROR_CODE_O";
    public const string MemoAmountsParameterName = "R_CUR_MEMO_AMOUNTS_O";
    public const string MemoRecordsParameterName = "R_CUR_MEMO_DET_O";

    public const string IgnoreValidationParameterName = "IGNORE_VALIDATION_I";
    #endregion

    #region GetLinkedMemoAmountDetails Constants

    public const string MemoIdParameterName = "MEMO_ID_I";
    public const string MemoReasonCodeParameterName = "REASON_CODE_I";
    public const string MemoRejectedInvIdParameterName = "REJECTED_INVOICE_ID_I";
    public const string MemoBMCMIndicatorIdParameterName = "BM_CM_IND_ID_I";
    public const string MemoRejectionStageParameterName = "REJECTION_STAGE_I";
    public const string RejMemoAmountsParameterName = "R_CUR_MEMO_AMOUNTS_O";

    #endregion

    public const string RejectionMemoIdParameterName = "REJECTION_MEMO_ID_I";
    public const string InheritRMAwbDetailsFunctionName = "InheritRMAwbDetails";
    #region  - LoadStategy constants -

    public const string RmIdParameterName = "REJECTION_MEMO_ID_I";
    public const string CorrespondenceIdParamName = "CORRESPONDENCE_ID_I";
    public const string RejectionMemoAwbIdParameterName = "REJECTION_MEMO_AWB_ID_I";
    #endregion

    public const string InheritRMCouponDetails = "InheritRMCouponDetails";

    public const string BillingCodeParameterName = "BILLING_CODE_I";
    public const string ValidateRMAcceptableAmountDiffFunctionName = "ValidateRejectionMemoAcceptableAmountDifference";

    // CMP#650
    public const string TransactionTypeParameterName = "TRANSACTION_TYPE_I";
    public const string YourTransactionReasonCodeParameterName = "YOUR_REASON_CODE_O";

    #region CMP#674 - PROC_GET_PAX_RM_COUPONS Constants

    public const string InvoiceId = "INVOICE_ID_I";
    public const string RejectionMemoId = "REJECTION_MEMO_ID_I";
    public const string YourInvoiceNo = "YOUR_INVOICE_NO_I";
    public const string YourRejectionNo = "YOUR_REJ_NO_I";
    public const string YourInvoiceYear = "YOUR_INV_BILL_YEAR_I";
    public const string YourInvoiceMonth = "YOUR_INV_BILL_MONTH_I";
    public const string YourInvoicePeriod = "YOUR_INV_BILL_PERIOD_I";
    public const string IsCargoYourRejectionCouponDroppedFunctionName = "IsCargoYourRejectionCouponDropped";

    #endregion

  }
}
