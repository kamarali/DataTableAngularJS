using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Purging
{
  public interface IPurgingTransactionRepository
  {
    /// <summary>
    /// Delete expired date transaction.
    /// </summary>
    /// <param name="currentPeriod">The current period date</param>
    /// <param name="lastClosedBillingPeriod">The last closed period date</param>
    /// <param name="errorInvoiceRetentionPeriod">The retention period for error invoices</param>
    /// <returns>Integer result</returns>
    IList<SupportingDocPurgingDetails> DeleteTransaction(DateTime currentPeriod, DateTime lastClosedBillingPeriod, int errorInvoiceRetentionPeriod);

    /// <summary>
    /// Delete expired date files.
    /// </summary>
    /// <param name="currentDatePeriod">The current period date</param>
    void DeleteTemporaryFiles(DateTime currentDatePeriod);

    // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
    /// <summary>
    /// Queue Offline Collection files for purging based on:
    /// 1. All invoice offline collection files which are created on or before invoice end period.
    /// 2. All form c offline collection files which are created on or before form c end period.
    /// </summary>
    /// <param name="invEndPeriod">Threshold period tobe used to queue invoice offlice collection files.</param>
    /// <param name="forCEndPeriod">Threshold period tobe used to queue form c offlice collection files.</param>
    void QueueOfflineCollectionFilesForPurging(DateTime invEndPeriod, DateTime forCEndPeriod);
  }
}
