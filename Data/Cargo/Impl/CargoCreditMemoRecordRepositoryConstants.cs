namespace Iata.IS.Data.Cargo.Impl
{
  internal class CargoCreditMemoRecordRepositoryConstants
  {
    #region GetCreditMemoDuplicateCount Constants

    public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
    public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
    public const string CreditMemoNumberParameterName = "CREDIT_MEMO_NO_I";
    public const string BillingMonthParameterName = "BILLING_MONTH_I";
    public const string BillingYearParameterName = "BILLING_YEAR_I";
    public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
    
    public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetCmDuplicateCountFunctionName = "GetCargoCMDuplicateCount";
    public const string GetCreditMemoDuplicateCountFunctionName = "GetCreditMemoCouponDuplicateCount";

    #endregion 

    #region GetAwbDuplicateCount Constants

    public const string CMAwbSerialNumberParameterName = "AWB_SERIAL_NO_I";
    public const string CMAwbIssuingAirlineParameterName = "AWB_ISSUING_AIRLINE_I";
    public const string AwbBillingCodeParameterName = "AWB_BILLING_CODE_I";
    public const string BillingMemberParameterName = "BILLING_MEMBER_ID_I";
    public const string BilledMemberParameterName = "BILLED_MEMBER_ID_I";
    public const string AwbBillingMonthParameterName = "BILLING_MONTH_I";
    public const string AwbBillingYearParameterName = "BILLING_YEAR_I";
    public const string AwbDuplicateCountParameterName = "DUPLICATE_COUNT_O";
    public const string GetCreditMemoAwbDuplicateCountFunctionName = "GetCargoCreditMemoAwbDuplicateCount";


    #endregion 

    #region LoadStrategy constants

    public const string CreditMemoIdParameterName = "CREDIT_MEMO_ID_I";
    public const string CreditMemoAwbIdparameterName = "CREDIT_MEMO_AWB_ID_I";
    #endregion
  }
}
