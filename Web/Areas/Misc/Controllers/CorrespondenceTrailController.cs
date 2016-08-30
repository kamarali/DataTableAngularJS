using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Business.Common;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Misc.Controllers
{
  public class CorrespondenceTrailController : ISController
  {
    private const string CorrespondenceTrailSearchGridAction = "CorrespondenceTrailSearchGridData";
    private readonly IMiscUatpInvoiceManager _miscUatpInvoiceManager;
    private readonly IMiscCorrespondenceManager _miscCorrespondenceManager;
    //CMP508-Audit Trail Download with Supporting Documents
    private readonly IReferenceManager _referenceManager;
    private const string SucessMessage ="PDF generation for the selected correspondence(s) is in progress.You will be notified via email once it is ready for download.";
    private const string ErrorMessage = "Error.Plesae try again.";
    private const string NoDataMessage = "No correspondences found.";


    public CorrespondenceTrailController(IMiscUatpInvoiceManager miscUatpInvoiceManager, IMiscCorrespondenceManager miscCorrespondenceManager, IReferenceManager referenceManager)
    {
      _miscUatpInvoiceManager = miscUatpInvoiceManager;
      _miscCorrespondenceManager = miscCorrespondenceManager;
      //CMP508-Audit Trail Download with Supporting Documents
      _referenceManager = referenceManager;
    }

    // SCP#447047: Correspondences
    [ISAuthorize(Business.Security.Permissions.Menu.MiscDownloadCorrespondences)]
    public ActionResult Index(CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria)
    {
      string criteria = null;
      criteria = correspondenceTrailSearchCriteria != null ? new JavaScriptSerializer().Serialize(correspondenceTrailSearchCriteria) : string.Empty;
      SessionUtil.MiscCorrTrailSearchCriteria = criteria;

      ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
      ViewData["ClearIntegerFields"] = true;


      if (SessionUtil.MiscCorrTrailSearchCriteria != null)
      {
        criteria = SessionUtil.MiscCorrTrailSearchCriteria;
        correspondenceTrailSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceTrailSearchCriteria)) as CorrespondenceTrailSearchCriteria;
        if (correspondenceTrailSearchCriteria != null)
        {
          if (correspondenceTrailSearchCriteria.FromDate.HasValue == false)
          {
            correspondenceTrailSearchCriteria.FromDate = DateTime.UtcNow;
          }
          if (correspondenceTrailSearchCriteria.ToDate.HasValue == false)
          {
            correspondenceTrailSearchCriteria.ToDate = DateTime.UtcNow;
          }
        }

        ViewData[ViewDataConstants.CorrespondenceSearch] = "Correspondence";
      }



      if (SessionUtil.MiscCorrTrailSearchCriteria == null) correspondenceTrailSearchCriteria.FromDate = correspondenceTrailSearchCriteria.ToDate = DateTime.UtcNow;

      ViewData[ViewDataConstants.CorrespondenceTrailSearchCriteria] = correspondenceTrailSearchCriteria;

      var correspondenceTrailSearchGrid = new CorrespondenceTrailSearchGrid(ControlIdConstants.CorrespondenceTrailSearchGrid,
                                                                        Url.Action(CorrespondenceTrailSearchGridAction,
                                                                                   new
                                                                                   {
                                                                                     criteria
                                                                                   }));
      ViewData[ViewDataConstants.CorrespondenceTrailSearchResultGrid] = correspondenceTrailSearchGrid.Instance;

      return View();

    }


    public JsonResult CorrespondenceTrailSearchGridData(string criteria)
    {
      var memberId = SessionUtil.MemberId;
      CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria = null;

      var correspondenceTrailSearchGrid = new CorrespondenceTrailSearchGrid(ControlIdConstants.CorrespondenceTrailSearchGrid,
                                                                        Url.Action(CorrespondenceTrailSearchGridAction,
                                                                                   new
                                                                                   {
                                                                                     area = "Misc"
                                                                                   }));


      if (SessionUtil.MiscCorrTrailSearchCriteria != null && criteria != null)
      {
        correspondenceTrailSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceTrailSearchCriteria)) as CorrespondenceTrailSearchCriteria;

        if (correspondenceTrailSearchCriteria != null)
        {
          if (correspondenceTrailSearchCriteria.CorrBilledMemberText == null) correspondenceTrailSearchCriteria.CorrBilledMemberId = 0;
          correspondenceTrailSearchCriteria.CorrBillingMemberId = memberId;
          if (correspondenceTrailSearchCriteria.FromDate != null)
          {
            correspondenceTrailSearchCriteria.FromDate = correspondenceTrailSearchCriteria.FromDate.Value.ToLocalTime();
          }
          if (correspondenceTrailSearchCriteria.ToDate != null)
          {
            correspondenceTrailSearchCriteria.ToDate = correspondenceTrailSearchCriteria.ToDate.Value.ToLocalTime();
          }
        }
      }

      IQueryable<CorrespondenceTrailSearchResult> correspondenceTrailSearchResultData;


      if (correspondenceTrailSearchCriteria != null)
      {
        correspondenceTrailSearchResultData = _miscUatpInvoiceManager.GetCorrespondenceTrailSearchResult(correspondenceTrailSearchCriteria, (int)BillingCategoryType.Misc);
      }
      else
      {
        correspondenceTrailSearchResultData = null;
      }

      return correspondenceTrailSearchGrid.DataBind(correspondenceTrailSearchResultData);
    }


    public JsonResult ClearSearch(string entity)
    {
      SessionUtil.MiscCorrTrailSearchCriteria = null;
      return Json(new UIMessageDetail
      {
        Message = "Criteria cleared"
      });
    }

    [HttpPost]
    public JsonResult RequestForCorrespondenceTrailReport(string transactionIds)
    {
      if (transactionIds.Length > 0)
      {
        var correspondenceNumbersString = new List<string>(transactionIds.Split(','));
        var correspondenceNumbers = new List<long>();

        try
        {
          //SCP351773 - SRM: Exception occurred in Report Download Service. - SIS Production - 23MAR2015
          if (SessionUtil.UserId > 0)
          {
            foreach (string correspondenceNumberString in correspondenceNumbersString)
            {
              correspondenceNumbers.Add(Convert.ToInt64(correspondenceNumberString));

            }

            if (correspondenceNumbers.Count > 0)
            {
              var correspondenceTrailReportRequestMessage = new ReportDownloadRequestMessage();
              //CMP-508-Audit Trail Download with Supporting Documents-v1.0
              correspondenceTrailReportRequestMessage.OfflineReportType = OfflineReportType.CorrespondenceReportDownload;
              correspondenceTrailReportRequestMessage.RecordId = Guid.NewGuid();
              correspondenceTrailReportRequestMessage.BillingCategoryType = BillingCategoryType.Misc;
              correspondenceTrailReportRequestMessage.RequestingMemberId = SessionUtil.MemberId;
              correspondenceTrailReportRequestMessage.UserId = SessionUtil.UserId;
              correspondenceTrailReportRequestMessage.CorrespondenceNumbers = correspondenceNumbers;
              correspondenceTrailReportRequestMessage.InputData = string.Join(",",
                                                                                                correspondenceNumbers.ToArray());



              correspondenceTrailReportRequestMessage.DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                                        "MiscInvoice",
                                                                        new
                                                                        {
                                                                          area = "Misc",
                                                                          billingType = "Receivables"
                                                                        }));


              //CMP-508-Audit Trail Download with Supporting Documents
              // SCP227747: Cargo Invoice Data Download
              // Message will display on screen depending on Success or Failure of Enqueing message to queue.
              var isEnqueSuccess = false;
              isEnqueSuccess = _referenceManager.EnqueTransactionTrailReport(correspondenceTrailReportRequestMessage);
              return Json(isEnqueSuccess ? new UIMessageDetail { Message = SucessMessage, IsFailed = false } : new UIMessageDetail { Message = ErrorMessage, IsFailed = true });
            }
          }
          else
          {
            return Json(new UIMessageDetail { Message = ErrorMessage, IsFailed = true });
          }
        }
        catch (Exception)
        {
          return Json(new UIMessageDetail { Message = ErrorMessage,  IsFailed = true });
        }
      }
      return Json(new UIMessageDetail { Message = NoDataMessage, IsFailed = true });
    }

    [HttpPost]
    public JsonResult RequestForCorrespondenceTrailReportAll(int recordCount)
    {
      if (recordCount == 0)
      {
        return Json(new UIMessageDetail
        {
          Message = NoDataMessage,
          IsFailed = true
        });

      }

      var correspondenceNumbers = new List<long>();
      var correspondenceNumbersString = new List<string>();
      var criteria = SessionUtil.MiscCorrTrailSearchCriteria;
      CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria = null;
      List<CorrespondenceTrailSearchResult> correspondenceTrailSearchResultData;


      if (SessionUtil.MiscCorrTrailSearchCriteria != null)
      {
        correspondenceTrailSearchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CorrespondenceTrailSearchCriteria)) as CorrespondenceTrailSearchCriteria;

        if (correspondenceTrailSearchCriteria != null)
        {

          if (correspondenceTrailSearchCriteria.CorrBilledMemberText == null)
            correspondenceTrailSearchCriteria.CorrBilledMemberId = 0;
          correspondenceTrailSearchCriteria.CorrBillingMemberId = SessionUtil.MemberId;
          if (correspondenceTrailSearchCriteria.FromDate != null)
          {
            correspondenceTrailSearchCriteria.FromDate = correspondenceTrailSearchCriteria.FromDate.Value.ToLocalTime();
          }
          if (correspondenceTrailSearchCriteria.ToDate != null)
          {
            correspondenceTrailSearchCriteria.ToDate = correspondenceTrailSearchCriteria.ToDate.Value.ToLocalTime();
          }

          try
          {
            correspondenceTrailSearchResultData =
                _miscUatpInvoiceManager.GetCorrespondenceTrailSearchResult(correspondenceTrailSearchCriteria,(int)BillingCategoryType.Misc).ToList();

            foreach (var searchResult in correspondenceTrailSearchResultData)
            {
              correspondenceNumbersString.Add(searchResult.TransactionNumber);
              correspondenceNumbers.Add(Convert.ToInt64(searchResult.TransactionNumber));
            }


            if (correspondenceNumbers.Count > 0)
            {
              var correspondenceTrailReportRequestMessage = new ReportDownloadRequestMessage();
              //CMP-508-Audit Trail Download with Supporting Documents
			  correspondenceTrailReportRequestMessage.OfflineReportType = OfflineReportType.CorrespondenceReportDownload;
              correspondenceTrailReportRequestMessage.RecordId = Guid.NewGuid();
              correspondenceTrailReportRequestMessage.BillingCategoryType = BillingCategoryType.Misc;
              correspondenceTrailReportRequestMessage.RequestingMemberId = SessionUtil.MemberId;
              correspondenceTrailReportRequestMessage.UserId = SessionUtil.UserId;
              correspondenceTrailReportRequestMessage.CorrespondenceNumbers = correspondenceNumbers;
              correspondenceTrailReportRequestMessage.InputData = string.Join(",",
                                                                                                correspondenceNumbers
                                                                                                    .
                                                                                                    ToArray());
              correspondenceTrailReportRequestMessage.DownloadUrl =
                  UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
                                                       "MiscInvoice",
                                                       new
                                                       {
                                                         area = "Misc",
                                                         billingType = "Receivables"
                                                       }));
              //CMP-508-Audit Trail Download with Supporting Documents
              // SCP227747: Cargo Invoice Data Download
              // Message will display on screen depending on Success or Failure of Enqueing message to queue.
              var isEnqueSuccess = false;
              isEnqueSuccess = _referenceManager.EnqueTransactionTrailReport(correspondenceTrailReportRequestMessage);
              return Json(isEnqueSuccess ? new UIMessageDetail { Message = SucessMessage, IsFailed = false } : new UIMessageDetail { Message = ErrorMessage, IsFailed = true });
            }
          }
          catch (Exception exception)
          {
            return Json(new UIMessageDetail { Message = ErrorMessage, IsFailed = true });
          }
        }
      }
      return Json(new UIMessageDetail { Message = NoDataMessage, IsFailed = true });
    }
  }
}
