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
  public class InterlinePayablesAnalysisController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    public InterlinePayablesAnalysisController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }
    //
    // GET: /Reports/InterlinePayablesAnalysis/
    [ISAuthorize(Business.Security.Permissions.Menu.RepFinCtrlInterlinePayablesAnalysis)]
    public ActionResult InterlinePayablesAnalysis()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var lastClosedPeriod = _calenderManager.GetLastClosedBillingPeriod();//GetCurrentBillingPeriod();
      ViewData["LastClosedYear"] = lastClosedPeriod.Year;
      ViewData["LastClosedMonth"] = lastClosedPeriod.Month;
      ViewData["LastClosedPeriod"] = lastClosedPeriod.Period;
      ViewData["MemberCodeField"] = "Billing Member";
      return View("InterlinePayablesAnalysis");
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 21-10-2011
    /// Purpose: Action Result Method For Interline Billing summary Report
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Menu.RepFinCtrlInterlineBillingsummary)]
    public ActionResult InterlineBillingSummaryReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var previosPeriod = _calenderManager.GetLastClosedBillingPeriod();//GetCurrentBillingPeriod();
      ViewData["PreviousClosedYear"] = previosPeriod.Year;
      ViewData["PreviousClosedMonth"] = previosPeriod.Month;
      ViewData["PreviousClosedPeriod"] = previosPeriod.Period;
      return View();
    }
  }
}
