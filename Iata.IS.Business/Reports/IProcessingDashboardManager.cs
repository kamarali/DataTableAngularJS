using System;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports;
using Ionic.Zip;
using System.IO;

namespace Iata.IS.Business.Reports
{
  public interface IProcessingDashboardManager
  {
    List<int> GetAllInvoiceYear();
    List<ProcessingDashboardInvoiceStatusResultSet> GetInvoiceStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchCriteria);
    List<ProcessingDashboardFileStatusResultSet> GetFileStatusResultForProcDashBrd(ProcessingDashboardSearchEntity searchCriteria);
    ProcessingDashboardInvoiceDetail GetInvoiceDetailsForProcDashBrd(Guid invoiceId);
    List<Member> GetAggregatedSponsoredMemberList(int memberId);
    ProcessingDashboardSearchEntity SetProcessingDashboardSearchEntity(int categoryId, int memberId);
    byte[] GenerateCsv(ProcessingDashboardSearchEntity searchCriteria, char separator,bool isInvoiceStatusCsv);
    List<ProcessingDashboardInvoiceActionStatus> MarkInvoiceForLateSubmission(string[] invoiceIdArray, int memberId, int userId, Boolean isMarkfileForLateSubmission);
    List<ProcessingDashboardFileActionStatus> MarkFileForLateSubmission(string selectedFileIds, int memberId,int userId);
    List<ProcessingDashboardInvoiceActionStatus> IncrementInvoiceBillingPeriod(string[] invoiceIdArray, int userId);
    List<ProcessingDashboardInvoiceActionStatus> DeleteSelectedInvoices(string[] invoiceIdArray, int dummyMemberId, int userId);
    List<ProcessingDashboardInvoiceActionStatus> ResubmitSelectedInvoices(string[] invoiceIdArray);
    List<ProcessingDashboardFileActionStatus> IncrementBillingPeriodForInvoicesWithinFile(string fileIdsString, int memberId,int userId);
    List<ProcessingDashboardFileWarningDetail> GetFileInvoicesErrorWarning(Guid fileId);
    bool IsLateSubmissionWindowOpen();
    List<ProcessingDashboardFileDeleteActionStatus> DeleteFiles(string fileIdsString, int memberId, int userid);
    List<int> GetAllIsFileLogYear();
    List<int> GetCalendarYear();

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
    * Desc: SP called to get file procssing progress detail. */
      bool GetFileProgressDetails(Guid fileLogId, ref string processName, ref string processState, ref int queuePosition);
  }
}
