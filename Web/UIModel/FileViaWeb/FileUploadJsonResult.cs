using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Iata.IS.Web.UIModel.FileViaWeb
{
  public class FileUploadJsonResult : JsonResult
  {


    public override void ExecuteResult(ControllerContext context)
    {

      this.ContentType = "text/html";

      context.HttpContext.Response.Write("<textarea>");

      base.ExecuteResult(context);

      context.HttpContext.Response.Write("</textarea>");

    }


  }
}