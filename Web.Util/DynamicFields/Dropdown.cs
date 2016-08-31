using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Core.DI;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util.DynamicFields.Base;
using System;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class Dropdown : FieldBase
  {
    private const string _pleaseSelectText = "Please Select";

    public Dropdown(HtmlHelper html)
      : base(html)
    {
      
    }


    /// <summary>
    /// Render html for control of type dropdown.
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <param name="controlId">id for control</param>
    /// <param name="fieldCount">Field instance number in multiple occurrence field</param>
    /// <param name="controlValue">value to be displayed in control</param>
    /// <param name="isTemplate">Is html used in template</param>
    /// <returns></returns>
    public override MvcHtmlString ToMvcHtmlString(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      //Create display test label
      var labelHtml = Label(field).ToHtmlString();
      //Get value to populate in control, applicable in case of edit
      var fieldValue = string.Empty;
      if (!(isTemplate.HasValue && isTemplate.Value))
        fieldValue = GetValue(field, controlValue);

      if (!field.DataSourceId.HasValue)
      {
        return MvcHtmlString.Create("<span>Invalid datasourceId</span>");
      }

      if (!IsViewMode())
      {
        //Get list of values to populate dropdown
        var dataSourceSelectList = new List<SelectListItem>();
        //Added try catch to ressolve error from EF wen DataSourceId is null
        try
        {
          var dataSource = Ioc.Resolve<IMiscInvoiceManager>(typeof(IMiscInvoiceManager)).GetDataSourceValues(field.DataSourceId.Value);
          dataSourceSelectList = dataSource.Select(dataValue => new SelectListItem
          {
            Text = dataValue.Text.Trim(),
            Value = dataValue.Value.Trim(),
            Selected = dataValue.Value.Trim() == fieldValue
          }).Distinct().ToList();
        }
        catch (Exception ex)
        {
        }

        //Get html for textbox
        string dropdownHtml;
        if (_html != null)
          dropdownHtml = GetDropdownHtml(field, groupCount, fieldValue, dataSourceSelectList, controlId, fieldCount);
        else
          dropdownHtml = GetAjaxDropdownHtml(field, groupCount, fieldValue, dataSourceSelectList, controlId, fieldCount);

        var controlHtml = MvcHtmlString.Create(string.Format("{0}{1}", labelHtml, dropdownHtml));
        return controlHtml;
      }
      else
      {
        var controlTextbox = new TextBox(_html);
        return controlTextbox.ToMvcHtmlString(field, groupCount, controlId, fieldCount, fieldValue, isTemplate);
      }
    }

    /// <summary>
    /// Create Html for dropdown using HtmlHelper
    /// </summary>
    /// <param name="field"></param>
    /// <param name="fieldValue"></param>
    /// <param name="dataSourceValueList"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <returns></returns>
    protected string GetDropdownHtml(FieldMetaData field, string groupCount, string fieldValue, List<SelectListItem> dataSourceValueList, string controlId = null, string fieldCount = null)
    {
      //Get html attributes for control
      var fieldHtmlAttributes = GetHtmlAttributes(field);

      return _html.DropDownList(GetControlId(field.FieldName, groupCount, controlId, fieldCount), dataSourceValueList, _pleaseSelectText, fieldHtmlAttributes).ToString();
    }

    /// <summary>
    /// Create html for textbox using TagBuilder, used in ajax call to render html
    /// </summary>
    /// <param name="field"></param>
    /// <param name="fieldValue"></param>
    /// <param name="dataSourceValueList"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <returns></returns>
    protected string GetAjaxDropdownHtml(FieldMetaData field, string groupCount, string fieldValue, List<SelectListItem> dataSourceValueList, string controlId = null, string fieldCount = null)
    {
      //Get html attributes for control
      var htmlAttributes = GetAjaxHtmlAttributes(field);

      //Create tag builder and get html for textbox
      var tag = new TagBuilder(Constants.SelectTagName);
      var dropdownId = GetControlId(field.FieldName, groupCount, controlId, fieldCount);
      tag.MergeAttribute(Constants.IdAttributeName, dropdownId);
      tag.MergeAttribute(Constants.NameAttributeName, dropdownId);

      //Add html attributes
      tag.MergeAttributes(htmlAttributes);

      //Assign value
      //tag.GenerateId(GetControlId(field.FieldName, controlId, fieldCount));  
      var selectOptions = new StringBuilder();     
      //Add item for "please select"
      var option = new TagBuilder(Constants.OptionTagName);
      option.Attributes.Add(Constants.ValueAttributeName, string.Empty);
      option.InnerHtml = _pleaseSelectText;
      selectOptions.AppendLine(option.ToString(TagRenderMode.Normal));
              
      foreach (var item in dataSourceValueList)
      {
        option = new TagBuilder(Constants.OptionTagName);                     
        option.Attributes.Add(Constants.ValueAttributeName, item.Value);                     
        if (fieldValue != null && item.Value == fieldValue)
          option.Attributes.Add(Constants.SelectedAttributeName, Constants.SelectedAttributeValueName);                     
        option.InnerHtml = item.Text;                     
        selectOptions.AppendLine(option.ToString(TagRenderMode.Normal));
      } 
      tag.InnerHtml = selectOptions.ToString();             
      return tag.ToString(TagRenderMode.Normal); 
    }

  }
}
