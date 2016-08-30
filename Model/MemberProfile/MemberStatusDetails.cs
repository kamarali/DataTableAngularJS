using System;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class MemberStatusDetails : EntityBase<int>
  {
    public Member Member { get; set; }

    public int MemberId { get; set; }

    public int MembershipStatusId { get; set; }

    public MemberStatus MembershipStatus
    {
      get
      {
        return (MemberStatus)MembershipStatusId;
      }
      set
      {
        MembershipStatusId = Convert.ToInt32(value);
      }
    }
    
   
    public string DisplayIchMembershipStatus
    {
      get
      {
        return Enum.GetName(typeof(IchMemberShipStatus), MembershipStatusId);
       
      }
      
    }
    public DateTime StatusChangeDate { get; set; }

    public string MemberType { get; set; }
  }
}
