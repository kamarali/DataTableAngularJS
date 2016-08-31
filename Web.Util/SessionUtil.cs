using System;
using System.Collections.Generic;
using System.Web;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile.Enums;
using log4net;
using System.Reflection;

namespace Iata.IS.Web.Util
{
  /// <summary>
  /// Utility class that acts as a gateway to the session state. All session usage must happen through this interface.
  /// </summary>
  public class SessionUtil
  {
    private const string UserIdKey = "userId";
    private const string UserLanguageCodKey = "UserLanguageCode";
    private const string AdminUserIdKey = "AdminUserId";
    private const string OperatingUserIdKey = "OperatingUserId";
    private const string SelectedUserIdKey = "SelectedUserId";
    private const string UsernameKey = "username";
    private const string TimeZoneKey = "TimeZone";
    private const string ProxyOptionKey = "ProxyOption";
    private const string UserMemberIdKey = "MemberId";
    private const string UserMemberNameKey = "MemberName";
    private const string InvoiceSearchCriteriaKey = "SearchCriteria";
    private const string SearchTypeKey = "BHSearchType";
    private const string SelectedMemberIdKey = "SelectedMemberId";
    private const string FormCSearchCriteriaKey = "FormCSearchCriteria";
    private const string MiscInvoiceSearchCriteriaKey = "MiscInvoiceSearchCriteria";
    private const string BillingCategoryIdKey = "BillingCategoryId";
    private const string PaxInvoiceSearchCriteriaKey = "PaxInvoiceSearchCriteriaKey";
    private const string CGOInvoiceSearchCriteriaKey = "CGOInvoiceSearchCriteriaKey";
    private const string ISEmailToSend = "IsEmailToSend";
    private const string CorrSearchCriteriaKey = "CorrSearchCriteria";
    private const string PaxCorrSearchCriteriaKey = "PaxCorrSearchCriteria";
    private const string CGOCorrSearchCriteriaKey = "CGOCorrSearchCriteria";
    private const string CgoCorrTrailSearchCriteriaKey = "CgoCorrTrailSearchCriteriaKey";
    private const string MiscCorrTrailSearchCriteriaKey = "MiscCorrTrailSearchCriteriaKey";
    private const string UatpCorrTrailSearchCriteriaKey = "UatpCorrTrailSearchCriteriaKey";
    private const string PaxCorrTrailSearchCriteriaKey = "PaxCorrTrailSearchCriteriaKey";
    private const string PageSizeSelectedKey = "PageSizeSelected";

    private const string RejectionMemoRecordIdsKey = "RejectionMemoRecordIds";
    private const string CurrentPageSelectedKey = "CurrentPageSelected";
    private const string CurrentPageGridIdKey = "CurrentPageGridId";

    private const string UserCategoryIdKey = "UserCategoryID";
    private const string UserCategoryOfSelectedUserKey = "UserCategoryOfSelectedUser";
    private const string UnsavedWarningMessagesEnabledKey = "unsavedWarningMessages";


    private const string ISSuperUser = "IsSuperUserCreation";
    private const string ManageUserSearchCriteriaKey = "ManageUserSearchCriteria";

    private const string IsLoggedInKey = "IsLoggedIn";
    private const string NewPasswordKey = "NewPassword";

    private const string SessionIdKey = "SessionID";

    private const string AlertRequiredKey = "AlertRequired";

    private const string AlertMessagesDataKey = "AlertMessagesData";
    
    private const string UserType = "UserType";

    private const string PaxInvoiceBHRecords = "PaxInvoiceBillingHistoryRecords";
    private const string LocationAssociationSearchIdsKey = "LocationAssociationSearchIds";

