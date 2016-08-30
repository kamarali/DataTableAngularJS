using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;
using System.Linq;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoConfigurationRepository : Repository<CargoConfiguration>, ICargoConfigurationRepository
  {
    public override IQueryable<CargoConfiguration> Get(System.Linq.Expressions.Expression<System.Func<CargoConfiguration, bool>> where)
    {
      var cargoConfigurations = EntityObjectSet.Include("Member").Where(where);

      return cargoConfigurations;
    }
  }
}
