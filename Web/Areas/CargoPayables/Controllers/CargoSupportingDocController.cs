using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Security.Permissions.Cargo.Payables;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Business.Cargo;
using Iata.IS.Model.Base;
//using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Cargo.Enums;
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

namespace Iata.IS.Web.Areas.CargoPayables.Controllers
{
    public class CargoSupportingDocController : ISController
    {
        private const string PayableSupportingDocSearchGridAction = "PayableSupportingDocSearchGridData";
        private ICargoSupportingDocumentManager _supportingDocumentManager;
        public CargoSupportingDocController(ICargoSupportingDocumentManager supportingDocumentManager)
        {
            _supportingDocumentManager = supportingDocumentManager;
        }

        [ISAuthorize(ViewSupportingDocuments.Query)]
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

        [ValidateAntiForgeryToken]
        [HttpPost]
        [ISAuthorize(ViewSupportingDocuments.Query)]
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

        [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ManageSupportingDocuments.Add)]
        [HttpPost]
        public JsonResult UploadAttachment(string invoiceId, string transactionId, string transactionType)
        {
            string files = string.Empty;
            var attachments = new List<Attachment>();
            bool isUploadSuccess;
            string message;
            HttpPostedFileBase fileToSave;
            FileAttachmentHelper fileUploadHelper = null;

            //try
            //{
            //    foreach (string file in Request.Files)
            //    {
            //        fileToSave = Request.Files[file];
            //        if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
            //        {
            //            continue;
            //        }

            //        var recordType = Convert.ToInt32(transactionType);

            //        //if (recordType == (int)SupportingDocRecordType.FormC)
            //        //{
            //        //    var formC = _samplingFormCManager.GetSamplingFormCDetails(invoiceId);

            //        //    fileUploadHelper = new FileAttachmentHelper
            //        //    {
            //        //        FileToSave = fileToSave,
            //        //        FileRelativePath = String.Format("{0}_{1}_{2}", formC.ProvisionalBillingMemberId, formC.ProvisionalBillingYear, formC.ProvisionalBillingMonth)
            //        //    };
            //if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
            //{
            //    throw new ISBusinessException(Messages.InvalidFileName);
            //}
            //        //    if (!fileUploadHelper.ValidateFileExtention(formC.ProvisionalBillingMemberId, BillingCategoryType.Pax))
            //        //    {
            //        //        throw new ISBusinessException(Messages.InvalidFileExtension);
            //        //    }
            //        //}
            //        //else
            //        //{
            //        var invoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
            //        //var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
            //        fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

            //        if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
            //        {
            //            throw new ISBusinessException(Messages.InvalidFileExtension);
            //        }
            //        //}

            //        //Check if duplicate file exists
            //        if (_supportingDocumentManager.IsDuplicateFileName(fileUploadHelper.FileOriginalName, transactionId, recordType))
            //        {
            //            throw new ISBusinessException(Messages.FileDuplicateError);
            //        }

            //        if (fileUploadHelper.SaveFile())
            //        {
            //            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            //            var transactionIdGuid = transactionId.ToGuid();
            //            var attachment = new SupportingDocumentAttachment
            //            {
            //                Id = fileUploadHelper.FileServerName,
            //                OriginalFileName = fileUploadHelper.FileOriginalName,
            //                FileSize = fileUploadHelper.FileToSave.ContentLength,
            //                LastUpdatedBy = SessionUtil.UserId,
            //                ServerId = fileUploadHelper.FileServerInfo.ServerId,
            //                FileStatus = FileStatusType.Received,
            //                FilePath = fileUploadHelper.FileRelativePath,
            //                ParentId = transactionIdGuid
            //            };
            //            var attachmentResult = _supportingDocumentManager.AddSupportingDoc(attachment, recordType);
            //            attachments.Add(attachmentResult);
            //        }
            //    }

            //    isUploadSuccess = true;
            //    message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
            //}
            //catch (ISBusinessException ex)
            //{
            //    isUploadSuccess = false;
            //    message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
            //    if (fileUploadHelper != null)
            //    {
            //        fileUploadHelper.DeleteFile();
            //    }
            //}
            //catch (Exception)
            //{
            //    isUploadSuccess = false;
            //    message = Messages.FileUploadUnexpectedError;
            //    if (fileUploadHelper != null)
            //    {
            //        fileUploadHelper.DeleteFile();
            //    }
            //}



            message = Messages.FileUploadUnexpectedError; 
            isUploadSuccess = true;
            return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
        }
    }
}
