using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.ReceivablesReport;
using Iata.IS.Core;
using Iata.IS.Core.File;
using Iata.IS.Model.Common;
using Iata.IS.Model.Reports.ReceivablesReport;
using Iata.IS.Web.Util;
using Iata.IS.Business.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Web.Util.Filters;
using log4net;
using BillingType = Iata.IS.Model.Enums.BillingType;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Business.Reports.OfflineReportManger;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class ReceivablesReportController : ISController
  {
    private readonly ICalendarManager _calenderManager;
    private readonly IReceivablesReport _receivablesReport;
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public IReferenceManager ReferenceManager { get; set; }
		private readonly IOfflineReportLogManager _offlineReportLogManager;

		public ReceivablesReportController(ICalendarManager calenderManager,IReceivablesReport receivablesReport,IOfflineReportLogManager offlineReportLogManager)
    {
      _calenderManager = calenderManager;
      _receivablesReport = receivablesReport;
			_offlineReportLogManager = offlineReportLogManager;
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesRMBMCMSummaryAccess)]
    [HttpGet]
    public ActionResult ReceivablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Pax;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["UserCategory"] = SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;
      ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

      // CMP-523: Source Code in Passenger RM BM CM Summary Reports
      // For pre-population of the source codes for Memo Type = "All".
      var sourceCodes = ReferenceManager.GetSourceCodesList((int) TransactionType.None);
      var sourceCodeNonBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == false).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();
      var sourceCodeBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == true).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList(); 
     
      var list = sourceCodeNonBilateral.Union(sourceCodeBilateral);

      ViewData["SourceCodeList"] = new MultiSelectList(list, "SourceCodeIdentifier", "SourceCodeIdentifier");
      
      return View();
    }

    /// <summary>
		/// Post method to Queue the report, It will generate by Offline.
    /// </summary>
    /// <param name="receivablesReportModel"> ReceivablesReportModel</param>
    /// <param name="searchCriteria"> Search Criteria </param>
    /// <param name="broweserDateTime"> Browser Date Time </param>
    /// <returns> File or View </returns>
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesRMBMCMSummaryAccess)]
    public ActionResult ReceivablesReport(ReceivablesReportModel receivablesReportModel,string searchCriteria, string broweserDateTime)
    {
    	_logger.InfoFormat("RMBMCM Receivable Report: starting to generate RMBMCM Receivable Report.");
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
          var rmBmCmReceivableReportModel = new RmBmCmReceivableReportModel
                                              {
                                                  FromPeriod = receivablesReportModel.FromPeriod,
                                                  SettlementMethod = receivablesReportModel.SettlementMethod,
                                                  InvoiceNo = receivablesReportModel.InvoiceNo,
                                                  BilledEntityCodeId = receivablesReportModel.BilledEntityCodeId,
                                                  RMBMCMNo = receivablesReportModel.RMBMCMNo,
                                                  FromBillingMonth = receivablesReportModel.FromBillingMonth,
                                                  FromBillingYear = receivablesReportModel.FromBillingYear,
                                                  MemoType = receivablesReportModel.MemoType,
                                                  SubmissionMethodId = receivablesReportModel.SubmissionMethodId,
                                                  SourceCodes = (String.IsNullOrEmpty(Request.Form["SourceCode"]))
                                                                    ? ""
                                                                    : Request.Form["SourceCode"],
                                                  SearchCriteriaParams = searchCriteria,
                                                  BroweserDateTime = broweserDateTime
                                              };

          //Create Offline report log object for inserting data into OfflineReportlog table.
          var offlineReportLog = new OfflineReportLog
                                   {
                                       Id = Guid.NewGuid(),
                                       MemberId = SessionUtil.MemberId,
                                       UserId = SessionUtil.UserId,
                                       RequestDateTime = DateTime.UtcNow,
                                       OfflineReportId = (Int32) OfflineReportType.PassengerReceivablesRmbmcmSummary,
                                       SearchCriterion = searchCriteria,
                                       DownloadLinkId = null,
                                       LastUpdatedBy = SessionUtil.UserId
                                   };
          rmBmCmReceivableReportModel.OfflineReportLogId = offlineReportLog.Id;
          rmBmCmReceivableReportModel.FileName = offlineReportLog.Id.ToString();

          //Create a enque message object for generate offline report.
          var enqueMessage = new ReportDownloadRequestMessage
                               {
                                   RecordId = Guid.NewGuid(),
                                   BillingCategoryType = BillingCategoryType.Pax,
                                   UserId = SessionUtil.UserId,
                                   RequestingMemberId = SessionUtil.MemberId,
                                   InputData =
                                       ConvertUtil.SerializeXml(rmBmCmReceivableReportModel, rmBmCmReceivableReportModel.GetType()),
                                        DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadReport", "OfflineReports", new {area = "Reports"})),
                                   OfflineReportType = OfflineReportType.PassengerReceivablesRmbmcmSummary
                               };
          _offlineReportLogManager.AddOfflineReportLog(offlineReportLog);

          // Message will display on screen depending on Success or Failure of Enqueing message to queue.
          ReferenceManager.EnqueTransactionTrailReport(enqueMessage);

          ShowSuccessMessage(Business.Constants.OfflineReportMessage);
        }
        catch (Exception)
        {
          ShowErrorMessage("Failed to download the receivable rmbmcm report, please try again!");
        }
      }

      ViewData["CategoryName"] = BillingCategoryType.Pax;
    	ViewData["currentYear"] = receivablesReportModel.FromBillingYear;
    	ViewData["currentMonth"] = receivablesReportModel.FromBillingMonth;
    	ViewData["UserCategory"] = SessionUtil.MemberId;
    	ViewData["Period"] = receivablesReportModel.FromPeriod;
    	ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

    	// CMP-523: Source Code in Passenger RM BM CM Summary Reports
    	// For pre-population of the source codes for Memo Type = "All".
    	var sourceCodes = ReferenceManager.GetSourceCodesList(Convert.ToInt32(receivablesReportModel.MemoType));
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
    	return View();
    }

  	#region Unsed Function
