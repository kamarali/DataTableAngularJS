using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.SandBox;

namespace Iata.IS.Business.Reports.SandBoxTransaction
{
    public interface ISandBoxTransaction
    {
        /// <summary>
        /// Returns list of future updates Sand Box Trnasaction records for passed search criteria
        /// </summary>
        /// <returns>List of Future Updates class object</returns>
        List<CertificationTransactionDetailsReport> GetSandBoxTransationData(int memberId, DateTime fromdate, DateTime todate,
                                                                    int fileFormatId, int billingCategoryId, string requestType, int transactionGroupId);        
    }
}
