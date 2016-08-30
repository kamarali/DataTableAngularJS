using System.Configuration;
using System.Linq;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.Util;
using System.Web.Mvc;
using Iata.IS.Model.MiscUatp;
using System.Web.Script.Serialization;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Core.Exceptions;
using Iata.IS.Business.MemberProfile;
using BillingType = Iata.IS.Model.Enums.BillingType;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.UIModel.ErrorDetail;
using System;
using Iata.IS.Business;
using System.IO;
using Iata.IS.Core;
using Enyim.Caching;
using System.Reflection;

namespace Iata.IS.Web.Areas.Uatp.Controllers
{
  public class ManageUatpInvoiceController : ManageMiscUatpControllerBase
  {
      private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
      private const string SearchResultGridAction = "SearchResultGridData";
      private const string InvoiceControllerNameSuffix = "Invoice";
      private const string CreditNoteControllerNameSuffix = "CreditNote";
      private const string ViewActionName = "View";
      private const string EditActionName = "Edit";
      private readonly IMemberManager _memberManager;
      public IUatpInvoiceManager UatpInvoiceManager { get; set; }
      public IRepository<InvoiceBase> InvoiceReository { get; set; }

      public ManageUatpInvoiceController(IMemberManager memberManager)
    {
      BillingCategory = BillingCategoryType.Uatp;
      AreaText = "Uatp";
      _memberManager = memberManager;
    }

    protected override IMiscUatpInvoiceManager Manager
    {
      get
      {
        return UatpInvoiceManager;
      }
    }

    protected override RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
    {
      var uatpConfig = MemberManager.GetUATPConfiguration(SessionUtil.MemberId);
      return uatpConfig != null ? uatpConfig.RejectionOnValidationFailure : base.GetRejectionOnValidationFailureFlag();
    }

      //========================New override action==================================
    [ISAuthorize(Business.Security.Permissions.UATP.Receivables.Manage.Query)]
    public override ActionResult Index(MiscSearchCriteria miscSearchCriteria)
    {
        SessionUtil.SearchType = "ManageInvoice";

        try
        {
            if (miscSearchCriteria != null)
            {
                miscSearchCriteria.BillingCategory = BillingCategory;
                // If Billed Member text is empty, reset the Billed Member Id
                // this check is made to handle the scenario where user has explicitly deleted the contents 
                // from Billed Member text box.
                if (string.IsNullOrEmpty(miscSearchCriteria.BilledMemberText))
                {
                    miscSearchCriteria.BilledMemberId = -1;
                }
                if (miscSearchCriteria.BillingPeriod == 0)
                {
                    var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
                    miscSearchCriteria.BillingPeriod = currentBillingPeriod.Period;
                    miscSearchCriteria.BillingMonth = currentBillingPeriod.Month;
                    miscSearchCriteria.BillingYear = currentBillingPeriod.Year;
                }
            }

            string criteria = miscSearchCriteria != null ? new JavaScriptSerializer().Serialize(miscSearchCriteria) : string.Empty;

            var invoiceSearchGrid = new UatpInvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
            {
                area = AreaText,
                criteria
            }));

            ViewData[ViewDataConstants.RejectionOnValidationFlag] = GetRejectionOnValidationFailureFlag();
            //(miscConfig != null) ? miscConfig.RejectionOnValidationFailureId : (int)RejectionOnValidationFailure.RejectInvoiceInError;
            ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;
            ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
        }
        catch (ISBusinessException exception)
        {
            ShowErrorMessage(exception.ErrorCode);
        }

