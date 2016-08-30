using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.ACHRecapSheet.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.SystemMonitor;


namespace Iata.IS.Data.SystemMonitor.Imp
{
  public class SystemMonitorRepository : Repository<Permission>, ISystemMonitorRepository
  {

    //CMP529 : Daily Output Generation for MISC Bilateral Invoices
    public const string EnqueueDailyOutputFunctionName = "EnqueueDailyOutput";

    //CMP#622: MISC Outputs Split as per Location IDs
    public const string FinalizeSupportingDocumentLocationFunctionName = "FinalizeSuppDocLoc";//"EnqueueLocationOutputOar";
    public const string DailyFinalizeSupportingDocumentLocationFunctionName = "DailyFinalizeSuppDocLoc";//"EnqueueDailyLocationOutputOar";

    private const string LocMember = "MEMBER_ID_I";
    private const string LocIsBilling = "IS_BILLING_I";
    private const string LocYear = "BILLING_YEAR_I";
    private const string LocMonth = "BILLING_MONTH_I";
    private const string LocPeriod = "BILLING_PERIOD_I";
    private const string LocIsReprocessing = "IS_REPROCESSING_I";
    private const string LocFileType = "FILETYPE_ID_I";
    private const string LocLocationCode = "MEM_LOCATIONCODE_I";
    private const string LocFileGendate = "FILEGENERATIONDATE_ID_I";
    private const string LocNilFileReq = "IS_REQ_MISC_LOC_NILFILE_I";
    private const string LocIsXmlGen = "IS_XML_GENERATION_I";

    private const string DailyLocMember = "MEMBER_ID_I";
    private const string DailyLocTargetDate = "TARGET_DATE_I";
    private const string DailyLocNilFileReq = "NIL_FILE_REQUIRED_I";
    private const string DailyLocationCode = "LOCATION_ID_I";
    private const string DailyLocIsXmlGen = "IS_XML_GENERATION_I";
    private const string DailyLocResult = "SUCCESS_O";

      /// <summary>
      /// SCP#440318 - SRM: current stats screen is too slow
      /// </summary>
      /// <param name="pageSize"></param>
      /// <param name="pageNo"></param>
      /// <param name="sortColumn"></param>
      /// <param name="sortOrder"></param>
      /// <returns></returns>
    public IList<CompletedJobs> GetCompletedJobs(int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null)
    {
        var parameters = new ObjectParameter[4];
        parameters[0] = new ObjectParameter("PAGE_SIZE_I", typeof(int)) { Value = pageSize ?? 5 };
        parameters[1] = new ObjectParameter("PAGE_NO_I", typeof(int)) { Value = pageNo ?? 1 };
        parameters[2] = new ObjectParameter("SORT_COLUMN_I", typeof(string)) { Value = sortColumn };
        parameters[3] = new ObjectParameter("SORT_ORDER_I", typeof(string)) { Value = sortOrder };
        
        var completedJobsList = ExecuteStoredFunction<CompletedJobs>("GetSysMonitorCompletedJobs", parameters);
        return completedJobsList.ToList();
    }

    public IList<PendingJobs> GetPendingJobs()
    {

      var pendingJobsList = ExecuteStoredFunction<PendingJobs>("GetSysMonitorPendingJobs");

      return pendingJobsList.ToList();
    }


    public IList<LoggedInUser> GetLoggedInUser()
    {

      var loggedInUserList = ExecuteStoredFunction<LoggedInUser>("GetSysMonitorLoggedInUsers");

      return loggedInUserList.ToList();
    }

    public IList<LoggedInUserByRegion> GetLoggedInUserByRegion()
    {

      var loggedInUserList = ExecuteStoredFunction<LoggedInUserByRegion>("GetSysLoggedInUserByRegion");

      return loggedInUserList.ToList();
    }


    public IList<OutstandingItems> GetOutStandingItems()
    {

      var loggedInUserList = ExecuteStoredFunction<OutstandingItems>("GetSysOutstandingItems");

      return loggedInUserList.ToList();
    }

    public IList<IsWebResponse> GetIsWebResponse()
    {
        var isWebResponseList = ExecuteStoredFunction<IsWebResponse>("GetSysIsWebResponse");
        return isWebResponseList.ToList();
    }

    public IList<ProceesedFiles> GetProcessedDataFiles()
    {
      var processedDataFilesList = ExecuteStoredFunction<ProceesedFiles>("GetSysProcessedDataFiles");

      return processedDataFilesList.ToList();
    }

    public IList<ProceesedFiles> GetProcessedSupportingDoc()
    {
      var processedSupportDocList = ExecuteStoredFunction<ProceesedFiles>("GetSysProcessedSupportDoc");

      return processedSupportDocList.ToList();
    }


