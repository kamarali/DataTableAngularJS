using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class InterlineBillingSummaryDashboardController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    public InterlineBillingSummaryDashboardController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }

    public ActionResult InterlineBillingSummaryDashBoardReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["MemberId"] = (int)SessionUtil.MemberId;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PeriodNo"] = currentPeriod.Period;
      return View();
    }
  }
}
