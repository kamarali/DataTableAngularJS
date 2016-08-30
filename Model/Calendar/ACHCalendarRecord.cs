using FileHelpers;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Calendar
{
  [DelimitedRecord(",")]
  public class AchCalendarRecord : CalendarRecordBase
  {
    [FieldOrder(4), EventMap(CalendarEvents.RecapSheetSubmissionDeadline)]
    public string RecapSheetSubmissionDeadline { get; set; }

    [FieldOrder(5), EventMap(CalendarEvents.SettlementSheetPostingDateAchtransactions)]
    public string SettlementSheetPostingDateAchtransactions { get; set; }

    [FieldOrder(6), EventMap(CalendarEvents.ProtestDeadlineAchtransactions)]
    public string ProtestDeadlineAchtransactions { get; set; }

    [FieldOrder(7), EventMap(CalendarEvents.SettlementDayAchtransactions)]
    public string SettlementDayAchtransactions { get; set; }

    [FieldOrder(8), EventMap(CalendarEvents.SettlementSheetPostingDateIatatransactions)]
    public string SettlementSheetPostingDateIatatransactions { get; set; }

    [FieldOrder(9), EventMap(CalendarEvents.ProtestDeadlineIatatransactions)]
    public string ProtestDeadlineIatatransactions { get; set; }

    [FieldOrder(10), EventMap(CalendarEvents.SettlementDayIatatransactions)]
    public string SettlementDayIatatransactions { get; set; }
  }
}