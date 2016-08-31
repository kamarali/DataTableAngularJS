using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.Cargo.RMBMCMSummary;
using Iata.IS.Model.Reports.Cargo;

namespace Iata.IS.Business.Reports.Cargo.RMBMCMSummary.Impl
{
    public class RMBMCMSummaryReport : IRMBMCMSummaryReport
    {
        public IRMBMCMSummaryReportData IrmbmcmSummaryReportdataProp { get; set; }


        public RMBMCMSummaryReport(IRMBMCMSummaryReportData Irmbmcmreportdata)
        {
            IrmbmcmSummaryReportdataProp = Irmbmcmreportdata;
        }
        public List<RMBMCMSummaryCargoReceivableReport> GetRMBMCMSummaryReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod, int memoType, int submissionMethod, int billingEntityId, int billedEntityCode, string invoiceNo, string rmbmcmNo, int billingType)
        {
            return IrmbmcmSummaryReportdataProp.GetRmBmCmSummaryReport(billingYear, billingMonth, billingPeriod, settlementMethod, memoType, submissionMethod, billingEntityId, billedEntityCode, invoiceNo, rmbmcmNo, billingType);
        }
    }
}
