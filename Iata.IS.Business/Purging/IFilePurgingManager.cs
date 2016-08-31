using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Purging
{
  public interface IFilePurgingManager
  {
    void SearchPurgingFiles(string arg);

    void QueuePurgingFile(PurgingFileQueueMessage purgingFileQueueMessages);


    void PurgeFiles(PurgingFileQueueMessage purgingFileQueueMessage);

    bool UpdateIsFilePurge(Guid isFilePurgeId);

    // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
    /// <summary>
    /// Method fetches current billing period if open else pervious period and use it 
    /// to calulate threshold periods used for purging of invoice/form c offline collection files.
    /// Call stored procedure by passing calulated threshold periods. The stored proc then queues
    /// invoice/form c offline collection files to file purging queue.
    /// </summary>
    void QueueOfflineCollectionFilesForPurging();

  }
}
