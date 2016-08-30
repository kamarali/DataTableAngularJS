using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Core.DI;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Web.Util.DynamicFields;
using System.Collections.Generic;
using Iata.IS.Model.Enums;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class DynamicFieldHtmlHelper
  {
    private const string _pleaseSelectText = "Please Select";
    /// <summary>
    ///  Creates textbox control for dynamic field and sets its selected value.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// <param name="controlCount"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldTextBox(this HtmlHelper html, FieldMetaData field, string controlId = null)
    {
      var dynamicTextbox = new TextBox(html);    
      return dynamicTextbox.ToMvcHtmlString(field, controlId);
    }

    /// <summary>
    /// Creates dropdown control for dynamic field and sets its selected value.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldDropdown(this HtmlHelper html, FieldMetaData field, string controlId = null)
    {
      var dynamicDropdown = new Dropdown(html);
      return dynamicDropdown.ToMvcHtmlString(field, controlId);
    }

    /// <summary>
    /// Creates datepicker control for dynamic field
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldDatepicker(this HtmlHelper html, FieldMetaData field, string controlId = null)
    {
      var dynamicDatePicker = new Datepicker(html);
      return dynamicDatePicker.ToMvcHtmlString(field, controlId);
    }

    /// <summary>
    /// Creates checkbox control for dynamic field
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldCheckbox(this HtmlHelper html, FieldMetaData field, string controlId = null)
    {
      var dynamicCheckbox = new Checkbox(html);
      return dynamicCheckbox.ToMvcHtmlString(field, controlId);
    }

    /// <summary>
    ///  Creates Autocomplete control for dynamic field and sets its selected value.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// /// <param name="actionUrl"></param>
    /// <param name="controlId"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldAutocomplete(this HtmlHelper html, FieldMetaData field, string actionUrl, string controlId = null)
    {
      var dynamicAutocomplete = new AutoComplete(html, actionUrl, false);
      return dynamicAutocomplete.ToMvcHtmlString(field, controlId);
    }

    /// <summary>
    /// Creates controls for dynamic field with Attribute  
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// /// <param name="actionUrl"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldwithAttribute(this HtmlHelper html, FieldMetaData field, string actionUrl)
    {
      var dynamicFieldwithAttributes = new FieldwithAttribute(html, actionUrl, false);
      return dynamicFieldwithAttributes.ToMvcHtmlString(field, "");
    }

    /// <summary>
    /// Creates controls for dynamic field with multiple occurrence  
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// <param name="actionUrl"></param>
    /// <param name="imagePath"></param>
    /// <param name="templateDivName"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldMultipleOccurrence(this HtmlHelper html, FieldMetaData field, string actionUrl, string imagePath, string templateDivName)
    {
      var dynamicFieldMultiOccurrence = new FieldMultipleOccurrence(html, actionUrl, imagePath, templateDivName);
      return dynamicFieldMultiOccurrence.ToMvcHtmlString(field, string.Empty);
    }

    /// <summary>
    /// Creates controls for dynamic field with multiple occurrence  
    /// </summary>
    /// <param name="html"></param>
    /// <param name="field"></param>
    /// <param name="actionUrl"></param>
    /// <param name="imagePath"></param>
    /// <param name="templateDivName"></param>
    /// <param name="script"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicFieldGroup(this HtmlHelper html, FieldMetaData field, string actionUrl, string imagePath, string templateDivName, out string script)
    {
      var dynamicFieldGroup = new Group(html, actionUrl, imagePath, templateDivName);
      var groupHtml = dynamicFieldGroup.ToMvcHtmlString(field, field.Id.ToString());
      script = dynamicFieldGroup.ScriptForGroup;
      return groupHtml;
    }

    /// <summary>
    /// Get dropdown for optional group
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlId"></param>
    /// <param name="selectedValue"></param>
    /// <param name="chargeCodeId"></param>
    /// <param name="chargeCodeTypeId"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    public static MvcHtmlString DynamicOptionalDropdownList(this HtmlHelper html, string controlId, string selectedValue, int chargeCodeId, int? chargeCodeTypeId, BillingCategoryType billingCategory, object htmlAttributes = null)
    {
      var optionalGroupList = Ioc.Resolve<IMiscUatpInvoiceManager>(typeof(IMiscUatpInvoiceManager)).GetOptionalGroupDetails(chargeCodeId, chargeCodeTypeId);
        //SCP207711:UATP details should not be allowed to be captured for MISC invoices at LID level 
        if(billingCategory == BillingCategoryType.Misc)
        {
            optionalGroupList.Remove(optionalGroupList.FirstOrDefault(f => f.FieldName.ToLower() == "UATPDetails".ToLower()));
        }

      var optionalGroupSelectList = optionalGroupList.Select(group => new SelectListItem
      {
        Text = group.DisplayText,
        Value = group.FieldMetadataId.ToString()
      }).ToList();

      var script = GetScriptForOptionalField(optionalGroupList);
      return MvcHtmlString.Create(string.Format ("{0}\n{1}", html.DropDownList(controlId, optionalGroupSelectList, _pleaseSelectText, htmlAttributes), script));
    }

    /// <summary>
    /// Generate script to initialise array of group details for optional group
    /// </summary>
    /// <param name="optionalGroupList"></param>
    /// <returns></returns>
    private static string GetScriptForOptionalField(List<DynamicGroupDetail> optionalGroupList)
    {
      var sb = new StringBuilder();
      if (optionalGroupList.Count > 0)
      {
        sb.Append("<script type='text/javascript'>");
        sb.Append("$(document).ready(function (){");
        foreach (var group in optionalGroupList)
          sb.AppendFormat("InitializeOptionalDynamicGroupInfo('{0}', {1});", group.FieldMetadataId, group.MaxOccurrence);
        sb.Append("});");
        sb.Append("</script>");
      }
      return sb.ToString();
    }
  }
}
