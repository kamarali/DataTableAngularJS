using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;

namespace Iata.IS.Web.Util.DynamicFields.Base
{
  public abstract class FieldBase
  {
    protected HtmlHelper _html;

    protected FieldBase(HtmlHelper html)
    {
      _html = html;
    }

    /// <summary>
    /// Create label for display text
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <returns></returns>
    protected static MvcHtmlString Label(FieldMetaData field)
    {
      var tagBuilder = new TagBuilder("label");
      tagBuilder.MergeAttribute("for", field.FieldName, true);

      string innerText = string.Empty;
      if (field.FieldType != FieldType.Attribute)
      {
        if (field.RequiredType == RequiredType.Mandatory)
        {
          innerText = "<span>*</span>";
        }
        else if (field.RequiredType == RequiredType.Recommended)
        {
          innerText = "<span style=\"color:Red\">+</span>";
        }
      }
      tagBuilder.InnerHtml = string.Format("{0}{1}:", innerText, field.DisplayText);

      return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
    }

    /// <summary>
    /// Create htmlAttribute object using values stored in metadata
    /// </summary>
    protected object GetHtmlAttributes(FieldMetaData field)
    {
      string cssClass = field.CssClass;
      switch (field.DataType)
      {
        case DataType.Alphabetic:
          cssClass += "alphabetsOnly";
          break;
        //Commented code to allow special characters for AN datatype fields, allow ascii range 32 to 126
        //case DataType.AlphaNumeric:
        //  cssClass += "alphaNumericWithSpace";
        //  break;
        case DataType.Numeric:
          cssClass += "number";
          break;
      }
      if (field.FieldName == Constants.CountryCodeFieldName)
      {
        cssClass += " "+Constants.CountryCodeFieldName;
      }
      if(field.FieldName == Constants.SubDivisionCodeFieldName)
      {
        cssClass += " "+Constants.SubDivisionCodeFieldName;
      }
      if (field.ControlType == ControlType.Date)
      {
        cssClass += "datePicker";
      }
      else if (field.ControlType == ControlType.DateTime)
      {
        cssClass += "dateTimePicker dynamicFieldDateTimeValue";
      }
      if (field.ControlType == ControlType.AutoComplete || field.ControlType == ControlType.EditableAutoComplete)
      {
        cssClass += "autocComplete";
      }

      if (field.FieldType != FieldType.Attribute)
      {
        switch (field.RequiredType)
        {
          case RequiredType.Mandatory:
            cssClass += " required";
            break;
          case RequiredType.Recommended:
            cssClass += " recommended";
            break;
          case RequiredType.Optional:
            cssClass += " optional";
            break;
        }
      }

      if ((field.ControlType != ControlType.Dropdown && field.ControlType != ControlType.AutoComplete) && !string.IsNullOrEmpty(field.DataLength))
      {
        switch (field.DataType)
        {
          case DataType.AlphaNumeric:
            return new { @class = cssClass, maxLength = field.DataLength };

          case DataType.Alphabetic:
            return new { @class = cssClass, maxLength = field.DataLength };
          
          case DataType.Numeric:
            var dataRange = GetNumberForRange(field.DataLength);
            //Added for dynamic fields data size check, to fix issue If range is 999999999999999.999 and error message is displayed as 1000000000000000
            cssClass += " dynamicFieldRange";
            return new { @class = cssClass, minDataSize = -(dataRange.DataSize), maxDataSize = dataRange.DataSize, dataSizeDecimal = dataRange.AllowedDecimalPlaces, roundTo = dataRange.AllowedDecimalPlaces };
          
          case DataType.PositiveNumber:
            var rangeMax = GetNumberForRange(field.DataLength);
            //Added for dynamic fields data size check, to fix issue If range is 999999999999999.999 and error message is displayed as 1000000000000000
            cssClass += " dynamicFieldRange";
            return new { @class = cssClass, minDataSize = (decimal)0, maxDataSize = rangeMax.DataSize, dataSizeDecimal = rangeMax.AllowedDecimalPlaces };
        }
      }

    

      return new { @class = cssClass, };
    }

