using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class BilateralConfiguration : EntityBase<string>
  {
    public string SentTo { get; set; }

    public string SentBy { get; set; }

    public DateTime SentOn { get; set; }

    public string Element { get; set; }

    public char OldValue { get; set; }

    public char NewValue { get; set; }

    public DateTime ChangeEffectiveDate { get; set; }

    public string Status { get; set; }
  }
}
