using System;
using Iata.IS.Model.MemberProfile.Enums;

namespace Iata.IS.Model.MemberProfile
{
  public class AuditAttribute : Attribute
  {
    public UpdateFlavor UpdateFlavor { get; set; }

    public ElementGroupType ElementGroup { get; set; }

    public string ElementGroupDisplayName { get; set; }

    public string ElementTable { get; set; }

    public string ElementName { get; set; }

    public bool IncludeDisplayNames { get; set; }

    public bool IncludeRelationId { get; set; }

    public bool MaskValues { get; set; }

    public int IgnoreValue { get; set; }

    public AuditAttribute()
    {
      IgnoreValue = int.MinValue;
    }
  }
}