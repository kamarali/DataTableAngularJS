using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    public class CgoRMReasonAcceptableDiffRepository : Repository<CgoRMReasonAcceptableDiff>, ICgoRMReasonAcceptableDiffRepository
    {
        public IQueryable<CgoRMReasonAcceptableDiff> GetAllCgoRMReasonAcceptableDiffs()
        {
            var CgoRMReasonAcceptableDiffList = EntityObjectSet.Include("ReasonCode");
            return CgoRMReasonAcceptableDiffList;
        }

        public CgoRMReasonAcceptableDiff GetCgoRMReasonAcceptableDiffDetail(int reasonCodeId)
        {
            var CgoRMReasonAcceptableDiffList = EntityObjectSet.Include("ReasonCode").Where(reasonCode => reasonCode.Id == reasonCodeId).FirstOrDefault(); 
            return CgoRMReasonAcceptableDiffList;
        }
        
    }
}
