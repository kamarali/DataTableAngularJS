namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MiscInvoiceRepositoryConstants
  {
    #region GetMiscUaptLineItemNumberUpdate Constants

    public const string InvoiceIdParameterName = "INVOICE_ID_I";
    public const string NewInvoiceIdParameterName = "NEW_INVOICE_ID_I";
    public const string OldInvoiceIdParameterName = "OLD_INVOICE_ID_I";

    public const string LineItemIdParameterName = "LINE_ITEM_ID_I";
    public const string LineItemNoParameterName = "LINE_ITEM_NO_I";
    public const string RollupValueParameterName = "ROLLUP_I";

    public const string SerialNumberParameterName = "SR_NO_I";
    public const string IsLineItemNumberParameterName = "IS_LINE_ITEM_NO";
    public const string UpdateLineItemNumberFunctionName = "UpdateMiscUatpLineItemNumber";

    public const string TotalTaxAmountParameterName = "TOTAL_TAX_AMOUNT_I";
    public const string TotalVatAmountParameterName = "TOTAL_VAT_AMOUNT_I";
    public const string TotalAddOnAmountParameterName = "TOTAL_ADDON_AMOUNT_I";

    #endregion

    #region UpdateMiscUatpInvoiceTotal Constants

    public const string UpdateInvoiceTotalFunctionName = "UpdateMiscUatpInvoiceTotal";

    public const string CopyToRejectionInvoice = "CopyToRejectionInvoice";

    // SCP324672: Wrong amount invoice
    public const string UpdateMUInvoiceSummaryFunctionName = "UpdateMiscUatpInvoiceSummary";

    #endregion

    #region GetLineItemDetailsNavigations

    public const string CurrentLineItemDetailParameterName = "CURRENT_LINE_ITEMDETAIL_ID_I";
    //public const string LineItemIdParameterName = "LINE_ITEM_ID_I";
    public const string IsOnCreateParameterName = "IS_ON_CREATE_I";
    public const string LineItemDetailNavigationFunctionName = "GetLineItemDetailsNavigation";

    #endregion

    #region UpdateLineItemDetailEndDate

    public const string LineItemEndDateParameterName = "END_DATE_I";
    public const string UpdateLineItemDetailEndDateFunctionName = "UpdateLineItemDetailEndDate";

    #endregion

    #region UpdateMiscUatpFileInvoiceStatus
    public const string FileNameParameterName = "FILE_NAME_I";
    public const string ProcessId = "PID_I";
    public const string BillingMemeberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string IsBadFileExists = "IS_BAD_FILE_EXISTS_I";
    public const string LaFlag = "LA_FLAG_I";
    public const string UpdateInvoiceAndFileStatusFunctionName = "UpdateInvoiceAndFileStatus";
    #endregion

    #region UpdateCorrespondenceStatus
    public const string UpdateCorrespondenceStatusFunctionName = "UpdateCorrespondenceStatus";
    #endregion

    #region ValidateMiscUatpInvoiceLocation

    public const string LocationValidationResultParameterName = "RES_O";
    public const string ValidateLocationFunctionName = "ValdiateMiscUatpInvoiceLocation";

    #endregion

    #region GetDerivedVat

    public const string DerivedVatInvoiceIdParameterName = "INVOICE_ID_I";
    public const string GetMiscUatpDerivedVatFunctionName = "GetMiscUatpDerivedVat";

    #endregion

    public const string DeleteLineItemFunctionName = "DeleteLineItem";

    #region DeleteLineItemDetail
    public const string DeleteLineItemDetailFunctionName = "DeleteLineItemDetail";
    public const string LineItemDetailIdParameterName = "LINE_ITEM_DETAIL_ID_I";
    #endregion

    #region LoadStrategy Constants
    public const string InvoiceNoParameterName = "INVOICE_NO_I";
    public const string BilledMemberParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string InvoiceStatusParameterName = "INVOICE_STATUS_IDS_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string LoadStrategyParameterName = "LOAD_STRATEGY_I";
    public const string BillingCategoryIdParameterName = "BILLING_CATEGORY_ID_I";
    public const string RejectionStageParameterName = "REJECTION_STAGE_I";
    public const string ChargeCategoryIdParameterName = "CHARGE_CATEGORY_I";
    public const string ChargeCodeIdParameterName = "CHARGE_CODE_I";
    public const string InclusionStatusIdParameterName = "INCLUSION_STATUS_I";
    public const string IsWbGenDateParameterName = "IS_WEB_FILE_GENERATION_DATE_I";
    public const string SubmissionMethodParameterName = "SUBMISSION_METHOD_ID_I";
    public const string OnBehalfTransmitterParameterName = "TRANSMITTER_CODE";
    public const string DailyDeliveryStatusParameterName = "DAILY_DELIVERY_STATUS_I";
    public const string TargetDateParameterName = "TARGET_DATE_I";
    public const string OutputType = "OUTPUT_TYPE_I";
    public const string LocationId = "LOCATION_ID_I";
    #endregion

    #region Misc Isweb Invoices LoadStrategy Constants
    
    public const string BillingMemberParameter = "BILLING_MEMBER_ID_I";
    public const string InvoiceStatusIdParameter = "INVOICE_STATUS_IDS_I";
    public const string BillingCategoryIdParameter = "BILLING_CATEGORY_ID_I";
    public const string IsWbGenDateParameter = "IS_WEB_FILE_GENERATION_DATE_I";
    public const string IsRegenerateParameter = "IS_REGENERATION_I";
    public const string OutputTypeParameter = "IS_LOCATION_OUTPUT_I";
    public const string LocationIdParameter = "LOCATION_ID_I";
    public const string LoadStrategyParameters = "LOAD_STRATEGY_I";
    
    #endregion

    #region UpdateInclusionStatus

    public const string InclusionStatusParameterName = "INCLUSION_STATUS_I";
    public const string InvoiceIdsParameterName = "INVOICE_IDS_I";
    public const string IsUpdateGenerationDateParameterName = "IS_UPDATE_GENERATION_DATE_I";
    public const string UpdateInclusionStatusFunctionName = "UpdateInclusionStatus";

    #endregion

    public const string FilterParameterName = "FILTER_I";
    public const string ExceptionStringOutputParameterName = "EXCEPTION_CODE_O";
    public const string GetExceptionCodeFunctionName = "GetExceptionCodes";

    #region UpdateExpiryDatePeriod Constants
    public const string TransactionIdParameterName = "TRANSACTION_ID_I";
    public const string TransactionTypeParameter = "TRANSACTION_TYPE_I";
    public const string ExpiryPeriodParameterName = "EXPIRY_PERIOD_I";
    public const string UpdateExpiryDatePeriodFunctionName = "UpdateMUExpiryDatePeriod";
    #endregion

    //#region UpdateInvoiceSetLAParameters

    //public const string UpdateInvoiceSetLaParametersFunctionName = "UpdateInvoiceSetLAParameters";

    //#endregion


    #region Validate Misc TAX, VAT and Add On charge Breakdown

    public const string ProcValidateMiscBreakdown = "ProcValidateMiscBreakdown";

    public const string TransactionId = "TRANS_ID";
    public const string TransactionType = "TRANS_TYPE";
    public const string TotalTaxAmount = "TOTAL_TAX_AMOUNT";
    public const string SumTaxBreakdown = "SUM_TAX_BRDOWN";
    public const string TotalVatAmount = "TOTAL_VAT_AMOUNT";
    public const string SumVatBreakdown = "SUM_VAT_BRDOWN";
    public const string TotalAddonAmount = "TOTAL_ADDON_AMOUNT";
    public const string SumAddonBreakdown = "SUM_ADDON_BRDOWN";
    public const string ReturnResult = "RETURNRESULT";

    #endregion


    
    #region Validate Misc TAX, VAT Total Amount and  Its  Breakdown Sum Total

    public const string ProcValidateMiscTotalAmount = "ProcValidateMiscTotalAmount";
    public const string InvoiceId = "INV_ID";
    public const string ReturnParam = "RETURNRESULT";

    #endregion

    #region Update invoice Status for duplicate BM

    public const string ISFileLogIdInput = "IS_FILE_LOG_ID_I";
    public const string MUCorrBMDUCheckFunctionName = "MUCorrBMDUCheck";
	public const string MUInvRMDUCheckFunctionName = "MUInvRMDUCheck";

    #endregion
    
  }
}
