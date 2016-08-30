using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
    public interface ICargoBillingMemoRepository : IRepository<CargoBillingMemo>
    {
        /// <summary>
        /// Gets the Cargo billing memo duplicate count.
        /// </summary>
        /// <param name="billingMemoNumber">The billing memo number.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingPeriod">The billing period.</param>
        /// <returns>Returns count of record</returns>
        long GetCargoBillingMemoDuplicateCount(string billingMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod);

        /// <summary>
        /// Get Single Billing Memo record
        /// </summary>
        /// <param name="billingMemoId">The Billing Memo Id</param>
        /// <returns>Single record of Billing Memo</returns>
        CargoBillingMemo Single(Guid billingMemoId);
    }
}
