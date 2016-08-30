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
  public class RejectionAnalysisRecController : ISController
  {

    private readonly ICalendarManager _calenderManager;
    public RejectionAnalysisRecController(ICalendarManager calenderManager)
    {
      _calenderManager = calenderManager;
    }
    //
    // GET: /Reports/RejectionAnalysisRec/
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.ReceivablesRejnAnalysisAccess)]
    public ActionResult CgoRejectionAnalysisRec()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["IsPayableReport"] = 0;
      ViewData["MemberCodeField"] = "Billed Member Code";
      ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
      return View("CgoRejectionAnalysisRec");
    }
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.PayablesRejnAnalysisAccess)]
    public ActionResult CgoRejectionAnalysisPay()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["IsPayableReport"] = 1;
      ViewData["MemberCodeField"] = "Billing Member Code";
      return View("CgoRejectionAnalysisRec");
    }
  }
}
