using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.Pax;
using Iata.IS.Data.SystemMonitor;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SystemMonitor;
using log4net;

namespace Iata.IS.Business.SystemMonitor.Imp
{
  public class SystemMonitorManager : ISystemMonitorManager
  {
    public ISystemMonitorRepository SystemMonitorRepository { get; set; }
    public IRepository<IsInputFile> IsInputFileRep { get; set; }
    public IInvoiceRepository InvoiceRepository { get; set; }
    public ICargoInvoiceRepository CargoRepository { get; set;}
    public IRepository<RequestLog> RequestLogRepository { get; set; }
    public IRepository<Member> MemberRepository { get; set; }
    public IMiscInvoiceRepository MiscInvoiceRepository { private get; set; }
    public ISamplingFormCRepository SamplingFormCRepository { get; set; }
    public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }
    public IMiscConfigurationRepository MiscConfigurationRepository  { get; set; }


    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


    public IQueryable<PaxInvoice> GetOfflineCollectionPaxInvoices(int invoiceStatusId, int billingYear, int billingMonth, int billingPeriod, int memberId)
    {

      IQueryable<PaxInvoice> invoiceList = null;

      if (memberId > 0)
      {
        invoiceList =
        InvoiceRepository.Get(
          invoice =>
          invoice.BillingCategoryId == (int)BillingCategoryType.Pax && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && invoice.BillingYear == billingYear &&
          invoice.BillingMonth == billingMonth
          && invoice.BillingPeriod == billingPeriod && (invoice.BillingMemberId == memberId || invoice.BilledMemberId == memberId));

      }
      else
      {
        invoiceList =
        InvoiceRepository.Get(
          invoice =>
          invoice.BillingCategoryId == (int)BillingCategoryType.Pax && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && invoice.BillingYear == billingYear &&
          invoice.BillingMonth == billingMonth
          && invoice.BillingPeriod == billingPeriod);
      }


      //if (memberId > 0)
      //{
      //  invoiceList = invoiceList.Where(u => u.BillingMemberId == memberId || u.BilledMemberId == memberId);

      //  var str = invoiceList.Count();
      //}

      return invoiceList;
    }

