using System;
using System.Collections.Generic;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class BlockingRule : EntityBase<int>
  {
    public string RuleName { get; set; }

    public string ClearingHouse { get; set; }

    public string Description { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }

    public int MemberId_1 { get; set; }

    public string DeletedBlockedCreditorString { get; set; }

    public string DeletedBlockedDebtorString { get; set; }

    public string DeletedGroupByBlockString { get; set; }

    public string DeletedExceptionRowString { get; set; }

    /// <summary>
    /// Property to get and set whether user has opened Blocking rule details in edit mode
    /// </summary>
    public bool IsInEditMode { get; set; }

    //Added code in get method .The value of field will be determined by Aggregated member object
    private string _memberText;
    public string MemberText
    {
      get
      {
        return !string.IsNullOrEmpty(_memberText) ? _memberText : Member != null ? string.Format("{0}-{1}-{2}", Member.MemberCodeAlpha, Member.MemberCodeNumeric, Member.CommercialName) : string.Empty;
      }
      set
      {
        _memberText = value;
      }
    }

    public List<BlockMember> BlockedMembers { get; set; }

    public List<BlockGroup> BlockedGroups { get; set; }

    public List<BlockGroupException> BlockedGroupExceptions { get; set; }

    public DateTime CreatedOn { get; set; } 

    public bool IsDeleted { get; set; }
    public BlockingRule()
    {
      BlockedMembers = new List<BlockMember>();
      BlockedGroups = new List<BlockGroup>();
      BlockedGroupExceptions = new List<BlockGroupException>();
    }
  }
}
