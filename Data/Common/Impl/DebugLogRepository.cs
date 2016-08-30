using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Data.Impl;
using System.Data.Objects;
using Devart.Data.Oracle;
//using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Common.Impl
{
    class DebugLogRepository : Repository<DebugLog>, IDebugLogRepository
    {
        public IQueryable<DebugLog> GetAllLogs()
    {
        return EntityObjectSet;
    }
    }
}
