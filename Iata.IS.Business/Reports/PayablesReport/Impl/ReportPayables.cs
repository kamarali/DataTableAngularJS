using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.PayablesReport;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.PayablesReport;

namespace Iata.IS.Business.Reports.PayablesReport.Impl
{
    public class ReportPayables : IPayablesReport
    {

        public  IPayablesReportData IpayablesreportdataProp { get; set; }


        public ReportPayables(IPayablesReportData Ipayablesreportdata)
        {
            IpayablesreportdataProp = Ipayablesreportdata;
        }

        //SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'   
        //remove Submission Method Field
        // CMP523 - Source Code in RMBMCM Summary Report
        public List<PayablesReportModel> GetPayablesReportDetails(int loginMemberId, int billingMonth, int billingYear, int billingPeriod, int settlementMethod, int MemoType, string invoiceNo, int billingEntityCode, string RMBMCMNo, string sourceCode)
        {
            return IpayablesreportdataProp.GetPayablesReportDetails(loginMemberId, billingMonth, billingYear, billingPeriod,
                                                               settlementMethod,MemoType,  invoiceNo,
                                                               billingEntityCode, RMBMCMNo, sourceCode);
        }



        public List<RMBMCMSummaryCargoReceivableReport> GetRMBMCMSummaryPayblesReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod, int memoType, int submissionMethod, int billingEntityId, int billedEntityCode, string invoiceNo, string rmbmcmNo, int billingType)
        {
            return IpayablesreportdataProp.GetRmBmCmSummaryPayblesReport(billingYear, billingMonth, billingPeriod, settlementMethod, memoType, submissionMethod, billingEntityId, billedEntityCode, invoiceNo, rmbmcmNo, billingType);
        }
    }
}
