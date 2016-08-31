using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
  public interface IManageContactsManager
  {
  /// <summary>
  /// Adds new contact type to database
  /// </summary>
  /// <param name="contactType">Contact Type class object</param>
  /// <returns>Updated contactType class object</returns>
    ContactType AddContactType(ContactType contactType);

    /// <summary>
    /// updates contact type object to database
    /// </summary>
    /// <param name="contactType">Contact Type class object</param>
    /// <returns>Updated contactType class object</returns>
    ContactType UpdateContactType(ContactType contactType);

    /// <summary>
    /// Deletes contact type corresponding to contactTypeID passed
    /// </summary>
    /// <param name="contactTypeId"></param>
    /// <returns>True if deleted successfully false otherwise</returns>
    bool DeleteContactType(int contactTypeId);

    /// <summary>
    /// Gets details of contact type corresponding to cntact type id passed
    /// </summary>
    /// <param name="contactTypeId">ID of contact type for which details should be retrieved</param>
    /// <returns>Contact Type class object</returns>
    ContactType GetContactTypeDetails(int contactTypeId);

    /// <summary>
    /// Gets details of contact type corresponding to cntact type id passed
    /// </summary>
    /// <returns>Contact Type class object</returns>
    IList<ContactType> GetContactTypeList();

    /// <summary>
    /// Gets the contact type list.
    /// </summary>
    /// <param name="typeId">The type id.</param>
    /// <param name="groupId">The group id.</param>
    /// <param name="subGroupId">The sub group id.</param>
    /// <returns></returns>
    List<ContactType> GetContactTypeList(int typeId, int groupId, int subGroupId);

      /// <summary>
      /// Get User/Contact List based on UserId and Member Id
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
    List<LocationAssociation> GetUserContactListForLocAssociation(int userId, int memberId);

      /// <summary>
      /// Get Assigned User/Conact association
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    List<UserContactLocations> GetUserContactAssignedLocAssociation(int userId);

      /// <summary>
      ///  save the changes made to the Location Association of the Target User/contact
      /// </summary>
      /// <param name="locationSelectedIds"> Associated Location Ids </param>
      /// <param name="excludedLocIds"> Invisible Location Ids on screen. These should be excluded while saving the record</param>
      /// <param name="userContactId">Target User/Contact Id</param>
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
      /// MEMBER USERS TO VIEW ‘OWN LOCATION ASSOCIATION’
      /// </summary>
      /// <param name="userId"></param>
      /// <returns></returns>
    List<UserAssignedLocation> GetOwnAssignedLocAssociation(int userId);

      /// <summary>
      /// Populate Location Association based on User Id and MemberId 
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
    List<MemberLocationAssociation> GetMemberAssociationLocForDropdown(int userId, int memberId);

      /// <summary>
      /// Populate Location Association based on User Id and MemberId 
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
    List<MemberLocationAssociation> GetMemberAssociationLocForSearch(int userId, int memberId);

      /// <summary>
      /// Populate Location Associations based on User ID and Member Id for Invoice/Credit Capture
      /// </summary>
      /// <param name="userId"></param>
      /// <param name="memberId"></param>
      /// <returns></returns>
      List<MemberLocationAssociation> GetMemberAssoLocForInvCapture(int userId, int memberId);



  }
}
