using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.PaxCgoMscTopTenPartner;

namespace Iata.IS.Data.Reports.PaxCgoMscTopTenPartner
{
    public interface IPaxCgoMscTopTenPartner : IRepository<InvoiceBase>
    {
        List<PaxCgoMscTopTenPartnerReportModel> GetPaxCgoMscTopTenPartner(int billingMonth, int billingYear, int billingCategory, int loginEntityId, int currencyCode, int isPayableReport);

    }
}