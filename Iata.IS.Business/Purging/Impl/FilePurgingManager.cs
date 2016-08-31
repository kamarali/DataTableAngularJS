using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Smtp;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Data.LegalArchive;
using Iata.IS.Data.LegalArchive.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Data.Purging;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.LegalArchive;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.SupportingDocuments;
using log4net;
using Iata.IS.Business.Reports.OfflineReportManger;
using Iata.IS.Business.Common;

namespace Iata.IS.Business.Purging.Impl
{
    public class FilePurgingManager : IFilePurgingManager
    {
        private const string LogsFolderName = "Logs";

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IRepository<ServerServiceMapping> _serverServiceMappingRepository;
        private IRepository<IsInputFile> _isFileRepository;
        private IRepository<UnlinkedSupportingDocument> _unlinkedSupportingDocumentRepository;
        private IRepository<CorrReportFile> _corrReportFileRepository;
        private IInvoiceRepository _invoiceRepository;
        private IRepository<IsFilePurgeQueue> _isFilePurgeQueueRepository;
        public IReferenceManager ReferenceManager { private get; set; }
        private const int InvalidPurgePeriod = 0;
        private DateTime InvalidDate = new DateTime();
        private IOfflineReportLogManager _offlineReportLogManager;
        //SCP317057: ISWEB requested OAR files are not getting purged
        private IRepository<IsHttpDownloadLink> _isHttpDownloadLinkRepository;

