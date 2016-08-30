using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.Jobs.ACHRecapSheet;
using Iata.IS.Business.Jobs.Calendar;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.Reports;
using Iata.IS.Business.SystemMonitor;
using Iata.IS.Core;
using Iata.IS.Business.ValueConfirmation;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.LegalXMLFileGenerator;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SystemMonitor;
using Iata.IS.Web.Controllers;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.UIModel.Grid.Profile;
using Iata.IS.Web.UIModel.Grid.SystemMonitor;
using Iata.IS.Web.Util;
using System.Web.Script.Serialization;
using Iata.IS.Web.Util.Filters;
using log4net;
using CompletedJobs = Iata.IS.Web.UIModel.Grid.SystemMonitor.CompletedJobs;
using IsWebResponse = Iata.IS.Web.UIModel.Grid.SystemMonitor.IsWebResponse;
using OutstandingItems = Iata.IS.Web.UIModel.Grid.SystemMonitor.OutstandingItems;
using PendingJobs = Iata.IS.Web.UIModel.Grid.SystemMonitor.PendingJobs;
using Iata.IS.Business.LegalArchive;
using FileType = Iata.IS.Model.Pax.Enums.FileType;
using Iata.IS.AdminSystem;

namespace Iata.IS.Web.Areas.ISOps.Controllers
{
    public class ManageSystemMonitorController : ISController
    {

        private readonly ICalendarManager _calenderManager;
        private readonly IArchivalManager _archivalManager;

        public ManageSystemMonitorController(ICalendarManager calenderManager, IArchivalManager archivalManager)
        {
            _calenderManager = calenderManager;
            _archivalManager = archivalManager;
        }

        #region "Declaration"

        private const string CompletedJobGridAction = "CompletedJobGridData";
        private const string PendingJobGridAction = "PendingJobGridData";
        private const string SystemAlertGridAction = "SystemAlertGridData";
        private const string LoggedInUsersGridAction = "LoggedInUsersGridData";
        private const string OutStandingItemsGridAction = "OutStandingItemsGridData";
        private const string IsWebResponseGridAction = "IsWebResponseGridData";
        public const string AchRecapSheet = "ACH_RECAP_SHEET";
        public const string OldIdec = "DOWNGRADE_IDEC";
        public const string BvcRequest = "BVC_REQUEST";
        public const string ResendOutputFile = "RESEND_OUTPUT_FILES";
        public const string OfflineCollection = "OFFLINE_COLLECTION_ARCHIVE";
        public const string ProcessedInvoiceDataCsv = "PROCESSED_INVOICE_DATA_CSV";
        public const string NilFormC = "NIL_FORM_C";
        public const string InvoiceFile = "INVOICE_FILE";
        public const string UpdatePendingInvoice = "UPDATE_PENDING_INVOICES";

        public IMemberRepository Member { get; set; }

        public IAlertMessageNotesRepositiory MsgRepository { get; set; }

        public ISystemMonitorManager SystemMonitorManager { get; set; }

        public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }

        public ICalendarManager CalendatManager { get; set; }

        public IRepository<IsInputFile> IsInputFileRepository { get; set; }

        public IUatpAtcanDetailsManager UatpAtcanDetailsManager { get; set; }

        public IRepository<SisExtendedParameter> SisExtendedParameterRepository { get; set; }

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        #endregion

