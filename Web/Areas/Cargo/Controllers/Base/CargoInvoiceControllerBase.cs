
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Business.Common;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Web.Controllers.Base;
using Iata.IS.Web.UIModel.BillingHistory.Cargo;
using Iata.IS.Web.UIModel.BillingHistory.Pax;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using Iata.IS.Business;
using System;
using Iata.IS.Core.DI;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Cargo.Controllers.Base
{
  public abstract class CargoInvoiceControllerBase : InvoiceControllerBase<CargoInvoice>
  {
    private readonly ICargoInvoiceManager _invoiceManager;
    public IValidationErrorManager ValidationManager { get; set; }
    public IReferenceManager ReferenceManager { get; set; }

    private const string InvoiceVatGridAction = "InvoiceVatGrid";
    private const string AvailableVatGridAction = "AvailableVatGridData";
    private const string UnappliedAmountVatGridAction = "UnappliedAmountVatGridData";
    public const string SubTotalGridAction = "SubTotalGridData";

    protected CargoInvoiceControllerBase(ICargoInvoiceManager invoiceManager)
    {
      _invoiceManager = invoiceManager;
      
    }

    [HttpPost]
    public JsonResult InitiateRejection(string rejectedRecordIds, string invoiceId, int billingYear, int billingMonth, int billingPeriod, int smi, int rejectionTransactionType)
    {
        // check if transactions have been rejected in some rejection memo.

        var transactions = _invoiceManager.GetRejectedTransactionDetails(rejectedRecordIds);
        //CMP#624 : 2.10 - Change#6 : Time Limits
        // While calculating time limit for SMI X it should behave like SMI I.
        transactions.IsTransactionOutsideTimeLimit = !ReferenceManager.IsTransactionInTimeLimitMethodH((TransactionType)rejectionTransactionType, smi, billingYear, billingMonth, billingPeriod);



        // For display of warning message for - 1. Coupon/memo already rejected, 2. Rejection outside time limit.
        if ((transactions.Transactions != null && transactions.Transactions.Count > 0) || transactions.IsTransactionOutsideTimeLimit)
        {
            return Json(transactions);
        }

        return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
    }

    [HttpPost]
    public JsonResult InitiateDuplicateRejection(string rejectedRecordIds, string invoiceId)
    {
      return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
    }

    public JsonResult GetInvoicesForBillingHistory(string rejectedRecordIds, string invoiceId)
    {
      var invoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
        
      var billedMemberId = invoice.BillingMemberId;
      var billingMemberId = invoice.BilledMemberId;
      const int billingCode = 0; // Search for invoice. For credit-note, billing code is 4.

      var invoices = _invoiceManager.GetInvoicesForBillingHistory(billingCode, billedMemberId, SessionUtil.MemberId, invoice.SettlementMethodId);

      TempData[TempDataConstants.RejectedRecordIds] = string.Format("{0}@{1}", rejectedRecordIds, invoiceId);
      TempData[TempDataConstants.CorrespondenceNumber] = null;
      var result = new JsonResult();

      // If single open invoice found, redirect to rejection memo create page of that invoice.
      if (invoices.Count() == 1)
      {
        string redirectUrl = Url.Action("RMCreate", "Invoice", new { invoiceId = invoices.First().Id.Value(), area = "Cargo" });
            
        IList<CargoAuditTrailInvoice> invoiceList = new List<CargoAuditTrailInvoice> {
                                                   new CargoAuditTrailInvoice {
                                                       RedirectUrl = redirectUrl
                                                     }
                                                 };

        result.Data = invoiceList.ToList();

        return result;
     }

     if (invoices.Count() > 1)
     {
      IList<CargoAuditTrailInvoice> invoiceList = invoices.Select(paxInvoice => new CargoAuditTrailInvoice
      {
        Id = paxInvoice.Id,
        InvoiceNumber = paxInvoice.InvoiceNumber
      }).ToList();
      result.Data = invoiceList.ToList();

      return result;
     }

     var url = Url.Action("Create", "Invoice") + "?FromBillingHistory=true";

     result.Data = new UIMessageDetail
     {
      Message = "Success",
      isRedirect = true,
      RedirectUrl = url
    };

      return result;
    }

    protected override CargoInvoice GetInvoiceHeader(string invoiceNumber)
    {
      return _invoiceManager.GetInvoiceHeaderDetails(invoiceNumber);
    }
    /// <summary>
    /// Add member location info
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="invoice"></param>
    protected void MakeInvoiceRenderReady(Guid invoiceId, CargoInvoice invoice)
    {
      //invoice.LastUpdatedBy = SessionUtil.UserId;
      InitMemberLocationInfo(invoice);
    }
    //protected override CargoInvoice GetInvoiceHeader(string invoiceNumber)
    //{
    //  return _invoiceManager.GetInvoiceHeaderDetails(invoiceNumber);
    //}

    //protected override bool IsValidBillingCode(InvoiceBase invoice)
    //{
    //  bool isValidBillingCode = false;
    //  var paxInvoice = (PaxInvoice) invoice;
    //  if (paxInvoice != null)
    //  {
    //    if (paxInvoice.BillingCode == BillingCodeId)
    //    {
    //      if (paxInvoice.BillingCode == 0)
    //      {
    //        if (InvoiceType == paxInvoice.InvoiceType) {isValidBillingCode = true;}
    //      }
    //      else
    //      {
    //        isValidBillingCode = true;
    //      }
    //    }
    //  }

    //  return isValidBillingCode;
    //}

    //================================

    protected CargoInvoice ValidateInvoice(ICargoInvoiceManager manager, string invoiceId)
    {
      CargoInvoice invoice = null;

      try
      {
        // Validate Invoice
         invoice = manager.ValidateInvoice(invoiceId);

         switch (invoice.InvoiceStatus)
         {
             case InvoiceStatusType.ReadyForSubmission:
                 var message = string.Empty;
                 //SCP85837: PAX CGO Sequence Number
                 if (invoice.IsRecordSequenceArranged == RecordSequence.IsArranged)
                 {
                     message += " ¥ " + Messages.InvalidSequenceNoSerial;
                 }
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

    protected CargoInvoice SubmitInvoice(string invoiceId)
    {
        CargoInvoice invoice = null;

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
    [ValidateAntiForgeryToken]
    [HttpPost]
    public virtual ActionResult Submit(string invoiceId)
    {
        var submittedInvoice = SubmitInvoice(invoiceId);

        var controllerName = submittedInvoice.InvoiceType == InvoiceType.CreditNote ? "CreditNote" : "Invoice";

        return RedirectToAction(submittedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling ? "View" : "Edit", controllerName, new { invoiceId });

    }
    //===============new added ====================
    /// <summary>
    /// Invoice level VAT 
    /// </summary>
    /// <returns></returns>
    //public virtual ActionResult Vat(string invoiceId)
    //{
    //  return View(VatBase(invoiceId));
    //}

    //public virtual ActionResult VatView(string invoiceId)
    //{
    //  return View("Vat", VatBase(invoiceId));
    //  //return View("Edit", new { invoiceId = "" });
    //}

    //private CargoInvoice VatBase(string invoiceId)
    //{
    //  CargoInvoice invoice = new CargoInvoice();// _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
    //  // bool isGridViewOnly = false;

    //  //SetPageMode(invoice.InvoiceStatus);

    //  //if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    //  //{
    //  //    isGridViewOnly = true;
    //  //}

    //  //// Create grid instance for invoice vat 
    //  //var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);
    //  //ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

    //  //if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
    //  //{
    //  //    // Create grid instance for available vat 
    //  //    var availableVatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));
    //  //    ViewData[ViewDataConstants.AvailableVatGrid] = availableVatGrid.Instance;

    //  //    ////Create grid instance for vat not applied amount
    //  //    //var unappliedAmountVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
    //  //    //ViewData[ViewDataConstants.UnappliedAmountVatGrid] = unappliedAmountVatGrid.Instance;
    //  //}

    //  return invoice;
    //}

    ///// <summary>
    ///// Save Invoice level VAT 
    ///// </summary>
    ///// <returns></returns>
    //[HttpPost]
    //public virtual JsonResult Vat(FormCollection form)
    //{
    //    try
    //    {
    //        var vat = new JavaScriptSerializer().Deserialize(form[0], typeof(InvoiceVat));
    //        var record = vat as InvoiceVat;
    //        _invoiceManager.AddInvoiceLevelVat(record);

    //        var details = new UIMessageDetail
    //        {
    //            IsFailed = false,
    //            Message = Messages.RecordSaveSuccessful,
    //            isRedirect = true,
    //            RedirectUrl = Url.Action("Vat")

    //        };
    //        return Json(details);
    //    }
    //    catch (ISBusinessException businessException)
    //    {
    //        ShowErrorMessage(businessException.ErrorCode);

    //        var details = new UIMessageDetail
    //        {
    //            IsFailed = false,
    //            Message = string.Format(Messages.RecordSaveException, GetDisplayMessage(businessException.ErrorCode))
    //        };
    //        return Json(details);
    //    }
    //}

    ///// <summary>
    ///// Delete Invoice Vat Record
    ///// </summary>
    ///// <param name="transactionId">Transaction Id</param>
    ///// <returns></returns>
    //public virtual JsonResult VatDelete(string transactionId)
    //{
    //    UIMessageDetail details;
    //    try
    //    {
    //        //Delete record
    //        bool isDeleted = _invoiceManager.DeleteInvoiceLevelVat(transactionId);

    //        details = GetDeleteMessage(isDeleted);
    //    }
    //    catch (ISBusinessException ex)
    //    {
    //        details = HandleDeleteException(ex.ErrorCode);
    //    }

    //    return Json(details);
    //}

    ///// <summary>
    ///// Vat Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public JsonResult InvoiceVatGrid(string invoiceId, bool isGridViewOnly)
    //{
    //    //Create grid instance and retrieve data from database
    //    var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);

    //    var vatData = _invoiceManager.GetInvoiceLevelVatList(invoiceId).AsQueryable();
    //    return invoiceVatGrid.DataBind(vatData);
    //}

    ///// <summary>
    ///// Available Vat Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public JsonResult AvailableVatGridData(string invoiceId)
    //{
    //    //Create grid instance and retrieve data from database
    //    var vatGrid = new AvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));

    //    var vatData = _invoiceManager.GetInvoiceLevelDerivedVatList(invoiceId).AsQueryable();
    //    int count = 1;
    //    foreach (var derivedVatDetails in vatData)
    //    {
    //        derivedVatDetails.RowNumber = count++;
    //    }
    //    return vatGrid.DataBind(vatData);
    //}

    ///// <summary>
    ///// Used to instantiate SourceCode vat Total grid
    ///// </summary>
    ///// <returns>null</returns>
    //public JsonResult AvailableEmptySourceCodeVatTotalGridData()
    //{
    //    return null;
    //}

    ///// <summary>
    /////Unapplied Vat amount Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public JsonResult UnappliedAmountVatGridData(string invoiceId)
    //{
    //    //Create grid instance and retrieve data from database
    //    var unappliedVatGrid = new UnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
    //    var notAppliedVatList = _invoiceManager.GetNonAppliedVatList(invoiceId).AsQueryable();
    //    int count = 1;
    //    foreach (var nonAppliedVatDetails in notAppliedVatList)
    //    {
    //        nonAppliedVatDetails.RowNumber = count++;
    //    }
    //    return unappliedVatGrid.DataBind(notAppliedVatList);
    //}

    //protected PaxInvoice ValidateInvoice(IInvoiceManager manager, string invoiceId)
    //{
    //    PaxInvoice invoice = null;

    //    try
    //    {
    //        // Validate Invoice
    //        invoice = manager.ValidateInvoice(invoiceId);

    //        switch (invoice.InvoiceStatus)
    //        {
    //            case InvoiceStatusType.ReadyForSubmission:
    //                ShowSuccessMessage(string.Format(Messages.InvoiceValidateSuccessful, invoice.InvoiceNumber));
    //                break;

    //            default:
    //                ShowErrorMessage(string.Format(Messages.InvoiceValidateFailed, invoice.InvoiceNumber), true);
    //                break;
    //        }
    //    }
    //    catch (ISBusinessException exception)
    //    {
    //        ShowErrorMessage(exception.ErrorCode, true);
    //    }

    //    return invoice;
    //}

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

    public JsonResult SubTotalGridData(string invoiceId)
    {
      var awbCodeGrid = new AwbCodeGrid(ControlIdConstants.SubTotalGridId, Url.Action(SubTotalGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);

      var subTotalist = _invoiceManager.GetSubTotalList(invoiceId).AsQueryable();

      return awbCodeGrid.DataBind(subTotalist.AsQueryable());
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
      return View("Vat", VatBase(invoiceId));
    }

    private CargoInvoice VatBase(string invoiceId)
    {
      CargoInvoice invoice = _invoiceManager.GetInvoiceHeaderDetails(invoiceId);
      bool isGridViewOnly = false;

      SetPageMode(invoice.InvoiceStatus);

      if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
      {
        isGridViewOnly = true;
      }

      // Create grid instance for invoice vat 
      var invoiceVatGrid = new VatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);
      ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

      if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
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

    // Called on Edit Invoice header page 
    public JsonResult GetSubmittedErrors(string invoiceId)
    {
      var submittedErrors = ValidationManager.GetValidationErrors(invoiceId);
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      return submittedErrorsGrid.DataBind(submittedErrors);
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
        var vat = new JavaScriptSerializer().Deserialize(form[0], typeof(CargoInvoiceTotalVat));
        var record = vat as CargoInvoiceTotalVat;
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
    [RestrictInvoiceUpdate(TransactionParamName = "id", IsJson = true, TableName = TransactionTypeTable.CGO_INVOICE_TOTAL_VAT)]
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
   
  }
}
