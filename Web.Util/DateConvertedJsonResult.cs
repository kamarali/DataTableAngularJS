using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Iata.IS.Web.Util
{
  public class DateConvertedJsonResult : JsonResult
  {
    public override void ExecuteResult(ControllerContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }
      HttpResponseBase response = context.HttpContext.Response;
      if (!string.IsNullOrEmpty(ContentType))
      {
        response.ContentType = ContentType;
      }
      else
      {
        response.ContentType = "application/json";
      }
      if (this.ContentEncoding != null)
      {
        response.ContentEncoding = this.ContentEncoding;
      }
      if (this.Data != null)
      {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        serializer.RegisterConverters(new[] { new DateTimeJsonConverter() });
        response.Write(serializer.Serialize(this.Data));
      }
    }

  }
}
