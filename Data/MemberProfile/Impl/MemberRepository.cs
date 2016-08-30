using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.IO;
using System.Linq;
using Enyim.Caching.Memcached;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.MemberProfile.Impl
{
  [Serializable]
  public class IdNumericCodeMap : ModelBase
  {
    public string MemberCodeNumeric { get; set; }
    public int MemberId { get; set; }
  }

  public class MemberRepository : Repository<Member>, IMemberRepository
  {
    public void Invalidate(int memberId)
    {
      CacheManager.Remove(GetCacheKey(memberId));
    }

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity to be deleted.</param>
    public override void Delete(Member entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;

      EntityObjectSet.DeleteObject(entity);

      // Remove specific member from cache
      CacheManager.Remove(GetCacheKey(entity.Id));

      // Remove it from id-Numeric code map);
      RemoveIdCodeMapEntry(entity);
    }

    /// <summary>
    /// Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns></returns>
    public override Member Update(Member entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;
      EntityObjectSet.ApplyCurrentValues(entity);

      // Remove specific member from cache
      CacheManager.Remove(GetCacheKey(entity.Id));

      // Remove it from id-Numeric code map);
      RemoveIdCodeMapEntry(entity);

      return entity;
    }

    /// <summary>
    /// Updates the id code map.
    /// </summary>
    /// <param name="member">The member.</param>
    private void RemoveIdCodeMapEntry(Member member)
    {
      var idCodeMap = (List<IdNumericCodeMap>) CacheManager.Get(typeof(Member).Name);

      if (idCodeMap == null)
      {
        return;
      }

      var idCodeMapEntry = idCodeMap.Where(rec => rec.MemberId == member.Id);
      idCodeMap.Remove(idCodeMapEntry.FirstOrDefault());
      CacheManager.Update(typeof(Member).Name, idCodeMap, StoreMode.Replace);
    }

    /// <summary>
    /// Gets the id for numeric code.
    /// </summary>
    /// <param name="numericCode">The numeric code.</param>
    /// <returns></returns>
    private int? GetIdForNumericCode(string numericCode)
    {
      var idCodeMap = (List<IdNumericCodeMap>) CacheManager.Get(typeof(Member).Name);
      if (idCodeMap != null)
      {
        var idCodeMapEntry = idCodeMap.Where(rec => rec.MemberCodeNumeric == numericCode);
        if (idCodeMapEntry.Count() > 0)
        {
          return idCodeMapEntry.FirstOrDefault().MemberId;
        }
      }

      return null;
    }

    /// <summary>
    /// Adds the id code map entry.
    /// </summary>
    /// <param name="member">The member.</param>
    private void AddIdCodeMapEntry(Member member)
    {
      var idCodeMap = (List<IdNumericCodeMap>) CacheManager.Get(typeof(Member).Name);

      var idCodeMapEntry = new IdNumericCodeMap { MemberCodeNumeric = member.MemberCodeNumeric, MemberId = member.Id };

      if (idCodeMap == null)
      {
        idCodeMap = new List<IdNumericCodeMap> { idCodeMapEntry };
        CacheManager.Update(typeof(Member).Name, idCodeMap, StoreMode.Add);
      }
      else
      {
        idCodeMap.Add(idCodeMapEntry);
        CacheManager.Update(typeof(Member).Name, idCodeMap, StoreMode.Replace);
      }
    }

    /// <summary>
    /// Deletes the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    void IRepository<Member>.Delete(Member entity)
    {
      entity.LastUpdatedOn = DateTime.UtcNow;
      EntityObjectSet.DeleteObject(entity);

      // Remove specific member from cache
      CacheManager.Remove(GetCacheKey(entity.Id));

      // Remove it from id-Numeric code map
      RemoveIdCodeMapEntry(entity);
    }

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="numericCode">The numeric code.</param>
    public Member GetMember(string numericCode)
    {
      // Get member id from ID-numeric code Map.
      int? memberId = GetIdForNumericCode(numericCode);

      // Try fetching it from Memcache then db
      var member = TryGetCachedCopy(memberId, numericCode);
      return member;
    }

    /// <summary>
    /// Gets the member.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    public Member GetMember(int memberId)
    {
      var member = TryGetCachedCopy(memberId, string.Empty);

      return member;
    }

    /// <summary>
    /// Gets the member id.
    /// </summary>
    /// <param name="numericCode">The numeric code.</param>
    public int GetMemberId(string numericCode)
    {
      // Get member id from ID-numeric code Map.
      int? memberId = GetIdForNumericCode(numericCode);

      // If member Id is null then get member from the database.
      if (!memberId.HasValue)
      {
        // Try fetching it from Member cache then db.
        var member = TryGetCachedCopy(memberId, numericCode);
        if (member != null)
        {
          memberId = member.Id;
        }
      }

      return memberId.HasValue ? memberId.Value : 0;
    }

    /// <summary>
    /// Gets the members numeric code in specific format for auto-complete field.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <param name="memberIdToSkip">The member id.</param>
    /// <param name="includePending">if set to true membership status is pending.</param>
    /// <param name="includeBasic">if set to true membership status is basic</param>
    /// <param name="includeRestricted">if set to true membership status is restricted.</param>
    /// <param name="includeTerminated">if set to true membership status is terminated.</param>
    /// <param name="includeOnlyAch">if set to true include ACH</param>
    /// <param name="includeOnlyIch">if set to true include ICH</param>
    /// <param name="excludeMergedMember">if set to true exclude merged member</param>
    /// <param name="ichZone">if set to true include ICH zones</param>
    /// <param name="includeMemberType">Assistance for includeOnlyAch and includeOnlyIch Parameters. Which will be used in further filteration of Member List</param>
    /// <returns></returns>
    /* CMP #596: Length of Member Accounting Code to be Increased to 12 
     * Desc: Added new parameter to exclude Type B Members. */
    public string GetMembers(string filter,
                             int memberIdToSkip,
                             bool includePending,
                             bool includeBasic,
                             bool includeRestricted,
                             bool includeTerminated,
                             bool includeOnlyAch,
                             bool includeOnlyIch,
                             int ichZone,
                             bool excludeMergedMember=false,
                             int includeMemberType = 0, 
                             bool excludeTypeBMembers = false)
    {
        // Added new parmeter INCLUDE_MEMBER_TYPE in assistance for INCLUDE_ACH_I and INCLUDE_ICH_I Parameters
        //which will be used in further filteration of Member List data by following logic: 
        //INCLUDE_MEMBER_TYPE =0 => Default
        //INCLUDE_MEMBER_TYPE =1 => Include Only ACH Members(Excluding Dual Members and Not a member of ACH/ICH)
        //INCLUDE_MEMBER_TYPE =2 => Include Only ICH Members(Excluding Dual Members and Not a member of ACH/ICH)
        //INCLUDE_MEMBER_TYPE =3 => Include ACH Member and ICH Member (Including Dual Members but Excluding Not a member of ACH/ICH)
        //INCLUDE_MEMBER_TYPE =4 => Get Only BVC participants members without terminated members
        
        // CMP597: INCLUDE_MEMBER_TYPE = 9 => exclude Members having ‘IS Membership Sub Status’ as “Terminated” (irrespective of ‘IS Membership Status’)
      var parameters = new ObjectParameter[13];
      parameters[0] = new ObjectParameter(MemberRepositoryConstants.FilterParameterName, typeof(string)) { Value = filter };
      parameters[1] = new ObjectParameter(MemberRepositoryConstants.CurrentMemberIdParameterName, typeof(int)) { Value = memberIdToSkip };
      parameters[2] = new ObjectParameter(MemberRepositoryConstants.IncludePendingParameterName, typeof(int)) { Value = includePending ? 1 : 0 };
      parameters[3] = new ObjectParameter(MemberRepositoryConstants.IncludeBasicParameterName, typeof(int)) { Value = includeBasic ? 1 : 0 };
      parameters[4] = new ObjectParameter(MemberRepositoryConstants.IncludeRestrictedParameterName, typeof(int)) { Value = includeRestricted ? 1 : 0 };
      parameters[5] = new ObjectParameter(MemberRepositoryConstants.IncludeTerminatedParameterName, typeof(int)) { Value = includeTerminated ? 1 : 0 };
      parameters[6] = new ObjectParameter(MemberRepositoryConstants.IncludeAchParameterName, typeof(int)) { Value = includeOnlyAch ? 1 : 0 };
      parameters[7] = new ObjectParameter(MemberRepositoryConstants.IncludeIchParameterName, typeof(int)) { Value = includeOnlyIch ? 1 : 0 };
      parameters[8] = new ObjectParameter(MemberRepositoryConstants.IncludeIchZonesParameterName, typeof(int)) { Value = ichZone };
      parameters[9] = new ObjectParameter(MemberRepositoryConstants.ExcludeMergedMember, typeof(int)) { Value = excludeMergedMember ? 1 : 0 };
      parameters[10] = new ObjectParameter(MemberRepositoryConstants.IncludeMemberType, typeof(int)) { Value = includeMemberType };
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
      * Desc: Added new parameter to exclude Type B Members. */
      parameters[11] = new ObjectParameter(MemberRepositoryConstants.ExcludeTypeBMember, typeof(int)) { Value = excludeTypeBMembers ? 1 : 0 };
      parameters[12] = new ObjectParameter(MemberRepositoryConstants.MemberStringOutputParameterName, typeof(string));

      ExecuteStoredProcedure(MemberRepositoryConstants.GetMembersFunctionName, parameters);

      return parameters[12].Value.ToString();
    }

    /// <summary>
    /// Get Final Parent's Member Id for given member Id
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <returns></returns>
    public int GetFinalParentDetails(int memberId)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(MemberRepositoryConstants.MemberIdParameterName, typeof(int)) { Value = memberId };
      parameters[1] = new ObjectParameter(MemberRepositoryConstants.MemberFinalParentId, typeof(int));
      ExecuteStoredProcedure(MemberRepositoryConstants.GetFinalParentFunctionName, parameters);

      return Convert.ToInt16(parameters[1].Value);
    }


    /// <summary>
    /// Get Final Parent's Member Id for given member Id
    /// </summary>
    /// <param name="memberId">Member Id</param>
    /// <returns></returns>
    public IList<ChildMemberList> GetChildMembers(int memberId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(MemberRepositoryConstants.MemberIdParameterName, typeof(int)) { Value = memberId };
      //parameters[1] = new ObjectParameter(MemberRepositoryConstants.ChildMemberList, typeof(IList<ChildMemberList>));
      //ExecuteStoredProcedure(MemberRepositoryConstants.GetChildMembers, parameters);

      var childMemberList = ExecuteStoredFunction<ChildMemberList>(MemberRepositoryConstants.GetChildMembers, parameters);

      return childMemberList.ToList();
    }

    
    /// <summary>
    /// Tries the get cached copy.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    /// <param name="numericCode">The numeric code.</param>
    /// <returns></returns>
    private Member TryGetCachedCopy(int? memberId, string numericCode)
    {
      try
      {
        string key;
        Member member = null;

        // Member Id is given or could retrieve from the IdNumericCode Map 
        // Then only try to fetch it from mem-cached
        if (memberId.HasValue)
        {
          key = GetCacheKey(memberId.Value);

          // Try Retrieving it from MemCache.
          member = (Member) CacheManager.Get(key);

          // if member is not null then return it. 
          if (member != null)
          {
            return member;
          }

          // If MemCache don't have the requires cacheable entity.
          // Then retrieve it from DB; Update the MemCache.
          // CacheManager.Add(key, object);
          // Return db output as it is.
          member = EntityObjectSet.Single(rec => rec.Id == memberId.Value);
          EntityObjectSet.Context.Refresh(RefreshMode.StoreWins, member);
        }

          // If only numeric code is given 
          // Fire query to db using numeric code 
        else if (!string.IsNullOrEmpty(numericCode))
        {
          member = EntityObjectSet.Single(rec => rec.MemberCodeNumeric == numericCode);
          EntityObjectSet.Context.Refresh(RefreshMode.StoreWins, member);
        }

        // if after Db call if there is no member matching criteria then return null.
        if (member == null)
        {
          return null;
        }

        // if member found in db - Update cache .
        key = GetCacheKey(member.Id);
        CacheManager.Add(key, member);
        AddIdCodeMapEntry(member);

        return member;
      }
      catch (ObjectDisposedException objectDisposedException)
      {
        // ObjectDisposedException is thrown from memCache client, from Enyim.Caching.Memcached.DefaultNodeLocator.callback_isAliveTimer(System.Object).
        // Log it.
        Logger.Error(objectDisposedException.Message, objectDisposedException);
      }
      catch (InvalidOperationException invalidOperationException)
      {
        // This exception will occur if member id is invalid.
        Logger.Error("Invalid member id.", invalidOperationException);
      }
      catch (IOException ioException)
      {
        // If any exception from memCache then log it.
        Logger.Error("I/O exception", ioException);
      }

      return null;
    }

    /// <summary>
    /// Gets the cache key.
    /// </summary>
    /// <param name="memberId">The member id.</param>
    private static string GetCacheKey(int memberId)
    {
      return (typeof(Member).Name + "_" + memberId);
    }

    /// <summary>
    /// Load strategy to load billed member detail for invoice header
    /// </summary>
    public static List<Member> LoadEntities(ObjectSet<Member> objectSet, LoadStrategyResult loadStrategyResult, Action<Member> link, string entity)
    {
      if (link == null)
      {
        link = new Action<Member>(c => { });
      }

      var invoiceMember = new List<Member>();
      var commonMaterializers = new CommonMaterializers();
      using (var reader = loadStrategyResult.GetReader(entity))
      {
        // first result set includes the category
        invoiceMember.AddRange(commonMaterializers.MemberMaterializer.Materialize(reader).Bind(objectSet).ForEach(link));
        reader.Close();
      }

      return invoiceMember;
    }

    /// <summary>
    /// LoadStrategy for the BHAudit
    /// </summary>
    public static List<Member> LoadAuditEntities(ObjectSet<Member> objectSet, LoadStrategyResult loadStrategyResult, Action<Member> link, string entity)
    {
      if (link == null)
      {
        link = new Action<Member>(c => { });
      }

      var invoiceMember = new List<Member>();
      var commonMaterializers = new CommonMaterializers();
      using (var reader = loadStrategyResult.GetReader(entity))
      {
        // first result set includes the category
        invoiceMember.AddRange(commonMaterializers.PaxInvoiceMemberAuditMaterializer.Materialize(reader).Bind(objectSet).ForEach(link));
        reader.Close();
      }

      return invoiceMember;
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
      var parameters = new ObjectParameter[3];

      parameters[0] = new ObjectParameter(MemberRepositoryConstants.MemberIdParameterName, typeof(int)) { Value = memberId };
      parameters[1] = new ObjectParameter(MemberRepositoryConstants.ConfigDetailParameterName, typeof(int)) { Value = (int) configParameter };
      parameters[2] = new ObjectParameter(MemberRepositoryConstants.ValueOutputParameterName, typeof(string));

      ExecuteStoredProcedure(MemberRepositoryConstants.GetMemberConfigFunctionName, parameters);
      return parameters[2].Value.ToString();
    }

      public IList<RequiredContactType> GetContactTypeByContactId(int contactId)
      {
          var parameters = new ObjectParameter[1];
          parameters[0] = new ObjectParameter("CONTACT_ID_I", typeof(Int32)) { Value = contactId };

          var contactTypeList = ExecuteStoredFunction<RequiredContactType>("GetRequiredContactType", parameters);

          return contactTypeList.ToList();
      }

      public IList<ISFileList> GetIsFileListByFileType(int fileType)
      {
          var parameters = new ObjectParameter[1];
          parameters[0] = new ObjectParameter("FileType_I", typeof(Int32)) { Value = fileType };

          var fileList = ExecuteStoredFunction<ISFileList>("GetFileListByFileType", parameters);

          return fileList.ToList();

      }

    /// <summary>
    /// CMP#520- Evaluates if user type has changed from normal to superuser and vice-versa
    /// and accordingly assigns permissions or deletes them
    /// </summary>
    /// <param name="userId">User Id of the User whose status is changed</param>
    /// <param name="userType">New value- Is Super User</param>
    /// <returns>boolean value </returns>
    public bool ChangeUserPermission (int userId, int userType)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(MemberRepositoryConstants.UserIdParameterName, typeof(Int32)) { Value = userId };
      parameters[1] = new ObjectParameter(MemberRepositoryConstants.IsSuperUserParameterName, typeof(Int32)) { Value = userType };
      
      ExecuteStoredProcedure(MemberRepositoryConstants.ChangeUserStatusFunctionName, parameters);
      
      return true;
    }

    public Member CreateISMember(Member member, Location location, MemberStatusDetails memberStatusDetails)
    {
      var parameters = new ObjectParameter[38];
      parameters[0] = new ObjectParameter(MemberRepositoryConstants.MemberCodeNumeric, typeof(String)) { Value = member.MemberCodeNumeric };
      parameters[1] = new ObjectParameter(MemberRepositoryConstants.MemberCodeAlpha, typeof(String)) { Value = member.MemberCodeAlpha };
      parameters[2] = new ObjectParameter(MemberRepositoryConstants.IsMemberShipStatus, typeof(Int32)) { Value = member.IsMembershipStatusId };
      parameters[3] = new ObjectParameter(MemberRepositoryConstants.IsMigrationStatus, typeof(Int32)) { Value = member.IsMigrationStatusId };
      parameters[4] = new ObjectParameter(MemberRepositoryConstants.IataMemberStatus, typeof(Int32)) { Value = member.IataMemberStatus };
      parameters[5] = new ObjectParameter(MemberRepositoryConstants.IsOpsComments, typeof(String)) { Value = member.IsOpsComments };
      parameters[6] = new ObjectParameter(MemberRepositoryConstants.MemberCommercialName, typeof(String)) { Value = member.CommercialName };
      parameters[7] = new ObjectParameter(MemberRepositoryConstants.IsMembershipSubStatusId, typeof(Int32)) { Value = member.IsMembershipSubStatusId };
      parameters[8] = new ObjectParameter(MemberRepositoryConstants.MemberLegalName, typeof(String)) { Value = member.LegalName };
      parameters[9] = new ObjectParameter(MemberRepositoryConstants.MemberShipStatus, typeof(Int32)) { Value = memberStatusDetails.MembershipStatusId };
      parameters[10] = new ObjectParameter(MemberRepositoryConstants.StatusChangeDate, typeof(DateTime)) { Value = memberStatusDetails.StatusChangeDate };
      parameters[11] = new ObjectParameter(MemberRepositoryConstants.MemberType, typeof(String)) { Value = memberStatusDetails.MemberType };
      parameters[12] = new ObjectParameter(MemberRepositoryConstants.LocationCode, typeof(String)) { Value = location.LocationCode };
      parameters[13] = new ObjectParameter(MemberRepositoryConstants.LocMemberLegalName, typeof(String)) { Value = location.MemberLegalName };
      parameters[14] = new ObjectParameter(MemberRepositoryConstants.LocMemberCommName, typeof(String)) { Value = location.MemberCommercialName };
      parameters[15] = new ObjectParameter(MemberRepositoryConstants.LocActive, typeof(Int32)) { Value = location.IsActive };
      parameters[16] = new ObjectParameter(MemberRepositoryConstants.RegistrationId, typeof(String)) { Value = location.RegistrationId };
      parameters[17] = new ObjectParameter(MemberRepositoryConstants.TaxVatRegistrationNumber, typeof(String)) { Value = location.TaxVatRegistrationNumber };
      parameters[18] = new ObjectParameter(MemberRepositoryConstants.AddTaxVatRegistrationNumber, typeof(String)) { Value = location.AdditionalTaxVatRegistrationNumber };
      parameters[19] = new ObjectParameter(MemberRepositoryConstants.CountryCode, typeof(String)) { Value = location.CountryId };
      parameters[20] = new ObjectParameter(MemberRepositoryConstants.AddressLine1, typeof(String)) { Value = location.AddressLine1 };
      parameters[21] = new ObjectParameter(MemberRepositoryConstants.AddressLine2, typeof(String)) { Value = location.AddressLine2 };
      parameters[22] = new ObjectParameter(MemberRepositoryConstants.AddressLine3, typeof(String)) { Value = location.AddressLine3 };
      parameters[23] = new ObjectParameter(MemberRepositoryConstants.CityName, typeof(String)) { Value = location.CityName };
      parameters[24] = new ObjectParameter(MemberRepositoryConstants.SubDivisonCode, typeof(String)) { Value = location.SubDivisionCode };
      parameters[25] = new ObjectParameter(MemberRepositoryConstants.SubDivisionName, typeof(String)) { Value = location.SubDivisionName };
      parameters[26] = new ObjectParameter(MemberRepositoryConstants.PostalCode, typeof(String)) { Value = location.PostalCode };
      parameters[27] = new ObjectParameter(MemberRepositoryConstants.LegalText, typeof(String)) { Value = location.LegalText };
      parameters[28] = new ObjectParameter(MemberRepositoryConstants.Iban, typeof(String)) { Value = location.Iban };
      parameters[29] = new ObjectParameter(MemberRepositoryConstants.Swift, typeof(String)) { Value = location.Swift };
      parameters[30] = new ObjectParameter(MemberRepositoryConstants.BankCode, typeof(String)) { Value = location.BankCode };
      parameters[31] = new ObjectParameter(MemberRepositoryConstants.BranchCode, typeof(String)) { Value = location.BranchCode };
      parameters[32] = new ObjectParameter(MemberRepositoryConstants.BankAccountNumber, typeof(String)) { Value = location.BankAccountNumber };
      parameters[33] = new ObjectParameter(MemberRepositoryConstants.BankAccountName, typeof(String)) { Value = location.BankAccountName };
      parameters[34] = new ObjectParameter(MemberRepositoryConstants.CurrencyCode, typeof(Int32)) { Value = location.CurrencyId };
      parameters[35] = new ObjectParameter(MemberRepositoryConstants.BankName, typeof(String)) { Value = location.BankName };
      parameters[36] = new ObjectParameter(MemberRepositoryConstants.LastUpdateBy, typeof(Int32)) { Value = location.LastUpdatedBy };
      parameters[37] = new ObjectParameter(MemberRepositoryConstants.MemberIdOut, typeof(Int32));

      ExecuteStoredProcedure(MemberRepositoryConstants.CreateISMemberFunctionName, parameters);

      member.Id = Convert.ToInt32(parameters[37].Value);

      //CMP544: If record is inserted via is-web it should be restricted in Sandbox.
      if (member.Id == -20401)
        throw new Exception("Record Insertion is restrict from IS WEB.", new Exception("-20401, Record Insertion is restrict from IS WEB."));

      return member;
    }

    #region SCP223072: Unable to change Member Code
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
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(MemberRepositoryConstants.OldMemberCodeNumericParameterName, typeof(string)) { Value = oldMemberCodeNumeric };
      parameters[1] = new ObjectParameter(MemberRepositoryConstants.NewMemberCodeNumericParameterName, typeof(string)) { Value = newMemberCodeNumeric };
      parameters[2] = new ObjectParameter(MemberRepositoryConstants.MembersIdParameterName, typeof (int)) {Value = memberId};
      parameters[3] = new ObjectParameter(MemberRepositoryConstants.CallFromParameterName, typeof(string)) { Value = callFrom };
      parameters[4] = new ObjectParameter(MemberRepositoryConstants.ResultParameterName, typeof(int));
      ExecuteStoredProcedure(MemberRepositoryConstants.UpdateBiConfigForNewMemberCodeNumericFunctionName, parameters);

      return Convert.ToInt32(parameters[4].Value);
    }
    #endregion

    #region "CMP #666: MISC Legal Archiving Per Location ID"

    public List<ArchivalLocations> GetAssignedArchivalLocations(int memberId, int archivalType)
    {
        var parameters = new ObjectParameter[2];
        parameters[0] = new ObjectParameter(ContactConstants.MemberIdParameterName, typeof(int)) { Value = memberId };
        parameters[1] = new ObjectParameter(ContactConstants.ArchivalType, typeof(int)) { Value = archivalType };
        return ExecuteStoredFunction<ArchivalLocations>(ContactConstants.GetArchivalLocations, parameters).ToList();
    }

    public bool InsertArchivalLocations(string locationSelectedIds, int associtionType, int loggedInUser, int memberId, int archivalType)
    {
        var parameters = new ObjectParameter[6];
        parameters[0] = new ObjectParameter("SELECTED_LOC_IDS", typeof(string)) { Value = locationSelectedIds };
        parameters[1] = new ObjectParameter("LOGGEDIN_USER_I", typeof(int)) { Value = loggedInUser };
        parameters[2] = new ObjectParameter("MEMBER_ID_I", typeof(int)) { Value = memberId };
        parameters[3] = new ObjectParameter("ASSOCIATION_TYPE_I", typeof(int)) { Value = associtionType };
        parameters[4] = new ObjectParameter("ARCHIVAL_TYPE_I", typeof(int)) { Value = archivalType };
        parameters[5] = new ObjectParameter("RESULT_O", typeof(int));
        ExecuteStoredProcedure("InsertArchivalLocations", parameters);

        return int.Parse(parameters[5].Value.ToString()) == 1;

    }

    public int GetArchivalLocsInconsistency(int memberId, int archReqMiscRecInvReq, int archReqMiscPayInvReq, int recAssociationType, int payAssociationType)
    {
        var parameters = new ObjectParameter[6];
        parameters[0] = new ObjectParameter("MEMBER_ID_I", typeof(int)) { Value = memberId };
        parameters[1] = new ObjectParameter("ARC_REQ_MISCRECINV_REQ_I", typeof(int)) { Value = archReqMiscRecInvReq };
        parameters[2] = new ObjectParameter("ARC_REQ_MISCPAYINV_REQ_I", typeof(int)) { Value = archReqMiscPayInvReq };
        parameters[3] = new ObjectParameter("REC_ASSOCIATION_TYPE_I", typeof(int)) { Value = recAssociationType };
        parameters[4] = new ObjectParameter("PAY_ASSOCIATION_TYPE_I", typeof(int)) { Value = payAssociationType };
        parameters[5] = new ObjectParameter("RESULT_O", typeof(int));
        ExecuteStoredProcedure("GetArchivalLocsInconsistency", parameters);
        return int.Parse(parameters[5].Value.ToString());

    }

    #endregion
  }
}
