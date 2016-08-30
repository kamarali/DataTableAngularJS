using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Reports.ReceivablesReport.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.PayablesReport;

namespace Iata.IS.Data.Reports.PayablesReport.Impl
{
    public class PayablesReportDataImpl : Repository<InvoiceBase>, IPayablesReportData
    {

        //SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'   
        //remove Submission Method Field
        public List<PayablesReportModel> GetPayablesReportDetails(int loginMemberId, int billingMonth, int billingYear, int billingPeriod, int settlementMethod, int MemoType, string invoiceNo, int billingEntityCode, string RMBMCMNo, string sourceCode)
        {
            var parameters = new ObjectParameter[10];
            parameters[0] = new ObjectParameter(PayablesReportDataImplConstants.MemberId, loginMemberId);
            parameters[1] = new ObjectParameter(PayablesReportDataImplConstants.BillingMonth, billingMonth);
            parameters[2] = new ObjectParameter(PayablesReportDataImplConstants.BillingYear, billingYear);
            parameters[3] = new ObjectParameter(PayablesReportDataImplConstants.BillingPeriod, billingPeriod);
            parameters[4] = new ObjectParameter(PayablesReportDataImplConstants.SettlementMethodId, settlementMethod);
            parameters[5] = new ObjectParameter(PayablesReportDataImplConstants.MemoType, MemoType);
            parameters[6] = new ObjectParameter(PayablesReportDataImplConstants.InvoiceNumber, invoiceNo);
            parameters[7] = new ObjectParameter(PayablesReportDataImplConstants.BillingEntityCode, billingEntityCode);
            parameters[8] = new ObjectParameter(PayablesReportDataImplConstants.RMBMCMNO, RMBMCMNo);
            // CMP523 - Source Code in RMBMCM Summary Report
            parameters[9] = new ObjectParameter(PayablesReportDataImplConstants.SourceCode, sourceCode);
            var list =
                ExecuteStoredFunction<PayablesReportModel>(PayablesReportDataImplConstants.PaxPayablesReport, parameters);
            return list.ToList();
        }


        public List<RMBMCMSummaryCargoReceivableReport> GetRmBmCmSummaryPayblesReport(int billingYear, int billingMonth, int billingPeriod, int settlementMethod, int memoType, int submissionMethod, int billingEntityId, int billedEntityCode, string invoiceNo, string RMBMCMNo, int billingType)
        {
            var parameters = new ObjectParameter[11];

            parameters[0] = new ObjectParameter(PayablesReportDataImplConstants.billngEntityCode_sum, billingEntityId);
            parameters[1] = new ObjectParameter(PayablesReportDataImplConstants.billingMonth_sum, billingMonth);
            parameters[2] = new ObjectParameter(PayablesReportDataImplConstants.billingYear_sum, billingYear);
            parameters[3] = new ObjectParameter(PayablesReportDataImplConstants.periodNo_sum, billingPeriod);
            parameters[4] = new ObjectParameter(PayablesReportDataImplConstants.settlementMethod_sum, settlementMethod);
            parameters[5] = new ObjectParameter(PayablesReportDataImplConstants.memoType_sum, memoType);
            parameters[6] = new ObjectParameter(PayablesReportDataImplConstants.dataSource_sum, submissionMethod);
            parameters[7] = new ObjectParameter(PayablesReportDataImplConstants.invoiceNumber_sum, invoiceNo);
            parameters[8] = new ObjectParameter(PayablesReportDataImplConstants.airlineCode_sum, billedEntityCode);
            parameters[9] = new ObjectParameter(PayablesReportDataImplConstants.rMBMCMNumber_sum, RMBMCMNo);
            parameters[10] = new ObjectParameter(PayablesReportDataImplConstants.billingType, billingType);


            var list =
                ExecuteStoredFunction<RMBMCMSummaryCargoReceivableReport>(PayablesReportDataImplConstants.GetRmBmCmSummaryPayblesReport, parameters);
            return list.ToList(); 
            
           
        }
    }
}

