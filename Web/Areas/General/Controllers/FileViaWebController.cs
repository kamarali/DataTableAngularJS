using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Reports;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Web.UIModel.FileViaWeb;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using log4net;
using Trirand.Web.Mvc;
using FileUploadJsonResult = Iata.IS.Web.UIModel.FileViaWeb.FileUploadJsonResult;
using Iata.IS.Business;
using Iata.IS.Model.Common;
using Iata.IS.Business.MemberProfile;

namespace Iata.IS.Web.Areas.General.Controllers
{
    public class FileViaWebController : ISController
    {
        public IRepository<IsInputFile> IsInputFileRepository { get; set; }

        public IReferenceManager ReferenceManager { get; set; }

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        #region Upload Region

        /// <summary>
        /// Files the manager upload.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.General.FileManagement.Upload)]
        public ActionResult FileManagerUpload()
        {

            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account", new { area = string.Empty });
            }


            bool bshowGrid = false;

            if (String.IsNullOrEmpty(Request.Params["ShowGrid"]) == false)
            {
                if (Request.Params["ShowGrid"].ToUpper().Equals("TRUE"))
                {
                    bshowGrid = true;
                }
                else
                {
                    bshowGrid = false;
                }
            }

            if (bshowGrid)
            {
                ViewData["ShowGrid"] = true;

            }


            var temp = (string)TempData["FileUploadStatus"];
            ViewData["FileUploadStatus"] = TempData["FileUploadStatus"];
            if (TempData["FileUploadStatus"] != null)
                ShowErrorMessage(TempData["FileUploadStatus"].ToString());

            ViewData["Upload"] = true;
            TimeZoneInfo utctimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
            string localtimezone = string.IsNullOrEmpty(SessionUtil.TimeZone) ? "UTC" : SessionUtil.TimeZone;
            TimeZoneInfo selectedtimeZone = TimeZoneInfo.FindSystemTimeZoneById(localtimezone);
            var LocalDateTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, utctimeZone, selectedtimeZone);

            BillingPeriod billingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            var searchCriteria = new IsInputFile();
            searchCriteria.BillingMonth = billingPeriod.Month;
            searchCriteria.BillingPeriod = billingPeriod.Period;
            searchCriteria.BillingYear = billingPeriod.Year;
            searchCriteria.FileSubmissionFrom = LocalDateTime;
            searchCriteria.FileSubmissionTo = LocalDateTime;
            searchCriteria.FileFormatId = 0;


