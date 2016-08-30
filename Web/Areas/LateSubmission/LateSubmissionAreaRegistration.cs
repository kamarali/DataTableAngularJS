using System.Web.Mvc;

namespace Iata.IS.Web.Areas.LateSubmission
{
  public class LateSubmissionAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "LateSubmission";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "LateSubmission_default",
          "LateSubmission/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}