//    /// <summary>
//    /// This method is used to Generate CSV File of Passenger Receivables RMBMCM Summary Report.
//    /// </summary>
//    /// <param name="receivablesReportModel"> Receivables Report Model </param>
//    /// <param name="searchCriteria"> Search Criteria </param>
//    /// <param name="broweserDateTime"> Broweser Date Time </param>
//    /// <returns> File or Action Method. </returns>
//    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesRMBMCMSummaryAccess)]
//    [HttpPost]
//  public ActionResult PassengerRmBmCmSummaryReceivablesCsvReport(ReceivablesReportModel receivablesReportModel,string searchCriteria,string broweserDateTime)
//    {
  		
//      #region commented

///*
//      var result = new JsonResult();
//      try
//      {
//        //int billingPeriod = receivablesReportModel.FromPeriod == -1 ? 0 : receivablesReportModel.FromPeriod;
//        //int settlementMethod = (String.IsNullOrEmpty(receivablesReportModel.SettlementMethod)) ? -1 : Convert.ToInt32(receivablesReportModel.SettlementMethod);
//        //int memoType = Convert.ToInt32(receivablesReportModel.MemoType);
//        //int submissionMethod = Convert.ToInt32(receivablesReportModel.SubmissionMethodId);
//        //string invoiceNo = (String.IsNullOrEmpty(receivablesReportModel.InvoiceNo)) ? "" : receivablesReportModel.InvoiceNo;
//        //int memberCode = (String.IsNullOrEmpty(receivablesReportModel.BilledEntityCodeId.ToString())) ? 0 : receivablesReportModel.BilledEntityCodeId;
//        //string rmBmCmNo = (String.IsNullOrEmpty(receivablesReportModel.RMBMCMNo)) ? "" : receivablesReportModel.RMBMCMNo;
//        ////CMP-523: Source Code in Passenger RM BM CM Summary Reports
//        //string sourceCode = (String.IsNullOrEmpty(Request.Form["SourceCode"])) ? "" : Request.Form["SourceCode"];

