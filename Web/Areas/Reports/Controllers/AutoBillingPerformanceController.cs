using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class AutoBillingPerformanceController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    public AutoBillingPerformanceController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }

    // GET: /Reports/AutoBillingPerformance/
    //[ISAuthorize(Business.Security.Permissions.Reports.FinancialController.Top10ReceivablesAccess)]
    public ActionResult AutoBillingPerformance()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      //ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["IsPayableReport"] = 0;
      ViewData["ReportPageName"] = "Auto-Billing Performance Report";
      ViewData[ViewDataConstants.BillingType] = Iata.IS.Model.Enums.BillingType.Receivables;
      return View("AutoBillingPerformance");
    }
  }
}
