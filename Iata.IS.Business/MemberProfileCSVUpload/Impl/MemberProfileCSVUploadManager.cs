using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile.Impl;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Pax.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MemberProfileCSVUpload;
using Iata.IS.Model.Pax.Common;
using iPayables.Security;
using iPayables.UserManagement;
using log4net;
using NVelocity;
using MemberManager = Iata.IS.Business.Common.MemberManager;
using Iata.IS.Business.MemberProfile;

namespace Iata.IS.Business.MemberProfileCSVUpload.Impl
{
    public class MemberProfileCsvUploadManager : IMemberProfileCSVUploadManager
    {

        #region Class Data Members

        /* Logger instance. */
        private readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IsInputFile iSFileLogRecord;

        private string userName;

        private bool _level1ValidationStatus = true;

        private bool _level2ValidationStatus = true;

        private Dictionary<int, List<string>> _csvFileData;
        private List<MemberProfileData> _memberProfiles = new List<MemberProfileData>();

        public IInvoiceRepository invRepository { get; set; }
        public IReferenceManager referenceManager { get; set; }
        public IMemberRepository memberRepository { get; set; }
        public IBroadcastMessagesManager broadcastMessagesManager { get; set; }
        public ISisMemberSubStatusManager membershipSubStatusManager { get; set; }

        //added for CMP685
        public IUserManagement AuthManager { get; set; }
        public IUserManagementManager UserManagementManager { get; set; }

      

        private List<string> existingMemberCodeNumericValues = new List<string>();

        private List<SisMemberSubStatus> membershipSubStatusDetails = null;

        private List<string> listOfAllActiveContries = new List<string>();

        private List<string> listOfAllActiveContriesSupportingDS = new List<string>();

        private List<string> existingUserEmailIdValues = new List<string>();

        private List<string> existingPermissionTemplateValues = new List<string>();

        private string decompressedDirectoryPath = string.Empty;

        string destinationFilePath = string.Empty;

        private List<String> ValidValuesForMiscDailyPayable = new List<String> {"N", "W", "WO", "WX", "WOX"};

        #endregion

        #region Common Code / Helper Methods Region

        public bool PerformSanityAndValidationForMemberProfileCSV(Guid isFileLogId)
        {
            try
            {
                iSFileLogRecord = referenceManager.GetIsInputFileFromIsFileId(isFileLogId);
                
                /* Get User Name From User Id */
                int userId = iSFileLogRecord.LastUpdatedBy;
                var AuthManager = new UserManagementModel();
                var sisUser = AuthManager.GetUserByUserID(userId);
                /* Name of SIS Ops user who uploaded the file. Format should be First Name followed by space followed by Last Name */
                this.userName = string.Format("{0} {1}", sisUser.FirstName, sisUser.LastName);

                /* Update IS-File Log Details indicating that Phase-1 i.e. - Validation Level-1 (Sanity) is Started */
                invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "SANITY_START", null, null, null, 0, 0, 0, 0, 0);

                /* Perform First Level Validations */
                Logger.Info("Beginning of Level 1 Validations.");
                bool level1ValidationResult = PerformFirstLevelValidations();
                Logger.InfoFormat("Completion of Level 1 Validations, outcome is {0}", GetLevel1ValidationStatus());

                if (GetLevel1ValidationStatus() == true && level1ValidationResult == true)
                {
                    /* Update IS-File Log Details indicating that Phase-1 i.e. - Validation Level-1 (Sanity) is completed */
                    invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "SANITY_END", null, null, null, 0, 0, 0, 0, 0);

                    /* Update IS-File Log Details indicating that Phase-1 i.e. - Validation Level-2 is Started */
                    invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "VALIDATION_START", null, null, null, 0, 0, 0, 0, 0);

                    /* Since Level 1 is success, Perform Level 2 Checks. */

                    /* First Read of Master Data */
                    Logger.Info("Initializing Member CSV Master Data.");
                    Iata.IS.Business.Common.MemberUploadCSVDataManager memberUploadCsvDataManager = new MemberUploadCSVDataManager();
                    memberUploadCsvDataManager.InitialiseMasterData(ref existingMemberCodeNumericValues,
                                                                    ref listOfAllActiveContries,
                                                                    ref listOfAllActiveContriesSupportingDS,
                                                                    ref existingUserEmailIdValues,
                                                                    ref existingPermissionTemplateValues);

                    /* Perform Second Level Validations */
                    Logger.Info("Beginning of Level 2 Validations.");
                    bool level2ValidationResult = PerformSecondLevelValidations();
                    Logger.InfoFormat("Completion of Level 2 Validations, outcome is {0}", GetLevel2ValidationStatus());

                    if (GetLevel2ValidationStatus() == true && level2ValidationResult == true)
                    {
                        /* Update IS-File Log Details indicating that Phase-1 i.e. - Validation Level-2 is completed */
                        invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "VALIDATION_END", null, null, null, 0, 0, 0, 0, 0);

                        /* Since both Level 1 and Level 2 Validations are Successful, Load Data in Database */
                        if (GetLevel1ValidationStatus() && GetLevel2ValidationStatus())
                        {
                            /* Update IS-File Log Details indicating that Phase-1 i.e. - Loading is Started */
                            invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "LOADER_START", null, null, null, 0, 0, 0, 0, 0);

                            /* Load Data */
                            Logger.Info("Beginning of Level 3 - Loading.");
                            bool dataLoadingResult = LoadMemberProfileCsvData();
                            Logger.InfoFormat("Completion of Level 3 - Loading, outcome is {0}", dataLoadingResult);

                            if (dataLoadingResult == true)
                            {
                                /* Update IS-File Log Details indicating that Phase-1 i.e. - Loading is completed */
                                invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "LOADER_END", null, null, null, 0, 0, 0, 0, 0);

                                /* Data Loading is Successful */
                                if (PerformAfterProcessingCleanUp())
                                {
                                    /* After Processing Cleanup is Successful */

                                    if (UpdateFileStatus((int)FileStatusType.Successful))
                                    {
                                        /* Update IS File Log Entry and File Location is Successful */
                                        Logger.Info("File Status Updated To Successful.");

                                        /* Send Success Email Notification */
                                        if (SendEmailNotification(0))
                                        {
                                            Logger.Info("Email Notification Sent Successfully.");
                                        }
                                        else
                                        {
                                            Logger.Info("Problem Sending Email Notification.");
                                        }
                                    }
                                    else
                                    {
                                        Logger.Info("Problem Updating File Status.");
                                    }
                                }
                                else
                                {
                                    Logger.Info("Problem Performing After Processing Cleanup");
                                }
                            }
                            else
                            {
                                Logger.Info("Problem Loading Data");

                                invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "LOADER_END", null, null, null, 0, 0, 0, 0, 0);

