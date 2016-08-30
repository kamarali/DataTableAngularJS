using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Security;
using Iata.IS.Core;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using System.Web;
using log4net;
using System.Reflection;
using Iata.IS.Business.Pax;
using System.Threading;
using Iata.IS.Core.DI;

namespace Iata.IS.Web.Areas.Cargo.Controllers
{
  public class CorrespondenceController : ISController
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly ICargoCorrespondenceManager _cargoCorrespondenceManager;
    private readonly ICalendarManager _calendarManager;
    private readonly IAuthorizationManager _authorizationManager;
    private readonly ICorrespondenceManager _correspondenceManager;
    private const int CargoBillingCategory = 2;
    private const string CorrespondenceRejectionGridAction = "CorrespondenceRejectionGridData";
    private const string CorrespondenceHistoryGridAction = "CorrespondenceHistoryGridData";
    public ICargoInvoiceManager CargoInvoiceManager { get; set; }
    public IReferenceManager ReferenceManager { get; set; }

    public CorrespondenceController(ICargoCorrespondenceManager cargoCorresondenceManager, ICalendarManager calendarManager, IAuthorizationManager authorizationManager, ICorrespondenceManager correspondenceManager)
    {
      _cargoCorrespondenceManager = cargoCorresondenceManager;
      _calendarManager = calendarManager;
      _authorizationManager = authorizationManager;
      _correspondenceManager = correspondenceManager;
    }

