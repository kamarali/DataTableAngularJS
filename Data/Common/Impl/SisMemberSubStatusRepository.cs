using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Pax;
using System.Data.Objects;
using Devart.Data.Oracle;
using Microsoft.Data.Extensions;


namespace Iata.IS.Data.Common.Impl
{
    public class SisMemberSubStatusRepository : Repository<SisMemberSubStatus>, ISisMemberSubStatusRepository
    {
        //public IQueryable<SisMemberSubStatus> GetAllSisMemberSubStatus()
        //{
        //    var MemberSubStatusList = EntityObjectSet.Include("SisMemberStatus");

        //    return MemberSubStatusList;
        //}
    }
}