using FileHelpers;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Calendar
{
  [DelimitedRecord(",")]
  public class IchCalendarRecord : CalendarRecordBase
  {
    [FieldOrder(4),EventMap(CalendarEvents.ClosureDay)]
    public string ClosureDay { get; set; }

    [FieldOrder(5),EventMap(CalendarEvents.AdviceDay)]
    public string AdviceDay { get; set; }

    [FieldOrder(6), EventMap(CalendarEvents.ProtestDeadline)]
    public string ProtestDeadline { get; set; }

    [FieldOrder(7), EventMap(CalendarEvents.EarlyCallDay)]
    public string EarlyCallDay { get; set; }

    [FieldOrder(8), EventMap(CalendarEvents.CallDay)]
    public string CallDay { get; set; }

    [FieldOrder(9), EventMap(CalendarEvents.SettlementDay)]
    public string SettlementDay { get; set; }

    [FieldOrder(10), EventMap(CalendarEvents.SuspensionDay)]
    public string SuspensionDay { get; set; }
  }
}

