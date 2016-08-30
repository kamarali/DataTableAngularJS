namespace Iata.IS.Data.Reports.Cargo.RejectionAnalysis.Impl
{
    public class CgoRejectionAnalysisRecConstant
    {
      // CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
      // input parameter updated (From Year Month and To Year Month)
        public const string FromBillingMonth = "V_FROM_BILLING_MONTH";
        public const string FromBillingYear = "V_FROM_BILLING_YEAR";
        public const string ToBillingMonth = "V_TO_BILLING_MONTH";
        public const string ToBillingYear = "V_TO_BILLING_YEAR";

        public const string LoginEntityId = "V_LOGIN_ENTITY_ID";
        public const string ToEntityId = "V_TO_ENTITY_ID";
        public const string CurrencyCode = "V_CURRENCY_CODE";
        public const string CgoRejAnalysisRec = "GetCgoRejAnalysisRec";
        public const string CgoRejAnalysisPay = "GetCgoRejAnalysisPay";
    }
}
