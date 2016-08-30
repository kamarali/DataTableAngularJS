using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Security.Permissions;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Web;
using Iata.IS.Web.UIModel.Account;
using Iata.IS.Web.UIModel.Grid.Profile;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using iPayables;
using iPayables.Security;
using iPayables.UserManagement;
using iPayables.UserManagement.Enums;
using log4net;
using SIS.Web.UIModels.Account;
using Trirand.Web.Mvc;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Business.ICP;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;

namespace SIS.Web.Controllers
{
    [LogActions]
    [ElmahHandleError]
    public class AccountController : Controller
    {
        private const int MinPasswordLength = 7;

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private string CurrentUserName { get; set; }

        public IUserManagementManager UserManagementManager { get; set; }

        public IPermissionManager PermissionManager { get; set; }

        public IMemberManager MemberManager { get; set; }

        public IUserManager UserManager { get; set; }

        /// <summary>
        /// Gets or sets Subdivision Repository.
        /// </summary>
        public IRepository<SubDivision> SubdivisionRepository { get; set; }


        /// <summary>
        /// Gets or sets the contact repository.
        /// </summary>
        /// <value>The contact repository.</value>
        public IRepository<Contact> ContactRepository { get; set; }

        public IUserManagement AuthManager { get; set; }

        //CMP #665: User related enhancement [Sec 2.8 Conditional Redirection of Users upon Login in IS-WEB]
        private readonly ISisMemberSubStatusManager _sisMemberSubStatusManager;

        public AccountController(IUserManagement authManager, ISisMemberSubStatusManager sisMemberSubStatusManager)
        {
            AuthManager = authManager ?? new UserManagementModel();
            _sisMemberSubStatusManager = sisMemberSubStatusManager;
            // ShouldDisplayLogoutOptionProxy();
        }

        [Authorize]
        private void ShouldDisplayLogoutOptionProxy()
        {
            var bShouldDisplayLogoutOptionProxy = false;
            ViewData["Message"] = AppSettings.HeaderMessage1;
            var iUserId = 0;

            try
            {
                if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
                {
                    iUserId = int.Parse(SessionUtil.UserId.ToString());
                }

                if (iUserId > 0)
                {
                    var sisUser = AuthManager.GetUserByUserID(iUserId);

                    CurrentUserName = sisUser.FirstName;
                    SessionUtil.Username = string.Format("{0} {1}", sisUser.FirstName, sisUser.LastName);
                    SessionUtil.UserLanguageCode = sisUser.LanguageCode;

                    if (UserManagementModel.GetUserByUserIDStatic(iUserId).UserID == iUserId)
                    {
                        bShouldDisplayLogoutOptionProxy = true;
                    }
                }
            }
            catch (ISSessionExpiredException exception)
            {
                // Swallowing the exception for now.
            }

            SessionUtil.IsLogOutProxyOption = bShouldDisplayLogoutOptionProxy;
        }

        [AcceptVerbs(HttpVerbs.Head)]
        public ActionResult LogOn()
        {
            // Clear User Related Session
            SessionUtil.UserId = 0;
            SessionUtil.IsLogOutProxyOption = false;
            SessionUtil.IsLoggedIn = false;

            //For Server version
            //string UNamePassword = string.Empty;
            //Add The cookie
            var aCookie = new HttpCookie("lastVisit") { Value = DateTime.Now.ToString(), Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Add(aCookie);
            var cookie = Response.Cookies["lastVisit"];

            var responseCookie = Response.Cookies["myCookie"];
            var requestCookie = Request.Cookies["myCookie"];
            if (responseCookie == null || requestCookie == null)
            {
                ModelState.AddModelError("AccountLockedNotActive", "Cookies are not enabled in your browser. Please adjust this in your security preferences before continuing.");
            }

            // For Dev version
            ViewData["UserName"] = string.Empty;
            ViewData["Password"] = string.Empty;

            return View();
        }


        [AcceptVerbs(HttpVerbs.Get)]

        public ActionResult LogOn(string dummyparam)
        {
            //For Server version
            //string UNamePassword = string.Empty;
            //Add The cookie
            var aCookie = new HttpCookie("lastVisit") { Value = DateTime.Now.ToString(), Expires = DateTime.Now.AddDays(1) };
            Response.Cookies.Add(aCookie);
            var cookie = Response.Cookies["lastVisit"];

            var responseCookie = Response.Cookies["myCookie"];
            var requestCookie = Request.Cookies["myCookie"];
            if (responseCookie == null || requestCookie == null)
            {
                ModelState.AddModelError("AccountLockedNotActive", "Cookies are not enabled in your browser. Please adjust this in your security preferences before continuing.");
            }

            // For Dev version
            ViewData["UserName"] = string.Empty;
            ViewData["Password"] = string.Empty;

            return View();
        }



        //SCP#483886 : Error message
        //Desc:ValidateAntiforgeryToken attribute removed.
        [AcceptVerbs(HttpVerbs.Post)] 
        [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", Justification = "Needs to take same parameter type as Controller.Redirect()")]
        public ActionResult LogOn(string userName, string password, string localTimeZoneId, string returnUrl)
        {
            //region start

            //this code is added to fix the problem:
            //When Unexpected error occurs within system. after relogin 'Logoff Proxy' label showing. It should show 'Logoff'
            // Clear User Related Session

            SessionUtil.UserId = 0;
            SessionUtil.IsLogOutProxyOption = false;
            SessionUtil.IsLoggedIn = false;

            //region end

            // Read the cookie 
            var cookie = Request.Cookies["lastVisit"];

            // If cookie not read then display message
            if (cookie == null || cookie.Value == null)
            {
                ModelState.AddModelError("AccountLockedNotActive", "Cookies are not enabled in your browser. Please adjust this in your security preferences before continuing.");
                return View();
            }

            // Set Time Zone 
            SessionUtil.TimeZone = !string.IsNullOrEmpty(localTimeZoneId) ? localTimeZoneId : "Pacific Standard Time";

            // Trim User name field for whitespace
            userName = userName.Trim();

            if (!ValidateLogOn(userName, password))
            {
                return View();
            }
                    


            // get account lockout time from configuration, this is used once user has been locked after certain failed login attempt
            var timeFrame = GetConfigKeyForUserManagement("AccountLockoutTime");
            // Get User Details 
            var loginUserInfo = PermissionManager.IsUserLogOn(timeFrame,userName,null);

            if (loginUserInfo != null)
            {
                // Verify Entered Password
                loginUserInfo = AuthManager.CheckHashedPassword(loginUserInfo, password);
                //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                password = string.Empty;

                //Update LogOn User Details as per entered Username and Passwoord
                //SCPID : 221797 - Password change policy : System does not prompt the user to change their password
                // Removed Password expiry duration parameter.
                //CMP685 - User Management
                var updatedUserInfo = PermissionManager.UpdateLogOnUser(loginUserInfo.IS_USER_ID, Session.SessionID, loginUserInfo.LoginStutusId, false);
            }

            if (loginUserInfo != null)
            {
                //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                loginUserInfo.Password = string.Empty;

                var maxLoginAllowed = SystemParameters.Instance.General.MaxLoginAllowed;
                if (maxLoginAllowed == null) // ReSharper disable HeuristicUnreachableCode
                {
                    ModelState.AddModelError("InvalidLogin", "System Parameters : Please check system parameter XML");
                    return View();
                }
                // ReSharper restore HeuristicUnreachableCode
                if (loginUserInfo.ActiveSessionCount > maxLoginAllowed)
                {
                    // Login Userd reached max. count
                    ModelState.AddModelError("Sessioncount", "Too many Users Logged in ... Please try after some time.");
                    return View();
                }
            }
            else
            {
                // User doesn't exist in database
                ModelState.AddModelError("InvalidLogin", AppSettings.InvalidUsernameOrPasswordEntered);
                return View();
            }


            switch (loginUserInfo.LoginStutusId)
            {
                case (int)LoginStatus.Success:
                    {
                        FormsAuthentication.SetAuthCookie(loginUserInfo.IS_USER_ID.ToString(), false);
                        SessionUtil.UserId = loginUserInfo.IS_USER_ID;
                        SessionUtil.AdminUserId = loginUserInfo.IS_USER_ID;
                        SessionUtil.UserLanguageCode = loginUserInfo.LanguageCode;
                        SessionUtil.OperatingUserId = loginUserInfo.IS_USER_ID;
                        SessionUtil.Username = string.Format("{0} {1}", loginUserInfo.FirstName, loginUserInfo.LastName);
                        SessionUtil.UserCategory = (UserCategory)loginUserInfo.UserCategoryId;
                        SessionUtil.MemberName = loginUserInfo.MembercommercialName;

                        // Invoke Permission Manager 
                        var permissionManager = Ioc.Resolve<IPermissionManager>();

                        // Invoke Permission Manager 
                        var permissionKeyList = permissionManager.GetUserPermissions(loginUserInfo.IS_USER_ID);

                        // Save the permissions (either in the cache or a plan B data store).
                        var memberManager = Ioc.Resolve<IUserManager>(typeof(IUserManager));
                        memberManager.SaveUserPermissions(loginUserInfo.IS_USER_ID, permissionKeyList);
                        SessionUtil.MemberId = loginUserInfo.MemberId;
                        SessionUtil.IsLoggedIn = true;

                        MiscSearchCriteria miscSearchCriteria = null;

                        //CMP-665-User Related Enhancements-FRS-v1.2.doc
                        //[Sec 2.8 Conditional Redirection of Users upon Login in IS-WEB]
                        //If RedirectUponLogin is true based on IsMemberSubStatus then redirect to ManageMiscDailyPayablesInvoice page otherwise home page.
                        if (loginUserInfo.UserCategoryId == int.Parse(AppSettings.UserCategoryMember))
                        {
                            //Get value of RedirectUponLogin from sisMemberSubStatusDetails.
                            bool redirectUponLogin =
                                _sisMemberSubStatusManager.GetSisMemberSubStatusDetails(
                                    loginUserInfo.IsMemberSubStatusId).RedirectUponLogin;

                            //If RedirectUponLogin is true then redirect to ManageMiscDailyPayablesInvoice page otherwise home page.
                            if (redirectUponLogin)
                            {
                                return RedirectToAction("ManageMiscDailyPayablesInvoice", "MiscPayables", new { IsRedirectUponLogin = true });
                            }
                        }

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        //CMP685
                        // if already session found of the user, redirect to Forcesession screen and get user input 
                        if (loginUserInfo.ActiveSessionCountOfUser > 0)
                        {
                          return RedirectToAction("ForceSession", "Account");
                        }
                        else
                        {
                          return RedirectToAction("Index", "Home");
                        }

                        
                    }
                case (int)LoginStatus.Failed:
                    {                      

                        //CMP685 - User Management 
                        var isMemberDeactivated = ((loginUserInfo.MemberId > 0) && (loginUserInfo.IsMemberStatusId != 1));
                        if ((loginUserInfo.IS_USER_ID > 0) && (loginUserInfo.IsUserActive == 0))
                        {
                            // Account has been  deactivated AdministratorEmail
                          ModelState.AddModelError("AccountLockedNotActive", String.Format(AppSettings.AccountDeactivatedMessage, AppSettings.AdministratorEmail));
                        }
                        else
                        {
                            if ((loginUserInfo.IS_USER_ID > 0) && (isMemberDeactivated))
                            {
                                // Member Account has been deactivated.
                                ModelState.AddModelError("AccountLockedNotActive", String.Format(AppSettings.MemberAccountDeactivatedMessage, AppSettings.AdministratorEmail));
                            }
                            else
                            {
                              //CMP685 - User Management 
                              // if user is Locked--inform user that account has been locked for certain time period
                              if (loginUserInfo.IsUserLocked == 1)
                              {
                                ModelState.AddModelError("AccountLocked", AppSettings.AccountLockedMessage);
                                return View();
                              }

                                if ((loginUserInfo.IS_USER_ID > 0) && (loginUserInfo.PasswordIsExpired))
                                {
                                    FormsAuthentication.SetAuthCookie(loginUserInfo.IS_USER_ID.ToString(), false);
                                    SessionUtil.UserId = loginUserInfo.IS_USER_ID;

                                    // Password expired or this is the first time logging in
                                    return RedirectToAction("ChangePassword", "Account");
                                }

                                // Login Failed - Invalid UserName and Password
                                ModelState.AddModelError("InvalidLogin", AppSettings.InvalidUsernameOrPasswordEntered);
                            }
                        }
                        break;
                    }
            }
            
            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post|HttpVerbs.Get)]
        public ActionResult LogOffProxy()
        {
            if (SessionUtil.UserId == 0)
            {
                return RedirectToAction("LogOn", "Account");
            }

            Logger.Info("Sign out Proxy Started User ID: " + SessionUtil.UserId);

            // Sign out the user.
            var adminUserId = UserManagementModel.SignOutProxyByProxyUserId(SessionUtil.UserId);
            adminUserId = SessionUtil.AdminUserId;
            Logger.Info("Sign out Proxy Started Sis Ops User ID: " + SessionUtil.AdminUserId);
            // Second time trying to Log Off Proxy
            UserManagementModel.SignOutProxyByAdminID(SessionUtil.AdminUserId);

            if (adminUserId > 0)
            {

                var sisUser = AuthManager.GetUserByUserID(adminUserId);
                SessionUtil.UserId = sisUser.UserID;
                SessionUtil.UserLanguageCode = sisUser.LanguageCode;
                SessionUtil.Username = string.Format("{0} {1}", sisUser.FirstName, sisUser.LastName);
                SessionUtil.UserCategory = (UserCategory)sisUser.CategoryID;
                SessionUtil.MemberName = sisUser.Member.MemberCommercialName;
                SessionUtil.MemberId = sisUser.Member.MemberID;
                SessionUtil.IsLogOutProxyOption = false;
                SessionUtil.UserTypeId = sisUser.UserType;
                Logger.Info("Sign out Proxy Login Done : " + sisUser.UserID);

            }

            // Set MemberId to "0" explicitly as there are LogOff proxy issues as memberId is not set to zero even if we log off.
            SessionUtil.MemberId = 0;

            // Back to the LogOffProxy page.
            return View();
        }

        //SCP237121: Prevention of Unhandled exception found in IS-WEB Log 
        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LogOff()
        {
            var iUserId = 0;
            try
            {
                if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
                {
                    iUserId = Int16.Parse(SessionUtil.UserId.ToString());
                }

                // Sign out the user.
                //SCPID : 221797 - Password change policy : System does not prompt the user to change their password
                // Removed Password expiry duration parameter.
                var updatedUserInfo = PermissionManager.UpdateLogOnUser(SessionUtil.UserId,Session.SessionID,(int)LoginStatus.LogOff);

                // Clear the session state.
                Session.RemoveAll();
                Session.Abandon();
                Session.Clear();

                FormsAuthentication.SignOut();
                Logger.InfoFormat("User {0} Sucessfully Logged Off.", iUserId);
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Error Occurred During Logging Off User {0}. Error Message: {1}, Error Stack Trace {2}", iUserId, ex.Message, ex.StackTrace);

            }

            Logger.Info("Redirecting to Log in page.");
            // Back to the Login page.);

            //CMP#381 : On clicking the logoff button in IS-WEB reroute the user to the IATA customer portal page https://portal.iata.org/ISSP_Login?lang=en_US
            //return Redirect("https://portal.iata.org/ISSP_Login?lang=en_US");
            // As per Kirk Email dated November 25, 2015 9:12 PM, we are redirecting to new logoff  SIS page instead to customer portal
            //return View();
            return RedirectToAction("LogOff", "Account");
        }

        [AcceptVerbs(HttpVerbs.Get)]
         public ActionResult LogOff(bool flag = false)
         {
             return View();
         }


        [Authorize]
        public ActionResult ForceSession()
        {
          return View();
        }

        [HttpPost]
        public ActionResult DeleteUserOtherActiveSessions()
        {
          //CMP685: Delete the active session of the user here
          // Here loginStatusId passed as 5 to make it seperate case in SP to delete the data
          if(!PermissionManager.UpdateLogOnUser(SessionUtil.UserId, Session.SessionID, 5, true))
          {
            Logger.ErrorFormat("Error Occurred During [DeleteUserOtherActiveSessions] {0}. Error Message: unable to delete the sessions", SessionUtil.UserId);
          }

          return RedirectToAction("Index", "Home");
        }


        [Authorize]
        [ISAuthorize(Profile.ProxyLoginAccess)]
        public ActionResult ProxyLoginSearch()
        {
            if (SessionUtil.UserId == 0)
            {
                return RedirectToAction("LogOn", "Account");
            }
            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ISAuthorize(Profile.ProxyLoginAccess)]
        [ValidateAntiForgeryToken]
        public ActionResult ProxyLoginSearch(string userEmailData)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;
            //CMP520 - Management of Super Users
            if (userEmailData.Contains("SU"))
            {
              userEmailData = userEmailData.Remove(0, 5);
            }

            if (string.IsNullOrEmpty(userId.ToString()) == false)
            {
                if (userEmailData.Length < 150 && userEmailData.Length > 0)
                {
                    if (UserManagementModel.SignInProxy(userId.ToString(), userEmailData))
                    {
                        var proxyLoginUserdetails = AuthManager.GetProxyUserByAdminUserID(userId);
                        if (proxyLoginUserdetails != null)
                        {
                            SessionUtil.UserId = proxyLoginUserdetails.UserID;
                            SessionUtil.UserLanguageCode = proxyLoginUserdetails.LanguageCode;
                            SessionUtil.Username = string.Format("{0} {1}", proxyLoginUserdetails.FirstName, proxyLoginUserdetails.LastName);
                            SessionUtil.UserCategory = (UserCategory)proxyLoginUserdetails.CategoryID;
                            SessionUtil.MemberName = proxyLoginUserdetails.Member.MemberCommercialName;
                            SessionUtil.MemberId = proxyLoginUserdetails.Member.MemberID;
                            SessionUtil.IsLogOutProxyOption = true;
                            SessionUtil.UserTypeId = proxyLoginUserdetails.UserType;

                            //SCP300357: XB-Q06-BidAir - user permisison missing
                            // Invoke Permission Manager 
                            var permissionManager = Ioc.Resolve<IPermissionManager>();
                            // Invoke Permission Manager 
                            var permissionKeyList = permissionManager.GetUserPermissions(proxyLoginUserdetails.UserID);
                            // Save the permissions (either in the cache or a plan B data store).
                            var memberManager = Ioc.Resolve<IUserManager>(typeof(IUserManager));
                            memberManager.SaveUserPermissions(proxyLoginUserdetails.UserID, permissionKeyList);
                        }

                        return RedirectToAction("ProxyLoginSuccess", "Account");
                    }
                }
                else
                {
                    TempData[ViewDataConstants.ErrorMessage] = "Error in Proxy Login, Please Check Inputs";
                }

                return View();
            }

            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        [AcceptVerbs(new[] { "Post" })]
        public ActionResult GetMemberNames(object param)
        {
            var memberListItems = new List<MemberListItem>();
            MemberListItem item;
            JsonResult newJsonMemberList;

            try
            {
                var reader = new StreamReader(Request.InputStream);
                var entityCode = reader.ReadToEnd();
                var categoryCode = int.Parse(entityCode);
                //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
                reader.Close();
                if (AuthManager.DoesCategoryIDContainMembers(categoryCode))
                {
                    var members = AuthManager.GetAllMembers();
                    IEnumerable<I_ISMember> sortedEnum = members.OrderBy(f => f.MemberCommercialName);
                    IList<I_ISMember> sortedList = sortedEnum.ToList();

                    foreach (var member in sortedList)
                    {
                        item = new MemberListItem { MemberName = member.MemberCommercialName, MemberCode = member.MemberID.ToString() };
                        memberListItems.Add(item);
                    }
                }

                newJsonMemberList = Json(memberListItems);
                return newJsonMemberList;
            }
            catch (Exception exception)
            {
                Logger.Error("Error in getting member names", exception);
                item = new MemberListItem { MemberName = string.Empty, MemberCode = string.Empty };
                memberListItems.Add(item);

                newJsonMemberList = Json(memberListItems);
                return newJsonMemberList;
            }
        }

        [AcceptVerbs(new[] { "Post" })]
        public ActionResult GetMemberLocationsByMember(object param)
        {
            var memberLocationList = new List<LocationListItem>();
            LocationListItem item;
            JsonResult newJsonMemberLocationList;

            try
            {
                var reader = new StreamReader(Request.InputStream);
                var entityCode = reader.ReadToEnd();
                List<ILocation> memberCodeDB;
                if (SessionUtil.MemberId <= 0)
                    memberCodeDB = AuthManager.GetAllMemberLocations(int.Parse(entityCode));
                else
                    memberCodeDB = AuthManager.GetAllMemberLocations(SessionUtil.MemberId);
                //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
                reader.Close();
                foreach (var location in memberCodeDB)
                {
                    item = new LocationListItem { LocationId = location.LOCATION_ID, LocationCode = location.LOCATION_CODE };
                    memberLocationList.Add(item);
                }

                newJsonMemberLocationList = Json(memberLocationList);
                return newJsonMemberLocationList;
            }
            catch (Exception exception)
            {
                Logger.Error("Error getting member locations.", exception);

                item = new LocationListItem { LocationId = 0, LocationCode = string.Empty };
                memberLocationList.Add(item);
                newJsonMemberLocationList = Json(memberLocationList);
                return newJsonMemberLocationList;
            }
        }

        [Authorize]
        [AcceptVerbs(new[] { "Post" })]
        public ActionResult GetMemberLocationByLocationID(object param)
        {
            var memberLocationList = new List<MemberLocationListItem>();
            JsonResult jsonMemberLocationList;

            try
            {
                var reader = new StreamReader(Request.InputStream);
                var entityCode = reader.ReadToEnd();

                var locations = AuthManager.GetMemberLocationsByMemberLocationId(entityCode);
                //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
                reader.Close();
                if (locations.Count() < 1)
                {
                    memberLocationList.Add(new MemberLocationListItem());
                }
                else
                {
                    var loc = locations[0];
                    var memlocation = new MemberLocationListItem { Address1 = loc.Address1, Address2 = loc.Address2, Address3 = loc.Address3, PostalCode = loc.POSTAL_CODE };

                    var countries = (List<MST_COUNTRY>)UserManagementModel.GetAllCountries();
                    var userCountry = from MST_COUNTRY country in countries
                                      where country.COUNTRY_CODE == loc.COUNTRY_CODE
                                      select country;
                    if (userCountry.Count() > 0)
                    {
                        memlocation.CountryCode = userCountry.FirstOrDefault().COUNTRY_CODE;
                        memlocation.CountryName = userCountry.FirstOrDefault().COUNTRY_NAME;
                    }
                    else
                    {
                        memlocation.CountryCode = string.Empty;
                    }

                    // Get SubDivisions
                    //IEnumerable<MST_SUB_DIVISION> subDivisions = from MST_SUB_DIVISION subDivision in (IEnumerable<MST_SUB_DIVISION>)UserManagementModel.GetAllSubDivisions()
                    //                                             where subDivision.COUNTRY_CODE == loc.COUNTRY_CODE
                    //                                             orderby subDivision.SUB_DIVISION_NAME
                    //                                             select subDivision;

                    //var userSubDivision = from MST_SUB_DIVISION subDivision in subDivisions
                    //                      where subDivision.SUB_DIVISION_CODE == loc.SUB_DIVISION_CODE
                    //                      select subDivision;

                    var userSubDivision = (List<MST_SUB_DIVISION>)UserManagementModel.GetSelectedSubDivisions(loc.COUNTRY_CODE, loc.SUB_DIVISION_CODE);

                    if (userSubDivision.Count() > 0)
                    {
                        memlocation.SubDivisionCode = userSubDivision.FirstOrDefault().SUB_DIVISION_CODE;
                        memlocation.SubDivisionName = userSubDivision.FirstOrDefault().SUB_DIVISION_NAME;
                    }

                    else
                    {
                        memlocation.SubDivisionCode = string.Empty;
                    }

                    memlocation.CityCode = loc.CITY_NAME;
                    memberLocationList.Add(memlocation);
                }

                jsonMemberLocationList = Json(memberLocationList);
                return jsonMemberLocationList;
            }
            catch (Exception exception)
            {
                Logger.Error("Error getting member location for a specified member location id.", exception);

                memberLocationList.Add(new MemberLocationListItem());
                jsonMemberLocationList = Json(memberLocationList);

                return jsonMemberLocationList;
            }
        }

        public ActionResult CheckForSession()
        {
            var userId = SessionUtil.UserId;

            var adminuserId = SessionUtil.AdminUserId;

            if (userId != adminuserId)
            {
                var sisUser = AuthManager.GetUserByUserID(adminuserId);
                SessionUtil.UserId = sisUser.UserID;
                SessionUtil.UserLanguageCode = sisUser.LanguageCode;
                SessionUtil.Username = string.Format("{0} {1}", sisUser.FirstName, sisUser.LastName);
                SessionUtil.UserCategory = (UserCategory)sisUser.CategoryID;
                SessionUtil.MemberName = sisUser.Member.MemberCommercialName;
                SessionUtil.MemberId = sisUser.Member.MemberID;
                SessionUtil.IsLogOutProxyOption = false;
                SessionUtil.UserTypeId = sisUser.UserType;
                Logger.Info("Sign out Proxy Login Done : " + sisUser.UserID);
                return Json(true);
            }

            return Json(false);
        }
        [Authorize]
        [AcceptVerbs(new[] { "Post" })]
        public ActionResult GetUsers(object param)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;

            var userListItems = new List<UserListItem>();

            try
            {
                var sisUser = AuthManager.User;

                if (!String.IsNullOrEmpty(userId.ToString()))
                {
                    sisUser = AuthManager.GetUserByUserID(int.Parse(userId.ToString()));
                }

                var reader = new StreamReader(Request.InputStream);
                var entityCode = reader.ReadToEnd();

                IEnumerable<IS_USER> userCodeDb = AuthManager.GetUsersListByMemberId(entityCode).ToList();
                //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
                reader.Close();

                if (sisUser.UserID > 0)
                {
                  userCodeDb = from user in userCodeDb
                               where user.IS_USER_ID != sisUser.UserID && user.IS_LOCKED == false
                               orderby user.EMAIL_ID ascending
                               select user;
                  //CMP520 - Management of Super Users
                  foreach (var isUser in userCodeDb.Where(isUser => isUser.USER_TYPE))
                    isUser.EMAIL_ID = string.Format("{0} {1}", "(SU)", isUser.EMAIL_ID);
                }

               foreach (var iu in userCodeDb)
                {
                  var ulItem = new UserListItem { UserEmail = iu.EMAIL_ID };
                   userListItems.Add(ulItem);
                }

                if (userListItems.Count() < 1)
                {
                    return Json(userListItems);
                }

                var newJsonUserList = Json(userListItems);
                return newJsonUserList;
            }
            catch (Exception exception)
            {
                Logger.Error("Error in GetUsers", exception);
                var userListItemReference = new UserListItem { UserEmail = string.Empty };
                userListItems.Add(userListItemReference);

                return Json(userListItems);
            }
        }

