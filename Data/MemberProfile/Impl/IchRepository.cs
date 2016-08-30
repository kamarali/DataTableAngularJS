using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class IchRepository : Repository<IchConfiguration>, IIchRepository
  {
    public override IQueryable<IchConfiguration> Get(System.Linq.Expressions.Expression<Func<IchConfiguration, bool>> where)
    {
      var ichList = EntityObjectSet.Include("Member").Where(where);

      return ichList;
    }

    public override IchConfiguration Single(System.Linq.Expressions.Expression<Func<IchConfiguration, bool>> where)
    {
      var ichList = EntityObjectSet.Include("Member").SingleOrDefault(where);

      return ichList;
    }

    public override IQueryable<IchConfiguration> GetAll()
    {
      var ichList = EntityObjectSet.Include("Member");

      return ichList;
    }
  }
}
