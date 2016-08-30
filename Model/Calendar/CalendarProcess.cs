using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.Calendar
{
  public class CalendarProcess : EntityBase<int>
  {
    public String ProcessName { get; set; }
    public string ProcessType { get; set; }
    public Int32 EventId { get; set; }
    public Int32 PeriodNo { get; set; }
    public string TriggerType { get; set; }
    public Int32 OffsetMonths { get; set; }
    public Int32 OffsetDays { get; set; }
    public Int32 OffsetHours { get; set; }
    public Int32 OffsetMinutes { get; set; }
    public Int32 OffsetSeconds { get; set; }
    public Int32 ThresholdTime { get; set; }
    public DateTime LastStartTime { get; set; }
    public DateTime LastEndTime { get; set; }
    public string RunningStatus { get; set; }
    public string ProcessStatus { get; set; }
    public Int32 DependentProcessId { get; set; }
    public Int32 MaxAttempt { get; set; }
    public Int32 Frequency { get; set; }

    public CalendarEvent CalendarEvent { get; set; }

  }
}