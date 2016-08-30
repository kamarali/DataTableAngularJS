using System.ComponentModel;


namespace Iata.IS.Model.MemberProfile
{
  public class DownloadBlockingRules
  {
    [DisplayName("Clearing House")]
    public string ClearingHouse { get; set; }

    [DisplayName("Member Code")]
    public string MemberCode { get; set; }

    [DisplayName("Blocking Rule")]
    public string BlockingRule { get; set; }

    [DisplayName("Description")]
    public string Description { get; set; }

    [DisplayName("Creation Date/Time (EST/EDT)")]
    public string CreationDateTime { get; set; }

    [DisplayName("Block Type")]
    public string BlockType { get; set; }

    [DisplayName("Zone(s)")]
    public string Zones { get; set; }

    [DisplayName("Zone Exception(s)")]
    public string ZoneExceptions { get; set; }

    [DisplayName("Member")]
    public string BlockedMember { get; set; }

    [DisplayName("PAX")]
    public string PaxBillingsBlocked { get; set; }

    [DisplayName("CGO")]
    public string CgoBillingsBlocked { get; set; }

    [DisplayName("UATP")]
    public string UatpBillingsBlocked { get; set; }

    [DisplayName("MISC")]
    public string MiscBillingsBlocked { get; set; }

  }
}
