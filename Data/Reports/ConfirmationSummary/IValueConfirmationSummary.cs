using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.ConfirmationSummary;

namespace Iata.IS.Data.Reports.ConfirmationSummary
{
    public interface IValueConfirmationSummary : IRepository<InvoiceBase>
    {
        List<ConfirmationSummaryModel> GetValueConfirmationSummaryData(int billingMonth, int billingYear, int billingPeriod);
    }// End IValueConfirmationSummary
}
