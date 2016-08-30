using Iata.IS.Model.Base;

namespace Iata.IS.Model.MemberProfile
{
  public class ContactTypeGroup : EntityBase<int>
  {
    public string Name { get; set; }

    public bool IsActive { get; set; }

  }
}