    /// <summary>
    /// Create htmlAttribute object using values stroed in metadata
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    protected Dictionary<string, string> GetAjaxHtmlAttributes(FieldMetaData field)
    {
      var htmlAttributes = new Dictionary<string, string>();
      string cssClass = field.CssClass;
      switch (field.DataType)
      {
        case DataType.Alphabetic:
          cssClass += "alphabetsOnly";
          break;
        //Commented code to allow special characters for AN datatype fields, allow ascii range 32 to 126
        //case DataType.AlphaNumeric:
        //  cssClass += "alphaNumericWithSpace";
        //  break;
        case DataType.Numeric:
          cssClass += "number";
          break;
      }
      if (field.FieldName == Constants.CountryCodeFieldName)
      {
        cssClass += " " + Constants.CountryCodeFieldName;
      }
      if (field.FieldName == Constants.SubDivisionCodeFieldName)
      {
        cssClass += " " + Constants.SubDivisionCodeFieldName;
      }
      if (field.ControlType == ControlType.Date)
      {
        cssClass += "datePicker";
      }
      else if (field.ControlType == ControlType.DateTime)
      {
        cssClass += "dateTimePicker dynamicFieldDateTimeValue";
      }
      if (field.ControlType == ControlType.AutoComplete || field.ControlType == ControlType.EditableAutoComplete)
      {
        cssClass += "autocComplete";
      }

      if (field.FieldType != FieldType.Attribute)
      {
        switch (field.RequiredType)
        {
          case RequiredType.Mandatory:
            cssClass += " required";
            break;
          case RequiredType.Recommended:
            cssClass += " recommended";
            break;
          case RequiredType.Optional:
            cssClass += " optional";
            break;
        }
      }
      if ((field.ControlType != ControlType.Dropdown && field.ControlType != ControlType.AutoComplete) && !string.IsNullOrEmpty(field.DataLength))
      {
        switch (field.DataType)
        {
          case DataType.AlphaNumeric:
            htmlAttributes.Add("class", cssClass);
            htmlAttributes.Add("maxLength", field.DataLength);
            break;
          case DataType.Alphabetic:
            htmlAttributes.Add("class", cssClass);
            htmlAttributes.Add("maxLength", field.DataLength);
            break;
          case DataType.Numeric:
            var dataRange = GetNumberForRange(field.DataLength);
            //Added for dynamic fields data size check, to fix issue If range is 999999999999999.999 and error message is displayed as 1000000000000000
            cssClass += " dynamicFieldRange";
            htmlAttributes.Add("class", cssClass);
            htmlAttributes.Add("minDataSize", (-(dataRange.DataSize)).ToString());
            htmlAttributes.Add("maxDataSize", dataRange.DataSize.ToString());
            htmlAttributes.Add("dataSizeDecimal", dataRange.AllowedDecimalPlaces.ToString());
            break;
          case DataType.PositiveNumber:
            var rangeMax = GetNumberForRange(field.DataLength);
            //Added for dynamic fields data size check, to fix issue If range is 999999999999999.999 and error message is displayed as 1000000000000000
            cssClass += " dynamicFieldRange";
            htmlAttributes.Add("class", cssClass);
            htmlAttributes.Add("minDataSize", "0");
            htmlAttributes.Add("maxDataSize", rangeMax.DataSize.ToString());
            htmlAttributes.Add("dataSizeDecimal", rangeMax.AllowedDecimalPlaces.ToString());
            break;
        }
      }
      if (htmlAttributes.Count == 0)
      {
        htmlAttributes.Add("class", cssClass);
      }
      return htmlAttributes;
    }

