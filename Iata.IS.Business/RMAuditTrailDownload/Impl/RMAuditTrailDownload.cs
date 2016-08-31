using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Iata.IS.Business.Cargo;
using Iata.IS.Data.Pax;
using Iata.IS.Business.Pax;
using Iata.IS.Business.MemberProfile;
using iPayables.UserManagement;
using Iata.IS.Model.Common;
using System.Reflection;
using Iata.IS.Core.DI;
using Iata.IS.Business.FileCore;
using Iata.IS.Model.Enums;
using System.IO;
using Iata.IS.Business.BroadcastMessages;
using NVelocity;
using Iata.IS.AdminSystem;

namespace Iata.IS.Business.RMAuditTrailDownload.Impl
{
  /// <summary>
  /// This class is used for generate audit trail for rejection memo and send mail to user.
  /// </summary>
  //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
  public class RMAuditTrailDownload : IRMAuditTrailDownload
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private IInvoiceManager _invoiceManager;
    private ICargoInvoiceManager _cgoInvoiceManager;
    private IUserManagement _authManager;

    /// <summary>
    /// Initialize IOC components.
    /// </summary>
    private void InitializeIocComponents()
    {
      _authManager = Ioc.Resolve<IUserManagement>();
      _invoiceManager = Ioc.Resolve<IInvoiceManager>();
      _cgoInvoiceManager = Ioc.Resolve<ICargoInvoiceManager>();
    }

    /// <summary>
    /// This function is used for generate rejection memo audit trail report based on request.
    /// </summary>
    /// <param name="requestMessage"></param>
    public void GenerateRejectionAuditTrailReportZip(ReportDownloadRequestMessage requestMessage, int ProcessingUnitNumber)
    {
      InitializeIocComponents();
      
      //Get member rejection ftp path.
      string rmAuditTrailReportPath = FileIo.GetForlderPath(SFRFolderPath.RMAuditTrailReportPath);
      string reportZipFilePath = string.Empty;

      Logger.InfoFormat("Request id [{0}]", requestMessage.RecordId);

      //Check rejection memo audit trail path exist or not.
      if (!Directory.Exists(rmAuditTrailReportPath))
        Directory.CreateDirectory(rmAuditTrailReportPath);

      switch (requestMessage.BillingCategoryType)
      {
        case BillingCategoryType.Pax:
          Logger.Info("Request for billing category Pax");
          reportZipFilePath = _invoiceManager.CreateRejectionAuditTrailPdf(requestMessage, rmAuditTrailReportPath, ProcessingUnitNumber);
          break;
        case BillingCategoryType.Cgo:
          Logger.Info("Request for billing category Cargo");
          reportZipFilePath = _cgoInvoiceManager.CreateRejectionAuditTrailPdf(requestMessage, rmAuditTrailReportPath, ProcessingUnitNumber);
          break;
      }

      //Check zip path exist or not.
      if (string.IsNullOrEmpty(reportZipFilePath))
      {
        Logger.InfoFormat("Failed to create the report for request id [{0}]", requestMessage.RecordId);
        SendReportCreationFailureAlert(requestMessage.UserId);
      }
      else if (File.Exists(reportZipFilePath))
      {
        try
        {
          //Send mail to user for RM audit trail .
          Logger.InfoFormat("Sending report for Rejection Memo Audit Trail : [{0}]", reportZipFilePath);
          SendReportCreationAlert(requestMessage.UserId, reportZipFilePath, requestMessage.DownloadUrl);
        }
        catch (Exception exception)
        {
          Logger.InfoFormat("Exception occurred [{0}]", exception.Message);
        }
      }
      else
      {
        Logger.InfoFormat("Failed to create the report for request id {0}", requestMessage.StringRecordId);
      }
    }

    /// <summary>
    /// Send mail to user.
    /// </summary>
    /// <param name="requestingUserId"></param>
    /// <param name="reportZipFilePath"></param>
    /// <param name="downloadUrl"></param>
    private void SendReportCreationAlert(int requestingUserId, string reportZipFilePath, string downloadUrl)
    {
      Logger.InfoFormat("Sending report creation mail to member id [{0}]", requestingUserId);

      Data.Impl.UnitOfWork unitOfWork = new Data.Impl.UnitOfWork(new Iata.IS.Data.Impl.ObjectContextAdapter());

      var isHttpDownloadLinkRepository = new Iata.IS.Data.Impl.Repository<IsHttpDownloadLink>(unitOfWork);
      var broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();

      var user = _authManager.GetUserByUserID(requestingUserId);
      var emailAddress = user == null ? string.Empty : user.Email;

      //Add Http Download Link
      var isHttpDownloadLink = new IsHttpDownloadLink
      {
        FilePath = reportZipFilePath,
        LastUpdatedBy = requestingUserId,
        LastUpdatedOn = DateTime.UtcNow
      };
      isHttpDownloadLinkRepository.Add(isHttpDownloadLink);
      unitOfWork.Commit();

      //Build http URL for download.
      var httpUrl = string.Format("{0}/{1}", downloadUrl, isHttpDownloadLink.Id);
      Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

      var context = new VelocityContext();
      context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
      context.Put("FileName", Path.GetFileNameWithoutExtension(reportZipFilePath));
      broadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl, EmailTemplateId.AuditTrailPackageDownloadNotification, context);

      Logger.InfoFormat("Http URL :[{0}]", httpUrl);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestingUserId"></param>
    private void SendReportCreationFailureAlert(int requestingUserId)
    {
      Logger.InfoFormat("Sending report creation failure alert to member id {0}", requestingUserId);
    }
  }
}
