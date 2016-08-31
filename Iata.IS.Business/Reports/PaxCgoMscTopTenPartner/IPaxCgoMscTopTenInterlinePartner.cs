using System.Collections.Generic;
using Iata.IS.Model.Reports.PaxCgoMscTopTenPartner;

namespace Iata.IS.Business.Reports.PaxCgoMscTopTenPartner
{
    public interface IPaxCgoMscTopTenInterlinePartner
    {
        List<PaxCgoMscTopTenPartnerReportModel> GetPaxCgoMscTopTenInterlinePartner(int billingMonth, int billingYear, int billingCategory, int loginEntityId, int currencyCode, int isPayableReport);

    }
}