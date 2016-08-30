using System.Collections.Generic;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.Calendar.Impl
{
  public class CalendarRepository : Repository<IsCalendar>, ICalendarRepository
  {
    public List<IsCalendar> GetEventList(string eventType)
    {
      return new List<IsCalendar>();
    }
  }
}
