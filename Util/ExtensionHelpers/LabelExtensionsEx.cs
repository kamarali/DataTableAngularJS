using System.Web.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class LabelExtensionsEx
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString LabelForEx<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
    {
      return LabelHelper(html,
                         ModelMetadata.FromLambdaExpression(expression, html.ViewData),
                         ExpressionHelper.GetExpressionText(expression));
    }

    internal static MvcHtmlString LabelHelper(HtmlHelper html, ModelMetadata metadata, string htmlFieldName)
    {
      var labelText = string.Format("{0}:", (metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last()));
      if (String.IsNullOrEmpty(labelText))
      {
        return MvcHtmlString.Empty;
      }

      var tag = new TagBuilder("label");
      tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
      tag.SetInnerText(labelText);
      return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
    }
  }
}