using System;

namespace Iata.IS.Data.Purging
{
  public interface IInvalidInvTempDataPurgingRepository
  {
    /// <summary>
    /// This method purges the invalid Invoices and Temporary Data
    /// This method will be called from invalid invoice temp data purge.
    /// </summary>
    /// <param name="dummyMemberId">Dummy Member Id from Config</param>
    /// <param name="currentBillingMonth">current period billing month</param>
    /// <param name="currentBillingYear">current period billing year</param>
    /// <param name="purgeLimit">purging limit in months</param>
    /// <param name="isWebOarPurgingSpan">Is Web Oar Purging Span as a Date</param>
    int PurgeInvalidInvTempData(int dummyMemberId, int currentBillingMonth, int currentBillingYear, int purgeLimit, DateTime isWebOarPurgingSpan);
  }
}
