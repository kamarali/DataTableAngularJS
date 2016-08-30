using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Security;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Misc;
using Iata.IS.Web.Util;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Web.Util.Filters;
using log4net;
using Iata.IS.Business.Pax;

namespace Iata.IS.Web.Areas.MU.Controllers
{
  public class CorrespondenceControllerBase : ISController
  {
    public static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public readonly IMiscCorrespondenceManager _miscCorrespondenceManager;
    public readonly ICalendarManager _calendarManager;
    public IReferenceManager _referenceManager { get; set; }
    private readonly IAuthorizationManager _authorizationManager;
    private readonly ICorrespondenceManager _correspondenceManager;
    public const string CorrespondenceRejectionGridAction = "CorrespondenceRejectionGridData";
    public const string CorrespondenceHistoryGridAction = "CorrespondenceHistoryGridData";

    public CorrespondenceControllerBase(IMiscCorrespondenceManager miscCorrespondenceManager, ICalendarManager calendarManager, IAuthorizationManager authorizationManager, ICorrespondenceManager correspondenceManager)
    {
      _miscCorrespondenceManager = miscCorrespondenceManager;
      _calendarManager = calendarManager;
      _authorizationManager = authorizationManager;
      _correspondenceManager = correspondenceManager;
    }

    private void CheckCorrespondenceStatus(string transactionId)
    {
        var correspondenceInDB = _miscCorrespondenceManager.GetCorrespondenceDetails(transactionId);

        if (correspondenceInDB != null && correspondenceInDB.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded)
        {
            throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadySent);
        }
    }
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpGet]
    public virtual ActionResult CreateCorrespondenceBase(string invoiceId, string transactionId, ProcessingContactType processingContact)
    {
      SetViewDataPageMode(PageMode.Create);
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;

      long correspondenceNumber;

      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Commented below line 
      // var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
      var invoice = _miscCorrespondenceManager.GetInvoiceHeaderDetail(invoiceId);
     

      var fromMember = _miscCorrespondenceManager.GetMember(invoice.BilledMemberId);
      var toMember = _miscCorrespondenceManager.GetMember(invoice.BillingMemberId);

      //Mail ids of correspondence contact of to member.
      var toEmailIds = _miscCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, processingContact);

      // Logic to set the correspondence number..
      correspondenceNumber = _miscCorrespondenceManager.GetInitialCorrespondenceNumber(invoice.BilledMemberId);

      //if (_miscCorrespondenceManager.IsFirstCorrespondence(invoice.BilledMemberId))
      //{
      //  correspondenceNumber = long.Parse(String.Format("{0}0000001", _miscCorrespondenceManager.GetNumericMemberCode(fromMember.MemberCodeNumeric)));
      //}
      //else
      //{
      //  correspondenceNumber = _miscCorrespondenceManager.GetCorrespondenceNumber(invoice.BilledMemberId);
      //  correspondenceNumber++;
      //}

      CheckCorrespondenceInitiator(invoiceId);

      //For correspondence from member will be billed member and to member will be billing member
      var correspondence = new MiscCorrespondence
      {
        Invoice = invoice,
        CorrespondenceDate = DateTime.UtcNow,
        CorrespondenceStage = 1,
        FromMember = fromMember,
        FromMemberId = invoice.BilledMemberId,
        ToMember = toMember,
        ToMemberId = invoice.BillingMemberId,
        ToEmailId = toEmailIds,
        ChargeCategory = invoice.ChargeCategory.Name,

        CorrespondenceNumber = correspondenceNumber,
        CorrespondenceStatus = CorrespondenceStatus.Open,
        CurrencyId = invoice.ListingCurrencyId,
        CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
        CorrespondenceOwnerId = SessionUtil.UserId,
        CorrespondenceOwnerName = SessionUtil.Username
      };

      //It will be populated as sum of all selected line item net totals of the invoice being rejected.
      if (invoice.InvoiceSummary != null)
      {
        correspondence.AmountToBeSettled = invoice.InvoiceSummary.TotalAmount;
      }

      //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = _referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, true);
      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      return View(correspondence);
    }

    public void CheckCorrespondenceInitiator(string invoiceId)
    {
      var correspondenceInDb = _miscCorrespondenceManager.GetFirstCorrespondenceDetails(invoiceId);

      ViewData[ViewDataConstants.CorrespondenceInitiator] = correspondenceInDb != null && correspondenceInDb.FromMemberId == SessionUtil.MemberId || correspondenceInDb == null;
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                             TransactionParamName = "transactionId",
                             CorrespondenceParamName = "correspondence",
                             InvList = false, TableName = TransactionTypeTable.MU_CORRESPONDENCE,
                             ActionParamName = "ViewCorrespondence")]
    public virtual ActionResult ReadyToSubmitCorrespondence(string invoiceId, string transactionId, MiscCorrespondence correspondence)
    {

      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit;

      correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
      try
      {
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
        if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
          correspondence.CurrencyId = invoice.ListingCurrencyId;
        if (string.IsNullOrEmpty(transactionId))
        {
          correspondence.Attachments.Clear();
          correspondence = _miscCorrespondenceManager.AddCorrespondence(correspondence, invoice.BillingCategory);
          _miscCorrespondenceManager.UpdateMiscCorrespondenceAttachment(correspondenceAttachmentIds, correspondence.Id);

          ShowSuccessMessage(Messages.MiscCorrespondenceCreateSuccessful);
        }
        else
        {
          correspondence.Id = transactionId.ToGuid();
          // Check if correspondence is already in responded state.
          CheckCorrespondenceStatus(transactionId);

          var savedCorr = _miscCorrespondenceManager.GetRecentCorrespondenceDetails(invoiceId);
          
          // SCP109163
          // Check if correspondence expiry date is crossed.
          if (invoice != null)
          {
            if (invoice.BillingCategory == BillingCategoryType.Misc && savedCorr.ExpiryDate < DateTime.UtcNow.Date)
            {
              throw new ISBusinessException(MiscErrorCodes.ExpiredCorrespondence);
            }

            if (invoice.BillingCategory == BillingCategoryType.Uatp && savedCorr.ExpiryDate < DateTime.UtcNow.Date)
            {
              throw new ISBusinessException(UatpErrorCodes.ExpiredCorrespondence);
            }
          }

          correspondence = _miscCorrespondenceManager.UpdateCorrespondence(correspondence);
          ShowSuccessMessage(Messages.MiscCorrespondenceUpdateSuccessful);
        }

        return RedirectToAction("EditCorrespondence",
                                new
                                  {
                                    invoiceId,
                                    transactionId = correspondence.Id.Value()
                                  });
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        correspondence.Attachments = _miscCorrespondenceManager.GetMiscCorrespondenceAttachments(correspondenceAttachmentIds);
        var billingMember = _miscCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _miscCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
        ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      }

      correspondence.Invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      if (string.IsNullOrEmpty(transactionId))
      {
        return View("CreateCorrespondence", correspondence);
      }

      return View("EditCorrespondence", correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.MU_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public virtual ActionResult CreateCorrespondence(string invoiceId, MiscCorrespondence correspondence)
    {
      //Get attachment Id list
      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
      correspondence.InvoiceId = invoiceId.ToGuid();
      var invoice = new MiscUatpInvoice();

      correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
      try
      {
        invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
        
        correspondence.Attachments.Clear();
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
          correspondence.CurrencyId = invoice.ListingCurrencyId;
        correspondence = _miscCorrespondenceManager.AddCorrespondence(correspondence, invoice.BillingCategory);
        //Update parent Id for attachment
        _miscCorrespondenceManager.UpdateMiscCorrespondenceAttachment(correspondenceAttachmentIds, correspondence.Id);
        ShowSuccessMessage(Messages.MiscCorrespondenceCreateSuccessful);

        return RedirectToAction("EditCorrespondence", new
        {
          invoiceId,
          transactionId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
        correspondence.Attachments = _miscCorrespondenceManager.GetMiscCorrespondenceAttachments(correspondenceAttachmentIds);
        ShowErrorMessage(exception.ErrorCode);
        correspondence.Invoice = invoice;
        var billingMember = _miscCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _miscCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
        ViewData[ViewDataConstants.InvoiceId] = invoiceId;

      }

      CheckCorrespondenceInitiator(invoiceId);

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      return View(correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.MU_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public virtual ActionResult CreateAndSendCorrespondence(string invoiceId, MiscCorrespondence correspondence)
    {
      Logger.InfoFormat("Start CreateAndSendCorrespondence time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      var invoice = new MiscUatpInvoice();

      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
      correspondence.InvoiceId = invoiceId.ToGuid();
      //  correspondence.CorrespondenceSentOnDate = DateTime.UtcNow;

      correspondence.CorrespondenceOwnerId = SessionUtil.UserId;

      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);
      try
      {
        invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
        //Get attachment Id list
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        correspondence.Attachments.Clear();
        if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
          correspondence.CurrencyId = invoice.ListingCurrencyId;

        //SCP#446268: Correspondence Problem (Check if TO_Email ID is blank re-fetch the contacts from Profile)
        if (String.IsNullOrEmpty(correspondence.ToEmailId))
        {
            string toEmail = null;

            //Mail ids of correspondence contact of To Member.
            if (invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
            {
                toEmail = _miscCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, ProcessingContactType.MiscCorrespondence);
            }
            else if (invoice.BillingCategoryId == (int)BillingCategoryType.Uatp)
            {

                toEmail = _miscCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, ProcessingContactType.UatpCorrespondence);
            }
            if (!String.IsNullOrEmpty(toEmail))
                correspondence.ToEmailId = toEmail;
        }

        correspondence = _miscCorrespondenceManager.AddCorrespondence(correspondence, invoice.BillingCategory);
        //Update parent Id for attachment
        _miscCorrespondenceManager.UpdateMiscCorrespondenceAttachment(correspondenceAttachmentIds, correspondence.Id);

        var correspondenceUrl = string.Format("{0}/{1}", UrlHelperEx.ToAbsoluteUrl(Url.Action("ViewLinkedCorrespondence", "Correspondence")), correspondence.Id);

        // Send the correspondence.
        /* CMP#657: Retention of Additional Email Addresses in Correspondences
           Adding code to get email ids from initiator and non-initiator*/
        var toEmailIds = _correspondenceManager.GetEmailIdsList(correspondence.ToEmailId,
                                                                      correspondence.AdditionalEmailInitiator,
                                                                      correspondence.AdditionalEmailNonInitiator);

        if (_miscCorrespondenceManager.ValidateCorrespondence(correspondence))
        {
          Logger.InfoFormat("Before CreateAndSendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
          // CMP#657: Retention of Additional Email Addresses in Correspondences.
          // Logic Moved to common location. i.e in corresponceManager.cs
          // _miscCorrespondenceManager.SendCorrespondenceMail(correspondenceUrl, toEmailIds, string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject));
          var frmMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.FromMemberId);
          var toMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.ToMemberId);
          _correspondenceManager.EmailAlertsOnSendingOfCorrespondences(BillingCategoryType.Misc, correspondenceUrl, toEmailIds,
                                                                       string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject),
                                                                       string.Format("{0}-{1}", frmMember.MemberCodeAlpha, frmMember.MemberCodeNumeric),
                                                                       string.Format("{0}-{1}", toMember.MemberCodeAlpha, toMember.MemberCodeNumeric));
          Logger.InfoFormat("After CreateAndSendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
        }

        correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;
        correspondence = _miscCorrespondenceManager.UpdateCorrespondence(correspondence);

        ShowSuccessMessage(Messages.MiscCorrespondenceSentSuccessful);

        return RedirectToAction("Correspondence", "Correspondence", new
        {
          invoiceId,
          transactionId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
        correspondence.Attachments = _miscCorrespondenceManager.GetMiscCorrespondenceAttachments(correspondenceAttachmentIds);
        ShowErrorMessage(exception.ErrorCode);
        correspondence.Invoice = invoice;
        var billingMember = _miscCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _miscCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
        ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      }

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      CheckCorrespondenceInitiator(invoiceId);

      return View("CreateCorrespondence", correspondence);
    }

    /// <summary>
    /// Opens the correspondence for edit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public virtual ActionResult OpenCorrespondenceForEdit(string invoiceId, string transactionId)
    {
      var correspondence = _miscCorrespondenceManager.GetCorrespondenceDetails(transactionId);
      SetViewDataPageMode(PageMode.Edit);
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;

      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

      //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
      ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
        _miscCorrespondenceManager.IsCorrespondenceEligibleForReply(
        invoice.BillingCategory == BillingCategoryType.Misc
            ? (int)BillingCategoryType.Misc
            : (int)BillingCategoryType.Uatp, correspondence.Id,
          SessionUtil.UserId, SessionUtil.MemberId,
          invoice.BillingCategory == BillingCategoryType.Misc
          ? Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence
          :  Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence
          );
      int memberId = SessionUtil.MemberId;
      if (correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed && (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit) && correspondence.FromMemberId == memberId)
      {
        return RedirectToAction("EditCorrespondence", "Correspondence", new
        {
          invoiceId,
          transactionId
        });
      }

      ViewData[ViewDataConstants.TransactionExists] = true;
      correspondence.Invoice = invoice;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      correspondence.ChargeCategory = invoice.ChargeCategory.Name;
      if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
        correspondence.CurrencyId = invoice.ListingCurrencyId;

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));


      //CMP527: Acceptance and Closure of Correspondence by Initiator- Start
      if ((_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence) && invoice.BillingCategoryId == (int)BillingCategorys.Miscellaneous)
        || (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence) && invoice.BillingCategoryId == (int)BillingCategorys.Uatp))
      {
        var corrCanClose = _miscCorrespondenceManager.CanCorrespondenceClose(invoice.BillingCategoryId, correspondence.Id.ToString());
        var canAbleToClose = Convert.ToBoolean(corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCanClose]);
        if (canAbleToClose)
        {
          var correspondenceInDb = _miscCorrespondenceManager.GetFirstCorrespondenceByCorrespondenceNo(correspondence.CorrespondenceNumber.Value);
          ViewData[ViewDataConstants.CorrespondenceCanClose] = correspondenceInDb.FromMemberId == SessionUtil.MemberId;
          ViewData[ViewDataConstants.CorrespondeneClosedScenario] = corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCloseScenario];
        }
      }
      //CMP527: End

      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;

      return View("Correspondence", correspondence);
    }

    private void SetCorrespondenceDetails(string invoiceId, MiscCorrespondence correspondence, MiscUatpInvoice invoice)
    {
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      correspondence.ChargeCategory = invoice.ChargeCategory.Name;
      if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
        correspondence.CurrencyId = invoice.ListingCurrencyId;

      correspondence.Invoice = invoice;
      var billingMember = _miscCorrespondenceManager.GetMember(correspondence.FromMemberId);
      var billedMember = _miscCorrespondenceManager.GetMember(correspondence.ToMemberId);
      correspondence.FromMember = billingMember;
      correspondence.ToMember = billedMember;

      CheckCorrespondenceInitiator(invoiceId);

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));
    }

    /// <summary>
    /// Allows to edit correspondence.
    /// </summary>
    ///  <param name="invoiceId">The invoice Id</param>
    /// <param name="transactionId">The correspondence Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public virtual ActionResult EditCorrespondence(string invoiceId, string transactionId)
    {
      SetViewDataPageMode(PageMode.Edit);
      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      //check whether transactions exist for this form DE header
      ViewData[ViewDataConstants.TransactionExists] = _miscCorrespondenceManager.IsTransactionExists(transactionId).ToString().ToLower();
      var correspondence = _miscCorrespondenceManager.GetCorrespondenceDetails(transactionId);

      // SCP251331: FW: Rare Occuring Issue found for Closed Pax Correspondence Functionalit
      if (correspondence == null)
      {
        return RedirectToAction("Index", "BillingHistory");
      }

      SetCorrespondenceDetails(invoiceId, correspondence, invoice);
      if (invoice != null)
        //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;
          /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = _referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, true);

      //CMP527: Acceptance and Closure of Correspondence by Initiator- Start
      if ((_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence) && invoice.BillingCategoryId == (int)BillingCategorys.Miscellaneous) 
         || (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence) && invoice.BillingCategoryId == (int)BillingCategorys.Uatp))
      {
        var corrCanClose = _miscCorrespondenceManager.CanCorrespondenceClose(invoice.BillingCategoryId,
                                                                         correspondence.Id.ToString());
        var canAbleToClose = Convert.ToBoolean(corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCanClose]);
        if (canAbleToClose)
        {
          var correspondenceInDb = _miscCorrespondenceManager.GetFirstCorrespondenceByCorrespondenceNo(correspondence.CorrespondenceNumber.Value);
          ViewData[ViewDataConstants.CorrespondenceCanClose] = correspondenceInDb.FromMemberId == SessionUtil.MemberId;
          ViewData[ViewDataConstants.CorrespondeneClosedScenario] = corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCloseScenario];
        }
      }
      //CMP527: End
      
      // SCP109163
      // Check if correspondence expiry date is crossed.
      if(invoice != null)
      {
        if(invoice.BillingCategory == BillingCategoryType.Misc && correspondence.ExpiryDate < DateTime.UtcNow.Date)
        {
          ShowErrorMessage(MiscErrorCodes.ExpiredCorrespondence);
        }

        if(invoice.BillingCategory == BillingCategoryType.Uatp && correspondence.ExpiryDate < DateTime.UtcNow.Date)
        {
          ShowErrorMessage(UatpErrorCodes.ExpiredCorrespondence);
        }
      }

      return View(correspondence);
    }

    /// <summary>
    /// Update sampling form D record
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="transactionId"></param>
    /// <param name="correspondence"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                           TransactionParamName = "transactionId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.MU_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public virtual ActionResult EditCorrespondence(string invoiceId, string transactionId, MiscCorrespondence correspondence)
    {
      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
      try
      {
        correspondence.Id = transactionId.ToGuid();
        correspondence.InvoiceId = invoiceId.ToGuid();
        // Check if correspondence is already in responded status.
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            CheckCorrespondenceStatus(transactionId);
        }

        var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

        var savedCorr = _miscCorrespondenceManager.GetRecentCorrespondenceDetails(invoiceId);

        // CMP#657: CIT9494: System behaving adversely when incorrect email id is provided.
        if (savedCorr != null)
        {
          correspondence.ExpiryDate = savedCorr.ExpiryDate;
        }

        // SCP109163
        // Check if correspondence expiry date is crossed.
        if (invoice != null)
        {
          if (invoice.BillingCategory == BillingCategoryType.Misc && savedCorr.ExpiryDate < DateTime.UtcNow.Date)
          {
            throw new ISBusinessException(MiscErrorCodes.ExpiredCorrespondence);
          }

          if (invoice.BillingCategory == BillingCategoryType.Uatp && savedCorr.ExpiryDate < DateTime.UtcNow.Date)
          {
            throw new ISBusinessException(UatpErrorCodes.ExpiredCorrespondence);
          }
        }
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
          correspondence.CurrencyId = invoice.ListingCurrencyId;
        correspondence = _miscCorrespondenceManager.UpdateCorrespondence(correspondence);

        ShowSuccessMessage(Messages.MiscCorrespondenceUpdateSuccessful);

        return RedirectToAction("EditCorrespondence", new
        {
          invoiceId,
          correspondenceId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        correspondence.Attachments = _miscCorrespondenceManager.GetMiscCorrespondenceAttachments(correspondenceAttachmentIds);
        var billingMember = _miscCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _miscCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
        ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      }

      correspondence.Invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

      CheckCorrespondenceInitiator(invoiceId);

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      return View(correspondence);
    }

    /// <summary>
    /// Used from invoice search result grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public virtual ActionResult ViewCorrespondence(string invoiceId)
    {
      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
      var correspondence = _miscCorrespondenceManager.GetRecentCorrespondenceDetails(invoiceId, SessionUtil.MemberId);
   //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
        ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
            _miscCorrespondenceManager.IsCorrespondenceEligibleForReply(
                invoice.BillingCategory == BillingCategoryType.Misc
                    ? (int) BillingCategoryType.Misc
                    : (int) BillingCategoryType.Uatp, correspondence.Id,
                SessionUtil.UserId, SessionUtil.MemberId,
                invoice.BillingCategory == BillingCategoryType.Misc
                    ?  Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence
                    : Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence
                          .DraftCorrespondence);

      correspondence.ChargeCategory = invoice.ChargeCategory.Name;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
        correspondence.CurrencyId = invoice.ListingCurrencyId;

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      SetViewDataPageMode(PageMode.View);
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;

      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;

      return View("Correspondence", correspondence);
    }

    /// <summary>
    /// Used from correspondence search result grid.
    /// </summary>
    /// <param name="invoiceId">Invoice id.</param>
    /// <param name="transactionId">The transaction id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    [Obsolete]
    public virtual ActionResult ViewCorrespondenceDetails(string invoiceId, string transactionId)
    {
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);
      var correspondence = _miscCorrespondenceManager.GetCorrespondenceDetails(transactionId);
    //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
        ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
            _miscCorrespondenceManager.IsCorrespondenceEligibleForReply(
                invoice.BillingCategory == BillingCategoryType.Misc
                    ? (int) BillingCategoryType.Misc
                    : (int) BillingCategoryType.Uatp, correspondence.Id,
                SessionUtil.UserId, SessionUtil.MemberId,
                invoice.BillingCategory == BillingCategoryType.Misc
                    ?  Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence
                    : Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence
                          .DraftCorrespondence);

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      correspondence.ChargeCategory = invoice.ChargeCategory.Name;
      if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
        correspondence.CurrencyId = invoice.ListingCurrencyId;

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      SetViewDataPageMode(PageMode.View);

      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;

      return View("Correspondence", correspondence);
    }

    /// <summary>
    /// Views the linked correspondence.
    /// </summary>
    /// <param name="invoiceId">The correspondence id.</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public virtual ActionResult ViewLinkedCorrespondence(string invoiceId)
    {
      var correspondence = _miscCorrespondenceManager.GetCorrespondenceDetails(invoiceId);
      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(correspondence.InvoiceId.ToString());
      
      //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
      ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
        _miscCorrespondenceManager.IsCorrespondenceEligibleForReply(
        invoice.BillingCategory == BillingCategoryType.Misc
            ? (int) BillingCategoryType.Misc
            : (int) BillingCategoryType.Uatp, correspondence.Id,
          SessionUtil.UserId, SessionUtil.MemberId,
                invoice.BillingCategory == BillingCategoryType.Misc
                    ?  Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence
                    : Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence
                          .DraftCorrespondence);
      ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;

      correspondence.ChargeCategory = invoice.ChargeCategory.Name;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
        correspondence.CurrencyId = invoice.ListingCurrencyId;

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.InvoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.InvoiceId }));

      SetViewDataPageMode(PageMode.View);

      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed)
      {
        correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;
      }

      return View("Correspondence", correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public virtual ActionResult Correspondence(string invoiceId)
    {
      // Retrieve memberId from Session variable and use it where ever required
      var memberId = SessionUtil.MemberId;
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;

      var correspondence = new MiscCorrespondence
      {
        CorrespondenceStatus = CorrespondenceStatus.Open,
        CorrespondenceSubStatus = CorrespondenceSubStatus.Saved
      };

      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

      correspondence = _miscCorrespondenceManager.GetRecentCorrespondenceDetails(invoiceId, memberId);

      // check whether transactions exist for this invoice.
      if (correspondence != null)
      {
          //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
          ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
              _miscCorrespondenceManager.IsCorrespondenceEligibleForReply(
                  invoice.BillingCategory == BillingCategoryType.Misc
                      ? (int)BillingCategoryType.Misc
                      : (int)BillingCategoryType.Uatp, correspondence.Id,
                  SessionUtil.UserId, SessionUtil.MemberId,
                  invoice.BillingCategory == BillingCategoryType.Misc
                      ? (int)Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.DraftCorrespondence
                      : (int)
                        Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence
                            .DraftCorrespondence);

        if (correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed && (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit) && correspondence.FromMemberId == memberId)
        {
          return RedirectToAction("EditCorrespondence", "Correspondence", new
          {
            invoiceId,
            transactionId = correspondence.Id
          });
        }

        ViewData[ViewDataConstants.TransactionExists] = true;

        //CMP527: Acceptance and Closure of Correspondence by Initiator- Start
        if ((_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence) && invoice.BillingCategoryId == (int)BillingCategorys.Miscellaneous)
         || (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.UATP.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence) && invoice.BillingCategoryId == (int)BillingCategorys.Uatp))
        {
          var corrCanClose = _miscCorrespondenceManager.CanCorrespondenceClose(invoice.BillingCategoryId, correspondence.Id.ToString());
          var canAbleToClose = Convert.ToBoolean(corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCanClose]);
          if (canAbleToClose)
          {
            var correspondenceInDb = _miscCorrespondenceManager.GetFirstCorrespondenceByCorrespondenceNo(correspondence.CorrespondenceNumber.Value);
            ViewData[ViewDataConstants.CorrespondenceCanClose] = correspondenceInDb.FromMemberId == SessionUtil.MemberId;
            ViewData[ViewDataConstants.CorrespondeneClosedScenario] = corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCloseScenario];
          }
        }
        //CMP527: End

        correspondence.Invoice = invoice;

        correspondence.ChargeCategory = invoice.ChargeCategory.Name;
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        if (!_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
          correspondence.CurrencyId = invoice.ListingCurrencyId;
      }
      else
      {
        return RedirectToAction("CreateCorrespondence", "Correspondence", new { invoiceId });
      }

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      // If logged-in member is the To Member Id of the correspondence, correspondence sub-status should be seen as 'Received'.
      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed)
      {
        correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;
      }

      return View(correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                           TransactionParamName = "transactionId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.MU_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public virtual ActionResult SendCorrespondence(string invoiceId, string transactionId, MiscCorrespondence correspondence)
    {
      Logger.InfoFormat("Start SendCorrespondence time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      var invoice = new MiscUatpInvoice();
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;

      // SCP431909: Kale - Correspondence Owner
      // 2. For MISC and UATP: 
      // a. Information about the ‘Correspondence Owner’ was NOT getting updated upon ‘send’ 
      // b. E.g. if User A creates a stage and User B sends it, the owner was still shown as User A 
      //    instead of correctly showing User B as per the user who actually ‘sent’ the correspondence.
      if(SessionUtil.UserId > 0)
      {
          correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
      }

      //correspondence.CorrespondenceSentOnDate = DateTime.UtcNow;
      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);
      correspondence.CorrespondenceDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);

      try
      {
        correspondence.Id = transactionId.ToGuid();
        correspondence.InvoiceId = invoiceId.ToGuid();
        // Check if correspondence is already in responded state.
        if (!string.IsNullOrWhiteSpace(transactionId))
        {
            CheckCorrespondenceStatus(transactionId);
        }
        
        invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

        var savedCorr = _miscCorrespondenceManager.GetRecentCorrespondenceDetails(invoiceId);

        // CMP#657: CIT9494: System behaving adversely when incorrect email id is provided.
        if (savedCorr != null)
        {
          correspondence.ExpiryDate = savedCorr.ExpiryDate;
        }

        //SCP#446268: Correspondence Problem (Check if TO_Email ID is blank re-fetch the contacts from Profile)
        if (String.IsNullOrEmpty(correspondence.ToEmailId))
        {
            string toEmail = null;

            //Mail ids of correspondence contact of To Member.
            if (invoice.BillingCategoryId == (int)BillingCategoryType.Misc)
            {
                toEmail = _miscCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, ProcessingContactType.MiscCorrespondence);
            }
            else if (invoice.BillingCategoryId == (int)BillingCategoryType.Uatp)
            {

                toEmail = _miscCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, ProcessingContactType.UatpCorrespondence);
            }
            if (!String.IsNullOrEmpty(toEmail))
                correspondence.ToEmailId = toEmail;
        }

        // SCP109163
        // Check if correspondence expiry date is crossed.
        if (invoice != null)
        {
          if (invoice.BillingCategory == BillingCategoryType.Misc && savedCorr.ExpiryDate < DateTime.UtcNow.Date)
          {
            throw new ISBusinessException(MiscErrorCodes.ExpiredCorrespondence);
          }

          if (invoice.BillingCategory == BillingCategoryType.Uatp && savedCorr.ExpiryDate < DateTime.UtcNow.Date)
          {
            throw new ISBusinessException(UatpErrorCodes.ExpiredCorrespondence);
          }
        }
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        if (invoice != null && !_referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
          correspondence.CurrencyId = invoice.ListingCurrencyId;

        // Get the absolute url for the correspondence.
        var correspondenceUrl = string.Format("{0}/{1}", UrlHelperEx.ToAbsoluteUrl(Url.Action("ViewLinkedCorrespondence", "Correspondence")), transactionId);

        /* CMP#657: Retention of Additional Email Addresses in Correspondences
           Adding code to get email ids from initiator and non-initiator*/
        var toEmailIds = _correspondenceManager.GetEmailIdsList(correspondence.ToEmailId,
                                                                      correspondence.AdditionalEmailInitiator,
                                                                      correspondence.AdditionalEmailNonInitiator);

        if (_miscCorrespondenceManager.ValidateCorrespondence(correspondence))
        {
          Logger.InfoFormat("Before SendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
          // CMP#657: Retention of Additional Email Addresses in Correspondences.
          // Logic Moved to common location. i.e in corresponceManager.cs
          // _miscCorrespondenceManager.SendCorrespondenceMail(correspondenceUrl, toEmailIds, string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject));
          var frmMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.FromMemberId);
          var toMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.ToMemberId);
          _correspondenceManager.EmailAlertsOnSendingOfCorrespondences(BillingCategoryType.Misc, correspondenceUrl, toEmailIds,
                                                                       string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject),
                                                                       string.Format("{0}-{1}", frmMember.MemberCodeAlpha, frmMember.MemberCodeNumeric),
                                                                       string.Format("{0}-{1}", toMember.MemberCodeAlpha, toMember.MemberCodeNumeric));
          Logger.InfoFormat("After SendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
        }

        correspondence = _miscCorrespondenceManager.UpdateCorrespondence(correspondence);

        ShowSuccessMessage(Messages.MiscCorrespondenceSentSuccessful);

        return RedirectToAction("Correspondence", "Correspondence", new
        {
          invoiceId
        });

      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
        correspondence.Attachments = _miscCorrespondenceManager.GetMiscCorrespondenceAttachments(correspondenceAttachmentIds);
        correspondence.Invoice = invoice;
        var billingMember = _miscCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _miscCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
        ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      }

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      CheckCorrespondenceInitiator(invoiceId);

      return View("EditCorrespondence", correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Misc.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpGet]
    public virtual ActionResult ReplyCorrespondenceBase(string invoiceId, string transactionId, ProcessingContactType processingContact)
    {
      var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

      var correspondenceInDb = _miscCorrespondenceManager.GetCorrespondenceDetails(transactionId);

      //Mail ids of correspondence contact of to member.
      var toEmailIds = _miscCorrespondenceManager.GetToEmailIds(correspondenceInDb.FromMemberId, processingContact);

      var correspondence = new MiscCorrespondence
      {
        Invoice = invoice,
        InvoiceId = invoice.Id,
        CorrespondenceDate = DateTime.UtcNow,
        CorrespondenceStage = correspondenceInDb.CorrespondenceStage + 1,
        ToMember = correspondenceInDb.FromMember,
        ToMemberId = correspondenceInDb.FromMemberId,
        FromMember = correspondenceInDb.ToMember,
        FromMemberId = correspondenceInDb.ToMemberId,
        CurrencyId = correspondenceInDb.CurrencyId,
        AmountToBeSettled = correspondenceInDb.AmountToBeSettled,
        CorrespondenceOwnerId = SessionUtil.UserId,
        CorrespondenceOwnerName = SessionUtil.Username,
        ToEmailId = toEmailIds,
        ChargeCategory = invoice.ChargeCategory.Name,
        ChargeCode = correspondenceInDb.ChargeCode,
        CorrespondenceNumber = correspondenceInDb.CorrespondenceNumber,
        YourReference = correspondenceInDb.OurReference,
        CorrespondenceStatus = CorrespondenceStatus.Open,
        CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
        AuthorityToBill = correspondenceInDb.AuthorityToBill,
        Subject = correspondenceInDb.Subject,
          /* CMP#657: Retention of Additional Email Addresses in Correspondences
                     Adding code to get email ids from initiator and non-initiator*/
        AdditionalEmailInitiator = correspondenceInDb.AdditionalEmailInitiator,
        AdditionalEmailNonInitiator = correspondenceInDb.AdditionalEmailNonInitiator
      };

      GetMiscCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));
      GetMiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      CheckCorrespondenceInitiator(invoiceId);
      ViewData[ViewDataConstants.InvoiceId] = invoiceId;

      //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = _referenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, true);

      return View("CreateCorrespondence", correspondence);
    }

    /// <summary>
    /// Fetch data for source code data and display in grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual JsonResult CorrespondenceRejectionGridData(string invoiceId)
    {
      var correspondenceRejectionGrid = new MiscCorrespondenceRejectionGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));

      var correspondenceRejectionTotal = _miscCorrespondenceManager.GetCorrespondenceRejectionList(invoiceId).AsQueryable();

      return correspondenceRejectionGrid.DataBind(correspondenceRejectionTotal);
    }

    /// <summary>
    /// Fetch data for source code data and display in grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [HttpGet]
    public virtual JsonResult CorrespondenceHistoryGridData(string invoiceId)
    {
      var correspondenceHistoryGrid = new MiscCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId }));

      var correspondenceHistoryTotal = _miscCorrespondenceManager.GetCorrespondenceHistoryList(invoiceId, SessionUtil.MemberId).AsQueryable();

      return correspondenceHistoryGrid.DataBind(correspondenceHistoryTotal);
    }

    /// <summary>
    /// Upload multiple Correspondence Attachment 
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="transactionId">Transaction Id</param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Invoice.CreateOrEdit)]
    [HttpPost]
    public virtual JsonResult CorrespondenceAttachmentUpload(string invoiceId, string transactionId)
    {
      string files = string.Empty;
      var attachments = new List<MiscUatpCorrespondenceAttachment>();
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
          if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0)) continue;

          MiscUatpInvoice invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

          fileUploadHelper = new FileAttachmentHelper
          {
            FileToSave = fileToSave,
            FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth)
          };

          //On Correpondence create/update
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _miscCorrespondenceManager.IsDuplicateMiscCorrespondenceAttachmentFileName(fileUploadHelper.FileOriginalName,
                                                                             transactionId.ToGuid()))
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
            files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
            var attachment = new MiscUatpCorrespondenceAttachment
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

            attachment = _miscCorrespondenceManager.AddRejectionMemoAttachment(attachment);
            isUploadSuccess = true;
            attachments.Add(attachment);
          }
        }
        message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
      }
      catch (ISBusinessException ex)
      {
        message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
        if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
      }
      catch (Exception)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    /// <summary>
    /// Download Correspondence Attachment  
    /// </summary>
    // [ISAuthorize(Business.Security.Permissions.Misc.Receivables.Invoice.Download)]
    [HttpGet]
    public virtual FileStreamResult CorrespondenceAttachmentDownload(string attachmentId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _miscCorrespondenceManager.GetMiscCorrespondenceAttachmentDetail(attachmentId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    /// <summary>
    /// Upload Attachment 
    /// </summary>
    [HttpPost]
    public virtual JsonResult Upload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<MiscUatpCorrespondenceAttachment>();
      // SCP335749: SRM Admin Alert - Offline collection generation failure notification - SIS Production 05 FEB 2015
      var isUploadSuccess = false;
      var message = string.Empty;
      HttpPostedFileBase fileToSave;
      FileAttachmentHelper fileUploadHelper = null;

      try
      {
        fileToSave = Request.Files[0];
        if (Equals(fileToSave, null) || String.IsNullOrEmpty(fileToSave.FileName) || !(fileToSave.ContentLength > 0))
        {
          throw new ISBusinessException("Upload unsuccessful.");
        }

        var invoice = _miscCorrespondenceManager.GetInvoiceDetail(invoiceId);

        fileUploadHelper = new FileAttachmentHelper
        {
          FileToSave = fileToSave,
          FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear, invoice.BillingMonth)
        };

        // On correspondence create/update
        if (!Equals(transactionId.ToGuid(), Guid.Empty) &&
            _miscCorrespondenceManager.IsDuplicateMiscCorrespondenceAttachmentFileName(
              fileUploadHelper.FileOriginalName,
              transactionId.ToGuid()))
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
          files = String.Format("{0}{1},", files, fileUploadHelper.FileOriginalName);
          var attachment = new MiscUatpCorrespondenceAttachment
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

          attachment = _miscCorrespondenceManager.AddRejectionMemoAttachment(attachment);
          isUploadSuccess = true;
          attachments.Add(attachment);
          message = string.Format(Messages.FileUploadSuccessful, files.TrimEnd(','));
        }
      }
      catch (ISBusinessException ex)
      {
        message = string.Format(Messages.FileUploadBusinessException, ex.ErrorCode);
        if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
      }
      catch (Exception)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
      }

      return new JsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    public void GetMiscCorrespondenceRejectGrid(string conrolId, string urlAction)
    {
      var gridModel = new MiscCorrespondenceRejectionGrid(conrolId, urlAction);
      ViewData[ViewDataConstants.CorrespondenceRejectionsGrid] = gridModel.Instance;
    }

    public void GetMiscCorrespondenceHistoryGrid(string conrolId, string urlAction)
    {
      var historyGridModel = new MiscCorrespondenceHistoryGrid(conrolId, urlAction);
      ViewData[ViewDataConstants.CorrespondenceHistoryGrid] = historyGridModel.Instance;
    }

    /// <summary>
    /// This method will not allow the user to reply to correspondence if a correspondence invoice already exists 
    /// for that correspondence.
    /// </summary>
    /// <param name="correspondenceRefNumber"></param>
    /// <returns></returns>
    [HttpPost]
    public virtual JsonResult IsCorrespondenceInvoiceExistsForCorrespondence(long correspondenceRefNumber)
    {
      int toMemberId = SessionUtil.MemberId;

      MiscUatpInvoice miscInvoice = _miscCorrespondenceManager.IsCorrespondenceInvoiceExistsForCorrespondence(toMemberId,
                                                                                                       correspondenceRefNumber);
      if (miscInvoice != null)
          return Json(new UIMessageDetail
          {
              //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202].
              Message = string.Format(Messages.CannotReplyToMiscCorrespondence, miscInvoice.InvoiceNumber, new DateTime(miscInvoice.BillingYear, miscInvoice.BillingMonth, miscInvoice.BillingPeriod).ToString("yyyy-MMM-dd")),
              IsFailed = true
          });

      //if (_miscCorrespondenceManager.IsCorrespondenceInvoiceExistsForCorrespondence(toMemberId, correspondenceRefNumber))
      //  return Json(new UIMessageDetail
      //  {
      //    Message = Messages.CannotReplyToMiscCorrespondence,
      //    IsFailed = true
      //  });

      return Json(new UIMessageDetail
      {
        IsFailed = false
      });
    }

    [HttpPost]
    public virtual JsonResult IsCorrespondenceOutSideTimeLimit(string invoiceId)
    {
      //SCP0000:Impact on MISC/UATP rejection linking due to purging
      MiscUatpInvoice invoiceHeader;
      bool isTimeLimitRecordFound = true;
      if (!_miscCorrespondenceManager.IsCorrespondenceOutSideTimeLimit(invoiceId, out invoiceHeader, ref isTimeLimitRecordFound))
      {
        if (invoiceHeader!=null)
        {
          DateTime expiryDateperiod=new DateTime(1973,1,1);
          if(invoiceHeader.ExpiryDatePeriod!=null && invoiceHeader.ExpiryDatePeriod==expiryDateperiod)
          {
            return
              Json(new UIMessageDetail() { IsFailed = true, Message = Messages.BMISC_10212 });
          }
        }
        return
          Json(new UIMessageDetail() { IsFailed = false, RedirectUrl = Url.Action("Correspondence", "Correspondence", new { invoiceId = invoiceId }) });
      }

      if (isTimeLimitRecordFound)
      {
          return Json(new UIMessageDetail() { IsFailed = true, Message = Messages.CorrespondenceOutSideTimeLimit });
      }
      else
      {
          /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
          Desc: system failed to retrieve Time Limit data from the master */
          return Json(new UIMessageDetail() {IsFailed = true, Message = Messages.BGEN_10906 });
      }
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date: 05-03-2012
    /// Purpose: This method is written to check the UserSession. [SCP ID: 11529-PR2290493 IS-WEB problems]
    /// Response: this method will returns string "true" if user session is alive else it will return LogOn URL
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public virtual JsonResult CheckUserSessions()
    {
      if (SessionUtil.UserId > 0)
      {
        return Json("true");
      }
      else
      {
        // Clear User Related Session
        SessionUtil.UserId = 0;
        SessionUtil.IsLogOutProxyOption = false;
        SessionUtil.IsLoggedIn = false;
        Session.Abandon();

        //return LogOn URL to Json Call.
        var retrnParam = AdminSystem.SystemParameters.Instance.General.LogOnURL;
        return Json(retrnParam);
      }
    }

    /// <summary>
    /// CMP 527:CloseCorrespondence
    /// </summary>
    /// <param name="correspondenceId">correspondence Id</param>
    /// <param name="invoiceId">Invoice Id</param>
    /// <param name="correspondenceStage">correspondence stage</param>
    /// <param name="correspondenceStatus">correspondence status</param>
    /// <param name="correspondenceSubStatus">correspondence sub status</param>
    /// <param name="scenarioId">close scenario</param>
    /// <param name="userAcceptanceComment">acceptance comments</param>
    /// <returns></returns>
    [HttpPost]
    public virtual ActionResult CloseCorrespondence(string correspondenceId, string invoiceId, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioId, string userAcceptanceComment)
    {
      var invoice = new MiscUatpInvoice();
      
      var returnMsg = string.Empty;
      var saved = false;
      var correspondence = _miscCorrespondenceManager.GetCorrespondenceDetails(correspondenceId);

      if (correspondence != null)
      {
          var correspondenceNumber = correspondence.CorrespondenceNumber.HasValue
                                    ? Convert.ToString(correspondence.CorrespondenceNumber.Value)
                                    : string.Empty;
          invoice = _miscCorrespondenceManager.GetInvoiceHeaderDetail(correspondence.InvoiceId.ToString());

          //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202].
          MiscUatpInvoice miscInvoice = _miscCorrespondenceManager.IsCorrespondenceInvoiceExistsForCorrespondence(SessionUtil.MemberId, correspondence.CorrespondenceNumber.HasValue
                                      ? correspondence.CorrespondenceNumber.Value
                                      : 0);


          if (miscInvoice != null)
          {

              if (correspondence.Invoice != null && correspondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
                  ShowErrorMessage(UatpErrorCodes.CanNotReplyToUatpCorrespondence, miscInvoice.InvoiceNumber, new DateTime(miscInvoice.BillingYear, miscInvoice.BillingMonth, miscInvoice.BillingPeriod).ToString("yyyy-MMM-dd"));
              ShowErrorMessage(MiscErrorCodes.CanNotReplyToMiscCorrespondence, miscInvoice.InvoiceNumber, new DateTime(miscInvoice.BillingYear, miscInvoice.BillingMonth, miscInvoice.BillingPeriod).ToString("yyyy-MMM-dd"));
          }
          else
          {
              /* CMP#657 - Send email notification of closure only to email contacts specified in last responded state. 
               * (And not to emails specified in newly created saved state correspondence) TFS Bug# 9560 */
              var lastRespondedCorrespondence = _miscCorrespondenceManager.GetLastRespondedCorrespondene(correspondence.CorrespondenceNumber.Value);

              saved = _miscCorrespondenceManager.CloseCorrespondence(correspondenceNumber, correspondenceStage,
                                                                     correspondenceStatus, correspondenceSubStatus, scenarioId,
                                                                     invoice.BillingCategoryId, userAcceptanceComment,
                                                                     SessionUtil.UserId, DateTime.UtcNow, ref returnMsg);
              if (saved)
              {
                  //SCP329272 - SIS:Correspondence - Closure of Passenger Correspondence No. 790000302
                  //Get Last Corr Detail for ToEmailId and ToAddtionalEmailId
                  var l_correspondence = _miscCorrespondenceManager.GetCorrespondenceDetails(correspondence.CorrespondenceNumber);

                  if (l_correspondence == null && correspondence.CorrespondenceStage == 1)
                  {
                      l_correspondence = correspondence;
                  }
                  
                  /* CMP#657: Retention of Additional Email Addresses in Correspondences
                    FRS Section: 2.5 Email Alerts on Acceptance of Closure of Correspondences*/
                  //SCP426039: FW: SIS:Correspondence - Closure of Passenger Correspondence No. 1760005673 - SIS Production
                  _miscCorrespondenceManager.SendCorrespondenceAlertOnClose(invoice, correspondenceNumber,
                                                                            correspondence.FromMemberId,
                                                                            correspondence.ToMemberId, scenarioId,
                                                                            SessionUtil.UserId, l_correspondence.ToEmailId,
                                                                            lastRespondedCorrespondence == null ? string.Empty : lastRespondedCorrespondence.AdditionalEmailInitiator,
                                                                            lastRespondedCorrespondence == null ? string.Empty : lastRespondedCorrespondence.AdditionalEmailNonInitiator);
                  ShowSuccessMessage(returnMsg);
              }
              else
              {
                  ShowErrorMessage(returnMsg, true);
              }
          }
      }
      else
      {
        invoice = _miscCorrespondenceManager.GetInvoiceHeaderDetail(invoiceId);
      }


      if (scenarioId <= 3 || scenarioId == 5 || scenarioId == 6)
      {
        return invoice.BillingCategoryId == (int) BillingCategorys.Miscellaneous
                 ? RedirectToAction("Index", "BillingHistory", new {area = "Misc"})
                 : RedirectToAction("Index", "BillingHistory", new {area = "UATP"});
      }
      else
      {
        return RedirectToAction("Correspondence", "Correspondence", new
        {
          invoiceId = invoice.Id.ToString()
        });
      }
    }


  }
}
