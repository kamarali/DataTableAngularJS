using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Business.Common;
using Iata.IS.Business.Security;
using Iata.IS.Core.DI;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile.Enums;
using System.Linq;

namespace Iata.IS.Web.Util
{
  public static class MenuHelper
  {
    private static Dictionary<int, bool> _sisMemberSubStatusList = new Dictionary<int, bool>();

    /// <summary>
    /// Spits out a menu item using the specified display text and action parameters.
    /// </summary>
    public static MvcHtmlString MenuItem(this HtmlHelper html, string displayText, string action, string controller, string area = "", bool addSeperator = false, int permissionId = -1, bool notImplemented = false, IList<int> userPermissions = null)
    {
      bool isAuthorized;

      if (userPermissions == null)
      {
        // Use the list of permissions to identify whether the user has the permission required for the menu item.
        isAuthorized = permissionId > 0 ? IsAuthorized(html, permissionId) : true;
      }
      else
      {
        // Fetch the user permissions to check whether the user has the permission required for the menu item.
        isAuthorized = userPermissions.Contains(permissionId);
      }

      if (isAuthorized)
      {
        var cssClass = addSeperator ? "separatorMenuItem" : "menuItem";
        var liString = string.Format("<li class='{0}'>{1}</li>", cssClass, html.ActionLink(displayText, action, controller, new { area }, new { }));

        return MvcHtmlString.Create(liString);
      }

      return MvcHtmlString.Create(string.Empty);
    }

    /// <summary>
    /// Spits out a menu item using the specified display text and url.
    /// </summary>
    public static MvcHtmlString MenuItem(this HtmlHelper html, string displayText, string url, int permissionId = -1)
    {
      var isAuthorized = permissionId > 0 ? IsAuthorized(html, permissionId) : true;
      
      return MvcHtmlString.Create(isAuthorized ? string.Format("<li><a href=\"{0}\">{1}</a></li>", url, displayText) : string.Empty);
    }

    /// <summary>
    /// Spits out a menu item using the specified display text and url.
    /// </summary>
    public static MvcHtmlString Tab(this HtmlHelper html, string tabDisplayText, string actionName, string controllerName, object routeValues, object htmlAttributes, params UserCategory[] allowedUserCategories)
    {
      if (Array.IndexOf(allowedUserCategories, SessionUtil.UserCategory) >= 0)
      {
          //CMP #665: User related enhancement [Sec 2.11: Conditional Display of All Tabs in the IS-WEB Member Profile screen]
          //This change is applicable for ‘Member User’ and A ‘SIS Ops User’ performs proxy login as a ‘Member User’. 
          //This change is not applicable for ‘SIS Ops User’, ‘ICH Ops User’ and ‘ACH Ops User’.
          bool sisMemberSubStatusId = false;
          if (SessionUtil.UserCategory == UserCategory.Member)
          {
              //Check null value then get value.
              var sisMemberProperty = routeValues.GetType().GetProperty("sisMemberSubStatusId");
              if (sisMemberProperty != null)
              {
                  sisMemberSubStatusId = Convert.ToBoolean(sisMemberProperty.GetValue(routeValues, null));
              }

              if (!sisMemberSubStatusId)
              {
                  var liString = string.Format("<li class=\"ui-tabs-container\">{0}</li>", html.ActionLink(tabDisplayText, actionName, controllerName, routeValues, htmlAttributes));
                  return MvcHtmlString.Create(liString);
              }
          }
          else
          {
              var liString = string.Format("<li class=\"ui-tabs-container\">{0}</li>", html.ActionLink(tabDisplayText, actionName, controllerName, routeValues, htmlAttributes));
              return MvcHtmlString.Create(liString);
          }
      }

      return MvcHtmlString.Create(string.Empty);
    }

    public static bool IsAuthorized(this HtmlHelper html, int permissionId, IList<int> userPermissions = null)
    {
      // If a list of user permissions are returned, then we can simply check whether the permission is present in that list.
      if (userPermissions != null)
      {
        return userPermissions.Contains(permissionId);
      }

      // Check in the database whether the currently logged in user has the permission in question.
      var authorizationManager = Ioc.Resolve<IAuthorizationManager>();

      return authorizationManager.IsAuthorized(SessionUtil.UserId, permissionId);
    }

  }
}
