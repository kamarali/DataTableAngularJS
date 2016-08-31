using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Model.Common;
using Iata.IS.Core;
using Iata.IS.Model.Enums;
using log4net;
using System.Reflection;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Business.Cargo;
using Iata.IS.Data.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.MemberProfile;
using iPayables.UserManagement;
using Iata.IS.Core.DI;
using Iata.IS.Model.Base;
using System.IO;
using Iata.IS.Business.FileCore;
using NVelocity;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.AdminSystem;
using Iata.IS.Data;
using Iata.IS.Model.Cargo.BillingHistory;
using Iata.IS.Model.MiscUatp.BillingHistory;

namespace Iata.IS.Business.AuditTrailPackageDownload.Impl
{
    public class AuditTrailPackageDownload : IAuditTrailPackageDownload
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ICargoInvoiceManager CargoInvoiceManager;
        private IMiscUatpInvoiceManager MiscUatpInvoiceManager;
        private INonSamplingInvoiceManager NonSamplingManager;
        private IQueryAndDownloadDetailsManager QueryAndDownloadDetailsManager;


        private void InitializeIocComponents()
        {
            CargoInvoiceManager = Ioc.Resolve<ICargoInvoiceManager>();
            NonSamplingManager = Ioc.Resolve<INonSamplingInvoiceManager>();
            QueryAndDownloadDetailsManager = Ioc.Resolve<IQueryAndDownloadDetailsManager>();
            MiscUatpInvoiceManager = Ioc.Resolve<IMiscUatpInvoiceManager>();
        }

        public void GenerateAuditTrailPackage(ReportDownloadRequestMessage message)
        {
            InitializeIocComponents();
            AuditTrailPackageRequest inputData =
                (AuditTrailPackageRequest)ConvertUtil.DeSerializeXml(message.InputData,
                                                                      typeof(AuditTrailPackageRequest));

            switch (message.BillingCategoryType)
            {
                case BillingCategoryType.Pax:
                    GeneratePaxAuditTrailPackage(message.UserId, message.DownloadUrl, inputData);
                    break;
                case BillingCategoryType.Cgo:
                    GenerateCargoAuditTrailPackage(message.UserId, message.DownloadUrl, inputData);
                    break;
                case BillingCategoryType.Misc:
                case BillingCategoryType.Uatp:
                    GenerateMuAuditTrailPackage(message.UserId, message.DownloadUrl, message.RequestingMemberId,
                                                inputData);
                    break;
            }
        }

        #region PAX

        private void GeneratePaxAuditTrailPackage(int userId, string downloadUrl, AuditTrailPackageRequest inputData)
        {
            //Get audit Trail
            Logger.InfoFormat("Retrieving audit trail for Transaction Id : {0}", inputData.TransactionId);
            PaxAuditTrail auditTrail = NonSamplingManager.GetbillingHistoryAuditTrail(inputData.TransactionId, inputData.TransactionType);

            //Convert it to html
            Logger.InfoFormat("Converting audit Trail to HTML");
            Dictionary<Attachment, int> suppDocs = new Dictionary<Attachment, int>();
            string htmlString = NonSamplingManager.GeneratePaxBillingHistoryAuditTrailPackage(auditTrail, out suppDocs);

            //Create Package
            Logger.InfoFormat("Creating package");
            string reportZipFilePath = CreatePackage(inputData.FileName, htmlString, suppDocs);

            //TODO : purging - add new file created in table

            //SEND Email
            Logger.InfoFormat("Sending email");
            SendEmail(userId, reportZipFilePath, downloadUrl, inputData.FileName);
        }

        #endregion

        #region CARGO

        private void GenerateCargoAuditTrailPackage(int userId, string downloadUrl, AuditTrailPackageRequest inputData)
        {
            // Retrieve auditTrail details for selected transaction
            CargoAuditTrail auditTrail = CargoInvoiceManager.GetBillingHistoryAuditTrail(inputData.TransactionId,
                                                                                         inputData.TransactionType);

            // Generate Audit trail html string through NVelocity 
            Dictionary<Attachment, int> suppDocs = new Dictionary<Attachment, int>();
            string htmlString = CargoInvoiceManager.GenerateCargoBillingHistoryAuditTrailPackage(auditTrail,
                                                                                                 out suppDocs);

            //Create Package
            Logger.InfoFormat("Creating package");
            string reportZipFilePath = CreatePackage(inputData.FileName, htmlString, suppDocs);

            //TODO : purging - add new file created in table

            //SEND Email
            Logger.InfoFormat("Sending email");
            SendEmail(userId, reportZipFilePath, downloadUrl, inputData.FileName);
        }

        #endregion

        #region MISC/UATP

        private void GenerateMuAuditTrailPackage(int userId, string downloadUrl, int memberId, AuditTrailPackageRequest inputData)
        {
            // Retrieve auditTrail details for selected transaction
            AuditTrailPdf auditTrail = MiscUatpInvoiceManager.GetBillingHistoryAuditTrailPdf(inputData.TransactionId);

            // Generate Audit trail html string through NVelocity 
            Dictionary<Attachment, int> suppDocs = new Dictionary<Attachment, int>();
            //suppDocs.
            string htmlString = MiscUatpInvoiceManager.GenerateMisUatpcBillingHistoryAuditTrailPackage(auditTrail, memberId, inputData.TransactionType, out suppDocs);

            //Create Package
            Logger.InfoFormat("Creating package");
            string reportZipFilePath = CreatePackage(inputData.FileName, htmlString, suppDocs);

            //TODO : purging - add new file created in table

            //SEND Email
            Logger.InfoFormat("Sending email");
            SendEmail(userId, reportZipFilePath, downloadUrl, inputData.FileName);
        }

