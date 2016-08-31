using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class ProfileFieldsHelper
  {
    /// <summary>
    /// Defines the mode in which the profile field is to be displayed.
    /// </summary>
    private enum DisplayMode
    {
      None = 0,
      ReadOnly = 1,
      ReadWrite = 2
    }

    /// <summary>
    /// Renders a profile field using the effective permissions for the specified user category.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="html">Html helper instance</param>
    /// <param name="expression"></param>
    /// <param name="label">The label string to be displayed</param>
    /// <param name="userCategory">The category of the user for whom the profile is to be rendered.</param>
    /// <param name="fieldHtmlAttributes">The field level html attributes.</param>
    /// <param name="fieldContainerHtmlAttributes">The container level html attributes.</param>
    /// <param name="futureUpdate">The future update object instance used for rendering the future update dialog.</param>
    /// <param name="labelAttributes">html Attributes for label corresponding to control</param>
    /// <param name="skipContainer">if true, do not render div tab for control</param>
    public static MvcHtmlString ProfileFieldFor<TModel, TValue>(this HtmlHelper<TModel> html,
                                                                Expression<Func<TModel, TValue>> expression,
                                                                string label,
                                                                UserCategory userCategory,
                                                                IDictionary<string, object> fieldHtmlAttributes = null,
                                                                IDictionary<string, object> fieldContainerHtmlAttributes = null,
                                                                FutureUpdate futureUpdate = null,
                                                                IDictionary<string, object> labelAttributes = null,
                                                                bool skipContainer = false,
                                                                int  profileMemberId=0)
    {
      var builder = new StringBuilder();
      var userAccess = MapUserCategoryToAccessFlag(userCategory);
      var actionName = html.ViewContext.RouteData.Values["action"].ToString();
      const string  CGOAction = "cgo";
      const string MISCAction = "misc";
      const string UATPAction = "uatp";
     
      
      if (!(expression.Body is MemberExpression))
      {
        return MvcHtmlString.Create("<div>expression is not MemberExpression.</div>");
      }
      
      // Retrieve the Name of the property represented by the expression.
      var propertyName = ((expression.Body as MemberExpression).Member).Name;

      // Retrieve list of profile permission attributes.
      var attributes = Attribute.GetCustomAttributes((expression.Body as MemberExpression).Member).AsQueryable().Where(a => a is ProfilePermissionAttribute).ToList();

      // The profile permission attribute was either not found or multiple permission attributes defined.
      if (attributes.Count != 1)
      {
        return MvcHtmlString.Create("<div>permission attribute undefined.</div>");
      }

      var permissionAttribute = attributes[0] as ProfilePermissionAttribute;

      // Ensure that we have a valid permission attribute to work with.
      if (permissionAttribute == null)
      {
        return MvcHtmlString.Create("<div>permission attribute undefined.</div>");
      }

      // Check if it is a future field, but the future field details are not provided.
      if (permissionAttribute.IsFutureField && futureUpdate == null)
      {
        return MvcHtmlString.Create("<div>future field but details not provided.</div>");
      }

      // Get the display mode for this field.
      var displayMode = CalculateEffectivePermission(userAccess, permissionAttribute.ReadAccessFlags, permissionAttribute.WriteAccessFlags);
      var editClass = string.Empty;

      //CMP-689-Flexible CH Activation Options
      // change the displaymode to readonly for field IchMembershipStatus in special case (extended case 19,20 of 16,17)
      if (futureUpdate != null && (futureUpdate.FieldType == 19 || futureUpdate.FieldType == 20))
      {
        displayMode = DisplayMode.ReadOnly;
      }

      // Create the dictionary if none was sent.
      fieldHtmlAttributes = fieldHtmlAttributes ?? new Dictionary<string, object>();

      switch (displayMode)
      {
        case DisplayMode.ReadOnly:
          if (!fieldHtmlAttributes.ContainsKey("disabled"))
          {
            fieldHtmlAttributes.Add("disabled", "disabled");
          }

          if (permissionAttribute.ControlType == ControlType.TextBox)
          {
              fieldHtmlAttributes.Remove("disabled");
              if (!fieldHtmlAttributes.ContainsKey("readOnly"))
              {
                  fieldHtmlAttributes.Add("readOnly", "true");    
              }
              
          }
         
          break;
      }

      // Check if field is not to be displayed.
      if (displayMode == DisplayMode.None)
      {
        return MvcHtmlString.Create(string.Empty);
      }

      // Prepare the attributes for the div.
      var containerAttributes = fieldContainerHtmlAttributes != null ? ConvertAttributesToString(fieldContainerHtmlAttributes) : string.Empty;
      var labelContainerAttributes = labelAttributes != null ? ConvertAttributesToString(labelAttributes) : string.Empty;

      // Prepare the label string with an asterix if the field is mandatory.
      if (!skipContainer)
      {
        builder.AppendFormat("<div{2}><label{3}>{0}{1}:</label>", permissionAttribute.IsMandatory ? "<span>* </span>" : string.Empty, label, containerAttributes, labelContainerAttributes);
      }

      var fieldValue = ModelMetadata.FromLambdaExpression(expression, html.ViewData).Model;

      switch (permissionAttribute.ControlType)
      {
        case ControlType.TextBox:
          builder.Append(html.TextBoxFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.TextArea:
          builder.Append(html.TextAreaFor(expression, fieldHtmlAttributes).ToHtmlString());

          //SCP ID 273704 - Problem in Member Profile Change html:-Without Changing COMMENT field on MEMBERDETAILS Tab 
          //Email attachment was sent because of it appends \r\n at the start of fieldValue
          if (!ReferenceEquals(fieldValue, null))
            builder.Remove(builder.ToString().IndexOf("&#13;&#10;"), 10);
          else
            builder.Replace("&#13;&#10;", "");
          break;
          
        case ControlType.CheckBox:
          builder.Append(html.CheckBoxFor(expression as Expression<Func<TModel, bool>>, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.SimpleTextBox:
          if (fieldValue != null)
          {
            var dateTimeFieldVal = Convert.ToDateTime(fieldValue);
            fieldValue = dateTimeFieldVal.ToString(FormatConstants.PeriodFormat);
          }
        // check if current action is cgo then append hidden field control for disable fields
        if (actionName.ToLower() == CGOAction || actionName.ToLower() == MISCAction || actionName.ToLower() == UATPAction)
        {
            builder.Append(html.TextBox(propertyName, fieldValue, fieldHtmlAttributes).ToHtmlString() +
                            html.Hidden(propertyName, fieldValue).ToHtmlString());
        }
        else
        {
            builder.Append(html.TextBox(propertyName, fieldValue, fieldHtmlAttributes).ToHtmlString());
        }
        break;

        case ControlType.DatePicker:
          if (fieldValue != null)
          {
            var dateTimeFieldVal = Convert.ToDateTime(fieldValue);
            fieldValue = dateTimeFieldVal.ToString(FormatConstants.DateFormat);
          }
          // check if current action is cgo then append hidden field control for disable fields
          if (actionName.ToLower() == CGOAction || actionName.ToLower() == MISCAction || actionName.ToLower() == UATPAction)
          {
              builder.Append(html.TextBox(propertyName, fieldValue, fieldHtmlAttributes).ToHtmlString() + html.Hidden(propertyName, fieldValue).ToHtmlString());
          }
          else
          {
              builder.Append(html.TextBox(propertyName, fieldValue, fieldHtmlAttributes).ToHtmlString());
          }
         
          break;

        case ControlType.MemberStatusDropdown:
          builder.Append(html.MembershipStatusDropdownList(propertyName, futureUpdate.CurrentValue, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.MemberSubStatusDropdown:
          builder.Append(html.MembershipSubStatusDropdownList(propertyName, string.Empty, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.CountryDropdown:
          builder.Append(futureUpdate != null
                           ? html.CountryCodeDropdownList(futureUpdate.FieldName, futureUpdate.CurrentValue, fieldHtmlAttributes).ToHtmlString()
                           : html.CountryCodeDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.CurrencyDropdown:
          if (futureUpdate != null)
          {
            builder.Append(html.CurrencyDropdownList(futureUpdate.FieldName, futureUpdate.CurrentValue, fieldHtmlAttributes).ToHtmlString());
          }
          else
          {
            builder.Append(html.CurrencyDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          }
          break;

        case ControlType.SalutationDropdown:
          builder.Append(html.SalutionDropdownList(propertyName, string.Empty, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.LocationDropdown:
          builder.Append(html.LocationIdDropdownList(propertyName, string.Empty,profileMemberId, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.MigrationStatusDropdown:
          builder.Append(html.MigrationStatusDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.IchMemberShipStatusDropdown:
          builder.Append(html.IchMembershipStatusDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.SuspensionDropdown:
          // Get value of field using expression passed
          DateTime? suspensionPeriod = null;

          if (fieldValue != null)
          {
            suspensionPeriod = Convert.ToDateTime(fieldValue);
          }
          builder.Append(html.SuspensionDropdown(propertyName, suspensionPeriod, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.DefaultSuspensionDropdown:
          // Get value of field using expression passed
          DateTime? defaultSuspensionPeriod = null;

          if (fieldValue != null)
          {
            defaultSuspensionPeriod = Convert.ToDateTime(fieldValue);
          }
          builder.Append(html.DefaultSuspensionDropdown(propertyName, defaultSuspensionPeriod, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.IchZoneDropdown:
          builder.Append(html.IchZoneDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.IchCategoryDropdown:
          builder.Append(html.IchCategoryDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.AggregatedTypeDropdown:
          builder.Append(html.AggregatedTypeDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.ICHWebReportOptionsDropdown:
          builder.Append(html.IchWebReportOptionsDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.AchCategoryDropdown:
          builder.Append(html.AchCategoryDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.RejectionOnValidatonFailureDropdown:
          builder.Append(html.RejectionOnValidatonFailureDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.SamplingCareerTypeDropdown:
          builder.Append(html.SamplingCareerTypeDropdownListFor(expression, fieldHtmlAttributes).ToHtmlString());
          break;

        case ControlType.OutputFileDeliveryDropdownList:
          builder.Append(html.OutputFileDeliveryDropdownListFor(expression, fieldHtmlAttributes));
          break;
      }

      if (permissionAttribute.IsFutureField)
      {
        var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
        BillingPeriod currentBillingPeriod = new BillingPeriod();

        // If field is editable for logged in user only then 'Edit' link will be displayed
        if (displayMode == DisplayMode.ReadWrite)
        {
          //Get current billing period value set in membercontroller.This value will be used in validating future period entered while setting future update values
          if (html.ViewData != null && html.ViewData.ContainsKey(ViewDataConstants.CurrentBillingPeriod))
          {
            currentBillingPeriod= (BillingPeriod) html.ViewData[ViewDataConstants.CurrentBillingPeriod];
          }

          /*SCP101407: FW: XML Validation Failure for 450-9B - SIS Production
            Description: Zone and Category are made immediate update fields for first edit now. Redirection after creating new member is stopped.
            This code below was enhanced to have id attribute for anchor tag Edit, Id is furter used to hide the future update link.
           */
          string postpendIdwith = futureUpdate != null && !string.IsNullOrEmpty(futureUpdate.FieldId)
                                        ? futureUpdate.FieldId
                                        : "";

          builder.AppendFormat(" <a id=FutureEditLinkFor_" + postpendIdwith + " class=\"{0} hidden ignoredirty\" onclick=\"return popupFutureUpdateDialog('#{1}', {2}, {3}, _calendarIcon,{4});\" href=\"#\">Edit...</a>",
                                 ((futureUpdate.FieldType == 6) || (futureUpdate.FieldType == 7)) ? "" : futureUpdate.EditLinkClass,
                                 futureUpdate.FieldId,
                                 futureUpdate.FieldType,
                                 futureUpdate.HasFuturePeriod ? 1 : 0, futureUpdate.IsFieldMandatory ? 1 : 0);
          
        }
        builder.Append(html.Hidden(futureUpdate.FieldName + "Old", futureUpdate.CurrentValue));
        builder.Append(html.Hidden(futureUpdate.FieldName + "FutureValue", futureUpdate.FutureValue));
        builder.Append(html.Hidden(futureUpdate.FieldName + "DisplayValue", futureUpdate.CurrentDisplayValue));
        builder.Append(html.Hidden(futureUpdate.FieldName + "FutureDisplayValue", futureUpdate.FutureDisplayValue));
        builder.Append(futureUpdate.HasFuturePeriod
                         ? html.Hidden(futureUpdate.FieldName + "FuturePeriod", futureUpdate.FuturePeriod)
                         : html.Hidden(futureUpdate.FieldId + "FutureDate", futureUpdate.FutureDate));
        //builder.AppendFormat(" <img class='{3}' id='{0}FutureDateInd' src='{5}' onclick='return displayFutureUpdateDetails(\"#{1}\", {4}, {2});' Title='Click to see future value' />",
        //                     futureUpdate.FieldId,
        //                     futureUpdate.FieldId,
        //                     futureUpdate.FieldType,
        //                     string.IsNullOrEmpty(futureUpdate.FutureValue) ? "hidden" : string.Empty,
        //                     futureUpdate.HasFuturePeriod ? 1 : 0,
        //                     urlHelper.Content("~/Content/Images/Exclamation.gif"));
       
        //SCP221813 - Auto Billing issue.
        builder.AppendFormat(" <img class='{3}' id='{0}FutureDateInd' src='{5}' onclick='return displayFutureUpdateDetails(\"#{1}\", {4}, {2});' Title='Click to see future value' />",
                            futureUpdate.FieldId,
                            futureUpdate.FieldId,
                            futureUpdate.FieldType,
                            string.IsNullOrEmpty(futureUpdate.FuturePeriod) && string.IsNullOrEmpty(futureUpdate.FutureDate) || (futureUpdate.FieldType == 14 && futureUpdate.CurrentValue == futureUpdate.FutureValue) ? "hidden" : string.Empty,
                            futureUpdate.HasFuturePeriod ? 1 : 0,
                            urlHelper.Content("~/Content/Images/Exclamation.gif"));
      }

      if (!skipContainer)
      {
        builder.AppendFormat("</div>");
      }

      return MvcHtmlString.Create(builder.ToString());
    }

    /// <summary>
    /// Renders a Checkbox for the reference data using accountId of the all four billing categories.
    /// </summary>
    /// <param name="html">The helper instance</param>
    /// <param name="controlName">Name of the control</param>
    /// <param name="controlValue">Value Yes/No of the control</param>
    /// <param name="accountId">Account Id of any billing category</param>
    /// <returns></returns>
    public static MvcHtmlString MemProfileDataChkBox(this HtmlHelper html, string controlName, bool controlValue, string accountId)
    {
      TagBuilder checkbox = new TagBuilder("input");
      checkbox.Attributes.Add("name", controlName);
      checkbox.Attributes.Add("id", controlName);
      checkbox.Attributes.Add("type", "checkbox");
      if (controlValue)
        checkbox.Attributes.Add("checked", "checked");

      checkbox.Attributes.Add("value", "true");
      if (string.IsNullOrEmpty(accountId))
      {
        checkbox.Attributes.Add("disabled", "disabled");
      }
      //TagBuilder hidden = new TagBuilder("input");
      //hidden.MergeAttribute("name", controlName);
      //hidden.MergeAttribute("type", "hidden");
      //hidden.MergeAttribute("value", "false");

      return MvcHtmlString.Create(checkbox.ToString(TagRenderMode.Normal));
    }
     
    /// <summary>
    /// Renders a profile field using the effective permissions for the specified user category.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="label">The label string to be displayed</param>
    /// <param name="userCategory">The category of the user for whom the profile is to be rendered.</param>
    /// <param name="controlType"></param>
    /// <param name="fieldHtmlAttributes">The field level html attributes.</param>
    /// <param name="fieldContainerHtmlAttributes">The container level html attributes.</param>
    /// <param name="futureUpdate">The future update object instance used for rendering the future update dialog.</param>
    /// <param name="labelAttributes">html Attributes for label corresponding to control</param>
    /// <param name="skipContainer">if true, do not render div tab for control</param>
    /// <param name="isFutureField"></param>
    public static MvcHtmlString ProfileField(this HtmlHelper html,
                                             string controlName,
                                             string label,
                                             UserCategory userCategory,
                                             ControlType controlType,
                                             IDictionary<string, object> fieldHtmlAttributes = null,
                                             IDictionary<string, object> fieldContainerHtmlAttributes = null,
                                             FutureUpdate futureUpdate = null,
                                             IDictionary<string, object> labelAttributes = null,
                                             bool skipContainer = false,
                                             bool isFutureField = false)
    {
      var builder = new StringBuilder();

      // Create the dictionary if none was sent.
      fieldHtmlAttributes = fieldHtmlAttributes ?? new Dictionary<string, object>();

      // Prepare the attributes for the div.
      var containerAttributes = fieldContainerHtmlAttributes != null ? ConvertAttributesToString(fieldContainerHtmlAttributes) : string.Empty;
      var labelContainerAttributes = labelAttributes != null ? ConvertAttributesToString(labelAttributes) : string.Empty;

      // Prepare the label string with an asterix if the field is mandatory.
      if (!skipContainer)
      {
        builder.AppendFormat("<div{1}><label{2}>{0}:</label>", label, containerAttributes, labelContainerAttributes);
      }

      switch (controlType)
      {
        case ControlType.CheckBox:
          builder.Append(html.CheckBox(controlName, fieldHtmlAttributes).ToHtmlString());
          break;
      }

      if (isFutureField)
      {
        var urlHelper = new UrlHelper(html.ViewContext.RequestContext);

        // If field is editable for logged in user only then 'Edit' link will be displayed

        builder.AppendFormat(" <a class=\"{0} hidden ignoredirty\" id=\"#{1}_edit\"  onclick=\"return popupFutureUpdateDialog('#{1}', {2}, {3}, _calendarIcon);\" href=\"#\">Edit...</a>",
                               ((futureUpdate.FieldType == 6) || (futureUpdate.FieldType == 7)) ? "" : futureUpdate.EditLinkClass,
                               futureUpdate.FieldId,
                               futureUpdate.FieldType,
                               futureUpdate.HasFuturePeriod ? 1 : 0);
        
        builder.Append(html.Hidden(futureUpdate.FieldName + "Old", futureUpdate.CurrentValue));
        builder.Append(html.Hidden(futureUpdate.FieldName + "FutureValue", futureUpdate.FutureValue));
        builder.Append(html.Hidden(futureUpdate.FieldName + "DisplayValue", futureUpdate.CurrentDisplayValue));
        builder.Append(html.Hidden(futureUpdate.FieldName + "FutureDisplayValue", futureUpdate.FutureDisplayValue));
        builder.Append(futureUpdate.HasFuturePeriod
                         ? html.Hidden(futureUpdate.FieldName + "FuturePeriod", futureUpdate.FuturePeriod)
                         : html.Hidden(futureUpdate.FieldId + "FutureDate", futureUpdate.FutureDate));
        builder.AppendFormat(" <img class='{3}' id='{0}FutureDateInd' src='{5}' onclick='return displayFutureUpdateDetails(\"#{1}\", {4}, {2});' Title='Click to see future value'/>",
                             futureUpdate.FieldId,
                             futureUpdate.FieldId,
                             futureUpdate.FieldType,
                             string.IsNullOrEmpty(futureUpdate.FutureValue) ? "hidden" : string.Empty,
                             futureUpdate.HasFuturePeriod ? 1 : 0,
                             urlHelper.Content("~/Content/Images/Exclamation.gif"));
      }

      if (!skipContainer)
      {
        builder.AppendFormat("</div>");
      }
      return MvcHtmlString.Create(builder.ToString());
    }

    /// <summary>
    /// Prepares a profile model instance to be saved to the database - by setting the values of the properties that were not rendered on the UI (so, are null).
    /// </summary>
    /// <typeparam name="T">The type of the profile model</typeparam>
    /// <param name="dbInstance">The instance of the profile model from the database.</param>
    /// <param name="updatedInstance">The instance of the profile model received from the UI.</param>
    /// <param name="userCategory">The user category of the logged in user.</param>
    public static void PrepareProfileModel<T>(T dbInstance, T updatedInstance, UserCategory userCategory) where T : class
    {
      // Get a list of all the properties that are secured - containing a the ProfilePermission attribute.
      var securedProperties = GetSecuredProperties(typeof(T));

      // Convert the user category to the correct access flag.
      var accessFlag = MapUserCategoryToAccessFlag(userCategory);

      foreach (var property in securedProperties)
      {
        // Get the ProfilePermission attribute for the property.
        foreach (ProfilePermissionAttribute permissionAttribute in property.GetCustomAttributes(typeof(ProfilePermissionAttribute), false))
        {
          // Get the effective permission from the user category and the access flags defined in the attribute.
          var effectivePermission = CalculateEffectivePermission(accessFlag, permissionAttribute.ReadAccessFlags, permissionAttribute.WriteAccessFlags);

          // If the property was not at all rendered or was in read only mode (hence not posted)
          if (effectivePermission == DisplayMode.None || effectivePermission == DisplayMode.ReadOnly)
          {
            // Update the property of the model instance received from the UI with the value of property from the database model instance.
            if (dbInstance != null)
            {
              property.SetValue(updatedInstance, property.GetValue(dbInstance, null), null);
            }
          }
        }
      }
    }

    /// <summary>
    /// Returns a list of property info objects for the auditable properties (using reflection).
    /// </summary>
    private static IEnumerable<PropertyInfo> GetSecuredProperties(Type modelType)
    {
      var props = from propertyInfo in modelType.GetProperties()
                  where Attribute.IsDefined(propertyInfo, typeof(ProfilePermissionAttribute))
                  select propertyInfo;

      return props;
    }

    /// <summary>
    /// Get a string representation that converts the container html attributes dictionary to a string that can be applied to the div.
    /// </summary>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    private static string ConvertAttributesToString(IEnumerable<KeyValuePair<string, object>> htmlAttributes)
    {
      var sb = new StringBuilder(" ");

      foreach (var htmlAttribute in htmlAttributes)
      {
        sb.AppendFormat("{0}=\"{1}\"", htmlAttribute.Key, htmlAttribute.Value);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Calculates the display mode for the given user category and the read and write access flags specified on that model property.
    /// </summary>
    /// <param name="userCategory"></param>
    /// <param name="readAccessFlags"></param>
    /// <param name="writeAccessFlags"></param>
    /// <returns></returns>
    private static DisplayMode CalculateEffectivePermission(AccessFlags userCategory, AccessFlags readAccessFlags, AccessFlags writeAccessFlags)
    {
      // Default display mode to None.
      var mode = DisplayMode.None;

      // Check if the specified user category has write access - if not, then check for read access.
      if ((writeAccessFlags & userCategory) == userCategory)
      {
        mode = DisplayMode.ReadWrite;
      }
      else if ((readAccessFlags & userCategory) == userCategory)
      {
        mode = DisplayMode.ReadOnly;
      }

      return mode;
    }

    /// <summary>
    /// Map the user categories defined in the database (UserCategory enum) to the access flags enum (which allows us to do logical and/or operations)
    /// </summary>
    /// <param name="userCategory">User category defined in the database.</param>
    /// <returns>The mapping access flag for the given user category.</returns>
    private static AccessFlags MapUserCategoryToAccessFlag(UserCategory userCategory)
    {
      switch (userCategory)
      {
        case UserCategory.SisOps:
          return AccessFlags.SisOps;

        case UserCategory.IchOps:
          return AccessFlags.IchOps;

        case UserCategory.AchOps:
          return AccessFlags.AchOps;

        case UserCategory.Member:
          return AccessFlags.Member;

        default:
          throw new ArgumentOutOfRangeException("userCategory");
      }
    }
  }
}

