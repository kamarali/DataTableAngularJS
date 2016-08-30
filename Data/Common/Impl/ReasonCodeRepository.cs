using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
//using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class ReasonCodeRepository : Repository<ReasonCode>, IReasonCodeRepository
    {
        public IQueryable<ReasonCode> GetAllReasonCodes()
        {
            var ReasonCodesList = EntityObjectSet.Include("TransactionType");
            return ReasonCodesList;
        }
        
    }
}
