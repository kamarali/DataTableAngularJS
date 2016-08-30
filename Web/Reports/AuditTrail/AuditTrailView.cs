using System;
namespace Iata.IS.Web.Reports.AuditTrail
{
  public class AuditTrailView
  {
    public int AuditId { get; set; }

    public string Group { get; set; }

    public string Element { get; set; }

    public string Action { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public string ChangedBy { get; set; }

    public DateTime ChangedOn { get; set; }

    public DateTime ChangeEffectiveDate { get; set; }

    public DateTime PeriodDatetime { get; set; }

    public string ChangeEffectivePeriod { get; set; }

    public string MemberPrefix { get; set; }

    public string MemberDesignator { get; set; }

    public string MemberName { get; set; }

    public string AdditionalInfo { get; set; }

    public Byte[] Logo { get; set; }

    public int ShowChangedBy { get; set; }

  }
}
