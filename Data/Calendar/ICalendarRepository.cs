using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.Calendar
{
  public interface ICalendarRepository : IRepository<ISCalendar>
  {
    /// <summary>
    /// Searches the calendar data.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="period">The period.</param>
    /// <returns></returns>
    IList<CalendarSearchResult> SearchCalendarData(int year, int month, int period);

    /// <summary>
    /// Return the events corresponding to the start and end date of billing period for given date and clearing house.
    /// </summary>
    /// <param name="inputDate">input date for which events need to fetch.</param>
    /// <param name="isDateOrPeriod">Date or period flag. if 0 - inputDate contains date otherwise contains period information.</param>
    /// <param name="clearingHouse">clearing house</param>
    /// <param name="forPeriodOrLateSubWindow">For period or late sub window.
    /// If value is 0 then return start and end events for billing period 
    /// otherwise return start and end events for late submission window.</param>
    /// <returns>
    /// list of calendar events.
    /// </returns>
    IList<ISCalendar> GetPeriodEvents(DateTime inputDate, int isDateOrPeriod, string clearingHouse, int forPeriodOrLateSubWindow);


  }
}
