using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class ContactTypeGroup : EntityBase<int>
  {
    public int GroupId { get; set; }

    public string GroupName { get; set; }

    public bool IsActive { get; set; }

    //last_updated_by
    //last_updated_on

  }
}
