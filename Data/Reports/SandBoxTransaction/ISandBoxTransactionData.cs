using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Data.Reports.SandBoxTransaction
{
    public interface ISandBoxTransactionData : IRepository<InvoiceBase>
    {
        List<CertificationTransactionDetailsReport> GetSandBoxTransactionDetails(int memberId, DateTime fromdate, DateTime todate, int fileFormatId, int billingCategoryId, string requestType, int transactionGroupId);        
    }
}
