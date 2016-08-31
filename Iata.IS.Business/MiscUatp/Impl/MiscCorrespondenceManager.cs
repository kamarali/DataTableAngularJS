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
using Iata.IS.Business.Reports.Common;
using Iata.IS.Business.Reports.MiscUatp;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using log4net;
using log4net.Repository.Hierarchy;
using NVelocity;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Data.MiscUatp.Impl;
using Iata.IS.Model.MiscUatp.Common;
namespace Iata.IS.Business.MiscUatp.Impl
{
  public class MiscCorrespondenceManager : CorrespondenceManager,IMiscCorrespondenceManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Gets or sets the Misc correspondence repository.
    /// </summary>
    /// <value>The country repository.</value>
    public IMiscCorrespondenceRepository MiscCorrespondenceRepository { get; set; }

    /// <summary>
    /// Gets or sets the Misc correspondence Attachment repository.
    /// </summary>
    /// <value>The country repository.</value>
    public IMiscCorrespondenceAttachmentRepository MiscCorrespondenceAttachmentRepository { get; set; }

      /// <summary>
      /// Gets or sets the charge category repository.
      /// </summary>
     /// <value>The charge category repository.</value>
    public IRepository<ChargeCategory> ChargeCategoryRepository { get; set; }

    /// <summary>
    /// Gets or sets the invoice summary repository.
    /// </summary>
    /// <value>The invoice summary repository.</value>
    public IRepository<InvoiceSummary> InvoiceSummaryRepository { get; set; }

    /// <summary>
    /// Gets or sets the Misc correspondence Attachment repository.
    /// </summary>
    /// <value>The country repository.</value>
    public IMiscInvoiceRepository MiscInvoiceRepository { get; set; }

    /// <summary>
    /// Gets or sets the line item repository.
    /// </summary>
    /// <value>The line item repository.</value>
    public ILineItemRepository LineItemRepository { get; set; }

    public IMiscInvoiceManager MiscInvoiceManager { get; set; }


    /// <summary>
    /// Gets or sets the reference manager.
    /// </summary>
    /// <value>The reference manager.</value>
    public IReferenceManager ReferenceManager { get; set; }

    public ICalendarManager CalendarManager { get; set; }

    // SCP210204: IS-WEB Outage [To resolve null reference]
    public MiscCorrespondenceManager(IReferenceManager referenceManager) : base(referenceManager)
    {
    }

    /// <summary>
    /// Creates the Misc correspondence.
    /// </summary>
    /// <param name="miscCorrespondence">The misc correspondence.</param>
    /// <param name="billingCategory">The billing category.</param>
    /// <returns></returns>
    public MiscCorrespondence AddCorrespondence(MiscCorrespondence miscCorrespondence, BillingCategoryType billingCategory)
    {
      if (ValidateCorrespondence(miscCorrespondence))
      {

        // SCP109163
        // Check if correspondence expiry date is crossed.
        if (billingCategory == BillingCategoryType.Misc && miscCorrespondence.ExpiryDate < DateTime.UtcNow.Date)
        {
          throw new ISBusinessException(MiscErrorCodes.ExpiredCorrespondence);
        }

        if (billingCategory == BillingCategoryType.Uatp && miscCorrespondence.ExpiryDate < DateTime.UtcNow.Date)
        {
          throw new ISBusinessException(UatpErrorCodes.ExpiredCorrespondence);
        }
        
        // Mark the correspondence status as Open.
        miscCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;

        // Validation for misc. correspondence.

        MiscCorrespondenceRepository.Add(miscCorrespondence);
        //Added the following for look to increment the correspondence ref number if already present in database.
        //TODO: need to change the count to 3 after UAT
        for (var tryCount = 0; tryCount < 25; tryCount++)
        {
          try
          {
            // Check if correspondence has already been created for the selected invoice. If yes, display error message.
            if(miscCorrespondence.CorrespondenceStage == 1 && MiscCorrespondenceRepository.First(corr => corr.InvoiceId == miscCorrespondence.InvoiceId) != null)
            {
              if(billingCategory == BillingCategoryType.Misc)
                throw new ISBusinessException(new MiscErrorCodes().ErrorCorrespondenceAlreadyCreated);
              
              throw new ISBusinessException(new UatpErrorCodes().ErrorCorrespondenceAlreadyCreated);
            }

            UnitOfWork.CommitDefault();
            tryCount = 25;
          }
          catch (Exception exception)
          {
            Logger.Error("Exception in MU AddCorrespondence Method.");

            Logger.ErrorFormat(
              "Exception Message: {0}, Inner Exception Message: {1}, Stack Trace: {2},  corr Ref No: {3}, stage: {4}, from: {5}, to: {6}, Amount: {7}, Subject: {8}, curresncy Code: {9}, corr details: {10}, status: {11}, sub-status: {12}",
              exception.Message, exception.InnerException.Message, exception.StackTrace,
              miscCorrespondence.CorrespondenceNumber, miscCorrespondence.CorrespondenceStage,
              miscCorrespondence.FromMemberId, miscCorrespondence.ToMemberId, miscCorrespondence.AmountToBeSettled,
              miscCorrespondence.Subject, miscCorrespondence.CurrencyId, miscCorrespondence.CorrespondenceDetails,
              miscCorrespondence.CorrespondenceStatusId, miscCorrespondence.CorrespondenceSubStatusId);

            Logger.Error(exception);

            if (exception.InnerException != null)
              Logger.Error(exception.InnerException);

            // Case when correspondence is being created by same member for two different transactions at the same time.
            // Check if exception has occurred due to duplicate correspondence ref. no. and stage. If yes, then increment the correspondence reference number.
            if (!(exception is ISBusinessException))
            {
              if (tryCount == 24 || miscCorrespondence.CorrespondenceStage > 1)
              {
                throw new ISBusinessException(ErrorCodes.InvalidCorrespondencRefNo);
              }
              if ( miscCorrespondence.CorrespondenceStage == 1)
              miscCorrespondence.CorrespondenceNumber++;
            }
            else
            {
              // throw the business exception - correspondence has already been created for the selected invoice.
              throw;
            }
          }
        }
        //SCP0000: PURGING AND SET EXPIRY DATE (REMOVE SET EXPIRY DATE FROM MEMOS AND CORR LEVEL)
        //UpdatePurgingExpiryPeriod(miscCorrespondence);
      }

      return miscCorrespondence;
    }
    //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
    //private void UpdatePurgingExpiryPeriod(MiscCorrespondence muCorrespondence)
    //{
    //  var invoiceBase = muCorrespondence.Invoice ?? MiscInvoiceRepository.Single(muCorrespondence.InvoiceId);

