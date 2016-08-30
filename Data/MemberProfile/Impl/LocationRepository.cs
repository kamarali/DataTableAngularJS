using System;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class LocationRepository : Repository<Location>, ILocationRepository
  {
    public override IQueryable<Location> Get(System.Linq.Expressions.Expression<Func<Location, bool>> where)
    {
      // TODO: Verify if including country is needed.
      var locationList = EntityObjectSet.Include("Currency").Include("Country").Where(where);

      return locationList;
    }

    public override Location Single(System.Linq.Expressions.Expression<Func<Location, bool>> where)
    {
      var location = EntityObjectSet.Include("Currency").Include("Country").SingleOrDefault(where);

      return location;
    }

    public void UpdateLocationInfo(Guid invoiceId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(LocationRepositoryConstants.InvoiceIdParameterName, typeof(Guid)) { Value = invoiceId };

      ExecuteStoredProcedure(LocationRepositoryConstants.UpdateLocationInfoFunctionName, parameters);
    }
  }
}
