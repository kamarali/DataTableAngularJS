using System;
namespace Iata.IS.Data.ValueConfirmation.Impl
{
  public class RequestValueConfirmationRepositoryConstants
  {
    #region RequestValueConfirmationFileRepositoryContants Constants

    // Following constants are used for stored procedure PROC_GET_ACH_RECAP_SHEET.
    public const string ICHBillingYear = "ICH_BILLING_YEAR_i";
    public const string ICHBillingMonth = "ICH_BILLING_MONTH_i";
    public const string ICHBillingPeriod = "ICH_BILLING_PERIOD_i";
    public const string ACHBillingYear = "ACH_BILLING_YEAR_i";
    public const string ACHBillingMonth = "ACH_BILLING_MONTH_i";
    public const string ACHBillingPeriod = "ACH_BILLING_PERIOD_i";
    public const string MaxAllowedCouponsPerVCF = "MAX_ALLOWED_COUPONS_PER_VCF";
    public const string RequestId = "REQUEST_ID_I";
    public const string RegenerationFlag = "REGENERATION_FLAG_I";
    public const string GetRequestValueConfirmationData = "GetRequestValueConfirmationData";
 

    // Following constants are used for Stored Proc PROC_UPDATE_INV_FOR_REQ_VCF
    public const string InvoiceIds = "INVOICE_IDs_i";
    public const string CouponIds = "COUPON_IDs_i";
    public const string FileName = "IS_FILE_NAME_i";
    public const string FileLocation = "IS_FILE_LOCATION_i";
    public const string ATPCOMemberId = "ATPCO_MEMBER_ID_i";
    public const string VCFKey = "VCF_KEY_i";
    public const string ExpectResponseDateTime = "EXPECTED_RESPONSE_DATE_TIME_i";
    public const string UpdateInvoicesAndCouponsForRequestVCF = "UpdateInvoicesAndCouponsForRequestVCF";
#endregion
  }
}
