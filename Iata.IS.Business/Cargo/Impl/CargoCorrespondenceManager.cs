using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Smtp;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Reports.Cargo;
using Iata.IS.Business.Reports.Common;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;
using NVelocity;
using TransactionType = Iata.IS.Model.Enums.TransactionType;


namespace Iata.IS.Business.Cargo.Impl
{
  public class CargoCorrespondenceManager : CorrespondenceManager,ICargoCorrespondenceManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /* /// <summary>
    /// Gets or sets the correspondence repository.
    /// </summary>
    /// <value>The country repository.</value> */
    public ICargoCorrespondenceRepository CargoCorrespondenceRepository { get; set; }
    public ICargoInvoiceRepository CargoInvoiceRepository { get; set; }
    public IRejectionMemoRecordRepository RejectionMemoRecordRepository { get; set; }
    public ICargoCorrespondenceAttachmentRepository CargoCorrespondenceAttachmentRepository { get; set; }

    /// <summary>
    /// Gets or sets the reference manager.
    /// </summary>
    /// <value>The reference manager.</value>
    public IReferenceManager ReferenceManager { get; set; }
    
    public ICalendarManager CalendarManager { get; set; }

    // SCP210204: IS-WEB Outage [To resolve null reference]
    public CargoCorrespondenceManager(IReferenceManager referenceManager) : base(referenceManager)
    {
    }

    public CargoCorrespondence AddCorrespondence(CargoCorrespondence cargoCorrespondence)
    {
      if (ValidateCorrespondence(cargoCorrespondence))
      {
        // SCP109163
        // Check if correspondence expiry date is crossed.
        if (cargoCorrespondence.ExpiryDate < DateTime.UtcNow.Date)
        {
          throw new ISBusinessException(CargoErrorCodes.ExpiredCorrespondence);
        }
        // Mark the correspondence status as Open.
        cargoCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;

        // Check if correspondence already present and in responded status.
        //var latestCorrespondence = GetRecentCorrespondenceDetails(cargoCorrespondence.CorrespondenceNumber);

        //if (latestCorrespondence != null && latestCorrespondence.CorrespondenceStage == cargoCorrespondence.CorrespondenceStage)
        //{
        //  throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadySent);
        //}

        // Validation for correspondence.

        CargoCorrespondenceRepository.Add(cargoCorrespondence);
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

            Logger.Error("Exception in Cgo AddCorrespondence Method.");

            Logger.ErrorFormat(
              "Exception Message: {0}, Inner Exception Message: {1}, Stack Trace: {2},  corr Ref No: {3}, stage: {4}, from: {5}, to: {6}, Amount: {7}, Subject: {8}, curresncy Code: {9}, corr details: {10}, status: {11}, sub-status: {12}",
              exception.Message, exception.InnerException.Message, exception.StackTrace,
              cargoCorrespondence.CorrespondenceNumber, cargoCorrespondence.CorrespondenceStage,
              cargoCorrespondence.FromMemberId, cargoCorrespondence.ToMemberId, cargoCorrespondence.AmountToBeSettled,
              cargoCorrespondence.Subject, cargoCorrespondence.CurrencyId, cargoCorrespondence.CorrespondenceDetails,
              cargoCorrespondence.CorrespondenceStatusId, cargoCorrespondence.CorrespondenceSubStatusId);

            Logger.Error(exception);

            if (exception.InnerException != null)
              Logger.Error(exception.InnerException);

            if (tryCount == 24 || cargoCorrespondence.CorrespondenceStage > 1)
              throw new ISBusinessException(ErrorCodes.InvalidCorrespondencRefNo);

            if (cargoCorrespondence.CorrespondenceStage == 1)
            {
              cargoCorrespondence.CorrespondenceNumber++;
            }
          }
        }

