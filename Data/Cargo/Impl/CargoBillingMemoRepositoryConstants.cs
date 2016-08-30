namespace Iata.IS.Data.Cargo.Impl
{
    internal static class CargoBillingMemoRepositoryConstants
    {
        #region GetBMDuplicateCount Constants

        public const string BilledMemberIdParameterName = "BILLED_MEMBER_ID_I";
        public const string BillingMemberIdParameterName = "BILLING_MEMBER_ID_I";
        public const string BillingMemoNumberParameterName = "BILLING_MEMO_NO_I";
        public const string BillingMonthParameterName = "BILLING_MONTH_I";
        public const string BillingYearParameterName = "BILLING_YEAR_I";
        public const string BillingPeriodParameterName = "BILLING_PERIOD_I";
        public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
        public const string GetBMDuplicateCountFunctionName = "GetCargoBMDuplicateCount";
        public const string GetBillingMemoCouponDuplicateCountFunctionName = "GetCargoBillingMemoAwbDuplicateCount";

        #endregion

        public const string BillingMemoIdParameterName = "BILLING_MEMO_ID_I";
        public const string BillingMemAwbIdParameterName = "BILLING_MEMO_AWB_ID_I";
    }
}
