using System.Collections.Generic;
using Iata.IS.Model.Reports.Cargo;

namespace Iata.IS.Business.Reports.Cargo.RMBMCMSummary
{
    public interface IRMBMCMSummaryReport
    {
        List<RMBMCMSummaryCargoReceivableReport> GetRMBMCMSummaryReport(int billingYear, int billingMonth,
                                                                     int billingPeriod, int settlementMethod,
                                                                     int memoType, int submissionMethod,
                                                                     int billingEntityId, int billedEntityCode,
                                                                     string invoiceNo, string rmbmcmNo, int billingType);
    }
}