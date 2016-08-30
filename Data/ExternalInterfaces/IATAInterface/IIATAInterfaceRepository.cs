using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;

namespace Iata.IS.Data.ExternalInterfaces.IATAInterface
{
  public interface IIATAInterfaceRepository
  {
    List<RechargeData> GetRechargeData(DateTime startDate, DateTime endDate, int memberId); // , BillingPeriod secondPeriod, BillingPeriod thirdPeriod, BillingPeriod fourthPeriod
  }
}
