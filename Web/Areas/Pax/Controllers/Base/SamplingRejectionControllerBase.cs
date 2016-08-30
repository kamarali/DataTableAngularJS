using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.UIModel.Grid.Pax;
using Iata.IS.Web.Util;
using log4net;

namespace Iata.IS.Web.Areas.Pax.Controllers.Base
{
  public abstract class SamplingRejectionControllerBase : PaxInvoiceControllerBase
  {
    protected ISamplingRejectionManager SamplingRejectionManager;
   
    private const string GetSummaryListAction = "GetSummaryList";
    private const string GetRejectionMemoListAction = "GetRejectionMemoList";
    private const string GetRMCouponListAction = "GetRMCouponList";
    private static readonly ILog Logger = LogManager.GetLogger("samplingRejectionControlerBase");
    private const int InvalidValue = 0;

    protected SamplingRejectionControllerBase(ISamplingRejectionManager samplingFormManager, IReferenceManager referenceManager, IMemberManager memberManager) : base(samplingFormManager)
    {
      SamplingRejectionManager = samplingFormManager;
      ReferenceManager = referenceManager;
      MemberManager = memberManager;
    }

    protected abstract override int BillingCodeId
    {
      get;
    }

    protected virtual int TransactionTypeId
    {
      get;
      set;
    }

    protected virtual string SamplingFormName
    {
      get;
      set;
    }

    [HttpGet]
    public virtual ActionResult Create()
    {
      string billedMemberText = string.Empty;
      int billedMemberId = 0;
      Member billedMember = null;
      int provisionalBillingMonth = 0;
      int provisionalBillingYear = 0;
      double? samplingConstant = null;
      bool isDELinkingSucessful = false;
      bool isFLinkingSucessful = false;
      //Reset invoice search criteria
      SessionUtil.InvoiceSearchCriteria = null;

      if (!Request.QueryString.AllKeys.Contains(TempDataConstants.FromBillingHistory))
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
            var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);

