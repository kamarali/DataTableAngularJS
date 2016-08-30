using System.Web.Mvc;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class HtmlHelperExtensions
  {
    public static MvcHtmlString LinkButton(this HtmlHelper html, string buttonText, string actionUrl)
    {
      return MvcHtmlString.Create(string.Format("<a class=\"lwoul\" href='{1}'><span class=\"secondaryButtonText\">{0}</span></a>", buttonText, actionUrl));
    }
  }
}