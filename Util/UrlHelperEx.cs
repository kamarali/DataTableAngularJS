using System;
using System.Web;

namespace Iata.IS.Web.Util
{
  public class UrlHelperEx
  {
    /// <summary> 
    /// Converts the provided app-relative path into an absolute URL containing the full host name. forceSecureSockets works only for http URLs.
    /// </summary> 
    /// <param name="relativeUrl">App-Relative path</param>
    /// <param name="forceSecureSockets">Forces the URL to use https instead of http</param>
    /// <returns>Provided relativeUrl parameter as fully qualified Url</returns> 
    /// <example>~/path/to/foo to http://www.web.com/path/to/foo</example> 
    public static string ToAbsoluteUrl(string relativeUrl, bool forceSecureSockets = true)
    {
      if (string.IsNullOrEmpty(relativeUrl))
      {
        return relativeUrl;
      }

      if (HttpContext.Current == null)
      {
        return relativeUrl;
      }

      var url = HttpContext.Current.Request.Url;
      var port = url.Port != 80 ? (":" + url.Port) : string.Empty;
      var urlScheme = url.Scheme;
      
      if (forceSecureSockets)
      {
        if (url.Scheme.Equals("http", StringComparison.InvariantCultureIgnoreCase))
        {
          urlScheme = "https";
        }
      }

      return String.Format("{0}://{1}{2}{3}", urlScheme, url.Host, port, relativeUrl);

    }
  }
}