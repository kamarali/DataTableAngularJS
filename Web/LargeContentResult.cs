using System;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Iata.IS.Web
{
  public class LargeContentResult : ContentResult
  {
    const string JsonRequestGetNotAllowed = "This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.";
    public LargeContentResult()
    {
      MaxJsonLength = 1024000;
      RecursionLimit = 100;
    }

    public int MaxJsonLength { get; set; }
    public int RecursionLimit { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }
      if (/*conRequestBehavior == JsonRequestBehavior.DenyGet && */
          String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
      {
        throw new InvalidOperationException(JsonRequestGetNotAllowed);
      }

      var response = context.HttpContext.Response;

      response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";

      if (ContentEncoding != null)
      {
        response.ContentEncoding = ContentEncoding;
      }
      if (Content == null)
      {
        return;
      }
      //var serializer = new JavaScriptSerializer { MaxJsonLength = MaxJsonLength, RecursionLimit = RecursionLimit };
      response.Write(Content);
    }
  }
}

