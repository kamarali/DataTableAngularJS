﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Common
{
    public interface IMinMaxAcceptableAmountRepository : IRepository<MinMaxAcceptableAmount>
    {
        IQueryable<MinMaxAcceptableAmount> GetAllMinMaxAcceptableAmounts();
    }
}
