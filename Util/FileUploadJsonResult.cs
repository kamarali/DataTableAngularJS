using System;
using System.Web.Mvc;

namespace Iata.IS.Web.Util
{
  /// <summary>
  /// Class used to return attachment upload result 
  /// </summary>
  public class FileUploadJsonResult : JsonResult
  {
    public override void ExecuteResult(ControllerContext context)
    {
      ContentType = "text/html";
      context.HttpContext.Response.Write("<textarea>");
      base.ExecuteResult(context);
      context.HttpContext.Response.Write("</textarea>");
    }
  }
}