    //  TransactionType otherCorrespondenceTransactionType = invoiceBase.BillingCategory == BillingCategoryType.Misc ? TransactionType.MiscOtherCorrespondence : TransactionType.UatpOtherCorrespondence;
    //  if (muCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open && muCorrespondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded)
    //  {
    //    TransactionType transactionType;
        
    //    if (muCorrespondence.AuthorityToBill)
    //      transactionType = invoiceBase.BillingCategory == BillingCategoryType.Misc ? TransactionType.MiscCorrInvoiceDueToAuthorityToBill : TransactionType.UatpCorrInvoiceDueToAuthorityToBill;
    //    else
    //      transactionType = otherCorrespondenceTransactionType;

    //    DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(transactionType, invoiceBase, invoiceBase.BillingCategory, Constants.SamplingIndicatorNo, null, true, muCorrespondence);
    //    MiscInvoiceRepository.UpdateExpiryDatePeriod(muCorrespondence.Id, (int)otherCorrespondenceTransactionType, expiryPeriod);
    //  }
    //}

    /// <summary>
    /// Updates the Misc correspondence.
    /// </summary>
    /// <param name="miscCorrespondence">The misc invoice.</param>
    /// <returns></returns>
    public MiscCorrespondence UpdateCorrespondence(MiscCorrespondence miscCorrespondence)
    {

      if (ValidateCorrespondence(miscCorrespondence))
      {
        //var miscUatpInvoiceInDb = MiscCorrespondenceRepository.Single(correspondence => correspondence.Id == miscCorrespondence.Id);
        // Call replaced by Load Strategy
        var miscUatpInvoiceInDb = MiscCorrespondenceRepository.Single(correspondenceId: miscCorrespondence.Id);
        //ValidateInvoice(miscUatpInvoice, miscUatpInvoiceInDb);

        miscCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;
        var updatedCorrespondenceData = MiscCorrespondenceRepository.Update(miscCorrespondence);

        // Changes to update attachment breakdown records.
        var listToDeleteAttachment = miscUatpInvoiceInDb.Attachments.Where(attachment => miscCorrespondence.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

        var attachmentIdList = (from attachment in miscCorrespondence.Attachments
                                where miscUatpInvoiceInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                                select attachment.Id).ToList();

        var rmCouponAttachmentInDb = MiscCorrespondenceAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));
        foreach (var recordAttachment in rmCouponAttachmentInDb)
        {
          if (IsDuplicateCorrespondenceAttachmentFileName(recordAttachment.OriginalFileName, miscCorrespondence.Id))
          {
            throw new ISBusinessException(ErrorCodes.DuplicateFileName);
          }

          recordAttachment.ParentId = miscCorrespondence.Id;
          MiscCorrespondenceAttachmentRepository.Update(recordAttachment);
        }
        foreach (var rmCouponRecordAttachment in listToDeleteAttachment)
        {
          MiscCorrespondenceAttachmentRepository.Delete(rmCouponRecordAttachment);
        }

        UnitOfWork.CommitDefault();
        //SCP0000: PURGING AND SET EXPIRY DATE (REMOVE SET EXPIRY DATE FROM MEMOS AND CORR LEVEL)
        //UpdatePurgingExpiryPeriod(miscCorrespondence);

        return updatedCorrespondenceData;
      }

