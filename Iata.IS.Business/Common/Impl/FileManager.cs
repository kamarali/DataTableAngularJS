using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Xml;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Pax;
using Iata.IS.Data.Common;
using Iata.IS.Data.MemberProfile;
using Iata.IS.FTP;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Common;
using System.Configuration;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfileDataFile;
using Iata.IS.Model.Pax.Common;
using iPayables.UserManagement;
using NVelocity;
using MailComponent = System.Net.Mail;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Business.TemplatedTextGenerator;
using Castle.Core.Smtp;
using Iata.IS.Business.MemberProfile;
using System.Collections.Generic;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using System.Text;
using log4net;
using System.Reflection;
using System.Linq;
using Iata.IS.Core;
using IObjectContext = Iata.IS.Data.IObjectContext;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;

namespace Iata.IS.Business.Common.Impl
{
    public class FileManager : IFileManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static int UploadFileFailureAttempt = 0;
        public static string TdfFileContent = string.Empty;

        // File Watcher Repository declaration
        public IRepository<FileWatcher> FileWatcherRepository { get; set; }

        public IRepository<SystemParameter> SystemParameterRepository { get; set; }

        public IRepository<IsInputFile> IsInputFileRep { get; set; }

        public ISystemParamRepository SystemParamRepository { get; set; }

        public IRepository<IsFileLogExt1Model> IsFileLogExt1Repository { get; set; }

        /// <summary>
        /// Gets or sets the member repository.
        /// </summary>
        /// <value>The member repository.</value>
        public IMemberRepository MemberRepository { get; set; }

        // User Management to insert IS_FILE_LOG Entry
        public IUserManagement AuthManager { get; set; }


        #region Internal Properties

        private static int MaxContactIdsForSingleEmail
        {
          get
          {
            var contactIdscount = ConfigurationManager.AppSettings.Get("MaxContactIdsForSingleEmail");
            int contactIdscountV;
            if (!string.IsNullOrEmpty(contactIdscount) && Int32.TryParse(contactIdscount, out contactIdscountV)) return contactIdscountV;

            // Default Value
            return 200;
          }
        }

      private static int _counterTdfFileForReferenceData = 1;

        #endregion Internal Properties



        /// <summary>
        /// The method will be used to upload a file to FTP to be pulled by member.
        /// </summary>
        /// <param name="fileUploadType"></param>
        /// <param name="memberCode"></param>
        /// <param name="originalFilePath"></param>
        /// <returns></returns>
        public string UploadFileForFTPPull(FileUploadType fileUploadType, string memberCode, string originalFilePath)
        {
            string serverPath = ConfigurationManager.AppSettings["ServerPath"];
            string futureUpdateFolder = Enum.GetName(typeof(FileUploadType), fileUploadType);
            string[] fileNameArray = null;
            string fileName = string.Empty;

            if (originalFilePath != null && File.Exists(originalFilePath))
            {
                //Extract the file name from file path

                fileNameArray = originalFilePath.Split('\\');

                var arrayLength = fileNameArray.Length;
                if (fileNameArray[arrayLength - 1] != null)
                {
                    fileName = fileNameArray[arrayLength - 1];
                }

                //Create folder hierarchy at server
                var serverFolderPath = string.Format("{0}\\{1}\\{2}", serverPath, memberCode, futureUpdateFolder);
                Directory.CreateDirectory(serverFolderPath);

                //Copy the file to the directory
                if (Directory.Exists(serverFolderPath))
                {
                    var serverFilePath = string.Format("{0}\\{1}\\{2}\\{3}", serverPath, memberCode, futureUpdateFolder, fileName);
                    //Copy the file from source to destination
                    File.Copy(originalFilePath, serverFilePath, true);

                    //Return the folder path
                    if (File.Exists(serverFilePath))
                    {
                        return serverFilePath;
                    }
                }
            }
            return string.Empty;
        }

      /// <summary>
      /// Create and save given File in IATA FTP Download folder. 
      /// </summary>
      /// <param name="outputFile">Recharge data xml.</param>
      /// <param name="fileName">Name of the file to create.</param>
      /// <param name="fileFormatType"></param>
      /// <param name="fileList"></param>
      /// <returns>Returns "true" if file is created and saved successfully in IATA FTP Download folder else returns "false".</returns>
      public bool UploadFileForFTPPull(string sourceFile, string fileName, FileFormatType fileFormatType)
      {
          try
          {
              // Get IATA FTP Download folder path. 
              string ftpPathIata = ConfigurationManager.AppSettings["IataFtpPath"];

              Logger.Debug(string.Format("IATA FTP Download Path: {0}", ftpPathIata));

              var outputFilePath = Path.Combine(ftpPathIata, fileName);

              // Delete File if already exists.
              if (File.Exists(outputFilePath))
              {
                  File.Delete(outputFilePath);
              } // End if
              Logger.Debug("outputFilePath : " + outputFilePath);
              // Check if download folder path is not null or empty.
              if (!string.IsNullOrEmpty(ftpPathIata))
              {
                  if (fileFormatType.Equals(FileFormatType.RechargeDataXml))
                  {
                      using (var writer = File.CreateText(outputFilePath))
                      {
                          writer.Write(sourceFile);
                          writer.Close();
                      } // End using
                  }
                  else if (fileFormatType.Equals(FileFormatType.LegalInvoiceXml))
                  {
                      var sourceFilePath = Path.Combine(sourceFile, fileName);
                      /* SCP 245276 - SIS Legal Invoice Zip file has not been generating since October 2013
                       * Description:Move is changed to Copy, once copied complete temp folder (invoice legal xml and legal xml zip) is deleted. 
                       *              Seperate code is added for deletion purpose.
                       */
                      File.Copy(sourceFilePath, outputFilePath);
                  }

                  Logger.InfoFormat("File is created at path: {0} ", outputFilePath);

                  // Check if entry for recharge data file exists in FILE LOG table.
                  var file = IsInputFileRep.First(f => f.FileName.Trim().ToLower().Contains(fileName.Trim().ToLower()));

                  // If file exists then update details
                  if (file != null)
                  {
                      Logger.DebugFormat("{0} file exists in FILE_LOG table. Update details.", fileName);
                      file.FileDate = DateTime.UtcNow;
                      file.ReceivedDate = DateTime.UtcNow;
                      file.SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString();
                      file.FileStatus = FileStatusType.PushedToDestination;
                      file.FileFormat = fileFormatType;
                  } // End if
                      // Else add new entry to FILE LOG table. 
                  else
                  {
                      Logger.DebugFormat("Adding {0} file to FILE_LOG table.", fileName);

                      IsInputFileRep.Add(new IsInputFile
                                             {
                                                 FileName = fileName,
                                                 SenderRecieverType = (int) FileSenderRecieverType.IATA,
                                                 FileDate = DateTime.UtcNow,
                                                 ReceivedDate = DateTime.UtcNow,
                                                 IsIncoming = true,
                                                 SenderReceiverIP =
                                                     Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                                                 FileStatus = FileStatusType.PushedToDestination,
                                                 FileFormat = fileFormatType,
                                                 FileLocation = Path.GetDirectoryName(outputFilePath)
                                             });
                  } // End else

                  // Save changes to DB.
                  UnitOfWork.CommitDefault();

                  Logger.DebugFormat("File details added.");

                  return true;
              } // End if
              else
              {
                  Logger.Info(string.Format("IATA FTP Download Path: {0} not found.", ftpPathIata));
              }

              return false;
          } // End try

          catch (Exception ex)
          {
              Logger.Error(ex);
              throw ex;
          } // End catch

      }// End UploadFileForFTPPull()

        /// <summary>
        /// To send the ValidationException CSV to the billing member.
        /// </summary>
        public bool SendValidationExceptionNotification(string zipFilePath, string parsedFileName, int memberId, ProcessingContactType processingContact, string sisOpsMail)
        {
            var message = new MailComponent.MailMessage();
            try
            {
                var contactTypeList = GetToEmailIds(memberId, processingContact);

                var toMailIds = new StringBuilder();
                var index = 0;

                if (contactTypeList != null)
                {
                    foreach (var contact in contactTypeList)
                    {
                        index += 1;
                        toMailIds.Append(index != contactTypeList.Count ? string.Format("{0}{1}", contact.EmailAddress, ",") : contact.EmailAddress);
                    }
                }

                if (!string.IsNullOrEmpty(toMailIds.ToString()))
                {
                    message.To.Add(toMailIds.ToString());
                }
                else
                {
                    Logger.Info("Email address is not available for the user, so not able to send the message");
                    return false;
                }

                var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
                var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

                var emailSettingParsingValidationExceptionNotification = emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.ParsingValidationExceptionNotification);
                message.From = new MailComponent.MailAddress(emailSettingParsingValidationExceptionNotification.SingleOrDefault().FromEmailAddress, "SIS No Reply");
                message.Subject = emailSettingParsingValidationExceptionNotification.SingleOrDefault().Subject; ;// string.Format("Your invoice files are ready for download. [File: {0}]", Path.GetFileName(outputZipFileName));

                var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
                var context = new NVelocity.VelocityContext();
                if (contactTypeList != null)
                {
                    context.Put("Message",
                                new ParsedValidationExceptionNotification()
                                  {
                                      RecipientName = string.IsNullOrEmpty(contactTypeList[0].FirstName) ? string.Empty : contactTypeList[0].FirstName,
                                      ParsedFileName = Path.GetFileName(parsedFileName),
                                      FileLocation = Path.GetFileName(zipFilePath)
                                  });

                    if (sisOpsMail != null)
                        context.Put("SISOpsEmailId", sisOpsMail);
                    else
                        context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                }
                message.Body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ParsingValidationExceptionNotification, context);
                //message.Body = string.Format("The downloadable zip file requested is ready for download at {0}", outputZipFileName);

                /*if (File.Exists(exceptionFilePath))
                {
                  MailComponent.Attachment attach = new MailComponent.Attachment(exceptionFilePath);
                  message.Attachments.Add(attach);
                }
                if (File.Exists(summaryFilePath))
                {
                  MailComponent.Attachment attach = new MailComponent.Attachment(summaryFilePath);
                  message.Attachments.Add(attach);
                }*/