    /// CMP685 - User Management : Hold captchastring
    private const string CaptchaStringKey = "captchastring";

    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private static void ThrowIfSessionExpired()
    {
      //try
      //{
        var isNewSession = HttpContext.Current.Session.IsNewSession;
        var cookieHeader = HttpContext.Current.Request.Headers["Cookie"];
        var isCookiePosted = cookieHeader != null && cookieHeader.IndexOf("ASP.NET_SessionId") >= 0;

        if (isNewSession && isCookiePosted)
        {
            try
            {
                Logger.InfoFormat("isNewSession {0}", isNewSession.ToString());
                Logger.InfoFormat("Cookie ASP.NET_SessionId {0}", cookieHeader.IndexOf("ASP.NET_SessionId"));
                Logger.InfoFormat("Cookie lastVisit {0}", cookieHeader.IndexOf("lastVisit"));
                HttpContext.Current.Session.Abandon();
            }
            catch (Exception ex) { }
          throw new ISSessionExpiredException("Session expired.");
        }
      //}
      //catch (Exception ex)
      //{

      //}
    }

    /// <summary>
    /// To Get Selected User Type Id
    /// </summary>
    public static int UserTypeId
    {
      get
      {
        if (HttpContext.Current.Session[UserType] != null)
        {
          return (int)HttpContext.Current.Session[UserType];
        }
        ThrowIfSessionExpired();
        return 0;
      }
      set
      {
        HttpContext.Current.Session[UserType] = value;
      }
    }

    public static int UserId
    {
      get
      {
        if (HttpContext.Current.Session[UserIdKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[UserIdKey]);
        }

        ThrowIfSessionExpired();


        return 0;
      }

      set
      {
        HttpContext.Current.Session[UserIdKey] = value;
      }
    }

    public static string UserLanguageCode
    {
      get
      {
        if (HttpContext.Current.Session[UserLanguageCodKey] != null)
        {
          return HttpContext.Current.Session[UserLanguageCodKey] as string;
        }
        ThrowIfSessionExpired();
        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[UserLanguageCodKey] = value;

      }
    }

    public static int AdminUserId
    {
      get
      {
        if (HttpContext.Current.Session[AdminUserIdKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[AdminUserIdKey]);
        }

        ThrowIfSessionExpired();

        return 0;
      }

      set
      {
        HttpContext.Current.Session[AdminUserIdKey] = value;
      }
    }

    public static int OperatingUserId
    {
      get
      {
        if (HttpContext.Current.Session[OperatingUserIdKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[OperatingUserIdKey]);
        }

        ThrowIfSessionExpired();

        return 0;
      }

      set
      {
        HttpContext.Current.Session[OperatingUserIdKey] = value;
      }
    }


    /// <summary>
    /// Selected User Id in permission to User screen
    /// </summary>
    public static int SelectedUserId
    {
      get
      {
        if (HttpContext.Current.Session[SelectedUserIdKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[SelectedUserIdKey]);
        }

        ThrowIfSessionExpired();

        return 0;
      }

      set
      {
        HttpContext.Current.Session[SelectedUserIdKey] = value;
      }
    }

    public static UserCategory UserCategory
    {
      get
      {
        if (HttpContext.Current.Session[UserCategoryIdKey] != null)
        {
          return (UserCategory)HttpContext.Current.Session[UserCategoryIdKey];
        }

        ThrowIfSessionExpired();

        return UserCategory.Member;
      }

      set
      {
        HttpContext.Current.Session[UserCategoryIdKey] = value;
      }
    }

    /// <summary>
    /// Permission to User : User Category Of selected user to maintain for Copy permission functionality
    /// </summary>
    public static int UserCategoryOfSelectedUser
    {
      get
      {
        if (HttpContext.Current.Session[UserCategoryOfSelectedUserKey] != null)
        {
          return (int)HttpContext.Current.Session[UserCategoryOfSelectedUserKey];
        }

        ThrowIfSessionExpired();

        return 0;
      }

      set
      {
        HttpContext.Current.Session[UserCategoryOfSelectedUserKey] = value;
      }
    }

