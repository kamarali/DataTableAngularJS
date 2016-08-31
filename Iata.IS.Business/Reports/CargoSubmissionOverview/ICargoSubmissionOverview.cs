using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.Reports.CargoSubmissionOverview
{

    public interface ICargoSubmissionOverview
    {
        List<Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview> GetSubmissionOverview(string billingType, int billingYearFrom, int billingYearTo, int billingMonthFrom, int billingMonthTo, int billingPeriodFrom, int billingPeriodTo, int BillingEntity, int BilledEntity, int settlementMethod, int Output);
      
    }
}
