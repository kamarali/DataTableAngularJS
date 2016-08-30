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
  public class PendingInvoicesController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    //
    // GET: /Reports/PendingInvoices/
    public PendingInvoicesController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }


    [ISAuthorize(Business.Security.Permissions.Reports.FinancialController.PendingErrorInvoicesAccess)]
    public ActionResult PendingInvoicesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["MemberId"] = (int)SessionUtil.MemberId;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["currentPeriodNo"] = currentPeriod.Period;
      return View();
    }
  }
}
