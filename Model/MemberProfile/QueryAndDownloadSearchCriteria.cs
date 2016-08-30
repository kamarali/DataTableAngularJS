
using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class QueryAndDownloadSearchCriteria
  {
    public string UserCategoryId { get; set; }

    public string ReportType { get; set; }

    public string MemberId { get; set; }

    public string CountryId { get; set; }

    public string ContactId { get; set; }

    public string MetaIdList { get; set; }

    public string TypeMetaIdList { get; set; }
    
    public string EmailId { get; set; }

    public string GroupIdList { get; set; }

    public string SubGroupIdList { get; set; }

    public string ContactTypeIdList { get; set; }

    public string PageNumber { get; set; }

    public string PageSize { get; set; }

    public string SortIds { get; set; }

    public string SortOrder { get; set; }

    public bool ISIch { get; set; }

    public bool ISAch { get; set; }

    public bool ISIata { get; set; }

    public bool ISDual { get; set; }

    public bool ISNonCh { get; set; }

    public bool ISOwnMember { get; set; }

  

  }
}