        /// <summary>
        /// This method searches for files to purge and returns List of meesages to queue 
        /// This method will be called from file purging job
        /// </summary>
        /// <returns></returns>
        public void SearchPurgingFiles(string arg)
        {
            try
            {
                _serverServiceMappingRepository = Ioc.Resolve<IRepository<ServerServiceMapping>>();
                _isFileRepository = Ioc.Resolve<IRepository<IsInputFile>>();
                _unlinkedSupportingDocumentRepository = Ioc.Resolve<IRepository<UnlinkedSupportingDocument>>();
                _corrReportFileRepository = Ioc.Resolve<IRepository<CorrReportFile>>();
                _isFilePurgeQueueRepository = Ioc.Resolve<IRepository<IsFilePurgeQueue>>();
                _offlineReportLogManager = Ioc.Resolve<IOfflineReportLogManager>();
                //SCP317057: ISWEB requested OAR files are not getting purged
                _isHttpDownloadLinkRepository = Ioc.Resolve<IRepository<IsHttpDownloadLink>>();

                var currentPurgingDateTime = DateTime.UtcNow;

                int inputDataFilesPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.InputDataFilesPurgePeriod;
                var inputDataFilesPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(inputDataFilesPurgePeriod);


                int outputDataFilesPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.OutputDataFilesPurgePeriod;
                var outputDataFilesPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(outputDataFilesPurgePeriod);

                //CMP529 : Daily Output Generation for MISC Bilateral Invoices
                int dailyOutputDataFilesPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.DailyMiscBilateralFilesPurgingPeriod;
                var dailyOutputFilesPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(dailyOutputDataFilesPurgePeriod);

                int unlinkedSuppDocPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.UnlinkedSupportDocFilesPurgePeriod;
                var unlinkedSuppDocPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(unlinkedSuppDocPurgePeriod);


                int corrReportFilesPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.CorrReportFilesPurgingPeriod;
                var corrReportFilesPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(corrReportFilesPurgePeriod);

                int logFilesPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.LogFilesPurgePeriod;
                var logFilesPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(logFilesPurgePeriod);


                int tempFilesPurgePeriod = SystemParameters.Instance.PurgingPeriodDetails.TemporaryFilesPurgePeriod;
                var tempFilesPurgingSpan = currentPurgingDateTime - TimeSpan.FromDays(tempFilesPurgePeriod);

                Logger.InfoFormat("Input Files Purge Period [{0}]", inputDataFilesPurgePeriod);
                Logger.InfoFormat("Output Files Purge Period [{0}]", outputDataFilesPurgePeriod);
                Logger.InfoFormat("Unlinked Supp Doc Files Purge Period [{0}]", unlinkedSuppDocPurgePeriod);
                Logger.InfoFormat("Corr Report Files Purge Period [{0}]", corrReportFilesPurgePeriod);

                Logger.InfoFormat("Log Files Purge Period [{0}]", logFilesPurgePeriod);
                Logger.InfoFormat("Temp Files Purge Period [{0}]", tempFilesPurgePeriod);

                Logger.InfoFormat("Input Files Purge Span [{0}]", inputDataFilesPurgingSpan);
                Logger.InfoFormat("Output Files Purge Span [{0}]", outputDataFilesPurgingSpan);
                Logger.InfoFormat("Unlinnked Supp Doc Files Span [{0}]", unlinkedSuppDocPurgingSpan);
                Logger.InfoFormat("Corr Report Files Purge Span [{0}]", corrReportFilesPurgingSpan);

                Logger.InfoFormat("Log Files Purge Span [{0}]", logFilesPurgingSpan);
                Logger.InfoFormat("Temp Files Purge Span [{0}]", tempFilesPurgingSpan);

                if (string.IsNullOrEmpty(arg) || arg.Equals("INPUT"))
                {
                    #region Input Files

                    if (inputDataFilesPurgePeriod != InvalidPurgePeriod)
                    {
                        Logger.Info("Searching for candidate input files started");
                        Logger.InfoFormat(
                            "Searching for candidate input files to be added to file purging queue which are older than [{0}]  days",
                            inputDataFilesPurgePeriod);
                        var inputFiles =
                            _isFileRepository.Get(
                                inputFile =>
                                inputFile.IsIncoming == false && inputFile.IsPurged == false &&
                                inputFile.FileDate <= inputDataFilesPurgingSpan && inputFile.FileDate > InvalidDate).ToList();
                        Logger.InfoFormat("Number of input files to be added to purging file queue:  [{0}] ",
                                          inputFiles.Count());
                        foreach (var isInputFile in inputFiles)
                        {
                            try
                            {
                                PurgeFile(ConvertUtil.ConvertGuidToString(isInputFile.Id), PurgingFileType.InputDataFile,
                                          Path.Combine(isInputFile.FileLocation, isInputFile.FileName), String.Empty,
                                          String.Empty);

                                Logger.InfoFormat("Purged input file at location [{0}] ",
                                                  Path.Combine(isInputFile.FileLocation, isInputFile.FileName));
                            }
                            catch (Exception exception)
                            {
                                Logger.Error("Error", exception);

                            }

                        }
                        Logger.Info("Searching for candidate input files completed");
                    }
                    else
                    {
                        Logger.Info("Input files purge period should not be zero");
                    }

                    #endregion
                }

                if (string.IsNullOrEmpty(arg) || arg.Equals("OUTPUT"))
                {
                    #region Output Files

                    if (outputDataFilesPurgePeriod != InvalidPurgePeriod)
                    {
                        Logger.InfoFormat(
                            "Searching for candidate output files to be added to file purging queue which are older than [{0}]  days",
                            outputDataFilesPurgePeriod);
                        //CMP529 : Daily Output Generation for MISC Bilateral Invoices - exclude Daily O/P file formats
                        var outputFiles =
                            _isFileRepository.Get(
                                ouputFile =>
                                ouputFile.IsIncoming && ouputFile.IsPurged == false &&
                                ouputFile.FileDate <= outputDataFilesPurgingSpan && ouputFile.FileDate > InvalidDate &&
                                (ouputFile.FileFormatId != (int)FileFormatType.DailyMiscBilateralIsXml &&
                                 ouputFile.FileFormatId != (int)FileFormatType.DailyMiscBilateralOfflineArchive &&
                                    //CMP#622: MISC Outputs Split as per Location ID
                                 ouputFile.FileFormatId != (int)FileFormatType.DailyMiscBilateralIsXmlLocSpec &&
                                 ouputFile.FileFormatId != (int)FileFormatType.DailyMiscBilateralOARLocSpec &&
                                    //262222 - SIS Legal Invoice showing up as purged after being regenerated
                                 ouputFile.FileFormatId != (int)FileFormatType.LegalInvoiceXml)).ToList
                                ();

                        Logger.InfoFormat("Number of output files to be added to purging file queue:  [{0}] ",
                                          outputFiles.Count());
                        foreach (var isOutputFile in outputFiles)
                        {
                            try
                            {
                                PurgeFile(ConvertUtil.ConvertGuidToString(isOutputFile.Id), PurgingFileType.OutputFile,
                                          Path.Combine(isOutputFile.FileLocation, isOutputFile.FileName), String.Empty,
                                          String.Empty);

                                Logger.InfoFormat("Purged output file at location [{0}] ",
                                                  Path.Combine(isOutputFile.FileLocation, isOutputFile.FileName));
                            }
                            catch (Exception exception)
                            {
                                Logger.Error("Error", exception);
                            }

                        }
                        Logger.Info("Searching for candidate output files completed");
                    }
                    else
                    {
                        Logger.Info("Output files purge period sould not be zero");
                    }

                    #endregion
                }

                //CMP529 : Daily Output Generation for MISC Bilateral Invoices
                if (string.IsNullOrEmpty(arg) || arg.Equals("DAILYOUTPUT"))
                {
                    #region Daily Output Files

                    if (dailyOutputDataFilesPurgePeriod != InvalidPurgePeriod)
                    {
                        Logger.InfoFormat(
                            "Searching for candidate daily output files to be added to file purging queue which are older than [{0}]  days",
                            dailyOutputDataFilesPurgePeriod);
                        var outputFiles =
                            _isFileRepository.Get(
                                ouputFile =>
                                ouputFile.IsIncoming && ouputFile.IsPurged == false &&
                                ouputFile.FileDate <= dailyOutputFilesPurgingSpan && ouputFile.FileDate > InvalidDate &&
                                (ouputFile.FileFormatId == (int)FileFormatType.DailyMiscBilateralIsXml ||
                                 ouputFile.FileFormatId == (int)FileFormatType.DailyMiscBilateralOfflineArchive ||
                                    //CMP#622: MISC Outputs Split as per Location ID
                                 ouputFile.FileFormatId == (int)FileFormatType.DailyMiscBilateralIsXmlLocSpec ||
                                 ouputFile.FileFormatId == (int)FileFormatType.DailyMiscBilateralOARLocSpec)).ToList
                                ();

                        Logger.InfoFormat("Number of output files to be added to purging file queue:  [{0}] ",
                                          outputFiles.Count());
                        foreach (var isOutputFile in outputFiles)
                        {
                            try
                            {
                                PurgeFile(ConvertUtil.ConvertGuidToString(isOutputFile.Id), PurgingFileType.OutputFile,
                                          Path.Combine(isOutputFile.FileLocation, isOutputFile.FileName), String.Empty,
                                          String.Empty);

                                Logger.InfoFormat("Purged output file at location [{0}] ",
                                                  Path.Combine(isOutputFile.FileLocation, isOutputFile.FileName));
                            }
                            catch (Exception exception)
                            {
                                Logger.Error("Error", exception);
                            }

                        }
                        Logger.Info("Searching for candidate output files completed");
                    }
                    else
                    {
                        Logger.Info("Output files purge period sould not be zero");
                    }

                    #endregion
                }

                if (string.IsNullOrEmpty(arg) || arg.Equals("UNLINKED"))
                {

                    #region Unlinked Supporting Documents

                    if (unlinkedSuppDocPurgePeriod != InvalidPurgePeriod)
                    {
                        try
                        {
                            Logger.Info("Searching for Unlinked Supporting Documents started");
                            Logger.InfoFormat(
                                "Searching for Unlinked Supporting Documents to be added to file purging queue which are older than [{0}]  days",
                                unlinkedSuppDocPurgePeriod);
                            var unlinkedSupportingDocumentsToQueue =
                                _unlinkedSupportingDocumentRepository.Get(
                                    supportingDoc =>
                                    supportingDoc.IsPurged == false &&
                                    supportingDoc.LastUpdatedOn <= unlinkedSuppDocPurgingSpan &&
                                    supportingDoc.LastUpdatedOn > InvalidDate)
                                    .
                                    ToList();
                            Logger.InfoFormat(
                                "Number of Unlinked Supporting Documents to be added to purging file queue:  [{0}] ",
                                unlinkedSupportingDocumentsToQueue.Count());
                            foreach (var unlinkedSupportingDocumentToQueue in unlinkedSupportingDocumentsToQueue)
                            {
                                try
                                {

                                    PurgeFile(ConvertUtil.ConvertGuidToString(unlinkedSupportingDocumentToQueue.Id),
                                              PurgingFileType.UnlinkedSupportingDocument,
                                              unlinkedSupportingDocumentToQueue.FilePath, String.Empty, String.Empty);

                                    Logger.InfoFormat("Purged unlinked supporting document at location [{0}] ",
                                                      unlinkedSupportingDocumentToQueue.FilePath);

                                }
                                catch (Exception exception)
                                {
                                    Logger.Error("Error ", exception);
                                }

                            }
                            Logger.Info("Searching for candidate legal archival files completed");
                        }
                        catch (Exception exception)
                        {
                            Logger.Error("Error ", exception);
                        }
                    }
                    else
                    {
                        Logger.Info("Unlinked Supporting Documents purge period should not be zero");
                    }

                    #endregion
                }

                if (string.IsNullOrEmpty(arg) || arg.Equals("CORRREPO"))
                {
                    #region Correspondence Report Files

                    if (corrReportFilesPurgePeriod != InvalidPurgePeriod)
                    {
                        try
                        {
                            Logger.Info("Searching for candidate correspondence report files started");
                            var correspondenceReportFilesToQueue =
                                _corrReportFileRepository.Get(
                                    reportFile =>
                                    reportFile.IsPurged == false && reportFile.FileDate <= corrReportFilesPurgingSpan &&
                                    reportFile.FileDate > InvalidDate)
                                    .
                                    ToList();
                            Logger.InfoFormat(
                                "Number of correspondence report files to be added to purging file queue:  [{0}] ",
                                correspondenceReportFilesToQueue.Count());
                            foreach (var correspondenceReportFile in correspondenceReportFilesToQueue)
                            {
                                try
                                {
                                    PurgeFile(ConvertUtil.ConvertGuidToString(correspondenceReportFile.Id),
                                              PurgingFileType.CorrespondenceReportFile,
                                              correspondenceReportFile.FilePath, String.Empty, String.Empty);

                                    Logger.InfoFormat("Purged correspondence report file at location [{0}] ",
                                                      correspondenceReportFile.FilePath);

                                }
                                catch (Exception exception)
                                {
                                    Logger.Error("Error ", exception);
                                }

                            }
                            Logger.Info("Searching for candidate correspondence report files completed");
                        }
                        catch (Exception exception)
                        {
                            Logger.Error("Error ", exception);
                        }
                    }
                    else
                    {
                        Logger.Info("candidate correspondence report files purge period sould not be zero");
                    }

                    #endregion
                }

                if (string.IsNullOrEmpty(arg) || arg.Equals("SUPPDOCS"))
                {
                    #region Supporting Documents Files

                    //Purge Supporting Documents Files which are queued in IS_FILE_PURGE_QUEUE Table.
                    var suppDocsForPurging = _isFilePurgeQueueRepository.Get(p => p.IsPurged == false && p.FileTypeId == (int)PurgingFileType.SupportingDocument).ToList();
                    Logger.InfoFormat("Number of Supporting Documents to be added to purging file queue: [{0}] ",
                                      suppDocsForPurging.Count);
                    foreach (var filetopurge in suppDocsForPurging)
                    {
                        Logger.InfoFormat("Deleting Supporting Document file at location [{0}]", filetopurge.FilePath);
                        if (DeleteFileFromLocation(filetopurge.FilePath))
                        {
                            Logger.Info("Updating purge status in database.");
                            Business.Common.FilePurgingManager.UpdateFilePurgeQueueStatus(ConvertUtil.ConvertGuidToString(filetopurge.Id));

                            Logger.InfoFormat("Deleting empty folders from: [{0}]", filetopurge.FilePath);
                            RemoveEmptyFolders(Directory.GetParent(filetopurge.FilePath).FullName);
                        }

                        //if (filetopurge.FilePath.IndexOf("\\SUPPDOCS\\") >= 0)
                        //{
                        //  Logger.InfoFormat("Deleting empty folders from: [{0}]",
                        //                    filetopurge.FilePath.Substring(0,
                        //                                                   filetopurge.FilePath.IndexOf("\\SUPPDOCS\\") +
                        //                                                   9));
                        //  if (Directory.Exists(filetopurge.FilePath.Substring(0, filetopurge.FilePath.IndexOf("\\SUPPDOCS\\") + 9)))
                        //  {
                        //    DeleteEmptyDirs(filetopurge.FilePath.Substring(0,
                        //                                                   filetopurge.FilePath.IndexOf("\\SUPPDOCS\\") +
                        //                                                   9));
                        //  }
                        //  else
                        //  {
                        //    Logger.InfoFormat("Message from file purging. Need urgent attention. File not found: [{0}]", filetopurge.FilePath);
                        //    CommonUtil.SendEmail(new Exception("Message from file purging. Need urgent attention. File not found: " + filetopurge.FilePath), "File not found: " + filetopurge.FilePath);
                        //  }
                        //}
                    }
                    #endregion
                }

                // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
                // Implemented code to delete offline collection files.
                if (string.IsNullOrEmpty(arg) || arg.Equals("OFFLINECOLLFILE"))
                {
                    #region Offline Collection Files
                    Logger.Info("Deleting Invoice Offline Collection Files.");
                    // Delete Invoice Offline Collection Files.
                    DeleteInvoiceFormCOfflineCollectionFiles();

                    Logger.Info("Completed Deletion of Invoice Offline Collection Files.");

                    Logger.Info("Deleting Form C Offline Collection Files.");

                    // Delete FormC Offline Collection Files.
                    DeleteInvoiceFormCOfflineCollectionFiles(false);

                    Logger.Info("Completed Deletion of Form C Offline Collection Files.");
                    #endregion

                }// End if - Offline Section

                //SCP317057: ISWEB requested OAR files are not getting purged
                if (string.IsNullOrEmpty(arg) || arg.Equals("ISWEBOAR"))
                {
                    #region ISWEB Requested OAR Files

                    if (tempFilesPurgePeriod != InvalidPurgePeriod)
                    {
                        try
                        {
                            string oarPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollWebRoot);

                            Logger.Info("Searching for candidate ISWEB Requested OAR Files started");
                            Logger.InfoFormat(
                                "Searching for candidate ISWEB Requested OAR Files at location [{0}] to be added to file purging queue which are older than [{1}]  days",
                                oarPath, tempFilesPurgePeriod);

                            DirectoryInfo sfrTemp = new DirectoryInfo(oarPath);

                            DeleteOarLinksFromFolder(sfrTemp, tempFilesPurgingSpan);

                            Logger.Info("Searching for candidate ISWEB Requested OAR Files completed");
                        }
                        catch (Exception exception)
                        {
                            Logger.Error("Error ", exception);
                        }

                    }
                    else
                    {
                        Logger.Info("candidate ISWEB Requested OAR Files purge period should not be zero");
                    }
                    #endregion
                }

                //#region Log Files

                //if (logFilesPurgePeriod != InvalidPurgePeriod)
                //{
                //    try
                //    {
                //        Logger.Info("Searching for candidate log files started");
                //        var serviceservicemapping = _serverServiceMappingRepository.GetAll().ToList();

                //        foreach (var service in serviceservicemapping)
                //        {
                //            var serviceLocation = service.ServiceLocation;
                //            var logFilesLocation = Path.Combine(serviceLocation, LogsFolderName);
                //            Logger.InfoFormat("Searching for log file at [{0}]", logFilesLocation);

                //            if (Directory.Exists(logFilesLocation))
                //            {
                //                var logFileDirectory = new DirectoryInfo(logFilesLocation);
                //                var logFiles = logFileDirectory.GetFiles();
                //                var logFilesToPurge = logFiles.Where(file => file.CreationTimeUtc <= logFilesPurgingSpan);

                //                foreach (var logFileToPurge in logFilesToPurge)
                //                {
                //                    // log should not delete. All logs will move to other location depend on server name and service name
                //                    PurgeFile(string.Empty, PurgingFileType.LogFile, logFileToPurge.FullName, String.Empty, String.Empty);
                //                }

                //            }
                //        }
                //        Logger.Info("Searching for candidate log files completed");
                //    }
                //    catch (Exception exception)
                //    {
                //        Logger.Error("Error ", exception);
                //    }
                //}
                //else
                //{
                //    Logger.Info("Candidate log files purge period should not be zero");
                //}

                //#endregion

                #region Temporary Files

                if (tempFilesPurgePeriod != InvalidPurgePeriod)
                {
                    try
                    {
                        //CMP508: Audit Trail Download with Supporting Documents
                        string basePath = FileIo.GetForlderPath(SFRFolderPath.SFRTempRootPath);

                        Logger.Info("Searching for candidate temporary files started");
                        Logger.InfoFormat(
                            "Searching for candidate temporary files at location [{0}] to be added to file purging queue which are older than [{1}]  days",
                                basePath, tempFilesPurgePeriod);

                        DirectoryInfo sfrTemp = new DirectoryInfo(basePath);

                        #region TEMPFILES

                        DeleteFilesFromFolder(sfrTemp, tempFilesPurgingSpan);

                        #endregion

                        #region TEMPFOLDER

                        //DELETE all the folders. empty folders and files which pass the criteria, but dont delete this dir
                        //SCP0000: Modified code to exempt deleteing CSV Output folder existing inside TEMP location
                        foreach (DirectoryInfo dirInfo in sfrTemp.GetDirectories().Where(dir => !dir.Name.ToUpper().Contains(@"LEGALXMLWORKFOLDER") && !dir.Name.ToUpper().Contains(@"CSVOUTPUT")))
                        {
                            Logger.InfoFormat("Searching for candidate Temporary folders at location [{0}] to be deleted ", dirInfo.FullName);

                            DeleteFilesAndFolders(dirInfo, tempFilesPurgingSpan);
                        }

                        #endregion

                        Logger.Info("Searching for candidate temporary files completed");
                    }
                    catch (Exception exception)
                    {
                        Logger.Error("Error ", exception);
                    }

                }
                else
                {
                    Logger.Info("Candidate temporary files purge period sould not be zero");
                }

                #endregion
            }
            catch (Exception ex)
            {
                var customMessage = string.Format("Purging file Error in SearchPurgingFiles Method : {0}", ex.Message);
                CommonUtil.SendEmail(ex, customMessage);
                throw;
            }
        }

