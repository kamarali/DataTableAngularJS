using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Calendar
{
  public class DerivedEventOffsetInfo : EntityBase<int>
  {
    public Int32 EventDerivedFromId { get; set; }
    public string EventType { get; set; }
    public Int32 OffsetPeriods { get; set; }
    public Int32 OffsetMonths { get; set; }
    public Int32 OffsetDays { get; set; }
    public Int32 OffsetHours { get; set; }
    public Int32 OffsetMinutes { get; set; }
    public Int32 OffsetSeconds { get; set; }

    public CalendarEvent CalendarEvent { get; set; }

  }
}