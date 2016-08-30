using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.Cargo.RMBMCMSummary;
using Iata.IS.Core;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.Cargo;
using Iata.IS.Model.Reports.ReportSearchCriteria;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;
using BillingType = Iata.IS.Model.Enums.BillingType;
using Iata.IS.Core.File;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
  public class RMBMCMSummaryReportController : ISController
  {
    //
    // GET: /Reports/RMBMCMSummaryReport/
    private readonly ICalendarManager _calenderManager;
    private readonly IRMBMCMSummaryReport _rmBmCmSummaryReport;
    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public RMBMCMSummaryReportController(ICalendarManager calenderManager, IRMBMCMSummaryReport rmBmCmSummaryReport)
    {
      _calenderManager = calenderManager;
      _rmBmCmSummaryReport = rmBmCmSummaryReport;
    }
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.ReceivablesRMBMCMSummaryAccess)]
    public ActionResult CargoReceivablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      ViewData["MemberId"] = SessionUtil.MemberId;
      ViewData["BillingType"] = BillingType.Receivables;
      ViewData["BillingTypeId"] = (int)BillingType.Receivables;
      ViewData["BillingTypeWords"] = "Receivables";
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PeriodNo"] = currentPeriod.Period;
      ViewData["MemberType"] = "Billed";

      return View("CargoRMBMCMSummaryReport");
    }

    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.PayablesRMBMCMSummaryAccess)]
    public ActionResult CargoPayablesReport()
    {
      IsMemberNullOrEmpty(SessionUtil.MemberId);
      var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
      ViewData["CategoryName"] = BillingCategoryType.Cgo;
      ViewData["BillingType"] = BillingType.Payables;
      ViewData["BillingTypeId"] = (int)BillingType.Payables;
      ViewData["MemberId"] = SessionUtil.MemberId;
      ViewData["BillingTypeWords"] = "Payables";
      ViewData["currentYear"] = currentPeriod.Year;
      ViewData["currentMonth"] = currentPeriod.Month;
      ViewData["PeriodNo"] = currentPeriod.Period;
      ViewData["MemberType"] = "Billing";

      return View("CargoRMBMCMSummaryReport");
    }
    /// <summary>
    /// Post method to get the report
    /// </summary>
    /// <param name="reportSearchCriteriaModel"> ReportSearchCriteriaModel </param>
    /// <param name="searchCriteria"> Search Criteria </param>
    /// <param name="broweserDateTime"> Broweser Date Time </param>
    /// <returns> File or View </returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.ReceivablesRMBMCMSummaryAccess)]
    [HttpPost]
    public ActionResult CargoReceivablesReport(ReportSearchCriteriaModel reportSearchCriteriaModel, string searchCriteria, string broweserDateTime)
    {
      // Call to function CargoRmBmCmSummaryPayablesReceivablesCsvReport to get the CSV file.
      return CargoRmBmCmSummaryPayablesReceivablesCsvReport(reportSearchCriteriaModel, searchCriteria, broweserDateTime);
    }

    /// <summary>
    /// Post method to get the report
    /// </summary>
    /// <param name="reportSearchCriteriaModel">Report Search Criteria Model</param>
    /// <param name="searchCriteria"> Search Criteria </param>
    /// <param name="broweserDateTime"> Browser Date Time</param>
    /// <returns> File or View </returns>
    [ISAuthorize(Business.Security.Permissions.Reports.Cargo.PayablesRMBMCMSummaryAccess)]
    [HttpPost]
    public ActionResult CargoPayablesReport(ReportSearchCriteriaModel reportSearchCriteriaModel, string searchCriteria, string broweserDateTime)
    {
      // Call to function CargoRmBmCmSummaryPayablesReceivablesCsvReport to get the CSV file.
      return CargoRmBmCmSummaryPayablesReceivablesCsvReport(reportSearchCriteriaModel, searchCriteria, broweserDateTime);
    }

    /// <summary>
    /// This method is used to get the result in CSV file format.
    /// </summary>
    /// <param name="reportSearchCriteriaModel"> ReportSearchCriteriaModel </param>
    /// <param name="searchCriteria"> Search Criteria</param>
    /// <param name="broweserDateTime"> Broweser Date Time </param>
    /// <returns> File or Action Method. </returns>
    private ActionResult CargoRmBmCmSummaryPayablesReceivablesCsvReport(ReportSearchCriteriaModel reportSearchCriteriaModel,
                                                                        string searchCriteria, string broweserDateTime)
    {
      try
      {
        // Parameter setup
        int periodNo = reportSearchCriteriaModel.Period == -1 ? 0 : reportSearchCriteriaModel.Period;
        int memoType = Convert.ToInt32(reportSearchCriteriaModel.MemoType);
        
        //SCP ID: 123422 -'Payable RM BM CM summary report has unnecessary parameter "Submission" type'   
        //Added Submission Method 0 for Payable report.
        int submissionMethod = reportSearchCriteriaModel.BillingType == 1 ? 0:Convert.ToInt32(reportSearchCriteriaModel.SubmissionMethodId);
        int billedMemberId = reportSearchCriteriaModel.AirlineId == "" ? 0 : Convert.ToInt32(reportSearchCriteriaModel.AirlineId);
        string invoiceNumber = (String.IsNullOrEmpty(reportSearchCriteriaModel.InvoiceNo)) ? "" : reportSearchCriteriaModel.InvoiceNo;
        string rmbmcmNumber = (String.IsNullOrEmpty(reportSearchCriteriaModel.RmbmcmNumber)) ? "" : reportSearchCriteriaModel.RmbmcmNumber;

        // Get Temp folder path.
        var tempFolderPath = Path.Combine(FileIo.GetForlderPath(SFRFolderPath.SFRTempRootPath), "CargoRmBmCmSummaryPayablesReceivablesCsvReport");

        // Create specified directory if it is not alreadey exist.
        if (!Directory.Exists(tempFolderPath))
          Directory.CreateDirectory(tempFolderPath);

        // Delete Oldest files from temp folder
        DeleteOldestFilesFromTempFolder(tempFolderPath);

        // Create unique file name with the help of Guid.
        var reportFileName = reportSearchCriteriaModel.BillingType == 1
                               ? tempFolderPath + @"\Cargo_Payables_RMBMCM_Summary_Report_" + Guid.NewGuid()
                               : tempFolderPath + @"\Cargo_Receivables_RMBMCM_Summary_Report_" + Guid.NewGuid();

        // Delete Oldest zip files from temp folder.
        if (System.IO.File.Exists(reportFileName + ".zip"))
          System.IO.File.Delete(reportFileName + ".zip");

        // Call to function CreateCsv
        var error = CreateCsv(reportFileName, reportSearchCriteriaModel.Year, reportSearchCriteriaModel.Month, periodNo,
                              reportSearchCriteriaModel.SettlementMethodIndicatorId, memoType, submissionMethod, billedMemberId,
                              invoiceNumber, rmbmcmNumber, reportSearchCriteriaModel.BillingType, searchCriteria, broweserDateTime);

        if (!string.IsNullOrEmpty(error))
        {
          ShowSuccessMessage(error);

          if (reportSearchCriteriaModel.BillingType == 1)
          {
            ViewData["CategoryName"] = BillingCategoryType.Cgo;
            ViewData["BillingType"] = BillingType.Payables;
            ViewData["BillingTypeId"] = (int)BillingType.Payables;
            ViewData["MemberId"] = SessionUtil.MemberId;
            ViewData["BillingTypeWords"] = "Payables";
            ViewData["currentYear"] = reportSearchCriteriaModel.Year;
            ViewData["currentMonth"] = reportSearchCriteriaModel.Month;
            ViewData["PeriodNo"] = reportSearchCriteriaModel.Period;
            ViewData["MemberType"] = "Billing";
          }
          else
          {
            ViewData["CategoryName"] = BillingCategoryType.Cgo;
            ViewData["MemberId"] = SessionUtil.MemberId;
            ViewData["BillingType"] = BillingType.Receivables;
            ViewData["BillingTypeId"] = (int)BillingType.Receivables;
            ViewData["BillingTypeWords"] = "Receivables";
            ViewData["currentYear"] = reportSearchCriteriaModel.Year;
            ViewData["currentMonth"] = reportSearchCriteriaModel.Month;
            ViewData["PeriodNo"] = reportSearchCriteriaModel.Period;
            ViewData["MemberType"] = "Billed";

          }
          return View("CargoRMBMCMSummaryReport");
        }
        // Make file available to download.
        var fileName = Path.GetFileName(reportFileName);

        var contentType = "application/zip";
        if (fileName != null)
        {
          var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileName);
          if (reg != null)
          {
            contentType = reg.GetValue("Content Type") as string;
          }
        }

        reportFileName = Path.GetFullPath(tempFolderPath + @"\" + Path.GetFileName(reportFileName + ".zip"));

        if (!System.IO.File.Exists(reportFileName))
        {
          return View("CargoRMBMCMSummaryReport", reportSearchCriteriaModel);
        }
        return File(reportFileName, contentType, Path.GetFileName(reportFileName));
      }
      catch (Exception exception)
      {
        _logger.Error("Unexpected Error Has Occurred", exception);
        return RedirectToAction("Error", "Home");
      }
    } // End of ExportToCsv()

    /// <summary>
    /// This method is used to Create CSV File of Cargo Receivables/Payables RMBMCM Summary Report
    /// </summary>
    /// <param name="reportFileName">reportFileName</param>
    /// <param name="billingMonth">Billing Month</param>
    /// <param name="billingYear">Billing Year</param>
    /// <param name="periodNo">Period Number</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="submissionMethod"> Submission Method</param>
    /// <param name="billedMemberId"> Billed MemberCode</param>
    /// <param name="invoiceNumber">Invoice Number</param>
    /// <param name="rmbmcmNumber">RM/MB/CM Number</param>
    /// <param name="billingType">Billing Type</param>
    /// <param name="searchCriteria"> Search Criteria </param>
    /// <param name="broweserDateTime"> Broweser Date Time </param>
    /// <returns> string </returns>

    
    private string CreateCsv(string reportFileName, int billingYear, int billingMonth, int periodNo, int settlementMethod, int memoType, int submissionMethod,
                             int billedMemberId, string invoiceNumber, string rmbmcmNumber, int billingType, string searchCriteria, string broweserDateTime)
    {
      //if billing type is Payables
      if (billingType == 1)
      {
        // Get data from database for given filter criterias.
        var csvListModel = new List<CargoRmBmCmSummaryPayablesReport>();
        csvListModel.AddRange(GetCsvListModelPayables(billingYear, billingMonth, periodNo, settlementMethod, memoType,
                                                      submissionMethod,
                                                      SessionUtil.MemberId, billedMemberId, invoiceNumber, rmbmcmNumber,
                                                      billingType));
        // To display 'Y' or 'N' insted of '0' or '1' for AttachmentIndicatorOrig field on report.
        foreach (var cargoRmBmCmSummaryReportBase in csvListModel)
        {
          if (cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig != null)
          {
            if (cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig.Equals("0"))
            {
              cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = "N";
            }
            else if (cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig.Equals("1"))
            {
              cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = "Y";
            }
            else
            {
              cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = " ";
            }
          }
          else
          {
            cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = " ";
          }
        }

        // If given filter criteria fetches data then generate CSV report for the data else
        // return message to displayed to user notifying him no data was found.
        if (csvListModel.Count() != 0)
        {

          #region Add Special Records

          // Search Criteria and Browser Date Time to report footer.
          if (!string.IsNullOrEmpty(searchCriteria))
          {
            searchCriteria = searchCriteria.Replace(",", " ");
          }

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
          CsvProcessor.GenerateCsvReport(csvListModel, reportFileName + ".csv", couponSpecialRecords);
        }
        else
        {
          return "The query did not produce any results. Please modify your query criteria.";
        }
      }
      //if billing type is Receivables
      else
      {
        var csvListModel = new List<CargoRmBmCmSummaryReceivablesReport>();
        // Get data from database for given filter criterias.
        csvListModel.AddRange(GetCsvListModelReceivables(billingYear, billingMonth, periodNo, settlementMethod, memoType,
                                                         submissionMethod,
                                                         SessionUtil.MemberId, billedMemberId, invoiceNumber,
                                                         rmbmcmNumber, billingType));
        // To display 'Y' or 'N' insted of '0' or '1' for AttachmentIndicatorOrig field on report.
        foreach (var cargoRmBmCmSummaryReportBase in csvListModel)
        {
          if (cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig != null)
          {
            if (cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig.Equals("0"))
            {
              cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = "N";
            }
            else if (cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig.Equals("1"))
            {
              cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = "Y";
            }
            else
            {
              cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = " ";
            }
          }
          else
          {
            cargoRmBmCmSummaryReportBase.AttachmentIndicatorOrig = " ";
          }
        }
        // If given filter criteria fetches data then generate CSV report for the data else
        // return message to displayed to user notifying him no data was found.
        if (csvListModel.Count() != 0)
        {
          #region Add Special Records
          // Search Criteria and Browser Date Time to report footer.

          // Remove "," from Search Criteria string to show in one cell of csv
          if (!string.IsNullOrEmpty(searchCriteria))
          {
            searchCriteria = searchCriteria.Replace(",", " ");
          }

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
          CsvProcessor.GenerateCsvReport(csvListModel, reportFileName + ".csv", couponSpecialRecords);
        }
        else
        {
          return "The query did not produce any results. Please modify your query criteria.";
        }
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
    /// This method is used to get date from database and map it according to Report display model for Cargo Receivables RMBMCM Summary Report.
    /// </summary>
    /// <param name="billingYear">Billing Year</param>
    /// <param name="billingMonth">Billing Month</param>
    /// <param name="billingPeriod">Billing Period</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="submissionMethod">Submission Method</param>
    /// <param name="billingEntityId">Billing Enetity code</param>
    /// <param name="billedEntityCode">Billed Entity code</param>
    /// <param name="invoiceNo">Invoice Number</param>
    /// <param name="rmbmcmNo">RM/BM/CM Number</param>
    /// <param name="billingType">Billing Type</param>
    /// <returns>List of CargoRmBmCmSummaryReceivablesReport Model</returns>
    private List<CargoRmBmCmSummaryReceivablesReport> GetCsvListModelReceivables(int billingYear, int billingMonth, int billingPeriod,
                                                                                 int settlementMethod, int memoType, int submissionMethod,
                                                                                 int billingEntityId, int billedEntityCode, string invoiceNo,
                                                                                 string rmbmcmNo, int billingType)
    {
      // Get data from database as per report search criteria.
      var filteredList = _rmBmCmSummaryReport.GetRMBMCMSummaryReport(billingYear, billingMonth, billingPeriod, settlementMethod,
                                                                     memoType, submissionMethod, billingEntityId, billedEntityCode,
                                                                     invoiceNo, rmbmcmNo, billingType);

      var csvListModel = new List<CargoRmBmCmSummaryReceivablesReport>();

      if (filteredList != null)
      {
        csvListModel.AddRange(
          filteredList.Select(
            receivablesReportModel => new CargoRmBmCmSummaryReceivablesReport
            {
              BillingMonth = receivablesReportModel.BillingMonth,
              PeriodNo = receivablesReportModel.PeriodNo,
              SettlementMethod = receivablesReportModel.SettlementMethod,
              AirlineCode = receivablesReportModel.AirlineCode,
              InvoiceNumber = receivablesReportModel.InvoiceNumber,
              MemoType = receivablesReportModel.MemoType,
              MemoNumber = receivablesReportModel.MemoNumber,
              Stage = receivablesReportModel.Stage,
              ReasonCode = receivablesReportModel.ReasonCode,
              CurrencyCode = receivablesReportModel.CurrencyCode,
              WeightCharges = receivablesReportModel.WeightCharges,
              ValuationCharges = receivablesReportModel.ValuationCharges,
              OtherChargeAmount = receivablesReportModel.OtherChargeAmount,
              IscAmount = receivablesReportModel.IscAmount,
              VatAmount = receivablesReportModel.VatAmount,
              NetAmount = receivablesReportModel.NetAmount,
              NumberofLinkedAwb = receivablesReportModel.NumberofLinkedAwb,
              AttachmentIndicatorOrig = receivablesReportModel.AttachmentIndicator.ToString()
            }));
      }
      return csvListModel;
    } // End GetCsvListModelReceivables()

    /// <summary>
    /// This method is used to get date from database and map it according to Report display model for Cargo Payables RMBMCM Summary Report.
    /// </summary>
    /// <param name="billingYear">Billing Year</param>
    /// <param name="billingMonth">Billing Month</param>
    /// <param name="billingPeriod">Billing Period</param>
    /// <param name="settlementMethod">Settlement Method</param>
    /// <param name="memoType">Memo Type</param>
    /// <param name="submissionMethod">Submission Method</param>
    /// <param name="billingEntityId">Billing Enetity code</param>
    /// <param name="billedEntityCode">Billed Entity code</param>
    /// <param name="invoiceNo">Invoice Number</param>
    /// <param name="rmbmcmNo">RM/BM/CM Number</param>
    /// <param name="billingType">Billing Type</param>
    /// <returns>List of CargoRmBmCmSummaryPayablesReport Model</returns>
    private List<CargoRmBmCmSummaryPayablesReport> GetCsvListModelPayables(int billingYear, int billingMonth, int billingPeriod,
                                                                           int settlementMethod, int memoType, int submissionMethod,
                                                                           int billingEntityId, int billedEntityCode, string invoiceNo,
                                                                           string rmbmcmNo, int billingType)
    {
      // Get data from database as per report search criteria.
      var filteredList = _rmBmCmSummaryReport.GetRMBMCMSummaryReport(billingYear, billingMonth, billingPeriod, settlementMethod,
                                                                     memoType, submissionMethod, billingEntityId, billedEntityCode,
                                                                     invoiceNo, rmbmcmNo, billingType);

      var csvListModel = new List<CargoRmBmCmSummaryPayablesReport>();

      if (filteredList != null)
      {
        csvListModel.AddRange(
          filteredList.Select(
            payablesReportModel => new CargoRmBmCmSummaryPayablesReport
            {
              BillingMonth = payablesReportModel.BillingMonth,
              PeriodNo = payablesReportModel.PeriodNo,
              SettlementMethod = payablesReportModel.SettlementMethod,
              AirlineCode = payablesReportModel.AirlineCode,
              InvoiceNumber = payablesReportModel.InvoiceNumber,
              MemoType = payablesReportModel.MemoType,
              MemoNumber = payablesReportModel.MemoNumber,
              Stage = payablesReportModel.Stage,
              ReasonCode = payablesReportModel.ReasonCode,
              CurrencyCode = payablesReportModel.CurrencyCode,
              WeightCharges = payablesReportModel.WeightCharges,
              ValuationCharges = payablesReportModel.ValuationCharges,
              OtherChargeAmount = payablesReportModel.OtherChargeAmount,
              IscAmount = payablesReportModel.IscAmount,
              VatAmount = payablesReportModel.VatAmount,
              NetAmount = payablesReportModel.NetAmount,
              NumberofLinkedAwb = payablesReportModel.NumberofLinkedAwb,
              AttachmentIndicatorOrig = payablesReportModel.AttachmentIndicator.ToString()
            }));
      }
      return csvListModel;
    } // End GetCsvListModelPayables()

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
  }
}