//        DateTime utcTime = DateTime.UtcNow;
//        string fileName = string.Format("PAX-Receivable RMBMCM Report-{0}-{1}-{2}", SessionUtil.UserId,
//                                        utcTime.ToString("yyyyMMdd"), utcTime.ToString("HHMMss"));
//      //receivablesReportModel.SourceCode = String.IsNullOrEmpty(Request.Form["SourceCode"])
//      //                                        ? ""
//      //                                        : Request.Form["SourceCode"];

//        var enqueMessage = new ReportDownloadRequestMessage
//                            {
//                              RecordId = Guid.NewGuid(),
//                              BillingCategoryType = BillingCategoryType.Pax,
//                              UserId = SessionUtil.UserId,
//                              RequestingMemberId = SessionUtil.MemberId,
//                              InputData =
//                                ConvertUtil.SerializeXml(receivablesReportModel, receivablesReportModel.GetType()),
//                              DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadFile",
//                                                                                 "Invoice",
//                                                                                 new
//                                                                                  {
//                                                                                    area = "Pax",
//                                                                                    billingType = "Receivables"
//                                                                                  })),
//                              ProcessInd = ProcessIndicator.ReceivableRmBmCmReportSummary
//                            };

//        // Message will display on screen depending on Success or Failure of Enqueing message to queue.
//        bool isEnqueSuccess = ReferenceManager.EnqueTransactionTrailReport(enqueMessage);

//        result.Data = isEnqueSuccess
//                        ? new UIMessageDetail
//                            {
//                              Message =
//                                string.Format(
//                                  @"Generation of the receivable rmbmcm report is in progress. You will be notified via 
//                        email once it is ready for download. [File: {0}.zip]",
//                                  fileName),
//                              IsFailed = false
//                            }
//                        : new UIMessageDetail
//                            {
//                              Message = "Failed to download the receivable rmbmcm report, please try again!",
//                              IsFailed = true
//                            };
//      }
//      catch (Exception)
//      {
//        return Json(new UIMessageDetail
//                      {
//                        Message = "Failed to download the receivable rmbmcm report, please try again!",
//                        IsFailed = true
//                      });
//      }


//      return Json(result);




//      // // Get Temp folder path.
//      // var tempFolderPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.SFRTempRootPath), "PassengerRmBmCmSummaryPayablesReceivablesCsvReport");

//      //_logger.InfoFormat("RMBMCM Receivable Report: Get temporary folder path '{0}'", tempFolderPath);

//      // // Create specified directory if it is not alreadey exist.
//      // if (!Directory.Exists(tempFolderPath))
//      //   Directory.CreateDirectory(tempFolderPath);

//      // // Delete Oldest files from temp folder
//      // DeleteOldestFilesFromTempFolder(tempFolderPath);

//      // _logger.InfoFormat("RMBMCM Receivable Report: Delete oldest file from temp folder.");

//      // // Create file name including current timestamp
//      // var reportFileName = tempFolderPath + @"\PAX_Receivable_RMBMCM_Summary_Report_" + Guid.NewGuid();

//      // // Delete Oldest zip files from temp folder.
//      // if (System.IO.File.Exists(reportFileName + ".zip"))
//      //   System.IO.File.Delete(reportFileName + ".zip");

//      // // Call to function CreateCsv
//      // var error = CreateCsv(reportFileName, receivablesReportModel.FromBillingMonth, receivablesReportModel.FromBillingYear, billingPeriod,
//      //                       settlementMethod, memoType, submissionMethod, invoiceNo, memberCode, rmBmCmNo, sourceCode,  searchCriteria, broweserDateTime);

//      // if (!string.IsNullOrEmpty(error))
//      // {
//      //  _logger.InfoFormat("RMBMCM Receivable Report: Show error message while generate RMBMCM Receivable Report.");
//      //   ShowSuccessMessage(error);

//      //   ViewData["CategoryName"] = BillingCategoryType.Pax;
//      //   ViewData["currentYear"] = receivablesReportModel.FromBillingYear;
//      //   ViewData["currentMonth"] = receivablesReportModel.FromBillingMonth;
//      //   ViewData["UserCategory"] = SessionUtil.MemberId;
//      //   ViewData["Period"] = receivablesReportModel.FromPeriod;
//      //   ViewData[ViewDataConstants.BillingType] = BillingType.Receivables;

