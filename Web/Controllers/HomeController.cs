using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using System.Xml.Serialization;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Business.Common;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Common;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.Reports.Enums;
using Iata.IS.Web.UIModel.Grid.Common;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using iPayables.UserManagement;
using System.IO;
using log4net;
using Iata.IS.Business.MemberProfile.Impl;


namespace Iata.IS.Web.Controllers
{
    [Authorize]
    [LogActions]
    [ElmahHandleError]
    public class HomeController : ISController
    {
        public IMemberManager MemberManager { get; set; }

        public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }

        private readonly ICalendarManager _calenderManager;

        public IMemberRepository Member { get; set; }

        public IPermissionManager PermissionManager { get; set; }

        public IAlertMessageNotesRepositiory MsgRepository { get; set; }

        private string CurrentUserName { get; set; }

        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public HomeController(ICalendarManager calenderManager)
        {
            _calenderManager = calenderManager;
        }

        [HomeAuthorizeAttribute]
        public ActionResult Index()
        {
            //ShouldDisplayLogoutOptionProxy();
          
            var gridAlert = new AlertsGrid("AlertsGridPopUp", Url.Action("GetAlertsDataPopup", "Home"));
            var gridMessage = new MessagesGrid("MessagesGridPopUp", Url.Action("GetMessagesDataPopup", "Home"));

            ViewData["AlertsGridPopUp"] = gridAlert.Instance;
            ViewData["MessagesGridPopUp"] = gridMessage.Instance;
            ViewData["Popup"] = 1;
            SessionUtil.AlertRequired = "YES";
            ViewData["Announcement"] =GetAlertsMessagesAnnouncements((int) MessageType.Announcement).OrderByDescending(a => a.FromDate).ToList();
            
            return View();
        }

        private List<AlertsMessagesAnnouncementsResultSet> GetAlertsMessagesAnnouncements(int type)
        {

          var msgList = new List<AlertsMessagesAnnouncementsResultSet>();

          //SCP197305: BROADCAST MESSAGE
          if (SessionUtil.UserCategory == UserCategory.SisOps && type == (int)MessageType.Alert)
          {
            msgList = BroadcastMessagesManager.GetSisOpsAlerts(DateTime.UtcNow).ToList();
            //Alert call is coming in sequence first for alert(type 3) and then for messages (type 2). Session AlertRequired will be null after message call
            //to avoid calling in other postback calls.
            if (type == 2)
              SessionUtil.AlertRequired = null;
            //Keeping session 'Announcement' to hold previous data for alert & messages
            SessionUtil.AlertMessagesData = msgList;
            return msgList;
          }
          string memberType = string.Empty;

          if (type == (int)MessageType.Announcement)
          {
            msgList = MsgRepository.GetAlertsMessagesAnnouncements(type, memberType, SessionUtil.UserId,
                                                                   DateTime.UtcNow);
          }
          else
          {
            var memberId = SessionUtil.MemberId;

            if (memberId > 0)
            {
              var member = Member.GetMember(memberId);

              member.IchMemberStatus = (member.IchMemberStatusId == (int)IchMemberShipStatus.Live ||
                                        member.IchMemberStatusId == (int)IchMemberShipStatus.Suspended)
                                         ? true
                                         : false;

              member.AchMemberStatus = (member.AchMemberStatusId == (int)AchMembershipStatus.Live ||
                                        member.AchMemberStatusId == (int)AchMembershipStatus.Suspended)
                                         ? true
                                         : false;

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

            }
            else
            {
              var category = SessionUtil.UserCategory;
              if (category == UserCategory.AchOps && category == UserCategory.IchOps)
              {
                memberType = "B";
              }
              else if (category == UserCategory.AchOps)
              {
                memberType = "A";
              }
              else if (category == UserCategory.IchOps)
              {
                memberType = "I";
              }
              else
              {
                memberType = "N";
              }

            }
            msgList = MsgRepository.GetAlertsMessagesAnnouncements(type, memberType, SessionUtil.UserId,
                                                                   DateTime.UtcNow);
          }

          return msgList;

        }

      public void ShouldDisplayLogoutOptionProxy()
        {
            // Retrieve UserId from Session and use it across the method
            var iUserId = 0;


            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
            {
                iUserId = int.Parse(SessionUtil.UserId.ToString());
            }

            ViewData["Message"] = "Welcome to Simplified Interline Settlement!";
            if (iUserId <= 0)
            {
                //IUserManagement userManagement = new UserManagementModel();
                //I_ISUser user = userManagement.GetUserByUserID(userId);
                //CurrentUserName = user.FirstName;
                //SessionUtil.Username = CurrentUserName;

                //if (userId > 0 && UserManagementModel.GetUserByUserIDStatic(userId).UserID != userId)
                //{
                //  SessionUtil.IsLogOutProxyOption = true;
                //}

                throw new ISSessionExpiredException("Session expired.");
            }
        }

        [HttpPost]
        public ActionResult GetAnnouncementsData()
        {
            SessionUtil.AlertRequired = "YES";
            var announcementData = GetAlertsMessagesAnnouncements((int)MessageType.Announcement).OrderByDescending(a => a.FromDate).ToList();

            foreach (var data in announcementData)
            {
                data.FromDate = CalendarManager.ConvertUtcTimeToYmq(data.FromDate);
            }
            return PartialView("Announcements", announcementData);
        }

        public ActionResult GetAlerts()
        {
            var gridAlert = new AlertsMessagesGrid("AlertsGrid", Url.Action("GetAlertsData", "Home"), (int)MessageType.Alert);

            return PartialView("Alerts", gridAlert.Instance);
        }

        public JsonResult GetAlertsData()
        {
            SessionUtil.AlertRequired = "YES";
            var gridAlert = new AlertsMessagesGrid("AlertsGrid", Url.Action("GetAlertsData", "Home"), (int)MessageType.Alert);
            var gridData = GetAlertsMessagesAnnouncements((int)MessageType.Alert).OrderBy(a => a.RAGIndicator).OrderByDescending(a => a.RaisedDate);

            foreach (var data in gridData)
            {
                data.RaisedDate = CalendarManager.ConvertUtcTimeToYmq(data.RaisedDate);

                if (data.Detail.Length > 60)
                {
                    data.Detail = String.Format("{0}...", data.Detail.Substring(0, 57));
                }
            }
            return gridAlert.DataBind(gridData.AsQueryable());
        }

        public JsonResult GetAlertsDataPopup()
        {
            SessionUtil.AlertRequired = "YES";
            var gridAlert = new AlertsGrid("AlertsGrid", Url.Action("GetAlertsDataPopup", "Home"));

            var gridData =
              GetAlertsMessagesAnnouncements((int)MessageType.Alert).OrderBy(a => a.RAGIndicator).OrderByDescending(a => a.RaisedDate);

            foreach (var data in gridData)
            {
                data.RaisedDate = CalendarManager.ConvertUtcTimeToYmq(data.RaisedDate);
            }

            return gridAlert.DataBind(gridData.AsQueryable());
        }

        public ActionResult GetMessages()
        {
            var gridMessage = new AlertsMessagesGrid("MessagesGrid", Url.Action("GetMessagesData", "Home"), (int)MessageType.Message);

            return PartialView("Messages", gridMessage.Instance);
        }

        [HttpPost]
        public JsonResult ClearMessage(Guid id)
        {
            if (SessionUtil.UserCategory == UserCategory.SisOps)
            {
                BroadcastMessagesManager.ClearSisOpsAlertMessage(id);
            }
            else
            {
                MsgRepository.ClearAlertMessage(id, SessionUtil.UserId);
            }
            return new JsonResult();
        }

        /// <summary>
        /// Home screen, Check user active session
        /// </summary>
        /// <returns></returns>
        public ActionResult CheckUserActiveSession()
        {
            if (SessionUtil.UserId <= 0)
            {
                return Json(new { redirect = Url.Action("LogOn", "Account", new { area = string.Empty }) });
            }

            return Json(new { samepage = true });
        }


        [HttpPost]
        public JsonResult GetAlertMessagesCount()
        {
            return Json(new { Alert = GetAlertsMessagesAnnouncements((int) MessageType.Alert).Count(),Message = GetAlertsMessagesAnnouncements((int)MessageType.Message).Count() });
        }

        public ActionResult SetSessionToManageAlerts(string key, string value)
        {
            //Session[key] = value;
            SessionUtil.AlertRequired = "YES";
            return this.Json(new { success = true });
        }

        public JsonResult GetMessagesData()
        {
            SessionUtil.AlertRequired = "YES";
            var gridMessage = new AlertsMessagesGrid("MessagesGrid", Url.Action("GetMessagesData", "Home"), (int)MessageType.Message);

            var gridData = GetAlertsMessagesAnnouncements((int)MessageType.Message).OrderByDescending(a => a.ExpiryDate);

            foreach (var data in gridData)
            {
                data.FromDate = CalendarManager.ConvertUtcTimeToYmq(data.FromDate);

                if (data.Detail.Length >= 57)
                {
                    data.Detail = String.Format("{0}...", data.Detail.Substring(0, 56));
                }
            }

            return gridMessage.DataBind(gridData.AsQueryable());
        }

        public JsonResult GetMessagesDataPopup()
        {
            SessionUtil.AlertRequired = "YES";
            var gridMessage = new MessagesGrid("MessagesGrid", Url.Action("GetMessagesDataPopup", "Home"));

            var gridData = GetAlertsMessagesAnnouncements((int)MessageType.Message).OrderByDescending(a => a.ExpiryDate);

            foreach (var data in gridData)
            {
                data.Detail =  HttpUtility.HtmlEncode(data.Detail);
                data.FromDate = CalendarManager.ConvertUtcTimeToYmq(data.FromDate);
            }
            return gridMessage.DataBind(gridData.AsQueryable());
        }

