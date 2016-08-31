using System;
using System.Collections.Generic;
using System.Linq;
using Iata.IS.Business.Reports.OfflineReportManger;
using Iata.IS.Business.Reports.PayablesReport;
using log4net;
using Iata.IS.Core.DI;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Core;
using Iata.IS.Business.FileCore;
using System.IO;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Common;
using Iata.IS.Model.Reports.PayablesReport;
using System.Reflection;

namespace Iata.IS.Business.Reports.OfflineReports.Imp
{
	public class PaxPayableRmBmCmReport : OfflineReportBase, IPaxPayableRmBmCmReport
	{
		private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		private IPayablesReport _payablesReport;
		private IOfflineReportLogManager _offlineReportLogManager; 

		/// <summary>
		/// Initialize Ioc Components.
		/// </summary>
		/// Need to be remove, code move to default constuctor.
		private void InitializeIocComponents()
		{
			_payablesReport = Ioc.Resolve<IPayablesReport>();
			_offlineReportLogManager = Ioc.Resolve<IOfflineReportLogManager>();
		}

		/// <summary>
		/// Default constructor will initialize Ioc component.
		/// </summary>
		public PaxPayableRmBmCmReport()
		{
			InitializeIocComponents();
		}

		/// <summary>
		/// This function is used to generate rmbmcm receivable report for pax.
		/// </summary>
		/// <param name="message"></param>
		public void GeneratePaxPaybleRmBmCmReport(ReportDownloadRequestMessage message)
		{
			var searchCriteria =
				(RmBmCmPayableReportModel) ConvertUtil.DeSerializeXml(message.InputData, typeof (RmBmCmPayableReportModel));

			Logger.InfoFormat("Retrieving Generated rmbmcm payable report for Transaction Id : {0}", message.RecordId);

			string memberFtpPath = FileIo.GetForlderPath(SFRFolderPath.PayableRmbmcmSummaryReportPath);

			//Create ftp member path if not exist.
			if (!Directory.Exists(memberFtpPath))
				Directory.CreateDirectory(memberFtpPath);

			var reportFileName = Path.Combine(memberFtpPath, searchCriteria.FileName);

			Logger.InfoFormat("RmBmCm payable report file path: '{0}'", reportFileName);

			int billingPeriod = searchCriteria.FromPeriod == -1 ? 0 : searchCriteria.FromPeriod;
			int settlementMethod = String.IsNullOrEmpty(searchCriteria.SettlementMethod)
			                       	? -1
			                       	: Convert.ToInt32(searchCriteria.SettlementMethod);
			string invoiceNo = (String.IsNullOrEmpty(searchCriteria.InvoiceNo)) ? String.Empty : searchCriteria.InvoiceNo;
			int memberCode = (String.IsNullOrEmpty(searchCriteria.BillingEntityCodeId.ToString())) ? 0 : searchCriteria.BillingEntityCodeId;
			string rmBmCmNo = (String.IsNullOrEmpty(searchCriteria.RMBMCMNo)) ? String.Empty : searchCriteria.RMBMCMNo;

			//Retrieve data from database based on search criteria.
			var filteredList = _payablesReport.GetPayablesReportDetails(message.RequestingMemberId,
			                                                            searchCriteria.FromBillingMonth,
			                                                            searchCriteria.FromBillingYear, billingPeriod,
			                                                            settlementMethod,
			                                                            Convert.ToInt32(searchCriteria.MemoType),
			                                                            invoiceNo, memberCode,
			                                                            rmBmCmNo, searchCriteria.SourceCodes);

			Logger.InfoFormat("Loading data from database for rmbmcm payable reoprt.");

			//Create csv list model.
			List<PaxPayablesRmBmCmSummaryReport> csvListModel = GetCsvListModel(filteredList);

			//Generate csv with search criteria and create zip file.
			GenerateZipFile(csvListModel, searchCriteria.SearchCriteriaParams, searchCriteria.BrowserDateTime, reportFileName);

			Logger.InfoFormat("Sending email to user");

			SendEmail(searchCriteria.OfflineReportLogId, message.UserId, reportFileName + ".ZIP", message.DownloadUrl,
			          searchCriteria.FileName,
			          _offlineReportLogManager.GetOfflineReportName((int) OfflineReportType.PassengerPayablesRmbmcmSummary),
			          searchCriteria.SearchCriteriaParams);

			Logger.InfoFormat("Finished execution of rmbmcm payable report");
		}

		/// <summary>
		/// This function is used to create csv list model for rmbmcm report. 
		/// </summary>
		/// <param name="filteredList"></param>
		/// <returns></returns>
		private static List<PaxPayablesRmBmCmSummaryReport> GetCsvListModel(IEnumerable<PayablesReportModel> filteredList)
		{
			var csvListModel = new List<PaxPayablesRmBmCmSummaryReport>();

			if (filteredList != null)
			{
				csvListModel.AddRange(filteredList.Select(
					payablesReportModel => new PaxPayablesRmBmCmSummaryReport
					                       	{
					                       		BillingMonth = payablesReportModel.BillingMonth,
					                       		PeriodNo = payablesReportModel.FromPeriod,
					                       		SettlementMethod = payablesReportModel.SettlementMethod,
					                       		BillingEntityCode = payablesReportModel.BillingEntityCode,
					                       		InvoiceNo = payablesReportModel.InvoiceNo,
					                       		MemoType = payablesReportModel.MemoType,
					                       		MemoNo = payablesReportModel.MemoNo,
					                       		Stage = payablesReportModel.Stage,
					                       		ReasonCode = payablesReportModel.ReasonCode,
					                       		CurrencyCode = payablesReportModel.CurrencyCode,
					                       		TotalGrossAmt = payablesReportModel.TotalGrossAmt,
					                       		TotalUatpAmt = payablesReportModel.TotalUATPAmt,
					                       		TotalHandlingFeeAmt = payablesReportModel.TotalHandlingFeeAmt,
					                       		TotalOtherCommAmt = payablesReportModel.TotalOtherCommAmt,
					                       		TotalIscAmount = payablesReportModel.TotalISCAmount,
					                       		TotalVatAmount = payablesReportModel.TotalVatAmount,
					                       		TotalTaxAmount = payablesReportModel.TotalTaxAmount,
					                       		TotalNetAmount = payablesReportModel.TotalNetAmount,
					                       		NoOfLinkedCoupons = payablesReportModel.NoofLinkCpns,
					                       		AttachmentIndicator = payablesReportModel.AttachmentIndicator,
					                       		//CMP-523: Source Code in Passenger RM BM CM Summary Reports
					                       		SourceCode = payablesReportModel.SourceCode
					                       	}));
			}
			return csvListModel;
		}
	}
}
