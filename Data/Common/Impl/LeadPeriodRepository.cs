using System.Linq;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;


namespace Iata.IS.Data.Common.Impl
{
  public class LeadPeriodRepository : Repository<LeadPeriod>, ILeadPeriodRepository
  {
    /// <summary>
    /// Gets all time limits.
    /// </summary>
    /// <returns></returns>
    public IQueryable<LeadPeriod> GetAllLeadPeriods()
    {
      return EntityObjectSet;
    }
  }
}
