using System.Linq;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.Calendar
{
  public interface ICalendarProcessRepository : IRepository<CalendarProcess>
  {
    IQueryable<CalendarProcess> GetAllCalendarProcesses();
  }
}