        // Update purging expiry period for correspondence.
        //SCP0000: PURGING AND SET EXPIRY DATE (REMOVE SET EXPIRY DATE FROM MEMOS AND CORR LEVEL)
        //UpdatePurgingExpiryPeriod(cargoCorrespondence);
      }

      return cargoCorrespondence;
    }

    /// <summary>
    /// Following method is used to Add Correspondence and update respective Rejection Memo
    /// </summary>
    /// <param name="cgoCorrespondence">Correspondence object</param>
    /// <param name="correspondenceAttachmentIds">Attachment list</param>
    /// <param name="rejectionMemoIds">Rejection memo id's string</param>
    /// <returns>Cargo correspondence object</returns>
    public CargoCorrespondence AddCorrespondenceAndUpdateRejection(CargoCorrespondence cgoCorrespondence, List<Guid> correspondenceAttachmentIds, string rejectionMemoIds, ref int operationStatusIndicator)
    {
        if (ValidateCorrespondence(cgoCorrespondence))
        {
            // Check if correspondence expiry date is crossed.
            if (cgoCorrespondence.ExpiryDate < DateTime.UtcNow.Date)
            {
                throw new ISBusinessException(CargoErrorCodes.ExpiredCorrespondence);
            }

            // Mark the correspondence status as Open.
            cgoCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;

            CargoCorrespondenceRepository.Add(cgoCorrespondence);

            //Update parent Id for attachment
            var attachmentInDb = CargoCorrespondenceAttachmentRepository.Get(bmCouponAttachment => correspondenceAttachmentIds.Contains(bmCouponAttachment.Id));
            foreach (var recordAttachment in attachmentInDb)
            {
                recordAttachment.ParentId = cgoCorrespondence.Id;
                CargoCorrespondenceAttachmentRepository.Update(recordAttachment);
            }

            /* SCP106534: ISWEB No-02350000768 
            Desc: Create corr is pushed to DB for better concurrency control. This prevents creation of orphan stage 1 corr in pax and cgo.
             * Attachments to be marked as a part of corr for updation in DB
            Date: 20/06/2013*/
            if(attachmentInDb != null)
            {
                cgoCorrespondence.Attachments = attachmentInDb.ToList();
            }

            //SCP106534: ISWEB No-02350000768 
            //Desc: As RM is expected for stage 1 corr. if not found throwing business exception.
            //Date: 20/06/2013
            if (string.IsNullOrEmpty(cgoCorrespondence.RejectionMemoIds) && cgoCorrespondence.CorrespondenceStage == 1)
            {
                /* Business exception - Invalid RM */
                throw new ISBusinessException(ErrorCodes.InvalidRejectionMemo);
            }

            // Update Correspondence Id in Rejection Memo only in case of first stage correspondence, 
            if (cgoCorrespondence.CorrespondenceStage == 1)
            {
                char[] sep = { ',' };
                var sRejectedMemoIds = cgoCorrespondence.RejectionMemoIds.Split(sep, StringSplitOptions.RemoveEmptyEntries);

                foreach (var rejMemoId in sRejectedMemoIds)
                {
                    var rejectionMemo = GetRejectedMemoDetails(rejMemoId);
                    // Update correspondence id for RM only if it is not assigned before.
                    if (rejectionMemo != null && !rejectionMemo.CorrespondenceId.HasValue)
                    {
                        rejectionMemo.CorrespondenceId = cgoCorrespondence.Id;
                        RejectionMemoRecordRepository.Update(rejectionMemo);
                    }
                    // if correspondence id for RM already exists then communicate the same
                    else if (rejectionMemo != null && rejectionMemo.CorrespondenceId.HasValue)
                    {
                        throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadyCreated);
                    }
                    // Problem getting RM and so provide such error.
                    else if (rejectionMemo == null)
                    {
                        /* Business exception - Invalid RM */
                        throw new ISBusinessException(ErrorCodes.InvalidRejectionMemo);
                    }
                }
            }

            operationStatusIndicator = CargoCorrespondenceRepository.CreateCorrespondence(ref cgoCorrespondence);
            //operationStatusIndicator value interpretation - 
            //-1 => Internal DB Exception
            //0  => Success (this is when CORRESPONDENCE_ID_O will have a value)
            //1  => RM already has corr linked to it.
            //2  => Problem updating RM
            //3  => ANYTHING BUT STAGE 1 CORR ALREADY EXISTS.

            // Over email decided to show same error in both these cases (and any other un anticipated case)
            //(operationStatusIndicator == -1 || operationStatusIndicator == 2)
            //Email attached to SCP 106534
            //Date: 20/06/2013
            if (operationStatusIndicator == 1)
            {
                throw new ISBusinessException(ErrorCodes.ErrorCorrespondenceAlreadyCreated);
            }
            else if(operationStatusIndicator == 3)
            {
                throw new ISBusinessException(ErrorCodes.CorrespondenceConcurrentUpdateError);
            }
            else if (operationStatusIndicator == -1 || operationStatusIndicator == 2)
            {
                throw new ISBusinessException(ErrorCodes.InternalDBErrorInCorrespondenceCreation);
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

        return cgoCorrespondence;
    }


    public CargoCorrespondence UpdateCorrespondence(CargoCorrespondence cargoCorrespondence)
    {
      if (ValidateCorrespondence(cargoCorrespondence))
      {
        //Replaced with Load Strategy implementation
        var correspondenceHeaderfromdb = CargoCorrespondenceRepository.Single(correspondenceId: cargoCorrespondence.Id);

        var updatedCorrespondenceData = CargoCorrespondenceRepository.Update(cargoCorrespondence);

        // Changes to update attachment breakdown records.
        var listToDeleteAttachment = correspondenceHeaderfromdb.Attachments.Where(attachment => cargoCorrespondence.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

        var attachmentIdList = (from attachment in cargoCorrespondence.Attachments
                                where correspondenceHeaderfromdb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                                select attachment.Id).ToList();

        if (attachmentIdList.Count > 0)
        {
          var attachmentInDb = CargoCorrespondenceAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));

          foreach (var recordAttachment in attachmentInDb)
          {
            if (IsDuplicateCorrespondenceAttachmentFileName(recordAttachment.OriginalFileName, cargoCorrespondence.Id))
            {
              throw new ISBusinessException(ErrorCodes.DuplicateFileName);
            }

            recordAttachment.ParentId = cargoCorrespondence.Id;
            CargoCorrespondenceAttachmentRepository.Update(recordAttachment);
          }
        }

        foreach (var rmCouponRecordAttachment in listToDeleteAttachment)
        {
          CargoCorrespondenceAttachmentRepository.Delete(rmCouponRecordAttachment);
        }

        UnitOfWork.CommitDefault();

        // Update correspondence purging expiry date.
        //SCP0000: PURGING AND SET EXPIRY DATE (REMOVE SET EXPIRY DATE FROM MEMOS AND CORR LEVEL)
        //UpdatePurgingExpiryPeriod(cargoCorrespondence);

        return updatedCorrespondenceData;
      }
      
      return cargoCorrespondence;
    }
    //SCP0000: PURGING AND SET EXPIRY DATE (REMOVE SET EXPIRY DATE FROM MEMOS AND CORR LEVEL)
    ///// <summary>
    ///// Updates the expiry period of correspondence for purging purpose.
    ///// </summary>
    ///// <param name="cargoCorrespondence">The correspondence.</param>
    //private void UpdatePurgingExpiryPeriod(CargoCorrespondence cargoCorrespondence)
    //{
    //  var invoiceBase = cargoCorrespondence.Invoice ?? CargoInvoiceRepository.Single(id: cargoCorrespondence.InvoiceId);

    //  // Set Perge Date only when correspondence is in Open Responded state.
    //  if (cargoCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open && cargoCorrespondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded)
    //  {
    //    TransactionType transactionType = cargoCorrespondence.AuthorityToBill ? TransactionType.CargoBillingMemoDueToAuthorityToBill : TransactionType.CargoOtherCorrespondence;
        
    //    DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(transactionType, invoiceBase, BillingCategoryType.Cgo, Constants.SamplingIndicatorNo, null, true, cargoCorrespondence);
        
    //    CargoInvoiceRepository.UpdateExpiryDatePeriod(cargoCorrespondence.Id, (int)TransactionType.CargoOtherCorrespondence, expiryPeriod);
            
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
      return CargoCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == miscCorrespondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    public void UpdateRejectedMemo(CargoRejectionMemo rejectedMemo)
    {
      RejectionMemoRecordRepository.Update(rejectedMemo);
      UnitOfWork.CommitDefault();
    }

    public IList<CargoCorrespondenceAttachment> UpdateCargoCorrespondenceAttachment(List<Guid> correspondenceAttachmentIds, Guid correspondenceId)
    {
      var attachmentInDb = new List<CargoCorrespondenceAttachment>().AsQueryable();

      if (correspondenceAttachmentIds.Count > 0)
      {
        attachmentInDb = CargoCorrespondenceAttachmentRepository.Get(bmCouponAttachment => correspondenceAttachmentIds.Contains(bmCouponAttachment.Id));

        foreach (var recordAttachment in attachmentInDb)
        {
          recordAttachment.ParentId = correspondenceId;
          CargoCorrespondenceAttachmentRepository.Update(recordAttachment);
        }

        UnitOfWork.CommitDefault();
      }

      return attachmentInDb.ToList();
    }

    /* public bool DeleteCorrespondence(string correspondenceId)
    {
      throw new NotImplementedException();
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
    } */

    public bool IsDuplicateCargoCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId)
    {
      return CargoCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == correspondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="cargoCorrespondence"></param>
    /// <returns></returns>
    public bool ValidateCorrespondence(CargoCorrespondence cargoCorrespondence)
    {

        // Validation for CityAirport as it is auto complete field in UI
        var toEmailId = cargoCorrespondence.ToEmailId != null
                            ? cargoCorrespondence.ToEmailId.Trim()
                            : cargoCorrespondence.ToEmailId;

        /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids from initiator and non-initiator and removing
                 additional email field*/
        var additionalEmailInitiator = cargoCorrespondence.AdditionalEmailInitiator != null
                                           ? cargoCorrespondence.AdditionalEmailInitiator.Trim()
                                           : cargoCorrespondence.AdditionalEmailInitiator;
        var additionalEmailNonInitiator = cargoCorrespondence.AdditionalEmailNonInitiator != null
                                              ? cargoCorrespondence.AdditionalEmailNonInitiator.Trim()
                                              : cargoCorrespondence.AdditionalEmailNonInitiator;
        if (string.IsNullOrEmpty(toEmailId + additionalEmailInitiator + additionalEmailNonInitiator))
        {
            throw new ISBusinessException(CargoErrorCodes.EnterEmailIds);
        }

        // Validation of Correspondence Subject
        if (string.IsNullOrEmpty(cargoCorrespondence.Subject))
        {
            throw new ISBusinessException(CargoErrorCodes.InvalidCorrespondenceSubject);
        }

        if (cargoCorrespondence.AmountToBeSettled < 0)
        {
            throw new ISBusinessException(CargoErrorCodes.CargoInvalidAmountToBeSettled);
        }

        var invoice = GetInvoiceDetail(cargoCorrespondence.InvoiceId.ToString());
        if (
            !ReferenceManager.IsValidNetAmount(Convert.ToDouble(cargoCorrespondence.AmountToBeSettled),
                                               cargoCorrespondence.CorrespondenceStage == 1
                                                   ? TransactionType.CargoCorrespondence
                                                   : TransactionType.CargoOtherCorrespondence,
                                               cargoCorrespondence.CurrencyId, invoice, isCorrespondence: true,
                                               correspondence: cargoCorrespondence, validateMaxAmount: false,
                                               applicableMinimumField: ApplicableMinimumField.AmountToBeSettled))
        {
            throw new ISBusinessException(ErrorCodes.CorrespondenceAmountIsNotInAllowedRange);
        }
        // Validation for CityAirport as it is auto complete field in UI
        if (string.IsNullOrEmpty(cargoCorrespondence.CorrespondenceNumber.ToString()))
        {
            throw new ISBusinessException(CargoErrorCodes.InvalidCorrespondenceNumber);
        }

        // Validation for CityAirport as it is auto complete field in UI
        if (cargoCorrespondence.CurrencyId == 0)
        {
            throw new ISBusinessException(CargoErrorCodes.InvalidCorrespondenceNumber);
        }

        //You cannot send a correspondence if it is expired
        if ((cargoCorrespondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded))
        {
            // var invoice = GetInvoiceDetail(cargoCorrespondence.InvoiceId.ToString());
            //if(IsCorrespondenceOutsideTimeLimit(cargoCorrespondence, invoice))
            //{
            //  throw new ISBusinessException(MiscUatpErrorCodes.ExpiredCorrespondence);
            //}
        }

        if (!HasValidAuthorityToBill(cargoCorrespondence))
        {
            throw new ISBusinessException(CargoErrorCodes.InvalidAuthorityToBill);
        }

        /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids of To,Initiator and Non-Initiator */
        var toEmailIds = GetEmailIdsList(cargoCorrespondence.ToEmailId,
                                         cargoCorrespondence.AdditionalEmailInitiator,
                                         cargoCorrespondence.AdditionalEmailNonInitiator);

      if (ValidateToEmailIds(toEmailIds) == false)
      {
          throw new ISBusinessException(CargoErrorCodes.InvalidEmailIds);
      }

        //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
        var nonSamplingInvoiceManager = Ioc.Resolve<ICargoInvoiceManager>(typeof (ICargoInvoiceManager));
        var billingMemos =
            nonSamplingInvoiceManager.GetBillingMemosForCorrespondence(cargoCorrespondence.CorrespondenceNumber.Value,
                                                                       cargoCorrespondence.FromMemberId);
        if(billingMemos.Transactions != null && billingMemos.Transactions.Count > 0)
        {
            throw new ISBusinessException(string.Format(Messages.BCGO_10380,
                                            billingMemos.Transactions[0].InvoiceNumber,
                                            billingMemos.Transactions[0].InvoicePeriod,
                                            billingMemos.Transactions[0].BatchNumber,
                                            billingMemos.Transactions[0].SequenceNumber,
                                            billingMemos.Transactions[0].BillingMemoNumber));
        }

      // Update the expiry date of correspondence.
      //if (cargoCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Saved && cargoCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.ReadyForSubmit)
      //{
      UpdateExpiryDate(cargoCorrespondence, invoice);
      //}
      return true;
    }

    private bool IsCorrespondenceOutsideTimeLimit(CargoCorrespondence cargoCorrespondence, CargoInvoice invoice, ref bool isTimeLimitRecordFound)
    {
      bool isInsideTimeLimit = true;
      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      SMI settlementMethod = (SMI) invoice.SettlementMethodId;
      if(ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
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
            isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodB(TransactionType.CargoCorrespondence, (int)invoice.InvoiceSmi, previousTransactionDate, invoice, cargoCorrespondence.CorrespondenceDate, ref isTimeLimitRecordFound);
          break;
        case SMI.Ach:
          isInsideTimeLimit = ReferenceManager.IsTransactionInTimeLimitMethodE(TransactionType.CargoCorrespondence, (int)invoice.InvoiceSmi, invoice, ref isTimeLimitRecordFound);
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

      transactionType = TransactionType.CargoCorrespondence;
      //CMP#624 : 2.10 - Change#6 : Time Limits
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
      SMI settlementMethod = (SMI)invoice.SettlementMethodId;
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
    public CargoCorrespondenceAttachment AddCorrespondenceAttachment(CargoCorrespondenceAttachment attach)
    {
      CargoCorrespondenceAttachmentRepository.Add(attach);
      UnitOfWork.CommitDefault();
      //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
      //attach = CargoCorrespondenceAttachmentRepository.Single(a => a.Id == attach.Id);
      return attach;
    }

    private bool HasValidAuthorityToBill(CargoCorrespondence cargoCorrespondence)
    {
      if (cargoCorrespondence.AuthorityToBill)
      {
        var correspondence = GetOriginalCorrespondenceDetails(cargoCorrespondence.CorrespondenceNumber);
        if (correspondence != null)
        {
          if (correspondence.FromMemberId == cargoCorrespondence.FromMemberId)
          {
            return false;
          }
        }
      }

      return true;
    }

    public CargoCorrespondence GetOriginalCorrespondenceDetails(long? correspondenceNumber)
    {
      var corrrespondence = CargoCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderBy(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return corrrespondence;
    }


    /// <summary>
    /// This function is used to get recent correspondence detail.
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    public CargoCorrespondence GetRecentCorrespondenceDetails(long? correspondenceNumber)
    {
      //INC 8863, I get an unexpected error occurred
      var corrrespondence = CargoCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return corrrespondence;
    }

    /// <summary>
    /// Fix Buf 8881 for CMP527: Creating new method for get last corrpesondence details.
    /// </summary>
    /// <param name="correspondenceNumber">correspondence number</param>
    /// <returns></returns>
    /// SCP199693: create BM and close correspondence at same time
    public CargoCorrespondence GetRecentCorrespondenceDetailWithClosedStatus(long? correspondenceNumber)
    {
      var corrrespondence = CargoCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return corrrespondence;
    }

    /* public IList<Correspondence> GetCorrespondenceHistoryList(string invoiceId)
    {
      throw new NotImplementedException();
    } */

    public IList<CargoRejectionMemo> GetCorrespondenceRejectionList(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      if (correspondenceGuid == Guid.Empty) return null;

      //Replaced with LoadStrategy call
      var correspondence = CargoCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);

      correspondence = correspondence.CorrespondenceStage == 1 ? correspondence : GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);

      var rejectionMemoList = RejectionMemoRecordRepository.Get(memo => memo.CorrespondenceId == correspondence.Id);

      return rejectionMemoList.ToList();
    }

    public CargoInvoice GetCorrespondenceRelatedInvoice(string correspondenceId)
    {
        var correspondenceGuid = correspondenceId.ToGuid();

        //Replaced with LoadStrategy call
        var correspondence = CargoCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);

        //Replaced with LoadStrategy call
        var firstCorrespondence = correspondence.CorrespondenceStage == 1
                                    ? correspondence
                                    : CargoCorrespondenceRepository.Single(correspondenceNumber: correspondence.CorrespondenceNumber, correspondenceStage: 1);

        //Replaced with LoadStrategy call
        var rejectionMemo = RejectionMemoRecordRepository.First(memo => memo.CorrespondenceId == firstCorrespondence.Id);
        //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
        return rejectionMemo != null ? CargoInvoiceRepository.Single(id: rejectionMemo.InvoiceId) : null;
        //return rejectionMemo != null ? CargoInvoiceRepository.Get(i=>i.Id==rejectionMemo.InvoiceId).SingleOrDefault() : null;
    }

    public CargoInvoice GetInvoiceDetail(string invoiceId)
    {
        Guid cgoInvoiceId;
        return Guid.TryParse(invoiceId, out cgoInvoiceId) ? CargoInvoiceRepository.Single(id: cgoInvoiceId) : null;
    }

    public Member GetMember(int memberId)
    {
      return MemberManager.GetMember(memberId);
    }

    public long GetCorrespondenceNumber(int memberId)
    {
      var correspondence = CargoCorrespondenceRepository.Get(c => c.FromMemberId == memberId && c.CorrespondenceStage == 1).OrderByDescending(c => c.CorrespondenceNumber).FirstOrDefault();
      long correspondNumber = 0;

      if (correspondence != null && correspondence.CorrespondenceNumber != null)
        correspondNumber = correspondence.CorrespondenceNumber.Value;

      return correspondNumber;
    }


    public long GetInitialCorrespondenceNumber(int memberId)
    {
        var correspondenceNumber = CargoCorrespondenceRepository.GetInitialCorrespondenceNumber(memberId);

        return correspondenceNumber;

    }

    // CMP#657: Retention of Additional Email Addresses in Correspondences.
    // Logic Moved to common location. i.e in corresponceManager.cs
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
    //        throw new ISBusinessException(GetErrorDescription(CargoErrorCodes.FailedToSendMail, eMailId.Trim()));
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

    /* public string GetChargeCodes(string invoiceId)
    {
      throw new NotImplementedException();
    } */

    public CargoCorrespondence GetRecentCorrespondenceDetails(string invoiceId)
    {
      /* var invoiceGuid = invoiceId.ToGuid();
      var corrrespondence = CargoCorrespondenceRepository.Get(correspondence => correspondence.InvoiceId == invoiceGuid
        && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
        && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
        && correspondence.CorrespondenceSubStatusId == 2)))
        )).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault(); */
      return new CargoCorrespondence();
    }

    public CargoCorrespondence GetRecentCorrespondenceDetails(string correspondenceId, int billingMemberId)
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
      var contactTypeList = MemberManager.GetContactsForContactType(memberId, processingContact);
      var contact = new Contact();

      if (contactTypeList != null && contactTypeList.Count > 0)
      {
        contact = contactTypeList.First();
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

    /* public IList<Correspondence> GetCorrespondenceHistoryList(string correspondenceId, int billingMemberId)
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

    } */

    public bool IsFirstCorrespondence(int memberId)
    {
      var correspondence = CargoCorrespondenceRepository.Get(c => c.FromMemberId == memberId && c.CorrespondenceStage == 1).FirstOrDefault();

      if (correspondence == null)
      {
        return true;
      }

      return false;
    }

    public List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold)
    {
      return CargoCorrespondenceRepository.UpdateCorrespondenceStatus(billingPeriod, _oornThreshold, _oernThreshold, _eornThreshold, _eoryThreshold, _eorybThreshold);
    }

    public string CreateCorrespondenceFormatPdf(string correspondenceId)
    {
      //CMP 527: Add variable to show acceptance comments on PDF.
      var isclosedByInitiator = false;

        var fromContact = new CorrespondenceReportContact();
        var toContact = new CorrespondenceReportContact();

        CorrespondenceReportContact reportModule = new CorrespondenceReportContact();
        CorrespondenceReportContact treportModule = new CorrespondenceReportContact();

      var correspondenceDetails = GetCorrespondenceDetails(correspondenceId);

      if(!string.IsNullOrEmpty(correspondenceDetails.AcceptanceComment))
      {
        isclosedByInitiator = true;
      }
      string correspondenceFormatReportPath = FileIo.GetForlderPath(SFRFolderPath.CorrespondenceFormatReportPath) + correspondenceId.ToGuid() + ".pdf";

      var fromContacts = GetContactDetails(correspondenceDetails.FromMemberId, ProcessingContactType.CargoCorrespondence);

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

      var toContacts = GetContactDetails(correspondenceDetails.ToMemberId, ProcessingContactType.CargoCorrespondence);

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

      var cargoReportManager = Ioc.Resolve<ICargoReportManager>();
      cargoReportManager.BuildCorrespondenceFormatReport(correspondenceDetails, ref correspondenceFormatReportPath, fromContact, toContact);

      return correspondenceFormatReportPath;
    }

    public CargoCorrespondence GetFirstCorrespondenceDetails(string correspondenceId)
    {
        /* SCP106534: ISWEB No-02350000768 
        Desc: Create corr is pushed to DB for better concurrency control. This prevents creation of orphan stage 1 corr in pax and cgo.
         * Added try-catch block to prevent screen crashing.
        Date: 20/06/2013*/
        try
        {
            var correspondenceGuid = correspondenceId.ToGuid();

            if (correspondenceGuid == Guid.Empty) return null;

            var correspondence = CargoCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
            if (correspondence.CorrespondenceStage == 1) return correspondence;

            //Replaced with LoadStrategy call
            correspondence = CargoCorrespondenceRepository.Single(correspondenceNumber: correspondence.CorrespondenceNumber, correspondenceStage: 1);

            return correspondence;
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// To Get last Corr Details for given corr No
    /// </summary>
    /// <param name="correspondenceRefNo"></param>
    /// <returns></returns>
    public CargoCorrespondence GetLastCorrespondenceDetails(long? correspondenceRefNo)
    {
      if (correspondenceRefNo != null)
      {
        var correspondence = CargoCorrespondenceRepository.GetAll().Where(corr => corr.CorrespondenceNumber == correspondenceRefNo).OrderByDescending(corr => corr.CorrespondenceStage).FirstOrDefault();
        return correspondence;
      }
      return null;
    }

    public CargoRejectionMemo GetRejectedMemoDetails(string rejectionMemoId)
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

    public CargoCorrespondence GetCorrespondenceDetails(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      //Replaced with LoadStrategy call
      var correspondenceHeader = CargoCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);

      return correspondenceHeader;
    }

    //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
      /// <summary>
      /// Retrieve the correspondence details from repository
      /// </summary>
      /// <param name="correspondenceId"></param>
      /// <returns></returns>
    public CargoCorrespondence GetCorrespondenceHeaderDetails(string correspondenceId)
    {
        var correspondenceGuid = correspondenceId.ToGuid();
        var correspondenceHeader = CargoCorrespondenceRepository.Get(i=>i.Id== correspondenceGuid).SingleOrDefault();
        return correspondenceHeader;
    }

    public CargoCorrespondenceAttachment GetCorrespondenceAttachmentDetail(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = CargoCorrespondenceAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Gets the Pax Correspondence attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<CargoCorrespondenceAttachment> GetCorrespondenceAttachments(List<Guid> attachmentIds)
    {
      return CargoCorrespondenceAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }

    /// <summary>
    /// Updates the expiry date.
    /// </summary>
    /// <param name="correspondence">The correspondence.</param>
    /// <param name="invoice">The invoice.</param>
    public void UpdateExpiryDate(CargoCorrespondence correspondence, CargoInvoice invoice)
    {
      DateTime expiryDate = DateTime.MinValue;
      var originalCorr = GetOriginalCorrespondenceDetails(correspondence.CorrespondenceNumber);
      //if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && correspondence.AuthorityToBill)
      //{ 
      //  expiryDate = !ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId)
      //                 ? ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.CargoBillingMemoDueToAuthorityToBill, invoice.SettlementMethodId, correspondence.CorrespondenceDate, invoice)
      //                 : ReferenceManager.GetTimeLimitMethodD1ExpiryDate(TransactionType.CargoBillingMemoDueToAuthorityToBill, Convert.ToInt32(SMI.Bilateral), correspondence.CorrespondenceDate, invoice);

      //  if (expiryDate != correspondence.CorrespondenceDate) correspondence.ExpiryDate = expiryDate;
      //}

      // If Corresp Status is Open and Sub Status is Saved/ Ready To Submit then set expiry date as expiry date of last transaction
      // i.e. 1. In case of N th Corresp set expiry equal to expiry of N-1 th Corresp
      //      2. In case of 1st Corresp set expiry equal to expiry of RM for which Corresp is created.
      if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit))
      {
        // N th Corresp Case
        if(correspondence.CorrespondenceStage > 1)
        {
          var correspondenceHistory = CargoCorrespondenceRepository.GetCorrespondenceForTraiReport(c => c.CorrespondenceNumber == correspondence.CorrespondenceNumber && c.InvoiceId == correspondence.InvoiceId).ToList();
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
              TransactionType.CargoCorrespondence, invoice.SettlementMethodId, new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod));

          if (firstCorrespTimeLimit != null)
          {
            // Check if Settlement Method is ACH then calculate expiry date by applying Calculation Method E.
            // Method E:
            // ‘X’ number of months is added to the month of the final rejection. The last calendar date of this 
            // month (newly calculated month) will be the time limit for the 1st correspondence. The actual period of the last rejection has no significance.
            // e.g. CGO 3rd Rejection  Correspondence #1
            //      1. Billing period of 3rd Rejection: 2011-Aug-P1
            //      2. Value of time limit: 6
            //      3. Time limit for the 1st Correspondence is calculated as 29-Feb-2012. Note: 2012 is a leap year, so 29th of Feb shall be the last calendar date
            if (invoice.SettlementMethodId == (int)SettlementMethodValues.Ach)
            {
              expiryDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1).AddMonths(firstCorrespTimeLimit.Limit).AddMonths(1).AddDays(-1);
            }
            // else calculate expiry date by applying Calculation Method B 
            // Method B:
            // Closure date of the 4th period of the billing month of the final rejection is determined (from the IS Calendar). 
            // The actual period of the last rejection has no significance. ‘X’ number of months is added to that date to determine the time limit for the 1st correspondence.
            // e.g. CGO 3rd Rejection  Correspondence #1
            //      1. Billing period of 3rd Rejection: 2010-Mar-P2
            //      2. Closure date of 2010-Mar-P4: 07-Apr-2010
            //      3. Value of time limit: 6
            //      4. Time limit for the 1st Correspondence is calculated as 07-Oct-2010
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
        }
        
      }
      // Get expiry date for N th Correspondence if Corresp Staus and Sub- Staus is other than Open & Saved/ReadyToSubmit respectively. 
      else
      {
          expiryDate = ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.CargoOtherCorrespondence, invoice.SettlementMethodId, correspondence.CorrespondenceDate, invoice);

          // Logic to get and set billing period info for corresp before which BM should be created in case:
          // 1. Due To Expiry i.e. other party(first corresp to member) fails to respond with in time limit.
          // 2. Due To Authority To Bill i.e. other party has autorized the corresp initiating party(first corresp from member) to create a BM.
          // Both the cases use Method D for calculating Billing Period info.
          //e.g.
          // 1. CGO 5th Correspondence (expired)  BM
          //    a. Date of 5th correspondence’s transmission: 03-Jan-2012
          //    b. Value of time limit: 5
          //    c. Time limit for BM: 2012-Jun-P4, calculated as:
          //       i.  2012-Jan + 5 months = 2012-Jun
          //       ii. Period = 4 (always)

          // 2. CGO 2nd Correspondence (having authority to bill)  BM
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
            ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.CargoBillingMemoDueToExpiry,
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
            ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.CargoBillingMemoDueToAuthorityToBill,
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

      if (expiryDate != DateTime.MinValue && expiryDate != correspondence.ExpiryDate)
      {
        correspondence.ExpiryDate = expiryDate.Date;
      }// End if

    }// End UpdateExpiryDate(string, int)

    public IList<CargoCorrespondence> GetCorrespondenceHistoryList(string correspondenceId, int billingMemberId)
    {

      var correspondenceGuid = correspondenceId.ToGuid();
      //Replaced with LoadStrategy call
      var correspondenceInDb = CargoCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);

      if (correspondenceInDb == null) return null;

      var correspondenceHistoryList = CargoCorrespondenceRepository.GetCorrespondenceWithAttachment(correspondence => correspondence.CorrespondenceNumber == correspondenceInDb.CorrespondenceNumber
        && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
        && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
        && correspondence.CorrespondenceSubStatusId == 2)))
        )).ToList();
      return correspondenceHistoryList.Count > 0 ? correspondenceHistoryList.OrderBy(corr => corr.CorrespondenceStage).ToList() : correspondenceHistoryList;

    }


    /// <summary>
    /// This method creates Pdf report for Cargo Correspondences
    /// Will be called from service
    /// </summary>
    /// <param name="message"></param>
    /// <param name="basePath"></param>
    public string CreateCgoCorrespondenceTrailPdf(ReportDownloadRequestMessage message, string basePath)
    {
      Logger.Info("Recieved Correspondence Trail Report Request ");
      string requestingMemberNumericCode = MemberManager.GetMember(message.RequestingMemberId).MemberCodeNumeric;
      string reportZipFolderName = string.Format("CCORR-{0}-{1}", requestingMemberNumericCode, message.RecordId);
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
        var correspondences = new List<CargoCorrespondence>();
        var fromReportModule = new List<Contact>();
        var toReportModule = new List<Contact>();
        string otherMemberNumericCode = string.Empty;

        var fromContacts = new List<CorrespondenceReportContact>();
        var toContacts = new List<CorrespondenceReportContact>();

        var corrs = CargoCorrespondenceRepository.GetCorrespondenceForTraiReport(corr => corr.CorrespondenceNumber == correspondenceNumber);
        if (corrs == null || corrs.Count() == 0)
        {
          Logger.InfoFormat("No Correspondences found for correspondence number {0} ", correspondenceNumber);
          continue;
        }
        //correspondences.AddRange(corrs.OrderBy(corr => corr.CorrespondenceStage));
				//SCP241018 - SIS: Download Correspondence- SIS Production is different with Billing History and Correspondence.
				//Those correspondence should not display in the pdf which has status "Ready For Submit" or "Saved" by billed member.
				correspondences = (from corr in corrs
													 where (corr.ToMemberId != message.RequestingMemberId ||
																	!correspondenceSubStatus.Contains(corr.CorrespondenceSubStatusId))
													 orderby corr.CorrespondenceStage
													 select corr).ToList();
        Logger.InfoFormat("Number of Correspondences for correspondence Number  {0} : {1}  ", correspondenceNumber, correspondences.Count);

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

            fromReportModule.Add(GetContactDetails(correspondences[correspondencesIndex].FromMemberId, ProcessingContactType.CargoCorrespondence));

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
          toReportModule.Add(GetContactDetails(correspondences[correspondencesIndex].ToMemberId, ProcessingContactType.CargoCorrespondence));

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

          //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
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
        string reportPdfPath = correspondanceTrailReportManager.CreateCorrespondenceTrailPdf(FileIo.GetForlderPath(SFRFolderPath.ISCorrRepFolder), correspondences, correspondenceNumber, fromContacts, toContacts, message.RequestingMemberId, otherMemberNumericCode);
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
    public CargoCorrespondence GetOnlyCorrespondenceUsingLoadStrategy(string correspondenceId)
    {
        var correspondenceGuid = correspondenceId.ToGuid();
        var correspondenceHeader = CargoCorrespondenceRepository.GetOnlyCorrespondenceUsingLoadStrategy(correspondenceGuid);
        return correspondenceHeader;
    }

    /// <summary>
    /// Gets a last responded correspondence from database. Useful for Close functionality in order to send emails 
    /// to all email Ids in previously responded state.
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    public CargoCorrespondence GetLastRespondedCorrespondene(long correspondenceNumber)
    {
        var correspondence =
            CargoCorrespondenceRepository.GetLastRespondedCorrespondene(corr => corr.CorrespondenceNumber == correspondenceNumber &&
                    corr.CorrespondenceSubStatusId == 2).OrderByDescending(corr => corr.CorrespondenceStage).FirstOrDefault();

        return correspondence;
    }
  }
}

