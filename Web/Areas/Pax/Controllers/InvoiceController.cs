using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Areas.Pax.Controllers.Base;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.Util;
using Iata.IS.Business.Pax;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Core.Exceptions;
using log4net;
using System.Reflection;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.Pax.Common;
using System.Web.Script.Serialization;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.AdminSystem;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Trirand.Web.Mvc;
using Iata.IS.Model.Common;
using System.Data;
using Iata.IS.Business.Web;

namespace Iata.IS.Web.Areas.Pax.Controllers
{
  public class InvoiceController : PaxInvoiceControllerBase
  {
    private readonly INonSamplingInvoiceManager _nonSamplingInvoiceManager;
    private readonly IReferenceManager _referenceManager;
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string PrimeBillingGridAction = "PrimeBillingGridData";
    // CMP #672: Validation on Taxes in PAX FIM Billings
    private const int FimSourceCode14Pax = 14;
    private const string SourceCodeGridAction = "SourceCodeGridData";
    private const string BillingMemoGridAction = "BillingMemoGridData";
    private readonly IInvoiceManager _invoiceManager;
    public InvoiceController(INonSamplingInvoiceManager nonSamplingInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager,IInvoiceManager InvoiceManager)
      : base(nonSamplingInvoiceManager)
    {
      _nonSamplingInvoiceManager = nonSamplingInvoiceManager;
      _referenceManager = referenceManager;
      MemberManager = memberManager;
      _invoiceManager = InvoiceManager;
    }

    protected override int BillingCodeId
    {
      get { return Convert.ToInt32(BillingCode.NonSampling); }
    }

    protected override InvoiceType InvoiceType
    {
      get { return InvoiceType.Invoice; }
    }