        [Authorize]
        [AcceptVerbs(new[] { "Post" })]
        public ActionResult GetUsersByCategory(object param)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;
            var userListItems = new List<UserListItem>();
            var reader = new StreamReader(Request.InputStream);
            var entityCode = reader.ReadToEnd();
            var intEntityCode = int.Parse(entityCode);

            var sisUser = AuthManager.User;
            //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
            reader.Close();
            if (!String.IsNullOrEmpty(userId.ToString()))
            {
                sisUser = AuthManager.GetUserByUserID(int.Parse(userId.ToString()));
            }

            if (intEntityCode > 0)
            {
                IEnumerable<I_ISUser> userCodeDB = AuthManager.GetUsersByCategory(AuthManager.GetUserCategories()[intEntityCode - 1]);
                //SCP52196 - SIS OPS super users ID  
                //Logic behind the populating users in auto populating field while proxy login for SIS Ops User has been updated, i.e. in auto populating text box field there will be no SIS Ops super user for selection.
                //Now on any SIS Ops user or SIS Ops super user will not able to proxy login to any other SIS Ops super user.

                if (sisUser.UserID > 0)
                {
                    if (intEntityCode == 1)
                    {
                        userCodeDB = from user in userCodeDB
                                     where user.UserID != sisUser.UserID && user.IsLocked == false && user.UserType != 1
                                     orderby user.Email ascending
                                     select user;
                        //CMP520 - Management of Super Users
                        foreach (var isUser in userCodeDB.Where(isUser => isUser.UserType == 1))
                          isUser.Email = string.Format("{0} {1}", "(SU)", isUser.Email);
                    }
                    else
                    {
                        userCodeDB = from user in userCodeDB
                                     where user.UserID != sisUser.UserID && user.IsLocked == false
                                     orderby user.Email ascending
                                     select user;
                        //CMP520 - Management of Super Users
                        foreach (var isUser in userCodeDB.Where(isUser => isUser.UserType == 1))
                          isUser.Email = string.Format("{0} {1}", "(SU)", isUser.Email);
                    }
                }

                foreach (var iu in userCodeDB)
                {
                    var ulItem = new UserListItem { UserEmail = iu.Email };
                    userListItems.Add(ulItem);
                }
            }

            if (userListItems.Count() < 1)
            {
                return Json(userListItems);
            }

            var jsonUserList = Json(userListItems);
            return jsonUserList;
        }

        [Authorize]
        public ActionResult Register(string superUserCreation, int SelectedMemberId=0)
        {
            SessionUtil.ManageUserSearchCriteria = null;
            var isSuperUserCreation = false;
            if (superUserCreation != null)
            {
                if (superUserCreation == "1")
                {
                    isSuperUserCreation = true;
                }
            }
            else
            {
                 SessionUtil.MemberName = string.Empty;
            }

            if (SessionUtil.UserId > 0)
            {
                PopulateRegisterForm(isSuperUserCreation);
            }
            else
            {
                return RedirectToAction("LogOn", "Account");
            }


            if (SelectedMemberId == 0 && SessionUtil.MemberId > 0)
            {
              SelectedMemberId = SessionUtil.MemberId;
            }
            
            ViewData["SelectedMemberId"] = SelectedMemberId;
            ViewData["IsSuperUserCreation"] = isSuperUserCreation;

            // CMP#668: Archival of IS-WEB Users and Removal from Screens
            TempData["isUserCreation"] = "True";

            return View();
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ISAuthorize(Profile.CreateOrManageUsersAccess)]
        [ValidateAntiForgeryToken]
        public ActionResult Register(CreateUserView userProfileInfoFromForm, bool isSuperUserCreation, string SelectedMemberId)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;

