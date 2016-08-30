using System.Collections.Generic;
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
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.MUPayables;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;
using BillingType = Iata.IS.Web.Util.BillingType;
using System.IO;
using System;

namespace Iata.IS.Web.Controllers.Base
{
  public class ManageMUPayablesControllerBase : ISController
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string SearchResultGridAction = "SearchResultGridData";
    protected virtual BillingCategoryType BillingCategory { get; set; }
    private const string InvoiceControllerNameSuffix = "Invoice";
    private const string CreditNoteControllerNameSuffix = "CreditNote";
    private const string ViewActionName = "View";
    protected string AreaText { get; set; }
    public readonly ICalendarManager _calendarManager;
    public readonly IMiscUatpInvoiceManager _miscUatpInvoiceManager;

    public IInvoiceManager InvoiceManager { get; set; }
    public IInvoiceOutputManager InvoiceOutputManager { get; set; }
    public IInvoiceOfflineCollectionDownloadManager InvoiceOfflineCollectionDownloadManager { get; set; }

    public ManageMUPayablesControllerBase(IMiscUatpInvoiceManager miscUatpInvoiceManager, ICalendarManager calendarManager)
    {
      _miscUatpInvoiceManager = miscUatpInvoiceManager;
      _calendarManager = calendarManager;
    }
    //
    // GET: /ManageMUPayablesControllerBase/

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    // [ISAuthorize(Business.Security.Permissions.Misc.Payables.Search.Query)]
    public virtual ActionResult Index(MiscSearchCriteria miscSearchCriteria)
    {
      SessionUtil.SearchType = "ManagePayablesInvoice";

      try
      {
        if (miscSearchCriteria != null)
        {
          // Retrieve LastBillingPeriod details
          var lastClosedBillingPeriodDetails = _calendarManager.GetLastClosedBillingPeriod();
          // If BillingPeriod/Year/Month == 0, set it with values retrieved from Last Billing Period
          miscSearchCriteria.BillingPeriod = miscSearchCriteria.BillingPeriod == 0 ? lastClosedBillingPeriodDetails.Period : miscSearchCriteria.BillingPeriod;
          miscSearchCriteria.BillingMonth = miscSearchCriteria.BillingMonth == 0 ? lastClosedBillingPeriodDetails.Month : miscSearchCriteria.BillingMonth;
          miscSearchCriteria.BillingYear = miscSearchCriteria.BillingYear == 0 ? lastClosedBillingPeriodDetails.Year : miscSearchCriteria.BillingYear;
          miscSearchCriteria.BillingCategory = BillingCategory;
          // If Billing Member text is empty, reset the Billing Member Id
          // this check is made to handle the scenario where user has explicitly deleted the contents 
          // from Billing Member text box.
          if (string.IsNullOrEmpty(miscSearchCriteria.BilledMemberText))
          {
            miscSearchCriteria.BillingMemberId = -1;
          }

          //CMP #655: IS-WEB Display per Location ID          
          //2.10	MISC IS-WEB PAYABLES - INVOICE SEARCH SCREEN
          var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager)); // IOC Resolve for interface
          var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId, SessionUtil.MemberId);
          ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode");
          if (miscSearchCriteria.BillingMemberLoc == null)
          {
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
          //End Code CMP#655

          //miscSearchCriteria.BillingPeriod = miscSearchCriteria.BillingPeriod == 0 ? CalendarManager.GetLastClosedBillingPeriod()0.GetCurrentBillingPeriod(DateTime.UtcNow).Period : miscSearchCriteria.BillingPeriod;
        }

        string criteria = miscSearchCriteria != null ? new JavaScriptSerializer().Serialize(miscSearchCriteria) : string.Empty;