        public ActionResult Error(string title)
        {
            return View("Error");
        }

        public ActionResult About()
        {
            if (ConfigurationManager.AppSettings.Get("SkyMagic") == "1")
            {
                return View();
            }

            return RedirectToAction("Index");
        }

        public ActionResult SetUserSession()
        {
            string billingMemberId = Request.Form["BillingMemberId"];

            if (!string.IsNullOrEmpty(billingMemberId))
            {
                SessionUtil.MemberId = Convert.ToInt32(billingMemberId);
            }

            return View("About");
        }

        public ActionResult EnableWarningMessages(bool unsavedWarningMessages)
        {
            SessionUtil.UnsavedWarningMessagesEnabled = unsavedWarningMessages;

            return View("About");
        }

        [HttpGet]
        public ActionResult CacheKeys()
        {
            const string keyListKey = "KeyList";
            var cacheManager = Ioc.Resolve<ICacheManager>();

            var keysList = cacheManager.Get(keyListKey) as StringCollection;

            if (keysList != null)
            {
                keysList.Remove(keyListKey);
            }

            return View(keysList);
        }

        [HttpGet]
        public ActionResult RemoveCacheKey(string key)
        {
            var cacheManager = Ioc.Resolve<ICacheManager>();

          

            cacheManager.Remove(key);

            return RedirectToAction("CacheKeys");
        }
      
