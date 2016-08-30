using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Data.Reports.ConfirmationDetails.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.ConfirmationSummary;

namespace Iata.IS.Data.Reports.ConfirmationSummary.Impl
{
    public class ValueConfirmationSummary :  Repository<InvoiceBase>, IValueConfirmationSummary
    {
        public List<ConfirmationSummaryModel> GetValueConfirmationSummaryData(int billingMonth, int billingYear , int billingPeriod)
        {
            var parameters = new ObjectParameter[3];
            parameters[0] = new ObjectParameter(ValueConfirmationSummaryConstant.BillingMonth, billingMonth);
            parameters[1] = new ObjectParameter(ValueConfirmationSummaryConstant.BillingYear, billingYear);
            parameters[2] = new ObjectParameter(ValueConfirmationSummaryConstant.BillingPeriod, billingPeriod);

            var list = ExecuteStoredFunction<ConfirmationSummaryModel>(ValueConfirmationSummaryConstant.ValueConfirmationSummary, parameters);

            return list.ToList();
        }// End GetValueConfirmationSummaryData
    }// End ValueConfirmationSummary
}
