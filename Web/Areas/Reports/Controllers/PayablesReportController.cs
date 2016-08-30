using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.PayablesReport;
using Iata.IS.Core;
using Iata.IS.Core.File;
using Iata.IS.Model.Common;
using Iata.IS.Model.Reports.PayablesReport;
using Iata.IS.Web.Util;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util.Filters;
using log4net;
using BillingType = Iata.IS.Model.Enums.BillingType;
using Iata.IS.Model.OfflineReportLog;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Business.Reports.OfflineReportManger;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class PayablesReportController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    private readonly IPayablesReport _payablesReport;
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IReferenceManager ReferenceManager { get; set; }
		private readonly IOfflineReportLogManager _offlineReportLogManager;

		public PayablesReportController(ICalendarManager calenderManager,IPayablesReport payablesReport,IOfflineReportLogManager offlineReportLogManager)
    {
      _calenderManager = calenderManager;
      _payablesReport = payablesReport;
			_offlineReportLogManager = offlineReportLogManager;
    }

    //
    // GET: /Reports/ReceivablesReport/
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.PayablesRMBMCMSummaryAccess)]
    public ActionResult PayablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Pax;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;

      // CMP-523: Source Code in Passenger RM BM CM Summary Reports
      // For pre-population of the source codes for Memo Type = "All".
      var sourceCodes = ReferenceManager.GetSourceCodesList((int)TransactionType.None);
      var sourceCodeNonBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == false).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();
      var sourceCodeBilateral = sourceCodes.Where(sc => sc.IsBilateralCode).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();

      var list = sourceCodeNonBilateral.Union(sourceCodeBilateral);

      ViewData["SourceCodeList"] = new MultiSelectList(list, "SourceCodeIdentifier", "SourceCodeIdentifier");
      return View();
    }

    /// <summary>
    /// Post method to Queue the report, It will generate by Offline
    /// </summary>
    /// <param name="payablesReportModel"> Payables Report Model </param>
    /// <param name="searchCriteria"> Search Criteria</param>
    /// <param name="broweserDateTime"> Browser Date Time </param>
    /// <returns> File or View </returns>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.PayablesRMBMCMSummaryAccess)]
    public ActionResult PayablesReport(PayablesReportModel payablesReportModel, string searchCriteria, string broweserDateTime)
    {
    	RmBmCmPayableReportModel rmBmCmPayableReportModel = null;
      // SCP386273: SRM:Exception occurred in Report Download Service. - SIS Production
      if (string.IsNullOrEmpty(searchCriteria) || string.IsNullOrEmpty(broweserDateTime))
      {
        _logger.InfoFormat("searchCriteria: {0} and broweserDateTime: {1}", searchCriteria, broweserDateTime);
        _logger.InfoFormat("either searchCriteria is null or broweserDateTime is null.");
        ShowErrorMessage("An internal error occurred while generating the report, please try generating the report again.");
      }
      else
      {
        try
        {
          //Create rmbmcm payable report model for generate offline report.
          rmBmCmPayableReportModel = new RmBmCmPayableReportModel
                                       {
                                           FromBillingMonth = payablesReportModel.FromBillingMonth,
                                           FromBillingYear = payablesReportModel.FromBillingYear,
                                           FromPeriod = payablesReportModel.FromPeriod,
                                           SettlementMethod = payablesReportModel.SettlementMethod,
                                           MemoType = payablesReportModel.MemoType,
                                           InvoiceNo = payablesReportModel.InvoiceNo,
                                           BillingEntityCodeId = payablesReportModel.BillingEntityCodeId,
                                           RMBMCMNo = payablesReportModel.RMBMCMNo,
                                           SourceCodes =
                                               (String.IsNullOrEmpty(Request.Form["SourceCode"]))  ? "" : Request.Form["SourceCode"],
                                           SearchCriteriaParams = searchCriteria,
                                           BrowserDateTime = broweserDateTime
                                       };

          //Create Offline report log object for inserting data into OfflineReportlog table.
          var offlineReportLog = new OfflineReportLog
                                   {
                                       Id = Guid.NewGuid(),
                                       MemberId = SessionUtil.MemberId,
                                       UserId = SessionUtil.UserId,
                                       RequestDateTime = DateTime.UtcNow,
                                       OfflineReportId = (Int32) OfflineReportType.PassengerPayablesRmbmcmSummary,
                                       SearchCriterion = searchCriteria,
                                       DownloadLinkId = null
                                   };
          rmBmCmPayableReportModel.OfflineReportLogId = offlineReportLog.Id;
          rmBmCmPayableReportModel.FileName = offlineReportLog.Id.ToString();

          //Create a enque message object for generate offline report.
          var enqueMessage = new ReportDownloadRequestMessage
                               {
                                   RecordId = Guid.NewGuid(),
                                   BillingCategoryType = BillingCategoryType.Pax,
                                   UserId = SessionUtil.UserId,
                                   RequestingMemberId = SessionUtil.MemberId,
                                   InputData =  ConvertUtil.SerializeXml(rmBmCmPayableReportModel, rmBmCmPayableReportModel.GetType()),
                                   DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadReport","OfflineReports", new {area = "Reports"})),
                                   OfflineReportType = OfflineReportType.PassengerPayablesRmbmcmSummary
                               };

          //insert data into offlinereportlog table.
          _offlineReportLogManager.AddOfflineReportLog(offlineReportLog);

          //Message will display on screen depending on Success or Failure of Enqueing message to queue.
          ReferenceManager.EnqueTransactionTrailReport(enqueMessage);
          ShowSuccessMessage(Business.Constants.OfflineReportMessage);
        }
        catch (Exception)
        {
          ShowErrorMessage("Failed to download the receivable rmbmcm report, please try again!");
        }
      }
      ViewData["CategoryName"] = BillingCategoryType.Pax;
    	ViewData["currentYear"] = payablesReportModel.FromBillingYear;
    	ViewData["currentMonth"] = payablesReportModel.FromBillingMonth;
    	ViewData["UserCategory"] = (int) SessionUtil.MemberId;
    	ViewData["Period"] = payablesReportModel.FromPeriod;
    	ViewData[ViewDataConstants.BillingType] = BillingType.Payables;

    	// CMP-523: Source Code in Passenger RM BM CM Summary Reports
    	// For pre-population of the source codes for Memo Type = "All".
      var sourceCodes = ReferenceManager.GetSourceCodesList(Convert.ToInt32(payablesReportModel.MemoType));
    	var sourceCodeNonBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == false).Select(sc => new
    	                                                                                               	{
    	                                                                                               		sc.
    	                                                                                               	SourceCodeIdentifier
    	                                                                                               	}).Distinct().
    		OrderBy(sci => sci.SourceCodeIdentifier).ToList();
    	var sourceCodeBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == true).Select(sc => new
    	                                                                                           	{
    	                                                                                           		sc.
    	                                                                                           	SourceCodeIdentifier
    	                                                                                           	}).Distinct().OrderBy(
    	                                                                                           		sci =>
    	                                                                                           		sci.
    	                                                                                           			SourceCodeIdentifier)
    		.ToList();

    	var list = sourceCodeNonBilateral.Union(sourceCodeBilateral);

    	ViewData["SourceCodeList"] = new MultiSelectList(list, "SourceCodeIdentifier", "SourceCodeIdentifier");

    	return View("PayablesReport");
    }// End of PayablesReport()

   /* /// <summary>
    /// This method is used to get the result in CSV file format.
    /// </summary>
    /// <param name="payablesReportModel"> Payables Report Model </param>
    /// <param name="searchCriteria"> Search Criteria </param>
    /// <param name="broweserDateTime"> Broweser Date Time </param>
    /// <returns> File or Action Method. </returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.PayablesRMBMCMSummaryAccess)]
    [HttpPost]
		public ActionResult PassengerRmBmCmSummaryPayablesCsvReport(PayablesReportModel payablesReportModel,string searchCriteria,string broweserDateTime)
    {
    	RmBmCmPayableReportModel rmBmCmPayableReportModel = null;
    	try
    	{
    		DateTime utcTime = DateTime.UtcNow;
    		rmBmCmPayableReportModel = new RmBmCmPayableReportModel
    		                           	{
    		                           		FromPeriod = payablesReportModel.FromPeriod,
    		                           		SettlementMethod = payablesReportModel.SettlementMethod,
    		                           		MemoType = payablesReportModel.MemoType,
    		                           		InvoiceNo = payablesReportModel.InvoiceNo,
    		                           		BillingEntityCodeId = payablesReportModel.BillingEntityCodeId,
    		                           		RMBMCMNo = payablesReportModel.RMBMCMNo,
    		                           		SourceCodes =
    		                           			(String.IsNullOrEmpty(Request.Form["SourceCode"])) ? "" : Request.Form["SourceCode"],
    		                           		FileName =
    		                           			string.Format("PAX-Payable RMBMCM Report-{0}-{1}-{2}", SessionUtil.UserId,
    		                           			              utcTime.ToString("yyyyMMdd"), utcTime.ToString("HHMMss")),
    		                           		SearchCriteria = searchCriteria,
    		                           		BrowserDateTime = broweserDateTime
    		                           	};

    		var enqueMessage = new ReportDownloadRequestMessage
    		                   	{
    		                   		RecordId = Guid.NewGuid(),
    		                   		BillingCategoryType = BillingCategoryType.Pax,
    		                   		UserId = SessionUtil.UserId,
    		                   		RequestingMemberId = SessionUtil.MemberId,
    		                   		InputData =
    		                   			ConvertUtil.SerializeXml(rmBmCmPayableReportModel, rmBmCmPayableReportModel.GetType()),
    		                   		DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
    		                   		                                                   "Invoice",
    		                   		                                                   new
    		                   		                                                   	{
    		                   		                                                   		area = "Pax",
    		                   		                                                   		billingType = "Receivables"
    		                   		                                                   	})),
    		                   		ProcessInd = OfflineReport.PassengerReceivablesRmbmcmSummary
    		                   	};

    		var offlineReportLog = new OfflineReportLog();
    		offlineReportLog.SearchCriterion = "";

    		_offlineReportLogManager.InsertOfflineReportLog(offlineReportLog);
    		// Message will display on screen depending on Success or Failure of Enqueing message to queue.
    		ReferenceManager.EnqueTransactionTrailReport(enqueMessage);

    		ShowSuccessMessage(string.Format(
    			@"Generation of the receivable rmbmcm report is in progress. You will be notified via 
                        email once it is ready for download. [File: {0}.zip]",
    			rmBmCmPayableReportModel.FileName));

    	}
    	catch (Exception)
    	{
    		ShowErrorMessage("Failed to download the receivable rmbmcm report, please try again!");
    	}
    	ViewData["CategoryName"] = BillingCategoryType.Pax;
    	ViewData["currentYear"] = payablesReportModel.FromBillingYear;
    	ViewData["currentMonth"] = payablesReportModel.FromBillingMonth;
    	ViewData["UserCategory"] = (int) SessionUtil.MemberId;
    	ViewData["Period"] = payablesReportModel.FromPeriod;
    	ViewData[ViewDataConstants.BillingType] = BillingType.Payables;

    	// CMP-523: Source Code in Passenger RM BM CM Summary Reports
    	// For pre-population of the source codes for Memo Type = "All".
    	var sourceCodes = ReferenceManager.GetSourceCodesList(Convert.ToInt32(rmBmCmPayableReportModel.MemoType));
    	var sourceCodeNonBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == false).Select(sc => new
    	                                                                                               	{
    	                                                                                               		sc.
    	                                                                                               	SourceCodeIdentifier
    	                                                                                               	}).Distinct().
    		OrderBy(sci => sci.SourceCodeIdentifier).ToList();
    	var sourceCodeBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == true).Select(sc => new
    	                                                                                           	{
    	                                                                                           		sc.
    	                                                                                           	SourceCodeIdentifier
    	                                                                                           	}).Distinct().OrderBy(
    	                                                                                           		sci =>
    	                                                                                           		sci.
    	                                                                                           			SourceCodeIdentifier)
    		.ToList();

    	var list = sourceCodeNonBilateral.Union(sourceCodeBilateral);

    	ViewData["SourceCodeList"] = new MultiSelectList(list, "SourceCodeIdentifier", "SourceCodeIdentifier");

    	return View("PayablesReport");
    }*/

  	/// <summary>
    /// This method is used to Create CSV File of Passenger Payables RMBMCM Summary Report
    /// </summary>
    /// <param name="reportFileName">Report File Name with location</param>
    /// <param name="billingMonth">Billing Month</param>
    /// <param name="billingYear">Billing Year</param>
    /// <param name="billingPeriod">Billing Period</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="invoiceNo">Invoice Number</param>  
    /// <param name="billingEntityCode">Member Code</param>
    /// <param name="rmBmCmNo">RM/BM/CM Number</param>
    /// <param name="sourceCode">Source Code</param>
    /// <param name="searchCriteria"> Search Criteria </param>
    /// <param name="broweserDateTime"> Broweser Date Time </param>
    /// <returns> string </returns>

    
   private string CreateCsv(string reportFileName, int billingMonth, int billingYear, int billingPeriod, int settlementMethod, int memoType,
                              string invoiceNo, int billingEntityCode, string rmBmCmNo, string sourceCode, string searchCriteria, string broweserDateTime)
    {
        //SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'   
       //remove Submission Method Field
      // Get data for the specified search criteria from database.
      var csvListModel = new List<PaxPayablesRmBmCmSummaryReport>();
      csvListModel.AddRange(GetCsvListModel(billingMonth, billingYear, billingPeriod, settlementMethod, memoType,
                                             invoiceNo, billingEntityCode, rmBmCmNo, sourceCode));

      // If given filter criteria fetches data then generate CSV report for the data else
      // return message to displayed to user notifying him no data was found.
      if (csvListModel.Count() != 0)
      {
        #region Add Special Records
        // Search Criteria and Browser Date Time to report footer.
        
        // Remove "," from Search Criteria string to show in one cell of csv
        //if (!string.IsNullOrEmpty(searchCriteria))
        //{
        //  searchCriteria = searchCriteria.Replace(",", " ");
        //}
        
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
                                                           Data = broweserDateTime
                                                         }
                                                     }
                                         }
                                     };
        #endregion
        // Generate CSV Report for given data.
        CsvProcessor.GenerateCsvReport(csvListModel, reportFileName + ".csv", couponSpecialRecords, includesearchCriteria: true);
      }
      else
      {
        return "The query did not produce any results. Please modify your query criteria.";
      }

      // Zip CSV file 
      if (System.IO.File.Exists(reportFileName + ".csv"))
      {
        // Delete file if its length is zero
        var f = new FileInfo(reportFileName + ".csv");
        if (f.Length == 0)
          System.IO.File.Delete(reportFileName + ".csv");
        else
        {
          // Make zip file available to download.
          FileIo.ZipOutputFile(reportFileName + ".csv");
        }
      }
      return string.Empty;
    } // End of CreateCsv

    /// <summary>
    /// This method is used to get date from database and map it according to Report display model for Passenger Payables RMBMCM Summary Report.
    /// </summary>
    /// <param name="billingMonth">Billing Month</param>  
    /// <param name="billingYear">Billing Year</param>
    /// <param name="billingPeriod">Billing Period</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="invoiceNo">Invoice Number</param>  
    /// <param name="billingEntityCode">Memeber Code</param>
    /// <param name="rmBmCmNo">RM/BM/CM Number</param>
    /// <param name="sourceCode">Source Code</param>
    /// <returns>List of PaxPayablesRmBmCmSummaryReportModel</returns>
    /// 
    private List<PaxPayablesRmBmCmSummaryReport> GetCsvListModel(int billingMonth, int billingYear, int billingPeriod,
                                                                      int settlementMethod, int memoType,
                                                                      string invoiceNo, int billingEntityCode, string rmBmCmNo, string sourceCode)
    {
      // Get data from database as per report search criteria.
        //SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'   
        //remove Submission Method Field

      var filteredList = _payablesReport.GetPayablesReportDetails(SessionUtil.MemberId, billingMonth, billingYear,
                                                                  billingPeriod, settlementMethod,
                                                                  memoType,  invoiceNo,
                                                                  billingEntityCode, rmBmCmNo, sourceCode);

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
    } // End of GetCsvListModel()

    /// <summary>
    /// This method is used to delete oldest files from temp folder.
    /// </summary>
    /// <param name="path">File path</param>
    private void DeleteOldestFilesFromTempFolder(string path)
    {
      try
      {
        var tempDownloadDir = new DirectoryInfo(path);
        foreach (var file in tempDownloadDir.GetFiles().Where(file => file.CreationTimeUtc <= DateTime.UtcNow.AddDays(-1)))
        {
          file.Delete();
        }
      }
      catch (Exception exception)
      {
        _logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1} ", exception.Message, exception.StackTrace);
      }
    }// End DeleteOldestFilesFromTempFolder()

    /// <summary>
    /// Author: Sanket Shrivastava
    /// Purpose: Pax: Sampling Billing Analysis - Payables
    /// </summary>
    /// // GET: /Reports/PayablesReport/
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.PayablesSamplingBillingAnalysisAccess)]
    public ActionResult IwSamplingReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Pax;
      //ViewData["currentYear"] = currentPeriod.Year;
      //ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;
      ViewData["BillingType"] = BillingType.Payables;
      ViewData["BillingTypeId"] = (int)BillingType.Payables;
      ViewData["BillingTypeText"] = "Payables";
      ViewData["MemberType"] = "Billing";
      return View("~/Areas/Reports/Views/ReceivablesReport/OwSamplingReport.aspx");
    }

    public ActionResult CargoPayablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      //var currentPeriod = _calenderManager.GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      //ViewData["currentYear"] = currentPeriod.Year;
      //ViewData["currentMonth"] = currentPeriod.Month;
      //ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      //ViewData["Period"] = currentPeriod.Period;
      ViewData["BillingType"] = BillingType.Payables;
      return View();
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Purpose: For Payables - Passanger Interline Billing Summary Report
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.PayablesInterlineBillSummaryAccess)]
    public ActionResult PaxInterlineBillingSummaryReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;

      ViewData["BillingType"] = BillingType.Payables;
      ViewData["BillingTypeId"] = (int)BillingType.Payables;
      ViewData["BillingTypeText"] = "Payables";
      ViewData["MemberType"] = "Billing";
      return View("~/Areas/Reports/Views/ReceivablesReport/PaxInterlineBillingSummaryReport.aspx");
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Purpose: For Payables - Passenger Rejection Analysis - Non Sampling
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.PayablesNonSampleRejAnalysisAccess)]
    public ActionResult PaxRejectionAnalysisNonSamplingReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      ViewData["BillingType"] = BillingType.Payables;
      ViewData["BillingTypeId"] = (int)BillingType.Payables;
      ViewData["BillingTypeText"] = "Payables";
      ViewData["MemberType"] = "Billing";
      return View("~/Areas/Reports/Views/ReceivablesReport/PaxRejectionAnalysisNonSamplingReport.aspx");
    }
  }
}