using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class ValidationDetailsController : Controller
    {
        private readonly ICalendarManager _calendarManager;

        public ValidationDetailsController(ICalendarManager calenderManager)
        {
            _calendarManager = calenderManager;
        }
        //
        // GET: /Reports/ValidationDetails/
        public ActionResult IsValidationDetail()
        {
            ViewData["ValidationCategory"] = BillingCategoryType.Pax;
            DateTime dt = DateTime.UtcNow;
            ViewData["CurrentYear"] = dt.Year;
            ViewData["CurrentMonth"] = dt.Month;
            ViewData["Period"] = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod().Period;
            return View();
        }

        public ActionResult MiscellaneousDetails()
        {
            ViewData["ValidationCategory"] = BillingCategoryType.Misc;
            DateTime dt = DateTime.UtcNow;
            ViewData["CurrentYear"] = dt.Year;
            ViewData["CurrentMonth"] = dt.Month;
            ViewData["Period"] = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod().Period;
            return View("IsValidationDetail");
        }

    }
}
