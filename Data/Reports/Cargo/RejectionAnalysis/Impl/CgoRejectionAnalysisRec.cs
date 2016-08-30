using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports.Cargo.RejectionAnalysis;

namespace Iata.IS.Data.Reports.Cargo.RejectionAnalysis.Impl
{
    public class CgoRejectionAnalysisRec : Repository<InvoiceBase>, ICgoRejectionAnalysisRec
    {
        // CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        // input parameter updated (From Year Month and To Year Month)
        public List<CgoRejectionAnalysisRecModel> GetCgoRejectionAnalysisRec(int fromBillingMonth, int fromBillingYear, int toBillingMonth, int toBillingYear, int loginEntityId, int toEntityId, int currencyCode, int isPayableReport)
        {
            var parameters = new ObjectParameter[7];
            parameters[0] = new ObjectParameter(CgoRejectionAnalysisRecConstant.FromBillingMonth, fromBillingMonth);
            parameters[1] = new ObjectParameter(CgoRejectionAnalysisRecConstant.FromBillingYear, fromBillingYear);
            parameters[2] = new ObjectParameter(CgoRejectionAnalysisRecConstant.ToBillingMonth, toBillingMonth);
            parameters[3] = new ObjectParameter(CgoRejectionAnalysisRecConstant.ToBillingYear, toBillingYear);
            parameters[4] = new ObjectParameter(CgoRejectionAnalysisRecConstant.LoginEntityId, loginEntityId);
            parameters[5] = new ObjectParameter(CgoRejectionAnalysisRecConstant.ToEntityId, toEntityId);
            parameters[6] = new ObjectParameter(CgoRejectionAnalysisRecConstant.CurrencyCode, currencyCode);

            var stroredFunctionName = string.Empty;
            // check here for Payable or Receivables report
            if (isPayableReport == 0)
            {
                stroredFunctionName = CgoRejectionAnalysisRecConstant.CgoRejAnalysisRec;

            }
            else if (isPayableReport == 1)
            {

                stroredFunctionName = CgoRejectionAnalysisRecConstant.CgoRejAnalysisPay;
            }

            var list =
                  ExecuteStoredFunction<CgoRejectionAnalysisRecModel>(stroredFunctionName, parameters);

            return list.ToList();
           
        }
    }
}