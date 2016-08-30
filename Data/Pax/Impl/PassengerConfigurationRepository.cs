using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.Pax.Impl
{
  public class PassengerConfigurationRepository : Repository<PassengerConfiguration>, IPassengerConfigurationRepository
  {
    public override IQueryable<PassengerConfiguration> Get(System.Linq.Expressions.Expression<System.Func<PassengerConfiguration, bool>> where)
    {
      var passengerConfigurations = EntityObjectSet.Include("Member").Where(where);

      return passengerConfigurations; 
    }
  }
}
