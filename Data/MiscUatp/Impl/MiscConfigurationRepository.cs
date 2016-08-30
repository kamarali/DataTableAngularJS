using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class MiscConfigurationRepository : Repository<MiscellaneousConfiguration>,IMiscConfigurationRepository
  {
    public override IQueryable<MiscellaneousConfiguration> Get(System.Linq.Expressions.Expression<System.Func<MiscellaneousConfiguration, bool>> where)
    {
      var miscConfigurations = EntityObjectSet.Include("Member").Where(where);

      return miscConfigurations;
    }
  }
}
