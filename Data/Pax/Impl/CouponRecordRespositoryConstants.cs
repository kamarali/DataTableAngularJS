namespace Iata.IS.Data.Pax.Impl
{
  internal class CouponRecordRespositoryConstants
  {
    /// <summary>
    /// Added for performance reasons.
    /// </summary>
    private CouponRecordRespositoryConstants()
    {
    }

    #region GetCouponDuplicateCount Constants

    public const string TicketCouponNumberParameterName = "TICKET_COUPON_NO_I";
    public const string TicketDocNumberParameterName = "TICKET_DOC_NO_I";
    public const string IssuingAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string BillingMemberParameterName = "BILLING_MEMBER_ID_I";
    public const string BilledMemberParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";

    public const string SourceCodeIdParameterName = "SOURCE_CODE_I";

    
    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetCouponNoDuplicateCountFunctionName = "GetCouponNoDuplicateCount";

    #endregion 

    #region GetForDLinkedCouponDetails Constants
    public const string InvoiceIdParameterName = "INVOICE_ID_I";
    public const string GetFormDLinkedCouponDetails = "GetFormDLinkedCouponDetails";
    #endregion

    #region GetForDLinkedCouponDetails Constants

    public const string ProvBillingMemberIdParameterName = "PROV_BILLING_MEMBER_ID_I";
    public const string FromMemberIdParameterName = "FROM_MEMBER_ID_I";
    public const string ProvBillingYearParameterName = "PROV_BILLING_YEAR_I";
    public const string ProvBillingMonthParameterName = "PROV_BILLING_MONTH_I";

    public const string GetLinkedCouponsForFormC = "GetLinkedCouponsForFormC";
    #endregion

    #region Load Strategy

    public const string PrimeCouponIdParameterName = "COUPON_ID_I";
    #endregion
  }
}