        return View(miscSearchCriteria);
    }


    public override JsonResult SearchResultGridData(string criteria, string sidx, string sord, int page, int rows)
    {
        MiscSearchCriteria searchCriteria = null;

        if (Request.UrlReferrer != null)
        {
            SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
            // Clearing the other two session variables so that 'Back to Billing History' is not seen.
            SessionUtil.MiscCorrSearchCriteria = null;
            SessionUtil.MiscInvoiceSearchCriteria = null;
        }
        //// TODO : Exception handling 
        if (!string.IsNullOrEmpty(criteria))
        {
            searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(MiscSearchCriteria)) as MiscSearchCriteria;
        }
        if (searchCriteria == null)
        {
            // if not criteria is fetch or can be created using the string, create empty default search.
            searchCriteria = new MiscSearchCriteria();
        }

        searchCriteria.BillingCategory = BillingCategory;
        searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? CalendarManager.GetCurrentBillingPeriod().Period : searchCriteria.BillingPeriod;

        if (searchCriteria.OwnerId == 0)
        {
            searchCriteria.OwnerId = SessionUtil.UserId;
        }

        // Create grid instance and retrieve data from database
        var invoiceSearchGrid = new UatpInvoiceSearchGrid(ControlIdConstants.SearchGrid,
                                                      Url.Action(SearchResultGridAction,
                                                                 new
                                                                 {
                                                                     area = AreaText,
                                                                     searchCriteria
                                                                 }));

        // add billing member id to search criteria.
        searchCriteria.BillingMemberId = SessionUtil.MemberId;
        var invoiceSearchedData = Manager.SearchInvoice(searchCriteria);
        return invoiceSearchGrid.DataBind(invoiceSearchedData.AsQueryable());
    }
    
    [RestrictInvoiceUpdate(InvParamName = "id", InvList = true, TableName = TransactionTypeTable.INVOICE)]
    public override ActionResult SubmitInvoices(string id, int alreadySubmittedInvCount = 0, int alreadyDeletedInvCount = 0)
    {
        // CMP400: handling deleted invoices also by using alreadyDeletedInvCount
        UIMessageDetail messageDetails;
        if (!string.IsNullOrEmpty(id))
        {
            var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            try
            {

                var submittedInvoices = Manager.SubmitInvoices(invoiceIdList.ToList());
                if (submittedInvoices.Count > 0)
                {
                    if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount > 0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} invoice(s) submitted Successfully. {1} invoice(s) already submitted. {2} invoice(s) already deleted.",
                                    submittedInvoices.Count, alreadySubmittedInvCount, alreadyDeletedInvCount)
                        };

                    }
                    else if (alreadySubmittedInvCount > 0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} invoice(s) Submitted Successfully. {1} invoice(s) already submitted..",
                                    submittedInvoices.Count, alreadySubmittedInvCount)
                        };

                    }
                    else if (alreadyDeletedInvCount > 0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} invoice(s) submitted successfully. {1} invoice(s) already deleted.",
                                    submittedInvoices.Count, alreadyDeletedInvCount)
                        };

                    }
                    else
                    {

                        messageDetails =
                            new UIMessageDetail
                            {
                                IsFailed = false,
                                Message = string.Format(Messages.InvoicesSubmittedCount, submittedInvoices.Count)
                            };
                    }
                }
                else
                {
                    if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount > 0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    Messages.InvoiceIneligibleForSubmission + "{0} invoice(s) already submitted. {1} invoice(s) already deleted.",
                                  alreadySubmittedInvCount, alreadyDeletedInvCount)
                        };

                    }
                    else if (alreadyDeletedInvCount > 0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(Messages.InvoiceIneligibleForSubmission +
                                                         " {0} invoice(s) already deleted.", alreadyDeletedInvCount)
                        };
                    }
                    else if (alreadySubmittedInvCount > 0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = true,
                            Message =
                                string.Format(
                                    Messages.InvoiceIneligibleForSubmission +
                                    " {0} invoice(s) already submitted..",
                                    alreadySubmittedInvCount)
                        };
                    }
                    else
                    {
                        messageDetails =
                            new UIMessageDetail
                            {
                                IsFailed = true,
                                Message = Messages.InvoiceIneligibleForSubmission
                            };
                    }
                }


                return Json(messageDetails);
            }
            catch (ISBusinessException exception)
            {
                messageDetails = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = GetDisplayMessageWithErrorCode(exception.ErrorCode)
                };

                return Json(messageDetails);
            }
        }

        else
        {
            if (alreadySubmittedInvCount > 0)
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = "Invoice(s) already submitted. Please refresh your page." };
            }
            else
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.SelectInvoiceForSubmission };
            }

            return Json(messageDetails);
        }

    }

    [HttpPost]
    public override ActionResult PresentInvoices(string id)
    {
        UIMessageDetail messageDetails;

        var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        try
        {
            var presentInvoices = Manager.PresentInvoices(invoiceIdList.ToList());
            if (presentInvoices.Count > 0)
            {
                messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesSubmittedCount, presentInvoices.Count) };
            }
            else
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
            }

            return Json(messageDetails);
        }
        catch (ISBusinessException exception)
        {
            messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

            return Json(messageDetails);
        }
    }

    public override ActionResult MarkInvoicesToProcessingComplete(string id)
    {
        UIMessageDetail messageDetails;

        var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        try
        {
            var processingCompleteInvoices = Manager.ProcessingCompleteInvoices(invoiceIdList.ToList());
            if (processingCompleteInvoices.Count > 0)
            {
                messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesProcessingCompleteCount, processingCompleteInvoices.Count) };
            }
            else
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForProcessingComplete };
            }

            return Json(messageDetails);
        }
        catch (ISBusinessException exception)
        {
            messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

            return Json(messageDetails);
        }
    }

    public override ActionResult EditInvoice(string id)
    {
        var invoice = Manager.GetInvoiceDetail(id);

        string controllerName = string.Format("{0}{1}",
          AreaText,
          invoice.InvoiceType != InvoiceType.CreditNote ? InvoiceControllerNameSuffix : CreditNoteControllerNameSuffix);

        return RedirectToAction(EditActionName, controllerName, new
        {
            area = AreaText,
            invoiceID = id
        });
    }

    public override ActionResult ViewInvoice(string id)
    {
        var invoice = Manager.GetInvoiceDetail(id);

        string controllerName = string.Format("{0}{1}",
          AreaText,
          invoice.InvoiceType != InvoiceType.CreditNote ? InvoiceControllerNameSuffix : CreditNoteControllerNameSuffix);

        return RedirectToAction(ViewActionName, controllerName, new
        {
            area = AreaText,
            invoiceID = id
        });
    }

    
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult ValidateInvoice(string id)
    {
        UIMessageDetail details;
        var invoice = Manager.GetInvoiceDetail(id);

        try
        {

            invoice = Manager.ValidateInvoice(id);
            details = invoice.InvoiceStatus == InvoiceStatusType.ReadyForSubmission
                        ? new UIMessageDetail
                        {
                            IsFailed = false,
                            Message = string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber)
                        }
                        : new UIMessageDetail
                        {
                            IsFailed = true,
                            Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber)
                        };
        }
        catch (ISBusinessException)
        {
            details = new UIMessageDetail
            {
                IsFailed = true,
                Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber)
            };
        }

        return Json(details);
    }

    [HttpGet]
    public override FileStreamResult DownloadPdf(string id)
    {
        var invoicePdfFilePath = InvoiceManager.GetInvoiceLegalPfdPath(id.ToGuid());
        var fileName = Path.GetFileName(invoicePdfFilePath);
        try
        {
            var fs = System.IO.File.Open(invoicePdfFilePath, FileMode.Open, FileAccess.Read);
            return File(fs, "application/octet", fileName);
        }
        catch (Exception exception)
        {
            Logger.Error(string.Format("Exception:{0} StackTrace:{1}", exception.Message, exception.StackTrace), exception);

            var memoryStream = ConvertUtil.GetMemoryStreamForMessage("Error occurred while downloading pdf file.");

            return File(memoryStream, "text/plain", "Download-Pdf-Error.txt");
        }
    }

    protected override void HandleUnknownAction(string actionName)
    {
        EditInvoice(Request.QueryString["transactionId"]);

        base.HandleUnknownAction(actionName);
    }

    public override JsonResult ClearSearch()
    {
        // clear search criteria from Session.
        SessionUtil.InvoiceSearchCriteria = null;
        // set the default values.
        //var defaultPeriod = CalendarManager.GetCurrentBillingPeriod();
        var defaultPeriod = "";
        try
        {
            defaultPeriod = CalendarManager.GetCurrentBillingPeriod().ToString();
        }
        catch (ISCalendarDataNotFoundException)
        {
            defaultPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich).ToString();
        }

        return Json(defaultPeriod);
    }
//=============================================================================
  }
}
