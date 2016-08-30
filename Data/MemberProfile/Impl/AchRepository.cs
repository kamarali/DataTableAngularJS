using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
    public class AchRepository : Repository<AchConfiguration>, IAchRepository
    {
        public override IQueryable<AchConfiguration> Get(System.Linq.Expressions.Expression<Func<AchConfiguration, bool>> where)
        {
            var achList = EntityObjectSet.Include("Member").Where(where);

            return achList;
        }

        public override AchConfiguration Single(System.Linq.Expressions.Expression<Func<AchConfiguration, bool>> where)
        {
            var achList = EntityObjectSet.Include("Member").SingleOrDefault(where);

            return achList;
        }

        public override IQueryable<AchConfiguration> GetAll()
        {
            var achList = EntityObjectSet.Include("Member");

            return achList;
        }
    }
}
