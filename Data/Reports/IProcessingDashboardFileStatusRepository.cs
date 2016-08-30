using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports;

namespace Iata.IS.Data.Reports
{
  public interface IProcessingDashboardFileStatusRepository
  {
    List<ProcessingDashboardFileStatusResultSet> GetFileStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchcriteria);

    /// <summary>
    /// Following method is used to pass File Ids to stored procedure "PROC_PERIOD_INCRMNT_IN_FILE", which increments BillingPeriod
    /// of Invoices within a file.
    /// </summary>
    /// <param name="fileIds">FileIds</param>
    /// <returns>List of Files with Action Status</returns>
    List<ProcessingDashboardFileActionStatus> IncrementBillingPeriodForInvoicesWithinFile(string fileIds, int billingYear, int billingMonth, int billingPeriod, int userId);

    /// <summary>
    /// Following method is used to pass File Ids to stored procedure "PROC_LATE_SUBMIT_IN_FILE", which marks
    /// Invoices within files for late submission.
    /// </summary>
    /// <param name="fileIds">FileIds</param>
    /// <returns>List of Files with Action Status</returns>
    List<ProcessingDashboardFileActionStatus> MarkInvoicesForLateSubmissionWithinFile(string fileIds, int billingYear, int billingMonth,
                                                                    int billingPeriod, int userId, int isICHLateAcceptanceAllowed, int isACHLateAcceptanceAllowed, bool IsAchManualControl, bool IsIchManualControl);

    /// <summary>
    /// Following method is used to pass File Id to stored procedure "PROC_FILE_INV_ERR_PROC_DASH"
    /// </summary>
    /// <param name="fileId">FileId</param>
    /// <returns>List of Invoices</returns>
    List<ProcessingDashboardFileWarningDetail> GetFileInvoicesErrorWarning(Guid fileId);

    /// <summary>
    /// Following method is used to Delete Files
    /// </summary>
    /// <param name="selectedFileIds">File Id's to be deleted</param>
    /// <returns>List of deleted and non deleted files</returns>
    List<ProcessingDashboardFileDeleteActionStatus> DeleteFiles(string selectedFileIds, int memberId, int userid);

    /// <summary>
    /// Method to delete invoices from processing dashboard
    /// </summary>
    /// <param name="selectedinvoiceIds"> comma seperated invoices ids, incase more than one invoice selected </param>
    /// <param name="memberId"> dummy member Id </param>
    /// <param name="userid">user Id </param>
    /// <param name="isIsWebInvoice">Is ISWEB invoice</param>
    /// <returns></returns>
    List<ProcessingDashInvoiceDeleteActionStatus> DeleteInvoices(string selectedinvoiceIds, int memberId, int userid, int isIsWebInvoice = 0);

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
    * Desc: SP called to get file procssing progress detail. */
      void GetFileProgressDetails(Guid fileLogId, ref string processName, ref string processState, ref int queuePosition);

  }// end IProcessingDashboardFileStatusRepository interface
}// end namespace
