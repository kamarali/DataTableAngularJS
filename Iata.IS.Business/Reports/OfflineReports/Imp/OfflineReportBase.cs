using System;
using System.Collections.Generic;
using Iata.IS.Model.Common;
using Iata.IS.Business.BroadcastMessages;
using iPayables.UserManagement;
using NVelocity;
using System.IO;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using Iata.IS.AdminSystem;
using log4net;
using System.Reflection;
using Iata.IS.Business.FileCore;
using Iata.IS.Core.File;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Business.Reports.OfflineReportManger;

namespace Iata.IS.Business.Reports.OfflineReports.Imp
{
  /// <summary>
  /// OfflineReportBase class(This class will contain common method which is related to offline report).
  /// </summary>
	public class OfflineReportBase
	{
		public const string DateFormat = "dd-MMM-yyyy HH:mm";
		// Logger instance.
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private readonly IOfflineReportLogManager _offlineReportLogManager;

   public OfflineReportBase()
   {
   	_offlineReportLogManager = Ioc.Resolve<IOfflineReportLogManager>(typeof (IOfflineReportLogManager));
   }

  	/// <summary>
		/// This function is used for send mail to user with report download link.
		/// </summary>
		/// <param name="offlineReportLogId"></param>
		/// <param name="userId"></param>
		/// <param name="reportZipFilePath"></param>
		/// <param name="downloadUrl"></param>
		/// <param name="fileName"></param>
		/// <param name="reportName"></param>
		public void SendEmail(Guid offlineReportLogId, int userId,string reportZipFilePath,string downloadUrl, string fileName, string reportName, string searchCriteria)
		{
			//Check report file is exist or not.
			if (File.Exists(reportZipFilePath))
			{
				Logger.InfoFormat("Adding entry of Offline Report : [{0}]", reportZipFilePath);
				SendReportCreationAlert(offlineReportLogId,userId,reportZipFilePath,downloadUrl,reportName, searchCriteria);
			}
			else
				SendReportCreationFailureAlert(fileName);
		}

		/// <summary>
		/// This function is used for send report to user.
		/// </summary>
		/// <param name="offlineReportLogId"></param>
		/// <param name="requestingUserId"></param>
		/// <param name="reportZipFilePath"></param>
		/// <param name="downloadUrl"></param>
		/// <param name="reportName"></param>
		private void SendReportCreationAlert(Guid offlineReportLogId,int requestingUserId, string reportZipFilePath,string downloadUrl,string reportName, string searchCriteria)
		{
			Logger.InfoFormat("Sending report creation mail to user id [{0}]", requestingUserId);

			// Add Http Download Link
			var isHttpDownloadLink = new IsHttpDownloadLink
			                         	{
			                         		FilePath = reportZipFilePath,
			                         		LastUpdatedBy = requestingUserId,
			                         		LastUpdatedOn = DateTime.UtcNow
			                         	};
			_offlineReportLogManager.AddHttpDownloadLink(isHttpDownloadLink);

			var broadcastMessagesManager = Ioc.Resolve<IBroadcastMessagesManager>();
			var authManager = Ioc.Resolve<IUserManagement>();

			var user = authManager.GetUserByUserID(requestingUserId);
			string emailAddress = user == null ? string.Empty : user.Email;

			//Build http url for download.
			string httpUrl = string.Format("{0}?downloadId={1}&offlineReportLogId={2}", downloadUrl, isHttpDownloadLink.Id,
			                               offlineReportLogId);
			Logger.InfoFormat("Send Download Url is {0} to {1}", httpUrl, emailAddress);

			var context = new VelocityContext();
			context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
			context.Put("ReportName", reportName);
			context.Put("UtcDateTime", DateTime.UtcNow.ToString(DateFormat));
			context.Put("SearchCriteria",searchCriteria.Replace("Search Criteria :",""));
			broadcastMessagesManager.SendOutputFileAvailableNotification(emailAddress, httpUrl,
			                                                             EmailTemplateId.ReportDownloadNotification,
			                                                             context);
			var offileReportLog = _offlineReportLogManager.GetOfflineReportLog(offlineReportLogId);

			offileReportLog.ReportGenerateDateTime = DateTime.Now;
			offileReportLog.DownloadLinkId = isHttpDownloadLink.Id;
			offileReportLog.LastUpdatedBy = requestingUserId;
			_offlineReportLogManager.UpdateOfflineReportLog(offileReportLog);

			Logger.InfoFormat("Http URL :[{0}]", httpUrl);
		}

		public void SendReportCreationFailureAlert(string fileName)
		{
			Logger.InfoFormat("Creation of audit trail package {0} failed", fileName);
		}

		/// <summary>
		/// This function is used to generate zip file for rmbmcm report.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="csvListModel"></param>
		/// <param name="searchCriteria"></param>
		/// <param name="browserDateTime"></param>
		/// <param name="reportFileName"></param>
		public void GenerateZipFile<T>(List<T> csvListModel,string searchCriteria,string browserDateTime,string reportFileName)
		{
			var couponSpecialRecords = new List<SpecialRecord>
			                           	{
			                           		new SpecialRecord
			                           			{
			                           				Cells = new List<SpecialCell>
			                           				        	{
			                           				        		new SpecialCell
			                           				        			{
			                           				        				Key = "BillingMonth",
			                           				        				Data = searchCriteria
			                           				        			}
			                           				        	}
			                           			},
			                           		new SpecialRecord
			                           			{
			                           				Cells = new List<SpecialCell>
			                           				        	{
			                           				        		new SpecialCell
			                           				        			{
			                           				        				Key = "BillingMonth",
			                           				        				Data = browserDateTime
			                           				        			}
			                           				        	}
			                           			}
			                           	};


			// Generate CSV Report for given data.
			CsvProcessor.GenerateCsvReport(csvListModel, reportFileName + ".csv", couponSpecialRecords,
			                               includesearchCriteria: true, isOfflineReport: true);

			//Zip CSV file 
			if (File.Exists(reportFileName + ".csv"))
				FileIo.ZipOutputFile(reportFileName + ".csv");
		}
	}
}
