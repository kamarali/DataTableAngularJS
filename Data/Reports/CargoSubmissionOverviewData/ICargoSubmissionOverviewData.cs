using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel;

namespace Iata.IS.Data.Reports.CargoSubmissionOverviewData
{
    public interface ICargoSubmissionOverviewData
    {
        List<ReceivableCargoSubmissionOverview> GetSubmissionOverviewData(string billingType, int billingYearFrom, int billingYearTo, int billingMonthFrom, int billingMonthTo, int billingPeriodFrom,
                       int billingPeriodTo, int BillingEntity, int BilledEntity, int settlementMethod, int output);
    }
}
