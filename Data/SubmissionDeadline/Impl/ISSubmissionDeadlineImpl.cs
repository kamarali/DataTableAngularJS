using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.SubmissionDeadline;

namespace Iata.IS.Data.SubmissionDeadline.Impl 
{
    public class ISSubmissionDeadlineImpl : Repository<InvoiceBase>, ISSubmissionDeadline
    {
        /// <summary>
        /// This method is used to get All the pending Invoices
        /// </summary>
        /// <param name="billingMonth">Billing Month</param>
        /// <param name="billingYear">Billing Year</param>
        /// <param name="billingPeriod">Billing Period</param>
        /// <returns>All pending Invoices</returns>
        public List<SubmissionData>GetPendingInvoices(int billingMonth, int billingYear, int billingPeriod)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(ISSubmissionDeadlineConstants.BillingMonth, billingMonth);
            parameters[1] = new ObjectParameter(ISSubmissionDeadlineConstants.BillingYear, billingYear);
            parameters[2] = new ObjectParameter(ISSubmissionDeadlineConstants.BillingPeriod, billingPeriod);

            var list = ExecuteStoredFunction<SubmissionData>(ISSubmissionDeadlineConstants.GetPendinginvoices, parameters) as IEnumerable<SubmissionData>;

            return list.ToList();
        }// End GetPendingInvoices
    }// end ISSubmissionDeadlineImpl class
}// End Namespace
