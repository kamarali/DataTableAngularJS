using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
  public interface IBlockingRulesManager
  {
    /// <summary>
    /// Add new BlockingRule to database.
    /// </summary>
    /// <param name="blockingRule">BlockinRule object.</param>
    /// <returns>Added blockingRule object.</returns>
    BlockingRule AddBlockingRule(BlockingRule blockingRule);

    /// <summary>
    /// Update BlockingRules record in the database.
    /// </summary>
    /// <param name="memberId">MemberId for which blockingrules is to be updated.</param>
    /// <param name="blockingRule">BlockingRules class oblject.</param>
    /// <returns>Updated BlockingRule object.</returns>
    BlockingRule UpdateBlockingRule(BlockingRule blockingRule);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="blockMember"></param>
    /// <returns></returns>
    BlockMember AddBlockMember(BlockMember blockMember);


    BlockGroup AddBlockGroup(BlockGroup blockGroup);

    List<BlockingRule> GetBlockingRuleList(string memberId, string blockingRule, string description, string clearingHouse);

    bool DeleteBlockingRule(int blockingRuleId);

    BlockingRule GetBlockingRuleDetails(int blockingRuleId);

    List<BlockGroup> GetBlockGroupList(int blockingRuleId);

    bool DeleteBlockedGroup(int groupId);
    List<BlockMember> GetBlockMemberList(int blockingRuleId, bool isDebtor);

    bool DeleteBlockedMember(int memberId);

    List<BlockGroupException> GetBlockGroupExceptionsList(int groupId);

    bool DeleteBlockedGroupException(int blockGroupId, int exceptionMemberId);

    BlockGroupException AddBlockGroupException(BlockGroupException blockGroupException);

    /// <summary>
    /// Update blocking member in database
    /// </summary>
    /// <param name="blockMember">Block Member details</param>
    void UpdateBlockMember(BlockMember blockMember);

    /// <summary>
    /// Update blocking group in database
    /// </summary>
    /// <param name="blockMember">Block Group details</param>
    void UpdateBlockGroup(BlockGroup blockGroup);

    /// <summary>
    /// Gets the count debtor or creditor members for given block rule id.
    /// </summary>
    /// <param name="blockingRuleId">blocking rule id</param>
    /// <param name="isDebtor">Boolean flag to indicate that is debtor or creditor</param>
    /// <returns>Returns the count of members matching the given criteria.</returns>
    long GetBlockMemberCount(int blockingRuleId, bool isDebtor);

    /// <summary>
    /// Gets the count of blocked groups for given block rule id.
    /// </summary>
    /// <param name="blockingRuleId">blocking rule id</param>
    /// <returns>Returns the count of blocked groups matching the given criteria.</returns>
    long GetBlockGroupCount(int blockingRuleId);

    /// <summary>
    /// Following method is used to generate BlockingRule update Xml when blocking rule is updated
    /// </summary>
    /// <param name="blockingRuleId">Blocking rule Id</param>
    /// <returns>Blocking rule update Xml</returns>
    void GenerateBlockingRuleUpdateXml(int blockingRuleId);

    /// <summary>
    /// To get the memberlist of blocked creditor/Debitor.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="isDebtor"></param>
    /// <returns></returns>
    List<BlockMember> GetBlockedMemberList(int memberId, bool isDebtor, bool isPax, bool isMisc, bool isUatp, bool isCargo);
    

    /// <summary>
    /// To check wheather member is blocked or not.
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="blockedMemberList"></param>
    /// <returns></returns>
    bool IsMemeberBlocked(int memberId,List<BlockMember> blockedMemberList);

    /// <summary>
    /// To get blocking rules by meberId
    /// </summary>
    /// <param name="memberId"></param>
    /// <returns></returns>
    List<BlockingRule> GetBlockingRuleDetailsByMemberId(int memberId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="clearingHouse"></param>
    /// <returns></returns>
    List<DownloadBlockingRules> GetBlokingRulesForClearingHouse(string clearingHouse);
   
  }
}
