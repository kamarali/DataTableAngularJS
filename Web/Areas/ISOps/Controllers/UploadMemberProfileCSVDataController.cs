using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfileCSVUpload.Impl;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.UploadMemberProfileCSVData;
using Iata.IS.Web.UIModel.MemberUploadCSVData;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using System.IO;
using iPayables_Data;
using Trirand.Web.Mvc;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Web.Areas.ISOps.Controllers
{
    public class UploadMemberProfileCSVDataController : ISController
    {
        public IRepository<IsInputFile> IsInputFileRepository { get; set; }
        /// <summary>
        /// CMP#608: Created controller to handle all UI actions
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.UploadMemberProfileData)]
        public ActionResult UploadMemberProfileCSVData()
        {

            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account", new { area = string.Empty });
            }

            ViewData["DownloadFileError"] = TempData["DownloadFileError"];

            var gridModel = new MemberUploadCSVModel();
            var downlaoddGrid = gridModel.MemberUploadCSVGrid;
            downlaoddGrid.DataUrl = Url.Action("UploadGridDataRequested");
            SetUpGrid(downlaoddGrid);
            ViewData["MemberUploadGridData"] = downlaoddGrid;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileUploadJsonResult AjaxUpload(HttpPostedFileBase file)
        {
            if (SessionUtil.UserId > 0)
            {
                if (file == null)
                {
                    TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.EmptyFile;
                    return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.EmptyFile } };
                }

                var fileFullPath = string.Empty;
                try
                {
                    if (file.FileName.Length > 4 && file.FileName.ToUpper().EndsWith(".ZIP"))
                    {
                        // file upload Location
                        string uploadLocationPath = SystemParameters.Instance.General.SFRRootBasePath +
                                                    @"\ToBeProcessed\MemberCSVUpload\";

                        if (!System.IO.Directory.Exists(uploadLocationPath))
                        {
                            System.IO.Directory.CreateDirectory(uploadLocationPath);
                        }

                        fileFullPath = Path.Combine(uploadLocationPath, Path.GetFileName(file.FileName));

                        if (FileIo.CheckFileExistInFolder(fileFullPath))
                        {
                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.FileExistsAlready } };
                        }

                        file.SaveAs(fileFullPath);

                        var memberProfileCsvUploadManager = new MemberProfileCsvUploadManager();

                        if (memberProfileCsvUploadManager.PerformFirstLevelValidationsForISWEB(fileFullPath))
                        {
                            BillingPeriod billingPeriod =
                                Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).
                                    GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
                            var isFileLogId = memberProfileCsvUploadManager.AddIsFileLogEntry(billingPeriod,
                                                                                              fileFullPath,
                                                                                              FileFormatType.MemberProfileCSVUpload,
                                                                                              SessionUtil.UserId);

                            // Add to queue

                            IDictionary<string, string> message = new Dictionary<string, string>
                                                                      {
                                                                          {
                                                                              "IS_FILE_LOG_ID",
                                                                              ConvertUtil.ConvertGuidToString(
                                                                                  isFileLogId)
                                                                              }
                                                                      };
                            var queueHelper = new QueueHelper("AQ_MEM_CSV_UPLOAD");
                            queueHelper.Enqueue(message);
                        }

                        return new FileUploadJsonResult { Data = new { message = AppSettings.FileUploadSuccessfulForMemberUploadCsv, isFailed = "false" } };
                    }
                    else
                    {
                        RedirectToAction("UploadMemberProfileCSVData", new { ShowGrid = "true" });
                        return new FileUploadJsonResult { Data = new { message = AppSettings.IncorrectFileType, flag = "true" } };

                    }
                }
                catch (ISBusinessException exception)
                {
                    /* In case of exception while uploading - delete file from temp location */
                    try
                    {
                        System.IO.File.Delete(fileFullPath);
                    }
                    catch (Exception exception1)
                    {
                        /* Eat it*/
                    }

                    var exMessage = string.Format("{0} - {1} {2}", exception.ErrorCode, exception.Message,
                                          Messages.ResourceManager.GetString(exception.ErrorCode));
                    return new FileUploadJsonResult { Data = new { message = exMessage, flag = "true" } };
                }
            }
            else
            {
                // Clear User Related Session
                SessionUtil.IsLogOutProxyOption = false;
                SessionUtil.IsLoggedIn = false;
                Session.Abandon();

                //return LogOn URL to Json Call.
                var returnParam = AdminSystem.SystemParameters.Instance.General.LogOnURL;

                return new FileUploadJsonResult { Data = new { message = returnParam, flag = "false" } };
            }
        }

        [Authorize]
        public JsonResult UploadGridDataRequested(string criteria)
        {
            var gridModel = new MemberUploadCSVModel();

            var memberProfileCsvUploadManager = new MemberProfileCsvUploadManager();
            List<MemberProfileUploadCsv> memberProfileUploadCsvList = 
                memberProfileCsvUploadManager.GetMemberUploadCSVDataFromISFileLog(Url.Content(AppSettings.DownloadZipFileImagePath), 
                AppSettings.DownloadFileImageAltText, AppSettings.DownloadFileImageToolTip);

            /* The EST/EDT equivalent of the upload date time of the file should be displayed on grid, so applying the conversion */
            foreach (var memberProfileUploadCsvListRow in memberProfileUploadCsvList)
            {
                memberProfileUploadCsvListRow.FileUploadDate = CalendarManager.ConvertUtcTimeToYmq(memberProfileUploadCsvListRow.FileUploadDate);
            }
            
            return gridModel.MemberUploadCSVGrid.DataBind(memberProfileUploadCsvList.AsQueryable<MemberProfileUploadCsv>());
        }

        /// <summary>
        /// Sets up grid.
        /// </summary>
        /// <param name="downloadGrid">The download search grid.</param>
        private static void SetUpGrid(JQGrid downloadGrid)
        {
            downloadGrid.ClientSideEvents.GridInitialized = "adjustGridContainer";
            downloadGrid.Columns.Find(c => c.DataField == "DownloadFileId").Formatter = new CustomFormatter
            {
                FormatFunction = "formatlink",
                UnFormatFunction = "unformatlink"
            };
        }


        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="fileToDownload">The file to download.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DownloadFile(object fileToDownload)
        {
            try
            {
                if (Util.SessionUtil.UserId == 0)
                {
                    return GetBlankFileResult();
                }
                else
                {
                    string[] FileLocationArray = (string[])fileToDownload;
                    string FileLocation = null;

                    if (FileLocationArray.Count() > 0)
                    {
                        FileLocation = FileLocationArray[0];
                    }



                    if (string.IsNullOrEmpty(FileLocation))
                    {
                        TempData["DownloadFileError"] = AppSettings.DownloadFileError;
                        return RedirectToAction("UploadMemberProfileCSVData");

                    }

                    var isFileGuid = ConvertUtil.ConvertStringtoGuid(FileLocation);
                    var isFile = IsInputFileRepository.Get(f => f.Id == isFileGuid).FirstOrDefault();
                   

                    FileLocation = isFile.FileLocation + "\\" + isFile.FileName;
                    
                    if (!System.IO.File.Exists(FileLocation))
                    {
                        TempData["DownloadFileError"] = AppSettings.DownloadFileError;
                        return RedirectToAction("UploadMemberProfileCSVData");
                    }

                    string Path = System.IO.Path.GetFullPath(FileLocation);
                    string FileName = System.IO.Path.GetFileName(FileLocation);

                    string contentType;
                    var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(FileName);
                    if (reg != null)
                    {
                        contentType = reg.GetValue("Content Type") as string;
                    }
                    else
                    {
                        contentType = "application/txt";
                    }

                    return File(FileLocation, contentType, System.IO.Path.GetFileName(FileLocation));

                }
            }
            catch (Exception e)
            {
                return GetBlankFileResult();
            }
        }

        /// <summary>
        /// Gets the blank file result.
        /// </summary>
        /// <returns></returns>
        private ActionResult GetBlankFileResult()
        {
            TempData["DownloadFileError"] = AppSettings.DownloadFileError;
            return RedirectToAction("UploadMemberProfileCSVData");
        }
    }
}