            var inv = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
            if (inv != null)
            {
              billedMemberText = inv.BillingMemberText;
              billedMemberId = inv.BillingMemberId;
              billedMember = inv.BillingMember;
              provisionalBillingYear = inv.ProvisionalBillingYear;
              provisionalBillingMonth = inv.ProvisionalBillingMonth;
              
              // Copy the sampling constant.
              if (inv.BillingCode == (int)BillingCode.SamplingFormDE && inv.SamplingFormEDetails != null)
              {
                samplingConstant = inv.SamplingFormEDetails.SamplingConstant;
                isDELinkingSucessful = true;
              }
              else if (inv.BillingCode == (int)BillingCode.SamplingFormF)
              {
                samplingConstant = inv.SamplingConstant;
                isFLinkingSucessful = true;
              }
            }
          }
        }
        else if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          var correspondenceRefNumber = TempData[TempDataConstants.CorrespondenceNumber].ToString();
          var correspondenceManager = Ioc.Resolve<IPaxCorrespondenceManager>(typeof(IPaxCorrespondenceManager));
          var correspondence = correspondenceManager.GetRecentCorrespondenceDetails(Convert.ToInt64(correspondenceRefNumber));
          if (correspondence != null && correspondence.InvoiceId != Guid.Empty)
          {
            var inv = SamplingRejectionManager.GetInvoiceHeaderDetails(correspondence.InvoiceId.Value());
            if (inv != null)
            {
              billedMemberText = inv.BillingMemberText;
              billedMemberId = inv.BillingMemberId;
              billedMember = inv.BillingMember;
            }
          }
        }
      }
      var invoice = new PaxInvoice
                      {
                        // This will make sure 'R: Adjustment Due to Protest' is not displayed in SMI dropdown.
                        BilledMemberId = Convert.ToInt32(billedMemberId),
                        BilledMemberText = billedMemberText,
                        BilledMember = billedMember,
                        InvoiceType = InvoiceType.Invoice,
                        InvoiceDate = DateTime.UtcNow,
                        BillingCode = BillingCodeId,
                        BillingMemberId = SessionUtil.MemberId,
                        ProvisionalBillingMonth = provisionalBillingMonth,
                        ProvisionalBillingYear = provisionalBillingYear,
                      };
      //set linking value
      invoice.IsFormFViaIS = isFLinkingSucessful;
      invoice.IsFormDEViaIS = isDELinkingSucessful;

      if (samplingConstant.HasValue) invoice.SamplingConstant = samplingConstant.Value;
      MakeInvoiceRenderReady(invoice.Id, invoice);

      var digitalSignatureRequired = GetDigitalSignatureRequired(invoice.BillingMemberId);
      ViewData[ViewDataConstants.DefaultDigitalSignatureRequiredId] = digitalSignatureRequired;
      invoice.DigitalSignatureRequiredId = digitalSignatureRequired;

      SetViewDataPageMode(PageMode.Create);

      if (Request.QueryString.AllKeys.Contains(TempDataConstants.FromBillingHistory))
      {
        KeepBillingHistoryDataInStore();
      }
      else
      {
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          TempData.Remove(TempDataConstants.RejectedRecordIds);
        }
      }

      var smIsToBeTreatedBilateral = ReferenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return View(invoice);
    }

    [HttpPost]
    public virtual ActionResult Create(PaxInvoice invoice)
    {
      try
      {
        invoice.InvoiceType = InvoiceType.Invoice;
        invoice.BillingCode = BillingCodeId;
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
        SamplingRejectionManager.CreateInvoice(invoice);

        ShowSuccessMessage(string.Format(Messages.RMHeaderCreateSuccessful, SamplingFormName));

        KeepBillingHistoryDataInStore();

        //Initiate rejection memo from billing history screen
        return TempData.ContainsKey(TempDataConstants.RejectedRecordIds)
                 ? RedirectToAction("RMCreate", new { invoiceId = invoice.Id.Value() })
                 : RedirectToAction("Edit", new { invoiceId = invoice.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
          /* CMP #624: ICH Rewrite-New SMI X 
        * Description: As per ICH Web Service Response Message specifications 
        * Refer FRS Section 3.3 Table 9. 
        * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

          var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
          var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

          invoice.BilledMember = SamplingRejectionManager.GetBilledMember(invoice.BilledMemberId);

          if (!string.IsNullOrWhiteSpace(invoice.ChValidationResult) && (invoice.ChValidationResult.Equals(validationResultFail, StringComparison.CurrentCultureIgnoreCase) ||
                  invoice.ChValidationResult.Equals(validationResultError, StringComparison.CurrentCultureIgnoreCase)))
          {
              ShowSmiXWebServiceErrorMessage(businessException.Message);
          }
          else
          {
              ShowErrorMessage(businessException.ErrorCode);
          }
        
        var smIsToBeTreatedBilateral = ReferenceManager.GetSMIsToBeTreatedBilateral();
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
      finally
      {
        SetViewDataPageMode(PageMode.Create);
      }

      MakeInvoiceRenderReady(invoice.Id, invoice);

      return View(invoice);
    }

    [HttpGet]
    public virtual ActionResult Edit(string invoiceId)
    {
      return EditViewHeader(invoiceId);
    }

    [HttpGet]
    public new virtual ActionResult View(string invoiceId)
    {
      //get summary listing
      var formFSummaryGrid = new FormFSummaryGrid(ControlIdConstants.FormFSummaryGridId, Url.Action(GetSummaryListAction, new { invoiceId }));
      ViewData[ViewDataConstants.FormFSummaryListGrid] = formFSummaryGrid.Instance;

      //Initialize RM grid
      var rejectionMemoGrid = new FormFRejectionMemoGrid(ControlIdConstants.RejectionMemoGridId, Url.Action(GetRejectionMemoListAction, new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      var smIsToBeTreatedBilateral = ReferenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);

      return EditViewHeader(invoiceId);
    }

    private ActionResult EditViewHeader(string invoiceId)
    {
      if (string.IsNullOrEmpty(InvoiceHeader.BillingMemberLocationCode))
      {
        InvoiceHeader.BillingMemberLocationCode = "-";
      }
      if (string.IsNullOrEmpty(InvoiceHeader.BilledMemberLocationCode))
      {
        InvoiceHeader.BilledMemberLocationCode = "-";
      }

      ViewData[ViewDataConstants.IsSubmittedStatus] = InvoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling;
      ViewData[ViewDataConstants.TransactionExists] = SamplingRejectionManager.IsTransactionExists(invoiceId);
      
      var smIsToBeTreatedBilateral = ReferenceManager.GetSMIsToBeTreatedBilateral();
      ViewData[ViewDataConstants.BilateralSMIs] = string.Join(",", smIsToBeTreatedBilateral);
      
      MakeInvoiceRenderReady(InvoiceHeader.Id, InvoiceHeader);

      //get summary listing
      var formFSummaryGrid = new FormFSummaryGrid(ControlIdConstants.FormFSummaryGridId, Url.Action(GetSummaryListAction, new { invoiceId }));
      ViewData[ViewDataConstants.FormFSummaryListGrid] = formFSummaryGrid.Instance;

      //Initialize RM grid
      var rejectionMemoGrid = new FormFRejectionMemoGrid(ControlIdConstants.RejectionMemoGridId, Url.Action(GetRejectionMemoListAction, new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

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

      return View("Edit", InvoiceHeader);
    }

    //
    // POST: /Pax/FormF/Edit/5
    [HttpPost]
    public virtual ActionResult Edit(string invoiceId, PaxInvoice invoice)
    {
        try
        {
            invoice.Id = invoiceId.ToGuid();
            invoice.InvoiceType = InvoiceType.Invoice;
            invoice.InvoiceStatus = InvoiceStatusType.Open;
            invoice.BillingCode = BillingCodeId;
            invoice.InvoiceDate = DateTime.UtcNow;
            invoice.SubmissionMethod = SubmissionMethod.IsWeb;
            invoice.LastUpdatedBy = SessionUtil.UserId;
            invoice.ValidationStatus = InvoiceValidationStatus.Pending;
            invoice.ValidationDate = DateTime.MinValue;

            SamplingRejectionManager.UpdateInvoice(invoice);

            ShowSuccessMessage(string.Format(Messages.RMHeaderUpdateSuccessful, SamplingFormName));

            return RedirectToAction("Edit", new {invoiceId = invoice.Id.Value()});
        }
        catch (ISBusinessException businessException)
        {
            /* CMP #624: ICH Rewrite-New SMI X 
            * Description: As per ICH Web Service Response Message specifications 
            * Refer FRS Section 3.3 Table 9. 
            * TFS Issue #9205: CMP 624: Incorrect error if SMI is changed to X on ISWEB. */

            var validationResultFail = "F"; // F when Invoice/Credit Note fails validation in ICH
            var validationResultError = "E"; // E when ICH receives a Bad Request from SIS

            invoice.BilledMember = SamplingRejectionManager.GetBilledMember(invoice.BilledMemberId);
            invoice.InvoiceTotalRecord = SamplingRejectionManager.GetInvoiceTotal(invoice.Id.Value());
            ViewData[ViewDataConstants.IsSubmittedStatus] = invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling;
            ViewData[ViewDataConstants.TransactionExists] = SamplingRejectionManager.IsTransactionExists(invoiceId);

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


            var smIsToBeTreatedBilateral = ReferenceManager.GetSMIsToBeTreatedBilateral();
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
        finally
        {
            SetViewDataPageMode(PageMode.Edit);
        }

        MakeInvoiceRenderReady(invoice.Id, invoice);
        //get summary listing
        var formFSummaryGrid = new FormFSummaryGrid(ControlIdConstants.FormFSummaryGridId,
                                                    Url.Action(GetSummaryListAction, new {invoiceId}));
        ViewData[ViewDataConstants.FormFSummaryListGrid] = formFSummaryGrid.Instance;

        //Initialize RM grid
        var rejectionMemoGrid = new FormFRejectionMemoGrid(ControlIdConstants.RejectionMemoGridId,
                                                           Url.Action(GetRejectionMemoListAction, new {invoiceId}));
        ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;
        SetViewDataPageMode(PageMode.Edit);

        return View(invoice);
    }

      //
    // Get: /Pax/FormF/Details
    [HttpGet]
    [Obsolete]
    public virtual ActionResult Details(string invoiceId)
    {
      InvoiceHeader.InvoiceTotalRecord = SamplingRejectionManager.GetInvoiceTotal(invoiceId);

      //get summary listing
      var formFSummaryGrid = new FormFSummaryGrid(ControlIdConstants.FormFSummaryGridId, Url.Action(GetSummaryListAction, new { invoiceId }));
      ViewData[ViewDataConstants.FormFSummaryListGrid] = formFSummaryGrid.Instance;

      return View(InvoiceHeader);
    }

    [HttpGet]
    [Obsolete]
    public virtual ActionResult DetailsView(string invoiceId)
    {
      var invoiceHeader = SamplingRejectionManager.GetInvoiceHeaderDetails(invoiceId);
      invoiceHeader.InvoiceTotalRecord = SamplingRejectionManager.GetInvoiceTotal(invoiceId);

      //get summary listing
      var formFSummaryGrid = new FormFSummaryGrid(ControlIdConstants.FormFSummaryGridId, Url.Action(GetSummaryListAction, new { invoiceId }));
      ViewData[ViewDataConstants.FormFSummaryListGrid] = formFSummaryGrid.Instance;

      return View("Details", invoiceHeader);
    }

    [HttpGet]
    public virtual JsonResult GetSummaryList(string invoiceId)
    {
      var formFSummaryGrid = new FormFSummaryGrid(ControlIdConstants.FormFSummaryGridId, Url.Action(GetSummaryListAction, new { invoiceId }));
      var formFSummaryList = SamplingRejectionManager.GetSourceCodeList(invoiceId);

      return formFSummaryGrid.DataBind(formFSummaryList.AsQueryable());
    }

    [HttpPost]
    // This is not an Ajax call, as after Validate, page should refresh and show Submit button.
    public virtual ActionResult ValidateInvoice(string invoiceId)
    {
      ValidateInvoice(SamplingRejectionManager, invoiceId);

      return RedirectToAction("Edit", new { invoiceId });
    }

    [HttpPost]
    public virtual JsonResult RMDelete(string transactionId)
    {
      UIMessageDetail details;

      try
      {
        var rm = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
        var invoiceId = rm.InvoiceId.ToString();

        //Delete record
        var isDeleted = SamplingRejectionManager.DeleteRejectionMemoRecord(transactionId);

        details = GetDeleteMessage(isDeleted, Url.Action("Edit", new { invoiceId }));
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    //
    // Get: /Pax/FormF/Rejection Memo
    [HttpGet]
    [Obsolete]
    public virtual ActionResult RMList(string invoiceId)
    {
      var rejectionMemoGrid = new FormFRejectionMemoGrid(ControlIdConstants.RejectionMemoGridId, Url.Action(GetRejectionMemoListAction, new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      return View(InvoiceHeader);
    }

    //
    // Get: /Pax/FormF/Rejection Memo
    [HttpGet]
    [Obsolete]
    public virtual ActionResult RMListView(string invoiceId)
    {
      var rejectionMemoGrid = new FormFRejectionMemoGrid(ControlIdConstants.RejectionMemoGridId, Url.Action(GetRejectionMemoListAction, new { invoiceId }));
      ViewData[ViewDataConstants.RejectionMemoListGrid] = rejectionMemoGrid.Instance;

      return View("RMList", InvoiceHeader);
    }

    [HttpGet]
    public virtual JsonResult GetRejectionMemoList(string invoiceId)
    {
      var rejectionMemoGrid = new FormFRejectionMemoGrid(ControlIdConstants.RejectionMemoGridId, Url.Action(GetRejectionMemoListAction, new { invoiceId }));
      var rejectionMemoList = SamplingRejectionManager.GetRejectionMemoList(invoiceId);

      return rejectionMemoGrid.DataBind(rejectionMemoList.AsQueryable());
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult RMCreate(string invoiceId)
    {
      // SCP371876: File Loading & Web Response - Form F RMCreate Performance Improvement
      var rejectionMemoRecord = new RejectionMemo();

      //var sourceCodeList = ReferenceManager.GetSourceCodeList(TransactionTypeId);
      //// For pre-population of the only source code for Credit Memo.
      //if (sourceCodeList.Count != 0)
      //{
      //  rejectionMemoRecord.SourceCodeId = sourceCodeList[0].SourceCodeIdentifier;
      //}

      // ADO.Net call to get the source code for specified transaction type.
      var referenceManager = new Iata.IS.Business.Common.ReferenceManager();
      var sourceCode = referenceManager.GetSourceCode(TransactionTypeId);
      if (sourceCode > 0)
      {
        rejectionMemoRecord.SourceCodeId = sourceCode;
      }

      rejectionMemoRecord.Invoice = InvoiceHeader;
      rejectionMemoRecord.SamplingConstant = rejectionMemoRecord.Invoice.SamplingConstant;

      if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        var originalRejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();
        var rejectedRecordIds = originalRejectedRecordIds;

        var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
        rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf('@'));
        var transactionType = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf(';') + 1);
        rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf(';'));
        var rejectionIdList = rejectedRecordIds.Split(',');

        //SCP99417:IS-web performance
        //Used GetInvoiceHeader method instead GetInvoiceHeaderDetails as only header details are used 
        //var rejectedInvoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectedInvoiceId);
        var rejectedInvoice = SamplingRejectionManager.GetInvoiceHeader(rejectedInvoiceId);

        ViewData[ViewDataConstants.FromBillingHistory] = true;
        if (rejectedInvoice != null)
        {
          rejectionMemoRecord.YourInvoiceNumber = rejectedInvoice.InvoiceNumber;
          rejectionMemoRecord.YourInvoiceBillingYear = rejectedInvoice.BillingYear;
          rejectionMemoRecord.YourInvoiceBillingMonth = rejectedInvoice.BillingMonth;
          rejectionMemoRecord.YourInvoiceBillingPeriod = rejectedInvoice.BillingPeriod;
          rejectionMemoRecord.IsLinkingSuccessful = true;
          switch (transactionType)
          {
            case "FD":
              rejectionMemoRecord.FIMBMCMIndicatorId = (int) FIMBMCMIndicator.None;
              break;
            case "BM":
              rejectionMemoRecord.FIMBMCMIndicatorId = (int) FIMBMCMIndicator.BMNumber;
              break;
            case "CM":
              rejectionMemoRecord.FIMBMCMIndicatorId = (int) FIMBMCMIndicator.CMNumber;
              break;
            default:
              rejectionMemoRecord.FIMBMCMIndicatorId = (int) FIMBMCMIndicator.None;
              break;
          }
        }

        switch (transactionType)
        {
          case "FD":
            //SCP99417:IS-web performance
            //Commented below code as coupon variable is not used 
            //var formDEManager = Ioc.Resolve<ISamplingFormDEManager>(typeof(ISamplingFormDEManager));
            //var coupon = formDEManager.GetSamplingFormD(rejectionIdList[0]);
            rejectionMemoRecord.RejectionStage = 1;
            //  rejectionMemoRecord.SourceCodeId = coupon.SourceCodeId;
            break;
          case "RM":
            {
              var rejectedMemo = SamplingRejectionManager.GetRejectionMemoRecordDetails(rejectionIdList[0]);

              if (rejectedMemo != null)
              {
                rejectionMemoRecord.RejectionStage = ++rejectedMemo.RejectionStage;
                rejectionMemoRecord.YourRejectionNumber = rejectedMemo.RejectionMemoNumber;
              }
            }
            break;
        }

        TempData[TempDataConstants.RejectedRecordIds] = originalRejectedRecordIds;
      }

      return View(rejectionMemoRecord);
    }

    [HttpPost]
    public virtual ActionResult RMCreate(string invoiceId, RejectionMemo rejectionMemoRecord)
    {
      // SCP371876: File Loading & Web Response - Form F RMCreate Performance Improvement
      // var attachmentIds = rejectionMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();

      var attachmentIds = new List<Guid>();

      foreach (var attachement in rejectionMemoRecord.Attachments)
      {
        attachmentIds.Add(attachement.Id);
      }

      try
      {
        validateRequiredField(rejectionMemoRecord, BillingCodeId == (int)BillingCode.SamplingFormXF);

        rejectionMemoRecord.Attachments.Clear();
        rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
        {
          var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();

          var rejectedInvoiceId = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf('@') + 1);
          
          //SCP99417:Used GetInvoiceHeader as only invoice header details are used 

          var rejectedInvoice = SamplingRejectionManager.GetInvoiceHeader(rejectedInvoiceId);

          if (BillingCodeId == (int)BillingCode.SamplingFormXF)
          {
            var criteria = new SamplingRMLinkingCriteria {
                                                           BilledMemberId = rejectedInvoice.BilledMemberId,
                                                           BillingMemberId = rejectedInvoice.BillingMemberId,
                                                           BillingMonth = rejectedInvoice.BillingMonth,
                                                           BillingYear = rejectedInvoice.BillingYear,
                                                           BillingPeriod = rejectedInvoice.BillingPeriod,
                                                           InvoiceNumber = rejectedInvoice.InvoiceNumber,
                                                           ReasonCode = rejectionMemoRecord.ReasonCode.ToUpper(),
                                                           RejectingInvoiceId = invoiceId.ToGuid(),
                                                           RejectionMemoNumber = rejectionMemoRecord.YourRejectionNumber,
                                                           ProvBillingMonth = rejectedInvoice.ProvisionalBillingMonth,
                                                           ProvBillingYear = rejectedInvoice.ProvisionalBillingYear
                                                         };

            var rejectionMemoResult = Ioc.Resolve<ISamplingFormXfManager>(typeof(ISamplingFormXfManager)).GetLinkedFormFDetails(criteria);


            rejectionMemoRecord.IsLinkingSuccessful = rejectionMemoResult.IsLinkingSuccessful;
            rejectionMemoRecord.IsBreakdownAllowed = rejectionMemoResult.HasBreakdown;
            rejectionMemoRecord.CurrencyConversionFactor = rejectionMemoResult.CurrencyConversionFactor;
          }
          else
          {
            var rmLinkingCriteria = new SamplingRMLinkingCriteria {
                                                                    ReasonCode = rejectionMemoRecord.ReasonCode.ToUpper(),
                                                                    BilledMemberId = rejectedInvoice.BillingMemberId,
                                                                    BillingMemberId = rejectedInvoice.BilledMemberId,
                                                                    BillingMonth = rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                    BillingPeriod = rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                    BillingYear = rejectionMemoRecord.YourInvoiceBillingYear,
                                                                    InvoiceNumber = rejectionMemoRecord.YourInvoiceNumber,
                                                                    ProvBillingMonth = rejectedInvoice.ProvisionalBillingMonth,
                                                                    ProvBillingYear = rejectedInvoice.ProvisionalBillingYear,
                                                                    RejectingInvoiceId = invoiceId.ToGuid()
                                                                  };

            var rejectionMemoResult = Ioc.Resolve<ISamplingFormFManager>(typeof(ISamplingFormFManager)).GetLinkedFormDEDetails(rmLinkingCriteria);
            rejectionMemoRecord.IsLinkingSuccessful = string.IsNullOrWhiteSpace(rejectionMemoResult.ErrorMessage);
            rejectionMemoRecord.IsBreakdownAllowed = true;
            rejectionMemoRecord.CurrencyConversionFactor = rejectionMemoResult.CurrencyConversionFactor;
          }
        }

        var warningMessage = string.Empty;

        SamplingRejectionManager.AddRejectionMemoRecord(rejectionMemoRecord, out warningMessage);
        SamplingRejectionManager.UpdateRejectionMemoAttachment(attachmentIds, rejectionMemoRecord.Id);

        GetRejectCouponListFromBillingHistory(rejectionMemoRecord);

        if (TempData.ContainsKey(TempDataConstants.RejectedRecordIds) || (TempData.ContainsKey(TempDataConstants.CorrespondenceNumber)))
        {
          TempData.Clear();
        }
        
        ShowSuccessMessage(string.Format(Messages.SamplingRMCreateSuccessful, SamplingFormName));
        if(!string.IsNullOrEmpty(warningMessage))
          ShowErrorMessage(warningMessage, true);

        return RedirectToAction("RMEdit", new { invoiceId, transactionId = rejectionMemoRecord.Id.Value() });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
      }

      rejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(invoiceId);
      rejectionMemoRecord.Attachments = SamplingRejectionManager.GetRejectionMemoAttachments(attachmentIds);
      //SCP289215 - UA Ticket 618 729 0229461 cpn 1
      SetViewDataPageMode(PageMode.Clone);

      KeepBillingHistoryDataInStore();

      return View(rejectionMemoRecord);
    }

    private void GetRejectCouponListFromBillingHistory(RejectionMemo rejectionMemo)
    {
      if (TempData == null || !TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        return;
      }

      var rejectedRecordIds = TempData[TempDataConstants.RejectedRecordIds].ToString();

      rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf('@'));
      var transactionType = rejectedRecordIds.Substring(rejectedRecordIds.LastIndexOf(';') + 1);
      rejectedRecordIds = rejectedRecordIds.Substring(0, rejectedRecordIds.LastIndexOf(';'));
      var rejectionIdList = rejectedRecordIds.Split(',');

      if (transactionType == "FD")
      {
        // SCP371876: File Loading & Web Response - Form F RMCreate Performance Improvement
        // GetSamplingFormDList() call is commented, because the Ids return from the method are same as what we pass to that method.

        //var formDEManager = Ioc.Resolve<ISamplingFormDEManager>(typeof(ISamplingFormDEManager));
        //var couponList = formDEManager.GetSamplingFormDList(rejectionIdList);
        //if (couponList == null)
        //{
        //  return;
        //}
        // CopyPrimeCouponListToRM(couponList, rejectionMemo);
        
        // Call of CopyPrimeCouponListToRM() is written below.
        if(rejectionIdList.Count() == 0)
        {
          return;
        }

        // SCP371876: File Loading & Web Response - Form F RMCreate Performance Improvement
        // GetInvoiceHeaderDetails() call is removed, because Billing and Billed member id are already in rejectionMemo object.
        // var invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemo.InvoiceId.ToString());

        foreach (var primeCoupon in rejectionIdList)
        {
          var formFManager = Ioc.Resolve<ISamplingFormFManager>(typeof(ISamplingFormFManager));

          var coupondetails = formFManager.GetSamplingCouponBreakdownSingleRecordDetails(primeCoupon.ToGuid(), rejectionMemo.Id,
                                                                                         rejectionMemo.Invoice.BillingMemberId,
                                                                                         rejectionMemo.Invoice.BilledMemberId);
          string message;
          coupondetails.Details.RejectionMemoId = rejectionMemo.Id;
          SamplingRejectionManager.AddRejectionMemoCouponDetails(coupondetails.Details, out message);
        }
      }

      return;
    }

    private  void CopyRMCouponListToRM(IList<RMCoupon> rmCouponList, RejectionMemo rejectionMemo)
    {
      IList<RMCoupon> newRmCouponList = new List<RMCoupon>();
      string message;

      foreach (var rmCoupon in rmCouponList)
      {
        var newrmCoupon = new RMCoupon {
                                         RejectionMemoId = rejectionMemo.Id
                                       };
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
        SamplingRejectionManager.AddRejectionMemoCouponDetails(newrmCoupon, out message);
      }
      rejectionMemo.CouponBreakdownRecord.AddRange(newRmCouponList);
    }

    private  void CopyRMCouponVatBreakdown(RMCoupon newrmCoupon, List<RMCouponVat> list)
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

    private void CopyRMCouponTaxBreakdown(RMCoupon newrmCoupon, List<RMCouponTax> list)
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

    private void CopyPrimeCouponListToRM(IList<SamplingFormDRecord> couponList, RejectionMemo rejectionMemo)
    {
      var invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemo.InvoiceId.ToString());

      foreach (var primeCoupon in couponList)
      {
        var formFManager = Ioc.Resolve<ISamplingFormFManager>(typeof(ISamplingFormFManager));
        var coupondetails = formFManager.GetSamplingCouponBreakdownSingleRecordDetails(primeCoupon.Id, rejectionMemo.Id, invoice.BillingMemberId, invoice.BilledMemberId);
        string message;
        coupondetails.Details.RejectionMemoId = rejectionMemo.Id;
        SamplingRejectionManager.AddRejectionMemoCouponDetails(coupondetails.Details, out message);
      }
    }

    private void KeepBillingHistoryDataInStore()
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
    }

    [HttpGet]
    public virtual ActionResult RMEdit(string invoiceId, string transactionId)
    {
      var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);
      ViewData[ViewDataConstants.BreakdownExists] = SamplingRejectionManager.IsCouponExists(transactionId);
      return View(rejectionMemoRecord);
    }

    private RejectionMemo GetRejectionMemoRecord(string transactionId, string invoiceId)
    {
      var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
      var transactionType = 0;

      // Depending on RejectionStage retrieve transaction type
      switch (rejectionMemoRecord.RejectionStage)
      {
        case 1:
          transactionType = (int) TransactionType.SamplingFormD;
          break;
        case 2:
          transactionType = (int) TransactionType.SamplingFormF;
          break;
        case 3:
          transactionType = (int) TransactionType.SamplingFormXF;
          break;
      }

      // Depending on TransactionType and reasonCode retrieve whether Coupon breakdown is mandatory or not
      bool isCouponBreakdownMandatory = ReferenceManager.GetReasonCode(rejectionMemoRecord.ReasonCode, transactionType).CouponAwbBreakdownMandatory;

      // Set Coupon breakdown value
      rejectionMemoRecord.CouponAwbBreakdownMandatory = isCouponBreakdownMandatory;

      rejectionMemoRecord.Invoice = InvoiceHeader;
      rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;
      var rmCouponGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.RMCouponGridId, Url.Action(GetRMCouponListAction, new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rmCouponGrid.Instance;

      return rejectionMemoRecord;
    }

    [HttpGet]
    public virtual ActionResult RMView(string invoiceId, string transactionId)
    {
      var rejectionMemoRecord = GetRejectionMemoRecord(transactionId, invoiceId);

      return View(rejectionMemoRecord);
    }

    [HttpPost]
    public virtual ActionResult RMEdit(string transactionId, RejectionMemo rejectionMemoRecord)
    {
        var logRefId = Guid.NewGuid();
        var log = ReferenceManager.GetDebugLog(DateTime.Now, "RMEdit", this.ToString(),
                                        this.GetType().ToString(), "Stage 1: RMEdit start", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);

      var attachmentIds = rejectionMemoRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
        validateRequiredField(rejectionMemoRecord, BillingCodeId == (int)BillingCode.SamplingFormXF);

        rejectionMemoRecord.Id = transactionId.ToGuid();
        rejectionMemoRecord.LastUpdatedBy = SessionUtil.UserId;

        string warningMessage = string.Empty;

         log = ReferenceManager.GetDebugLog(DateTime.Now, "RMEdit", this.ToString(),
                                      this.GetType().ToString(), "Stage 2: Call to UpdateRejectionMemoRecord sent", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);

        SamplingRejectionManager.UpdateRejectionMemoRecord(rejectionMemoRecord, out warningMessage);


        log = ReferenceManager.GetDebugLog(DateTime.Now, "RMEdit", this.ToString(),
                                   this.GetType().ToString(), "Stage 2: Call to UpdateRejectionMemoRecord completed", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);

        ShowSuccessMessage(string.Format(Messages.SamplingRMUpdateSuccessful, SamplingFormName));
        if (!string.IsNullOrEmpty(warningMessage))
          ShowErrorMessage(warningMessage, true);

         log = ReferenceManager.GetDebugLog(DateTime.Now, "RMEdit", this.ToString(),
                                      this.GetType().ToString(), "Stage 1: RMEdit completed", SessionUtil.UserId, logRefId.ToString());
        ReferenceManager.LogDebugData(log);
        return RedirectToAction("RMEdit", new { transactionId, invoiceId = rejectionMemoRecord.InvoiceId.Value() });
      }
      catch (ISBusinessException businessException)
      {
        ShowErrorMessage(businessException.ErrorCode);
        rejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());
        rejectionMemoRecord.Attachments = SamplingRejectionManager.GetRejectionMemoAttachments(attachmentIds);
        var rmCouponGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.RMCouponGridId, Url.Action(GetRMCouponListAction, new { transactionId }));
        ViewData[ViewDataConstants.RMCouponListGrid] = rmCouponGrid.Instance;
        ViewData[ViewDataConstants.BreakdownExists] = SamplingRejectionManager.IsCouponExists(transactionId);
        SetViewDataPageMode(PageMode.Edit);

        return View(rejectionMemoRecord);
      }
    }

    /// <summary>
    /// Upload Rejection Memo Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [HttpPost]
    public virtual JsonResult RMFormFXFAttachmentUpload(string invoiceId, string transactionId)
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
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }

          PaxInvoice invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(invoiceId);

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
            var attachment = new RejectionMemoAttachment
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

            attachment = SamplingRejectionManager.AddRejectionMemoAttachment(attachment);
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
      catch (Exception exception)
      {
        Logger.Error("Exception", exception);
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false)
        {
          fileUploadHelper.DeleteFile();
        }
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Rejection Memo attachment
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [HttpGet]
    public virtual FileStreamResult RMFormFXFAttachmentDownload(string invoiceId, string transactionId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = SamplingRejectionManager.GetRejectionMemoAttachmentDetails(transactionId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpGet]
    public virtual ActionResult RMCouponList(string transactionId)
    {
      var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());

      var rmCouponGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.RMCouponGridId, Url.Action(GetRMCouponListAction, new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rmCouponGrid.Instance;

      return View(rejectionMemoRecord);
    }

    [HttpGet]
    public virtual ActionResult RMCouponListView(string transactionId)
    {
      var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());

      var rmCouponGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.RMCouponGridId, Url.Action(GetRMCouponListAction, new { transactionId }));
      ViewData[ViewDataConstants.RMCouponListGrid] = rmCouponGrid.Instance;

      return View("RMCouponList", rejectionMemoRecord);
    }

    [HttpGet]
    public virtual JsonResult GetRMCouponList(string transactionId)
    {
      var rmCouponGrid = new RejectionMemoCouponBreakdownGrid(ControlIdConstants.RMCouponGridId, Url.Action(GetRMCouponListAction, new { transactionId }));

      var rmCouponList = SamplingRejectionManager.GetRejectionMemoCouponBreakdownList(transactionId);

      return rmCouponGrid.DataBind(rmCouponList.AsQueryable());
    }

    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult RMCouponCreate(string transactionId)
    {
      var rejectionMemoCoupon = new RMCoupon();
      var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoCoupon.RejectionMemoRecord = rejectionMemoRecord;
      rejectionMemoCoupon.LastUpdatedBy = SessionUtil.UserId;
      rejectionMemoCoupon.RejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());
      SetViewDataPageMode(PageMode.Create);

      return View(rejectionMemoCoupon);
    }

    private static bool CheckIfVatBreakdownDeleted(int vatCountBefore, int vatCouponAfter)
    {
      return vatCountBefore > vatCouponAfter;
    }

    [HttpPost]
    public virtual ActionResult RMCouponCreate(string transactionId, RMCoupon couponBreakdownRecord)
    {
      var couponAttachmentIds = couponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
          //TaxBreakDown Validation 
          if (inValidTaxBreakDown(couponBreakdownRecord.TaxAmountDifference, couponBreakdownRecord.TaxBreakdown))
          {
              throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
          }
          //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
          /* CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
          Ref: FRS Section 3.4 Table 18 Row 7 and 8 */
          MemberManager.ValidateIssuingAirline(couponBreakdownRecord.TicketOrFimIssuingAirline);

        couponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();
        couponBreakdownRecord.Attachments.Clear();
        couponBreakdownRecord.LastUpdatedBy = SessionUtil.UserId;
        int vatRecordCountBefore = couponBreakdownRecord.VatBreakdown.Count;
        var duplicateErrorMessage = string.Empty;
        SamplingRejectionManager.AddRejectionMemoCouponDetails(couponBreakdownRecord, out duplicateErrorMessage);

        int vatRecordCountAfter = couponBreakdownRecord.VatBreakdown.Count;
        SamplingRejectionManager.UpdateRejectionMemoCouponAttachment(couponAttachmentIds, couponBreakdownRecord.Id);
        
        ShowSuccessMessages(string.Format(Messages.SamplingRMCouponCreateSuccessful, SamplingFormName), vatRecordCountBefore, vatRecordCountAfter);
        if(!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("RMCouponEdit", new { transactionId, couponId = couponBreakdownRecord.Id });
      }
      catch (ISBusinessException businessException)
      {
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);

        var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
        couponBreakdownRecord.RejectionMemoRecord = rejectionMemoRecord;
        couponBreakdownRecord.RejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());

        couponBreakdownRecord.Attachments = SamplingRejectionManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Clone);
        
        // Done to prevent Save button from getting disabled on exception.
        ViewData[ViewDataConstants.IsExceptionOccurred] = true;
        
        return View(couponBreakdownRecord);
      }
    }

    private void ShowSuccessMessages(string message, int vatRecordCountBefore, int vatRecordCountAfter)
    {
      if (CheckIfVatBreakdownDeleted(vatRecordCountBefore, vatRecordCountAfter))
      {
        message += string.Format(" {0}", Messages.VatRecordsDeletedInfo);
      }

      ShowSuccessMessage(message);
    }

    [HttpGet]
    public virtual ActionResult RMCouponEdit(string transactionId, string couponId)
    {
      return RMCouponEditViewActionResult(couponId, transactionId);
    }

    private ActionResult RMCouponEditViewActionResult(string couponId, string transactionId)
    {
      var rejectionMemoCoupon = SamplingRejectionManager.GetRejectionMemoCouponDetails(couponId);
      var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(transactionId);
      rejectionMemoCoupon.RejectionMemoRecord = rejectionMemoRecord;
      rejectionMemoCoupon.LastUpdatedBy = SessionUtil.UserId;
      rejectionMemoCoupon.RejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());

      return View(rejectionMemoCoupon);
    }

    [HttpGet]
    public virtual ActionResult RMCouponView(string invoiceId, string transactionId, string couponId)
    {
      return RMCouponEditViewActionResult(couponId, transactionId);
    }

    [HttpPost]
    public virtual ActionResult RMCouponEdit(string transactionId, string couponId, RMCoupon couponBreakdownRecord)
    {
      var couponAttachmentIds = couponBreakdownRecord.Attachments.Select(attachment => attachment.Id).ToList();
      try
      {
         couponBreakdownRecord.Id = couponId.ToGuid();

        foreach (var tax in couponBreakdownRecord.TaxBreakdown)
        {
          tax.ParentId = couponBreakdownRecord.Id;
        }

        foreach (var vat in couponBreakdownRecord.VatBreakdown)
        {
          vat.ParentId = couponBreakdownRecord.Id;
        }

        couponBreakdownRecord.RejectionMemoId = transactionId.ToGuid();

        //TaxBreakDown Validation 
        if (inValidTaxBreakDown(couponBreakdownRecord.TaxAmountDifference, couponBreakdownRecord.TaxBreakdown))
        {
            throw new ISBusinessException(ErrorCodes.TaxAmountDifferenceIsNotMatchingWithSumOfBreakdowns, string.Empty);
        }
        //SCPID : 107941 - ISIDEC received with non-numerical carrier ref
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
        Ref: FRS Section 3.4 Table 18 Row 7 and 8 */
        MemberManager.ValidateIssuingAirline(couponBreakdownRecord.TicketOrFimIssuingAirline);
        int vatRecordCountBefore = couponBreakdownRecord.VatBreakdown.Count;
        var duplicateErrorMessage = string.Empty;
        SamplingRejectionManager.UpdateRejectionMemoCouponDetails(couponBreakdownRecord, out duplicateErrorMessage);
        int vatRecordCountAfter = couponBreakdownRecord.VatBreakdown.Count;

        ShowSuccessMessages(string.Format(Messages.SamplingRMCouponUpdateSuccessful, SamplingFormName), vatRecordCountBefore, vatRecordCountAfter);

        if (!string.IsNullOrEmpty(duplicateErrorMessage))
          ShowErrorMessage(duplicateErrorMessage, true);

        return RedirectToAction("RMCouponEdit", new { transactionId = couponBreakdownRecord.RejectionMemoId.Value(), couponId });
      }
      catch (ISBusinessException businessException)
      {
        ShowCustomErrorMessage(businessException.ErrorCode, businessException.Message);

        var rejectionMemoRecord = SamplingRejectionManager.GetRejectionMemoRecordDetails(couponBreakdownRecord.RejectionMemoId.Value());
        couponBreakdownRecord.RejectionMemoRecord = rejectionMemoRecord;

        couponBreakdownRecord.RejectionMemoRecord.Invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(rejectionMemoRecord.InvoiceId.Value());

        couponBreakdownRecord.Attachments = SamplingRejectionManager.GetRejectionMemoCouponAttachments(couponAttachmentIds);
        SetViewDataPageMode(PageMode.Edit);

        return View(couponBreakdownRecord);
      }
    }

    [HttpPost]
    public virtual JsonResult RMCouponDelete(string couponId)
    {
      UIMessageDetail details;

      try
      {
        //Delete record
        Guid invoiceId = new Guid(), transactionId = new Guid();
        var isDeleted = SamplingRejectionManager.DeleteRejectionMemoCouponRecord(couponId, ref transactionId, ref invoiceId);

        details = GetDeleteMessage(isDeleted, Url.Action("RMEdit", new { invoiceId, transactionId }));
      }
      catch (ISBusinessException ex)
      {
        details = HandleDeleteException(ex.ErrorCode);
      }

      return Json(details);
    }

    /// <summary>
    /// Upload Rejection Memo Coupon Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [HttpPost]
    public virtual JsonResult RMCouponFormFXFAttachmentUpload(string invoiceId, string transactionId)
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
        foreach (string file in Request.Files)
        {
          isUploadSuccess = false;
          fileToSave = Request.Files[file];
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
          {
            continue;
          }
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //PaxInvoice invoice = SamplingRejectionManager.GetInvoiceHeaderDetails(invoiceId);
          PaxInvoice invoice = SamplingRejectionManager.GetInvoiceDetailForFileUpload(invoiceId);

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
            var attachment = new RMCouponAttachment
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

            attachment = SamplingRejectionManager.AddRejectionMemoCouponAttachment(attachment);
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
            // assign user info from session and file server info.
            if (attachment.UploadedBy == null)
            {
                attachment.UploadedBy = new Iata.IS.Model.Common.User();
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
    /// Download Rejection Memo Coupon attachment 
    ///  </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="couponId">Coupon Id</param>
    /// <returns></returns>
    [HttpGet]
    public virtual FileStreamResult RMCouponFormFXFAttachmentDownload(string invoiceId, string couponId)
    {
      var fileDownloadHelper = new FileAttachmentHelper { Attachment = SamplingRejectionManager.GetRejectionMemoCouponAttachmentDetails(couponId) };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [HttpPost]
    public override ActionResult Submit(string invoiceId)
    {
      var submittedInvoice = SubmitInvoice(invoiceId);

      var controllerName = submittedInvoice.BillingCode == (int) BillingCode.SamplingFormF ? "FormF" : "FormXF";

      return RedirectToAction(submittedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling ? "View" : "Edit", controllerName, new { invoiceId });
    }

    /// <summary>
    /// This method is use to check amount difference in tax breakdowns with total amount.
    /// Issue: 65997 - No tax breakdwon for invoice 2012D74226, RM 618240311.
    /// This validation will be applicable Form F and XF.
    /// SCP105897 & SCP105226 [added Math.Round() for calculatedAmount]
    /// </summary>
    /// <param name="targetAmount">target Amount</param>
    /// <param name="rmTaxBreakdowns">RM Tax Breakdowns</param>
    /// <returns>return false if tax breakdowns are valid.</returns>
    private bool inValidTaxBreakDown(double targetAmount, IEnumerable<RMCouponTax> rmTaxBreakdowns = null)
    {
        double calculatedAmount = 0;
        // rejection memo tax breakdown validation
        if (rmTaxBreakdowns != null)
        {
            calculatedAmount = rmTaxBreakdowns.Aggregate<RMCouponTax, double>(0, (current, rmCouponTax) => current + rmCouponTax.AmountDifference);
        }
      return Math.Round(calculatedAmount, 2).Equals(targetAmount) ? false : true;
    }

    /// <summary>
    /// Immplement Server side validation manditory fileds on create/edit RM.
    /// SCP#271979 - Incorrect Provisional Billing month for invoice P566180613   
    /// </summary>
    /// <param name="rejectionMemoRecord">object for RM</param>
    /// <param name="isFormXF">Check if From F or XF</param>
    private void validateRequiredField(RejectionMemo rejectionMemoRecord, bool isFormXF)
    {
      if (rejectionMemoRecord.SourceCodeId == InvalidValue)
      {
        throw new ISBusinessException(ErrorCodes.InvalidSourceCode);
      }
      if (string.IsNullOrWhiteSpace(rejectionMemoRecord.RejectionMemoNumber) || !Regex.IsMatch(rejectionMemoRecord.RejectionMemoNumber, "^[a-zA-Z0-9]*$"))
      {
        throw new ISBusinessException(ErrorCodes.InvalidRejectionMemo);
      }
      if ((string.IsNullOrWhiteSpace(rejectionMemoRecord.YourRejectionNumber) || !Regex.IsMatch(rejectionMemoRecord.YourRejectionNumber, "^[a-zA-Z0-9]*$")) && isFormXF)
      {
        throw new ISBusinessException(ErrorCodes.InvalidYourRejectionNumber);
      }
      if (string.IsNullOrWhiteSpace(rejectionMemoRecord.YourInvoiceNumber) || !Regex.IsMatch(rejectionMemoRecord.YourInvoiceNumber, "^[a-zA-Z0-9]*$"))
      {
        throw new ISBusinessException(ErrorCodes.BlankYourInvoiceNumber);
      }
      if (rejectionMemoRecord.YourInvoiceBillingYear == InvalidValue ||
          rejectionMemoRecord.YourInvoiceBillingMonth == InvalidValue || rejectionMemoRecord.YourInvoiceBillingPeriod == InvalidValue)
      {
        throw new ISBusinessException(ErrorCodes.InvalidYourInvoiceBillingPeriod);
      }
    }
  }
}
