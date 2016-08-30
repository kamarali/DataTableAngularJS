using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.PaxCgoMscTopTenPartner;

namespace Iata.IS.Data.Reports.PaxCgoMscTopTenPartner.Impl
{
    public class PaxCgoMscTopTenPartner : Repository<InvoiceBase>, IPaxCgoMscTopTenPartner
    {

        public List<PaxCgoMscTopTenPartnerReportModel> GetPaxCgoMscTopTenPartner(int billingMonth, int billingYear, int billingCategory, int loginEntityId, int currencyCode, int isPayableReport)
        {
            var parameters = new ObjectParameter[5];
            parameters[0] = new ObjectParameter(PaxCgoMscTopTenPartnerConstant.BillingMonth, billingMonth);
            parameters[1] = new ObjectParameter(PaxCgoMscTopTenPartnerConstant.BillingYear, billingYear);
            parameters[2] = new ObjectParameter(PaxCgoMscTopTenPartnerConstant.BillingCategory, billingCategory);
            parameters[3] = new ObjectParameter(PaxCgoMscTopTenPartnerConstant.LoginEntityId, loginEntityId);
            parameters[4] = new ObjectParameter(PaxCgoMscTopTenPartnerConstant.CurrencyCode, currencyCode);

            var stroredFunctionName = string.Empty;
            // check here for Payable or Receivables report
            if (isPayableReport == 0)
            {
                stroredFunctionName = PaxCgoMscTopTenPartnerConstant.PaxCgoMscTopTenPartenerReportRec;

            }
            else if (isPayableReport == 1)
            {

                stroredFunctionName = PaxCgoMscTopTenPartnerConstant.PaxCgoMscTopTenPartenerReportPay;
            }

            var list =
                  ExecuteStoredFunction<PaxCgoMscTopTenPartnerReportModel>(stroredFunctionName, parameters);

            return list.ToList();
        }
    }
}