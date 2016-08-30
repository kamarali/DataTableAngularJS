using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Web.Util.BillingType;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class PaxCgoMscTopTenPartnerController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    public PaxCgoMscTopTenPartnerController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }
    //
    // GET: /Reports/PaxCgoMscTopTenPartner/
    [ISAuthorize(Business.Security.Permissions.Reports.FinancialController.Top10ReceivablesAccess)]
    public ActionResult PaxCgoMscTopTenPartnerRec()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["IsPayableReport"] = 0;
      ViewData["ReportPageName"] = "Receivable-Top 10 Interline Partner";
      ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
      return View("PaxCgoMscTopTenPartner");
    }

    [ISAuthorize(Business.Security.Permissions.Reports.FinancialController.Top10PayablesAccess)]
    public ActionResult PaxCgoMscTopTenPartnerPay()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["IsPayableReport"] = 1;
      ViewData["ReportPageName"] = "Payables-Top 10 Interline Partner";

      return View("PaxCgoMscTopTenPartner");
    }
  }
}
