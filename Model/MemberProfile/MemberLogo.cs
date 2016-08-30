using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile.Enums;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class MemberLogo : EntityBase<int>
  {
    public Member Member { get; set; }

    public int MemberId { get; set; }
    public byte[] Logo { get; set; }
  }
}