                emailSender.Send(message);
            }
            catch (Exception ex)
            {
                Logger.Error("Failed sending the mail", ex);
                message.Dispose();
                return false;
            }

            return true;
        }

        /// <summary>
        /// To get the Contact list for memeberId for specified ProcessingContactType.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="processingContact"></param>
        /// <returns></returns>
        private List<Contact> GetToEmailIds(int memberId, ProcessingContactType processingContact)
        {
            var toMailIds = new StringBuilder();
            var index = 0;
            var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
            List<Contact> contactTypeList = memberManager.GetContactsForContactType(memberId, processingContact);
            return contactTypeList;
        }

        public bool FileWatcherLog(FileWatcher fileWatcher)
        {
            bool returnValue = false;
            try
            {
                if (fileWatcher != null)
                {
                    FileWatcherRepository.Add(fileWatcher);
                    UnitOfWork.CommitDefault();
                    returnValue = true;
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Failed to add file watcher log", exception);
                returnValue = false;
            }

            return returnValue;
        }




        public IQueryable<FileWatcher> GetFilesByFileName(string fileName)
        {
            var filewatcher = FileWatcherRepository.Get(m => m.FileName == fileName);
            return filewatcher;
        }

        public IQueryable<FileWatcher> GetFilesByStatus(string status)
        {
            var filewatcher = FileWatcherRepository.Get(m => m.Status == status);
            return filewatcher;
        }

        public bool UpdateFileWatcherStatus(int fileWatcherId, string status)
        {
            bool returnValue = false;
            try
            {
                var filewatcher = FileWatcherRepository.Get(m => m.Id == fileWatcherId).SingleOrDefault();

                filewatcher.Status = status;

                filewatcher.LastUpdatedOn = DateTime.UtcNow;

                FileWatcherRepository.Update(filewatcher);
                UnitOfWork.CommitDefault();
                returnValue = true;
            }
            catch (Exception exception)
            {
                returnValue = false;

            }
            return returnValue;

        }

        public IList<IsInputFile> GetIsFileLogByStatus(int status)
        {

            //Add entry in is-file log                
            var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

            var isInputFile = referenceManager.GetIsInputFileByStatus(status);

            return isInputFile.ToList();
        }

        public bool InsertIsFileLog(FileWatcher fileWatcher)
        {
            bool returnValue = false;
            try
            {


                //Add entry in is-file log                
                var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

                // Check Sender Receiver Type

                int? memberId = MemberRepository.GetMemberId(fileWatcher.SourceId);
                int senderReceivertype = 0;

                if (memberId > 0)
                {
                    senderReceivertype = 1;
                }
                else
                {
                    memberId = null;
                    switch (fileWatcher.SourceId)
                    {
                        case "ATPCO":
                            senderReceivertype = (int)FileSenderRecieverType.ATPCO;
                            break;

                        case "ACH":
                            senderReceivertype = (int)FileSenderRecieverType.ACH;
                            break;
                        case "ICH":
                            senderReceivertype = (int)FileSenderRecieverType.ICH;
                            break;
                        case "IATA":
                            senderReceivertype = (int)FileSenderRecieverType.IATA;
                            break;


                    }

                }

                var isInputFile = new IsInputFile()
                {
                    FileDate = fileWatcher.FileModifiedDateTime,
                    FileLocation = Path.GetDirectoryName(fileWatcher.FilePath) + @"\",
                    FileName = fileWatcher.FileName,
                    FileStatus = FileStatusType.Received,
                    SenderReceiver = memberId,
                    IsIncoming = true,
                    BillingCategory = null,
                    ReceivedDate = DateTime.UtcNow,
                    SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                    SenderRecieverType = senderReceivertype

                };
                referenceManager.AddIsInputFile(isInputFile);

                returnValue = true;
            }
            catch (Exception exception)
            {
                returnValue = false;
                Logger.Error("Failed to Insert IS_FILE_LOG Entry", exception);
            }

            return returnValue;
        }

        /// <summary>
        /// FTP File sender main function.
        /// </summary>
        /// <returns></returns>
        public bool FtpFileSender()
        {
          //SCP357374: SRM: One FTP thread stuck - 9APR2015
          try
          {
            Logger.Info("File Sender Triggered.");
            try
            {
                Ioc.Initialize();
            }
            catch (Exception exception)
            {
                Logger.Error("Error in File Sender Ioc.Initialize()" + exception.Message);
                return false;
            }

            Logger.Info("File Sender Ioc.Initialize()");

            //Update all Files Lying with error status to status 9, which means pending. So that they can be resent.
            UpdateIsFileLogStatus();
            
            var isFileTypeConfigured = 0;
            try
            {
                isFileTypeConfigured = Convert.ToInt32(ConfigurationManager.AppSettings["IsFileType"]);
            }
            catch (Exception)
            {
                Logger.Error("Application is not configured to work on specific File Types. So working on all file types, which is the default behaiour.");
            }
            
            const int lMaxAttempt = 15; //SystemParameters.Instance.General.FtpFileUploadMaxAttempt;
            string sIinetFolderName = string.Empty;
            string sIinetServerName = string.Empty;
            string sIinetUserName = string.Empty;
            string sIinetPassword = string.Empty;
            int lIinetPort = 0;
            string sIinetSecurity = string.Empty;
            //CMP#622: MISC Outputs Split as per Location ID
            bool isLocationSpecific = false;
            bool isLocAccountAvailable = false;
            bool isMainAccountAvailable = false;
            int memberLocationId = 0 ;
            IList<IsInputFile> isInputFile = null;
            isInputFile = GetFileListByFileType(isFileTypeConfigured);

            if (isInputFile != null)
            {
                sIinetFolderName = SystemParameters.Instance.iiNet.iiNetFolderName.ToString();
                sIinetServerName = SystemParameters.Instance.iiNet.ServerName.ToString();
                sIinetUserName = SystemParameters.Instance.iiNet.UserName.ToString();
                sIinetPassword = SystemParameters.Instance.iiNet.Password.ToString();
                try
                {
                    lIinetPort = (int) SystemParameters.Instance.iiNet.Port;
                }
                catch (Exception)
                {
                    lIinetPort = 0;
                }

                sIinetSecurity = SystemParameters.Instance.iiNet.Security.ToString();

                foreach (IsInputFile sourceFile in isInputFile)
                {
                    // Push the File To FTP Server
                    if (sourceFile.FileLocation == null) sourceFile.FileLocation = string.Empty;
                    Logger.Info("Upload File To iiNet FileName : " + sourceFile.FileName);
                    ConnectionInfo connInfo = null;
                    // sourceFile.SenderRecieverType = Member ID
                    switch (sourceFile.SenderRecieverType)
                    {
                        case (int) FileSenderRecieverType.Member:

                            var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

                            if (sourceFile.SenderReceiver != null)
                            {
                                var fileSenderReceiverId = sourceFile.SenderReceiver.Value;
                                // get Member Technical Configuration
                                var memberTechnicalConfig = memberManager.GetMemberTechnicalConfig(sourceFile.SenderReceiver.Value);
                                
                                //CMP#622: MISC Outputs Split as per Location ID
                                if (isFileTypeConfigured == 5 || isFileTypeConfigured == 6 || !string.IsNullOrWhiteSpace(sourceFile.MiscLocationCode))
                                {
                                    isLocationSpecific = true;
                                }
                                else
                                {
                                    isLocationSpecific = false;
                                    isLocAccountAvailable = false;
                                    isMainAccountAvailable = false;
                                }
                                ArrayList accountIdArrayList = new ArrayList();
                                if (isLocationSpecific)
                                {
                                    accountIdArrayList = GetLocationAccountIds(sourceFile, memberTechnicalConfig, isFileTypeConfigured, out isLocAccountAvailable, out isMainAccountAvailable, out memberLocationId);
                                    Logger.InfoFormat("LocAccountAvailable : {0}, MainAccountAvailable {1}", isLocAccountAvailable, isMainAccountAvailable);
                                }
                                if (memberTechnicalConfig != null || accountIdArrayList.Count > 0)
                                {
                                    //if (memberTechnicalConfig.PaxAccountId != null ||
                                    //    memberTechnicalConfig.MiscAccountId != null ||
                                    //    memberTechnicalConfig.CgoAccountId != null ||
                                    //    memberTechnicalConfig.UatpAccountId != null)
                                    //    {
                                    if (sourceFile.BillingCategory != null)
                                    {
                                        switch ((int) sourceFile.BillingCategory)
                                        {
                                            case (int) BillingCategoryType.Pax:
                                                //below condition added on 23.03.12 after discusion with Robin
                                                if (memberTechnicalConfig.PaxAccountId != null)
                                                {
                                                    // Create Connection object 
                                                    connInfo = new ConnectionInfo
                                                                   {
                                                                       ServerName = sIinetServerName,
                                                                       UserName = sIinetUserName,
                                                                       Password = sIinetPassword,
                                                                       Port = lIinetPort,
                                                                       Security = sIinetSecurity,
                                                                       IsNormalFTP = true,
                                                                       AcceptAllCertificates = true
                                                                   };

                                                    // FTP File Upload Max attemp = 3, in case any error in FTP connection error

                                                    for (int i = 0; i <= lMaxAttempt - 1; i++)
                                                    {
                                                        Logger.Info(string.Format("iiNET FTP Upload attempt : {0} /MaxAttempts {1}", i + 1, lMaxAttempt));
                                                        if (UploadFileToiiNet(connInfo,
                                                                              sourceFile.FileLocation,
                                                                              sourceFile.FileName,
                                                                              sIinetFolderName,
                                                                              memberTechnicalConfig.PaxAccountId,
                                                                              sourceFile.Id))
                                                        {
                                                            UploadFileFailureAttempt = 0;
                                                            // Send Email notification to Member 
                                                            Logger.Info("File uploaded to iiNET: " + sourceFile.FileName);
                                                            Logger.Info("Get member contacts for acknowledgement notification - Start");
                                                            var outputAvailableContact = GetMemberContactsForFileType(Convert.ToInt32(sourceFile.SenderReceiver),
                                                                                                                      BillingCategoryType.Pax,
                                                                                                                      sourceFile.FileFormat);
                                                            Logger.Info("Get member contacts for acknowledgement notification - End");
                                                            //memberManager.GetContactsForContactType(
                                                            //    (int) sourceFile.SenderReceiver,
                                                            //    ProcessingContactType.PAXOutputAvailableAlert);
                                                            if (outputAvailableContact != null)
                                                            {
                                                                Logger.Info("Total PAXOutputAvailableAlert Found :" + outputAvailableContact.Count);
                                                                
                                                                //Send the email and alert notifications to member
                                                                SendMemberFileTransferNotification(sourceFile.FileName, outputAvailableContact, sourceFile.FileFormat);


                                                            }
                                                            else
                                                            {
                                                                Logger.Info("PAXOutputAvailableAlert Not Found ");
                                                            }
                                                            Logger.Info("Update file status in is file log - Start");
                                                            // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                            UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                                            Logger.Info("Update file status in is file log - End");
                                                            break;
                                                        }
                                                    }

                                                    if (lMaxAttempt == UploadFileFailureAttempt)
                                                    {

                                                        // send Email to SIS Ops about File Upload Failure
                                                        AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                                        UploadFileFailureAttempt = 0;
                                                    }
                                                    //else
                                                    //{
                                                    //    // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                    //    UpdateIsFileLog(sourceFile.Id,
                                                    //                    FileStatusType.PushedToDestination);
                                                    //}
                                                }
                                                else
                                                {
                                                    UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                                }

                                                break;

                                            case (int) BillingCategoryType.Cgo:
                                                // Send File To iiNET FTP folder for Cargo
                                                //below condition added on 23.03.12 after discusion with Robin
                                                if (memberTechnicalConfig.CgoAccountId != null)
                                                {
                                                    connInfo = new ConnectionInfo
                                                                   {
                                                                       ServerName = sIinetServerName,
                                                                       UserName = sIinetUserName,
                                                                       Password = sIinetPassword,
                                                                       Port = lIinetPort,
                                                                       Security = sIinetSecurity,
                                                                       IsNormalFTP = true,
                                                                       AcceptAllCertificates = true
                                                                   };

                                                    for (int i = 0; i <= lMaxAttempt - 1; i++)
                                                    {
                                                        Logger.Info(string.Format("iiNET FTP Upload attempt : {0} /MaxAttempts {1}", i + 1, lMaxAttempt));

                                                        if (UploadFileToiiNet(connInfo,
                                                                              sourceFile.FileLocation,
                                                                              sourceFile.FileName,
                                                                              sIinetFolderName,
                                                                              memberTechnicalConfig.CgoAccountId,
                                                                              sourceFile.Id))
                                                        {
                                                            UploadFileFailureAttempt = 0;
                                                            Logger.Info("UploadFileToiiNet FileName: " + sourceFile.FileName);
                                                            var outputAvailableContact = GetMemberContactsForFileType(Convert.ToInt32(sourceFile.SenderReceiver),
                                                                                                                      BillingCategoryType.Cgo,
                                                                                                                      sourceFile.FileFormat);

                                                            //var outputAvailableContact =
                                                            //    memberManager.GetContactsForContactType(
                                                            //        (int) sourceFile.SenderReceiver,
                                                            //        ProcessingContactType.CGOOutputAvailableAlert);
                                                            if (outputAvailableContact != null)
                                                            {
                                                                Logger.Info("Total CGOOutputAvailableAlert Found :" + outputAvailableContact.Count);
                                                                SendMemberFileTransferNotification(sourceFile.FileName, outputAvailableContact, sourceFile.FileFormat);
                                                            }
                                                            else
                                                            {
                                                                Logger.Info("CGOOutputAvailableAlert Not Found ");
                                                            }
                                                            // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                            UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                                            break;
                                                        }
                                                    }

                                                    if (lMaxAttempt == UploadFileFailureAttempt)
                                                    {

                                                        // send Email to SIS Ops about File Upload Failure
                                                        AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                                        UploadFileFailureAttempt = 0;
                                                    }
                                                    //else
                                                    //{
                                                    //    // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                    //    UpdateIsFileLog(sourceFile.Id,
                                                    //                    FileStatusType.PushedToDestination);
                                                    //}

                                                }
                                                else
                                                {
                                                    UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                                }



                                                break;

                                            case (int) BillingCategoryType.Misc:

                                                
                                                //CMP#622: MISC Outputs Split as per Location ID
                                                if (!isLocationSpecific)
                                                {
                                                    #region Non Location Specific

                                                    // Send File To iiNET FTP folder for Misc
                                                    //below condition added on 23.03.12 after discusion with Robin
                                                    if (memberTechnicalConfig.MiscAccountId != null)
                                                    {
                                                        connInfo = new ConnectionInfo
                                                        {
                                                            ServerName = sIinetServerName,
                                                            UserName = sIinetUserName,
                                                            Password = sIinetPassword,
                                                            Port = lIinetPort,
                                                            Security = sIinetSecurity,
                                                            IsNormalFTP = true,
                                                            AcceptAllCertificates = true
                                                        };

                                                        for (int i = 0; i <= lMaxAttempt - 1; i++)
                                                        {
                                                            Logger.Info(string.Format("iiNET FTP Upload attempt : {0} /MaxAttempts {1}", i + 1, lMaxAttempt));
                                                            if (UploadFileToiiNet(connInfo,
                                                                                  sourceFile.FileLocation,
                                                                                  sourceFile.FileName,
                                                                                  sIinetFolderName,
                                                                                  memberTechnicalConfig.MiscAccountId,
                                                                                  sourceFile.Id))
                                                            {
                                                                UploadFileFailureAttempt = 0;
                                                                Logger.Info("UploadFileToiiNet FileName: " + sourceFile.FileName);
                                                                //CMP#655IS-WEB Display per Location
                                                                var outputAvailableContact = GetMemberContactsForFileType(Convert.ToInt32(sourceFile.SenderReceiver),
                                                                                                                          BillingCategoryType.Misc,
                                                                                                                          sourceFile.FileFormat,sourceFile.MiscLocationCode);

                                                                //var outputAvailableContact =
                                                                //    memberManager.GetContactsForContactType(
                                                                //        (int) sourceFile.SenderReceiver,
                                                                //        ProcessingContactType.MISCOutputAvailableAlert);
                                                                if (outputAvailableContact != null)
                                                                {
                                                                    Logger.Info("Total MISCOutputAvailableAlert Found : " + outputAvailableContact.Count);
                                                                    SendMemberFileTransferNotification(sourceFile.FileName, outputAvailableContact, sourceFile.FileFormat);
                                                                }
                                                                else
                                                                {
                                                                    Logger.Info("MISCOutputAvailableAlert Not Found ");
                                                                }
                                                                // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                                UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                                                break;
                                                            }
                                                        }
                                                        
                                                        if (lMaxAttempt == UploadFileFailureAttempt)
                                                        {

                                                            // send Email to SIS Ops about File Upload Failure
                                                            AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                                            UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                                            UploadFileFailureAttempt = 0;
                                                        }
                                                        //else
                                                        //{
                                                        //    // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                        //    UpdateIsFileLog(sourceFile.Id,
                                                        //                    FileStatusType.PushedToDestination);
                                                        //}

                                                    }
                                                    else
                                                    {
                                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                                    }

                                                    #endregion
                                                 
                                                }
                                                else
                                                {
                                                    //CMP#622: MISC Outputs Split as per Location ID
                                                    #region Misc Location Specific
                                                    if (accountIdArrayList.Count > 0)
                                                    {
                                                        connInfo = new ConnectionInfo
                                                        {
                                                            ServerName = sIinetServerName,
                                                            UserName = sIinetUserName,
                                                            Password = sIinetPassword,
                                                            Port = lIinetPort,
                                                            Security = sIinetSecurity,
                                                            IsNormalFTP = true,
                                                            AcceptAllCertificates = true
                                                        };

                                                        for (int i = 0; i <= lMaxAttempt - 1; i++)
                                                        {
                                                            Logger.Info(string.Format("iiNET FTP Upload attempt : {0} /MaxAttempts {1}", i + 1, lMaxAttempt));
                                                            if (UploadFileToiiNetForAllAccountId(connInfo,
                                                                                  sourceFile,
                                                                                  sIinetFolderName,
                                                                                  accountIdArrayList))
                                                            {
                                                                UploadFileFailureAttempt = 0;
                                                                
                                                                Logger.Info("UploadFileToiiNet FileName: " + sourceFile.FileName);
                                                                var memberContact = GetMemberContactsForFileType(Convert.ToInt32(sourceFile.SenderReceiver),
                                                                                                                          BillingCategoryType.Misc,
                                                                                                                          sourceFile.FileFormat);
                                                                

                                                                var locationContact = new List<Contact>();
                                                                var mainLocationContact = new List<Contact>();

                                                                if (isLocAccountAvailable)
                                                                {
                                                                    locationContact = memberContact.Where(contact => contact.LocationId == memberLocationId).ToList();
                                                                }
                                                                if(isMainAccountAvailable)
                                                                {
                                                                    var locationRepository = Ioc.Resolve<ILocationRepository>(typeof(ILocationRepository));
                                                                    var memberlocation =
                                                                        locationRepository.Get(
                                                                            memberLocation =>
                                                                            memberLocation.MemberId == fileSenderReceiverId && memberLocation.IsActive &&
                                                                            memberLocation.LocationCode.ToUpper() == "MAIN").FirstOrDefault();
                                                                    if (memberlocation != null)
                                                                    {
                                                                        mainLocationContact = memberContact.Where(contact => contact.LocationId == memberlocation.Id).ToList();
                                                                    }
                                                                }
                                                                var outputAvailableContact = locationContact.Union(mainLocationContact).ToList();
                                                                //var outputAvailableContact =
                                                                //    memberManager.GetContactsForContactType(
                                                                //        (int) sourceFile.SenderReceiver,
                                                                //        ProcessingContactType.MISCOutputAvailableAlert);))
                                                                if (outputAvailableContact != null)
                                                                {
                                                                    Logger.Info("Total MISCOutputAvailableAlert Found : " + outputAvailableContact.Count);
                                                                    SendMemberFileTransferNotification(sourceFile.FileName, outputAvailableContact, sourceFile.FileFormat);
                                                                }
                                                                else
                                                                {
                                                                    Logger.Info("MISCOutputAvailableAlert Not Found ");
                                                                }
                                                                // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                                UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                                                break;
                                                            }
                                                        }

                                                        if (lMaxAttempt == UploadFileFailureAttempt)
                                                        {

                                                            // send Email to SIS Ops about File Upload Failure
                                                            AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                                            UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                                            UploadFileFailureAttempt = 0;
                                                        }
                                                        //else
                                                        //{
                                                        //    // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                        //    UpdateIsFileLog(sourceFile.Id,
                                                        //                    FileStatusType.PushedToDestination);
                                                        //} 

                                                    }
                                                    else
                                                    {
                                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound,true);
                                                    }

                                                    #endregion
                                                }

                                                
                                                break;

                                            case (int) BillingCategoryType.Uatp:

                                                // Send File To iiNET FTP folder for UATP
                                                //below condition added on 23.03.12 after discusion with Robin
                                                if (memberTechnicalConfig.UatpAccountId != null)
                                                {
                                                    connInfo = new ConnectionInfo
                                                                   {
                                                                       ServerName = sIinetServerName,
                                                                       UserName = sIinetUserName,
                                                                       Password = sIinetPassword,
                                                                       Port = lIinetPort,
                                                                       Security = sIinetSecurity,
                                                                       IsNormalFTP = true,
                                                                       AcceptAllCertificates = true
                                                                   };

                                                    for (int i = 0; i <= lMaxAttempt - 1; i++)
                                                    {
                                                        Logger.Info(string.Format("iiNET FTP Upload attempt : {0} /MaxAttempts {1}", i + 1, lMaxAttempt));
                                                        if (UploadFileToiiNet(connInfo,
                                                                              sourceFile.FileLocation,
                                                                              sourceFile.FileName,
                                                                              sIinetFolderName,
                                                                              memberTechnicalConfig.UatpAccountId,
                                                                              sourceFile.Id))
                                                        {
                                                            UploadFileFailureAttempt = 0;
                                                            Logger.Info("UploadFileToiiNet FileName: " + sourceFile.FileName);

                                                            var outputAvailableContact = GetMemberContactsForFileType(Convert.ToInt32(sourceFile.SenderReceiver),
                                                                                                                      BillingCategoryType.Uatp,
                                                                                                                      sourceFile.FileFormat);

                                                            //var outputAvailableContact =
                                                            //    memberManager.GetContactsForContactType(
                                                            //        (int) sourceFile.SenderReceiver,
                                                            //        ProcessingContactType.UATPOutputAvailableAlert);
                                                            if (outputAvailableContact != null)
                                                            {
                                                                Logger.Info("Total UATPOutputAvailableAlert Found : " + outputAvailableContact.Count);
                                                                SendMemberFileTransferNotification(sourceFile.FileName, outputAvailableContact, sourceFile.FileFormat);
                                                            }
                                                            else
                                                            {
                                                                Logger.Info("UATPOutputAvailableAlert Not Found ");
                                                            }

                                                            // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                            UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                                            break;
                                                        }
                                                    }

                                                    if (lMaxAttempt == UploadFileFailureAttempt)
                                                    {

                                                        // send Email to SIS Ops about File Upload Failure
                                                        AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                                        UploadFileFailureAttempt = 0;
                                                    }
                                                    //else
                                                    //{
                                                    //    // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                    //    UpdateIsFileLog(sourceFile.Id,
                                                    //                    FileStatusType.PushedToDestination);
                                                    //}

                                               }
                                                else
                                                {
                                                    UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                                }


                                                break;

                                        }
                                    }
                                    else
                                    {
                                        // If Billing Category Null


                                        // Get all billing category Account ID

                                        var accountIdArray = new ArrayList();

                                        if (!string.IsNullOrEmpty(memberTechnicalConfig.PaxAccountId))
                                        {
                                            if (!accountIdArray.Contains(memberTechnicalConfig.PaxAccountId)) accountIdArray.Add(memberTechnicalConfig.PaxAccountId);
                                        }
                                        if (!string.IsNullOrEmpty(memberTechnicalConfig.CgoAccountId))
                                        {
                                            if (!accountIdArray.Contains(memberTechnicalConfig.CgoAccountId)) accountIdArray.Add(memberTechnicalConfig.CgoAccountId);
                                        }
                                        if (!string.IsNullOrEmpty(memberTechnicalConfig.MiscAccountId))
                                        {
                                            if (!accountIdArray.Contains(memberTechnicalConfig.MiscAccountId)) accountIdArray.Add(memberTechnicalConfig.MiscAccountId);
                                        }

                                        if (!string.IsNullOrEmpty(memberTechnicalConfig.UatpAccountId))
                                        {
                                            if (!accountIdArray.Contains(memberTechnicalConfig.UatpAccountId)) accountIdArray.Add(memberTechnicalConfig.UatpAccountId);
                                        }

                                        //below condition added on 23.03.12 after discusion with Robin
                                        if (accountIdArray.Count != 0)
                                        {
                                            connInfo = new ConnectionInfo
                                                           {
                                                               ServerName = sIinetServerName,
                                                               UserName = sIinetUserName,
                                                               Password = sIinetPassword,
                                                               Port = lIinetPort,
                                                               Security = sIinetSecurity,
                                                               IsNormalFTP = true,
                                                               AcceptAllCertificates = true
                                                           };

                                            for (int i = 0; i <= lMaxAttempt - 1; i++)
                                            {

                                                if (UploadFileToiiNetForAllAccountId(connInfo, sourceFile, sIinetFolderName, accountIdArray))
                                                {
                                                    Logger.Info("UploadFileToiiNet FileName: " + sourceFile.FileName);
                                                    EmailSendAllBillingCategory(sourceFile.SenderReceiver, sourceFile.FileName, sourceFile.FileFormat);
                                                    break;
                                                }
                                            }

                                            if (lMaxAttempt == UploadFileFailureAttempt)
                                            {

                                                // send Email to SIS Ops about File Upload Failure
                                                AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                                UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                                UploadFileFailureAttempt = 0;
                                            }
                                            else
                                            {
                                                // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                                UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                            }

                                        }
                                        else
                                        {
                                            UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                        }

                                    }

                                }
                                else
                                {
                                    UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                }
                            }
                            else // ********************CMP597: code for Weekly Reference Data and Contact Data***********************
                            {
                                Logger.Info("Get Data from IS_FILE_LOG_EXT1 table");
                                // Get Data from IS_FILE_LOG_EXT1 table 
                                var isFileLogExt1 = IsFileLogExt1Repository.Get(fl => fl.IsFileLogId == sourceFile.Id && (fl.IinetUploadStatus == "P" || fl.IinetUploadStatus == "F")).ToList();

                                UploadFileFailureAttempt = 0;

                                Logger.Info("Sending the file (in zipped format) to all iiNET Account IDs");
                                // Send the file (in zipped format) to all iiNET Account IDs implementing following logic
                                if (isFileLogExt1.Count != 0)
                                {

                                    connInfo = new ConnectionInfo
                                                   {
                                                       ServerName = sIinetServerName,
                                                       UserName = sIinetUserName,
                                                       Password = sIinetPassword,
                                                       Port = lIinetPort,
                                                       Security = sIinetSecurity,
                                                       IsNormalFTP = true,
                                                       AcceptAllCertificates = true
                                                   };


                                    for (int i = 0; i <= lMaxAttempt - 1; i++)
                                    {
                                        if (UploadReferenceContacttoiiNet(connInfo, sourceFile, sIinetFolderName))
                                        {
                                            Logger.Info("UploadedFileToiiNet FileName: " + sourceFile.FileName);
                                            _counterTdfFileForReferenceData = 1;
                                            foreach (var ext1Model in isFileLogExt1)
                                            {
                                                var accArray = new ArrayList();
                                                var contactArray = new ArrayList();
                                                if (!string.IsNullOrEmpty(ext1Model.AccountIdLIst)) accArray.AddRange(ext1Model.AccountIdLIst.Split(new char[] { ',' }));

                                                if (!string.IsNullOrEmpty(ext1Model.ContactIdLIst)) contactArray.AddRange(ext1Model.ContactIdLIst.Split(new char[] { ',' }));

                                                if (CreateTdfFileForAllAccountId(sourceFile.FileName, accArray, true))
                                                {
                                                    _counterTdfFileForReferenceData += 1;

                                                    if (EmailSendForMemberProfileDataFiles(sourceFile, contactArray))
                                                    {
                                                        Logger.Info("Sending Email to Members Completed for FileName: " + sourceFile.FileName);
                                                    }

                                                    if (ext1Model.IinetUploadStatus == "F") //if file sending is failed periviously and now success then send intimation alert to sis support
                                                    {
                                                        if (EmailSendToSisSupport(sourceFile, contactArray))
                                                        {
                                                            Logger.Info("Sending Email to SIS Supoort Completed for FileName: " + sourceFile.FileName);
                                                        }
                                                    }
                                                    // Finaly Update the file status
                                                    ext1Model.IinetUploadStatus = "S";
                                                }
                                                else
                                                {
                                                    ext1Model.IinetUploadStatus = "F";
                                                }
                                            }
                                        }


                                        var objIsFtpLog = new IsftpLog
                                                              { IsfileLogId = sourceFile.Id, LogText = RichTextBoxLogWriter.IsFtpLog, TdfFileContent = TdfFileContent, LastUpdatedOn = DateTime.UtcNow };

                                        AddIsFtpLog(objIsFtpLog);
                                        RichTextBoxLogWriter.IsFtpLog = string.Empty;
                                        TdfFileContent = string.Empty;

                                        // File has been uploaded to iiNET. Now out of the loop.
                                        break;
                                    }
                                    if (lMaxAttempt == UploadFileFailureAttempt)
                                    {
                                        // send Email to SIS Ops about File Upload Failure
                                        AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), null);

                                    }


                                    //Check here if all the records in IS_FILE_LOG_EXT1 table related to IS_File_Log_id has status "S" means file sending successful otherwise file sending failed 
                                    if (isFileLogExt1.Where(c => c.IsFileLogId == sourceFile.Id && c.IinetUploadStatus != "S").Count() != 0)
                                    {
                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                        UpdateIsFileLogExt1(isFileLogExt1);
                                        UploadFileFailureAttempt = 0;
                                    }
                                    else
                                    {
                                        // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                        UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                                        UpdateIsFileLogExt1(isFileLogExt1);
                                        UploadFileFailureAttempt = 0;
                                    }

                                }
                                else
                                {
                                    UpdateIsFileLog(sourceFile.Id, FileStatusType.iiNetRecipientNotFound);
                                }
                            }

                            break;

                        case (int) FileSenderRecieverType.ACH:

                            var tempFileName = ConfigurationManager.AppSettings["SourceFileName"].ToString();

                            FileIo.RenameFile(sourceFile.FileLocation + @"\" + sourceFile.FileName, tempFileName);

                            sourceFile.FileName = tempFileName;

                            var achConnection = new AchConnection
                                                    {
                                                        // CertificatePath = SystemParameters.Instance.SisSslCertificate.CertificatePath,
                                                        // CertificatePassword = SystemParameters.Instance.SisSslCertificate.CertificatePassword,
                                                        Host = SystemParameters.Instance.Ach.ServerName,
                                                        Login = SystemParameters.Instance.Ach.UserName,
                                                        Password = SystemParameters.Instance.Ach.Password,
                                                        Port = SystemParameters.Instance.Ach.Port,
                                                        FtpWorkingDirectory = SystemParameters.Instance.Ach.FtpWorkingDirectory
                                                    };

                            for (int i = 0; i <= lMaxAttempt - 1; i++)
                            {
                                if (UploadFileToAchFtp(sourceFile.FileLocation, sourceFile.FileName, achConnection, sourceFile.Id)) break;
                            }

                            if (lMaxAttempt == UploadFileFailureAttempt)
                            {

                                // send Email to SIS Ops about File Upload Failure
                                AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                UploadFileFailureAttempt = 0;
                            }
                            else
                            {
                                // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                            }
                            
                            //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                            achConnection = null;

                            break;

                        case (int) FileSenderRecieverType.ICH:

                            connInfo = new ConnectionInfo
                                           {
                                               /* ServerName = SystemParameters.Instance.Ich.ServerName,
                                                UserName = SystemParameters.Instance.Ich.UserName,
                                                Password = SystemParameters.Instance.Ich.Password,
                                                Port = SystemParameters.Instance.Ich.Port,
                                                Security = SystemParameters.Instance.Ich.Security,*/
                                               AcceptAllCertificates = true,
                                               IsNormalFTP = false
                                           };
                            for (int i = 0; i <= lMaxAttempt - 1; i++)
                            {
                                if (UploadFileToFtp(connInfo, sourceFile.FileLocation, sourceFile.FileName, string.Empty, sourceFile.Id)) break;
                            }

                            if (lMaxAttempt == UploadFileFailureAttempt)
                            {

                                // send Email to SIS Ops about File Upload Failure
                                AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, sourceFile.FileName), sourceFile.SenderReceiver);
                                UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                UploadFileFailureAttempt = 0;
                            }
                            else
                            {
                                // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                            }

                            break;

                        case (int) FileSenderRecieverType.ATPCO:

                            connInfo = new ConnectionInfo
                                           {
                                               ServerName = SystemParameters.Instance.Atpco.ServerName,
                                               UserName = SystemParameters.Instance.Atpco.UserName,
                                               Password = SystemParameters.Instance.Atpco.Password,
                                               Port = SystemParameters.Instance.Atpco.Port,
                                               Security = SystemParameters.Instance.Atpco.Security,
                                               AcceptAllCertificates = true,
                                               IsNormalFTP = false
                                           };


                            Logger.Info("ATPCO FTP Details : " + connInfo.ServerName + " " + connInfo.UserName + " " + connInfo.Port);

                            // Rename file Name
                            string logFileName = sourceFile.FileName;
                            string[] splitArray = logFileName.Split(Convert.ToChar("_"));
                            string newFileName = splitArray[0];

                            FileIo.RenameFile(sourceFile.FileLocation + @"\" + sourceFile.FileName, newFileName);

                            for (int i = 0; i <= lMaxAttempt - 1; i++)
                            {
                                if (UploadFileToFtp(connInfo, sourceFile.FileLocation, newFileName, string.Empty, sourceFile.Id)) break;
                            }

                            if (lMaxAttempt == UploadFileFailureAttempt)
                            {

                                // send Email to SIS Ops about File Upload Failure
                                AddIssisOpsAlert(Path.Combine(sourceFile.FileLocation, logFileName), sourceFile.SenderReceiver);
                                UpdateIsFileLog(sourceFile.Id, FileStatusType.ErrorInFileSend);
                                UploadFileFailureAttempt = 0;
                            }
                            else
                            {
                                // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                                UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);
                            }

                            break;

                        case (int) FileSenderRecieverType.IATA:
                            // Update IS_FILE_LOG with File Status = 2 (Pushed to destination)
                            UpdateIsFileLog(sourceFile.Id, FileStatusType.PushedToDestination);

                            break;

                    }
                    //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                    connInfo = null;
                }
                //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                sIinetPassword = string.Empty;
            }
            return true;
           }
           catch (Exception exception)
           {
               Logger.Error("FtpFileSender Method " + exception.Message);
               return false;
           }

        }

      

      private void UpdateIsFileLogExt1( IEnumerable<IsFileLogExt1Model> ext1Model)
      {
        // ext1Model.IinetUploadStatus => P: Pending, S: Succesfully Uploaded, F: Failed while Uploading 
        foreach (var isFileLogExt1Model in ext1Model)
        {
          IsFileLogExt1Repository.Update(isFileLogExt1Model);
        }
        UnitOfWork.CommitDefault(); 
      }
      
      public bool EmailSendToSisSupport(IsInputFile sourceFile, ArrayList contactArray)
      {
        try
        {
          var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

          if (emailSender != null)
          {
            var emailAddresses = Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification");
            Logger.Error("Exception email address " + emailAddresses);
            var smptSettings = Core.Configuration.ConnectionString.GetSmptSetting();
            var mailBody = new StringBuilder();
            mailBody.Append("Hi,").Append(Environment.NewLine).Append(Environment.NewLine).Append("Member Profile Data files sending is failed periviously and now success for follwoing :").Append(Environment.NewLine)
              .Append("File details are:").Append(Environment.NewLine)
              .AppendFormat("File Name [{0}].", sourceFile.FileName).Append(Environment.NewLine)
              .AppendFormat("File Type [{0}].", sourceFile.FileFormat).Append(Environment.NewLine)
              .Append(Environment.NewLine).Append(
                "This is a system generated message - please do not reply.");

            var mailMessage = new MailMessage(smptSettings.FromAddress, emailAddresses, "Member Profile Data files sending failed periviously and now success.", mailBody.ToString());

            emailSender.Send(mailMessage);
          }// End if  
        }// End try
        catch (Exception ex)
        {
          Logger.InfoFormat("Handled Error. Error Message: {0}, StackTrace: {1}", ex.Message, ex.StackTrace);
          return false;
        }// End catch
        return true;
      }


      private static bool EmailSendForMemberProfileDataFiles(IsInputFile sourceFile, ArrayList contacts)
      {


        //create nvelocity data dictionary
        var context = new VelocityContext();
        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

        try
        {

          // CMP597: System sending single email alert twice to the same email id
          // Create unique contact list 
          //use a hashtable to create a unique list
          var contactHashTable = new Hashtable();

          foreach (var item in contacts)
          {
            //set a key in the hashtable for our arraylist value - leaving the hashtable value empty
            contactHashTable[item] = null;
          }

          //now grab the contacts from that hashtable 
          contacts = new ArrayList(contactHashTable.Keys);


          if (contacts != null && contacts.Count != 0)
          {
            Logger.Info("Total Available Contact Found : " + contacts.Count);

            var content = string.Empty;
            var billingMonthName = (new DateTime(sourceFile.BillingYear, sourceFile.BillingMonth, sourceFile.BillingPeriod)).ToString("MMM");

            var billingPeriod = string.Format("{0}-{1}-P{2}", sourceFile.BillingYear,
                                              billingMonthName, sourceFile.BillingPeriod);

            if (sourceFile.FileFormat == FileFormatType.ChangeInfoReferenceDataUpdateCsv)
            {
              content = "The Reference Data Changes file applicable for Billing Period " + billingPeriod + " was transferred to your iiNET account(s).";
            }
            if (sourceFile.FileFormat == FileFormatType.CompleteReferenceDataCsv)
            {
              content = "The Complete Reference Data file applicable for Billing Period " + billingPeriod + " was transferred to your iiNET account(s).";
            }
            if (sourceFile.FileFormat == FileFormatType.CompleteContactsDataCsv)
            {
              content = "The Complete Contacts Data file applicable for Billing Period " + billingPeriod + " was transferred to your iiNET account(s).";
            }

            context.Put("Content", content);
            context.Put("FileName", string.Format("Name of file: {0}", sourceFile.FileName));
            context.Put("SisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

            var emailToMember = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.iiNetFileTransferNotificationToMember, context);

            Logger.Info("Template Created for iiNet File Transfer Notification To Member.");

            //Get the eMail settings for user welcome notification
            var emailSettingForUserNotification =
              emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.iiNetFileTransferNotificationToMember);

            Logger.Info("Max ContactIds Count For Single Email : " + MaxContactIdsForSingleEmail);

            //loop through the contacts list and add them to clubed BCC list of mail to be sent
            for (var index = 0; index < contacts.Count; index += MaxContactIdsForSingleEmail)
            {
              //create a mail object to send mail
              var msgUserNotification = new MailMessage
              {
                From =
                  new MailAddress(emailSettingForUserNotification.SingleOrDefault().FromEmailAddress)
              };
              msgUserNotification.IsBodyHtml = true;

              var contactArray = contacts.GetRange(index, ((contacts.Count - index) > MaxContactIdsForSingleEmail) ? MaxContactIdsForSingleEmail : (contacts.Count - index));

              foreach (var ca in contactArray)
              {
                //Send the email and alert notifications to member
                msgUserNotification.Bcc.Add(new MailAddress(ca.ToString()));
              }


              if (sourceFile.FileFormat == FileFormatType.ChangeInfoReferenceDataUpdateCsv)
              {
                msgUserNotification.Subject = "SIS: Invoice Reference Data - Change Information File transferred to iiNET ";

              }
              else if (sourceFile.FileFormat == FileFormatType.CompleteReferenceDataCsv)
              {
                msgUserNotification.Subject = "SIS: Invoice Reference Data - Complete Information File transferred to iiNET ";

              }
              else if (sourceFile.FileFormat == FileFormatType.CompleteContactsDataCsv)
              {
                msgUserNotification.Subject = "SIS: Contacts Data - Complete Information File transferred to iiNET";
              }
              else
              {
                //set subject of mail (replace special field placeholders with values)
                msgUserNotification.Subject = emailSettingForUserNotification.SingleOrDefault().Subject + " - " + sourceFile.FileFormat;

              }

              //set body text of mail
              msgUserNotification.Body = emailToMember;

              //send the mail
              emailSender.Send(msgUserNotification);
            }

            Logger.Info("iiNet File Transfer Notification To Member sent");

            //*****************************************************************************
          }
          else
          {
            Logger.Info("Contact Not Found");
          }
          return true;
        }
        catch (Exception exception)
        {

          Logger.Error("Error in EmailSendForMemberProfileDataFiles ", exception);
          return false;
        }
      }
      

      public IList<IsInputFile> GetFileListByFileType(int fileType)
        {
          
          var inputFiles = new List<IsInputFile>();
          //SCP357374: SRM: One FTP thread stuck - 9APR2015
          try
          {
            var fileList = MemberRepository.GetIsFileListByFileType(fileType);

            foreach (var isFileList in fileList)
            {
                inputFiles.Add(new IsInputFile
                                    {
                                        Id = isFileList.Id,
                                        FileName = isFileList.FileName,
                                        IsIncoming = isFileList.IsIncoming,
                                        SenderReceiver = isFileList.SenderReceiver,
                                        BillingPeriod = isFileList.BillingPeriod,
                                        BillingMonth = isFileList.BillingMonth,
                                        ReceivedDate = isFileList.ReceivedDate,
                                        SenderReceiverIP = isFileList.SenderReceiverIP,
                                        FileLocation = isFileList.FileLocation,
                                        BillingCategory = isFileList.BillingCategory,
                                        LastUpdatedBy = isFileList.LastUpdatedBy,
                                        LastUpdatedOn = isFileList.LastUpdatedOn,
                                        FileVersion = isFileList.FileVersion,
                                        FileStatusId = isFileList.FileStatusId,
                                        FileFormatId = isFileList.FileFormatId,
                                        FileDate = isFileList.FileDate,
                                        OutputFileDeliveryMethodId = isFileList.OutputFileDeliveryMethodId,
                                        BillingYear = isFileList.BillingYear,
                                        SenderRecieverType = isFileList.SenderRecieverType,
                                        FileKey = isFileList.FileKey,
                                        ExpectedResponseTime = isFileList.ExpectedResponseTime,
                                        FileProcessStartTime = isFileList.FileProcessStartTime,
                                        FileProcessEndTime = isFileList.FileProcessEndTime,
                                        IsResponseRecieved = isFileList.IsResponseRecieved,
                                        BillingMonthFrom = isFileList.BillingMonthFrom,
                                        BillingMonthTo = isFileList.BillingMonthTo,
                                        BillingPeriodFrom = isFileList.BillingPeriodFrom,
                                        BillingPeriodTo = isFileList.BillingPeriodTo,
                                        FileFormat = isFileList.FileFormat,
                                        FileStatus = isFileList.FileStatus,
                                        FileSubmissionFrom = isFileList.FileSubmissionFrom,
                                        FileSubmissionTo = isFileList.FileSubmissionTo,
                                        UploadedBy = isFileList.UploadedBy,
                                        MiscLocationCode = isFileList.MiscLocationCode
                                    });

            }
          }
          catch (Exception exception)
          {
              Logger.Error("GetFileListByFileType Method " + exception.Message);
          }
          return inputFiles.ToList();
        }


        /// <summary>
        /// Send Email notification to all billing category contacts, in case billing category is null
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static bool EmailSendAllBillingCategory(int? memberId, string fileName, FileFormatType fileFormatType)
        {
            var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

            try
            {


                if (memberId != null)
                {
                    var paxoutputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.PAXOutputAvailableAlert);

                    if (paxoutputAvailableContact != null)
                    {
                        Logger.Info("Total output Available Contact Found : " + paxoutputAvailableContact.Count);
                        SendMemberFileTransferNotification(fileName, paxoutputAvailableContact, fileFormatType);
                    }
                    else
                    {
                        Logger.Info("Output Available Contact Not Found");
                    }

                    var miscOutputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.MISCOutputAvailableAlert);
                    if (miscOutputAvailableContact != null)
                    {
                        Logger.Info("Total miscOutputAvailableContact Found : " + miscOutputAvailableContact.Count);
                        SendMemberFileTransferNotification(fileName, miscOutputAvailableContact, fileFormatType);
                    }
                    else
                    {
                        Logger.Info("MISCOutputAvailableAlert Not Found");
                    }


                    var cargoOutputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.CGOOutputAvailableAlert);
                    if (cargoOutputAvailableContact != null)
                    {
                        Logger.Info("Total CGOOutputAvailableAlert Found : " + cargoOutputAvailableContact.Count);
                        SendMemberFileTransferNotification(fileName, cargoOutputAvailableContact, fileFormatType);
                    }
                    else
                    {
                        Logger.Info("CGOOutputAvailableAlert Not Found");
                    }


                    var uatpOutputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.UATPOutputAvailableAlert);
                    if (uatpOutputAvailableContact != null)
                    {
                        Logger.Info("Total uatpOutputAvailableContact Found : " + uatpOutputAvailableContact.Count);
                        SendMemberFileTransferNotification(fileName, uatpOutputAvailableContact, fileFormatType);
                    }
                    else
                    {
                        Logger.Info("uatpOutputAvailableContact Not Found");
                    }

                }
                return true;
            }

            catch (Exception exception)
            {
                Logger.Error("Error in EmailSendAllBillingCategory ", exception);
                return false;

            }

        }

        /// <summary>
        /// Add SIS ops alert , in case FTP upload failure
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="member"></param>
        private static void AddIssisOpsAlert(string fileName, int? member)
        {
            // Create an object of the nVelocity data dictionary

            var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
            var memberCode = string.Empty;
            if (member != null)
            {
                memberCode = memberManager.GetMemberCommercialName((int)member);
            }

            var context = new VelocityContext();
            context.Put("FilePath", Path.GetFileName(fileName));
            if (member != null)
            {
                context.Put("Member", "Member : " + memberCode);
            }
            else
            {
                context.Put("Member", string.Empty);
            }


            var issisOpsAlert = new ISSISOpsAlert
            {
                Message = "FTP File Upload Failure : " + Path.GetFileName(fileName),
                AlertDateTime = DateTime.UtcNow,
                IsActive = true,
                EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                Title = "FTP File Upload Failure: " + Path.GetFileName(fileName),
            };

            BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.FtpFileUploadFailure, context);
        }

        /// <summary>
        /// Upload File to FTP as per connection object credentials
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="fileLocation"></param>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <param name="isFileId"></param>
        /// <returns></returns>
        public static bool UploadFileToFtp(ConnectionInfo connInfo, string fileLocation, string fileName, string folderName, Guid isFileId)
        {

            bool returnType = false;

            try
            {
                UploadFileFailureAttempt += 1;
                // Check file Exist in SFR location
                if (!File.Exists(Path.Combine(fileLocation, fileName)))
                {
                    Logger.Info("File Not Exist : " + fileName);

                    var isFtpLog = new IsftpLog
                    {
                        IsfileLogId = isFileId,
                        LogText = "File Not Found : " + fileName,
                        TdfFileContent = "NA",
                        LastUpdatedOn = DateTime.UtcNow
                    };

                    AddIsFtpLog(isFtpLog);
                    RichTextBoxLogWriter.IsFtpLog = string.Empty;
                    return false;
                }

                Logger.Error("Uploading Started for file name : " + fileName);


                try
                {
                    var fcPax = new FtpsClient(connInfo);
                    fcPax.UploadFile(fileLocation, fileName, folderName);
                    returnType = true;
                }
                catch (Exception exception)
                {
                    returnType = false;
                    Logger.Error("Error while File : " + fileName + " Upload", exception);
                }

                var objIsFtpLog = new IsftpLog
                {
                    IsfileLogId = isFileId,
                    LogText = RichTextBoxLogWriter.IsFtpLog,
                    TdfFileContent = "NA",
                    LastUpdatedOn = DateTime.UtcNow
                };
                AddIsFtpLog(objIsFtpLog);
                RichTextBoxLogWriter.IsFtpLog = string.Empty;
            }
            catch (Exception exception)
            {
                returnType = false;
                Logger.Error("Error while File : " + fileName + " Upload", exception);
            }
            if (returnType)
            {
                UploadFileFailureAttempt = 0;
            }
            return returnType;
        }

        /// <summary>
        ///  Upload File to ACH FTP server as per connection object credentials
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="fileName"></param>
        /// <param name="achConnection"></param>
        /// <param name="isFileId"></param>
        /// <returns></returns>
        public static bool UploadFileToAchFtp(string fileLocation, string fileName, AchConnection achConnection, Guid isFileId)
        {
            bool returnType = false;


            try
            {
                UploadFileFailureAttempt += 1;
                // Check file Exist in SFR location
                if (!File.Exists(Path.Combine(fileLocation, fileName)))
                {
                    Logger.Info("File Not Exist : " + fileName);

                    var isFtpLog = new IsftpLog
                    {
                        IsfileLogId = isFileId,
                        LogText = "File Not Found : " + fileName,
                        TdfFileContent = "NA",
                        LastUpdatedOn = DateTime.UtcNow
                    };

                    AddIsFtpLog(isFtpLog);
                    RichTextBoxLogWriter.IsFtpLog = string.Empty;
                    return false;
                }

                Logger.Info("Uploading started for file name : " + fileName);


                var fcPax = new AchFtpClient();
                returnType = fcPax.UploadFile(fileName, fileLocation, achConnection);

                Logger.Info("UploadFileToAchFtp : File Uploaded: " + returnType.ToString());

                var objIsFtpLog = new IsftpLog
                                    {
                                        IsfileLogId = isFileId,
                                        LogText = RichTextBoxLogWriter.IsFtpLog,
                                        TdfFileContent = "NA",
                                        LastUpdatedOn = DateTime.UtcNow
                                    };

                AddIsFtpLog(objIsFtpLog);
                RichTextBoxLogWriter.IsFtpLog = string.Empty;
                if (returnType)
                    Logger.Info("Uploaded File for ACH : " + fileName);
            }
            catch (Exception exception)
            {
                returnType = false;
                Logger.Error("Error while File : " + fileName + " Upload", exception);
            }
            if (returnType)
            {
                UploadFileFailureAttempt = 0;
            }
            return returnType;
        }

        /// <summary>
        ///  Upload File to iiNET as per connection object credentials
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="fileLocation"></param>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <param name="accountId"></param>
        /// <param name="isFileId"></param>
        /// <returns></returns>
        public static bool UploadFileToiiNet(ConnectionInfo connInfo, string fileLocation, string fileName, string folderName, string accountId, Guid isFileId)
        {
            bool returnType = false;
            if (string.IsNullOrEmpty(accountId))
            {
                // If iiNet account ID is null or Empty, Simply update File Status to 21
                Logger.Info("Could not found iiNet Account Id for FileName : " + fileName);
                return false;
            }


            Logger.Info("iiNET FTP Details :" + connInfo.ServerName + connInfo.UserName + connInfo.Port);

            try
            {
                UploadFileFailureAttempt += 1;
                // Check file Exist in SFR location
                if (!File.Exists(Path.Combine(fileLocation, fileName)))
                {
                    Logger.Info("File Not Exist: " + Path.Combine(fileLocation, fileName));
                    var isFtpLog = new IsftpLog
                    {
                        IsfileLogId = isFileId,
                        LogText = "File Not Found : " + fileName,
                        TdfFileContent = "NA",
                        LastUpdatedOn = DateTime.UtcNow
                    };

                    AddIsFtpLog(isFtpLog);
                    RichTextBoxLogWriter.IsFtpLog = string.Empty;
                    return false;
                }


                RichTextBoxLogWriter.IsFtpLog = string.Empty;
                var fcPax = new FtpsClient(connInfo);
                Logger.Info("FTP File Sender Uploading File : " + fileName);

                try
                {
                    fcPax.UploadFile(fileLocation, fileName, folderName);
                    returnType = true;
                }
                catch (Exception exception)
                {
                    returnType = false;
                    Logger.Error("Error while File : " + fileName + " Upload", exception);
                }


                // Create And Push TDF file on iiNet Folder 
                Logger.Info("Creating TDF File: " + fileName);

                try
                {
                    if (returnType)
                    {
                        if (CreateTdfFile(fileName, accountId))
                        {
                            returnType = true;
                            Logger.Info("Uploaded  TDF File: " + fileName);
                        }
                        else
                        {
                            returnType = false;
                            Logger.Info("Not Uploaded  TDF File");
                        }
                    }

                }
                catch (Exception exception)
                {
                    returnType = false;
                    Logger.Error("Error while File : " + fileName + " Upload", exception);
                }



                var objIsFtpLog = new IsftpLog
                {
                    IsfileLogId = isFileId,
                    LogText = RichTextBoxLogWriter.IsFtpLog,
                    TdfFileContent = TdfFileContent,
                    LastUpdatedOn = DateTime.UtcNow
                };

                AddIsFtpLog(objIsFtpLog);
                RichTextBoxLogWriter.IsFtpLog = string.Empty;

            }
            catch (Exception exception)
            {
                returnType = false;
                Logger.Error("Error while File : " + fileName + " Upload", exception);
            }

            if (returnType)
            {
                UploadFileFailureAttempt = 0;
            }

            return returnType;
        }

        /// <summary>
        /// Upload File to iiNET for all account Id, in case billing category is null
        /// </summary>
        /// <param name="connInfo"></param>
        /// <param name="isInputFile"></param>
        /// <param name="folderName"></param>
        /// <param name="accountIdArrayList"></param>
        /// <returns></returns>
        public static bool UploadFileToiiNetForAllAccountId(ConnectionInfo connInfo, IsInputFile isInputFile, string folderName, ArrayList accountIdArrayList)
        {
            bool returnType = false;

            UploadFileFailureAttempt += 1;

            if (accountIdArrayList == null)
            {
                // If iiNet account ID is null or Empty, Simply update File Status to 21
                Logger.Info("Could not found iiNet Account Id for FileName : " + isInputFile.FileName);
                return false;
            }


            Logger.Info("iiNET FTP Details :ServerName := " + connInfo.ServerName + " UserName := " + connInfo.UserName + "  Port:= " + connInfo.Port);

            try
            {
                // Check file Exist in SFR location
                if (!File.Exists(Path.Combine(isInputFile.FileLocation, isInputFile.FileName)))
                {
                    Logger.Info("File Not Exist: " + isInputFile.FileName);
                    var isFtpLog = new IsftpLog
                    {
                        IsfileLogId = isInputFile.Id,
                        LogText = "File Not Found : " + isInputFile.FileName,
                        TdfFileContent = "NA",
                        LastUpdatedOn = DateTime.UtcNow
                    };

                    AddIsFtpLog(isFtpLog);
                    RichTextBoxLogWriter.IsFtpLog = string.Empty;
                    return false;
                }


                RichTextBoxLogWriter.IsFtpLog = string.Empty;
                var fcPax = new FtpsClient(connInfo);
                Logger.Info("FTP File Sender Uploading File : " + isInputFile.FileName);

                try
                {
                    fcPax.UploadFile(isInputFile.FileLocation, isInputFile.FileName, folderName);
                    returnType = true;
                }
                catch (Exception exception)
                {
                    returnType = false;
                    Logger.Error("Error while File : " + isInputFile.FileName + " Upload", exception);
                }


                // Create And Push TDF file on iiNet Folder 
                Logger.Info("Creating TDF File: " + isInputFile.FileName);

                try
                {
                    if (returnType)
                    {
                        if (CreateTdfFileForAllAccountId(isInputFile.FileName, accountIdArrayList))
                        {
                            returnType = true;
                            Logger.Info("Uploaded  TDF File: " + isInputFile.FileName);
                        }
                        else
                        {
                            returnType = false;
                            Logger.Info("Not Uploaded  TDF File");
                        }
                    }

                }
                catch (Exception exception)
                {
                    returnType = false;
                    Logger.Error("Error while File : " + isInputFile.FileName + " Upload", exception);
                }


                var objIsFtpLog = new IsftpLog
                {
                    IsfileLogId = isInputFile.Id,
                    LogText = RichTextBoxLogWriter.IsFtpLog,
                    TdfFileContent = TdfFileContent,
                    LastUpdatedOn = DateTime.UtcNow
                };

                AddIsFtpLog(objIsFtpLog);
                RichTextBoxLogWriter.IsFtpLog = string.Empty;
            }
            catch (Exception exception)
            {
                returnType = false;
                Logger.Error("Error while File : " + isInputFile.FileName + " Upload", exception);
            }
            if (returnType)
            {
                UploadFileFailureAttempt = 0;
            }
            return returnType;
        }






      /// <summary>
      /// This method has been written only for CMP#597. The reason behind to seperate out the method is to PUSH single file only and its TDF will send seperatly.
      /// </summary>
      /// <param name="connInfo"></param>
      /// <param name="isInputFile"></param>
      /// <param name="folderName"></param>
      /// <returns></returns>
      public static bool UploadReferenceContacttoiiNet(ConnectionInfo connInfo, IsInputFile isInputFile, string folderName)
      {
        bool returnType = false;
        UploadFileFailureAttempt += 1;

        Logger.Info("iiNET FTP Details :ServerName := " + connInfo.ServerName + " UserName := " + connInfo.UserName + "  Port:= " + connInfo.Port);
        try
        {
          // Check file Exist in SFR location
          if (!File.Exists(Path.Combine(isInputFile.FileLocation, isInputFile.FileName)))
          {
            Logger.Info("File Not Exist: " + isInputFile.FileName);
            var isFtpLog = new IsftpLog
            {
              IsfileLogId = isInputFile.Id,
              LogText = "File Not Found : " + isInputFile.FileName,
              TdfFileContent = "NA",
              LastUpdatedOn = DateTime.UtcNow
            };

            AddIsFtpLog(isFtpLog);
            RichTextBoxLogWriter.IsFtpLog = string.Empty;
            return false;
          }


          RichTextBoxLogWriter.IsFtpLog = string.Empty;
          var fcPax = new FtpsClient(connInfo);
          Logger.Info("FTP File Sender Uploading File : " + isInputFile.FileName);

          try
          {
            fcPax.UploadFile(isInputFile.FileLocation, isInputFile.FileName, folderName);
            returnType = true;
          }
          catch (Exception exception)
          {
            returnType = false;
            Logger.Error("Error while File : " + isInputFile.FileName + " Upload", exception);
          }
        }
        catch (Exception exception)
        {
          returnType = false;
          Logger.Error("Error while File : " + isInputFile.FileName + " Upload", exception);
        }
        if (returnType)
        {
          UploadFileFailureAttempt = 0;
        }
        return returnType;
     }

        /// <summary>
        /// Create TDF DBI file for iiNET FTP server
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static bool CreateTdfFile(string fileName, string accountId)
        {

            // string tdfPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            const string tdfPath = @"C:\";
            Logger.Info("TDF File Path : " + tdfPath);
            StreamWriter streamWriter = File.CreateText(tdfPath + @"\" + Path.GetFileNameWithoutExtension(fileName) + ".tdf");

            streamWriter.WriteLine("DATE/TIME:" + DateTime.UtcNow.ToString("yyyyMMddHHmm"));
            streamWriter.WriteLine("SERVICE:" + SystemParameters.Instance.iiNet.Service);
            streamWriter.WriteLine("FILENAME:" + fileName);
            streamWriter.WriteLine("DESCRIPTION:" + SystemParameters.Instance.iiNet.Description);
            streamWriter.WriteLine("SIGNATURE:" + SystemParameters.Instance.iiNet.Signature);
            streamWriter.WriteLine("SENDER:" + SystemParameters.Instance.iiNet.Sender);
            streamWriter.WriteLine("RECIPIENT:" + accountId);
            streamWriter.WriteLine("FILETYPE:" + SystemParameters.Instance.iiNet.FileType);

            streamWriter.Close();


            var connInfo = new ConnectionInfo
            {
                ServerName = SystemParameters.Instance.iiNet.ServerName,
                UserName = SystemParameters.Instance.iiNet.UserName,
                Password = SystemParameters.Instance.iiNet.Password,
                Port = SystemParameters.Instance.iiNet.Port,
                Security = SystemParameters.Instance.iiNet.Security,
                AcceptAllCertificates = true
            };

            var fc = new FtpsClient(connInfo);
            RichTextBoxLogWriter.IsFtpLog += " ============================= FTP Log For TDF File ============================= <br/>";
            fc.UploadFile(tdfPath + @"\", (Path.GetFileNameWithoutExtension(fileName) + ".tdf"), SystemParameters.Instance.iiNet.iiNetFolderName);


            StreamReader readSettings = null;
            TdfFileContent = string.Empty;
            string tdffilePath = tdfPath + @"\" + Path.GetFileNameWithoutExtension(fileName) + ".tdf";
            if (!string.IsNullOrEmpty(tdffilePath) && System.IO.File.Exists(tdffilePath))
            {
                readSettings = File.OpenText(tdffilePath);
            }

            if (readSettings != null)
            {
                TdfFileContent = Convert.ToString(readSettings.ReadToEnd());
                readSettings.Close();
            }

            File.Delete(tdfPath + @"\" + Path.GetFileNameWithoutExtension(fileName) + ".tdf");

            //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
            connInfo = null;

            return true;
        }

      /// <summary>
      /// Create TDF file and send to all billing category accound ID
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="accountIdArrayList"></param>
      /// <param name="IsTFDForReferenceContactDataFile"></param>
      /// <returns></returns>
        public static bool CreateTdfFileForAllAccountId(string fileName, ArrayList accountIdArrayList, bool IsTFDForReferenceContactDataFile = false)
      {
        bool returnType = false;
        try
        {

          if (accountIdArrayList.Count > 0)
          {

            // string tdfPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            const string tdfPath = @"C:\";
            Logger.Info("TDF File Path : " + tdfPath);

            var tdfFileName = fileName;

            if (IsTFDForReferenceContactDataFile)
            {
              tdfFileName = string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(fileName),
                                          _counterTdfFileForReferenceData.ToString());
            }

            StreamWriter streamWriter =
              File.CreateText(tdfPath + @"\" + Path.GetFileNameWithoutExtension(tdfFileName) + ".tdf");
            streamWriter.WriteLine("DATE/TIME:" + DateTime.UtcNow.ToString("yyyyMMddHHmm"));

            string strPreviousAccountId = string.Empty;
            int i = 0;
            for (i = 0; i <= accountIdArrayList.Count - 1; i++)
            {
              if (!string.IsNullOrEmpty(accountIdArrayList[i].ToString()))
              {
                if (strPreviousAccountId != accountIdArrayList[i].ToString())
                {

                  streamWriter.WriteLine("      ");
                  streamWriter.WriteLine("SERVICE:" + SystemParameters.Instance.iiNet.Service);
                  streamWriter.WriteLine("FILENAME:" + fileName);
                  streamWriter.WriteLine("DESCRIPTION:" + SystemParameters.Instance.iiNet.Description);
                  streamWriter.WriteLine("SIGNATURE:" + SystemParameters.Instance.iiNet.Signature);
                  streamWriter.WriteLine("SENDER:" + SystemParameters.Instance.iiNet.Sender);
                  streamWriter.WriteLine("RECIPIENT:" + accountIdArrayList[i]);
                  streamWriter.WriteLine("FILETYPE:" + SystemParameters.Instance.iiNet.FileType);

                }
              }
            }

            streamWriter.Close();

            var connInfo = new ConnectionInfo
                             {
                               ServerName = SystemParameters.Instance.iiNet.ServerName,
                               UserName = SystemParameters.Instance.iiNet.UserName,
                               Password = SystemParameters.Instance.iiNet.Password,
                               Port = SystemParameters.Instance.iiNet.Port,
                               Security = SystemParameters.Instance.iiNet.Security,
                               AcceptAllCertificates = true
                             };

            var fc = new FtpsClient(connInfo);
            RichTextBoxLogWriter.IsFtpLog +=
              " ============================= FTP Log For TDF File ============================= <br/>";
            fc.UploadFile(tdfPath + @"\", (Path.GetFileNameWithoutExtension(tdfFileName) + ".tdf"),
                          SystemParameters.Instance.iiNet.iiNetFolderName);

            StreamReader readSettings = null;
            string tdffilePath = tdfPath + @"\" + Path.GetFileNameWithoutExtension(tdfFileName) + ".tdf";
            if (!string.IsNullOrEmpty(tdffilePath) && System.IO.File.Exists(tdffilePath))
            {
              readSettings = System.IO.File.OpenText(tdffilePath);
            }

            if (readSettings != null)
            {
              TdfFileContent += Convert.ToString(readSettings.ReadToEnd());
              readSettings.Close();
            }

            File.Delete(tdfPath + @"\" + Path.GetFileNameWithoutExtension(tdfFileName) + ".tdf");
            returnType = true;

            //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
            connInfo = null;
          }
        }
        catch (Exception exception)
        {
          returnType = false;

          Logger.Error("Error occcur while upload TDF : " + fileName + " Exception :", exception);
        }
        return returnType;

      }

      /// <summary>
        /// Update IS FILE LOG status
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileStatusType"></param>
        /// <returns></returns>
        private static bool UpdateIsFileLog(Guid fileId, FileStatusType fileStatusType,bool isLocationSpecific = false)
        {
            
            bool returnValue = false;
            try
            {
                var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                var objIsInputFile = referenceManager.GetIsInputFileFromIsFileId(fileId);
                objIsInputFile.FileStatusId = (int)fileStatusType;
                objIsInputFile.ReceivedDate = DateTime.UtcNow;
                referenceManager.UpdateIsInputFile(objIsInputFile);
                Logger.Info("Update file status of file : " + objIsInputFile.FileName+" to " + fileStatusType);
                if (objIsInputFile.FileStatusId == (int)FileStatusType.iiNetRecipientNotFound)
                {
                    Logger.Info("iiNetRecipientNotFound Mail sending...........");
                    Logger.Info("billing category " + objIsInputFile.BillingCategory);
                    //CMP#622: MISC Outputs Split as per Location ID
                    bool result = EmailToOutputContacts(objIsInputFile.FileName, objIsInputFile.SenderReceiver, objIsInputFile.BillingCategory, objIsInputFile.FileFormat, objIsInputFile.MiscLocationCode);
                    if (result)
                    {
                        Logger.Info("Mail sent successfully to output contacts ");
                    }
                }
                returnValue = true;
            }
            catch (Exception exception)
            {
                Logger.Error("UpdateIsFileLog Method " + exception.Message);
                returnValue = false;
            }
            return returnValue;
        }

        private static bool EmailToOutputContacts(string fileName, int? memberId, int? billingCategory, FileFormatType fileformat,string locationCode = null)
        {
            var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));

             //create nvelocity data dictionary
            var context = new VelocityContext();
            //get an instance of email settings  repository
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
            //get an object of the EmailSender component
            var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
            //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
            var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

            // Make an object of emailaddress
            IEnumerable<string> emailAddress = null;

            try
            {


                if (memberId != null)
                {
                    var outputAvailableContact = new List<Contact>();
                    if(billingCategory == 1)
                    {
                        //outputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.PAXOutputAvailableAlert);
                        outputAvailableContact = GetMemberContactsForFileType((int)memberId, BillingCategoryType.Pax, fileformat);
                    }

                    else if(billingCategory == 2)
                    {

                        //outputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.CGOOutputAvailableAlert);
                        outputAvailableContact = GetMemberContactsForFileType((int)memberId, BillingCategoryType.Cgo, fileformat);
                    }
                    else if(billingCategory == 3)
                    {
                        //outputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.MISCOutputAvailableAlert);
                        outputAvailableContact = GetMemberContactsForFileType((int)memberId, BillingCategoryType.Misc, fileformat, locationCode);
                        
                    }
                    else
                    {
                        //outputAvailableContact = memberManager.GetContactsForContactType((int)memberId, ProcessingContactType.UATPOutputAvailableAlert); 
                        outputAvailableContact = GetMemberContactsForFileType((int)memberId, BillingCategoryType.Uatp, fileformat);
                    }

                    if (outputAvailableContact != null)
                    {
                       
                        // Genertae an email address for the pax contacts
                        emailAddress = outputAvailableContact.Select(c => c.EmailAddress);

                        Logger.Info("Total output Available Contact Found : " + outputAvailableContact.Count);
                        context.Put("ZipFileName", fileName);
                        context.Put("SisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);


                        var emailToInvoice = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.FileAvailableForDownload, context);

                        Logger.Error("Template Created for INVOICES To Member.");

                        //Get the eMail settings for user welcome notification
                        var emailSettingForUserNotification =
                          emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.FileAvailableForDownload);

                        //create a mail object to send mail 
                        var msgUserNotification = new MailMessage
                        {
                            From =
                              new MailAddress(emailSettingForUserNotification.SingleOrDefault().FromEmailAddress)
                        };
                        msgUserNotification.IsBodyHtml = true;

                        //loop through the contacts list and add them to To list of mail to be sent
                       
                        foreach (var MiscContacts in emailAddress)
                        {
                            msgUserNotification.To.Add(new MailAddress(MiscContacts));
                        }
                       
                        //set subject of mail (replace special field placeholders with values)
                        msgUserNotification.Subject = emailSettingForUserNotification.SingleOrDefault().Subject + " - " + fileformat;

                        //set body text of mail
                        msgUserNotification.Body = emailToInvoice;

                        //send the mail
                        emailSender.Send(msgUserNotification);

                        //clear nvelocity context data
                        context = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error in EmailToOutputContacts ", ex);
                return false;
            }

            return true;
        }

        private static void UpdateIsFileLogStatus()
        {

            try
            {
                var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

                var isInputFile = referenceManager.GetIsInputFileByStatus((int)FileStatusType.ErrorInFileSend).Where(s=>s.IsPurged == false);
                foreach (var inputFile in isInputFile)
                {
                    var objIsInputFile = referenceManager.GetIsInputFileFromIsFileId(inputFile.Id);
                    objIsInputFile.FileStatusId = (int)FileStatusType.AvailableForDownload;
                    objIsInputFile.ReceivedDate = DateTime.UtcNow;
                    referenceManager.UpdateIsInputFile(objIsInputFile);
                }


            }
            catch (Exception exception)
            {
                Logger.Error("UpdateIsFileLogStatus Method " + exception.Message);
            }

        }

        /// <summary>
        /// Add IS FTP LOG entry of FTP log and TDF file content.
        /// </summary>
        /// <param name="isftpLog"></param>
        /// <returns></returns>
        private static bool AddIsFtpLog(IsftpLog isftpLog)
        {
            bool returnValue = false;
            try
            {
                //var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                //referenceManager.AddIsFtpLog(isftpLog);
                var ftpLogManager = new IsFtpLogManager();
                ftpLogManager.AddIsFtpLog(isftpLog);
                returnValue = true;
                Logger.Info("Updated FTP LOG");
            }
            catch (Exception exception)
            {
                Logger.Error("AddIsFtpLog Method " + exception.Message);
                returnValue = false;
            }
            return returnValue;
        }

        /// <summary>
        /// Send File Upload notification to Member
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="memberEmailId"></param>
        /// <param name="fileFormatType"></param>
        /// <param name="content">default== null</param>
        /// <returns></returns>
        public static bool SendMemberFileTransferNotification(string fileName, List<Contact> outputAvailableContact, FileFormatType fileFormatType, string content = null)
        {

            bool flag;
            //create nvelocity data dictionary
            var context = new VelocityContext();
            //get an instance of email settings  repository
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
            //get an object of the EmailSender component
            var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
            //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
            var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

            try
            {
                context.Put("Content", content);
                context.Put("FileName", fileName);
                context.Put("SisOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);


                var emailToInvoice = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.iiNetFileTransferNotificationToMember, context);

                Logger.Info("Template Created for iiNet File Transfer Notification To Member.");

                var emailAddress = outputAvailableContact.Select(c => c.EmailAddress).ToList();
                //Get the eMail settings for user welcome notification
                var emailSettingForUserNotification =
                  emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.iiNetFileTransferNotificationToMember);

                //create a mail object to send mail 
                var msgUserNotification = new MailMessage
                {
                    From =
                      new MailAddress(emailSettingForUserNotification.SingleOrDefault().FromEmailAddress)
                };
                msgUserNotification.IsBodyHtml = true;

                //set subject of mail (replace special field placeholders with values)
                msgUserNotification.Subject = emailSettingForUserNotification.SingleOrDefault().Subject + " - " + fileFormatType;

                //set body text of mail
                msgUserNotification.Body = emailToInvoice;

                int emailPerMsg = 200;
                try
                {
                    emailPerMsg = Convert.ToInt32(ConfigurationManager.AppSettings["MaxContactIdsForSingleEmail"]);
                }
                catch (Exception ex)
                {
                    Logger.Error("Application is not configured form 'MaxContactIdsForSingleEmail' to email contacts. So working by using 200 as default no. of contacts in to emial address.",ex);
                }

                emailPerMsg = (emailPerMsg == 0 ? 200 : emailPerMsg);

                for (var index = 0; index < emailAddress.Count; index += emailPerMsg)
                {
                    var emailIdLimited =
                        emailAddress.GetRange(index, ((emailAddress.Count - index) > emailPerMsg) ? emailPerMsg : (emailAddress.Count - index)).Aggregate((mailOne, mailTwo) => mailOne + "," + mailTwo);
                    
                    msgUserNotification.To.Add(emailIdLimited);

                    if (!string.IsNullOrEmpty(msgUserNotification.To.ToString()))
                    {
                        //send the mail
                        emailSender.Send(msgUserNotification);
                        Logger.Info("iiNet File Transfer Notification To Member send on :" + msgUserNotification.To);
                    }

                    msgUserNotification.To.Clear();
                }

                //clear nvelocity context data
                context = null;
                flag = true;
            }

            catch (Exception exception)
            {
                Logger.Error("Error occurred occured in Sending mail for iiNet File Transfer Notification To Member.", exception);
                flag = false;
            }

            return flag;

        }


        /// <summary>
        /// Encrypt Original System Parameter XML file and store the same into database table
        /// </summary>
        /// <param name="originalXmlFilePath"> string of absolute file path </param>
        /// <param name="updatingUser"> Last Updated By </param>
        /// <param name="proxyUserId"> Operated By </param>
        /// <returns></returns>
        /// SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
        public string EncryptSystemParameterXml(string originalXmlFilePath, int updatingUser, int proxyUserId)
        {
            try
            {
                // Remove already existed XML file in Cache
                FlushSystemParameterXmlFile();
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(originalXmlFilePath);
                var returnType = Crypto.EncryptString(xmlDoc.InnerXml);
                var systemParamValue = SystemParameterRepository.Get(m => m.Version == "1.0").SingleOrDefault();
                if (systemParamValue != null)
                    SystemParameterRepository.Delete(systemParamValue);

                var sysParam = new SystemParameter
                                 {
                                     ConfigXml = returnType,
                                     Version = "1.0",
                                     UpdatedOn = DateTime.UtcNow,
                                     UserId = updatingUser,
                                     ProxyUserId = proxyUserId
                                 };
                SystemParameterRepository.Add(sysParam);
                UnitOfWork.CommitDefault();
            }
            catch (Exception exception)
            {
                return null;

            }
            return null;
        }

        /// <summary>
        /// Encrypt Original System Parameter XML file and store the same into database table
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="updatingUser"> Last Updated By </param>
        /// <param name="proxyUserId"> Operated By </param>
        /// <returns></returns>
        /// SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
        public string EncryptSystemParameterXml(XmlDocument xmlDoc, int updatingUser, int proxyUserId)
        {
            // Remove already existed XML file in Cache
            try
            {

                FlushSystemParameterXmlFile();
                // var xmlDoc = new XmlDocument();
                // xmlDoc.Load(originalXmlFilePath);
                var returnType = Crypto.EncryptString(xmlDoc.InnerXml);
                var systemParamValue = SystemParameterRepository.Get(m => m.Version == "1.0").SingleOrDefault();
                if (systemParamValue != null)
                    SystemParameterRepository.Delete(systemParamValue);

                var sysParam = new SystemParameter
                {
                    ConfigXml = returnType,
                    Version = "1.0",
                    UpdatedOn = DateTime.UtcNow,
                    UserId = updatingUser,
                    ProxyUserId = proxyUserId
                };
                SystemParameterRepository.Add(sysParam);
                UnitOfWork.CommitDefault();
            }
            catch (Exception exception)
            {
                return null;

            }
            return null;
        }


        /// <summary>
        /// Get System Parameter from either cache or database table
        /// </summary>
        /// <returns></returns>o
        public XmlDocument GetSystemParamXml()
        {
            return SystemParamRepository.GetSystemParamXml();
        }


        /// <summary>
        /// Remove chache version of old system parameter XML file
        /// </summary>
        public void FlushSystemParameterXmlFile()
        {
            SystemParamRepository.RemoveCachedSystemParam();
        }

        public void FlushConnectionstring()
        {
            SystemParamRepository.RemoveCachedConnectionString();
        }
        private static List<Contact> GetMemberContactsForFileType(int memberId, BillingCategoryType billingCategoryType, FileFormatType fileFormatType, string miscLocationCode = null)
        {
            var memContact = new List<Contact>();
            var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
            switch ((int)billingCategoryType)
            {
                case (int) BillingCategoryType.Pax:
                    switch ((int)fileFormatType)
                    {
                        case (int)FileFormatType.OfflineArchive:
                        case (int)FileFormatType.ProcessedInvoiceCsvReports:
                        case (int)FileFormatType.IsIdecOutbound:
                        case (int)FileFormatType.IsXmlOutbound:
                        case (int)FileFormatType.SanityCheckReportsForSupportingDocuments:
                        case (int)FileFormatType.ValidationSanityCheckReportsIsIdecIsXml:
                           memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.PAXOutputAvailableAlert);
                            break;
                            //memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.PAXValidationErrorAlert);
                            //break;
                        case (int)FileFormatType.PaxAutoBillingInvoicePosting:
                        case (int)FileFormatType.DailyAutoBillingIrregularityReport:
                        case (int)FileFormatType.SanityCheckReportsForUsageFile:
                        case (int)FileFormatType.IsrToMember:
                        case (int)FileFormatType.PaxAutoBillingRevenueRecognition:
                        memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.AutoBillingValueDeterminationAlerts);
                            break;
                        case (int)FileFormatType.BvcCsvReport:
                            memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.ValueConfirmationReportsAlerts);
                            break;
                    }
                    break;
                case (int)BillingCategoryType.Cgo:
                    switch ((int)fileFormatType)
                    {
                        case (int)FileFormatType.OfflineArchive:
                        case (int)FileFormatType.ProcessedInvoiceCsvReports:
                        case (int)FileFormatType.IsIdecOutbound:
                        case (int)FileFormatType.IsXmlOutbound:
                        case (int)FileFormatType.SanityCheckReportsForSupportingDocuments:
                        case (int)FileFormatType.ValidationSanityCheckReportsIsIdecIsXml:
                            memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.CGOOutputAvailableAlert);
                            break;
                            //memContact = memberManager.GetContactsForContactType(memberId, ProcessingContactType.CGOValidationErrorAlert);
                            //break;
                        
                    }
                    break;
                case (int)BillingCategoryType.Misc:
                     switch ((int)fileFormatType)
                    {
                        case (int)FileFormatType.OfflineArchive:
                        case (int)FileFormatType.ProcessedInvoiceCsvReports:
                        case (int)FileFormatType.IsIdecOutbound:
                        case (int)FileFormatType.IsXmlOutbound:
                        case (int)FileFormatType.MiscIsWebXml:
                        case (int)FileFormatType.SanityCheckReportsForSupportingDocuments:
                        case (int)FileFormatType.ValidationSanityCheckReportsIsIdecIsXml:
                        //CMP#622: MISC Outputs Split as per Location ID
                        case (int)FileFormatType.MiscIsWebXmlMiscLocSpec:
                        case (int)FileFormatType.IsXmlOutboundMiscLocSpec:
                        case (int)FileFormatType.OnbehalfIsXmlMiscLocSpec:
                        case (int)FileFormatType.OfflineArchiveMiscLocSpec:
                        case (int)FileFormatType.DailyMiscBilateralOARLocSpec:
                        case (int)FileFormatType.DailyMiscBilateralIsXmlLocSpec:
                        case (int)FileFormatType.DailyMiscBilateralIsXml:
                        case (int)FileFormatType.DailyMiscBilateralOfflineArchive:
                             //CMP#655IS-WEB Display per Location
                            memContact = memberManager.GetContactsForMiscOutputAlerts(memberId, ProcessingContactType.MISCOutputAvailableAlert, miscLocationCode);
                            break;
                            //memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.MISCValidationErrorAlert);
                            //break;
                        
                    }
                    break;

                case (int)BillingCategoryType.Uatp:
                     switch ((int)fileFormatType)
                    {
                        case (int)FileFormatType.OfflineArchive:
                        case (int)FileFormatType.ProcessedInvoiceCsvReports:
                        case (int)FileFormatType.IsIdecOutbound:
                        case (int)FileFormatType.IsXmlOutbound:
                        case (int)FileFormatType.UatpAtcanFile:
                        case (int)FileFormatType.SanityCheckReportsForSupportingDocuments:
                        case (int)FileFormatType.ValidationSanityCheckReportsIsIdecIsXml:
                             memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.UATPOutputAvailableAlert);
                            break;
                            //memContact = memberManager.GetContactsForContactType(memberId,ProcessingContactType.UATPValidationErrorAlert);
                            //break;
                        
                    }
                    break;

            }

            return memContact;

        }

        #region SCP 245276
        /* SCP 245276 - SIS Legal Invoice Zip file has not been generating since October 2013 
         * Description: Backup legal XML Generation Zip and then upload the same to IATA FTP Path.
         */

        public bool BackupAndUploadFileForFtpPull(string sourceFile, string fileName, FileFormatType fileFormatType)
        {
            try
            {
                // Get Legal XML Zip FTP folder path. 
                string legalXmlZipFolder = FileIo.GetForlderPath(SFRFolderPath.LegalXMLZipToIATAFolder);
                // Check if Legal XML Zip folder path is not null or empty.
                if (!string.IsNullOrEmpty(legalXmlZipFolder))
                {
                    Logger.InfoFormat("LegalXMLZip FTP Folder Path: {0}", legalXmlZipFolder);

                    // Build complete path for output file
                    var legalXmlZipFile = Path.Combine(legalXmlZipFolder, fileName);
                    Logger.InfoFormat("outputFilePath to Copy: {0}", legalXmlZipFile);

                    // Check if destination folder exists before Copy
                    if (Directory.Exists(legalXmlZipFolder))
                    {
                        // Delete File if already exists.
                        if (File.Exists(legalXmlZipFile))
                        {
                            File.Delete(legalXmlZipFile);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(legalXmlZipFolder);
                    }
                
                    var sourceFilePath = Path.Combine(sourceFile, fileName);
                    // Actually copy the file to Legal XML Zip path
                    File.Copy(sourceFilePath, legalXmlZipFile);
                    Logger.InfoFormat("LegalXMLZip File copied From Path: [{0}] , To path: [{1}]", sourceFilePath, legalXmlZipFile);

                    //Calling existing code - actually move file to IATA FTP Path */
                    if(UploadFileForFTPPull(sourceFile, fileName, fileFormatType))
                    {
                        /* Delete individual invoice legal xml files */
                        return deleteLegalXmlFilesAfterZipping(sourceFile);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Logger.Info(string.Format("Legal XML Zip FTP Path: {0} not found.", legalXmlZipFolder));
                    return false;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                throw;
            }
        }

        private bool deleteLegalXmlFilesAfterZipping(string sourceFile)
        {
            try
            {
                DirectoryInfo tempXmlFilePath = new DirectoryInfo(sourceFile);
                /* Delete this folder along with sub directories and/or files */
                tempXmlFilePath.Delete(true);
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                return false;
            }
        }
        #endregion

        #region CMP #596: Length of Member Accounting Code to be Increased to 12
       
        /// <summary>
        /// Since the length of the Accounting Code is increased from 3 to 12, patterns used for matching the naming convention is altered.
        /// Function gets fileName without extension and returns billing member numeric code using file name.
        /// Sandbox file is expected to be provided as input after removing ST- kind of prefix.
        /// Method is open ended and has to be added with more switch cases as and when required. 
        /// </summary>
        /// <param name="fileNameWithoutExtension"></param>
        /// <param name="fileFormatType"></param>
        /// <returns></returns>
        public string GetBillingCode(string fileNameWithoutExtension, FileFormatType fileFormatType)
        {
            string billingCode = string.Empty;
            int constantLengthOfFileNameAfterBillingCode = 0;

            try
            {
                if(fileFormatType != FileFormatType.None && !string.IsNullOrEmpty(fileNameWithoutExtension))
                {
                    switch (fileFormatType)
                    {
                        case FileFormatType.IsIdec:
                        case FileFormatType.IsXml:
                            /* E.g. PIDECF-2014120420150106172800, CXMLF-2014120420150106172900 */
                            constantLengthOfFileNameAfterBillingCode = 22;
                            billingCode = GetBillingNumericCode(fileNameWithoutExtension, constantLengthOfFileNameAfterBillingCode);
                            break;
                        case FileFormatType.Isr:
                        case FileFormatType.Usage:
                        case FileFormatType.SupportingDoc:
                        case FileFormatType.FormCXml:
                            /* E.g. PISR-05720150106172731, PUSF-00120150106120539, DSDF-001I2015010600106, PXFCF-12520150106174000 */
                            constantLengthOfFileNameAfterBillingCode = 14;
                            billingCode = GetBillingNumericCode(fileNameWithoutExtension, constantLengthOfFileNameAfterBillingCode);
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                billingCode = string.Empty;
                throw;
            }

            return billingCode;
        }

        /// <summary>
        /// Helper function to get billing member numeric code generically
        /// </summary>
        /// <param name="fileNameWithoutExtension"></param>
        /// <param name="constantLengthOfFileNameAfterBillingCode"></param>
        /// <param name="billingCodeIndexAfterSplitOnDash"></param>
        /// <returns></returns>
        private string GetBillingNumericCode(string fileNameWithoutExtension, int constantLengthOfFileNameAfterBillingCode, int billingCodeIndexAfterSplitOnDash = 1)
        {
            string billingCode = string.Empty;

            try
            {
                fileNameWithoutExtension =
                        fileNameWithoutExtension.Remove(fileNameWithoutExtension.Length - constantLengthOfFileNameAfterBillingCode);

                char[] splitOnChar = { '-' };

                billingCode = fileNameWithoutExtension.Split(splitOnChar)[billingCodeIndexAfterSplitOnDash];
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
                billingCode = string.Empty;
                throw;
            }
            return billingCode;
        }

        #endregion

        //CMP#622: MISC Outputs Split as per Location ID
        /// <summary>
        /// Gets the location account ids.
        /// </summary>
        /// <param name="inputFile">The input file.</param>
        /// <param name="memberTechConfig">The member tech config.</param>
        /// <param name="isFileTypeConfigured">The is file type configured.</param>
        /// <param name="isLocAccountAvailable">if set to <c>true</c> [is loc account available].</param>
        /// <param name="isMainAccountAvailable">if set to <c>true</c> [is main account available].</param>
        /// <param name="memberLocationId">The member location id.</param>
        /// <returns></returns>
        public ArrayList GetLocationAccountIds(IsInputFile inputFile, TechnicalConfiguration memberTechConfig, int isFileTypeConfigured,out bool isLocAccountAvailable,out bool isMainAccountAvailable,out int memberLocationId)
        {
            ArrayList accountIdArrayList = new ArrayList();
            isLocAccountAvailable = false;
            isMainAccountAvailable = false;
            memberLocationId = 0;

            if (inputFile.BillingCategory != null && (int)inputFile.BillingCategory == (int)BillingCategoryType.Misc && (!string.IsNullOrWhiteSpace(inputFile.MiscLocationCode) || isFileTypeConfigured == 5 || isFileTypeConfigured == 6))
            {
                int memberId = Convert.ToInt32(inputFile.SenderReceiver);
                string locationCode = inputFile.MiscLocationCode;
                
                //CMP#622: MISC Outputs Split as per Location ID
                var locationRepository = Ioc.Resolve<ILocationRepository>(typeof(ILocationRepository));
                var memberlocation = locationRepository.Get(memloc => memloc.MemberId == memberId && memloc.LocationCode == locationCode).FirstOrDefault();
                if (memberlocation != null )
                {
                    memberLocationId = memberlocation.Id;
                    var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                    var memMiscConfig = memberManager.GetMiscellaneousConfiguration(memberId);
                    //If location iiNet account ID is available 
                    if (!string.IsNullOrWhiteSpace(memberlocation.LociiNetAccountId))
                    {
                        accountIdArrayList.Add(memberlocation.LociiNetAccountId);
                        isLocAccountAvailable = true;
                        //Misc Config set to Receive Copy to Main Location
                       
                        //Add Main Location iiNet account Id in array list
                        if(memMiscConfig!= null && memMiscConfig.RecCopyOfLocSpecificMISCOutputsAtMain==true)
                        {
                            if (memberTechConfig != null && memberTechConfig.MiscAccountId != null && !string.IsNullOrWhiteSpace(memberTechConfig.MiscAccountId) && memberTechConfig.MiscAccountId != memberlocation.LociiNetAccountId)
                            {
                                accountIdArrayList.Add(memberTechConfig.MiscAccountId);
                                isMainAccountAvailable = true;
                            }
                        }
                    }
                    //If location iiNet account ID is not available and Main iiNet account Id is available
                    else if (memberTechConfig != null && memberTechConfig.MiscAccountId != null && !string.IsNullOrWhiteSpace(memberTechConfig.MiscAccountId))
                    {
                        
                        accountIdArrayList.Add(memberTechConfig.MiscAccountId);
                        //if received copy at main location then sent alert to main contact other wise to location contacts.
                        if (memMiscConfig != null && memMiscConfig.RecCopyOfLocSpecificMISCOutputsAtMain)
                        {
                            isMainAccountAvailable = true;
                        }
                        else
                        {
                            isLocAccountAvailable = true;
                        }
                    }
                    else
                    {
                        //email send to location contact
                        isLocAccountAvailable = true;
                    }
                }
                else if (memberTechConfig != null && memberTechConfig.MiscAccountId != null && !string.IsNullOrWhiteSpace(memberTechConfig.MiscAccountId))
                {
                    accountIdArrayList.Add(memberTechConfig.MiscAccountId);
                    isMainAccountAvailable = true;
                }
            }
            return accountIdArrayList;
        }
    }

}
