using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.MiscUatp.Impl;
using Iata.IS.Business.Reports.Pax.Impl;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Cargo;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.Pax;
using Iata.IS.Model.Base;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.SupportingDocuments;
using log4net;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Data.SupportingDocuments;

using NVelocity;
using Iata.IS.DR.Business.Validation;
using Iata.IS.DR.Business.OfflineCollection.Pax;
using Iata.IS.Business.BroadcastMessages;

namespace Iata.IS.Business.Common.Impl
{
  public class InvoiceOfflineCollectionManager : IInvoiceOfflineCollectionManager
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private StringBuilder _errors;
    private UnitOfWork unitOfWork = new UnitOfWork(new ObjectContextAdapter());
    public ICalendarManager CalendarManager { private get; set; }
    //public BillingPeriod BillingPeriod { get; private set; }
    public const string ListingsFolderName = "LISTINGS";
    public const string MemosFolderName = "MEMOS";
    public const string SupportingDocumentFolderName = "SUPPDOCS";
    public const string EinvoicesFolderName = "E-INVOICE";
    private IList<OfflineCollectionFolderType> _offlineCollectionFolderTypes;
    private List<InvoiceOfflineCollectionMetaData> _metaDataRecords;
    public ISamplingFormCRepository SamplingFormCRepository { private get; set; }
    public ISupportingDocumentRepository SupportingDocumentRepository { private get; set; }
    public ISupportingDocumentManager SupportingDocumentManager { private get; set; }
    public ISupportingDocumentGenerator SupportingDocumentGenerator { private get; set; }
    public IMemoReportGenerator MemoReportGenerator { private get; set; }
    public IPaxListingsReportGenerator ListingsReportGenerator { private get; set; }
    public IInvoiceRepository InvoiceRepository { private get; set; }
    public IEinvoiceDocumentGenerator EInvoiceDocumentGenerator { private get; set; }
    public IMiscSupportingDocumentGenerator MiscSupportingDocumentGenerator { private get; set; }
    public IMiscListingReportGenerator MiscListingReportGenerator { private get; set; }
    public IMiscInvoiceRepository MiscInvoiceRepository { private get; set; }
    public IRepository<OfflineCollectionFolderType> OfflineCollectionFolderTypeRepository { private get; set; }
    public IRepository<InvoiceBase> InvoiceBaseRepository { private get; set; }
    public IRepository<InvoiceOfflineCollectionMetaData> InvoiceOfflineCollectionMetaDataRepository { private get; set; }
    public ICargoInvoiceRepository CargoInvoiceRepository { private get; set; }
    public ICgoListingsReportGenerator CgoListingsReportGenerator { private get; set; }
    public IRepository<SamplingFormDRecord> FormDReository { get; set; }
    private ICurrencyManager currencyManager;

    public void Init()
    {
      _errors = new StringBuilder();
      //BillingPeriod = _calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      // Fetch all offlineCollectionFolderTypes
      _offlineCollectionFolderTypes = OfflineCollectionFolderTypeRepository.GetAll().ToList();
    }

    /// <summary>
    /// This function is used to Set offlineCollectionsStatus of invoice to Success and also set the offline collection Datetime.
    /// </summary>
    /// <param name="invoiceId"></param>
    public void SetSuccessfullOfflineCollectionStausForInvoice(string invoiceId)
    {
      var isOutputProcessLogRep = new Repository<IsOutputProcessLog>(unitOfWork);
      if (isOutputProcessLogRep != null)
      {
        Guid invoice = ConvertUtil.ConvertStringtoGuid(invoiceId);
        var isOutputProcessLog = isOutputProcessLogRep.Get(op => op.Id == invoice).FirstOrDefault();
        if (isOutputProcessLog != null)
        {
          // Set offlineCollectionsStatus of invoice to Success and also set the offline collection datetime.
          isOutputProcessLog.OfflineCollectionStatus = 1;
          isOutputProcessLog.OfflineCollectionDate = DateTime.UtcNow;
          // Save Changes done.
          unitOfWork.Commit();
        }
      }
    }

    /// <summary>
    /// This function is used to re-queue message for offline line collection and send mail to sis ops if retry count maximum
    /// </summary>
    /// <param name="invoiceStatusId"></param>
    /// <param name="invoiceId"></param>
    /// <param name="billingCategoryId"></param>
    /// <param name="isReprocess"></param>
    /// <param name="ex"></param>
    //SCP#391022 KAL: Optimization in offline collection.
    public void RequeueMessage(int invoiceStatusId, string invoiceId, int billingCategoryId, int isReprocess, Exception ex = null)
    {
      Logger.InfoFormat("Re-queuing values. INVOICE_STATUS_ID:{0}, INVOICE_ID:{1}, BILLING_CATEGORY_ID:{2}, IS_REPROCESS:{3} ", invoiceStatusId, invoiceId, billingCategoryId, isReprocess);
      var isOutputProcessLogRep = new Repository<IsOutputProcessLog>(unitOfWork);
      if (isOutputProcessLogRep != null)
      {
        Guid invoice = ConvertUtil.ConvertStringtoGuid(invoiceId);
        var isOutputProcessLog = isOutputProcessLogRep.Get(op => op.Id == invoice).FirstOrDefault();
        int retryCount = -1;
        if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings.Get("MaxNoOfTimesToRequeue")))
          retryCount = Convert.ToInt32(ConfigurationManager.AppSettings.Get("MaxNoOfTimesToRequeue"));
        if (isOutputProcessLog != null && retryCount != -1 && isOutputProcessLog.OfflineCollectionRetryCount > retryCount)
        {
          // Set offlineCollectionsStatus of invoice to Failed.
          isOutputProcessLog.OfflineCollectionStatus = 2;

          //SCP#391022 KAL: Optimization in offline collection.
          /******************************Send mail to sis ops after maximum retry***********************************************************************/
          const string serviceName = "Offline Collection Generation Service";

          // Send an email notification to SIS Ops for an exception.
          var broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>(typeof(IBroadcastMessagesManager));
          broadcastMessagesManager.SendISAdminExceptionNotification(EmailTemplateId.ISAdminExceptionNotification, serviceName, ex);
          Ioc.Release(broadcastMessagesManager);

          /************************************************************************************************************/
          // Save Changes done.
          unitOfWork.Commit();
        }
        else if (isOutputProcessLog != null && retryCount != -1 && isOutputProcessLog.OfflineCollectionRetryCount < retryCount)
        {
          try
          {
            // enqueue message
            IDictionary<string, string> messages = new Dictionary<string, string> {
                { "RECORD_ID", invoiceId },{"STATUS_ID",invoiceStatusId.ToString()},{"IS_FORM_C","0"},{"BILLING_CATEGORY_ID",billingCategoryId.ToString()},{"IS_GEN_OFFL_ZIP","0"},{"BILLING_PERIOD","0"},{"BILLING_MONTH","0"},{"BILLING_YEAR","0"},{"MEMBER_ID","0"},{"REGENERATE_FLAG",isReprocess.ToString()}
            };
            var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["sourceQueueName"].Trim());

            //Change priority to -1 so that this invoice should be pick up first. 
            queueHelper.Enqueue(messages, -1, 0);

            // Increment retry count. 
            isOutputProcessLog.OfflineCollectionRetryCount++;
            // Set offlineCollectionsStatus of invoice to Requeued.
            isOutputProcessLog.OfflineCollectionStatus = 3;
            // Save Changes done.
            unitOfWork.Commit();

            Logger.Info("Enqueued values.");

          } // end try