        #region "System Monitor"

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.SysMonitorAccess)]
        public ActionResult Manage()
        {
            ViewData["DownloadFileError"] = TempData["DownloadFileError"];

            return View();
        }

        #endregion

        #region "Current Stats"


        public ActionResult UserCountByRegion()
        {
            return View();
        }

        public ActionResult CurrentStats()
        {

            try
            {

                // Pending Jobs
                var pendingJobsGrid = new PendingJobs(ControlIdConstants.PendingJobGrid, Url.Action(PendingJobGridAction));
                pendingJobsGrid.ShowRefreshButton = true;
                ViewData[ViewDataConstants.PendingJobGrid] = pendingJobsGrid.Instance;

                // Complete Jobs
                var completedJobsGrid = new CompletedJobs(ControlIdConstants.CompletedJobGrid, Url.Action(CompletedJobGridAction));
                completedJobsGrid.ShowRefreshButton = true;
                ViewData[ViewDataConstants.CompletedJobGrid] = completedJobsGrid.Instance;

                // System Alerts 
                var systemAlertGrid = new SystemAlert(ControlIdConstants.SystemAlertGrid, Url.Action(SystemAlertGridAction));
                systemAlertGrid.ShowRefreshButton = true;
                ViewData[ViewDataConstants.SystemAlertsJobGrid] = systemAlertGrid.Instance;

                // Logged In Users by Members
                var loggedInUserGrid = new LoggedInUsers(ControlIdConstants.LoggedInUserGrid, Url.Action(LoggedInUsersGridAction));
                loggedInUserGrid.ShowRefreshButton = true;
                ViewData[ViewDataConstants.LoggedInUsersJobGrid] = loggedInUserGrid.Instance;

                // Outstanding Items Section
                var outstandingItemsGrid = new OutstandingItems(ControlIdConstants.OustandingItemsGrid, Url.Action(OutStandingItemsGridAction));
                outstandingItemsGrid.ShowRefreshButton = true;
                ViewData[ViewDataConstants.OutstandingItemsGrid] = outstandingItemsGrid.Instance;

                //IS-WEB Response Time Statistics
                var iSWebResponseGrid = new IsWebResponse(ControlIdConstants.IsWebResponseGrid, Url.Action(IsWebResponseGridAction));
                iSWebResponseGrid.ShowRefreshButton = true;
                ViewData[ViewDataConstants.ISWEBResponseGrid] = iSWebResponseGrid.Instance;

            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }

            return View();
        }

        
        /// <summary>
        /// Completed Job Record Grid binding 
        /// Author : Vinod Patil
        /// Date : 2 Aug 2011
        /// </summary>
        /// <returns></returns>
        public JsonResult CompletedJobGridData(string criteria, string sidx, string sord, int page, int rows)
        {
            var completedJobsGrid = new CompletedJobs(ControlIdConstants.CompletedJobGrid, Url.Action(CompletedJobGridAction));
            completedJobsGrid.ShowRefreshButton = true;
            string localtimezone = string.IsNullOrEmpty(SessionUtil.TimeZone) ? "UTC" : SessionUtil.TimeZone;
            //SCP#440318 - SRM: current stats screen is too slow
            var completedJobsList = SystemMonitorManager.GetCompletedJobs(localtimezone,rows,page,sidx,sord).AsQueryable();

            var totalRecords = completedJobsList.Select(x => x.TotalRows).FirstOrDefault();
            
            foreach (var completedJobse in completedJobsList)
            {
                completedJobse.StartTime = CalendarManager.ConvertUtcTimeToYmq(completedJobse.StartTime);
                completedJobse.EndTime = CalendarManager.ConvertUtcTimeToYmq(completedJobse.EndTime);
            }

            //return completedJobsGrid.DataBind(completedJobsList);
            return GetCompletedJobsGridPagination(page, rows, completedJobsList, totalRecords);
        }


        private JsonResult GetCompletedJobsGridPagination(int page, int rows, IQueryable<Model.SystemMonitor.CompletedJobs> completedJobsList, int? totalRecords)
        {
            //Calculating total pages grid will show
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            //Creating json result to bind to database.
            //SCP 448241: SRM:time format change
            var jsonData = new
            {
                total = totalPages,
                page = page,
                records = totalRecords,
                rows = completedJobsList.ToArray()
            };

            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Pending Job Record Grid binding 
        /// Author : Vinod Patil
        /// Date : 2 Aug 2011
        /// </summary>
        /// <returns></returns>
        public JsonResult PendingJobGridData()
        {
            var pendingJobsGrid = new PendingJobs(ControlIdConstants.PendingJobGrid, Url.Action(PendingJobGridAction));
            pendingJobsGrid.ShowRefreshButton = true;
            var pendingJobsList = SystemMonitorManager.GetPendingJobs().AsQueryable();
            foreach (var pendingJobse in pendingJobsList)
            {
                pendingJobse.FileTime = CalendarManager.ConvertUtcTimeToYmq(pendingJobse.FileTime);
            }

            return pendingJobsGrid.DataBind(pendingJobsList);

        }

        /// <summary>
        /// System Alert Record Grid binding 
        /// Author : Vinod Patil
        /// Date : 2 Aug 2011
        /// </summary>
        /// <returns></returns>
        public JsonResult SystemAlertGridData()
        {
            var systemAlertGrid = new SystemAlert(ControlIdConstants.SystemAlertGrid, Url.Action(SystemAlertGridAction));
            systemAlertGrid.ShowRefreshButton = true;
            var gridData = BroadcastMessagesManager.GetSisOpsAlerts(null);

            foreach (var data in gridData)
            {
                data.RaisedDate = CalendarManager.ConvertUtcTimeToYmq(data.RaisedDate);

                //if (data.Detail.Length > 60)
                //{
                //  data.Detail = String.Format("{0}...", data.Detail.Substring(0, 57));
                //}
            }

            return systemAlertGrid.DataBind(gridData.AsQueryable());

        }

        /// <summary>
        /// Logged In  user by Members Record Grid binding 
        /// Author : Vinod Patil
        /// Date : 3 Aug 2011
        /// </summary>
        /// <returns></returns>
        public JsonResult LoggedInUsersGridData()
        {
            var loggedInUsersGrid = new LoggedInUsers(ControlIdConstants.LoggedInUserGrid, Url.Action(LoggedInUsersGridAction));
            loggedInUsersGrid.ShowRefreshButton = true;
            var loggedInuserList = SystemMonitorManager.GetLoggedInUser().AsQueryable();

            return loggedInUsersGrid.DataBind(loggedInuserList);

        }

        /// <summary>
        /// Outstanding Items Record Grid binding 
        /// Author : Vinod Patil
        /// Date : 3 Aug 2011
        /// </summary>
        /// <returns></returns>
        public JsonResult OutStandingItemsGridData()
        {
            var outstandingItemsGrid = new OutstandingItems(ControlIdConstants.OustandingItemsGrid, Url.Action(OutStandingItemsGridAction));
            outstandingItemsGrid.ShowRefreshButton = true;
            var outstandingItemsList = SystemMonitorManager.GetOutStandingItems().AsQueryable();
            foreach (var outstandingItemse in outstandingItemsList)
            {
                outstandingItemse.SentOnDate = CalendarManager.ConvertUtcTimeToYmq(outstandingItemse.SentOnDate);
            }

            return outstandingItemsGrid.DataBind(outstandingItemsList);

        }

        /// <summary>
        /// Author : Sachin Pharande
        /// Date : 05-04-2012
        /// Purpos: IS-WEB Response Time Statistics
        /// </summary>
        /// <returns></returns>
        public JsonResult IsWebResponseGridData()
        {
            var iSWebResponseGrid = new Iata.IS.Web.UIModel.Grid.SystemMonitor.IsWebResponse(ControlIdConstants.IsWebResponseGrid, Url.Action(IsWebResponseGridAction));
            
            iSWebResponseGrid.ShowRefreshButton = true;

            var iSWebResponseList = SystemMonitorManager.GetIsWebResponse().AsQueryable();

            return iSWebResponseGrid.DataBind(iSWebResponseList);
        }

        /// <summary>
        /// Get Alert Messages Accouncement concrete  function
        /// </summary>
        /// <param name="type"> List of AlertsMessagesAnnouncementsResultSet  object</param>
        /// <returns></returns>
        private List<AlertsMessagesAnnouncementsResultSet> GetAlertsMessagesAnnouncements(int type)
        {
            var msgList = new List<AlertsMessagesAnnouncementsResultSet>();

            string memberType = string.Empty;

            if (type == (int)MessageType.Announcement)
            {
                msgList = MsgRepository.GetAlertsMessagesAnnouncements(type, memberType, SessionUtil.UserId, DateTime.UtcNow);
            }
            else
            {
                var memberId = SessionUtil.MemberId;

                if (memberId > 0)
                {
                    var member = Member.GetMember(memberId);

                    member.IchMemberStatus = (member.IchMemberStatusId == (int)IchMemberShipStatus.Live || member.IchMemberStatusId == (int)IchMemberShipStatus.Suspended) ? true : false;

                    member.AchMemberStatus = (member.AchMemberStatusId == (int)AchMembershipStatus.Live || member.AchMemberStatusId == (int)AchMembershipStatus.Suspended) ? true : false;

                    if (member.IchMemberStatus && member.AchMemberStatus)
                    {
                        memberType = "B";
                    }
                    else if (member.IchMemberStatus)
                    {
                        memberType = "I";
                    }
                    else if (member.AchMemberStatus)
                    {
                        memberType = "A";
                    }
                    else
                    {
                        memberType = "N";
                    }

                    msgList = MsgRepository.GetAlertsMessagesAnnouncements(type, memberType, SessionUtil.UserId, DateTime.UtcNow);
                }
            }

            return msgList;
        }

        /// <summary>
        /// Get User Count by Region
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetUserCountByRegion()
        {

            var userCountByRegion = SystemMonitorManager.GetLoggedInUserByRegion();

            return Json(userCountByRegion);

        }

        #endregion

        #region "Re-Processing"

        public ActionResult Reprocessing()
        {

            var searchCriteria = new PendingInvoices();
            var gridModel = new PendingInvoiceSearchGrid(ViewDataConstants.SysMonPendingInvoices, Url.Action("PendingInvoiceSearchGridData", "ManageSystemMonitor", new
            {
                searchCriteria.BillingYear,
                searchCriteria.BillingMonth,
                searchCriteria.BillingPeriod,
                searchCriteria.BillingCategoryId,
                searchCriteria.BillingMemberId,
                searchCriteria.BilledMemberId
            }));
            ViewData[ViewDataConstants.SysMonPendingInvoices] = gridModel.Instance;

            BillingPeriod currentPeriod;
            BillingPeriod previouslyClosedPeriod;
            try
            {
                currentPeriod = _calenderManager.GetCurrentBillingPeriod();
                previouslyClosedPeriod = _calenderManager.GetLastClosedBillingPeriod();
            }
            catch (ISCalendarDataNotFoundException)
            {
                currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
                previouslyClosedPeriod = currentPeriod;
            }
            ViewData["currentYear"] = currentPeriod.Year;
            ViewData["currentMonth"] = currentPeriod.Month;
            ViewData["previouslyClosedPeriod"] = previouslyClosedPeriod.Period;
            ViewData["previouslyClosedMonth"] = previouslyClosedPeriod.Month;
            ViewData["previouslyClosedYear"] = previouslyClosedPeriod.Year;


            var systemMonitorManage = new SystemMonitorManage();
            return View(systemMonitorManage);
        }

        [HttpPost]
        public JsonResult Rearchive(int reArchiveBillingYear, int reArchiveBillingMonth, int reArchiveBillingPeriod)
        {
            var details = new UIExceptionDetail();
            try
            {
                _archivalManager.QueueInvoicesForArchive(reArchiveBillingPeriod, reArchiveBillingMonth, reArchiveBillingYear, true);

                details.IsFailed = false;
                details.Message = "successfully Queued Re-archive for given Year, Month and Period";

            }
            catch (Exception)
            {

                details.IsFailed = true;
                details.Message = "Error While Re-archive for given Year, Month and Period";

            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult RegenerateAchRecapSheet(int recapBillingYear, int recapBillingMonth, int recapBillingPeriod)
        {
            var details = new UIExceptionDetail();
            try
            {
                // Revert ACH Recap Sheet Invoice Status for Regeneration of sheet
                var billingPeriod = new BillingPeriod
                {
                    Year = recapBillingYear,
                    Month = recapBillingMonth,
                    Period = recapBillingPeriod
                };
                //SystemMonitorManager.RevertAchRecapSheetData(billingPeriod);
                var achRecapSheetGeneratorJob = new ACHRecapSheetGeneratorJob();
                int achRecapSheetRegenerationFleg = 1;
                RequestLog userRequest = new RequestLog
                {
                    BillingYear = billingPeriod.Year,
                    BillingMonth = billingPeriod.Month,
                    BillingPeriod = billingPeriod.Period,
                    ActionType = AchRecapSheet,
                    BillingCategoryId = (int)BillingCategoryType.Pax,
                    FileTypeId = (int)FileFormatType.AchRecapSheet,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    InvoiceIds = "3,4,5,6"
                };
                Guid requestId = SystemMonitorManager.AddRequest(userRequest);
                /*Below method used to regenerate ACH Recapsheet for Provided Period.
                While regenerate ACH Recapsheet only file get generated without updating any data in DB.
                Any email not sent excepting mail on exception.
                add Is_file_log entry on file generation.*/

                achRecapSheetGeneratorJob.GenerateACHRecapSheet(billingPeriod, achRecapSheetRegenerationFleg);

                details.IsFailed = false;
                details.Message = "successfully Queued Re-Generated Recap sheet for given Year, Month and Period";

            }
            catch (Exception)
            {

                details.IsFailed = true;
                details.Message = "Error While Re-Generating Recap sheet for given Year, Month and Period";

            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult RegenerateOldIdec(int oldIdecBillingYear, int oldIdecBillingMonth, int oldIdecMemberId)
        {
            var details = new UIExceptionDetail();
            try
            {
                // Revert ACH Recap Sheet Invoice Status for Regeneration of sheet
                var billingPeriod = new BillingPeriod
                {
                    Year = oldIdecBillingYear,
                    Month = oldIdecBillingMonth,
                    Period = 4
                };
                var _objInvMgr = Ioc.Resolve<IInvoiceManager>();
                var _objCGOInvMgr = Ioc.Resolve<ICargoInvoiceManager>();

                int regenerationFlag = 1;
                RequestLog userRequest = new RequestLog
                {
                    BillingYear = billingPeriod.Year,
                    BillingMonth = billingPeriod.Month,
                    BillingPeriod = billingPeriod.Period,
                    ActionType = "OLD IDEC",
                    BillingCategoryId = (int)BillingCategoryType.Pax,
                    FileTypeId = (int)FileFormatType.OldIdec,
                    MemberId = SessionUtil.MemberId,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    InvoiceIds = "4,5,6"
                };
               // SystemMonitorManager.AddRequest(userRequest);
                _logger.InfoFormat("RegenerateOldIdec Param: Billing MemberID- {0} : Period - {1}-{2}-{3} ", oldIdecMemberId, billingPeriod.Year, billingPeriod.Month, billingPeriod.Period);

             
                //-------------Cargo OldIdec--------------------------
                RequestLog userRequestCGO = new RequestLog
                {
                    BillingYear = billingPeriod.Year,
                    BillingMonth = billingPeriod.Month,
                    BillingPeriod = billingPeriod.Period,
                    ActionType = "OLD IDEC",
                    BillingCategoryId = (int)BillingCategoryType.Cgo,
                    FileTypeId = (int)FileFormatType.OldIdec,
                    MemberId = SessionUtil.MemberId,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    InvoiceIds = "4,5,6"
                };
              //  SystemMonitorManager.AddRequest(userRequest);

                EnqueueOldIdec(billingPeriod.Year, billingPeriod.Month);
                //-----------------------------------------------------

                details.IsFailed = false;
                details.Message = "successfully Queued Re-Generated Old IDEC for given Year, Month and Member";

            }
            catch (Exception)
            {

                details.IsFailed = true;
                details.Message = "Error While Re-Generated Old IDEC for given Year, Month and Member";

            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult RegenerateBvcRequest(int bvcBillingYear, int bvcBillingMonth, int bvcBillingPeriod)
        {
            var details = new UIExceptionDetail();
            try
            {
                // Revert ACH Recap Sheet Invoice Status for Regeneration of sheet
                var billingPeriod = new BillingPeriod
                {
                    Year = bvcBillingYear,
                    Month = bvcBillingMonth,
                    Period = bvcBillingPeriod
                };
                var userRequest = new RequestLog
                {
                    BillingYear = billingPeriod.Year,
                    BillingMonth = billingPeriod.Month,
                    BillingPeriod = billingPeriod.Period,
                    ActionType = BvcRequest,
                    BillingCategoryId = (int)BillingCategoryType.Pax,
                    FileTypeId = (int)FileFormatType.ValueConfirmation,

                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    Remarks = "Value Confurmation Status in(2,3,4)"
                };
                int regenerationFlag = 1;
                var _objReqVCF = Ioc.Resolve<IRequestVCF>();
                Guid requestId = SystemMonitorManager.AddRequest(userRequest);
                _objReqVCF.GenerateRequestVCFInternal(billingPeriod, requestId, regenerationFlag);
                details.IsFailed = false;
                details.Message = "Successfully Queued Re-Generated BVC Request File for given Year, Month and Period";

            }
            catch (Exception)
            {

                details.IsFailed = true;
                details.Message = "Error While Re-Generating BVC Request File for given Year, Month and Period";

            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult RegenerateOfflineCollection(int billingYear, int billingMonth, int billingPeriod, int memberId, int billingCategory, int fileType, int stages)
        {


            var details = new UIExceptionDetail();

            //SCP132419 - SRM: Duplicate OAR's generated May P3.
            BillingPeriod currentPeriod = _calenderManager.GetCurrentBillingPeriod();

            DateTime currentPeriodDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);
            DateTime passedPeriodDate = new DateTime(billingYear, billingMonth, billingPeriod);
            if (passedPeriodDate.Date >= currentPeriodDate.Date)
            {
                details.IsFailed = true;
                details.Message = "OAR Generation for current and future period is not allowed.";
                return Json(details);
            }
            

            try
            {

                var userRequest = new RequestLog
                {
                    BillingYear = billingYear,
                    BillingMonth = billingMonth,
                    BillingPeriod = billingPeriod,
                    ActionType = OfflineCollection,
                    BillingCategoryId = billingCategory,
                    FileTypeId = fileType,
                    MemberId = memberId,
                    Stage = stages,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow
                };
                SystemMonitorManager.AddRequest(userRequest);

                switch (stages)
                {
                    case 1:
                        // Billing Category
                        switch (billingCategory)
                        {
                            case (int)BillingCategoryType.Pax:
                                var invoiceList = SystemMonitorManager.GetOfflineCollectionPaxInvoices((int)InvoiceStatusType.ReadyForBilling, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);

                                foreach (var paxInvoice in invoiceList)
                                {
                                    // Enque AQ_INV_STATUS

                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(paxInvoice.Id),
                                                       (int)InvoiceStatusType.ReadyForBilling, billingYear, billingMonth, billingPeriod,
                                                       billingCategory);

                                }

                                foreach (var paxInvoice in invoiceList)
                                {
                                    // Enque AQ_INV_STATUS
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(paxInvoice.Id),
                                                       (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                       billingCategory);

                                }

                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ReadyForBilling);
                                break;

                            case (int)BillingCategoryType.Cgo:
                                var cargoInvoiceList = SystemMonitorManager.GetOfflineCollectionCargoInvoices((int)InvoiceStatusType.ReadyForBilling, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);

                                foreach (var cargoInvoice in cargoInvoiceList)
                                {
                                    // Enque AQ_INV_STATUS

                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(cargoInvoice.Id),
                                                       (int)InvoiceStatusType.ReadyForBilling, billingYear, billingMonth, billingPeriod,
                                                       billingCategory);

                                }

                                foreach (var cargoInvoice in cargoInvoiceList)
                                {
                                    // Enque AQ_INV_STATUS
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(cargoInvoice.Id),
                                                       (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                       billingCategory);

                                }

                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ReadyForBilling);
                                break;

                            case (int)BillingCategoryType.Misc:
                                var miscInvoiceList = SystemMonitorManager.GetOfflineCollectionMiscInvoices((int)InvoiceStatusType.ReadyForBilling, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);

                                foreach (var miscInvoice in miscInvoiceList)
                                {
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(miscInvoice.Id),
                                                             (int)InvoiceStatusType.ReadyForBilling, billingYear, billingMonth, billingPeriod,
                                                             billingCategory);

                                }

                                foreach (var miscInvoice in miscInvoiceList)
                                {
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(miscInvoice.Id),
                                                             (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                             billingCategory);

                                }
                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ReadyForBilling);
                                break;

                            case (int)BillingCategoryType.Uatp:
                                var uatpInvoiceList = SystemMonitorManager.GetOfflineCollectionUatpInvoices((int)InvoiceStatusType.ReadyForBilling, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);

                                foreach (var uatpInvoice in uatpInvoiceList)
                                {
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(uatpInvoice.Id),
                                                             (int)InvoiceStatusType.ReadyForBilling, billingYear, billingMonth, billingPeriod,
                                                             billingCategory);

                                }

                                foreach (var uatpInvoice in uatpInvoiceList)
                                {
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(uatpInvoice.Id),
                                                             (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                             billingCategory);

                                }
                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ReadyForBilling);
                                break;
                        }


                        break;
                    case 2:

                        // Billing Category
                        switch (billingCategory)
                        {
                            case (int)BillingCategoryType.Pax:
                                var invoiceList = SystemMonitorManager.GetOfflineCollectionPaxInvoices((int)InvoiceStatusType.ProcessingComplete, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);


                                foreach (var paxInvoice in invoiceList)
                                {
                                    // Enque AQ_INV_STATUS
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(paxInvoice.Id),
                                                              (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                              billingCategory);

                                }
                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ProcessingComplete);
                                break;

                            case (int)BillingCategoryType.Cgo:
                                var cargoInvoiceList = SystemMonitorManager.GetOfflineCollectionCargoInvoices((int)InvoiceStatusType.ProcessingComplete, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);
                                foreach (var cargoInvoice in cargoInvoiceList)
                                {
                                    // Enque AQ_INV_STATUS
                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(cargoInvoice.Id),
                                                              (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                              billingCategory);

                                }
                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ProcessingComplete);
                                break;

                            case (int)BillingCategoryType.Misc:
                                var miscInvoiceList = SystemMonitorManager.GetOfflineCollectionMiscInvoices((int)InvoiceStatusType.ProcessingComplete, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);

                                foreach (var miscInvoice in miscInvoiceList)
                                {
                                    // Enque AQ_INV_STATUS

                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(miscInvoice.Id),
                                                             (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                             billingCategory);


                                }
                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ProcessingComplete);
                                break;

                            case (int)BillingCategoryType.Uatp:
                                var uatpInvoiceList = SystemMonitorManager.GetOfflineCollectionUatpInvoices((int)InvoiceStatusType.ProcessingComplete, billingYear,
                                                                                            billingMonth, billingPeriod, memberId);

                                foreach (var uatpInvoice in uatpInvoiceList)
                                {
                                    // Enque AQ_INV_STATUS

                                    EnqueueOfflineCollection(memberId, ConvertUtil.ConvertGuidToString(uatpInvoice.Id),
                                                             (int)InvoiceStatusType.ProcessingComplete, billingYear, billingMonth, billingPeriod,
                                                             billingCategory);


                                }
                                // EnQueue Invoice Offline Collection download and Archive
                                EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, (int)InvoiceStatusType.ProcessingComplete);
                                break;
                        }

                        break;
                    case 3:
                        // EnQueue Invoice Offline Collection download and Archive
                        // Invoice Status = 1 is for only System Monitor. It would be non zero status
                        EnqueueOfflineCollectionDownload(memberId, billingYear, billingMonth, billingPeriod, billingCategory, 1);
                        break;
                }

                details.IsFailed = false;
                details.Message = "successfully Queued  Offline Collection as per given input parameters.";

            }
            catch (Exception)
            {

                details.IsFailed = true;
                details.Message = "Error While regenerating Offline Collection.";

            }
            return Json(details);
        }

        private static void EnqueueOldIdec(int billingYear, int billingMonth)
        {
            // Enque AQ_OLD_IDEC
            IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "CLOSING_PERIOD",string.Format("{0}{1}04",billingYear,billingMonth.ToString().PadLeft(2,'0'))  }
                                                                          };
            var queueHelper = new QueueHelper("AQ_OLD_IDEC");
            queueHelper.Enqueue(messages);
        }

        private static void EnqueueOfflineCollection(int memberId, string invoiceId, int invoiceStatusId, int billingYear, int billingMonth, int billingPeriod, int billingCategory)
        {
            // Enque AQ_INV_STATUS
            IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "RECORD_ID",invoiceId  },
                                                                            { "STATUS_ID", invoiceStatusId.ToString() },
                                                                            { "IS_FORM_C", "0" },
                                                                            { "BILLING_CATEGORY_ID", billingCategory.ToString() },
                                                                            { "IS_GEN_OFFL_ZIP", "0" },
                                                                            { "BILLING_PERIOD", billingPeriod.ToString() },
                                                                            { "BILLING_MONTH", billingMonth.ToString() },
                                                                            { "BILLING_YEAR", billingYear.ToString() },
                                                                            { "MEMBER_ID", memberId.ToString() },
                                                                            { "REGENERATE_FLAG", "1"}
                                                                              
                                                                          };
            var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["OfflineCollectionQueueName"].Trim());
            queueHelper.Enqueue(messages);
        }


        private void EnqueueOfflineCollectionDownload(int memberId, int billingYear, int billingMonth, int billingPeriod, int billingCategory, int invoiceStatus)
        {

            if (memberId > 0)
            {
                IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "RECORD_ID",string.Empty  },
                                                                            { "STATUS_ID", invoiceStatus.ToString() },
                                                                            { "IS_FORM_C", "0" },
                                                                            { "BILLING_CATEGORY_ID", billingCategory.ToString() },
                                                                            { "IS_GEN_OFFL_ZIP", "1" },
                                                                            { "BILLING_PERIOD", billingPeriod.ToString() },
                                                                            { "BILLING_MONTH", billingMonth.ToString() },
                                                                            { "BILLING_YEAR", billingYear.ToString() },
                                                                            { "MEMBER_ID", memberId.ToString() },
                                                                            { "REGENERATE_FLAG", "1"}
                                                                              
                                                                          };
                var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["OfflineCollectionQueueName"].Trim());
                queueHelper.Enqueue(messages);
            }
            else
            {

                var invoiceMembers = SystemMonitorManager.GetInvoiceEligibleMembers(billingYear, billingMonth, billingPeriod, billingCategory);

                var memberArray = new int[invoiceMembers.Count() * 2];

                var i = 0;

                foreach (var invoiceMember in invoiceMembers)
                {
                    memberArray[i] = invoiceMember.BillingMemberId;
                    i += 1;
                    memberArray[i] = invoiceMember.BilledMemberId;
                    i += 1;
                }

                memberArray = memberArray.Distinct().ToArray();

                foreach (var member in memberArray)
                {
                    IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "RECORD_ID",string.Empty  },
                                                                            { "STATUS_ID", invoiceStatus.ToString() },
                                                                            { "IS_FORM_C", "0" },
                                                                            { "BILLING_CATEGORY_ID", billingCategory.ToString() },
                                                                            { "IS_GEN_OFFL_ZIP", "1" },
                                                                            { "BILLING_PERIOD", billingPeriod.ToString() },
                                                                            { "BILLING_MONTH", billingMonth.ToString() },
                                                                            { "BILLING_YEAR", billingYear.ToString() },
                                                                            { "MEMBER_ID", member.ToString() },
                                                                            { "REGENERATE_FLAG", "1"}
                                                                              
                                                                          };
                    var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["OfflineCollectionQueueName"].Trim());
                    queueHelper.Enqueue(messages);
                }

            }


        }

        [HttpPost]
        public JsonResult ReCreateAndTransmitProcessedInvoiceDataCsv(int recapBillingYear, int recapBillingMonth, int recapBillingPeriod, int memberId)
        {

            var userRequest = new RequestLog
            {
                BillingYear = recapBillingYear,
                BillingMonth = recapBillingMonth,
                BillingPeriod = recapBillingPeriod,
                ActionType = ProcessedInvoiceDataCsv,
                MemberId = memberId,
                LastUpdatedBy = SessionUtil.UserId,
                LastUpdatedOn = DateTime.UtcNow
            };
            SystemMonitorManager.AddRequest(userRequest);

            var details = new UIExceptionDetail();
            try
            {

                var billingPeriod = new BillingPeriod
                {
                    Year = recapBillingYear,
                    Month = recapBillingMonth,
                    Period = recapBillingPeriod
                };

                SystemMonitorManager.InsertProcessCVSMessageInOracleQueue(memberId, billingPeriod);

                details.IsFailed = false;
                details.Message = "Successfully Queued: Create And Transmit Processed Invoice Data CSV reports.";

            }
            catch (Exception exception)
            {

                details.IsFailed = true;
                details.Message = "End with error: Create And Transmit Processed Invoice Data CSV reports. " + exception.Message;

            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult ReGenerateNilFormC(string provisinalBillingYear, int memberID)
        {


            var details = new UIExceptionDetail();
            try
            {

                var billlingYear = Convert.ToInt32(provisinalBillingYear.Substring(0, 4));
                var billlingMonth = Convert.ToInt32(provisinalBillingYear.Substring(4, 2));
                var userRequest = new RequestLog
                {
                    BillingYear = billlingYear,
                    BillingMonth = billlingMonth,
                    ActionType = NilFormC,
                    MemberId = memberID,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow
                };
                SystemMonitorManager.AddRequest(userRequest);

                // validate provisinalBillingYear input

                SystemMonitorManager.InsertGenerateNilFormCMessageInOracleQueue(memberID, provisinalBillingYear);

                details.IsFailed = false;
                details.Message = "Successfully Queued Generate Nil Form C";

            }
            catch (Exception)
            {

                details.IsFailed = true;
                details.Message = "Error While Generate Nil Form C ";

            }
            return Json(details);

        }

        public JsonResult IsValidMonthForXmlGeneration(int billingYear, int billingMonth)
        {
          BillingPeriod currentBillingPeriod = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
          var details = new UIExceptionDetail();

          DateTime currentPeriodDate = DateTime.ParseExact(
            string.Format("{0}-{1}-{2}", currentBillingPeriod.Year,
                          currentBillingPeriod.Month.ToString().PadLeft(2, '0'),
                          currentBillingPeriod.Period.ToString().PadLeft(2, '0')), "yyyy-MM-dd",
            CultureInfo.InvariantCulture);

          DateTime triggerPeriodDate = DateTime.ParseExact(
            string.Format("{0}-{1}-04", billingYear, billingMonth.ToString().PadLeft(2, '0')), "yyyy-MM-dd", CultureInfo.InvariantCulture);
          
          if (triggerPeriodDate >= currentPeriodDate)
          {
            details.IsFailed = true;
            details.Message =
              "Error-Period 4 has not closed for the Billing Year and Month provided. Please try generation/re-generation for a Billing Year and Month for which Period 4 has closed";

          }
          else
          {
            details.IsFailed = false;
            var invoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();
            long count = invoiceBaseRepository.GetCount(
              i => i.BillingYear == billingYear && i.BillingMonth == billingMonth && i.InvoiceStatusId == 6);

            details.Message = count +
                              " Invoices identified for legal XML file creation. You will be informed via email after successful file creation";
          }
          return Json(details);
        }

        public JsonResult GenerateLegalXmlfromSm(int billingYear, int billingMonth)
        {
            var details = new UIExceptionDetail();
            try
            {
                try
                {
                    var userRequest = new RequestLog
                    {
                        BillingYear = billingYear,
                        BillingMonth = billingMonth,
                        BillingPeriod =  4,
                        ActionType = InvoiceFile,
                        FileTypeId = -1,
                        InvoiceIds = null,
                        BillingCategoryId = 4,
                        LastUpdatedBy = SessionUtil.UserId,
                        LastUpdatedOn = DateTime.UtcNow,
                        FileGenerationDate = DateTime.UtcNow.ToShortDateString()
                    };

                    SystemMonitorManager.AddRequest(userRequest);

                }
                catch (Exception ex)
                {
                    details.IsFailed = true;
                    details.Message = "Error while retrieving FTP Log" + ex.Message;
                }
                var relativeTime = 2;
                var LegalXmlGenJobName = "LegalXmlGenerationJob";

                var triggerManager = Ioc.Resolve<ITriggerManager>(typeof(ITriggerManager));
                triggerManager.CreateTriggerForGivenJobAndDateTime(LegalXmlGenJobName,
                                                                   string.Format("{0}-{1:D2}-{2:D2}", billingYear, billingMonth, 4),
                                                                   DateTime.UtcNow.AddMinutes(relativeTime));
                Ioc.Release(triggerManager);

                details.IsFailed = false;
                details.Message = "Queue entry added for Legal Xml Generation. You will be informed via email after successful file creation";
            }
            catch (Exception exception)
            {
                details.IsFailed = true;
                details.Message = "Failed to queue Legal Xml Generation";
            }
            return Json(details);
        }


        [HttpPost]
        public JsonResult InvoiceFileGenerate(int memberId, int billingCategory, int fileType, int isInvoiceStatusPresented, int isInvoiceStatusReadyForBilling, int isInvoiceStatusClaimed, int isInvoiceStatusProcessingComp, int billingYear, int billingMonth, int billingPeriod, string fileGenerationDate)
        {
            var details = new UIExceptionDetail();

            var invoiceStatusIds = string.Empty;
            try
            {
                if (isInvoiceStatusReadyForBilling == 1)
                    invoiceStatusIds = "3" + ",";
                if (isInvoiceStatusClaimed == 1)
                    invoiceStatusIds += "4" + ",";
                if (isInvoiceStatusProcessingComp == 1)
                    invoiceStatusIds += "5" + ",";
                if (isInvoiceStatusPresented == 1)
                    invoiceStatusIds += "6" + ",";

                if (invoiceStatusIds.Length > 0)
                {
                    invoiceStatusIds = invoiceStatusIds.Substring(0, invoiceStatusIds.Length - 1);
                }

                var userRequest = new RequestLog
                {
                    BillingYear = billingYear,
                    BillingMonth = billingMonth,
                    BillingPeriod = billingPeriod,
                    ActionType = InvoiceFile,
                    MemberId = memberId,
                    FileTypeId = fileType,
                    InvoiceIds = invoiceStatusIds,
                    BillingCategoryId = billingCategory,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    FileGenerationDate = fileGenerationDate
                };
                SystemMonitorManager.AddRequest(userRequest);

                int isOnBehalfofInvoice = 0;
                if (fileType == 6 || fileType == 7)
                {
                    isOnBehalfofInvoice = 1;
                }

                if (memberId > 0)
                {
                    IDictionary<string, string> queueMessage = new Dictionary<string, string>
                                                     {
                                                       { "Member_Id", memberId.ToString() },
                                                       { "Is_Billing", isOnBehalfofInvoice.ToString() },
                                                       { "Year", billingYear.ToString() },
                                                       { "Month", billingMonth.ToString() },
                                                       { "Period", billingPeriod.ToString() },
                                                       { "Reprocessing", "1" },
                                                       { "InvoiceStatusIds", invoiceStatusIds },
                                                       { "BillingCategory", billingCategory.ToString() },
                                                       { "FileType", fileType.ToString() },
                                                       { "FileGenerationDate", fileGenerationDate }
                                                     };
                    var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["OutputfileGenerationQueueName"].Trim());
                    queueHelper.Enqueue(queueMessage);
                }
                else
                {
                  var memberArray = new List<int>();

                  //SCP#340872 - Issue in 'Daily IS-XML files for Receivables IS-WEB Invoices' output file
                  if (fileType == 7)
                  {
                    var members = SystemMonitorManager.GetIsWebMiscInvMemberList();
                  
                   memberArray = members.Select(mem=>mem.MemberId).Distinct().ToList();
                  }
                  else
                  {
                    var invoiceMembers = SystemMonitorManager.GetInvoiceEligibleMembers(billingYear, billingMonth,
                                                                                        billingPeriod, billingCategory);

                    memberArray.AddRange(invoiceMembers.Select(mem => mem.BillingMemberId));
                    memberArray.AddRange(invoiceMembers.Select(mem => mem.BilledMemberId));
            
                    memberArray = memberArray.Distinct().ToList();
                  }

                  foreach (var member in memberArray)
                    {
                        IDictionary<string, string> queueMessage = new Dictionary<string, string>
                                                     {
                                                       { "Member_Id", member.ToString() },
                                                       { "Is_Billing", isOnBehalfofInvoice.ToString() },
                                                       { "Year", billingYear.ToString() },
                                                       { "Month", billingMonth.ToString() },
                                                       { "Period", billingPeriod.ToString() },
                                                       { "Reprocessing", "1" },
                                                       { "InvoiceStatusIds", invoiceStatusIds },
                                                       { "BillingCategory", billingCategory.ToString() },
                                                       { "FileType", fileType.ToString() },
                                                       { "FileGenerationDate", fileGenerationDate }
                                                     };
                        var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["OutputfileGenerationQueueName"].Trim());
                        queueHelper.Enqueue(queueMessage);
                    }

                }



                details.IsFailed = false;
                details.Message = "Successfully Queued Invoice File Generation";

            }
            catch (Exception e)
            {

                details.IsFailed = true;
                details.Message = "Error While Invoice File Generation";

            }
            return Json(details);

        }


        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueMiscDailyLocOar(int memberId, string locationCode, DateTime targetDate)
        {
            var details = new UIExceptionDetail();

            /* Check to restrict future Date as Input */
            if (!IsValidInputPeriodOrDate(targetDate, false))
            {
                details.IsFailed = true;
                details.Message = "MISC Daily Bilateral Offline Archive Files Generation for future Date is not allowed.";
                return Json(details);
            }

            var result = SystemMonitorManager.EnqueueDailyLocationOutputOar(memberId, targetDate, locationCode, false);

            if (result == 1)
            {
                details.Message = "Queue entry added for MISC Daily Location Specific OAR file generation";
                details.IsFailed = false;    
            }
            else
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;    
            }

            return Json(details);
        }
        
        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueMiscDailyLocXml(int memberId, string locationCode, DateTime targetDate)
        {
            var details = new UIExceptionDetail();

            /* Check to restrict future Date as Input */
            if (!IsValidInputPeriodOrDate(targetDate, false))
            {
                details.IsFailed = true;
                details.Message = "MISC Daily Bilateral IS-XML Files Generation for future Date is not allowed.";
                return Json(details);
            }

            var result = SystemMonitorManager.EnqueueDailyLocationOutputOar(memberId, targetDate, locationCode, true);

            if (result == 1)
            {
                details.Message = "Queue entry added for MISC Daily Location Specific IS-XML file generation";
                details.IsFailed = false;
            }
            else
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;
            }

            return Json(details);
        }

        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueueMiscLocOnBehalfFile(int memberId, int billingYear, int billingMonth, int billingPeriod, string locationCode, string fileGenerationDate, int isXmlGeneration = 1, int billingCategory = 3, int isBilling = 1, int isReprocessing = 1)
        {
            var details = new UIExceptionDetail();
            /* Check to restrict current and future Period/Date as Input */
            if (!IsValidInputPeriodOrDate(new DateTime(billingYear, billingMonth, billingPeriod), true))
            {
                details.IsFailed = true;
                details.Message = "MISC Location Specific On behalf of IS-XML files Generation for current and future period is not allowed.";
                return Json(details);
            }

            try
            {
                SystemMonitorManager.EnqueueLocationOutputOar(memberId, isBilling, billingYear, billingMonth, billingPeriod, isReprocessing, (int)FileType.MiscLocationOnBehalfOfInvoice, locationCode, null, isXmlGeneration, billingCategory);

                details.Message = "Queue entry added for MISC Location Specific On Behalf of IS-XML File generation";
                details.IsFailed = false;
            }
            catch (Exception)
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;
            }
            
            return Json(details);
        }

        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueueMiscLocIsWebXmlFile(int memberId, string locationCode, string fileGenerationDate, int billingYear = 0, int billingMonth = 0, int billingPeriod = 0, int isXmlGeneration = 1, int billingCategory = 3, int isBilling = 1, int isReprocessing = 1)
        {
            var details = new UIExceptionDetail();

            DateTime inputFileGenerationDate;
            if(DateTime.TryParse(fileGenerationDate, out inputFileGenerationDate))
            {
                /* Check to restrict current and future Date as Input */
                if (!IsValidInputPeriodOrDate(inputFileGenerationDate, false))
                {
                    details.IsFailed = true;
                    details.Message = "MISC Location Specific IsWeb Xml Files Generation for future Date is not allowed.";
                    return Json(details);
                }
            }

            try
            {
                SystemMonitorManager.EnqueueLocationOutputOar(memberId, isBilling, billingYear, billingMonth, billingPeriod, isReprocessing, (int)FileType.MiscLocationIsWebInvoices, locationCode, fileGenerationDate, isXmlGeneration, billingCategory);

            details.Message = "Queue entry added for MISC Location Specific ISWEB XML File generation";
            details.IsFailed = false;
            }
            catch (Exception)
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;
            }
            return Json(details);
        }

        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueueMiscLocIsXmlFile(int memberId, int billingYear, int billingMonth, int billingPeriod, string locationCode, string fileGenerationDate, int isXmlGeneration = 1, int billingCategory = 3, int isBilling = 0, int isReprocessing = 1)
        {
            var details = new UIExceptionDetail();

            /* Check to restrict current and future Period as Input */
            if (!IsValidInputPeriodOrDate(new DateTime(billingYear, billingMonth, billingPeriod), true))
            {
                details.IsFailed = true;
                details.Message = "MISC Location Specific ISXML Outbound files Generation for current and future period is not allowed.";
                return Json(details);
            }

            try
            {
            SystemMonitorManager.EnqueueLocationOutputOar(memberId, isBilling, billingYear, billingMonth, billingPeriod, isReprocessing, (int)FileType.MiscLocationIsxml, locationCode, null, isXmlGeneration, billingCategory);

            details.Message = "Queue entry added for MISC Location Specific IS-XML Outbound Payable File generation";
            details.IsFailed = false;
            }
            catch (Exception)
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;
            }
            return Json(details);
        }

        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueueMiscLocOarRecFile(int memberId, int billingYear, int billingMonth, int billingPeriod, string locationCode, string fileGenerationDate, int isXmlGeneration = 0, int billingCategory = 3, int isBilling = 1, int isReprocessing = 1)
        {
            var details = new UIExceptionDetail();

            /* Check to restrict current and future Period as Input */
            if (!IsValidInputPeriodOrDate(new DateTime(billingYear, billingMonth, billingPeriod), true))
            {
                details.IsFailed = true;
                details.Message = "MISC Location Specific Offline Archive files Generation for current and future period is not allowed.";
                return Json(details);
            }

            try
            {
            SystemMonitorManager.EnqueueLocationOutputOar(memberId, isBilling, billingYear, billingMonth, billingPeriod, isReprocessing, (int)FileFormatType.OfflineArchiveMiscLocSpec, locationCode, null, isXmlGeneration, billingCategory);

            details.Message = "Queue entry added for MISC Location Specific OAR Receivable File generation";
            details.IsFailed = false;
            }
            catch (Exception)
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;
            }
            return Json(details);
        }

        //CMP#622: MISC Outputs Split as per Location IDs
        [HttpPost]
        public JsonResult EnqueueMiscLocOarPayFile(int memberId, int billingYear, int billingMonth, int billingPeriod, string locationCode, string fileGenerationDate, int isXmlGeneration = 0, int billingCategory = 3, int isBilling = 0, int isReprocessing = 1)
        {
            var details = new UIExceptionDetail();

            /* Check to restrict current and future Period as Input */
            if (!IsValidInputPeriodOrDate(new DateTime(billingYear, billingMonth, billingPeriod), true))
            {
                details.IsFailed = true;
                details.Message = "MISC Location Specific Offline Archive files Generation for current and future period is not allowed.";
                return Json(details);
            }

            try
            {
            SystemMonitorManager.EnqueueLocationOutputOar(memberId, isBilling, billingYear, billingMonth, billingPeriod, isReprocessing, (int)FileFormatType.OfflineArchiveMiscLocSpec, locationCode, null, isXmlGeneration, billingCategory);

            details.Message = "Queue entry added for MISC Location Specific OAR Payable File generation";
            details.IsFailed = false;
            }
            catch (Exception)
            {
                details.Message = "Error While Adding Queue Entry";
                details.IsFailed = false;
            }
            return Json(details);
        }

        [Authorize]
        public JsonResult PendingInvoiceSearchGridData(int billingYear, int billingMonth, int billingPeriod, int billingCategoryId, int billingMemberId, int billedMemberId)
        {
            var searchCriteria = new PendingInvoices
            {
                BillingYear = billingYear,
                BillingMonth = billingMonth,
                BillingPeriod = billingPeriod,
                BillingCategoryId = billingCategoryId,
                BillingMemberId = billingMemberId,
                BilledMemberId = billedMemberId
            };

            var details = new UIExceptionDetail();
            var gridModel = new PendingInvoiceSearchGrid(ViewDataConstants.SysMonPendingInvoices, Url.Action("PendingInvoiceSearchGridData", "ManageSystemMonitor", new { searchCriteria }));
            var pendingInvoiceses = SystemMonitorManager.GetPendingInvoices(searchCriteria);
            try
            {
                return gridModel.DataBind(pendingInvoiceses.AsQueryable());

            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error While Search Pending Invoices.";
                return null;
            }
        }


        public JsonResult UpdatePendingInvoices(string selectedInvoiceIDs, int billingYear, int billingMonth, int periodNumber)
        {
            var details = new UIExceptionDetail();
            try
            {
                // Check current Period and Void period to avoid updation 

                var currentPeriod = CalendatManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);


                if (currentPeriod.Year == billingYear && currentPeriod.Month == billingMonth && currentPeriod.Period == periodNumber)
                {
                    details.IsFailed = true;
                    details.Message = "Can Not Update Invoices Of Currently Open Billing Period.";
                }
                else
                {
                    var userRequest = new RequestLog
                    {
                        ActionType = UpdatePendingInvoice,
                        LastUpdatedBy = SessionUtil.UserId,
                        LastUpdatedOn = DateTime.UtcNow
                    };
                    SystemMonitorManager.AddRequest(userRequest);

                    if (!string.IsNullOrEmpty(selectedInvoiceIDs))
                    {
                        string[] invoiceIdsArray = selectedInvoiceIDs.Split(',');

                        foreach (var invoiceId in invoiceIdsArray)
                        {
                            var invoiceGuid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(invoiceId));
                            SystemMonitorManager.UpdatePendingInvoiceStatus(invoiceGuid);
                        }

                        details.IsFailed = false;
                        details.Message = "Selected Invoice Status Has Been Set to Presented";
                    }
                }

            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error While updataing Invoice Status.";
            }
            return Json(details);
        }


        /// <summary>
        /// This function is used to retry all failed en-queue message in ICP consumer queue.
        /// CMP#665: User Related Enhancements-FRS-v1.2 [Sec 2.2: SIS Ops Reprocessing Tab in the System Monitor] 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RetryFailedEnqueueMessageForIcp()
        {
            var details = new UIExceptionDetail();

            try
            {
                //CMP #665: User Related Enhancements-FRS-v1.2 [sec 2.4.3	Web Service Call to ICP].
                //Request Type "R": Retry Failed message to queue, ActionType 1: Create User, ActionType 2: Enable User, ActionType 3: Disable User  
                new Business.Common.ReferenceManager().EnqueueMessageInIcpLogConsumer(string.Empty, "R", 1);
                details.IsFailed = false;
                details.Message = "Successfully Queued All Failed Users to ICP.";
            }
            catch 
            {
            }
            return Json(details);
        }


        #endregion

        #region "Graphs"

        public ActionResult Graphs()
        {
            return View();
        }

        public FileResult CreateProcessedFilesChart()
        {
            var peoples = SystemMonitorManager.GetProcessedDataFiles();
            var chart = new Chart
            {
                Width = 700,
                Height = 300,
                BackColor = Color.FromArgb(211, 223, 240),
                BorderlineDashStyle = ChartDashStyle.Solid,
                BackSecondaryColor = Color.White,
                BackGradientStyle = GradientStyle.TopBottom,
                BorderlineWidth = 1,
                Palette = ChartColorPalette.BrightPastel,
                BorderlineColor = Color.FromArgb(26, 59, 105),
                RenderType = RenderType.BinaryStreaming,
                BorderSkin = { SkinStyle = BorderSkinStyle.Emboss },
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.Normal
            };
            chart.Titles.Add(CreateTitle());
            chart.Legends.Add(CreateLegend("Processed Failed", Color.Red));
            chart.Legends.Add(CreateLegend("Processed Successfully", Color.Green));
            chart.Series.Add(CreateSeriesForProcessedSuccessfully(peoples, SeriesChartType.Column));
            chart.Series.Add(CreateSeriesForProcessedFailed(peoples, SeriesChartType.Column));
            chart.ChartAreas.Add(CreateChartArea());


            var ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }

        public FileResult CreateProcessedSupprotingChart()
        {
            var peoples = SystemMonitorManager.GetProcessedSupportingDoc();
            var chart = new Chart
            {
                Width = 700,
                Height = 300,
                BackColor = Color.FromArgb(211, 223, 240),
                BorderlineDashStyle = ChartDashStyle.Solid,
                BackSecondaryColor = Color.White,
                BackGradientStyle = GradientStyle.TopBottom,
                BorderlineWidth = 1,
                Palette = ChartColorPalette.BrightPastel,
                BorderlineColor = Color.FromArgb(26, 59, 105),
                RenderType = RenderType.BinaryStreaming,
                BorderSkin = { SkinStyle = BorderSkinStyle.Emboss },
                AntiAliasing = AntiAliasingStyles.All,
                TextAntiAliasingQuality = TextAntiAliasingQuality.Normal
            };
            chart.Titles.Add(CreateSupportingDocTitle());
            chart.Legends.Add(CreateLegend("No. Of Supporting Document Processed", Color.Green));
            chart.Series.Add(CreateSeriesForProcessedSupportingDoc(peoples, SeriesChartType.Column));
            chart.ChartAreas.Add(CreateChartSupprotingDocArea());
            chart.DataBind();
            var ms = new MemoryStream();
            chart.SaveImage(ms);
            return File(ms.GetBuffer(), @"image/png");
        }

        [NonAction]
        public Title CreateTitle()
        {
            Title title = new Title();
            title.Text = "Processed Data Import Files Over Last 24 Hours";
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            title.ShadowOffset = 3;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }

        [NonAction]
        public Title CreateSupportingDocTitle()
        {
            Title title = new Title();
            title.Text = "No. Of Supporting Document Processed";
            title.ShadowColor = Color.FromArgb(32, 0, 0, 0);
            title.Font = new Font("Trebuchet MS", 14F, FontStyle.Bold);
            title.ShadowOffset = 3;
            title.ForeColor = Color.FromArgb(26, 59, 105);

            return title;
        }

        [NonAction]
        public Legend CreateLegend(string legentName, Color colorName)
        {
            Legend legend = new Legend();
            legend.Name = legentName;
            legend.Docking = Docking.Bottom;
            legend.BackColor = colorName;
            legend.Alignment = StringAlignment.Center;
            legend.BackColor = Color.Transparent;
            legend.Font = new Font(new FontFamily("Trebuchet MS"), 9);
            legend.LegendStyle = LegendStyle.Row;
            legend.Enabled = true;
            legend.IsTextAutoFit = true;
            return legend;
        }

        [NonAction]
        public Series CreateSeriesForProcessedSuccessfully(IList<ProceesedFiles> results, SeriesChartType chartType)
        {
            var seriesDetail = new Series
            {

                Name = "Processed Successfully",
                IsValueShownAsLabel = true,
                Color = Color.Green,
                ChartType = chartType,
                BorderWidth = 2
            };
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            foreach (ProceesedFiles result in results)
            {
                point = new DataPoint
                {

                    AxisLabel = result.Hours,
                    YValues = new double[] { double.Parse(result.Passed.ToString()) }


                };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = "Result Chart";

            return seriesDetail;
        }

        [NonAction]
        public Series CreateSeriesForProcessedFailed(IList<ProceesedFiles> results, SeriesChartType chartType)
        {
            var seriesDetail = new Series
            {

                Name = "Processing Failed",
                IsValueShownAsLabel = true,
                Color = Color.Red,
                ChartType = chartType,
                BorderWidth = 2
            };
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            DataPoint point;

            foreach (ProceesedFiles result in results)
            {
                point = new DataPoint
                {

                    AxisLabel = result.Hours,
                    YValues = new double[] { double.Parse(result.Failed.ToString()) }


                };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = "Result Chart";

            return seriesDetail;
        }

        [NonAction]
        public Series CreateSeriesForProcessedSupportingDoc(IList<ProceesedFiles> results, SeriesChartType chartType)
        {
            var seriesDetail = new Series
            {

                Name = "No. Of Supporting Document Processed",
                IsValueShownAsLabel = true,
                Color = Color.FromArgb(198, 99, 99),
                ChartType = chartType,
                BorderWidth = 2

            };
            seriesDetail["DrawingStyle"] = "Cylinder";
            seriesDetail["PieDrawingStyle"] = "SoftEdge";
            seriesDetail["PixelPointWidth"] = "30";
            DataPoint point;

            foreach (ProceesedFiles result in results)
            {
                point = new DataPoint
                {

                    AxisLabel = result.Hours,
                    YValues = new double[] { double.Parse(result.Failed.ToString()) }
                };
                seriesDetail.Points.Add(point);
            }
            seriesDetail.ChartArea = "Supporting Chart";

            return seriesDetail;
        }

        [NonAction]
        public ChartArea CreateChartArea()
        {
            var chartArea = new ChartArea
            {
                Name = "Result Chart",
                BackColor = Color.Transparent,
                AxisX = { IsLabelAutoFit = false },
                AxisY = { IsLabelAutoFit = false }
            };
            chartArea.AxisY.Title = "Files Processed";
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.Interval = 5;
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.Title = "Last 24 hours";

            return chartArea;
        }

        [NonAction]
        public ChartArea CreateChartSupprotingDocArea()
        {
            var chartArea = new ChartArea
            {
                Name = "Supporting Chart",
                BackColor = Color.Transparent,
                AxisX = { IsLabelAutoFit = false },
                AxisY = { IsLabelAutoFit = false }
            };
            chartArea.AxisY.Title = "Files Processed";
            chartArea.AxisX.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.LabelStyle.Font = new Font("Verdana,Arial,Helvetica,sans-serif", 8F, FontStyle.Regular);
            chartArea.AxisY.Interval = 5;
            chartArea.AxisY.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisY.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.MajorGrid.LineColor = Color.FromArgb(64, 64, 64, 64);
            chartArea.AxisX.Interval = 1;
            chartArea.AxisX.Title = "Last 24 hours";

            return chartArea;
        }

        #endregion

        #region "Re-Sending Files"
        public ActionResult ResendFile()
        {
            OutputFile searchCriteria = new OutputFile();
            searchCriteria.FileSubmissionFrom = DateTime.UtcNow;
            searchCriteria.FileSubmissionTo = DateTime.UtcNow;
            var gridModel = new OutpuFileSearchGrid("SearchOutpuFileGrid", Url.Action("ResendFileSearchGridData", "ManageSystemMonitor", new
            {
                searchCriteria.ProvisionalBillingYear,
                searchCriteria.ProvisionalBillingMonth,
                searchCriteria.ProvisionalBillingPeriod,
                searchCriteria.FileMemberId,
                searchCriteria.FileStatusId,
                searchCriteria.FileFormatId,
                searchCriteria.FileName,
                searchCriteria.MiscLocationCode,
                searchCriteria.FileSubmissionFrom,
                searchCriteria.FileSubmissionTo
            }));
            ViewData["SearchResultOutpuFileGrid"] = gridModel.Instance;
            return View(searchCriteria);
        }


        [Authorize]
        public JsonResult ResendFileSearchGridData(
            int provisionalBillingYear,
            int provisionalBillingMonth,
            int provisionalBillingPeriod,
            int fileMemberId,
            int fileStatusId,
            int fileFormatId,
            string fileName,
            string miscLocationCode,
            DateTime? fileSubmissionFrom,
            DateTime? fileSubmissionTo)
        {
            var searchCriteria = new OutputFile();
            searchCriteria.ProvisionalBillingYear = provisionalBillingYear;
            searchCriteria.ProvisionalBillingMonth = provisionalBillingMonth;
            searchCriteria.ProvisionalBillingPeriod = provisionalBillingPeriod;
            searchCriteria.FileMemberId = fileMemberId;
            searchCriteria.FileStatusId = fileStatusId;
            searchCriteria.FileFormatId = fileFormatId;
            searchCriteria.FileName = fileName;
            //CMP#622: MISC Outputs Split as per Location IDs
            searchCriteria.MiscLocationCode = miscLocationCode;
            searchCriteria.FileSubmissionFrom = fileSubmissionFrom;
            searchCriteria.FileSubmissionTo = fileSubmissionTo;

            var details = new UIExceptionDetail();
            var gridModel = new OutpuFileSearchGrid("SearchOutpuFileGrid", Url.Action("ResendFileSearchGridData", "ManageSystemMonitor", new { searchCriteria }));
            var fileList = SystemMonitorManager.GetOutputFilesToResend(searchCriteria);

            foreach (var outputFile in fileList)
            {
                outputFile.FileDate = CalendarManager.ConvertUtcTimeToYmq(outputFile.FileDate);
            }

            try
            {
                return gridModel.DataBind(fileList.AsQueryable());
            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error While Search Files.";
                return null;
            }
        }

        public JsonResult ResendSelectedFiles(string selectedFileIDs)
        {
            var details = new UIExceptionDetail();
            try
            {
                RequestLog userRequest = new RequestLog
                {
                    ActionType = ResendOutputFile,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow
                };
                SystemMonitorManager.AddRequest(userRequest);
                if (!string.IsNullOrEmpty(selectedFileIDs))
                {
                    string[] fileIds = selectedFileIDs.Split(',');
                    var inputFiles = SystemMonitorManager.GetIsInputFilesByIds(fileIds);
                    int resendFlag = 1;
                    //Resnd the Selected files.
                    //Update status of the file to 9 
                    foreach (var isInputFile in inputFiles)
                    {
                        isInputFile.FileStatusId = (int)FileStatusType.AvailableForDownload;
                    }
                    UnitOfWork.CommitDefault();
                    details.IsFailed = false;
                    details.Message = "Your request has been queued.Files will be sent to recipient shortly.";
                }
            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error While Resend Selected Files." + ex.Message;
            }
            return Json(details);
        }


        public JsonResult GetFtpFileLog(string fileId)
        {
            var details = new UIExceptionDetail();
            try
            {
                var guid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(fileId));
                var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                var ftpLog = referenceManager.GetFTPLog(guid);

                if (ftpLog != null)
                {
                    details.Message = ftpLog.LogText;
                }
                else
                {
                    details.Message = "FTP Log not found in the Database";
                }

                details.IsFailed = false;

            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error while retrieving FTP Log" + ex.Message;
            }
            return Json(details);
        }
        public JsonResult EnqueUatpAtcanDetails(int memberId, int billingMonth, int billingYear, int billingPeriod, int billingTypeId = 1)
        {
            var details = new UIExceptionDetail();
            try
            {
                var userRequest = new RequestLog
                {
                    BillingYear = billingYear,
                    BillingMonth = billingMonth,
                    BillingPeriod = billingPeriod,
                    MemberId = memberId,
                    BillingTypeId = 0,
                    ActionType = InvoiceFile,
                    FileTypeId = 9,
                    InvoiceIds = null,
                    BillingCategoryId = 4,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    FileGenerationDate = DateTime.UtcNow.ToShortDateString()
                };

                SystemMonitorManager.AddRequest(userRequest);

            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error while retrieving FTP Log" + ex.Message;
            }
            // Enque AQ_OUTPUT_FILE_GEN
            // Enque AQ_OUTPUT_FILE_GEN
            int isBilling = 0;
            if (billingTypeId == 1)
                isBilling = 2;
            else if (billingTypeId == 2)
                isBilling = 1;
            if (memberId > 0)
            {
              IDictionary<string, string> messages = new Dictionary<string, string>
                                                       {
                                                         {"Member_Id", memberId.ToString()},
                                                         {"Year", billingYear.ToString()},
                                                         {"Month", billingMonth.ToString()},
                                                         {"Period", billingPeriod.ToString()},
                                                         {"Is_Billing", isBilling.ToString()},
                                                         {"Reprocessing", "1"},
                                                         {"BillingCategory", "4"},
                                                         {"FileType", "9"},
                                                       };
              var queueHelper = new QueueHelper("AQ_OUTPUT_FILE_GEN");
              queueHelper.Enqueue(messages);
            }
            else
            {
              var invoiceMembers = SystemMonitorManager.GetInvoiceEligibleMembers(billingYear, billingMonth, billingPeriod, (int)BillingCategoryType.Uatp);
              
              var memberArray = new int[invoiceMembers.Count() * 2];

                var i = 0;

                foreach (var invoiceMember in invoiceMembers)
                {
                    memberArray[i] = invoiceMember.BillingMemberId;
                    i += 1;
                    memberArray[i] = invoiceMember.BilledMemberId;
                    i += 1;
                }

                memberArray = memberArray.Distinct().ToArray();

                foreach (var member in memberArray)
                {
                  IDictionary<string, string> messages = new Dictionary<string, string>
                                                       {
                                                         {"Member_Id", member.ToString()},
                                                         {"Year", billingYear.ToString()},
                                                         {"Month", billingMonth.ToString()},
                                                         {"Period", billingPeriod.ToString()},
                                                         {"Is_Billing", isBilling.ToString()},
                                                         {"Reprocessing", "1"},
                                                         {"BillingCategory", "4"},
                                                         {"FileType", "9"},
                                                       };
                  var queueHelper = new QueueHelper("AQ_OUTPUT_FILE_GEN");
                  queueHelper.Enqueue(messages);
                }
            }

            details.IsFailed = false;
            details.Message = "UATP ATCAN Statement Generated";

            return Json(details);
        }


        [HttpPost]
        public JsonResult EnqueAutoBillingFileGeneration(int memberId, int billingMonth, int billingYear, int billingPeriod, string fileType, int couponStatus)
        {
            var details = new UIExceptionDetail();
            try
            {
                var userRequest = new RequestLog
                {
                    BillingYear = billingYear,
                    BillingMonth = billingMonth,
                    BillingPeriod = billingPeriod,
                    MemberId = memberId,
                    ActionType = InvoiceFile,
                    FileTypeId = -1,
                    InvoiceIds = null,
                    BillingCategoryId = 4,
                    LastUpdatedBy = SessionUtil.UserId,
                    LastUpdatedOn = DateTime.UtcNow,
                    FileGenerationDate = DateTime.UtcNow.ToShortDateString()
                };

                SystemMonitorManager.AddRequest(userRequest);

            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Error while retrieving FTP Log" + ex.Message;
            }



            if (fileType.Equals("RevenueRecognitionFile"))
            {

                // Enque AQ_DRR_REPORT
                IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "MEMBER_ID",memberId.ToString()  },
                                                                            { "YEAR", billingYear.ToString() },
                                                                            { "MONTH", billingMonth.ToString() },
                                                                            { "PERIOD", billingPeriod.ToString() },
                                                                            { "FILE_FORMAT_TYPE", ((int)FileFormatType.PaxAutoBillingRevenueRecognition).ToString() },
                                                                            { "REGENERATE_FLAG", "1" },
                                                                            { "COUPON_INCLUSION_STATUS", couponStatus.ToString() }
                                                                            };

                var queueHelper = new QueueHelper("AQ_DRR_REPORT");
                queueHelper.Enqueue(messages);

                details.Message = "Queue entry added for Daily Revenue Recognition report generation";
            }
            else if (fileType.Equals("ValueRequestIrregularityReport"))
            {
                // Enque AQ_DRR_REPORT
                IDictionary<string, string> messages = new Dictionary<string, string> {
                                                                            { "MEMBER_ID",memberId.ToString()  },
                                                                            { "YEAR", billingYear.ToString() },
                                                                            { "MONTH", billingMonth.ToString() },
                                                                            { "PERIOD", billingPeriod.ToString() },
                                                                            { "FILE_FORMAT_TYPE", ((int)FileFormatType.DailyAutoBillingIrregularityReport).ToString() },
                                                                            { "REGENERATE_FLAG", "1" },
                                                                            { "COUPON_INCLUSION_STATUS", couponStatus.ToString() }
                                                                            };
                var queueHelper = new QueueHelper("AQ_DRR_REPORT");
                queueHelper.Enqueue(messages);

                details.Message = "Queue entry added for Daily Value Request Irregularity report generation";
            }
            else if (fileType.Equals("InvoicePostingFile"))
            {
                // Enque AQ_OUTPUT_FILE_GEN
                IDictionary<string, string> messages = new Dictionary<string, string>
                                                       {
                                                           {"Member_Id", memberId.ToString()},
                                                           {"Year", billingYear.ToString()},
                                                           {"Month", billingMonth.ToString()},
                                                           {"FileType", ((int) FileFormatType.PaxAutoBillingInvoicePosting).ToString()},
                                                           {"Period", billingPeriod.ToString()},
                                                           {"Reprocessing", "0"},
                                                           {"InvoiceStatusIds", ((int)InvoiceStatusType.Presented).ToString()},
                                                           {"BillingCategory", ((int)BillingCategoryType.Pax).ToString()},
                                                           {"Is_Billing", "1"}

                                                       };
                var queueHelper = new QueueHelper("AQ_OUTPUT_FILE_GEN");
                queueHelper.Enqueue(messages);

                details.Message = "Queue entry added for Weekly Invoice Posting file generation";
            }

            details.IsFailed = false;
            return Json(details);
        }

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        [HttpPost]
        public JsonResult EnqueMiscDailyBilateralIsXml(int memberId, DateTime targetDate)
        {
            var details = new UIExceptionDetail();

            /* Check to restrict current and future Date as Input */
            if(!IsValidInputPeriodOrDate(targetDate, false))
            {
                details.IsFailed = true;
                details.Message = "MISC Daily Bilateral IS-XML Files Generation for future date is not allowed.";
                return Json(details);
            }

            //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
            SystemMonitorManager.EnqueueDailyOutput(memberId, targetDate, true);

            details.Message = "Queue entry added for MISC Daily Bilateral IS-XML file generation";
            details.IsFailed = false;
            return Json(details);
        }

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        [HttpPost]
        public JsonResult EnqueMiscDailyBilateralOar(int memberId, DateTime targetDate)
        {
            var details = new UIExceptionDetail();
            /* Check to restrict current and future Period/Date as Input */
            if (!IsValidInputPeriodOrDate(targetDate, false))
            {
                details.IsFailed = true;
                details.Message = "MISC Daily Bilateral Offline Archive Files Generation for future date is not allowed.";
                return Json(details);
            }

            details.Message = "Queue entry added for MISC Daily Bilateral OAR file generation";

            //CMP-619-652-682-Changes in MISC Daily Bilateral Delivery-FRS-v0.3 [Remove Nill file parameters]
            SystemMonitorManager.EnqueueDailyOutput(memberId, targetDate, false);

            details.IsFailed = false;
            return Json(details);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DownloadFile(object FileToDownload)
        {
            try
            {
                if (Util.SessionUtil.UserId == 0)
                {
                    return RedirectToAction("LogOn", "Account", new { area = "" });
                }
                else
                {
                    string[] FileLocationArray = (string[])FileToDownload;
                    string fileId = null;
                    var FileLocation = string.Empty;

                    if (FileLocationArray.Count() > 0)
                    {
                        fileId = FileLocationArray[0];
                    }

                    if (string.IsNullOrEmpty(fileId))
                    {
                        TempData["DownloadFileError"] = AppSettings.DownloadFileError;
                        return RedirectToAction("Manage");
                    }

                    var guid = ConvertUtil.ConvertStringtoGuid(ConvertNetGuidToOracleGuid(fileId));

                    var isFile = IsInputFileRepository.Get(f => f.Id == guid).FirstOrDefault();

                    FileLocation = isFile.FileLocation + "\\" + isFile.FileName;
                    //CMP559 : Add Submission Method Column to Processing Dashboard
                    if (!isFile.IsPurged && !isFile.IsIncoming && isFile.FileFormat != FileFormatType.SupportingDoc)
                        FileLocation = FileIo.GetInputFileLocation(isFile);

                    // SCP 62418 : Files Purged error - Changed the message and added isPurged check.
                    if (!System.IO.File.Exists(FileLocation) && (isFile.FileFormat != FileFormatType.SupportingDoc) && !(isFile.IsPurged))
                    {
                        _logger.Info("File Not Exists On Specified location" + FileLocation);
                        TempData["DownloadFileError"] = "The File has been deleted";
                        return RedirectToAction("Manage");
                    }
                    else if (isFile.FileFormat == FileFormatType.SupportingDoc)
                    {
                       //CMP559 : Add Submission Method Column to Processing Dashboard
                        FileLocation = FileIo.GetInputFileLocation(isFile);
                        // SCP 62418 : Files Purged error - Changed the message  
                      if (!System.IO.File.Exists(FileLocation))
                        {
                            _logger.Info("File Not Exists On Specified location" + FileLocation);
                            TempData["DownloadFileError"] = "The File has been deleted";
                            return RedirectToAction("Manage");
                        }
                    }

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

                    //var tempFolderPath = @"C:\TempDownloadFolder";
                    //// Create Temp Folder in C :\ to copy network files 
                    //if (!Directory.Exists(tempFolderPath))
                    //{
                    //    Directory.CreateDirectory(tempFolderPath);
                    //}

                    //// Delete Oldest files from Temp Folder
                    //DeleteOldestFilesFromTempFolder(tempFolderPath);

                    ////Copy Network file into Temp Folder for Download
                    //System.IO.File.Copy(FileLocation, tempFolderPath + @"\" + System.IO.Path.GetFileName(FileLocation), true);

                    ////FileLocation = System.IO.Path.GetFullPath(FileLocation);
                    //FileLocation = Path.GetFullPath(tempFolderPath + @"\" + System.IO.Path.GetFileName(FileLocation));

                    return File(FileLocation, contentType, System.IO.Path.GetFileName(FileLocation));

                }
            }
            catch (Exception exception)
            {
                _logger.Error("Unexpected Error Has Occurred", exception);
                TempData["DownloadFileError"] = AppSettings.DownloadFileError;
                return RedirectToAction("Manage");
            }


        }

        //private void DeleteOldestFilesFromTempFolder(string path)
        //{
        //    var tempDownloadDir = new DirectoryInfo(path);
        //    foreach (FileInfo file in tempDownloadDir.GetFiles())
        //    {
        //        if (file.CreationTimeUtc <= DateTime.UtcNow.AddDays(-1))
        //        {
        //            file.Delete();
        //        }
        //    }

        //}

        /// <summary>
        /// Converts string(i.e. FileId) to GUID and converts it to ByteArray.
        /// Iterates through ByteArray, convert it to Hexadecimal equivalent and appends each hex values to 
        /// create a string(i.e. FileId in Oracle GUID format)
        /// </summary>
        private string ConvertNetGuidToOracleGuid(string invoiceIds)
        {
            // Declare variable of type StringBuilder
            var oracleGuidBuilder = new StringBuilder();

            // Iterate through file Id's and split on ','
            foreach (var netGuid in invoiceIds.Split(','))
            {
                // Call "ConvertNetGuidToOracleGuid()" method which will return string 
                oracleGuidBuilder.Append(ToOracleGuid(netGuid));
                oracleGuidBuilder.Append(",");
            }// end foreach()

            // Declare string variable
            string oracleGuid;
            // Set string variable to Guid in Oracle format
            oracleGuid = oracleGuidBuilder.ToString();
            // Trim last ',' of comma separated GUID string
            oracleGuid = oracleGuid.TrimEnd(',');

            return oracleGuid;

        }// end ConvertNetGuidToOracleGuid()

        private string ToOracleGuid(string oracleGuid)
        {
            // Convert string to Guid
            Guid netGuid = new Guid(oracleGuid);
            // Convert Guid to ByteArray
            byte[] guidBytes = netGuid.ToByteArray();
            // Create StringBuilder
            var oracleGuidBuilder = new StringBuilder();
            // Iterate through ByteArray, get Hex equivalent of each byte and append it to string
            foreach (var singleByte in guidBytes)
            {
                // Get Hex equivalent of each byte
                var hexEqui = singleByte.ToString("X");
                // Append each Hex equivalent to construct Guid.(Pad '0' to single byte)
                oracleGuidBuilder.Append(hexEqui.ToString().PadLeft(2, '0'));
            }// end foreach()

            // Return Guid string in Oracle format
            return oracleGuidBuilder.ToString();
        }
        #endregion

        #region Reset Validation Caches Used for Pasring And Validation

        [HttpPost]
        public JsonResult ResetValidationCaches()
        {
            var details = new UIExceptionDetail();
            try
            {
                _logger.Info("Flushing all keys present in MemCache.");

                const string keyListKey = "KeyList";
                var cacheManager = Ioc.Resolve<ICacheManager>();

                var keysList = cacheManager.Get(keyListKey) as StringCollection;

                if (keysList != null)
                {
                    keysList.Remove(keyListKey);
                }

                _logger.Info("Updating values in ADMIN_SIS_EXT_PARAMETER table.");
                var serverInstancestoReset = ConfigurationManager.AppSettings["ValidationCacheResetOnServiceInstances"];
                string[] strSeperator = { "," };
                if (!string.IsNullOrWhiteSpace(serverInstancestoReset))
                {
                    foreach (string serverInstance in serverInstancestoReset.Split(strSeperator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        _logger.InfoFormat("Set value to '1' for Parameter [{0}].", serverInstance);
                        // ReSharper disable AccessToModifiedClosure
                        var service = SisExtendedParameterRepository.First(sp => sp.ParameterName == serverInstance);
                        // ReSharper restore AccessToModifiedClosure
                        if (service != null)
                        {
                            service.ParameterValue = "1";
                        }
                    }

                    UnitOfWork.CommitDefault();
                }

                details.IsFailed = false;
                details.Message = "Successfully reloaded master tables used by parsing and validation process.";
            }
            catch (Exception ex)
            {
                details.IsFailed = true;
                details.Message = "Failed to reload master tables used by parsing and validation process.";
            }

            return Json(details);
        }

        #endregion


        #region Restricting Future Date/Period for Re-Submission
        
        /// <summary>
        /// A function to restrict future Target Date / Current and Future Period input Values 
        /// while reprocessing. This check is added specially for reprocessing functionalities 
        /// in which nil file genration is possible.
        /// </summary>
        /// <param name="inputvalue"></param>
        /// <param name="isPeriodValue"></param>
        /// <returns></returns>
        private bool IsValidInputPeriodOrDate(DateTime inputvalue, bool isPeriodValue)
        {
            try
            {
                /* Input is Period */
                if(isPeriodValue)
                {
                    /* Get current billing Period from Calendar */
                    BillingPeriod currentPeriod = _calenderManager.GetCurrentBillingPeriod();
                    DateTime currentPeriodDate = new DateTime(currentPeriod.Year, currentPeriod.Month, currentPeriod.Period);

                    /* Compare input value with current billing period Date */
                    if (inputvalue.Date >= currentPeriodDate.Date)
                    {
                        /* Input invalid - Either current or Future */
                        return false;
                    }
                    else
                    {
                        /* Input is Valid */
                        return true;
                    }
                }
                /* Input is Date */
                else
                {
                    DateTime currentDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
                    /* Compare input value with current billing period Date */
                    if (inputvalue.Date > currentDate.Date)
                    {
                        /* Input invalid - Future Date */
                        return false;
                    }
                    else
                    {
                        /* Input is Valid - Past or Current Date */
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                /* Problem - Log it and treat input as invalid */
                return false;
            }

            /* Default case - Invalid input */
            return false;
        }

        #endregion

    }
}
