using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel.BillingHistory.Cargo;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.Util;
using Iata.IS.Business.Cargo;
using Iata.IS.Core.Exceptions;
using log4net;
using System.Reflection;
using Iata.IS.Web.Util.Filters;

using System.Web.Script.Serialization;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.Cargo.Common;

using Iata.IS.Web.Areas.Cargo.Controllers.Base;
using BillingCode = Iata.IS.Model.Cargo.Enums.BillingCode;
using RejectionMemo = Iata.IS.Model.Cargo.CargoRejectionMemo;
using SubmissionMethod = Iata.IS.Model.Cargo.Enums.SubmissionMethod;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.AdminSystem;
using Trirand.Web.Mvc;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using System.Data;

//using TransactionType = Iata.IS.Model.Cargo.Enums.TransactionType;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class InvoiceController : CargoInvoiceControllerBase
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    //private readonly IReferenceManager _referenceManager;
    // public ICargoInvoiceManager CargoInvoiceManager { get; set; }
    private readonly ICargoInvoiceManager _cargoInvoiceManager;
    private readonly IReferenceManager _referenceManager;
    private const string PrepaidBillingGridAction = "PrepaidBillingGridData";
    private const string SubTotalGridAction = "SubTotalGridData";
    private const string BillingMemoGridAction = "BillingMemoGridData";
    private const string InvoiceVatGridAction = "InvoiceVatGrid";
    private const string AvailableVatGridAction = "AvailableVatGridData";
    private const string UnappliedAmountVatGridAction = "UnappliedAmountVatGridData";
    public InvoiceController(ICargoInvoiceManager cargoInvoiceManager, IReferenceManager referenceManager, IMemberManager memberManager)
      : base(cargoInvoiceManager)
    {

      _referenceManager = referenceManager;
      MemberManager = memberManager;
      _cargoInvoiceManager = cargoInvoiceManager;
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult Create()
    {
      string billedMemberText = string.Empty;
      int billedMemberId = 0;
      Member billedMember = null;
      /* CMP #624: ICH Rewrite-New SMI X
        * Description: SMi X should be preselected, when user control is here from billing history screen.  */
      var previousInvoiceSMI = -1;

      if (!(Request.QueryString.AllKeys.Contains(TempDataConstants.FromBillingHistory) && Request.QueryString.Get(TempDataConstants.FromBillingHistory) == "true"))
      {
        SessionUtil.CGOCorrSearchCriteria = SessionUtil.CGOInvoiceSearchCriteria = SessionUtil.InvoiceSearchCriteria = null;
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

            var inv = _cargoInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
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
          var correspondenceManager = Ioc.Resolve<ICargoCorrespondenceManager>(typeof(ICargoCorrespondenceManager));
          var correspondence = correspondenceManager.GetRecentCorrespondenceDetails(Convert.ToInt64(correspondenceRefNumber));
          if (correspondence != null && correspondence.InvoiceId != Guid.Empty)
          {
            var inv = _cargoInvoiceManager.GetInvoiceHeaderDetails(correspondence.InvoiceId.Value());
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


      var invoice = new CargoInvoice
      {
        BilledMemberId = Convert.ToInt32(billedMemberId),
        BilledMemberText = billedMemberText,
        BilledMember = billedMember,
        BillingMemberText = SessionUtil.MemberName,
        InvoiceDate = DateTime.UtcNow,
        InvoiceTypeId = (int)Model.Enums.InvoiceType.Invoice,
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

   
    [ValidateAntiForgeryToken]
    [HttpPost]
    public override ActionResult Submit(string invoiceId)
    {
        return base.Submit(invoiceId);
    }
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    public ActionResult Create(CargoInvoice invoice)
    {
        //try
        //{
        //  CargoInvoice obj = new CargoInvoice();
        //  obj.BillingMonth = 12;
        //  obj.BillingYear = 2011;
        //  obj.InvoiceStatus = InvoiceStatusType.Open;
        //  ViewData[ViewDataConstants.PageMode] = PageMode.View;
        //  return RedirectToAction("Edit", new { invoiceId = "" });
        //}
        //catch (Exception ex)
        //{

        //}
        //return RedirectToAction("Edit", new { invoiceId = "02f041ab-fce6-1bf6-e040-007f01003755" });
        //return View(obj);
        try
        {
            invoice.BillingMemberId = SessionUtil.MemberId;
            invoice.InvoiceTypeId = (int) Model.Enums.InvoiceType.Invoice;
            //invoice.BillingCode = Convert.ToInt32(BillingCode..NonSampling);
            invoice.InvoiceDate = DateTime.UtcNow;
            invoice.SubmissionMethodId = (int) SubmissionMethod.IsWeb;
            
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
            if (((TempData.ContainsKey(TempDataConstants.RejectedRecordIds) &&
                  TempData[TempDataConstants.RejectedRecordIds] != null)
                 ||
                 (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber) &&
                  TempData[TempDataConstants.CorrespondenceNumber] != null)
                ) &&
                (TempData.ContainsKey(TempDataConstants.PreviousInvoiceSMI) &&
                 TempData[TempDataConstants.PreviousInvoiceSMI] != null)
                )
            {
                if (TempData[TempDataConstants.PreviousInvoiceSMI].ToString() ==
                    ((int) SMI.IchSpecialAgreement).ToString() &&
                    invoice.SettlementMethodId != (int) SMI.IchSpecialAgreement)
                {
                    /* Old is X new is not */
                    throw new ISBusinessException(CargoErrorCodes.RejInvBHLinkingCheckOldInvSmiX);
                }
                else if (TempData[TempDataConstants.PreviousInvoiceSMI].ToString() !=
                         ((int) SMI.IchSpecialAgreement).ToString() &&
                         invoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
                {
                    /* Old is not X but new is X */
                    throw new ISBusinessException(CargoErrorCodes.RejInvBHLinkingCheckOldInvSmiNotX);
                }
            }

            invoice = _cargoInvoiceManager.CreateInvoice(invoice);

            ShowSuccessMessage(Messages.InvoiceCreateSuccessful);

            //  KeepBillingHistoryDataInStore(true);

            //Initiate rejection memo from billing history screen
            if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) &&
                TempData[TempDataConstants.RejectedRecordIds] != null)
            {
                return RedirectToAction("RMCreate", new {invoiceId = invoice.Id.Value()});
            }

            // Initiate billing memo from billing history screen
            if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber))
            {
                //  return RedirectToAction("BMCreate", new { invoiceId = invoice.Id.Value() });
            }

            return RedirectToAction("Edit", new {invoiceId = invoice.Id.Value()});
        }
        catch (ISBusinessException exception)
        {
            /* CMP #624: ICH Rewrite-New SMI X 
              * Description: As per ICH Web Service Response Message specifications 
              * Refer FRS Section 3.3 Table 9. 
              * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

            var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
            var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

            invoice.BilledMember = _cargoInvoiceManager.GetBilledMember(invoice.BilledMemberId);

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
                var billingMemberLocationInfo =
                    invoice.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
                if (billingMemberLocationInfo != null && !string.IsNullOrEmpty(billingMemberLocationInfo.LegalText))
                {
                    ViewData["IsLegalTextSet"] = true;
                }
            }
            KeepBillingHistoryDataInStore(true);
        }

        MakeInvoiceRenderReady(invoice.Id, invoice);
        return View(invoice);
    }


      //[RestrictUnauthorizedUpdate]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    public ActionResult Edit(string invoiceId)
    {
      var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      ViewData[ViewDataConstants.TransactionExists] = _cargoInvoiceManager.IsTransactionExists(invoiceId);

      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      // Clear rejection memo data stored while navigating from Billing history.
      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        TempData.Remove(TempDataConstants.RejectedRecordIds);
      }

      // Currently, not all invoices has corresponding InvoiceTotal record entries 
      // in database, hence creating empty object of InvoiceTotal object.
      if (InvoiceHeader.CGOInvoiceTotal == null)
      {
        InvoiceHeader.CGOInvoiceTotal = new CargoInvoiceTotal();
      }

      // Create Source Code grid instance
      var sourceCodeGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SubTotalGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
      ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;

      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
      {
        // Get all submitted errors.
        var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
        submittedErrorsGrid.Instance.ToolBarSettings.ToolBarPosition = ToolBarPosition.Bottom;
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

      //KeepBillingHistoryDataInStore(ViewData[ViewDataConstants.FromBillingHistory] != null ? (bool)ViewData[ViewDataConstants.FromBillingHistory] : false);
      return View(InvoiceHeader);
    }

    /* public JsonResult SubTotalGridData(string invoiceId)
    {
     // ViewData[ViewDataConstants.BillingType] = "Receivable";
      var awbCodeGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SubTotalGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);

     // List<BillingCodeSubTotal> tt = new List<BillingCodeSubTotal>();
      var subTotalist = _cargoInvoiceManager.GetSubTotalList(invoiceId).AsQueryable();

      return awbCodeGrid.DataBind(subTotalist.AsQueryable());
    } */

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult Edit(string invoiceId, CargoInvoice invoice)
    {
        try
        {
            invoice.Id = invoiceId.ToGuid();
            invoice.InvoiceType = InvoiceType.Invoice;
            invoice.InvoiceStatus = InvoiceStatusType.Open;
            invoice.BillingMemberId = SessionUtil.MemberId;
            invoice.InvoiceDate = DateTime.UtcNow;
            invoice.SubmissionMethodId = (int)SubmissionMethod.IsWeb;
            invoice.LastUpdatedBy = SessionUtil.UserId;
            invoice = _cargoInvoiceManager.UpdateInvoice(invoice);

            ShowSuccessMessage(Messages.InvoiceUpdateSuccessful);
            //test below code
            ViewData[ViewDataConstants.PageMode] = PageMode.View;
            //--------------
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

            invoice.BilledMember = _cargoInvoiceManager.GetBilledMember(invoice.BilledMemberId);

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
                var billingMemberLocationInfo =
                    invoice.MemberLocationInformation.Find(locInfo => locInfo.IsBillingMember);
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
            invoice.CGOInvoiceTotal = new CargoInvoiceTotal();
        }
        var sourceCodeGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId,
                                             Url.Action(SubTotalGridAction, new {invoiceId}),
                                             (ViewData[ViewDataConstants.BillingType].ToString() ==
                                              Util.BillingType.Payables)
                                                 ? true
                                                 : false);
        ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
        SetViewDataPageMode(PageMode.Edit);

        return View(invoice);
    }

      /// <summary>
    /// Displays invoice header and screen for AWB billing creation.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [OutputCache(CacheProfile = "donotCache")]
    [HttpGet]
    public ActionResult AwbPrepaidBillingCreate(string invoiceId)
    {
      // Retrieve Batch and Sequence number which will be pre populated, for AWB Prepaid
      int batchNumber;
      int sequenceNumber;
      _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.AWBPrepaid, out batchNumber, out sequenceNumber);

      SetViewDataPageMode(PageMode.Create);

      if (InvoiceHeader.CGOInvoiceTotal == null)
      {
        InvoiceHeader.CGOInvoiceTotal = new CargoInvoiceTotal();
      }
      var awbPrepaidRecord = new AwbRecord
      {
        Invoice = InvoiceHeader,
        InvoiceId = invoiceId.ToGuid(),

        LastUpdatedBy = SessionUtil.UserId,
        BatchSequenceNumber = batchNumber,
        RecordSequenceWithinBatch = sequenceNumber
       // CheckDigit = 9
      };


      // Set ViewData, "IsPostback" to false
      ViewData[ViewDataConstants.IsPostback] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData[TempDataConstants.AwbPrepaidRecord] != null)
      {
        // Set Viewdata
        ViewData[ViewDataConstants.AwbPrepaidRecord] = TempData[TempDataConstants.AwbPrepaidRecord];
      }


      SetViewDataPageMode(PageMode.Create);

      ViewData[ViewDataConstants.IsPostback] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData[TempDataConstants.AwbPrepaidRecord] != null)
      {
        // Set Viewdata
        ViewData[ViewDataConstants.AwbPrepaidRecord] = TempData[TempDataConstants.AwbPrepaidRecord];
      }

      return View(awbPrepaidRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult AwbChargeCollectBillingCreate(string invoiceId)
    {
      // Retrieve Batch and Sequence number which will be pre populated, for AWB Charge Collect
      int batchNumber;
      int sequenceNumber;
      _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.AWBChargeCollect, out batchNumber, out sequenceNumber);

      SetViewDataPageMode(PageMode.Create);

      if (InvoiceHeader.CGOInvoiceTotal == null)
      {
        InvoiceHeader.CGOInvoiceTotal = new CargoInvoiceTotal();
      }
      var awbChargeCollectRecord = new AwbRecord
      {
        Invoice = InvoiceHeader,
        InvoiceId = invoiceId.ToGuid(),
        LastUpdatedBy = SessionUtil.UserId,
        BatchSequenceNumber = batchNumber,
        RecordSequenceWithinBatch = sequenceNumber
        // CheckDigit = 9
      };


      // Set ViewData, "IsPostback" to false
      ViewData[ViewDataConstants.IsPostback] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData[TempDataConstants.AwbChargeCollectRecord] != null)
      {
        // Set Viewdata
        ViewData[ViewDataConstants.AwbChargeCollectRecord] = TempData[TempDataConstants.AwbChargeCollectRecord];
      }


      SetViewDataPageMode(PageMode.Create);

      ViewData[ViewDataConstants.IsPostback] = false;

      // If action is 'Save and Add New' then populate the previous source code, batch number and sequence no+1
      if (TempData[TempDataConstants.AwbChargeCollectRecord] != null)
      {
        // Set Viewdata
        ViewData[ViewDataConstants.AwbChargeCollectRecord] = TempData[TempDataConstants.AwbChargeCollectRecord];
      }

      return View(awbChargeCollectRecord);
    }
    /// <summary>
    /// Creates AWB billing coupon and redirect to the GET version of this action.
    /// </summary>

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult AwbPrepaidBillingCreate(string invoiceId, AwbRecord awbRecord)
    {
        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                                    BillingCategorys.Cargo.ToString(), "Stage 1:AwbPrepaidBillingCreate Start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();

      log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                                 BillingCategorys.Cargo.ToString(), "Stage 2: Attachment id's selected ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      if (awbRecord.CarriageFromId!=null)
      awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
      awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
      awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
      awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();


      try
      {
          log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                                 BillingCategorys.Cargo.ToString(), "Stage 3: ValidateMandatoryFieldForAwbPrepaidBilling Start", SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);

        // Validate Server side mandatory field for Awb Prepaid billing record
        ValidateMandatoryFieldForAwbPrepaidBilling(awbRecord);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                              BillingCategorys.Cargo.ToString(), "Stage 3: ValidateMandatoryFieldForAwbPrepaidBilling Completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        string duplicateCouponErrorMessage;

        awbRecord.Attachments.Clear();
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        awbRecord.BillingCodeId = (int)BillingCode.AWBPrepaid;
        _cargoInvoiceManager.AddAwbRecord(awbRecord, out duplicateCouponErrorMessage);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                            BillingCategorys.Cargo.ToString(), "Stage 4: AddAwbRecord Completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        _cargoInvoiceManager.UpdateAwbAttachment(couponAttachmentIds, awbRecord.Id);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                          BillingCategorys.Cargo.ToString(), "Stage 5: UpdateAwbAttachment Completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);


        //  _cargoInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, awbRecord.Id);
        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateCouponErrorMessage);
        ShowSuccessMessage(Messages.AwbRecordCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        TempData[TempDataConstants.AwbPrepaidRecord] = string.Format(@"{0}-{1}", awbRecord.BatchSequenceNumber, recSeqNum);
        // Increment sequence no by 1
       // ModelState.SetModelValue("RecordSequenceWithinBatch",
     //                            new ValueProviderResult(awbRecord.RecordSequenceWithinBatch + 1, (awbRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreate ", this.ToString(),
                       BillingCategorys.Cargo.ToString(), "Stage 6: AwbPrepaidBillingCreate Completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
        return RedirectToAction("AwbPrepaidBillingCreate", new { invoiceId });
        //return RedirectToAction("Create");
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(awbRecord);
      //try
      //{

      //  return RedirectToAction("AwbPrepaidBillingCreate", new { invoiceId });
      //}
      //catch (ISBusinessException exception)
      //{
      //  // Set ViewData, "IsPostback" to true, if exception occurs
      //  ViewData[ViewDataConstants.IsPostback] = true;
      //  ShowErrorMessage(exception.ErrorCode);

      //}
      //return View(awbRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult AwbChargeCollectBillingCreate(string invoiceId, AwbRecord awbRecord)
    {

      #region "commented"
      //try
      //{
      //  #region
      //  // string duplicateCouponErrorMessage;

      //  //couponRecord.Attachments.Clear();
      //  //couponRecord.LastUpdatedBy = SessionUtil.UserId;
      //  //_nonSamplingInvoiceManager.AddCouponRecord(couponRecord, out duplicateCouponErrorMessage);
      //  //_nonSamplingInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, couponRecord.Id);
      //  //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateCouponErrorMessage);
      //  //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful);
      //  //if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
      //  //    ShowErrorMessage(duplicateCouponErrorMessage, true);

      //  // TempData[TempDataConstants.PrimeCouponRecord] = string.Format(@"{0}-{1}-{2}", couponRecord.SourceCodeId, couponRecord.BatchSequenceNumber, couponRecord.RecordSequenceWithinBatch + 1);

      //  return RedirectToAction("AwbChargeCollectBillingCreate", new { invoiceId });
      //  #endregion
      //}
      //catch (ISBusinessException exception)
      //{
      //  // Set ViewData, "IsPostback" to true, if exception occurs
      //  ViewData[ViewDataConstants.IsPostback] = true;
      //  ShowErrorMessage(exception.ErrorCode);

      //}
      //return View(awbRecord);
#endregion
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      if (awbRecord.CarriageFromId!=null)
      awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
      awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
      awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
      awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();


      try
      {
        string duplicateCouponErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 10 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        awbRecord.Attachments.Clear();
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        awbRecord.BillingCodeId = (int)BillingCode.AWBChargeCollect;
        _cargoInvoiceManager.AddAwbRecord(awbRecord, out duplicateCouponErrorMessage);
        _cargoInvoiceManager.UpdateAwbAttachment(couponAttachmentIds, awbRecord.Id);
        //  _cargoInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, awbRecord.Id);
        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateCouponErrorMessage);
        ShowSuccessMessage(Messages.AwbRecordCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        TempData[TempDataConstants.AwbChargeCollectRecord] = string.Format(@"{0}-{1}", awbRecord.BatchSequenceNumber, recSeqNum);

        return RedirectToAction("AwbChargeCollectBillingCreate", new { invoiceId });
        //return RedirectToAction("Create");
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(awbRecord);
    }

    /// <summary>
    /// Upload prime billing Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult AwbBillingAttachmentUpload(string invoiceId, string transactionId)
    {
      #region""
      string files = string.Empty;
      var attachments = new List<AwbAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method AwbBillingAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
          CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceDetails(invoiceId);
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
          Logger.Info("fileUploadHelper is created.");
          // On Prime Billing Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoInvoiceManager.IsDuplicateAwbAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
          {
            throw new ISBusinessException(Messages.FileDuplicateError);
          }
          Logger.Info("Successfully checked for duplication file attachment");
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
            Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new AwbAttachment
            {
              Id = fileUploadHelper.FileServerName,
              OriginalFileName = fileUploadHelper.FileOriginalName,
              FileSize = fileUploadHelper.FileToSave.ContentLength,
              LastUpdatedBy = SessionUtil.UserId,
              ServerId = fileUploadHelper.FileServerInfo.ServerId,
              FileStatus = FileStatusType.Received,
              FilePath = fileUploadHelper.FileRelativePath
            };


            attachment = _cargoInvoiceManager.AddAwbAttachment(attachment);
            Logger.Info("Attachment Entry is inserted  in database successfully");
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            // assign user info from session and file server info.
            if (attachment.UploadedBy==null)
              {
                attachment.UploadedBy=new User(); 
              }
              attachment.UploadedBy.Id = SessionUtil.UserId;
             attachment.UploadedBy.FirstName = SessionUtil.Username;
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
      }
      catch (Exception ex)
      {
        message = Messages.FileUploadUnexpectedError;
        Logger.Error("Exception", ex);
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }
      #endregion

      // return new FileUploadJsonResult { Data = new { } };
      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Creates AWB billing and allows to duplicate same record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbPrepaidBillingCreate")]
    public ActionResult AwbPrepaidBillingDuplicate(string invoiceId, AwbRecord awbRecord)
    {

      #region""
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      if (awbRecord.CarriageFromId!=null)
      awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
      awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
      awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
      awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      awbRecord.BillingCodeId = (int)BillingCode.AWBPrepaid;

      try
      {
        // Validate Server side mandatory field for Awb Prepaid billing record
        ValidateMandatoryFieldForAwbPrepaidBilling(awbRecord);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        string duplicateErrorMessage;

        awbRecord.Attachments.Clear();

        _cargoInvoiceManager.AddAwbRecord(awbRecord, out duplicateErrorMessage);
        _cargoInvoiceManager.UpdateAwbAttachment(couponAttachmentIds, awbRecord.Id);
        awbRecord.AwbIssueingAirline = string.Empty;
        awbRecord.AwbSerialNumber =0;
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);        
        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull, false);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);
        ViewData["FromClone"] = true;
        SetViewDataPageMode(PageMode.Clone);
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        // Increment sequence no by 1
        ModelState.SetModelValue("RecordSequenceWithinBatch",
                                 new ValueProviderResult(recSeqNum, (awbRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));
        //Initialize CheckDigit to 9
        // ModelState.SetModelValue("CheckDigit", new ValueProviderResult(9, (awbRecord.CheckDigit).ToString(), CultureInfo.InvariantCulture));

        awbRecord.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
        
        awbRecord.Id = Guid.Empty;

        // Retrieve Batch and Sequence number which will be pre populated, for AWB Prepaid
        int batchNumber;
        int sequenceNumber;
        _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.AWBPrepaid, out batchNumber, out sequenceNumber);
        awbRecord.BatchSequenceNumber = batchNumber;
        awbRecord.RecordSequenceWithinBatch = sequenceNumber;
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
        //SCP164395:CGO - IS Web response stats . Moved the below code into catch section. Required in case of exception only
        // also check if invoice data is already populated or not 
        if (awbRecord.Invoice == null)
        {
            awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        }
      }
        
     

      #endregion
      return View("AwbPrepaidBillingCreate", awbRecord);
    }

    [ValidateAntiForgeryToken]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbPrepaidRecordEdit")]
    public ActionResult AwbPrepaidBillingClone(string invoiceId, string transactionId, AwbRecord awbRecord)
    {
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      awbRecord.BillingCodeId = (int)BillingCode.AWBPrepaid;
      awbRecord.Id = transactionId.ToGuid();

      //foreach (var tax in awbRecord.TaxBreakdown)
      //{
      //  tax.ParentId = awbRecord.Id;
      //}

      foreach (var vat in awbRecord.VatBreakdown)
      {
        vat.ParentId = awbRecord.Id;
      }

      try
      {
        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 9 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        _cargoInvoiceManager.UpdateAwbRecord(awbRecord, out duplicateErrorMessage);

        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        awbRecord.AwbIssueingAirline = string.Empty;
        awbRecord.AwbSerialNumber = 0;
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage, false);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful, false);
        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull, false);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        awbRecord.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        // Increment sequence no by 1
        ModelState.SetModelValue("RecordSequenceWithinBatch",
                                 new ValueProviderResult(recSeqNum, (awbRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));
        //Initialize CheckDigit to 9
        //ModelState.SetModelValue("CheckDigit", new ValueProviderResult(9, (couponRecord.CheckDigit).ToString(), CultureInfo.InvariantCulture));
        ViewData["FromClone"] = true;
        awbRecord.Id = Guid.Empty;

        // Retrieve Batch and Sequence number which will be pre populated, for AWB Prepaid
        int batchNumber;
        int sequenceNumber;
        _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.AWBPrepaid, out batchNumber, out sequenceNumber);
        awbRecord.BatchSequenceNumber = batchNumber;
        awbRecord.RecordSequenceWithinBatch = sequenceNumber;

        return View("AwbPrepaidBillingCreate", awbRecord);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("AwbPrepaidRecordEdit", awbRecord);
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult RMCreate(string invoiceId)
    {
      // Retrieve Batch and Sequence number which will be pre populated, for Rejection Memo
      int batchNumber;
      int sequenceNumber;
      _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.RejectionMemo, out batchNumber, out sequenceNumber);

      SetViewDataPageMode(PageMode.Create);
      var rejectionMemoRecord = new RejectionMemo { Invoice = InvoiceHeader, InvoiceId = invoiceId.ToGuid(), LastUpdatedBy = SessionUtil.UserId, BatchSequenceNumber = batchNumber, RecordSequenceWithinBatch = sequenceNumber};

      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        string originalRejectedRecordIds = null;
        if (TempData[TempDataConstants.RejectedRecordIds] != null)
        {
          originalRejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();
        }
        
        var rejectedRecordIds = originalRejectedRecordIds;

        if (rejectedRecordIds != null && rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf('@') + 1)
        {
          var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
          rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf('@'));
          if (rejectedRecordIds.Length > rejectedRecordIds.LastIndexOf(';') + 1)
          {
            var transactionType = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf(';') + 1);
            rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf(';'));
            var rejectionIdList = rejectedRecordIds.Split(',');

            var rejectedInvoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
            ViewData[ViewDataConstants.FromBillingHistory] = true;
            if (rejectedInvoice != null)
            {
              rejectionMemoRecord.YourInvoiceNumber = rejectedInvoice.InvoiceNumber;
              rejectionMemoRecord.YourInvoiceBillingYear = rejectedInvoice.BillingYear;
              rejectionMemoRecord.YourInvoiceBillingMonth = rejectedInvoice.BillingMonth;
              rejectionMemoRecord.YourInvoiceBillingPeriod = rejectedInvoice.BillingPeriod;
              rejectionMemoRecord.IsLinkingSuccessful = true;

              // transaction type being rejected
              switch (transactionType)
              {
                case "AWB Prepaid":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.None;
                  break;
                case "AWB Charge Collect":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.None;
                  break;
                case "AWB":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.None;
                  break;
                case "Billing Memo":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.BMNumber;
                  break;
                case "BM":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.BMNumber;
                  break;
                case "Credit Memo":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.CMNumber;
                  break;
                case "CM":
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.CMNumber;
                  break;
                default:
                  rejectionMemoRecord.BMCMIndicatorId = (int)BMCMIndicator.None;
                  break;
              }
            }

            if (transactionType == "AWB Prepaid" || transactionType == "AWB Charge Collect" || transactionType == "AWB")
            {
              rejectionMemoRecord.RejectionStage = 1;
            }
            else if (transactionType == "Billing Memo" || transactionType == "BM")
            {
              rejectionMemoRecord.RejectionStage = 1;
              var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(rejectionIdList[0]);
              rejectionMemoRecord.YourBillingMemoNumber = billingMemo.BillingMemoNumber;
              rejectionMemoRecord.BMCMIndicator = BMCMIndicator.BMNumber;

                // CMP#650 : Autopopulate reason code
                if(billingMemo.ReasonCode.ToUpper().Equals("2C"))
                {
                    rejectionMemoRecord.ReasonCode = "16";
                }
                else if (billingMemo.ReasonCode.ToUpper().Equals("2Z"))
                {
                    rejectionMemoRecord.ReasonCode = "17";
                }
                
            }
            else if (transactionType == "Credit Memo" || transactionType == "CM")
            {
              rejectionMemoRecord.RejectionStage = 1;
              var creditNoteManager = Ioc.Resolve<ICargoCreditNoteManager>(typeof(ICargoCreditNoteManager));
              var creditMemo = creditNoteManager.GetCreditMemoRecordDetails(rejectionIdList[0]);
              rejectionMemoRecord.YourBillingMemoNumber = creditMemo.CreditMemoNumber;
              rejectionMemoRecord.BMCMIndicator = BMCMIndicator.CMNumber;
            }
            else if (transactionType == "Rejection Memo" || transactionType == "RM") 
            {
              var rejectedMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(rejectionIdList[0]);
              if (rejectedMemo != null)
              {
                rejectionMemoRecord.RejectionStage = ++rejectedMemo.RejectionStage;
                rejectionMemoRecord.YourRejectionNumber = rejectedMemo.RejectionMemoNumber;
                if (rejectedMemo.BMCMIndicator == BMCMIndicator.BMNumber || rejectedMemo.BMCMIndicator == BMCMIndicator.CMNumber)
                {
                  rejectionMemoRecord.BMCMIndicatorId = rejectedMemo.BMCMIndicatorId;
                  rejectionMemoRecord.YourBillingMemoNumber = rejectedMemo.YourBillingMemoNumber;

                  // CMP#650 : Autopopulate reason code
                  if (rejectedMemo.ReasonCode.ToUpper().Equals("16"))
                  {
                      rejectionMemoRecord.ReasonCode = "16";
                  }
                  else if (rejectedMemo.ReasonCode.ToUpper().Equals("17"))
                  {
                      rejectionMemoRecord.ReasonCode = "17";
                  }
                    
                }
              }
            }
          }
        }
        TempData[TempDataConstants.RejectedRecordIds] = originalRejectedRecordIds;
      }

      return View(rejectionMemoRecord);
    }

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

    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMCreate(string invoiceId, RejectionMemo record)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      record.AttachmentIndicatorOriginal = record.Attachments.Count() > 0;

      var attachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP

          if (
              !IsYourBillingDateValid(record.YourInvoiceBillingYear, record.YourInvoiceBillingMonth,
                                      record.YourInvoiceBillingPeriod))
          {
              throw new ISBusinessException("Invalid Your Invoice Billing Date.", "");
          }

          #endregion

          record.Attachments.Clear();

          string errorMessage, warningMessage;
          var rmLinkingDetails = SetRejectionMemoLinkingDetails(record, invoiceId);
          record.LastUpdatedBy = SessionUtil.UserId;
          record.BillingCode = (int) BillingCode.RejectionMemo;

          //SCPID : 105938 - RE: IATA- SIS Bug
          string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(record);
          if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage);

          // CMP#650 set reason code coming from linking

          if(rmLinkingDetails != null && !string.IsNullOrEmpty(rmLinkingDetails.ReasonCode))
          {
              record.YourReasonCode = rmLinkingDetails.ReasonCode;
          }

          _cargoInvoiceManager.AddRejectionMemoRecord(record, out errorMessage, out warningMessage);

          _cargoInvoiceManager.UpdateRejectionMemoAttachment(attachmentIds, record.Id);

          GetRejectAwbListFromBillingHistory(record);

          //TempData.Clear();
          ShowSuccessMessage(string.Format("{0}{1}{2}", errorMessage, Environment.NewLine, Messages.RMCreateSuccessful));
          if (!String.IsNullOrEmpty(warningMessage))
              ShowErrorMessage(warningMessage, true);
          // On clicking Save in case of adding  a new rejection record, 
          // if the selected reason code mandates coupon breakdown, then system should automatically open the RM Coupon Breakdown screen for data capture.
          // Also if the user came from billing history then no need to go to Coupon create page.
          return RedirectToAction("RMEdit", new {invoiceId, transactionId = record.Id.Value()});
      }
      catch (ISBusinessException exception)
      {
          ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
          record.Attachments = _cargoInvoiceManager.GetRejectionMemoAttachments(attachmentIds);
          ViewData[ViewDataConstants.IsPostback] = true;
      }

      // Set all details of rejection memo if it is from billing history.
      SetRejectionMemoDetailsForBillingHistory(record);

      KeepBillingHistoryDataInStore(true);

      record.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      return View(record);
    }

    [HttpGet]
    public ActionResult RMEdit(string invoiceId, string transactionId)
    {
      var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

      if (_cargoInvoiceManager.GetRejectionMemoAwbCount(transactionId) != 0)
      {
        ViewData[ViewDataConstants.BreakdownExists] = true;
      }
      else
      {
        ViewData[ViewDataConstants.BreakdownExists] = false;
      }

      var rejectionMemoAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.RejectionMemoAwbGrid] = rejectionMemoAwbGrid.Instance;

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
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMEdit")]
    public ActionResult RMEditAndAddNew(string invoiceId, string transactionId, RejectionMemo rejectionMemoRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rejectionMemoRecord.AttachmentIndicatorOriginal = rejectionMemoRecord.Attachments.Count() > 0;

      try
      {

          #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
          if (!IsYourBillingDateValid(rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingPeriod))
          {
            throw new ISBusinessException("Invalid Your Invoice Billing Date.","");
          }
          #endregion

          //SCPID : 105938 - RE: IATA- SIS Bug
          string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(rejectionMemoRecord);
          if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage);
          
          rejectionMemoRecord.Id = transactionId.ToGuid();
        rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

        //SCP85837: PAX CGO Sequence No
        rejectionMemoRecord.BillingCode = (int)BillingCode.RejectionMemo;
        foreach (var vat in rejectionMemoRecord.RejectionMemoVat)
        {
          vat.ParentId = rejectionMemoRecord.Id;
        }

        string warningMessage;

        rejectionMemoRecord = _cargoInvoiceManager.UpdateRejectionMemoRecord(rejectionMemoRecord, out warningMessage);
        
        ShowSuccessMessage(string.Format("{0}", Messages.RMUpdateSuccessful));
        if (!String.IsNullOrEmpty(warningMessage))
          ShowErrorMessage(warningMessage, true);

        TempData.Remove(TempDataConstants.RejectedRecordIds);

        return RedirectToAction("RMCreate", "Invoice", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        rejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

        var rejectionMemoAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "Invoice", new { transactionId }));
        ViewData[ViewDataConstants.RejectionMemoAwbGrid] = rejectionMemoAwbGrid.Instance;

        SetViewDataPageMode(PageMode.Edit);

        return View("RMEdit", rejectionMemoRecord);
      }
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMEdit(string invoiceId, string transactionId, RejectionMemo rejectionMemoRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rejectionMemoRecord.AttachmentIndicatorOriginal = rejectionMemoRecord.Attachments.Count() > 0;

      try
      {

          #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
          if (!IsYourBillingDateValid(rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingPeriod))
          {
            throw new ISBusinessException("Invalid Your Invoice Billing Date.","");
          }
          #endregion

          //SCPID : 105938 - RE: IATA- SIS Bug
          string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(rejectionMemoRecord);
          if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage);

          rejectionMemoRecord.Id = transactionId.ToGuid();
        rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

        //SCP85837: PAX CGO Sequence No
        rejectionMemoRecord.BillingCode = (int)BillingCode.RejectionMemo;
        foreach (var vat in rejectionMemoRecord.RejectionMemoVat)
        {
          vat.ParentId = rejectionMemoRecord.Id;
        }

        string warningMessage;
        rejectionMemoRecord = _cargoInvoiceManager.UpdateRejectionMemoRecord(rejectionMemoRecord, out warningMessage);
        
        ShowSuccessMessage(string.Format("{0}", Messages.RMUpdateSuccessful));
        if (!String.IsNullOrEmpty(warningMessage))
          ShowErrorMessage(warningMessage, true);

        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          ViewData[ViewDataConstants.FromBillingHistory] = true;
        }

        return RedirectToAction("RMEdit", "Invoice", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        rejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

        var rejectionMemoAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "Invoice", new { transactionId }));
        ViewData[ViewDataConstants.RejectionMemoAwbGrid] = rejectionMemoAwbGrid.Instance;

        SetViewDataPageMode(PageMode.Edit);

        return View(rejectionMemoRecord);
      }
    }


    //<summary>
    //Gets the rejection memo record.
    //</summary>
    //<param name="transactionId">The transaction id.</param>
    //<param name="invoiceId">The invoice id.</param>
    //<returns></returns>
    private RejectionMemo GetRejectionMemoRecord(string transactionId, string invoiceId)
    {
      var rejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      var transactionType = 0;

      // Depending on RejectionStage retrieve transaction type
      switch (rejectionMemoRecord.RejectionStage)
      {
        case 1:
          transactionType = (int)TransactionType.CargoRejectionMemoStage1;
          break;
        case 2:
          transactionType = (int)TransactionType.CargoRejectionMemoStage2;
          break;
        case 3:
          transactionType = (int)TransactionType.CargoRejectionMemoStage3;
          break;
      }

      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      var reasonCode = _referenceManager.GetReasonCode(rejectionMemoRecord.ReasonCode, transactionType);
      bool isCouponBreakdownMandatory = reasonCode == null ? false : reasonCode.CouponAwbBreakdownMandatory;
      rejectionMemoRecord.Invoice = InvoiceHeader;

      // Set Coupon breakdown value
      rejectionMemoRecord.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

      return rejectionMemoRecord;
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

        var rejectedInvoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
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
            case "AWB Prepaid":
              record.BMCMIndicatorId = (int)BMCMIndicator.None;
              break;
            case "AWB Charge Collect":
              record.BMCMIndicatorId = (int)BMCMIndicator.None;
              break;
            case "AWB":
              record.BMCMIndicatorId = (int)BMCMIndicator.None;
              break;
            case "Billing Memo":
              record.BMCMIndicatorId = (int)BMCMIndicator.BMNumber;
              break;
            case "BM":
              record.BMCMIndicatorId = (int)BMCMIndicator.BMNumber;
              break;
            case "Credit Memo":
              record.BMCMIndicatorId = (int)BMCMIndicator.CMNumber;
              break;
            case "CM":
              record.BMCMIndicatorId = (int)BMCMIndicator.CMNumber;
              break;
            default:
              record.BMCMIndicatorId = (int)BMCMIndicator.None;
              break;
          }
        }

        if (transactionType == "AWB" || transactionType == "AWB Prepaid" || transactionType == "AWB Charge Collect")
        {
          record.RejectionStage = 1;
        }
        else if (transactionType == "BM" || transactionType == "Billing Memo")
        {
          record.RejectionStage = 1;
          var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(rejectionIdList[0]);
          record.YourBillingMemoNumber = billingMemo.BillingMemoNumber;
          record.BMCMIndicator = BMCMIndicator.BMNumber;
        }
        else if (transactionType == "CM" || transactionType == "Credit Memo")
        {
          record.RejectionStage = 1;
          var creditNoteManager = Ioc.Resolve<ICargoCreditNoteManager>(typeof(ICargoCreditNoteManager));
          var creditMemo = creditNoteManager.GetCreditMemoRecordDetails(rejectionIdList[0]);
          record.YourBillingMemoNumber = creditMemo.CreditMemoNumber;
          record.BMCMIndicator = BMCMIndicator.CMNumber;

        }
        else if (transactionType == "RM" || transactionType == "Rejection Memo")
        {
          var rejectedMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(rejectionIdList[0]);
          if (rejectedMemo != null)
          {
            record.RejectionStage = ++rejectedMemo.RejectionStage;
            record.YourRejectionNumber = rejectedMemo.RejectionMemoNumber;

            if (rejectedMemo.BMCMIndicator == BMCMIndicator.BMNumber || rejectedMemo.BMCMIndicator == BMCMIndicator.CMNumber)
            {
              record.BMCMIndicatorId = rejectedMemo.BMCMIndicatorId;
              record.YourBillingMemoNumber = rejectedMemo.YourBillingMemoNumber;
            }
          }
        }
        TempData[TempDataConstants.RejectedRecordIds] = originalRejectedRecordIds;
      }
    }


    private void GetRejectAwbListFromBillingHistory(CargoRejectionMemo rejectionMemo)
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

        if (transactionType == "AWB" || transactionType == "AWB Prepaid" || transactionType == "AWB Charge Collect")
        {
          var couponList = _cargoInvoiceManager.GetAwbRecordList(rejectionIdList);

          if (couponList == null)
          {
            return;
          }

          CopyAwbRecordListToRM(couponList, rejectionMemo);
        }
        return;
      }
    }

    [HttpPost]
    public virtual JsonResult GetRMLinkingDetails(FormCollection form)
    {
      try
      {
        var rmLinking = new JavaScriptSerializer().Deserialize(form[0], typeof(CgoRMLinkingCriteria));
        var criteria = rmLinking as CgoRMLinkingCriteria;

        if (criteria != null)
          criteria.IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod;
          
        var rejectionMemoResult = _cargoInvoiceManager.GetRejectionMemoLinkingDetails(criteria);

        return Json(rejectionMemoResult);
      }
      catch (ISBusinessException exception)
      {
        Logger.Error("Error while getting RM linking details.", exception);
      }

      return Json(new object());
    }

    /// <summary>
    ///call from jquery using Ajax and json, for getting the rejection memo coupon break down details
    /// </summary>
    [HttpPost]
    public JsonResult GetRMAwbBreakdownDetails(string issuingAirline, string serialNo, string rmId, int billingMemberId, int billedMemberId, int billingCode)
    {
      string awbSerialNo = "0";
      
      if (serialNo.Length == 8)
      {
        awbSerialNo = serialNo.Substring(0, 7);
      }

      var rmLinkedAwbRecords = _cargoInvoiceManager.GetRMAwbBreakdownRecordDetails(issuingAirline, Convert.ToInt32(awbSerialNo), rmId.ToGuid(), Convert.ToInt32(billingMemberId), Convert.ToInt32(billedMemberId), billingCode);

      return new DateConvertedJsonResult(){ Data = rmLinkedAwbRecords };
    }

    [HttpPost]
    public virtual JsonResult GetLinkedMemoDetailsForRM(FormCollection form)
    {
      try
      {
        var rmLinking = new JavaScriptSerializer().Deserialize(form[0], typeof(CgoRMLinkingCriteria));
        var criteria = rmLinking as CgoRMLinkingCriteria;

        var rejectionMemoResult = _cargoInvoiceManager.GetLinkedMemoAmountDetails(criteria);

        return Json(rejectionMemoResult);
      }
      catch (ISBusinessException exception)
      {
        Logger.Error("Error while getting linked memo details for RM.", exception);
      }

      return Json(new object());
    }

    /// <summary>
    /// Get the single record details from the list of RM coupon
    /// </summary>
    [HttpPost]
    public JsonResult GetRMAwbBreakdownSingleRecordDetails(string awbId, string rejectionMemoId)
    {
      var rmLinkedSingleAwbDetails = _cargoInvoiceManager.GetRMAwbBreakdownSingleRecordDetails(awbId.ToGuid(), rejectionMemoId.ToGuid());

      //return Json(rmLinkedSingleAwbDetails);
      return new DateConvertedJsonResult() { Data = rmLinkedSingleAwbDetails };
    }

    private void CopyAwbRecordListToRM(IList<AwbRecord> couponList, CargoRejectionMemo rejectionMemo)
    {
      var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(rejectionMemo.InvoiceId.ToString());

      IList<RMAwb> newRmCouponList = new List<RMAwb>();

      foreach (var awbRecord in couponList)
      {
        var coupondetails = _cargoInvoiceManager.GetRMAwbBreakdownSingleRecordDetails(awbRecord.Id, rejectionMemo.Id);
        string message;
        coupondetails.Details.RejectionMemoId = rejectionMemo.Id;
        _cargoInvoiceManager.AddRejectionMemoAwbDetails(coupondetails.Details, rejectionMemo.InvoiceId.ToString(), out message, true, null);
      }

      rejectionMemo.CouponBreakdownRecord.AddRange(newRmCouponList);
    }

    /// <summary>
    /// Sets the rejection memo linking details.
    /// </summary>
    /// <param name="record">The record, object of Rejection Memo</param>
    /// <param name="invoiceId">The invoice id.</param>
    private CgoRMLinkingResultDetails SetRejectionMemoLinkingDetails(RejectionMemo record, string invoiceId)
    {
        CgoRMLinkingResultDetails rejectionMemoResult = null;
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) && (record.RejectionStage > 1 || record.BMCMIndicatorId == (int)BMCMIndicator.BMNumber || record.BMCMIndicatorId == (int)BMCMIndicator.CMNumber))
        {
            var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();

            var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
            var rejectedInvoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(rejectedInvoiceId);

            var criteria = new CgoRMLinkingCriteria
                               {

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
                                   BMCMIndicatorId = record.BMCMIndicatorId,
                                   YourBillingMemoNumber = record.YourBillingMemoNumber,
                                   TransactionType = record.RejectionStage == 1 ? "BM" : "RM"
                               };

            rejectionMemoResult = _cargoInvoiceManager.GetRejectionMemoLinkingDetails(criteria);
            record.IsLinkingSuccessful = rejectionMemoResult.IsLinkingSuccessful;
            record.IsBreakdownAllowed = rejectionMemoResult.HasBreakdown;
            record.CurrencyConversionFactor = rejectionMemoResult.CurrencyConversionFactor;
        }

        return rejectionMemoResult;
    }


    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMCreate")]
    public ActionResult RMCreateAndAddNew(string invoiceId, RejectionMemo record)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      record.AttachmentIndicatorOriginal = record.Attachments.Count() > 0;

      //PaxInvoice invoice = null;
      var attachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {

        #region SCP197299: Incorrect ISIDEC format - PIDECT-21720130904.ZIP
        if (!IsYourBillingDateValid(record.YourInvoiceBillingYear, record.YourInvoiceBillingMonth, record.YourInvoiceBillingPeriod))
        {
          throw new ISBusinessException("Invalid Your Invoice Billing Date.","");
        }
        #endregion

        record.Attachments.Clear();
        record.LastUpdatedBy = SessionUtil.UserId;

        //SCP85837: PAX CGO Sequence No
        record.BillingCode = (int)BillingCode.RejectionMemo;
        var errorMessage = string.Empty;
        var warningMessage = string.Empty;

        //SCPID : 105938 - RE: IATA- SIS Bug
        string rmBreakdownErrMessage = ValidateRMMemoFieldsCalculation(record);
        if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
            throw new ISBusinessException(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage);

        var cgoRMLinkingResultDetails = SetRejectionMemoLinkingDetails(record, invoiceId);

        // CMP#650 set reason code coming from linking
        if (cgoRMLinkingResultDetails != null && !string.IsNullOrEmpty(cgoRMLinkingResultDetails.ReasonCode))
        {
            record.YourReasonCode = cgoRMLinkingResultDetails.ReasonCode;
        }

        _cargoInvoiceManager.AddRejectionMemoRecord(record, out errorMessage, out warningMessage);

        _cargoInvoiceManager.UpdateRejectionMemoAttachment(attachmentIds, record.Id);

        GetRejectAwbListFromBillingHistory(record);

        TempData.Clear();
        
        ShowSuccessMessage(string.Format("{0}{1}{2}", errorMessage, Environment.NewLine, Messages.RMCreateSuccessful));
        if (!String.IsNullOrEmpty(warningMessage))
          ShowErrorMessage(warningMessage, true);

        return RedirectToAction("RMCreate", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        record.Attachments = _cargoInvoiceManager.GetRejectionMemoAttachments(attachmentIds);
        ViewData[ViewDataConstants.IsPostback] = true;
      }

      SetRejectionMemoDetailsForBillingHistory(record);

      KeepBillingHistoryDataInStore(true);

      record.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      return View("RMCreate", record);
    }

    /// <summary>
    /// RejectionMemo Coupon breakdown create
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [OutputCache(CacheProfile = "donotCache")]
    [HttpGet]
    public ActionResult RMPrepaidAwbCreate(string invoiceId, string transactionId)
    {
      var breakdownrecord = new RMAwb();
      var rejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoRecord.Invoice = InvoiceHeader;
      rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
      breakdownrecord.RejectionMemoRecord = rejectionMemoRecord;
      SetViewDataPageMode(PageMode.Create);

      // Set ViewData, "IsPostback" to false
      ViewData["IsPostback"] = false;

      // If action is 'Save and Add New' then populate the previous batch number and sequence no+1
      if (TempData["RMCouponRecord"] != null)
      {
        // Set Viewdata
        ViewData["RMCouponRecord"] = TempData["RMCouponRecord"];
      }
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details

      if ((rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3 ) && rejectionMemoRecord.IsLinkingSuccessful == true)
      {
          CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, rejectionMemoRecord.Invoice.BilledMemberId, rejectionMemoRecord.Invoice.BillingMemberId, null, null);

          if (yourInvoice != null && (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0 )
          {
              CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
              if (yourRm!=null && yourRm.CouponBreakdownRecord.Count == 0)
              ViewData["IsAwbLinkingRequired"] = false;
          }
      }
      return View(breakdownrecord);
    }

    /// <summary>
    /// RejectionMemo Coupon breakdown create
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [OutputCache(CacheProfile = "donotCache")]
    [HttpGet]
    public ActionResult RMChargeCollectAwbCreate(string invoiceId, string transactionId)
    {
      var breakdownrecord = new RMAwb();
      var rejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoRecord.Invoice = InvoiceHeader;
      rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
      breakdownrecord.RejectionMemoRecord = rejectionMemoRecord;
      SetViewDataPageMode(PageMode.Create);

      // Set ViewData, "IsPostback" to false
      ViewData["IsPostback"] = false;

      // If action is 'Save and Add New' then populate the previous batch number and sequence no+1
      if (TempData["RMCouponRecord"] != null)
      {
        // Set Viewdata
        ViewData["RMCouponRecord"] = TempData["RMCouponRecord"];
      }
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3) && rejectionMemoRecord.IsLinkingSuccessful == true)
      {
          CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, rejectionMemoRecord.Invoice.BilledMemberId, rejectionMemoRecord.Invoice.BillingMemberId, null, null);

          if (yourInvoice != null && (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
          {
              CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
              if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
                  ViewData["IsAwbLinkingRequired"] = false;
          }
      }
      return View(breakdownrecord);
    }

    //[HttpPost]
    //public virtual JsonResult GetRMLinkingDetails(FormCollection form)
    //{
    //  try
    //  {
    //    var rmLinking = new JavaScriptSerializer().Deserialize(form[0], typeof(RMLinkingCriteria));
    //    var criteria = rmLinking as RMLinkingCriteria;

    //    if (criteria != null)
    //      criteria.IgnoreValidationOnMigrationPeriod = SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod;

    //    var rejectionMemoResult = _cargoInvoiceManager.GetRejectionMemoLinkingDetails(criteria);

    //    return Json(rejectionMemoResult);
    //  }
    //  catch (ISBusinessException exception)
    //  {
    //    Logger.Error("Error while getting RM linking details.", exception);
    //  }

    //  return Json(new object());
    //}

    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult RejectionMemoAttachmentUpload(string invoiceId, string transactionId)
    {
      string files = string.Empty;
      var attachments = new List<CgoRejectionMemoAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method RejectionMemoAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
          //CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
          CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceDetails(invoiceId);
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
          Logger.Info("Checking for duplicate supporting document attachement");
          // On Rejection Memo Edit
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoInvoiceManager.IsDuplicateRejectionMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
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

          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            CgoRejectionMemoAttachment attachment = new CgoRejectionMemoAttachment
                               {
                                 Id = fileUploadHelper.FileServerName,
                                 OriginalFileName = fileUploadHelper.FileOriginalName,
                                 FileSize = fileUploadHelper.FileToSave.ContentLength,
                                 LastUpdatedBy = SessionUtil.UserId,
                                 ServerId = fileUploadHelper.FileServerInfo.ServerId,
                                 FileStatus = FileStatusType.Received,
                                 FilePath = fileUploadHelper.FileRelativePath
                               };

            attachment = _cargoInvoiceManager.AddRejectionMemoAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in databse");
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
      // return Json(null);
      //return new FileUploadJsonResult { Data = new { } };
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download)]
    [HttpGet]
    public FileStreamResult RejectionMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetRejectionMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    ///  Creates  RM Prepaid AWB.
    /// </summary>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMPrepaidAwbCreate(string invoiceId, string transactionId, RMAwb rmAwbRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwbRecord.AttachmentIndicatorOriginal = rmAwbRecord.Attachments.Count() > 0;

      var awbAttachmentIds = rmAwbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        rmAwbRecord.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);
        AddRMAwbRecord(rmAwbRecord, transactionId, invoiceId, awbAttachmentIds);

        //TempData["RMCouponRecord"] = string.Format(@"{0}-{1}", rmAwbRecord.SerialNo, rmAwbRecord.TicketOrFimIssuingAirline);

        return RedirectToAction("RMPrepaidAwbCreate", new { invoiceId, transactionId, transaction = "RMEdit", couponId = rmAwbRecord.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        SetViewDataPageMode(PageMode.Clone); // Done to hide the serial number field.
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
   //     rmAwbRecord.Attachments = _cargoInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
      }

      rmAwbRecord.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rmAwbRecord.RejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && rmAwbRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
        CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rmAwbRecord.RejectionMemoRecord.YourInvoiceNumber, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingMonth, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingYear, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingPeriod, rmAwbRecord.RejectionMemoRecord.Invoice.BilledMemberId, rmAwbRecord.RejectionMemoRecord.Invoice.BillingMemberId, null, null);

        if (yourInvoice != null && (rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
        {
          CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rmAwbRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
          if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
            ViewData["IsAwbLinkingRequired"] = false;
        }
      }
      return View(rmAwbRecord);
    }

    /// <summary>
    /// Creates  RM Charge Collect AWB and redirects to RM Edit.
    /// </summary>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMChargeCollectAwbCreate(string invoiceId, string transactionId, RMAwb rmAwbRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwbRecord.AttachmentIndicatorOriginal = rmAwbRecord.Attachments.Count() > 0;

      var awbAttachmentIds = rmAwbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        rmAwbRecord.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);
        AddRMAwbRecord(rmAwbRecord, transactionId, invoiceId, awbAttachmentIds);

        //TempData["RMCouponRecord"] = string.Format(@"{0}-{1}", rmAwbRecord.SerialNo, rmAwbRecord.TicketOrFimIssuingAirline);

        return RedirectToAction("RMChargeCollectAwbCreate", new { invoiceId, transactionId, transaction = "RMEdit", couponId = rmAwbRecord.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        SetViewDataPageMode(PageMode.Clone); // Done to hide the serial number field.
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        //     rmAwbRecord.Attachments = _cargoInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
      }

      rmAwbRecord.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rmAwbRecord.RejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && rmAwbRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
        CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rmAwbRecord.RejectionMemoRecord.YourInvoiceNumber, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingMonth, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingYear, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingPeriod, rmAwbRecord.RejectionMemoRecord.Invoice.BilledMemberId, rmAwbRecord.RejectionMemoRecord.Invoice.BillingMemberId, null, null);

        if (yourInvoice != null && (rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
        {
          CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rmAwbRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
          if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
            ViewData["IsAwbLinkingRequired"] = false;
        }
      }
      return View(rmAwbRecord);
    }

    /// <summary>
    /// Creates  RM Prepaid AWB and redirects to RM Edit.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMPrepaidAwbCreate")]
    public ActionResult RMPrepaidAwbCreateAndReturn(string invoiceId, string transactionId, RMAwb rmAwbRecord)
    {
      ////////////////////////////////////////
      var logRefId = Guid.NewGuid();
      var log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 1:RMPrepaidAwbCreateAndReturn -Start ", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwbRecord.AttachmentIndicatorOriginal = rmAwbRecord.Attachments.Count() > 0;

      //Get attachment Id list
      var awbAttachmentIds = rmAwbRecord.Attachments.Select(attachment => attachment.Id).ToList();
           

      try
      {
        ////////////////////////////////////////
        log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 2:AddRMAwbRecord -Start", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);
        ////////////////////////////////////////

        rmAwbRecord.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);
        AddRMAwbRecord(rmAwbRecord, transactionId, invoiceId, awbAttachmentIds, logRefId);

        ////////////////////////////////////////
        log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 2:AddRMAwbRecord -End", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);
        ////////////////////////////////////////

     //   TempData["RMCouponRecord"] = "";

        rmAwbRecord.Attachments.Clear(); // Attachments should not be duplicated. 


        ////////////////////////////////////////
        log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                    BillingCategorys.Cargo.ToString(), "Stage 1.1:RMPrepaidAwbCreateAndReturn -End", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);
        ////////////////////////////////////////

        return RedirectToAction("RMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        //rmAwbRecord.Attachments = _cargoInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 3:GetRejectionMemoRecordDetails -Start", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      rmAwbRecord.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 3:GetRejectionMemoRecordDetails -End", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 4:GetInvoiceHeaderDetails -Start", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      rmAwbRecord.RejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 4:GetInvoiceHeaderDetails -End", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && rmAwbRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
        ////////////////////////////////////////
        log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 5:GetInvoiceWithRMCoupons -Start", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);
        ////////////////////////////////////////
        CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rmAwbRecord.RejectionMemoRecord.YourInvoiceNumber, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingMonth, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingYear, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingPeriod, rmAwbRecord.RejectionMemoRecord.Invoice.BilledMemberId, rmAwbRecord.RejectionMemoRecord.Invoice.BillingMemberId, null, null);
        ////////////////////////////////////////
        log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 5:GetInvoiceWithRMCoupons -End", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);
        ////////////////////////////////////////

        if (yourInvoice != null && (rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
        {
          CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rmAwbRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
          if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
            ViewData["IsAwbLinkingRequired"] = false;
        }
      }

      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbCreateAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 1.2:RMPrepaidAwbCreateAndReturn -End", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////

      return View("RMPrepaidAwbCreate", rmAwbRecord);
    }

    /// <summary>
    /// Creates RM Charge collect AWB and redirects to RM Edit
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMChargeCollectAwbCreate")]
    public ActionResult RMChargeCollectAwbCreateAndReturn(string invoiceId, string transactionId, RMAwb rmAwbRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwbRecord.AttachmentIndicatorOriginal = rmAwbRecord.Attachments.Count() > 0;

      //Get attachment Id list
      var awbAttachmentIds = rmAwbRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
        rmAwbRecord.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);
        AddRMAwbRecord(rmAwbRecord, transactionId, invoiceId, awbAttachmentIds);

        //   TempData["RMCouponRecord"] = "";

        rmAwbRecord.Attachments.Clear(); // Attachments should not be duplicated. 

        return RedirectToAction("RMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        //rmAwbRecord.Attachments = _cargoInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      rmAwbRecord.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rmAwbRecord.RejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && rmAwbRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
        CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rmAwbRecord.RejectionMemoRecord.YourInvoiceNumber, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingMonth, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingYear, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingPeriod, rmAwbRecord.RejectionMemoRecord.Invoice.BilledMemberId, rmAwbRecord.RejectionMemoRecord.Invoice.BillingMemberId, null, null);

        if (yourInvoice != null && (rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
        {
          CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rmAwbRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
          if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
            ViewData["IsAwbLinkingRequired"] = false;
        }
      }
      return View("RMChargeCollectAwbCreate", rmAwbRecord);
    }

    /// <summary>
    /// Fetch data for Rejection Memo Coupon Breakdown for Rejection Memo and display it in grid
    /// </summary>
    /// <param name="transactionId"></param>
    public JsonResult RejectionMemoAwbGridData(string transactionId)
    {
      // Create grid instance and retrieve data from database
      var rmAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "Invoice", new { transactionId }));
      var rejectionMemoAwbList = _cargoInvoiceManager.GetRejectionMemoAwbList(transactionId);

      return rmAwbGrid.DataBind(rejectionMemoAwbList.AsQueryable());
    }

    /// <summary>
    /// Used when navigated from the listing grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <param name="couponId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult RMAwbEdit(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetRMAwbRecord(couponId, transactionId);
      if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
        return RedirectToAction("RMPrepaidAWBEdit", new { invoiceId, transactionId, couponId });

      return RedirectToAction("RMChargeCollectAWBEdit", new { invoiceId, transactionId, couponId });
    }

    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    [HttpGet]
    public ActionResult RMAwbView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetRMAwbRecord(couponId, transactionId);
      SetViewDataPageMode(PageMode.View);
      if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
        return RedirectToAction("RMPrepaidAwbView", new { invoiceId, transactionId, couponId });

      return RedirectToAction("RMChargeCollectAwbView", new { invoiceId, transactionId, couponId });
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult RMPrepaidAwbEdit(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetRMAwbRecord(couponId, transactionId);
      
      return View("RMPrepaidAWBEdit", awbRecord);
    }

    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult RMPrepaidAwbView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetRMAwbRecord(couponId, transactionId);

      return View("RMPrepaidAWBEdit", awbRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult RMChargeCollectAwbEdit(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetRMAwbRecord(couponId, transactionId);

      return View("RMChargeCollectAWBEdit", awbRecord);
    }

    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult RMChargeCollectAwbView(string invoiceId, string transactionId, string couponId)
    {
      var awbRecord = GetRMAwbRecord(couponId, transactionId);

      return View("RMChargeCollectAWBEdit", awbRecord);
    }

    private RMAwb GetRMAwbRecord(string couponId, string transactionId)
    {
      var rejectionMemoAwbDetails = _cargoInvoiceManager.GetRejectionMemoAwbDetails(couponId);
     
      rejectionMemoAwbDetails.LastUpdatedBy = SessionUtil.UserId;
      rejectionMemoAwbDetails.RejectionMemoRecord.Invoice = InvoiceHeader;
      return rejectionMemoAwbDetails;
    }

    /// <summary>
    /// Delete Rejection memo AWB Record
    /// </summary>
    /// <param name="transactionId">RM AWB Id</param>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(IsJson = true,TransactionParamName = "transactionId",TableName =TransactionTypeTable.CGO_RM_AWB)]
    public JsonResult RMAwbDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        Guid invoiceId;
        Guid rejectionMemoId;
        var isDeleted = _cargoInvoiceManager.DeleteRejectionMemoAwbRecord(transactionId, out rejectionMemoId, out invoiceId);

        details = isDeleted ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, RedirectUrl = Url.Action("RMEdit", new { invoiceId, transactionId = rejectionMemoId }), isRedirect = true } : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
        if(isDeleted)
          ShowSuccessMessage(Messages.DeleteSuccessful, true);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Creates  RM Prepaid AWB and allows to duplicate same record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMPrepaidAwbCreate")]
    public ActionResult RMPrepaidAwbDuplicate(string invoiceId, string transactionId, RMAwb rmAwbRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwbRecord.AttachmentIndicatorOriginal = rmAwbRecord.Attachments.Count() > 0;

      //Get attachment Id list
      var awbAttachmentIds = rmAwbRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        rmAwbRecord.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);
        AddRMAwbRecord(rmAwbRecord, transactionId, invoiceId, awbAttachmentIds);

        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        rmAwbRecord.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
    //    rmAwbRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      rmAwbRecord.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rmAwbRecord.RejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && rmAwbRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
          CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rmAwbRecord.RejectionMemoRecord.YourInvoiceNumber, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingMonth, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingYear, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingPeriod, rmAwbRecord.RejectionMemoRecord.Invoice.BilledMemberId, rmAwbRecord.RejectionMemoRecord.Invoice.BillingMemberId, null, null);

          if (yourInvoice != null && (rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
          {
              CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rmAwbRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
              if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
                  ViewData["IsAwbLinkingRequired"] = false;
          }
      }
      return View("RMPrepaidAwbCreate", rmAwbRecord);

    }

    /// <summary>
    /// Creates RM Charge collect AWB and allows to duplicate same record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMChargeCollectAwbCreate")]
    public ActionResult RMChargeCollectAwbDuplicate(string invoiceId, string transactionId, RMAwb rmAwbRecord)
    {
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwbRecord.AttachmentIndicatorOriginal = rmAwbRecord.Attachments.Count() > 0;

      //Get attachment Id list
      var awbAttachmentIds = rmAwbRecord.Attachments.Select(attachment => attachment.Id).ToList();

      try
      {
          // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;

        rmAwbRecord.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);
        AddRMAwbRecord(rmAwbRecord, transactionId, invoiceId, awbAttachmentIds);

        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        rmAwbRecord.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
      }
      catch (ISBusinessException businessException)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);
        //    rmAwbRecord.Attachments = _nonSamplingInvoiceManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Create);
      }

      rmAwbRecord.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
      rmAwbRecord.RejectionMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // Done to prevent Save button from getting disabled on exception.
      ViewData[ViewDataConstants.IsExceptionOccurred] = true;
      //CMP#501 :
      ViewData["IsAwbLinkingRequired"] = true;
      //Get your invoice details to check AWB Record count.

      if ((rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && rmAwbRecord.RejectionMemoRecord.IsLinkingSuccessful == true)
      {
          CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rmAwbRecord.RejectionMemoRecord.YourInvoiceNumber, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingMonth, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingYear, rmAwbRecord.RejectionMemoRecord.YourInvoiceBillingPeriod, rmAwbRecord.RejectionMemoRecord.Invoice.BilledMemberId, rmAwbRecord.RejectionMemoRecord.Invoice.BillingMemberId, null, null);

          if (yourInvoice != null && (rmAwbRecord.RejectionMemoRecord.RejectionStage == 2 || rmAwbRecord.RejectionMemoRecord.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
          {
              CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rmAwbRecord.RejectionMemoRecord.YourRejectionNumber).FirstOrDefault();
              if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
                  ViewData["IsAwbLinkingRequired"] = false;
          }
      }
      return View("RMChargeCollectAwbCreate", rmAwbRecord);

    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMPrepaidAwbEdit(string couponId, string transactionId, string invoiceId, RMAwb rmAwb)
    {
      rmAwb.RejectionMemoId = transactionId.ToGuid();
      var awbAttachmentIds = rmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      rmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);

      try
      {
        EditRMAwb(rmAwb, couponId, invoiceId);

        //TempData["RMCouponRecord"] = string.Format(@"{0}-{1}", rmAwb.SerialNo, rmAwb.TicketOrFimIssuingAirline);

        return RedirectToAction("RMPrepaidAwbCreate", new { invoiceId, transactionId, couponId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var rejectionMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        rejectionMemo.Invoice = invoice;
        rmAwb.RejectionMemoRecord = rejectionMemo;
        rmAwb.Attachments = _cargoInvoiceManager.GetRejectionMemoAwbAttachments(awbAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMPrepaidAwbEdit", rmAwb);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult RMChargeCollectAwbEdit(string couponId, string transactionId, string invoiceId, RMAwb rmAwb)
    {
      rmAwb.RejectionMemoId = transactionId.ToGuid();
      var awbAttachmentIds = rmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      rmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);

      try
      {
        EditRMAwb(rmAwb, couponId, invoiceId);

        //TempData["RMCouponRecord"] = string.Format(@"{0}-{1}", rmAwb.SerialNo, rmAwb.TicketOrFimIssuingAirline);

        return RedirectToAction("RMChargeCollectAwbCreate", new { invoiceId, transactionId, couponId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var rejectionMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        rejectionMemo.Invoice = invoice;
        rmAwb.RejectionMemoRecord = rejectionMemo;
        rmAwb.Attachments = _cargoInvoiceManager.GetRejectionMemoAwbAttachments(awbAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMChargeCollectAwbEdit", rmAwb);
    }

    private void EditRMAwb(RMAwb rmAwb, string couponId, string invoiceId)
    {

        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "EditRMAwb", this.ToString(),
                                      BillingCategorys.Cargo.ToString(), "Stage 1:EditRMAwb Start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);
      // SCP165091: Attachments
      // Added server-side validation for attachement indecator original
      rmAwb.AttachmentIndicatorOriginal = rmAwb.Attachments.Count() > 0;

      rmAwb.Id = couponId.ToGuid();
      rmAwb.LastUpdatedBy = SessionUtil.UserId;
      // Assign parent rejection AWB record id to Other Charge records.
      foreach (var otherCharge in rmAwb.OtherCharge)
      {
        otherCharge.ParentId = rmAwb.Id;
      }
      // Assign parent rejection AWB record id to vat records.
      foreach (var vat in rmAwb.AwbVat)
      {
        vat.ParentId = rmAwb.Id;
      }

      // Assign parent rejection AWB record id to prorate ladder detail records.
      foreach (var rmAwbProrateLadderDetail in rmAwb.ProrateLadder)
      {
        rmAwbProrateLadderDetail.ParentId = rmAwb.Id;
      }

      string duplicateErrorMessage;
      log = _referenceManager.GetDebugLog(DateTime.Now, "EditRMAwb", this.ToString(),
                                 BillingCategorys.Cargo.ToString(), "Stage 2:ValidateRMAwbBreakdownFieldsCalculation Start ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      //SCPID : 105938 - RE: IATA- SIS Bug
      string rmBreakdownErrMessage = ValidateRMAwbBreakdownFieldsCalculation(rmAwb);
      log = _referenceManager.GetDebugLog(DateTime.Now, "EditRMAwb", this.ToString(),
                              BillingCategorys.Cargo.ToString(), "Stage 2:ValidateRMAwbBreakdownFieldsCalculation completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
          throw new ISBusinessException(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage);

      //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
      Ref: FRS Section 3.4 Table 18 Row 11 and 12 */
      MemberManager.ValidateIssuingAirline(rmAwb.AwbIssueingAirline);
      log = _referenceManager.GetDebugLog(DateTime.Now, "EditRMAwb", this.ToString(),
                            BillingCategorys.Cargo.ToString(), "Stage 3:ValidateIssuingAirline completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      int vatRecordCountBefore = rmAwb.AwbVat.Count;
      int otherChargeCountBefore = rmAwb.OtherCharge.Count;
      _cargoInvoiceManager.UpdateRejectionMemoAwbDetails(rmAwb, invoiceId, out duplicateErrorMessage);
      int vatRecordCountAfter = rmAwb.AwbVat.Count;
      int otherChargeCountAfter = rmAwb.OtherCharge.Count;
  
      log = _referenceManager.GetDebugLog(DateTime.Now, "EditRMAwb", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 4:UpdateRejectionMemoAwbDetails completed ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      ShowSuccessMessages(Messages.RMAwbUpdateSuccessful, vatRecordCountBefore, vatRecordCountAfter, otherChargeCountBefore, otherChargeCountAfter);
      if (!String.IsNullOrEmpty(duplicateErrorMessage))
        ShowErrorMessage(duplicateErrorMessage, true);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMPrepaidAwbEdit")]
    public ActionResult RMPrepaidAwbClone(string couponId, string transactionId, string invoiceId, RMAwb rmAwb)
    {
      rmAwb.RejectionMemoId = transactionId.ToGuid();
      var couponAttachmentIds = rmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      var rejectionMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);

      var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      rejectionMemo.Invoice = invoice;
      rmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);
      rmAwb.RejectionMemoRecord = rejectionMemo;

      try
      {
        EditRMAwb(rmAwb, couponId, invoiceId);

        rmAwb.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;
        //CMP#501 :
        ViewData["IsAwbLinkingRequired"] = true;
        //Get your invoice details

        if ((rejectionMemo.RejectionStage == 2 || rejectionMemo.RejectionStage == 3) && rejectionMemo.IsLinkingSuccessful == true)
        {
            CargoInvoice yourInvoice = _cargoInvoiceManager.GetInvoiceWithRMCoupons(rejectionMemo.YourInvoiceNumber, rejectionMemo.YourInvoiceBillingMonth, rejectionMemo.YourInvoiceBillingYear, rejectionMemo.YourInvoiceBillingPeriod, rejectionMemo.Invoice.BilledMemberId, rejectionMemo.Invoice.BillingMemberId, null, null);

            if (yourInvoice != null && (rejectionMemo.RejectionStage == 2 || rejectionMemo.RejectionStage == 3) && yourInvoice.CGORejectionMemo.Count > 0)
            {
                CargoRejectionMemo yourRm = yourInvoice.CGORejectionMemo.Where(c => c.RejectionMemoNumber == rejectionMemo.YourRejectionNumber).FirstOrDefault();
                if (yourRm != null && yourRm.CouponBreakdownRecord.Count == 0)
                    ViewData["IsAwbLinkingRequired"] = false;
            }
        }
        return View("RMPrepaidAwbCreate", rmAwb);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        
        rmAwb.Attachments = _cargoInvoiceManager.GetRejectionMemoAwbAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMPrepaidAwbEdit", rmAwb);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMChargeCollectAwbEdit")]
    public ActionResult RMChargeCollectAwbClone(string couponId, string transactionId, string invoiceId, RMAwb rmAwb)
    {
      rmAwb.RejectionMemoId = transactionId.ToGuid();
      var couponAttachmentIds = rmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      var rejectionMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);

      var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
      rejectionMemo.Invoice = invoice;
      rmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);
      rmAwb.RejectionMemoRecord = rejectionMemo;

      try
      {
        EditRMAwb(rmAwb, couponId, invoiceId);

        rmAwb.Attachments.Clear(); // attachments should not be duplicated.
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = false;
        // Set ViewData "FromClone" to true
        ViewData["FromClone"] = true;

        return View("RMChargeCollectAwbCreate", rmAwb);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
        
        rmAwb.Attachments = _cargoInvoiceManager.GetRejectionMemoAwbAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMChargeCollectAwbEdit", rmAwb);
    }

    /// <summary>
    /// 
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMPrepaidAwbEdit")]
    public ActionResult RMPrepaidAwbEditAndReturn(string couponId, string transactionId, string invoiceId, RMAwb rmAwb)
    {
      rmAwb.RejectionMemoId = transactionId.ToGuid();
      var logRefId = Guid.NewGuid();
      var log = _referenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbEditAndReturn", this.ToString(),
                                  BillingCategorys.Cargo.ToString(), "Stage 1:RMPrepaidAwbEditAndReturn Start ", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      var awbAttachmentIds = rmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      log = _referenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbEditAndReturn", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2:Attachment Id's selected", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      rmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);

      try
      {
           log = _referenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbEditAndReturn", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 3:EditRMAwb start", SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);
         EditRMAwb(rmAwb, couponId, invoiceId);
         log = _referenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbEditAndReturn ", this.ToString(),
                              BillingCategorys.Cargo.ToString(), "Stage 3:EditRMAwb completed", SessionUtil.UserId, logRefId.ToString());
         _referenceManager.LogDebugData(log);
        //  TempData["RMCouponRecord"] = "";
        return RedirectToAction("RMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var rejectionMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        rejectionMemo.Invoice = invoice;
        rmAwb.RejectionMemoRecord = rejectionMemo;
        rmAwb.Attachments = _cargoInvoiceManager.GetRejectionMemoAwbAttachments(awbAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      log = _referenceManager.GetDebugLog(DateTime.Now, "RMPrepaidAwbEditAndReturn", this.ToString(),
                           BillingCategorys.Cargo.ToString(), "Stage 1 :RMPrepaidAwbEditAndReturn completed", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);
      return View("RMPrepaidAwbEdit", rmAwb);
    }

    /// <summary>
    /// 
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "RMChargeCollectAwbEdit")]
    public ActionResult RMChargeCollectAwbEditAndReturn(string couponId, string transactionId, string invoiceId, RMAwb rmAwb)
    {
      rmAwb.RejectionMemoId = transactionId.ToGuid();
      var awbAttachmentIds = rmAwb.Attachments.Select(attachment => attachment.Id).ToList();
      rmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);

      try
      {
         EditRMAwb(rmAwb, couponId, invoiceId);
        //  TempData["RMCouponRecord"] = "";
        return RedirectToAction("RMEdit", new { invoiceId, transactionId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData["IsPostback"] = true;
        ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

        var rejectionMemo = _cargoInvoiceManager.GetRejectionMemoRecordDetails(transactionId);
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        rejectionMemo.Invoice = invoice;
        rmAwb.RejectionMemoRecord = rejectionMemo;
        rmAwb.Attachments = _cargoInvoiceManager.GetRejectionMemoAwbAttachments(awbAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("RMChargeCollectAwbEdit", rmAwb);
    }

    private void AddRMAwbRecord(RMAwb rmAwbRecord, string transactionId, string invoiceId, List<Guid> awbAttachmentIds, Guid? logRefId = null)
    {

      ////////////////////////////////////////
      var log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.1: ValidateRMAwbBreakdownFieldsCalculation -Start", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////

        rmAwbRecord.RejectionMemoId = transactionId.ToGuid();
        //SCPID : 105938 - RE: IATA- SIS Bug
      string rmBreakdownErrMessage = ValidateRMAwbBreakdownFieldsCalculation(rmAwbRecord);
          if (!string.IsNullOrEmpty(rmBreakdownErrMessage))
              throw new ISBusinessException(ErrorCodes.InvalidCalculation, rmBreakdownErrMessage);

          ////////////////////////////////////////
          log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                    BillingCategorys.Cargo.ToString(), "Stage 2.1:ValidateRMAwbBreakdownFieldsCalculation-End", SessionUtil.UserId, logRefId.ToString());
          ReferenceManager.LogDebugData(log);
          ////////////////////////////////////////

      string duplicateErrorMessage;
      

      rmAwbRecord.Attachments.Clear();
      rmAwbRecord.LastUpdatedBy = SessionUtil.UserId;
      int vatRecordCountBefore = rmAwbRecord.AwbVat.Count;
      int otherChargeCountBefore = rmAwbRecord.OtherCharge.Count;

      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.2:AddRejectionMemoAwbDetails-Start", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      _cargoInvoiceManager.AddRejectionMemoAwbDetails(rmAwbRecord, invoiceId, out duplicateErrorMessage, logRefId: logRefId);
      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.2:AddRejectionMemoAwbDetails-End", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////

      int vatRecordCountAfter = rmAwbRecord.AwbVat.Count;
      int otherChargeCountAfter = rmAwbRecord.OtherCharge.Count;
      //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 11 and 12 */

      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.3:ValidateIssuingAirline -Start", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      MemberManager.ValidateIssuingAirline(rmAwbRecord.AwbIssueingAirline);
      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.3:ValidateIssuingAirline-End", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////

      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.4:UpdateRejectionMemoAwbAttachment-Start", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////
      _cargoInvoiceManager.UpdateRejectionMemoAwbAttachment(awbAttachmentIds, rmAwbRecord.Id);
      ////////////////////////////////////////
      log = ReferenceManager.GetDebugLog(DateTime.Now, "AddRMAwbRecord", this.ToString(),
                                BillingCategorys.Cargo.ToString(), "Stage 2.4:UpdateRejectionMemoAwbAttachment-End", SessionUtil.UserId, logRefId.ToString());
      ReferenceManager.LogDebugData(log);
      ////////////////////////////////////////

      ShowSuccessMessages(Messages.RMAwbCreateSuccessful, vatRecordCountBefore, vatRecordCountAfter, otherChargeCountBefore, otherChargeCountAfter);
      if (!String.IsNullOrEmpty(duplicateErrorMessage))
        ShowErrorMessage(duplicateErrorMessage, true);
    }

    private void ShowSuccessMessages(string message, int vatRecordCountBefore, int vatRecordCountAfter, int otherChargeCountBefore, int otherChargeCountAfter)
    {
      if (CheckIfVatBreakdownDeleted(vatRecordCountBefore, vatRecordCountAfter))
      {
        message += string.Format(" {0}", Messages.VatRecordsDeletedInfo);
      }
      
      if(CheckIfOtherChargeBreakdownDeleted(otherChargeCountBefore, otherChargeCountAfter))
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
    /// Upload Rejection Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult RMAwbAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<RMAwbAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;
      try
      {
          Logger.Info("Started execution for method RMAwbAttachmentUpload for invoice ID" + invoiceId);
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
        //CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceDetails(invoiceId);
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
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoInvoiceManager.IsDuplicateRejectionMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
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
          Logger.Info("Attachment validated successfully");
          if (fileUploadHelper.SaveFile())
          {
            Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new RMAwbAttachment()
            {
              Id = fileUploadHelper.FileServerName,
              OriginalFileName = fileUploadHelper.FileOriginalName,
              FileSize = fileUploadHelper.FileToSave.ContentLength,
              LastUpdatedBy = SessionUtil.UserId,
              ServerId = fileUploadHelper.FileServerInfo.ServerId,
              FileStatus = FileStatusType.Received,
              FilePath = fileUploadHelper.FileRelativePath
            };

            attachment = _cargoInvoiceManager.AddRejectionMemoAwbAttachment(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new User();
            }
            // assign user info from session and file server info.
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
    /// Download Rejection Memo AWB attachment
    /// </summary>
    /// <param name="invoiceId">Invoice id</param>
    /// <param name="couponId">Coupon id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download)]
    [HttpGet]
    public FileStreamResult RMAwbAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetRejectionMemoAwbAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ValidateAntiForgeryToken]
    [RestrictUnauthorizedUpdate]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "Edit")]
    public ActionResult ValidateInvoice(string invoiceId)
    {
        ValidateInvoice(_cargoInvoiceManager, invoiceId);

      return RedirectToAction("Edit");
    }

    /// <summary>
    /// Fetch data for Prepaid Billing for Invoice and display it in grid
    /// </summary>
    public JsonResult PrepaidBillingGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var preapidBillingGrid = new AwbPrepaidBillingGrid(ControlIdConstants.CouponGridId, Url.Action(PrepaidBillingGridAction, new { invoiceId }));
      var prepaidBillingCoupons = _cargoInvoiceManager.GetAwbPrepaidBillingRecordList(invoiceId).AsQueryable();
      //List<AwbRecord> invObj = new List<AwbRecord>();
      return preapidBillingGrid.DataBind(prepaidBillingCoupons.AsQueryable());
    }

    /// <summary>
    /// Fetch data for Prepaid Billing for Invoice and display it in grid
    /// </summary>
    public JsonResult ChargeCollectBillingGridData(string invoiceId)
    {
      // Create grid instance and retrieve data from database
      var chargeCollectBillingGrid = new AwbChargeCollectGrid(ControlIdConstants.CouponGridId, Url.Action("ChargeCollectBillingGridData", new { invoiceId }));
      var chargeCollectBillingCoupons = _cargoInvoiceManager.GetAwbChargeCollectBillingRecordList(invoiceId).AsQueryable();
      //List<AwbRecord> invObj = new List<AwbRecord>();
      return chargeCollectBillingGrid.DataBind(chargeCollectBillingCoupons.AsQueryable());
    }
    /// <summary>
    /// Display Prepaid Billing coupons in Invoice 
    /// </summary>

    public ActionResult AwbPrepaidBillingList(string invoiceId)
    {
      
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }
      var primeBillingGrid = new AwbPrepaidBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action(PrepaidBillingGridAction, new { invoiceId }), isRejectionAllowed);
      ViewData[ViewDataConstants.PrimeBillingGrid] = primeBillingGrid.Instance;

      return View(InvoiceHeader);
    }


    public ActionResult AwbPrepaidBillingListView(string invoiceId)
    {
      // Create grid instance for PrimeBilling grid
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }
      var PrepaidBillingGrid = new AwbPrepaidBillingGrid(ViewDataConstants.PrimeBillingGrid, Url.Action("PrepaidBillingGridData", new { invoiceId }), isRejectionAllowed);
      ViewData[ViewDataConstants.PrimeBillingGrid] = PrepaidBillingGrid.Instance;

      return View("AwbPrepaidBillingList", InvoiceHeader);
    }

    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    [HttpGet]
    public ActionResult AwbPrepaidBillingView(string invoiceId, string transactionId)
    {
      var awbRecord = GetAwbRecord(transactionId, invoiceId);

      return View("AwbPrepaidRecordEdit", awbRecord);
    }
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    [HttpGet]
    public ActionResult AwbChargeCollectBillingView(string invoiceId, string transactionId)
    {
      var awbRecord = GetAwbRecord(transactionId, invoiceId);

      return View("AwbChargeCollectRecordEdit", awbRecord);
    }
    //ChargeCollectBillingListView
    public ActionResult AwbChargeCollectBillingList(string invoiceId)
    {
      //Create grid instance for PrimeBilling grid
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }
      var PrepaidBillingGrid = new AwbChargeCollectGrid(ViewDataConstants.PrimeBillingGrid, Url.Action("ChargeCollectBillingGridData", new { invoiceId }), isRejectionAllowed);
      ViewData[ViewDataConstants.PrimeBillingGrid] = PrepaidBillingGrid.Instance;
      //===========NK==========
     // CargoInvoice InvoiceHeader = new CargoInvoice();
      //=========================
      return View(InvoiceHeader);
    }


    public ActionResult AwbChargeCollectBillingListView(string invoiceId)
    {
      // Create grid instance for PrimeBilling grid
      bool isRejectionAllowed = false;
      if (ViewData[ViewDataConstants.BillingType] != null && ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
      {
        isRejectionAllowed = true;
      }
      var PrepaidBillingGrid = new AwbChargeCollectGrid(ViewDataConstants.PrimeBillingGrid, Url.Action("ChargeCollectBillingGridData", new { invoiceId }), isRejectionAllowed);
      ViewData[ViewDataConstants.PrimeBillingGrid] = PrepaidBillingGrid.Instance;

      return View("AwbChargeCollectBillingList", InvoiceHeader);
    }

    public JsonResult RMAwbPrepaidData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var rmAwbPrepaidGrid = new RejectionMemoAwbGrid(ControlIdConstants.RejectionMemoAwbPrepaidGrid, Url.Action("RMAwbPrepaidData", "Invoice", new { invoiceId }));
      List<RMAwb> rmawbPrepaidData = new List<RMAwb>();


      try
      {
        // return contactsGrid.DataBind(contacts.AsQueryable());
        // return null;
        return rmAwbPrepaidGrid.DataBind(rmawbPrepaidData.AsQueryable());
      }
      catch (ISBusinessException be)
      {
        ViewData["errorMessage"] = be.ErrorCode;
        return null;
      }
    }

    /// <summary>
    /// Display Rejection Memo in Invoice 
    /// </summary>
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult RMList(string invoiceId)
    {
      if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
      {
        return RedirectToAction("RMListView", new { invoiceId });
      }

      // Clear rejection memo data stored while navigating from Billing history.
      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        TempData.Remove(TempDataConstants.RejectedRecordIds);
      }

      // Create grid instance for Rejection Memo grid
      var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "Invoice", new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      return View(InvoiceHeader);
    }

    /// <summary>
    /// Display Rejection Memo in Invoice 
    /// </summary>
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult RMListView(string invoiceId)
    {
      // Create grid instance for Rejection Memo grid
      var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", "Invoice", new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      return View("RMList", InvoiceHeader);
    }

    /// <summary>
    /// Fetch data for Rejection Memo for Invoice and display it in grid
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult RMGridData(string invoiceId)
    {
      //Create grid instance and retrieve data from database
      var rejectionMemoGrid = new RejectionMemoGrid(ViewDataConstants.RejectionMemoListGrid, Url.Action("RMGridData", new { invoiceId }));

      var rejectionMemoCouponsList = _cargoInvoiceManager.GetRejectionMemoList(invoiceId);

      return rejectionMemoGrid.DataBind(rejectionMemoCouponsList.AsQueryable());
    }

    /// <summary>
    /// Delete Rejection memo
    /// </summary>
    /// <param name="transactionId"></param>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.CGO_REJECTION_MEMO)]
    public JsonResult RMDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        var isDeleted = _cargoInvoiceManager.DeleteRejectionMemoRecord(transactionId);

        details = GetDeleteMessage(isDeleted);

        return Json(details);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);

        return Json(details);
      }
    }

    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    [HttpGet]
    public ActionResult RMView(string invoiceId, string transactionId)
    {
      var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

      if (_cargoInvoiceManager.GetRejectionMemoAwbCount(transactionId) != 0)
      {
        ViewData[ViewDataConstants.BreakdownExists] = true;
      }
      else
      {
        ViewData[ViewDataConstants.BreakdownExists] = false;
      }

      var rejectionMemoAwbGrid = new RejectionMemoAwbGrid(ViewDataConstants.RejectionMemoAwbGrid, Url.Action("RejectionMemoAwbGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.RejectionMemoAwbGrid] = rejectionMemoAwbGrid.Instance;

      // This is done so as to not show the Reject icon when user navigates to RM listing page from correspondence.
      if (Request.QueryString["fc"] != null && Convert.ToBoolean(Request.QueryString["fc"]))
      {
        TempData[TempDataConstants.FromCorrespondence] = true;
      }

      return View("RMEdit", rejectionMemoRecord);
    }

    //public JsonResult GetSubmittedErrors(string invoiceId)
    //{
    //  var submittedErrors = ValidationManager.GetValidationErrors(invoiceId);
    //  var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
    //  return submittedErrorsGrid.DataBind(submittedErrors);
    //}
    /* public JsonResult GetSubmittedErrors(string invoiceId)
    {
      var submittedErrors = ValidationManager.GetValidationErrors(invoiceId);
      var submittedErrorsGrid = new SubmittedErrorsGrid(ControlIdConstants.SubmittedErrorsGridId, Url.Action("GetSubmittedErrors", new { invoiceId }));
      return submittedErrorsGrid.DataBind(submittedErrors);
    } */

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

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbPrepaidBillingCreate")]
    public ActionResult AwbPrepaidBillingCreateAndReturn(string invoiceId, AwbRecord awbRecord)
    {
        var logRefId = Guid.NewGuid();
        var log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
                           BillingCategorys.Cargo.ToString(), "Stage 1: AwbPrepaidBillingCreateAndReturn Start", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();

      log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
                         BillingCategorys.Cargo.ToString(), "Stage 2: couponAttachmentIds Selected", SessionUtil.UserId, logRefId.ToString());
      _referenceManager.LogDebugData(log);

      if (awbRecord.CarriageFromId!=null)
      awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
       awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
      awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
      awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();


      try
      {
          log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
                       BillingCategorys.Cargo.ToString(), "Stage 3: ValidateMandatoryFieldForAwbPrepaidBilling start", SessionUtil.UserId, logRefId.ToString());
          _referenceManager.LogDebugData(log);

        // Validate Server side mandatory field for Awb Prepaid billing record
        ValidateMandatoryFieldForAwbPrepaidBilling(awbRecord);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
                     BillingCategorys.Cargo.ToString(), "Stage 3: ValidateMandatoryFieldForAwbPrepaidBilling completed ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);


        string duplicateCouponErrorMessage;

        awbRecord.Attachments.Clear();
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        awbRecord.BillingCodeId = (int)BillingCode.AWBPrepaid;

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
                   BillingCategorys.Cargo.ToString(), "Stage 4: AddAwbRecord start ", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        _cargoInvoiceManager.AddAwbRecord(awbRecord, out duplicateCouponErrorMessage);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
                BillingCategorys.Cargo.ToString(), "Stage 4: AddAwbRecord completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        _cargoInvoiceManager.UpdateAwbAttachment(couponAttachmentIds, awbRecord.Id);

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
             BillingCategorys.Cargo.ToString(), "Stage 5: UpdateAwbAttachment completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);


        //  _cargoInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, awbRecord.Id);
        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateCouponErrorMessage);
        ShowSuccessMessage(Messages.AwbRecordCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData[TempDataConstants.AwbPrepaidRecord] = "";

        log = _referenceManager.GetDebugLog(DateTime.Now, "AwbPrepaidBillingCreateAndReturn ", this.ToString(),
           BillingCategorys.Cargo.ToString(), "Stage 6: AwbPrepaidBillingCreateAndReturn completed", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

        return RedirectToAction("AwbPrepaidBillingList", new { invoiceId });
        //return RedirectToAction("Create");
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

     // return View(awbRecord);
      return View("AwbPrepaidBillingCreate", awbRecord);

     // var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();
      
    }
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]

    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbPrepaidRecordEdit")]
    public ActionResult AwbPrepaidBillingEditAndReturn(string invoiceId, string transactionId, AwbRecord awbRecord)
    {
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      if (awbRecord.CarriageFromId != null)
        awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
        awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
        awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
        awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();
      try
      {
        awbRecord.Id = transactionId.ToGuid();
        awbRecord.LastUpdatedBy = SessionUtil.UserId;

        ValidateMandatoryFieldForAwbPrepaidBilling(awbRecord);
        //foreach (var tax in awbRecord.TaxBreakdown)
        //{
        //  tax.ParentId = couponRecord.Id;
        //}
        foreach (var vat in awbRecord.VatBreakdown)
        {
          vat.ParentId = awbRecord.Id;
        }
          //below code added on 2.11.11
        foreach (var otCharge in awbRecord.OtherChargeBreakdown)
        {
            otCharge.ParentId = awbRecord.Id;
        }
        string duplicateErrorMessage;
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        awbRecord.BillingCodeId = (int)BillingCode.AWBPrepaid;
        _cargoInvoiceManager.UpdateAwbRecord(awbRecord, out duplicateErrorMessage);

        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("AwbPrepaidBillingList", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("AwbPrepaidRecordEdit", awbRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbChargeCollectBillingCreate")]
    public ActionResult AwbChargeCollectBillingCreateAndReturn(string invoiceId, AwbRecord awbRecord)
    {


      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      if (awbRecord.CarriageFromId!=null)
      awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
      awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
      awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
      awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();


      try
      {
        string duplicateCouponErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 10 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        awbRecord.Attachments.Clear();
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        awbRecord.BillingCodeId = (int)BillingCode.AWBChargeCollect;
        _cargoInvoiceManager.AddAwbRecord(awbRecord, out duplicateCouponErrorMessage);
        _cargoInvoiceManager.UpdateAwbAttachment(couponAttachmentIds, awbRecord.Id);
        //  _cargoInvoiceManager.UpdateCouponRecordAttachment(couponAttachmentIds, awbRecord.Id);
        //ShowSuccessMessage(Messages.PrimeBillingCreateSuccessful + duplicateCouponErrorMessage);
        ShowSuccessMessage(Messages.AwbRecordCreateSuccessful);
        if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
          ShowErrorMessage(duplicateCouponErrorMessage, true);

        TempData[TempDataConstants.AwbPrepaidRecord] = "";

        return RedirectToAction("AwbChargeCollectBillingList", new { invoiceId });
        //return RedirectToAction("Create");
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      // return View(awbRecord);
      return View("AwbChargeCollectBillingCreate", awbRecord);

      // var couponAttachmentIds = couponRecord.Attachments.Select(attachment => attachment.Id).ToList();

    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbChargeCollectRecordEdit")]
    public ActionResult AwbChargeCollectBillingEditAndReturn(string invoiceId, string transactionId, AwbRecord awbRecord)
    {
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      if (awbRecord.CarriageFromId != null)
        awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
        awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
        awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
        awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();
      try
      {
        awbRecord.Id = transactionId.ToGuid();
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        //foreach (var tax in awbRecord.TaxBreakdown)
        //{
        //  tax.ParentId = couponRecord.Id;
        //}
        foreach (var vat in awbRecord.VatBreakdown)
        {
          vat.ParentId = awbRecord.Id;
        }

        foreach (var otCharge in awbRecord.OtherChargeBreakdown)
        {
            otCharge.ParentId = awbRecord.Id;
        }
        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 10 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        awbRecord.LastUpdatedBy = SessionUtil.UserId;
        awbRecord.BillingCodeId = (int)BillingCode.AWBChargeCollect;
        _cargoInvoiceManager.UpdateAwbRecord(awbRecord, out duplicateErrorMessage);

        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("AwbChargeCollectBillingList", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View("AwbChargeCollectRecordEdit", awbRecord);
    }

    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbChargeCollectBillingCreate")]
    public ActionResult AwbChargeCreditBillingDuplicate(string invoiceId, AwbRecord awbRecord)
    {

      #region""
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      if(awbRecord.CarriageFromId!=null)
      awbRecord.CarriageFromId = awbRecord.CarriageFromId.ToUpper();
      if (awbRecord.CarriageToId != null)
      awbRecord.CarriageToId = awbRecord.CarriageToId.ToUpper();
      if (awbRecord.ConsignmentOriginId != null)
      awbRecord.ConsignmentOriginId = awbRecord.ConsignmentOriginId.ToUpper();
      if (awbRecord.ConsignmentDestinationId != null)
      awbRecord.ConsignmentDestinationId = awbRecord.ConsignmentDestinationId.ToUpper();
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      awbRecord.BillingCodeId = (int)BillingCode.AWBChargeCollect;
      try
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 10 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        awbRecord.Attachments.Clear();

        _cargoInvoiceManager.AddAwbRecord(awbRecord, out duplicateErrorMessage);
        _cargoInvoiceManager.UpdateAwbAttachment(couponAttachmentIds, awbRecord.Id);
        
        awbRecord.AwbIssueingAirline = string.Empty;
        awbRecord.AwbSerialNumber = 0;
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull, false);
          if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        SetViewDataPageMode(PageMode.Clone);
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        // Increment sequence no by 1
        ModelState.SetModelValue("RecordSequenceWithinBatch",
                                 new ValueProviderResult(recSeqNum, (awbRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));
        
        awbRecord.Attachments.Clear(); // Attachments should not be duplicated. 
        SetViewDataPageMode(PageMode.Clone);
        ViewData["FromClone"] = true;
        awbRecord.Id = Guid.Empty;

        // Increment Sequence number on Save and Duplicate
        int batchNumber;
        int sequenceNumber;
        _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.AWBChargeCollect, out batchNumber, out sequenceNumber);

        awbRecord.BatchSequenceNumber = batchNumber;
        awbRecord.RecordSequenceWithinBatch = sequenceNumber;
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);

        //SCP164395:CGO-IS Web response stats. Moved the below code into catch section. Required in case of exception only
        // also check if invoice data is already populated or not 
        if (awbRecord.Invoice == null)
        {
            awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        }
          SetViewDataPageMode(PageMode.Create);
      }

      
      #endregion
      return View("AwbChargeCollectBillingCreate", awbRecord);
    }

    [ValidateAntiForgeryToken]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "AwbChargeCollectRecordEdit")]
    public ActionResult AwbChargeCreditBillingClone(string invoiceId, string transactionId, AwbRecord awbRecord)
    {
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      awbRecord.BillingCodeId = (int)BillingCode.AWBChargeCollect;
      var couponAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();

      awbRecord.Id = transactionId.ToGuid();

      foreach (var vat in awbRecord.VatBreakdown)
      {
        vat.ParentId = awbRecord.Id;
      }

      try
      {
        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 10 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        _cargoInvoiceManager.UpdateAwbRecord(awbRecord, out duplicateErrorMessage);
        awbRecord.AwbIssueingAirline = string.Empty;
        awbRecord.AwbSerialNumber = 0;
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull, false);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage);

        awbRecord.Attachments.Clear(); 
        SetViewDataPageMode(PageMode.Clone);

        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = false;
        ViewData["FromClone"] = true;
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        // Increment sequence no by 1
        ModelState.SetModelValue("RecordSequenceWithinBatch",
                                 new ValueProviderResult(recSeqNum, (awbRecord.RecordSequenceWithinBatch).ToString(), CultureInfo.InvariantCulture));
        
        awbRecord.Id = Guid.Empty;

        // Increment Sequence number on Save and Duplicate
        int batchNumber;
        int sequenceNumber;
        _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.AWBChargeCollect, out batchNumber, out sequenceNumber);

        awbRecord.BatchSequenceNumber = batchNumber;
        awbRecord.RecordSequenceWithinBatch = sequenceNumber;

        return View("AwbChargeCollectBillingCreate", awbRecord);
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(couponAttachmentIds);
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        SetViewDataPageMode(PageMode.Edit);
      }

      return View("AwbChargeCollectRecordEdit", awbRecord);
    }
    #region Billing Memo

    /// <summary>
    /// BMs the create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    [RestrictUnauthorizedUpdate]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    //[RestrictInvoiceUpdate(InvParamName ="invoiceId",IsJson = false,TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMCreate(string invoiceId)
    {
      // Retrieve Batch and Sequence number which will be pre populated, for Billing Memo
      int batchNumber;
      int sequenceNumber;
      _cargoInvoiceManager.GetBatchAndSequenceNumber(invoiceId.ToGuid(), (int)BillingCode.BillingMemo, out batchNumber, out sequenceNumber);

      SetViewDataPageMode(PageMode.Create);
      var billingMemoRecord = new CargoBillingMemo { Invoice = InvoiceHeader, InvoiceId = invoiceId.ToGuid(), LastUpdatedBy = SessionUtil.UserId, BatchSequenceNumber = batchNumber, RecordSequenceWithinBatch = sequenceNumber };
      if (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber) && TempData[TempDataConstants.CorrespondenceNumber] != null)
      {
        var correspondenceRefNumber = TempData[TempDataConstants.CorrespondenceNumber].ToString();
          
        KeepBillingHistoryDataInStore(true);
        var correspondenceManager = Ioc.Resolve<ICargoCorrespondenceManager>(typeof(ICargoCorrespondenceManager));
        var correspondence = correspondenceManager.GetRecentCorrespondenceDetails(Convert.ToInt64(correspondenceRefNumber));
        
        //INC 8863, I get an unexpected error occurred. 
        if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Closed || (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && correspondence.AuthorityToBill))
        {
          billingMemoRecord.ReasonCode = "6A";
        }
        else if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
        {
          billingMemoRecord.ReasonCode = "6B";
        }

        if (billingMemoRecord.Invoice.ListingCurrencyId != null && correspondence.CurrencyId != null)
        {
          try
          {         

            //SCP219674 : InvalidAmountToBeSettled Validation
            decimal netAmountBilled = billingMemoRecord.NetBilledAmount.HasValue ? billingMemoRecord.NetBilledAmount.Value : 0;
            var isValid = ReferenceManager.ValidateCorrespondenceAmounttobeSettled(billingMemoRecord.Invoice,
                                                                               ref netAmountBilled,
                                                                               correspondence.CurrencyId.Value,
                                                                               correspondence.AmountToBeSettled);
            billingMemoRecord.NetBilledAmount = billingMemoRecord.BilledTotalWeightCharge = netAmountBilled;
          }
          catch (ISBusinessException businessException)
          {
            ShowErrorMessage(businessException.ErrorCode);
          }
        }
        billingMemoRecord.CorrespondenceReferenceNumber = Convert.ToInt64(correspondenceRefNumber);

        ViewData[ViewDataConstants.FromBillingHistory] = true;
      }

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      if (billingMemoRecord.CorrespondenceReferenceNumber == 0)
      {
        billingMemoRecord.CorrespondenceReferenceNumber = new Business.Web.CargoInvoiceManager().GetCargoDatabaseCorrRefNumber(billingMemoRecord.Id);
      }
      return View(billingMemoRecord);
    }

    /// <summary>
    /// BMs the create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMCreate(string invoiceId, CargoBillingMemo record)
    {
        var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
        
        try
        {
            record.LastUpdatedBy = SessionUtil.UserId;
            record.Attachments.Clear();
            record.BillingCode = (int)BillingCode.BillingMemo;
            if (record.NetBilledAmount == null) record.NetBilledAmount = 0;

            // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
            // On Create/Edit screen for BM with reason code other than 6A/6B, correspondence ref no value will be stored as NULL in database.
            // Here we are making as -1, later it will update to NULL by database trigger.
            _cargoInvoiceManager.AddBillingMemoRecord(record, Convert.ToString(Request.Form["UserCorrRefNo"]).Equals("-1"));

            _cargoInvoiceManager.UpdateBillingMemoAttachment(billingMemoAttachmentIds, record.Id);
            TempData.Clear();
            ShowSuccessMessage(Messages.BMCreateSuccessful);
            // On clicking Save in case of adding  a new billing memo record, 
            // if the selected reason code mandates coupon breakdown, then system should automatically open the BM Coupon Breakdown screen for data capture.
            //if (record.CouponAwbBreakdownMandatory)
            //    return RedirectToAction("BMCouponCreate", new { invoiceId, transactionId = record.Id.Value() });

            return RedirectToAction("BMEdit", new { invoiceId, transactionId = record.Id.Value() });
        }
        catch (ISBusinessException exception)
        {
            ShowErrorMessage(exception.ErrorCode);
            record.Attachments = _cargoInvoiceManager.GetBillingMemoAttachments(billingMemoAttachmentIds);
        }

      //KeepBillingHistoryDataInStore(true);

      if (TempData.ContainsKey("correspondenceNumber"))
      {
        ViewData[ViewDataConstants.FromBillingHistory] = true;
      }
      record.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(record);
    }

    /// <summary>
    /// BMs the edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    public ActionResult BMEdit(string invoiceId, string transactionId)
    {
      var billingMemoRecord = GetBillingMemoRecord(invoiceId, transactionId);
      ViewData[ViewDataConstants.BreakdownExists] = _cargoInvoiceManager.GetBillingMemoAwbCount(transactionId) > 0 ? true : false;
      var billingMemoAwbGrid = new CargoBMAwbGrid(ViewDataConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;

      // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
      if (billingMemoRecord.CorrespondenceReferenceNumber == 0)
      {
        billingMemoRecord.CorrespondenceReferenceNumber = new Business.Web.CargoInvoiceManager().GetCargoDatabaseCorrRefNumber(billingMemoRecord.Id);
      }

      return View(billingMemoRecord);
    }

    /// <summary>
    /// BMs the edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMEdit(string invoiceId, string transactionId, CargoBillingMemo record)
    {
        var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
        try
        {
            record.Id = transactionId.ToGuid();
            record.LastUpdatedBy = SessionUtil.UserId;
            record.BillingCode = (int)BillingCode.BillingMemo;
            //Assign parent id for VAt records
            foreach (var vat in record.BillingMemoVat)
            {
                vat.ParentId = record.Id;
            }

            // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
            // On Create/Edit screen for BM with reason code other than 6A/6B, correspondence ref no value will be stored as NULL in database.
            // Here we are making as -1, later it will update to NULL by database trigger.
            _cargoInvoiceManager.UpdateBillingMemoRecord(record, Convert.ToString(Request.Form["UserCorrRefNo"]).Equals("-1"));

            ShowSuccessMessage(Messages.BMUpdateSuccessful);
            return RedirectToAction("BMEdit", new { invoiceId, transactionId });
        }
        catch (ISBusinessException be)
        {
            ShowErrorMessage(be.ErrorCode);
            record.Attachments = _cargoInvoiceManager.GetBillingMemoAttachments(billingMemoAttachmentIds);

        }
        finally
        {
            SetViewDataPageMode(PageMode.Edit);
        }

        record.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        // Initialize BMCoupon grid
        var billingMemoAwbGrid = new CargoBMAwbGrid(ViewDataConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbGridData", "Invoice", new { transactionId }));
        ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;
        return View(record);
    }

    /// <summary>
    /// BMs the edit and add new.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="billingMemoRecord">The billing memo record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMEdit")]
    public ActionResult BMEditAndAddNew(string invoiceId, string transactionId, CargoBillingMemo billingMemoRecord)
    {
        var billingMemoAttachmentIds = billingMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();
        try
        {
            billingMemoRecord.Id = transactionId.ToGuid();
            billingMemoRecord.LastUpdatedBy = SessionUtil.UserId;
            billingMemoRecord.BillingCode = (int)BillingCode.BillingMemo;
            //Assign parent id for Vat records
            foreach (var vat in billingMemoRecord.BillingMemoVat)
            {
                vat.ParentId = billingMemoRecord.Id;
            }

            // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
            // On Create/Edit screen for BM with reason code other than 6A/6B, correspondence ref no value will be stored as NULL in database.
            // Here we are making as -1, later it will update to NULL by database trigger.
            _cargoInvoiceManager.UpdateBillingMemoRecord(billingMemoRecord, Convert.ToString(Request.Form["UserCorrRefNo"]).Equals("-1"));

            ShowSuccessMessage(Messages.BMUpdateSuccessful);
            return RedirectToAction("BMCreate", new { invoiceId, transactionId });
        }
        catch (ISBusinessException be)
        {
            ShowErrorMessage(be.ErrorCode);
            billingMemoRecord.Attachments = _cargoInvoiceManager.GetBillingMemoAttachments(billingMemoAttachmentIds);
            
            // CMP#673: Validation on Correspondence Reference Number in PAX/CGO Billing Memos
            // Below two lines of code is not related to this CMP673 but, in case of exception this grid need to be initialize.
            // Initialize BMCoupon grid
            var billingMemoAwbGrid = new CargoBMAwbGrid(ViewDataConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbGridData", "Invoice", new { transactionId }));
            ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;
        }
        finally
        {
            SetViewDataPageMode(PageMode.Edit);
        }

        billingMemoRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        ViewData[ViewDataConstants.BreakdownExists] = _cargoInvoiceManager.GetBillingMemoAwbCount(transactionId) > 0 ? true : false;
      
        // Initialize BMCoupon grid
        //var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
        //ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;
        //ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        //ViewData[ViewDataConstants.BreakdownExists] = _nonSamplingInvoiceManager.GetBillingMemoCouponCount(transactionId) > 0 ? true : false;

        // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
        if (billingMemoRecord.CorrespondenceReferenceNumber == 0)
        {
          billingMemoRecord.CorrespondenceReferenceNumber = new Business.Web.CargoInvoiceManager().GetCargoDatabaseCorrRefNumber(billingMemoRecord.Id);
        }

        return View("BMEdit", billingMemoRecord);
    }

    /// <summary>
    /// Delete billing memo
    /// </summary>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.CGO_BILLING_MEMO)]
    public JsonResult BMDelete(string transactionId)
    {
        UIMessageDetail details;
        try
        {
            //Delete record
            var isDeleted = _cargoInvoiceManager.DeleteBillingMemoRecord(transactionId);

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
    /// Gets billing memo list and displays in grid for given invoice.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult BMList(string invoiceId)
    {
        if (InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
        {
            return RedirectToAction("BMListView", new { invoiceId });
        }

        // Create grid instance for Billing Memo grid
        var billingMemoGrid = new CargoBMListGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
        ViewData[ViewDataConstants.BillingMemoGrid] = billingMemoGrid.Instance;

        return View(InvoiceHeader);
    }

    /// <summary>
    /// BMs the list view.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns></returns>
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult BMListView(string invoiceId)
    {
        // Create grid instance for Billing Memo grid
        var billingMemoGrid = new CargoBMListGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
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
        var billingMemoGrid = new CargoBMListGrid(ControlIdConstants.TransactionGridId, Url.Action(BillingMemoGridAction, new { invoiceId }));
        var billingMemoCoupons = _cargoInvoiceManager.GetBillingMemoList(invoiceId).AsQueryable();

        return billingMemoGrid.DataBind(billingMemoCoupons);
    }

    [HttpGet]
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult BMView(string invoiceId, string transactionId)
    {
      
      var billingMemoRecord = GetBillingMemoRecord(invoiceId, transactionId);
      ViewData[ViewDataConstants.BreakdownExists] = _cargoInvoiceManager.GetBillingMemoAwbCount(transactionId) > 0 ? true : false;
      // Initialize BMCoupon grid
      var billingMemoAwbGrid = new CargoBMAwbGrid(ViewDataConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbGridData", "Invoice", new { transactionId }));
      ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;

      return View("BMEdit", billingMemoRecord);
    }
    [HttpGet]
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult BMAwbPrepaidView(string invoiceId, string transactionId, string couponId)
    {
      var bmAwbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);



      return View("BMAwbPrepaidEdit", bmAwbRecord);
    }
    [HttpGet]
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
    public ActionResult BMAwbChargeCollectView(string invoiceId, string transactionId, string couponId)
    {
      var bmAwbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);
      return View("BMAwbChargeCollectEdit", bmAwbRecord);
    }
    private CargoBillingMemo GetBillingMemoRecord(string invoiceId, string transactionId)
    {
        var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        //var billingMemo = new CargoBillingMemo();
        billingMemo.Invoice = InvoiceHeader;
        billingMemo.LastUpdatedBy = SessionUtil.UserId;

      //var transactionTypeId = billingMemo.ReasonCode == "6A"
      //                              ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
      //                              : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      //var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(billingMemo.ReasonCode, transactionTypeId).CouponAwbBreakdownMandatory;

      // Set Coupon breakdown value
      //billingMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      return billingMemo;
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
    public JsonResult BillingMemoAttachmentUpload(string invoiceId, string transactionId)
    {
      string files = string.Empty;
      var attachments = new List<CargoBillingMemoAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      string message;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;

      try
      {
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues(AwbBillingAttachmentUpload)
          //var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
          Logger.Info("Started execution for method BillingMemoAttachmentUpload for invoice ID" + invoiceId);
          var invoice = _cargoInvoiceManager.GetInvoiceDetails(invoiceId);
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
          Logger.Info("Checking File is valid or not");
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoInvoiceManager.IsDuplicateBillingMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
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

          if (fileUploadHelper.SaveFile())
          {
              Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new CargoBillingMemoAttachment
            {
              Id = fileUploadHelper.FileServerName,
              OriginalFileName = fileUploadHelper.FileOriginalName,
              FileSize = fileUploadHelper.FileToSave.ContentLength,
              LastUpdatedBy = SessionUtil.UserId,
              ServerId = fileUploadHelper.FileServerInfo.ServerId,
              FileStatus = FileStatusType.Received,
              FilePath = fileUploadHelper.FileRelativePath
            };

            attachment = _cargoInvoiceManager.AddBillingMemoAttachment(attachment);
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new User();
            }
            // assign user info from session and file server info.
            attachment.UploadedBy.Id = SessionUtil.UserId;
            attachment.UploadedBy.FirstName = SessionUtil.Username;
            attachment.FileServer = fileUploadHelper.FileServerInfo;
            isUploadSuccess = true;
            attachments.Add(attachment);
            Logger.Info("Attachment Entry is inserted successfully in database");
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
    /// Billings the memo attachment download.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download)]
    [HttpGet]
    public FileStreamResult BillingMemoAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetBillingMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// BMs the awb prepaid data.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    public JsonResult BillingMemoAwbGridData(string transactionId)
    {
        // Create grid instance and retrieve data from database
        var billingMemoAwbGrid = new CargoBMAwbGrid(ControlIdConstants.BillingMemoAwbGrid, Url.Action("BillingMemoAwbData", "Invoice", new { transactionId }));
        ViewData[ViewDataConstants.BillingMemoAwbGrid] = billingMemoAwbGrid.Instance;
        var billingMemoAwbs = _cargoInvoiceManager.GetBMAwbList(transactionId).AsQueryable();
        try
        {
            // return contactsGrid.DataBind(contacts.AsQueryable());
            // return null;
            return billingMemoAwbGrid.DataBind(billingMemoAwbs);
        }
        catch (ISBusinessException be)
        {
            ViewData["errorMessage"] = be.ErrorCode;
            return null;
        }

    }

    
    /// <summary>
    /// BMs the awb prepaid create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    public ActionResult BMAwbPrepaidCreate(string invoiceId, string transactionId)
    {
        SetViewDataPageMode(PageMode.Create);

        var bmAwb = new CargoBillingMemoAwb { LastUpdatedBy = SessionUtil.UserId };
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = InvoiceHeader;
        billingMemoRecord.BillingCode = (int)BillingCode.AWBPrepaid;
        // Set Airline flight designator to Billing Member name
        //bmAwb.AwbIssueingAirline = InvoiceHeader.BillingMember.MemberCodeAlpha;
        bmAwb.BillingMemoRecord = billingMemoRecord;

        ViewData["IsAddNewBMCoupon"] = TempData["IsAddNewBMCoupon"] != null && Convert.ToBoolean(TempData["IsAddNewBMCoupon"]);

        return View(bmAwb);
    }

    /// <summary>
    /// Gets the coupon of corresponding billingMemoId and its coupon record.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMAwbPrepaidCreate(string invoiceId, string transactionId, CargoBillingMemoAwb record)
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
            record.BillingMemoId = transactionId.ToGuid();
            record.LastUpdatedBy = SessionUtil.UserId;
            record.AwbBillingCode = (int) BillingCode.AWBPrepaid;
            record.Attachments.Clear();
            if (record.AwbSerialNumber > 0)
            {
                var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7, '0');
                var awbnumber = (awbnumberDisplay.Substring(0, 7));
                //$('#AwbSerialNumber').val(awbnumber);
                record.AwbSerialNumber = Convert.ToInt32(awbnumber);
            }
            string duplicateCouponErrorMessage;
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 13 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.AddBMAwbRecord(record,invoiceId,out duplicateCouponErrorMessage);
            _cargoInvoiceManager.UpdateBMAwbAttachment(couponAttachmentIds, record.Id);
            //ShowSuccessMessage("Billing Memo AWB record created successfully." + duplicateCouponErrorMessage);
            //        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
            ShowSuccessMessage(Messages.BMCouponCreateSuccessful);
            if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
                ShowErrorMessage(duplicateCouponErrorMessage, true);

            TempData["IsAddNewBMCoupon"] = true;
            SetViewDataPageMode(PageMode.Create);

            // Set ViewData, "IsPostback" to false
            ViewData["IsPostback"] = false;
            return RedirectToAction("BMAwbPrepaidCreate", new { transactionId });
        }
        catch (ISBusinessException businessException)
        {
            ShowErrorMessage(businessException.ErrorCode);
            ViewData["IsPostback"] = true;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
        }
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = invoice;
        record.BillingMemoRecord = billingMemoRecord;

        return View(record);
    }

    
    /// <summary>
    /// BMs the awb prepaid create and return.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMAwbPrepaidCreate")]
    public ActionResult BMAwbPrepaidCreateAndReturn(string invoiceId, string transactionId, CargoBillingMemoAwb record)
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
            record.BillingMemoId = transactionId.ToGuid();
            record.LastUpdatedBy = SessionUtil.UserId;
            record.AwbBillingCode = (int)BillingCode.AWBPrepaid;
            record.Attachments.Clear();
            if (record.AwbSerialNumber > 0)
            {
                var awbnumberDisplay = Convert.ToString(record.AwbSerialNumber).PadLeft(7,'0');
                var awbnumber = (awbnumberDisplay.Substring(0, 7));
                //$('#AwbSerialNumber').val(awbnumber);
                record.AwbSerialNumber = Convert.ToInt32(awbnumber);
            }
            string duplicateCouponErrorMessage;
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 13 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.AddBMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
            _cargoInvoiceManager.UpdateBMAwbAttachment(couponAttachmentIds, record.Id);
            //ShowSuccessMessage("Billing Memo AWB record created successfully. " + duplicateCouponErrorMessage);
            //        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
            ShowSuccessMessage(Messages.BMCouponCreateSuccessful);
            if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
                ShowErrorMessage(duplicateCouponErrorMessage, true);

            TempData["IsAddNewBMCoupon"] = true;

            return RedirectToAction("BMEdit", new {invoiceId, transactionId });
        }
        catch (ISBusinessException businessException)
        {
            ShowErrorMessage(businessException.ErrorCode);
            ViewData["IsPostback"] = true;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
        }
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = invoice;
        record.BillingMemoRecord = billingMemoRecord;

        return View("BMAwbPrepaidCreate",record);
    }

    /// <summary>
    /// BMs the awb prepaid create and duplicate.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMAwbPrepaidCreate")]
    public ActionResult BMAwbPrepaidCreateAndDuplicate(string invoiceId, string transactionId, CargoBillingMemoAwb record)
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
            record.BillingMemoId = transactionId.ToGuid();
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
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 13 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.AddBMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
            _cargoInvoiceManager.UpdateBMAwbAttachment(couponAttachmentIds, record.Id);
            //ShowSuccessMessage("Billing Memo AWB record created successfully." + duplicateCouponErrorMessage);
            //        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
            ShowSuccessMessage(Messages.BMCouponCreateSuccessful);
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
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
        }
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = invoice;
        record.BillingMemoRecord = billingMemoRecord;
        return View("BMAwbPrepaidCreate", record);
    }

    /// <summary>
    /// BMs the awb edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult BMAwbEdit(string invoiceId, string transactionId, string couponId)
    {
        var awbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);
        if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
        {
            Session["helplinkurl"] = "Bm_Awb_Prepaid_edit";
            return View("BMAwbPrepaidEdit", awbRecord);
        }

        Session["helplinkurl"] = "Bm_Awb_chargeCollectEdit_edit";
        return View("BMAwbChargeCollectEdit", awbRecord);
    }
    //[ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult BMAwbView(string invoiceId, string transactionId, string couponId)
    {
        SetViewDataPageMode(PageMode.View);
      var awbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);
      if (awbRecord.AwbBillingCode == Convert.ToInt32(BillingCode.AWBPrepaid))
      {
          Session["helplinkurl"] = "Bm_Awb_Prepaid_view";
          return View("BMAwbPrepaidEdit", awbRecord);
      }
      Session["helplinkurl"] = "Bm_Awb_ChargeCollect_view";
        return View("BMAwbChargeCollectEdit", awbRecord);
    }

    /// <summary>
    /// BMs the awb delete.
    /// </summary>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.CGO_BM_AWB)]
    public JsonResult BMAwbDelete(string transactionId)
    {
        UIMessageDetail details;
        try
        {
            // Delete record
            Guid invoiceId;
            Guid billingMemoId;
            var isDeleted = _cargoInvoiceManager.DeleteBillingMemoAwbRecord(transactionId, out billingMemoId, out invoiceId);

            details = isDeleted ? new UIMessageDetail { IsFailed = false, Message = Messages.DeleteSuccessful, RedirectUrl = Url.Action("BMEdit", new { invoiceId, transactionId = billingMemoId }), isRedirect = true } : new UIMessageDetail { IsFailed = true, Message = Messages.DeleteFailed };
            if (isDeleted)
                ShowSuccessMessage(Messages.DeleteSuccessful, true);
        }
        catch (ISBusinessException ex)
        {
            details = HandleDeleteException(ex.ErrorCode);
        }

        return Json(details);
    }

    /// <summary>
    /// BMs the awb prepaid edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    public ActionResult BMAwbPrepaidEdit(string invoiceId, string transactionId, string couponId)
    {
        var bmAwbRecord = GetBMAwbRecord(invoiceId, transactionId,couponId);

        //ViewData[ViewDataConstants.BreakdownExists] = _cargoInvoiceManager.GetBillingMemoCouponCount(transactionId) > 0 ? true : false;

        //Initialize BMCoupon grid
        // var billingMemoCouponGrid = new BillingMemoCouponGrid(ViewDataConstants.BillingMemoCouponGrid, Url.Action("BillingMemoCouponGridData", "Invoice", new { transactionId }));
        //ViewData[ViewDataConstants.BillingMemoCouponGrid] = billingMemoCouponGrid.Instance;

        return View(bmAwbRecord);
    }

    /// <summary>
    /// Gets the BM awb record.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The bm awb id.</param>
    /// <returns></returns>
    private CargoBillingMemoAwb GetBMAwbRecord(string invoiceId, string transactionId, string couponId)
    {
        var bmAwb = _cargoInvoiceManager.GetBMemoAwbRecordDetails(couponId);
        //var billingMemo = new CargoBillingMemo();
        bmAwb.BillingMemoRecord.Invoice = InvoiceHeader;
        bmAwb.LastUpdatedBy = SessionUtil.UserId;

        //var transactionTypeId = billingMemo.ReasonCode == "6A"
        //                              ? (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill
        //                              : billingMemo.ReasonCode == "6B" ? (int)TransactionType.PasNsBillingMemoDueToExpiry : (int)TransactionType.BillingMemo;
        // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
        //var isCouponBreakdownMandatory = _referenceManager.GetReasonCode(billingMemo.ReasonCode, transactionTypeId).CouponAwbBreakdownMandatory;

        // Set Coupon breakdown value
        //billingMemo.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

        return bmAwb;
    }
    
    /// <summary>
    /// BMs the awb prepaid edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The bm awb id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMAwbPrepaidEdit(string invoiceId, string transactionId, string couponId, CargoBillingMemoAwb record)
    {
        var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
        try
        {
            record.BillingMemoId = transactionId.ToGuid();
            record.LastUpdatedBy = SessionUtil.UserId;
            record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);
            //Assign parent id for VAt records
            EditBMAwb(record, couponId, invoiceId);

            ShowSuccessMessage(Messages.BMUpdateSuccessful);
            return RedirectToAction("BMAwbPrepaidCreate", new { invoiceId, transactionId, couponId });
        }
        catch (ISBusinessException be)
        {
            ShowErrorMessage(be.ErrorCode);
            var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
            var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            billingMemo.Invoice = invoice;
            record.BillingMemoRecord = billingMemo;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(billingMemoAttachmentIds);
        }
        finally
        {
            SetViewDataPageMode(PageMode.Edit);
        }

        return View(record);
    }

    private void EditBMAwb(CargoBillingMemoAwb bmAwb, string couponId, string invoiceId)
    {
        bmAwb.Id = couponId.ToGuid();
        bmAwb.LastUpdatedBy = SessionUtil.UserId;
        // Assign parent rejection coupon record id to tax records
        foreach (var otherCharge in bmAwb.OtherCharges)
        {
            otherCharge.ParentId = bmAwb.Id;
        }
        // Assign parent rejection coupon record id to vat records
        foreach (var vat in bmAwb.AwbVat)
        {
            vat.ParentId = bmAwb.Id;
        }

        // Assign parent BM AWB record id to prorate ladder detail records.
        foreach (var prorateLadderDetail in bmAwb.ProrateLadder)
        {
          prorateLadderDetail.ParentId = bmAwb.Id;
        }

        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 13 and 14 */
        MemberManager.ValidateIssuingAirline(bmAwb.AwbIssueingAirline);
        int vatRecordCountBefore = bmAwb.AwbVat.Count;
        int otherChargeCountBefore = bmAwb.OtherCharges.Count;
        _cargoInvoiceManager.UpdateBMAwbRecord(bmAwb, invoiceId, out duplicateErrorMessage);
         int vatRecordCountAfter = bmAwb.AwbVat.Count;
        int otherChargeCountAfter = bmAwb.OtherCharges.Count;

        ShowSuccessMessages(Messages.BMCouponUpdateSuccessful, vatRecordCountBefore, vatRecordCountAfter, otherChargeCountBefore, otherChargeCountAfter);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
            ShowErrorMessage(duplicateErrorMessage, true);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMAwbPrepaidCreate")]
    public ActionResult BMPrepaidAwbClone(string couponId, string transactionId, string invoiceId, CargoBillingMemoAwb bmAwb)
    {
        bmAwb.BillingMemoId = transactionId.ToGuid();
        var couponAttachmentIds = bmAwb.Attachments.Select(attachment => attachment.Id).ToList();
        var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);

        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        billingMemo.Invoice = invoice;
        bmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);

        try
        {
            bmAwb.Id = couponId.ToGuid();
            bmAwb.LastUpdatedBy = SessionUtil.UserId;
            // Assign parent rejection coupon record id to tax records
            foreach (var awbOtherCharge in bmAwb.OtherCharges)
            {
                awbOtherCharge.ParentId = bmAwb.Id;
            }
            // Assign parent rejection coupon record id to vat records
            foreach (var vat in bmAwb.AwbVat)
            {
                vat.ParentId = bmAwb.Id;
            }
            string duplicateErrorMessage;
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 13 */
            MemberManager.ValidateIssuingAirline(bmAwb.AwbIssueingAirline);
            _cargoInvoiceManager.UpdateBMAwbRecord(bmAwb, invoiceId, out duplicateErrorMessage);
            bmAwb.BillingMemoRecord = billingMemo;

            ShowSuccessMessage(Messages.BMCouponUpdateSuccessful);
            if (!String.IsNullOrEmpty(duplicateErrorMessage))
                ShowErrorMessage(duplicateErrorMessage, true);

            bmAwb.Attachments.Clear(); // attachments should not be duplicated.
            SetViewDataPageMode(PageMode.Clone);

            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData["IsPostback"] = false;
            // Set ViewData "FromClone" to true
            ViewData["FromClone"] = true;

            return View("BMAwbPrepaidCreate", bmAwb);
        }
        catch (ISBusinessException exception)
        {
            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData["IsPostback"] = true;
            ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
            bmAwb.BillingMemoRecord = billingMemo;
            bmAwb.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
            SetViewDataPageMode(PageMode.Edit);
        }

        return View("BMAwbPrepaidEdit", bmAwb);
    }

    /// <summary>
    /// 
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMEdit")]
    public ActionResult BMPrepaidAwbEditAndReturn(string couponId, string transactionId, string invoiceId, CargoBillingMemoAwb bmAwb)
    {
        bmAwb.BillingMemoId = transactionId.ToGuid();
        var awbAttachmentIds = bmAwb.Attachments.Select(attachment => attachment.Id).ToList();
        bmAwb.AwbBillingCode = Convert.ToInt32(BillingCode.AWBPrepaid);

        try
        {
            EditBMAwb(bmAwb, couponId, invoiceId);
            //  TempData["RMCouponRecord"] = "";
            return RedirectToAction("BMEdit", new { invoiceId, transactionId });
        }
        catch (ISBusinessException exception)
        {
            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData["IsPostback"] = true;
            ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

            var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
            billingMemo.Invoice = invoice;
            bmAwb.BillingMemoRecord = billingMemo;
            bmAwb.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(awbAttachmentIds);
            SetViewDataPageMode(PageMode.Edit);
        }

        return View("BMAwbPrepaidEdit", bmAwb);
    }


    /// <summary>
    /// BMs the awb charge collect edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <returns></returns>
    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    public ActionResult BMAwbChargeCollectEdit(string invoiceId, string transactionId, string couponId)
    {
        var bmAwbRecord = GetBMAwbRecord(invoiceId, transactionId, couponId);
        return View(bmAwbRecord);
    }

    /// <summary>
    /// BMs the awb charge collect edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="couponId">The coupon id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMAwbChargeCollectEdit(string invoiceId, string transactionId, string couponId, CargoBillingMemoAwb record)
    {
        var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
        try
        {
            record.BillingMemoId = transactionId.ToGuid();
            record.LastUpdatedBy = SessionUtil.UserId;
            record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);
            EditBMAwb(record, couponId, invoiceId);

            ShowSuccessMessage(Messages.BMUpdateSuccessful);
            return RedirectToAction("BMAwbChargeCollectCreate", new { invoiceId, transactionId, couponId });
        }
        catch (ISBusinessException be)
        {
            ShowErrorMessage(be.ErrorCode);
            ViewData["IsPostback"] = true;
            var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
            var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            billingMemo.Invoice = invoice;
            record.BillingMemoRecord = billingMemo;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(billingMemoAttachmentIds);
        }
        finally
        {
            SetViewDataPageMode(PageMode.Edit);
        }
        return View(record);
    }

    /// <summary>
    /// Updates coupon record and allows user to create new record using same information of this coupon.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMAwbChargeCollectCreate")]
    public ActionResult BMChargeCollectAwbClone(string couponId, string transactionId, string invoiceId, CargoBillingMemoAwb record)
    {
        record.BillingMemoId = transactionId.ToGuid();
        var billingMemoAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
        var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);

        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        billingMemo.Invoice = invoice;
        record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);

        try
        {
            record.Id = couponId.ToGuid();
            record.LastUpdatedBy = SessionUtil.UserId;
            // Assign parent rejection coupon record id to tax records
            foreach (var awbOtherCharge in record.OtherCharges)
            {
                awbOtherCharge.ParentId = record.Id;
            }
            // Assign parent rejection coupon record id to vat records
            foreach (var vat in record.AwbVat)
            {
                vat.ParentId = record.Id;
            }
            string duplicateErrorMessage;
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 14 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.UpdateBMAwbRecord(record, invoiceId, out duplicateErrorMessage);
            record.BillingMemoRecord = billingMemo;

            ShowSuccessMessage(Messages.BMCouponUpdateSuccessful);
            if (!String.IsNullOrEmpty(duplicateErrorMessage))
                ShowErrorMessage(duplicateErrorMessage, true);

            record.Attachments.Clear(); // attachments should not be duplicated.
            SetViewDataPageMode(PageMode.Clone);

            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData["IsPostback"] = false;
            // Set ViewData "FromClone" to true
            ViewData["FromClone"] = true;

            return View("BMAwbChargeCollectCreate", record);
        }
        catch (ISBusinessException exception)
        {
            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData["IsPostback"] = true;
            ShowCustomErrorMessage(exception.ErrorCode, exception.Message);
            record.BillingMemoRecord = billingMemo;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(billingMemoAttachmentIds);
            SetViewDataPageMode(PageMode.Edit);
        }

        return View("BMAwbChargeCollectEdit", record);
    }

    /// <summary>
    /// 
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMEdit")]
    public ActionResult BMChargeCollectAwbEditAndReturn(string couponId, string transactionId, string invoiceId, CargoBillingMemoAwb record)
    {
        record.BillingMemoId = transactionId.ToGuid();
        var awbAttachmentIds = record.Attachments.Select(attachment => attachment.Id).ToList();
        record.AwbBillingCode = Convert.ToInt32(BillingCode.AWBChargeCollect);

        try
        {
            EditBMAwb(record, couponId, invoiceId);
            //  TempData["RMCouponRecord"] = "";
            return RedirectToAction("BMEdit", new { invoiceId, transactionId });
        }
        catch (ISBusinessException exception)
        {
            // Set ViewData, "IsPostback" to true, if exception occurs
            ViewData["IsPostback"] = true;
            ShowCustomErrorMessage(exception.ErrorCode, exception.Message);

            var billingMemo = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
            var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
            billingMemo.Invoice = invoice;
            record.BillingMemoRecord = billingMemo;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(awbAttachmentIds);
            SetViewDataPageMode(PageMode.Edit);
        }

        return View("BMAwbChargeCollectEdit", record);
    }

    /// <summary>
    /// BMs the awb charge collect create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    public ActionResult BMAwbChargeCollectCreate(string invoiceId, string transactionId)
    {
        SetViewDataPageMode(PageMode.Create);

        var bmAwb = new CargoBillingMemoAwb { LastUpdatedBy = SessionUtil.UserId };
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = InvoiceHeader;
        billingMemoRecord.BillingCode = (int)BillingCode.AWBChargeCollect;
        // Set Airline flight designator to Billing Member name
        //bmAwb.AwbIssueingAirline = InvoiceHeader.BillingMember.MemberCodeAlpha;
        bmAwb.BillingMemoRecord = billingMemoRecord;

        ViewData["IsAddNewBMCoupon"] = TempData["IsAddNewBMCoupon"] != null && Convert.ToBoolean(TempData["IsAddNewBMCoupon"]);

        return View(bmAwb);
    }

    /// <summary>
    /// BMs the awb charge collect create.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult BMAwbChargeCollectCreate(string invoiceId, string transactionId, CargoBillingMemoAwb record)
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
            record.BillingMemoId = transactionId.ToGuid();
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
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 14 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.AddBMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
            _cargoInvoiceManager.UpdateBMAwbAttachment(couponAttachmentIds, record.Id);
            //ShowSuccessMessage("Billing Memo AWB record created successfully.");
            //        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
            ShowSuccessMessage(Messages.BMCouponCreateSuccessful);
            if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
                ShowErrorMessage(duplicateCouponErrorMessage, true);

            TempData["IsAddNewBMCoupon"] = true;

            return RedirectToAction("BMAwbChargeCollectCreate", new { transactionId });
        }
        catch (ISBusinessException businessException)
        {
            ShowErrorMessage(businessException.ErrorCode);
            ViewData["IsPostback"] = true;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
        }
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = invoice;
        record.BillingMemoRecord = billingMemoRecord;

        return View(record);
    }

    /// <summary>
    /// BMs the awb charge collect create and return.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMAwbChargeCollectCreate")]
    public ActionResult BMAwbChargeCollectCreateAndReturn(string invoiceId, string transactionId, CargoBillingMemoAwb record)
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
            record.BillingMemoId = transactionId.ToGuid();
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
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 14 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.AddBMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
            _cargoInvoiceManager.UpdateBMAwbAttachment(couponAttachmentIds, record.Id);
            //ShowSuccessMessage("Billing Memo AWB record created successfully.");
            //        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
            ShowSuccessMessage(Messages.BMCouponCreateSuccessful);
            if (!String.IsNullOrEmpty(duplicateCouponErrorMessage))
                ShowErrorMessage(duplicateCouponErrorMessage, true);

            TempData["IsAddNewBMCoupon"] = true;

            return RedirectToAction("BMEdit", new { invoiceId, transactionId });
        }
        catch (ISBusinessException businessException)
        {
            ShowErrorMessage(businessException.ErrorCode);
            ViewData["IsPostback"] = true;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
        }
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = invoice;
        record.BillingMemoRecord = billingMemoRecord;

        return View("BMAwbChargeCollectCreate", record);
    }

    /// <summary>
    /// BMs the awb charge collect create and duplicate.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <param name="record">The record.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE, ActionParamName = "BMAwbChargeCollectCreate")]
    public ActionResult BMAwbChargeCollectDuplicate(string invoiceId, string transactionId, CargoBillingMemoAwb record)
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
            record.BillingMemoId = transactionId.ToGuid();
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
            //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
            Ref: FRS Section 3.4 Table 18 Row 14 */
            MemberManager.ValidateIssuingAirline(record.AwbIssueingAirline);
            _cargoInvoiceManager.AddBMAwbRecord(record, invoiceId, out duplicateCouponErrorMessage);
            _cargoInvoiceManager.UpdateBMAwbAttachment(couponAttachmentIds, record.Id);
            //ShowSuccessMessage("Billing Memo AWB record created successfully.");
          
            //        ShowSuccessMessage(Messages.BMCouponCreateSuccessful + duplicateCouponErrorMessage);
            ShowSuccessMessage(Messages.BMCouponCreateSuccessful);
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
            record.Attachments.Clear(); // Attachments should not be duplicated. 
            SetViewDataPageMode(PageMode.Clone);
            //return RedirectToAction("BMAwbChargeCollectCreate", new { transactionId });
        }
        catch (ISBusinessException businessException)
        {
            ShowErrorMessage(businessException.ErrorCode);
            ViewData["IsPostback"] = true;
            record.Attachments = _cargoInvoiceManager.GetBMAwbAttachments(couponAttachmentIds);
        }
        var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        var billingMemoRecord = _cargoInvoiceManager.GetBillingMemoRecordDetails(transactionId);
        billingMemoRecord.Invoice = invoice;
        record.BillingMemoRecord = billingMemoRecord;

        return View("BMAwbChargeCollectCreate", record);
    }
    
    /// <summary>
    /// BMs the awb attachment upload.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = true, FileType = true, TableName = TransactionTypeTable.INVOICE)]
    public JsonResult BMAwbAttachmentUpload(string invoiceId, string transactionId)
    {
        string files = string.Empty;
        var attachments = new List<BMAwbAttachment>();
        // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015  
        var isUploadSuccess = false;
        string message;
        HttpPostedFileBase fileToSave;
        FileAttachmentHelper fileUploadHelper = null;

        try
        {
            Logger.Info("Started execution for method BMAwbAttachmentUpload for invoice ID" + invoiceId);
            var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
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
                if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoInvoiceManager.IsDuplicateBillingMemoAttachmentFileName(fileUploadHelper.FileOriginalName, transactionId.ToGuid()))
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
                Logger.Info("validated file successfully");
                if (fileUploadHelper.SaveFile())
                {
                    Logger.Info("File " + fileUploadHelper.FileOriginalName + " is saved successfully at " + fileUploadHelper.FileRelativePath + " folder");
                    files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
                    var attachment = new BMAwbAttachment
                    {
                        Id = fileUploadHelper.FileServerName,
                        OriginalFileName = fileUploadHelper.FileOriginalName,
                        FileSize = fileUploadHelper.FileToSave.ContentLength,
                        LastUpdatedBy = SessionUtil.UserId,
                        ServerId = fileUploadHelper.FileServerInfo.ServerId,
                        FileStatus = FileStatusType.Received,
                        FilePath = fileUploadHelper.FileRelativePath
                    };

                    attachment = _cargoInvoiceManager.AddBMAwbAttachment(attachment);
                    isUploadSuccess = true;
                    attachments.Add(attachment);
                    Logger.Info("Attachment Entry is inserted successfully in database");
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
    /// Billings the memo attachment download.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download)]
    [HttpGet]
    public FileStreamResult BMAwbAttachmentDownload(string invoiceId, string couponId)
    {
        var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetBMAwbAttachmentDetails(couponId) };

        return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    public JsonResult BillingMemoAwbProrateLadderGridData(string prorateLadderId)
    {
        // Create grid instance and retrieve data from database
        var bmAwbProrateLadderGrid = new BMAwbProrateLadderDetailGrid(ViewDataConstants.BillingMemoAwbProrateLadderGrid, Url.Action("BillingMemoAwbProrateLadderGridData", "Invoice", new { prorateLadderId }));
        ViewData[ViewDataConstants.BillingMemoAwbProrateLadderGrid] = bmAwbProrateLadderGrid.Instance;
        if (!string.IsNullOrEmpty(prorateLadderId))
        {
            //var prorateLadderGuid = ConvertUtil.ConvertStringtoGuid(prorateLadderId);
            var prorateLadderGuid = prorateLadderId.ToGuid();
            var bmAwbProrateLadderDetails = _cargoInvoiceManager.GetBMAwbProrateLadderDetailList(prorateLadderGuid).AsQueryable();
            try
            {
                // return contactsGrid.DataBind(contacts.AsQueryable());
                // return null;
                return bmAwbProrateLadderGrid.DataBind(bmAwbProrateLadderDetails);
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
    
    #endregion

    /// <summary>
    /// Update Awb record details for given coupon id and redirect to Get version of same action.
    /// </summary>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult AwbPrepaidRecordEdit(string invoiceId, string transactionId, AwbRecord awbRecord)
    {
      awbRecord.Id = transactionId.ToGuid();
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      awbRecord.BillingCodeId = (int)BillingCode.AWBPrepaid;
      //foreach (var tax in awbRecord.TaxBreakdown)
      //{
      //  tax.ParentId = awbRecord.Id;
      //}

      foreach (var vat in awbRecord.VatBreakdown)
      {
        vat.ParentId = awbRecord.Id;
      }

      var awbAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 9 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        _cargoInvoiceManager.UpdateAwbRecord(awbRecord, out duplicateErrorMessage);
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        TempData[TempDataConstants.AwbPrepaidRecord] = string.Format(@"{0}-{1}", awbRecord.BatchSequenceNumber, recSeqNum);
        return RedirectToAction("AwbPrepaidBillingCreate", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(awbAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(awbRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult AwbPrepaidRecordEdit(string invoiceId, string transactionId)
    {
      var awbRecord = GetAwbRecord(transactionId, invoiceId);

      // Set Airline flight designator to Billing Member name
     // awbRecord.AirlineFlightDesignator = awbRecord.Invoice.BillingMember.MemberCodeAlpha;

      return View(awbRecord);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpGet]
    public ActionResult AwbChargeCollectRecordEdit(string invoiceId, string transactionId)
    {
      var awbRecord = GetAwbRecord(transactionId, invoiceId);

      // Set Airline flight designator to Billing Member name
      // awbRecord.AirlineFlightDesignator = awbRecord.Invoice.BillingMember.MemberCodeAlpha;

      return View(awbRecord);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    [RestrictInvoiceUpdate(InvParamName = "invoiceId", IsJson = false, TableName = TransactionTypeTable.INVOICE)]
    public ActionResult AwbChargeCollectRecordEdit(string invoiceId, string transactionId, AwbRecord awbRecord)
    {
      awbRecord.Id = transactionId.ToGuid();
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      awbRecord.BillingCodeId = (int)BillingCode.AWBChargeCollect;
      //foreach (var tax in awbRecord.TaxBreakdown)
      //{
      //  tax.ParentId = awbRecord.Id;
      //}
      
      foreach (var vat in awbRecord.VatBreakdown)
      {
        vat.ParentId = awbRecord.Id;
      }

      var awbAttachmentIds = awbRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        string duplicateErrorMessage;
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 10 */
        MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);
        _cargoInvoiceManager.UpdateAwbRecord(awbRecord, out duplicateErrorMessage);
        awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful + duplicateErrorMessage);
        //ShowSuccessMessage(Messages.PrimeBillingUpdateSuccessful);
        ShowSuccessMessage(Messages.AwbRecordUpdateSuccessfull);
        if (!String.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);
        var recSeqNum = _cargoInvoiceManager.GetAwbBatchRecSeqNumber(awbRecord.BatchSequenceNumber, awbRecord.Invoice.InvoiceNumber);
        TempData[TempDataConstants.AwbPrepaidRecord] = string.Format(@"{0}-{1}", awbRecord.BatchSequenceNumber, recSeqNum);
        return RedirectToAction("AwbChargeCollectBillingCreate", new { invoiceId });
      }
      catch (ISBusinessException exception)
      {
        // Set ViewData, "IsPostback" to true, if exception occurs
        ViewData[ViewDataConstants.IsPostback] = true;
        ShowErrorMessage(exception.ErrorCode);
        awbRecord.Attachments = _cargoInvoiceManager.GetAwbRecordAttachments(awbAttachmentIds);
      }
      finally
      {
        SetViewDataPageMode(PageMode.Edit);
      }

      awbRecord.Invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

      return View(awbRecord);
    }
    private AwbRecord GetAwbRecord(string transactionId, string invoiceId)
    {
      var awbRecord = _cargoInvoiceManager.GetAwbRecordDetails((transactionId));
      awbRecord.Invoice = InvoiceHeader;
      awbRecord.LastUpdatedBy = SessionUtil.UserId;
      return awbRecord;
    }
    /// <summary>
    /// Delete Awb Prepaid billing record
    /// </summary>
    /// <param name="transactionId">Id of prime billing record which is to be deleted</param>
    
    [HttpPost]
    [RestrictInvoiceUpdate(IsJson = true, TransactionParamName = "transactionId", TableName = TransactionTypeTable.CGO_AIR_WAY_BILL)]
    public JsonResult AwbPrepaidRecordDelete(string transactionId)
    {
      UIMessageDetail details;
      try
      {
        // Delete record
        var isDeleted = _cargoInvoiceManager.DeleteAwbRecord(transactionId);
        //var isDeleted = true;
        details = GetDeleteMessage(isDeleted);
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download)]
    [HttpGet]
    public FileStreamResult AwbAttachmentDownload(string invoiceId, string transactionId)
    {
        var fileDownloadHelper = new FileAttachmentHelper { Attachment = _cargoInvoiceManager.GetAwbAttachmentDetails(transactionId) };

        return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    ///// <summary>
    ///// Delete Invoice Vat Record
    ///// </summary>
    ///// <param name="transactionId">Transaction Id</param>
    ///// <returns></returns>
    //public virtual JsonResult VatDelete(string transactionId)
    //{
    //  UIMessageDetail details;
    //  try
    //  {
    //    //Delete record
    //    bool isDeleted = _cargoInvoiceManager.DeleteInvoiceLevelVat(transactionId);

    //    details = GetDeleteMessage(isDeleted);
    //  }
    //  catch (ISBusinessException ex)
    //  {
    //    details = HandleDeleteException(ex.ErrorCode);
    //  }

    //  return Json(details);
    //}

    ///// <summary>
    ///// Vat Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public JsonResult InvoiceVatGrid(string invoiceId, bool isGridViewOnly)
    //{
    //  //Create grid instance and retrieve data from database
    //  var invoiceVatGrid = new CargoVatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);

    //  var vatData = _cargoInvoiceManager.GetInvoiceLevelVatList(invoiceId).AsQueryable();
    //  return invoiceVatGrid.DataBind(vatData);
    //}

    ///// <summary>
    ///// Available Vat Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public JsonResult AvailableVatGridData(string invoiceId)
    //{
    //  //Create grid instance and retrieve data from database
    //  var vatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));

    //  var vatData = _cargoInvoiceManager.GetInvoiceLevelDerivedVatList(invoiceId).AsQueryable();
    //  int count = 1;
    //  foreach (var derivedVatDetails in vatData)
    //  {
    //    derivedVatDetails.RowNumber = count++;
    //  }
    //  return vatGrid.DataBind(vatData);
    //}

    //#region"Invoice Vat"
    ///// <summary>
    ///// Save Invoice level VAT 
    ///// </summary>
    ///// <returns></returns>
    //[HttpPost]
    //public new virtual JsonResult Vat(FormCollection form)
    //{
    //  try
    //  {
    //    var vat = new JavaScriptSerializer().Deserialize(form[0], typeof(CargoInvoiceTotalVat));
    //    var record = vat as CargoInvoiceTotalVat;
    //    _cargoInvoiceManager.AddInvoiceLevelVat(record);

    //    var details = new UIMessageDetail
    //    {
    //      IsFailed = false,
    //      Message = Messages.RecordSaveSuccessful,
    //      isRedirect = true,
    //      RedirectUrl = Url.Action("Vat")

    //    };
    //    return Json(details);
    //  }
    //  catch (ISBusinessException businessException)
    //  {
    //    ShowErrorMessage(businessException.ErrorCode);

    //    var details = new UIMessageDetail
    //    {
    //      IsFailed = false,
    //      Message = string.Format(Messages.RecordSaveException, GetDisplayMessage(businessException.ErrorCode))
    //    };
    //    return Json(details);
    //  }
    //}

    /////// <summary>
    /////// Delete Invoice Vat Record
    /////// </summary>
    /////// <param name="transactionId">Transaction Id</param>
    /////// <returns></returns>
    //public virtual JsonResult VatDelete(string transactionId)
    //{
    //  UIMessageDetail details;
    //  try
    //  {
    //    //Delete record
    //    bool isDeleted = _cargoInvoiceManager.DeleteInvoiceLevelVat(transactionId);

    //    details = GetDeleteMessage(isDeleted);
    //  }
    //  catch (ISBusinessException ex)
    //  {
    //    details = HandleDeleteException(ex.ErrorCode);
    //  }

    //  return Json(details);
    //}

    ///// <summary>
    ///// Vat Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public new JsonResult InvoiceVatGrid(string invoiceId, bool isGridViewOnly)
    //{
    //  //Create grid instance and retrieve data from database
    //  var invoiceVatGrid = new CargoVatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);

    //  var vatData = _cargoInvoiceManager.GetInvoiceLevelVatList(invoiceId).AsQueryable();
    //  return invoiceVatGrid.DataBind(vatData);
    //}

    ///// <summary>
    ///// Available Vat Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public new JsonResult AvailableVatGridData(string invoiceId)
    //{
    //  //Create grid instance and retrieve data from database
    //  var vatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));

    //  var vatData = _cargoInvoiceManager.GetInvoiceLevelDerivedVatList(invoiceId).AsQueryable();
    //  int count = 1;
    //  foreach (var derivedVatDetails in vatData)
    //  {
    //    derivedVatDetails.RowNumber = count++;
    //  }
    //  return vatGrid.DataBind(vatData);
    //}

    ///// <summary>
    ///// Used to instantiate SourceCode vat Total grid
    ///// </summary>
    ///// <returns>null</returns>
    public JsonResult AvailableEmptySourceCodeVatTotalGridData()
    {
      return null;
    }

    ///// <summary>
    /////Unapplied Vat amount Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public new JsonResult UnappliedAmountVatGridData(string invoiceId)
    //{
    //  //Create grid instance and retrieve data from database
    //  var unappliedVatGrid = new CargoUnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
    //  var notAppliedVatList = _cargoInvoiceManager.GetNonAppliedVatList(invoiceId).AsQueryable();
    //  int count = 1;
    //  foreach (var nonAppliedVatDetails in notAppliedVatList)
    //  {
    //    nonAppliedVatDetails.RowNumber = count++;
    //  }
    //  return unappliedVatGrid.DataBind(notAppliedVatList);
    //}

    ///// <summary>
    ///// Invoice level VAT 
    ///// </summary>
    ///// <returns></returns>
    //public new virtual ActionResult Vat(string invoiceId)
    //{
    //  return View(VatBase(invoiceId));
    //}

    //public new virtual ActionResult VatView(string invoiceId)
    //{
    //  return View("Vat", VatBase(invoiceId));
    //}

    //private CargoInvoice VatBase(string invoiceId)
    //{
    //  CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
    //  bool isGridViewOnly = false;

    //  SetPageMode(invoice.InvoiceStatus);

    //  if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    //  {
    //    isGridViewOnly = true;
    //  }

    //  // Create grid instance for invoice vat 
    //  var invoiceVatGrid = new CargoVatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);
    //  ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

    //  if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
    //  {
    //    // Create grid instance for available vat 
    //    var availableVatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));
    //    ViewData[ViewDataConstants.AvailableVatGrid] = availableVatGrid.Instance;

    //    //Create grid instance for vat not applied amount
    //    var unappliedAmountVatGrid = new CargoUnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
    //    ViewData[ViewDataConstants.UnappliedAmountVatGrid] = unappliedAmountVatGrid.Instance;
    //  }

    //  return invoice;
    //}
    //#endregion

    ///// <summary>
    ///// Used to instantiate SourceCode vat Total grid
    ///// </summary>
    ///// <returns>null</returns>
    //public JsonResult AvailableEmptySourceCodeVatTotalGridData()
    //{
    //  return null;
    //}

    ///// <summary>
    /////Unapplied Vat amount Data to populate in the Grid
    ///// </summary>
    ///// <returns></returns>
    //public JsonResult UnappliedAmountVatGridData(string invoiceId)
    //{
    //  //Create grid instance and retrieve data from database
    //  var unappliedVatGrid = new CargoUnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
    //  var notAppliedVatList = _cargoInvoiceManager.GetNonAppliedVatList(invoiceId).AsQueryable();
    //  int count = 1;
    //  foreach (var nonAppliedVatDetails in notAppliedVatList)
    //  {
    //    nonAppliedVatDetails.RowNumber = count++;
    //  }
    //  return unappliedVatGrid.DataBind(notAppliedVatList);
    //}

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
    //}

    //private CargoInvoice VatBase(string invoiceId)
    //{
    //  CargoInvoice invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);
    //  bool isGridViewOnly = false;

    //  SetPageMode(invoice.InvoiceStatus);

    //  if (ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    //  {
    //    isGridViewOnly = true;
    //  }

    //  // Create grid instance for invoice vat 
    //  var invoiceVatGrid = new CargoVatGrid(ControlIdConstants.InvoiceVatGridId, Url.Action(InvoiceVatGridAction, new { invoiceId, isGridViewOnly }), isGridViewOnly);
    //  ViewData[ViewDataConstants.InvoiceVatGrid] = invoiceVatGrid.Instance;

    //  if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Receivables)
    //  {
    //    // Create grid instance for available vat 
    //    var availableVatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action(AvailableVatGridAction, new { invoiceId }));
    //    ViewData[ViewDataConstants.AvailableVatGrid] = availableVatGrid.Instance;

    //    //Create grid instance for vat not applied amount
    //    var unappliedAmountVatGrid = new CargoUnappliedVat(ControlIdConstants.UnappliedAmountVatGridId, Url.Action(UnappliedAmountVatGridAction, new { invoiceId }));
    //    ViewData[ViewDataConstants.UnappliedAmountVatGrid] = unappliedAmountVatGrid.Instance;
    //  }

    //  return invoice;
    //}
    

     public JsonResult ValidateAwbSerialNumber(int awbSerailNumber)
     {
      
         var result = _cargoInvoiceManager.ValidateAwbSerialNumber(awbSerailNumber,
                                                                   Convert.ToInt32(
                                                                     awbSerailNumber.ToString().Substring(7, 1)));
         return Json(result);

     }

    #region "payables"
     /// <summary>
     /// Allows to edit an invoice.
     /// </summary>
     /// <param name="invoiceId">The invoice id.</param>
     /// <param name="billingType">Type of the billing.</param>
     /// <returns></returns>
     [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.View)]
     public new ActionResult View(string invoiceId, string billingType)
     {
       var smIsToBeTreatedBilateral = _referenceManager.GetSMIsToBeTreatedBilateral();
       ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

       ViewData[ViewDataConstants.TransactionExists] = _cargoInvoiceManager.IsTransactionExists(invoiceId);

       // Currently, not all invoices has corresponding InvoiceTotal record entries 
       // in database, hence creating empty object of InvoiceTotal object.
       if (InvoiceHeader.CGOInvoiceTotal == null)
       {
         InvoiceHeader.CGOInvoiceTotal = new CargoInvoiceTotal();
       }

       // Create Source Code grid instance
       var sourceCodeGrid = new AwbCodeGrid(ControlIdConstants.SourceCodeGridId, Url.Action(SubTotalGridAction, new { invoiceId }), (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables) ? true : false);
       ViewData[ViewDataConstants.SourceCodeGrid] = sourceCodeGrid.Instance;
       MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

       if (InvoiceHeader != null && InvoiceHeader.MemberLocationInformation.Count > 0)
       {
         foreach (var memLocation in InvoiceHeader.MemberLocationInformation)
         {
           memLocation.LegalText = (!string.IsNullOrEmpty(InvoiceHeader.LegalText) ? InvoiceHeader.LegalText.Trim() : string.Empty);
         }
       }

       // If BillingType is Payables instantiate SourceCode Vat Total grid
       if (ViewData[ViewDataConstants.BillingType].ToString() == Util.BillingType.Payables)
       {
         var availableVatGrid = new CargoAvailableVatGrid(ControlIdConstants.AvailableVatGridId, Url.Action("AvailableEmptySourceCodeVatTotalGridData"));
         ViewData["VatGrid"] = availableVatGrid.Instance;
       }

       return View("Edit", InvoiceHeader);
     }

     /// <summary>
     /// Following action is used to retrieve Source Code Vat total and display it on SourceCodeVatTotal grid
     /// </summary>
     /// <param name="sourceCodeId">SourceCodeVat Total Id</param>
     /// <returns>Json result for SourceCode vat total</returns>
     public JsonResult GetBillingCodeVatTotal(string sourceCodeId)
     {
       // Call GetSourceCodeVatTotal() method which returns SourceCode vat total details
       var sourceCodeVatTotalList = _cargoInvoiceManager.GetBillingCodeVatTotal(sourceCodeId);
         //var sourceCodeVatTotalList = _cargoInvoiceManager.FetchBillingCodeVatTotal(sourceCodeId); 

       // Return Json result
       return Json(sourceCodeVatTotalList);
     }

     // Note: Commented following method to fix Parser error issue on Cargo Payables screen when user clicked on Reject button.
     // Will remove commented code when Issue is verified and tested. 
     /*[HttpPost]
     public JsonResult InitiateRejection(string rejectedRecordIds, string invoiceId, int billingYear, int billingMonth, int smi, int rejectionTransactionType)
     {
       // check if transactions have been rejected in some rejection memo.

       var transactions = _cargoInvoiceManager.GetRejectedTransactionDetails(rejectedRecordIds);

       transactions.IsTransactionOutsideTimeLimit = !_referenceManager.IsTransactionInTimeLimitMethodH((Model.Pax.Enums.TransactionType)rejectionTransactionType, smi, billingYear,
                                                         billingMonth);



       // For display of warning message for - 1. Coupon/memo already rejected, 2. Rejection outside time limit.
       if ((transactions.Transactions != null && transactions.Transactions.Count > 0) || transactions.IsTransactionOutsideTimeLimit)
       {
         return Json(transactions);
       }

       return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
     } */

      public JsonResult InitiateDuplicateRejection(string rejectedRecordIds, string invoiceId)
     {
       return GetInvoicesForBillingHistory(rejectedRecordIds, invoiceId);
     } 

     private JsonResult GetInvoicesForBillingHistory(string rejectedRecordIds, string invoiceId)
     {
       var invoice = _cargoInvoiceManager.GetInvoiceHeaderDetails(invoiceId);

       var billingCode = invoice.BillingCode;
       if (billingCode > (int)Iata.IS.Model.Pax.Enums.BillingCode.NonSampling) ++billingCode;
       var billedMemberId = invoice.BillingMemberId;
       var billingMemberId = invoice.BilledMemberId;

       var invoices = _cargoInvoiceManager.GetInvoicesForBillingHistory(billingCode, billedMemberId, SessionUtil.MemberId, invoice.SettlementMethodId);

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

       var url =  Url.Action("Create", "Invoice") + "?FromBillingHistory=true";

       result.Data = new UIMessageDetail
       {
         Message = "Success",
         isRedirect = true,
         RedirectUrl = url
       };
       return result;
     }
#endregion

     public JsonResult RejectionMemoAwbProrateLadderGridData(string prorateLadderId)
     {
         // Create grid instance and retrieve data from database
         var rmAwbProrateLadderGrid = new RMAwbProrateLadderDetailGrid(ViewDataConstants.RejectionMemoAwbProrateLadderGrid, Url.Action("RejectionMemoAwbProrateLadderGridData", "Invoice", new { prorateLadderId }));
         ViewData[ViewDataConstants.RejectionMemoAwbProrateLadderGrid] = rmAwbProrateLadderGrid.Instance;
         if (!string.IsNullOrEmpty(prorateLadderId))
         {
             //var prorateLadderGuid = ConvertUtil.ConvertStringtoGuid(prorateLadderId);
             var prorateLadderGuid = prorateLadderId.ToGuid();
             var rmAwbProrateLadderDetails = _cargoInvoiceManager.GetRMAwbProrateLadderDetailList(prorateLadderGuid).AsQueryable();
             try
             {
                 // return contactsGrid.DataBind(contacts.AsQueryable());
                 // return null;
                 return rmAwbProrateLadderGrid.DataBind(rmAwbProrateLadderDetails);
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
    /// SCP74897: CIDECT-00120130103.zip doesn't look correct
    /// Validate Mandatory fields from AWB prepaid billing record
    /// </summary>
    /// <param name="awbRecord">object of awb record</param>
     public void ValidateMandatoryFieldForAwbPrepaidBilling(AwbRecord awbRecord)
     {

       if(awbRecord.BatchSequenceNumber <= 0 || awbRecord.RecordSequenceWithinBatch <=0)
       {
         throw new ISBusinessException(CargoErrorCodes.BatchRecordSequenceNoReq);
       }

       //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
       /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 9 */
       MemberManager.ValidateIssuingAirline(awbRecord.AwbIssueingAirline);

       if (string.IsNullOrEmpty(awbRecord.AwbSerialNumberCheckDigit))
       {
         throw new ISBusinessException(CargoErrorCodes.AwbSrNoAndCheckDigitRequired);
       }
       
       if (awbRecord.AwbDate== null)
       {
         throw new ISBusinessException(CargoErrorCodes.InvalidAwbDate);
       }

       if (string.IsNullOrEmpty(awbRecord.ConsignmentOriginId))
       {
         throw new ISBusinessException(CargoErrorCodes.OriginofAwbIsInvalid);
       }

       if (string.IsNullOrEmpty(awbRecord.ConsignmentDestinationId))
       {
         throw new ISBusinessException(CargoErrorCodes.DestinationofAwbIsInvalid);
       }

       if (string.IsNullOrEmpty(awbRecord.CarriageFromId))
       {
         throw new ISBusinessException(CargoErrorCodes.FromofAwbIsInvalid);
       }

       if (string.IsNullOrEmpty(awbRecord.CarriageToId))
       {
         throw new ISBusinessException(CargoErrorCodes.ToofAwbIsInvalid);
       }

       if (string.IsNullOrEmpty(awbRecord.CurrencyAdjustmentIndicator))
       {
         throw new ISBusinessException(CargoErrorCodes.InvalidCurrencyAdjustmentInd);
       }
       if (awbRecord.DateOfCarriage == null)
       {
         throw new ISBusinessException(CargoErrorCodes.InvalidDateOfCarriage);
       }

     }

     /// <summary>
     /// SCPID : 105938 - RE: IATA- SIS Bug
     /// Validates the calculation of RMMemo Fields
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
                 if (
                     ConvertUtil.Round((record.AcceptedTotalWeightCharge ?? 0) - (record.BilledTotalWeightCharge ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalWeightChargeDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Weight Charges");

                 if (
                     ConvertUtil.Round(
                         (record.AcceptedTotalValuationCharge ?? 0) - (record.BilledTotalValuationCharge ?? 0),
                         Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalValuationChargeDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Valuation Charges");

                 if (
                     ConvertUtil.Round(
                         ((record.AcceptedTotalOtherChargeAmount ?? 0) - record.BilledTotalOtherChargeAmount ?? 0),
                         Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalOtherChargeDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Other Charges");

                 if (
                     ConvertUtil.Round((record.AcceptedTotalVatAmount ?? 0) - (record.BilledTotalVatAmount ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((decimal) (record.TotalVatAmountDifference ?? 0),
                                       Constants.CgoDecimalPlaces))
                     fields.Add("VAT Amount");

                 if (
                     ConvertUtil.Round((record.AcceptedTotalIscAmount ?? 0) - (record.AllowedTotalIscAmount ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalIscAmountDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("ISC ");
             }
             else
             {

                 if (
                     ConvertUtil.Round((record.BilledTotalWeightCharge ?? 0) - (record.AcceptedTotalWeightCharge ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalWeightChargeDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Weight Charges");

                 if (
                     ConvertUtil.Round(
                         (record.BilledTotalValuationCharge ?? 0) - (record.AcceptedTotalValuationCharge ?? 0),
                         Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalValuationChargeDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Valuation Charges");

                 if (
                     ConvertUtil.Round(
                         (record.BilledTotalOtherChargeAmount ?? 0) - (record.AcceptedTotalOtherChargeAmount ?? 0),
                         Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalOtherChargeDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Other Charges");

                 if (
                     ConvertUtil.Round((record.BilledTotalVatAmount ?? 0) - (record.AcceptedTotalVatAmount ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((decimal)(record.TotalVatAmountDifference ?? 0),
                                       Constants.CgoDecimalPlaces))
                     fields.Add("VAT Amount");

                 if (
                     ConvertUtil.Round((record.AllowedTotalIscAmount ?? 0) - (record.AcceptedTotalIscAmount ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.TotalIscAmountDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("ISC ");
             }

             return String.Join(",", fields);
         }
         return string.Empty;
     }
    
     /// <summary>
     /// SCPID : 105938 - RE: IATA- SIS Bug
     /// Validates the calculation of RM Awb breakdown fields
     /// </summary>
     /// <param name="record"></param>
     /// <returns></returns>
     private string ValidateRMAwbBreakdownFieldsCalculation(RMAwb record)
     {
         if (record != null)
         {
             List<string> fields = new List<string>();
             record.RejectionMemoRecord = _cargoInvoiceManager.GetRejectionMemoRecordDetails(record.RejectionMemoId.ToString());
             if (record.RejectionMemoRecord.RejectionStage == (int)RejectionStage.StageTwo)
             {
                 if (
                     ConvertUtil.Round((record.AcceptedWeightCharge ?? 0) - (record.BilledWeightCharge ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.WeightChargeDiff ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Weight Charges");

                 if (
                     ConvertUtil.Round((record.AcceptedValuationCharge ?? 0) - (record.BilledValuationCharge ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.ValuationChargeDiff ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Valuation Charges");

                 if (
                     ConvertUtil.Round(record.AcceptedOtherCharge - record.BilledOtherCharge, Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round(record.OtherChargeDiff, Constants.CgoDecimalPlaces))
                     fields.Add("Other Charges");

                 if (
                     ConvertUtil.Round((record.AcceptedVatAmount ?? 0) - (record.BilledVatAmount ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.VatAmountDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("VAT Amount");

                 if (
                     ConvertUtil.Round(record.AcceptedIscAmount - record.AllowedIscAmount, Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round(record.IscAmountDifference, Constants.CgoDecimalPlaces))
                     fields.Add("ISC ");
             }
             else
             {
                 if (
                     ConvertUtil.Round((record.BilledWeightCharge ?? 0) - (record.AcceptedWeightCharge ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.WeightChargeDiff ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Weight Charges");

                 if (
                     ConvertUtil.Round((record.BilledValuationCharge ?? 0) - (record.AcceptedValuationCharge ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.ValuationChargeDiff ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("Valuation Charges");

                 if (
                     ConvertUtil.Round(record.BilledOtherCharge - record.AcceptedOtherCharge, Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round(record.OtherChargeDiff, Constants.CgoDecimalPlaces))
                     fields.Add("Other Charges");

                 if (
                     ConvertUtil.Round((record.BilledVatAmount ?? 0) - (record.AcceptedVatAmount ?? 0),
                                       Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round((record.VatAmountDifference ?? 0), Constants.CgoDecimalPlaces))
                     fields.Add("VAT Amount");

                 if (
                     ConvertUtil.Round(record.AllowedIscAmount - record.AcceptedIscAmount, Constants.CgoDecimalPlaces) !=
                     ConvertUtil.Round(record.IscAmountDifference, Constants.CgoDecimalPlaces))
                     fields.Add("ISC ");
             }

             return String.Join(",", fields);
         }
         return string.Empty;
     }      
  }
}
