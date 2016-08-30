using System.Web.Mvc;

namespace Iata.IS.Web.Areas.ISOps
{
  public class ISOpsAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "ISOps";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "ISOps_default",
          "ISOps/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
