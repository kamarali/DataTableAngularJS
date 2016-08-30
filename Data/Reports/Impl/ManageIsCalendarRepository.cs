using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports;
using Iata.IS.Model.Calendar;
namespace Iata.IS.Data.Reports.Impl
{
    public class ManageIsCalendarRepository : Repository<ISCalendar>, IManageIsCalendarRepository
    {
        /// <summary>
        /// Gets the IS clearing house calendar.
        /// </summary>
        /// <param name="clearanceYear">The clearance year.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="timeZone">The time zone.</param>
        /// <returns></returns>
        public List<IsClearingHouseCalendar> GetIsClearingHouseCalendar(int clearanceYear, int eventType, string timeZone)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(ManageIsCalendarRepositoryConstants.ClearanceYear, typeof(Int32)) { Value = clearanceYear };
            parameters[1] = new ObjectParameter(ManageIsCalendarRepositoryConstants.CalandarEventType, typeof(Int32)) { Value = eventType };
            parameters[2] = new ObjectParameter(ManageIsCalendarRepositoryConstants.TimeZone, typeof(String)) { Value = "UTC" };
            //timezones
            TimeZoneInfo utctimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
            TimeZoneInfo selectedtimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var list = ExecuteStoredFunction<IsClearingHouseCalendar>(ManageIsCalendarRepositoryConstants.GetIsCalendarDetails, parameters);
            var convertedCalendarventDateList = (from c in list
                                                 select new IsClearingHouseCalendar
                                                 {
                                                     ClearanceMonth = c.ClearanceMonth,
                                                     CalendarMonth=c.CalendarMonth,
                                                     EventCategory = c.EventCategory,
                                                     EventDescription = c.EventDescription,
                                                     Period = c.Period,
                                                     EventDescriptionOrder = c.EventDescriptionOrder,
                                                     EventDateTime = TimeZoneInfo.ConvertTime(c.EventDateTime, utctimeZone, selectedtimeZone)
                                                 }).ToList();
            return convertedCalendarventDateList.ToList();
        }
    }
}
