using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common.Impl
{
    public class MaxAcceptableAmountRepository : Repository<MaxAcceptableAmount>, IMaxAcceptableAmountRepository
    {
        public IQueryable<MaxAcceptableAmount> GetAllMaxAcceptableAmounts()
        {
            var maxAcceptableAmountList = EntityObjectSet.Include("TransactionType");

            return maxAcceptableAmountList;
        }

    }
}
