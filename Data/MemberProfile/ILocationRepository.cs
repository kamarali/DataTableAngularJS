using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
    public interface ILocationRepository : IRepository<Location>
    {
      void UpdateLocationInfo(Guid invoiceId);
    }
}
