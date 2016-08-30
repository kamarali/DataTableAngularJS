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
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Common;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class CreditNoteController : PaxInvoiceControllerBase
  {
    private readonly INonSamplingCreditNoteManager _nonSamplingCreditNoteManager;
    private readonly IReferenceManager _referenceManager;
    private const string CMCouponGridAction = "CMCouponGridData";
    private const string CreditMemoGridAction = "CreditMemoGridData";
    private const string SourceCodeGridAction = "SourceCodeGridData";

    public CreditNoteController(INonSamplingCreditNoteManager nonSamplingCreditNoteManager, IReferenceManager referenceManager, IMemberManager memberManager) : base(nonSamplingCreditNoteManager)
    {
      _nonSamplingCreditNoteManager = nonSamplingCreditNoteManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.NonSampling); }
    }

    protected override InvoiceType InvoiceType
    {
      get { return InvoiceType.CreditNote; }
    }

    //
    // GET: /Pax/CreditNote/Create
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    public ActionResult Create()
    {
      SessionUtil.InvoiceSearchCriteria = null;

      var invoice = new PaxInvoice { InvoiceType = InvoiceType.CreditNote, InvoiceDate = DateTime.UtcNow, BillingMemberId = SessionUtil.MemberId, LastUpdatedBy = SessionUtil.UserId};
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

    //
    // POST: /Pax/CreditNote/Create
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    public ActionResult Create(PaxInvoice invoice)
    {
      try
      {
        invoice.InvoiceDate = DateTime.UtcNow;
        invoice.InvoiceType = InvoiceType.CreditNote;
        invoice.BillingCode = Convert.ToInt32(BillingCode.NonSampling);
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.SubmissionMethod = SubmissionMethod.IsWeb;
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
        var creditNoteHeader = _nonSamplingCreditNoteManager.CreateInvoice(invoice);

        ShowSuccessMessage(Messages.CreditNoteCreateSuccessful);

        return RedirectToAction("Edit", new { invoiceId = creditNoteHeader.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
          /* CMP #624: ICH Rewrite-New SMI X 
        * Description: As per ICH Web Service Response Message specifications 
        * Refer FRS Section 3.3 Table 9. 
        * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

          invoice.BilledMember = _nonSamplingCreditNoteManager.GetBilledMember(invoice.BilledMemberId);

          if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowSmiXWebServiceErrorMessage(businessException.Message);
          }
          else //if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase))
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
          var billingMemberLocationInfo = invoice.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
          if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
          {
            ViewData["IsLegalTextSet"] = true;
          }
        }
      }
      MakeInvoiceRenderReady(invoice.Id, invoice);
      
      return View(invoice);
    }

    //
    // GET: /Pax/CreditNote/Edit/5
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    public ActionResult Edit(string invoiceId)
    {
      try
      {
        if (string.IsNullOrEmpty(InvoiceHeader.BillingMemberLocationCode))
        {
          InvoiceHeader.BillingMemberLocationCode = "-";
        }
        if (string.IsNullOrEmpty(InvoiceHeader.BilledMemberLocationCode))
        {
          InvoiceHeader.BilledMemberLocationCode = "-";
        }

        ViewData[ViewDataConstants.TransactionExists] = _nonSamplingCreditNoteManager.IsCreditMemoExists(invoiceId);
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }

      MakeInvoiceRenderReady(invoiceId.ToGuid(), InvoiceHeader);
      // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is
      // not retrieve from database.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      //Initialize source code grid
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;

      // Initialize Credit Memo grid
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;

      // Get all submitted errors.
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      ViewData[ViewDataConstants.SubmittedErrorsGrid] = submittedErrorsGrid.Instance;

      if (InvoiceHeader.MemberLocationInformation != null)
      {
        var billingMemberLocationInfo = InvoiceHeader.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
        if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
        {
          ViewData["IsLegalTextSet"] = true;
        }
      }

      return View(InvoiceHeader);
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.View)]
    public new ActionResult View(string invoiceId)
    {
      ViewData[ViewDataConstants.TransactionExists] = _nonSamplingCreditNoteManager.IsCreditMemoExists(invoiceId);
      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is
      // not retrieve from database.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      //Initialize source code grid
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
      //Initialize Credit Memo grid
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;

      // If BillingType is Payables instantiate SourceCode Vat Total grid
      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
        ViewData["VatGrid"] = availableVatGrid.Instance;
      }

      return View("Edit", InvoiceHeader);
    }

    //
    // POST: /Pax/CreditNote/Edit/5
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", TableName = TransactionTypeTable.INVOICE, IsJson = false)]
    public ActionResult Edit(string invoiceId, PaxInvoice invoice)
    {
      try
      {
        invoice.Id = invoiceId.ToGuid();
        invoice.InvoiceType = InvoiceType.CreditNote;
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.BillingMemberId = SessionUtil.MemberId;
        invoice.SubmissionMethod = SubmissionMethod.IsWeb;
        invoice.LastUpdatedBy = SessionUtil.UserId;
        _nonSamplingCreditNoteManager.UpdateInvoice(invoice);

        ShowSuccessMessage(Messages.CreditNoteUpdateSuccessful);

        return RedirectToAction("Edit", new { invoiceId });
      }
      catch (ISBusinessException businessException)
      {
          /* CMP #624: ICH Rewrite-New SMI X 
            * Description: As per ICH Web Service Response Message specifications 
            * Refer FRS Section 3.3 Table 9. 
            * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

          invoice.BilledMember = _nonSamplingCreditNoteManager.GetBilledMember(invoice.BilledMemberId);

          if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowSmiXWebServiceErrorMessage(businessException.Message);
          }
          else //if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowErrorMessage(businessException.ErrorCode);
          }
        
          SetViewDataPageMode(PageMode.Edit);

          if (invoice.MemberLocationInformation != null)
          {
              var billingMemberLocationInfo = invoice.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
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
        invoice.InvoiceTotalRecord = new InvoiceTotal();
      }

      //Initialize source code grid
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
      //Initialize Credit Memo grid
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;
      SetViewDataPageMode(PageMode.Edit);

      return View(invoice);
    }

    /// <summary>
    /// Upload  Credit tMemo Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult CreditMemoAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<CreditMemoAttachment>();
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
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            //Get the invoice header details
         var invoice = _nonSamplingCreditNoteManager.GetInvoiceDetailForFileUpload(invoiceId);

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };
         
          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }
          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }

          if (fileUploadHelper.SaveFile())
          {
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new CreditMemoAttachment
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

            attachment = _nonSamplingCreditNoteManager.AddCreditMemoAttachment(attachment);

            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            // assign user info from session and file server info.
            if (attachment.UploadedBy==null)
              {
                  attachment.UploadedBy=new User();
              }
            attachment.UploadedBy.Id = SessionUtil.UserId;
            attachment.UploadedBy.FirstName = SessionUtil.Username; 
            attachment.FileServer = fileUploadHelper.FileServerInfo;
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
    /// Download  Credit Memo attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionId">Transaction id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Download)]
    [HttpGet]
    public FileStreamResult CreditMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingCreditNoteManager.GetCreditMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    //
    // GET: /Pax/CreditNote/{invoiceId}/CreditNoteDetails
    [Obsolete]
    public ActionResult CreditNoteDetails(string invoiceId)
    {
      var invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      // Creating empty InvoiceTotalRecord object in case if InvoiceTotal Records is
      // not retrieve from database.
      if (invoice.InvoiceTotalRecord == null)
      {
        invoice.InvoiceTotalRecord = new InvoiceTotal();
      }

      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;

      return View(invoice);
    }

    //
    // GET: /Pax/CreditNote/{invoiceId}/CreditMemoList
    [Obsolete]
    public ActionResult CreditMemoList(string invoiceId)
    {
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.CreditMemoGrid] = creditMemoGrid.Instance;

      var invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View(invoice);
    }

    public JsonResult CreditMemoGridData(string invoiceId)
    {
      var creditMemoGrid = new CreditMemoGrid(ControlIdConstants.CreditMemoGrid, Url.Action(CreditMemoGridAction, new { invoiceId }));

      var creditMemoList = _nonSamplingCreditNoteManager.GetCreditMemoList(invoiceId);

      return creditMemoGrid.DataBind(creditMemoList.AsQueryable());
    }

    //
    //GET: /Pax/CreditNote/{invoiceId}/CreditMemoCreate
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CreditMemoCreate(string invoiceId)
    {
      // Set ViewData, "IsPostback" to false
      ViewData[ViewDataConstants.IsPostback] = false;
      
      SetViewDataPageMode(PageMode.Create);

      var creditMemo = new CreditMemo {
                                        LastUpdatedBy = SessionUtil.UserId
                                      };

      var sourceCodeList = _referenceManager.GetSourceCodeList(Convert.ToInt32(TransactionType.CreditMemo));

      // For pre-population of the only source code for Credit Memo.
      if (sourceCodeList.Count != 0)
      {
        creditMemo.SourceCodeId = sourceCodeList[0].SourceCodeIdentifier;
      }

      creditMemo.Invoice = InvoiceHeader;
      return View("CMCreate", creditMemo);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CreditMemoCreate(string invoiceId, CreditMemo creditMemoRecord)
    {
      var couponAttachmentIds = creditMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        creditMemoRecord.Attachments.Clear();
        creditMemoRecord.LastUpdatedBy = SessionUtil.UserId;
        creditMemoRecord = _nonSamplingCreditNoteManager.AddCreditMemoRecord(creditMemoRecord);
        _nonSamplingCreditNoteManager.UpdateCreditMemoAttachment(couponAttachmentIds, creditMemoRecord.Id);

        ShowSuccessMessage(Messages.CMCreateSuccessful);

        return RedirectToAction("CreditMemoEdit", new { transactionId = creditMemoRecord.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      creditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCreate", creditMemoRecord);
    }

    //
    //GET: /Pax/CreditNote/{invoiceId}/CreditMemoEdit/{creditMemoId}
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult CreditMemoEdit(string invoiceId, string transactionId)
    {
      var creditMemo = GetCreditMemo(transactionId, invoiceId);
      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingCreditNoteManager.GetCreditMemoCouponBreakdownCount(transactionId) > 0 ? true : false;

      return View("CMEdit", creditMemo);
    }

    private CreditMemo GetCreditMemo(string transactionId, string invoiceId)
    {
      var creditMemo = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemo.LastUpdatedBy = SessionUtil.UserId;
      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(creditMemo.ReasonCode, (int) TransactionType.CreditMemo).CouponAwbBreakdownMandatory;

      // Set Coupon breakdown value
      creditMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      creditMemo.Invoice = InvoiceHeader;
      var creditMemoCouponGrid = new CreditMemoCouponGrid(ControlIdConstants.CreditMemoCouponGrid, Url.Action(CMCouponGridAction, new { transactionId }));
      ViewData[ViewDataConstants.CMCouponGrid] = creditMemoCouponGrid.Instance;

      return creditMemo;
    }
    
    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.View)]
    [HttpGet]
    public ActionResult CreditMemoView(string invoiceId, string transactionId)
    {
      var creditMemo = GetCreditMemo(transactionId, invoiceId);
      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingCreditNoteManager.GetCreditMemoCouponBreakdownCount(transactionId) > 0 ? true : false;
      return View("CMEdit", creditMemo);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CreditMemoEdit(string invoiceId, string transactionId, CreditMemo creditMemoRecord)
    {
      var couponAttachmentIds = creditMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
        creditMemoRecord.InvoiceId = invoiceId.ToGuid();
        creditMemoRecord.Id = transactionId.ToGuid();

        _nonSamplingCreditNoteManager.UpdateCreditMemoRecord(creditMemoRecord);

        ShowSuccessMessage(Messages.CMUpdateSuccessful);

        return RedirectToAction("CreditMemoEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      creditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);
      var creditMemoCouponGrid = new CreditMemoCouponGrid(ControlIdConstants.CreditMemoCouponGrid, Url.Action(CMCouponGridAction, new { transactionId }));
      ViewData[ViewDataConstants.CMCouponGrid] = creditMemoCouponGrid.Instance;

      return View("CMEdit", creditMemoRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_CREDIT_MEMO)]
    public ActionResult CreditMemoDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        var cm = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        var invoiceId = cm.InvoiceId.ToString();

        //Delete record
        var isDeleted = _nonSamplingCreditNoteManager.DeleteCreditMemoRecord(transactionId);

        details = GetDeleteMessage(isDeleted, Url.Action("Edit", new { invoiceId }));

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    public JsonResult CMCouponGridData(string transactionId)
    {
      var creditMemoCouponGrid = new CreditMemoCouponGrid(ControlIdConstants.CreditMemoCouponGrid, Url.Action(CMCouponGridAction, new { transactionId }));
      var creditMemoCouponBreakdownList = _nonSamplingCreditNoteManager.GetCreditMemoCouponBreakdownList(transactionId);

      return creditMemoCouponGrid.DataBind(creditMemoCouponBreakdownList.AsQueryable());
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CreditMemoCouponCreate(string transactionId, string invoiceId)
    {
      SetViewDataPageMode(PageMode.Create);

      var couponCreditBreakdownRecord = new CMCoupon {
                                                       LastUpdatedBy = SessionUtil.UserId,
                                                       CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId)
                                                     };
      couponCreditBreakdownRecord.CreditMemoRecord.Invoice = InvoiceHeader;
      couponCreditBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
      // Set Airline flight designator to Billing Member name
      couponCreditBreakdownRecord.AirlineFlightDesignator = couponCreditBreakdownRecord.CreditMemoRecord.Invoice.BillingMember.MemberCodeAlpha;
      return View("CMCouponCreate", couponCreditBreakdownRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult CreditMemoCouponEdit(string invoiceId, string transactionId, string couponId)
    {
      var couponCreditBreakdownRecord = GetCouponCreditBreakdownRecord(couponId, transactionId, invoiceId);
      // Set Airline flight designator to Billing Member name
      couponCreditBreakdownRecord.AirlineFlightDesignator = couponCreditBreakdownRecord.CreditMemoRecord.Invoice.BillingMember.MemberCodeAlpha;
      return View("CMCouponEdit", couponCreditBreakdownRecord);
    }

    private CMCoupon GetCouponCreditBreakdownRecord(string couponId, string transactionId, string invoiceId)
    {
      var couponCreditBreakdownRecord = _nonSamplingCreditNoteManager.GetCreditMemoCouponDetails(couponId);
      couponCreditBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
      couponCreditBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      couponCreditBreakdownRecord.CreditMemoRecord.Invoice = InvoiceHeader;

      // added code to remove the extra special char
      if (!string.IsNullOrEmpty(couponCreditBreakdownRecord.ProrateSlipDetails))
        couponCreditBreakdownRecord.ProrateSlipDetails =
          couponCreditBreakdownRecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);

      return couponCreditBreakdownRecord;
    }
    
    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.View)]
    [HttpGet]
    public ActionResult CreditMemoCouponView(string invoiceId, string transactionId, string couponId)
    {
      var couponCreditBreakdownRecord = GetCouponCreditBreakdownRecord(couponId, transactionId, invoiceId);
      return View("CMCouponEdit", couponCreditBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CreditMemoCouponCreate(string invoiceId, string transactionId, CMCoupon creditMemoCouponBreakdownRecord)
    {
      var couponAttachmentIds = creditMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //SCP65997: - No tax breakdwon for invoice 2012D74226, RM 618240311
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(creditMemoCouponBreakdownRecord.TaxAmount, creditMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 4 */
        MemberManager.ValidateIssuingAirline(creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);
        creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();
        creditMemoCouponBreakdownRecord.Attachments.Clear();
        creditMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        var duplicateErrorMessage = string.Empty;
        creditMemoCouponBreakdownRecord = _nonSamplingCreditNoteManager.AddCreditMemoCouponRecord(creditMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);
        _nonSamplingCreditNoteManager.UpdateCreditMemoCouponAttachment(couponAttachmentIds, creditMemoCouponBreakdownRecord.Id);

        ShowSuccessMessage(Messages.CMCouponCreateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("CreditMemoCouponCreate", new { transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoCouponBreakdownRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCouponCreate", creditMemoCouponBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CreditMemoCouponCreate")]
    public ActionResult CreditMemoCouponCreateAndReturn(string invoiceId, string transactionId, CMCoupon creditMemoCouponBreakdownRecord)
    {
      var couponAttachmentIds = creditMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //SCP65997: - No tax breakdwon for invoice 2012D74226, RM 618240311
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(creditMemoCouponBreakdownRecord.TaxAmount, creditMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 4 */
          MemberManager.ValidateIssuingAirline(creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);
        creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();
        creditMemoCouponBreakdownRecord.Attachments.Clear();
        creditMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        var duplicateErrorMessage = string.Empty;
        creditMemoCouponBreakdownRecord = _nonSamplingCreditNoteManager.AddCreditMemoCouponRecord(creditMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);
        _nonSamplingCreditNoteManager.UpdateCreditMemoCouponAttachment(couponAttachmentIds, creditMemoCouponBreakdownRecord.Id);

        ShowSuccessMessage(Messages.CMCouponCreateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("CreditMemoEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoCouponBreakdownRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCouponCreate", creditMemoCouponBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CreditMemoCouponCreate")]
    public ActionResult CreditMemoCouponDuplicate(string invoiceId, string transactionId, CMCoupon creditMemoCouponBreakdownRecord)
    {
      var couponAttachmentIds = creditMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCP65997: - No tax breakdwon for invoice 2012D74226, RM 618240311
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(creditMemoCouponBreakdownRecord.TaxAmount, creditMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 4 */
          MemberManager.ValidateIssuingAirline(creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);
        creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();
        creditMemoCouponBreakdownRecord.Attachments.Clear();
        creditMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        var duplicateErrorMessage = string.Empty;
        creditMemoCouponBreakdownRecord = _nonSamplingCreditNoteManager.AddCreditMemoCouponRecord(creditMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);
        _nonSamplingCreditNoteManager.UpdateCreditMemoCouponAttachment(couponAttachmentIds, creditMemoCouponBreakdownRecord.Id);

        ShowSuccessMessage(Messages.CMCouponCreateSuccessful, false);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);
        creditMemoCouponBreakdownRecord.Attachments.Clear(); // Clear again. Attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoCouponBreakdownRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
      creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCouponCreate", creditMemoCouponBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult CreditMemoCouponEdit(string couponId, string transactionId, string invoiceId, CMCoupon creditMemoCouponBreakdownRecord)
    {
      var couponAttachmentIds = creditMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCP65997: - No tax breakdwon for invoice 2012D74226, RM 618240311
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(creditMemoCouponBreakdownRecord.TaxAmount, creditMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 4 */
          MemberManager.ValidateIssuingAirline(creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);
        creditMemoCouponBreakdownRecord.Id = couponId.ToGuid();
        creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();
        creditMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        var duplicateErrorMessage = string.Empty;
        _nonSamplingCreditNoteManager.UpdateCreditMemoCouponRecord(creditMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);

        ShowSuccessMessage(Messages.CMCouponUpdateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("CreditMemoCouponCreate", new { transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoCouponBreakdownRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      creditMemoCouponBreakdownRecord.Id = couponId.ToGuid();
      creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();

      creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);

      creditMemoCouponBreakdownRecord.CreditMemoRecord.InvoiceId = invoiceId.ToGuid();

      creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCouponEdit", creditMemoCouponBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMCouponEdit")]
    public ActionResult CreditMemoCouponClone(string couponId, string transactionId, string invoiceId, CMCoupon creditMemoCouponBreakdownRecord)
    {
      var couponAttachmentIds = creditMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCP65997: - No tax breakdwon for invoice 2012D74226, RM 618240311
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(creditMemoCouponBreakdownRecord.TaxAmount, creditMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 4 */
          MemberManager.ValidateIssuingAirline(creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);
        creditMemoCouponBreakdownRecord.Id = couponId.ToGuid();
        creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();
        creditMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        var duplicateErrorMessage = string.Empty;
        _nonSamplingCreditNoteManager.UpdateCreditMemoCouponRecord(creditMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);

        creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);
        creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

        ShowSuccessMessage(Messages.CMCouponUpdateSuccessful, false);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        creditMemoCouponBreakdownRecord.Attachments.Clear(); // Attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        return View("CMCouponCreate", creditMemoCouponBreakdownRecord);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoCouponBreakdownRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      creditMemoCouponBreakdownRecord.Id = couponId.ToGuid();
      creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();

      creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);

      creditMemoCouponBreakdownRecord.CreditMemoRecord.InvoiceId = invoiceId.ToGuid();

      creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCouponEdit", creditMemoCouponBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "CMCouponEdit")]
    public ActionResult CreditMemoEditAndReturn(string couponId, string transactionId, string invoiceId, CMCoupon creditMemoCouponBreakdownRecord)
    {
      var couponAttachmentIds = creditMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCP65997: - No tax breakdwon for invoice 2012D74226, RM 618240311
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(creditMemoCouponBreakdownRecord.TaxAmount, creditMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 4 */
          MemberManager.ValidateIssuingAirline(creditMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);
        creditMemoCouponBreakdownRecord.Id = couponId.ToGuid();
        creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();

        var duplicateErrorMessage = string.Empty;
        _nonSamplingCreditNoteManager.UpdateCreditMemoCouponRecord(creditMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);

        ShowSuccessMessage(Messages.CMCouponUpdateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("CreditMemoEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        creditMemoCouponBreakdownRecord.Attachments = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      creditMemoCouponBreakdownRecord.Id = couponId.ToGuid();
      creditMemoCouponBreakdownRecord.CreditMemoId = transactionId.ToGuid();

      creditMemoCouponBreakdownRecord.CreditMemoRecord = _nonSamplingCreditNoteManager.GetCreditMemoRecordDetails(transactionId);

      creditMemoCouponBreakdownRecord.CreditMemoRecord.InvoiceId = invoiceId.ToGuid();

      creditMemoCouponBreakdownRecord.CreditMemoRecord.Invoice = _nonSamplingCreditNoteManager.GetInvoiceHeaderDetails(invoiceId);

      return View("CMCouponEdit", creditMemoCouponBreakdownRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "couponId", TableName = TransactionTypeTable.PAX_CM_COUPON_BREAKDOWN)]
    public ActionResult CreditMemoCouponDelete(string couponId)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        Guid invoiceId = new Guid(), creditMemoId = new Guid();
        var isDeleted = _nonSamplingCreditNoteManager.DeleteCreditMemoCouponRecord(couponId, ref creditMemoId, ref invoiceId);

        details = GetDeleteMessage(isDeleted, Url.Action("CreditMemoEdit", new { invoiceId, transactionId = creditMemoId }));

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Upload  Credit Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult CreditMemoCouponAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<CMCouponAttachment>();
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
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Get invoice header details
         var invoice = _nonSamplingCreditNoteManager.GetInvoiceDetailForFileUpload(invoiceId);

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }

          if (fileUploadHelper.SaveFile())
          {
              files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
              var attachment = new CMCouponAttachment
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

              attachment = _nonSamplingCreditNoteManager.AddCreditMemoCouponAttachment(attachment);
              //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
              // assign user info from session and file server info.
              if (attachment.UploadedBy == null)
              {
                  attachment.UploadedBy=new User();
              }
            attachment.UploadedBy.Id = SessionUtil.UserId;
            attachment.UploadedBy.FirstName = SessionUtil.Username; 
            attachment.FileServer = fileUploadHelper.FileServerInfo;
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
    /// Download  Credit Memo Coupon attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Download)]
    [HttpGet]
    public FileStreamResult CreditMemoCouponAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingCreditNoteManager.GetCreditMemoCouponAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleCreditNote.Validate)]
    public ActionResult ValidateInvoice(string invoiceId)
    {
      ValidateInvoice(_nonSamplingCreditNoteManager, invoiceId);

      return RedirectToAction("Edit");
    }

    /// <summary>
    /// This method is use to check amount difference in tax breakdowns with total amount.
    /// Issue: 65997 - No tax breakdwon for invoice 2012D74226, RM 618240311.
    /// This validation will be applicable CM coupons.
    /// SCP105897 & SCP105226 [added Math.Round() for calculatedAmount]
    /// </summary>
    /// <param name="targetAmount">target Amount</param>
    /// <param name="cmTaxBreakdowns">CM Tax Breakdowns</param>
    /// <returns>return false if tax breakdowns are valid.</returns>
    private bool inValidTaxBreakDown(double targetAmount, IEnumerable<CMCouponTax> cmTaxBreakdowns = null)
    {
       double calculatedAmount = 0;
        // credit memo coupon tax breakdown validation
        if (cmTaxBreakdowns != null)
        {
            calculatedAmount = cmTaxBreakdowns.Aggregate<CMCouponTax, double>(0, (current, cmCouponTax) => current + cmCouponTax.Amount);
        }
      return Math.Round(calculatedAmount, 2).Equals(targetAmount) ? false : true;
    }
  }
}
