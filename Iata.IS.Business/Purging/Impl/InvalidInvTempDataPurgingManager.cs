using System;
using System.Reflection;
using Iata.IS.Data.Purging;
using log4net;

namespace Iata.IS.Business.Purging.Impl
{
  public class InvalidInvTempDataPurgingManager : IInvalidInvTempDataPurgingManager
  {private const string LogsFolderName = "Logs";

  private IInvalidInvTempDataPurgingRepository PurgeInvalidInvTmpData { get; set; }
    
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public InvalidInvTempDataPurgingManager(IInvalidInvTempDataPurgingRepository purgeInvalidInvTmpData)
    {
        PurgeInvalidInvTmpData = purgeInvalidInvTmpData;
    }

    /// <summary>
    /// This method purges the invalid Invoices and Temporary Data
    /// This method will be called from invalid invoice temp data purge.
    /// </summary>
    /// <param name="dummyMemberId">Dummy Member Id from Config</param>
    /// <param name="currentBillingMonth">current period billing month</param>
    /// <param name="currentBillingYear">current period billing year</param>
    /// <param name="purgeLimit">purging limit in months</param>
    /// <param name="isWebOarPurgingSpan">Is Web Oar Purging Span as a Date</param>
    public int PurgeInvalidInvTempData(int dummyMemberId, int currentBillingMonth, int currentBillingYear, int purgeLimit, DateTime isWebOarPurgingSpan)
    {
        return PurgeInvalidInvTmpData.PurgeInvalidInvTempData(dummyMemberId, currentBillingMonth, currentBillingYear, purgeLimit, isWebOarPurgingSpan);
    }
  }
}
