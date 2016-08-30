using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class RMReasonAcceptableDiffRepository : Repository<RMReasonAcceptableDiff>, IRMReasonAcceptableDiffRepository
    {
        public IQueryable<RMReasonAcceptableDiff> GetAllRMReasonAcceptableDiffs()
        {
            var RMReasonAcceptableDiffsList = EntityObjectSet.Include("ReasonCode.TransactionType");
            return RMReasonAcceptableDiffsList;
        }
        public RMReasonAcceptableDiff GetRMReasonAcceptableDiffDetail(int reasonCodeId)
        {
            var RMReasonAcceptableDiff = EntityObjectSet.Include("ReasonCode").Where(rmcode => rmcode.Id == reasonCodeId).FirstOrDefault();
            return RMReasonAcceptableDiff;
        }

    }
}