//      //   // CMP-523: Source Code in Passenger RM BM CM Summary Reports
//      //   // For pre-population of the source codes for Memo Type = "All".
//      //   var sourceCodes = ReferenceManager.GetSourceCodesList(memoType);
//      //   var sourceCodeNonBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == false).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();
//      //   var sourceCodeBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == true).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();

//      //   var list = sourceCodeNonBilateral.Union(sourceCodeBilateral);

//      //   ViewData["SourceCodeList"] = new MultiSelectList(list, "SourceCodeIdentifier", "SourceCodeIdentifier");

//      //   return View("ReceivablesReport");

//      //SCP223595: RM BM CM SUMMARY
//      // Make file available to download.
//      // var fileName = Path.GetFileName(reportFileName);
//      //_logger.InfoFormat("RMBMCM Receivable Report: Execution of generate rmbmcm receivable report is finished.");
//      //var contentType = "application/zip";
//      //if (fileName != null)
//      //{
//      //  var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileName);
//      //  if (reg != null)
//      //  {
//      //    contentType = reg.GetValue("Content Type") as string;
//      //  }
//      //}

//      //  reportFileName = Path.GetFullPath(tempFolderPath + @"\" + Path.GetFileName(reportFileName + ".zip"));

//      //  if (!System.IO.File.Exists(reportFileName))
//      //  {
//      //    return RedirectToAction("ReceivablesReport", "ReceivablesReport");
//      //  }
//      //  return File(reportFileName, "application/zip", Path.GetFileName(reportFileName));
//      //}
//      //catch (Exception exception)
//      //{
//      //  _logger.Error("Unexpected Error Has Occurred", exception);
//      //  return RedirectToAction("Error", "Home");
//      //}*/

//      #endregion;
//    }

