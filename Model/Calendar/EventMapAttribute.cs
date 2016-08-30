using System;
using Iata.IS.Model.Enums;

namespace Iata.IS.Model.Calendar
{
  public class EventMapAttribute : Attribute
  {
    public readonly CalendarEvents CalendarEvents;

    public EventMapAttribute(CalendarEvents calendarEvents)
    {
      CalendarEvents = calendarEvents;
    }
  }
}