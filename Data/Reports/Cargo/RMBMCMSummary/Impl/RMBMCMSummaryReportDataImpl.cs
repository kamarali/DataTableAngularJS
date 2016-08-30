using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.Cargo;

namespace Iata.IS.Data.Reports.Cargo.RMBMCMSummary.Impl
{
    public class RMBMCMSummaryReportDataImpl : Repository<InvoiceBase>, IRMBMCMSummaryReportData
    {
        public List<RMBMCMSummaryCargoReceivableReport> GetRmBmCmSummaryReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod, int memoType, int submissionMethod, int billingEntityId, int billedEntityCode, string invoiceNo, string RMBMCMNo, int billingType)
        {
            var parameters = new ObjectParameter[11];

            parameters[0] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.billngEntityCode_sum, billingEntityId);
            parameters[1] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.billingMonth_sum, billingMonth);
            parameters[2] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.billingYear_sum, billingYear);
            parameters[3] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.periodNo_sum, billingPeriod);
            parameters[4] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.settlementMethod_sum, settlementMethod);
            parameters[5] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.memoType_sum, memoType);
            parameters[6] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.dataSource_sum, submissionMethod);
            parameters[7] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.invoiceNumber_sum, invoiceNo);
            parameters[8] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.airlineCode_sum, billedEntityCode);
            parameters[9] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.rMBMCMNumber_sum, RMBMCMNo);
            parameters[10] = new ObjectParameter(RMBMCMSummaryReportDataImplConstants.billingType, billingType);


            var list =
                ExecuteStoredFunction<RMBMCMSummaryCargoReceivableReport>(RMBMCMSummaryReportDataImplConstants.GetRmBmCmSummaryReport, parameters);
            return list.ToList();


        }
    }
}
