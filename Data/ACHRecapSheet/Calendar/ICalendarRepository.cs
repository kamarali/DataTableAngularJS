using System.Collections.Generic;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.Calendar
{
  public interface ICalendarRepository : IRepository<IsCalendar>
  {
    List<IsCalendar> GetEventList(string eventType);

  }
}
