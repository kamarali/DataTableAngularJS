using Iata.IS.Model.Base;
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class IchSuspensionDetail : EntityBase<int>
  {
    
    public int SuspensionPeriod { get; set; }

    public int SuspensionMonth { get; set; }

    public int SuspensionYear { get; set; }
    
    public int DefaultSuspensionPeriod { get; set; }

    public int DefaultSuspensionMonth { get; set; }

    public int DefaultSuspensionYear { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }
  }
}