          if (string.IsNullOrEmpty(userId.ToString()) == false)
          {
              if (!ValidateRegistration(userProfileInfoFromForm, true))
              {
                  PopulateRegisterForm(isSuperUserCreation);
                  return View();
              }

              // Get Current Logged User's Information
              var sisUser = AuthManager.GetUserByUserID(int.Parse(userId.ToString()));
              var isSysAdmin = (sisUser.CategoryID == int.Parse(AppSettings.UserCategorySysOps));
              var isSuperUser = sisUser.UserType;

              // Generate New Password
              var dataSecurity = new DataSecurity();
              var salt = dataSecurity.CreateRandomSalt(6);
              var password = dataSecurity.Password.GenerateRandomPassword(8);
              var hashedPassword = dataSecurity.HashString(password, ref salt);

              // Create User Object
              var newUser = AuthManager.User;

              if (isSuperUserCreation)
              {
                  userProfileInfoFromForm.UserCategoryData = AppSettings.UserCategoryMember;
                  userProfileInfoFromForm.MemberData = Convert.ToString(SelectedMemberId);
                  newUser.UserType = 1;
              }

              // Super user  can create only super user except Sis Ops
              if (isSuperUser == 1)
              {
                  switch (Convert.ToInt32(userProfileInfoFromForm.UserCategoryData))
                  {
                      case (int) UserCategory.SisOps:
                          newUser.UserType = 0;
                          break;

                      case (int) UserCategory.IchOps:
                          newUser.UserType = 1;
                          break;

                      case (int) UserCategory.AchOps:
                          newUser.UserType = 1;
                          break;

                      case (int) UserCategory.Member:
                          newUser.UserType = 1;
                          break;
                  }
              }

              // Place code here to get Selected Category from dropdown box
              if (isSysAdmin)
              {
                  if (string.IsNullOrEmpty(userProfileInfoFromForm.UserCategoryData))
                  {
                      ModelState.AddModelError("NoUserCategorySelected", AppSettings.UserCategoryRequired);
                      PopulateRegisterForm(isSuperUserCreation);
                      return View();
                  }

                  newUser.CategoryID = int.Parse(userProfileInfoFromForm.UserCategoryData);

                  if ((newUser.CategoryID > 0) && (newUser.CategoryID == int.Parse(AppSettings.UserCategoryMember)))
                  {
                      if ((string.IsNullOrEmpty(userProfileInfoFromForm.MemberData) == false) &&
                          (int.Parse(userProfileInfoFromForm.MemberData) <= 0))
                      {
                          ModelState.AddModelError("NoMemberSelected", AppSettings.MemberSelectionRequired);
                          PopulateRegisterForm(isSuperUserCreation);
                          return View();
                      }
                  }
              }
              else
              {
                  newUser.CategoryID = sisUser.CategoryID;
              }

              newUser.StaffID = string.IsNullOrEmpty(userProfileInfoFromForm.StaffID) == false
                                    ? userProfileInfoFromForm.StaffID
                                    : null;

              newUser.Department = string.IsNullOrEmpty(userProfileInfoFromForm.StaffID) == false
                                       ? userProfileInfoFromForm.Department
                                       : null;

              newUser.Division = string.IsNullOrEmpty(userProfileInfoFromForm.Divison) == false
                                     ? userProfileInfoFromForm.Divison
                                     : null;

              if (!string.IsNullOrEmpty(userProfileInfoFromForm.UserLanguageCode))
              {
                  newUser.LanguageCode = userProfileInfoFromForm.UserLanguageCode;
              }
              else
              {
                  newUser.LanguageCode = "en";
              }

              if ((string.IsNullOrEmpty(userProfileInfoFromForm.LocationID)) ||
                  (userProfileInfoFromForm.LocationID.Equals("0")))
              {
                  newUser.Location.Address1 = string.IsNullOrEmpty(userProfileInfoFromForm.Address1) == false
                                                  ? userProfileInfoFromForm.Address1
                                                  : null;

                  newUser.Location.Address2 = string.IsNullOrEmpty(userProfileInfoFromForm.Address2) == false
                                                  ? userProfileInfoFromForm.Address2
                                                  : null;

                  newUser.Location.Address3 = string.IsNullOrEmpty(userProfileInfoFromForm.Address3) == false
                                                  ? userProfileInfoFromForm.Address3
                                                  : null;

                  newUser.Location.POSTAL_CODE = String.IsNullOrEmpty(userProfileInfoFromForm.PostalCode) == false
                                                     ? userProfileInfoFromForm.PostalCode
                                                     : null;

                  newUser.Location.SUB_DIVISION_CODE = (string.IsNullOrEmpty(userProfileInfoFromForm.SubDivisionName) ==
                                                        false)
                                                           ? userProfileInfoFromForm.SubDivisionCode
                                                           : null;

                  newUser.Location.CITY_NAME = userProfileInfoFromForm.CityName;
                  var countryFound = false;
                  if (!string.IsNullOrEmpty(userProfileInfoFromForm.CountryName))
                  {
                      newUser.Location.COUNTRY_CODE = userProfileInfoFromForm.CountryCode;
                      countryFound = true;
                  }

                  if (countryFound == false)
                  {
                      newUser.Location.CountryID = 0;
                  }
              }
              else
              {
                  newUser.Location.LocationID = userProfileInfoFromForm.LocationID;
              }

              newUser.Location.SITA_Address = string.IsNullOrEmpty(userProfileInfoFromForm.SITAAddress) == false
                                                  ? userProfileInfoFromForm.SITAAddress
                                                  : null;

              newUser.Email = userProfileInfoFromForm.EmailAddress;
              newUser.FirstName = userProfileInfoFromForm.FirstName;

              newUser.LastName = string.IsNullOrEmpty(userProfileInfoFromForm.LastName)
                                     ? " "
                                     : userProfileInfoFromForm.LastName.Trim(); // SCP444865 - Two-word last name

              newUser.IsLocked = false;
              newUser.IsActive = true;
              
              // CMP#668: Archival of IS-WEB Users and Removal from Screens
              newUser.IsArchived = false;

              newUser.Phone.Fax = string.IsNullOrEmpty(userProfileInfoFromForm.Fax) == false
                                      ? userProfileInfoFromForm.Fax
                                      : null;

              newUser.Phone.MobileNumber = string.IsNullOrEmpty(userProfileInfoFromForm.Mobile) == false
                                               ? userProfileInfoFromForm.Mobile
                                               : null;

              newUser.Phone.PhoneNumber1 = userProfileInfoFromForm.Telephone1;

              newUser.Phone.PhoneNumber2 = String.IsNullOrEmpty(userProfileInfoFromForm.Telephone2) == false
                                               ? userProfileInfoFromForm.Telephone2
                                               : null;

              newUser.PasswordExpirationDateTime = DateTime.UtcNow.AddDays(-1);

              newUser.Password = hashedPassword;
              newUser.Salt = salt;
              newUser.ModifiedBy = sisUser.UserID;

              newUser.PositionOrTitle = String.IsNullOrEmpty(userProfileInfoFromForm.PositionTitle) == false
                                            ? userProfileInfoFromForm.PositionTitle
                                            : null;

              newUser.Salutation = userProfileInfoFromForm.Salutation;

              if (isSysAdmin)
              {
                  if ((newUser.CategoryID == int.Parse(AppSettings.UserCategoryMember)) &&
                      (string.IsNullOrEmpty(userProfileInfoFromForm.MemberData) == false))
                  {
                      newUser.Member.MemberID = int.Parse(userProfileInfoFromForm.MemberData);
                  }
                  else
                  {
                      newUser.Member.MemberID = 0;
                  }
              }
              else
              {
                  newUser.Member.MemberID = sisUser.Member.MemberID;
              }

              SessionUtil.UserLanguageCode = newUser.LanguageCode;

              newUser.SessionID = null;

              sisUser = AuthManager.CreateNewUser(newUser);
              if (sisUser.UserID < 1)
              {
                  ModelState.AddModelError("DatabaseEntryFailed", AppSettings.DatabaseEntryFailedMessage);
                  PopulateRegisterForm(isSuperUserCreation);

                  return View();
              }

              //CMP685 - User Management
              // If new user creation is successfull, create a dynamic reset password link and send on user email id to create login password
              string carrierFlag = "N"; string urlPath = Path.Combine(SystemParameters.Instance.General.LogOnURL, AppSettings.UrlPasswordResetLink).Replace("\\", "/");
              if (!UserManagementManager.ResetUserPassword(sisUser.UserID, urlPath, ref carrierFlag))
              {
                ModelState.AddModelError("FailedToCreateUserCredentials", "Some error occured while creating user credentials!!");
                PopulateRegisterForm(isSuperUserCreation);

                return View();
              }

              //CMP #665: Conditional Suppression of OTP Email to New Member Users.
              //This change is applicable for creation of new users of User Category ‘Member’ ONLY. The New Member User could be a Super User or a Normal User.
              //suppressOtbEmail is true, will not send password email otherwise will send.
              //This change is not applicable for new users belonging to User Categories ‘SIS Ops’, ‘ICH Ops’ and ‘ACH Ops’
              if (sisUser.CategoryID == int.Parse(AppSettings.UserCategoryMember))
              {
                  //Get suppressOtbEmail value from master sisMemberSubStatus.
                  bool suppressOtpEmail =
                      _sisMemberSubStatusManager.GetSisMemberSubStatusDetails(sisUser.Member.ISMembershipSubStatus).SuppressOtpEmail;
                  if (!suppressOtpEmail)
                  {
                    UserManagementManager.SendUserWelcomeNotification(sisUser.FirstName, sisUser.LastName, sisUser.Email, carrierFlag);
                      Logger.InfoFormat(
                          "Welcome notification has been sent to user, suppressOtpEmail: {0}, ISMembershipSubStatus: {1}",
                          suppressOtpEmail, sisUser.Member.ISMembershipSubStatus);
                  }
                  else
                  {
                      Logger.InfoFormat(
                          "Welcome notification did not send because suppressOtpEmail: {0}, ISMembershipSubStatus: {1}",
                          suppressOtpEmail, sisUser.Member.ISMembershipSubStatus);
                  }

                  //CMP #655: IS-WEB Display per Location ID
                  //2.2	NEW USER CREATION USING IS-WEB
                  TempData["ShouldShowContAssoMessage"] = "true";

              }
              else
              {
                UserManagementManager.SendUserWelcomeNotification(sisUser.FirstName, sisUser.LastName, sisUser.Email, carrierFlag);                   
              }

              //CMP #665: User Related Enhancements-FRS-v1.2 [sec 2.4.3	Web Service Call to ICP].
              //Request Type "O": Create user first time(not retry), ActionType 1: Create User, ActionType 2: Enable User, ActionType 3: Disable User  
              new ReferenceManager().EnqueueMessageInIcpLogConsumer(sisUser.Email, "O", 1);


            if (sisUser.UserType == 1 || sisUser.CategoryID == 1)
                {
                    // TO FIX SPIRA ISSUE# 4840
                    //if (isSuperUser == 1)
                    //{
                    if (sisUser.UserType == 1)
                    {
                        PermissionManager.AssignPermissionToSuperUser(sisUser.UserID, sisUser.CategoryID);
                    }
                    else
                    {
                        if (sisUser.CategoryID == 1)
                        {
                            PermissionManager.AssignPermissionToSisOpsNormalUser(sisUser.UserID, sisUser.CategoryID);
                        }
                    }
                    //}
                }

                MemberManager.IsUserEmailIdInMemberContact(sisUser.Email, newUser.Member.MemberID);

                TempData["ShouldEdit"] = sisUser.Email;
                TempData["ShowSavedMessage"] = "true";
                //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
                password = hashedPassword =  string.Empty;

                return RedirectToAction("UserEditFromMember", "Account");
            }

            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        private void PopulateRegisterForm(bool IsSuperUserCreation = false)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;
            
            if (string.IsNullOrEmpty(userId.ToString()) == false)
            {
                try
                {
                    var sisUser = AuthManager.GetUserByUserID(int.Parse(userId.ToString()));
                    var editOtherEmail = (string)TempData["ShouldEdit"];
                    //SCP207710 - Change Super User(Allow valid special character).
                    if ((string.IsNullOrWhiteSpace(editOtherEmail) == false) && Regex.IsMatch(editOtherEmail.ToLower(), FormatConstants.ValidEmailPattern))
                    {
                        sisUser = AuthManager.GetUserByUserName(editOtherEmail);

                        //CMP-520: Management of Super Users
                        /*Super User Check box will be an editable field only under the following circumstances:
                          i)  The screen mode is SIS Ops i.e. only if logged-in user belongs to SIS Ops category 
                               AND
                          ii) The user whose details are displayed on the screen belongs to the following categories:
                             ICH Ops or ACH Ops or Member Users i.e. "editOtherEmail's Category" is other than SIS Ops*/

                        if (sisUser.CategoryID == (int) UserCategory.SisOps)
                        {
                            ViewData["MakeCheckBoxEditable"] = false;
                        }
                        else
                            ViewData["MakeCheckBoxEditable"] = SessionUtil.UserCategory == UserCategory.SisOps;
                    }

                    var bSysAdmin = (sisUser.CategoryID == int.Parse(AppSettings.UserCategorySysOps));
                    var bUserIsMember = (sisUser.CategoryID == int.Parse(AppSettings.UserCategoryMember));
                    var bUserModify = false;

                    if ((ViewData.ContainsKey("UserModify")) && (ViewData["UserModify"] != null))
                    {
                        bUserModify = (bool)ViewData["UserModify"];
                    }

                    if (bSysAdmin)
                    {
                        if (bUserModify == false)
                        {
                            if (!IsSuperUserCreation)
                            {
                                ViewData["ISOPS"] = bSysAdmin;
                            }

                            var userCategoryList = new List<UserCategoryListItem>();
                            var userCategories = AuthManager.GetUserCategories();
                            var userCategoryCode = 0;
                            foreach (var cat in userCategories)
                            {
                                userCategoryCode = userCategoryCode + 1;
                                var item = new UserCategoryListItem { UserCategoryName = cat, UserCategoryCode = userCategoryCode.ToString() };
                                userCategoryList.Add(item);
                            }

                            ViewData["UserCategoryList"] = userCategoryList;

                            var memberList = new List<MemberListItem>();
                            var memberitem = new MemberListItem { MemberCode = string.Empty, MemberName = string.Empty };
                            memberList.Add(memberitem);
                            ViewData["MemberList"] = memberList;
                        }
                    }
                    else
                    {
                        if (ViewData.ContainsKey("ISOPS"))
                        {
                            ViewData.Remove("ISOPS");
                        }
                    }

                    var locationCode = new List<LocationListItem>();
                    var customLocationListItem = new LocationListItem { LocationCode = string.Empty, LocationId = 0 };

                    // Create Custom List Item
                    locationCode.Add(customLocationListItem);

                    // Get Locations By MemberID
                    if (bUserIsMember)
                    {
                        // Get All Member Locations
                        IEnumerable<MEM_LOCATION> allLocations =
                            AuthManager.GetAllMemberLocationsByID(sisUser.Member.MemberID);

                        allLocations = from loc in allLocations
                                       where loc.MEMBER_ID == sisUser.Member.MemberID
                                       select loc;

                        // Create List Items
                        foreach (var ml in allLocations)
                        {
                            var locationCodeListItemReference = new LocationListItem
                                                                    {
                                                                        LocationCode = ml.LOCATION_CODE,
                                                                        LocationId = ml.LOCATION_ID
                                                                    };
                            locationCode.Add(locationCodeListItemReference);
                        }
                    }

                    // Display Member Locations
                    ViewData["LocationCodeList"] = locationCode;
                    ViewData["SubDivisionName"] = new SelectList(new List<string>());
                    ViewData["CityName"] = sisUser.Location.CITY_NAME;

                    // Get All salutations For Salutation Drop Down List
                    var salutationList = new List<SalutationItem>();

                    var blankSalutationItem = new SalutationItem { SalutationName = "" };
                    salutationList.Add(blankSalutationItem);

                    foreach (var currentSalutationItemString in Enum.GetNames(typeof(Salutation)))
                    {
                        var currentSalutationItem = new SalutationItem();
                        if (currentSalutationItemString.ToUpper().Equals("UNDEFINED") == false)
                        {
                            currentSalutationItem.SalutationName = currentSalutationItemString;
                            salutationList.Add(currentSalutationItem);
                        }
                    }

                    ViewData["SalutationList"] = salutationList;

                    // Get All Countries
                    var countryList = (List<MST_COUNTRY>)UserManagementModel.GetAllCountries();
                    countryList.Sort(CompareCountriesByCountryName);
                    var countryNamesList = new List<CountryListItem>();

                    var itemNoSelect = new CountryListItem { CountryName = "Select", CountryCode = "" };
                    countryNamesList.Add(itemNoSelect);
                    foreach (var mc in countryList)
                    {
                        var country = new CountryListItem { CountryName = mc.COUNTRY_NAME, CountryCode = mc.COUNTRY_CODE };

                        countryNamesList.Add(country);
                    }

                    ViewData["CountryNameList"] = countryNamesList;
                }
                catch (Exception ex)
                {
                    Logger.Error("Error in PopulateRegisterForm", ex);
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [Authorize]
        private CreateUserView PopulateModifyUserForm(string editOtherUser)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;

            var userProfileInfoFromForm = new CreateUserView();
            if (userId > 0)
            {
                try
                {
                    var sisUser = AuthManager.GetUserByUserID(userId);

                    //SCP207710 - Change Super User(Allow valid special character).
                    if ((string.IsNullOrWhiteSpace(editOtherUser) == false) && Regex.IsMatch(editOtherUser.ToLower(), FormatConstants.ValidEmailPattern))
                    {
                        sisUser = AuthManager.GetUserByUserName(editOtherUser);
                        userProfileInfoFromForm.HiddenEmailAddress = editOtherUser;
                    }

                    var emailOfPersonToEdit = (string)TempData["ShouldEdit"];
                    if (string.IsNullOrWhiteSpace(emailOfPersonToEdit) == false)
                    {
                        ViewData["ShouldEdit"] = emailOfPersonToEdit;
                    }

                    if (String.IsNullOrEmpty(emailOfPersonToEdit) == false)
                    {
                        userProfileInfoFromForm.EmailAddress = emailOfPersonToEdit;
                        sisUser = AuthManager.GetUserByUserName(emailOfPersonToEdit);
                        ViewData["ShouldShowRoleEdit"] = true;
                    }
                    else
                    {
                        userProfileInfoFromForm.EmailAddress = sisUser.Email;
                    }

                    var bIsMemberUser = (sisUser.CategoryID == int.Parse(AppSettings.UserCategoryMember));

                    userProfileInfoFromForm.Salutation = sisUser.Salutation;
                    userProfileInfoFromForm.FirstName = sisUser.FirstName;
                    userProfileInfoFromForm.LastName = sisUser.LastName;
                    userProfileInfoFromForm.PositionTitle = sisUser.PositionOrTitle;
                    userProfileInfoFromForm.StaffID = sisUser.StaffID;
                    userProfileInfoFromForm.Divison = sisUser.Division;
                    userProfileInfoFromForm.Department = sisUser.Department;
                    userProfileInfoFromForm.Address1 = sisUser.Location.Address1;
                    userProfileInfoFromForm.Address2 = sisUser.Location.Address2;
                    userProfileInfoFromForm.Address3 = sisUser.Location.Address3;
                    userProfileInfoFromForm.PostalCode = sisUser.Location.POSTAL_CODE;
                    userProfileInfoFromForm.SITAAddress = sisUser.Location.SITA_Address;
                    userProfileInfoFromForm.Telephone1 = sisUser.Phone.PhoneNumber1;
                    userProfileInfoFromForm.Telephone2 = sisUser.Phone.PhoneNumber2;
                    userProfileInfoFromForm.Mobile = sisUser.Phone.MobileNumber;
                    userProfileInfoFromForm.Fax = sisUser.Phone.Fax;
                    userProfileInfoFromForm.CityName = sisUser.Location.CityName;
                    userProfileInfoFromForm.CityData = sisUser.Location.CityName;
                    userProfileInfoFromForm.UserLanguageCode = sisUser.LanguageCode;
                    // SCP#412041: KALE: Issue with user creation.
                    //userProfileInfoFromForm.HiddenEmailAddress = editOtherUser;
                    userProfileInfoFromForm.HiddenEmailAddress = sisUser.Email;
                    userProfileInfoFromForm.UserType = Convert.ToBoolean(sisUser.UserType);
                   
                    // CMP#668: Archival of IS-WEB Users and Removal from Screens
                    userProfileInfoFromForm.IsArchived = sisUser.IsArchived;
                  //CMP685: after this CMP IS_ACTIVE will be used to activate and deactivate account in place of IS_LOCKED  
                  TempData["isUserActive"] = sisUser.IsActive;

                    // Get Countries
                    var countries = (List<MST_COUNTRY>)UserManagementModel.GetAllCountries();
                    var userCountry = from MST_COUNTRY country in countries
                                      where country.COUNTRY_CODE == sisUser.Location.COUNTRY_CODE
                                      select country;
                    // Get SubDivisions
                    //var subDivisions = from MST_SUB_DIVISION subDivision in (IEnumerable<MST_SUB_DIVISION>)UserManagementModel.GetAllSubDivisions()
                    //                   where subDivision.COUNTRY_CODE == sisUser.Location.COUNTRY_CODE
                    //                   select subDivision;

                    //Added by Ranjit Kumar-04-07-2011

                    //var sbDivisions = (List<MST_SUB_DIVISION>)UserManagementModel.GetAllSubDivisions();
                    var sbDivisions = (IEnumerable<MST_SUB_DIVISION>)UserManagementModel.GetAllSubDivisions();

                    var subDivisionName = (List<MST_SUB_DIVISION>)UserManagementModel.GetSelectedSubDivisions(sisUser.Location.COUNTRY_CODE, sisUser.Location.SUB_DIVISION_CODE);

                    //var subDivisions = from MST_SUB_DIVISION subDivision in sbDivisions.AsEnumerable()
                    //                   where subDivision.COUNTRY_CODE == sisUser.Location.COUNTRY_CODE
                    //                   select subDivision;

                    //var userSubDivision = from MST_SUB_DIVISION subDivision in subDivisions.AsEnumerable()
                    //                      where subDivision.SUB_DIVISION_CODE == sisUser.Location.SUB_DIVISION_CODE
                    //                      select subDivision;

                    //var userSubDivision = from MST_SUB_DIVISION subDivision in sbDivisions
                    //                      where subDivision.COUNTRY_CODE == sisUser.Location.COUNTRY_CODE && subDivision.SUB_DIVISION_CODE == sisUser.Location.SUB_DIVISION_CODE
                    //                      select subDivision;

                    // Create Country List
                    var countryList = new List<CountryListItem>();
                    var countryListItem = new CountryListItem();
                    foreach (var country in countries)
                    {
                        countryListItem = new CountryListItem { CountryName = country.COUNTRY_NAME, CountryCode = country.COUNTRY_CODE };
                        countryList.Add(countryListItem);
                    }

                    // Get Selected User Country
                    if (userCountry.Count() > 0)
                    {
                        var country = userCountry.First();
                        countryListItem = new CountryListItem { CountryName = country.COUNTRY_NAME, CountryCode = country.COUNTRY_CODE };
                    }
                    else
                    {
                        countryListItem.CountryCode = "0";
                        countryListItem.CountryName = "";
                    }

                    // Display Country List With User Selected Item Selected
                    userProfileInfoFromForm.CountryName = countryListItem.CountryName;
                    userProfileInfoFromForm.CountryCode = countryListItem.CountryCode;
                    userProfileInfoFromForm.CountryData = countryListItem.CountryCode;

                    // Create Sub Division List
                    var subDivisionList = new List<SubDivisonListItem>();
                    var subDivisionListItem = new SubDivisonListItem();

                    //foreach (var subDivision in subDivisions)
                    //{
                    //  subDivisionListItem = new SubDivisonListItem { SubDivisionName = subDivision.SUB_DIVISION_NAME, SubDivisionCode = subDivision.SUB_DIVISION_CODE };
                    //  subDivisionList.Add(subDivisionListItem);
                    //}

                    //Added by Ranjit Kumar-04-07-2011
                    foreach (var subDivision in sbDivisions)
                    {
                        subDivisionListItem = new SubDivisonListItem { SubDivisionName = subDivision.SUB_DIVISION_NAME, SubDivisionCode = subDivision.SUB_DIVISION_CODE };
                        subDivisionList.Add(subDivisionListItem);
                    }

                    // Get User Sub Division
                    //if (userSubDivision.Count() > 0)
                    //{
                    //    var subDivision = userSubDivision.First();
                    //    subDivisionListItem = new SubDivisonListItem { SubDivisionName = subDivision.SUB_DIVISION_NAME, SubDivisionCode = subDivision.SUB_DIVISION_CODE };
                    //}
                    if (subDivisionName.Count() > 0)
                    {
                        var subDivision = subDivisionName.First();
                        subDivisionListItem = new SubDivisonListItem { SubDivisionName = subDivision.SUB_DIVISION_NAME, SubDivisionCode = subDivision.SUB_DIVISION_CODE };
                    }

                    else
                    {
                        subDivisionListItem.SubDivisionCode = "0";
                        subDivisionListItem.SubDivisionName = "";
                    }

                    userProfileInfoFromForm.SubDivisionName = subDivisionListItem.SubDivisionName;
                    userProfileInfoFromForm.SubDivisionCode = subDivisionListItem.SubDivisionCode;
                    userProfileInfoFromForm.SubDivisionData = subDivisionListItem.SubDivisionCode;

                    // Create Member Location List
                    var locationCode = new List<LocationListItem>();

                    var customLocationListItem = new LocationListItem { LocationCode = "", LocationId = 0 };

                    // Create Custom List Item
                    locationCode.Add(customLocationListItem);

                    // Get Locations By MemberID
                    if (bIsMemberUser)
                    {
                        // Get All Member Locations
                        IEnumerable<MEM_LOCATION> allLocations = AuthManager.GetAllMemberLocationsByID(sisUser.Member.MemberID);

                        allLocations = from loc in allLocations
                                       where loc.MEMBER_ID == sisUser.Member.MemberID
                                       select loc;

                        // Create List Items
                        foreach (var location in allLocations)
                        {
                            var locationitem = new LocationListItem { LocationId = location.LOCATION_ID, LocationCode = location.LOCATION_CODE };
                            locationCode.Add(locationitem);
                        }

                        //CMP #665: Sec 2.7.1: Conditional Disabling of User Details 
                        //This change is applicable for creation of new users of User Category ‘Member’ ONLY. The New Member User could be a Super User or a Normal User.
                        //Disable User Profile Updates is true, will disable First Name, Last Name, Email Address. otherwise behavior will not change.
                        ViewData["DisableUserProfileUpdates"] =
                            _sisMemberSubStatusManager.GetSisMemberSubStatusDetails(
                                sisUser.Member.ISMembershipSubStatus).DisableUserProfileUpdates;
                    }

                    var selectValue = AppSettings.CustomLocationIdentifier;

                    // Get Users Member Location
                    if (sisUser.Location.LOCATION_ID > 0)
                    {
                        selectValue = sisUser.Location.LocationID;
                    }

                    // Display Member Locations
                    ViewData["LocationCodeList"] = locationCode;
                    ViewData["selectedLocation"] = selectValue;
                    
                }
                catch (Exception exception)
                {
                    Logger.Error("Error in PopulateModiftUserForm", exception);
                }
            }

            return userProfileInfoFromForm;
        }

        [Authorize]
        private static int CompareCountriesByCountryName(MST_COUNTRY country1, MST_COUNTRY country2)
        {
            return country1.COUNTRY_NAME.CompareTo(country2.COUNTRY_NAME);
        }

        [Authorize]
        [AcceptVerbs(new[] { "Post" })]
        public ActionResult GetSubDivisions(object param)
        {
            var reader = new StreamReader(Request.InputStream);

            var entityCode = reader.ReadToEnd();

            List<MST_SUB_DIVISION> subDivisionsDB;
            var subDivisionItems = new List<SubDivisonListItem>();
            var unitOfWork = new UnitOfWork(new ObjectContextAdapter());
            var countryRepository = new Repository<MST_COUNTRY>(unitOfWork);
            var subDivisonRepository = new Repository<MST_SUB_DIVISION>(unitOfWork);
            //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
            reader.Close();

            IEnumerable<MST_COUNTRY> countries = countryRepository.Get(cntry => cntry.COUNTRY_CODE.ToUpper() == entityCode.ToUpper());

            if (countries.Count() > 0)
            {
                var countryId = countries.First().COUNTRY_CODE;
                subDivisionsDB = subDivisonRepository.Get(sd => sd.COUNTRY_CODE == countryId);
            }
            else
            {
                subDivisionsDB = new List<MST_SUB_DIVISION>();
            }

            foreach (var sd in subDivisionsDB)
            {
                var sdItem = new SubDivisonListItem { SubDivisionName = sd.SUB_DIVISION_NAME, SubDivisionCode = sd.SUB_DIVISION_CODE };
                subDivisionItems.Add(sdItem);
            }

            var newJsonCityList = Json(subDivisionItems);

            return newJsonCityList;
        }

        // This is the default action for the View. Use it to setup your grid Model.
        [Authorize]
        public ActionResult UserSearchControl()
        {
            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account");
            }

            var gridModel = new SearchBarModel();
            var ordersGrid = gridModel.OrdersGrid;

            ordersGrid.DataUrl = Url.Action("SearchGridDataRequested");
            SetUpGrid(ordersGrid);

            return View(gridModel);
        }

