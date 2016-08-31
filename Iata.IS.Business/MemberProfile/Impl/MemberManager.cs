using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions; 
using Iata.IS.Data;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.Enums;
using System.Net.Mail;
using iPayables.UserManagement;
using log4net;
using System.Configuration;
using Castle.Core.Smtp;
using Iata.IS.Business.TemplatedTextGenerator;
using NVelocity;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class MemberManager : IMemberManager, IValidationMemberManager
  {
    private const int Active = 1;
    private const int InActive = 2;

    private const string MainLocation = "Main";
    private const string UatpLocation = "UATP";

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    // user id of logged in user
    public int UserId { get; set; }

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Gets or sets the notification manager.
    /// </summary>
    /// <value>The notification manager.</value>
    public INotificationManager NotificationManager { get; set; }

    /// <summary>
    /// Gets or sets the Future update manager.
    /// </summary>
    /// <value>The Future update manager.</value>
    public IFutureUpdatesManager FutureUpdatesManager { get; set; }

    /// <summary>
    /// Gets or sets the location repository.
    /// </summary>
    /// <value>The location repository.</value>
    public ILocationRepository LocationRepository { get; set; }

    /// <summary>
    /// Gets or sets the member repository.
    /// </summary>
    /// <value>The member repository.</value>
    public IMemberRepository MemberRepository { get; set; }

    /// <summary>
    /// Gets or sets the contact repository.
    /// </summary>
    /// <value>The contact repository.</value>
    public IRepository<Contact> ContactRepository { get; set; }

    /// <summary>
    /// Gets or sets the eBilling repository.
    /// </summary>
    /// <value>The eBilling repository.</value>
    public IRepository<EBillingConfiguration> EBillingRepository { get; set; }

    /// <summary>
    /// Gets or sets the Technical repository.
    /// </summary>
    /// <value>The Technical repository.</value>
    public IRepository<TechnicalConfiguration> TechnicalRepository { get; set; }

    /// <summary>
    /// Gets or sets the passenger repository.
    /// </summary>
    /// <value>The passenger repository.</value>
    public IRepository<PassengerConfiguration> PassengerRepository { get; set; }

    /// <summary>
    /// Gets or sets the cargo configuration repository.
    /// </summary>
    /// <value>The cargo configuration repository.</value>
    public IRepository<CargoConfiguration> CargoRepository { get; set; }

    /// <summary>
    /// Gets or sets the mem_contact_type_matrix repository.
    /// </summary>
    /// <value>The mem_contact_type_matrix repository.</value>
    public IRepository<ContactContactTypeMatrix> ContactTypeMatrixRepository { get; set; }

    /// <summary>
    /// Gets or sets the miscellaneous configuration repository.
    /// </summary>
    /// <value>The miscellaneous configuration repository.</value>
    public IRepository<MiscellaneousConfiguration> MiscellaneousRepository { get; set; }

    /// <summary>
    /// Gets or sets the UATP configuration repository.
    /// </summary>
    /// <value>The UATP configuration repository.</value>
    public IRepository<UatpConfiguration> UatpRepository { get; set; }

    /// <summary>
    /// Gets or sets the Ach configuration repository.
    /// </summary>
    /// <value>The Ach configuration repository.</value>
    public IAchRepository AchRepository { get; set; }

    /// <summary>
    /// Gets or sets the ICH configuration repository.
    /// </summary>
    /// <value>The ICH configuration repository.</value>
    public IIchRepository IchRepository { get; set; }

    /// <summary>
    /// Gets or sets the contact type configuration repository.
    /// </summary>
    /// <value>The contact type repository.</value>
    public IRepository<ContactType> ContactTypeRepository { get; set; }

    /// <summary>
    /// Gets or sets the future updates repository.
    /// </summary>
    /// <value>The FutureUpdates Repository.</value>
    public IRepository<FutureUpdates> FutureUpdatesRepository { get; set; }

    /// <summary>
    /// Gets or sets the MemberStatus  repository.
    /// </summary>
    public IRepository<MemberStatusDetails> MemberStatusRepository { get; set; }

    /// <summary>
    /// Gets or sets the ACHException Repository.
    /// </summary>
    /// <value>The A ACHException repository.</value>
    public IRepository<AchException> AchExceptionRepository { get; set; }

    /// <summary>
    /// Gets or sets Country Repository.
    /// </summary>
    public IRepository<Country> CountryRepository { get; set; }

    /// <summary>
    /// Gets or sets Subdivision Repository.
    /// </summary>
    public IRepository<SubDivision> SubdivisionRepository { get; set; }

    /// <summary>
    /// Gets or sets DSRequiredCountrymapping Repository.
    /// </summary>
    public IRepository<DSRequiredCountrymapping> DsRequiredCountryRepository { get; set; }

    /// <summary>
    /// Gets or sets Contacts Repository.
    /// </summary>
    public IContactRepository ContactsRepository { get; set; }

    /// <summary>
    /// Gets or sets Dual Member Repository.
    /// </summary>
    public IDualMemberRepository DualMemberRepository { get; set; }

    /// <summary>
    /// Gets or sets calendar Manager .
    /// </summary>
    public ICalendarManager CalendarManager { get; set; }

    /// <summary>
    /// Gets or sets currency repository .
    /// </summary>
    public IRepository<Currency> CurrencyRepository { get; set; }


    /// <summary>
    /// Gets or sets the MemberLogo repository.
    /// </summary>
    /// <value>The Ach configuration repository.</value>
    public IRepository<MemberLogo> MemberLogoRepository { get; set; }

    /// <summary>
    /// Gets or sets ICH Update Handler Manager .
    /// </summary>
    public IICHUpdateHandler IchUpdateHandler { get; set; }

    public IRepository<Member> MembersRepository { get; set; }
    public IUserManagement AuthManager { get; set; }
    public IRepository<EmailTemplate> EmailSettingsRepository { get; set; }

     public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

    private static void TrimFields(Member member)
    {
      if(member.DefaultLocation != null)
      {
        if(!string.IsNullOrEmpty(member.DefaultLocation.TaxVatRegistrationNumber))
          member.DefaultLocation.TaxVatRegistrationNumber = member.DefaultLocation.TaxVatRegistrationNumber.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.RegistrationId))
          member.DefaultLocation.RegistrationId = member.DefaultLocation.RegistrationId.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.AdditionalTaxVatRegistrationNumber))
          member.DefaultLocation.AdditionalTaxVatRegistrationNumber = member.DefaultLocation.AdditionalTaxVatRegistrationNumber.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.AddressLine1))
          member.DefaultLocation.AddressLine1 = member.DefaultLocation.AddressLine1.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.AddressLine2))
          member.DefaultLocation.AddressLine2 = member.DefaultLocation.AddressLine2.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.AddressLine3))
          member.DefaultLocation.AddressLine3 = member.DefaultLocation.AddressLine3.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.CityName))
          member.DefaultLocation.CityName = member.DefaultLocation.CityName.Trim();
        if (!string.IsNullOrEmpty(member.DefaultLocation.PostalCode))
          member.DefaultLocation.PostalCode = member.DefaultLocation.PostalCode.Trim();
      }
    }

    /// <summary>
    /// Creates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    public Member CreateISMember(Member member)
    {
      TrimFields(member);
      // Pad 0's to left of numeric code entered by user
      member.MemberCodeNumeric = GetMemberNumericCode(member.MemberCodeNumeric);

      // If member code numeric is changed, check if member  with member numeric code already exists in database. If exists, throw exception
      if (MemberRepository.Get(m => m.MemberCodeNumeric == member.MemberCodeNumeric).Count() != 0)
      {
        throw new ISBusinessException(ErrorCodes.DuplicateMemberNumericCodeFound);
      }

      // Mark the member as PartiallyCreated
      member.IsPartillyCreated = true;

      // Commercial Name and legal name is stored in member as well as location table so that retrieving commercial Name and 
      // legal name in Search Member functionality will be easy.
      member.CommercialName = member.DefaultLocation.MemberCommercialName;
      member.LegalName = member.DefaultLocation.MemberLegalName;


      // Add Member Status Details
      DateTime updatedStatusDate;

      if (member.TerminationDate != null && member.IsMembershipStatusId == InActive)
      {
        updatedStatusDate = member.TerminationDate.Value;
      }

      if (member.EntryDate != null && member.IsMembershipStatusId == Active)
      {
        updatedStatusDate = member.EntryDate.Value;
      }
      else
      {
        updatedStatusDate = DateTime.UtcNow;
      }

      var memberStatus = new MemberStatusDetails
                           {
                             MemberId = member.Id,
                             MembershipStatusId = member.IsMembershipStatusId,
                             StatusChangeDate = updatedStatusDate,
                             MemberType = "MEM",
                             LastUpdatedOn = DateTime.UtcNow
                           };


      var location = new Location
                       {
                         MemberId = member.Id,
                         LocationCode = MainLocation,
                         MemberLegalName = member.DefaultLocation.MemberLegalName,
                         MemberCommercialName = member.DefaultLocation.MemberCommercialName,
                         RegistrationId = member.DefaultLocation.RegistrationId,
                         TaxVatRegistrationNumber = member.DefaultLocation.TaxVatRegistrationNumber,
                         AdditionalTaxVatRegistrationNumber =
                           member.DefaultLocation.AdditionalTaxVatRegistrationNumber,
                         CountryId = member.DefaultLocation.CountryId,
                         AddressLine1 = member.DefaultLocation.AddressLine1,
                         AddressLine2 = member.DefaultLocation.AddressLine2,
                         AddressLine3 = member.DefaultLocation.AddressLine3,
                         SubDivisionName = member.DefaultLocation.SubDivisionName,
                         PostalCode = member.DefaultLocation.PostalCode,
                         Iban = member.DefaultLocation.Iban,
                         Swift = member.DefaultLocation.Swift,
                         BankCode = member.DefaultLocation.BankCode,
                         BranchCode = member.DefaultLocation.BranchCode,
                         BankAccountName = member.DefaultLocation.BankAccountName,
                         BankAccountNumber = member.DefaultLocation.BankAccountNumber,
                         CurrencyId = member.DefaultLocation.CurrencyId,
                         BankName = member.DefaultLocation.BankName,
                         CityName = member.DefaultLocation.CityName,
                         IsActive = true
                       };

      // Trim all address related fields.
      if (!string.IsNullOrEmpty(location.TaxVatRegistrationNumber))
        location.TaxVatRegistrationNumber = location.TaxVatRegistrationNumber.Trim();
      if (!string.IsNullOrEmpty(location.AdditionalTaxVatRegistrationNumber))
        location.AdditionalTaxVatRegistrationNumber = location.AdditionalTaxVatRegistrationNumber.Trim();
      if (!string.IsNullOrEmpty(location.AddressLine1))
        location.AddressLine1 = location.AddressLine1.Trim();
      if (!string.IsNullOrEmpty(location.AddressLine2))
        location.AddressLine2 = location.AddressLine2.Trim();
      if (!string.IsNullOrEmpty(location.AddressLine3))
        location.AddressLine3 = location.AddressLine3.Trim();
      if (!string.IsNullOrEmpty(location.CityName))
        location.CityName = location.CityName.Trim();
      if (!string.IsNullOrEmpty(location.PostalCode))
        location.PostalCode = location.PostalCode.Trim();


      //Get subdivision code corresponding to subdivision name for location
      if (location.SubDivisionName != null)
      {
        var subdivisionRecord =
          SubdivisionRepository.Single(
            subdiv => subdiv.Name == location.SubDivisionName && subdiv.CountryId == location.CountryId);

        location.SubDivisionCode = subdivisionRecord != null ? subdivisionRecord.Id : null;
      }

      if (location.IsUatpLocation)
      {
        location.LocationCode = UatpLocation;
        location.IsActive = true;
      }

      if (location.LocationCode == null)
      {
        location.LocationCode = GetLocationCode(location.CityName, location.MemberId);
      }

      return MemberRepository.CreateISMember(member, location, memberStatus);
      
    }


        /// <summary>
        /// Creates the member.
        /// </summary>
        /// <param name="member">The member.</param>
        public Member CreateMember(Member member)
        {
          TrimFields(member);
            // Pad 0's to left of numeric code entered by user
            member.MemberCodeNumeric = GetMemberNumericCode(member.MemberCodeNumeric);

        // If member code numeric is changed, check if member  with member numeric code already exists in database. If exists, throw exception
      if (MemberRepository.Get(m => m.MemberCodeNumeric == member.MemberCodeNumeric).Count() != 0)
      {
        throw new ISBusinessException(ErrorCodes.DuplicateMemberNumericCodeFound);
      }

      // Mark the member as PartiallyCreated
      member.IsPartillyCreated = true;



      // Commercial Name and legal name is stored in member as well as location table so that retrieving commercial Name and 
      // legal name in Search Member functionality will be easy.
      member.CommercialName = member.DefaultLocation.MemberCommercialName;
      member.LegalName = member.DefaultLocation.MemberLegalName;

      // Add the member to the repository.
      MemberRepository.Add(member);
     UnitOfWork.CommitDefault();
      // Ascertain the audit trail entries (commit needs to be done before since we need the relation id in the audit entries).
      var auditProcessor = new AuditProcessor<Member>();
      var futureUpdatesList = auditProcessor.ProcessAuditEntries(member.Id, ActionType.Create, ElementGroupType.MemberDetails, null, member, UserId);

      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(futureUpdatesList);

      // Add Member Status Details
      DateTime updatedStatusDate;

      if (member.TerminationDate != null && member.IsMembershipStatusId == InActive)
      {
        updatedStatusDate = member.TerminationDate.Value;
      }

      if (member.EntryDate != null && member.IsMembershipStatusId == Active)
      {
        updatedStatusDate = member.EntryDate.Value;
      }
      else
      {
        updatedStatusDate = DateTime.UtcNow;
      }

      UpdateMemberStatusHistory(updatedStatusDate, member);

      // Create member location information using the fields in member instance 
      var location = new Location
                       {
                         MemberId = member.Id,
                         LocationCode = MainLocation,
                         MemberLegalName = member.DefaultLocation.MemberLegalName,
                         MemberCommercialName = member.DefaultLocation.MemberCommercialName,
                         RegistrationId = member.DefaultLocation.RegistrationId,
                         TaxVatRegistrationNumber = member.DefaultLocation.TaxVatRegistrationNumber,
                         AdditionalTaxVatRegistrationNumber = member.DefaultLocation.AdditionalTaxVatRegistrationNumber,
                         CountryId = member.DefaultLocation.CountryId,
                         AddressLine1 = member.DefaultLocation.AddressLine1,
                         AddressLine2 = member.DefaultLocation.AddressLine2,
                         AddressLine3 = member.DefaultLocation.AddressLine3,
                         SubDivisionName = member.DefaultLocation.SubDivisionName,
                         PostalCode = member.DefaultLocation.PostalCode,
                         Iban = member.DefaultLocation.Iban,
                         Swift = member.DefaultLocation.Swift,
                         BankCode = member.DefaultLocation.BankCode,
                         BranchCode = member.DefaultLocation.BranchCode,
                         BankAccountName = member.DefaultLocation.BankAccountName,
                         BankAccountNumber = member.DefaultLocation.BankAccountNumber,
                         CurrencyId = member.DefaultLocation.CurrencyId,
                         BankName = member.DefaultLocation.BankName,
                         CityName = member.DefaultLocation.CityName,
                         IsActive = true
                       };

      UpdateMemberLocation(member.Id, location);
      member.DefaultLocation = location;

      // As part of the CREATE MEMBER functionality, when the member profile gets created, 
      // system should create E-Billing, PAX, CGO, MISC, UATP records with default values for mandatory fields.

      //Create eBilling Configuration
      var eBillingConfiguration = new EBillingConfiguration();

      //Create passenger configuration
      var passengerConfiguration = new PassengerConfiguration();

      //Create Cargo configuration
      var cargoConfiguration = new CargoConfiguration();

      //Create Miscellaneous configuration
      var miscConfiguration = new MiscellaneousConfiguration();

      //Create UATP configuration
      var uatpConfiguration = new UatpConfiguration();

      UpdateEBillingConfiguration(member.Id, eBillingConfiguration);
      UpdatePassengerConfiguration(member.Id, passengerConfiguration);
      UpdateCargoConfiguration(member.Id, cargoConfiguration);
      UpdateMiscellaneousConfiguration(member.Id, miscConfiguration);
      UpdateUatpConfiguration(member.Id, uatpConfiguration);

      return member;
    }

    /// <summary>
    /// Updates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    public Member UpdateMember(Member member)
    {
       TrimFields(member);
      var memberFutureUpdatesList = new List<FutureUpdates>();
      var auditProcessor = new AuditProcessor<Member>();

      //Read value for ACH status and ICH Status of the member
      var ichConfiguration = GetIchConfig(member.Id);
      var achConfiguration = GetAchConfig(member.Id);
      var insertMessageinMessageQueue = false;
      // Pad 0's to left of numeric code entered by user
      member.MemberCodeNumeric = GetMemberNumericCode(member.MemberCodeNumeric);

      var memberData = MemberRepository.Single(m => m.Id == member.Id);

     
      if (memberData != null)
      {
        // If member code numeric is changed, check if member  with member numeric code already exists in database.If exists, throw exception
        if (memberData.MemberCodeNumeric != member.MemberCodeNumeric)
        {
          if (MemberRepository.Get(m => m.MemberCodeNumeric == member.MemberCodeNumeric).Count() != 0)
          {
            throw new ISBusinessException(ErrorCodes.DuplicateMemberNumericCodeFound);
          }
          // SCP223072: Unable to change Member Code
          // User is renaming the Member Code Numeric hence,
          // update the Member SAN folder path, Member Logo file, Path in BIConfiguration table and Correspondence reference number.
          UpdateFolderAndPath(memberData.MemberCodeNumeric, member.MemberCodeNumeric, memberData.Id);
        }
        else
        {
          // If member code numeric is changed, check if member with member numeric code is the only member in database.If exists, throw exception
          if (MemberRepository.Get(m => m.MemberCodeNumeric == member.MemberCodeNumeric).Count() != 1)
          {
            throw new ISBusinessException(ErrorCodes.DuplicateMemberNumericCodeFound);
          }
        }

        var defaultLocationDetail = LocationRepository.Single(location => location.MemberId == member.Id && location.LocationCode == MainLocation);
        if (defaultLocationDetail != null && memberData.DefaultLocation == null)
        {
          // Assign default location to member
          memberData.DefaultLocation = defaultLocationDetail;
        }
      }

      // Add Member Status Details
      // SCP196804: IS Entry date changed when updates are done to the locations
      // Desc: By defalut pass null to UpdateMemberStatusHistory() in order to bypass the code which unnecessarily updates the IS Entry Date.
      // SCP235645 - IS Membership status changes not recorded in the change history or profile changes audit trail// 
      DateTime? updatedStatusDate;

      if (member.TerminationDate != null && member.IsMembershipStatusId == (int)MemberStatus.Terminated)
      {
        updatedStatusDate = member.TerminationDate.Value;
      }
      else if (member.EntryDate != null && member.IsMembershipStatusId == (int)MemberStatus.Active)
      {
        updatedStatusDate = member.EntryDate.Value;
      }
      else if (member.EntryDate == null && member.IsMembershipStatusId == (int)MemberStatus.Active)// this condition is for while Location Tab where EntryDate is null
      {
        updatedStatusDate = null;
      }
      else 
      {
        updatedStatusDate = member.IsMembershipStatusId != memberData.IsMembershipStatusId
                              ? (DateTime?) DateTime.UtcNow
                              : null;
      }

      UpdateMemberStatusHistory(updatedStatusDate, member);
     member.LegalName = member.DefaultLocation.MemberLegalName;

     // CMP597: TFS_Bug_8930 IS WEB -Memebr legal name is not a future update field from SIS ops login.
     // Simultaneous updation of 'Member Legal Name' on 'Member Details' and 'Locations' tabs
     member.LegalNameFuturePeriod = member.DefaultLocation.MemberLegalNameFuturePeriod;
     member.LegalNameFutureValue = member.DefaultLocation.MemberLegalNameFutureValue;

      member.CommercialName = member.DefaultLocation.MemberCommercialName;

      // Get information about immediate updates for sending them to contacts
      // Check if IATA Membership value is updated
      if (memberData != null)
      {
        // Get the list of pending future updates for member detail.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(member.Id, ElementGroupType.MemberDetails);
        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(member.Id, ActionType.Update, ElementGroupType.MemberDetails, memberData, member, UserId, pendingUpdates);

        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);

        /* SCP# 400659 - Member Legal name change does not trigger XML sent to ICH
        Desc: Legal Name future updated value is now considered for deciding if update XMl has to be sent or not.
        Prior to this current value was being used for comparison (hence expression evaluates as alwys false), which was incorrect. */
        string latestMemberLegalName = member.LegalName;

        if (memberFutureUpdatesList != null)
        {
            foreach (var memberFutureUpdate in memberFutureUpdatesList)
            {
                if (memberFutureUpdate.ElementName.Equals("MEMBER_LEGAL_NAME", StringComparison.CurrentCultureIgnoreCase))
                {
                    latestMemberLegalName = memberFutureUpdate.NewVAlue;
                }
            }
        }

        //Send update to ICH if the member is Live ICH member
        //And if value of any of the fileds Member code numeric, member code alpha,member legal name,member commercial name,comments is changed
        if (((ichConfiguration != null) && (ichConfiguration.IchMemberShipStatusId != 4)) || ((achConfiguration != null) && (achConfiguration.AchMembershipStatusId != 4)))
        {
            if ((memberData.MemberCodeNumeric != member.MemberCodeNumeric) || (memberData.MemberCodeAlpha != member.MemberCodeAlpha) || (memberData.LegalName != latestMemberLegalName) ||
              (memberData.CommercialName != member.CommercialName) || (memberData.IsOpsComments != member.IsOpsComments))
          {
            insertMessageinMessageQueue = true;
          }
        }
      }

      // Update future updates to database
      try
      {
        if (memberData != null)
        {
          // If membership status is changed and future status value is not set as Terminated then add immediate update record to database
          if ((memberData.IsMembershipStatusId != member.IsMembershipStatusId) &&
              (member.IsMembershipStatusIdFutureValue == null))
          {
            AddAuditTrailForImmediateUpdates(ElementGroupType.MemberDetails,
                                             member.Id,
                                             "IS_MEMBERSHIP_STATUS",
                                             memberData.IsMembershipStatusId.ToString(),
                                             member.IsMembershipStatusId.ToString(),
                                             (int) ActionType.Update,
                                             null,
                                             Enum.GetName(typeof(MemberStatus),memberData.IsMembershipStatusId),
                                             Enum.GetName(typeof(MemberStatus), member.IsMembershipStatusId),
                                             ref memberFutureUpdatesList);
          }
            // Set future updates for Termination Date if future values are set by user
          else if ((memberData.IsMembershipStatusIdFuturePeriod != member.IsMembershipStatusIdFuturePeriod) &&
                   (member.IsMembershipStatusIdFutureValue != null))
          {
            UpdateFutureUpdates(ElementGroupType.MemberDetails,
                                member.Id,
                                "IS_MEMBERSHIP_STATUS",
                                memberData.IsMembershipStatusId.ToString(),
                                member.IsMembershipStatusIdFutureValue.ToString(),
                                member.IsMembershipStatusIdFuturePeriod,
                                null,
                                (int) ActionType.Update,
                                false,
                                null,
                                member.IsMembershipStatusIdDisplayValue,
                                member.IsMembershipStatusIdFutureDisplayValue,
                                ref memberFutureUpdatesList);
          }
        }
      }
      catch (ISBusinessException)
      {
        throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
      }

      // Update member's information to database
      var updatedMember = MemberRepository.Update(member);

      // Update Default location details to database
      member.DefaultLocation.MemberId = member.Id;
      var locationData = LocationRepository.Single(loc => loc.Id == member.DefaultLocation.Id);
      // Get the list of pending future updates for Location.
      //SCP325349 - Unable to change member legal name(Get pending future update based on relation id).
      var pendingUpdatesLocation = FutureUpdatesManager.GetPendingFutureUpdates(member.Id, ElementGroupType.Locations, locationData.Id);
      var auditProcessorLocation = new AuditProcessor<Location>();
      var locationFutureUpdates = auditProcessorLocation.ProcessAuditEntries(member.Id,
                                                ActionType.Update,
                                                                           ElementGroupType.Locations,
                                                                             locationData,
                                                                             member.DefaultLocation,
                                                                             UserId,
                                                                             pendingUpdatesLocation);

      memberFutureUpdatesList.AddRange(locationFutureUpdates);
      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(locationFutureUpdates);
      var updatedLocation = LocationRepository.Update(member.DefaultLocation);
      updatedMember.DefaultLocation = updatedLocation;

     // Update contact details to database
      if (member.ContactList != null)
      {
        UpdateContactContactTypeMatrix(member.ContactList, string.Empty);
      }

      UnitOfWork.CommitDefault();

     // Send email for immediate updates.
     // SCP186215: Member Code Mismatch between Member and Location Details
     // Email alerts to all Members for change of Reference Data should not be sent when the Member’s IS Membership Status is Pending
      if(member.IsMembershipStatusId != (int)MemberStatus.Pending)
      {
        SendImmediateUpdatesEmail(memberFutureUpdatesList);
      }

      // Call InsertMessageInOracleQueue() method which will insert message in Oracle queue
      if (insertMessageinMessageQueue == true)
      {
          /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
           * Desc: Passing ICH Membership status and ACH membership status for this member. */
          InsertMessageInOracleQueue("MemberProfileUpdate", member.Id,
                                     ichConfiguration != null ? ichConfiguration.IchMemberShipStatusId : -1,
                                     achConfiguration != null ? achConfiguration.AchMembershipStatusId : -1);
        // SCP186215: Member Code Mismatch between Member and Location Details
        // // Email alerts to all Members for change of Reference Data should not be sent when the Member’s IS Membership Status is Pending
        if (member.IsMembershipStatusId != (int)MemberStatus.Pending)
        {
          SendMailToIchForMemberProfileUpdate(member.Id);
        }
      }

      return updatedMember;
    }

    #region SCP223072: Unable to change Member Code
    /// <summary>
    /// Function to update the Member SAN folder path, Member Logo file, Path in BIConfiguration table and Correspondence reference number.
    /// </summary>
    /// <param name="oldMemberCodeNumeric">Old Member Code Numeric</param>
    /// <param name="newMemberCodeNumeric">New Member Code Numeric</param>
    /// <param name="memberId">Member Id</param>
    private void UpdateFolderAndPath(string oldMemberCodeNumeric, string newMemberCodeNumeric, int memberId)
    {
      // Get base path for FTP Root Folder.
      var ftpRootBaseDirectoryPath = SystemParameters.Instance.General.FtpRootBasePath;

      // Get bse path for Member Logo.
      var memberLogoDirectoryPath = SystemParameters.Instance.General.MemberLogoFileLocation;

      // Try for Step 1. To rename the member SAN FTP folder path
      try
      {
        // 1. To rename the member SAN FTP folder path
        if (RenameDirectoryOrFile(ftpRootBaseDirectoryPath, oldMemberCodeNumeric, newMemberCodeNumeric, true))
        {
          Logger.InfoFormat(string.Format("Succsess: To rename the member SAN FTP folder path."));
          Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(ftpRootBaseDirectoryPath, oldMemberCodeNumeric)));
          Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(ftpRootBaseDirectoryPath, newMemberCodeNumeric)));

          // Try for Step 2. To rename the Member Logo file.
          try
          {
            // 2. To rename the Member Logo file.
            if (RenameDirectoryOrFile(memberLogoDirectoryPath, oldMemberCodeNumeric, newMemberCodeNumeric, false))
            {
              Logger.InfoFormat(string.Format("Succsess: To rename the Member Logo file."));
              Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(memberLogoDirectoryPath, oldMemberCodeNumeric)));
              Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(memberLogoDirectoryPath, newMemberCodeNumeric)));

              // Try for Step 3. To update BICONFIGURATION table and Correspondence reference number.
              try
              {
                // Removing entry of Member from Cache
                var cacheManager = Ioc.Resolve<ICacheManager>();
                var memberIdKey = string.Format("Member_{0}", memberId);
                
                cacheManager.Remove(memberIdKey);

                // 3. To update BICONFIGURATION table and Correspondence reference number.
                var updateResult = UpdateBiConfigForNewMemberCodeNumeric(oldMemberCodeNumeric, newMemberCodeNumeric, memberId, "ISWEB");

                // If result of update operation is successful.
                if (updateResult != 0)
                {
                  // Removing entry of Member from Cache
                  cacheManager.Remove(memberIdKey);

                  // If only BICONFIGURATION table update operatin is successful.
                  if (updateResult == 1)
                  {
                    Logger.InfoFormat(string.Format("Only BICONFIGURATION table updated."));
                  } // end If
                  // If both i.e BICONFIGURATION table update and MEM_LAST_CORR_REFNO table update is successful.
                  if (updateResult == 2)
                  {
                    Logger.InfoFormat(string.Format("BICONFIGURATION and MEM_LAST_CORR_REFNO table updated."));
                  } // End if.
                } // End if.
                // Else result of update operation is fail then roll back step 2 and step 1.
                else
                {
                  Logger.InfoFormat(string.Format("Failure: To update BICONFIGURATION table and MEM_LAST_CORR_REFNO table."));

                  // Throw new business exception.
                  throw new ISBusinessException(ErrorCodes.UnableToChangeMemberPrefix);
                } // End else.
              } // End of Try for Step 3. To update BICONFIGURATION table and Correspondence reference number.
              // Catch for Step 3. To update BICONFIGURATION table and Correspondence reference number.
              catch (Exception exception)
              {
                Logger.ErrorFormat(string.Format("Exception occured while updating BICONFIGURATION table: {0}",
                                                 exception));
                Logger.InfoFormat(string.Format("Failure: To update BICONFIGURATION table."));

                // Roll Back Step 2. To rename the Member Logo file.s
                RollbackMemberLogoFile(oldMemberCodeNumeric, newMemberCodeNumeric, memberLogoDirectoryPath);

                // throw new business exception.
                throw new ISBusinessException(ErrorCodes.UnableToChangeMemberPrefix);
              } // End of Catch for Step 3. To update BICONFIGURATION table and Correspondence reference number.
            } // End if for Step 2. To rename the Member Logo file.
            // Else of Step 2. To rename the Member Logo file.
            else
            {
              Logger.InfoFormat(string.Format("Failure: To rename the Member Logo file."));
              Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(memberLogoDirectoryPath, oldMemberCodeNumeric)));
              Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(memberLogoDirectoryPath, newMemberCodeNumeric)));

              // Throw new business exception.
              throw new ISBusinessException(ErrorCodes.UnableToChangeMemberPrefix);
            } // End else of Step 2. To rename the Member Logo file.
          } // End of Try for Step 2. To rename the Member Logo file.
          // Catch for Step 2. To rename the Member Logo file.
          catch (Exception exception)
          {
            Logger.ErrorFormat(string.Format("Exception occured while renaming MemberLogo file: {0}", exception));
            Logger.InfoFormat(string.Format("Failure: To rename the Member Logo file."));
            Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(memberLogoDirectoryPath, oldMemberCodeNumeric)));
            Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(memberLogoDirectoryPath, newMemberCodeNumeric)));

            // Roll Back Step 1. To rename the member SAN FTP folder path
            RollbackSanFolderPath(ftpRootBaseDirectoryPath, oldMemberCodeNumeric, newMemberCodeNumeric);

            // Throw new business exception.
            throw new ISBusinessException(ErrorCodes.UnableToChangeMemberPrefix);
          } // End of Catch for Step 2. To rename the Member Logo file.
        } // End if of Step 1. To rename the member SAN FTP folder path
        // Else for Step 1. To rename the member SAN FTP folder path
        else
        {
          Logger.InfoFormat(string.Format("Failure: To rename the member SAN FTP folder path."));
          Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(ftpRootBaseDirectoryPath, oldMemberCodeNumeric)));
          Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(ftpRootBaseDirectoryPath, newMemberCodeNumeric)));
          
          // Throw new business exception.
          throw new ISBusinessException(ErrorCodes.UnableToChangeMemberPrefix);
        } // End else of Step 1. To rename the member SAN FTP folder path
      } // End of Try for Step 1. To rename the member SAN FTP folder path
      // Catch for Step 1. To rename the member SAN FTP folder path
      catch (Exception exception)
      {
        Logger.ErrorFormat(string.Format("Exception occured while updating Member Code Numeric: {0}", exception));

        // Throw new business exception.
        throw new ISBusinessException(ErrorCodes.UnableToChangeMemberPrefix);
      } // End of Catch for Step 1. To rename the member SAN FTP folder path
    } // End of method UpdateFolderAndPath.

    /// <summary>
    /// Method to Rollback Member Logo File rename operation.
    /// </summary>
    /// <param name="oldMemberCodeNumeric">Old Member Code Numeric</param>
    /// <param name="newMemberCodeNumeric">New Member Code Numeric</param>
    /// <param name="memberLogoDirectoryPath">Member Logo Directory Path</param>
    private static void RollbackMemberLogoFile(string oldMemberCodeNumeric, string newMemberCodeNumeric, string memberLogoDirectoryPath)
    {
      // Member logo rename success.
      if (RenameDirectoryOrFile(memberLogoDirectoryPath, newMemberCodeNumeric, oldMemberCodeNumeric, false))
      {
        Logger.InfoFormat(string.Format("Roll Back step 2 Succsess: To rename the Member Logo file."));
        Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(memberLogoDirectoryPath, newMemberCodeNumeric)));
        Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(memberLogoDirectoryPath, oldMemberCodeNumeric)));
      } // End if
      // Memeber logo rename failure.
      else
      {
        Logger.InfoFormat(string.Format("Roll Back step 2 Failure: To rename the Member Logo file."));
        Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(memberLogoDirectoryPath, newMemberCodeNumeric)));
        Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(memberLogoDirectoryPath, oldMemberCodeNumeric)));
      } // End else
    } // End of method RollbackMemberLogoFile

    /// <summary>
    /// Method to Rollback SAN Folder Path.
    /// </summary>
    /// <param name="ftpRootBaseDirectoryPath">FTP Root Directory Path</param>
    /// <param name="oldMemberCodeNumeric">Old Member Code Numeric</param>
    /// <param name="newMemberCodeNumeric">New Member Code Numeric</param>
    private static void RollbackSanFolderPath(string ftpRootBaseDirectoryPath, string oldMemberCodeNumeric, string newMemberCodeNumeric)
    {
      // SAN Folder path rename success.
      if (RenameDirectoryOrFile(ftpRootBaseDirectoryPath, newMemberCodeNumeric, oldMemberCodeNumeric, true))
      {
        Logger.InfoFormat("Roll back step 1 Success: To rename the member SAN FTP folder path.");
        Logger.InfoFormat(string.Format("Rollback Succsess: To rename the member SAN FTP folder path."));
        Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(ftpRootBaseDirectoryPath, newMemberCodeNumeric)));
        Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(ftpRootBaseDirectoryPath, oldMemberCodeNumeric)));
      } // End if
      // SAN Folder path rename failure.
      else
      {
        Logger.InfoFormat("Roll back step 1 Failure: To rename the member SAN FTP folder path.");
        Logger.InfoFormat(string.Format("Rollback Succsess: To rename the member SAN FTP folder path."));
        Logger.InfoFormat(string.Format("Source path: {0}", Path.Combine(ftpRootBaseDirectoryPath, newMemberCodeNumeric)));
        Logger.InfoFormat(string.Format("Destination path: {0}", Path.Combine(ftpRootBaseDirectoryPath, oldMemberCodeNumeric)));
      } // End else
    } // End of Method RollbackSanFolderPath

    /// <summary>
    /// Function to rename directory or file.
    /// </summary>
    /// <param name="basePath">base path</param>
    /// <param name="oldMembercodeNumeric">old member code numeric</param>
    /// <param name="newMemberCodeNumeric">new member code numeric</param>
    /// <param name="isDirectoryOrFile">If true then rename FTP Root directory for the given Old member code numeric; else rename the member logo file.</param>
    /// <returns>true if operation successfull else false.</returns>
    private static bool RenameDirectoryOrFile(string basePath, string oldMembercodeNumeric, string newMemberCodeNumeric, bool isDirectoryOrFile)
    {
      bool operationSuccessful;
      try
      {
        // if isDirectoryOrFile = true then its rename directory operation.
        if (isDirectoryOrFile)
        {
          // If Directory exists.
          if (Directory.Exists(Path.Combine(basePath, oldMembercodeNumeric)))
          {
            // SCP294264 - Unable to change member code 
            // Soft delete (Rename) here if new directory name already exist.
            if (Directory.Exists(Path.Combine(basePath, newMemberCodeNumeric)))
            {              
              var deleteddirectoryName = newMemberCodeNumeric + "_Deleted_" + System.DateTime.UtcNow.ToString("ddMMMyyyy'T'HHmmss");
              Directory.Move(Path.Combine(basePath, newMemberCodeNumeric), Path.Combine(basePath, deleteddirectoryName));
            }
            // Rename directory.
            Directory.Move(Path.Combine(basePath, oldMembercodeNumeric), Path.Combine(basePath, newMemberCodeNumeric));
            operationSuccessful = true;
          } // End if
          // Else Directory does not exists.
          else
          {
            Logger.InfoFormat(string.Format("Skipping this step because path does not exist: {0}", Path.Combine(basePath, oldMembercodeNumeric)));
            operationSuccessful = true;
          } // End else
        } // End if
        // if isDirectoryOrFile = false then its rename file operation.
        else
        {
          // If Directory exists.
          if (Directory.Exists(basePath))
          {
            // Get files for the member includeing all extentions.
            var allFiles = Directory.GetFiles(basePath, oldMembercodeNumeric + ".*");
            
            // If files exists.
            if (allFiles.Count() > 0)
            {
              // Read all files one by one.
              foreach (var file in allFiles)
              {
                // SCP294264 - Unable to change member code 
                // Soft delete (Rename) here if new file name already exist.
                if (File.Exists(Path.Combine(basePath, newMemberCodeNumeric + Path.GetExtension(file))))
                {
                  var deletedFileName = newMemberCodeNumeric + "_Deleted_" + System.DateTime.UtcNow.ToString("ddMMMyyyy'T'HHmmss");
                  File.Move(file, Path.Combine(basePath, deletedFileName + Path.GetExtension(file)));
                }
                // Rename file.
                File.Move(file, Path.Combine(basePath, newMemberCodeNumeric + Path.GetExtension(file)));
              } // End foreach
              operationSuccessful = true;
            } // End if
            // If Files does not exists..
            else
            {
              Logger.InfoFormat( string.Format("Skipping this step because path does not contains logo file for the member."));
              Logger.InfoFormat(string.Format("Path: {0}, Member Code: {1}", basePath, oldMembercodeNumeric));
              operationSuccessful = true;
            } // End else
          } // End if
          // If directory does not exists.
          else
          {
            Logger.InfoFormat(string.Format("Skipping this step because path does not exist: {0}", basePath));
            operationSuccessful = true;
          } // End else
        }
      } // End of Try
      catch (Exception ex)
      {
        Logger.InfoFormat("Handled Error Occured for {0}.", isDirectoryOrFile ? "Renaming SAN folder path" : "Renaming Member Logo file/s");
        Logger.Error(ex);
        operationSuccessful = false;
      } // End of catch
      return operationSuccessful;
    } // End of Method RenameDirectoryOrFile.

    /// <summary>
    /// Method to udpate BiConfiguration Table and Mem_Last_Corr_Ref table for the Member.
    /// </summary>
    /// <param name="oldMemberCodeNumeric">Old Member Code Numeric</param>
    /// <param name="newMemberCodeNumeric">New Member CodeNumeric</param>
    /// <param name="memberId">Member Id</param>
    /// <param name="callFrom">In case of member update 'ISWEB'</param>
    /// <returns> 0 when failure; 1 when BiConfiguration Table update success; 2 when both i.e BiConfiguration Table and Mem_Last_Corr_Ref table update success.</returns>
    public int UpdateBiConfigForNewMemberCodeNumeric(string oldMemberCodeNumeric, string newMemberCodeNumeric, int memberId, string callFrom = null)
    {
      return MemberRepository.UpdateBiConfigForNewMemberCodeNumeric(oldMemberCodeNumeric, newMemberCodeNumeric, memberId, callFrom);
    }
    #endregion

    public List<Location> GetMemberLocationList(int memberId, bool showOnlyActiveLocations = false)
    {
      var memberlocationid = Convert.ToInt32(memberId);
      /*
      SCP# : 105901 - Billing to inactive location 
      Desc : Check to verify wheather location is active or not is added.
      Date : 24-May-2013
      */
       var memberLocationList = showOnlyActiveLocations ? LocationRepository.Get(ml => ml.MemberId == memberId && ml.IsActive) : LocationRepository.Get(ml => ml.MemberId == memberId);

      // Date: 18-06-2012
      // Modified By: Sachin Pharande
      // Action: Added sorting on location code.
      // Reason: To display the Location codes in sorted order after partial page (tab) load on member profile screen.
      // Issue: SCP ID : 23155 - Changing locations becomes adding location.

        if (memberLocationList.Count() > 0)
        {
            var integerMemberLocationList = memberLocationList.ToList();
            integerMemberLocationList.Clear();
            var stringMemberLocationList = memberLocationList.ToList();
            stringMemberLocationList.Clear();

            foreach (var mll in memberLocationList)
            {
                if (Regex.IsMatch(mll.LocationCode, @"^[0-9]+$"))
                {
                    integerMemberLocationList.Add(mll);
                }
                else stringMemberLocationList.Add(mll);
            }

            if (integerMemberLocationList.Count != 0)
                integerMemberLocationList = integerMemberLocationList.OrderBy(l => int.Parse(l.LocationCode)).ToList();

            if (stringMemberLocationList.Count != 0)
                stringMemberLocationList = stringMemberLocationList.OrderBy(l => l.LocationCode).ToList();

            if (integerMemberLocationList.Count != 0)
                stringMemberLocationList.AddRange(integerMemberLocationList);
            return stringMemberLocationList.ToList();
        }

        return memberLocationList.ToList();
    }

    /// <summary>
    /// Updates the E-billing configuration.
    /// </summary>
    public EBillingConfiguration UpdateEBillingConfiguration(int memberId, EBillingConfiguration eBillingConfiguration)
    {
      // Set the member id for the e-billing configuration.
      eBillingConfiguration.MemberId = memberId;

      var auditProcessor = new AuditProcessor<EBillingConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = null;
      // Check whether e-billing configuration exists already.
      var eBillingRecordInDb = EBillingRepository.Single(eBilling => eBilling.MemberId == memberId);

      if (eBillingRecordInDb == null)
      {
        // Add the e-billing configuration to the repository.
        EBillingRepository.Add(eBillingConfiguration);

        // Ascertain the audit trail entries.
        // memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.EBilling, null, eBillingConfiguration, UserId);
      }
      else
      {
        // Get the list of pending future updates.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(eBillingConfiguration.MemberId, ElementGroupType.EBilling);

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(eBillingConfiguration.MemberId,
                                                                     ActionType.Update,
                                                                     ElementGroupType.EBilling,
                                                                     eBillingRecordInDb,
                                                                     eBillingConfiguration,
                                                                     UserId,
                                                                     pendingUpdates);

        // Update e-billing configuration in repository.
        EBillingRepository.Update(eBillingConfiguration);
      }

        // Add code here for adding DS required country list
      bool updateResult = AddRemoveDsRequiredCountry(memberId,
                                                     (int)BillingTypes.Billing,
                                                     eBillingConfiguration.BillingCountiesToAdd,
                                                     eBillingConfiguration.BillingCountiesToRemove,
                                                     eBillingConfiguration.DSReqCountriesAsBillingFuturePeriod,
                                                     ref memberFutureUpdatesList);

      if (updateResult)
      {
        AddRemoveDsRequiredCountry(memberId,
                                   (int)BillingTypes.Billed,
                                   eBillingConfiguration.BilledCountiesToAdd,
                                   eBillingConfiguration.BilledCountiesToRemove,
                                   eBillingConfiguration.DSReqCountriesAsBilledFuturePeriod,
                                   ref memberFutureUpdatesList);
      }

      if (eBillingConfiguration.ContactList != null)
      {
        UpdateContactContactTypeMatrix(eBillingConfiguration.ContactList, string.Empty);
      }
      if (memberFutureUpdatesList != null)
      {
        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);
      }

      //CMP #666: MISC Legal Archiving Per Location ID
      if (!String.IsNullOrEmpty(eBillingConfiguration.MiscRecArchivingLocs) && eBillingConfiguration.RecAssociationType > 0)
      {
          // Remove first "," char from string
          if (eBillingConfiguration.MiscRecArchivingLocs.Length > 0) eBillingConfiguration.MiscRecArchivingLocs = eBillingConfiguration.MiscRecArchivingLocs.Substring(1, eBillingConfiguration.MiscRecArchivingLocs.Length - 1);
          InsertArchivalLocations(eBillingConfiguration.MiscRecArchivingLocs, eBillingConfiguration.RecAssociationType, eBillingConfiguration.LastUpdatedBy,
                                  memberId, 1); // ArchivalType: 1- Receivable
      }

      if (!String.IsNullOrEmpty(eBillingConfiguration.MiscPayArchivingLocs) && eBillingConfiguration.PayAssociationType > 0)
      {
          // Remove first "," char from string
          if (eBillingConfiguration.MiscPayArchivingLocs.Length > 0) eBillingConfiguration.MiscPayArchivingLocs = eBillingConfiguration.MiscPayArchivingLocs.Substring(1, eBillingConfiguration.MiscPayArchivingLocs.Length - 1);
          InsertArchivalLocations(eBillingConfiguration.MiscPayArchivingLocs, eBillingConfiguration.PayAssociationType, eBillingConfiguration.LastUpdatedBy,
                                  memberId, 2); // ArchivalType: 2- Payable
      }

      // Commit e-billing configurations, audit entries and special cases.
      UnitOfWork.CommitDefault();
     _logger.Debug("eBilling future updates updated successfully");
      if (memberFutureUpdatesList != null)
      {
        // Send email for immediate updates.
        SendImmediateUpdatesEmail(memberFutureUpdatesList);
      }
      return eBillingConfiguration;
    }



    /// <summary>
    /// Updates the cargo configuration.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="cargoConfiguration">The cargo configuration.</param>
    public CargoConfiguration UpdateCargoConfiguration(int memberId, CargoConfiguration cargoConfiguration)
    {
      // Set the member id for the cargo configuration.
      cargoConfiguration.MemberId = memberId;

      var auditProcessor = new AuditProcessor<CargoConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = null;

      // Check whether the cargo configuration already exists.
      var cargoRecordInDb = CargoRepository.Single(cargo => cargo.MemberId == memberId);
      if (cargoRecordInDb == null)
      {
        // Add cargo configuration to the repository.
        CargoRepository.Add(cargoConfiguration);
        // Ascertain the audit trail entries.
        // memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(cargoConfiguration.MemberId, ActionType.Create, ElementGroupType.Cgo, null, cargoConfiguration, UserId);
      }
      else
      {
        // Get the list of pending future updates.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(cargoConfiguration.MemberId, ElementGroupType.Cgo);

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(cargoConfiguration.MemberId, ActionType.Update, ElementGroupType.Cgo, cargoRecordInDb, cargoConfiguration, UserId, pendingUpdates);

       CargoRepository.Update(cargoConfiguration);
        if (cargoConfiguration.ContactList != null)
        {
          UpdateContactContactTypeMatrix(cargoConfiguration.ContactList, string.Empty);
        }
      }

      if (memberFutureUpdatesList != null)
      {
        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);
      }

      UnitOfWork.CommitDefault();
      if (memberFutureUpdatesList != null)
      {
        // Send email for immediate updates.
        SendImmediateUpdatesEmail(memberFutureUpdatesList);
      }
      return cargoConfiguration;
    }

    /// <summary>
    /// Updates the passenger configuration.
    /// </summary>
    /// <param name="memberId">member Id for which passenger configuration is added</param>
    /// <param name="passengerConfiguration">The passenger configuration.</param>
    /// <returns></returns>
    public PassengerConfiguration UpdatePassengerConfiguration(int memberId, PassengerConfiguration passengerConfiguration)
    {
      // Set the member id for the technical configuration.
      passengerConfiguration.MemberId = memberId;

      var auditProcessor = new AuditProcessor<PassengerConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = null;

      // Check whether passenger configuration record already exists in database.
      var passengerConfigurationRecordInDb = PassengerRepository.Single(passenger => passenger.MemberId == memberId);

      if (passengerConfigurationRecordInDb == null)
      {
        // Create the technical configuration.
        PassengerRepository.Add(passengerConfiguration);

        // Ascertain the audit trail entries.
        //memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.Pax, passengerConfigurationRecordInDb, passengerConfiguration, UserId);
      }
      else
      {
        /*DEPENDENT FIELDS RELATED CHANGE START*/
        //if old value for Participate in value determination is true and now it is false, then delete existing future update records for auto billling
        List<FutureUpdates> futureUpdatesRecord;
        if ((passengerConfiguration.IsParticipateInValueDetermination == false) && passengerConfigurationRecordInDb.IsParticipateInValueDetermination)
        {
          futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Pax, memberId, "IS_PARTICIPATE_IN_AUTO_BILLING", null);

          if ((futureUpdatesRecord != null) && (futureUpdatesRecord.Count > 0))
          {
            foreach (var fUpdate in futureUpdatesRecord)
            {
              FutureUpdatesRepository.Delete(fUpdate);
            }
          }
        }

        if ((passengerConfiguration.SamplingCareerTypeId == 1) && (passengerConfigurationRecordInDb.SamplingCareerTypeId != 1))
        {
          passengerConfiguration.IsConsolidatedProvisionalBillingFileRequiredFutureValue = null;
          futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Pax, memberId, "IS_CONS_PROV_BILLING_FILE_REQ", null);

          if ((futureUpdatesRecord != null) && (futureUpdatesRecord.Count > 0))
          {
            foreach (var fUpdate in futureUpdatesRecord)
            {
              FutureUpdatesRepository.Delete(fUpdate);
            }
          }
        }
        /*DEPENDENT FIELDS RELATED CHANGE END*/

        // Get the list of pending future updates.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(memberId, ElementGroupType.Pax);

             // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.Pax, passengerConfigurationRecordInDb, passengerConfiguration, UserId, pendingUpdates);

        // Update the passenger config to the database.
        PassengerRepository.Update(passengerConfiguration);

        if (passengerConfiguration.ContactList != null)
        {
          UpdateContactContactTypeMatrix(passengerConfiguration.ContactList, string.Empty);
        }
      }

      // If the object has changed then send the updates through an email.
      if (memberFutureUpdatesList != null)
      {
        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);
      }

      // Commit audit and technical configuration.
      UnitOfWork.CommitDefault();

            // If the object has changed then send the updates through an email.
            if (memberFutureUpdatesList != null)
            {
                SendImmediateUpdatesEmail(memberFutureUpdatesList);
            }

      return passengerConfiguration;
    }
    /// <summary>
    /// Updates the miscellaneous configuration.
    /// </summary>
    /// <param name="memberId">member ID for which miscellaneous details should be fetched</param>
    /// <param name="miscellaneousConfiguration">The miscellaneous configuration.</param>
    /// <returns></returns>
    public MiscellaneousConfiguration UpdateMiscellaneousConfiguration(int memberId, MiscellaneousConfiguration miscellaneousConfiguration)
    {
      // Set the member id for the miscellaneous configuration.
      miscellaneousConfiguration.MemberId = memberId;

      var auditProcessor = new AuditProcessor<MiscellaneousConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = null;

      // Check whether miscellaneous configuration exists already.
      var miscRecordInDb = MiscellaneousRepository.Single(misc => misc.MemberId == memberId);

      if (miscRecordInDb == null)
      {
        // Add miscellaneous configuration to the repository.
        MiscellaneousRepository.Add(miscellaneousConfiguration);

        // Ascertain the audit trail entries.
        // memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.Miscellaneous, null, miscellaneousConfiguration, UserId);
      }
      else
      {
        // Ascertain the audit trail entries.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(memberId, ElementGroupType.Miscellaneous);
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.Miscellaneous, miscRecordInDb, miscellaneousConfiguration, UserId, pendingUpdates);

        // Update miscellaneous configuration to the repository.
        MiscellaneousRepository.Update(miscellaneousConfiguration);
      }

      if (miscellaneousConfiguration.ContactList != null)
      {
        UpdateContactContactTypeMatrix(miscellaneousConfiguration.ContactList, string.Empty);
      }

      if (memberFutureUpdatesList != null)
      {
        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);
      }
      // Commit the miscellaneous changes and audit entries.
      UnitOfWork.CommitDefault();

      if (memberFutureUpdatesList != null)
      {
        // Send email for immediate updates.
        SendImmediateUpdatesEmail(memberFutureUpdatesList);
      }
      return miscellaneousConfiguration;
    }

    /// <summary>
    /// Updates the uatp configuration.
    /// </summary>
    /// <param name="memberId">member id for which UATP data should be fetched</param>
    /// <param name="uatpConfiguration">The uatp configuration.</param>
    /// <returns></returns>
    public UatpConfiguration UpdateUatpConfiguration(int memberId, UatpConfiguration uatpConfiguration)
    {
      // Set the member id for the UATP configuration.
      uatpConfiguration.MemberId = memberId;

      var auditProcessor = new AuditProcessor<UatpConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = null;

      // Check whether UATP configuration exists already.
      var uatpRecordInDb = UatpRepository.Single(uatp => uatp.MemberId == memberId);

      if (uatpRecordInDb == null)
      {
        // Add the UATP configuration to the repository.
        UatpRepository.Add(uatpConfiguration);

        // Ascertain the audit trail entries.
        // memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.Uatp, null, uatpConfiguration, UserId);
      }
      else
      {
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(memberId, ElementGroupType.Uatp);
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.Uatp, uatpRecordInDb, uatpConfiguration, UserId, pendingUpdates);

        // Get member data corresponding to member id passed
        var member = GetMember(memberId);

        // Set future updates (special cases).

        if (member != null)
        {
          if (uatpConfiguration.IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod != null)
          {
            var futureUpdates = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails, memberId, "IS_UATP_INV_HANDLED_BY_ATCAN", null);
            if ((futureUpdates != null) && (futureUpdates.Count > 0))
            {
              foreach (var update in futureUpdates)
              {
                if ((Convert.ToBoolean(update.NewVAlue) == false) && (uatpConfiguration.IsBillingDataSubmittedByThirdPartiesRequiredFutureValue == true))
                {
                  if (Convert.ToDateTime(update.ChangeEffectivePeriod) > Convert.ToDateTime(uatpConfiguration.IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod))
                  {
                    throw new ISBusinessException(ErrorCodes.InvalidIsUatpInvoiceHandledByAtcanError);
                  }
                }
              }
            }
          }
        }

        if (uatpConfiguration.ISUatpInvIgnoreFromDsprocFuturePeriod != null)
        {
          var futureUpdates = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails, memberId, "DIGITAL_SIGN_APPLICATION", null);
          if ((futureUpdates != null) && (futureUpdates.Count > 0))
          {
            foreach (var update in futureUpdates)
            {
              if (!Convert.ToBoolean(update.NewVAlue))
              {
                if (Convert.ToDateTime(update.ChangeEffectivePeriod) < Convert.ToDateTime(uatpConfiguration.ISUatpInvIgnoreFromDsprocFuturePeriod))
               {
                  throw new ISBusinessException(ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError);
                }
              }
            }
          }
        }

        if (uatpConfiguration.ISUatpInvIgnoreFromDsprocFuturePeriod != null)
        {
          var futureUpdates = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails, memberId, "DIGITAL_SIGN_APPLICATION", null);
          if ((futureUpdates != null) && (futureUpdates.Count > 0))
          {
            foreach (var update in futureUpdates)
            {
              if (Convert.ToBoolean(update.NewVAlue))
              {
                if (Convert.ToDateTime(update.ChangeEffectivePeriod) > Convert.ToDateTime(uatpConfiguration.ISUatpInvIgnoreFromDsprocFuturePeriod))
                {
                  throw new ISBusinessException(ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError);
                }
              }
            }
          }
        }

        // Updated digital signature flag.
        uatpConfiguration.IsDigitalSignatureRequired = member.DigitalSignApplication;
        uatpConfiguration.UatpInvoiceHandledbyAtcan = member.UatpInvoiceHandledbyAtcan;
        var futureUpdatesEbillingData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.MemberDetails, "MEMBER_DETAILS", null, null);
        if ((futureUpdatesEbillingData != null) && (futureUpdatesEbillingData.Count != 0))
        {
          foreach (var futureUpdate in futureUpdatesEbillingData.AsQueryable())
          {
            if (futureUpdate.ElementName == "DIGITAL_SIGN_APPLICATION")
            {
              uatpConfiguration.IsDigitalSignatureRequiredFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
            if (futureUpdate.ElementName == "IS_UATP_INV_HANDLED_BY_ATCAN")
            {
              uatpConfiguration.UatpInvoiceHandledbyAtcanFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
          }
        }

        // Update the UATP configuration to the repository. 
        UatpRepository.Update(uatpConfiguration);
      }

      if (memberFutureUpdatesList != null)
      {
        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);
      }
      if (uatpConfiguration.ContactList != null)
      {
        UpdateContactContactTypeMatrix(uatpConfiguration.ContactList, string.Empty);
      }

      UnitOfWork.CommitDefault();

             if (memberFutureUpdatesList != null)
            {
                // Send email for immediate updates.
                SendImmediateUpdatesEmail(memberFutureUpdatesList);
            }

            return uatpConfiguration;
        }

        /// <summary>
        /// Updates the ICH configuration.
        /// </summary>
        /// <param name="memberId">The id of the member.</param>
        /// <param name="ichConfiguration">The ICH configuration.</param>
        /// <returns></returns>
        public IchConfiguration UpdateIchConfiguration(int memberId, IchConfiguration ichConfiguration)
        {
           var billingCategory = (BillingCategoryType)Convert.ToInt32("1");

      // Set the member id for the ICH configuration.
      ichConfiguration.MemberId = memberId;

      if (ichConfiguration.AggregatedById == 0)
      {
        ichConfiguration.AggregatedById = null;
      }
      if (ichConfiguration.SponsoredById == 0)
      {
        ichConfiguration.SponsoredById = null;
      }

      var auditProcessor = new AuditProcessor<IchConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = new List<FutureUpdates>();

     // Check whether ICH configuration exists already.
      var ichConfigInDb = IchRepository.Single(ich => ich.MemberId == memberId);

      if (ichConfigInDb == null)
      {
        // Ascertain the audit trail entries.
        var futureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.Ich, ichConfigInDb, ichConfiguration, UserId);

        memberFutureUpdatesList.AddRange(futureUpdatesList);
        IchRepository.Add(ichConfiguration);

                // Add code for adding membership status in create mode.
                AddAuditTrailForImmediateUpdates(ElementGroupType.Ich,
                                                 memberId,
                                                 "ICH_MEMBERSHIP_STATUS",
                                                 null,
                                                 ichConfiguration.IchMemberShipStatusId.ToString(),
                                                 (int)ActionType.Create,
                                                 null,
                                                 ichConfiguration.IchMemberShipStatusIdDisplayValue,
                                                 ichConfiguration.IchMemberShipStatusIdFutureDisplayValue,
                                                 ref memberFutureUpdatesList);


                //CMP-689-Flexible CH Activation Options-FRS-v0.1
                //update futureupdate record here when member is not have ichmembership (IchMemberShipStatus changed from Not A member to Live)
                if (ichConfiguration.IchMemberShipStatusIdFuturePeriod != null && (ichConfiguration.IchMemberShipStatusIdFutureValue != null))
                {
                  UpdateFutureUpdates(ElementGroupType.Ich,
                                      memberId,
                                      "ICH_MEMBERSHIP_STATUS",
                                      ichConfiguration.IchMemberShipStatusId.ToString(),
                                      ichConfiguration.IchMemberShipStatusIdFutureValue.ToString(),
                                      ichConfiguration.IchMemberShipStatusIdFuturePeriod,
                                      null,
                                      (int)ActionType.Update,
                                      false,
                                      null,
                                      ichConfiguration.IchMemberShipStatusIdDisplayValue,
                                      ichConfiguration.IchMemberShipStatusIdFutureDisplayValue,
                                      ref memberFutureUpdatesList);
                }
        
        //Send notification completion of configuration of Ich/Ach elements to IS ops.

        var memberDetails = GetMemberDetails(ichConfiguration.MemberId);
        SendCompletionOfIchAchSpecificElementsForMemberNotification(memberDetails.MemberCodeAlpha + "-" + memberDetails.MemberCodeNumeric + "-" + memberDetails.CommercialName, "ICH");
      }
      else
      {
        try
        {
          // If membership status is changed and future status value is not set as Terminated then add immediate update record to database
          if ((ichConfigInDb.IchMemberShipStatusId != ichConfiguration.IchMemberShipStatusId) && (ichConfiguration.IchMemberShipStatusIdFutureValue == null))
          {
            AddAuditTrailForImmediateUpdates(ElementGroupType.Ich,
                                             memberId,
                                             "ICH_MEMBERSHIP_STATUS",
                                             ichConfigInDb.IchMemberShipStatusId.ToString(),
                                             ichConfiguration.IchMemberShipStatusId.ToString(),
                                             (int)ActionType.Update,
                                             null,
                                             ichConfiguration.IchMemberShipStatusIdDisplayValue,
                                             ichConfiguration.IchMemberShipStatusIdFutureDisplayValue,
                                             ref memberFutureUpdatesList);
          }
          // Set future updates for Termination Date if future values are set by user
          else if ((ichConfigInDb.IchMemberShipStatusIdFuturePeriod != ichConfiguration.IchMemberShipStatusIdFuturePeriod) && (ichConfiguration.IchMemberShipStatusIdFutureValue != null))
          {
            UpdateFutureUpdates(ElementGroupType.Ich,
                                memberId,
                                "ICH_MEMBERSHIP_STATUS",
                                ichConfigInDb.IchMemberShipStatusId.ToString(),
                                ichConfiguration.IchMemberShipStatusIdFutureValue.ToString(),
                                ichConfiguration.IchMemberShipStatusIdFuturePeriod,
                                null,
                                (int)ActionType.Update,
                                false,
                                null,
                                ichConfiguration.IchMemberShipStatusIdDisplayValue,
                                ichConfiguration.IchMemberShipStatusIdFutureDisplayValue,
                                ref memberFutureUpdatesList);
          }

          // Ascertain the audit trail entries.
          var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(memberId, ElementGroupType.Ich);
          var futureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.Ich, ichConfigInDb, ichConfiguration, UserId, pendingUpdates);
          memberFutureUpdatesList.AddRange(futureUpdatesList);
          IchRepository.Update(ichConfiguration);
        }
        catch (ISBusinessException)
        {
          throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
        }
      }

      //Add future update entry if Reinstatement Period value is entered
      if (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended)
      {
        if (ichConfiguration.ReinstatementPeriod != null)
        {
          UpdateFutureUpdates(ElementGroupType.Ich,
                              memberId,
                              "REINSTATEMENT_PERIOD",
                              ((int)IchMemberShipStatus.Suspended).ToString(),
                              ((int)IchMemberShipStatus.Live).ToString(),
                              ichConfiguration.ReinstatementPeriod.ToString(),
                              null,
                              (int)ActionType.Update,
                              false,
                              null,
                              Enum.GetName(typeof(IchMemberShipStatus), ichConfiguration.IchMemberShipStatusId),
                              Enum.GetName(typeof(IchMemberShipStatus), (int)IchMemberShipStatus.Live),
                              ref memberFutureUpdatesList);
        }
        else
        {
          // if Reinstatement period was already specified and value is deleted from UI, then existing entry for future update should be removed
          var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, memberId, "REINSTATEMENT_PERIOD", null);

          if (futureUpdatesRecord != null && futureUpdatesRecord.Count > 0)
          {
            FutureUpdatesRepository.Delete(futureUpdatesRecord[0]);
          }
        }
      }


      // Update sponsored member list to database
      var updateResult = UpdateSponsoredMemberList(memberId, ichConfiguration.SponsororAddList, ichConfiguration.SponsororDeleteList, ichConfiguration.SponsororFuturePeriod, ref memberFutureUpdatesList);
      if (updateResult == false)
      {
        throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
      }

      // Update aggregator member list to database
      updateResult = UpdateAggregators(memberId, ichConfiguration.AggregatorAddList, ichConfiguration.AggregatorDeleteList, ichConfiguration.AggregatorFuturePeriod, ref memberFutureUpdatesList);
      if (updateResult == false)
      {
        throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
      }

      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(memberFutureUpdatesList);





      // Review: Can this be commented?
      UnitOfWork.CommitDefault();

      //CMP-689-Flexible CH Activation Options
      // before this CMP only Members having ‘ICH Membership Status’ as "Live", "Suspended" or "Terminated" are included in the update XML
      // With this CMP, the system should also consider the following:
      //  	Members whose future value of ‘ICH Membership Status’ = “Live” (and where the current value = “Not a Member” or "Terminated")
      var isFutureUpdateLive = (ichConfiguration.IchMemberShipStatusIdFutureValue == (int)IchMemberShipStatus.Live &&
                                          (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.NotAMember ||
                                            ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Terminated))
                                              ? true
                                              : false;

     // Add Member Status Details
      var updatedStatusDate = new DateTime();



      if (ichConfiguration.TerminationDate != null && ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Terminated)
      {
        updatedStatusDate = ichConfiguration.TerminationDate.Value;
      }
      else if (ichConfiguration.EntryDate != null && (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Live || isFutureUpdateLive)) //CMP-689-Flexible CH Activation Options
      {
        updatedStatusDate = ichConfiguration.EntryDate.Value;
      }
      else if (ichConfiguration.StatusChangedDate != null && ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended)
      {
        updatedStatusDate = ichConfiguration.StatusChangedDate.Value;
      }

      if (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.NotAMember && !isFutureUpdateLive) //CMP-689-Flexible CH Activation Options
      {
        updatedStatusDate = DateTime.UtcNow.Date;
      }

      if (ichConfiguration.IchMemberShipStatusId != (int)IchMemberShipStatus.Suspended)
      {
        ichConfiguration.DefaultSuspensionDate = null;
      }

      UpdateIchMemberStatusHistory(updatedStatusDate, ichConfiguration, ichConfiguration.DefaultSuspensionDate);

      if (ichConfiguration.ContactList != null)
      {
        UpdateContactContactTypeMatrix(ichConfiguration.ContactList, string.Empty);
      }

      // Rolling up changes in ICH member status 
      var memberData = MemberRepository.Single(m => m.Id == memberId);
      memberData.IchMemberStatusId = ichConfiguration.IchMemberShipStatusId;

      MemberRepository.Update(memberData);

      // Commit all ICH configuration changes, audit entries, special cases.
      UnitOfWork.CommitDefault();

            // Send email for immediate updates.
            SendImmediateUpdatesEmail(memberFutureUpdatesList);
          
    

     // Call InsertMessageInOracleQueue() method which will insert message in Oracle queue
    if (ichConfiguration.IchMemberShipStatusId != (int)IchMemberShipStatus.NotAMember || isFutureUpdateLive)
    {
      /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
       * Desc: Passing ICH Membership status for this member. */
      InsertMessageInOracleQueue("MemberProfileUpdate", memberId, ichMembershipStatusId: ichConfiguration.IchMemberShipStatusId, isFutureUpdateLive: isFutureUpdateLive);
      SendMailToIchForMemberProfileUpdate(memberId);
    }

   if (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended)
      {
        if (ichConfiguration.DefaultSuspensionDate != null)
        {
          if (ichConfiguration.StatusChangedDate != null)
          {
            InsertSuspendedMemberMessageInOracleQueue(memberId, (DateTime)ichConfiguration.DefaultSuspensionDate,
                                                      (DateTime)ichConfiguration.StatusChangedDate,
                                                      IsDualMember(memberId) ? "IB" : "I");
          }
        }
      }

      //  IchUpdateHandler.GenerateXMLforICHUpdates(memberId);
      return ichConfiguration;
    }

    /// <summary>
    /// Updates the ach configuration.
    /// </summary>
    /// <param name="memberId">The id of the member.</param>
    /// <param name="achConfiguration">The ach configuration.</param>
    /// <returns></returns>
    public AchConfiguration UpdateAchConfiguration(int memberId, AchConfiguration achConfiguration)
    {
      // Set the member id for the ach configuration.
      achConfiguration.MemberId = memberId;

      var updateResult = true;
      var auditProcessor = new AuditProcessor<AchConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList;

      // Check whether ach configuration exists already.
      var achConfigInDb = AchRepository.Single(ach => ach.MemberId == memberId);

      if (achConfigInDb == null)
      {
        // Add ACH config to the repository.
        AchRepository.Add(achConfiguration);

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.AchConfiguration, achConfigInDb, achConfiguration, UserId);

        //Send notification completion of configuration of Ich/Ach elements to IS ops.
        var memberDetails = GetMemberDetails(achConfiguration.MemberId);
        SendCompletionOfIchAchSpecificElementsForMemberNotification(memberDetails.MemberCodeAlpha + "-" + memberDetails.MemberCodeNumeric + "-" + memberDetails.CommercialName, "ACH");
      }
      else
      {
        // Get the list of pending future updates.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(memberId, ElementGroupType.AchConfiguration);

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.AchConfiguration, achConfigInDb, achConfiguration, UserId, pendingUpdates);

        // Update ACH config in the repository.
        AchRepository.Update(achConfiguration);
      }
      // Update exception data set for passenger category
      if (!string.IsNullOrEmpty(achConfiguration.PaxExceptionMemberAddList) || !string.IsNullOrEmpty(achConfiguration.PaxExceptionMemberDeleteList))
      {
        updateResult = UpdateACHExceptionMembers(memberId,
                                                 achConfiguration.PaxExceptionMemberAddList,
                                                 achConfiguration.PaxExceptionMemberDeleteList,
                                                 (int)BillingCategoryType.Pax,
                                                 achConfiguration.PaxExceptionFuturePeriod,
                                                 ref memberFutureUpdatesList);
      }

      if (updateResult && (!string.IsNullOrEmpty(achConfiguration.CgoExceptionMemberAddList) || !string.IsNullOrEmpty(achConfiguration.CgoExceptionMemberDeleteList)))
      {
        updateResult = UpdateACHExceptionMembers(memberId,
                                                 achConfiguration.CgoExceptionMemberAddList,
                                                 achConfiguration.CgoExceptionMemberDeleteList,
                                                 (int)BillingCategoryType.Cgo,
                                                 achConfiguration.CgoExceptionFuturePeriod,
                                                 ref memberFutureUpdatesList);
      }

      if (updateResult && (!string.IsNullOrEmpty(achConfiguration.MiscExceptionMemberAddList) || !string.IsNullOrEmpty(achConfiguration.MiscExceptionMemberDeleteList)))
      {
        updateResult = UpdateACHExceptionMembers(memberId,
                                                 achConfiguration.MiscExceptionMemberAddList,
                                                 achConfiguration.MiscExceptionMemberDeleteList,
                                                 (int)BillingCategoryType.Misc,
                                                 achConfiguration.MiscExceptionFuturePeriod,
                                                 ref memberFutureUpdatesList);
      }

      if (updateResult && (!string.IsNullOrEmpty(achConfiguration.UatpExceptionMemberAddList) || !string.IsNullOrEmpty(achConfiguration.UatpExceptionMemberDeleteList)))
      {
        updateResult = UpdateACHExceptionMembers(memberId,
                                                 achConfiguration.UatpExceptionMemberAddList,
                                                 achConfiguration.UatpExceptionMemberDeleteList,
                                                 (int)BillingCategoryType.Uatp,
                                                 achConfiguration.UatpExceptionFuturePeriod,
                                                 ref memberFutureUpdatesList);
      }

      if (updateResult == false)
      {
        throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
      }

      try
      {
        if (achConfigInDb != null)
        {
          // If membership status is changed and future status value is not set as Terminated then add immediate update record to database
          if ((achConfigInDb.AchMembershipStatusId != achConfiguration.AchMembershipStatusId) && (achConfiguration.AchMembershipStatusIdFutureValue == null))
          {
            AddAuditTrailForImmediateUpdates(ElementGroupType.AchConfiguration,
                                             memberId,
                                             "ACH_MEMBERSHIP_STATUS",
                                             achConfigInDb.AchMembershipStatusId.ToString(),
                                             achConfiguration.AchMembershipStatusId.ToString(),
                                             (int)ActionType.Update,
                                             null,
                                             null,
                                             null,
                                             ref memberFutureUpdatesList);
          }
          // Set future updates for Termination Date if future values are set by user
          else if ((achConfigInDb.AchMembershipStatusIdFuturePeriod != achConfiguration.AchMembershipStatusIdFuturePeriod) && (achConfiguration.AchMembershipStatusIdFutureValue != null))
          {
            UpdateFutureUpdates(ElementGroupType.AchConfiguration,
                                memberId,
                                "ACH_MEMBERSHIP_STATUS",
                                achConfigInDb.AchMembershipStatusId.ToString(),
                                achConfiguration.AchMembershipStatusIdFutureValue.ToString(),
                                achConfiguration.AchMembershipStatusIdFuturePeriod,
                                null,
                                (int)ActionType.Update,
                                false,
                                null,
                                achConfiguration.AchMembershipStatusIdDisplayValue,
                                achConfiguration.AchMembershipStatusIdFutureDisplayValue,
                                ref memberFutureUpdatesList);
          }
        }
        else
        {
          //CMP-689-Flexible CH Activation Options
          //update futureupdate record here when member is not having Ach membership (AchMemberShipStatus changed from Not A member to Live)
          if (achConfiguration.AchMembershipStatusIdFuturePeriod != null && achConfiguration.AchMembershipStatusIdFutureValue != null)
          {
            UpdateFutureUpdates(ElementGroupType.AchConfiguration,
                                memberId,
                                "ACH_MEMBERSHIP_STATUS",
                                achConfiguration.AchMembershipStatusId.ToString(),
                                achConfiguration.AchMembershipStatusIdFutureValue.ToString(),
                                achConfiguration.AchMembershipStatusIdFuturePeriod,
                                null,
                                (int)ActionType.Update,
                                false,
                                null,
                                achConfiguration.AchMembershipStatusIdDisplayValue,
                                achConfiguration.AchMembershipStatusIdFutureDisplayValue,
                                ref memberFutureUpdatesList);
          }
        }
      }
      catch (ISBusinessException)
      {
        throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
      }

      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(memberFutureUpdatesList);



      //Add future update entry if Reinstatement Period value is entered
      if (achConfiguration.AchMembershipStatusId == (int)AchMembershipStatus.Suspended)
      {
        if (achConfiguration.ReinstatementPeriod != null)
        {
          UpdateFutureUpdates(ElementGroupType.AchConfiguration,
                              memberId,
                              "REINSTATEMENT_PERIOD",
                              ((int)AchMembershipStatus.Suspended).ToString(),
                              ((int)AchMembershipStatus.Live).ToString(),
                              achConfiguration.ReinstatementPeriod.ToString(),
                              null,
                              (int)ActionType.Update,
                              false,
                              null,
                              Enum.GetName(typeof(AchMembershipStatus), achConfiguration.AchMembershipStatusId),
                              Enum.GetName(typeof(AchMembershipStatus), (int)AchMembershipStatus.Live),
                              ref memberFutureUpdatesList);
        }
        else
        {
          // if Reinstatement period was already specified and value is deleted from UI, then existing entry for future update should be removed
          var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ach, memberId, "REINSTATEMENT_PERIOD", null);

          if (futureUpdatesRecord != null && futureUpdatesRecord.Count > 0)
          {
            FutureUpdatesRepository.Delete(futureUpdatesRecord[0]);
          }
        }
      }

      // CMP-689-Flexible CH Activation Options
      // before this CMP only Members having ‘ACH Membership Status’  as "Live", "Suspended" or "Terminated" are included in the update XML
      // With this CMP, the system should also consider the following:
      //  a.	Members whose future value of ‘ACH Membership Status’ = “Live” (and where the current value = “Not a Member” or "Terminated")
      var isFutureUpdateLive = (achConfiguration != null &&
                                   achConfiguration.AchMembershipStatusIdFutureValue == (int)IchMemberShipStatus.Live &&
                                   (achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.NotAMember ||
                                    achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Terminated))
                                      ? true
                                      : false;

      // Add Member Status Details
      var updatedStatusDate = new DateTime();

      if (achConfiguration.TerminationDate != null && achConfiguration.AchMembershipStatusId == (int)AchMembershipStatus.Terminated)
      {
        updatedStatusDate = achConfiguration.TerminationDate.Value;
      }

      if (achConfiguration.EntryDate != null && (achConfiguration.AchMembershipStatusId == (int)AchMembershipStatus.Live || isFutureUpdateLive)) // CMP-689-Flexible CH Activation Options
      {
        updatedStatusDate = achConfiguration.EntryDate.Value;
      }

      if (achConfiguration.StatusChangedDate != null && achConfiguration.AchMembershipStatusId == (int)AchMembershipStatus.Suspended)
      {
        updatedStatusDate = achConfiguration.StatusChangedDate.Value;
      }

      if (achConfiguration.AchMembershipStatusId == (int)AchMembershipStatus.NotAMember && !isFutureUpdateLive) //CMP-689-Flexible CH Activation Options
      {
        updatedStatusDate = DateTime.UtcNow.Date;
      }

      if (achConfiguration.AchMembershipStatusId != (int)IchMemberShipStatus.Suspended)
      {
        achConfiguration.DefaultSuspensionDate = null;
      }

      UpdateAchMemberStatusHistory(updatedStatusDate, achConfiguration, achConfiguration.DefaultSuspensionDate);

      if (achConfiguration.ContactList != null)
      {
        UpdateContactContactTypeMatrix(achConfiguration.ContactList, string.Empty);
      }

      // Rolling up changes in ACH member status 
      var memberData = MemberRepository.Single(m => m.Id == memberId);
      memberData.AchMemberStatusId = achConfiguration.AchMembershipStatusId;

      MemberRepository.Update(memberData);


      // Commit all ach configuration changes, audit entries, special cases.
      UnitOfWork.CommitDefault();

      // Send email for immediate updates.
      SendImmediateUpdatesEmail(memberFutureUpdatesList);

      
      // Call InsertMessageInOracleQueue() method which will insert message in Oracle queue
      if (achConfiguration.AchMembershipStatusId != (int)AchMembershipStatus.NotAMember || isFutureUpdateLive)
      {
          /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
           * Desc: Passing ACH membership status for this member. */
        InsertMessageInOracleQueue("MemberProfileUpdate", memberId, achMembershipStatusId: achConfiguration.AchMembershipStatusId, isFutureUpdateLive: isFutureUpdateLive);
      }

      if (achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Suspended)
      {
        if (achConfiguration.DefaultSuspensionDate != null)
        {
          if (achConfiguration.StatusChangedDate != null)
          {
            InsertSuspendedMemberMessageInOracleQueue(memberId, (DateTime)achConfiguration.DefaultSuspensionDate,
                                                      (DateTime)achConfiguration.StatusChangedDate,
                                                      IsDualMember(memberId) ? "AB" : "A");
          }
        }
      }
      //IchUpdateHandler.GenerateXMLforACHUpdates(memberId);
      return achConfiguration;
    }

    /// <summary>
    /// Updates the member location.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="location">The location.</param>
    public Location UpdateMemberLocation(int memberId, Location location)
    {
      // Trim all address related fields.
      if(!string.IsNullOrEmpty(location.TaxVatRegistrationNumber))
        location.TaxVatRegistrationNumber = location.TaxVatRegistrationNumber.Trim();
      if (!string.IsNullOrEmpty(location.AdditionalTaxVatRegistrationNumber))
        location.AdditionalTaxVatRegistrationNumber = location.AdditionalTaxVatRegistrationNumber.Trim();
      if (!string.IsNullOrEmpty(location.AddressLine1))
        location.AddressLine1 = location.AddressLine1.Trim();
      if (!string.IsNullOrEmpty(location.AddressLine2))
        location.AddressLine2 = location.AddressLine2.Trim();
      if (!string.IsNullOrEmpty(location.AddressLine3))
        location.AddressLine3 = location.AddressLine3.Trim();
      if (!string.IsNullOrEmpty(location.CityName))
        location.CityName = location.CityName.Trim();
      if (!string.IsNullOrEmpty(location.PostalCode))
        location.PostalCode = location.PostalCode.Trim();

      // Set the member id for the location.
      location.MemberId = memberId;

      //Get subdivision code corresponding to subdivision name for location
      if (location.SubDivisionName != null)
      {
        var subdivisionRecord = SubdivisionRepository.Single(subdiv => subdiv.Name == location.SubDivisionName && subdiv.CountryId == location.CountryId);

        location.SubDivisionCode = subdivisionRecord != null ? subdivisionRecord.Id : null;
      }

      var auditProcessor = new AuditProcessor<Location>();
      List<FutureUpdates> memberFutureUpdatesList;
      // Check whether location details exist already
      var locationRecordInDb = LocationRepository.Single(locationDetails => locationDetails.MemberId == memberId && locationDetails.Id == location.Id);

      if (locationRecordInDb == null)
      {
        if (location.IsUatpLocation)
        {
          location.LocationCode = UatpLocation;
          location.IsActive = true;
        }

        if (location.LocationCode == null)
        {
          location.LocationCode = GetLocationCode(location.CityName, location.MemberId);
        }

        // Add the location and commit (location needs to be committed first since we need the location id for the relation id field).
        LocationRepository.Add(location);

        // Commented written but not followed:Add the location and commit (location needs to be committed first since we need the location id for the relation id field).
        // CMP#597 : After location add, commit required for location id
        UnitOfWork.CommitDefault();
        
        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(location.MemberId, ActionType.Create, ElementGroupType.Locations, null, location, UserId);
      }
      else
      {
        // TODO: Populate LocationId on UI when details of location are viewed so that we do not have to use LocationCode for retrieving LocationID

        if (locationRecordInDb.LocationCode == MainLocation || locationRecordInDb.LocationCode == UatpLocation)
        {
          location.IsActive = true;
        }
        location.Id = locationRecordInDb.Id;
        location.LocationCode = locationRecordInDb.LocationCode;
        if (location.IsUatpLocation)
        {
          location.LocationCode = UatpLocation;
        }
        else
        {
          if ((locationRecordInDb.LocationCode != MainLocation) && (locationRecordInDb.CityName != location.CityName) && (locationRecordInDb.IsUatpLocation != location.IsUatpLocation))
          {
            location.LocationCode = GetLocationCode(location.CityName, location.MemberId);
          }
        }

        if ((locationRecordInDb.LocationCode == UatpLocation) && (!location.IsUatpLocation))
        {
          location.LocationCode = GetLocationCode(location.CityName, location.MemberId);
        }

        // Get the list of pending future updates for Location.
        var pendingLocationUpdates = FutureUpdatesManager.GetFutureUpdatesList(location.MemberId, (int)ElementGroupType.Locations, "MEM_LOCATION", null, location.Id);
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(location.MemberId, ActionType.Update, ElementGroupType.Locations, locationRecordInDb, location, UserId, pendingLocationUpdates);

          // Update the location in the repository.
        LocationRepository.Update(location);
      }


      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(memberFutureUpdatesList);

      // Commit location and audit entries.
      UnitOfWork.CommitDefault();

      // Send email for immediate updates.
      SendImmediateUpdatesEmail(memberFutureUpdatesList);
      // When a new location is added, send email to Members for which the profile element "Other Ptcpt Ref Fields Change Notification" is set
      // Get all locations for the member
      List<Location> memberLocationList = GetMemberLocationList(memberId);

      // TODO:Code for sending email alert and CSV report containing location details
     return location;
    }

    /// <summary>
    /// Updates the contact.
    /// </summary>
    /// /// <param name="memberId">Member Id in session for which contact should be added</param>
    /// <param name="contact">Contact class object</param>
    /// <returns>updated contact class object</returns>
    public Contact UpdateContact(int memberId, Contact contact)
    {
      //Read value for ACH status and ICH Status of the member
      var insertMessageinMessageQueue = false;
      var ichConfiguration = GetIchConfig(memberId);

      // Set the member id for the contact.
      contact.MemberId = memberId;
      //Get subdivision code corresponding to subdivision name for location
      if (contact.SubDivisionName != null)
      {
        var subdivisionRecord = SubdivisionRepository.Single(subdiv => subdiv.Name == contact.SubDivisionName);

        contact.SubDivisionCode = subdivisionRecord != null ? subdivisionRecord.Id : null;
      }

      var auditProcessor = new AuditProcessor<Contact>();
      List<FutureUpdates> memberFutureUpdatesList;

      // Check whether contact details exist already
      var contactRecordInDb = ContactRepository.Single(contactDetails => contactDetails.Id == contact.Id);

      if (contactRecordInDb == null)
      {
        // Check for duplicate same member.
        // if C1 is contact of M1 then C1 can be contact of M2
        //SCP344445: Member profile - UT Air
        if (ContactRepository.GetCount(contacts => contacts.EmailAddress.ToLower() == contact.EmailAddress.ToLower() && contacts.MemberId == contact.MemberId) > 0)

        // Check for duplicate within all contacts data.
        //if (ContactRepository.GetCount(contacts => contacts.EmailAddress.ToLower() == contact.EmailAddress.ToLower()) != 0)
        {
          throw new ISBusinessException(ErrorCodes.DuplicateEmailIdFound);
        }

        // Add the contact and commit. (Need to commit contact first since we need the contact id for the relation id.)
        ContactRepository.Add(contact);
        UnitOfWork.CommitDefault();

           // CMP#655IS-WEB DISPLAY PER LOCATION
          // Inherit user assigned location association and assigned to new conatact incase conatact is aleady a IS USER
           var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager)); // IOC resolve for interface
          contact.IsContactIsUser = memberLocation.InsertLocationAssociation("", "", contact.Id, "",
                                                                             contact.EmailAddress, 0, contact.MemberId,
                                                                             1);


        // Send update to ICH if the member is Live ICH member and new contact is added/edited/updated
        //Read value for ACH status and ICH Status of the member)
        if ((ichConfiguration != null) && (ichConfiguration.IchMemberShipStatusId != 4))
        {
          insertMessageinMessageQueue = true;
        }

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(contact.MemberId, ActionType.Create, ElementGroupType.Contacts, null, contact, UserId);
      }
      else
      {
        // Validation checks.
        //SCP344445: Member profile - UT Air
        if (contact.EmailAddress.ToLower() != contactRecordInDb.EmailAddress.ToLower() && ContactRepository.Get(contacts => contacts.EmailAddress.ToLower() == contact.EmailAddress.ToLower() && contacts.MemberId == memberId).Count() > 0)
        {
          throw new ISBusinessException(ErrorCodes.DuplicateEmailIdFound);
        }

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(contact.MemberId, ActionType.Update, ElementGroupType.Contacts, contactRecordInDb, contact, UserId);

        // Update the contact in the repository.
        ContactRepository.Update(contact);

        // Send update to ICH if the member is Live ICH member and new contact is added/edited/updated
        //Read value for ACH status and ICH Status of the member
        if ((ichConfiguration != null) && (ichConfiguration.IchMemberShipStatusId != 4))
        {
          insertMessageinMessageQueue = true;
        }
      }

      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(memberFutureUpdatesList);

      // Commit contact and audit entries.
      UnitOfWork.CommitDefault();




      // Call InsertMessageInOracleQueue() method which will insert message in Oracle queue
      if (insertMessageinMessageQueue)
      {
          /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
           * Desc: Passing ICH Membership status for this member. */
          InsertMessageInOracleQueue("MemberProfileUpdate", memberId, ichMembershipStatusId: ichConfiguration != null ? ichConfiguration.IchMemberShipStatusId : -1);
        SendMailToIchForMemberProfileUpdate(memberId);
      }

      // Send email for immediate updates.
      SendImmediateUpdatesEmail(memberFutureUpdatesList);

        return contact;
    }

    public void UpdateContactEmailId(string newEmailId, string oldEmailId, int memberId)
    {
      //SCP333083: XML Validation Failure for A30-XB - SIS Production
      var contactDetails = ContactRepository.Single(con => con.EmailAddress == oldEmailId && con.MemberId == memberId);
      if (contactDetails != null)
      {
        contactDetails.EmailAddress = newEmailId;
        ContactRepository.Update(contactDetails);
        UnitOfWork.CommitDefault();
        //SCP#409719 - ICH Contacts
        //Desc: Hooked a call to MemberManager.InsertMessageInOracleQueue() to quque member for ICH Profile Update XML Generation.  
        InsertMessageInOracleQueue("MemberProfileUpdate", memberId);
      }
      else
      {
        IsUserEmailIdInMemberContact(newEmailId, memberId);
      }
    }

    /// <summary>
    /// Updates the technical configuration.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="technicalConfiguration">The technical configuration.</param>
    public TechnicalConfiguration UpdateTechnicalConfiguration(int memberId, TechnicalConfiguration technicalConfiguration)
    {
      // Set the member id for the technical configuration.
      technicalConfiguration.MemberId = memberId;

      // Check whether technical configuration record already exists in database
      var technicalRecordInDb = TechnicalRepository.Single(technical => technical.MemberId == memberId);
      var auditProcessor = new AuditProcessor<TechnicalConfiguration>();
      List<FutureUpdates> memberFutureUpdatesList = null;


      if (technicalRecordInDb == null)
      {
        // Create the technical configuration.
        TechnicalRepository.Add(technicalConfiguration);

        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Create, ElementGroupType.Technical, technicalRecordInDb, technicalConfiguration, UserId);
      }
      else
      {
        // Ascertain the audit trail entries.
        memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.Technical, technicalRecordInDb, technicalConfiguration, UserId);

        // Update the technical config to the database.
        TechnicalRepository.Update(technicalConfiguration);
      }

      // Process the objects and generate the audit entries.
      AddUpdateAuditEntries(memberFutureUpdatesList);

      // Commit audit and technical configuration.
      UnitOfWork.CommitDefault();

      //CMP #625: New Fields in ICH Member Profile Update XML.
      //Added entry in the queue "aq_ich_pro_upd_int_tab" for send update xml to ICH.
      var ichConfiguration = GetIchConfig(memberId);
      var achConfiguration = GetAchConfig(memberId);
      //Send update to ICH if the member is Live ICH member and ACH member.
      if (((ichConfiguration != null) && (ichConfiguration.IchMemberShipStatusId != 4)) || ((achConfiguration != null) && (achConfiguration.AchMembershipStatusId != 4)))
          /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
           * Desc: Passing ICH Membership status and ACH membership status for this member. */
          InsertMessageInOracleQueue("MemberProfileUpdate", memberId,
                                     ichMembershipStatusId:
                                         ichConfiguration != null ? ichConfiguration.IchMemberShipStatusId : -1,
                                     achMembershipStatusId:
                                         achConfiguration != null ? achConfiguration.AchMembershipStatusId : -1);

      // If the object has changed then send the updates through an email.
      SendImmediateUpdatesEmail(memberFutureUpdatesList);

      return technicalConfiguration;
    }

    /// <summary>
    /// Sets Future update class object
    /// </summary>
    /// <param name="id">future update record id (in case of update)</param>
    /// <param name="memberId">ID of member</param>
    /// <param name="elementGroupType"></param>
    /// <param name="elementName">Name of element whose value is changed</param>
    /// <param name="oldValue">old value of element</param>
    /// <param name="newValue">new value of element</param>
    /// <param name="changeEffectivePeriod">period value when change will be effective</param>
    /// <param name="changeEffectiveDate">date value when change will be effective</param>
    /// <param name="actionId">Denotes action add or update</param>
    /// <param name="isChangeApplied">Denotes whether update is effective immediately</param>
    /// <param name="relationId"></param>
    /// <returns>FutureUpdates class object</returns>
    private FutureUpdates GetFutureUpdateObject(int id,
                                                int memberId,
                                                ElementGroupType elementGroupType,
                                                string elementName,
                                                string oldValue,
                                                string newValue,
                                                string changeEffectivePeriod,
                                                string changeEffectiveDate,
                                                int actionId,
                                                bool isChangeApplied,
                                                int? relationId,
                                                string oldValueDisplayName,
                                                string newValueDisplayName)
    {
      var futureUpdate = new FutureUpdates();
      try
      {
        if (id != 0)
        {
          futureUpdate.Id = id;
        }

        futureUpdate.MemberId = memberId;
        futureUpdate.ElementGroupTypeId = (int)elementGroupType;

        // get an instance of element group repository
        var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));
        var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)elementGroupType);
        futureUpdate.TableName = elementGroup.FirstOrDefault().TableName;
        futureUpdate.DisplayGroup = elementGroup.FirstOrDefault().Group;

        futureUpdate.ElementName = elementName;
        futureUpdate.OldVAlue = oldValue;
        futureUpdate.NewVAlue = newValue;
        futureUpdate.BilateralMemberId = null;
        futureUpdate.RelationId = relationId;

        if ((changeEffectivePeriod != null) && (!changeEffectivePeriod.Equals("YYYY-MMM-PP")))
        {
          futureUpdate.ChangeEffectivePeriod = Convert.ToDateTime(changeEffectivePeriod);
        }

        if (changeEffectiveDate != null)
        {
          futureUpdate.ChangeEffectiveOn = Convert.ToDateTime(changeEffectiveDate);
        }

        futureUpdate.IsChangeApplied = isChangeApplied;

        // if old or oldValueDisplayName/oldValueDisplayName is True then set Yes, if False set No in DB
        if (newValue == bool.TrueString)
        {
          newValueDisplayName = "Yes";
          oldValueDisplayName = "No";
        }
        else if (newValue == bool.FalseString)
        {
          newValueDisplayName = "No";
          oldValueDisplayName = "Yes";
        }

        if (!string.IsNullOrEmpty(oldValueDisplayName))
        {
          futureUpdate.OldValueDisplayName = oldValueDisplayName;
        }
        else
        {
          futureUpdate.OldValueDisplayName = oldValue;
        }

        if (!string.IsNullOrEmpty(newValueDisplayName))
        {
          futureUpdate.NewValueDisplayName = newValueDisplayName;
        }
        else
        {
          futureUpdate.NewValueDisplayName = newValue;
        }

    futureUpdate.ActionTypeId = actionId;

        // TODO:This hard coding should be removed once logged in user is stored
        futureUpdate.LastUpdatedBy = UserId;
        futureUpdate.LastUpdatedOn = DateTime.UtcNow;
        futureUpdate.ModifiedOn = DateTime.UtcNow;
      }
      catch (ISBusinessException ex)
      {
        _logger.Debug(string.Format("In Error : {0}-{1}", ex.ErrorCode, ex.InnerException));
      }

      return futureUpdate;
    }
    /// <summary>
    /// Terminates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    public bool TerminateMember(Member member)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Gets member id from member code.
    /// </summary>
    public int GetMemberId(string memberCode)
    {
      if (memberCode.Length > 0)
      {
        return MemberRepository.GetMemberId(memberCode);
      }

      return 0;
    }

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    public Member GetMemberDetails(int memberId)
    {
      Member memberDetail = null;
      if (memberId > 0)
      {
        // Get details of member record
        var memberRepository = Ioc.Resolve<IMemberRepository>();
        memberDetail = memberRepository.GetMember(memberId);
      }

     return memberDetail;
    }

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    public Member GetMember(int memberId, bool includeFutureUpdates = false)
    {
      Member memberDetail = null;
      if (memberId > 0)
      {
        // Get details of member record
        memberDetail = MemberRepository.GetMember(memberId);

       // Get details of default location.
        if (memberDetail != null)
        {
            if(memberDetail.ParentMemberId > 0)
            {
                var parentMember = GetMemberDetails(memberDetail.ParentMemberId);
                if (parentMember != null)
                    memberDetail.ParentMember = parentMember;
            }
            
          var defaultLocationDetail = LocationRepository.Single(location => location.MemberId == memberId && location.LocationCode == MainLocation);


          if (defaultLocationDetail != null)
          {
            // Assign default location to member
            memberDetail.DefaultLocation = defaultLocationDetail;
          }

          // Read value for ACH status and ICH Status of the member
          var ichConfiguration = GetIchConfig(memberId, includeFutureUpdates);

          if (ichConfiguration != null)
          {
            switch (ichConfiguration.IchMemberShipStatusId)
            {
              case (int)IchMemberShipStatus.Live:
              case (int)IchMemberShipStatus.Suspended:
                memberDetail.IchMemberStatus = true;
                break;

          case (int)IchMemberShipStatus.Terminated:
              case (int)IchMemberShipStatus.NotAMember:
                memberDetail.IchMemberStatus = false;
                break;
            }
          }
          else
          {
            memberDetail.IchMemberStatus = false;
          }

          var achConfiguration = GetAchConfig(memberId, includeFutureUpdates);
          if (achConfiguration != null)
          {
            switch (achConfiguration.AchMembershipStatusId)
            {
              case (int)AchMembershipStatus.Live:
              case (int)AchMembershipStatus.Suspended:
                memberDetail.AchMemberStatus = true;
                break;

              case (int)AchMembershipStatus.Terminated:
              case (int)AchMembershipStatus.NotAMember:
                memberDetail.AchMemberStatus = false;
                break;
            }
          }
          else
          {
            memberDetail.AchMemberStatus = false;
          }

          if (memberDetail.DefaultLocation == null)
          {
            return memberDetail;
          }

          // Only include future updates if they are required.
          if (includeFutureUpdates)
          {
            var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Locations, "MEM_LOCATION", null, memberDetail.DefaultLocation.Id);
            if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
            {
              foreach (var futureUpdate in futureUpdatesData.AsQueryable())
              {
                if (futureUpdate.ElementName == "REGISTRATION_ID")
                {
                  memberDetail.DefaultLocation.RegistrationIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.RegistrationIdFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "TAX_VAT_REGISTRATION_NUMBER")
                {
                  memberDetail.DefaultLocation.TaxVatRegistrationNumberFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.TaxVatRegistrationNumberFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "ADD_TAX_VAT_REGISTRATION_NUM")
                {
                  memberDetail.DefaultLocation.AdditionalTaxVatRegistrationNumberFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.AdditionalTaxVatRegistrationNumberFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "ADDRESS_LINE1")
                {
                  memberDetail.DefaultLocation.AddressLine1FuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.AddressLine1FutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "ADDRESS_LINE2")
                {
                  memberDetail.DefaultLocation.AddressLine2FuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.AddressLine2FutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "ADDRESS_LINE3")
                {
                  memberDetail.DefaultLocation.AddressLine3FuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.AddressLine3FutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "CITY_NAME")
                {
                  memberDetail.DefaultLocation.CityNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.CityNameFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "SUB_DIVISION_NAME")
                {
                  memberDetail.DefaultLocation.SubDivisionNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.SubDivisionNameFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "POSTAL_CODE")
                {
                  memberDetail.DefaultLocation.PostalCodeFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.PostalCodeFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "COUNTRY_CODE")
                {
                  memberDetail.DefaultLocation.CountryIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.CountryIdFutureValue = futureUpdate.NewVAlue;
                  memberDetail.DefaultLocation.CountryIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                }
                else if (futureUpdate.ElementName == "MEMBER_LEGAL_NAME")
                {
                  memberDetail.DefaultLocation.MemberLegalNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.MemberLegalNameFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
                else if (futureUpdate.ElementName == "IS_ACTIVE")
                {
                  memberDetail.DefaultLocation.IsActiveFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DefaultLocation.IsActiveFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
              }
            }
            var futureUpdatesMemberData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.MemberDetails, "MEMBER_DETAILS", null, null);
            if ((futureUpdatesMemberData != null) && (futureUpdatesMemberData.Count != 0))
           {
              foreach (var futureUpdate in futureUpdatesMemberData.AsQueryable())
              {
                if (futureUpdate.ElementName == "IS_MEMBERSHIP_STATUS")
                {
                  memberDetail.IsMembershipStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.IsMembershipStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                  memberDetail.IsMembershipStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                }
                if (futureUpdate.ElementName == "DIGITAL_SIGN_APPLICATION")
                {
                  memberDetail.DigitalSignApplicationFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DigitalSignApplicationFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }

                if (futureUpdate.ElementName == "DIGITAL_SIGN_VERIFICATION")
                {
                  memberDetail.DigitalSignVerificationFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.DigitalSignVerificationFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }

                if (futureUpdate.ElementName == "IS_UATP_INV_HANDLED_BY_ATCAN")
                {
                  memberDetail.UatpInvoiceHandledbyAtcanFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.UatpInvoiceHandledbyAtcanFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }
                if (futureUpdate.ElementName == "IS_MERGED")
                {
                    memberDetail.IsMergedFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                    if (futureUpdate.NewVAlue!=null)
                    {
                        memberDetail.IsMergedFutureValue = bool.Parse(futureUpdate.NewVAlue);
                    }
                }
                if (futureUpdate.ElementName == "PARENT_MEMBER_ID")
                {
                    memberDetail.ParentMemberIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                    if (futureUpdate.NewVAlue != null)
                    {
                        memberDetail.ParentMemberIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                        memberDetail.ParentMemberIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                    }
                }
                if (futureUpdate.ElementName == "MERGER_DATE")
                {
                    memberDetail.ActualMergerDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                    memberDetail.ActualMergerDateFutureValue =(futureUpdate.NewVAlue);
                }
                // CMP597: TFS_Bug_8930 IS WEB -Memebr legal name is not a future update field from SIS ops login
                if(futureUpdate.ElementName == "MEMBER_LEGAL_NAME")
                {
                  memberDetail.LegalNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  memberDetail.LegalNameFutureValue = Convert.ToString(futureUpdate.NewVAlue);
                }
              }
            }
          }
            if(memberDetail.ActualMergerDate !=null)
            {
                var dateTimeFieldVal = Convert.ToDateTime(memberDetail.ActualMergerDate);
                memberDetail.ActualMergerDate = dateTimeFieldVal.ToString("yyyy-MMM-dd");
            }
            if (memberDetail.ActualMergerDateFutureValue != null)
            {
                var dateTimeFieldVal = Convert.ToDateTime(memberDetail.ActualMergerDateFutureValue);
                memberDetail.ActualMergerDateFutureValue = dateTimeFieldVal.ToString("yyyy-MMM-dd");
            }
            if (memberDetail.ActualMergerDateFutureValue != null || memberDetail.ParentMemberIdFutureValue != null)
            {
                if (memberDetail.ParentMemberIdFutureValue !=0)
                memberDetail.IsMergedFutureValue = true;
            }
        }
      }
        
        return memberDetail;
    }

  /// <summary>
    /// Get Cargo Configuration
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    public CargoConfiguration GetCargoConfig(int memberId, bool includeFutureUpdates = false)
    {
      CargoConfiguration cargoConfigDetail = null;
      if (memberId > 0)
      {
        cargoConfigDetail = CargoRepository.Single(cargo => cargo.MemberId == memberId);

        // Only include future updates if they are required.
        if (includeFutureUpdates)
        {
          var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Cgo, "MEM_CGO_CONFIGURATION", null, null);

          if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesData.AsQueryable())
            {
              if (futureUpdate.ElementName == "IS_PDF_AS_OTHER_OUT_AS_BILLED")
              {
                cargoConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLED")
              {
                cargoConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_DS_FIL_AS_OTH_OUT_AS_BILLED")
              {
                cargoConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_MEMO_AS_OTH_OUT_AS_BILLED")
              {
                cargoConfigDetail.IsMemoAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsMemoAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_SUPP_AS_OTH_OUT_AS_BILLED")
              {
                cargoConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_PDF_AS_OTH_OUT_AS_BILLING")
              {
                cargoConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLING")
              {
                cargoConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_DS_AS_OTH_OUT_AS_BILLING")
              {
                cargoConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_MEMO_AS_OTH_OUT_AS_BILLING")
              {
                cargoConfigDetail.IsMemoAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                cargoConfigDetail.IsMemoAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "BILLING_INVOICE_IDEC_OUTPUT")
              {
                cargoConfigDetail.BillingInvoiceIdecOutputFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.BillingInvoiceIdecOutputFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "BILLING_INVOICE_XML_OUTPUT")
              {
                cargoConfigDetail.BillingInvoiceXmlOutputFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.BillingInvoiceXmlOutputFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "PRIME_ISIDEC_MIG_DATE")
              {
                cargoConfigDetail.PrimeBillingIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.PrimeBillingIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "PRIME_ISIDEC_MIG_STAT_ID")
              {
                cargoConfigDetail.PrimeBillingIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.PrimeBillingIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.PrimeBillingIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "PRIME_ISXML_MIG_DATE")
              {
                cargoConfigDetail.PrimeBillingIsxmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.PrimeBillingIsxmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "PRIME_ISXML_MIG_STAT_ID")
              {
                cargoConfigDetail.PrimeBillingIsxmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.PrimeBillingIsxmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.PrimeBillingIsxmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }

             if (futureUpdate.ElementName == "RM_ISIDEC_MIG_DATE")
              {
                cargoConfigDetail.RmIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.RmIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "RM_ISIDEC_MIG_STAT_ID")
              {
                cargoConfigDetail.RmIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.RmIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.RmIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }

              if (futureUpdate.ElementName == "RM_ISXML_MIG_DATE")
              {
                cargoConfigDetail.RmIsXmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.RmIsXmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "RM_ISXML_MIG_STAT_ID")
              {
                cargoConfigDetail.RmIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.RmIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.RmIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "BM_ISIDEC_MIG_DATE")
              {
                cargoConfigDetail.BmIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.BmIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "BM_ISIDEC_MIG_STAT_ID")
              {
                cargoConfigDetail.BmIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.BmIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.BmIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "BM_ISXML_MIG_DATE")
              {
                cargoConfigDetail.BmIsXmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.BmIsXmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "BM_ISXML_MIG_STAT_ID")
              {
                cargoConfigDetail.BmIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.BmIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.BmIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "CM_ISIDEC_MIG_DATE")
              {
                cargoConfigDetail.CmIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.CmIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "CM_ISIDEC_MIG_STAT_ID")
              {
                cargoConfigDetail.CmIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.CmIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.CmIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "CM_ISXML_MIG_DATE")
              {
                cargoConfigDetail.CmIsXmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.CmIsXmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "CM_ISXML_MIG_STAT_ID")
              {
                cargoConfigDetail.CmIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                cargoConfigDetail.CmIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                cargoConfigDetail.CmIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
            }
          }
        }
      }

      /*DEPENDENT FIELDS RELATED CHANGE START*/
      var memberDetail = MemberRepository.Single(mem => mem.Id == memberId);




      if (memberDetail != null)
      {
          if (cargoConfigDetail == null)
          {
              cargoConfigDetail = new CargoConfiguration {CgoOldIdecMember = memberDetail.CgoOldIdecMember};
          }
          else
          {
              cargoConfigDetail.CgoOldIdecMember = memberDetail.CgoOldIdecMember;
          }
      }
        /*DEPENDENT FIELDS RELATED CHANGE END*/
      return cargoConfigDetail;
    }

    /// <summary>
    /// Get Ach Configuration
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    public AchConfiguration GetAchConfig(int memberId, bool includeFutureUpdates = false)
    {
      AchConfiguration achConfigDetail = null;
      if (memberId > 0)
      {
        achConfigDetail = AchRepository.Single(ach => ach.MemberId == memberId);

        // Only include future updates if they are required.
        if (includeFutureUpdates)
        {
          var futureUpdatesExceptionsData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Ach, "MEM_ACH_EXCEPTION", null, null);

          if ((futureUpdatesExceptionsData != null) && (futureUpdatesExceptionsData.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesExceptionsData.AsQueryable())
            {
              if ((futureUpdate.ElementName == "EXCEPTION_MEMBER_ID") && (futureUpdate.RelationId == 1))
              {
                achConfigDetail.PaxExceptionFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                achConfigDetail.PaxExceptionFutureValue = futureUpdate.NewVAlue;
              }
              if ((futureUpdate.ElementName == "EXCEPTION_MEMBER_ID") && (futureUpdate.RelationId == 2))
              {
                achConfigDetail.CgoExceptionFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                achConfigDetail.CgoExceptionFutureValue = futureUpdate.NewVAlue;
              }
              if ((futureUpdate.ElementName == "EXCEPTION_MEMBER_ID") && (futureUpdate.RelationId == 4))
              {
                achConfigDetail.UatpExceptionFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                achConfigDetail.UatpExceptionFutureValue = futureUpdate.NewVAlue;
              }
              if ((futureUpdate.ElementName == "EXCEPTION_MEMBER_ID") && (futureUpdate.RelationId == 3))
              {
                achConfigDetail.MiscExceptionFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                achConfigDetail.MiscExceptionFutureValue = futureUpdate.NewVAlue;
              }
            }
          }

          var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.AchConfiguration, "MEM_ACH_CONFIGURATION", null, null);

          if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesData.AsQueryable())
            {
              if (futureUpdate.ElementName == "ACH_MEMBERSHIP_STATUS")
              {
                achConfigDetail.AchMembershipStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                achConfigDetail.AchMembershipStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                achConfigDetail.AchMembershipStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "REINSTATEMENT_PERIOD")
              {
                achConfigDetail.ReinstatementPeriod = Convert.ToDateTime(FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod));
              }
            }
          }

          // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV 
          // Get Future update record of Member Legal Name
          var futureMemberLegalName = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails, 0, "MEMBER_LEGAL_NAME", memberId.ToString(), null);
          if ((futureMemberLegalName != null) && (futureMemberLegalName.Count > 0))
          {
            achConfigDetail.MemberNameFutureValue = futureMemberLegalName[0].NewValueDisplayName;
            achConfigDetail.MemberNameChangePeriodFrom = FormatFuturePeriodValue(futureMemberLegalName[0].ChangeEffectivePeriod);
          }


        }
      }

      return achConfigDetail;
    }

   /// <summary>
    /// Gets only Ich configurations of member. (Without future update information.)
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public IchConfiguration GetIchDetails(int memberId)
    {
      IchConfiguration ichConfigDetail = null;

      if (memberId > 0)
      {
        // Get details of ICH configuration corresponding to member ID passed
        ichConfigDetail = IchRepository.Single(ich => ich.MemberId == memberId);
      }
      return ichConfigDetail;
    }

    /// <summary>
    /// Gets details of ICH configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which ICH configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns>IchConfiguration class object</returns>
    public IchConfiguration GetIchConfig(int memberId, bool includeFutureUpdates = false)
    {
      IchConfiguration ichConfigDetail = null;
      if (memberId > 0)
      {
        // Get details of ICH configuration corresponding to member ID passed
        ichConfigDetail = IchRepository.Single(ich => ich.MemberId == memberId);

        if (ichConfigDetail != null)
        {
          // Get data for member which is sponsoring currently selected member
          if (ichConfigDetail.SponsoredById != null)
          {
            var sposoredByMember = MemberRepository.Single(mem => mem.Id == ichConfigDetail.SponsoredById);

            if (sposoredByMember != null)
            {
              ichConfigDetail.SponsoredBy = sposoredByMember;
            }
          }

          // Get data for member which is aggregating currently selected member
          if (ichConfigDetail.AggregatedById != null)
          {
            var aggregatedByMember = MemberRepository.Single(mem => mem.Id == ichConfigDetail.AggregatedById);

            if (aggregatedByMember != null)
            {
              ichConfigDetail.AggregatedBy = aggregatedByMember;
            }
          }

          // Only include future updates if they are required.
          if (includeFutureUpdates)
          {
            var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Ich, "MEM_ICH_CONFIGURATION", null, null);

            if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
            {
              foreach (var futureUpdate in futureUpdatesData.AsQueryable())
              {
                if (futureUpdate.ElementName == "IS_AGGREGATOR")
                {
                  ichConfigDetail.IsAggregatorFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.IsAggregatorFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }

                if (futureUpdate.ElementName == "CAN_SUBMIT_PAX_IN_F12_FILES")
                {
                  ichConfigDetail.CanSubmitPaxInF12FilesFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.CanSubmitPaxInF12FilesFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }
                if (futureUpdate.ElementName == "CAN_SUBMIT_CGO_IN_F12_FILES")
                {
                  ichConfigDetail.CanSubmitCargoInF12FilesFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.CanSubmitCargoInF12FilesFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }

                if (futureUpdate.ElementName == "CAN_SUBMIT_MSC_IN_F12_FILES")
                {
                  ichConfigDetail.CanSubmitMiscInF12FilesFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.CanSubmitMiscInF12FilesFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }
                if (futureUpdate.ElementName == "CAN_SUBMIT_UATP_IN_F12_FILES")
                {
                  ichConfigDetail.CanSubmitUatpinF12FilesFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.CanSubmitUatpinF12FilesFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }
                if (futureUpdate.ElementName == "SPONSORED_BY")
                {
                  ichConfigDetail.SponseredByTextFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.SponseredByTextFutureValueId = Convert.ToInt16(futureUpdate.NewVAlue);

                  if (ichConfigDetail.SponseredByTextFutureValueId > 0)
                  {
                    var sposoredByMemberFuture = MemberRepository.Single(mem => mem.Id == ichConfigDetail.SponseredByTextFutureValueId);

                   if (sposoredByMemberFuture != null)
                    {
                      ichConfigDetail.SponsoredByFuture = sposoredByMemberFuture;
                 }
                  }
                }

            if (futureUpdate.ElementName == "AGGREGATED_BY")
                {
                  ichConfigDetail.AggregatedByTextFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.AggregatedByTextFutureValueId = Convert.ToInt16(futureUpdate.NewVAlue);

                  if (ichConfigDetail.AggregatedByTextFutureValueId > 0)
                  {
                    var aggregatedByMemberFuture = MemberRepository.Single(mem => mem.Id == ichConfigDetail.AggregatedByTextFutureValueId);


                    if (aggregatedByMemberFuture != null)
                    {
                      ichConfigDetail.AggregatedByFuture = aggregatedByMemberFuture;
                    }
                  }
                }
                if (futureUpdate.ElementName == "ZONE_ID")
                {
                  ichConfigDetail.IchZoneIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.IchZoneIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                  ichConfigDetail.IchZoneIdDisplayValue = futureUpdate.OldValueDisplayName;
                  ichConfigDetail.IchZoneIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                }
                if (futureUpdate.ElementName == "ICH_MEMBER_CATEGORY_ID")
                {
                  ichConfigDetail.IchCategoryIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.IchCategoryIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                  ichConfigDetail.IchCategoryIdDisplayValue = futureUpdate.OldValueDisplayName;
                  ichConfigDetail.IchCategoryIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                }

                if (futureUpdate.ElementName == "AGGREGATORLISTTOADD")
                {
                  ichConfigDetail.AggregatorFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.AggregatorFutureValue = futureUpdate.NewVAlue;
                }
                if (futureUpdate.ElementName == "AGGREGATED_TYPE")
                {
                  ichConfigDetail.AggregatedTypeIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.AggregatedTypeIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                  ichConfigDetail.AggregatedTypeIdDisplayValue = futureUpdate.OldValueDisplayName;
                  ichConfigDetail.AggregatedTypeIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                }
            
    if (futureUpdate.ElementName == "ICH_MEMBERSHIP_STATUS")
                {
                  ichConfigDetail.IchMemberShipStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ichConfigDetail.IchMemberShipStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                  ichConfigDetail.IchMemberShipStatusIdDisplayValue = futureUpdate.OldValueDisplayName;
                  ichConfigDetail.IchMemberShipStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
                }

                if (futureUpdate.ElementName == "REINSTATEMENT_PERIOD")
                {
                  ichConfigDetail.ReinstatementPeriod = Convert.ToDateTime(FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod));
                }
              }
            }
            // Get records where selected member is set as future sponsorer.
            // If future update record exist in database where passed memberId is a sponsoror then get all these records from database
            var futureSponsoredList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "SPONSORED_BY", memberId.ToString(), null);
            // Get future aggregated members for delete operation
            //ID : 43358 - Member profile XML from SIS to ICH - Aggregator Function
            futureSponsoredList.AddRange(FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "SPONSORED_BY", memberId.ToString(), null, Convert.ToString(memberId)));

            if ((futureSponsoredList != null) && (futureSponsoredList.Count > 0))
            {
              ichConfigDetail.SponsororFuturePeriod = FormatFuturePeriodValue(futureSponsoredList[0].ChangeEffectivePeriod);
            }


             // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV 
             // Get Future update record of Member Legal Name

            var futureMemberLegalName = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.MemberDetails, memberId, "MEMBER_LEGAL_NAME", null);
            if ((futureMemberLegalName != null) && (futureMemberLegalName.Count > 0))
            {
              ichConfigDetail.MemberNameFutureValue = futureMemberLegalName[0].NewValueDisplayName;
              ichConfigDetail.MemberNameChangePeriodFrom = FormatFuturePeriodValue(futureMemberLegalName[0].ChangeEffectivePeriod);
            }
            
            // Get records where selected member is set as future sponsorer.
            // If future update record exist in database where passed memberId is a sponsoror then get all these records from database
            var futureAggregatedList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "AGGREGATED_BY", memberId.ToString(), null);
            //ID : 43358 - Member profile XML from SIS to ICH - Aggregator Function
            // Get members those will be delete in future.
            futureAggregatedList.AddRange(FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0,
                                                                                        "AGGREGATED_BY",
                                                                                        memberId.ToString(),
                                                                                        null,
                                                                                        Convert.ToString(
                                                                                            memberId)));

            if ((futureAggregatedList != null) && (futureAggregatedList.Count > 0))
            {
              ichConfigDetail.AggregatorFuturePeriod = FormatFuturePeriodValue(futureAggregatedList[0].ChangeEffectivePeriod);
            }
          }
        }
      }

      return ichConfigDetail;
    }
    /// <summary>
    /// Gets details of passenger configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which passenger configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns>Passenger Configuration class object</returns>
    public PassengerConfiguration GetPassengerConfiguration(int memberId, bool includeFutureUpdates = false)
    {
      PassengerConfiguration passengerConfigDetail = null;

      if (memberId > 0)
      {
        //SCP#143361: RM loading Optimization 
        passengerConfigDetail = PassengerRepository.Single(pax => pax.MemberId == memberId);
        //SCP#000000: Null check added to avoid impact of SCP#143361
        //Desc: Implemented a null check before refreshing a object. This is added to stop getting an unexpected exception in cases when
        //      member does not have a pax config. 
        //Date: 18-July-2013
        if (passengerConfigDetail != null)
        {
            PassengerRepository.Refresh(passengerConfigDetail);
        }

          // Only include future updates if they are required.
        if (passengerConfigDetail != null && includeFutureUpdates)
        {
          var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Pax, "MEM_PAX_CONFIGURATION", null, null);
          if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesData.AsQueryable())
            {
              if (futureUpdate.ElementName == "IS_PARTICIPATE_IN_AUTO_BILLING")
              {
                passengerConfigDetail.IsParticipateInAutoBillingFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsParticipateInAutoBillingFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "ISIDEC_OUTPUT_FILE_VERSION")
              {
                passengerConfigDetail.IsIdecOutputVersionFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsIdecOutputVersionFutureValue = futureUpdate.NewVAlue;
              }
              if (futureUpdate.ElementName == "ISXML_OUTPUT_FILE_VERSION")
              {
                passengerConfigDetail.IsXmlOutputVersionFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsXmlOutputVersionFutureValue = futureUpdate.NewVAlue;
              }
              if (futureUpdate.ElementName == "IS_PDF_AS_OTHER_OUT_AS_BILLED")
              {
                passengerConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLED")
              {
                passengerConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_DS_FIL_AS_OTH_OUT_AS_BILLED")
              {
                passengerConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_MEMO_AS_OTH_OUT_AS_BILLED")
              {
                passengerConfigDetail.IsMemoAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsMemoAsOtherOutputAsBilledEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_SUPP_AS_OTH_OUT_AS_BILLED")
              {
                passengerConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_PDF_AS_OTH_OUT_AS_BILLING")
              {
                passengerConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLING")
              {
                passengerConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_DS_AS_OTH_OUT_AS_BILLING")
              {
                passengerConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_MEMO_AS_OTH_OUT_AS_BILLING")
              {
                passengerConfigDetail.IsMemoAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                passengerConfigDetail.IsMemoAsOtherOutputAsBillingEntityFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "IS_CONS_PROV_BILLING_FILE_REQ")
              {
                passengerConfigDetail.IsConsolidatedProvisionalBillingFileRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.IsConsolidatedProvisionalBillingFileRequiredFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_AUTOMATED_VC_DET_RPT_REQ")
              {
                passengerConfigDetail.IsAutomatedVcDetailsReportRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.IsAutomatedVcDetailsReportRequiredFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_AUTOMATED_VC_DET_RPT_REQ")
              {
                passengerConfigDetail.IsIsrFileRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.IsIsrFileRequiredFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "CUT_OFF_TIME")
              {
                passengerConfigDetail.CutOffTimeFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.CutOffTimeFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_ISR_FILE_REQUIRED")
              {
                passengerConfigDetail.IsIsrFileRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.IsIsrFileRequiredFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "BILLING_INVOICE_IDEC_OUTPUT")
              {
                passengerConfigDetail.BillingInvoiceIdecOutputFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.BillingInvoiceIdecOutputFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "BILLING_INVOICE_XML_OUTPUT")
              {
                passengerConfigDetail.BillingInvoiceXmlOutputFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.BillingInvoiceXmlOutputFutureValue = string.IsNullOrEmpty(futureUpdate.NewVAlue) ? false : bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "NON_S_PRIME_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.NonSamplePrimeBillingIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSamplePrimeBillingIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "NON_S_PRIME_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSamplePrimeBillingIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSamplePrimeBillingIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSamplePrimeBillingIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "NON_S_PRIME_ISXML_MIG_DATE")
              {
                passengerConfigDetail.NonSamplePrimeBillingIsxmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSamplePrimeBillingIsxmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "NON_S_PRIME_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSamplePrimeBillingIsxmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSamplePrimeBillingIsxmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSamplePrimeBillingIsxmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }

              if (futureUpdate.ElementName == "S_PRO_PRIME_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.SamplingProvIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SamplingProvIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "S_PRO_PRIME_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.SamplingProvIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SamplingProvIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SamplingProvIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_PRO_PRIME_ISXML_MIG_DATE")
              {
                passengerConfigDetail.SamplingProvIsxmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SamplingProvIsxmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "S_PRO_PRIME_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.SamplingProvIsxmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SamplingProvIsxmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SamplingProvIsxmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "NON_S_RM_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.NonSampleRmIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleRmIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "NON_S_RM_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSampleRmIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleRmIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSampleRmIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }




              if (futureUpdate.ElementName == "NON_S_RM_ISXML_MIG_DATE")
              {
                passengerConfigDetail.NonSampleRmIsXmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleRmIsXmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "NON_S_RM_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSampleRmIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleRmIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSampleRmIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "NON_S_BM_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.NonSampleBmIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleBmIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "NON_S_BM_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSampleBmIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleBmIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSampleBmIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "NON_S_BM_ISXML_MIG_DATE")
              {
                passengerConfigDetail.NonSampleBmIsXmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleBmIsXmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "NON_S_BM_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSampleBmIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleBmIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSampleBmIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "NON_S_CM_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.NonSampleCmIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleCmIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "NON_S_CM_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSampleCmIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleCmIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSampleCmIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "NON_S_CM_ISXML_MIG_DATE")
              {
                passengerConfigDetail.NonSampleCmIsXmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleCmIsXmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "NON_S_CM_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.NonSampleCmIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.NonSampleCmIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.NonSampleCmIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_FORM_C_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.SampleFormCIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormCIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "S_FORM_C_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.SampleFormCIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormCIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SampleFormCIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_FORM_C_ISXML_MIG_DATE")
              {
                passengerConfigDetail.SampleFormCIsxmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormCIsxmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "S_FORM_C_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.SampleFormCIsxmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormCIsxmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SampleFormCIsxmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_FORM_DE_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.SampleFormDeIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormDeIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "S_FORM_DE_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.SampleFormDeIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormDeIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SampleFormDeIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_FORM_DE_ISXML_MIG_DATE")
              {
                passengerConfigDetail.SampleFormDeIsxmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormDeIsxmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }
              if (futureUpdate.ElementName == "S_FORM_DE_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.SampleFormDEisxmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormDEisxmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SampleFormDEisxmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_FORM_FXF_ISIDEC_MIG_DATE")
              {
                passengerConfigDetail.SampleFormFxfIsIdecMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormFxfIsIdecMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "S_FORM_FXF_ISIDEC_MIG_STAT_ID")
              {
                passengerConfigDetail.SampleFormFxfIsIdecMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormFxfIsIdecMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SampleFormFxfIsIdecMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "S_FORM_FXF_ISXML_MIG_DATE")
              {
                passengerConfigDetail.SampleFormFxfIsxmlMigratedDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormFxfIsxmlMigratedDateFutureValue = Convert.ToDateTime(FormatFutureValues(futureUpdate.NewVAlue));
              }

              if (futureUpdate.ElementName == "S_FORM_FXF_ISXML_MIG_STAT_ID")
              {
                passengerConfigDetail.SampleFormFxfIsxmlMigratedStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SampleFormFxfIsxmlMigratedStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SampleFormFxfIsxmlMigratedStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }

              if (futureUpdate.ElementName == "SAMPLING_CARRIER_TYPE_ID")
              {
                passengerConfigDetail.SamplingCareerTypeIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                passengerConfigDetail.SamplingCareerTypeIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                passengerConfigDetail.SamplingCareerTypeIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
              if (futureUpdate.ElementName == "LISTING_CURRENCY_CODE_NUM")
              {
                passengerConfigDetail.ListingCurrencyIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                //SCP221813: Auto Billing issue
                passengerConfigDetail.ListingCurrencyIdFutureValue = String.IsNullOrEmpty(futureUpdate.NewVAlue)
                                                                       ? (int?) null
                                                                       : Convert.ToInt32(futureUpdate.NewVAlue);

                passengerConfigDetail.ListingCurrencyIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
            }
          }
        }
        /*DEPENDENT FIELDS RELATED CHANGE START*/
        var memberDetail = MemberRepository.Single(mem => mem.Id == memberId);
        if (passengerConfigDetail == null)
        {
          passengerConfigDetail = new PassengerConfiguration();
        }

        if (memberDetail != null)
        {
          passengerConfigDetail.IsParticipateInValueDetermination = memberDetail.IsParticipateInValueDetermination;
          passengerConfigDetail.IsParticipateInValueDeterminationDisplay = memberDetail.IsParticipateInValueDetermination ? "Activated" : "Not Activated";
          passengerConfigDetail.IsParticipateInValueConfirmation = memberDetail.IsParticipateInValueConfirmation;
          passengerConfigDetail.IsParticipateInValueConfirmationDisplay = memberDetail.IsParticipateInValueConfirmation ? "Activated" : "Not Activated";

          passengerConfigDetail.PaxOldIdecMember = memberDetail.PaxOldIdecMember;
        }

        /*DEPENDENT FIELDS RELATED CHANGE END*/
      }

      return passengerConfigDetail;
    }

    /// <summary>
    /// Gets details of miscellaneous configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which miscellaneous configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns>Miscellaneous Configuration class object</returns>
    public MiscellaneousConfiguration GetMiscellaneousConfiguration(int memberId, bool includeFutureUpdates = false)
    {
      MiscellaneousConfiguration miscellaneousConfigDetail = null;
      if (memberId > 0)
      {
        // Get details of misc configuration corresponding to member ID passed
        miscellaneousConfigDetail = MiscellaneousRepository.Single(misc => misc.MemberId == memberId);
      }

      // Only include future updates if they are required.
      if (miscellaneousConfigDetail != null && includeFutureUpdates)
      {
        var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Miscellaneous, "MEM_MSC_CONFIGURATION", null, null);

        if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
        {
          foreach (var futureUpdate in futureUpdatesData.AsQueryable())
          {
            if (futureUpdate.ElementName == "IS_PDF_AS_OTHER_OUT_AS_BILLED")
            {
              miscellaneousConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
            if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLED")
            {
              miscellaneousConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
            if (futureUpdate.ElementName == "IS_DS_FIL_AS_OTH_OUT_AS_BILLED")
            {
              miscellaneousConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }

            if (futureUpdate.ElementName == "IS_SUPP_AS_OTH_OUT_AS_BILLED")
            {
              miscellaneousConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
            if (futureUpdate.ElementName == "IS_PDF_AS_OTH_OUT_AS_BILLING")
            {
              miscellaneousConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }

            if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLING")
            {
              miscellaneousConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
            if (futureUpdate.ElementName == "IS_DS_AS_OTH_OUT_AS_BILLING")
            {
              miscellaneousConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
              miscellaneousConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }

            if (futureUpdate.ElementName == "BILLING_XML_OUTPUT")
            {
              miscellaneousConfigDetail.BillingXmlOutputFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              miscellaneousConfigDetail.BillingXmlOutputFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }
            if (futureUpdate.ElementName == "IS_BILLNG_DATA_BY_3RDPARTY_REQ")
            {
              miscellaneousConfigDetail.IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              miscellaneousConfigDetail.IsBillingDataSubmittedByThirdPartiesRequiredFutureValue = bool.Parse(futureUpdate.NewVAlue);
            }

            if (futureUpdate.ElementName == "BILLING_ISXML_MIG_STAT_ID")
            {
              miscellaneousConfigDetail.BillingIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              miscellaneousConfigDetail.BillingIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
              miscellaneousConfigDetail.BillingIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
            }

            if (futureUpdate.ElementName == "IS_DAILY_XML_REQUIRED")

            {
              miscellaneousConfigDetail.IsDailyXmlRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              miscellaneousConfigDetail.IsDailyXmlRequiredValue = bool.Parse(futureUpdate.NewVAlue);
            }
          }
        }
      }
      return miscellaneousConfigDetail;
    }

    /// <summary>
    /// Gets details of UATP configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which UATP configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns>UatpConfiguration class object</returns>
    public UatpConfiguration GetUATPConfiguration(int memberId, bool includeFutureUpdates = false,Member uatpMember = null)
    {
      UatpConfiguration uatpConfigDetail = null;
      if (memberId > 0)
      {
        // Get details of misc configuration corresponding to member ID passed
        uatpConfigDetail = UatpRepository.Single(uatp => uatp.MemberId == memberId);
      }
      Member member;

      if (uatpMember != null)
        member = uatpMember;
      else
        member = MemberRepository.GetMember(memberId);
      if (uatpConfigDetail == null)
      {
        uatpConfigDetail = new UatpConfiguration();
        if (member != null)
        {
          uatpConfigDetail.IsDigitalSignatureRequired = member.DigitalSignApplication;

          // Only include future updates if they are required.
          if (includeFutureUpdates)
          {
            var futureUpdatesMemberData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.MemberDetails, "MEMBER_DETAILS", null, null);
            if ((futureUpdatesMemberData != null) && (futureUpdatesMemberData.Count != 0))
            {
              foreach (var futureUpdate in futureUpdatesMemberData.AsQueryable())
              {
                if (futureUpdate.ElementName == "DIGITAL_SIGN_APPLICATION")
                {
                  uatpConfigDetail.IsDigitalSignatureRequiredFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }
              }
            }
          }
        }
      }
      else
      {
        if (member != null)
        {
          uatpConfigDetail.IsDigitalSignatureRequired = member.DigitalSignApplication;

          // Only include future updates if they are required.
          if (includeFutureUpdates)
          {
            var futureUpdatesMemberData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.MemberDetails, "MEMBER_DETAILS", null, null);
            if ((futureUpdatesMemberData != null) && (futureUpdatesMemberData.Count != 0))
            {
              foreach (var futureUpdate in futureUpdatesMemberData.AsQueryable())
              {
                if (futureUpdate.ElementName == "DIGITAL_SIGN_APPLICATION")
                {
                  uatpConfigDetail.IsDigitalSignatureRequiredFutureValue = bool.Parse(futureUpdate.NewVAlue);
                }
              }
            }
          }
        }

        // Only include future updates if they are required.
        if (includeFutureUpdates)
        {
          var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.Uatp, "MEM_UATP_CONFIGURATION", null, null);

          if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesData.AsQueryable())
            {
              if (futureUpdate.ElementName == "IS_PDF_AS_OTHER_OUT_AS_BILLED")
              {
                uatpConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsPdfAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLED")
              {
                uatpConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsDetailListingAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_DS_FIL_AS_OTH_OUT_AS_BILLED")
              {
                uatpConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsDsFileAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "IS_SUPP_AS_OTH_OUT_AS_BILLED")
              {
                uatpConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsSuppDocAsOtherOutputAsBilledEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_PDF_AS_OTH_OUT_AS_BILLING")
              {
                uatpConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsPdfAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "IS_LIST_AS_OTH_OUT_AS_BILLING")
              {
                uatpConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsDetailListingAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_DS_AS_OTH_OUT_AS_BILLING")
              {
                uatpConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureDate = FormatFutureDateValue(futureUpdate.ChangeEffectiveOn);
                uatpConfigDetail.IsDsFileAsOtherOutputAsBillingEntityFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "BILLING_XML_OUTPUT")
              {
                uatpConfigDetail.BillingxmlOutputFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                uatpConfigDetail.BillingxmlOutputFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "IS_BILLNG_DATA_BY_3RDPARTY_REQ")
              {
                uatpConfigDetail.IsBillingDataSubmittedByThirdPartiesRequiredFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                uatpConfigDetail.IsBillingDataSubmittedByThirdPartiesRequiredFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "BILLING_ISXML_MIG_DATE")
              {
                uatpConfigDetail.BillingIsXmlMigrationDateFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                uatpConfigDetail.BillingIsXmlMigrationDateFutureValue = Convert.ToDateTime(futureUpdate.NewVAlue);
              }

              if (futureUpdate.ElementName == "IS_UATP_INV_IGNORE_FROM_DSPROC")
              {
                uatpConfigDetail.ISUatpInvIgnoreFromDsprocFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                uatpConfigDetail.ISUatpInvIgnoreFromDsprocFutureValue = bool.Parse(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "BILLING_ISXML_MIG_STAT_ID")
              {
                uatpConfigDetail.BillingIsXmlMigrationStatusIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                uatpConfigDetail.BillingIsXmlMigrationStatusIdFutureValue = Convert.ToInt32(futureUpdate.NewVAlue);
                uatpConfigDetail.BillingIsXmlMigrationStatusIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
              }
            }
          }
        }
      }

      if (member != null)
      {
        if (uatpConfigDetail == null)
        {
          uatpConfigDetail = new UatpConfiguration();
        }
        uatpConfigDetail.UatpInvoiceHandledbyAtcan = member.UatpInvoiceHandledbyAtcan;
        uatpConfigDetail.UatpInvoiceHandledbyAtcanFutureValue = member.UatpInvoiceHandledbyAtcanFutureValue;
        uatpConfigDetail.UatpInvoiceHandledbyAtcanFuturePeriod = member.UatpInvoiceHandledbyAtcanFuturePeriod;
        uatpConfigDetail.UatpInvoiceHandledbyAtcanDisplay = member.UatpInvoiceHandledbyAtcan ? "Activated" : "Not Activated";
      }

      return uatpConfigDetail;
    }

    /// <summary>
    /// Get Technical Configuration
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public TechnicalConfiguration GetTechnicalConfig(int memberId)
    {
      TechnicalConfiguration technicalDetail = null;
      if (memberId > 0)
      {
        technicalDetail = TechnicalRepository.Single(technical => technical.MemberId == memberId);
      }

      return technicalDetail;
    }
    /// <summary>
    /// Get E-billing Configuration
    /// </summary>
    public EBillingConfiguration GetEbillingConfig(int memberId, bool includeFutureUpdates = false)
    {
      EBillingConfiguration ebillingConfigDetail = null;
     if (memberId > 0)
      {
        ebillingConfigDetail = EBillingRepository.Single(eBilling => eBilling.MemberId == memberId);
      // Only include future updates if they are required.
        if (includeFutureUpdates)
        {
          var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.EBilling, "MEM_E_BILLING_CONFIGURATION", null, null);
          if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesData.AsQueryable())
            {
              if (futureUpdate.ElementName == "LEGAL_TEXT")
              {
                ebillingConfigDetail.LegalTextFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalTextFutureValue = futureUpdate.NewVAlue;
              }

              //code added to get the future updated values and assign to viewing model.
              if (futureUpdate.ElementName == "LEGAL_ARC_REQ_PAX_REC_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforPaxRecInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforPaxRecInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
              if (futureUpdate.ElementName == "LEGAL_ARC_REQ_PAX_PAY_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforPaxPayInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforPaxPayInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
              if(futureUpdate.ElementName == "LEGAL_ARC_REQ_CGO_REC_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforCgoRecInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforCgoRecInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
              if(futureUpdate.ElementName == "LEGAL_ARC_REQ_CGO_PAY_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforCgoPayInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforCgoPayInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }                                                        
              if(futureUpdate.ElementName == "LEGAL_ARC_REQ_MISC_REC_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforMiscRecInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforMiscRecInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
              if(futureUpdate.ElementName == "LEGAL_ARC_REQ_MISC_PAY_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforMiscPayInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforMiscPayInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
              if(futureUpdate.ElementName == "LEGAL_ARC_REQ_UATP_REC_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforUatpRecInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforUatpRecInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
              if(futureUpdate.ElementName == "LEGAL_ARC_REQ_UATP_PAY_INV")
              {
                ebillingConfigDetail.LegalArchRequiredforUatpPayInvFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                ebillingConfigDetail.LegalArchRequiredforUatpPayInvFutureValue = Convert.ToBoolean(futureUpdate.NewVAlue);
              }
            }
          }

          var futureUpdatesDataDsReq = FutureUpdatesManager.GetFutureUpdatesList(memberId, (int)ElementGroupType.EBillingDS, "MEM_DS_REQUIRED_COUNTRY_MATRIX", null, null);
          if ((futureUpdatesDataDsReq != null) && (futureUpdatesDataDsReq.Count != 0))
          {
            foreach (var futureUpdate in futureUpdatesDataDsReq.AsQueryable())
            {
              if (futureUpdate.ElementName == "COUNTRY_CODE")
              {
                if (futureUpdate.RelationId == 2)
                {
                  ebillingConfigDetail.DSReqCountriesAsBillingFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ebillingConfigDetail.DSReqCountriesAsBillingFutureValue = futureUpdate.NewVAlue;
                }
                if (futureUpdate.RelationId == 1)
                {
                  ebillingConfigDetail.DSReqCountriesAsBilledFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                  ebillingConfigDetail.DSReqCountriesAsBilledFutureValue = futureUpdate.NewVAlue;
                }
              }
            }
          }
        }

        // Get member query
        var memberData = MemberRepository.Get(memberdata => memberdata.Id == memberId);
        // Get only required properties of member
        var memberList = memberData.Select(mem => new { mem.DigitalSignApplication, mem.DigitalSignVerification, mem.LegalArchivingRequired }).ToList();

        if (memberList.Count > 0)
        {
          var member = memberList[0];
          if (ebillingConfigDetail == null)
          {
            ebillingConfigDetail = new EBillingConfiguration();
          }
          ebillingConfigDetail.IsDigitalSignatureRequired = member.DigitalSignApplication;
          ebillingConfigDetail.IsDigitalSignatureRequiredDisplay = member.DigitalSignApplication ? "Activated" : "Not Activated";
          ebillingConfigDetail.IsDsVerificationRequired = member.DigitalSignVerification;
          ebillingConfigDetail.IsDsVerificationRequiredDisplay = member.DigitalSignVerification ? "Activated" : "Not Activated";
          ebillingConfigDetail.IsLegalArchievingRequired = member.LegalArchivingRequired;
          ebillingConfigDetail.IsLegalArchievingRequiredDisplay = member.LegalArchivingRequired ? "Activated" : "Not Activated";
        }
      }

      return ebillingConfigDetail;
    }

    /// <summary>
    /// Gets the member location info.
    /// </summary>
    /// <param name="locationId">The location Id</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    public Location GetMemberLocationDetails(int locationId, bool includeFutureUpdates = false)
    {
      var memberLocation = LocationRepository.Single(location => location.Id == locationId);

      if (memberLocation != null && includeFutureUpdates)
      {
        var futureUpdatesData = FutureUpdatesManager.GetFutureUpdatesList(memberLocation.MemberId, (int)ElementGroupType.Locations, "MEM_LOCATION", null, memberLocation.Id);
        if ((futureUpdatesData != null) && (futureUpdatesData.Count != 0))
        {
          foreach (var futureUpdate in futureUpdatesData.AsQueryable())
          {
            if (futureUpdate.ElementName == "REGISTRATION_ID")
            {
              memberLocation.RegistrationIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.RegistrationIdFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "TAX_VAT_REGISTRATION_NUMBER")
            {
              memberLocation.TaxVatRegistrationNumberFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.TaxVatRegistrationNumberFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "ADD_TAX_VAT_REGISTRATION_NUM")
            {
              memberLocation.AdditionalTaxVatRegistrationNumberFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.AdditionalTaxVatRegistrationNumberFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "ADDRESS_LINE1")
            {
              memberLocation.AddressLine1FuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.AddressLine1FutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "ADDRESS_LINE2")
            {
              memberLocation.AddressLine2FuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.AddressLine2FutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "ADDRESS_LINE3")
            {
              memberLocation.AddressLine3FuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.AddressLine3FutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "CITY_NAME")
            {
              memberLocation.CityNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.CityNameFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "SUB_DIVISION_NAME")
            {
              memberLocation.SubDivisionNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.SubDivisionNameFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "POSTAL_CODE")
            {
              memberLocation.PostalCodeFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.PostalCodeFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "LEGAL_TEXT")
            {
              memberLocation.LegalTextFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.LegalTextFutureValue = Convert.ToString(futureUpdate.NewVAlue);
            }
            else if (futureUpdate.ElementName == "COUNTRY_CODE")
            {
              memberLocation.CountryIdFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.CountryIdFutureValue = futureUpdate.NewVAlue;
              memberLocation.CountryIdFutureDisplayValue = futureUpdate.NewValueDisplayName;
            }
            // CMP597: TFS_Bug_8930 IS WEB -Memebr legal name is not a future update field from SIS ops login
            else if (futureUpdate.ElementName == "MEMBER_LEGAL_NAME")
            {
              memberLocation.MemberLegalNameFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
              memberLocation.MemberLegalNameFutureValue = futureUpdate.NewVAlue;
            }
            // CMP597 : Not able to update Active checkbox.
            else if (futureUpdate.ElementName == "IS_ACTIVE")
            {
                memberLocation.IsActiveFuturePeriod = FormatFuturePeriodValue(futureUpdate.ChangeEffectivePeriod);
                memberLocation.IsActiveFutureValue = futureUpdate.NewVAlue;
            }
          }
        }

        //settting the member property to null as this property gets initialized at the instance when we use read the memberId property.
        //The member property contains member details including the logo. Which can be huge. This causes JsonSerialization max length exceeds exception.
        //so setting this property to null to avaoid sending huge data to client side
        memberLocation.Member = null;
      }

      return memberLocation;
    }

    /// <summary>
    /// Gets contact details.
    /// </summary>
    /// <param name="contactId">Contact Id whose details to be fetched.</param>
    /// <returns>Contact details.</returns>
    public Contact GetContactDetails(int contactId)
    {
      var contactDetails = ContactRepository.Single(ml => ml.Id == contactId);

      // if LocationId is selected for a contact then get location details for the locationid and assign it to contact object
      if ((contactDetails != null) && (contactDetails.LocationId > 0))
      {
        var locationDetails = LocationRepository.Single(loc => loc.Id == contactDetails.LocationId);

        if (locationDetails != null)
        {
          contactDetails.AddressLine1 = locationDetails.AddressLine1;
          contactDetails.AddressLine2 = locationDetails.AddressLine2;
          contactDetails.AddressLine3 = locationDetails.AddressLine3;
          contactDetails.AddressLine2 = locationDetails.AddressLine2;
          contactDetails.CountryId = locationDetails.CountryId;
          contactDetails.SubDivisionName = locationDetails.SubDivisionName;
          contactDetails.PostalCode = locationDetails.PostalCode;
          contactDetails.CityName = locationDetails.CityName;
        }
      }

      return contactDetails;
    }

    public Contact GetContactDetailsByEmailAndMember(string emailId, int memberId)
    {
      var contactDetails = ContactRepository.Single(ml => ml.EmailAddress == emailId && ml.MemberId == memberId);

      // if LocationId is selected for a contact then get location details for the locationid and assign it to contact object
      if ((contactDetails != null) && (contactDetails.LocationId > 0))
      {
        var locationDetails = LocationRepository.Single(loc => loc.Id == contactDetails.LocationId);

        if (locationDetails != null)
        {
          contactDetails.AddressLine1 = locationDetails.AddressLine1;
          contactDetails.AddressLine2 = locationDetails.AddressLine2;
          contactDetails.AddressLine3 = locationDetails.AddressLine3;
          contactDetails.AddressLine2 = locationDetails.AddressLine2;
          contactDetails.CountryId = locationDetails.CountryId;
          contactDetails.SubDivisionName = locationDetails.SubDivisionName;
          contactDetails.PostalCode = locationDetails.PostalCode;
          contactDetails.CityName = locationDetails.CityName;
        }
      }

      return contactDetails;
    }

    /// <summary>
    /// Get Contact Details by Email Id
    /// </summary>
    /// <param name="emailId"> string of email Id </param>
    /// <returns>Contact object</returns>
    public Contact GetContactDetailsByEmail(string emailId)
    {
      var contactDetails = ContactRepository.Single(ml => ml.EmailAddress == emailId);

      // if LocationId is selected for a contact then get location details for the locationid and assign it to contact object
      if ((contactDetails != null) && (contactDetails.LocationId > 0))
      {
        var locationDetails = LocationRepository.Single(loc => loc.Id == contactDetails.LocationId);

        if (locationDetails != null)
        {
          contactDetails.AddressLine1 = locationDetails.AddressLine1;
          contactDetails.AddressLine2 = locationDetails.AddressLine2;
          contactDetails.AddressLine3 = locationDetails.AddressLine3;
          contactDetails.AddressLine2 = locationDetails.AddressLine2;
          contactDetails.CountryId = locationDetails.CountryId;
          contactDetails.SubDivisionName = locationDetails.SubDivisionName;
          contactDetails.PostalCode = locationDetails.PostalCode;
          contactDetails.CityName = locationDetails.CityName;
        }
      }

      return contactDetails;
    }

    /// <summary>
    /// Check User Profile email Id is in contact Member 
    /// </summary>
    /// <param name="emailId"></param>
    /// <returns></returns>
    public bool IsUserEmailIdInMemberContact(string emailId, int memberId)
    {
      var contactDetails = ContactRepository.Get(ml => ml.EmailAddress == emailId && ml.MemberId == memberId);
      if (contactDetails.Count() > 0)
      {
        foreach (var contactDetail in contactDetails)
        {
          contactDetail.StaffId = string.Empty;
          contactDetail.SalutationId = 0;
          contactDetail.FirstName = string.Empty;
          contactDetail.LastName = string.Empty;
          contactDetail.PositionOrTitle = string.Empty;
          contactDetail.Division = string.Empty;
          contactDetail.Department = string.Empty;
          contactDetail.LocationId = 0;
          contactDetail.CountryId = string.Empty;
          contactDetail.AddressLine1 = string.Empty;
          contactDetail.AddressLine2 = string.Empty;
          contactDetail.AddressLine3 = string.Empty;
          contactDetail.CityName = string.Empty;
          contactDetail.SubDivisionCode = string.Empty;
          contactDetail.SubDivisionName = string.Empty;
          contactDetail.PostalCode = string.Empty;
          contactDetail.PhoneNumber1 = string.Empty;
          contactDetail.PhoneNumber2 = string.Empty;
          contactDetail.MobileNumber = string.Empty;
          contactDetail.FaxNumber = string.Empty;
          contactDetail.SitaAddress = string.Empty;

          ContactRepository.Update(contactDetail);
        }

        UnitOfWork.CommitDefault();
      }
      return true;
    }

    /// <summary>
    /// Retrieves details of 'Default' location for member ID passed
    /// </summary>
    /// <param name="memberId">ID of member for which default location details need to be retrieved</param>
    /// <param name="locationCode">location code for default location</param>
    /// <returns>Location class object</returns>
    public Location GetMemberDefaultLocation(int memberId, string locationCode)
    {
      var memberLocation = LocationRepository.Single(ml => ml.MemberId == memberId && ml.LocationCode.ToUpper() == locationCode.ToUpper());

      return memberLocation;
    }

    /// <summary>
    /// Following method checks whether member's default location exists
    /// </summary>
    /// <param name="memberId">member Id whose Location details are to be checked</param>
    /// <param name="locationCode">Location code</param>
    /// <returns>true if default location exists else false</returns>
    public bool MemberDefaultLocationExists(int memberId, string locationCode)
    {
      // Retrieve Member location details
      var memberLocationExists = LocationRepository.Get(ml => ml.MemberId == memberId && ml.LocationCode.ToUpper() == locationCode.ToUpper()).Count() > 0;
      // Return Member location details
      return memberLocationExists;
    }

    /// <summary>
    /// Following method is used to retrieve Member location details for Location dropdown
    /// </summary>
    /// <param name="memberId">Member Id whose Location details are to be retrieved</param>
    /// <returns>Member Location details</returns>
    public IQueryable<Location> GetMemberLocationDetailsForDropdown(int memberId, bool showOnlyActiveLocations = false)
    {
      // Retrieve Member location details
      /*
      SCP# : 105901 - Billing to inactive location 
      Desc : Check to verify wheather location is active or not is added.
      Date : 24-May-2013
      */
      var memberLocation = showOnlyActiveLocations ? LocationRepository.Get(ml => ml.MemberId == memberId && ml.IsActive) : LocationRepository.Get(ml => ml.MemberId == memberId);
      
      // Return member Location details
      return memberLocation;
    }

   
    /// <summary>
    /// Gets the status history for Ich Configuration of a member
    /// </summary>
    /// <returns></returns>
    public IchConfiguration GetIchMemberStatusList(IchConfiguration ichConfiguration)
    {
      // Get last 2 status records for the member.
      var statusDetail = MemberStatusRepository.Get(ichConf => ichConf.MemberId == ichConfiguration.MemberId && ichConf.MemberType == "ICH").OrderByDescending(ichConf => ichConf.Id).Take(2);
      MemberStatusDetails previousStatus;

      if (statusDetail.Count() > 1)
      {
        var dbCurrentMemberStatusId = statusDetail.FirstOrDefault().MembershipStatusId;

        //if in first record , status=Live
        //  Check if value in previous record is 'Suspended' ,if yes set Reinstatement field
        if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Live)
        {
          ichConfiguration.EntryDate = statusDetail.First().StatusChangeDate;
        }

        //if in first record , status=Terminated
        //  Check if value in previous record is 'Terminated' ,if yes set Termination Date

        {
          if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Terminated)
          {
            ichConfiguration.TerminationDate = statusDetail.First().StatusChangeDate;
          }
        }

        //if in first record , status=Suspended
        //  Check if value in previous record is 'Suspended' ,if yes set Suspension From Date and suspension default date
        if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Suspended)
        {
          ichConfiguration.DefaultSuspensionDate = statusDetail.First().StatusChangeDate;
          previousStatus = statusDetail.Skip(1).FirstOrDefault();
          ichConfiguration.StatusChangedDate = previousStatus.StatusChangeDate;
        }
      }
      else if (statusDetail.Count() == 1)
      {
        int dbCurrentMemberStatusId = statusDetail.FirstOrDefault().MembershipStatusId;
        if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Live)
        {
          ichConfiguration.EntryDate = statusDetail.First().StatusChangeDate;
        }
        else if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Suspended)
        {
          ichConfiguration.DefaultSuspensionDate = statusDetail.First().StatusChangeDate;

          previousStatus = statusDetail.Skip(1).FirstOrDefault();
          ichConfiguration.StatusChangedDate = previousStatus.StatusChangeDate;
        }
        else if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Terminated)
        {
          ichConfiguration.TerminationDate = statusDetail.First().StatusChangeDate;
        }
      }
      return ichConfiguration;
    }

    /// <summary>
    /// Gets the status history for ICH Configuration of a member
    /// </summary>
    /// <returns></returns>
    public AchConfiguration GetAchMemberStatusList(AchConfiguration achConfiguration)
    {
      //gET LAST 2 STATUS RECORDS FOR THE member
      var statusDetail = MemberStatusRepository.Get(achConf => achConf.MemberId == achConfiguration.MemberId && achConf.MemberType == "ACH").OrderByDescending(achConf => achConf.Id).Take(2);
      MemberStatusDetails previousStatus;

      if (statusDetail.Count() > 1)
      {
        int dbCurrentMemberStatusId = statusDetail.FirstOrDefault().MembershipStatusId;
        //if in first record , status=Live
        //  Check if value in previous record is 'Suspended' ,if yes set Reinstatement field
        if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Live)
        {
          achConfiguration.EntryDate = statusDetail.First().StatusChangeDate;
        }

        //if in first record , status=Terminated
        //  Check if value in previous record is 'Terminated' ,if yes set Termination Date

        {
          if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Terminated)
          {
            achConfiguration.TerminationDate = statusDetail.First().StatusChangeDate;
          }
        }

        //if in first record , status=Suspended
        //  Check if value in previous record is 'Suspended' ,if yes set Suspension From Date and suspension default date
        if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Suspended)
        {
          achConfiguration.DefaultSuspensionDate = statusDetail.First().StatusChangeDate;
          previousStatus = statusDetail.Skip(1).FirstOrDefault();
          achConfiguration.StatusChangedDate = previousStatus.StatusChangeDate;
        }
      }
      else if (statusDetail.Count() == 1)
      {
        int dbCurrentMemberStatusId = statusDetail.FirstOrDefault().MembershipStatusId;
        if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Live)
        {
          achConfiguration.EntryDate = statusDetail.First().StatusChangeDate;
        }
        else if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Suspended)
        {
          achConfiguration.DefaultSuspensionDate = statusDetail.First().StatusChangeDate;

          previousStatus = statusDetail.Skip(1).FirstOrDefault();
          achConfiguration.StatusChangedDate = previousStatus.StatusChangeDate;
        }
        else if (dbCurrentMemberStatusId == (int)IchMemberShipStatus.Terminated)
        {
          achConfiguration.TerminationDate = statusDetail.First().StatusChangeDate;
        }
      }
      return achConfiguration;
    }

    /// <summary>
    /// Adds the audit entries to the future updates repository depending on whether the entry is being added or updated.
    /// </summary>
    /// <param name="futureUpdatesList"></param>
    private void AddUpdateAuditEntries(IEnumerable<FutureUpdates> futureUpdatesList)
    {
      if (futureUpdatesList == null)
      {
        throw new ArgumentNullException("futureUpdatesList");
      }

      foreach (var auditEntry in futureUpdatesList)
      {
        if (auditEntry.Id != 0)
        {
          FutureUpdatesRepository.Update(auditEntry);
        }
        else
        {
          FutureUpdatesRepository.Add(auditEntry);
        }
      }
    }

      /// <summary>
      /// Following method will insert Member profile Update message to Oracle queue
      /// CMP-689-Flexible CH Activation Options
      /// one parameter added isFutureUpdateIsLive
      /// </summary>
      /// <param name="messageType">Type of message</param>
      /// <param name="memberId">Updated MemberId</param>
    /// <param name="ichMembershipStatusId">Member ich Membership Status Id</param>
    /// <param name="achMembershipStatusId">Member ach Membership Status Id</param>
    /// <param name="isFutureUpdateIsLive">isFutureUpdateIsLive= false</param>
      //409719
    public void InsertMessageInOracleQueue(string messageType, int memberId, int ichMembershipStatusId = -1, int achMembershipStatusId = -1, bool isFutureUpdateLive = false)
    {
        /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
         * Desc: Additional parametes added for taking decison on members ICH/ACH membership status */
        if (validateMembershipStatus(memberId, ichMembershipStatusId, achMembershipStatusId, isFutureUpdateLive))
        {
            Logger.InfoFormat("Queue member Id [{0} for sending ICH Profile Update", memberId);

            try
            {
                IDictionary<string, string> messages = new Dictionary<string, string>
                                                           {
                                                               {"MSG_TYPE", messageType},
                                                               {"MSG_KEY", memberId.ToString()}
                                                           };
                var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["ProfileUpdateQueueName"].Trim());
                queueHelper.Enqueue(messages);
            }
            catch (Exception exception)
            {
                Logger.Error("Error occurred while adding message to queue.", exception);
                SendUnexpectedErrorNotificationToISAdmin("Member Profile Update", exception.Message, memberId);
            }
        }
        Logger.InfoFormat("Membership status is Not Live/Suspended and also Future value not Live(isFutureUpdateIsLive = false) so not queue member Id [{0}] for Sending ICH Profile Update", memberId);
    }

      /// <summary>
      /// SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
      /// Desc: Method to restrict ICH profile Update for Members having ACH/ICH membership status anything but Live or Suspended.
      /// //CMP-689-Flexible CH Activation Options
      /// before this CMP only Members having ‘ICH/ACH Membership Status’ as "Live", "Suspended" or "Terminated" are included in the update XML
      /// With this CMP, the system should also consider the following:
      ///  	Members whose future value of ‘ICH/ACH Membership Status’ = “Live” (and where the current value = “Not a Member” or "Terminated")
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="ichMembershipStatusId"></param>
      /// <param name="achMembershipStatusId"></param>
      /// <returns></returns>
      private bool validateMembershipStatus(int memberId, int ichMembershipStatusId, int achMembershipStatusId, bool isFutureUpdateLive)
      {
          if (isFutureUpdateLive)
          {
            Logger.InfoFormat("Future Update is 'Live' [isFutureUpdateIsLive : {0}]  from Terminated or Not a member so returning true for member Id [{1}]", isFutureUpdateLive, memberId);
            return true;
          }

          if (ichMembershipStatusId == -1)
          {
              Logger.InfoFormat("ICH membership status not provided, so fetching it for member ID = [{0}]", memberId);
              /* Both configuration not passed, Get it from DB and take decision. */
              var ichConfiguration = GetIchConfig(memberId);
              ichMembershipStatusId = ichConfiguration == null ? -1 : ichConfiguration.IchMemberShipStatusId;
              Logger.InfoFormat("ICH membership status is [{0}] for member Id [{1}]", ichMembershipStatusId, memberId);
          }

          if (achMembershipStatusId == -1)
          {
              Logger.InfoFormat("ACH membership status not provided, so fetching it for member ID = [{0}]", memberId);
              var achConfiguration = GetAchConfig(memberId);
              achMembershipStatusId = achConfiguration == null ? -1 : achConfiguration.AchMembershipStatusId;
              Logger.InfoFormat("ACH membership status is [{0}] for member Id [{1}]", achMembershipStatusId, memberId);
          }

          /* In cases when both ICH and ACH Configs are Found. */
          if (ichMembershipStatusId != -1 && achMembershipStatusId != -1)
          {
              Logger.InfoFormat("Both ICH and ACH configuration available for member Id [{0}]", memberId);
              switch (ichMembershipStatusId)
              {
                  case (int)IchMemberShipStatus.NotAMember:
                  case (int)IchMemberShipStatus.Terminated:
                      Logger.InfoFormat("ICH Membership status is NotAMember/Terminated so now looking for ACH Membership Status for member Id [{0}]", memberId);
                      switch (achMembershipStatusId)
                      {
                          case (int)AchMembershipStatus.NotAMember:
                          case (int)AchMembershipStatus.Terminated:
                              Logger.InfoFormat("ACH Membership status is NotAMember/Terminated so returning false for member Id [{0}]", memberId);
                              return false;
                          default:
                              Logger.InfoFormat("ACH Membership status is [{0}] so returning true for member Id [{1}]", achMembershipStatusId, memberId);
                              return true;
                      }
                  default:
                      Logger.InfoFormat("ICH Membership status is [{0}] so returning true for member Id [{1}]", ichMembershipStatusId, memberId);
                      return true;
              } 
          }

          else if (ichMembershipStatusId != -1 && achMembershipStatusId == -1)
          {
              Logger.InfoFormat("ICH configuration available, but ACH configuration Not available for member Id [{0}]", memberId);
              switch (ichMembershipStatusId)
              {
                  case (int) IchMemberShipStatus.NotAMember:
                  case (int) IchMemberShipStatus.Terminated:
                      Logger.InfoFormat("ICH Membership status is NotAMember/Terminated so returning false for member Id [{0}]", memberId);
                      return false;
                  default:
                      Logger.InfoFormat("ICH Membership status is [{0}] so returning true for member Id [{1}]", ichMembershipStatusId, memberId);
                      return true;
              }
          }

          else if (ichMembershipStatusId == -1 && achMembershipStatusId != -1)
          {
              Logger.InfoFormat("ICH configuration NOT available, but ACH configuration available for member Id [{0}]", memberId);
              switch (achMembershipStatusId)
              {
                  case (int)AchMembershipStatus.NotAMember:
                  case (int)AchMembershipStatus.Terminated:
                      Logger.InfoFormat("ACH Membership status is NotAMember/Terminated so returning false for member Id [{0}]", memberId);
                      return false;
                  default:
                      Logger.InfoFormat("ACH Membership status is [{0}] so returning true for member Id [{1}]", achMembershipStatusId, memberId);
                      return true;
              }
          }
          else
          {
              Logger.InfoFormat("Neither ICH Nor ACH configuration available for member Id [{0}]", memberId);
              /* By default do not queue */
              Logger.InfoFormat("By default returning false for member Id [{0}]", memberId);
              return false;
          }
      }
    /// <summary>
    /// Insert Suspendedmember message to Oracle queue
    /// </summary>
    /// <param name="memberId">Suspended memberId.</param>
    /// <param name="defaultSuspensionDate">Default Suspension Date.</param>
    /// <param name="fromSuspensionDate">From Suspension Date.</param>
    /// <param name="clearingHouse">Clearing House.</param>
    private static void InsertSuspendedMemberMessageInOracleQueue(int memberId, DateTime defaultSuspensionDate, DateTime fromSuspensionDate, string clearingHouse)
    {
      Logger.Info("Enqueuing values.DEFAULT_SUSPENSION_DATE:" + defaultSuspensionDate + "FROM_SUSPENSION_DATE" + fromSuspensionDate + "MEMBER_ID" + memberId + "CLEARING_HOUSE" + clearingHouse);
      try
      {
        // enque message
        IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                     { "DEFAULT_SUSPENSION_DATE", defaultSuspensionDate.ToString() },
                                                                              { "FROM_SUSPENSION_DATE", fromSuspensionDate.ToString() },
                                                                              { "MEMBER_ID", memberId.ToString() },
                                                                              { "CLEARING_HOUSE", clearingHouse }
                                                                            };
        var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["SuspendedMembersourceQueueName"].Trim());
        queueHelper.Enqueue(messages);
        Logger.Info("Enqueued values.");
      } // end try
      catch (Exception exception)
      {
        Logger.Error("Error occurred while adding message to queue.", exception);
      } // end catch

    }

    /// <summary>
    /// UpdateIchMemberStatusHistory 
    /// </summary>
    /// <param name="updatedStatusDate"></param>
    /// <param name="ichConfiguration"></param>
    /// <param name="defaultSuspensionDate"></param>
    private void UpdateIchMemberStatusHistory(DateTime updatedStatusDate, IchConfiguration ichConfiguration, DateTime? defaultSuspensionDate)
    {
      IQueryable<MemberStatusDetails> statusDeatil = MemberStatusRepository.Get(d => d.MemberId == ichConfiguration.MemberId && d.MemberType == "ICH");
      bool flag = false;

      var recentStatusDetails = statusDeatil.OrderByDescending(md => md.Id).Take(2);

      if (recentStatusDetails.Count() >= 1)
      {
        int dbCurrentMemberStatusId = recentStatusDetails.FirstOrDefault().MembershipStatusId;

        if (dbCurrentMemberStatusId == ichConfiguration.IchMemberShipStatusId)
        {
          if (ichConfiguration.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended)
          {
            var first = recentStatusDetails.Take(1).FirstOrDefault(m => m.StatusChangeDate == defaultSuspensionDate && m.MembershipStatusId == ichConfiguration.IchMemberShipStatusId);

            var second = recentStatusDetails.Skip(1).FirstOrDefault(m => m.StatusChangeDate == updatedStatusDate && m.MembershipStatusId == ichConfiguration.IchMemberShipStatusId);





            if (first != null && second != null)
            {
              flag = true;
            }
          }
          else
          {
            var ms = recentStatusDetails.Take(1).FirstOrDefault(m => m.StatusChangeDate == updatedStatusDate && m.MembershipStatusId == ichConfiguration.IchMemberShipStatusId);

            if (ms != null)
            {
              flag = true;
            }
          }
        }
      }

      if (!flag)
      {
        var memberStatus = new MemberStatusDetails();
        memberStatus.MemberId = ichConfiguration.MemberId;
        memberStatus.MembershipStatusId = ichConfiguration.IchMemberShipStatusId;
        memberStatus.StatusChangeDate = updatedStatusDate;
        memberStatus.MemberType = "ICH";
        memberStatus.LastUpdatedOn = DateTime.UtcNow;
        MemberStatusRepository.Add(memberStatus);

        // for suspended user two entries goes into member_deatil_status table this one is second and above is first)
        if (!defaultSuspensionDate.HasValue)
        {
          return;
        }
        var memberStatusDefault = new MemberStatusDetails();
        memberStatusDefault.MemberId = ichConfiguration.MemberId;
        memberStatusDefault.MembershipStatusId = ichConfiguration.IchMemberShipStatusId;
        memberStatusDefault.StatusChangeDate = defaultSuspensionDate.Value;
        memberStatusDefault.MemberType = "ICH";
        memberStatusDefault.LastUpdatedOn = DateTime.UtcNow;
        MemberStatusRepository.Add(memberStatusDefault);
      }
    }

    /// <summary>
    /// Update Ach MemberStatusHistory 
    /// </summary>
    /// <param name="updatedStatusDate"></param>
    /// <param name="achConfiguration"></param>
    private void UpdateAchMemberStatusHistory(DateTime updatedStatusDate, AchConfiguration achConfiguration, DateTime? defaultSuspensionDate)
    {
      IQueryable<MemberStatusDetails> statusDeatil = MemberStatusRepository.Get(d => d.MemberId == achConfiguration.MemberId && d.MemberType == "ACH");
      bool flag = false;

      var recentStatusDetails = statusDeatil.OrderByDescending(md => md.Id).Take(2);

      if (recentStatusDetails.Count() >= 1)
      {
        int dbCurrentMemberStatusId = recentStatusDetails.FirstOrDefault().MembershipStatusId;

        if (dbCurrentMemberStatusId == achConfiguration.AchMembershipStatusId)
        {
          if (achConfiguration.AchMembershipStatusId == (int)IchMemberShipStatus.Suspended)
          {
            var first = recentStatusDetails.Take(1).FirstOrDefault(m => m.StatusChangeDate == defaultSuspensionDate && m.MembershipStatusId == achConfiguration.AchMembershipStatusId);
            var second = recentStatusDetails.Skip(1).FirstOrDefault(m => m.StatusChangeDate == updatedStatusDate && m.MembershipStatusId == achConfiguration.AchMembershipStatusId);

            if (first != null && second != null)
            {
              flag = true;
            }
          }
          else
          {
            var ms = recentStatusDetails.Take(1).FirstOrDefault(m => m.StatusChangeDate == updatedStatusDate && m.MembershipStatusId == achConfiguration.AchMembershipStatusId);

           if (ms != null)
            {
              flag = true;
            }
          }
        }
      }

      if (!flag)
      {
        var memberStatus = new MemberStatusDetails();
        memberStatus.MemberId = achConfiguration.MemberId;
        memberStatus.MembershipStatusId = achConfiguration.AchMembershipStatusId;
        memberStatus.StatusChangeDate = updatedStatusDate;
        memberStatus.MemberType = "ACH";
        memberStatus.LastUpdatedOn = DateTime.UtcNow;
        MemberStatusRepository.Add(memberStatus);

        // for suspended user two entries goes into member_deatil_status table this one is second and above is first)
        if (!defaultSuspensionDate.HasValue)
        {
          return;
        }
        var memberStatusDefault = new MemberStatusDetails();
        memberStatusDefault.MemberId = achConfiguration.MemberId;
        memberStatusDefault.MembershipStatusId = achConfiguration.AchMembershipStatusId;
        memberStatusDefault.StatusChangeDate = defaultSuspensionDate.Value;
        memberStatusDefault.MemberType = "ACH";
        memberStatusDefault.LastUpdatedOn = DateTime.UtcNow;
        MemberStatusRepository.Add(memberStatusDefault);
      }
    }

    /// <summary>
    /// Get contacts for given MemberId
    /// </summary>
    /// <param name="memberId">The Member Id.</param>
    /// <returns></returns>
    public IList<Contact> GetMemberContactList(int memberId)
    {
      var contacts = ContactRepository.Get(contact => contact.MemberId == memberId && contact.IsActive == true).ToList();
      IList<Contact> contactList = new List<Contact>(contacts);
      var emptyContacts = contacts.Where(c => string.IsNullOrEmpty(c.FirstName));
      foreach (var contact in emptyContacts)
      {
        if (String.IsNullOrEmpty(contact.FirstName))
        {
          // var user = GetUserByEmailId(contact.EmailAddress, memberId);
          //SCP333083: XML Validation Failure for A30-XB - SIS Production
          var user = AuthManager.GetUserByUserMailIdTest(contact.EmailAddress, memberId);
          
          if (String.IsNullOrEmpty(user.FirstName) && String.IsNullOrEmpty(user.LastName))
          {
            //Remove contacts with blank FirstName and LastName
            contactList.Remove(contact);
          }
          else
          {
            contact.FirstName = user.FirstName;
            contact.LastName = user.LastName;
          }
        }
      }
      return contactList.ToList();
    }

    /// <summary>
    /// Get all active contacts
    /// </summary>
    /// <returns></returns>
    public IList<Contact> GetAllMemberContactList()
    {
      var contacts = ContactRepository.Get(contact => contact.IsActive);

      return contacts.ToList();
    }

    /// <summary>
    /// Validates member id
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns>
    /// 	<c>true</c> if [is valid airline code] [the specified airline code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidMemberId(string memberId)
    {
      return MemberRepository.GetCount(member => member.MemberCodeNumeric == memberId) > 0;
    }

    /// <summary>
    /// Validates the air line code.
    /// </summary>
    /// <param name="airlineCode">The airline code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid airline code] [the specified airline code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidAirlineCode(string airlineCode)
    {
      return MemberRepository.GetCount(member => member.MemberCodeNumeric == airlineCode) > 0;
    }

    /// <summary>
    /// Validates the air line alpha code.
    /// </summary>
    /// <param name="airlineAlphaCode"></param>
    /// <returns></returns>
    public bool IsValidAirlineAlphaCode(string airlineAlphaCode)
    {
      return MemberRepository.GetCount(member => member.MemberCodeAlpha.ToUpper() == airlineAlphaCode.ToUpper()) > 0;
    }

    /// <summary>
    /// Gets the allowed file extensions for given member and billing category.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="billingCategoryType">The billing category type.</param>
    /// <returns></returns>
    public string GetAllowedFileExtensions(int memberId, BillingCategoryType billingCategoryType)
    {
      var allowedFileExtensions = string.Empty;
      switch (billingCategoryType)
      {
        case BillingCategoryType.Pax:
          allowedFileExtensions = GetMemberConfigurationValue(memberId, MemberConfigParameter.PaxValidFileExtensions);
          break;
        case BillingCategoryType.Cgo:
          var cgoConfiguration = GetCargoConfig(memberId);
          if (cgoConfiguration != null)
          {
            allowedFileExtensions = cgoConfiguration.CgoAllowedFileTypesForSupportingDocuments;
          }
          break;
        case BillingCategoryType.Misc:
          allowedFileExtensions = GetMemberConfigurationValue(memberId, MemberConfigParameter.MiscValidFileExtensions);
          break;
        case BillingCategoryType.Uatp:
          var uatpConfiguration = GetUATPConfiguration(memberId);
          if (uatpConfiguration != null)
          {
            allowedFileExtensions = uatpConfiguration.UatpAllowedFileTypesForSupportingDocuments;
          }
          break;
      }
      return string.Format(@"{0},{1}", allowedFileExtensions, SystemParameters.Instance.General.AllowedDefaultAttachmentExtensions).Trim(new[] { ',' }).ToLower();
    }

 

    public bool CheckFileDuplicateStatus(string fileName)
    {
      return false;
    }

    public string GetFileModifiedName(int attachmentId)
    {
      return "";
    }

    //TODO: Fake To Remove/update end

    /// <summary>
    /// Determines whether [is valid member location] [the specified airline code].
    /// </summary>
    /// <param name="memberLocationCode">The member location code.</param>
    /// <param name="memberId">The member id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid member location] [the specified airline code]; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidMemberLocation(string memberLocationCode, long memberId)
    {
      /*
      SCP# : 105901 - Billing to inactive location 
      Desc : Check to verify wheather location is active or not is added.
      Date : 22-Apr-2013
      */
      var locations = LocationRepository.Get(locationRecord => locationRecord.MemberId == memberId && locationRecord.LocationCode.ToUpper() == memberLocationCode.ToUpper() && locationRecord.IsActive);
      if (locations == null || locations.Count() == 0)
      {
        return false;
      }
      return true;
    }

    /// <summary>
    /// Returns the list of Members present in database
    /// Currently, PAX module is using method GetMemberList for autocomplete feature so this method is renamed as GetMemberListfromDb
    /// Once member profile module starts working perfectly, this method will be renamed to GetMemberList
    /// </summary>
    /// <returns>List of member class objects</returns>
    public IList<Member> GetMemberListFromDB()
    {
      var memberList = MemberRepository.GetAll();

      return memberList.ToList();
    }

    /// <summary>
    /// To get the eligible members list for DRR report from database.
    /// SCP272586: DailyRevenueRecogRepGenService throws exception
    /// </summary>
    /// <returns>List of Members</returns>
    public IList<Member> GetMemberListFromDbForDrrReport()
    {
      var drrMemberList = from memberDetails in MemberRepository.GetAll()
                          join memPaxConfiguration in PassengerRepository.GetAll() on memberDetails.Id equals memPaxConfiguration.MemberId
                          where memberDetails.Id > 0 && (memberDetails.IsParticipateInValueDetermination || memPaxConfiguration.IsParticipateInAutoBilling)
                          select memberDetails;
      return drrMemberList.ToList();
    }

    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
     * Desc: Added new parameter excludeTypeBMembers. */
    public string GetMemberListForUI(string filter,
                                     int memberIdToSkip,
                                     bool includePending = true,
                                     bool includeBasic = true,
                                     bool includeRestricted = true,
                                     bool includeTerminated = false,
                                     bool includeOnlyAch = false,
                                     bool includeOnlyIch = false,
                                     int ichZone = 0,
                                     bool excludeMergedMember = false,
                                     int includeMemberType = 0, 
                                     bool excludeTypeBMembers = false)
    {
        return MemberRepository.GetMembers(filter, memberIdToSkip, includePending, includeBasic, includeRestricted, includeTerminated, includeOnlyAch, includeOnlyIch, ichZone, excludeMergedMember, includeMemberType, excludeTypeBMembers);
    }

    /// <summary>
    /// Returns the list of ICH/Ach Members present in database
    /// </summary>
    /// <returns>List of member class objects</returns>
    public IList<Member> GetMemberListForIchOrAch(int category, string menuType)
    {
      if (category == (int)UserCategory.IchOps || (menuType == "ich" && category == (int)UserCategory.SisOps))
      {
        var ichMembersId =
          IchRepository.Get(ich => ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Live || ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended).Select(i => i.MemberId);
        var memberList = MemberRepository.Get(m => ichMembersId.Contains(m.Id));

        return memberList.ToList();
      }
      else
      {
        var achMembersId =
          AchRepository.Get(ach => ach.AchMembershipStatusId == (int)AchMembershipStatus.Live || ach.AchMembershipStatusId == (int)AchMembershipStatus.Suspended).Select(i => i.MemberId);
        var memberList = MemberRepository.Get(m => achMembersId.Contains(m.Id));

        return memberList.ToList();
      }
    }

    /// <summary>
    /// Returns the list of ICH/Ach Members present in database
    /// </summary>
    /// <returns>List of member class objects</returns>
    public IList<Member> GetMembersBasedOnUserCategory(int category,bool isBothIchAch = false)
    {
      //CMP602
      if (isBothIchAch)
      {
        var achMembersId =
                AchRepository.Get(
                  ach =>
                  ach.AchMembershipStatusId == (int)AchMembershipStatus.Live ||
                  ach.AchMembershipStatusId == (int)AchMembershipStatus.Suspended).Select(i => i.MemberId);
        var achMemberList = MemberRepository.Get(m => achMembersId.Contains(m.Id));

        var ichMembersId =
          IchRepository.Get(
            ich =>
            ich.IchMemberShipStatusId == (int) IchMemberShipStatus.Live ||
            ich.IchMemberShipStatusId == (int) IchMemberShipStatus.Suspended).Select(i => i.MemberId);
        var ichMemberList = MemberRepository.Get(m => ichMembersId.Contains(m.Id));

        var memberList = achMemberList.Union(ichMemberList);
        return memberList.ToList();
      }

      switch (category)
      {
        case (int) UserCategory.SisOps:
          {
            return GetMemberListFromDB();
          }
        case (int) UserCategory.AchOps:
          {
            var achMembersId =
              AchRepository.Get(
                ach =>
                ach.AchMembershipStatusId == (int) AchMembershipStatus.Live ||
                ach.AchMembershipStatusId == (int) AchMembershipStatus.Suspended).Select(i => i.MemberId);
            var memberList = MemberRepository.Get(m => achMembersId.Contains(m.Id));

            return memberList.ToList();
          }
        default:
          {
            var ichMembersId =
              IchRepository.Get(
                ich =>
                ich.IchMemberShipStatusId == (int) IchMemberShipStatus.Live ||
                ich.IchMemberShipStatusId == (int) IchMemberShipStatus.Suspended).Select(i => i.MemberId);
            var memberList = MemberRepository.Get(m => ichMembersId.Contains(m.Id));

            return memberList.ToList();
          }
      }
    }

    /// <summary>
    /// Assigns contact types assigned to one contact to another contact
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public bool ReplaceContacts(int contactId, int replacedContactId)
    {
      //Check whether data for contact type mapping exists for contact for which contacts should be replaced
      var assignedContactsList = ContactTypeMatrixRepository.Get(contact => contact.ContactId == contactId);

      //if data for contact type mapping does not exists for contact for which contacts should be replaced, return false
      if (assignedContactsList.ToList().Count == 0)
      {
        return false;
      }
      else
      {
        //for each contact type mapping, set contactID = contact ID to be replaced and update the database
        foreach (var contact in assignedContactsList.ToList())
        {
          var newcontact = new ContactContactTypeMatrix { ContactId = replacedContactId, ContactTypeId = contact.ContactTypeId };
          var contactTypeMatrixRecord = ContactTypeMatrixRepository.Single(con => con.ContactId == newcontact.ContactId && con.ContactTypeId == newcontact.ContactTypeId);
          if (contactTypeMatrixRecord == null)
          {
            ContactTypeMatrixRepository.Add(newcontact);
            UnitOfWork.CommitDefault();
          }
          ContactTypeMatrixRepository.Delete(contact);
          UnitOfWork.CommitDefault();
        }
      }

      return true;
    }

    /// <summary>
    /// Copies contact types of one contact to another contact
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    public bool CopyContacts(int contactId, int copyToContactId)
    {
      //Check whether data for contact type mapping exists for contact for which contacts should be replaced
      var assignedContactsList = ContactTypeMatrixRepository.Get(contact => contact.ContactId == contactId);

      //if data for contact type mapping does not exists for contact, return false
      if (assignedContactsList.ToList().Count == 0)
      {
        return false;
      }
      else
      {
        //for each contact type mapping, set contactID = contact ID to be replaced and update the database
        foreach (var contact in assignedContactsList.ToList())
        {
          var newcontact = new ContactContactTypeMatrix { ContactId = copyToContactId, ContactTypeId = contact.ContactTypeId };
          var contactTypeMatrixRecord = ContactTypeMatrixRepository.Single(con => con.ContactId == newcontact.ContactId && con.ContactTypeId == newcontact.ContactTypeId);
          if (contactTypeMatrixRecord == null)
          {
            ContactTypeMatrixRepository.Add(newcontact);
            UnitOfWork.CommitDefault();
          }
        }
      }

      return true;
    }

    public bool DeleteContact(int contactId, int memberId)
    {
      //SCP99417:Is-web Performace 
      //Commented below code and added in if (isDeleted==1) as it is used ony if isDeleted==1
      //var ichConfiguration = GetIchConfig(memberId);
      var contactRecordInDb = ContactRepository.Single(contactDetails => contactDetails.Id == contactId);
      //List<FutureUpdates> memberFutureUpdatesList = new List<FutureUpdates>();*/

      var isDeleted = ContactsRepository.DeleteMemberContact(contactId);

      if (isDeleted == 1)
      {
        var ichConfiguration = GetIchConfig(memberId);
        List<FutureUpdates> memberFutureUpdatesList = new List<FutureUpdates>();

        //AddAuditTrailForImmediateUpdates(ElementGroupType.Contacts, memberId, "CONTACT_ID", "NA", "NA", (int)ActionType.Delete, contactId, "NA", "NA", ref memberFutureUpdatesList);

        // Add FirstName,LastName and EmailAdress in future update table instead of NA for Audit Trail.
        AddAuditTrailForImmediateUpdates(ElementGroupType.Contacts, memberId, "CONTACT_ID", contactRecordInDb.FirstName, contactRecordInDb.LastName, (int)ActionType.Delete, contactId, contactRecordInDb.EmailAddress, "NA", ref memberFutureUpdatesList);
        UnitOfWork.CommitDefault();

        if ((ichConfiguration != null) && (ichConfiguration.IchMemberShipStatusId != 4))
        {
            /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
             * Desc: Passing ICH Membership status for this member. */
            InsertMessageInOracleQueue("MemberProfileUpdate", memberId, ichMembershipStatusId: ichConfiguration.IchMemberShipStatusId);
          SendMailToIchForMemberProfileUpdate(memberId);
        }

        SendImmediateUpdatesEmail(memberFutureUpdatesList);
      }

      return isDeleted == 1;
    }

    /// <summary>
    /// Maps newly configured sponsored members against a member and removes sponsored member mapping for passed member list
    /// </summary>
    /// <param name="memberId">ID of member for which new sponsored member should be added or existing sponsored member mapping should be removed</param>
    /// <param name="addedSponsoredMembers">List of memberIDs which should be marked as members sponsored by member denoted by member ID</param>
    /// <param name="deletedSponsoredMembers">List of memberIDs which would not be sponsored by member denoted by member ID</param>
    /// <param name="futurePeriod">Future period value from which sponsorer values will be effective</param>
    /// <param name="futureUpdatesList"></param>
    /// <returns>True if list added and removed successfully,false otherwise</returns>
    public bool UpdateSponsoredMemberList(int memberId, string addedSponsoredMembers, string deletedSponsoredMembers, string futurePeriod, ref List<FutureUpdates> futureUpdatesList)
    {
      bool updateResult = false;
      int oldSponsoredBy = 0;
      string oldSponsoredMemberName = null;

      // If future period is specified , add data to future update table else add data to ICH_configuration table

      // Do some thing only if valid member id is passed 
      if (memberId != 0)
      {
        var toBeDeletedMembers = new string[] { };
        var toBeAddedMembers = new string[] { };
        if (!string.IsNullOrEmpty(deletedSponsoredMembers))
        {
          //trim last comma (if present) and split in to id strings
          toBeDeletedMembers = deletedSponsoredMembers.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }
        if (!string.IsNullOrEmpty(addedSponsoredMembers))
        {
          // trim last comma (if present) and split in to id strings
          toBeAddedMembers = addedSponsoredMembers.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }

        var memberCommercialName = GetMemberCommercialName(memberId);

        // Update all existing fu records periods regardless of whether they are in deleted or added list or not
        if (!(toBeAddedMembers.Count() == 0 && toBeDeletedMembers.Count() == 0))
        {
          //get an instance of element group repository
          var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));
          var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)ElementGroupType.Ich);
          var tableName = elementGroup.FirstOrDefault().TableName;
          var strMemberId = memberId.ToString();

          List<FutureUpdates> futureUpdateData = null;
          var futureUpdateRepository = Ioc.Resolve<IFutureUpdatesRepository>(typeof(IFutureUpdatesRepository));
          futureUpdateData =
            futureUpdateRepository.Get(
              futureUpdate =>
              (futureUpdate.NewVAlue == strMemberId || futureUpdate.OldVAlue == strMemberId) && futureUpdate.ElementName == "SPONSORED_BY" && futureUpdate.TableName == tableName &&
              futureUpdate.IsChangeApplied == false).ToList();

          foreach (var fu in futureUpdateData)
          {
            // if future period is change and
            // if the existing future update record in not present in Add and Remove list and 
            // then update period of future update to list used for sending mails
            if (fu.ChangeEffectivePeriod != Convert.ToDateTime(futurePeriod) && !toBeDeletedMembers.Contains(fu.MemberId.ToString()) && !toBeAddedMembers.Contains(fu.MemberId.ToString()))
            {
              fu.ChangeEffectivePeriod = Convert.ToDateTime(futurePeriod);
              futureUpdateRepository.Update(fu);
              //Add in list to send mails
              futureUpdatesList.Add(fu);
            }
          }
        }

        if (toBeDeletedMembers.Count() > 0)
        {
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            foreach (string sponsoredMember in toBeDeletedMembers)
            {
              int sponsoredMemberId = Convert.ToInt32(sponsoredMember);

              // check if the sponsored member is already sponsored by this member
              var recordForThisSponsorshipRelationship = IchRepository.Single(member => member.MemberId == sponsoredMemberId && member.SponsoredById == memberId);
              // Check whether pending future updates record is present for this Sponsorship Relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, sponsoredMemberId, "SPONSORED_BY", memberId.ToString(), null);

              // If future update record for this sponsorship relation ship is not present
              // and this sponsorship relation ship is already effective then only add or update a future update record
              if ((futureUpdatesRecord == null || futureUpdatesRecord.Count <= 0) && (recordForThisSponsorshipRelationship != null))
              {
                // add sponsored by in future update
                updateResult = UpdateFutureUpdates(ElementGroupType.Ich,
                                                   sponsoredMemberId,
                                                   "SPONSORED_BY",
                                                   memberId.ToString(),
                                                   null,
                                                   futurePeriod,
                                                   null,
                                                   (int)ActionType.Delete,
                                                   false,
                                                   null,
                                                   memberCommercialName,
                                                   String.Empty,
                                                   ref futureUpdatesList);
              }
              else if (futureUpdatesRecord != null && futureUpdatesRecord.Count > 0)
              {
                FutureUpdatesRepository.Delete(futureUpdatesRecord[0]);
                updateResult = true;
              }
            }
          }
          else
          {
            foreach (string arrStr in toBeDeletedMembers)
            {
              var memId = 0;
              int.TryParse(arrStr, out memId);
              var ichMemberRecordinDb = IchRepository.Single(member => member.MemberId == memId);
              if (ichMemberRecordinDb != null)
              {
                ichMemberRecordinDb.SponsoredById = 0;
                IchRepository.Update(ichMemberRecordinDb);
                updateResult = true;
              }
            }
          }
        }
        else
        {
          updateResult = true;
        }

        // add sponsored members only if a list is passed
        if (toBeAddedMembers.Count() > 0)
        {
          // if a future period is specified then add in future update else we'll add in ich config table
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            // loop on the list of sponsored members
            foreach (string sponsoredMember in toBeAddedMembers)
            {
              int sponsoredMemberId = Convert.ToInt32(sponsoredMember);

              // check if the sponsored member is already sponsored by this member
              var recordForThisSponsorshipRelationship = IchRepository.Single(member => member.MemberId == sponsoredMemberId && member.SponsoredById == memberId);
              // Check whether pending future updates record is present for this Sponsorship Relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, sponsoredMemberId, "SPONSORED_BY", memberId.ToString(), null);

              // If future update record for this sponsorship relation ship is present
              // or this sponsorship relationship is not already effective then only add or update a future update record
              if ((futureUpdatesRecord != null && futureUpdatesRecord.Count > 0) || recordForThisSponsorshipRelationship == null)
              {
                // get record of the the sponsored member from db
                var ichsponsoredmember = IchRepository.Single(member => member.MemberId == sponsoredMemberId);
                // is the sponsored member's ich record present and is a sponsoror was set for it?
                // if so then get name of the old sponsoror of this member
                if ((ichsponsoredmember != null) && (ichsponsoredmember.SponsoredById.HasValue))
                {
                  oldSponsoredBy = ichsponsoredmember.SponsoredById.Value;

                  if (oldSponsoredBy > 0)
                  {
                    // get member name for sponsored member
                    var memberData = MemberRepository.Single(mem => mem.Id == oldSponsoredBy);
                    if (memberData != null)
                    {
                      oldSponsoredMemberName = memberData.CommercialName;
                    }
                  }
                }

                // if ich configuration found for this member then add future update record for it
                if (ichsponsoredmember != null)
                {
                  //add sponsored by in future update
                  updateResult = UpdateFutureUpdates(ElementGroupType.Ich,
                                                     Convert.ToInt32(sponsoredMember),
                                                     "SPONSORED_BY",
                                                     (oldSponsoredBy == 0 ? null : oldSponsoredBy.ToString()),
                                                     memberId.ToString(),
                                                     futurePeriod,
                                                     null,
                                                     (int)ActionType.Update,
                                                     false,
                                                     null,
                                                     oldSponsoredMemberName,
                                                     memberCommercialName,
                                                     ref futureUpdatesList);

                  //add an is sponsoring dummy record in future updates list to be sent to mail sender
                  updateResult = UpdateFutureUpdates(ElementGroupType.Ich,
                                                     memberId,
                                                     "IS SPONSORING",
                                                     null,
                                                     sponsoredMember.ToString(),
                                                     futurePeriod,
                                                     null,
                                                     (int)ActionType.Update,
                                                     true,
                                                     null,
                                                     null,
                                                     ichsponsoredmember.Member.CommercialName,
                                                     ref futureUpdatesList);
                }
              }
            }
          }
          // else if a future period is not specified then add we'll add in ICH config table
          else
          {
            // loop on the list of sponsored members
            foreach (string arrStr in toBeAddedMembers)
            {
              var memId = 0;
              int.TryParse(arrStr, out memId);
              var ichMemberRecordinDb = IchRepository.Single(member => member.MemberId == memId);

              // if record of the sponsored member is found in the ICH configuration then update its sponsored by id field
              if (ichMemberRecordinDb != null)
              {
                ichMemberRecordinDb.SponsoredById = memberId;
                IchRepository.Update(ichMemberRecordinDb);

                // Add audit trail records for records inserted in create mode
                AddAuditTrailForImmediateUpdates(ElementGroupType.Ich,
                                                 Convert.ToInt32(memId),
                                                 "SPONSORED_BY",
                                                 null,
                                                 memberId.ToString(),
                                                 (int)ActionType.Create,
                                                 null,
                                                 "",
                                                 memberCommercialName,
                                                 ref futureUpdatesList);

                // add an is sponsoring dummy record in future updates list to be sent to mail sender
                AddAuditTrailForImmediateUpdates(ElementGroupType.Ich,
                                                 memberId,
                                                 "IS SPONSORING",
                                                 null,
                                                 memId.ToString(),
                                                 (int)ActionType.Create,
                                                 null,
                                                 "",
                                                 ichMemberRecordinDb.Member.CommercialName,
                                                 ref futureUpdatesList);

                updateResult = true;
              }
            }
          }
        }
        else
        {
          updateResult = true;
        }

        UnitOfWork.CommitDefault();
      }

      return updateResult;
    }

    public IList<ContactType> GetContactTypesList(string tabName)
    {
      IList<ContactType> contactTypes = null;

      if (tabName.Equals("Member"))
      {
        contactTypes = ContactTypeRepository.Get(contactType => contactType.Member).ToList();
      }

      return contactTypes;
    }

    /// <summary>
    /// Maps newly configured Aggregators  against a member and removes aggregator member mapping for passed member list
    /// </summary>
    /// <param name="memberId">ID of member for which new aggregator member should be added or existing aggregator member mapping should be removed</param>
    /// <param name="addedAggregatorMembers">List of memberIDs which should be marked as members aggregator by member denoted by member ID</param>
    /// <param name="deletedAggregatorMembers">List of memberIDs which would not be aggregator by member denoted by member ID</param>
    /// <param name="futurePeriod"></param>
    /// <returns>True if list added and removed successfully,false otherwise</returns>
    public bool UpdateAggregators(int memberId, string addedAggregatorMembers, string deletedAggregatorMembers, string futurePeriod, ref List<FutureUpdates> futureUpdatesList)
    {
      bool updateResult = false;
      int oldAggreagatedBy = 0;
      string oldAggreagatedMemberName = null;

      //If future period is specified , add data to future update table else add data to ICH_configuration table

      //Do some thing only if valid member id is passed 
      if (memberId != 0)
      {
        var toBeDeletedMembers = new string[] { };
        var toBeAddedMembers = new string[] { };
        if (!string.IsNullOrEmpty(deletedAggregatorMembers))
        {
          //trim last comma (if present) and split in to id strings
          toBeDeletedMembers = deletedAggregatorMembers.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }
        if (!string.IsNullOrEmpty(addedAggregatorMembers))
        {
          //trim last comma (if present) and split in to id strings
          toBeAddedMembers = addedAggregatorMembers.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }

        var memberCommercialName = GetMemberCommercialName(memberId);

        //Update all existing fu records periods regardless of whether they are in deleted or added list or not
        if (!(toBeAddedMembers.Count() == 0 && toBeDeletedMembers.Count() == 0))
        {
          //get an instance of element group repository
          var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));
          var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)ElementGroupType.Ich);
          var tableName = elementGroup.FirstOrDefault().TableName;
          var strMemberId = memberId.ToString();

          List<FutureUpdates> futureUpdateData = null;
          var futureUpdateRepository = Ioc.Resolve<IFutureUpdatesRepository>(typeof(IFutureUpdatesRepository));
          futureUpdateData =
            futureUpdateRepository.Get(
              futureUpdate =>
              (futureUpdate.NewVAlue == strMemberId || futureUpdate.OldVAlue == strMemberId) && futureUpdate.ElementName == "AGGREGATED_BY" && futureUpdate.TableName == tableName &&
              futureUpdate.IsChangeApplied == false).ToList();

          foreach (var fu in futureUpdateData)
          {
            //if future period is change and
            //if the existing future update record in not present in Add and Remove list and 
            //then update period of future update to list used for sending mails
            if (fu.ChangeEffectivePeriod != Convert.ToDateTime(futurePeriod) && !toBeDeletedMembers.Contains(fu.MemberId.ToString()) && !toBeAddedMembers.Contains(fu.MemberId.ToString()))
            {
              fu.ChangeEffectivePeriod = Convert.ToDateTime(futurePeriod);
              futureUpdateRepository.Update(fu);
              //Add in list to send mails
              futureUpdatesList.Add(fu);
            }
          }
        }

        if (toBeDeletedMembers.Count() > 0)
        {
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            //loop on the list of Aggregated members 
            foreach (string aggregatedMember in toBeDeletedMembers)
            {
              int aggregatedMemberId = Convert.ToInt32(aggregatedMember);

              //check if the sponsored member is already sponsored by this member
              var recordForThisAggregationshipRelationship = IchRepository.Single(member => member.MemberId == aggregatedMemberId && member.AggregatedById == memberId);
              //Check whether pending future updates reocrd is present for this Aggregationship Relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, aggregatedMemberId, "AGGREGATED_BY", memberId.ToString(), null);

              //If future update record for this Aggregationship relation ship is not present
              //or this Aggregationship relationship is already effective then only add or update a future update record
              if ((futureUpdatesRecord == null || futureUpdatesRecord.Count <= 0) && (recordForThisAggregationshipRelationship != null))
              {
                //add sponsored by in future update
                updateResult = UpdateFutureUpdates(ElementGroupType.Ich,
                                                   aggregatedMemberId,
                                                   "AGGREGATED_BY",
                                                   memberId.ToString(),
                                                   null,
                                                   futurePeriod,
                                                   null,
                                                   (int)ActionType.Delete,
                                                   false,
                                                   null,
                                                   memberCommercialName,
                                                   String.Empty,
                                                   ref futureUpdatesList);
              }
              else if (futureUpdatesRecord != null && futureUpdatesRecord.Count > 0)
              {
                FutureUpdatesRepository.Delete(futureUpdatesRecord[0]);
                updateResult = true;
              }
            }
          }
          else
          {
            foreach (string arrStr in toBeDeletedMembers)
            {
              var memId = 0;
              int.TryParse(arrStr, out memId);
              var ichMemberRecordinDb = IchRepository.Single(member => member.MemberId == memId);
              if (ichMemberRecordinDb != null)
              {
                ichMemberRecordinDb.AggregatedById = 0;
                IchRepository.Update(ichMemberRecordinDb);

                updateResult = true;
              }
            }
          }
        }
        else
        {
          updateResult = true;
        }

        //add aggregated members only if a list is passed
        if (toBeAddedMembers.Count() > 0)
        {
          //if a future period is specified then add in future update else we'll add in ich config table
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            //loop on the list of aggregated members
            foreach (string aggreagatedMember in toBeAddedMembers)
            {
              int aggregatedMemberId = Convert.ToInt32(aggreagatedMember);

              //check if the aggregated member is already aggregated by this member
              var recordForThisAggregationshipRelationship = IchRepository.Single(member => member.MemberId == aggregatedMemberId && member.AggregatedById == memberId);
              //Check whether pending future updates reocrd is present for this Aggregationship Relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, aggregatedMemberId, "AGGREGATED_BY", memberId.ToString(), null);

              //If future update record for this Aggregationship relation ship is present
              //or this Aggregationship relationship is not already effective then only add or update a future update record
              if ((futureUpdatesRecord != null && futureUpdatesRecord.Count > 0) || recordForThisAggregationshipRelationship == null)
              {
                //get record of the the aggregated member from db
                var ichaggreagetedmember = IchRepository.Single(member => member.MemberId == aggregatedMemberId);
                //is the aggregated member's ich record present and is an aggregator was set for it?
                //if so then get name of the old aggregator of this member
                if ((ichaggreagetedmember != null) && (ichaggreagetedmember.AggregatedById.HasValue))
                {
                  oldAggreagatedBy = ichaggreagetedmember.AggregatedById.Value;

                  if (oldAggreagatedBy > 0)
                  {
                    //get member name for sponsored member
                    var memberData = MemberRepository.Single(mem => mem.Id == oldAggreagatedBy);
                    if (memberData != null)
                    {
                      oldAggreagatedMemberName = memberData.CommercialName;
                    }
                  }
                }
                //14-Dec-10
                //if ich configuration found for this member then add future update record for it
                if (ichaggreagetedmember != null)
                {
                  //add aggregated by in future update
                  updateResult = UpdateFutureUpdates(ElementGroupType.Ich,
                                                     Convert.ToInt32(aggreagatedMember),
                                                     "AGGREGATED_BY",
                                                     oldAggreagatedBy.ToString(),
                                                     memberId.ToString(),
                                                     futurePeriod,
                                                     null,
                                                     (int)ActionType.Update,
                                                     false,
                                                     null,
                                                     oldAggreagatedMemberName,
                                                     memberCommercialName,
                                                     ref futureUpdatesList);

                  //add an is aggregating dummy record in future updates list to be sent to mail sender
                  updateResult = UpdateFutureUpdates(ElementGroupType.Ich,
                                                     memberId,
                                                     "IS AGGREGATING",
                                                     null,
                                                     aggreagatedMember.ToString(),
                                                     futurePeriod,
                                                     null,
                                                     (int)ActionType.Update,
                                                     true,
                                                     null,
                                                     null,
                                                     memberCommercialName,
                                                     ref futureUpdatesList);
                }
              }
            }
          }
          //else if a future period is not specified then add we'll add in ich config table
          else
          {
            //loop on the list of aggregated members
            foreach (string arrStr in toBeAddedMembers)
            {
              var memId = 0;
              int.TryParse(arrStr, out memId);
              var ichMemberRecordinDb = IchRepository.Single(member => member.MemberId == memId);
              //if record of the aggregated member is found in the ich configuration then update its Aggregetedbyid field
              if (ichMemberRecordinDb != null)
              {
                ichMemberRecordinDb.AggregatedById = memberId;
                IchRepository.Update(ichMemberRecordinDb);

                //Add audit trail records for records inserted in create mode

                AddAuditTrailForImmediateUpdates(ElementGroupType.Ich,
                                                 Convert.ToInt32(memId),
                                                 "AGGREGATED_BY",
                                                 null,
                                                 memberId.ToString(),
                                                 (int)ActionType.Create,
                                                 null,
                                                 "",
                                                 memberCommercialName,
                                                 ref futureUpdatesList);

                updateResult = true;
              }
            }
          }
        }
        else
        {
          updateResult = true;
        }

        UnitOfWork.CommitDefault();
      }

      return updateResult;
    }

    private bool UpdateFutureUpdates(ElementGroupType elementGroupType,
                                     int memberId,
                                     string elementName,
                                     string oldValue,
                                     string newValue,
                                     string futurePeriodValue,
                                     string futureDateValue,
                                     int actionId,
                                     bool isChangeApplied,
                                     int? relationId,
                                     string oldValueDisplayName,
                                     string newValueDisplayName,
                                     ref List<FutureUpdates> futureUpdatesList)
    {
      List<FutureUpdates> futureUpdatesRecord;

      if (futurePeriodValue != null || futureDateValue != null)
      {
        _logger.Debug(string.Format("Future Update old value {0}", oldValue));
        _logger.Debug(string.Format("Future Update new value {0}", newValue));

        _logger.Debug(string.Format("Future Update for {0}", elementName));

        if (oldValue != newValue)
        {
          _logger.Debug("Inside new value check");
          if ((elementName == "EXCEPTION_MEMBER_ID") || (elementName == "COUNTRY_CODE"))
          {
            futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(elementGroupType, memberId, elementName, newValue, relationId);
          }
          else
          {
            _logger.Debug("Getting pending future update records");
            //If future update record already exists for a field, then update it else create new record
            futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(elementGroupType, memberId, elementName, relationId);
          }

          if ((futureUpdatesRecord != null) && (futureUpdatesRecord.Count > 0))
          {
            //This condition is added to check if user has changed Future Dated Value or Future Date/Period
            //Then only we need to update this records data
            if (futureUpdatesRecord.FirstOrDefault().NewVAlue != newValue || (futureDateValue != null && futureUpdatesRecord[0].ChangeEffectiveOn.Value != Convert.ToDateTime(futureDateValue)) ||
                (futurePeriodValue != null && futureUpdatesRecord[0].ChangeEffectivePeriod.Value != Convert.ToDateTime(futurePeriodValue)))
            {
              //Pending Future update record exists for the specific element so update the record
              _logger.Debug("Update future update object as it already exists");
              FutureUpdates updates = futureUpdatesRecord[0];
              var futureUpdates = SetFutureUpdateObject(ref updates,
                                                        elementGroupType,
                                                        oldValue,
                                                        newValue,
                                                        futurePeriodValue,
                                                        futureDateValue,
                                                        actionId,
                                                        isChangeApplied,
                                                        relationId,
                                                        oldValueDisplayName,
                                                        newValueDisplayName);
              futureUpdates = FutureUpdatesRepository.Update(updates);

              _logger.Debug(string.Format("Member object for Future Update {0} - {1} is {2}", futureUpdates.TableName, futureUpdates.ElementName, futureUpdates.Member == null ? "null" : "not null"));
              futureUpdatesList.Add(futureUpdates);
           }
          }
          else
          {
            _logger.Debug("Get new future update object");
            //Form future update object and add new record to future updates audit trail table
            var futureUpdates = GetFutureUpdateObject(0,
                                                      memberId,
                                                      elementGroupType,
                                                      elementName,
                                                      oldValue,
                                                      newValue,
                                                      futurePeriodValue,
                                                      futureDateValue,
                                                      actionId,
                                                      isChangeApplied,
                                                      relationId,
                                                      oldValueDisplayName,
                                                      newValueDisplayName);
            _logger.Debug(string.Format("Member object for Future Update {0} - {1} is {2}", futureUpdates.TableName, futureUpdates.ElementName, futureUpdates.Member == null ? "null" : "not null"));

            futureUpdatesList.Add(futureUpdates);
            if ((elementName != "IS SPONSORING") && (elementName != "IS AGGREGATING"))
            {
              FutureUpdatesRepository.Add(futureUpdates);
            }
          }
        }
        else
        {
          //There is no future period value specified
          _logger.Debug(string.Format("Returning flag {0}", "true"));
          return true;
        }
      }

      _logger.Debug(string.Format("Returning flag {0}", "true"));

      return true;
    }

    private static string FormatFuturePeriodValue(DateTime? changeEffectivePeriod)
    {
      return (changeEffectivePeriod != null ? changeEffectivePeriod.Value.ToString("yyyy-MMM-dd") : string.Empty);
    }

    private static string FormatFutureDateValue(DateTime? changeEffectiveValue)
    {
      return (changeEffectiveValue != null ? changeEffectiveValue.Value.ToString("dd-MMM-yy") : string.Empty);
    }

    private static string FormatFutureValues(string futureValue)
    {
      return futureValue != null ? Convert.ToDateTime(futureValue).ToString("yyyy-MMM-dd") : null;
    }

    /// <summary> 
    /// Adds contact-contactType record in contactTypeMatrix.
    /// </summary>
    /// <param name="contactContactTypeMatrix">The contacttypeMatrix record.</param>
    /// <returns>Added record.</returns>
    private ContactContactTypeMatrix AddContactTypeMatrix(ContactContactTypeMatrix contactContactTypeMatrix)
    {
      var contactContactTypeMatrixRecord = ContactTypeMatrixRepository.Single(ml => ml.ContactId == contactContactTypeMatrix.ContactId && ml.ContactTypeId == contactContactTypeMatrix.ContactTypeId);

      if (contactContactTypeMatrixRecord == null)
      {
        ContactTypeMatrixRepository.Add(contactContactTypeMatrix);
        UnitOfWork.CommitDefault();
      }

      return contactContactTypeMatrix;
    }

    /// <summary>
    /// Delete contactCobtactTypeMatrix record.
    /// </summary>
    /// <param name="contactContactTypeMatrix">contactContactTypeMatrix record to be deleted.</param>
    private void DeleteContactTypeMatrix(ContactContactTypeMatrix contactContactTypeMatrix)
    {
      var contactContactTypeMatrixRecord = ContactTypeMatrixRepository.Single(ml => ml.ContactId == contactContactTypeMatrix.ContactId && ml.ContactTypeId == contactContactTypeMatrix.ContactTypeId);
      if (contactContactTypeMatrixRecord != null)
      {
        ContactTypeMatrixRepository.Delete(contactContactTypeMatrixRecord);
        UnitOfWork.CommitDefault();
      }
    }

    /// <summary>
    /// Update ContactContactTypeMatrix record.
    /// </summary>
    /// <param name="contactAssignmentList">ContactContactTypeMatrix record to be deleted.</param>
    public string UpdateContactContactTypeMatrix(string contactAssignmentList, string ichContactTypes)
    {
      int memberId = 0;

      string[] contactAssignments = contactAssignmentList.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

      // This for loop will only Assign contact Type
      foreach (var contactAssign in contactAssignments)
      {
        string[] contactTypeAssignments = contactAssign.Split("!".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (contactTypeAssignments.Length == 2)
        {
          var contactId = int.Parse(contactTypeAssignments[0]);

          // Retrieve memberId for current contactId 
          memberId = ContactsRepository.Single(c => c.Id == contactId).MemberId;

          //Get contact types
          string[] assignments = contactTypeAssignments[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

          foreach (var assignment in assignments)
          {
            int contactTypeIndex = assignment.IndexOf('_');
            int contactTypeId = int.Parse(assignment.Substring(0, contactTypeIndex));
            var value = bool.Parse(assignment.Substring(contactTypeIndex + 1));

            var contactContactTypeMatrixRecord = new ContactContactTypeMatrix { ContactId = contactId, ContactTypeId = contactTypeId };
            if (value)
            {
              var result = AddContactTypeMatrix(contactContactTypeMatrixRecord);
            }

          }
        }
      } // end foreach


      var missingRequiredContactTypeId = string.Empty;
      // This for loop will only delete contact Type
      foreach (var contactAssign in contactAssignments)
      {

        string[] contactTypeAssignments = contactAssign.Split("!".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (contactTypeAssignments.Length == 2)
        {
          var contactId = int.Parse(contactTypeAssignments[0]);

          // Retrieve memberId for current contactId 
          memberId = ContactsRepository.Single(c => c.Id == contactId).MemberId;

          //Get contact types
          string[] assignments = contactTypeAssignments[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

          foreach (var assignment in assignments)
          {

            int contactTypeIndex = assignment.IndexOf('_');
            int contactTypeId = int.Parse(assignment.Substring(0, contactTypeIndex));
            var value = bool.Parse(assignment.Substring(contactTypeIndex + 1));

            var contactContactTypeMatrixRecord = new ContactContactTypeMatrix { ContactId = contactId, ContactTypeId = contactTypeId };
            if (value)
            {
              // nothing to do
            }
            else
            {
              var isOnlyContact = IsOnlyContactAssigned(0, memberId, contactContactTypeMatrixRecord.ContactTypeId);
              //SCPIDID : 112924 - Only contact assigned to specific Contact Type marked as Inactive - no error [TC:073575]
              if (isOnlyContact)
              {
                var contactType =
                    ContactTypeRepository.Single(
                        c => c.Id == contactContactTypeMatrixRecord.ContactTypeId);
                if (contactType != null)
                {
                  if (!missingRequiredContactTypeId.Contains(contactType.Id.ToString()))
                  {
                    missingRequiredContactTypeId += contactType.Id + ",";
                  }

                  // return contactType.ContactTypeName;
                }

              }
              else
              {
                if (!CheckContactTypeMarked(contactTypeId, contactAssignments))
                {
                  var contactType =
                  ContactTypeRepository.Single(
                      c => c.Id == contactContactTypeMatrixRecord.ContactTypeId && c.Required == true);

                  // Get count of totaL contact assigned to contact type  
                  var contactTypeMatrix =
                    ContactTypeMatrixRepository.Get(c => c.ContactTypeId == contactContactTypeMatrixRecord.ContactTypeId && c.Contact.MemberId == memberId && c.ContactType.Required == true);

                  var contactTypeMatrixCount = contactTypeMatrix.Count();

                  if (contactTypeMatrixCount == 1)
                  {
                    var onlyContactId = 0;
                    foreach (var objContactTypeMatrix in contactTypeMatrix)
                    {
                      onlyContactId = objContactTypeMatrix.ContactId;
                    }

                    if (onlyContactId == contactId)
                    {

                      if (!missingRequiredContactTypeId.Contains(contactType.Id.ToString()))
                      {
                        missingRequiredContactTypeId += contactType.Id + ",";
                      }
                    }
                  }


                  if (contactType != null && contactTypeMatrixCount == 0)
                  {

                    if (!missingRequiredContactTypeId.Contains(contactType.Id.ToString()))
                    {
                      missingRequiredContactTypeId += contactType.Id + ",";
                    }

                    //  return contactType.ContactTypeName;
                  }
                  else
                  {
                    DeleteContactTypeMatrix(contactContactTypeMatrixRecord);
                  }
                }
                else
                {
                  DeleteContactTypeMatrix(contactContactTypeMatrixRecord);
                }

              }

            }


          }
        }
      } // end foreach

      if (!string.IsNullOrEmpty(missingRequiredContactTypeId))
      {
        if (missingRequiredContactTypeId.Length > 0)
        {
          missingRequiredContactTypeId = missingRequiredContactTypeId.Remove(missingRequiredContactTypeId.Length - 1, 1);
          return missingRequiredContactTypeId;
        }
      }
      // If ichContactTypes string equals true call InsertMessageInOracleQueue() method which will insert member 
      // profile update message in the queue
      if (ichContactTypes.Equals("true"))
      {
        InsertMessageInOracleQueue("MemberProfileUpdate", memberId);
        SendMailToIchForMemberProfileUpdate(memberId);
      } // end if()
      return string.Empty;
    }


   /// <summary>
    /// Gets exception member data for a specific member
    /// </summary>
    /// <param name="memberId">Id of member for which exception data should be fetched form database</param>
    /// <param name="exbillingCategoryId"></param>
    /// <returns>List of ACHException class objects</returns>
    public IList<AchException> GetExceptionMembers(int memberId, int exbillingCategoryId, bool includeFutureUpdates)
    {
      AchException futureExceptionMember;

      IList<AchException> achExceptionMembers = AchExceptionRepository.Get(achException => achException.MemberId == memberId && achException.BillingCategoryId == exbillingCategoryId).ToList();

      //TODO: in ACHExceptionRepository, include member object
      foreach (var achException in achExceptionMembers)
      {
        var mem = MemberRepository.Single(member => member.Id == achException.ExceptionMemberId);

        if (mem != null)
        {
          achException.ExceptionMemberCommercialName = mem.DisplayCommercialName;
        }
      }

      // Only include future updates if they are required.
      if (includeFutureUpdates)
      {
        //If future update record exist in database where passed memberId is a sponsoror then get all these records from database
        var futureExceptionsList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ach, memberId, "EXCEPTION_MEMBER_ID", exbillingCategoryId);

        //We need to add future sponsored members to current sponsored members
        if (futureExceptionsList != null)
        {
            foreach (var exceptionMember in futureExceptionsList)
            {
                //SCP482581 - Member Profile XML corruption - aggregator and aggregated / wrong period and member not removed
                //If Action Type Create then add exception member in the list else if Action Type Delete then remove exception member from list.
                if (exceptionMember.ActionType == ActionType.Create)
                {
                    int exceptionMemberValueId = Convert.ToInt32(exceptionMember.NewVAlue);
                    var member = MemberRepository.Single(mem => mem.Id == exceptionMemberValueId);
                    if (member != null)
                    {
                        futureExceptionMember = new AchException();
                        futureExceptionMember.ExceptionMemberCommercialName = member.DisplayCommercialName;
                        futureExceptionMember.ExceptionMemberId = member.Id;
                        achExceptionMembers.Add(futureExceptionMember);
                    }
                }
                else if (exceptionMember.ActionType == ActionType.Delete)
                {
                    int exceptionMemberValueId = Convert.ToInt32(exceptionMember.OldVAlue);

                    achExceptionMembers.Remove(
                        achExceptionMembers.Where(ach => ach.ExceptionMemberId == exceptionMemberValueId).FirstOrDefault
                            ());
                }
            }
        }
      }
      return achExceptionMembers;
    }

    /// <summary>
    /// Get Sponsored List
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="includeFutureUpdates"></param>
    /// <returns></returns>
    public IList<IchConfiguration> GetSponsoredList(int memberId, bool includeFutureUpdates)
    {
      IchConfiguration futureSponsor;

      // Get all current sponsored members
      IList<IchConfiguration> sponsors = IchRepository.Get(ich => ich.SponsoredById == memberId).ToList();
      foreach (var sponsor in sponsors)
      {
        if (sponsor != null)
        {
          var member = MemberRepository.Single(mem => mem.Id == sponsor.MemberId);
          sponsor.CommercialName = member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" + member.CommercialName;
        }
      }

      // Only include future updates if they are required.
      if (includeFutureUpdates)
      {
          //If future update record exist in database where passed memberId is a sponsoror then get all these records from database
          var futureSponsoredList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "SPONSORED_BY",
                                                                                 memberId.ToString(), null);

          //We need to add future sponsored members to current sponsored members
          if (futureSponsoredList != null)
          {
              foreach (var sponsoredMember in futureSponsoredList)
              {
                  var member = MemberRepository.Single(mem => mem.Id == sponsoredMember.MemberId);
                  if (member != null)
                  {
                      futureSponsor = new IchConfiguration();
                      futureSponsor.CommercialName = member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" +
                                                     member.CommercialName;
                      futureSponsor.MemberId = member.Id;
                      sponsors.Add(futureSponsor);
                  }
              }
          }

          //SCP482581 - Member Profile XML corruption - aggregator and aggregated / wrong period and member not removed
          //Get future removed sponsored list from database
          var futureRemovedSponsoredList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0,
                                                                                        "SPONSORED_BY", null, null,
                                                                                        memberId.ToString());

          if (futureRemovedSponsoredList != null)
          {
              foreach (var sponsoredMember in futureRemovedSponsoredList)
              {
                  //Remove list from sponsors list.
                  sponsors.Remove(sponsors.Where(s => s.MemberId == sponsoredMember.MemberId).FirstOrDefault());
              }
          }

      }

        return sponsors;
    }

    /// <summary>
    /// Get Aggregator list
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="includeFutureUpdates"></param>
    /// <returns></returns>
    public IList<IchConfiguration> GetAggregatorsList(int memberId, bool includeFutureUpdates)
    {
      IchConfiguration futureAggregator;

      // Get all current aggregated members
      IList<IchConfiguration> aggregators = IchRepository.Get(ich => ich.AggregatedById == memberId).ToList();
      foreach (var aggregator in aggregators)
      {
        if (aggregator != null)
        {
          var member = MemberRepository.Single(mem => mem.Id == aggregator.MemberId);
          aggregator.CommercialName = member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" + member.CommercialName; ;


        }
      }

      // Only include future updates if they are required.
      if (includeFutureUpdates)
      {
        //If future update record exist in database where passed memberId is a aggregator then get all these records from database
        var futureAggregatedList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "AGGREGATED_BY", memberId.ToString(), null);

        //We need to add future aggregated members to current aggregated members
        if (futureAggregatedList != null)
        {
          foreach (var aggregatedMember in futureAggregatedList)
          {
            var member = MemberRepository.Single(mem => mem.Id == aggregatedMember.MemberId);
            if (member != null)
            {
              futureAggregator = new IchConfiguration();
              futureAggregator.CommercialName = member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" + member.CommercialName; ;
              futureAggregator.MemberId = member.Id;
              aggregators.Add(futureAggregator);
            }
          }
        }

                //SCP482581 - Member Profile XML corruption - aggregator and aggregated / wrong period and member not removed
                //Get future removed sponsored list from database
                var futureRemovedAggregatedList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ich, 0, "AGGREGATED_BY", null, null, memberId.ToString());

                if (futureRemovedAggregatedList != null)
                {
                    foreach (var aggregatedMember in futureRemovedAggregatedList)
                    {
                        //Remove list from sponsors list.
                        aggregators.Remove(aggregators.Where(s => s.MemberId == aggregatedMember.MemberId).FirstOrDefault());
                    }
                }

      }

      return aggregators;
    }

    /// <summary>
    /// Maps newly configured exception members and removes exception members mapping for passed member list against a member
    /// </summary>
    /// <param name="memberId">ID of member for which new exception members should be added or existing exception members mapping should be removed</param>
    /// <param name="addedExceptionMembers">List of memberIDs which should be marked as exception members</param>
    /// <param name="deletedExceptionMembers">List of memberIDs which would not be exception members</param>
    /// <param name="billingCategoryId">Billing category id for which exceptions should be marked</param>
    /// <returns>True if list added and removed successfully,false otherwise</returns>
    public bool UpdateACHExceptionMembers(int memberId,
                                          string addedExceptionMembers,
                                          string deletedExceptionMembers,
                                          int billingCategoryId,
                                          string futurePeriod,
                                          ref List<FutureUpdates> futureUpdatesList)
    {
      AchException achException = null;
      bool updateResult = false;
      string exceptionMemberName = String.Empty;
      //If future period is specified , add data to future update table else add data to MEM_ACH_EXCEPTION table
      //Do some thing only if valid member id is passed 
      if (memberId != 0)
      {
        var toBeDeletedMembers = new string[] { };
        var toBeAddedMembers = new string[] { };
        if (!string.IsNullOrEmpty(deletedExceptionMembers))
        {
          //trim last comma (if present) and split in to id strings
          toBeDeletedMembers = deletedExceptionMembers.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }
        if (!string.IsNullOrEmpty(addedExceptionMembers))
        {
          //trim last comma (if present) and split in to id strings
          toBeAddedMembers = addedExceptionMembers.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }

        //Update all existing fu records' periods regardless of whether they are in deleted or added list or not
        if (!(toBeAddedMembers.Count() == 0 && toBeDeletedMembers.Count() == 0))



        {
          //get an instance of element group repository
          var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));
          var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)ElementGroupType.Ach);
          var tableName = elementGroup.FirstOrDefault().TableName;
          elementGroupRepository = null;
          elementGroup = null;

          List<FutureUpdates> futureUpdateData = null;
          var futureUpdateRepository = Ioc.Resolve<IFutureUpdatesRepository>(typeof(IFutureUpdatesRepository));
          futureUpdateData =
            futureUpdateRepository.Get(
              futureUpdate =>
              futureUpdate.MemberId == memberId && futureUpdate.ElementName == "EXCEPTION_MEMBER_ID" && futureUpdate.TableName == tableName && futureUpdate.RelationId == billingCategoryId &&
              futureUpdate.IsChangeApplied == false).ToList();

          foreach (var fu in futureUpdateData)
          {
            //if future period is change and
            //if the existing future update record in not present in Add and Remove list and 
            //then update period of future update to list used for sending mails
            if (fu.ChangeEffectivePeriod != Convert.ToDateTime(futurePeriod) && !toBeDeletedMembers.Contains(fu.OldVAlue) && !toBeAddedMembers.Contains(fu.NewVAlue))
            {
              fu.ChangeEffectivePeriod = Convert.ToDateTime(futurePeriod);
              futureUpdateRepository.Update(fu);
              //Add in list to send mails
              futureUpdatesList.Add(fu);
            }
          }
          futureUpdateRepository = null;
        }

        if (toBeDeletedMembers.Count() > 0)
        {
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            //loop on the list of exceptions
            foreach (string exceptionMember in toBeDeletedMembers)
            {
              int exceptionMemberId = Convert.ToInt32(exceptionMember);

              //check if exception already exists for the member
              var recordForThisRelationship =
                AchExceptionRepository.Single(
                  existingachException =>
                  existingachException.MemberId == memberId && existingachException.ExceptionMemberId == exceptionMemberId && existingachException.BillingCategoryId == billingCategoryId);

              //Check whether pending future updates are present for this ach exception relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ach, memberId, "EXCEPTION_MEMBER_ID", exceptionMember, billingCategoryId);

              //If future update record for this ACH Exception relationship is not present
              //and thisACH Exception relationship is already effective then only add or update a future update record
              if ((futureUpdatesRecord == null || futureUpdatesRecord.Count <= 0) && (recordForThisRelationship != null))
              {
                //get member name for exception member
                var memberData = MemberRepository.Single(mem => mem.Id == exceptionMemberId);

                if (memberData == null)
                {
                  return false;



                }
                exceptionMemberName = memberData.CommercialName;

                //add ACH Exceptio relationship deletion record in future update
                updateResult = UpdateFutureUpdates(ElementGroupType.Ach,
                                                   memberId,
                                                   "EXCEPTION_MEMBER_ID",
                                                   exceptionMember,
                                                   null,
                                                   futurePeriod,
                                                   null,
                                                   (int)ActionType.Delete,
                                                   false,
                                                   billingCategoryId,
                                                   exceptionMemberName,
                                                   null,
                                                   ref futureUpdatesList);
              }
              else if (futureUpdatesRecord != null && futureUpdatesRecord.Count > 0)
              {
                FutureUpdatesRepository.Delete(futureUpdatesRecord[0]);
                updateResult = true;
              }
            }
          }
          else
          {
            //loop on the list of exceptions
            foreach (string arrStr in toBeDeletedMembers)
            {
              var exceptionMemId = 0;
              int.TryParse(arrStr, out exceptionMemId);

              var achExceptionRecord =
                AchExceptionRepository.Single(
                  existingachException =>
                  existingachException.MemberId == memberId && existingachException.ExceptionMemberId == exceptionMemId && existingachException.BillingCategoryId == billingCategoryId);


              if (achExceptionRecord != null)
              {
                AchExceptionRepository.Delete(achExceptionRecord);
              }



            }
          }
        }
        else
        {
          updateResult = true;
        }

        //add exceptions only if a list is passed
        if (toBeAddedMembers.Count() > 0)



        {
          //if a future period is specified then add in future update else we'll add in ACH Exception table
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            //loop on the list of countries
            foreach (string exceptionMember in toBeAddedMembers)
            {
              int exceptionMemberId = Convert.ToInt32(exceptionMember);







              //check if exception already exists for the member
              var recordForThisRelationship =
                AchExceptionRepository.Single(
                  existingachException =>
                  existingachException.MemberId == memberId && existingachException.ExceptionMemberId == exceptionMemberId && existingachException.BillingCategoryId == billingCategoryId);

              //Check whether pending future updates are present for this ach exception relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Ach, memberId, "EXCEPTION_MEMBER_ID", exceptionMember, billingCategoryId);

              //If future update record for this Exception relationship is present
              //or this Exception relation ship is not already effective then only add or update a future update record
              if ((futureUpdatesRecord != null && futureUpdatesRecord.Count > 0) || recordForThisRelationship == null)
              {
                //get member name for exception member
                var memberData = MemberRepository.Single(mem => mem.Id == exceptionMemberId);
                if (memberData == null)
                {
                  return false;



                }
                exceptionMemberName = memberData.CommercialName;

                updateResult = UpdateFutureUpdates(ElementGroupType.Ach,
                                                   memberId,
                                                   "EXCEPTION_MEMBER_ID",
                                                   null,
                                                   exceptionMemberId.ToString(),
                                                   futurePeriod,
                                                   null,
                                                   (int)ActionType.Create,
                                                   false,
                                                   billingCategoryId,
                                                   null,
                                                   exceptionMemberName,
                                                   ref futureUpdatesList);
              }
            }
          }
          else
          {
            //loop on the list of countries
            foreach (string arrStr in toBeAddedMembers)
            {
              var exceptionMemId = 0;
              int.TryParse(arrStr, out exceptionMemId);

              var memberData = MemberRepository.Single(mem => mem.Id == exceptionMemId);
              if (memberData == null)
              {
                return false;
              }
              exceptionMemberName = memberData.CommercialName;

              //Todo: Remove hardcoded 1 and replace with logged in user id
              achException = new AchException { BillingCategoryId = billingCategoryId, MemberId = memberId, ExceptionMemberId = exceptionMemId, LastUpdatedBy = 1, LastUpdatedOn = DateTime.UtcNow };

              AchExceptionRepository.Add(achException);

              //Add audit trail records for records inserted in create mode

              AddAuditTrailForImmediateUpdates(ElementGroupType.Ach,
                                               memberId,
                                               "EXCEPTION_MEMBER_ID",
                                               null,
                                               exceptionMemId.ToString(),
                                               (int)ActionType.Create,
                                               billingCategoryId,
                                               "",
                                               exceptionMemberName,
                                               ref futureUpdatesList);
            }
          }
        }
        else
        {
          updateResult = true;
        }
      }
      else
      {
        return false;
      }

      UnitOfWork.CommitDefault();
      return true;
    }

    /// <summary>
    /// UpdateMemberStatusHistory 
    /// </summary>
    /// <param name="updatedStatusDate"></param>
    /// <param name="member"></param>
    // SCP196804: IS Entry date changed when updates are done to the locations
    // Desc: Input date time parameter is changed to nullable value. This is used to pass null and bypass the code when not required.
    private void UpdateMemberStatusHistory(DateTime? updatedStatusDateNullable, Member member)
    {
        DateTime updatedStatusDate = DateTime.UtcNow;
        if (updatedStatusDateNullable.HasValue)
        {
            updatedStatusDate = updatedStatusDateNullable.Value;

            var statusDetail =
                MemberStatusRepository.Get(memStatus => memStatus.MemberId == member.Id && memStatus.MemberType == "MEM");
            var flag = false;

            var recentStatusDetails = statusDetail.OrderByDescending(md => md.Id).Take(1);
            if (recentStatusDetails.Count() == 1)
            {
                var dbCurrentMemberStatusId = recentStatusDetails.FirstOrDefault().MembershipStatusId;

                if (dbCurrentMemberStatusId == member.IsMembershipStatusId)
                {
                    var memberStatusDetails =
                        recentStatusDetails.Take(1).FirstOrDefault(
                            m =>
                            m.StatusChangeDate == updatedStatusDate &&
                            m.MembershipStatusId == member.IsMembershipStatusId);

                    if (memberStatusDetails != null)
                    {
                        flag = true;
                    }
                }
            }

            if (!flag)
            {
                var memberStatus = new MemberStatusDetails
                                       {
                                           MemberId = member.Id,
                                           MembershipStatusId = member.IsMembershipStatusId,
                                           StatusChangeDate = updatedStatusDate,
                                           MemberType = "MEM",
                                           LastUpdatedOn = DateTime.UtcNow
                                       };
                MemberStatusRepository.Add(memberStatus);
            }
        }
    }

    /// <summary>
    /// This method is used for retrieving all Status records of a member for ICH configuration,Ach Configration or member details configuration
    /// </summary>
    public List<MemberStatusDetails> GetMemberStatus(int memberId, string memberType)
    {
      var memberDetails = MemberStatusRepository.Get(member => member.MemberId == memberId && member.MemberType.Equals(memberType)).OrderBy(member => member.LastUpdatedOn).ToList();

      return memberDetails;
    }

    /// <summary>
    /// Retrieves contact type name corresponding to contact type id passed
    /// </summary>
    /// <param name="contactTypeId">contact type ID for which contact type name needs to be determined</param>
    /// <returns>contact type name</returns>
    public string GetContactTypeName(int contactTypeId)
    {
      var contactType = ContactTypeRepository.Single(contType => contType.Id == contactTypeId);

      if (contactType != null)
      {
        var contactTypeName = contactType.ContactTypeName;
        return contactTypeName;
      }

      return string.Empty;
    }

    /// <summary>
    /// Method which generates locationCode for passed cityName
    /// </summary>
    private string GetLocationCode(string cityName, int memberId)
    {
      string locationCode;

      //SCP234058 - FW: The matter of setting Locations in ISWEB
      //Before, Sorting by location_id.
      var locationRecord =
        LocationRepository.Get(location => location.MemberId == memberId && location.LocationCode != MainLocation && location.LocationCode != UatpLocation).ToList().OrderByDescending(location => Convert.ToUInt32(location.LocationCode)).
          FirstOrDefault();

      if (locationRecord == null)
      {
        // Modified existing logic for LocationCode generation
        // 0's would not be appended.Only sequence number denoting count of locations for that member will be used in LocationCode
        locationCode = "1";
      }
      else
      {
        var sequenceNumber = int.Parse(locationRecord.LocationCode) + 1;
        locationCode = sequenceNumber.ToString();
      }

      return locationCode;
    }

    /// <summary>
    ///  Member control fields update functionality
    /// </summary>
    public Member UpdateMemberControl(int memberId, Member member)
    {
      var memberData = MemberRepository.Single(memb => memb.Id == memberId);

      if (memberData != null)
      {
        var auditProcessor = new AuditProcessor<Member>();

        // Get the list of pending future updates.
        var pendingUpdates = FutureUpdatesManager.GetPendingFutureUpdates(memberId, ElementGroupType.SISOperations);

        // Ascertain the audit trail entries.
        var memberFutureUpdatesList = auditProcessor.ProcessAuditEntries(memberId, ActionType.Update, ElementGroupType.SISOperations, memberData, member, UserId, pendingUpdates);

       
        // Update the dependent fields in the passenger config to the database.
        try
        {
          var paxConfig = PassengerRepository.Single(pax => pax.MemberId == memberId);

          /*DEPENDENT FIELDS RELATED CHANGE START*/
          if (memberData.IsParticipateInValueDetermination != member.IsParticipateInValueDetermination)
          {
            if (!member.IsParticipateInValueDetermination)
            {
              if (paxConfig != null)
              {
                paxConfig.IsParticipateInAutoBilling = false;
                paxConfig.InvoiceNumberRangeFrom = null;
                paxConfig.InvoiceNumberRangeTo = null;
                paxConfig.InvoiceNumberRangePrefix = null;
              }
            }
          }

          if (memberData.IsParticipateInValueConfirmation != member.IsParticipateInValueConfirmation)
          {
            if (!member.IsParticipateInValueConfirmation)
            {
              if (paxConfig != null)
              {
                var oldIsAutomated = paxConfig.IsAutomatedVcDetailsReportRequired;
                paxConfig.IsAutomatedVcDetailsReportRequired = false;
                AddAuditTrailForImmediateUpdates(ElementGroupType.Pax,
                                                 memberId,
                                                 "IS_AUTOMATED_VC_DET_RPT_REQ",
                                                 oldIsAutomated.ToString(),
                                                 false.ToString(),
                                                 (int)ActionType.Update,
                                                 null,
                                                 null,
                                                 null,
                                                 ref memberFutureUpdatesList);
              }
            }
          }

          if (paxConfig != null)
          {
            if ((memberData.IsParticipateInValueConfirmation != member.IsParticipateInValueConfirmation) || (memberData.IsParticipateInValueDetermination != member.IsParticipateInValueDetermination))
            {
              if ((!member.IsParticipateInValueConfirmation) || (!member.IsParticipateInValueDetermination))
              {
                PassengerRepository.Update(paxConfig);
              }
            }
          }

          //If value of 'PaxOldIdecMember' is changed to false then change value field 'Down Convert IS Tran to Old IDEC format' field to false
          if ((memberData.PaxOldIdecMember != member.PaxOldIdecMember) && (member.PaxOldIdecMember == false))
          {
            var passengerData = PassengerRepository.Single(pax => pax.MemberId == memberId);

            if ((passengerData != null) && (passengerData.DownConvertISTranToOldIdec == true))
            {
              passengerData.DownConvertISTranToOldIdec = false;
              PassengerRepository.Update(passengerData);
            }
          }

          //If value of 'CgoOldIdecMember' is changed to false then change value field 'Down Convert IS Tran to Old IDEC format' field to false
          if ((memberData.CgoOldIdecMember != member.CgoOldIdecMember) && (member.CgoOldIdecMember == false))
          {
            var cargoData = CargoRepository.Single(pax => pax.MemberId == memberId);

            if ((cargoData != null) && (cargoData.DownConvertISTranToOldIdeccgo == true))
            {
              cargoData.DownConvertISTranToOldIdeccgo = false;
              CargoRepository.Update(cargoData);






            }
          }
          /*DEPENDENT FIELDS RELATED CHANGE END*/
        }
        catch (ISBusinessException ex)
        {
          throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails, "UpdateMemberControl failed.", ex);
        }

        // If DS signature application value is false then check whether future period value set for field 'IS_UATP_INV_IGNORE_FROM_DSPROC' present on UATP tab
        // is less than period value set for field digital signature application
        try



        {
          if ((member.DigitalSignApplicationFutureValue != null) && (member.DigitalSignApplicationFutureValue == false))
          {
            var futureUpdates = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Uatp, memberId, "IS_UATP_INV_IGNORE_FROM_DSPROC", null);
            if ((futureUpdates != null) && (futureUpdates.Count > 0))
            {
              foreach (var update in futureUpdates)
              {
                if (Convert.ToDateTime(update.ChangeEffectivePeriod) > Convert.ToDateTime(member.DigitalSignApplicationFuturePeriod))
                {
                  FutureUpdatesRepository.Delete(update);
                }
              }
            }
          }

       if ((member.DigitalSignApplicationFutureValue != null) && (member.DigitalSignApplicationFutureValue == true))
          {
            var futureUpdates = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.Uatp, memberId, "IS_UATP_INV_IGNORE_FROM_DSPROC", null);
            if ((futureUpdates != null) && (futureUpdates.Count > 0))
            {
              foreach (var update in futureUpdates)
              {
                if (Convert.ToDateTime(update.ChangeEffectivePeriod) < Convert.ToDateTime(member.DigitalSignApplicationFuturePeriod))
                {
                  throw new ISBusinessException(ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError);
                }
              }
            }
          }

          // When Legal Archiving required is set to false, dependent attributes in eBilling configuration should be disabled
          if ((member.LegalArchivingRequired == false) && (memberData.LegalArchivingRequired == true))
          {
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_PAX_REC_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_PAX_PAY_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_CGO_REC_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_CGO_PAY_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_MISC_REC_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_MISC_PAY_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_UATP_REC_INV");
            DeleteFutureUpdate(memberId, member, "LEGAL_ARC_REQ_UATP_PAY_INV");

            var eBilling = EBillingRepository.Single(ebilling => ebilling.MemberId == memberId);

            if (eBilling != null)
            {
              eBilling.LegalArchRequiredforPaxRecInv = false;
              eBilling.LegalArchRequiredforPaxPayInv = false;
              eBilling.LegalArchRequiredforCgoRecInv = false;
              eBilling.LegalArchRequiredforCgoPayInv = false;
              eBilling.LegalArchRequiredforMiscRecInv = false;
              eBilling.LegalArchRequiredforMiscPayInv = false;
              eBilling.LegalArchRequiredforUatpRecInv = false;
              eBilling.LegalArchRequiredforUatpPayInv = false;

              //231363: SRM: CDC
              //When the control field is turned off in tab SIS Ops, then dependent fields in tab e-Billing will be automatically and forcibly set to False. 
              eBilling.IncludeListingsPaxRecArch = false;
              eBilling.IncludeListingsPaxPayArch = false;
              eBilling.IncludeListingsCgoRecArch = false;
              eBilling.IncludeListingsCgoPayArch = false;
              eBilling.IncludeListingsMiscRecArch = false;
              eBilling.IncludeListingsMiscPayArch = false;
              eBilling.IncludeListingsUatpRecArch = false;
              eBilling.IncludeListingsUatpPayArch = false;
            }
          }
        }
        catch (ISBusinessException ex)
        {
          _logger.Debug("UpdateMemberControl failed.", ex);
          if (ex.ErrorCode == ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError)
          {
            throw new ISBusinessException(ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError);
          }

          throw new ISBusinessException(ErrorCodes.FutureUpdateErrorEBilling);


        }

        memberData.EnableEInvWgInfoContact = member.EnableEInvWgInfoContact;
        memberData.EnableFirstNFinalInfoContact = member.EnableFirstNFinalInfoContact;
        memberData.EnableFnfAiaInfoContact = member.EnableFnfAiaInfoContact;
        memberData.EnableFnfAsgInfoContact = member.EnableFnfAsgInfoContact;
        memberData.EnableIawgMemInfoContact = member.EnableIawgMemInfoContact;
        memberData.EnableIchPanelInfoContact = member.EnableIchPanelInfoContact;
        memberData.EnableOldIdecScInfoContact = member.EnableOldIdecScInfoContact;
        memberData.EnableRawgMemInfoContact = member.EnableRawgMemInfoContact;
        memberData.EnableSampMemInfoContact = member.EnableSampMemInfoContact;
        memberData.EnableSampQSmartInfoContact = member.EnableSampQSmartInfoContact;
        memberData.EnableSampScMemInfoContact = member.EnableSampScMemInfoContact;
        memberData.EnableSisScInfoContact = member.EnableSisScInfoContact;
        memberData.AllowContactDetailsDownload = member.AllowContactDetailsDownload;
        memberData.IsParticipateInValueConfirmation = member.IsParticipateInValueConfirmation;
        memberData.IsParticipateInValueDetermination = member.IsParticipateInValueDetermination;
        memberData.LegalArchivingRequired = member.LegalArchivingRequired;
        memberData.PaxOldIdecMember = member.PaxOldIdecMember;
        memberData.CgoOldIdecMember = member.CgoOldIdecMember;
        memberData.ActualMergerDate = member.ActualMergerDate;
        memberData.CdcCompartmentIDforInv = member.CdcCompartmentIDforInv != null ? member.CdcCompartmentIDforInv.Trim() : member.CdcCompartmentIDforInv;
        //memberData.CdcCompartmentIDforPayInv = member.CdcCompartmentIDforPayInv;
        //memberData.CdcCompartmentIDforRecInv = member.CdcCompartmentIDforRecInv;
        //memberData.CdcSectionIDofMember = member.CdcSectionIDofMember;

        var ichConfiguration = IchRepository.Single(mem => mem.MemberId == memberId);
        //Check if value for field UatpInvoiceHandledbyAtcan is modified.If modified then update should be sent to ICH)
        if ((member.UatpInvoiceHandledbyAtcan != memberData.UatpInvoiceHandledbyAtcan) || (member.UatpInvoiceHandledbyAtcanFutureValue != memberData.UatpInvoiceHandledbyAtcanFutureValue) ||
            (member.UatpInvoiceHandledbyAtcanFuturePeriod != memberData.UatpInvoiceHandledbyAtcanFuturePeriod))
        {
          // Call InsertMessageInOracleQueue() method which will insert message in Oracle queue
          //Send update to ICH if the member is Live ICH member
          if ((ichConfiguration != null) && (ichConfiguration.IchMemberShipStatusId != 4))
          {
              /* SCP #416213 - Member Profiles sent to ICH while member is Terminated on ICH
               * Desc: Passing ICH Membership status for this member. */ 
              InsertMessageInOracleQueue("MemberProfileUpdate", memberId, ichMembershipStatusId: ichConfiguration.IchMemberShipStatusId);
            SendMailToIchForMemberProfileUpdate(memberId);
          }
        }

      
        member = MemberRepository.Update(memberData);

        // Process the objects and generate the audit entries.
        AddUpdateAuditEntries(memberFutureUpdatesList);

        UnitOfWork.CommitDefault();

        // Send email for immediate updates.
        SendImmediateUpdatesEmail(memberFutureUpdatesList);

        // Call InsertMessageInOracleQueue() method which will insert message in Oracle queue in case of Member Merger Information added/Delete/Updated
        // Send Mail also to Ich-Ops for intimation
        InsertMessageInOracleQueue("MemberProfileUpdate", memberId);
        SendMailToIchForMemberProfileUpdate(memberId);
        
      }

      return member;
    }

    private void DeleteFutureUpdate(int memberId, Member member, string fieldName)
    {
      var futureUpdates = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.EBilling, memberId, fieldName, null);
      if ((futureUpdates != null) && (futureUpdates.Count > 0))
      {
        foreach (var update in futureUpdates)
        {
          if (Convert.ToDateTime(update.ChangeEffectivePeriod) > Convert.ToDateTime(member.DigitalSignApplicationFuturePeriod))
          {
            FutureUpdatesRepository.Delete(update);
          }
        }
      }
    }

    /// <summary>
    /// Sends an email for all the immediate updates.
    /// </summary>
    /// <param name="auditEntries"></param>
    private static void SendImmediateUpdatesEmail(List<FutureUpdates> auditEntries)
    {
      if (auditEntries.Count > 0)
      {
        var emailHandler = Ioc.Resolve<IMemberProfileImmediateUpdatesEmailHandler>();

        emailHandler.UpdatesList = auditEntries;
        emailHandler.SendMailsForImmediateMemberProfileUpdates();
      }
    }


    /// <summary>
    /// This method retrieves contacts assigned to contact type passed for specified member id
    /// </summary>
    /// <param name="memberId">member id</param>
    /// <param name="processingContactType">Processing contact type</param>
    /// <returns></returns>
    public List<Contact> GetContactsForContactType(int memberId, ProcessingContactType processingContactType)
    {
        //SCPID 85039: To improve the performace , we moved the entire logic into storedProcedure
        return ContactsRepository.GetContactMemberInformation(memberId, (int)processingContactType);

    }

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="memberId">member id</param>
      /// <param name="processingContactType">Processing contact type</param>
      /// <param name="miscLocationCode"></param>
      /// <returns></returns>
    public List<Contact> GetContactsForMiscOutputAlerts(int memberId, ProcessingContactType processingContactType, string miscLocationCode)
    {
      //SCPID 85039: To improve the performace , we moved the entire logic into storedProcedure
        return ContactsRepository.GetContactsForMiscOutputAlerts(memberId, (int)processingContactType, miscLocationCode);
       
    }

    /// <summary>
    /// This method retrieves contacts assigned to contact types passed.
    /// </summary>
    /// <param name="processingContactTypeList">Processing contact type</param>
    /// <returns></returns>
    public List<Contact> GetContactsForContactTypes(List<int> processingContactTypeList)
    {
      ContactTypeRepository = Ioc.Resolve<IRepository<ContactType>>();
      var contactList = new List<Contact>();

      // Get contacttypeID corresponding to processing contact type
      var contactTypeIdList = ContactTypeRepository.Get(ct => processingContactTypeList.Contains(ct.Id)).Select(ct => ct.Id).ToList();

      // Get all contactIDs corresponding to contactTypeID denoted by the processing contact type passed
      if (contactTypeIdList.Count <= 0)
      {
        return contactList;
      }

      var contactIdList = ContactTypeMatrixRepository.Get(ctlist => contactTypeIdList.Contains(ctlist.ContactTypeId)).Select(type => type.ContactId).Distinct().ToList();

      if (contactIdList.Count > 0)
      {
        contactList = ContactRepository.Get(cnct => contactIdList.Contains(cnct.Id)).ToList();
      }

      return contactList;
    }

    /// <summary>
    /// Gets list of contacts.
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <param name="firstName">firstName</param>
    /// <param name="lastName">lastName</param>
    /// <param name="emailAddress">emailAddress</param>
    /// <param name="staffId">staffId</param>
    /// <param name="userCategory"></param>
    /// <returns>Contact list.</returns>
    public List<ContactData> GetMemberContacts(int memberId, string firstName, string lastName, string emailAddress, string staffId, int userCategory)
    {
      return ContactsRepository.GetContactUserInformation(memberId, firstName, lastName, emailAddress, staffId, userCategory);
    }

    /// <summary>
    /// Returns a list of subdivision codes for the specified country code and the given sub division token.
    /// </summary>
    public IList<SubDivision> GetSubDivisionList(string countryCode, string subdivision)
    {

      if (subdivision == string.Empty)
      {
        var subDivisionList =
            SubdivisionRepository.Get(
                subDivision =>
                subDivision.IsActive && subDivision.Country.Id == countryCode);
        return subDivisionList.ToList();
      }
      else
      {
        var subDivisionList =
            SubdivisionRepository.Get(
                subDivision =>
                subDivision.IsActive && subDivision.Country.Id == countryCode &&
                subDivision.Name.ToUpper().Contains(subdivision.ToUpper()));
        return subDivisionList.ToList();
      }

    }

    /// <summary>
    /// Returns a list of subdivision codes for the specified country code and the given sub division token.
    /// </summary>
    public IList<SubDivision> GetSubDivisionListByCountryName(string countryName, string subdivision)
    {

      if (subdivision == string.Empty)
      {
        var subDivisionList =
            SubdivisionRepository.Get(
                subDivision =>
                subDivision.IsActive && subDivision.Country.Name == countryName);
        return subDivisionList.ToList();
      }
      else
      {
        var subDivisionList =
            SubdivisionRepository.Get(
                subDivision =>
                subDivision.IsActive && subDivision.Country.Name == countryName &&
                subDivision.Name.ToUpper().Contains(subdivision.ToUpper()));
        return subDivisionList.ToList();
      }

    }

    /// <summary>
    /// Returns a list of countries that ATOS supports digital signatures for.
    /// </summary>
    public List<Country> GetDsSupportedByAtosCountryList()
    {
      var countryList = CountryRepository.Get(country => country.IsActive && country.DsSupportedByAtos);
      return countryList.ToList();
    }

    public bool AddRemoveDsRequiredCountry(int memberId, int billingType, string addedCountryList, string deletedCountryList, string futurePeriod, ref List<FutureUpdates> futureUpdatesList)
    {
      bool updateResult = false;

      //If future period is specified , add data to future update table else add data to EBilling_configuration table

      //Do some thing only if valid member id is passed 
      if (memberId != 0)
      {
        var toBeDeletedMembers = new string[] { };
        var toBeAddedMembers = new string[] { };
        if (!string.IsNullOrEmpty(deletedCountryList))
        {
          //trim last comma (if present) and split in to id strings
          toBeDeletedMembers = deletedCountryList.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }
        if (!string.IsNullOrEmpty(addedCountryList))
        {
          //trim last comma (if present) and split in to id strings
          toBeAddedMembers = addedCountryList.Trim().TrimEnd(new char[] { ',' }).Split(',');
        }

        //Update all existing fu records' periods regardless of whether they are in deleted or added list or not
        if (!(toBeAddedMembers.Count() == 0 && toBeDeletedMembers.Count() == 0))
        {
          //get an instance of element group repository
          var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));
          var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)ElementGroupType.EBillingDS);
          var tableName = elementGroup.FirstOrDefault().TableName;

          List<FutureUpdates> futureUpdateData = null;
          var futureUpdateRepository = Ioc.Resolve<IFutureUpdatesRepository>(typeof(IFutureUpdatesRepository));
          futureUpdateData =
            futureUpdateRepository.Get(
              futureUpdate =>
              futureUpdate.MemberId == memberId && futureUpdate.ElementName == "COUNTRY_CODE" && futureUpdate.TableName == tableName && futureUpdate.RelationId == billingType &&
              futureUpdate.IsChangeApplied == false).ToList();

          foreach (var fu in futureUpdateData)
          {
            //if future period is change and
            //if the existing future update record in not present in Add and Remove list and 
            //then update period of future update to list used for sending mails
            if (fu.ChangeEffectivePeriod != Convert.ToDateTime(futurePeriod) && !toBeDeletedMembers.Contains(fu.OldVAlue) && !toBeAddedMembers.Contains(fu.NewVAlue))
            {
              fu.ChangeEffectivePeriod = Convert.ToDateTime(futurePeriod);
              futureUpdateRepository.Update(fu);
              //Add in list to send mails
              futureUpdatesList.Add(fu);
            }
          }
          futureUpdateRepository = null;
        }

        string countryName;
        if (toBeDeletedMembers.Count() > 0)
        {
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            //loop on the list of countries
            foreach (string country in toBeDeletedMembers)
            {
              string countryId = country;

              //check if for this country DS is already required by this member
              var recordForThisRelationship = DsRequiredCountryRepository.Single(model => model.MemberId == memberId && model.BillingTypeId == billingType && model.CountryId == countryId);
              //Check whether pending future updates record is present for this DS REquired relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.EBillingDS, memberId, "COUNTRY_CODE", countryId.ToString(), billingType);

              //If future update record for this DS REquired relationship is not present
              //and this DS REquired relationship is already effective then only add or update a future update record
              if ((futureUpdatesRecord == null || futureUpdatesRecord.Count <= 0) && (recordForThisRelationship != null))
              {
                //get country name
                var countryData = CountryRepository.Single(countryObject => countryObject.Id == countryId);

                if (countryData == null)
                {
                  return false;
                }
                countryName = countryData.Name;

                //add DS REquired relationship deletion record in future update
                updateResult = UpdateFutureUpdates(ElementGroupType.EBillingDS,
                                                   memberId,
                                                   "COUNTRY_CODE",
                                                   countryId,
                                                   null,
                                                   futurePeriod,
                                                   null,
                                                   (int)ActionType.Delete,
                                                   false,
                                                   billingType,
                                                   countryName,
                                                   String.Empty,
                                                   ref futureUpdatesList);
              }
              else if (futureUpdatesRecord != null && futureUpdatesRecord.Count > 0)
              {
                FutureUpdatesRepository.Delete(futureUpdatesRecord[0]);
                updateResult = true;
              }
            }
          }
          else
          {
            //loop on the list of countries
            foreach (string arrStr in toBeDeletedMembers)
            {
              var countryId = arrStr;
              var dsRequiredCountryRecord = DsRequiredCountryRepository.Single(model => model.MemberId == memberId && model.BillingTypeId == billingType && model.CountryId == countryId);

              if (dsRequiredCountryRecord != null)
              {
                DsRequiredCountryRepository.Delete(dsRequiredCountryRecord);
                updateResult = true;
              }
            }
          }
        }
        else
        {
          updateResult = true;
        }

        //add country only if a list is passed
        if (toBeAddedMembers.Count() > 0)
        {
          //if a future period is specified then add in future update else we'll add in DS Required table
          if (!string.IsNullOrEmpty(futurePeriod))
          {
            //loop on the list of countries
            foreach (string country in toBeAddedMembers)
            {
              var countryId = country;

              //check if for this country DS is already required by this member
              var recordForThisRelationship = DsRequiredCountryRepository.Single(model => model.MemberId == memberId && model.BillingTypeId == billingType && model.CountryId == countryId);
              //Check whether pending future updates record is present for this DS REquired relationship
              var futureUpdatesRecord = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.EBillingDS, memberId, "COUNTRY_CODE", countryId.ToString(), billingType);

              //If future update record for this DS REquired relationship is present
              //or this DS REquired relation ship is not already effective then only add or update a future update record
              if ((futureUpdatesRecord != null && futureUpdatesRecord.Count > 0) || recordForThisRelationship == null)
              {
                //get country name
                var countryData = CountryRepository.Single(countryObject => countryObject.Id == countryId);
                if (countryData == null)
                {
                  return false;
                }
                countryName = countryData.Name;

                updateResult = UpdateFutureUpdates(ElementGroupType.EBillingDS,
                                                   memberId,
                                                   "COUNTRY_CODE",
                                                   null,
                                                   countryId.ToString(),
                                                   futurePeriod,
                                                   null,
                                                   (int)ActionType.Create,
                                                   false,
                                                   billingType,
                                                   null,
                                                   countryName,
                                                   ref futureUpdatesList);
              }
            }
          }
          else
          {
            try
            {
              //loop on the list of countries
              for (var i = 0; i < toBeAddedMembers.Length; i++)
              {
                var countryId = toBeAddedMembers[i];

                //get country name
                var countryData = CountryRepository.Single(countryObject => countryObject.Id == countryId);
                if (countryData == null)
                {
                  return false;
                }
                countryName = countryData.Name;

                var dsRequiredCountryRecord = DsRequiredCountryRepository.Single(model => model.MemberId == memberId && model.BillingTypeId == billingType && model.CountryId == countryId);
                if (dsRequiredCountryRecord == null)
                {
                  var dsRequiredCountrymapping = new DSRequiredCountrymapping();
                  dsRequiredCountrymapping.BillingTypeId = billingType;
                  dsRequiredCountrymapping.MemberId = memberId;
                  dsRequiredCountrymapping.CountryId = toBeAddedMembers[i];
                  dsRequiredCountrymapping.LastUpdatedBy = 1;
                  dsRequiredCountrymapping.LastUpdatedOn = DateTime.UtcNow;
                  DsRequiredCountryRepository.Add(dsRequiredCountrymapping);

                  //Add audit trail records for cDS required country records inserted in create mode

                  AddAuditTrailForImmediateUpdates(ElementGroupType.EBillingDS,
                                                   memberId,
                                                   "COUNTRY_CODE",
                                                   null,
                                                   dsRequiredCountrymapping.CountryId,
                                                   (int)ActionType.Create,
                                                   billingType,
                                                   "",
                                                   countryName,
                                                   ref futureUpdatesList);
                }
              }
            }
            catch (Exception)
            {
              return false;
            }
          }
        }
        else
        {
          updateResult = true;
        }
        UnitOfWork.CommitDefault();
      }
      return updateResult;
    }

    /// <summary>
    /// This method retrieves all users of given member 
    /// </summary>
    /// <param name="memberId">member id</param>
    /// <returns></returns>
    public List<I_ISUser> GetUserList(int memberId)
    {
      var users = Ioc.Resolve<IUserManagement>(typeof(IUserManagement)).GetUsersByMemberID(memberId);
      return users.ToList();
    }

    /// <summary>
    /// This method retrieves all users of given category 
    /// </summary>
    /// <param name="categoryId">category id</param>
    /// <returns></returns>
    public List<I_ISUser> GetUsersByCategory(string categoryId)
    {
      var users = Ioc.Resolve<IUserManagement>(typeof(IUserManagement)).GetUsersByCategory(Convert.ToDecimal(categoryId));
      return users.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="billingType"></param>
    /// <param name="includeFutureUpdates"></param>
    /// <returns></returns>
    public IList<Country> GetDsRequiredCountryList(int memberId, int billingType, bool includeFutureUpdates)
    {
      IList<Country> countries = new List<Country>();
      Country futureCountry;

      //Get all current Current DS Required Country records for this member for passed billing type
      var dsRequiredCountryList = DsRequiredCountryRepository.Get(dsreq => dsreq.MemberId == memberId && dsreq.BillingTypeId == billingType).ToList();
      //  var countries = new List<Country>();
      foreach (var dsRequiredCountry in dsRequiredCountryList)
      {
        var countryRecord = CountryRepository.Single(country => country.IsActive && country.Id == dsRequiredCountry.CountryId);
        if (countryRecord != null)
        {
          countries.Add(countryRecord);
        }
      }

      // Only include future updates if they are required.
      if (includeFutureUpdates)
      {
        //If future update record exist in database where passed memberId billing type are there then get all these records from database
        var futureCountryList = FutureUpdatesManager.GetPendingFutureUpdates(ElementGroupType.EBillingDS, memberId, "COUNTRY_CODE", billingType);
        //We need to add future sponsored members to current sponsored members
        if (futureCountryList != null)
        {
          foreach (var country in futureCountryList)
          {
            FutureUpdates countryValue = country;
            var countryValueId = countryValue.NewVAlue;
            var countryRecord = CountryRepository.Single(ctry => ctry.IsActive && ctry.Id == countryValueId);

            if (countryRecord != null)
            {
              futureCountry = new Country { Id = countryRecord.Id, Name = countryRecord.Name };
              countries.Add(futureCountry);
            }
          }
        }
      }

      return countries;
    }

   /// <summary>
    /// To get the MemberCode for MemberId.
    /// </summary>
    public string GetMemberCode(int memberId)
    {
      var membersRepository = Ioc.Resolve<IRepository<Member>>();
      Member memberDetail = null;
      if (memberId != 0)
      {
        memberDetail = membersRepository.First(member => member.Id == memberId);
      }

      if (memberDetail != null)
      {
        return memberDetail.MemberCodeNumeric;
      }

      return null;
    }

     

    /// <summary>
    /// Gets user record for given EmailId.
    /// </summary>
    /// <param name="emailId">Email Id.</param>
    /// <param name="memberId">Member Id.</param>
    /// <returns>User record.</returns>
    public I_ISUser GetUserByEmailId(string emailId, int memberId)
    {
      //SCP333083: XML Validation Failure for A30-XB - SIS Production
      var userRecord = AuthManager.GetUserByUserMailIdTest(emailId, memberId);
      //if ((userRecord.UserID>0 || userRecord.Member.MemberID>0)&& userRecord.Member.MemberID != memberId)
      //{
      //  throw new ISBusinessException(ErrorCodes.NotTheUserOfSelectedMember);
      //}
      if (userRecord.Location.LOCATION_ID > 0)
      {
        var locationDetails = LocationRepository.Single(location => location.Id == userRecord.Location.LOCATION_ID);
        if (locationDetails != null)
        {
          userRecord.Location.ADD_TAX_VAT_REGISTRATION_NUM = locationDetails.AdditionalTaxVatRegistrationNumber;
          userRecord.Location.Address1 = locationDetails.AddressLine1;
          userRecord.Location.Address2 = locationDetails.AddressLine2;
          userRecord.Location.Address3 = locationDetails.AddressLine3;
          userRecord.Location.BANK_ACCOUNT_NAME = locationDetails.BankAccountName;
          userRecord.Location.BANK_CODE = locationDetails.BankCode;
          userRecord.Location.BANK_NAME = locationDetails.BankName;
          userRecord.Location.SUB_DIVISION_CODE = locationDetails.SubDivisionCode;
          userRecord.Location.SUB_DIVISION_NAME = locationDetails.SubDivisionName;
          userRecord.Location.BRANCH_CODE = locationDetails.BranchCode;
          userRecord.Location.CITY_NAME = locationDetails.CityName;
          userRecord.Location.COUNTRY_CODE = locationDetails.CountryId;
          userRecord.Location.IBAN = locationDetails.Iban;
          userRecord.Location.LOCATION_CODE = locationDetails.LocationCode;
          userRecord.Location.POSTAL_CODE = locationDetails.PostalCode;
          userRecord.Location.REGISTRATION_ID = locationDetails.RegistrationId;
          userRecord.Location.SWIFT = locationDetails.Swift;
          //userRecord.Location. = locationDetails.LocationCode;

        }
      }
      return userRecord;
    }

    /// <summary>
    /// Gets City Name an SubDividion Name.
    /// </summary>
    /// <param name="cityId">cityId.</param>
    /// <param name="sunDivisionId">SunDivisionId.</param>
    public String[] GetUserCityNameAndSubDivisionName(int cityId, string sunDivisionId)
    {
      // TODO: CityRepository needs to replaced by CityAirportRepository.
      var result = new string[2];

      return result;
    }

    public String GetRecentUpdateDateTimeForFutureUpdate(int userId)
    {
      var list = FutureUpdatesRepository.Get(f => f.LastUpdatedBy == userId).OrderByDescending(f => f.LastUpdatedOn);

      if (list.Count() >= 1)
      {
        return list.First().LastUpdatedOn.ToString("dd-MMM-yy") + " " + list.First().LastUpdatedOn.ToShortTimeString();
      }

      return string.Empty;
    }

    private bool AddAuditTrailForImmediateUpdates(ElementGroupType elementGroupType,
                                                  int memberId,
                                                  string elementName,
                                                  string oldValue,
                                                  string newValue,
                                                  int actionId,
                                                  int? relationId,
                                                  string oldValueDisplayName,
                                                  string newValueDisplayName,
                                                  ref List<FutureUpdates> futureUpdatesList)
    {
      //If old value of a field is different than new value or if contact is being deleted then get future update class object
      if ((oldValue != newValue) || (actionId == (int)ActionType.Delete))
      {
        try
        {
          var immediateUpdates = GetFutureUpdateObject(0,
                                                       memberId,
                                                       elementGroupType,
                                                       elementName,
                                                       oldValue,
                                                       newValue,
                                                       null,
                                                       FormatFutureDateValue(DateTime.UtcNow),
                                                       actionId,
                                                       true,
                                                       relationId,
                                                       oldValueDisplayName,
                                                       newValueDisplayName);
          futureUpdatesList.Add(immediateUpdates);

          FutureUpdatesRepository.Add(immediateUpdates);
        }
        catch (Exception)
        {
          return false;
        }
      }

      return true;
    }

    private static FutureUpdates SetFutureUpdateObject(ref FutureUpdates futureUpdates,
                                                       ElementGroupType elementGroupType,
                                                       string oldValue,
                                                       string newValue,
                                                       string changeEffectivePeriod,
                                                       string changeEffectiveDate,
                                                       int actionId,
                                                       bool isChangeApplied,
                                                       int? relationId,
                                                       string oldValueDisplayName,
                                                       string newValueDisplayName)
    {
      //get an instance of element group repository
      var elementGroupRepository = Ioc.Resolve<IRepository<ElementGroup>>(typeof(IRepository<ElementGroup>));
      var elementGroup = elementGroupRepository.Get(eg => eg.Id == (int)elementGroupType);
      futureUpdates.DisplayGroup = elementGroup.FirstOrDefault().Group;

      futureUpdates.OldVAlue = oldValue;
      futureUpdates.NewVAlue = newValue;
      futureUpdates.BilateralMemberId = null;

      futureUpdates.RelationId = relationId;
      futureUpdates.ElementGroupTypeId = (int)elementGroupType;

      if (changeEffectivePeriod != null)
      {
        futureUpdates.ChangeEffectivePeriod = Convert.ToDateTime(changeEffectivePeriod);
      }

      if (changeEffectiveDate != null)
      {
        futureUpdates.ChangeEffectiveOn = Convert.ToDateTime(changeEffectiveDate);
      }

      futureUpdates.IsChangeApplied = isChangeApplied;

      if (!string.IsNullOrEmpty(oldValueDisplayName))
      {
        futureUpdates.OldValueDisplayName = oldValueDisplayName;
      }
      else
      {
        futureUpdates.OldValueDisplayName = oldValue;
      }

      if (!string.IsNullOrEmpty(newValueDisplayName))
      {
        futureUpdates.NewValueDisplayName = newValueDisplayName;
      }
      else
      {
        futureUpdates.NewValueDisplayName = newValue;
      }

      futureUpdates.ActionTypeId = actionId;

      //TODO:This hard coding should be removed once logged in user is stored
      futureUpdates.LastUpdatedBy = 1;
      futureUpdates.LastUpdatedOn = DateTime.UtcNow;

      return futureUpdates;
    }

    /// <summary>
    /// To get ICH member list.
    /// </summary>
    /// <returns></returns>
    public IList<Member> GetIchMemberList()
    {
      var memberList = IchRepository.Get(ich => ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Live || ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended);
      var member = memberList.Select(m => m.Member);
      return member.ToList();
    }

    /// <summary>
    /// Get list of ICH members for given zone id.
    /// </summary>
    /// <param name="zoneId">ich zone id</param>
    /// <returns>Get list of ich members</returns>
    public IList<Member> GetIchMemberListForZone(int zoneId)
    {
      var memberList =
        IchRepository.Get(ich => (ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Live || ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended) && ich.IchZoneId == zoneId);
      var member = memberList.Select(m => m.Member);
      return member.ToList();
    }

    /// <summary>
    /// To get dual member list.
    /// </summary>
    /// <returns></returns>
    public IList<Member> GetDualMemberList(int memberId)
    {
      var memberList = DualMemberRepository.GetDualMemberList();
      memberList.Remove(GetMember(memberId));

      return memberList;
    }

    /// <summary>
    /// To send email notification to Ich/ach ops for new member creation.
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    public bool SendMemberCreationMailToIchAchOps(string memberName, string clearingHouse)
    {
      try
      {
        //declare an object of the nVelocity data dictionary
        VelocityContext context;
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
        List<MailAddress> ichMailAdressList = null;
        List<MailAddress> achMailAdressList = null;
        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        //object of the nVelocity data dictionary
        context = new VelocityContext();
        context.Put("MemberName", memberName);
        context.Put("ClearingHouse", clearingHouse);
        context.Put("SISOpsEmailid", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        var emailSettingForMemberCreation = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.MemberCreationToIchAchOps);

        //generate email body text for own profile updates contact type mail
        var emailToOwnProfileUpdateContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberCreationToIchAchOps, context);
        //create a mail object to send mail
        var msgForMemberCreation = new MailMessage { From = new MailAddress(emailSettingForMemberCreation.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };
        //loop through the contacts list and add them to To list of mail to be sent

        if (clearingHouse.Equals("ICH"))
        {

          if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
          {
            var emailAddressList = AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail;
            var formatedEmailList = emailAddressList.Replace(',', ';');
            ichMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

            foreach (var mailaddr in ichMailAdressList)
            {
              msgForMemberCreation.To.Add(mailaddr);
            }
          }
        }
        else if (clearingHouse.Equals("ACH"))
        {
          if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ACHDetails.AchOpsEmail))
          {
            var emailAddressList = AdminSystem.SystemParameters.Instance.ACHDetails.AchOpsEmail;
            var formatedEmailList = emailAddressList.Replace(',', ';');
            achMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

            foreach (var mailaddr in achMailAdressList)
            {
              msgForMemberCreation.To.Add(mailaddr);
            }

          }
        }

        //set subject of mail (replace special field placeholders with values)
        var subject = emailSettingForMemberCreation.SingleOrDefault().Subject.Replace("$MemberName$", memberName);
        subject = subject.Replace("$ClearingHouse$", clearingHouse);
        msgForMemberCreation.Subject = subject;

        //set body text of mail
        msgForMemberCreation.Body = emailToOwnProfileUpdateContactsText;

        // send the mail.
        emailSender.Send(msgForMemberCreation);

        return true;
      }
      catch (Exception exception)
      {
        _logger.Error("Error occurred in Member Creation Notification Email Handler (Send Mails for a Single Member method).", exception);
        return false;
      }
    }

    /// <summary>
    /// Get information for contact and contact type assignment
    /// </summary>
    /// <param name="searchCriteria">search criteria.</param>
    /// <param name="memberId">logged in member id</param>
    /// <param name="recordCount">total record count</param>
    /// <returns>data table containing search result</returns>
    public DataTable GetContactAssignmentData(ContactAssignmentSearchCriteria searchCriteria, int memberId, int userCategoryId, out int recordCount)
    {
      const string singleQuote = "'";
      var dataTable = ContactsRepository.GetContactAssignmentData(searchCriteria, memberId, userCategoryId, out recordCount);
      for (int i = 0; i < dataTable.Rows.Count; i++)
      {
        dataTable.Rows[i]["First_Name"] = string.Format("{0} {1}", dataTable.Rows[i]["FIRST_NAME"], dataTable.Rows[i]["LAST_NAME"]);
      }

      if (dataTable.Columns.Contains("Last_Name"))
      {
        dataTable.Columns.Remove("Last_Name");
      }
      if (dataTable.Columns.Contains("RN"))
      {
        dataTable.Columns.Remove("RN");
      }
      dataTable.Columns["FIRST_NAME"].Caption = "Contact Name";

      foreach (DataColumn dtColumn in dataTable.Columns)
      {
        if ((dtColumn.ColumnName != "FIRST_NAME") && dtColumn.ColumnName != "CONTACT_ID")
        {
          dtColumn.ColumnName = dtColumn.ColumnName.Trim(singleQuote.ToCharArray());
          if (dtColumn.ColumnName != "EMAIL_ADDRESS")
          {
            var contactTypeName = GetContactTypeName(Convert.ToInt32(dtColumn.ColumnName));
            dtColumn.Caption = contactTypeName;
          }
        }
      }

      dataTable.AcceptChanges();

      return dataTable;
    }

    /// <summary>
    /// Check if only contact assigned for a contact type.
    /// </summary>
    /// <param name="contactId">The contact Id.</param>
    /// <returns>true if is only contact assigned , false otherwise.</returns>
    public bool IsOnlyContactAssigned(int contactId, int memberId, int contactTypeId)
    {
      var isOnlyContact = ContactsRepository.IsOnlyContact(contactId, memberId, contactTypeId);

      return isOnlyContact == 1;
    }

    /// <summary>
    /// Remove all contact assignments
    /// </summary>
    /// <param name="contactId">The contact Id.</param>
    public void RemoveAllContactAssignments(int contactId)
    {
      var contactTypeMatrixRecords = ContactTypeMatrixRepository.Get(m => m.ContactId == contactId);
      foreach (var contactContactTypeMatrix in contactTypeMatrixRecords)
      {
        ContactTypeMatrixRepository.Delete(contactContactTypeMatrix);
      }
    }

    /// <summary>
    /// Get the list of Aggregated,Sponsored member of perticular member
    /// </summary>
    /// <param name="memberId">The member Id.</param>
    /// <param name="includeFutureUpdates">Include Future updates or not</param>
    public List<Member> GetAggregatedSponsoredMemberList(int memberId, bool includeFutureUpdates)
    {
      var aggList = GetAggregatorsList(memberId, includeFutureUpdates).Select(agg => agg.Member).ToList();
      var spoList = GetSponsoredList(memberId, includeFutureUpdates).Select(spo => spo.Member).ToList();

      aggList.AddRange(spoList);
      return aggList;
    }

    /// <summary>
    /// This function is used to get ClearingHouse Detail.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    //SCP312527 - IS-Web Performance (Controller: ProcessingDashboard - Log Action: FSSearchResultGridData)
    public ClearingHouse GetClearingHouseDetail(int memberId)
    {
      bool ichMemberShipStatus = false;
      bool achMemberShipStatus = false;

      //Read ich configuration based on member id. 
      var ichConfiguration = GetIchConfig(memberId, false);

      if (ichConfiguration != null)
      {
        switch (ichConfiguration.IchMemberShipStatusId)
        {
          case (int)IchMemberShipStatus.Live:
          case (int)IchMemberShipStatus.Suspended:
            ichMemberShipStatus = true;
            break;

          case (int)IchMemberShipStatus.Terminated:
          case (int)IchMemberShipStatus.NotAMember:
            ichMemberShipStatus = false;
            break;
        }
      }

      if (ichMemberShipStatus)
      {
        return ClearingHouse.Ich;
      }

      //Read ach configuration based on member id.
      var achConfiguration = GetAchConfig(memberId, false);
      if (achConfiguration != null)
      {
        switch (achConfiguration.AchMembershipStatusId)
        {
          case (int)AchMembershipStatus.Live:
          case (int)AchMembershipStatus.Suspended:
            achMemberShipStatus = true;
            break;

          case (int)AchMembershipStatus.Terminated:
          case (int)AchMembershipStatus.NotAMember:
            achMemberShipStatus = false;
            break;
        }
      }

      if (achMemberShipStatus)
        return ClearingHouse.Ach;
      return ClearingHouse.Ich;
    }

    /// <summary>
    /// This method is used to get  formatted(YYYY-MMM-PP) next period of a member 
    /// </summary>
    public string GetFormattedNextPeriod(int memberId)
    {
      ClearingHouse clearingHouse = GetClearingHouseDetail(memberId);
      BillingPeriod billingPeriod = CalendarManager.GetNextBillingPeriod(clearingHouse);
      string formattedPeriod = string.Empty;
      if (billingPeriod.Period != 0)
      {
        formattedPeriod = billingPeriod.Year + "-" + Enum.GetName(typeof(Month), billingPeriod.Month) + "-" + "0" + billingPeriod.Period;
      }

      return formattedPeriod;
    }

    /// <summary>
    /// This method is used to get member location 
    /// </summary>
    public Location GetMemberLocation(int locationId)
    {
      return LocationRepository.Single(l => l.Id == locationId);
    }

    /// <summary>
    /// This method is used to get currency name 
    /// </summary>
    public string GetCurrencyName(int currencyId)
    {
      return CurrencyRepository.Single(c => c.Id == currencyId).Code;
    }

    /// <summary>
    /// This method is used to get currency name 
    /// </summary>
    public string GetCountryName(string countryId)
    {
      return CountryRepository.Single(c => c.Id == countryId).Name;
    }

    /// <summary>
    /// To get member commercial name.
    /// </summary>
    /// <param name="memberId">member Id.</param>
    /// <returns></returns>
    public string GetMemberCommercialName(int memberId)
    {
      var memberRepository = Ioc.Resolve<IRepository<Member>>();
      var record = memberRepository.Single(member => member.Id == memberId);

      return record.CommercialName;
    }

    public string GetMemberCodeAlpha(int memberId)
    {
      var memberRepository = Ioc.Resolve<IRepository<Member>>();
      var record = memberRepository.Single(member => member.Id == memberId);

      return record.MemberCodeAlpha;
    }

    /// <summary>
    /// To get the Meber Id by alpha code.
    /// </summary>
    /// <param name="alphaCode"></param>
    /// <returns></returns>
    public int GetMemberIdByCodeAlpha(string alphaCode)
    {
      var memberRepository = Ioc.Resolve<IRepository<Member>>();
      var record = memberRepository.Single(member => member.MemberCodeAlpha == alphaCode);

      return record.Id;
    }

    public TechnicalConfiguration GetMemberTechnicalConfig(int memberId)
    {
      TechnicalConfiguration technicalDetail = null;
      if (memberId > 0)
      {
        technicalDetail = TechnicalRepository.Single(technical => technical.MemberId == memberId);
      }

      return technicalDetail;
    }

    /// <summary>
    /// To send email notification to billing member on billed member suspension.
    /// </summary>
    /// <param name="memberId">billed member Id.</param>
    /// <param name="memberCodeAlpha"></param>
    /// <param name="invoiceBases">Invoice list.</param>
    ///  <param name="billingMEmberId">billing member Id.</param>
    /// <param name="billedMemberDisplayText"></param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>


    public bool SendMemberSuspensionNotification(int memberId, int billingMEmberId, string memberCodeAlpha, string billedMemberDisplayText,
                                      List<InvoiceBase> invoiceBases, string clearingHouse)
    {
      try
      {
        //declare an object of the nVelocity data dictionary
        VelocityContext context;
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        //object of the nVelocity data dictionary
        List<MailAddress> sisOpsEmailAddressList = null;
        context = new VelocityContext();
        context.Put("Invoices", invoiceBases);
        context.Put("BilledMemberText", billedMemberDisplayText);
        context.Put("memberId", memberId);
        if (clearingHouse.Equals("I") || clearingHouse.Equals("IB"))
          context.Put("clearingHouse", "ICH");
        if (clearingHouse.Equals("A") || clearingHouse.Equals("AB"))
          context.Put("clearingHouse", "ACH");
        var emailSettingForMemberSuspension = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.MemberSuspensionNotification);

        VelocityContext htmlContentContext = new VelocityContext();
        htmlContentContext.Put("Invoices", invoiceBases);
        htmlContentContext.Put("BilledMemberText", billedMemberDisplayText);
        htmlContentContext.Put("memberId", memberId);
        //generate email body text for own profile updates contact type mail
        var emailToOwnProfileUpdateContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberSuspensionNotification, context);
        //create a mail object to send mail
        var msgForMemberSuspension = new MailMessage { From = new MailAddress(emailSettingForMemberSuspension.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };
        //loop through the contacts list and add them to To list of mail to be sent
        var emailContentforAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.MemberSuspendedInvoiceHtmlContents, htmlContentContext);

        // TODO:Populate email Address list for members whom member suspension  notification is to be sent
        var memberManager = Ioc.Resolve<MemberManager>(typeof(IMemberManager));
        if (clearingHouse.Equals("I") || clearingHouse.Equals("IB"))
        {
          var ichMemberContactList = memberManager.GetContactsForContactType(billingMEmberId,
                                                                             ProcessingContactType.ICHPrimaryContact);
          if (ichMemberContactList != null)
          {
            foreach (var contact in ichMemberContactList)
            {
              msgForMemberSuspension.To.Add(new MailAddress(contact.EmailAddress));
            }
          }
        }
        if (clearingHouse.Equals("A") || clearingHouse.Equals("AB"))
        {
          var achMemberContactList = memberManager.GetContactsForContactType(billingMEmberId,
                                                                             ProcessingContactType.ACHPrimaryContact);
          if (achMemberContactList != null)
          {
            foreach (var contact in achMemberContactList)
            {
              msgForMemberSuspension.To.Add(new MailAddress(contact.EmailAddress));
            }
          }
        }

        //Add CC to SisOps contacts
        //if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
        //{
        //  var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
        //  var formatedEmailList = emailAddressList.Replace(',', ';');
        //  sisOpsEmailAddressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

        //  foreach (var mailaddr in sisOpsEmailAddressList)
        //  {
        //    msgForMemberSuspension.CC.Add(mailaddr);
        //  }

        //}
        //set subject of mail (replace special field placeholders with values)
        var billingMember = memberManager.GetMemberDetails(memberId);
        msgForMemberSuspension.Subject = emailSettingForMemberSuspension.SingleOrDefault().Subject.Replace("$MemberCode$", string.Format("{0}-{1}", billingMember.MemberCodeNumeric.PadLeft(3, '0'), billingMember.MemberCodeAlpha));

        //set body text of mail
        msgForMemberSuspension.Body = emailToOwnProfileUpdateContactsText;
        string tempFoldePath = Path.GetTempPath();

        string fileName = string.Format("{0} - List of Invoices Submitted Against Suspended Member.htm", billingMember.MemberCodeNumeric.PadLeft(3, '0'));
        string filePath = Path.Combine(tempFoldePath, fileName);
        var attachmentFileStream = new StreamWriter(filePath, false);
        attachmentFileStream.WriteLine(emailContentforAttachment);
        attachmentFileStream.Close();

        msgForMemberSuspension.Attachments.Add(new System.Net.Mail.Attachment(filePath));

        //send the mail
        if (msgForMemberSuspension.To.Count() > 0)
        {
          emailSender.Send(msgForMemberSuspension);
        }

        return true;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred in Member Suspension Notification Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }
    }

    /// <summary>
    /// To send email notification to IS ops for completion of Ich/Ach specific elements for member.
    /// </summary>
    /// <param name="memberName">Member Name</param>
    /// <param name="clearingHouse">Clearing House</param>
    /// <returns></returns>
    public bool SendCompletionOfIchAchSpecificElementsForMemberNotification(string memberName, string clearingHouse)
    {
      try
      {
        //declare an object of the nVelocity data dictionary
        VelocityContext context;
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
        List<MailAddress> isOpsMailAdressList = null;
        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        //object of the nVelocity data dictionary
        context = new VelocityContext();
        context.Put("Member", memberName);
        context.Put("ClearingHouse", clearingHouse);
        var emailSettingForMemberCreation = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.CompetionofConfigurationofIchAchSpecificElement);

        //generate email body text for own profile updates contact type mail
        var emailToOwnProfileUpdateContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.CompetionofConfigurationofIchAchSpecificElement, context);
        //create a mail object to send mail
        var msgForMemberCreation = new MailMessage { From = new MailAddress(emailSettingForMemberCreation.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };

        if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
        {
          var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
          var formatedEmailList = emailAddressList.Replace(',', ';');
          isOpsMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

          foreach (var mailaddr in isOpsMailAdressList)
          {
            msgForMemberCreation.To.Add(mailaddr);
          }

        }
        // msgForMemberCreation.To.Add(new MailAddress(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail));

        //set subject of mail (replace special field placeholders with values)

        var subject = emailSettingForMemberCreation.SingleOrDefault().Subject.Replace("$Member$", memberName);
        subject = subject.Replace("$ClearingHouse$", clearingHouse);
        msgForMemberCreation.Subject = subject;

        //set body text of mail
        msgForMemberCreation.Body = emailToOwnProfileUpdateContactsText;

        // send the mail.
        emailSender.Send(msgForMemberCreation);

        return true;
      }
      catch (Exception exception)
      {
        _logger.Error("Error occurred in Completion of configuration of Ich/Ach specific elements Notification Email Handler (Send Mails for a Single Member method).", exception);
        return false;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="memberCodeAlpha"></param>
    /// <param name="billedAirline"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingCategory"></param>
    /// <param name="invoiceNumber"></param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    public bool SendSuspendedInvoiceNotification(int billingMemberId, string memberCodeAlpha, string billedAirline,
                                      string billingPeriod, BillingCategoryType billingCategory, string invoiceNumber, string clearingHouse)
    {
      try
      {

        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        List<MailAddress> sisOpsMailAdressList = null;

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        //object of the nVelocity data dictionary
        //declare an object of the nVelocity data dictionary for Html Content
        VelocityContext htmlContentContext = new VelocityContext();
        htmlContentContext.Put("InvoiceNumber", invoiceNumber);
        htmlContentContext.Put("BilledMemberText", billedAirline);
        htmlContentContext.Put("BillingPeriod", billingPeriod);
        htmlContentContext.Put("BillingCategory", Convert.ToString(billingCategory).ToUpper());
        //declare an object of the nVelocity data dictionary for Emial template
        VelocityContext context = new VelocityContext();
        context.Put("n", "\n");
        context.Put("SISOpsEmail", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        if (clearingHouse.Equals("I"))
          context.Put("clearingHouse", "ICH");
        else
        {
          context.Put("clearingHouse", "ACH");
        }
        var emailSettingForMemberSuspension = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.SuspendedInvoiceEmail);
        var emailSettingForMemberSuspensionHtml = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.SuspendedInvoiceHtmlContents);
        //generate email body text for own profile updates contact type mail
        var emailToOwnProfileUpdateContactsText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.SuspendedInvoiceEmail, context);
        //create a mail object to send mail
        var msgForMemberSuspension = new MailMessage { From = new MailAddress(emailSettingForMemberSuspension.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };
        //loop through the contacts list and add them to To list of mail to be sent
        var emailContentforAttachment = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.SuspendedInvoiceHtmlContents, htmlContentContext);

        // TODO:Populate email Address list for members whom member suspension  notification is to be sent
        var memberManager = Ioc.Resolve<MemberManager>(typeof(IMemberManager));
        if (clearingHouse.Equals("I"))
        {
          var ichMemberContactList = memberManager.GetContactsForContactType(billingMemberId,
                                                                             ProcessingContactType.ICHPrimaryContact);
          if (ichMemberContactList != null)
          {
            foreach (var contact in ichMemberContactList)
            {
              msgForMemberSuspension.To.Add(new MailAddress(contact.EmailAddress));
            }
          }
        }
        else
        {

          var achMemberContactList = memberManager.GetContactsForContactType(billingMemberId,
                                                                             ProcessingContactType.ACHPrimaryContact);
          if (achMemberContactList != null)
          {
            foreach (var contact in achMemberContactList)
            {
              msgForMemberSuspension.To.Add(new MailAddress(contact.EmailAddress));
            }
          }
        }


        // Following code is commented because as per pilot issue.4426 and as per discussion with Robin, SIS Ops users should not be cc ed in suspension related emails.
        //Add CC to Sisops contacts
        //if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
        //{
        //  var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
        //  var formatedEmailList = emailAddressList.Replace(',', ';');
        //  sisOpsMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

        //  foreach (var mailaddr in sisOpsMailAdressList)
        //  {
        //    msgForMemberSuspension.CC.Add(mailaddr);
        //  }

        //}

        //set subject of mail (replace special field placeholders with values)
        var billingMember = memberManager.GetMemberDetails(billingMemberId);
        msgForMemberSuspension.Subject = emailSettingForMemberSuspension.SingleOrDefault().Subject.Replace("$MemberCode$", string.Format("{0}-{1}", billingMember.MemberCodeNumeric.PadLeft(3, '0'), billingMember.MemberCodeAlpha));

        //set body text of mail
        msgForMemberSuspension.Body = emailToOwnProfileUpdateContactsText;
        string tempFoldePath = Path.GetTempPath();

        string fileName = string.Format("{0} - List of Invoices Submitted Against Suspended Member.htm", billingMember.MemberCodeNumeric.PadLeft(3, '0'));
        string filePath = Path.Combine(tempFoldePath, fileName);

        using (var attachmentFileStream = new StreamWriter(filePath, false))
        {
          attachmentFileStream.WriteLine(emailContentforAttachment);
        }

        msgForMemberSuspension.Attachments.Add(new System.Net.Mail.Attachment(filePath));
        //send the mail);
        if (msgForMemberSuspension.To.Count() > 0)
        {
          emailSender.Send(msgForMemberSuspension);
        }

        return true;
      }
      catch (Exception exception)
      {
        Logger.Error("Error occurred in Suspended Invoice Notification Email Handler (Send Mails for a multiple Members method).", exception);
        return false;
      }
    }

    /// <summary>
    /// To add alert for member suspension.
    /// </summary>
    /// <param name="memberId">billnig memberId.</param>
    /// <param name="invoiceNumber">suspended invoiceNumber</param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    public bool AddSuspendedInvoiceAlert(int memberId, string invoiceNumber, string clearingHouse)
    {
      var displayClearingHOuse = "";
      displayClearingHOuse = clearingHouse.Equals("I") ? "ICH" : "ACH";
      var broadcastMessagesManager = Ioc.Resolve<BroadcastMessagesManager>(typeof(IBroadcastMessagesManager));
      var messageRecipients = new ISMessageRecipients
      {
        MemberId = memberId,
        ContactTypeId = string.Format("{0},{1}", (int)ProcessingContactType.ICHPrimaryContact,
                                      (int)ProcessingContactType.ACHPrimaryContact),
        IsMessagesAlerts = new ISMessagesAlerts
        {
          Message = "Please note that the Invoice No." + " '" + invoiceNumber + "' " + "has been marked as suspended as the invoice has been submitted against a suspended member. Kindly note that this invoice will be kept in IS only for records but will not be processed by the " + displayClearingHOuse + ". Upon re-instatement notification of the suspended member, invoices which have not already been settled on a bi-lateral basis may be resubmitted for settlement. Members can resubmit invoices through the Manage Suspended Invoices screen in IS-Web.",
          StartDateTime = DateTime.UtcNow,
          LastUpdatedOn = DateTime.UtcNow,
          IsActive = true,
          TypeId = (int)MessageType.Alert,
          RAGIndicator = (int)RAGIndicator.Green
        }
      };

      broadcastMessagesManager.AddAlerts(messageRecipients);
      return true;
    }

    /// <summary>
    /// To get member DisplayCommercialName
    /// </summary>
    /// <param name="memberId">memberId</param>
    /// <returns></returns>
    public string GtMembeDiaplayCommercialName(int memberId)
    {

      var memberRepository = Ioc.Resolve<IRepository<Member>>();
      var record = memberRepository.Single(member => member.Id == memberId);
      return record.DisplayCommercialName;
    }

    /// <summary>
    /// Function used to fetch config values for member. This stored procedure is added for performance improvement to fetch only one column value 
    /// instead of fetching one config object for one column value
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="configParameter"></param>
    /// <returns></returns>
    public string GetMemberConfigurationValue(int memberId, MemberConfigParameter configParameter)
    {
      return MemberRepository.GetMemberConfigurationValue(memberId, configParameter);
    }
    /// <summary>
    /// Get list of contacts of contact type Other Members Invoice Reference Data Updates for this member.
    /// </summary>
    /// <param name="processingContactType"></param>
    /// <returns></returns>
    public List<Contact> GetContactsForContactType(ProcessingContactType processingContactType)
    {
      // Remove hard coding from here after enum contact type is contains enum values
      // This method should return all contacts assigned to specified contact type
      ContactTypeRepository = Ioc.Resolve<IRepository<ContactType>>();
      List<ContactContactTypeMatrix> contactMatrixList;
      List<Contact> contactList = new List<Contact>();
      var contact = new Contact();

      // Get contacttypeID corresponding to processing contact type
      var contactType = ContactTypeRepository.Get(ct => ct.Id == (int)processingContactType).ToList();

      // Get all contactIDs corresponding to contactTypeID denoted by the processing contact type passed
      if ((contactType.Count() == 1))
      {
        var contactTypeId = contactType[0].Id;

        // SCP186215: Member Code Mismatch between Member and Location Details
        // Get only those contacts of member whose 'Is Membership Status' is not Pending.
        contactMatrixList =  ContactTypeMatrixRepository.Get( ctlist => ctlist.ContactTypeId == contactTypeId &&
                                                                        ctlist.Contact.Member.IsMembershipStatus != MemberStatus.Pending).ToList();

        if (contactMatrixList.Count() > 0)
        {
          foreach (var contactmatrixrecord in contactMatrixList)
          {
            if (contactmatrixrecord != null)
            {
              ContactContactTypeMatrix contactmatrixrecord1 = contactmatrixrecord;
              contact = ContactRepository.Single(cnct => cnct.Id == contactmatrixrecord1.ContactId);

              if (contact != null)
              {
                contactList.Add(contact);
              }
            }
          }
          if (contactList.Count == 0)
          {
            contactList = null;
          }
        }
      }
      else
      {
        contactList = null;
      }

      return contactList;
    }

    /// <summary>
    /// To check if member is dual member.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    public bool IsDualMember(int memberId)
    {

      var ichConfigRepository = Ioc.Resolve<IRepository<IchConfiguration>>();
      var achConfigRepository = Ioc.Resolve<IRepository<AchConfiguration>>();

      var ichConfig = ichConfigRepository.Single(ich => ich.MemberId == memberId && (ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Live || ich.IchMemberShipStatusId == (int)IchMemberShipStatus.Suspended));
      var achConfig = achConfigRepository.Single(ach => ach.MemberId == memberId && (ach.AchMembershipStatusId == (int)AchMembershipStatus.Live || ach.AchMembershipStatusId == (int)AchMembershipStatus.Suspended));
      return (ichConfig != null) && (achConfig != null);
    }


    /// <summary>
    /// To send email notification to IS admin about unexpected errror.
    /// </summary>
    /// <param name="erroMessage">"error message"</param>
    /// <param name="serviceName">"service name"</param>
    /// <returns></returns>
    public bool SendUnexpectedErrorNotificationToISAdmin(string serviceName, string erroMessage, int memberId)
    {
      try
      {
        var memberName = "";

        //declare an object of the nVelocity data dictionary
        VelocityContext context;
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
        List<MailAddress> isOpsMailAdressList = null;
        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nVelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));


        var memberData = MemberRepository.Single(mem => mem.Id == memberId);

        _logger.Info("Getting Member data for sending email");

        if (memberData != null)
        {
          memberName = memberData.MemberCodeNumeric + "-" + memberData.MemberCodeAlpha;
        }


        //object of the nVelocity data dictionary
        context = new VelocityContext();
        context.Put("ServiceName", serviceName);
        context.Put("ErrorMessage", erroMessage);
        context.Put("MemberName", memberName);
        var emailSettingForMemberCreation = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.ISAdminNotificationForUnexpectedError);

        //generate email body text for own profile updates contact type mail
        var emailToISAdminText = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ISAdminNotificationForUnexpectedError, context);
        //create a mail object to send mail
        var msgForMemberCreation = new MailMessage { From = new MailAddress(emailSettingForMemberCreation.SingleOrDefault().FromEmailAddress), IsBodyHtml = true };

        if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
        {
          var emailAddressList = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail;
          var formatedEmailList = emailAddressList.Replace(',', ';');
          isOpsMailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);

          foreach (var mailaddr in isOpsMailAdressList)
          {
            msgForMemberCreation.To.Add(mailaddr);
          }

        }

        var subject = emailSettingForMemberCreation.SingleOrDefault().Subject.Replace("$ServiceName$", serviceName).Replace("$MemberName$", memberName);
        
        msgForMemberCreation.Subject = subject;

        //set body text of mail
        msgForMemberCreation.Body = emailToISAdminText;


        // send the mail.
        emailSender.Send(msgForMemberCreation);
        
        return true;
      }
      catch (Exception exception)
      {
        _logger.Error("Error occurred in unexprected error Notification to IS admin Email Handler (Send Mails for a Single Member method).", exception);
        return false;
      }
    }

    private bool SendMailToIchForMemberProfileUpdate(int memberId)
    {
      if (ConfigurationManager.AppSettings["EmailNotification"] == "true")
      {
        try
        {
          // Get an object of the EmailSender component
          var memberPrefix = string.Empty;
          var memberDesignator = string.Empty;
          var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
          var memberData = MemberRepository.Single(mem => mem.Id == memberId);

          Logger.Info("Getting Member data for sending email");
          
          if (memberData != null)
          {
            memberPrefix = memberData.MemberCodeNumeric;
            memberDesignator = memberData.MemberCodeAlpha;
          }
          if (memberId == 0)
          {
            memberPrefix = "ICH";
            memberDesignator = "Settlement";
          }
          Logger.Info(string.Format("EmailSender instance is: [{0}]", emailSender != null ? "NOT NULL" : "NULL"));
          

          // Object of the nVelocity data dictionary
          var context = new VelocityContext();
          context.Put("MemberName", memberPrefix + "-" + memberDesignator);
          context.Put("MemberId", memberId);

           Logger.Info(string.Format("EmailSettingsRepository instance is: [{0}]",
                                    EmailSettingsRepository != null ? "NOT NULL" : "NULL"));


        var emailSettingForISAdminAlert =
              EmailSettingsRepository.Get(
                  esfopu => esfopu.Id == (int)EmailTemplateId.ICHMemberProfileUpdateNotification);

          // Generate email body text for own profile updates contact type mail
          Logger.Info(string.Format("TemplatedTextGenerator instance is: [{0}]",
                                    TemplatedTextGenerator != null ? "NOT NULL" : "NULL"));
          var emailToISAdminText =
              TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ICHMemberProfileUpdateNotification,
                                                           context);



          // Create a mail object to send mail
          var msgForISAdminAlert = new MailMessage
                                       {
                                         From =
                                             new MailAddress(
                                             emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                                         IsBodyHtml = true
                                       };

          var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
          msgForISAdminAlert.Subject = subject;
          msgForISAdminAlert.Subject = subject.Replace("$MemberName$", memberPrefix + "-" + memberDesignator);
          // Loop through the contacts list and add them to To list of mail to be sent
          if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
          {
            var emailAddressList = AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail;
            // If (emailAddressList.Contains(','))
            var formatedEmailList = emailAddressList.Replace(',', ';');
            var mailAdressList = ConvertUtil.ConvertToMailAddresses(formatedEmailList);
            
            foreach (var mailaddr in mailAdressList)
              {
              msgForISAdminAlert.To.Add(mailaddr);
              }
          }


          // Set body text of mail
          msgForISAdminAlert.Body = emailToISAdminText;
          // Send the mail
          emailSender.Send(msgForISAdminAlert);
                      return true;
        }
        catch (Exception exception)
         {
          Logger.Error(
              "Error occurred while sending alert to ICH Admin for BlockinRuleUpdate/Add/Delete Notification",
              exception);
          return false;
        }
      }

      return false;
    }


    public IList<RequiredContactType> GetRequiredTypeByContactId(int contactId)
    {
      var contactTypeList = MemberRepository.GetContactTypeByContactId(contactId);
      return contactTypeList;
    }

    /// <summary>
    /// Get Final Parent's Member Id for given member Id
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <returns></returns>
    public int GetFinalParentDetails(int memberId)
    {
      var finalParentId = MemberRepository.GetFinalParentDetails(memberId);
      return finalParentId;
    }

    /// <summary>
    /// Get Child Member list for given member Id
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <returns></returns>
    public IList<ChildMemberList> GetChildMembers(int memberId)
    {
      var childMemberList = MemberRepository.GetChildMembers(memberId);
      return childMemberList;
    }



    public string GetContactTypeNameById(int contactTypeId)
    {
      var returnType = string.Empty;
      var contactTypeName = ContactTypeRepository.Single(c => c.Id == contactTypeId);
      if (contactTypeName != null)
      {
        returnType = contactTypeName.ContactTypeName;
      }
      return returnType;
    }
    private bool CheckContactTypeMarked(int paramContactTypeId, string[] contactAssignments)
        {
            int countRows = 0;
            foreach (var contactAssign in contactAssignments)
            {
                string[] contactTypeAssignments = contactAssign.Split("!".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (contactTypeAssignments.Length == 2)
                {
                    //Get contact types
                    string[] assignments = contactTypeAssignments[1].Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var assignment in assignments)
                    {
                        int contactTypeIndex = assignment.IndexOf('_');
                        int contactTypeId = int.Parse(assignment.Substring(0, contactTypeIndex));
                        var value = bool.Parse(assignment.Substring(contactTypeIndex + 1));

                        if (value)
                        {
                            if (contactTypeId == paramContactTypeId)
                            {
                                countRows += 1;
                            }
                        }
                        if (countRows >= 1)
                        {
                            return true;
                        }
                    }
                }
            } // end foreach
            return false;

        }

        public bool UploadMemberLogo(int memberId, byte[] memberLogo)
        {
            var memberLogoInDB = MemberLogoRepository.Single(memb => memb.MemberId == memberId);

            if (memberLogoInDB != null)
            {
                memberLogoInDB.Logo = memberLogo;
                if (MemberRepository != null)
                {
                    MemberLogoRepository.Update(memberLogoInDB);
                }
            }
            else
            {
                // Member logo configuration with specified member Id does not exists in database
                memberLogoInDB = new MemberLogo { MemberId = memberId, Logo = memberLogo };
                if (MemberRepository != null)
                {
                    MemberLogoRepository.Add(memberLogoInDB);
                }
            }

            UnitOfWork.CommitDefault();
            return true;
        }

        /// <summary>
        /// Gets logo information for a specific member
        /// </summary>
        /// <param name="memberId">Id of member for which logo needs to be retrieved from database</param>
        /// <returns>byte array containing logo image details</returns>
        public byte[] GetMemberLogo(int memberId)
        {
            var memberData = MemberLogoRepository.Single(memb => memb.MemberId == memberId);
            var memberLogo = new byte[] { };
            if (memberData != null)
            {
                memberLogo = memberData.Logo;
            }

            if (memberLogo != null)
            {
                return memberLogo;
            }

            return new byte[] { };
        }

      /// <summary>
      /// CMP#400: Audit Trail Report for Deleted Invoices
      /// </summary>
      /// <param name="memberId"></param>
      /// <returns></returns>
      public  bool IsUserIdentificationInAuditTrail(int memberId)
      {
          try
          {
              return EBillingRepository.Single(eBilling => eBilling.MemberId == memberId).IsHideUserNameInAuditTrail;
          }
          catch (Exception)
          {
              return false;
          }
         
      }


      
        /// <summary>
        /// Pads the numeric code by '0' to make the total length 4.
        /// </summary>
        private static string GetMemberNumericCode(string numericCode)
        {
            const int totalPaddedLength = 3;
            const char padChar = '0';

            return numericCode.PadLeft(totalPaddedLength, padChar);
        }

      /// <summary>
      /// Checks if Member Code / issuing Airline is numeric and valid member numeric code
      /// </summary>
      /// <param name="memberCode"></param>
      /// <returns></returns>
      public void ValidateIssuingAirline(string memberCode)
      {
          bool isValid = false;
          if (!string.IsNullOrEmpty(memberCode))
          {
              int numericCode;
              if (int.TryParse(memberCode, out numericCode))
              {
                  if (IsValidAirlineCode(memberCode))
                  {
                      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
                        Desc: The Issuing Airline should be a 3 or 4 digit pure numeric code, Applying New validation #MW5.
                        Ref: FRS Section 3.4 Point 24 */
                      if (memberCode.Length == 3 || memberCode.Length == 4)
                      {
                          isValid = true;
                      }
                  }
              }
          }
          if (!isValid)
              throw new ISBusinessException(ErrorCodes.InvalidIssuingAirline);
      }

      /// <summary>
      /// validate auto calculated amount and percentage like: ISC percentage and ISC amount.
      /// </summary>
      /// <param name="iscpercentage">isc percentage</param>
      /// <param name="evaluatedGrossAmount">evaluated gross amount</param>
      /// <param name="iscAmount">isc amount</param>
      public void ValidateIscPerAndAmt(double evaluatedGrossAmount, double iscpercentage, double iscAmount)
      {
          if (!(Math.Round((evaluatedGrossAmount * iscpercentage) / 100, 2).Equals(iscAmount)))
          {
            throw new ISBusinessException(ErrorCodes.InvalidIscAmount);
          }
      }

      /// <summary>
      /// CMP#520- Evaluates if user type has changed from normal to superuser and vice-versa
      /// and accordingly assigns permissions or deletes them
      /// </summary>
      /// <param name="userId">User Id of the User whose status is changed</param>
      /// <param name="isSuperUser">New value- Is Super User</param>
      public bool ChangeUserPermission(int userId, int isSuperUser)
      {
        var userType = isSuperUser;
        try
        {
          return MemberRepository.ChangeUserPermission(userId, userType);
        }
        catch(Exception exception)
        {
          Logger.Error("Exception:", exception);
        }
        return false;
      }

      #region "CMP #666: MISC Legal Archiving Per Location ID"
      public List<ArchivalLocations> GetAssignedArchivalLocations(int memberId, int archivalType)
      {
          return MemberRepository.GetAssignedArchivalLocations(memberId, archivalType);
      }

      public bool InsertArchivalLocations(string locationSelectedIds, int associtionType, int loggedInUser, int memberId, int archivalType)
      {
          return MemberRepository.InsertArchivalLocations(locationSelectedIds, associtionType, loggedInUser, memberId,
                                                            archivalType);
      }

      public int GetArchivalLocsInconsistency(int memberId, int archReqMiscRecInvReq,  int archReqMiscPayInvReq, int recAssociationType, int payAssociationType)
      {
          return MemberRepository.GetArchivalLocsInconsistency(memberId, archReqMiscRecInvReq, archReqMiscPayInvReq, recAssociationType, payAssociationType);
      }

      public int GetAssociationType(List<ArchivalLocations> listUserContactLocations)
      {
          if (listUserContactLocations.Count >= 1)
          {
              if (listUserContactLocations.Count == 1)
              {
                  if (listUserContactLocations[0].LocationId == 0) // Indicate None location assigned
                  {
                      return (int)ArchivalAssociation.None;
                  }
                  return (int)ArchivalAssociation.SpecificLocation;
              }
              return (int)ArchivalAssociation.SpecificLocation;
          }
          return (int)ArchivalAssociation.AllLocation;
      }

      public List<ArchivalLocations> GetSortedLocationCode(List<ArchivalLocations> listLocationCode)
      {

          if (listLocationCode.Count() > 0)
          {
              var integerMemberLocationList = listLocationCode.ToList();
              integerMemberLocationList.Clear();
              var stringMemberLocationList = listLocationCode.ToList();
              stringMemberLocationList.Clear();

              foreach (var mll in listLocationCode.Where(mll => mll.LocationId > 0))
              {
                  if (Regex.IsMatch(mll.LocationCode, @"^[0-9]+$"))
                  {
                      integerMemberLocationList.Add(mll);
                  }
                  else stringMemberLocationList.Add(mll);
              }

              if (integerMemberLocationList.Count != 0)
                  integerMemberLocationList = integerMemberLocationList.OrderBy(l => int.Parse(l.LocationCode)).ToList();

              if (stringMemberLocationList.Count != 0)
                  stringMemberLocationList = stringMemberLocationList.OrderBy(l => l.LocationCode).ToList();

              if (integerMemberLocationList.Count != 0)
                  stringMemberLocationList.AddRange(integerMemberLocationList);
              return stringMemberLocationList.ToList();
          }
          return listLocationCode;
      }

      #endregion
  }
}

