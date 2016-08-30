using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Web.Util.DynamicFields.Base;

namespace Iata.IS.Web.Util.DynamicFields
{
  public class TextBox : FieldBase
  {
    public TextBox(HtmlHelper html)
      : base(html)
    {
      
    }

    /// <summary>
    /// Render html for control of type textbox.
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
        if(!(isTemplate.HasValue && isTemplate.Value))
          fieldValue = GetValue(field, controlValue);

      //Get html for textbox
      string textBoxHtml;
      if(_html != null)
        textBoxHtml = GetTextboxHtml(field, groupCount, fieldValue, controlId, fieldCount);
      else
        textBoxHtml = GetAjaxTextboxHtml(field, groupCount, fieldValue, controlId, fieldCount);

      var controlHtml = MvcHtmlString.Create(string.Format("{0}{1}", labelHtml, textBoxHtml));
      return controlHtml;
    }

    /// <summary>
    /// Create Html for textbox using HtmlHelper
    /// </summary>
    /// <param name="field"></param>
    /// <param name="fieldValue"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <returns></returns>
    protected string GetTextboxHtml(FieldMetaData field, string groupCount, string fieldValue, string controlId = null, string fieldCount = null)
    {
      //Get html attributes for control
      var fieldHtmlAttributes = GetHtmlAttributes(field);

      return _html.TextBox(GetControlId(field.FieldName, groupCount, controlId, fieldCount), fieldValue, fieldHtmlAttributes).ToString();
    }

    /// <summary>
    /// Create html for textbox using TagBuilder, used in ajax call to render html
    /// </summary>
    /// <param name="field"></param>
    /// <param name="fieldValue"></param>
    /// <param name="controlId"></param>
    /// <param name="fieldCount"></param>
    /// <returns></returns>
    protected string GetAjaxTextboxHtml(FieldMetaData field, string groupCount, string fieldValue, string controlId = null, string fieldCount = null)
    {
      //Get html attributes for control
      var htmlAttributes = GetAjaxHtmlAttributes(field);

      //Create tag builder and get html for textbox
      var tag = new TagBuilder(Constants.InputTagName);
      tag.MergeAttribute(Constants.TypeAttributeName, Constants.TextboxTypeName);
      var textBoxId = GetControlId(field.FieldName, groupCount, controlId, fieldCount);
      tag.MergeAttribute(Constants.IdAttributeName, textBoxId);
      tag.MergeAttribute(Constants.NameAttributeName, textBoxId);

      //Add html attributes
      tag.MergeAttributes(htmlAttributes);

      //Assign value
      tag.MergeAttribute(Constants.ValueAttributeName, fieldValue);
      return tag.ToString(TagRenderMode.Normal);
    }
  }
}
