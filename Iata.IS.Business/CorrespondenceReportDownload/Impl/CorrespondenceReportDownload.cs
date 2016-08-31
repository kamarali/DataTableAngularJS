using System;
using System.IO;
using log4net;
using System.Reflection;
using Iata.IS.Model.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Model.Enums;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Core.DI;
using NVelocity;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Cargo;
using Iata.IS.Data.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.MemberProfile;
using iPayables.UserManagement;

namespace Iata.IS.Business.CorrespondenceReportDownload.Impl
{
    public class CorrespondenceReportDownload : ICorrespondenceReportDownload
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ICargoCorrespondenceManager _cargoCorrespondenceManager;
        private IInvoiceRepository _invoiceRepository;
        private IPaxCorrespondenceManager _paxCorrespondenceManager;
        private IMiscCorrespondenceManager _miscCorrespondenceManager;
        private IMemberManager _memberManager;
        private IUserManagement _authManager;

        private void InitializeIocComponents()
        {
            _cargoCorrespondenceManager = Ioc.Resolve<ICargoCorrespondenceManager>();
            _paxCorrespondenceManager = Ioc.Resolve<IPaxCorrespondenceManager>();
            _miscCorrespondenceManager = Ioc.Resolve<IMiscCorrespondenceManager>();
            _memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
            _authManager = Ioc.Resolve<IUserManagement>();
            _invoiceRepository = Ioc.Resolve<IInvoiceRepository>();
        }

        public void GenerateCorrespondenceTrailReportZip(ReportDownloadRequestMessage requestMessage)
        {
            InitializeIocComponents();

            var memberDetails = _memberManager.GetMemberDetails(requestMessage.RequestingMemberId);
            string memberCodeNumeric = memberDetails.MemberCodeNumeric;
            // string memberFtpPath = FileIo.GetFtpDownloadFolderPath(memberCodeNumeric);
            string memberFtpPath = FileIo.GetForlderPath(SFRFolderPath.ISCorrRepFolder);
            string reportZipFilePath = string.Empty;

            Logger.InfoFormat("Request id [{0}]", requestMessage.RecordId);

            if (!Directory.Exists(memberFtpPath))
            {
                Directory.CreateDirectory(memberFtpPath);
            }

            switch (requestMessage.BillingCategoryType)
            {

                case BillingCategoryType.Pax:
                    Logger.Info("Request for billing category Pax");
                    reportZipFilePath = _paxCorrespondenceManager.CreatePaxCorrespondenceTrailPdf(requestMessage, memberFtpPath);
                    break;

                case BillingCategoryType.Cgo:
                    Logger.Info("Request for billing category Cargo");
                    reportZipFilePath = _cargoCorrespondenceManager.CreateCgoCorrespondenceTrailPdf(requestMessage, memberFtpPath);

                    break;

                case BillingCategoryType.Misc:
                    Logger.Info("Request for billing category Misc");
                    reportZipFilePath = _miscCorrespondenceManager.CreateMuCorrespondenceTrailPdf(requestMessage, memberFtpPath);

                    break;

                case BillingCategoryType.Uatp:
                    Logger.Info("Request for billing category UATP");
                    reportZipFilePath = _miscCorrespondenceManager.CreateMuCorrespondenceTrailPdf(requestMessage, memberFtpPath);
                    break;
            }

            if (string.IsNullOrEmpty(reportZipFilePath))
            {
                Logger.InfoFormat("Failed to create the report for request id [{0}]", requestMessage.RecordId);
                SendReportCreationFailureAlert(requestMessage.UserId);

            }
            else if (File.Exists(reportZipFilePath))
            {
                try
                {
                    Logger.InfoFormat("Adding entry of Correspondence report : [{0}]", reportZipFilePath);
                    _invoiceRepository.InsertToCorrReport(reportZipFilePath);
                    SendReportCreationAlert(requestMessage.UserId, reportZipFilePath, requestMessage.DownloadUrl);
                }
                catch (Exception exception)
                {
                    Logger.InfoFormat("Exception occured [{0}]", exception.Message);
                }
            }
            else
            {
                Logger.InfoFormat("Failed to create the report for request id {0}", requestMessage.StringRecordId);
            }
        }

        private void SendReportCreationAlert(int requestingUserId, string reportZipFilePath, string downloadUrl)
        {
            Logger.InfoFormat("Sending report creation mail to member id [{0}]", requestingUserId);

            Data.Impl.UnitOfWork unitOfWork = new Data.Impl.UnitOfWork(new Iata.IS.Data.Impl.ObjectContextAdapter());

            var isHttpDownloadLinkRepository = new Iata.IS.Data.Impl.Repository<IsHttpDownloadLink>(unitOfWork);
            var broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();

            var user = _authManager.GetUserByUserID(requestingUserId);
            var emailAddress = user == null ? string.Empty : user.Email;

            // Add Http Download Link
            var isHttpDownloadLink = new IsHttpDownloadLink
            {
                FilePath = reportZipFilePath,
                LastUpdatedBy = requestingUserId,
                LastUpdatedOn = DateTime.UtcNow
            };
            isHttpDownloadLinkRepository.Add(isHttpDownloadLink);
            unitOfWork.Commit();

            //Build http url for download.
            var httpUrl = string.Format("{0}/{1}", downloadUrl, isHttpDownloadLink.Id);
            Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

            var context = new VelocityContext();
            context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
            broadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl, EmailTemplateId.CorrespondenceReportDownloadNotification, context);

            Logger.InfoFormat("Http URL :[{0}]", httpUrl);
        }

        private void SendReportCreationFailureAlert(int requestingUserId)
        {
            Logger.InfoFormat("Sending report creation failure alert to member id {0}", requestingUserId);
        }
    }
}
