using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class AchEntryDetail : EntityBase<int>
  {
    public DateTime EntryDate { get; set; }

    public DateTime TerminationDate { get; set; }

    public Member Member { get; set; }

    public int MemberId { get; set; }
  }
}
