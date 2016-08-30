using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Cargo.Common;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Business.Cargo;
using Iata.IS.Model.Base;
using Iata.IS.Business;
using Iata.IS.Model.Enums;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Business.Common;
using Iata.IS.Web.UIModel.Grid.Common;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
    public class CgoSupportingDocController : ISController
    {
        private const string SupportingDocSearchGridAction = "SupportingDocSearchGridData";
        //
        // GET: /Cargo/SupportingDoc/

        private ICargoSupportingDocumentManager _supportingDocumentManager;
        //private ISupportingDocumentManager _supportingDocumentManager;
        //private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
        private readonly ICargoInvoiceManager _invoiceManager;
        //private readonly ISamplingFormCManager _samplingFormCManager;

        public ICalendarManager CalendarManager { private get; set; }

        public CgoSupportingDocController(ICargoSupportingDocumentManager supportingDocumentManager, ICargoInvoiceManager InvoiceManager)
        {
            _supportingDocumentManager = supportingDocumentManager;
            _invoiceManager = InvoiceManager;
            
            
        }

        [HttpGet]
        public ActionResult Index()
        {
            SetViewDataBillingType(Util.BillingType.Receivables);
            var searchCriteria = new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn) { SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote };
            searchCriteria.BillingMemberId = SessionUtil.MemberId;

            //Initialize search result grid
            var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction, "CgoSupportingDoc"));

            ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = suppDocResultGrid.Instance;


            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "CgoSupportingDoc",
                                                                                      new
                                                                                      {
                                                                                      }));
            ViewData[ViewDataConstants.AttachmentResultGrid] = attachmentGrid.Instance;
            return View(searchCriteria);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SupportingDocSearchCriteria searchCriteria)
        {
            if(searchCriteria.AttachmentIndicatorOriginal == 2)
            {
                searchCriteria.AttachmentIndicatorOriginal = 0;
            }
            searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote;
            SetViewDataBillingType(Util.BillingType.Receivables);
            searchCriteria.BillingMemberId = SessionUtil.MemberId;
            searchCriteria.CutOffDateEventName = CalendarConstants.SupportingDocumentsLinkingDeadlineColumn;
            var criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;
            
            //Initialize search result grid
            var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction,
                                                                                      new
                                                                                      {
                                                                                          criteria
                                                                                      }));

            ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = suppDocResultGrid.Instance;

            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "CgoSupportingDoc",
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

        public JsonResult SupportingDocSearchGridData(string criteria)
        {
            
            var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction, "CgoSupportingDoc",
                                                                                      new { area = "Cargo", criteria }));
            if (!string.IsNullOrEmpty(criteria))
            {
                var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
                var searchResult = _supportingDocumentManager.GetCargoSupportingDocumentSearchResult(searchCriteria).AsQueryable();
                return suppDocResultGrid.DataBind(searchResult);
            }
            return suppDocResultGrid.DataBind(null);
        }

       
        //public JsonResult SupportingDocSearchGridData(string criteria)
        //{

        //    var searchCriteria = new SupportingDocSearchCriteria();
                      
        //    if (!string.IsNullOrEmpty(criteria))
        //    {
        //        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
        //    }
                       
        //    // Create grid instance and retrieve data from database

        //    //var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SearchGrid, Url.Action(SupportingDocSearchGridAction, new { area = "Cargo", criteria }));

        //    var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SearchGrid, Url.Action(SupportingDocSearchGridAction, "SupportingDoc",
        //                                                                                                             new { area = "Cargo", criteria }));
        //    searchCriteria.SupportingDocumentTypeId = (int)SupportingDocType.Cargo;

        //    var searchResult = _supportingDocumentManager.GetSupportingDocumentSearchResult(searchCriteria).AsQueryable();

        //    return suppDocResultGrid.DataBind(searchResult);

        //}


        public JsonResult AttachmentGridData(string invoiceId, string transactionId, string transactionType)
        {
            var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "CgoSupportingDoc",
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

        [ValidateAntiForgeryToken]
        [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.ManageSupportingDocuments.Add)]
        [HttpPost]
        public JsonResult UploadAttachment(string invoiceId, string transactionId, string transactionType)
        {
            string files = string.Empty;
            var attachments = new List<Attachment>();
            // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
            var isUploadSuccess = false;
            string message;
            HttpPostedFileBase fileToSave;
            FileAttachmentHelper fileUploadHelper = null;

            try
            {
                foreach (string file in Request.Files)
                {
                  isUploadSuccess = false;
                    fileToSave = Request.Files[file];
                    if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
                    {
                        continue;
                    }

                    var recordType = Convert.ToInt32(transactionType);

                    //if (recordType == (int)SupportingDocRecordType.FormC)
                    //{
                    //    var formC = _samplingFormCManager.GetSamplingFormCDetails(invoiceId);

                    //    fileUploadHelper = new FileAttachmentHelper
                    //    {
                    //        FileToSave = fileToSave,
                    //        FileRelativePath = String.Format("{0}_{1}_{2}", formC.ProvisionalBillingMemberId, formC.ProvisionalBillingYear, formC.ProvisionalBillingMonth)
                    //    };
                    //if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
                    //{
                    //    throw new ISBusinessException(Messages.InvalidFileName);
                    //}
                    //    if (!fileUploadHelper.ValidateFileExtention(formC.ProvisionalBillingMemberId, BillingCategoryType.Pax))
                    //    {
                    //        throw new ISBusinessException(Messages.InvalidFileExtension);
                    //    }
                    //}
                    //else
                    //{
                    var invoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);

                    DateTime eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                              invoice.BillingYear, invoice.BillingMonth,
                                              invoice.BillingPeriod);

                    if (DateTime.UtcNow > eventTime)
                    {
                        throw new ISBusinessException(Messages.SupportingDocDeadline);
                    }
                    //var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
                        fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

                        if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
                        {
                            throw new ISBusinessException(Messages.InvalidFileName);
                        }
   
                    if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
                        {
                            throw new ISBusinessException(Messages.InvalidFileExtension);
                        }
                    //}

                    //Check if duplicate file exists
                    if (_supportingDocumentManager.IsDuplicateFileName(fileUploadHelper.FileOriginalName, transactionId, recordType))
                    {
                        throw new ISBusinessException(Messages.FileDuplicateError);
                    }

                    if (fileUploadHelper.SaveFile())
                    {
                        files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
                        var transactionIdGuid = transactionId.ToGuid();
                        var attachment = new SupportingDocumentAttachment
                        {
                            Id = fileUploadHelper.FileServerName,
                            OriginalFileName = fileUploadHelper.FileOriginalName,
                            FileSize = fileUploadHelper.FileToSave.ContentLength,
                            LastUpdatedBy = SessionUtil.UserId,
                            ServerId = fileUploadHelper.FileServerInfo.ServerId,
                            FileStatus = FileStatusType.Received,
                            FilePath = fileUploadHelper.FileRelativePath,
                            ParentId = transactionIdGuid
                        };
                        var attachmentResult = _supportingDocumentManager.AddSupportingDoc(attachment, recordType, transactionIdGuid);
                        isUploadSuccess = true;
                        attachments.Add(attachmentResult);
                    }
                }
                message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
            }
            catch (ISBusinessException ex)
            {
                message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
                if (fileUploadHelper != null && isUploadSuccess == false)
                {
                    fileUploadHelper.DeleteFile();
                }
            }
            catch (Exception)
            {
                message = Messages.FileUploadUnexpectedError;
                if (fileUploadHelper != null && isUploadSuccess == false)
                {
                    fileUploadHelper.DeleteFile();
                }
            }

            return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
        }

        [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.ManageSupportingDocuments.Remove)]
        [HttpPost]
        public JsonResult AttachmentDelete(string attachmentId, int transactionType, string InvoivceId)
        {
            UIMessageDetail details;
            try
            {

                var invoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();

                var invoiceId = new Guid(InvoivceId);

                var invoice = invoiceBaseRepository.First(i => i.Id == invoiceId);


                DateTime eventTime = CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                                      invoice.BillingYear, invoice.BillingMonth,
                                                      invoice.BillingPeriod);

                if (DateTime.UtcNow > eventTime)
                {
                    throw new ISBusinessException(Messages.SupportingDocDeadline);
                }
                //Delete record
                bool isDeleted = _supportingDocumentManager.DeleteSupportingDoc(attachmentId, transactionType, invoice);

                details = isDeleted
                            ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful }
                            : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };

            }
            catch (ISBusinessException ex)
            {
                details = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = string.Format(Messages.DeleteException, GetDisplayMessage(ex.ErrorCode))
                };
            }

            return Json(details);
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
