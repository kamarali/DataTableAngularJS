using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Security;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.UIModel.BillingHistory.Pax;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Business;
using System;
using Iata.IS.Core.DI;
using Iata.IS.Web.Util.Filters;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Web.Areas.Pax.Controllers.Base
{
  public abstract class PaxInvoiceControllerBase : InvoiceControllerBase<PaxInvoice>
  {
    private readonly IInvoiceManager _invoiceManager;
    public IValidationErrorManager ValidationManager { get; set;}
    public IReferenceManager ReferenceManager { get; set; }

    private const string InvoiceVatGridAction = "InvoiceVatGrid";
    private const string AvailableVatGridAction = "AvailableVatGridData";
    private const string UnappliedAmountVatGridAction = "UnappliedAmountVatGridData";
    private const string SourceCodeGridAction = "SourceCodeGridData";

    protected abstract int BillingCodeId { get; }

    protected PaxInvoiceControllerBase(IInvoiceManager invoiceManager)
    {
      _invoiceManager = invoiceManager;
    }

    protected override PaxInvoice GetInvoiceHeader(string invoiceNumber)
    {
      return _invoiceManager.GetInvoiceHeaderDetails(invoiceNumber);
    }

    protected override bool IsValidBillingCode(InvoiceBase invoice)
    {
      bool isValidBillingCode = false;
      var paxInvoice = (PaxInvoice) invoice;
      if (paxInvoice != null)
      {
        if (paxInvoice.BillingCode == BillingCodeId)
        {
          if (paxInvoice.BillingCode == 0)
          {
            if (InvoiceType == paxInvoice.InvoiceType) {isValidBillingCode = true;}
          }
          else
          {
            isValidBillingCode = true;
          }
        }
      }

      return isValidBillingCode;
    }

    /// <summary>
    /// Fetch data for source code data and display in grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult SourceCodeGridData(string invoiceId)
    {
      var sourceCodeGrid = new SourceCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SourceCodeGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);

      var sourceCodeTotal = _invoiceManager.GetSourceCodeList(invoiceId).AsQueryable();

      return sourceCodeGrid.DataBind(sourceCodeTotal);
    }

    /// <summary>
    /// Following action is used to retrieve Source Code Vat total and display it on SourceCodeVatTotal grid
    /// </summary>
    /// <param name="sourceCodeId">SourceCodeVat Total Id</param>
    /// <returns>Json result for SourceCode vat total</returns>
    public JsonResult GetSourceCodeVatTotal(string sourceCodeId)
    {
      // Call GetSourceCodeVatTotal() method which returns SourceCode vat total details
      var sourceCodeVatTotalList = _invoiceManager.GetSourceCodeVatTotal(sourceCodeId);

      // Return Json result
      return Json(sourceCodeVatTotalList);
    }

    /// <summary>
    /// Invoice level VAT 
    /// </summary>
    /// <returns></returns>
    public virtual ActionResult Vat(string invoiceId)
    {
      return View(VatBase(invoiceId));
    }

    public virtual ActionResult VatView(string invoiceId)
    {
      return View("Vat",VatBase(invoiceId));
    }

    private PaxInvoice VatBase(string invoiceId)
    {
      PaxInvoice invoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
      bool isGridViewOnly = false;

      SetPageMode(invoice.InvoiceStatus);

      // If Invoice submission method is Auto Billing, set page mode to view
      if (invoice.SubmissionMethodId == (int)SubmissionMethod.AutoBilling)
      {
        ViewData[ViewDataConstants.PageMode] = PageMode.View;
      }

      if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
        isGridViewOnly = true;
      }

      // Create grid instance for invoice vat 
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);
      ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

      if(ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
      {
        // Create grid instance for available vat 
        var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.AvailableVatGrid] = availableVatGrid.Instance;

        //Create grid instance for vat not applied amount
        var unappliedAmountVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.UnappliedAmountVatGrid] = unappliedAmountVatGrid.Instance;
      }
      
      return invoice;
    }

    /// <summary>
    /// Save Invoice level VAT 
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, TableName = TransactionTypeTable.INVOICE)]
    public virtual JsonResult Vat(FormCollection form, string invoiceId)
    {
      try
      {
        var vat = new JavaScriptSerializer().Deserialize(form[0], typeof(InvoiceVat));
        var record = vat as InvoiceVat;
        _invoiceManager.AddInvoiceLevelVat(record);

        var details = new UIMessageDetail
                        {
                          IsFailed = false,
                          Message = Messages.RecordSaveSuccessful,
                          isRedirect = true,
                          RedirectUrl = Url.Action("Vat")

                        };
        return Json(details);
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);

        var details = new UIMessageDetail
                        {
                          IsFailed = false,
                          Message = string.Format(Messages.RecordSaveException, GetDisplayMessage(businessException.ErrorCode))
                        };
        return Json(details);
      }
    }
    ///SCP120658 changed parameter name from Transaction Id to id
    /// <summary>
    /// Delete Invoice Vat Record
    /// </summary>
    /// <param name="id">Transaction Id</param>
    /// <returns></returns>
    [RestrictInvoiceUpdate(TransactionParamName = "id", IsJson = true, TableName = TransactionTypeTable.PAX_INVOICE_TOT_VAT_BREAKDOWN)]
    public virtual JsonResult VatDelete(string id)
    {
      UIMessageDetail details;
      try
      {
        //Delete record
        bool isDeleted = _invoiceManager.DeleteInvoiceLevelVat(id);

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
    /// <returns></returns>
    public JsonResult InvoiceVatGrid(string invoiceId, bool isGridViewOnly)
    {
      //Create grid instance and retrieve data from database
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);

      var vatData = _invoiceManager.GetInvoiceLevelVatList(invoiceId).AsQueryable();
      return invoiceVatGrid.DataBind(vatData);
    }

    /// <summary>
    /// Available Vat Data to populate in the Grid
    /// </summary>
    /// <returns></returns>
    public JsonResult AvailableVatGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var vatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));

      var vatData = _invoiceManager.GetInvoiceLevelDerivedVatList(invoiceId).AsQueryable();
      int count = 1;
      foreach (var derivedVatDetails in vatData)
      {
        derivedVatDetails.RowNumber = count++;
      }
      return vatGrid.DataBind(vatData);
    }

    /// <summary>
    /// Used to instantiate SourceCode vat Total grid
    /// </summary>
    /// <returns>null</returns>
    public JsonResult AvailableEmptySourceCodeVatTotalGridData()
    {
      return null;
    }

    /// <summary>
    ///Unapplied Vat amount Data to populate in the Grid
    /// </summary>
    /// <returns></returns>
    public JsonResult UnappliedAmountVatGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var unappliedVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
      var notAppliedVatList = _invoiceManager.GetNonAppliedVatList(invoiceId).AsQueryable();
      int count = 1;
      foreach (var nonAppliedVatDetails in notAppliedVatList)
      {
        nonAppliedVatDetails.RowNumber = count++;
      }
      return unappliedVatGrid.DataBind(notAppliedVatList);
    }

    protected PaxInvoice ValidateInvoice(IInvoiceManager manager, string invoiceId)
    {
      PaxInvoice invoice = null;

      try
      {
        bool isFutureSubmitted;
        // Validate Invoice
        invoice = manager.ValidateInvoice(invoiceId, out isFutureSubmitted);

        var message = string.Empty;
        //SCP85837: PAX CGO Sequence Number
        if (invoice.IsRecordSequenceArranged == RecordSequence.IsArranged)
        {
            message += " ¥ " + Messages.InvalidSequenceNoSerial;
        }

        //SCP149711: Incorrect Form E UA to 3M
        if (invoice.IsRecalculatedFormE == RecalculateFormE.Yes)
        {
          message += " ¥ " + Messages.RecalculateFormE;
        }

        switch (invoice.InvoiceStatus)
        {
            case InvoiceStatusType.ReadyForSubmission:
                ShowSuccessMessage(string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber) + message);
                break;

            case InvoiceStatusType.FutureSubmitted:
                ShowSuccessMessage(string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber) + message);
                break;

            default:
                ShowErrorMessage(string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber), true);
                break;
        }


      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode, true);
      }

      return invoice;
    }

    protected PaxInvoice SubmitInvoice(string invoiceId)
    {
      PaxInvoice invoice = null;

      try
      {
        invoice = _invoiceManager.SubmitInvoice(invoiceId);

       

        switch (invoice.InvoiceStatus)
        {
          case InvoiceStatusType.ReadyForBilling:
            TempData[ViewDataConstants.SuccessMessage] = string.Format(Messages.InvoiceSubmissionSuccessful, invoice.InvoiceNumber);
            break;
          case InvoiceStatusType.ValidationError:
            TempData[ViewDataConstants.ErrorMessage] = string.Format(Messages.InvoiceSubmissionFailed, invoice.InvoiceNumber, EnumMapper.GetInvoiceStatusDisplayValue((int)invoice.InvoiceStatus));
            break;
        }
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode, true);
      }

      return invoice;
    }

    protected UIMessageDetail HandleDeleteException(string errorCode)
    {
      return new UIMessageDetail
      {
        IsFailed = true,
        Message = string.Format(Messages.DeleteException, GetDisplayMessage(errorCode))
      };
    }

    protected UIMessageDetail GetDeleteMessage(bool isDeleted)
    {
      return isDeleted ? new UIMessageDetail
      {
        IsFailed = false,
        Message = Messages.DeleteSuccessful
      } : new UIMessageDetail
      {
        IsFailed = true,
        Message = Messages.DeleteFailed
      };
    }

    /// <summary>
    /// Delete record and redirect to given url
    /// </summary>
    /// <param name="isDeleted"></param>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    protected UIMessageDetail GetDeleteMessage(bool isDeleted, string redirectUrl)
    {
      return isDeleted ? new UIMessageDetail
      {
        IsFailed = false,
        Message = Messages.DeleteSuccessful,
        RedirectUrl = redirectUrl,
        isRedirect = true
      } : new UIMessageDetail
      {
        IsFailed = true,
        Message = Messages.DeleteFailed
      };
    }

    /// <summary>
    /// Add member location info
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="invoice"></param>
    protected void MakeInvoiceRenderReady(Guid invoiceId, PaxInvoice invoice)
    {
      invoice.LastUpdatedBy = SessionUtil.UserId;
      InitMemberLocationInfo(invoice);
    }

    // Called on Edit Invoice header page 
    public JsonResult GetSubmittedErrors(string invoiceId)
    {
      var submittedErrors = ValidationManager.GetValidationErrors(invoiceId);
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      return submittedErrorsGrid.DataBind(submittedErrors);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    public virtual ActionResult Submit(string invoiceId)
    {
      var submittedInvoice = SubmitInvoice(invoiceId);

      var controllerName = submittedInvoice.InvoiceType == InvoiceType.CreditNote ? "CreditNote" : "Invoice";

      return RedirectToAction(submittedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling ? "View" : "Edit", controllerName, new { invoiceId });
      
    }
    
    [HttpPost]
    public JsonResult InitiateRejection(string rejectedRecordIds, string invoiceId, int billingYear, int billingMonth, int billingPeriod, int smi, int rejectionTransactionType)
    {
      var authorizationManager = Ioc.Resolve<IAuthorizationManager>();
      if (SessionUtil.IsLoggedIn && authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Pax.Receivables.NonSampleInvoice.CreateOrEdit))
      {
          //CMP#641: Time Limit Validation on Third Stage PAX Rejections
          if (rejectionTransactionType == (int)TransactionType.RejectionMemo3 || rejectionTransactionType == (int)TransactionType.SamplingFormXF)
          {
              var transactionType = (rejectionTransactionType == (int)TransactionType.SamplingFormXF) ? TransactionType.SamplingFormXF : TransactionType.RejectionMemo3;
              var yourInvoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
              IList<IsValidationExceptionDetail> exceptionDetailsList = new List<IsValidationExceptionDetail>();
              _invoiceManager.ValidatePaxStageThreeRmForTimeLimit(transactionType, smi, null, yourInvoice, exceptionDetailsList: exceptionDetailsList,isBillingHistory:true);
              if (exceptionDetailsList.Count > 0)
              {
                  var jsonresult = new JsonResult();
                  //Display failure message to user
                  jsonresult.Data = new UIMessageDetail
                  {
                      Message = exceptionDetailsList.First().ErrorDescription,
                      ErrorCode = "BPAXNS_10969",
                      IsFailed = true
                  };
                  return jsonresult;
              }

          }

        // check if transactions have been rejected in some rejection memo.
        var transactions = _invoiceManager.GetRejectedTransactionDetails(rejectedRecordIds);
        var result = new JsonResult();
        //CMP#624 : 2.10 - Change#6 : Time Limits
        // While calculating time limit for SMI X it should behave like SMI I.
        // Check if rejection is outside time limit.
        transactions.IsTransactionOutsideTimeLimit = !ReferenceManager.IsTransactionInTimeLimitMethodH((TransactionType) rejectionTransactionType, smi, billingYear,billingMonth, billingPeriod);

        // For display of warning message for - 1. Coupon/memo already rejected, 2. Rejection outside time limit.
        if ((transactions.Transactions != null && transactions.Transactions.Count > 0) || transactions.IsTransactionOutsideTimeLimit)
        {
          return Json(transactions);
        }
        return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
      }
      else
      {
        return new JsonResult() { Data = new UIMessageDetail { Message = "Success", IsFailed = true } };
      }
    }

    [HttpPost]
    public JsonResult InitiateDuplicateRejection(string rejectedRecordIds, string invoiceId)
    {
      return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
    }

    private JsonResult GetInvoicesForBillingHistory(string rejectedRecordIds, string invoiceId)
    {
      var invoice = GetInvoiceHeader(invoiceId);

      var billingCode = invoice.BillingCode;
      if (billingCode > (int)BillingCode.NonSampling)
      {
        ++billingCode;
      }
      var billedMemberId = invoice.BillingMemberId;
      var billingMemberId = invoice.BilledMemberId;
      var settlementMethodId = invoice.SettlementMethodId;
      var invoices = billingCode != (int)BillingCode.NonSampling
                       ? _invoiceManager.GetInvoicesForSamplingBillingHistory(billingCode, billedMemberId, billingMemberId, invoice.ProvisionalBillingMonth, invoice.ProvisionalBillingYear, settlementMethodId)
                       : _invoiceManager.GetInvoicesForBillingHistory(billingCode, billedMemberId, SessionUtil.MemberId, settlementMethodId);

      TempData[TempDataConstants.RejectedRecordIds] = string.Format("{0}@{1}", rejectedRecordIds, invoiceId);
      var result = new JsonResult();

      // If single open invoice found, redirect to rejection memo create page of that invoice.
      if (invoices.Count() == 1)
      {
        string redirectUrl = string.Empty;
        switch (invoice.BillingCode)
        {
          case (int)BillingCode.NonSampling:
            redirectUrl = Url.Action("RMCreate", "Invoice", new { invoiceId = invoices.First().Id.Value() });
            break;
          case (int)BillingCode.SamplingFormDE:
            redirectUrl = Url.Action("RMCreate", "FormF", new { invoiceId = invoices.First().Id.Value() });
            break;
          case (int)BillingCode.SamplingFormF:
            redirectUrl = Url.Action("RMCreate", "FormXF", new { invoiceId = invoices.First().Id.Value() });
            break;
        }

        IList<AuditTrailInvoice> invoiceList = new List<AuditTrailInvoice>()
                                                 {
                                                   new AuditTrailInvoice()
                                                     {
                                                       RedirectUrl = redirectUrl
                                                     }
                                                 };

        result.Data = invoiceList.ToList();

        return result;
      }

      if (invoices.Count() > 1)
      {
        IList<AuditTrailInvoice> invoiceList = invoices.Select(paxInvoice => new AuditTrailInvoice() { Id = paxInvoice.Id, InvoiceNumber = paxInvoice.InvoiceNumber }).ToList();
        result.Data = invoiceList.ToList();

        return result;
      }

      var url = string.Empty;

      switch (invoice.BillingCode)
      {
        case (int)BillingCode.NonSampling:
          url = Url.Action("Create", "Invoice") + "?FromBillingHistory=true";
          break;
        case (int)BillingCode.SamplingFormDE:
          url = Url.Action("Create", "FormF") + "?FromBillingHistory=true";
          break;
        case (int)BillingCode.SamplingFormF:
          url = Url.Action("Create", "FormXF") + "?FromBillingHistory=true";
          break;
      }

      result.Data = new UIMessageDetail { Message = "Success", isRedirect = true, RedirectUrl = url };
      return result;
    }
  }
}
