using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports.OfflineReportManger;
using Iata.IS.Business.Reports.ProcessingDashBoard;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Common;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Model.Reports.ProcessingDashBoard;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.Enums;
using log4net;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
    public class SisUsageReportController : ISController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICalendarManager _calenderManager;
        private readonly IReferenceManager _referenceManager;
        private readonly IOfflineReportLogManager _offlineReportLogManager;

        public SisUsageReportController(ICalendarManager calenderManager, IReferenceManager referenceManager, IOfflineReportLogManager offlineReportLogManager)
        {
            _calenderManager = calenderManager;
            //CMP #659: SIS IS-WEB Usage Report.
            _referenceManager = referenceManager;
            _offlineReportLogManager = offlineReportLogManager;
        }

        /// <summary>
        /// Following action is used to display SISUsage report.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Reports.SISUsageReport.Access)]
        public ActionResult SISUsageReport()
        {
            var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);//GetCurrentBillingPeriod();
            ViewData["currentYear"] = currentPeriod.Year;
            ViewData["currentMonth"] = currentPeriod.Month;
            ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
            ViewData["Period"] = currentPeriod.Period;
            return View();
        }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date: 21-03-2012
        /// Purpose: Following POST action is used to display SISUsage report.
        /// </summary>
        /// <param name="sisUsageReportModel"></param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Reports.SISUsageReport.Access)]
        [HttpPost]
        public ActionResult SISUsageReport(SisUsageReportModel sisUsageReportModel)
        {
            try
            {
                //reading view model
                Logger.Info("SISUsageReport Controller Execution Started");
                DateTime fromdate = new DateTime(sisUsageReportModel.FromBillingYear,
                                                 sisUsageReportModel.FromBillingMonth, sisUsageReportModel.FromPeriod);
                DateTime todate = new DateTime(sisUsageReportModel.ToBillingYear, sisUsageReportModel.ToBillingMonth,
                                               sisUsageReportModel.ToPeriod);

                int membeId = SessionUtil.UserCategory == UserCategory.SisOps
                                  ? sisUsageReportModel.MemberId == 0 ? 0 : sisUsageReportModel.MemberId
                                  : SessionUtil.MemberId;

                /* SCP 263438: Usage report producing incomplete results for YH-361
                   Description: Usage report fetch logic is changed to consider member id instead of member code and designator.
                */

                Logger.Info("Download SISUageReport for memberID " + membeId);
                int participantType = sisUsageReportModel.ParticipantType == string.Empty
                                          ? 0
                                          : Convert.ToInt32(sisUsageReportModel.ParticipantType);

                List<RechargeData> rechargeData = new List<RechargeData>();

                //call to business method to get the values from database.
                Logger.Info("Starting to execute GetSISUsageReport");

                rechargeData = Ioc.Resolve<ISISUsageRecharge>(typeof(ISISUsageRecharge)).GetSISUsageReport(fromdate,
                                                                                                            todate,
                                                                                                            membeId,
                                                                                                            participantType);
                Logger.Info("Execution of GetSISUsageReport is finished");
                foreach (var t in rechargeData)
                {
                    t.MemberCode = t.AlphaCode + "-" + t.NumericCode;
                }

                DataTable dt = ListToDataTable(rechargeData);

                ReportDocument orpt = new ReportDocument();

                string reportPath = Server.MapPath(SessionUtil.UserCategory == UserCategory.SisOps
                                                       ? "~/Reports/ProcessingDashBoard/SISUsageReportOPS.rpt"
                                                       : "~/Reports/ProcessingDashBoard/SISUsageReport.rpt");

                Logger.Info("Started Execution of ReportDocument Load");
                //loading path
                orpt.Load(reportPath);

                Logger.Info("ReportDocument Load finished");
                //define datasource
                orpt.SetDataSource(dt);
                Logger.Info("ReportDocument SetDataSource finished");
                //temparary file location to store the file.
                var tempFolderPath = FileIo.GetForlderPath(SFRFolderPath.ISUsageReport);

                // Create Temp Folder in SFR to copy network files 
                if (!Directory.Exists(tempFolderPath))
                {
                    Directory.CreateDirectory(tempFolderPath);
                    Logger.Info("CreateDirectory for " + tempFolderPath);
                }

                //Delete Oldest files from Temp Folder
                DeleteOldestFilesFromTempFolder(tempFolderPath);

                Logger.Info("DeleteOldestFilesFromTempFolder finished");
                //storing file to the disk
                string fileName;
                string fileNameTimeStamp = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm_ss");

                if (sisUsageReportModel.DownloadFileFormats == 1 && SessionUtil.UserCategory == UserCategory.SisOps)
                {
                    fileName = Path.Combine(tempFolderPath, "SISUsageReportSisOps" + fileNameTimeStamp + ".xls");
                    orpt.ExportToDisk(ExportFormatType.ExcelRecord, fileName);
                }
                else if (sisUsageReportModel.DownloadFileFormats == 1 && SessionUtil.UserCategory != UserCategory.SisOps)
                {
                    fileName = Path.Combine(tempFolderPath, "SISUsageReport" + fileNameTimeStamp + ".xls");
                    orpt.ExportToDisk(ExportFormatType.ExcelRecord, fileName);

                }
                else if (sisUsageReportModel.DownloadFileFormats != 1 &&
                         SessionUtil.UserCategory == UserCategory.SisOps)
                {
                    fileName = Path.Combine(tempFolderPath, "SISUsageReportSisOps" + fileNameTimeStamp + ".pdf");
                    orpt.ExportToDisk(ExportFormatType.PortableDocFormat, fileName);

                }
                else
                {
                    fileName = Path.Combine(tempFolderPath, "SISUsageReport" + fileNameTimeStamp + ".pdf");
                    orpt.ExportToDisk(ExportFormatType.PortableDocFormat, fileName);

                }
                Logger.Info("ExportToDisk is finished for file " + fileName);
                //read the file from temparary location and make available to download
                string contentType;
                var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(fileName);
                if (reg != null)
                {
                    contentType = reg.GetValue("Content Type") as string;
                }
                else
                {
                    contentType = "application/txt";
                }
                Logger.Info("SISUsageReport executed successfully");
                return File(fileName, contentType, Path.GetFileName(fileName));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date: 21-03-2012
        /// Purpose: function to delete old files from temparay loaction.
        /// </summary>
        /// <param name="path"></param>
        private static void DeleteOldestFilesFromTempFolder(string path)
        {
            var tempDownloadDir = new DirectoryInfo(path);
            foreach (FileInfo file in tempDownloadDir.GetFiles())
            {
                if (file.CreationTimeUtc <= DateTime.UtcNow.AddDays(-1))
                {
                    file.Delete();
                }
            }

        }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date: 21-03-2012
        /// Purpose: Function to load list to DataTable.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable(List<RechargeData> list)
        {
            DataTable dt = new DataTable();
            foreach (PropertyInfo info in typeof(RechargeData).GetProperties())
            {
                dt.Columns.Add(new DataColumn(info.Name, info.PropertyType));
            }
            foreach (RechargeData t in list)
            {
                DataRow row = dt.NewRow();
                foreach (PropertyInfo info in typeof(RechargeData).GetProperties())
                {
                    row[info.Name] = info.GetValue(t, null);
                }

                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// Following action is used to display SIS is-web Usage report.
        /// </summary>
        /// <returns></returns>
        /// CMP #659: SIS IS-WEB Usage Report.
        [ISAuthorize(Business.Security.Permissions.Reports.SISUsageReport.Access)]
        public ActionResult SisIsWebUsageReport()
        {
            //Values for these fields should be set as per the previously closed period
            var lastClosedBillingPeriod = _calenderManager.GetLastClosedBillingPeriod();

            var sisIsWebUsageReportModel = new SisUsageReportModel
                                                               {
                                                                   FromBillingYear = lastClosedBillingPeriod.Year,
                                                                   ToBillingYear = lastClosedBillingPeriod.Year,
                                                                   FromBillingMonth = lastClosedBillingPeriod.Month,
                                                                   ToBillingMonth = lastClosedBillingPeriod.Month,
                                                                   FromPeriod = lastClosedBillingPeriod.Period,
                                                                   ToPeriod = lastClosedBillingPeriod.Period,
                                                               };
            return View(sisIsWebUsageReportModel);
        }

        /// <summary>
        /// Following action is used to display SISUsage report.
        /// </summary>
        /// <returns></returns>
        /// CMP #659: SIS IS-WEB Usage Report.
        [ISAuthorize(Business.Security.Permissions.Reports.SISUsageReport.Access)]
        [HttpPost]
        public ActionResult SisIsWebUsageReport(SisUsageReportModel sisIsWebUsageReportModel)
        {
            try
            {
               
                //Prepare serach criteria for sis is-web usage report to display offlineReport Grid.
                string searchCriteria = "Search Criteria : From Billing Year: " +
                                        sisIsWebUsageReportModel.FromBillingYear + "; From Billing Month: " +
                                        CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(
                                            sisIsWebUsageReportModel.FromBillingMonth)
                                        + "; From Period: " + sisIsWebUsageReportModel.FromPeriod + "; To Billing Year: " +
                                        sisIsWebUsageReportModel.ToBillingYear + "; To Billing Month: " +
                                        CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(
                                            sisIsWebUsageReportModel.ToBillingMonth) + "; To Period: " +
                                        sisIsWebUsageReportModel.ToPeriod;

                Logger.InfoFormat("SIS IS-WEB Usage Report: Search Criteria for Sis Is-Web Usage Report: {0}.",
                                  searchCriteria);

                //Create Offline report log object for inserting data into OfflineReportlog table.
                var offlineReportLog = new OfflineReportLog
                {
                    Id = Guid.NewGuid(),
                    MemberId = SessionUtil.MemberId,
                    UserId = SessionUtil.UserId,
                    RequestDateTime = DateTime.UtcNow,
                    OfflineReportId = (Int32)OfflineReportType.SisIsWebUsageReports,
                    SearchCriterion = searchCriteria,
                    DownloadLinkId = null,
                    LastUpdatedBy = SessionUtil.UserId
                };

                //Add Search Criteria in the Model 
                sisIsWebUsageReportModel.SearchCriteria = searchCriteria;
                sisIsWebUsageReportModel.OfflineReportLogId = offlineReportLog.Id;

                //Create a enque message object for SIS IS-WEB Usage Report.
                var enqueMessage = new ReportDownloadRequestMessage
                {
                    RecordId = Guid.NewGuid(),
                    BillingCategoryType = BillingCategoryType.Pax,
                    UserId = SessionUtil.UserId,
                    RequestingMemberId = SessionUtil.MemberId,
                    InputData =
                        ConvertUtil.SerializeXml(sisIsWebUsageReportModel, sisIsWebUsageReportModel.GetType()),
                    DownloadUrl = UrlHelperEx.ToAbsoluteUrl(Url.Action("DownloadReport", "OfflineReports", new { area = "Reports" })),
                    OfflineReportType = OfflineReportType.SisIsWebUsageReports
                };

                _offlineReportLogManager.AddOfflineReportLog(offlineReportLog);

                //Enqueue the message for isweb usage report.
                _referenceManager.EnqueTransactionTrailReport(enqueMessage);

                //Message will display on screen depending on Success or Failure of Enqueing message to queue.
                ShowSuccessMessage(Business.Constants.OfflineReportMessage);

                Logger.InfoFormat("SIS IS-WEB Usage Report has been queued successfully");
            }
            catch (Exception)
            {
                ShowErrorMessage("Failed to download the SIS IS-WEB Usage Report, please try again!");
            }

            return View(sisIsWebUsageReportModel);
        }

    }
}