    /// <summary>
    /// Return decimal number to use as min/max for numeric value validation
    /// </summary>
    /// <param name="dataLength"></param>
    /// <returns></returns>
    private static DFNumericFieldDataSize GetNumberForRange(string dataLength)
    {
      var dfDataSize = new DFNumericFieldDataSize();
      if (!string.IsNullOrEmpty(dataLength))
      {
        if (dataLength.Contains(","))
        {
          // Return decimal value if data length is comma separated value
          var decimalValue = string.Empty;
          string[] length = dataLength.Split(',');

          if (length.Length == 2)
          {
            //number after comma is used as count or digits after decimal 
            int decimalLength = Convert.ToInt32(length[1].Trim());
            dfDataSize.AllowedDecimalPlaces = decimalLength;
            //length of number is data length - count of digits after decimal
            int numberLength = Convert.ToInt32(length[0].Trim()) - Convert.ToInt32(length[1].Trim());
            decimalValue = string.Format("{0}.{1}", new string(Constants.NumericFieldValueRangeNumber, numberLength), new string(Constants.NumericFieldValueRangeNumber, decimalLength));
          }

          dfDataSize.DataSize = Convert.ToDecimal(decimalValue);
        }
        else
        {
          // Return data length
          var numberforRange = new string(Constants.NumericFieldValueRangeNumber, Convert.ToInt32(dataLength.Trim()));
          dfDataSize.DataSize = Convert.ToDecimal(numberforRange);
        }
      }
      else
        dfDataSize.DataSize = 9;

      return dfDataSize; 
    }

    /// <summary>
    /// Render html for control for field metadata object
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <param name="groupCount"></param>
    /// <param name="controlId">id for control</param>
    /// <param name="fieldCount">Field instance number in multiple occurrence field</param>
    /// <param name="controlValue">value to be displayed in control</param>
    /// <param name="isTemplate">Is html used in template</param>
    /// <returns></returns>
    public abstract MvcHtmlString ToMvcHtmlString(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null);

    /// <summary>
    /// Return value to be populated in control
    /// Is controlValue field has value then return it, else return value of first object in FieldValue collection of field metadata
    /// </summary>
    /// <param name="field">Field metadata for which value is to be displayed</param>
    /// <param name="controlValue">Value for control</param>
    /// <returns></returns>
    protected string GetValue(FieldMetaData field, string controlValue = null)
    {
      string fieldValue = string.Empty;
      if (controlValue == null)
      {
        if (field.FieldValues != null && field.FieldValues.Count > 0)
        {
          fieldValue = field.FieldValues[0].Value;
        }
      }
      else
      {
        fieldValue = controlValue;
      }
      return fieldValue;
    }

    /// <summary>
    /// Get html for given field metadata
    /// </summary>
    protected MvcHtmlString GetFieldControl(FieldMetaData field, string groupCount, string actionUrl = null, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      if (this.IsViewMode() && field.FieldName == "FlightDateTime")
      {
        field.ControlType = ControlType.TextBox;
        field.ControlTypeId = (int) ControlType.TextBox;
      }
      var controlTextbox = new TextBox(_html);
      switch (field.ControlType)
      {
        case ControlType.TextBox:
          return controlTextbox.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
        case ControlType.Dropdown:
          var controlDropdown = new Dropdown(_html);
          return controlDropdown.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
        case ControlType.CheckBox:
          var controlCheckbox = new Checkbox(_html);
          return controlCheckbox.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
        case ControlType.Date:
          var controlDatepicker = new Datepicker(_html);
          return controlDatepicker.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
        case ControlType.DateTime:
          var controlDateTimepicker = new DateTimePicker(_html);
          return controlDateTimepicker.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
        case ControlType.AutoComplete:
          var controlAutocomplete = new AutoComplete(_html, actionUrl, false);
          return controlAutocomplete.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
      }
      return controlTextbox.ToMvcHtmlString(field, groupCount, controlId, fieldCount, controlValue, isTemplate);
    }

    /// <summary>
    /// Create image
    /// </summary>
    protected MvcHtmlString Image(bool isPlusImage, string imageId, string imagePath, string imageClass, string clickEvent = null)
    {
      var tagBuilder = new TagBuilder(Constants.ImageTagName);
      tagBuilder.MergeAttribute("id", imageId + Constants.ImageIdSuffix, true);
      string cssClass = string.Format("linkImage {0}", imageClass);
      tagBuilder.MergeAttribute("class", cssClass, true);

      if (isPlusImage)
      {
        tagBuilder.MergeAttribute("name", "plus", true);
        imagePath += "plus.png";
      }
      else
      {
        tagBuilder.MergeAttribute("name", "minus", true);
        imagePath += "minus.png";
      }
      tagBuilder.MergeAttribute("src", imagePath, true);

      if (clickEvent != null)
      {
        tagBuilder.MergeAttribute("onclick", clickEvent);
      }

      return MvcHtmlString.Create(string.Format("<span style=\"display:inline\">{0}</span>", tagBuilder.ToString(TagRenderMode.Normal)));
    }

