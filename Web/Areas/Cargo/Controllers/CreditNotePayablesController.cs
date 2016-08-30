using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Areas.Cargo.Controllers.Base;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class CreditNotePayablesController : CargoInvoiceControllerBase
  {
    private readonly ICargoCreditNoteManager _cargoCreditNoteManager;
    private const string CmAwbGridAction = "CreditMemoAwbGridData";
    private const string CreditMemoGridAction = "CreditMemoGridData";
    public const string SourceCodeGridAction = "SourceCodeGridData";
    private readonly IReferenceManager _referenceManager;

    public CreditNotePayablesController(ICargoCreditNoteManager cargoCreditNoteManager, IMemberManager memberManager, IReferenceManager referenceManager)
      : base(cargoCreditNoteManager)
    {
      _cargoCreditNoteManager = cargoCreditNoteManager;
      MemberManager = memberManager;
      _referenceManager = referenceManager;
    }

    public JsonResult CreditMemoGridData(string invoiceId)
    {
      var creditMemoGrid = new CargoCreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));

      var creditMemoList = _cargoCreditNoteManager.GetCreditMemoList(invoiceId);

      return creditMemoGrid.DataBind(creditMemoList.AsQueryable());
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.View)]
    public new ActionResult View(string invoiceId)
    {
      ViewData[ViewDataConstants.TransactionExists] = _cargoCreditNoteManager.IsCreditMemoExists(invoiceId);
      MakeInvoiceRenderReady(invoiceId.ToGuid(), InvoiceHeader);

      // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is not retrieve from database.
      if (InvoiceHeader.CGOInvoiceTotal == null)
      {
        InvoiceHeader.CGOInvoiceTotal = new CargoInvoiceTotal();
      }
      //Initialize Awb code grid
      var subTotalGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SubTotalGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SubTotalGrid] = subTotalGrid.Instance;

      // Initialize Credit Memo grid
      var creditMemoGrid = new CargoCreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;

      // Get all submitted errors.
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      ViewData[ViewDataConstants.SubmittedErrorsGrid] = submittedErrorsGrid.Instance;

      // If BillingType is Payables instantiate SourceCode Vat Total grid
      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        var availableVatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
        ViewData["VatGrid"] = availableVatGrid.Instance;
      }

      return View("Edit", InvoiceHeader);
    }

    public JsonResult AvailableEmptySourceCodeVatTotalGridData()
    {
      return null;
    }

    //done
    /// <summary>
    /// Following action is used to retrieve Source Code Vat total and display it on SourceCodeVatTotal grid
    /// </summary>
    /// <param name="sourceCodeId">SourceCodeVat Total Id</param>
    /// <returns>Json result for SourceCode vat total</returns>
    public JsonResult GetBillingCodeVatTotal(string sourceCodeId)
    {
      // Call GetSourceCodeVatTotal() method which returns SourceCode vat total details
      var sourceCodeVatTotalList = _cargoCreditNoteManager.GetBillingCodeVatTotal(sourceCodeId);
      // Return Json result
      return Json(sourceCodeVatTotalList);
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.View)]
    public ActionResult CMView(string invoiceId, string transactionId)
    {
      var creditMemoRecord = GetCreditMemoRecord(invoiceId, transactionId);
      ViewData[ViewDataConstants.BreakdownExists] = _cargoCreditNoteManager.GetCreditMemoAwbBreakdownCount(transactionId) > 0 ? true : false;
      // Initialize CMAwb grid
      var creditMemoAwbGrid = new CargoCMAwbGrid(ViewDataConstants.CreditMemoAwbGrid, Url.Action("CreditMemoAwbGridData", "CreditNotePayables", new { transactionId }));
      ViewData[ViewDataConstants.CreditMemoAwbGrid] = creditMemoAwbGrid.Instance;

      return View("CMEdit", creditMemoRecord);
    }

    /// <summary>
    /// BMs the awb prepaid data.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    public JsonResult CreditMemoAwbGridData(string transactionId)
    {
      // Create grid instance and retrieve data from database
      var creditMemoAwbGrid = new CargoCMAwbGrid(ControlIdConstants.CreditMemoAwbGrid, Url.Action("CreditMemoAwbData", "CreditNotePayables", new { transactionId }));
      ViewData[ViewDataConstants.CreditMemoAwbGrid] = creditMemoAwbGrid.Instance;
      var creditMemoAwbs = _cargoCreditNoteManager.GetCMAwbList(transactionId).AsQueryable();
      try
      {
        return creditMemoAwbGrid.DataBind(creditMemoAwbs);
      }
      catch (ISBusinessException be)
      {
        ViewData["errorMessage"] = be.ErrorCode;
        return null;
      }
    }

    private CargoCreditMemo GetCreditMemoRecord(string invoiceId, string transactionId)
    {
      var creditMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      //var billingMemo = new CargoBillingMemo();
      creditMemo.Invoice = InvoiceHeader;
      creditMemo.LastUpdatedBy = SessionUtil.UserId;

      return creditMemo;
    }

    /// <summary>
    /// Billings the memo attachment download.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.Download)]
    public FileStreamResult CreditMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoCreditNoteManager.GetCreditMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.View)]
    public ActionResult CMAwbView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
        return RedirectToAction("CMAwbPrepaidView", new { invoiceId, transactionId, couponId });

      return RedirectToAction("CMAwbChargeCollectView", new { invoiceId, transactionId, couponId });
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.View)]
    public ActionResult CMAwbPrepaidView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      return View("CMAwbPrepaidEdit", awbRecord);
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.View)]
    public ActionResult CMAwbChargeCollectView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      return View("CMAwbChargeCollectEdit", awbRecord);
    }

    /// <summary>
    /// Gets the BM awb record.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The bm awb id.</param>
    /// <returns></returns>
    private CMAirWayBill GetCMAwbRecord(string invoiceId, string transactionId, string couponId)
    {
      var cmAwb = _cargoCreditNoteManager.GetCreditMemoAwbRecordDetails(couponId);
      cmAwb.CreditMemoRecord.Invoice = InvoiceHeader;
      cmAwb.LastUpdatedBy = SessionUtil.UserId;

      return cmAwb;
    }

    /// <summary>
    /// Billings the memo attachment upload.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult CreditMemoAttachmentUpload(string invoiceId, string transactionId)
    {
      string files = string.Empty;
      var attachments = new List<CargoCreditMemoAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;

      try
      {
        var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          //On Billing Memo Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoCreditNoteManager.IsDuplicateBillingMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
            throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (fileUploadHelper.SaveFile())
          {
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new CargoCreditMemoAttachment
            {
              Id = fileUploadHelper.FileServerName,
              OriginalFileName = fileUploadHelper.FileOriginalName,
              FileSize = fileUploadHelper.FileToSave.ContentLength,
              LastUpdatedBy = SessionUtil.UserId,
              ServerId = fileUploadHelper.FileServerInfo.ServerId,
              FileStatus = FileStatusType.Received,
              FilePath = fileUploadHelper.FileRelativePath
            };

            attachment = _cargoCreditNoteManager.AddCreditMemoAttachment(attachment);
            isUploadSuccess = true;
            attachments.Add(attachment);
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
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// CMs the awb attachment upload.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult CMAwbAttachmentUpload(string invoiceId, string transactionId)
    {
      string files = string.Empty;
      var attachments = new List<CMAwbAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;

      try
      {
        var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          //On Billing Memo Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoCreditNoteManager.IsDuplicateCreditMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
            throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (fileUploadHelper.SaveFile())
          {
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new CMAwbAttachment
            {
              Id = fileUploadHelper.FileServerName,
              OriginalFileName = fileUploadHelper.FileOriginalName,
              FileSize = fileUploadHelper.FileToSave.ContentLength,
              LastUpdatedBy = SessionUtil.UserId,
              ServerId = fileUploadHelper.FileServerInfo.ServerId,
              FileStatus = FileStatusType.Received,
              FilePath = fileUploadHelper.FileRelativePath
            };

            attachment = _cargoCreditNoteManager.AddCMAwbAttachment(attachment);
            isUploadSuccess = true;
            attachments.Add(attachment);
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
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download  Credit Memo Awb attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Payables.CreditNote.Download)]
    [HttpGet]
    public FileStreamResult CreditMemoAwbAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoCreditNoteManager.GetCMAwbAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }
  }
}