using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using System.Data.Objects;

namespace Iata.IS.Data.Reports.PendingInvoicesInErrorReport.Impl
{
    public class PendingInvoicesInErrorReportDataImpl : Repository<InvoiceBase>, IPendingInvoicesInErrorReportData
    {

        public List<Model.Reports.PendingInvoicesInErrorReport> GetPendingInvoicesInErrorReport(int memberId,int billingMonth, int billingYear, int billingPeriod, int billingCategory, int settlementMethod, int errorType, int isTotalsRequired)
        {
            var parameters = new ObjectParameter[8];

            parameters[0] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.memberId, memberId);
            parameters[1] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.billingMonth , billingMonth);
            parameters[2] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.billingYear , billingYear);
            parameters[3] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.periodNo , billingPeriod);
            parameters[4] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.billingCategory , billingCategory);
            parameters[5] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.settlementMethodId, settlementMethod);
            parameters[6] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.errorType, errorType);
            parameters[7] = new ObjectParameter(PendingInvoicesInErrorReportDataImplConstants.isTotalsRequired , isTotalsRequired);

            var list = ExecuteStoredFunction<Iata.IS.Model.Reports.PendingInvoicesInErrorReport>(PendingInvoicesInErrorReportDataImplConstants.GetPendigInvoicesInErrorReport, parameters);
            return list.ToList();
        }
    }
}