        #endregion

        #region COMMON

        private string CreatePackage(string fileName, string htmlString, Dictionary<Attachment, int> suppDocs)
        {
            string basePath = FileIo.GetForlderPath(SFRFolderPath.ISAuditTrailFolder);

            //Create base Folder
            Logger.InfoFormat("Creating folder {0}", Path.Combine(basePath, fileName));
            Directory.CreateDirectory(Path.Combine(basePath, fileName));

            //Generate PDF in folder
            Logger.InfoFormat("Generating pdf at {0}", Path.Combine(basePath, fileName, fileName));
            GenerateAuditTrailPdfFromHtmlString(htmlString, Path.Combine(basePath, fileName, fileName) + ".PDF");

            //copy supporting docs
            if (suppDocs.Count > 0)
            {
                Logger.InfoFormat("Creating folder {0}", Path.Combine(basePath, fileName, "SUPPDOCS"));
                string suppDocsPath = Path.Combine(basePath, fileName, "SUPPDOCS");
                Directory.CreateDirectory(suppDocsPath);
                Logger.InfoFormat("Copying supporting documents");
                CopySupportingDocuments(suppDocs, suppDocsPath);
            }

            //Zip main Folder
            FileIo.ZipOutputFolder(Path.Combine(basePath, fileName), Path.Combine(basePath, fileName) + ".zip");
            return string.Format("{0}.zip", Path.Combine(basePath, fileName));

        }

        private void GenerateAuditTrailPdfFromHtmlString(string auditTrailHtmlString, string fileLocation)
        {
            string guid = Guid.NewGuid().ToString();

            // wkhtmltopdf.exe file path which converts html string to pdf
            var htmlToPdfExePath = AppDomain.CurrentDomain.BaseDirectory + @"\wkhtmltopdf.exe";
            // Following call converts html file to pdf 
            var file = QueryAndDownloadDetailsManager.ConvertHtmlToPdf(auditTrailHtmlString, string.Format(@"AuditTrail_{0}", guid), htmlToPdfExePath);
            // Write all file content
            File.WriteAllBytes(fileLocation, file);
        }

        private void CopySupportingDocuments(Dictionary<Attachment, int> suppDocs, string basePath)
        {
            foreach (var folderEntry in suppDocs.GroupBy(e => e.Value))
            {
                string folderPath = Path.Combine(basePath, folderEntry.First().Value.ToString());
                Directory.CreateDirectory(folderPath);
                foreach (var attachment in folderEntry)
                {
                    try
                    {
                        string attachmentLocation = FileIo.GetAttachmentPath(attachment.Key);
                        Logger.InfoFormat("Copying file {0} from {1} to {2}", attachment.Key.OriginalFileName, attachmentLocation, Path.Combine(folderPath, attachment.Key.OriginalFileName));
                        File.Copy(attachmentLocation, Path.Combine(folderPath, attachment.Key.OriginalFileName));
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat(@"Exception occured while copying file : {0} \n {1}",
                                           attachment.Key.OriginalFileName, ex.Message);
                    }
                }
            }
        }

        private void SendEmail(int userId, string reportZipFilePath, string downloadUrl, string fileName)
        {
            if (File.Exists(reportZipFilePath))
            {
                Logger.InfoFormat("Adding entry of audit trail package : [{0}]", reportZipFilePath);
                SendReportCreationAlert(userId, reportZipFilePath, downloadUrl, fileName);
            }
            else
                SendReportCreationFailureAlert(fileName);
        }

        private void SendReportCreationAlert(int requestingUserId, string reportZipFilePath, string downloadUrl, string fileName)
        {
            Logger.InfoFormat("Sending report creation mail to user id [{0}]", requestingUserId);

            Iata.IS.Data.Impl.UnitOfWork unitOfWork = new Iata.IS.Data.Impl.UnitOfWork(new Iata.IS.Data.Impl.ObjectContextAdapter());

            var isHttpDownloadLinkRepository = new Iata.IS.Data.Impl.Repository<IsHttpDownloadLink>(unitOfWork);

            // Add Http Download Link
            IsHttpDownloadLink isHttpDownloadLink = new IsHttpDownloadLink
            {
                FilePath = reportZipFilePath,
                LastUpdatedBy = requestingUserId,
                LastUpdatedOn = DateTime.UtcNow
            };
            isHttpDownloadLinkRepository.Add(isHttpDownloadLink);
            unitOfWork.Commit();

            IBroadcastMessagesManager broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();
            IUserManagement authManager = Ioc.Resolve<IUserManagement>();

            I_ISUser user = authManager.GetUserByUserID(requestingUserId);
            string emailAddress = user == null ? string.Empty : user.Email;

            //Build http url for download.
            string httpUrl = string.Format("{0}/{1}", downloadUrl, isHttpDownloadLink.Id);
            Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

            VelocityContext context = new VelocityContext();
            context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
            context.Put("FileName", fileName);
            broadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl, EmailTemplateId.AuditTrailPackageDownloadNotification, context);

            Logger.InfoFormat("Http URL :[{0}]", httpUrl);
        }

        private void SendReportCreationFailureAlert(string fileName)
        {
            Logger.InfoFormat("Creation of audit trail package {0} failed", fileName);
        }

        #endregion
    }
}
