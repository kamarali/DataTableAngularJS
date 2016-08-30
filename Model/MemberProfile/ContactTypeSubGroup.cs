using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class ContactTypeSubGroup : EntityBase<int>
  {
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public int GroupId { get; set; }

    public ContactTypeGroup ContactTypeGroup { get; set; }
  }
}
