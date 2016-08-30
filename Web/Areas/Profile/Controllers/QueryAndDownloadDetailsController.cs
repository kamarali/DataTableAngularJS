using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Iata.IS.Business.Common;
using Iata.IS.Business.Pax;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.ExtensionHelpers;
using Iata.IS.Web.Util.Filters;
using iPayables.UserManagement;
using log4net;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
    public class QueryAndDownloadDetailsController : ISController
    {
        private readonly IQueryAndDownloadDetailsManager _queryAndDownloadDetailsManager;
        private readonly IReferenceManager _referenceManager;
        private readonly IUserManagement _iUserManagement;
        private readonly IMemberManager _memberManager;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public QueryAndDownloadDetailsController(IQueryAndDownloadDetailsManager queryAndDownloadDetailsManager, IReferenceManager referenceManager, IUserManagement iUserManagement, IMemberManager memberManager)
        {
            _queryAndDownloadDetailsManager = queryAndDownloadDetailsManager;
            _referenceManager = referenceManager;
            _iUserManagement = iUserManagement;
            _memberManager = memberManager;
        }

        /// <summary>
        /// Get member details view.
        /// </summary>
        /// <returns></returns>
        [ISAuthorize(Business.Security.Permissions.Reports.MemberContactReports.Access)]
        public ActionResult MemberDetails()
        {
            // Retrieve memberId from Session and use its value across the method
            var memberId = SessionUtil.MemberId;

            //Get user category for logged in user.
            int userCategoryId = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
            ViewData["UserCategoryID"] = userCategoryId;
            ViewData["MemberId"] = Convert.ToInt32(memberId);
            if (userCategoryId == (int)Model.MemberProfile.Enums.UserCategory.SisOps)
            {
                ViewData["AllowContactDetailsDownload"] = true;
            }
            else
            {
                //Get member details for logged in user.
                Member member = _memberManager.GetMember(Convert.ToInt32(memberId));
                if (member != null)
                    ViewData["AllowContactDetailsDownload"] = member.AllowContactDetailsDownload;
                else
                    ViewData["AllowContactDetailsDownload"] = false;
            }

            ////Set ViewData for contact type groups, contact type sub groups and contact types
            //SetContactTypeData();

            //Get all member profile metadata.
            ProfileMetadataAvailableFields[] profileMetadataAvailableFields = _queryAndDownloadDetailsManager.GetProfileMetadataAvailableFields(userCategoryId, 1, false).ToArray();
            ViewData["ProfileMetadataAvailableFields"] = new MultiSelectList(profileMetadataAvailableFields, "MetaId", "MetaName");

            return View();
        }

        public JsonResult GetAvailableMemberProfileMetadata(int reportType, string memberId)
        {
            //Get user category for logged in user.
            int userCategoryId = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;
            int selectedMemberId;
            bool isOwnMember = Int32.TryParse(memberId, out selectedMemberId) ? SessionUtil.MemberId.Equals(selectedMemberId) : false;
            ProfileMetadataAvailableFields[] profileMetadataAvailableFields = _queryAndDownloadDetailsManager.GetProfileMetadataAvailableFields(userCategoryId, reportType, isOwnMember).ToArray();

            return Json(new MultiSelectList(profileMetadataAvailableFields, "MetaId", "MetaName"));
        }

        public JsonResult GetContactTypes(string groupIds, string subGroupIds)
        {
            //Convert comma separated group ids into list of integers.
            var grpIdList = new List<int>();
            if (groupIds.Trim().Length > 0)
                grpIdList = groupIds.Split(',').Select(s => int.Parse(s)).ToList();

            //Convert comma separated sub group ids into list of integers.
            var subGrpIdList = new List<int>();
            if (subGroupIds.Trim().Length > 0)
                subGrpIdList = subGroupIds.Split(',').Select(s => int.Parse(s)).ToList();

            var contactTypes = _queryAndDownloadDetailsManager.GetContactTypeList(grpIdList, subGrpIdList);

            return Json(contactTypes);
        }

        [HttpPost]
        public JsonResult GetContactTypeSubGroups(string groupIds)
        {
            //Convert comma separated group ids into list of integers.
            var grpIdList = new List<int>();
            if (groupIds.Trim().Length > 0)
                grpIdList = groupIds.Split(',').Select(s => int.Parse(s)).ToList();

            var contactTypeSubGroups = _referenceManager.GetContactTypeSubGroupList(grpIdList);

            return Json(contactTypeSubGroups);
        }

        /// <summary>
        /// Get contact names and contact email information for given member id.
        /// </summary>
        /// <param name="memberId">member id.</param>
        /// <returns>List of contact names and contact email with contact id information for given member id.</returns>
        public JsonResult GetContactDetailsForMember(string memberId)
        {
            var memberContactList = _memberManager.GetMemberContactList(Convert.ToInt32(memberId));

            //Get contact names
            var distinctContactNames = memberContactList.Select(con => new
            {
                con.Id,
                con.Name
            });
            if (distinctContactNames.Count() > 1)
            {
                distinctContactNames = distinctContactNames.GroupBy(con => con.Name).Select(g => g.First()).OrderBy(con => con.Name);
            }

            //Get contact email addresses.
            var distinctContactEmails = memberContactList.Select(con => new
            {
                con.Id,
                Email = con.EmailAddress
            });
            if (distinctContactEmails.Count() > 1)
            {
                distinctContactEmails = distinctContactEmails.GroupBy(con => con.Email).Select(g => g.First()).OrderBy(con => con.Email);
            }

            return Json(new { ContactName = distinctContactNames, ContactEmail = distinctContactEmails });
        }

        /// <summary>
        /// Perform search based on given search criteria.
        /// </summary>
        /// <param name="userCategoryId"></param>
        /// <param name="reportType"></param>
        /// <param name="metaIdList">list of viewable meta ids.</param>
        /// <param name="typeMetaIdList"></param>
        /// <param name="memberId">member id</param>
        /// <param name="isIch">boolean flag to filter ich members.</param>
        /// <param name="isAch">boolean flag to filter ach members.</param>
        /// <param name="isIata">boolean flag to filter iata members.</param>
        /// <param name="isDual">boolean flag to filter dual members.</param>
        /// <param name="isNonCh">boolean flag to filter non-ch members.</param>
        /// <param name="countryId">country id.</param>
        /// <param name="contactId"></param>
        /// <param name="emailId">email address.</param>
        /// <param name="groupIdList">contact group id list.</param>
        /// <param name="subGroupIdList">contact sub group id list.</param>
        /// <param name="contactTypeIdList">contact type id list.</param>
        /// <param name="sortIds">sort meta id list.</param>
        /// <param name="sortOrder">sort order list.</param>
        [HttpPost]
        public JsonResult Search(string userCategoryId, string reportType, string metaIdList, string typeMetaIdList, string memberId, string countryId, bool isIch, bool isAch, bool isIata, bool isDual, bool isNonCh, string contactId, string emailId, string groupIdList, string subGroupIdList, string contactTypeIdList, string sortIds, string sortOrder)
        {

            var searchCriteria = GetSearchCriteria(userCategoryId, reportType, metaIdList, typeMetaIdList, memberId, countryId, isIch, isAch, isIata, isDual, isNonCh, contactId, emailId, groupIdList, subGroupIdList, contactTypeIdList, sortIds, sortOrder);

            TempData["SearchCriteria"] = searchCriteria;

            var jsonString = "";

            if (reportType == "1" || reportType == "2")
            {
                var metaNameList = _queryAndDownloadDetailsManager.GetMemberProfileFieldNames(searchCriteria.MetaIdList);
                jsonString = JsonHelper.JsonForJqgridColumns(metaNameList);
            }
            else if (reportType == "3")
            {
                searchCriteria.MetaIdList = "";
                int totalRecordCount;
                var resultSet = _queryAndDownloadDetailsManager.SearchMemberContactDetails(searchCriteria, true, out totalRecordCount, false);
                jsonString = string.Format("[{{\"Html\":\"{0}\"}}]", _queryAndDownloadDetailsManager.ConvertDataTableToHtmlString(resultSet));
            }

            return Json(jsonString);
        }

        private static QueryAndDownloadSearchCriteria GetSearchCriteria(string userCategoryId, string reportType, string metaIdList, string typeMetaIdList, string memberId, string countryId, bool isIch, bool isAch, bool isIata, bool isDual, bool isNonCh, string contactId, string emailId, string groupIdList, string subGroupIdList, string contactTypeIdList, string sortIds, string sortOrder)
        {
            int selectedMemberId;
            return new QueryAndDownloadSearchCriteria
            {
                UserCategoryId = userCategoryId,
                ReportType = reportType,
                ISOwnMember = Int32.TryParse(memberId, out selectedMemberId) ? SessionUtil.MemberId.Equals(selectedMemberId) : false,
                MetaIdList = metaIdList,
                TypeMetaIdList = typeMetaIdList,
                MemberId = memberId,
                CountryId = countryId,
                ISIch = isIch,
                ISAch = isAch,
                ISIata = isIata,
                ISDual = isDual,
                ISNonCh = isNonCh,
                ContactId = contactId,
                EmailId = emailId,
                GroupIdList = groupIdList,
                SubGroupIdList = subGroupIdList,
                ContactTypeIdList = contactTypeIdList,
                SortIds = sortIds,
                SortOrder = sortOrder
            };
        }

        [ValidateAntiForgeryToken]
        public ActionResult DownloadReport(FormCollection formCollection)
        {
            var metaIds = formCollection["selectedMetaList"].Split('#');
            var selectedContactType = metaIds.Length == 2 ? metaIds[1].Replace("T", "") : "";
            int selectedMemberId;
            var searchCriteria = new QueryAndDownloadSearchCriteria
            {
                MemberId = formCollection["id"],
                MetaIdList = metaIds[0],
                TypeMetaIdList = metaIds.Length == 2 ? metaIds[1] : "",
                UserCategoryId = formCollection["userCategoryID"],
                ISOwnMember = Int32.TryParse(formCollection["id"], out selectedMemberId) ? SessionUtil.MemberId.Equals(selectedMemberId) : false,
                ReportType = formCollection["reportType"],
                CountryId = formCollection["Country"],
                ContactId = formCollection["ContactName"] != null ? formCollection["ContactName"].TrimEnd(new[] { ',' }) : null,
                EmailId = formCollection["Email"] != null ? formCollection["Email"].TrimEnd(new[] { ',' }) : null,
                ISAch = Convert.ToBoolean(formCollection["ach"].Replace(",false", "")),
                ISIch = Convert.ToBoolean(formCollection["ich"].Replace(",false", "")),
                ISIata = Convert.ToBoolean(formCollection["iata"].Replace(",false", "")),
                ISNonCh = Convert.ToBoolean(formCollection["nonch"].Replace(",false", "")),
                ISDual = Convert.ToBoolean(formCollection["dual"].Replace(",false", "")),
                ContactTypeIdList = selectedContactType,//formCollection["contactType"],
                GroupIdList = formCollection["contactTypeGroup"],
                SubGroupIdList = formCollection["contactTypeSubGroup"],
                SortIds = formCollection["sortIds"],
                SortOrder = formCollection["sortOrder"]
            };

            //if (!(string.IsNullOrEmpty(searchCriteria.EmailId) && string.IsNullOrEmpty(searchCriteria.ContactId)))
            //{
            //    searchCriteria.GroupIdList = string.Empty;
            //    searchCriteria.SubGroupIdList = string.Empty;
            //    searchCriteria.ContactTypeIdList = string.Empty;
            //}

            var outputPath = "";
            var outputFileName = "";
            var contentType = "";
            var guid = Guid.NewGuid().ToString();

            int totalRecordCount;
            if (searchCriteria.ReportType == "1" || searchCriteria.ReportType == "2")
            {
                outputFileName = searchCriteria.ReportType == "1" ? "MemberDetails" : "ContactDetails";
                var resultSet = _queryAndDownloadDetailsManager.SearchMemberContactDetails(searchCriteria, false, out totalRecordCount, true);
                var result = resultSet.Tables[0];
                if (result.Columns.Contains("MEMBER_ID")) result.Columns.Remove("MEMBER_ID");
                outputPath = _queryAndDownloadDetailsManager.ConvertDataTableToCsv(result, string.Format(@"{0}_{1}", outputFileName, guid), ",");
                contentType = "text/x-csv";
                outputFileName += ".csv";

                if (!System.IO.File.Exists(outputPath))
                {
                    ShowErrorMessage("Output file does not exists.", true);
                    return RedirectToAction("MemberDetails", "QueryAndDownloadDetails");
                }
            }
            else if (searchCriteria.ReportType == "3")
            {

                var inputHtml = _queryAndDownloadDetailsManager.SearchMemberContactDetails(searchCriteria, out totalRecordCount, false);
                var htmlToPdfExePath = Request.PhysicalApplicationPath + @"bin\wkhtmltopdf.exe";
                Logger.InfoFormat(@"HtmlToPdfExePath: {0}", htmlToPdfExePath);
                var file = _queryAndDownloadDetailsManager.ConvertHtmlToPdf(inputHtml, string.Format(@"ContactDetails_{0}", guid), htmlToPdfExePath);
                contentType = "application/pdf";
                outputFileName = "ContactDetails.pdf";
                if (file != null)
                {
                    return File(file, contentType, Server.HtmlEncode(outputFileName));
                }
                ShowErrorMessage("Output file does not exists.", true);
                return RedirectToAction("MemberDetails", "QueryAndDownloadDetails");
            }

            return File(outputPath, contentType, Server.HtmlEncode(outputFileName));
        }


        [ISAuthorize(Business.Security.Permissions.ISOps.ManageMasters.DownloadMemberProfileInfo)]
        public ActionResult DownloadReportForMemberdetails()
        {
            var outputPath = "";
            var outputFileName = "MemberDetails";
            var contentType = "";
            var guid = Guid.NewGuid().ToString();
            var resultSet = _queryAndDownloadDetailsManager.GetMemberDetails();
            var result = resultSet.Tables[0];
            outputPath = _queryAndDownloadDetailsManager.ConvertDataTableToCsv(result, string.Format(@"{0}_{1}", outputFileName, guid), ",");
            contentType = "text/x-csv";
            outputFileName += ".csv";

            if (!System.IO.File.Exists(outputPath))
            {
                ShowErrorMessage("Output file does not exists.", true);
                return RedirectToAction("MemberDetails", "QueryAndDownloadDetails");
            }
            return File(outputPath, contentType, Server.HtmlEncode(outputFileName));
        }

        public LargeContentResult GetMyGridData(string sidx, string sord, int page, int rows, FormCollection formCollection)
        {
            QueryAndDownloadSearchCriteria searchCriteria;
            if (TempData["SearchCriteria"] != null)
            {
                searchCriteria = (QueryAndDownloadSearchCriteria)TempData["SearchCriteria"];

                //Set sort parameters if specified.
                if (!string.IsNullOrEmpty(sidx) && !string.IsNullOrEmpty(sord))
                {
                    searchCriteria.SortIds = sidx;
                    searchCriteria.SortOrder = sord;
                }

                //Restore the search criteria for future use.
                TempData["SearchCriteria"] = searchCriteria;
                int totalRecordCount;
                return new LargeContentResult { Content = JsonHelper.JsonForJqgrid(GetMyDataTable(sidx, sord, page, rows, searchCriteria, out totalRecordCount), rows, totalRecordCount, page), MaxJsonLength = int.MaxValue };
            }
            // todo: handle else condition if TempData["SearchCriteria"] is null
            return new LargeContentResult { Content = JsonHelper.JsonForJqgrid(new DataTable(), rows, 0, page), MaxJsonLength = int.MaxValue };
        }

        public DataTable GetMyDataTable(string sidx, string sord, int page, int pageSize, QueryAndDownloadSearchCriteria searchCriteria, out int totalRecordCount)
        {
            searchCriteria.PageNumber = page.ToString();
            searchCriteria.PageSize = pageSize.ToString();

            DataTable dt = _queryAndDownloadDetailsManager.SearchMemberContactDetails(searchCriteria, true, out totalRecordCount, false).Tables[0];
            if (dt.Columns.Contains("RN")) dt.Columns.Remove("RN");
            if (dt.Columns.Contains("MEMBER_ID")) dt.Columns.Remove("MEMBER_ID");
            return dt;
        }

    }
}