      return miscCorrespondence;
    }

   /// <summary>
   /// Creates the correspondence format report in PDF format.
   /// </summary>
   /// <param name="correspondenceId"></param>
   /// <param name="processingContactType"></param>
   /// <returns></returns>
    public string CreateCorrespondenceFormatPdf(string correspondenceId, ProcessingContactType processingContactType)
   {
      //CMP527: Add new variable to show acceptance comments on PDF.
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

      var miscUatpReportManager = Ioc.Resolve<IMiscUatpReportManager>();

      var fromContacts = GetContactDetails(correspondenceDetails.FromMemberId, processingContactType);

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

      var toContacts = GetContactDetails(correspondenceDetails.ToMemberId, processingContactType);


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
      if (fromContact != null)
      {
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
        //If Country is not populated, populate it explicitly for given id
        if (fromContact.Country == null)
        {
          var countryList = ReferenceManager.GetCountryList();
          country = (from c in countryList
                     where c.Id == fromContact.CountryId
                     select c).FirstOrDefault();
          fromContact.Country = country;
        }
      }

      //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
      if (toContact != null)
      {
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
        if (toContact.Country == null)
        {
          var countryList = ReferenceManager.GetCountryList();
          country = (from c in countryList
                     where c.Id == toContact.CountryId
                     select c).FirstOrDefault();
          toContact.Country = country;
        }

        miscUatpReportManager.BuildCorrespondenceFormatReport(correspondenceDetails, ref correspondenceFormatReportPath, fromContact, toContact);
      }

      return correspondenceFormatReportPath;
    }

    public Contact GetContactDetails(int memberId, ProcessingContactType processingContact)
    {
      var contactTypeList = MemberManager.GetContactsForContactType(memberId, processingContact);
      var contact = new Contact();
      if (contactTypeList != null && contactTypeList.Count > 0)
      {
        contact = contactTypeList.FirstOrDefault();
      }
      return contact;
    }


    /// <summary>
    /// Check for duplicate file name of correspondence attachment
    /// </summary>
    /// <param name="fileName">file name</param>
    /// <param name="miscCorrespondenceId">correspondence Id</param>
    /// <returns></returns>
    public bool IsDuplicateCorrespondenceAttachmentFileName(string fileName, Guid miscCorrespondenceId)
    {
      return MiscCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == miscCorrespondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Gets the numeric member code.
    /// </summary>
    /// <param name="memberCode">The member code.</param>
    /// <returns></returns>
    /// CMP#596: converted int to long to support membernumeric code upto 12 digits
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

    /// <summary>
    /// Deletes the Misc correspondence.
    /// </summary>
    /// <param name="correspondenceId">The Misc correspondence id.</param>
    /// <returns></returns>
    public bool DeleteCorrespondence(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      //var correspondenceToBeDeleted = MiscCorrespondenceRepository.Single(correspondence => correspondence.Id == correspondenceGuid);
      // Call replaced by Load Strategy
      var correspondenceToBeDeleted = MiscCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
      if (correspondenceToBeDeleted == null) return false;
      MiscCorrespondenceRepository.Delete(correspondenceToBeDeleted);
      UnitOfWork.CommitDefault();
      return true;
    }

    /// <summary>
    /// Determines whether transaction exists for the specified correspondence id
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns>
    /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
    /// </returns>
    public bool IsTransactionExists(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      var isTransactionExists = (MiscCorrespondenceRepository.GetCount(correspondence => correspondence.Id == correspondenceGuid) > 0);

      return isTransactionExists;
    }

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceId">correspondence id To Be fetched..</param>
    /// <returns></returns>
    public MiscCorrespondence GetCorrespondenceDetails(string correspondenceId)
    {
      var correspondenceGuid = correspondenceId.ToGuid();
      // Call replaced by Load Strategy
      var correspondenceHeader = MiscCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
      return correspondenceHeader;
    }

    /// <summary>
    /// Function to retrieve Last correspondence details of the given correspondence No
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    public MiscCorrespondence GetCorrespondenceDetails(long? correspondenceNumber)
    {

      // Call replaced by Load Strategy
      //INC 8863, I get an unexpected error occurred.
      var corrrespondence = MiscCorrespondenceRepository.GetCorr(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();

      return corrrespondence;
    }

    /// <summary>
    /// Function to check validate correspondence based on correspondence reference for create correspondence invoice.
    /// </summary>
    /// <param name="correspondenceNumber">Validate for given correspondence reference number</param>
    /// <param name="billingCategory">Used to decide error message to be return</param>
    /// <returns></returns>
    // SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    public void ValidateCorrespondenceReference(long? correspondenceNumber, BillingCategoryType billingCategory)
    {
        var miscCorrespondence = MiscCorrespondenceRepository.GetCorr(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber &&
            (correspondence.CorrespondenceStatusId == (int)CorrespondenceStatus.Open ||
              correspondence.CorrespondenceStatusId == (int)CorrespondenceStatus.Expired ||
              correspondence.CorrespondenceStatusId == (int)CorrespondenceStatus.Closed)).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();

        if (miscCorrespondence == null)
        {
          //SCP226313: ERROR MESSAGES
          if (billingCategory == BillingCategoryType.Misc)
            throw new ISBusinessException(new MiscErrorCodes().InvoiceNotExistForCorrespondence);
          throw new ISBusinessException(new UatpErrorCodes().InvoiceNotExistForCorrespondence);
        }
        switch (miscCorrespondence.CorrespondenceStatus)
        {
            case CorrespondenceStatus.Closed:
                if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
                    throw new ISBusinessException(new UatpErrorCodes().CorrRefNoClosed);
                throw new ISBusinessException(new MiscErrorCodes().CorrRefNoClosed);
           case CorrespondenceStatus.Open:
                if (!miscCorrespondence.AuthorityToBill)
                {
                    if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
                        throw new ISBusinessException(new UatpErrorCodes().InvalidCorrespondenceStatusAuthorityToBill);
                    throw new ISBusinessException(new MiscErrorCodes().InvalidCorrespondenceStatusAuthorityToBill);
                }
                break;
        }
    }

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id for authority to bill
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    public MiscCorrespondence GetCorrespondenceForAuthorityToBillDetails(long? correspondenceNumber)
    {
      //var corrrespondence = MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber && correspondence.CorrespondenceStatusId == 1 && correspondence.AuthorityToBill).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      //// Call replaced by Load Strategy
      var corrrespondence =
        MiscCorrespondenceRepository.Get(correspondenceNumber: correspondenceNumber, correspondenceStatusId: 1, authorityToBill: true).OrderByDescending(
          correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return corrrespondence;
    }

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id - If billing member is To Member of the correspondence then he should not allow to view the Saved and Ready to Submit correspondences of the other member.
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    public MiscCorrespondence GetRecentCorrespondenceDetails(string invoiceId, int billingMemberId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var corrrespondence = MiscCorrespondenceRepository.GetCorr(correspondence => correspondence.InvoiceId == invoiceGuid
        && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
        && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
        && correspondence.CorrespondenceSubStatusId == 2)))
        )).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return corrrespondence;
    }

    /// <summary>
    /// Function to retrieve correspondence details of the given correspondence id
    /// </summary>
    /// <param name="correspondenceNumber"></param>
    /// <returns></returns>
    public MiscCorrespondence GetOriginalCorrespondenceDetails(long? correspondenceNumber)
    {
      //var corrrespondence = MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderBy(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      // Call replaced by Load Strategy
      var corrrespondence = MiscCorrespondenceRepository.Get(correspondenceNumber: correspondenceNumber).OrderBy(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
      return corrrespondence;
    }


    /// <summary>
    /// Retrieve recent Correspondence Id for provided invoice id. 
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public MiscCorrespondence GetRecentCorrespondenceDetails(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      //var correspondence = MiscCorrespondenceRepository.Get(corr => corr.InvoiceId == invoiceGuid).OrderByDescending(c => c.CorrespondenceStage).FirstOrDefault();
      // Call replace by Load strategy
      var correspondence = MiscCorrespondenceRepository.Get(invoiceId: invoiceGuid).OrderByDescending(c => c.CorrespondenceStage).FirstOrDefault();
      return correspondence;
    }

    public MiscCorrespondence GetFirstCorrespondenceDetails(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      
        // Call replaced by load strategy
        var correspondence = MiscCorrespondenceRepository.Get(invoiceId: invoiceGuid).OrderBy(c => c.CorrespondenceStage).FirstOrDefault();
        return correspondence;
    }

    /// <summary>
    /// Function to retrieve correspondence number of the given member id
    /// </summary>
    /// <param name="memberId">Member id .</param>
    /// <returns></returns>
    public long GetCorrespondenceNumber(int memberId)
    {
      //var correspondence = MiscCorrespondenceRepository.Get(c => c.FromMemberId == memberId).OrderByDescending(c => c.CorrespondenceNumber).FirstOrDefault();
      // Call replaced by load strategy
      var correspondence = MiscCorrespondenceRepository.Get(fromMemberId: memberId, correspondenceStage: 1).OrderByDescending(c => c.CorrespondenceNumber).FirstOrDefault();
      long correspondNumber = 0;

      if (correspondence != null && correspondence.CorrespondenceNumber != null)
        correspondNumber = correspondence.CorrespondenceNumber.Value;

      return correspondNumber;
    }

    public long GetInitialCorrespondenceNumber(int memberId)
    {
        var correspondenceNumber = MiscCorrespondenceRepository.GetInitialCorrespondenceNumber(memberId);

        return correspondenceNumber;

    }


    /// <summary>
    /// Function to retrieve correspondence number of the given member id
    /// </summary>
    /// <param name="memberId">Member id .</param>
    /// <returns></returns>
    public bool IsFirstCorrespondence(int memberId)
    {
      //var correspondence = MiscCorrespondenceRepository.Get(c => c.FromMemberId == memberId && c.CorrespondenceStage == 1).FirstOrDefault();
      // Call replaced by Load Strategy
      var correspondence = MiscCorrespondenceRepository.Get(fromMemberId: memberId, correspondenceStage: 1).FirstOrDefault();
      if (correspondence == null)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Function to retrieve invoice details of the given correspondence id
    /// </summary>
    /// <param name="invoiceId">invoice id To Be fetched..</param>
    /// <returns></returns>
    public MiscUatpInvoice GetInvoiceDetail(string invoiceId)
    {
      return MiscInvoiceManager.GetInvoiceDetail(invoiceId);
    }

    /// <summary>
    /// Function to retrieve invoice details of the given correspondence id
    /// SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
    /// </summary>
    /// <param name="invoiceId">invoice id To Be fetched..</param>
    /// <returns></returns>
    public MiscUatpInvoice GetInvoiceHeaderDetail(string invoiceId)
    {
        var id = invoiceId.ToGuid();
        var invoice=MiscInvoiceRepository.Get(i=>i.Id==id).SingleOrDefault();

        //Retrieve the charge category details
       invoice.ChargeCategory=ChargeCategoryRepository.Get(i => i.Id == invoice.ChargeCategoryId).SingleOrDefault();

        //Get the invoiceSummary details
       invoice.InvoiceSummary = InvoiceSummaryRepository.Get(i => i.InvoiceId == invoice.Id).SingleOrDefault();

        return invoice;
    }
   
    /// <summary>
    /// Gets the Misc Correspondence attachments.
    /// </summary>
    /// <param name="attachmentIds">The attachment ids.</param>
    /// <returns></returns>
    public List<MiscUatpCorrespondenceAttachment> GetMiscCorrespondenceAttachments(List<Guid> attachmentIds)
    {
      return MiscCorrespondenceAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
    }


    /// <summary>
    /// Add Misc Correspondence Attachment record
    /// </summary>
    /// <param name="attach">Misc Correspondence Attachment record</param>
    /// <returns></returns>
    public MiscUatpCorrespondenceAttachment AddRejectionMemoAttachment(MiscUatpCorrespondenceAttachment attach)
    {
      MiscCorrespondenceAttachmentRepository.Add(attach);

      UnitOfWork.CommitDefault();

      return attach;
    }

    /// <summary>
    /// Updates the Misc Correspondence attachment.
    /// </summary>
    /// <param name="attachments">The attachments.</param>
    /// <param name="parentId">The parent id.</param>
    /// <returns></returns>
    public IList<MiscUatpCorrespondenceAttachment> UpdateMiscCorrespondenceAttachment(IList<Guid> attachments, Guid parentId)
    {
      var attachmentInDb = MiscCorrespondenceAttachmentRepository.Get(miscCorrespondence => attachments.Contains(miscCorrespondence.Id));
      foreach (var recordAttachment in attachmentInDb)
      {
        recordAttachment.ParentId = parentId;
        MiscCorrespondenceAttachmentRepository.Update(recordAttachment);
      }
      UnitOfWork.CommitDefault();
      return attachmentInDb.ToList();
    }

    /// <summary>
    /// Gets the Misc Correspondence attachment detail.
    /// </summary>
    /// <param name="attachmentId">The attachment id.</param>
    /// <returns></returns>
    public MiscUatpCorrespondenceAttachment GetMiscCorrespondenceAttachmentDetail(string attachmentId)
    {
      var attachmentGuid = attachmentId.ToGuid();
      var attachmentRecord = MiscCorrespondenceAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

      return attachmentRecord;
    }

    /// <summary>
    /// Determines whether specified file name already exists for given invoice.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns>
    /// true if specified file name found in repository; otherwise, false.
    /// </returns>
    public bool IsDuplicateMiscCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId)
    {
      return MiscCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == correspondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
    }

    /// <summary>
    /// Retrieve Correspondence History List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public IList<MiscCorrespondence> GetCorrespondenceHistoryList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      //var correspondenceHistoryList = MiscCorrespondenceRepository.Get(corr => corr.InvoiceId == invoiceGuid).ToList();
      // Call replaced by Load Strategy
      var correspondenceHistoryList = MiscCorrespondenceRepository.Get(invoiceId: invoiceGuid).ToList();
      return correspondenceHistoryList;
    }

    /// <summary>
    /// Retrieve Correspondence History List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <param name="billingMemberId"></param>
    /// <returns></returns>
    public IList<MiscCorrespondence> GetCorrespondenceHistoryList(string invoiceId, int billingMemberId)
    {
      var invoiceGuid = invoiceId.ToGuid();

      var correspondenceHistoryList = MiscCorrespondenceRepository.GetCorr(correspondence => correspondence.InvoiceId == invoiceGuid
        && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
        && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
        && correspondence.CorrespondenceSubStatusId == 2)))
        )).ToList();

      return correspondenceHistoryList;
    }

    /// <summary>
    /// Retrieve Correspondence Rejection List
    /// </summary>
    /// <param name="invoiceId"></param>
    /// <returns></returns>
    public IList<MiscUatpInvoice> GetCorrespondenceRejectionList(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();

      var invoiceList = MiscInvoiceRepository.Get(inv => inv.Id == invoiceGuid).ToList();

      return invoiceList;
    }

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="miscCorrespondence"></param>
    /// <returns></returns>
    public bool ValidateCorrespondence(MiscCorrespondence miscCorrespondence)
    {

      // Validation for CityAirport as it is auto complete field in UI
      var toEmailId = miscCorrespondence.ToEmailId != null ? miscCorrespondence.ToEmailId.Trim() : miscCorrespondence.ToEmailId;
      /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids from initiator and non-initiator and removing
                 additional email field*/
      var additionalEmailInitiator = miscCorrespondence.AdditionalEmailInitiator != null
                                         ? miscCorrespondence.AdditionalEmailInitiator.Trim()
                                         : miscCorrespondence.AdditionalEmailInitiator;
      var additionalEmailNonInitiator = miscCorrespondence.AdditionalEmailNonInitiator != null
                                            ? miscCorrespondence.AdditionalEmailNonInitiator.Trim()
                                            : miscCorrespondence.AdditionalEmailNonInitiator;
      if (string.IsNullOrEmpty(toEmailId + additionalEmailInitiator + additionalEmailNonInitiator))
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.EnterEmailIds);
        throw new ISBusinessException(MiscErrorCodes.EnterEmailIds);
      }

      // Validation of Correspondence Subject
      if (string.IsNullOrEmpty(miscCorrespondence.Subject))
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.InvalidCorrespondenceSubject);
        throw new ISBusinessException(MiscErrorCodes.InvalidCorrespondenceSubject);
      }

      // Validation for CityAirport as it is auto complete field in UI
      if (miscCorrespondence.AmountToBeSettled < 0)
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.InvalidAmountToBeSettled);
        throw new ISBusinessException(MiscErrorCodes.InvalidAmountToBeSettled);
      }

      // Validation for CityAirport as it is auto complete field in UI
      if (string.IsNullOrEmpty(miscCorrespondence.CorrespondenceNumber.ToString()))
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.InvalidCorrespondenceNumber);
        throw new ISBusinessException(MiscErrorCodes.InvalidCorrespondenceNumber);
      }

      // Validation for CityAirport as it is auto complete field in UI
      if (miscCorrespondence.CurrencyId == 0)
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.InvalidCorrespondenceNumber);
        throw new ISBusinessException(MiscErrorCodes.InvalidCorrespondenceNumber);
      }
      var invoice = GetInvoiceDetail(miscCorrespondence.InvoiceId.ToString());

      if (invoice != null)
      {
        if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscCorrespondence.AmountToBeSettled),
          invoice.BillingCategory == BillingCategoryType.Misc ? miscCorrespondence.CorrespondenceStage == 1 ? TransactionType.MiscCorrespondence : TransactionType.MiscOtherCorrespondence : miscCorrespondence.CorrespondenceStage == 1 ? TransactionType.UatpCorrespondence : TransactionType.UatpOtherCorrespondence, miscCorrespondence.CurrencyId, invoice, applicableMinimumField: ApplicableMinimumField.AmountToBeSettled, isCorrespondence: true, correspondence: miscCorrespondence, validateMaxAmount: false))
        {
          throw new ISBusinessException(ErrorCodes.CorrespondenceAmountIsNotInAllowedRange);
        }
      }

      //You cannot send a correspondence if it is expired
      if ((miscCorrespondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded))
      {
        var isOutSideTimeLimit = false;
        DateTime previousTransactionDate;
        if (miscCorrespondence.CorrespondenceStage == 1)
        {
          previousTransactionDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
        }
        else
        {

          // Get correspondence of previous stage.
          MiscCorrespondence previousCorrespondence = MiscCorrespondenceRepository.Single(null,
                                                                           miscCorrespondence.CorrespondenceNumber,
                                                                           miscCorrespondence.CorrespondenceStage - 1);
          previousTransactionDate = previousCorrespondence.CorrespondenceDate;
        }

        if (invoice.InvoiceSmi != SMI.Ach)
        {
          isOutSideTimeLimit =
              !ReferenceManager.IsTransactionInTimeLimitMethodC(TransactionType.MiscOtherCorrespondence,
                                                                invoice.SettlementMethodId,
                                                                miscCorrespondence.CorrespondenceDate,
                                                                previousTransactionDate);
        }
        else
        {
          isOutSideTimeLimit =
              !ReferenceManager.IsTransactionInTimeLimitMethodG(TransactionType.MiscOtherCorrespondence,
                                                                invoice.SettlementMethodId,
                                                                miscCorrespondence.CorrespondenceDate,
                                                                previousTransactionDate);
        }

        if (isOutSideTimeLimit)
        {
          if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
            throw new ISBusinessException(UatpErrorCodes.ExpiredCorrespondence);
          throw new ISBusinessException(MiscErrorCodes.ExpiredCorrespondence);
        }
      }

      if (!HasValidAuthorityToBill(miscCorrespondence))
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.InvalidAuthorityToBill);
        throw new ISBusinessException(MiscErrorCodes.InvalidAuthorityToBill);
      }

      /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids of To,Initiator and Non-Initiator */
        var toEmailIds = GetEmailIdsList(miscCorrespondence.ToEmailId,
                                         miscCorrespondence.AdditionalEmailInitiator,
                                         miscCorrespondence.AdditionalEmailNonInitiator);

      if (ValidateToEmailIds(toEmailIds) == false)
      {
        if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
          throw new ISBusinessException(UatpErrorCodes.InvalidEmailIds);
        throw new ISBusinessException(MiscErrorCodes.InvalidEmailIds);
      }

      //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202].
      MiscUatpInvoice miscInvoice = IsCorrespondenceInvoiceExistsForCorrespondence(miscCorrespondence.FromMemberId, miscCorrespondence.CorrespondenceNumber.Value);

      if (miscInvoice != null)
      {
          if (miscCorrespondence.Invoice != null &&
              miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
              throw new ISBusinessException(GetErrorDescription(UatpErrorCodes.CanNotReplyToUatpCorrespondence, miscInvoice.InvoiceNumber, new DateTime(miscInvoice.BillingYear, miscInvoice.BillingMonth, miscInvoice.BillingPeriod).ToString("yyyy-MMM-dd")));
          throw new ISBusinessException(GetErrorDescription(MiscErrorCodes.CanNotReplyToMiscCorrespondence, miscInvoice.InvoiceNumber, new DateTime(miscInvoice.BillingYear, miscInvoice.BillingMonth, miscInvoice.BillingPeriod).ToString("yyyy-MMM-dd")));
      }

        // Updates correspondence expiry date.
      //if (miscCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Saved && miscCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.ReadyForSubmit)
      //{
          UpdateExpiryDate(miscCorrespondence, invoice);
      //}

         

      return true;
    }



    private bool HasValidAuthorityToBill(MiscCorrespondence miscCorrespondence)
    {
      if (miscCorrespondence.AuthorityToBill)
      {
        var correspondence = GetOriginalCorrespondenceDetails(miscCorrespondence.CorrespondenceNumber);
        if (correspondence != null)
        {
          if (correspondence.FromMemberId == miscCorrespondence.FromMemberId)
          {
            return false;
          }
        }
      }

      return true;
    }

    /// <summary>
    /// Validates the correspondence.
    /// </summary>
    /// <param name="toEmailId"></param>
    /// <returns></returns>
    public bool ValidateToEmailIds(string toEmailId)
    {

      if (toEmailId == null)
      {
        return false;
      }
      toEmailId = toEmailId.Replace("\r", string.Empty).Replace("\n", string.Empty);
      string[] eMailIds = toEmailId.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

      if (eMailIds.All(emailId => string.IsNullOrEmpty(emailId.Trim()))) return false;

      if (eMailIds.Any(eMailId => IsValidEmailId(eMailId.Trim()) == false))
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Get the member of provided member Id .
    /// </summary>
    /// <param name="memberId">The Member Id.</param>
    /// <returns></returns>
    public Member GetMember(int memberId)
    {
      return MemberManager.GetMember(memberId);
    }

    /// <summary>
    /// Get the member of provided member Id .
    /// </summary>
    /// <param name="memberId">The Member Id.</param>
    /// <param name="processingContact"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get the charge codes for provided invoice Id .
    /// </summary>
    /// <param name="invoiceId">The invoice Id.</param>
    /// <returns></returns>
    public string GetChargeCodes(string invoiceId)
    {
      var invoiceGuid = invoiceId.ToGuid();
      var lineItemList = LineItemRepository.Get(invoiceGuid).ToList();
      var chargeCodes = string.Empty;

      if (lineItemList.Count > 1)
      {
        chargeCodes = lineItemList.Aggregate(chargeCodes, (current, li) => String.Format("{0},{1}", current, li.DisplayChargeCode));
      }
      else if (lineItemList.Count == 1)
      {
        chargeCodes = lineItemList[0].DisplayChargeCode;
      }

      return chargeCodes;
    }

    /// <summary>
    /// Gets the error description.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="mailId">The mail id.</param>
    /// <returns></returns>
    private static string GetErrorDescription(string errorCode, string mailId)
    {
      var errorDescription = Messages.ResourceManager.GetString(errorCode);

      // Replace place holders in error message with appropriate record names.
      if (!string.IsNullOrEmpty(errorDescription))
        errorDescription = string.Format(errorDescription, mailId);
      return errorDescription;
    }

    /// <summary>
    /// Gets the error description with any number of argument.
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
    private static string GetErrorDescription(string errorCode, params String[] values)
    {
        var errorDescription = Messages.ResourceManager.GetString(errorCode);

        // Replace place holders in error message with appropriate values.
        if (!string.IsNullOrEmpty(errorDescription))
            errorDescription = String.Format(errorDescription, values);

        return String.Format("{0} - {1}", errorCode, errorDescription);
    }
    
    // CMP#657: Retention of Additional Email Addressed in Correspondences.
    ///// <summary>
    ///// Function to send correspondence email
    ///// </summary>
    ///// <param name="correspondPageUrl">Correspond Page Url.</param>
    ///// <param name="toEmailIds">To email id's</param>
    ///// <param name="subject">Email subject</param>
    ///// <returns> bool </returns>
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
    //    if (eMailId != null && !string.IsNullOrEmpty(eMailId.Trim()))
    //    {
    //      var mailMessage = new MailMessage(emailSettingForCorrespondence.SingleOrDefault().FromEmailAddress, eMailId.Trim(), subject, messageBody) { IsBodyHtml = true };
    //      try
    //      {
    //        emailSender.Send(mailMessage);
    //      }
    //      catch (Exception)
    //      {
    //        //if (miscCorrespondence.Invoice != null && miscCorrespondence.Invoice.BillingCategory.Equals(BillingCategoryType.Uatp))
    //        //    throw new ISBusinessException(UatpErrorCodes.FailedToSendMail);
    //        throw new ISBusinessException(GetErrorDescription(MiscErrorCodes.FailedToSendMail, eMailId));
    //      }
    //    }
    //  }

    //  return true;
    //}

    public List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold)
    {
      return MiscCorrespondenceRepository.UpdateCorrespondenceStatus(billingCategoryType, billingPeriod, _oornThreshold, _oernThreshold, _eornThreshold, _eoryThreshold, _eorybThreshold);
    }

    /// <summary>
    /// Returns true if billing memo created with given correspondence reference number.
    /// SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202].
    /// DESC: Change in return parameter.
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="correspondenceRefNumber"></param>
    /// <returns></returns>
    public MiscUatpInvoice IsCorrespondenceInvoiceExistsForCorrespondence(int billingMemberId, long correspondenceRefNumber)
    {
        var miscInvoiceList = MiscInvoiceRepository.Get(
          invoice =>
          invoice.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice &&
          invoice.CorrespondenceRefNo == correspondenceRefNumber && invoice.BillingMemberId == billingMemberId).ToList();
        return miscInvoiceList.Count > 0 ? miscInvoiceList[0] : null;

        //return MiscInvoiceRepository.GetCount(
        //  invoice => invoice.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice && invoice.CorrespondenceRefNo == correspondenceRefNumber && invoice.BillingMemberId == billingMemberId) > 0;
    }

    //SCP0000:Impact on MISC/UATP rejection linking due to purging
    /// <summary>
    /// Validates, user can create correspondence or not if rejection invoice is out side time limit.
    /// </summary>
    /// <param name="invoiceId">The invoice id.</param>
    /// <param name="invHeader">The invoice Header.</param>
    /// <returns>
    /// true if [is valid correspondence time limit] [the specified misc correspondence]; otherwise, false.
    /// </returns>
    public bool IsCorrespondenceOutSideTimeLimit(string invoiceId, out MiscUatpInvoice invHeader, ref bool isTimeLimitRecordFound)
    {
      var isOutsideTimeLimit = false;
      var invoiceHeader = GetInvoiceDetail(invoiceId);
      invHeader = invoiceHeader;
      if (invoiceHeader.RejectionStage == 1)
      {
        //CMP#624 : 2.10 - Change#6 : Time Limits
        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
        /* SCP#387982 - SRM: Initiate Correspondence timelimit incorrect for SMI I 
        Desc: Prior to this code fix, current system date (correspondence initiation date) was being used. This mistake is now corrected. 
        Code is now updated to use Previous transaction (Rej Stage 3 Invoice) billing Period as input for time limit determination. */
        var previousTransactionDate = new DateTime(invoiceHeader.BillingYear, invoiceHeader.BillingMonth, invoiceHeader.BillingPeriod);
        if (invoiceHeader.InvoiceSmi == SMI.IchSpecialAgreement || invoiceHeader.InvoiceSmi == SMI.Ich || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules ||
            ReferenceManager.IsSmiLikeBilateral(Convert.ToInt32(invoiceHeader.SettlementMethodId), false))
        {
            isOutsideTimeLimit =
              !ReferenceManager.IsTransactionInTimeLimitMethodB(TransactionType.MiscCorrespondence, invoiceHeader.SettlementMethodId, previousTransactionDate, invoiceHeader, DateTime.UtcNow, ref isTimeLimitRecordFound);
        }
          /* SMI A does not allowed to raise correspondence on stage 1 rejection - so SMI A is invalid here, and so this case is not handled. */
      }
      else if (invoiceHeader.RejectionStage == 2 && invoiceHeader.InvoiceSmi == SMI.Ach)
      {
          isOutsideTimeLimit = !ReferenceManager.IsTransactionInTimeLimitMethodF(TransactionType.MiscCorrespondence, invoiceHeader.SettlementMethodId, invoiceHeader, ref isTimeLimitRecordFound);
      }

      return isOutsideTimeLimit;
    }

    /// <summary>
    /// Updates the expiry date.
    /// </summary>
    /// <param name="correspondence">The correspondence.</param>
    /// <param name="invoice">The invoice.</param>
    public void UpdateExpiryDate(MiscCorrespondence correspondence, InvoiceBase invoice)
    {
      DateTime expiryDate = DateTime.MinValue;
      // If Corresp Status is Open and Sub Status is Saved/ Ready To Submit then set expiry date as expiry date of last transaction
      
      // i.e. 1. In case of N th Corresp set expiry equal to expiry of N-1 th Corresp
      //      2. In case of 1st Corresp set expiry equal to expiry of RM for which Corresp is created.
      if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && (correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || correspondence.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit))
      {
        // N th Corresp Case
        if(correspondence.CorrespondenceStage > 1)
        {
          var correspondenceHistory = MiscCorrespondenceRepository.GetCorr(c => c.CorrespondenceNumber == correspondence.CorrespondenceNumber && c.InvoiceId == correspondence.InvoiceId).ToList();
          if (correspondenceHistory != null && correspondenceHistory.Count > 0)
          {
            var prevCorrespondence =
              correspondenceHistory.Where(c => c.CorrespondenceStage == (correspondence.CorrespondenceStage - 1)).
                FirstOrDefault();
            if (prevCorrespondence != null)
            {
              expiryDate = prevCorrespondence.ExpiryDate;
            }// End if
            
          }// End if  
        }// End if
        // 1st Corresp Case
        else
        {
          // Get Time Limit for raising 1st Correspondence.
          var firstCorrespTimeLimit =
              ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.MiscCorrespondence,
                                                                    invoice.SettlementMethodId,
                                                                      new DateTime(invoice.BillingYear,
                                                                                   invoice.BillingMonth,
                                                                                   invoice.BillingPeriod));

          // Check if Settlement Method is ACH then calculate expiry date by applying Calculation Method F.
          // Method F:
          // ‘X’ number of days is added to the last calendar date month of the final rejection. The period of the last rejection has no significance.
          // e.g. 2nd Rejection Invoice  Correspondence #1
          //      1. Billing period of 2nd Rejection: 2010-Jan-P2
          //      2. Value of time limit: 90
          //      3. Last date of Jan-2010: 31-Jan-2010
          //      4. Time limit for the 1st Correspondence is calculated as 01-May-2010 (31-Jan-2010 + 90 days)
          if (invoice.InvoiceSmi == SMI.Ach)
          {
            expiryDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1).AddMonths(1).AddDays(-1).AddDays(firstCorrespTimeLimit.Limit);
          }//End if
            
          // else calculate expiry date by applying Calculation Method B 
          // Method B:
          // Closure date of the 4th period of the billing month of the final rejection is determined (from the IS Calendar). 
          // The actual period of the last rejection has no significance. ‘X’ number of months is added to that date to determine the time limit for the 1st correspondence.
          // e.g. 1st Rejection  Correspondence #1
          //      1. Billing period of 1st Rejection: 2010-Mar-P2
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

        }// End else
          
      }// End if
      else
      {
        expiryDate = invoice.InvoiceSmi == SMI.Ach
                       ? ReferenceManager.GetTimeLimitMethodExpiryDateMethodG(TransactionType.MiscOtherCorrespondence,
                                                                              invoice.SettlementMethodId,
                                                                              correspondence.CorrespondenceDate, invoice)
                       : ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.MiscOtherCorrespondence,
                                                                       invoice.SettlementMethodId,
                                                                       correspondence.CorrespondenceDate, invoice);

        // Logic to get and set billing period info for corresp before which BM should be created in case:
        // 1. Due To Expiry i.e. other party(first corresp to member) fails to respond with in time limit.
        // 2. Due To Authority To Bill i.e. other party has autorized the corresp initiating party(first corresp from member) to create a BM.
        // Both the cases use Method D for calculating Billing Period info.
        //e.g.
        // 1. MISC 5th Correspondence (expired)  BM
        //    a. Date of 5th correspondence’s transmission: 03-Jan-2012
        //    b. Value of time limit: 5
        //    c. Time limit for BM: 2012-Jun-P4, calculated as:
        //       i.  2012-Jan + 5 months = 2012-Jun
        //       ii. Period = 4 (always)

        // 2. MISC 2nd Correspondence (having authority to bill)  BM
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
          ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.MiscCorrInvoiceDueToExpiry,
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
          ReferenceManager.GetTransactionTimeLimitTransactionType(TransactionType.MiscCorrInvoiceDueToAuthorityToBill,
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

  }// End UpdateExpiryDate()


    /// <summary>
    /// This method creates Pdf report for Mu Correspondences
    /// Will be called from service
    /// </summary>
    /// <param name="message"></param>
    /// <param name="basePath"></param>
    public string CreateMuCorrespondenceTrailPdf(ReportDownloadRequestMessage message, string basePath)
    {
      Logger.Info("Recieved Correspondence Trail Report Request ");
      string requestingMemberNumericCode = MemberManager.GetMember(message.RequestingMemberId).MemberCodeNumeric;
      string reportZipFolderName = String.Empty;
      reportZipFolderName = string.Format(message.BillingCategoryType == BillingCategoryType.Misc ? "MCORR-{0}-{1}" : "UCORR-{0}-{1}", requestingMemberNumericCode, message.RecordId);
      List<long> correspondenceNumbers = message.CorrespondenceNumbers;
      Logger.InfoFormat("Number of Correspondence Trails {0} ", correspondenceNumbers.Count);

      var correspondanceTrailReportManager = Ioc.Resolve<ICorrespondanceTrailReportManager>();
    	Int32[] correspondenceSubStatus = {
    	                                  	(Int32) CorrespondenceSubStatus.Saved,
    	                                  	(Int32) CorrespondenceSubStatus.ReadyForSubmit
    	                                  };
      var reportPdfPaths = new List<string>();
      foreach (var correspondenceNumber in correspondenceNumbers)
      {
        var correspondences = new List<MiscCorrespondence>();
        var fromReportModule = new List<Contact>();
        var toReportModule = new List<Contact>();
        string otherMemberNumericCode = string.Empty;

        var fromContacts = new List<CorrespondenceReportContact>();
        var toContacts = new List<CorrespondenceReportContact>();

        var corrs = MiscCorrespondenceRepository.GetCorrespondenceForTraiReport(corr => corr.CorrespondenceNumber == correspondenceNumber);
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

          fromReportModule.Add(GetContactDetails(correspondences[correspondencesIndex].FromMemberId, ProcessingContactType.MiscCorrespondence));

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
          toReportModule.Add(GetContactDetails(correspondences[correspondencesIndex].ToMemberId, ProcessingContactType.MiscCorrespondence));

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

        string reportPdfPath = correspondanceTrailReportManager.CreateCorrespondenceTrailPdf(FileIo.GetForlderPath(SFRFolderPath.ISCorrRepFolder), correspondences, correspondenceNumber, fromContacts, toContacts, message.RequestingMemberId, message.BillingCategoryType, otherMemberNumericCode);
        if (File.Exists(reportPdfPath))
        {
          Logger.InfoFormat("Correspondence Pdf created for correspondence Number {0} at location {1} ", correspondenceNumber, reportPdfPath);
          reportPdfPaths.Add(reportPdfPath);
        }
        else
        {
          Logger.InfoFormat("Correspondence Pdf creation failed forcorrespondence Number {0} ", correspondenceNumber);
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
      //FileIo.MoveFile(Path.Combine(sfrTempRootPath, zipFileName), Path.Combine(basePath, zipFileName));
      return Path.Combine(basePath, zipFileName);

    }

    //CMP527: Get first stage correspondence by using correspondence number.
    public MiscCorrespondence GetFirstCorrespondenceByCorrespondenceNo(long correspondenceNo)
    {
      var correspondence = MiscCorrespondenceRepository.GetAll().Where(corr => corr.CorrespondenceNumber == correspondenceNo).OrderBy(corr=>corr.CorrespondenceStage).ToList().FirstOrDefault();
      return correspondence;
    }

    public MiscCorrespondence GetLastRespondedCorrespondene(long correspondenceNumber)
    {
        var correspondence =
            MiscCorrespondenceRepository.GetLastRespondedCorrespondene(corr => corr.CorrespondenceNumber == correspondenceNumber &&
                    corr.CorrespondenceSubStatusId == 2).OrderByDescending(corr => corr.CorrespondenceStage).FirstOrDefault();

        return correspondence;
    }
  }
}
