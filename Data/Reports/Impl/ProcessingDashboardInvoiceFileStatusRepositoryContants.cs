using System;
namespace Iata.IS.Data.Reports.Impl
{
  public class ProcessingDashboardInvoiceFileStatusRepositoryContants
  {
    #region ProcessingDashboardInvoiceStatusRepository Constants

    public const string BillingYear = "BILLING_YEAR";
    public const string BillingMonth = "BILLING_MONTH";
    public const string BillingPeriod = "BILLING_PERIOD";
    public const string BilledMemberId = "BILLED_MEMBER_ID";
    public const string BillingMemberId = "BILLING_MEMBER_ID";
    public const string SettlementMethodId = "SETTLEMENT_METHOD_ID";
    public const string BillingCategoryId = "BILLING_CATEGORY_ID";
    public const string InvoiceStatusId = "INVOICE_STATUS_ID";
    public const string IsUserId = "IS_USER_ID";
    public const string IsFileName = "IS_FILE_NAME";
    public const string IsInvoiceNo = "INVOICE_NO";
    public const string GetInvoiceStatusSearchResult = "ProcessingDashboardSearchInvoiceStatus";
    public const string InvoiceId = "INVOICE_ID_i";
    public const string GetInvoiceDetailsResult = "InvoiceDetailsProcessingDashboard";
    public const string GetInvoiceForCsvDownload = "ProcessingDashboardInvoiceCsvDownload";
    public const string UniqueInvoiceNo = "UNIQUE_INVOICE_NO";
    public const string IsShowClaimFailed = "IS_SHOW_CLAIM_FAILED";
    //CMP559 : Add Submission Method Column to Processing Dashboard
    public const string SubmissionMethodId = "SUBMISSION_METHOD_ID";
    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public const string DailyDeliveryStatusId = "DAILY_DELIVERY_STATUS";

    // Following constants are used for stored procedure which retrieves Invoice File list
    public const string GetFileStatusSearchResult = "ProcessingDashboardSearchFileStatus";
    public const string FileBillingYear = "BILLING_YEAR_i";
    public const string FileBillingMonth = "BILLING_MONTH_i";
    public const string FileBillingPeriod = "BILLING_PERIOD_i";
    public const string FileBillingMemberId = "BILLING_MEMBER_ID_i";
    public const string FileBillingCategoryId = "BILLING_CATEGORY_ID_i";
    public const string FileFormatId = "FILE_FORMAT_ID_i";
    public const string FileStatusId = "FILE_STATUS_ID_i";
    public const string FileName = "FILE_NAME_i";
    public const string UserId = "IS_USER_ID_i";
    public const string IsIsWebInvoice = "IS_ISWEB_INV_i";
    public const string FileGeneratedDateFrom = "FILE_RECEIVED_SENT_ON_FROM";
    public const string FileGeneratedDateTo = "FILE_RECEIVED_SENT_ON_TO";

    // Following constants are used for stored procedure which Increments Billing period for Invoices within
    // a File and stored procedure which marks Invoices within a file for LateSubmission.
    public const string FileIds = "FILE_IDs_i";
    public const string MemberId = "MEMBER_ID_i";
    public const string CurrentBillingYear = "CURR_BILLING_YEAR_i";
    public const string CurrentBillingMonth = "CURR_BILLING_MONTH_i";
    public const string CurrentBillingPeriod = "CURR_BILLING_PERIOD_i";
    public const string IsIchLateAcceptanceAllowed = "ICH_LATE_WINDOW_OPEN";
    public const string IsAchLateAcceptanceAllowed = "ACH_LATE_WINDOW_OPEN";
    public const string IsAchManualControl = "IS_ACH_MANUAL_CTRL";
    public const string IsIchManualControl = "IS_ICH_MANUAL_CTRL";
    public const string IncrementBillingPeriodForInvoicesWithinFile = "IncrementBillingPeriodForInvoicesWithinFile";
    public const string MarkInvoicesForLateSubmissionWithinFile = "MarkInvoicesForLateSubmissionWithinFile";
    public const string DeleteFiles = "DeleteFilesProcessingDashboard";
    public const string FileUserId = "LAST_UPDATED_BY_i";
    
    //used for file invoice warning 
    public const string FileId = "FILE_ID";
    public const string GetFileInvErrorWarning = "FileInvoiceErrorProcessingDashboard";

    public const string InvoiceIds = "INVOICE_IDS_I";
    public const string DeleteInvoices = "DeleteInvoicesProcessingDashboard";

    #endregion

    // Procedure to set purging expiry period of transactions.
    public const string SetPurgingExpiryPeriod = "SetPurgingExpiryPeriod";

      /* CMP #675: Progress Status Bar for Processing of Billing Data Files. Constants to execute SP - PROC_GET_FILE_PROGRESS_DETAILS */
    public const string GetFileProgressDetailsSpName = "GetFileProgressDetails";
    public const string FileLogId = "IS_FILE_LOG_ID_I";
    public const string ProcessName = "PROCESS_NAME_O";
    public const string ProcessState = "PROCESS_STATE_O";
    public const string QueuePosition = "QUEUE_POSITION_O";
  }
}