    /// <summary>
    /// Username of the logged in user.
    /// </summary>
    public static string Username
    {
      get
      {
        if (HttpContext.Current.Session[UsernameKey] != null)
        {
          return HttpContext.Current.Session[UsernameKey] as string;
        }

        ThrowIfSessionExpired();

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[UsernameKey] = value;
      }
    }

    /// <summary>
    /// Proxy Login/Logout Option 
    /// </summary>
    public static bool IsLogOutProxyOption
    {
      get
      {
        if (HttpContext.Current.Session[ProxyOptionKey] != null)
        {
          return (bool)HttpContext.Current.Session[ProxyOptionKey];
        }

        return false;
      }

      set
      {
        HttpContext.Current.Session[ProxyOptionKey] = value;
      }
    }

    /// <summary>
    /// Logged in user's member id.
    /// </summary>
    public static int MemberId
    {
      get
      {
        if (HttpContext.Current.Session[UserMemberIdKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[UserMemberIdKey].ToString());
        }

        ThrowIfSessionExpired();

        return 0;
      }

      set
      {
        HttpContext.Current.Session[UserMemberIdKey] = value;
      }
    }

    /// <summary>
    /// MemberId 
    /// </summary>
    public static int SelectedMemberId
    {
      get
      {
        if (HttpContext.Current.Session[SelectedMemberIdKey] != null)
        {
          return (int)HttpContext.Current.Session[SelectedMemberIdKey];
        }

        ThrowIfSessionExpired();

        return 0;
      }

      set
      {
        HttpContext.Current.Session[SelectedMemberIdKey] = value;
      }
    }

    /// <summary>
    /// Logged in user member name.
    /// </summary>
    public static string MemberName
    {
      get
      {
        if (HttpContext.Current.Session[UserMemberNameKey] != null)
        {

          return HttpContext.Current.Session[UserMemberNameKey] as string;

        }


        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[UserMemberNameKey] = value;
      }
    }

    /// <summary>
    /// Logged in user member name.
    /// </summary>
    public static string InvoiceSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[InvoiceSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[InvoiceSearchCriteriaKey] as string;
        }

        return null;
      }


