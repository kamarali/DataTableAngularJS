using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Enums = Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.BillingHistory.Pax;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.Enums;
using Enyim.Caching;
using System.Reflection;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class BillingHistoryController : ISController
  {
    private const string BillingHistorySearchGridAction = "BillingHistorySearchGridData";
    private const string RejectionMemoSearchResultGridAction = "RejectionMemoSearchResultGridData";
    private const string PaxBillingHistory = "PaxBillingHistory";
    private readonly IInvoiceManager _invoiceManager;
    private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
    private readonly IPaxCorrespondenceManager _paxCorrespondenceManager;
    private readonly INonSamplingCreditNoteManager _nonSamplingCreditNoteManager;
    private readonly ISamplingFormCManager _samplingFormCManager;
    private readonly ISamplingFormDEManager _samplingformDEManager;
    public ISamplingFormCManager FormCManager { get; set; }
    private readonly IQueryAndDownloadDetailsManager _queryAndDownloadDetailsManager;
    private readonly IReferenceManager _referenceManager;
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    public BillingHistoryController(IInvoiceManager invoiceManager, IPaxCorrespondenceManager paxCorrespondenceManager, INonSamplingInvoiceManager nonSamplingInvoiceManager, INonSamplingCreditNoteManager nonSamplingCreditNoteManager, ISamplingFormCManager samplingFormCManager, ISamplingFormDEManager samplingFormDEManager, IQueryAndDownloadDetailsManager queryAndDownloadDetailsManager, IReferenceManager referenceManager)
    {
      _invoiceManager = invoiceManager;
      _paxCorrespondenceManager = paxCorrespondenceManager;
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
      _nonSamplingCreditNoteManager = nonSamplingCreditNoteManager;
      _samplingFormCManager = samplingFormCManager;
      _samplingformDEManager = samplingFormDEManager;
      _queryAndDownloadDetailsManager = queryAndDownloadDetailsManager;
      _referenceManager = referenceManager;
    }

    // GET: /Pax/BillingHistory/
   [ISAuthorize(Business.Security.Permissions.Pax.BillingHistoryAndCorrespondence.ViewAuditTrail)]
    public ActionResult Index(InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria, string searchType)
    {
      string criteria = null;
      SessionUtil.SearchType = PaxBillingHistory;

      switch (searchType)
      {
        case "Invoice":
          criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;
          SessionUtil.PaxCorrSearchCriteria = null;
          SessionUtil.PaxInvoiceSearchCriteria = criteria;
          SessionUtil.FormCSearchCriteria = null;
          SessionUtil.InvoiceSearchCriteria = null;
          //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
          SessionUtil.PaxInvoiceBillingHistoryCount = 0;
          ViewData[ViewDataConstants.CorrespondenceSearch] = "Invoice";
          break;
        case "Correspondence":
          criteria = correspondenceSearchCriteria != null ? new JavaScriptSerializer().Serialize(correspondenceSearchCriteria) : string.Empty;
          SessionUtil.PaxCorrSearchCriteria = criteria;
          SessionUtil.PaxInvoiceSearchCriteria = null;
          SessionUtil.FormCSearchCriteria = null;
          SessionUtil.InvoiceSearchCriteria = null;
          ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
          //CMP612: Changes to PAX CGO Correspondence Audit Trail Download.
          ShowLinkedRejectionMemo(null);
          break;
        default:
          // Criteria values should not be picked from the Session when user comes from the menu.
          if (Request.QueryString["back"] == null)
          {
            // clear the session.
            SessionUtil.PaxCorrSearchCriteria = null;
            SessionUtil.PaxInvoiceSearchCriteria = null;
            SessionUtil.PaxInvoiceBillingHistoryCount = 0;
          } // Values should come from Session only when user has clicked the Back to Billing history button.
          else
          {
            if (SessionUtil.PaxCorrSearchCriteria != null)
            {
              criteria = SessionUtil.PaxCorrSearchCriteria;
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
            else if (SessionUtil.PaxInvoiceSearchCriteria != null)
            {
              criteria = SessionUtil.PaxInvoiceSearchCriteria;
              searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(InvoiceSearchCriteria)) as InvoiceSearchCriteria;
              ViewData[ViewDataConstants.CorrespondenceSearch] = "Invoice";
            }
          }
          break;
      }

      if (SessionUtil.PaxCorrSearchCriteria == null) correspondenceSearchCriteria.FromDate = correspondenceSearchCriteria.ToDate = DateTime.UtcNow;

      ViewData[ViewDataConstants.invoiceSearchCriteria] = searchCriteria;
      ViewData[ViewDataConstants.correspondenceSearchCriteria] = correspondenceSearchCriteria;

      var billingHistorySearchResultGrid = new BillingHistorySearchGrid(ControlIdConstants.BHSearchResultsGrid,
                                                                        Url.Action(BillingHistorySearchGridAction,
                                                                                   new
                                                                                   {
                                                                                     criteria
                                                                                   }));
     ViewData[ViewDataConstants.BHSearchResultsGrid] = billingHistorySearchResultGrid.Instance;

      return View();
    }

    public JsonResult BillingHistorySearchGridData(string criteria, string sidx, string sord, int page, int rows)
    {
      // Retrieve MemberId from Session variable and use it across the method
      var memberId = SessionUtil.MemberId;
      InvoiceSearchCriteria searchCriteria = null;
      CorrespondenceSearchCriteria correspondenceSearchCriteria = null;

      var billingHistorySearchResultGrid = new BillingHistorySearchGrid(ControlIdConstants.BHSearchResultsGrid,
                                                                        Url.Action(BillingHistorySearchGridAction,
                                                                                   new
                                                                                   {
                                                                                     area = "Pax"
                                                                                   }));

      if (SessionUtil.PaxInvoiceSearchCriteria != null && criteria != null)
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
      else if (SessionUtil.PaxCorrSearchCriteria != null && criteria != null)
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

      IQueryable<PaxBillingHistorySearchResult> invoiceSearchedData;
      
      //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
      int TotalCountRequired = 1;
      int? totalRecords = 0;
      //check the session variable

      if (searchCriteria != null)
      {              
          //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //Passed new parameter totalCountRequired as 1/0
          invoiceSearchedData = _invoiceManager.GetBillingHistorySearchResult(searchCriteria, correspondenceSearchCriteria, page, rows, sidx, sord, TotalCountRequired);

          #region Billing History Grid Pagination
          
              totalRecords = invoiceSearchedData.Select(x => x.TotalRows).FirstOrDefault();
              //Set the  session variable 
              SessionUtil.PaxInvoiceBillingHistoryCount = totalRecords.Value;
          
          return GetBillingHistoryGridPagination(page, rows, invoiceSearchedData, totalRecords);

          #endregion
      }
      else if (correspondenceSearchCriteria != null)
      {
        invoiceSearchedData = _invoiceManager.GetBillingHistoryCorrSearchResult(correspondenceSearchCriteria, page, rows, sidx, sord, TotalCountRequired);

        #region Billing History Grid Pagination
        
          totalRecords = invoiceSearchedData.Select(x => x.TotalRows).FirstOrDefault();
          //Set the  session variable 
          SessionUtil.PaxInvoiceBillingHistoryCount = totalRecords.Value;
        
        return GetBillingHistoryGridPagination(page, rows, invoiceSearchedData, totalRecords);

        #endregion
      }
      else
      {
        invoiceSearchedData = null;
      }

      return billingHistorySearchResultGrid.DataBind(invoiceSearchedData);
    }

    #region CMP612: Changes to PAX CGO Correspondence Audit Trail Download.

    /// <summary>
    /// This function is used for display rejection memo with linked correspondence.
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
      IQueryable<Iata.IS.Model.Pax.PaxLinkedCorrRejectionSearchData> rejectionSearchData = null;
      if (!String.IsNullOrEmpty(correspondenceId))
         rejectionSearchData = _invoiceManager.GetLinkedCorrRejectionSearchResult(correspondenceId.ToGuid()).AsQueryable();

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
       //string fileName = "PAX-RM Audit Trail-" + SessionUtil.UserId + DateTime.Now.ToString("-yyyyMMdd-HHmmss");
       // rejectionMemoIds = rejectionMemoIds + ";" + fileName;

       // _invoiceManager.CreateRejectionAuditTrailPdf1(rejectionMemoIds, "D:\\temp");
       // return new JsonResult();
      try
      {
          // SCP450430: Exception occurred in Report Download Service. - SIS Production
          if (string.IsNullOrWhiteSpace(rejectionMemoIds))
          {
              Logger.Info("PAX rejctionMemoIds are blank. Record will not be inserted in queue for report generation service.");
              return Json(new UIMessageDetail { Message = ControlIdConstants.ErrorMesageForRMAuditTrail, IsFailed = true });
          }
         
         //SCP351773 - SRM: Exception occurred in Report Download Service. - SIS Production - 23MAR2015
        if (SessionUtil.UserId > 0)
        {
          string fileName = "PAX-RM Audit Trail-" + SessionUtil.UserId + DateTime.Now.ToString("-yyyyMMdd-HHmmss");
          rejectionMemoIds = rejectionMemoIds + ";" + fileName;

          //Create object for rejection memo audit trail.
          var rejectionMemoReportRequestMessage = new ReportDownloadRequestMessage
          {
            OfflineReportType = OfflineReportType.LinkedCorrespondenceRMAuditTrail,
            RecordId = Guid.NewGuid(),
            BillingCategoryType = BillingCategoryType.Pax,
            RequestingMemberId = SessionUtil.MemberId,
            UserId = SessionUtil.UserId,
            InputData = rejectionMemoIds,
            DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                      "Invoice",
                                                                      new
                                                                      {
                                                                        area = "Pax",
                                                                        billingType = "Receivables"
                                                                      }))
          };

          Logger.InfoFormat("Request for rejection memo audit trail. file: {0}", fileName);

          //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
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

    private JsonResult GetBillingHistoryGridPagination(int page, int rows, IQueryable<PaxBillingHistorySearchResult> invoiceSearchedData, int? totalRecords)
    {
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

      return Json(jsonData, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    public JsonResult InitiateRejection(string rejectedRecordIds, string invoiceId, int billingYear, int billingMonth, int billingPeriod, int smi, int rejectionTransactionType)
    {
        //CMP#641: Time Limit Validation on Third Stage PAX Rejections
        if(rejectionTransactionType == (int)TransactionType.RejectionMemo3 || rejectionTransactionType == (int)TransactionType.SamplingFormXF )
        {
            var transactionType = (rejectionTransactionType == (int) TransactionType.SamplingFormXF) ? TransactionType.SamplingFormXF : TransactionType.RejectionMemo3;
            var yourInvoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
            IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
            _invoiceManager.ValidatePaxStageThreeRmForTimeLimit(transactionType, smi, null, yourInvoice, exceptionDetailsList: exceptionDetailsList, isBillingHistory: true);

            if (exceptionDetailsList.Count > 0)
            {
                var result = new JsonResult();
                //Display failure message to user
                result.Data = new UIMessageDetail
                {
                    Message = exceptionDetailsList.First().ErrorDescription,
                    ErrorCode = "BPAXNS_10969",
                    IsFailed = true
                };
                return result;
            }
            
        }

      // check if transactions have been rejected in some rejection memo.
      var transactions = _invoiceManager.GetRejectedTransactionDetails(rejectedRecordIds);
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
      var invoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);

      var billingCode = invoice.BillingCode;
      if (billingCode > (int)BillingCode.NonSampling) ++billingCode;
      var billedMemberId = invoice.BillingMemberId;
      var billingMemberId = invoice.BilledMemberId;
      var settlementMethodId = invoice.SettlementMethodId;

        var invoices = billingCode != (int) BillingCode.NonSampling
                           ? _invoiceManager.GetInvoicesForSamplingBillingHistory(billingCode, billedMemberId,
                                                                                  billingMemberId,
                                                                                  invoice.ProvisionalBillingMonth,
                                                                                  invoice.ProvisionalBillingYear,
                                                                                  settlementMethodId)
                           : _invoiceManager.GetInvoicesForBillingHistory(billingCode, billedMemberId,
                                                                          SessionUtil.MemberId, settlementMethodId);

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
        string redirectUrl = string.Empty;
        switch (invoice.BillingCode)
        {
          case (int)BillingCode.NonSampling:
            redirectUrl = Url.Action("RMCreate", "Invoice", new { invoiceId = invoices.First().Id.Value() });
            break;
          case (int)BillingCode.SamplingFormDE:
            redirectUrl = Url.Action("RMCreate", "FormF", new { invoiceId = invoices.First().Id.Value() });
            break;
          case (int)BillingCode.SamplingFormF:
            redirectUrl = Url.Action("RMCreate", "FormXF", new { invoiceId = invoices.First().Id.Value() });
            break;
        }

        IList<AuditTrailInvoice> invoiceList = new List<AuditTrailInvoice> {
                                                   new AuditTrailInvoice {
                                                       RedirectUrl = redirectUrl
                                                     }
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

      var url = string.Empty;

      switch (invoice.BillingCode)
      {
        case (int)BillingCode.NonSampling:
          url = Url.Action("Create", "Invoice") + "?FromBillingHistory=true";
          break;
        case (int)BillingCode.SamplingFormDE:
          url = Url.Action("Create", "FormF") + "?FromBillingHistory=true";
          break;
        case (int)BillingCode.SamplingFormF:
          url = Url.Action("Create", "FormXF") + "?FromBillingHistory=true";
          break;
      }

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
      var billingMemos = _nonSamplingInvoiceManager.GetBillingMemosForCorrespondence(Convert.ToInt64(correspondenceRefNumber), SessionUtil.MemberId);
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

      var correspondenceDetails = _paxCorrespondenceManager.GetOriginalCorrespondenceDetails(refNo);

      var rejectionInvoice = _paxCorrespondenceManager.GetCorrespondenceRelatedInvoice(correspondenceDetails.Id.ToString());
      var billedMemberId = rejectionInvoice.BillingMemberId;

      var invoices = _invoiceManager.GetInvoicesForBillingHistory((int)BillingCode.NonSampling, billedMemberId, SessionUtil.MemberId, rejectionInvoice.SettlementMethodId);

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

    public ActionResult PaxBillingHistoryAuditTrail(string transactionId, string transactionType)
    {
      var auditTrail = _invoiceManager.GetbillingHistoryAuditTrail(transactionId, transactionType);
      ViewData["TransactionId"] = transactionId;
      ViewData["TransactionType"] = transactionType;
      return View(auditTrail);
    }

    /// <summary>
    /// Following action generates BillingHistory Audit trail PDF file
    /// </summary>
    /// <param name="transactionId">Selected Transaction</param>
    /// <param name="transactionType">Selected type Transaction</param>
    public ActionResult GenerateBillingHistoryAuditTrailPdfForPax(string transactionId, string transactionType)
    {
      // Retrieve auditTrail details for selected transaction
      var auditTrail = _invoiceManager.GetbillingHistoryAuditTrail(transactionId, transactionType);
      // Generate Audit trail html string through NVelocity 
      string htmlString = _nonSamplingInvoiceManager.GeneratePaxBillingHistoryAuditTrailPdf(auditTrail);

      var filePath = Server.MapPath(@"\AuditTrailPdf");
      if (!System.IO.Directory.Exists(filePath))
      {
        System.IO.Directory.CreateDirectory(filePath);
      }

      string fileLocation = filePath + "\\PaxAuditTrail.pdf";

      // Following call will generate Audit trail pdf file from html string
      GenerateAuditTrailPdfFromHtmlString(htmlString, fileLocation);

      const string contentType = "application/pdf";
      // If pdf file is not null return file
      return File(fileLocation, contentType, System.IO.Path.GetFileName(fileLocation));
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
          SessionUtil.PaxCorrSearchCriteria = null;
          break;
        case "#invoiceSearchCriteria":
          SessionUtil.PaxInvoiceSearchCriteria = null;
          SessionUtil.PaxInvoiceBillingHistoryCount = 0;
          break;
        default:
          SessionUtil.PaxCorrSearchCriteria = null;
          SessionUtil.PaxInvoiceBillingHistoryCount = 0;

          break;
      }

      return Json(new UIMessageDetail {
        Message = "Criteria cleared"
      });
    }


    public ActionResult ViewFormCCoupon(string id)
    {
      var formCCoupon = FormCManager.GetSamplingFormCRecordDetails(id);
      var formCDetail = FormCManager.GetSamplingFormCDetails(formCCoupon.SamplingFormCId.ToString());
      
      return RedirectToAction("CouponView", "FormC", new
      {
        area = "Pax",
        transactionId = id,
        provisionalBillingMonth = formCDetail.ProvisionalBillingMonth,
        provisionalBillingYear = formCDetail.ProvisionalBillingYear,
        provisionalBillingMemberId = formCDetail.ProvisionalBillingMemberId,
        fromMemberId = formCDetail.FromMemberId,
        listingCurrencyId = formCDetail.ListingCurrencyId,
        invoiceStatusId = formCDetail.InvoiceStatusId
      });
    }



    [HttpPost]
    public JsonResult IsBillingMemoInvoiceOutSideTimeLimit(string correspondenceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate)
    {
      if (!_nonSamplingInvoiceManager.IsBillingMemoInvoiceOutSideTimeLimit(correspondenceId, authorityToBill, correspondenceStatusId, correspondenceDate))
      {
        return
          Json(new UIMessageDetail { IsFailed = false });
      }

      return
          Json(new UIMessageDetail { IsFailed = true, Message = Messages.BPAXNS_10766 });
    }

    [HttpGet]
    public FileStreamResult BillingHistoryAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingInvoiceManager.GetCouponLevelAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryRMCouponAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryRMAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingInvoiceManager.GetRejectionMemoAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryBMAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingInvoiceManager.GetBillingMemoAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryBMCouponAttachmentDownload(string invoiceId)
    {

      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryCMAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingCreditNoteManager.GetCreditMemoAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryCMCouponAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryFormCAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _samplingFormCManager.GetSamplingFormCRecordAttachmentDetails(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryFormDEAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _samplingformDEManager.GetSamplingFormDAttachment(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public FileStreamResult BillingHistoryCorrAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _paxCorrespondenceManager.GetCorrespondenceAttachmentDetail(invoiceId)
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
          string fileName = string.Format("PAX-Audit Trail-{0}-{1}-{2}", SessionUtil.UserId, utcTime.ToString("yyyyMMdd"), utcTime.ToString("HHMMss"));

          AuditTrailPackageRequest data = new AuditTrailPackageRequest()
                                              {
                                                FileName = fileName,
                                                TransactionId = transactionId,
                                                TransactionType = transactionType
                                              };

          ReportDownloadRequestMessage enqueMessage = new ReportDownloadRequestMessage();
          enqueMessage.RecordId = Guid.NewGuid();
          enqueMessage.BillingCategoryType = BillingCategoryType.Pax;
          enqueMessage.UserId = SessionUtil.UserId;
          enqueMessage.RequestingMemberId = SessionUtil.MemberId;
          enqueMessage.InputData = ConvertUtil.SerializeXml(data, data.GetType());
          enqueMessage.DownloadUrl = GetUrl();
          enqueMessage.OfflineReportType = OfflineReportType.AuditTrailPackageDownload;
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
        {
          //Display failure message to user
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
                                                         area = "Pax",
                                                         billingType = "Receivables"
                                                     }));
    }

  }
}
