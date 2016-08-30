using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class BlockMember : EntityBase<int>
  {
    public bool IsDebtors { get; set; }
    
    public bool Pax { get; set; }

    public bool Cargo { get; set; }

    public bool Uatp { get; set; }

    public bool Misc { get; set; }

    //Blocked member
    public Member Member { get; set; }

    public int MemberId { get; set; }
    
    public string MemberText { get; set; }

    public BlockingRule BlockingRule { get; set; }

    public int BlockingRuleId { get; set; }

    public bool IsDeleted { get; set; }

    public string DisplayMemberCode
    {
        get
        {
            return Member != null ? Member.MemberCodeAlpha + "-"  + Member.MemberCodeNumeric : string.Empty;
        }
    }

    public string DisplayMemberCommercialName
    {
        get
        {
            return Member != null ? Member.CommercialName : null;
        }
    }

  }
}