       public JsonResult GetCacheObjectDetails(string key)
        {
            string returnObjectDetails;
           try
           {
               var cacheManager = Ioc.Resolve<ICacheManager>();
               var cache = cacheManager.Get(key);

               var x = new XmlSerializer(cache.GetType());
             
               using (var writer = new StringWriter())
               {
                   x.Serialize(writer, cache);
                   returnObjectDetails = writer.ToString();
               }
           }
           catch (Exception exception)
           {
               returnObjectDetails = "Error Occured While Serializing the Cache Object";

           }
           

           return Json(returnObjectDetails);
        }



        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult GetIsEvents()
        {
            const string eventCategory = "IS";
            var gridIsEvent = new UpcomingEventsGrid("UpcomingEventsGrid", Url.Action("GetIsEventData", "Home", new { eventCategory }));
            var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            ViewData["currentPeriod"] = EnumList.GetMonthDisplayValue((Month)currentPeriod.Month) + " " + currentPeriod.Year + " P" + currentPeriod.Period;
            return PartialView("UpcomingEventsGridCOntrol", gridIsEvent.Instance);
        }

        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult GetIchEvents()
        {
            const string eventCategory = "ICH";
            var gridIsEvent = new UpcomingEventsGrid("UpcomingEventsGrid", Url.Action("GetIsEventData", "Home", new { eventCategory }));
            var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            ViewData["currentPeriod"] = EnumList.GetMonthDisplayValue((Month)currentPeriod.Month) + " " + currentPeriod.Year + " P" + currentPeriod.Period;
            return PartialView("ICHUpcomingEventsGridCOntrol", gridIsEvent.Instance);
        }
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult GetAchEvents()
        {
            const string eventCategory = "ACH";
            var gridIsEvent = new UpcomingEventsGrid("UpcomingEventsGrid", Url.Action("GetIsEventData", "Home", new { eventCategory }));
            var currentPeriod = _calenderManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ach);
            ViewData["currentPeriod"] = EnumList.GetMonthDisplayValue((Month)currentPeriod.Month) + " " + currentPeriod.Year + " P" + currentPeriod.Period;
            return PartialView("ACHUpcomingEventsGridControl", gridIsEvent.Instance);
        }

        public JsonResult GetIsEventData(string eventCategory, string timeZoneId)
        {
            var gridAlert = new UpcomingEventsGrid("UpcomingEventsGrid", Url.Action("GetIsEventData", "Home", new { eventCategory }));
            var gridData = _calenderManager.GetUpComingIsCalendarList(eventCategory, 5, SessionUtil.TimeZone);
            return gridAlert.DataBind(gridData.AsQueryable());
            //return gridAlert.DataBind(null);
        }

        private IEnumerable<UpcomingEventsResultSet> GetIsUpcomingEvents()
        {
            var eventsList = new List<UpcomingEventsResultSet>();
            var baseDate = new DateTime(2011, 4, 23, 23, 59, 0);
            baseDate = baseDate.AddDays(1 * 7);
            var isEvent = new UpcomingEventsResultSet();

            isEvent.Period = "May 2011 P1";
            isEvent.EventDescription = "Test Event";
            isEvent.YmqDateTime = System.DateTime.UtcNow;
            isEvent.LocalDateTime = System.DateTime.UtcNow;
            eventsList.Add(isEvent);

            return eventsList;
        }

        public ActionResult RedirectToHelpContent(string requestUrl)
        {


            string localpath = Request.Url.LocalPath;
            int index1 = localpath.IndexOf("Home", 0);
            string remainpath = localpath.Substring(0, (index1 - 1));
            requestUrl = requestUrl.Remove(0, remainpath.Length);
            _logger.DebugFormat("remainpath" + remainpath);
            _logger.DebugFormat("requestUrl" + requestUrl);

            string appArea = string.Empty;
            string appController = string.Empty;
            string controllerAction = string.Empty;
            string redirectHelpContent = "MemberUserGuide/Topic_Under_Construction.htm"; // Default Home Page
            int isnumber = 0;
            string[] splitUrl = requestUrl.Split(Convert.ToChar("/"));
            int strlength = splitUrl.Length;
            appArea = splitUrl[1];

            if (splitUrl[strlength - 1].Length >= 32)
            {
              
                if (splitUrl[strlength - 1].Any(s => char.IsNumber(s) || char.IsDigit(s)))
                {
                    isnumber = 1;
                }
            }
            if (isnumber == 1)
            {
                controllerAction = splitUrl[strlength - 2];
            }
            else
            {
                controllerAction = splitUrl[strlength - 1];
            }


            if (controllerAction.Contains("?"))
            {
                int index = controllerAction.IndexOf('?');
                controllerAction = controllerAction.Substring(0, (index - 1));
            }

            if (controllerAction.Length >= 32 && isnumber == 1)
            {
                controllerAction = splitUrl[strlength - 3];

                if (controllerAction.Contains("?"))
                {
                    int index = controllerAction.IndexOf('?');
                    controllerAction = controllerAction.Substring(0, (index - 1));
                }
            }


            if (strlength > 5)
            {
                strlength = 5;
            }

            //bool flag = false;
            //if (splitUrl[1].Contains("IS-WEB-SIT") || splitUrl[1].Contains("IS-WEB-UAT") || splitUrl[1].Contains("is-web-postreview"))
            //{
            //    flag = true;
            //    appArea = splitUrl[2];
            //}
            //if (flag)
            //{
            //    while (strlength > 6)
            //    {
            //        strlength = 6;
            //    }
            //}
            //else
            //{
            //    while (strlength > 5)
            //    {
            //        strlength = 5;
            //    }
            //}

            appController = splitUrl[strlength - 2];

            if (appController.ToLower() == "formc")
            {
                if (splitUrl[strlength - 1].ToLower() == "viewdetails")
                    controllerAction = "viewdetails";
                if (splitUrl[strlength - 1].ToLower() == "couponview")
                    controllerAction = "couponview";
            }

            if (controllerAction == "EditCorrespondence" && appArea == "Misc")
                appController = splitUrl[strlength - 3];

            if (controllerAction == "OpenCorrespondenceForEdit" && appArea == "Misc")
                appController = "Correspondence";

            if (controllerAction == "CreateRejectionInvoice" && appArea == "Misc")
                appController = "MiscInvoice";

            //if (controllerAction == "RMCouponCreate" && appArea == "Pax")
            //    appController = splitUrl[strlength - 3];

            if (controllerAction.ToLower() == "creditmemoedit")
            {
                if (splitUrl[strlength - 3].ToLower() == "cmedit")
                    controllerAction = "cmedit";
            }

            if (appController.ToLower() == "createbillingmemo")
            {
                controllerAction = "Correspondence";
            }


            #region legalArchive

            if (appArea.ToLower() == "legalarchive")
            {
                if (appController.ToLower() == "archiveretrieval")
                {
                    if (controllerAction.ToLower() == "search")
                        redirectHelpContent = "General/Retrieving_Legal_Archive.htm";

                    if (controllerAction.ToLower() == "downloadretrievedfiles")
                        redirectHelpContent = "General/Downloading_Retrieved_Invoices.htm";
                }
            }

            #endregion


            #region cargo

            if (appArea == "Cargo")
            {
              if (appController.ToLower() == "cargomanageinvoice" || appController.ToLower() == "manageinvoice")
                 {
                     if (controllerAction.ToLower() == "index")
                         redirectHelpContent = "Cargo/Searching_an_Invoice.htm";
                 }

                 else if (appController.ToLower() == "invoice")
                 {
                     switch (controllerAction.ToLower())
                     {
                         case "create":
                             redirectHelpContent = "Cargo/Creating_Invoice_Header.htm";
                             break;
                         case "view":
                             redirectHelpContent = "Cargo/Viewing_an_Invoice.htm";
                             break;
                         case "edit":
                             redirectHelpContent = "Cargo/Editing_Invoice_Header.htm";
                             break;
                         case "awbprepaidbillingcreate":
                             redirectHelpContent = "Cargo/Creating_Prepaid_AWB_.htm";
                             break;
                         case "awbprepaidbillinglist":
                             redirectHelpContent = "Cargo/AWB_Prepaid_Billing_Records.htm";
                             break;
                         case "awbprepaidrecordedit":
                             redirectHelpContent = "Cargo/Editing_Prepaid_AWB.htm";
                             break;
                         case "awbchargecollectbillingcreate":
                             redirectHelpContent = "Cargo/Creating_Charges_Collect_AWB.htm";
                             break;
                         case "awbchargecollectbillinglist":
                             redirectHelpContent = "Cargo/AWB_Charge_Collect_Billing_Records.htm";
                             break;
                         case "awbchargecollectrecordedit":
                             redirectHelpContent = "Cargo/Editing_AWB_Charge_Collect_Billing.htm";
                             break;
                         case "rmcreate":
                             redirectHelpContent = "Cargo/Creating_a_Rejection_Memo.htm";
                             break;
                         case "rmlist":
                             redirectHelpContent = "Cargo/Rejection_Memo_Listing.htm";
                             break;
                         case "rmedit":
                             redirectHelpContent = "Cargo/Editing_Rejection_Memo.htm";
                             break;
                         case "rmprepaidawbcreate":
                             redirectHelpContent = "Cargo/Adding_Prepaid_AWB_Billing_for_RM.htm";
                             break;
                         case "rmprepaidawbedit":
                             redirectHelpContent = "Cargo/Editing_Prepaid_AWB_for_RM.htm";
                             break;
                         case "rmchargecollectawbcreate":
                             redirectHelpContent = "Cargo/Adding_Charges_Collect_AWB_for_RM.htm";
                             break;
                         case "rmchargecollectawbedit":
                             redirectHelpContent = "Cargo/Editing_Charges_Collect_AWB_for_RM.htm";
                             break;
                         case "bmcreate":
                             redirectHelpContent = "Cargo/Creating_Billing_Memo.htm";
                             break;
                         case "bmedit":
                             redirectHelpContent = "Cargo/Editing_Billing_Memo.htm";
                             break;
                         case "bmawbprepaidcreate":
                             redirectHelpContent = "Cargo/Adding_Prepaid_AWB_Billing_for_BM.htm";
                             break;
                         case "bmawbedit":
                             redirectHelpContent = Session["helplinkurl"].ToString() == "Bm_Awb_Prepaid_edit"
                                                       ? "Cargo/Editing_Prepaid_AWB_for_BM.htm"
                                                       : "Cargo/Editing_Charges_Collect_AWB_for_BM.htm";

                             Session["helplinkurl"] = null;
                             break;
                         case "bmawbchargecollectcreate":
                             redirectHelpContent = "Cargo/Adding_Charges_Collect_AWB_for_BM.htm";
                             break;
                         case "bmlist":
                             redirectHelpContent = "Cargo/Billing_Memo_Listing.htm";
                             break;
                         case "vatview":
                             redirectHelpContent = "Cargo/Invoice_VAT.htm";
                             break;
                         case "awbprepaidbillinglistview":
                             redirectHelpContent = "Cargo/AWB_Prepaid_Billing_Records.htm";
                             break;
                         case "awbprepaidbillingview":
                             redirectHelpContent = "Cargo/Viewing_AWB_Prepaid_Billing.htm";
                             break;
                         case "awbchargecollectbillinglistview":
                             redirectHelpContent = "Cargo/AWB_Charge_Collect_Billing_Records.htm";
                             break;
                         case "awbchargecollectbillingview":
                             redirectHelpContent = "Cargo/Viewing_AWB_Charge_Collect_Billing.htm";
                             break;
                         case "rmlistview":
                             redirectHelpContent = "Cargo/Rejection_Memo_Listing.htm";
                             break;
                         case "rmview":
                             redirectHelpContent = "Cargo/Viewing_a_Rejection_Memo.htm";
                             break;
                         case "rmprepaidawbview":
                             redirectHelpContent = "Cargo/Viewing_RM_Prepaid_AWB_.htm";
                             break;
                         case "rmchargecollectawbview":
                             redirectHelpContent = "Cargo/Viewing_RM_Charge_Collect_AWB.htm";
                             break;
                         case "bmlistview":
                             redirectHelpContent = "Cargo/Billing_Memo_Listing.htm";
                             break;
                         case "bmview":
                             redirectHelpContent = "Cargo/Viewing_a_Billing_Memo.htm";
                             break;
                         case "bmawbview":
                             if (Session["helplinkurl"].ToString() == "Bm_Awb_Prepaid_view")
                             {
                                 redirectHelpContent = "Cargo/Viewing_BM_Prepaid_AWB.htm";
                                 Session["helplinkurl"] = null;
                             }
                             else
                             {
                                 redirectHelpContent = "Cargo/Viewing_BM_Charge_Collect_AWB.htm";
                                 Session["helplinkurl"] = null;
                             }
                             break;
                         case "vat":
                             redirectHelpContent = "Cargo/Invoice_VAT.htm#Viewing_Invoice_VAT";
                             break;
                     }
                 }

                 else if (appController.ToLower() == "creditnote")
                 {
                     switch (controllerAction.ToLower())
                     {
                         case "create":
                             redirectHelpContent = "Cargo/Creating_Credit_Note_Header.htm";
                             break;

                         case "edit":
                             redirectHelpContent = "Cargo/Editing_Credit_Memo_Header.htm";
                             break;

                         case "cmcreate":
                             redirectHelpContent = "Cargo/Creating_a_Credit_Memo.htm";
                             break;

                         case "cmedit":
                             redirectHelpContent = "Cargo/Editing_a_Credit_Memo.htm";
                             break;

                         case "cmawbprepaidcreate":
                             redirectHelpContent = "Cargo/Adding_AWB_Prepaid_Billing_for_CM.htm";
                             break;

                         case "cmawbedit":
                             if (Session["helplinkurl"] == "Cm_Awb_Prepaid_edit")
                             {
                                 redirectHelpContent = "Cargo/Editing_CM_AWB_Prepaid_Billing.htm";
                                 Session["helplinkurl"] = null;
                             }
                             else
                             {
                                 redirectHelpContent = "Cargo/Editing_CM_AWB_Charge_Collect_Billing.htm";
                                 Session["helplinkurl"] = null;
                             }
                             break;

                         case "cmawbchargecollectcreate":
                             redirectHelpContent = "Cargo/Adding_AWB_Charge_Collect_Billing_for_CM.htm";
                             break;

                         case "vat":
                             redirectHelpContent = "Cargo/Credit_Note_VAT.htm";
                             break;

                         case "view":
                             redirectHelpContent = "Cargo/Viewing_a_Credit_Memo.htm";
                             break;

                         case "cmview":
                             redirectHelpContent = "Cargo/Viewing_a_Credit_Memo.htm";
                             break;

                         case "cmawbprepaidview":
                             redirectHelpContent = "Cargo/Viewing_CM_Prepaid_AWB.htm";
                             break;

                         case "cmawbchargecollectview":
                             redirectHelpContent = "Cargo/Viewing_CM_Charge_Collect_AWB.htm";
                             break;

                         case "vatview":
                             redirectHelpContent = "Cargo/Credit_Note_VAT.htm#Viewing_Credit_Note_VAT";
                             break;

                     }

                 }

                 else if (appController.ToLower() == "correspondence")
                 {
                     if (controllerAction.ToLower() == "correspondence")
                         redirectHelpContent = "Cargo/Managing_Correspondence.htm";

                     if (controllerAction.ToLower() == "cargocreatecorrespondence")
                         redirectHelpContent = "Cargo/Initiating_Correspondence.htm";

                     if (controllerAction.ToLower() == "editcargocorrespondence")
                         redirectHelpContent = "Cargo/Editing_Correspondence.htm";

                     
                 }

                 else if (appController.ToLower() == "billinghistoryaudittrail")
                 {
                         redirectHelpContent = "Cargo/Viewing_Audit_Trail.htm";
                 }

                 else if (appController.ToLower() == "cargosupportingdoc")
                 {
                     if (controllerAction.ToLower() == "index")
                         redirectHelpContent = "Cargo/Managing_Support_Documents.htm";
                 }

                 else if (appController.ToLower() == "unlinkedsupportingdocument")
                 {
                     if (controllerAction.ToLower() == "index")
                         redirectHelpContent = "Cargo/Correct_Supporting_Document_Linking_Errors.htm";
                 }

                 else  if (controllerAction.ToLower() == "billinghistory")
                     redirectHelpContent = "Cargo/Billing_History.htm";

                 else if (controllerAction.ToLower() == "validationerrorcorrection")
                     redirectHelpContent = "Cargo/Validation_Error_Correction.htm";

                 else if (controllerAction.ToLower() == "correspondencetrail")
                     redirectHelpContent = "Cargo/Downloading_Correspondence_Reports.htm";
                
                
            }

            else if (appArea == "CargoPayables")
            {

                if (controllerAction.ToLower() == "payablesinvoicesearch")
                    redirectHelpContent = "Cargo/Searching_an_Invoice.htm";

                else if (appController.ToLower() == "cargosupportingdoc")
                {
                    if (controllerAction.ToLower() == "payablesupportingdocs")
                        redirectHelpContent = "Cargo/Managing_Support_Documents.htm#Managing_the_Support_Documents_using_Payables_submenu";

                }

            }

                #endregion

            #region Passenger

            else if(appController.ToLower() == "autobilling")
            {
                if(controllerAction.ToLower() == "index")
                    redirectHelpContent = "Passenger/Correcting_Autobilling_Invoices.htm";
            }

            else if(controllerAction.ToLower() == "paxvalidationerrorcorrection")
            {
                if (Session["helplinkurl"].ToString() == "Formcs_validation_error_correction")
                        {
                            redirectHelpContent =
                                "Passenger/Validation_Error_Correction_for_Form_C.htm";
                            Session["helplinkurl"] = null;
                        }

                else if (Session["helplinkurl"].ToString() == "Invoices_creditnote_validation_error_correction")
                {
                    redirectHelpContent =
                        "Passenger/Validation_Error_Correction_for_Invoices_Credit_Note.htm";
                    Session["helplinkurl"] = null;
                }
            }

            else if (controllerAction.ToLower() == "correspondencetrail")
            {
                redirectHelpContent =
                       "Passenger/Downloading_Correspondence_Reports.htm";
            }
            
            else if (appController.ToLower() == "invoice")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "create")
                    redirectHelpContent = "Passenger/Creating_Invoice_Header.htm";

                if (controllerAction.ToLower() == "primebillingcreate")
                    redirectHelpContent = "Passenger/Creating_Prime_Billing.htm";

                if (controllerAction.ToLower() == "primebillinglist")
                    redirectHelpContent = "Passenger/Prime_Billing_Coupon_List.htm";

                if (controllerAction.ToLower() == "primebillinglistview")
                    redirectHelpContent = "Passenger/Prime_Billing_Coupon_List.htm";

                if (controllerAction.ToLower() == "rmcreate")
                    redirectHelpContent = "Passenger/Creating_a_Rejection_Memo.htm";

                if (controllerAction.ToLower() == "rmcouponcreate")
                    redirectHelpContent = "Passenger/Adding_a_Rejection_Memo_Coupon.htm";

                if (controllerAction.ToLower() == "rmlist")
                    redirectHelpContent = "Passenger/Rejection_Memo_Coupon_List.htm";

                if (controllerAction.ToLower() == "rmlistview")
                    redirectHelpContent = "Passenger/Rejection_Memo_Coupon_List.htm";

                if (controllerAction.ToLower() == "bmcreate")
                    redirectHelpContent = "Passenger/Creating_Billing_Memo.htm";

                if (controllerAction.ToLower() == "bmcouponcreate")
                    redirectHelpContent = "Passenger/Adding_Billing_Memo_Coupon.htm";

                if (controllerAction.ToLower() == "bmlist")
                    redirectHelpContent = "Passenger/Billing_Memo_List_Screen.htm";

                if (controllerAction.ToLower() == "bmlistview")
                    redirectHelpContent = "Passenger/Billing_Memo_List_Screen.htm";

                if (controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Passenger/Editing_Invoice_Header.htm";

                if (controllerAction.ToLower() == "primebillingedit")
                    redirectHelpContent = "Passenger/Editing_the_Prime_Billing.htm";

                if (controllerAction.ToLower() == "rmedit")
                    redirectHelpContent = "Passenger/Editing_the_Rejection_Memo.htm";

                if (controllerAction.ToLower() == "vat")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Invoice_VAT";

                if (controllerAction.ToLower() == "view")
                    redirectHelpContent = "Passenger/View_Non-Sampling_Invoice.htm";

                if (controllerAction.ToLower() == "primebillingview")
                    redirectHelpContent = "Passenger/View_Non-Sampling_Invoice.htm#Viewing_the_Prime_Billing";

                if (controllerAction.ToLower() == "bmview")
                    redirectHelpContent = "Passenger/View_Non-Sampling_Invoice.htm#Viewing_the_Billing_Memo";

                if (controllerAction.ToLower() == "rmview")
                    redirectHelpContent = "Passenger/View_Non-Sampling_Invoice.htm#Viewing_the_Rejection_Memo";

                if (controllerAction.ToLower() == "vatview")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Viewing_Invoice_VAT";

                if (controllerAction.ToLower() == "bmedit")
                    redirectHelpContent = "Passenger/Editing_the_Billing_Memo.htm";

                if (controllerAction.ToLower() == "bmcouponedit")
                    redirectHelpContent = "Passenger/Editing_the_Billing_Memo.htm#Editing_Billing_Memo_Coupon";

                if (controllerAction.ToLower() == "rmcouponview")
                    redirectHelpContent = "Passenger/View_Non-Sampling_Invoice.htm#Viewing_the_Rejection_Memo_Coupon";

                if (controllerAction.ToLower() == "bmcouponview")
                    redirectHelpContent = "Passenger/View_Non-Sampling_Invoice.htm#Viewing_the_Billing_Memo_Coupon";


                if (controllerAction.ToLower() == "rmcouponedit")
                    redirectHelpContent = "Passenger/Editing_the_Rejection_Memo.htm#Editing_the_Rejection_Memo_Coupon";



            }
            else if (appController.ToLower() == "billinghistoryaudittrail")
            {
                redirectHelpContent = "Passenger/Working_with_Audit_Trail.htm";
            }

            else if (appController.ToLower() == "createbillingmemo" && controllerAction.ToLower() == "correspondence")
            {
                redirectHelpContent = "Miscellaneous/Creating_Correspondence_Invoice.htm";
            }

            else if (appController.ToLower() == "manageinvoice" && controllerAction.ToLower() == "index")
                redirectHelpContent = "Passenger/Searching_Invoice.htm";

            else if (appController.ToLower() == "correspondence" && appArea == "Pax")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "correspondence")
                    redirectHelpContent = "Passenger/Managing_Correspondence.htm";

                if (controllerAction.ToLower() == "replycorrespondence" || controllerAction.ToLower() == "paxcreatecorrespondencefor")
                    redirectHelpContent = "Passenger/Initiating_Correspondence.htm";

                if (controllerAction.ToLower() == "paxcreatecorrespondence")
                    redirectHelpContent = "Passenger/Initiating_Correspondence.htm";

                if (controllerAction.ToLower() == "editpaxcorrespondence")
                    redirectHelpContent = "Passenger/Initiating_Correspondence.htm";
            }

            else if (appController.ToLower() == "correspondence" && appArea == "Misc")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "correspondence")
                    redirectHelpContent = "Miscellaneous/Managing_Correspondence.htm";

                if (controllerAction.ToLower() == "replycorrespondence")
                    redirectHelpContent = "Miscellaneous/Creating_Correspondence_Invoice.htm";

                //if (controllerAction.ToLower() == "editcorrespondence")
                //    redirectHelpContent = "Passenger/Managing_Correspondence.htm";

                if (controllerAction.ToLower() == "opencorrespondenceforedit")
                    redirectHelpContent = "Miscellaneous/Creating_Correspondence_via_Billing_History_and_Correspondence.htm";
            }

            else if (appController.ToLower() == "creditnote")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "create")
                    redirectHelpContent = "Passenger/Creating_Credit_Note_Header.htm";

                if (controllerAction.ToLower() == "creditmemocreate")
                    redirectHelpContent = "Passenger/Creating_Non_Sampling_Credit_Note_.htm";

                if (controllerAction.ToLower() == "creditmemocouponcreate")
                    redirectHelpContent = "Passenger/Create_Credit_Memo_Coupon.htm";

                if (controllerAction.ToLower() == "creditmemoedit")
                    redirectHelpContent = "Passenger/Editing_a_Credit_Memo.htm";

                if (controllerAction.ToLower() == "vat")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Credit_Note_VAT";

                if (controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Passenger/Editing_Credit_Note.htm";

                if (controllerAction.ToLower() == "creditmemocouponedit")
                    redirectHelpContent = "Passenger/Editing_a_Credit_Memo.htm#Editing_a_Credit_Memo_Coupon";

                if (controllerAction.ToLower() == "cmedit")
                    redirectHelpContent = "Passenger/Editing_a_Credit_Memo.htm";

                if (controllerAction.ToLower() == "view")
                    redirectHelpContent = "Passenger/Viewing_a_Credit_Note.htm";

                if (controllerAction.ToLower() == "creditmemoview")
                    redirectHelpContent = "Passenger/Viewing_a_Credit_Note.htm#Viewing_a_Credit_Memo";

                if (controllerAction.ToLower() == "vatview")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Viewing_Invoice_VAT";


            }

            else if (appController.ToLower() == "formc")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "create")
                    redirectHelpContent = "Passenger/Creating_Form_C_Header.htm";

                if (controllerAction.ToLower() == "index")
                    redirectHelpContent = "Passenger/Searching_Sampling_Form_C.htm";

                if (controllerAction.ToLower() == "couponcreate")
                    redirectHelpContent = "Passenger/Creating_Form_C_.htm";

                if (controllerAction.ToLower() == "payablessearch")
                    redirectHelpContent = "Passenger/Searching_Sampling_Form_C.htm#Searching_Form_C_using_Payables_submenu";

                if (controllerAction.ToLower() == "view")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                if (controllerAction.ToLower() == "viewdetails")
                    redirectHelpContent = "Passenger/Viewing_Form_C.htm";

                if (controllerAction.ToLower() == "couponview")
                    redirectHelpContent = "Passenger/Viewing_Form_C.htm#Viewing_Sampling_Form_C_Coupon";

                if (controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Passenger/Editing_Form_C.htm";

                if (controllerAction.ToLower() == "couponedit")
                    redirectHelpContent = "Passenger/Editing_Form_C.htm#Editing_Sampling_Form_C_Coupon";




            }
            else if (appController.ToLower() == "formde")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";

                if (appController.ToLower() == "formde" && controllerAction.ToLower() == "create")
                    redirectHelpContent = "Passenger/Creating_Form_D_E_Header.htm";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "formdcreate")
                    redirectHelpContent = "Passenger/Creating_Form_D_E.htm#Create_Form_D";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "provisionalinvoice")
                    redirectHelpContent = "Passenger/Creating_Form_D_E.htm#Provisional_Invoice_Record";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "formdedit")
                    redirectHelpContent = "Passenger/Editing_Form_D_E_.htm";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Passenger/Editing_Form_D_E.htm";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "vat")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Form_E_VAT_Capture";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "formeedit")
                    redirectHelpContent = "Passenger/Creating_Form_D_E.htm#Creating_Form_E";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "view")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "vatview")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Viewing_Invoice_VAT";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "details")
                    redirectHelpContent = "Passenger/Editing_Form_D_E_.htm#Editing_Form_E";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "formeview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "formdview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "provisionalinvoiceview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm#Viewing_Provisional_Invoice";

            }

            else if (appController.ToLower() == "formf")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";

                if (appController.ToLower() == "formf" && controllerAction.ToLower() == "create")
                    redirectHelpContent = "Passenger/Creating_Form_F_Header.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "rmcreate")
                    redirectHelpContent = "Passenger/Creating_Form_F.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "rmcouponcreate")
                    redirectHelpContent = "Passenger/Adding_a_Rejection_Memo_Coupon.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "vatview")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Viewing_Invoice_VAT";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "view")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Passenger/Editing_Form_F_Header.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "rmedit")
                    redirectHelpContent = "Passenger/Editing_Form_F.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "rmcouponedit")
                    redirectHelpContent = "Passenger/Editing_Form_F.htm#Editing_Form_F_Coupon";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "vat")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Form_F_VAT_Capture";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "rmview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "rmcouponview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm#Viewing_Sampling_Form_Coupon";
            }


            else if (appController.ToLower() == "formxf")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "create")
                    redirectHelpContent = "Passenger/Creating_Form_XF_Header.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "rmcreate")
                    redirectHelpContent = "Passenger/Creating_Form_XF_.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "rmcouponcreate")
                    redirectHelpContent = "Passenger/Adding_a_Rejection_Memo_Coupon.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "vat")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Form_XF_VAT_Capture";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "view")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "vatview")
                    redirectHelpContent = "Passenger/VAT_Breakdown.htm#Viewing_Invoice_VAT";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Passenger/Editing_Form_XF_Header.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "rmedit")
                    redirectHelpContent = "Passenger/Editing_Form_XF.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "rmcouponedit")
                    redirectHelpContent = "Passenger/Editing_Form_XF.htm#Editing_Form_XF_Coupon";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "rmview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm";

                else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "rmcouponview")
                    redirectHelpContent = "Passenger/Viewing_Sampling_Form1.htm#Viewing_Sampling_Form_Coupon";

            }

            else if (appController.ToLower() == "supportingdoc")
            {
                redirectHelpContent = "Passenger/Topic_Under_Construction.htm";
                if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "index")
                    redirectHelpContent = "Passenger/Managing_the_Support_Documents.htm";
                else if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "payablesupportingdocs")
                    redirectHelpContent = "Passenger/Managing_the_Support_Documents.htm#Managing_the_Supporting_Documents_Using_Payables_submenu";
            }
            else if (appController.ToLower() == "unlinkedsupportingdocument" && controllerAction.ToLower() == "index" && appArea == "Pax")
                redirectHelpContent = "Passenger/Correcting_the_Linking_Errors_for_Supporting_Documents.htm";


            //if (appArea.ToLower() == "pax")
            //    redirectHelpContent = GetPassengerHelpUrl(appController, controllerAction);

            #endregion

            #region Miscellaneous

            if (appController.ToLower() == "bhaudittrail")
            {
                redirectHelpContent = "Miscellaneous/Viewing_Audit_Trail.htm";
            }
            if (appArea.ToLower() == "misc")
            {
                if (appController.ToLower() == "supportingdoc")
                {
                    redirectHelpContent = "Miscellaneous/Topic_Under_Construction.htm";
                    if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "index")
                        redirectHelpContent = "Miscellaneous/Manage_Supporting_Documents.htm#Managing_the_Supporting_Documents_using_Receivables_submenu";
                    else if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "payablesupportingdocs")
                        redirectHelpContent = "Miscellaneous/Topic_Under_Construction.htm";
                }
                else if(appController.ToLower() == "correspondencetrail")
                {
                      if (controllerAction.ToLower() == "index")
                     {
                         redirectHelpContent =
                             "Miscellaneous/Downloading_Correspondence_Reports.htm";
                     }
                }

                else if(appController.ToLower() == "MiscValidationErrorCorrection".ToLower())
                {
                    if(controllerAction.ToLower() == "Index".ToLower())
                        redirectHelpContent =
                             "Miscellaneous/Correcting_Validation_Errors.htm";

                }
                else if (appController.ToLower() == "BillingHistory".ToLower())
                {
                  if (controllerAction.ToLower() == "Index".ToLower())
                  {
                    redirectHelpContent = "Miscellaneous/Billing_History.htm";
                  }
                }
                else if (appController.ToLower() == "miscpayinvoice".ToLower())
                {
                    redirectHelpContent = "Miscellaneous/Topic_Under_Construction.htm";
                    if (controllerAction.ToLower() == "create")
                        redirectHelpContent = "Miscellaneous/Creating_Invoice_Header.htm";

                    if (controllerAction.ToLower() == "lineitemcreate")
                        redirectHelpContent = "Miscellaneous/Creating_Line_Item.htm";

                    if (controllerAction.ToLower() == "lineitemdetailcreate")
                        redirectHelpContent = "Miscellaneous/Creating_Line_Item_Detail.htm";

                    if (controllerAction.ToLower() == "edit")
                        redirectHelpContent = "Miscellaneous/Editing.htm";

                    if (controllerAction.ToLower() == "lineitemdetailedit")
                        redirectHelpContent = "Miscellaneous/Editing_Line_Item_Detail.htm";

                    if (controllerAction.ToLower() == "lineitemedit")
                        redirectHelpContent = "Miscellaneous/Editing_Line_Item.htm";

                    if (controllerAction.ToLower() == "createrejectioninvoice")
                        redirectHelpContent = "Miscellaneous/Rejecting_Invoice_via_View_Billing_History.htm";

                    if (controllerAction.ToLower() == "view")
                        redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm";

                    if (controllerAction.ToLower() == "lineitemview")
                        redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item";

                    if (controllerAction.ToLower() == "lineitemdetailview")
                        redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item_Details";

                    if (controllerAction.ToLower() == "lineitemdetailview")
                        redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item_Details";

                    if (controllerAction.ToLower() == "showdetails")
                        redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm";
                }
            }


            if (appArea.ToLower() == "miscpayables")
            {
                if (appController.ToLower() == "supportingdoc")
                {
                    redirectHelpContent = "Miscellaneous/Topic_Under_Construction.htm";
                    if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "index")
                        redirectHelpContent = "Miscellaneous/Manage_Supporting_Documents.htm#Managing_the_Supporting_Documents_using_Payables_submenu";
                    else if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "payablesupportingdocs")
                        redirectHelpContent = "Miscellaneous/Manage_Supporting_Documents.htm#Managing_the_Supporting_Documents_using_Payables_submenu";
                }
            }

            if (appController.ToLower() == "miscinvoice")
            {
                redirectHelpContent = "Miscellaneous/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "create")
                    redirectHelpContent = "Miscellaneous/Creating_Invoice_Header.htm";

                if (controllerAction.ToLower() == "lineitemcreate")
                    redirectHelpContent = "Miscellaneous/Creating_Line_Item.htm";

                if (controllerAction.ToLower() == "lineitemdetailcreate" || controllerAction.ToLower() == "lineitemdetailclone")
                    redirectHelpContent = "Miscellaneous/Creating_Line_Item_Detail.htm";

                if (controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Miscellaneous/Editing.htm";

                if (controllerAction.ToLower() == "lineitemdetailedit" || controllerAction.ToLower() == "lineitemdetailduplicate" )
                    redirectHelpContent = "Miscellaneous/Editing_Line_Item_Detail.htm";

                if (controllerAction.ToLower() == "lineitemedit")
                    redirectHelpContent = "Miscellaneous/Editing_Line_Item.htm";

                if (controllerAction.ToLower() == "createrejectioninvoice")
                    redirectHelpContent = "Miscellaneous/Rejecting_Invoice_via_View_Billing_History.htm";

                if (controllerAction.ToLower() == "view")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm";

                if (controllerAction.ToLower() == "lineitemview")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item";

                if (controllerAction.ToLower() == "lineitemdetailview")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item_Details";

                if (controllerAction.ToLower() == "lineitemdetailview")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item_Details";

                if (controllerAction.ToLower() == "showdetails")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm";
            }


            else if (appController.ToLower() == "misccreditnote")
            {
                redirectHelpContent = "Miscellaneous/Topic_Under_Construction.htm";
                if (controllerAction.ToLower() == "create")
                    redirectHelpContent = "Miscellaneous/Creating_Miscellaneous_Credit_Note_Invoice_Header.htm";

                if (controllerAction.ToLower() == "lineitemcreate")
                    redirectHelpContent = "Miscellaneous/_Creating_Line_Item.htm";

                if (controllerAction.ToLower() == "lineitemdetailcreate")
                    redirectHelpContent = "Miscellaneous/Creating_Line_Item_Detail.htm";

                if (controllerAction.ToLower() == "lineitemedit")
                    redirectHelpContent = "Miscellaneous/Editing_Miscellaneous_Credit_Note.htm#Editing_Line_Item";

                if (controllerAction.ToLower() == "view")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm";

                if (controllerAction.ToLower() == "edit")
                    redirectHelpContent = "Miscellaneous/Editing_Miscellaneous_Credit_Note.htm";

                if (controllerAction.ToLower() == "lineitemdetailedit")
                    redirectHelpContent = "Miscellaneous/Editing_Miscellaneous_Credit_Note.htm#Editing_Line_Item_Detail";

                if (controllerAction.ToLower() == "lineitemdetailview")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item_Details";

                if (controllerAction.ToLower() == "lineitemview")
                    redirectHelpContent = "Miscellaneous/Viewing_an_Invoice.htm#Viewing_Line_Item";
            }

            else if (appController.ToLower() == "unlinkedsupportingdocument" && controllerAction.ToLower() == "index" && appArea == "Misc")
                redirectHelpContent = "Miscellaneous/Correcting_Supporting_Document_Linking_Errors.htm";

            //else if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "index")
            //  redirectHelpContent = "";
            #endregion


            #region member
            if (appController.ToLower() == "member")
            {
                if (SessionUtil.UserCategory == UserCategory.AchOps)
                {
                    if (controllerAction.ToLower() == "manage")
                        redirectHelpContent = "AchOps/Managing_the_Member_Profile.htm";
                }
                else if (SessionUtil.UserCategory == UserCategory.IchOps)
                {
                    if (controllerAction.ToLower() == "manage")
                        redirectHelpContent = "IchOps/Managing_the_Member_Profile.htm";
                }
                else if (SessionUtil.UserCategory == UserCategory.Member)
                {
                    if (Session["helplinkurl"] != null)
                    {
                        if (Session["helplinkurl"].ToString() == "Managing_Location_Details_")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Managing_Location_Details_";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Managing_Contacts_")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Managing_Contacts_";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Managing_E-billing_Services")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Managing_E-billing_Services";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Managing_Passenger_Billing_Configurations")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Managing_Passenger_Billing_Configurations";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Managing_UATP_Billing_Configurations")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Managing_UATP_Billing_Configurations";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Viewing_ACH_Configurations")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Viewing_ACH_Configurations";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Viewing_ICH_Configurations")
                        {
                            redirectHelpContent =
                                "MemberProfile/Managing_Member_Profile.htm#Viewing_ICH_Configurations";
                            Session["helplinkurl"] = null;
                        }
                        else if (Session["helplinkurl"].ToString() == "Managing_Cargo")
                        {
                            redirectHelpContent =
                                "MemberUserGuide/Topic_Under_Construction.htm";
                            Session["helplinkurl"] = null;
                        }

                    }


                    else if (controllerAction.ToLower() == "manage")
                        redirectHelpContent = "MemberProfile/Managing_Member_Profile.htm";

                }

                else if (SessionUtil.UserCategory == UserCategory.SisOps)
                {

                }
            }

            #endregion


            #region "Reports"

            else if (appController.ToLower() == "processingdashboard")
            {

                if (controllerAction.ToLower() == "isprocessingdashboard".ToLower())
                {
                    if (Session["helplinkurl"].ToString() == "Invoice_Status_Details")
                    {
                        redirectHelpContent = "Reports/Managing_the_Processing_Dashboard.htm#Viewing_Invoice_Status_Details";

                        //Session["helplinkurl"] = null;
                    }
                    else if (Session["helplinkurl"].ToString() == "File_Status_Details")
                    {
                        redirectHelpContent = "Reports/Managing_the_Processing_Dashboard.htm#Viewing_File_Status_Details";

                        //Session["helplinkurl"] = null;
                    }
                }


            }

            else if(appController.ToLower() == "PendingInvoices".ToLower())
            {
                if (controllerAction.ToLower() == "PendingInvoicesReport".ToLower())
                    redirectHelpContent = "Reports/Pending_Invoices_In_Error_Report.htm";

            }
                
            else if (appController.ToLower() == "Miscellaneous".ToLower())
            {
                if (controllerAction.ToLower() == "MiscChargeSummary".ToLower())
                  redirectHelpContent = "Reports/MISC_Receivables_Invoice_Summary_Report.htm";

                if (controllerAction.ToLower() == "MiscChargePaySummary".ToLower())
                  redirectHelpContent = "Reports/MISC_Payables_Invoice_Summary_Report.htm";

            }

            else if (appController.ToLower() == "sisusagereport".ToLower())
            {

                if (controllerAction.ToLower() == "sisusagereport".ToLower())
                    redirectHelpContent = "Reports/SIS_Usage_Report.htm";
                if (controllerAction.ToLower() == "sisiswebusagereport".ToLower())
                    redirectHelpContent = "Reports/SIS_IS-WEB_Usage_Report.htm";

            }

            else if (appController.ToLower() == "queryanddownloaddetails".ToLower())
            {

                if (controllerAction.ToLower() == "memberdetails".ToLower())

                    redirectHelpContent = "Reports/Member_Details_Report.htm";

            }

            else if (appController.ToLower() == "invoicedeletionaudit".ToLower())
            {

                if (controllerAction.ToLower() == "invoicedeletionaudit".ToLower())

                    redirectHelpContent = "Reports/Invoice_Deletion_Audit_Trail_Report.htm";

            }

            else if (appController.ToLower() == "userpermissionreport".ToLower())
            {

                if (controllerAction.ToLower() == "userpermissionreport".ToLower())

                    redirectHelpContent = "Reports/User_Permission_Report.htm";

            }

            else if (appController.ToLower() == "iscalendar".ToLower())
            {

                if (controllerAction.ToLower() == "iscalendar".ToLower())

                    redirectHelpContent = "Reports/IS_and_CH_Calendar_Report.htm";

            }

            else if (appController.ToLower() == "memberlocation".ToLower())
            {

                if (controllerAction.ToLower() == "memberlocation".ToLower())

                    redirectHelpContent = "Reports/Invoice_Reference_Data_Report.htm";

            }



            else if (appController.ToLower() == "correspondencestatus".ToLower())
            {

                if (controllerAction.ToLower() == "paxcorrespondencestatus".ToLower())

                    redirectHelpContent = "Reports/Passenger__Correspondence_Status_Report.htm";


                if (controllerAction.ToLower() == "misccorrespondencestatus".ToLower())
                    redirectHelpContent = "Reports/Miscellaneous__Correspondence_Status_Report.htm";

                        if (controllerAction.ToLower() == "CGOCorrespondenceStatus".ToLower())
                    redirectHelpContent = "Reports/Correspondence_Status_Report_Cgo.htm";

                

            }

            else if (appController.ToLower() == "confirmationdetail".ToLower())
            {

                if (controllerAction.ToLower() == "confirmationdetail".ToLower())

                    redirectHelpContent = "Reports/BVC_(Billing_Value_Confirmation)_Details_Report.htm";

            }

            else if (appController.ToLower() == "supportingmismatchdocument".ToLower())
            {

                if (controllerAction.ToLower() == "mismatchdocument".ToLower())
                    redirectHelpContent = "Reports/Receivables_–_Supporting_Documents_Mismatch.htm";

                if (controllerAction.ToLower() == "CgoMismatchDocument".ToLower())
                    redirectHelpContent = "Reports/Supporting_Attachments_Mismatch_Report.htm";

                if (controllerAction.ToLower() == "miscmismatchdocument".ToLower())
                   redirectHelpContent = "Reports/MISC_Receivables_Supporting_Documents_Mismatch_Report.htm";
            }

            else if (appController.ToLower() == "payablesreport".ToLower())
            {

                if (controllerAction.ToLower() == "payablesreport".ToLower())
                    redirectHelpContent = "Reports/Payables_–_RM_BM_CM_Summary.htm";

                if (controllerAction.ToLower() == "PaxInterlineBillingSummaryReport".ToLower())
                    redirectHelpContent = "Reports/Payables_-_Passenger_Interline_Billing_Summary.htm";

                if (controllerAction.ToLower() == "PaxRejectionAnalysisNonSamplingReport".ToLower())
                    redirectHelpContent = "Reports/Payables_-_Passenger_Rejection_Analysis_-_Non_Sampling_.htm";

                if (controllerAction.ToLower() == "IwSamplingReport".ToLower())
                    redirectHelpContent = "Reports/Payables_-_Passenger_Sampling_Billing_Analysis.htm";

            }

            else if (appController.ToLower() == "receivablesreport".ToLower())
            {

                if (controllerAction.ToLower() == "receivablesreport".ToLower())
                    redirectHelpContent = "Reports/Receivables_–_RM_BM_CM_Summary.htm";

                if (controllerAction.ToLower() == "PaxInterlineBillingSummaryReport".ToLower())
                    redirectHelpContent = "Reports/Receivables_-_Passenger_Interline_Billing_Summary.htm";

                if (controllerAction.ToLower() == "PaxRejectionAnalysisNonSamplingReport".ToLower())
                    redirectHelpContent = "Reports/Receivables_-_Passenger_Rejection_Analysis_-_Non_Sampling_.htm";

                if (controllerAction.ToLower() == "OwSamplingReport".ToLower())
                    redirectHelpContent = "Reports/Receivables_-_Passenger_Sampling_Billing_Analysis.htm";

            }

            else if (appController.ToLower() == "InterlinePayablesAnalysis".ToLower())
            {

                if (controllerAction.ToLower() == "InterlineBillingSummaryReport".ToLower())
                    redirectHelpContent = "Reports/Interline_Billing_Summary.htm";

                if (controllerAction.ToLower() == "InterlinePayablesAnalysis".ToLower())
                    redirectHelpContent = "Reports/Interline_Payables_Analysis_Report.htm";
                
            }

            else if (appController.ToLower() == "paxcgomsctoptenpartner".ToLower())
            {

                if (controllerAction.ToLower() == "PaxCgoMscTopTenPartnerRec".ToLower())
                    redirectHelpContent = "Reports/Top_10_Interline_Partners_-_Receivables.htm";

                if (controllerAction.ToLower() == "PaxCgoMscTopTenPartnerPay".ToLower())
                    redirectHelpContent = "Reports/Top_10_Interline_Partners_-_Payables.htm";

            }

            else if (appController.ToLower() == "CargoInterlineBillingSummary".ToLower())
            {
                
                if (controllerAction.ToLower() == "ReceivablesReport".ToLower())
                    redirectHelpContent = "Reports/Receivables_Interline_Summary.htm";

                if (controllerAction.ToLower() == "PayablesReport".ToLower())
                    redirectHelpContent = "Reports/Payables_Interline_Summary.htm";

            }

            else if (appController.ToLower() == "RejectionAnalysisRec".ToLower())
            {

                if (controllerAction.ToLower() == "CgoRejectionAnalysisRec".ToLower())
                    redirectHelpContent = "Reports/Rejection_Analysis_-_Receivables.htm";

                if (controllerAction.ToLower() == "CgoRejectionAnalysisPay".ToLower())
                    redirectHelpContent = "Reports/Rejection_Analysis_-_Payables.htm";

            }

            else if (appController.ToLower() == "RMBMCMSummaryReport".ToLower())
            {

                if (controllerAction.ToLower() == "CargoReceivablesReport".ToLower())
                    redirectHelpContent = "Reports/Receivables_RM_BM_CM_Summary.htm";

                if (controllerAction.ToLower() == "CargoPayablesReport".ToLower())
                    redirectHelpContent = "Reports/Payables_RM_BM_CM_Summary.htm";

            }

            else if (appController.ToLower() == "SupportingMismatchDocument".ToLower())
            {

                

            }

            else if (appController.ToLower() == "ReceivableCargoSubmissionOverview".ToLower())
            {

                if (controllerAction.ToLower() == "PayableCargoSubmissionOverview".ToLower())
                    redirectHelpContent = "Reports/Cargo_Submission_Overview_-_Payables.htm";
            }

          
            else if (controllerAction.ToLower() == "ReceivableCargoSubmissionOverview".ToLower())
            {
                redirectHelpContent = "Reports/Cargo_Submission_Overview_-_Receivables.htm";

            }

            else if (controllerAction.ToLower() == "RMBMCMDetails".ToLower())
            {
                redirectHelpContent = "Reports/RM-BM-CM_Details_Report.htm";

            }




            //else if (appController.ToLower() == "correspondencestatus".ToLower())
            //{

            //    if (controllerAction.ToLower() == "misccorrespondencestatus".ToLower())

            //        redirectHelpContent = "Reports/Miscellaneous__Correspondence_Status_Report.htm";

            //}



            #endregion

            #region Uatp
                    if(appArea == "Uatp")
                    {
                        if (appController.ToLower() == "UatpValidationErrorCorrection".ToLower())
                        {
                            if(controllerAction.ToLower() == "index")
                                redirectHelpContent = "Uatp/Correcting_Validation_Errors.htm";

                        }

                        else if (appController.ToLower() == "ShowDetails".ToLower())
                        {
                            if (controllerAction.ToLower() == "ShowDetails".ToLower())
                                redirectHelpContent = "Uatp/Viewing_an_Invoice_Credit_Note_using_Billing_History_and_Correspondence_submenu.htm";

                        }
                        else if(appController.ToLower() == "uatpinvoice")
                      {
                          switch (controllerAction.ToLower())
                          {
                              case "create":
                                  redirectHelpContent = "Uatp/Creating_Invoice_Header.htm";
                                  break;
                              case "lineitemcreate":
                                  redirectHelpContent = "Uatp/Creating_Line_Item.htm";
                                  break;
                              case "lineitemdetailcreate":
                                  redirectHelpContent = "Uatp/Creating_Line_Item_Detail.htm";
                                  break;
                              case "lineitemedit":
                                  redirectHelpContent = "Uatp/Editing_Line_Item.htm";
                                  break;
                              case "lineitemdetailedit":
                                  redirectHelpContent = "Uatp/Editing_Line_Item_Detail.htm";
                                  break;
                              case "edit":
                                  redirectHelpContent = "Uatp/Editing_UATP_Invoice.htm";
                                  break;
                              case "view":
                                  redirectHelpContent = "Uatp/Viewing_an_Invoice_Credit_Note_using_Receivables_submenu.htm";
                                  break;
                              case "lineitemview":
                                  redirectHelpContent = "Uatp/Viewing_a_Line_Item.htm";
                                  break;
                              case "lineitemdetailview":
                                  redirectHelpContent = "Uatp/Viewing_a_Line_Item_Detail.htm";
                                  break;
                          }

                      }

                      else if (appController.ToLower() == "uatpcreditnote")
                      {
                          switch (controllerAction.ToLower())
                          {
                              case "create":
                                  redirectHelpContent = "Uatp/Creating_UATP_Credit_Note_Header.htm";
                                  break;
                              case "lineitemcreate":
                                  redirectHelpContent = "Uatp/Creating_Line_Item_CR.htm";
                                  break;
                              case "lineitemedit":
                                  redirectHelpContent = "Uatp/Editing_Credit_Note.htm#Editing_Line_Item";
                                  break;
                              case "lineitemdetailcreate":
                                  redirectHelpContent = "Uatp/Creating_Line_Item_Detail_CR.htm";
                                  break;
                              case "lineitemdetailedit":
                                  redirectHelpContent = "Uatp/Editing_Credit_Note.htm#Editing_Line_Item_Detail";
                                  break;
                              case "edit":
                                  redirectHelpContent = "Uatp/Editing_Credit_Note.htm";
                                  break;
                              case "view":
                                  redirectHelpContent = "Uatp/Viewing_an_Invoice_Credit_Note_using_Receivables_submenu.htm";
                                  break;
                              case "lineitemview":
                                  redirectHelpContent = "Uatp/Viewing_a_Line_Item.htm";
                                  break;
                              case "lineitemdetailview":
                                  redirectHelpContent = "Uatp/Viewing_a_Line_Item_Detail.htm";
                                  break;
                                  
                          }
                         
                      }

                      else if (appController.ToLower() == "manageuatpinvoice")
                      {
                          if (controllerAction.ToLower() == "index")
                              redirectHelpContent = "Uatp/Searching_for_Invoices_Credit_Note.htm";
                      }

                      else if (appController.ToLower() == "uatpsupportingdoc")
                      {
                          if (controllerAction.ToLower() == "index")
                              redirectHelpContent = "Uatp/Managing_Supporting_Documents.htm";
                      }

                      else if (appController.ToLower() == "unlinkedsupportingdocument")
                      {
                          if (controllerAction.ToLower() == "index")
                              redirectHelpContent = "Uatp/Correcting_Support_Documents_Linking_Errors.htm";
                      }

                      else if (appController.ToLower() == "correspondencetrail")
                      {
                          if (controllerAction.ToLower() == "index")
                              redirectHelpContent = "Uatp/Downloading_Correspondence_Reports.htm";
                      }

                      else if (appController.ToLower() == "opencorrespondenceforedit")
                      {
                              redirectHelpContent = "Uatp/Managing_Correspondence.htm";
                      }

                      else if (appController.ToLower() == "createcorrespondence")
                      {
                          redirectHelpContent = "Uatp/Initiating_Correspondence.htm";
                      }

                          

                      else if (appController.ToLower() == "billinghistory")
                      {
                          if (controllerAction.ToLower() == "index")
                              redirectHelpContent = "Uatp/Billing_History.htm";
                      }

                      else if (controllerAction.ToLower() == "bhaudittrail")
                          redirectHelpContent = "Uatp/Viewing_Audit_Trail.htm";

                    }
                    else if(appArea == "UatpPayables")
                    {
                        if (controllerAction.ToLower() == "manageuatppayablesinvoice")
                            redirectHelpContent = "Uatp/Searching_Invoice_Credit_Note_via_Payables.htm";

                        else if (appController.ToLower() == "supportingdoc")
                        {
                            if (controllerAction.ToLower() == "payablesupportingdocs")
                                redirectHelpContent = "Uatp/Managing_Supporting_Documents.htm#Managing_Supporting_Documents_using_Payables_submenu";
                        }
                    }
            #endregion




            else if (appController.ToLower() == "pax")
            {
                if (controllerAction.ToLower() == "billinghistory")
                    redirectHelpContent = "Passenger/Billing_History_.htm";
            }
            // For Account Controller
            else if (appController.ToLower() == "audittrail")
            {
                if (controllerAction.ToLower() == "audittrail")
                    redirectHelpContent = "MemberProfile/Viewing_Profile_Changes.htm";

                else if (SessionUtil.UserCategory == UserCategory.AchOps)
                {
                    if (controllerAction.ToLower() == "ach")
                        redirectHelpContent = "AchOps/Viewing_ACH_Profile_Changes.htm";
                }
                else if (SessionUtil.UserCategory == UserCategory.IchOps)
                {
                    if (controllerAction.ToLower() == "ich")
                        redirectHelpContent = "IchOps/Viewing_ICH_Profile_Changes.htm";
                }

            }


            // For Account Controller
            else if (appController.ToLower() == "account")
            {
                if (controllerAction.ToLower() == "register")
                    redirectHelpContent = "MemberProfile/Creating_New_User.htm";

                else if (controllerAction.ToLower() == "searchormodify")
                    redirectHelpContent = "MemberProfile/Searching_Existing_Users.htm";

                else if (controllerAction.ToLower() == "usereditfrommember")
                    redirectHelpContent = "MemberProfile/Editing_Existing_Users.htm";

                else if (controllerAction.ToLower() == "proxyloginsuccess")
                    redirectHelpContent = "MemberUserGuide/Introduction_to_Home_Page.htm";
            }

            else if (appController.ToLower() == "permission")
            {
                if (controllerAction.ToLower() == "managepermissiontemplate")
                    redirectHelpContent = "MemberProfile/Manage_Permission_Template.htm";

                if (controllerAction.ToLower() == "permissiontouser")
                    redirectHelpContent = "MemberProfile/Assigning_Permissions_to_User.htm";

                if (controllerAction.ToLower() == "newpermissiontemplate")
                    redirectHelpContent = "MemberProfile/Assigning_Permissions_to_User.htm";
            }
           else if (appController.ToLower() == "locationassociation")
               {
                   if (controllerAction.ToLower() == "managelocationassociation")
                   {
                       redirectHelpContent = "MemberProfile/Manage_Location_Association.htm";
                   }
                   if (controllerAction.ToLower() == "viewlocationassociation")
                   {
                       redirectHelpContent = "MemberProfile/View_Manage_Location_Associations.htm";
                   }
                   if (controllerAction.ToLower() == "modifylocationassociation")
                   {
                       redirectHelpContent = "MemberProfile/Modify_Location_Association.htm";
                   }
               }

                        // For Profile Controller
            else if (appController.ToLower() == "profile" && controllerAction.ToLower() == "managecontacts")
                redirectHelpContent = "MemberProfile/Managing_Contacts.htm";

            //For PaxPayables Controller
            else if (appController.ToLower() == "paxpayables" && controllerAction.ToLower() == "managepaxpayablesinvoice")
                redirectHelpContent = "Passenger/Searching_Invoice.htm#Searching_Invoice_using_Payables_submenu";

            // For MiscPayables Controller
            else if (appController.ToLower() == "miscpayables" && controllerAction.ToLower() == "managemiscpayablesinvoice")
                redirectHelpContent = "Miscellaneous/Searching_Invoice_via_Payables.htm";

           //For MiscPayables Controller
                    else if (appController.ToLower() == "miscpayables" && controllerAction.ToLower() == "managemiscdailypayablesinvoice")
                      redirectHelpContent = "Miscellaneous/Viewing_Daily_Bilateral_Invoices.htm";

            // For Misc Controller
            else if (appController.ToLower() == "misc" && controllerAction.ToLower() == "billinghistory")
                redirectHelpContent = "Miscellaneous/Searching_Invoice_via_View_Billing_History.htm";

            // For ManageMiscInvoice Controller
            else if (appController.ToLower() == "managemiscinvoice" && controllerAction.ToLower() == "index")
                redirectHelpContent = "Miscellaneous/Searching_Invoice_via_Receivables.htm";

            //else if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "payablesupportingdocs")
            //  redirectHelpContent = "";


            else if (appController.ToLower() == "home" && controllerAction.ToLower() == "index")
                redirectHelpContent = "MemberUserGuide/Introduction_to_Home_Page.htm";

            else if (appController.ToLower() == "fileviaweb")
            {
                if (controllerAction.ToLower() == "filemanagerupload")
                    redirectHelpContent = "General/Uploading_a_File.htm";

                if (controllerAction.ToLower() == "filemanagerdownload")
                    redirectHelpContent = "General/Downloading_a_File.htm";
            }


            else if (appController.ToLower() == "managesuspendedinvoices")
            {
                if (controllerAction.ToLower() == "managesuspendedinvoices")
                    redirectHelpContent = "General/Resubmitting_the_Suspended_Invoices.htm";
            }

            if (appController.ToLower() == "managesuspendedinvoices")
            {
                if (controllerAction.ToLower() == "membersuspendedinvoices")
                    redirectHelpContent = "Reports/Suspended_Billings_Report.htm";
            }

            if (string.IsNullOrEmpty(redirectHelpContent)) redirectHelpContent = "MemberUserGuide/Topic_Under_Construction.htm";

            var modifiedHelpContentFilePath = GetMultilangualHelpContentFilePath(redirectHelpContent);

            return Redirect(modifiedHelpContentFilePath);
        }

        //public string GetPassengerHelpUrl(string appController, string controllerAction)
        //{
        //    string returnUrl = string.Empty;
        //    if (appController.ToLower() == "invoice")
        //    {
        //        if (controllerAction.ToLower() == "create")
        //            returnUrl = "Passenger/Creating_Invoice_Header.htm";

        //        if (controllerAction.ToLower() == "primebillingcreate")
        //            returnUrl = "Passenger/Creating_Prime_Billing.htm";
        //    }
        //    else if (appController.ToLower() == "manageinvoice" && controllerAction.ToLower() == "index")
        //        returnUrl = "Passenger/Searching_Invoice.htm";

        //    else if (appController.ToLower() == "creditnote" && controllerAction.ToLower() == "create")
        //        returnUrl = "Passenger/Creating_Credit_Note_Header.htm";

        //    else if (appController.ToLower() == "formc")
        //    {
        //        if (controllerAction.ToLower() == "create")
        //            returnUrl = "Passenger/Creating_Form_C_Header.htm";

        //        if (controllerAction.ToLower() == "index")
        //            returnUrl = "Passenger/Searching_Sampling_Form_C.htm";
        //    }

        //    else if (appController.ToLower() == "formde" && controllerAction.ToLower() == "create")
        //        returnUrl = "Passenger/Creating_Form_D_E_Header.htm";

        //    else if (appController.ToLower() == "formf" && controllerAction.ToLower() == "create")
        //        returnUrl = "Passenger/Creating_Form_F_Header.htm";

        //    else if (appController.ToLower() == "formxf" && controllerAction.ToLower() == "create")
        //        returnUrl = "Passenger/Creating_Form_XF_Header.htm";

        //    else if (appController.ToLower() == "unlinkedsupportingdocument" && controllerAction.ToLower() == "index")
        //        returnUrl = "";

        //    else if (appController.ToLower() == "receivables" && controllerAction.ToLower() == "billinghistory")
        //        returnUrl = "";

        //    else if (appController.ToLower() == "formc" && controllerAction.ToLower() == "payablessearch")
        //        returnUrl = "";

        //    return returnUrl;
        //}

        public string GetMiscellaneousHelpUrl(string appController, string controllerAction)
        {
            string returnUrl = string.Empty;
            if (appController.ToLower() == "miscinvoice" && controllerAction.ToLower() == "create")
                returnUrl = "Miscellaneous/Creating_Invoice_Header.htm";

            else if (appController.ToLower() == "misccreditnote" && controllerAction.ToLower() == "create")
                returnUrl = "Miscellaneous/Creating_Miscellaneous_Credit_Note_Invoice_Header.htm";

            else if (appController.ToLower() == "unlinkedsupportingdocument" && controllerAction.ToLower() == "index")
                returnUrl = "Miscellaneous/Correcting_Supporting_Document_Linking_Errors.htm";

            else if (appController.ToLower() == "supportingdoc" && controllerAction.ToLower() == "index")
                returnUrl = "";

            return GetMultilangualHelpContentFilePath(returnUrl);
        }

        [ISAuthorize(Business.Security.Permissions.Help.ACHOpsHelpAccess)]
        public ActionResult AchOpsManageBlocks()
        {

            return Redirect(GetMultilangualHelpContentFilePath("AchOps/Managing_Blocks.htm"));
        }

        [ISAuthorize(Business.Security.Permissions.Help.ACHOpsHelpAccess)]
        public ActionResult AchOpsManagLateSubmissions()
        {
            return Redirect(GetMultilangualHelpContentFilePath("AchOps/Managing_Late_Submissions.htm"));
        }

        [ISAuthorize(Business.Security.Permissions.Help.ACHOpsHelpAccess)]
        public ActionResult AchOpsAllowedAchCurrencies()
        {
            return Redirect(GetMultilangualHelpContentFilePath("AchOps/Allowed_ACH_Currencies_of_Clearance_Setup.htm"));
        }


        [ISAuthorize(Business.Security.Permissions.Help.ICHOpsHelpAccess)]
        public ActionResult IchOpsManageBlocks()
        {
            return Redirect(GetMultilangualHelpContentFilePath("IchOps/Managing_Blocks.htm"));
        }

        [ISAuthorize(Business.Security.Permissions.Help.ICHOpsHelpAccess)]
        public ActionResult IchOpsManagLateSubmissions()
        {
           return Redirect(GetMultilangualHelpContentFilePath("IchOps/Managing_Late_Submissions.htm"));
        }

        [ISAuthorize(Business.Security.Permissions.Help.SisOpsHelpAccess)]
        public ActionResult IntroductionToSisOps()
        {
            return Redirect(GetMultilangualHelpContentFilePath("SisOps/Introduction_to_SIS_Ops.htm"));
        }

        /// <summary>
        /// Get Multilangual Help Content File Path depends on user's prefered language
        /// <param name="filePath">path of help file</param>
        /// </summary>
        private string GetMultilangualHelpContentFilePath(string filePath)
        {
            // code to append language code depends on user's language
            var usersPreferedLanguage = string.IsNullOrEmpty(SessionUtil.UserLanguageCode)?"en":SessionUtil.UserLanguageCode;
            
            // prefix root folder with language code ex: help/fr/filename.htm
            var newfilePath = string.Format("~/Help/{0}/", usersPreferedLanguage) + filePath;

            return newfilePath;
            #region old code 
            // if (usersPreferedLanguage.ToLower() != "en" && )
            // {
            //     // prefix root folder with language code ex: help_fr
            //     var contentFolder = string.Format("~/Help_{0}/", usersPreferedLanguage);
            //     var tempPath = filePath;
            //     filePath = string.Empty;
            //     filePath = contentFolder + tempPath;

            //     modifiedfilename = (originalfileName.Trim().ToLower() == "topic_under_construction" &&
            //                             originalfileName != string.Empty)
            //                                ? originalfileName
            //                                : string.Format( /*file name + _<user's languagecode>*/
            //                                      "{0}_{1}", originalfileName, usersPreferedLanguage);
            // }
            // else
            // {
            //     filePath = string.Format("{0}{1}", "~/Help/", filePath);
            //     modifiedfilename = originalfileName;
            // }
            // return filePath.Replace(originalfileName, modifiedfilename);
            #endregion
        }

    }
}
