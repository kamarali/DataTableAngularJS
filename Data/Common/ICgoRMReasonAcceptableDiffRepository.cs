using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;
namespace Iata.IS.Data.Common
{
    public interface ICgoRMReasonAcceptableDiffRepository : IRepository<CgoRMReasonAcceptableDiff>
    {
        IQueryable<CgoRMReasonAcceptableDiff> GetAllCgoRMReasonAcceptableDiffs();

        CgoRMReasonAcceptableDiff GetCgoRMReasonAcceptableDiffDetail(int reasonCodeId);
    }
}