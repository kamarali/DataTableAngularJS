using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class RMBMCMDetailsController : ISController
  {

    private readonly ICalendarManager _calendarManager;

    public RMBMCMDetailsController(ICalendarManager calendarManager)
    {
      _calendarManager = calendarManager;
    }

    //
    // GET: /Reports/RMBMCMDetails/
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.RMBMCMDetailsAccess)]
    public ActionResult Index()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CurrentMonth"] = currentPeriod.Month;
      ViewData["CurrentYear"] = currentPeriod.Year;
      ViewData["CurrentPeriod"] = currentPeriod.Period;
      return View();
    }
  }
}