          catch (Exception exception)
          {
            Logger.Error("Error occurred while adding message to queue.", exception);
            //SCP301427: FW: Admin Alert - Error in creating Legal Invoice Archive zip file of CDC Arkhinéo- SIS Production
            // Desc:- Exception wasn't getting thrown out
            throw;
          } // end catch
        }
      }
    }


    /// <summary>
    /// Creates the offline collection.
    /// </summary>
    /// <param name="invoiceOfflineCollectionMessage">The invoice offline collection meta data.</param>
    public void CreateOfflineCollection(InvoiceOfflineCollectionMessage invoiceOfflineCollectionMessage, InvoiceBase invoice)
    {
      try
      {
        // Initialize Ioc 
        //Ioc.Initialize();
        Init();
        // initialize errors to empty string
        _errors.Length = 0;
        // create InvoiceOfflineMetaData empty collection
        _metaDataRecords = new List<InvoiceOfflineCollectionMetaData>();
        Logger.Info("Offline collection folder types are fetched.");
        switch (invoiceOfflineCollectionMessage.BillingCategory)
        {
          case BillingCategoryType.Misc:
          case BillingCategoryType.Uatp:
            CreateMuOfflineCollection(invoiceOfflineCollectionMessage);
            break;
          case BillingCategoryType.Pax:
            CreatePaxOfflineCollection(invoiceOfflineCollectionMessage, invoice);
            break;
          case BillingCategoryType.Cgo:
            CreateCgoOfflineCollection(invoiceOfflineCollectionMessage);
            break;
        }
        _metaDataRecords.Clear();
        _metaDataRecords = null;
        _offlineCollectionFolderTypes.Clear();
        _offlineCollectionFolderTypes = null;
      }

      catch (Exception e)
      {
        _errors.AppendLine(string.Format("Exception occurred while generating reports for InvoiceId [{0}] ",
                                         invoiceOfflineCollectionMessage.StringRecordId));
        _errors.AppendLine(e.Message);
        Logger.InfoFormat("Exception occurred while generating reports for InvoiceId [{0}] ",
                                          invoiceOfflineCollectionMessage.StringRecordId);
        Logger.Info("Exception : ", e);
        // Exception is rethrown as message for which this exception is occurred should be rolled back and retry count will get incremented in Queue.
        throw;
      }

      // If _errors object contains any data, send an alert to Admin person.
      if (_errors.Length > 0)
      {
        Logger.InfoFormat("Sending alert for Exception : {0}", _errors.ToString());
        SendAlert(invoiceOfflineCollectionMessage);
      }
    }

    /// <summary>
    /// Creates the invoice meta data record.
    /// </summary>
    /// <param name="paxInvoice">The pax invoice.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="folderTypeName">Name of the folder type.</param>
    /// <param name="logger">The logger.</param>
    /// <returns></returns>
    private InvoiceOfflineCollectionMetaData CreateInvoiceMetaDataRecord(PaxInvoice paxInvoice, string filePath, string folderTypeName, ILog logger)
    {
      // Find OfflineCollectionFolderType object for parameter folderTypeName 
      logger.InfoFormat("Searching for [{0}] type in Invoice Offline Collection Folder Type Master...", folderTypeName);
      var offlineCollectionFolderType = _offlineCollectionFolderTypes.SingleOrDefault(ft => ft.Name.Equals(folderTypeName));
      if (offlineCollectionFolderType != null)
      {
        // Create InvoiceOfflineCollectionMetaData object for folder; parameter folderTypeName
        logger.InfoFormat("Create InvoiceOfflineCollectionMetaData object for folder [{0}] for FormC...", folderTypeName);
        return new InvoiceOfflineCollectionMetaData
        {
          PeriodNo = paxInvoice.BillingPeriod,
          BillingYear = paxInvoice.BillingYear,
          BillingMonth = paxInvoice.BillingMonth,
          BillingMemberCode = paxInvoice.BillingMember.MemberCodeNumeric,
          BilledMemberCode = paxInvoice.BilledMember.MemberCodeNumeric,
          OfflineCollectionFolderTypeId = offlineCollectionFolderType.Id,
          BillingCategoryId = (int)BillingCategoryType.Pax,
          FilePath = filePath,
          InvoiceNumber = paxInvoice.InvoiceNumber
        };
      }
      logger.InfoFormat("[{0}] folder type record could not find in database.", folderTypeName);
      return null;
    }

    /// <summary>
    /// Creates the form C meta data record.
    /// </summary>
    /// <param name="samplingFormC">The sampling form C.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="folderTypeName">Name of the folder type.</param>
    /// <param name="logger"></param>
    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    private InvoiceOfflineCollectionMetaData CreateFormCMetaDataRecord(SamplingFormC samplingFormC, string filePath, string folderTypeName, ILog logger, BillingPeriod billingPeriod)
    {
      // Find OfflineCollectionFolderType object for parameter folderTypeName 
      logger.InfoFormat("Searching for [{0}] type in Invoice Offline Collection Folder Type Master...", folderTypeName);
      var offlineCollectionFolderType = _offlineCollectionFolderTypes.SingleOrDefault(ft => ft.Name.Equals(folderTypeName));
      if (offlineCollectionFolderType != null)
      {
        // Create InvoiceOfflineCollectionMetaData object for folder; parameter folderTypeName
        logger.InfoFormat("Create InvoiceOfflineCollectionMetaData object for folder [{0}] for FormC...", folderTypeName);
        return new InvoiceOfflineCollectionMetaData
        {
          PeriodNo = billingPeriod.Period,
          BillingYear = billingPeriod.Year,
          BillingMonth = billingPeriod.Month,
          BillingMemberCode = samplingFormC.FromMember.MemberCodeNumeric,
          BilledMemberCode = samplingFormC.ProvisionalBillingMember.MemberCodeNumeric,
          OfflineCollectionFolderTypeId = offlineCollectionFolderType.Id,
          BillingCategoryId = (int)BillingCategoryType.Pax,
          FilePath = filePath,
          IsFormC = true,
          ProvisionalBillingMonth = samplingFormC.ProvisionalBillingMonth,
          ProvisionalBillingYear = samplingFormC.ProvisionalBillingYear
        };
      }
      else
      {
        logger.InfoFormat("[{0}] folder type record could not find in database.", folderTypeName);
        return null;
      }
    }

    /// <summary>
    /// Creates the misc invoice meta data record.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="filePath">The file path.</param>
    /// <param name="folderTypeName">Name of the folder type.</param>
    /// <param name="logger">The logger.</param>
    /// <returns></returns>
    private InvoiceOfflineCollectionMetaData CreateMuInvoiceMetaDataRecord(MiscUatpInvoice miscUatpInvoice, string filePath, string folderTypeName, ILog logger)
    {
      // Find OfflineCollectionFolderType object for parameter folderTypeName 
      logger.InfoFormat("Searching for [{0}] type in Invoice Offline Collection Folder Type Master...", folderTypeName);
      var offlineCollectionFolderType = _offlineCollectionFolderTypes.SingleOrDefault(ft => ft.Name.Equals(folderTypeName));
      if (offlineCollectionFolderType != null)
      {
        // Create InvoiceOfflineCollectionMetaData object for folder; parameter folderTypeName
        logger.InfoFormat("Create InvoiceOfflineCollectionMetaData object for folder [{0}] for Misc Invoice...", folderTypeName);
        return new InvoiceOfflineCollectionMetaData
        {
          PeriodNo = miscUatpInvoice.BillingPeriod,
          BillingYear = miscUatpInvoice.BillingYear,
          BillingMonth = miscUatpInvoice.BillingMonth,
          BillingMemberCode = miscUatpInvoice.BillingMember.MemberCodeNumeric,
          BilledMemberCode = miscUatpInvoice.BilledMember.MemberCodeNumeric,
          OfflineCollectionFolderTypeId = offlineCollectionFolderType.Id,
          BillingCategoryId = miscUatpInvoice.BillingCategoryId,
          InvoiceNumber = miscUatpInvoice.InvoiceNumber,
          FilePath = filePath
        };
      }
      else
      {
        logger.InfoFormat("[{0}] folder type record could not find in database.", folderTypeName);
        return null;
      }
    }

    /// <summary>
    /// Creates the Cgo invoice meta data record.
    /// </summary>
    /// <param name="cgoInvoice"></param>
    /// <param name="filePath"></param>
    /// <param name="folderTypeName"></param>
    /// <param name="logger"></param>
    /// <returns></returns>
    private InvoiceOfflineCollectionMetaData CreateCgoInvoiceMetaDataRecord(CargoInvoice cgoInvoice, string filePath, string folderTypeName, ILog logger)
    {
      // Find OfflineCollectionFolderType object for parameter folderTypeName 
      logger.InfoFormat("Searching for [{0}] type in Invoice Offline Collection Folder Type Master...", folderTypeName);
      var offlineCollectionFolderType = _offlineCollectionFolderTypes.SingleOrDefault(ft => ft.Name.Equals(folderTypeName));
      if (offlineCollectionFolderType != null)
      {
        // Create InvoiceOfflineCollectionMetaData object for folder; parameter folderTypeName
        logger.InfoFormat("Create InvoiceOfflineCollectionMetaData object for folder [{0}] ...", folderTypeName);
        return new InvoiceOfflineCollectionMetaData
        {
          PeriodNo = cgoInvoice.BillingPeriod,
          BillingYear = cgoInvoice.BillingYear,
          BillingMonth = cgoInvoice.BillingMonth,
          BillingMemberCode = cgoInvoice.BillingMember.MemberCodeNumeric,
          BilledMemberCode = cgoInvoice.BilledMember.MemberCodeNumeric,
          OfflineCollectionFolderTypeId = offlineCollectionFolderType.Id,
          BillingCategoryId = (int)BillingCategoryType.Cgo,
          FilePath = filePath,
          InvoiceNumber = cgoInvoice.InvoiceNumber
        };
      }
      logger.InfoFormat("[{0}] folder type record could not find in database.", folderTypeName);
      return null;
    }

    /// <summary>
    /// Creates Base Folder structure
    /// </summary>
    /// <param name="rootPath">The root path</param>
    /// <param name="billingMember"></param>
    /// <param name="billedMember"></param>
    /// <param name="billingCategory"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="invoiceNumber"></param>
    /// <param name="logger"></param>
    /// <param name="isFormC"></param>
    /// <param name="baseFolderlNameYear"></param>
    /// <param name="baseFolderlNameMonth"></param>
    /// <param name="baseFolderlNamePeriod"></param>
    /// <param name="requestViaIsWeb"></param>
    /// <returns></returns>
    public string CreateBaseFolderStructure(string rootPath, string billingMember, string billedMember, BillingCategoryType billingCategory,
                                            BillingPeriod billingPeriod, string invoiceNumber, ILog logger, bool isFormC,
                                            int baseFolderlNameYear = 0, int baseFolderlNameMonth = 0, int baseFolderlNamePeriod = 0, bool requestViaIsWeb = false, bool getSANBasePathFromDatabase = true)
    {
      // in root directory 1st : folder will be like YearMonthPeriod [\\rootFolder\20110301]
      logger.InfoFormat("Creating first level folder [YearMonthPeriod]...");
      string folderPath;
      // SCP162502  Form C - AC OAR Jul P3 failure - No alert received 
      // In case of Form C period will be 00.
      if (isFormC)
      {
        //folderPath = Path.Combine(rootPath, string.Format("{0}", GetFormattedBillingMonthYear(billingPeriod.Month, billingPeriod.Year, PaxReportConstants.PaxFormCReportDateFormat)));
        //SCP162502  Form C - AC OAR Jul P3 failure - No alert received
        // Using current output period as first level folder name in case of form C
        var billingPeriodForFormCFirstLevelFolderName = new BillingPeriod(baseFolderlNameYear, baseFolderlNameMonth, baseFolderlNamePeriod);
        //CMP599
        // Get OfflineCollection fo
        var rootFolderPath = requestViaIsWeb
          ? rootPath
          : FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR, billingPeriodForFormCFirstLevelFolderName);

        folderPath = Path.Combine(rootFolderPath, string.Format("{0}{1}", GetFormattedBillingMonthYear(billingPeriodForFormCFirstLevelFolderName.Month, billingPeriodForFormCFirstLevelFolderName.Year, PaxReportConstants.PaxReportFolderDataFormat), billingPeriodForFormCFirstLevelFolderName.Period.ToString().PadLeft(2, '0')));
      }
      else
      {
        // SCP#369538 - SRM: Daily ouputs are slow - Delivered on 16-May-2015.
        // Removed invOfflineColData cursor fetching. Instead path is build in C# code itself.
        var rootFolderPath = string.Empty;
        if (getSANBasePathFromDatabase)
        {
          /* get root folder path from data base SIS Admin San Path Config Table */
          rootFolderPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR, billingPeriod);
        }
        else
        {
          /* use root folder path from supplied parameter */
          rootFolderPath = rootPath;
        }
        folderPath = Path.Combine(rootFolderPath, string.Format("{0}{1}", GetFormattedBillingMonthYear(billingPeriod.Month, billingPeriod.Year, PaxReportConstants.PaxReportFolderDataFormat), billingPeriod.Period.ToString().PadLeft(2, '0')));
      }

      if (!Directory.Exists(folderPath)) CreateDirectoryOnly(folderPath);

      // 3rd : BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code [\\rootFolder\20110301\Offline Collection\PAX-0011-0022]
      logger.InfoFormat("Creating third level folder [BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code]...");
      folderPath = Path.Combine(folderPath, string.Format("{0}-{1}-{2}", billingCategory, billingMember, billedMember)).ToUpper();
      if (!Directory.Exists(folderPath)) CreateDirectoryOnly(folderPath);

      /* 4th : For FORM C : FORMC-YYYYMMPP
       *       For invoice : Invoice Number */
      logger.InfoFormat("Creating fourth level folder [{0}]...", isFormC ? "FORMC-YYYYMMPP" : "Invoice Number");
      //Fixed bug 2378 Offline Data Collection Folder Name
      folderPath = Path.Combine(folderPath, isFormC ? string.Format("FORMC-{0}", GetFormattedBillingMonthYear(billingPeriod.Month, billingPeriod.Year, PaxReportConstants.PaxFormCReportDateFormat)) : string.Format("INV-{0}", invoiceNumber)).ToUpper();
      if (!Directory.Exists(folderPath)) CreateDirectoryOnly(folderPath);
      return folderPath;
    }

    /// <summary>
    /// Gets the formatted billing month year.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <param name="year">The year.</param>
    /// <param name="dateFormat">The date format.</param>
    /// <returns></returns>
    public static string GetFormattedBillingMonthYear(int month, int year, string dateFormat)
    {
      try
      {
        // check if passed billingYear and billingMonth is valid,
        // and if yes, convert it to given DateFormat string
        return new DateTime(year, month, 1).ToString(dateFormat);
      }
      catch (Exception)
      {
        return string.Empty;
      }

    }

    /// <summary>
    /// Creates the E invoice.
    /// </summary>
    private void CreateEInvoice()
    {
      //TODO: Ipayable will implement this.  
    }

    /// <summary>
    /// Updates the offline collection meta data.
    /// </summary>
    /// <param name="logger">The logger.</param>
    private void UpdateOfflineCollectionMetaData(ILog logger)
    {
      if (_metaDataRecords == null || _metaDataRecords.Count <= 0)
      {
        logger.Info("No meta data records found to save.");
        return;
      }

      if (InvoiceOfflineCollectionMetaDataRepository == null)
      {
        logger.Info("Unable to create generic repository object to save InvoiceOfflineCollectionMetaData object.");
        return;
      }
      _metaDataRecords.Distinct(new MetaDataRecordComparer()).ToList().ForEach(InvoiceOfflineCollectionMetaDataRepository.Add);
      logger.Info("saving InvoiceOfflineCollectionMetaData objects...");
      UnitOfWork.CommitDefault();
    }

    /// <summary>
    /// System Monitor : Check count of Invoice Meta Data Record as per Invoice Number
    /// Author : Vinod Patil
    /// Date :24 Aug 2011
    /// </summary>
    /// <param name="invoiceNumber">string Invoice Number </param>
    /// <returns></returns>
    private bool CheckMetaDataRecordByInvoiceNumber(string invoiceNumber, int billingYear, int billingMonth, int billingPeriod)
    {
      bool returnType = false;

      if (_metaDataRecords != null)
      {



        //var invoicelist = InvoiceOfflineCollectionMetaDataRepository.Get(invoice => invoice.InvoiceNumber == invoiceNumber && invoice.BillingYear == billingYear && invoice.BillingMonth == billingMonth && invoice.PeriodNo == billingPeriod);  //_metaDataRecords.Where(u => u.InvoiceNumber == invoiceNumber);)
        //if (invoicelist.Count() >= 4)
        // returnType = false;
      }

      return returnType;
    }


    /// <summary>
    /// Sends the alert.
    /// </summary>
    private void SendAlert(InvoiceOfflineCollectionMessage invOfflineCollectionMessage)
    {
      var billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      //var currentBillingPeriodText = string.Format("{0} {1} P{2}", CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(billingPeriod.Month), billingPeriod.Year, billingPeriod.Period);

      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();

      var invoiceBase = InvoiceBaseRepository.Single(inv => inv.Id == invOfflineCollectionMessage.RecordId);

      // SCP118509: Admin Alert - Offline collection generation failure notification - SIS Production
      // Using invoice billing period insted of current open billing period.
      var currentBillingPeriodText = string.Format("{0} {1} P{2}",
                                                   invoiceBase != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(invoiceBase.BillingMonth) : string.Empty,
                                                   invoiceBase != null ? invoiceBase.BillingYear.ToString() : string.Empty,
                                                   invoiceBase != null ? invoiceBase.BillingPeriod.ToString() : string.Empty);

      context.Put("InvoiceNumber", invoiceBase != null ? invoiceBase.InvoiceNumber : string.Empty);
      context.Put("InvoiceId", invOfflineCollectionMessage.StringRecordId);
      context.Put("Period", currentBillingPeriodText);
      context.Put("ErrorMessage", _errors);
      var message = string.Format("Offline collection generation failure in period {0}{1}.",
                                  currentBillingPeriodText,
                                  (invoiceBase != null ? " for invoice number " + invoiceBase.InvoiceNumber : string.Empty));

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = message,
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
        Title = "Offline collection generation failure alert"
      };
      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminOfflineCollectionGenerationFailureAlert, context);
      Logger.Info("Sent IS Admin alert");
    }

    /// <summary>
    /// Creates the misc offline collection.
    /// </summary>
    /// <param name="invoiceOfflineCollectionMessage">The invoice offline collection message.</param>
    private void CreateMuOfflineCollection(InvoiceOfflineCollectionMessage invoiceOfflineCollectionMessage)
    {
      currencyManager = Ioc.Resolve<ICurrencyManager>(typeof(ICurrencyManager));

      var perfStopWatch = new Stopwatch();
      perfStopWatch.Start();
      Logger.Debug("Creating MiscInvoiceRepository object...");
      var stopWatch = new Stopwatch();
      Logger.Info("-----------------------------------------------------------------");
      Logger.Info("Fetching Misc invoice and related data for report SP-PROC_LOAD_MU_INVOICES function Initiated");
      stopWatch.Start();

      var muOfflineColManager = new DR.Business.OfflineCollection.MU.OfflineCollectionManager();

      var miscUatpInvoice = muOfflineColManager.GetInvoiceToGenerateOfflineCollection(invoiceOfflineCollectionMessage.StringRecordId);
      stopWatch.Stop();
      Logger.InfoFormat("Fetching Misc invoice and related data for report SP-PROC_LOAD_MU_INVOICES : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);

      if (miscUatpInvoice != null)
      {
        Logger.InfoFormat("Misc invoice & related data fetched for invoice Id [{0}]", invoiceOfflineCollectionMessage.StringRecordId);
        Logger.Info("Creating base folder structure based on Invoices period...");

        stopWatch.Reset();
        Logger.Info("-----------------------------------------------------------------");
        Logger.Info(" Create a base folder structure for MiscInvoice Listing report function Initiated");
        stopWatch.Start();



        // Create a base folder structure for MiscInvoice Listing report 
        string miscInvoiceFolderPath = CreateBaseFolderStructure(invoiceOfflineCollectionMessage.RootPath,
                                                                 miscUatpInvoice.BillingMember.MemberCodeNumeric,
                                                                 miscUatpInvoice.BilledMember.MemberCodeNumeric,
                                                                 miscUatpInvoice.BillingCategory,
                                                                 new BillingPeriod(miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth, miscUatpInvoice.BillingPeriod),
                                                                 miscUatpInvoice.InvoiceNumber,
                                                                 Logger,
                                                                 false);
        Logger.InfoFormat("Base folder structure [{0}] created.", miscInvoiceFolderPath);

        stopWatch.Stop();
        Logger.InfoFormat(" Create a base folder structure for MiscInvoice Listing report  : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);


        if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
        {
          // Create Misc Listing Report Generator Object
          Logger.Info("ReadyForBilling trigger fired");
          Logger.Debug("Creating MiscInvoiceRepository object...");

          if (MiscListingReportGenerator != null)
          {
            Logger.Info("MiscInvoiceRepository object created.");
            string miscInvoiceListingPath = Path.Combine(miscInvoiceFolderPath, ListingsFolderName);

            stopWatch.Reset();
            Logger.Info("-----------------------------------------------------------------");
            Logger.Info("Create Listings folder for MiscInvoice  function Initiated");
            stopWatch.Start();

            // Create Listings folder for MiscInvoice 
            if (!Directory.Exists(miscInvoiceListingPath)) CreateDirectoryOnly(miscInvoiceListingPath);
            Logger.InfoFormat("MiscInvoice Listings folder [{0}] created.", miscInvoiceListingPath);

            //CMP#588: Customized Listing Report.
            var isLayoutExist = new MiscListingManager().IsMiscInvoiceLayoutDefinationExist(miscUatpInvoice.Id);

            if (isLayoutExist && miscUatpInvoice.BillingCategoryId == (int)BillingCategorys.Miscellaneous)
            {
              var customizedListing = new CustomizedListing();
              IEnumerable<int> chargeCodeList = (from li in miscUatpInvoice.LineItems
                                                 orderby li.LineItemNumber ascending
                                                 select li.ChargeCodeId).Distinct();

              customizedListing.BuildListingReport(miscUatpInvoice.Id, miscUatpInvoice.InvoiceNumber, chargeCodeList.ToList(), miscInvoiceListingPath, _errors, Logger);
            }
            else
            {
              // Pass control to CreateMiscListing method of miscListingReportGenerator
              var billingCurrencyPrecision = miscUatpInvoice.BillingCategoryId == (int)BillingCategorys.Miscellaneous
                                               ? currencyManager.GetBillingCurrencyPrecision(
                                                 miscUatpInvoice.ListingCurrencyDisplayText)
                                               : 3;
              MiscListingReportGenerator.CreateMuListing(miscUatpInvoice, miscInvoiceListingPath, _errors, Logger,
                                                         billingCurrencyPrecision);
              Logger.Info("MiscInvoice Default Listings report generated.");
            }

            stopWatch.Stop();
            Logger.InfoFormat("Create Listings folder for MiscInvoice  : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);



            // Create MetaData object for Misc invoice Listings Folder
            var miscInvoiceMetaDataRecord = CreateMuInvoiceMetaDataRecord(miscUatpInvoice, miscInvoiceListingPath, ListingsFolderName, Logger);
            if (miscInvoiceMetaDataRecord != null) _metaDataRecords.Add(miscInvoiceMetaDataRecord);
            else _errors.AppendFormat("{0} folder type do not found in master table.", ListingsFolderName);
          }
          else
          {
            Logger.InfoFormat("Unable to create MiscListingReportGenerator object for invoice Id [{0}]", invoiceOfflineCollectionMessage.StringRecordId);
            _errors.AppendFormat("Unable to create MiscListingReportGenerator object for invoice Id [{0}]", invoiceOfflineCollectionMessage.StringRecordId);
          }


          stopWatch.Reset();
          Logger.Info("-----------------------------------------------------------------");
          Logger.Info("Create supporting document base folder path 'SUPPDOCS' & EinvoicesFolder   function Initiated");
          stopWatch.Start();

          // Create supporting document base folder path 'SUPPDOCS'
          string miscInvoiceSupportingDocumentPath = Path.Combine(miscInvoiceFolderPath, SupportingDocumentFolderName);
          if (!Directory.Exists(miscInvoiceSupportingDocumentPath)) CreateDirectoryOnly(miscInvoiceSupportingDocumentPath);
          Logger.InfoFormat("MiscInvoice Supporting Documents folder [{0}] created.", miscInvoiceSupportingDocumentPath);

          // Added code to optimize offline collection for misc invoice.
          if (miscUatpInvoice.SubmissionMethod == SubmissionMethod.IsWeb)
          {
            MiscSupportingDocumentGenerator.CreateMiscSupportingDocument(miscUatpInvoice, miscInvoiceSupportingDocumentPath, _errors, Logger);
          }
          // Create MetaData object for Misc invoice Supporting documents Folder
          var miscInvoiceSuppDocMetaDataRecord = CreateMuInvoiceMetaDataRecord(miscUatpInvoice, miscInvoiceSupportingDocumentPath, SupportingDocumentFolderName, Logger);

          if (miscInvoiceSuppDocMetaDataRecord != null) _metaDataRecords.Add(miscInvoiceSuppDocMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in master table.", SupportingDocumentFolderName);

          // Create "EinvoicesFolder" for EInvoices Documents
          string eInvoiceDocsPath = Path.Combine(miscInvoiceFolderPath, EinvoicesFolderName);
          if (!Directory.Exists(eInvoiceDocsPath)) CreateDirectoryOnly(eInvoiceDocsPath);
          Logger.InfoFormat("EInvoice Documents folder [{0}] created", eInvoiceDocsPath);
          // Create MetaData object for EInvoice Documents and add it to collection for final commit
          var eInvoiceMetaDataRecord = CreateMuInvoiceMetaDataRecord(miscUatpInvoice, eInvoiceDocsPath, EinvoicesFolderName, Logger);
          if (eInvoiceMetaDataRecord != null) _metaDataRecords.Add(eInvoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in mater table.", EinvoicesFolderName);
          stopWatch.Stop();
          Logger.InfoFormat("Create supporting document base folder path 'SUPPDOCS' & EinvoicesFolder   : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);

          // System Monitor 
          // Author : Vinod Patil
          // Date : 24 Aug 2011
          // Purpose: Insert Meta Data Record entry incase no record found for PAX invoice Number
          if (invoiceOfflineCollectionMessage.IsReGenerate)
          {
            // ReGeneration Process
            if (!CheckMetaDataRecordByInvoiceNumber(miscUatpInvoice.InvoiceNumber, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth, miscUatpInvoice.BillingPeriod))
            {
              // Save MetaData objects for invoice
              UpdateOfflineCollectionMetaData(Logger);
            }
          }
          else
          {
            // Normal Process
            // Save MetaData objects for invoice
            UpdateOfflineCollectionMetaData(Logger);
          }

          Logger.Info("Misc Invoice record meta data saved.");


          /* If the invoice status is processing complete,
         * check for unlinked documents, if any and link it to invoice, so that when we 
         * fetch attachment data along with invoice, we will get updated data.*/
          #region Linked - Unlinked documents lying in ISA, if any

          LinkUnlinkedDocuments(invoiceOfflineCollectionMessage);
          #endregion
        }// If MiscInvoice status is ProcessingComplete, then generate supporting documents from Misc 
        else if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
        {
          Logger.Info("ProcessingComplete trigger fired");

          stopWatch.Reset();
          Logger.Info("-----------------------------------------------------------------");
          Logger.Info("MiscInvoice status is ProcessingComplete, then generate supporting documents from Misc  function Initiated");
          stopWatch.Start();


          // Create MiscSupportingDocumentGenerator object
          Logger.Debug("Creating MiscSupportingDocumentGenerator' object...");

          if (MiscSupportingDocumentGenerator != null)
          {
            Logger.Info("MiscSupportingDocumentGenerator' object created.");


            // Create supporting document base folder path 'SUPPDOCS'
            string miscInvoiceSupportingDocumentPath = Path.Combine(miscInvoiceFolderPath, SupportingDocumentFolderName);
            if (!Directory.Exists(miscInvoiceSupportingDocumentPath)) CreateDirectoryOnly(miscInvoiceSupportingDocumentPath);
            Logger.InfoFormat("MiscInvoice Supporting Documents folder [{0}] created.", miscInvoiceSupportingDocumentPath);

            // Create Misc Supporting Documents...
            MiscSupportingDocumentGenerator.CreateMiscSupportingDocument(miscUatpInvoice, miscInvoiceSupportingDocumentPath, _errors, Logger);
            Logger.InfoFormat("Coping MiscInvoice Supporting Documents completed.");

            //Generate EInvoices documents
            // Create "EinvoicesFolder" for EInvoices Documents
            string eInvoiceDocsPath = Path.Combine(miscInvoiceFolderPath, EinvoicesFolderName);
            if (!Directory.Exists(eInvoiceDocsPath)) CreateDirectoryOnly(eInvoiceDocsPath);
            Logger.InfoFormat("EInvoice Documents folder [{0}] created", eInvoiceDocsPath);

            EInvoiceDocumentGenerator.CreateEinvoiceDocuments(miscUatpInvoice, eInvoiceDocsPath);
            Logger.InfoFormat("Coping EInvoice Documents completed.");
          }
          else
          {
            _errors.Append("Unable to create MiscSupportingDocumentGenerator object.");
            Logger.InfoFormat("Unable to create MiscSupportingDocumentGenerator object.");
          }

          stopWatch.Stop();
          Logger.InfoFormat("MiscInvoice status is ProcessingComplete, then generate supporting documents from Misc    : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
        }
        miscUatpInvoice = null;
      }
      else
      {
        Logger.InfoFormat("Misc Invoice data not found for invoice Id [{0}]", invoiceOfflineCollectionMessage.StringRecordId);
        _errors.AppendFormat("Misc Invoice data not found for invoice Id [{0}]", invoiceOfflineCollectionMessage.StringRecordId);
      }
      perfStopWatch.Stop();
      Logger.InfoFormat("Offline collection generation of invoice completed for MiscUatp invoice id [{0}] Time elapsed [{1}]", invoiceOfflineCollectionMessage.StringRecordId, perfStopWatch.Elapsed);
    }


    /// <summary>
    /// Creates the cgo offline collection.
    /// </summary>
    /// <param name="invoiceOfflineCollectionMessage">The invoice offline collection message.</param>
    private void CreateCgoOfflineCollection(InvoiceOfflineCollectionMessage invoiceOfflineCollectionMessage)
    {
      var stopWatch = new Stopwatch();
      stopWatch.Start();
      Logger.InfoFormat("Fetching invoice offline collection data for InvoiceId [{0}] record...", invoiceOfflineCollectionMessage.StringRecordId);

      var cargoInvoice = new DR.Business.OfflineCollection.Cargo.OfflineCollectionManager().GetInvoiceOfflineCollectionData(invoiceOfflineCollectionMessage.StringRecordId);

      if (cargoInvoice != null)
      {
        Logger.Info("Offline collection data for cargo invoice fetched.");
        Logger.Info("Creating base folder structure based on Cargo Invoices period...");
        // Create Base Folder Structure for CgoInvoice
        string cgoInvoiceFolderPath = CreateBaseFolderStructure(invoiceOfflineCollectionMessage.RootPath,
                                                             cargoInvoice.BillingMember.MemberCodeNumeric,
                                                             cargoInvoice.BilledMember.MemberCodeNumeric,
                                                             cargoInvoice.BillingCategory,
                                                             new BillingPeriod(cargoInvoice.BillingYear, cargoInvoice.BillingMonth, cargoInvoice.BillingPeriod),
                                                             cargoInvoice.InvoiceNumber, Logger, false);
        Logger.InfoFormat("Base folder structure [{0}] created.", cgoInvoiceFolderPath);

        // If invoice status is ReadyForBilling, Generate all required reports 
        if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
        {
          Logger.Info("ReadyForBilling trigger fired.");

          // Create "LISTINGS" folder for Cgo invoice listing reports
          string invoiceListingPath = Path.Combine(cgoInvoiceFolderPath, ListingsFolderName);
          if (!Directory.Exists(invoiceListingPath)) Directory.CreateDirectory(invoiceListingPath);
          Logger.InfoFormat("Invoice Listings folder [{0}] created", invoiceListingPath);

          // Generate Cgo Listing Reports
          CgoListingsReportGenerator.CreateCgoListings(cargoInvoice, invoiceListingPath, _errors,
                                                       Logger);
          Logger.InfoFormat("Invoice Listings reports generated.");



          // Create MetaData object for invoice listing reports and add it to collection for final commit
          var invoiceMetaDataRecord = CreateCgoInvoiceMetaDataRecord(cargoInvoice, invoiceListingPath, ListingsFolderName, Logger);
          if (invoiceMetaDataRecord != null) _metaDataRecords.Add(invoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in master table.", ListingsFolderName);


          // Create "MEMOS" folder for memo files
          Logger.Debug("Creating MemoReportGenerator object...");
          string invoiceMemoPath = Path.Combine(cgoInvoiceFolderPath, MemosFolderName);
          if (!Directory.Exists(invoiceMemoPath)) Directory.CreateDirectory(invoiceMemoPath);
          Logger.InfoFormat("Cargo Invoice Memos folder [{0}] created", invoiceMemoPath);

          // Generate Memos
          MemoReportGenerator.CreateMemoReports(cargoInvoice, invoiceMemoPath, _errors);
          Logger.InfoFormat("Cargo Invoice Memos generated.");

          // Create MetaData object for Memos and add it to collection for final commit
          invoiceMetaDataRecord = CreateCgoInvoiceMetaDataRecord(cargoInvoice, invoiceMemoPath, MemosFolderName, Logger);
          if (invoiceMetaDataRecord != null) _metaDataRecords.Add(invoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in master table.", MemosFolderName);


          // Create "SUPPDOCS" folder for Invoice Supporting Documents
          string invoiceSuppDocsPath = Path.Combine(cgoInvoiceFolderPath, SupportingDocumentFolderName);
          if (!Directory.Exists(invoiceSuppDocsPath)) Directory.CreateDirectory(invoiceSuppDocsPath);
          Logger.InfoFormat("Invoice Supporting Documents folder [{0}] created", invoiceSuppDocsPath);

          // Create MetaData object for Invoice Supporting Documents and add it to collection for final commit
          var suppInvoiceMetaDataRecord = CreateCgoInvoiceMetaDataRecord(cargoInvoice, invoiceSuppDocsPath, SupportingDocumentFolderName, Logger);
          if (suppInvoiceMetaDataRecord != null) _metaDataRecords.Add(suppInvoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in matser table.", SupportingDocumentFolderName);

          // Added code to copy supp docs to Invoice Supporting Documents folder.
          if ((cargoInvoice.SubmissionMethod == SubmissionMethod.IsWeb))
          {
            SupportingDocumentGenerator.CreateSupportingDocument(cargoInvoice, invoiceSuppDocsPath, _errors, Logger);
          }
          // Create "EinvoicesFolder" for EInvoices Documents
          string eInvoiceDocsPath = Path.Combine(cgoInvoiceFolderPath, EinvoicesFolderName);
          if (!Directory.Exists(eInvoiceDocsPath)) Directory.CreateDirectory(eInvoiceDocsPath);
          Logger.InfoFormat("EInvoice Documents folder [{0}] created", eInvoiceDocsPath);

          // Create MetaData object for EInvoice Documents and add it to collection for final commit
          var eInvoiceMetaDataRecord = CreateCgoInvoiceMetaDataRecord(cargoInvoice, eInvoiceDocsPath, EinvoicesFolderName, Logger);
          if (eInvoiceMetaDataRecord != null) _metaDataRecords.Add(eInvoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in mater table.", EinvoicesFolderName);

          /* If the invoice status is processing complete,
          * check for unlinked documents, if any and link it to invoice, so that when we 
          * fetch attachment data along with invoice, we will get updated data.*/

        }// If InvoiceStatus is ProcessingComplete, Generate supporting documents for CgoInvoice
        else if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
        {
          Logger.Info("ProcessingComplete trigger fired.");

          // Create "SUPPDOCS" folder for Invoice Supporting Documents
          string invoiceSuppDocsPath = Path.Combine(cgoInvoiceFolderPath, SupportingDocumentFolderName);
          if (!Directory.Exists(invoiceSuppDocsPath)) Directory.CreateDirectory(invoiceSuppDocsPath);
          Logger.InfoFormat("Invoice Supporting Documents folder [{0}] created", invoiceSuppDocsPath);

          SupportingDocumentGenerator.CreateSupportingDocument(cargoInvoice, invoiceSuppDocsPath, _errors, Logger);
          //// Create MetaData object for Invoice Supporting Documents and add it to collection for final commit
          //var invoiceMetaDataRecord = CreateCgoInvoiceMetaDataRecord(cargoInvoice, invoiceSuppDocsPath, SupportingDocumentFolderName, Logger);
          //if (invoiceMetaDataRecord != null) _metaDataRecords.Add(invoiceMetaDataRecord);
          //else _errors.AppendFormat("{0} folder type do not found in mater table.", SupportingDocumentFolderName);

          //Generate EInvoices documents
          // Create "EinvoicesFolder" for EInvoices Documents
          string eInvoiceDocsPath = Path.Combine(cgoInvoiceFolderPath, EinvoicesFolderName);
          if (!Directory.Exists(eInvoiceDocsPath)) Directory.CreateDirectory(eInvoiceDocsPath);
          Logger.InfoFormat("EInvoice Documents folder [{0}] created", eInvoiceDocsPath);

          EInvoiceDocumentGenerator.CreateEinvoiceDocuments(cargoInvoice, eInvoiceDocsPath);
          Logger.InfoFormat("Coping EInvoice Documents completed.");

        }

        // System Monitor 
        // Purpose: Insert Meta Data Record entry incase no record found for Cgo invoice Number
        if (invoiceOfflineCollectionMessage.IsReGenerate)
        {
          // ReGeneration Process
          if (!CheckMetaDataRecordByInvoiceNumber(cargoInvoice.InvoiceNumber, cargoInvoice.BillingYear, cargoInvoice.BillingMonth, cargoInvoice.BillingPeriod))
          {
            // Save MetaData objects for invoice
            UpdateOfflineCollectionMetaData(Logger);
          }
        }
        else
        {
          // Normal Process
          // Save MetaData objects for invoice
          UpdateOfflineCollectionMetaData(Logger);
        }

        Logger.Info("Invoice record meta data saved.");

        #region Linked - Unlinked documents lying in ISA, if any
        LinkUnlinkedDocuments(invoiceOfflineCollectionMessage);

        #endregion
      }
      else
      {
        Logger.InfoFormat("Invoice record not found for invoice id [{0}].", invoiceOfflineCollectionMessage.StringRecordId);
        _errors.AppendFormat("Invoice record not found for invoice id [{0}].", invoiceOfflineCollectionMessage.StringRecordId);
      }

      stopWatch.Stop();
      Logger.InfoFormat("Offline collection generation of invoice completed for cargo invoice id [{0}] Time elapsed [{1}]", invoiceOfflineCollectionMessage.StringRecordId, stopWatch.Elapsed);

    }



    /// <summary>
    /// Creates the pax offline collection.
    /// </summary>
    /// <param name="invoiceOfflineCollectionMessage">The invoice offline collection message.</param>
    private void CreatePaxOfflineCollection(InvoiceOfflineCollectionMessage invoiceOfflineCollectionMessage, InvoiceBase invoice)
    {
      var perfStopWatch = new Stopwatch();
      perfStopWatch.Start();
      Logger.InfoFormat("Fetching invoice offline collection data for InvoiceId [{0}] record...", invoiceOfflineCollectionMessage.StringRecordId);

      var stopWatch = new Stopwatch();
      Logger.Info("-----------------------------------------------------------------");
      Logger.Info("Get Invoice Offline Collection Data SP-PROC_LOAD_PAX_INVOICE function Initiated");
      stopWatch.Start();
      var offlineColManager = new OfflineCollectionManager();

      PaxInvoice paxInvoice = (invoice != null && (invoice.SubmissionMethod == SubmissionMethod.IsWeb || invoice.SubmissionMethod == SubmissionMethod.AutoBilling))
                     ? InvoiceRepository.GetInvoiceOfflineCollectionDataWithISWebAttachments(
                       invoiceOfflineCollectionMessage.StringRecordId)
                     : offlineColManager.GetInvoiceOfflineCollectionData(invoiceOfflineCollectionMessage.StringRecordId);

      //if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
      //{
      //  paxInvoice = InvoiceRepository.GetInvoiceOfflineCollectionDataWithISWebAttachments(invoiceOfflineCollectionMessage.StringRecordId);
      //}
      //else
      //{
      //  paxInvoice = InvoiceRepository.GetInvoiceOfflineCollectionData(invoiceOfflineCollectionMessage.StringRecordId);
      //}

      stopWatch.Stop();
      Logger.InfoFormat("Get Invoice Offline Collection Data SP - PROC_LOAD_PAX_INVOICE : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
      Logger.Info("-----------------------------------------------------------------");


      if (paxInvoice != null)
      {
        Logger.Info("Offline collection data for paxInvoice fetched.");
        Logger.Info("Creating base folder structure based on Invoices period...");
        // Create Base Folder Structure for PaxInvoice

        stopWatch.Reset();

        Logger.Info("-----------------------------------------------------------------");
        Logger.Info("Create Base Folder function Initiated");
        stopWatch.Start();

        string invoiceFolderPath = CreateBaseFolderStructure(invoiceOfflineCollectionMessage.RootPath,
                                             paxInvoice.BillingMember.MemberCodeNumeric,
                                                             paxInvoice.BilledMember.MemberCodeNumeric,
                                                             paxInvoice.BillingCategory,
                                                           new BillingPeriod(paxInvoice.BillingYear, paxInvoice.BillingMonth, paxInvoice.BillingPeriod),
                                                             paxInvoice.InvoiceNumber,
                                                             Logger,
                                                             false);

        stopWatch.Stop();
        Logger.InfoFormat("Create Base Folder function Initiated : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
        Logger.Info("-----------------------------------------------------------------");

        Logger.InfoFormat("Base folder structure [{0}] created.", invoiceFolderPath);




        // If invoice status is ReadyForBilling, Generate all required reports 
        if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
        {
          Logger.Info("ReadyForBilling trigger fired.");

          // Create "LISTINGS" folder for invoice listing reports
          string invoiceListingPath = Path.Combine(invoiceFolderPath, ListingsFolderName);
          if (!Directory.Exists(invoiceListingPath)) CreateDirectoryOnly(invoiceListingPath);
          Logger.InfoFormat("Invoice Listings folder [{0}] created", invoiceListingPath);
          stopWatch.Stop();
          Logger.InfoFormat("Create Base Folder function Initiated : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
          Logger.Info("-----------------------------------------------------------------");

          Logger.InfoFormat("Base folder structure [{0}] created.", invoiceFolderPath);

          Logger.Info("-----------------------------------------------------------------");
          stopWatch.Reset();
          Logger.Info("Generate Pax Listing Reports function Initiated");
          stopWatch.Start();

          // Generate Pax Listing Reports
          Logger.InfoFormat("Fetch Coupon count {0}", paxInvoice.CouponDataRecord.Count);
          ListingsReportGenerator.CreatePaxListings(paxInvoice, invoiceListingPath, _errors, Logger, paxInvoice.CouponDataRecord);
          Logger.InfoFormat("Invoice Listings reports generated.");

          stopWatch.Stop();
          Logger.InfoFormat("Generate Pax Listing Reports : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
          Logger.Info("-----------------------------------------------------------------");
          stopWatch.Reset();
          Logger.Info("Generate Pax details pdf Reports function Initiated");
          stopWatch.Start();

          // Generate Pax details pdf
          ListingsReportGenerator.CreatePaxDetailsPdf(invoiceOfflineCollectionMessage.StringRecordId, invoiceListingPath, _errors, Logger, paxInvoice);
          Logger.InfoFormat("Invoice Detials pdf generated.");


          stopWatch.Stop();
          Logger.InfoFormat("Generate Pax details pdf Reports : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
          Logger.Info("-----------------------------------------------------------------");


          //stopWatch.Stop();
          //Logger.InfoFormat("Generate Pax Listing Reports : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
          //Logger.Info("-----------------------------------------------------------------");
          //stopWatch.Reset();
          //Logger.Info("Generate Pax details pdf Reports function Initiated");
          //stopWatch.Start();

          // Generate Pax details pdf
          //ListingsReportGenerator.CreatePaxDetailsPdf(invoiceOfflineCollectionMessage.StringRecordId, invoiceListingPath, _errors, Logger, paxInvoiceCoupons);
          //Logger.InfoFormat("Invoice Detials pdf generated.");

          //paxInvoiceCoupons.Clear();
          //paxInvoiceCoupons = null;

          // Create MetaData object for invoice listing reports and add it to collection for final commit
          var invoiceMetaDataRecord = CreateInvoiceMetaDataRecord(paxInvoice, invoiceListingPath, ListingsFolderName, Logger);
          if (invoiceMetaDataRecord != null) _metaDataRecords.Add(invoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in mater table.", ListingsFolderName);
          Logger.Debug("Creating MemoReportGenerator object...");

          if (paxInvoice.BillingCode != 5) //NOTE  : For Form D/E there are no Memos
          {
            // Create "MEMOS" folder for memo files
            string invoiceMemoPath = Path.Combine(invoiceFolderPath, MemosFolderName);
            if (!Directory.Exists(invoiceMemoPath)) CreateDirectoryOnly(invoiceMemoPath);
            Logger.InfoFormat("Invoice Memos folder [{0}] created", invoiceMemoPath);

            Logger.Info("-----------------------------------------------------------------");
            stopWatch.Reset();
            Logger.Info("Generate Memos Reports function Initiated");
            stopWatch.Start();
            // Generate Memos
            MemoReportGenerator.CreateMemoReports(paxInvoice, invoiceMemoPath, _errors);
            Logger.InfoFormat("Invoice Memos generated.");

            stopWatch.Stop();
            Logger.InfoFormat("Generate Memos Reports : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
            Logger.Info("-----------------------------------------------------------------");


            // Create MetaData object for Memos and add it to collection for final commit
            invoiceMetaDataRecord = CreateInvoiceMetaDataRecord(paxInvoice, invoiceMemoPath, MemosFolderName, Logger);
            if (invoiceMetaDataRecord != null) _metaDataRecords.Add(invoiceMetaDataRecord);
            else _errors.AppendFormat("{0} folder type do not found in mater table.", MemosFolderName);
          }

          // Create "SUPPDOCS" folder for Invoice Supporting Documents
          string invoiceSuppDocsPath = Path.Combine(invoiceFolderPath, SupportingDocumentFolderName);
          if (!Directory.Exists(invoiceSuppDocsPath)) CreateDirectoryOnly(invoiceSuppDocsPath);
          Logger.InfoFormat("Invoice Supporting Documents folder [{0}] created", invoiceSuppDocsPath);


          // Create MetaData object for Invoice Supporting Documents and add it to collection for final commit
          var suppInvoiceMetaDataRecord = CreateInvoiceMetaDataRecord(paxInvoice, invoiceSuppDocsPath, SupportingDocumentFolderName, Logger);
          if (suppInvoiceMetaDataRecord != null) _metaDataRecords.Add(suppInvoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in mater table.", SupportingDocumentFolderName);

          // Added code to optimize offline collection for pax invoice.
          if ((paxInvoice.SubmissionMethod == SubmissionMethod.IsWeb) || (paxInvoice.SubmissionMethod == SubmissionMethod.AutoBilling))
          {
            SupportingDocumentGenerator.CreateSupportingDocument(paxInvoice, invoiceSuppDocsPath, _errors, Logger);
          }

          // Create "EinvoicesFolder" for EInvoices Documents
          string eInvoiceDocsPath = Path.Combine(invoiceFolderPath, EinvoicesFolderName);
          if (!Directory.Exists(eInvoiceDocsPath)) CreateDirectoryOnly(eInvoiceDocsPath);
          Logger.InfoFormat("EInvoice Documents folder [{0}] created", eInvoiceDocsPath);



          // Create MetaData object for EInvoice Documents and add it to collection for final commit
          var eInvoiceMetaDataRecord = CreateInvoiceMetaDataRecord(paxInvoice, eInvoiceDocsPath, EinvoicesFolderName, Logger);
          if (eInvoiceMetaDataRecord != null) _metaDataRecords.Add(eInvoiceMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in mater table.", EinvoicesFolderName);

          /* If the invoice status is processing complete,
          * check for unlinked documents, if any and link it to invoice, so that when we 
          * fetch attachment data along with invoice, we will get updated data.*/

          //// 3rd : BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code [\\rootFolder\20110301\Offline Collection\PAX-0011-0022]
          //logger.InfoFormat("Creating third level folder [BillingCategory-Billing Member Numeric Code-Billed Member Numeric Code]...");
          //folderPath = Path.Combine(folderPath, string.Format("{0}-{1}-{2}", billingCategory, billingMember, billedMember)).ToUpper();
          //if (!Directory.Exists(folderPath)) CreateDirectoryOnly(folderPath);

        } // If InvoiceStatus is ProcessingComplete, Generate supporting documents for PaxInvoice

        else if (invoiceOfflineCollectionMessage.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
        {
          Logger.Info("ProcessingComplete trigger fired.");

          Logger.Info("-----------------------------------------------------------------");
          stopWatch.Reset();
          Logger.Info("Invoice Supporting Documents function Initiated");
          stopWatch.Start();

          Logger.Info("-----------------------------------------------------------------");
          stopWatch.Reset();
          Logger.Info("Invoice Supporting Documents function Initiated");
          stopWatch.Start();

          // Create "SUPPDOCS" folder for Invoice Supporting Documents
          string invoiceSuppDocsPath = Path.Combine(invoiceFolderPath, SupportingDocumentFolderName);
          if (!Directory.Exists(invoiceSuppDocsPath)) CreateDirectoryOnly(invoiceSuppDocsPath);
          Logger.InfoFormat("Invoice Supporting Documents folder [{0}] created", invoiceSuppDocsPath);

          SupportingDocumentGenerator.CreateSupportingDocument(paxInvoice, invoiceSuppDocsPath, _errors, Logger);

          stopWatch.Stop();
          Logger.InfoFormat("Invoice Supporting Documents : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);

          stopWatch.Stop();
          Logger.InfoFormat("Invoice Supporting Documents : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);

          Logger.Info("-----------------------------------------------------------------");
          stopWatch.Reset();
          Logger.Info("EInvoice Documents function Initiated");
          stopWatch.Start();

          //Generate EInvoices documents
          // Create "EinvoicesFolder" for EInvoices Documents
          string eInvoiceDocsPath = Path.Combine(invoiceFolderPath, EinvoicesFolderName);
          if (!Directory.Exists(eInvoiceDocsPath)) CreateDirectoryOnly(eInvoiceDocsPath);
          Logger.InfoFormat("EInvoice Documents folder [{0}] created", eInvoiceDocsPath);

          EInvoiceDocumentGenerator.CreateEinvoiceDocuments(paxInvoice, eInvoiceDocsPath);
          Logger.InfoFormat("Coping EInvoice Documents completed.");


          stopWatch.Stop();
          Logger.InfoFormat("EInvoice Documents : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
          Logger.Info("-----------------------------------------------------------------");

        }

        // System Monitor 
        // Author : Vinod Patil
        // Date : 24 Aug 2011
        // Purpose: Insert Meta Data Record entry incase no record found for PAX invoice Number
        if (invoiceOfflineCollectionMessage.IsReGenerate)
        {
          // ReGeneration Process
          if (!CheckMetaDataRecordByInvoiceNumber(paxInvoice.InvoiceNumber, paxInvoice.BillingYear, paxInvoice.BillingMonth, paxInvoice.BillingPeriod))
          {
            // Save MetaData objects for invoice
            UpdateOfflineCollectionMetaData(Logger);
          }
        }
        else
        {
          // Normal Process
          // Save MetaData objects for invoice
          Logger.Info("-----------------------------------------------------------------");
          stopWatch.Reset();
          Logger.Info("UpdateOfflineCollectionMetaData function Initiated");
          stopWatch.Start();
          UpdateOfflineCollectionMetaData(Logger);
          stopWatch.Stop();
          Logger.InfoFormat("UpdateOfflineCollectionMetaData function : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
          Logger.Info("-----------------------------------------------------------------");
        }

        Logger.Info("Invoice record meta data saved.");
        #region Linked - Unlinked documents lying in ISA, if any
        Logger.Info("-----------------------------------------------------------------");
        stopWatch.Reset();
        Logger.Info("LinkUnlinkedDocuments function Initiated");
        stopWatch.Start();
        LinkUnlinkedDocuments(invoiceOfflineCollectionMessage);
        stopWatch.Stop();
        Logger.InfoFormat("LinkUnlinkedDocuments function : Time Required: [{0}]", stopWatch.ElapsedMilliseconds);
        Logger.Info("-----------------------------------------------------------------");

        #endregion

        paxInvoice = null;
        invoiceFolderPath = null;
      }
      else
      {
        Logger.InfoFormat("Invoice record not found for invoice id [{0}].", invoiceOfflineCollectionMessage.StringRecordId);
        _errors.AppendFormat("Invoice record not found for invoice id [{0}].", invoiceOfflineCollectionMessage.StringRecordId);
      }

      perfStopWatch.Stop();
      Logger.InfoFormat("Offline collection generation of invoice completed for pax invoice id [{0}] Time elapsed [{1}]", invoiceOfflineCollectionMessage.StringRecordId, perfStopWatch.Elapsed);
    }

    /// <summary>
    /// To link unlinked documents
    /// </summary>
    /// <param name="invoiceOfflineCollectionMessage"></param>
    private void LinkUnlinkedDocuments(InvoiceOfflineCollectionMessage invoiceOfflineCollectionMessage)
    {
      Logger.Info("Linking unlinked documents...");

      Logger.Info("Creating InvoiceBaseRepository object...");
      // Fetch only BaseInvoice object
      var invoiceBase = InvoiceBaseRepository.Single(inv => inv.Id == invoiceOfflineCollectionMessage.RecordId);

      if (invoiceBase != null)
      {
        Logger.Info("InvoiceBase record found. Calling LinkUnlinkedDocumentsForInvoice()...");
        LinkUnlinkedDocumentsForInvoice(invoiceBase);
        invoiceBase = null;
      }
      else
      {
        Logger.InfoFormat("Invoice record not found for invoice id [{0}].", invoiceOfflineCollectionMessage.StringRecordId);
        _errors.AppendFormat("Invoice record not found for invoice id [{0}].", invoiceOfflineCollectionMessage.StringRecordId);
      }
    }

    /// <summary>
    /// To link unlinked documents for invoice
    /// </summary>
    /// <param name="invoiceBase"></param>
    /// <param name="skipSuppDocLinkingDeadlineCheck">Indicated whether called from Finalization process</param>
    //SCP133627 - LP/544-Mismatch in CRSupporting File
    public void LinkUnlinkedDocumentsForInvoice(InvoiceBase invoiceBase, bool skipSuppDocLinkingDeadlineCheck = false)
    {
      if (invoiceBase != null)
      {

        var unlinkedDocuments = new List<UnlinkedSupportingDocument>();

        // below if condi. added for Form D, because form D record fetch with ProvisionalNumber.
        if (invoiceBase.BillingCode == (int)BillingCode.SamplingFormDE)
        {
          //SCP396433: Supporting doc mismatch
          var provisionalInvoiceList =
            FormDReository.Get(inv => inv.InvoiceId == invoiceBase.Id).Select(
              invoice =>
              new
              {
                invoice.ProvisionalInvoiceNumber,
                invoice.Invoice.ProvisionalBillingMonth,
                invoice.Invoice.ProvisionalBillingYear
              }).Distinct().ToList();
          //fetched the unlinked supp. doc. from unlinked supp. doc table and added to UnlinkedSupportingDocument variable 'unlinkedDocuments'.
          foreach (var provInvoice in provisionalInvoiceList)
          {
            // SCP
            // Added is not purged and prov billing month and year filter criteria to below query. 
            // Get unlinked documents for the search criteria which are not purged.
            var provUnlinkedDocuments = SupportingDocumentRepository.Get(doc => doc.InvoiceNumber.ToUpper() == provInvoice.ProvisionalInvoiceNumber.ToUpper() &&
                                                 doc.BillingCategoryId == invoiceBase.BillingCategoryId &&
                                                 doc.BillingMemberId == invoiceBase.BillingMemberId &&
                                                 doc.BilledMemberId == invoiceBase.BilledMemberId && doc.BillingYear == provInvoice.ProvisionalBillingYear && doc.BillingMonth == provInvoice.ProvisionalBillingMonth && doc.IsPurged == false).ToList();


            if (provUnlinkedDocuments.Count > 0)
            {
              unlinkedDocuments.AddRange(provUnlinkedDocuments);
            }// End if

          }// End foreach

        }// End if
        else
        {
          // SCP
          // Added is not purged and billing period, month and year filter criteria to below query.
          // Get unlinked documents for the search criteria which are not purged.
          unlinkedDocuments = SupportingDocumentRepository.Get(doc =>
                                      doc.InvoiceNumber.ToUpper() == invoiceBase.InvoiceNumber.ToUpper() && doc.BillingCategoryId == invoiceBase.BillingCategoryId && doc.BillingMemberId == invoiceBase.BillingMemberId &&
                                      doc.BilledMemberId == invoiceBase.BilledMemberId && doc.BillingYear == invoiceBase.BillingYear && doc.BillingMonth == invoiceBase.BillingMonth && doc.PeriodNumber == invoiceBase.BillingPeriod && doc.IsPurged == false).ToList();

        }// End else

        if (unlinkedDocuments.Count > 0)
        {
          //Logger.Info("Unlinked Supporting Docs found...");
          // Link supporting documents lying in ISA if any for this invoice.);
          foreach (var unlinkedDocument in unlinkedDocuments)
          {
            Logger.InfoFormat("Linking Document: '{0}' to InvoiceNo: {1}, BillingMemberId: {2}, BilledMemberId: {3}, BillingCategoryId: {4}, BillingPeriod: {5}, BillingMonth: {6}, BillingYear: {7}.", unlinkedDocument.OriginalFileName, unlinkedDocument.InvoiceNumber, unlinkedDocument.BillingMemberId, unlinkedDocument.BilledMemberId, unlinkedDocument.BillingCategoryId, unlinkedDocument.PeriodNumber, unlinkedDocument.BillingMonth, unlinkedDocument.BillingYear);

            // SCP62083: Supporting documents not received in SIS - SN-082
            // added parameter invoiceBase to get original invoice details in LinkDocument method.
            //SCP133627 - LP/544-Mismatch in CRSupporting File
            SupportingDocumentManager.LinkDocument(unlinkedDocument, invoiceBase, skipSuppDocLinkingDeadlineCheck);

          }// End foreach

        }// End if

      }
    }

    //SCP162502  Form C - AC OAR Jul P3 failure - No alert received
    //Method to link unlinked supporting docs for Form C
    public void LinkUnlinkedDocumentsForFormC(UnlinkedSupportingDocument unlinkedSupportingDocument, bool skipSuppDocLinkingDeadlineCheck = false)
    {
      if (unlinkedSupportingDocument != null)
      {
        SupportingDocumentManager.LinkDocument(unlinkedSupportingDocument, null, skipSuppDocLinkingDeadlineCheck);
      }
    }

    public void CreateFormCOfflineCollection(string rootPath, ILog logger, BillingPeriod billingPeriod, int memberId, bool isProvisional)
    {
      Init();
      _metaDataRecords = new List<InvoiceOfflineCollectionMetaData>();

      logger.InfoFormat("Fetching SamplingFormC offline collection data for billing period [{0}/{1}/{2}]",
                        billingPeriod.Period.ToString().PadLeft(2, '0'),
                        billingPeriod.Month.ToString().PadLeft(2, '0'),
                        billingPeriod.Year.ToString().PadLeft(2, '0'));

      var samplingFormCs = SamplingFormCRepository.GetFormCDataForOfflineArchive(
                      new DateTime(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period), memberId: memberId, isProvisional: isProvisional);

      // If SamplingFormC object is fetched successfully
      #region Create Form C Listings
      foreach (var sfcMemberGroup in samplingFormCs.GroupBy(sfc => new
      {
        sfc.ProvisionalBillingMemberId,
        sfc.FromMemberId,
        sfc.ProvisionalBillingMonth,
        sfc.ProvisionalBillingYear
      }))
      {
        var samplingFormCList = sfcMemberGroup.ToList();
        GenerateFormCListingReports(logger, samplingFormCList, rootPath, billingPeriod);
      }
      #endregion

      // Consolidate current period and previous period supporting documents
      IEnumerable<SamplingFormC> prevPeriodFormCs = GetPrevPeriodFormCForAttachments(new DateTime(billingPeriod.Year, billingPeriod.Month, billingPeriod.Period), memberId, isProvisional);
      samplingFormCs.AddRange(prevPeriodFormCs);

      #region Create Supporting Documents for Current as well as previous period



      GenerateFormCSuppDocuments(logger, samplingFormCs, rootPath, billingPeriod);

      #endregion
      // Save MetaData objects to store 
      UpdateOfflineCollectionMetaData(logger);
      logger.Info("Form C record meta data saved.");

    }

    /// <summary>
    /// This will generate Form C Supp documents 
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="samplingFormCs">List of samplingFormC</param>
    /// <param name="rootPath">rootPath</param>
    /// <param name="billingPeriod"></param>
    /// <param name="requestViaIsWeb"></param>
    private void GenerateFormCSuppDocuments(ILog logger, IEnumerable<SamplingFormC> samplingFormCs, string rootPath, BillingPeriod billingPeriod, bool requestViaIsWeb = false)
    {
      foreach (var consolidatedSamplingFormC in samplingFormCs)
      {
        //SCP162502  Form C - AC OAR Jul P3 failure - No alert received
        var formCFolderPath = CreateBaseFolderStructure(rootPath,
                                                        consolidatedSamplingFormC.FromMember.MemberCodeNumeric,
                                                        consolidatedSamplingFormC.ProvisionalBillingMember.MemberCodeNumeric,
                                                        BillingCategoryType.Pax,
                                                        consolidatedSamplingFormC.ProvisionalBillingPeriod,
                                                        null,
                                                        logger,
                                                        true, billingPeriod.Year, billingPeriod.Month, billingPeriod.Period, requestViaIsWeb);

        // Create 'SUPPDOCS' folder for SupportingDocuments
        var formCSuppDocsPath = Path.Combine(formCFolderPath, SupportingDocumentFolderName);
        if (!Directory.Exists(formCSuppDocsPath)) CreateDirectoryOnly(formCSuppDocsPath);
        logger.InfoFormat("FORM C Supporting Documents folder [{0}] created.", formCSuppDocsPath);
        SupportingDocumentGenerator.CreateFormCSupportingDocument(consolidatedSamplingFormC.SamplingFormCDetails, formCSuppDocsPath, _errors, logger, requestViaIsWeb);
        // Create MetaData Object for SupportingDocument and add it in collection 
        logger.InfoFormat("FORM C Supporting Documents generated.");
        if (InvoiceOfflineCollectionMetaDataRepository.First(iocmd => iocmd.FilePath.Equals(formCSuppDocsPath)) == null)
        {
          var formCMetaDataRecord = CreateFormCMetaDataRecord(consolidatedSamplingFormC, formCSuppDocsPath, SupportingDocumentFolderName, logger, billingPeriod);
          if (formCMetaDataRecord != null) _metaDataRecords.Add(formCMetaDataRecord);
          else _errors.AppendFormat("{0} folder type do not found in mater table.", SupportingDocumentFolderName);
        }
        logger.InfoFormat("FORM C supporting Docs generated for [BillingMember {0}, FromMember {1}, BillingYear {2}, BillingMonth {3}, BillingPeriod {4}, FormCId {5}] generated",
                          consolidatedSamplingFormC.ProvisionalBillingMemberText,
                          consolidatedSamplingFormC.FromMemberText,
                          consolidatedSamplingFormC.ProvisionalBillingYear,
                          consolidatedSamplingFormC.ProvisionalBillingMonth,
                          consolidatedSamplingFormC.ProvisionalBillingPeriod,
                          consolidatedSamplingFormC.Id);
      }
      return;
    }

    /// <summary>
    /// This function will generate Listing Reports of Form C grouped by criteria
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="sfcMemberGroup">Form C grouped by criteria</param>
    /// <param name="rootPath">rootPath</param>
    /// <param name="billingPeriod"></param>
    /// <param name="requestViaIsWeb"></param>
    private void GenerateFormCListingReports(ILog logger, List<SamplingFormC> sfcMemberGroup, string rootPath, BillingPeriod billingPeriod, bool requestViaIsWeb = false)
    {
      SamplingFormC firstSamplingFormC = sfcMemberGroup.First();
      logger.Info("Offline collection data for SamplingFormC fetched.");
      logger.Info("Creating base folder structure...");
      // Create base folder structure for FormC listing report
      //SCP162502  Form C - AC OAR Jul P3 failure - No alert received
      var formCFolderPath = CreateBaseFolderStructure(rootPath,
                                                      firstSamplingFormC.FromMember.MemberCodeNumeric,
                                                      firstSamplingFormC.ProvisionalBillingMember.MemberCodeNumeric,
                                                      BillingCategoryType.Pax,
                                                      firstSamplingFormC.ProvisionalBillingPeriod,
                                                      null,
                                                      logger,
                                                      true, billingPeriod.Year, billingPeriod.Month, billingPeriod.Period, requestViaIsWeb);
      logger.InfoFormat("Base folder structure [{0}] created.", formCFolderPath);

      // If InvoiceStatus is ReadyForBilling generate all required report files.

      // Create Listings folder
      logger.Debug("Creating Listings folder for Sampling FormC listing report...");
      var formCListingPath = Path.Combine(formCFolderPath, ListingsFolderName);
      if (!Directory.Exists(formCListingPath)) CreateDirectoryOnly(formCListingPath);
      logger.InfoFormat("FORM C Listings folder [{0}] created.", formCListingPath);

      // Generate FormC Listings report
      try
      {
        ListingsReportGenerator.CreateFormCListing(sfcMemberGroup, formCListingPath, _errors, logger);
      }
      catch (IOException ioException)
      {
        //Handled "The process cannot access the file because it is being used by another process " exception for mutli thread scenario
        //BuggId : 6912
        logger.InfoFormat("Exception occured while creating Form C listing , {0}", ioException.Message);

      }

      logger.InfoFormat("FORM C Listings reports generated.");



      // Create MetaData object for FormC Listings Folder

      if (InvoiceOfflineCollectionMetaDataRepository.First(iocmd => iocmd.FilePath.Equals(formCListingPath)) == null)
      {
        var formCMetaDataRecord = CreateFormCMetaDataRecord(firstSamplingFormC, formCListingPath, ListingsFolderName, logger, billingPeriod);
        if (formCMetaDataRecord != null) _metaDataRecords.Add(formCMetaDataRecord);
        else _errors.AppendFormat("{0} folder type do not found in mater table.", ListingsFolderName);
      }
      logger.InfoFormat("FORM C Listing generated for [BillingMember {0}, FromMember {1}, BillingYear {2}, BillingMonth {3}, BillingPeriod {4}, FormCId {5}] generated",
                        firstSamplingFormC.ProvisionalBillingMemberText,
                        firstSamplingFormC.FromMemberText,
                        firstSamplingFormC.ProvisionalBillingYear,
                        firstSamplingFormC.ProvisionalBillingMonth,
                        firstSamplingFormC.ProvisionalBillingPeriod,
                        firstSamplingFormC.Id);
      return;
    }

    /// <summary>
    /// This will return Form C objects of which Form C Attachments are completed on this period
    /// </summary>
    /// <param name="dateTime">Current period</param>
    /// <param name="memberId"></param>
    /// <param name="isProvisional"></param>
    /// <returns>List of SamplingFormC</returns>
    private IEnumerable<SamplingFormC> GetPrevPeriodFormCForAttachments(DateTime dateTime, int memberId, bool isProvisional)
    {
      return SamplingFormCRepository.GetFormCDataForOfflineArchive(dateTime, 1, memberId, isProvisional);
    }

    /// <summary>
    /// En-queues the invoice download request to the system for background processing.
    /// </summary>
    /// <param name="messages"></param>
    /// <returns></returns>
    public bool EnqueueDownloadRequest(IDictionary<string, string> messages)
    {
      try
      {
        var offlineCollectionDownloadQueueName = ConfigurationManager.AppSettings["OfflineCollectionDownloadQueueName"];
        if (!string.IsNullOrEmpty(offlineCollectionDownloadQueueName))
        {
          var queueHelper = new QueueHelper(offlineCollectionDownloadQueueName);
          queueHelper.Enqueue(messages);
          return true;
        }
        // SCP227747: Cargo Invoice Data Download
        // Adding filure reason in logs
        Logger.InfoFormat(
          "Error in Enque message for :: Invoice Id: [{0}], User Id: [{1}], Is Form C: [{2}], Is Receivables: [{3}], Output Zip File Name: [{4}], Download Url: [{5}], Is IS-WEB Download: [ {6}]",
          messages["RECORD_ID"], messages["USER_ID"], messages["IS_FORM_C"], messages["IS_RECEIVABLE"],
          messages["OUTPUT_ZIP_FILE_NAME"], messages["DOWNLOAD_URL"], messages["IS_WEB_DOWNLOAD"]);
        Logger.InfoFormat("In AppSettings OfflineCollectionDownloadQueueName: {0}", offlineCollectionDownloadQueueName);
        return false;
      }
      catch (Exception exception)
      {
        // SCP227747: Cargo Invoice Data Download
        // Adding filure reason in logs
        Logger.InfoFormat(
          "Error in Enque message for :: Invoice Id: [{0}], User Id: [{1}], Is Form C: [{2}], Is Receivables: [{3}], Output Zip File Name: [{4}], Download Url: [{5}], Is IS-WEB Download: [ {6}]",
          messages["RECORD_ID"], messages["USER_ID"], messages["IS_FORM_C"], messages["IS_RECEIVABLE"],
          messages["OUTPUT_ZIP_FILE_NAME"], messages["DOWNLOAD_URL"], messages["IS_WEB_DOWNLOAD"]);

        Logger.Error("Error occurred while adding message to queue.", exception);
        return false;
      }
    }

    /// <summary>
    /// This function will create Form-C offlineCollection for Web click event
    /// </summary>
    /// <param name="logger">logger</param>
    /// <param name="provisionalBillingMonth">provisionalBillingMonth of sampling form C</param>
    /// <param name="provisionalBillingYear">provisionalBillingYear of sampling form C</param>
    /// <param name="fromMemberId">fromMemberId of sampling form C</param>
    /// <param name="invoiceStatusIds">invoiceStatusIds of sampling form C</param>
    /// <param name="provisionalBillingMemberId">provisionalBillingMemberId of sampling form C</param>
    /// <param name="listingCurrencyCodeNum">listingCurrencyCodeNum of sampling form C</param>
    /// <param name="offlinerootFolderPath">offlinerootFolderPath of sampling form C</param>
    /// <returns>List InvoiceOfflineCollectionMetaData</returns>
    public List<InvoiceOfflineCollectionMetaData> GenerateFormCofflineCollectionForWeb(ILog logger, int provisionalBillingMonth, int provisionalBillingYear, int fromMemberId, string invoiceStatusIds, int provisionalBillingMemberId, int listingCurrencyCodeNum, string offlinerootFolderPath)
    {
      Init();
      _metaDataRecords = new List<InvoiceOfflineCollectionMetaData>();
      var billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      var samplingFormCs = SamplingFormCRepository.GetSamplingFormCDetails(provisionalBillingMonth: provisionalBillingMonth,
                                                                           provisionalBillingYear: provisionalBillingYear,
                                                                           fromMemberId: fromMemberId,
                                                                           invoiceStatusIds: invoiceStatusIds,
                                                                           provisionalBillingMemberId: provisionalBillingMemberId,
                                                                           listingCurrencyCodeNum: listingCurrencyCodeNum).ToList();

      if (samplingFormCs.Count > 0)
      {
        GenerateFormCListingReports(logger, samplingFormCs, offlinerootFolderPath, billingPeriod, true);

        GenerateFormCSuppDocuments(logger, samplingFormCs, offlinerootFolderPath, billingPeriod, true);
      }
      return _metaDataRecords;
    }
    public class MetaDataRecordComparer : IEqualityComparer<InvoiceOfflineCollectionMetaData>
    {
      #region Implementation of IEqualityComparer<in InvoiceOfflineCollectionMetaData>

      /// <summary>
      /// Determines whether the specified objects are equal.
      /// </summary>
      /// <returns>
      /// true if the specified objects are equal; otherwise, false.
      /// </returns>
      /// <param name="first">The first object of type <paramref name="InvoiceOfflineCollectionMetaData"/> to compare.</param><param name="second">The second object of type <paramref name="InvoiceOfflineCollectionMetaData"/> to compare.</param>
      public bool Equals(InvoiceOfflineCollectionMetaData first, InvoiceOfflineCollectionMetaData second)
      {
        if (first == null) return second == null;
        if (second == null) return false;
        return first.FilePath.Equals(second.FilePath);
      }

      /// <summary>
      /// Returns a hash code for the specified object.
      /// </summary>
      /// <returns>
      /// A hash code for the specified object.
      /// </returns>
      /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param><exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and <paramref name="obj"/> is null.</exception>
      public int GetHashCode(InvoiceOfflineCollectionMetaData obj)
      {
        return obj.FilePath.GetHashCode();
      }

      #endregion
    }

    private static void CreateDirectoryOnly(string path)
    {
      var direcory = Directory.CreateDirectory(path);
      direcory = null;
    }


    /// <summary>
    /// This function is used to en-queue message for offline collection download
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="billingPeriod"></param>
    /// <param name="billingMonth"></param>
    /// <param name="billingYear"></param>
    /// <param name="invoiceStatusId"></param>
    /// <param name="billingCategory"></param>
    /// <param name="delays"></param>
    /// SCP419599: SIS: Admin Alert - Error in creating Legal Invoice Archive zip file of CDC - SISPROD -16oct2016
    public void EnqueueOfflineCollectionDownload(int memberId, int billingPeriod, int billingMonth, int billingYear, int invoiceStatusId, string billingCategory, int delays)
    {
        InvoiceRepository.EnqueueOfflineCollectionDownload(memberId, billingPeriod, billingMonth, billingYear, invoiceStatusId, billingCategory, delays);
    }
  }

}

