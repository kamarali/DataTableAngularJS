using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class MinMaxAcceptableAmountRepository : Repository<MinMaxAcceptableAmount>, IMinMaxAcceptableAmountRepository
    {
        public IQueryable<MinMaxAcceptableAmount> GetAllMinMaxAcceptableAmounts()
        {
            var minMaxAcceptableAmountList = EntityObjectSet.Include("TransactionType");

            return minMaxAcceptableAmountList;
        }

    }
}
