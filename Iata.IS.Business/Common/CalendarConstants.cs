namespace Iata.IS.Business.Common
{
  public static class CalendarConstants
  {
    public const string InputDateFormat = "yyMMddHHmm";
    public const string FromClearanceHouse = "From Clearance House";
    public const string CalendarMonth = "Calendar Month";
    public const string Period = "Period";
    public const string JobGroupName = "SIS";

    #region ACH Event Name Constants

    public const string RecapSheetSubmissionDeadline = "Recap Sheet Submission Deadline";
    public const string SettlementSheetPostingDateAchTransactions = "Settlement Sheet Posting Date (ACH transactions)";
    public const string ProtestDeadlineAchTransactions = "Protest Deadline (ACH transactions)";
    public const string SettlementDayAchTransactions = "Settlement Day (ACH transactions)";
    public const string SettlementSheetPostingDateIataTransactions = "Settlement Sheet Posting Date (IATA transactions)";
    public const string ProtestDeadlineIataTransactions = "Protest Deadline (IATA transactions)";
    public const string SettlementDayIataTransactions = "Settlement Day (IATA transactions)";

    #endregion

    #region ICH Event Name Constants

    public const string ClosureDay = "Closure Day";
    public const string AdviceDay = "Advice Day";
    public const string ProtestDeadline = "Protest Deadline";
    public const string EarlyCallDay = "Early Call Day";
    public const string CallDay = "Call Day";
    public const string SettlementDay = "Settlement Day";
    public const string SuspensionDay = "Suspension Day";

    #endregion

    #region Derived Event Name Constants

    public const string FutureDatedSubmissionsOpen = "Submissions Open (Future dated submissions)";
    public const string SubmissionsOpen = "Submissions Open";
    public const string SubmissionDeadlineForIchAndBilateralInvoices = "Submission Deadline for ICH and Bilateral Invoices";
    public const string ClosureOfLateSubmissionsIch = "Closure of Late Submissions (ICH Invoices)";
    public const string SubmissionDeadlineForAchInvoices = "Submission Deadline for ACH Invoices";
    public const string ClosureOfLateSubmissionsAch = "Closure of Late Submissions (ACH Invoices)";
    public const string SupportingDocumentsLinkingDeadline = "Supporting Documents Linking Deadline";
    public const string BillingOutputGeneration = "Billing Output Generation";
    public const string AutoBillingInvoiceFinalization = "Auto Billing Invoice Finalization";

    #endregion

    #region ACH Event Column Name Constants

    public const string RecapSheetSubmissionDeadlineColumn = "RECAP_SHEET_SUB_DEADLINE";
    public const string SettlementSheetPostingDateAchTransactionsColumn = "SETTLE_SHEET_POSTING_ACH";
    public const string ProtestDeadlineAchTransactionsColumn = "PROTEST_DEADLINE_ACH";
    public const string SettlementDayAchTransactionsColumn = "SETTLE_DAY_ACH";
    public const string SettlementSheetPostingDateIataTransactionsColumn = "SETTLE_SHEET_POSTING_IATA";
    public const string ProtestDeadlineIataTransactionsColumn = "PROTEST_DEADLINE_IATA";
    public const string SettlementDayIataTransactionsColumn = "SETTLE_DAY_IATA";

    #endregion

    #region ICH Event Column Name Constants

    public const string ClosureDayColumn = "CLOSURE_DAY";
    public const string AdviceDayColumn = "ADVICE_DAY";
    public const string ProtestDeadlineColumn = "PROTEST_DEADLINE";
    public const string EarlyCallDayColumn = "EARLY_CALL_DAY";
    public const string CallDayColumn = "CALL_DAY";
    public const string SettlementDayColumn = "SETTLEMENT_DAY";
    public const string SuspensionDayColumn = "SUSPENSION_DAY";

    #endregion

    #region Derived Event Column Name Constants

    public const string FutureDatedSubmissionsOpenColumn = "FUTURE_DATED_SUB_OPEN";
    public const string SubmissionsOpenColumn = "SUB_OPEN";
    public const string SubmissionDeadlineForIchAndBilateralInvoicesColumn = "SUB_DEADLINE_ICH_BILATERAL";
    public const string ClosureOfLateSubmissionsIchColumn = "LATE_SUB_CLOSURE_ICH";
    public const string SubmissionDeadlineForAchInvoicesColumn = "SUB_DEADLINE_ACH";
    public const string ClosureOfLateSubmissionsAchColumn = "LATE_SUB_CLOSURE_ACH";
    public const string SupportingDocumentsLinkingDeadlineColumn = "SUPPORT_DOCS_LINK_DEADLINE";
    public const string BillingOutputGenerationColumn = "BILLING_OUTPUT_GEN";
    public const string AutoBillingInvoiceFinalizationColumn = "AUTO_BILL_INV_FINALIZATION";

    #endregion

    #region Trigger Constants

    public const string ACHMissingSubmissionsAlert1Trigger = "ACHMissingSubmissionsAlert1Trigger";
    public const string ACHMissingSubmissionsAlert2Trigger = "ACHMissingSubmissionsAlert2Trigger";
    public const string CreateAndTransmitProcessedInvoiceDataCsvTrigger = "CreateAndTransmitProcessedInvoiceDataCsvTrigger";
    public const string FinalizeInvoiceStatusToProcessingCompleteTrigger = "FinalizeInvoiceStatusToProcessingCompleteTrigger";
    public const string GenerateSISRechargeDataTrigger = "IATARechargeDataGeneratorTrigger";
    public const string OutputFileGenerationTrigger = "OutputFileGenerationTrigger";
    public const string InvoiceOfflineCollectionGenerationTrigger = "InvoiceOfflineCollectionGenerationTrigger";
    public const string GenerateNilFormCTrigger = "GenerateNilFormCTrigger";
    public const string SupportingDocFinalizationTrigger = "SupportingDocFinalizationTrigger";

    public const string SettlementWithAchTrigger = "SettlementWithAchTrigger";
    public const string SubmissionDeadlineAlertsForUnclosedIchInvoicesTrigger = "SubmissionDeadlineAlertsForUnclosedIchInvoicesTrigger";
    public const string PendingAttachmentAlertForPaxFutureSubmissionsIchInvoicesTrigger = "PendingAttachmentAlertForPaxFutureSubmissionsIchInvoicesTrigger";
    public const string GenerationAndDeliveryOfLegalInvoiceDataForIataTrigger = "GenerationAndDeliveryOfLegalInvoiceDataForIataTrigger";
    public const string DowngradeOnceMonthlyToOldidecAndSendToAtpTrigger = "DowngradeOnceMonthlyToOldidecAndSendToAtpTrigger";
    public const string MissingSubmissionDeadlineNotificationToMembersTrigger = "MissingSubmissionDeadlineNotificationToMembersTrigger";
    public const string AutoOpeningOfICHLateSubmissionWindowTrigger = "AutoOpeningOfICHLateSubmissionWindowTrigger";
    public const string AutoOpeningOfACHLateSubmissionWindowTrigger = "AutoOpeningOfACHLateSubmissionWindowTrigger";
    public const string AutoClosingOfICHLateSubmissionWindowTrigger = "AutoClosingOfICHLateSubmissionWindowTrigger";
    public const string AutoClosingOfACHLateSubmissionWindowTrigger = "AutoClosingOfACHLateSubmissionWindowTrigger";
    public const string ValueConfirmationTrigger = "ValueConfirmationTrigger";

    #endregion
  }
}

//public const string FinalizationOfSupportingDocsForAllInvoicesTrigger = "FinalizationOfSupportingDocsForAllInvoicesTrigger";