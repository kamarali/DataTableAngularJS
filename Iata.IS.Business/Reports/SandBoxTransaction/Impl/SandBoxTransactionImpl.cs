using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.SandBoxTransaction;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Business.Reports.SandBoxTransaction.Impl
{
    public class SandBoxTransactionImpl : ISandBoxTransaction
    {
        private ISandBoxTransactionData SandBoxTransactionParam { get; set; }

        public SandBoxTransactionImpl(ISandBoxTransactionData sandBoxTransactionData)
        {
            SandBoxTransactionParam = sandBoxTransactionData;
        }
        public List<CertificationTransactionDetailsReport> GetSandBoxTransationData(int memberId, DateTime fromdate, DateTime todate, int fileFormatId, int billingCategoryId, string requestType, int transactionGroupId)
        {
            return SandBoxTransactionParam.GetSandBoxTransactionDetails(memberId, fromdate, todate, fileFormatId, billingCategoryId, requestType, transactionGroupId);

        }
    }
}
