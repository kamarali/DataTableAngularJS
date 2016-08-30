using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.LateSubmission;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.LateSubmission;
using Iata.IS.Web.Util;
using iPayables.UserManagement;
using log4net;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.LateSubmission.Controllers
{
  public class LateSubmissionController : ISController
  {
    private readonly IProcessLateSubmissionManager _lateSubmissionManager;
    private readonly ICalendarManager _calenderManager;
    private readonly I_ISUser _isUser;
    private readonly IReferenceManager _referenceManager;
    // Logger instance.
    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public LateSubmissionController(IProcessLateSubmissionManager lateSubmissionManager, ICalendarManager calenderManager, I_ISUser isUser, IReferenceManager referenceManager)
    {
      _lateSubmissionManager = lateSubmissionManager;
      _calenderManager = calenderManager;
      _isUser = isUser;
      _referenceManager = referenceManager;
    }

      [ISAuthorize(Business.Security.Permissions.IchOps.ManageLateSubmissionAccess)]
    public ActionResult IchLateSubmission()
    {
      ViewData["header"] = "Late Submission Invoices Pending ICH Acceptance";
      ViewData["lateSubStatus"] = _calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ich, _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich)); //ConfigurationManager.AppSettings["ICHLateAcceptanceAllowed"];
      ViewData["userCategory"] = (int)UserCategory.IchOps;
      
      /*Master Grid Display*/
      var gridModel = new LateSubmissionHeaderGrid("LateSubmissionGrid", Url.Action("LateSubmissionGridData", "LateSubmission", new { userCategory = (int)UserCategory.IchOps }));
      ViewData["LateSubmissionGrid"] = gridModel.Instance;

      /*Detail Grid Display*/
      var gridModelDetail = new LateSubmissionInvoiceDetail("LateSubmissionInvoiceDetailsGrid", Url.Action("LateSubmissionInvoiceDetailsGridData", "LateSubmission"));
      ViewData["LateSubmissionInvoiceDetailsGrid"] = gridModelDetail.Instance;

      return View("LateSubmission");
    }
      
      [ISAuthorize(Business.Security.Permissions.AchOps.ManageLateSubmissionAccess)]
    public ActionResult AchLateSubmission()
    {
      ViewData["header"] = "Late Submission Invoices Pending ACH Acceptance";
      ViewData["lateSubStatus"] = _calenderManager.IsLateSubmissionWindowOpen(ClearingHouse.Ach, _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach));
      ViewData["userCategory"] = (int)UserCategory.AchOps;
     
      /*Master Grid Display*/
      var gridModel = new LateSubmissionHeaderGrid("LateSubmissionGrid", Url.Action("LateSubmissionGridData", "LateSubmission", new { userCategory = (int)UserCategory.AchOps }));
      ViewData["LateSubmissionGrid"] = gridModel.Instance;

      /*Detail Grid Display*/
      var gridModelDetail = new LateSubmissionInvoiceDetail("LateSubmissionInvoiceDetailsGrid", Url.Action("LateSubmissionInvoiceDetailsGridData", "LateSubmission"));
      ViewData["LateSubmissionInvoiceDetailsGrid"] = gridModelDetail.Instance;

      return View("LateSubmission");
    }

    public JsonResult LateSubmissionGridData(int userCategory)
    {
      var gridModel = new LateSubmissionHeaderGrid("LateSubmissionGrid",
                                                   Url.Action("LateSubmissionGridData", "LateSubmission"));
      string type = userCategory == (int)UserCategory.IchOps ? "Ich" : "Ach";

      return gridModel.DataBind(_lateSubmissionManager.GetLateSubmittedInvoiceMemberSummary(type).AsQueryable());
       
    }

    public JsonResult LateSubmissionInvoiceDetailsGridData()
    {
      return null;
    }

    public JsonResult GetSelectedMemberInvoiceDetail(int memberId, int catId)
    {   
      string clearenceType = catId == (int)UserCategory.IchOps ? "Ich" : "Ach";

      var gridModelDetail = new LateSubmissionInvoiceDetail("LateSubmissionInvoiceDetailsGrid", Url.Action("LateSubmissionInvoiceDetailsGridData", "LateSubmission"));
      var list = _lateSubmissionManager.GetLateSubmittedInvoicesByMemberId(clearenceType, memberId);

      return gridModelDetail.DataBind(list.AsQueryable());
    }

    public JsonResult AcceptInvoices(string invoiceIds)
    {
      //SCP99417:Is web performance
      //var allRequestedInvoicesList = _lateSubmissionManager.GetLateSubmissionInvoicesByInvoiceIds(invoiceIds);

         var logRefId = Guid.NewGuid();
         var log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptInvoices", this.ToString(),
                                     "Ich", "Stage 1: AcceptLateSubmittedInvoice start", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      var acceptedInvoicesList = _lateSubmissionManager.AcceptLateSubmittedInvoice(invoiceIds);

      log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptInvoices", this.ToString(),
                                  "Ich", "Stage 1: AcceptLateSubmittedInvoice completed", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      var returnMessage = string.Empty;
     /* var allInvoicesString = "";

      foreach (var allInv in allRequestedInvoicesList)
      {
        allInvoicesString = allInvoicesString + allInv.InvoiceNumber + "<br/>";
      }*/

         // SCP52187: Deleted invoice appearing in Late submissions
        //  Error and success message style has been changed.Before it showned in green color despite the message is error
      var details = new UIExceptionDetail();

      if (acceptedInvoicesList.Count == 0)
      {
        var allRequestedInvoicesList = _lateSubmissionManager.GetLateSubmissionInvoicesByInvoiceIds(invoiceIds);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptInvoices", this.ToString(),
                                "Ich", "Stage 2: GetLateSubmissionInvoicesByInvoiceIds completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      
        var allInvoicesString = "";

        foreach (var allInv in allRequestedInvoicesList)
        {
          allInvoicesString = allInvoicesString + allInv.InvoiceNumber + "<br/>";
        }

          
          details.Message = "Unable to ACCEPT below mentioned invoice(s) as either billing member is not active or billed member is terminated <br/>" +
            allInvoicesString;
          details.IsFailed = true;

          log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptInvoices", this.ToString(),
                               "Ich", "Stage 3:" + details.Message, SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);

          return Json(details);
      }
      else
      {
          details.Message = "Invoice(s) accepted successfully";
          details.IsFailed = false;

          log = _referenceManager.GetDebugLog(DateTime.Now, "AcceptInvoices", this.ToString(),
                                "Ich", "Stage 2: AcceptInvoices Successfully completed", SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);

          return Json(details);
      }
    }

    public JsonResult RejectInvoices(string invoiceIds, int catId)
    {
      // For Accept  and Reject transaction should not be allowed to process by two user simultaneously.
      Thread.Sleep(SessionUtil.UserId + 500);

      // Check selected Invoice whether Accepted
      var lateInvoiceList = _lateSubmissionManager.GetLateSubmissionInvoicesByInvoiceIds(invoiceIds);
      var acceptedInvoicesString = string.Empty;

      foreach (var invoiceBase in lateInvoiceList)
      {
        if (invoiceBase.LateSubmissionRequestStatus == LateSubmissionRequestStatus.Accepted)
        {
          acceptedInvoicesString = acceptedInvoicesString + invoiceBase.InvoiceNumber + "<br/>";
        }
      }

      bool flag = _lateSubmissionManager.RejectLateSubmittedInvoice(invoiceIds, catId);

      var returnMessage = string.Empty;
      if (!string.IsNullOrEmpty(acceptedInvoicesString) && flag)
      {
        returnMessage = "Invoice(s) rejected successfully: Unable to REJECT below mentioned invoice(s) as these are already ACCEPTED and taken ahead for processing:<br/>" +
            acceptedInvoicesString;
        return Json(returnMessage);
      }
      if (!string.IsNullOrEmpty(acceptedInvoicesString) && !flag)
      {
        returnMessage = "Unable to REJECT below mentioned invoice(s) as these are already ACCEPTED and taken ahead for processing:<br/>" + acceptedInvoicesString;
        return Json(returnMessage);
      }
      if (string.IsNullOrEmpty(acceptedInvoicesString) && flag)
      {
        returnMessage = "Invoice(s) rejected successfully";

        return Json(returnMessage);
      }
      if (string.IsNullOrEmpty(acceptedInvoicesString) && !flag)
      {
        returnMessage = "Unable to REJECT invoice(s) as they are already REJECTED";
        return Json(returnMessage);

      }

      return Json(flag);

    }

    public JsonResult CloseLateSubmissionWindow(int catId)
    {
        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow", this.ToString(),
                                     "Ich", "Stage 1: CloseLateSubmissionWindow start", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      string clearenceType;
      BillingPeriod billingPeriod;
      if (catId == (int)UserCategory.IchOps)
      {
        clearenceType = "Ich";
        billingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);
        log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow", this.ToString(),
                                   "Ich", "Stage 2: GetLastClosedBillingPeriod-Ich completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
        
      }
      else
      {
        clearenceType = "Ach";
        billingPeriod = _calenderManager.GetLastClosedBillingPeriod(ClearingHouse.Ach);
        log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow", this.ToString(),
                                "Ich", "Stage 2: GetLastClosedBillingPeriod-Ach completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
        
      }

      try
      {   
          var errorMessage = _lateSubmissionManager.CloseLateSubmissionWindow(clearenceType, billingPeriod, SessionUtil.UserId, DateTime.UtcNow);
        log = _referenceManager.GetDebugLog(DateTime.Now, "CloseLateSubmissionWindow", this.ToString(),
                              "Ich", "Stage 1: CloseLateSubmissionWindow completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        if (string.IsNullOrEmpty(errorMessage))
        {
          return Json(string.Format("The {0} late submission window is closed successfully.", clearenceType.ToUpper()));
        }
        return Json(errorMessage);
      }
      catch (Exception exception)
      {
        logger.Info("Exception occurred in CloseLateSubmissionWindow().", exception);
        return Json(string.Format("An error occurred while manually closing the {0} late submission window.", clearenceType.ToUpper()));
      }
    }
  }
}
