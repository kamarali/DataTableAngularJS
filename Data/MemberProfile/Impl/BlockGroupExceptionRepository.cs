using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
    public class BlockGroupExceptionRepository : Repository<BlockGroupException>, IBlockGroupExceptionRepository
    {
        public override IQueryable<BlockGroupException> Get(System.Linq.Expressions.Expression<Func<BlockGroupException, bool>> where)
        {
            var blockMemberList = EntityObjectSet.Include("ExceptionMember").Where(where);

            return blockMemberList;
        }
    }
}