        /// <summary>
        /// Removes empty folders 
        /// </summary>
        /// <param name="filePath"></param>
        private static void RemoveEmptyFolders(string filePath)
        {
            try
            {
                if (!Directory.EnumerateDirectories(filePath).Any())
                {
                    if (!Directory.EnumerateFileSystemEntries(filePath).Any())
                    {
                        Directory.Delete(filePath);
                        RemoveEmptyFolders(Directory.GetParent(filePath).FullName);
                    }
                }
            }
            catch (Exception)
            {
                Logger.Info("Directory not found.");
            }

        }

        // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
        /// <summary>
        /// Method deletes all the Invoice/FormC offline collection files and folders(i.e. Except files and folder inside "SUPPDOCS" folder) to be purged.
        /// Folder path from which to delete files is fetched from IsFilePurgeQueue. 
        /// Also deletes Invoice offline collection meta data associated with it.
        /// </summary>
        /// <param name="isInvoice">Flag indicates:
        ///                         "true": Delete Invoice offline collection files/folders.
        ///                         "false:  Delete FormC offline collection files/folders.</param>
        private void DeleteInvoiceFormCOfflineCollectionFiles(bool isInvoice = true)
        {
            var invoiceOffColMetaDataRepository =
                  Ioc.Resolve<IRepository<InvoiceOfflineCollectionMetaData>>(
                    typeof(IRepository<InvoiceOfflineCollectionMetaData>));

            var sisSanPathConfigRepository = Ioc.Resolve<IRepository<SisSanPathConfiguration>>(
                typeof(IRepository<SisSanPathConfiguration>));

            // Fetch list of all records for which Invoice offline collection files needs to be deleted.
            var invFormCOfflineFilesToPurge = isInvoice
                                              ? _isFilePurgeQueueRepository.Get(
                                                p =>
                                                p.IsPurged == false &&
                                                p.FileTypeId == (int)PurgingFileType.InvoiceOffliceCollectionFilesFolders).
                                                  ToList()
                                              : _isFilePurgeQueueRepository.Get(
                                                p =>
                                                p.IsPurged == false &&
                                                p.FileTypeId == (int)PurgingFileType.FormCOffliceCollectionFilesFolders).
                                                  ToList();

            var processingType = isInvoice ? "Invoice" : "Form C";


            // Iterate through each BillingYearMonthPeriod directory. e.g. "20131002" etc.
            foreach (var isFilePurgeQueue in invFormCOfflineFilesToPurge)
            {
                try
                {


                    Logger.InfoFormat("Processing Started for {0} Offline Collection Directory: [{1}]", processingType, isFilePurgeQueue.FilePath);

                    string sFilePath = isFilePurgeQueue.FilePath.Replace("\\", "");
                    // Parse calendar period from the file path.
                    DateTime calendarPeriod = DateTime.ParseExact(sFilePath.Substring(sFilePath.Length - 8), "yyyyMMdd", CultureInfo.InvariantCulture);

                    Logger.Info("Deleting Files for Date: " + calendarPeriod.ToString());

                    if (Directory.Exists(isFilePurgeQueue.FilePath))
                    {
                        if (isInvoice)
                        {
                            Logger.Info("Fetching List of all invoice directories present in file path to purge");
                            // Fetch List of all invoice directories present in file path to purge.
                            var invDirs = Directory.GetDirectories(isFilePurgeQueue.FilePath, "INV-*", SearchOption.AllDirectories);

                            Logger.Info("Iterating through all the einvoice folders present inside invoice folders and delete them");
                            // Iterate through all the einvoice folders present inside invoice folders and delete them
                            foreach (var eInvoiceDir in Directory.GetDirectories(isFilePurgeQueue.FilePath, "E-INVOICE", SearchOption.AllDirectories).Where(eInv => invDirs.Count(eInv.Contains) > 0))
                            {
                                Directory.Delete(eInvoiceDir, true);
                            }// End foreach

                            Logger.Info("Iterating through all the listings folders present inside invoice folders and delete them");
                            // Iterate through all the listings folders present inside invoice folders and delete them
                            foreach (var listingDir in Directory.GetDirectories(isFilePurgeQueue.FilePath, "LISTINGS", SearchOption.AllDirectories).Where(listing => invDirs.Count(listing.Contains) > 0))
                            {
                                Directory.Delete(listingDir, true);
                            }// End foreach

                            Logger.Info("Iterating through all the memos folders present inside invoice folders and delete them");
                            // Iterate through all the memos folders present inside invoice folders and delete them
                            foreach (var memosDir in Directory.GetDirectories(isFilePurgeQueue.FilePath, "MEMOS", SearchOption.AllDirectories).Where(memo => invDirs.Count(memo.Contains) > 0))
                            {
                                Directory.Delete(memosDir, true);
                            }// End foreach

                        }// End if
                        else
                        {
                            Logger.Info("Fetch List of all formc directories present in file path to purge");
                            // Fetch List of all formc directories present in file path to purge.
                            var frmCDirs = Directory.GetDirectories(isFilePurgeQueue.FilePath, "FORMC-*", SearchOption.AllDirectories);

                            Logger.Info("Iterating through all the listings folders present inside Form C folders and delete them");
                            // Iterate through all the listings folders present inside formc folders and delete them
                            foreach (var listingDir in Directory.GetDirectories(isFilePurgeQueue.FilePath, "LISTINGS", SearchOption.AllDirectories).Where(listing => frmCDirs.Count(listing.Contains) > 0))
                            {
                                Directory.Delete(listingDir, true);
                            }// End foreach

                        }// End else

                        Logger.Info("Iterating through all empty directories and deleting them");
                        // Iterate through all empty directories and delete them.
                        foreach (var emptyDir in Directory.GetDirectories(isFilePurgeQueue.FilePath, "*", SearchOption.AllDirectories).Where(dir => new DirectoryInfo(dir).GetDirectories().Count() == 0 && new DirectoryInfo(dir).GetFiles().Count() == 0).ToList())
                        {
                            Directory.Delete(emptyDir);
                        }// End foreach

                        // Fetch the SAN Path for which Invoice/FormC offline collection file purging is complete.
                        var purgedOfflineColSanPath = sisSanPathConfigRepository.Get(sisSanPath => sisSanPath.Id == calendarPeriod.Date).FirstOrDefault();

                        // If record found.
                        if (purgedOfflineColSanPath != null)
                        {
                            if (isInvoice)
                            {
                                // Set IsInvoicePurged flag to true.
                                purgedOfflineColSanPath.IsInvoicePurged = true;
                            }// End if
                            else
                            {
                                // Set IsFormCPurged flag to true.
                                purgedOfflineColSanPath.IsFormCPurged = true;
                            }// End else

                            // Update details in DB.
                            sisSanPathConfigRepository.Update(purgedOfflineColSanPath);

                            // Commit changes.
                            UnitOfWork.CommitDefault();

                        }// End if

                    }// End if

                    Logger.Info("Updating isPurged, LastUpdatedOn info and save it to database.");

                    // Update isPurged and LastUpdatedOn info and save it to database.
                    UpdateIsFilePurge(isFilePurgeQueue.Id);

                    Logger.InfoFormat("Processing Ended for {0} Offline Collection Directory: [{1}]", processingType, isFilePurgeQueue.FilePath);
                }
                catch (Exception)
                {
                    Logger.InfoFormat("Error while processing Offline Collection Directory: [{0}]", isFilePurgeQueue.FilePath);
                }
            } // End foreach - BillingYearMonthPeriod directories.
        }// End DeleteInvoiceFormCOfflineCollectionFiles()

