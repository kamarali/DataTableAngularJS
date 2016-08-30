using System;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using System.Collections.Generic;
using System.Web;
using System.Web.Script.Serialization;
using Iata.IS.Web.UIModel.Grid.Common;
using System.Globalization;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class FormDEController : PaxInvoiceControllerBase
  {
    private readonly ISamplingFormDEManager _samplingformDEManager;
    private readonly IReferenceManager _referenceManager;
    private const string FormDSourceCodeGridAction = "FormDSourceCodeGridData";
    private const string FormEVatGridAction = "FormEVatGridData";
    private const string FormEAvailableVatGridAction = "FormEAvailableVatGridData";
    private const string FormEUnappliedAmountVatGridAction = "FormEUnappliedAmountVatGridData";
    private const string FormDGridAction = "FormDGridData";
    private const string ProvisionalInvoiceGridAction = "ProvisionalInvoiceGridData";

    public FormDEController(ISamplingFormDEManager samplingformDEManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(samplingformDEManager)
    {
      _samplingformDEManager = samplingformDEManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.SamplingFormDE); }
    }

    /// <summary>
    /// Returns view that allows to create new form DE header.
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult Create()
    {
      var invoice = new PaxInvoice
                    {
                      BillingMemberText = SessionUtil.MemberName,
                      InvoiceDate = DateTime.UtcNow,
                      InvoiceType = InvoiceType.Invoice,
                      BillingMemberId = SessionUtil.MemberId
                    };
      MakeInvoiceRenderReady(invoice.Id, invoice);

      var digitalSignatureRequired = GetDigitalSignatureRequired(invoice.BillingMemberId);
      ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId] = digitalSignatureRequired;
      invoice.DigitalSignatureRequiredId = digitalSignatureRequired;
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View(invoice);
    }

    /// <summary>
    /// Creates new form DE header if validation succeeds, else returns to create form DE header screen along with business exception message.
    /// </summary>
    /// <param name="invoice"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    public ActionResult Create(PaxInvoice invoice)
    {
      invoice.InvoiceType = InvoiceType.Invoice;
      invoice.BillingCode = Convert.ToInt32(BillingCode.SamplingFormDE);
      invoice.InvoiceDate = DateTime.UtcNow;
      invoice.BillingMemberId = SessionUtil.MemberId;
      invoice.SubmissionMethod = SubmissionMethod.IsWeb;

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

      try
      {
          DateTime dummyDate;
          var provisionalBillingDate = string.Format("{0}{1}{2}", Convert.ToString(invoice.ProvisionalBillingYear).PadLeft(4, '0'), Convert.ToString(invoice.ProvisionalBillingMonth).PadLeft(2, '0'), "01");
          if (!DateTime.TryParseExact(provisionalBillingDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dummyDate))
          {
              throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalMonthYear);
          }

          invoice = _samplingformDEManager.CreateInvoice(invoice);
          ShowSuccessMessage(Messages.FormDECreateSuccessful);

          return RedirectToAction("Edit", new
                                              {
                                                  invoiceId = invoice.Id.Value()
                                              });
      }
      catch (ISBusinessException exception)
      {
          /* CMP #624: ICH Rewrite-New SMI X 
            * Description: As per ICH Web Service Response Message specifications 
            * Refer FRS Section 3.3 Table 9. 
            * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

          invoice.BilledMember = _samplingformDEManager.GetBilledMember(invoice.BilledMemberId);

          if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && (invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase) ||
                  invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase)))
          {
              ShowSmiXWebServiceErrorMessage(exception.Message);
          }
          else
          {
              ShowErrorMessage(exception.ErrorCode);
          }
          
          var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
          ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

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

    /// <summary>
    /// Allows to edit an form DE header.
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult Edit(string invoiceId)
    {
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      // check whether transactions exist for this form DE header
      ViewData[ViewDataConstants.TransactionExists] = _samplingformDEManager.IsTransactionExists(invoiceId) || _samplingformDEManager.IsProvisionalInvoiceExists(invoiceId);

      if (string.IsNullOrEmpty(InvoiceHeader.BillingMemberLocationCode)) InvoiceHeader.BillingMemberLocationCode = "-";
      if (string.IsNullOrEmpty(InvoiceHeader.BilledMemberLocationCode)) InvoiceHeader.BilledMemberLocationCode = "-";
      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Create grid instance for Form D Details grid
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));
      ViewData[ControlIdConstants.FormDSourceCodeGridId] = formDSourceCodeGrid.Instance;

      // Create grid instance for Form D List
      var samplingFormDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }));
      ViewData[ControlIdConstants.FormDGridId] = samplingFormDGrid.Instance;

      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        // Get all submitted errors.
        var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
        ViewData[ViewDataConstants.SubmittedErrorsGrid] = submittedErrorsGrid.Instance;
      }

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

    /// <summary>
    /// Allows to edit an form DE header.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    [HttpGet]
    public new ActionResult View(string invoiceId)
    {
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      // check whether transactions exist for this form DE header
      ViewData[ViewDataConstants.TransactionExists] = _samplingformDEManager.IsTransactionExists(invoiceId);

      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Create grid instance for Form D Details grid
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));
      ViewData[ControlIdConstants.FormDSourceCodeGridId] = formDSourceCodeGrid.Instance;

      // Create grid instance for Form D List
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }

      var samplingFormDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }), isRejectionAllowed);

      ViewData[ControlIdConstants.FormDGridId] = samplingFormDGrid.Instance;

      return View("Edit", InvoiceHeader);
    }

    /// <summary>
    /// Updates form DE header if validation succeeds else displays business exception message on edit form DE header screen.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult Edit(string invoiceId, PaxInvoice invoice)
    {
        try
        {
            invoice.Id = invoiceId.ToGuid();
            invoice.InvoiceDate = DateTime.UtcNow;
            invoice.InvoiceStatus = InvoiceStatusType.Open;
            invoice.InvoiceType = InvoiceType.Invoice;
            invoice.BillingCode = Convert.ToInt32(BillingCode.SamplingFormDE);
            invoice.BillingMemberId = SessionUtil.MemberId;
            invoice.SubmissionMethod = SubmissionMethod.IsWeb;
            invoice.LastUpdatedBy = SessionUtil.UserId;

            DateTime dummyDate;
            var provisionalBillingDate = string.Format("{0}{1}{2}", Convert.ToString(invoice.ProvisionalBillingYear).PadLeft(4, '0'), Convert.ToString(invoice.ProvisionalBillingMonth).PadLeft(2, '0'), "01");
            if (!DateTime.TryParseExact(provisionalBillingDate, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dummyDate))
            {
                throw new ISBusinessException(SamplingErrorCodes.InvalidProvisionalMonthYear);
            }

            invoice = _samplingformDEManager.UpdateInvoice(invoice);

            ShowSuccessMessage(Messages.FormDEUpdateSuccessful);
            if (invoice.ProvisionalBillingMonth != 0 && invoice.ProvisionalBillingYear != 0 && invoice.IsFormABViaIS &&
                invoice.IsFormCViaIS)
            {
                DateTime provisiondate = new DateTime(invoice.ProvisionalBillingYear, invoice.ProvisionalBillingMonth, 1);

                string provisiondatetime = provisiondate.ToString("MMM") + " " + provisiondate.Year;

                ShowSuccessMessage(string.Format(Messages.ProvisionalInvoiceLink, provisiondatetime));
            }

            return RedirectToAction("Edit", new {invoiceId});
        }
        catch (ISBusinessException exception)
        {
            /* CMP #624: ICH Rewrite-New SMI X 
            * Description: As per ICH Web Service Response Message specifications 
            * Refer FRS Section 3.3 Table 9. 
            * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

            var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
            var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

            invoice.BilledMember = _samplingformDEManager.GetBilledMember(invoice.BilledMemberId);
            // Check whether transactions exist for this form DE header
            ViewData[ViewDataConstants.TransactionExists] = _samplingformDEManager.IsTransactionExists(invoiceId);

            if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && (invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase) ||
                    invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase)))
            {
                ShowSmiXWebServiceErrorMessage(exception.Message);
            }
            else
            {
                ShowErrorMessage(exception.ErrorCode);
            }
            
            var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
            ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

            if (InvoiceHeader != null && InvoiceHeader.MemberLocationInformation != null)
            {
                var billingMemberLocationInfo =
                    InvoiceHeader.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
                if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
                {
                    ViewData["IsLegalTextSet"] = true;
                }
            }
        }

      MakeInvoiceRenderReady(invoice.Id, invoice);

      // Create grid instance for Form D Details grid
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));
      ViewData[ControlIdConstants.FormDSourceCodeGridId] = formDSourceCodeGrid.Instance;

      // Create grid instance for Form D List
      var samplingFormDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }));

      ViewData[ControlIdConstants.FormDGridId] = samplingFormDGrid.Instance;
      SetViewDataPageMode(PageMode.Edit);

      return View(invoice);
    }

    /// <summary>
    /// Display source code list
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    [Obsolete]
    public ActionResult FormDDetails(string invoiceId)
    {
      //Create grid instance for Form D Details grid
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));
      ViewData[ControlIdConstants.FormDSourceCodeGridId] = formDSourceCodeGrid.Instance;

      return View(InvoiceHeader);
    }

    /// <summary>
    /// Display Form D List 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Obsolete]
    public ActionResult FormDList(string invoiceId)
    {
      // Create grid instance for Form D List
      var samplingFormDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }));

      ViewData[ControlIdConstants.FormDGridId] = samplingFormDGrid.Instance;

      return View(InvoiceHeader);
    }

    /// <summary>
    /// Delete From Form D
    /// </summary>
    /// <param name="transactionId">Id of transaction to be deleted.</param>
    /// <returns></returns>
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_FORM_D_COUPON_RECORD)]
    public JsonResult FormDDelete(string transactionId)
    {
      UIMessageDetail details = null;
      try
      {
        var formD = _samplingformDEManager.GetSamplingFormD(transactionId);
        var invoiceId = formD.InvoiceId.ToString();

        // Delete record
        bool isDeleted = _samplingformDEManager.DeleteSamplingFormD(transactionId);

        return Json(GetDeleteMessage(isDeleted, Url.Action("Edit", new { invoiceId })));
      }
      catch (ISBusinessException ex)
      {
        HandleDeleteException(ex.ErrorCode);

        return Json(details);
      }
    }

    /// <summary>
    /// Returns view that allows to create new sampling form D record.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [RestrictUnauthorizedUpdate]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult FormDCreate(string invoiceId)
    {
      var samplingFormD = new SamplingFormDRecord {
                                                    LastUpdatedBy = SessionUtil.UserId
                                                  };

      var sourceCodeList = _referenceManager.GetSourceCodeList(Convert.ToInt32(TransactionType.SamplingFormD));

      // For pre-population of the only source code for sampling form D record.
      if (sourceCodeList.Count != 0)
      {
        samplingFormD.SourceCodeId = sourceCodeList[0].SourceCodeIdentifier;
      }

      samplingFormD.Invoice = InvoiceHeader;

      //Change to populate billing member numeric code for Ticket issuing airline
      var billingMember = _samplingformDEManager.GetBilledMember(InvoiceHeader.BillingMemberId);
      samplingFormD.TicketIssuingAirline = billingMember.MemberCodeNumeric;
      samplingFormD.Invoice = InvoiceHeader;

      // Set ViewData, "IsPostback" to false
      ViewData[ViewDataConstants.IsPostback] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData[TempDataConstants.SamplingFormDRecord] != null)
      {
        if(TempData[TempDataConstants.SamplingFormDRecord].ToString() == string.Empty)
        {
          // Set Viewdata(To set sourceCodeIdentifier and ticketIssuingAirline on SaveAndBackToOverview click)
          ViewData[ViewDataConstants.SamplingFormDRecord] = string.Format(@"{0}-{1}", sourceCodeList[0].SourceCodeIdentifier, billingMember.MemberCodeNumeric);
        }
        else
        {
          // Set Viewdata
          ViewData[ViewDataConstants.SamplingFormDRecord] = TempData[TempDataConstants.SamplingFormDRecord]; 
        }
      }
      
      return View(samplingFormD);
    }

    /// <summary>
    /// Create sampling Form D record
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult FormDCreate(string invoiceId, SamplingFormDRecord record)
    {
      //Get attachment Id list
      var formDAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
        //TaxBreakDown Validation 
        if (inValidTaxBreakDown(record.TaxAmount, record.TaxBreakdown))
        {
          throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
        }
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 6 */
        MemberManager.ValidateIssuingAirline(record.TicketIssuingAirline);
        //SCPID : 124519 - related to SF#03199440 / SCP# 96711 - ISC rate is 0 in Form D record in IS-IDEC April P4
        _samplingformDEManager.ValidateIscPerAndAmt(record.EvaluatedGrossAmount, record.IscPercent, record.IscAmount);
        string duplicateErrorMessage;
        record.Attachments.Clear();
        record.LastUpdatedBy = SessionUtil.UserId;
        record = _samplingformDEManager.AddSamplingFormD(record, out duplicateErrorMessage);

        // Update parent Id for attachment
        _samplingformDEManager.UpdateSamplingFormDAttachment(formDAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.FormDCreateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        TempData[TempDataConstants.SamplingFormDRecord] = string.Format(@"{0}-{1}", record.SourceCodeId,
                                                                        record.TicketIssuingAirline);

        return RedirectToAction("FormDCreate", new {invoiceId});
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message, true);
        record.Attachments = _samplingformDEManager.GetSamplingFormDRecordAttachments(formDAttachmentIds);
      }

      record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      return View(record);
    }


    /// <summary>
    /// Creates prime billing coupon and allows to duplicate same record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "FormDCreate")]
    public ActionResult FormDDuplicate(string invoiceId, SamplingFormDRecord record)
    {
      //Get attachment Id list
      var formDAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 6 */
          MemberManager.ValidateIssuingAirline(record.TicketIssuingAirline);

          //SCPID : 124519 - related to SF#03199440 / SCP# 96711 - ISC rate is 0 in Form D record in IS-IDEC April P4
          _samplingformDEManager.ValidateIscPerAndAmt(record.EvaluatedGrossAmount, record.IscPercent, record.IscAmount);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        string duplicateErrorMessage;

        record.Attachments.Clear();
        record.LastUpdatedBy = SessionUtil.UserId;
        record = _samplingformDEManager.AddSamplingFormD(record, out duplicateErrorMessage);

        // Update parent Id for attachment
        _samplingformDEManager.UpdateSamplingFormDAttachment(formDAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.FormDCreateSuccessful, false);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        SetViewDataPageMode(PageMode.Clone);

        record.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message, true);
        record.Attachments = _samplingformDEManager.GetSamplingFormDRecordAttachments(formDAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      return View("FormDCreate", record);

    }

    /// <summary>
    /// Creates prime billing coupon and redirects to prime billing coupon listing
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "FormDCreate")]
    public ActionResult FormDCreateAndReturn(string invoiceId, SamplingFormDRecord record)
    {
      //Get attachment Id list
      var formDAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {

          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 6 */
          MemberManager.ValidateIssuingAirline(record.TicketIssuingAirline);
          //SCPID : 124519 - related to SF#03199440 / SCP# 96711 - ISC rate is 0 in Form D record in IS-IDEC April P4
          _samplingformDEManager.ValidateIscPerAndAmt(record.EvaluatedGrossAmount, record.IscPercent, record.IscAmount);

        string duplicateErrorMessage;

        record.Attachments.Clear();
        record.LastUpdatedBy = SessionUtil.UserId;
        record = _samplingformDEManager.AddSamplingFormD(record, out duplicateErrorMessage);

        // Update parent Id for attachment
        _samplingformDEManager.UpdateSamplingFormDAttachment(formDAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.FormDCreateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        TempData[TempDataConstants.SamplingFormDRecord] = "";

        record.Attachments.Clear(); // Attachments should not be duplicated. 

        return RedirectToAction("Edit", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message, true);
        record.Attachments = _samplingformDEManager.GetSamplingFormDRecordAttachments(formDAttachmentIds);
      }

      record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      return View("FormDCreate", record);
    }

    /// <summary>
    /// Returns view that allows to edit new sampling form D record.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult FormDEdit(string invoiceId, string transactionId)
    {
      var samplingFormDRecord = new SamplingFormDRecord();

      try
      {
        // TODO: Disable the keys on which data pre-population was done When linking was successful.

        if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling) SetViewDataPageMode(PageMode.View);

        samplingFormDRecord = _samplingformDEManager.GetSamplingFormD(transactionId);
        samplingFormDRecord.Invoice = InvoiceHeader;
        samplingFormDRecord.LastUpdatedBy = SessionUtil.UserId;

        // added code to remove the extra special char
        if (!string.IsNullOrEmpty(samplingFormDRecord.ProrateSlipDetails))
          samplingFormDRecord.ProrateSlipDetails =
            samplingFormDRecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);


        return View(samplingFormDRecord);
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }
      samplingFormDRecord.Invoice = InvoiceHeader;

      return View(samplingFormDRecord);
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    [HttpGet]
    public ActionResult FormDView(string invoiceId, string transactionId)
    {
      var samplingFormDRecord = _samplingformDEManager.GetSamplingFormD(transactionId);
      samplingFormDRecord.Invoice = InvoiceHeader;

      return View("FormDEdit", samplingFormDRecord);
    }

    /// <summary>
    /// Update sampling form D record
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <param name="record"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult FormDEdit(string invoiceId, string transactionId, SamplingFormDRecord record)
    {
      var formDAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 6 */
          MemberManager.ValidateIssuingAirline(record.TicketIssuingAirline);
          //SCPID : 124519 - related to SF#03199440 / SCP# 96711 - ISC rate is 0 in Form D record in IS-IDEC April P4
          _samplingformDEManager.ValidateIscPerAndAmt(record.EvaluatedGrossAmount, record.IscPercent, record.IscAmount);

        string duplicateErrorMessage;
        record.Id = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        //Assign parentId for tax and vat records
        foreach (var tax in record.TaxBreakdown)
        {
          tax.ParentId = record.Id;
        }
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }
        record = _samplingformDEManager.UpdateSamplingFormD(record, out duplicateErrorMessage);
        // TODO: Add changes for validation: Any change in amounts will result in change of Form E level calculations
        ShowSuccessMessage(Messages.FormDUpdateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        TempData[TempDataConstants.SamplingFormDRecord] = string.Format(@"{0}-{1}", record.SourceCodeId, record.TicketIssuingAirline);

        return RedirectToAction("FormDCreate", new { invoiceId});
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        SetViewDataPageMode(PageMode.Edit);
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        record.Attachments = _samplingformDEManager.GetSamplingFormDRecordAttachments(formDAttachmentIds);
      }

      record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);
      return View(record);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "FormDEdit")]
    public ActionResult FormDClone(string invoiceId, string transactionId, SamplingFormDRecord record)
    {
      var formDAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      record.Id = transactionId.ToGuid();

      try
      {

          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 6 */
          MemberManager.ValidateIssuingAirline(record.TicketIssuingAirline);
          //SCPID : 124519 - related to SF#03199440 / SCP# 96711 - ISC rate is 0 in Form D record in IS-IDEC April P4
          _samplingformDEManager.ValidateIscPerAndAmt(record.EvaluatedGrossAmount, record.IscPercent, record.IscAmount);

        string duplicateErrorMessage;
        record.Id = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        //Assign parentId for tax and vat records
        foreach (var tax in record.TaxBreakdown)
        {
          tax.ParentId = record.Id;
        }
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }
        record = _samplingformDEManager.UpdateSamplingFormD(record, out duplicateErrorMessage);

        record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

        // TODO: Add changes for validation: Any change in amounts will result in change of Form E level calculations
        ShowSuccessMessage(Messages.FormDUpdateSuccessful, false);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        record.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        
        return View("FormDCreate", record);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        record.Attachments = _samplingformDEManager.GetSamplingFormDRecordAttachments(formDAttachmentIds);
        record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);
        SetViewDataPageMode(PageMode.Edit);
      }
      
      return View("FormDEdit", record);
    }

    /// <summary>
    /// Creates prime billing coupon and redirects to prime billing coupon listing
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "FormDEdit")]
    public ActionResult FormDEditAndReturn(string invoiceId, string transactionId, SamplingFormDRecord record)
    {
      //Get attachment Id list
      var formDAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 6 */
          MemberManager.ValidateIssuingAirline(record.TicketIssuingAirline);
          //SCPID : 124519 - related to SF#03199440 / SCP# 96711 - ISC rate is 0 in Form D record in IS-IDEC April P4
          _samplingformDEManager.ValidateIscPerAndAmt(record.EvaluatedGrossAmount, record.IscPercent, record.IscAmount);
        string duplicateErrorMessage;
        record.Id = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        //Assign parentId for tax and vat records
        foreach (var tax in record.TaxBreakdown)
        {
          tax.ParentId = record.Id;
        }
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }
        record = _samplingformDEManager.UpdateSamplingFormD(record, out duplicateErrorMessage);

        record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

        ShowSuccessMessage(Messages.FormDUpdateSuccessful);
        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        TempData[TempDataConstants.SamplingFormDRecord] = "";

        record.Attachments.Clear(); // Attachments should not be duplicated. 

        return RedirectToAction("Edit", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        SetViewDataPageMode(PageMode.Edit);
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message, true);
        record.Attachments = _samplingformDEManager.GetSamplingFormDRecordAttachments(formDAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.Id = transactionId.ToGuid();
      }

      record.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      return View("FormDEdit", record);
    }


    /// <summary>
    /// Upload Form D Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult FormDAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<SamplingFormDRecordAttachment>();
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

          var invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

          fileUploadHelper = new FileAttachmentHelper
          {
            FileToSave = fileToSave,
            FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth)
          };

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
            var attachment = new SamplingFormDRecordAttachment
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

            attachment = _samplingformDEManager.AddSamplingFormDAttachment(attachment);
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

      return new FileUploadJsonResult
      {
        Data = new
        {
          IsFailed = !isUploadSuccess,
          Message = message,
          Attachment = attachments,
          Length = attachments.Count
        }
      };
    }

    /// <summary>
    /// Download form D attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionId">Transaction id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Download)]
    [HttpGet]
    public FileStreamResult FormDAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
                                                  {
                                                    Attachment = _samplingformDEManager.GetSamplingFormDAttachment(transactionId)
                                                  };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// GET: Create Form E
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    public ActionResult FormECreate(string invoiceId)
    {
      var samplingFormEDetail = new SamplingFormEDetail
      {
        Invoice = InvoiceHeader,
        LastUpdatedBy = SessionUtil.UserId
      };

      return View(samplingFormEDetail);
    }

    /// <summary>
    /// POST: Create Form E
    /// </summary>
    /// <param name="invoiceId">Form D/E invoice Id</param>
    /// <param name="samplingFormEDetail">Sampling Form E record to be created</param>
    /// <returns>Created Form E record details</returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult FormECreate(string invoiceId, SamplingFormEDetail samplingFormEDetail)
    {
      try
      {
        samplingFormEDetail.Id = invoiceId.ToGuid();
        samplingFormEDetail.LastUpdatedBy = SessionUtil.UserId;
        _samplingformDEManager.CreateSamplingFormE(samplingFormEDetail);

        ShowSuccessMessage(Messages.FormECreateSuccessful);

        return RedirectToAction("FormEEdit", new { invoiceId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
      }

      return View(samplingFormEDetail);
    }

    /// <summary>
    /// GET: Edit Form E
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    public ActionResult FormEEdit(string invoiceId)
    {
      var samplingFormEDetail = _samplingformDEManager.GetSamplingFormE(invoiceId);
      samplingFormEDetail.Invoice = InvoiceHeader;
      samplingFormEDetail.LastUpdatedBy = SessionUtil.UserId;
      ViewData["FormAbTotal"] = samplingFormEDetail.TotalAmountFormB;
      return View(samplingFormEDetail);
    }

   
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    public ActionResult FormEView(string invoiceId)
    {
      var samplingFormEDetail = _samplingformDEManager.GetSamplingFormE(invoiceId);
      samplingFormEDetail.Invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      return View("FormEEdit", samplingFormEDetail);
    }

    /// <summary>
    /// GET: Edit Form E
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    public ActionResult Details(string invoiceId)
    {
      var samplingFormEDetail = _samplingformDEManager.GetSamplingFormE(invoiceId);
      samplingFormEDetail.Invoice = InvoiceHeader;
      ViewData["FormAbTotal"] = samplingFormEDetail.TotalAmountFormB;
   

      return View("FormEEdit", samplingFormEDetail);
    }

    /// <summary>
    /// POST: Edit Form E
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="samplingFormEDetail"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult FormEEdit(string invoiceId, string transactionId, SamplingFormEDetail samplingFormEDetail)
    {
      try
      {
        samplingFormEDetail.Id = invoiceId.ToGuid();
        samplingFormEDetail.LastUpdatedBy = SessionUtil.UserId;
        _samplingformDEManager.UpdateSamplingFormE(samplingFormEDetail);

        ShowSuccessMessage(Messages.FormEUpdateSuccessful);

        return RedirectToAction("Details", new
                                           {
                                             invoiceId
                                           });
      }
      catch (ISBusinessException be)
      {
        ShowErrorMessage(be.ErrorCode, true);
      }

      var invoice = _samplingformDEManager.GetInvoiceHeaderDetails(invoiceId);

      samplingFormEDetail.Invoice = invoice;

      ViewData["FormAbTotal"] = samplingFormEDetail.TotalAmountFormB;

      return View(samplingFormEDetail);
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult ProvisionalInvoice(string invoiceId)
    {
      var record = new ProvisionalInvoiceRecordDetail();

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (!ReferenceManager.IsSmiLikeBilateral(InvoiceHeader.SettlementMethodId, true))
      {
        ViewData[ViewDataConstants.NotBilateralSettlementMethod] = true;
      }
      
      record.InvoiceId = invoiceId.ToGuid();
      record.Invoice = InvoiceHeader;

      var provisionalInvoiceGrid = new ProvisionalInvoiceGrid(ControlIdConstants.ProvisionalInvoiceGridId, Url.Action(ProvisionalInvoiceGridAction, new
                                                                                                                                                    {
                                                                                                                                                      invoiceId
                                                                                                                                                    }), InvoiceHeader.IsFormABViaIS);

      ViewData[ControlIdConstants.ProvisionalInvoiceGridId] = provisionalInvoiceGrid.Instance;
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View(record);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    [HttpGet]
    public ActionResult ProvisionalInvoiceView(string invoiceId)
    {
      var record = new ProvisionalInvoiceRecordDetail();
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (!ReferenceManager.IsSmiLikeBilateral(InvoiceHeader.SettlementMethodId, true))
      {
        ViewData[ViewDataConstants.NotBilateralSettlementMethod] = true;
      }
    
      record.InvoiceId = invoiceId.ToGuid();
      record.Invoice = InvoiceHeader;

      var provisionalInvoiceGrid = new ProvisionalInvoiceGrid(ControlIdConstants.ProvisionalInvoiceGridId, Url.Action(ProvisionalInvoiceGridAction, new
      {
        invoiceId
      }), InvoiceHeader.IsFormABViaIS);

      ViewData[ControlIdConstants.ProvisionalInvoiceGridId] = provisionalInvoiceGrid.Instance;
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View("ProvisionalInvoice", record);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult ProvisionalInvoiceCreate(FormCollection form, string invoiceId)
    {
      try
      {
        var provInvoice = new JavaScriptSerializer().Deserialize(form[0], typeof(ProvisionalInvoiceRecordDetail));
        var record = provInvoice as ProvisionalInvoiceRecordDetail;
        if (record != null)
        {
          record.LastUpdatedBy = SessionUtil.UserId;
          _samplingformDEManager.AddProvisionalInvoice(record);
        }

        var details = new UIExceptionDetail
        {
          IsFailed = false,
          Message = Messages.RecordSaveSuccessful
        };
        return Json(details);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        var details = new UIExceptionDetail
        {
          IsFailed = true,
          Message = string.Format(Messages.RecordSaveException, GetDisplayMessage(businessException.ErrorCode))
        };
        return Json(details);
      }
    }

    //
    // POST: /Pax/{invoiceId}/FormDE/Delete/5
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(TransactionParamName = "id", IsJson = true, TableName = TransactionTypeTable.PAX_FORM_E_PROV_INVOICE)]
    public ActionResult ProvisionalInvoiceDelete(string id)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        var isDeleted = _samplingformDEManager.DeleteProvisionalInvoice(id);

        details = GetDeleteMessage(isDeleted);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Fetch data form Provisional Invoices
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult ProvisionalInvoiceGridData(string invoiceId)
    {
      var provisionalInvoiceGrid = new ProvisionalInvoiceGrid(ControlIdConstants.ProvisionalInvoiceGridId, Url.Action(ProvisionalInvoiceGridAction));
      var provisionalInvoiceRecords = _samplingformDEManager.GetProvisionalInvoiceList(invoiceId);

      return provisionalInvoiceGrid.DataBind(provisionalInvoiceRecords);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    public override ActionResult Vat(string invoiceId)
    {
      return View(VatBase(invoiceId));
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    public override ActionResult VatView(string invoiceId)
    {
      return View("Vat", VatBase(invoiceId));
    }

    private PaxInvoice VatBase(string invoiceId)
    {
      // Flag to set whether grid is to be displayed in View mode
      bool isGridViewOnly = false;

      // If Page mode is view set "isGridViewOnly" variable to true, depending on this variable action column is displayed.  
      if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
        isGridViewOnly = true;
      }

      // Create grid instance for invoice vat list
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(FormEVatGridAction, new { invoiceId }), isGridViewOnly);
      ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
      {
        // Create grid instance for available vat 
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(FormEAvailableVatGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.AvailableVatGrid] = availableVatGrid.Instance;

        //Create grid instance for vat not applied amount
        var unappliedAmountVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(FormEUnappliedAmountVatGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.UnappliedAmountVatGrid] = unappliedAmountVatGrid.Instance;
      }

      return InvoiceHeader;
    }

    /// <summary>
    /// Save Invoice level VAT 
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public override JsonResult Vat(FormCollection form, string invoiceId)
    {
      try
      {
        var vat = new JavaScriptSerializer().Deserialize(form[0], typeof(SamplingFormEDetailVat));
        var record = vat as SamplingFormEDetailVat;
        _samplingformDEManager.AddSamplingFormEVat(record);

        var details = new UIExceptionDetail
        {
          IsFailed = false,
          Message = Messages.RecordSaveSuccessful
        };
        return Json(details);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        var details = new UIExceptionDetail
        {
          IsFailed = false,
          Message = string.Format(Messages.RecordSaveException, GetDisplayMessage(businessException.ErrorCode))
        };
        return Json(details);
      }
    }

    /// <summary>
    /// Delete Invoice Vat Record
    /// </summary>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.CreateOrEdit)]
    [RestrictInvoiceUpdate(TransactionParamName = "Id", IsJson = true, TableName = TransactionTypeTable.PAX_FORM_E_VAT_BREAKDOWN)]
   public override JsonResult VatDelete(string Id)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        bool isDeleted = _samplingformDEManager.DeleteSamplingFormEVat(Id);

        details = GetDeleteMessage(isDeleted);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Vat Data to populate in the Grid
    /// </summary>
    public JsonResult FormEVatGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(FormEVatGridAction, new { invoiceId }));

      var vatData = _samplingformDEManager.GetSamplingFormEVatList(invoiceId);
      return invoiceVatGrid.DataBind(vatData);
    }

    /// <summary>
    /// Available Vat Data to populate in the Grid
    /// </summary>
    public JsonResult FormEAvailableVatGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var vatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(FormEAvailableVatGridAction, new { invoiceId }));

      var vatData = _samplingformDEManager.GetFormDInvoiceLevelDerivedVatList(invoiceId).AsQueryable();
      int count = 1;
      foreach (var derivedVatDetails in vatData)
      {
        derivedVatDetails.RowNumber = count++;
      }
      return vatGrid.DataBind(vatData);
    }

    /// <summary>
    /// Unapplied Vat amount Data to populate in the Grid
    /// </summary>
    public JsonResult FormEUnappliedAmountVatGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var unappliedVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(FormEUnappliedAmountVatGridAction, new { invoiceId }));
      var notAppliedVatList = _samplingformDEManager.GetFormDNonAppliedVatList(invoiceId).AsQueryable();
      int count = 1;
      foreach (var nonAppliedVatDetails in notAppliedVatList)
      {
        nonAppliedVatDetails.RowNumber = count++;
      }
      return unappliedVatGrid.DataBind(notAppliedVatList);
    }

    /// <summary>
    /// Fetch data for Form D and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult FormDGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var formDGrid = new SamplingFormDGrid(ControlIdConstants.FormDGridId, Url.Action(FormDGridAction, new { invoiceId }));

      var formDList = _samplingformDEManager.GetSamplingFormDList(invoiceId);

      return formDGrid.DataBind(formDList);
    }

    /// <summary>
    /// Fetch data for Form D Source Code and display it in grid
    /// </summary>
    /// <returns></returns>
    public JsonResult FormDSourceCodeGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var formDSourceCodeGrid = new SamplingFormDDetailsSourceCodeGrid(ControlIdConstants.FormDSourceCodeGridId, Url.Action(FormDSourceCodeGridAction, new { invoiceId }));

      var sourceCodeList = _samplingformDEManager.GetSourceCodeList(invoiceId).AsQueryable();
      return formDSourceCodeGrid.DataBind(sourceCodeList);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Validate)]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "FormEEdit")]
    public ActionResult ValidateInvoice(string invoiceId)
    {
      ValidateInvoice(_samplingformDEManager, invoiceId);

      return RedirectToAction("FormEEdit");
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Submit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "FormEView")]
    public override ActionResult Submit(string invoiceId)
    {
      var submittedInvoice = SubmitInvoice(invoiceId);

      return RedirectToAction(submittedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling ? "FormEView" : "FormEEdit", "FormDE");
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Validate)]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public ActionResult ValidateDEHeaderInvoice(string invoiceId)
    {
      ValidateInvoice(_samplingformDEManager, invoiceId);

      return RedirectToAction("Edit");
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.Submit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "View")]
    public ActionResult SubmitDEHeader(string invoiceId)
    {
      var submittedInvoice = SubmitInvoice(invoiceId);

      return RedirectToAction("View");
    }


    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.SampleFormDE.View)]
    [HttpPost]
    public JsonResult GetFormDLinkedCouponDetails(Guid invoiceId, int ticketCouponNumber, long? ticketDocNumber, string issuingAirline)
    {
        //SCP0000: Elmah Exceptions log removal
        List<LinkedCoupon> linkedCoupons = null;

      if (ticketDocNumber != null)
        linkedCoupons = _samplingformDEManager.GetFormDLinkedCouponDetails(invoiceId, ticketCouponNumber, (long) ticketDocNumber, issuingAirline);

      string errorMessage = string.Empty;
        if (linkedCoupons != null)
        {
            if (linkedCoupons.Count == 0) errorMessage = Messages.CouponDoesNotExist;
            else if (!string.IsNullOrEmpty(linkedCoupons[0].ErrorCode))
                errorMessage = Messages.ResourceManager.GetString(linkedCoupons[0].ErrorCode);

            var linkedCouponDetails = new LinkedCouponDetails
                                          {
                                              LinkedCoupons = linkedCoupons,
                                              ErrorMessage = errorMessage
                                          };
            return Json(linkedCouponDetails);
        }
        return null;
    }

    /// <summary>
    /// This method is use to check amount difference in tax breakdowns with total amount.
    /// Issue: 65997 - No tax breakdwon for invoice 2012D74226, RM 618240311.
    /// This validation will be applicable From DE coupons.
    /// SCP105897 & SCP105226 [added Math.Round() for calculatedAmount]
    /// </summary>
    /// <param name="targetAmount">target Amount</param>
    /// <param name="taxBreakdowns">tax Breakdowns</param>
    /// <returns>return false if tax breakdowns are valid.</returns>
    private bool inValidTaxBreakDown(double targetAmount, IEnumerable<SamplingFormDTax> taxBreakdowns = null)
    {
       double calculatedAmount = 0;
        // credit memo coupon tax breakdown validation
        if (taxBreakdowns != null)
        {
            calculatedAmount = taxBreakdowns.Aggregate<SamplingFormDTax, double>(0, (current, cmCouponTax) => current + cmCouponTax.Amount);
        }
      return Math.Round(calculatedAmount, 2).Equals(targetAmount) ? false : true;
    }
  }
}
