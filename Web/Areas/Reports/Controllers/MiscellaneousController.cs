using System;
using System.Web.Mvc;
using Iata.IS.Web.Util;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class MiscellaneousController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    private readonly IChargeCodeManager _ChargeCodeManager = null;

    public MiscellaneousController(ICalendarManager calenderManager, IChargeCodeManager chargeCodeManager)
    {
      _calenderManager = calenderManager;
      _ChargeCodeManager = chargeCodeManager;
    }

    //
    // GET: /Reports/Miscellaneous/
    /// <summary>
    /// Author: Sanket Shrivastava
    /// Purpose: MISCELLANEOUS INVOICE SUMMARY REPORT - Receivables
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Misc.RecInvoiceSummaryAccess)]
    public ActionResult MiscChargeSummary()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      var previousBillingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Misc;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PerviousYear"] = previousBillingPeriod.Year;
      ViewData["PerviousMonth"] = previousBillingPeriod.Month;
      ViewData["PerviousPeriod"] = previousBillingPeriod.Period;
      ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;
      ViewData["BillingType"] = Iata.IS.Model.Enums.BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)Iata.IS.Model.Enums.BillingType.Receivables;
      ViewData["BillingTypeText"] = "Receivables";
      ViewData["MemberType"] = "Billed";
      return View();
    }


    /// <summary>
    /// Author: Sanket Shrivastava
    /// Purpose: MISCELLANEOUS CHARGE SUMMARY REPORT - Payables
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Misc.PayChargeSummaryAccess)]
    public ActionResult MiscChargePaySummary()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var previousBillingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Misc;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PerviousYear"] = previousBillingPeriod.Year;
      ViewData["PerviousMonth"] = previousBillingPeriod.Month;
      ViewData["PerviousPeriod"] = previousBillingPeriod.Period;
      ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;
      ViewData["BillingType"] = Iata.IS.Model.Enums.BillingType.Payables;
      ViewData["BillingTypeId"] = (int)Iata.IS.Model.Enums.BillingType.Payables;
      ViewData["BillingTypeText"] = "Payables";
      ViewData["MemberType"] = "Billing";
      return View("~/Areas/Reports/Views/Miscellaneous/MiscChargeSummary.aspx");
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Purpose: Miscellaneous Substitution Values Report
    /// Date Of Creation: 29-11-2011
    /// </summary>
    /// <returns></returns>
    public ActionResult MiscSubstitutionValuesReport()
    {
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Misc;
      ViewData["MemberId"] = (int)SessionUtil.MemberId;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PeriodNo"] = currentPeriod.Period;
      return View();
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Purpose: To Get Charge Code List for Given Charge Category
    /// 01-12-2011
    /// </summary>
    /// <param name="chargeCategoryId"></param>
    /// <returns></returns>
    public JsonResult GetChargeCodeList(string chargeCategoryId)
    {
      var chargeCodes = _ChargeCodeManager.GetChargeCodeList("", Convert.ToInt32(chargeCategoryId));
      var result = new JsonResult();
      result.Data = chargeCodes;
      result.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return result;
    }
  }
}
