using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Reports.Common;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using log4net;
using log4net.Repository.Hierarchy;
using NVelocity;
using Iata.IS.Business.Reports.Pax;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Business.Pax.Impl
{
  public class PaxCorrespondenceManager : CorrespondenceManager, IPaxCorrespondenceManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Gets or sets the correspondence repository.
    /// </summary>
    /// <value>The country repository.</value>
    public IPaxCorrespondenceRepository PaxCorrespondenceRepository { get; set; }
    public IInvoiceRepository InvoiceRepository { get; set; }
    public IRejectionMemoRecordRepository RejectionMemoRecordRepository { get; set; }
    public IPaxCorrespondenceAttachmentRepository PaxCorrespondenceAttachmentRepository { get; set; }
    
    //SCP210204: IS-WEB Outage (Comment below code and same is refered from CorrespondenceManager)
    /// <summary>
    /// Gets or sets the reference manager.
    /// </summary>
    /// <value>The reference manager.</value>
    //public IReferenceManager ReferenceManager { get; set; }

    public ICalendarManager CalendarManager { get; set; }

    // SCP210204: IS-WEB Outage [To resolve null reference]
    public PaxCorrespondenceManager(IReferenceManager referenceManager) : base(referenceManager)
    {
    }

    public Correspondence AddCorrespondence(Correspondence paxCorrespondence)
    {
      
      if (ValidateCorrespondence(paxCorrespondence))
      {
        // SCP109163
        // Check if correspondence expiry date is crossed.
        if (paxCorrespondence.ExpiryDate < DateTime.UtcNow.Date)
        {
          throw new ISBusinessException(ErrorCodes.ExpiredCorrespondence);
        }
        // Mark the correspondence status as Open.
        paxCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;

        // Check if correspondence already present and in responded status.
        //var latestCorrespondence = GetRecentCorrespondenceDetails(paxCorrespondence.CorrespondenceNumber);

        //if (latestCorrespondence != null && latestCorrespondence.CorrespondenceStage == paxCorrespondence.CorrespondenceStage)
        //{
        //  throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadySent);
        //}

        // Validation for correspondence.
        PaxCorrespondenceRepository.Add(paxCorrespondence);
        //Added the following for look to increment the correspondence ref number if already present in database
        //TODO: need to change the count to 3 after UAT
        for (var tryCount = 0; tryCount < 25; tryCount++)
        {
          try
          {
            UnitOfWork.CommitDefault();
            tryCount = 25;
          }
          catch (Exception exception)
          {

            Logger.Error("Exception in Pax AddCorrespondence Method.");

            Logger.ErrorFormat(
              "Exception Message: {0}, Inner Exception Message: {1}, Stack Trace: {2},  corr Ref No: {3}, stage: {4}, from: {5}, to: {6}, Amount: {7}, Subject: {8}, curresncy Code: {9}, corr details: {10}, status: {11}, sub-status: {12}",
              exception.Message, exception.InnerException.Message, exception.StackTrace,
              paxCorrespondence.CorrespondenceNumber, paxCorrespondence.CorrespondenceStage,
              paxCorrespondence.FromMemberId, paxCorrespondence.ToMemberId, paxCorrespondence.AmountToBeSettled,
              paxCorrespondence.Subject, paxCorrespondence.CurrencyId, paxCorrespondence.CorrespondenceDetails,
              paxCorrespondence.CorrespondenceStatusId, paxCorrespondence.CorrespondenceSubStatusId);

            Logger.Error(exception);

            if (exception.InnerException != null)
              Logger.Error(exception.InnerException);

            if (tryCount == 24 || paxCorrespondence.CorrespondenceStage > 1)
            {
              throw new ISBusinessException(ErrorCodes.InvalidCorrespondencRefNo);
            }

            if (paxCorrespondence.CorrespondenceStage == 1)
            {
              paxCorrespondence.CorrespondenceNumber++;
            }

            //if (paxCorrespondence.CorrespondenceStage > 1)
            //{
            //paxCorrespondence.CorrespondenceNumber++;
            //}
            //else
            //{
            //throw new ISBusinessException(ErrorCodes.DuplicateCorrspondenceNumber);
            //}

          }
        }
        //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
        //// Update correspondence purging expiry date.
        //UpdatePurgingExpiryPeriod(paxCorrespondence);
      }
      
      return paxCorrespondence;
    }

    public Correspondence AddCorrespondenceAndUpdateRejection(Correspondence paxCorrespondence, List<Guid> correspondenceAttachmentIds, string rejectionMemoIds, ref int operationStatusIndicator)
    {
        if (ValidateCorrespondence(paxCorrespondence))
        {
            // Check if correspondence expiry date is crossed.
            if (paxCorrespondence.ExpiryDate < DateTime.UtcNow.Date)
            {
                throw new ISBusinessException(ErrorCodes.ExpiredCorrespondence);
            }
            // Mark the correspondence status as Open.
            paxCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;
            //SCP210204: IS-WEB Outage (Comment below code and implement in SP)
            //PaxCorrespondenceRepository.Add(paxCorrespondence);

            //Update parent Id for attachment
            var attachmentInDb = PaxCorrespondenceAttachmentRepository.Get(bmCouponAttachment => correspondenceAttachmentIds.Contains(bmCouponAttachment.Id));
            //SCP210204: IS-WEB Outage (Comment below code and implement in SP)
            //foreach (var recordAttachment in attachmentInDb)
            //{
            //    recordAttachment.ParentId = paxCorrespondence.Id;
            //    PaxCorrespondenceAttachmentRepository.Update(recordAttachment);
            //}

            /* SCP106534: ISWEB No-02350000768 
            Desc: Create corr is pushed to DB for better concurrency control. This prevents creation of orphan stage 1 corr in pax and cgo.
             * Attachments to be marked as a part of corr for updation in DB.
            Date: 20/06/2013*/
            if (attachmentInDb != null)
            {
                paxCorrespondence.Attachments = attachmentInDb.ToList();
            }

            //SCP106534: ISWEB No-02350000768 
            //Desc: As RM is expected for stage 1 corr. if it is not found throwing business exception.
            //Date: 20/06/2013
            if (string.IsNullOrEmpty(paxCorrespondence.RejectionMemoIds) && paxCorrespondence.CorrespondenceStage == 1)
            {
                throw new ISBusinessException(ErrorCodes.InvalidRejectionMemo);
            }

            //SCP210204: IS-WEB Outage (Comment below code and implement in SP)
            //// Update Correspondence Id in Rejection Memo only in case of first stage correspondence, 
            //if (paxCorrespondence.CorrespondenceStage == 1)
            //{
            //    char[] sep = { ',' };
            //    var sRejectedMemoIds = paxCorrespondence.RejectionMemoIds.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            //    int sourceCode = 0;

            //    foreach (var rejMemoId in sRejectedMemoIds)
            //    {
            //        var rejectionMemo = GetRejectedMemoDetails(rejMemoId);
            //        //CMP526 - Passenger Correspondence Identifiable by Source Code
            //        sourceCode = sourceCode == 0 ? rejectionMemo.SourceCodeId : sourceCode;
            //        if(sourceCode != rejectionMemo.SourceCodeId)
            //        {
            //            throw new ISBusinessException(
            //                "Please select rejection memos of 3rd stage belonging to the same Source Code");
            //        }
            //        // Update correspondence id for RM only if it is not assigned before.
            //        if (rejectionMemo != null && !rejectionMemo.CorrespondenceId.HasValue)
            //        {
            //            rejectionMemo.CorrespondenceId = paxCorrespondence.Id;
            //            RejectionMemoRecordRepository.Update(rejectionMemo);
            //        }
            //        // if correspondence id for RM already exists then communicate the same
            //        else if (rejectionMemo != null && rejectionMemo.CorrespondenceId.HasValue)
            //        {
            //            throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadyCreated);
            //        }
            //        // Problem getting RM and so provide such error.
            //        else if (rejectionMemo == null)
            //        {
            //            /* Business exception - Invalid RM */
            //            throw new ISBusinessException(ErrorCodes.InvalidRejectionMemo);
            //        }
            //    }
            //}
            
            operationStatusIndicator = PaxCorrespondenceRepository.CreateCorrespondence(ref paxCorrespondence);
            
            //operationStatusIndicator value interpretation - 
            //-1 => Internal DB Exception
            //0  => Success (this is when CORRESPONDENCE_ID_O will have a value)
            //1  => RM already has corr linked to it.
            //2  => Problem updating RM

            // Over email decided to show same error in both these cases (and any other un anticipated case)
            //(operationStatusIndicator == -1 || operationStatusIndicator == 2)
            //Email attached to SCP 106534
            //Date: 20/06/2013
            if (operationStatusIndicator == 1)
            {
                throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadyCreated);
            }
            else if (operationStatusIndicator == 3)
            {
                throw new ISBusinessException(ErrorCodes.CorrespondenceConcurrentUpdateError);
            }
            else if (operationStatusIndicator == -1 || operationStatusIndicator == 2)
            {
                throw new ISBusinessException(ErrorCodes.InternalDBErrorInCorrespondenceCreation);
            }
            //SCP210204: IS-WEB Outage (Error of CMP526)
            else if (operationStatusIndicator == 4)
            {
              throw new ISBusinessException("Please select rejection memos of 3rd stage belonging to the same Source Code");
            }
            //Added below code just for better readability of code
            //else
            //{
            //    // 0 => Success.
            //}
        }
        //Added below code just for better readability of code
        //else
        //{
        //      //This else IS for ValidateCorrespondence falure. ValidateCorrespondence() itself throws business exceptions.
        //}
        
        return paxCorrespondence;
    }

    public Correspondence UpdateCorrespondence(Correspondence paxCorrespondence)
    {

      if (ValidateCorrespondence(paxCorrespondence))
      {
        //Replaced with Load Strategy implementation
        //var correspondenceHeaderfromdb = PaxCorrespondenceRepository.Single(correspondenceId: paxCorrespondence.Id);
        //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
        var correspondenceHeaderfromdb = PaxCorrespondenceRepository.GetCorrespondenceWithAttachment(corr => corr.Id == paxCorrespondence.Id).FirstOrDefault();
        //var correspondenceHeaderfromdb = PaxCorrespondenceRepository.Single(correspondence => correspondence.Id == paxCorrespondence.Id);

        var updatedCorrespondenceData = PaxCorrespondenceRepository.Update(paxCorrespondence);
        
        // Changes to update attachment breakdown records.
        var listToDeleteAttachment = correspondenceHeaderfromdb.Attachments.Where(attachment => paxCorrespondence.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();



        var attachmentIdList = (from attachment in paxCorrespondence.Attachments
                                where correspondenceHeaderfromdb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                                select attachment.Id).ToList();






        var attachmentInDb = PaxCorrespondenceAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));

        foreach (var recordAttachment in attachmentInDb)
        {
          if (IsDuplicateCorrespondenceAttachmentFileName(recordAttachment.OriginalFileName, paxCorrespondence.Id))
          {
            throw new ISBusinessException(ErrorCodes.DuplicateFileName);
          }

          recordAttachment.ParentId = paxCorrespondence.Id;
          PaxCorrespondenceAttachmentRepository.Update(recordAttachment);
        }

        foreach (var rmCouponRecordAttachment in listToDeleteAttachment)
        {
          PaxCorrespondenceAttachmentRepository.Delete(rmCouponRecordAttachment);
        }

        UnitOfWork.CommitDefault();
        //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
        //// Update correspondence purging expiry date.
        //UpdatePurgingExpiryPeriod(paxCorrespondence);
        
        return updatedCorrespondenceData;
      }

      return paxCorrespondence;
    }
    //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //private void UpdatePurgingExpiryPeriod(Correspondence paxCorrespondence)
    //{
    //  if (paxCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open)
    //  {
    //    //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
    //    //  var invoiceBase = paxCorrespondence.Invoice ?? InvoiceRepository.Single(id: paxCorrespondence.InvoiceId);
    //    var invoiceBase = paxCorrespondence.Invoice ?? InvoiceRepository.Get(i => i.Id == paxCorrespondence.InvoiceId).SingleOrDefault();
    //    var transactionType = paxCorrespondence.AuthorityToBill ? TransactionType.PasNsBillingMemoDueToAuthorityToBill : TransactionType.PaxOtherCorrespondence;

    //    DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(transactionType, invoiceBase, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null, true, paxCorrespondence);

    //    InvoiceRepository.UpdateExpiryDatePeriod(paxCorrespondence.Id, (int)TransactionType.PaxOtherCorrespondence, expiryPeriod);
    //  }
    //}

    /// <summary>
    /// Check for duplicate file name of correspondence attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="miscCorrespondenceId">correspondence Id</param>
    /// <returns></returns>
    public bool IsDuplicateCorrespondenceAttachmentFileName(string fileName, Guid miscCorrespondenceId)
    {
      return PaxCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == miscCorrespondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }


    public void UpdateRejectedMemo(RejectionMemo rejectedMemo)
    {
      RejectionMemoRecordRepository.Update(rejectedMemo);
      UnitOfWork.CommitDefault();
    }

    public IList<CorrespondenceAttachment> UpdatePaxCorrespondenceAttachment(List<Guid> correspondenceAttachmentIds, Guid correspondenceId)
    {
      
      var attachmentInDb = PaxCorrespondenceAttachmentRepository.Get(bmCouponAttachment => correspondenceAttachmentIds.Contains(bmCouponAttachment.Id));

      foreach (var recordAttachment in attachmentInDb)
      {
        recordAttachment.ParentId = correspondenceId;
        PaxCorrespondenceAttachmentRepository.Update(recordAttachment);
      }

      UnitOfWork.CommitDefault();

      return attachmentInDb.ToList();
    }

    public bool DeleteCorrespondence(string correspondenceId)
    {
      throw new NotImplementedException();
    }

    public Correspondence GetCorrespondenceDetails(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      //Replaced with LoadStrategy call
      var correspondenceHeader = PaxCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
      //var correspondenceHeader = PaxCorrespondenceRepository.Single(correspondence => correspondence.Id == correspondenceGuid);
      
      return correspondenceHeader;
    }

    public Correspondence GetCorrespondenceDetailsForSaveAndSend(string correspondenceId)
    {
      
      var correspondenceGuid = correspondenceId.ToGuid();
      var correspondenceHeader = PaxCorrespondenceRepository.Get(i => i.Id == correspondenceGuid).SingleOrDefault();
      
      return correspondenceHeader;
    }
    public Correspondence GetCorrespondenceDetails(long? correspondenceNumber)
    {
      throw new NotImplementedException();
    }

    public bool IsTransactionExists(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      var isTransactionExists = (PaxCorrespondenceRepository.GetCount(correspondence => correspondence.Id == correspondenceGuid) > 0);

      return isTransactionExists;
    }


    public bool IsDuplicatePaxCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId)
    {
      return PaxCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == correspondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="paxCorrespondence"></param>
    /// <returns></returns>
    public bool ValidateCorrespondence(Correspondence paxCorrespondence)
    {
      
      // Validation for emailid
      var toEmailId = paxCorrespondence.ToEmailId != null ? paxCorrespondence.ToEmailId.Trim() : paxCorrespondence.ToEmailId;

      /* CMP#657: Retention of Additional Email Addresses in Correspondences
       * Adding code to get email ids from initiator and non-initiator and removing
       * additional email field*/
      var additionalEmailInitiator = paxCorrespondence.AdditionalEmailInitiator != null
                                           ? paxCorrespondence.AdditionalEmailInitiator.Trim()
                                           : paxCorrespondence.AdditionalEmailInitiator;
      var additionalEmailNonInitiator = paxCorrespondence.AdditionalEmailNonInitiator != null
                                            ? paxCorrespondence.AdditionalEmailNonInitiator.Trim()
                                            : paxCorrespondence.AdditionalEmailNonInitiator;
      if (string.IsNullOrEmpty(toEmailId + additionalEmailInitiator + additionalEmailNonInitiator))
      {
        throw new ISBusinessException(ErrorCodes.EnterEmailIds);
      }

      // Validation of Correspondence Subject
      if (string.IsNullOrEmpty(paxCorrespondence.Subject))
      {
        throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceSubject);
      }
      //SCP210204: IS-WEB Outage (QA Issue Fix)
      if (paxCorrespondence.AmountToBeSettled < 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceAmountToBeSettled);
      }

      var invoice = GetInvoiceDetail(paxCorrespondence.InvoiceId.ToString());
      TransactionType transactionType = TransactionType.PaxCorrespondence;
      if (invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        transactionType = TransactionType.PaxCorrespondenceSampling;
      }
      if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(paxCorrespondence.AmountToBeSettled), paxCorrespondence.CorrespondenceStage == 1 ? transactionType : TransactionType.PaxOtherCorrespondence, paxCorrespondence.CurrencyId, invoice, applicableMinimumField: ApplicableMinimumField.AmountToBeSettled, isCorrespondence: true, correspondence: paxCorrespondence, validateMaxAmount: false))
      {
        throw new ISBusinessException(ErrorCodes.CorrespondenceAmountIsNotInAllowedRange);
      }
      // Validation for CorrespondenceNumber
      if (string.IsNullOrEmpty(paxCorrespondence.CorrespondenceNumber.ToString()))
      {
        throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceNumber);
      }

      // Validation for CurrencyId
      if (!paxCorrespondence.CurrencyId.HasValue || paxCorrespondence.CurrencyId == 0)
      {
        throw new ISBusinessException(ErrorCodes.InvalidCurrencyCode);
      }

      //You cannot send a correspondence if it is expired
      if ((paxCorrespondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded))
      {
        // var invoice = GetInvoiceDetail(paxCorrespondence.InvoiceId.ToString());
        //if(IsCorrespondenceOutsideTimeLimit(paxCorrespondence, invoice))
        //{
        //  throw new ISBusinessException(MiscUatpErrorCodes.ExpiredCorrespondence);
        //}
      }

      if (!HasValidAuthorityToBill(paxCorrespondence))
      {
        throw new ISBusinessException(ErrorCodes.InvalidAuthorityToBill);
      }

      /* CMP#657: Retention of Additional Email Addresses in Correspondences
       * Get distinct email id list*/
        string toEmailIds = GetEmailIdsList(paxCorrespondence.ToEmailId,
                                            paxCorrespondence.AdditionalEmailInitiator,
                                            paxCorrespondence.AdditionalEmailNonInitiator);

      if (ValidateToEmailIds(toEmailIds) == false)
      {
        throw new ISBusinessException(ErrorCodes.InvalidEmailIds);
      }


      //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
      var nonSamplingInvoiceManager = Ioc.Resolve<INonSamplingInvoiceManager>(typeof(INonSamplingInvoiceManager));
      var billingMemos =
          nonSamplingInvoiceManager.GetBillingMemosForCorrespondence(paxCorrespondence.CorrespondenceNumber.Value,
                                                                     paxCorrespondence.FromMemberId);
      if (billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
      {
        throw new ISBusinessException(string.Format(Messages.BPAXNS_10940,
                                        billingMemos.Transactions[0].InvoiceNumber,
                                        billingMemos.Transactions[0].InvoicePeriod,
                                        billingMemos.Transactions[0].BatchNumber,
                                        billingMemos.Transactions[0].SequenceNumber,
                                        billingMemos.Transactions[0].BillingMemoNumber));
      }

      // Update the expiry date of correspondence.
      //if (paxCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Saved && paxCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.ReadyForSubmit)
      //{
      UpdateExpiryDate(paxCorrespondence, invoice);
      //}


      return true;

    }


    private bool IsCorrespondenceOutsideTimeLimit(Correspondence paxCorrespondence, PaxInvoice invoice, ref bool isTimeLimitRecordFound)
    {
      //CMP#624 : 2.10 - Change#6 : Time Limits
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      var isInsideTimeLimit = true;
      //SMI settlementMethod = ReferenceManager.GetBilateralSmi(invoice.SettlementMethodId);
      SMI settlementMethod = (SMI) invoice.SettlementMethodId;
      if (ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
      {
        settlementMethod = SMI.Bilateral;
      }
      /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
        Desc: Prior to this code fix, current system date (correspondence initiation date) was being used. This mistake is now corrected. 
        Code is now updated to use Previous transaction (Rej Stage 3 Invoice) billing Period as input for time limit determination. */
      var previousTransactionDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
      switch (settlementMethod)
      {
        case SMI.Ich:
        case SMI.AchUsingIataRules:
        case SMI.IchSpecialAgreement:
        case SMI.Bilateral:
            isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodB(TransactionType.PaxCorrespondence, (int)invoice.InvoiceSmi, previousTransactionDate, invoice, paxCorrespondence.CorrespondenceDate, ref isTimeLimitRecordFound);
          break;
        case SMI.Ach:
          isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodE(TransactionType.PaxCorrespondence, (int)invoice.InvoiceSmi, invoice, ref isTimeLimitRecordFound);
          break;
        default:
          isInsideTimeLimit = false;
          break;
      }

      return !isInsideTimeLimit;
    }

    /// <summary>
    /// Determines whether [is correspondence outside time limit] [the specified invoice id].
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <returns>
    /// true if [is correspondence outside time limit] [the specified invoice id]; otherwise, false.
    /// </returns>
    public bool IsCorrespondenceOutsideTimeLimit(string invoiceId, ref bool isTimeLimitRecordFound)
    {
      var isInsideTimeLimit = true;
      var invoice = GetInvoiceDetail(invoiceId);
      TransactionType transactionType = 0;
      if (invoice.BillingCode == (int)BillingCode.SamplingFormF || invoice.BillingCode == (int)BillingCode.SamplingFormXF)
      {
        transactionType = TransactionType.PaxCorrespondenceSampling;
      }
      else
      {
        transactionType = TransactionType.PaxCorrespondence;
      }
      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      //SMI settlementMethod = ReferenceManager.GetBilateralSmi(invoice.SettlementMethodId);
      SMI settlementMethod = (SMI)invoice.SettlementMethodId;
      if (ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
      {
        settlementMethod = SMI.Bilateral;
      }
      switch (settlementMethod)
      {
        case SMI.Ich:
        case SMI.AchUsingIataRules:
        case SMI.Bilateral:
        case SMI.IchSpecialAgreement:
            /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
            Desc: Prior to this code fix, current system date (correspondence initiation date) was being used. This mistake is now corrected. 
            Code is now updated to use Previous transaction (Rej Stage 3 Invoice) billing Period as input for time limit determination. */
            var previousTransactionDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
            isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodB(transactionType, (int)invoice.InvoiceSmi, previousTransactionDate, invoice, DateTime.UtcNow, ref isTimeLimitRecordFound);
            break;
        case SMI.Ach:
            isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodE(transactionType, (int)invoice.InvoiceSmi, invoice, ref isTimeLimitRecordFound);
          break;
        default:
          isInsideTimeLimit = false;
          break;
      }
      
      return !isInsideTimeLimit;
    }

    /// <summary>
    /// Add Correspondence Attachment record
    /// </summary>
    /// <param name="attach">Correspondence Attachment record</param>
    /// <returns></returns>
    public CorrespondenceAttachment AddCorrespondenceAttachment(CorrespondenceAttachment attach)
    {
      PaxCorrespondenceAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      return attach;
    }

    private bool HasValidAuthorityToBill(Correspondence paxCorrespondence)
    {
      if (paxCorrespondence.AuthorityToBill)
      {
        var correspondence = GetOriginalCorrespondenceDetails(paxCorrespondence.CorrespondenceNumber);
        if (correspondence != null)
        {
          if (correspondence.FromMemberId == paxCorrespondence.FromMemberId)
          {
            return false;
          }
        }
      }

      return true;
    }





    public Correspondence GetOriginalCorrespondenceDetails(long? correspondenceNumber)
    {
      var corrrespondence = PaxCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderBy(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      
      return corrrespondence;
    }

    /// <summary>
    /// This function is used to get last correspondence detail.
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <param name="getLastCorr"></param>
    /// <returns></returns>
    /// SCP199693: create BM and close correspondence at same time
    public Correspondence GetRecentCorrespondenceDetails(long? correspondenceNumber, bool getLastCorr = false)
    {
      return getLastCorr
               ? PaxCorrespondenceRepository.Get(
                 correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderByDescending(
                   correspondence => correspondence.CorrespondenceStage).FirstOrDefault()
               : PaxCorrespondenceRepository.Get(
                 correspondence =>
                 correspondence.CorrespondenceNumber == correspondenceNumber &&
                 (correspondence.CorrespondenceSubStatusId == (int) CorrespondenceSubStatus.Responded ||
                  correspondence.CorrespondenceSubStatusId == (int) CorrespondenceSubStatus.DueToExpiry)).
                   OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
    }

    public IList<Correspondence> GetCorrespondenceHistoryList(string invoiceId)
    {
      throw new NotImplementedException();
    }

    public IList<RejectionMemo> GetCorrespondenceRejectionList(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      if (correspondenceGuid == Guid.Empty) return null;

      //Replaced with LoadStrategy call
      //var correspondence = PaxCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      var correspondence = PaxCorrespondenceRepository.Get(i => i.Id == correspondenceGuid).SingleOrDefault();
      //var correspondence = PaxCorrespondenceRepository.Single(corr => corr.Id == correspondenceGuid);

      correspondence = correspondence.CorrespondenceStage == 1 ? correspondence : GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);

      var rejectionMemoList = RejectionMemoRecordRepository.Get(memo => memo.CorrespondenceId == correspondence.Id);
      
      return rejectionMemoList.ToList();
    }

    public PaxInvoice GetCorrespondenceRelatedInvoice(string correspondenceId, Correspondence correspondence=null)
    {
      //SCP210204: IS-WEB Outage (added log)
        /* SCP106534: ISWEB No-02350000768 
        Desc: Create corr is pushed to DB for better concurrency control. This prevents creation of orphan stage 1 corr in pax and cgo.
         * Added try-catch block to prevent screen crashing.
        Date: 20/06/2013*/
      
        try
        {
          //SCP210204: IS-WEB Outage (Get correspondence from DB only if correspondence object pass to this method is null)
          PaxInvoice paxinvoice = null;
          if (correspondence == null)
          {
            var correspondenceGuid = correspondenceId.ToGuid();

            //Replaced with LoadStrategy call
            //var correspondence = PaxCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
            correspondence = PaxCorrespondenceRepository.Get(i => i.Id == correspondenceGuid).SingleOrDefault();
          }

          //Replaced with LoadStrategy call
            var firstCorrespondence = correspondence.CorrespondenceStage == 1
                                        ? correspondence
                                        : PaxCorrespondenceRepository.Single(correspondenceNumber: correspondence.CorrespondenceNumber, correspondenceStage: 1);
            //SCP210204: IS-WEB Outage (get invoice from RM only if first corr dose not having invoiceid)
            if (firstCorrespondence != null && (firstCorrespondence.InvoiceId == new Guid()))
            {
              //Replaced with LoadStrategy call
              var rejectionMemo = RejectionMemoRecordRepository.First(memo => memo.CorrespondenceId == firstCorrespondence.Id);

              //return rejectionMemo != null ? InvoiceRepository.Single(id: rejectionMemo.InvoiceId) : null;
              paxinvoice = rejectionMemo != null ? InvoiceRepository.Get(i => i.Id == rejectionMemo.InvoiceId).SingleOrDefault() : null;
            }
            else if (firstCorrespondence != null)
            {
              paxinvoice =  InvoiceRepository.Get(i => i.Id == firstCorrespondence.InvoiceId).SingleOrDefault();
            }
          

          return paxinvoice;

        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public PaxInvoice GetInvoiceDetail(string invoiceId)
    {
      
      Guid paxInvoiceId;
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      //return Guid.TryParse(invoiceId,out paxInvoiceId) ? InvoiceRepository.Single(id: paxInvoiceId) : null;
      var invoiceHeader = Guid.TryParse(invoiceId, out paxInvoiceId) ? InvoiceRepository.Get(i => i.Id == paxInvoiceId).SingleOrDefault() : null;
      
      return invoiceHeader;
    }

    public Member GetMember(int memberId)
    {
      return MemberManager.GetMemberDetails(memberId);
    }

    public long GetCorrespondenceNumber(int memberId)
    {
      
      var correspondence = PaxCorrespondenceRepository.Get(c => c.FromMemberId == memberId && c.CorrespondenceStage == 1).OrderByDescending(c => c.CorrespondenceNumber).FirstOrDefault();
      long correspondNumber = 0;

      if (correspondence != null && correspondence.CorrespondenceNumber != null)
        correspondNumber = correspondence.CorrespondenceNumber.Value;

      return correspondNumber;
    }
   
        /// <summary>
        /// Retrieves only invoice details from stored procedure
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceDetailFromSp(string invoiceId)
        {
          //SCP210204: IS-WEB Outage (added log)
          //pass includeBillingBilled=true to in GetInvoiceHeader method;
          
            Guid paxInvoiceId;
            var invoice = Guid.TryParse(invoiceId, out paxInvoiceId)
                       ? InvoiceRepository.GetInvoiceHeader(paxInvoiceId,true)
                       : null;
            
          return invoice;
        }

    public long GetInitialCorrespondenceNumber(int memberId)
    {
      var correspondenceNumber = PaxCorrespondenceRepository.GetInitialCorrespondenceNumber(memberId);
      
      return correspondenceNumber;

    }

    // CMP#657: Retention of Additional Email Addressed in Correspondences.
    //public bool SendCorrespondenceMail(string correspondPageUrl, string toEmailIds, string subject)
    //{
    //  var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
    //  var context = new VelocityContext();

    //  context.Put("CorrespondenceUrl", correspondPageUrl);
    //  context.Put("SisOpsEmailId", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
    //  var messageBody = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.CorrespondenceResponse, context);

    //  var emailSender = Ioc.Resolve<IEmailSender>();


    //  string[] eMailIds = toEmailIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
    //  var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
    //  var emailSettingForCorrespondence = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.CorrespondenceResponse);
    //  foreach (var eMailId in eMailIds)
    //  {
    //    //Check added to fix issue for Mozilla 4,5
    //    if (!string.IsNullOrEmpty(eMailId.Trim()))
    //    {
    //      var mailMessage = new MailMessage(emailSettingForCorrespondence.SingleOrDefault().FromEmailAddress, eMailId.Trim(), subject, messageBody) { IsBodyHtml = true };
    //      try
    //      {
    //        emailSender.Send(mailMessage);
    //      }
    //      catch (Exception)
    //      {
    //        throw new ISBusinessException(GetErrorDescription(ErrorCodes.FailedToSendMail, eMailId.Trim()));
    //      }
    //    }

    //  }


    //  return true;
    //}

    //private static string GetErrorDescription(string errorCode, string mailId)
    //{
    //  var errorDescription = Messages.ResourceManager.GetString(errorCode);

    //  // Replace place holders in error message with appropriate record names.
    //  if (!string.IsNullOrEmpty(errorDescription))
    //  {
    //    errorDescription = string.Format(errorDescription, mailId);
    //  }

    //  return errorDescription;
    //}
    
    public string GetChargeCodes(string invoiceId)
    {
      throw new NotImplementedException();
    }

    public Correspondence GetRecentCorrespondenceDetails(string invoiceId)
    {
      //var invoiceGuid = invoiceId.ToGuid();
      //var corrrespondence = PaxCorrespondenceRepository.Get(correspondence => correspondence.InvoiceId == invoiceGuid
      //  && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
      //  && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
      //  && correspondence.CorrespondenceSubStatusId == 2)))
      //  )).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return new Correspondence();
    }

    public Correspondence GetRecentCorrespondenceDetails(string correspondenceId, int billingMemberId)
    {
      throw new NotImplementedException();
    }

    public string GetToEmailIds(int memberId, ProcessingContactType processingContact)
    {
      var toMailIds = new StringBuilder();
      var index = 0;
      List<Contact> contactTypeList = MemberManager.GetContactsForContactType(memberId, processingContact);
      if (contactTypeList != null)
      {
        foreach (var contact in contactTypeList)
        {
          index += 1;
          toMailIds.Append(index != contactTypeList.Count ? string.Format("{0}{1}", contact.EmailAddress, ";") : contact.EmailAddress);
        }
      }
      
      return toMailIds.ToString();
    }

    public Contact GetContactDetails(int memberId, ProcessingContactType processingContact)
    {
      var contactList = MemberManager.GetContactsForContactType(memberId, processingContact);
      var contact = new Contact();

      if (contactList != null && contactList.Count > 0)
      {
        contact = contactList.First();
      }

      return contact;
    }



    public int GetNumericMemberCode(string memberCode)
    {
      var index = 0;
      int value;

      if (Validators.IsWholeNumber(memberCode))
      {
        return Convert.ToInt32(memberCode);
      }

      var memberCodeAsciiChars = new byte[memberCode.Length];
      Encoding.ASCII.GetBytes(memberCode.ToUpper(), 0, memberCode.Length, memberCodeAsciiChars, 0);
      foreach (var memberCodeAsciiValue in memberCodeAsciiChars)
      {
        if (memberCodeAsciiValue <= 90 && memberCodeAsciiValue >= 65)
        {
          //To get A = 10, B=11
          value = memberCodeAsciiValue - 55;
          string toReplace = memberCode.Substring(index, 1);
          memberCode = memberCode.Replace(toReplace, value.ToString());
        }
        index++;
      }

      int numericMemberCode;
      int returnValue;
      if (Int32.TryParse(memberCode, out numericMemberCode))
      {
        returnValue = numericMemberCode > 9999 ? 0 : numericMemberCode;
      }
      else
      {
        returnValue = 0;
      }

      return returnValue;
    }

    public bool ValidateToEmailIds(string toEmailIds)
    {
      if (toEmailIds == null)
      {
        return false;
      }
      toEmailIds = toEmailIds.Replace("\r", string.Empty).Replace("\n", string.Empty);
      string[] eMailIds = toEmailIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      if (eMailIds.Any(eMailId => IsValidEmailId(eMailId.Trim()) == false))
      {
        return false;
      }

      return true;
    }

    public static bool IsValidEmailId(string inputEmailId)
    {
        if (inputEmailId != null)
        {
            //SCP207710 - Change Super User(Allow valid special character).
            var re = new Regex(Constants.ValidEmailPattern);
            if (re.IsMatch(inputEmailId.ToLower())) return (true);
        }
      return false;
    }

    public IList<Correspondence> GetCorrespondenceHistoryList(string correspondenceId, int billingMemberId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      //Replaced with LoadStrategy call
      var correspondenceInDb = PaxCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
      //var correspondenceInDb = PaxCorrespondenceRepository.Single(correspondence => correspondence.Id == correspondenceGuid);
      if (correspondenceInDb == null) return null;
      var correspondenceHistoryList = PaxCorrespondenceRepository.GetCorrespondenceWithAttachment(correspondence => correspondence.CorrespondenceNumber == correspondenceInDb.CorrespondenceNumber
        && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
        && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
        && correspondence.CorrespondenceSubStatusId == 2)))
        )).ToList();
      
      return correspondenceHistoryList.Count > 0 ? correspondenceHistoryList.OrderBy(corr => corr.CorrespondenceStage).ToList() : correspondenceHistoryList;

    }




    public bool IsFirstCorrespondence(int memberId)
    {
      var correspondence = PaxCorrespondenceRepository.Get(c => c.FromMemberId == memberId && c.CorrespondenceStage == 1).FirstOrDefault();

      if (correspondence == null)
      {
        return true;
      }

      return false;
    }

    public List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold)
    {
      return PaxCorrespondenceRepository.UpdateCorrespondenceStatus(billingPeriod, _oornThreshold, _oernThreshold, _eornThreshold, _eoryThreshold, _eorybThreshold);
    }

    public string CreateCorrespondenceFormatPdf(string correspondenceId)
    {

      var fromContact = new CorrespondenceReportContact();
      var toContact = new CorrespondenceReportContact();

      CorrespondenceReportContact reportModule = new CorrespondenceReportContact();
      CorrespondenceReportContact treportModule = new CorrespondenceReportContact();

      var correspondenceDetails = GetCorrespondenceDetails(correspondenceId);

      string correspondenceFormatReportPath = FileIo.GetForlderPath(SFRFolderPath.CorrespondenceFormatReportPath) + correspondenceId.ToGuid() + ".pdf";
      
      var fromContacts = GetContactDetails(correspondenceDetails.FromMemberId, ProcessingContactType.PaxCorrespondence);

      reportModule.AddressLine1 = fromContacts.AddressLine1;
      reportModule.AddressLine2 = fromContacts.AddressLine2;
      reportModule.AddressLine3 = fromContacts.AddressLine3;
      reportModule.CityName = fromContacts.CityName;
      reportModule.Country = fromContacts.Country;
      reportModule.CountryId = fromContacts.CountryId;
      reportModule.EmailAddress = fromContacts.EmailAddress;
      reportModule.FirstName = fromContacts.FirstName;
      reportModule.LastName = fromContacts.LastName;
      reportModule.Location = fromContacts.Location;
      reportModule.LocationId = fromContacts.LocationId;
      reportModule.PositionOrTitle = fromContacts.PositionOrTitle;
      reportModule.PostalCode = fromContacts.PostalCode;
      reportModule.Salutation = fromContacts.Salutation;
      reportModule.SalutationId = fromContacts.SalutationId;

      fromContact = reportModule;


      var toContacts = GetContactDetails(correspondenceDetails.ToMemberId, ProcessingContactType.PaxCorrespondence);

      treportModule.AddressLine1 = toContacts.AddressLine1;
      treportModule.AddressLine2 = toContacts.AddressLine2;
      treportModule.AddressLine3 = toContacts.AddressLine3;
      treportModule.CityName = toContacts.CityName;
      treportModule.Country = toContacts.Country;
      treportModule.CountryId = toContacts.CountryId;
      treportModule.Department = toContacts.Department;
      treportModule.Division = toContacts.Division;
      treportModule.EmailAddress = toContacts.EmailAddress;
      treportModule.FirstName = toContacts.FirstName;
      treportModule.LastName = toContacts.LastName;
      treportModule.Location = toContacts.Location;
      treportModule.LocationId = toContacts.LocationId;
      treportModule.PositionOrTitle = toContacts.PositionOrTitle;
      treportModule.PostalCode = toContacts.PostalCode;
      treportModule.Salutation = toContacts.Salutation;
      treportModule.SalutationId = toContacts.SalutationId;


      toContact = treportModule;

      Location memberLocation;
      Country country;

      //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
      if (fromContact.LocationId > 0)
      {
        memberLocation = MemberManager.GetMemberLocationDetails(fromContact.LocationId);
        fromContact.AddressLine1 = memberLocation.AddressLine1;
        fromContact.AddressLine2 = memberLocation.AddressLine2;
        fromContact.AddressLine3 = memberLocation.AddressLine3;
        fromContact.Country = memberLocation.Country;
        fromContact.PostalCode = memberLocation.PostalCode;
        fromContact.CityName = memberLocation.CityName;
      }
      else
      {
        memberLocation = MemberManager.GetMemberDefaultLocation(correspondenceDetails.FromMemberId, "MAIN");
        fromContact.AddressLine1 = memberLocation.AddressLine1;
        fromContact.AddressLine2 = memberLocation.AddressLine2;
        fromContact.AddressLine3 = memberLocation.AddressLine3;
        fromContact.Country = memberLocation.Country;
        fromContact.PostalCode = memberLocation.PostalCode;
        fromContact.CityName = memberLocation.CityName;
      }

      
      //If Country is not populated, populate it explicitly for given id
      if (fromContact.Country == null && !string.IsNullOrEmpty(fromContact.CountryId))
      {
        var countryList = ReferenceManager.GetCountryList();
        country = (from c in countryList
                   where c.Id == fromContact.CountryId
                   select c).First();
        fromContact.Country = country;
      }

     
      //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
      if (toContact.LocationId > 0)
      {
        memberLocation = MemberManager.GetMemberLocationDetails(toContact.LocationId);
        toContact.AddressLine1 = memberLocation.AddressLine1;
        toContact.AddressLine2 = memberLocation.AddressLine2;
        toContact.AddressLine3 = memberLocation.AddressLine3;
        toContact.Country = memberLocation.Country;
        toContact.PostalCode = memberLocation.PostalCode;
        toContact.CityName = memberLocation.CityName;
      }
      else
      {
        memberLocation = MemberManager.GetMemberDefaultLocation(correspondenceDetails.ToMemberId, "MAIN");
        toContact.AddressLine1 = memberLocation.AddressLine1;
        toContact.AddressLine2 = memberLocation.AddressLine2;
        toContact.AddressLine3 = memberLocation.AddressLine3;
        toContact.Country = memberLocation.Country;
        toContact.PostalCode = memberLocation.PostalCode;
        toContact.CityName = memberLocation.CityName;
      }

      //If Country is not populated, populate it explicitly for given id
      if (toContact.Country == null && !string.IsNullOrEmpty(toContact.CountryId))
      {
        var countryList = ReferenceManager.GetCountryList();
        country = (from c in countryList
                   where c.Id == toContact.CountryId
                   select c).First();
        toContact.Country = country;
      }

      var paxReportManager = Ioc.Resolve<IPaxReportManager>();

      paxReportManager.BuildCorrespondenceFormatReport(correspondenceDetails, ref correspondenceFormatReportPath, fromContact, toContact);

      return correspondenceFormatReportPath;
    }

    public Correspondence GetFirstCorrespondenceDetails(string correspondenceId, bool withoutLoadStrategy = false)
    {
        var correspondenceGuid = correspondenceId.ToGuid();

        if (correspondenceGuid == Guid.Empty) return null;

        var correspondence = PaxCorrespondenceRepository.GetFirstCorrespondence(correspondenceGuid);

        return correspondence;
    }

    /// <summary>
    /// To Get last Corr Details for given corr No
    /// </summary>
    /// <param name="correspondenceRefNo"></param>
    /// <returns></returns>
    public Correspondence GetLastCorrespondenceDetails(long? correspondenceRefNo)
    {
      if (correspondenceRefNo != null)
      {
        var correspondence = PaxCorrespondenceRepository.GetAll().Where(corr => corr.CorrespondenceNumber == correspondenceRefNo).OrderByDescending(corr => corr.CorrespondenceStage).FirstOrDefault();
        return correspondence;
      }
      return null;  
    }

    public RejectionMemo GetRejectedMemoDetails(string rejectionMemoId)
    {
      var rejectedMemoId = rejectionMemoId.ToGuid();
      /* SCP# 106534 - ISWEB No-02350000768
      * Desc: Method GetRejectedMemoDetails() is modified to read from database and not from the cache. Reading from cache was in problem, 
      * since it does not reflect recent changes in RM record in focus.
      * Date: 20/06/2013
      */
      var rejectedMemo = RejectionMemoRecordRepository.Single(rejectedMemoId, null);
      return rejectedMemo;
    }

    public CorrespondenceAttachment GetCorrespondenceAttachmentDetail(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = PaxCorrespondenceAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Gets the Pax Correspondence attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<CorrespondenceAttachment> GetCorrespondenceAttachments(List<Guid> attachmentIds)
    {
      return PaxCorrespondenceAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Gets the pax correspondence attachments.
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns></returns>
    public List<CorrespondenceAttachment> GetPaxCorrespondenceAttachments(Guid correspondenceId)
    {
      return PaxCorrespondenceAttachmentRepository.Get(attachment => attachment.ParentId == correspondenceId).ToList();
    }

    /// <summary>
    /// Updates the expiry date.
    /// </summary>
    /// <param name="correspondence">The correspondence.</param>
    /// <param name="invoice">The invoice.</param>
    public void UpdateExpiryDate(Correspondence correspondence, PaxInvoice invoice)
    {
      DateTime expiryDate = DateTime.MinValue;

      //if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && correspondence.AuthorityToBill)
      //{
      //  expiryDate = !ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId)
      //                 ? ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.PasNsBillingMemoDueToAuthorityToBill, invoice.SettlementMethodId, correspondence.CorrespondenceDate,invoice)
      //                 : ReferenceManager.GetTimeLimitMethodD1ExpiryDate(TransactionType.PasNsBillingMemoDueToAuthorityToBill, Convert.ToInt32(SMI.Bilateral), correspondence.CorrespondenceDate,invoice);

      //  if (expiryDate != correspondence.CorrespondenceDate) correspondence.ExpiryDate = expiryDate;
      //}

      // If Corresp Status is Open and Sub Status is Saved/ Ready To Submit then set expiry date as expiry date of last transaction
      // i.e. 1. In case of N th Corresp set expiry equal to expiry of N-1 th Corresp
      //      2. In case of 1st Corresp set expiry equal to expiry of RM for which Corresp is created.
      if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit))
      {
        // N th Corresp Case
        if (correspondence.CorrespondenceStage > 1)
        {
          var correspondenceHistory = PaxCorrespondenceRepository.Get(c => c.CorrespondenceNumber == correspondence.CorrespondenceNumber && c.InvoiceId == correspondence.InvoiceId).ToList();
          if (correspondenceHistory != null && correspondenceHistory.Count > 0)
          {
            var prevCorrespondence =
              correspondenceHistory.Where(c => c.CorrespondenceStage == (correspondence.CorrespondenceStage - 1)).
                FirstOrDefault();

            if (prevCorrespondence != null)
            {
              expiryDate = prevCorrespondence.ExpiryDate;
            }

          }
        }
        // 1st Corresp Case 
        else
        {
          // Get Time Limit for raising 1st Correspondence.
          var firstCorrespTimeLimit =
            ReferenceManager.GetTransactionTimeLimitTransactionType(
              (invoice.BillingCode == (int)BillingCode.SamplingFormF ||
               invoice.BillingCode == (int)BillingCode.SamplingFormXF)
                ? TransactionType.PaxCorrespondenceSampling
                : TransactionType.PaxCorrespondence, invoice.SettlementMethodId, new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod));

          if (firstCorrespTimeLimit != null)
          {
            // Check if Settlement Method is ACH then calculate expiry date by applying Calculation Method E.
            // Method E:
            // ‘X’ number of months is added to the month of the final rejection. The last calendar date of this 
            // month (newly calculated month) will be the time limit for the 1st correspondence. The actual period of the last rejection has no significance.
            // e.g. PAX 3rd Rejection  Correspondence #1
            //      1. Billing period of 3rd Rejection: 2010-Jan-P2
            //      2. Value of time limit: 7
            //      3. Time limit for the 1st Correspondence is calculated as 31-Aug-2010
            if (invoice.SettlementMethodId == (int)SettlementMethodValues.Ach)
            {
              expiryDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1).AddMonths(firstCorrespTimeLimit.Limit).AddMonths(1).AddDays(-1);
            }
            // else calculate expiry date by applying Calculation Method B 
            // Method B:
            // Closure date of the 4th period of the billing month of the final rejection is determined (from the IS Calendar). 
            // The actual period of the last rejection has no significance. ‘X’ number of months is added to that date to determine the time limit for the 1st correspondence.
            // e.g. PAX 3rd Rejection  Correspondence #1
            //      1. Billing period of 3rd Rejection: 2010-Apr-P1
            //      2. Closure date of 2010-Apr-P4: 09-May-2010
            //      3. Value of time limit: 6
            //      4. Time limit for the 1st Correspondence is calculated as 09-Nov-2010
            else
            {
              try
              {
                // Get end date for 4th Period of invoice billing month year.
                var period = CalendarManager.GetBillingPeriod(ClearingHouse.Ich, invoice.BillingYear,
                                                                  invoice.BillingMonth, 4);
                if (period != null)
                {
                  // Calculate expiry date by adding months to the end date of period.
                  expiryDate = period.EndDate.AddMonths(firstCorrespTimeLimit.Limit);
                }// End if
              }// End try
              catch (Exception ex)
              {
                Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
              }// End catch
            }// End else  
          }// End if
        }// End if
      }// End if

      // Get expiry date for N th Correspondence if Corresp Staus and Sub- Staus is other than Open & Saved/ReadyToSubmit respectively. 
      else
      {
        // Get expiry date for creating N th correspondence.
        expiryDate = ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.PaxOtherCorrespondence, invoice.SettlementMethodId, correspondence.CorrespondenceDate, invoice);

        // Logic to get and set billing period info for corresp before which BM should be created in case:
        // 1. Due To Expiry i.e. other party(first corresp to member) fails to respond with in time limit.
        // 2. Due To Authority To Bill i.e. other party has autorized the corresp initiating party(first corresp from member) to create a BM.
        // Both the cases use Method D for calculating Billing Period info.
        //e.g.
        // 1. PAX 5th Correspondence (expired)  BM
        //    a. Date of 5th correspondence’s transmission: 03-Jan-2012
        //    b. Value of time limit: 5
        //    c. Time limit for BM: 2012-Jun-P4, calculated as:
        //       i.  2012-Jan + 5 months = 2012-Jun
        //       ii. Period = 4 (always)

        // 2. PAX 2nd Correspondence (having authority to bill)  BM
        //    a. Date of 2nd correspondence’s transmission: 13-Jul-2012
        //    b. Value of time limit: 5
        //    c. Time limit for BM: 2012-Dec-P4, calculated as:
        //       i.  2012-Jul + 5 months = 2012-Dec
        //       ii. Period = 4 (always)
        DateTime bmExpiryPeriod = DateTime.MinValue;

        // Implementation for case 1 mentioned above.
        // Corresp having odd stage, will always be the corresp created by corresp initiating party(first corresp from member).
        if ((correspondence.CorrespondenceStage % 2) > 0)
        {
          // Get time limit for BillingMemoDueToExpiry transc type.
          var bmTimeLimit =
          ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.PasNsBillingMemoDueToExpiry,
                                                                  invoice.SettlementMethodId,
                                                                  correspondence.CorrespondenceDate);
          // Method D implementation
          bmExpiryPeriod = new DateTime(correspondence.CorrespondenceDate.Year, correspondence.CorrespondenceDate.Month, 4).AddMonths(bmTimeLimit.Limit);
        }
        // Implementation for case 2 mentioned above.
        // Corresp having even stage and having authority to bill, will always be the corresp created by the other party(first corresp to member).
        else if (correspondence.AuthorityToBill)
        {
          // Get time limit for BillingMemoDueToAuthorityToBill transc type.
          var bmTimeLimit =
          ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.PasNsBillingMemoDueToAuthorityToBill,
                                                                  invoice.SettlementMethodId,
                                                                  correspondence.CorrespondenceDate);
          // Method D implementation
          bmExpiryPeriod = new DateTime(correspondence.CorrespondenceDate.Year, correspondence.CorrespondenceDate.Month, 4).AddMonths(bmTimeLimit.Limit);
        }

        if (bmExpiryPeriod > DateTime.MinValue)
        {
          correspondence.BMExpiryPeriod = bmExpiryPeriod;
        }

      }// End else

      if (expiryDate > DateTime.MinValue && expiryDate != correspondence.ExpiryDate)
      {
        correspondence.ExpiryDate = expiryDate.Date;
      }// End if
      
    }


    /// <summary>
    /// This method creates Pdf report for Cargo Correspondences
    /// Will be called from service
    /// </summary>
    /// <param name="message"></param>
    /// <param name="basePath"></param>
    public string CreatePaxCorrespondenceTrailPdf(ReportDownloadRequestMessage message, string basePath)
    {
      Logger.Info("Recieved Correspondence Trail Report Request ");
      string requestingMemberNumericCode = MemberManager.GetMember(message.RequestingMemberId).MemberCodeNumeric;
      string reportZipFolderName = string.Format("PCORR-{0}-{1}", requestingMemberNumericCode, message.RecordId);
      List<long> correspondenceNumbers = message.CorrespondenceNumbers;
      Logger.InfoFormat("Number of Correspondence Trails {0} ", correspondenceNumbers.Count);
			Int32[] correspondenceSubStatus = {
    	                                  	(Int32) CorrespondenceSubStatus.Saved,
    	                                  	(Int32) CorrespondenceSubStatus.ReadyForSubmit
    	                                  };
      var correspondanceTrailReportManager = Ioc.Resolve<ICorrespondanceTrailReportManager>();

      var reportPdfPaths = new List<string>();
      foreach (var correspondenceNumber in correspondenceNumbers)
      {
        var correspondences = new List<Correspondence>();
        var fromReportModule = new List<Contact>();
        var toReportModule = new List<Contact>();
        string otherMemberNumericCode = string.Empty;

        var fromContacts = new List<CorrespondenceReportContact>();
        var toContacts = new List<CorrespondenceReportContact>();

        var corrs = PaxCorrespondenceRepository.GetCorrespondenceForTraiReport(corr => corr.CorrespondenceNumber == correspondenceNumber);
        if (corrs == null || corrs.Count() == 0)
        {
          Logger.InfoFormat("No correspondences found for correspondence number {0} ", correspondenceNumber);
          continue;
        }
        //correspondences.AddRange(corrs.OrderBy(corr => corr.CorrespondenceStage));
				//SCP241018 - SIS: Download Correspondence- SIS Production is different with Billing History and Correspondence.
        //Those correspondence should not display in the pdf which has status "Ready For Submit" or "Saved" by billed member.
	correspondences = (from corr in corrs where (corr.ToMemberId != message.RequestingMemberId || !correspondenceSubStatus.Contains(corr.CorrespondenceSubStatusId)) orderby corr.CorrespondenceStage select corr).ToList();
        Logger.InfoFormat("Number of correspondences for correspondence Number  {0} : {1}  ", correspondenceNumber, correspondences.Count);

        if (message.RequestingMemberId == correspondences[0].FromMemberId)
        {
          otherMemberNumericCode = MemberManager.GetMember(correspondences[0].ToMemberId).MemberCodeNumeric;
        }
        else
        {
          otherMemberNumericCode = MemberManager.GetMember(correspondences[0].FromMemberId).MemberCodeNumeric;
        }
        Location memberLocation;
        Country country;

        for (int correspondencesIndex = 0; correspondencesIndex < correspondences.Count; correspondencesIndex++)
        {
          CorrespondenceReportContact reportModule = new CorrespondenceReportContact();
          fromReportModule.Add(GetContactDetails(correspondences[correspondencesIndex].FromMemberId, ProcessingContactType.PaxCorrespondence));

          reportModule.AddressLine1 = fromReportModule[correspondencesIndex].AddressLine1;
          reportModule.AddressLine2 = fromReportModule[correspondencesIndex].AddressLine2;
          reportModule.AddressLine3 = fromReportModule[correspondencesIndex].AddressLine3;
          reportModule.CityName = fromReportModule[correspondencesIndex].CityName;
          reportModule.Country = fromReportModule[correspondencesIndex].Country;
          reportModule.CountryId = fromReportModule[correspondencesIndex].CountryId;
          reportModule.EmailAddress = fromReportModule[correspondencesIndex].EmailAddress;
          reportModule.FirstName = fromReportModule[correspondencesIndex].FirstName;
          reportModule.LastName = fromReportModule[correspondencesIndex].LastName;
          reportModule.Location = fromReportModule[correspondencesIndex].Location;
          reportModule.LocationId = fromReportModule[correspondencesIndex].LocationId;
          reportModule.PositionOrTitle = fromReportModule[correspondencesIndex].PositionOrTitle;
          reportModule.PostalCode = fromReportModule[correspondencesIndex].PostalCode;
          reportModule.Salutation = fromReportModule[correspondencesIndex].Salutation;
          reportModule.SalutationId = fromReportModule[correspondencesIndex].SalutationId;

          fromContacts.Add(reportModule);
          CorrespondenceReportContact treportModule = new CorrespondenceReportContact();
          toReportModule.Add(GetContactDetails(correspondences[correspondencesIndex].ToMemberId, ProcessingContactType.PaxCorrespondence));


          treportModule.AddressLine1 = toReportModule[correspondencesIndex].AddressLine1;
          treportModule.AddressLine2 = toReportModule[correspondencesIndex].AddressLine2;
          treportModule.AddressLine3 = toReportModule[correspondencesIndex].AddressLine3;
          treportModule.CityName = toReportModule[correspondencesIndex].CityName;
          treportModule.Country = toReportModule[correspondencesIndex].Country;
          treportModule.CountryId = toReportModule[correspondencesIndex].CountryId;
          treportModule.Department = toReportModule[correspondencesIndex].Department;
          treportModule.Division = toReportModule[correspondencesIndex].Division;
          treportModule.EmailAddress = toReportModule[correspondencesIndex].EmailAddress;
          treportModule.FirstName = toReportModule[correspondencesIndex].FirstName;
          treportModule.LastName = toReportModule[correspondencesIndex].LastName;
          treportModule.Location = toReportModule[correspondencesIndex].Location;
          treportModule.LocationId = toReportModule[correspondencesIndex].LocationId;
          treportModule.PositionOrTitle = toReportModule[correspondencesIndex].PositionOrTitle;
          treportModule.PostalCode = toReportModule[correspondencesIndex].PostalCode;
          treportModule.Salutation = toReportModule[correspondencesIndex].Salutation;
          treportModule.SalutationId = toReportModule[correspondencesIndex].SalutationId;

          toContacts.Add(treportModule);

          //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact);
          if (fromContacts[correspondencesIndex].LocationId > 0)
          {
            memberLocation = MemberManager.GetMemberLocationDetails(fromContacts[correspondencesIndex].LocationId);
            fromContacts[correspondencesIndex].AddressLine1 = memberLocation.AddressLine1;
            fromContacts[correspondencesIndex].AddressLine2 = memberLocation.AddressLine2;
            fromContacts[correspondencesIndex].AddressLine3 = memberLocation.AddressLine3;
            fromContacts[correspondencesIndex].Country = memberLocation.Country;
            fromContacts[correspondencesIndex].PostalCode = memberLocation.PostalCode;
            fromContacts[correspondencesIndex].CityName = memberLocation.CityName;
          }

          //If Country is not populated, populate it explicitly for given id
          if (fromContacts[correspondencesIndex].Country == null && !string.IsNullOrEmpty(fromContacts[correspondencesIndex].CountryId))
          {
            var countryList = ReferenceManager.GetCountryList();
            country = (from c in countryList
                       where c.Id == fromContacts[correspondencesIndex].CountryId
                       select c).First();
            fromContacts[correspondencesIndex].Country = country;
          }
          //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
          if (toContacts[correspondencesIndex].LocationId > 0)
          {
            memberLocation = MemberManager.GetMemberLocationDetails(toContacts[correspondencesIndex].LocationId);
            toContacts[correspondencesIndex].AddressLine1 = memberLocation.AddressLine1;
            toContacts[correspondencesIndex].AddressLine2 = memberLocation.AddressLine2;
            toContacts[correspondencesIndex].AddressLine3 = memberLocation.AddressLine3;
            toContacts[correspondencesIndex].Country = memberLocation.Country;
            toContacts[correspondencesIndex].PostalCode = memberLocation.PostalCode;
            toContacts[correspondencesIndex].CityName = memberLocation.CityName;
          }
          else
          {
            memberLocation = MemberManager.GetMemberDefaultLocation(correspondences[correspondencesIndex].ToMemberId, "MAIN");
            toContacts[correspondencesIndex].AddressLine1 = memberLocation.AddressLine1;
            toContacts[correspondencesIndex].AddressLine2 = memberLocation.AddressLine2;
            toContacts[correspondencesIndex].AddressLine3 = memberLocation.AddressLine3;
            toContacts[correspondencesIndex].Country = memberLocation.Country;
            toContacts[correspondencesIndex].PostalCode = memberLocation.PostalCode;
            toContacts[correspondencesIndex].CityName = memberLocation.CityName;
          }
          //If Country is not populated, populate it explicitly for given id
          if (toContacts[correspondencesIndex].Country == null && !string.IsNullOrEmpty(toContacts[correspondencesIndex].CountryId))
          {
            var countryList = ReferenceManager.GetCountryList();
            country = (from c in countryList
                       where c.Id == toContacts[correspondencesIndex].CountryId
                       select c).First();
            toContacts[correspondencesIndex].Country = country;
          }
        }
        string reportPdfPath = correspondanceTrailReportManager.CreateCorrespondenceReportPdf(FileIo.GetForlderPath(SFRFolderPath.ISCorrRepFolder), correspondences, correspondenceNumber, fromContacts, toContacts, message.RequestingMemberId, otherMemberNumericCode);
        if (File.Exists(reportPdfPath))
        {
          Logger.InfoFormat("Correspondence Trail Pdf created for correspondence Number {0} at location {1} ", correspondenceNumber, reportPdfPath);
          reportPdfPaths.Add(reportPdfPath);
        }
        else
        {
          Logger.InfoFormat("Correspondence Trail Pdf creation failed forcorrespondence Number {0} ", correspondenceNumber);
        }
      }
      if (reportPdfPaths.Count > 0)
      {
        string reportZipFilePath = CreateCorrespondenceTrailZip(basePath, reportZipFolderName, reportPdfPaths);
        Logger.InfoFormat("Report Zip Created at location {0}", reportZipFilePath);
        return reportZipFilePath;
      }
      else
      {
        return string.Empty;
      }

    }

    private string CreateCorrespondenceTrailZip(string basePath, string zipFolderName, List<string> reportPdfPaths)
    {
      Logger.InfoFormat("Creating Zip ");
      var zipFileName = string.Format("{0}.ZIP", zipFolderName);
      var sfrTempRootPath = FileIo.GetForlderPath(SFRFolderPath.ISCorrRepFolder);
      var zipFolder = Path.Combine(sfrTempRootPath, zipFolderName);

      FileIo.ZipOutputFile(zipFolder, string.Empty, zipFileName, reportPdfPaths.ToArray());
      // FileIo.MoveFile(Path.Combine(sfrTempRootPath, zipFileName), Path.Combine(basePath, zipFileName));
      return Path.Combine(basePath, zipFileName);

    }

    /// <summary>
    /// Gets Only Correspondence from database Using Load Strategy. No other details like attachment, from member/ to member etc.. will be loaded.
    /// </summary>
    /// <param name="correspondenceId"></param>
    /// <param name="correspondenceNumber"></param>
    /// <param name="correspondenceStage"></param>
    /// <returns></returns>
    public Correspondence GetOnlyCorrespondenceUsingLoadStrategy(string correspondenceId)
    {
        var correspondenceGuid = correspondenceId.ToGuid();
        var correspondenceHeader = PaxCorrespondenceRepository.GetOnlyCorrespondenceUsingLoadStrategy(correspondenceGuid);
        return correspondenceHeader;
    }

    /// <summary>
    /// Gets a last responded correspondence from database. Useful for Close functionality in order to send emails 
    /// to all email Ids in previously responded state.
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    public Correspondence GetLastRespondedCorrespondene(long correspondenceNumber)
    {
        var correspondence =
            PaxCorrespondenceRepository.GetLastRespondedCorrespondene(corr => corr.CorrespondenceNumber == correspondenceNumber &&
                    corr.CorrespondenceSubStatusId == 2).OrderByDescending(corr => corr.CorrespondenceStage).FirstOrDefault();

        return correspondence;
    }

  }
}