            string criteria = new JavaScriptSerializer().Serialize(searchCriteria);
            var gridModel = new FileUploadsModel();
            var uploadGrid = gridModel.FileUploadGrid;
            uploadGrid.DataUrl = Url.Action("UploadGridDataRequested", new { criteria });
            SetUpUploadGrid(uploadGrid);
            ViewData["FileUploadGridData"] = uploadGrid;
            return View(searchCriteria);
        }

        /// <summary>
        /// Files the manager download.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult FileManagerUpload(IsInputFile searchCriteria)
        {
            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account", new { area = string.Empty });
            }


            bool bshowGrid = false;

            if (String.IsNullOrEmpty(Request.Params["ShowGrid"]) == false)
            {
                if (Request.Params["ShowGrid"].ToUpper().Equals("TRUE"))
                {
                    bshowGrid = true;
                }
                else
                {
                    bshowGrid = false;
                }
            }

            if (bshowGrid)
            {
                ViewData["ShowGrid"] = true;

            }

            string criteria = new JavaScriptSerializer().Serialize(searchCriteria);
            var gridModel = new FileUploadsModel();
            var uploadGrid = gridModel.FileUploadGrid;
            uploadGrid.DataUrl = Url.Action("UploadGridDataRequested", new { criteria });
            SetUpUploadGrid(uploadGrid);
            ViewData["FileUploadGridData"] = uploadGrid;
            return View(searchCriteria);
        }
        /// <summary>
        /// Populates the fields.
        /// </summary>
        private void PopulateFields()
        {
            var billingMonthFromList = new List<MonthAndMonthNumber>();
            var billingMonthToList = new List<MonthAndMonthNumber>();
            var billingPeriodFromList = new List<int>();
            var billingPeriodToList = new List<int>();

            var uow = new iPayables_Data.UnitOfWork(ConnectionString.Instance.DirectConnectionString, iPayables_Data.UnitOfWork.ConnectionType.Data);

            List<iPayables_Data.Common.AvailableFileTypes> fileTypeList = iPayables_Business.Common.GetAllAvailableFileTypes(uow);
            var billingYearsList = new List<int>();
            for (int i = 1; i < 13; i++)
            {
                var currentMonthListItem = new MonthAndMonthNumber();
                currentMonthListItem.Month = GetMonthFromNumber(i);
                currentMonthListItem.MonthNumber = i;
                billingMonthFromList.Add(currentMonthListItem);
                billingMonthToList.Add(currentMonthListItem);
                if (i < 5)
                {
                    billingPeriodFromList.Add(i);
                    billingPeriodToList.Add(i);
                }
            }

            if (string.IsNullOrEmpty(SystemParameters.Instance.General.BillingYearToStartWith.ToString()))
            {
                throw new Exception(Iata.IS.Web.AppSettings.BillingYearExceptionString);
            }


            int startyear = int.Parse(SystemParameters.Instance.General.BillingYearToStartWith.ToString());

            for (int i = startyear; i <= DateTime.Now.Year; i++)
            {
                billingYearsList.Add(i);
            }

            ViewData["BillingMonthFromList"] = billingMonthFromList;
            ViewData["BillingMonthToList"] = billingMonthToList;
            ViewData["BillingPeriodFromList"] = billingPeriodFromList;
            ViewData["BillingPeriodToList"] = billingPeriodToList;
            ViewData["BillingYearsList"] = billingYearsList;
            ViewData["FileTypeList"] = fileTypeList;

        }

        public static bool EnqueueFileForSanityCheck(string filePath, string fileName, string memNumCode)
        {
            bool isEnqueued = false;
            Logger.Info("Enqueuing values.MEMBER_NUM_CODE:" + memNumCode + "FILE_NAME:" + fileName + "FILE_PATH:" + filePath);

            try
            {
                // enqueue message
                IDictionary<string, string> messages = new Dictionary<string, string> {
                { "MEMBER_NUM_CODE", memNumCode },
                { "FILE_NAME", fileName },
                { "FILE_PATH", filePath }
            };
                var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["SanityCheckQueueName"].Trim());

                queueHelper.Enqueue(messages);
                isEnqueued = true;
                Logger.Info("Enqueued values.");

            } // end try

            catch (Exception exception)
            {
                isEnqueued = false;
                Logger.Error("Error occurred while adding message to queue.", exception);
            } // end catch
            return isEnqueued;
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileUploadJsonResult AjaxUpload(HttpPostedFileBase file)
        {
            try
            {

                if (SessionUtil.UserId > 0)
                {
                    if (file == null)
                    {
                        TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.EmptyFile;
                        return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.EmptyFile } };
                    }



                    if (file.FileName.Length > 4 && file.FileName.ToUpper().EndsWith(".ZIP"))
                    {
                        // File was sent

                        if (
                          !System.Text.RegularExpressions.Regex.IsMatch(System.IO.Path.GetFileNameWithoutExtension(file.FileName),
                                                                        "^[a-zA-Z0-9._+-]*$"))
                        {
                            //ModelState.AddModelError("IncorrectFileType", Iata.IS.Web.AppSettings.IncorrectFileType);
                            //TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.IncorrectFileType;

                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.IncorrectFileType } };
                        }
                        //Get Logged On User's Information
                        iPayables.UserManagement.IUserManagement AuthManager = new iPayables.UserManagement.UserManagementModel();
                        iPayables.UserManagement.I_ISUser SISUser = AuthManager.GetUserByUserID(Iata.IS.Web.Util.SessionUtil.UserId);

                        //Get User's MemberID
                        int memberId = SISUser.Member.MemberID;

                        // tempFolder Location
                        string tempUploadLocation = @"C:\SFR";
                        if (!System.IO.Directory.Exists(tempUploadLocation))
                        {
                            System.IO.Directory.CreateDirectory(tempUploadLocation);
                        }

                        //Get MemberCode
                        string MemberCode = string.Empty;
                        var uow = new iPayables_Data.UnitOfWork(ConnectionString.Instance.ServiceConnectionString, iPayables_Data.UnitOfWork.ConnectionType.Data);
                        if (memberId > 0)
                        {
                            MemberCode = iPayables_Data.Common.GetMemberCode(memberId, uow);
                        }
                        else
                        {
                            //ModelState.AddModelError("FailedSaving", Iata.IS.Web.AppSettings.SaveFailed);
                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.SaveFailed } };


                        }


                        if (String.IsNullOrEmpty(MemberCode) == false)
                        {
                        }
                        else
                        {
                            //ModelState.AddModelError("FailedSaving", Iata.IS.Web.AppSettings.SaveFailed);
                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.SaveFailed } };

                        }

                        MemberCode = iPayables_Data.Common.GetShortMemberCode(memberId, uow);

                        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                        iPayables_Business.Common.FileTypes fileType = iPayables_Business.Common.SanityCheckFileTypeUpload(fileNameWithoutExtension, MemberCode);
                        if (fileType == iPayables_Business.Common.FileTypes.INVALID)
                        {
                            //ModelState.AddModelError("IncorrectFileType", Iata.IS.Web.AppSettings.IncorrectFileType);
                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.IncorrectFileType } };

                        }

                        if (FileIo.CheckFileExistInFolder(Path.Combine(FileIo.GetMemberUploadFolder(MemberCode), Path.GetFileName(file.FileName))) && ReferenceManager.IsRecordExistIn_Aq_SanityCheck(Path.GetFileName(file.FileName)))
                        {
                            //ModelState.AddModelError("FailedSaving", Iata.IS.Web.AppSettings.FileExistsAlready);

                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.FileExistsAlready } };

                        }
                        else if (iPayables_Data.Common.IsFileThereForMemberWithoutLocation(fileNameWithoutExtension, memberId.ToString(), uow))
                        {
                            //ModelState.AddModelError("FailedSaving", Iata.IS.Web.AppSettings.FileExistsAlready);
                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.FileExistsAlready } };

                        }

                        string fileLocation = FileIo.GetMemberUploadFolder(MemberCode);
                        fileLocation = System.IO.Path.GetFullPath(fileLocation);
                        file.SaveAs((Path.Combine(fileLocation, Path.GetFileName(file.FileName))));

                        //SCP293136 - SRM: Error while uploading Supp Docs via IS-Web
                        //Enque the File For sanity check
                        if (!EnqueueFileForSanityCheck(fileLocation, Path.GetFileName(file.FileName), MemberCode))
                        {
                            FileIo.DeleteFileFromLocation((Path.Combine(fileLocation, Path.GetFileName(file.FileName))));
                            return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.FileUploadFailsForIsWeb } };
                        }

                        iPayables_Business.Common.EmailToCorrectLocation(AppSettings.FileUploadSuccessful, string.Format(AppSettings.FileUploadSuccessfulBody, Path.GetFileName(file.FileName)), fileType, memberId.ToString(), uow);
                        //Spira IN008334- file upload message appears as a red warning
                        return new FileUploadJsonResult { Data = new { message = AppSettings.FileUploadSuccessfulForWeb, isFailed = "false" } };
                    }
                    else
                    {
                        //ModelState.AddModelError("FileShouldBeZip", AppSettings.ZipFileRequired);
                        //TempData["FileUploadStatus"] = AppSettings.ZipFileRequired;
                        RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                        return new FileUploadJsonResult { Data = new { message = Iata.IS.Web.AppSettings.IncorrectFileType, flag = "true" } };

                    }
                    RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });

                    return new FileUploadJsonResult { Data = new { message = string.Format("{0} uploaded successfully.", System.IO.Path.GetFileName(file.FileName)) } };
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
            catch (Exception ex)
            {
                RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                var strMessage = "There was an error in the file upload process. Please try again, or contact your administrator." + ex.Message;
                return new FileUploadJsonResult { Data = new { message = strMessage } };

            }
        }

        /// <summary>
        /// Uploads the file.
        /// </summary>
        /// <param name="uploadedFile">The uploaded file.</param>
        /// <param name="submit">The submit.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UploadFile(HttpPostedFileBase uploadedFile)
        {
            try
            {
                if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()))
                {
                    return RedirectToAction("LogOn", "Account", new { area = string.Empty });
                }
                else
                {
                    if (uploadedFile == null)
                    {
                        TempData["FileUploadStatus"] = AppSettings.EmptyFile;
                        return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                    }

                    if (uploadedFile.ContentLength == 25800)
                    {
                        TempData["FileUploadStatus"] = "Upload File Max. Size : 25MB";
                        return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                    }

                    //SCP112937 - Sanity Check Error in Supporting Docs
                    if(uploadedFile.FileName.Length > 65)
                    {
                        TempData["FileUploadStatus"] = Messages.FileNameLengthError;
                        return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                    }

                    if (uploadedFile.FileName.Length > 4 && uploadedFile.FileName.ToUpper().EndsWith(".ZIP"))
                    {  // File was sent

                        if (!System.Text.RegularExpressions.Regex.IsMatch(Path.GetFileNameWithoutExtension(uploadedFile.FileName), "^[a-zA-Z0-9._+-]*$"))
                        {
                            ModelState.AddModelError("IncorrectFileType", Iata.IS.Web.AppSettings.IncorrectFileType);
                            TempData["FileUploadStatus"] = AppSettings.IncorrectFileType;

                            return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                        }

                        //Get Logged On User's Information
                        iPayables.UserManagement.IUserManagement AuthManager = new iPayables.UserManagement.UserManagementModel();
                        iPayables.UserManagement.I_ISUser SISUser = AuthManager.GetUserByUserID(SessionUtil.UserId);

                        //Get User's MemberID
                        int memberId = SISUser.Member.MemberID;

                        // tempFolder Location
                        string tempUploadLocation = @"C:\SFR";
                        if (!Directory.Exists(tempUploadLocation))
                        {
                            Directory.CreateDirectory(tempUploadLocation);
                        }

                        //Get MemberCode
                        string MemberCode = string.Empty;
                        var uow = new iPayables_Data.UnitOfWork(ConnectionString.Instance.DirectConnectionString, iPayables_Data.UnitOfWork.ConnectionType.Data);
                        if (memberId > 0)
                        {
                            MemberCode = iPayables_Data.Common.GetMemberCode(memberId, uow);
                        }
                        else
                        {
                            ModelState.AddModelError("FailedSaving", AppSettings.SaveFailed);
                            TempData["FileUploadStatus"] = AppSettings.SaveFailed;

                            return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                        }


                        if (String.IsNullOrEmpty(MemberCode) == false)
                        {
                        }
                        else
                        {
                            ModelState.AddModelError("FailedSaving", AppSettings.SaveFailed);
                            TempData["FileUploadStatus"] = AppSettings.SaveFailed;

                            return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                        }

                        MemberCode = iPayables_Data.Common.GetShortMemberCode(memberId, uow);

                        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(uploadedFile.FileName);
                        iPayables_Business.Common.FileTypes fileType = iPayables_Business.Common.SanityCheckFileTypeUpload(fileNameWithoutExtension, MemberCode);
                        if (fileType == iPayables_Business.Common.FileTypes.INVALID)
                        {
                            ModelState.AddModelError("IncorrectFileType", Iata.IS.Web.AppSettings.IncorrectFileType);
                            TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.IncorrectFileType;

                            return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                        }
                        if (FileIo.CheckFileExistInFolder(Path.Combine(FileIo.GetMemberUploadFolder(MemberCode), Path.GetFileName(uploadedFile.FileName))))
                        {
                            ModelState.AddModelError("FailedSaving", Iata.IS.Web.AppSettings.FileExistsAlready);
                            TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.FileExistsAlready;

                            return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });

                        }
                        else if (iPayables_Data.Common.IsFileThereForMemberWithoutLocation(fileNameWithoutExtension, memberId.ToString(), uow))
                        {
                            ModelState.AddModelError("FailedSaving", Iata.IS.Web.AppSettings.FileExistsAlready);
                            TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.FileExistsAlready;

                            return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                        }

                        string fileLocation = FileIo.GetMemberUploadFolder(MemberCode);
                        fileLocation = System.IO.Path.GetFullPath(fileLocation);
                        uploadedFile.SaveAs((Path.Combine(fileLocation, Path.GetFileName(uploadedFile.FileName))));
                        iPayables_Business.Common.EmailToCorrectLocation(AppSettings.FileUploadSuccessful, string.Format(Iata.IS.Web.AppSettings.FileUploadSuccessfulBody, System.IO.Path.GetFileName(uploadedFile.FileName)), fileType, memberId.ToString(), uow);
                        ShowSuccessMessage(Iata.IS.Web.AppSettings.FileUploadSuccessfulForWeb);
                    }
                    else
                    {
                        ModelState.AddModelError("FileShouldBeZip", Iata.IS.Web.AppSettings.ZipFileRequired);
                        TempData["FileUploadStatus"] = Iata.IS.Web.AppSettings.ZipFileRequired;
                        return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
                    }

                }
                return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
            }
            catch (Exception Ex)
            {
                TempData["FileUploadStatus"] = "There was an error in the file upload process. Please try again, or contact your administrator." + Ex.Message;
                return RedirectToAction("FileManagerUpload", new { ShowGrid = "true" });
            }
        }

        /// <summary>
        /// Uploads the grid control request data.
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadGridControlRequestData()
        {
            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account", new { area = string.Empty });
            }
            else
            {
                var gridModel = new FileUploadsModel();
                var uploadGrid = gridModel.FileUploadGrid;

                uploadGrid.DataUrl = Url.Action("UploadGridDataRequested");
                SetUpUploadGrid(uploadGrid);

                return View(gridModel);
            }
        }

        /// <summary>
        /// This method is called when the grid requests data
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public JsonResult UploadGridDataRequested(string criteria)
        {
            var gridModel = new FileUploadsModel();
            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                var listOfUserData = new List<iPayables_Data.Common.ReturnResultUploadedSearch>();

                return gridModel.FileUploadGrid.DataBind(listOfUserData.AsQueryable<iPayables_Data.Common.ReturnResultUploadedSearch>());
            }
            else
            {
                try
                {
                    var searchCriteria = new IsInputFile();

                    if (!string.IsNullOrEmpty(criteria))
                    {
                        searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(IsInputFile)) as IsInputFile;
                    }

                    var searchCriteriastr = new iPayables_Data.Common.StructForUploadableSearch
                    {
                        Billing_Month = searchCriteria.BillingMonth,
                        Billing_Period = searchCriteria.BillingPeriod,
                        Billing_Year = searchCriteria.BillingYear,
                        FileType = searchCriteria.FileFormatId,
                        Filename = searchCriteria.FileName,
                        FileSubmissionFrom = searchCriteria.FileSubmissionFrom,
                        FileSubmissionTo = searchCriteria.FileSubmissionTo
                    };
                    string localtimezone = string.IsNullOrEmpty(SessionUtil.TimeZone) ? "UTC" : SessionUtil.TimeZone;
                    var uow = new iPayables_Data.UnitOfWork(ConnectionString.Instance.DirectConnectionString, iPayables_Data.UnitOfWork.ConnectionType.Data);
                    List<iPayables_Data.Common.ReturnResultUploadedSearch> listOfSearchResults = iPayables_Data.Common.GetRecentlyUploadedFilesForGrid(Iata.IS.Web.Util.SessionUtil.MemberId, searchCriteriastr, uow, localtimezone);
                    return gridModel.FileUploadGrid.DataBind(listOfSearchResults.AsQueryable<iPayables_Data.Common.ReturnResultUploadedSearch>());
                }
                catch (Exception e)
                {
                    var listOfUserData = new List<iPayables_Data.Common.ReturnResultUploadedSearch>();
                    return gridModel.FileUploadGrid.DataBind(listOfUserData.AsQueryable<iPayables_Data.Common.ReturnResultUploadedSearch>());
                }
            }

        }

        /// <summary>
        /// Sets up upload grid.
        /// </summary>
        /// <param name="UploadGrid">The upload grid.</param>
        private void SetUpUploadGrid(JQGrid UploadGrid)
        {
            UploadGrid.ToolBarSettings.ShowSearchToolBar = false;
            UploadGrid.ClientSideEvents.GridInitialized = "adjustGridContainer";
        }
        /// <summary>
        /// Batches the interface submissions exist.
        /// </summary>
        /// <returns></returns>
        Boolean BatchInterfaceSubmissionsExist()
        {
            return false;
        }

        /// <summary>
        /// Gets the month from number.
        /// </summary>
        /// <param name="_Number">The _ number.</param>
        /// <returns></returns>
        private string GetMonthFromNumber(int _Number)
        {
            string number = _Number.ToString();

            switch (number)
            {
                case "1":
                    return AppSettings.MonthJanuary;
                case "2":
                    return AppSettings.MonthFebruary;
                case "3":
                    return AppSettings.MonthMarch;
                case "4":
                    return AppSettings.MonthApril;
                case "5":
                    return AppSettings.MonthMay;
                case "6":
                    return AppSettings.MonthJune;
                case "7":
                    return AppSettings.MonthJuly;
                case "8":
                    return AppSettings.MonthAugust;
                case "9":
                    return AppSettings.MonthSeptember;
                case "10":
                    return AppSettings.MonthOctober;
                case "11":
                    return AppSettings.MonthNovember;
                default:
                    return AppSettings.MonthDecember;
            }
        }

        /// <summary>
        /// Sets the defaults for upload.
        /// </summary>
        private void SetDefaultsForUpload()
        {
            BillingPeriod billingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetLastClosedBillingPeriod(ClearingHouse.Ich);
            if (TempData.ContainsKey("BillingPeriodFrom") == false)
            {
                ViewData["BillingPeriodFrom"] = TempData["BillingPeriodFrom"] = billingPeriod.Period;
            }
            else
            {
                ViewData["BillingPeriodFrom"] = TempData["BillingPeriodFrom"];
            }

            if (TempData.ContainsKey("BillingPeriodTo") == false)
            {
                ViewData["BillingPeriodTo"] = TempData["BillingPeriodTo"] = billingPeriod.Period;
            }
            else
            {
                ViewData["BillingPeriodTo"] = TempData["BillingPeriodTo"];
            }

            if (TempData.ContainsKey("BillingMonthFrom") == false)
            {
                ViewData["BillingMonthFrom"] = TempData["BillingMonthFrom"] = billingPeriod.Month;
            }
            else
            {
                ViewData["BillingMonthFrom"] = TempData["BillingMonthFrom"];
            }

            if (TempData.ContainsKey("BillingYear") == false)
            {
                ViewData["BillingYear"] = TempData["BillingYear"] = billingPeriod.Year;
            }
            else
            {
                ViewData["BillingYear"] = TempData["BillingYear"];
            }

            if (TempData.ContainsKey("BillingMonthTo") == false)
            {
                ViewData["BillingMonthTo"] = TempData["BillingMonthTo"] = billingPeriod.Month;
            }
            else
            {
                ViewData["BillingMonthTo"] = TempData["BillingMonthTo"];
            }

            if (TempData.ContainsKey("FileType") == false)
            {
                ViewData["FileType"] = TempData["FileType"] = null;
            }
            else
            {
                ViewData["FileType"] = TempData["FileType"];
            }
        }
        
        #endregion

        #region Download Region

        private string FileManagerDownloadSearchType = "FileManagerDownloadSearch";
        private string DailyOutputDownloadSearchType = "DailyOutputDownloadSearch";

        /// <summary>
        /// Files the manager download.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.General.FileManagement.Download)]
        public ActionResult FileManagerDownload()
        {

            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account", new { area = string.Empty });
            }


            ViewData["DownloadFileError"] = TempData["DownloadFileError"];

            bool bshowGrid = false;

            if (String.IsNullOrEmpty(Request.Params["ShowGrid"]) == false)
            {
                if (Request.Params["ShowGrid"].ToUpper().Equals("TRUE"))
                {
                    bshowGrid = true;
                }
                else
                {
                    bshowGrid = false;
                }
            }

            if (bshowGrid)
            {
                ViewData["ShowGrid"] = true;

            }

            ViewData["Download"] = true;

            var searchCriteria = InitializeFileDownloadModel();
            //CMP529 : Daily Output Generation for MISC Bilateral Invoices
            DailyOutputFileDownloadSearch dailySearchModel = InitializeDailyOpFileDownloadModel();

            ViewData["DailyOutputFileDownloadSearch"] = dailySearchModel;

            //CMP #655: IS-WEB Display per Location ID
            ViewData["AssociatedLocation"] = new MultiSelectList(GetLocationAssociation(searchCriteria).ToArray(), "locationId", "locationCode");
            SessionUtil.LocationAssociationSearchIds = searchCriteria.MiscLocationCode;
            searchCriteria.MiscLocationCode = string.Empty;

            string criteria = new JavaScriptSerializer().Serialize(searchCriteria);
            string dailyOpSearchCriteria = new JavaScriptSerializer().Serialize(dailySearchModel);

            var gridModel = new FileDownloadSearchModel();

            gridModel.FileDownloadSearchGrid.DataUrl = Url.Action("SearchGridDataRequested", new { criteria, dailyOutputSearchCriteria = dailyOpSearchCriteria, searchType = FileManagerDownloadSearchType });

            SetUpGrid(gridModel.FileDownloadSearchGrid);
            ViewData["FileDownloadGridData"] = gridModel.FileDownloadSearchGrid;
            return View(searchCriteria);
        }

        /// <summary>
        /// Files the manager download.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileManagerDownload(IsInputFile searchCriteria)
        {

            if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account", new { area = string.Empty });
            }


            //CMP529 : Daily Output Generation for MISC Bilateral Invoices
            DailyOutputFileDownloadSearch dailyOpSearchModel = InitializeDailyOpFileDownloadModel();
            ViewData["DailyOutputFileDownloadSearch"] = dailyOpSearchModel;

            //CMP #655: IS-WEB Display per Location ID
            var associatedLocations = GetLocationAssociation(searchCriteria);
              ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode");
            // server Side Validation for Associatin Location
            var selectedBillingMemberLocationList = searchCriteria.MiscLocationCode.Split(Convert.ToChar(","));
            SessionUtil.LocationAssociationSearchIds = "";
                foreach (var location in from location in selectedBillingMemberLocationList
                                         where location != null
                                         let contains = associatedLocations.SingleOrDefault(l => l.LocationCode == location)
                                         where contains != null
                                         select location)
                {
                    SessionUtil.LocationAssociationSearchIds += "," + location;
                }
            if (SessionUtil.LocationAssociationSearchIds.Length == 0) SessionUtil.LocationAssociationSearchIds = "0";
            searchCriteria.MiscLocationCode = string.Empty;
            


            string criteria = new JavaScriptSerializer().Serialize(searchCriteria);
            string dailyOpSearchCriteria = new JavaScriptSerializer().Serialize(dailyOpSearchModel);

            var gridModel = new FileDownloadSearchModel();
            gridModel.FileDownloadSearchGrid.DataUrl = Url.Action("SearchGridDataRequested", new { criteria, dailyOutputSearchCriteria = dailyOpSearchCriteria, searchType = FileManagerDownloadSearchType });
            SetUpGrid(gridModel.FileDownloadSearchGrid);
            ViewData["FileDownloadGridData"] = gridModel.FileDownloadSearchGrid;
            return View(searchCriteria);
        }

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult FileManagerDailyOutputDownload(DailyOutputFileDownloadSearch searchCriteria)
        {
          if (string.IsNullOrEmpty(Util.SessionUtil.UserId.ToString()))
          {
            return RedirectToAction("LogOn", "Account", new { area = string.Empty });
          }
          IsInputFile fileManagerDownloadSearchModel = InitializeFileDownloadModel();
          ViewData["DailyOutputFileDownloadSearch"] = searchCriteria;

          //CMP #655: IS-WEB Display per Location ID
           var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager));
           var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId,SessionUtil.MemberId);
           ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "LocationId", "LocationCode");
           // server Side Validation for Associatin Location
            var selectedBillingMemberLocationList = searchCriteria.MiscLocCode.Split(Convert.ToChar(","));
           SessionUtil.LocationAssociationSearchIds = "";
           foreach (var location in from location in selectedBillingMemberLocationList
                                    where location != null
                                    let contains = associatedLocations.SingleOrDefault(l => l.LocationCode == location)
                                    where contains != null
                                    select location)
           {
               SessionUtil.LocationAssociationSearchIds += "," + location;
           }
           if (SessionUtil.LocationAssociationSearchIds.Length == 0) SessionUtil.LocationAssociationSearchIds = "0";
           searchCriteria.MiscLocCode = string.Empty;

          string criteria = new JavaScriptSerializer().Serialize(searchCriteria);
          string fileDownloadSearchCriteria = new JavaScriptSerializer().Serialize(fileManagerDownloadSearchModel);

          var gridModel = new FileDownloadSearchModel();
            gridModel.FileDownloadSearchGrid.DataUrl = Url.Action("SearchGridDataRequested",
                                                                  new
                                                                      {
                                                                          criteria = fileDownloadSearchCriteria,
                                                                          dailyOutputSearchCriteria = criteria,
                                                                          searchType = DailyOutputDownloadSearchType
                                                                      });
          SetUpGrid(gridModel.FileDownloadSearchGrid);
          ViewData["FileDownloadGridData"] = gridModel.FileDownloadSearchGrid;
          return View("FileManagerDownload", fileManagerDownloadSearchModel);
        }


        private List<MemberLocationAssociation> GetLocationAssociation(IsInputFile objInputFile)
        {
            //CMP #655: IS-WEB Display per Location ID
            var associatedLocations = new List<MemberLocationAssociation>();
            if (objInputFile != null)
            {
                var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager));
                associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId,
                                                                                      SessionUtil.MemberId);
                if (objInputFile.MiscLocationCode == null)
                {
                    foreach (var item in associatedLocations)
                    {
                        objInputFile.MiscLocationCode += "," + item.LocationCode;
                    }
                    if (associatedLocations.Count == 0) objInputFile.MiscLocationCode = "0";
                }
            }
          
            return associatedLocations;
        }

        /// <summary>
       /// This method is called when the grid requests data
       /// </summary>
       /// <param name="criteria">The criteria.</param>
       /// <param name="dailyOutputSearchCriteria"> Daily Output Search Criteria</param>
       /// <param name="searchType"> Search Type </param>
       /// <returns></returns>
       [Authorize]
       public JsonResult SearchGridDataRequested(string criteria, string dailyOutputSearchCriteria, string searchType)
       {
         // SCP320544: Dashboard Optimization (SearchGridDataRequested)
         // Logs added.
         Logger.InfoFormat("--------------- Start method SearchGridDataRequested ---------------------");
         
         var gridModel = new FileDownloadSearchModel();
         if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()))
         {
           var listOfUserData = new List<iPayables_Data.Common.ReturnResultDownloadableSearch>();
           
           return gridModel.FileDownloadSearchGrid.DataBind(listOfUserData.AsQueryable());
         }

         try
         {
           var searchCriteria = new IsInputFile();
           
           if (!string.IsNullOrEmpty(criteria))
           {
             searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof (IsInputFile)) as IsInputFile;
           }

           var dailyOpFileSearchCriteria = new JavaScriptSerializer().Deserialize(dailyOutputSearchCriteria, typeof (DailyOutputFileDownloadSearch))
                                            as DailyOutputFileDownloadSearch;

             
           var searchCriteriaStr = new iPayables_Data.Common.StructForDownloadableSearch
                                     {
                                       Billing_Month_From = searchCriteria == null ? DateTime.UtcNow.Month : searchCriteria.BillingMonthFrom,
                                       Billing_Month_To = searchCriteria == null ? DateTime.UtcNow.Month : searchCriteria.BillingMonthTo,
                                       Billing_Period_From = searchCriteria == null ? 1 : searchCriteria.BillingPeriodFrom,
                                       Billing_Period_To = searchCriteria == null ? 1 : searchCriteria.BillingPeriodTo,
                                       Billing_Year = searchCriteria == null ? DateTime.UtcNow.Year : searchCriteria.BillingYear,
                                       FileType = searchType == FileManagerDownloadSearchType ? searchCriteria.FileFormatId : dailyOpFileSearchCriteria.FileFormatId,
                                       //CMP529 : Daily Output Generation for MISC Bilateral Invoices
                                       DeliveryDateFrom = dailyOpFileSearchCriteria == null ? DateTime.UtcNow : dailyOpFileSearchCriteria.DeliveryDateFrom,
                                       DeliveryDateTo = dailyOpFileSearchCriteria == null ? DateTime.UtcNow : dailyOpFileSearchCriteria.DeliveryDateTo,
                                       //CMP#622: MISC Outputs Split as per Location ID
                                       MiscLocationCode = string.IsNullOrWhiteSpace(SessionUtil.LocationAssociationSearchIds) ? null : SessionUtil.LocationAssociationSearchIds,
                                       SearchType = searchType == FileManagerDownloadSearchType ? 1 : 2
                                     };
           
           var memberId = SessionUtil.MemberId;
           
           Logger.InfoFormat("Member Id: {0}", memberId);
           Logger.InfoFormat("Search Criteria: ");

           if (searchCriteria != null)
           {
             Logger.InfoFormat(
                 "Billing_Month_From = {0}, Billing_Month_To = {1}, Billing_Period_From = {2}, Billing_Period_To = {3}, Billing_Year = {4}, SearchType = {5},",
                 searchCriteria.BillingMonthFrom,
                 searchCriteria.BillingMonthTo,
                 searchCriteria.BillingPeriodFrom,
                 searchCriteria.BillingPeriodTo,
                 searchCriteria.BillingYear,
                 searchType == FileManagerDownloadSearchType ? 1 : 2);

             if (dailyOpFileSearchCriteria != null)
             {
               Logger.InfoFormat("FileType = {0}, DeliveryDateFrom =  {1}, DeliveryDateTo = {2}",
                                 searchType == FileManagerDownloadSearchType ? searchCriteria.FileFormatId
                                                                             : dailyOpFileSearchCriteria.FileFormatId, dailyOpFileSearchCriteria.DeliveryDateFrom,
                                 dailyOpFileSearchCriteria.DeliveryDateTo);
             }
             else
             {
               Logger.InfoFormat("dailyOpFileSearchCriteria is null");
             }
           }
           else
           {
             Logger.InfoFormat("searchCriteria is null");
           }

           // Change for CMP597 : If file type is Change Information for Reference Data or Complete Reference Data or Complete Contacts Data than get data for all members
           if (searchCriteria.FileFormatId == (int)FileFormatType.ChangeInfoReferenceDataUpdateCsv ||
               searchCriteria.FileFormatId == (int)FileFormatType.CompleteReferenceDataCsv || searchCriteria.FileFormatId == (int)FileFormatType.CompleteContactsDataCsv)
           {
             memberId = 0;
           }

           var localtimezone = string.IsNullOrEmpty(SessionUtil.TimeZone) ? "UTC" : SessionUtil.TimeZone;
           
           Logger.InfoFormat("Time Zone: {0}", localtimezone);
           
           var uow = new iPayables_Data.UnitOfWork(ConnectionString.Instance.DirectConnectionString, iPayables_Data.UnitOfWork.ConnectionType.Data);
           
           Logger.InfoFormat("DataBase Call Start.");
           
           var listOfSearchResults = iPayables_Business.Common.SearchForDownloadableRecords(searchCriteriaStr,
                                                                                            AppSettings.DownloadFileImageAltText,
                                                                                            AppSettings.DownloadFileImageToolTip,
                                                                                            Url.Content(AppSettings.DownloadZipFileImagePath),
                                                                                            memberId,
                                                                                            uow,
                                                                                            localtimezone);
           Logger.InfoFormat("DataBase Call End.");
           Logger.InfoFormat("--------------- End method SearchGridDataRequested ---------------------");
           
           return gridModel.FileDownloadSearchGrid.DataBind(listOfSearchResults.AsQueryable());
         }
         catch (Exception)
         {
           var listOfUserData = new List<iPayables_Data.Common.ReturnResultDownloadableSearch>();
           
           return gridModel.FileDownloadSearchGrid.DataBind(listOfUserData.AsQueryable());
         }
       }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="FileToDownload">The file to download.</param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DownloadFile(object FileToDownload)
        {
            try
            {
                if (Util.SessionUtil.UserId == 0)
                {
                    return GetBlankFileResult();
                }
                else
                {
                    string[] FileLocationArray = (string[])FileToDownload;
                    string FileLocation = null;

                    if (FileLocationArray.Count() > 0)
                    {
                        FileLocation = FileLocationArray[0];
                    }

                    if (string.IsNullOrEmpty(FileLocation))
                    {
                        TempData["DownloadFileError"] = AppSettings.DownloadFileError;
                        return RedirectToAction("FileManagerDownload");

                    }
                    var isFileGuid = ConvertUtil.ConvertStringtoGuid(FileLocation);
                    var isFile = IsInputFileRepository.Get(f => f.Id == isFileGuid).FirstOrDefault();

                    //Get MemberCode
                    string MemberCode = string.Empty;
                    iPayables_Data.UnitOfWork uow = new iPayables_Data.UnitOfWork(ConnectionString.Instance.DirectConnectionString, iPayables_Data.UnitOfWork.ConnectionType.Data);

                    FileLocation = isFile.FileLocation + "\\" + isFile.FileName;
                    if (!System.IO.File.Exists(FileLocation))
                    {
                        TempData["DownloadFileError"] = "File Not Exists On Specified location ";
                        return RedirectToAction("FileManagerDownload");
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

                    //var tempFolderPath = FileIo.GetForlderPath(SFRFolderPath.SFRTempRootPath);
                    //// Create Temp Folder in C :\ to copy network files 
                    //if (!Directory.Exists(tempFolderPath))
                    //{
                    //  Directory.CreateDirectory(tempFolderPath);
                    //}

                    //// Delete Oldest files from Temp Folder
                    //DeleteOldestFilesFromTempFolder(tempFolderPath);

                    ////Copy Network file into Temp Folder for Download
                    //System.IO.File.Copy(FileLocation, tempFolderPath + @"\" + System.IO.Path.GetFileName(FileLocation), true);

                    ////FileLocation = System.IO.Path.GetFullPath(FileLocation);
                    //FileLocation = System.IO.Path.GetFullPath(FileLocation + @"\" + System.IO.Path.GetFileName(FileLocation));
                 
                  return File(FileLocation, contentType, System.IO.Path.GetFileName(FileLocation));

                }
            }
            catch (Exception e)
            {
                return GetBlankFileResult();
            }
        }


      
      //private void DeleteOldestFilesFromTempFolder(string path)
      //{
      //  var tempDownloadDir = new DirectoryInfo(path);
      //  foreach (FileInfo file in tempDownloadDir.GetFiles())
      //  {
      //    if (file.CreationTimeUtc <= DateTime.UtcNow.AddDays(-1))
      //    {
      //      file.Delete();
      //    }
      //  }

      //}

        /// <summary>
        /// Gets the blank file result.
        /// </summary>
        /// <returns></returns>
        private ActionResult GetBlankFileResult()
        {
            TempData["DownloadFileError"] = AppSettings.DownloadFileError;

            var u = new UrlHelper(this.ControllerContext.RequestContext);
            return Redirect(u.Action("FileManagerDownload", "FileViaWeb", new { ShowGrid = "true" }, "http"));
        }

        /// <summary>
        /// Sets up grid.
        /// </summary>
        /// <param name="downloadSearchGrid">The download search grid.</param>
        private void SetUpGrid(JQGrid downloadSearchGrid)
        {
            // show the search toolbar
            downloadSearchGrid.ToolBarSettings.ShowSearchToolBar = false;
            downloadSearchGrid.ClientSideEvents.GridInitialized = "adjustGridContainer";
            downloadSearchGrid.Columns.Find(c => c.DataField == "Action").Formatter = new CustomFormatter
            {
                FormatFunction = "formatlink",
                UnFormatFunction = "unformatlink"
            };
        }

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        private DailyOutputFileDownloadSearch InitializeDailyOpFileDownloadModel()
        {
            DailyOutputFileDownloadSearch dailySearchModel = new DailyOutputFileDownloadSearch();
            dailySearchModel.DeliveryDateFrom = (dailySearchModel.DeliveryDateFrom.HasValue)
                                                                ? dailySearchModel.DeliveryDateFrom.Value.ToLocalTime()
                                                                : DateTime.UtcNow.Date.AddDays(-1);
            dailySearchModel.DeliveryDateTo = (dailySearchModel.DeliveryDateTo.HasValue)
                                                                 ? dailySearchModel.DeliveryDateTo.Value.ToLocalTime()
                                                                 : DateTime.UtcNow.Date.AddDays(-1);
            dailySearchModel.FileFormatId = 0;
            return dailySearchModel;
        }

        private IsInputFile InitializeFileDownloadModel()
        {
            BillingPeriod billingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            var searchCriteria = new IsInputFile();
            searchCriteria.BillingMonthFrom = billingPeriod.Month;
            searchCriteria.BillingMonthTo = billingPeriod.Month;
            searchCriteria.BillingPeriodFrom = billingPeriod.Period;
            searchCriteria.BillingPeriodTo = billingPeriod.Period;
            searchCriteria.BillingYear = billingPeriod.Year;
            searchCriteria.FileFormatId = 0;
            return searchCriteria;
        }

        #endregion

    }
}
