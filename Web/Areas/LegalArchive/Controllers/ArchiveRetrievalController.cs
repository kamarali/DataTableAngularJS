using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Model.Cargo;
using Iata.IS.Business.MemberProfile;
using System.Web.Script.Serialization;
using Iata.IS.Web.UIModel.Grid.Cargo;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.Common;
using Iata.IS.Business.Cargo;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Business.Cargo;
using Iata.IS.Model.Cargo.Enums;
using Iata.IS.Business;
using Iata.IS.Core.Exceptions;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using System.IO;
using log4net;
using System.Reflection;
using Iata.IS.Model.Enums;
using Iata.IS.Business.LegalArchive;
using Iata.IS.Web.UIModel.Grid.LegalArchive;
using Iata.IS.Model.LegalArchive;
using Trirand.Web.Mvc;
using Iata.IS.Business.FileCore;

namespace Iata.IS.Web.Areas.LegalArchive.Controllers
{
    public class ArchiveRetrievalController : ISController
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IArchiveSearchManager _searchArchiveManager;
        private readonly IRetrievalManager _retrievalManager;
        private readonly IMemberManager _memberManager;
        private int _currentLoggedInMember;
        private const int maxRetrivalLimit = 99999;
        private const string searchSession = "LegalArchiveSearchCriteria";

        public ArchiveRetrievalController(IArchiveSearchManager searchArchiveManager, IRetrievalManager retrievalManager, IMemberManager memberManager)
        {
            _searchArchiveManager = searchArchiveManager;
            _retrievalManager = retrievalManager;
            _currentLoggedInMember = SessionUtil.MemberId;
            _memberManager = memberManager;

        }

