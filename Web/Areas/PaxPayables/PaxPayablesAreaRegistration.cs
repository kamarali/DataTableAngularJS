using System.Web.Mvc;

namespace Iata.IS.Web.Areas.PaxPayables
{
  public class PaxPayablesAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "PaxPayables";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "PaxPayables_default",
          "PaxPayables/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
      context.MapRoute("PayablesManageSupportingDocuments", "PaxPayables/{billingType}/SupportingDoc/{action}", new { area = "PaxPayables", controller = "SupportingDoc", billingType = "Payables" });
    }
  }
}
