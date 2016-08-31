using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Data.MiscUatp.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using System.Linq;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using iPayables.UserManagement;
using log4net;
using NVelocity;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;
using System.Text;
using Iata.IS.Data.Cargo;
using Iata.IS.Model.Pax.Common;
using System.Linq;

namespace Iata.IS.Business.Common.Impl
{
  public class InvoiceOfflineCollectionDownloadManager : IInvoiceOfflineCollectionDownloadManager
  {
    public IMemberManager _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
    public IBroadcastMessagesManager BroadcastMessagesManager { private get; set; }
    public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }
    public ISamplingFormCRepository SamplingFormCRepository = Ioc.Resolve<ISamplingFormCRepository>();
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IRepository<InvoiceOfflineCollectionMetaData> InvoiceOfflineCollectionMetaDataRepository { get; set; }
    public IRepository<IsHttpDownloadLink> IsHttpDownloadLinkRepository { get; set; }
    public IMemberManager MemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
    private Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>> _hPerInvoiceDirectory;
    public IUserManagement AuthManager = Ioc.Resolve<IUserManagement>();
    public IReferenceManager ReferenceManager;
    private string _baseFolderPath;
    private string _indexXmlPath;
    public IInvoiceOfflineCollectionManager InvoiceOfflineCollectionManager { private get; set; }
    private const string EInvoiceFolderNameConstant = "E-INVOICE";
    private const string ListingsFolderNameConstant = "LISTINGS";
    private const string MemosFolderNameConstant = "MEMOS";
    private const string SuppdocsFolderNameConstant = "SUPPDOCS";
    private const string FormCUiFolderNameConstant = "FormC_UI";

    //SCP132419 - SRM: Duplicate OAR's generated May P3.
    public bool IsFileExist(string filePath)
    {
        var fileName = string.Format("{0}.ZIP", Path.GetFileNameWithoutExtension(filePath));
        List<IsInputFile> files = null;
        try
        {
            ReferenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
            files = ReferenceManager.GetAllIsInputFile(fileName);
        }
        catch (Exception exception)
        {
            Logger.InfoFormat("Error occured while getting file entry in Is Input File table : {0}",
                              exception.Message);
        }

        if (files == null || files.Count == 0)
            return false;
        foreach (var file in files)
        {
            var fileinfo = new FileInfo(string.Format("{0}\\{1}", file.FileLocation, fileName));
            if (fileinfo.Exists)
                return true;
        }
        return false;
    }
      
