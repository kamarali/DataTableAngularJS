using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Data.Common
{
    public interface IReasonCodeRepository : IRepository<ReasonCode>
    {
        IQueryable<ReasonCode> GetAllReasonCodes();

     }
}
