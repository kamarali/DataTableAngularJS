using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.UnlinkedSupportingDocument;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
  public class UnlinkedSupportingDocumentController : ISController
  {
    private const string SearchResultGridAction = "SearchResultGridData";
    private readonly ISupportingDocumentManager _iSupportingDocumentManager;
    public ICalendarManager CalendarManager { get; set; }

    public UnlinkedSupportingDocumentController(ISupportingDocumentManager iSupportingDocumentManager)
    {
      _iSupportingDocumentManager = iSupportingDocumentManager;
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Misc.Receivables.CorrectSupportingDocumentsLinkingErrors.Correct)]
    public ActionResult Index()
    {
      var objUnlinkedSupportingDocumentEx = new UnlinkedSupportingDocumentEx();
      try
      {
        var unlinkedSupportingDocumentSearchGrid = new UnlinkedSupportingDocumentGrid(ControlIdConstants.SupportingDocumentSearchGrid,
                                                                                      Url.Action(SearchResultGridAction,
                                                                                                 new
                                                                                                   {
                                                                                                     billingYear = 0,
                                                                                                     billingMonth = 0,
                                                                                                     billingPeriod = -1,
                                                                                                     billedmember = 0,
                                                                                                     billingMember = SessionUtil.MemberId,
                                                                                                     invoiceNumber = string.Empty,
                                                                                                     fileName = string.Empty
                                                                                                   }), BillingCategoryType.Misc);

        ViewData[ViewDataConstants.supportingDocumentSearchGrid] = unlinkedSupportingDocumentSearchGrid.Instance;
        ViewData[ViewDataConstants.MismatchTransactionModel] = new SupportingDocumentRecord();

        // Set first day of previous period as submission date, if exists.
        try
        {
          var previousPeriod = CalendarManager.GetLastClosedBillingPeriod();
          objUnlinkedSupportingDocumentEx.SubmissionDate = previousPeriod.StartDate;
        }
        catch (ISCalendarDataNotFoundException) { }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }
      return View(objUnlinkedSupportingDocumentEx);
    }

    /// <summary>
    /// Indexes the specified search criteria.
    /// </summary>
    /// <param name="searchCriteria">The search criteria.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    public ActionResult Index(UnlinkedSupportingDocumentEx searchCriteria)
    {
      try
      {
        if (searchCriteria != null)
        {
          var unlinkedSupportingDocumentSearchGrid = new UnlinkedSupportingDocumentGrid(ControlIdConstants.SupportingDocumentSearchGrid,
                                                                                        Url.Action(SearchResultGridAction,
                                                                                                   new
                                                                                                     {
                                                                                                       billingYear = searchCriteria.BillingYear,
                                                                                                       billingMonth = searchCriteria.BillingMonth,
                                                                                                       billingPeriod = searchCriteria.PeriodNumber,
                                                                                                       billedmember = searchCriteria.BilledMemberId,
                                                                                                       billingMember = SessionUtil.MemberId,
                                                                                                       invoiceNumber = searchCriteria.InvoiceNumber,
                                                                                                       fileName = searchCriteria.OriginalFileName,
                                                                                                       submissionDate = searchCriteria.SubmissionDate
                                                                                                     }), BillingCategoryType.Misc);

          ViewData[ViewDataConstants.supportingDocumentSearchGrid] = unlinkedSupportingDocumentSearchGrid.Instance;
          ViewData[ViewDataConstants.MismatchTransactionModel] = new SupportingDocumentRecord();
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      return View(searchCriteria);
    }

    public JsonResult SearchResultGridData(int billingYear, int billingMonth, int billingPeriod, int billedmember, int billingMember, string invoiceNumber, string fileName, DateTime? submissionDate)
    {
      //Create grid instance and retrieve data from database
      var unlinkedSupportingDocumentSearchGrid = new UnlinkedSupportingDocumentGrid(ControlIdConstants.SupportingDocumentSearchGrid,
                                                                                    Url.Action(SearchResultGridAction,
                                                                                               new
                                                                                                 {
                                                                                                   billingYear,
                                                                                                   billingMonth,
                                                                                                   billingPeriod,
                                                                                                   billedmember,
                                                                                                   billingMember,
                                                                                                   invoiceNumber,
                                                                                                   fileName,
                                                                                                   submissionDate
                                                                                                 }), BillingCategoryType.Misc);

      //Get data from the database and bind to the grid
      var lisOfUnlinkedSupportingDocument =
        _iSupportingDocumentManager.GetUnlinkedSupportingDocuments(billingYear,
                                                                   billingMonth,
                                                                   billingPeriod,
                                                                   billedmember,
                                                                   billingMember,
                                                                   invoiceNumber,
                                                                   fileName,
                                                                   submissionDate,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   (int)BillingCategoryType.Misc).ToArray().AsQueryable();
      //set databind property of DataGrid 
      return unlinkedSupportingDocumentSearchGrid.DataBind(lisOfUnlinkedSupportingDocument);
    }

    public JsonResult SearchResultMismatchGridData(int billedMemberId,
                                                   int billingMemberId,
                                                   int clearanceMonth,
                                                   int clearancePeriod,
                                                   int billingCategory,
                                                   string invoiceNumber,
                                                   int batchNumber,
                                                   int sequenceNumber,
                                                   int breakdownSerialNumber, int chargeCategoryId)
    {
      var recordSearchCriteria = new RecordSearchCriteria
      {
        BilledMemberId = billedMemberId,
        BillingMemberId = billingMemberId,
        ClearanceMonth = clearanceMonth,
        ClearancePeriod = clearancePeriod,
        BillingCategory = billingCategory,
        InvoiceNumber = invoiceNumber,
        BatchNumber = batchNumber,
        SequenceNumber = sequenceNumber,
        BreakdownSerialNumber = breakdownSerialNumber,
        ChargeCategoryId = chargeCategoryId > 0 ? (int?)chargeCategoryId : null
      };

      var lisOfMismatchedTransactionDocuments = _iSupportingDocumentManager.GetRecordListWithAttachments(recordSearchCriteria).ToArray().AsQueryable();

      //TODO
      if (lisOfMismatchedTransactionDocuments.Count() > 0)
      {
        var lisOfMismatchedTransactionDocument = lisOfMismatchedTransactionDocuments.ElementAt(0);
        return Json(lisOfMismatchedTransactionDocument);
      }
      return Json(null);
    }

    [HttpPost]
    public JsonResult GetSelectedUnlinkedSupportingDocumentDetails(string unlinkedDocumentId)
    {
      var lisOfUnlinkedSupportingDocumentDetails = _iSupportingDocumentManager.GetSelectedUnlinkedSupportingDocumentDetails(unlinkedDocumentId.ToGuid());
      return Json(lisOfUnlinkedSupportingDocumentDetails);
    }

    [HttpPost]
    public JsonResult LinkDocuments(string invoiceNumber,
                                    int billingMemberId,
                                    int billedMemberId,
                                    int billingYear,
                                    int billingMonth,
                                    int periodNumber,
                                    int batchNumber,
                                    int sequenceNumber,
                                    int breakdownSerialNumber,
                                    string filePath,
                                    string originalFileName,
                                    string id)
    {
      var unlinkedSupportingDocument = new UnlinkedSupportingDocument
                                         {
                                           InvoiceNumber = invoiceNumber,
                                           BillingMemberId = billingMemberId,
                                           BilledMemberId = billedMemberId,
                                           BillingYear = billingYear,
                                           BillingMonth = billingMonth,
                                           PeriodNumber = periodNumber,
                                           BatchNumber = batchNumber,
                                           SequenceNumber = sequenceNumber,
                                           FilePath = filePath,
                                           OriginalFileName = originalFileName,
                                           BillingCategoryId = (int)BillingCategoryType.Misc,
                                           CouponBreakdownSerialNumber = breakdownSerialNumber,
                                           Id = id.ToGuid()
                                         };

      var returnFlag = _iSupportingDocumentManager.LinkDocument(unlinkedSupportingDocument, null);
      return Json(returnFlag);
    }

    /// <summary>
    /// Deletes the unlink documents.
    /// </summary>
    /// <param name="id">The id.</param>
    /// <returns></returns>
    [HttpPost]
    public JsonResult DeleteUnLinkDocuments(string id)
    {
      var unlinkedSupportingDocument = new UnlinkedSupportingDocument {
                                                                        Id = id.ToGuid()
                                                                      };

      var returnFlag = _iSupportingDocumentManager.DeleteUnlinkedDocuments(unlinkedSupportingDocument);
      var details = returnFlag
                                  ? new UIMessageDetail
                                      {
                                        IsFailed = false,
                                        Message = "Unlinked document deleted sucessfully."
                                      }
                                  : new UIMessageDetail
                                      {
                                        IsFailed = true,
                                        Message = "Error while deleting Unlinked document."
                                      };
      return Json(details);
    }
  }
}