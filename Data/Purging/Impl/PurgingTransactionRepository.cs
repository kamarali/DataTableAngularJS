using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Purging.Impl
{
  class PurgingTransactionRepository : Repository<InvoiceBase>, IPurgingTransactionRepository
  {
    /// <summary>
    /// Deletes expired transactions and queues linked supporting documents for deletion.
    /// </summary>
    /// <param name="currentPeriod">The current period date</param>
    /// <param name="lastClosedBillingPeriod">Last closed billing period</param>
    /// <param name="errorInvoiceRetentionPeriod">Retention period for error invoices</param>
    /// <returns>Integer result</returns>
    public IList<SupportingDocPurgingDetails> DeleteTransaction(DateTime currentPeriod, DateTime lastClosedBillingPeriod, int errorInvoiceRetentionPeriod)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(PurgingTransactionConstants.CurrentPeriod, currentPeriod);
      parameters[1] = new ObjectParameter(PurgingTransactionConstants.LastClosedPeriod, lastClosedBillingPeriod);
      parameters[2] = new ObjectParameter(PurgingTransactionConstants.ErrorInvoiceRetentionPeriod, errorInvoiceRetentionPeriod);
      var purgingDetails = ExecuteStoredFunction<SupportingDocPurgingDetails>(PurgingTransactionConstants.DeleteTransactionFunctionName, parameters);

      return purgingDetails.ToList();
    }
    /// <summary>
    /// Delete expired date files.
    /// </summary>
    /// <param name="currentDatePeriod">The current period date</param>
    public void DeleteTemporaryFiles(DateTime currentDatePeriod)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(PurgingTransactionConstants.CurrentPeriod, currentDatePeriod);
      ExecuteStoredProcedure(PurgingTransactionConstants.DeleteTemporaryFilesFunctionName, parameters);
    }

    // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
    /// <summary>
    /// Queue Offline Collection files for purging based on:
    /// 1. All invoice offline collection files which are created on or before invoice end period.
    /// 2. All form c offline collection files which are created on or before form c end period.
    /// </summary>
    /// <param name="invEndPeriod">Threshold period tobe used to queue invoice offlice collection files.</param>
    /// <param name="forCEndPeriod">Threshold period tobe used to queue form c offlice collection files.</param>
    public void QueueOfflineCollectionFilesForPurging(DateTime invEndPeriod, DateTime forCEndPeriod)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(PurgingTransactionConstants.InvoiceEndPeriod, invEndPeriod);
      parameters[1] = new ObjectParameter(PurgingTransactionConstants.FormCEndPeriod, forCEndPeriod);
      ExecuteStoredProcedure(PurgingTransactionConstants.QueueOffColFilesForPurgingFunctionName, parameters);
    }// End QueueOfflineCollectionFilesForPurging()
  }
}
