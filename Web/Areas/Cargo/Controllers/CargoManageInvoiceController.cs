using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Cargo;
//using Iata.IS.Business.Security.Permissions.Cargo.Receivables;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LateSubmission.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using System.IO;
using Iata.IS.Core;
using log4net;
using BillingType = Iata.IS.Web.Util.BillingType;
using ISearchInvoiceManager = Iata.IS.Business.Cargo.ISearchInvoiceManager;
using SearchCriteria = Iata.IS.Model.Cargo.SearchCriteria;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class CargoManageInvoiceController : ISController
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string SearchResultGridAction = "SearchResultGridData";
    private readonly ICalendarManager _calendarManager;
    private readonly ICargoInvoiceManager _invoiceManager;
    private readonly ISearchInvoiceManager _searchInvoiceManager;
    public IInvoiceManager InvoiceManagerBase { get; set; }
    public IRepository<InvoiceBase> InvoiceReository { get; set; }

    public IMemberManager MemberManager
    {
      get;
      set;
    }

    public CargoManageInvoiceController(ISearchInvoiceManager searchInvoiceManager,
                                   ICargoInvoiceManager invoiceManager,  
                                   ICalendarManager calendarManager
      )
    {
        _searchInvoiceManager = searchInvoiceManager;
        _invoiceManager = invoiceManager;
        _calendarManager = calendarManager;
    }

    /// <summary>
    /// Invoice search - When page get posted this method get invoked
    /// </summary>
   [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.Manage.Query)]
    public ActionResult Index(SearchCriteria searchCriteria)
    {
      try
      {
        if (searchCriteria != null)
        {
          var currentBillingPeriodDetails = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
          searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? currentBillingPeriodDetails.Period : searchCriteria.BillingPeriod;
          searchCriteria.BillingMonth = searchCriteria.BillingMonth == 0 ? currentBillingPeriodDetails.Month : searchCriteria.BillingMonth;
          searchCriteria.BillingYear = searchCriteria.BillingYear == 0 ? currentBillingPeriodDetails.Year : searchCriteria.BillingYear;
        }

        string criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

        var invoiceSearchGrid = new InvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction,"CargoManageInvoice", new { area = "Cargo", criteria }));
        ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;
        ViewData[ViewDataConstants.RejectionOnValidationFlag] = GetRejectionOnValidationFailureFlag();
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      return View(searchCriteria);
    }

   
    /// <summary>
    /// Fetch invoice searched result and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult SearchResultGridData(string criteria, string sidx, string sord, int page, int rows)
    {
      var searchCriteria = new SearchCriteria();

      if (Request.UrlReferrer != null)
      {
        SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
        SessionUtil.CGOCorrSearchCriteria = null;
        SessionUtil.CGOInvoiceSearchCriteria = null;
      }

      if (!string.IsNullOrEmpty(criteria))
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SearchCriteria)) as SearchCriteria;
      }

      searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetCurrentBillingPeriod().Period : searchCriteria.BillingPeriod;

      // Create grid instance and retrieve data from database
      var invoiceSearchGrid = new InvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction,"CargoManageInvoice" ,new { area = "Cargo", searchCriteria }));

      // add billing member id to search criteria.
      searchCriteria.BillingMemberId = SessionUtil.MemberId;

      // If Owner Id in searchCriteria is not set, then default to current user id.
      if (searchCriteria.OwnerId == 0)
      {
        searchCriteria.OwnerId = SessionUtil.UserId;
      }

      var invoiceSearchedData = _searchInvoiceManager.GetInvoices(searchCriteria, page, rows, sidx, sord).AsQueryable();
      
      #region SCP - 85037:
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

      return Json(jsonData, JsonRequestBehavior.AllowGet); //return invoiceSearchGrid.DataBind(invoiceSearchedData);
      #endregion
      //return invoiceSearchGrid.DataBind(invoiceSearchedData);

    }
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id",InvList = true, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult SubmitInvoices(string id, int alreadySubmittedInvCount = 0, int alreadyDeletedInvCount = 0)
    {
        UIMessageDetail messageDetails = new UIMessageDetail();

        // If user has not selected Invoices to submit display message
       // CMP400: handling deleted invoices also by using alreadyDeletedInvCount
        if (!string.IsNullOrEmpty(id))
        {
            var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            try
            {
               // ID : 296572 - Submission and Assign permission to user doesn't match !
               //Get invoice count if manage invoice screen display mix invoices.
               //if count > 0 then show generic error in alert message property.
               var invoicesTobeSubmit = _invoiceManager.ChkInvSubmitPermission(invoiceIdList, SessionUtil.UserId);
               var permissionReq = (invoiceIdList.Count - invoicesTobeSubmit.Count) > 0 ? "One or more invoice(s)/credit note(s) could not submitted due to missing permissions." : string.Empty;
               var submittedInvoices = _invoiceManager.SubmitInvoices(invoicesTobeSubmit);

                if (submittedInvoices.Count > 0)
                {
                    if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount >0)
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
                    else if(alreadySubmittedInvCount>0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} invoice(s) submitted Successfully. {1} invoice(s) already submitted.",
                                    submittedInvoices.Count, alreadySubmittedInvCount),
                            AlertMessage = permissionReq
                        };

                    }
                    else if (alreadyDeletedInvCount>0)
                    {
                        messageDetails = new UIMessageDetail
                        {
                            IsFailed = false,
                            Message =
                                string.Format(
                                    "{0} invoice(s) submitted Successfully. {1} invoice(s) already deleted.",
                                    submittedInvoices.Count, alreadyDeletedInvCount),
                            AlertMessage = permissionReq
                        };

                    }
                    else
                    {
                        messageDetails = new UIMessageDetail
                                             {
                                                 IsFailed = false,
                                                 Message =
                                                     string.Format(Messages.InvoicesSubmittedCount, submittedInvoices.Count),
                                                 AlertMessage = permissionReq
                                             };
                    }
                }
                else
                {
                    if (alreadySubmittedInvCount > 0 && alreadyDeletedInvCount >0)
                    {
                        messageDetails = new UIMessageDetail
                                             {
                                                 IsFailed = false,
                                                 Message =
                                                     string.Format(
                                                         Messages.InvoiceIneligibleForSubmission + "{0} invoice(s) already submitted. {1} invoice(s) already deleted.",
                                                       alreadySubmittedInvCount, alreadyDeletedInvCount),
                                                 AlertMessage = permissionReq
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
                                                         " {0} invoice(s) already submitted.", alreadySubmittedInvCount)+"/"+permissionReq
                                             };
                    }
                    else
                    {
                        messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission + "/" + permissionReq };
                    }
                }
                return Json(messageDetails);
            }
            catch (ISBusinessException exception)
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

                return Json(messageDetails);
            }
        }
        else
        {
            if (alreadySubmittedInvCount > 0)
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = "Invoice(s) already submitted. Please refresh your page."};
            }
            else
            {
                messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.SelectInvoiceForSubmission };
            }

            return Json(messageDetails);
        }
        //messageDetails = new UIMessageDetail { IsFailed = false, Message = "Saved Successfully" };
        //return Json(messageDetails);
    }
    //[HttpPost]
    //public ActionResult PresentInvoices(string id)
    //{
    //    UIMessageDetail messageDetails;

    //    var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    //    try
    //    {
    //        var presentInvoices = _invoiceManager.PresentInvoices(invoiceIdList.ToList());
    //        if (presentInvoices.Count > 0)
    //        {
    //            messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesSubmittedCount, presentInvoices.Count) };
    //        }
    //        else
    //        {
    //            messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForSubmission };
    //        }

    //        return Json(messageDetails);
    //    }
    //    catch (ISBusinessException exception)
    //    {
    //        messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

    //        return Json(messageDetails);
    //    }
    //}

    //[HttpPost]
    //public ActionResult MarkInvoicesToProcessingComplete(string id)
    //{
    //    UIMessageDetail messageDetails;

    //    var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    //    try
    //    {
    //        var processingCompleteInvoices = _invoiceManager.ProcessingCompleteInvoices(invoiceIdList.ToList());
    //        if (processingCompleteInvoices.Count > 0)
    //        {
    //            messageDetails = new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoicesProcessingCompleteCount, processingCompleteInvoices.Count) };
    //        }
    //        else
    //        {
    //            messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.InvoiceIneligibleForProcessingComplete };
    //        }

    //        return Json(messageDetails);
    //    }
    //    catch (ISBusinessException exception)
    //    {
    //        messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

    //        return Json(messageDetails);
    //    }
    //}

    /// <summary>
    /// This action method calls Invoice Edit page
    /// </summary>
    /// <returns></returns>
    public ActionResult EditInvoice(string id)
    {
        //SCP325376 - File Loading & Web Response Stats EditInvoice CargoManageInvoice
        var invoice = _invoiceManager.GetInvoiceDetails(id);

        string controller = invoice.InvoiceType == InvoiceType.Invoice ? "Invoice" : "CreditNote";
       
        ViewData[ViewDataConstants.PageMode] = PageMode.Edit;
        return RedirectToAction("Edit", controller, new { area = "Cargo", invoiceId = id });
    }
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult ValidateInvoice(string id)
    {
        // TODO: This method needs refactoring
        UIMessageDetail details;
        //SCP325375: File Loading & Web Response Stats ManageInvoice
        var invoice = _invoiceManager.GetInvoiceDetails(id);

       try
       {
           invoice = _invoiceManager.ValidateInvoice(id);

           details = invoice.InvoiceStatus ==InvoiceStatusType.ReadyForSubmission
                       ? new UIMessageDetail { IsFailed = false, Message = string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber) }
                       : new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
       }
       catch (ISBusinessException)
       {
           details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber) };
       }
        return Json(details);
    }

    public ActionResult ViewInvoice(string id)
    {
        //SCP325377 - File Loading & Web Response Stats ViewInvoice CargoManageInvoice
        var invoice = _invoiceManager.GetInvoiceDetails(id);

        string controller = invoice.InvoiceType == InvoiceType.Invoice ? "Invoice" : "CreditNote";
        
        return RedirectToAction("View", controller, new { area = "Cargo", invoiceId = id });
    }

    /// <summary>
    /// This action method calls invoice delete action method
    /// </summary>
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "id", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult DeleteInvoice(string id)
    {
        UIExceptionDetail details;

        //SCP325378 - File Loading & Web Response Stats DeleteInvoice CargoManageInvoice
        var invoice = _invoiceManager.GetInvoiceDetails(id);

        var dummyMemberId = MemberManager.GetMemberId(String.IsNullOrEmpty(ConfigurationManager.AppSettings["DummyMembercode"].Trim()) ? "000" : ConfigurationManager.AppSettings["DummyMembercode"].Trim());
        var userId = SessionUtil.AdminUserId;

        try
        {
            var invoiceIdStringInOracleFormat = ConvertUtil.ConvertNetGuidToOracleGuid(id);

            var isDeleted = InvoiceManagerBase.DeleteInvoice(invoiceIdStringInOracleFormat, dummyMemberId, userId, 1);

            details = isDeleted
                        ? new UIExceptionDetail { IsFailed = false, Message = string.Format(Messages.InvoiceDeleteSuccessful, invoice.InvoiceNumber) }
                        : new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
        }
        catch (ISBusinessException)
        {
            details = new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed, invoice.InvoiceNumber) };
        }
        //details = new UIExceptionDetail {};
        return Json(details);
    }
    [HttpGet]
    public FileStreamResult DownloadPdf(string id)
    {
        var invoicePdfFilePath = _invoiceManager.GetInvoiceLegalPfdPath(id.ToGuid());
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
              int memberId = _invoiceManager.GetInvoiceDetails(id).BillingMemberId;

              if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
              {
                var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                       "Invoice",
                                                                       new
                                                                       {
                                                                         area = "Cargo",
                                                                         billingType = "Receivables"
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
                                                      {"DOWNLOAD_URL", downloadUrl },
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
    private RejectionOnValidationFailure GetRejectionOnValidationFailureFlag()
    {
        var rejectionFlag = RejectionOnValidationFailure.RejectInvoiceInError;
        var rejectionFlagId = MemberManager.GetMemberConfigurationValue(SessionUtil.MemberId, MemberConfigParameter.CargoRejectionOnValidationFailure);
        if (!string.IsNullOrEmpty(rejectionFlagId))
            rejectionFlag = (RejectionOnValidationFailure)Convert.ToInt32(rejectionFlagId);
        return rejectionFlag;
    }

    [HttpPost]
    public ActionResult PresentInvoices(string id)
    {
      UIMessageDetail messageDetails;

      var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      try
      {
        var presentInvoices = _invoiceManager.PresentInvoices(invoiceIdList.ToList());
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

    [HttpPost]
    public ActionResult MarkInvoicesToProcessingComplete(string id)
    {
      UIMessageDetail messageDetails;

      var invoiceIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

      try
      {
        var processingCompleteInvoices = _invoiceManager.ProcessingCompleteInvoices(invoiceIdList.ToList());
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

  }
}
