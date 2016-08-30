using System;
namespace Iata.IS.Data.ACHRecapSheet.Impl
{
  public class ACHRecapSheetRepositoryContants
  {
    #region ACHRecapSheetRepository Constants

    // Following constants are used for stored procedure PROC_GET_ACH_RECAP_SHEET.
    public const string CurrentBillingYear = "BILLING_YEAR_i";
    public const string CurrentBillingMonth = "BILLING_MONTH_i";
    public const string CurrentBillingPeriod = "BILLING_PERIOD_i";
    public const string GetACHRecapSheetData = "GetAchRecapSheetData";
    public const string ACHRecapSheetStatus = "ACH_RECAP_STATUS_i";
    public const string ACHRecapSheetRegenerateFlag = "ACH_RECAP_REGENERATE_FLAG_I";
    public const string UpdateInvoiceACHRecapSheetStatus = "UpdateInvoiceACHRecapSheetStatus";

    // Following constants are used for stored procedure PROC_UPDATE_ACH_RECAP_SHEET.
    //Note the Billing Yr, Mnth and Period parameters are declared above
    public const string FileName = "IS_FILE_NAME_i";
    public const string FileLocation = "IS_FILE_LOCATION_i";
    public const string ACHMemberId = "ACH_MEMBER_ID_i";
    public const string UpdateACHRecapSheetData = "UpdateACHRecapSheetData";
    public const string RevertACHRecapSheetData = "RevertACHRecapSheetData";

    #endregion
  }
}
