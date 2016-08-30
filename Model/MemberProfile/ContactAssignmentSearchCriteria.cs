using System;

namespace Iata.IS.Model.MemberProfile
{
  [Serializable]
  public class ContactAssignmentSearchCriteria
  {
    public string ContactTypeCategory { get; set; }

    public string GroupId { get; set; }

    public string SubGroupId { get; set; }

    public string TypeId { get; set; }

    public string Columns { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int MemberId { get; set; }
  }
}