        // This method is called when the grid requests data
        [Authorize]
        public JsonResult SearchGridDataRequested()
        {
            // customize the default Orders grid model with our custom settings
            var gridModel = new SearchBarModel();
            SetUpGrid(gridModel.OrdersGrid);

            //var listOfUserData = (List<TableRowForHelper>)HttpContext.Cache[Session.SessionID + "Results"];
            var listOfUserData = (List<TableRowForHelper>)Session[Session.SessionID + "Results"];

            if (listOfUserData != null)
                return gridModel.OrdersGrid.DataBind(listOfUserData.AsQueryable());

            return null;
        }

        [Authorize]
        private void SetUpGrid(JQGrid ordersGrid)
        {
            ordersGrid.ID = "userSearchGrid";

            // Bind Data To Grid
            ordersGrid.DataUrl = Url.Action("SearchGridDataRequested");

            // show the search toolbar
            ordersGrid.ToolBarSettings.ShowSearchToolBar = false;
            ordersGrid.PagerSettings.PageSizeOptions = SystemParameters.Instance.UIParameters.PageSizeOptions;
            ordersGrid.PagerSettings.PageSize = SystemParameters.Instance.UIParameters.DefaultPageSize;

            ordersGrid.Columns.Find(c => c.DataField == "Actions").Formatter = new CustomFormatter { FormatFunction = "formatlink", UnFormatFunction = "unformatlink" };
        }

        [Authorize]
        private List<TableRowForHelper> GetUserSearchResults(SearchBarModel gridModal)
        {
            var listOfUserData = new List<TableRowForHelper>();
            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
            {
                var currentUser = AuthManager.GetUserByUserID(int.Parse(SessionUtil.UserId.ToString()));

                var noSearchCriteria = ((String.IsNullOrEmpty(gridModal.FirstName)) && (String.IsNullOrEmpty(gridModal.LastName)) && (String.IsNullOrEmpty(gridModal.Email)) && (gridModal.StatusId == 0));


                //Get Users Based On Search Criteria
                var users = new List<IS_USER>().AsQueryable();

                var unitOfWork = new UnitOfWork(new ObjectContextAdapter());
                var userRepository = new Repository<IS_USER>(unitOfWork);

                var userCateGoryId = (int)SessionUtil.UserCategory;
                if (noSearchCriteria)
                {
                    gridModal.StatusId = 1;
                    gridModal.UserCategoryId = userCateGoryId;
                }
                switch (userCateGoryId)
                {
                   //##################################################################################
                     // CMP685- USER MANAGEMENT
                     // IS_ACTIVE flag will be used in place of IS_LOCK for Activating/deactivating users
                     // below all the places where IS_LOCK has been used for account deactivate is replaced by IS_ACTIVE flag
                  //##################################################################################

                    case (int)UserCategory.SisOps:
                        switch (gridModal.StatusId)
                        {
                            case 1:
                                users = userRepository.Get(user => user.IS_ACTIVE == true).AsQueryable();
                                break;

                            case 2:

                                users = userRepository.Get(user => user.IS_ACTIVE == false).AsQueryable();
                                break;

                            default:
                                users = userRepository.GetAll().AsQueryable();
                                break;
                        }

                        if (gridModal.MemberId > 0)
                        {
                            users = users.Where(u => u.MEMBER_ID == gridModal.MemberId);
                        }

                        users = gridModal.UserCategoryId > 0 ? users.Where(u => u.USER_CATEGORY_ID == gridModal.UserCategoryId) : users.Where(u => u.USER_CATEGORY_ID > 0);
                        if (!string.IsNullOrEmpty(gridModal.FirstName))
                        {
                            users = users.Where(user => user.FIRST_NAME.ToUpper().Contains(gridModal.FirstName.ToUpper()));
                        }
                        if (!string.IsNullOrEmpty(gridModal.LastName))
                        {
                            users = users.Where(user => !string.IsNullOrEmpty(user.LAST_NAME) && user.LAST_NAME.ToUpper().Contains(gridModal.LastName.ToUpper()));
                        }
                        if (!string.IsNullOrEmpty(gridModal.Email))
                        {
                            users = users.Where(user => user.EMAIL_ID.ToUpper().Contains(gridModal.Email.ToUpper()));
                        }
                        break;

                    case (int)UserCategory.IchOps:
                    case (int)UserCategory.AchOps:

                        switch (gridModal.StatusId)
                        {
                            case 1:
                                users =
                                  userRepository.Get(
                                    user =>
                                     user.IS_ACTIVE == true).AsQueryable();
                                break;

                            case 2:
                                users =
                                  userRepository.Get(
                                    user =>
                                     user.IS_ACTIVE == false).AsQueryable();
                                break;

                            default:
                                users =
                                  userRepository.GetAll().AsQueryable();
                                break;
                        }

                        users = users.Where(u => u.USER_CATEGORY_ID == userCateGoryId);
                        if (!string.IsNullOrEmpty(gridModal.FirstName))
                        {
                            users = users.Where(user => user.FIRST_NAME.ToUpper().Contains(gridModal.FirstName.ToUpper()));
                        }
                        if (!string.IsNullOrEmpty(gridModal.LastName))
                        {
                            users = users.Where(user => user.LAST_NAME.ToUpper().Contains(gridModal.LastName.ToUpper()));
                        }
                        if (!string.IsNullOrEmpty(gridModal.Email))
                        {
                            users = users.Where(user => user.EMAIL_ID.ToUpper().Contains(gridModal.Email.ToUpper()));
                        }
                        break;
                    case (int)UserCategory.Member:

                        switch (gridModal.StatusId)
                        {
                            case 1:
                                users =
                                  userRepository.Get(
                                    user =>
                                     user.IS_ACTIVE == true &&
                                     user.MEMBER_ID == currentUser.Member.MemberID).AsQueryable();
                                break;
                            case 2:
                                users =
                                  userRepository.Get(
                                    user =>
                                     user.IS_ACTIVE == false &&
                                     user.MEMBER_ID == currentUser.Member.MemberID).AsQueryable();
                                break;

                            default:
                                users =
                                  userRepository.Get(
                                    user =>
                                     user.MEMBER_ID == currentUser.Member.MemberID).AsQueryable();
                                break;
                        }
                        if (!string.IsNullOrEmpty(gridModal.FirstName))
                        {
                            users = users.Where(user => user.FIRST_NAME.ToUpper().Contains(gridModal.FirstName.ToUpper()));
                        }
                        if (!string.IsNullOrEmpty(gridModal.LastName))
                        {
                            users = users.Where(user => !string.IsNullOrEmpty(user.LAST_NAME) && user.LAST_NAME.ToUpper().Contains(gridModal.LastName.ToUpper()));
                        }
                        if (!string.IsNullOrEmpty(gridModal.Email))
                        {
                            users = users.Where(user => user.EMAIL_ID.ToUpper().Contains(gridModal.Email.ToUpper()));
                        }
                        break;

                    default:
                        users = userRepository.Get(
                                   user =>
                                     user.MEMBER_ID == currentUser.Member.MemberID).AsQueryable();
                        break;
                }

                //Filter Return Result And Remove Current User's Record If It Exist
                var queriedListOfUsers = from user in users
                                         where ((user.IS_USER_ID > ((decimal)0)) && (user.IS_USER_ID != currentUser.UserID))
                                         select user;

                //Loop Through Users And Prepare Grid Data
                var rowNum = 0;
                foreach (var user in queriedListOfUsers)
                {
                    var userType = "No";

                    if (user.USER_TYPE)
                    {
                        userType = "Yes";
                    }

                    // CMP#668: Archival of IS-WEB Users and Removal from Screens
                    var isArchived = "No";
                    if(user.IS_ARCHIVED)
                    {
                        isArchived = "Yes";
                    }

                    // SCP98867: User Validity
                    // In case of email id contains apostrophe (') javascript is unable to parse complete email Id.
                    // As space is not allowed in email id and hence (') is replaced with space.
                    var userEmailId = user.EMAIL_ID.Replace("'"," ");

                    // CMP#668: Archival of IS-WEB Users and Removal from Screens
                    var currentRow = new TableRowForHelper { ID = (rowNum + 1).ToString(), FirstName = user.FIRST_NAME, LastName = user.LAST_NAME, EmailAddress = user.EMAIL_ID, SuperUser = userType, IsArchived = isArchived };
                    var actionLinks = new StringBuilder();

                    actionLinks.Append(
                      String.Format(
                        "<a href=\"javascript:PostData('{0}', 'ShouldEdit');\"><img src=\"{1}\" alt=\"" + AppSettings.EditUserImageAltText + "\" title=\"" + AppSettings.EditUserAccountImageToolTip +
                        "\" style='border:0px;' /></a>&nbsp;&nbsp;&nbsp;",
                        userEmailId,
                        Url.Content(AppSettings.EditUserAccountImagePath)));
                    actionLinks.Append(
                      String.Format(
                        "<a href=\"javascript:PostData('{0}', 'ResetPassword');\"><img onclick=\" javascript: return confirm('Are you sure you want to reset password?');\"  src=\"{1}\" alt=\"" +
                        AppSettings.ResetPasswordImageAltText + "\" title=\"" + AppSettings.ResetPasswordImageToolTip + "\" style='border:0px;'/></a>&nbsp;&nbsp;&nbsp;",
                        userEmailId,
                        Url.Content(AppSettings.ResetPasswordImagePath)));

                    if (user.IS_ACTIVE == false)
                    {
                        actionLinks.Append(
                          String.Format(
                            "<a href=\"javascript:PostData('{0}', 'ChangeActiveStatus');\" ><img onclick=\" javascript: return confirm('Are you sure you want to activate account?');\"  src=\"{1}\" alt=\"" +
                            AppSettings.ActivateUserAccountImageAltText + "\" title=\"" + AppSettings.ActivateUserAccountImageToolTip + "\" style='border:0px;'/></a> &nbsp;&nbsp;&nbsp;",
                            userEmailId,
                            Url.Content(AppSettings.ActivateUserAccountImagePath)));
                    }
                    else
                    {
                        actionLinks.Append(
                          String.Format(
                            "<a href=\"javascript:PostData('{0}', 'ChangeActiveStatus');\"><img onclick=\" javascript: return confirm('Are you sure you want to deactivate account?');\" src=\"{1}\" alt=\"" +
                            AppSettings.DeactivateUserAccountImageAltText + "\" title=\"" + AppSettings.DeactivateUserAccountImageToolTip + "\"  style='border:0px;'/></a>&nbsp;&nbsp;&nbsp;",
                            userEmailId,
                            Url.Content(AppSettings.DeactivateUserAccountImagePath)));
                    }
                    if (SessionUtil.UserCategory == UserCategory.SisOps && user.MEMBER_ID > 0)
                    {
                        // CMP #655: IS-WEB Display per Location ID
                        // Section : 2.1.4	NEW POPUP FOR SIS OPS USERS TO MANAGE MEMBER LOCATION ASSOCIATIONS
                        actionLinks.Append(
                          string.Format(
                                "<a style=cursor:hand target=_parent href=\"javascript:LocationAssociation('{0}','{1}','{2}','{3}');\"><img title='Manage Location Association' alt='Manage Location Association' style=border-style:none src={4} /></a>",                                    
                                    user.IS_USER_ID,
                                    user.MEMBER_ID,
                                    userEmailId,
                                    Url.Action("GetLocationListForSisops", "LocationAssociation", new { area = "Profile" }),
                                    Url.Content(AppSettings.LocationAssociationIconPath)));

                    }


                    currentRow.Actions = actionLinks.ToString();
                    listOfUserData.Add(currentRow);
                    rowNum += 1;
                }
                //}
            }

            // Return Grid Data
            return listOfUserData;
        }

