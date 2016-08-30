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
    public interface IChargeCodeTypeRepository : IRepository<ChargeCodeType>
    {
      /// <summary>
      /// This function is used to get misc charge code type based on criteria.
      /// </summary>
      /// <param name="ChargeCategoryId"></param>
      /// <param name="ChargeCodeId"></param>
      /// <param name="chargeCodeTypeName"></param>
      /// <returns></returns>
      //CMP #636: Standard Update Mobilization.
      List<ChargeCodeTypeSearchData> GetMiscChargeCodeType(int chargeCategoryId, int chargeCodeId, string chargeCodeTypeName);
    }
}