                                /* Send Problem Loading Email Notification */
                                CleanUpAndNotifyBeforeHaltingProcessing(3,
                                                                        MemberProfileCSVUploadConstants.
                                                                            LoadingFailureReasonText);
                            }
                        }
                    }
                    else
                    {
                        /* Update IS-File Log Details indicating that Phase-1 i.e. - Validation Level-2 is completed */
                        invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "VALIDATION_END", null, null, null, 0, 0, 0, 0, 0);

                        /* Stop Further Processing */
                        return false;
                    }
                }
                /* This is added just for logical completion - Return without doing any further Processing */
                else
                {
                    /* Update IS-File Log Details indicating that Phase-1 i.e. - Validation Level-1 (Sanity) is completed */
                    invRepository.PopulateFileProcessingStats(iSFileLogRecord.Id, "SANITY_END", null, null, null, 0, 0, 0, 0, 0);

                    /* Stop Further Processing */
                    return false;
                }
            }
            catch (Exception exception)
            {
                const string serviceName = MemberProfileCSVUploadConstants.ServiceName;
                Logger.InfoFormat("Exception occurred in {0}", MemberProfileCSVUploadConstants.ServiceName);
                Logger.Error(exception);
                broadcastMessagesManager.SendISAdminExceptionNotification(EmailTemplateId.ISAdminExceptionNotification,
                                                                          serviceName, exception, false);

                /* Decided that there will not be any retry attempt, so eating this exception */
                //throw;
            }

            return true;
        }

        private bool GetLevel1ValidationStatus()
        {
            return _level1ValidationStatus;
        }

        private void SetLevel1ValidationStatus(bool newStatus)
        {
            if (!newStatus)
            {
                _level1ValidationStatus = false;//newStatus
            }
        }

        private bool GetLevel2ValidationStatus()
        {
            return _level2ValidationStatus;
        }

        private void SetLevel2ValidationStatus(bool newStatus)
        {
            if (!newStatus)
            {
                _level2ValidationStatus = false;//newStatus
            }
        }

        private bool PerformLengthCheckOnFileName(string filename)
        {
            string fileNameWithExtension = Path.GetFileName(filename);
            if (fileNameWithExtension != null)
            {
                if (fileNameWithExtension.Length > MemberProfileCSVUploadConstants.MaxFileNameLength)
                {
                    /* validation failed */
                    return false;
                }
                else
                {
                    /* Validation Pass */
                    return true;
                }
            }
            else
            {
                /* validation failed */
                return false;
            }
        }

        public bool CleanUpAndNotifyBeforeHaltingProcessing(int level, string failureReasonText, string failedRowNumber = null,
            string failedField = null, string failedFieldValue = null)
        {
            try
            {
                #region Perform Cleanup and Update File Status To Failed

                if (PerformAfterProcessingCleanUp())
                {
                    Logger.Info("After Processing Cleanup is Successful");
                    Logger.Info("Updating File Status To Failure.");

                    if (UpdateFileStatus((int)FileStatusType.Failed))
                    {
                        Logger.Info("Successfully Updated File Status To Failure.");
                    }
                    else
                    {
                        Logger.Info("Problem Updating File Status.");
                    }
                }
                else
                {
                    Logger.Info("Problem invRepository After Processing Cleanup");
                }

                #endregion

                #region Send Email

                SendEmailNotification(level, failureReasonText, failedRowNumber, failedField, failedFieldValue);

                #endregion

                /* Terminating the Single Instance, in order to halt the program and restrict further processing of the file on first validation/loading failure. */
                Environment.Exit(0);
            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.InfoFormat("Exception occurred in UpdateFileStatusAndSendNotificationEmail() ");
                Logger.Error(exception);
                //throw;
                return false;
            }

            /* Everything is successful */
            return true;
        }

        private bool SendEmailNotification(int level, string failureReasonText = null, string failedRowNumber = null, string failedField = null, string failedFieldValue = null)
        {
            try
            {
                /* Format File Date and Time */
                string submissionDateTime = iSFileLogRecord.FileDate.ToString("dd-MMM-yyyy, HH:mm:ss");

                Logger.InfoFormat(
                    "Sending Failure Email Notification. Validation Level = {0}, File Name = {1}, Submission Time = {2}, User Name = {3}, Reason For Failure = {4}, Row No = {5}, Column Name = {6}, Field Value = {7} ",
                    level, iSFileLogRecord.FileName,
                    submissionDateTime, userName,
                    failureReasonText, failedRowNumber,
                    failedField, failedFieldValue);

                /* Send Email */
                broadcastMessagesManager.SendMemberProfileCsvUploadEmailNotification(level, iSFileLogRecord.FileName,
                                                                                     submissionDateTime, userName,
                                                                                     failureReasonText, failedRowNumber,
                                                                                     failedField, failedFieldValue);
            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.InfoFormat("Exception occurred in SendEmailNotification() ");
                Logger.Error(exception);
                //throw;
                return false;
            }

            /* Everything is successful */
            return true;
        }

        private bool UpdateFileStatus(int fileStatusId)
        {
            try
            {
                iSFileLogRecord.FileStatusId = fileStatusId;
                iSFileLogRecord.FileLocation = Path.GetDirectoryName(destinationFilePath);

                iSFileLogRecord = referenceManager.UpdateIsInputFile(iSFileLogRecord);
            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.InfoFormat("Exception occurred in UpdateFileStatus() ");
                Logger.Error(exception);
                //throw;
                return false;
            }

            /* Everything is successful */
            return true;
        }

        private bool PerformAfterProcessingCleanUp()
        {
            try
            {
                /* Move File To Processed Directory Path */

                /* 1. Build Source File path */
                string sourceFilePath = Path.Combine(iSFileLogRecord.FileLocation,
                                                     iSFileLogRecord.FileName);

                Logger.InfoFormat("Source File Path is: [{0}]", sourceFilePath);

                /* 2.Destination File Path */
                destinationFilePath = BuildDestinationPath();
                if(string.IsNullOrWhiteSpace(destinationFilePath))
                {
                    Logger.Info("Destination File Path is: null");
                    return false;
                }

                Logger.InfoFormat("Destination File Path is: [{0}]", destinationFilePath);

                if (File.Exists(destinationFilePath))
                {
                    Logger.InfoFormat("Destination File is Already Existing So deleting it Path is: [{0}]", destinationFilePath);
                    File.Delete(destinationFilePath);
                    Logger.InfoFormat("Already Existing Destination File Deleted Path is: [{0}]", destinationFilePath);
                }
                
                /* Move File */
                bool isFileMovedSuccessfully = FileIo.MoveFile(sourceFilePath, destinationFilePath);
                if (isFileMovedSuccessfully == true)
                {
                    Logger.Info("File Moved Successfully");

                    Logger.Info("Deleting Source File along with Temp Directory Used For Decompression");

                    //Move delete Source File so commenting below code 
                    ///* Delete File From Source Path */
                    //File.Delete(sourceFilePath);

                    /* Delete decompressedDirectoryPath - Used Temporarily for Decompression */
                    Logger.InfoFormat("Decompressed Directory Path is: [{0}]", decompressedDirectoryPath);
                    if (!string.IsNullOrWhiteSpace(decompressedDirectoryPath))
                    {
                        if(Directory.Exists(decompressedDirectoryPath))
                        {
                            Logger.InfoFormat("Decompressed Directory Found So Now Deleting it Path is: [{0}]", decompressedDirectoryPath);
                            Directory.Delete(decompressedDirectoryPath, true);
                            Logger.Info("Decompressed Directory Deleted");
                        }  
                    }

                    Logger.Info("Source File+Directory Cleanup Completed Successfully");

                }
                else
                {
                    /* Problem in File Movement */
                    Logger.Info("Problem in File Movement");
                    return false;
                }
            }
            catch (Exception exception)
            {
                const string serviceName = MemberProfileCSVUploadConstants.ServiceName;
                Logger.Info("Exception occurred in PerformAfterProcessingCleanUp()");
                Logger.Error(exception);

                return false;
            }

            return true;
        }

        private string BuildDestinationPath()
        {
            try
            {
                /* E.g. Path - <ROOT SAN PATH>\SFRRoot\Processed\InputFiles\ */
                destinationFilePath = FileIo.GetForlderPath(SFRFolderPath.ProcessedInputPath);

                /* E.g. Path - 20150218 */
                string dateFolder = string.Format(@"{0}{1}{2}", iSFileLogRecord.FileDate.Year.ToString().PadLeft(4, '0'),
                          iSFileLogRecord.FileDate.Month.ToString().PadLeft(2, '0'),
                          iSFileLogRecord.FileDate.Day.ToString().PadLeft(2, '0'));

                /* E.g. Path - <ROOT SAN PATH>\SFRRoot\Processed\InputFiles\20150218\ */
                destinationFilePath = Path.Combine(destinationFilePath, dateFolder);

                /* E.g. Path - <ROOT SAN PATH>\SFRRoot\Processed\InputFiles\20150218\MemberCSVUpload */
                destinationFilePath = Path.Combine(destinationFilePath, "MemberCSVUpload");

                /* E.g. Path - <ROOT SAN PATH>\SFRRoot\Processed\InputFiles\20150218\MemberCSVUpload\CMP608-1.zip */
                destinationFilePath = Path.Combine(destinationFilePath, iSFileLogRecord.FileName);
            }
            catch (Exception exception)
            {
                const string serviceName = MemberProfileCSVUploadConstants.ServiceName;
                Logger.Info("Exception occurred in BuildDestinationPath()");
                Logger.Error(exception);
                destinationFilePath = null;

                /* Question - Decided that there will not be any retry attempt, so eating this exception */
                //throw;
            }
            return destinationFilePath;
        }

        #endregion

        #region Level 1 Processing of File

        private bool PerformFirstLevelValidations()
        {
            try
            {
                #region Validation 1 - Duplicate file check

                try
                {
                    if (!string.IsNullOrWhiteSpace((iSFileLogRecord.FileName)))
                    {
                        List<IsInputFile> existingFiles = referenceManager.GetAllIsInputFile(iSFileLogRecord.FileName);

                        if (existingFiles != null && existingFiles.Count > 1)
                        {
                            /* Set Level 1 Validation Status */
                            SetLevel1ValidationStatus(false);

                            /* Another File with same name exists and so its a problem */
                            CleanUpAndNotifyBeforeHaltingProcessing(1, Messages.ResourceManager.GetString(ErrorCodes.L1Dot1));
                            

                            /* Stop Further Processing */
                            return false;
                        }
                        /* Validation Pass - Not a duplicate file. This is added just for logical completion */
                        //else
                        //{
                        //}
                    }
                    /* This is unexpected and added just for logical completion */
                    //else
                    //{
                    //    return false;
                    //}
                }
                catch (Exception exception)
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    /* Log it before throwing. But surely throw and allow calling function to handle it. */
                    Logger.Error("exception occurred in PerformFirstLevelValidations() | Validation 1 - Duplicate file check, Details - ", exception);

                    /* Stop Further Processing */
                    return false;
                }

                #endregion

                #region Validation 2 - Decompression

                try
                {
                    /* Decompression Path */
                    decompressedDirectoryPath = Guid.NewGuid().ToString();
                    decompressedDirectoryPath = Path.Combine(iSFileLogRecord.FileLocation,
                                                                     "WorkingDirectory_" + decompressedDirectoryPath);
                    var zipfilePath = Path.Combine(iSFileLogRecord.FileLocation, iSFileLogRecord.FileName);
                    bool isDecompressSuccessful = FileIo.ExtractFilesFromZip(zipfilePath, decompressedDirectoryPath);

                    if (!isDecompressSuccessful)
                    {
                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        /* Problem in unzipping the file */
                        CleanUpAndNotifyBeforeHaltingProcessing(1, Messages.ResourceManager.GetString(
                                                                         ErrorCodes.L1Dot2));

                        /* Stop Further Processing */
                        return false;
                    }
                    /* Valid Case - Decompression Successful. This is added just for logical completion 
                     * Unzipped files are now in unzipDirectoryPath */
                    //else
                    //{
                    //}
                }
                catch (Exception exception)
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    /* Log it before throwing. But surely throw and allow calling function to handle it. */
                    Logger.Error("exception occurred in PerformFirstLevelValidations() | Validation 2 - Decompression, Details - ", exception);

                    /* Stop Further Processing */
                    return false;
                }

                #endregion

                #region Validation 3 and 3a - Availability of Exactly One CSV file post decompression & Validation 4 - Length of filename
                string memberUploadedCSVFile = string.Empty;
                try
                {
                    int csvFilesFoundAfterDecompression = 0;
                    csvFilesFoundAfterDecompression = Directory.GetFiles(decompressedDirectoryPath, "*.*", SearchOption.TopDirectoryOnly).Where(
                        decompressedFileName => decompressedFileName.ToUpper().EndsWith(".CSV")).Count();

                    /* No CSV File Found After Unzip */
                    if (csvFilesFoundAfterDecompression <= 0)
                    {
                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        /* No CSV file found after decompression */
                        CleanUpAndNotifyBeforeHaltingProcessing(1, Messages.ResourceManager.GetString(
                                                                         ErrorCodes.L1Dot3));

                        /* Stop Further Processing */
                        return false;
                    }

                    /* Multiple CSV File Found After Unzip */
                    if (csvFilesFoundAfterDecompression > 1)
                    {
                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        /* No CSV file found after decompression */
                        CleanUpAndNotifyBeforeHaltingProcessing(1, Messages.ResourceManager.GetString(
                                                                         ErrorCodes.L1Dot3a));

                        /* Stop Further Processing */
                        return false;
                    }

                    try
                    {
                        #region CSV File Length Check
                        memberUploadedCSVFile = Directory.GetFiles(decompressedDirectoryPath, "*.*", SearchOption.TopDirectoryOnly).Where(
                                                        decompressedFileName => decompressedFileName.ToUpper().EndsWith(".CSV")).First();

                        /* 4. Length of filename of CSV file  - Validation common for IS-WEB and Service */
                        if (!PerformLengthCheckOnFileName(memberUploadedCSVFile))
                        {
                            /* Set Level 1 Validation Status */
                            SetLevel1ValidationStatus(false);

                            /* File name is too long and so its a problem */
                            CleanUpAndNotifyBeforeHaltingProcessing(1, Messages.ResourceManager.GetString(
                                                                             ErrorCodes.L1Dot4));

                            /* Stop Further Processing */
                            return false;
                        }
                        #endregion
                    }
                    catch (Exception exception)
                    {
                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        /* Log it before throwing. But surely throw and allow calling function to handle it. */
                        Logger.Error("exception occurred in PerformFirstLevelValidations() | Validation 4 - Length of filename, Details - ", exception);

                        /* Stop Further Processing */
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    /* Log it before throwing. But surely throw and allow calling function to handle it. */
                    Logger.Error("exception occurred in PerformFirstLevelValidations() | Validation 3 - Availability of a CSV file post decompression, Details - ", exception);

                    /* Stop Further Processing */
                    return false;
                }

                #endregion

                #region Validation 5 - Availability of data in CSV file and Validation 6 - Structural correctness of CSV file

                try
                {
                    ReadCsvFileAndValidateCsvData(memberUploadedCSVFile);
                }
                catch (Exception exception)
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    /* Log it before throwing. But surely throw and allow calling function to handle it. */
                    Logger.Error("exception occurred in PerformFirstLevelValidations() | " +
                    "Validation 5 - Availability of data in CSV file and Validation 6 - Structural correctness of CSV file, Details - ",
                        exception);

                    /* Stop Further Processing */
                    return false;
                }

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 1 Validation Status */
                SetLevel1ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in PerformFirstLevelValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            return GetLevel1ValidationStatus();
        }

        private bool ReadCsvFileAndValidateCsvData(string fileName)
        {
            try
            {
                /* Default Value False, indicating no data row existing in Csv File */
                bool isDataRowFound = false;

                /* Initialize data structure to hold CSV Data */
                _csvFileData = new Dictionary<int, List<string>>();

                /* Using Stream Reader, read CSV File line by line */
                using (var reader = new StreamReader(fileName))
                {
                    int rowNumber = 1;
                    string row;
                    while ((row = reader.ReadLine()) != null)
                    {
                        if (rowNumber == 1)
                        {
                            /* Skipping first row - Expected to be a header row and hence to be discarded */
                            rowNumber++;
                            continue;
                        }

                        /* Adding a delimiter at the end of line read */
                        row = row + ",";

                        isDataRowFound = true;
                        var columns = ExtractData(row);
                      if (columns.Count != MemberProfileCSVUploadConstants.ExpectedNumberOfColumnsInCsvFile)
                      {
                        #region Validation 6 - Structural correctness of CSV file

                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        reader.Dispose();
                        /* Problem with this row - Mismatch between actual and Expected (40) number of columns */
                        CleanUpAndNotifyBeforeHaltingProcessing(1, (Messages.ResourceManager.GetString(
                          ErrorCodes.L1Dot6)) + rowNumber.ToString());

                        /* Stop Further Processing */
                        return false;

                        #endregion
                      }
                      else
                      {
                        /* Perfect row - No Column Mismatch */
                        /* Preserve complete Row with its number as key and string[] of columns/fields as value */
                        _csvFileData.Add(rowNumber, columns.ToList());
                        rowNumber++;
                      }
                    }
                }

                #region Validation 5 - Availability of data in CSV file

                if (!isDataRowFound)
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    /* No Data Row Found in Csv File */
                    CleanUpAndNotifyBeforeHaltingProcessing(1, Messages.ResourceManager.GetString(
                                                                         ErrorCodes.L1Dot5));

                    /* Stop Further Processing */
                    return false;
                }

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 1 Validation Status */
                SetLevel1ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ReadCsvFileAndValidateCsvData() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Processing Completed Successfully */
            return true;
        }

        public bool PerformFirstLevelValidationsForISWEB(string filename)
        {
            try
            {
                #region 1. No file chosen

                if (string.IsNullOrWhiteSpace(filename))
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    Logger.Info("Throwing Business Exception - " + ErrorCodes.NoFileChosen);
                    /* Throw business exception - Error message - No file chosen */
                    throw new ISBusinessException(ErrorCodes.NoFileChosen);

                }
                /* Validation Pass - Added just for logical completion */
                //else
                //{
                //    return true;
                //}

                #endregion

                #region 2. User selected file has an extension other than ‘.zip’

                var fileExtension = Path.GetExtension(filename);
                if (string.IsNullOrWhiteSpace(fileExtension) || !fileExtension.Equals(".Zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    Logger.Info("Throwing Business Exception - " + ErrorCodes.FileChosenIsNotZIP);

                    /* Throw business exception - Error message - Please choose a file in compressed format with a .zip extension */
                    throw new ISBusinessException(ErrorCodes.FileChosenIsNotZIP);
                }
                /* Validation Pass - Added just for logical completion */
                //else
                //{
                //    return true;
                //}

                #endregion

                #region 3. Length of filename of CSV file

                /* Applying Length Check on ZIP File Name */
                if (!PerformLengthCheckOnFileName(filename))
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    Logger.Info("Throwing Business Exception - " + ErrorCodes.InvalidFileNameLength);

                    /* Throw business exception - Error message - File name is too long. The maximum permissible length is 50 including the extension */
                    throw new ISBusinessException(ErrorCodes.InvalidFileNameLength);
                }
                /* Validation Pass - Added just for logical completion */
                //else
                //{
                //    return true;
                //}

                /* Peep into the Uploaded ZIP File (Without Decompressing) to validate Length Check */
                var ListOfFilesInsideZip = FileIo.GetInsideFileNamesWithoutDecompressingZip(filename);
                if (ListOfFilesInsideZip != null)
                {
                    if (ListOfFilesInsideZip.Count != 1)
                    {
                        /* #Files = 0 => invalid Zip file */
                        /* #Files > 1 => Multiple Files inside zip */
                        /* In any case this is not acceptable */
                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        Logger.Info("Throwing Business Exception - " + ErrorCodes.L1Dot3a);

                        throw new ISBusinessException(ErrorCodes.L1Dot3a);
                    }

                    /* Exact 1 file found inside zip, so applying length check on it. */
                    var nameOfFirstFileInsideZip = ListOfFilesInsideZip.FirstOrDefault();

                    var resultOfLengthCheck = PerformLengthCheckOnFileName(nameOfFirstFileInsideZip);
                    if (!resultOfLengthCheck)
                    {
                        /* Set Level 1 Validation Status */
                        SetLevel1ValidationStatus(false);

                        Logger.Info("Throwing Business Exception - " + ErrorCodes.InvalidUnzipFileNameLength);

                        /* Throw business exception - Error message - File name is too long. The maximum permissible length is 50 including the extension */
                        throw new ISBusinessException(ErrorCodes.InvalidUnzipFileNameLength);
                    }
                    /* Validation Pass - Added just for logical completion */
                    //else
                    //{
                    //    return true;
                    //}
                }
                /* No file found inside zip / problem getting file inside zip without decompression - Code Should Continue? */
                else
                {
                    /* Set Level 1 Validation Status */
                    SetLevel1ValidationStatus(false);

                    Logger.Info("Throwing Business Exception - " + ErrorCodes.L1Dot2);

                    throw new ISBusinessException(ErrorCodes.L1Dot2);
                }

                #endregion

            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in PerformFirstLevelValidationsForISWEB() ");
                Logger.Error(exception);
                throw;
            }

            return GetLevel1ValidationStatus();
        }

        /// <summary>
        /// To extract data from CSV row.
        /// </summary>
        /// <param name="valueToSplit">string with delimiter comma</param>
        /// <param name="delimiter">delimiter to seperate columns</param>
        /// <param name="escapeChar">double code to exclude</param>
        /// <param name="strictEscapeToSplitEvaluation"></param>
        /// <param name="captureEndingNull"></param>
        /// <returns></returns>
        public List<string> ExtractData(string valueToSplit, char delimiter = ',', char escapeChar = '"',
                bool strictEscapeToSplitEvaluation = true, bool captureEndingNull = false)
        {
            var list = new List<string>();
            var stringBuilder = new StringBuilder();

            bool bInEscapeVal = false;

            for (int i = 0; i < valueToSplit.Length; i++)
            {
                if (!bInEscapeVal)
                {
                    // Escape values must come immediately after a split.
                    // abc,"b,ca",cab has an escaped comma.
                    // abc,b"ca,c"ab does not.
                    if (escapeChar == valueToSplit[i] &&
                        (!strictEscapeToSplitEvaluation || (i == 0 || (i != 0 && delimiter == valueToSplit[i - 1]))))
                    {
                        bInEscapeVal = true;
                    }
                    else if (delimiter == valueToSplit[i])
                    {
                        list.Add(stringBuilder.ToString());
                        stringBuilder = new StringBuilder();
                    }
                    else
                    {
                        stringBuilder.Append(valueToSplit[i]);
                    }
                }
                else
                {
                    // Can't use switch b/c we're comparing to a variable, I believe.
                    if (escapeChar == valueToSplit[i])
                    {
                        // Repeated escape always reduces to one escape char in this logic.
                        // So if you wanted "I'm ""double quote"" crazy!" to come out with 
                        // the double double quotes, you're toast.
                        if (i + 1 < valueToSplit.Length && escapeChar == valueToSplit[i + 1])
                        {
                            i++;
                            stringBuilder.Append(escapeChar);
                        }
                        else if (!strictEscapeToSplitEvaluation)
                        {
                            bInEscapeVal = false;
                        }

                        else if ('"' == escapeChar && i + 2 < valueToSplit.Length &&
                                 valueToSplit[i + 1] == ',' && valueToSplit[i + 2] == ' ')
                        {
                            i = i + 2;
                            stringBuilder.Append("\", ");
                        }

                        else if (i + 1 == valueToSplit.Length || (i + 1 < valueToSplit.Length && valueToSplit[i + 1] == delimiter))
                        {
                            bInEscapeVal = false;
                        }
                        else
                        {
                            stringBuilder.Append(escapeChar);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(valueToSplit[i]);
                    }
                }
            }


            if ((captureEndingNull && delimiter == valueToSplit[valueToSplit.Length]) || (stringBuilder.Length > 0))
            {
                list.Add(stringBuilder.ToString());
            }

            return list;
        }

        #endregion

        #region Level 2 Processing of File

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool PerformSecondLevelValidations()
        {
            try
            {
                /* Now Here - 
                 * 1. Data is available in _csvFileData. (already structurally Verified in Level 1) */

                /* Iterate over each Data Row and Validate every Column/Field in it */
                for (int csvDataKey = 2; csvDataKey <= _csvFileData.Count + 1; csvDataKey++)
                {
                    List<string> dataRow;
                    if (_csvFileData.TryGetValue(csvDataKey, out dataRow))
                    {
                        /* Expected Key is Found. Value of this Key represents a complete Row */
                        /* Validate Complete Data Row */
                        MemberProfileData memberProfileData = new MemberProfileData();
                        ValidateDataRow(csvDataKey, dataRow, ref memberProfileData);
                        if (!GetLevel2ValidationStatus())
                        {
                            /* Problem in Row Validation, So Abort Further Processing */
                            Logger.InfoFormat("Problem Validating Row number {0} in CSV File {1}", csvDataKey, iSFileLogRecord.FileName);

                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            /* Stop Further Processing */
                            return false;
                        }
                        else
                        {
                            /* All Columns in Row are validated Successfully */
                            _memberProfiles.Add(memberProfileData);
                        }
                    }
                    else
                    {
                        /* Problem While Parsing and Storing the CSV Data. */
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        /* Stop Further Processing */
                        return false;
                    }
                }
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in PerformSecondLevelValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Processing Completed Successfully */
            return true;
        }

        #region Level 2 Validation Helper Methods

        private void ValidateDataRow(int rowNumber, List<string> dataRow, ref MemberProfileData memberProfileData)
        {
            try
            {
                for (int columnNumber = 0; columnNumber < MemberProfileCSVUploadConstants.ExpectedNumberOfColumnsInCsvFile; columnNumber++)
                {
                    /* Expected order of columns is well know (as per FRS) */

                    /* Column Data */
                    string columnData = dataRow[columnNumber];

                    /* Apply Validations on Column */
                    ValidateColumnData(rowNumber, columnNumber, columnData, ref memberProfileData);

                    if (!GetLevel2ValidationStatus())
                    {
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        /* Problem in Row Validation, So Abort Further Processing */
                        Logger.InfoFormat("Problem Validating Row number {0} Column number {1} in CSV File {2}", rowNumber, columnNumber, iSFileLogRecord.FileName);

                        /* Problem in Column Validation, So Abort Further Processing */
                        //break;
                        return;
                    }
                }
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ValidateDataRow() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                throw;
            }

            /* Processing Completed Successfully */
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowNumber">Row number within CSV file, after considering the header/first row of the file to be row number 1.</param>
        /// <param name="columnNumber">0 based column number.</param>
        /// <param name="columnData"></param>
        private void ValidateColumnData(int rowNumber, int columnNumber, string columnData, ref MemberProfileData memberProfileData)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    /* Ref FRS#608: Section 3.2.4 Point 48
                     * Desc: Before performing validations on any field, it should be trimmed, 
                     * i.e. the leading and trailing spaces should be removed (but not the spaces within the value) */
                    columnData = columnData.Trim();
                }
                
                /* Generic Field Validation Attributes Default Value */
                string fieldName = string.Empty;
                CsvColumnDataType dataType = CsvColumnDataType.None;
                int minimumLength = 0;
                int maximumLength = 0;
                bool isMandatory = false;
                int dependentColumnNumber = 0;

                GetGenericValidationAttributeValuesForColumn(columnNumber, ref columnData, ref fieldName, ref dataType,
                                                             ref minimumLength, ref maximumLength, ref isMandatory, ref dependentColumnNumber, ref memberProfileData);

                Logger.InfoFormat("Data Validation Started For Row: {0} Column Name: {1}", rowNumber, fieldName);

                Logger.InfoFormat("Starting Generic Field Data Validations.");

                ApplyGenericFieldDataValidation(rowNumber, columnData, fieldName, dataType, minimumLength, maximumLength,
                                                isMandatory);

                Logger.InfoFormat("Completion of Generic Field Data Validations.");

                /* Only When Generic Field Validations are Successful apply Field Specific Validations (if Any) */
                if (!GetLevel2ValidationStatus())
                {
                    return;
                }

                switch (columnNumber)
                {
                    case 0:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        MemberCodeNumericFieldValidations(rowNumber, columnNumber, columnData, fieldName);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 4:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        IsMembershipStatusFieldValidations(rowNumber, columnData, fieldName, ref memberProfileData);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 5:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        IsMembershipSubStatusFieldValidations(rowNumber, columnData, fieldName, ref memberProfileData);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 15:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        CountryCodeFieldValidations(rowNumber, columnData, fieldName);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    /*CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3*/
                    case 29:
                         Logger.InfoFormat("Start of Field 'MISCDailyPayableOptionsForBilateralInvoices' Specific Validations.");
                         MiscDailyPayableOptionsValidation(rowNumber, columnData, fieldName);
                        Logger.InfoFormat("Completion of Field 'MISCDailyPayableOptionsForBilateralInvoices' Specific Validations.");
                        break;
                    case 16:
                    case 17:
                    case 21:
                    case 28:
                    case 30:
                    case 31:
                    case 32:
                    case 33:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        BooleanFieldValidations(rowNumber, columnData, fieldName, isMandatory);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 18:
                    case 19:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        InvoicesDSToBeAppliedForFieldValidations(rowNumber, columnData, fieldName, dependentColumnNumber);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 22:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        CDCCompartmentIDForInvFieldValidations(rowNumber, columnData, fieldName, dependentColumnNumber);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 23:
                    case 24:
                    case 25:
                    case 26:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        PayAndRecLAAndListingRelatedFieldValidations(rowNumber, columnData, fieldName, dependentColumnNumber, isMandatory);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 27:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        MISCAllowedFileTypesForSupportingDocumentsFieldValidations(rowNumber, columnData, fieldName);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 35:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        NewUserEmailAddressFieldValidations(rowNumber, columnNumber, columnData, fieldName, dependentColumnNumber);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    case 36:
                    case 37:
                    case 38:
                    case 39:
                        Logger.InfoFormat("Start of Field Specific Validations.");
                        NewUserFieldValidations(rowNumber, columnNumber, columnData, fieldName, dependentColumnNumber);
                        Logger.InfoFormat("Completion of Field Specific Validations.");
                        break;

                    default:
                        Logger.InfoFormat("No Field Specific Validations Exists For Field: {0}.", fieldName);
                        break;
                }


            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ValidateColumnData() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                throw;
            }
        }

        #region Generic Field Data Validations

        private void GetGenericValidationAttributeValuesForColumn(int columnNumber, ref string columnData, ref string fieldName, ref CsvColumnDataType dataType,
                                                             ref int minimumLength, ref int maximumLength, ref bool isMandatory, ref int dependentColumnNumber, ref MemberProfileData memberProfileData)
        {
            if (columnNumber >= MemberProfileCSVUploadConstants.ExpectedNumberOfColumnsInCsvFile)
            {
                return;
            }

            /* Default Value for Dependent Column Number */
            dependentColumnNumber = -1;
            switch (columnNumber)
            {
                case 0:
                    fieldName = "MemberCodeNumeric";
                    dataType = CsvColumnDataType.ANA;
                    minimumLength = 3;
                    maximumLength = 12;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MemberCodeNumeric = columnData.ToUpper().Trim();
                    }
                    break;

                case 1:
                    fieldName = "MemberCodeAlpha";
                    dataType = CsvColumnDataType.ANA;
                    minimumLength = 2;
                    maximumLength = 2;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MemberCodeAlpha = columnData.ToUpper().Trim();
                    }
                    break;

                case 2:
                    fieldName = "LegalName";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = true;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.LegalName = columnData;
                    break;

                case 3:
                    fieldName = "CommercialName";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = true;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.CommercialName = columnData;
                    break;

                case 4:
                    fieldName = "ISMembershipStatus";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = true;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.ISMembershipStatus = columnData;
                    break;

                case 5:
                    fieldName = "ISMembershipSubStatus";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 255;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.ISMembershipSubStatus = columnData.ToUpper().Trim();
                    }
                    break;

                case 6:
                    fieldName = "RegistrationID";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 25;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.RegistrationID = columnData;
                    break;

                case 7:
                    fieldName = "TaxVatRegistrationNumber";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 25;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.TaxVatRegistrationNumber = columnData;
                    break;

                case 8:
                    fieldName = "AddTaxVatRegistrationNumber";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 25;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.AddTaxVatRegistrationNumber = columnData;
                    break;

                case 9:
                    fieldName = "AddressLine1";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 70;
                    isMandatory = true;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.AddressLine1 = columnData;
                    break;

                case 10:
                    fieldName = "AddressLine2";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 70;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.AddressLine2 = columnData;
                    break;

                case 11:
                    fieldName = "AddressLine3";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 70;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.AddressLine3 = columnData;
                    break;

                case 12:
                    fieldName = "CityName";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 50;
                    isMandatory = true;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.CityName = columnData;
                    break;

                case 13:
                    fieldName = "SubDivisionName";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 50;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.SubDivisionName = columnData;
                    break;

                case 14:
                    fieldName = "PostalCode";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 50;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.PostalCode = columnData;
                    break;

                case 15:
                    fieldName = "CountryCode";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 2;
                    maximumLength = 2;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.CountryCode = columnData.ToUpper().Trim();
                    }
                    break;

                case 16:
                    fieldName = "DigitalSignApplication";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.DigitalSignApplication = columnData.ToUpper().Trim();
                    }
                    break;

                case 17:
                    fieldName = "DigitalSignVerification";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.DigitalSignVerification = columnData.ToUpper().Trim();
                    }
                    break;

                case 18:
                    fieldName = "ReceivablesInvoicesDSToBeAppliedFor";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 2;
                    maximumLength = 2000;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.ReceivablesInvoicesDSToBeAppliedFor = columnData.ToUpper().Trim();
                    }
                    dependentColumnNumber = 16;
                    break;

                case 19:
                    fieldName = "PayablesInvoicesDSToBeAppliedFor";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 2;
                    maximumLength = 2000;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.PayablesInvoicesDSToBeAppliedFor = columnData.ToUpper().Trim();
                    }
                    dependentColumnNumber = 16;
                    break;

                case 20:
                    fieldName = "DefaultInvoiceFooterText";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 0;
                    maximumLength = 700;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.DefaultInvoiceFooterText = columnData;
                    break;

                case 21:
                    fieldName = "LegalArchivingRequired";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = true;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.LegalArchivingRequired = columnData.ToUpper().Trim();
                    }
                    break;

                case 22:
                    fieldName = "CDCCompartmentIDForInv";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = false;
                    dependentColumnNumber = 21;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.CDCCompartmentIDForInv = columnData;
                    break;

                case 23:
                    fieldName = "LegalArchRequiredForMISCRecInv";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.LegalArchRequiredForMISCRecInv = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.LegalArchRequiredForMISCRecInv = MemberProfileCSVUploadConstants.No;
                    }
                    dependentColumnNumber = 21;
                    break;

                case 24:
                    fieldName = "IncludeListingsMISCRecArch";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.IncludeListingsMISCRecArch = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.IncludeListingsMISCRecArch = MemberProfileCSVUploadConstants.No;
                    }
                    dependentColumnNumber = 21;
                    break;

                case 25:
                    fieldName = "LegalArchRequiredForMISCPayInv";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.LegalArchRequiredForMISCPayInv = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.LegalArchRequiredForMISCPayInv = MemberProfileCSVUploadConstants.No;
                    }
                    dependentColumnNumber = 21;
                    break;

                case 26:
                    fieldName = "IncludeListingsMISCPayArch";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.IncludeListingsMISCPayArch = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.IncludeListingsMISCPayArch = MemberProfileCSVUploadConstants.No;
                    }
                    dependentColumnNumber = 21;
                    break;

                case 27:
                    fieldName = "MISCAllowedFileTypesForSupportingDocuments";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 500;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.MISCAllowedFileTypesForSupportingDocuments = columnData;
                    break;

                case 28:
                    fieldName = "MISCBilledInvoiceXMLOutput";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCBilledInvoiceXMLOutput = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCBilledInvoiceXMLOutput = MemberProfileCSVUploadConstants.No;
                    }
                    break;

                case 29:
                    fieldName = "MISCDailyPayableOptionsForBilateralInvoices";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 3;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();

                        /* CMP-619-652-682:  section 2.4 Loading of Member Profile Data using CSV Files
                         During loading values should be interpreted as follows:
                            If N, then:
                            ‘Daily Delivery in IS-WEB’ = No/False
                            ‘Daily Offline Archive Outputs’ = No/False
                            ‘Daily IS-XML Files’ = No/False
                            If W, then:
                            ‘Daily Delivery in IS-WEB’ = Yes/True
                            ‘Daily Offline Archive Outputs’ = No/False
                            ‘Daily IS-XML Files’ = No/False
                            If WO, then:
                            ‘Daily Delivery in IS-WEB’ = Yes/True
                            ‘Daily Offline Archive Outputs’ = Yes/True
                            ‘Daily IS-XML Files’ = No/False
                            If WX, then:
                            ‘Daily Delivery in IS-WEB’ = Yes/True
                            ‘Daily Offline Archive Outputs’ = No/False
                            ‘Daily IS-XML Files’ = Yes/True
                            If WOX, then:
                            ‘Daily Delivery in IS-WEB’ = Yes/True
                            ‘Daily Offline Archive Outputs’ = Yes/True
                            ‘Daily IS-XML Files’ = Yes/True
                         */
                        switch (columnData)
                        {
                            case "N":
                                memberProfileData.MiscDailyPayableOptionsForBilateralInvoices = MemberProfileCSVUploadConstants.No;
                                memberProfileData.MiscDailyPayableOfflineAchiveOutputs = MemberProfileCSVUploadConstants.No;
                                memberProfileData.MiscDailyPayableIsXmlFiles = MemberProfileCSVUploadConstants.No;
                                break;
                            case "W":
                                memberProfileData.MiscDailyPayableOptionsForBilateralInvoices = MemberProfileCSVUploadConstants.Yes;
                                memberProfileData.MiscDailyPayableOfflineAchiveOutputs = MemberProfileCSVUploadConstants.No;
                                memberProfileData.MiscDailyPayableIsXmlFiles = MemberProfileCSVUploadConstants.No;
                                break;
                            case "WO":
                                memberProfileData.MiscDailyPayableOptionsForBilateralInvoices = MemberProfileCSVUploadConstants.Yes;
                                memberProfileData.MiscDailyPayableOfflineAchiveOutputs = MemberProfileCSVUploadConstants.Yes;
                                memberProfileData.MiscDailyPayableIsXmlFiles = MemberProfileCSVUploadConstants.No;
                                break;
                            case "WX":
                                memberProfileData.MiscDailyPayableOptionsForBilateralInvoices = MemberProfileCSVUploadConstants.Yes;
                                memberProfileData.MiscDailyPayableOfflineAchiveOutputs = MemberProfileCSVUploadConstants.No;
                                memberProfileData.MiscDailyPayableIsXmlFiles = MemberProfileCSVUploadConstants.Yes;
                                break;
                            case "WOX":
                                memberProfileData.MiscDailyPayableOptionsForBilateralInvoices = MemberProfileCSVUploadConstants.Yes;
                                memberProfileData.MiscDailyPayableOfflineAchiveOutputs = MemberProfileCSVUploadConstants.Yes;
                                memberProfileData.MiscDailyPayableIsXmlFiles = MemberProfileCSVUploadConstants.Yes;
                                break;
                        }
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MiscDailyPayableOptionsForBilateralInvoices = MemberProfileCSVUploadConstants.No;
                        memberProfileData.MiscDailyPayableOfflineAchiveOutputs = MemberProfileCSVUploadConstants.No;
                        memberProfileData.MiscDailyPayableIsXmlFiles = MemberProfileCSVUploadConstants.No;
                    }
                    break;

                case 30:
                    fieldName = "MISCIsPdfAsOtherOutputAsBilledEntity";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsPdfAsOtherOutputAsBilledEntity = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsPdfAsOtherOutputAsBilledEntity = MemberProfileCSVUploadConstants.No;
                    }
                    break;

                case 31:
                    fieldName = "MISCIsDetailListingAsOtherOutputAsBilledEntity";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsDetailListingAsOtherOutputAsBilledEntity = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsDetailListingAsOtherOutputAsBilledEntity = MemberProfileCSVUploadConstants.No;
                    }
                    break;

                case 32:
                    fieldName = "MISCIsSuppDocAsOtherOutputAsBilledEntity";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsSuppDocAsOtherOutputAsBilledEntity = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsSuppDocAsOtherOutputAsBilledEntity = MemberProfileCSVUploadConstants.No;
                    }
                    break;

                case 33:
                    fieldName = "MISCIsDSFileAsOtherOutputAsBilledEntity";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Validations */
                        columnData = columnData.ToUpper().Trim();
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsDSFileAsOtherOutputAsBilledEntity = columnData.ToUpper().Trim();
                    }
                    else
                    {
                        /* Default Value -  If value is not provided for this field, it should be assumed to be N */
                        columnData = MemberProfileCSVUploadConstants.No;
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.MISCIsDSFileAsOtherOutputAsBilledEntity = MemberProfileCSVUploadConstants.No;
                    }
                    break;

                case 34:
                    fieldName = "MISCIINetAccountId";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 50;
                    isMandatory = false;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.MISCIINetAccountId = columnData;
                    break;

                case 35:
                    fieldName = "NewUserEmailAddress";
                    dataType = CsvColumnDataType.ANC;
                    minimumLength = 1;
                    maximumLength = 250;
                    isMandatory = false;
                    dependentColumnNumber = 36;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.NewUserEmailAddress = columnData;
                    break;

                case 36:
                    fieldName = "NewUserFirstName";
                    dataType = CsvColumnDataType.AND;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = false;
                    dependentColumnNumber = 35;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.NewUserFirstName = columnData;
                    break;

                case 37:
                    fieldName = "NewUserLastName";
                    dataType = CsvColumnDataType.AND;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = false;
                    dependentColumnNumber = 35;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.NewUserLastName = columnData;
                    break;

                case 38:
                    fieldName = "NewUserType";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 1;
                    isMandatory = false;
                    dependentColumnNumber = 35;
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Need to Convert to Upper Case Before Loading */
                        memberProfileData.NewUserType = columnData.ToUpper().Trim();
                    }
                    break;

                case 39:
                    fieldName = "PermissionTemplateForNewUser";
                    dataType = CsvColumnDataType.ANB;
                    minimumLength = 1;
                    maximumLength = 100;
                    isMandatory = false;
                    dependentColumnNumber = 35;
                    /* Loading should be performed without any case conversion */
                    memberProfileData.PermissionTemplateForNewUser = columnData;
                    break;
            }
        }

        private void ApplyGenericFieldDataValidation(int rowNumber, string columnData, string fieldName,
            CsvColumnDataType dataType, int minimumLength, int maximumLength, bool isMandatory)
        {
            try
            {

                #region 1. Data Type Validation

                if (!IsDataTypeValid(columnData, dataType))
                {
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                        ErrorCodes.L2Dot1), rowNumber.ToString(), fieldName,
                                                             columnData);

                    /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                    Logger.InfoFormat("Data Type Validation Failed Column Data {0}, Expected Data Type {1} ", columnData, dataType);

                    /* Stop Further Processing */
                    return;
                }

                #endregion

                #region 2. Mandatory Validation

                if (!IsMandatoryValid(columnData, isMandatory))
                {
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                        ErrorCodes.L2Dot2), rowNumber.ToString(), fieldName,
                                                             columnData);

                    /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                    Logger.Info("Data Not Provided when Field is Mandatory");

                    /* Stop Further Processing */
                    return;
                }

                #endregion

                #region 3. Minimum Length Validation

                if (!IsMinimumLengthValid(columnData, minimumLength))
                {
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                        ErrorCodes.L2Dot3) + minimumLength, rowNumber.ToString(), fieldName,
                                                             columnData);

                    /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                    Logger.InfoFormat("Minimum Length Constraint Violated, Column Data: {0}, Expected Min Length: {1} ", columnData, minimumLength);

                    /* Stop Further Processing */
                    return;
                }

                #endregion

                #region 4. Maximum Length Validation

                if (!IsMaximumLengthValid(columnData, maximumLength))
                {
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                        ErrorCodes.L2Dot4) + maximumLength, rowNumber.ToString(), fieldName,
                                                             columnData);

                    /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                    Logger.InfoFormat("Maximum Length Constraint Violated, Column Data: {0}, Expected Min Length: {1} ", columnData, maximumLength);

                    /* Stop Further Processing */
                    return;
                }

                #endregion

            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ApplyGenericFieldDataValidation() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                throw;
            }
        }

        private bool IsDataTypeValid(string columnData, CsvColumnDataType columnDataType)
        {
            if (!string.IsNullOrWhiteSpace(columnData))
            {
                try
                {
                    switch (columnDataType)
                    {
                        case CsvColumnDataType.ANA:
                            /* Alpha-numeric, restricted to English letters a-z and A-Z and/or Digits 0-9 */
                            Regex Regex_ANA = new Regex("^[a-z,A-Z,0-9]*$");
                            return Regex_ANA.IsMatch(columnData);

                        case CsvColumnDataType.ANB:
                            /* Alpha-numeric, restricted to ASCII range 32-126, English letters a-z and A-Z Digits 0-9 Special characters including space */
                            /* RegEx Obtained From - http://www.catonmat.net/blog/my-favorite-regex/ */
                            Regex Regex_ANB = new Regex("^[ -~]*$");
                            return Regex_ANB.IsMatch(columnData);

                        case CsvColumnDataType.ANC:
                            /* Alpha-numeric without any restrictions, but excluding space */
                            Regex Regex_ANC = new Regex("^[^ ]*$");
                            return Regex_ANC.IsMatch(columnData);

                        case CsvColumnDataType.AND:
                            /* Alpha-numeric without any restrictions */
                            return true;

                        default:
                            return false;
                    }
                }
                catch (Exception exception)
                {
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                    Logger.Info("Exception occurred in IsDataTypeValid() ");
                    Logger.Error(exception);

                    /* Stop Further Processing */
                    return false;
                }
            }
            else
            {
                Logger.Info("Column Data Type Validation is Bypassed, since column Data is Null.");
            }

            /* Processing Completed Successfully */
            return true;
        }

        private bool IsMandatoryValid(string columnData, bool isMandatory)
        {
            try
            {
                if (isMandatory)
                {
                    if (string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Field is mandatory and null/empty - Not acceptable as input */
                        return false;
                    }
                    /* Code For better readability and logical completion 
                    else
                    {
                         Valid Input 
                    }
                    */
                }
                else
                {
                    /* In case of input not being mandatory validation is bypassed */
                    Logger.Info("Column Data mandatory Check Validation is Bypassed, since column Data is Not Mandatory.");
                }
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ValidateDataColumn_Mandatory() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Processing Completed Successfully */
            return true;
        }

        private bool IsMinimumLengthValid(string columnData, int permissibleMinimumLength)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    if (columnData.Length < permissibleMinimumLength)
                    {
                        /* Field length is not acceptable - Invalid Input */
                        return false;
                    }
                }
                /* For and logical completion */
                //else
                //{
                //     Valid Input 
                //}

            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in IsMinimumLengthValid() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Processing Completed Successfully */
            return true;
        }

        private bool IsMaximumLengthValid(string columnData, int permissibleMaximumLength)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    if (columnData.Length > permissibleMaximumLength)
                    {
                        /* Field is mandatory and length is not acceptable - Invalid Input */
                        return false;
                    }
                }
                /* For and logical completion */
                //else
                //{
                //     Valid Input 
                //}
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in IsMaximumLengthValid() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Processing Completed Successfully */
            return true;
        }

        private void BooleanFieldValidations(int rowNumber, string columnData, string fieldName, bool isMandatory)
        {
            try
            {
                #region Validation #F17.1, #F18.1,  #F22.1, #24.1, #F29.1

                if(!isMandatory)
                {
                    /* This field in not mandatory, so blank value is acceptable */
                    if(string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Acceptable input - validation Pass */
                        return;
                    }
                }

                /* Desc: Valid values are Y or N
                         Match against valid values should be case-insensitive */

                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    if (columnData.Equals(MemberProfileCSVUploadConstants.Yes, StringComparison.InvariantCultureIgnoreCase)
                        || columnData.Equals(MemberProfileCSVUploadConstants.No, StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Acceptable input - validation Pass */
                        return;
                    }
                }

                /* Control Here indicates - Invalid Validation Fails */

                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Same Error Message Apply for  - Validation #F17.1 and #F18.1 and #F22.1 and #F24.1 and #F25.1 and 
                 * #F26.1 and #F27.1 and #F29.1 and #F31.1 and #F32.1 and #F33.1 and #F34.1 */
                string failureReason = Messages.ResourceManager.GetString(ErrorCodes.F17Dot1);

                CleanUpAndNotifyBeforeHaltingProcessing(2, failureReason, rowNumber.ToString(), fieldName, columnData);

                /* Stop Further Processing */
                return;

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in CountryCodeFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private string GetDependentFieldValue(int rowNumber, int dependentColumnNumber)
        {
            try
            {
                List<string> dependentRow = null;
                if (_csvFileData.TryGetValue(rowNumber, out dependentRow))
                {
                    /* Row Found and So retrieving expected column data */
                    return dependentRow[dependentColumnNumber];
                }
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in GetDependentFieldValue() ");
                Logger.Error(exception);
            }

            /* Stop Further Processing */
            return null;
        }

        #endregion

        #region Field Specific Validations

        private void MemberCodeNumericFieldValidations(int rowNumber, int columnNumber, string columnData, string fieldName)
        {
            try
            {
                #region Validation #F1.1

                /* Desc: Should not be a duplicate of any other value of ‘MemberCodeNumeric’ within the file
                         Duplicate should be checked after conversion to uppercase */

                /* Loop Through Every CSV File Data Row */
                foreach (KeyValuePair<int, List<string>> rows in _csvFileData)
                {
                    /* Exclude current row */
                    if (rows.Key != rowNumber)
                    {
                        /* Check if Member Code Numeric is Same */
                        if ((rows.Value)[columnNumber].ToUpper().Trim().Equals(columnData, StringComparison.InvariantCultureIgnoreCase))
                        {
                            /* Duplicate MemberCodeNumeric Exists in File */

                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                ErrorCodes.F1Dot1), rowNumber.ToString(), fieldName,
                                                                     columnData);

                            /* Stop Further Processing */
                            return;
                        }
                    }
                }

                #endregion

                #region Validation #F1.2

                /* Desc: Should not be a duplicate of any existing Member in the Member Profile, irrespective of the ‘IS Membership Status’
                         Duplicate should be checked after conversion to uppercase */

                /* Existing Member Code Numeric Values is kind of Master Data and Hence Loaded only once before beginning Level 2 Validations */
                if (existingMemberCodeNumericValues.Contains(columnData.ToUpper()))
                {
                    /* Member Code Numeric is Already Existing in Database - Validation Failure */
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                        ErrorCodes.F1Dot2), rowNumber.ToString(), fieldName,
                                                             columnData);

                    /* Stop Further Processing */
                    return;
                }
                /* For Logical Completion */
                //else
                //{
                //    Validation Pass
                //}

                #endregion

                #region Validation #F1.3

                /* Desc: This validation is applicable when a 4 numeric value is provided, with or without leading zeroes
                         Value should be equal to or greater than 3600 */

                int intNumericCode = 0;
                if (int.TryParse(columnData, out intNumericCode) && columnData.Length == MemberProfileCSVUploadConstants.Four)
                {
                    /* Bug fix to create member with numeric code greater then 3599. 
                     * if (intNumericCode <= 3600)*/
                    if (intNumericCode <= 3599)
                    {
                        /* Member Code Numeric is Not Acceptable - Validation Failure */
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                            ErrorCodes.F1Dot3), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    }
                    /* For Logical Completion */
                    //else
                    //{
                    //    Validation Pass
                    //}
                }
                /* For Logical Completion */
                //else
                //{
                //    Validation Bypassed Since input Member Code Numeric is Not Numeric
                //}

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in FieldSpecificValidations_MemberCodeNumeric() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void IsMembershipStatusFieldValidations(int rowNumber, string columnData, string fieldName, ref MemberProfileData memberProfileData)
        {
            try
            {
                #region Validation #F5.1

                /* Desc: Should be a valid ‘IS Membership Status’ as per values maintained in the system
                         Match against ‘IS Membership Status’ should be case-insensitive*/

                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    columnData = columnData.ToUpper();
                    int membershipStatusId = 0;

                    if (columnData.Equals(MemberStatus.Active.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Validation Pass - Acceptable Input */
                        memberProfileData.ISMembershipStatusId = (int)MemberStatus.Active;
                    }
                    else if (columnData.Equals(MemberStatus.Basic.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Validation Pass - Acceptable Input */
                        memberProfileData.ISMembershipStatusId = (int)MemberStatus.Basic;
                    }
                    else if (columnData.Equals(MemberStatus.Pending.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Validation Pass - Acceptable Input */
                        memberProfileData.ISMembershipStatusId = (int)MemberStatus.Pending;
                    }
                    else if (columnData.Equals(MemberStatus.Restricted.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Validation Pass - Acceptable Input */
                        memberProfileData.ISMembershipStatusId = (int)MemberStatus.Restricted;
                    }
                    else if (columnData.Equals(MemberStatus.Terminated.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Validation Pass - Acceptable Input */
                        memberProfileData.ISMembershipStatusId = (int)MemberStatus.Terminated;
                    }
                    else
                    {
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                            ErrorCodes.F5Dot1), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    }
                }

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ISMembershipStatusFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void IsMembershipSubStatusFieldValidations(int rowNumber, string columnData, string fieldName, ref MemberProfileData memberProfileData)
        {
            try
            {
                #region Validation #F6.1

                /* Desc: Should be a valid ‘IS Membership Sub Status’ as per the Member Sub Status master 
                         Match against ‘IS Membership Sub Status’ should be case-insensitive
                         Only Active records from the master should be matched */

                if (membershipSubStatusDetails == null || membershipSubStatusDetails.Count == 0)
                {
                    /* First Read of Master Data */
                    var membershipSubStatusManager = Ioc.Resolve<ISisMemberSubStatusManager>(typeof(ISisMemberSubStatusManager));
                    membershipSubStatusDetails = membershipSubStatusManager.GetSisMemberSubStatusList(null);
                }
                /* For Logical Completion */
                //else
                //{
                //    /* Data Already Available in Class Data Member */   
                //}

                foreach (var membershipSubStatusDetail in membershipSubStatusDetails)
                {
                    if (membershipSubStatusDetail.Description.Equals(columnData, StringComparison.InvariantCultureIgnoreCase)
                        && membershipSubStatusDetail.IsActive)
                    {
                        /* Sub status found - Validation Pass */
                        memberProfileData.ISMembershipSubStatusId = membershipSubStatusDetail.Id;
                        return;
                    }
                }

                /* Control Here indicates - Invalid Sub Status */

                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                    ErrorCodes.F6Dot1), rowNumber.ToString(), fieldName,
                                                         columnData);

                /* Stop Further Processing */
                return;

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ISMembershipSubStatusFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void CountryCodeFieldValidations(int rowNumber, string columnData, string fieldName)
        {
            try
            {
                #region Validation #F16.1

                /* Desc: Should be a valid ‘Country Code’ as per the Country master 
                         Match against ‘Country Code’ should be case-insensitive
                         Only Active records from the master should be matched */

                /* All Active Country Code Values is kind of Master Data and Hence Loaded only once before beginning Level 2 Validations */
                if(listOfAllActiveContries.Contains(columnData.ToUpper()))
                {
                    /* Country Code found - Validation Pass */
                    return;
                }
                
                /* Control Here indicates - Invalid Validation Fails */

                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Same Error Message Apply for  - #F6.1 and #F16.1 */
                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                    ErrorCodes.F6Dot1), rowNumber.ToString(), fieldName,
                                                         columnData);

                /* Stop Further Processing */
                return;

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in CountryCodeFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        /// <summary>
        /// This function is used to validate the field 'MISCDailyPayableOptionsForBilateralInvoices'
        /// CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
        /// </summary>
        /// <param name="rowNumber"></param>
        /// <param name="columnData"></param>
        /// <param name="fieldName"></param>
        private void MiscDailyPayableOptionsValidation(int rowNumber, string columnData, string fieldName)
        {
            try
            {
                #region Validation #F30.1

                /* Desc: Should be a valid values 'N, W, WO, WX, WOX' for column ‘MISCDailyPayableOptionsForBilateralInvoices’  */
                if (ValidValuesForMiscDailyPayable.Contains(columnData.ToUpper()))
                {
                    /* Country Code found - Validation Pass */
                    return;
                }

                /* Control Here indicates - Invalid Validation Fails */

                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Same Error Message Apply for  - #F6.1 and #F16.1 */
                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(ErrorCodes.F30Dot1),
                                                        rowNumber.ToString(), fieldName, columnData);

                /* Stop Further Processing */
                return;

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in MiscDailyPayableOptionsValidation() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void InvoicesDSToBeAppliedForFieldValidations(int rowNumber, string columnData, string fieldName, int dependentColumnNumber)
        {
            try
            {
                #region Validation #F19.1 and #F20.1

                /* Desc: Values should not be provided in this field if ‘DigitalSignApplication’ is N or n */

                Logger.InfoFormat("Getting dependent Field Value, Field Name: {0}, Dependent Field Number: {1}",
                                  fieldName, dependentColumnNumber);

                string dependentColumnData = GetDependentFieldValue(rowNumber, dependentColumnNumber);

                Logger.InfoFormat("Value For Dependent Field is [{0}]", dependentColumnData);

                if (!string.IsNullOrWhiteSpace(dependentColumnData))
                {
                    if (dependentColumnData.Equals(MemberProfileCSVUploadConstants.No, StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Value in Dependent Field - ‘DigitalSignApplication’ is N or n */
                        if (!string.IsNullOrWhiteSpace(columnData))
                        {
                            /* Validation Failed - Value Provided in this field when Dependent Column is has value 'N'/'n' */
                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            /* Same Error Message Apply for  - #F19.1 and #F20.1 */
                            CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                ErrorCodes.F19Dot1), rowNumber.ToString(), fieldName,
                                                                     columnData);

                            /* Stop Further Processing */
                            return;
                        }
                    }
                }
                /* Dependent field is Mandatory and So control is not expected here - Below Code is just for Logical Completion */
                //else
                //{
                //    /* Set Level 2 Validation Status */
                //    SetLevel2ValidationStatus(false);

                //    UpdateFileStatusAndSendNotificationEmail(2, Messages.ResourceManager.GetString(
                //        ErrorCodes.F16Dot1), rowNumber.ToString(), fieldName,
                //                                             columnData);

                //    /* Stop Further Processing */
                //    return;
                //}

                #endregion

                /* If data provided in this optional field only then - 
                 * 1. input patter matching validation - 19.2, 20.2
                 * 2. country validation 19.3, 20.3
                 * 3. And DS support validation 19.4, 20.4                  
                 * Will apply, else need to by pass these validations. 
                 */
                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    /* Data provided in this optional field and so below validation will apply */
                    #region Validation #F19.2 and #F20.2

                    /* Desc: Data representation of country code(s) in this field should be correct
                         Each country code should be of exactly 2 characters 
                         Commas should separate two countries, if multiple countries are provided
                         Extra commas should not exist
                         No spaces should exist within the value of this field */

                    Regex pattern = new Regex("^([a-zA-Z0-9]{2})(,[a-zA-Z0-9]{2})*$");

                    /* Data provided in this optional field and hence it should match with specified format. */
                    if (!pattern.IsMatch(columnData))
                    {
                        /* Validation Failed */
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        /* Same Error Message Apply for  - #F19.2 ad #F20.2 */
                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                ErrorCodes.F19Dot2), rowNumber.ToString(), fieldName,
                                                                    columnData);

                        /* Stop Further Processing */
                        return;
                    }

                    /* For Logical Completion */
                    //else
                    //{
                    /* No input data in this field and so format matching is bypassed. */
                    //}
                    #endregion

                    Country validCountry = null;

                    foreach (string countryCode in columnData.Split(','))
                    {
                        if (!listOfAllActiveContries.Contains(countryCode.ToUpper()))
                        {
                            #region Validation #F19.3 and #F20.3

                            /* Desc: Every Country Code provided in this column should be a valid ‘Country Code’ as per the Country master 
                         Match against ‘Country Code’ should be case-insensitive
                         Only Active records from the master should be matched */

                            /* Validation Failed - Country Code is Invalid */

                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            /* Same Error Message Apply for  - #F19.3 and #F20.3 */
                            CleanUpAndNotifyBeforeHaltingProcessing(2, string.Format(Messages.ResourceManager.GetString(
                               ErrorCodes.F19Dot3), countryCode), rowNumber.ToString(), fieldName,
                                                                     columnData);

                            /* Stop Further Processing */
                            return;

                            #endregion
                        }
                        /* Control Here Indicates - Valid Country Code, So now Checking if DS Supported */

                        #region Validation #F19.4 and #F20.4

                        /* Desc: Every Country Code provided in this column should support Digital Signature, 
                          * as per the Country master */

                        /* List Of All Active Contries Supporting Digital Signature Values is kind of Master Data and Hence 
                         * Loaded only once before beginning Level 2 Validations */
                        if (!listOfAllActiveContriesSupportingDS.Contains(countryCode))
                        {
                            /* Validation Failed - DS is not Supported By This Country */
                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            /* Same Error Message Apply for  - #F19.4 and #F20.4 */
                            CleanUpAndNotifyBeforeHaltingProcessing(2,
                                                                    string.Format(
                                                                        Messages.ResourceManager.GetString(
                                                                                ErrorCodes.F19Dot4),
                                                                        countryCode), rowNumber.ToString(), fieldName,
                                                                    columnData);

                            /* Stop Further Processing */
                            return;
                        }

                        /* For Logical Completion */
                        //else
                        //{
                        //    /* Validation Pass */
                        //}

                        #endregion

                    }
                }
                /* For Logical Completion */
                //else
                //{
                    /* No input data provided in this optional field, so bypassing these validations */
                //}
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in InvoicesDSToBeAppliedForFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void CDCCompartmentIDForInvFieldValidations(int rowNumber, string columnData, string fieldName, int dependentColumnNumber)
        {
            try
            {
                Logger.InfoFormat("Getting dependent Field Value, Field Name: {0}, Dependent Field Number: {1}",
                                  fieldName, dependentColumnNumber);

                string dependentColumnData = GetDependentFieldValue(rowNumber, dependentColumnNumber);

                Logger.InfoFormat("Value For Dependent Field is [{0}]", dependentColumnData);

                if (!string.IsNullOrWhiteSpace(dependentColumnData))
                {
                    #region Validation #F23.1

                    /* Desc: Values should not be provided in this field if ‘LegalArchivingRequired’ is N or n */

                    if (dependentColumnData.Equals(MemberProfileCSVUploadConstants.No, StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Value in Dependent Field - ‘LegalArchivingRequired’ is N or n */
                        if (!string.IsNullOrWhiteSpace(columnData))
                        {
                            /* Validation Failed - Value Provided in this field when Dependent Column is has value 'N'/'n' */
                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                ErrorCodes.F23Dot1), rowNumber.ToString(), fieldName,
                                                                     columnData);

                            /* Stop Further Processing */
                            return;
                        }
                    }

                    #endregion

                    #region Validation #F23.2

                    /* Desc: Values should be provided in this field if ‘LegalArchivingRequired’ is Y or y */

                    else if (dependentColumnData.Equals(MemberProfileCSVUploadConstants.Yes, StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Value in Dependent Field - ‘LegalArchivingRequired’ is Y or y */
                        if (string.IsNullOrWhiteSpace(columnData))
                        {
                            /* Validation Failed - Value Should be Provided in this field when Dependent Column is has value 'Y'/'y' */
                            /* Set Level 2 Validation Status */
                            SetLevel2ValidationStatus(false);

                            CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                ErrorCodes.F23Dot2), rowNumber.ToString(), fieldName,
                                                                     columnData);

                            /* Stop Further Processing */
                            return;
                        }
                    }

                    #endregion

                    /* For Logical Completion */
                    //else
                    //{

                    //}
                }
                /* Dependent field is Mandatory and So control is not expected here - Below Code is just for Logical Completion */
                //else
                //{
                //    /* Set Level 2 Validation Status */
                //    SetLevel2ValidationStatus(false);

                //    UpdateFileStatusAndSendNotificationEmail(2, Messages.ResourceManager.GetString(
                //        ErrorCodes.F16Dot1), rowNumber.ToString(), fieldName,
                //                                             columnData);

                //    /* Stop Further Processing */
                //    return;
                //}

                #region Validation #F23.3

                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    /* Desc: If a value is provided, it should not have any spaces within it (inside the value). Leading and trailing spaces should be ignored */
                    columnData = columnData.Trim();

                    if (columnData.Contains(" "))
                    {
                        /* Validation Failed - Value Contains Space in it */
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                            ErrorCodes.F23Dot3), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    }
                }

                #endregion

            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in CDCCompartmentIDForInvFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void PayAndRecLAAndListingRelatedFieldValidations(int rowNumber, string columnData, string fieldName, int dependentColumnNumber, bool isMandatory)
        {
            try
            {
                #region Validation #F24.1

                /* Desc: Valid values are Y or N, Match against valid values should be case-insensitive */
                BooleanFieldValidations(rowNumber, columnData, fieldName, isMandatory);

                if (!GetLevel2ValidationStatus())
                {
                    /* Validation Failed - So Bypassing Further Checks */
                    return;
                }

                #endregion

                #region Validation #F24.2

                /* Desc: If ‘LegalArchivingRequired’ is N or n, then this value should not be Y or y */

                Logger.InfoFormat("Getting dependent Field Value, Field Name: {0}, Dependent Field Number: {1}",
                                  fieldName, dependentColumnNumber);

                string dependentColumnData = GetDependentFieldValue(rowNumber, dependentColumnNumber);

                Logger.InfoFormat("Value For Dependent Field is [{0}]", dependentColumnData);

                if (!string.IsNullOrWhiteSpace(dependentColumnData))
                {
                    if (dependentColumnData.Equals(MemberProfileCSVUploadConstants.No, StringComparison.InvariantCultureIgnoreCase))
                    {
                        /* Value in Dependent Field - ‘LegalArchivingRequired’ is N or n */
                        if (!string.IsNullOrWhiteSpace(columnData))
                        {
                            if (columnData.Equals(MemberProfileCSVUploadConstants.Yes, StringComparison.InvariantCultureIgnoreCase))
                            {
                                /* Validation Failed - Dependent Field is 'N'/'n' and this Field is 'Y'/'y' */
                                /* Set Level 2 Validation Status */
                                SetLevel2ValidationStatus(false);

                                /* Same Error Message Apply for  - #F24.2 and #F25.2 and #F26.2 and #F27.2 */
                                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                    ErrorCodes.F24Dot2), rowNumber.ToString(), fieldName,
                                                                         columnData);

                                /* Stop Further Processing */
                                return;
                            }
                        }
                    }
                }
                /* Dependent field is Mandatory and So control is not expected here - Below Code is just for Logical Completion */
                //else
                //{
                //    /* Set Level 2 Validation Status */
                //    SetLevel2ValidationStatus(false);

                //    UpdateFileStatusAndSendNotificationEmail(2, Messages.ResourceManager.GetString(
                //        ErrorCodes.F16Dot1), rowNumber.ToString(), fieldName,
                //                                             columnData);

                //    /* Stop Further Processing */
                //    return;
                //}

                #endregion

            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in LegalArchRequiredForMISCRecInvFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void MISCAllowedFileTypesForSupportingDocumentsFieldValidations(int rowNumber, string columnData, string fieldName)
        {
            try
            {
                #region Validation #F28.1

                /* If value is not provided need to bypass pattern matching. */
                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    /* Desc: Data representation of extension(s) in this field should be correct
                         Each extension should consist of a dot followed by a minimum of one character, or a maximum of six characters
                         Commas should separate two extensions, if multiple extensions are provided
                         Extra commas should not exist
                         No spaces should exist within the value of this field */

                    Regex pattern = new Regex(@"^[\.]([a-zA-Z]{1}[0-9a-zA-Z]{0,5})+((,{1}[.][a-zA-Z]{1}[0-9a-zA-Z]{0,5}){0,9})+$");
                    if (!pattern.IsMatch(columnData))
                    {
                        /* Validation Failed */
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        /* Same Error Message Apply for  - #F19.2 and #F28.1 */
                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                ErrorCodes.F19Dot2), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    } 
                }

                #endregion
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in MISCAllowedFileTypesForSupportingDocumentsFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void NewUserEmailAddressFieldValidations(int rowNumber, int columnNumber, string columnData, string fieldName, int dependentColumnNumber)
        {
            try
            {
                bool isEmailIdProvided = false;
                /* Desc: Should be a valid value as per standard email address rules */
                if (!string.IsNullOrWhiteSpace(columnData))
                {
                    isEmailIdProvided = true;
                    if (!Regex.IsMatch(columnData.ToLower(), MemberProfileCSVUploadConstants.ValidEmailPattern))
                    {
                        #region Validation #F36.1

                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                            ErrorCodes.F36Dot1), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;

                        #endregion
                    }
                    /* For Logical Completion */
                    //else
                    //{
                    //    Input Value is Provided and it Matches with Email Standards
                    //}
                }
                /* For Logical Completion */
                //else
                //{
                //    /* No input Value Provided in this Field */
                //    isEmailIdProvided = false;
                //}

                Logger.InfoFormat("Getting dependent Field Value, Field Name: {0}, Dependent Field Number: {1}",
                                  fieldName, dependentColumnNumber);

                string dependentColumnData = GetDependentFieldValue(rowNumber, dependentColumnNumber);

                Logger.InfoFormat("Value For Dependent Field is [{0}]", dependentColumnData);

                if (string.IsNullOrWhiteSpace(dependentColumnData))
                {
                    #region Validation #F36.2

                    /* Dependent Field 'NewUserFirstName' is Blank */
                    if (isEmailIdProvided == true)
                    {
                        /* Desc: Value should not be provided in this field if ‘NewUserFirstName’ is blank */

                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                            ErrorCodes.F36Dot2), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    }

                    #endregion
                }
                else
                {
                    /* Dependent Field 'NewUserFirstName' is NOT Blank */
                    #region Validation #F36.3

                    /* Desc: A value should be provided in this field if ‘NewUserFirstName’ is non-blank */
                    if (isEmailIdProvided == false)
                    {
                        /* Desc: Value should not be provided in this field if ‘NewUserFirstName’ is blank */

                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                            ErrorCodes.F36Dot3), rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    }

                    #endregion
                }

                #region Validation #F36.4

                /* Desc: Should not be a duplicate of any other value of ‘NewUserEmailAddress’ within the file
                         Duplicate should be checked after conversion to uppercase */

                /* Loop Through Every CSV File Data Row */
                foreach (KeyValuePair<int, List<string>> rows in _csvFileData)
                {
                    /* Exclude current row */
                    if (rows.Key != rowNumber)
                    {
                        /* Check if NewUserEmailAddress is Same */
                        string emailAddress = (rows.Value)[columnNumber];
                        if (!string.IsNullOrWhiteSpace(emailAddress))
                        {
                            if (emailAddress.ToUpper().Trim().Equals(columnData, StringComparison.InvariantCultureIgnoreCase))
                            {
                                /* Duplicate MemberCodeNumeric Exists in File */

                                /* Set Level 2 Validation Status */
                                SetLevel2ValidationStatus(false);

                                /* Same Error Message Apply for  - #F1.1 and #F36.4 */
                                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                    ErrorCodes.F1Dot1), rowNumber.ToString(), fieldName,
                                                                         columnData);

                                /* Stop Further Processing */
                                return;
                            }
                        }
                    }
                }

                #endregion

                #region Validation #F36.5

                /* Desc: Should not be a duplicate of any existing user Email Address in the system 
                 *       (irrespective of active/inactive, locked/unlocked and user category)
                 *       Duplicate should be checked after conversion to uppercase */

                /* Existing User Email Id Values is kind of Master Data and Hence Loaded only once before beginning Level 2 Validations */
                if (existingUserEmailIdValues.Contains(columnData.ToUpper()))
                {
                    /* User mail Id is Already Existing in Database - Validation Failure */
                    /* Set Level 2 Validation Status */
                    SetLevel2ValidationStatus(false);

                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                        ErrorCodes.F36Dot5), rowNumber.ToString(), fieldName,
                                                             columnData);

                    /* Stop Further Processing */
                    return;
                }
                /* For Logical Completion */
                //else
                //{
                //    Validation Pass
                //}

                #endregion

            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in NewUserEmailAddressFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        private void NewUserFieldValidations(int rowNumber, int columnNumber, string columnData, string fieldName, int dependentColumnNumber)
        {
            try
            {
                Logger.InfoFormat("Getting dependent Field Value, Field Name: {0}, Dependent Field Number: {1}",
                                  fieldName, dependentColumnNumber);

                string dependentColumnData = GetDependentFieldValue(rowNumber, dependentColumnNumber);

                Logger.InfoFormat("Value For Dependent Field is [{0}]", dependentColumnData);

                #region Validation #F37.1, #F38.1, #F39.1, #F40.1

                if (string.IsNullOrWhiteSpace(dependentColumnData))
                {
                    /* Desc: Value should not be provided in this field if ‘NewUserEmailAddress’ is blank */

                    /* Dependent Field 'NewUserEmailAddress' is Blank */
                    if (!string.IsNullOrWhiteSpace(columnData))
                    {
                        /* Value Provided in This Field When Not Expected */
                        /* Set Level 2 Validation Status */
                        SetLevel2ValidationStatus(false);

                        string failureReasonText = Messages.ResourceManager.GetString(
                                    ErrorCodes.F37Dot1);

                        /* Same Error Message Apply for  - #F37.1 and #F38.1 and #F39.1 and #F40.1 */
                        CleanUpAndNotifyBeforeHaltingProcessing(2, failureReasonText, rowNumber.ToString(), fieldName,
                                                                 columnData);

                        /* Stop Further Processing */
                        return;
                    }
                }

                #endregion

                else
                {
                    /* Dependent Field 'NewUserEmailAddress' is NOT Blank */

                    switch (columnNumber)
                    {
                        //CMP-665-User Related Enhancements-FRS
                        //Sec 2.4.1 ‘Last Name’ Changes from Optional to Mandatory
                        case 36:
                        case 37:
                            #region Validation #F37.2

                            /* Desc: A value should be provided in this field if ‘NewUserEmailAddress’ is non-blank */

                            if (string.IsNullOrWhiteSpace(columnData))
                            {
                                /* Value NOT Provided in This Field When Expected */
                                /* Set Level 2 Validation Status */
                                SetLevel2ValidationStatus(false);

                                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                    ErrorCodes.F37Dot2), rowNumber.ToString(), fieldName,
                                                                         columnData);

                                /* Stop Further Processing */
                                return;
                            }

                            #endregion
                            break;

                        case 38:
                            #region Validation #F39.2

                            /* Desc: Value S or N should be provided in this field if ‘NewUserEmailAddress’ is non-blank 
                             * Match against valid values should be case-insensitive */

                            if (string.IsNullOrWhiteSpace(columnData))
                            {
                                /* Value NOT Provided in This Field - Validation Failed */

                                /* Set Level 2 Validation Status */
                                SetLevel2ValidationStatus(false);

                                CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                    ErrorCodes.F39Dot2), rowNumber.ToString(), fieldName,
                                                                         columnData);

                                /* Stop Further Processing */
                                return;
                            }
                            else //if (!string.IsNullOrWhiteSpace(columnData))
                            {
                                if (!columnData.Equals(MemberProfileCSVUploadConstants.SuperUserTypeChar, StringComparison.InvariantCultureIgnoreCase)
                                    && !columnData.Equals(MemberProfileCSVUploadConstants.NonSuperUserTypeChar, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    /* Input is unacceptable - validation Failed */
                                    /* Set Level 2 Validation Status */
                                    SetLevel2ValidationStatus(false);

                                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                        ErrorCodes.F39Dot2), rowNumber.ToString(), fieldName,
                                                                             columnData);

                                    /* Stop Further Processing */
                                    return;
                                }
                            }

                            #endregion
                            break;

                        case 39:
                            #region Validation #F40.2

                            if (!string.IsNullOrWhiteSpace(columnData))
                            {
                                /* Value Provided in This Field */

                                /* Desc: If a non-blank value is provided, it should be a valid Template Name 
                                 * as per the Permission Templates existing in the system, where:
                                 * User Category is Member, and Owner Member and System Defined is True
                                 * Match against Template Names should be case-insensitive */

                                /* Existing Permission Template Values is kind of Master Data and Hence Loaded only once before beginning Level 2 Validations */
                                if (!existingPermissionTemplateValues.Contains(columnData.ToUpper()))
                                {
                                    /* User Permission Template is NOT Existing in Database - Validation Failure */
                                    /* Set Level 2 Validation Status */
                                    SetLevel2ValidationStatus(false);

                                    CleanUpAndNotifyBeforeHaltingProcessing(2, Messages.ResourceManager.GetString(
                                        ErrorCodes.F40Dot2), rowNumber.ToString(), fieldName,
                                                                             columnData);

                                    /* Stop Further Processing */
                                    return;
                                }
                                /* For Logical Completion */
                                //else
                                //{
                                //    Validation Pass
                                //}
                            }
                            /* For Logical Completion */
                            //else
                            //{
                            //    /* Optional Column, So may not be provided in input. */
                            //}

                            #endregion
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                /* Set Level 2 Validation Status */
                SetLevel2ValidationStatus(false);

                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in NewUserFieldValidations() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return;
            }
        }

        #endregion

        #endregion

        #endregion

        #region Level 3 Processing of File - Data Loading

        private bool LoadMemberProfileCsvData()
        {
            /* Call Database SP to load Member Profile */
            MemberManager memberManager = new MemberManager();
            try
            {
                /* Create Temp Password and salt for it */

                List<string> memberNumricCodes = new List<string>();
                List<MemberProfileCSVUploadUser> Users = new List<MemberProfileCSVUploadUser>();
                
                Logger.InfoFormat("Number of member Profiles To Load [{0}]", _memberProfiles.Count);
                for (int cnt = 0; cnt < _memberProfiles.Count; cnt++)
                {
                    try
                    {
                        var memberProfileData = _memberProfiles[cnt];

                        /* Preserve Member Code Numeric Values FOr Further Use - SAN Folder Creation */
                        memberNumricCodes.Add(memberProfileData.MemberCodeNumeric);

                        Logger.InfoFormat("Starting To Load Member with Numeric Code: {0},", memberProfileData.MemberCodeNumeric);

                        if (!ApplyTransformationsBeforeCallingLoadingSP(ref memberProfileData, ref Users))
                        {
                            /* Failed Applying Transformations Before Calling Validations */
                            return false;
                        }
                        Logger.Info("Transformations Applied Successfully and Now Calling Database SP To Load Member");

                        int memberIdNewlyAdded = memberManager.CreateMemberViaCsv(memberProfileData);

                        if(memberIdNewlyAdded < 0)
                        {
                            /* Execution was successful, but resulted into some Database Problem, So need to Rollback this Transaction */
                            Logger.Info("Calling Database SP to Rollback Inserted Members");

                            bool isRollbackSuccessful = memberManager.RollbackAlreadyInsertedMembers(ConvertUtil.ConvertGuidToString(iSFileLogRecord.Id));
                            if (isRollbackSuccessful)
                            {
                                Logger.Info("Rollback Successfully.");
                            }
                            else
                            {
                                Logger.Info("Problem in Rolling back the transaction.");
                            }

                            /* Stop Further Processing */
                            return false;
                        }

                        Logger.InfoFormat(
                            "Member Code Numeric [{0}] is added successfully, Member Id generated for it is[{1}]",
                            memberProfileData.MemberCodeNumeric, memberIdNewlyAdded);
                        
                        //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                        memberProfileData = null;
                    }
                    catch (Exception exception)
                    {
                        /* In case of Any Exception - Rollback */
                        Logger.Info("Exception occurred in LoadMemberProfileCsvData() ");
                        Logger.Error(exception);

                        Logger.Info("Calling Database SP to Rollback Inserted Members");

                        bool isRollbackSuccessful = memberManager.RollbackAlreadyInsertedMembers(ConvertUtil.ConvertGuidToString(iSFileLogRecord.Id));
                        if (isRollbackSuccessful)
                        {
                            Logger.Info("Rollback Successfully.");
                        }
                        else
                        {
                            Logger.Info("Problem in Rolling back the transaction.");
                        }

                        /* Stop Further Processing */
                        return false;
                    }
                }

                /* Set Member Active now, as everything is successfully completed. */
                Logger.InfoFormat("Completed Loading All the [{0}] Members", _memberProfiles.Count);
                Logger.Info("Calling Database SP to Activate These Newly Inserted Members");

                /* Call Database SP to activate Members */
                bool isUpdationSuccessful = memberManager.UpdateNewlyInsertedMembers(ConvertUtil.ConvertGuidToString(iSFileLogRecord.Id));
                if (isUpdationSuccessful)
                {
                    Logger.Info("All New Members Are Activated Successfully.");

                    /* Creating SAN Folder Structure For These Newly Created members */
                    Logger.Info("Creating SAN Folder Structure For All Newly Created members...");
                    CreateSANFolderStructure(memberNumricCodes);

                    Logger.Info("Sending Password Email To All Newly Created members Users ...");
                    SendPasswordEmailNotification(Users);
                }
                else
                {
                    Logger.Info("Problem Activating Newly Inserted Members.");
                }

                Logger.Info("Job Completed Successfully, Members Activated Successfully");
            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in LoadMemberProfileCsvData() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Processing completed Successfully */
            return true;
        }

        private void CreateSANFolderStructure(List<string> memberNumricCodes)
        {
            try
            {
                string ftpBasePath = AdminSystem.SystemParameters.Instance.General.FtpRootBasePath.ToString();
                foreach (var memberCode in memberNumricCodes)
                {
                    Logger.InfoFormat("Creating Folder Structure For member Code numeric [{0}]", memberCode);
                    string memberFolder = Path.Combine(ftpBasePath, memberCode);

                    if (!Directory.Exists(memberFolder))
                    {
                        Directory.CreateDirectory(memberFolder);
                        Logger.InfoFormat("Created Folder Path: [{0}]", memberFolder);
                    }
                    else
                    {
                        Logger.InfoFormat("Folder already Exists Path:[{0}]", memberFolder);
                    }

                    string memberUploadFolder = Path.Combine(memberFolder,
                                                             MemberProfileCSVUploadConstants.UploadFolderName);
                    if (!Directory.Exists(memberUploadFolder))
                    {
                        Directory.CreateDirectory(memberUploadFolder);
                        Logger.InfoFormat("Created Folder Path: [{0}]", memberUploadFolder);
                    }
                    else
                    {
                        Logger.InfoFormat("Folder already Exists Path:[{0}]", memberUploadFolder);
                    }

                    string memberDownloadFolder = Path.Combine(memberFolder,
                                                               MemberProfileCSVUploadConstants.DownloadFolderName);
                    if (!Directory.Exists(memberDownloadFolder))
                    {
                        Directory.CreateDirectory(memberDownloadFolder);
                        Logger.InfoFormat("Created Folder Path: [{0}]", memberDownloadFolder);
                    }
                    else
                    {
                        Logger.InfoFormat("Folder already Exists Path:[{0}]", memberDownloadFolder);
                    }

                    string memberErrorFolder = Path.Combine(memberDownloadFolder,
                                                            MemberProfileCSVUploadConstants.ErrorFolderName);
                    if (!Directory.Exists(memberErrorFolder))
                    {
                        Directory.CreateDirectory(memberErrorFolder);
                        Logger.InfoFormat("Created Folder Path: [{0}]", memberErrorFolder);
                    }
                    else
                    {
                        Logger.InfoFormat("Folder already Exists Path:[{0}]", memberErrorFolder);
                    }
                }
            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in CreateSANFolderStructure() ");
                Logger.Error(exception);

                /* Stop Further Processing */
            }
        }

        private bool ApplyTransformationsBeforeCallingLoadingSP(ref MemberProfileData memberProfileData, ref List<MemberProfileCSVUploadUser> Users)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(memberProfileData.NewUserEmailAddress))
                {
                    // Generate New Password
                    var dataSecurity = new DataSecurity();
                    var salt = dataSecurity.CreateRandomSalt(6);
                    var password = dataSecurity.Password.GenerateRandomPassword(8);
                    //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                    memberProfileData.Password = dataSecurity.HashString(password, ref salt); 
                    memberProfileData.Salt = salt;

                    /* Preserve New User Details For Further Use - Welcome Email Notification */
                    MemberProfileCSVUploadUser User = new MemberProfileCSVUploadUser();
                    User.EmailId = memberProfileData.NewUserEmailAddress;
                    User.FirstName = memberProfileData.NewUserFirstName;
                    User.LastName = memberProfileData.NewUserLastName;
                    User.Password = password;
                    //CMP-665-User Related Enhancements-FRS
                    //Sec 2.4.2 Conditional Suppression of OTP Email to New Member Users
                    User.IsMembershipSubStatusId = memberProfileData.ISMembershipSubStatusId;

                    Users.Add(User);
                    //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                    password =  string.Empty;
                }
                else
                {
                    memberProfileData.Password = string.Empty;
                    memberProfileData.Salt = string.Empty;
                }
                
                memberProfileData.EntryDate = iSFileLogRecord.FileDate.ToString();
                memberProfileData.Location = MemberProfileCSVUploadConstants.Location;
                memberProfileData.MemberType = MemberProfileCSVUploadConstants.MemberType;
                memberProfileData.IsLocationActive = 1;
                memberProfileData.UserId = iSFileLogRecord.LastUpdatedBy;
                memberProfileData.isLegalArchRequiredForMISCRecInv = GetIntegetValue(memberProfileData.LegalArchRequiredForMISCRecInv);
                memberProfileData.isLegalArchRequiredForMISCPayInv = GetIntegetValue(memberProfileData.LegalArchRequiredForMISCPayInv);
                memberProfileData.isIncludeListingsMISCRecArch = GetIntegetValue(memberProfileData.IncludeListingsMISCRecArch);
                memberProfileData.isIncludeListingsMISCPayArch = GetIntegetValue(memberProfileData.IncludeListingsMISCPayArch);
                memberProfileData.isMISCBilledInvoiceXMLOutput = GetIntegetValue(memberProfileData.MISCBilledInvoiceXMLOutput);

                //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3
                memberProfileData.IsMiscDailyPayableOptionsForBilateralInvoices = GetIntegetValue(memberProfileData.MiscDailyPayableOptionsForBilateralInvoices);
                memberProfileData.IsMiscDailyPayableOfflineAchiveOutputs = GetIntegetValue(memberProfileData.MiscDailyPayableOfflineAchiveOutputs);
                memberProfileData.IsMiscDailyPayableIsXmlFiles = GetIntegetValue(memberProfileData.MiscDailyPayableIsXmlFiles);
               
                memberProfileData.isMISCIsPdfAsOtherOutputAsBilledEntity = GetIntegetValue(memberProfileData.MISCIsPdfAsOtherOutputAsBilledEntity);
                
                memberProfileData.isMISCIsDetailListingAsOtherOutputAsBilledEntity = GetIntegetValue(memberProfileData.MISCIsDetailListingAsOtherOutputAsBilledEntity);
                memberProfileData.isMISCIsSuppDocAsOtherOutputAsBilledEntity = GetIntegetValue(memberProfileData.MISCIsSuppDocAsOtherOutputAsBilledEntity);
                memberProfileData.isMISCIsDSFileAsOtherOutputAsBilledEntity = GetIntegetValue(memberProfileData.MISCIsDSFileAsOtherOutputAsBilledEntity);
                memberProfileData.isDigitalSignApplication = GetIntegetValue(memberProfileData.DigitalSignApplication);
                memberProfileData.isDigitalSignVerification = GetIntegetValue(memberProfileData.DigitalSignVerification);
                memberProfileData.isLegalArchRequired = GetIntegetValue(memberProfileData.LegalArchivingRequired);
                memberProfileData.NewUserTypeId = string.IsNullOrWhiteSpace(memberProfileData.NewUserType) ? 0 : GetIntegetValue(memberProfileData.NewUserType);

                memberProfileData.IsFileLogId = ConvertUtil.ConvertGuidToString(iSFileLogRecord.Id);
            }
            catch (Exception exception)
            {
                /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                Logger.Info("Exception occurred in ApplyTransformationsBeforeCaallingLoadingSP() ");
                Logger.Error(exception);

                /* Stop Further Processing */
                return false;
            }

            /* Everything Completed Successfully */
            return true;
        }

        private int GetIntegetValue(string strValue)
        {
            if (strValue.Equals(MemberProfileCSVUploadConstants.Yes, StringComparison.InvariantCultureIgnoreCase))
                return 1;
            else if (strValue.Equals(MemberProfileCSVUploadConstants.No, StringComparison.InvariantCultureIgnoreCase))
                return 0;
            else if (strValue.Equals(MemberProfileCSVUploadConstants.SuperUserTypeChar, StringComparison.InvariantCultureIgnoreCase))
                return 1;
            else if (strValue.Equals(MemberProfileCSVUploadConstants.NonSuperUserTypeChar, StringComparison.InvariantCultureIgnoreCase))
                return 0;
            else
                return 0;
        }

        private void SendPasswordEmailNotification(List<MemberProfileCSVUploadUser> Users)
        {
            for (int cnt = 0; cnt < Users.Count; cnt++)
            {
                try
                {
                    //CMP #665-User Related Enhancements-FRS
                    //Sec 2.4.2 Conditional Suppression of OTP Email to New Member Users
                    // ‘Suppress OTP Email’ of that Sub Status reads as No/False  The OTP email should be sent
                    //‘Suppress OTP Email’ of that Sub Status reads as Yes/True  The OTP email should NOT be sent.
                    var suppressOtpEmail =
                        membershipSubStatusDetails.Where(memStatus => memStatus.Id == Users[cnt].IsMembershipSubStatusId)
                            .Select(memStatus => memStatus.SuppressOtpEmail).FirstOrDefault();
                    if (!suppressOtpEmail)
                    {
                      #region Old code Commented by CMP685
                      /*
                       * 
                       * 
                      //create nvelocity data dictionary
                      var context = new VelocityContext();
                      //get an instance of email settings  repository
                      var emailSettingsRepository =
                          Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                      //get an object of the EmailSender component
                      var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
                      //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
                      var templatedTextGenerator =
                          Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

                      try
                      {
                        context.Put("FirstName", Users[cnt].FirstName);
                        context.Put("LastName", Users[cnt].LastName);
                        context.Put("EmailAddress", Users[cnt].EmailId);
                        context.Put("SisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                        context.Put("Password", Users[cnt].Password);
                        context.Put("LogonURL", SystemParameters.Instance.General.LogOnURL);
                        var emailToInvoice =
                            templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.UserWelcomeNotification,
                                                                         context);

                        Logger.Error("Template Created for User Notification.");
                        //Get the eMail settings for user welcome notification
                        var emailSettingForUserNotification =
                            emailSettingsRepository.Get(
                                esfirdu => esfirdu.Id == (int)EmailTemplateId.UserWelcomeNotification);
                        //create a mail object to send mail 
                        var msgUserNotification = new MailMessage
                                                      {
                                                        From =
                                                            new MailAddress(
                                                            emailSettingForUserNotification.SingleOrDefault().
                                                                FromEmailAddress)
                                                      };
                        msgUserNotification.IsBodyHtml = true;
                        //loop through the contacts list and add them to To list of mail to be sent
                        string emailTo = Users[cnt].EmailId;
                        msgUserNotification.To.Add(new MailAddress(emailTo));
                        //set subject of mail (replace special field placeholders with values)
                        msgUserNotification.Subject = emailSettingForUserNotification.SingleOrDefault().Subject;
                        //set body text of mail
                        msgUserNotification.Body = emailToInvoice;
                        //send the mail
                        emailSender.Send(msgUserNotification); 
                      

                        }

                        catch (Exception exception)
                        {
                            Logger.Error("Error occurred occurred in Sending mail for User Notification.", exception);
                        }
                       *
                       *
                       */
                      #endregion

                      #region New Code CMP685 : url link will be sent to user to reset password

                      //Get user ID by new User EMail ID
                      I_ISUser sisUser = AuthManager.GetUserByUserMailId(Users[cnt].EmailId);

                      //generate create new password url link here
                      string carrierFlag = "NF"; string urlPath = Path.Combine(SystemParameters.Instance.General.LogOnURL, "Account/ResetPassword?siscpt=").Replace("\\", "/");
                      if (!UserManagementManager.ResetUserPassword(sisUser.UserID, urlPath, ref carrierFlag))
                      {
                        Logger.InfoFormat("Error occurred occurred in Sending mail for User Notification  for url [0]", urlPath);
                      } 

                      #endregion
                    }

                    //CMP #665: User Related Enhancements-FRS-v1.2 [sec 2.4.3	Web Service Call to ICP].
                    //Request Type "O": Create User(Not Retry), ActionType 1: Create User, ActionType 2: Enable User, ActionType 3: Disable User  
                    new Common.ReferenceManager().EnqueueMessageInIcpLogConsumer(Users[cnt].EmailId, "O", 1);
                }
                catch (Exception exception)
                {
                    /* Log it before throwing. Calling function from IS-WEB (Controller) has to handle this exception. */
                    Logger.Info("Exception occurred in SendPasswordEmailNotification() ");
                    Logger.Error(exception);

                    /* Stop Further Processing */
                }
            }
        }
        
        #endregion

        #region Methods For USe on IS-WEB

        /// <summary>
        /// This will add an entry of File upload in File log table
        /// The Guid returned will be used to add a entry into queue
        /// </summary>
        /// <param name="billingPeriod"></param>
        /// <param name="filePath"></param>
        /// <param name="fileFormatType"></param>
        /// <param name="userId"></param>
        ///  <param name="locationId"></param>
        public Guid AddIsFileLogEntry(BillingPeriod billingPeriod, string filePath, FileFormatType fileFormatType, int userId, string locationId = null)
        {
            var isInputZipFile = new IsInputFile
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(filePath),
                IsIncoming = false,
                FileDate = DateTime.UtcNow,
                BillingPeriod = billingPeriod.Period,
                BillingMonth = billingPeriod.Month,
                SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                FileLocation = Path.GetDirectoryName(filePath),
                FileStatusId = (int)FileStatusType.InProgress,
                FileFormatId = (int)fileFormatType,
                BillingYear = billingPeriod.Year,
                SenderRecieverType = (int)FileSenderRecieverType.IATA,
                ReceivedDate = DateTime.UtcNow,
                FileVersion = "0.1",
                //OutputFileDeliveryMethodId = 1,
                ExpectedResponseTime = 0,
                FileProcessStartTime = DateTime.UtcNow,
                LastUpdatedBy = userId,
                LastUpdatedOn = DateTime.UtcNow
            };

            Logger.Debug("Adding Zip entry in IsInputFile.");

            try
            {
                var invoiceRepository = Ioc.Resolve<IInvoiceRepository>(typeof(IInvoiceRepository));

                //If file is created using system monitor screen then is purge flag should be true.
                invoiceRepository.AddFileLogEntry(isInputZipFile);

                Logger.Debug("Zip file Entry added in IsInputFile table.");

                return isInputZipFile.Id;

            }// End try
            catch (Exception exception)
            {
                Logger.InfoFormat("Error occured while adding entry in IS_FILE_LOG table : {0}", exception.Message);
                return Guid.Empty;
            }// End catch

        }

        /// <summary>
        /// CMP #608: Load Member Profile - CSV Option
        /// Get List of all entries from DB having file format "Upload Member Profile Data"
        /// </summary>
        /// <returns></returns>
        public List<MemberProfileUploadCsv> GetMemberUploadCSVDataFromISFileLog(string pathToDisplayImage, string imageAltText, string imageToolTip)
        {
            var memberUploadCSVDataManager = new MemberUploadCSVDataManager();
            DataTable memberUploadCSVDataTable =
                memberUploadCSVDataManager.GetMemberUploadCsvData((int)FileFormatType.MemberProfileCSVUpload);

            if (memberUploadCSVDataTable != null)
            {
                var memberProfileUploadCsvList = (from DataRow row in memberUploadCSVDataTable.Rows
                                                  select new MemberProfileUploadCsv
                                                  {
                                                      FileUploadDate = Convert.ToDateTime(row["FILE_DATETIME"]),
                                                      UploadedBy = Convert.ToString(row["LAST_UPDATED_BY"]),
                                                      Status = GetStatusMessage(Convert.ToString(row["FILE_STATUS_ID"])),
                                                      FileName = Convert.ToString(row["FILE_NAME"]),
                                                      DownloadFileId = String.Format(
                                                      "<a border='0' href=\"javascript:PostData('{0}',' ');\"><img border='0' src=\"" + pathToDisplayImage +
                                                      "\" alt=\"" + imageAltText + "\" title=\"" + imageToolTip + "\" /></a>&nbsp;",
                                                      ConvertUtil.ConvertByteToString((byte[])row["IS_FILE_LOG_ID"]))
                                                  }).ToList();


                return memberProfileUploadCsvList;
            }

            // return empty list
            return new List<MemberProfileUploadCsv>();
        }

        private static string GetStatusMessage(string status)
        {
            if (status == ((int)FileStatusType.InProgress).ToString())
            {
                return "In Progress";
            }

            if (status == ((int)FileStatusType.Successful).ToString())
            {
                return "Successful";
            }

            if (status == ((int)FileStatusType.Failed).ToString())
            {
                return "Failed";
            }
            return string.Empty;
        }

        #endregion
    }

    internal class MemberProfileCSVUploadConstants
    {
        public const string ServiceName = "Iata.IS.Service.MemberProfileCSVUpload";

        public const int MaxFileNameLength = 50;

        public const int ExpectedNumberOfColumnsInCsvFile = 40;

        public const string Yes = "Y";

        public const string No = "N";

        /* Same RegEx Used on IS-Web User Creation Screen is Used Here. 
         * Iata.IS.Web.Util.FormatConstants.ValidEmailPattern */
        public const string ValidEmailPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";

        public const string LoadingFailureReasonText =
            "An exception was encountered in the loading process. Please contact the support team for corrective action.";

        public const string Location = "Main";

        public const string UploadFolderName = "Upload";

        public const string DownloadFolderName = "Download";

        public const string ErrorFolderName = "Err";

        public const string SuperUserTypeChar = "S";

        public const string NonSuperUserTypeChar = "N";

        public const string MemberType = "MEM";

        public const int Four = 4;
    }

    /// <summary>
    /// CMP #608: Load Member Profile - CSV Option
    /// Added Class to bind data to grid
    /// </summary>
    public class MemberProfileUploadCsv
    {
        public DateTime FileUploadDate { get; set; }
        public string UploadedBy { get; set; }
        public string Status { get; set; }
        public string FileName { get; set; }
        public string DownloadFileId { get; set; }
    }

    internal class MemberProfileCSVUploadUser
    {
        public string EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        //CMP #665: Sec 2.4.2 Conditional Suppression of OTP Email to New Member Users
        //Added new property for get IsMembershipSubStatusId of member.
        public int IsMembershipSubStatusId { get; set; }
    }
}
