using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
  public interface IDualMemberRepository : IRepository<Member>
  {
    /// <summary>
    /// To get dual member List.
    /// </summary>
    /// <returns>List of member records.</returns>
    List<Member> GetDualMemberList();
  }
}
