using Iata.IS.Model.Base;
using System;
namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
    public class AchSuspensionDetail : EntityBase<int>
  {
    public int SuspensionPeriod { get; set; }

    public int SuspensionMonth { get; set; }

    public int SuspensionYear { get; set; }

    public int ApplicableSuspensionPeriod { get; set; }

    public int ApplicableSuspensionMonth { get; set; }

    public int ApplicableSuspensionYear { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }
  }
}
