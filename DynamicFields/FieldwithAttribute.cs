using System.Linq;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class FieldwithAttribute : FieldBase
  {
    private readonly string _actionUrl = string.Empty;
    private readonly bool _generateScriptForMultipleOccurrence = false;
    public bool IsParentMultiOccurrence;
    private string _scriptForSubFields = string.Empty; 
    public string ScriptToInitializeSubFields = string.Empty;
    public string ScriptForMultipleOccurrence = string.Empty;

    public FieldwithAttribute(HtmlHelper html, string actionUrl, bool generateScriptForMultipleOccurrence)
      : base(html)
    {
      _actionUrl = actionUrl;
      _generateScriptForMultipleOccurrence = generateScriptForMultipleOccurrence;
    }

    /// <summary>
    /// Creates controls for dynamic field with Attribute  
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <param name="controlId">id for control</param>
    /// <param name="fieldCount">Field instance number in multiple occurrence field</param>
    /// <param name="controlValue">value to be displayed in control</param>
    /// <param name="isTemplate">Is html used in template</param>
    /// <returns></returns>
    public override MvcHtmlString ToMvcHtmlString(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      var fieldHtml = string.Format("<div>{0}</div>", GetFieldAttributeControl(field, groupCount, controlId ?? field.FieldName, fieldCount, controlValue, isTemplate));

      //Get html for attributes
      var attributeHtml = new StringBuilder();
      foreach (var attribute in field.SubFields.Where(f => f.FieldType == FieldType.Attribute))
      {
        string fieldControlValue = null;
        if (field.FieldValues.Count > 0 && field.FieldValues[0] != null)
        {
          if (field.FieldValues[0].AttributeValues.Select(a => a.FieldMetaDataId).Contains(attribute.Id))
          {
            var attributeValue = field.FieldValues[0].AttributeValues.Single(a => a.FieldMetaDataId == attribute.Id);
            if (attributeValue != null)
              fieldControlValue = attributeValue.Value;
          }
          else 
            fieldControlValue = string.Empty;
        }

        attributeHtml.AppendFormat("<div>{0}</div>", GetFieldAttributeControl(attribute, groupCount, attribute.ParentId + "_" + attribute.Id.ToString(), fieldCount, fieldControlValue, isTemplate));
      }

      if (IsParentMultiOccurrence)
        return MvcHtmlString.Create(string.Format("{0}{1}", fieldHtml, attributeHtml));
      else
      {
        var cssClassName = GetFieldwithAttributeCssClassName(field);
        return MvcHtmlString.Create(string.Format("<div class=\"{0}\">{1}{2}</div>", cssClassName, fieldHtml, attributeHtml));
      }

    }

    /// <summary>
    /// Generate template for field for group/ multiple occ fields. This template is added in template of Group/ multiple occ field
    /// </summary>
    /// <param name="field"></param>
    /// <param name="scriptforField"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <returns></returns>
    public string GetTemplateHtmlForGroup(FieldMetaData field, string groupCount, out string scriptforField, string controlId = null, string fieldCount = null, string controlValue = null)
    {
      var fieldHtml = string.Format("<div>{0}</div>", GetFieldAttributeControl(field, groupCount, controlId ?? field.FieldName, fieldCount, controlValue, true));

      //Get html for attributes
      var attributeHtml = new StringBuilder();
      foreach (var attribute in field.SubFields.Where(f => f.FieldType == FieldType.Attribute))
      {
        attributeHtml.AppendFormat("<div>{0}</div>", GetFieldAttributeControl(attribute, groupCount, attribute.ParentId.ToString() + "_" + attribute.Id.ToString(), fieldCount, null, true));
      }

      var controlHtml = string.Empty;
      if (IsParentMultiOccurrence)
        controlHtml = string.Format("{0}{1}", fieldHtml, attributeHtml);
      else
      {
        var cssClassName = GetFieldwithAttributeCssClassName(field);
        controlHtml = string.Format(GetOuterDivHtml(field, cssClassName), fieldHtml + attributeHtml);
      }

      scriptforField = _scriptForSubFields;
      return controlHtml;
    }

    /// <summary>
    /// Get html for field which is displayed on UI
    /// </summary>
    /// <param name="field"></param>
    /// <param name="scriptforField"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <returns></returns>
    public string GetDisplayHtml(FieldMetaData field, string groupCount, out string scriptforField, string controlId = null, string fieldCount = null, string controlValue = null)
    {
      //Get html for field
      var fieldHtml = string.Format("<div>{0}</div>", GetFieldAttributeControl(field, groupCount, controlId, fieldCount, controlValue));

      //Get html for attributes
      var attributeHtml = new StringBuilder();
      foreach (var attribute in field.SubFields.Where(f => f.FieldType == FieldType.Attribute))
      {
        string fieldControlValue = null;
        if (field.FieldValues.Count > 0 && field.FieldValues[0] != null)
        {
          //Get value to be populated in attribute control for given fieldValue object 
          if (field.FieldValues[0].AttributeValues.Select(a => a.FieldMetaDataId).Contains(attribute.Id))
          {
            var attributeValue = field.FieldValues[0].AttributeValues.Single(a => a.FieldMetaDataId == attribute.Id);
            if (attributeValue != null)
              fieldControlValue = attributeValue.Value;
          }
          else
            fieldControlValue = string.Empty;
        }
        else
        {
          fieldControlValue = string.Empty;
        }
        attributeHtml.AppendFormat("<div>{0}</div>", GetFieldAttributeControl(attribute, groupCount, attribute.ParentId.ToString() + "_" + attribute.Id.ToString(), fieldCount, fieldControlValue));
      }

      scriptforField = _scriptForSubFields;
      if (IsParentMultiOccurrence)
        return string.Format("{0}{1}", fieldHtml, attributeHtml);
      else
      {
        var cssClassName = GetFieldwithAttributeCssClassName(field);
        return string.Format(GetOuterDivHtml(field, cssClassName), fieldHtml + attributeHtml);
      }
    }

    /// <summary>
    /// Get html and scripts for field metadata
    /// </summary>
    /// <param name="field"></param>
    /// <param name="actionUrl"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    protected MvcHtmlString GetFieldAttributeControl(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      if (field.ControlType == ControlType.AutoComplete || field.ControlType == ControlType.EditableAutoComplete)
      {
        var generateMultipleOccScript = false;
        if (isTemplate.HasValue && isTemplate.Value)
          generateMultipleOccScript = true;
        var controlAutoComplete = new AutoComplete(_html, _actionUrl, generateMultipleOccScript);
        if (field.ControlType == ControlType.EditableAutoComplete)
          controlAutoComplete = new EditableAutoComplete(_html, _actionUrl, generateMultipleOccScript);
        string scriptForField;
        //Get html for autocomplete field
        var fieldHtml = controlAutoComplete.GetHtml(field, groupCount, out scriptForField, controlId, fieldCount, controlValue, isTemplate);

        //Script to initialise autocomplete is added in scriptforSubFields
        _scriptForSubFields += scriptForField;
        ScriptToInitializeSubFields += controlAutoComplete.ScriptToInitializeAutocomplete;

        //If parent of this field is of type multiple occurrence, script for subFields to be added in script to clone multi occ parent
        if (generateMultipleOccScript)
          ScriptForMultipleOccurrence += controlAutoComplete.ScriptForMultipleOccurrence;
        return MvcHtmlString.Create(fieldHtml);
      }
      else
      {
        return GetFieldControl(field, groupCount, _actionUrl, controlId, fieldCount, controlValue, isTemplate);
      }
    }

    /// <summary>
    /// Get css class name for field with attribute. If field has only one attribute, then stylesheet is different to render control with more width so that data
    /// is properly displayed. Id field has more than 1 attribute then css class is different which has less width for controls
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string GetFieldwithAttributeCssClassName(FieldMetaData field)
    {
      if (field.SubFields.Where(child => child.FieldType == FieldType.Attribute).Count() == 1)
        return Constants.DynamicFieldWithOneAttributeCssName;
      else
        return Constants.DynamicFieldWithAttributeCssName;
    }
  }
}
