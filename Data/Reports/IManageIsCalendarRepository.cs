using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports;
using Iata.IS.Model.Calendar;
namespace Iata.IS.Data.Reports
{
    public interface IManageIsCalendarRepository 
    {
        List<IsClearingHouseCalendar> GetIsClearingHouseCalendar(int clearanceYear, int eventType, string timeZone);

    }
}
