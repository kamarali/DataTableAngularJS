using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.SystemMonitor;

namespace Iata.IS.Business.SystemMonitor
{
 public interface ISystemMonitorManager
  {
     IList<CompletedJobs> GetCompletedJobs(string localTimeZoneId, int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null);

   IList<PendingJobs> GetPendingJobs();

   IList<LoggedInUser> GetLoggedInUser();

   IList<OutstandingItems> GetOutStandingItems();

   IList<IsWebResponse> GetIsWebResponse();

   IList<ProceesedFiles> GetProcessedDataFiles();

   IList<ProceesedFiles> GetProcessedSupportingDoc();

   IList<LoggedInUserByRegion> GetLoggedInUserByRegion();

   void RevertAchRecapSheetData(BillingPeriod billingPeriod);

   void InsertGenerateNilFormCMessageInOracleQueue(int memberId, string provisionalBillingMonth);

   void InsertProcessCVSMessageInOracleQueue(int memberId, BillingPeriod billingPeriod);

   IQueryable<PaxInvoice> GetOfflineCollectionPaxInvoices( int invoiceStatusId, int billingYear,
                                                       int billingMonth, int billingPeriod, int memberId);

    
   IQueryable<MiscUatpInvoice> GetOfflineCollectionMiscInvoices( int invoiceStatusId,
                                                                int billingYear, int billingMonth, int billingPeriod, int memberId);

   /// <summary>
   /// Author: Sachin Pharande
   /// Date Of Creation : 25-11-2011
   /// Purpose: To Get Offline Collection for Cargo Invoices
   /// </summary>
   /// <param name="invoiceStatusId"></param>
   /// <param name="billingYear"></param>
   /// <param name="billingMonth"></param>
   /// <param name="billingPeriod"></param>
   /// <param name="memberId"></param>
   /// <returns></returns>
   IQueryable<CargoInvoice> GetOfflineCollectionCargoInvoices(int invoiceStatusId, int billingYear,
                                                              int billingMonth, int billingPeriod, int memberId);

   /// <summary>
   /// Author: Sachin Pharande
   /// Date Of Creation : 25-11-2011
   /// Purpose: To Get Offline Collection for UATP Invoices
   /// </summary>
   /// <param name="invoiceStatusId"></param>
   /// <param name="billingYear"></param>
   /// <param name="billingMonth"></param>
   /// <param name="billingPeriod"></param>
   /// <param name="memberId"></param>
   /// <returns></returns>
   IQueryable<MiscUatpInvoice> GetOfflineCollectionUatpInvoices(int invoiceStatusId, int billingYear, 
                                                                int billingMonth, int billingPeriod, int memberId);

   IList<InvoiceBase> GetInvoiceEligibleMembers(int billingYear, int billingMonth, int billingPeriod, int billingCategory);

   
   Guid AddRequest(RequestLog request);

   IList<OutputFile> GetOutputFilesToResend(OutputFile outputFile);

   IQueryable<IsInputFile> GetIsInputFilesByIds(string[] inputFilesIds);

   IList<PendingInvoices> GetPendingInvoices(PendingInvoices pendingInvoices);

   void UpdatePendingInvoiceStatus(Guid invoiceId);

   //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    /// <summary>
    /// Enqueue Daily Output
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="targetDate"></param>
    /// <param name="isXmlGeneration"></param>
   /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
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
     /// <param name="isXmlGeneration"></param>
     /// <param name="billingCategory"></param>
  void EnqueueLocationOutputOar(int memberId, int isBilling, int billingYear, int billingMonth, int billingPeriod, int isReprocessing, int fileType, string locationCode, string fileGenerationDate, int isXmlGeneration, int billingCategory);


   List<MiscellaneousConfiguration> GetIsWebMiscInvMemberList();
  }
}
