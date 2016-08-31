using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;
using iPayables.UserManagement;

namespace Iata.IS.Business.MemberProfile
{
  /// <summary>
  /// 
  /// </summary>
  public interface IMemberManager
  {
    int UserId { get; set; }
    /// <summary>
    /// Creates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    Member CreateMember(Member member);

    /// <summary>
    /// Creates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    Member CreateISMember(Member member);

    /// <summary>
    /// This method will get locations corresponding to member ID passed
    /// </summary>
    /// <param name="memberId">ID of airline member</param>
    /// <returns></returns>
    List<Location> GetMemberLocationList(int memberId, bool showOnlyActiveLocations = false);

    /// <summary>
    /// Updates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    Member UpdateMember(Member member);

    /// <summary>
    /// Terminates the member.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <returns></returns>
    bool TerminateMember(Member member);

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns></returns>
    Member GetMember(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    Member GetMemberDetails(int memberId);

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberCode">The member code.</param>
    /// <returns></returns>
    int GetMemberId(string memberCode);

    /// <summary>
    /// Get Final Parent's Member Id for given member Id
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <returns></returns>
    int GetFinalParentDetails(int memberId);

    /// <summary>
    /// Get Child Member list for given member Id
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <returns></returns>
    IList<ChildMemberList> GetChildMembers(int memberId);

    /// <summary>
    /// To get the MemberCode for MemberId.
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    string GetMemberCode(int memberId); 

    /// <summary>
    /// Gets the technical config
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    TechnicalConfiguration GetTechnicalConfig(int memberId);

    /// <summary>
    /// Gets the E-billing configuration
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    EBillingConfiguration GetEbillingConfig(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Gets the member location info.
    /// </summary>
    /// <param name="locationId">The location ID whose details should be fetched.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    Location GetMemberLocationDetails(int locationId, bool includeFutureUpdates = false);

    /// <summary>
    ///Gets contact details.
    /// </summary>
    /// <param name="contactId">Contact Id whose details to be fetched.</param>
    /// <returns>Contact details.</returns>
    Contact GetContactDetails(int contactId);

    /// <summary>
    /// Retrieves details of 'Default' location for member ID passed
    /// </summary>
    /// <param name="memberId">ID of member for which default location details need to be retrieved</param>
    /// <param name="locationCode">location code for default location</param>
    /// <returns>Location class object</returns>
    Location GetMemberDefaultLocation(int memberId, string locationCode);

    
    /// <summary>
    /// Gets the status history for Ich Configuration of a member
    /// </summary>
    /// <returns></returns>
    IchConfiguration GetIchMemberStatusList(IchConfiguration ichConfiguration);

    /// <summary>
    /// Gets the status history for Ich Configuration of a member
    /// </summary>
    /// <returns></returns>
    AchConfiguration GetAchMemberStatusList(AchConfiguration achConfiguration);

    /// <summary>
    /// Updates the E billing configuration.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="eBillingConfiguration">The e billing configuration.</param>
    /// <returns></returns>
    EBillingConfiguration UpdateEBillingConfiguration(int memberId, EBillingConfiguration eBillingConfiguration);

    /// <summary>
    /// Adds or Updates the cargo configuration.
    /// </summary>
    /// <param name="cargoConfiguration">The cargo configuration.</param>
    /// <param name="memberId">member Id corresponding to passenger configuration</param>
    /// <returns></returns>
    CargoConfiguration UpdateCargoConfiguration(int memberId, CargoConfiguration cargoConfiguration);

    /// <summary>
    /// Adds or updates the passenger configuration.
    /// </summary>
    /// <param name="memberId">member Id corresponding to passenger configuration</param>
    /// <param name="passengerConfiguration">The passenger configuration.</param>
    /// <returns></returns>
    PassengerConfiguration UpdatePassengerConfiguration(int memberId, PassengerConfiguration passengerConfiguration);

    /// <summary>
    /// Updates the miscellaneous configuration.
    /// </summary>
    /// <param name="memberId">memberId for which miscellaneous data should be fetched</param>
    /// <param name="miscellaneousConfiguration">The miscellaneous configuration.</param>
    /// <returns>MiscellaneousConfiguration class object</returns>
    MiscellaneousConfiguration UpdateMiscellaneousConfiguration(int memberId, MiscellaneousConfiguration miscellaneousConfiguration);

    /// <summary>
    /// Adds or Updates the UATP configuration.
    /// </summary>
    /// <param name="memberId">member Id for which UATP configuration should be fetched</param>
    /// <param name="uatpConfiguration">The UATP configuration class object</param>
    /// <returns>UatpConfiguration class object</returns>
    UatpConfiguration UpdateUatpConfiguration(int memberId, UatpConfiguration uatpConfiguration);

    /// <summary>
    /// Gets the Ich config
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns></returns>
    IchConfiguration GetIchConfig(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Updates the ich configuration.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="ichConfiguration">The ich configuration.</param>
    /// <returns></returns>
    IchConfiguration UpdateIchConfiguration(int memberId, IchConfiguration ichConfiguration);

    /// <summary>
    /// Updates the ach configuration.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="achConfiguration">The ach configuration.</param>
    /// <returns></returns>
    AchConfiguration UpdateAchConfiguration(int memberId, AchConfiguration achConfiguration);

    /// <summary>
    /// Updates the contact.
    /// </summary>
    /// <param name="memberId">Member Id in session for which contact should be added</param>
    /// <param name="contact">Contact class object</param>
    /// <returns>updated contact class object</returns>
    Contact UpdateContact(int memberId, Contact contact);

    /// <summary>
    /// Gets list of contacts.
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <param name="firstName">firstName</param>
    /// <param name="lastName">lastName</param>
    /// <param name="emailAddress">emailAddress</param>
    /// <param name="staffId">staffId</param>
    /// <returns>Contact list.</returns>
    List<ContactData> GetMemberContacts(int memberId, string firstName, string lastName, string emailAddress, string staffId, int userCategory);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    IList<Contact> GetMemberContactList(int memberId);

    /// <summary>
    /// Get all active contacts
    /// </summary>
    IList<Contact> GetAllMemberContactList();

    /// <summary>
    /// Updates the member location.
    /// </summary>
    /// <param name="location">The location.</param>
    /// <param name="memberId">Member ID.</param>
    /// <returns></returns>
    Location UpdateMemberLocation(int memberId, Location location);

    /// <summary>
    /// Updates the technical configuration.
    /// </summary>
    /// <param name="technicalConfiguration">The technical configuration.</param>
    /// <param name="memberId">Member Id.</param>
    /// <returns></returns>
    TechnicalConfiguration UpdateTechnicalConfiguration(int memberId, TechnicalConfiguration technicalConfiguration);

    /// <summary>
    /// Determines whether [is valid airline code] [the specified airline code].
    /// </summary>
    /// <param name="airlineCode">The airline code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid airline code] [the specified airline code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidAirlineCode(string airlineCode);

    /// <summary>
    /// Determines whether [is valid airline alpha code] [the specified airline alpha code].
    /// </summary>
    /// <param name="airlineAlphaCode">The airline alpha code.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid airline alpha code] [the specified airline alpha code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidAirlineAlphaCode(string airlineAlphaCode);

    /// <summary>
    /// Determines whether [is valid member location] [the specified airline code].
    /// </summary>
    /// <param name="memberLocationCode">The member location code.</param>
    /// <param name="memberId">The member id.</param>
    /// <returns>
    /// 	<c>true</c> if [is valid member location] [the specified airline code]; otherwise, <c>false</c>.
    /// </returns>
    bool IsValidMemberLocation(string memberLocationCode, long memberId);

    /// <summary>
    /// Gets the allowed file extensions for given member and billing category.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="billingCategoryType">The billing category type.</param>
    /// <returns></returns>
    string GetAllowedFileExtensions(int memberId, BillingCategoryType billingCategoryType);

    
    string GetFileModifiedName(int attachmentId);
    bool CheckFileDuplicateStatus(string fileName);
    //TODO: Fake To Remove/update END

    /// <summary>
    /// Returns the list of Members present in database
    /// Currently, PAX module is using method GetMemberList for auto complete feature so this method is renamed as GetMemberListfromDb
    /// Once member profile module starts working perfectly, this method will be renamed to GetMemberList
    /// </summary>
    /// <returns>List of member class objects</returns>
    IList<Member> GetMemberListFromDB();

    /// <summary>
    /// To get the eligible members list for DRR report from database.
    /// SCP272586: DailyRevenueRecogRepGenService throws exception
    /// </summary>
    /// <returns></returns>
    IList<Member> GetMemberListFromDbForDrrReport();

      /// <summary>
      /// This method returns a list of members formatted as [alpha code]-[numeric code]-[commercial name]|[member id]
      /// </summary>
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
     * Desc: Added new parameter excludeTypeBMembers. */
    string GetMemberListForUI(string filter, int memberIdToSkip, bool includePending = true, bool includeBasic = true,
                                bool includeRestricted = true, bool includeTerminated = false,
                                bool includeOnlyAch = false, bool includeOnlyIch = false, int ichZone = 0,
                                bool excludeMergedMember = false, int includeMemberType = 0, bool excludeTypeBMembers = false);

    /// <summary>
    /// Gets the Cargo configuration
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    CargoConfiguration GetCargoConfig(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Gets the Ach configuration
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    AchConfiguration GetAchConfig(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Assigns contact types assigned to one contact to another contact
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    bool ReplaceContacts(int contactId, int replacedContactId);

    /// <summary>
    /// Copies contact types of one contact to another contact
    /// </summary>
    /// <returns>True if successful, false otherwise</returns>
    bool CopyContacts(int contactId, int copyToContactId);

    /// <summary>
    /// Gets details of passenger configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which passenger configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns>PassengerConfiguration class object</returns>
    PassengerConfiguration GetPassengerConfiguration(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Gets details of miscellaneous configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which miscellaneous configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <returns>MiscellaneousConfiguration class object</returns>
    MiscellaneousConfiguration GetMiscellaneousConfiguration(int memberId, bool includeFutureUpdates = false);

    /// <summary>
    /// Gets details of UATP configuration saved against a member ID
    /// </summary>
    /// <param name="memberId">member ID for which Uatp configuration should be fetched</param>
    /// <param name="includeFutureUpdates">Include future update data</param>
    /// <param name="uatpMember"></param>
    /// <returns>Uatp Configuration class object</returns>
    UatpConfiguration GetUATPConfiguration(int memberId, bool includeFutureUpdates = false, Member uatpMember = null);

    /// <summary>
    /// Gets ContactType list.
    /// </summary>
    /// <param name="tabName">Tab Name for which ContactTypes need to be fetched.</param>
    /// <returns>ContactTypes List.</returns>
    IList<ContactType> GetContactTypesList(string tabName);

    /// <summary>
    /// Maps newly configured sponsored members against a member and removes sponsored member mapping for passed member list
    /// </summary>
    /// <param name="memberId">ID of member for which new sponsored member should be added or existing sponsored member mapping should be removed</param>
    /// <param name="addedSponsoredMembers">List of memberIDs which should be marked as members sponsored by member denoted by member ID</param>
    /// <param name="deletedSponsoredMembers">List of memberIDs which would not be sponsored by member denoted by member ID</param>
    /// <param name="futurePeriod">Future period value from which sponsorer values will be effective</param>
    /// <param name="futureUpdatesList"></param>
    /// <returns>True if list added and removed successfully,false otherwise</returns>
    bool UpdateSponsoredMemberList(int memberId, string addedSponsoredMembers, string deletedSponsoredMembers, string futurePeriod, ref List<FutureUpdates> futureUpdatesList);

    /// <summary>
    /// Maps newly configured aggregator members against a member and removes aggregator member mapping for passed member list
    /// </summary>
    /// <param name="memberId">ID of member for which new aggregator member should be added or existing aggregator member mapping should be removed</param>
    /// <param name="addedAggregatorMembers">List of memberIDs which should be marked as members aggregator by member denoted by member ID</param>
    /// <param name="deletedAggregatorMembers">List of memberIDs which would not be aggregator by member denoted by member ID</param>
    /// <param name="futurePeriod"></param>
    /// <param name="futureUpdatesList"></param>
    /// <returns>True if list added and removed successfully,false otherwise</returns>
    bool UpdateAggregators(int memberId, string addedAggregatorMembers, string deletedAggregatorMembers, string futurePeriod, ref List<FutureUpdates> futureUpdatesList);

    /// <summary>
    /// Get Aggregator List
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="includeFutureUpdates"></param>
    /// <returns></returns>
    IList<IchConfiguration> GetAggregatorsList(int memberId, bool includeFutureUpdates);

    /// <summary>
    /// Get Sponsored List
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="includeFutureUpdates"></param>
    /// <returns></returns>
    IList<IchConfiguration> GetSponsoredList(int memberId, bool includeFutureUpdates);

    /// <summary>
    /// Updates ContactContactTypeMatrix record.
    /// </summary>
    /// <param name="contactAssignmentList"></param>
    /// <param name="ichContactTypes"></param>
    string UpdateContactContactTypeMatrix(string contactAssignmentList, string ichContactTypes);

    /// <summary>
    /// Uploads logo to a member record
    /// </summary>
    /// <param name="memberId">Id of member for which logo needs to be uploaded</param>
    /// <param name="memberLogo">byte array containing logo information</param>
    /// <returns>returns true if logo uploaded successfully, false otherwise</returns>
    bool UploadMemberLogo(int memberId, byte[] memberLogo);

    /// <summary>
    /// Gets logo information for a specific member
    /// </summary>
    /// <param name="memberId">Id of member for which logo needs to be retrieved from database</param>
    /// <returns>byte array containing logo image details</returns>
    byte[] GetMemberLogo(int memberId);

    /// <summary>
    /// Gets exception member data for a specific member
    /// </summary>
    /// <param name="memberId">Id of member for which exception data should be fetched form database</param>
    /// <param name="exbillingCategoryId">billing category ID</param>
    /// <param name="includeFutureUpdates"></param>
    /// <returns>List of ACHException class objects</returns>
    IList<AchException> GetExceptionMembers(int memberId, int exbillingCategoryId, bool includeFutureUpdates);

    /// <summary>
    /// Maps newly configured exception members against a member and removes exception members mapping for passed member list
    /// </summary>
    /// <param name="memberId">ID of member for which new exception members should be added or existing exception members mapping should be removed</param>
    /// <param name="addedExceptionMembers">List of memberIDs which should be marked as exception members</param>
    /// <param name="deletedExceptionMembers">List of memberIDs which would not be exception members</param>
    /// <param name="billingCategoryId">Billing category id for which exceptions should be marked</param>
    /// <param name="futurePeriod"></param>
    /// <param name="futureUpdatesList"></param>
    /// <returns>True if list added and removed successfully,false otherwise</returns>
    bool UpdateACHExceptionMembers(int memberId, string addedExceptionMembers, string deletedExceptionMembers, int billingCategoryId, string futurePeriod, ref List<FutureUpdates> futureUpdatesList);

    /// <summary>
    /// Retrieve MemberStatus details
    /// </summary>
    /// <returns></returns>
    List<MemberStatusDetails> GetMemberStatus(int memberId, string memberType);

    /// <summary>
    /// Retrieves contact type name corresponding to contact type id passed
    /// </summary>
    /// <param name="contactTypeId">contact type ID for which contact type name needs to be determined</param>
    /// <returns>contact type name</returns>
    string GetContactTypeName(int contactTypeId);

    /// <summary>
    /// Member control field update
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="member"></param>
    /// <returns></returns>
    Member UpdateMemberControl(int memberId, Member member);


    /// <summary>
    /// This method retrieves contacts assigned to contact type passed for specified member id
    /// </summary>
    /// <param name="memberId">member id</param>
    /// <param name="processingContactType">Processing contact type</param>
    /// <returns></returns>
    List<Contact> GetContactsForContactType(int memberId, ProcessingContactType processingContactType);

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="memberId">member id</param>
      /// <param name="processingContactType">Processing contact type</param>
      /// <param name="miscLocationCode"></param>
      /// <returns></returns>
      List<Contact> GetContactsForMiscOutputAlerts(int memberId, ProcessingContactType processingContactType,  string miscLocationCode);

   
    /// <summary>
    /// This method retrieves contacts assigned to contact types passed.
    /// </summary>
    /// <param name="processingContactTypeList">Processing contact type</param>
    /// <returns></returns>
    List<Contact> GetContactsForContactTypes(List<int> processingContactTypeList);

    /// <summary>
    /// Deletes member contact
    /// </summary>
    bool DeleteContact(int contactId, int memberId);

    /// <summary>
    /// Get DS supported country List.
    /// </summary>
    /// <returns></returns>
    List<Country> GetDsSupportedByAtosCountryList();

    /// <summary>
    /// Add DS Supported Country.
    /// </summary>
    /// <param name="memberId">The member Id.</param>
    /// <param name="billingType">The billing Type.</param>
    /// <param name="addedCountryList"></param>
    /// <param name="deletedCountryList"></param>
    /// <param name="futurePeriod"></param>
    /// <param name="futureUpdatesList"></param>
    /// <returns></returns>
    bool AddRemoveDsRequiredCountry(int memberId, int billingType, string addedCountryList, string deletedCountryList, string futurePeriod, ref List<FutureUpdates> futureUpdatesList);

    IList<Country> GetDsRequiredCountryList(int memberId, int billingType, bool includeFutureUpdates);

    /// <summary>
    ///  Returns the list of Ich/Ach Members present in database
    /// </summary>
    /// <param name="category">User Category</param>
    /// <param name="menuType">which menu user selected ich or ach</param>
    /// <returns></returns>
    IList<Member> GetMemberListForIchOrAch(int category, string menuType);

    /// <summary>
    /// Gets user record for given EmailId.
    /// </summary>
    /// <param name="emailId">Email Id.</param>
    /// <param name="memberId">Member Id.</param>
    /// <returns>User record.</returns>
    I_ISUser GetUserByEmailId(string emailId,int memberId);

    /// <summary>
    /// Gets City Name an SubDividion Name.
    /// </summary>
    /// <param name="cityId">cityId.</param>
    /// <param name="sunDivisionId">SunDivisionId.</param>
    /// <returns></returns>
    String[] GetUserCityNameAndSubDivisionName(int cityId, string sunDivisionId);

    /// <summary>
    /// Gets last updated date and time from mem_future_update table.
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <returns></returns>
    String GetRecentUpdateDateTimeForFutureUpdate(int userId);

    IList<SubDivision> GetSubDivisionList(string countryCode, string subdivision);

    /// <summary>
    /// To get Ich member list.
    /// </summary>
    /// <returns></returns>
    IList<Member> GetIchMemberList();

    /// <summary>
    /// To get dual member list.
    /// </summary>
    /// <returns></returns>
    IList<Member> GetDualMemberList(int memberId);

    /// <summary>
    /// Get list of Ich members for given zone id.
    /// </summary>
    /// <param name="zoneId">ich zone id</param>
    /// <returns>Get list of ich members</returns>
    IList<Member> GetIchMemberListForZone(int zoneId);

    /// <summary>
    /// To send email notification to Ich/ach ops for new member creation.
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    bool SendMemberCreationMailToIchAchOps(string memberName, string clearingHouse);

    /// <summary>
    /// This method retrieves all users of given member 
    /// </summary>
    /// <param name="memberId">member id</param>
    /// <returns></returns>
    List<I_ISUser> GetUserList(int memberId);

    /// <summary>
    /// Get information for contact and contact type assignment
    /// </summary>
    /// <param name="searchCriteria">search criteria.</param>
    /// <param name="memberId">logged in member id</param>
    /// <param name="userCategoryId">User category of logged in user</param>
    /// <param name="recordCount">total record count</param>
    /// <returns>data table containing search result</returns>
    System.Data.DataTable GetContactAssignmentData(ContactAssignmentSearchCriteria searchCriteria, int memberId, int userCategoryId, out int recordCount);

    /// <summary>
    /// Check if only contact assigned for a contact type.
    /// </summary>
    /// <param name="contactId">The contact Id.</param>
    /// <returns>true if is only contact assigned , false otherwise.</returns>
    bool IsOnlyContactAssigned(int contactId, int memberId, int contactTypeId);

    /// <summary>
    /// Remove all contact assignments
    /// </summary>
    /// <param name="contactId">The contact Id.</param>
    void RemoveAllContactAssignments(int contactId);

    /// <summary>
    /// Get the list of Aggregated,Sponsored member of particular member
    /// </summary>
    /// <param name="memberId">The member Id.</param>
    /// <param name="includeFutureUpdates">Include Future updates or not</param>
    List<Member> GetAggregatedSponsoredMemberList(int memberId, bool includeFutureUpdates);

    /// <summary>
    /// Get the member category
    /// </summary>
    /// <param name="memberId">The member Id.</param>
    ClearingHouse GetClearingHouseDetail(int memberId);

    /// <summary>
    /// This method is used to get  formatted(YYYY-MMM-PP) next period of a member 
    /// </summary>
    string GetFormattedNextPeriod(int memberId);

    /// <summary>
    /// This method is used to get member location 
    /// </summary>
    Location GetMemberLocation(int locationId);

    /// <summary>
    /// This method is used to get currency name 
    /// </summary>
    string GetCurrencyName(int currencyId);

    /// <summary>
    /// This method is used to get currency name 
    /// </summary>
    string GetCountryName(string countryId);

    /// <summary>
    /// To get member commeracial Name.
    /// </summary>
    /// <param name="memberId">member Id.</param>
    /// <returns></returns>
    string GetMemberCommercialName(int memberId);

    /// <summary>
    /// To get member CodeAlpha.
    /// </summary>
    /// <param name="memberId">member Id.</param>
    /// <returns></returns>
    string GetMemberCodeAlpha(int memberId);

    /// <summary>
    /// To get the Meber Id by alpha code.
    /// </summary>
    /// <param name="alphaCode"></param>
    /// <returns></returns>
    int GetMemberIdByCodeAlpha(string alphaCode);

    /// <summary>
    /// Gets only Ich configurations of member. (Without future update information.)
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    IchConfiguration GetIchDetails(int memberId);

    IList<Member> GetMembersBasedOnUserCategory(int category, bool isBothIchAch = false);

    Contact GetContactDetailsByEmailAndMember(string emailId, int memberId);

    /// <summary>
    /// Get Contact Details by Email
    /// </summary>
    /// <param name="emailId"> string type </param>
    /// <returns> Contact object </returns>
    Contact GetContactDetailsByEmail(string emailId);

    bool IsUserEmailIdInMemberContact(string emailId, int memberId);
    TechnicalConfiguration GetMemberTechnicalConfig(int memberId);
    /// <summary>
    /// To send email notification to billing member on billed member suspension.
    /// </summary>
    /// <param name="memberId">billed member Id.</param>
    /// <param name="memberCodeAlpha"></param>
    /// <param name="invoiceBases">Invoice list.</param>
    /// <param name="billingMEmberId">billing member Id.</param>
    /// <param name="billedMemberDisplayText"></param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    bool SendMemberSuspensionNotification(int memberId, int billingMEmberId, string memberCodeAlpha,string billedMemberDisplayText,
                                          List<InvoiceBase> invoiceBases,string clearingHouse);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="billingMemberId"></param>
    /// <param name="memberCodeAlpha"></param>
    /// <param name="billedAirline"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingCategory"></param>
    /// <param name="invoiceNumber"></param>
    /// <param name="cleringHouse"></param>
    /// <returns></returns>
    bool SendSuspendedInvoiceNotification(int billingMemberId, string memberCodeAlpha, string billedAirline,
                                          string billingPeriod, BillingCategoryType billingCategory, string invoiceNumber,string cleringHouse);


    IList<SubDivision> GetSubDivisionListByCountryName(string countryName, string subdivision);

    /// <summary>
    /// To get member DisplayCommercialName
    /// </summary>
    /// <param name="memberId">memberId</param>
    /// <returns></returns>
    string GtMembeDiaplayCommercialName(int memberId);

    /// <summary>
    /// To add alert for member suspension.
    /// </summary>
    /// <param name="memberId">billnig memberId.</param>
    /// <param name="invoiceNumber">suspended invoiceNumber</param>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    bool AddSuspendedInvoiceAlert(int memberId, string invoiceNumber,string clearingHouse);

    /// <summary>
    /// Function used to fetch config values for member. This stored procedure is added for performance improvement to fetch only one column value 
    /// instead of fetching one config object for one column value
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="configParameter"></param>
    /// <returns></returns>
    string GetMemberConfigurationValue(int memberId, MemberConfigParameter configParameter);

    /// <summary>
    /// Get list of contacts of contact type Other Members Invoice Reference Data Updates for this member.
    /// </summary>
    /// <param name="processingContactType"></param>
    /// <returns></returns>
    List<Contact> GetContactsForContactType(ProcessingContactType processingContactType);

    /// <summary>
    /// Following method is used to retrieve Member location details for Location dropdown
    /// </summary>
    /// <param name="memberId">Member Id whose Location details are to be retrieved</param>
    /// <returns>Member Location details</returns>
    IQueryable<Location> GetMemberLocationDetailsForDropdown(int memberId, bool showOnlyActiveLocations = false);

    /// <summary>
    /// Following method checks whether member's default location exists
    /// </summary>
    /// <param name="memberId">member Id whose Location details are to be checked</param>
    /// <param name="locationCode">Location code</param>
    /// <returns>true if default location exists else false</returns>
    bool MemberDefaultLocationExists(int memberId, string locationCode);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="erroMessage"></param>
    /// <returns></returns>
    bool SendUnexpectedErrorNotificationToISAdmin(string serviceName, string erroMessage, int memberId);
      
      /// <summary>
      /// Get Contact Type by contact Id
      /// </summary>
      /// <param name="contactId"> contact ID </param>
      /// <returns> object of RequiredContactType </returns>
      IList<RequiredContactType> GetRequiredTypeByContactId(int contactId);

      /// <summary>
      /// Get Contact Type Name by ID
      /// </summary>
      /// <param name="contactTypeId"></param>
      /// <returns></returns>
      string GetContactTypeNameById(int contactTypeId);

      /// <summary>
      ///  Update Contact Email ID incase IS User Email Id updated
      /// </summary>
      /// <param name="memberID"></param>
      /// <returns></returns>
      void UpdateContactEmailId(string newEmailId, string oldEmailId, int memberId);

      /// <summary>
      /// CMP#400: Audit Trail Report for Deleted Invoices
      /// </summary>
      /// <param name="memberId"></param>
      /// <returns></returns>
      bool IsUserIdentificationInAuditTrail(int memberId);

      /// <summary>
      /// Checks if Member Code / issuing Airline is numeric and valid member numeric code
      /// </summary>
      /// <param name="memberCode"></param>
      /// <returns></returns>
      void ValidateIssuingAirline(string memberCode);

      /// <summary>
      /// validate auto calculated amount and percentage like: ISC percentage and ISC amount.
      /// </summary>
      /// <param name="percentage">isc percentage</param>
      /// <param name="evaluatedGrossAmount">evaluated gross amount</param>
      /// <param name="amount">isc amount</param>
      void ValidateIscPerAndAmt(double evaluatedGrossAmount, double percentage, double amount);

      /// <summary>
      /// CMP#520- Evaluates if user type has changed from normal to superuser and vice-versa
      /// and accordingly assigns permissions or deletes them
      /// </summary>
      /// <param name="userId">User Id of the User whose status is changed</param>
      /// <param name="isSuperUser">New value- Is Super User</param>
      bool ChangeUserPermission(int userId, int isSuperUser);

      #region SCP223072: Unable to change Member Code
      /// <summary>
      /// Method to udpate BiConfiguration Table and Mem_Last_Corr_Ref table for the Member.
      /// </summary>
      /// <param name="oldMemberCodeNumeric">Old Member Code Numeric</param>
      /// <param name="newMemberCodeNumeric">New Member CodeNumeric</param>
      /// <param name="member_id">Member Id</param>
      /// <param name="callFrom">In case of member update 'ISWEB'</param>
      /// <returns> 0 when failure; 1 when BiConfiguration Table update success; 2 when both i.e BiConfiguration Table and Mem_Last_Corr_Ref table update success.</returns>
      int UpdateBiConfigForNewMemberCodeNumeric(string oldMemberCodeNumeric, string newMemberCodeNumeric, int member_id, string callFrom = null);
      #endregion

      //SCP#409719 - ICH Contacts
      //Desc: Hooked a call to MemberManager.InsertMessageInOracleQueue() to quque member for ICH Profile Update XML Generation.
      //CMP-689-Flexible CH Activation Options : parameter added isFutureUpdateIsLive
      void InsertMessageInOracleQueue(string messageType, int memberId, int ichMembershipStatusId = -1, int achMembershipStatusId = -1, bool isFutureUpdateLive = false);

      #region "CMP #666: MISC Legal Archiving Per Location ID"
      List<ArchivalLocations> GetAssignedArchivalLocations(int memberId, int archivalType);

      bool InsertArchivalLocations(string locationSelectedIds, int associtionType, int loggedInUser, int memberId,
                                   int archivalType);

      int GetArchivalLocsInconsistency(int memberId, int archReqMiscRecInvReq, int archReqMiscPayInvReq, int recAssociationType, int payAssociationType);

      
      int GetAssociationType(List<ArchivalLocations> listUserContactLocations);

      List<ArchivalLocations> GetSortedLocationCode(List<ArchivalLocations> listLocationCode);

      #endregion
  }
}
