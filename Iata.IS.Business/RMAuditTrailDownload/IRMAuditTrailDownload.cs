using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.RMAuditTrailDownload
{
  /// <summary>
  /// This interface is used for generate audit trail for rejection memo and send mail to user.
  /// </summary>
  //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
  public interface IRMAuditTrailDownload
  {
    /// <summary>
    /// This function is used for generate rejection memo audit trail report based on request.
    /// </summary>
    /// <param name="requestMessage"></param>
    void GenerateRejectionAuditTrailReportZip(ReportDownloadRequestMessage requestMessage, int ProcessingUnitNumber);
  }
}