        [Authorize]
        [ISAuthorize(Profile.CreateOrManageUsersAccess)]
        public ActionResult SearchOrModify()
        {
            var MemberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
            var httpEncoder = new HttpEncoderEx();
            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account");
            }

            string firstName;
            string lastName;
            string emailAddress;
            string statusId;
            string memberId;
            string userCategoryId;
            string memberName;

            ViewData["Status"] = "3"; //Set Default Selected Status;
            
            //SIS_SCR_REPORT_23_jun-2016_2: Cross Site Scripting
            var viewMode = httpEncoder.GetHtmlEncodeValue(Request.Form["ViewMode"]);
            var shouldChangeActiveStatus = httpEncoder.GetHtmlEncodeValue(Request.Form["ChangeActiveStatus"]);
            var resetPassword = httpEncoder.GetHtmlEncodeValue(Request.Form["ResetPassword"]);
            var shouldEdit = httpEncoder.GetHtmlEncodeValue(Request.Form["ShouldEdit"]);
            var shouldProxy = httpEncoder.GetHtmlEncodeValue(Request.Form["ShouldProxy"]);

            // SCP98867: User Validity
            // replaceing space with apostrophe (') which was replaced in javascript call.
            shouldChangeActiveStatus = shouldChangeActiveStatus == null ? shouldChangeActiveStatus : shouldChangeActiveStatus.Replace(" ", "'");
            resetPassword = resetPassword == null ? resetPassword : resetPassword.Replace(" ", "'");
            shouldEdit = shouldEdit == null ? shouldEdit : shouldEdit.Replace(" ", "'");
            shouldProxy = shouldProxy == null ? shouldProxy : shouldProxy.Replace(" ", "'");
            
            //SIS_SCR_REPORT_23_jun-2016_2: Cross Site Scripting
            ViewData["FirstName"] = firstName = httpEncoder.GetHtmlEncodeValue(Request.Form["FirstName"]);
            ViewData["LastName"] = lastName = httpEncoder.GetHtmlEncodeValue(Request.Form["LastName"]);
            ViewData["Email"] = emailAddress = httpEncoder.GetHtmlEncodeValue(Request.Form["Email"]);
            var memberEmailAddress = httpEncoder.GetHtmlEncodeValue(Request.Form["MemberEmail"]);
            ViewData["Status"] = statusId = httpEncoder.GetHtmlEncodeValue(Request.Form["StatusId"]);
            ViewData["MemberId"] = memberId = httpEncoder.GetHtmlEncodeValue(Request.Form["MemberId"]);
            ViewData["UserCategoryId"] = userCategoryId = httpEncoder.GetHtmlEncodeValue(Request.Form["UserCategoryId"]);
            ViewData["MemberName"] = memberName = httpEncoder.GetHtmlEncodeValue(Request.Form["MemberName"]);

