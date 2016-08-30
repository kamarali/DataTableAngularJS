using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.SystemMonitor;

namespace Iata.IS.Data.SystemMonitor
{
  public interface ISystemMonitorRepository
  {
      IList<CompletedJobs> GetCompletedJobs(int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null);

    IList<PendingJobs> GetPendingJobs();

    IList<LoggedInUser> GetLoggedInUser();

    IList<OutstandingItems> GetOutStandingItems();

    IList<ProceesedFiles> GetProcessedDataFiles();

    IList<ProceesedFiles> GetProcessedSupportingDoc();

    IList<LoggedInUserByRegion> GetLoggedInUserByRegion();

    void RevertAchRecapSheetData(BillingPeriod billingPeriod);

    IList<PendingInvoices> GetPendingInvoices(PendingInvoices pendingInvoices);

    IList<IsWebResponse> GetIsWebResponse();

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    /// <summary>
    /// Enqueue Daily Output
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="isXmlGeneration"></param>
    ///CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
    void EnqueueDailyOutput(int memberId, DateTime targetDate, bool isXmlGeneration);


    //CMP#622: MISC Outputs Split as per Location IDs
    /// <summary>
    /// Enqueue Daily Location Specific Output/OAR
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="isXmlGeneration"></param>
    /// <param name="locationCode"></param>
    /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
    int EnqueueDailyLocationOutputOar(int memberId, DateTime targetDate, string locationCode, bool isXmlGeneration);
     
    //CMP#622: MISC Outputs Split as per Location IDs
    /// <summary>
    /// Enqueue Location Specific Output/OAR
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="isBilling"></param>
    /// <param name="billingYear"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="isReprocessing"></param>
    /// <param name="fileType"></param>
    /// <param name="locationCode"></param>
    /// <param name="fileGenerationDate"></param>
    /// <param name="nilFileRequired"></param>
    /// <param name="isXmlGeneration"></param>
    /// <param name="billingCategory"></param>
    void FinalizeSupportingDocumentLocation(int memberId, int isBilling, int billingYear, int billingMonth, int billingPeriod, int isReprocessing, int fileType, string locationCode, string fileGenerationDate, bool nilFileRequired, int isXmlGeneration, int billingCategory);


    /// <summary>
    /// SCP362066 - Supporting Document Missing
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    bool IsRecordExistIn_Aq_SanityCheck(string fileName);
  }
}
