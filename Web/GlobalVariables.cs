using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Iata.IS.Web
{
  public static class GlobalVariables
  {
    public static int DefaultPageSize
    {
      get { return Convert.ToInt32( HttpContext.Current.Application["DefaultPageSize"]); }
      set
      {
        HttpContext.Current.Application["DefaultPageSize"] =
          AdminSystem.SystemParameters.Instance.UIParameters.DefaultPageSize;
      }
    }
    public static string PageSizeOptions
    {
      get { return HttpContext.Current.Application["PageSizeOptions"] != null? HttpContext.Current.Application["PageSizeOptions"].ToString():string.Empty; }
      set
      {
        HttpContext.Current.Application["PageSizeOptions"] =
          AdminSystem.SystemParameters.Instance.UIParameters.PageSizeOptions;
      }
    }
  }
}