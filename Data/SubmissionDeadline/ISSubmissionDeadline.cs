using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.SubmissionDeadline;

namespace Iata.IS.Data.SubmissionDeadline
{
    public interface ISSubmissionDeadline
    {
        List<SubmissionData> GetPendingInvoices(int billingMonth, int billingYear, int billingPeriod);
    }// End ISSubmissionDeadline
}// End namespace
