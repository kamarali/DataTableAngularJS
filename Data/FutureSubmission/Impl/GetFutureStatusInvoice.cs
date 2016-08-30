using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.FutureSubmission;

namespace Iata.IS.Data.FutureSubmission.Impl
{
    public class GetFutureStatusInvoice : Repository<InvoiceBase> , IGetFutureStatusInvoice
    {
        private const string billing_year = "BILLING_YEAR_I";
        private const string billing_month = "BILLING_MONTH_I";
        private const string billing_period = "BILLING_PERIOD_I";
        private const string getInvoices = "GetFutureSubmittedInvoices";

        private const string Invoice_Id = "INVOICE_ID_I";
        private const string updateFutureSubmittedInvoice = "UpdateFutureSubmittedInvoice";
        private const string deleteErrorCorrectableInvoices = "DeleteErrorCorrectableInvoices";

        public List<FutureSubmissionInvoice> GetFutureStatusInvoices(int billingYear, int billingMonth, int period)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(billing_year, billingYear);
            parameters[1] = new ObjectParameter(billing_month, billingMonth);
            parameters[2] = new ObjectParameter(billing_period, period);

            var list = ExecuteStoredFunction<FutureSubmissionInvoice>(getInvoices, parameters) as IEnumerable<FutureSubmissionInvoice>;

            return list.ToList();
        }

        public int UpdateFutureSubmittedInvoice(Guid invoiceId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(Invoice_Id, invoiceId);
            parameters[1] = new ObjectParameter("R_RESULT_O", typeof(int));

            ExecuteStoredProcedure(updateFutureSubmittedInvoice, parameters);
            return Convert.ToInt32(parameters[1].Value);
        }

        public int DeleteErrorCorrectableInvoices(Guid invoiceId)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(Invoice_Id, invoiceId);
            parameters[1] = new ObjectParameter("R_RESULT_O", typeof(int));

            ExecuteStoredProcedure(deleteErrorCorrectableInvoices, parameters);
            return Convert.ToInt32(parameters[1].Value);
        }
    }
}