    /// <summary>
    /// Creates the member invoices offline collection zip file.
    /// and returns the output file name on the FTP server
    /// </summary>
    /// <param name="member">member</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="isReceivable">isReceivable = is Billed member?</param>
    /// <param name="options">options : containing list of required/selected offline documents</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isFormC">isFormC</param>
    /// <param name="invoiceStatus"></param>
    //SCP132419 - SRM: Duplicate OAR's generated May P3.
    public List<string> GetMemberInvoicesOfflineCollectionZip(Member member, BillingPeriod billingPeriod, bool isReceivable, List<string> options, BillingCategoryType billingCategoryType, bool isFormC, int invoiceStatus, bool checkOARGenerated = false)
    {
      var outputFileNames = new List<string>();
      if (!isFormC)
      {
        //SCP132419 - SRM: Duplicate OAR's generated May P3.
        //Check if entry exists in IS_File_Log and exists physically on location
        if (checkOARGenerated)
        {
          var fileName = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, false, 0, 0, true);
            if (IsFileExist(fileName))
            {
                Logger.InfoFormat("Skipping {0} file generataion as file is already generated for member {1}.", fileName, member.Id);
                return outputFileNames;
            }
        }  
          
        Logger.InfoFormat("Fetching Invoice Offline MetaData Collection of Invoices for member id - {0} ", member.Id);
        var invOfflineMetaDataCollection = FetchPerMemberInvoiceMetaData(member.MemberCodeNumeric, billingPeriod, billingCategoryType, isReceivable, options);
        Logger.InfoFormat("{0} Offline MetaData Collection records found for member id - {1} ", invOfflineMetaDataCollection.Count(), member.Id);
        var invoiceBases = new List<InvoiceBase>();
        //_hPerInvoiceDirectory will store the per invoice or per form c entry
        _hPerInvoiceDirectory = new Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>>();
        Logger.Info("Created new PerInvoiceDirectory.");
        foreach (IGrouping<string, InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDatas in invOfflineMetaDataCollection.GroupBy(cdr => cdr.InvoiceNumber))
        {
          var invoiceOfflineCollectionMetaDataList = invoiceOfflineCollectionMetaDatas.ToList();
          var invoiceNumber = invoiceOfflineCollectionMetaDataList[0].InvoiceNumber;

          //SCP279970: OAR Optimization.
          var stopwatch = new Stopwatch();
          stopwatch.Start();

          var invoiceBaseList = new InvoiceBaseManager().GetInvoiceBaseDetails(member.Id, billingPeriod.Year,
                                                                               billingPeriod.Month, billingPeriod.Period,
                                                                               invoiceNumber, (int) billingCategoryType,
                                                                               isReceivable);

          stopwatch.Stop();
          Logger.Info("GET BASE INVOICE DETAILS : TIME[" + stopwatch.Elapsed + "]");

          if (invoiceBaseList != null)
          {
            foreach (var invoiceBase in invoiceBaseList)
            {
              var otherMemberId = isReceivable ? invoiceBase.BilledMemberId : invoiceBase.BillingMemberId;
              var otherMemberCode = _memberManager.GetMemberCode(otherMemberId);//billing

              var filteredOfflineCollMetadata = isReceivable
                        ? invoiceOfflineCollectionMetaDataList.FindAll(i => i.BilledMemberCode == otherMemberCode)
                        : invoiceOfflineCollectionMetaDataList.FindAll(i => i.BillingMemberCode == otherMemberCode);

              if (invoiceStatus > 0)
              {

                if (invoiceBase != null && (invoiceBase.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoiceBase.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoiceBase.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoiceBase.InvoiceStatusId == (int)InvoiceStatusType.Presented))
                {
                  Logger.InfoFormat("Generating Invoice Offline Collection for invoice number {0}", invoiceBase.InvoiceNumber);
                  invoiceBases.Add(invoiceBase);
                  GetInvoiceOfflineCollection(filteredOfflineCollMetadata, member, billingCategoryType, billingPeriod, isReceivable, false, invoiceBase);
                }
                else
                {
                  Logger.InfoFormat("Invoice number {0} not found for Period {1}, Month {2},Year {3},{4} {5},Billing Category {6} in System Monitor Reprocessing",
                                    invoiceNumber,
                                    billingPeriod.Period,
                                    billingPeriod.Month,
                                    billingPeriod.Year,
                                    isReceivable ? "BillingMemberId" : "BilledMemberId",
                                    member.Id,
                                    billingCategoryType);
                }
              }
              else
              {

                if (invoiceBase != null && (invoiceBase.InvoiceStatus == InvoiceStatusType.ProcessingComplete || invoiceBase.InvoiceStatus == InvoiceStatusType.Presented))
                {
                  Logger.InfoFormat("Generating Invoice Offline Collection for invoice number {0}", invoiceBase.InvoiceNumber);
                  invoiceBases.Add(invoiceBase);
                  GetInvoiceOfflineCollection(filteredOfflineCollMetadata, member, billingCategoryType, billingPeriod, isReceivable, false, invoiceBase);
                }
                else
                {
                  Logger.InfoFormat("Invoice number {0} not found for Period {1}, Month {2},Year {3},{4} {5},Billing Category {6} in Processing Complete or Presented status",
                                    invoiceNumber,
                                    billingPeriod.Period,
                                    billingPeriod.Month,
                                    billingPeriod.Year,
                                    isReceivable ? "BillingMemberId" : "BilledMemberId",
                                    member.Id,
                                    billingCategoryType);
                }
              }


            }
          }
        }
        if (invoiceBases.Count > 0)
        {
          Logger.Info("Creating Index file.");
          Logger.InfoFormat("BillingPeriod: {0}, BillingCategoryType: {1}, MemberId: {2}, IsReceivable: {3}",
                            billingPeriod, billingCategoryType, member.Id, isReceivable);
          CreateIndexFile(invoiceBases, new List<SamplingFormC>(), billingPeriod, billingCategoryType, member.Id, isReceivable, false);

          if (!XmlValidator.ValidateXml(_indexXmlPath))
            Logger.InfoFormat("Error occured while validating Index file {0}.", _indexXmlPath);

          var parentFolderName = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, false, 0, 0, true);

          var outputFileName = string.Format("{0}.ZIP", parentFolderName);
          FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);
          //Move zip to ftp path
          Logger.Info("Moving file to FTP location.");
          var ftpfilePath = Path.Combine(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric), Path.GetFileName(outputFileName));
          FileIo.MoveFile(outputFileName, ftpfilePath);
          outputFileNames.Add(ftpfilePath);
          Directory.Delete(parentFolderName);
        }
      }
      else
      {
        //Logic for Form C :-
        //Create one offline archive file per Billing Member , for perticuler Provisional Year-month.which contains multiple form Cs having different billed members

        Logger.InfoFormat("Fetching Invoice Offline MetaData Collection of FormCs for member id - {0} ", member.Id);
        var formCOfflineMetaDataCollection = FetchPerMemberFormCMetaData(member.MemberCodeNumeric, billingPeriod, billingCategoryType, isReceivable, options).ToList();
        Logger.InfoFormat("{0} Offline MetaData Collection records found for member id - {1} ", formCOfflineMetaDataCollection.Count(), member.Id);

        _hPerInvoiceDirectory = new Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>>();
        Logger.Info("Created new PerInvoiceDirectory.");
        foreach (var samplingFormCRecordPerFile in formCOfflineMetaDataCollection.GroupBy(sfc => new
                                                                                                    {
                                                                                                      sfc.ProvisionalBillingMonth,
                                                                                                      sfc.ProvisionalBillingYear
                                                                                                    }))
        {
          var samplingFormCs = new List<SamplingFormC>();
          //For FormC - unique record can be identified by the criteria - BilledMemberCode,BillingMemberCode,ProvisionalBillingMonth,ProvisionalBillingYear
          foreach (var formCOfflineMetaDataCollections in samplingFormCRecordPerFile.GroupBy(sfc => new
                                                                                                      {
                                                                                                        sfc.BilledMemberCode,
                                                                                                        sfc.BillingMemberCode
                                                                                                      }))
          {
            var formCOfflineCollectionMetaDataList = formCOfflineMetaDataCollections.ToList();
            var formCOfflineCollectionMetaDataRecord = formCOfflineCollectionMetaDataList[0];
            List<SamplingFormC> samplingFormCList;

            //    if (isReceivable) // Billing

            if (invoiceStatus > 0)
            {
              samplingFormCList =
              SamplingFormCRepository.Get(
                i =>
                i.ProvisionalBillingMonth == formCOfflineCollectionMetaDataRecord.ProvisionalBillingMonth && i.ProvisionalBillingYear == formCOfflineCollectionMetaDataRecord.ProvisionalBillingYear &&
                i.FromMember.MemberCodeNumeric == formCOfflineCollectionMetaDataRecord.BillingMemberCode && i.ProvisionalBillingMember.MemberCodeNumeric == formCOfflineCollectionMetaDataRecord.BilledMemberCode
                && (i.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || i.InvoiceStatusId == (int)InvoiceStatusType.Presented || i.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || i.InvoiceStatusId == (int)InvoiceStatusType.Claimed)).ToList();

            }
            else
            {
              samplingFormCList =
                SamplingFormCRepository.Get(
                  i =>
                  i.ProvisionalBillingMonth == formCOfflineCollectionMetaDataRecord.ProvisionalBillingMonth && i.ProvisionalBillingYear == formCOfflineCollectionMetaDataRecord.ProvisionalBillingYear &&
                  i.FromMember.MemberCodeNumeric == formCOfflineCollectionMetaDataRecord.BillingMemberCode && i.ProvisionalBillingMember.MemberCodeNumeric == formCOfflineCollectionMetaDataRecord.BilledMemberCode
                  && (i.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || i.InvoiceStatusId == (int)InvoiceStatusType.Presented)).ToList();
            }

            /*      else
                    samplingFormCList =
                      SamplingFormCRepository.Get(
                        i =>
                        i.ProvisionalBillingMonth == formCOfflineCollectionMetaDataRecord.ProvisionalBillingMonth && i.ProvisionalBillingYear == formCOfflineCollectionMetaDataRecord.ProvisionalBillingYear &&
                         i.FromMember.MemberCodeNumeric == formCOfflineCollectionMetaDataRecord.BilledMemberCode && i.ProvisionalBillingMember.MemberCodeNumeric == formCOfflineCollectionMetaDataRecord.BillingMemberCode).ToList();
                  */
            if (samplingFormCList.Count > 0)
            {
              Logger.InfoFormat("Generating Invoice Offline Collection of FormC for member id {0},ProvisionalBillingMonth {1},ProvisionalBillingYear {2},FromMember id {3},ProvisionalBillingMember id {4}"
                , member.Id, samplingFormCList[0].ProvisionalBillingMonth, samplingFormCList[0].ProvisionalBillingYear, samplingFormCList[0].FromMemberId, samplingFormCList[0].ProvisionalBillingMemberId);
              samplingFormCs.Add(samplingFormCList[0]);
              GetInvoiceOfflineCollection(formCOfflineCollectionMetaDataList, member, billingCategoryType, billingPeriod, isReceivable, true, null, samplingFormCList[0]);
            }
            else
            {
              Logger.InfoFormat("SamplingFormC for member id {0},ProvisionalBillingMonth {1},ProvisionalBillingYear {2},FromMember id {3},ProvisionalBillingMember id {4} not found in Processing complete or presented."
                , member.Id, formCOfflineCollectionMetaDataRecord.ProvisionalBillingMonth, formCOfflineCollectionMetaDataRecord.ProvisionalBillingYear, formCOfflineCollectionMetaDataRecord.BillingMemberCode, formCOfflineCollectionMetaDataRecord.BilledMemberCode);
            }
          }
          if (samplingFormCs.Count > 0)
          {
            //SCP132419 - SRM: Duplicate OAR's generated May P3.
            //Check if entry exists in IS_File_Log and exists physically on location
            if (checkOARGenerated)
            {
              var fileName = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, true, samplingFormCs.ToList()[0].ProvisionalBillingMonth, samplingFormCs.ToList()[0].ProvisionalBillingYear, true);
                if (IsFileExist(fileName))
                {
                    Logger.InfoFormat("Skipping {0} file generataion as file is already generated for member {1}.", fileName, member.Id);
                    continue;
                }
            }
              
            Logger.Info("Creating Index file.");
            Logger.InfoFormat("BillingPeriod: {0}, BillingCategoryType: {1}, MemberId: {2}, IsReceivable: {3}",
                            billingPeriod, billingCategoryType, member.Id, isReceivable);
            CreateIndexFile(new List<InvoiceBase>(), samplingFormCs, billingPeriod, billingCategoryType, member.Id, isReceivable, true);

            if (!XmlValidator.ValidateXml(_indexXmlPath))
              Logger.InfoFormat("Error occured while validating Index file {0}.", _indexXmlPath);

            var parentFolderName = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, true, samplingFormCs.ToList()[0].ProvisionalBillingMonth, samplingFormCs.ToList()[0].ProvisionalBillingYear, true);
            
            var outputFileName = string.Format("{0}.ZIP", parentFolderName);
            FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);
            //Move zip to ftp path
            Logger.Info("Moving file to FTP location.");
            var ftpfilePath = Path.Combine(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric), Path.GetFileName(outputFileName));
            FileIo.MoveFile(outputFileName, ftpfilePath);
            outputFileNames.Add(ftpfilePath);
            Directory.Delete(parentFolderName);
          }
        }
      }
      return outputFileNames;
    }

    /// <summary>
    /// Gets the invoice offline collection.
    /// </summary>
    /// <param name="userId">Login user id</param>
    /// <param name="zipFileName">Name of the zip file.</param>
    /// <param name="id">The invoice id or Form C id.</param>
    /// <param name="options">The options.</param>
    /// <param name="downloadUrl">download url</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="isFormC">isFormC</param>
   /// <returns></returns>
    public string GetInvoiceOfflineCollectionZip(int userId, string zipFileName, Guid id, List<string> options, string downloadUrl, bool isReceivable, bool isFormC)
    {

      // Create an object of the nVelocity data dictionary
      var context = new VelocityContext();
      context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
      var user = AuthManager.GetUserByUserID(userId);
      var emailAddress = user == null ? string.Empty : user.Email;
      Logger.InfoFormat("User is {0} for UserId {1} Email Id {2}.", user == null ? "NULL" : "NOT NULL", userId, emailAddress);
      _hPerInvoiceDirectory = new Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>>();
      if (isFormC)
      {
        var samplingFormC = SamplingFormCRepository.First(i => i.Id == id);
        if (samplingFormC != null)
        {
          var member = _memberManager.GetMember(isReceivable ? samplingFormC.FromMemberId : samplingFormC.ProvisionalBillingMemberId);
          var billingPeriod = new BillingPeriod
          {
            Month = samplingFormC.ProvisionalBillingMonth,
            Year = samplingFormC.ProvisionalBillingYear
          };
          
          var offlinerootFolderPath = FileIo.GetForlderPath(SFRFolderPath.OfflineCollectionFormCUiFolder); //ConfigurationManager.AppSettings["OfflineCollectionUIRootFolderPath"];
          var invOfflineMetaDataCollection = InvoiceOfflineCollectionManager.GenerateFormCofflineCollectionForWeb(Logger,
                                                                                                                  samplingFormC.ProvisionalBillingMonth,
                                                                                                                  samplingFormC.ProvisionalBillingYear,
                                                                                                                  samplingFormC.FromMemberId,
                                                                                                                  samplingFormC.InvoiceStatusId.ToString(),
                                                                                                                  samplingFormC.ProvisionalBillingMemberId,
                                                                                                                  samplingFormC.ListingCurrencyId.HasValue ? samplingFormC.ListingCurrencyId.Value : 0,
                                                                                                                  Path.Combine(offlinerootFolderPath, Guid.NewGuid().ToString()));

          invOfflineMetaDataCollection = GetFilteredInvoiceOfflineMetadata(invOfflineMetaDataCollection, options);

          //Fixed Simultaneous 2 zip download related issue
          GetInvoiceOfflineCollection(invOfflineMetaDataCollection, member, BillingCategoryType.Pax, billingPeriod, isReceivable, true,null, samplingFormC, isWebZip: true, zipFileName: zipFileName);

          var samplingFormCs = new List<SamplingFormC> {
                                                     samplingFormC
                                                   };
          Logger.InfoFormat("BillingPeriod: {0}, BillingCategoryType: {1}, MemberId: {2}, IsReceivable: {3}",
                            billingPeriod, 1, member.Id, isReceivable);
          CreateIndexFile(new List<InvoiceBase>(), samplingFormCs, billingPeriod, BillingCategoryType.Pax, member.Id, isReceivable, true);

          if (!XmlValidator.ValidateXml(_indexXmlPath))
            Logger.InfoFormat("Error occurred while validating Index file {0}.", _indexXmlPath);

          var outputFileName = string.Format("{0}\\{1}.ZIP", Path.GetDirectoryName(string.Format("{0}.ZIP", _baseFolderPath)), zipFileName);

          // Create zip file.
          FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);

          // Copy to ftp path
          var ftpfilePath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollWebRoot), Path.GetFileName(outputFileName));  //(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric), Path.GetFileName(outputFileName));
          Logger.Info(string.Format("Copying file from {0} to {1}.", outputFileName, ftpfilePath));
          File.Copy(outputFileName, ftpfilePath, true);

          // Add Http Download Link
          var isHttpDownloadLink = new IsHttpDownloadLink
          {
            FilePath = ftpfilePath,
            LastUpdatedBy = member.Id,
            LastUpdatedOn = DateTime.UtcNow
          };
          IsHttpDownloadLinkRepository.Add(isHttpDownloadLink);
          UnitOfWork.CommitDefault();

          //Build http url for download.
          var httpUrl = string.Format("{0}/{1}", downloadUrl, isHttpDownloadLink.Id);
          Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

          context.Put("ProvisionalMonth", string.Format("{0} {1}", CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(samplingFormC.ProvisionalBillingMonth).ToUpper(), samplingFormC.ProvisionalBillingYear));
          context.Put("MemberCode", string.Format("{0} - {1}", member.MemberCodeAlpha, member.MemberCodeNumeric));
          BroadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl, EmailTemplateId.OutputFileAvailableForFormCAlert, context);
          
          //Delete unused folder after generate OAR for formc.
          foreach (var invoiceOfflineCollectionMetaData in invOfflineMetaDataCollection)
          {
              Logger.InfoFormat("Delete unused folder after generate OAR for formc, file path: [{0}]", invoiceOfflineCollectionMetaData.FilePath);
              DeleteFolderFormc(invoiceOfflineCollectionMetaData.FilePath, offlinerootFolderPath);
          }
          return httpUrl;
        }
        Logger.InfoFormat("Invoice Id {0} not found", id);
      }
      else
      {
        var invoiceBase = InvoiceBaseRepository.Single(i => i.Id == id);
        if (invoiceBase != null)
        {
          var member = _memberManager.GetMember(isReceivable ? invoiceBase.BillingMemberId : invoiceBase.BilledMemberId);

          var billingMemberCode = _memberManager.GetMemberCode(invoiceBase.BillingMemberId);
          var billedMemberCode = _memberManager.GetMemberCode(invoiceBase.BilledMemberId);
          var billingPeriod = new BillingPeriod
          {
            Month = invoiceBase.BillingMonth,
            Period = invoiceBase.BillingPeriod,
            Year = invoiceBase.BillingYear
          };

          var invOfflineMetaDataCollection =
            InvoiceOfflineCollectionMetaDataRepository.Get(
              i =>
              i.BilledMemberCode == billedMemberCode && i.BillingMemberCode == billingMemberCode &&
              i.BillingCategoryId == invoiceBase.BillingCategoryId && i.BillingMonth == invoiceBase.BillingMonth && i.BillingYear == invoiceBase.BillingYear &&
              i.InvoiceNumber.ToUpper() == invoiceBase.InvoiceNumber.ToUpper() && i.PeriodNo == invoiceBase.BillingPeriod).ToList();

          invOfflineMetaDataCollection = GetFilteredInvoiceOfflineMetadata(invOfflineMetaDataCollection, options);

          if (invOfflineMetaDataCollection.ToList().Find(i => i.OfflineCollectionFolderTypeId == 1) != null &&
            (invoiceBase.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoiceBase.InvoiceStatus == InvoiceStatusType.Claimed))
          {
            //Generate EInvoices
            var eInvoiceDocumentGenerator = Ioc.Resolve<IEinvoiceDocumentGenerator>();
            var invOfflineMetaDataObj = invOfflineMetaDataCollection.Find(i => i.OfflineCollectionFolderTypeId == 1);
            if (invOfflineMetaDataObj != null)
            {
              Logger.InfoFormat("Generating EInvoices on path {0}", invOfflineMetaDataObj.FilePath);
              eInvoiceDocumentGenerator.CreateEinvoiceDocuments(invoiceBase, invOfflineMetaDataObj.FilePath);
            }
          }

          //FetchPerMemberInvoiceMetaData(member.MemberCodeNumeric, billingPeriod, invoiceBase.BillingCategory, isReceivable, options).ToList();
          //Fixed Simultaneous 2 zip download related issue
          GetInvoiceOfflineCollection(invOfflineMetaDataCollection, member, invoiceBase.BillingCategory, billingPeriod, isReceivable, false, invoiceBase, isWebZip: true, zipFileName: zipFileName);

          // No need to delete PDFs, as the PDFs are created at the offline collection path directly.
          //if (invOfflineMetaDataCollection.ToList().Find(i => i.OfflineCollectionFolderTypeId == 1) != null &&
          //  (invoiceBase.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoiceBase.InvoiceStatus == InvoiceStatusType.Claimed))
          //{
          //  //Delete EInvoices :- as these are generated from Web
          //  Logger.Info("Deleting temparary EInvoice files created.");
          //  var invOfflineMetaDataObj = invOfflineMetaDataCollection.Find(i => i.OfflineCollectionFolderTypeId == 1);
          //  if (invOfflineMetaDataObj != null)
          //  {
          //    string[] filePaths = Directory.GetFiles(invOfflineMetaDataObj.FilePath);
          //    foreach (string filePath in filePaths)
          //      File.Delete(filePath);
          //  }
          //}

          var invoiceBases = new List<InvoiceBase> {
                                                     invoiceBase
                                                   };

          Logger.InfoFormat("BillingPeriod: {0}, BillingCategoryType: {1}, MemberId: {2}, IsReceivable: {3}",
                            billingPeriod, invoiceBase.BillingCategory, member.Id, isReceivable);

          CreateIndexFile(invoiceBases, new List<SamplingFormC>(), billingPeriod, invoiceBase.BillingCategory, member.Id, isReceivable, false);

          if (!XmlValidator.ValidateXml(_indexXmlPath))
            Logger.InfoFormat("Error occured while validating Index file {0}.", _indexXmlPath);

          var outputFileName = string.Format("{0}\\{1}.ZIP", Path.GetDirectoryName(string.Format("{0}.ZIP", _baseFolderPath)), zipFileName);

          // Create zip file.
          FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);

          // Copy to ftp path
          var ftpfilePath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollWebRoot), Path.GetFileName(outputFileName));  //(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric), Path.GetFileName(outputFileName));
          Logger.Info(string.Format("Copying file from {0} to {1}.", outputFileName, ftpfilePath));
          File.Copy(outputFileName, ftpfilePath, true);

          // Add Http Download Link
          var isHttpDownloadLink = new IsHttpDownloadLink
          {
            FilePath = ftpfilePath,
            LastUpdatedBy = member.Id,
            LastUpdatedOn = DateTime.UtcNow
          };
          IsHttpDownloadLinkRepository.Add(isHttpDownloadLink);
          UnitOfWork.CommitDefault();

          //Build http url for download.
          var httpUrl = string.Format("{0}/{1}", downloadUrl, isHttpDownloadLink.Id);
          Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

          context.Put("InvoiceNumber", invoiceBase.InvoiceNumber);
          BroadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl, EmailTemplateId.OutputFileAvailableAlert, context);
          Logger.Info("Send an email to " + emailAddress);

          return httpUrl;
        }
        Logger.InfoFormat("Invoice Id {0} not found", id);
      }

      return string.Empty;
    }

    /// <summary>
    /// Gets the invoice offline collection.
    /// </summary>
    /// <param name="invoiceBase">invoiceBase</param>
    /// <param name="invoiceOfflineCollectionMetaDataCollection">invoice Offline Collection Meta Data Collection</param>
    /// <param name="member">member</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="isFormC">isFormC</param>
    /// <param name="samplingFormC"></param>
    /// <param name="isWebZip">Set this flag to true for Web zip icon click</param>
    /// <param name="zipFileName">Pass this value only for Web click : Will be used to create unique base folder for two invoices of same member</param>
    /// <param name="targetDate">CMP529: If Daily OAR required then target date is required.</param>
    /// <param name="isDailyOARRequired">CMP529: If Dail OAR required then this flag will be true.</param>
     /// <param name="locationId">CMP622: true if location specific file is required.</param>
    /// <param name="isLocationSpecNilFile">CMP622: true if location specific file is required.</param>
    private void GetInvoiceOfflineCollection(IEnumerable<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection, Member member, BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, bool isReceivable, bool isFormC, InvoiceBase invoiceBase = null, SamplingFormC samplingFormC = null, bool isWebZip = false, string zipFileName = null, DateTime? targetDate = null, bool isDailyOARRequired = false, string locationId = null, bool isLocationSpecNilFile = false)
    {
      var options = GetUpdatedOptions(invoiceOfflineCollectionMetaDataCollection);
      _baseFolderPath = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, isFormC, isFormC ? (samplingFormC != null ? samplingFormC.ProvisionalBillingMonth : 0) : 0, isFormC ? (samplingFormC != null ? samplingFormC.ProvisionalBillingYear : 0) : 0, false, isWebZip, zipFileName, targetDate,isDailyOARRequired,locationId:locationId, isLocationSpecNilFile:isLocationSpecNilFile);
      Logger.Info("Parent folder created.");
      var memberId = isFormC && samplingFormC != null ? (isReceivable ? samplingFormC.ProvisionalBillingMemberId : samplingFormC.FromMemberId) : (isReceivable ? invoiceBase.BilledMemberId : invoiceBase.BillingMemberId);
      var provYear = samplingFormC != null ? samplingFormC.ProvisionalBillingYear : 0;
      var provMonth = samplingFormC != null ? samplingFormC.ProvisionalBillingMonth : 0;
      var invoiceNumber = invoiceBase != null ? invoiceBase.InvoiceNumber : string.Empty;
      var uniqueIdentityPerRecord = invoiceBase != null ? String.Format("{0}{1}", invoiceBase.InvoiceNumber, isReceivable ? invoiceBase.BilledMemberId : invoiceBase.BillingMemberId) : string.Format("{0}{1}{2}00", memberId, provYear, provMonth.ToString().PadLeft(2, '0'));
      string subFolderFullPath;
      CreateSubFolders(_baseFolderPath, memberId, uniqueIdentityPerRecord, options, isFormC, out subFolderFullPath, string.Format("{0}{1}00", provYear, provMonth.ToString().PadLeft(2, '0')), invoiceNumber);
      Logger.Info("Sub folder created.");
      // SCP387738: FW: Wrong contents of D-OAR-MISC-P-D9N-20150710.ZIP
      CopyInvoiceOfflineCollection(invoiceOfflineCollectionMetaDataCollection, uniqueIdentityPerRecord, isFormC, isReceivable, member, billingCategoryType, isDailyOARRequired);

      // If sub folder is empty then delete it.
      if (!string.IsNullOrEmpty(subFolderFullPath) && Directory.EnumerateDirectories(subFolderFullPath).Count() == 0)
      {
        Directory.Delete(subFolderFullPath);
        Logger.Info("Sub folder deleted: " + subFolderFullPath);
      }
      Logger.Info("Invoice Offline Collection documents copied.");
    }

    /// <summary>
    /// This will delete the options which are NOT there in the invoiceOfflineCollectionMetaDataCollection
    /// </summary>
    /// <param name="invoiceOfflineCollectionMetaDataCollection">invoice Offline Collection Meta Data Collection</param>
    private static List<string> GetUpdatedOptions(IEnumerable<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection)
    {
      var newOptions = new List<string>();
      foreach (var invoiceOfflineCollectionMetaData in invoiceOfflineCollectionMetaDataCollection)
      {
        if (!newOptions.Contains(invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId.ToString()))
        {
          // SCP260324: Unable to download invoices from SIS System
          // Check if directory exist, otherwise skip
          if (Directory.Exists(invoiceOfflineCollectionMetaData.FilePath))
          {
            //Add option only if the respective files are present/generated in the directory
            //For Supporting Doc case we have folders inside the SUPPORT folder hence check for GetDirectories method
            if (Directory.GetFiles(invoiceOfflineCollectionMetaData.FilePath).Count() > 0 || Directory.GetDirectories(invoiceOfflineCollectionMetaData.FilePath).Count() > 0)
              newOptions.Add(invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId.ToString());
          }
        }
      }
      return newOptions;
    }

    /// <summary>
    /// Create base folder as per specification in IS File Specs
    /// </summary>
    /// <param name="member">member</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isFormC">isFormC</param>
    /// <param name="formCProvMonth">Provisional month of Form C</param>
    /// <param name="formCProvYear">Form C Provisional Year</param>
    /// <param name="isZipFileName">Note : If this flag is set, method will return Zip file name else it will return Folder name</param>
    /// <param name="isWebZip">Set this flag to true for Web zip icon click</param>
    /// <param name="zipFileName">Pass this value only for Web click : Will be used to create unique base folder for two invoices of same member</param>
    /// <param name="targetDate">CMP529: If Daily OAR required then target date is required.</param>
    /// <param name="isDailyOARRequired">CMP529: If Dail OAR required then this flag will be true.</param>
    /// <param name="nilFile">CMP529: If append "-NODATA.TXT" in zipfile name.</param>
    /// <param name="locationId">Location ID for location specific OAR</param>
    /// <param name="isLocationSpecNilFile">if nil file for location specific is required.</param>
    /// <returns>Returns the offline archive parent folder path</returns>
    public string GetParentFolderName(Member member, BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, bool isReceivable, bool isFormC, int formCProvMonth, int formCProvYear, bool isZipFileName, bool isWebZip = false, string zipFileName = null, DateTime? targetDate=null,bool isDailyOARRequired = false, bool nilFile= false, string locationId = null, bool isLocationSpecNilFile = false)
    {
      try
      {
        string type = isReceivable ? "R" : "P";
        string billingCategoryString = isFormC ? "FORMC" : billingCategoryType.ToString().ToUpper();

        string billingAirlinePrefix = member.MemberCodeAlpha;
        /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: No code change required here. 
        Because member code numeric fetched from database will be used as is for file name creation. 
        Ref: FRS Section 3.6 Table 24 Row 7 */
        string billingAirlineNumericCode = member.MemberCodeNumeric;

        string billingYearMonthPeriod;
        if (isFormC)
        {
          var currentDateTime = DateTime.UtcNow;
          billingYearMonthPeriod = string.Format("{0}{1}{2}", currentDateTime.Year, currentDateTime.Month.ToString().PadLeft(2, '0'), currentDateTime.Day.ToString().PadLeft(2, '0'));
        }
        else
          billingYearMonthPeriod = string.Format("{0}{1}{2}", billingPeriod.Year, billingPeriod.Month.ToString().PadLeft(2, '0'), billingPeriod.Period.ToString().PadLeft(2, '0'));

        string folderName;
        if (isZipFileName)
        {
          const string prefix = "OAR";
          if (isFormC)
          {
            var provisinalDate = string.Format("{0}{1}", formCProvYear, formCProvMonth.ToString().PadLeft(2, '0'));
            folderName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", prefix, billingCategoryString, type, billingAirlineNumericCode, provisinalDate, billingYearMonthPeriod);
          }
          if (!string.IsNullOrEmpty(locationId))
          {
            //CMP#622 : build search criteria on basis of output type for output process :
            /*
            Field 1: Always D-OAR to indicate Daily
            Field 2: Always MISC, such files apply for MISC Invoices/Credit Notes only  
            Field 3: Always P, such files are always sent from a Payables perspective only
            Field 4 (CC): The prefix of the receiver/Billed Member  (e.g. JL for Japan Airlines)
            Field 5 (DDDDDDDDDDDD): The accounting code of the receiver/Billed Member (e.g. 131 for Japan Airlines). ). The length of this field will range between 3 and 12
            Field 6 (YYYYMMDD): The delivery date of the OAR
            Field 7: Always L followed by MMMMMMM: The Location ID of the target recipient for which the file is generated. The length of Location ID will range between 1 and 7 
            Ex: D-OAR-MISC-P-131-20161211-L12.zip
             */
              folderName = isDailyOARRequired
                               ? (string.Format("{0}-{1}-{2}-{3}-{4}-{5}-L{6}",
                                                "D",
                                                prefix,
                                                billingCategoryString,
                                                type,
                                                billingAirlineNumericCode,
                                                targetDate.HasValue ? targetDate.Value.ToString("yyyyMMdd") : string.Empty,
                                                string.IsNullOrEmpty(locationId) == false ? isLocationSpecNilFile ? string.Format("{0}-{1}", locationId, "NODATA") : locationId : string.Empty))
                               : string.Format("{0}-{1}-{2}-{3}-{4}-L{5}",
                                               prefix,
                                               billingCategoryString,
                                               type,
                                               billingAirlineNumericCode,
                                               billingYearMonthPeriod,
                                               (isLocationSpecNilFile ? (string.Format("{0}-{1}", locationId, "NODATA")) : locationId));
          }
          else
          {
            //CMP529: if Daily OAR requied then pattern will be :
            /*
             Field 1: Always D to indicate Daily
             Field 2: Always MISC, such files apply for MISC Invoices/Credit Notes only  
             Field 3: Always P, such files are always sent from a Payables perspective only
             Field 4 (CC): The prefix of the receiver/Billed Member  (e.g. JL for Japan Airlines)
             Field 5 (DDD): The accounting code of the receiver/Billed Member (e.g. 131 for Japan Airlines)
             Field 6 (YYYYMMDD): The delivery date of the OAR
             Ex: D-OAR-MISC-P-131-20131211
             */

            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
                Desc: No code change required here. Because - 
                * 1. i/p = 57 then PadLeft(3, '0') returns 057
                * 2. i/p = 125 then PadLeft(3, '0') returns 125
                * 3. i/p = 4300 then PadLeft(3, '0') returns 4300
                * Ref: FRS Section 3.6 Table 24 Row 18 and 19
                */
            folderName = isDailyOARRequired
                           ? string.Format("{0}-{1}-{2}-{3}-{4}-{5}", "D", prefix, billingCategoryString, type,
                                           billingAirlineNumericCode, targetDate.HasValue ? nilFile ? string.Format("{0}-{1}", targetDate.Value.ToString("yyyyMMdd"), "NODATA") : targetDate.Value.ToString("yyyyMMdd") : string.Empty)
                           : string.Format("{0}-{1}-{2}-{3}-{4}", prefix, billingCategoryString, type,
                                           billingAirlineNumericCode, billingYearMonthPeriod);
          }
        }
        else
        {
          if (isFormC)
          {
            var provisinalDate = string.Format("{0}{1}", formCProvYear, formCProvMonth.ToString().PadLeft(2, '0'));
            folderName = string.Format("{0}-{1}-{2}-{3}-{4}", billingCategoryString, type, billingAirlinePrefix, billingAirlineNumericCode, billingYearMonthPeriod);
            //folderName = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", billingCategoryString, type, billingAirlinePrefix, billingAirlineNumericCode, provisinalDate, billingYearMonthPeriod);
          }
          if (!string.IsNullOrEmpty(locationId))
          {
            //CMP622: build search criteria on basis of output type for output process :
            /*
            Field 1: Always D to indicate Daily
            Field 2: Always MISC, such files apply for MISC Invoices/Credit Notes only  
            Field 3: Always P, such files are always sent from a Payables perspective only
            Field 4 (CC): The prefix of the receiver/Billed Member  (e.g. JL for Japan Airlines)
            Field 5 (DDDDDDDDDDDD): The accounting code of the receiver/Billed Member (e.g. 131 for Japan Airlines). ). The length of this field will range between 3 and 12
            Field 6 (YYYYMMDD): The delivery date of the OAR
            Field 7: Always L followed by MMMMMMM: The Location ID of the target recipient for which the file is generated. The length of Location ID will range between 1 and 7 
            Ex: D-MISC-P-JL-131-20131211-L12
             */
              folderName = isDailyOARRequired
                               ? (string.Format("{0}-{1}-{2}-{3}-{4}-{5}-L{6}",
                                                "D",
                                                billingCategoryString,
                                                type,
                                                billingAirlinePrefix,
                                                billingAirlineNumericCode,
                                                targetDate.HasValue ? targetDate.Value.ToString("yyyyMMdd") : string.Empty,
                                                locationId))
                               : string.Format("{0}-{1}-{2}-{3}-{4}-L{5}",
                                               billingCategoryString,
                                               type,
                                               billingAirlinePrefix,
                                               billingAirlineNumericCode,
                                               billingYearMonthPeriod,
                                               (isLocationSpecNilFile ? (string.Format("{0}-{1}", locationId, "NODATA")) : locationId));
          }
          else
          {
            //CMP529: if Daily OAR requied then pattern will be :
            /*
             Field 1: Always D to indicate Daily
             Field 2: Always MISC, such files apply for MISC Invoices/Credit Notes only  
             Field 3: Always P, such files are always sent from a Payables perspective only
             Field 4 (CC): The prefix of the receiver/Billed Member  (e.g. JL for Japan Airlines)
             Field 5 (DDD): The accounting code of the receiver/Billed Member (e.g. 131 for Japan Airlines)
             Field 6 (YYYYMMDD): The delivery date of the OAR
             Ex: D-MISC-P-JL-131-20131211
             */
            folderName = isDailyOARRequired
                           ? string.Format("{0}-{1}-{2}-{3}-{4}-{5}", "D", billingCategoryString, type,
                                           billingAirlinePrefix, billingAirlineNumericCode,
                                           targetDate.HasValue?targetDate.Value.ToString("yyyyMMdd"):string.Empty)
                           : string.Format("{0}-{1}-{2}-{3}-{4}", billingCategoryString, type, billingAirlinePrefix,
                                           billingAirlineNumericCode, billingYearMonthPeriod);
          }
        }
        //Temporary base path to be configured and fetched from config.
        string basePath = FileIo.GetForlderPath(SFRFolderPath.ISOARFolderPath); // ConfigurationManager.AppSettings["OfflineCollectionDownloadSFR"];)
        //string basePath = @"D:\Report"; // ConfigurationManager.AppSettings["OfflineCollectionDownloadSFR"];)
        if (isWebZip && zipFileName != null)
        {
          basePath = Path.Combine(basePath, zipFileName);
          Logger.InfoFormat("Created new folder path :[{0}]", basePath);
        }
        if (!Directory.Exists(basePath))
        {
          Directory.CreateDirectory(basePath);
        }

        string folderFullPath = Path.Combine(basePath, folderName);

        if (!Directory.Exists(folderFullPath))
        {
          Directory.CreateDirectory(folderFullPath);
        }


        return folderFullPath;
      }
      catch (Exception ex)
      {
        Logger.Error("Error creating base folder", ex);
        throw;
      }
    }

    /// <summary>
    /// Creates sub folder as per specification in IS File Specs
    /// </summary>
    /// <param name="baseFolder">base Folder path</param>
    /// <param name="memberId">memberId</param>
    /// <param name="uniqueIdentityPerRecord">For Invoices = {invoiceNumber}{AirlineId}  or CodeYYYMM00 (Code is Member code of the member)in case of FORMC</param>
    /// <param name="options">options</param>
    /// <param name="isFormC">isFormC</param>
    /// <param name="subFolderFullPath">The sub folder full path.</param>
    /// <param name="formCIdentity">formCIdentity is = {provYear}{provMonth}00"</param>
    /// <param name="invoiceIdentity">Invoice Number</param>
    private void CreateSubFolders(string baseFolder, int memberId, string uniqueIdentityPerRecord, List<string> options, bool isFormC, out string subFolderFullPath, string formCIdentity = null, string invoiceIdentity = null)
    {
      subFolderFullPath = string.Empty;
      try
      {
        var member = _memberManager.GetMember(memberId);

        var subFolderName = string.Format("{0}-{1}", member.MemberCodeAlpha, member.MemberCodeNumeric);
        subFolderFullPath = Path.Combine(baseFolder, subFolderName);

        if (!Directory.Exists(subFolderFullPath))
        {
          Directory.CreateDirectory(subFolderFullPath);
        }

        //Build Index.xml path
        var indexXmlPath = Path.Combine(baseFolder, "INDEX.XML");
        _indexXmlPath = indexXmlPath;

        //If options count is greater that zero then only add the path on Dictionary object
        if (options.Count > 0)
        {
          if (!_hPerInvoiceDirectory.ContainsKey(uniqueIdentityPerRecord))
          {
            _hPerInvoiceDirectory.Add(uniqueIdentityPerRecord, new Dictionary<string, InvoiceOfflineCollectionFilePath>());
            Logger.InfoFormat("Added new entry - {0} in _hPerInvoiceDirectory.", uniqueIdentityPerRecord);
          }

          Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName;
          _hPerInvoiceDirectory.TryGetValue(uniqueIdentityPerRecord, out hDocumentFileName);

          //Create Invoice folder
          var invoiceFolderPath = isFormC ? Path.Combine(subFolderFullPath, string.Format("FORMC-{0}", formCIdentity)) : Path.Combine(subFolderFullPath, string.Format("INV-{0}", invoiceIdentity));
          if (!Directory.Exists(invoiceFolderPath))
          {
            Directory.CreateDirectory(invoiceFolderPath);
          }

          var relativePath = isFormC
                               ? string.Format("/{0}/{1}", subFolderName, string.Format("FORMC-{0}", formCIdentity))
                               : string.Format("/{0}/{1}", subFolderName, string.Format("INV-{0}", invoiceIdentity));
          //Create E-INVOICE folder
          if (options.Contains("1"))
          {
            var eInvoiceFolderPath = Path.Combine(invoiceFolderPath, EInvoiceFolderNameConstant);
            if (!Directory.Exists(eInvoiceFolderPath))
            {
              Directory.CreateDirectory(eInvoiceFolderPath);
            }
            hDocumentFileName.Add(EInvoiceFolderNameConstant,
                                  new InvoiceOfflineCollectionFilePath
                                    {
                                      PhysicalPath = eInvoiceFolderPath,
                                      RelativePath = string.Format("{0}/{1}", relativePath, EInvoiceFolderNameConstant)
                                    });
            Logger.InfoFormat("Added new entry of EInvoiceFolderPath against entry {0}.", uniqueIdentityPerRecord);
          }

          //Create LISTINGS folder
          if (options.Contains("2"))
          {
            var listingFolderPath = Path.Combine(invoiceFolderPath, ListingsFolderNameConstant);
            if (!Directory.Exists(listingFolderPath))
            {
              Directory.CreateDirectory(listingFolderPath);
            }
            hDocumentFileName.Add(ListingsFolderNameConstant,
                                  new InvoiceOfflineCollectionFilePath
                                    {
                                      PhysicalPath = listingFolderPath,
                                      RelativePath = string.Format("{0}/{1}", relativePath, ListingsFolderNameConstant)
                                    });
            Logger.InfoFormat("Added new entry of ListingFolderPath against entry {0}.", uniqueIdentityPerRecord);
          }

          //Create MEMOS folder
          if (options.Contains("3"))
          {
            var memoFolderPath = Path.Combine(invoiceFolderPath, MemosFolderNameConstant);
            if (!Directory.Exists(memoFolderPath))
            {
              Directory.CreateDirectory(memoFolderPath);
            }
            hDocumentFileName.Add(MemosFolderNameConstant,
                                  new InvoiceOfflineCollectionFilePath
                                    {
                                      PhysicalPath = memoFolderPath,
                                      RelativePath = string.Format("{0}/{1}", relativePath, MemosFolderNameConstant)
                                    });
            Logger.InfoFormat("Added new entry of MemoFolderPath against entry {0}.", uniqueIdentityPerRecord);
          }

          //Create SUPPDOCS folder
          if (options.Contains("4"))
          {
            var supportingDocFolderPath = Path.Combine(invoiceFolderPath, SuppdocsFolderNameConstant);
            if (!Directory.Exists(supportingDocFolderPath))
            {
              Directory.CreateDirectory(supportingDocFolderPath);
            }
            hDocumentFileName.Add(SuppdocsFolderNameConstant,
                                  new InvoiceOfflineCollectionFilePath
                                    {
                                      PhysicalPath = supportingDocFolderPath,
                                      RelativePath = string.Format("{0}/{1}", relativePath, SuppdocsFolderNameConstant)
                                    });
            Logger.InfoFormat("Added new entry of SupportingDocFolderPath against entry {0}.", uniqueIdentityPerRecord);
          }
          _hPerInvoiceDirectory.Remove(uniqueIdentityPerRecord);
          _hPerInvoiceDirectory.Add(uniqueIdentityPerRecord, hDocumentFileName);
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error creating sub-folder", ex);
        throw;
      }
    }

    /// <summary>
    /// Copies the invoice offline collection to respective destination folder.
    /// </summary>
    /// <param name="invoiceOfflineCollectionMetaDataCollection">invoice Offline Collection Meta Data Collection</param>
    /// <param name="uniqueIdentityPerRecord">Unique Identity Per Record</param>
    /// <param name="isFormC"></param>
    /// <param name="isReceivable"></param>
    /// <param name="member"></param>
    /// <param name="billingCategoryType"></param>
    /// <param name="isDailyOARRequired"> True for Daily OAR</param>
    private void CopyInvoiceOfflineCollection(IEnumerable<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection, string uniqueIdentityPerRecord, bool isFormC, bool isReceivable, Member member, BillingCategoryType billingCategoryType, bool isDailyOARRequired = false)
    {
      string memberCode = member.MemberCodeNumeric;
      Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName;
      InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePath;

      if (_hPerInvoiceDirectory.TryGetValue(uniqueIdentityPerRecord, out hDocumentFileName))
      {

        foreach (var invoiceOfflineCollectionMetaData in invoiceOfflineCollectionMetaDataCollection)
        {
          if (Directory.Exists(invoiceOfflineCollectionMetaData.FilePath))
          {
            if (invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId == 1)
            {
              //Copy EInvoices
              if (hDocumentFileName.TryGetValue(EInvoiceFolderNameConstant, out invoiceOfflineCollectionFilePath))
              {
                try
                {
                  // SCP387738: FW: Wrong contents of D-OAR-MISC-P-D9N-20150710.ZIP
                  CopyEInvoiceDocuments(invoiceOfflineCollectionMetaData.FilePath, invoiceOfflineCollectionFilePath.PhysicalPath, member, isReceivable, billingCategoryType, isDailyOARRequired);
                }
                catch (Exception exception)
                {
                  Logger.InfoFormat("Exception occurred while copying E-Invoice  documents - {0}", exception.Message);
                }

                //FileIo.CopyFolder(invoiceOfflineCollectionMetaData.FilePath, invoiceOfflineCollectionFilePath.PhysicalPath);
              }
            }
            else if (invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId == 2)
            {
              //Copy Listings
              if (hDocumentFileName.TryGetValue(ListingsFolderNameConstant, out invoiceOfflineCollectionFilePath))
              {
                FileIo.CopyFolder(invoiceOfflineCollectionMetaData.FilePath, invoiceOfflineCollectionFilePath.PhysicalPath);
                if (isFormC)
                {
                  //Logic : Since Form C listing files are generated and copied for both Payables and receivables prospective
                  //we have to delete other listing file
                  //Note : delete the listing files where in the member code for the member for which we are downloading a file is there in the filename
                  DeleteFormCListingCsv(invoiceOfflineCollectionFilePath.PhysicalPath, memberCode);
                }
              }
            }
            else if (invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId == 3)
            {
              //Copy Memos
              if (hDocumentFileName.TryGetValue(MemosFolderNameConstant, out invoiceOfflineCollectionFilePath))
              {
                FileIo.CopyFolder(invoiceOfflineCollectionMetaData.FilePath, invoiceOfflineCollectionFilePath.PhysicalPath);
              }
            }
            else if (invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId == 4)
            {
              //Copy Supporting Docs
              if (hDocumentFileName.TryGetValue(SuppdocsFolderNameConstant, out invoiceOfflineCollectionFilePath))
              {
                FileIo.CopyFolder(invoiceOfflineCollectionMetaData.FilePath, invoiceOfflineCollectionFilePath.PhysicalPath, true);
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// This function will copy E-Invoice documents based on member's member profile attribute
    /// </summary>
    /// <param name="sourceEInvoiceFolderPath">sourceEInvoiceFolderPath</param>
    /// <param name="destinationEInvoiceFolderPath">destinationEInvoiceFolderPath</param>
    /// <param name="member">member</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isDailyOARRequired"> True if Daily OAR</param>
    private void CopyEInvoiceDocuments(string sourceEInvoiceFolderPath, string destinationEInvoiceFolderPath, Member member, bool isReceivable, BillingCategoryType billingCategoryType, bool isDailyOARRequired = false)
    {
      bool dsRequired = false, pdfRequired = false;
      if (billingCategoryType == BillingCategoryType.Pax)
      {
        var paxConfigurationRepository = Ioc.Resolve<IPassengerConfigurationRepository>();
        if (paxConfigurationRepository != null)
        {
          var paxConfiguration = paxConfigurationRepository.First(i => i.MemberId == member.Id);
          if (paxConfiguration != null)
          {
            if (isReceivable)
            {
              dsRequired = paxConfiguration.IsDsFileAsOtherOutputAsBillingEntity;
              pdfRequired = paxConfiguration.IsPdfAsOtherOutputAsBillingEntity;
            }
            else
            {
              dsRequired = paxConfiguration.IsDsFileAsOtherOutputAsBilledEntity;
              pdfRequired = paxConfiguration.IsPdfAsOtherOutputAsBilledEntity;
            }
          }
        }
      }
      else if (billingCategoryType == BillingCategoryType.Misc)
      {
        var miscConfigurationRepository = Ioc.Resolve<IMiscConfigurationRepository>();
        if (miscConfigurationRepository != null)
        {
          var miscConfiguration = miscConfigurationRepository.First(i => i.MemberId == member.Id);
          if (miscConfiguration != null)
          {
            if (isReceivable)
            {
              dsRequired = miscConfiguration.IsDsFileAsOtherOutputAsBillingEntity;
              pdfRequired = miscConfiguration.IsPdfAsOtherOutputAsBillingEntity;
            }
            else
            {
              dsRequired = miscConfiguration.IsDsFileAsOtherOutputAsBilledEntity;
              pdfRequired = miscConfiguration.IsPdfAsOtherOutputAsBilledEntity;

              // SCP387738: FW: Wrong contents of D-OAR-MISC-P-D9N-20150710.ZIP
              // This is common method used by all type and all the possible ways of OAR generation.
              // As per CMP-529-Daily Output Generation for MISC Bilateral Invoices-v1.2
              // E-Invoice must included in Daily OAR, irrespective of member misc configuration for Invoice PDF.
              if(isDailyOARRequired)
              {
                dsRequired = true;
                pdfRequired = true;
              }
            }
          }
        }
      }
      else if (billingCategoryType == BillingCategoryType.Uatp)
      {
        var uatpConfigurationRepository = Ioc.Resolve<IUatpConfigurationRepository>();
        if (uatpConfigurationRepository != null)
        {
          var uatpConfiguration = uatpConfigurationRepository.First(i => i.MemberId == member.Id);
          if (uatpConfiguration != null)
          {
            if (isReceivable)
            {
              dsRequired = uatpConfiguration.IsDsFileAsOtherOutputAsBillingEntity;
              pdfRequired = uatpConfiguration.IsPdfAsOtherOutputAsBillingEntity;
            }
            else
            {
              dsRequired = uatpConfiguration.IsDsFileAsOtherOutputAsBilledEntity;
              pdfRequired = uatpConfiguration.IsPdfAsOtherOutputAsBilledEntity;
            }
          }
        }
      }
      else if (billingCategoryType == BillingCategoryType.Cgo)
      {
        var cargoConfigurationRepository = Ioc.Resolve<ICargoConfigurationRepository>();
        if (cargoConfigurationRepository != null)
        {
          var cargoConfiguration = cargoConfigurationRepository.First(i => i.MemberId == member.Id);
          if (cargoConfiguration != null)
          {
            if (isReceivable)
            {
              dsRequired = cargoConfiguration.IsDsFileAsOtherOutputAsBillingEntity;
              pdfRequired = cargoConfiguration.IsPdfAsOtherOutputAsBillingEntity;
            }
            else
            {
              dsRequired = cargoConfiguration.IsDsFileAsOtherOutputAsBilledEntity;
              pdfRequired = cargoConfiguration.IsPdfAsOtherOutputAsBilledEntity;
            }
          }
        }
      }

      if (dsRequired && pdfRequired)
      {
        //Copy whole folder
        if (isReceivable)
        {
          //NOTE: for Receivable do not copy Xml Verifigation file (Naming convention - AXVF-CC-DDD-NNNNNNNNNN.XML)
          //Copy all excluding Xml Verifigation file
          foreach (var enumerateFile in Directory.EnumerateFiles(sourceEInvoiceFolderPath))
          {
            if (Path.GetExtension(enumerateFile).ToLower().CompareTo(".xml") == 0)
            {
              if (Path.GetFileName(enumerateFile).Substring(1, 3).CompareTo("XVF") != 0)
                File.Copy(enumerateFile, Path.Combine(destinationEInvoiceFolderPath, Path.GetFileName(enumerateFile)), true);
              else
                Logger.Info("Ignoring Xml Verifigation file ");
            }
            else
            {
              //Copy Invoice PDF
              File.Copy(enumerateFile, Path.Combine(destinationEInvoiceFolderPath, Path.GetFileName(enumerateFile)), true);
            }
          }
        }
        else
        {
          Logger.Info("Copying whole e-invoice folder");
          FileIo.CopyFolder(sourceEInvoiceFolderPath, destinationEInvoiceFolderPath);
        }
      }
      else if (dsRequired)
      {
        //Copy only ds files if exists in the E-Invoice folder
        //NOTE: for Receivable do not copy Xml Verifigation file (Naming convention - AXVF-CC-DDD-NNNNNNNNNN.XML)
        Logger.Info("Copying only Digital signature documents folder");
        foreach (var enumerateFile in Directory.EnumerateFiles(sourceEInvoiceFolderPath))
        {
          if (Path.GetExtension(enumerateFile).ToLower().CompareTo(".xml") == 0)
          {
            if (File.Exists(enumerateFile))
            {
              if (isReceivable)
              {
                if (Path.GetFileName(enumerateFile).Substring(1, 3).CompareTo("XVF") != 0)
                  File.Copy(enumerateFile, Path.Combine(destinationEInvoiceFolderPath, Path.GetFileName(enumerateFile)), true);
                else
                  Logger.Info("Ignoring Xml Verifigation file ");
              }
              else
              {
                File.Copy(enumerateFile, Path.Combine(destinationEInvoiceFolderPath, Path.GetFileName(enumerateFile)), true);
              }
            }
          }
        }
      }
      else if (pdfRequired)
      {
        //Copy only Pdf file if exists in the E-Invoice folder
        Logger.Info("Copying only Pdf documents folder");
        foreach (var enumerateFile in Directory.EnumerateFiles(sourceEInvoiceFolderPath))
        {
          if (Path.GetExtension(enumerateFile).ToLower().CompareTo(".pdf") == 0)
          {
            if (File.Exists(enumerateFile))
            {
              File.Copy(enumerateFile, Path.Combine(destinationEInvoiceFolderPath, Path.GetFileName(enumerateFile)), true);
            }
          }
        }
      }
      // If E-Invoice is not copied to archieve folder then delete empty E-Invoice folder.
      else if (Directory.EnumerateFiles(destinationEInvoiceFolderPath).Count() == 0)
      {
        Directory.Delete(destinationEInvoiceFolderPath);
      }
    }

    /// <summary>
    /// Delete Form C listing csv
    /// </summary>
    /// <param name="listingFolderPath"></param>
    /// <param name="memberCode"></param>
    private void DeleteFormCListingCsv(string listingFolderPath, string memberCode)
    {
      try
      {
        Logger.Info("Deleting unneccessory listing files");
        if (Directory.Exists(listingFolderPath))
        {
          foreach (var filePath in Directory.GetFiles(listingFolderPath))
          {
            var fileName = Path.GetFileName(filePath);
            //fileName is in format PFORMC-DDD -YYYYMMPP-EEE.CSV
            var array = fileName.Split('-');
            if (array.Count() > 0)
            {
              if (array[1].CompareTo(memberCode) == 0)
              {
                File.Delete(filePath);
                Logger.InfoFormat("Deleted file {0}", filePath);
              }
            }
          }
        }
      }
      catch (Exception)
      {
        Logger.ErrorFormat("Error while deleting Csv from file path {0}", listingFolderPath);
      }
    }

    /// <summary>
    /// Creates index.xml to provide the zip folder content details.
    /// </summary>
    /// <param name="invoiceBases">Collection of invoiceBase</param>
    /// <param name="samplingFormCs">Collection of SamplingFormC</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="memberId">For Receivable it should be BillingMemberId and For Payable it should be BilledMemberId</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="targetDate">CMP529: If Daily OAR required then target date is required.</param>
    /// <param name="isDailyOARRequired">CMP529: If Dail OAR required then this flag will be true.</param>
    /// <param name="isFormC"></param>
    private void CreateIndexFile(IEnumerable<InvoiceBase> invoiceBases, IEnumerable<SamplingFormC> samplingFormCs, BillingPeriod billingPeriod, BillingCategoryType billingCategoryType, int memberId, bool isReceivable, bool isFormC, DateTime? targetDate = null, bool isDailyOARRequired = false, string locationId = null)
    {
      try
      {
        var xDoc = new XmlDocument();

        //Add the XML declaration section
        XmlNode xmlnode = xDoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
        xDoc.AppendChild(xmlnode);

        //Add root element
        XmlElement xmlRootElement = (isReceivable) ? xDoc.CreateElement("", "SISReceivablesIndexTransmission", "") : isDailyOARRequired ? xDoc.CreateElement("", "SISPayablesDailyMiscIndexTransmission", "") : xDoc.CreateElement("", "SISPayablesIndexTransmission", "");
        XmlText xmlRootText = xDoc.CreateTextNode("");
        xmlRootElement.AppendChild(xmlRootText);
        xDoc.AppendChild(xmlRootElement);

        const string xmlns = "http://www.w3.org/2000/xmlns/", xsi = "http://www.w3.org/2001/XMLSchema-instance";
        
        #region CMP #596: Length of Member Accounting Code to be Increased to 12
        
          /*   Desc: Code changed to have billing category specific separate XSD URLs
           Ref: FRS Section 3.6 Table 24 Row 7 */
          var noNameSpaceLocation = string.Empty;
          if(isReceivable)
          {
              var billingCategory = GetBillingCategory(billingCategoryType, isFormC);
              switch(billingCategory)
              {
                  case "F":
                  case "P":
                  case "C":
                      noNameSpaceLocation =
                          Core.Configuration.ConnectionString.GetconfigAppSetting("SISReceivablesPaxCargoIndexTransLocation");
                      break;
                  case "M":
                  case "U":
                      noNameSpaceLocation =
                          Core.Configuration.ConnectionString.GetconfigAppSetting("SISReceivablesMiscUatpIndexTransLocation");
                      break;
              }
          }
          else //Payable block
          {
              if(isDailyOARRequired)
              {
                  noNameSpaceLocation =
                      Core.Configuration.ConnectionString.GetconfigAppSetting("SISPayablesDailyMiscIndexTransLocation");
              }
              else
              {
                  var billingCategory = GetBillingCategory(billingCategoryType, isFormC);
                  switch (billingCategory)
                  {
                      case "F":
                      case "P":
                      case "C":
                          noNameSpaceLocation =
                              Core.Configuration.ConnectionString.GetconfigAppSetting("SISPayablesPaxCargoIndexTransLocation");
                          break;
                      case "M":
                      case "U":
                          noNameSpaceLocation =
                              Core.Configuration.ConnectionString.GetconfigAppSetting("SISPayablesMiscUatpIndexTransLocation");
                          break;
                  }
              }
          }

        #endregion

        XmlAttribute xsiNamespace = xDoc.CreateAttribute("xmlns:xsi", xmlns);
        xsiNamespace.Value = xsi;
        xmlRootElement.SetAttributeNode(xsiNamespace);

        xsiNamespace = xDoc.CreateAttribute("xsi:noNamespaceSchemaLocation", xsi);
        xsiNamespace.Value = noNameSpaceLocation;
        xmlRootElement.SetAttributeNode(xsiNamespace);

        XmlElement xmlHeaderElement = AddElement(xDoc, (isReceivable) ? "SISReceivablesIndexHeader" : isDailyOARRequired ? "SISPayablesDailyMiscIndexHeader" : "SISPayablesIndexHeader", "", xmlRootElement);
        AddElement(xDoc, "Version", (isReceivable) ? "IATA:SISReceivablesIndexV1.0.0.0" : isDailyOARRequired ? "IATA:SISPayablesDailyMiscIndexTransmission V1.0.0" : "IATA:SISPayablesIndexV1.0.0.0", xmlHeaderElement);

        //Add new Guid
        AddElement(xDoc, "TransmissionID", Guid.NewGuid().ToString(), xmlHeaderElement);

          var member = _memberManager.GetMember(memberId);

        //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
        //Create new node as "TransmissionData" with attribute for daily OAR only
        if (targetDate.HasValue)
        {
            if (ReferenceManager == null)
                ReferenceManager = Ioc.Resolve<IReferenceManager>(typeof (IReferenceManager));

            var dailyOutputProcessLog = ReferenceManager.GetDailyOutputProcessLogData(memberId, locationId, targetDate.Value);

            if (dailyOutputProcessLog != null)
            {
                //Get file name for Daily OAR.
                string fileName = String.Format("{0}.ZIP",
                                                Path.GetFileName(GetParentFolderName(member, billingCategoryType,
                                                                                     new BillingPeriod(), false, false,
                                                                                     0,
                                                                                     0, true,
                                                                                     targetDate:
                                                                                         dailyOutputProcessLog.
                                                                                         TargetDate,
                                                                                     isDailyOARRequired: true,
                                                                                     locationId: locationId)));


                var element = AddElement(xDoc, "TransmissionData", fileName, xmlHeaderElement);
                element.SetAttribute("Name", "PreviousISOutputFileName");
            }
            else
            {
                var element = AddElement(xDoc, "TransmissionData", "None", xmlHeaderElement);
                element.SetAttribute("Name", "PreviousISOutputFileName");
            }
        }
         
        AddElement(xDoc, isReceivable ? "BillingMember" : "BilledMember", member.MemberCodeNumeric, xmlHeaderElement);

        var billingYear = billingPeriod.Year.ToString();


        //CMP529: If DailyOAR required then create Delivery Date Tag. 
        if (isDailyOARRequired)
        {
          AddElement(xDoc, "DeliveryDate", targetDate.HasValue ? targetDate.Value.ToString("yyyy-MM-dd") : string.Empty, xmlHeaderElement);
        }
        else
        {
          //The Clearance Month for Form C will be the Provisional Billing Month.
          if (isFormC && samplingFormCs.Count() > 0)
          {
            var provisionalBillingYear = samplingFormCs.ToList()[0].ProvisionalBillingYear;
            var provisionalBillingYearString = provisionalBillingYear.ToString();
            // If provisional billing year is 4 digit get last 2 digit in year else if length is 2 then use it as it is.
            provisionalBillingYearString = provisionalBillingYearString.Length == 2
                                             ? provisionalBillingYearString
                                             : provisionalBillingYearString.Substring(2, 2);
            AddElement(xDoc, "ClearanceMonth", string.Format("{0}{1}", samplingFormCs.ToList()[0].ProvisionalBillingMonth.ToString().PadLeft(2, '0'), provisionalBillingYearString), xmlHeaderElement);
          }
          else
            AddElement(xDoc, "ClearanceMonth", string.Format("{0}{1}", billingPeriod.Month.ToString().PadLeft(2, '0'), billingYear.Substring(billingYear.Length - 2, 2)), xmlHeaderElement);

          AddElement(xDoc, "PeriodNumber", isFormC ? "00" : billingPeriod.Period.ToString(), xmlHeaderElement);
        }
       
        AddElement(xDoc, "BillingCategory", GetBillingCategory(billingCategoryType, isFormC), xmlHeaderElement);

        foreach (var invoiceBase in invoiceBases)
        {
          GenerateInvoiceIndexFile(xDoc, isReceivable, xmlHeaderElement, invoiceBase, isDailyOARRequired:isDailyOARRequired);
        }

        foreach (var samplingFormC in samplingFormCs)
        {
          GenerateSamplingFormCIndexFile(xDoc, isReceivable, xmlHeaderElement, samplingFormC);
        }
        xDoc.Save(_indexXmlPath);
      }
      catch (Exception exception)
      {
        Logger.Error(string.Format("Error creating index file [{0}]", _indexXmlPath), exception);
        throw;
      }
    }

    /// <summary>
    /// This will append the data in the Index file for a Sampling Form C invoice 
    /// </summary>
    /// <param name="xDoc">xDoc</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="xmlHeaderElement">xmlHeaderElement</param>
    /// <param name="samplingFormC">samplingFormC</param>
    private void GenerateSamplingFormCIndexFile(XmlDocument xDoc, bool isReceivable, XmlElement xmlHeaderElement, SamplingFormC samplingFormC)
    {
      XmlElement xmlInvoiceHeaderElement = AppendInvoiceHeaderData(xDoc, xmlHeaderElement, isReceivable, true, null, samplingFormC);

      Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName;
      var samplingFormCIdentity = string.Format("{0}{1}{2}00", (isReceivable ? samplingFormC.ProvisionalBillingMemberId : samplingFormC.FromMemberId), samplingFormC.ProvisionalBillingYear, samplingFormC.ProvisionalBillingMonth.ToString().PadLeft(2, '0'));
      if (_hPerInvoiceDirectory.TryGetValue(samplingFormCIdentity, out hDocumentFileName))
      {
        //Add Detailed ListingFiles
        AppendDetailedListingData(xDoc, xmlInvoiceHeaderElement, hDocumentFileName);

        //Add Suppdocs data
        InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforSupportingdocs;
        if (hDocumentFileName.TryGetValue(SuppdocsFolderNameConstant, out invoiceOfflineCollectionFilePathforSupportingdocs) && !isReceivable)
        {
          AppendSuppDocsData(xDoc, xmlInvoiceHeaderElement, invoiceOfflineCollectionFilePathforSupportingdocs, true);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xDoc"></param>
    /// <param name="xmlHeaderElement"></param>
    /// <param name="isReceivable"></param>
    /// <param name="invoiceBase"></param>
    /// <param name="samplingFormC"></param>
    /// <param name="isFormC"></param>
    /// <param name="isDailyOARRequired">CMP 529: if Daily misc OAR requried then flag will be true</param>
    /// <returns></returns>
    private XmlElement AppendInvoiceHeaderData(XmlDocument xDoc, XmlElement xmlHeaderElement, bool isReceivable, bool isFormC, InvoiceBase invoiceBase = null, SamplingFormC samplingFormC = null, bool isDailyOARRequired = false)
    {
      XmlElement xmlInvoiceHeaderElement = AddElement(xDoc, "InvoiceHeader", "", xmlHeaderElement);
      
      if (isFormC)
      {
        if (samplingFormC != null)
        {
          Member bMember = isReceivable ? _memberManager.GetMember(samplingFormC.ProvisionalBillingMemberId) : _memberManager.GetMember(samplingFormC.FromMemberId);
          AddElement(xDoc, isReceivable ? "BilledMember" : "BillingMember", bMember.MemberCodeNumeric, xmlInvoiceHeaderElement);
          AddElement(xDoc, "InvoiceNumber", "0", xmlInvoiceHeaderElement);
          return xmlInvoiceHeaderElement;
        }
      }
      else
      {
        if (invoiceBase != null)
        {
          if (isDailyOARRequired)
          {
            //CMP529: Append Clearance Month and period
            AppendDailyOARData(xDoc, xmlInvoiceHeaderElement, invoiceBase);
          }

          Member bMember = isReceivable ? _memberManager.GetMember(invoiceBase.BilledMemberId) : _memberManager.GetMember(invoiceBase.BillingMemberId);
          AddElement(xDoc, isReceivable ? "BilledMember" : "BillingMember", bMember.MemberCodeNumeric, xmlInvoiceHeaderElement);
          AddElement(xDoc, "InvoiceNumber", invoiceBase.InvoiceNumber, xmlInvoiceHeaderElement);
          return xmlInvoiceHeaderElement;
        }
      }
      return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xDoc"></param>
    /// <param name="xmlInvoiceHeaderElement"></param>
    /// <param name="hDocumentFileName"></param>
    private static void AppendDetailedListingData(XmlDocument xDoc, XmlElement xmlInvoiceHeaderElement, Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName)
    {
      InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforListings;
      if (hDocumentFileName.TryGetValue(ListingsFolderNameConstant, out invoiceOfflineCollectionFilePathforListings))
      {
        var listingFiles = Directory.GetFiles(invoiceOfflineCollectionFilePathforListings.PhysicalPath);
        int srNo = 0;
        foreach (var listingFilePath in listingFiles)
        {
          srNo++;
          var listingFileName = Path.GetFileName(listingFilePath);
          var xmlDetailedListingFileElement = AddElement(xDoc, "DetailedListingFiles", string.Empty, xmlInvoiceHeaderElement);
          AddElement(xDoc, "SrNo", srNo.ToString(), xmlDetailedListingFileElement);
          AddElement(xDoc, "FileName", string.Format("{0}/{1}", invoiceOfflineCollectionFilePathforListings.RelativePath, listingFileName), xmlDetailedListingFileElement);
        }
      }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="xDoc"></param>
    /// <param name="xmlInvoiceHeaderElement"></param>
    /// <param name="invoiceBase"></param>
    private static void AppendDailyOARData(XmlDocument xDoc, XmlElement xmlInvoiceHeaderElement, InvoiceBase invoiceBase)
    {
      AddElement(xDoc, "ClearanceMonth", string.Format("{0}{1}", invoiceBase.BillingMonth.ToString().PadLeft(2, '0'), invoiceBase.BillingYear.ToString().Substring(invoiceBase.BillingYear.ToString().Length - 2, 2)), xmlInvoiceHeaderElement);
      AddElement(xDoc, "PeriodNumber", invoiceBase.BillingPeriod.ToString(), xmlInvoiceHeaderElement);
    }

    /// <summary>
    /// This will append the data in the Index file for the invoice  
    /// </summary>
    /// <param name="xDoc">xDoc</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="xmlHeaderElement">xmlHeaderElement</param>
    /// <param name="invoiceBase">invoiceBase</param>
    /// <param name="isDailyOARRequired">CMP 529: if Daily misc OAR requried then flag will be true</param>
    private void GenerateInvoiceIndexFile(XmlDocument xDoc, bool isReceivable, XmlElement xmlHeaderElement, InvoiceBase invoiceBase, bool isDailyOARRequired=false)
    {
      Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName;
      var uniqueIdentityPerRecord = String.Format("{0}{1}", invoiceBase.InvoiceNumber, isReceivable ? invoiceBase.BilledMemberId : invoiceBase.BillingMemberId);

      Logger.InfoFormat("GenerateInvoiceIndexFile() => uniqueIdentityPerRecord(Invoice Number + MemberId): {0}", uniqueIdentityPerRecord);

     
      if (_hPerInvoiceDirectory.TryGetValue(uniqueIdentityPerRecord, out hDocumentFileName))
      {
        XmlElement xmlInvoiceHeaderElement = AppendInvoiceHeaderData(xDoc, xmlHeaderElement, isReceivable, false, invoiceBase, null, isDailyOARRequired);


        //Add E-Invoicing Files
        AppendEInvoiceData(xDoc, xmlInvoiceHeaderElement, hDocumentFileName);

        //Add Detailed ListingFiles
        AppendDetailedListingData(xDoc, xmlInvoiceHeaderElement, hDocumentFileName);

        //Add Invoice Supporting Attachments for MISC
        AppendMuSuppDocsData(xDoc, isReceivable, invoiceBase, xmlInvoiceHeaderElement, hDocumentFileName);

        //Add Memos
        InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforMemos;
        InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforSupportingdocs;
        if (hDocumentFileName.TryGetValue(MemosFolderNameConstant, out invoiceOfflineCollectionFilePathforMemos) && (invoiceBase.BillingCategory == BillingCategoryType.Pax || invoiceBase.BillingCategory == BillingCategoryType.Cgo) &&
            (invoiceBase.BillingCode != 5)) //NOTE : For FORM D/E no Memos are there
        {
          AppendMemoAndSuppDocsData(xDoc, isReceivable, xmlInvoiceHeaderElement, hDocumentFileName, invoiceOfflineCollectionFilePathforMemos, invoiceBase.BillingCategory);
        }
        //Add Pax attachments
        else if (hDocumentFileName.TryGetValue(SuppdocsFolderNameConstant, out invoiceOfflineCollectionFilePathforSupportingdocs) && (invoiceBase.BillingCategory == BillingCategoryType.Pax || invoiceBase.BillingCategory == BillingCategoryType.Cgo)) //Removed check for Isreceivable as it is NOT required for UI download option
        {
          //It is Form D/E
          if (invoiceBase.BillingCode == 5) AppendSuppDocsData(xDoc, xmlInvoiceHeaderElement, invoiceOfflineCollectionFilePathforSupportingdocs, true);
          else AppendSuppDocsData(xDoc, xmlInvoiceHeaderElement, invoiceOfflineCollectionFilePathforSupportingdocs, false);
        }
      }
    }

    private void AppendSuppDocsData(XmlDocument xDoc, XmlElement xmlInvoiceHeaderElement, InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforSupportingdocs, bool isFormC, IDictionary<string, bool> processedAttachmentMemos = null)
    {
      foreach (var memoLevelFolderPath in Directory.EnumerateDirectories(invoiceOfflineCollectionFilePathforSupportingdocs.PhysicalPath))
      {
        Logger.InfoFormat("AppendSuppDocsData()=> MemoLevelFolderPath: {0}", memoLevelFolderPath);

        var memoLevelFolderName = Path.GetFileName(memoLevelFolderPath);

        if (isFormC)
        {
          var xmlBatchDetailsElement = AddElement(xDoc, "BatchDetails", string.Empty, xmlInvoiceHeaderElement);
          var stringArr = memoLevelFolderName.Split('-');
          if (stringArr.Count() == 3)
          {
            Logger.InfoFormat("ProvisionalInvoiceNumber: {0}, BatchNumber: {1}, SequenceNumber: {2}", stringArr[0], stringArr[1], stringArr[2]);

            AddElement(xDoc, "ProvisionalInvoiceNumber", stringArr[0], xmlBatchDetailsElement);
            
            var batchNumber = Convert.ToInt32(stringArr[1]);
            var sequenceNumber = Convert.ToInt32(stringArr[2]);
            AddElement(xDoc, "BatchNumber", batchNumber.ToString(), xmlBatchDetailsElement);
            AddElement(xDoc, "SequenceNumber", sequenceNumber.ToString(), xmlBatchDetailsElement);
          }
          CreateLineItemDetailsAttachments(invoiceOfflineCollectionFilePathforSupportingdocs.RelativePath, memoLevelFolderPath, xDoc, xmlBatchDetailsElement);
        }
        else
        {
          var batchNumberString = memoLevelFolderName.Substring(0, 5);
          var seqNumberString = memoLevelFolderName.Substring(6, 5);

          Logger.InfoFormat("BatchNumber: {0}, SequenceNumber: {1}", batchNumberString, seqNumberString);

          if (processedAttachmentMemos == null || !processedAttachmentMemos.ContainsKey(batchNumberString + "_" + seqNumberString))
          {
            var xmlBatchDetailsElement = AddElement(xDoc, "BatchDetails", string.Empty, xmlInvoiceHeaderElement);

            var batchNumber = Convert.ToInt32(batchNumberString);
            var sequenceNumber = Convert.ToInt32(seqNumberString);
            AddElement(xDoc, "BatchNumber", batchNumber.ToString(), xmlBatchDetailsElement);
            AddElement(xDoc, "SequenceNumber", sequenceNumber.ToString(), xmlBatchDetailsElement);
            CreateLineItemDetailsAttachments(invoiceOfflineCollectionFilePathforSupportingdocs.RelativePath,
                                             memoLevelFolderPath, xDoc, xmlBatchDetailsElement);
          }
        }
      }
    }

    private void AppendMemoAndSuppDocsData(XmlDocument xDoc, bool isReceivable, XmlElement xmlInvoiceHeaderElement, Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName, InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforMemos,BillingCategoryType billingCategoryType)
    {
      InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforSupportingdocs;
      IDictionary<string, bool> processedAttachmentMemos = new Dictionary<string, bool>();
      foreach (var memoFilePath in Directory.GetFiles(invoiceOfflineCollectionFilePathforMemos.PhysicalPath))
      {
        var memoFileName = Path.GetFileName(memoFilePath);
        var xmlBatchDetailsElement = AddElement(xDoc, "BatchDetails", string.Empty, xmlInvoiceHeaderElement);
        var batchNumberString = billingCategoryType == BillingCategoryType.Cgo ? memoFileName.Substring(4, 5) : memoFileName.Substring(6, 5);

        var batchNumber = Convert.ToInt32(batchNumberString);

        var seqNumberString = billingCategoryType == BillingCategoryType.Cgo ? memoFileName.Substring(10, 5) : memoFileName.Substring(12, 5);
        
        var sequenceNumber = Convert.ToInt32(seqNumberString);
        AddElement(xDoc, "BatchNumber", batchNumber.ToString(), xmlBatchDetailsElement);
        AddElement(xDoc, "SequenceNumber", sequenceNumber.ToString(), xmlBatchDetailsElement);
        AddElement(xDoc, "MemoFileName", string.Format("{0}/{1}", invoiceOfflineCollectionFilePathforMemos.RelativePath, memoFileName), xmlBatchDetailsElement);

        if (hDocumentFileName.TryGetValue(SuppdocsFolderNameConstant, out invoiceOfflineCollectionFilePathforSupportingdocs) && !isReceivable)
        {
          foreach (var memoLevelFolderPath in Directory.EnumerateDirectories(invoiceOfflineCollectionFilePathforSupportingdocs.PhysicalPath))
          {
            var memoLevelFolder = Path.GetFileName(memoLevelFolderPath);
            if (memoLevelFolder.Equals(string.Format("{0}-{1}", batchNumber.ToString().PadLeft(5, '0'), sequenceNumber.ToString().PadLeft(5, '0'))))
            {
              // Store batch number and seq numbers for which attachments are processed and ignore them while processing all the attachments.
              processedAttachmentMemos.Add(batchNumberString + "_" + seqNumberString, true);
              if (xmlBatchDetailsElement != null)
              {
                if (xDoc != null)
                {
                  CreateLineItemDetailsAttachments(invoiceOfflineCollectionFilePathforSupportingdocs.RelativePath, memoLevelFolderPath, xDoc, xmlBatchDetailsElement);
                }
                else
                {
                  Logger.InfoFormat("xDoc is null" + invoiceOfflineCollectionFilePathforSupportingdocs.PhysicalPath);
                }
              }
              else
              {
                Logger.InfoFormat("xmlBatchDetailsElement is null" + invoiceOfflineCollectionFilePathforSupportingdocs.PhysicalPath);
              }
              break;
            }
          }
        }
      }

      // Get supporting document path from dictionary.
      if (hDocumentFileName.TryGetValue(SuppdocsFolderNameConstant, out invoiceOfflineCollectionFilePathforSupportingdocs))
        AppendSuppDocsData(xDoc, xmlInvoiceHeaderElement, invoiceOfflineCollectionFilePathforSupportingdocs, false, processedAttachmentMemos);
    }

    private void AppendMuSuppDocsData(XmlDocument xDoc, bool isReceivable, InvoiceBase invoiceBase, XmlElement xmlInvoiceHeaderElement, Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName)
    {
      InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforMuAttachments;
      if (hDocumentFileName.TryGetValue(SuppdocsFolderNameConstant, out invoiceOfflineCollectionFilePathforMuAttachments) && (invoiceBase.BillingCategory == BillingCategoryType.Misc || invoiceBase.BillingCategory == BillingCategoryType.Uatp))
      {
        var suppDocFileNames = Directory.GetFiles(invoiceOfflineCollectionFilePathforMuAttachments.PhysicalPath);
        int srNo = 0;
        foreach (var suppAttachmentFilePath in suppDocFileNames)
        {
          srNo++;
          var suppdocsFileName = Path.GetFileName(suppAttachmentFilePath);
          var xmlInvoiceSupportingAttachmentElement = AddElement(xDoc, "InvoiceSupportingAttachments", string.Empty, xmlInvoiceHeaderElement);
          AddElement(xDoc, "AttachmentNumber", srNo.ToString(), xmlInvoiceSupportingAttachmentElement);
          AddElement(xDoc, "AttachmentFileName", string.Format("{0}/{1}", invoiceOfflineCollectionFilePathforMuAttachments.RelativePath, suppdocsFileName), xmlInvoiceSupportingAttachmentElement);
        }
      }
    }

    private void AppendEInvoiceData(XmlDocument xDoc, XmlElement xmlInvoiceHeaderElement, Dictionary<string, InvoiceOfflineCollectionFilePath> hDocumentFileName)
    {
      InvoiceOfflineCollectionFilePath invoiceOfflineCollectionFilePathforEinvoice;
      if (hDocumentFileName.TryGetValue(EInvoiceFolderNameConstant, out invoiceOfflineCollectionFilePathforEinvoice))
      {
        if (Directory.Exists(invoiceOfflineCollectionFilePathforEinvoice.PhysicalPath))
        {
          var eInvoiceFiles = Directory.GetFiles(invoiceOfflineCollectionFilePathforEinvoice.PhysicalPath);
          int srNo = 0;

          // Delete an empty e-invoice folder.
          if (eInvoiceFiles.Length <= 0)
          {
            try
            {
              Directory.Delete(invoiceOfflineCollectionFilePathforEinvoice.PhysicalPath);
              Logger.Info("Empty E-Invoice folder deleted. Path: " +
                          invoiceOfflineCollectionFilePathforEinvoice.PhysicalPath);
            }
            catch (Exception exception)
            {
              Logger.InfoFormat(
                "Unable to delete empty E-Invoice folder. Path: {0}, Exception:{1} StackTrace:{2}" +
                invoiceOfflineCollectionFilePathforEinvoice.PhysicalPath,
                exception.Message, exception.StackTrace);
            }
            return;
          }

          foreach (var eInvoiceFilePath in eInvoiceFiles)
          {
            srNo++;
            var eInvoiceFileName = Path.GetFileName(eInvoiceFilePath);
            var xmlEInvoicingFileElement = AddElement(xDoc, "EInvoicingFiles", string.Empty, xmlInvoiceHeaderElement);
            AddElement(xDoc, "SrNo", srNo.ToString(), xmlEInvoicingFileElement);
            AddElement(xDoc, "FileName",
                       string.Format("{0}/{1}", invoiceOfflineCollectionFilePathforEinvoice.RelativePath,
                                     eInvoiceFileName), xmlEInvoicingFileElement);
          }
        }
      }
    }

    /// <summary>
    /// This will add the LineItemDetailsAttachments entry in the Index file node
    /// for the perticular batch
    /// </summary>
    /// <param name="memoLevelFolderPath">memo Level Folder Path</param>
    /// <param name="xDoc">xDoc</param>
    /// <param name="xmlBatchDetailsElement">xmlBatchDetailsElement</param>
    /// <param name="suppDocFolderRelativePath">supp Doc Folder Relative Path</param>
    private static void CreateLineItemDetailsAttachments(string suppDocFolderRelativePath, string memoLevelFolderPath, XmlDocument xDoc, XmlElement xmlBatchDetailsElement)
    {
      var memoLevelSrNo = 0;
      var memoLevelFolder = Path.GetFileName(memoLevelFolderPath);
      XmlElement xmlLineItemDetailAttachmentsElement;
      foreach (var memoLevelFileName in Directory.EnumerateFiles(memoLevelFolderPath))
      {
        memoLevelSrNo++;
        var memoLevelFile = Path.GetFileName(memoLevelFileName);
        xmlLineItemDetailAttachmentsElement = AddElement(xDoc, "LineItemDetailsAttachments", string.Empty, xmlBatchDetailsElement);

        AddElement(xDoc, "AttachmentNumber", memoLevelSrNo.ToString(), xmlLineItemDetailAttachmentsElement);
        AddElement(xDoc, "AttachmentFileName", string.Format("{0}/{1}/{2}", suppDocFolderRelativePath, memoLevelFolder, memoLevelFile), xmlLineItemDetailAttachmentsElement);
      }
      var isLineItemDetailsAttachmentsElementAdded = false;
      xmlLineItemDetailAttachmentsElement = null;
      foreach (var breakdownLevelFolderName in Directory.EnumerateDirectories(memoLevelFolderPath))
      {
        var breakdownLevelFolder = Path.GetFileName(breakdownLevelFolderName);

        if (!isLineItemDetailsAttachmentsElementAdded)
        {
          xmlLineItemDetailAttachmentsElement = AddElement(xDoc, "LineItemDetailsAttachments", string.Empty, xmlBatchDetailsElement);
          isLineItemDetailsAttachmentsElementAdded = true;
        }
        Logger.InfoFormat("BreakdownSerialNumber: {0}", breakdownLevelFolder);
        var xmlBreakdownDetailsElement = AddElement(xDoc, "BreakdownDetails", string.Empty, xmlLineItemDetailAttachmentsElement);
        AddElement(xDoc, "BreakdownSerialNumber", Convert.ToInt32(breakdownLevelFolder).ToString(), xmlBreakdownDetailsElement);
        var memoBreakdownAttachmentSrNo = 0;

        foreach (var breakdownLevelFileName in Directory.EnumerateFiles(breakdownLevelFolderName))
        {
          memoBreakdownAttachmentSrNo++;
          var xmlBreakdownDetailsAttachmentsElement = AddElement(xDoc, "BreakdownDetailsAttachments", string.Empty, xmlBreakdownDetailsElement);
          var memoBreakdownLevelFile = Path.GetFileName(breakdownLevelFileName);
          AddElement(xDoc, "AttachmentNumber", memoBreakdownAttachmentSrNo.ToString(), xmlBreakdownDetailsAttachmentsElement);
          AddElement(xDoc, "AttachmentFileName", string.Format("{0}/{1}/{2}/{3}", suppDocFolderRelativePath, memoLevelFolder, breakdownLevelFolder, memoBreakdownLevelFile), xmlBreakdownDetailsAttachmentsElement);
        }
      }
    }

    /// <summary>
    /// This will return the BillingCategoryType cahracter for File Name
    /// </summary>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isFormC"></param>
    /// <returns>Returns the Billing category character</returns>
    private static string GetBillingCategory(BillingCategoryType billingCategoryType, bool isFormC)
    {
      switch (billingCategoryType)
      {
        case BillingCategoryType.Pax:
          if (isFormC) return "F";
          return "P";
        case BillingCategoryType.Cgo:
          return "C";
        case BillingCategoryType.Misc:
          return "M";
        case BillingCategoryType.Uatp:
          return "U";
        default:
          Logger.InfoFormat("Invalid BillingCategoryType {0}", billingCategoryType);
          return string.Empty;
      }
    }

    /// <summary>
    /// This will add the Xml node in the parent node
    /// </summary>
    /// <param name="xDoc">xDoc</param>
    /// <param name="nodeText">nodeText</param>
    /// <param name="nodeValue">nodeValue</param>
    /// <param name="parentElement">parentElement</param>
    /// <returns>xmlElement</returns>
    private static XmlElement AddElement(XmlDocument xDoc, string nodeText, string nodeValue, XmlElement parentElement)
    {
      try
      {
        if (xDoc != null)
        {
          if (nodeText != null)
          {
            XmlElement xmlElement = xDoc.CreateElement("", nodeText, "");
            if (nodeValue != null)
            {
              XmlText xmlText = xDoc.CreateTextNode(nodeValue);
              xmlElement.AppendChild(xmlText);
            }
            else
            {
              Logger.InfoFormat("nodeValue is null " + nodeText + "Node Value " + nodeValue);
              return null;
            }
            if (parentElement != null)
            {
              parentElement.AppendChild(xmlElement);
            }
            else
            {
              Logger.InfoFormat("parent element is null " + nodeText + " Node Value " + nodeValue);
              return null;
            }
            return xmlElement;
          }
          else
          {
            Logger.InfoFormat("nodeText is null " + nodeText + " Node Value " + nodeValue);
            return null;
          }
        }
        else
        {
          Logger.InfoFormat("xDoc is null" + nodeText + " Node Value " + nodeValue);
          return null;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error adding xml element to xml document" + nodeText + "Node Value " + nodeValue, ex);
        throw;
      }
    }

    /// <summary>
    /// Sends the alert.
    /// </summary>
    private void SendAlert()
    {

    }

    /// <summary>
    /// Gets the invoice meta data.
    /// </summary>
    /// <param name="memberNumericCode">member Numeric Code</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="options">options</param>
    private IEnumerable<InvoiceOfflineCollectionMetaData> FetchPerMemberInvoiceMetaData(string memberNumericCode, BillingPeriod billingPeriod, BillingCategoryType billingCategoryType, bool isReceivable, List<string> options)
    {
      List<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection;

      if (isReceivable)
        invoiceOfflineCollectionMetaDataCollection = InvoiceOfflineCollectionMetaDataRepository.Get(i => i.BillingMemberCode == memberNumericCode && i.BillingMonth == billingPeriod.Month
            && i.BillingYear == billingPeriod.Year && i.PeriodNo == billingPeriod.Period && !i.IsFormC && i.BillingCategoryId == (int)billingCategoryType).ToList();
      else
        invoiceOfflineCollectionMetaDataCollection = InvoiceOfflineCollectionMetaDataRepository.Get(i => i.BilledMemberCode == memberNumericCode && i.BillingMonth == billingPeriod.Month
            && i.BillingYear == billingPeriod.Year && i.PeriodNo == billingPeriod.Period && !i.IsFormC && i.BillingCategoryId == (int)billingCategoryType).ToList();


      /*
       * Unnecessary code was written During CMP. It should be removed 
       * invoiceOfflineCollectionMetaDataCollection =  InvoiceOfflineCollectionMetaDataRepository.Get(i => i.BilledMemberCode == memberNumericCode && i.BillingMonth == billingPeriod.Month
           && i.BillingYear == billingPeriod.Year && i.PeriodNo == billingPeriod.Period && !i.IsFormC && i.BillingCategoryId == (int)billingCategoryType).ToList();*/


      return GetFilteredInvoiceOfflineMetadata(invoiceOfflineCollectionMetaDataCollection, options);
    }

    /// <summary>
    /// Gets the invoice meta data.
    /// </summary>
    /// <param name="memberNumericCode">member Numeric Code</param>
    /// <param name="billingPeriod">billingPeriod</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="isReceivable">isReceivable</param>
    /// <param name="options">options</param>
    private IEnumerable<InvoiceOfflineCollectionMetaData> FetchPerMemberFormCMetaData(string memberNumericCode, BillingPeriod billingPeriod, BillingCategoryType billingCategoryType, bool isReceivable, List<string> options)
    {
      List<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection;

      if (isReceivable)
        invoiceOfflineCollectionMetaDataCollection = InvoiceOfflineCollectionMetaDataRepository.Get(i => i.BillingMemberCode == memberNumericCode && i.BillingMonth == billingPeriod.Month
            && i.BillingYear == billingPeriod.Year && i.PeriodNo == billingPeriod.Period && i.IsFormC && i.BillingCategoryId == (int)billingCategoryType).ToList();
      else
        invoiceOfflineCollectionMetaDataCollection = InvoiceOfflineCollectionMetaDataRepository.Get(i => i.BilledMemberCode == memberNumericCode && i.BillingMonth == billingPeriod.Month
            && i.BillingYear == billingPeriod.Year && i.PeriodNo == billingPeriod.Period && i.IsFormC && i.BillingCategoryId == (int)billingCategoryType).ToList();

      return GetFilteredInvoiceOfflineMetadata(invoiceOfflineCollectionMetaDataCollection, options);
    }

    /// <summary>
    /// Gets the InvoiceOfflineMetadata selected by the user
    /// </summary>
    /// <param name="invoiceOfflineCollectionMetaDataCollection"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    private static List<InvoiceOfflineCollectionMetaData> GetFilteredInvoiceOfflineMetadata(List<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDataCollection, List<string> options)
    {
      var filteredinvoiceOfflineCollectionMetaDataCollection = new List<InvoiceOfflineCollectionMetaData>();

      foreach (var invoiceOfflineCollectionMetaData in invoiceOfflineCollectionMetaDataCollection)
      {
        if (options.Contains(invoiceOfflineCollectionMetaData.OfflineCollectionFolderTypeId.ToString()))
        {
          filteredinvoiceOfflineCollectionMetaDataCollection.Add(invoiceOfflineCollectionMetaData);
        }
      }
      return filteredinvoiceOfflineCollectionMetaDataCollection;
    }

    /// <summary>
    /// Sends an email alert to all contacts of the member
    /// </summary>
    /// <param name="memberId"></param>
    /// <param name="billingPeriod"></param>
    public bool SendOutputAvailableAlert(int memberId, BillingPeriod billingPeriod)
    {
      var currentBillingPeriod = string.Format("{0:D4}{1:D2}{2:D2}", billingPeriod.Year, billingPeriod.Month, billingPeriod.Period);
      var currentBillingPeriodText = string.Format("{0:D2}/{1:D4}/{2:D2}", billingPeriod.Month, billingPeriod.Year, billingPeriod.Period);
      var outputAvailableAlertContactTypeList = new List<int> {
                                                                (int) ProcessingContactType.PAXOutputAvailableAlert,
                                                                (int) ProcessingContactType.CGOOutputAvailableAlert,
                                                                (int) ProcessingContactType.MISCOutputAvailableAlert,
                                                                (int) ProcessingContactType.UATPOutputAvailableAlert
                                                              };
      var contactList = MemberManager.GetContactsForContactTypes(outputAvailableAlertContactTypeList);
      var contactsForThisMember = contactList.Where(c => c.MemberId == memberId).Select(c => c.EmailAddress).ToArray();
      if (contactsForThisMember.Length > 0)
      {
        BroadcastMessagesManager.SendOutputAvailableAlert(memberId, contactsForThisMember, currentBillingPeriod, currentBillingPeriodText, EmailTemplateId.OfflineOutputAvailableAlert);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Sends the failure alert.
    /// </summary>
    public void SendFailureAlert(string memberCode, BillingPeriod billingPeriod, BillingCategoryType billingCategoryType, string errorMessage, bool isFormC)
    {
      var currentBillingPeriodText = string.Format("{0} {1} P{2}", CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(billingPeriod.Month), billingPeriod.Year, billingPeriod.Period);
      // Create an object of the nVelocity data dictionary
      var billingCategoryString = isFormC ? "FORMC" : billingCategoryType.ToString();
      var context = new VelocityContext();
      context.Put("MemberCode", memberCode);
      context.Put("Period", currentBillingPeriodText);
      context.Put("Category", billingCategoryString);
      context.Put("ErrorMessage", errorMessage);
      const string message = "Offline collection download failure for member {0} in period {1} of category {2}";
      const string title = "Offline collection download failure alert";

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, memberCode, currentBillingPeriodText, billingCategoryString),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
        Title = title
      };
      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminOfflineCollectionDownloadFailureAlert, context);
      Logger.Info("Sent IS-Admin alert");
    }

    /// <summary>
    /// CMP529
    /// Creates the member Daily invoices offline collection zip file.
    /// and returns the output file name on the FTP server
    /// </summary>
    /// <param name="invoiceBaseList">invoiceBaseList</param>
    /// <param name="member">member</param>
    /// <param name="billingCategoryType">billingCategoryType</param>
    /// <param name="targetDate">If Daily OAR required then target date is required.</param>
    /// <param name="isReprocessing">flag for reprocessing.</param>
    /// <param name="isFileExist">check if file is already exist</param>
    /// <param name="locationId">CMP622: true if location specific file is required.</param>
    /// SCP279970: OAR Optimization

    // SCP#369538 - SRM: Daily ouputs are slow - Delivered on 16-May-2015.
    // Removed invOfflineColData cursor fetching. Instead path is build in C# code itself.
    public string GetMemberDailyInvoicesOfflineCollectionZip(IQueryable<InvoiceBase> invoiceBaseList, Member member, BillingCategoryType billingCategoryType, DateTime targetDate,bool isReprocessing, out bool isFileExist, string locationId = null)
    {
      var filename = GetParentFolderName(member, billingCategoryType, new BillingPeriod(), false, false, 0,
                                           0, true, targetDate: targetDate, isDailyOARRequired: true, locationId: locationId);

      var existingOutputFileName = string.Format("{0}.ZIP", filename);

      if (IsFileExist(existingOutputFileName) && !isReprocessing)
      {
        isFileExist = true;
        return existingOutputFileName;
      }

      Logger.Info("GetMemberDailyInvoicesOfflineCollectionZip Method");
      var ftpfilePath = string.Empty;

      var invoiceBases = new List<InvoiceBase>();

      //_hPerInvoiceDirectory will store the per invoice or per form c entry
      _hPerInvoiceDirectory = new Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>>();

      Logger.Info("Created new PerInvoiceDirectory.");
      Dictionary<BillingPeriod, string> adminSANPathCache = new Dictionary<BillingPeriod, string>();

      /* Looping On Invoices */
      foreach (var invoiceBase in invoiceBaseList)
      {
          Logger.InfoFormat("Building Offline Collection Metadata Path For Invoice {0} in Period {1} ", invoiceBase.InvoiceNumber, invoiceBase.InvoiceBillingPeriod);

          var filteredOfflineCollMetadata = BuildOfflineCollectionMetadataPath(invoiceBase, adminSANPathCache);

          invoiceBases.Add(invoiceBase);
          
          GetInvoiceOfflineCollection(filteredOfflineCollMetadata, member, billingCategoryType,
                                      new BillingPeriod(),
                                      false, false, invoiceBase,
                                      targetDate: targetDate, isDailyOARRequired: true, locationId: locationId);
      }

      if (invoiceBases.Count > 0)
      {
        Logger.Info("Creating Index file.");

        CreateIndexFile(invoiceBases, new List<SamplingFormC>(), new BillingPeriod(), billingCategoryType, member.Id, false, false, targetDate, true, locationId);

        if (!XmlValidator.ValidateXml(_indexXmlPath))
          Logger.InfoFormat("Error occured while validating Index file {0}.", _indexXmlPath);

        var parentFolderName = GetParentFolderName(member, billingCategoryType, new BillingPeriod(), false, false, 0,
                                                   0, true, targetDate: targetDate, isDailyOARRequired: true, locationId: locationId);

        var outputFileName = string.Format("{0}.ZIP", parentFolderName);
        FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);
        //Move zip to ftp path
        Logger.Info("Moving file to FTP location.");
        ftpfilePath = Path.Combine(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric),
                                       Path.GetFileName(outputFileName));
        
        FileIo.MoveFile(outputFileName, ftpfilePath);

        Logger.InfoFormat("Zip copied from temp location {0} to ftp location {1}", outputFileName, ftpfilePath);
        
        Directory.Delete(parentFolderName);
      }
      isFileExist = false;
      return ftpfilePath;
    }

    /// <summary>
    /// This function is used to delete folder which is not used after generate OAR for formc.
    /// </summary>
    /// <param name="directoryInfo">directory info object for delete folder.</param>
    /// <param name="rootPath">root path of OAR.</param>
    /// <returns>return true.</returns>
    private static bool DeleteFolderRecursively(DirectoryInfo directoryInfo,String rootPath)
    {
        if (!NormalizePath(directoryInfo.FullName).Equals(NormalizePath(rootPath)) &&
            directoryInfo.GetDirectories().Count() == 0 && directoryInfo.GetFiles().Count() == 0)
        {
            Logger.InfoFormat("Deleting directory: [{0}]", directoryInfo.FullName);
            Directory.Delete(directoryInfo.FullName, true);
            return DeleteFolderRecursively(directoryInfo.Parent, rootPath);
        }
        return true;
    }

    /// <summary>
    /// This function is used to delete folder for formc after generate OAR
    /// </summary>
    /// <param name="path">listing path</param>
    /// <param name="offlinerootFolderPath">offline root folder path</param>
    private static void DeleteFolderFormc(string path,String offlinerootFolderPath)
    {
        try
        {
            //Check following path exist or not.
            if (Directory.Exists(path))
            {
                var directoryInfo = new DirectoryInfo(path);
                //Check file and folder exist of not.
                if (directoryInfo.GetFiles().Count() > 0 || directoryInfo.GetDirectories().Count() > 0)
                {
                    Logger.InfoFormat("Deleting directory: [{0}]", directoryInfo.FullName);
                    Directory.Delete(directoryInfo.FullName, true);
                    DeleteFolderRecursively(directoryInfo.Parent, offlinerootFolderPath);
                }
                else
                {
                    DeleteFolderRecursively(directoryInfo, offlinerootFolderPath);
                }
            }
        }
        catch (Exception exception)
        {
            Logger.ErrorFormat("Exception occured while delete unused folder after generating OAR for formc, Exception:{0} StackTrace:{1}",
                exception.Message, exception.StackTrace);
        }
    }

    /// <summary>
    /// This function is used to return path.
    /// </summary>
    /// <param name="path">path</param>
    /// <returns></returns>
    public static string NormalizePath(string path)
    {
        //Return normize path.
        return Path.GetFullPath(new Uri(path).LocalPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToUpperInvariant();
    }
    
    //CMP#622: MISC Outputs Split as per Location IDs
    #region CMP622 : MISC LOCATION OAR

    /// <summary>
    /// Gets the member location invoices offline collection zip.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="billingPeriod">The billing period.</param>
    /// <param name="isReceivable">if set to <c>true</c> [is receivable].</param>
    /// <param name="options">The options.</param>
    /// <param name="billingCategoryType">Type of the billing category.</param>
    /// <param name="memberLocationCode">The member location code.</param>
    /// <param name="isNilfileRequired">if set to <c>true</c> [is nilfile required].</param>
    /// <param name="checkOARGenerated">if set to <c>true</c> [check OAR generated].</param>
    /// <param name="isRegeneration">if set to <c>true</c> [is regeneration].</param>
    /// <returns></returns>
    public List<string> GetMemberLocationInvoicesOfflineCollectionZip(Member member, BillingPeriod billingPeriod, bool isReceivable, List<string> options, BillingCategoryType billingCategoryType, string memberLocationCode, bool isNilfileRequired, bool checkOARGenerated, bool isRegeneration = false)
    {
        var outputFileNames = new List<string>();

        if (checkOARGenerated)
        {
            var fileName = GetParentFolderName(member, BillingCategoryType.Misc, billingPeriod, isReceivable, false, 0, 0, true, locationId: memberLocationCode);
            var nilFileName = GetParentFolderName(member, BillingCategoryType.Misc, billingPeriod, isReceivable, false, 0, 0, true, locationId: memberLocationCode, isLocationSpecNilFile: true);
            if (IsFileExist(fileName))
            {
                Logger.InfoFormat("Skipping {0} file generataion as file is already generated for member {1} for location [{2}].", fileName, member.Id,memberLocationCode);
                return outputFileNames;
            }
            else if (IsFileExist(nilFileName))
            {
                Logger.InfoFormat("Skipping {0} file generataion as file is already generated for member {1} for location [{2}].", nilFileName, member.Id,memberLocationCode);
                return outputFileNames;
            }
        }

        Logger.InfoFormat("Fetching Invoice Offline MetaData Collection of Invoices for member id - {0} ", member.Id);
        var invOfflineMetaDataCollection = FetchPerMemberInvoiceMetaData(member.MemberCodeNumeric, billingPeriod, BillingCategoryType.Misc, isReceivable, options);
        Logger.InfoFormat("{0} Offline MetaData Collection records found for member id - {1} ", invOfflineMetaDataCollection.Count(), member.Id);
        var invoiceBases = new List<InvoiceBase>();
        //_hPerInvoiceDirectory will store the per invoice or per form c entry
        _hPerInvoiceDirectory = new Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>>();
        Logger.Info("Created new PerInvoiceDirectory.");
        foreach (IGrouping<string, InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaDatas in invOfflineMetaDataCollection.GroupBy(cdr => cdr.InvoiceNumber))
        {
            var invoiceOfflineCollectionMetaDataList = invoiceOfflineCollectionMetaDatas.ToList();
            var invoiceNumber = invoiceOfflineCollectionMetaDataList[0].InvoiceNumber;

            //SCP279970: OAR Optimization.
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var invoiceBaseList = new InvoiceBaseManager().GetInvoiceBaseDetails(member.Id,
                                                                                 billingPeriod.Year,
                                                                                 billingPeriod.Month,
                                                                                 billingPeriod.Period,
                                                                                 invoiceNumber,
                                                                                 (int) BillingCategoryType.Misc,
                                                                                 isReceivable,
                                                                                 true,
                                                                                 memberLocationCode);

            stopwatch.Stop();
            Logger.Info("GET MEBER LOCATION BASE INVOICE DETAILS : TIME[" + stopwatch.Elapsed + "]");

            if (invoiceBaseList != null)
            {
                foreach (var invoiceBase in invoiceBaseList)
                {
                    var otherMemberId = isReceivable ? invoiceBase.BilledMemberId : invoiceBase.BillingMemberId;
                    var otherMemberCode = _memberManager.GetMemberCode(otherMemberId); //billing

                    var filteredOfflineCollMetadata = isReceivable
                                                          ? invoiceOfflineCollectionMetaDataList.FindAll(i => i.BilledMemberCode == otherMemberCode)
                                                          : invoiceOfflineCollectionMetaDataList.FindAll(i => i.BillingMemberCode == otherMemberCode);

                    if (!isRegeneration)
                    {
                        //Invoice OR CreditNote status = Processing Complete or Presented
                        if (invoiceBase != null && (invoiceBase.InvoiceStatus == InvoiceStatusType.ProcessingComplete || invoiceBase.InvoiceStatus == InvoiceStatusType.Presented))
                        {
                            Logger.InfoFormat("Generating Invoice Offline Collection for invoice number {0}", invoiceBase.InvoiceNumber);
                            invoiceBases.Add(invoiceBase);
                            GetInvoiceOfflineCollection(filteredOfflineCollMetadata, member, BillingCategoryType.Misc, billingPeriod, isReceivable, false, invoiceBase);
                        }
                        else
                        {
                            Logger.InfoFormat("Invoice number {0} not found for Period {1}, Month {2},Year {3},{4} {5},Billing Category {6} in Processing Complete or Presented status",
                                              invoiceNumber,
                                              billingPeriod.Period,
                                              billingPeriod.Month,
                                              billingPeriod.Year,
                                              isReceivable ? "BillingMemberId" : "BilledMemberId",
                                              member.Id,
                                              BillingCategoryType.Misc);
                        }
                    }
                    else
                    {
                        //Invoice OR CreditNote status =  Presented
                        if (invoiceBase != null && (invoiceBase.InvoiceStatus == InvoiceStatusType.Presented))
                        {
                            Logger.InfoFormat("Generating Invoice Offline Collection for invoice number {0}", invoiceBase.InvoiceNumber);
                            invoiceBases.Add(invoiceBase);
                            GetInvoiceOfflineCollection(filteredOfflineCollMetadata, member, BillingCategoryType.Misc, billingPeriod, isReceivable, false, invoiceBase);
                        }
                        else
                        {
                            Logger.InfoFormat("Invoice number {0} not found for Period {1}, Month {2},Year {3},{4} {5},Billing Category {6} in Presented status",
                                              invoiceNumber,
                                              billingPeriod.Period,
                                              billingPeriod.Month,
                                              billingPeriod.Year,
                                              isReceivable ? "BillingMemberId" : "BilledMemberId",
                                              member.Id,
                                              BillingCategoryType.Misc);
                        }
                    }
                }
            }
        }
        if (invoiceBases.Count > 0)
        {
            Logger.Info("Creating Index file.");
            Logger.InfoFormat("BillingPeriod: {0}, BillingCategoryType: {1}, MemberId: {2}, IsReceivable: {3}", billingPeriod, billingCategoryType, member.Id, isReceivable);
            CreateIndexFile(invoiceBases, new List<SamplingFormC>(), billingPeriod, billingCategoryType, member.Id, isReceivable, false);

            if (!XmlValidator.ValidateXml(_indexXmlPath)) Logger.InfoFormat("Error occured while validating Index file {0}.", _indexXmlPath);

            var parentFolderName = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, false, 0, 0, true,locationId:memberLocationCode);
            

            var outputFileName = string.Format("{0}.ZIP", parentFolderName);
            FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);
            //Move zip to ftp path
            Logger.Info("Moving file to FTP location.");
            var ftpfilePath = Path.Combine(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric), Path.GetFileName(outputFileName));
            FileIo.MoveFile(outputFileName, ftpfilePath);
            outputFileNames.Add(ftpfilePath);
            Directory.Delete(parentFolderName);
        }
        else
        {
            //CMP#622: MISC Outputs Split as per Location IDs
            if (isNilfileRequired)
            {
                Logger.InfoFormat("Generating Nil file as 0 Misc invoices found of location [{1}] for Billed Member Id: {0}.",member.Id,memberLocationCode);

                var folderName = GetParentFolderName(member, billingCategoryType, billingPeriod, isReceivable, false, 0, 0,true,locationId:memberLocationCode,isLocationSpecNilFile:true);

                var filename = string.Format("{0}.ZIP", Path.GetFileName(folderName));

                if (!IsFileExist(filename))
                {
                    var nilFilePath = GenerateNilOarFile(member, folderName);

                    Logger.InfoFormat("Nil file generated as 0 Misc invoices found for Billed Member Id: {0}.",
                                      member.Id);

                    Logger.InfoFormat("Adding File: [{0}] to File Log table.", nilFilePath);

                    //Add entry in Is File log table
                    outputFileNames.Add(nilFilePath);

                    Logger.InfoFormat("Successfully Added File: [{0}] to File Log table.", nilFilePath);
                }
                else
                {
                    Logger.InfoFormat("Skipping {0} Nil file generataion as file is already generated for member {1}.", filename,
                                      member.Id);
                }
            }
        }


        return outputFileNames;
    }

    /// <summary>
    /// Gets the location invoice offline collection zip.
    /// </summary>
    /// <param name="userId">The user id.</param>
    /// <param name="zipFileName">Name of the zip file.</param>
    /// <param name="id">The id.</param>
    /// <param name="options">The options.</param>
    /// <param name="downloadUrl">The download URL.</param>
    /// <param name="isReceivable">if set to <c>true</c> [is receivable].</param>
    /// <param name="memberLocationCode">The member location code.</param>
    /// <param name="isNilfileRequired">if set to <c>true</c> [is nilfile required].</param>
    /// <returns></returns>
    public string GetLocationInvoiceOfflineCollectionZip(int userId, string zipFileName, Guid id, List<string> options, string downloadUrl, bool isReceivable, string memberLocationCode, bool isNilfileRequired)
    {

        // Create an object of the nVelocity data dictionary
        var context = new VelocityContext();
        context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
        var user = AuthManager.GetUserByUserID(userId);
        var emailAddress = user == null ? string.Empty : user.Email;
        Logger.InfoFormat("User is {0} for UserId {1} Email Id {2}.", user == null ? "NULL" : "NOT NULL", userId, emailAddress);
        _hPerInvoiceDirectory = new Dictionary<string, Dictionary<string, InvoiceOfflineCollectionFilePath>>();

        var invoiceBase = InvoiceBaseRepository.Single(i => i.Id == id);
        if (invoiceBase != null)
        {
            var member = _memberManager.GetMember(isReceivable ? invoiceBase.BillingMemberId : invoiceBase.BilledMemberId);

            var billingMemberCode = _memberManager.GetMemberCode(invoiceBase.BillingMemberId);
            var billedMemberCode = _memberManager.GetMemberCode(invoiceBase.BilledMemberId);
            var billingPeriod = new BillingPeriod { Month = invoiceBase.BillingMonth, Period = invoiceBase.BillingPeriod, Year = invoiceBase.BillingYear };

            var invOfflineMetaDataCollection =
                InvoiceOfflineCollectionMetaDataRepository.Get(
                    i =>
                    i.BilledMemberCode == billedMemberCode && i.BillingMemberCode == billingMemberCode && i.BillingCategoryId == invoiceBase.BillingCategoryId &&
                    i.BillingMonth == invoiceBase.BillingMonth && i.BillingYear == invoiceBase.BillingYear && i.InvoiceNumber.ToUpper() == invoiceBase.InvoiceNumber.ToUpper() &&
                    i.PeriodNo == invoiceBase.BillingPeriod).ToList();

            invOfflineMetaDataCollection = GetFilteredInvoiceOfflineMetadata(invOfflineMetaDataCollection, options);

            if (invOfflineMetaDataCollection.ToList().Find(i => i.OfflineCollectionFolderTypeId == 1) != null &&
                (invoiceBase.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoiceBase.InvoiceStatus == InvoiceStatusType.Claimed))
            {
                //Generate EInvoices
                var eInvoiceDocumentGenerator = Ioc.Resolve<IEinvoiceDocumentGenerator>();
                var invOfflineMetaDataObj = invOfflineMetaDataCollection.Find(i => i.OfflineCollectionFolderTypeId == 1);
                if (invOfflineMetaDataObj != null)
                {
                    Logger.InfoFormat("Generating EInvoices on path {0}", invOfflineMetaDataObj.FilePath);
                    eInvoiceDocumentGenerator.CreateEinvoiceDocuments(invoiceBase, invOfflineMetaDataObj.FilePath);
                }
            }

            //Fixed Simultaneous 2 zip download related issue
            GetInvoiceOfflineCollection(invOfflineMetaDataCollection, member, invoiceBase.BillingCategory, billingPeriod, isReceivable, false, invoiceBase, isWebZip: true, zipFileName: zipFileName,locationId:memberLocationCode);

            
            var invoiceBases = new List<InvoiceBase> { invoiceBase };

            Logger.InfoFormat("BillingPeriod: {0}, BillingCategoryType: {1}, MemberId: {2}, IsReceivable: {3}", billingPeriod, invoiceBase.BillingCategory, member.Id, isReceivable);

            CreateIndexFile(invoiceBases, new List<SamplingFormC>(), billingPeriod, invoiceBase.BillingCategory, member.Id, isReceivable, false);

            if (!XmlValidator.ValidateXml(_indexXmlPath)) Logger.InfoFormat("Error occured while validating Index file {0}.", _indexXmlPath);

            var outputFileName = string.Format("{0}\\{1}.ZIP", Path.GetDirectoryName(string.Format("{0}.ZIP", _baseFolderPath)), zipFileName);

            // Create zip file.
            FileIo.ZipOutputFolder(_baseFolderPath, outputFileName);

            // Copy to ftp path
            var ftpfilePath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollWebRoot), Path.GetFileName(outputFileName));
                //(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric), Path.GetFileName(outputFileName));
            Logger.Info(string.Format("Copying file from {0} to {1}.", outputFileName, ftpfilePath));
            File.Copy(outputFileName, ftpfilePath, true);

            // Add Http Download Link
            var isHttpDownloadLink = new IsHttpDownloadLink { FilePath = ftpfilePath, LastUpdatedBy = member.Id, LastUpdatedOn = DateTime.UtcNow };
            IsHttpDownloadLinkRepository.Add(isHttpDownloadLink);
            UnitOfWork.CommitDefault();

            //Build http url for download.
            var httpUrl = string.Format("{0}/{1}", downloadUrl, isHttpDownloadLink.Id);
            Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

            context.Put("InvoiceNumber", invoiceBase.InvoiceNumber);
            BroadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl, EmailTemplateId.OutputFileAvailableAlert, context);
            Logger.Info("Send an email to " + emailAddress);

            return httpUrl;
        }
        else
        {
            Logger.InfoFormat("Invoice Id {0} not found", id);
        }


        return string.Empty;
    }

    /// <summary>
    /// Generates the nil oar file.
    /// </summary>
    /// <param name="member">The member.</param>
    /// <param name="outputFileName">Name of the output file.</param>
    /// <returns></returns>
    private static string GenerateNilOarFile(Member member, string outputFileName)
    {

        var ftpfilePath = string.Empty;

        try
        {
            Logger.Info("Under GenerateNilOutputFile Method");

            // Create Nil File Path.
            string nilFilePath = Path.Combine(outputFileName, "NODATA.TXT");


            // Create Nil File.
            File.WriteAllText(nilFilePath, string.Empty);

            // Create Zipped Nil File.
            var noDataZipFilePath = FileIo.ZipOutputFile(nilFilePath);

            // Create Nil File Path for Member( i.e. member's download folder).
            //string ftpfilePath = Path.Combine(FileIo.GetFtpDownloadFolderPath(memberCodeNumeric, FileIo.GetFtpRootBasePath()),
            //                             outputFileName);
            ftpfilePath = Path.Combine(FileIo.GetFtpDownloadFolderPath(member.MemberCodeNumeric, FileIo.GetFtpRootBasePath()),
                           string.Format("{0}.ZIP", Path.GetFileName(outputFileName)));


            // Delete file if it already exists.
            File.Delete(ftpfilePath);

            // Remane and Move File to Member's Download folder.
            FileIo.MoveFile(noDataZipFilePath, ftpfilePath);

            Logger.InfoFormat("Nil file zip copied from temp location {0} to ftp location {1}", noDataZipFilePath, ftpfilePath);

        }// End try
        catch (Exception ex)
        {
            Logger.InfoFormat("Error occur On Nil file Generation: Exception: {0}", ex.StackTrace);

        }// End catch

        return ftpfilePath;

    }

    #endregion

      /// <summary> 
      /// </summary>
      /// <param name="invoiceBase">Invoice Base Info record for which Offline Collection Path is to be Generated</param>
      /// <param name="adminSANPathCache"></param>
      /// <returns></returns>

      // SCP#369538 - SRM: Daily ouputs are slow - Delivered on 16-May-2015.
      // Removed invOfflineColData cursor fetching. Instead path is build in C# code itself.
      private IEnumerable<InvoiceOfflineCollectionMetaData> BuildOfflineCollectionMetadataPath(InvoiceBase invoiceBase, Dictionary<BillingPeriod, string> adminSANPathCache)
      {
          var invoiceofflineCollectionBaseFolderPath = string.Empty;
          if (adminSANPathCache.ContainsKey(invoiceBase.InvoiceBillingPeriod) && !string.IsNullOrWhiteSpace(adminSANPathCache[invoiceBase.InvoiceBillingPeriod]))
          {
              /* Here Root Folder Path is Obtained From Cache */
              invoiceofflineCollectionBaseFolderPath = adminSANPathCache[invoiceBase.InvoiceBillingPeriod].ToUpper();
          }
          else
          {
              invoiceofflineCollectionBaseFolderPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollDownloadSFR, invoiceBase.InvoiceBillingPeriod);
              if (!adminSANPathCache.ContainsKey(invoiceBase.InvoiceBillingPeriod) && !string.IsNullOrWhiteSpace(invoiceofflineCollectionBaseFolderPath))
              {
                  adminSANPathCache.Add(invoiceBase.InvoiceBillingPeriod, invoiceofflineCollectionBaseFolderPath.ToUpper());
              }
          }

          // e.g. - > invoiceofflineCollectionBaseFolderPath = \\10.1.2.122\san\SIS_QA17\SFRRoot\Processed\Report
          Logger.InfoFormat("Invoice offline Collection Base Folder SAN Path is [{0}]", invoiceofflineCollectionBaseFolderPath);

          // Create a further folder structure for MiscInvoice Listing report 
          string invoiceofflineCollectionFolderPath = InvoiceOfflineCollectionManager.CreateBaseFolderStructure(invoiceofflineCollectionBaseFolderPath,
                                                                   invoiceBase.BillingMember.MemberCodeNumeric,
                                                                   invoiceBase.BilledMember.MemberCodeNumeric,
                                                                   invoiceBase.BillingCategory,
                                                                   invoiceBase.InvoiceBillingPeriod,
                                                                   invoiceBase.InvoiceNumber,
                                                                   Logger,
                                                                   false, getSANBasePathFromDatabase: false);

          /* e.g. -> invoiceofflineCollectionFolderPath = \\10.1.2.122\SAN\SIS_QA17\SFRROOT\PROCESSED\REPORT\20150502\MISC-131-057\INV-DESC001\ */

          Logger.InfoFormat("Invoice offline Collection Base folder structure Path is [{0}].", invoiceofflineCollectionFolderPath);

          /* Now need to append appropriate Folder names to this path */
          List<InvoiceOfflineCollectionMetaData> invoiceOfflineCollectionMetaData = new List<InvoiceOfflineCollectionMetaData>();

          InvoiceOfflineCollectionMetaData einvoice = GetInvoiceOfflineCollectionMetaDataObject(invoiceBase,
                                                                                                invoiceofflineCollectionFolderPath,
                                                                                                1,
                                                                                                EInvoiceFolderNameConstant);
          invoiceOfflineCollectionMetaData.Add(einvoice);
          Logger.InfoFormat("Invoice offline Collection E-Invoice Path is [{0}].", einvoice.FilePath);

          InvoiceOfflineCollectionMetaData listing = GetInvoiceOfflineCollectionMetaDataObject(invoiceBase,
                                                                                                invoiceofflineCollectionFolderPath,
                                                                                                2,
                                                                                                ListingsFolderNameConstant);
          invoiceOfflineCollectionMetaData.Add(listing);
          Logger.InfoFormat("Invoice offline Collection Listing Path is [{0}].", listing.FilePath);

          InvoiceOfflineCollectionMetaData suppdocs = GetInvoiceOfflineCollectionMetaDataObject(invoiceBase,
                                                                                                invoiceofflineCollectionFolderPath,
                                                                                                4,
                                                                                                SuppdocsFolderNameConstant);
          invoiceOfflineCollectionMetaData.Add(suppdocs);
          Logger.InfoFormat("Invoice offline Collection Supp Doc Path is [{0}].", suppdocs.FilePath);

          return invoiceOfflineCollectionMetaData;
      }

      /// <summary>
      /// </summary>
      /// <param name="invoiceBase"></param>
      /// <param name="invoiceofflineCollectionFolderPath"></param>
      /// <param name="folderTypeId"></param>
      /// <param name="folderNameToAppend"></param>
      /// <returns></returns>

      // SCP#369538 - SRM: Daily ouputs are slow - Delivered on 16-May-2015.
      // Removed invOfflineColData cursor fetching. Instead path is build in C# code itself.
      private InvoiceOfflineCollectionMetaData GetInvoiceOfflineCollectionMetaDataObject(InvoiceBase invoiceBase, string invoiceofflineCollectionFolderPath, int folderTypeId, string folderNameToAppend)
      {
          InvoiceOfflineCollectionMetaData invoiceOfflineCollectionMetaData = new InvoiceOfflineCollectionMetaData()
                                                                                  {
                                                                                      PeriodNo =
                                                                                          invoiceBase.BillingPeriod,
                                                                                      BillingYear =
                                                                                          invoiceBase.BillingYear,
                                                                                      BillingMonth =
                                                                                          invoiceBase.BillingMonth,
                                                                                      BillingMemberCode =
                                                                                          invoiceBase.BillingMember.
                                                                                          MemberCodeNumeric,
                                                                                      BilledMemberCode =
                                                                                          invoiceBase.BilledMember.
                                                                                          MemberCodeNumeric,
                                                                                      BillingCategoryId =
                                                                                          invoiceBase.BillingCategoryId,
                                                                                      InvoiceNumber =
                                                                                          invoiceBase.InvoiceNumber,
                                                                                      OfflineCollectionFolderTypeId =
                                                                                          folderTypeId,
                                                                                      FilePath =
                                                                                          Path.Combine(
                                                                                              invoiceofflineCollectionFolderPath
                                                                                                  .ToUpper(),
                                                                                              folderNameToAppend),
                                                                                      IsFormC = false,
                                                                                      ProvisionalBillingMonth = 0,
                                                                                      ProvisionalBillingYear = 0
                                                                                  };

          return invoiceOfflineCollectionMetaData;
      }
  
      /// <summary>
      /// This function is used to get invoice listing PDF folder path from invoice offline collection table.
      /// CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
      /// </summary>
      /// <param name="invoiceId"></param>
      /// <returns></returns>
      public string GetInvoiceListingPdfPath(Guid invoiceId)
      {
          //Get invoice object.
          var invoice = InvoiceBaseRepository.Single(i => i.Id == invoiceId);
          
          //Get billing and billed member code.
          var billingMemberCode = _memberManager.GetMemberCode(invoice.BillingMemberId);
          var billedMemberCode = _memberManager.GetMemberCode(invoice.BilledMemberId);

          //Get listing path based on criteria.
          var listingPath = InvoiceOfflineCollectionMetaDataRepository.Get(
                              i =>
                              i.BilledMemberCode == billedMemberCode && i.BillingMemberCode == billingMemberCode &&
                              i.BillingCategoryId == invoice.BillingCategoryId && i.BillingMonth == invoice.BillingMonth &&
                              i.BillingYear == invoice.BillingYear &&
                              i.InvoiceNumber.ToUpper() == invoice.InvoiceNumber.ToUpper() && i.PeriodNo == invoice.BillingPeriod &&
                              i.OfflineCollectionFolderTypeId == 2 /*Listing Only*/).Select(i => i.FilePath).FirstOrDefault();

          return listingPath;
      }
  }
}