    /// <summary>
    /// Returns view that allows to create new invoice.
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult Create()
    {
      //Reset invoice search criteria

      string billedMemberText = string.Empty;
      int billedMemberId = 0;
      Member billedMember = null;
      var previousInvoiceSMI = -1;
      if (!(Request.QueryString.AllKeys.Contains(TempDataConstants.FromBillingHistory) && Request.QueryString.Get(TempDataConstants.FromBillingHistory) == "true"))
      {
        SessionUtil.PaxCorrSearchCriteria = SessionUtil.PaxInvoiceSearchCriteria = SessionUtil.InvoiceSearchCriteria = null;
      }
      else
      {
        ViewData[ViewDataConstants.FromBillingHistory] = true;
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) && TempData[TempDataConstants.RejectedRecordIds] != null)
        {
          var originalRejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();
          var rejectedRecordIds = originalRejectedRecordIds;

          if (rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf('@') + 1)
          {
            string rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);

            var inv = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
            if (inv != null)
            {
              billedMemberText = inv.BillingMemberText;
              billedMemberId = inv.BillingMemberId;
              billedMember = inv.BillingMember;
              previousInvoiceSMI = inv.SettlementMethodId;
            }
          }
        }
        else if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
        {
          var correspondenceRefNumber = TempData[TempDataConstants.CorrespondenceNumber].ToString();
          var correspondenceManager = Ioc.Resolve<IPaxCorrespondenceManager>(typeof(IPaxCorrespondenceManager));
          var correspondence = correspondenceManager.GetRecentCorrespondenceDetails(Convert.ToInt64(correspondenceRefNumber));
          if (correspondence != null && correspondence.InvoiceId != Guid.Empty)
          {
            var inv = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(correspondence.InvoiceId.Value());
            if (inv != null)
            {
              billedMemberText = inv.BillingMemberText;
              billedMemberId = inv.BillingMemberId;
              billedMember = inv.BillingMember;
              previousInvoiceSMI = inv.SettlementMethodId;
            }
          }
        }
      }

      var invoice = new PaxInvoice
                      {
                        BilledMemberId = Convert.ToInt32(billedMemberId),
                        BilledMemberText = billedMemberText,
                        BilledMember = billedMember,
                        BillingMemberText = SessionUtil.MemberName,
                        InvoiceDate = DateTime.UtcNow,
                        InvoiceType = InvoiceType.Invoice,
                        BillingMemberId = SessionUtil.MemberId,
                        InvoiceOwnerId = SessionUtil.UserId,
                        LastUpdatedBy = SessionUtil.UserId
                      };

      if (previousInvoiceSMI != -1)
      {
          invoice.SettlementMethodId = previousInvoiceSMI;
          //CMP#624 : TFS#9271
          TempData[TempDataConstants.PreviousInvoiceSMI] = previousInvoiceSMI;
      }
        
      MakeInvoiceRenderReady(new Guid(), invoice);

      var digitalSignatureRequired = GetDigitalSignatureRequired(invoice.BillingMemberId);
      ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId] = digitalSignatureRequired;
      invoice.DigitalSignatureRequiredId = digitalSignatureRequired;

      KeepBillingHistoryDataInStore(Request.QueryString.AllKeys.Contains(TempDataConstants.FromBillingHistory));

      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View(invoice);
    }

    private void KeepBillingHistoryDataInStore(bool isBillingHistory)
    {
      if (isBillingHistory)
      {
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds];
          TempData[TempDataConstants.RejectedRecordIds] = rejectedRecordIds;
        }

        if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
        {
          var rejectedRecordIds = TempData[TempDataConstants.CorrespondenceNumber];
          TempData[TempDataConstants.CorrespondenceNumber] = rejectedRecordIds;
        }
        //CMP#624 : 
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          var previousSMI = TempData[TempDataConstants.PreviousInvoiceSMI];
          TempData[TempDataConstants.PreviousInvoiceSMI] = previousSMI;
        }
      }
      else
      {
        ViewData[ViewDataConstants.FromBillingHistory] = false;
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          TempData.Remove(TempDataConstants.RejectedRecordIds);
        }
        if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
        {
          TempData.Remove(TempDataConstants.CorrespondenceNumber);
        }
        //CMP#624 : 
        if (TempData.ContainsKey(TempDataConstants.PreviousInvoiceSMI))
        {
          TempData.Remove(TempDataConstants.PreviousInvoiceSMI);
        }
      }
     
       
    }

    /// <summary>
    /// Creates new invoice if validation succeeds, else returns to create invoice screen along with business exception message.
    /// </summary>
    /// <param name="invoice"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [OutputCache(CacheProfile = "donotCache")]
    //[RestrictInvoiceUpdate(InvParamName = "", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult Create(PaxInvoice invoice)
    {
      try
      {
        invoice.BillingMemberId = SessionUtil.MemberId;
        invoice.InvoiceType = InvoiceType.Invoice;
        invoice.BillingCode = Convert.ToInt32(BillingCode.NonSampling);
        invoice.InvoiceDate = DateTime.UtcNow;
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

        invoice.LastUpdatedBy = SessionUtil.UserId;
        /* CMP #624: ICH Rewrite-New SMI X
        * Description: Check preserved invoice SMI, as X has to be rejected/BM by X only. 
        * Refer FRS Section 2.14 Change #9 */
        if (    (  (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) && TempData[TempDataConstants.RejectedRecordIds] != null) 
                    ||
                   (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber) && TempData[TempDataConstants.CorrespondenceNumber] != null)
                ) &&
                (TempData.ContainsKey(TempDataConstants.PreviousInvoiceSMI) && TempData[TempDataConstants.PreviousInvoiceSMI] != null)
           )
        {
            if(TempData[TempDataConstants.PreviousInvoiceSMI].ToString() == ((int)SMI.IchSpecialAgreement).ToString() && 
                invoice.SettlementMethodId != (int)SMI.IchSpecialAgreement)
            {
                /* Old is X new is not */
                throw new ISBusinessException(ErrorCodes.PaxNSRejInvBHLinkingCheckForSmiX);
            }
            else if(TempData[TempDataConstants.PreviousInvoiceSMI].ToString() != ((int)SMI.IchSpecialAgreement).ToString() && 
                invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            {
                /* Old is not X but new is X */
                throw new ISBusinessException(ErrorCodes.PaxNSRejctionInvoiceLinkingCheckForSmiX);
            }
        }

        invoice = _nonSamplingInvoiceManager.CreateInvoice(invoice);

        ShowSuccessMessage(Messages.InvoiceCreateSuccessful);

        KeepBillingHistoryDataInStore(true);

        //Initiate rejection memo from billing history screen
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) && TempData[TempDataConstants.RejectedRecordIds] != null)
        {
          return RedirectToAction("RMCreate", new { invoiceId = invoice.Id.Value() });
        }

        // Initiate billing memo from billing history screen
        if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
        {
          return RedirectToAction("BMCreate", new { invoiceId = invoice.Id.Value() });
        }

        return RedirectToAction("Edit", new { invoiceId = invoice.Id.Value() });
      }
      catch (ISBusinessException exception)
      {
          /* CMP #624: ICH Rewrite-New SMI X 
             * Description: As per ICH Web Service Response Message specifications 
             * Refer FRS Section 3.3 Table 9. 
             * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */
          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS
        invoice.BilledMember = _nonSamplingInvoiceManager.GetBilledMember(invoice.BilledMemberId);

        if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase))
        {
            ShowSmiXWebServiceErrorMessage(exception.Message);
        }
        else //if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase))
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
        //CMP#624 : TFS#9272 : System is getting crash when user tries to create ICH invoice from Billing History Screen for PAX.
        KeepBillingHistoryDataInStore(true);
      }

      MakeInvoiceRenderReady(invoice.Id, invoice);
      return View(invoice);
    }

    /// <summary>
    /// Allows to edit an invoice.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    public ActionResult Edit(string invoiceId)
    {
      ViewData[ViewDataConstants.TransactionExists] = _nonSamplingInvoiceManager.IsTransactionExists(invoiceId);

      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Currently, not all invoices has corresponding InvoiceTotal record entries 
      // in database, hence creating empty object of InvoiceTotal object.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      // Create Source Code grid instance
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;

      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        // Get all submitted errors.
        var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
        submittedErrorsGrid.Instance.ToolBarSettings.ToolBarPosition = ToolBarPosition.Bottom;
        ViewData[ViewDataConstants.SubmittedErrorsGrid] = submittedErrorsGrid.Instance;
      }

      KeepBillingHistoryDataInStore(ViewData[ViewDataConstants.FromBillingHistory] != null ? (bool)ViewData[ViewDataConstants.FromBillingHistory] : false);
      
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

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
    /// Allows to edit an invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="billingType">Type of the billing.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public new ActionResult View(string invoiceId, string billingType)
    {
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      ViewData[ViewDataConstants.TransactionExists] = _nonSamplingInvoiceManager.IsTransactionExists(invoiceId);

      // Currently, not all invoices has corresponding InvoiceTotal record entries 
      // in database, hence creating empty object of InvoiceTotal object.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      // Create Source Code grid instance
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      if (InvoiceHeader != null && InvoiceHeader.MemberLocationInformation.Count>0)
      {
        foreach(var memLocation in InvoiceHeader.MemberLocationInformation)
        {
          memLocation.LegalText = (!string.IsNullOrEmpty(InvoiceHeader.LegalText)? InvoiceHeader.LegalText.Trim() : string.Empty);
        }
      }

      // If BillingType is Payables instantiate SourceCode Vat Total grid
      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
        ViewData["VatGrid"] = availableVatGrid.Instance;
      }

      return View("Edit", InvoiceHeader);
    }

    /// <summary>
    /// Updates invoice if validation succeeds else displays business exception message on edit invoice screen.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult Edit(string invoiceId, PaxInvoice invoice)
    {
      try
      {
        invoice.Id = invoiceId.ToGuid();

        invoice.InvoiceType = InvoiceType.Invoice;
        invoice.InvoiceStatus = InvoiceStatusType.Open;
        invoice.BillingMemberId = SessionUtil.MemberId;
        invoice.InvoiceDate = DateTime.UtcNow;
        invoice.SubmissionMethod = SubmissionMethod.IsWeb;
        invoice.LastUpdatedBy = SessionUtil.UserId;
        invoice = _nonSamplingInvoiceManager.UpdateInvoice(invoice);

        ShowSuccessMessage(Messages.InvoiceUpdateSuccessful);

        return RedirectToAction("Edit", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {

          /* CMP #624: ICH Rewrite-New SMI X 
             * Description: As per ICH Web Service Response Message specifications 
             * Refer FRS Section 3.3 Table 9. 
             * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */
          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS
          invoice.BilledMember = _nonSamplingInvoiceManager.GetBilledMember(invoice.BilledMemberId);
          if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowSmiXWebServiceErrorMessage(exception.Message);
          }
          else //if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase))
          {
              ShowErrorMessage(exception.ErrorCode);
          }

        //TFS 9343: SIT CMP 624 : System is enabling the disabled field SMI in case of SM X.
        ViewData[ViewDataConstants.TransactionExists] = _nonSamplingInvoiceManager.IsTransactionExists(invoiceId);
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
      /* This is added to avoid concurrent changes to invoice header. For Details please refer TFS bug #9268 */
      catch (UpdateException exception)
      {
          if (exception != null && exception.InnerException != null && exception.InnerException.Message != null &&
              exception.InnerException.Message.ToUpper().Contains("ORA-20001"))
          {
              ShowErrorMessage(ErrorCodes.PreventConcurrentInvoiceHeaderUpdate);
          }
          //TFS 9343: SIT CMP 624 : System is enabling the disabled field SMI in case of SM X.
          ViewData[ViewDataConstants.TransactionExists] = _nonSamplingInvoiceManager.IsTransactionExists(invoiceId);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      MakeInvoiceRenderReady(invoice.Id, invoice);

      // Currently, not all invoices has corresponding InvoiceTotal record entries 
      // in database, hence creating empty object of InvoiceTotal object.
      if (invoice.InvoiceTotalRecord == null)
      {
        invoice.InvoiceTotalRecord = new InvoiceTotal();
      }

      // Create Source Code grid instance
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
      SetViewDataPageMode(PageMode.Edit);

      return View(invoice);
    }

    /// <summary>
    /// Delete invoice
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult InvoiceDelete(string invoiceId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        var isDeleted = _nonSamplingInvoiceManager.DeleteInvoice(invoiceId);

        details = GetDeleteMessage(isDeleted);

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);

        return Json(details);
      }
    }

    /// <summary>
    /// Retrieves invoice details, along with source code list and invoice total record for given invoice id.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [Obsolete]
    public ActionResult InvoiceDetails(string invoiceId)
    {
      // Currently, not all invoices has corresponding InvoiceTotal record entries 
      // in database, hence creating empty object of InvoiceTotal object.
      if (InvoiceHeader.InvoiceTotalRecord == null)
      {
        InvoiceHeader.InvoiceTotalRecord = new InvoiceTotal();
      }

      // Create Source Code grid instance
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;

      return View(InvoiceHeader);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Validate)]
    [RestrictUnauthorizedUpdate]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public ActionResult ValidateInvoice(string invoiceId)
    {
      ValidateInvoice(_nonSamplingInvoiceManager, invoiceId);

      return RedirectToAction("Edit");
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Submit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public override ActionResult Submit(string invoiceId)
    {
      return base.Submit(invoiceId);
    }

    /// <summary>
    /// Display Prime Billing coupons in Invoice 
    /// </summary>
    //[ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult PrimeBillingList(string invoiceId)
    {
      //Create grid instance for PrimeBilling grid
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }
      var primeBillingGrid = new PrimeBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action(PrimeBillingGridAction, new { invoiceId }), isRejectionAllowed);
      ViewData[ViewDataConstants.PrimeBillingGrid] = primeBillingGrid.Instance;

      return View(InvoiceHeader);
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult PrimeBillingListView(string invoiceId)
    {
      // Create grid instance for PrimeBilling grid
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }
      var primeBillingGrid = new PrimeBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action(PrimeBillingGridAction, new { invoiceId }), isRejectionAllowed);
      ViewData[ViewDataConstants.PrimeBillingGrid] = primeBillingGrid.Instance;

      return View("PrimeBillingList", InvoiceHeader);
    }

    /// <summary>
    /// Fetch data for Prime Billing Coupons for Invoice and display it in grid
    /// </summary>
    public JsonResult PrimeBillingGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var primeBillingGrid = new PrimeBillingGrid(ControlIdConstants.CouponGridId, Url.Action(PrimeBillingGridAction, new { invoiceId }));
      var primeBillingCoupons = _nonSamplingInvoiceManager.GetPrimeBillingCouponList(invoiceId).AsQueryable();

      return primeBillingGrid.DataBind(primeBillingCoupons);
    }

    /// <summary>
    /// Delete prime billing record
    /// </summary>
    /// <param name="transactionId">Id of prime billing record which is to be deleted</param>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_COUPON_RECORD)]
    public JsonResult PrimeBillingDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        var isDeleted = _nonSamplingInvoiceManager.DeleteCouponRecord(transactionId);

        details = GetDeleteMessage(isDeleted);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Displays invoice header and screen for prime billing coupon creation.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult PrimeBillingCreate(string invoiceId)
    {
      SetViewDataPageMode(PageMode.Create);

      var couponRecord = new PrimeCoupon
                           {
                             Invoice = InvoiceHeader,
                             InvoiceId = invoiceId.ToGuid(),
                             ElectronicTicketIndicator = true,
                             TicketOrFimIssuingAirline = InvoiceHeader.BilledMember != null ? InvoiceHeader.BilledMember.MemberCodeNumeric : string.Empty,
                             AirlineFlightDesignator = InvoiceHeader.BillingMember != null ? InvoiceHeader.BillingMember.MemberCodeAlpha : string.Empty,
                             LastUpdatedBy = SessionUtil.UserId,
                             CheckDigit = 9
                           };

      // Set ViewData, "IsPostback" to false
      ViewData[ViewDataConstants.IsPostback] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData[TempDataConstants.PrimeCouponRecord] != null)
      {
        // Set Viewdata
        ViewData[ViewDataConstants.PrimeCouponRecord] = TempData[TempDataConstants.PrimeCouponRecord];
      }

      return View(couponRecord);
    }

    /// <summary>
    /// Creates prime billing coupon and redirect to the GET version of this action.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "PrimeBillingCreate")]
    public ActionResult PrimeBillingCreate(string invoiceId, PrimeCoupon couponRecord)
    {
      var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRecord.TaxAmount, pcTaxBreakdowns: couponRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 1 */
          MemberManager.ValidateIssuingAirline(couponRecord.TicketOrFimIssuingAirline);
        string duplicateCouponErrorMessage;

        couponRecord.Attachments.Clear();
        couponRecord.LastUpdatedBy = SessionUtil.UserId;
        _nonSamplingInvoiceManager.AddCouponRecord(couponRecord, out duplicateCouponErrorMessage);
        _nonSamplingInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, couponRecord.Id);
        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateCouponErrorMessage);
        ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful);
        if(! String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData[TempDataConstants.PrimeCouponRecord] = string.Format(@"{0}-{1}-{2}", couponRecord.SourceCodeId, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch + 1);

        return RedirectToAction("PrimeBillingCreate", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
      }

      couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(couponRecord);
    }

    /// <summary>
    /// Creates prime billing coupon and allows to duplicate same record.
    /// </summary>
    
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "PrimeBillingCreate")]
    public ActionResult PrimeBillingDuplicate(string invoiceId, PrimeCoupon couponRecord)
    {
      var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRecord.TaxAmount, pcTaxBreakdowns: couponRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 1 */
          MemberManager.ValidateIssuingAirline(couponRecord.TicketOrFimIssuingAirline);
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        string duplicateErrorMessage;

        couponRecord.Attachments.Clear();

        _nonSamplingInvoiceManager.AddCouponRecord(couponRecord, out duplicateErrorMessage);
        _nonSamplingInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, couponRecord.Id);

        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateErrorMessage, false);
        ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful, false);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        SetViewDataPageMode(PageMode.Clone);

        // Increment sequence no by 1
        ModelState.SetModelValue("RecordSequenceWithinBatch",
                                 new ValueProviderResult(couponRecord.RecordSequenceWithinBatch + 1, (couponRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));
        //Initialize CheckDigit to 9
        ModelState.SetModelValue("CheckDigit", new ValueProviderResult(9, (couponRecord.CheckDigit).ToString(), CultureInfo.InvariantCulture));
        
        //CMP #672: Validation on Taxes in PAX FIM Billings
        if (couponRecord.SourceCodeId == FimSourceCode14Pax)
        {
            ModelState.SetModelValue("CouponTotalAmount", new ValueProviderResult(couponRecord.CouponTotalAmount.ToString(), couponRecord.CouponTotalAmount.ToString(), CultureInfo.InvariantCulture));
        }

        couponRecord.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
        // Done for Attachment upload issue after 'save and duplicate'. Transaction Id should be empty for a fresh capture of the transaction.
        couponRecord.Id = Guid.Empty;
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("PrimeBillingCreate", couponRecord);
    }

    /// <summary>
    /// Creates prime billing coupon and redirects to prime billing coupon listing
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "PrimeBillingCreate")]
    public ActionResult PrimeBillingCreateAndReturn(string invoiceId, PrimeCoupon couponRecord)
    {
      var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRecord.TaxAmount, pcTaxBreakdowns: couponRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 1 */
          MemberManager.ValidateIssuingAirline(couponRecord.TicketOrFimIssuingAirline);
        string duplicateErrorMessage;
        couponRecord.Attachments.Clear();
        couponRecord.LastUpdatedBy = SessionUtil.UserId;
        _nonSamplingInvoiceManager.AddCouponRecord(couponRecord, out duplicateErrorMessage);
        _nonSamplingInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, couponRecord.Id);
        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateErrorMessage);
        ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);


        TempData[TempDataConstants.PrimeCouponRecord] = "";

        return RedirectToAction("PrimeBillingList", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
      }

      couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("PrimeBillingCreate", couponRecord);
    }

    /// <summary>
    /// Retrieves prime billing coupon details for given coupon id.
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    public ActionResult PrimeBillingEdit(string invoiceId, string transactionId)
    {
      var couponRecord = GetCouponRecord(transactionId, invoiceId);

      // Set Airline flight designator to Billing Member name
      couponRecord.AirlineFlightDesignator = couponRecord.Invoice.BillingMember.MemberCodeAlpha;

      return View(couponRecord);
    }

    private PrimeCoupon GetCouponRecord(string transactionId, string invoiceId)
    {
      var couponRecord = _nonSamplingInvoiceManager.GetCouponRecordDetails((transactionId));
      couponRecord.Invoice = InvoiceHeader;
      couponRecord.LastUpdatedBy = SessionUtil.UserId;
      return couponRecord;
    }

    //SCP419601: PAX permissions issue
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    [HttpGet]
    public ActionResult PrimeBillingView(string invoiceId, string transactionId)
    {
      var couponRecord = GetCouponRecord(transactionId, invoiceId);

      return View("PrimeBillingEdit", couponRecord);
    }

    /// <summary>
    /// Update prime billing coupon details for given coupon id and redirect to Get version of same action.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult PrimeBillingEdit(string invoiceId, string transactionId, PrimeCoupon couponRecord)
    {
      couponRecord.Id = transactionId.ToGuid();
      couponRecord.LastUpdatedBy = SessionUtil.UserId;
      foreach (var tax in couponRecord.TaxBreakdown)
      {
        tax.ParentId = couponRecord.Id;
      }

      foreach (var vat in couponRecord.VatBreakdown)
      {
        vat.ParentId = couponRecord.Id;
      }

      var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRecord.TaxAmount, pcTaxBreakdowns: couponRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 1 */
          MemberManager.ValidateIssuingAirline(couponRecord.TicketOrFimIssuingAirline);
        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateCouponRecord(couponRecord, out duplicateErrorMessage);

        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
        ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);

        TempData[TempDataConstants.PrimeCouponRecord] = string.Format(@"{0}-{1}-{2}", couponRecord.SourceCodeId, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch + 1);
        return RedirectToAction("PrimeBillingCreate", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        couponRecord.Id = transactionId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(couponRecord);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "PrimeBillingEdit")]
    public ActionResult PrimeBillingClone(string invoiceId, string transactionId, PrimeCoupon couponRecord)
    {
      var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();

      couponRecord.Id = transactionId.ToGuid();

      foreach (var tax in couponRecord.TaxBreakdown)
      {
        tax.ParentId = couponRecord.Id;
      }

      foreach (var vat in couponRecord.VatBreakdown)
      {
        vat.ParentId = couponRecord.Id;
      }

      try
      {

          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRecord.TaxAmount, pcTaxBreakdowns: couponRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 1 */
          MemberManager.ValidateIssuingAirline(couponRecord.TicketOrFimIssuingAirline);

        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateCouponRecord(couponRecord, out duplicateErrorMessage);

        couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage, false);
        ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful , false);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        couponRecord.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;

        // Increment sequence no by 1
        ModelState.SetModelValue("RecordSequenceWithinBatch",
                                 new ValueProviderResult(couponRecord.RecordSequenceWithinBatch + 1, (couponRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));
        //Initialize CheckDigit to 9
        ModelState.SetModelValue("CheckDigit", new ValueProviderResult(9, (couponRecord.CheckDigit).ToString(), CultureInfo.InvariantCulture));

        // Done for Attachment upload issue after 'save and duplicate'. Transaction Id should be empty for a fresh capture of the transaction.
        couponRecord.Id = Guid.Empty;

        return View("PrimeBillingCreate", couponRecord);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
        couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        // SCP190774: ERROR SAVING RM
        couponRecord.Id = transactionId.ToGuid();
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("PrimeBillingEdit", couponRecord);
    }

    private void ShowSuccessMessages(string message, int vatRecordCountBefore, int vatRecordCountAfter)
    {
      if (CheckIfVatBreakdownDeleted(vatRecordCountBefore, vatRecordCountAfter))
      {
        message += string.Format(" {0}", Messages.VatRecordsDeletedInfo);
      }

      ShowSuccessMessage(message);
    }

    private static bool CheckIfVatBreakdownDeleted(int vatCountBefore, int vatCouponAfter)
    {
      return vatCountBefore > vatCouponAfter;
    }

    /// <summary>
    /// Updates coupon record and returns the user to prime billing coupon list.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "PrimeBillingEdit")]
    public ActionResult PrimeBillingEditAndReturn(string invoiceId, string transactionId, PrimeCoupon couponRecord)
    {
      var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRecord.TaxAmount, pcTaxBreakdowns: couponRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 1 */
          MemberManager.ValidateIssuingAirline(couponRecord.TicketOrFimIssuingAirline);
        couponRecord.Id = transactionId.ToGuid();
        couponRecord.LastUpdatedBy = SessionUtil.UserId;
        foreach (var tax in couponRecord.TaxBreakdown)
        {
          tax.ParentId = couponRecord.Id;
        }
        foreach (var vat in couponRecord.VatBreakdown)
        {
          vat.ParentId = couponRecord.Id;
        }

        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateCouponRecord(couponRecord, out duplicateErrorMessage);

        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
        ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("PrimeBillingList", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        couponRecord.Attachments = _nonSamplingInvoiceManager.GetCouponRecordAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        couponRecord.Id = transactionId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      couponRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("PrimeBillingEdit", couponRecord);
    }

    /// <summary>
    /// Gets billing memo list and displays in grid for given invoice.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult BMList(string invoiceId)
    {
      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
      {
        return RedirectToAction("BMListView", new { invoiceId });
      }

      // Create grid instance for Billing Memo grid
      var billingMemoGrid = new BillingMemoGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.BillingMemoGrid] = billingMemoGrid.Instance;

      return View(InvoiceHeader);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult BMListView(string invoiceId)
    {
      // Create grid instance for Billing Memo grid
      var billingMemoGrid = new BillingMemoGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
      ViewData[ViewDataConstants.BillingMemoGrid] = billingMemoGrid.Instance;

      return View("BMList", InvoiceHeader);
    }

    /// <summary>
    /// Fetch billing memo for Invoice
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult BillingMemoGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var billingMemoGrid = new BillingMemoGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
      var billingMemoCoupons = _nonSamplingInvoiceManager.GetBillingMemoList(invoiceId).AsQueryable();

      return billingMemoGrid.DataBind(billingMemoCoupons);
    }

    /// <summary>
    /// Delete billing memo
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_BILLING_MEMO)]
    public JsonResult BMDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        var isDeleted = _nonSamplingInvoiceManager.DeleteBillingMemoRecord(transactionId);

        details = GetDeleteMessage(isDeleted);

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);

        return Json(details);
      }
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult BMCreate(string invoiceId)
    {
      SetViewDataPageMode(PageMode.Create);

      var billingMemoRecord = new BillingMemo { Invoice = InvoiceHeader, InvoiceId = invoiceId.ToGuid(), LastUpdatedBy = SessionUtil.UserId };

      if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
      {
        var correspondenceRefNumber = TempData[TempDataConstants.CorrespondenceNumber].ToString();

        KeepBillingHistoryDataInStore(true);



        var correspondenceManager = Ioc.Resolve<IPaxCorrespondenceManager>(typeof(IPaxCorrespondenceManager));
        //INC 8863, I get an unexpected error occurred. 
        var correspondence = correspondenceManager.GetRecentCorrespondenceDetails(Convert.ToInt64(correspondenceRefNumber), true);
        //var invoiec = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(correspondence.InvoiceId.ToString());

        //INC 8863, I get an unexpected error occurred. 
        if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Closed || (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && correspondence.AuthorityToBill))
        {
          billingMemoRecord.ReasonCode = "6A";
        }
        else if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
        {
          billingMemoRecord.ReasonCode = "6B";
        }

        var sourceCodeList = _referenceManager.GetSourceCodeList(billingMemoRecord.ReasonCode == "6A" ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill : (int)TransactionType.PasNsBillingMemoDueToExpiry).Where(sc => sc.SourceCodeIdentifier == 9).ToList();

        // For pre-population of the only source code for Credit Memo.
        billingMemoRecord.SourceCodeId = sourceCodeList.Count != 0 ? sourceCodeList[0].SourceCodeIdentifier : 9;


        if (billingMemoRecord.Invoice.ListingCurrencyId != null && correspondence.CurrencyId != null)
        {
          try
          {
            // CMP#624 : 2.10-Change#4 - Conditional validation of PAX/CGO 6A/6B BM amounts  
            var yourInvoice = _invoiceManager.GetInvoiceHeader(correspondence.InvoiceId.ToString());
            //SCP219674 : InvalidAmountToBeSettled Validation
            decimal netAmountBilled = billingMemoRecord.NetAmountBilled;
            var isValid = ReferenceManager.ValidateCorrespondenceAmounttobeSettled(billingMemoRecord.Invoice,
                                                                               ref netAmountBilled,
                                                                               correspondence.CurrencyId.Value,
                                                                               correspondence.AmountToBeSettled, yourInvoice);

            billingMemoRecord.NetAmountBilled =
            billingMemoRecord.TotalGrossAmountBilled = netAmountBilled;
          }
          catch (ISBusinessException businessException)
          {
            ShowErrorMessage(businessException.ErrorCode);
          }
        }

        billingMemoRecord.CorrespondenceRefNumber = Convert.ToInt64(correspondenceRefNumber);
        ViewData[ViewDataConstants.FromBillingHistory] = true;
      }

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      if (billingMemoRecord.CorrespondenceRefNumber == 0)
      {
        billingMemoRecord.CorrespondenceRefNumber = new Business.Web.PaxNonSamplingInvoiceManager().GetPaxDatabaseCorrRefNumber(billingMemoRecord.Id);
      }

      return View(billingMemoRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMCreate(string invoiceId, BillingMemo record)
    {
      var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        record.LastUpdatedBy = SessionUtil.UserId;
        record.Attachments.Clear();
        
        // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
        // On Create/Edit screen for BM with reason code other than 6A/6B, correspondence ref no value will be stored as NULL in database.
        // Here we are making as -1, later it will update to NULL by database trigger.
        _nonSamplingInvoiceManager.AddBillingMemoRecord(record, Convert.ToString(Request.Form["UserCorrRefNo"]).Equals("-1"));

        _nonSamplingInvoiceManager.UpdateBillingMemoAttachment(billingMemoAttachmentIds, record.Id);

        TempData.Clear();
        ShowSuccessMessage(Messages.BMCreateSuccessful);

        // On clicking Save in case of adding  a new billing memo record, 
        // if the selected reason code mandates coupon breakdown, then system should automatically open the BM Coupon Breakdown screen for data capture.
        if (record.CouponAwbBreakdownMandatory)
          return RedirectToAction("BMCouponCreate", new { invoiceId, transactionId = record.Id.Value() });

        return RedirectToAction("BMEdit", new { invoiceId, transactionId = record.Id.Value() });
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoAttachments(billingMemoAttachmentIds);
      }

      KeepBillingHistoryDataInStore(true);

      if (TempData.ContainsKey("correspondenceNumber"))
      {
        ViewData[ViewDataConstants.FromBillingHistory] = true;
      }
      record.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(record);
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    public ActionResult BMEdit(string invoiceId, string transactionId)
    {
      var billingMemoRecord = GetBillingMemoRecord(invoiceId, transactionId);

      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingInvoiceManager.GetBillingMemoCouponCount(transactionId) > 0 ? true : false;

      //Initialize BMCoupon grid
      var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      if (billingMemoRecord.CorrespondenceRefNumber == 0)
      {
        billingMemoRecord.CorrespondenceRefNumber = new Iata.IS.Business.Web.PaxNonSamplingInvoiceManager().GetPaxDatabaseCorrRefNumber(billingMemoRecord.Id);
      }

      return View(billingMemoRecord);
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult BMView(string invoiceId, string transactionId)
    {
      var billingMemoRecord = GetBillingMemoRecord(invoiceId, transactionId);

      // Initialize BMCoupon grid
      var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      if (billingMemoRecord.CorrespondenceRefNumber == 0)
      {
        billingMemoRecord.CorrespondenceRefNumber = new Iata.IS.Business.Web.PaxNonSamplingInvoiceManager().GetPaxDatabaseCorrRefNumber(billingMemoRecord.Id);
      }

      return View("BMEdit", billingMemoRecord);
    }

    private BillingMemo GetBillingMemoRecord(string invoiceId, string transactionId)
    {
      var billingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      billingMemo.Invoice = InvoiceHeader;
      billingMemo.LastUpdatedBy = SessionUtil.UserId;

      var transactionTypeId = billingMemo.ReasonCode == "6A"
                                    ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
                                    : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(billingMemo.ReasonCode, transactionTypeId).CouponAwbBreakdownMandatory;

      // Set Coupon breakdown value
      billingMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      return billingMemo;
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMEdit(string invoiceId, string transactionId, BillingMemo record)
    {
      var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        record.Id = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        //Assign parent id for VAt records
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }

        // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
        // On Create/Edit screen for BM with reason code other than 6A/6B, correspondence ref no value will be stored as NULL in database.
        // Here we are making as -1, later it will update to NULL by database trigger.
        _nonSamplingInvoiceManager.UpdateBillingMemoRecord(record, Convert.ToString(Request.Form["UserCorrRefNo"]).Equals("-1"));

        ShowSuccessMessage(Messages.BMUpdateSuccessful);
        return RedirectToAction("BMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException be)
      {
        ShowErrorMessage(be.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoAttachments(billingMemoAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      record.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Initialize BMCoupon grid
      var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingInvoiceManager.GetBillingMemoCouponCount(transactionId) > 0 ? true : false;

      return View(record);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMEdit")]
    public ActionResult BMEditAndAddNew(string invoiceId, string transactionId, BillingMemo billingMemoRecord)
    {
      var billingMemoAttachmentIds = billingMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        billingMemoRecord.Id = transactionId.ToGuid();
        billingMemoRecord.LastUpdatedBy = SessionUtil.UserId;
        //Assign parent id for Vat records
        foreach (var vat in billingMemoRecord.VatBreakdown)
        {
          vat.ParentId = billingMemoRecord.Id;
        }

        // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
        // On Create/Edit screen for BM with reason code other than 6A/6B, correspondence ref no value will be stored as NULL in database.
        // Here we are making as -1, later it will update to NULL by database trigger.
        _nonSamplingInvoiceManager.UpdateBillingMemoRecord(billingMemoRecord, Convert.ToString(Request.Form["UserCorrRefNo"]).Equals("-1"));

        ShowSuccessMessage(Messages.BMUpdateSuccessful);
        return RedirectToAction("BMCreate", new { invoiceId, transactionId });
      }
      catch (ISBusinessException be)
      {
        ShowErrorMessage(be.ErrorCode);
        billingMemoRecord.Attachments = _nonSamplingInvoiceManager.GetBillingMemoAttachments(billingMemoAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      billingMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Initialize BMCoupon grid
      var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingInvoiceManager.GetBillingMemoCouponCount(transactionId) > 0 ? true : false;

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      if (billingMemoRecord.CorrespondenceRefNumber == 0)
      {
        billingMemoRecord.CorrespondenceRefNumber = new Iata.IS.Business.Web.PaxNonSamplingInvoiceManager().GetPaxDatabaseCorrRefNumber(billingMemoRecord.Id);
      }

      return View("BMEdit", billingMemoRecord);
    }

    /// <summary>
    /// Upload Billing Memo attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult BillingMemoAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<BillingMemoAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;

      try
      {
          Logger.Info("Started execution for method BillingMemoAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Get the invoice header details 
        var invoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);
        Logger.Info("Fetched all invoice details successfully.");
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          Logger.Info("Started saving the file" + fileToSave);
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          //On Billing Memo Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _nonSamplingInvoiceManager.IsDuplicateBillingMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          Logger.Info("Attachment successfully validated");
          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new BillingMemoAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };

            attachment = _nonSamplingInvoiceManager.AddBillingMemoAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new User();
            }
            // assign user info from session and file server info.
            attachment.UploadedBy.Id= SessionUtil.UserId;
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
        Logger.Error("Exception", ex);
      }
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
        Logger.Error("Exception", ex);
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Billing Memo attachment 
    ///  </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionId">Transaction id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download)]
    [HttpGet]
    public FileStreamResult BillingMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetBillingMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Display Rejection Memo in Invoice 
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult RMList(string invoiceId)
    {
      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
      {
        return RedirectToAction("RMListView", new { invoiceId });
      }

      // Clear rejection memo data from Tempdata.
      TempData.Clear();

      // Create grid instance for Rejection Memo grid
      var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "Invoice", new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      return View(InvoiceHeader);
    }

    /// <summary>
    /// Display Rejection Memo in Invoice 
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult RMListView(string invoiceId)
    {
      // Create grid instance for Rejection Memo grid
      var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "Invoice", new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      return View("RMList", InvoiceHeader);
    }

    /// <summary>
    /// returns rejection memo create screen along with given invoice header details.
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult RMCreate(string invoiceId)
    {
      SetViewDataPageMode(PageMode.Create);

      var record = new RejectionMemo { Invoice = InvoiceHeader, InvoiceId = invoiceId.ToGuid(), LastUpdatedBy = SessionUtil.UserId };
        
      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        var originalRejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();
        var rejectedRecordIds = originalRejectedRecordIds;

        if (rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf('@') + 1)
        {
          var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
          rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf('@'));
          if (rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf(';') + 1)
          {
            var transactionType = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf(';') + 1);
            rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf(';'));
            var rejectionIdList = rejectedRecordIds.Split(',');

            var rejectedInvoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
            ViewData[ViewDataConstants.FromBillingHistory] = true;
            if (rejectedInvoice != null)
            {
              record.YourInvoiceNumber = rejectedInvoice.InvoiceNumber;
              record.YourInvoiceBillingYear = rejectedInvoice.BillingYear;
              record.YourInvoiceBillingMonth = rejectedInvoice.BillingMonth;
              record.YourInvoiceBillingPeriod = rejectedInvoice.BillingPeriod;
              record.IsLinkingSuccessful = true;

              // transaction type being rejected
              switch (transactionType)
              {
                case "PC":
                  record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.None;
                  break;
                case "BM":
                  record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.BMNumber;
                  break;
                case "CM":
                  record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.CMNumber;
                  break;
                default:
                  record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.None;
                  break;
              }
            }

            if (transactionType == "PC")
            {
              var coupon = _nonSamplingInvoiceManager.GetCouponRecordDetails(rejectionIdList[0]);
              record.RejectionStage = 1;
              record.SourceCodeId = coupon.SourceCodeId;

              if (coupon.SourceCodeId == 14) // FIM
              {
                record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.FIMNumber;
                record.FimBMCMNumber = coupon.TicketDocOrFimNumber.ToString();
                record.FimCouponNumber = coupon.TicketOrFimCouponNumber;
              }
            }
            else if (transactionType == "BM")
            {
              record.RejectionStage = 1;
              var billingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(rejectionIdList[0]);
              record.FimBMCMNumber = billingMemo.BillingMemoNumber;
              record.FimCouponNumber = billingMemo.FimCouponNumber;
              record.FIMBMCMIndicator = FIMBMCMIndicator.BMNumber;
              record.SourceCodeId = billingMemo.SourceCodeId;
             
            }
            else if (transactionType == "CM")
            {
              record.RejectionStage = 1;
              var creditNoteManager = Ioc.Resolve<INonSamplingCreditNoteManager>(typeof(INonSamplingCreditNoteManager));
              var creditMemo = creditNoteManager.GetCreditMemoRecordDetails(rejectionIdList[0]);
              record.FimBMCMNumber = creditMemo.CreditMemoNumber;
              record.FIMBMCMIndicator = FIMBMCMIndicator.CMNumber;
              record.SourceCodeId = creditMemo.SourceCodeId;

            }
            else if (transactionType == "RM")
            {
              var rejectedMemo = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(rejectionIdList[0]);
              if (rejectedMemo != null)
              {
                record.SourceCodeId = rejectedMemo.SourceCodeId;
                record.RejectionStage = ++rejectedMemo.RejectionStage;
                record.YourRejectionNumber = rejectedMemo.RejectionMemoNumber;
                if (rejectedMemo.FIMBMCMIndicatorId == 2)
                {
                  record.FIMBMCMIndicatorId = 2;
                  record.FimBMCMNumber = rejectedMemo.FimBMCMNumber;
                  record.FimCouponNumber = rejectedMemo.FimCouponNumber;
                }
                else if (rejectedMemo.FIMBMCMIndicatorId == 3)
                {
                  record.FIMBMCMIndicatorId = 3;
                  record.FimBMCMNumber = rejectedMemo.FimBMCMNumber;
                }
                else if (rejectedMemo.FIMBMCMIndicatorId == 4)
                {
                  record.FIMBMCMIndicatorId = 4;
                  record.FimBMCMNumber = rejectedMemo.FimBMCMNumber;
                }
                else if ((rejectedMemo.FIMBMCMIndicatorId == 0 || rejectedMemo.FIMBMCMIndicatorId == 1) && (rejectedMemo.SourceCodeId ==44 || rejectedMemo.SourceCodeId == 45) )
                {
                    rejectedMemo.FIMBMCMIndicatorId = 2;
                    record.FimBMCMNumber = rejectedMemo.FimBMCMNumber;
                    record.FimCouponNumber = rejectedMemo.FimCouponNumber;
                }

                //if (rejectedMemo.CouponBreakdownRecord.Count == 0)
                //{
                //  record.TotalGrossAcceptedAmount = rejectedMemo.TotalGrossAcceptedAmount;
                //  record.TotalGrossAmountBilled = rejectedMemo.TotalGrossAcceptedAmount;
                //  record.AcceptedHandlingFee = rejectedMemo.AcceptedHandlingFee;
                //  record.AllowedHandlingFee = rejectedMemo.AcceptedHandlingFee;
                //  record.AcceptedIscAmount = rejectedMemo.AcceptedIscAmount;
                //  record.AllowedIscAmount = rejectedMemo.AcceptedIscAmount;
                //  record.AcceptedOtherCommission = rejectedMemo.AcceptedOtherCommission;
                //  record.AllowedOtherCommission = rejectedMemo.AcceptedOtherCommission;
                //  record.AcceptedUatpAmount = rejectedMemo.AcceptedUatpAmount;
                //  record.AllowedUatpAmount = rejectedMemo.AcceptedUatpAmount;
                //}
              }
            }
          }
        }

        TempData[TempDataConstants.RejectedRecordIds] = originalRejectedRecordIds;
      }

      return View(record);
    }

    /// <summary>
    /// Validate FIMNumber and FIMCouponNumber for rejection memo having source code 44/45/46.
    /// </summary>
    /// <param name="rejectionMemo">RejectionMemo</param>
    /// <returns>Error Code if validation for FIMNumber and FIMCouponNumber fails.</returns>
    private static string ValidateRMFimDetails(RejectionMemo rejectionMemo)
    {
        if (rejectionMemo != null)
        {
            // Variable in which FimBMCMNumber is parsed. 
            long rmFimBmCmNumber;

            // FIMBMCMNumber check for blank and 0 value. Check done for source code 45/46.
            if (rejectionMemo.IsLinkingSuccessful == false && (rejectionMemo.SourceCodeId == 45 || rejectionMemo.SourceCodeId == 46) && (string.IsNullOrEmpty(rejectionMemo.FimBMCMNumber) || (Validators.IsWholeNumber(rejectionMemo.FimBMCMNumber) && Int64.TryParse(rejectionMemo.FimBMCMNumber, out rmFimBmCmNumber) && rmFimBmCmNumber == 0)))
            {
                return ErrorCodes.MandatoryFimNumberAndFimCouponNumber;
            }// End if

            // FIMBMCMNumber check for invalid format i.e. should be numeric. Check done for source code 45/46.
            if ((rejectionMemo.SourceCodeId == 44 || rejectionMemo.SourceCodeId == 45 || rejectionMemo.SourceCodeId == 46) && !string.IsNullOrEmpty(rejectionMemo.FimBMCMNumber) && !Validators.IsWholeNumber(rejectionMemo.FimBMCMNumber))
            {
                return ErrorCodes.InvalidFimNumber;
            }// End if

            // FimCouponNumber check for blank and 0 value for source code 44/45/46.
            // SCP203896: Missing Prev Doc Num on SFI 21 Element 18
            // Added check for IsLinkingSuccessful is null
            if ((rejectionMemo.SourceCodeId == 44 || ((rejectionMemo.SourceCodeId == 45 || rejectionMemo.SourceCodeId == 46) && (rejectionMemo.IsLinkingSuccessful == false || rejectionMemo.IsLinkingSuccessful == null))) && (string.IsNullOrEmpty(rejectionMemo.FimBMCMNumber) || rejectionMemo.FimCouponNumber == 0))
            {
                return ErrorCodes.MandatoryFimNumberAndFimCouponNumber;
            }// End if

        }// End if null check.

        return string.Empty;

    }// End ValidateRMFimDetails()

    #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP

    /// <summary>
    /// Method is used to check the valid Your Billing Year/Month/Period
    /// </summary>
    /// <param name="yourInvoiceBillingYear"></param>
    /// <param name="yourInvoiceBillingMonth"></param>
    /// <param name="yourInvoiceBillingPeriod"></param>
    /// <returns>true or false</returns>
    private bool IsYourBillingDateValid(int yourInvoiceBillingYear, int yourInvoiceBillingMonth, int yourInvoiceBillingPeriod)
    {
      DateTime yourInvoiceBillingDate;
      var yourInvoiceDateString = string.Format("{2}{1}{0}", Convert.ToString(yourInvoiceBillingPeriod).PadLeft(2, '0'),
                                                             Convert.ToString(yourInvoiceBillingMonth).PadLeft(2, '0'),
                                                             Convert.ToString(yourInvoiceBillingYear).PadLeft(4, '0'));

      if (DateTime.TryParseExact(yourInvoiceDateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out yourInvoiceBillingDate))
      {
        if (yourInvoiceBillingDate.Day < 1 || yourInvoiceBillingDate.Day > 4)
        {
          return false;
        }
      }
      else
      {
        return false;
      }
      return true;
    }

    #endregion

    /// <summary>
    /// Creates new rejection memo and allows user to add new record.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="record"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMCreate(string invoiceId, RejectionMemo record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        
        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (!IsYourBillingDateValid(record.YourInvoiceBillingYear, record.YourInvoiceBillingMonth, record.YourInvoiceBillingPeriod))
        {
          throw new ISBusinessException("Invalid Your Invoice Billing Date.","");
        }
        #endregion

        record.Attachments.Clear();
        SetRejectionMemoLinkingDetails(record, invoiceId);
        string errMessage = ValidateRMFimDetails(record);
        //SCPID : 105938 - RE: IATA- SIS Bug
        string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(record);
        if (!string.IsNullOrEmpty(errMessage))
        {
          ShowErrorMessage(errMessage, true);
          ViewData[ViewDataConstants.IsPostback] = true;
        }
        else if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
        {
            ShowCustomErrorMessage(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage, true);
            ViewData[ViewDataConstants.IsPostback] = true;
        }
        else if (record.FIMBMCMIndicatorId == 2 && string.IsNullOrEmpty(record.ReasonCode) && string.IsNullOrEmpty(record.FimBMCMNumber) && record.FimCouponNumber == null)
        {
            string warningMessage = "Please enter the remaining data before saving if you intend to reject a non-migrated transaction.";
            if (!String.IsNullOrEmpty(warningMessage))
                ShowErrorMessage(warningMessage, true);
            //ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
            //record.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoAttachments(couponAttachmentIds);
            ViewData[ViewDataConstants.IsPostback] = true;
        }
        else
        {

          //CMP614: Source Code Validation for PAX RMs
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
          * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(record, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
            throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);

            string errorMessage, warningMessage;
            
            record.LastUpdatedBy = SessionUtil.UserId;
            _nonSamplingInvoiceManager.AddRejectionMemoRecord(record, out errorMessage, out warningMessage);

            _nonSamplingInvoiceManager.UpdateRejectionMemoAttachment(couponAttachmentIds, record.Id);

            GetRejectCouponListFromBillingHistory(record);

            // TempData.Clear();

            ShowSuccessMessage(string.Format("{0}{1}{2}", errorMessage, Environment.NewLine, Messages.RMCreateSuccessful));

            if (!String.IsNullOrEmpty(warningMessage))
                ShowErrorMessage(warningMessage, true);
            // On clicking Save in case of adding  a new rejection record, 
            // if the selected reason code mandates coupon breakdown, then system should automatically open the RM Coupon Breakdown screen for data capture.
            // Also if the user came from billing history then no need to go to Coupon create page.
            return RedirectToAction("RMEdit", new {invoiceId, transactionId = record.Id.Value()});
        }
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        record.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoAttachments(couponAttachmentIds);
        ViewData[ViewDataConstants.IsPostback] = true;
      }

      // Set all details of rejection memo if it is from billing history.
      SetRejectionMemoDetailsForBillingHistory(record);

      KeepBillingHistoryDataInStore(true);

      record.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      return View(record);
    }

    /// <summary>
    /// Creates new rejection memo and allows user to add new record.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="record"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMCreate")]
    public ActionResult RMCreateAndAddNew(string invoiceId, RejectionMemo record)
    {
      PaxInvoice invoice = null;
      var logRefId = Guid.NewGuid();

      var log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                               BillingCategorys.Passenger.ToString(), "Stage 1: In RMCreateAndAddNew Start ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (!IsYourBillingDateValid(record.YourInvoiceBillingYear, record.YourInvoiceBillingMonth, record.YourInvoiceBillingPeriod))
        {
          throw new ISBusinessException("Invalid Your Invoice Billing Date.", "");
        }
        #endregion

        record.Attachments.Clear();
        record.LastUpdatedBy = SessionUtil.UserId;

        var errorMessage = string.Empty;
        var warningMessage = string.Empty;

          //=====================
        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                             BillingCategorys.Passenger.ToString(), "Stage 2: SetRejectionMemoLinkingDetails Start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        SetRejectionMemoLinkingDetails(record, invoiceId);

        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                             BillingCategorys.Passenger.ToString(), "Stage 2: SetRejectionMemoLinkingDetails Completed ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        string errMessage = ValidateRMFimDetails(record);

        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                          BillingCategorys.Passenger.ToString(), "Stage 3: ValidateRMFimDetails Completed ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        //SCPID : 105938 - RE: IATA- SIS Bug
        string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(record);

        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                       BillingCategorys.Passenger.ToString(), "Stage 4: ValidateRMMemoFieldsCalculation Completed ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
        if (!string.IsNullOrEmpty(errMessage))
        {
          ShowErrorMessage(errMessage, true);
          ViewData[ViewDataConstants.IsPostback] = true;
        }
        else if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
        {
            ShowCustomErrorMessage(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage, true);
            ViewData[ViewDataConstants.IsPostback] = true;
        }   
        else if (record.FIMBMCMIndicatorId == 2 && string.IsNullOrEmpty(record.ReasonCode) && string.IsNullOrEmpty(record.FimBMCMNumber) && record.FimCouponNumber == null)
        {
            warningMessage = "Please enter the remaining data before saving if you intend to reject a non-migrated transaction.";
            if (!String.IsNullOrEmpty(warningMessage))
                ShowErrorMessage(warningMessage, true);
            ViewData[ViewDataConstants.IsPostback] = true;
        }
        else
        {
          //CMP614: Source Code Validation for PAX RMs
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
           * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(record, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);

             log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                          BillingCategorys.Passenger.ToString(), "Stage 5: AddRejectionMemoRecord Start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

            _nonSamplingInvoiceManager.AddRejectionMemoRecord(record, out errorMessage, out warningMessage);

            log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                         BillingCategorys.Passenger.ToString(), "Stage 5: AddRejectionMemoRecord Completed ", SessionUtil.UserId, logRefId.ToString());
            _referenceManager.LogDebugData(log);

            _nonSamplingInvoiceManager.UpdateRejectionMemoAttachment(couponAttachmentIds, record.Id);

            log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                         BillingCategorys.Passenger.ToString(), "Stage 6: UpdateRejectionMemoAttachment Completed ", SessionUtil.UserId, logRefId.ToString());
            _referenceManager.LogDebugData(log);

            GetRejectCouponListFromBillingHistory(record);

            log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                       BillingCategorys.Passenger.ToString(), "Stage 6: GetRejectCouponListFromBillingHistory Completed ", SessionUtil.UserId, logRefId.ToString());
            _referenceManager.LogDebugData(log);

            TempData.Clear();

            // ShowSuccessMessage(string.Format("{0}{1}{2} {3}", errorMessage, Environment.NewLine, Messages.RMCreateSuccessful, warningMessage));
            ShowSuccessMessage(string.Format("{0}{1}{2}", errorMessage, Environment.NewLine, Messages.RMCreateSuccessful));

            if (!String.IsNullOrEmpty(warningMessage))
                ShowErrorMessage(warningMessage, true);

            return RedirectToAction("RMCreate", new {invoiceId});
        }
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        record.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoAttachments(couponAttachmentIds);
        ViewData[ViewDataConstants.IsPostback] = true;
      }

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                    BillingCategorys.Passenger.ToString(), "Stage 7: SetRejectionMemoDetailsForBillingHistory start ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      SetRejectionMemoDetailsForBillingHistory(record);

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                    BillingCategorys.Passenger.ToString(), "Stage 8: SetRejectionMemoDetailsForBillingHistory Completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      KeepBillingHistoryDataInStore(true);

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
                 BillingCategorys.Passenger.ToString(), "Stage 9: KeepBillingHistoryDataInStore Completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);


      record.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMCreateAndAddNew", this.ToString(),
               BillingCategorys.Passenger.ToString(), "Stage 10: GetInvoiceHeaderDetails Completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      return View("RMCreate", record);
    }

    /// <summary>
    ///  Set all details of rejection memo if it is from billing history..
    /// </summary>
    /// <param name="record">The record.</param>
    private void SetRejectionMemoDetailsForBillingHistory(RejectionMemo record)
    {
      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        var originalRejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();
        var rejectedRecordIds = originalRejectedRecordIds;

        var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
        rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf('@'));
        var transactionType = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf(';') + 1);
        rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf(';'));
        var rejectionIdList = rejectedRecordIds.Split(',');

        var rejectedInvoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
        ViewData[ViewDataConstants.FromBillingHistory] = true;
        if (rejectedInvoice != null)
        {
          record.YourInvoiceNumber = rejectedInvoice.InvoiceNumber;
          record.YourInvoiceBillingYear = rejectedInvoice.BillingYear;
          record.YourInvoiceBillingMonth = rejectedInvoice.BillingMonth;
          record.YourInvoiceBillingPeriod = rejectedInvoice.BillingPeriod;
          record.IsLinkingSuccessful = true;

          switch (transactionType)
          {
            case "PC":
              record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.None;
              break;
            case "BM":
              record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.BMNumber;
              break;
            case "CM":
              record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.CMNumber;
              break;
            default:
              record.FIMBMCMIndicatorId = (int)FIMBMCMIndicator.None;
              break;
          }
        }

        if (transactionType == "PC")
        {
          var coupon = _nonSamplingInvoiceManager.GetCouponRecordDetails(rejectionIdList[0]);
          record.RejectionStage = 1;
          record.SourceCodeId = coupon.SourceCodeId;
        }
        else if (transactionType == "BM")
        {
          record.RejectionStage = 1;
          var billingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(rejectionIdList[0]);
          record.FimBMCMNumber = billingMemo.BillingMemoNumber;
          record.FIMBMCMIndicator = FIMBMCMIndicator.BMNumber;
          record.SourceCodeId = billingMemo.SourceCodeId;
        }
        else if (transactionType == "CM")
        {
          record.RejectionStage = 1;
          var creditNoteManager = Ioc.Resolve<INonSamplingCreditNoteManager>(typeof(INonSamplingCreditNoteManager));
          var creditMemo = creditNoteManager.GetCreditMemoRecordDetails(rejectionIdList[0]);
          record.FimBMCMNumber = creditMemo.CreditMemoNumber;
          record.FIMBMCMIndicator = FIMBMCMIndicator.CMNumber;
          record.SourceCodeId = creditMemo.SourceCodeId;
        }
        else if (transactionType == "RM")
        {
          var rejectedMemo = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(rejectionIdList[0]);
          if (rejectedMemo != null)
          {
            record.SourceCodeId = rejectedMemo.SourceCodeId;
            record.RejectionStage = ++rejectedMemo.RejectionStage;
            record.YourRejectionNumber = rejectedMemo.RejectionMemoNumber;
          }
        }
        TempData[TempDataConstants.RejectedRecordIds] = originalRejectedRecordIds;
      }
    }

    /// <summary>
    /// Sets the rejection memo linking details.
    /// </summary>
    /// <param name="record">The record.</param>
    /// <param name="invoiceId">The invoice id.</param>
    private void SetRejectionMemoLinkingDetails(RejectionMemo record, string invoiceId)
    {
      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) && (record.RejectionStage > 1 || record.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber || record.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber))
      {
        var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();

          var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
         //SCP85039:Performance improvement of is-web during RMCreate and return
         //var rejectedInvoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
          var rejectedInvoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(rejectedInvoiceId);
          var criteria = new RMLinkingCriteria {
                                               BilledMemberId = rejectedInvoice.BilledMemberId,
                                               BillingMemberId = rejectedInvoice.BillingMemberId,
                                               BillingMonth = rejectedInvoice.BillingMonth,
                                               BillingYear = rejectedInvoice.BillingYear,
                                               BillingPeriod = rejectedInvoice.BillingPeriod,
                                               InvoiceNumber = rejectedInvoice.InvoiceNumber,
                                               ReasonCode = record.ReasonCode.ToUpper(),
                                               RejectedInvoiceId = invoiceId.ToGuid(),
                                               RejectionMemoNumber = record.YourRejectionNumber,
                                               RejectionStage = record.RejectionStage,
                                               FimBmCmIndicatorId = record.FIMBMCMIndicatorId,
                                               FimBMCMNumber = record.FimBMCMNumber,
                                               FimCouponNumber = record.FIMBMCMIndicatorId == 2 ? record.FimCouponNumber : null
                                             };

        var rejectionMemoResult = _nonSamplingInvoiceManager.GetRejectionMemoLinkingDetails(criteria);

        record.IsLinkingSuccessful = rejectionMemoResult.IsLinkingSuccessful;
        record.IsBreakdownAllowed = rejectionMemoResult.HasBreakdown;
        record.CurrencyConversionFactor = rejectionMemoResult.CurrencyConversionFactor;
      }
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMEdit")]
    public ActionResult RMEditAndAddNew(string invoiceId, string transactionId, RejectionMemo rejectionMemoRecord)
    {
      try
      {
        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (!IsYourBillingDateValid(rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingPeriod))
        {
          throw new ISBusinessException("Invalid Your Invoice Billing Date.","");
        }
        #endregion

        SetRejectionMemoLinkingDetails(rejectionMemoRecord, invoiceId);
        string errMessage = ValidateRMFimDetails(rejectionMemoRecord);
        //SCPID : 105938 - RE: IATA- SIS Bug
        string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(rejectionMemoRecord);
        if (!string.IsNullOrEmpty(errMessage))
        {
          ShowErrorMessage(errMessage, true);
          ViewData[ViewDataConstants.IsPostback] = true;
        }
        else if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
        {
            ShowCustomErrorMessage(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage, true);
            ViewData[ViewDataConstants.IsPostback] = true;
        }
        else
        {
          //CMP614: Source Code Validation for PAX RMs
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
           * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(rejectionMemoRecord, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);

          rejectionMemoRecord.Id = transactionId.ToGuid();
          rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
          foreach (var vat in rejectionMemoRecord.RejectionMemoVat)
          {
            vat.ParentId = rejectionMemoRecord.Id;
          }

          string warningMessage;
          rejectionMemoRecord = _nonSamplingInvoiceManager.UpdateRejectionMemoRecord(rejectionMemoRecord, out warningMessage);

          //ShowSuccessMessage(string.Format("{0} {1}", Messages.RMUpdateSuccessful, warningMessage));
          ShowSuccessMessage(string.Format("{0}", Messages.RMUpdateSuccessful));
          if (!String.IsNullOrEmpty(warningMessage))
            ShowErrorMessage(warningMessage, true);

          TempData.Remove(TempDataConstants.RejectedRecordIds);

          return RedirectToAction("RMCreate", "Invoice", new { invoiceId, transactionId });
        }
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
      }

      rejectionMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      var rejectionMemoCouponBreakdownGrid = new RejectionMemoCouponBreakdownGrid(ViewDataConstants.RMCouponListGrid,
                                                                                  Url.Action("RejectionMemoCouponBreakdownGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rejectionMemoCouponBreakdownGrid.Instance;

      SetViewDataPageMode(PageMode.Edit);

      return View("RMEdit", rejectionMemoRecord);
    }

    private void GetRejectCouponListFromBillingHistory(RejectionMemo rejectionMemo)
    {
      if (TempData == null || !TempData.ContainsKey(TempDataConstants.RejectedRecordIds) || rejectionMemo.RejectionStage > 1)
      {
        return;
      }

      var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();

      if (rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf('@') + 1)
      {
        rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
      }
      rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf('@'));
      if (rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf(';') + 1)
      {
        var transactionType = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf(';') + 1);
        rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf(';'));
        var rejectionIdList = rejectedRecordIds.Split(',');
        rejectionMemo.IsLinkingSuccessful = true;
        rejectionMemo.IsFromBillingHistory = true;

        if (transactionType == "PC")
        {
          var couponList = _nonSamplingInvoiceManager.GetPrimeBillingCouponList(rejectionIdList);
          if (couponList == null)
          {
            return;
          }

          CopyPrimeCouponListToRM(couponList, rejectionMemo);
        }
        return;
      }
    }

    private void CopyCMCouponListToRM(IList<CMCoupon> cmCouponList, RejectionMemo rejectionMemo)
    {

      IList<RMCoupon> newRmCouponList = new List<RMCoupon>();
      foreach (var cmCoupon in cmCouponList)
      {
        var newrmCoupon = new RMCoupon();


        newrmCoupon.AllowedHandlingFee = newrmCoupon.HandlingDifference = cmCoupon.HandlingFeeAmount;
        newrmCoupon.AllowedIscAmount = newrmCoupon.IscDifference = cmCoupon.IscAmountBilled;
        newrmCoupon.AllowedIscPercentage = cmCoupon.IscPercent;
        newrmCoupon.AllowedOtherCommission = newrmCoupon.OtherCommissionDifference = cmCoupon.OtherCommissionBilled;
        newrmCoupon.AllowedOtherCommissionPercentage = cmCoupon.OtherCommissionPercent;
        newrmCoupon.AllowedUatpAmount = newrmCoupon.UatpDifference = cmCoupon.UatpAmountBilled;
        newrmCoupon.AllowedUatpPercentage = cmCoupon.UatpPercent;
        newrmCoupon.GrossAmountBilled = Convert.ToDouble(cmCoupon.GrossAmountCredited);
        newrmCoupon.TaxAmountBilled = newrmCoupon.TaxAmountDifference = cmCoupon.TaxAmount;
        newrmCoupon.VatAmountBilled = newrmCoupon.VatAmountDifference = cmCoupon.TaxAmount;

        newrmCoupon.AgreementIndicatorSupplied = cmCoupon.AgreementIndicatorSupplied;
        newrmCoupon.AgreementIndicatorValidated = cmCoupon.AgreementIndicatorValidated;
        newrmCoupon.AirlineOwnUse = cmCoupon.AirlineOwnUse;
        newrmCoupon.CheckDigit = cmCoupon.CheckDigit;
        newrmCoupon.FromAirportOfCoupon = cmCoupon.FromAirportOfCoupon;
        newrmCoupon.ISValidationFlag = cmCoupon.ISValidationFlag;
        newrmCoupon.NfpReasonCode = cmCoupon.NfpReasonCode;
        newrmCoupon.ProrateSlipDetails = cmCoupon.ProrateSlipDetails;
        newrmCoupon.ReasonCode = cmCoupon.ReasonCode;
        newrmCoupon.ReferenceField1 = cmCoupon.ReferenceField1;
        newrmCoupon.ReferenceField2 = cmCoupon.ReferenceField2;
        newrmCoupon.ReferenceField3 = cmCoupon.ReferenceField3;
        newrmCoupon.ReferenceField4 = cmCoupon.ReferenceField4;
        newrmCoupon.ReferenceField5 = cmCoupon.ReferenceField5;
        newrmCoupon.SerialNo = cmCoupon.SerialNo;
        newrmCoupon.SettlementAuthorizationCode = cmCoupon.SettlementAuthorizationCode;
        newrmCoupon.TicketDocOrFimNumber = cmCoupon.TicketDocOrFimNumber;
        newrmCoupon.TicketOrFimCouponNumber = cmCoupon.TicketOrFimCouponNumber;
        newrmCoupon.TicketOrFimIssuingAirline = cmCoupon.TicketOrFimIssuingAirline;
        newrmCoupon.ToAirportOfCoupon = cmCoupon.ToAirportOfCoupon;
        newrmCoupon.ValidatedPmi = cmCoupon.ValidatedPmi;

        CopyCMCouponTaxBreakdown(newrmCoupon, cmCoupon.TaxBreakdown);
        CopyCMCouponVatBreakdown(newrmCoupon, cmCoupon.VatBreakdown);

        newRmCouponList.Add(newrmCoupon);
      }

      rejectionMemo.CouponBreakdownRecord.AddRange(newRmCouponList);
    }

    private static void CopyCMCouponVatBreakdown(RMCoupon newrmCoupon, List<CMCouponVat> list)
    {
      var newrmCouponVatList = new List<RMCouponVat>();
      foreach (var rmCouponVat in list)
      {
        var newrmCouponVat = new RMCouponVat();
        newrmCouponVat.VatBaseAmount = rmCouponVat.VatBaseAmount;
        newrmCouponVat.VatCalculatedAmount = rmCouponVat.VatCalculatedAmount;
        newrmCouponVat.VatLabel = rmCouponVat.VatLabel;
        newrmCouponVat.VatPercentage = rmCouponVat.VatPercentage;
        newrmCouponVat.VatText = rmCouponVat.VatText;

        newrmCouponVatList.Add(newrmCouponVat);
      }
      newrmCoupon.VatBreakdown.AddRange(newrmCouponVatList);
    }

    private static void CopyCMCouponTaxBreakdown(RMCoupon newrmCoupon, List<CMCouponTax> list)
    {
      var newrmCouponTaxList = new List<RMCouponTax>();
      foreach (var rmCouponTax in list)
      {
        var newrmCouponTax = new RMCouponTax { Amount = rmCouponTax.Amount, AmountAccepted = 0.00D, AmountDifference = rmCouponTax.Amount, TaxCode = rmCouponTax.TaxCode, TaxCodeId = rmCouponTax.TaxCodeId };

        newrmCouponTaxList.Add(newrmCouponTax);
      }

      newrmCoupon.TaxBreakdown.AddRange(newrmCouponTaxList);
    }

    private static void CopyBMCouponListToRM(IList<BMCoupon> couponList, RejectionMemo rejectionMemo)
    {
      IList<RMCoupon> newRmCouponList = new List<RMCoupon>();
      foreach (var bmCoupon in couponList)
      {
        var newrmCoupon = new RMCoupon();

        newrmCoupon.AllowedHandlingFee = newrmCoupon.HandlingDifference = bmCoupon.HandlingFeeAmount;
        newrmCoupon.AllowedIscAmount = newrmCoupon.IscDifference = bmCoupon.IscAmountBilled;
        newrmCoupon.AllowedIscPercentage = bmCoupon.IscPercent;
        newrmCoupon.AllowedOtherCommission = newrmCoupon.OtherCommissionDifference = bmCoupon.OtherCommissionBilled;
        newrmCoupon.AllowedOtherCommissionPercentage = bmCoupon.OtherCommissionPercent;
        newrmCoupon.AllowedUatpAmount = newrmCoupon.UatpDifference = bmCoupon.UatpAmountBilled;
        newrmCoupon.AllowedUatpPercentage = bmCoupon.UatpPercent;

        newrmCoupon.TaxAmountBilled = newrmCoupon.TaxAmountDifference = bmCoupon.TaxAmount;
        newrmCoupon.VatAmountBilled = newrmCoupon.VatAmountDifference = bmCoupon.VatAmount;

        newrmCoupon.AgreementIndicatorSupplied = bmCoupon.AgreementIndicatorSupplied;
        newrmCoupon.AgreementIndicatorValidated = bmCoupon.AgreementIndicatorValidated;
        newrmCoupon.AirlineOwnUse = bmCoupon.AirlineOwnUse;
        newrmCoupon.CheckDigit = bmCoupon.CheckDigit;
        newrmCoupon.FromAirportOfCoupon = bmCoupon.FromAirportOfCoupon;
        newrmCoupon.ISValidationFlag = bmCoupon.ISValidationFlag;
        newrmCoupon.NfpReasonCode = bmCoupon.NfpReasonCode;
        newrmCoupon.ProrateSlipDetails = bmCoupon.ProrateSlipDetails;
        newrmCoupon.ReasonCode = bmCoupon.ReasonCode;
        newrmCoupon.ReferenceField1 = bmCoupon.ReferenceField1;
        newrmCoupon.ReferenceField2 = bmCoupon.ReferenceField2;
        newrmCoupon.ReferenceField3 = bmCoupon.ReferenceField3;
        newrmCoupon.ReferenceField4 = bmCoupon.ReferenceField4;
        newrmCoupon.ReferenceField5 = bmCoupon.ReferenceField5;
        newrmCoupon.SerialNo = bmCoupon.SerialNo;
        newrmCoupon.SettlementAuthorizationCode = bmCoupon.SettlementAuthorizationCode;
        newrmCoupon.TicketDocOrFimNumber = bmCoupon.TicketDocOrFimNumber;
        newrmCoupon.TicketOrFimCouponNumber = bmCoupon.TicketOrFimCouponNumber;
        newrmCoupon.TicketOrFimIssuingAirline = bmCoupon.TicketOrFimIssuingAirline;
        newrmCoupon.ToAirportOfCoupon = bmCoupon.ToAirportOfCoupon;
        newrmCoupon.ValidatedPmi = bmCoupon.ValidatedPmi;

        CopyBMCouponTaxBreakdown(newrmCoupon, bmCoupon.TaxBreakdown);
        CopyBMCouponVatBreakdown(newrmCoupon, bmCoupon.VatBreakdown);

        newRmCouponList.Add(newrmCoupon);
      }

      rejectionMemo.CouponBreakdownRecord.AddRange(newRmCouponList);
    }

    private static void CopyBMCouponVatBreakdown(RMCoupon newrmCoupon, List<BMCouponVat> list)
    {
      var newrmCouponVatList = new List<RMCouponVat>();
      foreach (var rmCouponVat in list)
      {
        var newrmCouponVat = new RMCouponVat();
        newrmCouponVat.VatBaseAmount = rmCouponVat.VatBaseAmount;
        newrmCouponVat.VatCalculatedAmount = rmCouponVat.VatCalculatedAmount;
        newrmCouponVat.VatLabel = rmCouponVat.VatLabel;
        newrmCouponVat.VatPercentage = rmCouponVat.VatPercentage;
        newrmCouponVat.VatText = rmCouponVat.VatText;

        newrmCouponVatList.Add(newrmCouponVat);
      }
      newrmCoupon.VatBreakdown.AddRange(newrmCouponVatList);
    }

    private static void CopyBMCouponTaxBreakdown(RMCoupon newrmCoupon, List<BMCouponTax> list)
    {
      var newrmCouponTaxList = new List<RMCouponTax>();
      foreach (var rmCouponTax in list)
      {
        var newrmCouponTax = new RMCouponTax { Amount = rmCouponTax.Amount, AmountAccepted = 0.00D, AmountDifference = rmCouponTax.Amount, TaxCode = rmCouponTax.TaxCode, TaxCodeId = rmCouponTax.TaxCodeId };

        newrmCouponTaxList.Add(newrmCouponTax);
      }

      newrmCoupon.TaxBreakdown.AddRange(newrmCouponTaxList);
    }

    private void CopyRMCouponListToRM(IList<RMCoupon> rmCouponList, RejectionMemo rejectionMemo)
    {
      IList<RMCoupon> newRmCouponList = new List<RMCoupon>();
      foreach (var rmCoupon in rmCouponList)
      {
        var newrmCoupon = new RMCoupon();
        newrmCoupon.AllowedHandlingFee = newrmCoupon.HandlingDifference = rmCoupon.AcceptedHandlingFee;
        newrmCoupon.AllowedIscAmount = newrmCoupon.IscDifference = rmCoupon.AcceptedIscAmount;
        newrmCoupon.AllowedIscPercentage = rmCoupon.AcceptedIscPercentage;
        newrmCoupon.AllowedOtherCommission = newrmCoupon.OtherCommissionDifference = rmCoupon.AcceptedOtherCommission;
        newrmCoupon.AllowedOtherCommissionPercentage = rmCoupon.AcceptedOtherCommissionPercentage;
        newrmCoupon.AllowedUatpAmount = newrmCoupon.UatpDifference = rmCoupon.AcceptedUatpAmount;
        newrmCoupon.AllowedUatpPercentage = rmCoupon.AcceptedUatpPercentage;
        newrmCoupon.GrossAmountBilled = rmCoupon.GrossAmountAccepted;
        newrmCoupon.TaxAmountBilled = newrmCoupon.TaxAmountDifference = rmCoupon.TaxAmountAccepted;
        newrmCoupon.VatAmountBilled = newrmCoupon.VatAmountDifference = rmCoupon.VatAmountAccepted;

        newrmCoupon.AgreementIndicatorSupplied = rmCoupon.AgreementIndicatorSupplied;
        newrmCoupon.AgreementIndicatorValidated = rmCoupon.AgreementIndicatorValidated;
        newrmCoupon.AirlineOwnUse = rmCoupon.AirlineOwnUse;
        newrmCoupon.CheckDigit = rmCoupon.CheckDigit;
        newrmCoupon.FromAirportOfCoupon = rmCoupon.FromAirportOfCoupon;
        newrmCoupon.ISValidationFlag = rmCoupon.ISValidationFlag;
        newrmCoupon.NfpReasonCode = rmCoupon.NfpReasonCode;
        newrmCoupon.ProrateSlipDetails = rmCoupon.ProrateSlipDetails;
        newrmCoupon.ReasonCode = rmCoupon.ReasonCode;
        newrmCoupon.ReferenceField1 = rmCoupon.ReferenceField1;
        newrmCoupon.ReferenceField2 = rmCoupon.ReferenceField2;
        newrmCoupon.ReferenceField3 = rmCoupon.ReferenceField3;
        newrmCoupon.ReferenceField4 = rmCoupon.ReferenceField4;
        newrmCoupon.ReferenceField5 = rmCoupon.ReferenceField5;
        newrmCoupon.SerialNo = rmCoupon.SerialNo;
        newrmCoupon.SettlementAuthorizationCode = rmCoupon.SettlementAuthorizationCode;
        newrmCoupon.TicketDocOrFimNumber = rmCoupon.TicketDocOrFimNumber;
        newrmCoupon.TicketOrFimCouponNumber = rmCoupon.TicketOrFimCouponNumber;
        newrmCoupon.TicketOrFimIssuingAirline = rmCoupon.TicketOrFimIssuingAirline;
        newrmCoupon.ToAirportOfCoupon = rmCoupon.ToAirportOfCoupon;
        newrmCoupon.ValidatedPmi = rmCoupon.ValidatedPmi;

        CopyRMCouponTaxBreakdown(newrmCoupon, rmCoupon.TaxBreakdown);
        CopyRMCouponVatBreakdown(newrmCoupon, rmCoupon.VatBreakdown);

        newRmCouponList.Add(newrmCoupon);
      }

      rejectionMemo.CouponBreakdownRecord.AddRange(newRmCouponList);
    }

    private static void CopyRMCouponVatBreakdown(RMCoupon newrmCoupon, List<RMCouponVat> list)
    {
      var newrmCouponVatList = new List<RMCouponVat>();
      foreach (var rmCouponVat in list)
      {
        var newrmCouponVat = new RMCouponVat();
        newrmCouponVat.VatBaseAmount = rmCouponVat.VatBaseAmount;
        newrmCouponVat.VatCalculatedAmount = rmCouponVat.VatCalculatedAmount;
        newrmCouponVat.VatLabel = rmCouponVat.VatLabel;
        newrmCouponVat.VatPercentage = rmCouponVat.VatPercentage;
        newrmCouponVat.VatText = rmCouponVat.VatText;

        newrmCouponVatList.Add(newrmCouponVat);
      }
      newrmCoupon.VatBreakdown.AddRange(newrmCouponVatList);
    }

    private static void CopyRMCouponTaxBreakdown(RMCoupon newrmCoupon, List<RMCouponTax> list)
    {
      var newrmCouponTaxList = new List<RMCouponTax>();
      foreach (var rmCouponTax in list)
      {
        var newrmCouponTax = new RMCouponTax
                               {
                                 Amount = rmCouponTax.AmountAccepted,
                                 AmountAccepted = 0.00D,
                                 AmountDifference = rmCouponTax.AmountAccepted,
                                 TaxCode = rmCouponTax.TaxCode,
                                 TaxCodeId = rmCouponTax.TaxCodeId
                               };

        newrmCouponTaxList.Add(newrmCouponTax);
      }

      newrmCoupon.TaxBreakdown.AddRange(newrmCouponTaxList);
    }

    private void CopyPrimeCouponListToRM(IList<PrimeCoupon> couponList, RejectionMemo rejectionMemo)
    {
      var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(rejectionMemo.InvoiceId.ToString());

      IList<RMCoupon> newRmCouponList = new List<RMCoupon>();

      foreach (var primeCoupon in couponList)
      {
        if (primeCoupon.SourceCodeId == 14) // for FIM documents, do not copy the coupons.
          continue;

        var coupondetails = _nonSamplingInvoiceManager.GetRMCouponBreakdownSingleRecordDetails(primeCoupon.Id, rejectionMemo.Id, invoice.BillingMemberId, invoice.BilledMemberId);
        string message;
        coupondetails.Details.RejectionMemoId = rejectionMemo.Id;
        _nonSamplingInvoiceManager.AddRejectionMemoCouponDetails(coupondetails.Details, rejectionMemo.InvoiceId.ToString(), out message, isFromBillingHistory : true);
      }

      rejectionMemo.CouponBreakdownRecord.AddRange(newRmCouponList);
    }

    private void CopyPrimeCouponVatBreakdown(RMCoupon newrmCoupon, List<PrimeCouponVat> list)
    {
      var newrmCouponVatList = new List<RMCouponVat>();
      foreach (var couponVat in list)
      {
        var newrmCouponVat = new RMCouponVat();

        newrmCouponVat.VatIdentifierId = couponVat.VatIdentifierId;
        newrmCouponVat.VatBaseAmount = couponVat.VatBaseAmount;
        newrmCouponVat.VatCalculatedAmount = couponVat.VatCalculatedAmount;
        newrmCouponVat.VatLabel = couponVat.VatLabel;
        newrmCouponVat.VatPercentage = couponVat.VatPercentage;
        newrmCouponVat.VatText = couponVat.VatText;

        newrmCouponVatList.Add(newrmCouponVat);
      }
      newrmCoupon.VatBreakdown.AddRange(newrmCouponVatList);
    }

    private void CopyPrimeCouponTaxBreakdown(RMCoupon newrmCoupon, List<PrimeCouponTax> list)
    {
      var newrmCouponTaxList = new List<RMCouponTax>();
      foreach (var primeCouponTax in list)
      {
        var newrmCouponTax = new RMCouponTax { Amount = primeCouponTax.Amount, AmountAccepted = primeCouponTax.Amount, TaxCode = primeCouponTax.TaxCode, TaxCodeId = primeCouponTax.TaxCodeId };

        newrmCouponTaxList.Add(newrmCouponTax);
      }

      newrmCoupon.TaxBreakdown.AddRange(newrmCouponTaxList);
    }

    [HttpGet]
    [RestrictUnauthorizedUpdate]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult RMEdit(string invoiceId, string transactionId)
    {
      var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

      if (_nonSamplingInvoiceManager.GetRejectionMemoCouponBreakdownCount(transactionId) != 0)
      {
        ViewData[ViewDataConstants.BreakdownExists] = true;
      }
      else
      {
        ViewData[ViewDataConstants.BreakdownExists] = false;
      }

      var rejectionMemoCouponBreakdownGrid = new RejectionMemoCouponBreakdownGrid(ViewDataConstants.RMCouponListGrid,
                                                                                  Url.Action("RejectionMemoCouponBreakdownGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rejectionMemoCouponBreakdownGrid.Instance;

      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        ViewData[ViewDataConstants.FromBillingHistory] = true;
        // Store in Tempdata again.
        var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds];
        TempData[TempDataConstants.RejectedRecordIds] = rejectedRecordIds;
      }

      return View(rejectionMemoRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMEdit(string invoiceId, string transactionId, RejectionMemo rejectionMemoRecord)
    {
      try
      {
        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (!IsYourBillingDateValid(rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingPeriod))
        {
          throw new ISBusinessException("Invalid Your Invoice Billing Date.","");
        }
        #endregion

        SetRejectionMemoLinkingDetails(rejectionMemoRecord, invoiceId);
        string errMessage = ValidateRMFimDetails(rejectionMemoRecord);
        //SCPID : 105938 - RE: IATA- SIS Bug
        string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(rejectionMemoRecord);
        if (!string.IsNullOrEmpty(errMessage))
        {
            ShowErrorMessage(errMessage, true);
            ViewData[ViewDataConstants.IsPostback] = true;
        }
        else if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
        {   
            ShowCustomErrorMessage(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage, true);
            ViewData[ViewDataConstants.IsPostback] = true;
        }
        else 
        {
          //CMP614: Source Code Validation for PAX RMs
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
          * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(rejectionMemoRecord, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);

          rejectionMemoRecord.Id = transactionId.ToGuid();
          rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
          foreach (var vat in rejectionMemoRecord.RejectionMemoVat)
          {
            vat.ParentId = rejectionMemoRecord.Id;
          }

          string warningMessage;
          rejectionMemoRecord = _nonSamplingInvoiceManager.UpdateRejectionMemoRecord(rejectionMemoRecord, out warningMessage);

         // ShowSuccessMessage(string.Format("{0} {1}", Messages.RMUpdateSuccessful, warningMessage));
          ShowSuccessMessage(string.Format("{0}", Messages.RMUpdateSuccessful));
          if (!String.IsNullOrEmpty(warningMessage))
            ShowErrorMessage(warningMessage, true);
        
          if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
          {
            ViewData[ViewDataConstants.FromBillingHistory] = true;
          }

          return RedirectToAction("RMEdit", "Invoice", new { invoiceId, transactionId });
        }
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
      }

      rejectionMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      var rejectionMemoCouponBreakdownGrid = new RejectionMemoCouponBreakdownGrid(ViewDataConstants.RMCouponListGrid,
                                                                                  Url.Action("RejectionMemoCouponBreakdownGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rejectionMemoCouponBreakdownGrid.Instance;

      SetViewDataPageMode(PageMode.Edit);

      return View(rejectionMemoRecord);
    }

    /// <summary>
    /// Gets the rejection memo record.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    private RejectionMemo GetRejectionMemoRecord(string transactionId, string invoiceId)
    {
      var rejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      var transactionType = 0;

      // Depending on RejectionStage retrieve transaction type
      switch (rejectionMemoRecord.RejectionStage)
      {
        case 1:
          transactionType = (int)TransactionType.RejectionMemo1;
          break;
        case 2:
          transactionType = (int)TransactionType.RejectionMemo2;
          break;
        case 3:
          transactionType = (int)TransactionType.RejectionMemo3;
          break;
      }

      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      bool isCouponBreakdownMandatory = _referenceManager.GetReasonCode(rejectionMemoRecord.ReasonCode, transactionType).CouponAwbBreakdownMandatory;
      rejectionMemoRecord.Invoice = InvoiceHeader;

      // Set Coupon breakdown value
      rejectionMemoRecord.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

      return rejectionMemoRecord;
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    [HttpGet]
    public ActionResult RMView(string invoiceId, string transactionId)
    {
      var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

      if (_nonSamplingInvoiceManager.GetRejectionMemoCouponBreakdownCount(transactionId) != 0)
      {
        ViewData[ViewDataConstants.BreakdownExists] = true;
      }
      else
      {
        ViewData[ViewDataConstants.BreakdownExists] = false;
      }

      var rejectionMemoCouponBreakdownGrid = new RejectionMemoCouponBreakdownGrid(ViewDataConstants.RMCouponListGrid,
                                                                                  Url.Action("RejectionMemoCouponBreakdownGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rejectionMemoCouponBreakdownGrid.Instance;

      // This is done so as to not show the Reject icon when user navigates to RM listing page from correspondence.
      if (Request.QueryString["fc"] != null && Convert.ToBoolean(Request.QueryString["fc"]))
      {
        TempData[TempDataConstants.FromCorrespondence] = true;
      }

      return View("RMEdit", rejectionMemoRecord);
    }

    /// <summary>
    /// Delete Rejection memo
    /// </summary>
    /// <param name="transactionId"></param>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.PAX_REJECTION_MEMO)]
    public JsonResult RMDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        var isDeleted = _nonSamplingInvoiceManager.DeleteRejectionMemoRecord(transactionId);

        details = GetDeleteMessage(isDeleted);

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);

        return Json(details);
      }
    }

    [HttpPost]
    public virtual JsonResult GetRMLinkingDetails(FormCollection form)
    {
      try
      {
        var rmLinking = new JavaScriptSerializer().Deserialize(form[0], typeof(RMLinkingCriteria));
        var criteria = rmLinking as RMLinkingCriteria;

        if (criteria != null)
          criteria.IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod;
        
        var rejectionMemoResult = _nonSamplingInvoiceManager.GetRejectionMemoLinkingDetails(criteria);

          return Json(rejectionMemoResult);
      }
      catch (ISBusinessException exception)
      {
        Logger.Error("Error while getting RM linking details.", exception);
      }

      return Json(new object());
    }

    [HttpPost]
    public virtual JsonResult GetLinkedMemoDetailsForRM(FormCollection form)
    {
      try
      {
        var rmLinking = new JavaScriptSerializer().Deserialize(form[0], typeof(RMLinkingCriteria));
        var criteria = rmLinking as RMLinkingCriteria;

        var rejectionMemoResult = _nonSamplingInvoiceManager.GetLinkedMemoAmountDetails(criteria);

          return Json(rejectionMemoResult);
      }
      catch (ISBusinessException exception)
      {
        Logger.Error("Error while getting linked memo details for RM.", exception);
      }

      return Json(new object());
    }

    /// <summary>
    /// Upload Rejection Memo Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult RejectionMemoAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<RejectionMemoAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method RejectionMemoAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Get invoice header details
        var invoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);
        Logger.Info("Fetched all invoice details successfully.");
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          Logger.Info("Started saving the file" + fileToSave);
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          // On Rejection Memo Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _nonSamplingInvoiceManager.IsDuplicateRejectionMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }
          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          Logger.Info("Attachment successfully validated");
          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new RejectionMemoAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };

            attachment = _nonSamplingInvoiceManager.AddRejectionMemoAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
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
        Logger.Error("Exception", ex);
      }
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
        Logger.Error("Exception", ex);
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Rejection Memo attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="transactionId">Transaction id</param>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download)]
    [HttpGet]
    public FileStreamResult RejectionMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetRejectionMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// This action will display create page of billing memo coupon breakdown.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    public ActionResult BMCouponCreate(string invoiceId, string transactionId)
    {
      SetViewDataPageMode(PageMode.Create);

      var billingMemoCoupon = new BMCoupon { LastUpdatedBy = SessionUtil.UserId };
      var billingMemoRecord = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      billingMemoRecord.Invoice = InvoiceHeader;

      // Set Airline flight designator to Billing Member name
      billingMemoCoupon.AirlineFlightDesignator = InvoiceHeader.BillingMember.MemberCodeAlpha;
      billingMemoCoupon.BillingMemo = billingMemoRecord;

      ViewData["IsAddNewBMCoupon"] = TempData["IsAddNewBMCoupon"] != null && Convert.ToBoolean(TempData["IsAddNewBMCoupon"]);

      return View(billingMemoCoupon);
    }

    /// <summary>
    /// Gets the coupon of corresponding billingMemoId and its coupon record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMCouponCreate(string invoiceId, string transactionId, BMCoupon record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
        //TaxBreakDown Validation 
        if (inValidTaxBreakDown(record.TaxAmount, bmTaxBreakdowns: record.TaxBreakdown))
        {
             throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
        }
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 3 */
        MemberManager.ValidateIssuingAirline(record.TicketOrFimIssuingAirline);
        record.BillingMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.Attachments.Clear();
        string duplicateCouponErrorMessage;
        _nonSamplingInvoiceManager.AddBillingMemoCouponDetails(record, invoiceId, out duplicateCouponErrorMessage);
        _nonSamplingInvoiceManager.UpdateBillingMemoCouponAttachment(couponAttachmentIds, record.Id);

