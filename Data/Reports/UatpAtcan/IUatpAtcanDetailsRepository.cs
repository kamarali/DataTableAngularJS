using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Reports.UatpAtcan
{
    public interface IUatpAtcanDetailsRepository : IRepository<InvoiceBase>
    {
        /// <summary>
        /// Get UatpAtcanDetails
        /// </summary>
        /// <param name="memberId">Member Id for UatpAtcan details</param>
        /// <param name="period">Period</param>
        /// <param name="billingMonth">Billing Month</param>
        /// <param name="billingYear">Billing Year</param>
        /// <param name="billingTypeId">Billing type payable/receivable</param>
        /// <returns></returns>
        List<UatpAtcanDetails> GetUatpAtcanDetails(int memberId, int period, int billingMonth, int billingYear, int billingTypeId);
    }
}
