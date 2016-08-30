using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ExternalInterfaces.ICHInterface;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.ExternalInterfaces.ICHInterface
{
    public interface IICHInterfaceRepository
    {
        List<ICHSettlementData> GetICHSettlementData(BillingPeriod curBillingPeriod, BillingPeriod prevBillingPeriod, int maxRecordCount);

        //This method will call stored procedure 'PROC_GET_ICH_CROSS_CHECK_DATA' and get invoice list corresponding to billing period passed
        List<CrossCheckRequestInvoice> GetICHCrossCheckRequestData(BillingPeriod curBillingPeriod, DateTime startDateTime, DateTime endDateTime);

        //This method will call stored procedure 'PROC_SETTLEMENT_DATA_FORRESEND' and get invoice list corresponding to invoice id list passed
        List<ICHSettlementData> GetICHSettlementDataForResend(string invoiceIdList);

        void UpdateInvoiceStatusAfterSettlement(string invoiceIds);
        void UpdateInvoiceStatusAfterFailedSettlement(string invoiceIds);

        void UpdateInvoiceStatusNullAfterFailedSettlement(string invoiceIds);
    }
}
