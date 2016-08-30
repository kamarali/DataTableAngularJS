using System;

namespace Iata.IS.Model.MemberProfile
{
  public class AuditTrail
  {
    public string FromPeriod { get; set; }

    public int FromMonth { get; set; }

    public int FromYear { get; set; }

    public string ToPeriod { get; set; }

    public int ToMonth { get; set; }

    public int ToYear { get; set; }

    public string User { get; set; }

    public string AuditId { get; set; }

    public DateTime AuditEntryDate { get; set; }

    public string BilateralPartner { get; set; }

    public string Group { get; set; }

    public string Element { get; set; }

    public string ChangedBy { get; set; }

    public DateTime ChangedOn { get; set; }

    public string Action { get; set; }

    public string OldValue { get; set; }

    public string NewValue { get; set; }

    public DateTime ChangeEffectiveOnDate { get; set; }

    public int ChangeEffectiveOnPeriod { get; set; }

    public DateTime ChangeEffectiveDate { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string ElementList { get; set; }


  }
}