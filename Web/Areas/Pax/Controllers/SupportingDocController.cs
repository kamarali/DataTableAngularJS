using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.SupportingDocuments.Enums;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Business;
using Iata.IS.Model.Enums;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Business.Common;
using Iata.IS.Web.UIModel.Grid.Common;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
    public class SupportingDocController : ISController
    {
      private const string SupportingDocSearchGridAction = "SupportingDocSearchGridData";
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

      [HttpGet]
      [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ManageSupportingDocuments.Add)]
      public ActionResult Index()
      {
        SetViewDataBillingType(Util.BillingType.Receivables); 
        var searchCriteria = new SupportingDocSearchCriteria(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn) { SupportingDocumentTypeId = (int)SupportingDocType.InvoiceCreditNote };
        searchCriteria.BillingMemberId = SessionUtil.MemberId;

        //Initialize search result grid
        var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction, "SupportingDoc"));
          
        ViewData[ViewDataConstants.SupportingDocSearchResultGrid] = suppDocResultGrid.Instance;


        var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "SupportingDoc",
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
        SetViewDataBillingType(Util.BillingType.Receivables);
          //searchCriteria.BillingMonth = 10;
          //searchCriteria.BillingYear = 2012;
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

        var attachmentGrid = new SupportingAttachmentGrid(ControlIdConstants.AttachmentGrid, Url.Action("AttachmentGridData", "SupportingDoc",
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
        var suppDocResultGrid = new SupportingDocSearchResultGrid(ControlIdConstants.SupportingDocSearchResultGrid, Url.Action(SupportingDocSearchGridAction, "SupportingDoc",
                                                                                  new
                                                                                  {
                                                                                    criteria
                                                                                  }));
        if (!string.IsNullOrEmpty(criteria))
        {
          var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(SupportingDocSearchCriteria)) as SupportingDocSearchCriteria;
          var searchResult = _supportingDocumentManager.GetSupportingDocumentSearchResult(searchCriteria).AsQueryable();
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

      [ValidateAntiForgeryToken]
      [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ManageSupportingDocuments.Add)]
      [HttpPost]
      public JsonResult UploadAttachment(string invoiceId, string transactionId, string transactionType)
      {
        var files = string.Empty;
        var attachments = new List<Attachment>();
        // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
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

            if (recordType == (int)SupportingDocRecordType.FormC)
            {
              //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
              var formC = _samplingFormCManager.GetSamplingFormCDetailsForAttachmentUpload(invoiceId);

              fileUploadHelper = new FileAttachmentHelper
              {
                FileToSave = fileToSave,
                FileRelativePath = String.Format("{0}_{1}_{2}", formC.ProvisionalBillingMemberId, formC.ProvisionalBillingYear, formC.ProvisionalBillingMonth)
              };

              if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
              {
                  throw new ISBusinessException(Messages.InvalidFileName);
              }

              if (!fileUploadHelper.ValidateFileExtention(formC.ProvisionalBillingMemberId, BillingCategoryType.Pax))
              {
                throw new ISBusinessException(Messages.InvalidFileExtension);
              }
            }
            else 
            {
               //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
              var invoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);
              fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

              if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
              {
                  throw new ISBusinessException(Messages.InvalidFileName);
              }

              if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
              {
                throw new ISBusinessException(Messages.InvalidFileExtension);
              }
            }
            
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
              //Add the user details from session
              if (attachmentResult.UploadedBy == null)
              {
                attachmentResult.UploadedBy = new User {FirstName = SessionUtil.Username};
              }
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

      [ISAuthorize(Business.Security.Permissions.Pax.Receivables.ManageSupportingDocuments.Remove)]
      [HttpPost]
      public JsonResult AttachmentDelete(string attachmentId, int transactionType, string invoiceId)
      {
        UIMessageDetail details;
        try
        {
          var invoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();
          var invId = new Guid(invoiceId);
          var invoice = invoiceBaseRepository.First(i => i.Id == invId);

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
