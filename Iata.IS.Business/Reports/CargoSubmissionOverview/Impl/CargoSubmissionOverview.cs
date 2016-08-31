using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.CargoSubmissionOverviewData;
using Iata.IS.Data.Reports.CargoSubmissionOverviewData.Impl;

namespace Iata.IS.Business.Reports.CargoSubmissionOverview.Impl
{
    public class CargoSubmissionOverview : ICargoSubmissionOverview
    {

        public ICargoSubmissionOverviewData SubmissionOverview;

        public CargoSubmissionOverview(ICargoSubmissionOverviewData _submissionOverview)
        {
            SubmissionOverview = _submissionOverview;
        }


        public List<Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview> GetSubmissionOverview(string billingType, int billingYearFrom, int billingYearTo, int billingMonthFrom, int billingMonthTo, int billingPeriodFrom, int billingPeriodTo, int BillingEntity, int BilledEntity, int settlementMethod, int Output)
        {
            return SubmissionOverview.GetSubmissionOverviewData(billingType,billingYearFrom, billingYearTo, billingMonthFrom, billingMonthTo, billingPeriodFrom,
                       billingPeriodTo, BillingEntity, BilledEntity,settlementMethod, Output);
        }
    }
}
