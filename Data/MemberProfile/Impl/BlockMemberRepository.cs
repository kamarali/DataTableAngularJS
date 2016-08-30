using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
    public class BlockMemberRepository : Repository<BlockMember>, IBlockMemberRepository
    {
        public override IQueryable<BlockMember> Get(System.Linq.Expressions.Expression<Func<BlockMember, bool>> where)
        {
            var blockMemberList = EntityObjectSet.Include("Member").Where(where);

            return blockMemberList;
        }
    }
}
