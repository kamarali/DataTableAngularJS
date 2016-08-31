using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Business.Reports.OfflineReportManger;
using log4net;
using System.Reflection;
using Iata.IS.Model.Reports.ReceivablesReport;
using Iata.IS.Model.Enums;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.Common;
using System.IO;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.ReceivablesReport;
using Iata.IS.Model.OfflineReportLog;

namespace Iata.IS.Business.Reports.OfflineReports.Imp
{

	/// <summary>
	/// Generate rmbmcm report.
	/// </summary>
	public class PaxReceivableRmBmCmReport : OfflineReportBase, IPaxReceivableRmBmCmReport
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private IReceivablesReport _receivablesReport;
		private IOfflineReportLogManager _offlineReportLogManager; 

		/// <summary>
		/// Initialize Ioc Components.
		/// </summary>
		private void InitializeIocComponents()
		{
			_receivablesReport = Ioc.Resolve<IReceivablesReport>();
			_offlineReportLogManager = Ioc.Resolve<IOfflineReportLogManager>();
		}

		/// <summary>
		/// Default constructor will initialize Ioc component.
		/// </summary>
		public PaxReceivableRmBmCmReport()
		{
			InitializeIocComponents();
		}

		/// <summary>
		/// This function is used to generate rmbmcm receivable report for pax.
		/// </summary>
		/// <param name="message"></param>
		public void GeneratePaxReceivableRmBmCmReport(ReportDownloadRequestMessage message)
		{
      //Deserialize input object and cast based on model.
			var searchCriteria =
				(RmBmCmReceivableReportModel) ConvertUtil.DeSerializeXml(message.InputData, typeof (RmBmCmReceivableReportModel));

			Logger.InfoFormat("Retrieving Generated rmbmcm receivable report for Transaction Id : {0}", message.RecordId);

      //Get FTP path.
			string memberFtpPath = FileIo.GetForlderPath(SFRFolderPath.ReceivableRmbmcmSummaryReportPath);

			//Create ftp member path if not exist.
			if (!Directory.Exists(memberFtpPath))
				Directory.CreateDirectory(memberFtpPath);

			var reportFileName = Path.Combine(memberFtpPath, searchCriteria.FileName);

			Logger.InfoFormat("RmBmCm receivable report file path: '{0}'", reportFileName);

			int billingPeriod = searchCriteria.FromPeriod == -1 ? 0 : searchCriteria.FromPeriod;
			int settlementMethod = String.IsNullOrEmpty(searchCriteria.SettlementMethod)
			                       	? -1
			                       	: Convert.ToInt32(searchCriteria.SettlementMethod);
			string invoiceNo = (String.IsNullOrEmpty(searchCriteria.InvoiceNo)) ? String.Empty : searchCriteria.InvoiceNo;
			int memberCode = (String.IsNullOrEmpty(searchCriteria.BilledEntityCodeId.ToString())) ? 0 : searchCriteria.BilledEntityCodeId;
			string rmBmCmNo = (String.IsNullOrEmpty(searchCriteria.RMBMCMNo)) ? String.Empty : searchCriteria.RMBMCMNo;

			//Retrieve data from database based on search criteria.
			var filteredList = _receivablesReport.GetReceivablesReportDetails(message.RequestingMemberId,
			                                                                  searchCriteria.FromBillingMonth,
			                                                                  searchCriteria.FromBillingYear, billingPeriod,
			                                                                  settlementMethod,
			                                                                  Convert.ToInt32(searchCriteria.MemoType),
			                                                                  Convert.ToInt32(searchCriteria.SubmissionMethodId),
			                                                                  invoiceNo, memberCode,
			                                                                  rmBmCmNo, searchCriteria.SourceCodes);

			Logger.InfoFormat("Loading data from database for rmbmcm receivable reoprt.");

			//Create csv list model.
			List<PaxReceivablesRmBmCmSummaryReport> csvListModel = GetCsvListModel(filteredList);

			//Generate csv with search criteria and create zip file.
			GenerateZipFile(csvListModel, searchCriteria.SearchCriteriaParams, searchCriteria.BroweserDateTime, reportFileName);

			Logger.InfoFormat("Sending email to user");
      
      //Send mail to user.
			SendEmail(searchCriteria.OfflineReportLogId, message.UserId, reportFileName + ".ZIP", message.DownloadUrl,
			          searchCriteria.FileName,
			          _offlineReportLogManager.GetOfflineReportName((int) OfflineReportType.PassengerReceivablesRmbmcmSummary),
			          searchCriteria.SearchCriteriaParams);

			Logger.InfoFormat("Finished execution of rmbmcm receivable report");
		}

		/// <summary>
		/// This function is used to create csv list model for rmbmcm report. 
		/// </summary>
		/// <param name="filteredList"></param>
		/// <returns></returns>
		private static List<PaxReceivablesRmBmCmSummaryReport> GetCsvListModel(IEnumerable<ReceivablesReportModel> filteredList)
		{
			var csvListModel = new List<PaxReceivablesRmBmCmSummaryReport>();

			if (filteredList != null)
			{
				csvListModel.AddRange(filteredList.Select(
					receivablesReportModel => new PaxReceivablesRmBmCmSummaryReport
					                          	{
					                          		BillingMonth = receivablesReportModel.BillingMonth,
					                          		PeriodNo = receivablesReportModel.FromPeriod,
					                          		SettlementMethod = receivablesReportModel.SettlementMethod,
					                          		BilledEntityCode = receivablesReportModel.BilledEntityCode,
					                          		InvoiceNo = receivablesReportModel.InvoiceNo,
					                          		MemoType = receivablesReportModel.MemoType,
					                          		MemoNo = receivablesReportModel.MemoNo,
					                          		Stage = receivablesReportModel.Stage,
					                          		ReasonCode = receivablesReportModel.ReasonCode,
					                          		CurrencyCode = receivablesReportModel.CurrencyCode,
					                          		TotalGrossAmt = receivablesReportModel.TotalGrossAmt,
					                          		TotalUatpAmt = receivablesReportModel.TotalUATPAmt,
					                          		TotalHandlingFeeAmt = receivablesReportModel.TotalHandlingFeeAmt,
					                          		TotalOtherCommAmt = receivablesReportModel.TotalOtherCommAmt,
					                          		TotalIscAmount = receivablesReportModel.TotalISCAmount,
					                          		TotalVatAmount = receivablesReportModel.TotalVatAmount,
					                          		TotalTaxAmount = receivablesReportModel.TotalTaxAmount,
					                          		TotalNetAmount = receivablesReportModel.TotalNetAmount,
					                          		NoOfLinkedCoupons = receivablesReportModel.NoofLinkCpns,
					                          		AttachmentIndicator = receivablesReportModel.AttachmentIndicator,
					                          		//CMP-523: Source Code in Passenger RM BM CM Summary Reports
					                          		SourceCode = receivablesReportModel.SourceCode
					                          	}));
			}
			return csvListModel;
		}
	}
}
