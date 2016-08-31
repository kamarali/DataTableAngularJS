using System.Collections.Generic;
using Iata.IS.Data.Reports.Cargo.RejectionAnalysis;
using Iata.IS.Model.Reports.Cargo.RejectionAnalysis;

namespace Iata.IS.Business.Reports.Cargo.RejectionAnalysis.Impl
{
    public class CgoRejAnalysisRec : ICgoRejAnalysisRec
    {
        private ICgoRejectionAnalysisRec CgoRejectionAnalysisRec { get; set; }

        public CgoRejAnalysisRec(ICgoRejectionAnalysisRec icgoRejectionAnalysisRec)
        {
            CgoRejectionAnalysisRec = icgoRejectionAnalysisRec;
        }

        // CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        // input parameter updated (From Year Month and To Year Month)
        public List<CgoRejectionAnalysisRecModel> GetCgoRejectionAnalysisRec(int fromBillingMonth, int fromBillingYear, int toBillingMonth, int toBillingYear, int loginEntityId, int toEntityId, int currencyCode, int isPayableReport)
        {
          return CgoRejectionAnalysisRec.GetCgoRejectionAnalysisRec(fromBillingMonth, fromBillingYear, toBillingMonth, toBillingYear, loginEntityId,
                                                                      toEntityId, currencyCode, isPayableReport);
        }
    }
}