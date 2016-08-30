namespace Iata.IS.Data.Pax.Impl
{
  internal class RMCouponBreakdownRecordRepositoryConstants 
  {
    #region GetRMDetails
    public const string RejectionStageParameterName = "REJECTION_STAGE_I";
    public const string TicketIssuingAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string TicketDocNumberParameterName = "TICKET_DOC_NO_I";
    public const string CouponNumberParameterName = "COUPON_NO_I";
    public const string YourInvoiceNumberParameterName = "YOUR_INV_NO_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetRMCouponDuplicateCountFunctionName = "GetRMCouponDuplicateCount";
    #endregion
   
    #region GetRMCouponLinkingDetails

    public const string CpnTicketIssuingAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string CouponNoParameterName = "COUPON_NO_I";
    public const string TicketDocNoParameterName = "TICKET_DOC_NO_I";
    public const string RejectionMemoIdParameterName = "REJECTION_MEMO_ID_I";
    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string ErrorCodeParameterName = "ERROR_CODE_O";
    public const string CouponAmountsParameterName = "R_CUR_COUPON_AMOUNTS_O";
    public const string CouponListParameterName = "R_CUR_COUPON_LIST_O";
    public const string CouponTaxParameterName = "R_CUR_TAX_O";

    #endregion

    #region GetLinkedCouponAmountDetails

    public const string CouponIdParameterName = "COUPON_ID_I";

    #endregion
   
  }
}
