using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Model.Enums.BillingType;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class ReceivableCargoSubmissionOverviewController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    public ReceivableCargoSubmissionOverviewController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }
    // GET: /Reports/CargoSubmissionOverview/

    //Action method for Receivables
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.ReceivablesSubmissionOverviewAccess)]
    public ActionResult Index()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["currentPeriod"] = currentPeriod.Period;

      ViewData["BillingType"] = BillingType.Receivables;
      return View("ReceivableCargoSubmissionOverview");
    }

    //Action method for Payables
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.PayablesSubmissionOverviewAccess)]
    public ActionResult PayableCargoSubmissionOverview()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["currentPeriod"] = currentPeriod.Period;

      ViewData["BillingType"] = BillingType.Payables;
      return View("ReceivableCargoSubmissionOverview");
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.PayablesSubmissionOverviewAccess)]
    [HttpPost]
    public ActionResult PayableCargoSubmissionOverview(string formid)
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["currentPeriod"] = currentPeriod.Period;

      ViewData["BillingType"] = BillingType.Payables;
      return View("ReceivableCargoSubmissionOverview");
    }
  }
}
