namespace Iata.IS.Model.Enums
{
  /// <summary>
  /// Represents the different calendar events.
  /// </summary>
  public enum CalendarEvents
  {
    ClosureDay = 1,
    AdviceDay = 2,
    ProtestDeadline = 3,
    EarlyCallDay = 4,
    CallDay = 5,
    SettlementDay = 6,
    SuspensionDay = 7,

    RecapSheetSubmissionDeadline = 8,
    SettlementSheetPostingDateAchtransactions = 9,
    ProtestDeadlineAchtransactions = 10,
    SettlementDayAchtransactions = 11,
    SettlementSheetPostingDateIatatransactions = 12,
    ProtestDeadlineIatatransactions = 13,
    SettlementDayIatatransactions = 14,

    SubmissionsOpen = 15,
    SubmissionsOpenFuturedatedsubmissions = 16,
    SubmissionDeadlineforIchandBilateralInvoices = 17,
    SubmissionDeadlineforAchInvoices = 18,
    ClosureofLateSubmissionsIchInvoices = 19,
    ClosureofLateSubmissionsAchInvoices = 20,
    SupportingDocumentsLinkingDeadline = 21,
    BillingOutputGeneration = 22
  }
}