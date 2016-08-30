using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MiscUatp.Impl
{
  public class UatpConfigurationRepository : Repository<UatpConfiguration>, IUatpConfigurationRepository
  {
    public override IQueryable<UatpConfiguration> Get(System.Linq.Expressions.Expression<System.Func<UatpConfiguration, bool>> where)
    {
      var uatpConfigurations = EntityObjectSet.Include("Member").Where(where);

      return uatpConfigurations;
    }
  }
}