        public void DeleteFilesAndFolders(DirectoryInfo folder, DateTime tempFilesPurgingSpan)
        {
            //DELETE all the folders. empty folders and files which pass the criteria, but dont delete this dir
            //DeleteFilesFromFolder(folder, tempFilesPurgingSpan);
            foreach (DirectoryInfo dirInfo in folder.GetDirectories())
            {
                //SCP0000: Modified code to exempt deleteing CSV Output folder existing inside TEMP location
                Logger.InfoFormat("Purging TempFolder from location [{0}] ", dirInfo.FullName);
                //DeleteFilesRecursively(dirInfo, tempFilesPurgingSpan);
                DeleteEmptyDirs(dirInfo.FullName);
            }
        }

        //public void DeleteFilesRecursively(DirectoryInfo folder, DateTime tempFilesPurgingSpan)
        //{
        //    DeleteFilesFromFolder(folder, tempFilesPurgingSpan);
        //    foreach (DirectoryInfo dirInfo in folder.GetDirectories())
        //    {
        //        DeleteFilesRecursively(dirInfo, tempFilesPurgingSpan);
        //    }
        //}

        public void DeleteFilesFromFolder(DirectoryInfo folder, DateTime tempFilesPurgingSpan)
        {
            var receivableReportPath = FileIo.GetForlderPath(SFRFolderPath.ReceivableRmbmcmSummaryReportPath);
            var payableReportPath = FileIo.GetForlderPath(SFRFolderPath.PayableRmbmcmSummaryReportPath);
            var enableOfflineReportPurging = ConfigurationManager.AppSettings["EnableOfflineReportPurging"];
            //DELETE FILES FROM TEMP DIRECTORY
            var tempFiles = folder.GetFiles("*.*", SearchOption.AllDirectories).Where(file => !file.FullName.ToUpper().Contains(@"\LEGALXMLWORKFOLDER\"));
            Logger.InfoFormat("Number of temporary files found : [{0}] ", tempFiles.Count());

            //GET TEMP FILES TO BE DELETED
            var tempFilesToQueue = tempFiles.Where(file => file.CreationTimeUtc <= tempFilesPurgingSpan).ToList();
            Logger.InfoFormat("Number of temporary files found to be added to purging file queue : [{0}] ", tempFilesToQueue.Count());

            foreach (var tempFileToQueue in tempFilesToQueue)
            {
                try
                {
                    //SCP223595 - FW: RM BM CM SUMMARY. 
                    //Purged from offline report log table data.
                    if (tempFileToQueue.FullName.Contains(receivableReportPath) || tempFileToQueue.FullName.Contains(payableReportPath))
                    {
                        if (enableOfflineReportPurging == "1")
                        {
                            Guid offlineReportLogId = tempFileToQueue.Name.ToLower().Replace(".zip", String.Empty).ToGuid();
                            _offlineReportLogManager.DeleteOfflineReportLogRow(offlineReportLogId);
                            Logger.InfoFormat("Data deleted from offline_report_log table, file name [{0}] ", tempFileToQueue.FullName);
                        }
                        else
                            continue;
                    }
                    PurgeFile(string.Empty, PurgingFileType.TemporaryFile, tempFileToQueue.FullName, String.Empty, String.Empty);
                    Logger.InfoFormat("Purged temp files at location [{0}] ", tempFileToQueue.FullName);
                }
                catch (Exception exception)
                {
                    Logger.Error("Error ", exception);
                }
            }
        }

        /// <summary>
        /// This method add entry in file purging queue
        /// Method will be called from SearchPurgingFiles() 
        /// </summary>
        /// <param name="purgingFileQueueMessage"></param>
        [Obsolete]
        public void QueuePurgingFile(PurgingFileQueueMessage purgingFileQueueMessage)
        {
            Logger.InfoFormat("Queueing file for purging");

            try
            {
                Logger.InfoFormat("{0}{1}", "Adding entry to queue", purgingFileQueueMessage.PurgingFilePath);
                var filePurgingQueueName = ConfigurationManager.AppSettings["FilePurgingQueueName"];
                if (!string.IsNullOrEmpty(filePurgingQueueName))
                {
                    var queueHelper = new QueueHelper(filePurgingQueueName);
                    IDictionary<string, string> queueMessage = new Dictionary<string, string>();


                    queueMessage.Add("PURGING_FILE_PATH", purgingFileQueueMessage.PurgingFilePath);
                    queueMessage.Add("PURGING_FILE_TYPE_ID", (purgingFileQueueMessage.PurgingFileTypeId).ToString());
                    queueMessage.Add("PURGING_FILE_ID", purgingFileQueueMessage.FileId);
                    queueMessage.Add("SERVER_NAME", purgingFileQueueMessage.ServerName);
                    queueMessage.Add("SERVICE_NAME", purgingFileQueueMessage.ServiceName);
                    queueHelper.Enqueue(queueMessage);

                    Logger.InfoFormat("Enqueued file at [{0}]for purging", purgingFileQueueMessage.PurgingFilePath);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat("Exception occured while enqueueing file at [{0}] ", exception);
                //Send Mail
            }
        }

        /// <summary>
        /// This method purges the given file
        /// Purging depends on PurgingFileType
        /// This method will be called from FilePurgingService after a messages is dequeued.
        /// </summary>
        /// <returns></returns>
        public void PurgeFiles(PurgingFileQueueMessage purgingFileQueueMessage)
        {
            PurgeFile(purgingFileQueueMessage.FileId, purgingFileQueueMessage.PurgingFileType,
                       purgingFileQueueMessage.PurgingFilePath, purgingFileQueueMessage.ServerName,
                       purgingFileQueueMessage.ServiceName);
        }

        /// <summary>
        /// This method purge file based on file types
        /// </summary>
        /// <param name="purgeFileId">unique identifier for file by it's id</param>
        /// <param name="fileType">define it's file type</param>
        /// <param name="filePath">location where file is reside</param>
        /// <param name="serverName">server name where log files will move</param>
        /// <param name="serviceName">service name by which log files move</param>
        private void PurgeFile(string purgeFileId, PurgingFileType fileType, string filePath, string serverName = "",
                               string serviceName = "")
        {
            _invoiceRepository = Ioc.Resolve<IInvoiceRepository>();

            switch (fileType)
            {
                case PurgingFileType.InputDataFile:

                    Logger.InfoFormat("Deleting input data file at location [{0}]", filePath);
                    if (DeleteFileFromLocation(filePath))
                    {
                        _invoiceRepository.UpdateFileLogPurgedStatus(purgeFileId, 1, (int)PurgingFileType.InputDataFile);
                    }
                    break;

                case PurgingFileType.OutputFile:
                    Logger.InfoFormat("Deleting output data file at location [{0}]", filePath);
                    if (DeleteFileFromLocation(filePath))
                    {
                        _invoiceRepository.UpdateFileLogPurgedStatus(purgeFileId, 1, (int)PurgingFileType.OutputFile);
                    }
                    break;

                case PurgingFileType.SupportingDocument:
                    Logger.InfoFormat("Deleting (linked) supporting document data file at location [{0}]", filePath);
                    DeleteFileFromLocation(filePath);
                    break;

                case PurgingFileType.UnlinkedSupportingDocument:
                    Logger.InfoFormat("Deleting unlinked supporting document data file at location [{0}]", filePath);
                    if (DeleteFileFromLocation(filePath))
                    {
                        _invoiceRepository.UpdateFileLogPurgedStatus(purgeFileId, 1,
                                                                     (int)PurgingFileType.UnlinkedSupportingDocument);
                    }
                    break;
                case PurgingFileType.CorrespondenceFile:
                    Logger.InfoFormat("Deleting correspondence file at location [{0}]", filePath);
                    if (DeleteFileFromLocation(filePath))
                    {
                        _invoiceRepository.UpdateFileLogPurgedStatus(purgeFileId, 1, (int)PurgingFileType.CorrespondenceFile);
                    }

                    break;
                case PurgingFileType.CorrespondenceReportFile:
                    Logger.InfoFormat("Deleting correspondence report file at location [{0}]", filePath);
                    if (DeleteFileFromLocation(filePath))
                    {
                        _invoiceRepository.UpdateFileLogPurgedStatus(purgeFileId, 1,
                                                                     (int)PurgingFileType.CorrespondenceReportFile);
                    }
                    break;
                case PurgingFileType.LegalArchive:
                    Logger.InfoFormat("Deleting legal archive at location [{0}]", filePath);
                    DeleteFileFromLocation(filePath, true);
                    break;

                case PurgingFileType.LogFile:

                    string purgedLogFilesDistinationDirectory =
                        SystemParameters.Instance.PurgingPeriodDetails.LogFilePath;

                    if (purgedLogFilesDistinationDirectory != null &&
                        Directory.Exists(purgedLogFilesDistinationDirectory))
                    {
                        if (File.Exists(filePath))
                        {

                            string logFileName = Path.GetFileName(filePath);
                            string logFileDestinationPath = Path.Combine(purgedLogFilesDistinationDirectory, serverName,
                                                                         serviceName, logFileName);


                            //Check If directory for ServerName exist. If not create it.
                            if (!Directory.Exists(Path.Combine(purgedLogFilesDistinationDirectory, serverName)))
                            {
                                Logger.InfoFormat("Creating  directory for ServerName [{0}]",
                                                  Path.Combine(purgedLogFilesDistinationDirectory, serverName));
                                Directory.CreateDirectory(Path.Combine(purgedLogFilesDistinationDirectory, serverName));
                                Logger.InfoFormat("Created  directory for ServerName [{0}]",
                                                  Path.Combine(purgedLogFilesDistinationDirectory, serverName));

                                //Check If directory for ServiceName exist. If not create it.
                                if (
                                    !Directory.Exists(Path.Combine(purgedLogFilesDistinationDirectory, serverName,
                                                                   serviceName)))
                                {
                                    Logger.InfoFormat("Creating  directory for ServiceName [{0}]",
                                                      Path.Combine(purgedLogFilesDistinationDirectory, serverName,
                                                                   serviceName));
                                    Directory.CreateDirectory(Path.Combine(purgedLogFilesDistinationDirectory,
                                                                           serverName));
                                    Logger.InfoFormat("Created  directory for ServiceName [{0}]",
                                                      Path.Combine(purgedLogFilesDistinationDirectory, serverName,
                                                                   serviceName));

                                }

                            }

                            Logger.InfoFormat("Moving log file at location [{0}] to [{1}]", filePath,
                                              logFileDestinationPath);
                            if (MoveFile(filePath, logFileDestinationPath))
                            {
                                Logger.InfoFormat("Moved log file at location [{0}] to [{1}]", filePath,
                                                  logFileDestinationPath);
                            }
                            else
                            {
                                Logger.InfoFormat("Failed to move log file at location [{0}] to [{1}]", filePath,
                                                  logFileDestinationPath);
                            }

                        }
                        else
                        {
                            Logger.InfoFormat("No log file exist at location [{0}]", filePath);
                            // Send Mail

                        }
                    }
                    else
                    {
                        Logger.InfoFormat("PurgedLogFilesDistinationDirectory not defined in System parameters");
                    }
                    break;

                case PurgingFileType.TemporaryFile:
                    Logger.InfoFormat("Deleting temporary file at location [{0}]", filePath);
                    DeleteFileFromLocation(filePath);
                    break;

            }
        }

        /// <summary>
        /// Deletes file/directory from the location specified in the queue message.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isDirectoryDelete">Will be true for legal archive.</param>
        /// <returns></returns>
        private bool DeleteFileFromLocation(string filePath, bool isDirectoryDelete = false)
        {
            bool returnValue = false;
            try
            {
                if (!isDirectoryDelete)
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        returnValue = true;
                    }
                    else
                    {
                        Logger.Info("File not found.");
                    }
                }
                else
                {
                    if (Directory.Exists(filePath))
                    {
                        Directory.Delete(filePath, true);
                        returnValue = true;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Error in DeleteFileFromLocation Method ", exception);
                returnValue = false;
                //Send Mail
            }

            // Return true in any case, whether the file exist or not
            return true;
            //return returnValue;

        }

        //SCP317057: ISWEB requested OAR files are not getting purged
        /// <summary>
        /// Deletes file/directory from the location specified in the queue message.
        /// </summary>
        /// <param name="folder">Base Folder Path</param>
        /// <param name="tempFilesPurgingSpan">OAR files Purging Span.</param>
        /// <returns></returns>
        public void DeleteOarLinksFromFolder(DirectoryInfo folder, DateTime tempFilesPurgingSpan)
        {
            string oarlinkPath = FileIo.GetForlderPath(SFRFolderPath.PathOfflineCollWebRoot);

            //DELETE FILES FROM ISWEB DIRECTORY
            var iswebRequestedOarFiles = folder.GetFiles("*.*", SearchOption.AllDirectories);

            Logger.InfoFormat("Number of ISWEB Requested OAR Files to be added for file purging:  [{0}] ", iswebRequestedOarFiles.Count());

            //GET OAR LINK FILES TO BE DELETED
            var iswebRequestedOarFilesToPurge = iswebRequestedOarFiles.Where(file => file.CreationTimeUtc <= tempFilesPurgingSpan).ToList();

            Logger.InfoFormat("Number of ISWEB Requested OAR Files found eligible to be added for purging files: [{0}] ", iswebRequestedOarFilesToPurge.Count());

            foreach (var iswebRequestedOarFileToPurge in iswebRequestedOarFilesToPurge)
            {
                try
                {
                    if (iswebRequestedOarFileToPurge.FullName.Contains(oarlinkPath))
                    {
                        var filename = iswebRequestedOarFileToPurge.FullName;

                        var downloadLinkId = _isHttpDownloadLinkRepository.Single(downloadLink => downloadLink.FilePath == filename);

                        if (downloadLinkId != null)
                        {
                            _isHttpDownloadLinkRepository.Delete(downloadLinkId);
                        }
                        //Commit http download link row deletion.
                        UnitOfWork.CommitDefault();
                        Logger.InfoFormat("Data deleted from IS_HTTP_DOWNLOAD_Link table, file name [{0}] ", iswebRequestedOarFileToPurge.FullName);
                    }

                    PurgeFile(string.Empty, PurgingFileType.TemporaryFile, iswebRequestedOarFileToPurge.FullName, String.Empty, String.Empty);
                    Logger.InfoFormat("Purged ISWEB Requested OAR Files at location [{0}] ", iswebRequestedOarFileToPurge.FullName);
                }
                catch (Exception exception)
                {
                    Logger.Error("Error ", exception);
                }
            }
        }

        /// <summary>
        /// Moves the file from source path to destination path
        /// </summary>
        /// <param name="sourceFilePath">sourceFilePath</param>
        /// <param name="destinationFilePath">destinationFilePath</param>
        /// <returns></returns>
        private bool MoveFile(string sourceFilePath, string destinationFilePath)
        {
            bool returnValue = false;
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(destinationFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));


                if (File.Exists(destinationFilePath))
                {
                    Logger.Info("Deleting existing file.");
                    File.Delete(destinationFilePath);
                }

                File.Move(sourceFilePath, destinationFilePath);
                returnValue = true;
            }
            catch (Exception exception)
            {
                Logger.Error("Error in MoveFiletoTarget Method ", exception);
                //Send Mail

            }
            return returnValue;
        }

        /// <summary>
        /// Update record in IS_FILE_PURGE_QUEUE table to set ispurge flag to true.
        /// </summary>
        /// <param name="isFilePurgeId"></param>
        /// <returns></returns>
        public bool UpdateIsFilePurge(Guid isFilePurgeId)
        {
            bool result = false;
            try
            {

                var file = _isFilePurgeQueueRepository.Get(p => p.Id == isFilePurgeId).FirstOrDefault();
                if (file != null)
                {
                    file.IsPurged = true;
                    file.LastUpdatedOn = DateTime.UtcNow;
                }
                UnitOfWork.CommitDefault();
                result = true;

            }
            catch (Exception exception)
            {
                Logger.Error("Error in UpdateIsFilePurge Method :", exception);
                var customMessage = string.Format("Purging file Error in UpdateIsFilePurge Method : {0}", exception.Message);
                CommonUtil.SendEmail(exception, customMessage);
                result = false;
                //Send Mail
            }
            return result;
        }

        private static bool DeleteEmptyDirs(string dir)
        {
            if (String.IsNullOrEmpty(dir))
                throw new ArgumentException("Starting directory is a null reference or an empty string", "dir");

            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        //SCP0000: Modified code to exempt deleteing CSV Output folder existing inside TEMP location
                        Logger.InfoFormat("Deleted Empty temp folder from location [{0}] ", dir.ToUpper());
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception exception)
            {
                Logger.Error("Error in deleting empty directoy methos:", exception);
                var customMessage = string.Format("Purging file Error in DeleteEmptyDirs Method : {0}", exception.Message);
                CommonUtil.SendEmail(exception, customMessage);
                return false;
            }

            return true;
        }

