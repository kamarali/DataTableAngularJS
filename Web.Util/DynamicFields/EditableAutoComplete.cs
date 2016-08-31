using System.Text;
using System.Web.Mvc;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class EditableAutoComplete : AutoComplete
  {
    public EditableAutoComplete(HtmlHelper html, string actionUrl, bool generateScriptForMultipleOccurrence)
      : base(html, actionUrl, generateScriptForMultipleOccurrence)
    {
    }

    /// <summary>
    /// Generate script to register dynamic field of type autocomplete
    /// </summary>
    /// <param name="controlId">control Id</param>
    /// <param name="actionUrl">Url to fetch data</param>
    /// <param name="extraParam1">Data source Id</param>
    /// <returns></returns>
    public override string GenerateDynamicFieldAutoCompleteScript(string controlId, string actionUrl, string extraParam1)
    {
      controlId = controlId.Replace("-", "");
      var sb = new StringBuilder();

      sb.Append("<script type='text/javascript'>\n");
      sb.AppendFormat("function {0}{1}(caller)\n", Constants.AutoCompleteFunctionName, controlId);
      sb.Append("{");
      sb.AppendFormat("registerAutocomplete(caller, caller, \"{0}\", 0, false,null,\"{1}\");", actionUrl, extraParam1);
      sb.Append("}");
      sb.Append("</script>\n");

      return sb.ToString();
    }
  }
}