      set
      {
        HttpContext.Current.Session[InvoiceSearchCriteriaKey] = value;
      }
    }

    public static string SearchType
    {
      get
      {
        if (HttpContext.Current.Session[SearchTypeKey] != null)
        {
          return HttpContext.Current.Session[SearchTypeKey] as string;
        }

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[SearchTypeKey] = value;
      }
    }

    /// <summary>
    /// Property that holds the Form C search url. Required for back button.
    /// </summary>
    public static string FormCSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[FormCSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[FormCSearchCriteriaKey] as string;
        }

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[FormCSearchCriteriaKey] = value;
      }
    }

    /// <summary>
    /// Property that holds the Misc Invoice search URL. Required for back button.
    /// </summary>
    public static string MiscInvoiceSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[MiscInvoiceSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[MiscInvoiceSearchCriteriaKey] as string;
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[MiscInvoiceSearchCriteriaKey] = value;
      }
    }

    /// <summary>
    /// Property that holds the Misc/Uatp Billing Category Invoice search URL. Required for back button.
    /// </summary>
    public static int? BillingCategoryId
    {
      get
      {
        if (HttpContext.Current.Session[BillingCategoryIdKey] != null)
        {
          return HttpContext.Current.Session[BillingCategoryIdKey] as int?;
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[BillingCategoryIdKey] = value;
      }
    }

    public static string PaxInvoiceSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[PaxInvoiceSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[PaxInvoiceSearchCriteriaKey] as string;
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[PaxInvoiceSearchCriteriaKey] = value;
      }
    }
    public static string CGOInvoiceSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[CGOInvoiceSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[CGOInvoiceSearchCriteriaKey] as string;
        }





        return null;
      }

      set
      {
        HttpContext.Current.Session[CGOInvoiceSearchCriteriaKey] = value;
      }
    }

    public static bool IsEmailToSend
    {
      get
      {
        if (HttpContext.Current.Session[ISEmailToSend] != null)
        {
          return (bool)HttpContext.Current.Session[ISEmailToSend];
        }

        return false;
      }

      set
      {
        HttpContext.Current.Session[ISEmailToSend] = value;
      }
    }


    public static bool IsSuperUserCreation
    {
      get
      {
        if (HttpContext.Current.Session[ISSuperUser] != null)
        {
          return (bool)HttpContext.Current.Session[ISSuperUser];
        }
        return false;
      }
      set
      {
        HttpContext.Current.Session[ISSuperUser] = value;
      }
    }

    public static int PageSizeSelected
    {
      get
      {
        if (HttpContext.Current.Session[PageSizeSelectedKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[PageSizeSelectedKey]);
        }

        return 0;
      }

      set
      {
        HttpContext.Current.Session[PageSizeSelectedKey] = value;
      }
    }


    public static string RejectionMemoRecordIds
    {
      get
      {
        if (HttpContext.Current.Session[RejectionMemoRecordIdsKey] != null)
        {
          return Convert.ToString(HttpContext.Current.Session[RejectionMemoRecordIdsKey]);
        }

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[RejectionMemoRecordIdsKey] = value;
      }
    }

    //Added by Ranjit Kumar
    //Select current page index from session for maintain the current page, if user edit the button and after click the back button
    public static int CurrentPageSelected
    {
      get
      {
        if (HttpContext.Current.Session[CurrentPageSelectedKey] != null)
        {
          return Convert.ToInt32(HttpContext.Current.Session[CurrentPageSelectedKey]);
        }

        return 0;
      }

      set
      {
        HttpContext.Current.Session[CurrentPageSelectedKey] = value;
      }
    }
    //Added by Ranjit Kumar
    //Select current JQGrid id from session for maintain the current page, if user edit the button and after click the back button
    public static string CurrentPageGridId
    {
      get
      {
        if (HttpContext.Current.Session[CurrentPageGridIdKey] != null)
        {
          return HttpContext.Current.Session[CurrentPageGridIdKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[CurrentPageGridIdKey] = value;
      }
    }

    public static string MiscCorrSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[CorrSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[CorrSearchCriteriaKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[CorrSearchCriteriaKey] = value;
      }
    }

    public static string PaxCorrSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[PaxCorrSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[PaxCorrSearchCriteriaKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[PaxCorrSearchCriteriaKey] = value;
      }
    }
    public static string CGOCorrSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[CGOCorrSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[CGOCorrSearchCriteriaKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[CGOCorrSearchCriteriaKey] = value;
      }
    }
    public static string CgoCorrTrailSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[CgoCorrTrailSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[CgoCorrTrailSearchCriteriaKey].ToString();
        }
        return null;
      }

      set
      {
        HttpContext.Current.Session[CgoCorrTrailSearchCriteriaKey] = value;
      }
    }
    public static string MiscCorrTrailSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[MiscCorrTrailSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[MiscCorrTrailSearchCriteriaKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[MiscCorrTrailSearchCriteriaKey] = value;
      }
    }
    public static string UatpCorrTrailSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[UatpCorrTrailSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[UatpCorrTrailSearchCriteriaKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[UatpCorrTrailSearchCriteriaKey] = value;
      }
    }
    public static string PaxCorrTrailSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[PaxCorrTrailSearchCriteriaKey] != null)
        {
          return HttpContext.Current.Session[PaxCorrTrailSearchCriteriaKey].ToString();
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[PaxCorrTrailSearchCriteriaKey] = value;
      }
    }
    public static bool UnsavedWarningMessagesEnabled
    {
      get
      {
        if (HttpContext.Current.Session[UnsavedWarningMessagesEnabledKey] != null)
        {
          return (bool)HttpContext.Current.Session[UnsavedWarningMessagesEnabledKey];
        }

        return false;
      }

      set
      {
        HttpContext.Current.Session[UnsavedWarningMessagesEnabledKey] = value;
      }
    }
    public static string[] ManageUserSearchCriteria
    {
      get
      {
        if (HttpContext.Current.Session[ManageUserSearchCriteriaKey] != null)
        {
          return (string[])HttpContext.Current.Session[ManageUserSearchCriteriaKey];
        }

        return null;
      }

      set
      {
        HttpContext.Current.Session[ManageUserSearchCriteriaKey] = value;
      }
    }

    /// <summary>
    /// Username of the logged in user.
    /// </summary>
    public static string TimeZone
    {
      get
      {
        if (HttpContext.Current.Session[TimeZoneKey] != null)
        {
          return HttpContext.Current.Session[TimeZoneKey] as string;
        }

        ThrowIfSessionExpired();

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[TimeZoneKey] = value;
      }
    }
    public static bool IsLoggedIn
    {
      get
      {
        if (HttpContext.Current.Session[UserIdKey] != null)
        {
          return Convert.ToBoolean(HttpContext.Current.Session[IsLoggedInKey]);
        }

        ThrowIfSessionExpired();

        return false;
      }

      set
      {
        HttpContext.Current.Session[IsLoggedInKey] = value;
      }
    }
    public static string NewPassword
    {
      get
      {
        if (HttpContext.Current.Session[NewPasswordKey] != null)
        {
          return Convert.ToString(HttpContext.Current.Session[NewPasswordKey]);
        }

        ThrowIfSessionExpired();

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[NewPasswordKey] = value;
      }
    }



    public static string SessionId
    {
      get
      {
        if (HttpContext.Current.Session[SessionIdKey] != null)
        {
          return Convert.ToString(HttpContext.Current.Session[SessionIdKey]);
        }

        ThrowIfSessionExpired();

        return string.Empty;
      }

      set
      {
        HttpContext.Current.Session[SessionIdKey] = value;
      }
    }

    /// <summary>
      /// Maintain session to get alerts and message announcement on predefined time interval.
      /// </summary>
    public static string AlertRequired
    {
        get
        {
            if (HttpContext.Current.Session[AlertRequiredKey] != null)
            {
                return Convert.ToString(HttpContext.Current.Session[AlertRequiredKey]);
            }

            ThrowIfSessionExpired();

            return string.Empty;
        }

        set
        {
            HttpContext.Current.Session[AlertRequiredKey] = value;
        }
    }

    public static List<AlertsMessagesAnnouncementsResultSet> AlertMessagesData
    {
        get
        {
            if (HttpContext.Current.Session[AlertMessagesDataKey] != null)
            {
                return (List<AlertsMessagesAnnouncementsResultSet>)HttpContext.Current.Session[AlertMessagesDataKey];
            }

            ThrowIfSessionExpired();

            return null;
        }

        set
        {
            HttpContext.Current.Session[AlertMessagesDataKey] = value;
        }
    }

    public static int PaxInvoiceBillingHistoryCount
    {
        get
        {
            if (HttpContext.Current.Session[AlertMessagesDataKey] != null)
            {
                return (int)HttpContext.Current.Session[PaxInvoiceBHRecords];
            }

          return 0;
        }
        set
        {
            HttpContext.Current.Session[PaxInvoiceBHRecords] = value;
        }
    }

    public static string LocationAssociationSearchIds
    {
        get
        {
            if (HttpContext.Current.Session[LocationAssociationSearchIdsKey] != null)
            {
                return HttpContext.Current.Session[LocationAssociationSearchIdsKey] as string;
            }

            return null;
        }

        set
        {
            HttpContext.Current.Session[LocationAssociationSearchIdsKey] = value;
        }
    }

    /// <summary>
    /// CMP685 - User Management : Hold captchastring
    /// </summary>
    public static string CaptchaString
    {
      get
      {
        if (HttpContext.Current.Session[CaptchaStringKey] != null)
        {
          return HttpContext.Current.Session[CaptchaStringKey] as string;
        }
        return string.Empty;
      }
      set
      {
        HttpContext.Current.Session[CaptchaStringKey] = value;
      }
    }

  }
}


