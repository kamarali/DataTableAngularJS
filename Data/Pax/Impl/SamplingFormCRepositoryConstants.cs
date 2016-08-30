namespace Iata.IS.Data.Pax.Impl
{
  public class SamplingFormCRepositoryConstants
  {
    #region GetSamplingFormCSourceTotalList Constants

    public const string ProvisionalBillingMonthParameterName = "PROV_BILLING_MONTH_I";
    public const string ProvisionalBillingYearParameterName = "PROV_BILLING_YEAR_I";
    public const string ProvisionalBillingMemberIdParameterName = "PROV_BILLING_MEMBER_ID_I";
    public const string FormCIdParameterName = "FORM_C_ID_I";
    public const string FromMemberIdParameterName = "FROM_MEMBER_ID_I";
    public const string InvoiceStatusIdParameterName = "INVOICE_STATUS_ID_I";
    public const string ListingCurrencyIdParameterName = "LISTING_CURRENCY_ID_I";
    public const string ListingCurrencyCodeNumParamaterName = "LISTING_CURRENCY_CODE_NUM_I";
    public const string FormCDeatilIdParameterName = "FORM_C_DETAIL_ID_I";
    public const string ProcessingCompletedOnDateTimeParameterName = "PROCESSING_COMPLETED_ON_I";
    public const string FormCProcessingCompletedOnDateTimeParameterName = "ATTACH_IND_I";
    public const string GetFormCSourceTotalListFunctionName = "GetFormCSourceList";
    public const string GetFormCListFunctionName = "GetFormCList";

    public const string FormCIdsParameterName = "FORM_C_IDS_I";
    public static string UpdateFormCStatusFunctionName = "UpdateFormCInvoiceStatus";
    public const string UpdateFormCSourceCodeTotal = "UpdateFormCSourceCodeTotal";

    #endregion

    #region GetFormCPayablesList Constants
    public const string ProvisionalBilledMemberIdParameterName = "PROV_BILLED_MEMBER_ID_I";
    public const string ToMemberIdParameterName = "TO_MEMBER_ID_I";
    public const string GetFormCPayablesListFunctionName = "GetFormCPayablesList";
    #endregion

    public const string InvoiceStatusIdsParameterName = "INVOICE_STATUS_IDS_I";

    public const string TicketIssueAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string TicketDocNumberParameterName = "TICKET_DOC_NO_I";
    public const string CouponNumberParameterName = "COUPON_NO_I";
    public const string ProvInvoiceNumberParameterName = "PROV_INVOICE_NO_I";
    public const string BatchNumberParameterName = "BATCH_NO_I";
    public const string SquenceNumberParameterName = "SEQ_NO_I";
    public const string RecordCountParameterName = "IS_EXISTS_O";
    public const string IsSamplingFormCRecordExistsMethodName = "IsSamplingFormCRecordExists";
  }
}
