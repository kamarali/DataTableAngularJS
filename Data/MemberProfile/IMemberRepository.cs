using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
  public interface IMemberRepository : IRepository<Member>
  {
    /// <summary>
    /// Removes the member key from the cache.
    /// </summary>
    /// <param name="memberId"></param>
    void Invalidate(int memberId);

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <returns></returns>
    Member GetMember(int memberId);

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="numericCode">The numeric code.</param>
    /// <returns></returns>
    Member GetMember(string numericCode);

    /// <summary>
    /// Gets the member id.
    /// </summary>
    /// <param name="numericCode">The numeric code.</param>
    /// <returns></returns>
    int GetMemberId(string numericCode);

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
    /// Gets the members numeric code in specific format for auto-complete field.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="memberIdToSkip">The member id to be skipped from the result set.</param>
    /// <param name="includePending">if set to true membership status is pending.</param>
    /// <param name="includeBasic">if set to true membership status is basic</param>
    /// <param name="includeRestricted">if set to true membership status is restricted.</param>
    /// <param name="includeTerminated">if set to true membership status is terminated.</param>
    /// <param name="includeOnlyAch">if set to true include ach</param>
    /// <param name="includeOnlyIch">if set to true include ich</param>
    /// <param name="ichZone">ICH zone to include. Pass 0 for all zones.</param>
    /// <param name="includeMemberType">to include member type.</param>
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
     * Desc: Added new parameter excludeTypeBMembers. */
    string GetMembers(string filter, int memberIdToSkip, bool includePending, bool includeBasic, bool includeRestricted, bool includeTerminated, bool includeOnlyAch, bool includeOnlyIch, int ichZone, bool excludeMergedMember, int includeMemberType = 0, bool excludeTypeBMembers = false);

    /// <summary>
    /// Function used to fetch config values for member. This stored procedure is added for performance improvement to fetch only one column value 
    /// instead of fetching one config object for one column value
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="configParameter"></param>
    /// <returns></returns>
    string GetMemberConfigurationValue(int memberId, MemberConfigParameter configParameter);

      /// <summary>
      /// Get Contact Type Name by Contact Id
      /// </summary>
      /// <param name="contactId"> Contact ID </param>
      /// <returns> return object of  RequiredContactType</returns>
    IList<RequiredContactType> GetContactTypeByContactId(int contactId);


      IList<ISFileList> GetIsFileListByFileType(int fileType);

    /// <summary>
    /// CMP#520- Evaluates if user type has changed from normal to superuser and vice-versa
    /// and accordingly assigns permissions or deletes them
    /// </summary>
    /// <param name="userId">User Id of the User whose status is changed</param>
    /// <param name="userType">New value- Is Super User</param>
    /// <returns>boolean value </returns>
    bool ChangeUserPermission(int userId, int userType);

    /// <summary>
    /// Create member using stored procedure
    /// </summary>
    /// <param name="member"></param>
    /// <param name="location"></param>
    /// <param name="memberStatusDetails"></param>
    /// <returns></returns>
    Member CreateISMember(Member member, Location location, MemberStatusDetails memberStatusDetails);

    #region SCP223072: Unable to change Member Code

    /// <summary>
    /// Method to udpate BiConfiguration Table and Mem_Last_Corr_Ref table for the Member.
    /// </summary>
    /// <param name="oldMemberCodeNumeric">Old Member Code Numeric</param>
    /// <param name="newMemberCodeNumeric">New Member CodeNumeric</param>
    /// <param name="memberId">Member Id</param>
    /// <param name="callFrom">In case of member update 'ISWEB'</param>
    /// <returns> 0 when failure; 1 when BiConfiguration Table update success; 2 when both i.e BiConfiguration Table and Mem_Last_Corr_Ref table update success.</returns>
    int UpdateBiConfigForNewMemberCodeNumeric(string oldMemberCodeNumeric, string newMemberCodeNumeric, int memberId, string callFrom = null);
    #endregion

    #region "CMP #666: MISC Legal Archiving Per Location ID"
    List<ArchivalLocations> GetAssignedArchivalLocations(int memberId, int archivalType);

    bool InsertArchivalLocations(string locationSelectedIds, int associtionType, int loggedInUser, int memberId,
                                   int archivalType);

    int GetArchivalLocsInconsistency(int memberId, int archReqMiscRecInvReq, int archReqMiscPayInvReq, int recAssociationType, int payAssociationType);

      #endregion
  }
}