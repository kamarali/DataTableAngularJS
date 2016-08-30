using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.FutureSubmission;

namespace Iata.IS.Data.FutureSubmission
{
    public interface IGetFutureStatusInvoice
    {
        List<FutureSubmissionInvoice> GetFutureStatusInvoices(int billingYear, int billingMonth, int period);

        int UpdateFutureSubmittedInvoice(Guid invoiceId);

        int DeleteErrorCorrectableInvoices(Guid invoiceId);
    }
}
