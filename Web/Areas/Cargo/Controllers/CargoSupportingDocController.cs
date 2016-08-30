using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
using Iata.IS.Web.UIModel.Grid.Cargo;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
    public class CargoSupportingDocController : ISController
    {
        private const string PayableSupportingDocSearchGridAction = "PayableSupportingDocSearchGridData";
        
        private ISupportingDocumentManager _supportingDocumentManager { get; set; }
        private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
        private readonly ISamplingFormCManager _samplingFormCManager;

        public CargoSupportingDocController(ISupportingDocumentManager supportingDocumentManager, INonSamplingInvoiceManager nonSamplingInvoiceManager, ISamplingFormCManager samplingFormCManager)
        {
            _supportingDocumentManager = supportingDocumentManager;
            _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
            _samplingFormCManager = samplingFormCManager;
        }


        public ActionResult PayableSupportingDocs()
        {
            SetViewDataBillingType(Util.BillingType.Payables);
            var searchCriteria = new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn) { SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote };
            searchCriteria.BilledMemberId = SessionUtil.MemberId;

            //Initialize search result grid
            var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(ControlIdConstants.CargoPayableSupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction, "CargoSupportingDoc"));

            ViewData[ViewDataConstants.CargoPayableSupportingDocSearchResultGrid] = suppDocResultGrid.Instance;


            var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "CargoSupportingDoc",
                                                                                      new
                                                                                      {
                                                                                      }));
            ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;


            return View(searchCriteria);
        }

        [HttpPost]
        public ActionResult PayableSupportingDocs(SupportingDocSearchCriteria searchCriteria)
        {
            searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote;
            SetViewDataBillingType(Util.BillingType.Payables);
            searchCriteria.BilledMemberId = SessionUtil.MemberId;
            searchCriteria.CutOffDateEventName = CalendarConstants.SupportingDocumentsLinkingDeadlineColumn;
            var criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

            //Initialize search result grid
            var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(ControlIdConstants.CargoPayableSupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction,
                                                                                      new
                                                                                      {
                                                                                          criteria
                                                                                      }));

            ViewData[ViewDataConstants.CargoPayableSupportingDocSearchResultGrid] = suppDocResultGrid.Instance;

            var attachmentGrid = new PayableSupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "CargoSupportingDoc",
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
            var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(ControlIdConstants.CargoPayableSupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction,
                                                                                      new
                                                                                      {
                                                                                          criteria
                                                                                      }));
            if (!string.IsNullOrEmpty(criteria))
            {
                var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
                var searchResult = _supportingDocumentManager.GetCargoPayableSupportingDocumentSearchResult(searchCriteria).AsQueryable();
                return suppDocResultGrid.DataBind(searchResult);
            }
            return suppDocResultGrid.DataBind(null);
        }

        public JsonResult AttachmentGridData(string invoiceId, string transactionId, string transactionType)
        {
            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "CargoSupportingDoc",
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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web.Mvc;
//using Iata.IS.Business.Pax;
//using Iata.IS.Model.Cargo.Payables;
//using Iata.IS.Web.UIModel.Grid.Common;
//using Iata.IS.Web.Util;
//using System.Web.Script.Serialization;
//using Iata.IS.Business.SupportingDocuments;
//using Iata.IS.Business.Common;
//using Iata.IS.Model.SupportingDocuments.Enums;
//using Iata.IS.Web.UIModel.Grid.Cargo;

//namespace Iata.IS.Web.Areas.Cargo.Controllers
//{
//    public class CargoSupportingDocController : ISController
//    {
//        private const string PayableSupportingDocSearchGridAction = "PayableSupportingDocSearchGridData";
//        private ISupportingDocumentManager _supportingDocumentManager { get; set; }

//        public CargoSupportingDocController(ISupportingDocumentManager supportingDocumentManager)
//        {
//            _supportingDocumentManager = supportingDocumentManager;
//        }

//        public ActionResult PayableSupportingDocs()
//        {
//            //SetViewDataBillingType(Util.BillingType.Payables);
//            var searchCriteria = new CGOSupportingDoc(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn)
//                                     {
//                                         SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote
//                                     };

//            //searchCriteria.BilledMemberId = SessionUtil.MemberId;

//            //Initialize search result grid
//            var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(
//                ControlIdConstants.CargoPayableSupportingDocSearchResultGrid,
//                Url.Action(PayableSupportingDocSearchGridAction,
//                           "CargoSupportingDoc")
//                );

//            ViewData[ViewDataConstants.CargoPayableSupportingDocSearchResultGrid] = suppDocResultGrid.Instance;

//            return View(searchCriteria);
//        }

//        [HttpPost]
//        public ActionResult PayableSupportingDocs(CGOSupportingDoc searchCriteria)
//        {
//            searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote;
//            //SetViewDataBillingType(Util.BillingType.Payables);
//            //searchCriteria.BilledMemberId = SessionUtil.MemberId;
//            //searchCriteria.CutOffDateEventName = CalendarConstants.SupportingDocumentsLinkingDeadlineColumn;
//            var criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;

//            //Initialize search result grid
//            var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(
//                ControlIdConstants.CargoPayableSupportingDocSearchResultGrid,
//                Url.Action(PayableSupportingDocSearchGridAction,
//                           new
//                               {
//                                   criteria
//                               }));

//            ViewData[ViewDataConstants.CargoPayableSupportingDocSearchResultGrid] = suppDocResultGrid.Instance;

//            return View(searchCriteria);
//        }

//        public JsonResult PayableSupportingDocSearchGridData(string criteria)
//        {
//            var searchCriteria = new CGOSupportingDoc
//                                     {
//                                         BillingYear = 2011,
//                                         BillingMonth = 4,
//                                         BillingPeriod = "4",
//                                         BillingCode = Model.Cargo.Enums.BillingCode.AWBChargeCollect,
//                                         SupportingDocumentTypeId = 1,
//                                         Type = SupportingDocType.InvoiceCreditNote,
//                                         BilledMemberId = 1,
//                                         BilledMemberText = "abc",
//                                         BillingMemberText = "def",
//                                         BillingMemberId = 1,
//                                         InvoiceNumber = "z",
//                                         SourceCodeId = 1,
//                                         BatchSequenceNumber = 1,
//                                         RecordSequenceWithinBatch = 1,
//                                         RMBMCMNumber = "RMBMCMNumber",
//                                         AWBNumber = 1,
//                                         AttachmentIndicatorOriginal = 1,
//                                         AttachmentIndicatorValidated = true,
//                                         IsMismatchCases = true,
//                                         CutOffDateEventName = "CuroffDateEventName"
//                                     };

//            if (!string.IsNullOrEmpty(criteria))
//            {
//                searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CGOSupportingDoc)) as CGOSupportingDoc;
//            }

//            var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(
//                                                            ControlIdConstants.CargoPayableSupportingDocSearchResultGrid,
//                                                            Url.Action(PayableSupportingDocSearchGridAction, new { area = "Cargo", searchCriteria }));

//            var invObj = new List<CGOSupportingDoc>();
//            //invObj.Add(searchCriteria);
//            //invObj.Add(searchCriteria);

//            return suppDocResultGrid.DataBind(invObj.AsQueryable());


//            /*var suppDocResultGrid = new CargoPayableSupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(PayableSupportingDocSearchGridAction,
//                                                                                  new
//                                                                                  {
//                                                                                      criteria
//                                                                                  }));

//            if (!string.IsNullOrEmpty(criteria))
//            {

//                //var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(CGOSupportingDoc)) as CGOSupportingDoc;
//                //var searchResult = _supportingDocumentManager.GetCargoPayableSupportingDocumentSearchResult(searchCriteria).AsQueryable();

//                var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
//                var searchResult = _supportingDocumentManager.GetPayableSupportingDocumentSearchResult(searchCriteria).AsQueryable();

//                return suppDocResultGrid.DataBind(searchResult);
                
//            }
//            return suppDocResultGrid.DataBind(null);*/
//            /*
//                       string searchcriteria = criteria != null ? new JavaScriptSerializer().Serialize(criteria) : string.Empty;
            
//                       List<CargoSupportingDocSearchResult> invObj = new List<CargoSupportingDocSearchResult>();
//                       return suppDocResultGrid.DataBind(invObj.AsQueryable());
//           */
//        }
//    }
//}
