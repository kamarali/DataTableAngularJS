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
  public class CargoInterlineBillingSummaryController : ISController
  {

    private readonly ICalendarManager _calenderManager;
    public CargoInterlineBillingSummaryController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }

    //
    // GET: /Reports/CargoInterlineBillingSummary/
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.ReceivablesInterlineBillSummaryAccess)]
    public ActionResult ReceivablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      ViewData["MemberId"] = (int)SessionUtil.MemberId;
      ViewData["BillingType"] = BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)BillingType.Receivables;
      ViewData["MemberType"] = "Billed";
      ViewData["BillingTypeWords"] = "Receivables";
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PeriodNo"] = currentPeriod.Period;
      return View("CargoInterlineBillingSummaryReport");
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.PayablesInterlineBillSummaryAccess)]
    public ActionResult PayablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//etCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      ViewData["BillingType"] = BillingType.Payables;
      ViewData["BillingTypeId"] = (int)BillingType.Payables;
      ViewData["MemberType"] = "Billing";
      ViewData["MemberId"] = (int)SessionUtil.MemberId;
      ViewData["BillingTypeWords"] = "Payables";
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PeriodNo"] = currentPeriod.Period;

      return View("CargoInterlineBillingSummaryReport");
    }
  }
}
