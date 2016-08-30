using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.Calendar.Impl
{
  public class CalendarProcessRepository : Repository<CalendarProcess>, ICalendarProcessRepository
  {
    public IQueryable<CalendarProcess> GetAllCalendarProcesses()
    {
      var calendarProcessList = EntityObjectSet.Include("CalendarEvent");

      return calendarProcessList;
    }
  }
}