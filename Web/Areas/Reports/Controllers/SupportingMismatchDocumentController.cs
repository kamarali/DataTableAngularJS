using System;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using BillingType = Iata.IS.Web.Util.BillingType;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class SupportingMismatchDocumentController : ISController
  {
    private readonly ICalendarManager _calendarManager;

    public SupportingMismatchDocumentController(ICalendarManager calendarManager)
    {
      _calendarManager = calendarManager;
    }
    //
    // GET: /Reports/SupportingMismatchDocument/

    /// <summary>
    /// This action is executed to call MismatchDocument page
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesSupportingDocMismatchAccess)]
    public ActionResult MismatchDocument()
    {

      IsMemberNullOrEmpty(SessionUtil.MemberId);
      ViewData["Category"] = (int)BillingCategoryType.Pax;
      ViewData["CategoryName"] = BillingCategoryType.Pax;
      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      ViewData["CurrentMonth"] = currentPeriod.Month;
      ViewData["CurrentYear"] = currentPeriod.Year;
      ViewData["CurrentPeriod"] = currentPeriod.Period;
     
      return View();
    }// End MismatchDocument

    /// <summary>
    /// CMP# 519 Miscellaneous Supp Doc Mismatch Report
    /// This action is executed to call Miscellaneous MismatchDocument page
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Misc.SupportingDocMismatchAccess)]
    public ActionResult MiscMismatchDocument()
    {

      IsMemberNullOrEmpty(SessionUtil.MemberId);
      ViewData["Category"] = (int)BillingCategoryType.Misc;
      ViewData["CategoryName"] = BillingCategoryType.Misc;
      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      ViewData["CurrentMonth"] = currentPeriod.Month;
      ViewData["CurrentYear"] = currentPeriod.Year;
      ViewData["CurrentPeriod"] = currentPeriod.Period;

      return View();
    }// End MismatchDocument for Misc

    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.ReceivablesSupportingDocMismatchAccess)]
    public ActionResult CgoMismatchDocument()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      ViewData["Category"] = (int)BillingCategoryType.Cgo;
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CurrentMonth"] = currentPeriod.Month;
      ViewData["CurrentYear"] = currentPeriod.Year;
      ViewData["CurrentPeriod"] = currentPeriod.Period;
      ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

      return View();
    }// End MismatchDocument for Cargo
  }// End SupportingMismatchDocumentController class
}// End namespace
