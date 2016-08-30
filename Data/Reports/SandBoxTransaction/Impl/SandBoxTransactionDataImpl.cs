using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Data.Reports.SandBoxTransaction.Impl
{
    public class SandBoxTransactionDataImpl : Repository<InvoiceBase>, ISandBoxTransactionData
    {
        public List<CertificationTransactionDetailsReport> GetSandBoxTransactionDetails(int memberId, DateTime fromdate, DateTime todate, int fileFormatId, int billingCategoryId, string requestType, int transactionGroupId)
        {
            var parameters = new ObjectParameter[7];
            parameters[0] = new ObjectParameter(SandBoxTransactionDataConstants.MemberId, memberId);
            parameters[1] = new ObjectParameter(SandBoxTransactionDataConstants.Fromdate, fromdate);
            parameters[2] = new ObjectParameter(SandBoxTransactionDataConstants.Todate, todate);
            parameters[3] = new ObjectParameter(SandBoxTransactionDataConstants.FileFormatId, fileFormatId);
            parameters[4] = new ObjectParameter(SandBoxTransactionDataConstants.BillingCategoryId, billingCategoryId);
            parameters[5] = new ObjectParameter(SandBoxTransactionDataConstants.RequestType, requestType);
            parameters[6] = new ObjectParameter(SandBoxTransactionDataConstants.TransactionGroupId, transactionGroupId);

            var list = ExecuteStoredFunction<CertificationTransactionDetailsReport>("GetCertTransDetails", parameters);

            return list.ToList();
        }
    }
}
