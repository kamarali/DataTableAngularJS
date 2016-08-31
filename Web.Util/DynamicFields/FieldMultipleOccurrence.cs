using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class FieldMultipleOccurrence : FieldBase
  {
    private readonly string _actionUrl = string.Empty;
    private readonly string _imagePath = string.Empty;
    private readonly string _templateDivName = string.Empty;
    private string _scriptForSubFields = string.Empty;
    private string _scriptForCloneSubFields = string.Empty;
    public string ScriptToInitializeSubFields = string.Empty;
    public bool IsParentMultiOccurrence;

    public FieldMultipleOccurrence(HtmlHelper html, string actionUrl, string imagePath, string templateDivName)
      : base(html)
    {
      _actionUrl = actionUrl;
      _imagePath = imagePath;
      _templateDivName = templateDivName;
    }

    /// <summary>
    /// Creates controls for dynamic field with multiple occurrence  
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <param name="controlId">id for control</param>
    /// <param name="fieldCount">Field instance number in multiple occurrence field</param>
    /// <param name="controlValue">value to be displayed in control</param>
    /// <param name="isTemplate">Is html used in template</param>
    /// <returns></returns>
    public override MvcHtmlString ToMvcHtmlString(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      string scriptForControls;

      var controlHtml = GetDisplayHtml(field, null, out scriptForControls, controlId, fieldCount, controlValue);

      return MvcHtmlString.Create(string.Format("{0}\n{1}\n{2}", controlHtml, scriptForControls, _scriptForSubFields));
    
    }

    /// <summary>
    /// Get html for field which is displayed on UI
    /// </summary>
    /// <param name="field"></param>
    /// <param name="scriptforField"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="groupCount"></param>
    /// <returns></returns>
    public string GetDisplayHtml(FieldMetaData field, string groupCount, out string scriptforField, string controlId = null, string fieldCount = null, string controlValue = null)
    {
      //Create html for template
      var fieldTemplateHtml = GetTemplateHtml(field, groupCount, controlId, fieldCount);

      //html for plus and minus buttons
      var plusButtonHtml = Image(true, field.FieldName, _imagePath, string.Format("{0}{1}{2}CssClass", field.FieldName, Constants.DynamicGroupCountString, groupCount));
      var minusButtonHtml = Image(false, field.FieldName, _imagePath, string.Format("{0}{1}{2}CssClass", field.FieldName, Constants.DynamicGroupCountString, groupCount));


      //get html for fields to be displayed, instances equal to min occurrences are added
      var minOccurrenceCount = 1;
      if (field.MinOccurrence > 1)
        minOccurrenceCount = field.MinOccurrence;
      var displayOccurrenceCount = minOccurrenceCount;

      //If field has values, instances equal to field value count are rendered
      if (field.FieldValues.Count > minOccurrenceCount)
          displayOccurrenceCount = field.FieldValues.Count;

      var controlHtml = new StringBuilder();
      var fieldValueList = new List<FieldValue>(field.FieldValues);
      for (var i = 1; i <= displayOccurrenceCount; i++)
      {
        //In case of edit, only one FieldValue object should be passed to metadata
        field.FieldValues.Clear();
        int? fieldValueCount = null;
        var fieldControlValue = new FieldValue();
        if (fieldValueList.Count > i - 1 && fieldValueList[i - 1] != null)
        {
          fieldValueCount = i - 1;
          fieldControlValue = fieldValueList[fieldValueCount.Value];
        }
       
        field.FieldValues.Add(fieldControlValue);
        //Get html for field
        var fieldHtml = string.Format("{0}", GetMultiOccurrenceFieldControl(field, groupCount, controlId ?? field.FieldName, i.ToString()));

        //For first instance add plus button
        if (i == 1)
          fieldHtml += plusButtonHtml;
        else
        {
          //In case of edit, if entered values are more than min occurrence of field, add minus button for instances more than min occurrence count
          if (i > minOccurrenceCount)
            fieldHtml += minusButtonHtml;
        }
        if (field.HasAttributes)
        {
          var cssClassName = FieldwithAttribute.GetFieldwithAttributeCssClassName(field);
          controlHtml.AppendFormat(GetOuterDivHtml(field, cssClassName, string.Format("{0}{1}{2}_template{3}", field.FieldName, Constants.DynamicGroupCountString, groupCount, i)), fieldHtml);
        }
        else
          controlHtml.AppendFormat(GetOuterDivHtml(field, null, string.Format("{0}{1}{2}_template{3}", field.FieldName, Constants.DynamicGroupCountString, groupCount, i)), fieldHtml);
      }
      //Add fieldValue collection to field metadata.
      field.FieldValues.Clear();
      field.FieldValues.AddRange(fieldValueList);

      //Generate script to add/remove fields 
      scriptforField = GenerateMultiOccurrenceDynamicFieldScript(field.FieldName, _templateDivName, fieldTemplateHtml, displayOccurrenceCount, field.MaxOccurrence, groupCount, field.HasAttributes, _scriptForCloneSubFields);
      //Append scripts for subfields[e.g.scripts to initialize auto complete]
      scriptforField += _scriptForSubFields;
      return controlHtml.ToString();
    }

    /// <summary>
    /// Generate template for field of type multiple occurrence for group. This template is added in template of Group
    /// </summary>
    /// <param name="field"></param>
    /// <param name="scriptforField"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="groupCount"></param>
    /// <returns></returns>
    public string GetTemplateHtmlForGroup(FieldMetaData field, string groupCount, out string scriptforField, string controlId = null, string fieldCount = null, string controlValue = null)
    {
      //Create html for template
      var fieldTemplateHtml = GetTemplateHtml(field, groupCount, controlId, fieldCount);

      //html for plus and minus buttons
      var plusButtonHtml = Image(true, field.FieldName, _imagePath, string.Format("{0}CssClass", field.FieldName));
      //get html for fields to be displayed, instances equal to min occurrence are added

      var minOccurrenceCount = 1;
      if (field.MinOccurrence > 1)
        minOccurrenceCount = field.MinOccurrence;
      var displayOccurrenceCount = minOccurrenceCount;
      
      var controlHtml = new StringBuilder();
      for (var i = 1; i <= displayOccurrenceCount; i++)
      {
        var fieldHtml = string.Format("{0}", GetMultiOccurrenceFieldControl(field, groupCount, controlId ?? field.Id.ToString(), i.ToString(), null, true));

        if (i == 1)
          fieldHtml += plusButtonHtml;
        if (field.HasAttributes)
        {
          var cssClassName = FieldwithAttribute.GetFieldwithAttributeCssClassName(field);
          controlHtml.AppendFormat(GetOuterDivHtml(field, cssClassName, string.Format("{0}{1}{2}_template{3}", field.FieldName, Constants.DynamicGroupCountString, groupCount, i)), fieldHtml);
        }
        else
          controlHtml.AppendFormat(GetOuterDivHtml(field, null, string.Format("{0}{1}{2}_template{3}", field.FieldName, Constants.DynamicGroupCountString, groupCount, i)), fieldHtml);
      }
      
      scriptforField = _scriptForSubFields;
      return controlHtml.ToString();
    }

    /// <summary>
    /// Get html for template, template is used to clone field
    /// </summary>
    /// <param name="field"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="groupCount"></param>
    /// <returns></returns>
    public string GetTemplateHtml(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null)
    {
      //Create html for template
      var templateHtml = GetMultiOccurrenceFieldControl(field, groupCount, controlId, fieldCount, null, true);

      //html for plus and minus buttons
      var minusButtonHtml = Image(false, field.FieldName, _imagePath, string.Format("{0}CssClass", field.FieldName));

      //Get html that will be added in hidden template div. This template will be used to replicate when clicked on plus button
      var fieldTemplateHtml = string.Empty;
      if (field.HasAttributes)
      {
        var cssClassName = FieldwithAttribute.GetFieldwithAttributeCssClassName(field);
        fieldTemplateHtml = string.Format(GetOuterDivHtml(field, cssClassName, string.Format("{0}{1}{2}_template", field.FieldName, Constants.DynamicGroupCountString, groupCount)), string.Format("{0}{1}", templateHtml, minusButtonHtml));
      }
      else
        fieldTemplateHtml = string.Format(GetOuterDivHtml(field, null, string.Format("{0}{1}{2}_template", field.FieldName, Constants.DynamicGroupCountString, groupCount)), string.Format("{0}{1}", templateHtml, minusButtonHtml));
     
      return fieldTemplateHtml;
    }

    /// <summary>
    /// Generate script for dynamic field of type multiple occurrence
    /// </summary>
    /// <param name="controlId">control Id</param>
    /// <param name="templateDivId">Div id where template is added</param>
    /// <param name="templateHtml">html of template</param>
    /// <param name="minCount">min occurrence number</param>
    /// <param name="maxCount">max occurrence number</param>
    /// <param name="hasAttributes">Field has Attributes or not</param>
    /// <param name="scriptForCloneSubFields">Field has Attributes or not</param>
    /// <returns></returns>
    public string GenerateMultiOccurrenceDynamicFieldScript(string controlId, string templateDivId, string templateHtml, int minCount, int maxCount, string groupCount, bool hasAttributes = false, string scriptForCloneSubFields = null)
    {
      templateHtml = templateHtml.Replace("'", "\\'");
      var groupCountString = string.Format("{0}{1}", Constants.DynamicGroupCountString, groupCount);
      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>\n");
      sb.AppendFormat("var {0}{2}Count = 0; var {0}{2}Array = []; var {0}{2}_fieldTemplateName = \"{0}{2}_template\"; var {0}{2}_MaxCount = {1};", controlId, maxCount, groupCountString);
      if(groupCount == "1")
      {
        sb.Append("$(document).ready(function () {\n");

        sb.AppendFormat("$('#{0}').append('{1}');", templateDivId, templateHtml);
        sb.AppendFormat("$('.{0}{1}CssClass').live('click', {0}{1}_AppendRemoveTemplate);", controlId, groupCountString);
        for (var i = 1; i <= minCount; i++)
        {
          sb.AppendFormat("{0}{2}Array.push({1});", controlId, i, groupCountString);
        }
        sb.AppendFormat("{0}{2}Count = {1};", controlId, minCount, groupCountString);
        sb.AppendFormat("if ({0}{1}Array.length >= {0}{1}_MaxCount) ", controlId, groupCountString);
        sb.Append("{");
        sb.AppendFormat("$('#' + {0}{1}_fieldTemplateName + '1').find('img').hide();", controlId, groupCountString);
        sb.Append(" }");
        sb.Append("});\n");
      }
      else
      {
       var initializeScript = new StringBuilder();
       initializeScript.AppendFormat("$('.{0}{1}CssClass').live('click', {0}{1}_AppendRemoveTemplate);", controlId, groupCountString);
       for (var i = 1; i <= minCount; i++)
       {
         initializeScript.AppendFormat("{0}{2}Array.push({1});", controlId, i, groupCountString);
       }
       initializeScript.AppendFormat("{0}{2}Count = {1};", controlId, minCount, groupCountString);
       initializeScript.AppendFormat("if ({0}{1}Array.length >= {0}{1}_MaxCount) ", controlId, groupCountString);
       initializeScript.Append("{");
       initializeScript.AppendFormat("$('#' + {0}{1}_fieldTemplateName + '1').find('img').hide();", controlId, groupCountString);
       initializeScript.Append(" }");
       ScriptToInitializeSubFields += initializeScript.ToString();
      }

      sb.AppendFormat("function {0}{1}_AppendRemoveTemplate()", controlId, groupCountString);
      sb.Append("{");
      sb.Append("if(this.name == 'plus') {");
      sb.AppendFormat("var divTemplate = $('#{0}{1}_template').clone(true);", controlId, Constants.DynamicGroupCountString + "1");
      sb.Append("var template = divTemplate.get(0);");
      sb.AppendFormat("{0}{1}Count++;", controlId, groupCountString);
      sb.AppendFormat("$(template).attr('id', $(template).attr('id').replace('{2}{3}', '{1}') + {0}{1}Count);", controlId, groupCountString, Constants.DynamicGroupCountString, "1");

      //If field has attributes it has 2 div inside template div
      sb.Append(hasAttributes
                  ? "$(template).children().children().each(function (i) {"
                  : "$(template).children().each(function (i) {");

      sb.Append("var currentElem = $(this);");

      sb.AppendFormat("$(currentElem).attr('id', currentElem.attr('id').replace('{0}{1}','{0}{2}') + {3}{0}{2}Count);\n", Constants.DynamicGroupCountString, "1", groupCount, controlId);
      sb.AppendFormat("if (currentElem.is('img'))  currentElem.bind('click', {0}{1}_AppendRemoveTemplate);", controlId, groupCountString);
      sb.Append("else {");
      sb.AppendFormat("$(currentElem).attr('name', currentElem.attr('id'));", Constants.DynamicGroupCountString, "1", groupCount, controlId);
      sb.Append("}");
      sb.Append("\n     });");
      sb.AppendFormat("{0}{1}Array.push({0}{1}Count);", controlId, groupCountString);
      sb.Append("var button1 = $('img', template).get(0);");
      sb.AppendFormat("$(button1).addClass(\"{0}{1}CssClass\");", controlId, groupCountString);
      sb.AppendFormat("var divId = {0}{1}Array[{0}{1}Array.length - 2];", controlId, groupCountString);
      sb.AppendFormat("divTemplate.insertAfter('#{0}{1}_template' + divId);", controlId, groupCountString);

      sb.AppendFormat("if ({0}{1}Array.length >= {0}{1}_MaxCount) ", controlId, groupCountString);
      sb.Append("{");
      sb.AppendFormat("$('#' + {0}{1}_fieldTemplateName + '1').find('img').hide();", controlId, groupCountString);
      sb.Append(" }");
      if (!string.IsNullOrEmpty(scriptForCloneSubFields))
        sb.AppendFormat(scriptForCloneSubFields, controlId, groupCountString);
      sb.Append("} else if (this.name == 'minus') { ");
      sb.AppendFormat("var id = this.parentNode.parentNode.id.substring({0}{1}_fieldTemplateName.length);", controlId, groupCountString);
      sb.AppendFormat("var i; for (i = 0; i < {0}{1}Array.length; i++) ", controlId, groupCountString);
      sb.Append("{");
      sb.AppendFormat("if ({0}{1}Array[i] == id) ", controlId, groupCountString);
      sb.Append("{");
      sb.AppendFormat("{0}{1}Array.splice(i, 1); \n break;", controlId, groupCountString);
      sb.Append("} \n }");
      sb.AppendFormat("if ({0}{1}Array.length - 1 < {0}{1}_MaxCount)", controlId, groupCountString);
      sb.Append("{");
      sb.AppendFormat("$('#' + {0}{1}_fieldTemplateName + '1').find('img').show();", controlId, groupCountString);
      sb.Append("}");
      sb.Append("$(this.parentNode.parentNode).remove();");
      sb.Append("} }");


      sb.Append("</script>\n");
      sb.Replace(Environment.NewLine, "");

      return sb.ToString();
    }

    /// <summary>
    /// Get html and script for given field metadata 
    /// </summary>
    /// <param name="field"></param>
    /// <param name="groupCount"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    protected MvcHtmlString GetMultiOccurrenceFieldControl(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      if (field.ControlType == ControlType.AutoComplete || field.ControlType == ControlType.EditableAutoComplete)
      {
        
        var generateMultipleOccScript = false;
        if(isTemplate.HasValue && isTemplate.Value)
          generateMultipleOccScript = true;
        var controlAutoComplete = new AutoComplete(_html, _actionUrl, generateMultipleOccScript);
        if (field.ControlType == ControlType.EditableAutoComplete)
          controlAutoComplete = new EditableAutoComplete(_html, _actionUrl, generateMultipleOccScript);
        string scriptForField;
        //Get html for auto complete control
        var fieldHtml = controlAutoComplete.GetHtml(field, groupCount, out scriptForField, controlId, fieldCount, controlValue, isTemplate);

        //Script to initialize auto complete for field in html
        _scriptForSubFields += scriptForField;
        ScriptToInitializeSubFields += controlAutoComplete.ScriptToInitializeAutocomplete;

        //Script to initialize auto complete for field when its cloned. This script is added in script to clone template for multiple occ fields
        if (generateMultipleOccScript)
          _scriptForCloneSubFields += controlAutoComplete.ScriptForMultipleOccurrence;
        return MvcHtmlString.Create(fieldHtml);
      }
      else if(field.HasAttributes)
      {
        var generateMultiOccScript = false;
        if (isTemplate.HasValue && isTemplate.Value)
          generateMultiOccScript = true;
        var controlFieldWithAttribute = new FieldwithAttribute(_html, _actionUrl, generateMultiOccScript);
        controlFieldWithAttribute.IsParentMultiOccurrence = true;
        string scriptForField;
        //Get html for field with attributes
        var fieldwithAttrHtml = string.Empty;
        fieldwithAttrHtml = isTemplate.HasValue ? controlFieldWithAttribute.GetTemplateHtmlForGroup(field, groupCount, out scriptForField, controlId, fieldCount, controlValue) : controlFieldWithAttribute.GetDisplayHtml(field, groupCount, out scriptForField, controlId, fieldCount, controlValue);
        _scriptForSubFields += scriptForField;
        ScriptToInitializeSubFields += controlFieldWithAttribute.ScriptToInitializeSubFields;

        //Script for subfield when its cloned. This script is added in script to clone template for multiple occ fields
        if (generateMultiOccScript)
          _scriptForCloneSubFields += controlFieldWithAttribute.ScriptForMultipleOccurrence;

        return MvcHtmlString.Create(fieldwithAttrHtml);
      }
      else
      {
        return GetFieldControl(field, groupCount, _actionUrl, controlId, fieldCount, controlValue, isTemplate);
      }
    }

  }
}
