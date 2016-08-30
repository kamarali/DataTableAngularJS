using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Data.Common
{
    public interface IRMReasonAcceptableDiffRepository : IRepository<RMReasonAcceptableDiff>
    {
        IQueryable<RMReasonAcceptableDiff> GetAllRMReasonAcceptableDiffs();
        RMReasonAcceptableDiff GetRMReasonAcceptableDiffDetail(int reasonCodeId);
    }
}
