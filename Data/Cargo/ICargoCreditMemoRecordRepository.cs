using System;
using Iata.IS.Model.Cargo;

namespace Iata.IS.Data.Cargo
{
  public interface ICargoCreditMemoRecordRepository : IRepository<CargoCreditMemo>
  {
    /// <summary>
    /// Gets the credit memo duplicate count.
    /// </summary>
    /// <param name="creditMemoNumber">The credit memo number.</param>
    /// <param name="billedMemberId">The billed member id.</param>
    /// <param name="billingMemberId">The billing member id.</param>
    /// <param name="billingMonth">The billing month.</param>
    /// <param name="billingYear">The billing year.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <returns></returns>
    long GetCreditMemoDuplicateCount(string creditMemoNumber, int billedMemberId, int billingMemberId, int billingMonth, int billingYear, int billingPeriod);
    
    /// <summary>
    /// LoadStrategy method overload of Single method
    /// </summary>
    /// <param name="creditMemoId">CreditMemo Id</param>
    /// <returns>CreditMemo</returns>
    CargoCreditMemo Single(Guid creditMemoId);


  }
}
