using System;

namespace Iata.IS.Model.MemberProfile
{
  public class FutureUpdateDetails
  {

    public int AuditId { get; set; }

    public int MemberId { get; set; }

    public string MemberCodeNumeric { get; set; }

    public string MemberCodeAlpha { get; set; }

    public string MemberCommercialName { get; set; }

    public string MembeLegalName { get; set; }

    public int ActionId { get; set; }

    public string TableName { get; set; }

    public string Group { get; set; }

    public string Element { get; set; }

    public string Action { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public string ChangedBy { get; set; }

    public DateTime ChangedOn { get; set; }

    public DateTime ChangeEffectiveDate { get; set; }

    public DateTime PeriodDatetime { get; set; }

    public string MemberPrefix { get; set; }

    public string MemberDesignator { get; set; }

    public string MemberName { get; set; }

    public string AdditionalInfo { get; set; }

    public Byte[] Logo { get; set; }

    public int ShowChangedBy { get; set; }

    public string ChangeEffectivePeriod { get; set; }

  }
}
