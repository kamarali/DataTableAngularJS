using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Cargo.Impl
{
    internal static class CargoBillingMemoAwbRepositoryConstants
    {
        #region GetCouponDuplicateCount Constants

        public const string BMAwbSerialNumberParameterName = "AWB_SERIAL_NO_I";
        public const string BMAwbIssuingAirlineParameterName = "AWB_ISSUING_AIRLINE_I";
        public const string BillingMemberParameterName = "BILLING_MEMBER_ID_I";
        public const string BilledMemberParameterName = "BILLED_MEMBER_ID_I";
        public const string BillingMonthParameterName = "BILLING_MONTH_I";
        public const string BillingYearParameterName = "BILLING_YEAR_I";
        public const string DuplicateCountParameterName = "DUPLICATE_COUNT_O";
        public const string GetBillingMemoAwbDuplicateCountFunctionName = "GetCargoBillingMemoAwbDuplicateCount";
        public const string AwbBillingCodeParameterName = "AWB_BILLING_CODE_I";

        #endregion 
    }
}
