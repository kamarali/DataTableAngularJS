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
    public class TimeLimitRepository : Repository<TimeLimit>, ITimeLimitRepository
    {
        /// <summary>
        /// Gets all time limits.
        /// </summary>
        /// <returns></returns>
        public IQueryable<TimeLimit> GetAllTimeLimits()
        {
            var TimeLimitList = EntityObjectSet.Include("TransactionType").Include("SettlementMethod");
            return TimeLimitList;
        }
    }
}
