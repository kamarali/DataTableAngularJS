using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common
{
    public interface ITimeLimitRepository : IRepository<TimeLimit>
    {
        /// <summary>
        /// Gets all time limits.
        /// </summary>
        /// <returns></returns>
        IQueryable<TimeLimit> GetAllTimeLimits();
    }
}
