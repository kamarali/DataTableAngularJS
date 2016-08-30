using System;

namespace Iata.IS.Data.SanityCheck
{
  public interface ISanityCheckRepository
  {
    void UpdateFileStatus(string sanityCheckPassedFileIds, string sanityCheckFailedFileIds,
                          string sanityCheckPartialFileIds);

    void UpdateFileStatusForVCF(Guid responseVcfId, Guid requestVcfId, String filePath, string fileName, string fileId, String fileKey);

    //SCP#402740 : SRM- Sampling form C files not processing in SIS prod
    //Enqueue logic has been moved to stored procedure "PROC_ENQUEUE_FILE".
    /// <summary>
    /// Enqueues the form C for validation.
    /// </summary>
    /// <param name="isFileLogId">The is file log id.</param>
    void EnqueueFormCForValidation(Guid isFileLogId);

  }
}

