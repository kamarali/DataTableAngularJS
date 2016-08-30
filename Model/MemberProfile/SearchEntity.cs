using System;

namespace Iata.IS.Model.MemberProfile
{
  public class SearchEntity
  {

    public int MemberId { set; get; }

    // Used in AutoPopulate
    public string MemberText { set; get; }

    public string Designator { get; set; }

    public string Prefix { get; set; }

    public string CommercialName { get; set; }

    public string LegalName { get; set; }

    public int FromClearancePeriod { set; get; }

    public int FromClearanceYear { set; get; }

    public int FromClearanceMonth { set; get; }

    public int ToClearancePeriod { set; get; }

    public int ToClearanceYear { set; get; }

    public int ToClearanceMonth { set; get; }

    public DateTime FromDate { set; get; }

    public DateTime ToDate { set; get; }

    public string User { get; set; }


  }
}