        /// <summary>
        /// This method is use to search invoices based on criteria
        /// </summary>
         [ISAuthorize(Business.Security.Permissions.General.LegalArchive.SearchRetrieve)]
        [HttpGet]
        public ActionResult Search()
        {
            //Initialize search result grid
            var searchModel = new LegalArchiveSearchCriteria();


            //CMP #666: MISC Legal Archiving Per Location ID
            var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager)); // IOC resolve for interface
            var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId, SessionUtil.MemberId);
            ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode"); // Fill the ViewData for Location List Box
                foreach (var item in associatedLocations)
                {
                    searchModel.ArchivalLocationId += "," + item.LocationCode;
                }
                if (associatedLocations.Count == 0) searchModel.ArchivalLocationId = ",0";
            // End code CMP #666




            var legalArchiveResultGrid = new LegalArchiveSearchGrid(ControlIdConstants.LegalArchiveSearchGrid, Url.Action("SearchArchives", "ArchiveRetrieval"));
            ViewData[ViewDataConstants.LegalArchiveSearchGrid] = legalArchiveResultGrid.Instance;
            HttpContext.Session.Remove(searchSession);

           
            searchModel.SettlementMethodId = -1;


           
           // SCP221779: old invoices in SIS [Billing Year Dropdown value does't holds during Page post]
           var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
           searchModel.BillingYear = currentBillingPeriod.Year;
           
           return View(searchModel);
        }


        [ISAuthorize(Business.Security.Permissions.General.LegalArchive.SearchRetrieve)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(LegalArchiveSearchCriteria searchCriteria)
        {
            //Initialize search result grid
            HttpContext.Session.Add(searchSession, searchCriteria);


            // server Side Validation for Associatin Location
            var memberLocation = Ioc.Resolve<IManageContactsManager>(typeof(IManageContactsManager)); // IOC resolve for interface
            var associatedLocations = memberLocation.GetMemberAssociationLocForSearch(SessionUtil.UserId, SessionUtil.MemberId);
            ViewData["AssociatedLocation"] = new MultiSelectList(associatedLocations.ToArray(), "locationId", "locationCode"); // Fill the ViewData for Location List Box
            
            if (associatedLocations.Count == 0) searchCriteria.ArchivalLocationId = ",0";

            var selectedBillingMemberLocationList = searchCriteria.ArchivalLocationId.Split(Convert.ToChar(","));
            searchCriteria.ArchivalLocationId = "";
            foreach (var location in from location in selectedBillingMemberLocationList
                                     where location != null
                                     let contains = associatedLocations.SingleOrDefault(l => l.LocationCode == location)
                                     where contains != null
                                     select location)
            {
                searchCriteria.ArchivalLocationId += "," + location;
            }
            if (searchCriteria.ArchivalLocationId.Length == 0) searchCriteria.ArchivalLocationId = ",0";



            string criteria = searchCriteria != null ? new JavaScriptSerializer().Serialize(searchCriteria) : string.Empty;
            var legalArchiveResultGrid = new LegalArchiveSearchGrid(ControlIdConstants.LegalArchiveSearchGrid, Url.Action("SearchArchives", "ArchiveRetrieval", new { area = "LegalArchive", criteria }));
            ViewData[ViewDataConstants.LegalArchiveSearchGrid] = legalArchiveResultGrid.Instance;

            return View(searchCriteria);
        }

       
        [ISAuthorize(Business.Security.Permissions.General.LegalArchive.SearchRetrieve)]
        [HttpPost]
        public ActionResult RetriveArchive(string id,string selectedLocations,string model)
        {
            var messageDetails = new UIMessageDetail();

            // If user has not selected Invoices to submit display message
            if (id != null && model != null)
            {
                var archiveIdList = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var searchCriteriaValues = model.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                var searchCriteria = _searchArchiveManager.GetSearchCriteria(searchCriteriaValues.ToList());
                //CMP #666: MISC Legal Archiving Per Location ID
                searchCriteria.ArchivalLocationId = selectedLocations;
                try
                {
                    var memberId = _currentLoggedInMember;
                    var userId = SessionUtil.UserId;


                    if (archiveIdList.LongLength > 0 && archiveIdList.LongLength <= maxRetrivalLimit)
                    {
                        int totalNoOfInvoicesRetrive;
                        string jobId;
                        var result = _searchArchiveManager.RetriveLegalArchive(archiveIdList.ToList(), userId, memberId,
                                                                               searchCriteria, out jobId, out totalNoOfInvoicesRetrive);

                        if (result)
                        {
                            messageDetails = new UIMessageDetail
                                                 {
                                                     IsFailed = false,
                                                     Message =
                                                         string.Format(
                                                             "Retrieval Job ID {0} has been initiated for retrieval of {1} invoices.",
                                                             jobId, totalNoOfInvoicesRetrive) + "<br/>" +
                                                         " Please view screen Download Retrieved Invoices for retrieval status and for download of retrieved invoices."
                                                 };
                        }
                        else
                        {
                            messageDetails = new UIMessageDetail { IsFailed = true, Message = "system unable to retrive legal archives" };
                        }
                    }
                    else if (archiveIdList.LongLength > maxRetrivalLimit)
                    {
                        //Messages.InvoiceIneligibleForSubmission
                        messageDetails = new UIMessageDetail { IsFailed = true, Message = "Too many records matching the search criteria. Please refine the search criteria and try again" };
                    }
                    else
                    {
                        messageDetails = new UIMessageDetail { IsFailed = true, Message = "Please select record" };
                    }

                    return Json(messageDetails);
                }
                catch (ISBusinessException exception)
                {
                    messageDetails = new UIMessageDetail { IsFailed = true, Message = GetDisplayMessageWithErrorCode(exception.ErrorCode) };

                    return Json(messageDetails);
                }
            }
            else
            {

                //TODO: Change message caption string 
                messageDetails = new UIMessageDetail { IsFailed = true, Message = Messages.SelectInvoiceForSubmission };
                return Json(messageDetails);
            }
        }

        [ISAuthorize(Business.Security.Permissions.General.LegalArchive.SearchRetrieve)]
        [HttpPost]
        public ActionResult RetriveAll()
        {
            var messageDetails = new UIMessageDetail();
            var memberId = _currentLoggedInMember;
            var userId = SessionUtil.UserId;
            LegalArchiveSearchCriteria searchCriteria = null;

            if (HttpContext.Session[searchSession] != null)
                searchCriteria = (LegalArchiveSearchCriteria)HttpContext.Session[searchSession];
          
            if (searchCriteria != null)
            {
                var searchResult = _searchArchiveManager.SearchArchives(searchCriteria, _currentLoggedInMember).ToList();
                List<string> archiveIdList = searchResult.Select(m => m.Id.ToString()).ToList();

                if (archiveIdList.Count > 0 && archiveIdList.Count <= maxRetrivalLimit)
                {
                    int totalNoOfInvoicesRetrive;
                    string jobId;
                    var result = _searchArchiveManager.RetriveLegalArchive(archiveIdList.ToList(), userId, memberId,
                                                                           searchCriteria, out jobId,
                                                                           out totalNoOfInvoicesRetrive);

                    if (result)
                    {
                        messageDetails = new UIMessageDetail
                                             {
                                                 IsFailed = false,
                                                 Message =
                                                     string.Format(
                                                         "Retrieval Job ID {0} has been initiated for retrieval of {1} invoices.",
                                                         jobId, totalNoOfInvoicesRetrive) + "<br/>" +
                                                     " Please view screen Download Retrieved Invoices for retrieval status and for download of retrieved invoices."
                                             };
                    }
                    else
                    {
                        messageDetails = new UIMessageDetail
                                             {IsFailed = true, Message = "system unable to retrive legal archives"};
                    }

                }
                else if (archiveIdList.Count > maxRetrivalLimit)
                {
                    //Messages.InvoiceIneligibleForSubmission
                    messageDetails = new UIMessageDetail
                                         {
                                             IsFailed = true,
                                             Message =
                                                 "Too many records matching the search criteria. Please refine the search criteria and try again"
                                         };
                }
                else
                {
                    messageDetails = new UIMessageDetail
                                         {
                                             IsFailed = true,
                                             Message =
                                                 "No record found against search criteria. Please try again"
                                         };
                }
            }
            else
            {
                messageDetails = new UIMessageDetail
                {
                    IsFailed = true,
                    Message =
                        "Unable to process request. Please search again"
                };
            }

            return Json(messageDetails);
        }


        public JsonResult SearchArchives(string criteria)
        {
            var legalArchiveResultGrid = new LegalArchiveSearchGrid(ControlIdConstants.LegalArchiveSearchGrid, Url.Action("SearchArchives", "ArchiveRetrieval",
                                                                                         new { area = "LegalArchive", criteria }));
            if (!string.IsNullOrEmpty(criteria))
            {
                var searchCriteria = new JavaScriptSerializer().Deserialize(criteria, typeof(LegalArchiveSearchCriteria)) as LegalArchiveSearchCriteria;
                var searchResult = _searchArchiveManager.SearchArchives(searchCriteria, _currentLoggedInMember);
                return legalArchiveResultGrid.DataBind(searchResult);
            }

            return legalArchiveResultGrid.DataBind(null);
        }

        /// <summary>
        /// landing method for download retrival page 
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.General.LegalArchive.DownloadRetrievedInv)]
        [HttpGet]
        public ActionResult DownloadRetrievedFiles(int? sPageNo, int? sPageSize, int? dPageNo, int? dPageSize, string jobSyId = null)
        {
            var Id = jobSyId ?? _retrievalManager.GetJobSummaryId( _currentLoggedInMember,SessionUtil.UserId);

            /*Master Grid Display*/

            var gridModel = new ArchiveRetrivalJobSummaryGrid(ControlIdConstants.RetrivalJobSummaryGrid, Url.Action("RetrivalJobSummaryGridData", "ArchiveRetrieval"), sPageNo, sPageSize);

            ViewData[ViewDataConstants.RetrivalJobSummaryGridViewData] = gridModel.Instance;

            /*Detail Grid Display*/
            var gridModelDetail =
                new ArchiveRetrivalJobDetailsGrid(ControlIdConstants.ArchiveRetrivalJobDetailsGridControl, Url.Action("GetSelectedJobSummaryDetail", "ArchiveRetrieval", new { jobSummaryId = Id }), dPageNo,dPageSize);
            ViewData[ViewDataConstants.RetrivalJobDetailGridViewData] = gridModelDetail.Instance;
            
            return View();
        }

        /// <summary>
        /// get job details by job summary id 
        /// </summary>
        /// <param name="jobSummaryId">id of job summary table</param>
        /// <param name="currentPageNo">current Page no of detail grid</param>
        /// <param name="currentPageSize">current Page size of detail grid</param>
        /// <returns></returns>
        public JsonResult GetSelectedJobSummaryDetail(string jobSummaryId, int? currentPageNo, int? currentPageSize)
        {
            var gridModel = new ArchiveRetrivalJobDetailsGrid(ControlIdConstants.ArchiveRetrivalJobDetailsGridControl, Url.Action("GetSelectedJobSummaryDetail", "ArchiveRetrieval"), currentPageNo, currentPageSize);

            string imageAltText = Iata.IS.Web.AppSettings.DownloadFileImageAltText;
            string imageToolTip = Iata.IS.Web.AppSettings.DownloadFileImageToolTip;
            string imageUrl = Url.Content(Iata.IS.Web.AppSettings.DownloadZipFileImagePath);

            var id = new Guid(jobSummaryId);
            var list = _retrievalManager.GetJobDetailsByJobSummaryId(id, imageAltText, imageToolTip, imageUrl);
           
            return gridModel.DataBind(list.AsQueryable());
        }

        /// <summary>
        /// retrive job summary details for header grid
        /// </summary>
        /// <param name="currentPageNo">current Page no of summary grid</param>
        /// <param name="currentPageSize">current Page size of summary grid</param>
        /// <returns></returns>
        public JsonResult RetrivalJobSummaryGridData(int? currentPageNo, int? currentPageSize)
        {
            var gridModel = new ArchiveRetrivalJobSummaryGrid(ControlIdConstants.RetrivalJobSummaryGrid,
                                                              Url.Action("RetrivalJobSummaryGridData",
                                                                         "ArchiveRetrieval"), currentPageNo, currentPageSize);
         
            return gridModel.DataBind(_retrievalManager.GetRetrievedJobs(_currentLoggedInMember, SessionUtil.UserId).AsQueryable());
        }

        /// <summary>
        /// Downloads the file.
        /// </summary>
        /// <param name="FileToDownload">The file to download.</param>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.General.LegalArchive.DownloadRetrievedInv)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DownloadFile(string FileToDownload)
        {
            string rootFolder = FileIo.GetForlderPath(SFRFolderPath.LARetrieved);
            var fileInfo = FileToDownload.ToString().Split(new[] { '$' });
            string zipFile = fileInfo[0];  
            string jobId = fileInfo[1];
            var jobSummaryId = new Guid(fileInfo[2]);
            int sPageNo, sPageSize, dPageNo, dPageSize, summaryPageNo, summaryPageSize, detailPageNo, detailPageSize;
            summaryPageNo = int.TryParse((fileInfo[3]), out sPageNo) ? sPageNo : 1;
            summaryPageSize = int.TryParse((fileInfo[4]), out sPageSize) ? sPageSize : 5;
            detailPageNo = int.TryParse((fileInfo[5]), out dPageNo) ? dPageNo : 1;
            detailPageSize = int.TryParse((fileInfo[6]), out dPageSize) ? dPageSize : 5;
          
            try
            {
                if (string.IsNullOrEmpty(jobId) || string.IsNullOrEmpty(zipFile))
                {
                    TempData["DownloadFileError"] = AppSettings.DownloadFileError;

                    return RedirectToAction("DownloadRetrievedFiles", new { jobSyId = jobSummaryId, sPageNo = summaryPageNo, sPageSize = summaryPageSize, dPageNo = detailPageNo, dPageSize = detailPageSize });

                }
                var filefullPath = string.Format(@"{0}{1}\{2}", rootFolder, jobId, zipFile);
                
                if (!System.IO.File.Exists(filefullPath))
                {
                    TempData["DownloadFileError"] = "File Not Exists On Specified location ";

                    return RedirectToAction("DownloadRetrievedFiles", new { jobSyId = jobSummaryId, sPageNo = summaryPageNo, sPageSize = summaryPageSize, dPageNo = detailPageNo, dPageSize = detailPageSize });
                }

                string fileName = System.IO.Path.GetFileName(filefullPath);

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

                //var tempFolderPath = @"C:\TempDownloadFolder";

                //// Create Temp Folder in C :\ to copy network files 
                //if (!Directory.Exists(tempFolderPath))
                //{
                //    Directory.CreateDirectory(tempFolderPath);
                //}

                //// Delete Oldest files from Temp Folder
                //DeleteOldestFilesFromTempFolder(tempFolderPath);

                ////Copy Network file into Temp Folder for Download
                //System.IO.File.Copy(filefullPath, tempFolderPath + @"\" + System.IO.Path.GetFileName(filefullPath), true);

                ////FileLocation = System.IO.Path.GetFullPath(FileLocation);
                //filefullPath =
                //    System.IO.Path.GetFullPath(tempFolderPath + @"\" + System.IO.Path.GetFileName(filefullPath));

                return File(filefullPath, contentType, System.IO.Path.GetFileName(filefullPath));

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

            var u = new UrlHelper(this.ControllerContext.RequestContext);
            return Redirect(u.Action("DownloadRetrievedFiles", "ArchiveRetrieval", "http"));
        }

        ///// <summary>
        ///// delete all old files for folder
        ///// </summary>
        ///// <param name="path">folder path</param>
        ///// <returns></returns>
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
    }
}