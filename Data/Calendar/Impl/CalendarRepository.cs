using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.Calendar.Impl
{
  public class CalendarRepository : Repository<ISCalendar>, ICalendarRepository
  {
    /// <summary>
    /// Searches the calendar data.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <returns></returns>
    public IList<CalendarSearchResult> SearchCalendarData(int year, int month, int period)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter("CALENDAR_YEAR_I", typeof(int)) { Value = year };
      parameters[1] = new ObjectParameter("CALENDAR_MONTH_I", typeof(int)) { Value = month };
      parameters[2] = new ObjectParameter("CALENDAR_PERIOD_I", typeof(int)) { Value = period };

      var calendarSearchResults = ExecuteStoredFunction<CalendarSearchResult>("GetISCalendarEvents", parameters);

      return calendarSearchResults.ToList();

    }

    /// <summary>
    /// Gets the period events.
    /// </summary>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="isDateOrPeriod">Date or period flag. if 0 - inputDate contains date otherwise contains period information.</param>
    /// <param name="clearingHouse">The clearing house.</param>
    /// <param name="forPeriodOrLateSubWindow">For period or late sub window.
    /// If value is 0 then return start and end events for billing period 
    /// otherwise return start and end events for late submission window.</param>
    /// <returns></returns>
    public IList<ISCalendar> GetPeriodEvents(DateTime billingPeriod, int isDateOrPeriod, string clearingHouse, int forPeriodOrLateSubWindow)
    {
      var parameters = new ObjectParameter[4];
      parameters[0] = new ObjectParameter("BILLING_PERIOD", typeof(DateTime)) { Value = billingPeriod };
      parameters[1] = new ObjectParameter("IS_DATE_OR_PERIOD", typeof(string)) { Value = isDateOrPeriod };
      parameters[2] = new ObjectParameter("CLEARING_HOUSE_FLAG", typeof(string)) { Value = clearingHouse };
      parameters[3] = new ObjectParameter("IS_PERIOD_OR_LATE_SUB_WINDOW", typeof(string)) { Value = forPeriodOrLateSubWindow };


      var periodEvents = ExecuteStoredFunction<ISCalendar>("GetPeriodEvents", parameters);

      return periodEvents.ToList();
    }
  }
}
