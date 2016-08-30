using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.ExternalInterfaces.ICHInterface;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ExternalInterfaces.ICHInterface;

namespace Iata.IS.Data.ExternalInterfaces.ICHInterface.Impl
{
  public class ICHInterfaceRepository : Repository<InvoiceBase>, IICHInterfaceRepository
  {

    public List<ICHSettlementData> GetICHSettlementData(BillingPeriod curBillingPeriod, BillingPeriod prevBillingPeriod, int maxRecordCount)
    {


      var parameters = new ObjectParameter[7];
      parameters[0] = new ObjectParameter(ICHInterfaceRepositoryConstants.CurrentBillingMonth, curBillingPeriod.Month);
      parameters[1] = new ObjectParameter(ICHInterfaceRepositoryConstants.CurrentBillingYear, curBillingPeriod.Year);
      parameters[2] = new ObjectParameter(ICHInterfaceRepositoryConstants.CurrentBillingPeriod, curBillingPeriod.Period);
      parameters[3] = new ObjectParameter(ICHInterfaceRepositoryConstants.PreviousBillingMonth, prevBillingPeriod.Month);
      parameters[4] = new ObjectParameter(ICHInterfaceRepositoryConstants.PreviousBillingYear, prevBillingPeriod.Year);
      parameters[5] = new ObjectParameter(ICHInterfaceRepositoryConstants.PreviousBillingPeriod, prevBillingPeriod.Period);
      parameters[6] = new ObjectParameter(ICHInterfaceRepositoryConstants.MaxRecordCount, maxRecordCount);

      var list = ExecuteStoredFunction<ICHSettlementData>(ICHInterfaceRepositoryConstants.GetICHSettlementData, parameters) as IEnumerable<ICHSettlementData>;

      return list.ToList();

    }

    public List<CrossCheckRequestInvoice> GetICHCrossCheckRequestData(BillingPeriod curBillingPeriod, DateTime startDateTime, DateTime endDateTime)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(ICHInterfaceRepositoryConstants.BillingMonth, typeof(int)) { Value = curBillingPeriod.Month };
      parameters[1] = new ObjectParameter(ICHInterfaceRepositoryConstants.BillingYear, typeof(int)) { Value = curBillingPeriod.Year };
      parameters[2] = new ObjectParameter(ICHInterfaceRepositoryConstants.BillingPeriod, typeof(int)) { Value = curBillingPeriod.Period };
      parameters[3] = new ObjectParameter(ICHInterfaceRepositoryConstants.StartDateTime, typeof(DateTime)) { Value = startDateTime };
      parameters[4] = new ObjectParameter(ICHInterfaceRepositoryConstants.EndDateTime, typeof(DateTime)) { Value = endDateTime };
      return ExecuteStoredFunction<CrossCheckRequestInvoice>(ICHInterfaceRepositoryConstants.GetICHCrossCheckData, parameters).ToList();

    }

    public List<ICHSettlementData> GetICHSettlementDataForResend(string invoiceIdList)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(ICHInterfaceRepositoryConstants.invoiceIdList, typeof(int)) { Value = invoiceIdList };
      return ExecuteStoredFunction<ICHSettlementData>(ICHInterfaceRepositoryConstants.GetICHSettlementDataForResend, parameters).ToList();


    }


    public void UpdateInvoiceStatusAfterSettlement(string invoiceIds)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(ICHInterfaceRepositoryConstants.InvoiceIds, invoiceIds);
    //  parameters[1] = new ObjectParameter(ICHInterfaceRepositoryConstants.ICHSubmissionDate, ichSubmissionDate);
      ExecuteStoredProcedure(ICHInterfaceRepositoryConstants.UpdateInvoiceStatusAfterSettlement, parameters);
    }

    public void UpdateInvoiceStatusAfterFailedSettlement(string invoiceIds)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(ICHInterfaceRepositoryConstants.InvoiceIds, invoiceIds);
        //  parameters[1] = new ObjectParameter(ICHInterfaceRepositoryConstants.ICHSubmissionDate, ichSubmissionDate);
        ExecuteStoredProcedure(ICHInterfaceRepositoryConstants.UpdateInvoiceStatusAfterFailedSettlement, parameters);
    }



    public void UpdateInvoiceStatusNullAfterFailedSettlement(string invoiceIds)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter(ICHInterfaceRepositoryConstants.InvoiceIds, invoiceIds);
        //  parameters[1] = new ObjectParameter(ICHInterfaceRepositoryConstants.ICHSubmissionDate, ichSubmissionDate);
        ExecuteStoredProcedure(ICHInterfaceRepositoryConstants.UpdateInvoiceStatusNullAfterFailedSettlement, parameters);
    }
  }
}
