using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Master;

namespace Iata.IS.Data.MiscUatp
{
    public interface IChargeCodeRepository : IRepository<ChargeCode>
    {
      List<ChargeCodeSearchData> GetMiscChargeCode(Int32 chargeCategoryId, Int32 chargeCodeId);
    }
}
