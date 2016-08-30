using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;

namespace Iata.IS.Data.ExternalInterfaces.IATAInterface.Impl
{
  public class IATAInterfaceRepository : Repository<InvoiceBase>, IIATAInterfaceRepository
  {
    public List<RechargeData> GetRechargeData(DateTime startDate, DateTime endDate, int memberId) // , Model.Calendar.BillingPeriod secondPeriod, Model.Calendar.BillingPeriod thirdPeriod, Model.Calendar.BillingPeriod fourthPeriod
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(IATAInterfaceRepositoryConstants.StartDate,startDate);
      parameters[1] = new ObjectParameter(IATAInterfaceRepositoryConstants.EndDate, endDate);
      parameters[2] = new ObjectParameter(IATAInterfaceRepositoryConstants.MemberId, memberId);
      parameters[3] = new ObjectParameter(IATAInterfaceRepositoryConstants.PartcipantId, 0);
      parameters[4] = new ObjectParameter(IATAInterfaceRepositoryConstants.IsReportData, 0);
      
      var list = ExecuteStoredFunction<RechargeData>(IATAInterfaceRepositoryConstants.GetRechargeData, parameters) as IEnumerable<RechargeData>;

      return list.ToList();

      throw new NotImplementedException();
    }
  }
}
