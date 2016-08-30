using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Security.Permissions.Misc.Payables;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Business.Common;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Model.MiscUatp;
using PayableSupportingDocSearchResultGrid = Iata.IS.Web.UIModel.Grid.MUPayables.PayableSupportingDocSearchResultGrid;

namespace Iata.IS.Web.Areas.MiscPayables.Controllers
{
    public class MiscPayableSupportingDocController : MiscUatpControllerBase
    {
      private const string PayableSupportingDocSearchGridAction = "PayableSupportingDocSearchGridData";

      private ISupportingDocumentManager _supportingDocumentManager { get; set; }
      private readonly IMiscUatpInvoiceManager _miscInvoiceManager;

      public MiscPayableSupportingDocController(IMiscInvoiceManager miscInvoiceManager, IReferenceManager referenceManger, IMemberManager memberManager, ISupportingDocumentManager supportingDocumentManager)
        : base(miscInvoiceManager, referenceManger, memberManager)
      {
        _miscInvoiceManager = miscInvoiceManager;
        _supportingDocumentManager = supportingDocumentManager;
      }

      [ISAuthorize(ViewSupportingDocuments.Query)]
      public ActionResult PayableSupportingDocs()
      {
        var searchCriteria = new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn);
        searchCriteria.BilledMemberId = SessionUtil.MemberId;

        var searchResultGrid = new PayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction, "MiscPayableSupportingDoc"),true);
        ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;

        var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "MiscPayableSupportingDoc", new { }));
        ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;

        return View(searchCriteria);
      }

      [ValidateAntiForgeryToken]
      [HttpPost]
      [ISAuthorize(ViewSupportingDocuments.Query)]
      public ActionResult PayableSupportingDocs(SupportingDocSearchCriteria searchCriteria)
      {
        searchCriteria.BilledMemberId = SessionUtil.MemberId;
        searchCriteria.CutOffDateEventName = CalendarConstants.SupportingDocumentsLinkingDeadlineColumn;
        var criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

        var searchResultGrid = new PayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction, "MiscPayableSupportingDoc", new { criteria }), true);
        ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;

        var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "MiscPayableSupportingDoc", new { }));
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
          var suppDocResultGrid = new PayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction, "MiscPayableSupportingDoc",
                                                                                  new
                                                                                  {
                                                                                    criteria
                                                                                  }),true);
        if (!string.IsNullOrEmpty(criteria))
        {
          var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
          searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.MiscInvoice;

          var searchResult = _supportingDocumentManager.GetPayableSupportingDocumentSearchResult(searchCriteria).AsQueryable();
          return suppDocResultGrid.DataBind(searchResult);
        }
        return suppDocResultGrid.DataBind(null);
      }

      public JsonResult AttachmentGridData(string invoiceId)
      {
          var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "MiscPayableSupportingDoc", new { invoiceId }));
        if (!string.IsNullOrEmpty(invoiceId))
        {
          var attachmentList = _miscInvoiceManager.GetAttachments(invoiceId);

          //Set the FileSizeInKb to file size in Kilo bytes.
          attachmentList = attachmentList.Select(attachment => { attachment.FileSizeInKb = (attachment.FileSize / 1024M); return attachment; }).ToList();

          setSerialNoForAttachment(attachmentList);          

          return attachmentGrid.DataBind(attachmentList.AsQueryable());
        }
        return attachmentGrid.DataBind(null);
      }

      /// <summary>
      /// Download Invoice attachment
      ///  </summary>
      /// <param name="invoiceId">Invoice id</param>
      /// <param name="lineItemId">lineItem id</param>
      /// <returns></returns>
      [HttpGet]
      public FileStreamResult AttachmentDownload(string invoiceId, string lineItemId)
      {
        return base.InvoiceAttachmentDownload(invoiceId, lineItemId);
      }

      /// <summary>
      /// Set serial no for attachment grid
      /// </summary>
      /// <param name="attachment"></param>
      private void setSerialNoForAttachment(IList<MiscUatpAttachment> attachment)
      {
        var count = 1;
        foreach (var attach in attachment)
        {
          attach.SerialNo = count;
          count++;
        }
      }
    }
}