    /// <summary>
    /// Gte image tag for Expad image
    /// </summary>
    /// <param name="imageId"></param>
    /// <param name="imagePath"></param>
    /// <returns></returns>
    protected MvcHtmlString ExpandImage(string imageId, string imagePath)
    {
      var tagBuilder = new TagBuilder(Constants.ImageTagName);
      tagBuilder.MergeAttribute("id", imageId + Constants.ImageIdSuffix, true);
      tagBuilder.MergeAttribute("class", string.Format("{0}", Constants.ExpandImageCssClassName), true);

      tagBuilder.MergeAttribute("src", imagePath + Constants.ExpandImageName, true);

      tagBuilder.MergeAttribute("onclick", Constants.OptionalFieldsExpandFunction);

      return MvcHtmlString.Create(string.Format("<span style=\"display:inline\">{0}</span>", tagBuilder.ToString(TagRenderMode.Normal)));
    }

    /// <summary>
    /// Return outer div tag
    /// </summary>
    /// <param name="field"></param>
    /// <param name="cssClass"></param>
    /// <param name="divId"></param>
    /// <returns></returns>
    protected string GetOuterDivHtml(FieldMetaData field, string cssClass = null, string divId = null)
    {
      if (field.RequiredType == RequiredType.Optional)
      {
        return string.Format("<div class=\"{0} {1}\" id=\"{2}\">", Constants.OptionalFieldCssClassName, cssClass ?? string.Empty, divId ?? string.Empty) + "{0}</div>";
      }
      
      return string.Format("<div class=\"{0}\" id=\"{1}\">", cssClass ?? string.Empty, divId ?? string.Empty) + "{0}</div>";
    }

    /// <summary>
    /// Get control id for field, used to render value in ModelBinder
    /// </summary>
    protected string GetControlId(string fieldName, string groupCount, string controlId = null, string fieldCount = null)
    {
      return ControlIdConstants.DynamicFieldPrefix + (controlId ?? fieldName) + Constants.DynamicGroupCountString + groupCount + ControlIdConstants.DynamicFieldValueSuffix + (fieldCount ?? string.Empty);
    }

    /// <summary>
    /// Get control id for group id, used to render group html for multiple occurrence group
    /// </summary>
    protected string GetGroupControlId(string controlId, string grpCount = null)
    {
      return ControlIdConstants.DynamicFieldPrefix + controlId + Constants.DynamicGroupIdSuffix + (grpCount ?? string.Empty);
    }

    protected string GetAjaxHiddenHtml(string hiddenControlId, string hiddenControlValue)
    {
      //Create tag builder and get html for hidden field
      var tag = new TagBuilder(Constants.InputTagName);
      tag.MergeAttribute(Constants.TypeAttributeName, Constants.HiddenTypeName);
      tag.MergeAttribute(Constants.IdAttributeName, hiddenControlId);
      tag.MergeAttribute(Constants.NameAttributeName, hiddenControlId);

      //Assign value
      tag.MergeAttribute(Constants.ValueAttributeName, hiddenControlValue);
      return tag.ToString(TagRenderMode.Normal);
    }
    
    /// <summary>
    /// Check if page is open in View mode
    /// </summary>
    /// <returns></returns>
    public bool IsViewMode()
    {
      if (_html != null)
      {
        if (_html.ViewData != null && _html.ViewData.ContainsKey(ViewDataConstants.PageMode))
        {
          return string.Compare(PageMode.View, _html.ViewData[ViewDataConstants.PageMode].ToString(), true) == 0;
        }
      }

      return false;
    }
  }

  /// <summary>
  /// numeric dynamic field data size details
  /// </summary>
  public class DFNumericFieldDataSize
  {
    public decimal DataSize { get; set; }

    public int AllowedDecimalPlaces { get; set; }

  }
}