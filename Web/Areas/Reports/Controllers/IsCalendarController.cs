using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Model.Calendar;
using System.Collections.ObjectModel;
using Iata.IS.Model.Reports;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class IsCalendarController : ISController
    {
        //
        // GET: /Reports/IsCalendar/
        [ISAuthorize(Business.Security.Permissions.Reports.IsChCalendar.Access)]
        public ActionResult IsCalendar()
        {
            ReadOnlyCollection<TimeZoneInfo> tzCollection;
            tzCollection = TimeZoneInfo.GetSystemTimeZones();
            Dictionary<string, string> timeZones = tzCollection.ToDictionary(t => t.Id, t => t.DisplayName);
            ViewData["TimeZones"] = new SelectList(timeZones, "Key", "Value", "Eastern Standard Time");
            return View();
        }

    }
}
