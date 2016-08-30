using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
  public interface IContactRepository : IRepository<Contact>
  {

    /// <summary>
    /// Deletes Contact. 
    /// </summary>
    /// <param name="contactId">The ContactId.</param>
    /// <returns></returns>
    int DeleteMemberContact(int contactId);

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
    /// Checks if only contact assigned for contact type.
    /// </summary>
    /// <param name="contactId">The contactId.</param>
    /// <returns>true if only contact assigned , false otherwise.</returns>
    int IsOnlyContact(int contactId, int memberId, int contactTypeId);

    /// <summary>
    /// Get the contact information. 
    /// If email id exists in User table then return first name, last name and staff id from user table 
    /// otherwise return this information from contact table
    /// </summary>
    /// <param name="memberId">member id</param>
    /// <param name="firstName">first name</param>
    /// <param name="lastName">last name</param>
    /// <param name="emailAddress">email address</param>
    /// <param name="staffId">staff id</param>
    /// <returns>list of contact objects.</returns>
    List<ContactData> GetContactUserInformation(int memberId, string firstName, string lastName, string emailAddress, string staffId, int userCategory);

      /// <summary>
      /// Get Member contact 
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="contactTypeId"></param>
      /// <param name="miscLocationCode"></param>
      /// <returns></returns>
    List<Contact> GetContactsForMiscOutputAlerts(int memberId, int contactTypeId, string miscLocationCode);

    /// <summary>
    /// Get Member contact 
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="contactTypeId"></param>
    /// <returns></returns>
    List<Contact> GetContactMemberInformation(int memberId, int contactTypeId);

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
    List<LocationAssociation> GetUserContactListForLocAssociation(int userId, int memberId);

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    List<UserContactLocations> GetUserContactAssignedLocAssociation(int userId);

      /// <summary>
      ///  save the changes made to the Location Association of the Target User/contact
      /// </summary>
      /// <param name="locationSelectedIds"> Associated Location Ids </param>
      /// <param name="excludedLocIds"> Invisible Location Ids on screen. These should be excluded while saving the record</param>
      /// <param name="userContactId">Target User/Contact Id </param>
      /// <param name="associtionType"> Association Type</param>
      /// <param name="emailId">Email Id </param>
      /// <param name="loggedInUser">Logged In User</param>
      /// <param name="memberId">Member</param>
      /// <param name="isNewContact"></param>
      /// <returns>1/0</returns>
      bool InsertLocationAssociation(string locationSelectedIds, string excludedLocIds, int userContactId,
                                     string associtionType, string emailId, int loggedInUser, int memberId,
                                     int isNewContact = 0);
 

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    List<UserAssignedLocation> GetOwnAssignedLocAssociation(int userId);

      /// <summary>
      /// CMP#655IS-WEB Display per Location
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    List<MemberLocationAssociation> GetMemeberAssociationLocation(int userId);
  }
}
