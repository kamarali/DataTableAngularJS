using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Reports;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Data.Reports.Impl
{
  public class ProcessingDashboardFileStatusRepository : Repository<IsInputFile>, IProcessingDashboardFileStatusRepository
  {
    /// <summary>
    /// Following method is used to pass parameters to "PROC_SEARCH_FILE_PROC_DASH" stored procedure
    /// </summary>
    /// <param name="searchcriteria"></param>
    /// <returns>List of FileResult set</returns>
    public List<ProcessingDashboardFileStatusResultSet> GetFileStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchcriteria)
    {
      var parameters = new ObjectParameter[11];
      parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileBillingYear, typeof(Int32)) { Value = searchcriteria.BillingYear };
      parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileBillingMonth, typeof(Int32)) { Value = searchcriteria.BillingMonth };
      parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileBillingPeriod, typeof(Int32)) { Value = searchcriteria.BillingPeriod };
      parameters[3] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileBillingMemberId, typeof(Int32)) { Value = searchcriteria.BillingMemberId };
      parameters[4] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileBillingCategoryId, typeof(Int32)) { Value = searchcriteria.BillingCategory };
      parameters[5] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileFormatId, typeof(Int32)) { Value = searchcriteria.FileFormat };
      parameters[6] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileStatusId, typeof(Int32)) { Value = searchcriteria.FileStatus };
      parameters[7] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileName, typeof(Int32)) { Value = searchcriteria.FileName };
      parameters[8] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.UserId, typeof(Int32)) { Value = searchcriteria.IsUserId };
      parameters[9] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileGeneratedDateFrom, typeof(Int32)) { Value = searchcriteria.FileGeneratedDateFrom };
      parameters[10] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileGeneratedDateTo, typeof(Int32)) { Value = searchcriteria.FileGeneratedDateTo };
      // Execute stored procedure
      var list = ExecuteStoredFunction<ProcessingDashboardFileStatusResultSet>(ProcessingDashboardInvoiceFileStatusRepositoryContants.GetFileStatusSearchResult, parameters);
      return list.ToList();
    }// end GetFileStatusResultForProcDashBrd()

    /// <summary>
    /// Following method is used to pass File Ids to stored procedure "PROC_PERIOD_INCRMNT_IN_FILE", which increments BillingPeriod
    /// of Invoices within a file.
    /// </summary>
    /// <param name="fileIds">FileIds</param>
    /// <param name="billingYear">Current billing year</param>
    /// <param name="billingMonth">Current billing month</param>
    /// <param name="billingPeriod">Current billing period</param>
    /// <param name="userId">User id</param>
    /// <returns>List of Files with Action Status</returns>
    public List<ProcessingDashboardFileActionStatus> IncrementBillingPeriodForInvoicesWithinFile(string fileIds, int billingYear, int billingMonth, int billingPeriod,int userId)
    {
      var parameters = new ObjectParameter[5];
      parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileIds, typeof(string)) { Value = fileIds };
      parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.CurrentBillingYear, typeof(int)) { Value = billingYear };
      parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.CurrentBillingMonth, typeof(int)) { Value = billingMonth };
      parameters[3] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.CurrentBillingPeriod, typeof(int)) { Value = billingPeriod };
      parameters[4] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileUserId, typeof(int)) { Value = userId };

      // Execute Stored Procedure
      var fileList = ExecuteStoredFunction<ProcessingDashboardFileActionStatus>(ProcessingDashboardInvoiceFileStatusRepositoryContants.IncrementBillingPeriodForInvoicesWithinFile, parameters);
      // Return List
      return fileList.ToList();
    }// end IncrementBillingPeriodForInvoicesWithinFile()

      /// <summary>
      /// Following method is used to pass File Ids to stored procedure "PROC_LATE_SUBMIT_IN_FILE", which marks
      /// Invoices within files for late submission.
      /// </summary>
      /// <param name="fileIds">FileIds</param>
      /// <param name="billingYear">Current billing year</param>
      /// <param name="billingMonth">Current billing month</param>
      /// <param name="billingPeriod">Current billing period</param>
      /// <param name="userId"> Logged In  user </param>
      /// <param name="isICHLateAcceptanceAllowed"> ICH Late Submission Acceptance flag</param>
    /// <param name="isACHLateAcceptanceAllowed"> ACH Late Submission Acceptance flag</param>
      /// <param name="IsAchManualControl"> ACH Manual Control Flag </param>
    /// <param name="IsIchManualControl">ICH Manual Control Flag </param>
      /// <returns>List of Files with Action Status</returns>
      public List<ProcessingDashboardFileActionStatus> MarkInvoicesForLateSubmissionWithinFile(string fileIds, int billingYear, int billingMonth,
                                                                int billingPeriod, int userId, int isICHLateAcceptanceAllowed, int isACHLateAcceptanceAllowed, bool IsAchManualControl, bool IsIchManualControl)
    {
      var parameters = new ObjectParameter[9];
      parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileIds, typeof(string)) { Value = fileIds };
      parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.CurrentBillingYear, typeof(int)) { Value = billingYear };
      parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.CurrentBillingMonth, typeof(int)) { Value = billingMonth };
      parameters[3] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.CurrentBillingPeriod, typeof(int)) { Value = billingPeriod };
      parameters[4] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileUserId, typeof(int)) { Value = userId };
      parameters[5] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsIchLateAcceptanceAllowed, typeof(int)) { Value = isICHLateAcceptanceAllowed };
      parameters[6] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsAchLateAcceptanceAllowed, typeof(int)) { Value = isACHLateAcceptanceAllowed };
      parameters[7] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsAchManualControl, typeof(int)) { Value = IsAchManualControl };
      parameters[8] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsIchManualControl, typeof(int)) { Value = IsIchManualControl };

      // Execute Stored Procedure
      var fileList = ExecuteStoredFunction<ProcessingDashboardFileActionStatus>(ProcessingDashboardInvoiceFileStatusRepositoryContants.MarkInvoicesForLateSubmissionWithinFile, parameters);
      // Return List
      return fileList.ToList();
    }// end IncrementBillingPeriodForInvoicesWithinFile()

    /// <summary>
    /// Following method is used to pass File Id to stored procedure "PROC_FILE_INV_ERR_PROC_DASH"
    /// </summary>
    /// <param name="fileId">FileId</param>
    /// <returns>List of Invoices</returns>
    public List<ProcessingDashboardFileWarningDetail> GetFileInvoicesErrorWarning(Guid fileId)
    {
      var parameters = new ObjectParameter[1];
      parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileId, typeof(Guid)) { Value = fileId };

      // Execute Stored Procedure
      var fileList = ExecuteStoredFunction<ProcessingDashboardFileWarningDetail>(ProcessingDashboardInvoiceFileStatusRepositoryContants.GetFileInvErrorWarning, parameters);
      // Return List
      return fileList.ToList();
    }// end GetFileInvoicesErrorWarning()

    public List<ProcessingDashboardFileDeleteActionStatus> DeleteFiles(string selectedFileIds, int memberId, int userid)
    {
        // Declare variable of type ObjectParameter, with parameter count
        var parameters = new ObjectParameter[3];
        // Pass slelectedFileIds parameter to Stored procedure
        parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileIds, typeof(string)) { Value = selectedFileIds};

        parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.MemberId, typeof(string)) { Value = memberId };

        parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.UserId, typeof(string)) { Value = userid };
        // Execute Stored Procedure
        var fileList = ExecuteStoredFunction<ProcessingDashboardFileDeleteActionStatus>(ProcessingDashboardInvoiceFileStatusRepositoryContants.DeleteFiles, parameters);
        // Return List
        return fileList.ToList();
    }// end DeleteFiles()

    /// <summary>
    /// Method to delete invoices from processing dashboard
    /// </summary>
    /// <param name="selectedinvoiceIds"> comma seperated invoices ids, incase more than one invoice selected </param>
    /// <param name="memberId"> dummy member Id </param>
    /// <param name="userid">user Id </param>
    /// <param name="isIsWebInvoice">Is Web invoice </param>
    /// <returns></returns>
    public List<ProcessingDashInvoiceDeleteActionStatus> DeleteInvoices(string selectedinvoiceIds, int memberId, int userid,int isIsWebInvoice = 0)
    {
        // Declare variable of type ObjectParameter, with parameter count
        var parameters = new ObjectParameter[4];
        // Pass slelectedFileIds parameter to Stored procedure
        parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.InvoiceIds, typeof(string)) { Value = selectedinvoiceIds };

        parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.MemberId, typeof(string)) { Value = memberId };

        parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.UserId, typeof(string)) { Value = userid };

        parameters[3] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.IsIsWebInvoice, typeof(int)) { Value = isIsWebInvoice };

        // Execute Stored Procedure
        var invoiceList = ExecuteStoredFunction<ProcessingDashInvoiceDeleteActionStatus>(ProcessingDashboardInvoiceFileStatusRepositoryContants.DeleteInvoices, parameters);
        // Return List
        return invoiceList.ToList();
    }// end DeleteFiles()

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
      * Desc: SP called to get file procssing progress detail. */
    public void GetFileProgressDetails(Guid fileLogId, ref string processName, ref string processState, ref int queuePosition)
    {
        try
        {
            /* Parameters to SP */
            var parameters = new ObjectParameter[4];
            parameters[0] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.FileLogId,
                                                typeof(Guid)) { Value = fileLogId };
            parameters[1] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.ProcessName,
                                                typeof(string));
            parameters[2] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.ProcessState,
                                                typeof(string));
            parameters[3] = new ObjectParameter(ProcessingDashboardInvoiceFileStatusRepositoryContants.QueuePosition,
                                                typeof(Int32));

            /* Execute stored procedure PROC_GET_FILE_PROGRESS_DETAILS */
            ExecuteStoredProcedure(ProcessingDashboardInvoiceFileStatusRepositoryContants.GetFileProgressDetailsSpName,
                                   parameters);

            /* Gathering values for out Parameters from SP */
            processName = (parameters != null && parameters[1] != null && parameters[1].Value != null)
                              ? Convert.ToString(parameters[1].Value)
                              : "NONE";
            processState = (parameters != null && parameters[2] != null && parameters[2].Value != null)
                               ? Convert.ToString(parameters[2].Value)
                               : "NONE";
            queuePosition = (parameters != null && parameters[3] != null) ? Convert.ToInt32(parameters[3].Value) : -1;
        }
        catch (Exception exception)
        {
            throw;
        }
    }

  }// end ProcessingDashboardFileStatusRepository class
}// end namespace
