using Iata.IS.Model.Base;
using System;
namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class AchReinstatementDetail : EntityBase<int>
  {
    public int ReinstatementYear { get; set; }

    public int ReinstatementMonth { get; set; }

    public int ReinstatementPeriod { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }
  }
}
