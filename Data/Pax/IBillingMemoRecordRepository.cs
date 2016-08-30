using System;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Pax
{
  public interface IBillingMemoRecordRepository : IRepository<BillingMemo>
  {
    /// <summary>
    /// Gets the billing memo duplicate count.
    /// </summary>
    /// <param name="billingMemoNumber">The billing memo number.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns>Returns count of record</returns>
    long GetBillingMemoDuplicateCount(string billingMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod);

    /// <summary>
    /// Get Single Billing Memo record
    /// </summary>
    /// <param name="billingMemoId">The Billing Memo Id</param>
    /// <returns>Single record of Billing Memo</returns>
    BillingMemo Single(Guid billingMemoId);

   
  }
}
