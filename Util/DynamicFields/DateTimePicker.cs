using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util.DynamicFields.Base;
using System.Globalization;

namespace Iata.IS.Web.Util.DynamicFields
{
  class DateTimePicker : TextBox
  {
    public DateTimePicker(HtmlHelper html)
      : base(html)
    {
      
    }

    /// <summary>
    /// Creates datepicker control for dynamic field
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

      //Get html for datepicker
      string datePickerHtml;
      if (_html != null)
        datePickerHtml = GetTextboxHtml(field, groupCount, fieldValue, controlId, fieldCount);
      else
        datePickerHtml = GetAjaxTextboxHtml(field, groupCount, fieldValue, controlId, fieldCount);

      var controlHtml = MvcHtmlString.Create(string.Format("{0}{1}", labelHtml, datePickerHtml));

      return controlHtml;
    }
    
    /// <summary>
    /// Converts string value for DatePicker 
    /// </summary>
    /// <param name="field">Field metadata for which value is to be populated</param>
    /// <param name="controlValue">Value to be populated in datepicker</param>
    /// <returns></returns>
    protected new string GetValue(FieldMetaData field, string controlValue = null)
    {
      try
      {
        //If control Value is passed populate it in datepicker, else display value at 0th position in FieldValue list
        var selectedValue = string.Empty;
        if (controlValue != null)
          selectedValue = controlValue;
        else if (field.FieldValues.Count > 0 && field.FieldValues[0] != null)
        {
          selectedValue = field.FieldValues[0].Value;
        }
        if(!string.IsNullOrEmpty(selectedValue))
        {
          DateTime dtValue;
          bool isParsed = DateTime.TryParseExact(selectedValue, FormatConstants.DynamicFieldDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtValue);
          if(isParsed)
            return dtValue.ToString(FormatConstants.DynamicFieldDateTimeFormat);

          isParsed = DateTime.TryParseExact(selectedValue, FormatConstants.ISXmlDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtValue);
          if (isParsed)
            return dtValue.ToString(FormatConstants.DynamicFieldDateTimeFormat);

          isParsed = DateTime.TryParseExact(selectedValue, FormatConstants.ISXmlDateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dtValue);
          if (isParsed)
            return dtValue.ToString(FormatConstants.DynamicFieldDateTimeFormat);
        }
      }
      catch (Exception)
      {
        
      }
      return string.Empty;
    }
  }
}
