using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class Checkbox : FieldBase
  {
    public Checkbox(HtmlHelper html)
      : base(html)
    {
      
    }

    /// <summary>
    /// Creates checkbox control for dynamic field
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
      var fieldValue = false;
      if (!(isTemplate.HasValue && isTemplate.Value))
        fieldValue = GetValue(field, controlValue);

      var controlHtml = MvcHtmlString.Create(string.Format("{0}{1}", labelHtml, _html.CheckBox(GetControlId(field.FieldName, groupCount, controlId, fieldCount), fieldValue)));
      return controlHtml;
    }

    /// <summary>
    /// Converts string value to boolean
    /// </summary>
    /// <param name="field">Field metadata for which value is to be displayed</param>
    /// <param name="controlValue">Value for control</param>
    /// <returns></returns>
    protected new Boolean GetValue(FieldMetaData field, string controlValue = null)
    {
      var fieldValue = false;
      if (controlValue == null)
      {
        if (field.FieldValues != null && field.FieldValues.Count > 0)
        {
          fieldValue = Convert.ToBoolean(field.FieldValues[0].Value);
        }
      }
      else
      {
        fieldValue = Convert.ToBoolean(controlValue);
      }
      return fieldValue;
    }
  }
}