    private void CheckCorrespondenceStatus(string transactionId)
    {
      var correspondenceInDB = _cargoCorrespondenceManager.GetCorrespondenceDetails(transactionId);

      if (correspondenceInDB != null && correspondenceInDB.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded)
      {
        throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadySent);
      }
    }

    /// <summary>
    /// Retrieves Correspondence details for given invoice id.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CargoCreateCorrespondenceFor(string invoiceId, string rejectionMemoIds)
    {
      SetViewDataPageMode(PageMode.Create);

      if (TempData == null || !TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        return null;
      }

      //var rejectedMemos = TempData[TempDataConstants.RejectedRecordIds].ToString();

      var sRejectedMemos = rejectionMemoIds.Split(',');

      TempData[TempDataConstants.RejectedRecordIds] = sRejectedMemos;

      var invoice = _cargoCorrespondenceManager.GetInvoiceDetail(invoiceId);
      var fromMember = _cargoCorrespondenceManager.GetMember(invoice.BilledMemberId);
      var toMember = _cargoCorrespondenceManager.GetMember(invoice.BillingMemberId);
      var toEmailIds = _cargoCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, ProcessingContactType.CargoCorrespondence);

      ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false);

      long correspondenceNumber;

      // Logic to set the correspondence number..
      correspondenceNumber = _cargoCorrespondenceManager.GetInitialCorrespondenceNumber(invoice.BilledMemberId);

      //if (_cargoCorrespondenceManager.IsFirstCorrespondence(invoice.BilledMemberId))
      //{
      //  correspondenceNumber = long.Parse(String.Format("{0}0000001", _cargoCorrespondenceManager.GetNumericMemberCode(fromMember.MemberCodeNumeric)));
      //}
      //else
      //{
      //  correspondenceNumber = _cargoCorrespondenceManager.GetCorrespondenceNumber(invoice.BilledMemberId);
      //  correspondenceNumber++;
      //}

      CheckCorrespondenceInitiator(null);

      var amountToBeSettled = 0.00M;

      // Amount to be settled will be the sum of the net reject amounts of all RMs.
      if (sRejectedMemos.Count() > 0)
      {
        // Retrieve rejectionMemo details
        var rejectionMemoDetails = sRejectedMemos.Select(rejMemoId => _cargoCorrespondenceManager.GetRejectedMemoDetails(rejMemoId));
        // Retrieve BillingCode i.e. NonSampling/Sampling
        var billingCode = rejectionMemoDetails.Select(rm => rm.Invoice.BillingCode).FirstOrDefault();

        // If BillingCode == 0 i.e. NonSampling set Correspondence settlement amount value to TotalNetRejectAmount, else for Sampling Correspondence set SettlementAmount field value to
        // TotalNetRejectAmountAfterSamplingConstant.
        if (billingCode == 0)
          amountToBeSettled = (decimal)rejectionMemoDetails.Where(rejectionMemo => rejectionMemo != null).Sum(rejectionMemo => rejectionMemo.TotalNetRejectAmount);
      }

      var correspondence = new CargoCorrespondence
      {
        CorrespondenceDate = DateTime.UtcNow,
        CorrespondenceStage = 1,
        FromMember = fromMember,
        FromMemberId = invoice.BilledMemberId,
        ToMember = toMember,
        ToMemberId = invoice.BillingMemberId,
        ToEmailId = toEmailIds,
        CorrespondenceNumber = correspondenceNumber,
        CorrespondenceStatus = CorrespondenceStatus.Open,
        CurrencyId = invoice.BillingCurrencyId,
        CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
        CorrespondenceOwnerId = SessionUtil.UserId,
        CorrespondenceOwnerName = SessionUtil.Username,
        //CMP#648: Convert Exchange rate into nullable field.
        AmountToBeSettled = invoice.ExchangeRate == 0 ? 0 : ConvertUtil.Round(amountToBeSettled / (invoice.ExchangeRate.HasValue ? invoice.ExchangeRate.Value : 0.0M), Constants.PaxDecimalPlaces),
        RejectionMemoIds = rejectionMemoIds
      };

      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                    Url.Action("InitiateCorrespondenceRejectionGridData",
                                              new
                                              {
                                                invoiceId = invoice.Id
                                              }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                    Url.Action(CorrespondenceHistoryGridAction,
                                              new
                                              {
                                                invoiceId = correspondence.Id
                                              }));

      return View("CargoCreateCorrespondence", correspondence);
    }

    /// <summary>
    /// Retrieves Correspondence details for given invoice id.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult CargoCreateCorrespondence(string invoiceId)
    {
      SetViewDataPageMode(PageMode.Create);

      if (TempData == null || !TempData.ContainsKey(TempDataConstants.RejectedRecordIds))
      {
        return null;
      }

      var rejectedMemos = TempData[TempDataConstants.RejectedRecordIds].ToString();

      var sRejectedMemos = rejectedMemos.Split(',');

      TempData[TempDataConstants.RejectedRecordIds] = sRejectedMemos;

      var invoice = _cargoCorrespondenceManager.GetInvoiceDetail(invoiceId);
      var fromMember = _cargoCorrespondenceManager.GetMember(invoice.BilledMemberId);
      var toMember = _cargoCorrespondenceManager.GetMember(invoice.BillingMemberId);
      var toEmailIds = _cargoCorrespondenceManager.GetToEmailIds(invoice.BillingMemberId, ProcessingContactType.CargoCorrespondence);

      ViewData[ViewDataConstants.InvoiceId] = invoiceId;
      //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false);

      long correspondenceNumber;

      // Logic to set the correspondence number..
      if (_cargoCorrespondenceManager.IsFirstCorrespondence(invoice.BilledMemberId))
      {
        correspondenceNumber = long.Parse(String.Format("{0}0000001", _cargoCorrespondenceManager.GetNumericMemberCode(fromMember.MemberCodeNumeric)));
      }
      else
      {
        correspondenceNumber = _cargoCorrespondenceManager.GetCorrespondenceNumber(invoice.BilledMemberId);
        correspondenceNumber++;
      }

      CheckCorrespondenceInitiator(null);

      var amountToBeSettled = 0.00M;

      // Amount to be settled will be the sum of the net reject amounts of all RMs.
      if (sRejectedMemos.Count() > 0)
      {
        // Retrieve rejectionMemo details
        var rejectionMemoDetails = sRejectedMemos.Select(rejMemoId => _cargoCorrespondenceManager.GetRejectedMemoDetails(rejMemoId));
        // Retrieve BillingCode i.e. NonSampling/Sampling
        var billingCode = rejectionMemoDetails.Select(rm => rm.Invoice.BillingCode).FirstOrDefault();

        // If BillingCode == 0 i.e. NonSampling set Correspondence settlement amount value to TotalNetRejectAmount, else for Sampling Correspondence set SettlementAmount field value to
        // TotalNetRejectAmountAfterSamplingConstant.
        if (billingCode == 0)
          amountToBeSettled = (decimal)rejectionMemoDetails.Where(rejectionMemo => rejectionMemo != null).Sum(rejectionMemo => rejectionMemo.TotalNetRejectAmount);
      }

      var correspondence = new CargoCorrespondence
      {
        CorrespondenceDate = DateTime.UtcNow,
        CorrespondenceStage = 1,
        FromMember = fromMember,
        FromMemberId = invoice.BilledMemberId,
        ToMember = toMember,
        ToMemberId = invoice.BillingMemberId,
        ToEmailId = toEmailIds,
        CorrespondenceNumber = correspondenceNumber,
        CorrespondenceStatus = CorrespondenceStatus.Open,
        CurrencyId = invoice.BillingCurrencyId,
        CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
        CorrespondenceOwnerId = SessionUtil.UserId,
        CorrespondenceOwnerName = SessionUtil.Username,
        //CMP#648: Convert Exchange rate into nullable field.
        AmountToBeSettled = invoice.ExchangeRate == 0 ? 0 : ConvertUtil.Round(amountToBeSettled / (invoice.ExchangeRate.HasValue ? invoice.ExchangeRate.Value : 0.0M), Constants.PaxDecimalPlaces)
      };

      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                    Url.Action("InitiateCorrespondenceRejectionGridData",
                                              new
                                              {
                                                invoiceId = invoice.Id
                                              }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                    Url.Action(CorrespondenceHistoryGridAction,
                                              new
                                              {
                                                invoiceId = correspondence.Id
                                              }));

      return View(correspondence);
    }

    /// <summary>
    /// Called on Save.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="correspondence"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.CGO_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public ActionResult CargoCreateCorrespondence(string invoiceId, CargoCorrespondence correspondence)
    {
      /* SCP# 106534 - ISWEB No-02350000768
      * Desc: Removing the thread sleep. Sleep was coded to ensure concurrency, but it really doesn't surve the purpose. Infact it was identified as the reason for the 
      * performance issues in correspondance screen. Hence removing this (Thread.Sleep(SessionUtil.UserId + 500);) line of code.
      * Date: 20/06/2013
      */
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Commented below code
      //Thread.Sleep(SessionUtil.UserId + 500);
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
      string rejectionMemoString = string.Empty;

      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //Retireve original correspondance details only if invoice id is null
      //Commented below line 
      //var originalCorr = _cargoCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);

      correspondence.CorrespondenceOwnerId = SessionUtil.UserId;

      /* SCP106534: ISWEB No-02350000768 
      Desc: Added support for operation status parameter
      Date: 20/06/2013*/
      int operationStatusIndicator = -1;

      try
      {
        if (invoiceId == null)
        {
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //Retireve original correspondance details only if invoice id is null
          var originalCorr = _cargoCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
          var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
          correspondence.Attachments.Clear();

          correspondence.InvoiceId = originalCorr.InvoiceId;
          correspondence = _cargoCorrespondenceManager.AddCorrespondence(correspondence);

          //Update parent Id for attachment
          _cargoCorrespondenceManager.UpdateCargoCorrespondenceAttachment(correspondenceAttachmentIds, correspondence.Id);
        }
        else
        {
          if (!string.IsNullOrWhiteSpace(correspondence.RejectionMemoIds))
          {
            char[] sep = { ',' };
            var sRejectedMemoIds = correspondence.RejectionMemoIds.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            if (sRejectedMemoIds != null)
            {
              rejectionMemoString = string.Join(",", sRejectedMemoIds);
              foreach (var rejMemoId in sRejectedMemoIds)
              {
                var rejectionMemo = _cargoCorrespondenceManager.GetRejectedMemoDetails(rejMemoId);
                if (rejectionMemo != null && rejectionMemo.CorrespondenceId.HasValue)
                {
                  ShowErrorMessage(ErrorCodes.ErrorCorrespondenceAlreadyCreated, true);

                  return RedirectToAction("EditCargoCorrespondence", new
                                                                       {
                                                                         transactionId = rejectionMemo.CorrespondenceId
                                                                       });
                }
              }
            }
          }
          //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //Commented below line , as used only to get invoice id using invoiceid
          //CargoInvoice invoice = _cargoCorrespondenceManager.GetInvoiceDetail(invoiceId);
          //Get attachment Id list
          var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
          correspondence.Attachments.Clear();

          correspondence.InvoiceId = invoiceId.ToGuid();
          // correspondence = _cargoCorrespondenceManager.AddCorrespondence(correspondence);
          correspondence = _cargoCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence, correspondenceAttachmentIds, rejectionMemoString, ref operationStatusIndicator);
        }
        ShowSuccessMessage(Messages.CargoCorrespondenceCreateSuccessful);

        return RedirectToAction("EditCargoCorrespondence", new
        {
          transactionId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
          /*SCP159751: Create Correspondence for awb 784-41554763
            Desc: Code is added to display business exception error message on screen.
            Date: 02-Aug-2013*/
          ShowErrorMessage(exception.ErrorCode, true);

        /* SCP106534: ISWEB No-02350000768 
          Desc: Added support for operation status parameter.
              * it is 1 or 2 or 3 then this is business error while adding corr in DB because of paralel operation so it is success
          Date: 20/06/2013*/
        if (operationStatusIndicator != -1)
        {
          ShowErrorMessage(exception.ErrorCode, true);
          return RedirectToAction("EditCargoCorrespondence", new
          {
            transactionId = correspondence.Id.Value()
          });
        }

        var billingMember = _cargoCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _cargoCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;

        ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;
      }

      char[] sep1 = { ',' };
      var rejectedMemoIds = string.IsNullOrWhiteSpace(correspondence.RejectionMemoIds) ?
        null : correspondence.RejectionMemoIds.Split(sep1, StringSplitOptions.RemoveEmptyEntries);

      if (TempData != null)
      {
        var previousCorrespondenceId = TempData["PreviousCorrespondenceId"] ?? Guid.Empty;
        if (rejectedMemoIds != null)
        {
          string sRejectedMemoIds = string.Join(",", rejectedMemoIds);
          GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                    new
                                                                                                    {
                                                                                                      invoiceId = correspondence.InvoiceId,
                                                                                                      rejectedMemoIds = sRejectedMemoIds
                                                                                                    }));
          TempData[TempDataConstants.RejectedRecordIds] = rejectedMemoIds;
        }
        else
        {
          GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                      correspondence.Id == Guid.Empty
                                        ? Url.Action(CorrespondenceRejectionGridAction,
                                                     new { invoiceId = previousCorrespondenceId })
                                        : Url.Action(CorrespondenceRejectionGridAction,
                                                     new { invoiceId = correspondence.Id }));
        }

        if (correspondence.Id != Guid.Empty)
        {
          GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                       Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
          CheckCorrespondenceInitiator(correspondence.Id.ToString());
        }
        else
        {
          GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                       Url.Action(CorrespondenceHistoryGridAction,
                                                  new { invoiceId = previousCorrespondenceId }));
          CheckCorrespondenceInitiator(previousCorrespondenceId.ToString());
        }

        TempData["PreviousCorrespondenceId"] = previousCorrespondenceId;
      }

      return View(correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                         CorrespondenceParamName = "correspondence",
                         InvList = false, TableName = TransactionTypeTable.CGO_CORRESPONDENCE,
                         ActionParamName = "ViewCorrespondence")]
    public ActionResult CreateAndSendCorrespondence(CargoCorrespondence correspondence, string invoiceId)
    {
      /* SCP# 106534 - ISWEB No-02350000768
         * Desc: Removing the thread sleep. Sleep was coded to ensure concurrency, but it really doesn't surve the purpose. Infact it was identified as the reason for the 
         * performance issues in correspondance screen. Hence removing this (Thread.Sleep(SessionUtil.UserId + 500);) line of code.
         * Date: 20/06/2013
         */
      Logger.InfoFormat("Start CreateAndSendCorrespondence time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      /* SCP# 106534 - ISWEB No-02350000768
           * Desc: Sattus is Responded and not Saved.
           * Date: 20/06/2013
           */
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;
      //var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      //correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);

      correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
      /* SCP106534: ISWEB No-02350000768 
      Desc: Added support for operation status parameter
      Date: 20/06/2013*/
      int operationStatusIndicator = -1;

      try
      {
        //Get attachment Id list
        var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
        correspondence.Attachments.Clear();
        var originalCorrespondence = _cargoCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
        var invoiceIdToInsert = Guid.Empty;
        string rejectionMemoString = string.Empty;

        if (!string.IsNullOrWhiteSpace(correspondence.RejectionMemoIds) && correspondence.CorrespondenceStage == 1)
        {
          char[] sep = { ',' };
          var sRejectedMemoIds = correspondence.RejectionMemoIds.Split(sep, StringSplitOptions.RemoveEmptyEntries);
          if (sRejectedMemoIds != null)
          {
            rejectionMemoString = string.Join(",", sRejectedMemoIds);
            foreach (var rejMemoId in sRejectedMemoIds)
            {
              var rejectionMemo = _cargoCorrespondenceManager.GetRejectedMemoDetails(rejMemoId);
              if (rejectionMemo != null && rejectionMemo.CorrespondenceId.HasValue)
              {
                ShowErrorMessage(ErrorCodes.ErrorCorrespondenceAlreadyCreated, true);

                return RedirectToAction("EditCargoCorrespondence", new
                {
                  transactionId = rejectionMemo.CorrespondenceId
                });
              }

              invoiceIdToInsert = rejectionMemo.InvoiceId;
            }
          }
        }
        else
        {
          if (originalCorrespondence != null)
          {
            invoiceIdToInsert = originalCorrespondence.InvoiceId;
          }
        }

        if (invoiceIdToInsert == Guid.Empty)
          if (invoiceId != null)
          {
            invoiceIdToInsert = invoiceId.ToGuid();
          }
        correspondence.InvoiceId = invoiceIdToInsert;
        // correspondence = _cargoCorrespondenceManager.AddCorrespondence(correspondence);

        /* SCP# 106534 - ISWEB No-02350000768
           * Desc: Send on Date assigned for correspondence
           * Date: 20/06/2013
           */
        var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
        correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);

        //SCP#446268: Correspondence Problem (Check if TO_Email ID is blank re-fetch the contacts from Profile)
        if (String.IsNullOrEmpty(correspondence.ToEmailId))
        {
            //Mail ids of correspondence contact of To Member.
            var toEmail = _cargoCorrespondenceManager.GetToEmailIds(correspondence.ToMemberId, ProcessingContactType.CargoCorrespondence);
            if (!String.IsNullOrEmpty(toEmail))
                correspondence.ToEmailId = toEmail;
        }

        correspondence = _cargoCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence, correspondenceAttachmentIds, rejectionMemoString, ref operationStatusIndicator);

        var correspondenceUrl = string.Format("{0}/{1}", UrlHelperEx.ToAbsoluteUrl(Url.Action("ViewLinkedCorrespondence", "Correspondence")), correspondence.Id);

        // Send the correspondence.

        /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids of To,initiator and non-initiator*/
        var toEmailIds = _correspondenceManager.GetEmailIdsList(correspondence.ToEmailId,
                                                                      correspondence.AdditionalEmailInitiator,
                                                                      correspondence.AdditionalEmailNonInitiator);

        if (_cargoCorrespondenceManager.ValidateCorrespondence(correspondence))
        {
          Logger.InfoFormat("Before CreateAndSendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
          // CMP#657: Retention of Additional Email Addresses in Correspondences.
          // Logic Moved to common location. i.e in corresponceManager.cs
          //_cargoCorrespondenceManager.SendCorrespondenceMail(correspondenceUrl, toEmailIds, string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject));
          var frmMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.FromMemberId);
          var toMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.ToMemberId);
          _correspondenceManager.EmailAlertsOnSendingOfCorrespondences(BillingCategoryType.Pax, correspondenceUrl, toEmailIds,
                                                                       string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject),
                                                                       string.Format("{0}-{1}", frmMember.MemberCodeAlpha, frmMember.MemberCodeNumeric),
                                                                       string.Format("{0}-{1}", toMember.MemberCodeAlpha, toMember.MemberCodeNumeric));
          Logger.InfoFormat("After CreateAndSendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
        }

        /* SCP# 106534 - ISWEB No-02350000768
           * Desc: No need to update correspondence now.
           * Date: 20/06/2013
           */
        //correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;
        //correspondence = _cargoCorrespondenceManager.UpdateCorrespondence(correspondence);

        ShowSuccessMessage(Messages.MiscCorrespondenceSentSuccessful);

        return RedirectToAction("Correspondence", "Correspondence", new
        {
          invoiceId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
            /*SCP159751: Create Correspondence for awb 784-41554763
            Desc: Code is added to display business exception error message on screen.
            Date: 02-Aug-2013*/
            ShowErrorMessage(exception.ErrorCode, true);

        /* SCP106534: ISWEB No-02350000768 
        Desc: Added support for operation status parameter.
         * it is 1 or 2 or 3 then this is business error while adding corr in DB because of paralel operation so it is success
        Date: 20/06/2013*/
        if (operationStatusIndicator != -1)
        {
          ShowErrorMessage(exception.ErrorCode, true);
          return RedirectToAction("Correspondence", "Correspondence", new
          {
            invoiceId = correspondence.Id.Value()
          });
        }

        ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;
        var billingMember = _cargoCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _cargoCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
      }

      var rejectedMemoIds = TempData[TempDataConstants.RejectedRecordIds] as string[];

      if (TempData != null)
      {
        var previousCorrespondenceId = TempData["PreviousCorrespondenceId"] ?? Guid.Empty;
        if (rejectedMemoIds != null)
        {
          string sRejectedMemoIds = string.Join(",", rejectedMemoIds);
          GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                    new
                                                                                                    {
                                                                                                      invoiceId = correspondence.InvoiceId,
                                                                                                      rejectedMemoIds = sRejectedMemoIds
                                                                                                    }));
          TempData[TempDataConstants.RejectedRecordIds] = rejectedMemoIds;
        }
        else
        {
          GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                      correspondence.Id == Guid.Empty
                                        ? Url.Action(CorrespondenceRejectionGridAction,
                                                     new { invoiceId = previousCorrespondenceId })
                                        : Url.Action(CorrespondenceRejectionGridAction,
                                                     new { invoiceId = correspondence.Id }));
        }

        if (correspondence.Id != Guid.Empty)
        {
          GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                       Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
          CheckCorrespondenceInitiator(correspondence.Id.ToString());
        }
        else
        {
          GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                       Url.Action(CorrespondenceHistoryGridAction,
                                                  new { invoiceId = previousCorrespondenceId }));
          CheckCorrespondenceInitiator(previousCorrespondenceId.ToString());
        }

        TempData["PreviousCorrespondenceId"] = previousCorrespondenceId;
      }

      return View("CargoCreateCorrespondence", correspondence);
    }



    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    [HttpGet]
    public ActionResult Correspondence(string invoiceId)
    {
      var correspondence = _cargoCorrespondenceManager.GetCorrespondenceDetails(invoiceId);

      ViewData[ViewDataConstants.InvoiceId] = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(invoiceId).Id;

      //check whether transactions exist for this invoice.
      if (correspondence != null)
      {
          //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
          ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
            _cargoCorrespondenceManager.IsCorrespondenceEligibleForReply((int)BillingCategoryType.Cgo, correspondence.Id,
                                                                          SessionUtil.UserId, SessionUtil.MemberId,
                                                                         Business.Security.Permissions.Cargo.
                                                                           BillingHistoryAndCorrespondence.
                                                                           BillingHistoryAndCorrespondence.
                                                                           DraftCorrespondence);
   
       //INC 8863, I get an unexpected error occurred.
       // var linkedRejections =
       //   _cargoCorrespondenceManager.GetCorrespondenceRejectionList(invoiceId).Select(rejection => rejection.Id);
       // TempData[TempDataConstants.RejectedRecordIds] = string.Join(",", linkedRejections);

        if (correspondence.CorrespondenceStatus != CorrespondenceStatus.Closed &&
            (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved ||
             correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit) &&
            correspondence.FromMemberId == SessionUtil.MemberId)
        {
          return RedirectToAction("EditCargoCorrespondence", "Correspondence",
                                  new {transactionId = correspondence.Id});
        }
        ViewData[ViewDataConstants.TransactionExists] = true;

        //CMP527: Start 
        if (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence))
        {
          var corrCanClose = _cargoCorrespondenceManager.CanCorrespondenceClose(CargoBillingCategory, invoiceId);
          var closeCorrespondence =
            Convert.ToBoolean(corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCanClose]);

          if (closeCorrespondence)
          {
            var correspondenceInDb = _cargoCorrespondenceManager.GetFirstCorrespondenceDetails(invoiceId);
            ViewData[ViewDataConstants.CorrespondenceCanClose] = correspondenceInDb.FromMemberId == SessionUtil.MemberId;
            ViewData[ViewDataConstants.CorrespondeneClosedScenario] = corrCanClose[(int) CorrespondenceCloseStatus.CorrespondenceCloseScenario];
          }
        }
        //CMP527: End
      }
      else
      {
        return RedirectToAction("CargoCreateCorrespondence", "Correspondence", new { invoiceId });
      }

      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                  Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                  Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));

      ViewData[ViewDataConstants.ReplyCorrespondence] = false;



      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator)
      {
        correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;
      }

      return View("CargoCorrespondence", correspondence);
    }

    private void CheckCorrespondenceInitiator(string correspondenceId)
    {
      if (string.IsNullOrEmpty(correspondenceId))
      {
        ViewData[ViewDataConstants.CorrespondenceInitiator] = true;
        return;
      }

      var correspondenceInDb = _cargoCorrespondenceManager.GetFirstCorrespondenceDetails(correspondenceId);
      ViewData[ViewDataConstants.CorrespondenceInitiator] = correspondenceInDb != null && correspondenceInDb.FromMemberId == SessionUtil.MemberId || correspondenceInDb == null;
    }

    private void GetCorrespondenceRejectGrid(string conrolId, string urlAction)
    {
      var gridModel = new CargoCorrespondenceRejectionsGrid(conrolId, urlAction);
      ViewData[ViewDataConstants.CorrespondenceRejectionsGrid] = gridModel.Instance;
    }

    private void GetCorrespondenceHistoryGrid(string conrolId, string urlAction)
    {
      var historyGridModel = new CargoCorrespondenceHistoryGrid(conrolId, urlAction);
      ViewData[ViewDataConstants.CorrespondenceHistoryGrid] = historyGridModel.Instance;
    }

    public JsonResult InitiateCorrespondenceRejectionGridData(string invoiceId)
    {
      var correspondenceRejectionGrid = new CargoCorrespondenceRejectionsGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData", new { invoiceId }));

      var invoice = _cargoCorrespondenceManager.GetInvoiceDetail(invoiceId);
      if (invoice != null)
      {
        var rejectedMemoIds = SessionUtil.RejectionMemoRecordIds;
        var billingMember = string.Format("{0}-{1}-{2}", invoice.BillingMember.MemberCodeAlpha, invoice.BillingMember.MemberCodeNumeric, invoice.BillingMember.CommercialName);
        var period = string.Format("{0} {1} P{2}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(invoice.BillingMonth), invoice.BillingYear, invoice.BillingPeriod);
        string[] sRejectedMemoIds = rejectedMemoIds.Split(new[] { ',' });

        var linkedRejections = sRejectedMemoIds.Select(rejectionMemoId => new LinkedRejection
        {
          Id = rejectionMemoId,
          InvoiceId = invoice.Id.ToString(),
          BillingMemberText = billingMember,
          DisplayBillingPeriod = period,
          InvoiceNumber = invoice.InvoiceNumber,
          RejectionMemoNumber = _cargoCorrespondenceManager.GetRejectedMemoDetails(rejectionMemoId).RejectionMemoNumber,
          BillingCode = invoice.BillingCode
        }).ToList().AsQueryable();

        return correspondenceRejectionGrid.DataBind(linkedRejections);
      }

      return null;
    }

    [HttpPost]
    public JsonResult CheckIfBMExistsOnReply(long correspondenceRefNumber)
    {
      int toMemberId = SessionUtil.MemberId;

      var billingMemos = CargoInvoiceManager.GetBillingMemosForCorrespondence(correspondenceRefNumber, toMemberId);
      if (billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
      {
          return Json(new UIMessageDetail
          {
              //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
              //Message = string.Format(Messages.CannotReplyToCorrespondence, billingMemos.Transactions[0].BillingMemoNumber, billingMemos.Transactions[0].InvoiceNumber),
              Message =
                  string.Format(Messages.BCGO_10380,
                      billingMemos.Transactions[0].InvoiceNumber,
                      billingMemos.Transactions[0].InvoicePeriod,
                      billingMemos.Transactions[0].BatchNumber,
                      billingMemos.Transactions[0].SequenceNumber,
                      billingMemos.Transactions[0].BillingMemoNumber),
              IsFailed = true
          });
      }

      return Json(new UIMessageDetail
      {
        IsFailed = false
      });
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.SendCorrespondence)]
    public ActionResult ReplyCorrespondence(string transactionId)
    {
      var correspondenceInDb = _cargoCorrespondenceManager.GetCorrespondenceDetails(transactionId);
      var invoice = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId);

      ViewData[ViewDataConstants.InvoiceId] = invoice.Id.ToString();
      //ViewData[ViewDataConstants.SettlementMethodId] = invoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false);

      CheckCorrespondenceInitiator(transactionId);

      //Mail ids of correspondence contact of to member.
      var toEmailIds = _cargoCorrespondenceManager.GetToEmailIds(correspondenceInDb.FromMemberId, ProcessingContactType.CargoCorrespondence);
      var correspondence = new CargoCorrespondence
      {
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
        CorrespondenceNumber = correspondenceInDb.CorrespondenceNumber,
        YourReference = correspondenceInDb.OurReference,
        CorrespondenceStatus = CorrespondenceStatus.Open,
        CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit,
        AuthorityToBill = correspondenceInDb.AuthorityToBill,
        Subject = correspondenceInDb.Subject,
          /* CMP#657: Retention of Additional Email Addresses in Correspondences
                   Adding code to get email ids from initiator and non-initiator and removing
                   additional email field*/
        AdditionalEmailInitiator = correspondenceInDb.AdditionalEmailInitiator,
        AdditionalEmailNonInitiator = correspondenceInDb.AdditionalEmailNonInitiator
      };

      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                  Url.Action(CorrespondenceRejectionGridAction,
                                             new
                                             {
                                               invoiceId = transactionId
                                             }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                   Url.Action(CorrespondenceHistoryGridAction,
                                              new
                                              {
                                                invoiceId = transactionId
                                              }));

      TempData["PreviousCorrespondenceId"] = transactionId;
      return View("CargoCreateCorrespondence", correspondence);
    }

    public FilePathResult DownloadCorrespondence(string transactionId)
    {
      var IsclosedByInitiator = true;
      FileStream file = null;
      try
      {
        string pdfPath = _cargoCorrespondenceManager.CreateCorrespondenceFormatPdf(transactionId);

        file = System.IO.File.Open(pdfPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return File(pdfPath, "application/pdf");
      }
      catch (Exception)
      {
        return null;
      }
      finally
      {
        if (file != null)
        {
          file.Close();
        }
      }
    }

    /// <summary>
    /// Download Correspondence Attachment  
    /// </summary>
    /// <param name="invoiceId">Invoice Id</param>
    /// <returns></returns>
    // [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.Download)]
    [HttpGet]
    public FileStreamResult CorrespondenceAttachmentDownload(string invoiceId)
    {
      var fileDownloadHelper = new FileAttachmentHelper
      {
        Attachment = _cargoCorrespondenceManager.GetCorrespondenceAttachmentDetail(invoiceId)
      };

      return File(fileDownloadHelper.DownloadFile(), "application/octet", fileDownloadHelper.Attachment.OriginalFileName);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    public ActionResult ViewLinkedCorrespondence(string invoiceId)
    {
      var correspondence = _cargoCorrespondenceManager.GetCorrespondenceDetails(invoiceId);

      //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
      ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
        _cargoCorrespondenceManager.IsCorrespondenceEligibleForReply((int) BillingCategoryType.Cgo, correspondence.Id,
                                                                      SessionUtil.UserId, SessionUtil.MemberId,
                                                                     Business.Security.Permissions.Cargo.
                                                                       BillingHistoryAndCorrespondence.
                                                                       BillingHistoryAndCorrespondence.
                                                                       DraftCorrespondence);

      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));

      ViewData[ViewDataConstants.InvoiceId] = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(invoiceId).Id;

      SetViewDataPageMode(PageMode.View);
      if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;

      return View("CargoCorrespondence", correspondence);
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(InvParamName = "invoiceId",
                         TransactionParamName = "transactionId",
                         CorrespondenceParamName = "correspondence",
                         InvList = false, TableName = TransactionTypeTable.CGO_CORRESPONDENCE,
                         ActionParamName = "ViewCorrespondence")]
    public ActionResult ReadyToSubmitCorrespondence(string invoiceId, string transactionId, CargoCorrespondence correspondence)
    {
      /* SCP# 106534 - ISWEB No-02350000768
         * Desc: Removing the thread sleep. Sleep was coded to ensure concurrency, but it really doesn't surve the purpose. Infact it was identified as the reason for the 
         * performance issues in correspondance screen. Hence removing this (Thread.Sleep(SessionUtil.UserId + 500);) line of code.
         * Date: 20/06/2013
         */
      //Get attachment Id list
      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      string rejectionMemoString = string.Empty;
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.ReadyForSubmit;
      /* SCP106534: ISWEB No-02350000768 
          Desc: Added support for operation status parameter
          Date: 20/06/2013*/
      int operationStatusIndicator = -1;

      try
      {
        if (transactionId.ToGuid() == Guid.Empty)
        {

          if (!string.IsNullOrWhiteSpace(correspondence.RejectionMemoIds))
          {
            char[] sep = { ',' };
            var sRejectedMemoIds = correspondence.RejectionMemoIds.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            rejectionMemoString = string.Join(",", sRejectedMemoIds);
            foreach (var rejMemoId in sRejectedMemoIds)
            {
              var rejectionMemo = _cargoCorrespondenceManager.GetRejectedMemoDetails(rejMemoId);
              if (rejectionMemo != null && rejectionMemo.CorrespondenceId.HasValue)
              {
                ShowErrorMessage(ErrorCodes.ErrorCorrespondenceAlreadyCreated, true);

                return RedirectToAction("EditCargoCorrespondence", new
                {
                  transactionId = rejectionMemo.CorrespondenceId
                });
              }
            }
          }

          correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
          var originalCorr = _cargoCorrespondenceManager.GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
          correspondence.InvoiceId = originalCorr != null ? originalCorr.InvoiceId : invoiceId.ToGuid();
          correspondence.Attachments.Clear();
          // correspondence = _cargoCorrespondenceManager.AddCorrespondence(correspondence);
          correspondence = _cargoCorrespondenceManager.AddCorrespondenceAndUpdateRejection(correspondence, correspondenceAttachmentIds, rejectionMemoString, ref operationStatusIndicator);
        }
        else
        {
          correspondence.Id = transactionId.ToGuid();
          CheckCorrespondenceStatus(transactionId);

          var corrDetails = _cargoCorrespondenceManager.GetCorrespondenceDetails(transactionId);
          if (corrDetails != null)
          {
            correspondence.InvoiceId = corrDetails.InvoiceId;
            correspondence.CorrespondenceOwnerId = corrDetails.CorrespondenceOwnerId;

            // SCP109163
            // Check if correspondence expiry date is crossed.
            if (corrDetails.ExpiryDate < DateTime.UtcNow.Date)
            {
              throw new ISBusinessException(CargoErrorCodes.ExpiredCorrespondence);
            }
          }

          correspondence = _cargoCorrespondenceManager.UpdateCorrespondence(correspondence);
          ShowSuccessMessage(Messages.MiscCorrespondenceUpdateSuccessful);
        }

        return RedirectToAction("EditCargoCorrespondence", new
        {
          transactionId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
          /*SCP159751: Create Correspondence for awb 784-41554763
            Desc: Code is added to display business exception error message on screen.
            Date: 02-Aug-2013*/
          ShowErrorMessage(exception.ErrorCode, true);
        /* SCP106534: ISWEB No-02350000768 
          Desc: Added support for operation status parameter.
           * it is 1 or 2 or 3 then this is business error while adding corr in DB because of paralel operation so it is success
          Date: 20/06/2013*/
        if (operationStatusIndicator != -1)
        {
          ShowErrorMessage(exception.ErrorCode, true);
          return RedirectToAction("EditCargoCorrespondence", new
          {
            transactionId = correspondence.Id.Value()
          });
        }

        var billingMember = _cargoCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _cargoCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
        ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;
        correspondence.Attachments = _cargoCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
      }

      char[] sep1 = { ',' };
      var rejectedMemoIds = string.IsNullOrWhiteSpace(correspondence.RejectionMemoIds) ? null 
                                                                                       : correspondence.RejectionMemoIds.Split(sep1, StringSplitOptions.RemoveEmptyEntries);

      if (TempData != null)
      {
        var previousCorrespondenceId = TempData["PreviousCorrespondenceId"] ?? Guid.Empty;
        if (rejectedMemoIds != null)
        {
          string sRejectedMemoIds = string.Join(",", rejectedMemoIds);
          GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                         new {
                                                                                                invoiceId = correspondence.InvoiceId,
                                                                                                rejectedMemoIds = sRejectedMemoIds
                                                                                              }));
          TempData[TempDataConstants.RejectedRecordIds] = rejectedMemoIds;
        }
        else
        {
          GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, correspondence.Id == Guid.Empty ? Url.Action(CorrespondenceRejectionGridAction,
                                                                                        new { invoiceId = previousCorrespondenceId })
                                                                                            : Url.Action(CorrespondenceRejectionGridAction,
                                                                                        new { invoiceId = correspondence.Id }));
        }

        if (correspondence.Id != Guid.Empty)
        {
          GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
          CheckCorrespondenceInitiator(correspondence.Id.ToString());
        }
        else
        {
          GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, 
                                                                                       new { invoiceId = previousCorrespondenceId }));
          CheckCorrespondenceInitiator(previousCorrespondenceId.ToString());
        }

        TempData["PreviousCorrespondenceId"] = previousCorrespondenceId;
      }
      
      if (string.IsNullOrEmpty(transactionId) || transactionId.Equals(Guid.Empty.ToString()))
      {
        return View("CargoCreateCorrespondence", correspondence);
      }
      return RedirectToAction("EditCargoCorrespondence", new
      {
        transactionId = correspondence.Id.Value()
      });
    }

    /// <summary>
    /// Update Cargo Correspondence
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpGet]
    [OutputCache(CacheProfile = "donotCache")]
    public ActionResult EditCargoCorrespondence(string transactionId)
    {
      SetViewDataPageMode(PageMode.Edit);

      var correspondence = _cargoCorrespondenceManager.GetCorrespondenceDetails(transactionId);

      // SCP251331: FW: Rare Occuring Issue found for Closed Pax Correspondence Functionalit
      if (correspondence == null)
      {
        return RedirectToAction("Index", "BillingHistory");
      }

      // Retrieve Coreespondence related Invoice
      var correspondencerelatedInvoice = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId);
      ViewData[ViewDataConstants.InvoiceId] = correspondencerelatedInvoice.Id;
      // Set SettlementMethodId in ViewData
      //ViewData[ViewDataConstants.SettlementMethodId] = correspondencerelatedInvoice.SettlementMethodId;
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      ViewData[ViewDataConstants.IsSMILikeBilateral] = ReferenceManager.IsSmiLikeBilateral(correspondencerelatedInvoice.SettlementMethodId, false);
      CheckCorrespondenceInitiator(transactionId);

      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = transactionId }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = transactionId }));

      //CMP527:
      if (_authorizationManager.IsAuthorized(SessionUtil.UserId, Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.AcceptAndCloseCorrespondence))
      {
        var corrCanClose = _cargoCorrespondenceManager.CanCorrespondenceClose(CargoBillingCategory, transactionId);
        var closeCorrespondence = Convert.ToBoolean(corrCanClose[(int)CorrespondenceCloseStatus.CorrespondenceCanClose]);

        if (closeCorrespondence)
        {
          var correspondenceInDb = _cargoCorrespondenceManager.GetFirstCorrespondenceDetails(transactionId);
          ViewData[ViewDataConstants.CorrespondenceCanClose] = correspondenceInDb.FromMemberId == SessionUtil.MemberId;
          ViewData[ViewDataConstants.CorrespondeneClosedScenario] = corrCanClose[(int)CorrespondenceCloseStatus.CorrespondenceCloseScenario];
        }

      }
      //CMP527 End;

      // SCP109163
      // Check if correspondence expiry date is crossed.
      if (correspondence.ExpiryDate < DateTime.UtcNow.Date)
      {
        ShowErrorMessage(CargoErrorCodes.ExpiredCorrespondence);
      }

      return View(correspondence);
    }

    /// <summary>
    /// Update Cargo Correspondence
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="correspondence"></param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.DraftCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(TransactionParamName = "transactionId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.CGO_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public ActionResult EditCargoCorrespondence(string transactionId, CargoCorrespondence correspondence)
    {
      var correspondenceAttachmentIds = correspondence.Attachments.Select(attachment => attachment.Id).ToList();
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Saved;
      try
      {
        correspondence.Id = transactionId.ToGuid();

        // Check if correspondece is already in send status.
        if (!string.IsNullOrWhiteSpace(transactionId))
        {
          CheckCorrespondenceStatus(transactionId);
        }

        var corr = _cargoCorrespondenceManager.GetCorrespondenceDetails(transactionId);
        correspondence.InvoiceId = corr.InvoiceId;
        correspondence.CorrespondenceOwnerId = corr.CorrespondenceOwnerId;

        // Assign expiry date to main correspondence object
        correspondence.ExpiryDate = corr.ExpiryDate;

        // SCP109163
        // Check if correspondece is already in send status.
        if (corr.ExpiryDate < DateTime.UtcNow.Date)
        {
          throw new ISBusinessException(CargoErrorCodes.ExpiredCorrespondence);
        }



        correspondence = _cargoCorrespondenceManager.UpdateCorrespondence(correspondence);

        ViewData[ViewDataConstants.InvoiceId] = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId).Id;

        ShowSuccessMessage(Messages.MiscCorrespondenceUpdateSuccessful);

        return RedirectToAction("EditCargoCorrespondence", new
        {
          transactionId = correspondence.Id.Value()
        });
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);

        correspondence.Attachments = _cargoCorrespondenceManager.GetCorrespondenceAttachments(correspondenceAttachmentIds);
        var billingMember = _cargoCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _cargoCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;
      }
      CheckCorrespondenceInitiator(correspondence.Id.ToString());
      ViewData[ViewDataConstants.InvoiceId] = correspondence.InvoiceId;
      GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId = correspondence.Id }));
      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));
      return View(correspondence);
    }

    [ValidateAntiForgeryToken]
    [ISAuthorize(Business.Security.Permissions.Cargo.Receivables.CreateInvoice.CreateOrEdit)]
    [HttpPost]
    public JsonResult CorrespondenceAttachmentUpload(string invoiceId, string transactionId)
    {
      var files = string.Empty;
      var attachments = new List<CargoCorrespondenceAttachment>();
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

          if (invoiceId == null)
          {
            invoiceId = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId).Id.ToString();
          }

          CargoInvoice invoice = _cargoCorrespondenceManager.GetInvoiceDetail(invoiceId);

          fileUploadHelper = new FileAttachmentHelper
          {
            FileToSave = fileToSave,
            FileRelativePath = String.Format("{0}_{1}_{2}", invoice.BilledMemberId, invoice.BillingYear,
                                      invoice.BillingMonth)
          };

          //On Correpondence create/update
          if (!Equals(transactionId.ToGuid(), Guid.Empty) && _cargoCorrespondenceManager.IsDuplicateCargoCorrespondenceAttachmentFileName(fileUploadHelper.FileOriginalName,
                                                                             transactionId.ToGuid()))
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
            var attachment = new CargoCorrespondenceAttachment
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

            attachment = _cargoCorrespondenceManager.AddCorrespondenceAttachment(attachment);
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
        if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
      }
      catch (Exception)
      {
        message = Messages.FileUploadUnexpectedError;
        if (fileUploadHelper != null && isUploadSuccess == false) fileUploadHelper.DeleteFile();
      }

      return new FileUploadJsonResult { Data = new { IsFailed = !isUploadSuccess, Message = message, Attachment = attachments, Length = attachments.Count } };
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.SendCorrespondence)]
    [HttpPost]
    //Check for Correspondence Updates.
    [RestrictInvoiceUpdate(TransactionParamName = "transactionId",
                           CorrespondenceParamName = "correspondence",
                           InvList = false, TableName = TransactionTypeTable.CGO_CORRESPONDENCE,
                           ActionParamName = "ViewCorrespondence")]
    public ActionResult SendCorrespondence(string transactionId, CargoCorrespondence correspondence)
    {
      Logger.InfoFormat("Start SendCorrespondence time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
      correspondence.CorrespondenceStatus = CorrespondenceStatus.Open;
      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Responded;

      correspondence.CorrespondenceOwnerId = SessionUtil.UserId;
      var currentPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
      correspondence.CorrespondenceSentOnDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);

      try
      {
        correspondence.Id = transactionId.ToGuid();

        // Check if correspondence is already in responded status.
        if (!string.IsNullOrWhiteSpace(transactionId))
        {
          CheckCorrespondenceStatus(transactionId);
        }

        //SCP#446268: Correspondence Problem (Check if TO_Email ID is blank re-fetch the contacts from Profile)
        if (String.IsNullOrEmpty(correspondence.ToEmailId))
        {
            //Mail ids of correspondence contact of To Member.
            var toEmail = _cargoCorrespondenceManager.GetToEmailIds(correspondence.ToMemberId, ProcessingContactType.CargoCorrespondence);
            if (!String.IsNullOrEmpty(toEmail))
                correspondence.ToEmailId = toEmail;
        }

        // Get the absolute url for the correspondence.
        var correspondenceUrl = string.Format("{0}/{1}", UrlHelperEx.ToAbsoluteUrl(Url.Action("ViewCorrespondence", "Correspondence")), transactionId);

        /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids of To,initiator and non-initiator*/
        var toEmailIds = _correspondenceManager.GetEmailIdsList(correspondence.ToEmailId,
                                                                      correspondence.AdditionalEmailInitiator,
                                                                      correspondence.AdditionalEmailNonInitiator);

        var savedCorr = _cargoCorrespondenceManager.GetCorrespondenceDetails(transactionId);
        correspondence.InvoiceId = savedCorr.InvoiceId;



        // SCP109163
        // Check if correspondence expiry date is crossed.
        if (savedCorr.ExpiryDate < DateTime.UtcNow.Date)
        {
          throw new ISBusinessException(CargoErrorCodes.ExpiredCorrespondence);
        }

        if (_cargoCorrespondenceManager.ValidateCorrespondence(correspondence))
        {
          Logger.InfoFormat("Before SendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
          // CMP#657: Retention of Additional Email Addresses in Correspondences.
          // Logic Moved to common location. i.e in corresponceManager.cs
          // _cargoCorrespondenceManager.SendCorrespondenceMail(correspondenceUrl, toEmailIds, string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject));
          var frmMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.FromMemberId);
          var toMember = _correspondenceManager.GetCachedCopyOfMemberUsingId(correspondence.ToMemberId);
          _correspondenceManager.EmailAlertsOnSendingOfCorrespondences(BillingCategoryType.Pax, correspondenceUrl, toEmailIds,
                                                                       string.Format("{0}-{1}-{2}", "SIS:Correspondence", correspondence.CorrespondenceNumber, correspondence.Subject),
                                                                       string.Format("{0}-{1}", frmMember.MemberCodeAlpha, frmMember.MemberCodeNumeric),
                                                                       string.Format("{0}-{1}", toMember.MemberCodeAlpha, toMember.MemberCodeNumeric));
          Logger.InfoFormat("After SendCorrespondence email time: {0}", DateTime.Now.ToString("yyyy-MMM-dd HH:mm:ss:ffff"));
        }

        /* SCP 270815: FW: CGO Correspondence Expiry Dates
         * Description: Correspondence date is set to current value. This date is internally used to set (correspondence) Expiry Date
         */
        correspondence.CorrespondenceDate = DateTime.UtcNow.Date;
        correspondence = _cargoCorrespondenceManager.UpdateCorrespondence(correspondence);

        ShowSuccessMessage(Messages.MiscCorrespondenceSentSuccessful);

        return RedirectToAction("Correspondence", "Correspondence", new
        {
          invoiceId = transactionId
        });

      }
      catch (ISBusinessException exception)
      {
        var billingMember = _cargoCorrespondenceManager.GetMember(correspondence.FromMemberId);
        var billedMember = _cargoCorrespondenceManager.GetMember(correspondence.ToMemberId);
        correspondence.FromMember = billingMember;
        correspondence.ToMember = billedMember;

        /*SCP159751: Create Correspondence for awb 784-41554763
            Desc: Code is added to display business exception error message on screen.
            Date: 02-Aug-2013*/
        ShowErrorMessage(exception.ErrorCode, true);

        return RedirectToAction("EditCargoCorrespondence", new
        {
          transactionId = correspondence.Id.Value()
        });
      }

      ViewData[ViewDataConstants.InvoiceId] = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(transactionId).Id;

      CheckCorrespondenceInitiator(transactionId);

      var rejectedMemoIds = TempData[TempDataConstants.RejectedRecordIds] as string[];
      if (rejectedMemoIds != null)
      {
        string sRejectedMemoIds = string.Join(",", rejectedMemoIds);
        GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action("InitiateCorrespondenceRejectionGridData",
                                                                                                  new
                                                                                                  {
                                                                                                    invoiceId = correspondence.InvoiceId,
                                                                                                    rejectedMemoIds = sRejectedMemoIds
                                                                                                  }));
        TempData[TempDataConstants.RejectedRecordIds] = rejectedMemoIds;
      }

      GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new { invoiceId = correspondence.Id }));

      return RedirectToAction("Correspondence", "Correspondence", new
      {
        invoiceId = transactionId
      });
    }

    [ISAuthorize(Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence.BillingHistoryAndCorrespondence.ViewCorrespondence)]
    public ActionResult ViewCorrespondence(string correspondenceId, string invoiceId)
    {
      /* SCP106534: ISWEB No-02350000768 
      Desc: Create corr is pushed to DB for better concurrency control. This prevents creation of orphan stage 1 corr in pax and cgo.
          * Added try-catch block to prevent screen crashing.
      Date: 20/06/2013*/
      try
      {
        SetViewDataPageMode(PageMode.View);
        if (string.IsNullOrEmpty(correspondenceId)) correspondenceId = invoiceId;
        var correspondence = _cargoCorrespondenceManager.GetCorrespondenceDetails(correspondenceId);
        //CMP 573: User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
        ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply] =
          _cargoCorrespondenceManager.IsCorrespondenceEligibleForReply((int)BillingCategoryType.Cgo, correspondence.Id,
                                                                        SessionUtil.UserId, SessionUtil.MemberId,
                                                                       Business.Security.Permissions.Cargo.
                                                                         BillingHistoryAndCorrespondence.
                                                                         BillingHistoryAndCorrespondence.
                                                                         DraftCorrespondence);

        ViewData[ViewDataConstants.InvoiceId] = _cargoCorrespondenceManager.GetCorrespondenceRelatedInvoice(correspondenceId).Id;

        GetCorrespondenceRejectGrid(ControlIdConstants.CorrespondenceRejectionsGridId,
                                    Url.Action(CorrespondenceRejectionGridAction,
                                               new
                                               {
                                                 invoiceId = correspondenceId
                                               }));
        GetCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId,
                                     Url.Action(CorrespondenceHistoryGridAction,
                                                new
                                                {
                                                  invoiceId = correspondenceId
                                                }));

        // If logged in member is the To member Id of the correspondence, 
        // then correspondence sub-status should be seen as 'Received'.
        if (correspondence.ToMemberId == SessionUtil.MemberId && correspondence.CorrespondenceSubStatus != CorrespondenceSubStatus.AcceptedByCorrespondenceInitiator) correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Received;

        return View("CargoCorrespondence", correspondence);
      }
      catch (Exception exception)
      {
        ShowErrorMessage("This Correspondence has already been updated by another user, please try again.", true);
        return RedirectToAction("Index", "BillingHistory", new { area = "Cargo", back = true });
      }

    }

    /// <summary>
    /// Fetch data for correspondence related rejections and display in grid.
    /// </summary>
    /// <param name="invoiceId">Correspondence ID</param>
    /// <returns></returns>
    public JsonResult CorrespondenceRejectionGridData(string invoiceId)
    {
      var correspondenceRejectionGrid = new CargoCorrespondenceRejectionsGrid(ControlIdConstants.CorrespondenceRejectionsGridId, Url.Action(CorrespondenceRejectionGridAction, new { invoiceId }));

      var correspondenceRejectionTotal = _cargoCorrespondenceManager.GetCorrespondenceRejectionList(invoiceId);

      if (correspondenceRejectionTotal == null) return null;

      var id = correspondenceRejectionTotal.Count() > 0 ? correspondenceRejectionTotal.ElementAt(0).InvoiceId.ToString() : null;
      var invoice = _cargoCorrespondenceManager.GetInvoiceDetail(id);

      return correspondenceRejectionGrid.DataBind(CopyRejectionMemoToLinkedRejections(correspondenceRejectionTotal.AsQueryable(), invoice));
    }

    private static IQueryable<LinkedRejection> CopyRejectionMemoToLinkedRejections(IQueryable<CargoRejectionMemo> correspondenceRejectionTotal, CargoInvoice invoice)
    {
      var billingMember = string.Format("{0}-{1}-{2}", invoice.BillingMember.MemberCodeAlpha, invoice.BillingMember.MemberCodeNumeric, invoice.BillingMember.CommercialName);
      var invoiceNumber = invoice.InvoiceNumber;
      var period = string.Format("{0} {1} P{2}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(invoice.BillingMonth), invoice.BillingYear, invoice.BillingPeriod);

      return correspondenceRejectionTotal.Select(rejectionMemo => new LinkedRejection
      {
        Id = rejectionMemo.Id.ToString(),
        InvoiceId = rejectionMemo.InvoiceId.ToString(),
        BillingMemberText = billingMember,
        DisplayBillingPeriod = period,
        InvoiceNumber = invoiceNumber,
        RejectionMemoNumber = rejectionMemo.RejectionMemoNumber,
        BillingCode = invoice.BillingCode
      }).ToList().AsQueryable();
    }

    /// <summary>
    /// Fetch data for Correspondence history code data and display in grid.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public JsonResult CorrespondenceHistoryGridData(string invoiceId)
    {
      var correspondenceHistoryGrid = new CargoCorrespondenceHistoryGrid(ControlIdConstants.CorrespondenceHistoryGridId, Url.Action(CorrespondenceHistoryGridAction, new
      {
        invoiceId
      }));

      var historyList = _cargoCorrespondenceManager.GetCorrespondenceHistoryList(invoiceId, SessionUtil.MemberId);

      IOrderedQueryable<CargoCorrespondence> correspondenceHistoryTotal = null;

      if (historyList != null && historyList.Count > 0)
        correspondenceHistoryTotal = historyList.AsQueryable().OrderBy(corr => corr.CorrespondenceStage);

      return correspondenceHistoryGrid.DataBind(correspondenceHistoryTotal);
    }

    public JsonResult InitiateCorrespondence(string rejectedRecordIds)
    {
      var rejectedRecordIdList = rejectedRecordIds.Split(',');

      if (rejectedRecordIdList.Select(rejMemoId => _cargoCorrespondenceManager.GetRejectedMemoDetails(rejMemoId))
        .Any(rejectionMemo => rejectionMemo == null || rejectionMemo.CorrespondenceId != null))
      {
        return Json(new UIMessageDetail
        {
          isRedirect = false
        });
      }

      TempData[TempDataConstants.RejectedRecordIds] = rejectedRecordIds;

      SessionUtil.RejectionMemoRecordIds = rejectedRecordIds;
      return Json(new UIMessageDetail
      {
        isRedirect = true
      });
    }

    [HttpPost]
    public JsonResult IsCorrespondenceOutSideTimeLimit(string invoiceId)
    {
        bool isTimeLimitRecordFound = true;
        if (!_cargoCorrespondenceManager.IsCorrespondenceOutsideTimeLimit(invoiceId, ref isTimeLimitRecordFound))
      {
        return
          Json(new UIMessageDetail { IsFailed = false });
      }

      if (isTimeLimitRecordFound)
      {
          return Json(new UIMessageDetail { IsFailed = true, Message = Messages.CorrespondenceOutSideTimeLimit });
      }
      else
      {
          /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
          Desc: system failed to retrieve Time Limit data from the master */
          return Json(new UIMessageDetail { IsFailed = true, Message = Messages.BGEN_10906 });
      }
    }

    /// <summary>
    /// CMP 527:CloseCorrespondence
    /// </summary>
    /// <param name="correspondenceId">correspondence Id</param>
    /// <param name="correspondenceStage">correspondence stage</param>
    /// <param name="correspondenceStatus">correspondence status</param>
    /// <param name="correspondenceSubStatus">correspondence sub status</param>
    /// <param name="scenarioId">close scenario</param>
    /// <param name="userAcceptanceComment">acceptance comments</param>
    /// <returns></returns>
    [ValidateAntiForgeryToken]
    [HttpPost]
    public ActionResult CloseCorrespondence(string correspondenceId, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioId, string userAcceptanceComment)
    {
      var returnMsg = string.Empty;
      var saved = false;
      var correspondence = _cargoCorrespondenceManager.GetFirstCorrespondenceDetails(correspondenceId);

      if (correspondence != null)
      {
        

        //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
        var nonSamplingInvoiceManager = Ioc.Resolve<ICargoInvoiceManager>(typeof(ICargoInvoiceManager));
        
        var billingMemos =
          nonSamplingInvoiceManager.GetBillingMemosForCorrespondence(correspondence.CorrespondenceNumber.Value,
                                                                     correspondence.FromMemberId);

        if (billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
        {
          ShowErrorMessage(string.Format(Messages.BCGO_10380,
                                          billingMemos.Transactions[0].InvoiceNumber,
                                          billingMemos.Transactions[0].InvoicePeriod,
                                          billingMemos.Transactions[0].BatchNumber,
                                          billingMemos.Transactions[0].SequenceNumber,
                                          billingMemos.Transactions[0].BillingMemoNumber), true);

          return RedirectToAction("Correspondence", "Correspondence", new
                                                                        {
                                                                          invoiceId = correspondenceId
                                                                        });
        }

        var correspondenceNumber = Convert.ToString(correspondence.CorrespondenceNumber.Value);
        var rejectionMemoDetails = _cargoCorrespondenceManager.GetCorrespondenceRejectionList(correspondenceId);
        var rejectionInvoice =
          _cargoCorrespondenceManager.GetInvoiceDetail(rejectionMemoDetails.First().InvoiceId.ToString());

        //var closedCorrespondence = _cargoCorrespondenceManager.GetOnlyCorrespondenceUsingLoadStrategy(correspondenceId);
        var closedCorrespondence = _cargoCorrespondenceManager.GetLastRespondedCorrespondene(correspondence.CorrespondenceNumber.Value);

        saved = _cargoCorrespondenceManager.CloseCorrespondence(correspondenceNumber, correspondenceStage,
                                                                correspondenceStatus, correspondenceSubStatus,
                                                                scenarioId, CargoBillingCategory, userAcceptanceComment,
                                                                SessionUtil.UserId, DateTime.UtcNow, ref returnMsg);
        if (saved)
        {
            //SCP329272 - SIS:Correspondence - Closure of Passenger Correspondence No. 790000302
            //Get Last Corr Detail for ToEmailId and ToAddtionalEmailId
            var l_correspondence = _cargoCorrespondenceManager.GetLastCorrespondenceDetails(correspondence.CorrespondenceNumber);

            if (l_correspondence == null && correspondence.CorrespondenceStage == 1)
            {
                l_correspondence = correspondence;
            }
          /* CMP#657: Retention of Additional Email Addresses in Correspondences
             FRS Section: 2.5 Email Alerts on Acceptance of Closure of Correspondences*/
          //SCP426039: FW: SIS:Correspondence - Closure of Passenger Correspondence No. 1760005673 - SIS Production
          _cargoCorrespondenceManager.SendCorrespondenceAlertOnClose(rejectionInvoice, correspondenceNumber,
                                                                     correspondence.FromMemberId,
                                                                     correspondence.ToMemberId, scenarioId,
                                                                     SessionUtil.UserId, l_correspondence.ToEmailId,
                                                                     closedCorrespondence == null ? string.Empty : closedCorrespondence.AdditionalEmailInitiator,
                                                                     closedCorrespondence == null ? string.Empty : closedCorrespondence.AdditionalEmailNonInitiator,
                                                                     string.Join(",",
                                                                                 rejectionMemoDetails.Select(
                                                                                   rej => rej.RejectionMemoNumber).
                                                                                   ToList()));
          ShowSuccessMessage(returnMsg);
        }
        else
        {
          ShowErrorMessage(returnMsg, true);
        }
      }

      if (scenarioId <= 3  || scenarioId == 5 || scenarioId == 6)
      {
        return RedirectToAction("Index", "BillingHistory", new { area = "Cargo" });

      }
      else
      {
        return RedirectToAction("Correspondence", "Correspondence", new
        {
          invoiceId = correspondenceId
        });
      }
    }
  }
}

