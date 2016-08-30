using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.Common.Impl
{
    public class MinAcceptableAmountRepository : Repository<MinAcceptableAmount>, IMinAcceptableAmountRepository
    {
        public IQueryable<MinAcceptableAmount> GetAllMinAcceptableAmounts()
        {
            var minAcceptableAmountList = EntityObjectSet.Include("TransactionType");
            return minAcceptableAmountList;
        }

    }
}
