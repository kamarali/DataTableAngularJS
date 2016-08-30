using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using System.Web.Script.Serialization;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.Grid.PaxPayables;
using Iata.IS.Web.Util;
using Iata.IS.Core.Exceptions;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.UIModel.ErrorDetail;
using System.IO;
using log4net;

namespace Iata.IS.Web.Areas.PaxPayables.Controllers
{
  public class ManagePaxPayablesInvoiceController : ISController
  {
    //
    // GET: /PaxPayables/ManagePaxPayablesInvoice/
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string SearchResultGridAction = "SearchResultGridData";
    private readonly ICalendarManager _calendarManager;
    private readonly ISearchInvoiceManager _searchInvoiceManager;
    private readonly IInvoiceManager _invoiceManager;
    private readonly IInvoiceOfflineCollectionDownloadManager _invoiceOfflineCollectionDownloadManager;

    public ManagePaxPayablesInvoiceController(ICalendarManager icalendarManager, ISearchInvoiceManager isearchinvoicemanager, IInvoiceManager invoiceManager, IInvoiceOfflineCollectionDownloadManager invoiceOfflineCollectionDownloadManager)
    {
      _calendarManager = icalendarManager;
      _searchInvoiceManager = isearchinvoicemanager;
      _invoiceManager = invoiceManager;
      _invoiceOfflineCollectionDownloadManager = invoiceOfflineCollectionDownloadManager;
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Payables.Search.Query)]
    public ActionResult Index(SearchCriteria searchCriteria)
    {
      try
      {
        if (searchCriteria != null)
          searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetLastClosedBillingPeriod().Period : searchCriteria.BillingPeriod;

        string criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

        var invoiceSearchGrid = new InvoiceSearchGrid(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
        {
          area = "PaxPayables",
          criteria
        }));
        ViewData[ViewDataConstants.SearchGrid] = invoiceSearchGrid.Instance;
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
    [ISAuthorize(Business.Security.Permissions.Pax.Payables.Search.Query)]
    public JsonResult SearchResultGridData(string criteria)
    {
      var searchCriteria = new SearchCriteria();

      if (Request.UrlReferrer != null)
      {
        SessionUtil.InvoiceSearchCriteria = Request.UrlReferrer.ToString();
        SessionUtil.PaxCorrSearchCriteria = null;
        SessionUtil.FormCSearchCriteria = null;
        SessionUtil.PaxInvoiceSearchCriteria = null;
      }

      if (!string.IsNullOrEmpty(criteria))
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SearchCriteria)) as SearchCriteria;
      }

      if (searchCriteria != null)
      {
        searchCriteria.BillingYear = searchCriteria.BillingYear == 0 ? _calendarManager.GetLastClosedBillingPeriod().Year : searchCriteria.BillingYear;
        searchCriteria.BillingMonth = searchCriteria.BillingMonth == 0 ? _calendarManager.GetLastClosedBillingPeriod().Month : searchCriteria.BillingMonth;
        searchCriteria.BillingPeriod = searchCriteria.BillingPeriod == 0 ? _calendarManager.GetLastClosedBillingPeriod().Period : searchCriteria.BillingPeriod;
      }

      // Create grid instance and retrieve data from database
      var invoiceSearchGrid = new InvoiceSearchGrid(ControlIdConstants.SearchGrid,
                                                    Url.Action(SearchResultGridAction,
                                                               new
                                                               {
                                                                 area = "PaxPayables",
                                                                 searchCriteria
                                                               }));

      // add billed member id to search criteria.
      searchCriteria.BilledMemberId = SessionUtil.MemberId;
      var invoiceSearchedData = _searchInvoiceManager.GetAllPayables(searchCriteria).AsQueryable();

      return invoiceSearchGrid.DataBind(invoiceSearchedData);
    }

    [HttpGet]
    public ActionResult ViewInvoice(string id)
    {
        //SCPID : 325374 - File Loading & Web Response Stats -PayablesInvoiceSearch
        var invoice = _invoiceManager.GetInvoiceDetails(id);

      string controller;

      switch (invoice.BillingCode)
      {
        case (int)BillingCode.SamplingFormDE:
          controller = "FormDEPayables";
          break;
        case (int)BillingCode.NonSampling:
          controller = invoice.InvoiceType == InvoiceType.Invoice ? "InvoicePayables" : "CreditNotePayables";
          break;
        case (int)BillingCode.SamplingFormF:
          controller = "FormFPayables";
          break;
        case (int)BillingCode.SamplingFormXF:
          controller = "FormXFPayables";
          break;
        case (int)BillingCode.SamplingFormAB:
          controller = "FormABPayables";
          break;
        default:
          controller = invoice.InvoiceType == InvoiceType.Invoice ? "InvoicePayables" : "CreditNotePayables";
          break;
      }

      return RedirectToAction("View", controller, new { area = "Pax", invoiceID = id, BillingType = "Payables" });
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
          int memberId = _invoiceManager.GetInvoiceDetails(id).BilledMemberId;

          if (SessionUtil.UserId > 0 && SessionUtil.MemberId > 0 && memberId == SessionUtil.MemberId)
          {
            var downloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                    "Invoice",
                                    new
                                    {
                                      area = "Pax",
                                      billingType = "Payables"
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
  }
}
