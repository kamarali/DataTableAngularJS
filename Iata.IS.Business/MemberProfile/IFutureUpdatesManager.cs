using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Business.MemberProfile
{
  public interface IFutureUpdatesManager
  {
    /// <summary>
    /// Adds future update audit trail record for a member
    /// </summary>
    /// <param name="futureUpdates">Future Updates class object</param>
    /// <returns>Future Updates class object</returns>
    FutureUpdates AddFuturepdates(FutureUpdates futureUpdates);

    /// <summary>
    /// Returns list of future updates audit trail records for passed search criteria
    /// </summary>
    /// <returns>List of Future Updates class object</returns>
    List<FutureUpdates> GetFutureUpdatesList(int memberId, int elementGroupId, string tableName, int? relationId, int? locationId);

    /// <summary>
    /// Gets future update audit trail records corresponding to an element group and member ID
    /// </summary>
    /// <param name="egType">Element group</param>
    /// <param name="memberId">ID of member</param>
    /// <param name="elementName">Element for which audit trail records should be fetched.If null then records for all elements should be fetched</param>
    /// <param name="relationId"></param>
    /// <returns>Future Updates class object</returns>
    List<FutureUpdates> GetPendingFutureUpdates(ElementGroupType egType, int memberId, string elementName, int? relationId);

    /// <summary>
    /// Gets the entire list of pending changes to be applied for the specified group.
    /// </summary>
    /// <param name="memberId">The concerned member id.</param>
    /// <param name="elementGroupType">The element group for which the pending updates are to be retrieved.</param>
    /// <returns></returns>
    List<FutureUpdates> GetPendingFutureUpdates(int memberId, ElementGroupType elementGroupType, int? relationId = null);

    /// <summary>
    /// Gets future update audit trail records based on newvalue and element name specified
    /// </summary>
    /// <param name="egType">Element group</param>
    /// <param name="memberId"></param>
    /// <param name="elementName">Element for which audit trail records should be fetched.If null then records for all elements should be fetched</param>
    /// <param name="newValue"></param>
    /// <param name="relationId"></param>
    /// <param name="oldValue">this variable to get deleted member list</param>
    /// <returns>Future Updates class object</returns>
    List<FutureUpdates> GetPendingFutureUpdates(ElementGroupType egType, int memberId, string elementName, string newValue, int? relationId, string oldValue = null);

    
    /// <summary>
    /// Returns list of future updates audit trail records for passed search criteria
    /// </summary>
    /// <returns>List of Future Updates class object</returns>
    List<FutureUpdateDetails> GetFutureUpdatesList(string elementList, DateTime? fromDate, DateTime? todate, int userId, int userType, int isDateOrPeriodSearch, int memberId, string reportType);

    /// <summary>
    /// Returns list of future updates audit trail records for passed search criteria
    /// </summary>
    /// <returns>List of Future Updates class object</returns>
    List<FutureUpdates> GetFutureUpdatesList(string elementList, string fromDate, string todate);

    /// <summary>
    /// Returns list of future update records for updates done by member profile future update service job
    /// </summary>
    /// <returns>List of FutureUpdateTemp class object</returns>
    List<FutureUpdateTemp> GetFutureUpdatesDoneByService(DateTime ichBillingPeriod, DateTime achBillingPeriod);

    /// <summary>
    /// Returns display name value (human readable name for an id) for relation id in a future update record
    ///whose future update id is passed
    /// </summary>
    /// <returns>List of FutureUpdateTemp class object</returns>
    String GetRelationIdDisplayName(int futureUpdateId);

    /// <summary>
    /// Get user list for given user category and member id.
    /// </summary>
    /// <param name="userCategoryId">user category id</param>
    /// <param name="memberId"> member id</param>
    /// <returns>list of audit trail users corresponding to given user category id and member id.</returns>
    List<AuditTrailUserDetails> GetAuditTrailUserList(int userCategoryId, int memberId);

    /// <summary>
    /// This method will be used for applying the future updates applicable for current billing period or current date.
    /// </summary>
    void ApplyFutureUpdates();

    /// <summary>
    /// Inserts the member future updates message in oracle queue.
    /// </summary>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingPeriod">The billing period.</param>
    void InsertMemberFutureUpdatesMessageInOracleQueue(int billingYear, int billingMonth, int billingPeriod);
  }
}
