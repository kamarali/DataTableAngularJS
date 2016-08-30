using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Reports;
using Iata.IS.Web.UIModel.Grid.Reports;
using Iata.IS.Business.Reports;
using System.Web.Script.Serialization;
using Iata.IS.Web.Util;
using iPayables.UserManagement;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Calendar;
using Iata.IS.Web.Util.Filters;
using System.IO;
using Iata.IS.Data;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Business.FileCore;

namespace Iata.IS.Web.Areas.Reports.Controllers
 {
    public class ProcessingDashboardController : ISController
    {
        // Readonly field of type IProcessingDashboardManager
        private readonly IProcessingDashboardManager _dashboardManager;
        private readonly ICalendarManager _calenderManager;
        private readonly IMemberManager _memberManager;
        private readonly IUserManagement _iUserManagement;
        //CMP559 : Add Submission Method Column to Processing Dashboard
        public IRepository<IsInputFile> IsInputFileRepository { get; set; }

        private readonly IReferenceManager _referenceManager;
        /// <summary>
        /// Constructor for ProcessingDashboardController
        /// </summary>
        /// <param name="dashboardManager"></param>
        public ProcessingDashboardController(IProcessingDashboardManager dashboardManager, ICalendarManager calenderManager, IMemberManager memberManager, IUserManagement iUserManagement, IReferenceManager referenceManager)
        {
            _dashboardManager = dashboardManager;
            _calenderManager = calenderManager;
            _memberManager = memberManager;
            _iUserManagement = iUserManagement;
            _referenceManager = referenceManager;
        }

        /// <summary>
        /// Following action is used to Populate InvoiceList and FileList grid depending on search criteria selected
        /// by user.
        /// </summary>
        /// <param name="billingPeriod">Billing period i.e. current or previous</param>
        /// <param name="isInvoiceTabClicked">Check which tab is clicked by user</param>
        /// <param name="searchType">Type of Search i.e. Quick Search or Detailed Search</param>
        /// <returns>JsonResult to bind with InvoiceList or FileList grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult GetInvoiceAndFileGridData(string billingPeriod, bool isInvoiceTabClicked, string searchType, string searchCriteria)
        {
          // Retrieve userId from Session and use it across the method
          var userId = SessionUtil.UserId;

          // JavaScriptSerializer is used to Deserialize string to Model
          JavaScriptSerializer searchCriteriaSringToModel = new JavaScriptSerializer();

          // Populate "ProcessingDashboardSearchEntity" model with search criteria specified by user
          ProcessingDashboardSearchEntity searchcriteria = new ProcessingDashboardSearchEntity();

          // Declare variable of type Billing period
          BillingPeriod billingPeriodDetails = new BillingPeriod();

          // Retrieve member Clearence House he belongs to
          var clearingHouse = new ClearingHouse();
          switch ((int)SessionUtil.UserCategory)
          {
            case (int)UserCategory.Member:
              clearingHouse = _memberManager.GetClearingHouseDetail(SessionUtil.MemberId);
              break;
            case (int)UserCategory.IchOps:
              clearingHouse = ClearingHouse.Ich;
              break;
            case (int)UserCategory.AchOps:
              clearingHouse = ClearingHouse.Ach;
              break;
            case (int)UserCategory.SisOps:
              clearingHouse = ClearingHouse.Ich;
              break;
          }
          // If billingPeriod selected by user is currentand searchType is "QuickSearch", get Current billing period details
          if (billingPeriod == "Current" && searchType == "QuickSearch")
          {
            billingPeriodDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);
          }// end if()

          // If billingPeriod selected by user is Previous and searchType is "QuickSearch", get Previous billing period details
          if (billingPeriod == "Previous" && searchType == "QuickSearch")
          {
            billingPeriodDetails = _calenderManager.GetLastClosedBillingPeriod(clearingHouse);
          }// end if()

          // If user has clicked Invoice tab, populate InvoiceSearch grid depending on search criteria specified by user
          if (isInvoiceTabClicked)
          {
            // Create instance of InvoiceSearch grid
            var gridSearchResults = new InvoiceStatusSearchResults("ISSearchResultListGrid", Url.Action("ISSearchResultGridData", "ProcessingDashboard"), ((int)SessionUtil.UserCategory == (int)UserCategory.Member || (int)SessionUtil.UserCategory == (int)UserCategory.SisOps) ? true : false);

            // Create list of type "ProcessingDashboardInvoiceStatus"
            List<ProcessingDashboardInvoiceStatusResultSet> invoiceList = new List<ProcessingDashboardInvoiceStatusResultSet>();

            // If user has clicked detailedSearch radio button, deserialize seachCriteria details 
            if (searchType == "DetailedSearch")
            {
              // Deserialize search criteria string to Model properties
              searchcriteria = searchCriteriaSringToModel.Deserialize<ProcessingDashboardSearchEntity>(searchCriteria);
            }// end if()
            // If user has clicked quickSearch, check whether billing period is Current or Previous
            else
            {
              //CMP559 : Add Submission Method Column to Processing Dashboard  
              searchcriteria.SubmissionMethodId = -1;
              //CMP529 : Daily Output Generation for MISC Bilateral Invoices
              searchcriteria.DailyDeliverystatusId = -1;
              
              // If billing period is "Current" get billing details for Current period, else for previous period 
              if (billingPeriod == "Current")
              {
                searchcriteria.BillingYear = billingPeriodDetails.Year;
                searchcriteria.BillingMonth = billingPeriodDetails.Month;
                searchcriteria.BillingPeriod = billingPeriodDetails.Period;
                // Set other field values to "-1" as we check null or "-1" for fields not specified
                searchcriteria.BillingCategory = -1;
                searchcriteria.InvoiceStatus = -1;
                searchcriteria.SettlementMethod = -1;
              }// end if()
              else
              {
                searchcriteria.BillingYear = billingPeriodDetails.Year;
                searchcriteria.BillingMonth = billingPeriodDetails.Month;
                searchcriteria.BillingPeriod = billingPeriodDetails.Period;
                // Set other field values to "-1" as we check null or "-1" for fields not specified
                searchcriteria.BillingCategory = -1;
                searchcriteria.InvoiceStatus = -1;
                searchcriteria.SettlementMethod = -1;
              }// end else
            }// end else

            // Set UserId as currentUser's Id
            searchcriteria.IsUserId = userId;
            // Retrieve data from database for InvoiceList
            invoiceList = _dashboardManager.GetInvoiceStatusResultForProcDashBrd(searchcriteria);

            // Check whether InvoiceDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            foreach (var invoice in invoiceList.Where(invoice => invoice.InvoiceDate == DateTime.MinValue))
            {
              invoice.InvoiceDate = null;
            }
            foreach (var invoice in invoiceList)
            {
              invoice.UInvoiceId =  ConvertUtil.ConvertGuidToString(invoice.InvoiceId);
            }
              
            // Bind retrieved data to grid and return
            return gridSearchResults.DataBind(invoiceList.AsQueryable());
          }// end if()

          // If user has clicked File tab, populate InvoiceSearch grid depending on search criteria specified by user
          // Create instance of InvoiceSearch grid
          var fileGridSearchResults = new FileStatusSearchResults("FSSearchResultListGrid", Url.Action("FSSearchResultGridData", "ProcessingDashboard"));

          // Create list of type "ProcessingDashboardFileStatus"
          // List<ProcessingDashboardFileStatusResultSet> fileList = new List<ProcessingDashboardFileStatusResultSet>();

          // If user has clicked detailedSearch radio button, deserialize seachCriteria details 
          if (searchType == "DetailedSearch")
          {
            // Deserialize search criteria string to Model properties
            searchcriteria = searchCriteriaSringToModel.Deserialize<ProcessingDashboardSearchEntity>(searchCriteria);
          }// end if()
          else
          {
            // If billing period is "Current" get billing details for Current period, else for previous period 
            if (billingPeriod == "Current")
            {
              searchcriteria.BillingYear = billingPeriodDetails.Year;
              searchcriteria.BillingMonth = billingPeriodDetails.Month;
              searchcriteria.BillingPeriod = billingPeriodDetails.Period;
              // Set other field values to "-1" as we check null or "-1" for fields not specified
              searchcriteria.BillingCategory = -1;
            }// end if()
            else
            {
              searchcriteria.BillingYear = billingPeriodDetails.Year;
              searchcriteria.BillingMonth = billingPeriodDetails.Month;
              searchcriteria.BillingPeriod = billingPeriodDetails.Period;
              // Set other field values to "-1" as we check null or "-1" for fields not specified
              searchcriteria.BillingCategory = -1;
            }// end else
          }// end else

          // Set ISUSerId to current user
          searchcriteria.IsUserId = userId;
          // Retrieve data from database for FileList
          var fileList = _dashboardManager.GetFileStatusResultForProcDashBrd(searchcriteria);
          string localtimezone = string.IsNullOrEmpty(SessionUtil.TimeZone) ? "UTC" : SessionUtil.TimeZone;
          TimeZoneInfo utctimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
          TimeZoneInfo selectedtimeZone = TimeZoneInfo.FindSystemTimeZoneById(localtimezone);
          fileList = (from f in fileList
                      select new ProcessingDashboardFileStatusResultSet
                      {
                          IsFileLogId = f.IsFileLogId,
                          NumberOfInvoicesInFile = f.NumberOfInvoicesInFile,
                          NumberOfValidInvoicesInFile = f.NumberOfValidInvoicesInFile,
                          NumberOfInvalidInvoicesInFile = f.NumberOfInvalidInvoicesInFile,
                          FileName = f.FileName,
                          BillingMemberId = f.BillingMemberId,
                          BillingMemberCode = f.BillingMemberCode,
                          BillingMemberName = f.BillingMemberName,
                          BillingCategoryId = f.BillingCategoryId,
                          BillingCategory = f.BillingCategory,
                          FileFormatId = f.FileFormatId,
                          FileFormat = f.FileFormat,
                          ReceivedByIS = f.ReceivedByIS,
                          FileGeneratedDate = TimeZoneInfo.ConvertTime(f.ReceivedByIS, utctimeZone, selectedtimeZone),
                          FileStatusId = f.FileFormatId,
                          FileStatus = f.FileStatus,
                          RejectOnValidationFailure = f.RejectOnValidationFailure,
                          InvoicesInPeriodError = f.InvoicesInPeriodError,
                          //CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Added new column
                          FileProgressStatus = f.FileProgressStatus
                      }).ToList();
          // Bind retrieved data to grid and return
          return fileGridSearchResults.DataBind(fileList.AsQueryable());
        }// end GetInvoiceAndFileGridData()

        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult GetFileDetailForWarningDialogue(string fileId)
        {
            // Create instance of InvoiceSearch grid
            var dt = _dashboardManager.GetFileInvoicesErrorWarning(Guid.Parse(fileId));
            var gridSearchResults = new FileStatusLateSubmissionWarning("InvoiceDetailWarningGrid", Url.Action("GetGetFileDetailForWarningGridResult", "ProcessingDashboard"));
            return gridSearchResults.DataBind(dt.AsQueryable());

        }

        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult GetGetFileDetailForWarningGridResult()
        {
            return null;
        }

        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public ActionResult ISProcessingDashboard()
        {
          //SCP109185: IS DASHBOARD mandatory fields
          Session["IsSISOpsUser"] = "false";
          var categoryId = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
          if (categoryId == 1)
          {
            Session["IsSISOpsUser"] = "true";
          }
            return View();
        }

        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult InvoiceStatus()
        {
          ViewData["IsLateSubOpen"] = _dashboardManager.IsLateSubmissionWindowOpen();
          //SCP109185: IS DASHBOARD mandatory fields
          Session["IsSISOpsUser"] = "false";
            //CMP559 : Add Submission Method Column to Processing Dashboard
            ViewData["IsSISOpsUser"] = "false";
            ViewData["Status"] = "InvoiceStatus";
            ViewData["ResubmitIsOpsOnly"] = false;
            ViewData["UniqueInvoiceNoShowClaimFailedCasesOnly"] = "AllUser";
            var categoryId = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
            if (categoryId == 1 || categoryId == 2)
            {
                ViewData["ResubmitIsOpsOnly"] = "true";

                ViewData["UniqueInvoiceNoShowClaimFailedCasesOnly"] = "SIS_ICH_OpsUserOnly";
              
                if (categoryId == 1)
                {
                  Session["IsSISOpsUser"] = "true";
                  //CMP559 : Add Submission Method Column to Processing Dashboard
                  ViewData["IsSISOpsUser"] = "true";
                }
            }
         
            ViewData["ProcessingDashboardSearch"] = _dashboardManager.SetProcessingDashboardSearchEntity(categoryId, SessionUtil.MemberId);

            var gridSearchResults = new InvoiceStatusSearchResults("ISSearchResultListGrid", Url.Action("ISSearchResultGridData", "ProcessingDashboard"), ((int)SessionUtil.UserCategory == (int)UserCategory.Member || (int)SessionUtil.UserCategory == (int)UserCategory.SisOps) ? true : false);
            //gridSearchResults.Instance.Columns[19].Visible = categoryId == 1 || categoryId == 2;
            
            // SCP89957: Processing Dashboard - Missing Settlement File Sent Status.
            // Column[19] i.e "Unique Invoice ID" was initially made visible for only User Category 1 and 2, but now there are 2 columns were added in this grid in between,
            // and hence the making column visible on the basis of index fails.
            // Now column "Unique Invoice ID" column is made visisble on the basis of its field value.
            var isColumnPresent = gridSearchResults.Instance.Columns.Find(c => c.DataField == "UInvoiceId");
            if (isColumnPresent != null)
            {
              isColumnPresent.Visible = categoryId == 1 || categoryId == 2;
            } // end If

            ViewData["ISSearchResultListGrid"] = gridSearchResults.Instance;
            ViewData["InvoiceStatusActionResultList"] = new ProcessingDashboardInvoiceActionResults("ISInvoiceActionResultsGrid", Url.Action("GetEmptyInvoiceActionResultsList", "ProcessingDashboard")).Instance;
            ViewData["IsInvoiceTabClicked"] = "true";
            ViewData["UserCategory"] = categoryId;
            Session["helplinkurl"] = "Invoice_Status_Details";
            return PartialView("InvoiceStatusControl");
        }

        /// <summary>
        /// This will return partial view result for invoice search criteria result.
        /// </summary>
        /// <returns>PartialViewResult</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public ActionResult InvoiceStatusSearchResult()
        {
            return PartialView("InvoiceStatusSearchResultControl");
        }

        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult FileStatus()
        {
          //SCP109185: IS DASHBOARD mandatory fields
          Session["IsSISOpsUser"] = "false";
          ViewData["IsLateSubOpen"] = _dashboardManager.IsLateSubmissionWindowOpen();

          var categoryId = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
          ViewData["ProcessingDashboardSearch"] = _dashboardManager.SetProcessingDashboardSearchEntity(categoryId,
                                                                                                       SessionUtil.
                                                                                                         MemberId);
          if (categoryId == 1)
          {
            Session["IsSISOpsUser"] = "true";
          }

          var gridSearchResults = new FileStatusSearchResults("FSSearchResultListGrid",
                                                              Url.Action("FSSearchResultGridData", "ProcessingDashboard"));
          ViewData["FSSearchResultModel"] = gridSearchResults.Instance;
          ViewData["FileStatusActionResultList"] =
            new ProcessingDashboardFileActionResults("FSFileActionResultsGrid",
                                                     Url.Action("GetEmptyFileActionResultsList", "ProcessingDashboard"))
              .Instance;

          ViewData["FileStatusDeleteActionResultList"] =
            new ProcessingDashboardFileDeleteActionResults("FileDeleteActionStatusResultsGrid",
                                                           Url.Action("GetEmptyFileActionResultsList",
                                                                      "ProcessingDashboard")).Instance;

          var gridResults = new FileStatusLateSubmissionWarning("InvoiceDetailWarningGrid",
                                                                Url.Action("GetGetFileDetailForWarningGridResult",
                                                                           "ProcessingDashboard"));
          ViewData["InvoiceDetailWarning"] = gridResults.Instance;

          Session["helplinkurl"] = "File_Status_Details";

          return PartialView("FileStatusControl");
        }

      /// <summary>
        /// Following Action is used to populate InvoiceStatus grid when user clicks on Invoice Status tab. 
        /// </summary>
        /// <returns>InvoiceList JsonResult binded to InvoiceStatus grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult ISSearchResultGridData()
        {
          // Retrieve userId from Session and use it across the method
          var userId = SessionUtil.UserId;

          // Create instance of InvoiceSearch grid
          var gridSearchResults = new InvoiceStatusSearchResults("ISSearchResultListGrid",
                                                                 Url.Action("ISSearchResultGridData",
                                                                            "ProcessingDashboard"), ((int)SessionUtil.UserCategory == (int)UserCategory.Member || (int)SessionUtil.UserCategory == (int)UserCategory.SisOps)?true:false);

          var clearingHouse = new ClearingHouse();
          switch ((int) SessionUtil.UserCategory)
          {
            case (int) UserCategory.Member:
              clearingHouse = _memberManager.GetClearingHouseDetail(SessionUtil.MemberId);
              break;
            case (int) UserCategory.IchOps:
              clearingHouse = ClearingHouse.Ich;
              break;
            case (int) UserCategory.AchOps:
              clearingHouse = ClearingHouse.Ach;
              break;
            case (int) UserCategory.SisOps:
              clearingHouse = ClearingHouse.Ich;
              break;
          }


          // If user belongs to categoryId == 1, populate InvoiceStatus grid with invoices for current period
          // Populate "ProcessingDashboardSearchEntity" model with search criteria specified by user
          ProcessingDashboardSearchEntity searchcriteria = new ProcessingDashboardSearchEntity();

          // Call GetCurrentBillingPeriod() method which will return Current billing details
          var currentBillingDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);

          // Set Current billing details
          searchcriteria.BillingYear = currentBillingDetails.Year;
          searchcriteria.BillingMonth = currentBillingDetails.Month;
          searchcriteria.BillingPeriod = currentBillingDetails.Period;
          searchcriteria.IsUserId = userId;
          // Set other field values to "-1" as we check null or "-1" for fields not specified
          searchcriteria.BillingCategory = -1;
          searchcriteria.InvoiceStatus = -1;
          searchcriteria.SettlementMethod = -1;
          //CMP559 : Add Submission Method Column to Processing Dashboard
          searchcriteria.SubmissionMethodId = -1;
          //CMP529 : Daily Output Generation for MISC Bilateral Invoices
          searchcriteria.DailyDeliverystatusId = -1;

          // Retrieve data from database for InvoiceList
          var invoiceList = _dashboardManager.GetInvoiceStatusResultForProcDashBrd(searchcriteria);

          // Check whether InvoiceDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
          foreach (var invoice in invoiceList.Where(invoice => invoice.InvoiceDate == DateTime.MinValue))
          {
            invoice.InvoiceDate = null;
          }
          foreach (var invoice in invoiceList)
          {
              invoice.UInvoiceId = ConvertUtil.ConvertGuidToString(invoice.InvoiceId);
          }

          // Bind retrieved data to grid and return
          return gridSearchResults.DataBind(invoiceList.AsQueryable());

        }// end ISSearchResultGridData()

        /// <summary>
        /// Following action is used to populate FileStatus grid when user clicks on File Status tab. 
        /// </summary>
        /// <returns>FileList JsonResult binded to FileStatus grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult FSSearchResultGridData()
        {
          // Retrieve userId from Session and use it across the method
          var userId = SessionUtil.UserId;

          // Create instance of InvoiceSearch grid
          var gridSearchResults = new FileStatusSearchResults("FSSearchResultListGrid",
                                                              Url.Action("FSSearchResultGridData", "ProcessingDashboard"));

          // Create list of type "ProcessingDashboardInvoiceStatus"

          var clearingHouse = new ClearingHouse();
          switch ((int) SessionUtil.UserCategory)
          {
            case (int) UserCategory.Member:
              clearingHouse = _memberManager.GetClearingHouseDetail(SessionUtil.MemberId);
              break;
            case (int) UserCategory.IchOps:
              clearingHouse = ClearingHouse.Ich;
              break;
            case (int) UserCategory.AchOps:
              clearingHouse = ClearingHouse.Ach;
              break;
            case (int) UserCategory.SisOps:
              clearingHouse = ClearingHouse.Ich;
              break;
          }

          // Populate "ProcessingDashboardSearchEntity" model with search criteria specified by user
          var searchcriteria = new ProcessingDashboardSearchEntity();

          // Call GetCurrentBillingPeriod() method which will return Current billing details
          var currentBillingDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);

          // Set Current billing details
          searchcriteria.BillingYear = currentBillingDetails.Year;
          searchcriteria.BillingMonth = currentBillingDetails.Month;
          searchcriteria.BillingPeriod = currentBillingDetails.Period;
          searchcriteria.IsUserId = userId;
          // Set other field values to "-1" as we check null or "-1" for fields not specified
          searchcriteria.BillingCategory = -1;

          // Retrieve data from database for FileList
          var fileList = _dashboardManager.GetFileStatusResultForProcDashBrd(searchcriteria);
          string localtimezone = string.IsNullOrEmpty(SessionUtil.TimeZone) ? "UTC" : SessionUtil.TimeZone;
          TimeZoneInfo utctimeZone = TimeZoneInfo.FindSystemTimeZoneById("UTC");
          TimeZoneInfo selectedtimeZone = TimeZoneInfo.FindSystemTimeZoneById(localtimezone);
          fileList = (from f in fileList
                      select new ProcessingDashboardFileStatusResultSet
                      {
                          IsFileLogId = f.IsFileLogId,
                          NumberOfInvoicesInFile = f.NumberOfInvoicesInFile,
                          NumberOfValidInvoicesInFile = f.NumberOfValidInvoicesInFile,
                          NumberOfInvalidInvoicesInFile = f.NumberOfInvalidInvoicesInFile,
                          FileName = f.FileName,
                          BillingMemberId = f.BillingMemberId,
                          BillingMemberCode = f.BillingMemberCode,
                          BillingMemberName = f.BillingMemberName,
                          BillingCategoryId = f.BillingCategoryId,
                          BillingCategory = f.BillingCategory,
                          FileFormatId = f.FileFormatId,
                          FileFormat = f.FileFormat,
                          ReceivedByIS = f.ReceivedByIS,
                          FileGeneratedDate = TimeZoneInfo.ConvertTime(f.ReceivedByIS, utctimeZone, selectedtimeZone),
                          FileStatusId = f.FileFormatId,
                          FileStatus = f.FileStatus,
                          RejectOnValidationFailure = f.RejectOnValidationFailure,
                          InvoicesInPeriodError = f.InvoicesInPeriodError,
                          //CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Added new column
                          FileProgressStatus = f.FileProgressStatus
                      }).ToList();

          // Bind retrieved data to grid and return
          return gridSearchResults.DataBind(fileList.AsQueryable());


        }// end FSSearchResultGridData()

        [HttpPost]
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public ActionResult GetInvoiceDetail(Guid invoiceId)
        {
            ProcessingDashboardInvoiceDetail invoiceDetail = _dashboardManager.GetInvoiceDetailsForProcDashBrd(invoiceId);

            // Check whether InvoiceDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.InvoiceDate == DateTime.MinValue)
                invoiceDetail.InvoiceDate = null;

            // Check whether ReceivedInIS is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.ReceivedInIS == DateTime.MinValue)
                invoiceDetail.ReceivedInIS = null;

            // Check whether ValidationStatusDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.ValidationStatusDate == DateTime.MinValue)
                invoiceDetail.ValidationStatusDate = null;

            // Check whether ValueConfirmationStatusDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.ValueConfirmationStatusDate == DateTime.MinValue)
                invoiceDetail.ValueConfirmationStatusDate = null;

            // Check whether DigitalSignatureStatusDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.DigitalSignatureStatusDate == DateTime.MinValue)
                invoiceDetail.DigitalSignatureStatusDate = null;

            // Check whether SettlementFileStatusDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.SettlementFileStatusDate == DateTime.MinValue)
                invoiceDetail.SettlementFileStatusDate = null;

            // Check whether PresentedStatusDate is equal to DateTime.MinValue i.e.1/1/0001 12:00:00 AM, if yes set it to null 
            if (invoiceDetail.PresentedStatusDate == DateTime.MinValue)
                invoiceDetail.PresentedStatusDate = null;

            return PartialView("InvoiceDetailsOnDashboard", invoiceDetail);
        }// end GetInvoiceDetail()

        [HttpPost]
        public FileResult GenerateFileStatusCsv(string fileSearchCriteriaCsv, string fileBillingPeriodCsv, string fileSearchTypeCsv)
        {
            byte[] bytes = GetFileToDownload(fileSearchCriteriaCsv, fileBillingPeriodCsv, fileSearchTypeCsv, false);
            return File(bytes, "text/csv", "FileStatus.csv");
        }

        [HttpPost]
        public FileResult GenerateInvoiceStatusCsv(string searchCriteriaCsvServerSide, string billingPeriodCsv, string searchTypeCsv)
        {
            byte[] bytes = GetFileToDownload(searchCriteriaCsvServerSide, billingPeriodCsv, searchTypeCsv, true);
            return File(bytes, "text/csv", "InvoiceStatus.csv");
        }

        private byte[] GetFileToDownload(string searchCriteriaCsvServerSide, string billingPeriodCsv, string searchTypeCsv, bool isInvoiceStatus)
        {
            // JavaScriptSerializer is used to Deserialize string to Model
            JavaScriptSerializer searchCriteriaSringToModel = new JavaScriptSerializer();
            ProcessingDashboardSearchEntity searchCriteria = null;

            // parse incoming json string. in detail serach we will use this parse object
         
            try
            {
                if (!string.IsNullOrEmpty(searchCriteriaCsvServerSide))
                {
                    searchCriteria = searchCriteriaSringToModel.Deserialize<ProcessingDashboardSearchEntity>(searchCriteriaCsvServerSide);
                }
            }
            catch (Exception exception)
            {

            }
            // If user has clicked quickSearch, check whether billing period is Current or Previous
            if (searchTypeCsv != "DetailedSearch")
            {
                searchCriteria = new ProcessingDashboardSearchEntity();
                var billingDetails = new BillingPeriod();
               
                var clearingHouse = new ClearingHouse();
                switch ((int)SessionUtil.UserCategory)
                {
                  case (int)UserCategory.Member:
                    clearingHouse = _memberManager.GetClearingHouseDetail(SessionUtil.MemberId);
                    break;
                  case (int)UserCategory.IchOps:
                    clearingHouse = ClearingHouse.Ich;
                    break;
                  case (int)UserCategory.AchOps:
                    clearingHouse = ClearingHouse.Ach;
                    break;
                  case (int)UserCategory.SisOps:
                    clearingHouse = ClearingHouse.Ich;
                    break;
                }

                // If billing period is "Current" get billing details for Current period, else for previous period 
                if (billingPeriodCsv == "Current" || string.IsNullOrEmpty(billingPeriodCsv))
                  billingDetails = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);
                else
                    billingDetails = _calenderManager.GetLastClosedBillingPeriod(clearingHouse);

                searchCriteria.BillingYear = billingDetails.Year;
                searchCriteria.BillingMonth = billingDetails.Month;
                searchCriteria.BillingPeriod = billingDetails.Period;
                // Set other field values to "-1" as we check null or "-1" for fields not specified
                searchCriteria.BillingCategory = -1;
                searchCriteria.InvoiceStatus = -1;
                searchCriteria.SettlementMethod = -1;
                searchCriteria.IncludeProcessingDatesTimestamp = true;
                //CMP559 : Add Submission Method Column to Processing Dashboard
                searchCriteria.SubmissionMethodId = -1;
                //CMP529 : Daily Output Generation for MISC Bilateral Invoices
                searchCriteria.DailyDeliverystatusId = -1;
            }
            searchCriteria.IsUserId = SessionUtil.UserId;

            return _dashboardManager.GenerateCsv(searchCriteria, ',', isInvoiceStatus);

        }// end GetFileToDownload()

        /// <summary>
        /// Following action is used to set LateSubmission flag true for selected Invoices.
        /// </summary>
        /// <param name="selectedInvoiceIds">Comma separated InvoiceId string</param>
        /// <returns>JsonResult of Invoice List binded to grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.MarkforLateSubmission)]
        public JsonResult MarkInvoiceForLateSubmission(string selectedInvoiceIds)
        {
            // Split InvoiceId string on "," and add to InvoiceIdArray
            string[] invoiceIdArray = selectedInvoiceIds.Split(',');

            // Call "MarkInvoiceForLateSubmission" method passing InvoiceIdArray which will update LateSubmission flag
            // to true for selected Invoices
            List<ProcessingDashboardInvoiceActionStatus> invoiceList = _dashboardManager.MarkInvoiceForLateSubmission(invoiceIdArray, SessionUtil.MemberId, Convert.ToInt32(SessionUtil.UserId),false);

            // Return JsonResult of Invoice List
            return Json(invoiceList);
        }// end MarkInvoiceForLateSubmission()

        /// <summary>
        /// Following action is used to set LateSubmission flag true for invoices in selected Files.
        /// </summary>
        /// <param name="selectedFileIds">Comma separated FileId string</param>
        /// <returns>JsonResult of File List binded to grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.MarkforLateSubmission)]
        public JsonResult MarkFileForLateSubmission(string selectedFileIds)
        {
            // Call "MarkFileForLateSubmission" method passing comma separated list of File Ids  to update LateSubmission flag
            // to true for invoices in selected files
            List<ProcessingDashboardFileActionStatus> fileList = _dashboardManager.MarkFileForLateSubmission(selectedFileIds, SessionUtil.MemberId, SessionUtil.UserId);

            // Return JsonResult of Invoice List
            return Json(fileList);
        }// end MarkFileForLateSubmission()

        /// <summary>
        /// Following action is used for InvoiceAction grid Instantiation
        /// </summary>
        /// <param name="selectedFileIds">Selected InvoiceId's</param>
        /// <returns>JsonResult to bind with Grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult GetEmptyInvoiceActionResultsList(string selectedInvoiceIds)
        {
            // Return null
            return null;
        }// end GetEmptyInvoiceActionResultsList()

        /// <summary>
        /// Following action is used to increment BillingPeriod for selected Invoices.
        /// </summary>
        /// <param name="selectedInvoiceIds">Comma separated InvoiceId string</param>
        /// <returns>JsonResult of Invoice List binded to grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.IncrBillingPeriod)]
        public JsonResult IncrementInvoiceBillingPeriod(string selectedInvoiceIds)
        {
            var logRefId = Guid.NewGuid();
            var log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod", this.ToString(),
                                     "ProcessingDashboard", "Stage 1: IncrementInvoiceBillingPeriod start", SessionUtil.UserId, logRefId.ToString());
        _referenceManager.LogDebugData(log);

            
            // For Late Submission and Increament BillingPeriod  transaction should not be allowed to process by two user simultaneously.
        //SCP:170853 IS-Web response time - ICH Ops 
            //Thread.Sleep(SessionUtil.UserId + 500);

            // Split InvoiceId string on "," and add to InvoiceIdArray
            string[] invoiceIdArray = selectedInvoiceIds.Split(',');

            // Call "IncrementInvoiceBillingPeriod" method passing InvoiceIdArray which will increment Invoice
            // BillingPeriod for selected Invoices
            List<ProcessingDashboardInvoiceActionStatus> invoiceList = _dashboardManager.IncrementInvoiceBillingPeriod(invoiceIdArray, Convert.ToInt32(SessionUtil.UserId));

            log = _referenceManager.GetDebugLog(DateTime.Now, "IncrementInvoiceBillingPeriod", this.ToString(),
                                     "ProcessingDashboard", "Stage 1: IncrementInvoiceBillingPeriod completed", SessionUtil.UserId, logRefId.ToString());
            _referenceManager.LogDebugData(log);


            // Create instance of Invoice Action Results grid
            var invoiceGridResults = new ProcessingDashboardInvoiceActionResults("ISInvoiceActionResultsGrid", Url.Action("GetEmptyInvoiceActionResultsList", "ProcessingDashboard"));

            // Return JsonResult of Invoice List
            return Json(invoiceList);
        }// end IncrementInvoiceBillingPeriod()

        /// <summary>
        /// Following action is used to delete selected Invoices.
        /// </summary>
        /// <param name="selectedInvoiceIds">Comma separated InvoiceId string</param>
        /// <returns>JsonResult of Invoice List binded to grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.Delete)]
        public JsonResult DeleteSelectedInvoices(string selectedInvoiceIds)
        {
            //SCP0000: Elmah Exceptions log removal
            var invoiceIdArray = new string[] {};

            if(selectedInvoiceIds != null)
            {
              // Split InvoiceId string on "," and add to InvoiceIdArray
              invoiceIdArray = selectedInvoiceIds.Split(',');
            }

            var memberId = _memberManager.GetMemberId(String.IsNullOrEmpty(ConfigurationManager.AppSettings["DummyMembercode"].Trim()) ? "000" : ConfigurationManager.AppSettings["DummyMembercode"].Trim());
            
           // var proxy = _iUserManagement.GetProxyUserByAdminUserID()

            var userId = SessionUtil.AdminUserId;

            // Call "IncrementInvoiceBillingPeriod" method passing InvoiceIdArray which will increment Invoice
            // BillingPeriod for selected Invoices
            List<ProcessingDashboardInvoiceActionStatus> invoiceList = _dashboardManager.DeleteSelectedInvoices(invoiceIdArray, memberId, userId);

            // Create instance of Invoice Action Results grid
            var invoiceGridResults = new ProcessingDashboardInvoiceActionResults("ISInvoiceActionResultsGrid", Url.Action("GetEmptyInvoiceActionResultsList", "ProcessingDashboard"));

            // Return JsonResult of Invoice List
            return Json(invoiceList);
        }// end DeleteSelectedInvoices()

        /// <summary>
        /// Following action is used to Resubmit selected Invoices.
        /// </summary>
        /// <param name="selectedInvoiceIds">Comma separated InvoiceId string</param>
        /// <returns>JsonResult of Invoice List binded to grid</returns>
        public JsonResult ResubmitSelectedInvoices(string selectedInvoiceIds)
        {
            // Split InvoiceId string on "," and add to InvoiceIdArray
            string[] invoiceIdArray = selectedInvoiceIds.Split(',');

            // Call "ResubmitSelectedInvoices" method passing InvoiceIdArray which will ReSubmit Invoice 
            // and will update ICH Settelment Flag
            List<ProcessingDashboardInvoiceActionStatus> invoiceList = _dashboardManager.ResubmitSelectedInvoices(invoiceIdArray);

            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
            //Commented below code as the created object is not used 
            // Create instance of Invoice Action Results grid
           // var invoiceGridResults = new ProcessingDashboardInvoiceActionResults("ISInvoiceActionResultsGrid", Url.Action("GetEmptyInvoiceActionResultsList", "ProcessingDashboard"));

            // Return JsonResult of Invoice List
            return Json(invoiceList);
        }// end ResubmitSelectedInvoices()

        /// <summary>
        /// Following action is used for FileAction grid Instantiation
        /// </summary>
        /// <param name="selectedFileIds">Selected FileId's</param>
        /// <returns>Jsonresult to bind with Grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.View)]
        public JsonResult GetEmptyFileActionResultsList(string selectedFileIds)
        {
            // Return null
            return null;
        }// end GetEmptyFileActionResultsList()

        /// <summary>
        /// Following action is used to Increment Billing period for Invoices within a File
        /// </summary>
        /// <param name="selectedFileIds">Selected files Id's</param>
        /// <returns>JsonResult of File Details for binding with Grid</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.IncrBillingPeriod)]
        public JsonResult IncrementBillingPeriodForInvoicesWithinFile(string selectedFileIds)
        {
            // For Late Submission and Increament BillingPeriod  transaction should not be allowed to process by two user simultaneously.
            Thread.Sleep(SessionUtil.UserId + 500);

            // Call IncrementBillingPeriodForInvoicesWithinFile() method which will increment Billing period of Invoices within selected files
            List<ProcessingDashboardFileActionStatus> fileList = _dashboardManager.IncrementBillingPeriodForInvoicesWithinFile(selectedFileIds, SessionUtil.MemberId, SessionUtil.UserId);

            // Create instance of File Action Results grid
            var fileGridResults = new ProcessingDashboardFileActionResults("FSFileActionResultsGrid", Url.Action("GetEmptyFileActionResultsList", "ProcessingDashboard"));

            // Return JsonResult of File List
            return Json(fileList);
        }// end IncrementBillingPeriodForInvoicesWithinFile()

        /// <summary>
        /// Following action is used to delete files
        /// </summary>
        /// <param name="selectedFileIds">Id's of file to be deleted</param>
        /// <returns>JsonResult of non deleted files</returns>
        [ISAuthorize(Business.Security.Permissions.Reports.ProcessingDashboard.Delete)]
        public JsonResult DeleteFiles(string selectedFileIds)
        {

            // Call "DeleteFiles()" method which will delete selected files 
            var memberId = _memberManager.GetMemberId(String.IsNullOrEmpty(ConfigurationManager.AppSettings["DummyMembercode"].Trim()) ? "000" : ConfigurationManager.AppSettings["DummyMembercode"].Trim());
            var userId = SessionUtil.AdminUserId;

            var fileList = new List<ProcessingDashboardFileDeleteActionStatus>();
            if(memberId > 0 && userId > 0)
            {
                fileList = _dashboardManager.DeleteFiles(selectedFileIds, memberId, userId);
            }

            //SCP99417:Is-web performance
           //Commented below line as fileGridResults variable not used anywhere 
            // Create instance of File Action Results grid
            //  var fileGridResults = new ProcessingDashboardFileDeleteActionResults("FileDeleteActionStatusResultsGrid", Url.Action("GetEmptyFileActionResultsList", "ProcessingDashboard"));

            // Return JsonResult of File List
            return Json(fileList);
        }// end DeleteFiles()

        //CMP559 : Add Submission Method Column to Processing Dashboard
        public ActionResult DownloadFile(string fileLogId)
        {
            Guid isFileGuid = fileLogId.ToGuid();
            IsInputFile isFile = IsInputFileRepository.Get(f => f.Id == isFileGuid).FirstOrDefault();
            
            string fullFileName = Path.Combine(isFile.FileLocation, isFile.FileName);
           
            if (isFile.FileStatusId > 4)
                fullFileName = FileIo.GetInputFileLocation(isFile);

            return File(fullFileName, "application/octet", Path.GetFileName(fullFileName));
        }

    }// end ProcessingDashboardController class
}// end namespace


