using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;
using BillingType = Iata.IS.Model.Enums.BillingType;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Web.Controllers.Base
{
  public abstract class ManageMiscUatpControllerBase : ISController
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string SearchResultGridAction = "SearchResultGridData";
    private const string InvoiceControllerNameSuffix = "Invoice";
    private const string CreditNoteControllerNameSuffix = "CreditNote";
    private const string ViewActionName = "View";
    private const string EditActionName = "Edit";

    protected BillingCategoryType BillingCategory;

    protected string AreaText;

    //   public IMiscUatpInvoiceManager MiscUatpInvoiceManager { get; set; }
    public IInvoiceManager InvoiceManager { get; set; }

    public IInvoiceOutputManager InvoiceOutputManager { get; set; }

    public ICalendarManager CalendarManager { get; set; }

    public IMemberManager MemberManager { get; set; }
    public IRepository<InvoiceBase> InvoiceReository { get; set; }

    public IInvoiceOfflineCollectionDownloadManager InvoiceOfflineCollectionDownloadManager { get; set; }

    /// <summary>
    /// Abstract property which will return Manager object of type <see cref="IMiscUatpInvoiceManager"/>.
    /// The derived classes should return respective manager objects deriving from <see cref="IMiscUatpInvoiceManager"/>.
    /// </summary>
    /// <value>The manager.</value>
    protected abstract IMiscUatpInvoiceManager Manager { get; }


    public IMiscUatpInvoiceManager MiscUatpInvoiceManager { get; set; }

    protected virtual RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
    {
      // returning default value as RejectInvoiceInError.
      return RejectionOnValidationFailure.RejectInvoiceInError;
    }

    [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Manage.Query)]
    public virtual ActionResult Index(MiscSearchCriteria miscSearchCriteria)
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


            //CMP #655: IS-WEB Display per Location ID          
            //2.7	MISC IS-WEB RECEIVABLES - MANAGE INVOICE SCREEN
            var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager)); // IOC resolve for interface
            var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId, SessionUtil.MemberId);
            ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode"); // Fill the ViewData for Location List Box
            if (miscSearchCriteria.BillingMemberLoc == null)
            { // Default search criteria purpose
                foreach (var item in associatedLocations)
                {
                    miscSearchCriteria.BillingMemberLoc += "," + item.LocationCode;
                }
                if (associatedLocations.Count == 0) miscSearchCriteria.BillingMemberLoc = ",0";
            }
            else
            {// server Side Validation for Associatin Location
                var selectedBillingMemberLocationList = miscSearchCriteria.BillingMemberLoc.Split(Convert.ToChar(","));
                miscSearchCriteria.BillingMemberLoc = "";
                foreach (var location in from location in selectedBillingMemberLocationList
                                         where location != null
                                         let contains = associatedLocations.SingleOrDefault(l => l.LocationCode == location)
                                         where contains != null
                                         select location)
                {
                    miscSearchCriteria.BillingMemberLoc += "," + location;
                }
                if (miscSearchCriteria.BillingMemberLoc.Length == 0) miscSearchCriteria.BillingMemberLoc = ",0";
            }
            // End code CMP #655
            

            string criteria = miscSearchCriteria != null ? new JavaScriptSerializer().Serialize(miscSearchCriteria) : string.Empty;

            var invoiceSearchGrid = new MiscInvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
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


      /// <summary>
    /// Fetch invoice searched result and display it in grid
    /// </summary>
    /// <returns></returns>
    public virtual JsonResult SearchResultGridData(string criteria, string sidx, string sord, int page, int rows)
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

      //CMP #655: IS-WEB Display per Location ID
      //2.7	MISC IS-WEB RECEIVABLES - MANAGE INVOICE SCREEN
      if (searchCriteria.BillingMemberLoc != null)
      {
          if (searchCriteria.BillingMemberLoc.Length > 0)
              searchCriteria.BillingMemberLoc = searchCriteria.BillingMemberLoc.Substring(1, searchCriteria.BillingMemberLoc.Length - 1);
      }
      // Create grid instance and retrieve data from database
      var invoiceSearchGrid = new MiscInvoiceSearchGrid(ControlIdConstants.SearchGrid,
                                                    Url.Action(SearchResultGridAction,
                                                               new
                                                               {
                                                                 area = AreaText,
                                                                 searchCriteria
                                                               }));

      // add billing member id to search criteria.
      searchCriteria.BillingMemberId = SessionUtil.MemberId;

      var invoiceSearchedData = MiscUatpInvoiceManager.SearchInvoiceMisc(searchCriteria, page, rows, sidx, sord);
      #region SCP - 85039:

      //Total records related to search excluding page filter
      int totalRecords = invoiceSearchedData.Select(x => x.TotalRows).FirstOrDefault();

      //Calculating total pages grid will show
      int totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

      //Creating json result to bind to database.
      var jsonData = new
      {
          total = totalPages,
          page = page,
          records = totalRecords,
          rows = invoiceSearchedData.ToArray()
      };

      return Json(jsonData, JsonRequestBehavior.AllowGet); //invoiceSearchGrid.DataBind(invoiceSearchedData); 
      #endregion          
    }
   
    [RestrictInvoiceUpdate(InvParamName = "id", InvList = true, TableName = TransactionTypeTable.INVOICE)]
    public virtual ActionResult SubmitInvoices(string id, int alreadySubmittedInvCount = 0, int alreadyDeletedInvCount = 0)
    {
      // CMP400: handling deleted invoices also by using alreadyDeletedInvCount
      UIMessageDetail messageDetails;
     
      if (!string.IsNullOrEmpty(id))
      {
          var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

          try
          {
              // ID : 296572 - Submission and Assign permission to user doesn't match !
              //Get invoice count if manage invoice screen display mix invoices.
              //if count > 0 then show generic error in alert message property.
              var invoicesTobeSubmit = Manager.ChkInvSubmitPermission(invoiceIdList,SessionUtil.UserId);
              var permissionReq = (invoiceIdList.Count - invoicesTobeSubmit.Count) > 0 ?"One or more invoice(s)/credit note(s) could not submitted due to missing permissions.":string.Empty;
              var submittedInvoices = Manager.SubmitInvoices(invoicesTobeSubmit);
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
                                  submittedInvoices.Count, alreadySubmittedInvCount, alreadyDeletedInvCount),
                          AlertMessage = permissionReq
                      };

                  }
                  else if (alreadySubmittedInvCount > 0)
                  {
                      messageDetails = new UIMessageDetail
                                           {
                                               IsFailed = false,
                                               Message =
                                                   string.Format(
                                                       "{0} invoice(s) Submitted successfully. {1} invoice(s) already submitted.",
                                                       submittedInvoices.Count, alreadySubmittedInvCount),
                                               AlertMessage = permissionReq
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
                                  submittedInvoices.Count, alreadyDeletedInvCount),
                          AlertMessage = permissionReq
                      };

                  }
                  else
                  {

                      messageDetails =
                          new UIMessageDetail
                              {
                                  IsFailed = false,
                                  Message = string.Format(Messages.InvoicesSubmittedCount, submittedInvoices.Count), AlertMessage = permissionReq
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
                                alreadySubmittedInvCount, alreadyDeletedInvCount), AlertMessage = permissionReq
                      };

                  }
                  else if (alreadyDeletedInvCount > 0)
                  {
                      messageDetails = new UIMessageDetail
                      {
                          IsFailed = false,
                          Message =
                              string.Format(Messages.InvoiceIneligibleForSubmission +
                                                       " {0} invoice(s) already deleted.", alreadyDeletedInvCount),
                          AlertMessage = permissionReq
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
                                                       alreadySubmittedInvCount) + "/" + permissionReq
                                           };
                  }
                  else
                  {
                      messageDetails =
                          new UIMessageDetail
                              {
                                  IsFailed = true,
                                  Message = Messages.InvoiceIneligibleForSubmission + "/" + permissionReq
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
    public virtual ActionResult PresentInvoices(string id)
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

    public virtual ActionResult MarkInvoicesToProcessingComplete(string id)
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

    public virtual ActionResult EditInvoice(string id)
    {
        //SCP325377 - File Loading & Web Response Stats ViewInvoice CargoManageInvoice
        var invoice = Manager.GetInvoiceHeaderForManageScreen(id);

      string controllerName = string.Format("{0}{1}",
        AreaText,
        invoice.InvoiceType != InvoiceType.CreditNote ? InvoiceControllerNameSuffix : CreditNoteControllerNameSuffix);

      return RedirectToAction(EditActionName, controllerName, new
      {
        area = AreaText,
        invoiceID = id
      });
    }

    public virtual ActionResult ViewInvoice(string id)
    {
        //SCP325377: File Loading & Web Response Stats ViewInvoice CargoManageInvoice
        var invoice = Manager.GetInvoiceHeaderForManageScreen(id);

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
    public virtual JsonResult DeleteInvoice(string id)
    {
      UIExceptionDetail exceptionDetail;
      var dummyMemberId = MemberManager.GetMemberId(String.IsNullOrEmpty(ConfigurationManager.AppSettings["DummyMembercode"].Trim()) ? "000" : ConfigurationManager.AppSettings["DummyMembercode"].Trim());
      var userId = SessionUtil.AdminUserId;

      //SCP325378:File Loading & Web Response Stats DeleteInvoice CargoManageInvoice
      var invoice = Manager.GetInvoiceHeaderForManageScreen(id);

      try
      { 
        //var isDeleted = Manager.DeleteInvoice(id);
        //CMP 400: Before delete invoice. Add entry into INVOICE_DELETION_AUDIT table and update invoice billing and billed member by dummy member.
        var invoiceIdStringInOracleFormat = ConvertUtil.ConvertNetGuidToOracleGuid(id);

        //SCP312529 - IS-Web Performance - Processing Dashboard
        var isDeleted = InvoiceManager.DeleteInvoice(invoiceIdStringInOracleFormat, dummyMemberId, userId, 1);

        exceptionDetail = isDeleted
                            ? new UIExceptionDetail
                            {
                              IsFailed = false,
                              Message = string.Format(Messages.InvoiceDeleteSuccessful, invoice.InvoiceNumber)
                            }
                            : new UIExceptionDetail
                            {
                              IsFailed = true,
                              Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber)
                            };
      }
      catch (ISBusinessException)
      {
        exceptionDetail = new UIExceptionDetail
        {
          IsFailed = true,
          Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber)
        };
      }

      return Json(exceptionDetail);
    }

    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public virtual JsonResult ValidateInvoice(string id)
    {
      UIMessageDetail details;
      //SCP325375 - File Loading & Web Response Stats ManageInvoice
      var invoice = Manager.GetInvoiceHeaderForManageScreen(id);

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
    public virtual FileStreamResult DownloadPdf(string id)
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

    [HttpPost]
    public JsonResult DownloadZip(string id, string options)
    {
      UIMessageDetail details;
      try
      {
        Guid invoiceId;
        var zipFileName = Guid.NewGuid().ToString();
        if (Guid.TryParse(id, out invoiceId))
        {
          //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
          int memberId = InvoiceManager.GetInvoice(invoiceId).BillingMemberId;

          if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
          {
            var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                    "MiscInvoice",
                                    new
                                    {
                                      area = "Misc"
                                    }));

            var iInvoiceOfflineCollectionManager = Ioc.Resolve<IInvoiceOfflineCollectionManager>(typeof(IInvoiceOfflineCollectionManager));
            IDictionary<string, string> messages = new Dictionary<string, string>
                                                   {
                                                      { "RECORD_ID", ConvertUtil.ConvertGuidToString(invoiceId)},
                                                      { "USER_ID", SessionUtil.UserId.ToString() },
                                                      { "OPTIONS", options},
                                                      { "IS_FORM_C", "0" },
                                                      { "IS_RECEIVABLE", "1" },
                                                      { "OUTPUT_ZIP_FILE_NAME", zipFileName },
                                                      { "DOWNLOAD_URL", downloadUrl },
                                                      {"IS_WEB_DOWNLOAD","1"}
                                                   };
            // SCP227747: Cargo Invoice Data Download
            // Message will display on screen depending on Success or Failure of Enqueing message to queue.
            var isEnqueSuccess = false;
            isEnqueSuccess = iInvoiceOfflineCollectionManager.EnqueueDownloadRequest(messages);

            if (isEnqueSuccess)
            {
              details = new UIMessageDetail
              {
                IsFailed = false,
                Message =
                  string.Format(
                    "Generation of selected information in progress. You will be notified as per your profile settings, once the required information is ready and available for retrieval [File: {0}]",
                    string.Format("{0}.zip", zipFileName))
              };
            }
            else
            {
              details = new UIMessageDetail
              {
                IsFailed = true,
                Message = "Failed to download the invoice, please try again!"
              };
            }
          }
          else
          {
            details = new UIMessageDetail
            {
              IsFailed = true,
              RedirectUrl = Url.Action("LogOn", "Account", new { area = "" })
            };
          }
        }
        else
        {
          details = new UIMessageDetail
          {
            IsFailed = true,
            Message = "Given invoice id is not valid."
          };
        }
      }
      catch (Exception)
      {
        details = new UIMessageDetail
        {
          IsFailed = true,
          Message = "Failed to download the invoice, please try again!"
        };
      }

      return Json(details);
    }

    protected override void HandleUnknownAction(string actionName)
    {
      EditInvoice(Request.QueryString["transactionId"]);

      base.HandleUnknownAction(actionName);
    }

    public virtual JsonResult ClearSearch()
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
  }
}