    public void RevertAchRecapSheetData(BillingPeriod billingPeriod)
    {
      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingYear, typeof(Int32)) { Value = billingPeriod.Year };
      parameters[1] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingMonth, typeof(Int32)) { Value = billingPeriod.Month };
      parameters[2] = new ObjectParameter(ACHRecapSheetRepositoryContants.CurrentBillingPeriod, typeof(Int32)) { Value = billingPeriod.Period };
      ExecuteStoredProcedure(ACHRecapSheetRepositoryContants.RevertACHRecapSheetData, parameters);
    }

     public IList<PendingInvoices> GetPendingInvoices(PendingInvoices pendingInvoices)
     {
       var parameters = new ObjectParameter[6];
       parameters[0] = new ObjectParameter("BILLING_MONTH_I", typeof(Int32)) { Value = pendingInvoices.BillingMonth };
       parameters[1] = new ObjectParameter("BILLING_YEAR_I", typeof(Int32)) { Value = pendingInvoices.BillingYear };
       parameters[2] = new ObjectParameter("BILLING_PERIOD_I", typeof(Int32)) { Value = pendingInvoices.BillingPeriod };
       parameters[3] = new ObjectParameter("BILLING_MEMBER_ID_I", typeof(Int32)) { Value = pendingInvoices.BillingMemberId };
       parameters[4] = new ObjectParameter("BILLED_MEMBER_ID_I", typeof(Int32)) { Value = pendingInvoices.BilledMemberId };
       parameters[5] = new ObjectParameter("BILLING_CATEGORY_I", typeof(Int32)) { Value = pendingInvoices.BillingCategoryId };

       var pendingInvoicesList = ExecuteStoredFunction<PendingInvoices>("GetSysPendingInvoices", parameters);

       return pendingInvoicesList.ToList();

     }

     //CMP529 : Daily Output Generation for MISC Bilateral Invoices
     /// <summary>
     /// Enqueue Daily Output
     /// </summary>
     /// <param name="memberId"></param>
     /// <param name="targetDate"></param>
     /// <param name="isXmlGeneration"></param>
     /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
     public void EnqueueDailyOutput(int memberId, DateTime targetDate, bool isXmlGeneration)
     {
         var parameters = new ObjectParameter[3];
         parameters[0] = new ObjectParameter("TARGET_DATE_I", typeof(DateTime)) { Value = targetDate };
         parameters[1] = new ObjectParameter("MEMBER_ID_I", typeof(Int32)) { Value = memberId };
         parameters[2] = new ObjectParameter("IS_XML_GENERATION_I", typeof(Int32)) { Value = isXmlGeneration };

         ExecuteStoredProcedure(EnqueueDailyOutputFunctionName, parameters);

     }

      //CMP#622: MISC Outputs Split as per Location IDs
      /// <summary>
      /// Enqueue Daily Location Specific Output/OAR
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="targetDate"></param>
      /// <param name="isXmlGeneration"></param>
      /// <param name="locationCode"></param>
     ///CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
     public int EnqueueDailyLocationOutputOar(int memberId, DateTime targetDate, string locationCode, bool isXmlGeneration)
     {
         var parameters = new ObjectParameter[5];
         parameters[0] = new ObjectParameter(DailyLocMember, typeof(Int32)) { Value = memberId };
         parameters[1] = new ObjectParameter(DailyLocTargetDate, typeof(DateTime)) { Value = targetDate };
         parameters[2] = new ObjectParameter(DailyLocationCode, typeof(String)) { Value = locationCode };
         parameters[3] = new ObjectParameter(DailyLocIsXmlGen, typeof(Int32)) { Value = isXmlGeneration };
         parameters[4] = new ObjectParameter(DailyLocResult, typeof(Int32));

         ExecuteStoredProcedure(DailyFinalizeSupportingDocumentLocationFunctionName, parameters);
         var result = Convert.ToInt32(parameters[4].Value);
         return result;
     }

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
     public void FinalizeSupportingDocumentLocation(int memberId, int isBilling, int billingYear, int billingMonth, int billingPeriod, int isReprocessing, int fileType, string locationCode, string fileGenerationDate, bool nilFileRequired, int isXmlGeneration, int billingCategory)
     {
         var parameters = new ObjectParameter[11];
         parameters[0] = new ObjectParameter(LocMember, typeof(Int32)) { Value = memberId };
         parameters[1] = new ObjectParameter(LocIsBilling, typeof(Int32)) { Value = isBilling };
         parameters[2] = new ObjectParameter(LocYear, typeof(Int32)) { Value = billingYear };
         parameters[3] = new ObjectParameter(LocMonth, typeof(Int32)) { Value = billingMonth };
         parameters[4] = new ObjectParameter(LocPeriod, typeof(Int32)) { Value = billingPeriod };
         parameters[5] = new ObjectParameter(LocIsReprocessing, typeof(Int32)) { Value = isReprocessing };
         parameters[6] = new ObjectParameter(LocFileType, typeof(Int32)) { Value = fileType };
         parameters[7] = new ObjectParameter(LocLocationCode, typeof(String)) { Value = locationCode };
         parameters[8] = new ObjectParameter(LocFileGendate, typeof(String)) { Value = fileGenerationDate };
         parameters[9] = new ObjectParameter(LocNilFileReq, typeof(Int32)) { Value = nilFileRequired };
         parameters[10] = new ObjectParameter(LocIsXmlGen, typeof(Int32)) { Value = isXmlGeneration };

         ExecuteStoredProcedure(FinalizeSupportingDocumentLocationFunctionName, parameters);

     }

     /// <summary>
     /// SCP362066 - Supporting Document Missing
     /// </summary>
     /// <param name="fileName"></param>
     /// <returns></returns>
     public bool IsRecordExistIn_Aq_SanityCheck(string fileName)
     {
         try
         {
             var parameters = new ObjectParameter[2];
             parameters[0] = new ObjectParameter("File_Name_i", typeof(string))
             {
                 Value = fileName
             };
             parameters[1] = new ObjectParameter("IS_EXISTS_O", typeof(int));

             ExecuteStoredProcedure("IsRecordExistsInAQSanity", parameters);
             return Convert.ToBoolean(parameters[1].Value);
         }
         catch (Exception ex)
         {
             return false;
         }
     }

  }
}