    public IQueryable<MiscUatpInvoice> GetOfflineCollectionMiscInvoices(int invoiceStatusId, int billingYear, int billingMonth, int billingPeriod, int memberId)
    {
      var invoiceList =
        MiscInvoiceRepository.Get(
          invoice =>
          invoice.BillingCategoryId == (int)BillingCategoryType.Misc && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && invoice.BillingYear == billingYear &&
          invoice.BillingMonth == billingMonth
          && invoice.BillingPeriod == billingPeriod);

      if (memberId > 0)
      {
        invoiceList = invoiceList.Where(u => u.BillingMemberId == memberId || u.BilledMemberId == memberId);
      }

      return invoiceList;
    }

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
    public IQueryable<CargoInvoice> GetOfflineCollectionCargoInvoices(int invoiceStatusId, int billingYear, int billingMonth, int billingPeriod, int memberId)
    {
        IQueryable<CargoInvoice> invoiceList = null;

        if (memberId > 0)
        {
            invoiceList = CargoRepository.Get(invoice => invoice.BillingCategoryId == (int)BillingCategoryType.Cgo
                                                                        && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling ||
                                                                           invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed ||
                                                                           invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete ||
                                                                           invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented)
                                                                        && invoice.BillingYear == billingYear
                                                                        && invoice.BillingMonth == billingMonth
                                                                        && invoice.BillingPeriod == billingPeriod
                                                                        && (invoice.BillingMemberId == memberId ||
                                                                           invoice.BilledMemberId == memberId));
        }
        else
        {
            invoiceList = CargoRepository.Get(invoice => invoice.BillingCategoryId == (int)BillingCategoryType.Cgo
                                                                        && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling ||
                                                                           invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed ||
                                                                           invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete ||
                                                                           invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented)
                                                                        && invoice.BillingYear == billingYear
                                                                        && invoice.BillingMonth == billingMonth
                                                                        && invoice.BillingPeriod == billingPeriod);
        }

        return invoiceList;
    }

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
    public IQueryable<MiscUatpInvoice> GetOfflineCollectionUatpInvoices(int invoiceStatusId, int billingYear, int billingMonth, int billingPeriod, int memberId)
    {
        var invoiceList = MiscInvoiceRepository.Get(invoice => invoice.BillingCategoryId == (int)BillingCategoryType.Uatp
                                                            && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling ||
                                                               invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed ||
                                                               invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete ||
                                                               invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented)
                                                            && invoice.BillingYear == billingYear
                                                            && invoice.BillingMonth == billingMonth
                                                            && invoice.BillingPeriod == billingPeriod);
        if (memberId > 0)
        {
            invoiceList = invoiceList.Where(u => u.BillingMemberId == memberId || u.BilledMemberId == memberId);
        }

        return invoiceList;
    }
    
    public IList<InvoiceBase> GetInvoiceEligibleMembers(int billingYear, int billingMonth, int billingPeriod, int billingCategory)
    {
      var invoiceRepository = Ioc.Resolve<IRepository<InvoiceBase>>(typeof(IRepository<InvoiceBase>));
      var invoices = invoiceRepository.Get(i => i.BillingYear == billingYear && i.BillingMonth == billingMonth && i.BillingPeriod == billingPeriod && i.BillingCategoryId == billingCategory).ToList();
      Ioc.Release(invoiceRepository);
      return invoices;
    }


      /// <summary>
      /// SCP#440318 - SRM: current stats screen is too slow
      /// </summary>
      /// <param name="localTimeZoneId"></param>
      /// <param name="pageSize"></param>
      /// <param name="pageNo"></param>
      /// <param name="sortColumn"></param>
      /// <param name="sortOrder"></param>
      /// <returns></returns>
    public IList<CompletedJobs> GetCompletedJobs(string localTimeZoneId, int? pageSize = null, int? pageNo = null, string sortColumn = null, string sortOrder = null)
    {

      string localtimezone = localTimeZoneId;
      TimeZoneInfo utctimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
      TimeZoneInfo selectedtimeZone = TimeZoneInfo.FindSystemTimeZoneById(localtimezone);

      var systemMonitorCompJob = SystemMonitorRepository.GetCompletedJobs(pageSize,pageNo,sortColumn,sortOrder);

      return systemMonitorCompJob;
    }


    public IList<PendingJobs> GetPendingJobs()
    {
      return SystemMonitorRepository.GetPendingJobs();
    }

    public IList<LoggedInUser> GetLoggedInUser()
    {
      return SystemMonitorRepository.GetLoggedInUser();
    }

    public IList<LoggedInUserByRegion> GetLoggedInUserByRegion()
    {
      return SystemMonitorRepository.GetLoggedInUserByRegion();
    }


    public IList<OutstandingItems> GetOutStandingItems()
    {
      return SystemMonitorRepository.GetOutStandingItems();
    }

    public IList<IsWebResponse> GetIsWebResponse()
    {
        return SystemMonitorRepository.GetIsWebResponse();
    }

    public IList<ProceesedFiles> GetProcessedDataFiles()
    {

      return SystemMonitorRepository.GetProcessedDataFiles();

    }


    public void InsertGenerateNilFormCMessageInOracleQueue(int memberId, string provisionalBillingMonth)
    {

      var samplingFormCManager = Ioc.Resolve<ISamplingFormCManager>(typeof(ISamplingFormCManager));


      // Update sample digit for the provisional billing month
      samplingFormCManager.UpdateSampleDigit(provisionalBillingMonth);


      // en-queue message
      IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "PROV_BILLING_MONTH", provisionalBillingMonth },
                                                                            { "MEMBER_ID", memberId.ToString() },
                                                                            { "REGENERATION_FLAG", 1.ToString() }
                                                                          };
      var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["GenerateNilFormCJobQueueName"].Trim());
      queueHelper.Enqueue(messages);

    }

    public void InsertProcessCVSMessageInOracleQueue(int memberId, BillingPeriod billingPeriod)
    {
      // en-queue message
      IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "BILLING_MEMBER_ID", memberId.ToString() },
                                                                            { "BILLING_PERIOD", billingPeriod.Period.ToString() },
                                                                            { "BILLING_MONTH", billingPeriod.Month.ToString() },
                                                                            { "BILLING_YEAR", billingPeriod.Year.ToString() },
                                                                            { "REGENERATE_FLAG", 1.ToString() }
                                                                          };
      var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["ProcessCSVQueueName"].Trim());
      queueHelper.Enqueue(messages);
    }




    public IList<ProceesedFiles> GetProcessedSupportingDoc()
    {
      return SystemMonitorRepository.GetProcessedSupportingDoc();
    }

    public void RevertAchRecapSheetData(BillingPeriod billingPeriod)
    {

      SystemMonitorRepository.RevertAchRecapSheetData(billingPeriod);
    }

    public Guid AddRequest(RequestLog request)
    {
      RequestLogRepository.Add(request);
      UnitOfWork.CommitDefault();
      return request.Id;
    }


    public IList<PendingInvoices> GetPendingInvoices(PendingInvoices pendingInvoices)
    {
      return SystemMonitorRepository.GetPendingInvoices(pendingInvoices);
    }


    public void UpdatePendingInvoiceStatus(Guid invoiceId)
    {
      var invoice = InvoiceBaseRepository.Single(i => i.Id == invoiceId); 

      if (invoice != null)
      {
        invoice.InvoiceStatus = InvoiceStatusType.Presented;
        invoice.PresentedStatus = InvoiceProcessStatus.Completed;
        invoice.PresentedStatusDate = DateTime.UtcNow;
        InvoiceBaseRepository.Update(invoice);
      }
      else
      {
        var formC = SamplingFormCRepository.Single(formc => formc.Id == invoiceId);
        if (formC != null)
        {
          formC.InvoiceStatus = InvoiceStatusType.Presented;
          formC.PresentedStatus = (int)InvoiceProcessStatus.Completed;
          formC.PresentedStatusDate = DateTime.UtcNow;
          SamplingFormCRepository.Update(formC);
        }
      }
      UnitOfWork.CommitDefault();

    }

    public IList<OutputFile> GetOutputFilesToResend(OutputFile outputFile)
    {
      //SCP # 46826 - Resend Output Files Screen takes too long to load/refresh
      var isInputFileList = new List<IsInputFile>();
      var outputFileList = new List<OutputFile>();
      if (outputFile != null)
      {
        if ((outputFile.FileSubmissionFrom != null) && (outputFile.FileSubmissionTo != null))
        {
          var fromdate = Convert.ToDateTime(outputFile.FileSubmissionFrom);
          var fileSubmissionFrom = new DateTime(fromdate.Year, fromdate.Month, fromdate.Day);

          var todate = Convert.ToDateTime(outputFile.FileSubmissionTo);
          var fileSubmissionto = new DateTime(todate.Year, todate.Month, todate.Day, 23, 59, 59);

          isInputFileList =
            IsInputFileRep.Get(
              f => f.IsIncoming == true && f.FileDate >= fileSubmissionFrom && f.FileDate <= fileSubmissionto ).ToList();
        }
      }

      if (isInputFileList.Count > 0)
      {
        var memberList = MemberRepository.GetAll().ToList();
        if (outputFile != null)
        {
          if (outputFile.FileMemberId > 0)
          {
            isInputFileList = isInputFileList.Where(f => f.SenderReceiver == outputFile.FileMemberId).ToList();
          }
          if (outputFile.ProvisionalBillingYear > 0)
          {
            isInputFileList = isInputFileList.Where(f => f.BillingYear == outputFile.ProvisionalBillingYear).ToList();
          }
          if (outputFile.ProvisionalBillingMonth > 0)
          {
            isInputFileList = isInputFileList.Where(f => f.BillingMonth == outputFile.ProvisionalBillingMonth).ToList();
          }
          if (outputFile.ProvisionalBillingPeriod > 0)
          {
            isInputFileList = isInputFileList.Where(f => f.BillingPeriod == outputFile.ProvisionalBillingPeriod).ToList();
          }
          if (!string.IsNullOrEmpty(outputFile.FileName))
          {
            isInputFileList = isInputFileList.Where(f => f.FileName.Contains(outputFile.FileName)).ToList();
          }
          if (outputFile.FileStatusId > 0)
          {
            isInputFileList = isInputFileList.Where(f => f.FileStatusId == outputFile.FileStatusId).ToList();
          }
          if (outputFile.FileFormatId > 0)
          {
            isInputFileList = isInputFileList.Where(f => f.FileFormatId == outputFile.FileFormatId).ToList();
          }
          //CMP#622: MISC Outputs Split as per Location IDs
          if (!string.IsNullOrEmpty(outputFile.MiscLocationCode))
          {
              isInputFileList = isInputFileList.Where(f => Equals(f.MiscLocationCode, outputFile.MiscLocationCode)).ToList();
          }
        }
        var fileStatusList = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager)).GetFileStatusList();
        outputFileList = (from f in isInputFileList
                              join m in memberList on Convert.ToInt32(f.SenderReceiver) equals m.Id into gj
                              join fs in fileStatusList on f.FileStatusId equals fs.Key
                              from member in gj.DefaultIfEmpty()
                              select
                                  new OutputFile
                                      {
                                        FileId = f.Id,
                                        FileFormatId = f.FileFormatId,
                                        FileMemberId = Convert.ToInt32(f.SenderReceiver),
                                        //SCP268330: BVC - Generated for invalid member
                                        FileMemberName = (member == null || member.Id == 0 ? String.Empty : (member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" + member.CommercialName)),
                                        ProvisionalBillingPeriod = f.BillingPeriod,
                                        ProvisionalBillingMonth = f.BillingMonth,
                                        ProvisionalBillingYear = f.BillingYear,
                                        FileStatusId = f.FileFormatId,
                                        FileDate = f.FileDate,
                                        FileSubmissionFrom = outputFile != null ? outputFile.FileSubmissionFrom : null,
                                        FileSubmissionTo = outputFile != null ? outputFile.FileSubmissionTo : null,
                                        FileName = f.FileName,
                                        //CMP#622: MISC Outputs Split as per Location IDs
                                        MiscLocationCode = f.MiscLocationCode,
                                        FileStatus = fs.Value.ToString(),
                                        FileFormat = f.FileFormat.ToString(),
                                        IsPurged = f.IsPurged
                                      }).ToList();
      }
      return outputFileList;
    }

    public IQueryable<IsInputFile> GetIsInputFilesByIds(string[] inputFilesIds)
    {
      List<Guid> fileIdList = (from f in inputFilesIds
                               select new Guid(f)).ToList();
      var isInputFilesList = IsInputFileRep.Get(f => fileIdList.Any(id => id == f.Id)).ToList();
      return isInputFilesList.AsQueryable();
    }

      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      /// <summary>
      /// Enqueue Daily Output
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="targetDate"></param>
      /// <param name="isXmlGeneration"></param>
      ///CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
      public void EnqueueDailyOutput(int memberId, DateTime targetDate, bool isXmlGeneration)
      {
          SystemMonitorRepository.EnqueueDailyOutput(memberId, targetDate, isXmlGeneration);
      }

      //CMP#622: MISC Outputs Split as per Location IDs
      /// <summary>
      /// Enqueue Daily Location Specific Output/OAR
      /// </summary>
      /// <param name="memberId"></param>
      /// <param name="targetDate"></param>
      /// <param name="isXmlGeneration"></param>
      /// <param name="locationCode"></param>
      /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
      public int EnqueueDailyLocationOutputOar(int memberId, DateTime targetDate, string locationCode, bool isXmlGeneration)
      {
          var result = SystemMonitorRepository.EnqueueDailyLocationOutputOar(memberId, targetDate, locationCode, isXmlGeneration);
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
      /// <param name="isXmlGeneration"></param>
      /// <param name="billingCategory"></param>
      public void EnqueueLocationOutputOar(int memberId, int isBilling, int billingYear, int billingMonth, int billingPeriod, int isReprocessing, int fileType, string locationCode, string fileGenerationDate, int isXmlGeneration, int billingCategory)
      {
          SystemMonitorRepository.FinalizeSupportingDocumentLocation(memberId, isBilling, billingYear, billingMonth, billingPeriod,
                                                           isReprocessing, fileType, locationCode, fileGenerationDate,
                                                           SystemParameters.Instance.General.CreateNilFileLocationSpecificMISCOutputs, 
                                                           isXmlGeneration, billingCategory);
      }
      //SCP#340872 - Issue in 'Daily IS-XML files for Receivables IS-WEB Invoices' output file
    public List<MiscellaneousConfiguration> GetIsWebMiscInvMemberList()
    {
      var memberList = MiscConfigurationRepository.Get(i => i.IsDailyXmlRequired).ToList();

      return memberList;
    }
  }
}
