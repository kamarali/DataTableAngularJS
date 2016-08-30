using System.Web.Mvc;

namespace Iata.IS.Web.Areas.MU
{
  public class MUAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "MU";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "MU_default",
          "MU/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
