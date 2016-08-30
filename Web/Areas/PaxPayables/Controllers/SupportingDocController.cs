using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Security.Permissions.Pax.Payables;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Business;
using Iata.IS.Model.Enums;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Business.Common;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Web.UIModel.Grid.Common;

namespace Iata.IS.Web.Areas.PaxPayables.Controllers
{
    public class SupportingDocController : ISController
    {
      private const string PayableSupportingDocSearchGridAction = "PayableSupportingDocSearchGridData";
        //
        // GET: /Pax/SupportingDoc/

      private ISupportingDocumentManager _supportingDocumentManager { get; set; }
      private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
      private readonly ISamplingFormCManager _samplingFormCManager;

      public SupportingDocController(ISupportingDocumentManager supportingDocumentManager, INonSamplingInvoiceManager nonSamplingInvoiceManager, ISamplingFormCManager samplingFormCManager)
      {
        _supportingDocumentManager = supportingDocumentManager;
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
      _samplingFormCManager = samplingFormCManager;
      }

      [ISAuthorize(ViewSupportingDocuments.Query)]
      public ActionResult PayableSupportingDocs()
      {
        SetViewDataBillingType(Util.BillingType.Payables);
        var searchCriteria = new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn) { SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote };
        searchCriteria.BilledMemberId = SessionUtil.MemberId;

        //Initialize search result grid
        var suppDocResultGrid = new PayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction, "SupportingDoc"));

        ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = suppDocResultGrid.Instance;


        var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "SupportingDoc",
                                                                                  new
                                                                                  {
                                                                                  }));
        ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;
        
        
        return View(searchCriteria);
      }

      [ValidateAntiForgeryToken]
      [HttpPost]
      [ISAuthorize(ViewSupportingDocuments.Query)]
      public ActionResult PayableSupportingDocs(SupportingDocSearchCriteria searchCriteria)
      {
        SetViewDataBillingType(Util.BillingType.Payables);
        searchCriteria.BilledMemberId = SessionUtil.MemberId;
        searchCriteria.CutOffDateEventName = CalendarConstants.SupportingDocumentsLinkingDeadlineColumn;
        var criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

        //Initialize search result grid
        var suppDocResultGrid = new PayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction,
                                                                                  new
                                                                                  {
                                                                                    criteria
                                                                                  }));

        ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = suppDocResultGrid.Instance;

        var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "SupportingDoc",
                                                                                  new
                                                                                  {
                                                                                  }));
        ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;
        

        return View(searchCriteria);
      }

      /// <summary>
      /// Fetch data for supporting document search result
      /// </summary>
      /// <param name="criteria"></param>
      /// <returns></returns>
      public JsonResult PayableSupportingDocSearchGridData(string criteria)
      {
        var suppDocResultGrid = new PayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction,
                                                                                  new
                                                                                  {
                                                                                    criteria
                                                                                  }));
        if (!string.IsNullOrEmpty(criteria))
        {
          var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
          var searchResult = _supportingDocumentManager.GetPayableSupportingDocumentSearchResult(searchCriteria).AsQueryable();
          return suppDocResultGrid.DataBind(searchResult);
        }
        return suppDocResultGrid.DataBind(null);
      }

      public JsonResult AttachmentGridData(string invoiceId, string transactionId, string transactionType)
      {
        var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "SupportingDoc",
                                                                                  new
                                                                                  {
                                                                                    invoiceId,
                                                                                    transactionId,
                                                                                    transactionType
                                                                                }));
        if (!string.IsNullOrEmpty(transactionType))
        {
          var transactionRecordType = Convert.ToInt32(transactionType);
          var attachmentList = _supportingDocumentManager.GetAttachmentForSearchEntity(invoiceId, transactionId, transactionRecordType);
          return attachmentGrid.DataBind(attachmentList.AsQueryable());
        }
        return attachmentGrid.DataBind(null);
      }

      [HttpGet]
      public FileStreamResult AttachmentDownload(string attachmentId, int transactionType)
      {
        var attachmentDetail = _supportingDocumentManager.GetSupportingDocumentDetail(attachmentId, transactionType);
        var fileDownloadHelper = new FileAttachmentHelper { Attachment = attachmentDetail };

        return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
      }
    }
}