        var invoiceSearchGrid = new MiscInvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
        {
          criteria
        }), (int)BillingCategory);

        ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;

        //CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen] 
        var attachmentGrid = new MiscPayableAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", new { }));
        ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;

      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      return View(miscSearchCriteria);
    }

    /// <summary>
    /// This function is used to load attachment Grid data from database. 
    /// CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen] 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult AttachmentGridData(string invoiceId)
    {
        var attachmentGrid = new MiscPayableAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", new { invoiceId }));
        if (!string.IsNullOrEmpty(invoiceId))
        {
            var attachmentList = _miscUatpInvoiceManager.GetAttachments(invoiceId);

            //Set the FileSizeInKb to file size in Kilo bytes.
            attachmentList = attachmentList.Select(attachment => { attachment.FileSizeInKb = (attachment.FileSize / 1024M); return attachment; }).ToList();

            SetSerialNoForAttachment(attachmentList);

            return attachmentGrid.DataBind(attachmentList.AsQueryable());
        }
        return attachmentGrid.DataBind(null);
    }

    /// <summary>
    /// Fetch invoice searched result and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult SearchResultGridData(string criteria)
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
      //  searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? CalendarManager.GetCurrentBillingPeriod(DateTime.UtcNow).Period : searchCriteria.BillingPeriod;

      // Create grid instance and retrieve data from database
      var invoiceSearchGrid = new MiscInvoiceSearchGrid(ControlIdConstants.SearchGrid,
                                                    Url.Action(SearchResultGridAction,
                                                               new
                                                               {
                                                                 searchCriteria
                                                               }), (int)BillingCategory);

      // add billed member id to search criteria.
      searchCriteria.BilledMemberId = SessionUtil.MemberId;
      var invoiceSearchedData = _miscUatpInvoiceManager.SearchPayableInvoices(searchCriteria);

      return invoiceSearchGrid.DataBind(invoiceSearchedData.AsQueryable());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Invoice id.</param>
    /// <param name="searchType">Search type(payables/billing history).</param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult ViewInvoice(string id, string searchType)
    {
        //SCP325374 - File Loading & Web Response Stats -PayablesInvoiceSearch
        var invoice = _miscUatpInvoiceManager.GetInvoiceHeaderForManageScreen(id);

      string controllerName = string.Format("{0}{1}",
        AreaText,
        invoice.InvoiceType != InvoiceType.CreditNote ? InvoiceControllerNameSuffix : CreditNoteControllerNameSuffix);

        string areaName = AreaText;
        if (AreaText == "MiscPay" || AreaText == "MiscDailyPay")
        {
            areaName = "Misc";
        }
        else if (AreaText == "UatpPay")
            areaName = "Uatp";

      return RedirectToAction(ViewActionName, controllerName, new
      {
        area = areaName,
        invoiceID = id,
        billingType = BillingType.Payables,
        searchType
      });
    }

    [HttpGet]
    public ActionResult RejectInvoice(string id, string searchType)
    {
      var invoice = _miscUatpInvoiceManager.GetInvoiceDetail(id);

      string controllerName = string.Format("{0}{1}",
        AreaText,
        invoice.InvoiceType != InvoiceType.CreditNote ? InvoiceControllerNameSuffix : CreditNoteControllerNameSuffix);

      TempData["Reject"] = true;
      string areaName = AreaText;
      if (AreaText == "MiscPay")
      {
          areaName = "Misc";
      }
      else if (AreaText == "UatpPay")
          areaName = "Uatp";

      return RedirectToAction(ViewActionName, controllerName, new
      {
        area = areaName,
        invoiceID = id,
        searchType
      });
    }

    /// <summary>
    /// Rejection through Payables.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public JsonResult RejectLineItems(string id)
    {
      var uiMessageDetail = new UIMessageDetail();

      bool isCreditNoteRejection;

      // Check if invoice cannot be rejected or if it is a credit note.
      string message = _miscUatpInvoiceManager.GetRejectionErrorMessage(id, out isCreditNoteRejection);

      // For credit note, a confirmation box needs to be displayed. Hence setting the error code.
      if (message != null)
      {
        if (isCreditNoteRejection) uiMessageDetail.ErrorCode = message;
        // For other invoice types, an alert will be displayed.
        else uiMessageDetail.Message = message;
      }

      if (string.IsNullOrEmpty(uiMessageDetail.Message)) // If error message not set, redirect the user to View invoice with Reject button.
      {
        uiMessageDetail.RedirectUrl = Url.Action("RejectInvoice", new { id, searchType = "p" });
      }

      return Json(uiMessageDetail);
    }

    [HttpGet]
    public FileStreamResult DownloadPdf(string id)
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
          int memberId = InvoiceManager.GetInvoice(invoiceId).BilledMemberId;

          if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
          {
            var optionList = options.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                                                      { "IS_RECEIVABLE", "0" },
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
      catch (Exception ex)
      {
        details = new UIMessageDetail
        {
          IsFailed = true,
          Message = "Failed to download the invoice, please try again!"
        };
      }

      return Json(details);
    }
     
    /// <summary>
    /// This action to used to download listing pdf file.
    /// CMP #665:User Related Enhancements-FRS-v1.2 Sec 2.9: IS-WEB MISC Payables Invoice Search Screen
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult DownloadListing(string id, string IsDailyPayable = "")
    {
        var invoiceListingPdfFilePath = InvoiceOfflineCollectionDownloadManager.GetInvoiceListingPdfPath(id.ToGuid());

        //If listing path available then request to download otherwise display error message.
        if (!string.IsNullOrWhiteSpace(invoiceListingPdfFilePath) && Directory.Exists(invoiceListingPdfFilePath))
        {
            string filePath = Directory.GetFiles(invoiceListingPdfFilePath, "*.pdf").FirstOrDefault();

            var fileName = Path.GetFileName(filePath);
            try
            {
                //Open File
                var fs = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);
                return File(fs, "application/octet", fileName);
            }
            catch (Exception exception)
            {
                //Show error message on the is-web if error occurred while download listing pdf. 
                ShowErrorMessage(
                    "The Listing for this Invoice/Credit Note is not available. Please contact SIS Helpdesk for resolution",
                    true);
                Logger.Error(string.Format("Exception:{0} StackTrace:{1}", exception.Message, exception.StackTrace),
                             exception);

            }
        }

        Logger.Info(string.Format("The Listing PDF for this Invoice/Credit Note is not available. Invoice ID: {0} ", id));

        //Show error message on the is-web if error occurred while download listing pdf. 
        ShowErrorMessage(
            "The Listing for this Invoice/Credit Note is not available. Please contact SIS Helpdesk for resolution",
            true);

        return IsDailyPayable == "1"
                   ? RedirectToAction("Index", "ManageMiscDailyPayablesInvoice")
                   : RedirectToAction("Index", "ManageMiscPayablesInvoice");
        //Redirect to default page.
        
    }

    /// <summary>
    /// This function is used to Download misc Invoice attachment.
    /// CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen] 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual FileStreamResult AttachmentDownload(string id)
    {
        var fileDownloadHelper = new FileAttachmentHelper { Attachment = _miscUatpInvoiceManager.GetInvoiceAttachmentDetail(id) };

        //Download File 
        return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Set serial no for attachment grid
    /// CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen] 
    /// </summary>
    /// <param name="attachment"></param>
    private static void SetSerialNoForAttachment(IEnumerable<MiscUatpAttachment> attachment)
    {
        var count = 1;
        foreach (var attach in attachment)
        {
            attach.SerialNo = count;
            count++;
        }
    }

    public JsonResult ClearSearch()
    {
      // clear search criteria from Session.
      SessionUtil.InvoiceSearchCriteria = null;
      // set the default values.
      var defaultPeriod = _calendarManager.GetLastClosedBillingPeriod();

      return Json(defaultPeriod);
    }
  }
}