// End of ExportToCsv()

    /// <summary>
    /// This method is used to Create CSV File of Passenger Receivables RMBMCM Summary Report
    /// </summary>
    /// <param name="reportFileName">Report File Name with location</param>
    /// <param name="billingMonth">Billing Month</param>
    /// <param name="billingYear">Billing Year</param>
    /// <param name="billingPeriod">Billing Period</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="submissionMethod">Submission Method</param>
    /// <param name="invoiceNo">Invoice Number</param>
    /// <param name="billedEntityCode">Member Code</param>
    /// <param name="rmBmCmNo">RM/BM/CM Number</param>
    /// <param name="sourceCode">Source Code</param>
    /// <param name="searchCriteria"> Searach Criteria </param>
    /// <param name="broweserDateTime"> Broweser Date Time </param>
    /// <returns> string </returns>
    private string CreateCsv(string reportFileName, int billingMonth, int billingYear, int billingPeriod, int settlementMethod, int memoType,
                           int submissionMethod, string invoiceNo, int billedEntityCode, string rmBmCmNo, string sourceCode, string searchCriteria, string broweserDateTime)
    {
      // Get data for the specified search criteria from database.
      var csvListModel = new List<PaxReceivablesRmBmCmSummaryReport>();

       _logger.InfoFormat("RMBMCM Receivable Report: Create csv model and fetch data from database based on search criteria.");

      csvListModel.AddRange(GetCsvListModel(billingMonth, billingYear, billingPeriod, settlementMethod, memoType,
                                            submissionMethod, invoiceNo, billedEntityCode, rmBmCmNo, sourceCode));
      // If given filter criteria fetches data then generate CSV report for the data else
      // return message to displayed to user notifying him no data was found.
      if (csvListModel.Count() != 0)
      {
        #region Add Special Records
        // Search Criteria and Browser Date Time to report footer.

        // Remove "," from Search Criteria string to show in one cell of csv
          //if (!string.IsNullOrEmpty(searchCriteria))
          //{
          //    searchCriteria = searchCriteria.Replace(",", " ");
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
          _logger.InfoFormat("RMBMCM Receivable Report: Starting to generate csv report.");
          // Generate CSV Report for given data.
          CsvProcessor.GenerateCsvReport(csvListModel, reportFileName + ".csv", couponSpecialRecords, includesearchCriteria : true);
      }
      else
      {
        return "The query did not produce any results. Please modify your query criteria.";
      }
      _logger.InfoFormat("RMBMCM Receivable Report: Execution of generate csv report is finished.");

      //Zip CSV file 
      if (System.IO.File.Exists(reportFileName + ".csv"))
      {
        //Delete file if its length is zero
        var f = new FileInfo(reportFileName + ".csv");
        if (f.Length == 0)
          System.IO.File.Delete(reportFileName + ".csv");
        else
        {
          // make zip file available to download.
          FileIo.ZipOutputFile(reportFileName + ".csv");
        }
      }
      return string.Empty;
    }  // End of CreateCsv

    /// <summary>
    /// This method is used to get data from database and map it according to Report display model for Passenger Receivables RMBMCM Summary Report.
    /// </summary>
    /// <param name="billingMonth">Billing Month</param>
    /// <param name="billingYear">Billing Year</param>
    /// <param name="billingPeriod">Billing Period</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="submissionMethod">Submission Method</param>
    /// <param name="invoiceNo">Invoice Number</param>
    /// <param name="billedEntityCode">Memeber Code</param>
    /// <param name="rmBmCmNo">RM/BM/CM Number</param>
    /// <param name="sourceCode"> Source Code </param>
    /// <returns>List of PaxReceivablesRmBmCmSummaryReportModel</returns>
    private List<PaxReceivablesRmBmCmSummaryReport> GetCsvListModel(int billingMonth, int billingYear, int billingPeriod, int settlementMethod,
                                                         int memoType, int submissionMethod, string invoiceNo, int billedEntityCode, string rmBmCmNo, string sourceCode)
    {
      // get data from database as per report search criteria.
      var filteredList = _receivablesReport.GetReceivablesReportDetails(SessionUtil.MemberId, billingMonth, billingYear, billingPeriod, settlementMethod,
                                                                        memoType, submissionMethod, invoiceNo, billedEntityCode, rmBmCmNo, sourceCode);

      _logger.InfoFormat("RMBMCM Receivable Report: Execution of fetching data from database is finished.");
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
      
      _logger.InfoFormat("RMBMCM Receivable Report: Execution of creation csv model is finished.");
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

#endregion;

    /// <summary>
    /// Author: Sanket Shrivastava
    /// Purpose: Pax: Sampling Billing Analysis - Receivables
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesSamplingBillingAnalysisAccess)]
    public ActionResult OwSamplingReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Pax;
      //ViewData["currentYear"] = currentPeriod.Year;
      //ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;
      ViewData["BillingType"] = BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)BillingType.Receivables;
      ViewData["BillingTypeText"] = "Receivables";
      ViewData["MemberType"] = "Billed";
      return View();
    }
    public ActionResult CgoReceivablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["UserCategory"] = (int)SessionUtil.MemberId;
      ViewData["Period"] = currentPeriod.Period;
      ViewData["BillingType"] = BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)BillingType.Receivables;
      return View();
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Purpose: For Receivables - Passanger Interline Billing Summary Report
    /// </summary>
    /// <returns></returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesInterlineBillSummaryAccess)]
    public ActionResult PaxInterlineBillingSummaryReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;

      ViewData["BillingType"] = BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)BillingType.Receivables;
      ViewData["BillingTypeText"] = "Receivables";
      ViewData["MemberType"] = "Billed";
      return View();
    }

    /// <summary>
    /// Author: Sachin Pharande
    /// Date of Creation: 13-10-2011
    /// Purpose: For Receivables - Passanger Rejection Analysis - Non Sampling Report
    /// </summary>
    /// <returns></returns>

    [ISAuthorize(Business.Security.Permissions.Reports.Pax.ReceivablesNonSampleRejAnalysisAccess)]
    public ActionResult PaxRejectionAnalysisNonSamplingReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      ViewData["BillingType"] = BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)BillingType.Receivables;
      ViewData["BillingTypeText"] = "Receivables";
      ViewData["MemberType"] = "Billed";
      return View();
    }
  }
}