using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;

namespace Iata.IS.Data.SanityCheck.Impl
{
  public class SanityCheckRepository : Repository<InvoiceBase>, ISanityCheckRepository
  {
    public void UpdateFileStatus(string sanityCheckPassedFileIds, string sanityCheckFailedFileIds, string sanityCheckPartialFileIds)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter("SANITY_CHECK_PASSED_IDs", typeof(string)) { Value = sanityCheckPassedFileIds };
      parameters[1] = new ObjectParameter("SANITY_CHECK_FAILED_IDs", typeof(string)) { Value = sanityCheckFailedFileIds };
      parameters[2] = new ObjectParameter("SANITY_CHECK_PARTIAL_IDs", typeof(string)) { Value = sanityCheckPartialFileIds };

      // Execute stored procedure
      ExecuteStoredProcedure("FileStatusUpdate", parameters);
    }

    public void UpdateFileStatusForVCF(Guid responseVcfId, Guid requestVcfId, String filePath, string fileName, string fileId, String fileKey)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter("RESPONSE_VCF_ID", typeof(Guid)) { Value = responseVcfId };
      parameters[1] = new ObjectParameter("REQUEST_VCF_ID", typeof(Guid)) { Value = requestVcfId };
      parameters[2] = new ObjectParameter("RESPONSE_FILE_PATH", typeof(String)) { Value = filePath };
      parameters[3] = new ObjectParameter("RESPONSE_FILE_NAME", typeof(String)) { Value = fileName };
      parameters[4] = new ObjectParameter("RESPONSE_FILE_ID", typeof(String)) { Value = fileId };
      parameters[5] = new ObjectParameter("RESPONSE_FILE_KEY", typeof(String)) { Value = fileKey };

      // Execute stored procedure
      ExecuteStoredProcedure("VcfFileStatusUpdate", parameters);
    }

    /// <summary>
    /// Enqueues the form C for validation.
    /// </summary>
    /// <param name="isFileLogId">The is file log id.</param>
    /// <returns></returns>
    //SCP#402740 : SRM- Sampling form C files not processing in SIS prod
    //Enqueue logic has been moved to stored procedure "PROC_ENQUEUE_FILE".
    public void EnqueueFormCForValidation(Guid isFileLogId)
    {
        var parameters = new ObjectParameter[1];
        parameters[0] = new ObjectParameter("IS_FILE_LOG_ID_I", typeof(Guid)) { Value = isFileLogId };
        // Execute stored procedure
        // Adding entry into "VALIDATION_LOADER_QUEUE" table 
        ExecuteStoredProcedure("EnqueueFileForValidation", parameters);
    }

  }
}

