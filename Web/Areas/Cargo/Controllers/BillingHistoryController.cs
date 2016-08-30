using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Web.UIModel.BillingHistory.Cargo;
using Enums = Iata.IS.Model.Enums;
using Iata.IS.Model.Cargo.BillingHistory;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.BillingHistory.Pax;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.Enums;
using Enyim.Caching;
using System.Reflection;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class BillingHistoryController : ISController
  {
    private const string BillingHistorySearchGridAction = "BillingHistorySearchGridData";
    private const string CGOBillingHistory = "CGOBillingHistory";
    private const string RejectionMemoSearchResultGridAction = "RejectionMemoSearchResultGridData";
    private readonly ICargoInvoiceManager _cargoInvoiceManager;
    private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
    private readonly ICargoCorrespondenceManager _cargoCorrespondenceManager;
    private readonly INonSamplingCreditNoteManager _nonSamplingCreditNoteManager;
    private readonly ISamplingFormCManager _samplingFormCManager;
    private readonly ISamplingFormDEManager _samplingformDEManager;
    public ISamplingFormCManager FormCManager { get; set; }
    private readonly IQueryAndDownloadDetailsManager _queryAndDownloadDetailsManager;
    private readonly IReferenceManager _referenceManager;
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public BillingHistoryController(ICargoInvoiceManager cargoInvoiceManager, ICargoCorrespondenceManager cargoCorrespondenceManager, INonSamplingInvoiceManager nonSamplingInvoiceManager, INonSamplingCreditNoteManager nonSamplingCreditNoteManager, ISamplingFormCManager samplingFormCManager, ISamplingFormDEManager samplingFormDEManager, IQueryAndDownloadDetailsManager queryAndDownloadDetailsManager, IReferenceManager referenceManager)
    {
      _cargoInvoiceManager = cargoInvoiceManager;
      _cargoCorrespondenceManager = cargoCorrespondenceManager;
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
      _nonSamplingCreditNoteManager = nonSamplingCreditNoteManager;
      _samplingFormCManager = samplingFormCManager;
      _samplingformDEManager = samplingFormDEManager;
      _queryAndDownloadDetailsManager = queryAndDownloadDetailsManager;
      _referenceManager = referenceManager;
    }

    // GET: /Pax/BillingHistory/

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    public ActionResult Index(InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria, string searchType)
    {
      try
      {

        string criteria = null;
        SessionUtil.SearchType = CGOBillingHistory;
        ViewData["AwbSerialNumber"] = searchCriteria.AwbSerialNumber;
        switch (searchType)
        {
          case "Invoice":
            criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;
            SessionUtil.CGOCorrSearchCriteria = null;
            SessionUtil.CGOInvoiceSearchCriteria = criteria;
            SessionUtil.FormCSearchCriteria = null;
            SessionUtil.InvoiceSearchCriteria = null;
            ViewData[ViewDataConstants.CorrespondenceSearch] = "Invoice";
            break;
          case "Correspondence":
            criteria = correspondenceSearchCriteria != null ? new JavaScriptSerializer().Serialize(correspondenceSearchCriteria) : string.Empty;
            SessionUtil.CGOCorrSearchCriteria = criteria;
            SessionUtil.CGOInvoiceSearchCriteria = null;
            SessionUtil.FormCSearchCriteria = null;
            SessionUtil.InvoiceSearchCriteria = null;
            ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
            ViewData["ClearIntegerFields"] = true;
            ShowLinkedRejectionMemo("");
            break;
          default:
            // Criteria values should not be picked from the Session when user comes from the menu.
            if (Request.QueryString["back"] == null)
            {
              // clear the session.
              SessionUtil.CGOCorrSearchCriteria = null;
              SessionUtil.CGOInvoiceSearchCriteria = null;
              ViewData["ClearIntegerFields"] = true;
            } // Values should come from Session only when user has clicked the Back to Billing history button.
            else
            {
              if (SessionUtil.CGOCorrSearchCriteria != null)
              {
                criteria = SessionUtil.CGOCorrSearchCriteria;
                correspondenceSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceSearchCriteria)) as CorrespondenceSearchCriteria;
                if (correspondenceSearchCriteria != null)
                {
                  if (correspondenceSearchCriteria.FromDate != null)
                  {
                    correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.FromDate.Value.ToLocalTime();
                  }
                  if (correspondenceSearchCriteria.ToDate != null)
                  {
                    correspondenceSearchCriteria.ToDate = correspondenceSearchCriteria.ToDate.Value.ToLocalTime();
                  }
                }

                ShowLinkedRejectionMemo("");
                ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
              }
              else if (SessionUtil.CGOInvoiceSearchCriteria != null)
              {
                criteria = SessionUtil.CGOInvoiceSearchCriteria;
                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(InvoiceSearchCriteria)) as InvoiceSearchCriteria;
                ViewData[ViewDataConstants.CorrespondenceSearch] = "Invoice";
              }
            }
            break;
        }

        if (SessionUtil.CGOCorrSearchCriteria == null) correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.ToDate = DateTime.UtcNow;

        ViewData[ViewDataConstants.invoiceSearchCriteria] = searchCriteria;
        ViewData[ViewDataConstants.correspondenceSearchCriteria] = correspondenceSearchCriteria;

        var billingHistorySearchResultGrid = new BillingHistorySearchGrid(ControlIdConstants.BHSearchResultsGrid,
                                                                          Url.Action(BillingHistorySearchGridAction,
                                                                                     new
                                                                                     {
                                                                                       criteria
                                                                                     }));
        ViewData[ViewDataConstants.BHSearchResultsGrid] = billingHistorySearchResultGrid.Instance;
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }
      return View();
    }

    public JsonResult BillingHistorySearchGridData(string criteria)
    {
      // Retrieve MemberId from Session variable and use it across the method
      var memberId = SessionUtil.MemberId;
      InvoiceSearchCriteria searchCriteria = null;
      CorrespondenceSearchCriteria correspondenceSearchCriteria = null;
    
      var billingHistorySearchResultGrid = new BillingHistorySearchGrid(ControlIdConstants.BHSearchResultsGrid,
                                                                        Url.Action(BillingHistorySearchGridAction,
                                                                                   new
                                                                                   {
                                                                                     area = "Cargo"
                                                                                   }));

      if (SessionUtil.CGOInvoiceSearchCriteria != null && criteria != null)
      {
        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(InvoiceSearchCriteria)) as InvoiceSearchCriteria;

        if (searchCriteria != null)
        {
          if (searchCriteria.BillingTypeId == (int)Enums.BillingType.Payables)
          {
            searchCriteria.BillingMemberId = searchCriteria.BilledMemberId;
            searchCriteria.BilledMemberId = memberId;
          }
          else
          {
            searchCriteria.BillingMemberId = memberId;
          }
        }
      }
      else if (SessionUtil.CGOCorrSearchCriteria != null && criteria != null)
      {
        correspondenceSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceSearchCriteria)) as CorrespondenceSearchCriteria;

        if (correspondenceSearchCriteria != null)
        {
          if (correspondenceSearchCriteria.CorrBilledMemberText == null) correspondenceSearchCriteria.CorrBilledMemberId = 0;
          correspondenceSearchCriteria.CorrBillingMemberId = memberId;
          if (correspondenceSearchCriteria.FromDate != null)
          {
            correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.FromDate.Value.ToLocalTime();
          }
          if (correspondenceSearchCriteria.ToDate != null)
          {
            correspondenceSearchCriteria.ToDate = correspondenceSearchCriteria.ToDate.Value.ToLocalTime();
          }
        }
      }

      IQueryable<CargoBillingHistorySearchResult> invoiceSearchedData;

      if (searchCriteria != null)
      {
        invoiceSearchedData = _cargoInvoiceManager.GetBillingHistorySearchResult(searchCriteria, correspondenceSearchCriteria);
      }
      else if (correspondenceSearchCriteria != null)
      {
        invoiceSearchedData = _cargoInvoiceManager.GetBillingHistoryCorrSearchResult(correspondenceSearchCriteria);
      }
      else
      {
        invoiceSearchedData = null;
      }

      return billingHistorySearchResultGrid.DataBind(invoiceSearchedData);
    }

    #region CMP612: Changes to PAX CGO Correspondence Audit Trail Download.

    /// <summary>
    /// This function is used to show linked correspondence rejection memo.
    /// </summary>
    /// <param name="corrReferenceNo"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download.
    public ActionResult ShowLinkedRejectionMemo(string correspondenceId)
    {
      //Create rejection memo search grid instance.
      var rejectionSearchResultGrid = new RejectionMemoLinkedCorrespondenceGrid(ControlIdConstants.LinkedRejectionMemoGridId,
                                                                      Url.Action(RejectionMemoSearchResultGridAction,
                                                                                 new
                                                                                 {
                                                                                   correspondenceId
                                                                                 }));
      ViewData[ViewDataConstants.LinkedRejectionMemoGridId] = rejectionSearchResultGrid.Instance;

      return PartialView("LinkedCorrRejectionMemo", rejectionSearchResultGrid.Instance);
    }

    /// <summary>
    /// Fetch rejection memo linked with correspondence and display it in grid.
    /// </summary>
    /// <param name="corrReferenceNo"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    public JsonResult RejectionMemoSearchResultGridData(string correspondenceId)
    {
      // Create grid instance and retrieve data from database
      var rejectionSearchResultGrid = new RejectionMemoLinkedCorrespondenceGrid(ControlIdConstants.LinkedRejectionMemoGridId,
                                                                     Url.Action(RejectionMemoSearchResultGridAction,
                                                                                new
                                                                                {
                                                                                  correspondenceId
                                                                                }));
      Logger.Info("Fetching rejection memo linked with given correspondence from database based on criteria.");

      //Fetch data from database based on search criteria.
      IQueryable<Iata.IS.Model.Cargo.CgoLinkedCorrRejectionSearchData> rejectionSearchData = null;
      if (!String.IsNullOrEmpty(correspondenceId))
        rejectionSearchData = _cargoInvoiceManager.GetLinkedCorrRejectionSearchResult(correspondenceId.ToGuid()).AsQueryable();

      return rejectionSearchResultGrid.DataBind(rejectionSearchData);
    }

    /// <summary>
    /// This function is used to generate audit trail for rejection memo which linked with correspondence.
    /// </summary>
    /// <param name="rejectionMemoIds"></param>
    /// <returns></returns>
    //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
    public JsonResult GenerateAuditTrailLinkedCorrRMs(string rejectionMemoIds)
    {
      try
      {
          // SCP450430: Exception occurred in Report Download Service. - SIS Production
          if (string.IsNullOrWhiteSpace(rejectionMemoIds))
          {
              Logger.Info("Cargo rejctionMemoIds are blank. Record will not be inserted in queue for report generation service.");
              return Json(new UIMessageDetail { Message = ControlIdConstants.ErrorMesageForRMAuditTrail, IsFailed = true });
          }

         //SCP351773 - SRM: Exception occurred in Report Download Service. - SIS Production - 23MAR2015
        if (SessionUtil.UserId > 0)
        {
          string fileName = "CGO-RM Audit Trail-" + SessionUtil.UserId + DateTime.Now.ToString("-yyyyMMdd-HHmmss");
          rejectionMemoIds = rejectionMemoIds + ";" + fileName;

          //Create object for rejection memo audit trail.
          var rejectionMemoReportRequestMessage = new ReportDownloadRequestMessage
          {
            OfflineReportType = OfflineReportType.LinkedCorrespondenceRMAuditTrail,
            RecordId = Guid.NewGuid(),
            BillingCategoryType = BillingCategoryType.Cgo,
            RequestingMemberId = SessionUtil.MemberId,
            UserId = SessionUtil.UserId,
            InputData = rejectionMemoIds,
            DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                      "Invoice",
                                                                      new
                                                                      {
                                                                        area = "Cargo",
                                                                        billingType = "Receivables"
                                                                      }))
          };

          Logger.InfoFormat("Request for rejection memo audit trail. file: {0}", fileName);

          //CMP612: Changes to PAX CGO Correspondence Audit Trail Download.
          //Message will display on screen depending on Success or Failure of En-queuing message to queue.
          var isEnqueSuccess = false;
          isEnqueSuccess = _referenceManager.EnqueTransactionTrailReport(rejectionMemoReportRequestMessage);
          return Json(isEnqueSuccess ? new UIMessageDetail { Message = String.Format(ControlIdConstants.SucessMesageForRMAuditTrail, fileName), IsFailed = false } :
            new UIMessageDetail { Message = ControlIdConstants.ErrorMesageForRMAuditTrail, IsFailed = true });
        }
        else
        {
          return Json(new UIMessageDetail { Message = ControlIdConstants.ErrorMesageForRMAuditTrail, IsFailed = true });
        }
      }
      catch (Exception)
      {
        return Json(new UIMessageDetail { Message = ControlIdConstants.ErrorMesageForRMAuditTrail, IsFailed = true });
      }
    }

    #endregion;

    [HttpPost]
    public JsonResult InitiateRejection(string rejectedRecordIds, string invoiceId, int billingYear, int billingMonth, int billingPeriod, int smi, int rejectionTransactionType)
    {
      // check if transactions have been rejected in some rejection memo.
      
      var transactions = _cargoInvoiceManager.GetRejectedTransactionDetails(rejectedRecordIds);
      //CMP#624 : 2.10 - Change#6 : Time Limits
      // While calculating time limit for SMI X it should behave like SMI I.
      transactions.IsTransactionOutsideTimeLimit = !_referenceManager.IsTransactionInTimeLimitMethodH((TransactionType)rejectionTransactionType, smi, billingYear, billingMonth, billingPeriod);

      // For display of warning message for - 1. Coupon/memo already rejected, 2. Rejection outside time limit.
      if ((transactions.Transactions != null && transactions.Transactions.Count > 0) || transactions.IsTransactionOutsideTimeLimit)
      {
        return Json(transactions);
      }

      return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
    }

    public JsonResult InitiateDuplicateRejection(string rejectedRecordIds, string invoiceId)
    {
      return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
    } 

    private JsonResult GetInvoicesForBillingHistory(string rejectedRecordIds, string invoiceId)
    {
      var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      
      var billedMemberId = invoice.BillingMemberId;
      
      const int billingCode = 0; // Search for invoice. For credit-note, billing code is 4.
      var invoices = _cargoInvoiceManager.GetInvoicesForBillingHistory(billingCode, billedMemberId, SessionUtil.MemberId, invoice.SettlementMethodId);

      TempData[TempDataConstants.RejectedRecordIds] = string.Format("{0}@{1}", rejectedRecordIds, invoiceId);
      TempData[TempDataConstants.CorrespondenceNumber] = null;
      /* CMP #624: ICH Rewrite-New SMI X
        * Description: Preserve invoice SMI as it will be useful on later stages for invoice header validation on server side 
        * Refer FRS Section 2.14 Change #9 */
      TempData[TempDataConstants.PreviousInvoiceSMI] = invoice.SettlementMethodId.ToString();

      var result = new JsonResult();

      // If single open invoice found, redirect to rejection memo create page of that invoice.
      if (invoices.Count() == 1)
      {
        string redirectUrl = Url.Action("RMCreate", "Invoice", new { invoiceId = invoices.First().Id.Value(), area = "Cargo" });
         

        IList<CargoAuditTrailInvoice> invoiceList = new List<CargoAuditTrailInvoice> {
                                                   new CargoAuditTrailInvoice {
                                                       RedirectUrl = redirectUrl
                                                     }
                                                 };

        result.Data = invoiceList.ToList();

        return result;
      }

      if (invoices.Count() > 1)
      {
        IList<CargoAuditTrailInvoice> invoiceList = invoices.Select(paxInvoice => new CargoAuditTrailInvoice
        {
          Id = paxInvoice.Id,
          InvoiceNumber = paxInvoice.InvoiceNumber
        }).ToList();
        result.Data = invoiceList.ToList();

        return result;
      }

      var url = Url.Action("Create", "Invoice") + "?FromBillingHistory=true";

      result.Data = new UIMessageDetail
      {
        Message = "Success",
        isRedirect = true,
        RedirectUrl = url
      };
      return result;
    }

    [HttpPost]
    public JsonResult IsBillingMemoExistsForCorrespondence(string correspondenceRefNumber)
    {
      // check if transactions have been rejected in some rejection memo.
      var billingMemos = _cargoInvoiceManager.GetBillingMemosForCorrespondence(Convert.ToInt64(correspondenceRefNumber), SessionUtil.MemberId);
      if (billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
      {
        return Json(billingMemos);
      }

      return GetInvoicesForBillingMemo(correspondenceRefNumber);
    }

    private JsonResult GetInvoicesForBillingMemo(string corrReferenceNumber)
    {
      long refNo;
      long.TryParse(corrReferenceNumber, out refNo);

      var correspondenceDetails = _cargoCorrespondenceManager.GetOriginalCorrespondenceDetails(refNo);

      var rejectionInvoice = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(correspondenceDetails.Id.ToString());
      var billedMemberId = rejectionInvoice.BillingMemberId;

      var invoices = _cargoInvoiceManager.GetInvoicesForBillingHistory((int)BillingCode.NonSampling, billedMemberId, SessionUtil.MemberId, rejectionInvoice.SettlementMethodId);

      TempData[TempDataConstants.CorrespondenceNumber] = corrReferenceNumber;
      TempData[TempDataConstants.RejectedRecordIds] = null;
      /* CMP #624: ICH Rewrite-New SMI X
        * Description: Preserve invoice SMI as it will be useful on later stages for invoice header validation on server side 
        * Refer FRS Section 2.14 Change #9 */
      TempData[TempDataConstants.PreviousInvoiceSMI] = rejectionInvoice.SettlementMethodId.ToString();

      var result = new JsonResult();

      if (invoices.Count() == 1)
      {
        IList<AuditTrailInvoice> invoiceList = new List<AuditTrailInvoice> {
                                                         new AuditTrailInvoice { RedirectUrl = Url.Action("BMCreate", "Invoice", new { invoiceId = invoices.First().Id })}
                                                     };
        result.Data = invoiceList.ToList();

        return result;
      }

      if (invoices.Count() > 1)
      {
        IList<AuditTrailInvoice> invoiceList = invoices.Select(paxInvoice => new AuditTrailInvoice {
          Id = paxInvoice.Id,
          InvoiceNumber = paxInvoice.InvoiceNumber
        }).ToList();
        result.Data = invoiceList.ToList();

        return result;
      }

      result.Data = new UIMessageDetail
      {
        Message = "Success",
        isRedirect = true,
        RedirectUrl = Url.Action("Create", "Invoice") + "?FromBillingHistory=true"
      };

      return result;
    }

    //SCP312528 - IS-Web Performance (Controller: BillingHistory - Log Action: CargoBillingHistoryAuditTrail)
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    public ActionResult CargoBillingHistoryAuditTrail(string transactionId, string transactionType)
    {
      var auditTrail = _cargoInvoiceManager.GetBillingHistoryAuditTrail(transactionId, transactionType);
      ViewData["TransactionId"] = transactionId;
      ViewData["TransactionType"] = transactionType;
      return View(auditTrail);
    }

    /// <summary>
    /// Following action generates BillingHistory Audit trail PDF file
    /// </summary>
    /// <param name="transactionId">Selected Transaction</param>
    /// <param name="transactionType">Selected Transaction type</param>
    public ActionResult GenerateBillingHistoryAuditTrailPdfForCgo(string transactionId, string transactionType)
    {
      try
      {
        // Retrieve auditTrail details for selected transaction
        var auditTrail = _cargoInvoiceManager.GetBillingHistoryAuditTrail(transactionId, transactionType);
        // Generate Audit trail html string through NVelocity 
        string htmlString = _cargoInvoiceManager.GenerateCargoBillingHistoryAuditTrailPdf(auditTrail);

        var filePath = Server.MapPath(@"\AuditTrailPdf");
        if (!System.IO.Directory.Exists(filePath))
        {
          System.IO.Directory.CreateDirectory(filePath);
        }

        string fileLocation = filePath + "\\CargoAuditTrail.pdf";

        // Following call will generate Audit trail pdf file from html string
        GenerateAuditTrailPdfFromHtmlString(htmlString, fileLocation);

        const string contentType = "application/pdf";
        // If pdf file is not null return file
        return File(fileLocation, contentType, System.IO.Path.GetFileName(fileLocation));
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        return null;
      }
    }

    /// <summary>
    /// Following action generates AuditTrail.pdf file from AuditTrail.html string 
    /// </summary>
    /// <param name="auditTrailHtmlString">Audit trail html string</param>
    /// <returns>Generates Audit trail pdf file</returns>
    [HttpPost]
    public void GenerateAuditTrailPdfFromHtmlString(string auditTrailHtmlString, string fileLocation)
    {
      var guid = Guid.NewGuid().ToString();

      // wkhtmltopdf.exe file path which converts html string to pdf
      var htmlToPdfExePath = Request.PhysicalApplicationPath + @"bin\wkhtmltopdf.exe";
      // Following call converts html file to pdf 
      var file = _queryAndDownloadDetailsManager.ConvertHtmlToPdf(auditTrailHtmlString, string.Format(@"AuditTrail_{0}", guid), htmlToPdfExePath);
      // Write all file content
      System.IO.File.WriteAllBytes(fileLocation, file);
    }

    public JsonResult ClearSearch(string entity)
    {
      switch (entity)
      {
        case "#corrSearchCriteria":
          SessionUtil.CGOCorrSearchCriteria = null;
          break;
        case "#invoiceSearchCriteria":
          SessionUtil.CGOInvoiceSearchCriteria = null;
          break;
        default:
          SessionUtil.CGOCorrSearchCriteria = null;
          SessionUtil.CGOInvoiceSearchCriteria = null;
          break;
      }

      return Json(new UIMessageDetail {
        Message = "Criteria cleared"
      });
    }

    [HttpPost]
    public JsonResult IsBillingMemoInvoiceOutSideTimeLimit(string correspondenceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate)
    {
      if (!_cargoInvoiceManager.IsBillingMemoInvoiceOutSideTimeLimit(correspondenceId, authorityToBill, correspondenceStatusId, correspondenceDate))
      {
        return
          Json(new UIMessageDetail { IsFailed = false });
      }

      return
          Json(new UIMessageDetail { IsFailed = true, Message = Messages.BPAXNS_10766 });
    }
    
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetAwbAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryRMCouponAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetRejectionMemoAwbAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryRMAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetRejectionMemoAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryBMAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetBillingMemoAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryBMAwbAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetBillingMemoAwbAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryCMAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetCreditMemoAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryCMCouponAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoInvoiceManager.GetCreditMemoAwbAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryFormCAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _samplingFormCManager.GetSamplingFormCRecordAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryFormDEAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _samplingformDEManager.GetSamplingFormDAttachment(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    [HttpGet]
    public FileStreamResult BillingHistoryCorrAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoCorrespondenceManager.GetCorrespondenceAttachmentDetail(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    //CMP508:Audit Trail Download with Supporting Documents
    public JsonResult EnqueBillingHistoryAuditTrailDownload(string transactionId, string transactionType)
    {
        JsonResult result = new JsonResult();
        
        //SCP351773 - SRM: Exception occurred in Report Download Service. - SIS Production - 23MAR2015
        if (SessionUtil.UserId > 0)
        {
          DateTime utcTime = DateTime.UtcNow;
          string fileName = string.Format("CGO-Audit Trail-{0}-{1}-{2}", SessionUtil.UserId, utcTime.ToString("yyyyMMdd"), utcTime.ToString("HHMMss"));

          AuditTrailPackageRequest data = new AuditTrailPackageRequest()
          {
            FileName = fileName,
            TransactionId = transactionId,
            TransactionType = transactionType
          };

          ReportDownloadRequestMessage enqueMessage = new ReportDownloadRequestMessage();
          enqueMessage.RecordId = Guid.NewGuid();
          enqueMessage.BillingCategoryType = BillingCategoryType.Cgo;
          enqueMessage.UserId = SessionUtil.UserId;
          enqueMessage.RequestingMemberId = SessionUtil.MemberId;
          enqueMessage.InputData = ConvertUtil.SerializeXml(data, data.GetType());
          enqueMessage.DownloadUrl = GetUrl();
          enqueMessage.OfflineReportType = OfflineReportType.AuditTrailPackageDownload;

          // SCP227747: Cargo Invoice Data Download
          // Message will display on screen depending on Success or Failure of Enqueing message to queue.
          var isEnqueSuccess = false;
          isEnqueSuccess = _referenceManager.EnqueTransactionTrailReport(enqueMessage);

          if (isEnqueSuccess)
          {
            //Display success message to user
            result.Data = new UIMessageDetail
            {
              Message =
                  string.Format(
                      @"Generation of the audit trail package is in progress. You will be notified via 
                        email once it is ready for download. [File: {0}.zip]",
                      fileName),
              IsFailed = false
            };
          }
          else
          {
            //Display failure message to user
            result.Data = new UIMessageDetail
            {
              Message = "Failed to download the audit trail package, please try again!",
              IsFailed = true
            };
          }
        }
        else
        {//Display failure message to user
          result.Data = new UIMessageDetail
          {
            Message = "Failed to download the audit trail package, please try again!",
            IsFailed = true
          };
        }
        return result;
    }

    public string GetUrl()
    {
        return UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                     "Invoice",
                                                     new
                                                     {
                                                         area = "Cargo",
                                                         billingType = "Receivables"
                                                     }));
    }

  }
}
