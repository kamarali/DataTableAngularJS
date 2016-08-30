using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.PayablesReport;

namespace Iata.IS.Data.Reports.PayablesReport
{
    public interface IPayablesReportData : IRepository<InvoiceBase>
    {
        //SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'   
        //remove Submission Method Field
        // CMP523 - Source Code in RMBMCM Summary Report
        List<PayablesReportModel> GetPayablesReportDetails(int loginMemberId, int billingMonth, int billingYear, int billingPeriod, int settlementMethod, int MemoType, string invoiceNo, int billingEntityCode, string RMBMCMNo, string sourceCode);

        List<RMBMCMSummaryCargoReceivableReport> GetRmBmCmSummaryPayblesReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod, int MemoType, int submissionMethod, int billingEntityId, int billedEntityCode, string invoiceNo, string RMBMCMNo, int billingType);
    }
}
