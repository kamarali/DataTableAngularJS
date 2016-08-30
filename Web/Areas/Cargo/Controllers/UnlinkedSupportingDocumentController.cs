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
using BillingType = Iata.IS.Web.Util.BillingType;

namespace Iata.IS.Web.Areas.Cargo.Controllers
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
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CorrectSupportingDocumentsLinkingErrors.Correct)]
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
                                                                                                   }),
                                                                                      BillingCategoryType.Cgo);

        ViewData[ViewDataConstants.supportingDocumentSearchGrid] = unlinkedSupportingDocumentSearchGrid.Instance;
        ViewData[ViewDataConstants.MismatchTransactionModel] = new SupportingDocumentRecord();
        ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

        // Set first day of previous period as submission date, if exists.
        try
        {
          //var previousPeriod = CalendarManager.GetLastClosedBillingPeriod();
          //objUnlinkedSupportingDocumentEx.SubmissionDate = previousPeriod.StartDate;
        }
        catch (ISCalendarDataNotFoundException) { }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }
      return View(objUnlinkedSupportingDocumentEx);
    }

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
                                                                                                     }),
                                                                                        BillingCategoryType.Cgo);

          ViewData[ViewDataConstants.supportingDocumentSearchGrid] = unlinkedSupportingDocumentSearchGrid.Instance;
          ViewData[ViewDataConstants.MismatchTransactionModel] = new SupportingDocumentRecord();
          ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;
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
                                                                                                 }),
                                                                                    BillingCategoryType.Cgo);

      //Get data from the database and bind to the grid
      IQueryable<UnlinkedSupportingDocumentEx> lisOfUnlinkedSupportingDocument =
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
                                                                   (int)BillingCategoryType.Cgo).ToArray().AsQueryable();
      //set data bind property of DataGrid 
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
                                                   int breakdownSerialNumber)
    {
      var recordSearchCriteria = new RecordSearchCriteria
      {
        BilledMemberId = billedMemberId,
        BillingMemberId = billingMemberId,
        ClearanceMonth = clearanceMonth,
        ClearancePeriod = clearancePeriod,
        BillingCategory =  billingCategory,
        InvoiceNumber = invoiceNumber,
        BatchNumber = batchNumber,
        SequenceNumber = sequenceNumber,
        BreakdownSerialNumber = breakdownSerialNumber
      };

      var lisOfMismatchedTransactionDocuments = _iSupportingDocumentManager.GetRecordListWithAttachments(recordSearchCriteria).ToArray().AsQueryable();

      //TODO
      if (lisOfMismatchedTransactionDocuments.Count() > 0)
      {
        SupportingDocumentRecord lisOfMismatchedTransactionDocument = lisOfMismatchedTransactionDocuments.ElementAt(0);
        return Json(lisOfMismatchedTransactionDocument);
      }
      return Json(null);
    }

    [HttpPost]
    public JsonResult GetSelectedUnlinkedSupportingDocumentDetails(string unlinkedDocumentId)
    {
      UnlinkedSupportingDocumentEx lisOfUnlinkedSupportingDocumentDetails = _iSupportingDocumentManager.GetSelectedUnlinkedSupportingDocumentDetails(unlinkedDocumentId.ToGuid());
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
        BillingCategoryId = (int)BillingCategoryType.Cgo,
        CouponBreakdownSerialNumber = breakdownSerialNumber,
        Id = id.ToGuid()
      };

      string returnFlag = _iSupportingDocumentManager.LinkDocument(unlinkedSupportingDocument, null);
      return Json(returnFlag);
    }

    [HttpPost]
    public JsonResult DeleteUnLinkDocuments(string id)
    {
      var unlinkedSupportingDocument = new UnlinkedSupportingDocument();
      unlinkedSupportingDocument.Id = id.ToGuid();

      bool returnFlag = _iSupportingDocumentManager.DeleteUnlinkedDocuments(unlinkedSupportingDocument);
      UIMessageDetail details = returnFlag
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