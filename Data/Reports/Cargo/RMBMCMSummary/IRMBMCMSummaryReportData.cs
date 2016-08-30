using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.Cargo;

namespace Iata.IS.Data.Reports.Cargo.RMBMCMSummary
{
    public interface IRMBMCMSummaryReportData : IRepository<InvoiceBase>
    {
        List<RMBMCMSummaryCargoReceivableReport> GetRmBmCmSummaryReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod,
                                        int MemoType, int submissionMethod, int billingEntityId, int billedEntityCode,
                                        string invoiceNo, string RMBMCMNo, int billingType);
        
    }
}