using System.Collections.Generic;
using System.Xml.Serialization;

namespace Iata.IS.Model.MemberProfile
{
  public class MemberProfileUpdate
  {
    //CMP #625: New Fields in ICH Member Profile Update XML
    public int SISMemberID { get; set; }
    public string NumericMemberCode { get; set; }
    public string AlphaMemberCode { get; set; }
     
   // CMP#597: SIS to generate Weekly reference Data Update and Contact CSV
    public string MemberNameCurrentValue { get; set; }
    public string MemberNameFutureValue { get; set; }
    public string MemberNameChangePeriodFrom { get; set; }

    public string Comments { get; set; }
    public bool? UATPInvoiceHandledByATCANCurrentValue { get; set; }
    public bool? UATPInvoiceHandledByATCANFutureValue { get; set; }
    public string UATPInvoiceHandledByATCANPeriodFrom { get; set; }
    public string MemberStatusCurrentValue { get; set; }
    public string MemberStatusFutureValue { get; set; }
    //CMP-689-Flexible CH Activation Options
    public string MemberStatusPeriodFrom { get; set; }
    public string SuspensionPeriodFrom { get; set; }
    public string TerminationPeriodFrom { get; set; }
    public string ReinstatementPeriodFrom { get; set; }
    public string ZoneCurrentValue { get; set; }
    public string ZoneFutureValue { get; set; }
    public string ZoneChangePeriodFrom { get; set; }
    public string CategoryCurrentValue { get; set; }
    public string CategoryFutureValue { get; set; }
    public string CategoryChangePeriodFrom { get; set; }
    public string EntryDate { get; set; }
    public string TerminationDate { get; set; }
    public bool EarlyCallDay { get; set; }
    public string ICHComments { get; set; }
    public int ICHWebReportOptions { get; set; }
    public bool CanSubmitPAXWebF12FilesCurrentValue { get; set; }
    public bool? CanSubmitPAXWebF12FilesFutureValue { get; set; }
    public string CanSubmitPAXChangePeriodFrom { get; set; }
    public bool CanSubmitCGOWebF12FilesCurrentValue { get; set; }
    public bool? CanSubmitCGOWebF12FilesFutureValue { get; set; }
    public string CanSubmitCGOChangePeriodFrom { get; set; }
    public bool CanSubmitMISCWebF12FilesCurrentValue { get; set; }
    public bool? CanSubmitMISCWebF12FilesFutureValue { get; set; }
    public string CanSubmitMISCChangePeriodFrom { get; set; }
    public bool CanSubmitUATPWebF12FilesCurrentValue { get; set; }
    public bool? CanSubmitUATPWebF12FilesFutureValue { get; set; }
    public string CanSubmitUATPChangePeriodFrom { get; set; }
    //CMP #625: New Fields in ICH Member Profile Update XML
    [XmlElement(ElementName = "iiNetAccount")]
    public List<IInetAccountId> IInetAccountIds { get; set; }
    public List<Contact> Contacts { get; set; }
    public bool AggregatorCurrentValue { get; set; }
    public bool? AggregatorFutureValue { get; set; }
    public string AggregatorPeriodFrom { get; set; }
    public int? AggregatedTypeCurrentValue { get; set; }
    public int? AggregatedTypeFutureValue { get; set; }
    public string AggregatedTypePeriodFrom { get; set; }
    public List<Member> AggregatedMembersCurrentValue { get; set; }
    public List<Member> AggregatedMembersFutureValue { get; set; }
    public string AggregatedMembersPeriodFrom { get; set; }
    public List<Member> SponsoredMembersCurrentValue { get; set; }
    public List<Member> SponsoredMembersFutureValue { get; set; }
    public string SponsoredMembersPeriodFrom { get; set; }
    public string SponsororPeriodFrom { get; set; }
    public ParentMemberDetail ParentMemberCurrentValue { get; set; }
    public ParentMemberDetail ParentMemberFutureValue { get; set; }
  }
  public partial class ParentMemberDetail
  {
      public bool? IsMerged { get; set; }
      public string MergerEffectivePeriod { get; set; }
      public string ParentMemberCode { get; set; }
      public string PeriodFrom { get; set; }
  }

  /// <summary>
  /// This class is used for set account id.
  /// </summary>
  //CMP #625: New Fields in ICH Member Profile Update XML
  public class IInetAccountId
  {
    [XmlAttribute("Name")]
    public string Type { get; set; }

    [XmlText]
    public string Value { get; set; }
  }
}
