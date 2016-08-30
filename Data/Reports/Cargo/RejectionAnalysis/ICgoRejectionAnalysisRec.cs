using System.Collections.Generic;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.Cargo.RejectionAnalysis;

namespace Iata.IS.Data.Reports.Cargo.RejectionAnalysis
{
    public interface ICgoRejectionAnalysisRec : IRepository<InvoiceBase>
    {
      // CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
      // input parameter updated (From Year Month and To Year Month)
      List<CgoRejectionAnalysisRecModel> GetCgoRejectionAnalysisRec(int fromBillingMonth, int fromBillingYear, int toBillingMonth, int toBillingYear, int loginEntityId, int toEntityId, int currencyCode, int isPayableReport);

    }
}