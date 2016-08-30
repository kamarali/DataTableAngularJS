namespace Iata.IS.Data.Isr.Impl
{
  public class ValueRequestRepositoryConstants
  {
    public const string TicketDocNumberParameterName = "TICKETDOCUEMNTNUMBER_I";
    public const string CouponNumberParameterName = "COUPONNUMBER_I";
    public const string IssuingAirlineParameterName = "TICKET_ISSUING_AIRLINE_I";
    public const string BillingAirlineIdParameterName = "BILLING_AIRLINE_ID_I";
    public const string CouponCountParameterName = "TOTAL_COUNT_O";

    public const string CouponStatusParameterName = "COUPON_STATUS_ID_I";
    public const string CouponStatusDescriptionParameterName = "CPN_STATUS_DESCRIPTION_I";
    public const string ResponceFileNameParameterName = "RESPONSE_FILE_NAME_I";
    public const string ResponceDateParameterName = "RESPONSE_DATE_I";
    public const string IrrgReportParameterName = "INCLUDED_IN_IRREG_REPORT_I";
    public const string CouponUpdateResultParameterName = "R_RESULT_O";

    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string IsrFileRequriedParameterName = "IS_STAUS_UPDATED_O";

    public const string SubmissionIdParameterName = "SUBMISSION_METHOD_ID_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string IsrCouponDupParameterName = "R_CPNDUP_O";

    public const string AutoBillFileName = "FILE_NAME_I";
    public const string IsBadFileExists = "IS_BAD_FILE_EXISTS_I";

    public const string UpdateABCouponStatusFunctionName = "UpdateABCouponStatus";

    public const string GetValueRequestCouponCountFunctionName = "GetValueRequestCouponCount";

    public const string UpdateValueRequestCouponFunctionName = "UpdateValueRequestCoupon";

    public const string GetIsIsrFileRequriedFunctionName = "GetIsIsrFileRequried";

    public const string ChkIsIsrDuplicateCouponFunctionName = "ChkDuplicateIsrCoupon";

    #region Update Flag for "Included in Irregularity Report" in coupons

    public const string UnRequestedCouponIDs = "UNREQ_CPN_IDS_I";
    public const string RequestedCouponIDs = "REQ_CPN_IDS_I";
    public const string UpdateIncludedInIRReportFlag = "UpdateIncludedInIRReportFlag";

    #endregion
  }
}