            if(!string.IsNullOrEmpty(memberId) && memberId != "0")
            {
             var member =  MemberManager.GetMember(int.Parse(memberId));
              ViewData["MemberName"] =
                memberName =
                string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName);
            }
        
            var isChangeActivateAction = (string)(TempData["IsActivateAction"] ?? "false");
            if (ViewData.ContainsKey("HideGrid"))
            {
                ViewData.Remove("HideGrid");
            }

            if (string.IsNullOrEmpty(viewMode))
            {
                ViewData["ViewMode"] = 1;
                ViewData["HideGrid"] = true;
            }

            // Get activate and deactivate action flag to show up message in same screen  
            if (isChangeActivateAction == "true")
            {
                var searchConditionArray = SessionUtil.ManageUserSearchCriteria;
                firstName = searchConditionArray[0];
                lastName = searchConditionArray[1];
                emailAddress = searchConditionArray[2];
                statusId = searchConditionArray[3];
                memberName = searchConditionArray[4];
                memberId = searchConditionArray[5];
                userCategoryId = searchConditionArray[6];
                ViewData["UserCategoryId"] = userCategoryId;
                TempData["IsActivateAction"] = "false";

                var isActivationFalied = (string)(TempData["ActivationFailedError"] ?? "false");

                if (isActivationFalied == "true")
                {
                    ViewData[ViewDataConstants.ErrorMessage] = (TempData["HeaderMessage"] ?? "Account could not updated. An error occured while updating your account.");
                }
                else
                {
                    var headerMessage = (string)(TempData["HeaderMessage"] ?? "User information saved successfully");
                    ViewData[ViewDataConstants.SuccessMessage] = headerMessage;
                }
            }

            // Get Action Mode
            if (String.IsNullOrWhiteSpace(shouldChangeActiveStatus))
            {
                shouldChangeActiveStatus = String.Empty;
            }
            if (String.IsNullOrWhiteSpace(resetPassword))
            {
                resetPassword = String.Empty;
            }
            if (String.IsNullOrEmpty(shouldEdit))
            {
                shouldEdit = String.Empty;
            }
            if (String.IsNullOrEmpty(shouldProxy))
            {
                shouldProxy = String.Empty;
            }

            // Handle Action If Action Clicked
            if (resetPassword.Contains("@"))
            {
                return ModifyLockedOrPassword(resetPassword, null, statusId);
            }
            if (shouldChangeActiveStatus.Contains("@"))
            {
                return ModifyLockedOrPassword(null, shouldChangeActiveStatus, statusId);
            }

            if (shouldEdit.Contains("@"))
            {
                TempData["ShouldEdit"] = shouldEdit;
                // CMP-520: Management of Super Users
                // To fix bug 8850 we are passing emailOfPersonToEdit as 
                // it was getting null on page getting refreshed.
                return RedirectToAction("UserEditFromMember", "Account", new { emailID = shouldEdit });
            }

            if (shouldProxy.Contains("@"))
            {
                if (UserManagementModel.SignInProxy(SessionUtil.UserId.ToString(), shouldProxy))
                {
                    var proxyLoginUserdetails = AuthManager.GetProxyUserByAdminUserID(SessionUtil.UserId);
                    if (proxyLoginUserdetails != null)
                    {
                        SessionUtil.UserId = proxyLoginUserdetails.UserID;
                        SessionUtil.UserLanguageCode = proxyLoginUserdetails.LanguageCode;
                        SessionUtil.Username = string.Format("{0} {1}", proxyLoginUserdetails.FirstName, proxyLoginUserdetails.LastName);
                        SessionUtil.UserCategory = (UserCategory)proxyLoginUserdetails.CategoryID;
                        SessionUtil.MemberName = proxyLoginUserdetails.Member.MemberCommercialName;
                        SessionUtil.MemberId = proxyLoginUserdetails.Member.MemberID;
                        SessionUtil.IsLogOutProxyOption = true;

                        return RedirectToAction("ProxyLoginSuccess", "Account");
                    }
                }
            }

            // No Action Clicked, Get Criteria
            if (String.IsNullOrEmpty(firstName))
            {
                firstName = String.Empty;
            }
            if (String.IsNullOrEmpty(lastName))
            {
                lastName = String.Empty;
            }
            if (String.IsNullOrEmpty(emailAddress))
            {
                emailAddress = String.Empty;
            }
            if (String.IsNullOrEmpty(statusId))
            {
                statusId = "0";
            }

            // Create Grid
            var gridModel = new SearchBarModel();
            var ordersGrid = gridModel.OrdersGrid;

            gridModel.FirstName = firstName;
            gridModel.LastName = lastName;
            gridModel.Email = emailAddress;
            gridModel.StatusId = Convert.ToInt32(statusId);
            gridModel.ViewMode = viewMode;
            gridModel.MemberName = memberName;
            gridModel.MemberId = !string.IsNullOrEmpty(memberId) ? Convert.ToInt32(memberId) : 0;
            gridModel.UserCategoryId = !string.IsNullOrEmpty(userCategoryId) ? Convert.ToInt32(userCategoryId) : 0;

            // Maintain Search condition values 
            var searchArray = new string[7];
            searchArray[0] = firstName;
            searchArray[1] = lastName;
            searchArray[2] = emailAddress;
            searchArray[3] = statusId;
            searchArray[4] = memberName;
            searchArray[5] = memberId;
            searchArray[6] = userCategoryId;
            SessionUtil.ManageUserSearchCriteria = searchArray;

            var errormsg = string.Empty;
            // Validate Search Criteria
            if (!ValidateSearchValues(firstName, lastName, emailAddress, statusId, shouldEdit, shouldProxy, memberEmailAddress,out errormsg))
            {// ID : 271100 - Manage Users - Screen
                ModelState.AddModelError("InvalidEntryValidationFailed", "");
                ShowErrorMessage(errormsg, true);
                ViewData["HideGrid"] = true;
                
            }

            // Get Search Results
             var listOfUserData = GetUserSearchResults(gridModel);
             if (userCategoryId == "4" && (string.IsNullOrEmpty(memberId) || memberId == "0") && string.IsNullOrEmpty(emailAddress))
             {
               listOfUserData = new List<TableRowForHelper>();
             }

           if ((string.IsNullOrEmpty(viewMode) == false) && (listOfUserData.Count() <= 0))
            {// ID : 271100 - Manage Users - Screen
                ModelState.AddModelError("NoRecordsFound", "");
                ShowErrorMessage("No Records Found", true);
                ViewData["HideGrid"] = true;
                return View(gridModel);
            }

            //HttpContext.Cache.Remove(Session.SessionID + "Results");
            if (Session[Session.SessionID + "Results"] != null)
            {
                Session.Remove(Session.SessionID + "Results");
            }

            //Cache search results for binding to Grid
            // HttpContext.Cache[Session.SessionID + "Results"] = listOfUserData;
            Session[Session.SessionID + "Results"] = listOfUserData;

            // CMP #655: IS-WEB Display per Location ID
            // Section : 2.1.4	NEW POPUP FOR SIS OPS USERS TO MANAGE MEMBER LOCATION ASSOCIATIONS
            // Desc : Return the schema of Associated and UnAssociated ViewData to View
            var unAssociatedLocationsLocation = new List<UserContactLocations>();
            var listBoxAssociatedLoc = new List<UserContactLocations>();
            ViewData["UnAssociatedLocation"] = new MultiSelectList(unAssociatedLocationsLocation.ToArray(), "LocationId", "LocationName");
            ViewData["AssociatedLocation"] = new MultiSelectList(listBoxAssociatedLoc.ToArray(), "LocationId", "LocationName");

            //Configure Grid Display
            SetUpGrid(ordersGrid);

            //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
            resetPassword = string.Empty;

            return View(gridModel);
        }

        [Authorize]
        public bool ValidateModifyLockedOrPassword(string resetPassword, string lockAccount)
        {
            // SCP98867: User Validity
            // Using the same regular expression to validate the email id, which is used to validate email id at the time of user creation.
            var validEmailPattern = new Regex(FormatConstants.ValidEmailPattern);
          
            if (resetPassword == "" && lockAccount == "")
            {
                ModelState.AddModelError("EmailAddressNotPassedIn", "Email Required");
            }
            if (String.IsNullOrWhiteSpace(resetPassword) == false)
            {
              if (!validEmailPattern.IsMatch(resetPassword.ToLower()))
                {
                    ModelState.AddModelError("EmailAddressRegularExpression", string.Format("{0} is invalid", resetPassword));
                }
            }

            if (String.IsNullOrWhiteSpace(lockAccount) == false)
            {
              if (!validEmailPattern.IsMatch(lockAccount.ToLower()))
                {
                    ModelState.AddModelError("EmailAddressRegularExpression", string.Format("{0} is invalid", lockAccount));
                }
            }

            //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
            resetPassword = string.Empty;

            return ModelState.IsValid;
        }

        [Authorize]
        public ActionResult ModifyLockedOrPassword(string resetPassword, string lockAccount, string status)
        {
            if (String.IsNullOrWhiteSpace(resetPassword))
            {
                resetPassword = string.Empty;
            }

            if (String.IsNullOrWhiteSpace(lockAccount))
            {
                lockAccount = string.Empty;
            }

            if (ValidateModifyLockedOrPassword(resetPassword, lockAccount) == false)
            {
                return RedirectToAction("UserModifiedFailed", "Account");
            }

            if (resetPassword.Contains("@"))
            {
                var userToResetPasswordFor = AuthManager.GetUserByUserName(resetPassword);
                //CMP685- USER MANAGMENT
                // generate and send the Dynamic URL link for password reset here
                string carrierFlag = "R"; string urlPath = Path.Combine(SystemParameters.Instance.General.LogOnURL, AppSettings.UrlPasswordResetLink).Replace("\\", "/");
                if (UserManagementManager.ResetUserPassword(userToResetPasswordFor.UserID, urlPath, ref carrierFlag))
                {
                    TempData["IsActivateAction"] = "true";
                    TempData["StatusId"] = status;
                    TempData["ActivationFailedError"] = "false";
                    TempData["HeaderMessage"] = AppSettings.ResetPasswordSuccessMessage;

                    return RedirectToAction("SearchOrModify", "Account");
                }

                TempData["IsActivateAction"] = "true";
                TempData["ActivationFailedError"] = "true";
                TempData["HeaderMessage"] = "An error occured while user password reset.";

                return RedirectToAction("SearchOrModify", "Account");
            }

            if (lockAccount.Contains("@"))
            {
                var userToChangeActiveStatusFor = AuthManager.GetUserByUserName(lockAccount);
                var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                if (!userToChangeActiveStatusFor.IsActive)
                {
                    if (UserManagementManager.Unlock(userToChangeActiveStatusFor.UserID))
                    {
                        TempData["IsActivateAction"] = "true";
                        TempData["StatusId"] = status;
                        TempData["ActivationFailedError"] = "false";
                        TempData["HeaderMessage"] = "User has been activated";

                        //CMP #665: User Related Enhancements-FRS-v1.2 [sec 2.4.3	Web Service Call to ICP].
                        //if federation is exist then en-queue message in log icp consumer queue.
                        if (!String.IsNullOrWhiteSpace(userToChangeActiveStatusFor.FederationId))
                        {
                            //Request Type "null", ActionType 1: Create User, ActionType 2: Enable User, ActionType 3: Disable User  
                            new ReferenceManager().EnqueueMessageInIcpLogConsumer(userToChangeActiveStatusFor.Email, null, 2);
                        }
                        else
                        {
                            //Create object of IIcpEmailSenderManager
                            var icpEmailSenderManager = Ioc.Resolve<IIcpEmailSenderManager>(typeof (IIcpEmailSenderManager));
                            string memberCode = userToChangeActiveStatusFor.Member.MemberID > 0
                                                    ? userToChangeActiveStatusFor.Member.MemberCode.Insert(2, "-")
                                                    : string.Empty;

                            //Send email to sis support and sis ops due to federation is not available of user.
                            icpEmailSenderManager.SendIcpFailedEmailNotification(userToChangeActiveStatusFor.FirstName,
                                                                                 userToChangeActiveStatusFor.LastName,
                                                                                 userToChangeActiveStatusFor.Email,
                                                                                 GetUserCategoryName(userToChangeActiveStatusFor.CategoryID),
                                                                                 userToChangeActiveStatusFor.Member.MemberID,
                                                                                 memberCode, EmailTemplateId.IcpEnableUserNoFederationIdNotification);
                            //Release object of IIcpEmailSenderManager
                            Ioc.Release(icpEmailSenderManager);
                        }

                        //SCP#409719 - ICH Contacts
                        //Desc: Hooked a call to MemberManager.InsertMessageInOracleQueue() to quque member for ICH Profile Update XML Generation.
                        var contactDetails = memberManager.GetContactDetailsByEmailAndMember(userToChangeActiveStatusFor.Email, Convert.ToInt32(userToChangeActiveStatusFor.Member.MemberID));
                        if (contactDetails != null)
                        {
                            memberManager.InsertMessageInOracleQueue("MemberProfileUpdate", Convert.ToInt32(userToChangeActiveStatusFor.Member.MemberID));
                        }
                        return RedirectToAction("SearchOrModify", "Account");
                    }

                    TempData["IsActivateAction"] = "true";
                    TempData["ActivationFailedError"] = "true";
                    TempData["HeaderMessage"] = "An error occured while user activation.";

                    return RedirectToAction("SearchOrModify", "Account");
                }

                //// Check if the user is the only contact in Contact Types assigned to it.
                var contact = memberManager.GetContactDetailsByEmailAndMember(userToChangeActiveStatusFor.Email, Convert.ToInt32(userToChangeActiveStatusFor.Member.MemberID));

                //// If user is the only contact assigned then show error message.
                //SCPIDID : 112924 - Only contact assigned to specific Contact Type marked as Inactive - no error [TC:073575]
                if ((contact != null) && (memberManager.IsOnlyContactAssigned(contact.Id,0,0)))
                {
                  TempData["ActivationFailedError"] = "true";
                  TempData["IsActivateAction"] = "true";
                  TempData["HeaderMessage"] = "Unable to deactivate the user as the user is the only contact in one of the contact type assigned to it.";

                  return RedirectToAction("SearchOrModify", "Account");
                }

                if (AuthManager.LockUser(userToChangeActiveStatusFor.UserID))
                {
                    // If user is not the only contact assigned then remove all the 
                    // contact assignments and deactivate the contact of the user.
                    if (contact != null)
                    {
                        memberManager.RemoveAllContactAssignments(contact.Id);
                        contact.IsActive = false;
                        memberManager.UpdateContact(contact.MemberId, contact);
                    }

                    //CMP #665: User Related Enhancements-FRS-v1.2 [sec 2.4.3	Web Service Call to ICP].
                    //if federation is exist then en-queue message in log icp consumer queue.
                    if (!String.IsNullOrWhiteSpace(userToChangeActiveStatusFor.FederationId))
                    {
                        //Request Type "null", ActionType 1: Create User, ActionType 2: Enable User, ActionType 3: Disable User  
                        new ReferenceManager().EnqueueMessageInIcpLogConsumer(userToChangeActiveStatusFor.Email, null, 3);
                    }
                    else
                    {
                        //Create object of IIcpEmailSenderManager
                        var icpEmailSenderManager =
                            Ioc.Resolve<IIcpEmailSenderManager>(typeof(IIcpEmailSenderManager));
                        string memberCode = userToChangeActiveStatusFor.Member.MemberID > 0
                                                ? userToChangeActiveStatusFor.Member.MemberCode.Insert(2, "-")
                                                : string.Empty;

                        //Send email to sis support and sis ops due to federation is not available of user.
                        icpEmailSenderManager.SendIcpFailedEmailNotification(userToChangeActiveStatusFor.FirstName,
                                                                             userToChangeActiveStatusFor.LastName,
                                                                             userToChangeActiveStatusFor.Email,
                                                                             GetUserCategoryName(userToChangeActiveStatusFor.CategoryID),
                                                                             userToChangeActiveStatusFor.Member.MemberID,
                                                                             memberCode, EmailTemplateId.IcpDisableUserNoFederationIdNotification);
                        //Release object of IIcpEmailSenderManager
                        Ioc.Release(icpEmailSenderManager);
                    }


                    TempData["IsActivateAction"] = "true";
                    TempData["ActivationFailedError"] = "false";
                    TempData["HeaderMessage"] = "User has been Deactivated";
                    TempData["StatusId"] = status;

                    return RedirectToAction("SearchOrModify", "Account");
                }

                TempData["IsActivateAction"] = "true";
                TempData["HeaderMessage"] = "An error occured while user Deactivation.";

                return RedirectToAction("SearchOrModify", "Account");
            }

            return RedirectToAction("UserModifiedFailed", "Account");
        }

        [Authorize]
        [ISAuthorize(Profile.CreateOrManageUsersAccess)]
        public ActionResult UserEditFromMember(string emailId)
        {
            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()))
            {
                return RedirectToAction("LogOn", "Account");
            }

            if(!string.IsNullOrEmpty(emailId))
            {
              TempData["ShouldEdit"] = emailId;
            }

          ViewData["PasswordLength"] = AuthManager.MinPasswordLength;
            ViewData["Role1"] = "Administrator";
            var emailOfPersonToEdit = emailId;
            ViewData["UserModify"] = true;

            //CMP-520: Management of Super Users
            /*Only if emailOfPersonToEdit is not null, we will set ViewData true 
              to make SuperUser Chkbox Visible on page*/
            if (!string.IsNullOrEmpty(emailOfPersonToEdit))
            {
                ViewData["MakeCheckBoxVisible"] = true;
            }

          // SCP121760: SIS Ops User Edit User Screen 
          TempData["loggedInUserCategory"] = false;
          TempData["loggedInUserType"] = false;

          // Get the user details from database by providing user id from session.
          var iSUser = AuthManager.GetUserByUserID(SessionUtil.UserId);
          
          if (iSUser != null)
          {
            // set TempData valu depending on user category.
            if (iSUser.CategoryID == 1)
            {
              TempData["loggedInUserCategory"] = true;
            }
            // set TempData valu depending on user type (Super/Normal user).
            if (iSUser.UserType == 1)
            {
              TempData["loggedInUserType"] = true;
            }
          }
          else
          {
            return RedirectToAction("LogOn", "Account");
          }

          SessionUtil.MemberName = string.Empty;

            PopulateRegisterForm();
            if (Equals(TempData["ShowSavedMessage"], "true"))
            {
                TempData["ActivationFailedError"] = "false";

                //CMP #655: IS-WEB Display per Location ID
                //2.2	NEW USER CREATION USING IS-WEB
                if (Equals(TempData["ShouldShowContAssoMessage"], "true"))
                {
                    ViewData[ViewDataConstants.SuccessMessage] = "User information saved successfully. This user has been associated with all Location IDs of his/her organization. Please review associations and modify if required using screen Manage Location Associations.";
                }
                else
                {
                    ViewData[ViewDataConstants.SuccessMessage] = "User information saved successfully";
                }
                
                TempData["ShowSavedMessage"] = "false";
            }
            
            // CMP#668: Archival of IS-WEB Users and Removal from Screens
            TempData["isUserCreation"] = "False";

            return View("UserEditFromMember", PopulateModifyUserForm(emailOfPersonToEdit));
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ISAuthorize(Profile.CreateOrManageUsersAccess)]
        [ValidateAntiForgeryToken]
        public ActionResult UserEditFromMember(CreateUserView viewInfo)
        {
            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
            {
                ViewData["PasswordLength"] = 6;
                if (String.IsNullOrWhiteSpace(viewInfo.HiddenEmailAddress))
                {
                    viewInfo.HiddenEmailAddress = (string)ViewData["ShouldEdit"];
                }

                if (!ValidateRegistration(viewInfo, false))
                {
                    PopulateRegisterForm();
                    return View("UserEditFromMember", PopulateModifyUserForm(viewInfo.HiddenEmailAddress));
                }
                var sisUser = AuthManager.GetUserByUserName(viewInfo.HiddenEmailAddress);
                if (viewInfo.LastName == null)
                    viewInfo.LastName = "";
                if (viewInfo.PositionTitle == null)
                    viewInfo.PositionTitle = "";
                if (viewInfo.StaffID == null)
                    viewInfo.StaffID = "";
                if (viewInfo.Department == null)
                    viewInfo.Department = "";
                if (viewInfo.Divison == null)
                    viewInfo.Divison = "";
                if (viewInfo.SITAAddress == null)
                    viewInfo.SITAAddress = "";

                sisUser.Salutation = viewInfo.Salutation;
                sisUser.FirstName = viewInfo.FirstName;
                sisUser.LastName = viewInfo.LastName.Trim(); //SCP444865 - Two-word last name
                sisUser.Email = viewInfo.EmailAddress;
                sisUser.PositionOrTitle = viewInfo.PositionTitle;
                sisUser.StaffID = viewInfo.StaffID;
                sisUser.Department = viewInfo.Department;
                sisUser.Division = viewInfo.Divison;

                sisUser.Phone.PhoneNumber1 = viewInfo.Telephone1;
                sisUser.Phone.PhoneNumber2 = viewInfo.Telephone2;
                sisUser.Phone.Fax = viewInfo.Fax;
                sisUser.Phone.MobileNumber = viewInfo.Mobile;
                sisUser.LanguageCode = viewInfo.UserLanguageCode;

                // CMP#668: Archival of IS-WEB Users and Removal from Screens
                sisUser.IsArchived = viewInfo.IsArchived;

                if (string.IsNullOrEmpty(viewInfo.LocationID) || viewInfo.LocationID == "0")
                {
                    sisUser.Location.Address1 = viewInfo.Address1;
                    sisUser.Location.Address2 = viewInfo.Address2;
                    sisUser.Location.Address3 = viewInfo.Address3;
                    sisUser.Location.POSTAL_CODE = viewInfo.PostalCode;

                    if (string.IsNullOrEmpty(viewInfo.SubDivisionName) == false)
                    {
                        sisUser.Location.SUB_DIVISION_CODE = viewInfo.SubDivisionCode;
                    }
                    else
                    {
                        sisUser.Location.SUB_DIVISION_CODE = null;
                    }

                    sisUser.Location.CITY_NAME = viewInfo.CityName;
                    if (!string.IsNullOrEmpty(viewInfo.CountryName))
                    {
                        sisUser.Location.COUNTRY_CODE = viewInfo.CountryCode;
                    }
                    else
                    {
                        sisUser.Location.COUNTRY_CODE = null;
                    }
                    sisUser.Location.LocationID = string.Empty;
                }
                else
                {
                    sisUser.Location.LocationID = viewInfo.LocationID;
                }

                sisUser.Location.SITA_Address = viewInfo.SITAAddress;
                sisUser.ModifiedBy = SessionUtil.AdminUserId;

                if (Convert.ToBoolean(sisUser.UserType) != viewInfo.UserType)
                {
                  if(MemberManager.ChangeUserPermission(sisUser.UserID, Convert.ToInt32(viewInfo.UserType)) == false)
                  {
                    return RedirectToAction("UserModifiedFailed", "Account");
                  }
                }

                var statusCode = 0;

                //SCP333083: XML Validation Failure for A30-XB - SIS Production
                if (ContactRepository.Single(con => con.EmailAddress.ToLower() == viewInfo.HiddenEmailAddress.ToLower() && con.MemberId == sisUser.Member.MemberID) != null && viewInfo.HiddenEmailAddress.ToLower() != sisUser.Email.ToLower() && ContactRepository.GetCount(contacts => contacts.EmailAddress.ToLower() == sisUser.Email.ToLower() && contacts.MemberId == sisUser.Member.MemberID) > 0)
                {
                  ModelState.AddModelError("FirstNameRequired", string.Format("{0} {1}", sisUser.Email, AppSettings.DuplicateMemberContact));
                  return View("UserModify", PopulateModifyUserForm(null));
                }
                else
                {
                  if ((AuthManager.UpdateUserProfile(sisUser, ref statusCode) == false))
                  {
                    return RedirectToAction("UserModifiedFailed", "Account");
                  }
                  else
                  {
                    // Added Code to Update Conatct Email ID incase Email ID has changed by User.
                    // This event should only initiate at Edit mode.
                    //SCP333083: XML Validation Failure for A30-XB - SIS Production
                    MemberManager.UpdateContactEmailId(sisUser.Email, viewInfo.HiddenEmailAddress, sisUser.Member.MemberID);
                  }
                }
                // If we got this far, something failed, redisplay form
                TempData["ShouldEdit"] = sisUser.Email;
                TempData["ShowSavedMessage"] = "true";
                
                SessionUtil.UserLanguageCode = viewInfo.UserLanguageCode;

                return RedirectToAction("UserEditFromMember", "Account", new { emailID = sisUser.Email });
            }

            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        [HomeAuthorizeAttribute]
        public ActionResult UserModify()
        {
            if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
            {
                ViewData["PasswordLength"] = AuthManager.MinPasswordLength;
                ViewData["Role1"] = "Administrator";
                var emailOfPersonToEdit = (string)ViewData["ShouldEdit"];
                if (string.IsNullOrWhiteSpace(emailOfPersonToEdit) == false)
                {
                    ViewData["ShouldEdit"] = emailOfPersonToEdit;
                }

                ViewData["UserModify"] = true;
                //  PopulateRegisterForm();
                if (Equals(TempData["ShowSavedMessage"], "true"))
                {
                    TempData["ActivationFailedError"] = "false";
                    ViewData[ViewDataConstants.SuccessMessage] = "User information saved successfully";
                    TempData["ShowSavedMessage"] = "false";
                }

                // CMP#668: Archival of IS-WEB Users and Removal from Screens
                TempData["isUserCreation"] = "False";

                return View("UserModify", PopulateModifyUserForm(null));
            }

            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        public ActionResult UserModify([Bind(Prefix = "")] CreateUserView viewInfo)
        {
            // Retrieve UserId from Session and use it across method
            var userId = SessionUtil.UserId;
            //SCP464513:XML Validation Failure for 957-JJ - SIS Production 
            var dbEmailId = string.Empty;
            if (string.IsNullOrEmpty(userId.ToString()) == false)
            {
                ViewData["PasswordLength"] = 6;

                var sisUser = AuthManager.GetUserByUserID(int.Parse(userId.ToString()));
                //SCP464513:XML Validation Failure for 957-JJ - SIS Production 
                viewInfo.HiddenEmailAddress = dbEmailId = sisUser.Email;

                if ((String.IsNullOrEmpty(viewInfo.HiddenEmailAddress)))
                {
                    viewInfo.HiddenEmailAddress = sisUser.Email;
                }

              // Validate mandetary fields ie First Name and Email Id
                if(!ValidateUserNameAndEmailAddress(viewInfo))
                {
                    return View("UserModify", PopulateModifyUserForm(null));
                }

                if (!ValidateRegistration(viewInfo, false))
                {
                    //PopulateRegisterForm();
                    return View("UserModify", PopulateModifyUserForm(null));
                }

                if (viewInfo.LastName == null)
                    viewInfo.LastName = String.Empty;
                if (viewInfo.PositionTitle == null)
                    viewInfo.PositionTitle = String.Empty;
                if (viewInfo.StaffID == null)
                    viewInfo.StaffID = String.Empty;
                if (viewInfo.Department == null)
                    viewInfo.Department = String.Empty;
                if (viewInfo.Divison == null)
                    viewInfo.Divison = String.Empty;
                if (viewInfo.SITAAddress == null)
                    viewInfo.SITAAddress = String.Empty;

                sisUser.Salutation = viewInfo.Salutation;
                sisUser.FirstName = viewInfo.FirstName;
                sisUser.LastName = viewInfo.LastName;
                sisUser.Email = viewInfo.EmailAddress;

                sisUser.PositionOrTitle = viewInfo.PositionTitle;
                sisUser.StaffID = viewInfo.StaffID;
                sisUser.Department = viewInfo.Department;
                sisUser.Division = viewInfo.Divison;

                sisUser.Phone.PhoneNumber1 = viewInfo.Telephone1;
                sisUser.Phone.PhoneNumber2 = viewInfo.Telephone2;
                sisUser.Phone.Fax = viewInfo.Fax;
                sisUser.Phone.MobileNumber = viewInfo.Mobile;
                sisUser.LanguageCode = viewInfo.UserLanguageCode;

                // CMP#668: Archival of IS-WEB Users and Removal from Screens
                sisUser.IsArchived = viewInfo.IsArchived;

                if (string.IsNullOrEmpty(viewInfo.LocationID) || viewInfo.LocationID == "0")
                {
                    sisUser.Location.Address1 = viewInfo.Address1;
                    sisUser.Location.Address2 = viewInfo.Address2;
                    sisUser.Location.Address3 = viewInfo.Address3;
                    sisUser.Location.POSTAL_CODE = viewInfo.PostalCode;

                    if (string.IsNullOrEmpty(viewInfo.SubDivisionName) == false)
                    {
                        sisUser.Location.SUB_DIVISION_CODE = viewInfo.SubDivisionCode;
                    }
                    else
                    {
                        sisUser.Location.SUB_DIVISION_CODE = null;
                    }

                    sisUser.Location.CITY_NAME = viewInfo.CityName;
                    if (!string.IsNullOrEmpty(viewInfo.CountryName))
                    {
                        sisUser.Location.COUNTRY_CODE = viewInfo.CountryCode;
                    }
                    else
                    {
                        sisUser.Location.COUNTRY_CODE = null;
                    }
                    sisUser.Location.LocationID = string.Empty;
                }
                else
                {
                    sisUser.Location.LocationID = viewInfo.LocationID;
                }


                sisUser.Location.SITA_Address = viewInfo.SITAAddress;

                var statusCode = 0;

                //SCP464513:XML Validation Failure for 957-JJ - SIS Production 
                //Change Desc:Initially viewInfo.HiddenEmailAddress was use for comparison but now dbEmailId will be used.
                //SCP333083: XML Validation Failure for A30-XB - SIS Production
                if (ContactRepository.Single(con => con.EmailAddress.ToLower() == dbEmailId.ToLower() && con.MemberId == sisUser.Member.MemberID) != null && dbEmailId.ToLower() != sisUser.Email.ToLower() && ContactRepository.GetCount(contacts => contacts.EmailAddress.ToLower() == sisUser.Email.ToLower() && contacts.MemberId == sisUser.Member.MemberID) > 0)
                {
                  ModelState.AddModelError("FirstNameRequired", string.Format("{0} {1}", sisUser.Email, AppSettings.DuplicateMemberContact));
                  return View("UserModify", PopulateModifyUserForm(null));
                }
                else
                {
                  if ((AuthManager.UpdateUserProfile(sisUser, ref statusCode) == false))
                  {
                    return RedirectToAction("UserModifiedFailed", "Account");
                  }
                  else
                  {
                     //SCP464513:XML Validation Failure for 957-JJ - SIS Production 
                    //Change Desc:Initially viewInfo.HiddenEmailAddress was pass as argument but now dbEmailId will be used.
                    // Added Code to Update Conatct Email ID incase Email ID has changed by User.
                    // This event should only initiate at Edit mode.
                    //SCP333083: XML Validation Failure for A30-XB - SIS Production
                    MemberManager.UpdateContactEmailId(sisUser.Email, dbEmailId, sisUser.Member.MemberID);
                  }
                }

                TempData["ShouldEdit"] = sisUser.Email;
                TempData["ShowSavedMessage"] = "true";
                
                SessionUtil.UserLanguageCode = viewInfo.UserLanguageCode;

                return RedirectToAction("UserModify", "Account");
            }

            return RedirectToAction("LogOn", "Account");
        }

        [Authorize]
        public ActionResult UserModifySuccess()
        {
            //Display success status message
            return View();
        }

        [Authorize]
        public ActionResult UserModifiedFailed()
        {
            // Display failed status message
            return View();
        }

        /// <summary>
        /// CMP685- Entry point from the Dynamic URL Link for password reset
        /// </summary>
        /// <param name="siscpt"></param>
        /// <returns></returns>
        public ActionResult ResetPassword(string siscpt)
        {
          // Validate the identifier
          // Get expiration time from configuartion
          var userId = 0; var linkid = Guid.Empty; var timeFrame = GetConfigKeyForUserManagement("AccountResetLinkExpireTime");
          if (UserManagementManager.ValidateResetPasswordLink(siscpt,timeFrame, ref userId, ref linkid))
          {
            // set the page token with identifier which will be used to page validation while each navigation
            ViewData["siscpt"] = siscpt;
            return View();
          }
          
          //redirect to failed page message 
          return RedirectToAction("ResetPasswordFailed");
        }


        private int GetConfigKeyForUserManagement(string key)
        {
          if (key == "AccountResetLinkExpireTime")
          {
            try
            {
              var val = ConfigurationManager.AppSettings["AccountResetLinkExpireTime"];
              int num;
              if (!string.IsNullOrEmpty(val) && int.TryParse(val, out num))
              { 
                return num; 
              }
              else
              {
                Logger.Info("Error in getting AccountResetLinkExpireTime form [GetConfigKeyForUserManagement] from config");
                return 48;
              }              
            }
            catch (Exception ex)
            {
              Logger.Error("Error in getting AccountResetLinkExpireTime from  [GetConfigKeyForUserManagement]", ex);
              return 48;
            }
          }
          if (key == "AccountLockoutTime")
          {
            try
            {
              var val = ConfigurationManager.AppSettings["AccountLockoutTime"];
              int num;
              if (!string.IsNullOrEmpty(val) && int.TryParse(val, out num))
              {
                return num;
              }
              else
              {
                Logger.Info("Error in getting AccountLockoutTime form [GetConfigKeyForUserManagement] from config");
                return 30;
              }
            }
            catch (Exception ex)
            {
              Logger.Error("Error in getting AccountLockoutTime from  [GetConfigKeyForUserManagement]", ex);
              return 30;
            }
          }
          return 1;
        }

        
        [AcceptVerbs(HttpVerbs.Post)]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions result in password not being changed.")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string newPassword, string confirmPassword, string siscpt)
        {
          // validate the identifier again once user input the details 
          var userId = 0; var linkid = Guid.Empty; var timeFrame = GetConfigKeyForUserManagement("AccountResetLinkExpireTime");
          if (UserManagementManager.ValidateResetPasswordLink(siscpt,timeFrame,ref userId, ref linkid))
          {
            // reset the page token with identifier which will be used for page validation while each navigation
            ViewData["siscpt"] = siscpt;

            // validate input password
            if (!ValidateChangePassword(newPassword, confirmPassword))
            {
              return View("ResetPassword", "~/Views/Shared/Anonymous.Master");
            }

            var statusCode = 0; // update the password here
            if (!AuthManager.ResetPassword(userId, newPassword, confirmPassword, ref statusCode))
            {              
              if (statusCode == (int)ErrorCodes.InvalidPassword)
              {
                ModelState.AddModelError("ResetPassword", AppSettings.ChangePasswordFailedMessage1);
              }
              else
              {
                if (statusCode == (int)ErrorCodes.InvalidPasswordFormat)
                {
                  ModelState.AddModelError("ResetPassword", AppSettings.ChangePasswordFailedMessage2);
                }
                else
                {
                  ModelState.AddModelError("ResetPassword", AppSettings.ChangePasswordFailedMessage3);
                }
              }

              return View("ResetPassword", "~/Views/Shared/Anonymous.Master");
            }
            else if (UserManagementManager.DeleteResetPasswordLinkDataOnceUsed(userId, linkid)) // if password updated succesfully..delete the Dynamic Url Link data of password reset
            {
              return RedirectToAction("ChangePasswordSuccess");
            }
          }

          //redirect to failed page message 
          return RedirectToAction("ResetPasswordFailed");
        }



        //  [Authorize]
        public ActionResult ChangePassword()
        {
          ViewData["PasswordLength"] = AuthManager.MinPasswordLength;
          //    ShouldDisplayLogoutOptionProxy();
          var sisUser = AuthManager.GetUserByUserID(int.Parse(SessionUtil.UserId.ToString()));

          if (sisUser.UserID > 0)
          {

            CurrentUserName = sisUser.FirstName;
            SessionUtil.Username = string.Format("{0} {1}", sisUser.FirstName, sisUser.LastName);

          }

          ViewBag.DisplayCancelButton = false;
          if ((sisUser.UserID > 0) && (!sisUser.PasswordIsExpired))
          {
            ViewBag.DisplayCancelButton = true;
          }
          else
          {
            ViewData["ExpiredPasswordMessage"] = AppSettings.CurrentOrTemporaryPasswordExpiredMessage1;

            return View("ChangePassword", "~/Views/Shared/Anonymous.Master");
          }

          return View();

        }

        //  [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions result in password not being changed.")]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
          // Redirect to Login Screen in case session expired
          if (SessionUtil.UserId == 0)
          {
            return RedirectToAction("LogOn", "Account");
          }

          if (!ValidateChangePassword(newPassword, confirmPassword))
          {
            if (SessionUtil.IsLoggedIn)
              return View();
            else
              return View("ChangePassword", "~/Views/Shared/Anonymous.Master");
          }

          if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()) == false)
          {
            var sisUser = AuthManager.GetUserByUserID(int.Parse(SessionUtil.UserId.ToString()));            

            var statusCode = 0;
            AuthManager.FromEmailAddress = AppSettings.AdministratorEmail;
            if (!AuthManager.ChangePassword(sisUser.UserID, currentPassword, newPassword, confirmPassword, false, ref statusCode))
            {
              if (statusCode == (int)ErrorCodes.InvalidPassword)
              {
                ModelState.AddModelError("ChangePassword", AppSettings.ChangePasswordFailedMessage1);
                if (sisUser.LoginAttempts >= 3)
                {
                  AuthManager.LockUser(sisUser.UserID);
                  return RedirectToAction("ChangePasswordFailed");
                }

                sisUser = AuthManager.GetUserByUserID(int.Parse(SessionUtil.UserId.ToString()));
                statusCode = 0;
                sisUser.LoginAttempts = sisUser.LoginAttempts + 1;
                AuthManager.UpdateUserProfile(sisUser, ref statusCode);
              }
              else
              {
                if (statusCode == (int)ErrorCodes.InvalidPasswordFormat)
                {
                  ModelState.AddModelError("ChangePassword", AppSettings.ChangePasswordFailedMessage2);
                }
                else
                {
                  ModelState.AddModelError("ChangePassword", AppSettings.ChangePasswordFailedMessage3);
                }
              }              

              if (SessionUtil.IsLoggedIn)
                return View();
              else
                return View("ChangePassword", "~/Views/Shared/Anonymous.Master");
            }
          }
          else
          {
            return RedirectToAction("ChangePasswordFailed");
          }

          ViewData["PasswordLength"] = AuthManager.MinPasswordLength;

          //SessionUtil.UserId = 0;
          Session.Abandon();

          return RedirectToAction("ChangePasswordSuccess");
        }

        // [Authorize]
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        // [Authorize]
        public ActionResult ResetPasswordFailed()
        {
          return View();
        }

        // [Authorize]
        public ActionResult ChangePasswordFailed()
        {
          // Retrieve UserId from Session and use it across method
          var userId = SessionUtil.UserId;

          if (userId <= 0)
          {
            ViewData["FailedStatusMessage"] = "Change Password Failed. Your Session Has Been Lost. Please Try Again Later.";
          }
          else
          {
            ViewData["FailedStatusMessage"] = "Change Password Failed. Maximum Number Of Attempts Has Been Reached. Your Account Is Now Locked.";
            ViewData["NextSteps"] = "Please See Your Administrator To Have Your Account Unlocked.";
            AuthManager.SignOut(userId);
            Session.RemoveAll();
            Session.Abandon();
          }

          FormsAuthentication.SignOut();
          return View();
        }

        [Authorize]
        public ActionResult ProxyLoginSuccess()
        {
            if (SessionUtil.IsLoggedIn)
                return View();
            else
                return RedirectToAction("LogOn", "Account");
        }                   
        
        //[Authorize]
        public ActionResult ForgotPassword()
        {
            return View();
        }

       
        // [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForgotPassword(string userId, string captchaText)
        {
          if (!String.IsNullOrEmpty(userId))
          {

            // Validate Captcha text
            if (SessionUtil.CaptchaString != captchaText)
            {
              ModelState.AddModelError("InvalidCaptcha", "Invalid Captcha : Please retry again with captcha shown");
              return View();
            }
            //SCP#490868:SIS password has been reset - SIS Production
            var sisUser = AuthManager.GetUserByUserName(userId);

            if (sisUser.UserID != 0)
            {
              //SCP490868 - FW: SIS password has been reset - SIS Production
              if(sisUser.IsActive == false)
              {
                  ModelState.AddModelError("InActiveUser", "Your account is not active and the password cannot be reset.");
                  return View();
              }

                //CMP685- USER MANAGMENT
              // generate and send the Dynamic URL link for password reset here
              string carrierFlag = "F"; string urlPath = Path.Combine(SystemParameters.Instance.General.LogOnURL, AppSettings.UrlPasswordResetLink).Replace("\\", "/");
              if (UserManagementManager.ResetUserPassword(sisUser.UserID, urlPath, ref carrierFlag))
              {
                return RedirectToAction("ForgotPasswordSuccess", "Account");
              }
              else
              {
                return RedirectToAction("ForgotPasswordFailed", "Account");
              }
            }
            else
            {
                //NII001: User Enumeration: Ease of Exploitation
               return RedirectToAction("ForgotPasswordSuccess", "Account");
            }
          }
          else
          {
              //SCP490868 - FW: SIS password has been reset - SIS Production
              ModelState.AddModelError("InvalidUserId", "Username is mandatory. Please enter Username.");
              return View();
          }          
        }

        
        // [Authorize]
        public ActionResult ForgotPasswordSuccess()
        {
            return View();
        }

        // [Authorize]
        public ActionResult ForgotPasswordFailed()
        {
            return View();
        }

        //  [Authorize]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity is WindowsIdentity)
            {
                throw new InvalidOperationException("Windows authentication is not supported.");
            }
        }

        #region Validation Methods
        // ID : 271100 - Manage Users - Screen
        private bool ValidateSearchValues(string firstName, string lastName, string emailAddress, string status, string shouldEdit, string shouldProxy, string memberEmailAddress, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (SessionUtil.UserId <= 0)
            {
                return false;
            }

            if (String.IsNullOrEmpty(memberEmailAddress))
            {
                memberEmailAddress = string.Empty;
            }

            if ((String.IsNullOrEmpty(shouldEdit) == false) && (shouldEdit.ToUpper() == shouldProxy.ToUpper()))
            {
                ModelState.AddModelError("ProxyAndEditButtonBothHit", "Please Try Again, if problem persists, then contact administrator");
            }

            if (!Regex.IsMatch(firstName, "^[-a-zA-Z ]*$"))
            {
                errorMsg = "Invalid first name";
                ModelState.AddModelError("FirstNameSearchInvalid", "Search First Name Invalid");
            }

            if (!Regex.IsMatch(lastName, "^[-a-zA-Z ]*$"))
            {
                errorMsg = "Invalid last name";
                ModelState.AddModelError("LastNameSearchInvalid", "Search Last Name Invalid");
            }

            if (String.IsNullOrEmpty(shouldEdit) && Regex.IsMatch(shouldEdit.ToUpper(), @"^[a-zA-Z0-9._+-]+@[a-zA-Z0-9._+-]+.([a-zA-Z0-9._+-])*$"))
            {
                errorMsg = "Should edit invalid";
                ModelState.AddModelError("ShouldEditInvalid", "Please Try Again, if problem persists, then contact administrator");
            }

            if (String.IsNullOrEmpty(shouldProxy) && Regex.IsMatch(shouldProxy.ToUpper(), @"^[a-zA-Z0-9._+-]+@[a-zA-Z0-9._+-]+.([a-zA-Z0-9._+-])*$"))
            {
                errorMsg = "Should edit invalid";
                ModelState.AddModelError("ShouldEditInvalid", "Please Try Again, if problem persists, then contact administrator");
            }

            return ModelState.IsValid;
        }

        private bool ValidateChangePassword(string newPassword, string confirmPassword)
        {
            if (newPassword == null || newPassword.Length < AuthManager.MinPasswordLength)
            {
                ModelState.AddModelError("NewPasswordInvalid", String.Format(CultureInfo.CurrentCulture, AppSettings.SpecifyAPasswordOfCharacters, MinPasswordLength));
            }

            if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                ModelState.AddModelError("NewAndConfirmPasswordMisMatch", AppSettings.PasswordsDoNotMatchMessage);
            }

            return ModelState.IsValid;
        }
        
        private bool ValidateLogOn(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName))
            {
                ModelState.AddModelError("username", "You must specify a username.");
            }
            if (String.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "You must specify a password.");
            }

            return ModelState.IsValid;
        }

        [Authorize]
        private bool ValidateUserNameAndEmailAddress(CreateUserView userProfileInfoFromForm)
        {
          if (string.IsNullOrWhiteSpace(userProfileInfoFromForm.FirstName) == true)
          {
            ModelState.AddModelError("FirstNameRequired", string.Format("{0} {1}", userProfileInfoFromForm.FirstName, AppSettings.FirstNameRequiresAValueMessage));
          }
          else
          {
            if(!Regex.IsMatch(userProfileInfoFromForm.FirstName, @"^[-a-zA-Z ]*$"))
            {
              ModelState.AddModelError("FirstNameRegularExpression", String.Format(AppSettings.InvalidWithParameter, userProfileInfoFromForm.FirstName));
            }
            if (userProfileInfoFromForm.FirstName.Length > 100)
            {
              ModelState.AddModelError("FirstNameOfPersonToEditTooLong", AppSettings.FirstNameLengthTooLongText1);
            }
          }

          if (string.IsNullOrWhiteSpace(userProfileInfoFromForm.EmailAddress) == true)
          {
            ModelState.AddModelError("EmailRequiresAValue", string.Format("{0} {1}", userProfileInfoFromForm.EmailAddress, AppSettings.EmailRequiresAValue));
          }
          else
          {
              //SCP207710 - Change Super User(Allow valid special character).
              if (!Regex.IsMatch(userProfileInfoFromForm.EmailAddress.ToLower(), FormatConstants.ValidEmailPattern))
              {
                  ModelState.AddModelError("EmailAddressRegularExpression", String.Format(AppSettings.InvalidWithParameter, userProfileInfoFromForm.EmailAddress));
              }
              if (userProfileInfoFromForm.EmailAddress.Length > 250)
              {
                  ModelState.AddModelError("EmailAddressOfPersonToEditTooLong", AppSettings.EmailAddressTooLongText1);
              }
          }

          return ModelState.IsValid;
        }

        [Authorize]
        private bool ValidateRegistration(CreateUserView userProfileInfoFromForm, bool isCreating)
        {
            if (string.IsNullOrEmpty(userProfileInfoFromForm.EmailAddress) == false)
            {
                var emailOfPersonToEdit = string.Empty;
                if (string.IsNullOrEmpty(userProfileInfoFromForm.HiddenEmailAddress) == false)
                {
                    emailOfPersonToEdit = userProfileInfoFromForm.HiddenEmailAddress;
                }

                var userListToCheckEmail = (ISUser)AuthManager.GetUserByUserName(userProfileInfoFromForm.EmailAddress.ToUpper());
                var emailChanged = false;

                if (isCreating == false)
                {
                    emailChanged = (userProfileInfoFromForm.HiddenEmailAddress.ToUpper().Equals(userProfileInfoFromForm.EmailAddress.ToUpper()) == false);
                }

                if (((isCreating) || (emailChanged)) && (userListToCheckEmail.UserID > 0))
                {
                    ModelState.AddModelError("EmailInUse", string.Format("{0} {1}", userProfileInfoFromForm.EmailAddress, AppSettings.EmailAlreadyExistsMessage));
                }
                else if (isCreating == false)
                {
                    //UserListToCheckEmail is either user being edited or another user

                    if (String.IsNullOrWhiteSpace(emailOfPersonToEdit) == false)
                    {
                        if (!Regex.IsMatch(emailOfPersonToEdit.ToLower(), FormatConstants.ValidEmailPattern))
                        {
                            ModelState.AddModelError("EmailAddressRegularExpression", String.Format(AppSettings.InvalidWithParameter, userProfileInfoFromForm.EmailAddress));
                        }
                        if (userProfileInfoFromForm.HiddenEmailAddress.Length > 250)
                        {
                            ModelState.AddModelError("EmailAddressOfPersonToEditTooLong", AppSettings.EmailAddressTooLongText1);
                        }
                    }
                    else
                    {
                        var checkIfModifying = AuthManager.GetUserByUserID(int.Parse(SessionUtil.UserId.ToString()));
                        if (checkIfModifying.Email.Equals(userListToCheckEmail.Email) == false)
                        {
                            ModelState.AddModelError("EmailAddressRegularExpression", String.Format(AppSettings.InvalidWithParameter, userProfileInfoFromForm.EmailAddress));
                        }
                    }
                }

                if (isCreating)
                {
                    var memberId = string.IsNullOrEmpty(userProfileInfoFromForm.MemberData) ? SessionUtil.MemberId : Convert.ToInt32(userProfileInfoFromForm.MemberData);

                    if (memberId > 0)
                    {
                        // Check EmailId in Member Contact for this Member
                        var memberContact = MemberManager.GetContactDetailsByEmailAndMember(userProfileInfoFromForm.EmailAddress, Convert.ToInt32(userProfileInfoFromForm.MemberData));
                        if (memberContact != null)
                        {
                            if (memberContact.MemberId != memberId)
                            {
                                ModelState.AddModelError("EmailAddressIsMemberUserOfOtherUser", string.Format(AppSettings.EmailAddressIsMemberUserOfOtherUser, userProfileInfoFromForm.EmailAddress));
                            }
                        }
                    }
                }
            }

            return ModelState.IsValid;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ISAuthorize(Menu.RepFinCtrlAccessIchWebReports)]
        public ActionResult RedirectToIch()
        {

            var sisUser = AuthManager.GetUserByUserID(SessionUtil.UserId);
            var ichWebUrl = SystemParameters.Instance.ICHDetails.WebReportSingleSignOnURL;
            try
            {
                if (sisUser != null)
                {
                    var member = MemberManager.GetMember(sisUser.Member.MemberID);
                    var memberIch = MemberManager.GetIchConfig(sisUser.Member.MemberID);
                    //if no configuration found set to zero.
                    int ichWebReportOptionsId = memberIch != null ? memberIch.IchWebReportOptionsId : 0;
                    //if (Request.Cookies["ichwebauth"] == null)
                    //{
                    var strKey = SystemParameters.Instance.ICHDetails.IchWebReportEncryptionKey;
                    var contentDetails = string.Format("{0};{1}{2};{3};{4}", sisUser.Email, member.MemberCodeAlpha,
                                                       member.MemberCodeNumeric, ichWebReportOptionsId,
                                                       DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                    var encryptedContent = CryptoWrapper.EncryptAes(contentDetails, strKey);
                    //string content = CryptoWrapper.EncryptAes("mikejones;LH220;1", strKey);
                    //var myCookie = new HttpCookie("ichwebauth", encryptedContent) { Expires = DateTime.Now.AddMinutes(20)};
                    var ichwebauth = new HttpCookie("ichwebauth")
                    {
                        Name = "ichwebauth",
                        Value = encryptedContent,
                        Expires = DateTime.Now.AddMinutes(20),
                        Domain = "iata.org"
                    };
                    Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
                    Response.Cookies.Set(ichwebauth);
                    //}
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message, e.GetBaseException());
            }
            return Redirect(ichWebUrl);
            //  return RedirectToAction("TestViewForIch");
        }

        /// <summary>
        /// CMP #645: Access ACH Settlement Reports
        /// Function to get URL from SISConfig.xml and redirect in new tab
        /// </summary>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Get)]
        [ISAuthorize(Menu.RepFinCtrlAccessAchSettlementReports)]
        public ActionResult RedirectToAch()
        {
            var achWebUrl = Iata.IS.Core.Configuration.ConnectionString.GetconfigAppSetting("AccessAchSettlementReportUrl");
            return Redirect(achWebUrl);
        }

        /// <summary>
        /// Added new menu for ICH Protest and adjustment.
        /// </summary>
        /// <returns></returns>
        //CMP #630: Access to ICH Protest and Adjustment Screen
        [AcceptVerbs(HttpVerbs.Get)]
        [ISAuthorize(Menu.AccessIchProtestAndAdjustment)]
        public ActionResult RedirectToIchProtestAndAdjustment()
        {
          var sisUser = AuthManager.GetUserByUserID(SessionUtil.UserId);
          var ichWebUrl = Iata.IS.Core.Configuration.ConnectionString.GetconfigAppSetting("IchProtestAndAdjustmentUrl");
          try
          {
            if (sisUser != null)
            {
              var member = MemberManager.GetMember(sisUser.Member.MemberID);
              var memberIch = MemberManager.GetIchConfig(sisUser.Member.MemberID);

              //if no configuration found set to zero.
              int ichWebReportOptionsId = memberIch != null ? memberIch.IchWebReportOptionsId : 0;
              var strKey = Iata.IS.Core.Configuration.ConnectionString.GetconfigAppSetting("IchProtestAndAdjustmentKey");

              //Decrypt key for  ICH Protest and Adjustment Screen
              var decrykey = Iata.IS.Core.Crypto.DecryptString(strKey).Trim();  
              var contentDetails = string.Format("{0};{1}{2};{3};{4}", sisUser.Email, member.MemberCodeAlpha,
                                                 member.MemberCodeNumeric, ichWebReportOptionsId,
                                                 DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
              var encryptedContent = CryptoWrapper.EncryptAes(contentDetails, decrykey);
              var ichwebauth = new HttpCookie("ichwebauth")
              {
                Name = "ichwebauth",
                Value = encryptedContent,
                Expires = DateTime.Now.AddMinutes(20),
                Domain = "iata.org"
              };
              Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
              Response.Cookies.Set(ichwebauth);
            }
          }
          catch (Exception e)
          {
            Logger.Error(e.Message, e.GetBaseException());
          }
          return Redirect(ichWebUrl);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [Authorize]
        public ActionResult RedirectToIataHelpDeskForm()
        {
            // Not Decided yet to keep below hardcoded URL into Sys Parameter
          //below link updated for SCP#59532
          return Redirect("http://www.iata.org/services/finance/sis/Pages/sis-help-desk.aspx");
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult TestViewForIch()
        {
            string strKey = SystemParameters.Instance.ICHDetails.IchWebReportEncryptionKey;
            if (Request != null)
            {
                if (Request.Cookies != null)
                {
                    if (Request.Cookies["ichwebauth"] != null)
                    {

                        var depryptedCookie = CryptoWrapper.DecryptAes(Request.Cookies["ichwebauth"].Value, strKey);

                        //var encryptedValue = Request.Cookies["ichwebauth"].Value.ToString();
                        var arrSplittedCookie = depryptedCookie.Split(Convert.ToChar(";"));
                        TempData["UserID"] = arrSplittedCookie[0];
                        TempData["MemberID"] = arrSplittedCookie[1];
                        TempData["ReportOption"] = arrSplittedCookie[2];
                        TempData["Timestamp"] = arrSplittedCookie[3];
                    }
                    else
                    {
                        TempData["UserID"] = "Cookie not found";
                        TempData["MemberID"] = string.Empty;
                        TempData["ReportOption"] = string.Empty;
                        TempData["Timestamp"] = string.Empty;
                    }
                }
            }
            // return Redirect("https://ichwebqa.iata.org/ICHWeb2010/Login/IATAApplicationsMenu.asp");
            return View();
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return " Username already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return " A username for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return " The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return " The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return " The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return " The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return " The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return " The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return " The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return " An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion

        protected override void OnException(ExceptionContext filterContext)
        {
            // Log the exception.
            Logger.Error("Unhandled exception.", filterContext.Exception);

            base.OnException(filterContext);
            filterContext.ExceptionHandled = true;
            object routeValue;

            if (filterContext.Exception is ISSessionExpiredException || SessionUtil.UserId <= 0)
            {
                Session.Clear();
                filterContext.HttpContext.ClearError();
                filterContext.HttpContext.Response.Cookies.Remove("ASP.NET_SessionId");
                routeValue = new { title = "Session Expired", area = "" };
            }
            else
            {
                routeValue = new { area = "" };
                if (filterContext.Exception.Message.IndexOf("A required anti-forgery token") > -1)
                {
                    routeValue = new { area = "", title = "anti-forgery" };
                }
            }

            filterContext.Result = RedirectToAction("Error", "Home", routeValue);
        }

        [Authorize]
        public JsonResult GetContactDetails(string emailId)
        {
            var contact = MemberManager.GetContactDetailsByEmail(emailId);
            return Json(contact);
        }

        /// <summary>
        ///   Set End user browser local time zone value
        /// </summary>
        /// <param name = "timeZoneId"></param>
        /// <returns></returns>
        // [Authorize]
        public JsonResult SetTimeZone(string timeZoneId)
        {
            SessionUtil.TimeZone = !string.IsNullOrEmpty(timeZoneId) ? timeZoneId : "Pacific Standard Time";

            return Json(true);
        }

        /// <summary>
        ///   Log on screen, clear all user related sessions
        /// </summary>
        /// <returns></returns>
        // [Authorize]
        public JsonResult ClearUserSessions()
        {
            SessionUtil.UserId = 0;
            SessionUtil.IsLogOutProxyOption = false;
            return Json(true);
        }

        // [Authorize]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult ValueProtect()
        {
            return View();
        }

        //[Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ProtectString()
        {
            SecurityUtility.ProtectConnectionString(Request.ApplicationPath, true);

            return Json(true);
        }

        //   [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UnProtectString()
        {
            SecurityUtility.UnprotectConnectionString(Request.ApplicationPath, true);
            return Json(true);
        }

        //  [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RemoveConnectionCache()
        {
            var filemaneger = Ioc.Resolve<IFileManager>(typeof(IFileManager));
            filemaneger.FlushConnectionstring();
            return Json(true);
        }

        // [Authorize]
        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateAntiForgeryToken]
        // SCP253260: FW: question regarding CMP 459 - Validation of RM Billed(Added lastUpdatedby Column)
        public ActionResult UploadSystemParam(HttpPostedFileBase uploadedFile)
        {
            if (uploadedFile.FileName.Length > 4 && uploadedFile.FileName.ToUpper().EndsWith("XML"))
            {
                var filemaneger = Ioc.Resolve<IFileManager>(typeof(IFileManager));
                filemaneger.EncryptSystemParameterXml(uploadedFile.FileName, SessionUtil.OperatingUserId, SessionUtil.UserId);
                TempData[ViewDataConstants.SuccessMessage] = "Uploaded Successfully";
            }
            else
            {
                TempData[ViewDataConstants.ErrorMessage] = "Uploaded Incorrect File version";
            }
            ModelState.AddModelError("sysparamfile", string.Format("Uploaded Sys Param File :{0}", uploadedFile.FileName));

            return RedirectToAction("ValueProtect");
        }

        private const string SearchResultGridAction = "SearchResultGridData";

        [Authorize]
        
        public ActionResult ManageUsers(ManageUsers manageUsers)
        {
            try
            {
                if (string.IsNullOrEmpty(SessionUtil.UserId.ToString()))
                {
                    return RedirectToAction("LogOn", "Account");
                }

                string criteria = manageUsers != null ? new JavaScriptSerializer().Serialize(manageUsers) : string.Empty;

                var permissionSearchGrid = new ManageUsersSearch(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
                {
                    criteria
                }));

                ViewData[ViewDataConstants.SearchGrid] = permissionSearchGrid.Instance;

            }
            catch (ISBusinessException exception)
            {
                // ShowErrorMessage(exception.ErrorCode);
            }


            return View(manageUsers);
        }

        [Authorize]
        public JsonResult SearchResultGridData(string criteria)
        {

            var permission = new ManageUsers();
            if (!string.IsNullOrEmpty(criteria))
            {
                permission = new JavaScriptSerializer().Deserialize(criteria, typeof(ManageUsers)) as ManageUsers;
            }


            var permissionSearchGrid = new ManageUsersSearch(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
            {
                permission
            }));

            var gridModel = new SearchBarModel();
            var ordersGrid = gridModel.OrdersGrid;

            gridModel.FirstName = permission.FirstName;
            gridModel.LastName = permission.LastName;
            gridModel.Email = permission.Email;
            gridModel.StatusId = Convert.ToInt32(permission.StatusId);
            gridModel.MemberId = Convert.ToInt32(permission.MemberId);
            gridModel.UserCategoryId = Convert.ToInt32(permission.UserCategoryId);

            var listOfUserData = GetUserSearchResults(gridModel);

            return permissionSearchGrid.DataBind(listOfUserData.AsQueryable());

        }

        /// <summary>
        /// Author: Sachin Pharande
        /// Date: 05-03-2012
        /// Purpose: This method is written to check the UserSession. [SCP ID: 11529-PR2290493 IS-WEB problems]
        /// Response: this method will returns string "true" if user session is alive else it will return LogOn URL
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CheckUserSessions()
        {
          //SCP310398 - SRM:Exception occurred in Report Download Service. - SIS Production - 10Nov
          if (System.Web.HttpContext.Current.Session["userId"] != null && SessionUtil.UserId > 0)
          {
            return Json("true");
          }
          else
          {
            // Clear User Related Session
            SessionUtil.UserId = 0;
            SessionUtil.IsLogOutProxyOption = false;
            SessionUtil.IsLoggedIn = false;
            Session.Abandon();


            //SCP305855 - UAT: Session expired when rasing a RM- Spira Case 9766
            return Json("Session_Expired");

            //throw new ISSessionExpiredException("Session expired.");
            //var retrnParam = SystemParameters.Instance.General.LogOnURL;
            //return Json(retrnParam);
          }
        }

        /// <summary>
        /// SSO Error Page.
        /// CMP #665: User Related Enhancements-FRS-v1.2.doc
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        public ActionResult SSOError(string title)
        {
            return View();
        }

        // End of CheckUserSessions()
        /// <summary>
        /// To Show error message on screen // ID : 271100 - Manage Users - Screen
        /// </summary>
        /// <param name="message"></param>
        /// <param name="crossRequest"></param>
        private void ShowErrorMessage(string message, bool crossRequest = false)
        {
          if (crossRequest)
          {
            TempData[ViewDataConstants.ErrorMessage] = message;
          }
          else
          {
            ViewData[ViewDataConstants.ErrorMessage] = message;
          }
        }

        /// <summary>
        /// This function is used to get user category Name based on category id [CMP #665: User related enhancement].
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [NonAction]
        private static String GetUserCategoryName(int categoryId)
        { 
            var categoryName = string.Empty;
            
            //Category Id.
            switch (categoryId)
            {
                case (int)UserCategory.SisOps:
                    categoryName = "SIS Ops";
                    break;

                case (int)UserCategory.IchOps:
                    categoryName = "ICH Ops";
                    break;

                case (int)UserCategory.AchOps:
                    categoryName = "ACH Ops";
                    break;

                case (int)UserCategory.Member:
                    categoryName = "Member";
                    break;
            }

            //return category name.
            return categoryName;
        }

        #region Captcha Implimentation
        /// <summary>
        /// ShowCaptchaImage on UI
        /// </summary>
        /// <returns></returns>
        [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
        public FileResult ShowCaptchaImage()
        {
          int Imgwidth = 100;
          int Imgheight = 25;
          int CaptchaLenght = 5;

          // Create image
          Bitmap bmp = new Bitmap(Imgwidth, Imgheight);
          Graphics g = Graphics.FromImage(bmp);
          g.Clear(Color.FromArgb(215, 233, 248));

          // draw random lines on image
          var myPen = new Pen(System.Drawing.Color.FromArgb(0, 94, 189), 1);
          var r = new Random();
          for (var i = 0; i <= 3; i++) // <-- limit Here the line count
          {
            g.DrawLine(myPen, r.Next(0, Imgwidth * 20 / 100), r.Next(0, Imgheight),
                       r.Next((Imgwidth - (Imgwidth * 20 / 100)), Imgwidth), r.Next(0, Imgheight));
            g.DrawArc(myPen, 0, 15, 15, 10, 5, 0);
          }

          // Get captcha string and store it in session
          string randomString = UserManagementManager.GetCaptchaString(CaptchaLenght);
          SessionUtil.CaptchaString = randomString;

          //create a font for the text and shadow text
          Font font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold);

          //create a brush for the text and shadow text
          Brush shadowBrush = new SolidBrush(Color.FromArgb(255, 255, 255)); // <-- Here
          Brush textBrush = new SolidBrush(Color.FromArgb(0, 94, 189));

          // draw string on image
          g.DrawString(randomString, font, shadowBrush, 3, 3); // <-- Here
          g.DrawString(randomString, font, textBrush, 2, 2);

          var ms = new MemoryStream();
          bmp.Save(ms, ImageFormat.Png);
          bmp.Dispose();
          //SIS_SCR_REPORT_23_jun-2016_2 :Improper_Resource_Shutdown_or_Release
          g.Dispose();
          return File(ms.GetBuffer(), @"image/png");
        }

        #endregion
    }
}
