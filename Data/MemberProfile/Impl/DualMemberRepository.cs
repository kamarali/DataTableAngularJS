using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class DualMemberRepository:Repository<Member>, IDualMemberRepository
  {
    public List<Member> GetDualMemberList()
    {

      var membersList = ExecuteStoredFunction<Member>("getDualMemberList");

      return membersList.ToList();

    }
  }
}
