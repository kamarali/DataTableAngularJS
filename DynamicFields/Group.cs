using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Web.Util.DynamicFields.Base;
using System.Linq.Expressions;
using System.Linq;
using System;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class Group : FieldBase
  {
    private readonly string _actionUrl = string.Empty;
    private readonly string _imagePath = string.Empty;
    private readonly string _templateDivName = string.Empty;
    private string _scriptForSubFields = string.Empty;
    private string _scriptToInitializeSubFields = string.Empty;
    private bool _isMultipleOccurrenceGroup = false;
    public string ScriptForGroup = string.Empty;
    public string InitializeSubFieldsFunctionName = string.Empty;
    public string GroupDivId = string.Empty;

    public Group(HtmlHelper html, string actionUrl, string imagePath, string templateDivName)
      : base(html)
    {
      _actionUrl = actionUrl;
      _imagePath = imagePath;
      _templateDivName = templateDivName;
    }

    /// <summary>
    /// Creates controls for dynamic group with multiple occurrence  
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <param name="controlId">id for control</param>
    /// <param name="fieldCount">Field instance number in multiple occurrence field</param>
    /// <param name="controlValue">value to be displayed in control</param>
    /// <param name="isTemplate">Is html used in template</param>
    /// <returns></returns>
    public override MvcHtmlString ToMvcHtmlString(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      //html for plus and minus buttons
      var plusButtonHtml = Image(true, field.FieldName, _imagePath, string.Format("{0}CssClass", field.FieldName), Constants.AddGroupFunction);
      var minusButtonClickEvent = string.Empty;
      if (field.RequiredType == RequiredType.Optional)
        minusButtonClickEvent = Constants.RemoveOptionalGroupFunction;
      else
        minusButtonClickEvent = Constants.RemoveGroupFunction;
      var minusButtonHtml = Image(false, field.FieldName, _imagePath, string.Format("{0}CssClass", field.FieldName), minusButtonClickEvent);

      var expandImage = ExpandImage(field.FieldName, _imagePath);
      if (field.MaxOccurrence > 1)
      {
        _isMultipleOccurrenceGroup = true;

        MvcHtmlString imageHtml;
        if (field.RequiredType == RequiredType.Optional)
          imageHtml = minusButtonHtml;
        else
        {
          // check if any optional field exists within the group.
          var isOptionalFieldExists = field.SubFields.Count(subField => subField.RequiredTypeId == Convert.ToInt32(RequiredType.Optional)) > 0;

          if (isOptionalFieldExists) // Show Expand/Collapse button only when optional fields exist within the group.
          {
            imageHtml = MvcHtmlString.Create(string.Format("{0}{1}", expandImage, plusButtonHtml));
          }
          else
          {
            imageHtml = MvcHtmlString.Create(string.Format("{0}", plusButtonHtml));
          }
        }

        var displayHtml = string.Format("</div>{0}<div>", GetGroupHtml(field, "1", imageHtml));
        var scriptToInitializeSubFields = GetInitializeSubFieldScriptFunction(field.FieldName, groupCount, false);
        return MvcHtmlString.Create(string.Format("{0}\n{1}{2}", displayHtml, _scriptForSubFields, scriptToInitializeSubFields));
      }
      else
      {
        MvcHtmlString imageHtml = MvcHtmlString.Empty;
        if (field.RequiredType == RequiredType.Optional)
          imageHtml = minusButtonHtml;
        else
        {
          // check if any optional field exists within the group.
          var isOptionalFieldExists = field.SubFields.Count(subField => subField.RequiredTypeId == Convert.ToInt32(RequiredType.Optional)) > 0;

          if (isOptionalFieldExists) // Show Expand/Collapse button only when optional fields exist within the group.
          {
            imageHtml = expandImage;
          }
        }

        var displayHtml = string.Format("</div>{0}\n{1}<div>", GetGroupHtml(field, "1", imageHtml), _scriptForSubFields);
        var scriptToInitializeSubFields = GetInitializeSubFieldScriptFunction(field.FieldName, groupCount, false);
        return MvcHtmlString.Create(displayHtml + scriptToInitializeSubFields);
      }
    }

    /// <summary>
    /// Creates controls for dynamic group with multiple occurrence  
    /// </summary>
    /// <param name="field">field metadata</param>
    /// <param name="controlId">id for control</param>
    /// <param name="groupCount">Field instance number in multiple occurrence field</param>
    /// <returns></returns>
    public MvcHtmlString GetAjaxGroupHtml(FieldMetaData field, string groupCount, bool isOptionalGroup, string controlId = null)
    {
      //html for plus and minus buttons
      var minusButtonClickEvent = string.Empty;
      if (isOptionalGroup)
        minusButtonClickEvent = Constants.RemoveOptionalGroupFunction;
      else
        minusButtonClickEvent = Constants.RemoveGroupFunction;
      var minusButtonHtml = Image(false, field.FieldName, _imagePath, string.Format("{0}CssClass", field.FieldName), minusButtonClickEvent);

      var expandImage = ExpandImage(field.FieldName, _imagePath);
      _isMultipleOccurrenceGroup = true;
      var displayHtml = string.Empty;
      if (isOptionalGroup)
        displayHtml = string.Format("{0}", GetAjaxGroupHtml(field, groupCount, minusButtonHtml));
      else
      {
        // check if any optional field exists within the group.
        var isOptionalFieldExists = field.SubFields.Count(subField => subField.RequiredTypeId == Convert.ToInt32(RequiredType.Optional)) > 0;

        if (isOptionalFieldExists) // Show Expand/Collapse button only when optional fields exist within the group.
        displayHtml = string.Format("{0}", GetAjaxGroupHtml(field, groupCount, MvcHtmlString.Create(string.Format("{0}{1}", expandImage, minusButtonHtml))));
        else
        {
          displayHtml = string.Format("{0}", GetAjaxGroupHtml(field, groupCount, MvcHtmlString.Create(string.Format("{0}", minusButtonHtml))));
        }
      }
      var scriptToInitializeSubFields = GetInitializeSubFieldScriptFunction(field.FieldName, groupCount, true);

      return MvcHtmlString.Create(string.Format("{0}\n{1}{2}", displayHtml, _scriptForSubFields, scriptToInitializeSubFields));

    }

    /// <summary>
    /// Returns html for subfields in Group
    /// </summary>
    /// <param name="field"></param>
    /// <param name="imageHtml"></param>
    /// <param name="groupCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    public string GetAjaxGroupHtml(FieldMetaData field, string groupCount, MvcHtmlString imageHtml = null, string controlValue = null, bool? isTemplate = null)
    {
      var groupHtml = new StringBuilder();

      // Add hidden field for Group Metadata id.
      // Added parent metadata id in control id.
      var groupIdControlHtml = GetGroupIdHiddenField(GetGroupControlId(field.Id.ToString(), groupCount), field.Id.ToString());
      var controlHtml = new StringBuilder();

      // Get html for all the subfields in group.
      foreach (var subField in field.SubFields)
      {
        if (subField.FieldType == FieldType.Group)
        {
          controlHtml.Append(GetFieldControlForGroup(subField, groupCount, controlValue, isTemplate));
        }
        else
        {
          var fieldHtml = GetGroupFieldControl(subField, groupCount, subField.ParentId.ToString() + "_" + subField.Id.ToString(), controlValue, isTemplate);
          controlHtml.Append(fieldHtml);
        }
      }

      GroupDivId = string.Format("{0}_template{1}", field.FieldName, groupCount);
      ScriptForGroup += GenerateScriptForGroup(field, GroupDivId);
      if (groupCount.Equals("1"))
      {
        groupHtml.Append(string.Format("<div id=\"{0}_template{1}\"><h2>{2}  {3}{4}</h2>{5}</div>", field.FieldName, groupCount, field.DisplayText, imageHtml, groupIdControlHtml, controlHtml));
      }
      else
      {
        groupHtml.Append(string.Format("<div id=\"{0}_template{1}\"><h2>{2}  {3}{4}</h2>{5}</div>", field.FieldName, groupCount, field.DisplayText,
          imageHtml, groupIdControlHtml, controlHtml));
      }
      return groupHtml.ToString();
    }

    /// <summary>
    /// Returns html for subfields in Group
    /// </summary>
    /// <param name="field"></param>
    /// <param name="imageHtml"></param>
    /// <param name="groupCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    public string GetGroupHtml(FieldMetaData field, string groupCount, MvcHtmlString imageHtml = null, string controlValue = null, bool? isTemplate = null)
    {
      var minusButtonClickEvent = string.Empty;
      if (field.RequiredType == RequiredType.Optional)
        minusButtonClickEvent = Constants.RemoveOptionalGroupFunction;
      else
        minusButtonClickEvent = Constants.RemoveGroupFunction;
      var minusButtonHtml = Image(false, field.FieldName, _imagePath, string.Format("{0}CssClass", field.FieldName), minusButtonClickEvent);

      var expandImage = ExpandImage(field.FieldName, _imagePath);

      // If field values are present for the group metadata then get group html with field values.
      var noOfOccurrences = field.FieldValues.Count;

      // If field values are present for less than min. occurrences of the group then add group min. occurrences times.
      if (noOfOccurrences <= field.MinOccurrence)
      {
        noOfOccurrences = field.MinOccurrence == 0 ? 1 : field.MinOccurrence;
      }

      var groupHtml = new StringBuilder();
      for (var groupCounter = 1; groupCounter <= noOfOccurrences; groupCounter++)
      {
        FieldValue groupFieldValue = null;
        Guid? groupFieldValueId = null;
        if (field.FieldValues.Count >= groupCounter)
        {
          groupFieldValue = field.FieldValues[groupCounter - 1];
          groupFieldValueId = groupFieldValue.Id;
        }

        // Add hidden field for Group Metadata id.
        var groupIdControlHtml = GetGroupIdHiddenField(GetGroupControlId(field.Id.ToString(), groupCounter.ToString()), field.Id.ToString());
        var controlHtml = new StringBuilder();

        // Get html for all the subfields in group.
        FieldValue[] fieldValues = null;
        foreach (var subField in field.SubFields)
        {
          if (subField.FieldType == FieldType.Group)
          {
            // If more than 1 group within group is added then pass current group field value id to GetFieldControlForGroup.
            controlHtml.Append(GetFieldControlForGroup(subField, groupCounter.ToString(), null, null, groupFieldValueId));
          }
          else
          {
            fieldValues = subField.FieldValues.ToArray();

            // Filter out field values of only current instance of group.
            if (groupFieldValue != null)
            {
              if (groupFieldValue.Id == new Guid())
              {
                //In case of server side error, use group count to get field values for group
                UpdateFieldValues(subField, fieldValues.Where(fieldValue => fieldValue.ControlIdGroupCount == groupCounter.ToString()).ToList());
              }
              else
                UpdateFieldValues(subField, fieldValues.Where(fieldValue => fieldValue.ParentId == groupFieldValue.Id).ToList());
            }

            var fieldHtml = GetGroupFieldControl(subField, groupCounter.ToString(), subField.ParentId.ToString() + "_" + subField.Id.ToString());
            controlHtml.Append(fieldHtml);

            // Store all the field values again in field metadata.
            UpdateFieldValues(subField, fieldValues);
          }
        }

        GroupDivId = string.Format("{0}_template{1}", field.FieldName, groupCounter);
        ScriptForGroup += GenerateScriptForGroup(field, GroupDivId);
        if (groupCounter == 1)
        {
          groupHtml.Append(string.Format("<div id=\"{0}_template{1}\"><h2>{2}  {3}{4}</h2>{5}</div>", field.FieldName, groupCounter, field.DisplayText, imageHtml, groupIdControlHtml, controlHtml));
        }
        else
        {
          // check if any optional field exists within the group.
          var isOptionalFieldExists = field.SubFields.Count(subField => subField.RequiredTypeId == Convert.ToInt32(RequiredType.Optional)) > 0;
          MvcHtmlString minusButtonsForGroup;
          if(isOptionalFieldExists) // Show Expand/Collapse button only when optional fields exist within the group.
          //Display collapse/expand button for required dynamic groups only
            minusButtonsForGroup = MvcHtmlString.Create(string.Format("{0}{1}", expandImage, minusButtonHtml));
          else
          {
            minusButtonsForGroup = MvcHtmlString.Create(string.Format("{0}", minusButtonHtml));
          }

          if (field.RequiredType == RequiredType.Optional)
            minusButtonsForGroup = minusButtonHtml;

          groupHtml.Append(string.Format("<div id=\"{0}_template{1}\"><h2>{2}  {3}{4}</h2>{5}</div>", field.FieldName, groupCounter, field.DisplayText,
            minusButtonsForGroup, groupIdControlHtml, controlHtml));
        }
      }
      return groupHtml.ToString();
    }

    /// <summary>
    /// Updates the field values in Field Metadata.
    /// </summary>
    /// <param name="fieldMetaData">The field meta data.</param>
    /// <param name="fieldValues">The field values.</param>
    private static void UpdateFieldValues(FieldMetaData fieldMetaData, IList<FieldValue> fieldValues)
    {
      fieldMetaData.FieldValues.Clear();
      var noOfFieldValues = fieldValues.Count();
      for (var i = 0; i < noOfFieldValues; i++)
      {
        fieldMetaData.FieldValues.Add(fieldValues[i]);
      }
    }

    /// <summary>
    /// Get Fields for Group within Group
    /// </summary>
    /// <param name="field"></param>
    /// <param name="actionUrl"></param>
    /// <param name="groupCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    protected string GetFieldControlForGroup(FieldMetaData field, string groupCount, string controlValue = null, bool? isTemplate = null, Guid? groupFieldValueId = null)
    {
      var controlHtml = new StringBuilder();

      var groupIdControlHtml = GetGroupIdHiddenField(GetGroupControlId(field.ParentId.ToString() + "_" + field.Id.ToString(), groupCount.ToString()), field.Id.ToString());

      controlHtml.Append(groupIdControlHtml);

      var currentGroupFieldValues = field.FieldValues.Where(fv => fv.ParentId == groupFieldValueId).ToList();
      FieldValue currentGroupFieldValue = null;
      Guid? currentGroupFieldValueId = null;
      if (currentGroupFieldValues.Count > 0)
      {
        currentGroupFieldValueId = currentGroupFieldValues[0].Id;
        currentGroupFieldValue = currentGroupFieldValues[0];
      }

      FieldValue[] fieldValues = null;
      foreach (var subField in field.SubFields)
      {

        fieldValues = subField.FieldValues.ToArray();

        // Filter out field values of only current instance of group.
        if (currentGroupFieldValue != null)
        {
          if (currentGroupFieldValue.Id == new Guid())
          {
            //In case of server side error, use group count to get field values for group
            UpdateFieldValues(subField, fieldValues.Where(fieldValue => fieldValue.ParentId == currentGroupFieldValueId).ToList());
          }
          else
            UpdateFieldValues(subField, fieldValues.Where(fieldValue => fieldValue.ParentId == currentGroupFieldValueId).ToList());
        }

        var fieldHtml = GetGroupFieldControl(subField, groupCount, subField.ParentId + "_" + subField.Id, controlValue, isTemplate);
        controlHtml.Append(fieldHtml);

        // Store all the field values again in field metadata.
        UpdateFieldValues(subField, fieldValues);
      }
      return controlHtml.ToString();
    }

    /// <summary>
    /// Get field html and scripts for given field
    /// </summary>
    /// <param name="field"></param>
    /// <param name="actionUrl"></param>
    /// <param name="controlId"></param>
    /// <param name="groupCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    protected MvcHtmlString GetGroupFieldControl(FieldMetaData field, string groupCount, string controlId = null, string controlValue = null, bool? isTemplate = null)
    {
      if (field.IsMultipleOccurrence)
      {
        var controlMultiOccerrence = new FieldMultipleOccurrence(_html, _actionUrl, _imagePath, _templateDivName);
        string scriptForField;
        var fieldHtml = string.Empty;
        //Get html for field
        if (isTemplate.HasValue)
          fieldHtml = controlMultiOccerrence.GetTemplateHtmlForGroup(field, groupCount, out scriptForField, controlId, null, controlValue);
        else
          fieldHtml = controlMultiOccerrence.GetDisplayHtml(field, groupCount, out scriptForField, controlId, null, controlValue);

        //Script for subfields
        _scriptForSubFields += scriptForField;
        _scriptToInitializeSubFields += controlMultiOccerrence.ScriptToInitializeSubFields;

        return MvcHtmlString.Create(fieldHtml);
      }
      else if (field.HasAttributes)
      {
        var controlFieldWithAttribute = new FieldwithAttribute(_html, _actionUrl, false);
        string scriptForField;
        var fieldwithAttrHtml = string.Empty;
        //Render html for field
        if (isTemplate.HasValue)
          fieldwithAttrHtml = controlFieldWithAttribute.GetTemplateHtmlForGroup(field, groupCount, out scriptForField, controlId, null, controlValue);
        else
          fieldwithAttrHtml = controlFieldWithAttribute.GetDisplayHtml(field, groupCount, out scriptForField, controlId, null, controlValue);
        //Append script for subfields
        _scriptForSubFields += scriptForField;
        _scriptToInitializeSubFields += controlFieldWithAttribute.ScriptToInitializeSubFields;

        //Script for subfields that is to be added in script to clone parent
        //_scriptForCloneSubFields += controlFieldWithAttribute.ScriptForMultipleOccurrence;

        return MvcHtmlString.Create(fieldwithAttrHtml);
      }
      else if (field.ControlType == ControlType.AutoComplete || field.ControlType == ControlType.EditableAutoComplete)
      {
        var controlAutoComplete = new AutoComplete(_html, _actionUrl, false);
        if (field.ControlType == ControlType.EditableAutoComplete)
          controlAutoComplete = new EditableAutoComplete(_html, _actionUrl, false);
        string scriptForField;
        //Get html for autocomplete field
        var fieldHtml = controlAutoComplete.GetHtml(field, groupCount, out scriptForField, controlId, null, controlValue, isTemplate);
        //Add script to initialise autocomplete for rendered instances which will be displayed on UI
        _scriptForSubFields += scriptForField;
        _scriptToInitializeSubFields += controlAutoComplete.ScriptToInitializeAutocomplete;

        return MvcHtmlString.Create(string.Format(GetOuterDivHtml(field), fieldHtml));
      }
      else
      {
        var fieldHtml = GetFieldControl(field, groupCount, _actionUrl, controlId, null, controlValue, isTemplate);
        return MvcHtmlString.Create(string.Format(GetOuterDivHtml(field), fieldHtml));
      }
    }

    /// <summary>
    /// Generate script to initialise array for group information
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    public static string GenerateScriptForGroup(FieldMetaData field, string divId)
    {
      return string.Format(Constants.InitialiseGroupScriptFunction, field.Id, field.MaxOccurrence, divId);
    }

    public string GetGroupIdHiddenField(string controlId, string controlValue)
    {
      if (_html != null)
      {
        return _html.Hidden(controlId, controlValue).ToString();
      }
      else
        return GetAjaxHiddenHtml(controlId, controlValue);
    }

    public string GetInitializeSubFieldScriptFunction(string fieldId, string groupCount, bool isAjaxCall)
    {
      var sb = new StringBuilder();
      if (_scriptToInitializeSubFields.Length > 0)
      {
        sb.Append("<script type='text/javascript'>");
        if (isAjaxCall)
        {
          InitializeSubFieldsFunctionName = string.Format("{0}{1}{2}{3}{4}", ControlIdConstants.DynamicFieldPrefix,
                                                          fieldId, Constants.DynamicGroupCountString, groupCount,
                                                          Constants.MultipleGroupInitializeSubFieldFunctionNameSuffix);
          sb.AppendFormat("function {0}()", InitializeSubFieldsFunctionName);
        }
        else
          sb.Append("$(document).ready(function ()");
        sb.Append("{");
        sb.Append(_scriptToInitializeSubFields);
        sb.Append("}");
        if (!isAjaxCall)
          sb.Append(");");
        sb.Append("</script>");
      }
      return sb.ToString();
    }

  }
}
