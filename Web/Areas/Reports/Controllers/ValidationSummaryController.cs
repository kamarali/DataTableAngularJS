using System;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class ValidationSummaryController : ISController
    {

          private readonly ICalendarManager _calendarManager;

          public ValidationSummaryController(ICalendarManager calenderManager)
        {
            _calendarManager = calenderManager;
        }

        public ActionResult IsValidationSummary()
        {
            ViewData["ValidationCategory"] = "Passenger";
            DateTime dt = DateTime.UtcNow;
            ViewData["CurrentYear"] = dt.Year;
            ViewData["CurrentMonth"] = dt.Month;
            ViewData["Period"] = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod().Period;
            return View();
        }

        public ActionResult MiscellaneousSummary()
        {
            ViewData["ValidationCategory"] = "Miscellaneous";
            DateTime dt = DateTime.UtcNow;
            ViewData["CurrentYear"] = dt.Year;
            ViewData["CurrentMonth"] = dt.Month;
            ViewData["Period"] = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod().Period;
            return View("IsValidationSummary");
        }
    }
}
