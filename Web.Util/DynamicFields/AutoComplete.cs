using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class AutoComplete : TextBox
  {
    protected readonly string _actionUrl = string.Empty;
    protected readonly bool _generateScriptForMultipleOccurrence = false;
    public string ScriptForMultipleOccurrence = string.Empty;
    public string ScriptToInitializeAutocomplete = string.Empty;
    public bool IsParentMultiOccurrence;

    public AutoComplete(HtmlHelper html, string actionUrl, bool generateScriptForMultipleOccurrence)
      : base(html)
    {
      _actionUrl = actionUrl;
      _generateScriptForMultipleOccurrence = generateScriptForMultipleOccurrence;
    }

    /// <summary>
    /// Creates Autocomplete control for dynamic field and sets its selected value.
    /// </summary>
    /// <param name="field"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    public override MvcHtmlString ToMvcHtmlString(FieldMetaData field, string groupCount, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      string scriptForControls;
      var controlHtml = GetHtml(field, groupCount, out scriptForControls, controlId, fieldCount, controlValue, isTemplate);
      //Generate script that will be called on document.ready
      //scriptForControls = GenerateDynamicFieldAutoCompleteScript(GetControlId(field.FieldName, controlId, fieldCount), _actionUrl, field.DataSourceId.ToString());

      return MvcHtmlString.Create(string.Format("{0}\n{1}", controlHtml, scriptForControls));
    }

    /// <summary>
    /// Get html and script for autocomplete field
    /// </summary>
    /// <param name="field"></param>
    /// <param name="scriptforField"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <param name="controlValue"></param>
    /// <param name="isTemplate"></param>
    /// <returns></returns>
    public string GetHtml(FieldMetaData field, string groupCount, out string scriptforField, string controlId = null, string fieldCount = null, string controlValue = null, bool? isTemplate = null)
    {
      //Create display test label
      var labelHtml = Label(field).ToHtmlString();

      //Get value to populate in control, applicable in case of edit
      var fieldValue = string.Empty;
      if (!(isTemplate.HasValue && isTemplate.Value))
        fieldValue = GetValue(field, controlValue);

      //Get html for textbox
      string autoCompleteHtml;
      if (_html != null)
        autoCompleteHtml = GetTextboxHtml(field, groupCount, fieldValue, controlId, fieldCount);
      else
        autoCompleteHtml = GetAjaxTextboxHtml(field, groupCount, fieldValue, controlId, fieldCount);
      
      scriptforField = string.Empty;
      //Generate script that will be called in template cloning method
      if (groupCount == "1")
      {
        scriptforField = GenerateDynamicFieldAutoCompleteScript(GetControlId(field.FieldName, groupCount, field.Id.ToString(), fieldCount), _actionUrl, field.DataSourceId.ToString());
      }
      if(isTemplate == null || (isTemplate.HasValue && isTemplate.Value != true))
        ScriptToInitializeAutocomplete = GenerateInitializeAutoCompleteScript(GetControlId(field.FieldName, "1", field.Id.ToString()), GetControlId(field.FieldName, groupCount, controlId, fieldCount));

      if (_generateScriptForMultipleOccurrence)
        ScriptForMultipleOccurrence = GenerateAutoCompleteScriptForMultipleOccurrence(GetControlId(field.FieldName, "1", field.Id.ToString()), GetControlId(field.FieldName, groupCount, controlId));
      
      var controlHtml = string.Format("{0}{1}", labelHtml, autoCompleteHtml);
      return controlHtml;
    }
    
    /// <summary>
    /// Generate script to register dynamic field of type autocomplete
    /// </summary>
    /// <param name="controlId">control Id</param>
    /// <param name="actionUrl">Url to fetch data</param>
    /// <param name="extraParam1">Data source Id</param>
    /// <returns></returns>
    public virtual string GenerateDynamicFieldAutoCompleteScript(string controlId, string actionUrl, string extraParam1)
    {
      controlId = controlId.Replace("-", "");
      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>\n");
      sb.AppendFormat("function {0}{1}(caller)\n", Constants.AutoCompleteFunctionName, controlId);
      sb.Append("{");
      sb.AppendFormat("registerAutocomplete(caller, caller, \"{0}\", 0, true,null,\"{1}\");", actionUrl, extraParam1);
      sb.Append("}");
      sb.Append("</script>\n");

      return sb.ToString();
    }

    /// <summary>
    /// Generate script to register autocomplete for field. This will be used to generate script which will be 
    /// used to register autocomplete for control in template to clone multiple occurrence fields/ template of groups
    /// </summary>
    /// <param name="controlId"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    private string GenerateInitializeAutoCompleteScript(string fieldName, string controlId)
    {
      fieldName = fieldName.Replace("-", "");
      var sb = new StringBuilder();
      sb.AppendFormat("{0}{1}(\"{2}\");\n", Constants.AutoCompleteFunctionName, fieldName, controlId);
      return sb.ToString();
    }

    /// <summary>
    /// Generate script to initialise autocomplete when parent of field is of type multiple occurrence. This script will be called in script
    /// used to clone parent.
    /// </summary>
    /// <param name="controlId"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    private string GenerateAutoCompleteScriptForMultipleOccurrence(string fieldName, string controlId)
    {
      fieldName = fieldName.Replace("-", "");
      var sb = new StringBuilder();
      //sb.AppendFormat("if($(currentElem).attr(\"id\").indexOf('{0}') >= 0);", fieldName);
      sb.AppendFormat("{0}{1}(\"", Constants.AutoCompleteFunctionName, fieldName);
      sb.Append(controlId + "\" + {0}{1}Count);");
      return sb.ToString();
    }
  }
}
