using System;
using System.Data.Objects;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.Purging.Impl
{
 public class InvalidInvTempDataPurgingRepository : Repository<InvoiceBase>, IInvalidInvTempDataPurgingRepository
 {
    private const string DummyMemberId = "DUMMY_MEMBER_ID_I";
    private const string CurrentBillingYear = "CURR_BILLING_YEAR_I";
    private const string CurrentBillingMonth = "CURR_BILLING_MONTH_I";
    private const string PurgeLimit = "PURGE_LIMIT_I";
    private const string IsWebOarPurgingSpan = "IS_WEB_OAR_PUR_I";
    private const string ResultOut = "RESULT_O";
    private const string PurgeInvalidInvTmpData = "PurgeInvalidInvTmpData";

  
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
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(DummyMemberId, dummyMemberId);
      parameters[1] = new ObjectParameter(CurrentBillingMonth, currentBillingMonth);
      parameters[2] = new ObjectParameter(CurrentBillingYear, currentBillingYear);
      parameters[3] = new ObjectParameter(PurgeLimit, purgeLimit);
      parameters[4] = new ObjectParameter(IsWebOarPurgingSpan, isWebOarPurgingSpan);
      parameters[5] = new ObjectParameter(ResultOut, typeof(int));
      // Execute Store Procedure PROC_PRG_INVALID_INV_TMP_DATA.
      ExecuteStoredProcedure(PurgeInvalidInvTmpData, parameters);
      return Convert.ToInt32(parameters[5].Value);
    }
  }
}

