using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Cargo;
using Iata.IS.Business.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Web.Areas.Cargo.Controllers.Base;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Business.Pax;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class CreditNoteController : CargoInvoiceControllerBase
  {
    private readonly ICargoCreditNoteManager _cargoCreditNoteManager;
    private const string CMAwbGridAction = "CreditMemoAwbGridData";
    private const string CreditMemoGridAction = "CreditMemoGridData";
    public const string SourceCodeGridAction = "SourceCodeGridData";
    private readonly IReferenceManager _referenceManager;

    public CreditNoteController(ICargoCreditNoteManager cargoCreditNoteManager, IMemberManager memberManager, IReferenceManager referenceManager)
      : base(cargoCreditNoteManager)
    {
      _cargoCreditNoteManager = cargoCreditNoteManager;
      MemberManager = memberManager;
      _referenceManager = referenceManager;
    }

    protected override InvoiceType InvoiceType
    {
      get { return InvoiceType.CreditNote; }
    }

    //
    // GET: /Pax/CreditNote/Create
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    public ActionResult Create()
    {
      SessionUtil.InvoiceSearchCriteria = null;

      var invoice = new CargoInvoice { InvoiceType = InvoiceType.CreditNote, InvoiceDate = DateTime.UtcNow, BillingMemberId = SessionUtil.MemberId, LastUpdatedBy = SessionUtil.UserId};
      MakeInvoiceRenderReady(invoice.Id, invoice);
      var digitalSignatureRequired = GetDigitalSignatureRequired(invoice.BillingMemberId);
      ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId] = digitalSignatureRequired;
      invoice.DigitalSignatureRequiredId = digitalSignatureRequired;

      /*SCP 270845 - Some validations missing is IS-WEB
       * Description: Bilateral SMI so exchange rate is allowed to be anything but 0. 
       * Also enforce that exchange rate has to be 1.00000 in cases when same billing and listing currencies.
      */
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      SetViewDataPageMode(PageMode.Create);

      return View(invoice);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    public ActionResult Create(CargoInvoice invoice)
    {
        try
        {
            invoice.InvoiceDate = DateTime.UtcNow;
            invoice.InvoiceType = InvoiceType.CreditNote;
            invoice.BillingCode = 0; // Convert.ToInt32(BillingCode.CreditNote);
            invoice.InvoiceStatus = InvoiceStatusType.Open;
            invoice.SubmissionMethodId = (int) Iata.IS.Model.Enums.SubmissionMethod.IsWeb;
            invoice.BillingMemberId = SessionUtil.MemberId;
            /* SCP# 303708 - Invoice Search - Note Owner
            * Desc: Server side validation is added to prevent value 0 as Invoice/Credit Note owner Id. */
            if (SessionUtil.UserId > 0)
            {
                invoice.InvoiceOwnerId = SessionUtil.UserId;
            }
            else
            {
                throw new ISBusinessException(ErrorCodes.OwnerMissing);
            }
            invoice.LastUpdatedBy = SessionUtil.UserId;
            var creditNoteHeader = _cargoCreditNoteManager.CreateInvoice(invoice);

            ShowSuccessMessage(Messages.CreditNoteCreateSuccessful);

            return RedirectToAction("Edit", new {invoiceId = creditNoteHeader.Id.Value()});
        }
        catch (ISBusinessException businessException)
        {
            /* CMP #624: ICH Rewrite-New SMI X 
              * Description: As per ICH Web Service Response Message specifications 
              * Refer FRS Section 3.3 Table 9. 
              * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

            var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
            var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

            invoice.BilledMember = _cargoCreditNoteManager.GetBilledMember(invoice.BilledMemberId);

            if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) &&
                (invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase) ||
                 invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase)))
            {
                ShowSmiXWebServiceErrorMessage(businessException.Message);
            }
            else
            {
                ShowErrorMessage(businessException.ErrorCode);
            }

            /*SCP 270845 - Some validations missing is IS-WEB
            * Description: Bilateral SMI so exchange rate is allowed to be anything but 0. 
            * Also enforce that exchange rate has to be 1.00000 in cases when same billing and listing currencies.
            */
            var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
            ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
            
            SetViewDataPageMode(PageMode.Create);

            if (invoice.MemberLocationInformation != null)
            {
                var billingMemberLocationInfo =
                    invoice.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
                if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
                {
                    ViewData["IsLegalTextSet"] = true;
                }
            }
        }
        MakeInvoiceRenderReady(invoice.Id, invoice);

        return View(invoice);
    }

      [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    public ActionResult Edit(string invoiceId)
    {
      if (string.IsNullOrEmpty(InvoiceHeader.BillingMemberLocationCode))
      {
        InvoiceHeader.BillingMemberLocationCode = "-";
      }
      if (string.IsNullOrEmpty(InvoiceHeader.BilledMemberLocationCode))
      {
        InvoiceHeader.BilledMemberLocationCode = "-";
      }

      ViewData[ViewDataConstants.TransactionExists] = _cargoCreditNoteManager.IsCreditMemoExists(invoiceId);

      if (InvoiceHeader.MemberLocationInformation != null)
      {
        var billingMemberLocationInfo = InvoiceHeader.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
        if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
        {
          ViewData["IsLegalTextSet"] = true;
        }
      }

      MakeInvoiceRenderReady(invoiceId.ToGuid(), InvoiceHeader);
      // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is
      // not retrieve from database.
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

      return View(InvoiceHeader);
    }

    // POST: /Cargo/CreditNote/Edit/5
      [ValidateAntiForgeryToken]
      [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
      [HttpPost]
      [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
      public ActionResult Edit(string invoiceId, CargoInvoice invoice)
      {
          try
          {
              invoice.Id = invoiceId.ToGuid();
              invoice.InvoiceType = InvoiceType.CreditNote;
              invoice.InvoiceStatus = InvoiceStatusType.Open;
              invoice.BillingMemberId = SessionUtil.MemberId;
              invoice.SubmissionMethodId = (int) Iata.IS.Model.Enums.SubmissionMethod.IsWeb;
              invoice.LastUpdatedBy = SessionUtil.UserId;
              _cargoCreditNoteManager.UpdateInvoice(invoice);

              ShowSuccessMessage(Messages.CreditNoteUpdateSuccessful);

              return RedirectToAction("Edit", new {invoiceId});
          }
          catch (ISBusinessException businessException)
          {
              /* CMP #624: ICH Rewrite-New SMI X 
                * Description: As per ICH Web Service Response Message specifications 
                * Refer FRS Section 3.3 Table 9. 
                * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

              var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
              var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

              invoice.BilledMember = _cargoCreditNoteManager.GetBilledMember(invoice.BilledMemberId);

              if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) &&
                  (invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase) ||
                   invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase)))
              {
                  ShowSmiXWebServiceErrorMessage(businessException.Message);
              }
              else
              {
                  ShowErrorMessage(businessException.ErrorCode);
              }

              SetViewDataPageMode(PageMode.Edit);

              if (invoice.MemberLocationInformation != null)
              {
                  var billingMemberLocationInfo =
                      invoice.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
                  if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
                  {
                      ViewData["IsLegalTextSet"] = true;
                  }
              }
          }
          MakeInvoiceRenderReady(invoice.Id, invoice);
          // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is
          // not retrieve from database.
          if (invoice.InvoiceTotalRecord == null)
          {
              invoice.CGOInvoiceTotal = new CargoInvoiceTotal();
          }

          //Initialize source code grid
          var sourceCodeGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId,
                                               Url.Action(SubTotalGridAction, new {invoiceId}),
                                               (ViewData[ViewDataConstants.BillingType].ToString() ==
                                                Util.BillingType.Payables)
                                                   ? true
                                                   : false);
          ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
          //Initialize Credit Memo grid
          var creditMemoGrid = new CargoCreditMemoGrid(ControlIdConstants.CreditMemoGrid,
                                                       Url.Action(CreditMemoGridAction, new {invoiceId}));
          ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;

          var subTotalGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId,
                                             Url.Action(SubTotalGridAction, new {invoiceId}),
                                             (ViewData[ViewDataConstants.BillingType].ToString() ==
                                              Util.BillingType.Payables)
                                                 ? true
                                                 : false);
          ViewData[ViewDataConstants.SubTotalGrid] = subTotalGrid.Instance;

          SetViewDataPageMode(PageMode.Edit);

          return View(invoice);
      }

      
       [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.View)]
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

    ///// <summary>
    ///// Used to instantiate SourceCode vat Total grid
    ///// </summary>
    ///// <returns>null</returns>
    public JsonResult AvailableEmptySourceCodeVatTotalGridData()
    {
        return null;
    }

    public JsonResult CreditMemoGridData(string invoiceId)
    {
      var creditMemoGrid = new CargoCreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));

      var creditMemoList = _cargoCreditNoteManager.GetCreditMemoList(invoiceId);

      return creditMemoGrid.DataBind(creditMemoList.AsQueryable());
    }

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
    /// <summary>
    /// BMs the create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CMCreate(string invoiceId)
    {
      // Retrieve Batch and Sequence number which will be pre populated, for Credit Memo
      int batchNumber;
      int sequenceNumber;
      _cargoCreditNoteManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.CreditNote, out batchNumber, out sequenceNumber);

      SetViewDataPageMode(PageMode.Create);
      var creditMemoRecord = new CargoCreditMemo { Invoice = InvoiceHeader, InvoiceId = invoiceId.ToGuid(), LastUpdatedBy = SessionUtil.UserId, BatchSequenceNumber = batchNumber, RecordSequenceWithinBatch = sequenceNumber};
      if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
      {
        if (TempData[TempDataConstants.CorrespondenceNumber] != null)
        {
          string correspondenceRefNumber = TempData[TempDataConstants.CorrespondenceNumber].ToString();
        }

        ViewData[ViewDataConstants.FromBillingHistory] = true;
      }
      return View(creditMemoRecord);
    }

    /// <summary>
    /// BMs the create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CMCreate(string invoiceId, CargoCreditMemo record)
    {
      var creditMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        record.LastUpdatedBy = SessionUtil.UserId;
        record.Attachments.Clear();
        record.BillingCode = (int) BillingCode.CreditNote;
        if (record.NetAmountCredited == null) record.NetAmountCredited = 0;
        _cargoCreditNoteManager.AddCreditMemoRecord(record);
        _cargoCreditNoteManager.UpdateCreditMemoAttachment(creditMemoAttachmentIds, record.Id);
        TempData.Clear();
        ShowSuccessMessage(Messages.CMCreateSuccessful);
        
        return RedirectToAction("CMEdit", new { invoiceId, transactionId = record.Id.Value() });
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        record.Attachments = _cargoCreditNoteManager.GetCreditMemoAttachments(creditMemoAttachmentIds);
      }

      //KeepBillingHistoryDataInStore(true);

      if (TempData.ContainsKey("correspondenceNumber"))
      {
        ViewData[ViewDataConstants.FromBillingHistory] = true;
      }
      record.Invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View(record);
    }

    /// <summary>
    /// BMs the edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    public ActionResult CMEdit(string invoiceId, string transactionId)
    {
      var creditMemoRecord = GetCreditMemoRecord(invoiceId, transactionId);

      ViewData[ViewDataConstants.BreakdownExists] = _cargoCreditNoteManager.GetCreditMemoAwbBreakdownCount(transactionId) > 0 ? true : false;

      var creditMemoAwbGrid = new CargoCMAwbGrid(ViewDataConstants.CreditMemoAwbGrid, Url.Action("CreditMemoAwbGridData", "CreditNote", new { transactionId }));
      ViewData[ViewDataConstants.CreditMemoAwbGrid] = creditMemoAwbGrid.Instance;

      return View(creditMemoRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CMEdit(string invoiceId, string transactionId, CargoCreditMemo creditMemoRecord)
    {
      var couponAttachmentIds = creditMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
        creditMemoRecord.InvoiceId = invoiceId.ToGuid();
        creditMemoRecord.Id = transactionId.ToGuid();

        _cargoCreditNoteManager.UpdateCreditMemoRecord(creditMemoRecord);

        ShowSuccessMessage(Messages.CMUpdateSuccessful);

        return RedirectToAction("CMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoRecord.Attachments = _cargoCreditNoteManager.GetCreditMemoAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      creditMemoRecord.Invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoAwbGrid = new CargoCMAwbGrid(ControlIdConstants.CreditMemoAwbGrid, Url.Action(CMAwbGridAction, new { transactionId }));
      ViewData[ViewDataConstants.CreditMemoAwbGrid] = creditMemoAwbGrid.Instance;

      return View("CMEdit", creditMemoRecord);
    }

    [HttpGet]
     [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.View)]
    public ActionResult CMView(string invoiceId, string transactionId)
    {

      var creditMemoRecord = GetCreditMemoRecord(invoiceId, transactionId);
      ViewData[ViewDataConstants.BreakdownExists] = _cargoCreditNoteManager.GetCreditMemoAwbBreakdownCount(transactionId) > 0 ? true : false;
      // Initialize CMAwb grid
      var creditMemoAwbGrid = new CargoCMAwbGrid(ViewDataConstants.CreditMemoAwbGrid, Url.Action("CreditMemoAwbGridData", "CreditNote", new { transactionId }));
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
      var creditMemoAwbGrid = new CargoCMAwbGrid(ControlIdConstants.CreditMemoAwbGrid, Url.Action("CreditMemoAwbData", "CreditNote", new { transactionId }));
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
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Download)]
    [HttpGet]
    public FileStreamResult CreditMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoCreditNoteManager.GetCreditMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Download  Credit Memo Awb attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Download)]
    [HttpGet]
    public FileStreamResult CreditMemoAwbAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoCreditNoteManager.GetCMAwbAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Upload  Credit Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult CreditMemoAwbAttachmentUpload(string invoiceId, string transactionId)
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
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

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
              // Convert file size to KB.
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

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.CGO_CREDIT_MEMO)]
    public ActionResult CreditMemoDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        var cm = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        var invoiceId = cm.InvoiceId.ToString();

        //Delete record
        var isDeleted = _cargoCreditNoteManager.DeleteCreditMemoRecord(transactionId);

        details = GetDeleteMessage(isDeleted, Url.Action("Edit", new { invoiceId }));

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
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

    [HttpPost]
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
  //  [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "couponId", TableName = TransactionTypeTable.)]
    public ActionResult CMAwbProrateLadderDetailDelete(string couponId)
    {
      var prorateLadderDetailId = couponId;
      UIMessageDetail details;
      Guid awbCouponId;
      try
      {
        // Delete record
        bool isParentdelete;
        var isDeleted = _cargoCreditNoteManager.DeleteAwbProrateLadderDetailRecord(prorateLadderDetailId, out isParentdelete, out awbCouponId);
        if (isDeleted)
          ShowErrorMessage(Messages.DeleteSuccessful, true);
        return RedirectToAction("CMAwbProrateLadderCreate", new { couponId = awbCouponId, prorateLadderDetailId });
      }
      catch (ISBusinessException ex)
      {
        ShowErrorMessage(ex.ErrorCode);
      }
      return RedirectToAction("CMAwbProrateLadderCreate");

    }

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CMAwbProrateLadderCreate(string couponId, string prorateLadderId)
    {
      SetViewDataPageMode(PageMode.Create);
      
      var creditMemoAwbGrid = new CMAwbProrateLadderDetailGrid(ViewDataConstants.CreditMemoAwbProrateLadderGrid, Url.Action("CreditMemoAwbProrateLadderGridData", "CreditNote", new { prorateLadderId }));
      ViewData[ViewDataConstants.CreditMemoAwbProrateLadderGrid] = creditMemoAwbGrid.Instance;
      CMAwbProrateLadderDetail awbPLDetail = new CMAwbProrateLadderDetail();
      
      return View(awbPLDetail);
    }

    public JsonResult CreditMemoAwbProrateLadderGridData(string prorateLadderId)
    {
      // Create grid instance and retrieve data from database
      var cmAwbProrateLadderGrid = new CMAwbProrateLadderDetailGrid(ViewDataConstants.CreditMemoAwbProrateLadderGrid, Url.Action("CreditMemoAwbProrateLadderGridData", "CreditNote", new { prorateLadderId }));
      ViewData[ViewDataConstants.CreditMemoAwbProrateLadderGrid] = cmAwbProrateLadderGrid.Instance;
      if (!string.IsNullOrEmpty(prorateLadderId))
      {
        var prorateLadderGuid = prorateLadderId.ToGuid();
        var bmAwbProrateLadderDetails = _cargoCreditNoteManager.GetCMAwbProrateLadderDetailList(prorateLadderGuid).AsQueryable();
        try
        {
          return cmAwbProrateLadderGrid.DataBind(bmAwbProrateLadderDetails);
        }
        catch (ISBusinessException be)
        {
          ViewData["errorMessage"] = be.ErrorCode;
          return null;
        }
      }
      else
      {
        return null;
      }
    }

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    public ActionResult CMAwbProrateLadderCreate(string couponId, string prorateLadderId, string prorateCalCurrencyId, double totalAmount, CMAwbProrateLadderDetail awbPLDetail)
    {
      try
      {
        _cargoCreditNoteManager.AddCreditMemoAwbProrateLadderDetail(awbPLDetail);
        SetViewDataPageMode(PageMode.Edit);
        
        ShowSuccessMessage("AWB Prorate Ladder details added successfully.");
        return RedirectToAction("CMAwbProrateLadderCreate", new { couponId, prorateLadderId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
      }
      catch (Exception ex)
      {
        ShowErrorMessage(ex.ToString());
      }

      var billingMemoAwbGrid = new BMAwbProrateLadderDetailGrid(ViewDataConstants.BillingMemoAwbProrateLadderGrid, Url.Action("BillingMemoAwbProrateLadderGridData", "Invoice", new { prorateLadderId }));
      ViewData[ViewDataConstants.BillingMemoAwbProrateLadderGrid] = billingMemoAwbGrid.Instance;
      return View(awbPLDetail);
    }

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Validate)]
    public ActionResult ValidateInvoice(string invoiceId)
    {
      ValidateInvoice(_cargoCreditNoteManager, invoiceId);

      return RedirectToAction("Edit");
    }

    /// <summary>
    /// CMs awb prepaid create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    public ActionResult CMAwbPrepaidCreate(string invoiceId, string transactionId)
    {
      SetViewDataPageMode(PageMode.Create);

      var cmAwb = new CMAirWayBill { LastUpdatedBy = SessionUtil.UserId };
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = InvoiceHeader;
      creditMemoRecord.BillingCode = (int)BillingCode.AWBPrepaid;
      // Set Airline flight designator to Billing Member name
      //bmAwb.AwbIssueingAirline = InvoiceHeader.BillingMember.MemberCodeAlpha;
      cmAwb.CreditMemoRecord = creditMemoRecord;

      ViewData["IsAddNewCMCoupon"] = TempData["IsAddNewCMCoupon"] != null && Convert.ToBoolean(TempData["IsAddNewCMCoupon"]);

      return View(cmAwb);
    }

    /// <summary>
    /// Gets the coupon of corresponding billingMemoId and its coupon record.
    /// </summary>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CMAwbPrepaidCreate(string invoiceId, string transactionId, CMAirWayBill record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.CarriageFromId = record.CarriageFromId.ToUpper();
      record.CarriageToId = record.CarriageToId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentOriginId))
        record.ConsignmentOriginId = record.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentDestinationId))
        record.ConsignmentDestinationId = record.ConsignmentDestinationId.ToUpper();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 15 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = (int)BillingCode.AWBPrepaid;
        record.Attachments.Clear();
        if (record.AwbSerialNumber > 0)
        {
          var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
          var awbnumber = (awbnumberDisplay.Substring(0, 7));
          //$('#AwbSerialNumber').val(awbnumber);
          record.AwbSerialNumber = Convert.ToInt32(awbnumber);
        }
        string duplicateCouponErrorMessage;
        _cargoCreditNoteManager.AddCMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
        _cargoCreditNoteManager.UpdateCMAwbAttachment(couponAttachmentIds, record.Id);
        
        ShowSuccessMessage(Messages.CMCouponCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData["IsAddNewCMCoupon"] = true;
        SetViewDataPageMode(PageMode.Create);

        // Set ViewData, "IsPostback" to false
        ViewData["IsPostback"] = false;
        return RedirectToAction("CMAwbPrepaidCreate", new { transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        SetViewDataPageMode(PageMode.Clone); // Done to hide the serial number field.
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
      }
      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = invoice;
      record.CreditMemoRecord = creditMemoRecord;

      return View(record);
    }

    /// <summary>
    /// CMs the awb prepaid create and duplicate.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbPrepaidCreate")]
    public ActionResult CMAwbPrepaidCreateAndDuplicate(string invoiceId, string transactionId, CMAirWayBill record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.CarriageFromId = record.CarriageFromId.ToUpper();
      record.CarriageToId = record.CarriageToId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentOriginId))
        record.ConsignmentOriginId = record.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentDestinationId))
        record.ConsignmentDestinationId = record.ConsignmentDestinationId.ToUpper();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 15 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = (int)BillingCode.AWBPrepaid;
        record.Attachments.Clear();
        if (record.AwbSerialNumber > 0)
        {
          var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
          var awbnumber = (awbnumberDisplay.Substring(0, 7));
          //$('#AwbSerialNumber').val(awbnumber);
          record.AwbSerialNumber = Convert.ToInt32(awbnumber);
        }
        string duplicateCouponErrorMessage;
        _cargoCreditNoteManager.AddCMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
        _cargoCreditNoteManager.UpdateCMAwbAttachment(couponAttachmentIds, record.Id);
        
        ShowSuccessMessage(Messages.CMAwbCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        SetViewDataPageMode(PageMode.Clone);
        record.AwbIssueingAirline = string.Empty;
        record.AwbSerialNumber = 0;
        record.AwbCheckDigit = 0;
        record.Id = Guid.Empty;
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        record.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        ViewData["IsPostback"] = true;
        ShowErrorMessage(businessException.ErrorCode);
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
      }
      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = invoice;
      record.CreditMemoRecord = creditMemoRecord;
      return View("CMAwbPrepaidCreate", record);
    }

    /// <summary>
    /// CMs the awb prepaid create and return.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbPrepaidCreate")]
    public ActionResult CMAwbPrepaidCreateAndReturn(string invoiceId, string transactionId, CMAirWayBill record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.CarriageFromId = record.CarriageFromId.ToUpper();
      record.CarriageToId = record.CarriageToId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentOriginId))
        record.ConsignmentOriginId = record.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentDestinationId))
        record.ConsignmentDestinationId = record.ConsignmentDestinationId.ToUpper();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 15 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = (int)BillingCode.AWBPrepaid;
        record.Attachments.Clear();
        if (record.AwbSerialNumber > 0)
        {
          var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
          var awbnumber = (awbnumberDisplay.Substring(0, 7));
          //$('#AwbSerialNumber').val(awbnumber);
          record.AwbSerialNumber = Convert.ToInt32(awbnumber);
        }
        string duplicateCouponErrorMessage;
        _cargoCreditNoteManager.AddCMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
        _cargoCreditNoteManager.UpdateCMAwbAttachment(couponAttachmentIds, record.Id);
        
        ShowSuccessMessage(Messages.CMAwbCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData["IsAddNewCMCoupon"] = true;

        return RedirectToAction("CMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);

        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
      }
      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = invoice;
      record.CreditMemoRecord = creditMemoRecord;

      return View("CMAwbPrepaidCreate", record);
    }

    /// <summary>
    /// CMs the awb edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpGet]
    public ActionResult CMAwbEdit(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
      {
          Session["helplinkurl"] = "Cm_Awb_Prepaid_edit";
          return View("CMAwbPrepaidEdit", awbRecord);
      }
      Session["helplinkurl"] = "Cm_Awb_ChargeCollect_edit";
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
    /// CMs the awb prepaid edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <returns></returns>
    [HttpGet]
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    public ActionResult CMAwbPrepaidEdit(string invoiceId, string transactionId, string couponId)
    {
      var cmAwbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);

      return View(cmAwbRecord);
    }

    /// <summary>
    /// BMs the awb prepaid edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The bm awb id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CMAwbPrepaidEdit(string invoiceId, string transactionId, string couponId, CMAirWayBill record)
    {
      var creditMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 15 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);
        //Assign parent id for VAt records
        EditCMAwb(record, couponId, invoiceId);

        ShowSuccessMessage(Messages.CMAwbUpdateSuccessful);
        return RedirectToAction("CMAwbPrepaidCreate", new { invoiceId, transactionId, couponId });
      }
      catch (ISBusinessException be)
      {
        ShowErrorMessage(be.ErrorCode);
        var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        billingMemo.Invoice = invoice;
        record.CreditMemoRecord = billingMemo;
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(creditMemoAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
        record.Id = couponId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return View(record);
    }

    private void EditCMAwb(CMAirWayBill cmAwb, string couponId, string invoiceId)
    {
      cmAwb.Id = couponId.ToGuid();
      cmAwb.LastUpdatedBy = SessionUtil.UserId;
      // Assign parent rejection coupon record id to tax records
      foreach (var otherCharge in cmAwb.CMAwbOtherCharges)
      {
        otherCharge.ParentId = cmAwb.Id;
      }
      // Assign parent rejection coupon record id to vat records
      foreach (var vat in cmAwb.CMAwbVatBreakdown)
      {
        vat.ParentId = cmAwb.Id;
      }

      // Assign parent BM AWB record id to prorate ladder detail records.
      foreach (var prorateLadderDetail in cmAwb.CMAwbProrateLadder)
      {
        prorateLadderDetail.ParentId = cmAwb.Id;
      }

      string duplicateErrorMessage;

      int vatRecordCountBefore = cmAwb.CMAwbVatBreakdown.Count;
      int otherChargeCountBefore = cmAwb.CMAwbOtherCharges.Count;
      _cargoCreditNoteManager.UpdateCMAwbRecord(cmAwb, invoiceId, out duplicateErrorMessage);
      int vatRecordCountAfter = cmAwb.CMAwbVatBreakdown.Count;
      int otherChargeCountAfter = cmAwb.CMAwbOtherCharges.Count;

      ShowSuccessMessages(Messages.CMAwbUpdateSuccessful, vatRecordCountBefore, vatRecordCountAfter, otherChargeCountBefore, otherChargeCountAfter);
      if (!String.IsNullOrEmpty(duplicateErrorMessage))
        ShowErrorMessage(duplicateErrorMessage, true);
    }

    private void ShowSuccessMessages(string message, int vatRecordCountBefore, int vatRecordCountAfter, int otherChargeCountBefore, int otherChargeCountAfter)
    {
      if (CheckIfVatBreakdownDeleted(vatRecordCountBefore, vatRecordCountAfter))
      {
        message += string.Format(" {0}", Messages.VatRecordsDeletedInfo);
      }

      if (CheckIfOtherChargeBreakdownDeleted(otherChargeCountBefore, otherChargeCountAfter))
      {
        if (!message.Contains("NOTE"))
          message += " NOTE:" + string.Format(" {0}", Messages.OtherChargeRecordsDeletedInfo);
        else
          message += string.Format(" {0}", Messages.OtherChargeRecordsDeletedInfo);
      }

      ShowSuccessMessage(message);
    }

    private static bool CheckIfVatBreakdownDeleted(int vatCountBefore, int vatCountAfter)
    {
      return vatCountBefore > vatCountAfter;
    }

    private static bool CheckIfOtherChargeBreakdownDeleted(int otherChargeCountBefore, int otherChargeCountAfter)
    {
      return otherChargeCountBefore > otherChargeCountAfter;
    }

    /// <summary>
    /// CMs the awb charge collect create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    public ActionResult CMAwbChargeCollectCreate(string invoiceId, string transactionId)
    {
      SetViewDataPageMode(PageMode.Create);

      var cmAwb = new CMAirWayBill { LastUpdatedBy = SessionUtil.UserId };
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = InvoiceHeader;
      creditMemoRecord.BillingCode = (int)BillingCode.AWBChargeCollect;
      cmAwb.CreditMemoRecord = creditMemoRecord;

      ViewData["IsAddNewCMCoupon"] = TempData["IsAddNewCMCoupon"] != null && Convert.ToBoolean(TempData["IsAddNewCMCoupon"]);

      return View(cmAwb);
    }

    /// <summary>
    /// BMs the awb charge collect create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CMAwbChargeCollectCreate(string invoiceId, string transactionId, CMAirWayBill record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.CarriageFromId = record.CarriageFromId.ToUpper();
      record.CarriageToId = record.CarriageToId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentOriginId))
        record.ConsignmentOriginId = record.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentDestinationId))
        record.ConsignmentDestinationId = record.ConsignmentDestinationId.ToUpper();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 16 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = (int)BillingCode.AWBChargeCollect;
        record.Attachments.Clear();
        if (record.AwbSerialNumber > 0)
        {
          var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
          var awbnumber = (awbnumberDisplay.Substring(0, 7));
          //$('#AwbSerialNumber').val(awbnumber);
          record.AwbSerialNumber = Convert.ToInt32(awbnumber);
        }
        string duplicateCouponErrorMessage;
        _cargoCreditNoteManager.AddCMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
        _cargoCreditNoteManager.UpdateCMAwbAttachment(couponAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.CMAwbCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData["IsAddNewCMCoupon"] = true;

        return RedirectToAction("CMAwbChargeCollectCreate", new { transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);

        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
      }
      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = invoice;
      record.CreditMemoRecord = creditMemoRecord;

      return View(record);
    }

    /// <summary>
    /// CMs the awb charge collect edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <returns></returns>
    [HttpGet]
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    public ActionResult CMAwbChargeCollectEdit(string invoiceId, string transactionId, string couponId)
    {
      var cmAwbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      return View(cmAwbRecord);
    }

    /// <summary>
    /// BMs the awb charge collect edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CMAwbChargeCollectEdit(string invoiceId, string transactionId, string couponId, CMAirWayBill record)
    {
      var creditMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 16 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);
        EditCMAwb(record, couponId, invoiceId);

        ShowSuccessMessage(Messages.CMAwbUpdateSuccessful);
        return RedirectToAction("CMAwbChargeCollectCreate", new { invoiceId, transactionId });
      }
      catch (ISBusinessException be)
      {
        ShowErrorMessage(be.ErrorCode);
        var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        billingMemo.Invoice = invoice;
        record.CreditMemoRecord = billingMemo;
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(creditMemoAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
        record.Id = couponId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }
      return View(record);
    }

    /// <summary>
    /// BMs the awb charge collect create and duplicate.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbChargeCollectCreate")]
    public ActionResult CMAwbChargeCollectDuplicate(string invoiceId, string transactionId, CMAirWayBill record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.CarriageFromId = record.CarriageFromId.ToUpper();
      record.CarriageToId = record.CarriageToId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentOriginId))
        record.ConsignmentOriginId = record.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentDestinationId))
        record.ConsignmentDestinationId = record.ConsignmentDestinationId.ToUpper();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 16 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = (int)BillingCode.AWBChargeCollect;
        record.Attachments.Clear();
        if (record.AwbSerialNumber > 0)
        {
          var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
          var awbnumber = (awbnumberDisplay.Substring(0, 7));
          //$('#AwbSerialNumber').val(awbnumber);
          record.AwbSerialNumber = Convert.ToInt32(awbnumber);
        }
        string duplicateCouponErrorMessage;
        _cargoCreditNoteManager.AddCMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
        _cargoCreditNoteManager.UpdateCMAwbAttachment(couponAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.CMAwbCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);
        ViewData["IsPostback"] = false;
        SetViewDataPageMode(PageMode.Clone);
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        record.AwbIssueingAirline = string.Empty;
        record.AwbSerialNumber = 0;
        record.AwbCheckDigit = 0;
        record.Id = Guid.Empty;
        record.Attachments.Clear(); 
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);

        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
      }
      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = invoice;
      record.CreditMemoRecord = creditMemoRecord;

      return View("CMAwbChargeCollectCreate", record);
    }

    /// <summary>
    /// BMs the awb delete.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpPost]
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.CGO_CM_AWB)]
    public JsonResult CMAwbDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        Guid invoiceId;
        Guid creditMemoId;
        var isDeleted = _cargoCreditNoteManager.DeleteCreditMemoAwbRecord(transactionId, out creditMemoId, out invoiceId);

        details = isDeleted ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, RedirectUrl = Url.Action("CMEdit", new { invoiceId, transactionId = creditMemoId }), isRedirect = true } : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
        if (isDeleted)
          ShowSuccessMessage(Messages.DeleteSuccessful, true);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.View)]
    [HttpGet]
    public ActionResult CMAwbView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
        return RedirectToAction("CMAwbPrepaidView", new {invoiceId, transactionId, couponId});

      return RedirectToAction("CMAwbChargeCollectView", new { invoiceId, transactionId, couponId });
    }

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.View)]
    [HttpGet]
    public ActionResult CMAwbPrepaidView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      return View("CMAwbPrepaidEdit", awbRecord);
    }

    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.View)]
    [HttpGet]
    public ActionResult CMAwbChargeCollectView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetCMAwbRecord(invoiceId, transactionId, couponId);
      return View("CMAwbChargeCollectEdit", awbRecord);
    }

    /// <summary>
    /// BMs the awb charge collect create and return.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbChargeCollectCreate")]
    public ActionResult CMAwbChargeCollectCreateAndReturn(string invoiceId, string transactionId, CMAirWayBill record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.CarriageFromId = record.CarriageFromId.ToUpper();
      record.CarriageToId = record.CarriageToId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentOriginId))
        record.ConsignmentOriginId = record.ConsignmentOriginId.ToUpper();
      if (!string.IsNullOrEmpty(record.ConsignmentDestinationId))
        record.ConsignmentDestinationId = record.ConsignmentDestinationId.ToUpper();
      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 16 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.CreditMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.AwbBillingCode = (int)BillingCode.AWBChargeCollect;
        record.Attachments.Clear();
        if (record.AwbSerialNumber > 0)
        {
          var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
          var awbnumber = (awbnumberDisplay.Substring(0, 7));
          record.AwbSerialNumber = Convert.ToInt32(awbnumber);
        }
        string duplicateCouponErrorMessage;
        _cargoCreditNoteManager.AddCMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
        _cargoCreditNoteManager.UpdateCMAwbAttachment(couponAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.CMAwbCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData["IsAddNewCMCoupon"] = true;

        return RedirectToAction("CMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        ViewData["IsPostback"] = true;
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
      }
      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoRecord = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoRecord.Invoice = invoice;
      record.CreditMemoRecord = creditMemoRecord;

      return View("CMAwbChargeCollectCreate", record);
    }

    /// <summary>
    /// 
    /// </summary>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbChargeCollectCreate")]
    public ActionResult CMChargeCollectAwbEditAndReturn(string couponId, string transactionId, string invoiceId, CMAirWayBill record)
    {
      record.CreditMemoId = transactionId.ToGuid();
      var awbAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);

      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 16 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        EditCMAwb(record, couponId, invoiceId);
        //  TempData["RMCouponRecord"] = "";
        return RedirectToAction("CMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var creditMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
        creditMemo.Invoice = invoice;
        record.CreditMemoRecord = creditMemo;
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(awbAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.CreditMemoId = transactionId.ToGuid();
        record.Id = couponId.ToGuid();
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("CMAwbChargeCollectEdit", record);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbChargeCollectCreate")]
    public ActionResult CMChargeCollectAwbClone(string couponId, string transactionId, string invoiceId, CMAirWayBill record)
    {
      record.CreditMemoId = transactionId.ToGuid();
      var creditMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      var creditMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);

      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      creditMemo.Invoice = invoice;
      record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);

      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 16 */
        MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
        record.Id = couponId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent rejection coupon record id to tax records
        foreach (var awbOtherCharge in record.CMAwbOtherCharges)
        {
          awbOtherCharge.ParentId = record.Id;
        }
        // Assign parent rejection coupon record id to vat records
        foreach (var vat in record.CMAwbVatBreakdown)
        {
          vat.ParentId = record.Id;
        }
        string duplicateErrorMessage;
        _cargoCreditNoteManager.UpdateCMAwbRecord(record, invoiceId, out duplicateErrorMessage);
        record.CreditMemoRecord = creditMemo;

        ShowSuccessMessage(Messages.CMAwbUpdateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        record.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;

        return View("CMAwbChargeCollectCreate", record);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        record.CreditMemoRecord = creditMemo;
        record.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(creditMemoAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("CMAwbChargeCollectEdit", record);
    }

    /// <summary>
    /// Credit Memo Awb attachment download.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.Download)]
    [HttpGet]
    public FileStreamResult CMAwbAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoCreditNoteManager.GetCMAwbAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Credit Memo Prepaid Awb Edit and Return
    /// </summary>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]  
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbPrepaidCreate")]
    public ActionResult CMPrepaidAwbEditAndReturn(string couponId, string transactionId, string invoiceId, CMAirWayBill cmAwb)
    {
      cmAwb.CreditMemoId = transactionId.ToGuid();
      var awbAttachmentIds = cmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      cmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);

      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 15 */
        MemberManager.ValidateIssuingAirline(cmAwb.AwbIssueingAirline);        
        EditCMAwb(cmAwb, couponId, invoiceId);
        return RedirectToAction("CMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var creditMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
        creditMemo.Invoice = invoice;
        cmAwb.CreditMemoRecord = creditMemo;
        cmAwb.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(awbAttachmentIds);
        // SCP190774: ERROR SAVING RM
        cmAwb.CreditMemoId = transactionId.ToGuid();
        cmAwb.Id = couponId.ToGuid();
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("CMAwbPrepaidEdit", cmAwb);
    }

    [HttpGet]
    public JsonResult ValidateFromToSectors(string fromSectorCode, string toSectorCode)
    {
      UIMessageDetail uiMessageDetail;

      if (!string.IsNullOrEmpty(fromSectorCode) && !string.IsNullOrEmpty(toSectorCode) && fromSectorCode.Trim().Equals(toSectorCode.Trim()))
      {
        uiMessageDetail = new UIMessageDetail()
        {
          IsFailed = true,
          ErrorCode = Messages.ResourceManager.GetString(CargoErrorCodes.InvalidSectorCombination)
        };
      }
      else if (!string.IsNullOrEmpty(fromSectorCode) && !_referenceManager.IsValidAirportCode(fromSectorCode))
      {
        uiMessageDetail = new UIMessageDetail()
        {
          IsFailed = true,
          ErrorCode = Messages.ResourceManager.GetString(CargoErrorCodes.InvalidFromSectorCode)
        };
      }

      else if (!string.IsNullOrEmpty(toSectorCode) && !_referenceManager.IsValidAirportCode(toSectorCode))
      {
        uiMessageDetail = new UIMessageDetail()
        {
          IsFailed = true,
          ErrorCode = Messages.ResourceManager.GetString(CargoErrorCodes.InvalidToSectorCode)
        };
      }
      else
      {
        uiMessageDetail = new UIMessageDetail()
        {
          IsFailed = false
        };
      }

      return Json(uiMessageDetail, JsonRequestBehavior.AllowGet);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    // SCP#419602: SRM: CGO/MISC/UATP and Billing history permissions
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMAwbPrepaidCreate")]
    public ActionResult CMPrepaidAwbClone(string couponId, string transactionId, string invoiceId, CMAirWayBill cmAwb)
    {
      cmAwb.CreditMemoId = transactionId.ToGuid();
      var couponAttachmentIds = cmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      var creditMemo = _cargoCreditNoteManager.GetCreditMemoRecordDetails(transactionId);

      var invoice = _cargoCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      creditMemo.Invoice = invoice;
      cmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);

      try
      {
        // SCP190774: ERROR SAVING RM
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 15 */
        MemberManager.ValidateIssuingAirline(cmAwb.AwbIssueingAirline);
        cmAwb.Id = couponId.ToGuid();
        cmAwb.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent rejection coupon record id to tax records
        foreach (var awbOtherCharge in cmAwb.CMAwbOtherCharges)
        {
          awbOtherCharge.ParentId = cmAwb.Id;
        }
        // Assign parent rejection coupon record id to vat records
        foreach (var vat in cmAwb.CMAwbVatBreakdown)
        {
          vat.ParentId = cmAwb.Id;
        }
        string duplicateErrorMessage;
        _cargoCreditNoteManager.UpdateCMAwbRecord(cmAwb, invoiceId, out duplicateErrorMessage);
        cmAwb.CreditMemoRecord = creditMemo;

        ShowSuccessMessage(Messages.CMAwbUpdateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        cmAwb.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;

        return View("CMAwbPrepaidCreate", cmAwb);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        cmAwb.CreditMemoRecord = creditMemo;
        cmAwb.Attachments = _cargoCreditNoteManager.GetCMAwbAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("CMAwbPrepaidEdit", cmAwb);
    }
  } 
}
