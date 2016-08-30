using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common
{
  public interface ILeadPeriodRepository : IRepository<LeadPeriod>
  {
    /// <summary>
    /// Gets all lead period.
    /// </summary>
    /// <returns></returns>
    IQueryable<LeadPeriod> GetAllLeadPeriods();
  }
}
