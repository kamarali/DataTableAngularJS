using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.ReceivableCargoSubmissionOverviewModel;

namespace Iata.IS.Data.Reports.CargoSubmissionOverviewData.Impl
{
    public class CargoSubmissionOverviewData : Repository<InvoiceBase>, ICargoSubmissionOverviewData
    {
        public List<Model.Reports.ReceivableCargoSubmissionOverviewModel.ReceivableCargoSubmissionOverview> GetSubmissionOverviewData(string billingType, int billingYearFrom, int billingYearTo, int billingMonthFrom, int billingMonthTo, int billingPeriodFrom, int billingPeriodTo, int billingEntity, int billedEntity, int settlementMethod, int output)
        {
            var parameters = new ObjectParameter[11];

            parameters[0] = new ObjectParameter(SubmissionOverviewConstant.BillingType, billingType);
            parameters[1] = new ObjectParameter(SubmissionOverviewConstant.BillingYearFrom, billingYearFrom);
            parameters[2] = new ObjectParameter(SubmissionOverviewConstant.BillingYearTo, billingYearTo);
            parameters[3] = new ObjectParameter(SubmissionOverviewConstant.BillingMonthFrom, billingMonthFrom);
            parameters[4] = new ObjectParameter(SubmissionOverviewConstant.BillingMonthTo, billingMonthTo);
            parameters[5] = new ObjectParameter(SubmissionOverviewConstant.BillingPeriodFrom, billingPeriodFrom);
            parameters[6] = new ObjectParameter(SubmissionOverviewConstant.BillingPeriodTO, billingPeriodTo);
            parameters[7] = new ObjectParameter(SubmissionOverviewConstant.BILLING_ENTITY_CODE, billingEntity);
            parameters[8] = new ObjectParameter(SubmissionOverviewConstant.BILLED_ENTITY_CODE, billedEntity);
            parameters[9] = new ObjectParameter(SubmissionOverviewConstant.SettlementMethod, settlementMethod);
            parameters[10] = new ObjectParameter(SubmissionOverviewConstant.Output, output);

            var list = ExecuteStoredFunction<ReceivableCargoSubmissionOverview>(SubmissionOverviewConstant.GetSubmissionOverviewData, parameters);

            return list.ToList();
        }
    }
}