        // CMP599 - Multiple SAN for Offline Collection Files(One SAN Path per Calendar Period).
        /// <summary>
        /// Method fetches current billing period if open else pervious period and use it 
        /// to calulate threshold periods used for purging of Invoice/Form C offline collection files.
        /// Call stored procedure by passing calulated threshold periods. The stored proc then queues
        /// Invoice/Form C offline collection files to file purging queue which belong to periods equal to or
        /// are older than the threshold periods.  
        /// </summary>
        public void QueueOfflineCollectionFilesForPurging()
        {
            try
            {
                // Resolve calendar and purge transaction repositories. 
                var calendarManager = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager));
                var purgeTranRepository = Ioc.Resolve<IPurgingTransactionRepository>(typeof(IPurgingTransactionRepository));

                // Get current period if open else get previous period.
                var currentPeriod = calendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);

                Logger.InfoFormat("Fetched Current Period: P{0}-{1}-{2}", currentPeriod.Period, currentPeriod.Month.ToString().PadLeft(2, '0'), currentPeriod.Year.ToString().PadLeft(4, '0'));

                // Calculate threshold period for Invoice offline collection files.
                DateTime invEndPeriod = new DateTime(currentPeriod.Year, currentPeriod.Month, 4).AddMonths(-13);

                // Calculate threshold period for Form C offline collection files.
                DateTime formCEndPeriod = new DateTime(currentPeriod.Year, currentPeriod.Month, 4).AddMonths(-16);

                Logger.InfoFormat("Calling Stored Procedure to queue off line collection files. Input paramemters, invEndPeriod:{0}, formCEndPeriod:{1}", invEndPeriod.Date, formCEndPeriod.Date);

                // Call stored procedure which queue Invoice/Form C offline collection files to file purging queue.
                purgeTranRepository.QueueOfflineCollectionFilesForPurging(invEndPeriod.Date, formCEndPeriod.Date);

                Logger.Info("Stored procedure executed successfully.");

            }// End try
            catch (Exception ex)
            {
                Logger.Error("Exception occured while queuing offline collection files for purging.", ex);
                throw;
            }// End catch

        }// End QueueOfflineCollectionFilesForPurging()
    }
}