//        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
        ShowSuccessMessage(Messages.BMCouponCreateSuccessfully);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage,true);

        TempData["IsAddNewBMCoupon"] = true;

        return RedirectToAction("BMCouponCreate", new { transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);

        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM 
        record.BillingMemoId = transactionId.ToGuid();
      }
      var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      var billingMemoRecord = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      billingMemoRecord.Invoice = invoice;
      record.BillingMemo = billingMemoRecord;

      return View(record);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMCouponCreate")]
    public ActionResult BMCouponDuplicate(string invoiceId, string transactionId, BMCoupon record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, bmTaxBreakdowns: record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 3 */
        MemberManager.ValidateIssuingAirline(record.TicketOrFimIssuingAirline);
        record.BillingMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.Attachments.Clear();
        string duplicateErrorMEssage;
        _nonSamplingInvoiceManager.AddBillingMemoCouponDetails(record, invoiceId, out duplicateErrorMEssage);
        _nonSamplingInvoiceManager.UpdateBillingMemoCouponAttachment(couponAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.BMCouponCreateSuccessfully, false);
        if (!String.IsNullOrEmpty(duplicateErrorMEssage))
          ShowErrorMessage(duplicateErrorMEssage);

        SetViewDataPageMode(PageMode.Clone);
        record.Attachments.Clear(); // Clear again. Attachments should not be duplicated. 

        ViewData["IsAddNewBMCoupon"] = true;
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM 
        record.BillingMemoId = transactionId.ToGuid();
      }


      var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      var billingMemoRecord = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);

      billingMemoRecord.Invoice = invoice;
      record.BillingMemo = billingMemoRecord;

      return View("BMCouponCreate", record);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMCouponCreate")]
    public ActionResult BMCouponCreateAndReturn(string invoiceId, string transactionId, BMCoupon record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        //TaxBreakDown Validation 
         if (inValidTaxBreakDown(record.TaxAmount, bmTaxBreakdowns: record.TaxBreakdown))
         {
            throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
         }
         //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
         /* CMP #596: Length of Member Accounting Code to be Increased to 12 
         Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
         Ref: FRS Section 3.4 Table 18 Row 3 */
        MemberManager.ValidateIssuingAirline(record.TicketOrFimIssuingAirline);
        record.BillingMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        record.Attachments.Clear();

        string duplicateErrorMEssage;
        _nonSamplingInvoiceManager.AddBillingMemoCouponDetails(record, invoiceId, out duplicateErrorMEssage);
        _nonSamplingInvoiceManager.UpdateBillingMemoCouponAttachment(couponAttachmentIds, record.Id);

        ShowSuccessMessage(Messages.BMCouponCreateSuccessfully);
        if (!String.IsNullOrEmpty(duplicateErrorMEssage))
          ShowErrorMessage(duplicateErrorMEssage,true);

        return RedirectToAction("BMEdit", new { transaction = "BMEdit", invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM 
        record.BillingMemoId = transactionId.ToGuid();
      }

      var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      var billingMemoRecord = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);

      billingMemoRecord.Invoice = invoice;
      record.BillingMemo = billingMemoRecord;

      ViewData["IsAddNewBMCoupon"] = false;

      return View("BMCouponCreate", record);
    }

    /// <summary>
    /// Edit Billing Memo
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    public ActionResult BMCouponEdit(string invoiceId, string transactionId, string couponId)
    {
      var breakdownrecord = GetBreakdownrecord(couponId, transactionId, invoiceId);
      // Set Airline flight designator to Billing Member name
      breakdownrecord.AirlineFlightDesignator = breakdownrecord.BillingMemo.Invoice.BillingMember.MemberCodeAlpha;

      return View(breakdownrecord);
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    public ActionResult BMCouponView(string invoiceId, string transactionId, string couponId)
    {
      var breakdownrecord = GetBreakdownrecord(couponId, transactionId, invoiceId);

      return View("BMCouponEdit", breakdownrecord);
    }

    private BMCoupon GetBreakdownrecord(string couponId, string transactionId, string invoiceId)
    {
      var breakdownrecord = _nonSamplingInvoiceManager.GetBillingMemoCouponDetails(couponId);
      breakdownrecord.LastUpdatedBy = SessionUtil.UserId;
      breakdownrecord.BillingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      breakdownrecord.BillingMemo.Invoice = InvoiceHeader;
      // added code to remove the extra special char
      if (!string.IsNullOrEmpty(breakdownrecord.ProrateSlipDetails))
        breakdownrecord.ProrateSlipDetails =
          breakdownrecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);

      return breakdownrecord;
    }

    /// <summary>
    /// Edit Billing Memo
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMCouponCreate")]
    public ActionResult BMCouponEdit(string invoiceId, string transactionId, string couponId, BMCoupon record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, bmTaxBreakdowns: record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 3 */
        MemberManager.ValidateIssuingAirline(record.TicketOrFimIssuingAirline);
        record.Id = couponId.ToGuid();
        record.BillingMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent id to Tax records
        foreach (var tax in record.TaxBreakdown)
        {
          tax.ParentId = record.Id;
        }

        // Assign parent id for VAt records
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }
        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateBillingMemoCouponDetails(record, invoiceId, out duplicateErrorMessage);

        ShowSuccessMessage(Messages.BMCouponUpdateSuccessfully);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);

        return RedirectToAction("BMCouponCreate", "Invoice", new { invoiceId, transactionId, transaction = "BMEdit" });
      }
      catch (ISBusinessException businessException)
      {
        SetViewDataPageMode(PageMode.Edit);
        ShowErrorMessage(businessException.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM 
        record.Id = couponId.ToGuid();
        record.BillingMemoId = transactionId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      record.BillingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      record.BillingMemo.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(record);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMCouponCreate")]
    public ActionResult BMCouponClone(string invoiceId, string transactionId, string couponId, BMCoupon record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, bmTaxBreakdowns: record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 3 */
        MemberManager.ValidateIssuingAirline(record.TicketOrFimIssuingAirline);

        record.Id = couponId.ToGuid();
        record.BillingMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent id to Tax records
        foreach (var tax in record.TaxBreakdown)
        {
          tax.ParentId = record.Id;
        }

        // Assign parent id for VAt records
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }
        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateBillingMemoCouponDetails(record, invoiceId, out duplicateErrorMessage);

        ShowSuccessMessage(Messages.BMCouponUpdateSuccessfully, false);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        record.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.Id = couponId.ToGuid();
        record.BillingMemoId = transactionId.ToGuid();
        SetViewDataPageMode(PageMode.Edit);
      }


      record.BillingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      record.BillingMemo.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("BMCouponCreate", record);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMEdit")]
    public ActionResult BMCouponEditAndReturn(string invoiceId, string transactionId, string couponId, BMCoupon record)
    {
      var couponAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(record.TaxAmount, bmTaxBreakdowns: record.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.InvalidTotalTaxBreakdownAmounts, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 3 */
        MemberManager.ValidateIssuingAirline(record.TicketOrFimIssuingAirline);
        record.Id = couponId.ToGuid();
        record.BillingMemoId = transactionId.ToGuid();
        record.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent id to Tax records
        foreach (var tax in record.TaxBreakdown)
        {
          tax.ParentId = record.Id;
        }

        // Assign parent id for VAt records
        foreach (var vat in record.VatBreakdown)
        {
          vat.ParentId = record.Id;
        }

        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateBillingMemoCouponDetails(record, invoiceId, out duplicateErrorMessage);

        ShowSuccessMessage(Messages.BMCouponUpdateSuccessfully);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("BMEdit", new { invoiceId, transactionId, transaction = "BMEdit" });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        record.Attachments = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        record.Id = couponId.ToGuid();
        record.BillingMemoId = transactionId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      record.BillingMemo = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);
      record.BillingMemo.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("BMCouponEdit", record);
    }

    /// <summary>
    /// Get Coupon breakdown list for Billing memo
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [Obsolete]
    public ActionResult BMCouponList(string invoiceId, string transactionId)
    {
      var billingMemoRecord = _nonSamplingInvoiceManager.GetBillingMemoRecordDetails(transactionId);

      billingMemoRecord.Invoice = InvoiceHeader;

      var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;

      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
      {
        SetViewDataPageMode(PageMode.View);
      }

      return View(billingMemoRecord);
    }

    /// <summary>
    /// Upload Billing Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult BMCouponAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<BMCouponAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method BMCouponAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Get invoice header details
        var invoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);
        Logger.Info("Fetched all invoice details successfully.");
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          Logger.Info("Started saving the file" + fileToSave);
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          // On Billing Memo Coupon Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _nonSamplingInvoiceManager.IsDuplicateBillingMemoCouponAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          Logger.Info("Attachment successfully validated");
          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new BMCouponAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };

            attachment = _nonSamplingInvoiceManager.AddBillingMemoCouponAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            // assign user info from session and file server info.
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new User();
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
        Logger.Error("Exception", ex);
      }
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
        Logger.Error("Exception", ex);
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Billing Memo Coupon attachment
    ///  </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="couponId">Coupon id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download)]
    [HttpGet]
    public FileStreamResult BMCouponAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetBillingMemoCouponAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Fetch data for coupon list in Billing memo
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public JsonResult BillingMemoCouponGridData(string transactionId)
    {
      // Create grid instance and retrieve data from database
      var billingMemoCouponGrid = new BillingMemoCouponGrid(ControlIdConstants.CouponGridId, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));

      var billingMemoCoupons = _nonSamplingInvoiceManager.GetBillingMemoCouponList(transactionId).AsQueryable();
      return billingMemoCouponGrid.DataBind(billingMemoCoupons);
    }

    /// <summary>
    /// Delete billing memo coupon breakdown record
    /// </summary>
    /// <param name="couponId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "couponId", TableName = TransactionTypeTable.PAX_BM_COUPON_BREAKDOWN)]
    public JsonResult BMCouponDelete(string couponId)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        Guid invoiceId;
        Guid billingMemoId;
        bool isDeleted = _nonSamplingInvoiceManager.DeleteBillingMemoCouponRecord(couponId, out billingMemoId, out invoiceId);

        details = isDeleted
                    ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, RedirectUrl = Url.Action("BMEdit", new { invoiceId, transactionId = billingMemoId }), isRedirect = true }
                    : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }
      return Json(details);
    }

    /// <summary>
    /// RejectionMemo Coupon breakdown create
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [OutputCache(CacheProfile = "donotCache")]
    [HttpGet]
    public ActionResult RMCouponCreate(string invoiceId, string transactionId)
    {
      var breakdownrecord = new RMCoupon();
      var rejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoRecord.Invoice = InvoiceHeader;
      rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
      breakdownrecord.RejectionMemoRecord = rejectionMemoRecord;
      SetViewDataPageMode(PageMode.Create);

      // Set ViewData, "IsPostback" to false
      ViewData["IsPostback"] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData["RMCouponRecord"] != null)
      {
        // Set Viewdata
        ViewData["RMCouponRecord"] = TempData["RMCouponRecord"];
      }
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check coupon Breakdown record count.

      if (InvoiceHeader.BillingCode == (int)BillingCode.NonSampling && (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3 ) && rejectionMemoRecord.IsLinkingSuccessful == true)
      {
          if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
          {
              ViewData["IsAwbLinkingRequired"] = false;
          }
          else
          {
              PaxInvoice yourInvoice = _nonSamplingInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                                          rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                                          rejectionMemoRecord.YourInvoiceBillingYear,
                                                                                          rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                                          rejectionMemoRecord.Invoice.BilledMemberId,
                                                                                          rejectionMemoRecord.Invoice.BillingMemberId,
                                                                                          null,
                                                                                          null, rejectionMemoRecord.YourRejectionNumber);

              if (yourInvoice != null && (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3) && yourInvoice.RejectionMemoRecord.Count > 0)
              {
                  RejectionMemo yourRm = yourInvoice.RejectionMemoRecord.Where(c => c.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
                  if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0) ViewData["IsAwbLinkingRequired"] = false;
              }
          }
      }
      return View(breakdownrecord);
    }

    /// <summary>
    ///  RejectionMemo Coupon breakdown create
    /// </summary>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMCouponCreate(string invoiceId, string transactionId, RMCoupon rejectionMemoCouponBreakdownRecord)
    {
      // TODO: Get Billing Memo Coupon Breakdown data from the Database corresponding to the billingMemoId and to its corresponding coupon breakdown rec.
      var couponAttachmentIds = rejectionMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          rejectionMemoCouponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
          //SCPID : 105938 - RE: IATA- SIS Bug
          string errorMessage = ValidateRMCouponBreakdownFieldsCalculation(rejectionMemoCouponBreakdownRecord);
          if (!string.IsNullOrEmpty(errorMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, errorMessage);
          
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(rejectionMemoCouponBreakdownRecord.TaxAmountDifference, rejectionMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 2 */
          MemberManager.ValidateIssuingAirline(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);

          //CMP614: Source Code Validation for PAX RMs
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
           * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(rejectionMemoCouponBreakdownRecord, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);

        string duplicateErrorMessage;
       rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline =
              rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim();
        

        rejectionMemoCouponBreakdownRecord.Attachments.Clear();
        rejectionMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        int vatRecordCountBefore = rejectionMemoCouponBreakdownRecord.VatBreakdown.Count;
        var rejectionMemoCoupon = _nonSamplingInvoiceManager.AddRejectionMemoCouponDetails(rejectionMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);
        int vatRecordCountAfter = rejectionMemoCouponBreakdownRecord.VatBreakdown.Count;
        _nonSamplingInvoiceManager.UpdateRejectionMemoCouponAttachment(couponAttachmentIds, rejectionMemoCouponBreakdownRecord.Id);

        //ShowSuccessMessages(Messages.RMCouponCreateSuccessful + duplicateErrorMessage, vatRecordCountBefore, vatRecordCountAfter);
        ShowSuccessMessages(Messages.RMCouponCreateSuccessful , vatRecordCountBefore, vatRecordCountAfter);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);
        
        TempData["RMCouponRecord"] = string.Format(@"{0}-{1}", rejectionMemoCouponBreakdownRecord.SerialNo, rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);

        return RedirectToAction("RMCouponCreate", new { invoiceId, transactionId, transaction = "RMEdit", couponId = rejectionMemoCoupon.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        SetViewDataPageMode(PageMode.Clone); // Done to hide the serial number field.
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        rejectionMemoCouponBreakdownRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        rejectionMemoCouponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
      }

      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check coupon Breakdown record count.

      if (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BillingCode == (int)BillingCode.NonSampling && (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 2 || rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 3) && rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
        if (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
        {
          ViewData["IsAwbLinkingRequired"] = false;
        }
        else
        {
          PaxInvoice yourInvoice = _nonSamplingInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceNumber,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingMonth,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingYear,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BilledMemberId,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BillingMemberId,
                                                                                      null,
                                                                                      null, rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourRejectionNumber);

          if (yourInvoice != null && (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 2 || rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.RejectionMemoRecord.Count > 0)
          {
            RejectionMemo yourRm = yourInvoice.RejectionMemoRecord.Where(c => c.RejectionMemoNumber == rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
            if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0) ViewData["IsAwbLinkingRequired"] = false;
          }
        }
      }
      return View(rejectionMemoCouponBreakdownRecord);
    }

    /// <summary>
    /// Creates prime billing coupon and allows to duplicate same record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMCouponCreate")]
    public ActionResult RMCouponDuplicate(string invoiceId, string transactionId, RMCoupon rejectionMemoCouponBreakdownRecord)
    {
      //Get attachment Id list
      var couponAttachmentIds = rejectionMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          rejectionMemoCouponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
          //SCPID : 105938 - RE: IATA- SIS Bug
          string errorMessage = ValidateRMCouponBreakdownFieldsCalculation(rejectionMemoCouponBreakdownRecord);
          if (!string.IsNullOrEmpty(errorMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, errorMessage);

          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(rejectionMemoCouponBreakdownRecord.TaxAmountDifference, rejectionMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 2 */
          MemberManager.ValidateIssuingAirline(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);

          //CMP614: Source Code Validation for PAX RMs
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
           * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(rejectionMemoCouponBreakdownRecord, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);

        rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline =
         rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim();
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        string duplicateErrorMessage;
        

        rejectionMemoCouponBreakdownRecord.Attachments.Clear();
        rejectionMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        var rejectionMemoCoupon = _nonSamplingInvoiceManager.AddRejectionMemoCouponDetails(rejectionMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);
        _nonSamplingInvoiceManager.UpdateRejectionMemoCouponAttachment(couponAttachmentIds, rejectionMemoCouponBreakdownRecord.Id);

        //ShowSuccessMessage(Messages.RMCouponCreateSuccessful + duplicateErrorMessage);
        ShowSuccessMessage(Messages.RMCouponCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);

        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        rejectionMemoCouponBreakdownRecord.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        rejectionMemoCouponBreakdownRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        rejectionMemoCouponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
        SetViewDataPageMode(PageMode.Create);
      }

      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check coupon Breakdown record count.

      if (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BillingCode == (int)BillingCode.NonSampling && (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 2 || rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 3) && rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
          if (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
          {
              ViewData["IsAwbLinkingRequired"] = false;
          }
          else
          {
              PaxInvoice yourInvoice = _nonSamplingInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceNumber,
                                                                                          rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingMonth,
                                                                                          rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingYear,
                                                                                          rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                                          rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BilledMemberId,
                                                                                          rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BillingMemberId,
                                                                                          null,
                                                                                          null, rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourRejectionNumber);

              if (yourInvoice != null && (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 2 || rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.RejectionMemoRecord.Count > 0)
              {
                  RejectionMemo yourRm = yourInvoice.RejectionMemoRecord.Where(c => c.RejectionMemoNumber == rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
                  if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0) ViewData["IsAwbLinkingRequired"] = false;
              }
          }
      }
      return View("RMCouponCreate", rejectionMemoCouponBreakdownRecord);

    }

    /// <summary>
    /// Creates prime billing coupon and redirects to prime billing coupon listing
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMCouponCreate")]
    public ActionResult RMCouponCreateAndReturn(string invoiceId, string transactionId, RMCoupon rejectionMemoCouponBreakdownRecord)
    {
      //Get attachment Id list
      var couponAttachmentIds = rejectionMemoCouponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          rejectionMemoCouponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
          //SCPID : 105938 - RE: IATA- SIS Bug
          string errorMessage = ValidateRMCouponBreakdownFieldsCalculation(rejectionMemoCouponBreakdownRecord);
          if (!string.IsNullOrEmpty(errorMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, errorMessage);

          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(rejectionMemoCouponBreakdownRecord.TaxAmountDifference, rejectionMemoCouponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 2 */
          MemberManager.ValidateIssuingAirline(rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline);

          //CMP614: Source Code Validation for PAX RMs
          //Get Invoice Header Detail
         // rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
          //Desc: Validate RM source codes.
          /* 313204 - question for validation report NH/205
           * Desc: Error message for IS-WEB is removed and existing exception of file is used. */
          var sourceCodeErrormsg = ValidateRMSourceCodes(rejectionMemoCouponBreakdownRecord, invoiceId);
          if (!String.IsNullOrEmpty(sourceCodeErrormsg) && !sourceCodeErrormsg.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
              throw new ISBusinessException(ErrorCodes.PaxSourceCodes, sourceCodeErrormsg);


        string duplicateErrorMessage;
        rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline =
                     rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim();
        

        rejectionMemoCouponBreakdownRecord.Attachments.Clear();
        rejectionMemoCouponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;

        int vatRecordCountBefore = rejectionMemoCouponBreakdownRecord.VatBreakdown.Count;
        _nonSamplingInvoiceManager.AddRejectionMemoCouponDetails(rejectionMemoCouponBreakdownRecord, invoiceId, out duplicateErrorMessage);
        int vatRecordCountAfter = rejectionMemoCouponBreakdownRecord.VatBreakdown.Count;
        _nonSamplingInvoiceManager.UpdateRejectionMemoCouponAttachment(couponAttachmentIds, rejectionMemoCouponBreakdownRecord.Id);

        //ShowSuccessMessages(Messages.RMCouponCreateSuccessful + duplicateErrorMessage, vatRecordCountBefore, vatRecordCountAfter);
        ShowSuccessMessages(Messages.RMCouponCreateSuccessful, vatRecordCountBefore, vatRecordCountAfter);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);

        TempData["RMCouponRecord"] = "";

        rejectionMemoCouponBreakdownRecord.Attachments.Clear(); // Attachments should not be duplicated. 

        return RedirectToAction("RMEdit", new { invoiceId });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        rejectionMemoCouponBreakdownRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        rejectionMemoCouponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
        SetViewDataPageMode(PageMode.Create);
      }

        rejectionMemoCouponBreakdownRecord.RejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check coupon Breakdown record count.

      if (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BillingCode == (int)BillingCode.NonSampling && (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 2 || rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 3) && rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
        if (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
        {
          ViewData["IsAwbLinkingRequired"] = false;
        }
        else
        {
          PaxInvoice yourInvoice = _nonSamplingInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceNumber,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingMonth,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingYear,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BilledMemberId,
                                                                                      rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.Invoice.BillingMemberId,
                                                                                      null,
                                                                                      null, rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourRejectionNumber);

          if (yourInvoice != null && (rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 2 || rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.RejectionMemoRecord.Count > 0)
          {
            RejectionMemo yourRm = yourInvoice.RejectionMemoRecord.Where(c => c.RejectionMemoNumber == rejectionMemoCouponBreakdownRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
            if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0) ViewData["IsAwbLinkingRequired"] = false;
          }
        }
      }
      return View("RMCouponCreate", rejectionMemoCouponBreakdownRecord);
    }

    /// <summary>
    /// Upload Rejection Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult RMCouponAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<RMCouponAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method RMCouponAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Get invoice header details
        PaxInvoice invoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);
        Logger.Info("Fetched all invoice details successfully.");
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          Logger.Info("Started saving the file" + fileToSave);
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          // On Rejection Memo Coupon Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _nonSamplingInvoiceManager.IsDuplicateRejectionMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
              throw new ISBusinessException(Messages.InvalidFileName);
          }

          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
            throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          Logger.Info("Attachment successfully validated");
          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new RMCouponAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };

            attachment = _nonSamplingInvoiceManager.AddRejectionMemoCouponAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            // assign user info from session and file server info.
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new User();
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
        Logger.Error("Exception", ex);
      }
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
        Logger.Error("Exception", ex);
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Rejection Memo Coupon attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="couponId">Coupon id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download)]
    [HttpGet]
    public FileStreamResult RMCouponAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Fetch data for Rejection Memo for Invoice and display it in grid
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult RMGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var rejectionMemoGrid = new RejectionMemoGrid(ControlIdConstants.TransactionGridId, Url.Action("RMGridData", new { invoiceId }));

      var rejectionMemoCouponsList = _nonSamplingInvoiceManager.GetRejectionMemoList(invoiceId);

      return rejectionMemoGrid.DataBind(rejectionMemoCouponsList.AsQueryable());
    }

    /// <summary>
    /// Delete Rejection memo Coupon Breakdown Record
    /// </summary>
    /// <param name="couponId">Coupon Id</param>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "couponId", TableName = TransactionTypeTable.PAX_RM_COUPON_BREAKDOWN)]
    public JsonResult RMCouponDelete(string couponId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        Guid invoiceId;
        Guid rejectionMemoId;
        var isDeleted = _nonSamplingInvoiceManager.DeleteRejectionMemoCouponRecord(couponId, out rejectionMemoId, out invoiceId);

        details = isDeleted
                    ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, RedirectUrl = Url.Action("RMEdit", new { invoiceId, transactionId = rejectionMemoId }), isRedirect = true }
                    : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Fetch data for Rejection Memo Coupon Breakdown for Rejection Memo and display it in grid
    /// </summary>
    /// <param name="transactionId"></param>
    public JsonResult RejectionMemoCouponBreakdownGridData(string transactionId)
    {
      // Create grid instance and retrieve data from database
      var rmCbGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.CouponGridId, Url.Action("RejectionMemoCouponBreakdownGridData", "Invoice", new { transactionId }));
      var rejectionMemoCouponBreakdownList = _nonSamplingInvoiceManager.GetRejectionMemoCouponBreakdownList(transactionId);

      return rmCbGrid.DataBind(rejectionMemoCouponBreakdownList.AsQueryable());
    }

    /// <summary>
    /// Upload prime billing Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult PrimeBillingAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<PrimeCouponAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015 [Pax]
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method PrimeBillingAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //Get invoice header details 
          PaxInvoice invoice = _nonSamplingInvoiceManager.GetInvoiceDetailForFileUpload(invoiceId);
          Logger.Info("Fetched all invoice details successfully.");
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          Logger.Info("Started saving the file" + fileToSave);
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          fileUploadHelper = new FileAttachmentHelper { FileToSave = fileToSave, FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth) };

          // On Prime Billing Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _nonSamplingInvoiceManager.IsDuplicateCouponAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }

          if (fileUploadHelper.InvalidCharCheck(fileUploadHelper.FileOriginalName))
          {
            throw new ISBusinessException(Messages.InvalidFileName);
          }
          if (!fileUploadHelper.ValidateFileExtention(invoice.BilledMemberId, invoice.BillingCategory))
          {
              throw new ISBusinessException(Messages.InvalidFileExtension);
          }
          Logger.Info("Attachment successfully validated");
          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new PrimeCouponAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };


            attachment = _nonSamplingInvoiceManager.AddCouponLevelAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            // assign user info from session and file server info.
           if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new User();
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
        Logger.Error("Exception", ex);
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
        Logger.Error("Exception", ex);
      }
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        Logger.Error("Exception", ex);
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
        Logger.Error("Exception", ex);
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download prime billing attachment
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.Download)]
    [HttpGet]
    public FileStreamResult PrimeBillingAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _nonSamplingInvoiceManager.GetCouponLevelAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult RMCouponEdit(string invoiceId, string transactionId, string couponId)
    {
      var couponRejectionBreakdownRecord = GetCouponRejectionBreakdownRecord(couponId, transactionId, invoiceId);

      return View("RMCouponBreakdownEdit", couponRejectionBreakdownRecord);
    }

    private RMCoupon GetCouponRejectionBreakdownRecord(string couponId, string transactionId, string invoiceId)
    {
      var couponRejectionBreakdownRecord = _nonSamplingInvoiceManager.GetRejectionMemoCouponDetails(couponId);
      couponRejectionBreakdownRecord.RejectionMemoRecord = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      couponRejectionBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
      couponRejectionBreakdownRecord.RejectionMemoRecord.Invoice = InvoiceHeader;

      // added code to remove the extra special char
      if (!string.IsNullOrEmpty(couponRejectionBreakdownRecord.ProrateSlipDetails))
        couponRejectionBreakdownRecord.ProrateSlipDetails =
          couponRejectionBreakdownRecord.ProrateSlipDetails.Replace("\n", string.Empty).Replace("\r", string.Empty);

      return couponRejectionBreakdownRecord;
    }

    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.View)]
    [HttpGet]
    public ActionResult RMCouponView(string invoiceId, string transactionId, string couponId)
    {
      var couponRejectionBreakdownRecord = GetCouponRejectionBreakdownRecord(couponId, transactionId, invoiceId);

      return View("RMCouponBreakdownEdit", couponRejectionBreakdownRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMCouponEdit(string couponId, string transactionId, string invoiceId, RMCoupon couponRejectionBreakdownRecord)
    {
      couponRejectionBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
      var couponAttachmentIds = couponRejectionBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //SCPID : 105938 - RE: IATA- SIS Bug
          string errorMessage = ValidateRMCouponBreakdownFieldsCalculation(couponRejectionBreakdownRecord);
          if (!string.IsNullOrEmpty(errorMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, errorMessage);
          
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRejectionBreakdownRecord.TaxAmountDifference, couponRejectionBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 2 */
          MemberManager.ValidateIssuingAirline(couponRejectionBreakdownRecord.TicketOrFimIssuingAirline);         

        couponRejectionBreakdownRecord.TicketOrFimIssuingAirline =
          couponRejectionBreakdownRecord.TicketOrFimIssuingAirline.Trim();
        couponRejectionBreakdownRecord.Id = couponId.ToGuid();
        couponRejectionBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent rejection coupon record id to tax records
        foreach (var tax in couponRejectionBreakdownRecord.TaxBreakdown)
        {
          tax.ParentId = couponRejectionBreakdownRecord.Id;
        }
        // Assign parent rejection coupon record id to vat records
        foreach (var vat in couponRejectionBreakdownRecord.VatBreakdown)
        {
          vat.ParentId = couponRejectionBreakdownRecord.Id;
        }
        string duplicateErrorMessage;

        int vatRecordCountBefore = couponRejectionBreakdownRecord.VatBreakdown.Count;
        _nonSamplingInvoiceManager.UpdateRejectionMemoCouponDetails(couponRejectionBreakdownRecord, invoiceId, out duplicateErrorMessage);
        int vatRecordCountAfter = couponRejectionBreakdownRecord.VatBreakdown.Count;

        //ShowSuccessMessages(Messages.RMCouponUpdateSuccessful + duplicateErrorMessage, vatRecordCountBefore, vatRecordCountAfter);
        ShowSuccessMessages(Messages.RMCouponUpdateSuccessful, vatRecordCountBefore, vatRecordCountAfter);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);

        TempData["RMCouponRecord"] = string.Format(@"{0}-{1}", couponRejectionBreakdownRecord.SerialNo, couponRejectionBreakdownRecord.TicketOrFimIssuingAirline);

        return RedirectToAction("RMCouponCreate", new { invoiceId, transactionId, couponId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var rejectionMemo = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        rejectionMemo.Invoice = invoice;
        couponRejectionBreakdownRecord.RejectionMemoRecord = rejectionMemo;
        couponRejectionBreakdownRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        couponRejectionBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
        couponRejectionBreakdownRecord.Id = couponId.ToGuid();
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMCouponBreakdownEdit", couponRejectionBreakdownRecord);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMCouponBreakdownEdit")]
    public ActionResult RMCouponClone(string couponId, string transactionId, string invoiceId, RMCoupon couponRejectionBreakdownRecord)
    {
        var logRefId = Guid.NewGuid();

        var log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                                   BillingCategorys.Passenger.ToString(), "Stage 1: In RMCouponClone Start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);


      couponRejectionBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
      couponRejectionBreakdownRecord.TicketOrFimIssuingAirline =
       couponRejectionBreakdownRecord.TicketOrFimIssuingAirline.Trim();
      var couponAttachmentIds = couponRejectionBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                                 BillingCategorys.Passenger.ToString(), "Stage 2:  GetRejectionMemoRecordDetails Start ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      var rejectionMemo = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                                 BillingCategorys.Passenger.ToString(), "Stage 2: GetRejectionMemoRecordDetails completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
     
      var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

       log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                              BillingCategorys.Passenger.ToString(), "Stage 3: GetInvoiceHeaderDetails completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      rejectionMemo.Invoice = invoice;

      try
      {
          //SCPID : 105938 - RE: IATA- SIS Bug
          string errorMessage = ValidateRMCouponBreakdownFieldsCalculation(couponRejectionBreakdownRecord);
          if (!string.IsNullOrEmpty(errorMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, errorMessage);
          
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRejectionBreakdownRecord.TaxAmountDifference, couponRejectionBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                            BillingCategorys.Passenger.ToString(), "Stage 4: ValidateIssuingAirline start ", SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 2 */
          MemberManager.ValidateIssuingAirline(couponRejectionBreakdownRecord.TicketOrFimIssuingAirline);
          log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                            BillingCategorys.Passenger.ToString(), "Stage 4: ValidateIssuingAirline completed ", SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);
         
        couponRejectionBreakdownRecord.Id = couponId.ToGuid();
        couponRejectionBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent rejection coupon record id to tax records
        foreach (var tax in couponRejectionBreakdownRecord.TaxBreakdown)
        {
          tax.ParentId = couponRejectionBreakdownRecord.Id;
        }
        // Assign parent rejection coupon record id to vat records
        foreach (var vat in couponRejectionBreakdownRecord.VatBreakdown)
        {
          vat.ParentId = couponRejectionBreakdownRecord.Id;
        }
        string duplicateErrorMessage;
        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                          BillingCategorys.Passenger.ToString(), "Stage 5: UpdateRejectionMemoCouponDetails start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
         
        _nonSamplingInvoiceManager.UpdateRejectionMemoCouponDetails(couponRejectionBreakdownRecord, invoiceId, out duplicateErrorMessage);

        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                          BillingCategorys.Passenger.ToString(), "Stage 5: UpdateRejectionMemoCouponDetails completed ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
         
        couponRejectionBreakdownRecord.RejectionMemoRecord = rejectionMemo;

        // TODO: Add changes for validation: Any change in amounts will result in change of Form E level calculations
        //ShowSuccessMessage(Messages.RMCouponUpdateSuccessful + duplicateErrorMessage);
        ShowSuccessMessage(Messages.RMCouponUpdateSuccessful);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);

        couponRejectionBreakdownRecord.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        //CMP#501 :
        ViewData["IsAwbLinkingRequired"] = true;
        //Get your invoice details to check coupon Breakdown record count.

        if (invoice.BillingCode == (int)BillingCode.NonSampling && (rejectionMemo.RejectionStage == 2 || rejectionMemo.RejectionStage == 3) && rejectionMemo.IsLinkingSuccessful == true)
        {
            if (rejectionMemo.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
            {
                ViewData["IsAwbLinkingRequired"] = false;
            }
            else
            {
                log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                        BillingCategorys.Passenger.ToString(), "Stage 6: GetInvoiceWithRMCoupons start ", SessionUtil.UserId, logRefId.ToString());
                _referenceManager.LogDebugData(log);

                PaxInvoice yourInvoice = _nonSamplingInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemo.YourInvoiceNumber,
                                                                                            rejectionMemo.YourInvoiceBillingMonth,
                                                                                            rejectionMemo.YourInvoiceBillingYear,
                                                                                            rejectionMemo.YourInvoiceBillingPeriod,
                                                                                            rejectionMemo.Invoice.BilledMemberId,
                                                                                            rejectionMemo.Invoice.BillingMemberId,
                                                                                            null,
                                                                                            null, rejectionMemo.YourRejectionNumber);

                log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                        BillingCategorys.Passenger.ToString(), "Stage 6: GetInvoiceWithRMCoupons completed ", SessionUtil.UserId, logRefId.ToString());
                _referenceManager.LogDebugData(log);


                if (yourInvoice != null && (rejectionMemo.RejectionStage == 2 || rejectionMemo.RejectionStage == 3) && yourInvoice.RejectionMemoRecord.Count > 0)
                {
                    RejectionMemo yourRm = yourInvoice.RejectionMemoRecord.Where(c => c.RejectionMemoNumber == rejectionMemo.YourRejectionNumber).FirstOrDefault();
                    if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0) ViewData["IsAwbLinkingRequired"] = false;
                }
            }
        }
        log = _referenceManager.GetDebugLog(DateTime.Now, "RMCouponClone", this.ToString(),
                                   BillingCategorys.Passenger.ToString(), "Stage 1: RMCouponClone Completed ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        return View("RMCouponCreate", couponRejectionBreakdownRecord);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        couponRejectionBreakdownRecord.RejectionMemoRecord = rejectionMemo;
        couponRejectionBreakdownRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        couponRejectionBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
        couponRejectionBreakdownRecord.Id = couponId.ToGuid();
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMCouponBreakdownEdit", couponRejectionBreakdownRecord);
    }

    /// <summary>
    /// Creates prime billing coupon and redirects to prime billing coupon listing
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMCouponBreakdownEdit")]
    public ActionResult RMCouponEditAndReturn(string couponId, string transactionId, string invoiceId, RMCoupon couponRejectionBreakdownRecord)
    {
      couponRejectionBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
      var couponAttachmentIds = couponRejectionBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          //SCPID : 105938 - RE: IATA- SIS Bug
          string errorMessage = ValidateRMCouponBreakdownFieldsCalculation(couponRejectionBreakdownRecord);
          if (!string.IsNullOrEmpty(errorMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, errorMessage);
          
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponRejectionBreakdownRecord.TaxAmountDifference, couponRejectionBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 2 */
          MemberManager.ValidateIssuingAirline(couponRejectionBreakdownRecord.TicketOrFimIssuingAirline);

         couponRejectionBreakdownRecord.TicketOrFimIssuingAirline =
         couponRejectionBreakdownRecord.TicketOrFimIssuingAirline.Trim();
        couponRejectionBreakdownRecord.Id = couponId.ToGuid();
        couponRejectionBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent rejection coupon record id to tax records
        foreach (var tax in couponRejectionBreakdownRecord.TaxBreakdown)
        {
          tax.ParentId = couponRejectionBreakdownRecord.Id;
        }
        // Assign parent rejection coupon record id to vat records
        foreach (var vat in couponRejectionBreakdownRecord.VatBreakdown)
        {
          vat.ParentId = couponRejectionBreakdownRecord.Id;
        }

        int vatRecordCountBefore = couponRejectionBreakdownRecord.VatBreakdown.Count;

        string duplicateErrorMessage;
        _nonSamplingInvoiceManager.UpdateRejectionMemoCouponDetails(couponRejectionBreakdownRecord, invoiceId, out duplicateErrorMessage);
        int vatRecordCountAfter = couponRejectionBreakdownRecord.VatBreakdown.Count;

        // TODO: Add changes for validation: Any change in amounts will result in change of Form E level calculations

        //ShowSuccessMessages(Messages.RMCouponUpdateSuccessful + duplicateErrorMessage, vatRecordCountBefore, vatRecordCountAfter);
        ShowSuccessMessages(Messages.RMCouponUpdateSuccessful, vatRecordCountBefore, vatRecordCountAfter);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage,true);


        TempData["RMCouponRecord"] = "";

        couponRejectionBreakdownRecord.Attachments.Clear(); // Attachments should not be duplicated. 

        return RedirectToAction("RMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var rejectionMemo = _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        var invoice = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        rejectionMemo.Invoice = invoice;
        couponRejectionBreakdownRecord.RejectionMemoRecord = rejectionMemo;
        couponRejectionBreakdownRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        // SCP190774: ERROR SAVING RM
        couponRejectionBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
        couponRejectionBreakdownRecord.Id = couponId.ToGuid();
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMCouponBreakdownEdit", couponRejectionBreakdownRecord);
    }

    /// <summary>
    ///call from jquery using Ajax and json, for getting the rejection memo coupon break down details
    /// </summary>
    [HttpPost]
    public JsonResult GetRMCouponBreakdownDetails(string issuingAirline, string couponNo, long ticketDocNo, string rmId, string billingMemberId, string billedMemberId)
    {
      var rmLinkedCoupons = _nonSamplingInvoiceManager.GetRMCouponBreakdownRecordDetails(issuingAirline,
                                                                                         Convert.ToInt32(couponNo),
                                                                                         ticketDocNo,
                                                                                         rmId.ToGuid(),
                                                                                         Convert.ToInt32(billingMemberId),
                                                                                         Convert.ToInt32(billedMemberId));

      return Json(rmLinkedCoupons);
    }

    /// <summary>
    /// Get the single record details from the list of RM coupon
    /// </summary>
    [HttpPost]
    public JsonResult GetRMCouponBreakdownSingleRecordDetails(string couponId, string rejectionMemoId, int billingMemberId, int billedMemberId)
    {
      var rmLinkedSingleCouponsDetails = _nonSamplingInvoiceManager.GetRMCouponBreakdownSingleRecordDetails(couponId.ToGuid(), rejectionMemoId.ToGuid(), billingMemberId, billedMemberId);

      return Json(rmLinkedSingleCouponsDetails);
    }

    /// <summary>
    /// This method is use to check amount difference in tax breakdowns with total amount.
    /// Issue: 65997 - No tax breakdwon for invoice 2012D74226, RM 618240311.
    /// This validation will be applicable RM, BM, Prime coupons.
    /// SCP105897 & SCP105226 [added Math.Round() for calculatedAmount]
    /// </summary>
    /// <param name="targetAmount">target Amount</param>
    /// <param name="rmTaxBreakdowns">RM Tax Breakdowns</param>
    /// <param name="pcTaxBreakdowns">PC Tax Breakdowns</param>
    /// <param name="bmTaxBreakdowns">BM Tax Breakdowns</param>
    /// <returns>return false if tax breakdowns are valid.</returns>
    private bool inValidTaxBreakDown(double targetAmount, IEnumerable<RMCouponTax> rmTaxBreakdowns = null, IEnumerable<PrimeCouponTax> pcTaxBreakdowns = null, IEnumerable<BMCouponTax> bmTaxBreakdowns = null)
    {
      double calculatedAmount = 0;
      // Rejection memo tax breakdown validation
      if (rmTaxBreakdowns != null)
      {
        calculatedAmount = rmTaxBreakdowns.Aggregate<RMCouponTax, double>(0, (current, rmCouponTax) => current + rmCouponTax.AmountDifference);
      }
      // Prime coupon tax breakdown validation
      else if (pcTaxBreakdowns != null)
      {
        calculatedAmount = pcTaxBreakdowns.Aggregate<PrimeCouponTax, double>(0, (current, primeCouponTax) => current + primeCouponTax.Amount);
      }
      // Billing memo tax breakdown validation
      else if (bmTaxBreakdowns != null)
      {
        calculatedAmount = bmTaxBreakdowns.Aggregate<BMCouponTax, double>(0, (current, bmCouponTax) => current + bmCouponTax.Amount);
      }
      return Math.Round(calculatedAmount, 2).Equals(targetAmount) ? false : true;
    }

      /// <summary>
      /// SCPID : 105938 - RE: IATA- SIS Bug
      /// Validates the calculation of RM memo fields
      /// </summary>
      /// <param name="record"></param>
      /// <returns></returns>
      private string ValidateRMMemoFieldsCalculation(RejectionMemo record)
      {
        if (record != null)
        {
            List<string> fields = new List<string>();

            if (record.RejectionStage == (int)RejectionStage.StageTwo)
            {
                if (ConvertUtil.Round(record.TotalGrossAcceptedAmount - record.TotalGrossAmountBilled,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.TotalGrossDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Gross");

                if (ConvertUtil.Round(record.TotalTaxAmountAccepted - record.TotalTaxAmountBilled,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.TotalTaxAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Tax");

                if (ConvertUtil.Round(record.AcceptedIscAmount - record.AllowedIscAmount,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.IscDifference, Constants.PaxDecimalPlaces))
                    fields.Add("ISC");

                if (ConvertUtil.Round(record.AcceptedUatpAmount - record.AllowedUatpAmount,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.UatpAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("UATP");

                if (ConvertUtil.Round(record.AcceptedHandlingFee - record.AllowedHandlingFee,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.HandlingFeeAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Handling Fee");

                if (ConvertUtil.Round(record.AcceptedOtherCommission - record.AllowedOtherCommission,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.OtherCommissionDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Other Commission");

                if (ConvertUtil.Round(record.TotalVatAmountAccepted - record.TotalVatAmountBilled,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.TotalVatAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("VAT");
            }
            else
            {
                if (ConvertUtil.Round(record.TotalGrossAmountBilled - record.TotalGrossAcceptedAmount,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.TotalGrossDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Gross");

                if (ConvertUtil.Round(record.TotalTaxAmountBilled - record.TotalTaxAmountAccepted,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.TotalTaxAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Tax");

                if (ConvertUtil.Round(record.AllowedIscAmount - record.AcceptedIscAmount,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.IscDifference, Constants.PaxDecimalPlaces))
                    fields.Add("ISC");

                if (ConvertUtil.Round(record.AllowedUatpAmount - record.AcceptedUatpAmount,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.UatpAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("UATP");

                if (ConvertUtil.Round(record.AllowedHandlingFee - record.AcceptedHandlingFee,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.HandlingFeeAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Handling Fee");

                if (ConvertUtil.Round(record.AllowedOtherCommission - record.AcceptedOtherCommission,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.OtherCommissionDifference, Constants.PaxDecimalPlaces))
                    fields.Add("Other Commission");

                if (ConvertUtil.Round(record.TotalVatAmountBilled - record.TotalVatAmountAccepted,
                                      Constants.PaxDecimalPlaces) !=
                    ConvertUtil.Round(record.TotalVatAmountDifference, Constants.PaxDecimalPlaces))
                    fields.Add("VAT");

            }

            return String.Join(",", fields);
        }
        return string.Empty;
      }

      /// <summary>
      /// SCPID : 105938 - RE: IATA- SIS Bug
      /// Validates the calculation of RM coupon breakdown fields
      /// </summary>
      /// <param name="record"></param>
      /// <returns></returns>
      private string ValidateRMCouponBreakdownFieldsCalculation(RMCoupon record)
      {
          if (record != null)
          {
              List<string> fields = new List<string>();
              record.RejectionMemoRecord =
                  _nonSamplingInvoiceManager.GetRejectionMemoRecordDetails(record.RejectionMemoId.ToString());
              if (record.RejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo)
              {
                  if (
                      ConvertUtil.Round(record.GrossAmountAccepted - record.GrossAmountBilled,
                                        Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.GrossAmountDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Gross");

                  if (
                      ConvertUtil.Round(record.TaxAmountAccepted - record.TaxAmountBilled, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.TaxAmountDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Tax");

                  if (
                      ConvertUtil.Round(record.AcceptedIscAmount - record.AllowedIscAmount, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.IscDifference, Constants.PaxDecimalPlaces))
                      fields.Add("ISC");

                  if (
                      ConvertUtil.Round(record.AcceptedUatpAmount - record.AllowedUatpAmount, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.UatpDifference, Constants.PaxDecimalPlaces))
                      fields.Add("UATP");

                  if (
                      ConvertUtil.Round(record.AcceptedHandlingFee - record.AllowedHandlingFee,
                                        Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.HandlingDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Handling Fee");

                  if (
                      ConvertUtil.Round(record.AcceptedOtherCommission - record.AllowedOtherCommission,
                                        Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.OtherCommissionDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Other Commission");

                  if (
                      ConvertUtil.Round(record.VatAmountAccepted - record.VatAmountBilled, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.VatAmountDifference, Constants.PaxDecimalPlaces))
                      fields.Add("VAT");
              }
              else
              {
                  if (
                      ConvertUtil.Round(record.GrossAmountBilled - record.GrossAmountAccepted,
                                        Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.GrossAmountDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Gross");

                  if (
                      ConvertUtil.Round(record.TaxAmountBilled - record.TaxAmountAccepted, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.TaxAmountDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Tax");

                  if (
                      ConvertUtil.Round(record.AllowedIscAmount - record.AcceptedIscAmount, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.IscDifference, Constants.PaxDecimalPlaces))
                      fields.Add("ISC");

                  if (
                      ConvertUtil.Round(record.AllowedUatpAmount - record.AcceptedUatpAmount, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.UatpDifference, Constants.PaxDecimalPlaces))
                      fields.Add("UATP");

                  if (
                      ConvertUtil.Round(record.AllowedHandlingFee - record.AcceptedHandlingFee,
                                        Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.HandlingDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Handling Fee");

                  if (
                      ConvertUtil.Round(record.AllowedOtherCommission - record.AcceptedOtherCommission,
                                        Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.OtherCommissionDifference, Constants.PaxDecimalPlaces))
                      fields.Add("Other Commission");

                  if (
                      ConvertUtil.Round(record.VatAmountBilled - record.VatAmountAccepted, Constants.PaxDecimalPlaces) !=
                      ConvertUtil.Round(record.VatAmountDifference, Constants.PaxDecimalPlaces))
                      fields.Add("VAT");
              }

              return String.Join(",", fields);
          }
          return string.Empty;
      }

      /// <summary>
      /// This function is used to validate source code for rejection memo based on stage.
      /// </summary>
      /// <param name="rejectionMemoCouponBreakdownRecord"></param>
      //CMP614: Source Code Validation for PAX RMs
      public String ValidateRMSourceCodes(RejectionMemo rejectionMemoData,string invoiceId)
      {
        String sourceCodeErrorMsg = String.Empty;

        if (rejectionMemoData != null)
        {
          //Get Invoice detail.
          var invoiceDetail = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

          //CMP #614: Source Code Validation for PAX RMs.
          var sourceCodeCriteria = new Iata.IS.Model.Pax.Common.RMSourceCodeValidationCriteria()
          {
            InvoiceNumber = rejectionMemoData.YourInvoiceNumber,
            BillingYear = rejectionMemoData.YourInvoiceBillingYear,
            BillingMonth = rejectionMemoData.YourInvoiceBillingMonth,
            BillingPeriod = rejectionMemoData.YourInvoiceBillingPeriod,
            RejectionMemoNumber = rejectionMemoData.YourRejectionNumber,
            RejectionStage = rejectionMemoData.RejectionStage,
            BillingMemberId = invoiceDetail.BillingMemberId,
            BilledMemberId = invoiceDetail.BilledMemberId,
            FimBMCMNumber = rejectionMemoData.FimBMCMNumber,
            FimCouponNumber = rejectionMemoData.FimCouponNumber,
            SourceCode = rejectionMemoData.SourceCodeId,
            IgnoreValidationOnRMSourceCodes = 1
          };

          //Validated rejection memo source code. 
          sourceCodeErrorMsg = _nonSamplingInvoiceManager.ValidateRMSourceCode(sourceCodeCriteria);
        }

        //Return error msg.
        return sourceCodeErrorMsg;
      }

      /// <summary>
      /// This function is used to validate source code for rejection memo based on stage.
      /// </summary>
      /// <param name="rejectionMemoCouponBreakdownRecord"></param>
      //CMP614: Source Code Validation for PAX RMs
      private String ValidateRMSourceCodes(RMCoupon rmCoupon, String invoiceId)
      {
        String sourceCodeErrorMsg = String.Empty;

        if (rmCoupon != null && rmCoupon.RejectionMemoRecord != null && rmCoupon.RejectionMemoRecord.RejectionStage == 1)
        {
          //Get Invoice detail.
          var invoiceDetail = _nonSamplingInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

          //CMP #614: Source Code Validation for PAX RMs.
          var sourceCodeCriteria = new Iata.IS.Model.Pax.Common.RMSourceCodeValidationCriteria()
          {
            InvoiceNumber = rmCoupon.RejectionMemoRecord.YourInvoiceNumber,
            BillingYear = rmCoupon.RejectionMemoRecord.YourInvoiceBillingYear,
            BillingMonth = rmCoupon.RejectionMemoRecord.YourInvoiceBillingMonth,
            BillingPeriod = rmCoupon.RejectionMemoRecord.YourInvoiceBillingPeriod,
            RejectionMemoNumber = rmCoupon.RejectionMemoRecord.YourRejectionNumber,
            RejectionStage = rmCoupon.RejectionMemoRecord.RejectionStage,
            BillingMemberId = invoiceDetail.BillingMemberId,
            BilledMemberId = invoiceDetail.BilledMemberId,
            FimBMCMNumber = Convert.ToString(rmCoupon.TicketDocOrFimNumber),
            FimCouponNumber = rmCoupon.TicketOrFimCouponNumber,
            SourceCode = rmCoupon.RejectionMemoRecord.SourceCodeId,
            IgnoreValidationOnRMSourceCodes = 1
          };
         
          //Validated rejection memo source code. 
          sourceCodeErrorMsg = _nonSamplingInvoiceManager.ValidateRMSourceCode(sourceCodeCriteria);
        }

        //Return error msg.
        return sourceCodeErrorMsg;
      }
  }
}
