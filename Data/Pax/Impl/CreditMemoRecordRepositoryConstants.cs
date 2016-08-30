namespace Iata.IS.Data.Pax.Impl
{
  internal class CreditMemoRecordRepositoryConstants
  {
    #region GetCreditMemoDuplicateCount Constants

    public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string CreditMemoNumberParameterName = "CREDIT_MEMO_NO_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
    
    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetCmDuplicateCountFunctionName = "GetCMDuplicateCount";
    public const string GetCreditMemoDuplicateCountFunctionName = "GetCreditMemoCouponDuplicateCount";

    #endregion 

    #region LoadStrategy constants

    public const string CreditIdParameterName = "CREDIT_MEMO_ID_I";
    public const string CMCouponIdparameterName = "CREDIT_MEMO_COUPON_ID_I";
    #endregion
  }
}
