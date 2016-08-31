using System.Collections.Generic;
using Iata.IS.Data.Reports.PaxCgoMscTopTenPartner;
using Iata.IS.Model.Reports.PaxCgoMscTopTenPartner;

namespace Iata.IS.Business.Reports.PaxCgoMscTopTenPartner.Impl
{
    public class PaxCgoMscTopTenInterlinePartner : IPaxCgoMscTopTenInterlinePartner
    {
        private IPaxCgoMscTopTenPartner PaxCgoMscTopTenPartner { get; set; }

        public PaxCgoMscTopTenInterlinePartner(IPaxCgoMscTopTenPartner iPaxCgoMscTopTenPartner)
        {
            PaxCgoMscTopTenPartner = iPaxCgoMscTopTenPartner;
        }
        public List<PaxCgoMscTopTenPartnerReportModel> GetPaxCgoMscTopTenInterlinePartner(int billingMonth, int billingYear, int billingCategory, int loginEntityId, int currencyCode, int isPayableReport)
        {
           return  PaxCgoMscTopTenPartner.GetPaxCgoMscTopTenPartner(billingMonth, billingYear, billingCategory, loginEntityId, currencyCode, isPayableReport);
        }
    }
}