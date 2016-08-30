using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Web.UIModel.Grid.MU;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.MU.Controllers
{
    public class MuSupportingDocControllerBase : MiscUatpControllerBase
    {

        private readonly IMiscUatpInvoiceManager _invoiceManager;
        private readonly IReferenceManager _referenceManager;
        private ISupportingDocumentManager _supportingDocumentManager { get; set; }
        private const string SupportingDocSearchGridAction = "SupportingDocSearchGridData";
        private bool _isUatp;

        protected MuSupportingDocControllerBase(IMiscUatpInvoiceManager miscUatpInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager, ISupportingDocumentManager supportingDocumentManager,bool isUatp)
            : base(miscUatpInvoiceManager, referenceManager, memberManager)
    {
      _invoiceManager = miscUatpInvoiceManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
       _supportingDocumentManager = supportingDocumentManager;
            _isUatp = isUatp;
    }

        //
        // GET: /Misc/SupportingDoc/
        public virtual ActionResult Index()
        {
            dynamic searchCriteria = "";
            if (_isUatp)
            {
                searchCriteria =
                    new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn)
                    {
                        SupportingDocumentTypeId = (int)SupportingDocType.UatpInvoice,
                        BillingMemberId = SessionUtil.MemberId
                    };
            }
            else
            {
                 searchCriteria =
                    new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn)
                        {
                            SupportingDocumentTypeId = (int) SupportingDocType.MiscInvoice,
                            BillingMemberId = SessionUtil.MemberId
                        };
            }

            if (_isUatp)
            {
                var searchResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction), false);
                ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;
            }
            else
            {
                var searchResultGrid =
                    new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid,
                                                      Url.Action(SupportingDocSearchGridAction),true);
                ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;
            }

            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", new {  }));
            ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;

            return View(searchCriteria);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public virtual ActionResult Index(SupportingDocSearchCriteria searchCriteria)
        {
            searchCriteria.BillingMemberId = SessionUtil.MemberId;
            if (searchCriteria.IsUatp == false)
            {
                searchCriteria.SupportingDocumentTypeId = (int) SupportingDocType.MiscInvoice;
            }
            else
            {
                searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.UatpInvoice;
               var chargeCatList =
                    _referenceManager.GetChargeCategoryList(Iata.IS.Model.Enums.BillingCategoryType.Uatp).First();
                searchCriteria.ChargeCategoryId = chargeCatList.Id;
            }
            searchCriteria.CutOffDateEventName = CalendarConstants.SupportingDocumentsLinkingDeadlineColumn;
            var criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

            if (_isUatp)
            {
                var searchResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction, new { criteria }), false);
                ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;
            }
            else
            {
                
                var searchResultGrid =
                    new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid,
                                                      Url.Action(SupportingDocSearchGridAction ,new { criteria }), true);
                ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;
            }


            //var searchResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction, new { criteria }));
            //ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = searchResultGrid.Instance;

            
            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData",  new {  }));
            ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;

            return View(searchCriteria);
        }

        /// <summary>
        /// Fetch data for supporting document search result
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual JsonResult SupportingDocSearchGridData(string criteria)
        {
            dynamic suppDocResultGrid = "";
            if(_isUatp)
            {
                suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction,
                                                                                      new
                                                                                      {
                                                                                          criteria
                                                                                      }),false);
            }
            else
            {
                suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction,
                                                                                      new
                                                                                      {
                                                                                          criteria
                                                                                      }),true);
            }
           
            if (!string.IsNullOrEmpty(criteria))
            {
                var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
                if(searchCriteria.IsUatp == false)
                searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.MiscInvoice;
                else
                    searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.UatpInvoice;

                var searchResult = _supportingDocumentManager.GetSupportingDocumentSearchResult(searchCriteria).AsQueryable();
                return suppDocResultGrid.DataBind(searchResult);
            }
            return suppDocResultGrid.DataBind(null);
        }

        public virtual JsonResult AttachmentGridData(string invoiceId = null)
        {
            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData",  new { invoiceId }));
            if (!string.IsNullOrEmpty(invoiceId))
            {
                var attachmentList = _invoiceManager.GetAttachments(invoiceId);

                //Set the FileSizeInKb to file size in Kilo bytes.
                attachmentList = attachmentList.Select(attachment => { attachment.FileSizeInKb = (attachment.FileSize / 1024M); return attachment; }).ToList();

                setSerialNoForAttachment(attachmentList);

                return attachmentGrid.DataBind(attachmentList.AsQueryable());
            }
            return attachmentGrid.DataBind(null);
        }

        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Misc.Receivables.ManageSupportingDocuments.Add)]
        [HttpPost]
        public virtual JsonResult UploadAttachment(string invoiceId)
        {
            return UploadInvoiceAttachment(invoiceId, true);
        }

        [ISAuthorize(Business.Security.Permissions.Misc.Receivables.ManageSupportingDocuments.Remove)]
        [HttpPost]
        public virtual JsonResult AttachmentDelete(string attachmentId, bool isSupportingDoc = true)
        {
          return base.DeleteAttachment(attachmentId, isSupportingDoc);
        }

        /// <summary>
        /// Download Invoice attachment
        ///  </summary>
        /// <param name="invoiceId">Invoice id</param>
        /// <param name="lineItemId">lineItem id</param>
        /// <returns></returns>
        [HttpGet]
        public virtual FileStreamResult AttachmentDownload(string invoiceId, string lineItemId)
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
